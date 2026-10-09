using System.Collections.Generic;
using Verse;
using VEF.Abilities;

namespace ToxinWarcasket;

// A dev-mode "reset cooldown" for the VEF abilities this apparel grants, beside the
// "DEV: Reload to full" that the tank's CompApparelVerbOwner_Charged base already offers. VEF
// gives its abilities no dev gizmo of their own; Ability.cooldown is the tick the cooldown ends.
// Declared on the def with plain CompProperties and this compClass.
public class CompDevResetCooldowns : ThingComp
{
    public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
    {
        foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
            yield return gizmo;
        if (!DebugSettings.ShowDevGizmos || parent.GetComp<CompAbilitiesApparel>() is not { } abilities)
            yield break;
        yield return new Command_Action
        {
            defaultLabel = "DEV: Reset cooldown",
            action = () =>
            {
                foreach (Ability ability in abilities.GivenAbilities)
                    ability.cooldown = 0;
            },
        };
    }
}
