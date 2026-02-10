from src.main_window_logic import MainWindow
from PyQt6.QtWidgets import QApplication
import sys

app = QApplication(sys.argv)
w = MainWindow()
w.show()
sys.exit(app.exec())
