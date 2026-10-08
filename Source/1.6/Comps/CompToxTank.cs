using RimWorld;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// The armor's reagent tank, the one supply every tox gas emitter of the set draws on. A vanilla
// CompApparelReloadable underneath, so the reload job, the "reload" float menu, save/load of the
// charge count and the full tank on creation (PostPostMake) all come from the base class; one
// charge is one cell of gas (Props.gasPerCharge). remainingCharges is protected on
// CompApparelVerbOwner_Charged, which is what TryConsume/Add/Empty reach.
//
// The tank also owns:
//  - Downed burst: worn apparel gets no downed notification (ThingComp.Notify_Downed is called
//    on the pawn's own comps only), so CompTick watches Wearer.Downed for a false-to-true edge.
//    The edge state is saved, and resynced on equip, so a load or a downed pawn being dressed
//    never reads as a fresh fall.
//  - Death rupture: Notify_WearerDied comes from Apparel.Notify_PawnKilled, which Pawn.Kill
//    reaches through PreDeathPawnModifications before the pawn despawns, so Wearer's position
//    and map are still valid there.
//  - Venting: the vent ability only flips Venting; the emission ticks here, so the state lives
//    in one saved place. Non-player wearers vent on their own (AIVentCheck).
//
// Bursts skip an anaesthetized wearer, so no surgery (VFEP's warcasket removal among them)
// gasses the operating room, alive or dead. Every emitter skips an unspawned wearer (caravans,
// transporters, holding platforms).
public class CompToxTank : CompApparelReloadable
{
    private bool venting;
    private bool wasDowned;
    private int aiTicksWithoutTargets;

    // Ticks until the next vent charge. Unsaved: a load mid-vent at worst emits one charge early.
    private int ventCooldown;

    [Unsaved]
    private Effecter ventEffecter;

    public new CompProperties_ToxTank Props => (CompProperties_ToxTank)props;

    public bool Venting => venting;

    public float FillPercent => (float)RemainingCharges / MaxCharges;

    public bool TryConsume(int charges)
    {
        if (charges > remainingCharges)
            return false;
        remainingCharges -= charges;
        return true;
    }

    // Returns how many charges fit; the rest are lost.
    public int Add(int charges)
    {
        int added = Mathf.Clamp(charges, 0, MaxCharges - remainingCharges);
        remainingCharges += added;
        return added;
    }

    public void Empty() => remainingCharges = 0;

    public void SetVenting(bool on)
    {
        if (on && remainingCharges > 0)
        {
            venting = true;
            return;
        }
        venting = false;
        aiTicksWithoutTargets = 0;
        ventCooldown = 0;
        ventEffecter?.Cleanup();
        ventEffecter = null;
    }

    public override void CompTick()
    {
        base.CompTick();
        Pawn wearer = Wearer;
        if (wearer == null || wearer.Dead)
        {
            if (venting)
                SetVenting(false);
            return;
        }

        bool downed = wearer.Downed;
        if (downed != wasDowned)
        {
            wasDowned = downed;
            if (downed)
            {
                SetVenting(false);
                TryBurst(wearer);
            }
        }

        if (downed || !wearer.Spawned)
        {
            if (venting)
                SetVenting(false);
            return;
        }

        if (!wearer.IsColonistPlayerControlled && wearer.IsHashIntervalTick(Props.aiCheckInterval))
            AIVentCheck(wearer);
        if (venting)
            VentTick(wearer);
    }

