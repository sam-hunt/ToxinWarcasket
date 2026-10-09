using UnityEngine;
using Verse;

namespace ToxinWarcasket;

// Owns the mod settings only. The constructor runs before defs load, so it neither patches
// (ModInit's comment) nor touches the defs the settings override: ModInit reads their defaults
// and applies the settings once defs exist, and WriteSettings re-applies them whenever the
// settings window closes.
public class ToxinWarcasketMod : Mod
{
    public ToxinWarcasketMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<ToxinWarcasketSettings>();
    }

    public static ToxinWarcasketSettings Settings { get; private set; }

    public override string SettingsCategory() => "TXWC_SettingsCategory".Translate();

    public override void DoSettingsWindowContents(Rect inRect) => Settings.DoWindowContents(inRect);

    public override void WriteSettings()
    {
        base.WriteSettings();
        Settings.ApplyToDefs();
    }
}
