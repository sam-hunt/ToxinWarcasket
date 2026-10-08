# CLAUDE.md

Guidance for Claude Code (claude.ai/code) when working in this repository.

## Project Overview

**Toxin Warcasket** is a RimWorld 1.6 mod adding a single new warcasket apparel set (armor,
shoulder pads, helmet) for Vanilla Factions Expanded - Pirates (VFE Pirates), templated on
VFEP's Cataphract set and themed around vanilla's tox gas. VFE Pirates and Biotech are hard
dependencies (see Tox Gas and Biotech). Requires Harmony (bootstrapped in `ModInit.cs`; patch
classes, when any land, live in `Source/1.6/Patches/`).

**Key technologies:** C# (.NET Framework 4.7.2), Harmony, RimWorld modding API, XML defs.

**Def prefix:** `TXWC_`. A 7th-generation (Specialised) set on the Cataphract's plating; each
def's header carries its tuning rationale. `TODOs.md` holds the scoping notes for what has not
landed, chiefly the tox gas abilities, specified in `Docs/Research/ABILITIES_SPEC.md`.

**Sibling mod:** `../ShipcrackerWarcasket/` is the same author's spacer-tier set (templated on
the Siegebreaker) and the source of this repo's infrastructure. When both repos need the same
infra change, make it there first if it is the more exercised one, then mirror it here; the
two must not drift in the shared parts (build, CI, l10n shims, skills).

### Where documentation lives

**This file holds only cross-cutting rules and rationale.** Per-item values, tuning numbers and
decompile-verified call paths live in the header comment of the file they describe. When adding
or changing something, put the *why* there and only add a line here if it constrains work in
other files. Do not restate def values or call paths here; they drift.

**Comments describe the present, git describes the past.** A header or code comment explains the
current state where the code does not make it obvious: engine facts it relies on, what a number is
balanced against, cross-file coupling. It carries no dates, no previous values, no record of what
was tried and dropped, and no "seen in game" notes; that history belongs in the commit message
and diff. Def comments ship in the bundle, so keep them lean.

`Docs/Research/` is informational only and not mod content: `VFEP_WARCASKET_STATS.md` is the
VFEP roster's declared stats, `TOX_GAS_MECHANICS.md` the decompile-verified map of the tox gas
engine (the three Biotech gates, the buildup math, the vanilla emitters, the candidate ability
routes), `ABILITIES_SPEC.md` the agreed design and phased plan for the abilities and the 7th-gen
rebalance. Read the spec before implementing any of it; once a phase lands, the def headers and
code comments are the record and the spec is not kept in sync with tuning.

## Build Commands

```bash
# Build (outputs to 1.6/Assemblies/ AND atomically redeploys to the RimWorld Mods folder)
dotnet build ToxinWarcasket.sln -c Release

# Stage the mod into an arbitrary folder (used by CI; same manifest as the local deploy)
dotnet build Source/1.6/ToxinWarcasket.csproj -c Release \
  -t:StageMod -p:StageDir=/path/to/output/ToxinWarcasket

# Override RimWorld install path
RIMWORLD_PATH="/path/to/RimWorld" dotnet build ToxinWarcasket.sln -c Release
# Or: dotnet build -p:RimWorldPath="/path/to/RimWorld"
```

The build auto-detects the RimWorld install (Windows/Linux/Mac, including WSL targeting a Windows
install), falling back to the `Krafs.Rimworld.Ref` NuGet package in CI. Debug builds go to the
default `bin/` and never deploy; only Release builds touch `1.6/Assemblies/` and the Mods folder.

**WSL setup:** `RIMWORLD_PATH` in `~/.bashrc` pointing at the Windows install, e.g.
`/mnt/c/Program Files (x86)/Steam/steamapps/common/RimWorld`.

### Dependency-mod assemblies

`VFEPirates.dll` and `VEF.dll` are referenced compile-only (`Private="false"`), resolved by the
csproj in this order: explicit override (`-p:VfePiratesDir=` / `-p:VefDir=` or the `VFEP_PATH` /
`VEF_PATH` env vars), the Steam Workshop copy beside the install (ids 2723801948 and 2023507013),
the sibling source checkouts under `../VanillaExpanded/`, then the RimWorld `Mods/` folder. The
Workshop copy is preferred because it is what players run against. Both references are skipped
silently when the DLL is absent, so a build still succeeds without them until code uses their
types. CI fetches both DLLs from the Workshop with SteamCMD in the release workflow and injects
them via the `VEF_PATH` / `VFEP_PATH` environment variables.

