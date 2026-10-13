# Russian glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review). Initial generation 2026-10-13 (machine-assisted,
pending native review). Only
mod-specific coinages and grounding decisions; family-wide mechanics and
style live in `l10n/languages/Russian.md`.

VFEP grounding source: the community Russian translation of VFE Pirates,
OneCodeUnit/VanillaRussianExpanded on GitHub (Vanilla Russian Expanded). Its
set pattern is an adjective on `броня мертвеца`; a named set's shoulders and
helmet drop `мертвеца` (`наплечники прорывной брони`, `шлем прорывной брони`).
This mod follows that pattern, and takes the pack's Cataphract and Hazard
adjectives and its 7th-generation paragraph.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Russian | Grounding |
|---|---|---|
| warcasket | броня мертвеца | Community VFEP pack (`VFEP_Warcasket_Warcasket.label`). No separate word for "shell"; it is dropped in the part descriptions, as the pack does. |
| `<set>` warcasket / shoulders / helmet (label pattern) | `<set adj.>` броня мертвеца / наплечники `<set adj. gen.>` брони / шлем `<set adj. gen.>` брони | Pack pattern for named sets (Siegebreaker: `прорывная броня мертвеца`, `наплечники прорывной брони`, `шлем прорывной брони`). |
| warcasket foundry | цех мертвеца | Community VFEP pack (`VFEP_WarcasketFoundry.label`). Foundries in general are `цеха`. |
| entomb / weld into (the set) | заковать в броню мертвеца | Community VFEP pack (`VFEPirates.EntombInWarcasket`). |
| Siegebreaker / Brute / Guardian (set adjectives) | прорывная / беспощадная / охранная | Community pack set adjectives. |
| spacer-tech (warcasket) | космического уровня | Core `TechLevel_Spacer` = `космический`; the pack's spacer sets say `сделанная по технологиям космического уровня`. |
| chassis (the warcasket frame) | каркас | The pack's word for the warcasket frame. |
| Imperial | имперский | Royalty (`имперский дезертир`). |
| plasteel / uranium / chemfuel | пласталь / уран / химтопливо | Core; short form as in Odyssey and the pack (`химтоплива`). |
| jobReportString (`Using ability: {0}`) | применяет способность «{0}» | Shipcracker's rendering (no trailing period, guillemets around the label). |
| cooldownGerund (`on cooldown`) | перезаряжается | Shipcracker's rendering of the same vanilla comp field. |
| Settings: "Default: {0}." / " (default)" / Off | По умолчанию: {0}. / (по умолчанию) / Выкл | Core Keyed `Default`, `default`, `Off`. |
| Settings: cooldown / range | перезарядка / дальность | Core Keyed `CooldownTime`, `Range`. Slider units read `(в клетках)`, `(в часах)`, `(%)`. |
| Settings: quantities with the fuel name | `{0} ед. ({1})`, `Ёмкость бака ({0})` | Core uses `{0} ед.` for mass quantities (`LowMedicineDesc`); the fuel name stays nominative in parentheses, so no numeral or case agreement is needed. |
| (requires Odyssey) | (требуется Odyssey) | Core `ExpansionDef` keeps `Odyssey` in Latin; `требуется` as in Core `SkillNeededForConstructing`. |
| Workshop title | `<Set adj.>` броня мертвеца, sentence case | Contains `броня мертвеца`. Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). The Russian worker looks the `plural` table up before its
ending rules, and the genitive slots (`Осталось ...`, `Стоимость 1 ...`) go
through `case` lookups, so **every coined noun ships a `plural.txt` line and
`case.txt` rows for both numbers** (six cases each, `nom;gen;dat;acc;ins;prep`)
under `1.6/Languages/Russian/WordInfo/` (lowercase file names). Note the
Russian worker lowercases a missed key, so a miss also loses capitalization.

