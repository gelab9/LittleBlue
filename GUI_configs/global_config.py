import json
import os
from datetime import datetime

CONFIG_FILE = os.path.join(os.path.dirname(__file__), "config_state.json")

class GlobalConfig:
    def __init__(self):
        # Used by main.py and boot_checklist.py
        self.gui_state = "exited_safely"
        self.ranging_status = "unknown"
        self.ranging_timestamp = None
        self.routing_status = "unknown"
        self.routing_timestamp = None
        self.source_reset_status = "unknown"
        self.source_reset_timestamp = None
        self.pons_status_per_source = {}  # e.g., {"Ia_primary": {"status": "reset", "timestamp": "..."}}
        # Used in boot_checklist.py but missing in original
        self.source_reset_state = {}
        self.routing_state = {}
        self.ranging_state = {}

        # ct_config is instantiated but not directly used
        self.ct_config = {
            "routing_i2c_addr": 0x3C,
            "ranging_i2c_addr": {'a': 0x27, 'b': 0x25, 'c': 0x21},
            "last_range_bitmask": {'a': None, 'b': None, 'c': None},
            "last_routing_command": None
        }

        # External application paths (machine-specific)
        self.app_paths = {
            "temperature_rise": r"C:\dev\TempRise\app.py"
        }

    def to_dict(self):
        return {
            "gui_state": self.gui_state,
            "ranging_status": self.ranging_status,
            "ranging_timestamp": self.ranging_timestamp,
            "routing_status": self.routing_status,
            "routing_timestamp": self.routing_timestamp,
            "source_reset_status": self.source_reset_status,
            "source_reset_timestamp": self.source_reset_timestamp,
            "pons_status_per_source": self.pons_status_per_source,
            "source_reset_state": self.source_reset_state,
            "routing_state": self.routing_state,
            "ranging_state": self.ranging_state,
            "ct_config": self.ct_config,
            "app_paths": self.app_paths,
        }

    def save(self):
        try:
            with open(CONFIG_FILE, "w") as f:
                json.dump(self.to_dict(), f, indent=2)
        except Exception as e:
            print(f"Failed to save config: {e}")

    def load(self):
        if not os.path.exists(CONFIG_FILE):
            print("Config file not found. Creating new one with default values.")
            self.save()
            return
        try:
            with open(CONFIG_FILE, "r") as f:
                data = json.load(f)
                self.gui_state = data.get("gui_state", "exited_safely")
                self.ranging_status = data.get("ranging_status", "unknown")
                self.ranging_timestamp = data.get("ranging_timestamp")
                self.routing_status = data.get("routing_status", "unknown")
                self.routing_timestamp = data.get("routing_timestamp")
                self.source_reset_status = data.get("source_reset_status", "unknown")
                self.source_reset_timestamp = data.get("source_reset_timestamp")
                self.pons_status_per_source = data.get("pons_status_per_source", {})
                self.source_reset_state = data.get("source_reset_state", {})
                self.routing_state = data.get("routing_state", {})
                self.ranging_state = data.get("ranging_state", {})
                self.ct_config = data.get("ct_config", {
                    "routing_i2c_addr": 0x3C,
                    "ranging_i2c_addr": {'a': 0x27, 'b': 0x25, 'c': 0x21},
                    "last_range_bitmask": {'a': None, 'b': None, 'c': None},
                    "last_routing_command": None
                })
                self.app_paths = data.get("app_paths", {
                    "temperature_rise": r"Z:\\uslafvs001038\Software\\TempRise Fixture\\Current_Temperature\\Current_Temperature\bin\Debug\\Current_Temperature.exe"
                })
        except Exception as e:
            print(f"❌ Failed to load config: {e}")

# === Singleton Instance ===
config = GlobalConfig()
config.load()