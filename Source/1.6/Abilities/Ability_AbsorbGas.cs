using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using VEF.Abilities;
using Ability = VEF.Abilities.Ability;

namespace ToxinWarcasket;

// Tunes Ability_AbsorbGas, on the ability def; values and their rationale live there.
public class AbsorbGasExtension : DefModExtension
{
    public int pulseInterval = 30;

    // Taken from each gas type's density in every cell per pulse (densities run 0 to 255).
    public int unitsPerPulse = 64;

    public int autoCastCheckInterval = 60;

    // Summed density of every gas type in the pilot's cell (255 is one gas at full density).
    public int autoCastMinDensity = 1;

    // The inflow visuals. Each cell a pulse thins throws one gasFleck from that cell into the
    // pilot, tinted with the colour of the gas it took most of, scaled from gasFleckScale at a
    // sliver to gasFleckScale.max at a full pulse's cut, and timed to arrive over
    // gasFleckTravelSeconds. A pulse that takes nothing throws puffsPerPulse of puffFleck from the
    // disc's edge at puffSpeed cells a second instead, so a draw over clean air still shows.
    public FleckDef gasFleck;
    public FloatRange gasFleckScale = new(0.6f, 1.4f);
    public float gasFleckTravelSeconds = 0.8f;
    public Color smokeColor = Color.white;
    public Color toxGasColor = Color.white;
    public Color rotStinkColor = Color.white;
    public Color deadlifeDustColor = Color.white;
    public FleckDef puffFleck;
    public int puffsPerPulse = 4;
    public float puffSpeed = 3.5f;
}

// The helmet's Absorb Gas: for the ability's duration the respirator draws every gas within its
// radius toward the pilot, pulse by pulse, and banks what it takes in the worn armor's
// CompToxTank, if any. The filter is the conversion, so smoke and rot stink come out as tox
// gas; without the armor it only clears the air. One charge banks per CompToxTank.GasPerCharge
// units drawn in, the gas a charge vents, so the gas-per-charge setting is the exchange rate
// both ways. The radius and duration are mod settings, written onto the def. The draw is the
// apparel's own, ticked by the helmet's CompAbilitiesApparel (needsTickingInterval), so it ends
// if the helmet comes off.
//
// GasGrid.AddGas cannot subtract (it returns on amount <= 0), so a pulse writes densities with
// SetDirect, which does not redraw, and dirties the gas mesh itself as AddGas does. Cells out of
// the pilot's line of sight are skipped, as explosions skip them, so walls stop the draw.
//
// Cast without a job: VEF's StartAbilityJob ends the pilot's current job and starts its cast job
// even at castTime 0, which stops a moving pilot. The absorb has no warmup, so CreateCastJob
// calls Cast directly, the path VEF itself takes for a caravan member, and the pilot carries on.
//
// The draw and the armor's vent exclude each other, whichever starts last winning: a cast
// closes a venting tank, and CompToxTank.SetVenting stops a draw (StopAbsorbing), so the
// respirator never pulls in the vent's own cloud. The stopped draw's cooldown stands.
//
// Autocast: VEF's own autocast only offers a pawn's learned abilities as attack verbs
// (Pawn.TryGetAttackVerb), which apparel abilities never are, so its right-click toggle would do
// nothing here. TickInterval reads the toggle instead and casts when the pilot's cell holds
// gas. Player pilots only, as raiders never absorb, and never while the worn tank is venting,
// which would feed the vent's own cloud straight back in.
public class Ability_AbsorbGas : Ability
{
    // The tick the current draw ends; at or before now, no draw is running.
    private int absorbEndTick = -1;

    // Units absorbed short of a whole charge, carried to the next pulse.
    private int carriedUnits;

    private AbsorbGasExtension extension;

    private AbsorbGasExtension Extension => extension ??= def.GetModExtension<AbsorbGasExtension>();

    private bool Absorbing => absorbEndTick > Find.TickManager.TicksGame;

