"""PAC Power UPC1/UPC3 device driver (Python frontend).

This driver communicates with the VB.NET backend via REST API.
The backend handles actual serial port communication with the PAC Power hardware.

Based on original VB code: original/PacPowerUCP1.vb
"""

import logging
from dataclasses import dataclass
from typing import Optional


@dataclass
class PacPowerMetrics:
    """Measurement data from PAC Power supply."""

    volt_ln: list[float] | None = None    # Line-to-neutral voltage (3 phases)
    current_rms: list[float] | None = None  # RMS current (3 phases)
    frequency: float = 0.0
    power: list[float] | None = None       # Active power (3 phases)
    kva: list[float] | None = None         # Apparent power (3 phases)
    pf: list[float] | None = None          # Power factor (3 phases)

    def __post_init__(self):
        if self.volt_ln is None:
            self.volt_ln = [0.0, 0.0, 0.0]
        if self.current_rms is None:
            self.current_rms = [0.0, 0.0, 0.0]
        if self.power is None:
            self.power = [0.0, 0.0, 0.0]
        if self.kva is None:
            self.kva = [0.0, 0.0, 0.0]
        if self.pf is None:
            self.pf = [0.0, 0.0, 0.0]

    def to_dict(self) -> dict:
        return {
            "volt_ln": self.volt_ln,
            "current_rms": self.current_rms,
            "frequency": self.frequency,
            "power": self.power,
            "kva": self.kva,
            "pf": self.pf,
        }


class PacPowerDevice:
    """PAC Power device frontend state tracker."""

    def __init__(self, logger: Optional[logging.Logger] = None):
        self.logger = logger or logging.getLogger(__name__)
        self._connected = False
        self.metrics = PacPowerMetrics()
        self.idn: str = ""
        self.voltage_setpoint: float = 0.0

    @property
    def is_connected(self) -> bool:
        return self._connected

    def set_connected(self, connected: bool):
        self._connected = connected

    @staticmethod
    def parse_float(value: str) -> float:
        """Safely parse a float from a SCPI response string."""
        try:
            return float(value.strip())
        except (ValueError, AttributeError):
            return 0.0

    def update_voltage_from_api(self, data: dict):
        """Update voltage metrics from /pac/measure/voltage response."""
        self.metrics.volt_ln = [
            self.parse_float(data.get("volt1", "0")),
            self.parse_float(data.get("volt2", "0")),
            self.parse_float(data.get("volt3", "0")),
        ]

    def update_current_from_api(self, data: dict):
        """Update current metrics from /pac/measure/current response."""
        self.metrics.current_rms = [
            self.parse_float(data.get("curr1", "0")),
            self.parse_float(data.get("curr2", "0")),
            self.parse_float(data.get("curr3", "0")),
        ]

    def update_frequency_from_api(self, data: dict):
        """Update frequency from /pac/measure/frequency response."""
        self.metrics.frequency = self.parse_float(data.get("frequency", "0"))

    def update_power_from_api(self, data: dict):
        """Update power metrics from /pac/measure/power response."""
        self.metrics.power = [
            self.parse_float(data.get("power1", "0")),
            self.parse_float(data.get("power2", "0")),
            self.parse_float(data.get("power3", "0")),
        ]
        self.metrics.kva = [
            self.parse_float(data.get("kva1", "0")),
            self.parse_float(data.get("kva2", "0")),
            self.parse_float(data.get("kva3", "0")),
        ]
        self.metrics.pf = [
            self.parse_float(data.get("pf1", "0")),
            self.parse_float(data.get("pf2", "0")),
            self.parse_float(data.get("pf3", "0")),
        ]
