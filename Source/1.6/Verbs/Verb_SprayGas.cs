using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

public class VerbProperties_SprayGas : VerbProperties
{
    // Half the cone's width in degrees, either side of the line to the target.
    public float halfAngle = 15f;

    // Gas laid on each cell of the cone: 255 is a full cell.
    public int gasPerCell = 255;

    // Explosion propagation speed, so the cloud rolls outward rather than appearing at once.
    public float propagationSpeed = 0.6f;

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
// ConeCells wraps the angle. Like the explosion's own cells, the cone skips the pilot's cell
// (within 0.5) and anything out of the pilot's line of sight, so the pilot and everyone behind
// them stay clear. ToxGas is the tox grenade's harmless damage def; the gas does the work.
// Fuel is the nozzle's (CompJetNozzle), spent here as Verb_SpewFire spends its reloadable.
public class Verb_SprayGas : Verb
{
    private static readonly List<IntVec3> coneScratch = new();

    private VerbProperties_SprayGas Props => (VerbProperties_SprayGas)verbProps;

    protected override bool TryCastShot()
    {
        if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            return false;
        if (ReloadableCompSource is { } nozzle)
        {
            if (!nozzle.CanBeUsed(out _))
                return false;
            nozzle.UsedOnce();
        }

        IntVec3 origin = caster.Position;
        Map map = caster.Map;
        var cells = new List<IntVec3>();
        ConeCells(origin, currentTarget.Cell, map, EffectiveRange, Props.halfAngle, cells);
        if (cells.Count > 0)
        {
            GenExplosion.DoExplosion(origin, map, EffectiveRange, DamageDefOf.ToxGas, caster,
                postExplosionGasType: GasType.ToxGas, postExplosionGasAmount: Props.gasPerCell,
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

    // The range ring plus the cone the shot would fill, so the player can see who it reaches.
    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        if (!target.IsValid || caster?.Map == null)
            return;
        ConeCells(caster.Position, target.Cell, caster.Map, EffectiveRange, Props.halfAngle, coneScratch);
        GenDraw.DrawFieldEdges(coneScratch);
    }

    public static void ConeCells(IntVec3 origin, IntVec3 target, Map map, float range, float halfAngle,
        List<IntVec3> cells)
    {
        cells.Clear();
        if (target == origin)
            return;
        float aim = Mathf.Atan2(target.z - origin.z, target.x - origin.x) * Mathf.Rad2Deg;
        int count = GenRadial.NumCellsInRadius(range);
        for (int i = 0; i < count; i++)
        {
            IntVec3 offset = GenRadial.RadialPattern[i];
            if (offset.LengthHorizontalSquared <= 0)
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
