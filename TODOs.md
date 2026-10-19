# TODOs

- Test in game, as the smoke test cannot vary settings: the vacuum settings under Odyssey in
  the four combinations, neither, toxin vacuum only, spacer tier only, both, plus one run with
  Vanilla Gravship Expanded. Each piece's stat card shows one Vacuum Resistance line at the
  spacer values (0.2 / 0.1 / 0.7; 0.22 / 0.1 / 0.65 under VGE) whenever either setting is on,
  and none when neither is. In the window, the toxin vacuum row greys out and reads as held
  on the moment the spacer box is ticked, and shows the saved choice again when it is unticked.
- The vacuum resistance clause in the spacer-tier toxin warcaskets mod setting should
  also succinctly note in parenthesis that the stat requires Odyssey and should be
  colorized in its idiomatic purple
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
