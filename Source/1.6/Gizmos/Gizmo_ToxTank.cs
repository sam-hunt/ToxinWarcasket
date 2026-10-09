using RimWorld;
using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// The tox gas tank's gauge, the readout for the whole set's gas: vanilla's Gizmo_SetFuelLevel
// (the transport pod launcher's) over CompToxTank rather than a CompRefuelable. The bar is the
// tank's fill and, for the player's wearers, the drag handle sets TargetCharges, the level
// colonists reload to (Patches/CompApparelReloadable_ToxTank). The colours are the three tones of
// the armor texture's tox canisters, so the bar reads as the tank at a glance: the diffuse fills
// it, the highlight on hover, and the shade marks the target, which vanilla's pale marker would
// lose against the fill. Gizmo_Slider makes its bar textures on first draw, so CompToxTank keeps
// one instance.
public class Gizmo_ToxTank : Gizmo_Slider
{
    private static readonly Color CanisterShade = new(0x8a / 255f, 0x96 / 255f, 0x57 / 255f);
    private static readonly Color CanisterDiffuse = new(0xb0 / 255f, 0xbf / 255f, 0x73 / 255f);
    private static readonly Color CanisterHighlight = new(0xe6 / 255f, 0xef / 255f, 0xbd / 255f);

    private static bool draggingBar;

    private readonly CompToxTank tank;

    public Gizmo_ToxTank(CompToxTank tank)
    {
        this.tank = tank;
    }

    protected override float Target
    {
        get => (float)tank.TargetCharges / tank.MaxCharges;
        set => tank.TargetCharges = Mathf.RoundToInt(value * tank.MaxCharges);
    }

    protected override float ValuePercent => tank.FillPercent;

    // Vanilla's own gas label (Core's ToxGas key, via GasUtility.GetLabel), so every language
    // already has it.
    protected override string Title => GasType.ToxGas.GetLabel().CapitalizeFirst();

    protected override bool IsDraggable => tank.Wearer?.Faction == Faction.OfPlayer;

    protected override string BarLabel => $"{tank.RemainingCharges} / {tank.MaxCharges}";

    protected override Color BarColor => CanisterDiffuse;

    protected override Color BarHighlightColor => CanisterHighlight;

    protected override Color BarDragColor => CanisterShade;

    protected override bool DraggingBar
    {
        get => draggingBar;
        set => draggingBar = value;
    }

    protected override string GetTooltip() => "";
}
