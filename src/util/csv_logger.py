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

    # Radian metric column names (appended after channel temperatures)
    RADIAN_COLUMNS = ["Vrms", "Arms", "Hz", "Deg"]

    def initialize(self, channel_ids: list[int]):
        """Create the CSV file and write the header row."""
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        self._filepath = self.output_dir / f"daq_data_{timestamp}.csv"
        self._file = open(self._filepath, "w", newline="")
        self._writer = csv.writer(self._file, delimiter=self.delimiter)
        headers = ["Sample", "Timestamp"] + [str(ch) for ch in channel_ids] + self.RADIAN_COLUMNS
        self._writer.writerow(headers)
        self._file.flush()
        self._sample = 0

    def append_row(
        self,
        timestamp: str,
        channel_values: dict[int, float],
        radian_vrms: float | None = None,
        radian_arms: float | None = None,
        radian_hz: float | None = None,
        radian_deg: float | None = None,
    ):
        """Append one row of readings. channel_values maps channel_id -> value.

        Optional Radian metrics are written to the last four columns; empty string
        when not available so spreadsheet columns stay aligned.
        """
        if self._writer is None:
            return
        self._sample += 1

        def _fmt(v):
            return f"{v:.5f}" if v is not None else ""

        row = [self._sample, timestamp]
        for ch_id in sorted(channel_values.keys()):
            row.append(f"{channel_values[ch_id]:.4f}")
        row += [_fmt(radian_vrms), _fmt(radian_arms), _fmt(radian_hz), _fmt(radian_deg)]
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
