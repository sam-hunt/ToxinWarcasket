using HarmonyLib;
using RimWorld;

namespace ToxinWarcasket.Patches;

// The set's two reloadables bend vanilla's reload queries, which are not virtual, so the comps
// cannot override them. Other reloadables pass through untouched.
//
// The tox gas tank reloads to its gauge's target (CompToxTank.TargetCharges) instead of to full,
// as a CompRefuelable fills to its TargetFuelLevel. Reloads count against the target whether
// automatic (JobGiver_Reload, any time the tank sits below it) or ordered from the float menu, as
// refuelling does; at the default target of a full tank this is vanilla's behaviour.
//
// The jet nozzle's own jets are a fallback for shoulders worn without a tank (CompJetNozzle), so
// while the wearer wears one they never need reloading: no colonist carries chemfuel to them and
// the float menu offers none, and whatever they hold waits for the tank to come off.
//
// NeedsReload gates JobGiver_Reload, the float menu, MinAmmoNeeded and ReloadFrom; a false here
// zeroes the two amounts and makes the reload a no-op. MaxAmmoNeeded caps the chemfuel the job
// carries (JobGiver_Reload.MakeReloadJob), and ReloadFrom loads what was carried, so the tank
// lands on the target. ReloadFrom itself is left alone: its only cap is the tank's size, reached
// only if the pawn already held extra chemfuel when the job began.
[HarmonyPatch(typeof(CompApparelReloadable))]
public static class CompApparelReloadable_Reload
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CompApparelReloadable.NeedsReload))]
    public static bool NeedsReload(CompApparelReloadable __instance, ref bool __result)
    {
        switch (__instance)
        {
            case CompToxTank tank:
                __result = tank.RemainingCharges < tank.TargetCharges;
                return false;
            case CompJetNozzle nozzle when nozzle.WornTank != null:
                __result = false;
                return false;
            default:
                return true;
        }
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
