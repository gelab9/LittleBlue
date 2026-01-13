# =============================================================================
# CONFIDENTIAL - DO NOT DISTRIBUTE
# -----------------------------------------------------------------------------
# Title      : Big Blue GUI
# Author     : Amanda Kogut, Ben Eckert, Brendan Duffy, Connor Hise, Fred Holt, Mary Newell, Maxwell Thompson, Nathan Garrett, Zach Burton
# Created    : 2024
# Description: Internal use only. This software is proprietary and confidential.
#
# WARNING:
# This script contains confidential and proprietary information of [Landis+Gyr].
# Unauthorized use, reproduction, or distribution of this software, or any
# portion of it, may result in civil and/or criminal penalties.
#
# © [2025] [Landis+Gyr]. All rights reserved.
# =============================================================================


# ========== Standard Library ==========
import csv
import os
import re
import stat
import sys
import time
from datetime import datetime

# ========== Environment Fixes ==========
def fix_runtime_permissions():
    runtime_dir = f"/run/user/{os.getuid()}"
    try:
        if os.path.exists(runtime_dir):
            mode = stat.S_IMODE(os.stat(runtime_dir).st_mode)
            if mode != 0o700:
                os.chmod(runtime_dir, 0o700)
                print(f"Permissions fixed for {runtime_dir}")
    except Exception as e:
        print(f"Warning: could not check or fix {runtime_dir} permissions: {e}")

fix_runtime_permissions()

# Set once, only if not already provided by the environment
os.environ.setdefault("QT_QPA_PLATFORM", "xcb")

# ========== External Libraries ==========
import serial
import smbus  # or smbus2 if that's what the rest of your code uses

# ========== PyQt6 ==========
from PyQt6.QtCore import QThread, QTimer, Qt, pyqtSignal, QThreadPool
from PyQt6.QtGui import QBrush, QColor
from PyQt6.QtSerialPort import QSerialPort, QSerialPortInfo
from PyQt6.QtWidgets import (
    QApplication,
    QDialog,
    QFileDialog,
    QLabel,
    QMainWindow,
    QMessageBox,
    QProgressBar,
    QTableWidgetItem,
    QTableWidget,
    QVBoxLayout,
    QWidget,
)

# Initialize global thread pool (do this once at startup)
QThreadPool.globalInstance().setMaxThreadCount(os.cpu_count() or 4)

# ========== Project Modules ==========
from functools import partial
from GUI_configs.global_config import config
from port_scanner import scan_serial_ports
from serial_manager import serial_manager
from ui_mainwindow import Ui_MainWindow
from ui_manualcontrol import Ui_Form


