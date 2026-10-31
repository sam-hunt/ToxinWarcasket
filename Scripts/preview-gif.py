#!/usr/bin/env python3
"""Build the animated Workshop preview (.steamworkshop/Preview.gif) from the artist's Preview.mp4.

    python3 Scripts/preview-gif.py                      # .steamworkshop/Preview.mp4 -> .steamworkshop/Preview.gif
    python3 Scripts/preview-gif.py --work /tmp/preview  # keep the intermediates for inspection

Adapted from the family method (OverlordWarcasket Scripts/preview-gif.py), which composites the video's
hero onto About/Preview.png. That does not carry over here: this delivery (1920x1080 H.264, 12 fps padded
to 29.97, 6 s) is not a 3x blow-up of the PNG. Only the hero lines up with it (exactly 3x, offset +5, -6
video px); the title, hex, badge and background are re-laid out at other scales, and the ground fog
animates along with the hero, so a composite would seam wherever the two layouts meet. The GIF therefore
keeps the video's own framing. Nor does it need the family's tone curve: fitted over the aligned hero, it
came out within 3/255 of identity, so this delivery's codec did not shift the tones.

Steps, each with its intermediate left in --work:

1. frames/      ffmpeg extracts every frame losslessly (Krita's Windows build through wslpath when no
                `ffmpeg` is on PATH). The 12 fps animation is padded to 29.97 in a 3/2 cadence, so frames
                are taken one per 1/12 s slot (indices 0, 2, 5, 7, ...): 72 frames. Slotting by time rather
                than by change keeps the last slot, which barely moves, at its true duration.
2. mask.png     Pixels that ever change by more than 12/255 against frame 0 (at 640x360, box-filtered),
                opened 3 px, dilated 11 px, holes filled: 45% of the frame, the hero and the ground fog.
                Feathered 2 px for the composite; the binary mask tells the encoder which pixels may ever
                change.
3. c640/        Every frame downscaled with a Hamming filter (Lanczos rings on a 3x reduction, and LZW pays
                for that ringing every frame) and denoised in time: a pixel whose change to both
                neighbouring frames is below DENOISE is averaged with them. Outside the mask the frame is
                background.png, the mean of all 72 frames, which removes the codec's frame-to-frame noise
                from everything that does not move.
4. Preview.gif  One global 256-colour palette (libimagequant over three spread frames). Frame 0 is
                dithered outside the mask and not inside it, like every later frame, so frame 0 -> 1 shows
                no texture jump. Later frames carry only what changed, and a pixel keeps its last emitted
                colour until the truth has moved more than GATE from it. Outside the mask nothing ever
                changes. Delays run 8/8/9 cs so 72 frames take exactly 6.00 s.

Tuning record (640x360, 72 frames, 45% mask): GATE 0 2.98 MB, 17 1.11 MB, 18 1.07 MB, 19 1.03 MB
(shipped), 20 0.98 MB, 22 0.91 MB. The family cost model and the levers it rejected (dropped frames,
lossy LZW, a smaller palette, smaller frames) are in ../PREVIEW_GIF_FROM_MP4.md. The build is
deterministic: a rerun on the same inputs is byte-identical.

Requires Pillow built with libimagequant (Pillow 9.0's binding returns a one-colour palette for odd
sizes, so the palette is learned at 640 wide and the colour count asserted), gifsicle, and ffmpeg.
Nothing here ships; the MP4 stays outside the repo like the gameplay recordings.
"""
import argparse, glob, os, shutil, subprocess, sys, tempfile
from PIL import Image, ImageChops, ImageDraw, ImageFilter, ImageMath, ImageStat

SIZE = (640, 360)
MOTION_THRESHOLD = 12   # /255, against frame 0 at 640x360
DENOISE = 12            # /255: a moving-region pixel changing less than this to both neighbours is averaged
GATE = 19               # /255: a pixel keeps its emitted colour until the truth has moved further than this
DELAYS = (8, 8, 9)      # centiseconds, repeating: 72 frames = 600 cs
LIMIT = 1048576
KRITA_FFMPEG = '/mnt/c/Program Files (x86)/Steam/steamapps/common/Krita/krita/bin/ffmpeg.exe'


