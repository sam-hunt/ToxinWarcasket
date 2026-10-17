# Simplified Chinese glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); this mod's first machine-assisted pass
filled the charge nouns and the grounded table below. Only
mod-specific coinages and grounding decisions; family-wide mechanics and
style live in `l10n/languages/ChineseSimplified.md`.

VFEP terms are grounded against the community Vanilla Expanded Simplified
Chinese pack (GitHub `q847633684/Vanilla-Expanded-ZH`, the
`Vanilla Factions Expanded - Pirates` folder: its WarcasketDef ThingDef
injections, Keyed UI and ResearchProjectDef files), which also carries the
Cataphract and Hazard sets and the 7th-generation paragraph. Vanilla terms
are grounded against the Core, Royalty and Biotech `ChineseSimplified (简体中文).tar`.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Simplified Chinese | Grounding |
|---|---|---|
| warcasket | 战棺 | Community VFEP pack throughout (`VFEP.RequiresWarcasket` = 需要战棺, every set label). Workshop title anchor. |
| `<set>` warcasket / shoulders / helmet (label pattern) | `<set>`型战棺装甲 / `<set>`型战棺护肩 / `<set>`型战棺头盔 | Community pattern (`VFEP_Warcasket_Siegebreaker.label` = 破袭型战棺装甲 and siblings). The body piece takes 装甲 although the English label has no noun, exactly as the pack does. |
| siegebreaker / guardian / brute / aerial (set names) | 破袭型 / 圣盾型 / 狂怒型 / 突击型 | Community pack labels. The third-party 无畏 prefix (WarCasket Expanded) is not used; the VFEP pack has none. |
| Specialized spacer-tech warcasket shell/pauldrons/helmet | 特化型太空时代级别战棺装甲/护肩/头盔 | Community pack's opening phrase for every 10th-generation part; the 7th-gen opener is the pack's Cataphract line instead. |
| warcasket foundry | 战棺铸造厂 | Community `VFEP_WarcasketFoundry.label`. |
| weld on / entomb (Workshop) | 在战棺铸造厂完成改造; 封在战棺里 | Community renders entomb loosely (进入铸造厂, 战棺改造 in `VFEPirates.EntombingWarning`); the Workshop follows that looseness. |
| spacer warcaskets (research) | 「太空时代战棺」 | Community `VFEP_SpacerWarcaskets.label`; corner brackets per vanilla research-name citation (研究「基础逆重科技」). Cite `specialised warcaskets` the same way. |
| pilot | 驾驶员 | Community Keyed/lore (成为驾驶员的殖民者). |
| respirator | 呼吸器 | Coined (no vanilla respirator item or gene in the zh tars); plain everyday word. |
| breathing | 呼吸能力 | Core `Breathing.label`. |
| Imperial | 帝国 | Royalty adjective use (帝国逃兵). |
| plasteel / uranium | 玻璃钢 / 铀 | Core labels. |
| jobReportString (`Using ability: {0}`) | 使用{0}。 | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | 冷却中 | Shipcracker's rendering of the same vanilla comp field. |
| cooldown | 冷却时间 | Core Keyed `CooldownTime`. |
| Off / default / Default: on | 关 / 默认 / 默认：启用 | Core Keyed `Off`, `Default`, `Enabled`. |
| settings suffixes | （需要奥德赛） / （默认） | Full-width parens with no leading space, as vanilla 锻造台（燃料）. |
| Royalty / Odyssey | 皇权 / 奥德赛 | Core `ExpansionDefs`; ground Biotech's name the same way. |
| Workshop title | `<set>`型战棺 | Set name + 战棺 (the anchor term). Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). Chinese falls to the base `Pluralize` (lookup, else
unchanged), so one noun serves every slot and no lookup file is needed.

