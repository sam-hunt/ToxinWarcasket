# Japanese glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); one machine-assisted pass has run here. Only
mod-specific coinages and grounding decisions; family-wide mechanics and
style (ASCII `,` `.`, no LanguageWorker, polite register in descriptions,
「」 vs ASCII quotes per slot) live in `l10n/languages/Japanese.md`.

**Community VFEP translation:** only the noun is attested. The Japanese mod
database rimworld.2game.info tags VFE Pirates content as ウォーキャスケット and
marks VFEP as 日本語化対応, but no translation files were found, so every other
VFEP term below (foundry, set names, part labels, entomb) is coined in the
Shipcracker repo and inherited here. Vanilla Armour Expanded renders
"siegebreaker" as シージブレーカー, which is the basis for keeping set names
in katakana.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Japanese | Grounding |
|---|---|---|
| warcasket | ウォーキャスケット | Community VFEP noun (rimworld.2game.info tagging). Required in the Workshop title. |
| `<set>` warcasket / `<set>` warcasket helmet (label pattern) | `<セット>`・ウォーキャスケット / `<セット>`・ウォーキャスケットヘルメット | Core/Royalty attach ヘルメット and アーマー directly (カタフラクトヘルメット); Biotech/Odyssey separate a katakana modifier from a katakana compound with ・. |
| shoulders / pauldrons / shoulder pads | ショルダーアーマー | Coined; no vanilla shoulder piece exists. Chosen over bare ショルダー and ショルダーパッド. |
| warcasket shell | ウォーキャスケット外殻 | Coined; plain word for "shell". |
| spacer-tech | 宇宙技術を用いた | Royalty `Apparel_PackJump.description` renders "spacer militaries" as 宇宙技術を用いる一部の軍隊. |
| spacer (tech tier, Workshop) | 宇宙世紀 | Core Keyed `TechLevel_Spacer`. |
| Imperial | 帝国 / 帝国の | Royalty (`Empire.pawnSingular` 帝国; `Apparel_ArmorCataphract` 帝国のカタフラクト部隊). |
| sealed | 気密 | Odyssey `GravshipHull.description` (気密壁). |
| respirator | 呼吸装置 | Coined plain term; no vanilla respirator item. |
| breathing (capacity) | 肺機能 | Core PawnCapacityDef `Breathing.label`. |
| plasteel / uranium / chemfuel | プラスチール / ウラン / バイオ液化燃料 | Core. |
| ability | 能力 | Royalty Keyed (この能力を獲得しますか). |
| pilot (warcasket wearer) | パイロット | Odyssey `PilotAssistant` (パイロットアシスタント). |
| warcasket foundry | ウォーキャスケット鋳造台 | Coined; Korean community uses 주조대, the same "casting bench" idea. |
| entomb | 封入 | Proposed coinage; not used in any shipped string of this mod. |
| siegebreaker / brute / guardian / aerial | シージブレーカー / ブルート / ガーディアン / エアリアル系 | Katakana; シージブレーカー attested in Vanilla Armour Expanded's Japanese, the others coined to match. |
| jobReportString (`Using ability: {0}`) | 能力を使用中: {0} | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | クールダウン中 | Shipcracker's rendering of the same vanilla comp field. |
| cells (settings slider unit) | タイル | Core Keyed `RoomRequirementArea` ({0}タイル); no space before the unit. |
| cooldown | クールダウン | Core Keyed `StatsReport_Cooldown`, `AbilityOnCooldown`. |
| Default / Off / On | デフォルト / Off / On | Core Keyed `Default`, `Off`, `On` (vanilla keeps On/Off in Latin script). |
| (requires Odyssey) / (default) suffixes | ` (Odysseyが必要)` / ` (デフォルト)` | ASCII parens with a leading space, as in vanilla Keyed; が必要 from Core Alerts (`医者が必要`). Odyssey stays Latin. |
| Workshop title | `<セット>`・ウォーキャスケット, equal to the armor label | Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). Japanese falls to the base `Pluralize` (lookup, else
unchanged), so one noun serves every slot and no lookup file is needed.

