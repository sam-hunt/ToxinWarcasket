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

    // The spray verb's label, the jet command's and the job report's. It lives here because the
    // DefInjection walker never reaches a WarcasketDef's verbs: it lists the runtime type's
    // fields, and ThingDef.verbs is private to the base. Injection runs before ResolveReferences,
    // so the translated text is what lands on the verb.
    [MustTranslate]
    public string jetLabel;

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

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);
        if (jetLabel.NullOrEmpty())
            return;
        foreach (VerbProperties verb in parentDef.Verbs)
        {
            if (verb is VerbProperties_SprayGas)
                verb.label = jetLabel;
        }
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

    // CanBeUsed runs every frame the jet command shows, and the low-tank reason quotes only the
    // cost, so it is built once per settings write (ToxinWarcasketSettings.Generation).
    private string tankLowReason;
    private int tankLowGeneration = -1;

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
            if (gizmo is Command_VerbOwner { verb: Verb_SprayGas } jet)
            {
                if (!Props.jetDescription.NullOrEmpty())
                    jet.defaultDesc = Props.jetDescription;
                // Command_VerbTarget.IconDrawColor tints the icon with the gear's DrawColor, the
                // shoulders' dye, whenever the verb has an EquipmentSource, which a reloadable
                // comp's verb always does, so defaultIconColor never reaches the draw. Only
                // Command_VerbOwner.overrideColor wins over it.
                jet.overrideColor = Color.white;
            }
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
            if (tankLowReason == null || tankLowGeneration != ToxinWarcasketSettings.Generation)
            {
                tankLowGeneration = ToxinWarcasketSettings.Generation;
                tankLowReason = "TXWC_JetTankLow".Translate(Props.tankChargesPerShot.Named("COUNT"), tank.Props.ChargeNounArgument);
            }
            reason = tankLowReason;
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
    // Cheapest tests first: the line of sight is cast only for a group that would beat the best
    // so far.
    private Pawn BestTarget(Pawn wearer, Verb jet)
    {
        List<IAttackTarget> potential = wearer.Map.attackTargetsCache.GetPotentialTargetsFor(wearer);
        float range = jet.EffectiveRange;
        float rangeSq = range * range;
        float start = (jet.verbProps as VerbProperties_SprayGas)?.coneStart ?? 0f;
        float startSq = start * start;
        Pawn best = null;
        float bestScore = Props.aiTriggerBodySize - 0.001f;
        foreach (IAttackTarget candidate in potential)
        {
            if (candidate.Thing is not Pawn pawn)
                continue;
            int distSq = (pawn.Position - wearer.Position).LengthHorizontalSquared;
            if (distSq > rangeSq || distSq < startSq || !Affected(pawn, wearer))
                continue;
            float score = ClusterScore(wearer, pawn.Position);
            if (score > bestScore && jet.CanHitTarget(pawn))
            {
                best = pawn;
                bestScore = score;
            }
        }
        return best;
    }

    // The body size of the hostiles within aiClusterRadius of center that the gas would affect,
    // or -1 once an affected pawn of the wearer's own faction turns up among them. Read from the
    // cells' thing lists, so the cost is the disc's, not the map's pawn count.
    private float ClusterScore(Pawn wearer, IntVec3 center)
    {
        Map map = wearer.Map;
        int cells = GenRadial.NumCellsInRadius(Props.aiClusterRadius);
        float score = 0f;
        for (int i = 0; i < cells; i++)
        {
            IntVec3 cell = center + GenRadial.RadialPattern[i];
            if (!cell.InBounds(map))
                continue;
            foreach (Thing thing in cell.GetThingList(map))
            {
                if (thing is not Pawn other || other == wearer || other.Downed
                    || !GasUtility.IsAffectedByExposure(other))
                    continue;
                if (other.HostileTo(wearer))
                    score += other.BodySize;
                else if (other.Faction == wearer.Faction)
                    return -1f;
            }
        }
        return score;
    }

    private static bool Affected(Pawn pawn, Pawn wearer) =>
        !pawn.Downed && pawn.HostileTo(wearer) && GasUtility.IsAffectedByExposure(pawn)
        && !pawn.IsPsychologicallyInvisible();
}
