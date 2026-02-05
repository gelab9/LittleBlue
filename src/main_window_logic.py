"""Main application window with backend logic."""

import logging
from pathlib import Path
from datetime import datetime
from PyQt6.QtWidgets import (
    QMainWindow, QMessageBox, QFileDialog
)
from PyQt6.QtCore import QTimer, QThread, pyqtSignal
from PyQt6 import QtCore

from src.ui.main_window import Ui_MainWindow
from src.util.config_loader import ConfigLoader
from src.util.logging_setup import LoggingSetup
from src.util.stubs import NullCsvLogger
from src.util.stubs import NullReportGenerator
from src.util.stubs import NullStatistics
from src.util.stubs import NullTestController
from src.devices.daq_34970a import DAQ34970A
from serial.tools import list_ports
from PyQt6 import QtWidgets


class MainWindow(QMainWindow):
    """Main application window with integrated backend."""
    
    def __init__(self):
        super().__init__()
        
        # Initialize configuration
        self.config = ConfigLoader()
        self.base_dir = self.config.base_dir
        self.logs_dir = self.config.logs_dir
        self.data_dir = self.config.data_dir
        
        # Initialize logging
        logging_setup = LoggingSetup(
            self.config.get_logs_dir(),
            level=self.config.get("logging.level", "DEBUG"),
            log_name=self.config.get("logging.developer_log")
        )
        self.logger = logging_setup.setup_logger("SmartMeterApp")
        self.logger.info("=" * 60)
        self.logger.info("Application starting")
        self.logger.info(f"Mock Mode: {self.config.is_mock_mode()}")
        self.logger.info(f"Poll Interval: {self.config.get_poll_interval_ms()}ms")
        self.logger.info("=" * 60)
        
        # Setup UI from generated file
        self.ui = Ui_MainWindow()
        self.ui.setupUi(self)
        
        # Mock classes from before
        self.csv_logger = NullCsvLogger()
        self.statistics = NullStatistics()
        self.report_generator = NullReportGenerator()
        self.test_controller = NullTestController()

        
        # State variables
        self.test_start_time = None
        self.test_end_time = None

        # setup daq34709a instance variable
        self.daq = None
        self.daq_connected = False
        
        self.test_controller.daq = self.daq

        # Setup UI elements and connections
        self.setup_connections()
        self.populate_defaults()
        
        # Setup timer for polling
        self.poll_timer = QTimer()
        self.poll_timer.timeout.connect(self.on_poll)
        self.poll_interval_ms = self.config.get_poll_interval_ms()
        
        # Update window title
        self.setWindowTitle("Smart Meter GUI - Test & Development")
        
        self.logger.info("Application initialized successfully")

        # Initialize statistics and report generator
        self.statistics = NullStatistics(self.logger)
        self.report_generator = NullReportGenerator(self.config.get_data_dir(), self.logger)
        
    def setup_clock(self):
            """Setup clock to update timestamp label."""
            self.clock_timer = QTimer()
            self.clock_timer.timeout.connect(self.update_timestamp)
            self.clock_timer.start(1000)  # Update every second
    
    def setup_connections(self):
        """Connect UI signals to slots."""
        # DAQ tab connections
        self.ui.pb_Connect34970A.clicked.connect(self.on_connect_34970a)
        self.ui.pb_setVoltage.clicked.connect(self.on_set_voltage)
        self.ui.pb_voltageOff.clicked.connect(self.on_voltage_off)
        self.ui.pB_Start.clicked.connect(self.on_start_test)
        
        # Radian connections
        self.ui.pb_ConnectRadian.clicked.connect(self.on_connect_radian)
        
        # Cal Inst connections
        self.ui.pb_OpenCalInstConx.clicked.connect(self.on_open_cal_inst)
        self.ui.pb_CloseCalInstConx.clicked.connect(self.on_close_cal_inst)
        
        # PAC Power connections
        self.ui.pushButton.clicked.connect(self.on_connect_pac_power)
        
        # Data checkboxes
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

    def get_selected_daq_channels(self) -> list[int]:
        """
        Map the UI slot selection (100/200/300) to 34970A channel numbers.
        Temporary mapping:
        Slot 100 -> 101-120
        Slot 200 -> 201-220
        Slot 300 -> 301-320
        """
        if self.ui.button_slot100.isChecked():
            return list(range(101, 121))
        elif self.ui.button_slot200.isChecked():
            return list(range(201, 221))
        else:
            return list(range(301, 321))

    
    def refresh_serial_ports(self):
        ports = [p.device for p in list_ports.comports()]

        self.ui.cb_Port24970A.clear()
        self.ui.cb_Port24970A.addItems(ports)

        self.ui.cB_PortRadian.clear()
        self.ui.cB_PortRadian.addItems(ports)

        self.logger.info(f"Detected serial ports: {ports}")
    
    def populate_defaults(self):
        """Populate default values in UI elements."""
        # Set default values for reading intervals
        self.ui.cB_readIntervals.setCurrentIndex(0)
        
        # Set default mode
        self.ui.button_Free.setChecked(True)
        
        # Set default slot
        self.ui.button_slot100.setChecked(True)
        
        # Populate baud rate combo (already in UI but ensure values)
        baud_rates = ["9600", "19200", "38400", "57600", "115200"]
        self.ui.cb_baudRate24970A.clear()
        self.ui.cb_baudRate24970A.addItems(baud_rates)
        
        # Update timestamp
        self.update_timestamp()
        
        self.logger.info("UI defaults populated")

        self.refresh_serial_ports()
    
    def update_timestamp(self):
        """Update the timestamp label."""
        current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        self.ui.label_timeStamp.setText(current_time)
    
    # ============ DAQ Tab Handlers ============
    
    def on_connect_34970a(self):
        # If already connected, disconnect
        if self.daq and self.daq.is_connected():
            try:
                self.daq.disconnect()
            finally:
                self.test_controller.daq = None
                self.ui.pb_Connect34970A.setText("Connect")
                self.ui.textBrowser_State.setText("DISCONNECTED")
            return

        baud_rate = int(self.ui.cb_baudRate24970A.currentText())
        port = self.ui.cb_Port24970A.currentText()

        if not port:
            QMessageBox.warning(self, "Connection Error", "Please select a COM port")
            return

        try:
            self.daq = DAQ34970A(port=port, baudrate=baud_rate, logger=self.logger)
            self.daq.connect()
            idn = self.daq.identify()

            self.test_controller.daq = self.daq

            self.ui.textBrowser_State.setText(idn if idn else "CONNECTED (no IDN response)")
            self.ui.pb_Connect34970A.setText("Disconnect")
            self.logger.info(f"34970A connected: {idn}")

        except Exception as e:
            self.logger.error(f"34970A connection failed: {e}", exc_info=True)
            QMessageBox.critical(self, "DAQ Error", str(e))
            self.daq = None
            self.ui.pb_Connect34970A.setText("Connect")

    
    def on_set_voltage(self):
        """Handle set voltage button."""
        voltage_str = self.ui.lineEdit_Voltage.text()
        
        try:
            voltage = float(voltage_str)
            if voltage < 0 or voltage > 300:
                raise ValueError("Voltage must be between 0 and 300V")
            
            self.logger.info(f"Setting voltage to {voltage}V")
            # TODO: Implement actual voltage setting
            self.ui.textBrowser_State.setText(f"Voltage set to {voltage}V")
            
        except ValueError as e:
            QMessageBox.warning(self, "Invalid Input", f"Invalid voltage: {e}")
            self.logger.warning(f"Invalid voltage input: {voltage_str}")
    
    def on_voltage_off(self):
        """Handle voltage off button."""
        self.logger.info("Turning off voltage")
        self.ui.textBrowser_State.setText("Voltage OFF")
        # TODO: Implement actual voltage off
    
    def on_start_test(self):
        """Handle start test button."""
        if self.test_controller.is_running:
            # If already running, stop the test
            self.test_controller.stop()
            self.poll_timer.stop()
            self.on_test_complete()
            return
        
        # Get selected mode
        if self.ui.button_Free.isChecked():
            mode = self.test_controller.MODE_FREE
            target = 0
        elif self.ui.button_Readings.isChecked():
            mode = self.test_controller.MODE_READINGS
            try:
                target = int(self.ui.cB_readIntervals.currentText())
            except ValueError:
                target = 100
        else:
            mode = self.test_controller.MODE_DURATION
            try:
                target = int(self.ui.cB_readIntervals.currentText())
            except ValueError:
                target = 60
        
        # Get selected slot
        if self.ui.button_slot100.isChecked():
            slot = "Slot 100"
        elif self.ui.button_slot200.isChecked():
            slot = "Slot 200"
        else:
            slot = "Slot 300"
        
        self.logger.info(f"Starting test - Mode: {mode}, Slot: {slot}, Target: {target}")
        self.test_start_time = datetime.now()
        
        # Reset statistics
        self.statistics.reset()
        
        # Set test mode
        self.test_controller.set_mode(mode, target)
        
        # Update UI
        self.ui.label_startTime.setText(self.test_start_time.strftime("%H:%M:%S"))
        self.ui.pB_Start.setText("Stop Test")
        self.ui.pB_Start.setStyleSheet("background-color: #ff6b6b;")
        
        # Clear previous data
        self.ui.tableWidget_Data.setRowCount(0)

        # Configure 34970A scan channels
        if self.daq and self.daq.is_connected() and not self.config.is_mock_mode():
            channels = self.get_selected_daq_channels()
            try:
                self.daq.configure_scan(channels)
                self.logger.info(f"34970A scan configured for channels: {channels}")
            except Exception as e:
                self.logger.error(f"Failed to configure 34970A scan: {e}", exc_info=True)
                QMessageBox.critical(self, "DAQ Error", f"failed to configure scan list: \n{e}")
                return
        
        # Start the test controller
        self.test_controller.start()
        self.poll_timer.start(self.poll_interval_ms)
        
    def on_poll(self):
        """Handle polling timer tick."""
        reading = self.test_controller.get_reading()
        
        if reading:
            # Add to statistics
            self.statistics.add_reading(reading)
            
            # Log to CSV
            self.csv_logger.append_row(reading)
            
            # Update data display based on checkboxes
            display_data = {}
            if self.ui.cb_VoltsData.isChecked():
                display_data["Voltage"] = f"{reading.get('voltage_rms', 0):.2f}V"
            if self.ui.cb_CurrentData.isChecked():
                display_data["Current"] = f"{reading.get('current_rms', 0):.2f}A"
            if self.ui.cb_DataFrequency.isChecked():
                display_data["Frequency"] = f"{reading.get('frequency', 0):.2f}Hz"
            if self.ui.cb_PhaseData.isChecked():
                display_data["Phase"] = f"{reading.get('phase', 0):.2f}°"
            
            # Update table
            self.ui.tableWidget_Data.insertRow(0)
            col = 0
            for key, value in reading.items():
                if col < self.ui.tableWidget_Data.columnCount():
                    from PyQt6 import QtWidgets
                    self.ui.tableWidget_Data.setItem(0, col, QtWidgets.QTableWidgetItem(str(value)))
                    col += 1
            
            # Update text browser with last reading
            text = "Last Reading:\n"
            for k, v in display_data.items():
                text += f"{k}: {v}\n"
            self.ui.textBrowser_Data.setText(text)
            
            # Update reading count
            self.ui.label_totalNumReadings.setText(f"Total Number of Readings: {self.test_controller.reading_count}")
        
        # Check if test should stop automatically
        if not self.test_controller.is_running and self.ui.pB_Start.text() == "Stop Test":
            self.on_test_complete()
    
    def on_test_complete(self):
        """Handle test completion."""
        self.test_end_time = datetime.now()
        duration = (self.test_end_time - self.test_start_time).total_seconds()
        
        self.logger.info(f"Test completed. Duration: {duration:.2f}s, Readings: {self.test_controller.reading_count}")
        
        # Calculate statistics
        stats = self.statistics.get_summary()
        
        # Generate report
        test_name = self.test_start_time.strftime("%Y%m%d_%H%M%S_test")
        report_data = {
            'start_time': self.test_start_time.strftime("%Y-%m-%d %H:%M:%S"),
            'end_time': self.test_end_time.strftime("%Y-%m-%d %H:%M:%S"),
            'duration': duration,
            'mode': self.test_controller.mode,
            'statistics': stats
        }
        
        try:
            self.report_generator.generate_report(test_name, report_data)
        except Exception as e:
            self.logger.error(f"Failed to generate report: {e}")
        
        # Update UI with statistics
        self.ui.label_terminateTime.setText(self.test_end_time.strftime("%H:%M:%S"))
        self.ui.label_Duration.setText(f"Duration: {duration:.2f}s")
        self.ui.pB_Start.setText("Start Reading!")
        self.ui.pB_Start.setStyleSheet("")
        
        # Display summary statistics
        summary_text = f"Test Complete!\n\n"
        summary_text += f"Total Readings: {stats.get('total_readings', 0)}\n"
        summary_text += f"Duration: {duration:.2f}s\n\n"
        
        if stats.get('avg'):
            summary_text += "Average Values:\n"
            for field, value in stats['avg'].items():
                summary_text += f"  {field}: {value:.2f}\n"
        
        self.ui.textBrowser_Data.setText(summary_text)
        
        self.poll_timer.stop()
    
        QMessageBox.information(self, "Test Complete", 
            f"Test completed!\n\nReadings: {self.test_controller.reading_count}\nDuration: {duration:.2f}s\n\nReport saved!")
            
    # ============ Radian Tab Handlers ============
    
    def on_connect_radian(self):
        """Handle Radian connection."""
        self.logger.info("Radian connection requested (Mock)")
        QMessageBox.information(self, "Radian", "Radian connection (Mock implementation)")
    
    # ============ Cal Inst Handlers ============
    
    def on_open_cal_inst(self):
        """Handle Cal Inst open connection."""
        board_id = self.ui.spinBox_BoardID.value()
        prim_addr = self.ui.spinBox_PrimAddress.value()
        sec_addr = self.ui.cb_SecAddress.currentText()
        
        self.logger.info(f"Opening Cal Inst - Board: {board_id}, Primary: {prim_addr}, Secondary: {sec_addr}")
        QMessageBox.information(self, "Cal Inst", "Cal Inst connection opened (Mock)")
    
    def on_close_cal_inst(self):
        """Handle Cal Inst close connection."""
        self.logger.info("Closing Cal Inst connection")
        QMessageBox.information(self, "Cal Inst", "Cal Inst connection closed (Mock)")
    
    # ============ PAC Power Handlers ============
    
    def on_connect_pac_power(self):
        """Handle PAC Power connection."""
        baud_rate = self.ui.lineEdit.text()
        port = self.ui.comboBox_2.currentText()
        
        self.logger.info(f"Connecting to PAC Power on {port} at {baud_rate} baud")
        QMessageBox.information(self, "PAC Power", "PAC Power connection (Mock implementation)")
    
    # ============ Data Filter Handlers ============
    
    def on_data_filter_changed(self):
        """Handle data filter checkbox changes."""
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
    
    # ============ Mode Handlers ============
    
    def on_mode_changed(self):
        """Handle mode selection change."""
        if self.ui.button_Free.isChecked():
            mode = "Free"
        elif self.ui.button_Readings.isChecked():
            mode = "# of Readings"
        else:
            mode = "Duration"
        
        self.logger.debug(f"Test mode changed to: {mode}")
    
    # ============ Slot Handlers ============
    
    def on_slot_changed(self):
        """Handle slot selection change."""
        if self.ui.button_slot100.isChecked():
            slot = "Slot 100"
        elif self.ui.button_slot200.isChecked():
            slot = "Slot 200"
        else:
            slot = "Slot 300"
        
        self.logger.debug(f"Slot changed to: {slot}")
    
    def closeEvent(self, event):
        """Handle window close event."""
        if self.test_controller.is_running:
            reply = QMessageBox.question(self, "Test Running", 
                "A test is currently running. Do you want to stop it and exit?",
                QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            
            if reply == QMessageBox.StandardButton.Yes:
                self.test_controller.stop()
                self.poll_timer.stop()
                self.logger.info("Application closed while test was running")
                event.accept()
            else:
                event.ignore()
        else:
            self.logger.info("Application closed normally")
            event.accept()