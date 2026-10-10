using System;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// One player-tunable number, held in the units its settings row shows. Until the player moves its
// slider it is DefaultValue, read from the def field it overrides when defs load
// (ToxinWarcasketSettings.LoadDefaultsFromDefs), so the def and its header stay the record of the
// shipped tuning and another mod's XML patch moves the default too. Only an override is saved; a
// slider landing within half a step of the default clears it, and a saved override outside the
// slider's range (saved before the range moved) reads as its nearest end.
public sealed class Tunable
{
    private const float Unset = -1f;

    // A row is the label, the slider and the value side by side, as a vanilla SliderLabeled row
    // is, with a gap below it so neighbouring rows read apart, indented so it sits under its
    // section header (Listing.Indent only moves the cursor and would push the row off the right)
    // and padded on the right so it does not run up against the scrollbar.
    // The value column is sized to the widest value a row can show, so the slider runs out to
    // it and the row ends near the window's edge as the checkbox rows do; every format here is
    // at most four characters, so "00.00" with the default suffix (as translated) bounds it.
    // The settings' checkbox rows share the height, gap, indent and pad.
    internal const float RowHeight = 30f;
    internal const float RowGap = 6f;
    internal const float RowIndent = Listing.DefaultIndent;
    internal const float RowEndPad = Listing.DefaultIndent;
    private const float LabelShare = 0.4f;
    private const string WidestValue = "00.00";
    private const float SliderPad = 10f;

    private readonly string key;
    private readonly float min;
    private readonly float max;
    private readonly float step;
    private readonly Func<float, string> format;
    private float value = Unset;

    public Tunable(string key, float min, float max, float step, Func<float, string> format)
    {
        this.key = key;
        this.min = min;
        this.max = max;
        this.step = step;
        this.format = format;
        DefaultValue = Unset;
    }

    public float DefaultValue { get; set; }

    public float Value => value < 0f ? DefaultValue : Mathf.Clamp(value, min, max);

    public int IntValue => Mathf.RoundToInt(Value);

    public void Reset() => value = Unset;

    public void ExposeData() => Scribe_Values.Look(ref value, key, Unset);

    // Label "TXWC_<key>" (the unit is part of it) on the left, the slider in the middle, the
    // current value on the right with "(default)" while unset, and "TXWC_<key>Desc" with the
    // default as the whole row's tooltip.
    public void DoRow(Listing_Standard listing)
    {
        Rect row = listing.GetRect(RowHeight);
        row.xMin += RowIndent;
        row.xMax -= RowEndPad;
        Rect labelRect = row.LeftPart(LabelShare);
        float valueWidth = Text.CalcSize(WidestValue + "TXWC_DefaultSuffix".Translate()).x;
        var valueRect = new Rect(row.xMax - valueWidth, row.y, valueWidth, row.height);
        var sliderRect = new Rect(labelRect.xMax + SliderPad, row.y,
            valueRect.x - labelRect.xMax - 2f * SliderPad, row.height);

        Widgets.DrawHighlightIfMouseover(row);
        TooltipHandler.TipRegion(row, ("TXWC_" + key + "Desc").Translate(format(DefaultValue)));

        string shown = format(Value);
        if (value < 0f)
            shown += "TXWC_DefaultSuffix".Translate();
        TextAnchor anchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(labelRect, ("TXWC_" + key).Translate());
        Widgets.Label(valueRect, shown);
        Text.Anchor = anchor;

        float current = Value;
        float raw = Widgets.HorizontalSlider(sliderRect, current, min, max, middleAlignment: true);
        listing.Gap(RowGap);
        if (raw == current)
            return;
        float snapped = Mathf.Clamp(min + Mathf.Round((raw - min) / step) * step, min, max);
        value = Mathf.Abs(snapped - DefaultValue) < step * 0.5f ? Unset : snapped;
    }
}
