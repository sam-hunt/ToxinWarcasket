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
// replay's fixed sweep is an approximation of the same rule.
// The scratch state is three map-sized arrays indexed as the grid is: each cell's density, its
// peak, and whether gas can enter it, which the grid answers with an edifice lookup and a sweep
// asks of every neighbour, so it is asked once per estimate. A list of the cells touched is what
// the sweeps walk and what is cleared for the next estimate, so a shot's few hundred cells cost
// no hashing and no clearing of the whole map. It is static: the targeter draws on the main
// thread and nothing else calls this.
public static class GasSpreadEstimate
{
    private const int MaxDensity = GasGrid.MaxGasPerCell;
    private const int MinDiffusion = 17;
    private const int DiffusionTicks = 32;
    private const int DissipationTicks = 64;
    private const int DissipationAmount = 3;

    private const byte Untested = 0;
    private const byte Blocked = 1;
    private const byte Open = 2;

    private static int[] density = [];
    private static int[] peak = [];
    private static byte[] passable = [];

    // Cells with a peak above zero, in the order they were first gassed, and cells whose
    // passability was asked, both as grid indices.
    private static readonly List<int> touched = new();
    private static readonly List<int> tested = new();

    // Fills cells with every cell expected to reach minDensity within ticks of gas being laid
    // over sources.
    public static void Cells(Map map, List<IntVec3> sources, float gas, int minDensity, int ticks, List<IntVec3> cells)
    {
        cells.Clear();
        if (sources.Count == 0 || gas <= 0f)
            return;
        Prepare(map);
        GasGrid grid = map.gasGrid;
        CellIndices indices = map.cellIndices;
        LayDown(map, grid, sources, gas);
        int sweeps = ticks / DiffusionTicks;
        for (int i = 1; i <= sweeps; i++)
        {
            Diffuse(map, grid, i);
            if (i * DiffusionTicks % DissipationTicks == 0)
                Dissipate();
        }
        foreach (int i in touched)
        {
            if (peak[i] >= minDensity)
                cells.Add(indices.IndexToCell(i));
        }
    }

    // Sizes the arrays to the map and clears what the last estimate touched. The last
    // estimate's indices are cleared whatever map they were for: same-sized arrays are shared.
    private static void Prepare(Map map)
    {
        int count = map.cellIndices.NumGridCells;
        if (density.Length != count)
        {
            density = new int[count];
            peak = new int[count];
            passable = new byte[count];
        }
        else
        {
            foreach (int i in touched)
            {
                density[i] = 0;
                peak[i] = 0;
            }
            foreach (int i in tested)
                passable[i] = Untested;
        }
        touched.Clear();
        tested.Clear();
    }

    private static void LayDown(Map map, GasGrid grid, List<IntVec3> sources, float gas)
    {
        CellIndices indices = map.cellIndices;
        int share = Mathf.RoundToInt(gas / sources.Count);
        int perCell = Mathf.Min(share, MaxDensity);
        foreach (IntVec3 source in sources)
            Set(indices.CellToIndex(source), perCell);
        int remaining = (share - perCell) * sources.Count;
        if (remaining <= 0)
            return;
        map.floodFiller.FloodFill(IntVec3.Invalid, grid.GasCanMoveTo, (IntVec3 cell) =>
        {
            int i = indices.CellToIndex(cell);
            int room = MaxDensity - density[i];
            if (room > 0)
            {
                int take = Mathf.Min(room, remaining);
                Set(i, density[i] + take);
                remaining -= take;
            }
            return remaining <= 0;
        }, extraRoots: sources);
    }

    // The grid shuffles the four directions on every visit and visits cells in a random order; a
    // fixed order would pile the spread onto one side, so each cell starts its directions at a
    // step set by its position and the sweep, and alternate sweeps run the cells backwards. A
    // cell first gassed during a sweep joins the next one.
    private static void Diffuse(Map map, GasGrid grid, int sweepIndex)
    {
        CellIndices indices = map.cellIndices;
        int count = touched.Count;
        bool backwards = (sweepIndex & 1) == 0;
        for (int n = 0; n < count; n++)
        {
            int i = touched[backwards ? count - 1 - n : n];
            int a = density[i];
            if (a < MinDiffusion)
                continue;
            IntVec3 cell = indices.IndexToCell(i);
            int first = (cell.x * 3 + cell.z * 5 + sweepIndex) & 3;
            for (int d = 0; d < 4; d++)
            {
                IntVec3 neighbour = cell + GenAdj.CardinalDirections[(first + d) & 3];
                if (!neighbour.InBounds(map))
                    continue;
                int j = indices.CellToIndex(neighbour);
                if (!CanEnter(grid, neighbour, j))
                    continue;
                int b = density[j];
                int moved = Mathf.Abs(a - b) / 2;
                if (a > b && moved >= MinDiffusion)
                {
                    a -= moved;
                    Set(j, b + moved);
                    if (a < MinDiffusion)
                        break;
                }
            }
            density[i] = a;
        }
    }

    private static void Dissipate()
    {
        foreach (int i in touched)
            density[i] = Mathf.Max(density[i] - DissipationAmount, 0);
    }

    private static bool CanEnter(GasGrid grid, IntVec3 cell, int i)
    {
        byte state = passable[i];
        if (state == Untested)
        {
            state = grid.GasCanMoveTo(cell) ? Open : Blocked;
            passable[i] = state;
            tested.Add(i);
        }
        return state == Open;
    }

    private static void Set(int i, int value)
    {
        density[i] = value;
        if (value > peak[i])
        {
            if (peak[i] == 0)
                touched.Add(i);
            peak[i] = value;
        }
    }
}
