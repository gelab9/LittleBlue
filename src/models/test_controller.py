"""Test controller managing test lifecycle and modes."""

from __future__ import annotations
from datetime import datetime


class TestController:
    """Controls test execution, mode tracking, and completion detection."""

    MODE_FREE = 0       # runs until manually stopped
    MODE_DURATION = 1   # runs for a specified duration
    MODE_READINGS = 2   # runs for a specified number of readings

    def __init__(self):
        self.mode: int = self.MODE_FREE
        self.is_running: bool = False
        self.reading_count: int = 0
        self.target_readings: int = 0
        self.target_duration_s: float = 0.0
        self.start_time: datetime | None = None

    def start(self):
        self.is_running = True
        self.reading_count = 0
        self.start_time = datetime.now()

    def stop(self):
        self.is_running = False

    def record_reading(self):
        self.reading_count += 1

    def should_stop(self) -> bool:
        if not self.is_running:
            return True
        if self.mode == self.MODE_FREE:
            return False
        elif self.mode == self.MODE_READINGS:
            return self.reading_count >= self.target_readings
        elif self.mode == self.MODE_DURATION:
            if self.start_time is None:
                return False
            elapsed = (datetime.now() - self.start_time).total_seconds()
            return elapsed >= self.target_duration_s
        return False

    def elapsed_seconds(self) -> float:
        if self.start_time is None:
            return 0.0
        return (datetime.now() - self.start_time).total_seconds()
