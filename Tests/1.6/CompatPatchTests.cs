#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;

namespace ToxinWarcasket.Tests;

// CombatExtended.xml and SpacerTier.xml share 1.6/Patches/, and patch files within a mod apply in
// filesystem order, so neither may depend on running after the other. Under Combat Extended the
// first owns the plating, shield stats and costs of both tiers and the second skips its own
// versions of exactly those, which keeps the two off each other's nodes. These tests hold that
// split: every xpath resolves against the unpatched defs to one node per def it names, and a
// small interpreter of the operations both files use applies them in both orders, for every
// combination of Combat Extended and the spacer tier setting, to the same result with no
// duplicated stat or cost and the numbers in the table below.
public class CompatPatchTests
{
    private const string Armor = "TXWC_Warcasket_Toxin";
    private const string Shoulders = "TXWC_WarcasketShoulders_Toxin";
    private const string Helmet = "TXWC_WarcasketHelmet_Toxin";
    private static readonly string[] Pieces = { Armor, Shoulders, Helmet };

    private const string CombatExtendedPatch = "1.6/Patches/CombatExtended.xml";
    private const string SpacerTierPatch = "1.6/Patches/SpacerTier.xml";
    private const string CombatExtendedId = "ceteam.combatextended";

    // Table cell markers: the node must be absent, or present with any value.
    private const string? Absent = null;
    private const string Present = "*";

    private const string ShieldComp = "comps/li[@Class = \"VEF.Apparels.CompProperties_ShieldBubble\"]";

