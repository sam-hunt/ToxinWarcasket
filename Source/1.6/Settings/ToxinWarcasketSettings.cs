using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ToxinWarcasket.Integrations;
using UnityEngine;
using Verse;
using VFEPirates;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ToxinWarcasket;

// The set's mod settings. Each Tunable overrides a def field, written onto the def's props by
// ApplyToDefs (at startup, then whenever the window closes), so every reader of those props,
// vanilla's and VEF's included, sees the setting with no code of its own. The defaults, and the
// rationale for them, are the defs'.
//
// The sliders read in chemfuel, the one cost the player pays: the tank's and the jet's size in
// chemfuel, the gas yield in cells per chemfuel, the vent in chemfuel a second. The code keeps
// vanilla's "charge", the reloadable comp's own term, and the armor def makes a charge one
// chemfuel (its ammoCountPerCharge), so the gauge's "unit" (the tank's chargeNoun) is a chemfuel
// too; the conversion through ammoCountPerCharge here only keeps the window honest should a
// patch change that. Each section ends on a line of what its sliders amount to (a full tank, a
// jet, an absorb), so the balance reads off the window rather than being multiplied out.
public class ToxinWarcasketSettings : ModSettings
{
    private const string ArmorDefName = "TXWC_Warcasket_Toxin";
    private const string ShouldersDefName = "TXWC_WarcasketShoulders_Toxin";
    private const string HelmetDefName = "TXWC_WarcasketHelmet_Toxin";
    private const string AbsorbDefName = "TXWC_AbsorbGas";
    private const string VentDefName = "TXWC_VentTank";
    private const string ToxPackDefName = "Apparel_PackTox";

    // The whitespace above every section header but the first, and below the last row.
    private const float SectionGap = 28f;

    // Bumped by every ApplyToDefs, including the one at startup. A write moves numbers that
    // live instances have folded into strings (the abilities' tooltips, the jet's low-tank
    // reason); each rebuilds its own on its next request when this has moved.
    public static int Generation { get; private set; }

    // The armor's CompProperties_ToxTank: maxCharges, cellsPerCharge and ventChargesPerSecond,
    // in chemfuel.
    public readonly Tunable tankCapacity = new("TankCapacity", 20f, 200f, 10f, v => Mathf.RoundToInt(v).ToString());
    public readonly Tunable gasPerChemfuel = new("GasPerChemfuel", 0.25f, 3f, 0.25f, v => v.ToString("0.##"));
    public readonly Tunable ventRate = new("VentRate", 1f, 20f, 1f, v => Mathf.RoundToInt(v).ToString());

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

    // The absorb's AbilityDef radius, durationTime in seconds and cooldownTime in hours.
    public readonly Tunable absorbRadius = new("AbsorbRadius", 1.5f, 9.5f, 0.5f, v => v.ToString("0.#"));
    public readonly Tunable absorbDuration = new("AbsorbDuration", 1f, 15f, 0.5f, v => v.ToString("0.#"));
    public readonly Tunable absorbCooldown = new("AbsorbCooldown", 0f, 24f, 1f, v => Mathf.RoundToInt(v).ToString());

    // Gives VFEP's hazard warcasket the spacer sets' vacuum resistance under Odyssey. Unlike the
    // tunables it overrides no field of ours: PatchOperationSetting reads it by name while the
    // XML patches apply (1.6/Patches/HazardVacuumResistance.xml has the rationale), so a change
    // waits for a restart. Off by default.
    public bool hazardVacuum;

    // Rebuilds the set at VFEP's spacer tier. It overrides no field either: PatchOperationSetting
    // reads it while the XML patches apply (1.6/Patches/SpacerTier.xml has the values and the
    // rationale) and ApplySpacerText writes the tier's descriptions once at startup, so a change
    // waits for a restart. Off by default.
    public bool spacerTier;

    // Gives the set the spacer sets' vacuum resistance under Odyssey, as hazardVacuum does the
    // Hazard, without the rest of the tier. The patch (1.6/Patches/ToxinVacuumResistance.xml)
    // reads ToxinVacuumActive rather than this, so this stays the player's own choice and comes
    // back when the tier is switched off again. Off by default.
    public bool toxinVacuum;

    // What ToxinVacuumResistance.xml reads: the spacer tier carries the stat, so it holds the
    // toggle on.
    public bool ToxinVacuumActive => toxinVacuum || spacerTier;

