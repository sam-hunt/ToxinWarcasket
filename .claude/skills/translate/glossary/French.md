# French glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary; the first machine-assisted
pass has run. Only mod-specific coinages and grounding decisions; family-wide
mechanics and style (the elision worker, where `PostProcessed` runs, register
tells) live in `l10n/languages/French.md`. Folder `French`, grounded against
the Core, Royalty and Biotech `French (Français).tar`s.

VFEP grounding: VFEP ships English only and no community French translation
survives (the "Vanilla Factions Expanded - Pirates [Fr]" Workshop item,
id 2725064005, has been removed), so a French player sees "cataphract
warcasket", "hazard warcasket" and "warcasket foundry" in English in-game.
"warcasket" is therefore kept untranslated, as vanilla keeps "chemfuel": the
Shipcracker repo's native-speaker PR (#1, Joedox) replaced the earlier
"sarcophage", which matched nothing the player sees. Every other VFEP term
below (foundry, parts, research) is coined there and inherited here.

**Contractions are written by hand.** DefInjected values never pass through
`LanguageWorker_French.PostProcessed`, and hand-contracted text is inert
under it either way; the Workshop page is never post-processed at all.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

Rows marked *native* were settled by the Shipcracker native review and are
not reopened here.

| English | French | Grounding |
|---|---|---|
| warcasket | warcasket (m., pl. warcaskets; invariable as a modifier: `coque warcasket`) | *native*. Untranslated, matching VFEP's English labels in a French game (see above). Also the Workshop title anchor. |
| `<set>` warcasket / shoulders / helmet (label pattern) | warcasket `<set>` / épaulières de warcasket `<set>` / casque de warcasket `<set>` | *native* order. Core/Royalty apparel pattern `casque de cataphracte`, `armure de commando` (part `de` class noun); the set adjective stays invariable after the noun. |
| warcasket shell (torso) | coque warcasket | *native* (replaced the coined `carapace`); the `warcasket` modifier keeps it apart from a ship's `coque`. |
| pauldrons / shoulders | épaulières | **Coined** (no vanilla shoulder apparel; Core only has `épaule` body parts). Ordinary French armor term. Plural label: `[X_definite]` agreement may default masculine singular in vanilla keyed strings. |
| helmet | casque | Core `casque de commando`. |
| spacer / spacer-tech | spatial / de technologie spatiale | *native*. Core `TechLevel_Spacer` = `spatial`; `de technologie spatiale` for the attributive "spacer-tech". |
| warcasket foundry | fonderie de warcaskets | **Coined.** |
| a VFEP research label in prose (`spacer warcaskets`, `specialised warcaskets`) | « spacer warcaskets » (warcaskets spatiaux) | The English label in guillemets, because that is the clickable project every French player sees (VFEP is untranslated), with the French gloss in parentheses. Render `specialised warcaskets` the same way. |
| weld into / welded (entomb) | souder / soudé dans | Community VFEP Workshop page ("se soudent à l'intérieur"). |
| VFEP set names in prose | "ensemble" for set; names capitalized as proper names on the Workshop page only | **Coined** pattern. |
| onboard respirator | respirateur intégré | **Coined**, plain word (no vanilla French respirator item; Biotech has only `masque à gaz`). |
| sealed | étanche | Odyssey `GravshipHull.description` (`paroi étanche`); fine without Odyssey, it is plain French. |
| Imperial | impérial | Royalty `Empire.pawnSingular`. |
| plasteel / uranium / chemfuel | plastacier / uranium / chemfuel | Core labels; `chemfuel` stays untranslated as vanilla does. |
| ability description register | third-person present (`Allume les propulseurs...`) | *native*. The majority vanilla form (Royalty 23 third-person vs 8 infinitive); Biotech's infinitive `Longjump` is the outlier. |
| jobReportString (`Using ability: {0}`) | utilise la capacité : {0}. | Third-person present like vanilla `JobDef.reportString`s, French-spaced colon, trailing period kept. |
| cooldownGerund (`on cooldown`) | rechargement | Shipcracker's rendering of the same vanilla comp field. |
| settings window (Keyed) | Par défaut : {0}. / (par défaut) / Désactivé / (nécessite Odyssey) | Core Keyed `Default` = `Par défaut`, `Off` = `Désactivé(e)`; the DLC keeps its English name (Odyssey `ExpansionDef` label `Odyssey`). |
| cooldown / range / cells / hours | temps de recharge / portée / cases / heures | Core `FlightCooldown` = `temps de recharge du vol`, Keyed `Range` = `Portée`, `HoursLower` = `heures`; `case` is vanilla's map-tile word. |
| Workshop title | sentence case, contains `warcasket` | Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). `LanguageWorker_French.Pluralize` is rule-only and never
reads a `WordInfo/plural.txt`, so **each noun must be a single word**; a
multi-word noun gets its `s` on the last word.

