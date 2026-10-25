# Combat Extended Compatibility Spec

The design of a Combat Extended (CE) patch for the Toxin set, written before the work so a
clean session starts from facts. Not mod content. Engine facts below were read from CE
16.7.4.0 (Workshop 2890901044, RimWorld 1.6) by decompiling `CombatExtended.dll` with
`ilspycmd`, from its XML under `Defs/`, `Patches/` and `ModPatches/Vanilla Factions Expanded -
Pirates/`, and from the game's `Verse.ModContentPack` / `DirectXmlLoader` (patch ordering).
The sibling `../ShipcrackerWarcasket/` shipped the same patch first; its
`1.6/Mods/CombatExtended/Patches/` root and `Source/1.6/Settings/Settings_Landing.cs` are the
precedent and carry the per-value rationale in their headers.

Once the patch lands, the patch file's header and the def headers are the record (per
CLAUDE.md); this file is the rationale and is not kept in sync with later tuning.

## What CE does to the set today

Nothing good. CE's VFE Pirates patch is per-defName, so from CE the Toxin inherits only what
CE puts on VFEP's abstract bases: `Flammability` 0.25 on `VFEP_WarcasketPartBase` and a
`PartialArmorExt` on the helmet base (Eye x0.85) and the shoulder base (Hand x0.80). CE's
apparel autopatcher is off by default and matches no multi-layer warcasket preset
(`ApparelAutoPatcher`, presets in `Defs/ApparelAutoPatcherPresets/`). Everything else keeps
its vanilla number, read in CE's units:

| Mechanic | CE reading of the unpatched def | Effect |
| --- | --- | --- |
| `ArmorRating_Sharp` | millimetres of RHA, compared directly (`ArmorUtilityCE.TryPenetrateArmor`) | 1.65 mm is below CE's flak jacket (2). Every rifle round penetrates. |
| `ArmorRating_Blunt` | megapascals, same comparison | 0.72 MPa; CE's flak jacket is 1.5. |
| Mass | counted in full against `CarryWeight` (default 40 kg) by `CompInventory` | 108 kg worn: move speed at the 0.5 floor. |
| `Bulk` / `WornBulk` | missing stats default to 1 / 0, silently | No error, no encumbrance from bulk. |
| Smoke | `Smoke.ApplyHediffs` scales `SmokeInhalation` by the pawn's `SmokeSensitivity` (base 1, min 0) over Breathing capacity | The helmet grants nothing: `immuneToToxGasExposure`, FullHead coverage and VEF's extension are never consulted. |

What CE leaves alone, verified:

- **Vanilla tox gas.** `Harmony_GasUtility` and `Harmony_ToxicUtility` are transpilers that
  only insert a "should equip gas mask" notification; `ShouldGetGasExposureHediff`,
  `immuneToToxGasExposure`, `ToxicEnvironmentResistance` and `Hediff_ToxGasExposure` run as
  in vanilla. CE's own tox grenades and 40 mm tox rounds emit vanilla `ToxGas` through
  `postExplosionGasType`, so Absorb Gas takes CE's gas as it takes a tox pack's.
- **The set's explosions.** `GenExplosion.DoExplosion` is not patched (CE transpiles only
  `CompExplosive.Detonate`, the hunter drone and `Bombardment` onto `GenExplosionCE`). The
  vent, the downed burst and the death rupture call it with `DamageDefOf.ToxGas` and no
  damage amount, and CE does not patch that DamageDef, so there is no armor interaction to
  rescale. (The Shipcracker's breach blast carries a penetration and needed one; this set
  does not.)
