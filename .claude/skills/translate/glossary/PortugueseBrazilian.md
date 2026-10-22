# Brazilian Portuguese glossary: Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); first machine-assisted pass 2026-10-13, pending
native review. Only
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
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | unidade | `WordInfo/plural.txt` line `unidade;unidades`. Feminine, so vanilla's masculine-only frames (`um {CHARGENOUN}`, `seu último {CHARGENOUN}`) read "um unidade" there, as Shipcracker's `unidade de combustível` already does; pt-BR Keyed has no `CHARGENOUN_gender` switch, so no `WordInfo/Gender` file can help. | Shipcracker's "fuel unit" is `unidade de combustível`, mapped to `unidades de combustível` by its line. |
| jet (the shoulders' `chargeNoun`; one per activation) | jato | `WordInfo/plural.txt` line `jato;jatos`; masculine, fits every vanilla frame. Settings section "Gas jet" = `Jato de gás`, "Jets without a tank" = `Jatos sem tanque`; "spray gas" (the nozzle's `jetLabel`) = `pulverizar gás`. | Needs its own line. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## Grounded in the first pass

VFEP set names, the research label and the 7th-generation paragraph are
reconciled against the community pack's `VFE_Pirates_ThingDef.xml` and
`VFE_Pirates_ResearchProjectDef.xml`.