class MainWindow(QMainWindow, QThread):
    log_signal = pyqtSignal(str)
    class ExportManager:
        def __init__(self, main_window):
            self.main = main_window
            self.main.ui.table_summary.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)
            self.main.ui.table_script.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)

        def check_overwrite(self, file_path):
            """Checks if file exists and prompts user for overwrite permission."""
            import os
            if not file_path or not os.path.exists(file_path):
                return True
            reply = QMessageBox.question(
                self.main,
                "Overwrite File?",
                f"The file already exists:\n{file_path}\n\nOverwrite?",
                QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No
            )
            return reply == QMessageBox.StandardButton.Yes
    
        def export_summary_data(self):
            try:
                print("Export Summary triggered")
                default_dir = "/home/gelab/Documents"

                file_path, _ = QFileDialog.getSaveFileName(
                    self.main, "Save Summary Data", default_dir, "CSV Files (*.csv)"
                )

                # User canceled
                if not file_path:
                    print("Export canceled: no file selected.")
                    return

                if not self.check_overwrite(file_path):
                    print("Export canceled: user chose not to overwrite existing file.")
                    return

                table = self.main.ui.table_summary
                if table is None:
                    QMessageBox.critical(self.main, "Export Error", "Summary table is not available.")
                    return

                row_count = table.rowCount()
                col_count = table.columnCount()

                # Build headers safely
                headers = []
                for col in range(col_count):
                    h = table.horizontalHeaderItem(col)
                    headers.append(h.text().strip() if (h and h.text()) else f"Col{col+1}")

                # Write CSV with proper quoting and newlines
                import os, csv
                os.makedirs(os.path.dirname(file_path) or ".", exist_ok=True)
                with open(file_path, "w", newline="", encoding="utf-8") as f:
                    writer = csv.writer(f)
                    writer.writerow(headers)
                    for row in range(row_count):
                        row_data = []
                        for col in range(col_count):
                            item = table.item(row, col)
                            row_data.append(item.text().strip() if (item and item.text()) else "")
                        writer.writerow(row_data)

                print(f"Summary exported to {file_path}")
                QMessageBox.information(self.main, "Export Complete",
                                        f"Summary data exported to:\n{file_path}")

                # Optional cleanup if the app defines it
                cleanup = getattr(self.main, "delete_accuracy_temp", None)
                if callable(cleanup):
                    try:
                        cleanup()
                    except Exception as e:
                        print(f"[Cleanup Warning] delete_accuracy_temp failed: {e}")

            except Exception as e:
                print(f"[Export Error] {e}")
                QMessageBox.critical(self.main, "Export Error", f"Failed to export summary:\n{e}")

        def export_every_meter_data(self, script_table, summary_table, test_channels):
            try:
                if not script_table or script_table.rowCount() == 0:
                    QMessageBox.warning(self.main, "No Script", "Script tab has no data to export.")
                    return

                export_dir = QFileDialog.getExistingDirectory(self.main, "Select Export Folder")
                if not export_dir:
                    return  # User canceled

                exported_count = 0

                for chan, data in test_channels.items():
                    metadata = data.get("metadata", {})
                    meter_form = metadata.get("meter_form", "UNK")
                    serial_number = metadata.get("serial_number", "NA")
                    lv_hv = metadata.get("lv_hv", "NA")
                    bef_aft = metadata.get("bef_aft", "NA")

                    filename = f"{meter_form}-{serial_number}-{lv_hv}-{bef_aft}.csv"
                    full_path = os.path.join(export_dir, filename)

                    # === Find summary column ===
                    summary_col = None
                    expected_header = f"Meter {chan}"
                    for col in range(summary_table.columnCount()):
                        header_item = summary_table.horizontalHeaderItem(col)
                        if header_item and header_item.text().strip() == expected_header:
                            summary_col = col
                            break

                    if summary_col is None:
                        print(f"[SKIP] No summary column for Meter {chan}")
                        continue

                    has_data = any(
                        summary_table.item(row, summary_col) and summary_table.item(row, summary_col).text().strip()
                        for row in range(summary_table.rowCount())
                    )
                    if not has_data:
                        print(f"[SKIP] No data for Meter {chan} in summary table.")
                        continue

                    if not self.check_overwrite(full_path):
                        print("Export canceled: user chose not to overwrite existing file.")
                        continue

                    # === Prepare headers ===
                    headers = []
                    for col in range(script_table.columnCount()):
                        header = script_table.horizontalHeaderItem(col)
                        headers.append(header.text() if header else f"Col{col}")

                    accuracy_col_index = None
                    for idx, header in enumerate(headers):
                        if header.strip().lower() == "accuracy":
                            accuracy_col_index = idx
                            break
                    insert_index = accuracy_col_index if accuracy_col_index is not None else 1
                    if accuracy_col_index is None:
                        headers.insert(insert_index, "Accuracy")

                    # === Copy rows and insert accuracy and timestamps ===
                    rows = []
                    for row in range(script_table.rowCount()):
                        row_data = []
                        for col in range(script_table.columnCount()):
                            item = script_table.item(row, col)
                            row_data.append(item.text() if item else "")

                        accuracy = ""
                        if row < summary_table.rowCount():
                            acc_item = summary_table.item(row, summary_col)
                            if acc_item:
                                accuracy = acc_item.text().strip()

                        if accuracy_col_index is not None:
                            if insert_index < len(row_data):
                                row_data[insert_index] = accuracy
                            else:
                                row_data.append(accuracy)
                        else:
                            row_data.insert(insert_index, accuracy)

                        rows.append(row_data)

                    # === Write CSV ===
                    with open(full_path, "w", newline="") as f:
                        import csv
                        writer = csv.writer(f)
                        writer.writerow(headers)
                        writer.writerows(rows)

                    print(f"✅ Exported: {full_path} ({len(rows)} rows)")
                    exported_count += 1

                if exported_count > 0:
                    QMessageBox.information(self.main, "Export Complete", f"{exported_count} meter file(s) exported.")
                else:
                    QMessageBox.information(self.main, "No Data", "No meter files were exported. No data found.")

            except Exception as e:
                QMessageBox.critical(self.main, "Export Error", f"Export failed: {e}")
                print(f"[Export Error] {e}")

        def export_individual_meter_data(self):
            try:
                script_table = self.main.ui.table_script
                summary_table = self.main.ui.table_summary
                test_channels = self.main.test_channels

                if not script_table or script_table.rowCount() == 0:
                    QMessageBox.warning(self.main, "No Script", "Script tab has no data to export.")
                    return

                selected_chan_str = self.main.ui.dropDown_channel.currentText().strip()
                if not selected_chan_str.isdigit():
                    QMessageBox.warning(self.main, "Invalid Channel", "Please select a valid channel number.")
                    return

                chan = int(selected_chan_str)
                if chan not in test_channels:
                    QMessageBox.warning(self.main, "Inactive Channel", f"Channel {chan} is not active.")
                    return

                export_dir = QFileDialog.getExistingDirectory(self.main, "Select Export Folder")
                if not export_dir:
                    return  # User canceled
                
                if not self.check_overwrite(full_path):
                    print("Export canceled: user chose not to overwrite existing file.")
                    return

                data = test_channels[chan]
                metadata = data.get("metadata", {})
                meter_form = metadata.get("meter_form", "UNK")
                serial_number = metadata.get("serial_number", "NA")
                lv_hv = metadata.get("lv_hv", "NA")
                bef_aft = metadata.get("bef_aft", "NA")

                filename = f"{meter_form}-{serial_number}-{lv_hv}-{bef_aft}.csv"
                full_path = os.path.join(export_dir, filename)

                if not self.check_overwrite(full_path):
                    print("Export canceled: user chose not to overwrite existing file.")
                    return

                # === Find summary column ===
                summary_col = None
                expected_header = f"Meter {chan}"
                for col in range(summary_table.columnCount()):
                    header_item = summary_table.horizontalHeaderItem(col)
                    if header_item and header_item.text().strip() == expected_header:
                        summary_col = col
                        break

                # === Prepare headers ===
                headers = []
                for col in range(script_table.columnCount()):
                    header = script_table.horizontalHeaderItem(col)
                    headers.append(header.text() if header else f"Col{col}")

                accuracy_col_index = None
                for idx, header in enumerate(headers):
                    if header.strip().lower() == "accuracy":
                        accuracy_col_index = idx
                        break

                insert_index = accuracy_col_index if accuracy_col_index is not None else 1
                if accuracy_col_index is None:
                    headers.insert(insert_index, "Accuracy")

                # === Copy rows and insert accuracy and timestamps ===
                rows = []
                for row in range(script_table.rowCount()):
                    row_data = []
                    for col in range(script_table.columnCount()):
                        item = script_table.item(row, col)
                        row_data.append(item.text() if item else "")

                    accuracy = ""
                    if summary_col is not None and row < summary_table.rowCount():
                        acc_item = summary_table.item(row, summary_col)
                        if acc_item:
                            accuracy = acc_item.text().strip()

                    if accuracy_col_index is not None:
                        if insert_index < len(row_data):
                            row_data[insert_index] = accuracy
                        else:
                            row_data.append(accuracy)
                    else:
                        row_data.insert(insert_index, accuracy)
                    rows.append(row_data)

                # === Write CSV ===
                with open(full_path, "w", newline="") as f:
                    import csv
                    writer = csv.writer(f)
                    writer.writerow(headers)
                    writer.writerows(rows)

                print(f"Exported individual meter to: {full_path}")
                QMessageBox.information(self.main, "Export Complete", f"Data for channel {chan} exported to:\n{full_path}")

            except Exception as e:
                QMessageBox.critical(self.main, "Export Error", f"Export failed: {e}")
                print(f"[Export Error] {e}")
    
    class WaitDialog(QDialog):
        def __init__(self, duration_ms, message="Please wait...", parent=None):
            super().__init__(parent)
            self.setWindowTitle("Working...")
            self.setModal(True)
            self.setFixedSize(400, 200)
            layout = QVBoxLayout()
            self.label = QLabel(message)
            self.progress = QProgressBar()
            self.progress.setRange(0, duration_ms)
            self.progress.setValue(0)
            layout.addWidget(self.label)
            layout.addWidget(self.progress)
            self.setLayout(layout)

            self.timer = QTimer(self)
            self.interval = 50
            self.elapsed = 0
            self.duration = duration_ms
            self.timer.timeout.connect(self.update_progress)
            self.timer.start(self.interval)

        def update_progress(self):
            self.elapsed += self.interval
            self.progress.setValue(self.elapsed)
            if self.elapsed >= self.duration:
                self.timer.stop()
                self.accept()

    def show_wait_dialog(self, message="Please wait...", duration_ms=None):
        if duration_ms is None:
            raise ValueError("duration_ms must be provided for show_wait_dialog")
        wait_dialog = self.WaitDialog(duration_ms=duration_ms, message=message, parent=self)
        wait_dialog.exec()
        return wait_dialog

    def __init__(self):
        super().__init__()
        self.ui = Ui_MainWindow()
        self.ui.setupUi(self)

        # ---------- LOGGING (init FIRST so anything can safely append_log) ----------
        from collections import deque
        from PyQt6.QtCore import QTimer

        # keep QTextEdit from growing unbounded (optional but recommended)
        try:
            self.ui.TB_messagesReceived.document().setMaximumBlockCount(1000)
            self.ui.TB_messagesReceived.setUndoRedoEnabled(False)
        except Exception:
            pass

        # UI log buffer + timer (20 FPS)
        self._log_ui_buffer = deque()
        self.log_ui_buffer = self._log_ui_buffer  # back-compat alias some code may use
        self._log_ui_timer = QTimer(self)
        self._log_ui_timer.setInterval(50)
        self._log_ui_timer.timeout.connect(self._flush_log_ui)
        self._log_ui_timer.start()

        # File log buffer + timer (10 Hz)
        self._log_file_handle = None
        self._log_file_buffer = deque()
        self._log_file_timer = QTimer(self)
        self._log_file_timer.setInterval(100)
        self._log_file_timer.timeout.connect(self._flush_log_file)
        self._log_file_timer.start()

        # ---------- Rest of your setup ----------
        self.export_manager = MainWindow.ExportManager(self)
        self.setup_file_export_dropdown()

        # NOTE: safe now—even if it logs
        self.initialize_log_file()  # must set self.log_file_path (or leave None)

        config.gui_state = "running"
        config.save()

        # --- app state flags ---
        self.test_channels = {}          # Key: int(channel), Value: dict with step_time, kh
        self.test_channel_keys = []      # Used for iteration order in run_arduino_cycle
        self.initial_step = False        # Flag to track first test step for 'advance_step'
        self.test_aborted = False        # Enforce test abort
        self.test_active = False         # Disable GUI Elements when active
        self.corrected_source_values = False
        self.manual_control_active = False

        # Safe now—even if it logs
        self.initialize_hardware()

        # Buttons / wiring
        self.ui.pB_startTest.clicked.connect(lambda: self.clear_all_metrics())
        self.log_available_ports()  # safe now
        self.ui.pB_selectTest.clicked.connect(self.select_test_script)
        self.ui.pB_startTest.clicked.connect(self.start_test)
        self.ui.pB_addChannel.clicked.connect(self.add_channel)
        self.ui.pB_clearChannel.clicked.connect(self.clear_channel)
        self.ui.pB_abort.clicked.connect(self.abort_test)
        self.ui.pB_clearAllChannels.clicked.connect(self.clear_all_channels)
        self.ui.pB_wifi.clicked.connect(self.run_wifi_checker)
        self.ui.pb_stopTest.clicked.connect(lambda: self.abort_test(IsKeepingVoltage=True))

        # Channels dropdown
        self.channel_form = [str(i) for i in range(1, 17)]
        self.ui.dropDown_channel.addItems(self.channel_form)

        # Meter forms
        self.meter_forms = ["1S", "2S", "3S", "3SC", "4S", "5S", "6S", "7S", "8S", "9S", "10S", "11S", "12S",
                            "12Se", "13S", "14S", "15S", "16S", "17S", "24S", "25S", "26S", "29S", "35S", "36S", "39S", "45S", "46S", "56S", "66S", "76S"]
        self.ui.dropDown_meterForm.addItems(self.meter_forms)
        self.ui.dropDown_meterForm.currentIndexChanged.connect(self.autopopulate_kh)

        # LV/HV
        self.voltage_forms = ["LV", "HV"]
        self.ui.dropDown_lvHV.addItems(self.voltage_forms)

        # bef/aft
        self.phase_form = ["bef", "aft"]
        self.ui.dropDown_befAft.addItems(self.phase_form)

        # Test variable defaults
        self.ui.TB_Kh.setText("0.15")
        self.ui.TB_stepTime.setText("30")
        self.ui.TB_retries.setText("0")
        self.ui.TB_settleTime.setText("1")

        # Script/test state
        self.test_results_file = None
        self.test_script_file = None
        self.test_data = []
        self.current_step = 0
        self.progress_bar_value = 0
        self.correction_attempts = 0
        self.ui.progressBar.setValue(0)

        # Timers
        self.step_timer = QTimer(self)
        self.step_timer.timeout.connect(self.advance_step)

        self.scroll_timer = QTimer(self)
        self.scroll_timer.timeout.connect(self.advance_step)

        # LCD countdown timers
        self.est_time_remaining_timer = QTimer(self)
        self.est_time_remaining_timer.timeout.connect(self.update_est_time_remaining)
        self.next_test_timeout_timer = QTimer(self)
        self.next_test_timeout_timer.timeout.connect(self.update_next_test_timeout)
        self._poll_busy = False
        self._re_meter_counts = re.compile(r"Meter Count for Channel:\s*(\d+)\s+(\d+)")
        self._timeout_path = False

    def _ensure_log_open(self):
        """Open the log file handle once (lazy)."""
        try:
            if getattr(self, "log_file_path", None) and self._log_file_handle is None:
                self._log_file_handle = open(self.log_file_path, "a", encoding="utf-8")
        except Exception as e:
            print(f"Failed to open log file: {e}")
            self._log_file_handle = None

    def _queue_file_log(self, log_type: str, message: str):
        """Enqueue one line for the disk log (non-blocking)."""
        from datetime import datetime
        ts = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        self._log_file_buffer.append(f"[{ts}] {log_type.upper()}: {message}\n")

    def _flush_log_file(self):
        """Flush queued lines to disk (timer-driven and on close)."""
        try:
            if not self._log_file_buffer:
                return
            self._ensure_log_open()
            if self._log_file_handle is None:
                # No file configured—drop buffer to avoid unbounded growth
                self._log_file_buffer.clear()
                return
            self._log_file_handle.writelines(self._log_file_buffer)
            self._log_file_handle.flush()
            self._log_file_buffer.clear()
        except Exception as e:
            print(f"Failed to write to log file: {e}")

    def _flush_log_ui(self):
        """Flush queued HTML lines to the QTextEdit in one append (cheap)."""
        try:
            if not self._log_ui_buffer:
                return
            html = "".join(self._log_ui_buffer)
            self._log_ui_buffer.clear()
            self.ui.TB_messagesReceived.append(html)
        except Exception as e:
            print(f"Failed to flush UI log: {e}")

    def append_log(self, message, log_type="neutral"):
        """
        Non-blocking logger:
          - Buffer HTML for UI (flushed by _log_ui_timer)
          - Buffer plain text for file (flushed by _log_file_timer)
        Defensive if called very early.
        """
        # Defensive init (in case someone calls us before __init__ finishes)
        if not hasattr(self, "_log_ui_buffer"):
            from collections import deque
            self._log_ui_buffer = deque()
            self.log_ui_buffer = self._log_ui_buffer
        if not hasattr(self, "_log_file_buffer"):
            from collections import deque
            self._log_file_buffer = deque()

        color = {"positive": "green", "negative": "red", "neutral": "black"}.get(log_type, "black")
        formatted_html = f"<span style='color:{color}'>{log_type.upper()}: {message}</span>"
        # Queue for UI
        self._log_ui_buffer.append(formatted_html)
        # Queue for file
        self._queue_file_log(log_type, message)

    def closeEvent(self, event):
        """
        Graceful shutdown:
          - Confirm once
          - Stop timers
          - Flush UI + file buffers
          - Close file handle
          - Save config
        """
        if getattr(self, "_closing", False):
            event.accept()
            return

        reply = QMessageBox.question(
            self, "Exit Confirmation", "Are you sure you want to exit Big Blue?",
            QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No,
        )
        if reply != QMessageBox.StandardButton.Yes:
            event.ignore()
            return

        self._closing = True

        # Best-effort to stop any running test
        try:
            self.abort_test()
        except Exception:
            pass

        # Close ManualControlWindow if active
        try:
            if getattr(self, "manual_control_active", False):
                self.close_manual_control_window()
        except Exception:
            pass

        # Record state
        try:
            config.gui_state = "exited_safely"
            config.save()
        except Exception:
            pass

        # Log exit line (queued; don't call append_log if you want to avoid more UI work)
        self._queue_file_log("positive", "Exiting safely from Big Blue")
        try:
            self._log_ui_buffer.append("<span style='color:green'>POSITIVE: Exiting safely from Big Blue</span>")
        except Exception:
            pass

        # Stop timers so they don't fire while we flush
        for name in (
            "_log_ui_timer", "_log_file_timer",
            "est_time_remaining_timer", "next_test_timeout_timer",
            "step_timer", "scroll_timer", "update_count_timer"  # update_count_timer may or may not exist
        ):
            t = getattr(self, name, None)
            try:
                if t and t.isActive():
                    t.stop()
            except Exception:
                pass

        # Final synchronous flushes
        try:
            self._flush_log_ui()
        except Exception:
            pass
        try:
            self._flush_log_file()
        except Exception:
            pass

        # Close log handle
        try:
            if getattr(self, "_log_file_handle", None):
                self._log_file_handle.close()
                self._log_file_handle = None
        except Exception:
            pass

        event.accept()

    def initialize_hardware(self):
        self.initialize_serial_connections()  # Now includes scanning and mapping

        self.set_voltage_button_color("grey")
        self.set_current_button_color("grey")

        self.ui.pB_manualControl.clicked.connect(self.open_manual_control)

        self.serial_port = QSerialPort()
        self.serial_port.setBaudRate(115200)

        # === Run Boot Safety Checklist ===
        from GUI_configs.boot_checklist import boot_checklist
        boot_checklist(self)

    def initialize_serial_connections(self):
        # === Close old ports if any ===
        if hasattr(self, "serial_connections"):
            for conn in self.serial_connections.values():
                if conn and conn.isOpen():
                    try:
                        conn.close()
                    except Exception as e:
                        self.append_log(f"Error closing old port: {e}", "negative")
            time.sleep(0.1)  # Allow OS to release handles

        # === Scan for new device mapping (only opens briefly and closes immediately) ===
        device_ports = scan_serial_ports()

        # === Assign scanned ports ===
        self.Va = device_ports.get('Va')
        self.Vb = device_ports.get('Vb')
        self.Vc = device_ports.get('Vc')
        self.Ia = device_ports.get('Ia')
        self.Ib = device_ports.get('Ib')
        self.Ic = device_ports.get('Ic')
        self.arduino = device_ports.get('arduino')

        self.source_ports = {
            "Va": self.Va,
            "Vb": self.Vb,
            "Vc": self.Vc,
            "Ia": self.Ia,
            "Ib": self.Ib,
            "Ic": self.Ic
        }

        # === Reopen ports ===
        self.serial_connections = {}
        self.append_log("🔄 Scanned and reassigned serial ports:", "neutral")

        for key, port in self.source_ports.items():
            self.append_log(f"→ {key}: {port}", "neutral")
            if port:
                try:
                    connection = serial_manager.open_port(key, port)
                    if connection and connection.isOpen():
                        self.serial_connections[key] = connection
                        self.append_log(f"✅ Opened port {port} for {key}", "positive")
                    else:
                        self.append_log(f"❌ Failed to open port for {key}", "negative")
                except Exception as e:
                    self.append_log(f"❌ Exception opening {key} on {port}: {e}", "negative")
            else:
                self.append_log(f"⚠️ No port assigned for {key}", "negative")

    def initialize_log_file(self):
        try:
        # Generate timestamp and file name
            timestamp = datetime.now().strftime("%m.%d.%Y_%H.%M.%S")
            file_name = f"Log_for_Big_Blue_{timestamp}.rtf"

        # Static log folder path
            target_dir = "/home/gelab/Documents/BigBlue_Log_Files"

        # Ensure the folder exists
            os.makedirs(target_dir, exist_ok=True)

        # Full log file path
            self.log_file_path = os.path.join(target_dir, file_name)

        # Create or clear the log file
            with open(self.log_file_path, "w", encoding="utf-8") as f:
                f.write("")

            self.append_log(f"Log file initialized at: {self.log_file_path}")

        except Exception as ex:
            QMessageBox.critical(self, "Log Initialization Error", f"Failed to initialize log file:\n{str(ex)}")        

    def run_wifi_checker(self):
        """Runs the Pi_Wifi_Checker.py script when the WiFi button is pressed."""
        import subprocess
        try:
            subprocess.Popen(["python3", "/home/gelab/BigBlueFiles/Pi_Wifi_Checker.py"])
            self.append_log("Launched Pi_Wifi_Checker.py", "positive")
        except Exception as e:
            self.append_log(f"Failed to launch Pi_Wifi_Checker.py: {e}", "negative")
            QMessageBox.critical(self, "WiFi Checker Error", f"Could not launch WiFi checker:\n{e}")

    def handle_file_export_selection(self, index):
        option = self.ui.dropDown_fileExport.itemText(index)
        if option == "Export Summary Data":
            self.export_manager.export_summary_data()
        elif option == "Export Every Meter's Data":
            self.export_manager.export_every_meter_data(
                self.ui.table_script,
                self.ui.table_summary,
                self.test_channels
            )

        elif option == "Export Individual Meter's Data":
            self.export_manager.export_individual_meter_data()
        QTimer.singleShot(200, lambda: self.ui.dropDown_fileExport.setCurrentIndex(0))

    def check_ct_routing_status(self):
        from gpiozero import DigitalInputDevice
        from smbus2 import SMBus
        from datetime import datetime

        ROUTING_ADDR = 0x3A
        gpio5 = DigitalInputDevice(5)
        gpio6 = DigitalInputDevice(6)

        try:
            g5 = gpio5.value
            g6 = gpio6.value

            if g5 == 0 and g6 == 0:
                config.routing_status = "320+A"
                self.append_log("CT Routing: 320+A (both GPIO LOW)", "positive")

            elif g5 == 1 and g6 == 0:
                config.routing_status = "5.1-320A"
                self.append_log("CT Routing: 5.1-320A (GPIO5 HIGH, GPIO6 LOW)", "positive")

            elif g5 == 0 and g6 == 1:
                config.routing_status = "0-5A"
                self.append_log("CT Routing: 0-5A (GPIO5 LOW, GPIO6 HIGH)", "positive")

            elif g5 == 1 and g6 == 1:
                self.append_log("❌ CT Routing: INVALID - both GPIO HIGH", "negative")
                config.routing_status = "error"
                # Turn off GPIO5
                with SMBus(1) as bus:
                    bus.write_byte(ROUTING_ADDR, 0xFA)
                    time.sleep(0.5)
                    bus.write_byte(ROUTING_ADDR, 0xFF)
                    time.sleep(0.5)

            config.routing_timestamp = datetime.now().isoformat()
            config.save()
            return True

        except Exception as e:
            self.append_log(f"❌ CT Routing: Exception during check - {e}", "negative")
            config.routing_status = "error"
            config.routing_timestamp = datetime.now().isoformat()
            config.save()
            return False

    def log_available_ports(self):
        available_ports = QSerialPortInfo.availablePorts()
        if available_ports:
            self.append_log("Available COM ports found and ready for selection.", "positive")
        else:
            self.append_log("No COM ports available.", "negative")

    def connect_to_arduino_port(self):
        """Always use the port found by scan_serial_ports() for the Arduino."""
        if not self.arduino:
            QMessageBox.warning(self, "No Arduino Port", "Arduino port not found.")
            self.append_log("Cannot connect: self.arduino is empty.", "negative")
            return

        self.serial_port.setPortName(self.arduino)
        try:
            if self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                self.append_log(f"Connected to Arduino on {self.arduino}.", "positive")
                QMessageBox.information(self, "Connected", f"Connected to Arduino on {self.arduino}.")
            else:
                raise Exception("open() returned False")
        except Exception as ex:
            self.append_log(f"Error opening Arduino port {self.arduino}: {ex}", "negative")
            QMessageBox.critical(self, "Connection Error", f"Error opening Arduino port {self.arduino}: {ex}")

    def disconnect_serial_port(self):
        # Disconnect from the current COM port and clean up resources
        if self.serial_port.isOpen():
            self.serial_port.close()
            self.append_log("Disconnect from serial port.", "neutral")
            QMessageBox.information(self, "Disconnected", "Disconnected from the serial port.")
        else:
            self.append_log("Serial port was not open", "negative")
         
    def autopopulate_kh(self):
        kh_table = {
            "45S":  "0.1",
            "36S":  "0.15",
            "29S":  "????",
            "25S":  "????",
            "16Se": "1.8",
            "16S":  "1.8",
            "15S":  "????",
            "14S":  "????",
            "12Se": "????",
            "12S":  "1.2",
            "9S":   "0.15",
            "8S":   "2",
            "6S":   "3",
            "5S":   "????",
            "4S":   "0.05",
            "3SC":  "0.05",
            "3S":   "0.05",
            "2Se":  "1",
            "2S":   "0.6",
            "1S":   "0.3",
        }
        selected_form = self.ui.dropDown_meterForm.itemText(self.ui.dropDown_meterForm.currentIndex())
        self.ui.TB_Kh.setText(kh_table[selected_form])
        
    def angle_less_than_360(self, angle):
        # Ensure angle is within [0, 360)
        while angle < 0:
            angle += 360
        while angle >= 360:
            angle -= 360
        return angle 

    def _norm_angle(self, a):
        """Normalize degrees to [0, 360) and round to tame float noise."""
        try:
            a = float(a)
        except Exception:
            a = 0.0
        a = a % 360.0
        # -0.0 → 0.0
        if abs(a) < 1e-12:
            a = 0.0
        return round(a, 6)
    
    def _apply_angle_sanitization(self):
        """Normalize all script/command angles to [0,360) using existing _norm_angle()."""
        self.pab = self._norm_angle(getattr(self, "pab", 120.0))
        self.pac = self._norm_angle(getattr(self, "pac", 240.0))
        self.pha = self._norm_angle(getattr(self, "pha", 0.0))
        self.phb = self._norm_angle(getattr(self, "phb", 0.0))
        self.phc = self._norm_angle(getattr(self, "phc", 0.0))

    def source_value_check(self): 
        # TODO: Review and handle boundary conditions for source value check logic
        self.append_log("Starting source_value_check.", "neutral")

        try:
            # Verify the widgets exist
            if not all(hasattr(self.ui, attr) for attr in 
                    ['a_voltage', 'b_voltage', 'c_voltage', 'a_current', 'b_current', 'c_current']):
                self.append_log("One or more text fields are missing in the UI.", "negative")
                QMessageBox.critical(self, "Error", "One or more input fields are missing in the UI.")
                return False
        
             # Check voltage and current ranges
            if ((self.vA_sec > 600 or self.vB_sec > 600 or self.vC_sec > 600 or self.vA_sec < 0 or self.vB_sec < 0 or self.vC_sec < 0) and
                (self.Ia_sec > 350 or self.Ib_sec > 350 or self.Ic_sec > 350 or self.Ia_sec < 0 or self.Ib_sec < 0 or self.Ic_sec < 0)):
                self.append_log("Voltage and current values out of range.", "negative")
                QMessageBox.critical(self, "Voltage and Current Range Error", 
                                    "The voltage for the sources must be ≤ 404 volts and ≥ 0 volts, and current must be between 0 and 15 Amps.")
                return False
            elif (self.vA_sec > 600 or self.vB_sec > 600 or self.vC_sec > 600 or self.vA_sec < 0 or self.vB_sec < 0 or self.vC_sec < 0):
                self.append_log("Voltage values out of range.", "negative")
                QMessageBox.critical(self, "Voltage Range Error", 
                                    "The voltage for the sources must be ≤ 404 volts and ≥ 0 volts.")
                return False
            elif (self.Ia_sec > 350 or self.Ib_sec > 350 or self.Ic_sec > 350 or self.Ia_sec < 0 or self.Ib_sec < 0 or self.Ic_sec < 0):
                self.append_log("Current values out of range.", "negative")
                QMessageBox.critical(self, "Current Range Error", 
                                    "The current for the sources must be between 0 and 15 Amps.")
                return False

            return True

        except ValueError:
            self.append_log("Non-numeric value entered for voltage or current.", "negative")
            QMessageBox.critical(self, "Input Error", "Please enter valid numeric values for all fields.")
            return False
    
    def clear_all_metrics(self):
        self.clear_metric_row('A')
        self.clear_metric_row('B')
        self.clear_metric_row('C')

    def clear_metric_row(self, phase, current=False):
        # Log the action
        message = f"Set metrics to N/A for phase {phase}."
        self.append_log(message, "neutral")

        # Map phase to row index
        row_index = {'A': 0, 'B': 1, 'C': 2}.get(phase)
        if row_index is None:
            message = f"Invalid phase: {phase}. No metrics set to N/A."
            self.append_log(message, "negative")
            return

        # Decide which columns to clear
        if current:
            columns_to_clear = [0, 2, 4, 5]
        else:
            columns_to_clear = range(self.ui.table_metrics.columnCount())

        # Clear the specified columns
        for column in columns_to_clear:
            item = QTableWidgetItem("")
            item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
            self.ui.table_metrics.setItem(row_index, column, item)

    def validate_kh_value(self, kh_value):
        try:
            kh = float(kh_value)
            if kh < 0.025 or kh > 86.4:
                self.append_log("Kh value must be between 0.025 and 86.4.", "negative")
                QMessageBox.critical(self, "Invalid Input", "Kh value must be between 0.025 and 86.4.")
                return False
            return True
        except ValueError:
            self.append_log("The input in 'Kh' is not a valid number.", "negative")
            QMessageBox.critical(self, "Invalid Input", "The input in 'Kh' is not a valid number.")
        return False
    
    def smallest_kh_from_metrics(self, kh_array):
        # Get the smallest non-zero KH value from the given array of KH metrics
        smallest_value = float('inf')
        found_valid_kh = False

        for kh in kh_array:
            if self.validate_kh_value(kh):
                kh_value = float(kh)
                if kh_value > 0 and kh_value < smallest_value:
                    smallest_value = kh_value
                    found_valid_kh = True

        if found_valid_kh:
            self.append_log(f"Smallest non-zero Kh value found: {smallest_value}", "positive")
        else:
            self.append_log("No valid non-zero Kh values found.", "negative")
            smallest_value = 0.0

        return smallest_value
       
    def setup_file_export_dropdown(self):
        self.ui.dropDown_fileExport.clear()
        self.ui.dropDown_fileExport.addItem("File Export")
        self.ui.dropDown_fileExport.addItems([
            "Export Summary Data",
            "Export Every Meter's Data",
            "Export Individual Meter's Data"
        ])
        self.ui.dropDown_fileExport.activated.connect(self.handle_file_export_selection)

    def select_test_script(self):
        # Use the new CIFS mount point instead of GVFS
        default_path = "/mnt/HQA/TEST SCRIPTS/Big Blue Scripts/"

        # --- Warn if summary table has data ---
        summary_table = self.ui.table_summary
        if summary_table.rowCount() > 0 and any(
            summary_table.item(row, col) and summary_table.item(row, col).text().strip()
            for row in range(summary_table.rowCount())
            for col in range(summary_table.columnCount())
        ):
            reply = QMessageBox.warning(
                self,
                "Warning: Data Will Be Erased",
                "Loading a new test script will erase all current summary data.\n\n"
                "Do you want to continue?",
                QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No
            )
            if reply != QMessageBox.StandardButton.Yes:
                return

        if not os.path.exists(default_path):
            QMessageBox.critical(
                self,
                "HQA Share Not Available",
                f"The HQA share was not found at:\n{default_path}\n\n"
                "Make sure it's properly mounted to /mnt/HQA.\n"
                "Try running:  sudo mount -a"
            )
            return

        file_path, _ = QFileDialog.getOpenFileName(
            self,
            "Open Excel File",
            default_path,
            "File Type (*.xlsx *.xls *.tst *.csv)"
        )

        if file_path:
            self.test_script_file = file_path
            self.append_log(f"Test script file selected: {file_path}", "positive")
            self.load_test_script_into_table()

    def load_test_script_into_table(self):
        if not self.test_script_file:
            return

        self.ui.table_script.clearContents()
        self.ui.table_script.setRowCount(0)
        from PyQt6.QtWidgets import QHeaderView
        self.ui.table_script.horizontalHeader().setSectionResizeMode(QHeaderView.ResizeMode.Interactive)
        self.ui.table_summary.horizontalHeader().setSectionResizeMode(QHeaderView.ResizeMode.Interactive)
        self.ui.table_script.setHorizontalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAlwaysOn)
        self.ui.table_summary.setHorizontalScrollBarPolicy(Qt.ScrollBarPolicy.ScrollBarAlwaysOn)


        def _clear_summary_tab():
            self.ui.table_summary.clearContents()
            self.ui.table_summary.setRowCount(0)

        def _populate_summary_table():
            step_count = len(self.test_data)
            self.ui.table_summary.setRowCount(step_count)
            for i in range(step_count):
                step_item = QTableWidgetItem(str(i + 1))
                step_item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                self.ui.table_summary.setItem(i, 0, step_item)

        def _add_timestamp_columns():
            # Script Tab
            script_table = self.ui.table_script
            script_headers = [script_table.horizontalHeaderItem(i).text() if script_table.horizontalHeaderItem(i) else "" for i in range(script_table.columnCount())]
            added = False
            if "Step Started" not in script_headers:
                col = script_table.columnCount()
                script_table.setColumnCount(col + 1)
                script_table.setHorizontalHeaderItem(col, QTableWidgetItem("Step Started"))
                added = True
            if "Step Finished" not in script_headers:
                col = script_table.columnCount()
                script_table.setColumnCount(col + 1)
                script_table.setHorizontalHeaderItem(col, QTableWidgetItem("Step Finished"))
                added = True
            if added:
                self.append_log("Added timestamp columns to Script tab.", "neutral")

            # Summary Tab
            summary_table = self.ui.table_summary
            summary_headers = [summary_table.horizontalHeaderItem(i).text() if summary_table.horizontalHeaderItem(i) else "" for i in range(summary_table.columnCount())]
            added = False
            if "Step Started" not in summary_headers:
                col = summary_table.columnCount()
                summary_table.setColumnCount(col + 1)
                summary_table.setHorizontalHeaderItem(col, QTableWidgetItem("Step Started"))
                added = True
            if "Step Finished" not in summary_headers:
                col = summary_table.columnCount()
                summary_table.setColumnCount(col + 1)
                summary_table.setHorizontalHeaderItem(col, QTableWidgetItem("Step Finished"))
                added = True
            if added:
                self.append_log("Added timestamp columns to Summary tab.", "neutral")

        try:
            with open(self.test_script_file, 'r') as file:
                self.test_data = [line.strip().split() for line in file.readlines()]
                self.ui.table_script.setRowCount(len(self.test_data))
                for row_idx, values in enumerate(self.test_data):
                    step_item = QTableWidgetItem(str(row_idx + 1))
                    step_item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                    self.ui.table_script.setItem(row_idx, 0, step_item)
                    for col_idx, value in enumerate(values):
                        item = QTableWidgetItem(value)
                        item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                        self.ui.table_script.setItem(row_idx, col_idx + 2, item)

                # Center-align all headers if present
                for col in range(self.ui.table_script.columnCount()):
                    header_item = self.ui.table_script.horizontalHeaderItem(col)
                    if header_item:
                        header_item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)

                self.ui.table_script.setMinimumHeight(self.ui.table_script.rowHeight(0))
            _clear_summary_tab()
            _populate_summary_table()
            _add_timestamp_columns()

            # --- Ensure the Script tab is selected after loading ---
            if hasattr(self.ui, "tabWidget"):
                self.ui.tabWidget.setCurrentWidget(self.ui.tab_script)
            elif hasattr(self.ui, "tab_script"):
                # Fallback: try to set focus if tabWidget is not available
                self.ui.tab_script.setFocus()

        except Exception as e:
            QMessageBox.critical(self, "Error", f"Failed to load test script: {str(e)}")

    def protect_disable_buttons(self):
        # Invert the flag to set elements enabled when test is NOT active
        state = not self.test_active
        self.ui.dropDown_fileExport.setEnabled(state)
        self.ui.dropDown_channel.setEnabled(state)
        self.ui.dropDown_meterForm.setEnabled(state)
        self.ui.dropDown_lvHV.setEnabled(state)
        self.ui.dropDown_befAft.setEnabled(state)
        self.ui.dropDown_addVariables.setEnabled(state)
        self.ui.TB_SN.setEnabled(state)
        self.ui.TB_Kh.setEnabled(state)
        self.ui.TB_stepTime.setEnabled(state)
        self.ui.TB_retries.setEnabled(state)
        self.ui.TB_firstStep.setEnabled(state)
        self.ui.TB_settleTime.setEnabled(state)
        self.ui.pB_selectTest.setEnabled(state)
        self.ui.pB_startTest.setEnabled(state)
        self.ui.pB_manualControl.setEnabled(state)
        self.ui.pB_addChannel.setEnabled(state)
        self.ui.pB_clearChannel.setEnabled(state)
        self.ui.pB_clearAllChannels.setEnabled(state)
        self.ui.settleTime_TB.setEnabled(state)
        self.ui.cB_SourceCorrection.setEnabled(state)
        self.ui.dropDown_socket1.setEnabled(state)
        self.ui.dropDown_socket2.setEnabled(state)
        self.ui.TB_testTimeout.setEnabled(state)
        self.ui.TB_testPass.setEnabled(state)

        self.ui.pB_abort.setEnabled(not self.manual_control_active)
        self.ui.pb_stopTest.setEnabled(not self.manual_control_active)

    def check_com_errors(self, response):
        """Checks the response for communication errors."""
        return "ERROR" in response or not response.strip()
    
    def check_current_over_20A(self):
        """Check if any current exceeds 20A to set minimum settling time"""
        if self.Ia_sec >= 20.001 or self.Ib_sec >= 20.001 or self.Ic_sec >= 20.001:
                self.append_log("Current exceeds 20A", "neutral")
                return True
        else:
            self.append_log("Max current is less than 20", "positive")
            return False

    def _source_signature(self):
        """
        Tuple signature of the secondary setpoints used to detect same step.
        Includes voltages, currents, and phase angles (pab, pac, pha, phb, phc).
        Angles are normalized to [0, 360).
        Values rounded to tame float noise.
        """
        return (
            # Voltages (sec)
            round(float(self.vA_sec or 0.0), 6),
            round(float(self.vB_sec or 0.0), 6),
            round(float(self.vC_sec or 0.0), 6),

            # Currents (sec)
            round(float(self.Ia_sec or 0.0), 6),
            round(float(self.Ib_sec or 0.0), 6),
            round(float(self.Ic_sec or 0.0), 6),

            # Phase angles (normalized)
            self._norm_angle(self.pab),
            self._norm_angle(self.pac),
            self._norm_angle(self.pha),
            self._norm_angle(self.phb),
            self._norm_angle(self.phc),
        )

    def _reset_same_step_tracking(self):
        self._skip_full_setup = False

    def start_test(self):
        # 1. Verify test variables and parameters
        self.append_log("Testing 1. Verify test variables and parameters.")
        def verify_test_variables(self) -> bool:
            """
            Perform validation checks on test parameters.
            Returns True if all checks pass.
            Gives warnings where testing can begin without error, but functionality might be impacted.
            Otherwise False and relays errors to the user.
            """
            def fail(message: str) -> bool:
                self.append_log(message, "negative")
                QMessageBox.warning(self, "Invalid Test Parameter", message)
                return False
            
            #Check if test script is loaded
            if not self.test_data:
                QMessageBox.warning(self, "Error", "Please upload a test script file before starting the test.")
                return

            # Check if test channels are added
            self.test_channel_keys = sorted(self.test_channels.keys())
            if not self.test_channel_keys:
                QMessageBox.warning(self, "No Channels", "No channels have been added.")
                return

            # Check S/N
            sn_text = self.ui.TB_SN.text().strip()
            if sn_text == "":
                msg = (
                    "If no serial number is entered, data may not export correctly.\n\n"
                    "Would you like to enter it into the S/N box and try again?"
                )
                reply = QMessageBox.question(
                    self,
                    "Missing Serial Number",
                    msg,
                    QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No
                )
                if reply == QMessageBox.StandardButton.Yes:
                    return False
                else:
                    self.append_log("Warning: S/N is empty. Test can proceed, but S/N will not be recorded.", "neutral")
            elif not sn_text.isdigit() or len(sn_text) < 1 or len(sn_text) > 17:
                return fail("S/N must be a positive integer between 1 and 17 digits.")

            # Check Kh
            kh_text = self.ui.TB_Kh.text().strip()
            if not self.validate_kh_value(kh_text):
                return fail("Kh value must be a float between 0.025 and 86.4.")

            # Check first step
            first_step_text = self.ui.TB_firstStep.text().strip()
            try:
                first_step = int(first_step_text)
                if first_step < 1 or first_step > len(self.test_data) + 1:
                    return fail("First step must be a positive integer between 1 and the number of steps in the test script.")
            except ValueError:
                return fail("Invalid first step: must be a positive integer.")

            # Check step time
            step_time_text = self.ui.TB_stepTime.text().strip()
            try:
                step_time = float(step_time_text)
                if step_time < 1:
                    return fail("Step time must be a float greater than 1 second.")
            except ValueError:
                return fail("Invalid step time: must be a number.")

            # Check settle time
            settle_text = self.ui.TB_settleTime.text().strip()
            try:
                settle_time = float(settle_text)
                if settle_time < 1:
                    return fail("Source settle time must be a positive number greater than or equal to 1 second.")
            except ValueError:
                return fail("Invalid settle time: must be a number.")

            # Check retries
            retries_text = self.ui.TB_retries.text().strip()
            if not retries_text.isdigit() or int(retries_text) < 0:
                return fail("Retry count must be zero or a positive integer.")

            return True

        if verify_test_variables(self):
            self.append_log("Passed initial test input value checks", "positive")
        else:
            return

        # 2. Set Flags, Check for script, active channels, arduino.
        self.append_log("Testing 2. Set Flags, Check for script, active channels, arduino.")
        self.initial_step = True
        self.test_aborted = False
        self.test_active = True
        self.isIa_Low = False
        self.isIb_Low = False
        self.isIc_Low = False
        self.retryCount = 0
        self.allowedRetries = int(self.ui.TB_retries.text())
        self._last_set = None          # cache of last commanded (vA,vB,vC,Ia,Ib,Ic)
        self._skip_full_setup = False  # per-step flag to bypass setup/correction
        self._last_script_sig = None
        self._force_rearm_sources = False  # <— added: timeout will set this True to force re-arming

        self.append_log(f"Active test channels: {self.test_channel_keys}", "positive")
        self.protect_disable_buttons()
        self.append_log("Disabled Test Variables, Channels, and Manual Control buttons.")

        # 2 cont'd. Reset progress bar, step index, metrics table, and timers 
        self.append_log("Testing 2 cont'd. Initialize step, progress bar, and timers.")
        try:
            step_value = int(self.ui.TB_firstStep.text())
            if step_value <= 0:
                raise ValueError("Step must be a positive integer.")
            self.current_step = step_value - 1
            self.append_log(f"Set initial step to {step_value}", "positive")
        except ValueError as e:
            self.append_log(f"Invalid step input: {e}", "negative")
            QMessageBox.warning(self, "Invalid Input", "Please enter a positive integer in Step Start.")
            return

        self.progress_bar_value = 0
        self.ui.progressBar.setValue(0)
        self.progress_step_increment = 100 / (len(self.test_data) - self.current_step)
        self.clear_all_metrics()

        # Configure IMET command and send to Arduino
        try:
            value = self.ui.table_script.item(0, 3).text().strip() if self.ui.table_script.item(0, 3) else ""

            # Ensure port is open (same as RX logic)
            if not self.serial_port.isOpen():
                self.serial_port.setPortName(self.arduino)  # Make sure self.arduino is defined (e.g., "COM3")
                if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                    self.append_log("ERROR: Could not open Arduino port for configuration command", "negative")
                else:
                    self.append_log("Opened Arduino port for configuration command", "neutral")

            # If the port is open now, send the command
            if self.serial_port.isOpen():
                if value == "R":
                    self.ui.table_metrics.setHorizontalHeaderItem(0, QTableWidgetItem("Reactive (VAR)"))
                    command = "scpi,RX,'CONF:IMET:MLIS (VAR,VOLT,AMPS,PHASED,PHAS)',"
                    self.serial_port.write(command.encode("utf-8"))
                    self.serial_port.flush()
                    self.append_log(f"Configured for Power (VAR) by sending '{command}'", "neutral")
                elif value == "K":
                    self.ui.table_metrics.setHorizontalHeaderItem(0, QTableWidgetItem("Apparent (VA)"))
                    command = "scpi,RX,'CONF:IMET:MLIS (VA,VOLT,AMPS,PHASED,PHAS)',"
                    self.serial_port.write(command.encode("utf-8"))
                    self.serial_port.flush()
                    self.append_log(f"Configured for Power (VA) by sending '{command}'", "neutral")
                else:
                    self.ui.table_metrics.setHorizontalHeaderItem(0, QTableWidgetItem("True Pwr (W)"))
                    command = "scpi,RX,'CONF:IMET:MLIS (WATT,VOLT,AMPS,PHASED,PHAS)',"
                    self.serial_port.write(command.encode("utf-8"))
                    self.serial_port.flush()
                    self.append_log(f"Configured for Power (W), {command}", "neutral")
            else:
                self.append_log("Serial port is not open. Cannot send configuration command.", "negative")

        except Exception as e:
            self.append_log(f"Error setting header for table_metrics: {e}", "negative")

        # 3. Start countdown timers for LCDs
        self.est_time_remaining_value = 0
        self.next_test_timeout_value = 0
        self.retryCount = 0
        self.ui.lcd_timeRemaining.display(self.est_time_remaining_value)
        self.ui.lcd_timeout.display(self.next_test_timeout_value)
        self.ui.lcd_retry.display(self.retryCount)

        # 5. Advance step and initialize test
        self.advance_step()
        self.append_log("Test initialized. Waiting for RX metrics and Arduino setup to complete.", "positive")

    def advance_step(self):
        from gpiozero import DigitalInputDevice

        self.append_log("Testing 3. Called 'advance_step', initializing test step or ending test")

        if self.test_aborted:
            self.append_log("Test aborted. Skipping advance_step.", "neutral")
            return

        # Normal progress management
        if not self.initial_step:
            self.current_step += 1
            self.progress_bar_value += self.progress_step_increment
            self.ui.progressBar.setValue(int(self.progress_bar_value))
            self.append_log(f"Advanced to step {self.current_step + 1}", "neutral")

        self.retryCount = 0
        self.ui.lcd_retry.display(self.retryCount)

        # End-of-test handling (unchanged)
        if self.current_step >= self.ui.table_script.rowCount():
            self.current_step -= 1
            self.test_active = False
            self.test_aborted = False
            self.initial_step = False
            self.source_corrected_values = False
            self.protect_disable_buttons()
            self.append_log("Re-enable Test Variables, Channels, and Manual Control buttons after completion.")
            if self.ui.cB_creepAfter.isChecked():
                self.Ia_sec = 0
                self.Ib_sec = 0
                self.Ic_sec = 0
                self.send_source_commands()
            else:
                self.vA_sec = 0
                self.vB_sec = 0
                self.vC_sec = 0
                self.Ia_sec = 0
                self.Ib_sec = 0
                self.Ic_sec = 0
                self.send_source_commands()
            self.append_log(f"Test completed on step {self.current_step + 1}!", "positive")
            return

        if 0 <= self.current_step < self.ui.table_script.rowCount():
            current_item = self.ui.table_script.item(self.current_step, 0)
            if current_item:
                self.ui.table_script.scrollToItem(current_item)
                self.ui.table_script.selectRow(self.current_step)

        # --------- parse row values ---------
        try:
            self._reset_same_step_tracking()  # new: clear per-step skip flag

            def get_cell(row, col, default="0"):
                item = self.ui.table_script.item(row, col)
                if item is None or item.text().strip() == "":
                    self.append_log(f"Missing or empty value at row {row+1}, col {col+1}. Using default: {default}", "neutral")
                    return default
                return item.text().strip()

            # Voltages (sec)
            self.vA_sec = float(get_cell(self.current_step, 4))
            self.vB_sec = float(get_cell(self.current_step, 5))
            self.vC_sec = float(get_cell(self.current_step, 6))
            # Phase angles between voltages
            self.pab   = float(get_cell(self.current_step, 7))
            self.pac   = float(get_cell(self.current_step, 8))
            # Currents (sec)
            self.Ia_sec = float(get_cell(self.current_step, 9))
            self.Ib_sec = float(get_cell(self.current_step, 10))
            self.Ic_sec = float(get_cell(self.current_step, 11))
            # Current phase angles
            self.pha   = float(get_cell(self.current_step, 12))
            self.phb   = float(get_cell(self.current_step, 13))
            self.phc   = float(get_cell(self.current_step, 14))

            # Optional PAUSE=...
            pause_str = get_cell(self.current_step, 16, "")
            if pause_str.startswith("PAUSE="):
                try:
                    self.pause_time = float(pause_str.split("=", 1)[1])
                except ValueError:
                    self.pause_time = 0.0
                    self.append_log(f"Invalid PAUSE value: {pause_str}", "negative")
            else:
                self.pause_time = 0.0

            # Build script signature from the just-parsed row
            current_script_sig = self._source_signature()

            # Decide skip based on previous SCRIPT signature (not corrected outputs)
            if (not self.initial_step 
                and self._last_script_sig is not None 
                and current_script_sig == self._last_script_sig
                and not getattr(self, "_force_rearm_sources", False)):   # <— modified: don't skip when re-arm is forced
                self._skip_full_setup = True
                self.append_log(
                    "Same script V/I/angles as previous step → skipping setup/IMET/source-correction; going straight to accuracy.",
                    "positive"
                )
                # Update last script sig for the record (so consecutive identical rows keep skipping)
                self._last_script_sig = current_script_sig
                self.run_time_based_and_start_tests()
                return

            # Not identical (or re-arm forced) → proceed with normal path; also update last script sig for next step
            self._last_script_sig = current_script_sig

        except Exception as e:
            self.append_log(f"Error parsing test script values at step: {self.current_step + 1}: {e}", "negative")
            return

        # After setting self.Ia_sec/self.Ib_sec/self.Ic_sec
        self._sync_clamps_to_currents(reason="after row parse")

        self._last_script_sig = current_script_sig

        # NEW: force all angles into [0,360). This converts table entries like -120 → 240.
        self._apply_angle_sanitization()

        # Normal path: do the setup since signature differed OR a rearm was forced OR it's the first step
        self.vt_ranging()
        QTimer.singleShot(100, lambda: self._prep_and_continue_ranging() if not self.test_aborted else None)

    def vt_ranging(self):
        # ensure that all V_pri are set to half of V_sec and scaled for the transformer correction factors
        self.vA_pri = self.vA_sec / 2 * 0.9865 # 0.9865 is the correction factor for the A phase voltage
        self.vB_pri = self.vB_sec / 2 * 0.9855 # 0.9855 is the correction factor for the B phase voltage
        self.vC_pri = self.vC_sec / 2 * 0.9903 # 0.9903 is the correction factor for the C phase voltage
        self.append_log(
            f"Ranged expected outputs vA_sec = {self.vA_sec}, vB_sec = {self.vB_sec}, vC_sec = {self.vC_sec} : "
            f"vA_pri = {self.vA_pri}, vB_pri = {self.vB_pri}, vC_pri = {self.vC_pri}",
            "neutral"
        )

    def _prep_and_continue_ranging(self):
        """
        Prep per-phase sources and CT clamp states before CT ranging.

        Clamp rule per scripted secondary current (per phase):
          - OFF if I == 0 or 0 < I <= 5.0
          - ON  if 5.0 < I <= 320.0
          - OFF if I > 320.0

        Clamps are sent to the Arduino COM port (self.serial_port).
        Low/standard prep stays on the per-phase ports in self.serial_connections.
        """
        from PyQt6.QtCore import QTimer

        if getattr(self, "_skip_full_setup", False) and not getattr(self, "_force_rearm_sources", False):  # <— modified
            self.append_log("Skip flag set → bypassing ranging prep.", "neutral")
            self.run_time_based_and_start_tests()
            return

        # ---------- helpers ----------
        ZERO_EPS = 1e-6
        def _clam_state_for(i_val: float) -> str:
            try:
                i = float(i_val)
            except Exception:
                i = 0.0
            if abs(i) <= ZERO_EPS or i <= 5.0 + ZERO_EPS:
                return "OFF"
            if i <= 320.0 + ZERO_EPS:
                return "ON"
            return "OFF"

        def _open_arduino_if_needed() -> bool:
            # Prefer user's helper if present
            if hasattr(self, "_ensure_arduino_port_open"):
                return bool(self._ensure_arduino_port_open())
            # Fallback open logic
            try:
                if not self.serial_port.isOpen():
                    self.serial_port.setPortName(self.arduino)
                    if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                        self.append_log("ERROR: Could not open Arduino port for clamp commands", "negative")
                        return False
                    self.append_log("Opened Arduino port for clamp commands", "neutral")
                return True
            except Exception as e:
                self.append_log(f"ERROR opening Arduino port: {e}", "negative")
                return False

        # ---------- prior low/standard flags ----------
        wasIa_low = self.isIa_Low
        wasIb_low = self.isIb_Low
        wasIc_low = self.isIc_Low

        # 0 < I ≤ 0.24 A considered 'low'
        Ia = float(self.Ia_sec or 0.0)
        Ib = float(self.Ib_sec or 0.0)
        Ic = float(self.Ic_sec or 0.0)

        isIa_low = 0 < Ia <= 0.24
        isIb_low = 0 < Ib <= 0.24
        isIc_low = 0 < Ic <= 0.24

        Ia_modeChanged = (wasIa_low != isIa_low)
        Ib_modeChanged = (wasIb_low != isIb_low)
        Ic_modeChanged = (wasIc_low != isIc_low)

        # Nonzero currents get heavy prep only when mode changed or initial step
        prep_channels = []
        if (Ia_modeChanged or self.initial_step) and Ia > 0.0:
            prep_channels.append(("Ia", Ia, isIa_low, Ia_modeChanged))
        if (Ib_modeChanged or self.initial_step) and Ib > 0.0:
            prep_channels.append(("Ib", Ib, isIb_low, Ib_modeChanged))
        if (Ic_modeChanged or self.initial_step) and Ic > 0.0:
            prep_channels.append(("Ic", Ic, isIc_low, Ic_modeChanged))

        per_cmd_delay      = 250   # ms between sequential commands on a given port
        port_cascade_offset = 10   # ms stagger between different per-phase ports
        clamp_gap_ms        = 200  # ms between Arduino clamp writes (avoid coalescing)
        total_delay         = 0

        # === 1) Push CLAMP states for A/B/C to the Arduino port (based on scripted currents) ===
        if _open_arduino_if_needed():
            clamp_plan = [
                ("A", 1, _clam_state_for(Ia)),
                ("B", 2, _clam_state_for(Ib)),
                ("C", 3, _clam_state_for(Ic)),
            ]
            for i, (label, idx, state) in enumerate(clamp_plan):
                delay_ms = i * clamp_gap_ms
                def send_clamp(lbl=label, iidx=idx, st=state):
                    try:
                        msg = f"scpi,RX,'CONF:MEAS:CLAM{iidx}:ENAB {st}',\n".encode("utf-8")
                        self.serial_port.write(msg)
                        try:
                            self.serial_port.flush()
                        except Exception:
                            pass
                        self.append_log(f"{lbl} clamp → {st} :: {msg.decode().strip()}", "neutral")
                    except Exception as e:
                        self.append_log(f"{lbl} clamp send error: {e}", "negative")
                QTimer.singleShot(delay_ms, send_clamp)
            total_delay = max(total_delay, (len(clamp_plan) - 1) * clamp_gap_ms + per_cmd_delay)

        # === 2) Existing ranging prep for phases that are NONZERO and need mode prep ===
        for j, (key, current_value, isLowNow, modeChanged) in enumerate(prep_channels):
            port = self.serial_connections.get(key)
            if not (port and port.isOpen()):
                self.append_log(f"{key} port not open for ranging prep.", "negative")
                continue

            port_base_delay = j * port_cascade_offset + total_delay  # start after clamp wave
            cmd_delay = 0

            if (modeChanged or self.initial_step) and isLowNow:
                self.append_log(f"Sending low-current setup to {key}", "neutral")

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda p=port, k=key: (self.append_log(f"{k} → CURR:PROT:STAT ON", "neutral"),
                                           p.write(b"CURR:PROT:STAT ON\n")))
                cmd_delay += per_cmd_delay

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda p=port, k=key: (self.append_log(f"{k} → VOLT:RANG 400", "neutral"),
                                           p.write(b"VOLT:RANG 400\n")))
                cmd_delay += per_cmd_delay

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda p=port, k=key: (self.append_log(f"{k} → CURR 0.004", "neutral"),
                                           p.write(b"CURR 0.004\n")))
                cmd_delay += per_cmd_delay

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda k=key: self.append_log(f"Low-current prep complete for {k}", "positive"))

                setattr(self, f"is{key}_Low", True)

            elif (modeChanged or self.initial_step) and not isLowNow:
                self.append_log(f"Sending standard-current setup to {key}", "neutral")

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda p=port, k=key: (self.append_log(f"{k} → CURR:PROT:STAT OFF", "neutral"),
                                           p.write(b"CURR:PROT:STAT OFF\n")))
                cmd_delay += per_cmd_delay

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda p=port, k=key: (self.append_log(f"{k} → VOLT:RANG 200", "neutral"),
                                           p.write(b"VOLT:RANG 200\n")))
                cmd_delay += per_cmd_delay

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda p=port, k=key: (self.append_log(f"{k} → VOLT 100", "neutral"),
                                           p.write(b"VOLT 100\n")))
                cmd_delay += per_cmd_delay

                QTimer.singleShot(port_base_delay + cmd_delay,
                    lambda k=key: self.append_log(f"Standard-current prep complete for {k}", "positive"))

                setattr(self, f"is{key}_Low", False)

            total_delay = max(total_delay, port_base_delay + cmd_delay + per_cmd_delay)

        # === 3) Continue after all queued writes have time to transmit ===
        QTimer.singleShot(total_delay, lambda: self._continue_ct_ranging() if not self.test_aborted else None)

    def _continue_ct_ranging(self):
        if getattr(self, "_skip_full_setup", False) and not getattr(self, "_force_rearm_sources", False):  # <— modified
            self.append_log("Skip flag set → bypassing CT ranging.", "neutral")
            self.run_time_based_and_start_tests()
            return
        # ... keep your original body below ...

        tempIa_sec = self.Ia_sec
        tempIb_sec = self.Ib_sec
        tempIc_sec = self.Ic_sec

        # Step 1: Set all current setpoints to 0 and power down sources
        self.Ia_sec = 0
        self.Ib_sec = 0
        self.Ic_sec = 0
        self.send_source_commands()
        self.append_log("Waiting for current sources to power down before switching relay ranges", "neutral")

        # Step 2: Restore the original current setpoints
        self.Ia_sec = tempIa_sec
        self.Ib_sec = tempIb_sec   
        self.Ic_sec = tempIc_sec

        # Step 3: Send CT ranging command
        def continue_ct_setup():
            from CT_ranging_code import set_current, setup as ct_setup
            try:
                if self.test_aborted:
                    self.append_log("CT ranging skipped due to test abort.", "neutral")
                    return
                # If all currents are zero, skip CT setup and routing, jump to activate sources
                if self.Ia_sec == 0 and self.Ib_sec == 0 and self.Ic_sec == 0:
                    self.append_log("All currents are zero. Skipping CT ranging and routing, jumping to activate sources.", "neutral")
                    QTimer.singleShot(100, lambda: self._continue_activate_sources() if not self.test_aborted else None)
                    return
                ct_setup()
                if self.Ia_sec > 0:
                    set_current('a', self.Ia_sec)
                if self.Ib_sec > 0:
                    set_current('b', self.Ib_sec)
                if self.Ic_sec > 0:
                    set_current('c', self.Ic_sec)
                self.append_log("CT ranging completed.", "positive")
                QTimer.singleShot(200, lambda: self._continue_ct_routing() if not self.test_aborted else None)
            except Exception as e:
                self.append_log(f"CT Ranging I2C Error: {e}", "negative")
                QMessageBox.critical(self, "CT Ranging Error", f"CT ranging failed: {e}")

        # Wait 300ms before CT setup to ensure sources are fully powered down
        QTimer.singleShot(200, continue_ct_setup)

    def _continue_ct_routing(self):
        if getattr(self, "_skip_full_setup", False) and not getattr(self, "_force_rearm_sources", False):  # <— modified
            self.append_log("Skip flag set → bypassing CT routing.", "neutral")
            self.run_time_based_and_start_tests()
            return
        # ... keep your original body below ...

        from smbus2 import SMBus
        from gpiozero import DigitalInputDevice

        I2C_BUS = 1
        ROUTING_ADDR = 0x3A
        gpio5 = DigitalInputDevice(5)
        gpio6 = DigitalInputDevice(6)

        max_current = max(self.Ia_sec, self.Ib_sec, self.Ic_sec)
        self._ct_routing_bus = SMBus(I2C_BUS)  # Keep bus open for all steps

        def cleanup_bus():
            if hasattr(self, "_ct_routing_bus") and self._ct_routing_bus:
                try:
                    self._ct_routing_bus.close()
                except Exception:
                    pass
                self._ct_routing_bus = None

        def finish():
            self.append_log("CT routing completed.", "positive")
            cleanup_bus()
            QTimer.singleShot(100, self._continue_activate_sources)

        def fail(msg):
            self.append_log(f"CT Routing Error: {msg}", "negative")
            cleanup_bus()
            QMessageBox.critical(self, "CT Routing Error", f"Routing failed: {msg}")

        gpio5_state = gpio5.value
        gpio6_state = gpio6.value

        # 0–5A
        if max_current <= 5.0:
            if not (gpio6_state == 1 and gpio5_state == 0):
                def step1():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFA)
                    QTimer.singleShot(300, step2)
                def step2():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(7000, step3)
                def step3():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xF7)
                    QTimer.singleShot(300, step4)
                def step4():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(100, check_gpio)
                def check_gpio():
                    if not (gpio6.value == 1 and gpio5.value == 0):
                        fail("0–5A config not reached (GPIO6 HIGH, GPIO5 LOW)")
                    else:
                        finish()
                step1()
            else:
                finish()
        # 5–320A
        elif max_current <= 320.0:
            if not (gpio5_state == 1 and gpio6_state == 0):
                def step1():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFB)
                    QTimer.singleShot(300, step2)
                def step2():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(7000, step3)
                def step3():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xF6)
                    QTimer.singleShot(300, step4)
                def step4():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(100, check_gpio)
                def check_gpio():
                    if not (gpio5.value == 1 and gpio6.value == 0):
                        fail("5–320A config not reached (GPIO5 HIGH, GPIO6 LOW)")
                    else:
                        finish()
                step1()
            else:
                finish()
        # >320A
        else:
            if not (gpio5_state == 0 and gpio6_state == 0):
                def step1():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFA)
                    QTimer.singleShot(300, step2)
                def step2():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(7000, step3)
                def step3():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFB)
                    QTimer.singleShot(300, step4)
                def step4():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(100, check_gpio)
                def check_gpio():
                    if gpio5.value != 0 or gpio6.value != 0:
                        fail("320+A config not reached (both GPIO LOW)")
                    else:
                        finish()
                step1()
            else:
                finish()

    def _continue_activate_sources(self):
        if getattr(self, "_skip_full_setup", False):
            self.append_log("Skip flag set → not (re)sending sources; skipping to accuracy.", "neutral")
            # Do not call send_source_commands or IMET reads
            # Go straight to pause/test/accuracy pipeline
            self.check_pause_time()  # or run_time_based_and_start_tests(), either is fine
            return
        # ... keep your original body below ...

        if self.test_aborted:
            self.append_log("Source activation skipped due to test abort.", "neutral")
            return

        self.send_source_commands()
        self._sync_clamps_to_currents(reason="after source send")

        # Parse settling delay from textbox
        settle_text = self.ui.TB_settleTime.text().strip()
        try:
            # If all currents are zero, use 100 ms settle time
            if self.Ia_sec == 0 and self.Ib_sec == 0 and self.Ic_sec == 0:
                settle_delay_ms = 100
                self.append_log("All currents are zero. Waiting 100 ms for sources to settle.", "neutral")
            elif int(float(settle_text) * 1000) < 7500 and self.check_current_over_20A(): 
                settle_delay_ms = 7500
                self.append_log(f"Waiting {settle_delay_ms} ms for sources to settle.", "neutral")
            else:
                settle_delay_ms = int(float(settle_text) * 1000)
                self.append_log(f"Waiting {settle_delay_ms} ms for sources to settle.", "neutral")
        except:
            settle_delay_ms = 5000
            self.append_log("Invalid or missing settle time. Defaulting to 5000 ms.", "neutral")

        def read_initial_metrics(phase_index=0, attempt=0):
            """Reads initial metrics from the source for each phase and populates the metrics table.
            Retries each phase once if it fails or returns a communication error."""
            if self.test_aborted:
                self.append_log("Initial RX read aborted.", "neutral")
                return

            phases = [
                ('A', 1, 0),
                ('B', 2, 1),
                ('C', 3, 2)
            ]

            if phase_index >= len(phases):
                self.append_log("Initial metric read complete for all phases.", "positive")

                def after_initial_metrics():
                    if self.test_aborted:
                        self.append_log("Skipped RX read due to abort.", "neutral")
                        return

                    if self.ui.cB_SourceCorrection.isChecked():
                        self.append_log("Starting source correction as checkbox is checked.", "neutral")
                        self.source_correction()
                    else:
                        self.append_log("Skipping source correction (checkbox not checked).", "neutral")
                        try:
                            self.serial_port.flush()
                            self.serial_port.clear(QSerialPort.Direction.AllDirections)
                        except Exception as e:
                            self.append_log(f"Warning: Failed to flush serial port: {e}", "negative")
                        self.check_pause_time()

                QTimer.singleShot(200, after_initial_metrics)
                return

            phase_label, imet_num, row_index = phases[phase_index]

            try:
                if not self.serial_port.isOpen():
                    self.serial_port.setPortName(self.arduino)
                    if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                        self.append_log(f"ERROR: Could not open Arduino port for initial RX (Phase {phase_label})", "negative")
                        if attempt == 0:
                            QTimer.singleShot(250, lambda: read_initial_metrics(phase_index, attempt + 1))
                        else:
                            QTimer.singleShot(250, lambda: read_initial_metrics(phase_index + 1, 0))
                        return

                cmd = f"scpi,RX,'READ:IMET{imet_num}?',"
                self.serial_port.write(cmd.encode())
                self.append_log(f"Sent RX command for initial IMET{imet_num} (Phase {phase_label})", "neutral")

                def process_initial_response():
                    if self.test_aborted:
                        self.append_log(f"Initial RX read aborted during {phase_label}.", "neutral")
                        return

                    try:
                        response = ""
                        while self.serial_port.canReadLine():
                            line = self.serial_port.readLine().data().decode("utf-8", errors="replace").strip()
                            response += line

                        self.append_log(f"Initial IMET{imet_num} response for Phase {phase_label}: {response}", "neutral")

                        match = re.search(r"(VUNDer|CUNDer|OK|CRTRansient|COVer)[,~]\(([^)]+)\)", response)
                        comm_error = not match or len(response) < 30
                        values = re.split(r'[~,]', match.group(2)) if match else []
                        data_error = len(values) < 5

                        if comm_error or data_error:
                            self.append_log(
                                f"Initial metric extraction failed for Phase {phase_label} (attempt {attempt + 1})", "negative"
                            )
                            if attempt == 0:
                                QTimer.singleShot(250, lambda: read_initial_metrics(phase_index, attempt + 1))
                            else:
                                QTimer.singleShot(250, lambda: read_initial_metrics(phase_index + 1, 0))
                            return

                        metrics = [float(v) for v in values[:5]]
                        for col, val in enumerate(metrics):
                            item = QTableWidgetItem(f"{val:.4f}")
                            item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                            self.ui.table_metrics.setItem(row_index, col, item)
                        self.append_log(f"Initial metrics for Phase {phase_label} populated: {metrics}", "positive")

                    except Exception as e:
                        self.append_log(f"Error parsing initial RX response for Phase {phase_label}: {e}", "negative")
                        if attempt == 0:
                            QTimer.singleShot(250, lambda: read_initial_metrics(phase_index, attempt + 1))
                        else:
                            QTimer.singleShot(250, lambda: read_initial_metrics(phase_index + 1, 0))
                        return

                    QTimer.singleShot(250, lambda: read_initial_metrics(phase_index + 1, 0))

                QTimer.singleShot(1200, process_initial_response)

            except Exception as e:
                self.append_log(f"Exception sending initial RX command for Phase {phase_label}: {e}", "negative")
                if attempt == 0:
                    QTimer.singleShot(250, lambda: read_initial_metrics(phase_index, attempt + 1))
                else:
                    QTimer.singleShot(250, lambda: read_initial_metrics(phase_index + 1, 0))
                self.append_log(f"Completed pulling 'Initial Metrics'", "positive")

        # Start reading IMET metrics after settling time
        QTimer.singleShot(settle_delay_ms, lambda: read_initial_metrics())

    def scale_current(self, I_secondary):
        if 0.0 <= I_secondary <= 1.5: return round(I_secondary / 10, 6)
        elif 1.501 <= I_secondary <= 5.0: return round(I_secondary / 10, 6)
        elif 5.01 <= I_secondary <= 10.25: return round(I_secondary / 10, 6)
        elif 10.25 <= I_secondary <= 20.001: return round(I_secondary / 20, 6)
        elif 20.001 <= I_secondary <= 41: return round(I_secondary / 20, 6)
        elif 41 <= I_secondary <= 600.0: return round(I_secondary / 78, 6)
        else: return I_secondary

    def set_voltage_button_color(self, color):
        """Set the color of pB_Voltage."""
        self.ui.pB_Voltage.setStyleSheet(f"background-color: {color};")
    
    def set_current_button_color(self, color):
        """Set the color of pB_Current."""
        self.ui.pB_Current.setStyleSheet(f"background-color: {color};")
   
    def _normalize_and_apply_current_outp_for_sec(self, cmds_in):
        """
        Returns a LIST of 6 command strings [Va,Vb,Vc,Ia,Ib,Ic],
        forcing current channels to OUTP 0 when their *_sec == 0, else OUTP 1.
        Works with tuples/lists/strings/None from source_command_builder.
        """
        import re

        # 1) Normalize container -> list length 6 of strings
        if cmds_in is None:
            raw = []
        else:
            raw = list(cmds_in)

        while len(raw) < 6:
            raw.append("")

        norm = []
        for c in raw[:6]:
            if isinstance(c, (list, tuple)):
                parts = []
                for x in c:
                    if x is None:
                        continue
                    s = str(x).strip()
                    if s:
                        parts.append(s.strip("; "))
                c = ";".join(parts)
            elif c is None:
                c = ""
            else:
                c = str(c).strip()

            c = c.strip("; ").replace(";;", ";").strip()
            norm.append(c)

        cmds = norm  # mutable list

        # 2) Desired OUTP for current channels (indices 3,4,5) from *_sec
        ia_on = (float(getattr(self, "Ia_sec", 0.0)) > 0.0)
        ib_on = (float(getattr(self, "Ib_sec", 0.0)) > 0.0)
        ic_on = (float(getattr(self, "Ic_sec", 0.0)) > 0.0)

        desired = {
            3: "1" if ia_on else "0",  # Ia
            4: "1" if ib_on else "0",  # Ib
            5: "1" if ic_on else "0",  # Ic
        }

        # 3) Rewrite or insert OUTP token for Ia/Ib/Ic only
        pat = re.compile(r'\bOUTP\s+[01]\b', flags=re.IGNORECASE)
        for idx in (3, 4, 5):
            c = cmds[idx]
            if not c:
                cmds[idx] = f"OUTP {desired[idx]};"
                continue

            if pat.search(c):
                c = pat.sub(f"OUTP {desired[idx]}", c)
            else:
                c = f"OUTP {desired[idx]};{c}"

            cmds[idx] = c.strip("; ")

        return cmds

    def _ensure_arduino_port_open(self) -> bool:
        try:
            if not self.serial_port.isOpen():
                self.serial_port.setPortName(self.arduino)
                if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                    self.append_log("ERROR: Could not open Arduino port for clamp commands", "negative")
                    return False
                self.append_log("Opened Arduino port for clamp commands", "neutral")
            return True
        except Exception as e:
            self.append_log(f"ERROR opening Arduino port: {e}", "negative")
            return False

    def _sync_clamps_to_currents(self, *, reason: str = ""):

        from PyQt6.QtCore import QTimer

        def f(x):
            try:
                return float(x)
            except Exception:
                return 0.0

        Ia = f(getattr(self, "Ia_sec", 0.0))
        Ib = f(getattr(self, "Ib_sec", 0.0))
        Ic = f(getattr(self, "Ic_sec", 0.0))

        # --- Decide pulse mapping token (P1/P2/P3/TOT) based on scripted currents ---
        # onA = Ia > 0.0
        # onB = Ib > 0.0
        # onC = Ic > 0.0
        # num_on = int(onA) + int(onB) + int(onC)

        # if num_on >= 2 or num_on == 0:
        #     pulse_token = "TOT"
        # elif onA:
        #     pulse_token = "P1"
        # elif onB:
        #     pulse_token = "P2"
        # else:
        #     pulse_token = "P3"

        # --- Determine clamp states from scripted currents ---
        ZERO_EPS = 1e-6
        LOW_MAX = 5.0
        MID_MAX = 320.0

        def clamp_state(i):
            i = f(i)
            if abs(i) <= ZERO_EPS:
                return "OFF"
            if 0.0 < i <= LOW_MAX + ZERO_EPS:
                return "OFF"
            if i <= MID_MAX + ZERO_EPS:
                return "ON"
            return "OFF"

        plan = [
            ("A", 1, clamp_state(Ia)),
            ("B", 2, clamp_state(Ib)),
            ("C", 3, clamp_state(Ic)),
        ]

        # --- Ensure Arduino COM is open ---
        if not self._ensure_arduino_port_open():
            return

        # --- 1) Send RX pulse source selection first ---
        # try:
        #     sel_cmd = f"scpi,RX,'SYST:PORT2:SOUR {pulse_token}',\n"
        #     self.serial_port.write(sel_cmd.encode("utf-8"))
        #     try:
        #         self.serial_port.flush()
        #     except Exception:
        #         pass
        #     self.append_log(f"RX pulse mapping ? {pulse_token} ({reason}) :: {sel_cmd.strip()}", "neutral")
        # except Exception as e:
        #     self.append_log(f"RX pulse mapping send error: {e}", "negative")

        # Small gap before CLAM commands so the standard digests PORT2:SOUR cleanly
        selection_gap_ms = 200

        # --- 2) Staggered CLAM writes (after the selection gap) ---
        base_gap_ms = 150
        for i, (label, idx, state) in enumerate(plan):
            def send(idx=idx, label=label, state=state):
                try:
                    msg = f"scpi,RX,'CONF:MEAS:CLAM{idx}:ENAB {state}',\n"
                    self.serial_port.write(msg.encode("utf-8"))
                    try:
                        self.serial_port.flush()
                    except Exception:
                        pass
                    self.append_log(f"{label} clamp ? {state} ({reason}) :: {msg.strip()}", "neutral")
                except Exception as e:
                    self.append_log(f"{label} clamp send error: {e}", "negative")

            QTimer.singleShot(selection_gap_ms + i * base_gap_ms, send)

    def send_source_commands(self):
        """
        Sends voltage and current commands safely.

        RULES:
          • Voltage sources are ALWAYS enabled (OUTP 1). If script voltage magnitude is 0, send VOLT 0.
          • Current sources are enabled ONLY if script current > 0 (else OUTP 0).
          • Voltage phases always receive an explicit PHAS (A=0°, B=self.pab, C=self.pac).
          • Current phases use PHAS self.pha/self.phb/self.phc when ON.
          • Angle attributes are seeded ONCE from the script if not already set, and then preserved.
        """
        
        if self.test_aborted:
            self.append_log("Source activation skipped due to test abort.", "neutral")
            return
        self._sync_clamps_to_currents(reason="before source send")

        # ---------------- helpers ----------------
        def _cell_text(row, col):
            try:
                it = self.ui.table_script.item(row, col)
                return it.text().strip() if (it and it.text()) else ""
            except Exception:
                return ""

        def _cell_float(row, col, default=None):
            s = _cell_text(self.current_step, col)
            if s == "":
                return default
            try:
                return float(s)
            except Exception:
                return default

        def _seed_once(attr_name, default_factory):
            """If angle attribute is missing/None, seed it from script via default_factory()."""
            cur = getattr(self, attr_name, None)
            if cur is None:
                cur = default_factory()
                setattr(self, attr_name, cur)
            # normalize but DO NOT replace with hardcoded defaults
            try:
                setattr(self, attr_name, self._norm_angle(cur))
            except Exception:
                pass

        def _enforce_current_outp(cmd: str, on: bool) -> str:
            """
            Ensure current-channel OUTP is correct:
              - on=True  -> OUTP 1 (leave rest intact)
              - on=False -> OUTP 0; ensure zero magnitude (CURR 0 or VOLT 0) and strip PHAS if you prefer
            """
            import re
            desired = "1" if on else "0"
            if not cmd:
                return f"OUTP {desired};" if on else "OUTP 0;CURR 0;"

            # Replace any OUTP X with the desired one (or insert if missing)
            if re.search(r'\bOUTP\s+[01]\b', cmd, flags=re.IGNORECASE):
                cmd = re.sub(r'\bOUTP\s+[01]\b', f'OUTP {desired}', cmd, flags=re.IGNORECASE)
            else:
                cmd = f"OUTP {desired};{cmd}"

            if not on:
                # Force zero magnitude token present (prefer CURR 0; if not present, insert; also zero VOLT path)
                if "CURR" in cmd.upper():
                    cmd = re.sub(r'\bCURR\s+[^;]*', 'CURR 0', cmd, flags=re.IGNORECASE)
                elif "VOLT" in cmd.upper():
                    cmd = re.sub(r'\bVOLT\s+[^;]*', 'VOLT 0', cmd, flags=re.IGNORECASE)
                else:
                    cmd = f"{cmd};CURR 0"
            return cmd.strip("; ") + ";"

        # ---------------- seed PHAS only if missing ----------------
        # Voltage absolute targets from script (for seeding only)
        vB_target = _cell_float(self.current_step, 7, 120.0)  # pab
        vC_target = _cell_float(self.current_step, 8, 240.0)  # pac
        vA_target = 0.0

        # Seed B/C voltage angles once (A is always 0° command)
        _seed_once("pab", lambda: vB_target if vB_target is not None else 120.0)
        _seed_once("pac", lambda: vC_target if vC_target is not None else 240.0)

        # Seed current absolute angles once:
        # If PHx cell empty → in-phase with that phase's V target
        pha_script = _cell_float(self.current_step, 12, None)  # A current abs angle
        phb_script = _cell_float(self.current_step, 13, None)  # B current abs angle
        phc_script = _cell_float(self.current_step, 14, None)  # C current abs angle

        _seed_once("pha", lambda: vA_target if pha_script is None else pha_script)
        _seed_once("phb", lambda: (vB_target if vB_target is not None else 120.0) if phb_script is None else phb_script)
        _seed_once("phc", lambda: (vC_target if vC_target is not None else 240.0) if phc_script is None else phc_script)

        # ---------------- determine which magnitudes are "on" from script ----------------
        vA_on = self.vA_sec > 0.0
        vB_on = self.vB_sec > 0.0
        vC_on = self.vC_sec > 0.0
        iA_on = self.Ia_sec > 0.0
        iB_on = self.Ib_sec > 0.0
        iC_on = self.Ic_sec > 0.0

        # Clear UI rows when scripted 0 (we still leave V OUTP=1 with VOLT 0 for phase ref)
        if not vA_on: self.clear_metric_row('A')
        if not vB_on: self.clear_metric_row('B')
        if not vC_on: self.clear_metric_row('C')
        if not iA_on: self.clear_metric_row('A', current=True)
        if not iB_on: self.clear_metric_row('B', current=True)
        if not iC_on: self.clear_metric_row('C', current=True)

        # ---------------- compute primary volt magnitudes ----------------
        self.vt_ranging()  # updates vA_pri/vB_pri/vC_pri

        cmds = {}

        # --- Voltage channels: ALWAYS OUTP 1; VOLT is either ranged value or 0; always set PHAS ---
        # A (reference 0°)
        Va_volt = 0 if not vA_on else self.vA_pri
        cmds["Va"] = f"OUTP 1;VOLT {Va_volt};PHAS 0;"

        # B (use persisted self.pab set by correction loop / initial seed)
        Vb_volt = 0 if not vB_on else self.vB_pri
        cmds["Vb"] = f"OUTP 1;VOLT {Vb_volt};PHAS {self._norm_angle(getattr(self, 'pab', 120.0))};"

        # C (use persisted self.pac)
        Vc_volt = 0 if not vC_on else self.vC_pri
        cmds["Vc"] = f"OUTP 1;VOLT {Vc_volt};PHAS {self._norm_angle(getattr(self, 'pac', 240.0))};"

        # --- Current channels: OUTP only if >0; use persisted self.pha/b/c when ON ---
        if iA_on:
            if 0.0 < self.Ia_sec <= 0.24:
                cmds["Ia"] = f"OUTP 1;VOLT {round(self.Ia_sec * 1000)};PHAS {self._norm_angle(getattr(self, 'pha', vA_target))};"
            else:
                self.Ia_pri = self.scale_current(self.Ia_sec)
                cmds["Ia"] = f"OUTP 1;CURR {self.Ia_pri};PHAS {self._norm_angle(getattr(self, 'pha', vA_target))};"
        else:
            cmds["Ia"] = "OUTP 0;CURR 0;"

        if iB_on:
            if 0.0 < self.Ib_sec <= 0.24:
                cmds["Ib"] = f"OUTP 1;VOLT {round(self.Ib_sec * 1000)};PHAS {self._norm_angle(getattr(self, 'phb', vB_target if vB_target is not None else 120.0))};"
            else:
                self.Ib_pri = self.scale_current(self.Ib_sec)
                cmds["Ib"] = f"OUTP 1;CURR {self.Ib_pri};PHAS {self._norm_angle(getattr(self, 'phb', vB_target if vB_target is not None else 120.0))};"
        else:
            cmds["Ib"] = "OUTP 0;CURR 0;"

        if iC_on:
            if 0.0 < self.Ic_sec <= 0.24:
                cmds["Ic"] = f"OUTP 1;VOLT {round(self.Ic_sec * 1000)};PHAS {self._norm_angle(getattr(self, 'phc', vC_target if vC_target is not None else 240.0))};"
            else:
                self.Ic_pri = self.scale_current(self.Ic_sec)
                cmds["Ic"] = f"OUTP 1;CURR {self.Ic_pri};PHAS {self._norm_angle(getattr(self, 'phc', vC_target if vC_target is not None else 240.0))};"
        else:
            cmds["Ic"] = "OUTP 0;CURR 0;"

        # --- Final safety enforcement: if *_sec == 0, force OUTP 0 regardless of earlier string content ---
        cmds["Ia"] = _enforce_current_outp(cmds.get("Ia", ""), iA_on)
        cmds["Ib"] = _enforce_current_outp(cmds.get("Ib", ""), iB_on)
        cmds["Ic"] = _enforce_current_outp(cmds.get("Ic", ""), iC_on)

        # ---------------- send commands ----------------
        voltage_IsOn = True  # we always enable voltage OUTP
        current_IsOn = iA_on or iB_on or iC_on

        for key, cmd in cmds.items():
            if self.test_aborted:
                self.append_log("Aborting source activation mid-loop.", "neutral")
                return

            port = self.serial_connections.get(key)
            if port and port.isOpen():
                try:
                    # ensure one trailing ';' before newline
                    out = cmd.strip()
                    if not out.endswith(";"):
                        out += ";"
                    port.write((out + "\n").encode())
                    self.append_log(f"Sent to {key}: {out}", "positive")
                    QThread.msleep(250)
                except Exception as e:
                    self.append_log(f"Error sending to {key}: {e}", "negative")
            else:
                self.append_log(f"{key} port not open or not connected.", "negative")

        self.set_voltage_button_color("red" if voltage_IsOn else "grey")
        self.set_current_button_color("orange" if current_IsOn else "grey")

        try:
            self._last_set = self._source_signature()
        except Exception:
            pass

    def source_correction(self):
    """
    Closed-loop trim with IMET semantics and live metrics table updates.
    Faster, but preserves the original flow and UI timing enough to avoid early starts.
    It ALWAYS writes IMET values into table_metrics on every read so you can watch convergence.

    IMET:
      v_ang_meas = absolute V angle (deg)
      i_ang_meas = relative I angle (deg) = (I_abs − V_abs) mapped to (−180,180]

    Flow (unchanged):
      Pass 1: magnitudes (skip scripted zeros).
      Pass 2: angles → Voltage(B,C) absolute; Current(A,B,C) relative to V.
      Stop when |err| ≤ PHASE_TOL_DEG or attempts exhausted.
    """
    # ------------------- CONFIG -------------------
    V_ABS_MAX = getattr(self, "V_ABS_MAX", 300.0)
    I_ABS_MAX = getattr(self, "I_ABS_MAX", 600.0)
    V_MIN_CMD = 0.0
    I_MIN_CMD = 0.0

    # magnitude control (same spirit)
    KV = 0.6
    KI = 0.6
    V_MAX_STEP_ABS = 10.0
    I_MAX_STEP_ABS = 10.0
    V_MAX_STEP_PCT = 0.20
    I_MAX_STEP_PCT = 0.20
    V_ABS_TOL = 0.10  # volts
    I_ABS_TOL = 0.10  # amps

    # angle control
    PHASE_TOL_DEG   = 2.0
    VOLT_ANG_ATTEMPTS = 8
    I_ANG_ATTEMPTS    = 8
    # settles trimmed but conservative so table fills reliably
    VOLT_SETTLE_MS = 500
    CURR_SETTLE_MS = 700

    # magnitude pass
    MAG_ATTEMPTS  = 2
    MAG_SETTLE_MS = 350

    # ------------------- GUARDS -------------------
    if getattr(self, "_skip_full_setup", False):
        self.append_log("Skip flag set → bypassing source correction.", "neutral")
        self.check_pause_time()
        return
    if getattr(self, "test_aborted", False):
        self.append_log("Source correction skipped due to test abort.", "neutral")
        return

    import re, math
    from PyQt6.QtCore import QTimer, Qt
    from PyQt6.QtWidgets import QTableWidgetItem

    # ------------------- HELPERS -------------------
    def _wrap_pm180(x):  # (-180,180]
        return (x + 180.0) % 360.0 - 180.0

    def _wrap_err_abs(measured_abs, target_abs):
        m = self._norm_angle(measured_abs)
        t = self._norm_angle(target_abs)
        return _wrap_pm180(t - m)

    def _clamp(x, lo, hi): 
        return max(lo, min(hi, x))

    def _slew_limit(old, target, max_abs, max_pct):
        step = target - old
        cap = float('inf')
        if max_abs is not None: cap = min(cap, abs(max_abs))
        if max_pct is not None:
            base = abs(old) if old != 0 else abs(target)
            cap = min(cap, base * max_pct)
        if cap != float('inf'):
            step = _clamp(step, -cap, cap)
        return old + step

    def _safe_mag(target, lo, hi):
        try:
            if target is None or math.isnan(target) or math.isinf(target):
                return lo
            return _clamp(float(target), lo, hi)
        except Exception:
            return lo

    def _cell_text(row, col):
        try:
            it = self.ui.table_script.item(row, col)
            return it.text().strip() if (it and it.text()) else ""
        except Exception:
            return ""

    def _cell_float(row, col, default=None):
        s = _cell_text(self.current_step, col)
        if s == "":
            return default
        try:
            return float(s)
        except Exception:
            return default

    def _kick_imet_read(imet_num, on_ready, wait_ms=280):
        """Send READ and callback soon; slight delay so reply buffers."""
        if self.test_aborted:
            return
        try:
            if not self.serial_port.isOpen():
                self.serial_port.setPortName(self.arduino)
                if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                    self.append_log("ERROR: Could not open Arduino port for RX command", "negative")
                    QTimer.singleShot(wait_ms, lambda: on_ready(None))
                    return
            cmd = f"scpi,RX,'READ:IMET{imet_num}?',"
            self.serial_port.write(cmd.encode())
        except Exception as e:
            self.append_log(f"Failed to send RX command: {e}", "negative")
            QTimer.singleShot(wait_ms, lambda: on_ready(None))
            return
        QTimer.singleShot(wait_ms, on_ready)

    def _read_imet_parsed():
        """
        Read buffered RX text and parse most recent IMET tuple.
        Returns (p, v_mag, i_mag, v_ang_abs, i_ang_rel) or None.
        """
        try:
            response = ""
            while self.serial_port.canReadLine():
                line = self.serial_port.readLine().data().decode("utf-8", errors="replace").strip()
                response += line
            # also drain any remaining bytes
            try:
                while self.serial_port.bytesAvailable():
                    response += self.serial_port.read(self.serial_port.bytesAvailable()).data().decode("utf-8", errors="replace")
            except Exception:
                pass

            # optional: mirror RX into a visible textbox like your other paths do
            if response.strip():
                try:
                    self._paste_rx_text(response.strip(), header="--- IMET RX (correction) ---")
                except Exception:
                    pass

            m = re.search(r"(VUNDer|CUNDer|OK|CRTRansient|COVer)[,~]\(([^)]+)\)", response)
            if not m or len(response) < 30:
                return None
            vals = re.split(r'[~,]', m.group(2))
            if len(vals) < 5:
                return None

            p   = float(vals[0]); v = float(vals[1]); i = float(vals[2])
            v_a = float(vals[3]); i_r = _wrap_pm180(float(vals[4]))
            return p, v, i, v_a, i_r
        except Exception:
            return None

    def _update_metrics_row(phase_label, tup):
        """
        Write (p,v,i,v_abs,i_rel) into the metrics table for row A/B/C.
        """
        if tup is None:
            return
        p, v, i, v_abs, i_rel = tup
        row_index = {'A': 0, 'B': 1, 'C': 2}[phase_label]
        for col, val in enumerate((p, v, i, v_abs, i_rel)):
            try:
                item = QTableWidgetItem(f"{val:.4f}")
                item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                self.ui.table_metrics.setItem(row_index, col, item)
            except Exception:
                pass

    # absolute V target from script
    def _v_target_abs(phase_label):
        if phase_label == 'A':
            return 0.0
        elif phase_label == 'B':
            val = _cell_float(self.current_step, 7, 120.0)
            return self._norm_angle(0.0 if val is None else val)
        else:
            val = _cell_float(self.current_step, 8, 240.0)
            return self._norm_angle(0.0 if val is None else val)

    # absolute I target (blank → in-phase with V target)
    def _i_target_abs(phase_label, v_target_abs):
        col = 12 if phase_label == 'A' else (13 if phase_label == 'B' else 14)
        s = _cell_text(self.current_step, col)
        if s == "":
            return v_target_abs
        try:
            return self._norm_angle(float(s))
        except Exception:
            return v_target_abs

    # magnitudes expected from script or *_sec fallback
    def _expected_v_i(phase_label, voltage_col, idx):
        v_txt = _cell_text(self.current_step, voltage_col)
        if v_txt == "":
            v_fallback = getattr(self, f"v{phase_label}_sec", 0.0)
            try: exp_v = float(v_fallback if v_fallback is not None else 0.0)
            except Exception: exp_v = 0.0
        else:
            try: exp_v = float(v_txt)
            except Exception: exp_v = float(getattr(self, f"v{phase_label}_sec", 0.0) or 0.0)

        i_txt = _cell_text(self.current_step, 9 + idx)
        if i_txt == "":
            i_fallback = getattr(self, f"I{phase_label.lower()}_sec", 0.0)
            try: exp_i = float(i_fallback if i_fallback is not None else 0.0)
            except Exception: exp_i = 0.0
        else:
            try: exp_i = float(i_txt)
            except Exception: exp_i = float(getattr(self, f"I{phase_label.lower()}_sec", 0.0) or 0.0)

        return exp_v, exp_i

    phases = [
        ('A', 1, 'Ia', 'Va', 'pha', 4, 12),
        ('B', 2, 'Ib', 'Vb', 'phb', 5, 13),
        ('C', 3, 'Ic', 'Vc', 'phc', 6, 14),
    ]

    # ------------------- PASS 1: MAGNITUDES -------------------
    self.append_log("Pass 1: correcting magnitudes (fast, with live metrics) A→B→C…", "neutral")

    def pass1_correct_phase(idx):
        if self.test_aborted: return
        if idx >= len(phases):
            self.append_log("Pass 1 complete. Starting Pass 2 (angles).", "neutral")
            QTimer.singleShot(220, lambda: pass2_correct_phase(0))
            return

        phase_label, imet_num, *_rest, voltage_col, _ = phases[idx]
        attempts = {"v": 0, "i": 0}
        stage = {"s": "V"}

        def tick(): _kick_imet_read(imet_num, on_ready, wait_ms=280)

        def on_ready(_=None):
            if self.test_aborted: return
            parsed = _read_imet_parsed()
            # always push whatever we read into the metrics table
            _update_metrics_row(phase_label, parsed)

            if parsed is None:
                self.append_log(f"IMET parse failed (Phase {phase_label}) — skipping to next.", "negative")
                QTimer.singleShot(120, lambda: pass1_correct_phase(idx + 1))
                return

            _, v_meas_mag, i_meas_mag, _, _ = parsed
            exp_v, exp_i = _expected_v_i(phase_label, voltage_col, idx)

            v_set = getattr(self, f"v{phase_label}_sec", exp_v)
            i_set = getattr(self, f"I{phase_label.lower()}_sec", exp_i)

            # skip V magnitude if scripted 0
            if exp_v <= 0.0:
                stage["s"] = "I"

            if stage["s"] == "V":
                v_err = abs(exp_v - v_meas_mag)
                if v_err <= V_ABS_TOL or attempts["v"] >= MAG_ATTEMPTS:
                    self.append_log(f"{phase_label} V OK/skip (err={v_err:.3f} ≤ {V_ABS_TOL:.3f}).", "neutral")
                    stage["s"] = "I"; QTimer.singleShot(150, tick); return

                attempts["v"] += 1
                v_target = _safe_mag(v_set + KV * (exp_v - v_meas_mag), V_MIN_CMD, V_ABS_MAX)
                v_new = _safe_mag(_slew_limit(v_set, v_target, V_MAX_STEP_ABS, V_MAX_STEP_PCT), V_MIN_CMD, V_ABS_MAX)
                if v_new > V_ABS_MAX:
                    self.append_log(f"Safety abort: V set {v_new:.2f} > {V_ABS_MAX}", "negative"); self.abort_test(); return
                setattr(self, f"v{phase_label}_sec", v_new)
                self.send_source_commands()
                self.append_log(f"{phase_label} V step → {v_new:.3f} V (try {attempts['v']}/{MAG_ATTEMPTS})", "neutral")
                QTimer.singleShot(MAG_SETTLE_MS, tick); return

            # skip I magnitude if scripted 0
            if exp_i <= 0.0:
                self.append_log(f"{phase_label} I skipped (scripted 0).", "neutral")
                QTimer.singleShot(120, lambda: pass1_correct_phase(idx + 1)); return

            i_err = abs(exp_i - i_meas_mag)
            if i_err <= I_ABS_TOL or attempts["i"] >= MAG_ATTEMPTS:
                self.append_log(f"{phase_label} I OK/skip (err={i_err:.3f} ≤ {I_ABS_TOL:.3f}).", "neutral")
                QTimer.singleShot(120, lambda: pass1_correct_phase(idx + 1)); return

            attempts["i"] += 1
            if exp_i < 0.5:
                i_target = max(0.01, 0.9 * exp_i + 0.1 * i_meas_mag)
            else:
                i_target = i_set + KI * (exp_i - i_meas_mag)
            i_target = _safe_mag(i_target, I_MIN_CMD, I_ABS_MAX)
            i_new = _safe_mag(_slew_limit(i_set, i_target, I_MAX_STEP_ABS, I_MAX_STEP_PCT), I_MIN_CMD, I_ABS_MAX)
            if i_new > I_ABS_MAX:
                self.append_log(f"Safety abort: I set {i_new:.2f} > {I_ABS_MAX}", "negative"); self.abort_test(); return
            setattr(self, f"I{phase_label.lower()}_sec", i_new)
            self.send_source_commands()
            self.append_log(f"{phase_label} I step → {i_new:.3f} A (try {attempts['i']}/{MAG_ATTEMPTS})", "neutral")
            QTimer.singleShot(MAG_SETTLE_MS, tick)

        tick()

    # ------------------- PASS 2: ANGLES -------------------
    def pass2_correct_phase(idx):
        if self.test_aborted: return
        if idx >= len(phases):
            self.source_corrected_values = True
            self.append_log("Source correction complete (magnitudes + angles).", "positive")
            self.read_final_imet_snapshot()
            return

        phase_label, imet_num, *_rest, voltage_col, _ = phases[idx]
        attempts = {"vang": 0, "iang": 0}

        V_target_abs = self._norm_angle(_v_target_abs(phase_label))
        exp_v, exp_i = _expected_v_i(phase_label, voltage_col, idx)
        I_target_abs = self._norm_angle(_i_target_abs(phase_label, V_target_abs))
        delta_target = _wrap_pm180(I_target_abs - V_target_abs)

        # Voltage (B,C only)
        def trim_voltage():
            if self.test_aborted: return

            def _after_read(_=None):
                parsed = _read_imet_parsed()
                _update_metrics_row(phase_label, parsed)

                if parsed is None:
                    if attempts["vang"] < VOLT_ANG_ATTEMPTS:
                        attempts["vang"] += 1
                        self.append_log(f"{phase_label} V-angle read failed — retry {attempts['vang']}/{VOLT_ANG_ATTEMPTS}.", "negative")
                        QTimer.singleShot(VOLT_SETTLE_MS, trim_voltage); return
                    else:
                        self.append_log(f"{phase_label} V-angle read failed after retries; continuing.", "negative")
                        proceed_current(); return

                _, _, _, v_meas_abs, _ = parsed

                if exp_v <= 0.0 or phase_label == 'A':
                    proceed_current(); return

                err = _wrap_err_abs(v_meas_abs, V_target_abs)
                if abs(err) <= PHASE_TOL_DEG:
                    self.append_log(f"{phase_label} V angle OK (target {V_target_abs:.2f}°, meas {v_meas_abs:.2f}°, err {err:+.2f}°).", "positive")
                    proceed_current(); return

                # Incremental voltage command: cmd += err
                if phase_label == 'B':
                    cur_cmd = self._norm_angle(getattr(self, 'pab', V_target_abs))
                    self.pab = self._norm_angle(cur_cmd + err)
                    new_cmd = self.pab
                else:  # 'C'
                    cur_cmd = self._norm_angle(getattr(self, 'pac', V_target_abs))
                    self.pac = self._norm_angle(cur_cmd + err)
                    new_cmd = self.pac

                self.append_log(f"{phase_label} V-angle adjust → PHAS {new_cmd:.2f}° (err {err:+.2f}°).", "neutral")
                self.send_source_commands()

                if attempts["vang"] >= VOLT_ANG_ATTEMPTS:
                    self.append_log(f"{phase_label} V angle not within ±{PHASE_TOL_DEG}° after retries; continuing.", "negative")
                    proceed_current(); return

                attempts["vang"] += 1
                QTimer.singleShot(VOLT_SETTLE_MS, trim_voltage)

            _kick_imet_read(imet_num, _after_read, wait_ms=300)

        # Current relative to V (A,B,C)
        def trim_current():
            if self.test_aborted: return

            def _after_read(_=None):
                parsed = _read_imet_parsed()
                _update_metrics_row(phase_label, parsed)

                if parsed is None:
                    if attempts["iang"] < I_ANG_ATTEMPTS:
                        attempts["iang"] += 1
                        self.append_log(f"{phase_label} I-angle read failed — retry {attempts['iang']}/{I_ANG_ATTEMPTS}.", "negative")
                        QTimer.singleShot(CURR_SETTLE_MS, trim_current); return
                    else:
                        self.append_log(f"{phase_label} I-angle read failed after retries; moving on.", "negative")
                        QTimer.singleShot(200, lambda: pass2_correct_phase(idx + 1)); return

                _, _, _, v_meas_abs, i_meas_rel = parsed

                if exp_i <= 0.0:
                    self.append_log(f"{phase_label} I-angle skipped (scripted I=0).", "neutral")
                    QTimer.singleShot(200, lambda: pass2_correct_phase(idx + 1)); return

                err_rel = _wrap_pm180(delta_target - i_meas_rel)
                if abs(err_rel) <= PHASE_TOL_DEG:
                    self.append_log(
                        f"{phase_label} I−V angle OK (target {delta_target:+.2f}°, meas {i_meas_rel:+.2f}°, err {err_rel:+.2f}°).",
                        "positive"
                    )
                    QTimer.singleShot(200, lambda: pass2_correct_phase(idx + 1)); return

                # Incremental on CURRENT command: cmd += err_rel
                if phase_label == 'A':
                    cur_cmd = self._norm_angle(getattr(self, 'pha', I_target_abs))
                    self.pha = self._norm_angle(cur_cmd + err_rel)
                    new_cmd = self.pha
                elif phase_label == 'B':
                    cur_cmd = self._norm_angle(getattr(self, 'phb', I_target_abs))
                    self.phb = self._norm_angle(cur_cmd + err_rel)
                    new_cmd = self.phb
                else:
                    cur_cmd = self._norm_angle(getattr(self, 'phc', I_target_abs))
                    self.phc = self._norm_angle(cur_cmd + err_rel)
                    new_cmd = self.phc

                self.append_log(
                    f"{phase_label} I-angle adjust (relative) → PHAS {new_cmd:.2f}° "
                    f"[Δtarget {delta_target:+.2f}°, Δmeas {i_meas_rel:+.2f}°, err {err_rel:+.2f}°].",
                    "neutral"
                )
                self.send_source_commands()

                if attempts["iang"] >= I_ANG_ATTEMPTS:
                    self.append_log(f"{phase_label} I−V angle not within ±{PHASE_TOL_DEG}° after retries.", "negative")
                    QTimer.singleShot(200, lambda: pass2_correct_phase(idx + 1)); return

                attempts["iang"] += 1
                QTimer.singleShot(CURR_SETTLE_MS, trim_current)

            _kick_imet_read(imet_num, _after_read, wait_ms=320)

        def proceed_current():
            trim_current()

        trim_voltage()

    # kick off
    pass1_correct_phase(0)