    // The tank's feedsToxAbsorption: whether a charged tank stands in for tox gas to Vanilla
    // Races Expanded - Waster's tox absorption gene (Integrations/VREWasterIntegration). Written
    // onto the def like the tunables and read live from it, so no restart; its default is the
    // def's.
    public bool feedToxAbsorption = true;
    private bool feedToxAbsorptionDefault = true;

    private bool defaultsLoaded;
    private Vector2 scrollPosition;
    private float contentHeight;

    // Read with the defaults: the chemfuel a tank charge costs (the sliders' unit over the
    // charge fields), the tox pack's cells (the summaries' yardstick) and the absorb's draw.
    private int chemfuelPerCharge = 1;
    private float packCells = 45f;
    private AbsorbGasExtension absorbDraw;

    // The gizmo icon of the piece each section tunes, beside its header: the armor's vent, the
    // shoulders' jet (its verb's commandIcon) and the helmet's absorb.
    private Texture2D tankIcon;
    private Texture2D jetIcon;
    private Texture2D absorbIcon;

    // The tooltip over each section's header: the ability or verb that piece gives, its name as
    // VEF titles an ability's tooltip over the description its command shows in game.
    private string tankTip;
    private string jetTip;
    private string absorbTip;

    // Gas units (GasGrid.MaxGasPerCell to a full cell) one tank charge makes, wherever the set
    // spends or banks one: the vent, the bursts, the jet from the tank or its reserve, the absorb.
    public float GasPerCharge => gasPerChemfuel.Value * chemfuelPerCharge * GasGrid.MaxGasPerCell;

    private IEnumerable<Tunable> All =>
        [tankCapacity, gasPerChemfuel, ventRate, jetCost, jetReserve, jetConeWidth, jetRange, absorbRadius, absorbDuration, absorbCooldown];

    public override void ExposeData()
    {
        base.ExposeData();
        foreach (Tunable tunable in All)
            tunable.ExposeData();
        Scribe_Values.Look(ref hazardVacuum, "HazardVacuum");
        Scribe_Values.Look(ref spacerTier, "SpacerTier");
        Scribe_Values.Look(ref toxinVacuum, "ToxinVacuum");
        Scribe_Values.Look(ref feedToxAbsorption, "FeedToxAbsorption", true);
    }