Reference source checkouts: `../VanillaExpanded/VanillaFactionsExpanded-Pirates/` (its
`CLAUDE.md` maps the warcasket subsystem: `WarcasketDef`, `Apparel_Warcasket`,
`Building_WarcasketFoundry`, the entombing jobs) and `../VanillaExpanded/VanillaExpandedFramework/`.
Both are upstream repos, not ours: read from them, never commit into them from here.

### Deployment

The repo lives outside the Mods folder; every local Release build redeploys automatically and
atomically. The deploy folder name follows the project name (`Mods/ToxinWarcasket`).

- **One manifest, one place:** the `_ModFiles` ItemGroup in the `StageMod` target of
  `Source/1.6/ToxinWarcasket.csproj`; see that target's comments for how it globs and what it
  excludes. It is generic over folders, so a new `1.7/` or `Sounds/` needs no build change; only
  a brand-new *file type* does. Local deploy and CI release both call it, so they can't drift.
- **Stop hook (`.claude/hooks/sync-mod.sh`):** rebuilds+redeploys after a turn only when
  mod-relevant files changed, logs to `$TMPDIR/ToxinWarcasket-build.log`. On failure it
  exits 2 with the errors on stderr, which Claude Code feeds back to the agent and the turn
  continues; a second failure in the same turn (`stop_hook_active`) only warns, so it cannot
  loop. Tracked and wired
  by the tracked `.claude/settings.json`; the script is byte-identical across the mod family
  and derives the solution, project folder and mod name itself, so change it in the template
  and copy it verbatim, never per repo. It bails when no RimWorld install is found, so CI and
  contributors without the game are unaffected.

**`.claude/` is only partly gitignored.** `.gitignore` carries `.claude/*` followed by
`!.claude/skills/`, `!.claude/hooks/` and `!.claude/settings.json`, so the skills, the Stop hook
and its wiring are tracked and shared while `settings.local.json` (personal permissions) stays
local per machine. Editing a skill is a committed, team-visible change and must keep in step with
whatever it automates (e.g. `/release` encodes the CHANGELOG layout).

## Project Structure

```
About/           - Mod metadata (About.xml; Preview.png and PublishedFileId.txt once published)
Textures/        - Art (version-independent, loaded via the "/" root; no Common/ root), under
                   VFEP's Things/Pawn/Warcasketlike/WarcasketToxin/ layout
1.6/             - RimWorld 1.6 specific content
  Assemblies/    - Compiled DLLs (build output, gitignored)
  Defs/          - XML definitions (ThingDefs, etc.)
  Patches/       - XML patches to modify base game/other mods
  Mods/<Name>/   - Optional-mod/DLC compat roots (version-specific), gated in LoadFolders.xml
Mods/<Name>/     - Optional-mod/DLC compat roots (version-independent art)
Source/1.6/      - C# source code targeting net472
Scripts/         - l10n config shims + the expected-injections.json sidecar (not shipped)
Docs/Research/   - Reference notes (VFEP stats, tox gas engine map); not shipped
l10n/            - rimworld-l10n toolkit, git submodule pinned to a release tag (not shipped)
LoadFolders.xml  - Tells RimWorld which folders to load per game version
CHANGELOG.md     - Keep a Changelog format; load-bearing for releases (see below)
TODOs.md         - Scoping notes for the feature work that has not landed yet
```

## RimWorld Modding Context

- Target framework: .NET Framework 4.7.2
- References RimWorld assemblies via cross-platform paths in .csproj
- Uses `Verse` namespace for core modding APIs
- XML Defs define game objects; Patches modify existing Defs via XPath
- Harmony is referenced (`Lib.Harmony`, compile-only) and bootstrapped in `ModInit.cs`; the
  runtime DLL comes from the `brrainz.harmony` mod dependency declared in About.xml.

### Conventions

- All defs use the `TXWC_` prefix. **One def per file** in every `Defs/` `.xml`, named after the
  def with the prefix stripped. Defs load recursively and the deploy manifest globs, so new
  files/subfolders need no build change.
- Warcasket parts are `VFEPirates.WarcasketDef`s parented on VFEP's abstract bases
  (`VFEP_WarcasketArmorBase`, `VFEP_WarcasketShoulderPadBase`, `VFEP_WarcasketHelmetBase`), so
  the foundry, entombing flow and removal surgery pick them up without C#. Check VFEP's XML
  before reimplementing anything a base already provides. The template is the Cataphract
  (`VFEP_Warcasket_Cataphract` and its two pieces); a field this repo does not discuss in a def
  header is the Cataphract's, so diff against it before changing one.
