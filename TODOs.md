# TODOs

- Gas Jet cast time mod setting
- Evaluate balance of spacer upgrade welding costs
- Split out the mod settings ui per section similar to our other mods' pattern

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
