using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using AbilityDef = VEF.Abilities.AbilityDef;

namespace ToxinWarcasket;

// The set's mod settings: the window frame, the ExposeData / LoadDefaultsFromDefs / ApplyToDefs /
// ResetToDefaults fan-out and the shared row helpers. The class is split across
// Settings_<Section>.cs partials, one per window section, each owning its fields, scribe
// entries, def-backed defaults, apply and draw, so a setting is a one-file edit. Each Tunable
// overrides a def field, written onto the def's props by ApplyToDefs (on every play-data load,
// then whenever the window closes), so every reader of those props, vanilla's and VEF's
// included, sees the setting with no code of its own. The defaults, and the rationale for them,
// are the defs'.
//
// The sliders read in chemfuel, the one cost the player pays: the tank's and the jet's size in
// chemfuel, the gas yield in cells per chemfuel, the vent in chemfuel a second. The code keeps
// vanilla's "charge", the reloadable comp's own term, and the armor def makes a charge one
// chemfuel (its ammoCountPerCharge), so the gauge's "unit" (the tank's chargeNoun) is a chemfuel
// too; the conversion through ammoCountPerCharge here only keeps the window honest should a
// patch change that. Each section ends on a line of what its sliders amount to (a full tank, a
// jet, an absorb), so the balance reads off the window rather than being multiplied out.
public partial class ToxinWarcasketSettings : ModSettings
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

    private bool defaultsLoaded;
    private Vector2 scrollPosition;
    private float contentHeight;

    // Read with the defaults: the chemfuel a tank charge costs (the sliders' unit over the
    // charge fields) and the tox pack's cells (the summaries' yardstick).
    private int chemfuelPerCharge = 1;
    private float packCells = 45f;

    // Gas units (GasGrid.MaxGasPerCell to a full cell) one tank charge makes, wherever the set
    // spends or banks one: the vent, the bursts, the jet from the tank or its reserve, the absorb.
    public float GasPerCharge => gasPerChemfuel.Value * chemfuelPerCharge * GasGrid.MaxGasPerCell;

    public override void ExposeData()
    {
        base.ExposeData();
        ExposeTankSettings();
        ExposeJetSettings();
        ExposeAbsorbSettings();
        ExposeMiscSettings();
    }

    // Reads the def-backed defaults before ApplyToDefs overwrites them; defs must be loaded, so
    // Startup.Run calls this once per load, never the Mod constructor.
    public void LoadDefaultsFromDefs()
    {
        if (!TryGetDefs(out Defs defs))
            return;
        chemfuelPerCharge = Mathf.Max(1, defs.tank.ammoCountPerCharge);
        packCells = DefDatabase<ThingDef>.GetNamedSilentFail(ToxPackDefName)
            ?.GetCompProperties<CompProperties_ReleaseGas>()?.cellsToFill ?? packCells;
        LoadTankDefaults(defs);
        LoadJetDefaults(defs);
        LoadAbsorbDefaults(defs);
        LoadMiscDefaults(defs);
        defaultsLoaded = true;
    }

    public void ApplyToDefs()
    {
        if (!defaultsLoaded || !TryGetDefs(out Defs defs))
            return;
        ApplyTankToDefs(defs);
        ApplyJetToDefs(defs);
        ApplyAbsorbToDefs(defs);
        ApplyMiscToDefs(defs);
        Generation++;
    }

    public void ResetToDefaults()
    {
        ResetTankSettings();
        ResetJetSettings();
        ResetAbsorbSettings();
        ResetMiscSettings();
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
        DrawTankSection(listing);
        DrawJetSection(listing);
        DrawAbsorbSection(listing);
        DrawMiscSection(listing);
        listing.Gap(SectionGap);
        contentHeight = listing.CurHeight;
        listing.End();
        Widgets.EndScrollView();

        if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - buttonHeight, buttonWidth, buttonHeight),
                "RestoreToDefaultSettings".Translate()))
        {
            ResetToDefaults();
        }
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
