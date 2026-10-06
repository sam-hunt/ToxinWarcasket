# TODOs

## Content

- **Art polish.** The set's textures are wired in, but each item icon is a straight copy of its
  `_south` facing; give them dedicated icon art if that reads poorly in the foundry.
  `About/Preview.png` and `About/ModIcon.png` are also missing.
- **The tox gas ability.** Deferred by decision; the set ships as a pure protection suit until
  it lands. `Docs/Research/TOX_GAS_MECHANICS.md` maps the engine: the gas grid refuses ToxGas
  and the exposure hediff does not exist without Biotech, so the ability def, its DefInjected
  and its gizmo art all belong behind the Biotech load-folder gate drafted in `LoadFolders.xml`
  (`1.6/Mods/Biotech/` and `Mods/Biotech/`), never the main tree. The three candidate routes
  (the tox pack's `CompProperties_ReleaseGas` + `CompApparelReloadable`, a VEF apparel ability
  calling `GasUtility.AddGas`, an explosion verb with `postExplosionGasType`) are compared
  there. The full set's ToxicResistance 1.0 already zeroes gas buildup and the helmet's
  `immuneToToxGasExposure` removes the exposure debuff, so the wearer can stand in their own
  gas. Revisit the armor's costList when the emitter lands (a reagent tank prices onto that
  piece).
- **Tuning pass.** Plating, weight, speed and price are the Cataphract's verbatim; the toxin
  identity is ToxicResistance 1.0 across armor + helmet plus the helmet's
  ToxicEnvironmentResistance 0.8 and Chemfuel 20. Decide, once the ability exists, whether the
  set stays on `VFEP_AdvancedWarcaskets` (industrial, 4000) or moves up to
  `VFEP_SpecialisedWarcaskets` beside the Hazard set, and whether Biotech's tox-related
  research should gate the ability's root.
- **English text is not final.** Descriptions and the Workshop page are first drafts; the
  translation passes wait for them (see CLAUDE.md's Localization Toolchain section).

## Open questions

- Acquisition: foundry only, by construction. Warcasket parts are destroyed on drop and
  untradeable, so raid presence (all three pieces carry the Cataphract's `WarcasketHeavy` and
  `WarcasketCata` tags, so VFEP's Junker and Mercenary heavy pawnkinds can roll our pieces
  beside the Cataphract's) is threat and flavor, never loot. Decide whether that mix is wanted
  or the set should get its own tag until the ability makes a raider in it interesting.
- Does the Hazard set (VFEP's chemical/flamer 7th-gen set) need any parity or contrast note in
  the descriptions or the Workshop FAQ?

## Infrastructure follow-ups

- **Cut a release candidate to exercise CI before the real release.** The release
  workflow fetches VEF and VFEP from the Workshop with SteamCMD (anonymous login) and injects
  them via `VEF_PATH` / `VFEP_PATH`, but it has never run for this repo. `/release major rc` tags
  `v1.0.0-rc.1`: a GitHub prerelease that needs no CHANGELOG section. Check the Workshop
  fetch step, the translation gate and the zip's contents.
- **Translation passes** for the CONTRIBUTING.md roster, one language at a time via
  `/translate <Language>`, only once the English is final and shortly before release.
- **First Workshop publish.** Upload writes `About/PublishedFileId.txt`; commit it, add the
  Workshop link to the README's Installation section, fill the id into the README's
  commented-out Steam badges and uncomment them, and paste
  `.steamworkshop/Description/English.txt` into the page.
