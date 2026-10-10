using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace ToxinWarcasket;

// PatchOperationFindMod by package id rather than display name: runs match when any of packageIds
// is active, otherwise nomatch. ignorePostfix also matches the "_steam" id the game gives a
// Workshop copy loaded beside a local one.
public class PatchOperationModActive : PatchOperation
{
    public List<string> packageIds;
    public PatchOperation match;
    public PatchOperation nomatch;

    protected override bool ApplyWorker(XmlDocument xml)
    {
        bool active = packageIds.Any(id => ModLister.GetActiveModWithIdentifier(id, ignorePostfix: true) != null);
        PatchOperation operation = active ? match : nomatch;
        return operation == null || operation.Apply(xml);
    }

    public override string ToString() => $"{base.ToString()}({packageIds.ToCommaList()})";
}
