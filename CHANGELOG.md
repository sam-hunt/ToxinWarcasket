# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- Entries are short one-liners ready to paste as Steam Workshop change notes: what changed for the player, no rationale, no def names, no sub-bullets. -->

<!-- Convention: unreleased changes accumulate under the [Unreleased] heading at the top,
     directly below this intro, so a merged contribution is recorded where the release
     flow finds it. Each release renames that heading to its version and date and adds a
     link reference at the bottom of the file. The release workflow lifts the tagged
     version's section into the GitHub release body verbatim and FAILS a stable release
     if the section is missing, so write the section before tagging. Release candidates
     (X.Y.Z-rc.N) get no section: their changes land in the stable version's section at
     promotion. The /release skill walks through all of this. -->

## [Unreleased]

### Added

- Combat Extended support: CE armor values, bulk and carry capacity in line with VFE Pirates' own warcaskets at either tier, and a helmet sealed against CE smoke.

### Changed

- Smaller download: the shipped XML no longer carries developer comments, and every image is recompressed without any change to the pixels.
- American spelling throughout, matching the base game.

## [1.2.0] - 2026-10-22

### Added

- Optional spacer tier setting: rebuilds the set as a 10th generation spacer warcasket with spacer plating, a shield bubble, vacuum resistance and spacer costs (restart required).
- Optional vacuum resistance for the toxin set alone, beside the existing one for the Hazard set.
- Aiming time slider for the gas jet.
- Settings section headers show their ability's description as a tooltip.
- The new settings are translated into all 9 languages.

### Fixed

- Mod settings, the spacer tier text and the Waster support no longer revert after switching language mid-game.
- Settings toggles held by a missing mod or by the spacer tier now show the value actually in effect.
- Coloured notes in settings tooltips keep their colour.

## [1.1.0] - 2026-10-17

### Added

- Vanilla Races Expanded - Waster support: a tank with gas in it counts as tox gas for the tox absorption gene.

## [1.0.0] - 2026-10-17

First release.

### Added

- Toxin warcasket set: armor, shoulder pads, helmet, sharing one tox gas tank refilled with chemfuel.
- Vent the tank to flood the area around the pilot with tox gas.
- Gas jets spray a cone of tox gas from the tank, with a few charges of their own when worn without it.
- Respirator helmet: immune to tox gas, and absorbs nearby gases back into the tank.
- The tank bursts when the pilot is downed and ruptures on death.
- Mod settings for the gas yield, tank, jets and absorb, plus an optional vacuum resistance bump for VFE Pirates' Hazard set.
- Rarely worn by raiders.
- Save Our Ship 2 support (confirmed on 1.6; SOS2 itself does not start cleanly on 1.7 yet).
- RimWorld 1.6 and 1.7 support.
- Translated into 9 languages.

[Unreleased]: https://github.com/sam-hunt/ToxinWarcasket/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/sam-hunt/ToxinWarcasket/releases/tag/v1.2.0
[1.1.0]: https://github.com/sam-hunt/ToxinWarcasket/releases/tag/v1.1.0
[1.0.0]: https://github.com/sam-hunt/ToxinWarcasket/releases/tag/v1.0.0