    // The draw on pawn's worn helmet, if any.
    public static Ability_AbsorbGas WornBy(Pawn pawn)
    {
        if (pawn?.apparel == null)
            return null;
        foreach (Apparel apparel in pawn.apparel.WornApparel)
        {
            if (apparel.TryGetComp<CompAbilitiesApparel>() is not { } abilities)
                continue;
            foreach (Ability ability in abilities.GivenAbilities)
            {
                if (ability is Ability_AbsorbGas absorb)
                    return absorb;
            }
        }
        return null;
    }

    public void StopAbsorbing() => absorbEndTick = -1;

    // CompAbilitiesApparel re-runs Init whenever its unsaved wearer field differs from the
    // wearer, which includes the first gizmo request after a load, and Init resets autoCast to
    // the def's default; the player's toggle is kept through it.
    public override void Init()
    {
        bool wasAutoCast = autoCast;
        base.Init();
        autoCast = wasAutoCast && CanAutoCast;
    }

    // The gizmo is disabled for the draw, before the cooldown is checked: a cast mid-draw would
    // restart it and drop the carried remainder, and the cooldown setting can go below the
    // draw's duration. Core's "Already active" is the reason.
    public override bool IsEnabledForPawn(out string reason)
    {
        if (Absorbing)
        {
            reason = "AlreadyActive".Translate();
            return false;
        }
        return base.IsEnabledForPawn(out reason);
    }

    public override void CreateCastJob(params GlobalTargetInfo[] targets)
    {
        currentTargetingIndex = -1;
        currentTargets = new GlobalTargetInfo[def.targetCount];
        Cast(targets);
    }

