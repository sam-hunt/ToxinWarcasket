---
name: translate
description: Generate, update, or audit mod localization (DefInjected only) for a target language, grounded in Vanilla Factions Expanded - Pirates warcasket terminology plus vanilla Core/Biotech toxin and gas terminology for Toxin Warcasket's single warcasket set. Use ONLY when explicitly asked to add a language, update translations, or check translation freshness; never start a pass on your own initiative.
disable-model-invocation: true
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Toxin Warcasket. English is the
source of truth; every other language derives from it.

**Translation passes are deferred until shortly before the first release
and are expensive.** They run only when the user asks for one in that
session, one language at a time, and only after the English def text is
final (a pass over draft English is thrown away with the next wording
change). Generating or refreshing the expected-key sidecar (step 0 below)
is cheap and always fine; it is the translation itself that waits.

**The family-wide process lives in the `l10n/` submodule, load these first,
and only these** (progressive disclosure; if `l10n/` is empty, run
`git submodule update --init`):

- `l10n/process.md`, non-negotiables, file/format conventions, terminology
  grounding method, and the generation / update / audit workflows. This is
  the workflow authority; follow it step by step.
- `l10n/languages/<Language>.md`, the target language's engine mechanics,
  style rules, and vanilla-grounded common vocabulary. Read ONLY the target
  language's file.
- `glossary/<Language>.md` (beside this file), this mod's own coined-term
  table for the target language. Read it in the same pass. None exist yet;
  the first pass for a language creates it, starting from the sibling
  Shipcracker Warcasket repo's glossary for that language, whose VFEP
  warcasket vocabulary decisions (warcasket, foundry, entomb, set names)
  this mod inherits verbatim so the two sets read as one family in-game.
- `l10n/lessons.md`, cross-language lessons; read when generating a new
  language, skim otherwise. Its free-prose register lesson applies with
  full force here: the three descriptions are lore paragraphs with no
  vanilla sentence to mirror, so a separate register read of every
  description and Workshop sentence is part of every pass.
- `l10n/workshop.md`, the `.steamworkshop/` conventions, whenever the pass
  touches the Workshop description (every initial generation does).

