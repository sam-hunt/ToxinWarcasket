# German glossary, Toxin Warcasket

Seeded from the Shipcracker Warcasket glossary (machine-assisted there,
pending native review); no translation pass has run here yet. Only
mod-specific coinages and grounding decisions; family-wide mechanics and
style live in `l10n/languages/German.md`.

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
| unit (the tank's `chargeNoun`; one chemfuel, a volume the vent drains) | *pass* | Shipcracker's "fuel unit" is `Treibstoffeinheit`, whose line maps it to `Treibstoffeinheiten` (`Keine Treibstoffeinheiten mehr.`). A bare `Einheit` would also need a line (`Einheit;Einheiten`). |
| jet (the shoulders' `chargeNoun`; one per activation) | *pass* | Needs its own `plural.txt` line. The ability label "spray gas" and the settings' "Gas jet" section must use the same noun. |

## To ground in the first pass

- cataphract (set name, lore paragraph 3): Royalty `Apparel_ArmorCataphract.label` (Kataphrakt-Rüstung family; Royalty's research story says "Kataphraktrüstungen"), hyphenated per the compound rule.
- hazard (set name, settings' vacuum toggle and Workshop FAQ): coin per the compound rule.
- toxin (this set's name) and the Workshop title.
- the 7th-generation paragraph: no community pack to mirror; follow the meaning, not VFEP's dashes.
- pilot (the wearer): Core and Odyssey German for "pilot".
- Biotech: tox gas, gas mask, tox pack, pollution, the `ToxGasExposure` hediff label and stages; Core: toxic environment resistance, toxic fallout, rot stink, toxic buildup.
- This mod's coinages: tox gas tank, vent (noun and verb), absorb, gas jet, reserve jet, spray gas.

## Pending native review

Inherited from the Shipcracker glossary, still open there: Kriegssarg for
"warcasket" (rejected: Kriegssarkophag, Kriegskapsel, Panzersarg, Kampfsarg,
Kriegsrüstung, the English loan), Kriegssarg-Gießerei, einsargen,
Schulterpanzer, the set names and 'Raumfahrt-Kriegssärge'. Add this mod's own
coinages here as the pass makes them.
