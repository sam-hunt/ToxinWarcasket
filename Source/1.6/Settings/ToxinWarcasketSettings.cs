using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ToxinWarcasket;

// The set's mod settings. Each Tunable overrides a def field, written onto the def's props by
// ApplyToDefs (at startup, then whenever the window closes), so every reader of those props,
// vanilla's and VEF's included, sees the setting with no code of its own. The defaults, and the
// rationale for them, are the defs'.
//
// The player's word for a tank charge is "unit" (the tank's chargeNoun and every string here):
// the vent drains the tank continuously, so it reads as a volume rather than a count of
// activations. The code keeps vanilla's "charge", the reloadable comp's own term. The reserve
// jets stay "jets", one per activation.
public class ToxinWarcasketSettings : ModSettings
{
    private const string ArmorDefName = "TXWC_Warcasket_Toxin";
    private const string ShouldersDefName = "TXWC_WarcasketShoulders_Toxin";
    private const string AbsorbDefName = "TXWC_AbsorbGas";

    // The whitespace above every section header but the first, and below the last row.
    private const float SectionGap = 18f;

    // The armor's CompProperties_ToxTank: maxCharges, cellsPerCharge and ventChargesPerSecond.
    public readonly Tunable tankCapacity = new("TankCapacity", 10f, 100f, 5f, v => Mathf.RoundToInt(v).ToString());
    public readonly Tunable gasPerUnit = new("GasPerUnit", 0.5f, 6f, 0.25f, v => v.ToString("0.##"));
    public readonly Tunable ventRate = new("VentRate", 0.5f, 10f, 0.5f, v => v.ToString("0.#"));

    // The shoulders' CompProperties_JetNozzle: tankChargesPerShot and the own jets' maxCharges.
    // The own jets' ammoCountPerCharge follows the cost, so a jet costs the same chemfuel from
    // the tank or an own jet. At least one own jet: shoulders with none would be useless without
    // the tank, and the nozzle's disabled reason would quote a reload of nothing.
    public readonly Tunable jetCost = new("JetCost", 1f, 50f, 1f, v => Mathf.RoundToInt(v).ToString());
    public readonly Tunable jetReserve = new("JetReserve", 1f, 5f, 1f, v => Mathf.RoundToInt(v).ToString());

    // The shoulders' VerbProperties_SprayGas: the cone's full width (twice halfAngle) and range.
    public readonly Tunable jetConeWidth = new("JetConeWidth", 6f, 120f, 2f, v => v.ToString("0"));
    public readonly Tunable jetRange = new("JetRange", 2.9f, 29.9f, 1f, v => v.ToString("0.#"));

    // The absorb's AbilityDef radius, durationTime in seconds and cooldownTime in hours.
    public readonly Tunable absorbRadius = new("AbsorbRadius", 1.5f, 9.5f, 0.5f, v => v.ToString("0.#"));
    public readonly Tunable absorbDuration = new("AbsorbDuration", 1f, 15f, 0.5f, v => v.ToString("0.#"));
    public readonly Tunable absorbCooldown = new("AbsorbCooldown", 0f, 24f, 1f, v => Mathf.RoundToInt(v).ToString());

    // Gives VFEP's hazard warcasket the spacer sets' vacuum resistance under Odyssey. Unlike the
    // tunables it overrides no field of ours: PatchOperationHazardVacuum reads it while the XML
    // patches apply (1.6/Patches/HazardVacuumResistance.xml has the rationale), so a change
    // waits for a restart. Off by default.
    public bool hazardVacuum;

    private bool defaultsLoaded;
    private Vector2 scrollPosition;
    private float contentHeight;

    // Gas units (GasGrid.MaxGasPerCell to a full cell) one tank charge makes, wherever the set
    // spends or banks one: the vent, the bursts, the jet from the tank or its reserve, the absorb.
    public float GasPerCharge => gasPerUnit.Value * GasGrid.MaxGasPerCell;

    private IEnumerable<Tunable> All =>
        [tankCapacity, gasPerUnit, ventRate, jetCost, jetReserve, jetConeWidth, jetRange, absorbRadius, absorbDuration, absorbCooldown];

    public override void ExposeData()
    {
        base.ExposeData();
        foreach (Tunable tunable in All)
            tunable.ExposeData();
        Scribe_Values.Look(ref hazardVacuum, "HazardVacuum");
    }