    public override void Notify_WearerDied()
    {
        base.Notify_WearerDied();
        SetVenting(false);
        Pawn wearer = Wearer;
        if (wearer == null || !wearer.Spawned)
            return;
        TryBurst(wearer);
        Rupture(wearer.Position, wearer.Map);
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        wasDowned = pawn.Downed;
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        base.Notify_Unequipped(pawn);
        SetVenting(false);
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        SetVenting(false);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref venting, "venting");
        Scribe_Values.Look(ref wasDowned, "wasDowned");
        Scribe_Values.Look(ref aiTicksWithoutTargets, "aiTicksWithoutTargets");
    }

    private void VentTick(Pawn wearer)
    {
        if (remainingCharges <= 0)
        {
            SetVenting(false);
            return;
        }
        if (Props.ventEffecter != null)
        {
            ventEffecter ??= Props.ventEffecter.Spawn(wearer, TargetInfo.Invalid);
            ventEffecter.EffectTick(wearer, TargetInfo.Invalid);
        }
        if (--ventCooldown > 0)
            return;
        ventCooldown = Props.ventTicksPerCharge;
        GasUtility.AddGas(wearer.Position, wearer.Map, GasType.ToxGas, Props.gasPerCharge);
        remainingCharges--;
    }

    // Whole tank at once as a tox gas explosion sized to what was in it: ToxGas is the tox
    // grenade's damage def (defaultDamage 0), so the blast is visuals and sound and the gas, laid
    // per cell by the explosion, does the work. Explosions skip cells out of the centre's line of
    // sight, so walls shape the cloud.
    private void TryBurst(Pawn wearer)
    {
        if (!wearer.Spawned || remainingCharges < Props.minBurstCharges)
            return;
        if (wearer.health.hediffSet.HasHediff(HediffDefOf.Anesthetic))
            return;
        float radius = GenRadial.RadiusOfNumCells(remainingCharges);
        GenExplosion.DoExplosion(wearer.Position, wearer.Map, radius, DamageDefOf.ToxGas, wearer,
            postExplosionGasType: GasType.ToxGas, postExplosionGasAmount: Props.gasPerCharge);
        Empty();
    }

    private void Rupture(IntVec3 center, Map map)
    {
        PollutionGrid pollution = map.pollutionGrid;
        int filthKinds = Props.ruptureFilth.Count;
        foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, Props.ruptureRadius, useCenter: true))
        {
            if (!cell.InBounds(map))
                continue;
            if (pollution.CanPollute(cell))
                pollution.SetPolluted(cell, true);

            if (cell == center)
            {
                if (Props.ruptureCenterFilth != null)
                    FilthMaker.TryMakeFilth(cell, map, Props.ruptureCenterFilth);
                continue;
            }
            if (filthKinds == 0 || !Rand.Chance(Props.ruptureFilthChance))
                continue;
            int first = Rand.Range(0, filthKinds);
            FilthMaker.TryMakeFilth(cell, map, Props.ruptureFilth[first]);
            if (filthKinds > 1 && Rand.Chance(Props.ruptureSecondFilthChance))
            {
                int second = (first + Rand.Range(1, filthKinds)) % filthKinds;
                FilthMaker.TryMakeFilth(cell, map, Props.ruptureFilth[second]);
            }
        }
    }

    // The vanilla AI never toggles an apparel ability, so raiders decide here (player wearers
    // never reach this). Venting state is shared with the player toggle; a colonist who loses
    // player control mid-vent is handed to this check and stops once no target is near.
    private void AIVentCheck(Pawn wearer)
    {
        bool targets = wearer.Awake() && !wearer.InMentalState && HostilesInReach(wearer);
        if (venting)
        {
            if (targets)
                aiTicksWithoutTargets = 0;
            else if ((aiTicksWithoutTargets += Props.aiCheckInterval) >= Props.aiStopAfterTicks)
                SetVenting(false);
        }
        else if (targets && remainingCharges >= Props.aiMinCharges)
        {
            SetVenting(true);
        }
    }

    // CompToxPack.ChanceToUse's scan: hostile pawns the gas would debuff (GasUtility counts only
    // humanlikes without exposure immunity), by body size.
    private bool HostilesInReach(Pawn wearer)
    {
        Map map = wearer.Map;
        int cells = GenRadial.NumCellsInRadius(Props.aiTriggerRadius);
        float bodySize = 0f;
        for (int i = 0; i < cells; i++)
        {
            IntVec3 cell = wearer.Position + GenRadial.RadialPattern[i];
            if (!cell.InBounds(map))
                continue;
            foreach (Thing thing in cell.GetThingList(map))
            {
                if (thing is Pawn pawn && pawn != wearer && !pawn.Downed && pawn.HostileTo(wearer)
                    && GasUtility.IsAffectedByExposure(pawn) && !pawn.IsPsychologicallyInvisible())
                {
                    bodySize += pawn.BodySize;
                    if (bodySize >= Props.aiTriggerBodySize)
                        return true;
                }
            }
        }
        return false;
    }
}
