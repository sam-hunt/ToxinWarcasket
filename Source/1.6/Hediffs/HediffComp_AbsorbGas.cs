using RimWorld;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

public class HediffCompProperties_AbsorbGas : HediffCompProperties
{
    public int pulseInterval = 30;
    public float radius = 2.9f;

    // Taken from each gas type's density in every cell per pulse (densities run 0 to 255).
    public int unitsPerPulse = 64;

    // Absorbed units, summed over gas types, per tank charge: one full cell, as the tank emits.
    public int unitsPerCharge = 255;

    public HediffCompProperties_AbsorbGas()
    {
        compClass = typeof(HediffComp_AbsorbGas);
    }
}

// The helmet's Absorb Gas, carried by the short-lived hediff the ability puts on the pilot, so it
// ticks with the pawn and saves with it while the helmet itself never ticks. Each pulse thins
// every gas within the radius around the pilot's current cell and banks what it took in the worn
// armor's CompToxTank, if any; the filter is the conversion, so smoke and rot stink come out as
// tox reagent. Without the armor it only clears the air.
//
// GasGrid.AddGas cannot subtract (it returns on amount <= 0), so the pulse writes densities
// with SetDirect, which does not redraw, and dirties the gas mesh itself as AddGas does. Cells out
// of the pilot's line of sight are skipped, as explosions skip them, so walls stop the draw.
public class HediffComp_AbsorbGas : HediffComp
{
    // Units absorbed short of a whole charge, carried to the next pulse.
    private int carriedUnits;

    public HediffCompProperties_AbsorbGas Props => (HediffCompProperties_AbsorbGas)props;

    public override void CompPostTick(ref float severityAdjustment)
    {
        Pawn pawn = Pawn;
        if (!pawn.Spawned || !pawn.IsHashIntervalTick(Props.pulseInterval))
            return;
        int absorbed = Absorb(pawn.Position, pawn.Map);
        if (absorbed == 0)
            return;
        CompToxTank tank = WornTank(pawn);
        if (tank == null)
            return;
        carriedUnits += absorbed;
        tank.Add(carriedUnits / Props.unitsPerCharge);
        carriedUnits %= Props.unitsPerCharge;
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Scribe_Values.Look(ref carriedUnits, "carriedUnits");
    }

    private int Absorb(IntVec3 center, Map map)
    {
        GasGrid grid = map.gasGrid;
        int cut = Props.unitsPerPulse;
        int absorbed = 0;
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, Props.radius, useCenter: true))
        {
            if (!cell.InBounds(map))
                continue;
            int index = map.cellIndices.CellToIndex(cell);
            uint packed = grid.GetDirect(index);
            if (packed == 0 || !GenSight.LineOfSight(center, cell, map, skipFirstCell: true))
                continue;
            byte smoke = Thin(packed, GasType.BlindSmoke, cut, ref absorbed);
            byte toxic = Thin(packed, GasType.ToxGas, cut, ref absorbed);
            byte rotStink = Thin(packed, GasType.RotStink, cut, ref absorbed);
            byte deadlife = Thin(packed, GasType.DeadlifeDust, cut, ref absorbed);
            grid.SetDirect(index, smoke, toxic, rotStink, deadlife);
            map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Gas);
        }
        return absorbed;
    }

    // GasType's values are each gas's bit offset in the packed cell, as GasGrid.DensityAt reads.
    private static byte Thin(uint packed, GasType gas, int cut, ref int absorbed)
    {
        int density = (int)((packed >> (int)gas) & 0xFF);
        int taken = Mathf.Min(density, cut);
        absorbed += taken;
        return (byte)(density - taken);
    }

    private static CompToxTank WornTank(Pawn pawn)
    {
        if (pawn.apparel == null)
            return null;
        foreach (Apparel apparel in pawn.apparel.WornApparel)
        {
            CompToxTank tank = apparel.TryGetComp<CompToxTank>();
            if (tank != null)
                return tank;
        }
        return null;
    }
}
