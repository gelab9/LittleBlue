"""Radian Power Analyzer device driver (Python frontend).

This driver communicates with the VB.NET backend via REST API.
The backend handles actual serial port communication with Radian hardware.

Based on original VB code: original/mRadian.vb
"""

import logging
from typing import Optional
from src.devices.base import DeviceBase
from src.models.radian_metrics import RadianInstantMetrics, RadianAccumulatedMetrics, RadianPacket
from src.util.data_conversion import ti_float_to_ieee_single


class RadianDevice(DeviceBase):
    """Radian Power Analyzer device driver."""

    def __init__(self, logger: Optional[logging.Logger] = None):
        """Initialize Radian device.

        Args:
            logger: Optional logger instance
        """
        self.logger = logger or logging.getLogger(__name__)
        self._connected = False

    def connect(self, port: str, baud_rate: int = 9600) -> bool:
        """Connect to Radian power analyzer.

        Args:
            port: COM port name (e.g., "COM3")
            baud_rate: Baud rate (default 9600)

        Returns:
            True if connection successful, False otherwise
        """
        try:
            self.logger.info(f"Connecting to Radian on {port} @ {baud_rate}")
            # Connection will be handled by backend via REST API in main_window_logic
            self._connected = True
            return True
        except Exception as e:
            self.logger.error(f"Failed to connect to Radian: {e}")
            self._connected = False
            return False

    def disconnect(self) -> bool:
        """Disconnect from Radian.

        Returns:
            True if disconnection successful, False otherwise
        """
        try:
            self.logger.info("Disconnecting from Radian")
            self._connected = False
            return True
        except Exception as e:
            self.logger.error(f"Failed to disconnect from Radian: {e}")
            return False

    def is_connected(self) -> bool:
        """Check if device is connected.

        Returns:
            True if connected, False otherwise
        """
        return self._connected

    def parse_instant_metrics(self, data: bytes) -> Optional[RadianInstantMetrics]:
        """Parse instant metrics from Radian response.

        The response contains TI floating-point values for each metric.

        Args:
            data: Raw response bytes from Radian

        Returns:
            RadianInstantMetrics object or None if parsing fails
        """
        try:
            # Parse Radian packet structure
            packet = RadianPacket.from_bytes(data)
            if packet is None or len(packet.data) < 32:  # Need at least 8 floats * 4 bytes
                self.logger.error("Invalid packet or insufficient data for instant metrics")
                return None

            # Extract TI float values (4 bytes each)
            metrics = RadianInstantMetrics()
            offset = 0

            # Parse each metric (order based on original VB code)
            metrics.volt = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.amp = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.watt = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.va = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.var = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.frequency = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.phase = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.power_factor = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0

            self.logger.debug(f"Parsed instant metrics: V={metrics.volt:.2f}, "
                            f"A={metrics.amp:.2f}, W={metrics.watt:.2f}")

            return metrics

        except Exception as e:
            self.logger.error(f"Failed to parse instant metrics: {e}")
            return None

    def parse_accumulated_metrics(self, data: bytes) -> Optional[RadianAccumulatedMetrics]:
        """Parse accumulated metrics from Radian response.

        Args:
            data: Raw response bytes from Radian

        Returns:
            RadianAccumulatedMetrics object or None if parsing fails
        """
        try:
            packet = RadianPacket.from_bytes(data)
            if packet is None or len(packet.data) < 16:  # Need at least 4 floats * 4 bytes
                self.logger.error("Invalid packet or insufficient data for accumulated metrics")
                return None

            metrics = RadianAccumulatedMetrics()
            offset = 0

            # Parse accumulated energy values
            metrics.watt_hr = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.var_hr = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.q_hr = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0
            offset += 4

            metrics.va_hr = ti_float_to_ieee_single(packet.data[offset:offset+4]) or 0.0

            self.logger.debug(f"Parsed accumulated metrics: Wh={metrics.watt_hr:.2f}, "
                            f"VARh={metrics.var_hr:.2f}")

            return metrics

        except Exception as e:
            self.logger.error(f"Failed to parse accumulated metrics: {e}")
            return None

    def build_identify_command(self) -> str:
        """Build identify command.

        Returns:
            Hex string command (without checksum - backend adds it)
        """
        return "A6020000"

    def build_reset_metrics_command(self, reset_accum: bool = True,
                                   reset_instant: bool = False,
                                   reset_min: bool = False,
                                   reset_max: bool = False,
                                   reset_waveform: bool = False) -> str:
        """Build reset metrics command.

        Args:
            reset_accum: Reset accumulated metrics
            reset_instant: Reset instant metrics
            reset_min: Reset minimum values
            reset_max: Reset maximum values
            reset_waveform: Reset waveform buffers

        Returns:
            Hex string command
        """
        reset_code = 0
        if reset_waveform:
            reset_code |= 0x01
        if reset_instant:
            reset_code |= 0x02
        if reset_min:
            reset_code |= 0x04
        if reset_max:
            reset_code |= 0x08
        if reset_accum:
            reset_code |= 0x10

        return f"A6070001{reset_code:02X}"

    def build_start_accum_command(self, control_byte: int = 0x01,
                                 pulse_data: int = 0) -> str:
        """Build start accumulation command.

        Args:
            control_byte: Control mode (0x01 = normal gate start)
            pulse_data: Number of pulses (0 for query)

        Returns:
            Hex string command
        """
        pulse_msb = (pulse_data >> 8) & 0xFF
        pulse_lsb = pulse_data & 0xFF
        return f"A6080003{control_byte:02X}{pulse_msb:02X}{pulse_lsb:02X}"

    def build_start_timed_accum_command(self, time_seconds: float) -> str:
        """Build start timed accumulation test command.

        Args:
            time_seconds: Duration in seconds

        Returns:
            Hex string command
        """
        # Convert seconds to samples (20119.225 samples/sec for Rev 5+)
        samples_per_sec = 20119.225
        num_samples = int(time_seconds * samples_per_sec)

        byte0 = (num_samples >> 24) & 0xFF
        byte1 = (num_samples >> 16) & 0xFF
        byte2 = (num_samples >> 8) & 0xFF
        byte3 = num_samples & 0xFF

        return f"A60A0004{byte0:02X}{byte1:02X}{byte2:02X}{byte3:02X}"

    def build_stop_accum_command(self) -> str:
        """Build stop accumulation command.

        Returns:
            Hex string command
        """
        return "A6090000"
