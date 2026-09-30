# Glossary, Toxin Warcasket-specific terminology

Per-language files here (`German.md`, `Russian.md`, `ChineseSimplified.md`,
and so on, named after the RimWorld language folder) hold everything about
a language's translation that is specific to this mod: the VFEP warcasket
vocabulary decisions for that language (how "warcasket", "entomb", the set
name cataphract and "warcasket foundry" are rendered, and which community
VFEP translation, if any, they were grounded against), the toxin-domain
coinages, and any terms pending native review.

None exist yet: translation passes are deferred until shortly before the
first release. When a language's first pass runs, start its file from the
sibling Shipcracker Warcasket repo's `.claude/skills/translate/glossary/
<Language>.md` and inherit its VFEP vocabulary verbatim, so both sets read
as one family in-game; add only this mod's own terms.

Family-shared, mod-independent findings, LanguageWorker mechanics, style
and corpus rules, and vanilla-grounded common vocabulary (armor,
helmet, steel, quality tiers, tech levels, and so on), live
upstream in the `l10n/` submodule at `l10n/languages/<Language>.md`
(canonical checkout: `~/dev/rimworld-l10n`), since they apply to any mod in
the family, not just this one.

When a future translation pass coins a new Toxin Warcasket-specific term,
record it here. If a pass instead surfaces a correction to shared mechanics
or vocabulary, send that fix upstream to the l10n repo rather than
duplicating it here.
