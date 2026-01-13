# =============================================================================
# CONFIDENTIAL - DO NOT DISTRIBUTE
# -----------------------------------------------------------------------------
# Title      : Big Blue CT Routing Code
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


from gpiozero import DigitalInputDevice
from smbus2 import SMBus
from threading import Thread
import time


# =====================
# GPIO Monitoring Setup
# =====================
input1 = DigitalInputDevice(5)
input2 = DigitalInputDevice(6)


"""
The command 0xF7 activates the 0-5 amp path
0xFB turns that path off
Then, the command 0xF6 turns on the 5-320 amp path
0xFA turns that path off
If both paths are off the 320+ path is activated
"""


def monitor_gpio():
    print("Monitoring GPIO 5 and 6. Press Ctrl+C to stop.")
    try:
        while True:
            print(f"GPIO 5: {'HIGH' if input1.value else 'LOW'} | GPIO 6: {'HIGH' if input2.value else 'LOW'}")
            time.sleep(3)
    except KeyboardInterrupt:
        print("\nStopped GPIO monitoring.")


# =====================
# I2C Control Setup
# =====================
I2C_BUS = 1
DEVICE_ADDRESS = 0x3A
ALL_RELAYS_OFF = 0xFF

#
# 0xF6 = 5.1-320A on GPIO 5 High
# 0xFA = 5.1-320A off GPIO 5 LOW
# 0xF7 = 0-5A on GPIO 6 High
# 0xFB = 5.1-320A off GPIO 6 LOW
#

def i2c_command_loop():
    with SMBus(I2C_BUS) as bus:
        print(f"Ready to send hex commands to device at 0x{DEVICE_ADDRESS:02X}.")
        print("Type a hex byte (e.g., 0xFA) to control relays. Type 'exit' to quit.")

        while True:
            user_input = input("Enter hex byte to send: ").strip()
            if user_input.lower() in ['exit', 'quit']:
                print("Exiting.")
                break

            try:
                value = int(user_input, 16)
                if not 0 <= value <= 0xFF:
                    raise ValueError("Value out of range (0x00 - 0xFF)")

                bus.write_byte(DEVICE_ADDRESS, value)
                print(f"Sent {user_input} to device at 0x{DEVICE_ADDRESS:02X}")

                time.sleep(0.6)

                bus.write_byte(DEVICE_ADDRESS, ALL_RELAYS_OFF)
                print(f"Sent 0xFF to turn all relays OFF after 1 second")

            except ValueError as ve:
                print(f"Invalid input: {ve}")
            except Exception as e:
                print(f"Error communicating with 0x{DEVICE_ADDRESS:02X}: {e}")


# =====================
# Main Entry Point
# =====================
if __name__ == "__main__":
    gpio_thread = Thread(target=monitor_gpio, daemon=True)
    gpio_thread.start()

    i2c_command_loop()
    
    