    // Reads the def-backed defaults before the first ApplyToDefs overwrites them; defs must be
    // loaded, so ModInit's static constructor calls this, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        if (!TryGetDefs(out Defs defs))
            return;
        tankCapacity.DefaultValue = defs.tank.maxCharges;
        gasPerUnit.DefaultValue = defs.tank.cellsPerCharge;
        ventRate.DefaultValue = defs.tank.ventChargesPerSecond;
        jetCost.DefaultValue = defs.nozzle.tankChargesPerShot;
        jetReserve.DefaultValue = defs.nozzle.maxCharges;
        jetConeWidth.DefaultValue = defs.jet.halfAngle * 2f;
        jetRange.DefaultValue = defs.jet.range;
        absorbRadius.DefaultValue = defs.absorb.radius;
        absorbDuration.DefaultValue = defs.absorb.durationTime.TicksToSeconds();
        absorbCooldown.DefaultValue = (float)defs.absorb.cooldownTime / GenDate.TicksPerHour;
        defaultsLoaded = true;
    }

    public void ApplyToDefs()
    {
        if (!defaultsLoaded || !TryGetDefs(out Defs defs))
            return;
        defs.tank.maxCharges = tankCapacity.IntValue;
        defs.tank.cellsPerCharge = gasPerUnit.Value;
        defs.tank.ventChargesPerSecond = ventRate.Value;
        defs.nozzle.tankChargesPerShot = jetCost.IntValue;
        defs.nozzle.maxCharges = jetReserve.IntValue;
        defs.nozzle.ammoCountPerCharge = jetCost.IntValue * defs.tank.ammoCountPerCharge;
        defs.jet.halfAngle = jetConeWidth.Value / 2f;
        defs.jet.range = jetRange.Value;
        defs.absorb.radius = absorbRadius.Value;
        defs.absorb.durationTime = absorbDuration.Value.SecondsToTicks();
        defs.absorb.cooldownTime = absorbCooldown.IntValue * GenDate.TicksPerHour;
    }

    // One row per tunable under a header per piece: the tank (with the gas yield every piece
    // shares), the shoulders' jet, the helmet's absorb, then the miscellaneous toggles, with a
    // reset button below. Sections are set apart by whitespace and a medium-font header rather
    // than rules, which read as more sliders, and the rows sit indented under their header; the
    // last section closes on the same gap so the list does not end flush on its last row. The
    // sections outgrow the window's height, so they scroll, sized from the previous frame's
    // listing height; the scrollbar only takes its width once it is needed. maxOneColumn keeps
    // the listing growing downward: past its rect's height it would start a new column off to
    // the right, outside the window, and never report the height the scroll view needs.
    public void DoWindowContents(Rect inRect)
    {
        const float buttonHeight = 30f;
        const float buttonWidth = 200f;
        const float scrollbarWidth = 16f;
        var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height - buttonHeight - Listing.DefaultGap);
        float viewWidth = contentHeight > outRect.height ? outRect.width - scrollbarWidth : outRect.width;
        var viewRect = new Rect(0f, 0f, viewWidth, Mathf.Max(contentHeight, outRect.height));
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        var listing = new Listing_Standard { maxOneColumn = true };
        listing.Begin(viewRect);
        SectionHeader(listing, "TXWC_SectionTank", first: true);
        tankCapacity.DoRow(listing);
        gasPerUnit.DoRow(listing);
        ventRate.DoRow(listing);
        SectionHeader(listing, "TXWC_SectionJet");
        jetCost.DoRow(listing);
        jetReserve.DoRow(listing);
        jetConeWidth.DoRow(listing);
        jetRange.DoRow(listing);
        SectionHeader(listing, "TXWC_SectionAbsorb");
        absorbRadius.DoRow(listing);
        absorbDuration.DoRow(listing);
        absorbCooldown.DoRow(listing);
        SectionHeader(listing, "TXWC_SectionMisc");
        DoHazardVacuumRow(listing);
        listing.Gap(SectionGap);
        contentHeight = listing.CurHeight;
        listing.End();
        Widgets.EndScrollView();

        if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - buttonHeight, buttonWidth, buttonHeight),
                "RestoreToDefaultSettings".Translate()))
        {
            foreach (Tunable tunable in All)
                tunable.Reset();
            hazardVacuum = false;
        }
    }

    // A checkbox row laid out as a Tunable row is, the box where a slider row shows its value.
    // Disabled without Odyssey, which owns the stat, and the label says so; with Odyssey it
    // always says a change takes a restart, since the patch only reads the box at startup.
    private void DoHazardVacuumRow(Listing_Standard listing)
    {
        Rect row = listing.GetRect(Tunable.RowHeight);
        row.xMin += Tunable.RowIndent;
        row.xMax -= Tunable.RowEndPad;
        Widgets.DrawHighlightIfMouseover(row);
        TooltipHandler.TipRegion(row, "TXWC_HazardVacuumDesc".Translate());

        bool odyssey = ModsConfig.OdysseyActive;
        string label = "TXWC_HazardVacuum".Translate();
        if (!odyssey)
            label += "TXWC_RequiresOdysseySuffix".Translate();
        else
            label += "TXWC_RequiresRestartSuffix".Translate();
        Widgets.CheckboxLabeled(row, label, ref hazardVacuum, disabled: !odyssey);
        listing.Gap(Tunable.RowGap);
    }

    private static void SectionHeader(Listing_Standard listing, string key, bool first = false)
    {
        const float headerGap = 4f;
        if (!first)
            listing.Gap(SectionGap);
        GameFont font = Text.Font;
        Text.Font = GameFont.Medium;
        listing.Label(key.Translate());
        Text.Font = font;
        listing.Gap(headerGap);
    }

    private struct Defs
    {
        public CompProperties_ToxTank tank;
        public CompProperties_JetNozzle nozzle;
        public VerbProperties_SprayGas jet;
        public AbilityDef absorb;
    }

    private static bool TryGetDefs(out Defs defs)
    {
        ThingDef shoulders = DefDatabase<ThingDef>.GetNamedSilentFail(ShouldersDefName);
        defs = new Defs
        {
            tank = DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName)?.GetCompProperties<CompProperties_ToxTank>(),
            nozzle = shoulders?.GetCompProperties<CompProperties_JetNozzle>(),
            jet = shoulders?.Verbs.OfType<VerbProperties_SprayGas>().FirstOrDefault(),
            absorb = DefDatabase<AbilityDef>.GetNamedSilentFail(AbsorbDefName),
        };
        if (defs.tank != null && defs.nozzle != null && defs.jet != null && defs.absorb != null)
            return true;
        Log.Error("[Toxin Warcasket] Settings could not find the set's tank, gas jet or absorb def; they keep their XML values.");
        return false;
    }
}