    private static readonly string RepoRoot = FindRepoRoot();

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "ToxinWarcasket.sln")))
                return dir.FullName;
        }
        throw new InvalidOperationException("ToxinWarcasket.sln not found above the test output folder");
    }

    private static XDocument MergedDefs()
    {
        var defs = new XElement("Defs");
        foreach (var file in Directory.EnumerateFiles(Path.Combine(RepoRoot, "1.6/Defs"), "*.xml", SearchOption.AllDirectories))
            defs.Add(XDocument.Load(file).Root!.Elements());
        return new XDocument(defs);
    }

    private static XDocument LoadPatch(string relativePath) => XDocument.Load(Path.Combine(RepoRoot, relativePath));

    // ===== Xpath resolution =====

    [Theory]
    [InlineData(CombatExtendedPatch)]
    [InlineData(SpacerTierPatch)]
    public void EveryXPath_ResolvesToOneNodePerNamedDef(string relativePath)
    {
        var defs = MergedDefs();
        var operations = LoadPatch(relativePath).Descendants()
            .Where(e => e.Attribute("Class") != null && e.Element("xpath") != null)
            .ToList();
        Assert.NotEmpty(operations);
        foreach (var op in operations)
        {
            var xpath = op.Element("xpath")!.Value;
            var hits = defs.XPathSelectElements(xpath).Count();
            var expected = (string)op.Attribute("Class")! == "PatchOperationConditional"
                ? 0
                : Regex.Matches(xpath, "defName\\s*=\\s*\"").Count;
            Assert.True(hits == expected, $"{relativePath}: {xpath} resolved {hits} node(s), expected {expected}");
        }
    }

    // ===== Order independence =====

    // One stat row: a path relative to the def and its text in the armor, shoulders and helmet.
    private sealed class Row
    {
        public Row(string path, string? armor, string? shoulders, string? helmet)
        {
            Path = path;
            Values = new[] { armor, shoulders, helmet };
        }

        public string Path { get; }
        public string?[] Values { get; }
    }

    private static Row R(string path, string? armor, string? shoulders, string? helmet) => new(path, armor, shoulders, helmet);

    private static Row All(string path, string? value) => new(path, value, value, value);

    // costList children in order, "Name count, Name count", for armor, shoulders and helmet.
    private static string[] Costs(string armor, string shoulders, string helmet) => new[] { armor, shoulders, helmet };

    private static readonly Dictionary<(bool Ce, bool Spacer), (Row[] Rows, string[] Costs)> Expected = new()
    {
        [(false, false)] = (new[]
        {
            All("statBases/ArmorRating_Sharp", "1.65"),
            All("statBases/ArmorRating_Blunt", "0.72"),
            All("statBases/ArmorRating_Heat", "0.86"),
            All("statBases/Bulk", Absent),
            All("statBases/WornBulk", Absent),
            All("statBases/EnergyShieldEnergyMax", Absent),
            All("statBases/EnergyShieldRechargeRate", Absent),
            All("equippedStatOffsets/MoveSpeed", Present),
            All(ShieldComp, Absent),
        }, Costs(
            "ComponentIndustrial 8, Steel 140, Plasteel 20, Uranium 50, Chemfuel 30",
            "ComponentIndustrial 2, Steel 55, Plasteel 20, Chemfuel 15",
            "ComponentIndustrial 3, Steel 45, Plasteel 20, Chemfuel 10")),

        [(false, true)] = (new[]
        {
            All("statBases/ArmorRating_Sharp", "2"),
            All("statBases/ArmorRating_Blunt", "1"),
            All("statBases/ArmorRating_Heat", "1"),
            R("statBases/Mass", "50", "20", "8"),
            All("statBases/Bulk", Absent),
            All("statBases/WornBulk", Absent),
            R("statBases/EnergyShieldEnergyMax", "1.5", Absent, Absent),
            R("statBases/EnergyShieldRechargeRate", "0.01", Absent, Absent),
            All("equippedStatOffsets/MoveSpeed", Absent),
            R(ShieldComp, Present, Absent, Absent),
        }, Costs(
            "ComponentSpacer 6, Plasteel 150, Uranium 50, Chemfuel 30",
            "ComponentSpacer 2, Plasteel 60, Chemfuel 15",
            "ComponentSpacer 4, Plasteel 60, Chemfuel 10")),

        [(true, false)] = (new[]
        {
            R("statBases/ArmorRating_Sharp", "28", "28", "25"),
            R("statBases/ArmorRating_Blunt", "70", "70", "56"),
            All("statBases/ArmorRating_Heat", "0.86"),
            R("statBases/Bulk", "250", "15", "15"),
            R("statBases/WornBulk", "25", "5", "8"),
            R("statBases/NightVisionEfficiency_Apparel", Absent, Absent, "0.5"),
            R("equippedStatOffsets/SmokeSensitivity", Absent, Absent, "-1"),
            R("equippedStatOffsets/CarryWeight", "250", Absent, Absent),
            R("equippedStatOffsets/CarryBulk", "200", Absent, Absent),
            R("equippedStatOffsets/Suppressability", "-100", Absent, Absent),
            All("equippedStatOffsets/MoveSpeed", Present),
            All("statBases/EnergyShieldEnergyMax", Absent),
            All("statBases/EnergyShieldRechargeRate", Absent),
            All(ShieldComp, Absent),
        }, Costs(
            "ComponentIndustrial 16, Steel 280, Plasteel 20, Uranium 50, Chemfuel 30",
            "ComponentIndustrial 4, Steel 110, Plasteel 20, Chemfuel 15",
            "ComponentIndustrial 6, Steel 90, Plasteel 20, Chemfuel 10")),

        [(true, true)] = (new[]
        {
            R("statBases/ArmorRating_Sharp", "36", "36", "32"),
            R("statBases/ArmorRating_Blunt", "90", "90", "72"),
            All("statBases/ArmorRating_Heat", "1"),
            R("statBases/Mass", "50", "20", "8"),
            All("equippedStatOffsets/MoveSpeed", Absent),
            R("statBases/Bulk", "275", "15", "20"),
            R("statBases/WornBulk", "35", "5", "10"),
            R("statBases/NightVisionEfficiency_Apparel", Absent, Absent, "0.75"),
            R("equippedStatOffsets/SmokeSensitivity", Absent, Absent, "-1"),
            R("equippedStatOffsets/CarryWeight", "250", Absent, Absent),
            R("equippedStatOffsets/CarryBulk", "225", Absent, Absent),
            R("equippedStatOffsets/Suppressability", "-100", Absent, Absent),
            R("statBases/EnergyShieldEnergyMax", "2.25", Absent, Absent),
            R("statBases/EnergyShieldRechargeRate", "0.015", Absent, Absent),
            R(ShieldComp, Present, Absent, Absent),
        }, Costs(
            "ComponentSpacer 8, Plasteel 225, Uranium 50, Chemfuel 30",
            "ComponentSpacer 2, Plasteel 90, Chemfuel 15",
            "ComponentSpacer 6, Plasteel 90, Chemfuel 10")),
    };

    private static readonly string[] UniqueChildContainers = { "statBases", "equippedStatOffsets", "costList" };

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void BothOrders_GiveTheSameDefs(bool ceActive, bool spacerTier)
    {
        var context = new PatchContext(ceActive ? new[] { CombatExtendedId } : Array.Empty<string>(), spacerTier);
        var forward = Patched(context, CombatExtendedPatch, SpacerTierPatch);
        var reverse = Patched(context, SpacerTierPatch, CombatExtendedPatch);

        var forwardStats = WithStatListsSorted(forward);
        var reverseStats = WithStatListsSorted(reverse);
        Assert.True(XNode.DeepEquals(forwardStats, reverseStats),
            $"ceActive={ceActive}, spacerTier={spacerTier}: the two application orders differ at {FirstDifference(forwardStats.Root!, reverseStats.Root!)}");

        var (rows, costs) = Expected[(ceActive, spacerTier)];
        for (var i = 0; i < Pieces.Length; i++)
        {
            var def = forward.XPathSelectElements($"/Defs/VFEPirates.WarcasketDef[defName = \"{Pieces[i]}\"]").Single();
            var where = $"ceActive={ceActive}, spacerTier={spacerTier}, {Pieces[i]}";

            foreach (var container in UniqueChildContainers)
            {
                var node = def.Elements(container).ToList();
                Assert.True(node.Count == 1, $"{where}: {node.Count} {container} node(s), expected 1");
                var duplicates = node[0].Elements().GroupBy(e => e.Name.LocalName).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                Assert.True(duplicates.Count == 0, $"{where}: duplicated {container} children {string.Join(", ", duplicates)}");
            }

            foreach (var row in rows)
            {
                var expected = row.Values[i];
                var hits = def.XPathSelectElements(row.Path).ToList();
                if (expected == Absent)
                {
                    Assert.True(hits.Count == 0, $"{where}: {row.Path} present ({string.Join(", ", hits.Select(h => h.Value))}), expected absent");
                    continue;
                }
                Assert.True(hits.Count == 1, $"{where}: {row.Path} matched {hits.Count} node(s), expected 1");
                if (expected != Present)
                    Assert.True(hits[0].Value == expected, $"{where}: {row.Path} is {hits[0].Value}, expected {expected}");
            }

            var actualCosts = string.Join(", ", def.Element("costList")!.Elements().Select(e => $"{e.Name.LocalName} {e.Value}"));
            Assert.True(actualCosts == costs[i], $"{where}: costList is {actualCosts}, expected {costs[i]}");
        }
    }

    // Both files append to the same equippedStatOffsets under Combat Extended (the helmet's
    // SmokeSensitivity beside the spacer tier's PsychicSensitivity and ToxicResistance), so the
    // order of those children follows the order the files apply in. The game reads statBases and
    // equippedStatOffsets into StatModifier lists looked up by stat, where order carries no
    // meaning, so the comparison sorts them by name; the duplicate check keeps that sort lossless.
    // costList keeps its order: it is the order the bill lists the ingredients in.
    private static readonly string[] StatLists = { "statBases", "equippedStatOffsets" };

    private static XDocument WithStatListsSorted(XDocument defs)
    {
        var copy = new XDocument(defs);
        foreach (var list in copy.Root!.Elements().SelectMany(def => StatLists.SelectMany(name => def.Elements(name))))
            list.ReplaceNodes(list.Elements().OrderBy(e => e.Name.LocalName, StringComparer.Ordinal).ToList());
        return copy;
    }

    // The path to the first element whose name, attributes, text or children differ, with both
    // versions of it, for the failure message.
    private static string FirstDifference(XElement a, XElement b)
    {
        var path = a.Name.LocalName + (a.Element("defName") is { } defName ? $"[{defName.Value}]" : "");
        var childrenA = a.Elements().ToList();
        var childrenB = b.Elements().ToList();
        if (childrenA.Count == childrenB.Count && childrenA.Zip(childrenB, (x, y) => x.Name == y.Name).All(same => same))
        {
            for (var i = 0; i < childrenA.Count; i++)
            {
                if (!XNode.DeepEquals(childrenA[i], childrenB[i]))
                    return path + "/" + FirstDifference(childrenA[i], childrenB[i]);
            }
        }
        string Shape(XElement e) => e.HasElements
            ? string.Join(", ", e.Elements().Select(c => c.Name.LocalName))
            : e.ToString(SaveOptions.DisableFormatting);
        return $"{path}: [{Shape(a)}] vs [{Shape(b)}]";
    }

    private static XDocument Patched(PatchContext context, params string[] patchFiles)
    {
        var defs = MergedDefs();
        foreach (var file in patchFiles)
        {
            var operations = LoadPatch(file).Root!.Elements("Operation").ToList();
            for (var i = 0; i < operations.Count; i++)
                Assert.True(Apply(operations[i], defs, context), $"{file}: Operation {i + 1} failed (the game logs an error there)");
        }
        return defs;
    }

    // ===== Patch interpreter =====
    // The operation classes the two files use, with the game's semantics: an xpath that matches
    // nothing fails the operation unless it carries <success>Always</success>, and a sequence
    // stops at its first failure.

    private sealed class PatchContext
    {
        public PatchContext(IEnumerable<string> activePackageIds, bool spacerTier)
        {
            ActivePackageIds = new HashSet<string>(activePackageIds, StringComparer.OrdinalIgnoreCase);
            SpacerTier = spacerTier;
        }

        public HashSet<string> ActivePackageIds { get; }
        public bool SpacerTier { get; }
    }

    private static bool Apply(XElement op, XDocument defs, PatchContext context)
    {
        var cls = (string?)op.Attribute("Class") ?? throw new InvalidOperationException($"Operation without a Class: {op}");
        var result = cls switch
        {
            "PatchOperationReplace" => ForEachMatch(op, defs, node => node.ReplaceWith(ValueChildren(op))),
            "PatchOperationAdd" => ForEachMatch(op, defs, node => node.Add(ValueChildren(op))),
            "PatchOperationRemove" => ForEachMatch(op, defs, node => node.Remove()),
            "PatchOperationSequence" => op.Element("operations")!.Elements("li").All(li => Apply(li, defs, context)),
            "ToxinWarcasket.PatchOperationSetting" => Branch(op, SettingOn(op, context), defs, context),
            "ToxinWarcasket.PatchOperationModActive" => Branch(op,
                op.Element("packageIds")!.Elements("li").Any(li => context.ActivePackageIds.Contains(li.Value.Trim())), defs, context),
            _ => throw new NotSupportedException($"The test interpreter does not handle {cls}"),
        };

        var success = op.Element("success")?.Value.Trim();
        return success switch
        {
            null or "Normal" => result,
            "Always" => true,
            _ => throw new NotSupportedException($"The test interpreter does not handle <success>{success}</success>"),
        };
    }

    private static bool ForEachMatch(XElement op, XDocument defs, Action<XElement> action)
    {
        var nodes = defs.XPathSelectElements(op.Element("xpath")!.Value).ToList();
        foreach (var node in nodes)
            action(node);
        return nodes.Count > 0;
    }

    private static IEnumerable<XElement> ValueChildren(XElement op) =>
        op.Element("value")!.Elements().Select(e => new XElement(e)).ToList();

    private static bool SettingOn(XElement op, PatchContext context)
    {
        var setting = op.Element("setting")!.Value.Trim();
        return setting == "spacerTier"
            ? context.SpacerTier
            : throw new NotSupportedException($"The test interpreter does not know the setting {setting}");
    }

    private static bool Branch(XElement op, bool matched, XDocument defs, PatchContext context)
    {
        var branch = op.Element(matched ? "match" : "nomatch");
        return branch == null || Apply(branch, defs, context);
    }
}
