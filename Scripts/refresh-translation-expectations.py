#!/usr/bin/env python3
# Toxin Warcasket's config shim over the shared sidecar-refresh engine
# (l10n/refresh/refresh_expectations.py, the rimworld-l10n submodule),
# which drives the L10nProbe dev mod (source at l10n/probe/; build/deploy it
# only from the canonical ~/dev/rimworld-l10n checkout). The engine holds all
# logic; this file holds only this repo's config and the rationale behind it.
# Usage is unchanged (game must be closed):
#   python3 Scripts/refresh-translation-expectations.py [--no-launch]
# If l10n/ is empty, run: git submodule update --init

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "refresh"))
import refresh_expectations as engine  # noqa: E402  (import after sys.path edit)

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

engine.PACKAGE_ID = "shunter.toxinwarcasket"

# RATIONALE: VFE Pirates is the hard dependency (our defs parent on its
# abstract warcasket bases and would not resolve without it); Vanilla
# Expanded Framework and Harmony are VFEP's own hard deps and load before
# it. Biotech is the only DLC pinned, as the other hard dependency (see
# the checker shim's REQUIRED_DLCS note). Ideology is NOT pinned
# even though a stat leaf carries MayRequire for it: a gated leaf changes
# a number, never a def or a key. No family sibling rides along; this
# repo's list is its own. See the engine's header for the membership rule,
# the lowercase-id warning, and the pinning rationale; order is load
# order, the probe last.
#
# Plain ids on purpose: when the Workshop copy of a mod and a local Mods/
# copy coexist, RimWorld suffixes the Workshop one "_steam" and the plain id
# names the local copy. The plain id therefore resolves to whichever single
# copy is installed and never breaks when one of them is removed.
engine.CANONICAL_ACTIVE_MODS = [
    "brrainz.harmony",
    "ludeon.rimworld",
    "ludeon.rimworld.biotech",
    "oskarpotocki.vanillafactionsexpanded.core",
    "oskarpotocki.vfe.pirates",
    "shunter.toxinwarcasket",
    "shunter.l10nprobe",
]


# RATIONALE: the sidecar must hold every injection point the game can see
# under ANY setting, and the spacer tier toggle gates a patch
# (1.6/Patches/SpacerTier.xml) that only ADDS nodes to the defs (the
# shield comp's shieldTexPath, for one) and never removes or rewords a
# translatable one (player text a setting changes goes through Keyed/, per
# CLAUDE.md). A spacer-on boot therefore dumps the superset, so the probe
# boots with the setting forced on, whatever the player's own settings
# file says, and that file is handed back byte-identical afterwards. The
# engine's WORLDS mechanism merges boots over different MOD LISTS, not
# different settings, which is why the flip lives here. Keep this
# invariant when touching the spacer patch: a PatchOperationRemove of a
# translatable node there would need a second boot and a merge instead.
SETTINGS_FILE = "Mod_ToxinWarcasket_ToxinWarcasketMod.xml"
SPACER_ON = (
    '<?xml version="1.0" encoding="utf-8"?>\n'
    "<SettingsBlock>\n"
    '\t<ModSettings Class="ToxinWarcasket.ToxinWarcasketSettings">\n'
    "\t\t<SpacerTier>True</SpacerTier>\n"
    "\t</ModSettings>\n"
    "</SettingsBlock>"
)


def run_with_spacer_tier_on():
    if "--no-launch" in sys.argv:
        return engine.main()
    settings = engine.modsconfig_path().with_name(SETTINGS_FILE)
    original = settings.read_bytes() if settings.exists() else None
    settings.write_text(SPACER_ON, encoding="utf-8-sig")
    print("Probe boot forces the spacer tier setting on (superset of "
          "injection points); the saved settings are restored after.")
    try:
        return engine.main()
    finally:
        if original is None:
            settings.unlink(missing_ok=True)
        else:
            settings.write_bytes(original)
        print(f"Restored {settings}")


raise SystemExit(run_with_spacer_tier_on())
