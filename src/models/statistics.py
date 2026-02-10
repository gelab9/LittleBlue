"""Per-channel statistics tracking for DAQ readings."""

from __future__ import annotations


class ChannelStats:
    """Track running statistics for a single DAQ channel."""

    def __init__(self, channel_id: int):
        self.channel_id = channel_id
        self.current: float = 0.0
        self.max: float = float("-inf")
        self.min: float = float("inf")
        self._sum: float = 0.0
        self.count: int = 0

    def update(self, value: float):
        self.current = value
        if value > self.max:
            self.max = value
        if value < self.min:
            self.min = value
        self._sum += value
        self.count += 1

    @property
    def average(self) -> float:
        return self._sum / self.count if self.count > 0 else 0.0

    def reset(self):
        self.current = 0.0
        self.max = float("-inf")
        self.min = float("inf")
        self._sum = 0.0
        self.count = 0


class StatisticsTracker:
    """Track statistics across all active DAQ channels."""

    def __init__(self):
        self.channels: dict[int, ChannelStats] = {}
        self.ambient_channel: int | None = None

    def update_channel(self, channel_id: int, value: float):
        if channel_id not in self.channels:
            self.channels[channel_id] = ChannelStats(channel_id)
        self.channels[channel_id].update(value)

    def get_temp_rise(self, channel_id: int) -> float:
        """Temperature rise = current reading - ambient channel reading."""
        if self.ambient_channel is None:
            return 0.0
        ambient = self.channels.get(self.ambient_channel)
        ch = self.channels.get(channel_id)
        if ambient is None or ch is None:
            return 0.0
        return ch.current - ambient.current

    def get_channel(self, channel_id: int) -> ChannelStats | None:
        return self.channels.get(channel_id)

    def reset(self):
        self.channels.clear()

    def get_summary(self) -> dict:
        summary = {
            "total_readings": max((c.count for c in self.channels.values()), default=0),
            "channels": {},
        }
        for ch_id, stats in self.channels.items():
            summary["channels"][ch_id] = {
                "current": stats.current,
                "max": stats.max,
                "min": stats.min,
                "average": stats.average,
                "count": stats.count,
            }
        return summary
