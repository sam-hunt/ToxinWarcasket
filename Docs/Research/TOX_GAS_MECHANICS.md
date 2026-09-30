# Tox Gas Mechanics

Working notes on how vanilla tox gas works in the engine, so the warcasket gas ability can be designed in a later session. Informational only, not mod content. Everything below was read from decompiled code or shipped XML; anything that could not be verified is flagged as such.

Sources, all read on 2026-09-30: game build `1.6.4871 rev591` (`gameBuild` in the sibling repo's `Scripts/expected-injections.json`), `Assembly-CSharp.dll` via `ilspycmd`, the shipped XML under `Data/Core` and `Data/Biotech`, and Vanilla Expanded Framework (VEF) `1.6/Assemblies/VEF.dll` plus its `Source/` tree at `../VanillaExpanded/VanillaExpandedFramework`.

## Summary

Biotech is required at the engine level to spawn or suffer tox gas. Three independent gates enforce it:

1. **Spawn gate.** `Verse.GasGrid.AddGas` for `GasType.ToxGas` calls `ModLister.CheckBiotech("Tox gas")` and returns without adding anything when Biotech is inactive (the check logs an error). `GasGrid.SetDirect` (both overloads) additionally masks the tox byte to zero when `!ModsConfig.BiotechActive`, so even a direct write cannot store tox gas.
2. **Effect gate.** `GasUtility.PawnGasEffectsTickInterval` only runs the tox branch under `ModsConfig.BiotechActive`. `HediffComp_SeverityFromGasDensityDirect.UpdateSeverity` also returns early for ToxGas without Biotech.
3. **Def gate.** `HediffDefOf.ToxGasExposure` and `DamageDefOf.ToxGas` are marked `[MayRequireBiotech]`, so both are null without Biotech. The `ToxGasExposure` HediffDef and the `ToxGas` DamageDef live in `Data/Biotech`.

What exists in Core without Biotech, all verified in `Data/Core` or `Assembly-CSharp`:

- `ToxicResistance` and `ToxicEnvironmentResistance` StatDefs (`Defs/Stats/Stats_Pawns_General.xml`).
- The `ToxicBuildup` HediffDef (`Defs/HediffDefs/Hediffs_Global_Misc.xml`, `lethalSeverity` 1) and `Verse.ToxicUtility`, which also serves toxic fallout and polluted terrain.
- The `ToxGasReleasing` EffecterDef (`Defs/Effects/Effecter_Misc.xml`) and `Fleck_ToxGasSmall` (`Defs/Effects/Fleck_Visual.xml`).
- The `ApparelProperties.immuneToToxGasExposure` field, the `GeneDef.immuneToToxGasExposure` field, the `GasType.ToxGas` enum value, and the Core comp classes `CompReleaseGas`, `CompProperties_ReleaseGas`, `CompToxPack`, `Verb_DeployToxPack`.

Correction to a pre-verified fact: `CompToxPack` and `CompAIUsablePack` are in namespace `Verse`, not `RimWorld` (`CompReleaseGas`, `CompProperties_ReleaseGas` and `Verb_DeployToxPack` are `RimWorld`). No fact given in the brief was otherwise contradicted.

## The gas grid

`Verse.GasType` is an enum: `BlindSmoke` 0, `ToxGas` 8, `RotStink` 16, `DeadlifeDust` 24. The values are bit shifts.

| Item | Detail (decompile-verified) |
| --- | --- |
| Storage | `GasGrid.gasDensity` is a `uint[]`, one entry per cell. Each gas takes one byte: `DensityAt(index, type)` is `(gasDensity[index] >> (int)type) & 0xFF`. `MaxGasPerCell` is 255. `DensityPercentAt` divides by 255. |
| Accessors | `GasUtility.GasDensity(this IntVec3, Map, GasType)` returns the byte. `GasUtility.AnyGas(...)` is density greater than 0. `GasGrid.AnyGasAt(cell)` tests whether any of the four bytes is set. |
| `GasUtility.AddGas(cell, map, type, float radius)` | Amount is `255 * GenRadial.NumCellsInRadius(radius)`, so it fills a disc of that radius to full density in theory, then calls `GasGrid.AddGas`. |
| `GasUtility.AddGas(cell, map, type, int amount)` | Direct passthrough to `map.gasGrid.AddGas(cell, type, amount)`. |
| `GasGrid.AddGas(cell, type, amount, canOverflow = true)` | Returns if `amount <= 0` or `!GasCanMoveTo(cell)`. Adds to the type's byte via `AdjustedDensity`, which clamps to 255 and reports the excess. Excess goes to `Overflow`, a flood fill (`GasCanMoveTo` filter, max radius 40 cells) that tops up neighbours to 255 each until the excess is used. ToxGas hits the `ModLister.CheckBiotech` gate here, DeadlifeDust hits `ModLister.CheckAnomaly`. |
| `GasUtility.AddDeadifeGas` | Anomaly only, in passing: gated on `ModsConfig.AnomalyActive`, also marks corpses for a faction. |
| Blocking | `GasCanMoveTo` rejects out-of-bounds cells and any edifice with `FillCategory.Full` unless it is an open `Building_Door`. `EqualizeGasThroughBuilding` handles gas passing vents and similar. |
| Dissipation | `GasGrid.Tick` visits `ceil(area / 64)` random cells per tick and calls `TryDissipateGases`. Per visit, each type loses `round(base * factor)` where factor is 0.5 for a roofed cell, 1 for open sky, plus `vacuum * 25`. Bases: `DissipationAmount_BlindSmoke` 4, `_ToxGas` 3, `_RotStink` 4, `_DeadlifeDust` 3. Roofed tox gas therefore loses 2 (round of 1.5) per visit, open tox gas 3. |
| Diffusion | `GasGrid.Tick` also visits `ceil(area / 32)` cells per tick and calls `TryDiffuseGases`. It only processes ToxGas, RotStink and DeadlifeDust (BlindSmoke is not in the diffusion path, so smoke only spreads by overflow). A cell is skipped if the sum of those three is under `MinDiffusion` 17. For each shuffled cardinal neighbour, `TryDiffuseIndividualGas` moves half the difference only when the source is at least 17 and the half-difference is at least 17. |
| Estimation | `EstimateGasDiffusion` (used only by the Anomaly corpse marking) assumes `EstimatedMaxGasPerCell` 48. |
| Gate on ticking | `GasGrid.Tick` returns early unless `CalculateGasEffects` is true. Not investigated further. |

Practical scale from the constants: one full-density cell (255) takes about 85 dissipation visits open, or 128 roofed. Each cell is visited on average once per 64 ticks, so a lone full cell lasts on the order of 5000 to 8000 ticks if it never spreads. Spreading thins the gas, and thin cells are removed quickly because dissipation is a flat subtraction per visit.

## Effects on pawns

`GasUtility.PawnGasEffectsTickInterval(Pawn, int delta)` runs every 50 ticks per pawn (`GasCheckInterval` 50, gated by `IsHashIntervalTick(50, delta)`) and only for spawned pawns. The tox branch:

1. Requires `ModsConfig.BiotechActive`. Reads the ToxGas byte at `pawn.Position`. If 0, nothing happens.
2. Severity factor `num = density / 255`.
3. If the pawn has `HediffDefOf.ToxicBuildup` and its `CurStageIndex` is the last stage, `num *= ToxGasEffectOnExtremeBuildupFactor` (0.25). This softens further buildup once the pawn is already at the extreme stage.
4. If `ShouldGetGasExposureHediff` (that is, `IsAffectedByExposure` and no existing `ToxGasExposure`), adds `HediffDefOf.ToxGasExposure`.
5. Calls `ToxicUtility.DoPawnToxicDamage(pawn, num)`.

`ToxicUtility.DoPawnToxicDamage(p, extraFactor)`:

- Base `0.023006668`, multiplied by `max(1 - ToxicResistance, 0)` and by `max(1 - ToxicEnvironmentResistance, 0)`, then by `extraFactor`.
- Multiplied by a per-pawn seeded random in 0.85 to 1.15 (`Rand.ValueSeeded(thingIDNumber ^ 0x46EDC5D)`).
- Applied with `HealthUtility.AdjustSeverity(p, HediffDefOf.ToxicBuildup, num)`.
- So at full density and no resistance a pawn gains about 0.023 `ToxicBuildup` per 50 ticks, with `lethalSeverity` 1 on that hediff (about 43 checks, roughly 36 seconds of sustained full-density exposure, ignoring natural decay). Tox gas is therefore lethal, not just a debuff.
- Both resistance stats apply to gas. Resistances of 1 or more zero the damage entirely, which is why the Hazard set's `ToxicResistance +1` already makes wearers gas-proof for buildup. Mechanoid races in Core and in VFE Pirates carry `ToxicResistance 1`, and Core mechanoids also `ToxicEnvironmentResistance 1` (XML grep only, per-race values not audited).

Note that `DoPawnToxicDamage` is not race-limited: animals and mechs take buildup unless resistant. Only the `ToxGasExposure` hediff is limited to humanlikes.

`GasUtility.IsAffectedByExposure(Pawn)` decides whether the `ToxGasExposure` hediff (the sight and breathing debuff, plus pain) is applied:

- Returns false for anything not `RaceProps.Humanlike` (the decompile tests `Humanlike` twice, a harmless duplicate).
- Returns false if any worn apparel has `def.apparel.immuneToToxGasExposure`.
- Returns false if any gene has `def.immuneToToxGasExposure` (Biotech genes set it in `GeneDefs_Health.xml` and `GeneDefs_Spectrum.xml`).
- Otherwise true.
- Consequence: `immuneToToxGasExposure` only prevents the hediff. It does not stop the `ToxicBuildup` damage. Buildup protection comes only from `ToxicResistance` and `ToxicEnvironmentResistance`. The vanilla gas mask sets both: `immuneToToxGasExposure` true and `equippedStatOffsets` `ToxicEnvironmentResistance` 0.8.
- Stat descriptions: `ToxicResistance` is "How well this creature resists toxic buildup" (Core). `ToxicEnvironmentResistance` "protects against toxic fallout and rot stink exposure, but not against direct attacks with venom or injected poison". The `BulletToxic` DamageDef (Biotech) scales its buildup by `ToxicResistance` only, so environment resistance does not stop toxic bullets.
- `Pawn_HealthTracker` and `Hediff` contain no reference to ToxGas or these stats (grepped). No `Hediff_ToxGasExposure` or `HediffGiver_ToxicBuildup` class exists in this build (`ilspycmd -l` found no such type).

The `ToxGasExposure` HediffDef (`Data/Biotech/Defs/HediffDefs/Hediffs_Local_Misc.xml`): `HediffWithComps`, `initialSeverity` 1, `maxSeverity` 3, three stages (mild, moderate, severe) with Sight and Breathing offsets and pain. Its comp `HediffCompProperties_SeverityFromGasDensityDirect` (`gasType` ToxGas, `intervalTicks` 60, `densityStages` 0.2, 0.5, 1) sets severity to stage index + 1 from the local density percent every 60 ticks, and `CompShouldRemove` removes the hediff when the cell has no tox gas (or the pawn is dead or unspawned and uncarried).

`GasUtility.GetLungRotAffectedBodyParts(pawn)`: the not-missing `Lung` parts that carry no hediff with `preventsLungRot`. It feeds only the RotStink branch (`LungRotExposure`), not tox gas.

## Vanilla emitters

Two distinct mechanisms produce gas. Density-per-cell explosions add up to `postExplosionGasAmount` (default 255) to every cell within radius in one instant. `CompReleaseGas` trickles gas into one cell over time.

| Emitter | Def and mechanism | Numbers (XML) |
| --- | --- | --- |
| Tox pack | `Apparel_PackTox`, Belt layer, `BeltDefenseTox` tag. `CompProperties_ApparelReloadable` (1 charge, Chemfuel, 35 per refill, `baseReloadTicks` 60, hotkey Misc4), `CompProperties_ReleaseGas`, `CompProperties_AIUSablePack` with `compClass` `CompToxPack`, and a `Verb_DeployToxPack` verb (non-violent, `targetable` false, `hasStandardCommand`, `nonInterruptingSelfCast`). | `gasType` ToxGas, `cellsToFill` 45, `durationSeconds` 12.75, effecter `ToxGasReleasing`. |
| Tox grenade | `Weapon_GrenadeTox` throws `Proj_GrenadeTox` (`Projectile_Explosive`). Projectile fields: `damageDef` ToxGas, `explosionDelay` 100, `explosionRadius` 1.9, `postExplosionGasType` ToxGas. The weapon also carries `CompProperties_Explosive` (radius 2.66, `postExplosionGasType` ToxGas) for when it is shot. | Range 12.9, warmup 1.5. |
| Toxbomb launcher | Ranged weapon firing `Bullet_ToxbombLauncher` (`Projectile_Explosive`): `damageDef` ToxGas, `explosionRadius` 1.9, `postExplosionGasType` ToxGas. | Range 23.9, warmup 3.5, `forcedMissRadius` 1.9. |
| Tox shell | `Shell_Toxic` (`CompProperties_Explosive`, radius 4, `postExplosionGasType` ToxGas, wick 30 to 60). Fired as `Bullet_Shell_Tox`: `explosionRadius` 0.1, `flyOverhead`, and `postExplosionSpawnThingDef` `Shell_Toxic_Releasing` (water variant `Shell_Toxic_Releasing_Water`), a `ThingWithComps` with `CompReleaseGas`. | Releasing thing: `cellsToFill` 20, `durationSeconds` 10, `CompProperties_DestroyAfterDelay` 30000 ticks. |
| IED tox trap | `TrapIED_ToxGas`: `CompProperties_Explosive` radius 8.9, ToxGas damage type, `postExplosionGasType` ToxGas. | Cost 2 tox shells. |
| Gas on damage | Toxic waste item with `CompProperties_GasOnDamage` (`type` ToxGas, `damageFactor` 6): `CompGasOnDamage` calls `GasUtility.AddGas` when damaged. | Not relevant to apparel. |
| Tox mechanoid weapons | None found. No ToxGas or `postExplosionGas*` reference appears under `Data/Biotech/Defs/ThingDefs_Races` (grep). Biotech's toxic bullets use `BulletToxic`, which is buildup by damage, not gas. | Unverified beyond the grep. |
| Other gases, in passing | Core `BlindSmoke` uses the same fields (`postExplosionGasType` BlindSmoke on the smoke launcher, smoke grenade and shell). Deadlife dust is Anomaly only. | |

Fields, with decompiled evidence that the explosion path is shared:

- `Verse.ProjectileProperties.postExplosionGasType` is a `GasType?`. It has no amount or radius fields.
- `Verse.CompProperties_Explosive` has `postExplosionGasType`, `postExplosionGasRadiusOverride` (`float?`) and `postExplosionGasAmount` (int, default 255).
- `Verse.Projectile_Explosive` passes `def.projectile.postExplosionGasType` into `GenExplosion.DoExplosion` with a null radius override and amount fixed at 255. `CompExplosive` passes all three of its props.
- `Verse.Explosion` carries `postExplosionGasType`, `postExplosionGasAmount` (default 255) and `postExplosionGasRadiusOverride`. In its per-cell method, for non-skipped cells, if the type has a value and the cell is within `radiusOverride ?? radius` (squared distance compare) it calls `GasUtility.AddGas(c, map, type, amount)`. That call is the int overload, so it goes through the `AddGas` Biotech gate and overflow logic.
- `Explosion.TrySpawnExplosionThing` calls `StartRelease()` on any `CompReleaseGas` of a spawned post-explosion thing, which is how the shell's releasing thing begins emitting.

`CompReleaseGas` decompile:

- `TotalGas` is `ceil(cellsToFill * 255)`. `GasReleasedPerTick` is `TotalGas / durationSeconds / 60`.
- The pack: 11475 total, about 15 per tick, released 450 per 30-tick pulse over 12.75 seconds.
- `StartRelease()` sets `started`. `CompTick` does nothing until then or if `parent.MapHeld` is null. While started it spawns the effecter on the wearer (via `EffecterSourceThing`, which resolves an apparel, carried or equipped parent to its pawn), and every 30 ticks (`ReleaseGasInterval`) calls `GasUtility.AddGas(parent.PositionHeld, parent.MapHeld, Props.gasType, amount)` at the holder's current position, so gas follows a moving wearer. When `remainingGas` reaches 0 it resets to full and stops.
- `Notify_WearerDied` stops it and refills. `PostPostMake` fills. State is saved via `remainingGas` and `started`.
- Ticking needs the comp to tick. The pack def relies on apparel ticking while worn. `CompProperties_ReleaseGas` has no tickerType of its own, so the parent def must tick (Apparel tickers are covered by the base class, but this was not checked for a custom apparel def).
- Not itself Biotech-gated: `CompReleaseGas` has no `ModLister` call, the gate is inside `AddGas`. Without Biotech the comp would tick, spend its charge and add nothing.

Reload coupling: `Verb_DeployToxPack.TryDeploy(reloadable, releaseGas)` first calls `ModLister.CheckBiotech("Tox packs")`, then requires a non-null `CompApparelReloadable` with `CanBeUsed` true and a non-null `CompReleaseGas`, then calls `reloadable.UsedOnce()` and `releaseGas.StartRelease()`. The verb is reached through the normal apparel verb gizmo, and `CompApparelReloadable` (extends `CompApparelVerbOwner_Charged`) supplies charge display and reload. `CompApparelReloadable.CanBeUsed` fails at 0 charges. `replenishAfterCooldown` is a props option that restores charges over `baseReloadTicks` (not set on the pack).

AI use: `CompToxPack` extends `CompAIUsablePack`. Every tick `CompAIUsablePack.CompTick` checks `CanOpportunisticallyUseNow`: wearer non-null, alive, spawned, on the `checkInterval` (60) hash tick, not downed, awake, not player-controlled colonist, not in a mental state, then `Rand.Value < ChanceToUse`. `CompToxPack.ChanceToUse` returns 0 without Biotech, otherwise sums `BodySize` of hostile pawns within radius 1.9 that `IsAffectedByExposure` and are not psychologically invisible (capped once the sum reaches 1). `UsePack` calls `Verb_DeployToxPack.TryDeploy`. Player-controlled colonists never auto-trigger.

## Design hooks for an apparel ability

None of these are implemented. All are for later design.

### (a) Clone the tox pack

Put `CompProperties_ApparelReloadable`, `CompProperties_ReleaseGas` and a `Verb_DeployToxPack` verb on the warcasket piece, with optional `CompProperties_AIUSablePack` / `CompToxPack`.

- `CompReleaseGas` itself needs nothing special from the wearer: it emits at `parent.PositionHeld` and fires the effecter on the wearer. `Verb_DeployToxPack.TryDeploy` hard-requires both `CompApparelReloadable` and `CompReleaseGas` on the same apparel (it returns false otherwise).
- Biotech-gated: `ToxGas` in XML (the `GasType` value parses in Core), the `ToxGasReleasing` effecter is Core, and both `TryDeploy` and `GasGrid.AddGas` check Biotech at runtime. The def is safe to load without Biotech but will silently or noisily do nothing. The cleaner isolation is the `1.6/Mods/Biotech/` load folder gate: the comp and verb additions go in a patch under that folder, so without Biotech the apparel simply has no gas comp.
- `Apparel_PackTox` itself has `researchPrerequisite` ToxGas (a Biotech ResearchProjectDef) in its recipe, so do not copy the `recipeMaker` block.

### (b) VEF apparel ability that adds gas

`VEF.Abilities.CompProperties_AbilitiesApparel` (with `CompAbilitiesApparel`) grants VEF abilities from worn apparel, as the VFEP Aerial and Shock pieces already do (see `WARCASKET_ABILITIES.md` in the sibling repo).

- No gas ability class exists in `VEF.dll`. `ilspycmd -l` matched only: `AbilityExtension_Explosion`, `Ability_Spawn` with `AbilityExtension_Spawn` (spawns a ThingDef), the `Projectile_GasGrenade` and smoke projectile classes (BlindSmoke visual grenades), `MVCF.Verbs.Verb_SmokePop` (BlindSmoke), and animal-only `CompGasProducer`. None exposes an `AddGas` call for ToxGas, and there is no `AbilityExtension_Gas` or `AbilityExtension_SpawnGas`.
- Reusable via XML only: `AbilityExtension_Spawn` can spawn a `ThingWithComps` that carries `CompProperties_ReleaseGas` and `CompProperties_DestroyAfterDelay`, mirroring `Shell_Toxic_Releasing`. However nothing calls `StartRelease()` on a spawned thing outside the explosion path, so this needs a small helper comp (for example a comp that calls `StartRelease` in `PostSpawnSetup`). Not verified against VEF `Ability_Spawn` internals.
- Custom route: a C# `Ability` subclass whose cast calls `GasUtility.AddGas(cell, map, GasType.ToxGas, amount or radius)`. Biotech-gated at runtime inside `AddGas`, and the compiled reference to `GasType.ToxGas` is safe in Core (enum member exists). Put the def under the Biotech load folder.

### (c) Explosion with gas

`VEF.Abilities.AbilityExtension_Explosion` has `postExplosionGasType` (`GasType?`), `postExplosionGasAmount` (default 255) and `postExplosionGasRadiusOverride`, and `Ability_Explode` (VEF source, `Ability_Explode.cs` lines 19 and 41 to 43) forwards them to `GenExplosion.DoExplosion`. So a tox burst around the wearer is pure XML: `explosionRadius`, `explosionDamageDef` (`ToxGas` for the harmless-damage gas explosion, `onCaster`, `casterImmune`), `postExplosionGasType` ToxGas. This is the least code route. Alternatives: a projectile ability with `postExplosionGasType` on its `ProjectileProperties` (radius and amount not configurable there, fixed at radius of the explosion and 255), or a `CompProperties_Explosive` with the three fields.

- Biotech-gated: `DamageDefOf.ToxGas` is a Biotech def (`[MayRequireBiotech]`), and `AddGas` refuses ToxGas. Isolate by shipping the ability def, its `DefModExtension` and any DefInjected text behind `1.6/Mods/Biotech/`.

### Load folder and wearer protection

- The repo's `LoadFolders.xml` already has the commented gate `<li IfModActive="ludeon.rimworld.biotech">1.6/Mods/Biotech</li>` (and `Mods/Biotech` for art). Uncomment when the first gas def lands. With a `LoadFolders.xml`, only listed folders load, and `Mods/` is not scanned unless listed.
- Wearer protection: `immuneToToxGasExposure` on a helmet's `<apparel>` block is a Core-defined field, so it can live in ordinary defs and is inert without Biotech. It only removes the `ToxGasExposure` sight and breathing debuff. It does not stop `ToxicBuildup`. For real protection use `ToxicResistance` or `ToxicEnvironmentResistance` `equippedStatOffsets` (Core StatDefs, inert without Biotech gas), as the gas mask does (`ToxicEnvironmentResistance` +0.8). Since resistance stats multiply, a resistance of 1 on either stat fully blocks buildup from gas, so the wearer of a gas-emitting casket needs only one of them at 1.
- Friendly fire is real: gas hurts allies and the caster unless protected. An emitter on a suit with `ToxicResistance +1` (like the Hazard set) is self-safe.

## Where the code lives

All types are in `Assembly-CSharp.dll` unless noted.

| Type | What it does | DLL |
| --- | --- | --- |
| `Verse.GasType` | Enum of gases, bit shift values | Assembly-CSharp |
| `Verse.GasGrid` | Per-map packed density storage, dissipation, diffusion, overflow, Biotech and Anomaly gates | Assembly-CSharp |
| `Verse.GasUtility` | `AddGas` overloads, `GasDensity`, `AnyGas`, `PawnGasEffectsTickInterval`, `IsAffectedByExposure`, `GetLungRotAffectedBodyParts` | Assembly-CSharp |
| `Verse.ToxicUtility` | `DoPawnToxicDamage`, `ToxicBuildup` maths and resistance stats | Assembly-CSharp |
| `Verse.HediffComp_SeverityFromGasDensityDirect` (+ Properties) | Maps local density to `ToxGasExposure` severity, removes hediff when gas is gone | Assembly-CSharp |
| `RimWorld.CompReleaseGas`, `RimWorld.CompProperties_ReleaseGas` | Timed gas release at the holder's position | Assembly-CSharp |
| `Verse.CompAIUsablePack`, `Verse.CompToxPack`, `Verse.CompProperties_AIUSablePack` | AI-only opportunistic pack use, tox pack chance logic | Assembly-CSharp |
| `RimWorld.Verb_DeployToxPack` | Player verb: spends a reloadable charge and starts release, Biotech check | Assembly-CSharp |
| `RimWorld.CompApparelReloadable` | Charges, reload and `CanBeUsed` for apparel | Assembly-CSharp |
| `Verse.Explosion`, `Verse.Projectile_Explosive`, `RimWorld.CompExplosive`, `Verse.ProjectileProperties` | `postExplosionGas*` handling and `TrySpawnExplosionThing` release start | Assembly-CSharp |
| `RimWorld.CompGasOnDamage` | Adds gas when a thing is damaged | Assembly-CSharp |
| `RimWorld.ApparelProperties` and `Verse.GeneDef` | `immuneToToxGasExposure` fields | Assembly-CSharp |
| `VEF.Abilities.AbilityExtension_Explosion`, `Ability_Explode` | Explosion ability with `postExplosionGas*` fields | VEF.dll |
| `VEF.Abilities.CompProperties_AbilitiesApparel`, `CompAbilitiesApparel` | Grants VEF abilities from worn apparel | VEF.dll |
| `VEF.Abilities.AbilityExtension_Spawn`, `Ability_Spawn` | Spawns a ThingDef at target | VEF.dll |
