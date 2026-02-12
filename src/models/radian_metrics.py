"""Data models for Radian power analyzer metrics and packets.

This module defines the data structures for Radian instant and accumulated
metrics, as well as packet structures for communication.

Based on original VB code: original/mRadian.vb
"""

from dataclasses import dataclass
from datetime import datetime
from typing import Optional


@dataclass
class RadianInstantMetrics:
    """Instant (real-time) metrics from Radian power analyzer.

    These values represent the current instantaneous measurements.
    """
    volt: float = 0.0          # RMS Voltage (V)
    amp: float = 0.0           # RMS Current (A)
    watt: float = 0.0          # Active Power (W)
    va: float = 0.0            # Apparent Power (VA)
    var: float = 0.0           # Reactive Power (VAR)
    frequency: float = 0.0     # Frequency (Hz)
    phase: float = 0.0         # Phase angle (degrees)
    power_factor: float = 0.0  # Power Factor (dimensionless, -1 to 1)
    timestamp: Optional[datetime] = None

    def to_dict(self) -> dict:
        """Convert to dictionary for JSON serialization."""
        return {
            'volt': self.volt,
            'amp': self.amp,
            'watt': self.watt,
            'va': self.va,
            'var': self.var,
            'frequency': self.frequency,
            'phase': self.phase,
            'power_factor': self.power_factor,
            'timestamp': self.timestamp.isoformat() if self.timestamp else None
        }


@dataclass
class RadianAccumulatedMetrics:
    """Accumulated (integrated) metrics from Radian power analyzer.

    These values represent energy/charge integrated over time.
    """
    watt_hr: float = 0.0       # Active Energy (Wh)
    var_hr: float = 0.0        # Reactive Energy (VARh)
    q_hr: float = 0.0          # Charge (Ah)
    va_hr: float = 0.0         # Apparent Energy (VAh)
    pulses: int = 0            # Pulse count
    test_duration: float = 0.0 # Test duration (seconds)
    timestamp: Optional[datetime] = None

    def to_dict(self) -> dict:
        """Convert to dictionary for JSON serialization."""
        return {
            'watt_hr': self.watt_hr,
            'var_hr': self.var_hr,
            'q_hr': self.q_hr,
            'va_hr': self.va_hr,
            'pulses': self.pulses,
            'test_duration': self.test_duration,
            'timestamp': self.timestamp.isoformat() if self.timestamp else None
        }


@dataclass
class RadianPacket:
    """Radian communication packet structure.

    Packet format:
    - Start byte: 0x02 (STX)
    - Packet type: 1 byte
    - Length: 2 bytes (big-endian)
    - Data: variable length
    - Checksum: 2 bytes (16-bit, big-endian)
    """
    start: int = 0x02          # Start byte (STX)
    packet_type: int = 0       # Packet type identifier
    length: int = 0            # Data length (bytes)
    data: bytes = b''          # Packet data payload
    checksum: int = 0          # 16-bit checksum

    def to_bytes(self) -> bytes:
        """Convert packet to byte array for transmission.

        Returns:
            Complete packet as bytes including start, type, length, data, checksum
        """
        # Build packet: start + type + length (2 bytes, big-endian) + data
        packet = bytes([self.start, self.packet_type])
        packet += self.length.to_bytes(2, byteorder='big')
        packet += self.data

        # Checksum will be calculated and appended by data_validation utilities
        return packet

    @staticmethod
    def from_bytes(data: bytes) -> Optional['RadianPacket']:
        """Parse packet from received bytes.

        Args:
            data: Raw bytes received from Radian

        Returns:
            RadianPacket if valid, None otherwise
        """
        if len(data) < 6:  # Minimum: start + type + length(2) + checksum(2)
            return None

        try:
            packet = RadianPacket()
            packet.start = data[0]
            packet.packet_type = data[1]
            packet.length = int.from_bytes(data[2:4], byteorder='big')

            # Extract data (excluding checksum at end)
            if len(data) >= 4 + packet.length + 2:
                packet.data = data[4:4 + packet.length]
                packet.checksum = int.from_bytes(data[4 + packet.length:4 + packet.length + 2],
                                                byteorder='big')

            return packet
        except (IndexError, ValueError):
            return None


# Radian packet type constants
class RadianPacketType:
    """Radian packet type identifiers."""
    IDENTIFY = 0x01            # Device identification request
    RESET_METRICS = 0x02       # Reset accumulated metrics
    START_ACCUM = 0x03         # Start accumulation test
    STOP_ACCUM = 0x04          # Stop accumulation test
    GET_INSTANT = 0x10         # Get instant metrics
    GET_ACCUMULATED = 0x11     # Get accumulated metrics
    RESPONSE_OK = 0x20         # Command acknowledged
    RESPONSE_ERROR = 0x21      # Command error
    RESPONSE_DATA = 0x22       # Data response


# Radian message state machine states
class RadianMessageState:
    """States for Radian message reception state machine."""
    IDLE = 0                   # Waiting for start byte
    WAITING_FOR_START = 1      # Waiting for STX (0x02)
    WAITING_FOR_LENGTH = 2     # Waiting for length bytes
    WAITING_FOR_DATA = 3       # Receiving data bytes
    WAITING_FOR_END = 4        # Waiting for checksum
    VALIDATING_DATA = 5        # Validating checksum
    DATA_RECEIVED = 6          # Complete valid packet received
