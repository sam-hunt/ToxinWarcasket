# TODOs

## Content

- **English text is not final.** needs a review of whether it matches Vanilla/VFEP tone and reads naturally. In particular, the warcasket apparel description feels a bit "rambling" in its description of vents and nozzles etc

## Art

- Replace the `About/Preview.png` placeholder when it lands
- **Workshop images.** shipcracker as the template for the lineup

## Infrastructure follow-ups

- **Translation passes** for the CONTRIBUTING.md roster, one language at a time via
  `/translate <Language>`, only once the English is final and shortly before release.
- **Bump the l10n pin to v1.5.0 (`l10n/tools/bump-consumer.sh`) and apply its plural-lookup
  lesson to the two charge nouns** before the translation passes. The armor's tank
  (`chargeNoun` "unit") and the shoulders' (`chargeNoun` "jet") are shown only through vanilla's
  `{CHARGENOUN_plural}` / `{lookup: ...}` slots in five Keyed strings (charges remaining, max
  charges, reload cost, cost per charge, none left). Per `l10n/lessons.md` (v1.5.0): a
  multi-word rendering pluralizes wrongly unless the mod ships a `WordInfo/plural.txt` line
  (German, Spanish, Portuguese, Russian; Russian also `case.txt` rows), French never reads the
  table so its noun must be a single word, and CJK needs nothing. Shipcracker Warcasket's
  "fuel unit" relabel (its translate skill and per-language glossary rows) is the worked
  example; its checker ignores the WordInfo files, so the glossary row must name them.
- **First Workshop publish.** Upload writes `About/PublishedFileId.txt`; commit it, add the
  Workshop link to the README's Installation section, fill the id into the README's
  commented-out Steam badges and uncomment them, and paste
  `.steamworkshop/Description/English.txt` into the page.
