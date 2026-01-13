# =============================================================================
# CONFIDENTIAL - DO NOT DISTRIBUTE
# -----------------------------------------------------------------------------
# Title      : Big Blue Serial Connections Manager
# Author     : Amanda Kogut, Ben Eckert, Brendan Duffy, Connor Hise, Fred Holt, Mary Newell, Maxwell Thompson, Nathan Garrett, Zach Burton
# Created    : 2024
# Description: Internal use only. This software is proprietary and confidential.
#
# WARNING:
# This script contains confidential and proprietary information of [Landis+Gyr].
# Unauthorized use, reproduction, or distribution of this software, or any
# portion of it, may result in civil and/or criminal penalties.
#
# © [2025] [Landis+Gyr]. All rights reserved.
# =============================================================================

# serial_manager.py

from PyQt6.QtSerialPort import QSerialPort

class SerialPortManager:
    def __init__(self):
        self.connections = {}

    def open_port(self, key, port_name, baudrate=115200):
        # Close existing connection if it exists but points to a different port
        if key in self.connections:
            existing = self.connections[key]
            if existing.portName() != port_name or not existing.isOpen():
                existing.close()

        sp = QSerialPort()
        sp.setPortName(port_name)
        sp.setBaudRate(baudrate)

        if sp.open(QSerialPort.OpenModeFlag.ReadWrite):
            self.connections[key] = sp
            print(f"✅ Opened port {port_name} for {key}")
            return sp
        else:
            error_code = sp.error()
            print(f"❌ Failed to open port for {key} on {port_name} — QSerialPort error code: {error_code}")
            return None

    def close_port(self, key):
        if key in self.connections and self.connections[key].isOpen():
            self.connections[key].close()
            print(f"🔌 Closed port for {key}")

    def close_all(self):
        for key, conn in self.connections.items():
            if conn.isOpen():
                conn.close()
                print(f"🔌 Closed port for {key}")
        self.connections.clear()

# Singleton instance used across the application
serial_manager = SerialPortManager()
