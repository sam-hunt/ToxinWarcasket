# Absorb Gas Sound Spec

The design of a sound for Absorb Gas (`TXWC_AbsorbGas`), written before the work. It is not mod
content. This spec assumes the vanilla tornado's roar works as a suction sound once it is quieter,
higher pitched and shorter. **Step 0 confirms that assumption in game; if the roar fails it, this
spec is void.** Engine facts come from the 1.7 `Assembly-CSharp.dll` decompile, read with
`ilspycmd`. The behaviour these values rely on is not expected to differ from 1.6. VEF facts come
from `../VanillaExpanded/VanillaExpandedFramework/Source/VEF/Abilities/`.

Once the sound lands, the def headers and the class comment are the record (per CLAUDE.md).
This file is the rationale and is not kept in sync with later tuning.

## Why the absorb needs a sound

Every other effect of the set has one. The jet plays `GasPack_Deploy`. The vent plays
`GasPack_Deploy` as it opens, then `ToxGasReleasing`'s `GasReleasing` hiss. The burst plays the
ToxGas damage def's `Explosion_Smoke`. The absorb is silent for its whole draw (`durationTime`
300 ticks, a mod setting). It is the one effect that pulls gas in rather than letting it out.

## Source clip

The clip is `Misc/Tornado/TornadoLoop`, played by Core's `Tornado` SoundDef
(`World_Sustainers_Misc.xml`). The `Tornado` thing spawns that sustainer
(`MaintenanceType.PerTick`), maintains it every tick and drives `info.volumeFactor` through its
own fade-in/out. Its def:

| Field | Tornado | Problem for the absorb |
| --- | --- | --- |
| `volumeRange` | 70 | Far over the set's other sounds (`GasReleasing` 35, `GasPack_Deploy` 40). |
| `distRange` | 100~200 | Audible across most of a map. |
| `sustainAttack` / `sustainRelease` | 2.5 / 2.5 | The 2.5 s attack is half the 5 s draw. It also never applies to the first sample, because `sustainSkipFirstAttack` defaults to true. |
| `maxSimultaneous` | 1 | Fine for one tornado; two pilots would steal each other's voice. |

A new SoundDef reuses the clip by `clipPath`, which needs no new audio file in the mod.
`Tornado` itself cannot be reused: its volume, range and voice cap are in the def, not in a
`SoundInfo` the caller can change.

## Step 0: in-game check (before any code)

Spawn a `Tornado` with dev mode's Spawn thing, keep the camera on it and turn the game's sound
volume down until the roar sits under the colony's ambient level. The question is whether a
quiet roar, pitched up, reads as air being drawn into a respirator. A raised pitch shortens the
roar and makes it sound smaller (Unity resamples, so pitch and tempo move together). Unity's own
volume slider cannot preview the pitch shift. If the roar stays plausible at low volume, go on.
If it reads as weather, stop and look for another clip.

## The SoundDef

New file `1.6/Defs/SoundDefs/AbsorbGasDraw.xml`, one def per file:

```xml
<SoundDef>
  <defName>TXWC_AbsorbGasDraw</defName>
  <sustain>true</sustain>
  <context>MapOnly</context>
  <maxSimultaneous>2</maxSimultaneous>
  <priorityMode>PrioritizeNearest</priorityMode>
  <sustainFadeoutTime>0.6</sustainFadeoutTime>
  <subSounds>
    <li>
      <grains>
        <li Class="AudioGrain_Clip">
          <clipPath>Misc/Tornado/TornadoLoop</clipPath>
        </li>
      </grains>
      <volumeRange>14~14</volumeRange>
      <pitchRange>1.3~1.5</pitchRange>
      <distRange>10~30</distRange>
      <sustainAttack>0.4</sustainAttack>
      <sustainSkipFirstAttack>false</sustainSkipFirstAttack>
      <randomStartPoint>true</randomStartPoint>
    </li>
  </subSounds>
</SoundDef>
```

Every number here is a starting point for the in-game tuning pass, not a balanced value:

- **`volumeRange` 14.** One fifth of the tornado's 70, which puts it between Core's orbital
  targeter aiming loop (10) and its fire click (18) and well under the vent's hiss (35). A
  respirator should not mask the gunfire it is used under.
- **`pitchRange` 1.3~1.5.** Makes a smaller intake out of a weather-sized roar. The spread also
  keeps two pilots drawing at once from phasing.
- **`distRange` 10~30.** `GasPack_Deploy`'s, so the absorb carries as far as the set's other
  sounds.
- **Attack and fade.** `SampleSustainer` ramps the first sample in over `sustainAttack` unless
  `sustainSkipFirstAttack` holds, hence the explicit false. `sustainRelease` only applies at a
  sample's scheduled end, which a looping sample with the default `sustainLoopDurationRange` of
  9999 s never reaches, so it is left out. The tail after the draw ends is the SoundDef-level
  `sustainFadeoutTime`: `Sustainer.End` keeps it alive and `SampleSustainer` fades it out over that
  span. Both times are real seconds. At 3x speed the 300-tick draw lasts about 1.7 s, so 0.4 s in and
  0.6 s out still leave it audible.