| English | French | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | unité (f., pl. unités); lookup file: none needed (French `Pluralize` is rule-only; `CHARGENOUN_gender` is unused in vanilla fr Keyed, so no `WordInfo/Gender` line either) | Shipcracker's "fuel unit" is `unité` for exactly this reason (feminine against Core's masculine `{CHARGENOUN_plural} restants` is vanilla's own shape: `charge`, `grenade`; `Il ne reste plus de unités` elides to `d'unités`). The fuel is named by the reload-cost line beside it. |
| jet (the shoulders' `chargeNoun`; one per activation) | jet (m., pl. jets); lookup file: none needed (rule plural, masculine default matches) | Single word. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## Grounded in the first pass

| English | French | Grounding |
|---|---|---|
| toxin (set name; title, labels) | toxine (`warcasket toxine`, `casque de warcasket toxine`, `épaulières de warcasket toxine`); Workshop title `Warcasket toxine` | **Coined.** Noun in apposition, the same shape as Shipcracker's `brise-coque`; lowercase in-game, `le Toxine` (m., the warcasket) on the Workshop page. |
| cataphract (lore paragraph, Workshop) | cataphracte (`le modèle cataphracte` in-game; `Cataphracte` on the Workshop page) | Royalty `Apparel_ArmorCataphract.label` = `armure de cataphracte`. |
| hazard (VFEP set, settings and FAQ) | hazard (`warcasket hazard`, `ensemble Hazard` on the Workshop page) | Kept English: VFEP's label is untranslated in-game, and no French word reads as a set name. |
| Aerial (VFEP set, Workshop only) | Aérien | **Coined**, following Shipcracker's Workshop habit of translating VFEP set names (`Brise-siège`, `Gardien`). |
| 7th generation | 7e génération; the dashed aside becomes `qui, bien qu'interchangeables, fonctionnent mieux en ensemble` | Shipcracker's `10e génération`; no community VFEP pack. |
| pilot | pilote | Odyssey (`console de pilotage`, `pilote`); Shipcracker. |
| tox gas | gaz tox | Biotech `ToxGas.label` (GasDef and research). |
| gas mask | masque à gaz | Biotech `Apparel_GasMask.label`. |
| tox pack | pack tox (pl. packs tox) | Biotech `Apparel_PackTox.label` and its `chargeNoun`. |
| pollution / polluted terrain | pollution / terrain pollué | Biotech Keyed `TerrainPollutionTip`, `MessageWarningNotPollutedCell`. |
| tox gas exposure (hediff; stages) | gaz tox (bénigne / modérée / sévère); prose `exposition au gaz tox` | Biotech `ToxGasExposure.*`; the description uses `l'exposition au gaz tox`. |
| toxic environment resistance | résistance à l'environnement toxique | Core `ToxicEnvironmentResistance.label`. |
| toxic fallout | pluie toxique | Core `GameConditionDef`/`IncidentDef` `ToxicFallout.label` (the TaleDef's `retombée toxique` is the outlier). |
| rot stink | pestilence | Core `RotStink` stage label; also the stat description's word. |
| toxic buildup | accumulation toxique | Core `ToxicBuildup.label`. |
| smoke / deadlife dust | fumée / poussière de résurrection | Core `Smoke.label`, Keyed `DeadlifeDust`. |
| venom / psychic | venin / psychique | Core `Cobra` `crochets à venin`, `PsychicSensitivity.label`. |
| vacuum resistance | résistance au vide | Odyssey `VacuumResistance.label`. |
| tox gas tank | réservoir de gaz tox (`réservoir`) | **Coined**; `réservoir` as Shipcracker's tank. |
| vent (the openings) | évent(s) | **Coined**, plain French. |
| vent (the action; ability label, settings) | purge (`purge du réservoir`, `débit de purge`; verb `se purger`) | **Coined.** Noun-phrase ability label like Biotech `pulvérisation d'acide`. |
| absorb (ability; "the absorb") | absorption (`absorption de gaz`) | **Coined**, noun-phrase label pattern. |
| gas jet | jet de gaz | Biotech `FoamSpray.label` = `jet de mousse`. |
| reserve jet / jets without a tank | jets sans réservoir; prose `les propres jets des épaulières` | **Coined.** |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | pulvérisation de gaz | Biotech `AcidSpray.label` = `pulvérisation d'acide`. |
| cone / cloud | cône / nuage | Core `Stat_Thing_Weapon_MissRadius_Desc`; Biotech `nuage toxique`. |
| `Default: off.` | Par défaut : désactivé. | Glossary settings row (`Désactivé`). |
| `Default: on.` | Par défaut : activé. | Core Keyed `On` = `Activé(e)`; the counterpart of the `Default: off.` row. |
| waster (Biotech xenotype) | pestilent (pl. pestilents) | Biotech `XenotypeDef` `Waster.label` = `pestilent`. Not to be confused with `pestilence` (rot stink). |
| gene | gène | Biotech Keyed `Gene` = `gène`; prose `porteurs du gène`, as Biotech's `Les porteurs de ce gène`. |
| tox absorption (VREW gene) | absorption tox; prose `le gène d'absorption tox` | **Coined** (VREW ships English only), on Biotech `ToxResist_Partial.label` = `résistance tox`; shares `absorption` with the absorb ability's row. |
| pollution mutations (VREW) | mutations liées à la pollution | **Coined**; `mutation` is plain French (Anomaly `BecameMutant.label` = `a subi une mutation`). |
| keeping (mutations) active / counts as | maintient actives / compte comme | Coined: an on-state while gas is present, not growth. |
| tank feeds (the VREW toggle) | `Le réservoir alimente l'absorption tox des pestilents`; Workshop `le réservoir alimente le gène d'absorption tox d'un pilote pestilent` | **Coined.** Sentence-case label like `TXWC_HazardVacuum`. |
| tox cloud | nuage de gaz tox | Biotech `ToxGas.label` + glossary `nuage`. |
| Costs no gas. | Ne consomme aucun gaz. | **Coined**, plain French. |
| ` (requires Vanilla Races Expanded - Waster)` | ` (nécessite Vanilla Races Expanded - Waster)` | Pattern of `(nécessite Odyssey)`; the mod title stays in English. |

## Pending native review

Still open from the Shipcracker review, inherited as-is: `fonderie de
warcaskets`, `épaulières`, and the set-name capitalization pattern. Add this
mod's own coinages here as the pass makes them.

This pass's coinages, all machine-assisted: `toxine` as the set name (vs
the adjective `toxique`), `purge` for the vent action beside `évent` for the
openings, `absorption de gaz`, `jets sans réservoir`, `Aérien`, keeping
`hazard` in English, the lowercase set names in in-game lore prose
(`série toxine`, `modèle cataphracte`), and `plus protecteur mais plus
lourd` for "superior protection and worse weight".

The VRE Waster update pass's coinages, also machine-assisted: `absorption
tox` for the gene (on `résistance tox`), `mutations liées à la pollution`,
`alimente` for "feeds", and `Ne consomme aucun
gaz`.
