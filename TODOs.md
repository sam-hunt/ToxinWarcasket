# TODOs

- Review the spacer tier setting's player-facing text before release: the Keyed strings
  (`TXWC_SpacerTier*` in `1.6/Languages/English/Keyed/ToxinWarcasket.xml`), the Workshop
  description's settings and FAQ lines, the README's tier wording, and whether the hazard
  vacuum tooltip's "setting it further apart from the toxin warcasket" still reads right
  when both settings are on.
- Re-apply the settings after an in-session language switch. `LanguageDatabase.SelectLanguage`
  reloads all play data, but `StaticConstructorOnStartupUtility.CallAll` runs class
  constructors through `RuntimeHelpers.RunClassConstructor`, a no-op the second time, so
  `ModInit` never re-runs: the patches re-apply to the fresh defs, but `ApplyToDefs` (every
  slider and the waster feed) and `ApplySpacerText` do not, and the set runs on its XML
  defaults and 7th-gen text until a restart. Fix shape: hook a per-load point (a Harmony
  postfix on `CallAll`, or VEF's/vanilla's post-load callbacks) and move the defaults
  snapshot off the first-load path so a re-run cannot read values already overwritten.
  Check the sibling mods first; some already carry a per-load re-apply pattern worth
  reusing rather than inventing a third.
