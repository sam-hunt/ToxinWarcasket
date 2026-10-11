using Verse;
using Ability = VEF.Abilities.Ability;

namespace ToxinWarcasket;

// The armor's vent toggle. It only flips CompToxTank.Venting: the tank ticks the emission and
// saves the state, so this class holds none. VEF's base gizmo is a cast command, so GetGizmo
// hands out a plain Command_Toggle instead, as VFEP's Ability_SiegeMode does (without a
// dependency on VFEP's CommandAbilityToggle); the tank's own gauge shows the fill. VEF asks for
// gizmos every frame a wearer is selected; one command is kept and only its live fields
// refreshed, after Shipcracker's Ability_BreachJump, and the two strings it shows are built
// once per settings write (ToxinWarcasketSettings.Generation), as VEF's description formats
// and translates every line and the empty-tank reason quotes the tank's size. That reason
// quotes the tank's whole range rather than MinAmmoNeeded/MaxAmmoNeeded, which follow the
// gauge's reload target.
public class Ability_VentTank : Ability
{
    private CompToxTank tank;
    private Command_Toggle gizmo;
    private string description;
    private string emptyReason;
    private int settingsGeneration = -1;

    // holder is the armor; it never changes for this instance.
    private CompToxTank Tank => tank ??= holder.TryGetComp<CompToxTank>();

    // A toggle has nothing to autocast; VEF offers it to any one-target ability and says so in
    // the tooltip GetDescriptionForPawn builds.
    public override bool CanAutoCast => false;

    // VEF runs Init again when the apparel changes wearer.
    public override void Init()
    {
        base.Init();
        gizmo = null;
        description = null;
    }

    public override Gizmo GetGizmo()
    {
        CompToxTank t = Tank;
        if (description == null || settingsGeneration != ToxinWarcasketSettings.Generation)
        {
            settingsGeneration = ToxinWarcasketSettings.Generation;
            description = GetDescriptionForPawn();
            emptyReason = t.DisabledReason(t.Props.ammoCountPerCharge, t.MaxAmmoAmount());
        }
        gizmo ??= new Command_Toggle
        {
            defaultLabel = def.LabelCap,
            icon = def.icon,
            isActive = () => t.Venting,
            toggleAction = () => t.SetVenting(!t.Venting),
        };
        gizmo.defaultDesc = description;
        gizmo.Disabled = false;
        if (!t.Venting && t.RemainingCharges <= 0)
            gizmo.Disable(emptyReason);
        else if (!IsEnabledForPawn(out string reason))
            gizmo.Disable(reason.Colorize(ColorLibrary.RedReadable));
        return gizmo;
    }
}

