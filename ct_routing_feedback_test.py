# =============================================================================
# CONFIDENTIAL - DO NOT DISTRIBUTE
# -----------------------------------------------------------------------------
# Title      : Big Blue CT Routing Feedback Loop Test
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
from time import sleep


# Set up GPIO 5 and 6 as inputs
input1 = DigitalInputDevice(5)
input2 = DigitalInputDevice(6)


print("Monitoring GPIO 5 and 6. Press Ctrl+C to stop.")


try:
    while True:
        print(f"GPIO 5: {'HIGH' if input1.value else 'LOW'} | GPIO 6: {'HIGH' if input2.value else 'LOW'}")
        sleep(0.25)


except KeyboardInterrupt:
    print("\nExiting...")