**Where learnings land:** mod-independent findings (engine mechanics, a
language's grammar rule, corpus style facts) go in the `l10n/` submodule,
edit the canonical checkout at `~/dev/rimworld-l10n`, commit and tag there.
Mod-specific findings (coined terms, phrasing decisions) go in
`glossary/<Language>.md`. A VFEP-vocabulary decision that differs from the
Shipcracker glossary's must be reconciled there too, not forked here.

**Before any pass, bump the pin:** run `l10n/tools/bump-consumer.sh` (fetches
upstream's release tags, checks out the latest, commits the pointer as `chore:
Bump l10n submodule vOLD -> vNEW`; no-op when already current). This is one of
the three moments a pin moves (release, pass start, new upstream major), never
per upstream commit. If it reports a MAJOR bump, read the upstream release
notes for the shim or flow edit this repo owes before continuing.

## This mod's translation surface

- **No Keyed strings and no English Languages tree at all.** English is
  served entirely by the def XML's own fields; there is nothing under
  `1.6/Languages/English/`. The translation surface is DefInjected only,
  plus the Workshop page under `.steamworkshop/`.
- **Enumerate the key set from `Scripts/expected-injections.json`, never
  from a Languages folder or by scanning `1.6/Defs/`.** The sidecar is a
  dump of what the live game walks; regenerate it (game closed) with
  `python3 Scripts/refresh-translation-expectations.py` whenever the
  checker reports it stale. Take the English source text for each
  `<!-- EN: -->` comment from the sidecar's `english` field.
- **Def type folder:** `DefInjected/ThingDef/` for the three
  `VFEPirates.WarcasketDef`s (`TXWC_Warcasket_Toxin`,
  `TXWC_WarcasketShoulders_Toxin`, `TXWC_WarcasketHelmet_Toxin`): `label`,
  `description`, `shortDescription` (a VFEP field shown in the foundry's
  part picker; translate it like any other), and the armor's and shoulders'
  reload comps' `chargeNoun` (the tank's "reagent", the nozzle's "reserve
  jet"; vanilla reads it in "out of {CHARGENOUN}" style strings). The
  abilities and their hediff take the sidecar's other def types as their
  folder names: the two VEF ability defs (`TXWC_VentTank`, `TXWC_AbsorbGas`)
  and `HediffDef` (`TXWC_AbsorbingGas`). The game rolls a def type
  without its own database into its base, and the checker maps the element
  tag via `DEF_TYPE_ALIASES` in `Scripts/check-translations.py`; a
  `WarcasketDef` folder would never load. Never translate or place a
  non-`required` sidecar entry in any language file.
- **The three descriptions share their second and third paragraphs
  verbatim** (the 7th-generation lore and the Toxin series paragraph); only
  the first paragraph differs per part. Keep the shared paragraphs
  byte-identical across the three defs in every language, and keep each
  def's `shortDescription` identical to its description's first paragraph,
  as the English does. Paragraph breaks are the literal two-character
  `\n` sequences the def XML uses.
- **Compat roots carry no strings.** None is live: Biotech is a hard
  dependency, so the tox gas content and its DefInjected sit in the main
  tree. A future optional-mod root follows CLAUDE.md's Localization and
  Optional-Content Gating section.
- **Workshop page:** `.steamworkshop/Description/<Language>.txt`, per
  `l10n/workshop.md` and the folder's own `README.md`. The title's anchor
  term is "warcasket"; every localized title must contain the rendering of
  "warcasket" recorded in that language's glossary. There is no Keyed
  title key to keep in step with (`WORKSHOP_TITLE_KEY` is `None`).

## This mod's grounding domain

Domain mod: **Vanilla Factions Expanded - Pirates (VFEP)**, plus vanilla
Core and Biotech. **VFEP ships English only**, so its warcasket vocabulary
is not available from VFEP itself for any other language. The Shipcracker
glossary for the language records which community "Vanilla Expanded"
translation, if any, it was grounded against and what it coined; reuse
those decisions. Where a term is new here, coin it and record it in
`glossary/<Language>.md` rather than inventing silently at translation
time. Each glossary names the source it was grounded against.

Terms that MUST be grounded before use:

- from VFEP's own vocabulary: "warcasket" itself, the VFEP set names our
  text or Workshop page references (cataphract), "warcasket foundry",
  "shoulders" / "pauldrons", "helmet", "shell", the generation phrasing
  ("7th generation warcaskets ...", which VFEP's own descriptions repeat
  verbatim across the Aerial, Barrage, Hazard and Shock sets; ours reflows
  VFEP's dashed aside into commas, so follow the meaning, not the dashes);
- from vanilla Core (ground against the Core tar per `l10n/process.md`):
  apparel terms (armor, helmet, shoulder pads), steel, plasteel, uranium,
  chemfuel, respirator, breathing, toxic environment resistance, toxic
  fallout, rot stink, toxic buildup;
- from vanilla Biotech (ground against the Biotech tar): tox gas, gas mask,
  tox pack (its reload noun), pollution, and the ToxGasExposure hediff's
  label and stage names;
- this mod's own ability vocabulary, coined once and recorded in the
  glossary: reagent (tank), vent, absorb, gas jet, reserve jet.

Grep the tars for just this handful of terms; never extract or read a
whole tar. The vanilla-grounded answers for common words live in
`l10n/languages/<Language>.md`; this mod's own coined terms and VFEP-term
decisions live in `glossary/<Language>.md`.

## Workflows

Follow `l10n/process.md`'s Initial generation / Update pass / Audit-only
workflows verbatim. This mod's specifics on top:

- The checker: `python3 Scripts/check-translations.py` (`--strict` for new
  languages). Sidecar regen: `python3
  Scripts/refresh-translation-expectations.py` (game must be closed; drives
  the deployed L10nProbe, which must have this mod ticked in its settings).
- There is no compat-root routing to do (see above); everything lands in
  the main tree.
- The public roster is CONTRIBUTING.md's localization table, update it in
  the same commit as any language addition or native review.
- Machine-assisted passes are run as one Opus subagent per language with a
  bounded brief (the key list, the grounded VFEP terms, the language file
  and glossary, the register gate); the lead reviews every diff and owns
  the commit. Never fan out to every language at once without being asked;
  the family's token budget is the constraint.
