# =============================================================================
# CONFIDENTIAL - DO NOT DISTRIBUTE
# -----------------------------------------------------------------------------
# Title      : Big Blue Port Scanner
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


import serial
import serial.tools.list_ports
import time


def scan_serial_ports():
    dict_device = {
        "2345A01376": "Vc",
        "2345A01377": "Vb",
        "2345A02097": "Ia",
        "2345A01375": "Ib",
        "2345A02096": "Va",
        "2345A01374": "Ic"
    }

    name_list = []
    device_list = []
    arduino_assigned = False

    ports = serial.tools.list_ports.comports()

    for port in ports:
        print(f"Checking port {port.device}...")
        try:
            ser = serial.Serial(port.device, baudrate=115200, timeout=0.1)
            ser.reset_input_buffer()

            # === Check for SCPI response ===
            ser.write(b'*IDN?\n')
            time.sleep(0.001)
            response = ser.readline().decode('utf-8', errors='ignore').strip()

            if response:
                print(f"✅ Response from {port.device}: {response}")
                parts = [x.strip() for x in response.split(',')]
                if len(parts) >= 3:
                    serial_number = parts[2]
                    if serial_number in dict_device:
                        device_list.append(dict_device[serial_number])
                        name_list.append(port.device)
                        ser.close()  # ✅ Close right after successful match
                        continue
                    else:
                        print(f"⚠️ Unknown serial number: {serial_number}")
                else:
                    print(f"⚠️ Unexpected response format: {response}")
            else:
                print(f"⚠️ No response to *IDN? from {port.device}")

            # === Check for Arduino ===
            ser.reset_input_buffer()
            ser.write(b'info\n')
            time.sleep(0.001)
            response = ser.readline().decode('utf-8', errors='ignore').strip()

            if response == "Landis+Gyr Big Blue Project":
                print(f"🧠 Arduino detected on {port.device}")
                if not arduino_assigned:
                    device_list.append('arduino')
                    name_list.append(port.device)
                    arduino_assigned = True
                else:
                    print("⚠️ Arduino already assigned, skipping duplicate.")
            else:
                print(f"⚠️ Unexpected response to 'info': {response}")

            ser.close()  # ✅ Close port after trying both queries

        except (serial.SerialException, OSError) as e:
            print(f"❌ Could not open {port.device}: {e}")
        except Exception as e:
            print(f"⚠️ Unexpected error: {e}")
        time.sleep(0.05)

    final_dict = dict(zip(device_list, name_list))
    print("🔍 Final port assignments:", final_dict)
    return final_dict


    
if __name__ == "__main__":
    scan_serial_ports()


