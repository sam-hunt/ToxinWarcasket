# TODOs

## Content

- **Workshop art.** `About/Preview.png` and `About/ModIcon.png` are missing.
- **In-game test pass for the tox gas mechanics.** All three phases of
  `Docs/Research/ABILITIES_SPEC.md` have landed but none has been played yet; its "Edge cases
  and test list" is the checklist (downed/death bursts, anaesthetic, caravans, save/load
  mid-vent/absorb/jet, the jet's cone near allies, raider vent and jet AI).
- **Gizmo art.** Requested from the artist: vent, absorb and gas jet icons (paths in the spec).
- Check the gas rate of torso vent and gas jet
- Check how the armor rating sits against other same-tier warcaskets for cost and functionality
- Check the torso environmental resistance alone with the explosive on-down tox gas vent
- **English text is not final.** Descriptions and the Workshop page are first drafts; the
  translation passes wait for them (see CLAUDE.md's Localization Toolchain section).

## Infrastructure follow-ups

- **Translation passes** for the CONTRIBUTING.md roster, one language at a time via
  `/translate <Language>`, only once the English is final and shortly before release.
- **First Workshop publish.** Upload writes `About/PublishedFileId.txt`; commit it, add the
  Workshop link to the README's Installation section, fill the id into the README's
  commented-out Steam badges and uncomment them, and paste
  `.steamworkshop/Description/English.txt` into the page.
