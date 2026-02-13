"""Main application window with backend logic."""

import logging
from pathlib import Path
from datetime import datetime
from PyQt6.QtWidgets import (
    QMainWindow, QMessageBox, QFileDialog, QTableWidgetItem, QVBoxLayout,
    QTextBrowser, QHBoxLayout, QPushButton, QComboBox, QLabel,
)
from PyQt6.QtCore import QTimer, QThread, pyqtSignal, QSettings, QObject
from PyQt6.QtGui import QColor, QTextCharFormat, QFont
from PyQt6 import QtCore

from src.ui.main_window import Ui_MainWindow
from src.util.config_loader import ConfigLoader
from src.util.logging_setup import LoggingSetup
from src.util.csv_logger import CsvLogger
from src.models.statistics import StatisticsTracker
from src.devices.radian import RadianDevice
from src.devices.pac_power import PacPowerDevice
from src.devices.cal_inst import CalInstDevice
from src.models.channel import ChannelConfig, build_default_channels
from src.models.test_controller import TestController
from src.models.comparison import ComparisonTracker
from src.api.local_api import LocalApiClient, ApiWorker, ApiResult
from src.ui.plot_widget import PlotWidget
from serial.tools import list_ports


class _LogSignalEmitter(QObject):
    """Bridge between Python logging and Qt signals."""
    log_record = pyqtSignal(str, int)  # message, level