def log(msg):
    print(f'[preview-gif] {msg}', flush=True)


def max_channel_diff(a, b):
    d = ImageChops.difference(a, b)
    r, g, bl = d.split()
    return ImageChops.lighter(ImageChops.lighter(r, g), bl)


def extract_frames(mp4, work, ffmpeg):
    out = os.path.join(work, 'frames')
    if glob.glob(os.path.join(out, 'f*.png')):
        log(f'frames: reusing {out}')
    else:
        os.makedirs(out, exist_ok=True)
        if ffmpeg.lower().endswith('.exe'):
            win = lambda p: subprocess.check_output(['wslpath', '-w', p], text=True).strip()
            src, pattern = win(mp4), win(out) + '\\f%03d.png'
        else:
            src, pattern = mp4, os.path.join(out, 'f%03d.png')
        log(f'frames: {os.path.basename(ffmpeg)} -> {out}')
        subprocess.run([ffmpeg, '-v', 'error', '-y', '-i', src, '-an', '-fps_mode', 'passthrough',
                        '-pix_fmt', 'rgb24', pattern], check=True)
    paths = sorted(glob.glob(os.path.join(out, 'f*.png')))
    slots = [i * 5 // 2 for i in range(len(paths) * 2 // 5)]   # 0, 2, 5, 7, ...: one per 1/12 s
    frames = [Image.open(paths[i]).convert('RGB') for i in slots]
    # Each slot must be a fresh picture (the cadence holds) and its padding copies must not be.
    for i in slots[1:]:
        a, b = Image.open(paths[i - 1]).convert('RGB'), Image.open(paths[i]).convert('RGB')
        assert ImageStat.Stat(max_channel_diff(a.resize(SIZE, Image.BOX), b.resize(SIZE, Image.BOX))).mean[0] > 0.1 \
            or i == slots[-1], f'frame {i} is not a new picture; the 3/2 cadence assumption broke'
    log(f'frames: {len(frames)} slots of {len(paths)} frames')
    return frames


def build_mask(small, work):
    mx = Image.new('L', SIZE, 0)
    for im in small[1:]:
        mx = ImageChops.lighter(mx, max_channel_diff(im, small[0]))
    m = mx.point(lambda v: 255 if v > MOTION_THRESHOLD else 0)
    m = m.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.MaxFilter(11))
    inv = m.point(lambda v: 0 if v else 255)
    ImageDraw.floodfill(inv, (SIZE[0] - 1, 0), 128)
    holes = inv.point(lambda v: 255 if v == 255 else 0)
    binary = ImageChops.lighter(m, holes)
    feathered = binary.filter(ImageFilter.GaussianBlur(2))
    binary.save(os.path.join(work, 'mask.png'))
    cov = binary.histogram()[255] / (SIZE[0] * SIZE[1])
    log(f'mask: {cov:.0%} of the frame, bbox {binary.getbbox()}, {holes.histogram()[255]} hole px filled')
    return binary, feathered


def temporal_mean(frames):
    out = []
    for ch in range(3):
        acc = None
        for f in frames:
            c = f.getchannel(ch).convert('F')
            acc = c if acc is None else ImageMath.eval('a + b', a=acc, b=c)
        out.append(ImageMath.eval('convert(a / n + 0.5, "L")', a=acc, n=float(len(frames))))
    return Image.merge('RGB', out)


