# GUI_configs/boot_checklist.py

from datetime import datetime
from PyQt6.QtWidgets import QMessageBox
from GUI_configs.global_config import config
import time


def send_ranging_reset(self):
    from CT_ranging_code import setup as ct_setup, write_mcp23008
    import os
    from datetime import datetime

    try:
        # === Determine Pi boot time ===
        boot_time_epoch = os.stat("/proc").st_ctime
        boot_time = datetime.fromtimestamp(boot_time_epoch)

        last_range_timestr = config.ranging_timestamp

        ct_setup()  # initialize relays

        if not last_range_timestr:
            self.append_log("[INFO] No previous ranging timestamp found. Forcing all phases to open (safe default).", "neutral")
            for ph in ["a", "b", "c"]:
                write_mcp23008(ph, 0b00000000)
            config.ranging_status = "open"
            config.ranging_timestamp = datetime.now().isoformat()
            config.save()
            return True

        last_range_time = datetime.fromisoformat(last_range_timestr)

        if last_range_time < boot_time:
            self.append_log("[INFO] Pi rebooted after last ranging command. Resetting to safe low-current (open).", "neutral")
            for ph in ["a", "b", "c"]:
                write_mcp23008(ph, 0b00000000)
            config.ranging_status = "open"
            config.ranging_timestamp = datetime.now().isoformat()
            config.save()
            return True

        # === Skip reapplying bitmasks, just confirm state ===
        self.append_log("[INFO] Ranging was already set. No reset needed.", "neutral")
        return True

    except Exception as e:
        self.append_log(f"[ERROR] Ranging reset failed: {e}", "negative")
        config.ranging_status = "failed"
        config.ranging_timestamp = datetime.now().isoformat()
        config.save()
        return False


def send_asterion_pons_commands(self):
    """Send *RST, *CLS, *SRE 0, *ESE 0 to all Asterion sources and track results."""
    from datetime import datetime
    from PyQt6.QtCore import QThread
    from GUI_configs.global_config import config

    reset_cmds = ["PONS:CURR:PROT:DELAY 0.6", "*RST", "*CLS", "*SRE 0", "*ESE 0"]
    all_keys = ["Va", "Vb", "Vc", "Ia", "Ib", "Ic"]

    any_failures = False

    for key in all_keys:
        port = self.serial_connections.get(key)
        if port and port.isOpen():
            try:
                for cmd in reset_cmds:
                    port.write((cmd + "\n").encode())
                    self.append_log(f"Sent to {key}: {cmd}", "positive")
                    QThread.msleep(100)
                config.pons_status_per_source[key] = {
                    "status": "reset",
                    "timestamp": datetime.now().isoformat()
                }
            except Exception as e:
                self.append_log(f"Error sending reset to {key}: {e}", "negative")
                config.pons_status_per_source[key] = {
                    "status": "failed",
                    "timestamp": datetime.now().isoformat(),
                    "error": str(e)
                }
                any_failures = True
        else:
            self.append_log(f"{key} port not open or not connected.", "negative")
            config.pons_status_per_source[key] = {
                "status": "failed",
                "timestamp": datetime.now().isoformat(),
                "error": "Serial port not open"
            }
            any_failures = True

    config.save()

    return not any_failures


def show_retry_prompt(self, title, error_msg, tip_msg):
    box = QMessageBox(self)
    box.setIcon(QMessageBox.Icon.Warning)
    box.setWindowTitle(title)
    box.setText(error_msg)
    box.setInformativeText(tip_msg)
    box.setStandardButtons(QMessageBox.StandardButton.Retry | QMessageBox.StandardButton.Ignore)
    box.setDefaultButton(QMessageBox.StandardButton.Retry)
    response = box.exec()
    return response == QMessageBox.StandardButton.Retry


def boot_checklist(self):
    self.append_log("======= Running Boot Checklist =======", "neutral")

    # === [STEP 1] Asterion Source Reset ===
    self.append_log("[STEP 1] Asterion Source Reset", "neutral")
    while True:
        try:
            if send_asterion_pons_commands(self):
                config.source_reset_state = {
                    "status": "reset",
                    "timestamp": datetime.now().isoformat()
                }
                self.append_log("[PASS] Source reset completed", "positive")
                break
            else:
                failures = [
                    f"{k}: {v.get('error', 'unknown error')}"
                    for k, v in config.pons_status_per_source.items()
                    if v.get("status") != "reset"
                ]
                raise RuntimeError("Source reset failed for: " + ", ".join(failures) if failures else "Unknown failure")
        except Exception as e:
            config.source_reset_state = {
                "status": "failed",
                "timestamp": datetime.now().isoformat(),
                "error": str(e)
            }
            self.append_log(f"[FAIL] Source reset error: {e}", "negative")
            retry = show_retry_prompt(
                self,
                "Asterion Reset Failed",
                str(e),
                "Verify serial connections to sources. Click Retry after fixing cables or ports."
            )
            if retry:
                self.initialize_serial_connections() 
            else:
                break


    # === [STEP 2] Routing Check ===
    self.append_log("[STEP 2] Routing Check", "neutral")
    while True:
        try:
            if self.check_ct_routing_status():
                config.routing_state = {
                    "status": "open",
                    "timestamp": datetime.now().isoformat()
                }
                self.append_log("[PASS] Routing confirmed/reset to open", "positive")
                break
            else:
                raise RuntimeError("Routing GPIO check failed or reset unsuccessful")
        except Exception as e:
            config.routing_state = {
                "status": "failed",
                "timestamp": datetime.now().isoformat(),
                "error": str(e)
            }
            self.append_log(f"[FAIL] Routing error: {e}", "negative")
            retry = show_retry_prompt(
                self,
                "Routing Reset Failed",
                str(e),
                "Check GPIO wiring for pins 5 and 6. Confirm I2C relay control works, then try again."
            )
            if not retry:
                break

    # === [STEP 3] Ranging Reset ===
    self.append_log("[STEP 3] Ranging Check", "neutral")
    while True:
        try:
            if send_ranging_reset(self):
                config.ranging_state = {
                    "status": "open",
                    "timestamp": datetime.now().isoformat()
                }
                self.append_log("[PASS] Ranging set to open", "positive")
                break
            else:
                raise RuntimeError("Ranging reset failed")
        except Exception as e:
            config.ranging_state = {
                "status": "failed",
                "timestamp": datetime.now().isoformat(),
                "error": str(e)
            }
            self.append_log(f"[FAIL] Ranging error: {e}", "negative")
            retry = show_retry_prompt(
                self,
                "Ranging Reset Failed",
                str(e),
                "Check I2C connections and try again. Ensure relays respond when reset is issued."
            )
            if not retry:
                break

    config.save()
    self.append_log("✅ Boot checklist complete.", "positive")


if __name__ == "__main__":
    print("[INFO] Run this through the GUI entry point to ensure log routing.")
