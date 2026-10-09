#!/usr/bin/env python3
"""Replays RimWorld 1.6's GasGrid rules (decompiled Verse.GasGrid, mapped in TOX_GAS_MECHANICS.md)
on an open, unroofed field so the set's emitters can be compared with the tox pack by the cloud
they actually leave, before any play test. Informational, not mod content. Rules mirrored:
  - dissipation: area/64 cells visited per tick in a fixed random cycle, tox gas -3 per visit
  - diffusion:   area/32 cells visited per tick, a cell >= 17 pushes half the difference to each
                 cardinal neighbour when that half is >= 17 (shuffled order, stop when < 17)
  - AddGas overflow: excess over 255 flood-fills (8-neighbour BFS) outward, saturating cells
Columns: cells-eq is total gas in full cells; the thresholds are ToxGasExposure's density stages
(0.2 mild, 0.5 moderate, 1.0 severe). Run with no arguments for the pack, the vent at three
rates, the jet and the bursts, or name scenarios: python3 Docs/Research/gas-grid-sim.py pack
"""
import math
import random
import sys
from collections import deque

SIZE = 90
AREA = SIZE * SIZE
MAX = 255
DISS = 3  # tox gas, unroofed


class Grid:
    def __init__(self, seed=1):
        self.rng = random.Random(seed)
        self.gas = [0] * AREA
        self.order = list(range(AREA))
        self.rng.shuffle(self.order)
        self.cdis = 0
        self.cdif = self.rng.randrange(AREA // 2)
        self.ndis = math.ceil(AREA / 64)
        self.ndif = math.ceil(AREA / 32)
        self.dirs = [1, -1, SIZE, -SIZE]

    @staticmethod
    def idx(x, z):
        return z * SIZE + x

    def add(self, i, amount):
        if amount <= 0:
            return
        new = self.gas[i] + amount
        if new > MAX:
            self.gas[i] = MAX
            self.overflow(i, new - MAX)
        else:
            self.gas[i] = new

    def overflow(self, root, remaining):
        seen = {root}
        q = deque([root])
        limit = 5025  # GenRadial.NumCellsInRadius(40)
        processed = 0
        while q and remaining > 0 and processed < limit:
            i = q.popleft()
            processed += 1
            room = MAX - self.gas[i]
            if room > 0:
                take = min(room, remaining)
                self.gas[i] += take
                remaining -= take
                if remaining <= 0:
                    return
            x, z = i % SIZE, i // SIZE
            for dz in (-1, 0, 1):
                for dx in (-1, 0, 1):
                    if dx == 0 and dz == 0:
                        continue
                    nx, nz = x + dx, z + dz
                    if 0 <= nx < SIZE and 0 <= nz < SIZE:
                        j = nz * SIZE + nx
                        if j not in seen:
                            seen.add(j)
                            q.append(j)

    def tick(self):
        gas = self.gas
        order = self.order
        for _ in range(self.ndis):
            if self.cdis >= AREA:
                self.cdis = 0
            i = order[self.cdis]
            self.cdis += 1
            d = gas[i]
            if d > 0:
                gas[i] = d - DISS if d > DISS else 0
        for _ in range(self.ndif):
            if self.cdif >= AREA:
                self.cdif = 0
            i = order[self.cdif]
            self.cdif += 1
            a = gas[i]
            if a < 17:
                continue
            x, z = i % SIZE, i // SIZE
            dirs = self.dirs[:]
            self.rng.shuffle(dirs)
            changed = False
            for d in dirs:
                if d == 1 and x == SIZE - 1:
                    continue
                if d == -1 and x == 0:
                    continue
                if d == SIZE and z == SIZE - 1:
                    continue
                if d == -SIZE and z == 0:
                    continue
                j = i + d
                b = gas[j]
                if a < 17:
                    break
                num = abs(a - b) // 2
                if a > b and num >= 17:
                    a -= num
                    gas[j] = b + num
                    changed = True
                    if a < 17:
                        break
            if changed:
                gas[i] = a

    def stats(self):
        total = 0
        mild = moderate = severe = any_ = 0
        for d in self.gas:
            if d:
                any_ += 1
                total += d
                if d >= 51:
                    mild += 1
                if d >= 128:
                    moderate += 1
                if d >= 255:
                    severe += 1
        return total, any_, mild, moderate, severe


def radial_cells(cx, cz, radius):
    r2 = radius * radius
    cells = []
    r = int(radius) + 1
    for dz in range(-r, r + 1):
        for dx in range(-r, r + 1):
            if dx * dx + dz * dz <= r2:
                cells.append((cx + dx, cz + dz))
    # GenRadial order: by distance
    cells.sort(key=lambda c: (c[0] - cx) ** 2 + (c[1] - cz) ** 2)
    return cells


def cone_cells(cx, cz, radius, half_angle, aim_deg=0.0):
    out = []
    for (x, z) in radial_cells(cx, cz, radius):
        dx, dz = x - cx, z - cz
        if dx == 0 and dz == 0:
            continue
        ang = math.degrees(math.atan2(dz, dx))
        diff = (ang - aim_deg + 180) % 360 - 180
        if abs(diff) <= half_angle:
            out.append((x, z))
    return out


def run(name, schedule, ticks=9000, report=(0, 300, 600, 900, 1200, 1800, 2700, 3600, 5400, 7200, 9000), seed=1):
    """schedule: dict tick -> list of (index, amount) adds, or a callable(grid, tick)."""
    g = Grid(seed)
    print(f"\n=== {name} ===")
    print(f"{'t(s)':>5} {'cells-eq':>8} {'any':>5} {'>=0.2':>6} {'>=0.5':>6} {'full':>5}")
    gone_at = None
    peak_mild = 0
    for t in range(ticks + 1):
        if callable(schedule):
            schedule(g, t)
        elif t in schedule:
            for i, amt in schedule[t]:
                g.add(i, amt)
        if t in report:
            total, any_, mild, moderate, severe = g.stats()
            print(f"{t/60:5.0f} {total/255:8.1f} {any_:5d} {mild:6d} {moderate:6d} {severe:5d}")
        if t % 60 == 0:
            total, any_, mild, moderate, severe = g.stats()
            peak_mild = max(peak_mild, mild)
            if gone_at is None and t > 600 and mild == 0:
                gone_at = t
        g.tick()
    print(f"peak cells >= 0.2: {peak_mild}; cells >= 0.2 gone at: {gone_at/60 if gone_at else '>' + str(ticks//60)} s")


def main():
    c = Grid.idx(SIZE // 2, SIZE // 2)
    which = sys.argv[1:] or ["pack", "vent-now", "vent-3pack-5", "vent-3pack-2", "vent-40-2", "jet-now", "jet-1pack", "jet-10", "burst-1pack", "burst-3pack"]

    if "vent-40-2" in which:
        sched = {t * 30: [(c, 765)] for t in range(40)}
        run("vent 3 cells/charge, 2 charges/s, 40 charges (120 cells in 20 s; the shipped tank)", sched)

    if "jet-10" in which:
        cells = cone_cells(SIZE // 2, SIZE // 2, 12.9, 12)
        per = round(10 * 765 / len(cells))
        sched = {0: [(Grid.idx(x, z), per) for (x, z) in cells]}
        run(f"jet 10 charges: 30 cells of gas over {len(cells)} cone cells ({per}/cell; the shipped jet)", sched)

    if "pack" in which:
        # CompReleaseGas: 11475 units, 450 every 30 ticks (25 pulses of 450 + 225)
        sched = {}
        remaining = 11475
        t = 0
        while remaining > 0:
            amt = min(450, remaining)
            sched[t] = [(c, amt)]
            remaining -= amt
            t += 30
        run("tox pack (45 cells over 12.75 s at one cell)", sched)

    if "vent-now" in which:
        # current: 0.8 cell/charge (204 units) every 12 ticks, 50 charges
        sched = {t * 12: [(c, 204)] for t in range(50)}
        run("vent now: 50 x 204 units, 5/s (40 cells in 10 s)", sched)

    if "vent-3pack-5" in which:
        sched = {t * 12: [(c, 765)] for t in range(45)}
        run("vent 3 cells/charge, 5 charges/s, 45 charges (135 cells in 9 s)", sched)

    if "vent-3pack-2" in which:
        sched = {t * 30: [(c, 765)] for t in range(45)}
        run("vent 3 cells/charge, 2 charges/s, 45 charges (135 cells in 22.5 s)", sched)

    if "vent-3pack-1" in which:
        sched = {t * 60: [(c, 765)] for t in range(45)}
        run("vent 3 cells/charge, 1 charge/s, 45 charges (135 cells in 45 s)", sched)

    if "jet-now" in which:
        cells = cone_cells(SIZE // 2, SIZE // 2, 12.9, 12)
        per = round(15 * 204 / len(cells))
        sched = {0: [(Grid.idx(x, z), per) for (x, z) in cells]}
        run(f"jet now: 12 cells of gas over {len(cells)} cone cells ({per}/cell)", sched)

    if "jet-1pack" in which:
        cells = cone_cells(SIZE // 2, SIZE // 2, 12.9, 12)
        per = round(11475 / len(cells))
        sched = {0: [(Grid.idx(x, z), per) for (x, z) in cells]}
        run(f"jet 1 pack: 45 cells of gas over {len(cells)} cone cells ({per}/cell, overflows)", sched)

    if "jet-1pack-15" in which:
        cells = cone_cells(SIZE // 2, SIZE // 2, 12.9, 15)
        per = round(11475 / len(cells))
        sched = {0: [(Grid.idx(x, z), per) for (x, z) in cells]}
        run(f"jet 1 pack, 30 deg cone: 45 cells of gas over {len(cells)} cone cells ({per}/cell)", sched)

    if "burst-1pack" in which:
        cells = radial_cells(SIZE // 2, SIZE // 2, 3.75)[:45]
        sched = {0: [(Grid.idx(x, z), 255) for (x, z) in cells]}
        run(f"burst 45 cells at full ({len(cells)} cells)", sched)

    if "burst-3pack" in which:
        cells = radial_cells(SIZE // 2, SIZE // 2, 6.6)[:135]
        sched = {0: [(Grid.idx(x, z), 255) for (x, z) in cells]}
        run(f"burst 135 cells at full ({len(cells)} cells)", sched)


if __name__ == "__main__":
    main()
