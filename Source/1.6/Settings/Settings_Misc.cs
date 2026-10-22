using RimWorld;
using ToxinWarcasket.Integrations;
using UnityEngine;
using Verse;
using VFEPirates;

namespace ToxinWarcasket;

// Miscellaneous section: the toggles that are not sliders. hazardVacuum, spacerTier and
// toxinVacuum are read by PatchOperationSetting while the XML patches apply, so a change waits
// for a restart; feedToxAbsorption is written onto the tank's feedsToxAbsorption and read live.
// Each row's suffix and forced value follow the mod or DLC it needs.
public partial class ToxinWarcasketSettings
{
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

    private void ExposeMiscSettings()
    {
        Scribe_Values.Look(ref hazardVacuum, "HazardVacuum");
        Scribe_Values.Look(ref spacerTier, "SpacerTier");
        Scribe_Values.Look(ref toxinVacuum, "ToxinVacuum");
        Scribe_Values.Look(ref feedToxAbsorption, "FeedToxAbsorption", true);
    }

    private void LoadMiscDefaults(Defs defs)
    {
        feedToxAbsorptionDefault = defs.tank.feedsToxAbsorption;
    }

    private void ApplyMiscToDefs(Defs defs)
    {
        defs.tank.feedsToxAbsorption = feedToxAbsorption;
    }

    private void ResetMiscSettings()
    {
        hazardVacuum = false;
        spacerTier = false;
        toxinVacuum = false;
        feedToxAbsorption = feedToxAbsorptionDefault;
    }

    private void DrawMiscSection(Listing_Standard listing)
    {
        SectionHeader(listing, "TXWC_SectionMisc", null, null);
        DoSpacerTierRow(listing);
        DoToxinVacuumRow(listing);
        DoHazardVacuumRow(listing);
        DoFeedToxAbsorptionRow(listing);
    }

    // The spacer tier's descriptions, from Keyed strings rather than from the patch: the XML
    // patches apply before the DefInjected translations, which would put the 7th-gen text back
    // for every language but English, while this runs after both injection passes (Startup.Run,
    // once per load) and Keyed strings come translated per language. Load time only, as the
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
}
