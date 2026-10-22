# Traditional Chinese glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there, no
native review yet); first machine-assisted pass 2026-10-13. Mod-specific terms
and grounding decisions only; family-wide mechanics, punctuation rules and
the zh-Hant/zh-Hans inversion table live in `l10n/languages/ChineseTraditional.md`.

**Community VFEP source.** The Workshop pack "Vanilla Factions Expanded - Pirates - 簡&繁中文漢化包"
(id 2727638702) attests the noun 戰棺 on its page, but its Traditional files
could not be obtained. The file-confirmed lineage is the zh-Hans pack (GitHub
q847633684/Vanilla-Expanded-ZH): 战棺, `<set>型战棺装甲` / `<set>型战棺护肩` /
`<set>型战棺头盔`, 战棺铸造厂. Terms marked "derived" are Traditional forms of
that pack's terms, each checked character by character against the zh-Hant
Core/Royalty/Odyssey tars (not a script conversion). The page's 無畏戰棺 prefix
is deliberately not used.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Traditional Chinese | Grounding / notes |
|---|---|---|
| warcasket | 戰棺 | Attested on the community pack's page; zh-Hans files use 战棺. Workshop title anchor. |
| `<set>` warcasket / shoulders / helmet (label pattern) | `<set>`型戰棺裝甲 / `<set>`型戰棺護肩 / `<set>`型戰棺頭盔 | Derived from the zh-Hans pack's pattern. 頭盔 and 裝甲 match vanilla (Core `Apparel_PowerArmorHelmet` 海陸頭盔, 海陸裝甲). |
| pauldrons / shoulder pads | 護肩 | Derived (zh-Hans 护肩, also the foundry UI's `VFEP.Shoulderpads`). |
| torso piece (Workshop) | 裝甲主體 | Derived from the foundry UI's `VFEP.ArmorFrame` (装甲主体). |
| shell (warcasket torso) | 外殼 | Core `Shell.label`, Royalty cataphract helmet 防護外殼. |
| warcasket foundry | 戰棺鑄造廠 | Derived (zh-Hans 战棺铸造厂); 鑄造廠 is Core `FoundryApprentice76`. |
| Siegebreaker / Brute / Guardian / Aerial (set names) | 破襲型 / 狂怒型 / 聖盾型 / 空襲型 | Derived from the zh-Hans pack; 空襲型 is coined there, unverified. |
| entombed / welded into the set (Workshop) | 封在戰棺內; assembling at the foundry 組裝 | zh-Hans pack's `VFEP.WarcasketText` (封入, 组装) rather than a literal "weld". |
| specialized spacer-tech | 特化型太空科技 | 特化型 from the zh-Hans pack's descriptions; 太空科技 is Core `Synthread.description`. |
| respirator | 呼吸器 | Odyssey rulestrings (組裝呼吸器). |
| Imperial | 帝國 | Royalty cataphract descriptions (帝國的甲冑騎士). |
| chemfuel | 化合燃料 | Core label. |
| jobReportString (`Using ability: {0}`) | 使用能力：{0} | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | 冷卻 | Shipcracker's rendering of the same vanilla comp field. |
| tank (the armor's fuel tank, settings) | 燃料箱 | Odyssey `FuelStorage.label` 燃料箱 (Core Keyed 燃料槽 is the fueling port); reconsider for a tox gas tank. |
| Default: {0}. / (default) / Off | 預設值：{0}。 / (預設) / 關閉 | Core Keyed `Default` 預設, `Off` 關閉; 預設值 per the Taiwan register list. |
| requires Odyssey (suffix) | (需要「漫遊」DLC) | Core `SimulateNotOwning*` 「漫遊」DLC. ASCII parens set solid, so the suffix drops the English leading space. |
| Workshop title | `<set>`型戰棺 | Contains 戰棺. Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). Chinese falls to the base `Pluralize` (lookup, else
unchanged), so one noun serves every slot and no lookup file is needed.

| English | Traditional Chinese | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | 燃料 | Shipcracker's mass noun, so the family reads alike. A counted 單位 fails vanilla's slots (沒有單位。, 單位剩餘), so the counter 單位 goes in our own strings instead (`TXWC_JetTankLow` 需要{COUNT}單位燃料, the summaries' {0}單位化合燃料). No gender slot in zh-Hant Keyed (CHARGENOUN_gender count 0). Lookup file: none needed. |
| jet (the shoulders' `chargeNoun`; one per activation) | 噴射 | Coined; reads in every vanilla slot (噴射剩餘, 沒有噴射。, 每噴射裝填消耗). Same noun as 毒氣噴射 (gas jet, settings section) and 噴射毒氣 (spray gas). Lookup file: none needed. |

## Grounded in the first pass

| English | ChineseTraditional | Grounding |
|---|---|---|
| toxin (set name) / Workshop title | 毒素型 / 毒素型戰棺 | Coined; 毒素 is the vanilla word for toxins (63 hits, Core `VenomFangs`, `BloodRot`). Labels 毒素型戰棺裝甲 / 護肩 / 頭盔. |
| cataphract (set name) | 騎士型 | zh-Hans pack `VFEP_Warcasket_Cataphract.label` 骑士型 (fetched 2026-10-13). Royalty's vanilla armor is 全覆式裝甲 (`Apparel_ArmorCataphract.label`); the VFEP set name follows the pack, like the inherited 破襲型. |
| hazard (set name) | 險境型 | zh-Hans pack `VFEP_Warcasket_Hazard.label` 险境型; 險境 attested in zh-Hant tars. |
| specialised warcaskets (research) | 「特化型戰棺」 | zh-Hans pack `VFEP_SpecialisedWarcaskets.label` 特化型战棺. |
| spacer warcaskets (research, hazard toggle) | 太空戰棺 | Shipcracker zh-Hant Workshop 「太空戰棺」. |
| "Specialized warcasket X with superior protection and worse weight" | 特化型戰棺X，在增加機體重量的同時大幅提升了防護性。 | zh-Hans pack's Cataphract opener (进阶型→特化型). |
| 7th-generation paragraph | 由於機體基本性能的強化陷入瓶頸… | zh-Hans pack's Hazard paragraph, dash reflowed into ，; 玻璃钢→塑鋼 (zh-Hant Core `Plasteel`). |
| pilot | 駕駛員 | zh-Hant Core/Odyssey `Shuttle.description` (駕駛員的生物特徵). |
| tox gas / tox gas research | 毒氣 / 「毒氣」 | Biotech `ToxGas.label` (gas and research). |
| ToxGasExposure label / stages | 毒氣 / 輕度、中度、重度 | Biotech `ToxGasExposure.*`. |
| gas mask | 防毒面罩 | Biotech `Apparel_GasMask.label`. |
| tox pack (its reload noun) | 毒氣背包 | Biotech `Apparel_PackTox.label` and its `chargeNoun`. |
| pollution / polluted terrain | 污染 / 受污染地形 | Biotech `StatsReport_Pollution`, `PollutedTerrain` 受污染{0}. |
| toxic environment resistance | 有毒環境抗性 | Biotech `ToxicEnvironmentResistance.label`. |
| toxic fallout | 毒霧霾 | Core `ToxicFallout.label`. |
| rot stink | 腐爛臭氣 | Core `RotStink.stages.rot_stink.label`. |
| toxic buildup | 中毒 (prose 毒性累積) | Core `ToxicBuildup.label`. |
| smoke / deadlife dust | 煙霧 / 死者粉塵 | Core `Smoke.label`; Keyed `DeadlifeDust` (Core tar). |
| vacuum resistance | 真空抵抗力 | Odyssey `VacuumResistance.label`. |
| requires restart (suffix) | (需要重新啟動) | Core Keyed 重新啟動; set solid like the inherited Odyssey suffix. |
| chemical warfare | 化學戰爭 | Core rulestring `subject->化學戰爭`. |
| tox gas tank (settings section, prose "tank") | 毒氣罐 (short: 罐) | Coined; 毒氣罐 is Core `TrapIED_ToxGas.description` (tox gas canisters). Replaces the inherited 燃料箱 for this mod only: the tank's output is gas. |
| vent (noun: the openings) / vent (verb, settings "vent rate") | 排氣口 / 排放 | 排氣口 Core `AncientSmokeVent.description`; 排放 same file. Ability label "vent tank" = 排放毒氣. |
| absorb gas (ability, settings section; "the absorb") | 吸收氣體 | Coined. |
| gas jet (settings section) / shoulder jets (hardware) | 毒氣噴射 / 噴射口 | Coined; 噴射 is vanilla's spray verb (Biotech `AcidSpray.description`). |
| reserve jet (tankless jets) | 無毒氣罐時的噴射次數 (prose: 護肩自身的噴射) | Coined, descriptive. |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | 噴射毒氣 | Coined; matches the charge noun. |
| cone width / range (jet) / radius | 錐形寬度(度) / 距離(格) / 半徑(格) | 錐形 Core rulestring; 距離(格) matches Shipcracker's jump range. |
| waster (xenotype) | 荒地人 | Biotech `XenotypeDef` `Waster.label`. |
| gene | 基因 | Biotech (345 value hits; `Learning_Fast.description` 基因擁有者). |
| tox cloud | 毒雲 | Biotech value prose (毒性物質會爆炸並產生毒雲); already used by this file's vent tooltips. |
| tox absorption (VREW gene label) | 「毒氣吸收」基因 | Coined: Biotech `ToxGas.label` 毒氣 + 吸收 (the absorb row's verb; Biotech `CannotAbsorbPollution` 吸收污染). VREW ships English only. Quoted in 「」 as a named gene. |
| pollution mutations (VREW) | 污染突變 | Coined: 污染 (Biotech, row above) + 突變 (Core mech descriptions 突變品種). |
| keeping (mutations) active / counts as | 保持活躍 / 視為 | Coined: an on-state while gas is present, not growth; 視為 (is treated as) from Core prose. |
| Tank feeds waster tox absorption (setting label) | 毒氣罐供荒地人吸收毒氣 | Coined, descriptive "the tank lets wasters absorb tox gas"; the tooltip names the gene. |
| the tank feeds (a gene, Workshop) | 毒氣罐會供給…「毒氣吸收」基因 | Coined; the Workshop bullet names the gene where the setting label does not. |
| Default: on. / On | 預設值：開啟。 / 開啟 | Core Keyed `On` 開啟; same shape as the inherited 預設值：關閉。 |
| requires VRE - Waster (suffix) | (需要Vanilla Races Expanded - Waster) | Mod title kept in English, set solid like the Workshop page's Latin names; ASCII parens solid, no leading space, like the other suffixes. |

## Grounded in the v1.2 update (spacer tier, aiming time)

| English | ChineseTraditional | Grounding |
|---|---|---|
| aiming time (the gas jet's warmup, settings) | 瞄準時間(秒) | Core `AimingDelayFactor.label` and Keyed `RangedWarmupTime` 瞄準時間; the tooltip mirrors `Stat_Thing_Weapon_RangedWarmupTime_Desc` 此武器從瞄準到發射的時間。 |
| spacer tier | 太空級 | Coined: 太空 is Core `TechLevel_Spacer`, 級 as in a grade. Toggle label 毒素型戰棺為太空級; 太空級的真空抵抗力 for "the spacer tier's vacuum resistance". |
| (included by spacer tier) (suffix) | (太空級已包含) | Same solid ASCII-paren, no-leading-space shape as the other suffixes. |
| Toxin / Hazard warcasket resists vacuum (toggles) | 毒素型戰棺具真空抵抗力 / 險境型戰棺具真空抵抗力 | Sibling labels; the Hazard one is unchanged from the first pass. |
| Specialized spacer-tech warcasket X with standard protection (opener) | 特化型太空科技戰棺X，在不增加機體重量的同時極大幅提升了防護性。 | zh-Hans pack's Siegebreaker / Controller / Sarcophagus opener (特化型太空时代级别战棺X，在不增加机体重量的同时极大幅提升了防护性), with the inherited 特化型太空科技 prefix. Loose for "standard protection", but it is how the pack reads on every spacer set, as the 7th-gen opener follows the pack's Cataphract. |
| 10th-generation paragraph | 第十代戰棺有些特殊，它們是… | zh-Hans pack's paragraph (repeated on every spacer set, here from `VFEP_WarcasketHelmet_Siegebreaker.description`), dash reflowed into ，; 兼容性→相容性 (Core `Incompatible` 不相容), 借助→藉助, quotes →「戰棺」. |
| veteran raiders | 前來襲擊的精英機兵 | zh-Hans pack `VFEP_Veteran.label` 精英机兵 (fetched 2026-10-22 from q847633684/Vanilla-Expanded-ZH 1.6 PawnKindDef.xml); 精英 attested in zh-Hant (Odyssey 打撈者精英). |
| spacer components | 高級零件 | Core `ComponentSpacer.label`. |
| spacer armor and insulation / no speed penalty / toxic and psychic resistance / shield bubble | 太空級的護甲與隔熱性能 / 無移動速度懲罰 / 毒素與心靈抗性 / 一層護盾 | Core `ToxicResistance.label` 毒素抗性, `MoveSpeed` 移動速度, Keyed 速度懲罰 (`WillGetWorkSpeedPenalty`), `ShieldBelt.label` 護盾; 隔熱 and 心靈抗性 coined (stat labels 隔絕 - 冷/熱 and 心靈敏感度 do not read as prose). |
| spacer plating / spacer costs (Workshop) | 太空級裝甲 / 造價也提高到太空級 | Coined from the rows above. |

## Pending native review

Every "derived" term above: converted from the zh-Hans pack and
character-checked against the zh-Hant tars, but the actual Traditional pack
could not be compared. Add this mod's own coinages here as the pass makes them.

Coinages from the 2026-10-13 pass, all pending native review: 毒素型 (set
name), 毒氣罐 (tank), 排放毒氣 / 排放 / 排氣口 (vent), 吸收氣體 (absorb),
毒氣噴射 / 噴射 (gas jet and its charge noun), 噴射口, 噴射毒氣 (spray gas),
the reserve-jet phrasing, and the derived VFEP terms 騎士型 / 險境型 / 「特化型戰棺」
and the 7th-generation paragraph (character-checked, not compared against the
Traditional pack).

Coinages from the 2026-10-17 VRE Waster update, pending native review:
「毒氣吸收」基因 (tox absorption), 污染突變 (pollution mutations), the
setting label 毒氣罐供荒地人吸收毒氣, and 保持活躍 for the mutations staying active.

Coinages from the 2026-10-22 v1.2 update, pending native review: 太空級
(spacer tier), the adopted pack opener's 極大幅 for "standard protection",
前來襲擊的精英機兵 (veteran raiders), 隔熱性能 and 心靈抗性 in the spacer tier
tooltip.
