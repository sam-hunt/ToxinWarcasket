# German glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review), then filled by this repo's first machine-assisted
German pass. Only mod-specific coinages and grounding decisions; family-wide
mechanics and style live in `l10n/languages/German.md`.

**No community VFEP German translation could be grounded against.** A
Workshop pack "Vanilla Factions Expanded - Pirates Deutsch" (id 2810180496)
exists, but its page shows none of its terms and its files were not
available. Every VFEP term below is therefore coined in the Shipcracker repo
and inherited here. If that pack's files turn up, re-ground against them in
the Shipcracker glossary first and prefer its renderings.

## Inherited rows (shared with Shipcracker Warcasket; change them there first)

| English | German | Grounding |
|---|---|---|
| warcasket (coined) | Kriegssarg (m.), pl. Kriegssärge | Literal war + coffin, mirroring the English compound and vanilla's war- → Kriegs- pattern (`MeleeWeapon_Zeushammer` = Kriegshammer). Does not collide with Core's coffin furniture (`Sarcophagus` = Sarkophag). Masculine, so the Gender-table miss default is correct. |
| compound rule for Kriegssarg | always hyphenated to a set name or part: `<Set>`-Kriegssarg, Kriegssarg-Helm, Kriegssarg-Hülle, Kriegssarg-Schulterpanzer, Kriegssarg-Gießerei | Durchkopplung once a set prefix is present; kept uniform in descriptions too so labels and prose match. |
| `<set>` warcasket / shoulders / helmet (label pattern) | `<Set>`-Kriegssarg / `<Set>`-Kriegssarg-Schulterpanzer / `<Set>`-Kriegssarg-Helm | VFEP's `<set> warcasket <part>` label pattern. |
| pauldrons / shoulders / shoulder pads | Schulterpanzer | Ordinary German armor term; "Schultern" alone reads as the body part. |
| warcasket shell | Kriegssarg-Hülle | Per the compound rule. |
| warcasket foundry (coined) | Kriegssarg-Gießerei | foundry = Gießerei per Core `FoundryApprentice76.title` = Gießereilehrling. |
| entomb (coined) | einsargen | Real German verb (to put into a coffin), derived directly from Kriegssarg. The Workshop page's "weld" stays einschweißen, as in English. |
| spacer warcaskets (VFEP research, cited in prose) | 'Raumfahrt-Kriegssärge' | Core `TechLevel_Spacer` = Raumfahrt; single-quoted as a cited label per the language file. Render `specialised warcaskets` in the same shape. |
| VFEP set names (coined) | Belagerungsbrecher (siegebreaker), Rohling (brute), Wächter (guardian), Kontrolleur (controller), Sarkophag (sarcophagus), Luft-Kriegssarg (aerial) | Plain renderings, pending native review there. |
| spacer-tech | aus der Raumfahrttechnik | Core `Plasteel.description` / `Synthread.description` render "spacer-tech" exactly so. |
| Imperial | imperial (imperiale, imperialen ...) | Royalty `PawnKinds_Empire` (imperialer Grenadier). |
| respirator / breathing | Atemgerät / Atmen, Atemleistung | Odyssey research story "Atemgeräten"; Core `Breathing` = Atmen. |
| sealed | luftdicht versiegelt | Odyssey `GravshipHull.description` "luftdichte Wand". |
| chemfuel | Sprit | Core's chemfuel label. |
| mod (noun) | die Mod | Core Keyed `ModLoadFolder...` "About.xml der Mod". |
| jobReportString (`Using ability: {0}`) | setzt Fähigkeit '{0}' ein. | Shipcracker's rendering. |
| cooldownGerund (`on cooldown`) | klingt gerade ab | Shipcracker's rendering of the same vanilla comp field. |
| Off / On / default (settings) | Aus / an / Standard | Core Keyed `Off`, `On`, `Default`; "When enabled/disabled" = Wenn aktiviert / Wenn deaktiviert after Core `CommandAllowCutDesc`. |
| cooldown / cells | Abklingzeit / Zellen | Core Keyed `CooldownTime`, `MapSizeDesc`. |
| Odyssey (DLC) | Odyssey | Core `ExpansionDef` `Odyssey.label` keeps the English name. |
| Workshop title | `<Set>`-Kriegssarg, byte-identical to the body label | Contains Kriegssarg. Also the `TXWC_SettingsCategory` value (title coupling). |