| English | Russian | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | единица химтоплива | Coined after Shipcracker's `единица топлива`, naming the fuel since the settings count the tank in chemfuel. Relies on `WordInfo/plural.txt` (`единица химтоплива;единицы химтоплива`) and `WordInfo/case.txt` (both numbers, six cases). No gender file: Russian Core Keyed has no `CHARGENOUN_gender`. |
| jet (the shoulders' `chargeNoun`; one per activation) | струя | Coined. Relies on `WordInfo/plural.txt` (`струя;струи`) and `WordInfo/case.txt` (both numbers). Same noun as the settings' `Газовая струя` section and `TXWC_JetReserve` (`Струи без бака`). Vanilla's tox pack noun `пуск` was not reused (one launch of a pack, not a jet). |

## Grounded in the first pass

| English | Russian | Grounding |
|---|---|---|
| toxin (set adjective) / Workshop title | токсичная броня мертвеца / наплечники токсичной брони / шлем токсичной брони; title `Токсичная броня мертвеца` | Coined on the pack pattern; adjective as in Biotech `Apparel_PackTox.label` `токсичный ранец`. Title equals `TXWC_SettingsCategory`. |
| Toxin series (lore) | токсичная серия | Pack's `защитная серия` pattern. |
| cataphract (set adjective) | катафрактарская | Pack `VFEP_Warcasket_Cataphract.label` `катафрактарская броня мертвеца` (agrees with Royalty `Apparel_ArmorCataphract.label`). |
| hazard (set adjective) | защитная | Pack `VFEP_Warcasket_Hazard.label` `защитная броня мертвеца`. |
| Aerial (Workshop) | воздушная | Shipcracker's Workshop/settings usage. |
| 7th-generation paragraph | pack's paragraph verbatim (`Седьмое поколение стало результатом...`) | Pack's Aerial/Barrage/Hazard/Shock descriptions; of its two variants (comma after `Также` or not) the comma-less one. Its own comma in `вместо полной взаимозаменяемости, разные` is kept verbatim. |
| "specialised warcaskets" research | «Специализированная броня мертвеца» | Pack `VFEP_SpecialisedWarcaskets.label`. |
| spacer warcaskets (settings) | космическая броня мертвеца | Shipcracker's Workshop (`«Космическая броня мертвеца»`). |
| Specialized warcasket shell / pauldrons / helmet | Специализированная броня мертвеца / Специализированные наплечники брони мертвеца / Специализированный шлем брони мертвеца | Pack Hazard shortDescriptions. |
| superior protection and worse weight | с превосходной защитой ценой большего веса | Coined. |
| pilot | пилот | Pack (`Пилот может с комфортом...`), Shipcracker. |
| tox gas | токсичный газ | Biotech `ToxGas.label` (GasType and research). |
| gas mask | противогаз | Biotech `Apparel_GasMask.label`. |
| tox pack | токсичный ранец | Biotech `Apparel_PackTox.label` (its chargeNoun is `пуск`). |
| pollution / polluted terrain | загрязнение / загрязнённая почва | Biotech Keyed `PlanetPollution`, `MessageWarningPollutedCell`. |
| ToxGasExposure (label; stages) | отравление токсичным газом; лёгкое / тяжёлое / смертельное | Biotech `ToxGasExposure`. |
| toxic environment resistance | устойчивость к токсичной среде | Core/Biotech `ToxicEnvironmentResistance.label`. |
| toxic fallout | токсичные осадки | Core `ToxicFallout.label`. |
| rot stink | трупный газ | Core `RotStink` (gas). |
| toxic buildup | отравление токсинами | Core `ToxicBuildup.label`. |
| smoke | дым | Core `Smoke.label`. |
| deadlife dust | порошок мёртвой жизни | Anomaly `DeadlifeDust.label` (Workshop only). |
| vacuum resistance | устойчивость к вакууму | Odyssey `VacuumResistance.label`. |
| respirator | респиратор | Shipcracker; Core usage. |
| tox gas tank / tank | бак с токсичным газом / бак | Coined; `бак` as Shipcracker. |
| vent (ability label) / the vent (settings noun) | выпуск газа / выпуск | Coined; noun label as vanilla `FireBurst` `выброс пламени`. |
| vents (the hardware) | клапаны | Coined. |
| absorb gas (ability) / the absorb | поглощение газа / поглощение | Coined; noun label per vanilla ability style. |
| gas jet | газовая струя | Coined. |
| jets on the pauldrons (hardware) | сопла | Coined. |
| reserve jets (tankless) | собственные струи (наплечников) | Coined; settings label `Струи без бака`. |
| spray gas (jet verb label; not on the surface yet) | распыление газа | Coined; vanilla `FirefoampopMech` `распыление пены`. Recorded only, no key. |
| cells (settings units) | клетки, `(в клетках)` | Inherited slider-unit style; counts written as `Клеток ...: {1}` to avoid numeral agreement. |

## Pending native review

Nothing inherited is open for the VFEP rows (they are the community pack's).
This pass's coinages, all pending native review:

- `токсичная броня мертвеца` (set name and Workshop title; `токсическая` was the alternative).
- `единица химтоплива` (tank unit) and `струя` (shoulders' jet) as charge nouns.
- `бак с токсичным газом`, `выпуск газа`, `клапаны`, `поглощение газа`, `газовая струя`, `сопла`, `Струи без бака`, `распыление газа`.
- `с превосходной защитой ценой большего веса` (superior protection and worse weight).
- `TXWC_JetTankLow` restructured as `Недостаточно {lookup: {CHARGENOUN_plural}; Case; 1} в баке (нужно {COUNT}).` to avoid numeral agreement with an injected noun.