- **The gas jet verb.** `Verb_SprayGas` is a plain `Verb` subclass; CE replaces apparel verbs
  only where they launch projectiles (VFEP's Barrage shoulders, per-defName).
- **The reloadables.** No generic change to `CompApparelReloadable`; CE adds a "reload worn
  armor" gizmo and a reload job for colonists, which work with chemfuel.
- **VEF.** CE never reads `ApparelExtension`; its one VEF compat class targets methods that no
  longer exist. `pawnCapacityMinLevels` still applies through VEF's own postfix, which is why
  Breathing enters the smoke formula as the divisor.
- **CE's black smoke** (`Gas_BlackSmoke`, thingClass `CombatExtended.Smoke`) is a Thing, not a
  gas-grid entry, so Absorb Gas cannot take it. Nothing to fix; the helmet description could
  say "tox gas" where it is vague, so a CE player does not expect it to clear a burning room.

## The patch ordering trap

The game applies a mod's patches in `foldersToLoadDescendingOrder`, which is the
`LoadFolders.xml` list **reversed** (`ModContentPack.InitLoadFolders` adds folders last to
first; `DirectXmlLoader.XmlAssetsInModFolder` walks that list and keeps the first file seen per
relative path). Within one folder the order is `DirectoryInfo.GetFiles`, which this repo's
`SpacerTier.xml` header already refuses to rely on.

So a CE patch in a gated compat root, the Shipcracker's shape, applies **before** the main
tree's `SpacerTier.xml`, whose `PatchOperationReplace` on the three armor ratings would then
overwrite the CE numbers whenever the spacer tier is on. A main-tree file named to sort after
`SpacerTier.xml` would depend on filesystem order. Neither works as is.

**Design:** one main-tree file, `1.6/Patches/CombatExtended.xml`, gated by this repo's
`ToxinWarcasket.PatchOperationModActive` on `CETeam.CombatExtended` and branching with
`PatchOperationSetting` on `spacerTier`, owning every CE value for both tiers. `SpacerTier.xml`
wraps its plating operations (Sharp, Blunt, Heat) in a `PatchOperationModActive` `nomatch` on
the same id, so under CE neither file touches the other's nodes and their order stops
mattering. Everything else in `SpacerTier.xml` (mass, insulation, shield comp, resistances,
costs, research, tags) stays as it is; the CE file then replaces plating, shield numbers and
costs and adds bulk and carry on top, which are all `Replace` or `Add` of distinct nodes.
Costs are the one overlap: `SpacerTier.xml` replaces the whole `costList`, and the CE file
must replace the individual leaves inside it, which works in either order only because the
CE file branches on the same setting and so always sees the tier's list. Test that explicitly.

A `LoadFolders.xml` entry is not needed; the file lives in the main tree and gates itself.
The smoke test reads `PatchOperationSetting`'s error if the setting name is wrong.

## Values

CE's VFE Pirates patch rescales every VFEP set by chassis, and the Toxin's two tiers each
copy a VFEP chassis exactly, so the analogs are direct. Helmet heat stays vanilla (CE never
touches heat). Uranium, plasteel on the 7th-gen pieces and chemfuel are not in CE's rescale
and stay.

### 7th generation (setting off): the Aerial's numbers

The Toxin's 1.65 / 0.72 / 0.86 are the Aerial's to the decimal, so CE's Aerial values apply
(`Apparel_Warcaskets.xml`, `VFEP_*_Aerial`), with the carry offsets CE gives every industrial
frame.

| Piece | Sharp / Blunt | Bulk / WornBulk | Offsets | Costs (CE's x2 on steel and industrial components) |
| --- | --- | --- | --- | --- |
| Armor | 28 / 70 | 250 / 25 | CarryWeight +250, CarryBulk +200, Suppressability -100 | Steel 280, ComponentIndustrial 16 |
| Shoulders | 28 / 70 | 15 / 5 | | Steel 110, ComponentIndustrial 4 |
| Helmet | 25 / 56 | 15 / 8, NightVisionEfficiency_Apparel 0.5 | SmokeSensitivity -1 | Steel 90, ComponentIndustrial 6 |

The Aerial is the lightest reading of "a step over the Cataphract" (CE Cataphract 32 / 80,
helmet 28 / 63; Hazard 20 / 50); if the set should sit nearer the Cataphract under CE, split
the difference rather than copy it, since the Cataphract carries no comps and this set three.

### Spacer tier (setting on): the Siegebreaker's numbers

The tier copies the Siegebreaker's blunt 1 pattern, so CE's values for the Siegebreaker,
Controller and Sarcophagus apply (`Apparel_Warcaskets_Spacer.xml`).

| Piece | Sharp / Blunt | Bulk / WornBulk | Offsets | Costs (CE's x1.5 on plasteel, +2 spacer components on armor and helmet) |
| --- | --- | --- | --- | --- |
| Armor | 36 / 90 | 275 / 35 | CarryWeight +250, CarryBulk +225, Suppressability -100; shield 2.25 / 0.015 | Plasteel 225, ComponentSpacer 8 |
| Shoulders | 36 / 90 | 15 / 5 | | Plasteel 90, ComponentSpacer 2 |
| Helmet | 32 / 72 | 20 / 10, NightVisionEfficiency_Apparel 0.75 | SmokeSensitivity -1 | Plasteel 90, ComponentSpacer 6 |

The shield row is CE's x1.5 on the tier's 1.5 / 0.01 (CE sets the Guardian, Controller and
Sarcophagus to 2.25 / 0.015); VEF's `CompShieldBubble` absorbs CE projectiles unpatched, so
only the numbers move.

### Both tiers

- Helmet `SmokeSensitivity` -1 (stat clamps at 0: immunity), CE's convention for every
  sealed helmet, VFEP's included. `immuneToToxGasExposure` is already on the def.
- `NightVisionEfficiency_Apparel` on the helmet as above: CE gives it to every warcasket
  helmet, 0.5 on industrial and 0.75 on spacer.
- Nothing on the mod settings: no row stores a CE-unit field. (The Shipcracker's penetration
  slider did; see its `Settings_Landing.cs` if a future Toxin row ever does.)

## Also in the change

- `Scripts/integration-smoke-test.py`: `ceteam.combatextended` on `SMOKE_ACTIVE_MODS` after
  VFEP and SOS2 (CE's own `loadAfter`), before this mod; `"CE": ["CombatExtended"]` in
  `INTEGRATION_PATTERNS`. Boot it twice, with the spacer tier setting off and on, since the
  patch branches on it; the smoke engine reads the saved settings file.
- `.steamworkshop/Description/English.txt` and `README.md`: replace "Not tested with Combat
  Extended" with what the patch does; the release skill translates the description.
- `CHANGELOG.md` `[Unreleased]`: one player-facing line.
- `CLAUDE.md`: the smoke-test sentence names the integrations it boots; add CE. No other
  cross-cutting rule is new; the ordering fact belongs in the patch file's header.
- Def headers: one line each on the three defs pointing at the CE file, as the Shipcracker's
  do.

## Test list

- Spacer tier off, CE on: info card shows 28 / 70 on the armor; helmet 25 / 56; a pawn in
  the full set keeps normal move speed (carry offsets applied); a fire's smoke gives the
  wearer no inhalation hediff; a CE tox grenade's gas is absorbable.
- Spacer tier on, CE on: 36 / 90, helmet 32 / 72, shield bubble at 225 energy, spacer costs
  at the CE numbers in the foundry.
- CE off, both tiers: numbers unchanged from the def headers and `SpacerTier.xml`.
- No "duplicate node" or failed-patch errors at boot in any of the four combinations.
