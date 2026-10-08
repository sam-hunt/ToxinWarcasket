using Verse;
using Ability = VEF.Abilities.Ability;

namespace ToxinWarcasket;

// The armor's vent toggle. It only flips CompToxTank.Venting: the tank ticks the emission and
// saves the state, so this class holds none. VEF's base gizmo is a cast command, so GetGizmo
// hands out a toggle instead, as VFEP's Ability_SiegeMode does (with a command of our own rather
// than a dependency on VFEP's CommandAbilityToggle). VEF asks for gizmos every frame a wearer is
// selected; one command is kept and only its live fields refreshed, after Shipcracker's
// Ability_BreachJump.
public class Ability_VentTank : Ability
{
    private CompToxTank tank;
    private Command_VentTank gizmo;

    // holder is the armor; it never changes for this instance.
    private CompToxTank Tank => tank ??= holder.TryGetComp<CompToxTank>();

    // VEF runs Init again when the apparel changes wearer.
    public override void Init()
    {
        base.Init();
        gizmo = null;
    }

    public override Gizmo GetGizmo()
    {
        CompToxTank t = Tank;
        gizmo ??= new Command_VentTank(t)
        {
            defaultLabel = def.LabelCap,
            icon = def.icon,
            isActive = () => t.Venting,
            toggleAction = () => t.SetVenting(!t.Venting),
        };
        gizmo.defaultDesc = GetDescriptionForPawn();
        gizmo.Disabled = false;
        if (!t.Venting && t.RemainingCharges <= 0)
            gizmo.Disable(t.DisabledReason(t.MinAmmoNeeded(false), t.MaxAmmoNeeded(false)));
        else if (!IsEnabledForPawn(out string reason))
            gizmo.Disable(reason.Colorize(ColorLibrary.RedReadable));
        return gizmo;
    }
}

// A toggle whose corner label is the tank's fill, the readout for the whole set's reagent.
public class Command_VentTank : Command_Toggle
{
    private readonly CompToxTank tank;

    public Command_VentTank(CompToxTank tank)
    {
        this.tank = tank;
    }

    public override string TopRightLabel => tank.FillPercent.ToStringPercent();
}
