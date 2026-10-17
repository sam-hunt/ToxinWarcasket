using System;
using RimWorld;
using Verse;

namespace ToxinWarcasket.Integrations;

// Optional integration with Vanilla Races Expanded - Waster (VREW): a charged tank stands in for
// tox gas to its tox absorption gene, so a waster pilot's pollution mutations grow inside the
// suit without a cloud around it. The tank comp calls FeedToxAbsorption once a second
// (CompToxTank); this class owns every VREW name and does the work.
//
// What VREW does, decompile-verified (VanillaRacesExpanded-Waster.dll, Workshop 2983471725,
// 1.6 assembly): HediffCompToxAbsorption, on the VRE_ToxAbsorption hediff VEF attaches while the
// gene is active, runs every 60 ticks of the pawn's hash interval on a map. If
// GasUtility.AnyGas(pawn.Position, map, ToxGas), it adds vanilla's PollutionStimulus hediff when
// the pawn has the VREW_PollutionRush gene and lacks it, then adds 0.022 severity to every
// hediff carrying HediffComp_Pollution (that stimulus) or VREW's HediffCompGeneDependentPollution
// (its eleven mutation hediffs). A pure "any tox gas on my cell" test: nothing about exposure
// or resistance, so the suit's own immunity never got in its way. FeedToxAbsorption is that
// branch verbatim, on the same hash interval, skipped while the cell has gas (VREW's comp is
// feeding then, so the rates never stack) and gated on the gene exactly as VREW gates its
// mutations, so a pilot without it gets nothing: no gas is laid and no exposure applied.
//
// No compile-time reference to VREW's assembly, and no Harmony patch of it: three names are
// resolved by reflection once, at startup, and resolving a Type never runs its static
// constructor, so the patch-timing hazard in ModInit's comment does not arise. Self-reports
// drift at startup (the family's Pattern B): silent when VREW is not active; one Log.Warning
// when it is but a name failed to resolve, which the smoke test (VREW on its list) catches.
// ModInit forces this class's static constructor so the warning fires at startup, not on the
// first worn tank. VREW's two constants are copied, not read, so a VREW retune drifts this
// silently; re-verify them against its assembly when VREW updates.
public static class VREWasterIntegration
{
    public const string PackageId = "vanillaracesexpanded.waster";

    // VREW's HediffCompToxAbsorption.tickInterval and its per-interval severity.
    public const int TickInterval = 60;
    private const float SeverityPerInterval = 0.022f;

    private const string GeneDependentPollutionTypeName = "VanillaRacesExpandedWaster.HediffCompGeneDependentPollution";
    private const string ToxAbsorptionGeneName = "VRE_ToxAbsorption";
    private const string PollutionRushGeneName = "VREW_PollutionRush";

    // The mutation hediffs' comp type, or null when VREW is not loaded.
    private static readonly Type geneDependentPollutionType;
    private static readonly GeneDef toxAbsorptionGene;
    private static readonly GeneDef pollutionRushGene;

    // True while VREW is active and every name resolved. A Workshop copy beside a local one
    // carries a "_steam" postfix, which ModsConfig.IsActive would miss; the XML gates ignore it.
    public static bool ModActive { get; } = ModLister.GetActiveModWithIdentifier(PackageId, ignorePostfix: true) != null;

    public static bool Available => geneDependentPollutionType != null && toxAbsorptionGene != null && pollutionRushGene != null;

    static VREWasterIntegration()
    {
        if (!ModActive)
            return;
        try
        {
            geneDependentPollutionType = GenTypes.GetTypeInAnyAssembly(GeneDependentPollutionTypeName);
            toxAbsorptionGene = DefDatabase<GeneDef>.GetNamedSilentFail(ToxAbsorptionGeneName);
            pollutionRushGene = DefDatabase<GeneDef>.GetNamedSilentFail(PollutionRushGeneName);
        }
        catch (Exception ex)
        {
            Log.Warning("[Toxin Warcasket] Vanilla Races Expanded - Waster reflection failed (the tank will not feed tox absorption): " + ex);
            return;
        }
        if (Available)
        {
            // Positive evidence for a log reader: silence alone cannot tell "resolved" from
            // "VREW was not active".
            Log.Message("[Toxin Warcasket] Vanilla Races Expanded - Waster found; the tank feeds its tox absorption gene.");
            return;
        }
        Log.Warning("[Toxin Warcasket] Vanilla Races Expanded - Waster is active but " + GeneDependentPollutionTypeName
            + ", " + ToxAbsorptionGeneName + " or " + PollutionRushGeneName
            + " could not be resolved; the tank will not feed tox absorption. VREW may have changed.");
    }

    // What VREW's tox absorption does on a gassed cell, for a spawned pawn on a cell without gas.
    public static void FeedToxAbsorption(Pawn pawn)
    {
        if (!Available)
            return;
        Pawn_GeneTracker genes = pawn.genes;
        if (genes == null || !genes.HasActiveGene(toxAbsorptionGene))
            return;
        if (pawn.Position.AnyGas(pawn.Map, GasType.ToxGas))
            return;

        HediffSet hediffSet = pawn.health.hediffSet;
        if (genes.HasActiveGene(pollutionRushGene) && !hediffSet.HasHediff(HediffDefOf.PollutionStimulus))
            pawn.health.AddHediff(HediffDefOf.PollutionStimulus);
        foreach (Hediff hediff in hediffSet.hediffs)
        {
            if (IsPollutionHediff(hediff))
                hediff.Severity += SeverityPerInterval;
        }
    }

    private static bool IsPollutionHediff(Hediff hediff)
    {
        if (hediff is not HediffWithComps withComps || withComps.comps == null)
            return false;
        foreach (HediffComp comp in withComps.comps)
        {
            if (comp is HediffComp_Pollution || geneDependentPollutionType.IsInstanceOfType(comp))
                return true;
        }
        return false;
    }
}
