# TODOs

## Test in game

The smoke test cannot vary settings, so these need a manual run.

- **Vacuum settings under Odyssey:** all four combinations, plus one run with Vanilla Gravship
  Expanded (VGE).

  | Spacer tier | Toxin vacuum | Each piece's stat card              |
  |-------------|--------------|-------------------------------------|
  | off         | off          | no Vacuum Resistance line           |
  | off         | on           | one line, spacer values             |
  | on          | off          | one line, spacer values             |
  | on          | on           | one line, spacer values             |

  | Spacer values | Armor | Shoulders | Helmet |
  |---------------|-------|-----------|--------|
  | Odyssey       | 0.2   | 0.1       | 0.7    |
  | + VGE         | 0.22  | 0.1       | 0.65   |

  In the settings window, the toxin vacuum row:
  - greys out and reads as held on as soon as the spacer box is ticked;
  - shows the saved choice again once it is unticked.

- **Spacer tier tooltip, Odyssey note** (after the vacuum resistance clause):
  - reads in purple;
  - sits right in the sentence;
  - shows the same with and without Odyssey active.

## Before release

- **Review the spacer tier's player-facing text:**
  - Item text: Keyed `TXWC_SpacerShortDescription_*` and `TXWC_SpacerLore*`
  - Workshop description: the settings and FAQ lines
  - README: the tier wording

## Bugs

- **Settings stop applying after an in-session language switch** (until a restart).
  - **Cause:** `LanguageDatabase.SelectLanguage` reloads all play data, but
    `StaticConstructorOnStartupUtility.CallAll` runs class constructors through
    `RuntimeHelpers.RunClassConstructor`, a no-op the second time, so `ModInit` never re-runs.
  - **Effect:** the patches re-apply to the fresh defs, but `ApplyToDefs` (every slider, the
    waster feed) and `ApplySpacerText` do not, so the set runs on its XML defaults and
    7th-gen text.
  - **Fix:** hook a per-load point (a Harmony postfix on `CallAll`, or VEF's/vanilla's
    post-load callbacks), and move the defaults snapshot off the first-load path so a re-run
    cannot read values already overwritten.
  - **First:** check the sibling mods; some already carry a per-load re-apply pattern worth
    reusing rather than inventing a third.
