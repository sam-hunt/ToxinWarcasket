# Korean glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); first machine-assisted pass 2026-10-13. Only
mod-specific coinages and grounding decisions; family-wide mechanics, josa
markers and style live in `l10n/languages/Korean.md`.

Community VFEP translation grounded against: the RimWorldKorea `RMK` pack
(GitHub RimWorldKorea/RMK), VFE Pirates' ThingDef, WarcasketDef and Keyed
files. Its warcasket set labels follow `<bare set prefix> 워캐스킷 [어깨|헬멧]`
and every set description opens with `우주시대 기술로 특화된 워캐스킷 ...입니다.`;
this mod copies both patterns, and takes the pack's Cataphract and Hazard
prefixes and its 7th-generation paragraph.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | Korean | Grounding |
|---|---|---|
| warcasket | 워캐스킷 | RMK VFEP, every warcasket label. No separate word for "shell": RMK drops it (`...워캐스킷입니다.`), and so does this mod. |
| `<set>` warcasket / shoulders / helmet (label pattern) | `<prefix>` 워캐스킷 / `<prefix>` 워캐스킷 어깨 / `<prefix>` 워캐스킷 헬멧 | RMK `VFEP_Warcasket*_*.label`; "pauldrons" in the description is rendered the same way (`워캐스킷 어깨입니다.`). |
| warcasket foundry | 워캐스킷 주조대 | RMK `VFEP_WarcasketFoundry.label`; bare "foundries" in lore is `주조대`. |
| entomb / weld on | 안치 (워캐스킷에 안치) | RMK `VFEPirates.EntombInWarcasket`; "welded into" / "weld it on" both use 안치. |
| siegebreaker / brute / guardian (set prefixes) | 돌격 / 야수 / 수호 | RMK set prefixes. |
| Imperial | 제국 (bare prefix) | Royalty backstories: `제국 부대`, `제국군`. |
| spacer-tech / spacer | 우주시대 기술 / 우주시대 | RMK VFEP descriptions (`우주시대 기술로 특화된`); Core `TechLevel_Spacer` = `우주`. |
| chassis | 프레임 | RMK's foundry text `워캐스킷 프레임`. |
| pilot | 조종사 | RMK `VFEP_WarcasketHelmet_Shock` (`조종사의 반응 시간`). |
| onboard respirator | 내장 호흡 장치 | Coined; Core `Breathing.label` = `호흡`. |
| sealed (helmet) | 밀폐형 | `밀폐` is the usual word for an airtight suit. |
| plasteel / uranium / chemfuel | 플라스틸 / 우라늄 / 화학연료 | Core / Odyssey labels. |
| jobReportString (`Using ability: {0}`) | {0} 능력 사용 중 | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | 재사용 대기 중 | Shipcracker's rendering of the same vanilla comp field. |
| cooldown (slider caption) | 재사용 대기 | Core Keyed `StatsReport_Cooldown` = `재사용 대기시간`; `시간` dropped from the caption so it does not repeat the `(시간)` unit. |
| radius | 범위 | Core Keyed `AbilityEffectRadius` = `효과 범위`. |
| Off (slider value at 0) | 없음 | Core Keyed `None` = `없음`; Core `Off` = `끄기` is a toggle verb. A toggle's "on" is `켜짐`. |
| default / (requires Odyssey) | 기본값 / (오디세이 필요) | Core Keyed `Default`; ExpansionDef `Odyssey.label` = `오디세이`; shape after Core `BlockedByTerrain` (`({0_label} 필요)`). |
| fuel tank (settings) | 탱크 / 연료 탱크 | Shipcracker's Workshop description; reconsider for a tox gas tank. |
| Workshop title | `<prefix>` 워캐스킷, the armor label | Contains `워캐스킷`. Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). Korean falls to the base `Pluralize` (lookup, else
unchanged), so one noun serves every slot and no lookup file is needed.

