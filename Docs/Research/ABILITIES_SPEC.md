# Toxin Warcasket Abilities Spec

The design of the set's tox gas mechanics and the 7th-generation rebalance that came with them,
as shipped. Not mod content. Design decided 2026-10-08 and brought back in line with what shipped
on 2026-10-09; engine facts below were read from game build `1.6.4871 rev591`
(`Assembly-CSharp.dll` via `ilspycmd`, `Data/Core` and `Data/Biotech` XML), VFEP and VEF at the
checkouts under `../VanillaExpanded/`. `TOX_GAS_MECHANICS.md` is the engine map this builds on;
`VFEP_WARCASKET_STATS.md` holds the roster numbers quoted here.

The def headers and code comments are the record from here on (per CLAUDE.md); this file stays
as the design rationale and is not kept in sync with later tuning.

## Decisions

| Topic | Decision |
| --- | --- |
| Biotech | **Hard dependency.** Every emitter needs it (the gas grid refuses ToxGas, pollution is Biotech-only), so a Biotech-less set would be Cataphract plating at a Specialised price. All content lives in the main tree with no load-folder gate. |
| Tier | **Specialised (7th generation)**, beside Hazard: `VFEP_SpecialisedWarcaskets` **plus Biotech's `ToxGas`** (600, Industrial; the project that gates the tox pack and tox grenades). Vanilla `BuildableDef.IsResearchFinished` requires every listed prerequisite and VFEP's foundry filters its menu with it, so the second entry works with no code. VFEP's 7th-gen lore is "work better as a set" and "first to include plasteel", which a three-piece shared tank is. No 5th-gen piece carries any comp. Spacer (10th gen) is wrong: shields, vacuum, spacer components. |
| Plating, mass, speed | Keep the Cataphract's (Sharp 1.56 / Blunt 0.65 / Heat 0.78, 118 kg, MoveSpeed -1.35). Hazard also trades plating for function (Sharp 1.20); the slow chassis is the advancing gas cloud's identity. |
| Toxin stats | **Environment only.** No `ToxicResistance`; `ToxicEnvironmentResistance` armor 0.15 / shoulders 0.15 / helmet 0.8 (sum 1.1, clamps to the stat's max of 1.0); helmet keeps `immuneToToxGasExposure`. |
| Costs | Rebased to 7th-gen bands (table below). The tank's first fill is free, as VFEP prices its own reloadable sets; refills cost chemfuel. |
| Raider tags | **`WarcasketSpecialised`, `WarcasketAll`**, replacing the Cataphract's `WarcasketHeavy`/`WarcasketCata` (reasoning below). |
| Tank scale | Sized against the tox pack: close to three packs of gas a tank, two thirds of a pack a jet, so the set reads in a unit the player already knows. |
| Vent rate | 2 charges a second, 5% of the tank, a full tank in 20 s. A faster vent only makes the same gas a wider, shorter cloud (see Vent Tank). A mod setting. |
| Jet fuel | The worn tank, and nothing else, whenever one is worn. The shoulders' own two jets are a fallback for wearing them without the tank, never a reserve that stacks on it. |
| Absorb | A VEF ability on the helmet, cast without a job so the pilot keeps moving and the disc follows them. Banks into the tank only. |
| Settings | Mod settings override def fields in place at startup and when the window closes, with defaults read from the defs, so the def headers stay the record of the shipped tuning. |

### Why environment-only splits cleanly from Hazard

No VFEP warcasket carries `ToxicEnvironmentResistance` or `immuneToToxGasExposure` (grep of all
VFEP XML). Hazard's helmet carries `ToxicResistance +1`. Engine facts:

- `ToxicUtility.DoPawnToxicDamage` (tox gas, and fallout via `DoAirbornePawnToxicDamage`, and
  polluted terrain) multiplies by `1 - ToxicResistance` **and** `1 - ToxicEnvironmentResistance`,
  so either stat at 1.0 zeroes buildup from all of them.
- Only `ToxicResistance` covers direct toxins: `BulletToxic` (Biotech's toxic needle gun),
  venom, injected poison.
- Two Biotech effects test `ToxicEnvironmentResistance` with an exact threshold:
  `ThoughtWorker_NoxiousHaze` (acidic smog mood) is skipped at `>= 1f`, and
  `HediffComp_ImmunizableToxic.SeverityChangePerDay` progresses on polluted cells only while
  `< 1f`. Three float offsets of 0.8 + 0.1 + 0.1 may sum to 0.99999994 and miss both, hence the
  deliberate overshoot to 1.1.

Result: **Hazard shrugs off toxins that hit it; Toxin breathes poisoned air.** A Hazard pilot in
gas takes no buildup but still gets the `ToxGasExposure` debuff (sight, breathing, pain); a full
Toxin pilot gets neither, and is the set that fights inside its own cloud. Toxin is exposed to
toxic needles and venom; Hazard is not. Helmet alone equals a vanilla gas mask (0.8 plus
exposure immunity). Partial sets matter for the downed burst (see Edge cases).

### Raider tags

VFEP's pawn generator fills a warcasket wearer's empty slots with pieces whose tags intersect
the pawn kind's `apparelTags`. Among the same-tier sets, each one carries `WarcasketAll` plus,
where one exists, a role tag that pairs it with a pawn kind holding the matching weapon:
`WarcasketHussar` (Aerial, Shock) with `VFEP_Hussar`, `WarcasketSuperHeavy` (Barrage) with
`VFEP_Artillery`, `WarcasketFlamer` (Hazard) with `VFEP_Pyro`. `WarcasketSpecialised` (Barrage,
Hazard) is listed by no pawn kind. VFEP has no tox weapon and no pawn kind for one, so the Toxin
has no role tag to take and gets the tier's two common tags. The only VFEP pawn kind listing
`WarcasketAll` is the mercenary `VFEP_General`, so raiders in the set are rare (the Cataphract
tags would put it on `VFEP_Major`, `VFEP_HeavyWeaponsPlatform` and `VFEP_Artillery`). That suits
a 7th-gen set; a dedicated tox-trooper pawn kind would be the way to raise its presence later.

### Costs

| Piece | Cataphract (template) | Toxin |
| --- | --- | --- |
| Armor | Comp 5, Steel 175, Uranium 50 | Comp 8, Steel 150, Plasteel 20, Uranium 50 |
| Shoulders | Steel 75 | Comp 2, Steel 55, Plasteel 20 |
| Helmet | Comp 2, Steel 75 | Comp 3, Steel 45, Plasteel 20, Chemfuel 20 |
| Set | Comp 7, Steel 325, Uranium 50 | Comp 13, Steel 250, Plasteel 60, Uranium 50, Chemfuel 20 |

The armor carries no chemfuel: `CompApparelVerbOwner_Charged.PostPostMake` fills the tank on
creation, and VFEP's own reloadable sets (the Shock's armor and the Aerial's shoulders hold 100
chemfuel charges) cost none, where vanilla's packs charge their fill up front. The helmet's
Chemfuel 20 is the respirator's filter, priced as the gas mask prices its own. The shoulders'
Comp 2 is the jet nozzle (Controller's shoulders pay components for their verb). For comparison,
the other 7th-gen sets total Comp 11 to 14, Steel 220 to 360, Plasteel 65 to 75, Uranium 50.

## The tank (armor)

Every emitter draws on one tox gas tank on the armor.

- **Class:** `CompToxTank : CompApparelReloadable` (props
  `CompProperties_ToxTank : CompProperties_ApparelReloadable`). `remainingCharges` is
  `protected` on `CompApparelVerbOwner_Charged`, so the subclass adds `TryConsume(int)`,
  `Add(int)` (capped at `MaxCharges`) and `Empty()` without reflection. Vanilla reload jobs, the
  "reload" float menu, save/load and the full tank on creation come from the base class.
- **Units:** `maxCharges` 40, shown to the player as "units" (`chargeNoun`) because the vent
  drains the tank continuously and it reads as a volume. One charge is `cellsPerCharge` full
  cells of gas, 3, so a full tank is 120 cells against the tox pack's 45, close to three packs.
  Capacity and cells per charge are mod settings; a tank holding more than a lowered capacity
  drops the excess on the next tick.
- **Refill:** `ammoDef` Chemfuel, `ammoCountPerCharge` 2 (80 chemfuel a tank; three packs'
  refills are 105), `baseReloadTicks` 60. The tank is a permanent commitment where the pack is a
  utility slot, so it holds more and refills from the helmet.
- **Readout:** the gauge `Gizmo_ToxTank`, vanilla's `Gizmo_Slider` (the transport pod launcher's
  fuel gizmo) over the tank: the bar is the fill in the canisters' lime, and for the player's
  wearers the drag handle sets `TargetCharges`, the level colonists reload to, full by default.
  Vanilla's reload queries are not virtual, so Harmony prefixes on
  `CompApparelReloadable.NeedsReload` and `MaxAmmoNeeded`
  (`Patches/CompApparelReloadable_Reload.cs`) count against the target, as a refuelable building
  fills to its fuel target; every other reloadable passes through.
- **Ticking:** the armor already resolves to `tickerType` Normal via `VFEP_WarcasketArmorBase`.

### Passive: downed burst

When the wearer goes down with the armor on, the whole tank vents at once as a gas explosion,
sized by what was in it.

- **Trigger:** `CompToxTank.CompTick` watches `Wearer.Downed` for a false-to-true edge (worn
  apparel gets no downed notification: `ThingComp.Notify_Downed` is called on the pawn's own
  comps only). The edge state is saved and resynced on equip, so a load or a downed pawn being
  dressed never reads as a fresh fall.
- **Skip when:** the wearer is unspawned, the tank holds fewer than `minBurstCharges` (5), or
  the pawn carries the `Anesthetic` hediff. The anesthetic guard is what stops a medical
  operation (VFEP's warcasket removal surgery among them) from gassing the hospital. A pawn that
  is downed, stood up and downed again bursts again only if the tank was refilled in between.
- **Effect:** `GenExplosion.DoExplosion(position, map, radius, DamageDefOf.ToxGas, wearer,
  postExplosionGasType: GasType.ToxGas, postExplosionGasAmount: GasGrid.MaxGasPerCell)` with
  `radius = GenRadial.RadiusOfNumCells(cells)`, where `cells` is the tank's gas in full cells
  (charges times cells per charge): about 6.2 for a full tank, 3.1 for the 10 charges of a last
  jet, against the tox grenade's 1.9. The gas goes down at full density over as many cells as
  it fills, so the gas-per-charge setting sizes the cloud rather than thinning it; a thinner
  cloud would never reach the exposure hediff's top stage, which takes a full cell. `ToxGas` is
  the tox grenade's harmless damage def (`defaultDamage` 0, Biotech), so the explosion carries
  the visuals and sound and the gas does the work. Explosions skip cells without line of sight
  from the centre, so walls shape the cloud. Then `Empty()`.

### Passive: death rupture

When the wearer dies with the armor on, the canisters burst and foul the ground: a blast mark
under the corpse, chemical slime, spent acid and bile splashed around it.

- **Trigger:** `ThingComp.Notify_WearerDied`, called from `Apparel.Notify_PawnKilled`.
  `Pawn.Kill` runs `PreDeathPawnModifications` (which notifies apparel) **before**
  `DeSpawnOrDeselect`, so `Wearer.Position` and `Wearer.Map` are still valid. Skip if unspawned
  (caravans, transporters, holding containers).
- **Burst:** if the tank still holds 5 or more charges (killed outright rather than after a
  downed burst), fire the downed burst first.
- **Pollution:** every cell within `ruptureRadius` 2.0 where `map.pollutionGrid.CanPollute(cell)`
  gets `SetPolluted(cell, true)`. 2.0 covers 13 cells; 1.9 would cover 9 (`GenRadial` counts by
  `x² + z² <= r²`, so distance 2 is excluded). `SetPolluted` calls `ModLister.CheckBiotech`,
  fine with Biotech required.
- **Filth (all Core defs):** `Filth_BlastMark` on the centre cell; every other cell in the
  radius has a 70% chance of one of `Filth_Slime`, `Filth_SpentAcid`, `Filth_CorpseBile`
  (equal weights), via `FilthMaker.TryMakeFilth`, then a 30% roll for a second filth of a
  different kind for the layered look. Chances are props fields.
- Raiders in the set rupture too: each dead Toxin raider leaves 13 polluted cells on the
  player's map, which Biotech's clear-pollution work cleans. Intended flavor.

### Ability: Vent Tank (armor)

A toggle that empties the tank into a growing cloud around the pilot, who keeps moving.

- **Route:** VEF `CompAbilitiesApparel` on the armor granting `TXWC_VentTank`;
  `Ability_VentTank : VEF.Abilities.Ability` returns a plain `Command_Toggle` from `GetGizmo`
  (VFEP's `CommandAbilityToggle` is the precedent; no dependency on the VFEP type). The ability
  only flips `CompToxTank.Venting`; the tank comp ticks the emission, so state lives in one
  saved place. `CanAutoCast` is false: a toggle has nothing to autocast.
- **Emission:** `ventChargesPerSecond` 2, a mod setting, accrued as a fraction each tick and
  spent in whole charges, each `GasUtility.AddGas(wearer.Position, map, GasType.ToxGas,
  GasPerCharge)`. The owed fraction starts at one so a vent emits as it opens; a toggle-off
  wastes at most one charge. `GasGrid.AddGas` clamps the cell and floods the excess outward
  (`Overflow`, flood fill up to radius 40, blocked by walls and closed doors), so the cloud
  grows from wherever the pilot stands and trails them when they walk.
- **Why 2 a second:** the grid sets a cloud's life by its footprint, not its density. Every
  gassed cell loses 3 of 255 per 64 ticks unroofed (half that roofed) whatever its density, and
  neighbours equalize whenever they differ by 34 or more, so a faster vent only makes the same
  gas a wider, shorter cloud. On an open field (`gas-grid-sim.py`) the pack's release peaks at
  about 70 cells at the exposure hediff's mild stage or worse and is gone after 50 s; this tank
  at two a second peaks at about 165 and is gone after 80 s.
- **Stops when:** toggled off, the tank empties, the wearer is downed (the burst takes over),
  dies, despawns, or the armor comes off. Toggling off keeps what is left.
- **Visuals:** Core's `ToxGasReleasing` effecter on the wearer, the tox pack's.
- **Availability:** drafted only, 1 or more charges to start, no cooldown (fuel is the limiter,
  as on Aerial and Shock). The empty-tank reason quotes the tank's whole chemfuel range rather
  than the reload target's.
- **Exclusion:** venting and the helmet's absorb exclude each other, whichever starts last
  winning (see Absorb Gas).

## Ability: Absorb Gas (helmet)

The respirator inhales the air around the pilot and banks it in the tank.

- **Route:** VEF `CompAbilitiesApparel` on the helmet granting `TXWC_AbsorbGas`, self-cast,
  `cooldownTime` 7500 ticks (three in-game hours), `durationTime` 300, `radius` 3.5, usable
  undrafted. `Ability_AbsorbGas : VEF.Abilities.Ability` runs the draw itself on the comp's
  tick (`needsTickingInterval`), so the helmet redeclares `tickerType` Normal (the helmet base's
  is Never) and the draw ends if the helmet comes off. There is no hediff.
- **Jobless cast:** VEF's `StartAbilityJob` ends the pilot's current job and starts its cast
  job even at castTime 0, which stops a moving pilot. The absorb has no warmup, so
  `CreateCastJob` calls `Cast` directly, the path VEF takes for a caravan member, and the pilot
  carries on with the disc following them.
- **Effect:** every 30 ticks (10 pulses over 5 s) for each cell within radius 3.5 (37 cells, a
  disc seven across) of the pawn's current position and in the pilot's line of sight, remove up
  to 64 units from **each** gas byte (smoke, tox, rot stink, deadlife). Four pulses clear a full
  cell; the remaining six catch gas diffusing back in and whatever the pilot walks into. Write
  with `GasGrid.SetDirect(index, smoke, toxic, rotStink, deadlife)` and call
  `map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Gas)` afterwards (`AddGas` does the same;
  `SetDirect` alone does not redraw). `AddGas` cannot subtract (it returns on `amount <= 0`).
- **Refill:** units absorbed, summed across gas types, bank one charge per `GasPerCharge` units
  (the gas a charge vents, so the gas-per-charge setting is the exchange rate both ways) into
  the armor's `CompToxTank`, if worn; the remainder carries between pulses and is saved. The
  tank caps at capacity and excess is lost. Without the armor, the ability still clears the air.
  Smoke and rot stink become tox gas by design: the filter is the conversion. A saturated disc
  (37 full cells at 3 cells a charge) is about 12 charges, a jet and change, so reclaiming a
  cloud tops the tank up rather than refilling it.
- **Exclusion:** a cast closes a venting tank, and `CompToxTank.SetVenting(true)` stops a
  running draw, so the respirator never pulls in the vent's own cloud. The stopped draw's
  cooldown stands.
- **Autocast:** VEF's own autocast only offers a pawn's learned abilities as attack verbs,
  which apparel abilities never are, so the ability reads the toggle itself: off by default,
  checked every 60 ticks, casting when the pilot's cell holds any gas (summed density 1 or
  more). Player pilots only, never while the worn tank is venting. The toggle survives VEF's
  re-`Init` on wearer change and load.
- **Visuals:** every cell a pulse thins throws a `TXWC_AbsorbedGas` fleck into the pilot in the
  colour of the gas it gave most of (vanilla's tox and deadlife fleck colours; smoke and rot
  stink matched by eye); a pulse over clean air throws four of Core's `AirPuff` from the disc's
  edge toward the pilot instead, so the draw always shows.
- **Uses:** clears enemy tox grenades, tox shells, toxalope clouds or smoke off allies;
  recovers your own vented cloud; refills mid-fight.
- **Dev:** `CompDevResetCooldowns` on the helmet adds a dev-mode "reset cooldown" for its VEF
  abilities, beside the tank's vanilla "reload to full".

## Ability: Gas Jet (shoulders)

The tox gas version of the warcasket heavy flamer: a directional cone, safer near allies than
the vent.

- **Route:** a vanilla apparel verb on the shoulders, as the Barrage shoulders do, so the
  shoulders redeclare `tickerType` Normal for the verb tracker and the AI check (the shoulder
  base leaves it at Never). `onlyManualCast`, as the Barrage's: a drafted colonist never fires
  it on its own. A player order casts once where the pilot stands.
- **Verb:** `Verb_SprayGas : Verb`, modelled on vanilla `Verb_SpewFire` (Biotech scorcher's
  mini-flameblaster), which spends its reloadable inside `TryCastShot` and calls
  `GenExplosion.DoExplosion` with an `affectedAngle`. Ours computes the cone itself
  (`ConeCells`: walkable cells in the pilot's line of sight between `coneStart` and range,
  within `halfAngle` of the line to the target) and passes it as `overrideCells`, because the
  explosion's own angle test compares raw Atan2 degrees and cuts any cone straddling due west in
  half. The shot's gas is shared evenly over the cone's cells, so walls concentrate it rather
  than waste it; past a full cell `AddGas` overflows the excess into neighbours, so a cone
  smaller than the gas thickens at its edges. `DamageDefOf.ToxGas` carries no damage; the gas
  does the work.
- **Numbers:** range 12.9, half-angle 12 (a 24 degree cone), `coneStart` 3, warmup 1 s,
  `propagationSpeed` 0.6. The cone from 3 cells out to 12.9 is about 32 cells, so 10 charges at
  3 cells each (30 cells, two thirds of a pack) land just under full density on open ground.
  `coneStart` 3 because the angle test makes the cone a single-cell line for its first four
  cells, the pilot is immune and whoever stands beside them is not, and a one-wide stem bleeds
  sideways as much as any edge, so the shot begins where the cone is already three wide. Cone
  width and range are mod settings.
- **Targeter:** the range ring, the cone the shot lands in, and fainter around it the cells
  `GasSpreadEstimate` expects to hold any gas within 300 ticks, from a replay of the grid's own
  dissipation, diffusion and overflow rules. The threshold is 1 because the gas layer draws a
  cell holding one unit exactly like a full one, so the outer area is where the cloud will
  show and the cone is the part that bites.
- **Fuel:** `CompJetNozzle : CompApparelReloadable`, `tankChargesPerShot` 10 (a mod setting).
  - **With a tank worn** (`CompToxTank.WornBy`): a jet draws 10 charges from the tank and
    nothing else. Below 10 the command is disabled with the Keyed reason `TXWC_JetTankLow`
    ("Needs {COUNT} {CHARGENOUN_plural} in the tank."). The command's corner counts the tank's
    jets, remaining over capacity at the cost: 4 / 4 on a full tank, falling with the vent and
    rising with the absorb, since the tank is shared.
  - **Without a tank:** the nozzle's own two jets are the fuel (`maxCharges` 2, `chargeNoun`
    "jet", `ammoCountPerCharge` 20, the same chemfuel the tank spends on a jet), vanilla
    behaviour, corner 2 / 2, reloaded by the vanilla job. They exist so the shoulders keep their
    use worn alone or over another set's armor, not as a reserve that stacks on the tank.
  - **No stacking:** while a tank is worn the nozzle's own jets are never spent and never
    reloaded. The `NeedsReload` prefix in `Patches/CompApparelReloadable_Reload.cs` returns
    false for a `CompJetNozzle` whose wearer wears a tank; vanilla routes `MinAmmoNeeded`,
    `MaxAmmoNeeded`, `ReloadFrom`, the reload job and the float-menu reload through
    `NeedsReload`, so one prefix covers them all.
  - `GasPerShot` is the cost times the gas-per-charge setting, from the tank or an own jet
    alike; the settings recompute the nozzle's `ammoCountPerCharge` as the cost times the
    armor's chemfuel per charge.
  - `CanBeUsed` on the tank route repeats `CompApparelVerbOwner`'s pocket-map and vacuum checks,
    because `CompApparelReloadable.CanBeUsed` tests the nozzle's own charges before reaching
    them and C# cannot skip a level.
- **Visuals:** `TXWC_GasJetSpray`, an effecter throwing tox gas flecks along the line
  (vanilla's `Fire_SpewShort` is fire-coloured). The command's tooltip is the nozzle's
  `jetDescription`, as the gear's own description is the set's lore.

## Mod settings

`ToxinWarcasketSettings` holds one `Tunable` per number below. Each is written onto its def
field by `ApplyToDefs` at startup (from `ModInit`, after `PatchAll`, the first point defs exist)
and whenever the settings window closes, and reads its default from that same def field, so
every reader, vanilla's and VEF's included, sees the setting with no code of its own and the def
header stays the record of the default. Only an override is saved; a slider landing within half
a step of the default clears it. `ToxinWarcasketMod` owns the window only and never touches
defs or patches, because `Mod` constructors run before defs load.

| Setting | Def field | Default |
| --- | --- | --- |
| Tox gas per unit (cells) | tank `cellsPerCharge` | 3 |
| Capacity (units) | tank `maxCharges` | 40 |
| Vent rate (units a second) | tank `ventChargesPerSecond` | 2 |
| Jet cost (units) | nozzle `tankChargesPerShot`, and its `ammoCountPerCharge` as cost times the tank's | 10 |
| Jets without a tank | nozzle `maxCharges` | 2 |
| Cone width (degrees) | verb `halfAngle`, halved | 24 |
| Range (cells) | verb `range` | 12.9 |
| Absorb radius (cells) | ability `radius` | 3.5 |
| Absorb duration (seconds) | ability `durationTime` | 5 |
| Absorb cooldown (hours) | ability `cooldownTime` | 3 |

The one value read through the settings object rather than a worn comp is the gas per charge
(`ToxinWarcasketSettings.GasPerCharge`, cells per unit times `GasGrid.MaxGasPerCell`): every
emitter and the absorb's banking rate take it from there, because the shoulders' own jets need it
with no tank worn. Strings live in `1.6/Languages/English/Keyed/ToxinWarcasket.xml`.

## AI

The passives (downed burst, death rupture) work for raiders with no AI code. The actives:

- **Vent:** `CompToxTank.AIVentCheck` for non-player wearers, modelled on
  `CompToxPack.ChanceToUse`: every 60 ticks, if the tank holds 10 or more charges and hostile
  pawns within 4 cells that `GasUtility.IsAffectedByExposure` sum to body size 1 or more, start
  venting; stop after 300 ticks without any. Raiders then drive a moving cloud into the player's
  line. A colonist who loses player control mid-vent is handed to this check.
- **Gas jet:** the vanilla AI only turns to an apparel verb when the pawn has no usable weapon
  (`Pawn.TryGetAttackVerb`), so an armed raider would never jet. `CompJetNozzle.AIJetCheck`
  starts the cast itself every 60 ticks, as `CompToxPack` deploys its pack: at the hostile at the
  heart of the densest group the gas would affect within 2.9 cells of it (body size 1 or more),
  skipping any group with an affected pawn of the wearer's own faction in it and anyone inside
  the cone's start. Raiders never jet with a melee attacker on them (`Verb_SpewFire`'s guard).
- **Absorb:** player-only; raiders never cast it.

## Gizmo art (requested from the artist)

Paths follow Shipcracker's `UI/Abilities/Warcasket<Set>/` convention, 64x64 like vanilla
command icons:

| Gizmo | Piece | Path |
| --- | --- | --- |
| Vent Tank (toggle) | Armor | `Textures/UI/Abilities/WarcasketToxin/VentTank.png` |
| Absorb Gas | Helmet | `Textures/UI/Abilities/WarcasketToxin/AbsorbGas.png` |
| Gas Jet | Shoulders | `Textures/UI/Abilities/WarcasketToxin/GasJet.png` |

The tank's reload uses the vanilla reload command. Until the art arrives, the defs point at the
tox pack's item texture (`Things/Pawn/Humanlike/Apparel/ToxPack/ToxPack`, Biotech; the pack has
no dedicated command icon) as a placeholder.