def composite(uniq, feathered, work):
    out = os.path.join(work, 'c640'); os.makedirs(out, exist_ok=True)
    small = [im.resize(SIZE, Image.HAMMING) for im in uniq]
    n, still = len(small), lambda a, b: max_channel_diff(a, b).point(lambda v: 255 if v < DENOISE else 0)
    denoised = []
    for i, c in enumerate(small):
        p, q = small[i - 1], small[(i + 1) % n]
        quiet = ImageChops.multiply(still(c, p), still(c, q))
        denoised.append(Image.composite(Image.blend(Image.blend(p, q, 0.5), c, 0.5), c, quiet))
    background = temporal_mean(small)
    background.save(os.path.join(work, 'background.png'))
    frames = []
    for i, h in enumerate(denoised):
        c = Image.composite(h, background, feathered)
        # Pillow keeps RGB as four bytes per pixel and composite() blends the mask into the padding byte,
        # which libimagequant then reads as alpha and builds a visibly worse palette from. A fresh buffer
        # pads with 255, exactly as the frame would reload from its PNG.
        c = Image.frombytes('RGB', c.size, c.tobytes())
        c.save(os.path.join(out, f'c{i:02d}.png')); frames.append(c)
    return frames


def encode(frames, binary, out_path, work):
    n = len(frames); W, H = SIZE
    sample = Image.new('RGB', (W, H * 3))
    for k, i in enumerate((0, n // 3, 2 * n // 3)):
        sample.paste(frames[i], (0, H * k))
    pal = sample.quantize(256, method=Image.LIBIMAGEQUANT)
    assert len(pal.getcolors(256)) > 8, 'libimagequant returned a collapsed palette'
    remap = lambda im: im.quantize(palette=pal, dither=Image.NONE)
    outside = binary.point(lambda v: 0 if v else 255)
    tmp = os.path.join(work, 'gif'); os.makedirs(tmp, exist_ok=True)
    paths, prev = [], None
    for i, f in enumerate(frames):
        if i == 0:
            fr = pal.crop((0, 0, W, H)); fr.paste(remap(f), (0, 0), binary)
        else:
            cand = remap(f)
            moved = max_channel_diff(cand.convert('RGB'), prev.convert('RGB')).point(lambda v: 255 if v > GATE else 0)
            keep = ImageChops.lighter(moved.point(lambda v: 255 - v), outside)
            fr = cand.copy(); fr.paste(prev, (0, 0), keep)
        p = os.path.join(tmp, f'f{i:03d}.gif'); fr.save(p, interlace=False); paths.append(p); prev = fr
    args = ['gifsicle', '-O3', '--no-warnings', '--loopcount=forever']
    for i, p in enumerate(paths):
        args += [f'-d{DELAYS[i % len(DELAYS)]}', p]
    subprocess.run(args + ['-o', out_path], check=True)
    return os.path.getsize(out_path)


def main():
    global GATE
    ap = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    ap.add_argument('--mp4', default='.steamworkshop/Preview.mp4')
    ap.add_argument('--out', default='.steamworkshop/Preview.gif')
    ap.add_argument('--work', help='intermediates directory (default: a fresh temp dir, removed on success)')
    ap.add_argument('--ffmpeg', default=shutil.which('ffmpeg') or KRITA_FFMPEG)
    ap.add_argument('--gate', type=int, default=GATE, help='tuning only; the shipped value is GATE')
    ap.add_argument('--force', action='store_true', help='write even over the limit (reference encodes)')
    a = ap.parse_args()
    GATE = a.gate
    work = a.work or tempfile.mkdtemp(prefix='preview-gif-')
    os.makedirs(work, exist_ok=True)
    uniq = extract_frames(a.mp4, work, a.ffmpeg)
    binary, feathered = build_mask([im.resize(SIZE, Image.BOX) for im in uniq], work)
    frames = composite(uniq, feathered, work)
    size = encode(frames, binary, a.out + '.tmp', work)
    if size >= LIMIT and not a.force:
        os.remove(a.out + '.tmp')
        sys.exit(f'[preview-gif] {size} bytes is over the {LIMIT} byte Workshop limit; nothing written')
    os.replace(a.out + '.tmp', a.out)
    log(f'wrote {a.out}: {size} bytes ({size / LIMIT:.1%} of the limit), {len(frames)} frames, gate {GATE}')
    if not a.work:
        shutil.rmtree(work)


if __name__ == '__main__':
    main()