| English | Japanese | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | 燃料 | Shipcracker's tank noun, kept for the family: 燃料残数, 最大燃料数, 燃料回復コスト, 燃料が残っていません read as vanilla; a counted ユニット reads as a calque in those slots. Lookup file: none needed (base `Pluralize`, unchanged). Gender: none (`CHARGENOUN_gender` absent from ja Core Keyed). |
| jet (the shoulders' `chargeNoun`; one per activation) | 噴射 | Coined; 噴射残数, 最大噴射数, 噴射が残っていません. The same noun as the settings' ガス噴射 section and the spray-gas verb. Lookup file: none needed. Gender: none. |

## Grounded in the first pass

| English | Japanese | Grounding |
|---|---|---|
| toxin (set name) / Workshop title | トキシン / トキシン・ウォーキャスケット | Coined katakana, set-name habit; title equals `TXWC_SettingsCategory` and the armor label. |
| Toxin series | トキシンシリーズ | Coined. |
| cataphract | カタフラクト | Royalty `Apparel_ArmorCataphract.label` (カタフラクトアーマー). |
| hazard (set name) | ハザード / ハザード・ウォーキャスケット | Coined katakana. |
| 7th generation | 第7世代 | Shipcracker's 第10世代. VFEP's dashed aside reflowed as "パーツの互換性は保ちつつ,". |
| specialised warcaskets / spacer warcaskets (research) | 「特殊なウォーキャスケット」 / 「宇宙世紀のウォーキャスケット」 | Coined; the second is Shipcracker's Workshop rendering. 「」 per the language file. |
| specialized (part description opener) | 特殊な | Shipcracker's descriptions. |
| superior protection and worse weight | 防護性能に優れる反面,重量が増しています | Coined. |
| tox gas / tox gas (research) | 毒ガス | Biotech `ToxGas.label` (GasDef/ResearchProjectDef), `ToxGasExposure.label`. |
| tox gas exposure | 毒ガスへの曝露 | Biotech `ToxGasExposure.description`. |
| gas mask | ガスマスク | Biotech `Apparel_GasMask.label`. |
| tox pack | 毒ガスパック | Biotech `Apparel_PackTox.label` and its `chargeNoun`. |
| pollution / polluted terrain | 汚染 / 汚染された土地 | Biotech `PollutionPump`, `PollutedTerrain.label`. |
| toxic environment resistance | 毒環境耐性 | Biotech StatDef `ToxicEnvironmentResistance.label`. |
| toxic fallout | 死の灰 | Core GameConditionDef `ToxicFallout.label`. |
| rot stink | 腐敗臭 | Core `RotStink.stages.rot_stink.label`. |
| toxic buildup | 毒物の蓄積 | Core `ToxicBuildup.label` (毒物が蓄積), nominalised. |
| smoke | 煙幕 | Core `Smoke.label`. |
| deadlife dust | 死の塵 | Vanilla Keyed `DeadlifeDust`. |
| sharp / blunt (armor) | 突刺系 / 打撲系 | Core `ArmorRating_Sharp/Blunt.label`. |
| vacuum resistance | 真空耐性 | Odyssey `VacuumResistance.label`. |
| Miscellaneous | その他 | Core Keyed `Etc`. |
| (requires restart) | ` (再起動が必要)` | Core Keyed `ChangedTextureCompressionRestart` (再起動が必要です). |
| Mod settings (Workshop) | Mod 設定 | Core Keyed `ModSettings`. |
| tox gas tank | 毒ガスタンク | Coined. |
| vent (ability label) / vent (verb) / vents (the openings) | タンク放出 / 放出する / 放出口 | Coined; Core's 換気口 is the room vent, kept apart. |
| absorb gas (ability) / the absorb | ガス吸収 | Coined. |
| draw in (the respirator's action) | 吸い込む / 吸い戻す | Coined. |
| gas jet (settings section) / jets (the openings) | ガス噴射 / ガス噴射口, 肩部の噴射口 | Coined. |
| reserve jet / jets without a tank | ショルダーアーマー自体の噴射 / タンクなしの噴射回数 | Coined. |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | ガスを噴射 | Coined; follows Biotech `deploy_tox_pack` (毒ガスパックを展開) verb-phrase shape. |
| cone | 円錐 | Coined plain word. |

## Vanilla Races Expanded - Waster setting

| English | Japanese | Grounding |
|---|---|---|
| waster (xenotype) | ウェイスター | Biotech XenotypeDef `Waster.label`. |
| gene | 遺伝子 | Biotech Keyed `Gene`, `Genes`. |
| tox absorption (VREW gene) | 毒ガス吸収 / 毒ガス吸収の遺伝子 | Coined: 毒ガス (Biotech `ToxGas.label`) + 吸収 (Biotech Keyed `CanAbsorbPollution`, 汚染を吸収). The 毒 prefix keeps it apart from the helmet's ガス吸収 ability. VREW ships English only. |
| pollution mutations (VREW) | 汚染変異 | Coined: 汚染 (Biotech) + 変異 (Biotech `WasteRat.description` 変異種, `UniqueXenotypeDesc` ヒト変異体). They stay active as 有効な状態に保たれる. |
| keeping (mutations) active / counts as | 有効な状態に保たれます / 毒ガスとみなされ | Coined: an on-state while gas is present, not growth; みなす (is treated as). |
| Tank feeds waster tox absorption (toggle label) | タンクによるウェイスターの毒ガス吸収 | Coined noun phrase, the same shape as the `TXWC_HazardVacuum` label. |
| Default: on. | デフォルト: On. | The `Default: off.` pattern (デフォルト: Off.) with Core Keyed `On`. |
| (requires Vanilla Races Expanded - Waster) | ` (Vanilla Races Expanded - Wasterが必要)` | The Odyssey suffix pattern; the mod title stays English. |
| the tank feeds ... (setting, on by default) (Workshop bullet) | タンクが...にガスを供給します(設定,デフォルトはOn). | Coined; parenthetical attached before the period as in the page's Combat Extended bullet (未検証です(報告歓迎).). |

## Pending native review

Inherited and still open there: ショルダーアーマー and the resulting label
length, ウォーキャスケット鋳造台 (foundry) and 封入 (entomb), both unattested.
If the community VFEP translation surfaces, align these and the set names to
it in the Shipcracker glossary first.

This mod's first pass (machine-assisted): トキシン, ハザード (set names);
毒ガスタンク, タンク放出 / 放出口, ガス吸収, ガス噴射 and 噴射 (the jet
charge noun), ショルダーアーマー自体の噴射 (reserve jet), ガスを噴射; 燃料 as
the tank's charge noun inside a tank that is also measured in バイオ液化燃料
in the settings; 特殊な for "specialized"; the 7th-generation paragraph's
phrasing (no community VFEP rendering exists).

The VREW update pass (machine-assisted): 毒ガス吸収 for VREW's "tox
absorption" gene and 汚染変異 for its "pollution mutations" (no Japanese
VREW translation to align with), 有効な状態に保たれる for their staying active, and the toggle
label タンクによるウェイスターの毒ガス吸収.