## What shipped

All three phases of the original plan (the rebalance and Biotech switch, the tank and passives,
the abilities) have landed. Where each part lives:

- **Defs** (`1.6/Defs/`, one def per file): the three pieces under `ThingDefs_Misc/`
  (`Warcasket_Toxin`, `WarcasketShoulders_Toxin`, `WarcasketHelmet_Toxin`; the jet's verb and
  nozzle comp live on the shoulders), `AbilityDefs/VentTank.xml` and `AbsorbGas.xml`,
  `EffecterDefs/GasJetSpray.xml`, `FleckDefs/AbsorbedGas.xml`.
- **Code** (`Source/1.6/`): `Comps/CompToxTank.cs` and `CompProperties_ToxTank.cs` (tank,
  bursts, rupture, vent emission, vent AI), `Comps/CompJetNozzle.cs` (jet fuel and jet AI),
  `Comps/CompDevResetCooldowns.cs`, `Abilities/Ability_VentTank.cs`,
  `Abilities/Ability_AbsorbGas.cs`, `Verbs/Verb_SprayGas.cs`, `Verbs/GasSpreadEstimate.cs`,
  `Gizmos/Gizmo_ToxTank.cs`, `Settings/ToxinWarcasketMod.cs`, `ToxinWarcasketSettings.cs` and
  `Tunable.cs`, `ModInit.cs`.
