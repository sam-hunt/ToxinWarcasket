using System.Xml;
using Verse;

namespace ToxinWarcasket;

// Runs its match operation only while the hazard vacuum setting is on and Odyssey, which owns the
// VacuumResistance stat, is active (1.6/Patches/HazardVacuumResistance.xml has the values). Patches
// apply after the Mod constructors have read the settings, so the setting holds its saved value
// here, and a change takes effect at the next startup. Off, it succeeds without touching the XML,
// so the game logs no failed patch.
public class PatchOperationHazardVacuum : PatchOperation
{
    public PatchOperation match;

    protected override bool ApplyWorker(XmlDocument xml)
    {
        if (!ToxinWarcasketMod.Settings.hazardVacuum || !ModsConfig.OdysseyActive)
            return true;
        return match.Apply(xml);
    }
}