| English | Korean | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | 연료 | Same plain mass noun as Shipcracker's fuel unit, so the gauge reads `잔여 연료`, `연료당 재장전 비용`, `남은 연료가 없습니다` across the family; 단위 reads wrong in those slots. Lookup file: none needed. Gender: none (ko Core Keyed has no `CHARGENOUN_gender`). | Shipcracker's "fuel unit" is the plain mass noun 연료 (잔여 연료, 연료당 재장전 비용). Decide whether a counted 단위 reads better for a gauge counted in units of one. |
| jet (the shoulders' `chargeNoun`; one per activation) | 분사 | Same noun as the "Gas jet" section (가스 분사) and the spray-gas verb label; reads `잔여 분사`, `분사당 재장전 비용`. Lookup file: none needed. Gender: none. | The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## Grounded in the first pass

| English | Korean | Grounding |
|---|---|---|
| cataphract (set prefix) | 중무장 (`중무장 워캐스킷`) | RMK `VFEP_Warcasket_Cataphract.label`; Royalty's `근위대 갑옷` is the vanilla armor, not the VFEP set. |
| hazard (set prefix) | 재해 (`재해 워캐스킷`) | RMK `VFEP_Warcasket_Hazard.label`. |
| aerial (set prefix, Workshop) | 충격 | RMK `VFEP_Warcasket_Aerial.label` (RMK's Shock is 강습). |
| 7th-generation paragraph | `7세대 워캐스킷은 기존 성능을 ...취급되곤 했습니다.` | RMK Aerial/Hazard/Shock descriptions, verbatim; it already reflows the dashed aside into 모듈 방식, no dashes. |
| specialised warcaskets (research) | 전문화 워캐스킷 | RMK `VFEP_SpecialisedWarcaskets.label`. |
| spacer warcaskets | 우주시대 워캐스킷 | Shipcracker's Workshop FAQ. |
| specialized (description opener) | `무게는 늘어났지만 뛰어난 방어력을 갖춘 특화형 워캐스킷[ 어깨| 헬멧]입니다.` | RMK Cataphract opener `무게는 늘어났지만 뛰어난 방어력을 갖춘 신형 워캐스킷입니다.` with 특화형 for "specialized" (RMK's 7th-gen sets use `특화되었습니다`). |
| toxin (set prefix) / Workshop title | 독소 / 독소 워캐스킷 | Biotech `ToxGas.label` = `독소 가스`; title equals `TXWC_SettingsCategory`. |
| tox gas | 독소 가스 | Biotech `ToxGas.label`, `ToxGasExposure.label`. |
| ToxGasExposure stages | 약한 영향 / 보통 / 심각함 | Biotech HediffDef (not used in our text). |
| Tox Gas (research) | 독소 활용 | Biotech `ResearchProjectDef ToxGas.label`. |
| gas mask | 방독면 | Biotech `Apparel_GasMask.label`. |
| tox pack | 독소 등짐 | Biotech `Apparel_PackTox.label` (also its chargeNoun). |
| pollution / polluted terrain | 오염 / 오염된 지형 | Biotech Keyed `PlanetPollution`, `AlertToxicBuildupDesc`. |
| toxic environment resistance | 독성 환경 저항 | Core/Biotech `ToxicEnvironmentResistance.label`. |
| environmental toxins | 환경 독성 | Its description (`주위의 환경 독성`). |
| toxic fallout | 유독성 낙진 | Core `ToxicFallout.label`. |
| rot stink | 부패 가스 | Core `RotStink` stage labels, `LetterTextLungRot`. |
| toxic buildup | 중독 (hediff) / 독성 축적 (process) | Core `ToxicBuildup.label`; Biotech `AlertToxicBuildupDesc`. |
| smoke | 연막 | Core `Smoke.label`. |
| deadlife dust | 모조생명 먼지 | Core Keyed `DeadlifeDust`. |
| vacuum resistance | 진공 저항력 | Odyssey `VacuumResistance.label`. |
| Biotech (expansion) | 바이오테크 | ExpansionDef `Biotech.label`. |
| sharp / blunt armor | 날카로움 / 둔탁함 방어도 | Core `ArmorRating_Sharp/Blunt.label`. |
| range | 사거리 | Core Keyed `Range`. |
| respirator (helmet) | 호흡 장치 | Inherited 내장 호흡 장치, shortened. |
| tox gas tank | 독소 가스 탱크 | Coined. |
| vent (ability label / noun / verb) | 탱크 배출 / 배출구 / 배출 | Coined after Biotech's tox pack verb `독소 가스 배출`. |
| absorb gas / the absorb | 가스 흡수 / 흡수 | Coined. |
| gas jet / jets (hardware) | 가스 분사 / 분사기 | Coined. |
| reserve jet (tankless) | 어깨 자체의 분사, `탱크 미착용 시 분사 횟수` | Coined; shaped after Shipcracker's `어깨 미착용`. |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | 가스 분사 | Coined; same noun as the section and chargeNoun. |
| cone | 원뿔 | Coined (no vanilla analog). |
| shell's tank | 워캐스킷의 탱크 / 몸통의 탱크 | "shell" dropped per the warcasket row. |

## Pending native review

Nothing inherited is open for the VFEP rows (they are RMK's). This pass's
coinages: 독소 (set prefix), 독소 가스 탱크, 탱크 배출 / 배출구, 가스 흡수,
가스 분사 / 분사기, 분사 (jet chargeNoun), 연료 (unit chargeNoun), 원뿔,
특화형 in the description opener, and the Workshop's 화학전용.