- **Patches:** `Source/1.6/Patches/CompApparelReloadable_Reload.cs` (the tank's reload target,
  the nozzle's no-reload-with-a-tank).
- **Languages:** `1.6/Languages/English/Keyed/ToxinWarcasket.xml` (the settings window and the
  jet's tank reason). The defs' own text is DefInjected; the sidecar
  `Scripts/expected-injections.json` is its key set.
- **Research:** `Docs/Research/gas-grid-sim.py`, the open-field cloud simulator the vent rate
  and the targeter's drift area were sized with.

## Edge cases and test list

- **Partial sets and the burst:** the armor alone gives 0.15 environment resistance, so a downed
  pilot in a lone armor lies in their own full-tank cloud taking about 85% buildup. That is the
  cost of not wearing the helmet; check in game that it reads as fair, since a downed pawn cannot
  crawl out.
- Downed by anesthetic (any surgery, VFEP removal): no burst.
- Downed, rescued, tank refilled, downed again: second burst. Downed again with fewer than 5
  charges: nothing.
- Killed outright: burst + pollution + filth. Killed while downed after a burst: pollution and
  filth only.
- Death in a caravan, a transport pod or a holding platform: nothing.
- Pollution on unpollutable cells (water? floors?): `CanPollute` decides; filth still lands.
- Vent while walking through a door: gas follows; closed doors block overflow.
- Gauge target below the fill: no reload job, and the float menu offers none; target raised:
  colonists carry exactly the difference.
- Absorb with no armor: clears gas, refills nothing, no error. Absorb at a full tank: clears
  gas, excess lost.
- Absorb cast mid-vent: the vent closes. Vent opened mid-absorb: the draw ends, its cooldown
  stands.
- Autocast on, pilot walks into a thin edge of a cloud: the absorb fires; the toggle survives a
  save and a re-equip.
- Jet with a tank worn at 9 charges: disabled with the tank reason, the nozzle's own jets
  untouched and no reload job for them, even if they were spent earlier without the tank.
- Jet with the shoulders alone: spends an own jet, reloaded with 20 chemfuel; both own jets
  empty: disabled with vanilla's reload reason.
- Save and load mid-vent, mid-absorb, and mid-jet warmup on both fuel routes.
- Allies: jet beside a friendly does not gas them unless they are inside the cone; the AI jet
  skips any group with an affected friendly in it.
- Settings: capacity lowered below the fill drops the excess next tick; jets without a tank
  cannot go below 1, so the shoulders always work alone.
- Raiders: vent AI fires near player pawns; jet AI fires at the densest group; dead raiders
  rupture.
- Mechs: immune to all of it (`ToxicResistance` 1). Expected; noted in the Workshop FAQ.

## Open questions

None outstanding. The vent rate settled at 2 charges a second after the open-field simulation
(see Vent Tank); it and every other number above are mod settings, so later tuning is a slider
first and a def change second.
