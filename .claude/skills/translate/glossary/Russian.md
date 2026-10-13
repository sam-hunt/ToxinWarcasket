# Russian glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); no translation pass has run here yet. Only
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
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | *pass* | Shipcracker's "fuel unit" is `единица топлива`, with `единица топлива;единицы топлива` in `plural.txt` and both numbers' six cases in `case.txt`. |
| jet (the shoulders' `chargeNoun`; one per activation) | *pass* | Needs its own `plural.txt` line and `case.txt` rows. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## To ground in the first pass

- cataphract (set adjective, lore paragraph 3): the pack's `VFEP_Warcasket_Cataphract.label`, else Royalty `Apparel_ArmorCataphract.label`.
- hazard (set adjective, settings' vacuum toggle and Workshop FAQ): the pack's `VFEP_Warcasket_Hazard.label`.
- the 7th-generation paragraph: the pack's Aerial / Barrage / Hazard / Shock descriptions share it verbatim; take that rendering.
- "specialised warcaskets" research (Workshop FAQ): the pack's `VFEP_SpecialisedWarcaskets.label`.
- toxin (this set's adjective) and the Workshop title.
- pilot (the wearer): the pack's word (`оператор` in its Brute shoulders), else Core.
- Biotech: tox gas, gas mask, tox pack, pollution, the `ToxGasExposure` hediff label and stages; Core: toxic environment resistance, toxic fallout, rot stink, toxic buildup.
- This mod's coinages: tox gas tank, vent (noun and verb), absorb, gas jet, reserve jet, spray gas.

## Pending native review

Nothing inherited is open for the VFEP rows (they are the community pack's).
Add this mod's own coinages here as the pass makes them.
