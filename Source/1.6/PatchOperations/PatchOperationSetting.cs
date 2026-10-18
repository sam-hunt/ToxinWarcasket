using System.Reflection;
using System.Xml;
using Verse;

namespace ToxinWarcasket;

// Runs match while the named ToxinWarcasketSettings bool is on, nomatch otherwise; either branch
// may be absent. Patches apply after the Mod constructors have read the settings, so the field
// holds its saved value here, and a change takes effect at the next startup; the settings window
// says so on every toggle this gates. An absent branch succeeds without touching the XML, so the
// game logs no failed patch. A setting name that is not a public bool field is an error at patch
// time, where the smoke test reads it.
public class PatchOperationSetting : PatchOperation
{
    public string setting;
    public PatchOperation match;
    public PatchOperation nomatch;

    protected override bool ApplyWorker(XmlDocument xml)
    {
        FieldInfo field = typeof(ToxinWarcasketSettings).GetField(setting, BindingFlags.Instance | BindingFlags.Public);
        if (field == null || field.FieldType != typeof(bool))
        {
            Log.Error($"[Toxin Warcasket] {this}: no bool setting named '{setting}'.");
            return false;
        }
        bool on = (bool)field.GetValue(ToxinWarcasketMod.Settings);
        PatchOperation operation = on ? match : nomatch;
        return operation == null || operation.Apply(xml);
    }

    public override string ToString() => $"{base.ToString()}({setting})";
}
