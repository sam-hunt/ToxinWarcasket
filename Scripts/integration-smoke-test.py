#!/usr/bin/env python3
# Pre-release integration smoke test: boots the real game once with Toxin
# Warcasket, its dependency chain and the optional mods it patches for, on
# a pinned minimal list where the baseline is a clean log, then
# classifies every logged error/warning by origin and fails on anything
# attributed to this mod or an integration seam. Thin shim over the shared
# engine in l10n/smoke/startup_smoke.py (see its header for mechanics and the
# BetterTradersGuild v1.1.0 CWTL incident this exists to catch).
#
# Run this before every release, with the game closed:
#   python3 Scripts/integration-smoke-test.py              # boot + scan
#   python3 Scripts/integration-smoke-test.py --no-launch  # rescan last log
#   python3 Scripts/integration-smoke-test.py --strict     # any error fails
#   python3 Scripts/integration-smoke-test.py --spacer-tier  # spacer tier on

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "smoke"))
import startup_smoke as engine  # noqa: E402

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

engine.PACKAGE_ID = "shunter.toxinwarcasket"

# RATIONALE: the first five entries are the l10n CANONICAL_ACTIVE_MODS (the
# refresh shim explains them: VFE Pirates is the hard dep, VEF and Harmony
# are its deps, Biotech is the other hard dep). Then the two optional mods
# this repo integrates with. Vanilla Races Expanded - Waster: the tank
# feeds its tox absorption gene through names resolved by reflection
# (Source/1.6/Integrations/VREWasterIntegration.cs), which warns at
# startup when VREW is active but a name has drifted, a warning only a
# boot with VREW active can surface; it needs only VEF and Biotech, both
# above it. It is NOT on the refresh shim's list: it patches none of our
# defs, so it never changes the sidecar's key set. Combat Extended:
# 1.6/Patches/CombatExtended.xml fires on its package id and SpacerTier.xml
# skips its plating, shield and cost operations under it; both branch on the
# spacer tier setting, and a failed operation or a misnamed setting is an
# error only a boot with CE active can surface. The engine boots on default
# settings (spacer tier off), so boot once plain and once with
# --spacer-tier, which forces the setting on through
# Scripts/spacer_tier_boot.py (the setting is a restart-time patch gate,
# so it needs its own boot). CE loads after VFEP, where
# its own About.xml puts it. Probe last (auto-quit). Plain ids on purpose;
# see the refresh shim's "_steam" note.
# Save Our Ship 2 is deliberately absent: 1.6/Patches/SOS2Patch.xml is a
# best-effort mirror of VFEP's own patch (its header), checked statically by
# Tests/1.6/CompatPatchTests.cs and never booted here.
engine.SMOKE_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "ludeon.rimworld.biotech",
    "oskarpotocki.vanillafactionsexpanded.core",
    "oskarpotocki.vfe.pirates",
    "vanillaracesexpanded.waster",
    "ceteam.combatextended",
    "shunter.toxinwarcasket",
    "shunter.l10nprobe",
]

# Assembly/namespace name, the C# log prefix (also the About.xml display
# name, which the engine derives and appends itself), and the def prefix.
engine.OWN_PATTERNS = ["ToxinWarcasket", "[Toxin Warcasket]", "TXWC_"]

# The seams: VFEP's warcasket code (foundry, entombing, Apparel_Warcasket)
# walks our WarcasketDefs, VEF's ApparelExtension runs on them, VREW's
# names are what our integration resolves and its hediff comps are what it
# feeds, and CE's armor and inventory code reads the stats
# CombatExtended.xml sets. An error mentioning any of these gates the test
# even when the exception fires inside their code.
engine.INTEGRATION_PATTERNS = {
    "VFEP": ["VFEPirates"],
    "VEF": ["VEF."],
    "VREW": ["VanillaRacesExpandedWaster", "VanillaRacesExpanded-Waster"],
    "CE": ["CombatExtended"],
}

# Our own flag, taken off argv before the engine's argparse sees it.
if "--spacer-tier" in sys.argv:
    sys.argv.remove("--spacer-tier")
    from spacer_tier_boot import force_spacer_tier_on  # noqa: E402

    force_spacer_tier_on()
raise SystemExit(engine.main())
