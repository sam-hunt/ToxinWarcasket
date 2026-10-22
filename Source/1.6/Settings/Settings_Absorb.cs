using RimWorld;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// Absorb section: the helmet's absorb AbilityDef, whose radius, durationTime and cooldownTime
// the three sliders write. The AbsorbGasExtension on it (pulseInterval, unitsPerPulse) is read
// only, for the summary line. The header carries the ability's icon and tooltip.
public partial class ToxinWarcasketSettings
{
    // The absorb's AbilityDef radius, durationTime in seconds and cooldownTime in hours.
    public readonly Tunable absorbRadius = new("AbsorbRadius", 1.5f, 9.5f, 0.5f, v => v.ToString("0.#"));
    public readonly Tunable absorbDuration = new("AbsorbDuration", 1f, 15f, 0.5f, v => v.ToString("0.#"));
    public readonly Tunable absorbCooldown = new("AbsorbCooldown", 0f, 24f, 1f, v => Mathf.RoundToInt(v).ToString());

    // Read with the defaults: the absorb's draw, for the summary.
    private AbsorbGasExtension absorbDraw;

    // The helmet's absorb icon, beside the absorb section's header.
    private Texture2D absorbIcon;

    // The tooltip over the absorb section's header: the ability, its name as VEF titles an
    // ability's tooltip over the description its command shows in game.
    private string absorbTip;

    private void ExposeAbsorbSettings()
    {
        absorbRadius.ExposeData();
        absorbDuration.ExposeData();
        absorbCooldown.ExposeData();
    }

    private void LoadAbsorbDefaults(Defs defs)
    {
        absorbDraw = defs.absorbDraw;
        absorbIcon = defs.absorb.icon;
        absorbTip = HeaderTip(defs.absorb.LabelCap, defs.absorb.description);
        absorbRadius.DefaultValue = defs.absorb.radius;
        absorbDuration.DefaultValue = defs.absorb.durationTime.TicksToSeconds();
        absorbCooldown.DefaultValue = (float)defs.absorb.cooldownTime / GenDate.TicksPerHour;
    }

    private void ApplyAbsorbToDefs(Defs defs)
    {
        defs.absorb.radius = absorbRadius.Value;
        defs.absorb.durationTime = absorbDuration.Value.SecondsToTicks();
        defs.absorb.cooldownTime = absorbCooldown.IntValue * GenDate.TicksPerHour;
    }

    private void ResetAbsorbSettings()
    {
        absorbRadius.Reset();
        absorbDuration.Reset();
        absorbCooldown.Reset();
    }

    private void DrawAbsorbSection(Listing_Standard listing)
    {
        SectionHeader(listing, "TXWC_SectionAbsorb", absorbIcon, absorbTip);
        absorbRadius.DoRow(listing);
        absorbDuration.DoRow(listing);
        absorbCooldown.DoRow(listing);
        SummaryRow(listing, AbsorbSummary);
    }

    // Each pulse takes unitsPerPulse of a cell's density, so an absorb clears a cell only once
    // its pulses reach a full one; the gas banks at GasPerCharge, carried across pulses, so the
    // chemfuel is the floor of the whole draw (Ability_AbsorbGas).
    private string AbsorbSummary()
    {
        int cells = GenRadial.NumCellsInRadius(absorbRadius.Value);
        int pulses = absorbDuration.Value.SecondsToTicks() / absorbDraw.pulseInterval;
        int perCell = Mathf.Min(GasGrid.MaxGasPerCell, pulses * absorbDraw.unitsPerPulse);
        int chemfuel = Mathf.FloorToInt(cells * perCell / GasPerCharge) * chemfuelPerCharge;
        return "TXWC_AbsorbSummary".Translate(cells, chemfuel);
    }
}