    // Reads the def-backed defaults before the first ApplyToDefs overwrites them; defs must be
    // loaded, so ModInit's static constructor calls this, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        if (!TryGetDefs(out Defs defs))
            return;
        chemfuelPerCharge = Mathf.Max(1, defs.tank.ammoCountPerCharge);
        packCells = DefDatabase<ThingDef>.GetNamedSilentFail(ToxPackDefName)
            ?.GetCompProperties<CompProperties_ReleaseGas>()?.cellsToFill ?? packCells;
        absorbDraw = defs.absorbDraw;
        AbilityDef vent = DefDatabase<AbilityDef>.GetNamedSilentFail(VentDefName);
        tankIcon = vent?.icon;
        jetIcon = defs.jet.commandIcon.NullOrEmpty() ? null : ContentFinder<Texture2D>.Get(defs.jet.commandIcon, false);
        absorbIcon = defs.absorb.icon;
        tankTip = vent == null ? null : HeaderTip(vent.LabelCap, vent.description);
        jetTip = HeaderTip(defs.nozzle.jetLabel.CapitalizeFirst(), defs.nozzle.jetDescription);
        absorbTip = HeaderTip(defs.absorb.LabelCap, defs.absorb.description);
        tankCapacity.DefaultValue = defs.tank.maxCharges * chemfuelPerCharge;
        gasPerChemfuel.DefaultValue = defs.tank.cellsPerCharge / chemfuelPerCharge;
        ventRate.DefaultValue = defs.tank.ventChargesPerSecond * chemfuelPerCharge;
        jetCost.DefaultValue = defs.nozzle.tankChargesPerShot * chemfuelPerCharge;
        jetReserve.DefaultValue = defs.nozzle.maxCharges;
        jetConeWidth.DefaultValue = defs.jet.halfAngle * 2f;
        jetRange.DefaultValue = defs.jet.range;
        absorbRadius.DefaultValue = defs.absorb.radius;
        absorbDuration.DefaultValue = defs.absorb.durationTime.TicksToSeconds();
        absorbCooldown.DefaultValue = (float)defs.absorb.cooldownTime / GenDate.TicksPerHour;
        feedToxAbsorptionDefault = defs.tank.feedsToxAbsorption;
        defaultsLoaded = true;
    }

    public void ApplyToDefs()
    {
        if (!defaultsLoaded || !TryGetDefs(out Defs defs))
            return;
        defs.tank.maxCharges = Mathf.Max(1, tankCapacity.IntValue / chemfuelPerCharge);
        defs.tank.cellsPerCharge = gasPerChemfuel.Value * chemfuelPerCharge;
        defs.tank.ventChargesPerSecond = ventRate.Value / chemfuelPerCharge;
        defs.nozzle.tankChargesPerShot = Mathf.Max(1, jetCost.IntValue / chemfuelPerCharge);
        defs.nozzle.maxCharges = jetReserve.IntValue;
        defs.nozzle.ammoCountPerCharge = defs.nozzle.tankChargesPerShot * chemfuelPerCharge;
        defs.jet.halfAngle = jetConeWidth.Value / 2f;
        defs.jet.range = jetRange.Value;
        defs.absorb.radius = absorbRadius.Value;
        defs.absorb.durationTime = absorbDuration.Value.SecondsToTicks();
        defs.absorb.cooldownTime = absorbCooldown.IntValue * GenDate.TicksPerHour;
        defs.tank.feedsToxAbsorption = feedToxAbsorption;
        Generation++;
    }

    // The spacer tier's descriptions, from Keyed strings rather than from the patch: the XML
    // patches apply before the DefInjected translations, which would put the 7th-gen text back
    // for every language but English, while this runs after both injection passes (ModInit's
    // static constructor) and Keyed strings come translated per language. Startup only, as the
    // patch is, so the text never moves without the stats. Each description is its piece's short
    // description over the two paragraphs the pieces share, as the def XML lays them out.
    public void ApplySpacerText()
    {
        if (!spacerTier)
            return;
        SetSpacerText(ArmorDefName, "TXWC_SpacerShortDescription_Armor");
        SetSpacerText(ShouldersDefName, "TXWC_SpacerShortDescription_Shoulders");
        SetSpacerText(HelmetDefName, "TXWC_SpacerShortDescription_Helmet");
    }

    private static void SetSpacerText(string defName, string shortDescriptionKey)
    {
        if (DefDatabase<ThingDef>.GetNamedSilentFail(defName) is not WarcasketDef def)
        {
            Log.Error($"[Toxin Warcasket] Spacer tier setting could not find {defName}; it keeps its description.");
            return;
        }
        string shortDescription = shortDescriptionKey.Translate();
        def.shortDescription = shortDescription;
        def.description = shortDescription + "\n\n" + "TXWC_SpacerLoreGeneration".Translate() + "\n\n" + "TXWC_SpacerLoreSeries".Translate();
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
    //
    // A toggle's suffix names the one thing that stands between the player and it, highest first:
    // a missing mod or DLC ("requires ..."), which also disables the row, then a restart. A
    // requirement that is met is never named, in the label or the tooltip.
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
        SectionHeader(listing, "TXWC_SectionTank", tankIcon, tankTip, first: true);
        tankCapacity.DoRow(listing);
        gasPerChemfuel.DoRow(listing);
        ventRate.DoRow(listing);
        SummaryRow(listing, TankSummary);
        SectionHeader(listing, "TXWC_SectionJet", jetIcon, jetTip);
        jetCost.DoRow(listing);
        jetReserve.DoRow(listing);
        jetConeWidth.DoRow(listing);
        jetRange.DoRow(listing);
        SummaryRow(listing, JetSummary);
        SectionHeader(listing, "TXWC_SectionAbsorb", absorbIcon, absorbTip);
        absorbRadius.DoRow(listing);
        absorbDuration.DoRow(listing);
        absorbCooldown.DoRow(listing);
        SummaryRow(listing, AbsorbSummary);
        SectionHeader(listing, "TXWC_SectionMisc", null, null);
        DoSpacerTierRow(listing);
        DoToxinVacuumRow(listing);
        DoHazardVacuumRow(listing);
        DoFeedToxAbsorptionRow(listing);
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
            spacerTier = false;
            toxinVacuum = false;
            feedToxAbsorption = feedToxAbsorptionDefault;
        }
    }

    // What the sliders amount to, from their current values, in the numbers the player meets:
    // chemfuel, cells of gas, tox packs' worth (the pack's cellsToFill), jets and seconds.
    private string TankSummary()
    {
        int chemfuel = tankCapacity.IntValue;
        float cells = chemfuel * gasPerChemfuel.Value;
        float seconds = chemfuel / ventRate.Value;
        return "TXWC_TankSummary".Translate(chemfuel, cells.ToString("0.#"), (cells / packCells).ToString("0.#"), seconds.ToString("0.#"));
    }

    private string JetSummary()
    {
        int chemfuel = jetCost.IntValue;
        float cells = chemfuel * gasPerChemfuel.Value;
        int perTank = tankCapacity.IntValue / chemfuel;
        return "TXWC_JetSummary".Translate(chemfuel, cells.ToString("0.#"), (cells / packCells).ToString("0.#"), perTank, jetReserve.IntValue);
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

    // A muted line under a section's rows, indented and padded as they are, as tall as its text.
    // Nothing to sum up while the defs were not found, so the text is only built past that.
    private void SummaryRow(Listing_Standard listing, Func<string> summary)
    {
        if (!defaultsLoaded)
            return;
        string text = summary();
        float width = listing.ColumnWidth - Tunable.RowIndent - Tunable.RowEndPad;
        Rect rect = listing.GetRect(Text.CalcHeight(text, width));
        rect.xMin += Tunable.RowIndent;
        rect.xMax -= Tunable.RowEndPad;
        GUI.color = ColoredText.SubtleGrayColor;
        Widgets.Label(rect, text);
        GUI.color = Color.white;
    }

    // The colour of the Odyssey note in the spacer tier tooltip, whose vacuum resistance is the
    // one part of the tier that needs the DLC. Vanilla's one idiom for an expansion's name in
    // text (the research tab's source line) reads ExpansionDef.primaryColor, which the game's
    // own data makes white for Odyssey, so the note takes a purple from the colour library
    // instead, after the DLC's branding.
    private static readonly Color OdysseyColor = ColorLibrary.LightPurple;

    // The colour of Vanilla Races Expanded - Waster's title in the waster feed tooltip, set apart
    // from the Odyssey purple as the light green beside it in the colour library.
    private static readonly Color WasterColor = ColorLibrary.LightGreen;

    // The spacer tier rebuild; the patch and the text only read the box at startup, so the label
    // always says a change takes a restart. The tooltip's Odyssey note is passed in coloured so
    // translators never touch the markup, and only while Odyssey is missing, as the rows' own
    // requirement suffixes are. The tooltips are Resolve()d, never converted to string, because
    // TaggedString's implicit string conversion strips the colour tags.
    private void DoSpacerTierRow(Listing_Standard listing)
    {
        string label = "TXWC_SpacerTier".Translate() + "TXWC_RequiresRestartSuffix".Translate();
        string note = ModsConfig.OdysseyActive ? "" : "TXWC_RequiresOdysseySuffix".Translate().Colorize(OdysseyColor);
        string tooltip = "TXWC_SpacerTierDesc".Translate(note).Resolve();
        ToggleRow(listing, label, tooltip, ref spacerTier);
    }

    // The set's own vacuum resistance, shown off and disabled without Odyssey as the hazard row
    // is. While the spacer tier box is ticked (as it stands in the window, so before any restart)
    // the row is shown held on and disabled, with the suffix saying why.
    private void DoToxinVacuumRow(Listing_Standard listing)
    {
        bool odyssey = ModsConfig.OdysseyActive;
        string label = "TXWC_ToxinVacuum".Translate();
        bool? forced = null;
        if (!odyssey)
        {
            label += "TXWC_RequiresOdysseySuffix".Translate();
            forced = false;
        }
        else if (spacerTier)
        {
            label += "TXWC_IncludedInSpacerTierSuffix".Translate();
            forced = true;
        }
        else
            label += "TXWC_RequiresRestartSuffix".Translate();
        ToggleRow(listing, label, "TXWC_ToxinVacuumDesc".Translate(), ref toxinVacuum, forced);
    }

    // Shown off and disabled without Odyssey, which owns the stat, and the label says so; with
    // Odyssey it always says a change takes a restart, since the patch only reads the box at
    // startup.
    private void DoHazardVacuumRow(Listing_Standard listing)
    {
        bool odyssey = ModsConfig.OdysseyActive;
        string label = "TXWC_HazardVacuum".Translate();
        if (!odyssey)
            label += "TXWC_RequiresOdysseySuffix".Translate();
        else
            label += "TXWC_RequiresRestartSuffix".Translate();
        ToggleRow(listing, label, "TXWC_HazardVacuumDesc".Translate(), ref hazardVacuum, odyssey ? null : false);
    }

    // The tank's waster feed, shown off and disabled without Vanilla Races Expanded - Waster,
    // whose gene it feeds; the comp reads the box live, so no restart suffix. The tooltip takes
    // the mod's title as an argument, coloured so it stands out of the sentence and translators
    // never touch it.
    private void DoFeedToxAbsorptionRow(Listing_Standard listing)
    {
        bool waster = VREWasterIntegration.ModActive;
        string label = "TXWC_FeedToxAbsorption".Translate();
        if (!waster)
            label += "TXWC_RequiresVREWasterSuffix".Translate();
        string tooltip = "TXWC_FeedToxAbsorptionDesc".Translate(VREWasterIntegration.ModName.Colorize(WasterColor)).Resolve();
        ToggleRow(listing, label, tooltip, ref feedToxAbsorption, waster ? null : false);
    }

    // A checkbox row laid out as a Tunable row is, the box where a slider row shows its value.
    // A row with a forced value (a missing mod or DLC holds it off, another setting holds it on)
    // is disabled and shows that value, never the saved one, so it cannot read as on while it is
    // not; the field is left alone, so the player's choice returns once the row frees up. A
    // disabled row draws its label in the greyed box's colour too, since CheckboxLabeled only
    // greys the box.
    private static void ToggleRow(Listing_Standard listing, string label, string tooltip, ref bool value, bool? forced = null)
    {
        Rect row = listing.GetRect(Tunable.RowHeight);
        row.xMin += Tunable.RowIndent;
        row.xMax -= Tunable.RowEndPad;
        Widgets.DrawHighlightIfMouseover(row);
        TooltipHandler.TipRegion(row, tooltip);
        if (forced is bool shown)
        {
            GUI.color = Widgets.InactiveColor;
            Widgets.CheckboxLabeled(row, label, ref shown, disabled: true);
        }
        else
            Widgets.CheckboxLabeled(row, label, ref value);
        GUI.color = Color.white;
        listing.Gap(Tunable.RowGap);
    }

    // The header's text in the medium font after its icon, a square as tall as the header; a
    // header without one (the toggles', or any while the defs were not found) starts its text
    // at the left edge rather than after an empty slot. The tooltip covers the icon and the text
    // only, not the empty width after them.
    private static void SectionHeader(Listing_Standard listing, string key, Texture2D icon, string tooltip, bool first = false)
    {
        const float headerGap = 4f;
        const float headerHeight = 32f;
        const float iconPad = 8f;
        if (!first)
            listing.Gap(SectionGap);
        Rect rect = listing.GetRect(headerHeight);
        float left = rect.xMin;
        if (icon != null && icon != BaseContent.BadTex)
        {
            GUI.DrawTexture(rect.LeftPartPixels(headerHeight), icon);
            rect.xMin += headerHeight + iconPad;
        }
        GameFont font = Text.Font;
        TextAnchor anchor = Text.Anchor;
        Text.Font = GameFont.Medium;
        Text.Anchor = TextAnchor.MiddleLeft;
        string label = key.Translate();
        Widgets.Label(rect, label);
        if (!tooltip.NullOrEmpty())
            TooltipHandler.TipRegion(new Rect(left, rect.y, rect.xMin - left + Text.CalcSize(label).x, rect.height), tooltip);
        Text.Anchor = anchor;
        Text.Font = font;
        listing.Gap(headerGap);
    }

    // An ability's tooltip as VEF opens one: its name in vanilla's tip title colour over its
    // description. VEF's stat lines are left out; the rows below quote those numbers.
    private static string HeaderTip(string label, string description) =>
        description.NullOrEmpty() ? null : label.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + description;

    private struct Defs
    {
        public CompProperties_ToxTank tank;
        public CompProperties_JetNozzle nozzle;
        public VerbProperties_SprayGas jet;
        public AbilityDef absorb;
        public AbsorbGasExtension absorbDraw;
    }

    private static bool TryGetDefs(out Defs defs)
    {
        ThingDef shoulders = DefDatabase<ThingDef>.GetNamedSilentFail(ShouldersDefName);
        AbilityDef absorb = DefDatabase<AbilityDef>.GetNamedSilentFail(AbsorbDefName);
        defs = new Defs
        {
            tank = DefDatabase<ThingDef>.GetNamedSilentFail(ArmorDefName)?.GetCompProperties<CompProperties_ToxTank>(),
            nozzle = shoulders?.GetCompProperties<CompProperties_JetNozzle>(),
            jet = shoulders?.Verbs.OfType<VerbProperties_SprayGas>().FirstOrDefault(),
            absorb = absorb,
            absorbDraw = absorb?.GetModExtension<AbsorbGasExtension>(),
        };
        if (defs.tank != null && defs.nozzle != null && defs.jet != null && defs.absorb != null && defs.absorbDraw != null)
            return true;
        Log.Error("[Toxin Warcasket] Settings could not find the set's tank, gas jet or absorb def; they keep their XML values.");
        return false;
    }
}
