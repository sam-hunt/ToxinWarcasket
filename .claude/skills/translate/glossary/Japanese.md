# Japanese glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); no translation pass has run here yet. Only
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
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | *pass* | Shipcracker's "fuel unit" is the plain mass noun 燃料 (燃料残数, 燃料回復コスト); 燃料ユニット would be a calque. Decide whether a counted ユニット reads better for a gauge counted in units of one. |
| jet (the shoulders' `chargeNoun`; one per activation) | *pass* | The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## To ground in the first pass

- cataphract (set name, lore paragraph 3): Royalty `Apparel_ArmorCataphract.label` (カタフラクト family), in the katakana set-name habit.
- hazard (set name, settings' vacuum toggle and Workshop FAQ): coin in katakana to match.
- toxin (this set's name) and the Workshop title.
- the 7th-generation paragraph: no community pack to mirror; follow the meaning, not VFEP's dashes.
- "specialised warcaskets" / "spacer warcaskets" research (Workshop FAQ): coin; cite in 「」 per the language file.
- Biotech: tox gas, gas mask, tox pack, pollution, the `ToxGasExposure` hediff label and stages; Core: toxic environment resistance, toxic fallout, rot stink, toxic buildup.
- This mod's coinages: tox gas tank, vent (noun and verb), absorb, gas jet, reserve jet, spray gas.

## Pending native review

Inherited and still open there: ショルダーアーマー and the resulting label
length, ウォーキャスケット鋳造台 (foundry) and 封入 (entomb), both unattested.
If the community VFEP translation surfaces, align these and the set names to
it in the Shipcracker glossary first. Add this mod's own coinages here as the
pass makes them.
