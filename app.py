"""Smart Meter GUI Migration - Main Application Entry Point"""

import sys
from pathlib import Path

# Force matplotlib to use the Qt backend before any other matplotlib import.
# Without this, macOS activates its native backend first, then backend_qtagg
# tries to initialize a second one — causing a segmentation fault.
import matplotlib
matplotlib.use("QtAgg")

# Add src to path
sys.path.insert(0, str(Path(__file__).parent))

from PyQt6.QtWidgets import QApplication
from src.main_window_logic import MainWindow


def main():
    """Launch the application."""
    app = QApplication(sys.argv)
    qss_path = Path(__file__).parent / "style.qss"
    with open(qss_path, "r") as f:
        app.setStyleSheet(f.read())
    
    window = MainWindow()
    window.setWindowTitle("Temperature Rise")
    window.show()
    sys.exit(app.exec())


if __name__ == "__main__":
    main()