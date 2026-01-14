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
import subprocess
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
        self.temperature_rise_active = False

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
        #self.ui.pB_wifi.clicked.connect(self.run_wifi_checker)
        #self.ui.pb_stopTest.clicked.connect(lambda: self.abort_test(IsKeepingVoltage=True))

        # Channels dropdown
        self.channel_form = [str(i) for i in range(1, 17)]
        self.ui.dropDown_channel.addItems(self.channel_form)

        # Meter forms
        self.meter_forms = ["16S", "2S", "3S", "3SC", "4S", "5S", "6S", "7S", "8S", "9S", "10S", "11S", "12S",
                            "12Se", "13S", "14S", "15S", "1S", "17S", "24S", "25S", "26S", "29S", "35S", "36S", "39S", "45S", "46S", "56S", "66S", "76S"]
        self.ui.dropDown_meterForm.addItems(self.meter_forms)
        self.ui.dropDown_meterForm.currentIndexChanged.connect(self.autopopulate_kh)

        # LV/HV
        self.voltage_forms = ["LV", "HV"]
        self.ui.dropDown_lvHV.addItems(self.voltage_forms)

        # bef/aft
        self.phase_form = ["bef", "aft"]
        self.ui.dropDown_befAft.addItems(self.phase_form)

        # Test variable defaults
        self.ui.TB_Kh.setText("0.00001")
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

        # Close Temperature Rise application if active
        try:
            if getattr(self, "temperature_rise_active", False):
                self.close_temperature_rise_window()
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
        self.ui.pB_tempRise.clicked.connect(self.open_temperature_rise)

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
            if kh < 0.000001 or kh > 86.4:
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
        self.vA_pri = self.vA_sec / 2 * 0.989 # 0.9865 is the correction factor for the A phase voltage
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
            if abs(i) <= ZERO_EPS or i <= 100.0 + ZERO_EPS:
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

        # 0 < I ≤ 0.3 A considered 'low'
        Ia = float(self.Ia_sec or 0.0)
        Ib = float(self.Ib_sec or 0.0)
        Ic = float(self.Ic_sec or 0.0)

        isIa_low = 0 < Ia <= 0.3
        isIb_low = 0 < Ib <= 0.3
        isIc_low = 0 < Ic <= 0.3

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
                    lambda p=port, k=key: (self.append_log(f"{k} → CURR 2.0", "neutral"),
                                           p.write(b"CURR 2.0\n")))
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
                    lambda p=port, k=key: (self.append_log(f"{k} → VOLT 150", "neutral"),
                                           p.write(b"VOLT 150\n")))
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
                QTimer.singleShot(300, lambda: self._continue_ct_routing() if not self.test_aborted else None)
            except Exception as e:
                self.append_log(f"CT Ranging I2C Error: {e}", "negative")
                QMessageBox.critical(self, "CT Ranging Error", f"CT ranging failed: {e}")

        # Wait 300ms before CT setup to ensure sources are fully powered down
        QTimer.singleShot(300, continue_ct_setup)

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
        if max_current <= 100.0:
            if not (gpio6_state == 1 and gpio5_state == 0):
                def step1():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFA)
                    QTimer.singleShot(500, step2)
                def step2():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(7000, step3)
                def step3():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xF7)
                    QTimer.singleShot(500, step4)
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
                    QTimer.singleShot(500, step2)
                def step2():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(7000, step3)
                def step3():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xF6)
                    QTimer.singleShot(500, step4)
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
                    QTimer.singleShot(500, step2)
                def step2():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFF)
                    QTimer.singleShot(7000, step3)
                def step3():
                    self._ct_routing_bus.write_byte(ROUTING_ADDR, 0xFB)
                    QTimer.singleShot(500, step4)
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
        #self._sync_clamps_to_currents(reason="after source send")

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

                QTimer.singleShot(300, after_initial_metrics)
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

    def _sync_clamps_to_currents(self, *, reason: str = "", on_done=None):
        """
        Staggered CLAM routing based on Ia/Ib/Ic (sec).
        NEW: calls on_done() after the last CLAM write should be sent.
        """
        from PyQt6.QtCore import QTimer

        def f(x):
            try:
                return float(x)
            except Exception:
                return 0.0

        Ia = f(getattr(self, "Ia_sec", 0.0))
        Ib = f(getattr(self, "Ib_sec", 0.0))
        Ic = f(getattr(self, "Ic_sec", 0.0))

        ZERO_EPS = 1e-6
        LOW_MAX = 100.0
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

        # Ensure Arduino COM is open
        if not self._ensure_arduino_port_open():
            return

        # Small gap before CLAM commands
        selection_gap_ms = 200

        # Staggered CLAM writes (after selection gap)
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

        # NEW: completion callback after last CLAM write
        if callable(on_done):
            done_ms = selection_gap_ms + (len(plan) - 1) * base_gap_ms + 75
            QTimer.singleShot(done_ms, on_done)

    def send_source_commands(self, *, after=None, extra_settle_ms=0, hard_reset_currents=False):
        """
        MainWindow-safe source activation with a REAL completion callback.

        Guarantees:
          - Schedules all SCPI writes in a staged sequence
          - Calls `after()` ONLY after the last scheduled write + settle time
          - By default DOES NOT hard-power-down currents first (hard_reset_currents=False)

        Params:
          after: callback fired after arming is truly finished
          extra_settle_ms: additional settle time after last write before calling after()
          hard_reset_currents: if True, forces Ia/Ib/Ic OFF first (you said you generally do NOT want this)
        """
        from PyQt6.QtCore import QTimer

        if self.test_aborted:
            self.append_log("Source activation skipped due to test abort.", "neutral")
            return

        # token to cancel previous in-flight sequences
        self._src_send_token = getattr(self, "_src_send_token", 0) + 1
        token = self._src_send_token

        def still_valid():
            return (not self.test_aborted) and getattr(self, "_src_send_token", None) == token

        def port_for(key):
            p = self.serial_connections.get(key)
            if p and p.isOpen():
                return p
            self.append_log(f"{key} not connected or port not open.", "negative")
            return None

        def write_scpi(key, cmd, log_level="neutral"):
            if not still_valid():
                return
            p = port_for(key)
            if not p:
                return
            try:
                p.write((cmd.strip() + "\n").encode())
                try:
                    p.flush()
                except Exception:
                    pass
                self.append_log(f"{key} → {cmd.strip()}", log_level)
            except Exception as e:
                self.append_log(f"{key} write error: {e}", "negative")

        # ---------------- helpers used by your existing logic ----------------
        def _cell_text(row, col):
            try:
                it = self.ui.table_script.item(row, col)
                return it.text().strip() if (it and it.text()) else ""
            except Exception:
                return ""

        def _cell_float(row, col, default=None):
            try:
                s = _cell_text(row, col)
                return float(s) if s != "" else default
            except Exception:
                return default

        def _seed_once(attr, fn_default):
            if getattr(self, attr, None) is None:
                try:
                    setattr(self, attr, float(fn_default()))
                except Exception:
                    setattr(self, attr, float(fn_default()))

        def _enforce_current_outp(cmd: str, on: bool) -> str:
            desired = "1" if on else "0"
            if not cmd:
                return f"OUTP {desired};" if on else "OUTP 0;CURR 0;"
            import re
            if re.search(r"\bOUTP\s+[01]\b", cmd, flags=re.IGNORECASE):
                cmd = re.sub(r"\bOUTP\s+[01]\b", f"OUTP {desired}", cmd, flags=re.IGNORECASE)
            else:
                cmd = f"OUTP {desired};" + cmd
            if not on:
                if "CURR" in cmd.upper():
                    cmd = re.sub(r"\bCURR\s+[-+]?\d*\.?\d+\b", "CURR 0", cmd, flags=re.IGNORECASE)
                elif "VOLT" not in cmd.upper():
                    cmd += "CURR 0;"
            return cmd

        # ---------------- seed PHAS only if missing ----------------
        vB_target = _cell_float(self.current_step, 7, 120.0)   # pab
        vC_target = _cell_float(self.current_step, 8, 240.0)   # pac
        vA_target = 0.0

        _seed_once("pab", lambda: vB_target if vB_target is not None else 120.0)
        _seed_once("pac", lambda: vC_target if vC_target is not None else 240.0)
        _seed_once("pha", lambda: vA_target)
        _seed_once("phb", lambda: vB_target if vB_target is not None else 120.0)
        _seed_once("phc", lambda: vC_target if vC_target is not None else 240.0)

        # ---------------- build command strings ----------------
        cmds = {}

        Va_volt = 0 if getattr(self, "vA_pri", 0) == 0 else self.vA_pri
        Vb_volt = 0 if getattr(self, "vB_pri", 0) == 0 else self.vB_pri
        Vc_volt = 0 if getattr(self, "vC_pri", 0) == 0 else self.vC_pri

        cmds["Va"] = f"OUTP 1;VOLT {Va_volt};PHAS 0;"
        cmds["Vb"] = f"OUTP 1;VOLT {Vb_volt};PHAS {self._norm_angle(getattr(self, 'pab', 120.0))};"
        cmds["Vc"] = f"OUTP 1;VOLT {Vc_volt};PHAS {self._norm_angle(getattr(self, 'pac', 240.0))};"

        iA_on = float(getattr(self, "Ia_sec", 0.0) or 0.0) > 0.0
        iB_on = float(getattr(self, "Ib_sec", 0.0) or 0.0) > 0.0
        iC_on = float(getattr(self, "Ic_sec", 0.0) or 0.0) > 0.0

        # NOTE: keep your low-current VOLT trick exactly as you had it
        if iA_on:
            if 0.0 < self.Ia_sec <= 0.3:
                cmds["Ia"] = f"OUTP 1;VOLT {round(self.Ia_sec * 1000)};PHAS {self._norm_angle(getattr(self, 'pha', vA_target))};"
            else:
                self.Ia_pri = self.scale_current(self.Ia_sec)
                cmds["Ia"] = f"OUTP 1;CURR {self.Ia_pri};PHAS {self._norm_angle(getattr(self, 'pha', vA_target))};"
        else:
            cmds["Ia"] = "OUTP 0;CURR 0;"

        if iB_on:
            if 0.0 < self.Ib_sec <= 0.3:
                cmds["Ib"] = f"OUTP 1;VOLT {round(self.Ib_sec * 1000)};PHAS {self._norm_angle(getattr(self, 'phb', vB_target if vB_target is not None else 120.0))};"
            else:
                self.Ib_pri = self.scale_current(self.Ib_sec)
                cmds["Ib"] = f"OUTP 1;CURR {self.Ib_pri};PHAS {self._norm_angle(getattr(self, 'phb', vB_target if vB_target is not None else 120.0))};"
        else:
            cmds["Ib"] = "OUTP 0;CURR 0;"

        if iC_on:
            if 0.0 < self.Ic_sec <= 0.3:
                cmds["Ic"] = f"OUTP 1;VOLT {round(self.Ic_sec * 1000)};PHAS {self._norm_angle(getattr(self, 'phc', vC_target if vC_target is not None else 240.0))};"
            else:
                self.Ic_pri = self.scale_current(self.Ic_sec)
                cmds["Ic"] = f"OUTP 1;CURR {self.Ic_pri};PHAS {self._norm_angle(getattr(self, 'phc', vC_target if vC_target is not None else 240.0))};"
        else:
            cmds["Ic"] = "OUTP 0;CURR 0;"

        cmds["Ia"] = _enforce_current_outp(cmds.get("Ia", ""), iA_on)
        cmds["Ib"] = _enforce_current_outp(cmds.get("Ib", ""), iB_on)
        cmds["Ic"] = _enforce_current_outp(cmds.get("Ic", ""), iC_on)

        # --------- timing plan ----------
        base = 0
        step = 150

        total_ms = 0

        # Optional: hard-force currents off first (you generally do NOT want this)
        if hard_reset_currents:
            for k in ("Ia", "Ib", "Ic"):
                write_scpi(k, "OUTP 0;CURR 0;PHAS 0;", "neutral")
            self.append_log("Waiting 300 ms for current sources to power down...", "neutral")
            base += 300
            total_ms = base

        # voltages first
        for i, k in enumerate(("Va", "Vb", "Vc")):
            t = base + i * step
            QTimer.singleShot(t, lambda kk=k: write_scpi(kk, cmds[kk], "positive"))
            total_ms = max(total_ms, t)

        # currents next
        base2 = base + 3 * step + 150
        for i, k in enumerate(("Ia", "Ib", "Ic")):
            t = base2 + i * step
            QTimer.singleShot(t, lambda kk=k: write_scpi(kk, cmds[kk], "positive"))
            total_ms = max(total_ms, t)

        # "double tap" OUTP 1 for ON currents (keeps them from being flaky)
        base3 = base2 + 3 * step + 200

        def pulse_enable(key):
            if not still_valid():
                return
            should_on = {"Ia": iA_on, "Ib": iB_on, "Ic": iC_on}.get(key, False)
            if should_on:
                write_scpi(key, "OUTP 1;", "neutral")

        for i, k in enumerate(("Ia", "Ib", "Ic")):
            t = base3 + i * step
            QTimer.singleShot(t, lambda kk=k: pulse_enable(kk))
            total_ms = max(total_ms, t)

        # Extra settle if C is ON (your original idea), plus caller settle
        c_extra = 1200 if iC_on else 0
        done_ms = int(total_ms + c_extra + int(extra_settle_ms))

        if callable(after):
            QTimer.singleShot(done_ms, lambda: (still_valid() and after()))

    def source_correction(self):
        """
        Closed-loop trim with correct IMET angle semantics.

        FIXED ORDER (guaranteed):
          1) sync clamps
          2) arm sources
          3) WAIT until arming is done + settle
          4) start IMET reads / correction
        """
        # ---- CONFIG ----
        V_ABS_MAX = getattr(self, "V_ABS_MAX", 300.0)
        I_ABS_MAX = getattr(self, "I_ABS_MAX", 600.0)
        V_MIN_CMD = 0.0
        I_MIN_CMD = 0.0

        KV = 0.5
        KI = 0.5
        V_MAX_STEP_ABS = 10.0
        I_MAX_STEP_ABS = 40.0
        V_MAX_STEP_PCT = 0.20
        I_MAX_STEP_PCT = 0.70

        V_TOL = 0.01
        V_TOL_MIN = 0.05
        I_TOL = 0.01
        I_TOL_MIN = 0.01

        PHASE_TOL_DEG = 0.2
        VOLT_ANG_ATTEMPTS = 20
        I_ANG_ATTEMPTS = 20
        VOLT_SETTLE_MS = 800
        CURR_SETTLE_MS = 1200

        # IMPORTANT: this is the settle AFTER sources are armed before first IMET parse
        STARTUP_ARM_SETTLE_MS = getattr(self, "SRC_ARM_SETTLE_MS", 2000)

        if getattr(self, "_skip_full_setup", False):
            self.append_log("Skip flag set → bypassing source correction.", "neutral")
            self.check_pause_time()
            return
        if self.test_aborted:
            self.append_log("Source correction skipped due to test abort.", "neutral")
            return

        import re, math
        from PyQt6.QtCore import QTimer
        from PyQt6.QtSerialPort import QSerialPort  # needed for OpenModeFlag below

        # ---------- helpers ----------
        def _wrap_pm180(x):
            return (x + 180.0) % 360.0 - 180.0

        def _wrap_err_abs(measured_abs, target_abs):
            m = self._norm_angle(measured_abs)
            t = self._norm_angle(target_abs)
            return _wrap_pm180(t - m)

        def _clamp(x, lo, hi):
            return max(lo, min(hi, x))

        def _slew_limit(old, target, max_abs, max_pct):
            step = target - old
            cap = float("inf")
            if max_abs is not None:
                cap = min(cap, abs(max_abs))
            if max_pct is not None:
                cap = min(cap, abs(old) * max_pct)
            if cap != float("inf"):
                step = _clamp(step, -cap, cap)
            return old + step

        def _safe_mag(target, lo, hi):
            if not isinstance(target, (int, float)) or math.isnan(target) or math.isinf(target):
                return lo
            return _clamp(target, lo, hi)

        def _cell_text(row, col):
            try:
                it = self.ui.table_script.item(row, col)
                return it.text().strip() if (it and it.text()) else ""
            except Exception:
                return ""

        def _cell_float(row, col, default=None):
            s = _cell_text(row, col)
            if s == "":
                return default
            try:
                return float(s)
            except Exception:
                return default

        def _kick_imet_read(imet_num, on_ready):
            if self.test_aborted:
                return
            try:
                if not self.serial_port.isOpen():
                    self.serial_port.setPortName(self.arduino)
                    if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                        self.append_log("ERROR: Could not open Arduino port for RX command", "negative")
                        on_ready(None)
                        return
                cmd = f"scpi,RX,'READ:IMET{imet_num}?',"
                self.serial_port.write(cmd.encode())
            except Exception as e:
                self.append_log(f"Failed to send RX command: {e}", "negative")
                on_ready(None)
                return
            QTimer.singleShot(1200, on_ready)

        def _read_imet_parsed(_imet_num):
            try:
                response = ""
                while self.serial_port.canReadLine():
                    line = self.serial_port.readLine().data().decode("utf-8", errors="replace").strip()
                    response += line
                m = re.search(r"(VUNDer|CUNDer|OK|CRTRansient|COVer)[,~]\(([^)]+)\)", response)
                if not m or len(response) < 30:
                    return None
                vals = re.split(r"[~,]", m.group(2))
                if len(vals) < 5:
                    return None
                p = float(vals[0]); v = float(vals[1]); i = float(vals[2])
                v_abs = float(vals[3]); i_rel = float(vals[4])
                i_rel = _wrap_pm180(i_rel)
                return p, v, i, v_abs, i_rel
            except Exception:
                return None

        def _v_target_abs(phase_label):
            if phase_label == "A":
                return 0.0
            elif phase_label == "B":
                val = _cell_float(self.current_step, 7, 120.0)
                return self._norm_angle(0.0 if val is None else val)
            else:
                val = _cell_float(self.current_step, 8, 240.0)
                return self._norm_angle(0.0 if val is None else val)

        def _i_target_abs(phase_label, v_target_abs):
            col = 12 if phase_label == "A" else (13 if phase_label == "B" else 14)
            s = _cell_text(self.current_step, col)
            if s == "":
                return v_target_abs
            try:
                return self._norm_angle(float(s))
            except Exception:
                return v_target_abs

        def _expected_v_i(phase_label, voltage_col, idx):
            v_txt = _cell_text(self.current_step, voltage_col)
            if v_txt == "":
                v_fallback = getattr(self, f"v{phase_label}_sec", 0.0)
                try:
                    exp_v = float(v_fallback if v_fallback is not None else 0.0)
                except Exception:
                    exp_v = 0.0
            else:
                try:
                    exp_v = float(v_txt)
                except Exception:
                    exp_v = float(getattr(self, f"v{phase_label}_sec", 0.0) or 0.0)

            i_txt = _cell_text(self.current_step, 9 + idx)
            if i_txt == "":
                i_fallback = getattr(self, f"I{phase_label.lower()}_sec", 0.0)
                try:
                    exp_i = float(i_fallback if i_fallback is not None else 0.0)
                except Exception:
                    exp_i = 0.0
            else:
                try:
                    exp_i = float(i_txt)
                except Exception:
                    exp_i = float(getattr(self, f"I{phase_label.lower()}_sec", 0.0) or 0.0)

            return exp_v, exp_i

        phases = [
            ("A", 1, "Ia", "Va", "pha", 4, 12),
            ("B", 2, "Ib", "Vb", "phb", 5, 13),
            ("C", 3, "Ic", "Vc", "phc", 6, 14),
        ]

        # Small wrapper: arm sources, then ONLY after settle run cb
        def _arm_then(cb, settle_ms):
            if self.test_aborted:
                return
            self.send_source_commands(
                after=cb,
                extra_settle_ms=int(settle_ms),
                hard_reset_currents=False  # <-- IMPORTANT: do not power-down currents during correction
            )

        # ===== PASS 1: Magnitudes =====
        self.append_log("Pass 1: correcting magnitudes (skipping scripted zeros) A→B→C…", "neutral")

        def pass1_correct_phase(idx):
            if self.test_aborted:
                return
            if idx >= len(phases):
                self.append_log("Pass 1 complete. Starting Pass 2 (angles).", "neutral")
                QTimer.singleShot(300, lambda: pass2_correct_phase(0))
                return

            phase_label, imet_num, *_rest, voltage_col, _ = phases[idx]
            attempts = {"v": 0, "i": 0}
            stage = {"s": "V"}

            def tick():
                _kick_imet_read(imet_num, on_ready)

            def on_ready(_=None):
                if self.test_aborted:
                    return

                parsed = _read_imet_parsed(imet_num)
                if parsed is None:
                    self.append_log(f"IMET parse failed (Phase {phase_label}) — skipping to next.", "negative")
                    QTimer.singleShot(200, lambda: pass1_correct_phase(idx + 1))
                    return

                _, v_meas_mag, i_meas_mag, _, _ = parsed
                exp_v, exp_i = _expected_v_i(phase_label, voltage_col, idx)

                v_set = getattr(self, f"v{phase_label}_sec", exp_v)
                i_set = getattr(self, f"I{phase_label.lower()}_sec", exp_i)

                if exp_v <= 0.0:
                    stage["s"] = "I"

                if stage["s"] == "V":
                    v_err = abs(exp_v - v_meas_mag)
                    v_tol = max(V_TOL * max(exp_v, 1e-6), V_TOL_MIN)
                    if v_err <= v_tol or attempts["v"] >= 2:
                        self.append_log(f"{phase_label} V OK/skip (err={v_err:.3f} ≤ {v_tol:.3f}).", "neutral")
                        stage["s"] = "I"
                        QTimer.singleShot(250, tick)
                        return

                    attempts["v"] += 1
                    v_target = _safe_mag(v_set + KV * (exp_v - v_meas_mag), V_MIN_CMD, V_ABS_MAX)
                    v_new = _safe_mag(_slew_limit(v_set, v_target, V_MAX_STEP_ABS, V_MAX_STEP_PCT), V_MIN_CMD, V_ABS_MAX)
                    if v_new > V_ABS_MAX:
                        self.append_log(f"Safety abort: V set {v_new:.2f} > {V_ABS_MAX}", "negative")
                        self.abort_test()
                        return

                    setattr(self, f"v{phase_label}_sec", v_new)

                    self.append_log(f"{phase_label} V step → {v_new:.3f} V (try {attempts['v']}/2)", "neutral")
                    _arm_then(tick, VOLT_SETTLE_MS)
                    return

                if exp_i <= 0.0:
                    self.append_log(f"{phase_label} I skipped (scripted 0).", "neutral")
                    QTimer.singleShot(250, lambda: pass1_correct_phase(idx + 1))
                    return

                i_err = abs(exp_i - i_meas_mag)
                i_tol = max(I_TOL * max(exp_i, 1e-6), I_TOL_MIN)
                if i_err <= i_tol or attempts["i"] >= 2:
                    self.append_log(f"{phase_label} I OK/skip (err={i_err:.3f} ≤ {i_tol:.3f}).", "neutral")
                    QTimer.singleShot(250, lambda: pass1_correct_phase(idx + 1))
                    return

                attempts["i"] += 1
                if exp_i < 0.5:
                    i_target = max(0.01, 0.9 * exp_i + 0.1 * i_meas_mag)
                else:
                    i_target = i_set + KI * (exp_i - i_meas_mag)

                i_target = _safe_mag(i_target, I_MIN_CMD, I_ABS_MAX)
                i_new = _safe_mag(_slew_limit(i_set, i_target, I_MAX_STEP_ABS, I_MAX_STEP_PCT), I_MIN_CMD, I_ABS_MAX)
                if i_new > I_ABS_MAX:
                    self.append_log(f"Safety abort: I set {i_new:.2f} > {I_ABS_MAX}", "negative")
                    self.abort_test()
                    return

                setattr(self, f"I{phase_label.lower()}_sec", i_new)

                self.append_log(f"{phase_label} I step → {i_new:.3f} A (try {attempts['i']}/2)", "neutral")
                _arm_then(tick, CURR_SETTLE_MS)
                return

            tick()

        # ===== PASS 2: Angles =====
        def pass2_correct_phase(idx):
            if self.test_aborted:
                return
            if idx >= len(phases):
                self.source_corrected_values = True
                self.append_log("Source correction complete (magnitudes + angles).", "positive")
                # Give a little settle before the final snapshot
                QTimer.singleShot(800, lambda: (not self.test_aborted) and self.read_final_imet_snapshot())
                return

            phase_label, imet_num, *_rest, voltage_col, _ = phases[idx]
            attempts = {"vang": 0, "iang": 0}

            V_target_abs = self._norm_angle(_v_target_abs(phase_label))
            exp_v, exp_i = _expected_v_i(phase_label, voltage_col, idx)
            I_target_abs = self._norm_angle(_i_target_abs(phase_label, V_target_abs))
            delta_target = _wrap_pm180(I_target_abs - V_target_abs)

            def trim_voltage():
                if self.test_aborted:
                    return

                def _after_read(_=None):
                    parsed = _read_imet_parsed(imet_num)
                    if parsed is None:
                        if attempts["vang"] < VOLT_ANG_ATTEMPTS:
                            attempts["vang"] += 1
                            self.append_log(f"{phase_label} V-angle read failed — retry {attempts['vang']}/{VOLT_ANG_ATTEMPTS}.", "negative")
                            QTimer.singleShot(VOLT_SETTLE_MS, trim_voltage)
                            return
                        self.append_log(f"{phase_label} V-angle read failed after retries; continuing.", "negative")
                        proceed_current()
                        return

                    _, _, _, v_meas_abs, _ = parsed

                    if exp_v <= 0.0 or phase_label == "A":
                        proceed_current()
                        return

                    err = _wrap_err_abs(v_meas_abs, V_target_abs)
                    if abs(err) <= PHASE_TOL_DEG:
                        self.append_log(f"{phase_label} V angle OK (target {V_target_abs:.2f}°, meas {v_meas_abs:.2f}°, err {err:+.2f}°).", "positive")
                        proceed_current()
                        return

                    if phase_label == "B":
                        cur_cmd = self._norm_angle(getattr(self, "pab", V_target_abs))
                        self.pab = self._norm_angle(cur_cmd + err)
                        new_cmd = self.pab
                    else:
                        cur_cmd = self._norm_angle(getattr(self, "pac", V_target_abs))
                        self.pac = self._norm_angle(cur_cmd + err)
                        new_cmd = self.pac

                    self.append_log(f"{phase_label} V-angle adjust → PHAS {new_cmd:.2f}° (err {err:+.2f}°).", "neutral")

                    attempts["vang"] += 1
                    if attempts["vang"] > VOLT_ANG_ATTEMPTS:
                        self.append_log(f"{phase_label} V angle not within ±{PHASE_TOL_DEG}° after retries; continuing.", "negative")
                        proceed_current()
                        return

                    _arm_then(trim_voltage, VOLT_SETTLE_MS)
                    return

                _kick_imet_read(imet_num, _after_read)

            def trim_current():
                if self.test_aborted:
                    return

                def _after_read(_=None):
                    parsed = _read_imet_parsed(imet_num)
                    if parsed is None:
                        if attempts["iang"] < I_ANG_ATTEMPTS:
                            attempts["iang"] += 1
                            self.append_log(f"{phase_label} I-angle read failed — retry {attempts['iang']}/{I_ANG_ATTEMPTS}.", "negative")
                            QTimer.singleShot(CURR_SETTLE_MS, trim_current)
                            return
                        self.append_log(f"{phase_label} I-angle read failed after retries; moving on.", "negative")
                        QTimer.singleShot(400, lambda: pass2_correct_phase(idx + 1))
                        return

                    _, _, _, _v_meas_abs, i_meas_rel = parsed

                    if exp_i <= 0.0:
                        self.append_log(f"{phase_label} I-angle skipped (scripted I=0).", "neutral")
                        QTimer.singleShot(400, lambda: pass2_correct_phase(idx + 1))
                        return

                    err_rel = _wrap_pm180(delta_target - i_meas_rel)
                    if abs(err_rel) <= PHASE_TOL_DEG:
                        self.append_log(
                            f"{phase_label} I−V angle OK (target {delta_target:+.2f}°, meas {i_meas_rel:+.2f}°, err {err_rel:+.2f}°).",
                            "positive"
                        )
                        QTimer.singleShot(400, lambda: pass2_correct_phase(idx + 1))
                        return

                    if phase_label == "A":
                        cur_cmd = self._norm_angle(getattr(self, "pha", I_target_abs))
                        self.pha = self._norm_angle(cur_cmd + err_rel)
                        new_cmd = self.pha
                    elif phase_label == "B":
                        cur_cmd = self._norm_angle(getattr(self, "phb", I_target_abs))
                        self.phb = self._norm_angle(cur_cmd + err_rel)
                        new_cmd = self.phb
                    else:
                        cur_cmd = self._norm_angle(getattr(self, "phc", I_target_abs))
                        self.phc = self._norm_angle(cur_cmd + err_rel)
                        new_cmd = self.phc

                    self.append_log(
                        f"{phase_label} I-angle adjust (relative) → PHAS {new_cmd:.2f}° "
                        f"[Δtarget {delta_target:+.2f}°, Δmeas {i_meas_rel:+.2f}°, err {err_rel:+.2f}°].",
                        "neutral"
                    )

                    attempts["iang"] += 1
                    if attempts["iang"] > I_ANG_ATTEMPTS:
                        self.append_log(f"{phase_label} I−V angle not within ±{PHASE_TOL_DEG}° after retries.", "negative")
                        QTimer.singleShot(400, lambda: pass2_correct_phase(idx + 1))
                        return

                    _arm_then(trim_current, CURR_SETTLE_MS)
                    return

                _kick_imet_read(imet_num, _after_read)

            def proceed_current():
                trim_current()

            trim_voltage()

        # ✅ FIXED KICKOFF:
        # clamps → arm sources → WAIT STARTUP_ARM_SETTLE_MS → start IMET correction
        def _kickoff_pass1():
            if self.test_aborted:
                return
            pass1_correct_phase(0)

        self._sync_clamps_to_currents(
            reason="before source_correction kickoff",
            on_done=lambda: self.send_source_commands(
                after=_kickoff_pass1,
                extra_settle_ms=STARTUP_ARM_SETTLE_MS,
                hard_reset_currents=False
            )
        )
        return

    def read_final_imet_snapshot(self):
        if getattr(self, "_skip_full_setup", False):
            self.append_log("Skip flag set → bypassing final IMET snapshot.", "neutral")
            self.check_pause_time()
            return

        from PyQt6.QtCore import QTimer

        # NEW: wait a bit before first IMET read so the standard is stable
        initial_settle_ms = getattr(self, "FINAL_IMET_SETTLE_MS", 2000)

        phases = [
            ("A", 1, 0),
            ("B", 2, 1),
            ("C", 3, 2),
        ]

        def read_imet_sequential(phase_index=0):
            if self.test_aborted:
                return

            if phase_index >= len(phases):
                self.append_log("Final IMET snapshot complete for all phases.", "positive")
                QTimer.singleShot(300, lambda: not self.test_aborted and self.check_pause_time())
                return

            label, imet_num, row_index = phases[phase_index]
            cmd = f"scpi,RX,'READ:IMET{imet_num}?',"
            try:
                self.serial_port.write(cmd.encode())
                self.append_log(f"Sent final RX command for IMET{imet_num} (Phase {label})", "neutral")
            except Exception as e:
                self.append_log(f"Final IMET{imet_num} read failed: {e}", "negative")
                QTimer.singleShot(500, lambda: read_imet_sequential(phase_index + 1))
                return

            def process_final():
                if self.test_aborted:
                    return

                response = ""
                while self.serial_port.canReadLine():
                    line = self.serial_port.readLine().data().decode("utf-8", errors="replace").strip()
                    response += line

                self.append_log(f"Final IMET{imet_num} response: {response}", "neutral")
                import re, math
                match = re.search(r"(VUNDer|CUNDer|OK|CRTRansient|COVer)[,~]\(([^)]+)\)", response)
                if not match or len(response) < 30:
                    self.append_log(f"Final IMET{imet_num} response invalid.", "negative")
                    QTimer.singleShot(500, lambda: read_imet_sequential(phase_index + 1))
                    return

                values = re.split(r"[~,]", match.group(2))
                if len(values) < 5:
                    self.append_log(f"Incomplete final IMET{imet_num} data: {values}", "negative")
                    QTimer.singleShot(500, lambda: read_imet_sequential(phase_index + 1))
                    return

                try:
                    power   = float(values[0])
                    voltage = float(values[1])
                    current = float(values[2])
                    v_angle = float(values[3])   # absolute V (deg)
                    i_angle = float(values[4])   # RELATIVE (I − V) in deg

                    metrics = [power, voltage, current, v_angle, i_angle]
                    from PyQt6.QtWidgets import QTableWidgetItem
                    from PyQt6.QtCore import Qt

                    for col, val in enumerate(metrics):
                        item = QTableWidgetItem(f"{val:.4f}")
                        item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                        self.ui.table_metrics.setItem(row_index, col, item)

                    def _wrap_pm180_local(x):
                        return (x + 180.0) % 360.0 - 180.0

                    phi = _wrap_pm180_local(-i_angle)
                    pf_mag = math.cos(math.radians(abs(phi)))

                    UNITY_DEG = 0.5
                    if abs(phi) < UNITY_DEG:
                        pf_text = "PF: Unity"
                    elif phi > 0:
                        pf_text = f"lag: {pf_mag:.2f}"
                    else:
                        pf_text = f"lead: {pf_mag:.2f}"

                    self.ui.table_metrics.setItem(row_index, 5, QTableWidgetItem(pf_text))

                    self.append_log(
                        f"Final Phase {label} IMET values: P={power:.4f}, V={voltage:.4f}, I={current:.4f}, "
                        f"Vabs={v_angle:.4f}°, Δ(I−V)={i_angle:+.4f}°, φ(V−I)={phi:+.4f}°, PF={pf_text}",
                        "positive"
                    )
                except Exception as e:
                    self.append_log(f"Failed parsing final IMET{imet_num} data: {e}", "negative")

                QTimer.singleShot(500, lambda: read_imet_sequential(phase_index + 1))

            QTimer.singleShot(1200, process_final)

        # NEW: delay the START of reading
        self.append_log(f"Waiting {initial_settle_ms} ms before final IMET snapshot...", "neutral")
        QTimer.singleShot(initial_settle_ms, lambda: read_imet_sequential(0))
           
    def pause_test(self, on_complete=None):
        """
        Pauses for self.pause_time seconds, updating the est time remaining LCD.
        When finished, returns control to the caller or calls on_complete if provided.
        """
        if self.test_aborted:
            self.append_log("Pause aborted due to test abort.", "neutral")
            return

        pause_seconds = int(self.pause_time) if self.pause_time > 0 else 0
        if pause_seconds <= 0:
            self.append_log("Pause time is 0. No countdown.", "neutral")
            if on_complete:
                on_complete()
            return

        self.est_time_remaining_value = pause_seconds
        self.ui.lcd_timeRemaining.display(self.est_time_remaining_value)

        def countdown():
            if self.test_aborted:
                self.est_time_remaining_timer.stop()
                return
            if self.est_time_remaining_value > 0:
                self.est_time_remaining_value -= 1
                self.ui.lcd_timeRemaining.display(self.est_time_remaining_value)
                self.append_log(f"Pause time remaining {self.est_time_remaining_value} seconds.", "neutral") 
            if self.est_time_remaining_value <= 0:
                self.est_time_remaining_timer.stop()
                if on_complete:
                    self.initial_step = False
                    on_complete()  # Call advance_step or any other function

        self.est_time_remaining_timer.stop()
        self.est_time_remaining_timer.timeout.disconnect()
        self.est_time_remaining_timer.timeout.connect(countdown)
        self.est_time_remaining_timer.start(1000)

    def check_pause_time(self):
        if self.test_aborted:
            self.append_log("Pause check aborted due to test abort.", "neutral")
            return

        if self.pause_time > 0:
            self.append_log(f"Pause time set to {self.pause_time} seconds. Pausing test.", "neutral")
            if self.check_no_load_pause():
                # Show countdown, then advance step after pause
                self.pause_test(on_complete=lambda: not self.test_aborted and self.advance_step())
            else:
                # Normal pause, show countdown, then continue with tests
                self.pause_test(on_complete=lambda: (
                    self.append_log("Pause completed. Continuing with tests.", "neutral"),
                    self.run_time_based_and_start_tests()
                ))
        elif self.Ia_sec == 0 and self.Ib_sec == 0 and self.Ic_sec == 0 and self.pause_time <= 0:
            self.append_log(f"No test current set. Skipping test point", "neutral")
            self.initial_step = False
            self.advance_step()
        else:
            self.append_log("Pause time is 0. Continuing with tests.", "neutral")
            self.run_time_based_and_start_tests()

    def check_no_load_pause(self):
        """Check for voltage powered sources with no load, then checks for pause time. If no load and pause time is set, it will pause the test and advance step after the pause."""
        if self.test_aborted:
            self.append_log("No load pause check aborted due to test abort.", "neutral")
            return

        if self.Ia_sec == 0 and self.Ib_sec == 0 and self.Ic_sec == 0 and (self.vA_sec > 0 or self.vB_sec > 0 or self.vC_sec > 0) and self.pause_time > 0:
            self.append_log(f"No load pause time set to {self.pause_time} seconds. Pausing test.", "neutral")
            return True
        else:
            self.append_log("There is no 'No load pause'. Continuing to pause normally, before testing.", "neutral")
            return False
    
    def run_time_based_and_start_tests(self):
        if self.test_aborted:
            self.append_log("Aborted before pulse update and test start.", "neutral")
            return

        self.append_log("Called run_time_based_and_start_tests()", "neutral")
        watts = abs(self.watts_from_metrics())
        self.append_log(f"RX Watts for current step: {watts:.4f}", "neutral")

        try:
            test_time = int(self.ui.TB_stepTime.text())
            if test_time < 0.1:
                raise ValueError
        except ValueError:
            self.append_log("Invalid test time. Must be an integer ≥ 0.1.", "stupity")
            QMessageBox.warning(self, "Invalid Input", "Enter a valid test time (≥ 0.1 seconds).")
            return

        kh_array = [channel["kh"] for channel in self.test_channels.values() if channel.get("kh") is not None]
        smallestKh = self.smallest_kh_from_metrics(kh_array)

        displayed_pulses = 0
        for chan in self.test_channel_keys:
            kh = self.test_channels[chan]["kh"]
            pulses = int((watts * test_time) / (3600 * kh)) if kh != 0 else 1
            pulses = max(pulses, 1)
            self.test_channels[chan]["step_time"] = pulses
            self.append_log(f"Step {self.current_step + 1} | Channel {chan} => pulses={pulses} | kh={kh}", "neutral")

            if displayed_pulses == 0:
                displayed_pulses = pulses

        self.append_log(f"Calculated Time to Pulses (using smallest Kh={smallestKh}): {displayed_pulses}", "positive")

        QTimer.singleShot(500, lambda: not self.test_aborted and self.send_next_test_command())

    def send_next_test_command(self):
        if self.test_aborted:
            self.append_log("Test aborted before sending test commands.", "neutral")
            return

        if not hasattr(self, 'test_channel_keys') or not self.test_channel_keys:
            self.append_log("No test channels found. Aborting.", "negative")
            return

        self.append_log(f"Starting test for channels: {self.test_channel_keys}", "neutral")

        # Copy the list so we can mutate it safely
        channels_queue = self.test_channel_keys.copy()

        def send_next():  # do not remove this indentation
            if self.test_aborted:
                self.append_log("Test aborted during test command sending. Stopping loop.", "neutral")
                return

            if not channels_queue:
                self.append_log("All test commands sent. Waiting 2 seconds before issuing 'run'.", "neutral")
                self.serial_port.flush()
                QTimer.singleShot(2000, lambda: not self.test_aborted and self._send_run_and_start_polling())
                return

            chan = channels_queue.pop(0)
            config = self.test_channels.get(chan, {})
            if not config:
                self.append_log(f"No config found for channel {chan}", "negative")
                QTimer.singleShot(100, lambda: not self.test_aborted and send_next())
                return

            pulses = config.get("step_time", 1)
            kh = config.get("kh", 1.0)
            test_cmd = f"test,{chan},{pulses},{kh},0.00001,"

            try:
                if not self.serial_port.isOpen():
                    self.serial_port.setPortName(self.arduino)
                    self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite)

                self.serial_port.write((test_cmd + "\n").encode())
                self.append_log(f"Sent test command: {test_cmd}", "positive")

            except Exception as e:
                self.append_log(f"Error sending test command for channel {chan}: {e}", "negative")

            QTimer.singleShot(100, lambda: not self.test_aborted and send_next())

        # Kick off the first send
        QTimer.singleShot(100, lambda: not self.test_aborted and send_next())

    def _send_run_and_start_polling(self):
        if self.test_aborted:
            self.append_log("Test was aborted before 'run' command could be sent.", "neutral")
            return

        try:
            self.serial_port.write(b"run\n")
            self.append_log("Sent 'run' command to Arduino.", "positive")
            # === Add timestamp to "Step Started" columns ===
            from datetime import datetime
            timestamp = datetime.now().strftime("%H:%M:%S %m-%d-%Y")
            # Script Tab
            script_table = self.ui.table_script
            script_headers = [script_table.horizontalHeaderItem(i).text() if script_table.horizontalHeaderItem(i) else "" for i in range(script_table.columnCount())]
            if "Step Started" in script_headers:
                col = script_headers.index("Step Started")
                script_table.setItem(self.current_step, col, QTableWidgetItem(timestamp))
                self.append_log(f"Step Started timestamp added to script tab at row {self.current_step}, col {col}.", "neutral")
            # Summary Tab
            summary_table = self.ui.table_summary
            summary_headers = [summary_table.horizontalHeaderItem(i).text() if summary_table.horizontalHeaderItem(i) else "" for i in range(summary_table.columnCount())]
            if "Step Started" in summary_headers:
                col = summary_headers.index("Step Started")
                summary_table.setItem(self.current_step, col, QTableWidgetItem(timestamp))
                self.append_log(f"Step Started timestamp added to summary tab at row {self.current_step}, col {col}.", "neutral")

            # === Set estimated time remaining LCD to step time ===
            try:
                step_time = int(self.ui.TB_stepTime.text())
            except Exception:
                step_time = 10
            self.est_time_remaining_value = step_time
            self.ui.lcd_timeRemaining.display(self.est_time_remaining_value)

            # Default to test timeout to 55x step time if no valid timeout is set
            try:
                timeout_text = self.ui.TB_testTimeout.text().strip()
                if timeout_text and float(timeout_text) > step_time * 40:
                    self.next_test_timeout_value = int(float(timeout_text))
                    self.append_log(f"Using user-specified test timeout: {self.next_test_timeout_value} seconds.", "neutral")
                else:
                    raise ValueError
            except Exception:
                self.next_test_timeout_value = step_time * 40
                self.append_log(f"No valid test timeout set. Defaulting to {self.next_test_timeout_value} seconds (15x step time).", "neutral")
            self.ui.lcd_timeout.display(self.next_test_timeout_value)

            # Safety check before starting polling
            if self.test_aborted:
                self.append_log("Test was aborted before polling could start.", "neutral")
                return

            self.update_count_timer = QTimer(self)
            self.update_count_timer.timeout.connect(self.poll_update_count)
            self.update_count_timer.start(1000)

            self.append_log("Polling for count updates started.", "neutral")

        except Exception as e:
            self.append_log(f"Error sending 'run' command: {e}", "negative")

    def time_based_test(self):
        # Handles test-based calculations when Time-Based checkbox is checked

        # Gather Kh values from added test channels
        kh_array = [channel["kh"] for channel in self.test_channels.values() if channel.get("kh") is not None]

        # Inferred watts, smallestKh, and channelNumber from metrics
        watts = abs(self.watts_from_metrics())
        smallestKh = self.smallest_kh_from_metrics(kh_array)
        

        # Get test time from UI
        test_time = int(self.ui.TB_stepTime.text()) if self.ui.TB_stepTime.text() else 0

        # Calculate time to pulses
        time_to_pulses = int(watts * test_time / (3600 * smallestKh)) if smallestKh != 0 else 1
        if time_to_pulses < 1:
            time_to_pulses = 1  # Ensure at least one pulse

        # Logging
        self.append_log(f"Calculated Time to Pulses: {time_to_pulses}", "positive")

        return time_to_pulses

    def watts_from_metrics(self):
        # Calculates total watts from RX metrics of phases A, B, and C
        try:
            power_a = float(self.ui.table_metrics.item(0, 0).text()) if self.ui.table_metrics.item(0, 0) else 0.0
            power_b = float(self.ui.table_metrics.item(1, 0).text()) if self.ui.table_metrics.item(1, 0) else 0.0
            power_c = float(self.ui.table_metrics.item(2, 0).text()) if self.ui.table_metrics.item(2, 0) else 0.0
            # Calculate total watts
            watts = power_a + power_b + power_c

            if watts <= 0:
                watts = (self.vA_sec * self.Ia_sec +
                         self.vB_sec * self.Ib_sec + 
                         self.vC_sec * self.Ic_sec)

        except (ValueError, AttributeError) as e:
            # Log the error and set watts to 0 if any parsing or attribute error occurs
            self.append_log(f"Error calculating watts from metrics: {str(e)}", "negative")
            watts = (self.vA_sec * self.Ia_sec +
                        self.vB_sec * self.Ib_sec + 
                        self.vC_sec * self.Ic_sec)
        return watts

    def add_channel(self):
        selected_channel = self.ui.dropDown_channel.currentText()

        if selected_channel:
            meter_index = int(selected_channel) - 1
            header_item = self.ui.table_activeChannels.horizontalHeaderItem(meter_index)
            if not header_item:
                header_item = QTableWidgetItem(f"Meter {selected_channel}")
                self.ui.table_activeChannels.setHorizontalHeaderItem(meter_index, header_item)

            # Highlight header
            header_item.setBackground(QBrush(QColor(0, 255, 0)))

            try:
                step_time = int(self.ui.TB_stepTime.text())
                kh_value = float(self.ui.TB_Kh.text())

                if step_time <= 0:
                    raise ValueError("Step time must be a positive, non-zero integer.")
                if kh_value <= 0:
                    raise ValueError("Kh must be a positive rational number.")
            except ValueError as e:
                self.append_log(f"Invalid input: {e}", "negative")
                QMessageBox.critical(self, "Input Error", str(e))
                return

            # Get meter metadata
            meter_form = self.ui.dropDown_meterForm.currentText()
            lv_hv = self.ui.dropDown_lvHV.currentText()
            bef_aft = self.ui.dropDown_befAft.currentText()
            serial_number = self.ui.TB_SN.text().strip()

            chan = int(selected_channel)

            # Preserve existing format
            self.test_channels[chan] = {
                "step_time": step_time,
                "kh": kh_value,
                "metadata": {
                    "meter_form": meter_form,
                    "lv_hv": lv_hv,
                    "bef_aft": bef_aft,
                    "serial_number": serial_number
                }
            }

            self.append_log(
                f"Channel {chan} added with pulses={step_time}, kh={kh_value}, "
                f"form={meter_form}, LV/HV={lv_hv}, B/A={bef_aft}, S/N={serial_number}",
                "positive"
            )
            self.append_log("Channel added.", "neutral")

    def clear_channel(self):
        selected_channel = self.ui.dropDown_channel.currentText()

        if selected_channel:
            chan = int(selected_channel)
            meter_index = chan - 1

            if chan in self.test_channels:
                del self.test_channels[chan]  # This removes step_time, kh, and metadata

            header_item = self.ui.table_activeChannels.horizontalHeaderItem(meter_index)
            if header_item:
                header_item.setBackground(QBrush(QColor(255, 255, 255)))

            self.append_log(f"Channel {chan} cleared.", "neutral")
            QMessageBox.information(self, "Channel Cleared", f"Channel {chan} has been cleared.")

    def clear_all_channels(self):
        self.test_channels.clear()  # This clears all channels including metadata

        for meter_index in range(self.ui.table_activeChannels.columnCount()):
            header_item = self.ui.table_activeChannels.horizontalHeaderItem(meter_index)
            if header_item:
                header_item.setBackground(QBrush(QColor(255, 255, 255)))

        self.append_log("All channels cleared.", "neutral")
        QMessageBox.information(self, "All Channels Cleared", "All channels have been reset.")
   
    def _paste_rx_text(self, text: str, header: str = None):
        """
        Try to paste raw RX text into a visible multi-line textbox if present.
        Falls back to append_log if no suitable widget is found.
        """
        block = f"{header}\n{text}\n" if header else f"{text}\n"
        # common candidates; first one found wins
        candidates = [
            "TB_arduinoRX", "TB_rx", "TB_serialRX", "TB_console",
            "textBrowser_rx", "textBrowser", "textEdit_rx", "textEdit"
        ]
        for name in candidates:
            w = getattr(self.ui, name, None)
            if w is None:
                continue
            try:
                if hasattr(w, "append"):         # QTextBrowser/QTextEdit
                    w.append(block.rstrip("\n"))
                elif hasattr(w, "toPlainText") and hasattr(w, "setPlainText"):
                    prev = w.toPlainText()
                    w.setPlainText(prev + ("\n" if prev and not prev.endswith("\n") else "") + block)
                else:
                    continue
                return True
            except Exception:
                pass
        # fallback so you still see it
        self.append_log(block.rstrip("\n"), "neutral")
        return False

    def poll_update_count(self):
        """Non-blocking poll: send 'update count', then read a moment later."""
        if self.test_aborted:
            self.append_log("Polling skipped — test has been aborted.", "neutral")
            if hasattr(self, 'update_count_timer') and self.update_count_timer.isActive():
                self.update_count_timer.stop()
            return

        # === Update LCDs ===
        if self.est_time_remaining_value > 0:
            self.est_time_remaining_value -= 1
        else:
            self.est_time_remaining_value = 0
        self.ui.lcd_timeRemaining.display(self.est_time_remaining_value)

        if self.next_test_timeout_value > 0:
            self.next_test_timeout_value -= 1
            self.test_timed_out = False
        else:
            self.next_test_timeout_value = 0
            self.test_timed_out = True
        self.ui.lcd_timeout.display(self.next_test_timeout_value)

        # Prevent overlapping polls
        if getattr(self, "_poll_busy", False):
            return
        self._poll_busy = True

        # Ensure port open
        try:
            if not self.serial_port.isOpen():
                self.serial_port.setPortName(self.arduino)
                if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                    self.append_log("Failed to open Arduino serial port for polling.", "negative")
                    self._poll_busy = False
                    return
        except Exception as e:
            self.append_log(f"Serial open error: {e}", "negative")
            self._poll_busy = False
            return

        # Send request (non-blocking)
        try:
            self.serial_port.write(b"update count\n")
            self.append_log("Sent 'update count' to Arduino.", "neutral")
        except Exception as e:
            self.append_log(f"Write error in poll_update_count: {e}", "negative")
            self._poll_busy = False
            return

        def _finish_read():
            if self.test_aborted:
                self._poll_busy = False
                self.append_log("Test aborted during response wait. Stopping poll.", "neutral")
                return

            response = ""
            try:
                while self.serial_port.canReadLine():
                    line = self.serial_port.readLine().data().decode("utf-8", errors="replace")
                    response += line
                while self.serial_port.bytesAvailable():
                    chunk = self.serial_port.read(self.serial_port.bytesAvailable()).data().decode("utf-8", errors="replace")
                    response += chunk
            except Exception as e:
                self.append_log(f"Read error in poll_update_count: {e}", "negative")
                self._poll_busy = False
                return

            if response.strip():
                # paste raw block into textbox (mirrors your example behavior)
                self._paste_rx_text(response.strip(), header="--- Update Count RX ---")
                self.append_log(f"Update count response:\n{response.strip()}", "neutral")

            try:
                matches = self._re_meter_counts.findall(response) if hasattr(self, "_re_meter_counts") else re.findall(r"Meter Count for Channel:\s*(\d+)\s+(\d+)", response)
                self.channel_counts = {}
                all_zero = True

                for chan_str, count_str in matches:
                    chan = int(chan_str)
                    count = int(count_str)
                    self.channel_counts[chan] = count
                    self.append_log(f"Parsed channel {chan} countdown: {count}", "neutral")
                    if count > 0:
                        all_zero = False

                if self.test_aborted:
                    self.append_log("Test aborted after parsing. Halting polling.", "neutral")
                    self._poll_busy = False
                    return

                if all_zero and len(self.channel_counts) > 0:
                    self.append_log("All channels reached 0. Proceeding.", "positive")
                    if hasattr(self, 'update_count_timer') and self.update_count_timer.isActive():
                        self.update_count_timer.stop()
                    QTimer.singleShot(1000, self.send_update_finished)
                elif self.test_timed_out:
                    self.append_log("Timeout reached — sending 'update finished' FIRST, then cancel/reset after pasting.", "negative")
                    if hasattr(self, 'update_count_timer') and self.update_count_timer.isActive():
                        self.update_count_timer.stop()
                    self._timeout_path = True
                    QTimer.singleShot(100, self.send_update_finished)
            except Exception as e:
                self.append_log(f"Parse error in poll_update_count: {e}", "negative")
            finally:
                self._poll_busy = False

        QTimer.singleShot(250, _finish_read)

    def send_update_finished(self):
        """
        Sends the 'update finished' command and schedules reading accuracies.
        Works for both normal completion and timeout path.
        """
        self.append_log("send_update_finished() called.", "neutral")

        if self.test_aborted:
            self.append_log("Aborted: Skipping 'update finished' command.", "neutral")
            return

        try:
            self.append_log("Attempting to send 'update finished' to Arduino...", "neutral")
            self.serial_port.write(b"update finished\n")
            self.append_log("Sent 'update finished' to Arduino.", "positive")
            # Give the Arduino a moment to compile the accuracy payload
            QTimer.singleShot(1000, self.read_accuracy_and_advance)
            self.append_log("Scheduled read_accuracy_and_advance after 1 second.", "neutral")
        except Exception as e:
            self.append_log(f"Error sending 'update finished': {e}", "negative")
           
    def read_accuracy_and_advance(self):
        """ Reads the accuracy response (also pastes raw to textbox), writes tables, handles timeout path, then advances. """
        self.append_log("read_accuracy_and_advance() called.", "neutral")
        from datetime import datetime

        if self.test_aborted:
            self.append_log("Aborted: Skipping accuracy read and step advance.", "neutral")
            return

        timeout_path = bool(getattr(self, "_timeout_path", False))
        retry_required = False
        fail_count = 0
        no_std_pulses = False

        try:
            self.append_log("Attempting to read accuracy response from Arduino...", "neutral")
            response = ""
            while self.serial_port.canReadLine():
                line = self.serial_port.readLine().data().decode("utf-8", errors="replace").strip()
                response += line + "\n"
                self.append_log(f"Read line from Arduino: {line}", "neutral")

            # === NEW: paste raw accuracy block to textbox for viewing ===
            if response.strip():
                self._paste_rx_text(response.strip(), header="--- Accuracy RX ---")

            self.append_log(f"Full Accuracy response:\n{response}", "neutral")

            if self.test_aborted:
                self.append_log("Aborted after reading accuracy response.", "neutral")
                return

            accuracy_matches = re.findall(r"Accuracy\s+(\d+):\s*([-+]?\d*\.\d+|\d+|inf)", response)
            self.append_log(f"Found {len(accuracy_matches)} accuracy matches in response.", "neutral")

            try:
                pass_criteria_text = self.ui.TB_testPass.text().strip()
                self.passCriteria = float(pass_criteria_text)
                self.append_log(f"Using user-specified pass criteria: {self.passCriteria}", "neutral")
            except Exception:
                self.passCriteria = 0.2
                self.append_log("Invalid or missing pass criteria. Defaulting to 0.2.", "neutral")

            for chan_str, accuracy_str in accuracy_matches:
                chan = int(chan_str)
                is_inf = accuracy_str.lower() == "inf"
                if is_inf:
                    no_std_pulses = True
                    self.append_log(f"Parsed Accuracy for Channel {chan}: inf (no std pulses)", "neutral")
                else:
                    try:
                        accuracy = float(accuracy_str)
                        self.append_log(f"Parsed Accuracy for Channel {chan}: {accuracy}", "neutral")
                        if 100 - self.passCriteria <= accuracy <= 100 + self.passCriteria:
                            self.append_log(f"Accuracy for channel {chan} is within expected range.", "positive")
                            self.repeat_step = False
                            self.repeat_tracker = 0
                        else:
                            self.append_log(f"Accuracy for channel {chan} is out of range.", "negative")
                            fail_count += 1
                            if not timeout_path:
                                retry_required = True
                    except Exception:
                        self.append_log(f"Could not parse accuracy value '{accuracy_str}' for channel {chan}.", "negative")
                        if not timeout_path:
                            retry_required = True
                            fail_count += 1

                # Summary table paste
                if self.current_step < self.ui.table_summary.rowCount():
                    col = chan
                    display_value = "NO STD PULSES" if is_inf else (f"{float(accuracy_str):.3f}" if not is_inf else "inf")
                    accuracy_item = QTableWidgetItem(display_value)
                    accuracy_item.setTextAlignment(Qt.AlignmentFlag.AlignCenter)
                    self.ui.table_summary.setItem(self.current_step, col, accuracy_item)

                    timestamp_full = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                    step_row = self.current_step
                    step_finished_col_script = self.ui.table_script.columnCount() - 1
                    self.ui.table_script.setItem(step_row, step_finished_col_script, QTableWidgetItem(timestamp_full))
                    step_finished_col_summary = self.ui.table_summary.columnCount() - 1
                    self.ui.table_summary.setItem(step_row, step_finished_col_summary, QTableWidgetItem(timestamp_full))
                    self.append_log(f"Set accuracy in table_summary at row {self.current_step}, col {col}.", "neutral")

            # Display “Step Finished” columns with human format
            timestamp_disp = datetime.now().strftime("%H:%M:%S %m-%d-%Y")
            script_table = self.ui.table_script
            script_headers = [script_table.horizontalHeaderItem(i).text() if script_table.horizontalHeaderItem(i) else "" for i in range(script_table.columnCount())]
            if "Step Finished" in script_headers:
                col = script_headers.index("Step Finished")
                script_table.setItem(self.current_step, col, QTableWidgetItem(timestamp_disp))
                self.append_log(f"Step Finished timestamp added to script tab at row {self.current_step}, col {col}.", "neutral")

            summary_table = self.ui.table_summary
            summary_headers = [summary_table.horizontalHeaderItem(i).text() if summary_table.horizontalHeaderItem(i) else "" for i in range(summary_table.columnCount())]
            if "Step Finished" in summary_headers:
                col = summary_headers.index("Step Finished")
                summary_table.setItem(self.current_step, col, QTableWidgetItem(timestamp_disp))
                self.append_log(f"Step Finished timestamp added to summary tab at row {self.current_step}, col {col}.", "neutral")

            # PASS/FAIL/TIMEOUT status
            if "Accuracy" in script_headers:
                acc_col = script_headers.index("Accuracy")

                def set_row_color(row, color):
                    for col in range(script_table.columnCount()):
                        item = script_table.item(row, col)
                        if not item:
                            item = QTableWidgetItem("")
                            script_table.setItem(row, col, item)
                        item.setBackground(QColor(color))

                if no_std_pulses:
                    script_table.setItem(self.current_step, acc_col, QTableWidgetItem("NO STD PULSES"))
                    set_row_color(self.current_step, "red")
                elif self.test_timed_out or timeout_path:
                    script_table.setItem(self.current_step, acc_col, QTableWidgetItem("TIMEOUT"))
                    set_row_color(self.current_step, "red")
                elif fail_count == 0:
                    script_table.setItem(self.current_step, acc_col, QTableWidgetItem("PASS"))
                    set_row_color(self.current_step, "green")
                else:
                    script_table.setItem(self.current_step, acc_col, QTableWidgetItem(f"{fail_count} FAIL"))
                    set_row_color(self.current_step, "red")

        except Exception as e:
            self.append_log(f"Error reading accuracy: {e}", "negative")
            return

        if self.test_aborted:
            self.append_log("Test aborted before retry or step advance.", "neutral")
            return

        # Retry logic (disabled on timeout path)
        if not timeout_path:
            if retry_required:
                self.retryCount += 1
                self.ui.lcd_retry.display(self.retryCount)
                if self.retryCount <= self.allowedRetries:
                    self.append_log(f"[RETRY] Retrying step {self.current_step + 1} (Retry {self.retryCount} of {self.allowedRetries})", "neutral")
                    QTimer.singleShot(100, self.run_time_based_and_start_tests)
                    return
                else:
                    self.append_log(f"[RETRIES EXCEEDED] Proceeding to next step. Retry count: {self.retryCount}", "negative")

        # No retry (normal or timeout)
        self.retryCount = 0
        self.ui.lcd_retry.display(self.retryCount)

        if self.initial_step:
            self.append_log("Setting initial_step to False and advancing to next step.", "neutral")
            self.initial_step = False

        # === TIMEOUT PATH HANDLING ===
        if timeout_path or self.test_timed_out:
            self.append_log("Timeout path: OUTP 0 to all voltages, then cancel/reset, then advance.", "negative")

            # 1) Immediately force-off all voltages
            try:
                self._outp0_all_voltages()
            except Exception as e:
                self.append_log(f"Error while forcing voltages OFF on timeout: {e}", "negative")

            # 2) Ensure next step does NOT skip re-arming even if same values
            setattr(self, "_force_rearm_sources", True)
            try:
                self._last_voltage_setpoints = None
            except Exception:
                pass

            # 3) Proceed with existing cancel/reset sequence, then advance
            self._timeout_path = False
            self._send_cancel_reset_sequence(repeats=2, delay_ms=500, after=self.advance_step)
            return

        # === NORMAL PATH ===
        self.append_log("Advancing to next step after accuracy read.", "neutral")
        self.advance_step()

    def abort_test(self, IsKeepingVoltage=False):
        self.test_aborted = True  # Activate kill switch
        self.test_active = False  # Sets testing flag to inactive
        self.protect_disable_buttons()
        self.clear_all_metrics()
        self.append_log("Re-enable Test Variables, Channels, and Manual Control buttons after abort.")
        # 1) Stop polling timer if active
        try:
            if hasattr(self, 'update_count_timer') and self.update_count_timer.isActive():
                self.update_count_timer.stop()
                self.update_count_timer.timeout.disconnect(self.poll_update_count)
                self.append_log("Stopped and disconnected Arduino polling timer.", "neutral")
        except Exception as e:
            self.append_log(f"Error while stopping polling timer: {e}", "negative")

        # 2) Send OUTP 0 to all sources, unless IsKeepingVoltage is True (skip Va/Vb/Vc)
        for source_key in ["Va", "Vb", "Vc", "Ia", "Ib", "Ic"]:
            if IsKeepingVoltage and source_key in ["Va", "Vb", "Vc"]:
                self.append_log(f"Skipped OUTP 0 to {source_key} (IsKeepingVoltage=True)", "neutral")
                continue
            port = self.serial_connections.get(source_key)
            if port and port.isOpen():
                try:
                    port.write(b"OUTP 0;\n")
                    self.append_log(f"Sent 'OUTP 0' to {source_key}.", "neutral")
                except Exception as e:
                    self.append_log(f"Failed to send OUTP 0 to {source_key}: {e}", "negative")
            else:
                self.append_log(f"{source_key} port not open or connected.", "negative")
            if source_key in ["Va", "Vb", "Vc"]:
                self.set_voltage_button_color("grey")
            if source_key in ["Ia", "Ib", "Ic"]:
                self.set_current_button_color("grey")

        # 3) Send 'cancel' followed by 'reset' to Arduino
        def send_cancel_then_reset():
            try:
                if not self.serial_port.isOpen():
                    self.serial_port.setPortName(self.arduino)
                    self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite)
                self.serial_port.write(b"cancel\n")
                self.append_log("Sent 'cancel' to Arduino.", "neutral")
            except Exception as e:
                self.append_log(f"Failed to send 'cancel': {e}", "negative")
                return

            # Wait 1 second, then send 'reset'
            QTimer.singleShot(2000, send_reset)

        def send_reset():
            try:
                if not self.serial_port.isOpen():
                    self.serial_port.setPortName(self.arduino)
                    self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite)
                self.serial_port.write(b"reset\n")
                self.append_log("Sent 'reset' to Arduino.", "neutral")
            except Exception as e:
                self.append_log(f"Failed to send 'reset': {e}", "negative")

        send_cancel_then_reset()

    def _send_cancel_reset_sequence(self, repeats=3, delay_ms=1000, after=None):
        """Send 'cancel' then 'reset' to Arduino, multiple times, with non-blocking delay."""
        def do_once(i):
            if not self.serial_port.isOpen():
                self.serial_port.setPortName(self.arduino)
                self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite)
            try:
                self.serial_port.write(b"cancel\n")
                self.append_log(f"[Timeout] Sent 'cancel' #{i+1}", "neutral")
            except Exception as e:
                self.append_log(f"[Timeout] Failed to send 'cancel': {e}", "negative")

            def send_reset():
                try:
                    if not self.serial_port.isOpen():
                        self.serial_port.setPortName(self.arduino)
                        self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite)
                    self.serial_port.write(b"reset\n")
                    self.append_log(f"[Timeout] Sent 'reset' #{i+1}", "neutral")
                except Exception as e:
                    self.append_log(f"[Timeout] Failed to send 'reset': {e}", "negative")

                # schedule next repeat or final after-callback
                if i + 1 < repeats:
                    QTimer.singleShot(delay_ms, lambda: do_once(i + 1))
                elif after:
                    QTimer.singleShot(300, after)

            QTimer.singleShot(delay_ms, send_reset)

        do_once(0)

    def update_est_time_remaining(self):
        if self.est_time_remaining_value > 0:
            self.est_time_remaining_value -= 1
            self.ui.lcd_timeRemaining.display(self.est_time_remaining_value)
        else:
            self.est_time_remaining_timer.stop()

    def update_next_test_timeout(self):
        if self.next_test_timeout_value > 0:
            self.next_test_timeout_value -= 1
            self.ui.lcd_timeout.display(self.next_test_timeout_value)
        else:
            self.next_test_timeout_timer.stop()

    def open_manual_control(self):
        source_ports = {
            "Va": self.Va,
            "Vb": self.Vb,
            "Vc": self.Vc,
            "Ia": self.Ia,
            "Ib": self.Ib,
            "Ic": self.Ic
        }
        # Re-enable buttons
        self.test_active = True
        self.manual_control_active = True
        self.protect_disable_buttons()
        self.open_manual_window = ManualControlWindow(self.source_ports, self.arduino, self)
        self.open_manual_window.show()
        self.open_manual_window.update_label_colors_off()

    def close_manual_control_window(self):
        if getattr(self, "open_manual_window", None):
            try:
                self.open_manual_window.close()
                self.append_log("Manual Control window closed due to exit.", "neutral")
            except Exception as e:
                self.append_log(f"Error closing Manual Control window: {e}", "negative")
            self.manual_control_active = False
            self.open_manual_window = None

    def open_temperature_rise(self):
        """Launch the Temperature Rise application in a separate process."""
        app_path = config.app_paths.get("temperature_rise", "")
        
        if not app_path:
            self.append_log("Temperature Rise application path not configured.", "negative")
            QMessageBox.warning(self, "Configuration Error", "Temperature Rise application path is not configured.")
            return
        try:
            # Check if the file exists
            if not os.path.exists(app_path):
                self.append_log(f"Temperature Rise application not found at: {app_path}", "negative")
                QMessageBox.warning(self, "File Not Found", f"Could not find application:\n{app_path}")
                return
            
            # Launch the application in a separate process
            self.temperature_rise_process = subprocess.Popen(app_path)
            self.temperature_rise_active = True
            self.append_log("Temperature Rise application launched.", "positive")
            
            # Monitor the process in the background
            QTimer.singleShot(500, self.monitor_temperature_rise_process)
            
        except Exception as e:
            self.append_log(f"Error launching Temperature Rise application: {e}", "negative")
            QMessageBox.critical(self, "Launch Error", f"Failed to launch application:\n{e}")

    def monitor_temperature_rise_process(self):
        """Check if the Temperature Rise process is still running and clean up if closed."""
        if getattr(self, "temperature_rise_process", None):
            # Check if process has finished
            if self.temperature_rise_process.poll() is not None:
                # Process has ended
                self.temperature_rise_active = False
                self.append_log("Temperature Rise application closed.", "neutral")
            else:
                # Process still running, check again in 1 second
                QTimer.singleShot(1000, self.monitor_temperature_rise_process)

    def close_temperature_rise_window(self):
        """Close the Temperature Rise application if it's running."""
        if getattr(self, "temperature_rise_process", None):
            try:
                self.temperature_rise_process.terminate()
                try:
                    self.temperature_rise_process.wait(timeout=3)
                except subprocess.TimeoutExpired:
                    self.temperature_rise_process.kill()
                self.append_log("Temperature Rise application closed.", "neutral")
            except Exception as e:
                self.append_log(f"Error closing Temperature Rise application: {e}", "negative")
            self.temperature_rise_active = False
            self.temperature_rise_process = None
    
    def _outp0_all_voltages(self):
        """Force OFF all voltage sources (Va/Vb/Vc) now, and mark that next step must re-arm."""
        for source_key in ["Va", "Vb", "Vc"]:
            port = self.serial_connections.get(source_key)
            if port and port.isOpen():
                try:
                    port.write(b"OUTP 0;\n")
                    try:
                        port.flush()
                    except Exception:
                        pass
                    self.append_log(f"[Timeout] Sent 'OUTP 0' to {source_key}.", "neutral")
                except Exception as e:
                    self.append_log(f"[Timeout] Failed to send OUTP 0 to {source_key}: {e}", "negative")
            else:
                self.append_log(f"[Timeout] {source_key} port not open or connected.", "negative")

        # UI feedback (optional)
        try:
            self.set_voltage_button_color("grey")
        except Exception:
            pass

        # ensure the next step re-arms even if script row is identical
        self._force_rearm_sources = True
        try:
            self._last_voltage_setpoints = None
        except Exception:
            pass




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
                if scale and 0.0 < curr_val <= 0.3:
                    volt_cmd = round(curr_val * 1000)
                    cmd_Ia = f"{ard_command};VOLT {volt_cmd};PHAS {angle};"
                else:
                    curr = scale_current(curr_val_str) if scale else curr_val_str
                    cmd_Ia = f"{ard_command};CURR {curr};PHAS {angle};"

            if IbCheck:
                curr_val_str = self.ui.TB_ib.toPlainText()
                curr_val = float(curr_val_str) if curr_val_str else 0.0
                angle = self.ui.TB_ibAngle.toPlainText()
                if scale and 0.0 < curr_val <= 0.3:
                    volt_cmd = round(curr_val * 1000)
                    cmd_Ib = f"{ard_command};VOLT {volt_cmd};PHAS {angle};"
                else:
                    curr = scale_current(curr_val_str) if scale else curr_val_str
                    cmd_Ib = f"{ard_command};CURR {curr};PHAS {angle};"

            if IcCheck:
                curr_val_str = self.ui.TB_ic.toPlainText()
                curr_val = float(curr_val_str) if curr_val_str else 0.0
                angle = self.ui.TB_icAngle.toPlainText()
                if scale and 0.0 < curr_val <= 0.3:
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
   
    def _ensure_arduino_port_open(self) -> bool:
        """
        Ensure self.serial_port (QSerialPort) is open on self.arduino and ready RW.
        Creates self.serial_port if missing. Returns True on success, False otherwise.
        """
        try:
            from PyQt6.QtSerialPort import QSerialPort
        except Exception as e:
            self.append_log(f"PyQt6.QtSerialPort missing: {e}", "negative")
            return False

        try:
            # Create the port object if needed
            if not hasattr(self, "serial_port") or self.serial_port is None:
                self.serial_port = QSerialPort()

            # If it's open but on the wrong device, close it
            if self.serial_port.isOpen() and self.serial_port.portName() != str(self.arduino):
                try:
                    self.serial_port.close()
                except Exception:
                    pass

            # Open if not open
            if not self.serial_port.isOpen():
                self.serial_port.setPortName(str(self.arduino))
                if not self.serial_port.open(QSerialPort.OpenModeFlag.ReadWrite):
                    self.append_log("Arduino port open failed (QSerialPort).", "negative")
                    return False

                # Optional: set sane defaults if your device expects them
                try:
                    self.serial_port.setBaudRate(115200)
                except Exception:
                    pass

            return True

        except Exception as e:
            self.append_log(f"_ensure_arduino_port_open error: {e}", "negative")
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

        # --- Determine clamp states from scripted currents ---
        ZERO_EPS = 1e-6
        LOW_MAX = 100.0
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

        # Small gap before CLAM commands
        selection_gap_ms = 200

        # --- Staggered CLAM writes (after the selection gap) ---
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
                    self.append_log(f"{label} clamp → {state} ({reason}) :: {msg.strip()}", "neutral")
                except Exception as e:
                    self.append_log(f"{label} clamp send error: {e}", "negative")

            QTimer.singleShot(selection_gap_ms + i * base_gap_ms, send)

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
            QTimer.singleShot(300, self._prep_and_continue_jog)

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

            if current_value <= 0.3:
                self.append_log(f"Sending low-current setup to {key}", "neutral")
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → CURR:PROT:STAT ON", "neutral"), p.write(b"CURR:PROT:STAT ON\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → VOLT:RANG 400", "neutral"), p.write(b"VOLT:RANG 400\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → CURR 2.0", "neutral"), p.write(b"CURR 2.0\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda k=key: self.append_log(f"Low-current jog prep complete for {k}", "positive"))
                delay_counter += delay_step
            else:
                self.append_log(f"Sending standard-current setup to {key}", "neutral")
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → CURR:PROT:STAT OFF", "neutral"), p.write(b"CURR:PROT:STAT OFF\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → VOLT:RANG 200", "neutral"), p.write(b"VOLT:RANG 200\n")))
                delay_counter += delay_step
                QTimer.singleShot(delay_counter, lambda p=port, k=key: (self.append_log(f"{k} → VOLT 150", "neutral"), p.write(b"VOLT 150\n")))
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

        try:
            with SMBus(I2C_BUS) as bus:
                gpio5_state = gpio5.value
                gpio6_state = gpio6.value

                current = self.max_jog_current

                def routing_error(msg):
                    QMessageBox.critical(self, "Routing Error", msg)
                    raise Exception(msg)

                if current <= 320.0:
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
                elif current <= 321.0:
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

            # === After routing: drive RX clamps using your central helper ===
            if not self._ensure_arduino_port_open():
                self.append_log("Arduino port not open (QSerialPort) for clamp config.", "negative")
                QTimer.singleShot(1000, self._continue_jog_activate_sources)
                return

            # Determine the actual jog values we’re about to run
            ia_val = typed_current("Ia", getattr(self, "jog_Ia", 0.0))
            ib_val = typed_current("Ib", getattr(self, "jog_Ib", 0.0))
            ic_val = typed_current("Ic", getattr(self, "jog_Ic", 0.0))

            # Temporarily reflect jog values into *_sec so _sync_clamps_to_currents()
            # uses the correct thresholds without touching broader state.
            prev_Ia_sec = getattr(self, "Ia_sec", 0.0)
            prev_Ib_sec = getattr(self, "Ib_sec", 0.0)
            prev_Ic_sec = getattr(self, "Ic_sec", 0.0)
            self.Ia_sec = ia_val
            self.Ib_sec = ib_val
            self.Ic_sec = ic_val

            # Kick the clamp sync (staggered via QTimer inside)
            #self._sync_clamps_to_currents(reason="Jog clamps after routing")

            # Allow the staggered clamp writes to complete, then restore *_sec and proceed
            # _sync_ uses 200ms + (0,150,300)ms → last at ~500ms; add margin.
            def _restore_and_continue():
                self.Ia_sec = prev_Ia_sec
                self.Ib_sec = prev_Ib_sec
                self.Ic_sec = prev_Ic_sec
                self.append_log("CT routing + clamp sync completed.", "positive")
                self._continue_jog_activate_sources()

            QTimer.singleShot(800, _restore_and_continue)

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
