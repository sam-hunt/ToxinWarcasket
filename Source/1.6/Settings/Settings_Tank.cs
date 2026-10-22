using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ToxinWarcasket;

// Tank section: the armor's CompProperties_ToxTank, whose maxCharges, cellsPerCharge and
// ventChargesPerSecond the three sliders write, in chemfuel. The gas yield (gasPerChemfuel)
// is the one every piece shares: the jet, the vent, the bursts and the absorb's banking all
// read it through GasPerCharge. The header carries the vent ability's icon and tooltip.
public partial class ToxinWarcasketSettings
{
    // The armor's CompProperties_ToxTank: maxCharges, cellsPerCharge and ventChargesPerSecond,
    // in chemfuel.
    public readonly Tunable tankCapacity = new("TankCapacity", 20f, 200f, 10f, v => Mathf.RoundToInt(v).ToString());
    public readonly Tunable gasPerChemfuel = new("GasPerChemfuel", 0.25f, 3f, 0.25f, v => v.ToString("0.##"));
    public readonly Tunable ventRate = new("VentRate", 1f, 20f, 1f, v => Mathf.RoundToInt(v).ToString());

    // The armor's vent icon, beside the tank section's header.
    private Texture2D tankIcon;

    // The tooltip over the tank section's header: the vent ability, its name as VEF titles an
    // ability's tooltip over the description its command shows in game.
    private string tankTip;

    private void ExposeTankSettings()
    {
        tankCapacity.ExposeData();
        gasPerChemfuel.ExposeData();
        ventRate.ExposeData();
    }

    private void LoadTankDefaults(Defs defs)
    {
        AbilityDef vent = DefDatabase<AbilityDef>.GetNamedSilentFail(VentDefName);
        tankIcon = vent?.icon;
        tankTip = vent == null ? null : HeaderTip(vent.LabelCap, vent.description);
        tankCapacity.DefaultValue = defs.tank.maxCharges * chemfuelPerCharge;
        gasPerChemfuel.DefaultValue = defs.tank.cellsPerCharge / chemfuelPerCharge;
        ventRate.DefaultValue = defs.tank.ventChargesPerSecond * chemfuelPerCharge;
    }

    private void ApplyTankToDefs(Defs defs)
    {
        defs.tank.maxCharges = Mathf.Max(1, tankCapacity.IntValue / chemfuelPerCharge);
        defs.tank.cellsPerCharge = gasPerChemfuel.Value * chemfuelPerCharge;
        defs.tank.ventChargesPerSecond = ventRate.Value / chemfuelPerCharge;
    }

    private void ResetTankSettings()
    {
        tankCapacity.Reset();
        gasPerChemfuel.Reset();
        ventRate.Reset();
    }

    private void DrawTankSection(Listing_Standard listing)
    {
        SectionHeader(listing, "TXWC_SectionTank", tankIcon, tankTip, first: true);
        tankCapacity.DoRow(listing);
        gasPerChemfuel.DoRow(listing);
        ventRate.DoRow(listing);
        SummaryRow(listing, TankSummary);
    }

    // What the sliders amount to, from their current values, in the numbers the player meets:
    // chemfuel, cells of gas, tox packs' worth (the pack's cellsToFill) and seconds.
    private string TankSummary()
    {
        int chemfuel = tankCapacity.IntValue;
        float cells = chemfuel * gasPerChemfuel.Value;
        float seconds = chemfuel / ventRate.Value;
        return "TXWC_TankSummary".Translate(chemfuel, cells.ToString("0.#"), (cells / packCells).ToString("0.#"), seconds.ToString("0.#"));
    }
}
