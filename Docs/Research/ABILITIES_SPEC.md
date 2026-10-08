# Toxin Warcasket Abilities Spec

The implementation spec for the set's tox gas mechanics and the rebalance that comes with them.
Not mod content. Design decided 2026-10-08; engine facts below were read that day from game
build `1.6.4871 rev591` (`Assembly-CSharp.dll` via `ilspycmd`, `Data/Core` and `Data/Biotech`
XML), VFEP and VEF at the checkouts under `../VanillaExpanded/`. `TOX_GAS_MECHANICS.md` is the
engine map this builds on; `VFEP_WARCASKET_STATS.md` holds the roster numbers quoted here.

Once a phase lands, the def headers and code comments become the record of what shipped (per
CLAUDE.md); this file stays as the design rationale and is not kept in sync with tuning.

## Decisions

| Topic | Decision |
| --- | --- |
| Biotech | **Hard dependency.** Every emitter needs it (the gas grid refuses ToxGas, pollution is Biotech-only), so a Biotech-less set would be Cataphract plating at a Specialised price. The drafted `Mods/Biotech` load-folder gate is dropped; all content lives in the main tree. |
| Tier | **Specialised (7th generation)**, beside Hazard: `VFEP_SpecialisedWarcaskets` **plus Biotech's `ToxGas`** (600, Industrial; the project that gates the tox pack and tox grenades). Vanilla `BuildableDef.IsResearchFinished` requires every listed prerequisite and VFEP's foundry filters its menu with it, so the second entry works with no code. VFEP's 7th-gen lore is "work better as a set" and "first to include plasteel", which a three-piece shared tank is. No 5th-gen piece carries any comp. Spacer (10th gen) is wrong: shields, vacuum, spacer components. |
| Plating, mass, speed | Keep the Cataphract's (Sharp 1.56 / Blunt 0.65 / Heat 0.78, 118 kg, MoveSpeed -1.35). Hazard also trades plating for function (Sharp 1.20); the slow chassis is the advancing gas cloud's identity. |
| Toxin stats | **Environment only.** Drop `ToxicResistance` entirely; `ToxicEnvironmentResistance` armor 0.15 / shoulders 0.15 / helmet 0.8 (sum 1.1, clamps to the stat's max of 1.0); helmet keeps `immuneToToxGasExposure`. |
| Costs | Rebased to 7th-gen bands, plus the tank priced onto the armor (table below). |
| Raider tags | **`WarcasketSpecialised`, `WarcasketAll`**, replacing the Cataphract's `WarcasketHeavy`/`WarcasketCata` (reasoning below). |
| Vent rate | 10% of the tank per second for now, emitted one charge at a time (see Vent Tank); may slow slightly after play. |
| Absorb while moving | Allowed: the absorb disc follows the pilot. |

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
`WarcasketAll` is the mercenary `VFEP_General`, so raiders in the set become rarer than today
(the Cataphract tags put it on `VFEP_Major`, `VFEP_HeavyWeaponsPlatform` and `VFEP_Artillery`).
That suits a 7th-gen set; a dedicated tox-trooper pawn kind would be the way to raise its
presence later.

### Costs

| Piece | Today (Cataphract + filter) | New |
| --- | --- | --- |
| Armor | Comp 5, Steel 175, Uranium 50 | Comp 8, Steel 150, Plasteel 20, Uranium 50, **Chemfuel 100** |
| Shoulders | Steel 75 | Comp 2, Steel 55, Plasteel 20 |
| Helmet | Comp 2, Steel 75, Chemfuel 20 | Comp 3, Steel 45, Plasteel 20, Chemfuel 20 |
| Set | Comp 7, Steel 325, Uranium 50, Chemfuel 20 | Comp 13, Steel 250, Plasteel 60, Uranium 50, Chemfuel 120 |

The armor's Chemfuel 100 pays for the full tank it is built with
(`CompApparelVerbOwner_Charged.PostPostMake` fills charges on creation). The shoulders' Comp 2
is the jet nozzle (Controller's shoulders pay components for their verb). For comparison, the
other 7th-gen sets total Comp 11 to 14, Steel 220 to 360, Plasteel 65 to 75, Uranium 50.

## The tank (armor)

Every emitter draws on one reagent tank on the armor.

- **Class:** `CompToxTank : CompApparelReloadable` (props
  `CompProperties_ToxTank : CompProperties_ApparelReloadable`). `remainingCharges` is
  `protected` on `CompApparelVerbOwner_Charged`, so the subclass adds `TryConsume(int)`,
  `Add(int)` (capped at `MaxCharges`) and `Empty()` without reflection. Vanilla reload jobs, the
  "reload" float menu and save/load come from the base class.
- **Units:** `maxCharges` 100, one charge = 255 gas units = one cell at full density. The full
  tank is 100 cells, about 2.2 tox packs (the pack fills 45 cells). The vent shows it as a
  percentage.
- **Refill:** `ammoDef` Chemfuel, `ammoCountPerCharge` 1 (100 chemfuel per full tank, Aerial's
  and Shock's figure), `baseReloadTicks` 60, `chargeNoun` "reagent". Tox pack comparison:
  35 chemfuel for 45 cells; ours costs slightly more per cell but is reusable and refills from
  the helmet.
- **Readout:** the vent gizmo's extra label carries `RemainingCharges` (Shipcracker's
  `Ability_BreachJump.GetGizmo` pattern for a cached command with a live label).
- **Ticking:** the armor already resolves to `tickerType` Normal via `VFEP_WarcasketArmorBase`.

### Passive: downed burst

When the wearer goes down with the armor on, the whole tank vents at once as a gas explosion,
sized by what was in it.

- **Trigger:** `CompToxTank.CompTick` watches `Wearer.Downed` for a false-to-true transition
  (no apparel-side downed notification exists: `Notify_Downed` lives on `Hediff` and `Pawn`
  only). A Harmony postfix on `Pawn_HealthTracker.MakeDowned` is the alternative if the cause
  (`dinfo`) is needed; polling is enough and keeps the mod patch-free.
- **Skip when:** the wearer is unspawned, the tank holds fewer than 5 charges, or the pawn
  carries the `Anesthetic` hediff. The anesthetic guard is what stops a medical operation (VFEP's
  warcasket removal surgery among them) from gassing the hospital. A pawn that is downed, stood
  up and downed again bursts again only if the tank was refilled in between.
- **Effect:** `GenExplosion.DoExplosion(position, map, radius, DamageDefOf.ToxGas, wearer,
  postExplosionGasType: GasType.ToxGas, postExplosionGasAmount: 255)` with
  `radius = GenRadial.RadiusOfNumCells(charges)`: a full tank gives radius about 5.6. `ToxGas`
  is the tox grenade's harmless damage def (`defaultDamage` 0, Biotech), so the explosion
  carries the visuals and sound and the gas does the work. Explosions skip cells without line
  of sight from the centre, so walls shape the cloud. Then `Empty()`.

### Passive: death rupture

When the wearer dies with the armor on, the canisters burst and foul the ground. Layout follows
the reference screenshot: a blast mark under the corpse, chemical slime, spent acid and bile
splashed around it.

- **Trigger:** `ThingComp.Notify_WearerDied`, called from `Apparel.Notify_PawnKilled`.
  `Pawn.Kill` runs `PreDeathPawnModifications` (which notifies apparel) **before**
  `DeSpawnOrDeselect`, so `Wearer.Position` and `Wearer.Map` are still valid. Skip if unspawned
  (caravans, transporters, holding containers).
- **Burst:** if the tank still holds 5 or more charges (killed outright rather than after a
  downed burst), fire the downed burst first.
- **Pollution:** every cell within the rupture radius where `map.pollutionGrid.CanPollute(cell)`
  gets `SetPolluted(cell, true)`. **Radius note:** 1.9 covers **9** cells, not 13
  (`GenRadial` counts by `x² + z² <= r²`; distance 2 is excluded). 13 cells needs radius 2.0;
  the spec uses **2.0** to match the 13-tile intent. `SetPolluted` calls
  `ModLister.CheckBiotech`, fine now that Biotech is required.
- **Filth (all Core defs):** `Filth_BlastMark` on the centre cell; every other cell in the
  radius has a 70% chance of one of `Filth_Slime`, `Filth_SpentAcid`, `Filth_CorpseBile`
  (equal weights), via `FilthMaker.TryMakeFilth`. A second roll at 30% adds a second filth of a
  different kind for the layered look in the screenshot. Chances are tuning knobs on the props.
- Raiders in the set rupture too: each dead Toxin raider leaves 13 polluted cells on the
  player's map, which Biotech's clear-pollution work cleans. Intended flavor, and the answer to
  `TODOs.md`'s "make a raider in it interesting".

### Ability: Vent Tank (armor)

A toggle that empties the tank into a growing cloud around the pilot, who keeps moving.

- **Route:** VEF `CompAbilitiesApparel` on the armor granting `TXWC_VentTank`; a custom
  `Ability_VentTank : VEF.Abilities.Ability` that returns a `Command_Toggle` from `GetGizmo`
  (VFEP's `CommandAbilityToggle` is the precedent; write our own rather than depend on a VFEP
  type for a toggle). The ability only flips `CompToxTank.venting`; the tank comp ticks the
  emission, so state lives in one saved place.
- **Emission:** while `venting`, one charge at a time: every `ticksPerCharge` ticks (props
  field, **6**), `GasUtility.AddGas(wearer.Position, map, GasType.ToxGas, 255)`. That is **10% of
  the tank per second, a full tank in 10 s**, in small steps so the cloud grows smoothly and a
  toggle-off wastes at most one charge. Slowing it is a one-number change: 7 gives about 8.6%/s,
  8 gives 7.5%/s. `GasGrid.AddGas` clamps the cell to 255 and floods the excess outward
  (`Overflow`, flood fill up to radius 40, blocked by walls and closed doors), so the cloud
  grows from wherever the pilot stands and trails them when they walk.
- **Stops when:** toggled off, the tank empties, the wearer is downed (the burst takes over),
  dies, or despawns. Toggling off keeps what is left.
- **Visuals:** Core's `ToxGasReleasing` effecter on the wearer, the tox pack's.
- **Availability:** drafted only, 1+ charge to start, no cooldown (fuel is the limiter, as on
  Aerial and Shock).

## Ability: Absorb Gas (helmet)

The respirator inhales the air around the pilot and banks it in the tank.

- **Route:** VEF `CompAbilitiesApparel` on the helmet granting `TXWC_AbsorbGas`, self-cast,
  `cooldownTime` 2500 ticks (one in-game hour). The cast applies hediff `TXWC_AbsorbingGas`
  (`HediffComp_Disappears` 300 ticks) carrying `HediffComp_AbsorbGas`. The hediff ticks with the
  pawn, saves for free, and leaves the helmet at `tickerType` Never.
- **Effect:** every 30 ticks (10 pulses over 5 s) for each cell within radius **2.9** (25
  cells) of the pawn's current position, remove up to 64 units from **each** gas byte
  (smoke, tox, rot stink, deadlife). Four pulses clear a full cell; the remaining six catch gas
  diffusing back in. Write with `GasGrid.SetDirect(index, smoke, toxic, rotStink, deadlife)`
  and call `map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Gas)` afterwards (`AddGas` does
  the same; `SetDirect` alone does not redraw). `AddGas` cannot subtract (it returns on
  `amount <= 0`).
- **Refill:** units absorbed, summed across gas types, convert at 255 units to 1 charge into
  the armor's `CompToxTank`, if worn; the remainder carries over between pulses in the hediff.
  The tank caps at 100 and excess is lost. Without the armor, the ability still clears the air.
  Smoke and rot stink become tox reagent by design: the filter is the conversion.
- **Uses:** clears enemy tox grenades, tox shells, toxalope clouds or smoke off allies; recovers
  your own vented cloud; refills mid-fight. A saturated 2.9 disc is 25 charges (a quarter tank).
- **Availability:** drafted and undrafted (useful for cleanup), no fuel cost.

## Ability: Gas Jet (shoulders)

The tox gas version of the warcasket heavy flamer: a directional cone, safer near allies than
the vent.

- **Route:** a vanilla apparel verb on the shoulders, as the Barrage shoulders do. The verb
  ticks through the comp's verb tracker, so the shoulders need `tickerType` Normal (Barrage's
  shoulders redeclare it for the same reason; the shoulder base leaves it at Never).
- **Verb:** `Verb_SprayGas : Verb`, modelled on vanilla `Verb_SpewFire` (Biotech scorcher's
  mini-flameblaster). `Verb_SpewFire` already computes a ±13° arc toward the target and calls
  `GenExplosion.DoExplosion` with `affectedAngle`, but hardcodes `Flame`; ours passes
  `DamageDefOf.ToxGas`, `postExplosionGasType: GasType.ToxGas`, `postExplosionGasAmount: 255`,
  `propagationSpeed` 0.6. `ExplosionCellsToHit` skips cells within 0.5 of the origin and cells
  without line of sight, so the pilot's own cell and everything behind them stay clear.
- **Numbers:** range 11.9, half-angle 15° (a props field), warmup 1.0 s. That cone is about 32
  cells, matching the 30 charges it costs, so jet and vent spend the tank at the same gas per
  charge.
- **Fuel:** `CompJetNozzle : CompApparelReloadable`, `maxCharges` 1, `ammoDef` Chemfuel,
  `ammoCountPerCharge` 30 (the same 30 chemfuel the tank spends per jet).
  - `CanBeUsed`: true if the worn armor's tank holds 30+ charges **or** the nozzle holds its
    charge.
  - `UsedOnce`: draws 30 from the tank when it can, otherwise spends the nozzle's own charge.
    The nozzle is the reserve shot when the tank runs dry and the whole fuel source when the
    shoulders are worn alone.
  - `GizmoExtraLabel`: tank percent plus the reserve, e.g. `64% +1`.
- **Visuals:** needs a tox spray effecter; vanilla's `Fire_SpewShort` is fire-coloured. First
  pass: an `EffecterDef` throwing `Fleck_ToxGasSmall` along the line; the artist's effect can
  replace it.

## AI

The passives (downed burst, death rupture) work for raiders with no AI code. The actives:

- **Vent:** an opportunistic check in `CompToxTank.CompTick` for non-player pawns, modelled on
  `CompToxPack.ChanceToUse`: every 60 ticks, if the tank holds 20+ charges and hostile pawns
  within 4 cells that `GasUtility.IsAffectedByExposure` sum to body size 1 or more, start
  venting; stop when none remain for 5 s. Raiders then drive a moving cloud into the player's
  line.
- **Gas jet:** verify whether the vanilla AI picks apparel verbs as attack verbs (it does for
  Barrage's grenade verb if VFEP raiders throw grenades in game). If not, a `CompAIUsablePack`
  subclass on the shoulders aims at the densest group in range.
- **Absorb:** player-only in the first pass.

## Gizmo art (requested from the artist)

Paths follow Shipcracker's `UI/Abilities/Warcasket<Set>/` convention, 64x64 like vanilla
command icons:

| Gizmo | Piece | Path |
| --- | --- | --- |
| Vent Tank (toggle) | Armor | `Textures/UI/Abilities/WarcasketToxin/VentTank.png` |
| Absorb Gas | Helmet | `Textures/UI/Abilities/WarcasketToxin/AbsorbGas.png` |
| Gas Jet | Shoulders | `Textures/UI/Abilities/WarcasketToxin/GasJet.png` |

The tank's reload uses the vanilla reload command. Until the art arrives, point the defs at
the tox pack's item texture (`Things/Pawn/Humanlike/Apparel/ToxPack/ToxPack`, Biotech; the pack
has no dedicated command icon) as a placeholder.

## Implementation plan

### Phase 1: rebalance and the Biotech switch (XML and docs, no C#)

- `About/About.xml`: add `ludeon.rimworld.biotech` to `modDependencies` (it is already in
  `loadAfter`); rewrite the description's Cataphract and toxin-resistance wording.
- `LoadFolders.xml`: delete the drafted Biotech gate and its comment.
- All three defs: research to `VFEP_SpecialisedWarcaskets` + `ToxGas`; tags to
  `WarcasketSpecialised` + `WarcasketAll`; the toxin stat split above; the new costs;
  descriptions rewritten into VFEP's 7th-generation framing (Hazard's description is the
  template paragraph); headers rewritten (they cite 5th gen, the Cataphract's research and tags,
  and the Biotech gate).
