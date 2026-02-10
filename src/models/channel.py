"""Channel configuration model for the DAQ 34970A."""

from __future__ import annotations
from dataclasses import dataclass, field


@dataclass
class ChannelConfig:
    """Configuration for a single DAQ measurement channel."""

    channel_number: int          # e.g. 101, 102, ... 320
    channel_name: str = ""       # user-defined label
    enabled: bool = False
    gain: float = 1.0            # calibration multiplier
    offset: float = 0.0          # calibration additive offset

    def apply_calibration(self, raw_value: float) -> float:
        """Apply gain and offset calibration to a raw reading."""
        return raw_value * self.gain + self.offset


def build_default_channels() -> list[ChannelConfig]:
    """Build the default 60-channel list (3 slots x 20 channels)."""
    channels = []
    for slot_start in (101, 201, 301):
        for i in range(20):
            ch_num = slot_start + i
            channels.append(ChannelConfig(
                channel_number=ch_num,
                channel_name=f"CH{ch_num}",
                enabled=(slot_start == 101),  # slot 100 enabled by default
            ))
    return channels
