using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ToxinWarcasket;

public class VerbProperties_SprayGas : VerbProperties
{
    // Half the cone's width in degrees, either side of the line to the target.
    public float halfAngle = 15f;

    // Cells closer than this to the pilot get no gas (the shoulders def's header).
    public float coneStart = 3f;

    // Explosion propagation speed, so the cloud rolls outward rather than appearing at once.
    public float propagationSpeed = 0.6f;

    // The targeter's outer area: cells expected to hold spreadMinGas or more (of a full cell's
    // 255) within spreadTicks of the shot (GasSpreadEstimate; the threshold's rationale is in the
    // shoulders def's header).
    public int spreadMinGas = 1;
    public int spreadTicks = 300;

    public EffecterDef sprayEffecter;
    public int sprayEffecterTicks = 14;

    public VerbProperties_SprayGas()
    {
        verbClass = typeof(Verb_SprayGas);
    }
}

// The shoulders' gas jet: Verb_SpewFire (Biotech's scorcher) with tox gas for fire. The cone goes
// to GenExplosion as overrideCells instead of SpewFire's affectedAngle, because the explosion's
// own angle test compares raw Atan2 degrees and so cuts any cone that straddles due west in half;
// ConeCells wraps the angle. The shot's gas (CompJetNozzle.GasPerShot) is shared evenly over the
// cells the cone reaches, so walls concentrate it rather than waste it; past a full cell,
// GasGrid.AddGas overflows the excess into neighbouring cells, so a cone smaller than the gas
// thickens at its edges rather than losing any. Like the explosion's own cells, the cone skips
// anything out of the pilot's line of sight, and it starts coneStart out from the pilot, so the
// pilot and everyone beside and behind them stay clear. ToxGas is the tox grenade's harmless
// damage def; the gas does the work.
// Fuel is the nozzle's (CompJetNozzle), spent here as Verb_SpewFire spends its reloadable; a
// verb with no nozzle has no fuel to spend and never fires.
// A player order casts once where the pilot stands (OrderForceTarget), as vanilla's apparel
// Verb_LaunchProjectileStatic does; the base order is AttackStatic, which recasts until the fuel
// runs out.
public class Verb_SprayGas : Verb
{
    // The outer area's edges, dimmer than the cone's white so the cone stays the aim.
    private static readonly Color SpreadColor = new(1f, 1f, 1f, 0.3f);

    private static readonly List<IntVec3> coneScratch = new();
    private static readonly List<IntVec3> spreadScratch = new();

    private VerbProperties_SprayGas Props => (VerbProperties_SprayGas)verbProps;

    protected override bool TryCastShot()
    {
        if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            return false;
        if (ReloadableCompSource is not CompJetNozzle nozzle || !nozzle.CanBeUsed(out _))
            return false;
        float gas = nozzle.GasPerShot;
        nozzle.UsedOnce();

        IntVec3 origin = caster.Position;
        Map map = caster.Map;
        var cells = new List<IntVec3>();
        ConeCells(origin, currentTarget.Cell, map, Props.coneStart, EffectiveRange, Props.halfAngle, cells);
        if (cells.Count > 0)
        {
            GenExplosion.DoExplosion(origin, map, EffectiveRange, DamageDefOf.ToxGas, caster,
                postExplosionGasType: GasType.ToxGas, postExplosionGasAmount: Mathf.RoundToInt(gas / cells.Count),
                doVisualEffects: false, propagationSpeed: Props.propagationSpeed,
                doSoundEffects: false, overrideCells: cells);
        }
        if (Props.sprayEffecter != null)
        {
            AddEffecterToMaintain(Props.sprayEffecter.Spawn(origin, currentTarget.Cell, map),
                origin, currentTarget.Cell, Props.sprayEffecterTicks, map);
        }
        lastShotTick = Find.TickManager.TicksGame;
        return true;
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        Job job = JobMaker.MakeJob(JobDefOf.UseVerbOnThingStatic, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    // Verb_SpewFire's guard: an AI wearer does not stop to spray with a melee attacker on it.
    public override bool Available()
    {
        if (!base.Available())
            return false;
        if (CasterIsPawn)
        {
            Pawn pawn = CasterPawn;
            if (pawn.Faction != Faction.OfPlayer && pawn.mindState.MeleeThreatStillThreat
                && pawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(pawn.Position))
                return false;
        }
        return true;
    }

    // The range ring, the cone the shot lands in, and fainter around it the cells the gas should
    // drift over as it spreads (GasSpreadEstimate), so the player sees both who the jet hits and
    // where the cloud will show.
    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        if (!target.IsValid || caster?.Map == null)
            return;
        Map map = caster.Map;
        ConeCells(caster.Position, target.Cell, map, Props.coneStart, EffectiveRange, Props.halfAngle, coneScratch);
        if (coneScratch.Count == 0)
            return;
        if (ReloadableCompSource is CompJetNozzle nozzle)
        {
            GasSpreadEstimate.Cells(map, coneScratch, nozzle.GasPerShot, Props.spreadMinGas, Props.spreadTicks, spreadScratch);
            GenDraw.DrawFieldEdges(spreadScratch, SpreadColor);
        }
        GenDraw.DrawFieldEdges(coneScratch);
    }

    // The walkable cells in the pilot's line of sight between start and range of origin and
    // within halfAngle of the line to target.
    public static void ConeCells(IntVec3 origin, IntVec3 target, Map map, float start, float range, float halfAngle,
        List<IntVec3> cells)
    {
        cells.Clear();
        if (target == origin)
            return;
        float aim = Mathf.Atan2(target.z - origin.z, target.x - origin.x) * Mathf.Rad2Deg;
        float startSquared = Mathf.Max(start * start, 0.25f);
        int count = GenRadial.NumCellsInRadius(range);
        for (int i = 0; i < count; i++)
        {
            IntVec3 offset = GenRadial.RadialPattern[i];
            if (offset.LengthHorizontalSquared < startSquared)
                continue;
            float angle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
            if (Mathf.Abs(Mathf.DeltaAngle(aim, angle)) > halfAngle)
                continue;
            IntVec3 cell = origin + offset;
            if (cell.InBounds(map) && cell.Walkable(map)
                && GenSight.LineOfSight(origin, cell, map, skipFirstCell: true))
                cells.Add(cell);
        }
    }
}