- `README.md` and `.steamworkshop/Description/English.txt`: Biotech required, new numbers.
- `Scripts/*.py` shim comments ("Biotech is the planned compat gate", "no DLC is
  hard-required"); the pins stay, the reasons change.
- `.claude/skills/translate/SKILL.md` and `.claude/skills/release/SKILL.md`: drop the
  compat-root and "root opens once the ability lands" notes.
- `CLAUDE.md`: the Biotech section becomes a short "required, and why" note; the
  Localization and Optional-Content Gating section stays as the general rule for any future
  optional mod.
- Regenerate `Scripts/expected-injections.json` (descriptions change).

### Phase 2: the tank and the passives (first C#)

`CompToxTank` with `CompProperties_ToxTank` (tank, downed burst, death rupture, vent emission
and AI vent check), a `[DefOf]` class for the filth and hediff defs. First C# use of VEF types
comes in phase 3; this phase uses vanilla types only.

### Phase 3: the abilities

- Defs, one per file: `AbilityDefs/VentTank.xml`, `AbilityDefs/AbsorbGas.xml`,
  `HediffDefs/AbsorbingGas.xml`, `EffecterDefs/GasJetSpray.xml`; the shoulders' verb lives on
  the shoulders def.
- Code: `Ability_VentTank` + its toggle command, `HediffComp_AbsorbGas` (+ props),
  `Verb_SprayGas`, `CompJetNozzle` (+ props), and the AI jet comp if needed.
- Wiring: `CompAbilitiesApparel` on armor and helmet; `CompJetNozzle` + verb + `tickerType`
  Normal on the shoulders. Armor `ApparelExtension` needs no change.
- Labels and descriptions for the new defs, then regenerate the l10n sidecar.
- CI: VEF and VFEP DLLs already come from SteamCMD in `release.yml`.

## Edge cases and test list

- **Partial sets and the burst:** the armor alone gives 0.15 environment resistance, so a downed
  pilot in a lone armor lies in their own full-tank cloud taking about 85% buildup. That is the
  cost of not wearing the helmet; check in game that it reads as fair, since a downed pawn cannot
  crawl out.
- Downed by anesthetic (any surgery, VFEP removal): no burst.
- Downed, rescued, tank refilled, downed again: second burst. Downed again with an empty tank:
  nothing.
- Killed outright: burst + pollution + filth. Killed while downed after a burst: pollution and
  filth only.
- Death in a caravan, a transport pod or a holding platform: nothing.
- Pollution on unpollutable cells (water? floors?): `CanPollute` decides; filth still lands.
- Vent while walking through a door: gas follows; closed doors block overflow.
- Absorb with no armor: clears gas, refills nothing, no error.
- Absorb at a full tank: clears gas, excess lost.
- Jet with the tank at 29 charges and the nozzle loaded: spends the nozzle. With both empty:
  disabled with a reason.
- Save and load mid-vent, mid-absorb, mid-jet warmup.
- Allies: jet beside a friendly does not gas them unless they are inside the cone.
- Raiders: vent AI fires near player pawns; dead raiders rupture.
- Mechs: immune to all of it (`ToxicResistance` 1). Expected; noted in the Workshop FAQ.

## Open questions

None outstanding; the vent rate may be tuned down after play (see Vent Tank).
