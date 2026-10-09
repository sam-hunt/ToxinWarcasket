using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// Where a shot's gas will bite, for the gas jet's targeter. Lays the shot on a scratch copy of
// the grid as Verb_SprayGas lays it (an even share per cell, any excess over a full cell
// flood-filled outward to full, as GasGrid.AddGas overflows), then replays the grid's own
// diffusion and dissipation for the cloud's first seconds and keeps every cell whose density
// ever reaches a threshold. The rules (GasGrid.TryDiffuseGases, TryDissipateGases): once per 32
// ticks a cell of 17+ pushes half of any difference of 34+ into each cardinal neighbour gas can
// enter, and once per 64 ticks every gassed cell loses 3 (unroofed; roofed cells lose half, which
// the replay ignores, erring toward warning). The grid visits cells in a random order, so the
// replay's fixed sweep is an approximation of the same rule. A dictionary stands in for the
// grid, as a shot's cloud is a few hundred cells at most, and the scratch state is static: the
// targeter draws on the main thread and nothing else calls this.
public static class GasSpreadEstimate
{
    private const int MaxDensity = GasGrid.MaxGasPerCell;
    private const int MinDiffusion = 17;
    private const int DiffusionTicks = 32;
    private const int DissipationTicks = 64;
    private const int DissipationAmount = 3;

    private static readonly Dictionary<IntVec3, int> density = new();
    private static readonly Dictionary<IntVec3, int> peak = new();
    private static readonly List<IntVec3> sweep = new();

    // Fills cells with every cell expected to reach minDensity within ticks of gas being laid
    // over sources.
    public static void Cells(Map map, List<IntVec3> sources, float gas, int minDensity, int ticks, List<IntVec3> cells)
    {
        cells.Clear();
        density.Clear();
        peak.Clear();
        if (sources.Count == 0 || gas <= 0f)
            return;
        GasGrid grid = map.gasGrid;
        LayDown(map, grid, sources, gas);
        int sweeps = ticks / DiffusionTicks;
        for (int i = 1; i <= sweeps; i++)
        {
            Diffuse(grid, i);
            if (i * DiffusionTicks % DissipationTicks == 0)
                Dissipate();
        }
        foreach (KeyValuePair<IntVec3, int> cell in peak)
        {
            if (cell.Value >= minDensity)
                cells.Add(cell.Key);
        }
    }

    private static void LayDown(Map map, GasGrid grid, List<IntVec3> sources, float gas)
    {
        int share = Mathf.RoundToInt(gas / sources.Count);
        int perCell = Mathf.Min(share, MaxDensity);
        foreach (IntVec3 source in sources)
            Set(source, perCell);
        int remaining = (share - perCell) * sources.Count;
        if (remaining <= 0)
            return;
        map.floodFiller.FloodFill(IntVec3.Invalid, grid.GasCanMoveTo, (IntVec3 cell) =>
        {
            int room = MaxDensity - Get(cell);
            if (room > 0)
            {
                int take = Mathf.Min(room, remaining);
                Set(cell, Get(cell) + take);
                remaining -= take;
            }
            return remaining <= 0;
        }, extraRoots: sources);
    }

    // The grid shuffles the four directions on every visit and visits cells in a random order; a
    // fixed order would pile the spread onto one side, so each cell starts its directions at a
    // step set by its position and the sweep, and alternate sweeps run the cells backwards.
    private static void Diffuse(GasGrid grid, int sweepIndex)
    {
        sweep.Clear();
        sweep.AddRange(density.Keys);
        bool backwards = (sweepIndex & 1) == 0;
        for (int n = 0; n < sweep.Count; n++)
        {
            IntVec3 cell = sweep[backwards ? sweep.Count - 1 - n : n];
            int a = density[cell];
            if (a < MinDiffusion)
                continue;
            int first = (cell.x * 3 + cell.z * 5 + sweepIndex) & 3;
            for (int i = 0; i < 4; i++)
            {
                IntVec3 neighbour = cell + GenAdj.CardinalDirections[(first + i) & 3];
                if (!grid.GasCanMoveTo(neighbour))
                    continue;
                int b = Get(neighbour);
                int moved = Mathf.Abs(a - b) / 2;
                if (a > b && moved >= MinDiffusion)
                {
                    a -= moved;
                    Set(neighbour, b + moved);
                    if (a < MinDiffusion)
                        break;
                }
            }
            density[cell] = a;
        }
    }

    private static void Dissipate()
    {
        sweep.Clear();
        sweep.AddRange(density.Keys);
        foreach (IntVec3 cell in sweep)
            density[cell] = Mathf.Max(density[cell] - DissipationAmount, 0);
    }

    private static int Get(IntVec3 cell) => density.TryGetValue(cell, out int value) ? value : 0;

    private static void Set(IntVec3 cell, int value)
    {
        density[cell] = value;
        if (!peak.TryGetValue(cell, out int best) || value > best)
            peak[cell] = value;
    }
}