## Charge nouns

Both are shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots
(see the skill). The German worker delegates to the English one: lookup in
the `plural` table, else `+s`. Core's German `plural.txt` is thin, so **any
coined noun ships its own line** in `1.6/Languages/German/WordInfo/plural.txt`
(`singular;plural`, lowercase file name); `plural_decline` misses and returns
that plural unchanged.

| English | German | Grounding |
|---|---|---|
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | Spriteinheit (f.), pl. Spriteinheiten; relies on `WordInfo/plural.txt` (`Spriteinheit;Spriteinheiten`) | Coined: chemfuel = Sprit plus Einheit, parallel to Shipcracker's `Treibstoffeinheit`; one unit is one chemfuel, so the stat lines read `Spriteinheiten verbleibend` and `Keine Spriteinheiten mehr.` Feminine but absent from the Gender tables, so `Stat_Thing_ReloadPerCharge_Desc`'s gender switch falls to the masculine default (see Pending native review). |
| jet (the shoulders' `chargeNoun`; one per activation) | Gasstoß (m.), pl. Gasstöße; relies on `WordInfo/plural.txt` (`Gasstoß;Gasstöße`) | Coined: one burst of gas per activation. Masculine, so the Gender-table miss default is correct. Also the settings' "Gas jet" section and every "jet" meaning the shot (the verb label "spray gas" = Gas versprühen); the hardware ("gas jets on the pauldrons", "shoulder-mounted jets") is Gasdüsen / Düsen. |

## Grounded in the first pass

| English | German | Grounding |
|---|---|---|
| toxin (set name) / toxin warcasket | Toxin-Kriegssarg (helmet Toxin-Kriegssarg-Helm, shoulders Toxin-Kriegssarg-Schulterpanzer); the series is die Toxin-Baureihe | Coined: "Toxin" is itself a German word (das Toxin), so the name stays; compound rule as inherited. Also the Workshop title and `TXWC_SettingsCategory`. |
| Cataphract (VFEP set) | Kataphrakt (Kataphrakt-Entwurf, Kataphrakt-Chassis) | Royalty `Empire_Fighter_Cataphract.label` = Kataphrakt, `Apparel_ArmorCataphract.label` = Kataphraktrüstung. |
| Hazard (VFEP set) | Gefahrenschutz (Gefahrenschutz-Kriegssarg, das Gefahrenschutz-Set) | Coined: the hazmat set as "hazard protection"; compound rule. |
| Aerial (VFEP set, Workshop page) | Luft-Kriegssarg | Inherited set name. |
| specialised warcaskets (VFEP research) | 'spezialisierte Kriegssärge' | Same shape as 'Raumfahrt-Kriegssärge'; lowercase adjective start per the language file's research-label rule. "Specialized warcasket shell/helmet" in prose = spezialisierte Kriegssarg-Hülle / spezialisierter Kriegssarg-Helm. |
| 7th generation | 7. Generation (die Kriegssärge der 7. Generation) | Shipcracker's "Raumfahrt-Chassis der 10. Generation". VFEP's dashed aside is reflowed as "die zwar untereinander austauschbar sind, im Set aber besser funktionieren". |
| superior protection and worse weight | mit überlegenem Schutz und höherem Gewicht | Coined, plain description wording. |
| pilot (the wearer) | Pilot | Core/Odyssey (Pilotenkonsole, Pilotenassistent); Shipcracker's prose. |
| tox gas | Giftgas | Biotech `ToxGas.label` (gas, research project and hediff all Giftgas). |
| tox gas exposure (hediff), stages | Giftgas; mild / mittel / schwer; prose "Belastung durch Giftgas" | Biotech `ToxGasExposure.label`, `.stages.*`, `.description`. |
| gas mask | Gasmaske | Biotech `Apparel_GasMask.label`. |
| tox pack | Giftrucksack, pl. Giftrucksäcke | Biotech `Apparel_PackTox.label` (its own chargeNoun is Giftladung). |
| pollution / polluted terrain | Verseuchung / verseuchter Boden | Biotech `PollutedTerrain.label`, `PoluxTreeSpawn.letterText`. |
| toxic environment resistance | Giftresistenz (Umwelt) | Core `ToxicEnvironmentResistance.label`; "environmental toxins" = Umweltgifte from its description. |
| toxic fallout | giftiger Fallout | Core `ToxicFallout.label`. |
| rot stink | Verwesungsgeruch | Core `ToxicEnvironmentResistance.description`. |
| toxic buildup | Vergiftung | Core `ToxicBuildup.label`. |
| deadlife dust (Workshop page) | Deadlife-Staub | Core Keyed `DeadlifeDust`. |
| smoke | Rauch | Core `Smoke.label`. |
| sharp / blunt (armor) | Schnitt / Wucht | Core `ArmorRating_Sharp.label` / `ArmorRating_Blunt.label`. |
| vacuum resistance | Vakuumresistenz (adj. vakuumresistent) | Odyssey `VacuumResistance.label`. |
| Miscellaneous (settings section) | Sonstiges | Coined, ordinary UI word. |
| requires restart (suffix) | (erfordert Neustart) | Parallel to Shipcracker's " (erfordert Odyssey)"; Core `ChangedTextureCompressionRestart` "erfordert einen Neustart". |
| tox gas tank | Giftgastank | Coined: Giftgas + Tank. |
| vent tank (ability) / vent (verb) / vent rate | Tank ablassen / ablassen, das Ablassen / Ablassrate | Coined: ablassen is the ordinary verb for letting gas or liquid out; vanilla's tox pack verb is "Giftgas freisetzen". |
| vents (the hardware) | Ventile | Coined. |
| absorb gas (ability) / the absorb (noun) | Gas absorbieren / die Absorption | Coined; absorbieren per Biotech `AbsorbXenogerm.reportString` and `PoluxSeed.description`. Draw in = ansaugen. |
| gas jet (the shot, the settings section) | Gasstoß | See Charge nouns. |
| gas jets (the hardware on the pauldrons) | Gasdüsen / Düsen | Coined. |
| reserve jet ("Jets without a tank", the shoulders' own jets) | Gasstöße ohne Tank / eigene Gasstöße der Schulterpanzer | Coined from the charge noun. |
| spray gas (the nozzle's `jetLabel`, the jet command's label) | Gas versprühen | Coined; versprühen is also the jet description's verb. |
| cone width / range | Kegelbreite (Grad) / Reichweite (Zellen) | Coined / Shipcracker. |

## Pending native review

Inherited from the Shipcracker glossary, still open there: Kriegssarg for
"warcasket" (rejected: Kriegssarkophag, Kriegskapsel, Panzersarg, Kampfsarg,
Kriegsrüstung, the English loan), Kriegssarg-Gießerei, einsargen,
Schulterpanzer, the set names and 'Raumfahrt-Kriegssärge'.

This repo's first pass (machine-assisted): Toxin-Kriegssarg and die
Toxin-Baureihe; Gefahrenschutz for VFEP's Hazard set (a VFEP set name, so it
belongs in the Shipcracker glossary too once reviewed); 'spezialisierte
Kriegssärge'; Giftgastank; Tank ablassen / Ablassrate; Gas absorbieren / die
Absorption; Gasstoß for both the jet charge and the settings section, with
Gasdüsen for the hardware; Spriteinheit as the tank's charge noun (feminine
but missing from the Gender tables, so the one vanilla gender switch over a
charge noun prints "einen Spriteinheit"; a mod `WordInfo/Gender/Female.txt`
line would fix it, the same gap exists for Shipcracker's Treibstoffeinheit);
the free prose of the three descriptions, the ability and jet descriptions and
the Workshop page.
