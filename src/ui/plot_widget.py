"""Matplotlib-based chart widget for PyQt6.

Provides real-time temperature and current plotting with zoom, save, and load.
Based on original VB DataChart/CurrentChart implementation.
"""

import csv
import logging
from datetime import datetime, timedelta
from pathlib import Path
from typing import Optional

from PyQt6.QtWidgets import (
    QWidget, QVBoxLayout, QHBoxLayout, QPushButton,
    QRadioButton, QComboBox, QLabel, QFileDialog, QGroupBox,
    QButtonGroup, QSizePolicy,
)
from PyQt6.QtCore import Qt

from matplotlib.backends.backend_qtagg import FigureCanvasQTAgg as FigureCanvas
from matplotlib.backends.backend_qtagg import NavigationToolbar2QT as NavigationToolbar
from matplotlib.figure import Figure
import matplotlib.dates as mdates


class LiveChart:
    """Manages one matplotlib axes with multiple named series."""

    def __init__(self, ax, title: str, xlabel: str, ylabel: str):
        self.ax = ax
        self.title = title
        self.xlabel = xlabel
        self.ylabel = ylabel
        self._series: dict[str, tuple[list, list]] = {}  # name -> (x_data, y_data)
        self._lines: dict[str, object] = {}  # name -> Line2D
        self._line_width = 2
        self._visible = True

        self.ax.set_title(title, fontsize=12, fontweight="bold")
        self.ax.set_xlabel(xlabel, fontsize=10)
        self.ax.set_ylabel(ylabel, fontsize=10)
        self.ax.grid(True, alpha=0.3)

    def add_series(self, name: str):
        """Add a new named data series."""
        if name in self._series:
            return
        self._series[name] = ([], [])
        line, = self.ax.plot([], [], label=name, linewidth=self._line_width,
                             marker="o", markersize=2)
        self._lines[name] = line

    def clear_series(self):
        """Remove all series."""
        for line in self._lines.values():
            line.remove()
        self._series.clear()
        self._lines.clear()
        self.ax.legend()

    def add_point(self, series_name: str, x_val, y_val: float):
        """Add a data point to a series."""
        if series_name not in self._series:
            self.add_series(series_name)
        xd, yd = self._series[series_name]
        xd.append(x_val)
        yd.append(y_val)
        self._lines[series_name].set_data(xd, yd)

    def refresh(self):
        """Rescale axes and update legend."""
        self.ax.relim()
        self.ax.autoscale_view()
        if self._lines:
            self.ax.legend(fontsize=7, loc="upper left", ncols=2)

    def reset_zoom(self, axis: str = "both"):
        """Reset zoom on specified axis."""
        self.ax.relim()
        self.ax.autoscale_view()

    def set_line_width(self, width: int):
        """Set line width for all series."""
        self._line_width = width
        for line in self._lines.values():
            line.set_linewidth(width)

    def set_visible(self, visible: bool):
        """Toggle visibility of this chart's axes."""
        self._visible = visible
        self.ax.set_visible(visible)