class ManualControlWindow(QWidget):
    
    # === Initializing arduino and source ports and serial comms === #
    def __init__(self, source_ports, arduino_port, main_window_ref=None):
        super().__init__()  # Do NOT pass parent to QWidget
        self.ui = Ui_Form()
        self.ui.setupUi(self)

        self.MainWindow = main_window_ref  # Manual reference, not Qt parenting
        self.arduino = arduino_port
        self.source_ports = source_ports
        self.serial_connections = {}

        for key, port in self.source_ports.items():
            connection = serial_manager.open_port(key, port)
            if connection:
                self.serial_connections[key] = connection
                self.append_log(f"{key} connected on {port}", "positive")
            else:
                self.append_log(f"Failed to open {key} on {port}", "negative")

        self.serial_port = QSerialPort()
        self.serial_port.setBaudRate(115200)

        self.ui.TB_va.setText("120")
        self.ui.TB_vb.setText("120")
        self.ui.TB_vc.setText("120")
        self.ui.TB_vaAngle.setText("0")
        self.ui.TB_vbAngle.setText("120")
        self.ui.TB_vcAngle.setText("240")
        self.ui.TB_ia.setText("1.5")
        self.ui.TB_ib.setText("1.5")
        self.ui.TB_ic.setText("1.5")
        self.ui.TB_iaAngle.setText("0")
        self.ui.TB_ibAngle.setText("120")
        self.ui.TB_icAngle.setText("240")

        self.ui.pB_creep.clicked.connect(lambda: self.send_creep_command())
        self.ui.pB_jog.clicked.connect(lambda: self.send_jog_command())
        self.ui.pB_sendSCPI.clicked.connect(lambda: self.send_custom_command())
        self.ui.pB_toMain.clicked.connect(self.close)
        self.ui.pB_off.clicked.connect(lambda: self.send_manual_command("OUTP 0"))

    def closeEvent(self, event):
        try:
            self.send_manual_command("OUTP 0")
        except Exception as e:
            print(f"Error sending OUTP 0 during Manual Control close: {e}")

        try:
            if self.MainWindow:
                # Pass back actual open handles
                self.MainWindow.serial_connections = self.serial_connections
                self.MainWindow.Va = self.source_ports.get("Va")
                self.MainWindow.Vb = self.source_ports.get("Vb")
                self.MainWindow.Vc = self.source_ports.get("Vc")
                self.MainWindow.Ia = self.source_ports.get("Ia")
                self.MainWindow.Ib = self.source_ports.get("Ib")
                self.MainWindow.Ic = self.source_ports.get("Ic")
                self.MainWindow.arduino = self.arduino

                # Re-enable buttons
                self.MainWindow.test_active = False
                self.MainWindow.manual_control_active = False
                self.MainWindow.protect_disable_buttons()

                if hasattr(self.MainWindow, "append_log"):
                    self.MainWindow.append_log("Manual Control closed safely and ports returned.", "neutral")
        except Exception as e:
            print(f"Error updating parent ports: {e}")

        event.accept()

    def source_command_builder(self, VaCheck, VbCheck, VcCheck, IaCheck, IbCheck, IcCheck, RXcheck, ard_command):
        """Builds SCPI command strings per source for direct serial communication and handles ranging for current, and scaling factors for voltage AND current."""

        def scale_current(value_str):
            try:
                val = float(value_str)
                if 0.0 <= val <= 1.5:
                    return round(val / 10, 6)
                elif 1.501 <= val <= 5.0:
                    return round(val / 10, 6)
                elif 5.01 <= val <= 10.0:
                    return round(val / 10, 6)
                elif 10.001 <= val <= 20.0:
                    return round(val / 20, 6)
                elif 20.001 <= val <= 40.0:
                    return round(val / 20, 6)
                elif 40.001 <= val <= 600.0:
                    return round(val / 78, 6)
                else:
                    return val
            except ValueError:
                return value_str

        def scale_voltage(value_str):
            try:
                val = float(value_str)
                return round(val / 2, 6)
            except ValueError:
                return value_str

        try:
            ard_command = ard_command.upper()
            if ard_command in ["OUTPUT 1", "OUTPUT ON", "OUTP ON"]:
                ard_command = "OUTP 1"
            elif ard_command in ["OUTPUT 0", "OUTPUT OFF", "OUTP OFF"]:
                ard_command = "OUTP 0"

            scale = ard_command == "OUTP 1"

            cmd_Va = cmd_Vb = cmd_Vc = cmd_Ia = cmd_Ib = cmd_Ic = ""

            if VaCheck:
                volt = scale_voltage(self.ui.TB_va.toPlainText()) if scale else self.ui.TB_va.toPlainText()
                cmd_Va = f"{ard_command};VOLT {volt};PHAS {self.ui.TB_vaAngle.toPlainText()};"
            if VbCheck:
                volt = scale_voltage(self.ui.TB_vb.toPlainText()) if scale else self.ui.TB_vb.toPlainText()
                cmd_Vb = f"{ard_command};VOLT {volt};PHAS {self.ui.TB_vbAngle.toPlainText()};"
            if VcCheck:
                volt = scale_voltage(self.ui.TB_vc.toPlainText()) if scale else self.ui.TB_vc.toPlainText()
                cmd_Vc = f"{ard_command};VOLT {volt};PHAS {self.ui.TB_vcAngle.toPlainText()};"

            if IaCheck:
                curr_val_str = self.ui.TB_ia.toPlainText()
                curr_val = float(curr_val_str) if curr_val_str else 0.0
                angle = self.ui.TB_iaAngle.toPlainText()
                if scale and 0.0 < curr_val <= 0.24:
                    volt_cmd = round(curr_val * 1000)
                    cmd_Ia = f"{ard_command};VOLT {volt_cmd};PHAS {angle};"
                else:
                    curr = scale_current(curr_val_str) if scale else curr_val_str
                    cmd_Ia = f"{ard_command};CURR {curr};PHAS {angle};"

            if IbCheck:
                curr_val_str = self.ui.TB_ib.toPlainText()
                curr_val = float(curr_val_str) if curr_val_str else 0.0
                angle = self.ui.TB_ibAngle.toPlainText()
                if scale and 0.0 < curr_val <= 0.24:
                    volt_cmd = round(curr_val * 1000)
                    cmd_Ib = f"{ard_command};VOLT {volt_cmd};PHAS {angle};"
                else:
                    curr = scale_current(curr_val_str) if scale else curr_val_str
                    cmd_Ib = f"{ard_command};CURR {curr};PHAS {angle};"

            if IcCheck:
                curr_val_str = self.ui.TB_ic.toPlainText()
                curr_val = float(curr_val_str) if curr_val_str else 0.0
                angle = self.ui.TB_icAngle.toPlainText()
                if scale and 0.0 < curr_val <= 0.24:
                    volt_cmd = round(curr_val * 1000)
                    cmd_Ic = f"{ard_command};VOLT {volt_cmd};PHAS {angle};"
                else:
                    curr = scale_current(curr_val_str) if scale else curr_val_str
                    cmd_Ic = f"{ard_command};CURR {curr};PHAS {angle};"

            self.append_log(f"Builder result: Va={cmd_Va}, Vb={cmd_Vb}, Vc={cmd_Vc}, Ia={cmd_Ia}, Ib={cmd_Ib}, Ic={cmd_Ic}", "neutral")

            return cmd_Va, cmd_Vb, cmd_Vc, cmd_Ia, cmd_Ib, cmd_Ic

        except Exception as ex:
            self.append_log(f"Error building SCPI commands: {str(ex)}", "negative")
            return "", "", "", "", "", ""


    # === Sends SCPI commands to turn on voltage sources that are checkmarked. Does not activate any of the currents. === #
    def send_creep_command(self):
        """Sends OUTP 1 command to enabled voltage sources (Va, Vb, Vc)."""
        # === Early exit if no voltage checkboxes are selected === #
        if not (self.ui.cB_va.isChecked() or self.ui.cB_vb.isChecked() or self.ui.cB_vc.isChecked()):
            self.append_log("Creep aborted: No voltage channels selected.", "warning")
            return

        if not self.source_value_check():
            return
        

        try:
            # === Builds the SCPI commands in source_command_builder before sending them, sets all currents to false so they do not activate === #
            cmds = self.source_command_builder(
                VaCheck=self.ui.cB_va.isChecked(),
                VbCheck=self.ui.cB_vb.isChecked(),
                VcCheck=self.ui.cB_vc.isChecked(),
                IaCheck=False, IbCheck=False, IcCheck=False,
                RXcheck=False,
                ard_command="OUTP 1"
            )

            keys = ["Va", "Vb", "Vc"]
            for i, key in enumerate(keys):
                cmd = cmds[i]
                if cmd:
                    port = self.serial_connections.get(key)
                    if port and port.isOpen():
                        port.write(f"{cmd}\n".encode())
                        self.append_log(f"Sent to {key}: {cmd}", "positive")
                    else:
                        self.append_log(f"{key} not connected or port not open.", "negative")

            self.append_log("Creep command sent.", "neutral")

            # === Set green status indicators for voltage sources === #
            self.update_label_colors(update_current=False, color_mode="on")

        except Exception as ex:
            self.append_log(f"Error in creep command: {str(ex)}", "negative")
            

    def send_jog_command(self):
        if not (self.ui.cB_ia.isChecked() or self.ui.cB_ib.isChecked() or self.ui.cB_ic.isChecked()):
            self.append_log("Jog aborted: No current channels selected.", "warning")
            return

        try:
            self.jog_Ia = float(self.ui.TB_ia.toPlainText().strip()) if self.ui.cB_ia.isChecked() else 0.0
            self.jog_Ib = float(self.ui.TB_ib.toPlainText().strip()) if self.ui.cB_ib.isChecked() else 0.0
            self.jog_Ic = float(self.ui.TB_ic.toPlainText().strip()) if self.ui.cB_ic.isChecked() else 0.0

            if self.jog_Ia <= 0 and self.jog_Ib <= 0 and self.jog_Ic <= 0:
                self.append_log("Jog aborted: All selected current setpoints are 0 or invalid.", "warning")
                return

            self.max_jog_current = max(self.jog_Ia, self.jog_Ib, self.jog_Ic)

            # === Step 1: Force OFF all outputs ===
            for key in ["Ia", "Ib", "Ic"]:
                port = self.serial_connections.get(key)
                if port and port.isOpen():
                    port.write(b"OUTP 0;CURR 0;PHAS 0;\n")
                    self.append_log(f"Forced OFF to {key}", "neutral")
                else:
                    self.append_log(f"{key} not connected or port not open for force-OFF.", "negative")

            self.append_log("Waiting for current sources to power down (non-blocking)...", "neutral")

            # === Step 2: Delay, then call prep logic and continue ===
            QTimer.singleShot(200, self._prep_and_continue_jog)

        except Exception as ex:
            self.append_log(f"Jog init error: {str(ex)}", "negative")
            QMessageBox.critical(self, "Jog Init Error", f"Jog startup failed: {str(ex)}")


    def _prep_and_continue_jog(self):
        from PyQt6.QtCore import QTimer

        prep_channels = []

        if self.jog_Ia > 0:
            prep_channels.append(("Ia", self.jog_Ia))
        if self.jog_Ib > 0:
            prep_channels.append(("Ib", self.jog_Ib))
        if self.jog_Ic > 0:
            prep_channels.append(("Ic", self.jog_Ic))

        delay_step = 200  # ms between each command
        delay_counter = 0

        for key, current_value in prep_channels:
            port = self.serial_connections.get(key)
            if not (port and port.isOpen()):
                self.append_log(f"{key} port not open for jog prep.", "negative")
                continue

            if current_value <= 0.24:
                self.append_log(f"Sending low-current setup to {key}", "neutral")
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → CURR:PROT:STAT ON", "neutral"), p.write(b"CURR:PROT:STAT ON\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → VOLT:RANG 400", "neutral"), p.write(b"VOLT:RANG 400\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → CURR 0.004", "neutral"), p.write(b"CURR 0.004\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda k=key: self.append_log(f"Low-current jog prep complete for {k}", "positive"))
                delay_counter += delay_step
            else:
                self.append_log(f"Sending standard-current setup to {key}", "neutral")
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → CURR:PROT:STAT OFF", "neutral"), p.write(b"CURR:PROT:STAT OFF\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → VOLT:RANG 200", "neutral"), p.write(b"VOLT:RANG 200\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → VOLT 100", "neutral"), p.write(b"VOLT 100\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda k=key: self.append_log(f"Standard-current jog prep complete for {k}", "positive"))
                delay_counter += delay_step

        QTimer.singleShot(delay_counter + 500, self._continue_jog_ct_setup)



    def _continue_jog_ct_setup(self):
        from CT_ranging_code import set_current, setup as ct_setup

        try:
            ct_setup()

            # Conditionally send set_current based on checkbox state and current value
            if self.ui.cB_ia.isChecked() and self.jog_Ia > 0:
                set_current('a', self.jog_Ia)
            if self.ui.cB_ib.isChecked() and self.jog_Ib > 0:
                set_current('b', self.jog_Ib)
            if self.ui.cB_ic.isChecked() and self.jog_Ic > 0:
                set_current('c', self.jog_Ic)

            self.append_log("CT ranging completed.", "positive")

            QTimer.singleShot(100, self._continue_jog_ct_routing)  # Brief delay before routing

        except Exception as e:
            self.append_log(f"CT Ranging I2C Error: {e}", "negative")
            QMessageBox.critical(self, "CT Ranging Error", f"CT ranging failed: {e}")


    def _continue_jog_ct_routing(self):
        from smbus2 import SMBus
        import time  # Only for internal short waits
        from gpiozero import DigitalInputDevice
        from PyQt6.QtCore import QTimer

        I2C_BUS = 1
        ROUTING_ADDR = 0x3A
        gpio5 = DigitalInputDevice(5)
        gpio6 = DigitalInputDevice(6)

        # --- helper: read typed current from UI, fallback to jog_*, clamp to >= 0 ---
        def typed_current(phase_key: str, jog_fallback: float) -> float:
            try:
                tb = {"Ia": self.ui.TB_ia, "Ib": self.ui.TB_ib, "Ic": self.ui.TB_ic}[phase_key]
                txt = (tb.toPlainText() or "").strip()
                return max(0.0, float(txt if txt != "" else jog_fallback))
            except Exception:
                return max(0.0, float(jog_fallback or 0.0))

        # --- ensure QSerialPort is open and pointed at the Arduino port name (self.arduino) ---
        def ensure_arduino_port_open() -> bool:
            try:
                if not hasattr(self, "serial_port"):
                    return False
                if (not self.serial_port.isOpen()) or (self.serial_port.portName() != str(self.arduino)):
                    if self.serial_port.isOpen():
                        self.serial_port.close()
                    self.serial_port.setPortName(str(self.arduino))
                    if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                        return False
                return True
            except Exception:
                return False

        try:
            with SMBus(I2C_BUS) as bus:
                gpio5_state = gpio5.value
                gpio6_state = gpio6.value

                current = self.max_jog_current

                def routing_error(msg):
                    QMessageBox.critical(self, "Routing Error", msg)
                    raise Exception(msg)

                if current <= 5.0:
                    if not (gpio6_state == 1 and gpio5_state == 0):
                        bus.write_byte(ROUTING_ADDR, 0xFA)
                        time.sleep(0.5)
                        bus.write_byte(ROUTING_ADDR, 0xFF)
                        time.sleep(7)
                        bus.write_byte(ROUTING_ADDR, 0xF7)
                        time.sleep(0.5)
                        bus.write_byte(ROUTING_ADDR, 0xFF)
                        if not (gpio6.value == 1 and gpio5.value == 0):
                            routing_error("Expected 0–5A config: GPIO6 HIGH, GPIO5 LOW")
                elif current <= 320.0:
                    if not (gpio5_state == 1 and gpio6_state == 0):
                        bus.write_byte(ROUTING_ADDR, 0xFB)
                        time.sleep(0.5)
                        bus.write_byte(ROUTING_ADDR, 0xFF)
                        time.sleep(7)
                        bus.write_byte(ROUTING_ADDR, 0xF6)
                        time.sleep(0.5)
                        bus.write_byte(ROUTING_ADDR, 0xFF)
                        if not (gpio5.value == 1 and gpio6.value == 0):
                            routing_error("Expected 5–320A config: GPIO5 HIGH, GPIO6 LOW")
                else:
                    if not (gpio5_state == 0 and gpio6_state == 0):
                        bus.write_byte(ROUTING_ADDR, 0xFA)
                        time.sleep(0.5)
                        bus.write_byte(ROUTING_ADDR, 0xFF)
                        time.sleep(7)
                        bus.write_byte(ROUTING_ADDR, 0xFB)
                        time.sleep(0.5)
                        bus.write_byte(ROUTING_ADDR, 0xFF)
                        if not (gpio5.value == 0 and gpio6.value == 0):
                            routing_error("Expected 320+A config: both GPIO5 and GPIO6 LOW")

            # === After routing: send clamp enable/disable to Arduino over self.serial_port ===
            if not ensure_arduino_port_open():
                self.append_log("Arduino port not open (QSerialPort) for clamp config.", "negative")
                QTimer.singleShot(1000, self._continue_jog_activate_sources)
                return

            ia_val = typed_current("Ia", getattr(self, "jog_Ia", 0.0))
            ib_val = typed_current("Ib", getattr(self, "jog_Ib", 0.0))
            ic_val = typed_current("Ic", getattr(self, "jog_Ic", 0.0))

            clamp_plan = [
                ("Ia", ia_val, "CLAM1"),
                ("Ib", ib_val, "CLAM2"),
                ("Ic", ic_val, "CLAM3"),
            ]

            # timings
            per_phase_offset = 150   # ms stagger between phases
            verify_gap_1     = 60    # ms after first SET for first ENAB?
            reinforce_gap    = 160   # ms after first SET to send the same SET again (idempotent)
            verify_gap_2     = 260   # ms after first SET for second ENAB?

            for idx, (phase, val, clam_id) in enumerate(clamp_plan):
                # Spec: 0–5.1 => OFF, >5.1 => ON
                enab = "OFF" if (0.0 <= float(val) <= 5.1) else "ON"

                set_cmd    = f"scpi,RX,'CONF:MEAS:{clam_id}:ENAB {enab}',\n".encode()
                query_cmd  = f"scpi,RX,'CONF:MEAS:{clam_id}:ENAB?',\n".encode()

                base = idx * per_phase_offset

                # First SET
                QTimer.singleShot(
                    base,
                    lambda k=phase, v=val, e=enab, cid=clam_id, c=set_cmd: (
                        self.append_log(f"{k} ({v:.4f} A) → {cid}:ENAB {e}", "neutral"),
                        self.serial_port.write(c)
                    )
                )
                # First verification query
                QTimer.singleShot(base + verify_gap_1, lambda q=query_cmd: self.serial_port.write(q))
                # Reinforce SET (idempotent) to ensure final state
                QTimer.singleShot(base + reinforce_gap, lambda c=set_cmd: self.serial_port.write(c))
                # Second verification query
                QTimer.singleShot(base + verify_gap_2, lambda q=query_cmd: self.serial_port.write(q))

            self.append_log("CT routing completed.", "positive")
            QTimer.singleShot(1200 + 3 * per_phase_offset, self._continue_jog_activate_sources)

        except Exception as e:
            self.append_log(f"CT Routing Error: {e}", "negative")
            QMessageBox.critical(self, "CT Routing Error", f"Routing failed: {e}")

    # --- helper: make sure current channels use OUTP 0 when their jog is 0 ---
    def _apply_current_outp_overrides(self, cmds: list[str]) -> list[str]:
        """
        Ensures OUTP 0 is sent for current channels with 0A setpoint (or unchecked).
        If OUTP is already present, it is replaced; otherwise it's prepended.
        Expects cmds ordered as: [Va, Vb, Vc, Ia, Ib, Ic]
        """
        import re

        # Desired OUTP per current channel based on checkbox + jog value
        ia_on = (self.ui.cB_ia.isChecked() and float(getattr(self, "jog_Ia", 0.0)) > 0.0)
        ib_on = (self.ui.cB_ib.isChecked() and float(getattr(self, "jog_Ib", 0.0)) > 0.0)
        ic_on = (self.ui.cB_ic.isChecked() and float(getattr(self, "jog_Ic", 0.0)) > 0.0)

        desired = {
            3: "1" if ia_on else "0",  # Ia
            4: "1" if ib_on else "0",  # Ib
            5: "1" if ic_on else "0",  # Ic
        }

        # Normalize each current command string
        for idx in (3, 4, 5):
            cmd = cmds[idx]
            if not cmd:
                continue
            # If there's already an OUTP token, replace its value
            if re.search(r'\bOUTP\s+[01]\b', cmd, flags=re.IGNORECASE):
                cmd = re.sub(r'\bOUTP\s+[01]\b', f'OUTP {desired[idx]}', cmd, flags=re.IGNORECASE)
            else:
                # Prepend an OUTP state if none exists
                cmd = f"OUTP {desired[idx]};{cmd}"
            cmds[idx] = cmd

        return cmds
    
    def _normalize_and_apply_current_outp(self, cmds_in):
        """
        Returns a LIST of 6 command strings [Va,Vb,Vc,Ia,Ib,Ic],
        ensuring current channels send the correct OUTP (0 if jog == 0 or unchecked).
        Handles tuples/lists/strings/None safely.
        """
        import re

        # 1) Normalize container -> list of strings length 6
        if cmds_in is None:
            cmds = []
        else:
            cmds = list(cmds_in)  # safe copy (tuple->list, list->copy)

        # pad to 6
        while len(cmds) < 6:
            cmds.append("")

        norm = []
        for c in cmds[:6]:
            # Convert element to a single command string
            if isinstance(c, (list, tuple)):
                parts = []
                for x in c:
                    if x is None:
                        continue
                    s = str(x).strip()
                    if s:
                        parts.append(s.strip("; "))
                c = ";".join(parts)
            elif c is None:
                c = ""
            else:
                c = str(c).strip()

            # Clean duplicate semicolons/spaces
            c = c.strip("; ").replace(";;", ";").strip()
            norm.append(c)

        cmds = norm  # mutable list of six strings

        # 2) Decide desired OUTP for Ia/Ib/Ic (indices 3,4,5)
        def fval(v):
            try:
                return float(v)
            except Exception:
                return 0.0

        ia_on = (self.ui.cB_ia.isChecked() and fval(getattr(self, "jog_Ia", 0.0)) > 0.0)
        ib_on = (self.ui.cB_ib.isChecked() and fval(getattr(self, "jog_Ib", 0.0)) > 0.0)
        ic_on = (self.ui.cB_ic.isChecked() and fval(getattr(self, "jog_Ic", 0.0)) > 0.0)

        desired = {
            3: "1" if ia_on else "0",  # Ia
            4: "1" if ib_on else "0",  # Ib
            5: "1" if ic_on else "0",  # Ic
        }

        # 3) Rewrite or insert OUTP token for Ia/Ib/Ic only
        pat = re.compile(r'\bOUTP\s+[01]\b', flags=re.IGNORECASE)
        for idx in (3, 4, 5):
            c = cmds[idx]
            if not c:
                cmds[idx] = f"OUTP {desired[idx]};"
                continue

            if pat.search(c):
                c = pat.sub(f"OUTP {desired[idx]}", c)
            else:
                c = f"OUTP {desired[idx]};{c}"
            cmds[idx] = c.strip("; ")  # keep clean

        return cmds

    def _continue_jog_activate_sources(self):
        try:
            cmds_raw = self.source_command_builder(
                VaCheck=self.ui.cB_va.isChecked(),
                VbCheck=self.ui.cB_vb.isChecked(),
                VcCheck=self.ui.cB_vc.isChecked(),
                IaCheck=self.ui.cB_ia.isChecked(),
                IbCheck=self.ui.cB_ib.isChecked(),
                IcCheck=self.ui.cB_ic.isChecked(),
                RXcheck=False,
                ard_command="OUTP 1"  # OK; current channels will be forced to 0 if jog==0
            )

            # Normalize and enforce OUTP state for current channels
            cmds = self._normalize_and_apply_current_outp(cmds_raw)

            keys = ["Va", "Vb", "Vc", "Ia", "Ib", "Ic"]
            for key, cmd in zip(keys, cmds):
                if not cmd:
                    continue
                # ensure single trailing ';' before newline for cleaner parsers
                cmd_to_send = cmd.strip()
                if not cmd_to_send.endswith(";"):
                    cmd_to_send += ";"

                port = self.serial_connections.get(key)
                if port and port.isOpen():
                    port.write((cmd_to_send + "\n").encode())
                    self.append_log(f"Activated {key}: {cmd_to_send}", "positive")
                else:
                    self.append_log(f"{key} not connected or port not open.", "negative")

            self.append_log("Jog command completed.", "neutral")
            self.update_label_colors(update_current=True, color_mode="on")

        except Exception as e:
            self.append_log(f"Activation Error: {e}", "negative")
            QMessageBox.critical(self, "Activation Error", f"Failed to activate sources: {e}")

    # === Sends OUTP 0 to checkmarked sources === #
    def send_manual_command(self, action_type="OUTP 0"):
        """Sends OUTP 0 to checked voltage/current sources and updates their labels to red."""

        try:
            # === Check which sources are checked === #
            VaCheck = self.ui.cB_va.isChecked()
            VbCheck = self.ui.cB_vb.isChecked()
            VcCheck = self.ui.cB_vc.isChecked()
            IaCheck = self.ui.cB_ia.isChecked()
            IbCheck = self.ui.cB_ib.isChecked()
            IcCheck = self.ui.cB_ic.isChecked()

            # === Manually build simple OFF commands — no need to scale === #
            cmd_Va = f"{action_type};VOLT 0;PHAS 0;" if VaCheck else ""
            cmd_Vb = f"{action_type};VOLT 0;PHAS 0;" if VbCheck else ""
            cmd_Vc = f"{action_type};VOLT 0;PHAS 0;" if VcCheck else ""
            cmd_Ia = f"{action_type};CURR 0;PHAS 0;" if IaCheck else ""
            cmd_Ib = f"{action_type};CURR 0;PHAS 0;" if IbCheck else ""
            cmd_Ic = f"{action_type};CURR 0;PHAS 0;" if IcCheck else ""

            keys = ["Va", "Vb", "Vc", "Ia", "Ib", "Ic"]
            cmds = [cmd_Va, cmd_Vb, cmd_Vc, cmd_Ia, cmd_Ib, cmd_Ic]

            for key, cmd in zip(keys, cmds):
                if cmd:
                    port = self.serial_connections.get(key)
                    if port and port.isOpen():
                        port.write(f"{cmd}\n".encode())
                        self.append_log(f"Sent OFF to {key}: {cmd}", "positive")
                    else:
                        self.append_log(f"{key} not connected or port not open.", "negative")

            self.append_log("OFF command sent.", "neutral")
            self.update_label_colors(update_current=True, color_mode="off")

        except Exception as ex:
            self.append_log(f"Error sending manual OFF command: {str(ex)}", "negative")
            #QMessageBox.critical(self, "Manual Command Error", f"Failed to send OFF command: {str(ex)}")

    # === Turns voltage and current source labels red or green depending on what is activated  === #
    def update_label_colors(self, update_current=True, color_mode="auto"):
        """
        Updates the source button colors.

        Args:
            update_current (bool): If True, update both voltage & current buttons.
            color_mode (str): "on" = force green, "off" = force red, "auto" = based on checkboxes.
        """
        checkbox_button_mapping = {
            self.ui.cB_va: self.ui.pB_vaOff,   # Va
            self.ui.cB_vb: self.ui.pB_vbOff,   # Vb
            self.ui.cB_vc: self.ui.pB_vcOff,   # Vc
        }

        if update_current:
            checkbox_button_mapping.update({
                self.ui.cB_ia: self.ui.pB_iaOff,  # Ia
                self.ui.cB_ib: self.ui.pB_ibOff,  # Ib
                self.ui.cB_ic: self.ui.pB_icOff,  # Ic
            })

        for checkbox, button in checkbox_button_mapping.items():
            if color_mode == "on" and checkbox.isChecked():
                button.setStyleSheet("background-color: green; border-radius: 10px;")
            elif color_mode == "off" and checkbox.isChecked():
                button.setStyleSheet("background-color: red; border-radius: 10px;")
            elif color_mode == "auto":
                if checkbox.isChecked():
                    button.setStyleSheet("background-color: green; border-radius: 10px;")
                else:
                    button.setStyleSheet("background-color: red; border-radius: 10px;")


    # === Turns voltage and current source labels red or green depending on what is activated  REVIEW THIS - MAY NEED TO BE DELETED - MAY BE REDUNDANT === #
    def update_label_colors_off(self):
        """Sets only the closest status buttons to red (OFF state)."""
        
        # === Define mapping of checkboxes to only the closest push buttons (first column) === #
        checkbox_button_mapping = {
            self.ui.cB_va: self.ui.pB_vaOff,
            self.ui.cB_vb: self.ui.pB_vbOff,
            self.ui.cB_vc: self.ui.pB_vcOff,
            self.ui.cB_ia: self.ui.pB_iaOff,
            self.ui.cB_ib: self.ui.pB_ibOff,
            self.ui.cB_ic: self.ui.pB_icOff,
        }

        # === Loop through each button and set it to red (OFF state) === #
        for button in checkbox_button_mapping.values():
            button.setStyleSheet("background-color: red; border-radius: 10px;")

    
    def send_custom_command(self):
        # === Check if the serial port is open, if not, attempt to open it === #
        if not self.serial_port.isOpen():
            try:
                # === Attempt to open the serial port (adjust the port name if necessary) === #
                self.serial_port.setPortName(self.arduino)  # === Adjust this to your actual port name === #
                if self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                    self.append_log("Serial port opened successfully.", "positive")
                else:
                    raise Exception("Failed to open the serial port.")
            except Exception as e:
                self.append_log(f"Error opening serial port: {str(e)}", "negative")
                QMessageBox.critical(self, "Connection Error", f"Error opening serial port: {str(e)}")
                return

        # === Send the custom SCPI command === #
        try:
                command = self.ui.TB_scpiCommands.toPlainText().strip()
                print(f"{command}\n".encode())
                self.serial_port.write(f"{command}\n".encode())  # === Send the command as bytes === #
                self.append_log(f"Sent command: '{command}' to serial port.", "positive")
                
                # === Set up a timer to wait for the response from the SCPI device === #
                QTimer.singleShot(1000, self.read_arduino_response)  # === Wait 1 second for response === #
            
        except Exception as e:
            self.append_log(f"Error sending command: {str(e)}", "negative")
            QMessageBox.critical(self, "Send Command Error", f"Error sending command: {str(e)}")
   
            
    def read_arduino_response(self):
        """Reads the response from the Arduino and appends it to the log."""
        if not self.serial_port.canReadLine():      
            self.append_log("No response from Arduino.", "negative")
        else:
            while self.serial_port.canReadLine():    # === sees if line is avliable to read === #
                response = self.serial_port.readLine().data().decode('utf-8').strip()  # === Read the line and decode it === #
                if response:
                    self.append_log(f"Arduino response: {response}", "positive")


    def read_response(self):
        """Reads and logs the response from Arduino."""
        if self.serial_port.bytesAvailable() > 0:
            try:
                response = self.serial_port.readAll().data().decode().strip()
                if self.check_com_errors(response):
                    self.append_log("Communication error in response", "negative")
                else:
                    self.append_log(f"Response received: {response}", "positive")
                    self.ui.TB_scpiCommands.setPlainText(response + "\n" + self.ui.TB_scpiCommands.toPlainText())
                
                # === Clear buffers === #
                self.serial_port.clear()
                # === self.serial_port.reset_input_buffer() === #
                # === self.serial_port.reset_output_buffer() === #
                
            except Exception as ex:
                self.append_log(f"Error reading response: {str(ex)}", "negative")


    def source_value_check(self):
        """Validate source voltage and current values."""
        try:
            # === Ensure all fields exist and are not empty === #
            required_fields = {
                "A Voltage": self.ui.TB_va,
                "B Voltage": self.ui.TB_vb,
                "C Voltage": self.ui.TB_vc,
                "A Voltage Phase": self.ui.TB_vaAngle,
                "B Voltage Phase": self.ui.TB_vbAngle,
                "C Voltage Phase": self.ui.TB_vcAngle,
                "A Current": self.ui.TB_ia,
                "B Current": self.ui.TB_ib,
                "C Current": self.ui.TB_ic,
                "A Current Phase": self.ui.TB_iaAngle,
                "B Current Phase": self.ui.TB_ibAngle,
                "C Current Phase": self.ui.TB_icAngle,
            }
            
            """
            for label, field in required_fields.items():
                value = field.toPlainText().strip()
                if value == "" or not MainWindow.is_numeric(value):
                    self.append_log(f"Missing field for {label}. Please enter a value.", "negative")
                    QMessageBox.critical(self, "Input Error", f"Missing field for {label}. Please enter a value.")
                    return False
            """

            # === Convert values to float and check ranges === #
            a_volt = float(self.ui.TB_va.toPlainText().strip())
            b_volt = float(self.ui.TB_vb.toPlainText().strip())
            c_volt = float(self.ui.TB_vc.toPlainText().strip())

            a_curr = float(self.ui.TB_ia.toPlainText().strip())
            b_curr = float(self.ui.TB_ib.toPlainText().strip())
            c_curr = float(self.ui.TB_ic.toPlainText().strip())

            # === Voltage and Current Range Validation === #
            if not (0 <= a_volt <= 600) or not (0 <= b_volt <= 600) or not (0 <= c_volt <= 600):
                self.append_log("Voltage values out of range.", "negative")
                QMessageBox.critical(self, "Voltage Range Error", "Voltage must be between 0 and 600 volts.")
                return False

          

            self.append_log("Source values validated successfully", "positive")
            return True

        except ValueError:
            self.append_log("Non-numeric value entered for voltage or current.", "negative")
            QMessageBox.critical(self, "Input Error", "Please enter valid numeric values for voltage and current.")
            return False

    def check_com_errors(self, response):
        """Checks the response for communication errors."""
        return "ERROR" in response or not response.strip()

    
    def append_log(self, message, log_type="neutral"):
        """Log messages to both console, UI, and file."""
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        plain_message = f"[{timestamp}] {log_type.upper()}: {message}"

    # Log to console
        print(plain_message)

    # Log to UI
        self.ui.TB_eventLog.append(plain_message)

    # Log to file
        try:
            if hasattr(self, "log_file_path") and self.log_file_path:
                with open(self.log_file_path, "a", encoding="utf-8") as f:
                    f.write(plain_message + "\n")
        except Exception as e:
            print(f"File logging failed: {e}")


    def back_to_meter_test(self):
        """Return to the main meter test screen."""
        self.close()

if __name__ == "__main__":
    from PyQt6.QtWidgets import QApplication
    import sys

    app = QApplication(sys.argv)
    main_window = MainWindow()
    main_window.show()
    sys.exit(app.exec())
