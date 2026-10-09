using HarmonyLib;
using RimWorld;

namespace ToxinWarcasket.Patches;

// Makes the tox gas tank reload to its gauge's target (CompToxTank.TargetCharges) instead of to
// full, as a CompRefuelable fills to its TargetFuelLevel. CompApparelReloadable's reload queries
// are not virtual, so the tank cannot override them. Reloads count against the target whether
// automatic (JobGiver_Reload, any time the tank sits below it) or ordered from the float menu, as
// refuelling does; at the default target of a full tank this is vanilla's behaviour. Other
// reloadables pass through untouched.
//
// NeedsReload gates JobGiver_Reload, the float menu and MinAmmoNeeded. MaxAmmoNeeded caps the
// chemfuel the job carries (JobGiver_Reload.MakeReloadJob), and ReloadFrom loads what was
// carried, so the tank lands on the target. ReloadFrom itself is left alone: its only cap is the
// tank's size, reached only if the pawn already held extra chemfuel when the job began.
[HarmonyPatch(typeof(CompApparelReloadable))]
public static class CompApparelReloadable_ToxTank
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CompApparelReloadable.NeedsReload))]
    public static bool NeedsReload(CompApparelReloadable __instance, ref bool __result)
    {
        if (__instance is not CompToxTank tank)
            return true;
        __result = tank.RemainingCharges < tank.TargetCharges;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CompApparelReloadable.MaxAmmoNeeded))]
    public static bool MaxAmmoNeeded(CompApparelReloadable __instance, ref int __result)
    {
        if (__instance is not CompToxTank tank)
            return true;
        __result = tank.Props.ammoCountPerCharge * System.Math.Max(0, tank.TargetCharges - tank.RemainingCharges);
        return false;
    }
}