- Every VFEP warcasket base already carries a `VEF.Apparels.ApparelExtension`, and VEF merges
  duplicate extensions at resolve time, keeping the highest `priority` and dropping fields its
  `Merge` does not copy. Our extension entries carry `priority` 1 so ours survive; any new
  `ApparelExtension` field, from any load root, must go on that entry (rationale in the armor
  def's header).
- **C#:** root namespace `ToxinWarcasket`; patch classes live in `Source/1.6/Patches/` under
  the `.Patches` namespace suffix to avoid RimWorld type-name conflicts. Log with the
  `[Toxin Warcasket]` prefix.
- **Warnings are build errors.** The csproj sets `TreatWarningsAsErrors`, so every compiler and
  analyzer warning fails the build, locally, in the Stop hook and in CI. `.editorconfig`
  severities at `warning` block the build; `suggestion` is IDE-only.
- **Patch timing is the load-bearing hazard of this mod.** `PatchAll()` runs from a
  `[StaticConstructorOnStartup]` (`ModInit.cs`), *not* a `Mod` subclass constructor, on purpose:
  Mod constructors run before defs load, and applying a detour JIT-compiles the target and runs
  its declaring type's static ctor. Our targets will often be VFEP's own methods, and a VFEP type
  whose cctor resolves defs would be permanently broken by an early patch (the BetterTradersGuild
  v1.1.0 CWTL incident). Static ctors on startup run after defs load, so foreign targets are safe
  there. If a `Mod` subclass is added for settings, leave `PatchAll` where it is.
- **No em dashes in player-facing text** (def labels/descriptions, `Keyed/`, `About.xml`,
  `.steamworkshop/`); reflow the sentence instead. This file, code comments and def comments are
  unaffected.

## Tox Gas and Biotech

Tox gas is Biotech-locked at the engine level, not just by content: `GasGrid.AddGas` refuses
`ToxGas` without Biotech, the `ToxGasExposure` hediff and `ToxGas` damage def only exist with
it, `GasUtility` skips the tox branch without it, and cell pollution (the death rupture) calls
`ModLister.CheckBiotech` (`Docs/Research/TOX_GAS_MECHANICS.md` has the call paths). Every
mechanic of the set's design emits gas or pollution, so **Biotech is a hard dependency, by
decision**: a Biotech-less set would be plating at a 7th-gen price.

Gas content goes in the main tree like everything else; there is no Biotech load-folder gate
and references to Biotech defs need no `MayRequire`.

## Localization and Optional-Content Gating

- `MayRequire`/`MayRequireAnyOf` work on def root nodes and list items, but the DefInjected loader
  ignores XML attributes entirely. A DefInjected entry for a gated def placed in the main tree
  loads unconditionally and logs a "found no def named ..." startup error whenever the gating
  mod/DLC is absent.
- The fix is a folder gate: ship the gated content from a compat load root, loaded via an
  `IfModActive` entry in `LoadFolders.xml`.
  Two flavors, mirroring the ungated roots: `1.6/Mods/<Mod Name>/` for version-specific content
  (Defs, and the DefInjected targeting them) and root-level `Mods/<Mod Name>/` for
  version-independent content (art). The def's `MayRequire` becomes redundant and should be
  dropped when it moves. Gate on the package id (`ludeon.rimworld.odyssey`), never
  `PatchOperationFindMod` (matches by display name). The one deliberate exception is a patch
  that mirrors a VFEP patch: `1.6/Patches/SOS2Patch.xml` copies VFEP's own display-name gate so
  the Toxin and VFEP's sets flip to EVA-rated under exactly the same condition.
- Compat roots must sit BESIDE the well-known folders, never inside them: anything under
  `1.6/Defs/**` or `1.6/Languages/**` loads unconditionally at any depth.
- The game gates a DefInjected entry by the load root that CONTAINS it, never by the def it
  targets. The reverse mistake also bites: a main-tree def's translation placed in a compat root
  silently vanishes when the gate is closed.
- A compat root's language files must never reuse a main-tree file's language-relative path
  (`DefInjected/<Type>/<File>.xml`, `Keyed/<File>.xml`): the game dedups language files per mod by
  that path and silently skips one whole file, in an enumeration order that is not LoadFolders
  order. Suffix compat-root filenames with the gate's name (`Apparel_Odyssey.xml`).

## Localization Toolchain

English (the def XML's `label`, `description` and VFEP's `shortDescription`) is the source of
truth; there is no Keyed surface and no English `Languages/` tree. Other languages derive from it
via the `/translate` skill (`.claude/skills/translate/SKILL.md`: this mod's surface, grounding
domain and per-language glossary; the family-wide process lives in the `l10n/` submodule) and
are validated deterministically by `python3 Scripts/check-translations.py` (also a CI release
gate). The DefInjected expected set is the checked-in sidecar `Scripts/expected-injections.json`:
a dump of every injection point the *live* game sees for this mod, produced by
`Scripts/refresh-translation-expectations.py` driving the L10nProbe dev mod (source at
`l10n/probe/`; build/deploy it only from the canonical `~/dev/rimworld-l10n` checkout; this mod
is ticked in the probe's settings) through the game's own walker. The checker refuses to run
against stale expectations, so new content forces a regen; the release skill regenerates every
release.

**Translation passes are deferred and expensive; never start one unprompted.** They wait for
the English text to be final and run shortly before release, one language at a time, only when
the user asks for one in that session. Regenerating the sidecar and running the checker are
cheap and always fine; the translation itself is what waits. The public language roster lives
in CONTRIBUTING.md (all Planned until then). The VFEP vocabulary decisions per language are
inherited from the Shipcracker glossaries so the two sets read as one family in-game.

- **Shared l10n toolkit (`l10n/` submodule):** the checker/refresh/smoke engines, per-language
  mechanics references, cross-language lessons and Workshop conventions come from the
  `rimworld-l10n` repo, consumed as a git submodule pinned to a semver release tag (`git submodule
  status` names it; if `l10n/` is empty, run `git submodule update --init`). `Scripts/*.py` are
  thin per-repo config shims over its engines; each shim's comments carry this repo's rationale
  (the VFEP dependency chain, why Biotech is the only DLC pinned, why SOS2 is on the smoke list).
  Never edit `l10n/` in place here: mod-independent learnings go upstream in the canonical
  checkout; mod-specific ones go in the skill's glossary. The pin moves only at release, at the
  start of a translation pass, or when a new major lands (`l10n/tools/bump-consumer.sh`), never
  per upstream commit.
- **Def type folder:** the defs are `VFEPirates.WarcasketDef`, a `ThingDef` subclass without its
  own database, so the game dumps and injects them under `ThingDef`; the checker maps the element
  tag via `DEF_TYPE_ALIASES`. A DefInjected folder named `WarcasketDef` would never load.
- **Startup smoke test (pre-release):** `python3 Scripts/integration-smoke-test.py` (game closed)
  boots the deployed mod on a pinned list (VFEP chain + Biotech + Save Our Ship 2 with Vehicle
  Framework so `SOS2Patch.xml` actually runs), then classifies logged errors by origin and fails
  on anything attributed to this mod or a VFEP/VEF/SOS2 seam. Wired into the release skill.
  Sibling-mod "dump WILL fail" warnings at launch are expected: the probe is ticked for every
  family mod but each boot loads only its own list.
- **`.steamworkshop/`** holds the Workshop title and BBCode description per language
  (`Description/<Language>.txt`, the toolkit's convention; its `README.md` has the format and
  title rule). It is player-facing text and quotes def numbers, so a tuning change that moves
  one updates `English.txt` in the same commit; the release skill translates it from there.

**Releases:** run the `/release` skill, or by hand: add the version's `## [X.Y.Z]` section to
`CHANGELOG.md`, bump `About/About.xml` `<modVersion>` and `Source/1.6/Properties/AssemblyInfo.cs`,
then push a `v*.*.*` tag. The GitHub Actions workflow (`.github/workflows/release.yml`) builds,
stages via `StageMod`, lifts the tag's CHANGELOG section into the release body, and **fails the
release if that section is missing**. Release candidates are `X.Y.Z-rc.N` tags (the same glob
matches): CHANGELOG-less and Workshop-less, with the suffix only in `modVersion` and
`AssemblyInformationalVersion` (the numeric assembly attributes stay `X.Y.Z.0`); `release.yml`
gives any suffixed tag a stub body and marks it a prerelease, and `/release` measures every range
from the last *stable* tag.

## Debugging

1. **Dev Mode:** Settings > Dev Mode > Logging.
2. **Log:** `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
   (WSL: `/mnt/c/Users/*/AppData/LocalLow/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`;
   Linux: `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`).
3. **Logging convention:** `Log.Message("[Toxin Warcasket] ...")`; grep the prefix to isolate
   our output. VFEP's own messages carry no prefix; search by class name.
4. **Inspect the API:** `monodis` for signatures, `ilspycmd -t "Namespace.ClassName"` for method
   bodies, against the local install's `Assembly-CSharp.dll` (source of truth over the Krafs ref
   package) or the resolved `VFEPirates.dll` / `VEF.dll`. The `rimworld-logs` skill covers both.
5. **In-game test list:** VFE Pirates (Workshop 2723801948) and Vanilla Expanded Framework must
   both be active and load before this mod; the local deploy shows up as `ToxinWarcasket` in the
   mod list.
