#!/usr/bin/env python3
# Pre-release integration smoke test: boots the real game once with Toxin
# Warcasket, its dependency chain and the one optional mod it patches for,
# on a pinned minimal list where the baseline is a clean log, then
# classifies every logged error/warning by origin and fails on anything
# attributed to this mod or an integration seam. Thin shim over the shared
# engine in l10n/smoke/startup_smoke.py (see its header for mechanics and the
# BetterTradersGuild v1.1.0 CWTL incident this exists to catch).
#
# Run this before every release, with the game closed:
#   python3 Scripts/integration-smoke-test.py              # boot + scan
#   python3 Scripts/integration-smoke-test.py --no-launch  # rescan last log
#   python3 Scripts/integration-smoke-test.py --strict     # any error fails

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
# defs, so it never changes the sidecar's key set. Save Our Ship 2:
# 1.6/Patches/SOS2Patch.xml fires on its display name (or Universum's) and
# never runs otherwise, and a failed PatchOperation is exactly the kind of
# error only a boot with the mod active can surface. SOS2 hard-requires
# Vehicle Framework, which is here only for that reason. Universum is not
# installed locally, and the patch is the same either way. Probe last
# (auto-quit). Plain ids on purpose; see the refresh shim's "_steam" note.
engine.SMOKE_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "ludeon.rimworld.biotech",
    "oskarpotocki.vanillafactionsexpanded.core",
    "oskarpotocki.vfe.pirates",
    "vanillaracesexpanded.waster",
    "smashphil.vehicleframework",
    "kentington.saveourship2",
    "shunter.toxinwarcasket",
    "shunter.l10nprobe",
]

# Assembly/namespace name, the C# log prefix (also the About.xml display
# name, which the engine derives and appends itself), and the def prefix.
engine.OWN_PATTERNS = ["ToxinWarcasket", "[Toxin Warcasket]", "TXWC_"]

# The seams: VFEP's warcasket code (foundry, entombing, Apparel_Warcasket)
# walks our WarcasketDefs, VEF's ApparelExtension runs on them, VREW's
# names are what our integration resolves and its hediff comps are what it
# feeds, and SOS2 reads the EVA tag our patch adds. An error mentioning any
# of these gates the test even when the exception fires inside their code.
# Vehicle Framework is deliberately absent: it is only SOS2's dep and its
# own noise is third-party.
engine.INTEGRATION_PATTERNS = {
    "VFEP": ["VFEPirates"],
    "VEF": ["VEF."],
    "VREW": ["VanillaRacesExpandedWaster", "VanillaRacesExpanded-Waster"],
    "SOS2": ["SaveOurShip2", "SaveOurShip"],
}

raise SystemExit(engine.main())
