# Spanish (Castellano) glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); no translation pass has run here yet. Only
mod-specific coinages and grounding decisions; family-wide mechanics and
style live in `l10n/languages/Spanish.md`. Folder `Spanish`, grounded against
the Castilian tars only (never `SpanishLatin`, whose VFEP lineage uses
"sarcotraje").

VFEP grounding: the community Castilian-leaning VFE Pirates translation by
Cito2310 (GitHub `Cito2310/translate-project-rimworld`). Its label pattern is
`ataúd de guerra <set>` for the torso, `hombreras <set> de ataúd de guerra`
and `casco <set> de ataúd de guerra` for the other parts; this mod follows
it, and takes its Cataphract and Hazard renderings and its 7th-generation
paragraph where the pack has them.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Spanish | Grounding |
|---|---|---|
| warcasket | ataúd de guerra (pl. ataúdes de guerra) | Community VFEP (`VFEP_Warcasket_Warcasket.label`). Also the Workshop title anchor. |
| `<set>` warcasket / shoulders / helmet (label pattern) | ataúd de guerra `<set>` / hombreras `<set>` de ataúd de guerra / casco `<set>` de ataúd de guerra | Community VFEP label pattern (`ataúd de guerra bruto`, `hombreras bruto de ataúd de guerra`, `casco bruto de ataúd de guerra`). |
| warcasket shell / pauldrons (shoulders) / helmet | armazón / hombreras / casco | Community VFEP (`Un armazón de ataúd de guerra...`, `VFEP.Shoulderpads`, `VFEP.Helmet`). |
| warcasket foundry | forja de ataúd de guerra | Community VFEP `VFEP_WarcasketFoundry.label`. |
| entomb / welded into | sepultar / sepultado en | Community VFEP `VFEPirates.EntombingWarning` ("Sepultar colonos en un ataúd de guerra"). |
| specialized spacer-tech | especializado de tecnología espacial | Community VFEP 10th-gen descriptions. |
| VFEP set names in prose | Siegebreaker, Bruto, Guardián; "series" for set | Community VFEP (`La serie Bruto`); it leaves Siegebreaker in English and is inconsistent on `guardian`/`guardián`, so the accented form is used. |
| spacer (tech level) | era espacial | Core `TechLevel_Spacer`. |
| Imperial | imperial (lowercase adjective) | Royalty (`batallón imperial`). |
| sealed | hermético | Odyssey (`paredes y puertas herméticas`). |
| respirator | respirador (integrado / propio) | No vanilla Castilian item; plain word. |
| wearer | portador | Core (`Apparel_ArmorMarineHelmetPrestige.description`). |
| plasteel / uranium / chemfuel | plastiacero / uranio / biocombustible | Core labels. |
| jobReportString (`Using ability: {0}`) | usando habilidad: {0}. | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | en enfriamiento | Shipcracker's rendering of the same vanilla comp field. |
| settings: tank / cells / cooldown | depósito / casillas / enfriamiento | `depósito` is plain Castilian for a fuel tank (no vanilla item). Core Keyed `{0} casillas`, `CooldownTime` = `Enfriamiento`. |
| settings: Off / default / Default: on | Desactivado / (por defecto) / Por defecto: activado | Core `Off` = `Apagado` is a power state; `Desactivado` mirrors `Enabled` = `Activado`. Core `default` = `por defecto`. |
| settings: Odyssey | Odyssey | Core `ExpansionDef` keeps DLC names in English. |
| Workshop title | Ataúd de guerra `<set>`, sentence case | Contains the VFEP term. Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). The Spanish worker looks the `plural` table up before its
rules, and the rule pluralizes only the last word, so **a multi-word noun
ships a line** in `1.6/Languages/Spanish/WordInfo/plural.txt`
(`singular;plural`, lowercase file name); a single word needs none.

| English | Spanish | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | *pass* | Shipcracker's "fuel unit" is `unidad de combustible`, mapped to `unidades de combustible` by its line. A bare `unidad` needs no line. |
| jet (the shoulders' `chargeNoun`; one per activation) | *pass* | Single word needs no line. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## To ground in the first pass

- cataphract (set name, lore paragraph 3): the community pack's `VFEP_Warcasket_Cataphract.label`, else Royalty `Apparel_ArmorCataphract.label`.
- hazard (set name, settings' vacuum toggle and Workshop FAQ): the community pack's `VFEP_Warcasket_Hazard.label`.
- the 7th-generation paragraph: the community pack's Aerial / Barrage / Hazard / Shock descriptions share it verbatim; take that rendering.
- "specialised warcaskets" research (Workshop FAQ): the community pack's `VFEP_SpecialisedWarcaskets.label`.
- toxin (this set's name) and the Workshop title.
- pilot (the wearer): Core and Odyssey Castilian for "pilot"; the pack's own word if it has one.
- Biotech: tox gas, gas mask, tox pack, pollution, the `ToxGasExposure` hediff label and stages; Core: toxic environment resistance, toxic fallout, rot stink, toxic buildup.
- This mod's coinages: tox gas tank, vent (noun and verb), absorb, gas jet, reserve jet, spray gas.

## Pending native review

Nothing inherited is open for the VFEP rows (they are the community pack's).
Add this mod's own coinages here as the pass makes them.
