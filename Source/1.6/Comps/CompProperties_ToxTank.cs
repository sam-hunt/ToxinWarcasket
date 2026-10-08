using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ToxinWarcasket;

// The armor's reagent tank (see CompToxTank). The reload fields (maxCharges, ammoDef,
// ammoCountPerCharge, chargeNoun, ...) are vanilla CompApparelReloadable's; everything below
// tunes what the tank does with its charges. Values live in the armor def, rationale in its
// header.
public class CompProperties_ToxTank : CompProperties_ApparelReloadable
{
    // Gas units one charge releases: 255 is one cell at full density (GasGrid.MaxGasPerCell).
    public int gasPerCharge = 255;

    // Fewest charges that make a downed or death burst; below this the tank just stays put.
    public int minBurstCharges = 5;

    // Vent emission: one charge every ventTicksPerCharge ticks.
    public int ventTicksPerCharge = 6;
    public EffecterDef ventEffecter;

    // Opportunistic vent for non-player wearers, after CompToxPack.ChanceToUse: every
    // aiCheckInterval ticks, start venting once hostile pawns the gas affects within
    // aiTriggerRadius sum to aiTriggerBodySize, and stop after aiStopAfterTicks without any.
    public int aiCheckInterval = 60;
    public int aiMinCharges = 20;
    public float aiTriggerRadius = 4f;
    public float aiTriggerBodySize = 1f;
    public int aiStopAfterTicks = 300;

    // Death rupture: pollution over every cell within ruptureRadius, ruptureCenterFilth under
    // the body, and on each other cell a ruptureFilthChance roll for one of ruptureFilth, then a
    // ruptureSecondFilthChance roll for a second of a different kind.
    public float ruptureRadius = 2f;
    public ThingDef ruptureCenterFilth;
    public List<ThingDef> ruptureFilth = new();
    public float ruptureFilthChance = 0.7f;
    public float ruptureSecondFilthChance = 0.3f;

    public CompProperties_ToxTank()
    {
        compClass = typeof(CompToxTank);
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (string error in base.ConfigErrors(parentDef))
            yield return error;
        if (gasPerCharge <= 0)
            yield return "gasPerCharge must be positive";
        if (ventTicksPerCharge <= 0)
            yield return "ventTicksPerCharge must be positive";
        if (aiCheckInterval <= 0)
            yield return "aiCheckInterval must be positive";
    }
}
