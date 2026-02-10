"""CSV data logger for DAQ readings."""

from __future__ import annotations
import csv
from datetime import datetime
from pathlib import Path


class CsvLogger:
    """Logs channel readings to a CSV file."""

    def __init__(self, output_dir: Path, delimiter: str = ","):
        self.output_dir = output_dir
        self.delimiter = delimiter
        self._file = None
        self._writer = None
        self._filepath: Path | None = None

    def initialize(self, channel_ids: list[int]):
        """Create the CSV file and write the header row."""
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        self._filepath = self.output_dir / f"daq_data_{timestamp}.csv"
        self._file = open(self._filepath, "w", newline="")
        self._writer = csv.writer(self._file, delimiter=self.delimiter)
        headers = ["Timestamp"] + [str(ch) for ch in channel_ids]
        self._writer.writerow(headers)
        self._file.flush()

    def append_row(self, timestamp: str, channel_values: dict[int, float]):
        """Append one row of readings. channel_values maps channel_id -> value."""
        if self._writer is None:
            return
        row = [timestamp]
        for ch_id in sorted(channel_values.keys()):
            row.append(f"{channel_values[ch_id]:.4f}")
        self._writer.writerow(row)
        self._file.flush()

    def close(self):
        if self._file:
            self._file.close()
            self._file = None
            self._writer = None

    @property
    def filepath(self) -> Path | None:
        return self._filepath
