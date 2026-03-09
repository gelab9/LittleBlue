"""Main application window with backend logic."""

import logging
from pathlib import Path
from datetime import datetime
from PyQt6.QtWidgets import (
    QMainWindow, QMessageBox, QFileDialog, QTableWidgetItem, QVBoxLayout,
    QTextBrowser, QHBoxLayout, QPushButton, QComboBox, QLabel, QSpinBox,
    QLineEdit, QSizePolicy, QGroupBox, QRadioButton, QCheckBox, QTextEdit,
    QGridLayout,
)
from PyQt6.QtCore import QTimer, QThread, pyqtSignal, QSettings, QObject, QTime
from PyQt6.QtGui import QColor, QTextCharFormat, QFont, QAction
from PyQt6 import QtCore

from src.ui.main_window import Ui_MainWindow
from src.ui.email_dialog import EmailDialog
from src.util.email_service import LdapLookup, EmailService
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

        # Hidden until DAQ is identified
        self.ui.gB_ChannelData.setVisible(False)
        # Hidden until Duration mode is selected
        self.ui.gB_Duration.setVisible(False)

        # Make all dropdowns wide enough to show their full option text
        for ch_id in range(101, 111):
            combo = getattr(self.ui, f"comboBox_{ch_id}", None)
            if combo:
                combo.setMinimumWidth(130)
                combo.setSizeAdjustPolicy(
                    QComboBox.SizeAdjustPolicy.AdjustToContents
                )

        _other_combos = [
            "cb_baudRate24970A", "cb_Port24970A", "cB_PortRadian",
            "cB_readIntervals", "comboBox_PowerPac", "cb_SecAddress",
            "comboBox_13",
        ]
        for name in _other_combos:
            combo = getattr(self.ui, name, None)
            if combo:
                combo.setSizeAdjustPolicy(
                    QComboBox.SizeAdjustPolicy.AdjustToContents
                )
                combo.setMinimumContentsLength(8)  # reserve space even when empty

        # Tighten up the DAQ tab layout
        self._tighten_layout()

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

        # Slot state tracking — active slot and per-slot widget state cache
        self._current_slot: int = 100  # 100, 200, or 300
        self._slot_channel_states: dict[int, dict] = {100: {}, 200: {}, 300: {}}

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
            (self.ui.comboBox_15, self.ui.comboBox_18, self.ui.lineEdit_8),
            (self.ui.comboBox_16, self.ui.comboBox_17, self.ui.lineEdit_9),
            (self.ui.comboBox_19, self.ui.comboBox_20, self.ui.lineEdit_12),
            (self.ui.comboBox_23, self.ui.comboBox_24, self.ui.lineEdit_11),
            (self.ui.comboBox_21, self.ui.comboBox_22, self.ui.lineEdit_10),
        ]

        # Plot widget (embedded in tab_2 "DAQ Plot")
        self.plot_widget = PlotWidget(logger=self.logger, parent=self.ui.tab_2)
        tab2_layout = QVBoxLayout(self.ui.tab_2)
        tab2_layout.setContentsMargins(4, 4, 4, 4)
        tab2_layout.addWidget(self.plot_widget)

        # Error Log tab (tab_7)
        self._setup_error_log_tab()

        # DAQ Log tab (tab_3)
        self._setup_daq_log_tab()

        # Radian tab (tab_5)
        self._setup_radian_tab()

        # PAC Power tab (tab_6)
        self._setup_pac_power_tab()

        # Current Controller tab (tab_4)
        self._setup_current_controller_tab()

        # Mode input widgets (Duration HH:MM:SS, Readings count)
        self._setup_mode_inputs()

        # Wire UI
        self.setup_connections()
        self.populate_defaults()
        self._restore_settings()

        # API in-flight counter — used to drive indeterminate progress bar animation
        self._api_calls_in_flight = 0

        # Clock timer: keep label_timeStamp current, fires every second
        self._clock_timer = QTimer()
        self._clock_timer.timeout.connect(self.update_timestamp)
        self._clock_timer.start(1000)

        # Poll timer
        self.poll_timer = QTimer()
        self.poll_timer.timeout.connect(self.on_poll)
        self.poll_interval_ms = self.config.get_poll_interval_ms()

        # Current Controller data logger state
        self._cc_logging_active = False
        self._cc_sample_count = 0
        self._cc_logged_data: list[str] = []
        self._cc_logger_timer = QTimer()
        self._cc_logger_timer.timeout.connect(self._cc_on_logger_tick)

        # Email notification state
        self._email_recipients_to: list[str] = []
        self._email_recipients_cc: list[str] = []
        self._email_notify_test_done = True
        self._email_notify_threshold = False
        self._email_operator = ""
        self._email_serial = ""
        self._email_simulated = False
        self._email_project = ""
        self._email_notes = ""
        self._last_warning_time = 0.0

        # LDAP lookup service
        ldap_server = self.config.get("email.ldap_server", "")
        ldap_base_dn = self.config.get("email.ldap_base_dn", "")
        ldap_port = self.config.get("email.ldap_port", 389)
        self._ldap_lookup: LdapLookup | None = None
        if ldap_server:
            self._ldap_lookup = LdapLookup(ldap_server, ldap_base_dn, ldap_port)

        # Email sending service
        smtp_server = self.config.get("email.smtp_server", "")
        smtp_port = self.config.get("email.smtp_port", 25)
        smtp_tls = self.config.get("email.smtp_use_tls", False)
        from_addr = self.config.get("email.from_address", "")
        self._email_service = EmailService(smtp_server, smtp_port, smtp_tls, from_addr)
        self._email_domain = self.config.get("email.domain", "@landisgyr.com")

        # Email menu action
        email_action = QAction("Email", self)
        email_action.triggered.connect(self._open_email_dialog)
        self.ui.menubar.addAction(email_action)

        self.setWindowTitle("Smart Meter GUI - Test & Development")
        self.logger.info("Application initialized successfully")

    # ----------------------------------------------------------------
    # Setup
    # ----------------------------------------------------------------

    def _tighten_layout(self):
        """Remove extra spacing and fix alignment on the DAQ tab grid."""
        grid = self.ui.gridLayout_17

        # --- Reduce overall grid spacing and margins ---
        grid.setContentsMargins(4, 4, 4, 4)
        grid.setHorizontalSpacing(4)
        grid.setVerticalSpacing(4)

        # --- Remove the two spacer items that create dead gaps ---
        # Walk the grid backwards so removing items doesn't shift indices
        for i in reversed(range(grid.count())):
            item = grid.itemAt(i)
            if item and item.spacerItem():
                grid.removeItem(item)

        # --- Column stretch: give the data area (cols 3-7) most of the room ---
        # Left panel cols (connection + channels)
        grid.setColumnStretch(0, 1)
        grid.setColumnStretch(1, 1)
        grid.setColumnStretch(2, 0)   # was spacer column, collapse it
        # Middle panel cols (voltage, controls, start, state, parameters)
        grid.setColumnStretch(3, 2)
        grid.setColumnStretch(4, 2)
        grid.setColumnStretch(5, 1)
        grid.setColumnStretch(6, 1)
        grid.setColumnStretch(7, 0)   # was spacer column, collapse it
        # Right panel col (timestamp, mode, duration, info, radian, compare)
        grid.setColumnStretch(8, 3)

        # --- Row stretch: let the data/channel rows expand, keep header rows compact ---
        # Top rows (connection boxes, controls, voltage, slot radios)
        for r in range(5):
            grid.setRowStretch(r, 0)
        # Progress bar row
        grid.setRowStretch(5, 0)
        # Data table + channel area rows — these should absorb vertical space
        for r in range(6, 12):
            grid.setRowStretch(r, 1)

        # --- Constrain small group boxes so they don't balloon vertically ---
        preferred_v = QSizePolicy.Policy.Preferred
        compact_boxes = [
            self.ui.gb_connectionOne,
            self.ui.gB_Voltage,
            self.ui.gB_controlSettings,
            self.ui.gB_Parameters,
            self.ui.gB_Mode,
            self.ui.gB_Duration,
            self.ui.gB_PACPower,
            self.ui.gB_Radian,
            self.ui.gB_Informtation,
        ]
        for box in compact_boxes:
            sp = box.sizePolicy()
            sp.setVerticalPolicy(preferred_v)
            box.setSizePolicy(sp)

        # State browser doesn't need to be tall
        self.ui.textBrowser_State.setMaximumHeight(80)

        # Tighten sub-layouts inside group boxes
        for layout_name in [
            "gridLayout_18", "gridLayout_19", "gridLayout_20",
            "gridLayout_21", "gridLayout_25", "gridLayout_26",
            "gridLayout_27", "gridLayout_22", "gridLayout_28",
            "gridLayout_29", "gridLayout_30", "gridLayout_31",
        ]:
            sub = getattr(self.ui, layout_name, None)
            if sub:
                sub.setContentsMargins(4, 2, 4, 2)
                sub.setHorizontalSpacing(4)
                sub.setVerticalSpacing(2)

        # Also tighten the scroll-area inner layout for the Data group
        self.ui.gridLayout_24.setContentsMargins(2, 2, 2, 2)
        self.ui.gridLayout_24.setHorizontalSpacing(4)
        self.ui.gridLayout_24.setVerticalSpacing(2)

        # Outer layouts
        self.ui.gridLayout_15.setContentsMargins(2, 2, 2, 2)
        self.ui.gridLayout.setContentsMargins(2, 2, 2, 2)

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

        # Close loop control
        self.ui.cB_closeLoop.stateChanged.connect(self._on_close_loop_toggled)

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
        self.ui.comboBox_PowerPac.clear()
        self.ui.comboBox_PowerPac.addItems(ports)
        self.logger.info(f"Detected serial ports: {ports}")

    def populate_defaults(self):
        """Populate default values in UI elements."""
        self.ui.cB_readIntervals.setCurrentIndex(0)
        self.ui.button_Free.setChecked(True)
        self.ui.button_slot100.setChecked(True)

        baud_rates = ["9600", "19200", "38400", "57600", "115200"]
        self.ui.cb_baudRate24970A.clear()
        self.ui.cb_baudRate24970A.addItems(baud_rates)

        # Start progress bar in indeterminate/animated mode (overrides any .ui default value)
        self.ui.progressBar.setRange(0, 0)

        # Default deadband values for closed-loop current control
        self.ui.lineEdit_Controller.setText("10")
        self.ui.lineEdit_Accuracy.setText(".1")

        self.update_timestamp()
        self.logger.info("UI defaults populated")
        self.refresh_serial_ports()

    def update_timestamp(self):
        current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S %Z%z")
        self.ui.label_timeStamp.setText(current_time)

    # ----------------------------------------------------------------
    # Mode input widgets
    # ----------------------------------------------------------------

    def _setup_mode_inputs(self):
        """Add Readings (count) spin box to gB_Mode; Duration uses gB_Duration from UI."""
        # Readings: spin box for target count (no dedicated UI widget yet)
        self._readings_spinbox = QSpinBox(self.ui.gB_Mode)
        self._readings_spinbox.setRange(1, 999999)
        self._readings_spinbox.setValue(100)
        self._readings_spinbox.setVisible(False)

        mode_layout = self.ui.gB_Mode.layout()
        if mode_layout is not None:
            mode_layout.addWidget(self._readings_spinbox)
        else:
            self._readings_spinbox.move(10, 90)

        # Show the right widget for the current mode
        self._update_mode_input_visibility()

    def _update_mode_input_visibility(self):
        self.ui.gB_Duration.setVisible(self.ui.button_Duration.isChecked())
        self._readings_spinbox.setVisible(self.ui.button_Readings.isChecked())

    # ----------------------------------------------------------------
    # UI lock / unlock during test
    # ----------------------------------------------------------------

    def _lock_ui_for_test(self):
        """Disable controls that must not change while a test is running."""
        self.ui.cb_baudRate24970A.setEnabled(False)
        self.ui.button_RTD.setEnabled(False)
        self.ui.button_Thermocouple.setEnabled(False)
        self.ui.button_Free.setEnabled(False)
        self.ui.button_Readings.setEnabled(False)
        self.ui.button_Duration.setEnabled(False)
        self.ui.cb_VoltsData.setEnabled(False)
        self.ui.cb_CurrentData.setEnabled(False)
        self.ui.cb_DataFrequency.setEnabled(False)
        self.ui.cb_PhaseData.setEnabled(False)
        self.ui.cB_readIntervals.setEnabled(False)
        self.ui.spinBox_BoardID.setEnabled(False)
        self.ui.spinBox_PrimAddress.setEnabled(False)
        self.ui.cb_SecAddress.setEnabled(False)
        self.ui.pb_OpenCalInstConx.setEnabled(False)
        self.ui.pb_CloseCalInstConx.setEnabled(False)
        # Current Controller — disable logger start during DAQ test
        self._cc_toggle_logger_btn.setEnabled(False)

    def _unlock_ui_after_test(self):
        """Re-enable controls after a test stops."""
        self.ui.cb_baudRate24970A.setEnabled(True)
        self.ui.button_RTD.setEnabled(True)
        self.ui.button_Thermocouple.setEnabled(True)
        self.ui.button_Free.setEnabled(True)
        self.ui.button_Readings.setEnabled(True)
        self.ui.button_Duration.setEnabled(True)
        self.ui.cb_VoltsData.setEnabled(True)
        self.ui.cb_CurrentData.setEnabled(True)
        self.ui.cb_DataFrequency.setEnabled(True)
        self.ui.cb_PhaseData.setEnabled(True)
        self.ui.cB_readIntervals.setEnabled(True)
        # Cal Inst inputs — only unlock if not currently connected
        if not self.cal_inst_connected:
            self.ui.spinBox_BoardID.setEnabled(True)
            self.ui.spinBox_PrimAddress.setEnabled(True)
            self.ui.cb_SecAddress.setEnabled(True)
            self.ui.pb_OpenCalInstConx.setEnabled(True)
        self.ui.pb_CloseCalInstConx.setEnabled(self.cal_inst_connected)
        # Current Controller — re-enable logger start (only if not actively logging)
        if not self._cc_logging_active:
            self._cc_toggle_logger_btn.setEnabled(True)

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

    def api_post(self, action: str, path: str, json_payload: dict, timeout_s: float = None):
        payload = {"path": path, "json": json_payload}
        if timeout_s is not None:
            payload["timeout_s"] = timeout_s
        QtCore.QMetaObject.invokeMethod(
            self.api_worker,
            "do_post",
            QtCore.Qt.ConnectionType.QueuedConnection,
            QtCore.Q_ARG(str, action),
            QtCore.Q_ARG(dict, payload),
        )

    def on_api_progress(self, msg: str):
        self.ui.textBrowser_State.setText(msg)
        self._api_calls_in_flight += 1
        # Show busy animation unless a deterministic test is already tracking progress
        is_deterministic = (
            self.test_controller.is_running
            and self.test_controller.mode in (
                TestController.MODE_READINGS, TestController.MODE_DURATION
            )
        )
        if not is_deterministic:
            self.ui.progressBar.setRange(0, 0)

    def on_api_finished(self, action: str, res: ApiResult):
        # Decrement counter and restore bar to idle whenever no calls remain outside a test
        self._api_calls_in_flight = max(0, self._api_calls_in_flight - 1)
        if self._api_calls_in_flight == 0 and not self.test_controller.is_running:
            self.ui.progressBar.setRange(0, 0)  # back to animated idle

        if not res.ok:
            self.logger.error(f"{action} failed: {res.status} {res.error}")
            # Polling actions can fail transiently (instrument busy, partial read, etc.).
            # Log only — no popup — so the test can keep running uninterrupted.
            _TRANSIENT_ACTIONS = {"daq_read", "radian_instant_metrics", "daq_err",
                                  "pac_beep", "pac_init_voltage", "pac_zero_voltage",
                                  "daq_idn", "health", "daq_status", "daq_abort",
                                  "pac_poll_metrics", "cc_instant_metrics"}
            if action not in _TRANSIENT_ACTIONS:
                QMessageBox.critical(self, "API Error", f"{action} failed:\n{res.error}")
            return

        data = res.data or {}

        if action == "daq_connect":
            self.daq_connected = True
            self.ui.pb_Connect34970A.setText("Disconnect")
            self.ui.textBrowser_State.setText("CONNECTED")
            self.ui.gB_ChannelData.setVisible(True)  # show channel UI immediately on connect
            # Fire follow-ups only after a confirmed successful connect
            self.api_get("health", "/health")
            self.api_get("daq_status", "/daq/status")
            self.api_get("daq_idn", "/daq/idn")

        elif action == "daq_disconnect":
            self.daq_connected = False
            self.ui.pb_Connect34970A.setText("Connect")
            self.ui.textBrowser_State.setText("DISCONNECTED")

        elif action == "daq_status":
            self.ui.textBrowser_State.setText(str(data))

        elif action == "daq_idn":
            idn = data.get("idn", str(data))  # backend returns {"idn": "..."}
            self.ui.textBrowser_State.setText(idn)
            self.ui.textBrowser_lowerData.append(f"DAQ IDN: {idn}")

        elif action == "daq_latest":
            self.logger.debug(f"RAW LATEST: {data}")
            self.apply_latest_reading(data)

        elif action == "daq_setup":
            err = data.get("error", "")
            if err and not err.startswith('+0,"No error"'):
                self.ui.textBrowser_lowerData.append(f"DAQ setup error: {err}")
            self.ui.textBrowser_State.setText("DAQ setup complete")

        elif action == "daq_run":
            self.ui.textBrowser_State.setText("RUNNING")

        elif action == "daq_stop":
            self.ui.textBrowser_State.setText("STOPPED")

        elif action == "daq_abort":
            self.logger.debug("DAQ ABOR sent — instrument returned to idle")

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
            # Serial port opened — verify the device actually responds before
            # marking as connected.  Do NOT set radian_connected here yet.
            port = data.get("port", "")
            self.ui.pb_ConnectRadian.setText("Disconnect")
            self.ui.textBrowser_State.setText(f"Radian port open ({port}) — verifying device...")
            self.api_get("radian_identify", "/radian/identify")

        elif action == "radian_disconnect":
            self.radian_connected = False
            self.ui.pb_ConnectRadian.setText("Connect")
            self.ui.textBrowser_State.setText("RADIAN DISCONNECTED")

        elif action == "radian_identify":
            response = data.get("response", "")
            if response:
                # Device replied — confirmed truly connected
                self.radian_connected = True
                short = response[:20] + ("..." if len(response) > 20 else "")
                self.ui.textBrowser_State.setText(f"RADIAN CONNECTED — ID: {short}")
                self.ui.textBrowser_lowerData.append(f"Radian ID: {response}")
            else:
                # Port opened but device never answered
                self.radian_connected = False
                self.ui.pb_ConnectRadian.setText("Connect")
                self.ui.textBrowser_State.setText("Radian NOT responding — check device and cable")
                self.ui.textBrowser_lowerData.append(
                    "WARNING: Radian serial port opened but device did not respond."
                )
                QMessageBox.warning(
                    self,
                    "Radian Not Responding",
                    "The serial port opened but the Radian device did not respond.\n\n"
                    "Please check:\n"
                    "  • Radian is powered on\n"
                    "  • Cable is firmly connected\n"
                    "  • Correct COM port and baud rate are selected",
                )
                # Clean up the open port on the backend
                self.api_post("radian_disconnect", "/radian/disconnect", {})

        elif action == "radian_instant_metrics":
            hex_resp = data.get("response", "")
            if hex_resp:
                # Show raw response in lower data pane for debugging (temporary)
                try:
                    self.ui.textBrowser_lowerData.append(f"Radian RAW: {hex_resp}")
                except Exception:
                    pass
                try:
                    raw_bytes = bytes.fromhex(hex_resp)
                    metrics = self.radian_device.parse_instant_metrics(raw_bytes)
                    if metrics:
                        self._last_radian_metrics = metrics
                        self._append_daq_log_entry(metrics)
                        self._execute_close_loop()
                    else:
                        # Parsing returned None — log for debugging
                        self.ui.textBrowser_lowerData.append("Radian: failed to parse instant metrics (insufficient/invalid data)")
                except (ValueError, Exception) as e:
                    self.logger.warning(f"Failed to parse Radian metrics: {e}")
                    try:
                        self.ui.textBrowser_lowerData.append(f"Radian parse error: {e}")
                    except Exception:
                        pass

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
            # Matching original: beep to confirm comms then zero the voltage setpoint.
            # Do NOT send output-off — the original leaves output state as-is on connect.
            self.api_post("pac_beep", "/pac/send", {"cmd": ":SYST:BEEP"})
            self.pac_power_device.voltage_setpoint = 0.0
            self.api_post("pac_init_voltage", "/pac/send", {"cmd": ":VOLT1 0"})

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
            try:
                self.pac_power_device.voltage_setpoint = float(data.get("voltage", 0))
            except (ValueError, TypeError):
                pass
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
            # Also display in PAC Power tab log
            m = self.pac_power_device.metrics
            self._pac_log_browser.append(
                f"V={m.volt_ln[0]:.2f}V  I={m.current_rms[0]:.3f}A  "
                f"F={m.frequency:.2f}Hz  P={m.power[0]:.2f}W  PF={m.pf[0]:.3f}"
            )

        elif action == "pac_poll_metrics":
            # Silent poll used during tests when Radian is absent — just refresh metrics
            self.pac_power_device.update_voltage_from_api(data)
            self.pac_power_device.update_current_from_api(data)
            self.pac_power_device.update_frequency_from_api(data)
            self.pac_power_device.update_power_from_api(data)

        elif action == "pac_tab_get_voltage":
            v1 = data.get("volt1", "?")
            v2 = data.get("volt2", "?")
            v3 = data.get("volt3", "?")
            self._pac_log_browser.append(f"Voltage: V1={v1}  V2={v2}  V3={v3}")

        elif action == "pac_tab_get_frequency":
            freq = data.get("frequency", "?")
            self._pac_log_browser.append(f"Frequency: {freq} Hz")

        elif action == "pac_tab_set_frequency":
            self._pac_log_browser.append(f"Frequency set: {data.get('sent', '?')}")

        elif action == "radian_tab_instant":
            hex_resp = data.get("response", "")
            if hex_resp:
                try:
                    # Also show raw hex when manually requesting from Radian tab
                    try:
                        self.ui.textBrowser_lowerData.append(f"Radian RAW (tab): {hex_resp}")
                    except Exception:
                        pass
                    raw_bytes = bytes.fromhex(hex_resp)
                    metrics = self.radian_device.parse_instant_metrics(raw_bytes)
                    if metrics:
                        self._last_radian_metrics = metrics
                        self._display_radian_instant_metrics(metrics)
                    else:
                        self.ui.textBrowser_lowerData.append("Radian: failed to parse instant metrics (tab request)")
                except (ValueError, Exception) as e:
                    self.logger.warning(f"Failed to parse Radian metrics: {e}")

        # Current Controller tab handlers
        elif action == "cc_instant_metrics":
            hex_resp = data.get("response", "")
            if hex_resp:
                try:
                    raw_bytes = bytes.fromhex(hex_resp)
                    metrics = self.radian_device.parse_instant_metrics(raw_bytes)
                    if metrics:
                        line = self._cc_format_metrics_line(metrics)
                        self._cc_logged_data.append(line)
                        self._cc_data_log_browser.append(line)
                        self._cc_append_gpib_log(f"Sample {self._cc_sample_count}: OK")
                    else:
                        self._cc_append_gpib_log("Failed to parse metrics (insufficient data)")
                except (ValueError, Exception) as e:
                    self.logger.warning(f"CC logger parse error: {e}")
                    self._cc_append_gpib_log(f"Parse error: {e}")

        elif action == "cc_tab_instant":
            hex_resp = data.get("response", "")
            if hex_resp:
                try:
                    raw_bytes = bytes.fromhex(hex_resp)
                    metrics = self.radian_device.parse_instant_metrics(raw_bytes)
                    if metrics:
                        self._cc_append_gpib_log(
                            f"V={metrics.volt:.3f}V  I={metrics.amp:.3f}A  "
                            f"W={metrics.watt:.2f}W  F={metrics.frequency:.2f}Hz  "
                            f"Phase={metrics.phase:.2f}deg  PF={metrics.power_factor:.3f}"
                        )
                    else:
                        self._cc_append_gpib_log("Failed to parse instant metrics")
                except (ValueError, Exception) as e:
                    self.logger.warning(f"CC manual metrics parse error: {e}")
                    self._cc_append_gpib_log(f"Parse error: {e}")

        # NOTE: Do NOT auto-query SYST:ERR? after daq_read — the READ?
        # response for many channels takes time to transmit and a follow-up
        # SYST:ERR? will collide with it, causing -410 "Query INTERRUPTED"
        # and truncated data.  /daq/setup already checks errors internally.

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
        elif self.ui.button_slot300.isChecked():
            return list(range(301, 321))

    def _read_channel_config_from_ui(self) -> list[int]:
        """Read enabled channels from gB_ChannelData and update channel_configs.

        Widget names are always cB_101..cB_110, but the actual channel numbers
        depend on the active slot (100→101-110, 200→201-210, 300→301-310).
        """
        enabled_channels = []
        base = self._current_slot  # 100, 200, or 300

        for idx in range(10):
            widget_ch = 101 + idx       # widget name suffix (always 101-110)
            actual_ch = base + 1 + idx  # real channel number for this slot

            checkbox = getattr(self.ui, f"cB_{widget_ch}", None)
            if checkbox and checkbox.isChecked():
                enabled_channels.append(actual_ch)

                combo = getattr(self.ui, f"comboBox_{widget_ch}", None)
                channel_type = combo.currentText() if combo else ""

                gain_edit   = getattr(self.ui, f"gain_{widget_ch}", None)
                offset_edit = getattr(self.ui, f"offset_{widget_ch}", None)

                try:
                    gain = float(gain_edit.text()) if (gain_edit and gain_edit.text()) else 1.0
                except ValueError:
                    gain = 1.0

                try:
                    offset = float(offset_edit.text()) if (offset_edit and offset_edit.text()) else 0.0
                except ValueError:
                    offset = 0.0

                cfg = self._get_channel_config(actual_ch)
                if cfg:
                    cfg.channel_name = channel_type if channel_type else f"CH{actual_ch}"
                    cfg.gain = gain
                    cfg.offset = offset

        return enabled_channels

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
        if temp_rise is None:
            table.item(row, self.COL_TEMP_RISE).setText("N/A")
        else:
            table.item(row, self.COL_TEMP_RISE).setText(f"{temp_rise:.3f}")

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

    def on_set_voltage(self):
        voltage_str = self.ui.lineEdit_Voltage.text()
        try:
            voltage = float(voltage_str)
            if voltage < 0 or voltage > 300:
                raise ValueError("Voltage must be between 0 and 300V")
            self.logger.info(f"Setting voltage to {voltage}V")
            if self.cal_inst_connected:
                self.cal_inst_device.voltage_setpoint = voltage  # update immediately, don't wait for API response
                self.api_post("cal_inst_set_voltage", "/cal-inst/set-voltage", {"voltage": voltage})
            elif self.pac_connected:
                self.pac_power_device.voltage_setpoint = voltage  # update immediately, don't wait for API response
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
            # Matching original: zero voltage then disable output
            self.pac_power_device.voltage_setpoint = 0.0
            self.api_post("pac_zero_voltage", "/pac/send", {"cmd": ":VOLT1 0"})
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
            self.ui.textBrowser_lowerData.append(
                f"Log Terminated at:\t\t{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}"
            )
            self.ui.progressBar.setRange(0, 0)  # back to animated idle
            self._unlock_ui_after_test()
            # Reset DAQ to idle state (abort any ongoing scan)
            if self.daq_connected:
                self.daq_write("daq_abort", "ABOR")
            return

        # Use channel config panel if visible (after DAQ identified), else slot-based
        if self.ui.gB_ChannelData.isVisible():
            channels = self._read_channel_config_from_ui()
            if not channels:
                QMessageBox.warning(
                    self, "No Channels Selected",
                    "Please check at least one channel in the Channel Data panel"
                )
                return
        else:
            channels = self.get_selected_daq_channels()

        scan_list = ",".join(str(ch) for ch in channels)
        self._active_channels = channels

        # Reset statistics for new test
        self.statistics.reset()

        # Detect which channel (if any) is marked as "Ambient" — used for temp rise
        self.statistics.ambient_channel = None
        base = self._current_slot
        for idx in range(10):
            widget_ch = 101 + idx
            actual_ch = base + 1 + idx
            if actual_ch not in channels:
                continue
            combo = getattr(self.ui, f"comboBox_{widget_ch}", None)
            if combo and combo.currentText() == "Ambient":
                self.statistics.ambient_channel = actual_ch
                self.logger.info(f"Ambient channel set to {actual_ch}")
                break

        # Setup data table
        self.setup_data_table(channels)

        # Configure test controller mode
        if self.ui.button_Free.isChecked():
            self.test_controller.mode = TestController.MODE_FREE
        elif self.ui.button_Readings.isChecked():
            self.test_controller.mode = TestController.MODE_READINGS
            self.test_controller.target_readings = self._readings_spinbox.value()
        else:
            self.test_controller.mode = TestController.MODE_DURATION
            try:
                h = int(self.ui.lineEdit_Hours.text() or "0")
                m = int(self.ui.lineEdit_Minutes.text() or "0")
                sec = int(self.ui.lineEdit_Seconds.text() or "0")
            except ValueError:
                h, m, sec = 1, 0, 0
            self.test_controller.target_duration_s = h * 3600 + m * 60 + sec

        # Initialize CSV logger
        self.csv_logger = CsvLogger(self.data_dir)
        self.csv_logger.initialize(channels)
        self.logger.info(f"CSV logging to {self.csv_logger.filepath}")

        # Read the selected reading interval (seconds)
        interval_seconds = int(self.ui.cB_readIntervals.currentText() or "5")
        self.poll_interval_ms = interval_seconds * 1000

        # Configure DAQ via single sequenced setup call
        use_rtd = self.ui.button_RTD.isChecked()
        # daq_setup issues many SCPI commands with sleep delays; use a generous timeout
        self.api_post("daq_setup", "/daq/setup", {
            "scanList": scan_list,
            "useRtd": use_rtd,
            "tcType": "K",
            "triggerTimerSeconds": interval_seconds,
        }, timeout_s=30.0)

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
        # Free mode has no defined end — animate as indeterminate; others show deterministic %
        if self.test_controller.mode == TestController.MODE_FREE:
            self.ui.progressBar.setRange(0, 0)
        else:
            self.ui.progressBar.setRange(0, 100)
            self.ui.progressBar.setValue(0)
        self.ui.pB_Start.setText("Stop Test")
        self.ui.pB_Start.setStyleSheet("background-color: #ff6b6b;")
        self.ui.textBrowser_State.setText("RUNNING")
        self.ui.label_startTime.setText(
            f"Start Time: {self.test_start_time.strftime('%H:%M:%S')}"
        )
        self._lock_ui_for_test()
        self._init_data_log_display()
        self.poll_timer.start(self.poll_interval_ms)

    def on_poll(self):
        """Timer callback: request next reading from DAQ."""
        # Check if test should auto-stop
        if self.test_controller.should_stop():
            self.on_test_complete()
            return
        self.daq_query("daq_read", "READ?")
        # Fetch power-meter metrics for the lower data log display
        if self.radian_connected:
            # Radian: full metrics for close-loop control + display
            self.api_post("radian_instant_metrics", "/radian/command", {
                "hexCommand": "A60D0008002400000014FFFD",
                "timeoutMs": 2000,
            })
        elif self.pac_connected:
            # PAC fallback: silent measurement for lower browser display only
            self.api_get("pac_poll_metrics", "/pac/measure/all")

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

        # Capture log path before closing the logger
        completed_log_path = ""
        if self.csv_logger:
            if self.csv_logger.filepath:
                completed_log_path = str(self.csv_logger.filepath)
            self.csv_logger.close()
            self.csv_logger = None

        # Reset DAQ to idle state (abort any ongoing scan)
        if self.daq_connected:
            self.daq_write("daq_abort", "ABOR")

        self.plot_widget.stop_test()
        self.ui.progressBar.setRange(0, 0)  # back to animated idle
        self.ui.pB_Start.setText("Start Reading!")
        self.ui.pB_Start.setStyleSheet("")
        self.ui.textBrowser_State.setText("TEST COMPLETE")
        self._update_test_info()
        self.ui.textBrowser_lowerData.append(
            f"Log Terminated at:\t\t{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}"
        )
        self._unlock_ui_after_test()

        # Turn off power source if requested and a source is actually connected
        if self.ui.cB_sourceOff.isChecked() and (self.cal_inst_connected or self.pac_connected):
            self.logger.info("Turning off power source after test completion")
            self.on_voltage_off()

        # Send test-complete email notification
        self._send_test_complete_email(log_path=completed_log_path)

        QMessageBox.information(
            self,
            "Test Complete",
            f"Test completed!\n\n"
            f"Readings: {self.test_controller.reading_count}\n"
            f"Duration: {duration:.2f}s",
        )

    def _update_progress_bar(self):
        """Update the progress bar based on the current test mode."""
        self.ui.progressBar.setRange(0, 100)  # ensure normal range for deterministic modes
        if self.test_controller.mode == TestController.MODE_READINGS:
            target = self.test_controller.target_readings
            if target > 0:
                pct = min(100, int(self.test_controller.reading_count / target * 100))
                self.ui.progressBar.setValue(pct)
        elif self.test_controller.mode == TestController.MODE_DURATION:
            target = self.test_controller.target_duration_s
            if target > 0:
                elapsed = self.test_controller.elapsed_seconds()
                pct = min(100, int(elapsed / target * 100))
                self.ui.progressBar.setValue(pct)

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

    def _init_data_log_display(self):
        """Clear textBrowser_lowerData and write the column header row."""
        self.ui.textBrowser_lowerData.clear()
        header = "Sample"
        if self.ui.cb_VoltsData.isChecked():
            header += "\tVrms"        # single tab before Volts (matches original)
        if self.ui.cb_CurrentData.isChecked():
            header += "\t\tArms"
        if self.ui.cb_DataFrequency.isChecked():
            header += "\t\tHz"
        if self.ui.cb_PhaseData.isChecked():
            header += "\t\tDeg"
        header += "\t\tTime Stamp"
        self.ui.textBrowser_lowerData.append(f"<b>{header}</b>")

    def _append_data_log_row(self):
        """Append one sample row to textBrowser_lowerData (mirrors original lbDataLog).

        Data source priority:
          1. Radian instant metrics (when Radian is connected)
          2. PAC Power measurements (when Radian absent but PAC is connected)
          3. "--" placeholder (no instrument available)
        """
        def fmt(val):
            """Format like original VB: magnitude-based decimal places."""
            av = abs(val)
            if av < 10:
                return f"{val:8.5f}"
            elif av < 100:
                return f"{val:8.4f}"
            return f"{val:8.3f}"

        # Resolve display values from best available source
        radian = self._last_radian_metrics
        if radian is not None:
            volt_val = radian.volt
            amp_val  = radian.amp
            freq_val = radian.frequency
            phase_val = radian.phase
        elif self.pac_connected:
            pm = self.pac_power_device.metrics
            volt_val  = pm.volt_ln[0]       if pm.volt_ln       else None
            amp_val   = pm.current_rms[0]   if pm.current_rms   else None
            freq_val  = pm.frequency        if pm.frequency > 0 else None
            phase_val = None  # PAC does not provide phase angle
        else:
            volt_val = amp_val = freq_val = phase_val = None

        row = str(self.test_controller.reading_count)
        if self.ui.cb_VoltsData.isChecked():
            row += "\t" + (fmt(volt_val) if volt_val is not None else "--")
        if self.ui.cb_CurrentData.isChecked():
            row += "\t\t" + (fmt(amp_val) if amp_val is not None else "--")
        if self.ui.cb_DataFrequency.isChecked():
            row += "\t\t" + (fmt(freq_val) if freq_val is not None else "--")
        if self.ui.cb_PhaseData.isChecked():
            row += "\t\t" + (fmt(phase_val) if phase_val is not None else "--")
        row += "\t\t" + datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        self.ui.textBrowser_lowerData.append(row)
        sb = self.ui.textBrowser_lowerData.verticalScrollBar()
        sb.setValue(sb.maximum())

    def apply_latest_reading(self, data: dict):
        """Handle structured reading data (from /daq/latest)."""
        if not data:
            return
        channels = data.get("channels", data)
        for ch_str, value in channels.items():
            ch_id = int(ch_str)
            cfg = self._get_channel_config(ch_id)
            raw = float(value)
            calibrated = cfg.apply_calibration(raw) if (cfg and self.ui.cB_Calibration.isChecked()) else raw
            self.statistics.update_channel(ch_id, calibrated)
            self.update_table_row(ch_id)

    def apply_daq_response(self, resp: str):
        """Parse raw READ? response and update statistics + table."""
        resp = (resp or "").strip()
        if not resp:
            return

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
                calibrated = cfg.apply_calibration(raw_value) if (cfg and self.ui.cB_Calibration.isChecked()) else raw_value

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
            self._update_progress_bar()
            self._append_data_log_row()

            # Log to CSV — use Radian if connected, otherwise PAC fallback
            if self.csv_logger:
                rad = self._last_radian_metrics
                if rad is not None:
                    csv_vrms = rad.volt
                    csv_arms = rad.amp
                    csv_hz   = rad.frequency
                    csv_deg  = rad.phase
                elif self.pac_connected:
                    pm = self.pac_power_device.metrics
                    csv_vrms = pm.volt_ln[0]      if pm.volt_ln      else None
                    csv_arms = pm.current_rms[0]  if pm.current_rms  else None
                    csv_hz   = pm.frequency       if pm.frequency > 0 else None
                    csv_deg  = None
                else:
                    csv_vrms = csv_arms = csv_hz = csv_deg = None
                self.csv_logger.append_row(
                    timestamp_str, channel_values,
                    radian_vrms=csv_vrms, radian_arms=csv_arms,
                    radian_hz=csv_hz,    radian_deg=csv_deg,
                )

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

        # Log threshold warnings and send email
        if exceeded:
            warning_lines = []
            for idx in exceeded:
                pair = self.comparison.pairs[idx]
                msg = (
                    f"WARNING: Threshold exceeded on pair {idx + 1}: "
                    f"CH{pair.channel_a} vs CH{pair.channel_b}, "
                    f"diff={pair.difference:.3f}\u00b0C >= {pair.threshold}\u00b0C"
                )
                self.ui.textBrowser_lowerData.append(msg)
                warning_lines.append(
                    f"CH{pair.channel_a}: {pair.value_a:.3f}\u00b0C\n"
                    f"CH{pair.channel_b}: {pair.value_b:.3f}\u00b0C\n"
                    f"Temperature Difference: {pair.difference:.3f}\u00b0C\n"
                    f"Threshold: {pair.threshold}\u00b0C"
                )
            if warning_lines:
                self._send_threshold_warning_email("\n\n".join(warning_lines))

    # ----------------------------------------------------------------
    # Close current loop (proportional voltage control)
    # ----------------------------------------------------------------

    def _on_close_loop_toggled(self, _):
        """Enable/disable manual voltage controls based on close loop checkbox state.
        Mirrors the original chkCloseLoop_CheckedChanged behavior."""
        loop_on = self.ui.cB_closeLoop.isChecked()
        # Disable manual voltage input when close loop is active (matching original VB behavior)
        self.ui.lineEdit_Voltage.setEnabled(not loop_on)
        self.ui.pb_setVoltage.setEnabled(not loop_on)
        self.ui.pb_voltageOff.setEnabled(not loop_on)
        # Keep deadband/setpoint controls always editable
        self.ui.lineEdit_Controller.setEnabled(True)
        self.ui.lineEdit_Accuracy.setEnabled(True)
        self.ui.comboBox_13.setEnabled(True)

    def _execute_close_loop(self):
        """Proportional control: adjust voltage to maintain target current."""
        if not self.ui.cB_closeLoop.isChecked():
            return
        if not self.radian_connected:
            return

        metrics = self._last_radian_metrics
        if metrics is None:
            return

        measured_current = metrics.amp
        if measured_current <= 0:
            return

        try:
            target_current = float(self.ui.comboBox_13.currentText() or "0")
        except ValueError:
            return
        if target_current <= 0:
            return

        try:
            controller_db_pct = float(self.ui.lineEdit_Controller.text() or "5")
            accuracy_db_pct = float(self.ui.lineEdit_Accuracy.text() or "1")
        except ValueError:
            return

        control_deadband = controller_db_pct / 100.0 * target_current
        accuracy_deadband = accuracy_db_pct / 100.0 * target_current
        difference = measured_current - target_current

        if self.cal_inst_connected:
            old_voltage = self.cal_inst_device.voltage_setpoint
        elif self.pac_connected:
            old_voltage = self.pac_power_device.voltage_setpoint
        else:
            return

        if old_voltage <= 0:
            return

        new_voltage = None

        if abs(difference) < control_deadband:
            # Within control band: fine-tune only if outside the accuracy deadband.
            # The formula is naturally bidirectional:
            #   measured > target  →  ratio < 1  →  voltage decreases  →  current drops
            #   measured < target  →  ratio > 1  →  voltage increases  →  current rises
            # Outside the control band we do NOT adjust — large corrections would cause
            # runaway voltage swings (e.g. 200 V → 10 V in a single step). The operator
            # must manually bring the current near the setpoint first.
            if abs(difference) > accuracy_deadband / 1.7:
                new_voltage = round(old_voltage * target_current / measured_current, 1)

        if new_voltage is not None:
            self.logger.info(
                f"Close Loop: Measured={measured_current:.3f}A, "
                f"Target={target_current}A, OldV={old_voltage}V, NewV={new_voltage}V"
            )

            if self.cal_inst_connected:
                self.cal_inst_device.voltage_setpoint = new_voltage  # update immediately so next iteration uses new value
                self.api_post("cal_inst_set_voltage", "/cal-inst/set-voltage", {"voltage": new_voltage})
            elif self.pac_connected:
                self.pac_power_device.voltage_setpoint = new_voltage  # update immediately so next iteration uses new value
                self.api_post("pac_set_voltage", "/pac/set-voltage", {"voltage": new_voltage})

    # ----------------------------------------------------------------
    # Radian tab handlers
    # ----------------------------------------------------------------

    def on_connect_radian(self):
        """Connect or disconnect to Radian power analyzer."""
        if self.ui.pb_ConnectRadian.text() == "Disconnect":
            self.api_post("radian_disconnect", "/radian/disconnect", {})
            return

        port = self.ui.cB_PortRadian.currentText()
        baud_text = self.ui.lineEdit_baudRateRadian.text().strip()
        baud_rate = int(baud_text) if baud_text else 9600

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

        baud_rate_str = self.ui.lineEdit_PACBaud.text().strip()
        port = self.ui.comboBox_PowerPac.currentText()

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
        self._update_mode_input_visibility()
        self.logger.debug(f"Test mode changed to: {mode}")

    def on_slot_changed(self):
        if self.ui.button_slot100.isChecked():
            new_slot = 100
        elif self.ui.button_slot200.isChecked():
            new_slot = 200
        elif self.ui.button_slot300.isChecked():
            new_slot = 300
        else:
            return
        if new_slot == self._current_slot:
            return
        self._save_slot_state(self._current_slot)
        self._current_slot = new_slot
        self._update_channel_labels(new_slot)
        self._restore_slot_state(new_slot)
        self.logger.debug(f"Slot changed to: Slot {new_slot}")

    def _update_channel_labels(self, slot: int):
        """Relabel gB_ChannelData checkboxes to show the correct channel numbers for slot."""
        for idx in range(10):
            widget_ch = 101 + idx       # widget name (always cB_101..cB_110)
            actual_ch = slot + 1 + idx  # 101-110, 201-210, or 301-310
            cb = getattr(self.ui, f"cB_{widget_ch}", None)
            if cb:
                cb.setText(str(actual_ch))

    def _save_slot_state(self, slot: int):
        """Snapshot gB_ChannelData widget values into per-slot cache."""
        state = {}
        for idx in range(10):
            widget_ch = 101 + idx
            cb        = getattr(self.ui, f"cB_{widget_ch}", None)
            combo     = getattr(self.ui, f"comboBox_{widget_ch}", None)
            gain_edit = getattr(self.ui, f"gain_{widget_ch}", None)
            off_edit  = getattr(self.ui, f"offset_{widget_ch}", None)
            state[idx] = {
                "checked":   cb.isChecked()       if cb        else False,
                "combo_idx": combo.currentIndex() if combo     else 0,
                "gain":      gain_edit.text()     if gain_edit else "1.0",
                "offset":    off_edit.text()      if off_edit  else "0.0",
            }
        self._slot_channel_states[slot] = state

    def _restore_slot_state(self, slot: int):
        """Restore gB_ChannelData widgets from the per-slot cache (or defaults if empty)."""
        state = self._slot_channel_states.get(slot, {})
        for idx in range(10):
            widget_ch = 101 + idx
            row       = state.get(idx, {})
            cb        = getattr(self.ui, f"cB_{widget_ch}", None)
            combo     = getattr(self.ui, f"comboBox_{widget_ch}", None)
            gain_edit = getattr(self.ui, f"gain_{widget_ch}", None)
            off_edit  = getattr(self.ui, f"offset_{widget_ch}", None)
            if cb:
                cb.setChecked(row.get("checked", False))
            if combo:
                combo.setCurrentIndex(row.get("combo_idx", 0))
            if gain_edit:
                gain_edit.setText(row.get("gain", "1.0"))
            if off_edit:
                off_edit.setText(row.get("offset", "0.0"))

    # ----------------------------------------------------------------
    # Current Controller tab
    # ----------------------------------------------------------------

    def _setup_current_controller_tab(self):
        """Build Current Controller tab (tab_4) with data logger, save data,
        metrics, and GPIB log sections."""
        layout = QVBoxLayout(self.ui.tab_4)
        layout.setContentsMargins(4, 4, 4, 4)

        # --- Top row: Data Logger | Save Data | Metrics ---
        top_row = QHBoxLayout()

        # A. Data Logger group box
        gb_data_logger = QGroupBox("Data Logger")
        dl_layout = QVBoxLayout(gb_data_logger)
        self._cc_toggle_logger_btn = QPushButton("Start Logger")
        self._cc_toggle_logger_btn.setMinimumHeight(38)
        self._cc_toggle_logger_btn.clicked.connect(self._cc_on_toggle_logger)
        dl_layout.addWidget(self._cc_toggle_logger_btn)
        interval_row = QHBoxLayout()
        interval_row.addWidget(QLabel("Sample Interval"))
        self._cc_interval_edit = QLineEdit("5")
        self._cc_interval_edit.setMaximumWidth(55)
        interval_row.addWidget(self._cc_interval_edit)
        interval_row.addWidget(QLabel("Seconds"))
        interval_row.addStretch()
        dl_layout.addLayout(interval_row)
        gb_data_logger.setMaximumWidth(200)
        top_row.addWidget(gb_data_logger)

        # B. Save Data group box
        gb_save_data = QGroupBox("Save Data")
        sd_layout = QVBoxLayout(gb_save_data)
        self._cc_save_results_btn = QPushButton("Save Results")
        self._cc_save_results_btn.setMinimumHeight(38)
        self._cc_save_results_btn.clicked.connect(self._cc_on_save_results)
        sd_layout.addWidget(self._cc_save_results_btn)
        # Delimiter selection
        delim_row = QHBoxLayout()
        delim_row.addWidget(QLabel("Delimiter:"))
        self._cc_rb_comma = QRadioButton("Comma")
        self._cc_rb_comma.setChecked(True)
        self._cc_rb_tab = QRadioButton("Tab")
        self._cc_rb_space = QRadioButton("Space")
        self._cc_rb_semicolon = QRadioButton("Semi-Colon")
        delim_row.addWidget(self._cc_rb_tab)
        delim_row.addWidget(self._cc_rb_comma)
        delim_row.addWidget(self._cc_rb_space)
        delim_row.addWidget(self._cc_rb_semicolon)
        sd_layout.addLayout(delim_row)
        # Overwrite checkbox
        self._cc_overwrite_chk = QCheckBox("Over Write File")
        sd_layout.addWidget(self._cc_overwrite_chk)
        # File header notes
        sd_layout.addWidget(QLabel("File Header Notes:"))
        self._cc_notes_edit = QTextEdit()
        self._cc_notes_edit.setMaximumHeight(55)
        self._cc_notes_edit.setPlaceholderText("Enter notes for the file header...")
        sd_layout.addWidget(self._cc_notes_edit)
        top_row.addWidget(gb_save_data)

        # C. Metrics group box
        gb_metrics = QGroupBox("Metrics")
        met_layout = QVBoxLayout(gb_metrics)
        self._cc_metric_combo = QComboBox()
        self._cc_metric_combo.addItems([
            "All Instant Metrics", "Volts", "Amps", "Watts", "VA",
            "Frequency", "Phase", "VAR", "Power Factor",
        ])
        met_layout.addWidget(self._cc_metric_combo)
        self._cc_get_metrics_btn = QPushButton("Update\nInstant\nMetrics")
        self._cc_get_metrics_btn.setMinimumHeight(48)
        self._cc_get_metrics_btn.clicked.connect(self._cc_on_get_metrics)
        met_layout.addWidget(self._cc_get_metrics_btn)
        met_layout.addStretch()
        gb_metrics.setMaximumWidth(200)
        top_row.addWidget(gb_metrics)

        # D. GPIB Log section
        gpib_group = QGroupBox("GPIB Log")
        gpib_layout = QVBoxLayout(gpib_group)
        gpib_controls = QHBoxLayout()
        self._cc_verbose_chk = QCheckBox("Verbose")
        self._cc_verbose_chk.setChecked(True)
        self._cc_save_gpib_log_btn = QPushButton("Save Log")
        self._cc_save_gpib_log_btn.clicked.connect(self._cc_on_save_gpib_log)
        self._cc_clear_gpib_log_btn = QPushButton("Clear Log")
        self._cc_clear_gpib_log_btn.clicked.connect(
            lambda: self._cc_gpib_log_browser.clear())
        gpib_controls.addWidget(self._cc_verbose_chk)
        gpib_controls.addStretch()
        gpib_controls.addWidget(self._cc_save_gpib_log_btn)
        gpib_controls.addWidget(self._cc_clear_gpib_log_btn)
        gpib_layout.addLayout(gpib_controls)
        self._cc_gpib_log_browser = QTextBrowser()
        self._cc_gpib_log_browser.setFont(QFont("Consolas", 9))
        gpib_layout.addWidget(self._cc_gpib_log_browser)
        top_row.addWidget(gpib_group)

        layout.addLayout(top_row)

        # E. Data Log display (bottom)
        layout.addWidget(QLabel("Data Log:"))
        self._cc_data_log_browser = QTextBrowser()
        self._cc_data_log_browser.setFont(QFont("Consolas", 9))
        layout.addWidget(self._cc_data_log_browser)

    # -- Current Controller handlers --

    def _cc_on_toggle_logger(self):
        """Start or stop the Current Controller data logger."""
        if self._cc_logging_active:
            # Stop logging
            self._cc_logger_timer.stop()
            self._cc_logging_active = False
            self._cc_toggle_logger_btn.setText("Start Logger")
            self._cc_interval_edit.setEnabled(True)
            self._cc_metric_combo.setEnabled(True)
            self.logger.info(
                f"Current Controller logger stopped after {self._cc_sample_count} samples"
            )
            self._cc_append_gpib_log("Logger stopped")
            return

        # Validate Radian connection
        if not self.radian_connected:
            QMessageBox.warning(
                self, "Not Connected",
                "Radian is not connected. Connect the Radian before starting the logger."
            )
            return

        # Validate interval
        try:
            interval = float(self._cc_interval_edit.text().strip())
            if interval < 0.5:
                raise ValueError("Interval must be >= 0.5")
        except ValueError:
            QMessageBox.warning(
                self, "Invalid Interval",
                "Please enter a numeric sample interval >= 0.5 seconds."
            )
            return

        # Start logging
        self._cc_sample_count = 0
        self._cc_logged_data.clear()
        self._cc_data_log_browser.clear()

        # Build header
        header_parts = ["Sample#"]
        selected = self._cc_metric_combo.currentText()
        if selected == "All Instant Metrics":
            header_parts.extend(["Volts", "Amps", "Watts", "VA", "VAR",
                                 "Frequency", "Phase", "PF"])
        else:
            header_parts.append(selected)
        header_parts.append("Timestamp")
        self._cc_data_log_browser.append("\t".join(header_parts))

        self._cc_logging_active = True
        self._cc_toggle_logger_btn.setText("Stop Logger")
        self._cc_interval_edit.setEnabled(False)
        self._cc_metric_combo.setEnabled(False)
        self._cc_logger_timer.start(int(interval * 1000))
        self._cc_append_gpib_log(
            f"Logger started — interval {interval}s, metric: {selected}"
        )
        self.logger.info(f"Current Controller logger started: interval={interval}s")

    def _cc_on_logger_tick(self):
        """Timer tick: request Radian instant metrics for the data logger."""
        full_all_cmd = "A60D0008002400000014FFFD"
        self.api_post("cc_instant_metrics", "/radian/command", {
            "hexCommand": full_all_cmd,
            "timeoutMs": 2000,
        })

    def _cc_on_get_metrics(self):
        """Manual button: request instant metrics from Radian for Current Controller tab."""
        if not self.radian_connected:
            QMessageBox.warning(self, "Not Connected", "Radian is not connected")
            return
        full_all_cmd = "A60D0008002400000014FFFD"
        self.api_post("cc_tab_instant", "/radian/command", {
            "hexCommand": full_all_cmd,
            "timeoutMs": 2000,
        })

    def _cc_format_metrics_line(self, metrics) -> str:
        """Format a metrics object into a tab-separated data line."""
        self._cc_sample_count += 1
        parts = [str(self._cc_sample_count)]
        selected = self._cc_metric_combo.currentText()

        def fmt(val):
            av = abs(val)
            if av < 10:
                return f"{val:.5f}"
            elif av < 100:
                return f"{val:.4f}"
            return f"{val:.3f}"

        if selected == "All Instant Metrics" or selected == "Volts":
            parts.append(fmt(metrics.volt))
        if selected == "All Instant Metrics" or selected == "Amps":
            parts.append(fmt(metrics.amp))
        if selected == "All Instant Metrics" or selected == "Watts":
            parts.append(fmt(metrics.watt))
        if selected == "All Instant Metrics" or selected == "VA":
            parts.append(fmt(metrics.va))
        if selected == "All Instant Metrics" or selected == "VAR":
            parts.append(fmt(metrics.var))
        if selected == "All Instant Metrics" or selected == "Frequency":
            parts.append(fmt(metrics.frequency))
        if selected == "All Instant Metrics" or selected == "Phase":
            parts.append(fmt(metrics.phase))
        if selected == "All Instant Metrics" or selected == "Power Factor":
            parts.append(fmt(metrics.power_factor))

        parts.append(datetime.now().strftime("%Y-%m-%d %H:%M:%S"))
        return "\t".join(parts)

    def _cc_on_save_results(self):
        """Save logged data to CSV with selected delimiter."""
        if not self._cc_logged_data:
            QMessageBox.information(self, "No Data", "No data has been logged yet.")
            return

        path, _ = QFileDialog.getSaveFileName(
            self, "Save Logger Data",
            str(self.data_dir / "radian_log.csv"),
            "CSV Files (*.csv);;All Files (*)",
        )
        if not path:
            return

        # Determine delimiter
        if self._cc_rb_semicolon.isChecked():
            delim = ";"
        elif self._cc_rb_tab.isChecked():
            delim = "\t"
        elif self._cc_rb_space.isChecked():
            delim = " "
        else:
            delim = ","

        now = datetime.now()
        notes = self._cc_notes_edit.toPlainText().strip()

        with open(path, "w", encoding="utf-8") as f:
            f.write(f"Radian Logger System Log File\n")
            f.write(f"Date:,{now.strftime('%Y-%m-%d')}\n")
            f.write(f"Time:,{now.strftime('%H:%M:%S')}\n")
            if notes:
                f.write(f"Notes:,{notes}\n")
            f.write("\n")
            for line in self._cc_logged_data:
                # Data is stored tab-separated; replace tabs with chosen delimiter
                f.write(line.replace("\t", delim) + "\n")

        self.logger.info(f"Current Controller data saved to {path}")
        self._cc_append_gpib_log(f"Data saved to {path}")

    def _cc_on_save_gpib_log(self):
        """Save GPIB log contents to a text file."""
        path, _ = QFileDialog.getSaveFileName(
            self, "Save GPIB Log",
            str(self.data_dir / "gpib_log.txt"),
            "Text Files (*.txt);;All Files (*)",
        )
        if not path:
            return
        now = datetime.now()
        with open(path, "w", encoding="utf-8") as f:
            f.write("California Instruments GPIB System Log File\n")
            f.write(f"Date: {now.strftime('%Y-%m-%d')}\n")
            f.write(f"Time: {now.strftime('%H:%M:%S')}\n\n")
            f.write(self._cc_gpib_log_browser.toPlainText())
        self.logger.info(f"GPIB log saved to {path}")

    def _cc_append_gpib_log(self, message: str):
        """Append a message to the Current Controller GPIB log browser."""
        if self._cc_verbose_chk.isChecked():
            timestamp = datetime.now().strftime("%H:%M:%S")
            self._cc_gpib_log_browser.append(f"[{timestamp}] {message}")

    # ----------------------------------------------------------------
    # Radian tab
    # ----------------------------------------------------------------

    def _setup_radian_tab(self):
        """Build Radian tab (tab_5) with instant and accumulated metrics."""
        layout = QVBoxLayout(self.ui.tab_5)
        layout.setContentsMargins(4, 4, 4, 4)

        # Instant metrics section
        selector_row = QHBoxLayout()
        self._radian_metric_combo = QComboBox()
        self._radian_metric_combo.addItems([
            "All Instant Metrics", "Volts", "Amps", "Watts", "VA",
            "Frequency", "Phase", "VAR", "Power Factor",
        ])
        self._radian_get_btn = QPushButton("Get Instant Metrics")
        self._radian_get_btn.clicked.connect(self._on_get_radian_instant_metrics)
        selector_row.addWidget(QLabel("Metric:"))
        selector_row.addWidget(self._radian_metric_combo)
        selector_row.addWidget(self._radian_get_btn)
        selector_row.addStretch()
        layout.addLayout(selector_row)

        self._radian_instant_browser = QTextBrowser()
        self._radian_instant_browser.setFont(QFont("Consolas", 10))
        layout.addWidget(self._radian_instant_browser)

        # Accumulated metrics section
        layout.addWidget(QLabel("Accumulated Metrics:"))
        self._radian_accum_browser = QTextBrowser()
        self._radian_accum_browser.setFont(QFont("Consolas", 10))
        self._radian_accum_browser.setMaximumHeight(150)
        layout.addWidget(self._radian_accum_browser)

    def _on_get_radian_instant_metrics(self):
        """Manual button: request instant metrics from Radian."""
        if not self.radian_connected:
            QMessageBox.warning(self, "Not Connected", "Radian is not connected")
            return
        # Request the full RD2X instant metrics packet (matches original implementation)
        # This returns 10 TI floats (40 bytes) needed by the parser.
        full_all_cmd = "A60D0008002400000014FFFD"
        self.api_post("radian_tab_instant", "/radian/command", {
            "hexCommand": full_all_cmd,
            "timeoutMs": 2000,
        })

    def _display_radian_instant_metrics(self, metrics):
        """Display parsed instant metrics in the Radian tab."""
        selected = self._radian_metric_combo.currentText()
        lines = []

        def add(label, value, unit):
            lines.append(f"{label:20s}: {value:>12.5f} {unit}")

        if selected == "All Instant Metrics" or selected == "Volts":
            add("Voltage", metrics.volt, "V")
        if selected == "All Instant Metrics" or selected == "Amps":
            add("Current", metrics.amp, "A")
        if selected == "All Instant Metrics" or selected == "Watts":
            add("Power", metrics.watt, "W")
        if selected == "All Instant Metrics" or selected == "VA":
            add("Apparent Power", metrics.va, "VA")
        if selected == "All Instant Metrics" or selected == "VAR":
            add("Reactive Power", metrics.var, "VAR")
        if selected == "All Instant Metrics" or selected == "Frequency":
            add("Frequency", metrics.frequency, "Hz")
        if selected == "All Instant Metrics" or selected == "Phase":
            add("Phase", metrics.phase, "deg")
        if selected == "All Instant Metrics" or selected == "Power Factor":
            add("Power Factor", metrics.power_factor, "")

        self._radian_instant_browser.setText("\n".join(lines))

    # ----------------------------------------------------------------
    # PAC Power tab
    # ----------------------------------------------------------------

    def _setup_pac_power_tab(self):
        """Build PAC Power tab (tab_6) with voltage/frequency controls."""
        layout = QVBoxLayout(self.ui.tab_6)
        layout.setContentsMargins(4, 4, 4, 4)

        # Voltage control
        volt_row = QHBoxLayout()
        self._pac_volt_edit = QLineEdit()
        self._pac_volt_edit.setPlaceholderText("Voltage RMS")
        self._pac_set_volt_btn = QPushButton("Set Voltage")
        self._pac_get_volt_btn = QPushButton("Get Voltage")
        self._pac_set_volt_btn.clicked.connect(self._on_pac_set_voltage)
        self._pac_get_volt_btn.clicked.connect(self._on_pac_get_voltage)
        volt_row.addWidget(QLabel("Voltage:"))
        volt_row.addWidget(self._pac_volt_edit)
        volt_row.addWidget(QLabel("V"))
        volt_row.addWidget(self._pac_set_volt_btn)
        volt_row.addWidget(self._pac_get_volt_btn)
        layout.addLayout(volt_row)

        # Frequency control
        freq_row = QHBoxLayout()
        self._pac_freq_edit = QLineEdit()
        self._pac_freq_edit.setPlaceholderText("Frequency Hz")
        self._pac_set_freq_btn = QPushButton("Set Frequency")
        self._pac_get_freq_btn = QPushButton("Get Frequency")
        self._pac_set_freq_btn.clicked.connect(self._on_pac_set_frequency)
        self._pac_get_freq_btn.clicked.connect(self._on_pac_get_frequency)
        freq_row.addWidget(QLabel("Frequency:"))
        freq_row.addWidget(self._pac_freq_edit)
        freq_row.addWidget(QLabel("Hz"))
        freq_row.addWidget(self._pac_set_freq_btn)
        freq_row.addWidget(self._pac_get_freq_btn)
        layout.addLayout(freq_row)

        # Output control
        output_row = QHBoxLayout()
        self._pac_output_on_btn = QPushButton("Output ON")
        self._pac_output_off_btn = QPushButton("Output OFF")
        self._pac_measure_btn = QPushButton("Measure All")
        self._pac_output_on_btn.clicked.connect(
            lambda: self.api_post("pac_output_on", "/pac/output-on", {}))
        self._pac_output_off_btn.clicked.connect(
            lambda: self.api_post("pac_output_off", "/pac/output-off", {}))
        self._pac_measure_btn.clicked.connect(
            lambda: self.api_get("pac_measure_all", "/pac/measure/all"))
        output_row.addWidget(self._pac_output_on_btn)
        output_row.addWidget(self._pac_output_off_btn)
        output_row.addWidget(self._pac_measure_btn)
        output_row.addStretch()
        layout.addLayout(output_row)

        # Log/readback display
        self._pac_log_browser = QTextBrowser()
        self._pac_log_browser.setFont(QFont("Consolas", 9))
        layout.addWidget(self._pac_log_browser)

    def _on_pac_set_voltage(self):
        if not self.pac_connected:
            QMessageBox.warning(self, "Not Connected", "PAC Power is not connected")
            return
        try:
            voltage = float(self._pac_volt_edit.text())
            self.api_post("pac_set_voltage", "/pac/set-voltage", {"voltage": voltage})
        except ValueError:
            QMessageBox.warning(self, "Invalid Input", "Enter a valid voltage")

    def _on_pac_get_voltage(self):
        if not self.pac_connected:
            QMessageBox.warning(self, "Not Connected", "PAC Power is not connected")
            return
        self.api_get("pac_tab_get_voltage", "/pac/measure/voltage")

    def _on_pac_set_frequency(self):
        if not self.pac_connected:
            QMessageBox.warning(self, "Not Connected", "PAC Power is not connected")
            return
        try:
            freq = float(self._pac_freq_edit.text())
            self.api_post("pac_tab_set_frequency", "/pac/set-frequency", {"frequency": freq})
        except ValueError:
            QMessageBox.warning(self, "Invalid Input", "Enter a valid frequency")

    def _on_pac_get_frequency(self):
        if not self.pac_connected:
            QMessageBox.warning(self, "Not Connected", "PAC Power is not connected")
            return
        self.api_get("pac_tab_get_frequency", "/pac/measure/frequency")

    # ----------------------------------------------------------------
    # DAQ Log tab
    # ----------------------------------------------------------------

    def _setup_daq_log_tab(self):
        """Build DAQ Log tab (tab_3) with timestamped Radian metric samples."""
        layout = QVBoxLayout(self.ui.tab_3)
        layout.setContentsMargins(4, 4, 4, 4)

        controls = QHBoxLayout()
        self._daq_log_clear_btn = QPushButton("Clear")
        self._daq_log_clear_btn.clicked.connect(lambda: self._daq_log_browser.clear())
        self._daq_log_save_btn = QPushButton("Save")
        self._daq_log_save_btn.clicked.connect(self._save_daq_log)
        controls.addStretch()
        controls.addWidget(self._daq_log_clear_btn)
        controls.addWidget(self._daq_log_save_btn)
        layout.addLayout(controls)

        self._daq_log_browser = QTextBrowser()
        self._daq_log_browser.setFont(QFont("Consolas", 9))
        layout.addWidget(self._daq_log_browser)

        self._daq_sample_count = 0
        self._last_radian_metrics = None

    def _append_daq_log_entry(self, metrics):
        """Append a Radian metric sample to the DAQ Log tab."""
        self._daq_sample_count += 1
        parts = [str(self._daq_sample_count)]

        def fmt(val):
            """Format number like original VB: magnitude-based decimal places."""
            av = abs(val)
            if av < 10:
                return f"{val:.5f}"
            elif av < 100:
                return f"{val:.4f}"
            return f"{val:.3f}"

        if self.ui.cb_VoltsData.isChecked():
            parts.append(fmt(metrics.volt))
        if self.ui.cb_CurrentData.isChecked():
            parts.append(fmt(metrics.amp))
        if self.ui.cb_DataFrequency.isChecked():
            parts.append(fmt(metrics.frequency))
        if self.ui.cb_PhaseData.isChecked():
            parts.append(fmt(metrics.phase))

        parts.append(datetime.now().strftime("%Y-%m-%d %H:%M:%S"))
        self._daq_log_browser.append("\t".join(parts))

    def _save_daq_log(self):
        """Save DAQ log contents to a text file."""
        path, _ = QFileDialog.getSaveFileName(
            self, "Save DAQ Log", str(self.data_dir / "daq_log.txt"),
            "Text Files (*.txt);;All Files (*)",
        )
        if path:
            with open(path, "w", encoding="utf-8") as f:
                f.write(self._daq_log_browser.toPlainText())
            self.logger.info(f"DAQ log saved to {path}")

    # ----------------------------------------------------------------
    # Error Log tab
    # ----------------------------------------------------------------

    def _setup_error_log_tab(self):
        """Build the Error Log tab UI and attach the Qt log handler."""
        layout = QVBoxLayout(self.ui.tab_7)
        layout.setContentsMargins(4, 4, 4, 4)

        # Controls row
        controls = QHBoxLayout()
        self._log_level_combo = QComboBox()
        self._log_level_combo.addItems(["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"])
        self._log_level_combo.setCurrentText("DEBUG")
        self._log_level_combo.currentTextChanged.connect(self._on_log_level_filter_changed)

        self._log_clear_btn = QPushButton("Clear")
        self._log_clear_btn.clicked.connect(lambda: self._log_browser.clear())

        self._log_export_btn = QPushButton("Export")
        self._log_export_btn.clicked.connect(self._export_error_log)

        controls.addWidget(QLabel("Min Level:"))
        controls.addWidget(self._log_level_combo)
        controls.addStretch()
        controls.addWidget(self._log_clear_btn)
        controls.addWidget(self._log_export_btn)
        layout.addLayout(controls)

        # Log text browser
        self._log_browser = QTextBrowser()
        self._log_browser.setOpenExternalLinks(False)
        self._log_browser.setFont(QFont("Consolas", 9))
        layout.addWidget(self._log_browser)

        # Current filter level
        self._log_min_level = logging.DEBUG

        # Attach Qt handler to logger
        self._qt_log_handler = QtLogHandler()
        self._qt_log_handler.setFormatter(
            logging.Formatter("%(asctime)s - %(name)s - %(levelname)s - %(message)s")
        )
        self._qt_log_handler.emitter.log_record.connect(self._on_log_record)
        self.logger.addHandler(self._qt_log_handler)

    def _on_log_record(self, message: str, level: int):
        """Append a color-coded log record to the Error Log browser."""
        if level < self._log_min_level:
            return
        color = QtLogHandler.LEVEL_COLORS.get(level, "#000000")
        self._log_browser.append(f'<span style="color:{color}">{message}</span>')

    def _on_log_level_filter_changed(self, text: str):
        """Update the minimum log level filter."""
        level_map = {
            "DEBUG": logging.DEBUG,
            "INFO": logging.INFO,
            "WARNING": logging.WARNING,
            "ERROR": logging.ERROR,
            "CRITICAL": logging.CRITICAL,
        }
        self._log_min_level = level_map.get(text, logging.DEBUG)

    def _export_error_log(self):
        """Export the error log contents to a text file."""
        path, _ = QFileDialog.getSaveFileName(
            self, "Export Error Log", str(self.logs_dir / "error_log.txt"),
            "Text Files (*.txt);;All Files (*)",
        )
        if path:
            with open(path, "w", encoding="utf-8") as f:
                f.write(self._log_browser.toPlainText())
            self.logger.info(f"Error log exported to {path}")

    # ----------------------------------------------------------------
    # Email notifications
    # ----------------------------------------------------------------

    def _open_email_dialog(self):
        """Open the email notification settings dialog."""
        dlg = EmailDialog(self._ldap_lookup, self._email_domain, parent=self)
        dlg.load_state(
            to=self._email_recipients_to,
            cc=self._email_recipients_cc,
            notify_done=self._email_notify_test_done,
            notify_thresh=self._email_notify_threshold,
            operator=self._email_operator,
            serial=self._email_serial,
            simulated=self._email_simulated,
            project=self._email_project,
            notes=self._email_notes,
        )
        if dlg.exec() == EmailDialog.DialogCode.Accepted:
            self._email_recipients_to = dlg.recipients_to
            self._email_recipients_cc = dlg.recipients_cc
            self._email_notify_test_done = dlg.notify_test_done
            self._email_notify_threshold = dlg.notify_threshold
            self._email_operator = dlg.operator
            self._email_serial = dlg.serial_number
            self._email_simulated = dlg.simulated_meter
            self._email_project = dlg.project_number
            self._email_notes = dlg.notes
            self.logger.info(
                f"Email settings updated: to={self._email_recipients_to}, "
                f"cc={self._email_recipients_cc}"
            )

    def _get_current_metrics_dict(self) -> dict:
        """Get latest Radian metrics as a dict for email body."""
        m = self._last_radian_metrics
        if m:
            return {
                "volt": f"{m.volt:.3f}",
                "amp": f"{m.amp:.3f}",
                "frequency": f"{m.frequency:.2f}",
                "phase": f"{m.phase:.2f}",
            }
        return {"volt": "N/A", "amp": "N/A", "frequency": "N/A", "phase": "N/A"}

    def _send_test_complete_email(self, log_path: str = ""):
        """Send test-complete email if configured."""
        if not self._email_recipients_to:
            return
        if not self._email_notify_test_done:
            return

        start = self.test_start_time.strftime("%Y-%m-%d %H:%M:%S") if self.test_start_time else "N/A"
        end = self.test_end_time.strftime("%Y-%m-%d %H:%M:%S") if self.test_end_time else "N/A"

        # Send in background thread to avoid blocking UI
        import threading
        threading.Thread(
            target=self._email_service.send_test_complete,
            args=(
                self._email_recipients_to, self._email_recipients_cc,
                self._email_operator, self._email_serial,
                self._email_project, self._email_notes,
                self._get_current_metrics_dict(),
                log_path, start, end,
            ),
            daemon=True,
        ).start()
        self.logger.info("Test-complete email queued for sending")

    def _send_threshold_warning_email(self, warning_body: str):
        """Send threshold warning email if configured and cooldown elapsed."""
        if not self._email_recipients_to:
            return
        if not self._email_notify_threshold:
            return

        import time
        now = time.time()
        if now - self._last_warning_time < 600:  # 10-minute cooldown
            return
        self._last_warning_time = now

        import threading
        threading.Thread(
            target=self._email_service.send_threshold_warning,
            args=(
                self._email_recipients_to, self._email_recipients_cc,
                self._email_operator, self._email_serial,
                self._email_project, warning_body,
                self._get_current_metrics_dict(),
            ),
            daemon=True,
        ).start()
        self.logger.info("Threshold warning email queued for sending")

    # ----------------------------------------------------------------
    # Window close
    # ----------------------------------------------------------------

    def closeEvent(self, event):
        # Stop CC logger if running
        if self._cc_logging_active:
            self._cc_logger_timer.stop()
            self._cc_logging_active = False

        # Close LDAP connection
        if self._ldap_lookup:
            self._ldap_lookup.close()

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
        s.setValue("pac/port", self.ui.comboBox_PowerPac.currentText())
        s.setValue("pac/baud", self.ui.lineEdit_PACBaud.text())

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
        elif self.ui.button_slot300.isChecked():
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

        # Mode inputs
        s.setValue("test/duration_hours", self.ui.lineEdit_Hours.text())
        s.setValue("test/duration_minutes", self.ui.lineEdit_Minutes.text())
        s.setValue("test/duration_seconds", self.ui.lineEdit_Seconds.text())
        s.setValue("test/target_readings", self._readings_spinbox.value())

        # Current Controller
        s.setValue("cc/interval", self._cc_interval_edit.text())
        s.setValue("cc/verbose", self._cc_verbose_chk.isChecked())
        if self._cc_rb_tab.isChecked():
            s.setValue("cc/delimiter", "tab")
        elif self._cc_rb_space.isChecked():
            s.setValue("cc/delimiter", "space")
        elif self._cc_rb_semicolon.isChecked():
            s.setValue("cc/delimiter", "semicolon")
        else:
            s.setValue("cc/delimiter", "comma")

        # Email notifications
        s.setValue("email/recipients_to", self._email_recipients_to)
        s.setValue("email/recipients_cc", self._email_recipients_cc)
        s.setValue("email/notify_test_done", self._email_notify_test_done)
        s.setValue("email/notify_threshold", self._email_notify_threshold)
        s.setValue("email/operator", self._email_operator)
        s.setValue("email/serial", self._email_serial)
        s.setValue("email/simulated", self._email_simulated)
        s.setValue("email/project", self._email_project)
        s.setValue("email/notes", self._email_notes)

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
        self._select_combo_value(self.ui.comboBox_PowerPac, s.value("pac/port", ""))
        pac_baud = s.value("pac/baud", "")
        if pac_baud:
            self.ui.lineEdit_PACBaud.setText(str(pac_baud))

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
            slot = s.value("test/slot", 200, type=int)
        elif slot == 300:
            self.ui.button_slot300.setChecked(True)
            slot = s.value("test/slot", 300, type=int)
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

        # Mode inputs
        self.ui.lineEdit_Hours.setText(str(s.value("test/duration_hours", "1")))
        self.ui.lineEdit_Minutes.setText(str(s.value("test/duration_minutes", "0")))
        self.ui.lineEdit_Seconds.setText(str(s.value("test/duration_seconds", "0")))
        target_readings = s.value("test/target_readings", 100, type=int)
        self._readings_spinbox.setValue(target_readings)

        # Current Controller
        cc_interval = s.value("cc/interval", "5")
        if cc_interval:
            self._cc_interval_edit.setText(str(cc_interval))
        self._cc_verbose_chk.setChecked(s.value("cc/verbose", True, type=bool))
        cc_delim = s.value("cc/delimiter", "comma")
        if cc_delim == "tab":
            self._cc_rb_tab.setChecked(True)
        elif cc_delim == "space":
            self._cc_rb_space.setChecked(True)
        elif cc_delim == "semicolon":
            self._cc_rb_semicolon.setChecked(True)
        else:
            self._cc_rb_comma.setChecked(True)

        # Email notifications
        to_list = s.value("email/recipients_to", [])
        self._email_recipients_to = to_list if isinstance(to_list, list) else []
        cc_list = s.value("email/recipients_cc", [])
        self._email_recipients_cc = cc_list if isinstance(cc_list, list) else []
        self._email_notify_test_done = s.value("email/notify_test_done", True, type=bool)
        self._email_notify_threshold = s.value("email/notify_threshold", False, type=bool)
        self._email_operator = str(s.value("email/operator", ""))
        self._email_serial = str(s.value("email/serial", ""))
        self._email_simulated = s.value("email/simulated", False, type=bool)
        self._email_project = str(s.value("email/project", ""))
        self._email_notes = str(s.value("email/notes", ""))

        self.logger.info("Settings restored")

    @staticmethod
    def _select_combo_value(combo, value: str):
        """Select a combo box item by text value, if it exists."""
        if not value:
            return
        idx = combo.findText(str(value))
        if idx >= 0:
            combo.setCurrentIndex(idx)
