"""Main application window with backend logic."""

import logging
from pathlib import Path
from datetime import datetime
from PyQt6.QtWidgets import (
    QMainWindow, QMessageBox, QFileDialog, QTableWidgetItem
)
from PyQt6.QtCore import QTimer, QThread, pyqtSignal
from PyQt6 import QtCore

from src.ui.main_window import Ui_MainWindow
from src.util.config_loader import ConfigLoader
from src.util.logging_setup import LoggingSetup
from src.util.csv_logger import CsvLogger
from src.models.statistics import StatisticsTracker
from src.devices.radian import RadianDevice
from src.models.channel import ChannelConfig, build_default_channels
from src.models.test_controller import TestController
from src.api.local_api import LocalApiClient, ApiWorker, ApiResult
from serial.tools import list_ports


class MainWindow(QMainWindow):
    """Main application window with integrated backend."""

    # Table column indices
    COL_CHANNEL = 0
    COL_CURRENT = 1
    COL_MAX = 2
    COL_MIN = 3
    COL_AVERAGE = 4
    COL_TEMP_RISE = 5

    def __init__(self):
        super().__init__()

        # API client and worker thread
        self.api = LocalApiClient(base_url="http://127.0.0.1:5055", timeout_s=5.0)
        self.api_thread = QThread(self)
        self.api_worker = ApiWorker(self.api)
        self.api_worker.moveToThread(self.api_thread)
        self.api_worker.progress.connect(self.on_api_progress)
        self.api_worker.finished.connect(self.on_api_finished)
        self.api_thread.start()

        # Configuration
        self.config = ConfigLoader()
        self.base_dir = self.config.base_dir
        self.logs_dir = self.config.logs_dir
        self.data_dir = self.config.data_dir

        # Logging
        logging_setup = LoggingSetup(
            self.config.get_logs_dir(),
            level=self.config.get("logging.level", "DEBUG"),
            log_name=self.config.get("logging.developer_log"),
        )
        self.logger = logging_setup.setup_logger("SmartMeterApp")
        self.logger.info("=" * 60)
        self.logger.info("Application starting")
        self.logger.info(f"Mock Mode: {self.config.is_mock_mode()}")
        self.logger.info(f"Poll Interval: {self.config.get_poll_interval_ms()}ms")
        self.logger.info("=" * 60)

        # UI
        self.ui = Ui_MainWindow()
        self.ui.setupUi(self)

        # Real model objects (replacing Null* stubs)
        self.statistics = StatisticsTracker()
        self.test_controller = TestController()
        self.csv_logger: CsvLogger | None = None

        # Channel configuration (60 channels across 3 slots)
        self.channel_configs: list[ChannelConfig] = build_default_channels()

        # Active channels for the current test
        self._active_channels: list[int] = []
        # Map channel_id -> table row index for fast updates
        self._channel_row_map: dict[int, int] = {}

        # State
        self.test_start_time: datetime | None = None
        self.test_end_time: datetime | None = None
        self.daq_connected = False

        self.radian_connected = False
        self.radian_device = RadianDevice(logger=self.logger)

        # Wire UI
        self.setup_connections()
        self.populate_defaults()

        # Poll timer
        self.poll_timer = QTimer()
        self.poll_timer.timeout.connect(self.on_poll)
        self.poll_interval_ms = self.config.get_poll_interval_ms()

        self.setWindowTitle("Smart Meter GUI - Test & Development")
        self.logger.info("Application initialized successfully")

    # ----------------------------------------------------------------
    # Setup
    # ----------------------------------------------------------------

    def setup_connections(self):
        """Connect UI signals to slots."""
        # DAQ tab
        self.ui.pb_Connect34970A.clicked.connect(self.on_connect_34970a)
        self.ui.pb_setVoltage.clicked.connect(self.on_set_voltage)
        self.ui.pb_voltageOff.clicked.connect(self.on_voltage_off)
        self.ui.pB_Start.clicked.connect(self.on_start_test)

        # Radian
        self.ui.pb_ConnectRadian.clicked.connect(self.on_connect_radian)

        # Cal Inst
        self.ui.pb_OpenCalInstConx.clicked.connect(self.on_open_cal_inst)
        self.ui.pb_CloseCalInstConx.clicked.connect(self.on_close_cal_inst)

        # PAC Power
        self.ui.pushButton.clicked.connect(self.on_connect_pac_power)

        # Data filter checkboxes
        self.ui.cb_VoltsData.stateChanged.connect(self.on_data_filter_changed)
        self.ui.cb_CurrentData.stateChanged.connect(self.on_data_filter_changed)
        self.ui.cb_DataFrequency.stateChanged.connect(self.on_data_filter_changed)
        self.ui.cb_PhaseData.stateChanged.connect(self.on_data_filter_changed)

        # Mode selection
        self.ui.button_Free.toggled.connect(self.on_mode_changed)
        self.ui.button_Readings.toggled.connect(self.on_mode_changed)
        self.ui.button_Duration.toggled.connect(self.on_mode_changed)

        # Slot selection
        self.ui.button_slot100.toggled.connect(self.on_slot_changed)
        self.ui.button_slot200.toggled.connect(self.on_slot_changed)
        self.ui.button_slot300.toggled.connect(self.on_slot_changed)

    def refresh_serial_ports(self):
        ports = [p.device for p in list_ports.comports()]
        self.ui.cb_Port24970A.clear()
        self.ui.cb_Port24970A.addItems(ports)
        self.ui.cB_PortRadian.clear()
        self.ui.cB_PortRadian.addItems(ports)
        self.logger.info(f"Detected serial ports: {ports}")

    def populate_defaults(self):
        """Populate default values in UI elements."""
        self.ui.cB_readIntervals.setCurrentIndex(0)
        self.ui.button_Free.setChecked(True)
        self.ui.button_slot100.setChecked(True)

        baud_rates = ["9600", "19200", "38400", "57600", "115200"]
        self.ui.cb_baudRate24970A.clear()
        self.ui.cb_baudRate24970A.addItems(baud_rates)

        self.update_timestamp()
        self.logger.info("UI defaults populated")
        self.refresh_serial_ports()

    def update_timestamp(self):
        current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        self.ui.label_timeStamp.setText(current_time)

    # ----------------------------------------------------------------
    # SCPI helpers
    # ----------------------------------------------------------------

    def daq_write(self, action: str, cmd: str):
        self.api_post(action, "/daq/write", {"cmd": cmd})

    def daq_query(self, action: str, cmd: str):
        self.api_post(action, "/daq/query", {"cmd": cmd})

    # ----------------------------------------------------------------
    # API communication
    # ----------------------------------------------------------------

    def api_get(self, action: str, path: str):
        QtCore.QMetaObject.invokeMethod(
            self.api_worker,
            "do_get",
            QtCore.Qt.ConnectionType.QueuedConnection,
            QtCore.Q_ARG(str, action),
            QtCore.Q_ARG(str, path),
        )

    def api_post(self, action: str, path: str, json_payload: dict):
        payload = {"path": path, "json": json_payload}
        QtCore.QMetaObject.invokeMethod(
            self.api_worker,
            "do_post",
            QtCore.Qt.ConnectionType.QueuedConnection,
            QtCore.Q_ARG(str, action),
            QtCore.Q_ARG(dict, payload),
        )

    def on_api_progress(self, msg: str):
        self.ui.textBrowser_State.setText(msg)

    def on_api_finished(self, action: str, res: ApiResult):
        if not res.ok:
            self.logger.error(f"{action} failed: {res.status} {res.error}")
            QMessageBox.critical(self, "API Error", f"{action} failed:\n{res.error}")
            return

        data = res.data or {}

        if action == "daq_connect":
            self.daq_connected = True
            self.ui.pb_Connect34970A.setText("Disconnect")
            self.ui.textBrowser_State.setText("CONNECTED")

        elif action == "daq_disconnect":
            self.daq_connected = False
            self.ui.pb_Connect34970A.setText("Connect")
            self.ui.textBrowser_State.setText("DISCONNECTED")

        elif action == "daq_status":
            self.ui.textBrowser_State.setText(str(data))

        elif action == "daq_idn":
            self.ui.textBrowser_State.setText(str(data))

        elif action == "daq_latest":
            self.logger.debug(f"RAW LATEST: {data}")
            self.apply_latest_reading(data)

        elif action == "daq_setup":
            self.ui.textBrowser_State.setText("DAQ setup complete")

        elif action == "daq_run":
            self.ui.textBrowser_State.setText("RUNNING")

        elif action == "daq_stop":
            self.ui.textBrowser_State.setText("STOPPED")

        elif action == "daq_query":
            self.ui.textBrowser_State.setText(data.get("response", str(data)))

        elif action == "daq_read":
            resp = data.get("response", "")
            self.apply_daq_response(resp)

        elif action == "daq_err":
            err = data.get("error", "")
            if err and not err.startswith('+0,"No error"'):
                self.ui.textBrowser_lowerData.append(f"SYST:ERR? -> {err}")
        
        elif action == "radian_connect":
            self.radian_connected = True
            self.ui.pb_ConnectRadian.setText("Disconnect")
            self.ui.textBrowser_State.setText(f"RADIAN CONNECTED: {data.get('port', '')}")

        elif action == "radian_disconnect":
            self.radian_connected = False
            self.ui.pb_ConnectRadian.setText("Connect")
            self.ui.textBrowser_State.setText("RADIAN DISCONNECTED")

        elif action == "radian_identify":
            response = data.get("response", "")
            self.ui.textBrowser_lowerData.append(f"Radian ID: {response}")

        # Check SCPI errors only after DAQ command actions
        if action in (
            "daq_read", "daq_write", "daq_conf", "daq_scan",
            "daq_abort", "daq_cls", "daq_init", "daq_fmt_chan",
            "daq_fmt_time", "daq_tc_type",
        ):
            self.api_get("daq_err", "/daq/err")

        self.logger.debug(
            f"API finished {action}: status={res.status} data={res.data} err={res.error}"
        )

    # ----------------------------------------------------------------
    # Channel / table helpers
    # ----------------------------------------------------------------

    def get_selected_daq_channels(self) -> list[int]:
        """Return channel numbers for the selected slot."""
        if self.ui.button_slot100.isChecked():
            return list(range(101, 121))
        elif self.ui.button_slot200.isChecked():
            return list(range(201, 221))
        else:
            return list(range(301, 321))

    def _get_channel_config(self, channel_id: int) -> ChannelConfig | None:
        for cfg in self.channel_configs:
            if cfg.channel_number == channel_id:
                return cfg
        return None

    def setup_data_table(self, channels: list[int]):
        """Initialize the data table with one row per channel."""
        table = self.ui.tableWidget_Data
        headers = ["Channel", "Current", "Max", "Min", "Average", "Temp Rise"]
        table.setColumnCount(len(headers))
        table.setHorizontalHeaderLabels(headers)
        table.setRowCount(len(channels))

        self._channel_row_map = {}
        for row, ch_id in enumerate(channels):
            self._channel_row_map[ch_id] = row
            cfg = self._get_channel_config(ch_id)
            name = cfg.channel_name if cfg else f"CH{ch_id}"
            table.setItem(row, self.COL_CHANNEL, QTableWidgetItem(f"{name}@{ch_id}"))
            for col in range(1, len(headers)):
                table.setItem(row, col, QTableWidgetItem("--"))

    def update_table_row(self, channel_id: int):
        """Refresh one table row from statistics."""
        row = self._channel_row_map.get(channel_id)
        if row is None:
            return
        stats = self.statistics.get_channel(channel_id)
        if stats is None:
            return

        table = self.ui.tableWidget_Data
        table.item(row, self.COL_CURRENT).setText(f"{stats.current:.2f}")
        table.item(row, self.COL_MAX).setText(
            f"{stats.max:.2f}" if stats.count > 0 else "--"
        )
        table.item(row, self.COL_MIN).setText(
            f"{stats.min:.2f}" if stats.count > 0 else "--"
        )
        table.item(row, self.COL_AVERAGE).setText(f"{stats.average:.2f}")
        temp_rise = self.statistics.get_temp_rise(channel_id)
        table.item(row, self.COL_TEMP_RISE).setText(f"{temp_rise:.2f}")

    # ----------------------------------------------------------------
    # DAQ tab handlers
    # ----------------------------------------------------------------

    def on_connect_34970a(self):
        if self.ui.pb_Connect34970A.text() == "Disconnect":
            self.api_post("daq_disconnect", "/daq/disconnect", {})
            return

        baud_rate = int(self.ui.cb_baudRate24970A.currentText())
        port = self.ui.cb_Port24970A.currentText()
        if not port:
            QMessageBox.warning(self, "Connection Error", "Please select a COM port")
            return

        self.api_post("daq_connect", "/daq/connect", {"port": port, "baud": baud_rate})
        self.api_get("health", "/health")
        self.api_get("daq_status", "/daq/status")
        self.api_get("daq_idn", "/daq/idn")

    def on_set_voltage(self):
        voltage_str = self.ui.lineEdit_Voltage.text()
        try:
            voltage = float(voltage_str)
            if voltage < 0 or voltage > 300:
                raise ValueError("Voltage must be between 0 and 300V")
            self.logger.info(f"Setting voltage to {voltage}V")
            # TODO: Route to Cal Inst or PAC Power once backend endpoints exist
            self.ui.textBrowser_State.setText(f"Voltage set to {voltage}V")
        except ValueError as e:
            QMessageBox.warning(self, "Invalid Input", f"Invalid voltage: {e}")
            self.logger.warning(f"Invalid voltage input: {voltage_str}")

    def on_voltage_off(self):
        self.logger.info("Turning off voltage")
        self.ui.textBrowser_State.setText("Voltage OFF")
        # TODO: Route to Cal Inst or PAC Power once backend endpoints exist

    def on_start_test(self):
        # Stop test if already running
        if self.ui.pB_Start.text() == "Stop Test":
            self.poll_timer.stop()
            self.test_controller.stop()
            if self.csv_logger:
                self.csv_logger.close()
                self.csv_logger = None
            self.test_end_time = datetime.now()
            self.ui.pB_Start.setText("Start Reading!")
            self.ui.pB_Start.setStyleSheet("")
            self.ui.textBrowser_State.setText("STOPPED")
            self._update_test_info()
            return

        channels = self.get_selected_daq_channels()
        scan_list = ",".join(str(ch) for ch in channels)
        self._active_channels = channels

        # Reset statistics for new test
        self.statistics.reset()

        # Setup data table
        self.setup_data_table(channels)

        # Configure test controller mode
        if self.ui.button_Free.isChecked():
            self.test_controller.mode = TestController.MODE_FREE
        elif self.ui.button_Readings.isChecked():
            self.test_controller.mode = TestController.MODE_READINGS
            # TODO: Add a line edit for target readings in the UI
            self.test_controller.target_readings = 100
        else:
            self.test_controller.mode = TestController.MODE_DURATION
            # TODO: Add HH:MM:SS input fields in the UI
            self.test_controller.target_duration_s = 3600  # default 1 hour

        # Initialize CSV logger
        self.csv_logger = CsvLogger(self.data_dir)
        self.csv_logger.initialize(channels)
        self.logger.info(f"CSV logging to {self.csv_logger.filepath}")

        # Choose sensor type
        if self.ui.button_RTD.isChecked():
            conf = f"CONF:TEMP RTD,(@{scan_list})"
        else:
            conf = f"CONF:TEMP TC,(@{scan_list})"
            self.daq_write("daq_tc_type", "SENS:TEMP:TC:TYPE K")

        # Configure DAQ via SCPI
        self.daq_write("daq_abort", "ABOR")
        self.daq_write("daq_cls", "*CLS")
        self.daq_write("daq_conf", conf)
        self.daq_write("daq_scan", f"ROUT:SCAN (@{scan_list})")
        self.daq_write("daq_fmt_chan", "FORM:READ:CHAN ON")
        self.daq_write("daq_fmt_time", "FORM:READ:TIME ON")
        self.daq_write("daq_init", "INIT")

        # Start test
        self.test_controller.start()
        self.test_start_time = datetime.now()
        self.ui.pB_Start.setText("Stop Test")
        self.ui.pB_Start.setStyleSheet("background-color: #ff6b6b;")
        self.ui.textBrowser_State.setText("RUNNING")
        self.ui.label_startTime.setText(
            f"Start Time: {self.test_start_time.strftime('%H:%M:%S')}"
        )
        self.poll_timer.start(self.poll_interval_ms)

    def on_poll(self):
        """Timer callback: request next reading from DAQ."""
        # Check if test should auto-stop
        if self.test_controller.should_stop():
            self.on_test_complete()
            return
        self.daq_query("daq_read", "READ?")

    def on_test_complete(self):
        """Handle test completion (auto-stop from mode or manual)."""
        self.poll_timer.stop()
        self.test_controller.stop()
        self.test_end_time = datetime.now()
        duration = (self.test_end_time - self.test_start_time).total_seconds() if self.test_start_time else 0

        self.logger.info(
            f"Test completed. Duration: {duration:.2f}s, "
            f"Readings: {self.test_controller.reading_count}"
        )

        if self.csv_logger:
            self.csv_logger.close()
            self.csv_logger = None

        self.ui.pB_Start.setText("Start Reading!")
        self.ui.pB_Start.setStyleSheet("")
        self.ui.textBrowser_State.setText("TEST COMPLETE")
        self._update_test_info()

        QMessageBox.information(
            self,
            "Test Complete",
            f"Test completed!\n\n"
            f"Readings: {self.test_controller.reading_count}\n"
            f"Duration: {duration:.2f}s",
        )

    def _update_test_info(self):
        """Update the information panel labels."""
        if self.test_end_time:
            self.ui.label_terminateTime.setText(
                f"Terminate Time: {self.test_end_time.strftime('%H:%M:%S')}"
            )
        if self.test_start_time and self.test_end_time:
            duration = (self.test_end_time - self.test_start_time).total_seconds()
            self.ui.label_Duration.setText(f"Duration: {duration:.1f}s")
        self.ui.label_totalNumReadings.setText(
            f"Total Number of Readings: {self.test_controller.reading_count}"
        )

    # ----------------------------------------------------------------
    # Data display
    # ----------------------------------------------------------------

    def apply_latest_reading(self, data: dict):
        """Handle structured reading data (from /daq/latest)."""
        if not data:
            return
        channels = data.get("channels", data)
        for ch_str, value in channels.items():
            ch_id = int(ch_str)
            cfg = self._get_channel_config(ch_id)
            calibrated = cfg.apply_calibration(float(value)) if cfg else float(value)
            self.statistics.update_channel(ch_id, calibrated)
            self.update_table_row(ch_id)

    def apply_daq_response(self, resp: str):
        """Parse raw READ? response and update statistics + table."""
        resp = (resp or "").strip()
        if not resp:
            return

        # Append raw data to lower text browser
        self.ui.textBrowser_lowerData.append(resp)

        # Parse comma-separated response
        # With FORM:READ:CHAN ON and FORM:READ:TIME ON, format is:
        # value, timestamp, channel, value, timestamp, channel, ...
        parts = [p.strip() for p in resp.split(",") if p.strip()]

        channel_values: dict[int, float] = {}
        timestamp_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")

        i = 0
        while i + 2 < len(parts):
            try:
                raw_value = float(parts[i])
                ts = parts[i + 1].strip('"')
                ch_id = int(float(parts[i + 2]))

                # Apply calibration
                cfg = self._get_channel_config(ch_id)
                calibrated = cfg.apply_calibration(raw_value) if cfg else raw_value

                # Update statistics
                self.statistics.update_channel(ch_id, calibrated)
                self.update_table_row(ch_id)
                channel_values[ch_id] = calibrated

                if not timestamp_str:
                    timestamp_str = ts
            except (ValueError, IndexError) as e:
                self.logger.warning(f"Parse error at index {i}: {e}")
            i += 3

        # Record reading in test controller
        if channel_values:
            self.test_controller.record_reading()

            # Log to CSV
            if self.csv_logger:
                self.csv_logger.append_row(timestamp_str, channel_values)

            # Update reading count display
            self.ui.label_totalNumReadings.setText(
                f"Total Number of Readings: {self.test_controller.reading_count}"
            )

    # ----------------------------------------------------------------
    # Radian tab handlers
    # ----------------------------------------------------------------

    def on_connect_radian(self):
        """Connect or disconnect to Radian power analyzer."""
        if self.ui.pb_ConnectRadian.text() == "Disconnect":
            self.api_post("radian_disconnect", "/radian/disconnect", {})
            return

        port = self.ui.cB_PortRadian.currentText()
        baud_rate = 9600

        if not port:
            QMessageBox.warning(self, "Connection Error", "Please select a COM port for Radian")
            return

        self.logger.info(f"Connecting to Radian on {port} @ {baud_rate}")
        self.api_post("radian_connect", "/radian/connect", {"port": port, "baud": baud_rate})

    # ----------------------------------------------------------------
    # Cal Inst handlers
    # ----------------------------------------------------------------

    def on_open_cal_inst(self):
        board_id = self.ui.spinBox_BoardID.value()
        prim_addr = self.ui.spinBox_PrimAddress.value()
        sec_addr = self.ui.cb_SecAddress.currentText()
        self.logger.info(
            f"Opening Cal Inst - Board: {board_id}, Primary: {prim_addr}, Secondary: {sec_addr}"
        )
        QMessageBox.information(
            self, "Cal Inst", "Cal Inst connection opened (Mock)"
        )

    def on_close_cal_inst(self):
        self.logger.info("Closing Cal Inst connection")
        QMessageBox.information(
            self, "Cal Inst", "Cal Inst connection closed (Mock)"
        )

    # ----------------------------------------------------------------
    # PAC Power handlers
    # ----------------------------------------------------------------

    def on_connect_pac_power(self):
        baud_rate = self.ui.lineEdit.text()
        port = self.ui.comboBox_2.currentText()
        self.logger.info(f"Connecting to PAC Power on {port} at {baud_rate} baud")
        QMessageBox.information(
            self, "PAC Power", "PAC Power connection (Mock implementation)"
        )

    # ----------------------------------------------------------------
    # Filter / mode / slot handlers
    # ----------------------------------------------------------------

    def on_data_filter_changed(self):
        filters = []
        if self.ui.cb_VoltsData.isChecked():
            filters.append("Volts")
        if self.ui.cb_CurrentData.isChecked():
            filters.append("Current")
        if self.ui.cb_DataFrequency.isChecked():
            filters.append("Frequency")
        if self.ui.cb_PhaseData.isChecked():
            filters.append("Phase")
        self.logger.debug(f"Data filters: {', '.join(filters) if filters else 'None'}")

    def on_mode_changed(self):
        if self.ui.button_Free.isChecked():
            mode = "Free"
        elif self.ui.button_Readings.isChecked():
            mode = "# of Readings"
        else:
            mode = "Duration"
        self.logger.debug(f"Test mode changed to: {mode}")

    def on_slot_changed(self):
        if self.ui.button_slot100.isChecked():
            slot = "Slot 100"
        elif self.ui.button_slot200.isChecked():
            slot = "Slot 200"
        else:
            slot = "Slot 300"
        self.logger.debug(f"Slot changed to: {slot}")

    # ----------------------------------------------------------------
    # Window close
    # ----------------------------------------------------------------

    def closeEvent(self, event):
        if self.test_controller.is_running:
            reply = QMessageBox.question(
                self,
                "Test Running",
                "A test is currently running. Do you want to stop it and exit?",
                QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No,
            )
            if reply == QMessageBox.StandardButton.Yes:
                self.test_controller.stop()
                self.poll_timer.stop()
                if self.csv_logger:
                    self.csv_logger.close()
                self.logger.info("Application closed while test was running")
                event.accept()
            else:
                event.ignore()
        else:
            self.logger.info("Application closed normally")
            event.accept()
