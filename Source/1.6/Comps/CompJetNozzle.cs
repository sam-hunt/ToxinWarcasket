using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ToxinWarcasket;

public class CompProperties_JetNozzle : CompProperties_ApparelReloadable
{
    // Charges one jet draws from the worn armor's tank.
    public int tankChargesPerShot = 30;

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

// The shoulders' gas jet fuel. A jet draws Props.tankChargesPerShot from the worn armor's
// CompToxTank when it holds that many; otherwise it spends the nozzle's own charge, a reserve
// shot loaded with chemfuel like a tox pack. So the shoulders work alone on their reserve, and
// with the armor the reserve is the shot left when the tank runs dry.
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

    private CompToxTank WornTank
    {
        get
        {
            Pawn wearer = Wearer;
            if (wearer?.apparel == null)
                return null;
            foreach (Apparel apparel in wearer.apparel.WornApparel)
            {
                CompToxTank tank = apparel.TryGetComp<CompToxTank>();
                if (tank != null)
                    return tank;
            }
            return null;
        }
    }

    private bool TankCanPay => WornTank is { } tank && tank.RemainingCharges >= Props.tankChargesPerShot;

    // The tank's fill and the reserve, e.g. "64% +1"; the plain charge count without the armor.
    public override string GizmoExtraLabel
    {
        get
        {
            CompToxTank tank = WornTank;
            if (tank == null)
                return base.GizmoExtraLabel;
            string fill = tank.FillPercent.ToStringPercent();
            return RemainingCharges > 0 ? $"{fill} +{RemainingCharges}" : fill;
        }
    }

    public override bool CanBeUsed(out string reason)
    {
        if (!TankCanPay)
            return base.CanBeUsed(out reason);

        reason = "";
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
        if (WornTank is { } tank && tank.TryConsume(Props.tankChargesPerShot))
            return;
        base.UsedOnce();
    }

    public override void CompTick()
    {
        base.CompTick();
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
    // wearer's own faction in it.
    private Pawn BestTarget(Pawn wearer, Verb jet)
    {
        List<IAttackTarget> potential = wearer.Map.attackTargetsCache.GetPotentialTargetsFor(wearer);
        float range = jet.EffectiveRange;
        float clusterRadiusSq = Props.aiClusterRadius * Props.aiClusterRadius;
        Pawn best = null;
        float bestScore = Props.aiTriggerBodySize - 0.001f;
        foreach (IAttackTarget candidate in potential)
        {
            if (candidate.Thing is not Pawn pawn || !Affected(pawn, wearer)
                || !pawn.Position.InHorDistOf(wearer.Position, range) || !jet.CanHitTarget(pawn))
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
