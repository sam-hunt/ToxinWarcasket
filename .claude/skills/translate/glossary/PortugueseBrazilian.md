# Brazilian Portuguese glossary: Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); no translation pass has run here yet. Only
mod-specific coinages and grounding decisions; family-wide mechanics and
style live in `l10n/languages/PortugueseBrazilian.md`.

Community VFEP translation grounded against: RafaelNaymaier's
"Tradução Vanilla Expanded PT-BR" (GitHub `RafaelNaymaier/Traducao-Vanilla-Expanded-PT-BR`,
Workshop 3643091941), confirmed from its ThingDef and Keyed files. It also
carries the Cataphract and Hazard sets and the 7th-generation paragraph.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Brazilian Portuguese | Grounding |
|---|---|---|
| warcasket | carcaça de guerra | Community VFEP pack throughout (`VFEP_Warcasket_*.label`, `VFEP.RequiresWarcasket`). "shell" in the English first line folds into the same noun, as the pack does. |
| `<set>` warcasket / shoulders / helmet (label pattern) | carcaça de guerra `<set>` / ombreiras de carcaça de guerra `<set>` / capacete de carcaça de guerra `<set>` | Pack label pattern (`carcaça de guerra quebra-cerco`, `ombreiras de carcaça de guerra quebra-cerco`, `capacete de carcaça de guerra quebra-cerco`). |
| pauldrons / shoulders | ombreiras | Pack uses `ombreiras` for both the label and the "pauldrons" in descriptions. |
| warcasket foundry | fundição de carcaças de guerra | Pack's `VFEP_WarcasketFoundry` label. |
| entomb / welded into (Workshop) | integrar (em carcaça de guerra) | Pack's `VFEPirates.EntombInWarcasket` = "Integrar em carcaça de guerra". "welded" uses the same verb, since "soldado" reads as "soldier". |
| siegebreaker / brute / guardian (set names) | Quebra-cerco / Bruto / Guardião | Pack's lore paragraphs ("A série Quebra-cerco"). |
| Specialized spacer-tech warcasket ... | Carcaça de guerra de tecnologia espacial especializada ... | Verbatim opening of the pack's 10th-gen descriptions; the 7th-gen opener ("Specialized warcasket shell with superior protection and worse weight") is the pack's Cataphract line instead. |
| respirator | respirador de bordo | Pack's `telemetria de bordo` for "onboard". |
| wearer | portador | Core `Apparel_ShieldBelt.description`. |
| plasteel / uranium / chemfuel | plastiaço / urânio / combustível químico | Core material labels. The pack writes "plastoaço" in lore; vanilla's label wins. |
| Imperial | imperial (lowercase adjective) | Royalty `Empire.pawnSingular`. |
| jobReportString (`Using ability: {0}`) | usando habilidade: {0}. | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | em recarga | Shipcracker's rendering of the same vanilla comp field. |
| cooldown (settings) | tempo de recarga | Royalty Keyed `Cooldown`. Core's `CooldownTime` "Esfriamento" is the weapon sense, not an ability's. |
| cells (unit) / radius | células / raio | Core Keyed `MapSizeDesc` "Células", `AbilityEffectRadius` "Raio do efeito". |
| Off / default / Odyssey | Desligado / padrão / Odyssey | Core Keyed `Off`, `Default`; Core `ExpansionDef` `Odyssey.label` keeps the English name. |
| Workshop title | Carcaça de Guerra `<Set>`, Title Case | Contains the pack's "carcaça de guerra". Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). Portuguese overrides no `Pluralize`, so the base one runs:
lookup in the `plural` table, else the word unchanged. **Every coined noun
ships a line** in `1.6/Languages/PortugueseBrazilian/WordInfo/plural.txt`
(`singular;plural`, lowercase file name), single-word ones included, or it
never pluralizes.

| English | Brazilian Portuguese | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | *pass* | Shipcracker's "fuel unit" is `unidade de combustível`, mapped to `unidades de combustível` by its line. |
| jet (the shoulders' `chargeNoun`; one per activation) | *pass* | Needs its own line. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## To ground in the first pass

- cataphract (set name, lore paragraph 3): the pack's `VFEP_Warcasket_Cataphract.label`, else Royalty `Apparel_ArmorCataphract.label`.
- hazard (set name, settings' vacuum toggle and Workshop FAQ): the pack's `VFEP_Warcasket_Hazard.label`.
- the 7th-generation paragraph: the pack's Aerial / Barrage / Hazard / Shock descriptions share it verbatim; take that rendering.
- "specialised warcaskets" research (Workshop FAQ): the pack's `VFEP_SpecialisedWarcaskets.label`.
- toxin (this set's name) and the Workshop title.
- pilot (the wearer): Core and Odyssey pt-BR for "pilot"; the pack's own word if it has one.
- Biotech: tox gas, gas mask, tox pack, pollution, the `ToxGasExposure` hediff label and stages; Core: toxic environment resistance, toxic fallout, rot stink, toxic buildup.
- This mod's coinages: tox gas tank, vent (noun and verb), absorb, gas jet, reserve jet, spray gas.

## Pending native review

Nothing inherited is open for the VFEP rows (they are the community pack's).
Add this mod's own coinages here as the pass makes them.
