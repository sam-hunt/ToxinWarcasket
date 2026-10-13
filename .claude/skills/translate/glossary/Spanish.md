# Spanish (Castellano) glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); first machine-assisted pass 2026-10-13, no native
review yet. Only
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
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | unidad (pl. unidades, by rule; no `plural.txt` line) | Feminine, and vanilla's Core `Gender/Female.txt` lacks it: `Stat_Thing_ReloadDestroyOnEmpty_Desc` switches on `{CHARGENOUN_gender}`, so it ships `1.6/Languages/Spanish/WordInfo/Gender/Female.txt` (`unidad`). Bare noun, not Shipcracker's `unidad de combustible`, since the unit here is the chemfuel itself. | Shipcracker's "fuel unit" is `unidad de combustible`, mapped to `unidades de combustible` by its line. A bare `unidad` needs no line. |
| jet (the shoulders' `chargeNoun`; one per activation) | chorro (pl. chorros, by rule) | None needed: single word, masculine, which is the gender default on a table miss. Matches the settings' `Chorro de gas` section and vanilla's `chorro de veneno`/`chorro de bilis`. | Single word needs no line. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## Grounded in the first pass

The community pack (Cito2310) was fetched from GitHub
(`vanilla-factions-expanded/Tribal_Translate/DefInjected/{ThingDef,ResearchProjectDef}/Pirates_*.xml`,
despite the folder name these are the VFE Pirates files).

| English | Spanish | Grounding |
|---|---|---|
| cataphract (set name) | catafracto; `el diseño Catafracto` in prose | Community VFEP `VFEP_Warcasket_Cataphract.label` = `ataúd de guerra catafracto`, prose `El Catafracto`. (Royalty's `servoarmadura catafracta` agrees on the word.) |
| hazard (set name) | hazard (kept in English); `la serie Hazard`, `el Hazard` | Community VFEP `VFEP_Warcasket_Hazard.label` = `ataúd de guerra hazard`, prose `La serie Hazard`. |
| specialised warcaskets (research) | ataúdes de guerra especializados | Community VFEP `VFEP_SpecialisedWarcaskets.label`. |
| spacer warcaskets (in the hazard vacuum tooltip) | ataúdes de guerra de la era espacial | Shipcracker's FAQ wording; the pack's `ataúdes de guerra espacial` is ungrammatical. |
| 7th-generation paragraph | `Los ataúdes de guerra de 7.ª generación surgieron tras...` | The pack's shared Aerial/Barrage/Hazard/Shock paragraph, corrected: its `7ta` is a Latin American ordinal (Castilian `7.ª`), it has agreement slips (`que entran`, `a hacer más frecuente`) and mistranslates "a separate class of" as `separada de`. The structure and `si bien/aunque ... intercambiables, funcionan mejor en conjunto` and `unidades blindadas móviles` are kept; VFEP's dashed aside is commas. |
| superior protection and worse weight | una protección superior y un peor peso | Community VFEP `VFEP_Warcasket_Cataphract.description` verbatim. |
| toxin (set name) | toxina; label pattern `ataúd de guerra toxina` / `hombreras toxina de ataúd de guerra` / `casco toxina de ataúd de guerra`; prose `la serie Toxina`, `el Toxina` | Coined. Invariable noun in apposition like the pack's `hazard`/`catafracto`; vanilla attests `toxinas ambientales` (Core). |
| Workshop title / `TXWC_SettingsCategory` | Ataúd de guerra toxina | Inherited title pattern. |
| pilot | piloto | Core (`biofirma del piloto`), Odyssey (`consola de piloto`), community VFEP (`El piloto puede...`). |
| tox gas | toxigás (pl. toxigases) | Biotech `ToxGas.label`, `Apparel_GasMask.description`. |
| gas mask | máscara antigás | Biotech `Apparel_GasMask.label`. |
| tox pack (and its reload noun) | pack de tóxicos (pl. packs de tóxicos) | Biotech `Apparel_PackTox.label` and `chargeNoun`. |
| pollution / polluted terrain | contaminación / terreno contaminado | Biotech `PollutedTerrain.label`, `Apparel_GasMask.description`. |
| `ToxGasExposure` hediff and stages | toxigás: leve / moderada / grave; "exposición al toxigás" in prose | Biotech `ToxGasExposure.label`, stages, description. |
| toxic environment resistance | resistencia a entornos tóxicos | Biotech StatDef `ToxicEnvironmentResistance.label`. |
| toxic fallout | lluvia tóxica | Core `ToxicFallout.label`. |
| rot stink | olor a putrefacción | Core `RotStink` stage label. |
| toxic buildup | acumulación tóxica | Core `ToxicBuildup.label`. |
| environmental toxins | toxinas ambientales | Core/Biotech prose. |
| deadlife dust | polvo de la muerte | Core Keyed `DeadlifeDust` (the Core `book.txt` lookup has `polvo de necrovita`; Keyed wins for a player-facing noun). |
| smoke / sharp / blunt | humo / cortante / contundente | Core `Smoke.label`, `ArmorRating_Sharp`/`_Blunt`. |
| vacuum resistance | resistencia al vacío | Odyssey `VacuumResistance.label`. |
| requires restart | requiere un reinicio; suffix ` (requiere reinicio)` | Core `ChangedTextureCompressionRestart`. |
| Miscellaneous (settings section) | Varios | Core `MiscRecordsCategory`. |
| downed (pilot goes down) | queda incapacitado | Core `DownedCannotCrawl` (`incapacitado`). |
| tox gas tank | depósito de toxigás | Coined from the inherited `depósito`. |
| vent (ability verb / the process / the hardware) | purgar depósito / la purga / válvulas de purga | Coined. `purgar` is the plain engineering verb for bleeding gas from a tank; the ability label is a lowercase infinitive like Biotech's `desplegar pack de tóxicos`. |
| absorb (ability / noun) | absorber gas / la absorción | Coined, plain words. |
| gas jet (the action and charge) / jet hardware | chorro de gas / toberas (de gas) | Coined. `chorro` as in vanilla's `chorro de veneno`; the physical nozzles on the pauldrons are `toberas`, kept apart from the charge noun. |
| reserve jet (the shoulders' own jets) | chorros propios de las hombreras; settings label `Chorros sin depósito` | Coined, following the English settings label. |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | rociar gas | Coined; `rocía` is the jet description's verb. |
| targeter (Workshop only) | restructured: `Al apuntar se ve...` | No vanilla noun for the targeter in this slot. |

## Pending native review

Nothing inherited is open for the VFEP rows (they are the community pack's).

- `toxina` as the set name (noun in apposition; `el Toxina` with a masculine
  article for the model in Workshop prose).
- `purgar depósito` / `la purga` / `válvulas de purga` for vent.
- `absorber gas` / `la absorción`; `chorro`, `chorro de gas`, `toberas`;
  `depósito de toxigás`; `rociar gas`.
- `unidad` as the tank's charge noun and its `Gender/Female.txt` line.
- The corrected 7th-generation paragraph (diverges from the community pack's
  wording where that is ungrammatical or Latin American).
- `un peor peso` is the pack's Cataphract formula, kept for family
  consistency; a native reviewer may prefer `un peso mayor`.
