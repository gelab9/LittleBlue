"""Channel comparison with threshold monitoring.

Compares temperature readings between paired channels and detects
when the absolute difference exceeds a user-defined threshold.

Based on original VB Compare structure in m34970A.vb.
"""

import logging
from dataclasses import dataclass, field
from typing import Optional


@dataclass
class ComparePair:
    """A single channel comparison pair."""
    channel_a: int = 0       # First channel number (e.g., 101)
    channel_b: int = 0       # Second channel number (e.g., 102)
    threshold: float = 0.0   # Threshold in °C
    enabled: bool = False    # Whether threshold checking is active
    difference: float = 0.0  # Current absolute difference
    value_a: float = 0.0     # Latest reading from channel A
    value_b: float = 0.0     # Latest reading from channel B
    warning_sent: bool = True  # True = no warning pending, False = warning should fire

    @property
    def is_configured(self) -> bool:
        """True if both channels are selected."""
        return self.channel_a > 0 and self.channel_b > 0

    @property
    def a_is_hotter(self) -> bool:
        return self.value_a > self.value_b

    @property
    def b_is_hotter(self) -> bool:
        return self.value_b > self.value_a

    @property
    def threshold_exceeded(self) -> bool:
        return self.enabled and self.threshold > 0 and self.difference >= self.threshold


class ComparisonTracker:
    """Manages 5 channel comparison pairs with threshold monitoring."""

    NUM_PAIRS = 5

    def __init__(self, logger: Optional[logging.Logger] = None):
        self.logger = logger or logging.getLogger(__name__)
        self.pairs: list[ComparePair] = [ComparePair() for _ in range(self.NUM_PAIRS)]

    def configure_pair(self, index: int, channel_a: int, channel_b: int):
        """Set channels for a comparison pair."""
        if 0 <= index < self.NUM_PAIRS:
            self.pairs[index].channel_a = channel_a
            self.pairs[index].channel_b = channel_b

    def set_threshold(self, index: int, threshold: float, enabled: bool = True):
        """Set threshold value and enable/disable for a pair."""
        if 0 <= index < self.NUM_PAIRS:
            self.pairs[index].threshold = threshold
            self.pairs[index].enabled = enabled

    def update_readings(self, channel_values: dict[int, float]) -> list[int]:
        """Update all pairs with new channel readings.

        Returns list of pair indices where threshold was newly exceeded.
        """
        exceeded_indices = []

        for i, pair in enumerate(self.pairs):
            if not pair.is_configured:
                continue

            if pair.channel_a in channel_values:
                pair.value_a = channel_values[pair.channel_a]
            if pair.channel_b in channel_values:
                pair.value_b = channel_values[pair.channel_b]

            pair.difference = round(abs(pair.value_a - pair.value_b), 3)

            if pair.threshold_exceeded and pair.warning_sent:
                pair.warning_sent = False
                exceeded_indices.append(i)
                self.logger.warning(
                    f"Threshold exceeded on pair {i}: "
                    f"CH{pair.channel_a}={pair.value_a:.3f} vs "
                    f"CH{pair.channel_b}={pair.value_b:.3f}, "
                    f"diff={pair.difference:.3f} >= threshold={pair.threshold}"
                )

            if not pair.threshold_exceeded and not pair.warning_sent:
                pair.warning_sent = True

        return exceeded_indices

    def reset(self):
        """Reset all pairs."""
        for pair in self.pairs:
            pair.difference = 0.0
            pair.value_a = 0.0
            pair.value_b = 0.0
            pair.warning_sent = True
