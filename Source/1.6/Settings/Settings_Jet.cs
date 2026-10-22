using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// Jet section: the shoulders' CompProperties_JetNozzle (tankChargesPerShot, maxCharges and the
// own jets' ammoCountPerCharge) and VerbProperties_SprayGas (halfAngle and range). The header
// carries the verb's commandIcon and the nozzle's jet label and description.
public partial class ToxinWarcasketSettings
{
    // The shoulders' CompProperties_JetNozzle: tankChargesPerShot, in chemfuel, and the own
    // jets' maxCharges. The own jets' ammoCountPerCharge follows the cost, so a jet costs the
    // same chemfuel from the tank or an own jet. At least one own jet: shoulders with none would
    // be useless without the tank, and the nozzle's disabled reason would quote a reload of
    // nothing.
    public readonly Tunable jetCost = new("JetCost", 5f, 100f, 5f, v => Mathf.RoundToInt(v).ToString());
    public readonly Tunable jetReserve = new("JetReserve", 1f, 5f, 1f, v => Mathf.RoundToInt(v).ToString());

    // The shoulders' VerbProperties_SprayGas: the cone's full width (twice halfAngle) and range.
    public readonly Tunable jetConeWidth = new("JetConeWidth", 6f, 120f, 2f, v => v.ToString("0"));
    public readonly Tunable jetRange = new("JetRange", 2.9f, 29.9f, 1f, v => v.ToString("0.#"));

    // The shoulders' jet icon (its verb's commandIcon), beside the jet section's header.
    private Texture2D jetIcon;

    // The tooltip over the jet section's header: the jet's name over the description the nozzle
    // gives it.
    private string jetTip;

    private void ExposeJetSettings()
    {
        jetCost.ExposeData();
        jetReserve.ExposeData();
        jetConeWidth.ExposeData();
        jetRange.ExposeData();
    }

    private void LoadJetDefaults(Defs defs)
    {
        jetIcon = defs.jet.commandIcon.NullOrEmpty() ? null : ContentFinder<Texture2D>.Get(defs.jet.commandIcon, false);
        jetTip = HeaderTip(defs.nozzle.jetLabel.CapitalizeFirst(), defs.nozzle.jetDescription);
        jetCost.DefaultValue = defs.nozzle.tankChargesPerShot * chemfuelPerCharge;
        jetReserve.DefaultValue = defs.nozzle.maxCharges;
        jetConeWidth.DefaultValue = defs.jet.halfAngle * 2f;
        jetRange.DefaultValue = defs.jet.range;
    }

    private void ApplyJetToDefs(Defs defs)
    {
        defs.nozzle.tankChargesPerShot = Mathf.Max(1, jetCost.IntValue / chemfuelPerCharge);
        defs.nozzle.maxCharges = jetReserve.IntValue;
        defs.nozzle.ammoCountPerCharge = defs.nozzle.tankChargesPerShot * chemfuelPerCharge;
        defs.jet.halfAngle = jetConeWidth.Value / 2f;
        defs.jet.range = jetRange.Value;
    }

    private void ResetJetSettings()
    {
        jetCost.Reset();
        jetReserve.Reset();
        jetConeWidth.Reset();
        jetRange.Reset();
    }

    private void DrawJetSection(Listing_Standard listing)
    {
        SectionHeader(listing, "TXWC_SectionJet", jetIcon, jetTip);
        jetCost.DoRow(listing);
        jetReserve.DoRow(listing);
        jetConeWidth.DoRow(listing);
        jetRange.DoRow(listing);
        SummaryRow(listing, JetSummary);
    }

    // What one jet amounts to in the tank summary's numbers, and how many a full tank and the
    // shoulders' own reserve hold.
    private string JetSummary()
    {
        int chemfuel = jetCost.IntValue;
        float cells = chemfuel * gasPerChemfuel.Value;
        int perTank = tankCapacity.IntValue / chemfuel;
        return "TXWC_JetSummary".Translate(chemfuel, cells.ToString("0.#"), (cells / packCells).ToString("0.#"), perTank, jetReserve.IntValue);
    }
}