| English | Simplified Chinese | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | 燃料 | Lookup file: none needed. Kept Shipcracker's mass noun: 燃料剩余, 没有燃料。, 毒气罐中需要4燃料。 all read naturally, while a counted 单位 would print a bare 单位剩余. Settings prose says 化合燃料 / 每单位化合燃料 where the English names chemfuel. | Shipcracker's "fuel unit" is the plain mass noun 燃料 (燃料剩余, 没有燃料。), not 燃料罐 (a container). Decide whether a counted 单位 reads better for a gauge counted in units of one. |
| jet (the shoulders' `chargeNoun`; one per activation) | 喷射 | Lookup file: none needed. Same noun as the 毒气喷射 settings section and 无毒气罐时的喷射次数; reads 喷射剩余, 没有喷射。 Vanilla's generic 充能 was rejected so the gauge names what it counts. | The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## Grounded in the first pass

| English | Simplified Chinese | Grounding |
|---|---|---|
| cataphract (set name) | 骑士型 | Community pack `VFEP_Warcasket_Cataphract.label` = 骑士型战棺装甲 (Royalty's armor is 骑士装甲). |
| hazard (set name) | 险境型 | Community pack `VFEP_Warcasket_Hazard.label` = 险境型战棺装甲. |
| specialised warcaskets (research) | 「特化型战棺」 | Community pack `VFEP_SpecialisedWarcaskets.label`. |
| 7th-generation paragraph | 由于机体基本性能的强化陷入瓶颈…… | Community pack's Hazard/Barrage/Shock text verbatim, its —— reflowed to a comma. |
| "Specialized ... with superior protection and worse weight" | 特化型战棺`<part>`，在增加机体重量的同时大幅提升了防护性 | Community pack: superior = 大幅 (Shock), worse weight = 增加机体重量 (Barrage). |
| toxin (set name) / Workshop title | 毒素型 / 毒素型战棺 | Coined: toxin = 毒素, in the pack's `<set>`型 slot. |
| tox gas | 毒气 | Biotech `ToxGas.label`, also the research label and the `ToxGasExposure` hediff label. |
| ToxGasExposure stages | 较轻 / 中等 / 严重 | Biotech `ToxGasExposure.stages.*`. |
| gas mask / tox pack | 防毒面具 / 毒气背包 | Biotech `Apparel_GasMask.label`, `Apparel_PackTox.label` (also its `chargeNoun`). |
| pollution / polluted terrain | 污染 / 受污染的地形 | Biotech Keyed `PollutionLower`, `PollutedTerrain.label`. |
| toxic environment resistance | 有毒环境抗性 | Biotech stat label. |
| toxic fallout / toxic buildup | 有毒尘埃 / 中毒 | Core labels. |
| rot stink / deadlife dust / smoke | 腐气 / 死灵尘 / 烟雾 | Core Keyed `RotStink`, `DeadlifeDust`; 烟雾 is the prose form of Core's terse 烟. |
| chemfuel | 化合燃料 | Core `Chemfuel.label`. |
| sharp / blunt armor | 利器 / 钝器护甲 | Core `ArmorRating_Sharp/Blunt`. |
| vacuum resistance | 真空抗性 | Odyssey stat label. |
| Miscellaneous (settings section) | 杂项 | Core Keyed `MiscRecordsCategory`. |
| Biotech (Workshop) | Biotech | Left in English on the Workshop page, as Shipcracker leaves Odyssey; in-game DLC name is 生物技术. |
| tox gas tank | 毒气罐 | Coined; vanilla's own tank nouns are ~罐 (燃料罐, 烟罐). |
| vent (noun) / vent tank (ability) / vent rate | 排气阀 / 排放罐内毒气 / 排放速率 | Coined; kept apart from the tox pack's 释放毒气 so the two abilities do not share a label. |
| absorb / absorb gas | 吸收 / 吸收气体 | Coined; 气体, not 毒气, because it clears every gas. |
| gas jet | 毒气喷射 | Coined (settings section). |
| reserve jet ("Jets without a tank") | 无毒气罐时的喷射次数; prose 护肩自带的喷射 | Coined. |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | 喷射毒气 | Coined. |
| jet nozzle (shoulders' hardware) | 喷口 | Coined. |
| waster (xenotype) | 污骸种 | Biotech `XenotypeDef` `Waster.label`. |
| gene | 基因 | Biotech throughout (`GenesAndXenotypes.label` = 基因与异种人). |
| tox absorption (VREW gene) | 毒气吸收; 毒气吸收基因 | Coined: VREW ships English only. 毒气 (`ToxGas.label`) + 吸收, the absorb row's verb. |
| pollution mutations (VREW) | 污染突变 | Coined: 污染 (`PollutionLower`) + 突变 (Core's 突变小麦; Biotech's 变异 is its prefix-symbol slot, not prose). |
| tox cloud | 毒气云 | Already in `TXWC_VentRateDesc` (毒气云). |
| "Tank feeds waster tox absorption" (toggle) | 毒气罐供给污骸种的毒气吸收 | Coined; declarative like 险境型战棺可抵御真空. The Workshop bullet reuses 供给 (毒气罐供给污骸种驾驶员的毒气吸收基因), with 默认开启 for "on by default". |
| `Default: on.` | 默认：开。 | Core Keyed `On` = 开, pairing with this file's `Default: off.` = 默认：关 (Core `Off`). The inherited settings row's 默认：启用 (Core `Enabled`, as Shipcracker's `SCWC_VacuumBurnDesc`) has no Off counterpart in this file, so it is not used here. |
| Vanilla Races Expanded - Waster | Vanilla Races Expanded - Waster | Kept in English (Workshop title); suffix （需要Vanilla Races Expanded - Waster）. |

## Pending native review

Inherited and still open there: 呼吸器 (coined, no vanilla or community
source).

This mod's first pass (machine-assisted): 毒素型 (set name), 毒气罐,
排气阀 / 排放罐内毒气, 吸收气体, 毒气喷射, 喷射 (jet `chargeNoun`), 喷口,
喷射毒气, and the reuse of 燃料 for the tank's unit.

The VREW Waster update pass (machine-assisted): 毒气吸收 (gene), 污染突变,
the toggle label 毒气罐供给污骸种的毒气吸收, and 默认：开 beside this
file's 默认：关 (开 would mirror Core's On/Off pair).
