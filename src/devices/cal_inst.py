"""California Instruments CA501TAC device driver (Python frontend).

This driver communicates with the VB.NET backend via REST API.
The backend handles actual GPIB communication with the CA501TAC hardware.

Based on original VB code: original/GPIB_CA501TAC.vb
"""

import logging
from typing import Optional


class CalInstDevice:
    """Cal Inst CA501TAC frontend state tracker."""

    def __init__(self, logger: Optional[logging.Logger] = None):
        self.logger = logger or logging.getLogger(__name__)
        self._connected = False
        self.voltage_setpoint: float = 0.0
        self.board_id: int = 0
        self.primary_address: int = 1
        self.secondary_address: int = 0
        self.timeout_str: str = ""

    @property
    def is_connected(self) -> bool:
        return self._connected

    def set_connected(self, connected: bool):
        self._connected = connected

    def update_from_connect(self, data: dict):
        """Update state from /cal-inst/connect response."""
        self.board_id = data.get("boardId", self.board_id)
        self.primary_address = data.get("primaryAddress", self.primary_address)
        self.secondary_address = data.get("secondaryAddress", self.secondary_address)
        self.timeout_str = data.get("timeout", "")
        self._connected = data.get("connected", False)

    def update_voltage_setpoint(self, data: dict):
        """Update voltage setpoint from /cal-inst/set-voltage response."""
        try:
            self.voltage_setpoint = float(data.get("setPoint", "0.0"))
        except (ValueError, TypeError):
            self.voltage_setpoint = 0.0
