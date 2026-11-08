# Forces Toxin Warcasket's spacer tier setting on for a pinned boot, shared by
# the refresh and smoke shims (not shipped, not an engine: l10n/ stays
# untouched).
#
# RATIONALE: the l10n engine's pinned_boot (l10n/refresh/refresh_expectations.py)
# sets every pinned mod's settings file aside so the boot runs on DEFAULT
# settings, and has no per-boot settings dimension yet (its header says so).
# The spacer tier is a restart-time patch gate (1.6/Patches/SpacerTier.xml)
# that a default boot never applies, yet the sidecar needs the spacer-on
# superset of injection points and the smoke test needs a spacer-on boot for
# the CE patch's spacer branches. Writing the settings file before the engine
# runs is useless (it is set aside with the rest), so this wraps pinned_boot
# and writes the file INSIDE it, after the engine has cleared the folder, then
# puts back whatever sat there before the engine hands the player's own file
# back. Both engines look pinned_boot up on the module at call time, so
# replacing the attribute reaches the smoke engine's refresh.pinned_boot too.
import contextlib
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "refresh"))
import refresh_expectations as refresh  # noqa: E402  (import after sys.path edit)

SETTINGS_FILE = "Mod_ToxinWarcasket_ToxinWarcasketMod.xml"
SPACER_ON = (
    '<?xml version="1.0" encoding="utf-8"?>\n'
    "<SettingsBlock>\n"
    '\t<ModSettings Class="ToxinWarcasket.ToxinWarcasketSettings">\n'
    "\t\t<SpacerTier>True</SpacerTier>\n"
    "\t</ModSettings>\n"
    "</SettingsBlock>"
)


def force_spacer_tier_on():
    engine_boot = refresh.pinned_boot

    @contextlib.contextmanager
    def pinned_boot(rw, mods):
        with engine_boot(rw, mods):
            settings = refresh.modsconfig_path().with_name(SETTINGS_FILE)
            # Normally absent here (the engine set the player's copy aside);
            # kept anyway in case the engine did not find the mod's folder.
            before = settings.read_bytes() if settings.exists() else None
            settings.write_text(SPACER_ON, encoding="utf-8-sig")
            print("Boot forces Toxin Warcasket's spacer tier setting on.")
            try:
                yield
            finally:
                if before is None:
                    settings.unlink(missing_ok=True)
                else:
                    settings.write_bytes(before)

    refresh.pinned_boot = pinned_boot