class QtLogHandler(logging.Handler):
    """Logging handler that emits Qt signals for each log record."""

    # Color map for log levels
    LEVEL_COLORS = {
        logging.DEBUG: "#888888",
        logging.INFO: "#000000",
        logging.WARNING: "#cc8800",
        logging.ERROR: "#cc0000",
        logging.CRITICAL: "#ff0000",
    }

    def __init__(self):
        super().__init__()
        self.emitter = _LogSignalEmitter()

    def emit(self, record: logging.LogRecord):
        try:
            msg = self.format(record)
            self.emitter.log_record.emit(msg, record.levelno)
        except Exception:
            self.handleError(record)


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

        self.pac_connected = False
        self.pac_power_device = PacPowerDevice(logger=self.logger)

        self.cal_inst_connected = False
        self.cal_inst_device = CalInstDevice(logger=self.logger)

        # Channel comparison (5 pairs)
        self.comparison = ComparisonTracker(logger=self.logger)
        # UI widget references for comparison pairs: (comboA, comboB, diffLineEdit)
        self._compare_widgets = [
            (self.ui.comboBox_3, self.ui.comboBox_8, self.ui.lineEdit_2),
            (self.ui.comboBox_4, self.ui.comboBox_9, self.ui.lineEdit_3),
            (self.ui.comboBox_5, self.ui.comboBox_10, self.ui.lineEdit_4),
            (self.ui.comboBox_6, self.ui.comboBox_11, self.ui.lineEdit_5),
            (self.ui.comboBox_7, self.ui.comboBox_12, self.ui.lineEdit_6),
        ]

        # Plot widget (embedded in tab_2 "DAQ Plot")
        self.plot_widget = PlotWidget(logger=self.logger, parent=self.ui.tab_2)
        tab2_layout = QVBoxLayout(self.ui.tab_2)
        tab2_layout.setContentsMargins(4, 4, 4, 4)
        tab2_layout.addWidget(self.plot_widget)

        # Error Log tab (tab_5)
        # self._setup_error_log_tab()

        # Wire UI
        self.setup_connections()
        self.populate_defaults()
        self._restore_settings()

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
        self.ui.comboBox_2.clear()
        self.ui.comboBox_2.addItems(ports)
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

        # Cal Inst handlers
        elif action == "cal_inst_connect":
            self.cal_inst_connected = True
            self.cal_inst_device.update_from_connect(data)
            self.ui.pb_OpenCalInstConx.setEnabled(False)
            self.ui.pb_CloseCalInstConx.setEnabled(True)
            self.ui.spinBox_BoardID.setEnabled(False)
            self.ui.spinBox_PrimAddress.setEnabled(False)
            self.ui.cb_SecAddress.setEnabled(False)
            timeout = data.get("timeout", "")
            self.ui.textBrowser_State.setText(f"CAL INST CONNECTED (Timeout: {timeout})")
            self.ui.textBrowser_lowerData.append(
                f"GPIB Connected: Board={data.get('boardId')}, "
                f"Addr={data.get('primaryAddress')}, "
                f"SecAddr={data.get('secondaryAddress')}"
            )

        elif action == "cal_inst_disconnect":
            self.cal_inst_connected = False
            self.cal_inst_device.set_connected(False)
            self.ui.pb_OpenCalInstConx.setEnabled(True)
            self.ui.pb_CloseCalInstConx.setEnabled(False)
            self.ui.spinBox_BoardID.setEnabled(True)
            self.ui.spinBox_PrimAddress.setEnabled(True)
            self.ui.cb_SecAddress.setEnabled(True)
            self.ui.textBrowser_State.setText("CAL INST DISCONNECTED")

        elif action == "cal_inst_set_voltage":
            self.cal_inst_device.update_voltage_setpoint(data)
            self.ui.textBrowser_State.setText(
                f"Cal Inst voltage set to {data.get('voltage', '?')}V"
            )

        elif action == "cal_inst_voltage_off":
            self.cal_inst_device.update_voltage_setpoint(data)
            self.ui.textBrowser_State.setText("Cal Inst voltage OFF")

        # PAC Power handlers
        elif action == "pac_connect":
            self.pac_connected = True
            self.pac_power_device.set_connected(True)
            self.ui.pushButton.setText("Disconnect")
            self.ui.textBrowser_State.setText(f"PAC POWER CONNECTED: {data.get('port', '')}")
            self.api_get("pac_identify", "/pac/identify")

        elif action == "pac_disconnect":
            self.pac_connected = False
            self.pac_power_device.set_connected(False)
            self.ui.pushButton.setText("Connect")
            self.ui.textBrowser_State.setText("PAC POWER DISCONNECTED")

        elif action == "pac_identify":
            idn = data.get("idn", str(data))
            self.pac_power_device.idn = idn
            self.ui.textBrowser_lowerData.append(f"PAC IDN: {idn}")

        elif action == "pac_set_voltage":
            self.ui.textBrowser_State.setText(f"Voltage set to {data.get('voltage', '?')}V")

        elif action == "pac_output_on":
            self.ui.textBrowser_State.setText("PAC OUTPUT ON")

        elif action == "pac_output_off":
            self.ui.textBrowser_State.setText("PAC OUTPUT OFF")

        elif action == "pac_measure_all":
            self.pac_power_device.update_voltage_from_api(data)
            self.pac_power_device.update_current_from_api(data)
            self.pac_power_device.update_frequency_from_api(data)
            self.pac_power_device.update_power_from_api(data)
            self.ui.textBrowser_lowerData.append(
                f"PAC Metrics: {self.pac_power_device.metrics.to_dict()}"
            )

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
            if self.cal_inst_connected:
                self.api_post("cal_inst_set_voltage", "/cal-inst/set-voltage", {"voltage": voltage})
            elif self.pac_connected:
                self.api_post("pac_set_voltage", "/pac/set-voltage", {"voltage": voltage})
                self.api_post("pac_output_on", "/pac/output-on", {})
            else:
                QMessageBox.warning(self, "Not Connected", "No voltage source connected (Cal Inst or PAC Power)")
        except ValueError as e:
            QMessageBox.warning(self, "Invalid Input", f"Invalid voltage: {e}")
            self.logger.warning(f"Invalid voltage input: {voltage_str}")

    def on_voltage_off(self):
        self.logger.info("Turning off voltage")
        if self.cal_inst_connected:
            self.api_post("cal_inst_voltage_off", "/cal-inst/voltage-off", {})
        elif self.pac_connected:
            self.api_post("pac_output_off", "/pac/output-off", {})
        else:
            QMessageBox.warning(self, "Not Connected", "No voltage source connected (Cal Inst or PAC Power)")

    def on_start_test(self):
        # Stop test if already running
        if self.ui.pB_Start.text() == "Stop Test":
            self.poll_timer.stop()
            self.test_controller.stop()
            if self.csv_logger:
                self.csv_logger.close()
                self.csv_logger = None
            self.test_end_time = datetime.now()
            self.plot_widget.stop_test()
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

        # Populate compare comboboxes with active channels
        self._populate_compare_combos(channels)
        self.comparison.reset()

        # Initialize plot with channel names
        channel_names = []
        for ch_id in channels:
            cfg = self._get_channel_config(ch_id)
            name = cfg.channel_name if cfg else f"CH{ch_id}"
            channel_names.append(f"{name}@{ch_id}")
        self.plot_widget.start_test(channel_names)

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

        self.plot_widget.stop_test()
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

            # Update channel comparisons
            self._update_comparisons(channel_values)

            # Update plot with temperature data
            plot_data = {}
            for ch_id, val in channel_values.items():
                cfg = self._get_channel_config(ch_id)
                name = cfg.channel_name if cfg else f"CH{ch_id}"
                plot_data[f"{name}@{ch_id}"] = val
            self.plot_widget.add_temperature_points(plot_data)

            # Update reading count display
            self.ui.label_totalNumReadings.setText(
                f"Total Number of Readings: {self.test_controller.reading_count}"
            )

    # ----------------------------------------------------------------
    # Channel comparison
    # ----------------------------------------------------------------

    def _populate_compare_combos(self, channels: list[int]):
        """Populate all compare ComboBoxes with available channel numbers."""
        ch_strings = [""] + [str(ch) for ch in channels]
        for cb_a, cb_b, diff_edit in self._compare_widgets:
            cb_a.clear()
            cb_a.addItems(ch_strings)
            cb_b.clear()
            cb_b.addItems(ch_strings)
            diff_edit.setReadOnly(True)
            diff_edit.setText("")

    def _update_comparisons(self, channel_values: dict[int, float]):
        """Read compare ComboBox selections, update tracker, and display results."""
        for i, (cb_a, cb_b, diff_edit) in enumerate(self._compare_widgets):
            ch_a_text = cb_a.currentText()
            ch_b_text = cb_b.currentText()

            if ch_a_text and ch_b_text:
                try:
                    self.comparison.configure_pair(i, int(ch_a_text), int(ch_b_text))
                except ValueError:
                    continue
            else:
                self.comparison.configure_pair(i, 0, 0)
                diff_edit.setText("")

        exceeded = self.comparison.update_readings(channel_values)

        for i, (cb_a, cb_b, diff_edit) in enumerate(self._compare_widgets):
            pair = self.comparison.pairs[i]
            if not pair.is_configured:
                continue

            diff_edit.setText(f"{pair.difference:.3f}")

            # Color feedback: red stylesheet on the hotter channel's combobox
            red_style = "color: red; font-weight: bold;"
            normal_style = ""
            if pair.a_is_hotter:
                cb_a.setStyleSheet(red_style)
                cb_b.setStyleSheet(normal_style)
            elif pair.b_is_hotter:
                cb_a.setStyleSheet(normal_style)
                cb_b.setStyleSheet(red_style)
            else:
                cb_a.setStyleSheet(normal_style)
                cb_b.setStyleSheet(normal_style)

            # Highlight difference if threshold exceeded
            if pair.threshold_exceeded:
                diff_edit.setStyleSheet("background-color: #ffcccc; color: red; font-weight: bold;")
            else:
                diff_edit.setStyleSheet("")

        # Log threshold warnings
        if exceeded:
            for idx in exceeded:
                pair = self.comparison.pairs[idx]
                self.ui.textBrowser_lowerData.append(
                    f"WARNING: Threshold exceeded on pair {idx + 1}: "
                    f"CH{pair.channel_a} vs CH{pair.channel_b}, "
                    f"diff={pair.difference:.3f}\u00b0C >= {pair.threshold}\u00b0C"
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
        """Open GPIB connection to California Instruments CA501TAC."""
        board_id = self.ui.spinBox_BoardID.value()
        prim_addr = self.ui.spinBox_PrimAddress.value()
        sec_addr_text = self.ui.cb_SecAddress.currentText()

        try:
            sec_addr = int(sec_addr_text) if sec_addr_text and sec_addr_text != "None" else 0
        except ValueError:
            sec_addr = 0

        self.logger.info(
            f"Opening Cal Inst - Board: {board_id}, Primary: {prim_addr}, Secondary: {sec_addr}"
        )
        self.api_post("cal_inst_connect", "/cal-inst/connect", {
            "boardId": board_id,
            "primaryAddress": prim_addr,
            "secondaryAddress": sec_addr,
        })

    def on_close_cal_inst(self):
        """Close GPIB connection to Cal Inst."""
        self.logger.info("Closing Cal Inst connection")
        self.api_post("cal_inst_disconnect", "/cal-inst/disconnect", {})

    # ----------------------------------------------------------------
    # PAC Power handlers
    # ----------------------------------------------------------------

    def on_connect_pac_power(self):
        """Connect or disconnect to PAC Power supply."""
        if self.pac_connected:
            self.api_post("pac_disconnect", "/pac/disconnect", {})
            return

        baud_rate_str = self.ui.lineEdit.text().strip()
        port = self.ui.comboBox_2.currentText()

        if not port:
            QMessageBox.warning(self, "Connection Error", "Please select a COM port for PAC Power")
            return

        try:
            baud_rate = int(baud_rate_str) if baud_rate_str else 9600
        except ValueError:
            QMessageBox.warning(self, "Invalid Input", f"Invalid baud rate: {baud_rate_str}")
            return

        self.logger.info(f"Connecting to PAC Power on {port} @ {baud_rate}")
        self.api_post("pac_connect", "/pac/connect", {"port": port, "baud": baud_rate})

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
                self._save_settings()
                self.logger.info("Application closed while test was running")
                event.accept()
            else:
                event.ignore()
        else:
            self._save_settings()
            self.logger.info("Application closed normally")
            event.accept()

    # ----------------------------------------------------------------
    # Settings persistence (QSettings)
    # ----------------------------------------------------------------

    def _save_settings(self):
        """Save user preferences to persistent storage."""
        s = QSettings("LandisGyr", "SmartMeterGUI")

        # Window geometry
        s.setValue("window/geometry", self.saveGeometry())
        s.setValue("window/state", self.saveState())

        # DAQ
        s.setValue("daq/port", self.ui.cb_Port24970A.currentText())
        s.setValue("daq/baud", self.ui.cb_baudRate24970A.currentText())

        # Radian
        s.setValue("radian/port", self.ui.cB_PortRadian.currentText())

        # PAC Power
        s.setValue("pac/port", self.ui.comboBox_2.currentText())
        s.setValue("pac/baud", self.ui.lineEdit.text())

        # Cal Inst GPIB
        s.setValue("cal_inst/board_id", self.ui.spinBox_BoardID.value())
        s.setValue("cal_inst/primary_addr", self.ui.spinBox_PrimAddress.value())
        s.setValue("cal_inst/secondary_addr", self.ui.cb_SecAddress.currentText())

        # Test mode
        if self.ui.button_Free.isChecked():
            s.setValue("test/mode", "free")
        elif self.ui.button_Readings.isChecked():
            s.setValue("test/mode", "readings")
        else:
            s.setValue("test/mode", "duration")

        # Slot selection
        if self.ui.button_slot100.isChecked():
            s.setValue("test/slot", 100)
        elif self.ui.button_slot200.isChecked():
            s.setValue("test/slot", 200)
        else:
            s.setValue("test/slot", 300)

        # Sensor type
        s.setValue("test/sensor_rtd", self.ui.button_RTD.isChecked())

        # Voltage
        s.setValue("test/voltage", self.ui.lineEdit_Voltage.text())

        # Data filters
        s.setValue("filters/volts", self.ui.cb_VoltsData.isChecked())
        s.setValue("filters/current", self.ui.cb_CurrentData.isChecked())
        s.setValue("filters/frequency", self.ui.cb_DataFrequency.isChecked())
        s.setValue("filters/phase", self.ui.cb_PhaseData.isChecked())

        # Read interval
        s.setValue("test/read_interval_idx", self.ui.cB_readIntervals.currentIndex())

        self.logger.info("Settings saved")

    def _restore_settings(self):
        """Restore user preferences from persistent storage."""
        s = QSettings("LandisGyr", "SmartMeterGUI")

        # Window geometry
        geom = s.value("window/geometry")
        if geom:
            self.restoreGeometry(geom)
        state = s.value("window/state")
        if state:
            self.restoreState(state)

        # DAQ port/baud - select if available in combo
        self._select_combo_value(self.ui.cb_Port24970A, s.value("daq/port", ""))
        self._select_combo_value(self.ui.cb_baudRate24970A, s.value("daq/baud", "9600"))

        # Radian port
        self._select_combo_value(self.ui.cB_PortRadian, s.value("radian/port", ""))

        # PAC Power
        self._select_combo_value(self.ui.comboBox_2, s.value("pac/port", ""))
        pac_baud = s.value("pac/baud", "")
        if pac_baud:
            self.ui.lineEdit.setText(str(pac_baud))

        # Cal Inst GPIB
        board_id = s.value("cal_inst/board_id", 0, type=int)
        self.ui.spinBox_BoardID.setValue(board_id)
        prim_addr = s.value("cal_inst/primary_addr", 1, type=int)
        self.ui.spinBox_PrimAddress.setValue(prim_addr)
        self._select_combo_value(self.ui.cb_SecAddress, s.value("cal_inst/secondary_addr", ""))

        # Test mode
        mode = s.value("test/mode", "free")
        if mode == "readings":
            self.ui.button_Readings.setChecked(True)
        elif mode == "duration":
            self.ui.button_Duration.setChecked(True)
        else:
            self.ui.button_Free.setChecked(True)

        # Slot
        slot = s.value("test/slot", 100, type=int)
        if slot == 200:
            self.ui.button_slot200.setChecked(True)
        elif slot == 300:
            self.ui.button_slot300.setChecked(True)
        else:
            self.ui.button_slot100.setChecked(True)

        # Sensor type
        if s.value("test/sensor_rtd", False, type=bool):
            self.ui.button_RTD.setChecked(True)

        # Voltage
        voltage = s.value("test/voltage", "")
        if voltage:
            self.ui.lineEdit_Voltage.setText(str(voltage))

        # Data filters
        self.ui.cb_VoltsData.setChecked(s.value("filters/volts", False, type=bool))
        self.ui.cb_CurrentData.setChecked(s.value("filters/current", False, type=bool))
        self.ui.cb_DataFrequency.setChecked(s.value("filters/frequency", False, type=bool))
        self.ui.cb_PhaseData.setChecked(s.value("filters/phase", False, type=bool))

        # Read interval
        read_idx = s.value("test/read_interval_idx", 0, type=int)
        if 0 <= read_idx < self.ui.cB_readIntervals.count():
            self.ui.cB_readIntervals.setCurrentIndex(read_idx)

        self.logger.info("Settings restored")

    @staticmethod
    def _select_combo_value(combo, value: str):
        """Select a combo box item by text value, if it exists."""
        if not value:
            return
        idx = combo.findText(str(value))
        if idx >= 0:
            combo.setCurrentIndex(idx)
