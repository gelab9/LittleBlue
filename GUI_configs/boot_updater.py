#!/usr/bin/env python3

import sys
import os
from datetime import datetime

# Adjust this path to your actual project directory
SCRIPT_DIR = os.path.dirname(os.path.realpath(__file__))
CONFIG_PATH = os.path.join(SCRIPT_DIR, "GUI_configs")

if CONFIG_PATH not in sys.path:
    sys.path.insert(0, CONFIG_PATH)

try:
    from global_config import config
except ImportError as e:
    print(f"Failed to import config: {e}")
    sys.exit(1)

# === Set and persist boot timestamp ===
config.boot_timestamp = datetime.now().isoformat()
config.save()
print(f"[INFO] Boot timestamp set to: {config.boot_timestamp}")
