# Traditional Chinese glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there, no
native review yet); no translation pass has run here yet. Mod-specific terms
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
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | *pass* | Shipcracker's "fuel unit" is the plain mass noun 燃料 (燃料剩餘, 沒有燃料。), matching zh-Hans. Decide whether a counted 單位 reads better for a gauge counted in units of one. |
| jet (the shoulders' `chargeNoun`; one per activation) | *pass* | The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## To ground in the first pass

- cataphract (set name, lore paragraph 3): derive from the zh-Hans pack's `VFEP_Warcasket_Cataphract.label` and check against Royalty's zh-Hant `Apparel_ArmorCataphract.label`.
- hazard (set name, settings' vacuum toggle and Workshop FAQ): derive from the zh-Hans pack's `VFEP_Warcasket_Hazard.label`.
- the 7th-generation paragraph: derive from the zh-Hans pack's Aerial / Barrage / Hazard / Shock descriptions, which share it verbatim.
- "specialised warcaskets" research (Workshop FAQ): derive from the zh-Hans pack's `VFEP_SpecialisedWarcaskets.label`; cite in 「」.
- toxin (this set's name, in the `<set>`型 slot) and the Workshop title.
- pilot (the wearer): the zh-Hans pack's 驾驶员, checked against the zh-Hant tars.
- Biotech: tox gas, gas mask, tox pack, pollution, the `ToxGasExposure` hediff label and stages; Core: toxic environment resistance, toxic fallout, rot stink, toxic buildup.
- This mod's coinages: tox gas tank, vent (noun and verb), absorb, gas jet, reserve jet, spray gas.

## Pending native review

Every "derived" term above: converted from the zh-Hans pack and
character-checked against the zh-Hant tars, but the actual Traditional pack
could not be compared. Add this mod's own coinages here as the pass makes them.