class PlotWidget(QWidget):
    """Embeddable chart widget with controls for DAQ Plot tab."""

    def __init__(self, logger: Optional[logging.Logger] = None, parent=None):
        super().__init__(parent)
        self.logger = logger or logging.getLogger(__name__)
        self._test_start_time: datetime | None = None

        # Create matplotlib figure with two subplots (stacked)
        self.figure = Figure(figsize=(10, 6), tight_layout=True)
        self.canvas = FigureCanvas(self.figure)
        self.canvas.setSizePolicy(QSizePolicy.Policy.Expanding, QSizePolicy.Policy.Expanding)

        # Two axes: temperature (top) and current (bottom)
        self.ax_temp = self.figure.add_subplot(111)
        self.temp_chart = LiveChart(
            self.ax_temp,
            title="Temperature vs. Time",
            xlabel="Duration / HH:MM:SS",
            ylabel="Temperature / \u00b0C",
        )

        # Current chart shares the same figure but is toggled
        self.ax_current = self.figure.add_subplot(111)
        self.current_chart = LiveChart(
            self.ax_current,
            title="Current vs. Time",
            xlabel="Duration / HH:MM:SS",
            ylabel="Current / A",
        )
        # Start with current chart hidden
        self.ax_current.set_visible(False)
        self.current_chart.set_visible(False)

        self._active_chart = "temperature"

        # Build UI layout
        self._build_ui()

    def _build_ui(self):
        main_layout = QVBoxLayout(self)

        # Controls bar
        controls = QHBoxLayout()

        # Chart toggle radio buttons
        self.rb_temperature = QRadioButton("Temperature")
        self.rb_temperature.setChecked(True)
        self.rb_temperature.toggled.connect(self._on_chart_toggle)
        self.rb_current = QRadioButton("Current")
        chart_group = QButtonGroup(self)
        chart_group.addButton(self.rb_temperature)
        chart_group.addButton(self.rb_current)
        controls.addWidget(QLabel("Plot:"))
        controls.addWidget(self.rb_temperature)
        controls.addWidget(self.rb_current)

        controls.addSpacing(20)

        # Zoom reset buttons
        btn_reset_x = QPushButton("Reset X")
        btn_reset_x.setFixedWidth(70)
        btn_reset_x.clicked.connect(lambda: self._reset_zoom("x"))
        btn_reset_y = QPushButton("Reset Y")
        btn_reset_y.setFixedWidth(70)
        btn_reset_y.clicked.connect(lambda: self._reset_zoom("y"))
        btn_reset_xy = QPushButton("Reset XY")
        btn_reset_xy.setFixedWidth(70)
        btn_reset_xy.clicked.connect(lambda: self._reset_zoom("both"))
        controls.addWidget(btn_reset_x)
        controls.addWidget(btn_reset_y)
        controls.addWidget(btn_reset_xy)

        controls.addSpacing(20)

        # Line width
        controls.addWidget(QLabel("Line Width:"))
        self.cb_line_width = QComboBox()
        self.cb_line_width.addItems(["1", "2", "3"])
        self.cb_line_width.setCurrentIndex(1)  # default 2
        self.cb_line_width.setFixedWidth(50)
        self.cb_line_width.currentTextChanged.connect(self._on_line_width_changed)
        controls.addWidget(self.cb_line_width)

        controls.addSpacing(20)

        # Save / Load
        btn_save = QPushButton("Save Plot")
        btn_save.setFixedWidth(80)
        btn_save.clicked.connect(self._on_save_plot)
        btn_load = QPushButton("Load Plot")
        btn_load.setFixedWidth(80)
        btn_load.clicked.connect(self._on_load_plot)
        controls.addWidget(btn_save)
        controls.addWidget(btn_load)

        controls.addStretch()

        main_layout.addLayout(controls)

        # Navigation toolbar (matplotlib zoom/pan built-in)
        self.toolbar = NavigationToolbar(self.canvas, self)
        main_layout.addWidget(self.toolbar)

        # Canvas
        main_layout.addWidget(self.canvas)

    # ----------------------------------------------------------------
    # Public API
    # ----------------------------------------------------------------

    def start_test(self, channel_names: list[str]):
        """Initialize chart for a new test run."""
        self._test_start_time = datetime.now()

        # Setup temperature chart
        self.temp_chart.clear_series()
        for name in channel_names:
            self.temp_chart.add_series(name)

        # Setup current chart with single series
        self.current_chart.clear_series()
        self.current_chart.add_series("Current")

        self.canvas.draw()

    def add_temperature_points(self, channel_values: dict[str, float]):
        """Add temperature data points for all channels at current time."""
        if self._test_start_time is None:
            return

        elapsed = (datetime.now() - self._test_start_time).total_seconds()
        duration_str = str(timedelta(seconds=int(elapsed)))

        for name, value in channel_values.items():
            self.temp_chart.add_point(name, elapsed, value)

        if self._active_chart == "temperature":
            self.temp_chart.refresh()
            self.canvas.draw_idle()

    def add_current_point(self, current_value: float):
        """Add a current data point at current time."""
        if self._test_start_time is None:
            return

        elapsed = (datetime.now() - self._test_start_time).total_seconds()
        self.current_chart.add_point("Current", elapsed, current_value)

        if self._active_chart == "current":
            self.current_chart.refresh()
            self.canvas.draw_idle()

    def stop_test(self):
        """Finalize chart after test ends."""
        self.temp_chart.refresh()
        self.current_chart.refresh()
        self.canvas.draw()

    # ----------------------------------------------------------------
    # Slot handlers
    # ----------------------------------------------------------------

    def _on_chart_toggle(self, checked: bool):
        if self.rb_temperature.isChecked():
            self._active_chart = "temperature"
            self.ax_temp.set_visible(True)
            self.ax_current.set_visible(False)
        else:
            self._active_chart = "current"
            self.ax_temp.set_visible(False)
            self.ax_current.set_visible(True)
        self.canvas.draw()

    def _reset_zoom(self, axis: str):
        chart = self.temp_chart if self._active_chart == "temperature" else self.current_chart
        chart.reset_zoom(axis)
        self.canvas.draw()

    def _on_line_width_changed(self, text: str):
        try:
            width = int(text)
        except ValueError:
            return
        self.temp_chart.set_line_width(width)
        self.current_chart.set_line_width(width)
        self.canvas.draw()

    def _on_save_plot(self):
        filepath, _ = QFileDialog.getSaveFileName(
            self, "Save Plot", "", "PNG Files (*.png);;All Files (*.*)"
        )
        if filepath:
            self.figure.savefig(filepath, dpi=150)
            self.logger.info(f"Plot saved to {filepath}")

    def _on_load_plot(self):
        filepath, _ = QFileDialog.getOpenFileName(
            self, "Load Plot Data", "", "CSV Files (*.csv);;All Files (*.*)"
        )
        if not filepath:
            return

        try:
            self._load_csv_to_chart(filepath)
            self.logger.info(f"Plot loaded from {filepath}")
        except Exception as e:
            self.logger.error(f"Failed to load plot: {e}")

    def _load_csv_to_chart(self, filepath: str):
        """Load CSV data into the temperature chart."""
        with open(filepath, "r", newline="") as f:
            reader = csv.reader(f)
            headers = next(reader)

        if len(headers) < 2:
            return

        # First column is timestamp, rest are data series
        self.temp_chart.clear_series()
        series_names = headers[1:]
        for name in series_names:
            self.temp_chart.add_series(name)

        with open(filepath, "r", newline="") as f:
            reader = csv.reader(f)
            next(reader)  # skip header
            for row_idx, row in enumerate(reader):
                if len(row) < 2:
                    continue
                x_val = row_idx  # use row index as x
                for col_idx, name in enumerate(series_names):
                    if col_idx + 1 < len(row):
                        try:
                            y_val = float(row[col_idx + 1])
                            self.temp_chart.add_point(name, x_val, y_val)
                        except ValueError:
                            pass

        self.temp_chart.refresh()
        self.rb_temperature.setChecked(True)
        self._active_chart = "temperature"
        self.ax_temp.set_visible(True)
        self.ax_current.set_visible(False)
        self.canvas.draw()