| English | Brazilian Portuguese | Grounding |
|---|---|---|
| toxin (set name) / Workshop title | toxina (labels: `carcaça de guerra toxina`); `A série Toxina` in lore; title `Carcaça de Guerra Toxina` | coined, pack label pattern |
| cataphract (set name) | catafrática (`carcaça de guerra catafrática`; lore "no design catafrática") | Pack `VFEP_Warcasket_Cataphract.label`, its lore's "O design catafrática" |
| hazard (set name) | de risco (`carcaça de guerra de risco`; lore `A série de Risco`) | Pack `VFEP_Warcasket_Hazard.label` |
| aerial (set name, Workshop) | aérea | Pack `VFEP_Warcasket_Aerial.label` |
| 7th-generation paragraph | "As carcaças de guerra de 7ª geração surgiram após ..." | Pack `VFEP_Warcasket_Hazard.description` paragraph 2, its "— embora intercambiáveis —" reflowed into commas and its "plastoaço" replaced by Core's `plastiaço` |
| specialised warcaskets (research) | carcaças de guerra especializadas | Pack `VFEP_SpecialisedWarcaskets.label` |
| tox gas (research) | gases tóxicos | Biotech `ResearchProjectDef` `ToxGas.label` |
| pilot | piloto | Core backstories (`CargoPilot58.titleShort` "Piloto") |
| tox gas | gás tóxico | Biotech `ToxGas.label`, `ToxGasExposure.label` |
| tox gas exposure (stages) | gás tóxico (suave / moderado / severo) | Biotech `ToxGasExposure` |
| gas mask | máscara de gás | Biotech `Apparel_GasMask.label` |
| tox pack (and its reload noun) | mochila tóxica | Biotech `Apparel_PackTox.label` / `chargeNoun` |
| pollution / polluted terrain | poluição / terreno poluído | Biotech `ToxicEnvironmentResistance.description` |
| toxic environment resistance | resistência a ambiente tóxico | Biotech `ToxicEnvironmentResistance.label` |
| toxic fallout | chuva tóxica | Core `ToxicFallout.label` |
| rot stink | fedor de podridão | Core Keyed `RotStink`, hediff `RotStink` |
| toxic buildup | acúmulo tóxico | Core `ToxicBuildup.label` |
| smoke | fumaça | Core `Smoke.label` |
| sharp / blunt (armor) | cortante / contundente | Core `ArmorRating_Sharp/Blunt.label` |
| deadlife dust | poeira de vida morta | coined (Anomaly not grounded) |
| Miscellaneous (settings section) | Outros | Core Keyed `MiscRecordsCategory` |
| (requires restart) | (requer reinício) | coined from Core Keyed "reiniciado" |
| tox gas tank | tanque de gás tóxico | coined |
| vent (noun) / vent tank (ability) / vent rate | válvulas / abrir válvulas / Vazão das válvulas | coined; Core's `Vent` "saída de ventilação" is the temperature building, so avoided |
| absorb (noun) / absorb gas (ability) | a absorção / absorver gás | coined |
| gas jet / reserve jets | jato de gás / jatos próprios (das ombreiras), `Jatos sem tanque` | coined |
| spray (verb) | pulverizar | coined |
| superior protection and worse weight (7th-gen opener) | proteção superior e peso elevado | Pack `VFEP_WarcasketShoulders_Cataphract.description` |
| waster (xenotype) | toxicador / toxicadores (also as a trailing adjective: `piloto toxicador`) | Biotech `Waster.label` "toxicadores", `PirateWaster.pawnSingular` "toxicador", `PirateBossTox.label` "chefe toxicador" |
| gene | gene | Biotech Keyed `Gene` |
| tox absorption (VREW gene, `VRE_ToxAbsorption`) | absorção de gás tóxico (`o gene de absorção de gás tóxico`) | coined: VREW ships English only; the absorb row's `absorção` plus `gás tóxico`, on Biotech Keyed `AbsorbingPollutionNext` "Absorção de poluição" |
| pollution mutations (VREW's pollution-related genes) | mutações ligadas à poluição | coined; VREW's own wording is "pollution-related genes"; Core attests `mutada`/`mutante` |
| keeping (mutations) active / counts as | mantendo ativas / conta como | Coined: an on-state while gas is present, not growth. |
| tox cloud | nuvem tóxica | Biotech description text ("nuvem tóxica mortal") |
| Tank feeds waster tox absorption (settings toggle) | Tanque alimenta a absorção de gás tóxico dos toxicadores; Workshop: `o tanque alimenta o gene de absorção de gás tóxico de um piloto toxicador` | coined, label shape of `TXWC_HazardVacuum` |
| Default: on. / on by default | Padrão: ligado. / `(configuração, ligada por padrão)` (agreeing with `configuração`) | Core Keyed `On` "Ligado", mirroring `Padrão: desligado.` |
| (requires Vanilla Races Expanded - Waster) | (requer Vanilla Races Expanded - Waster) | Title kept in English (VREW's Workshop title), `(requer Odyssey)` pattern |
| aiming time (the gas jet's warmup, settings slider) | Tempo de pontaria (segundos); desc verb `mirar` | Core `AimingDelayFactor.label` "tempo de pontaria"; `mirar` matches the Workshop page's "A mira" |
| spacer tier (settings) | nível espacial (`Carcaça de guerra toxina é de nível espacial`; `no nível espacial de 10ª geração`; suffix ` (parte do nível espacial)`) | Core Keyed `TechLevel_Spacer` "Espacial"; the suffix avoids a participle that would have to agree with an unnamed setting |
| spacer armor / spacer components / shield bubble | blindagem de nível espacial / componentes avançados / uma bolha de escudo | Core `ComponentSpacer.label` "componente avançado"; the other two coined |
| standard protection (10th-gen opener) | Carcaça de guerra de tecnologia espacial especializada com proteção padrão, ... (especializadas/equipadas for ombreiras, especializado for capacete) | Pack `VFEP_Warcasket_Siegebreaker` / `VFEP_WarcasketShoulders_Siegebreaker` / `VFEP_WarcasketHelmet_Siegebreaker` descriptions, verbatim |
| 10th-generation paragraph (`TXWC_SpacerLoreGeneration`) | "As carcaças de guerra de 10ª geração foram, na verdade, adaptadas ..." | Pack `VFEP_Warcasket_Siegebreaker.description` paragraph 2, verbatim (no dashes to reflow; its English "dreadnoughts" kept as the pack has it) |
| spacer warcaskets (research) | carcaças de guerra espaciais | Pack `VFEP_SpacerWarcaskets.label` |
| research cited in prose (Keyed and Workshop) | ASCII double-quoted lowercase label: `a pesquisa "carcaças de guerra espaciais"` | No vanilla precedent; pt-BR research labels are lowercase, so the quotes do the job the English's Title Case does. Kept on the Workshop page even where the English dropped its quotes. Matches Shipcracker's pt-BR page |
| veteran raiders | invasores veteranos | Core `Raid` "invasão"; pack `VFEP_Veteran.label` "veterano" |
| Hazard / Toxin warcasket resists vacuum (toggles) | Carcaça de guerra de risco / toxina resiste ao vácuo; desc `Dá à carcaça de guerra ... a resistência ao vácuo do nível espacial` | sibling toggles share one shape; Core `VacuumResistance.label` "Resistência ao Vácuo" |

## Pending native review

Nothing inherited is open for the VFEP rows (they are the community pack's).

- Toxina as the set's own name (`A Toxina`, `A série Toxina`).
- `unidade` as a feminine charge noun in vanilla's masculine frames.
- válvulas / abrir válvulas, absorção, jato de gás, pulverizar gás, tanque de gás tóxico.
- poeira de vida morta (Anomaly's deadlife dust, not grounded).
- absorção de gás tóxico (VREW's tox absorption gene), mutações ligadas à
  poluição, and the toggle phrasing `Tanque alimenta a absorção ... dos
  toxicadores` / `o tanque alimenta o gene ...` (VRE Waster pass, 2026-10-17).