- **`randomStartPoint`.** The draw is far shorter than the loop, so every cast would otherwise
  play the same opening seconds of the clip.
- **`maxSimultaneous` 2.** Two pilots drawing at once are each heard; a third is dropped
  (`PrioritizeNearest` keeps the two nearest).
- **`tempoAffectedByGameSpeed`, `muteWhenPaused` and `canVacuumDampen` stay at their defaults**,
  as on `GasReleasing`. A paused draw keeps sounding, as the vent's hiss does.

## Wiring

**Why not VEF's `castSound`.** `AbilityDef.castSound` is a one-shot played from
`Ability.CastEffects`. A one-shot of a looping clip plays the clip's full length whatever the
draw's duration setting is, and nothing can stop it early.

**Why not an effecter.** The vent's sound rides `ToxGasReleasing` because that effecter already
existed with its flecks. The absorb draws its visuals itself. An effecter would add a def and the
same per-tick `EffectTick` and `Cleanup` bookkeeping as a bare sustainer, with nothing gained.

**The sustainer.** The changes:

1. `AbsorbGasExtension` gets `public SoundDef drawSound;`, and the def's extension gets
   `<drawSound>TXWC_AbsorbGasDraw</drawSound>`. This keeps the tuning on the def, like the flecks.
2. The ability def gets `<needsTicking>true</needsTicking>` beside `needsTickingInterval`.
   `CompAbilitiesApparel.CompTick` calls `Ability.Tick()` every tick only for abilities with
   that flag, and VEF's `ConfigErrors` flags an overridden `Tick` that lacks it. A `PerTick`
   sustainer ends once `TicksGame > lastMaintainTick + 1` (`Sustainer.SustainerUpdate`), so the
   existing `TickInterval` cannot maintain it. Off-camera pawns tick their interval in strides. The
   base `Ability.Tick` only steps `maintainedEffecters`, which the absorb has none of.
3. `Ability_AbsorbGas` holds an unsaved `Sustainer drawSustainer` and overrides `Tick()`:

   ```csharp
   public override void Tick()
   {
       base.Tick();
       SoundDef sound = Extension?.drawSound;
       if (sound != null && Absorbing && pawn is { Spawned: true })
       {
           if (drawSustainer == null || drawSustainer.Ended)
               drawSustainer = sound.TrySpawnSustainer(SoundInfo.InMap(pawn, MaintenanceType.PerTick));
           drawSustainer?.Maintain();
       }
       else if (drawSustainer != null)
       {
           if (!drawSustainer.Ended)
               drawSustainer.End();
           drawSustainer = null;
       }
   }
   ```

   The sound follows the draw's state: it starts on the first tick of a draw, whether cast by
   hand, by autocast or reloaded from a save, and stops on the tick after the draw ends. The
   draw can end by expiry, `StopAbsorbing` (the vent opening) or the pilot despawning.
   `TrySpawnSustainer` returns null when the voice cap is full, and the next tick retries.
   `SoundInfo.InMap(pawn, …)` makes the pawn the sound's maker, so it should follow a moving
   pilot as the tornado's follows the tornado. Hearing a moving pilot confirms this.
4. **The helmet coming off mid-draw** stops `Tick` from being called at all. The sustainer then
   ends itself through the `PerTick` rule and fades out over `sustainFadeoutTime`. The stale
   reference is dropped on the next tick that reaches `Tick`, by the `Ended` check.

The class comment gains one paragraph on the sound (the bare sustainer and why it ticks per tick),
and `AbsorbGas.xml`'s header gains a "Sound" bullet giving the clip and the volume it is set
against. `CHANGELOG.md` `[Unreleased]` gets a player-facing line under Added.

## Test list

- Hand cast, standing and walking: the roar fades in over about 0.4 s, follows the pilot, and
  fades out as the draw ends.
- At 1x, 3x and the top speed: the roar is audible at every speed and the tail is never cut.
- Paused mid-draw: the roar keeps sounding, as the vent does, and stops after unpausing once
  the draw ends.
- Opening the vent mid-draw: the roar stops and the vent's open sound and hiss take over.
- Autocast on in a smoke cloud: one roar per draw, no stacking on re-cast.
- Two pilots drawing side by side: both are heard, and a third is dropped.
- Save mid-draw and reload: the roar resumes for the rest of the draw.
- Strip the helmet mid-draw (dev mode): the roar fades out, with no "maintain ended sustainer"
  error in the log.
- Camera far from the pilot: silent outside about 30 cells.
- Next to gunfire and a venting pilot: the roar sits under both. If not, lower `volumeRange`.
