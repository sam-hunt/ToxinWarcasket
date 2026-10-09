using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ToxinWarcasket;

// tankChargesPerShot and the reserve's maxCharges are mod settings defaulting to the def's
// values; the settings also recompute ammoCountPerCharge as the cost times the armor's chemfuel
// per charge, so the XML value is only the default's product (ToxinWarcasketSettings).
public class CompProperties_JetNozzle : CompProperties_ApparelReloadable
{
    // Charges one jet draws from the worn armor's tank.
    public int tankChargesPerShot = 10;

    // The jet command's tooltip. Vanilla's apparel verb command shows the gear's description,
    // which for these shoulders is the set's lore rather than what the jet does.
    [MustTranslate]
    public string jetDescription;

    // Opportunistic jet for non-player wearers (see CompJetNozzle.AIJetCheck): every
    // aiCheckInterval ticks, fire at the hostile with the most hostile body size the gas would
    // affect within aiClusterRadius of it, once that reaches aiTriggerBodySize.
    public int aiCheckInterval = 60;
    public float aiClusterRadius = 2.9f;
    public float aiTriggerBodySize = 1f;

    public CompProperties_JetNozzle()
    {
        compClass = typeof(CompJetNozzle);
    }
}

// The shoulders' gas jet fuel. With the armor's CompToxTank worn, a jet draws
// Props.tankChargesPerShot from it and nothing else, and a tank short of that disables the jet.
// The nozzle's own charges, jets loaded with chemfuel like a tox pack, are the fuel only when no
// tank is worn: a fallback so the shoulders stay useful alone, not a supply on top of the tank's,
// so while a tank is worn they are neither spent here nor reloaded
// (Patches/CompApparelReloadable_Reload). Either way the jet's gas is GasPerShot, the tank
// charges it costs at the gas-per-charge setting, so an own jet's chemfuel in the def is those
// charges' worth (the armor's ammoCountPerCharge times the cost).
//
// CompApparelReloadable.CanBeUsed fails on an empty nozzle before it reaches its base's map
// checks, so with the tank paying those checks are repeated here rather than reached through it.
//
// The vanilla AI only turns to an apparel verb when the pawn has no usable weapon
// (Pawn.TryGetAttackVerb), so an armed raider would never jet; AIJetCheck starts the cast itself,
// as CompToxPack deploys its pack.
public class CompJetNozzle : CompApparelReloadable
{
    public new CompProperties_JetNozzle Props => (CompProperties_JetNozzle)props;

    // The tank the jet draws on, if the wearer wears one.
    public CompToxTank WornTank => CompToxTank.WornBy(Wearer);

    // Gas units one jet makes, from the tank or an own jet alike.
    public float GasPerShot => Props.tankChargesPerShot * ToxinWarcasketMod.Settings.GasPerCharge;

    // The jet command's corner: the jets the worn supply holds, the tank's when one is worn and
    // the nozzle's own otherwise.
    public override string GizmoExtraLabel
    {
        get
        {
            if (WornTank is not { } tank)
                return base.GizmoExtraLabel;
            int perShot = Mathf.Max(1, Props.tankChargesPerShot);
            return $"{tank.RemainingCharges / perShot} / {tank.MaxCharges / perShot}";
        }
    }

    public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
    {
        foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
        {
            if (gizmo is Command_VerbTarget { verb: Verb_SprayGas } jet && !Props.jetDescription.NullOrEmpty())
                jet.defaultDesc = Props.jetDescription;
            yield return gizmo;
        }
    }

    public override bool CanBeUsed(out string reason)
    {
        if (WornTank is not { } tank)
            return base.CanBeUsed(out reason);

        reason = "";
        if (tank.RemainingCharges < Props.tankChargesPerShot)
        {
            reason = "TXWC_JetTankLow".Translate(Props.tankChargesPerShot.Named("COUNT"), tank.Props.ChargeNounArgument);
            return false;
        }
        Map map = parent.MapHeld;
        if (map == null)
            return false;
        if (map.IsPocketMap && VerbProperties.Any(vp => !vp.useableInPocketMaps))
        {
            reason = "CannotUseReason_PocketMap".Translate(map.generatorDef.label);
            return false;
        }
        if (map.Biome.inVacuum && VerbProperties.Any(vp => !vp.useableInVacuum))
        {
            reason = "CannotFunctionInVacuum".Translate();
            return false;
        }
        return true;
    }

    public override void UsedOnce()
    {
        if (WornTank is { } tank)
        {
            tank.TryConsume(Props.tankChargesPerShot);
            return;
        }
        base.UsedOnce();
    }

    public override void CompTick()
    {
        base.CompTick();
        if (remainingCharges > MaxCharges)
            remainingCharges = MaxCharges;
        Pawn wearer = Wearer;
        if (wearer != null && wearer.IsHashIntervalTick(Props.aiCheckInterval))
            AIJetCheck(wearer);
    }

    private void AIJetCheck(Pawn wearer)
    {
        if (!wearer.Spawned || wearer.Dead || wearer.Downed || wearer.IsColonistPlayerControlled
            || !wearer.Awake() || wearer.InMentalState || wearer.stances.FullBodyBusy)
            return;
        Verb jet = null;
        foreach (Verb verb in AllVerbs)
        {
            if (verb is Verb_SprayGas)
            {
                jet = verb;
                break;
            }
        }
        if (jet == null || !jet.Available())
            return;
        Pawn target = BestTarget(wearer, jet);
        if (target != null)
            jet.TryStartCastOn(target);
    }

    // The hostile at the heart of the densest group the gas would affect (GasUtility counts only
    // humanlikes without exposure immunity), skipping any group with an affected pawn of the
    // wearer's own faction in it, and anyone inside the cone's start, where no gas lands.
    private Pawn BestTarget(Pawn wearer, Verb jet)
    {
        List<IAttackTarget> potential = wearer.Map.attackTargetsCache.GetPotentialTargetsFor(wearer);
        float range = jet.EffectiveRange;
        float start = (jet.verbProps as VerbProperties_SprayGas)?.coneStart ?? 0f;
        float startSq = start * start;
        float clusterRadiusSq = Props.aiClusterRadius * Props.aiClusterRadius;
        Pawn best = null;
        float bestScore = Props.aiTriggerBodySize - 0.001f;
        foreach (IAttackTarget candidate in potential)
        {
            if (candidate.Thing is not Pawn pawn || !Affected(pawn, wearer)
                || !pawn.Position.InHorDistOf(wearer.Position, range)
                || (pawn.Position - wearer.Position).LengthHorizontalSquared < startSq
                || !jet.CanHitTarget(pawn))
                continue;
            float score = 0f;
            bool allyNear = false;
            foreach (Pawn other in wearer.Map.mapPawns.AllPawnsSpawned)
            {
                if (other == wearer || other.Downed || !GasUtility.IsAffectedByExposure(other)
                    || (other.Position - pawn.Position).LengthHorizontalSquared > clusterRadiusSq)
                    continue;
                if (other.HostileTo(wearer))
                    score += other.BodySize;
                else if (other.Faction == wearer.Faction)
                    allyNear = true;
            }
            if (!allyNear && score > bestScore)
            {
                best = pawn;
                bestScore = score;
            }
        }
        return best;
    }

    private static bool Affected(Pawn pawn, Pawn wearer) =>
        !pawn.Downed && pawn.HostileTo(wearer) && GasUtility.IsAffectedByExposure(pawn)
        && !pawn.IsPsychologicallyInvisible();
}
