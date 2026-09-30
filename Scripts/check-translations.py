#!/usr/bin/env python3
# Toxin Warcasket's config shim over the shared translation checker
# (l10n/checker/check_translations.py, the rimworld-l10n submodule). The
# engine holds all logic; this file holds only this repo's config and the
# rationale behind it. Usage is unchanged:
#   python3 Scripts/check-translations.py [--strict] [--root PATH]
# If l10n/ is empty, run: git submodule update --init

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent / "l10n" / "checker"))
import check_translations as engine  # noqa: E402  (import after sys.path edit)

engine.REPO_ROOT = Path(__file__).resolve().parent.parent

# No [TranslationCanChangeCount]-style matching-token fields in this repo:
# the surface is three apparel defs' label/description text.
engine.PARITY_EXEMPT_FIELDS = set()

# RATIONALE: no DLC is a hard dependency (About.xml's modDependencies are
# Harmony and VFE Pirates). Biotech is pinned anyway because it is the
# mod's planned compat gate (LoadFolders.xml's commented Mods/Biotech
# roots): the tox gas ability can only exist with Biotech active, so the
# moment its def or DefInjected lands, a sidecar generated without Biotech
# would silently lose its keys. Pinning the DLC now means that change needs
# no shim edit. Ideology stays out: its only footprint is a MayRequire on a
# stat LEAF (SlaveSuppressionOffset), and a gated leaf drops a number, not a
# def, so it never changes the key set the probe dumps.
engine.REQUIRED_DLCS = {"Biotech"}

# Every def here is a VFEPirates.WarcasketDef, a ThingDef subclass with no
# database of its own (it adds shortDescription and three role flags). The
# game rolls it into DefDatabase<ThingDef>, so the probe's walker dumps
# these defs under ThingDef and DefInjected translations legally target
# 1.6/Languages/<Language>/DefInjected/ThingDef/. The checker needs the
# XML element tag mapped to that folder to match Defs/ against the sidecar.
engine.DEF_TYPE_ALIASES = {
    "VFEPirates.WarcasketDef": "ThingDef",
}

# This mod ships no Languages/ tree yet: its translatable surface is
# DefInjected only (the three defs' text fields) and there are no Keyed
# strings in code. That is a legal state, not a config error, so the engine
# notes it and checks sidecar freshness alone. Flip to False when a Keyed
# file lands (e.g. for mod settings).
engine.ALLOW_NO_KEYED_SURFACE = True

# No Keyed surface, so there is no settings-header key to couple the Steam
# Workshop title to. The description format/coverage checks still run
# against .steamworkshop/Description/.
engine.WORKSHOP_TITLE_KEY = None

raise SystemExit(engine.main())