    public override void Cast(params GlobalTargetInfo[] targets)
    {
        base.Cast(targets);
        if (CompToxTank.WornBy(pawn) is { Venting: true } tank)
            tank.SetVenting(false);
        absorbEndTick = Find.TickManager.TicksGame + GetDurationForPawn();
        carriedUnits = 0;
    }

    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);
        if (pawn == null || Extension is not { } ext)
            return;
        if (Absorbing)
            AbsorbTick(ext, delta);
        else if (autoCast && pawn.IsHashIntervalTick(ext.autoCastCheckInterval, delta))
            AutoCastCheck(ext);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref absorbEndTick, "absorbEndTick", -1);
        Scribe_Values.Look(ref carriedUnits, "carriedUnits");
    }

    private void AbsorbTick(AbsorbGasExtension ext, int delta)
    {
        if (!pawn.Spawned)
            return;
        if (!pawn.IsHashIntervalTick(ext.pulseInterval, delta))
            return;
        Map map = pawn.Map;
        float radius = GetRadiusForPawn();
        int absorbed = Absorb(ext, map, radius);
        if (absorbed == 0)
            SpawnPuffs(ext, map, radius);
        if (absorbed == 0 || CompToxTank.WornBy(pawn) is not { } tank)
            return;
        int unitsPerCharge = Mathf.Max(1, Mathf.RoundToInt(tank.GasPerCharge));
        carriedUnits += absorbed;
        tank.Add(carriedUnits / unitsPerCharge);
        carriedUnits %= unitsPerCharge;
    }

    private void AutoCastCheck(AbsorbGasExtension ext)
    {
        if (!pawn.Spawned || pawn.Downed || !pawn.IsColonistPlayerControlled || !IsEnabledForPawn(out _))
            return;
        if (CompToxTank.WornBy(pawn) is { Venting: true })
            return;
        if (GasInCell(pawn.Position, pawn.Map) >= ext.autoCastMinDensity)
            Cast(new GlobalTargetInfo(pawn));
    }

    private void SpawnPuffs(AbsorbGasExtension ext, Map map, float radius)
    {
        if (ext.puffFleck == null)
            return;
        Vector3 center = pawn.DrawPos;
        for (int i = 0; i < ext.puffsPerPulse; i++)
        {
            Vector3 from = center + Quaternion.AngleAxis(Rand.Range(0f, 360f), Vector3.up) * Vector3.forward * radius;
            if (!from.ShouldSpawnMotesAt(map))
                continue;
            FleckCreationData data = FleckMaker.GetDataStatic(from, map, ext.puffFleck, Rand.Range(1f, 1.5f));
            data.rotationRate = Rand.Range(-30f, 30f);
            data.velocityAngle = (center - from).AngleFlat();
            data.velocitySpeed = ext.puffSpeed;
            map.flecks.CreateFleck(data);
        }
    }

    private int Absorb(AbsorbGasExtension ext, Map map, float radius)
    {
        IntVec3 center = pawn.Position;
        GasGrid grid = map.gasGrid;
        int cut = ext.unitsPerPulse;
        int absorbed = 0;
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, radius, useCenter: true))
        {
            if (!cell.InBounds(map))
                continue;
            int index = map.cellIndices.CellToIndex(cell);
            uint packed = grid.GetDirect(index);
            if (packed == 0 || !GenSight.LineOfSight(center, cell, map, skipFirstCell: true))
                continue;
            int cellTaken = 0, mostTaken = 0;
            Color tint = default;
            byte smoke = Thin(packed, GasType.BlindSmoke, cut, ext.smokeColor, ref cellTaken, ref mostTaken, ref tint);
            byte toxic = Thin(packed, GasType.ToxGas, cut, ext.toxGasColor, ref cellTaken, ref mostTaken, ref tint);
            byte rotStink = Thin(packed, GasType.RotStink, cut, ext.rotStinkColor, ref cellTaken, ref mostTaken, ref tint);
            byte deadlife = Thin(packed, GasType.DeadlifeDust, cut, ext.deadlifeDustColor, ref cellTaken, ref mostTaken, ref tint);
            grid.SetDirect(index, smoke, toxic, rotStink, deadlife);
            map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Gas);
            absorbed += cellTaken;
            SpawnGasWisp(ext, map, cell, Mathf.Min(1f, (float)cellTaken / cut), tint);
        }
        return absorbed;
    }

    // GasType's values are each gas's bit offset in the packed cell, as GasGrid.DensityAt reads.
    // tint ends as the colour of the gas the cell gave most of (the first on a tie).
    private static byte Thin(uint packed, GasType gas, int cut, Color color, ref int cellTaken,
        ref int mostTaken, ref Color tint)
    {
        int density = (int)((packed >> (int)gas) & 0xFF);
        int taken = Mathf.Min(density, cut);
        if (taken > mostTaken)
        {
            tint = color;
            mostTaken = taken;
        }
        cellTaken += taken;
        return (byte)(density - taken);
    }

    private void SpawnGasWisp(AbsorbGasExtension ext, Map map, IntVec3 cell, float share, Color tint)
    {
        if (ext.gasFleck == null)
            return;
        Vector3 from = cell.ToVector3Shifted();
        if (!from.ShouldSpawnMotesAt(map))
            return;
        Vector3 to = pawn.DrawPos;
        to.y = from.y;
        FleckCreationData data = FleckMaker.GetDataStatic(from, map, ext.gasFleck, ext.gasFleckScale.LerpThroughRange(share));
        data.instanceColor = tint;
        data.rotationRate = Rand.Range(-60f, 60f);
        data.velocityAngle = (to - from).AngleFlat();
        data.velocitySpeed = (to - from).MagnitudeHorizontal() / ext.gasFleckTravelSeconds;
        map.flecks.CreateFleck(data);
    }

    private static int GasInCell(IntVec3 cell, Map map)
    {
        GasGrid grid = map.gasGrid;
        return grid.DensityAt(cell, GasType.BlindSmoke) + grid.DensityAt(cell, GasType.ToxGas)
            + grid.DensityAt(cell, GasType.RotStink) + grid.DensityAt(cell, GasType.DeadlifeDust);
    }
}
