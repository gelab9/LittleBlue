#!/usr/bin/env python3

import smbus
import time
import threading
from GUI_configs.global_config import config  # still imported if needed elsewhere

# =====================
# MCP23008 Registers
# =====================
MCP23008_IODIR = 0x00
MCP23008_OLAT  = 0x0A

# =====================
# Set up I2C bus
# =====================
bus = smbus.SMBus(1)

# Phase relays & low current status
low_current_status = {'a': False, 'b': False, 'c': False}
mcp23008State = {'a': 0x00, 'b': 0x00, 'c': 0x00}

# =====================
# Fixed I2C Addresses (no config dependency)
# =====================
MCP23008_ADDR = {'a': 0x27, 'b': 0x25, 'c': 0x21}
SECONDARY_RELAY_ADDR = 0x3C

# Relay cutoff patterns
RELAY_PATTERNS = {
    'a': 0b00011111,
    'b': 0b11100111,
    'c': 0b11111001
}

# Current thresholds and corresponding bitmasks
CURRENT_RANGES = [
    (0.000,   0.300,  0b00000101),
    (0.301,   1.5,    0b00010111),
    (1.501,   5.0,    0b00011011),
    (5.01,    10.0,   0b00011001),
    (10.001,  20.0,   0b00101001),
    (20.001,  40.0,   0b00100101),
    (40.001,  600.0,  0b00000101),
]

# =====================
# I2C Setup: direction only
# =====================
def setup():
    for phase in ['a', 'b', 'c']:
        addr = MCP23008_ADDR[phase]
        bus.write_byte_data(addr, MCP23008_IODIR, 0x00)

def write_mcp23008(phase, state):
    addr = MCP23008_ADDR[phase]
    mcp23008State[phase] = state
    bus.write_byte_data(addr, MCP23008_OLAT, state)

def update_secondary_relay():
    value = 0xFF
    for phase in ['a', 'b', 'c']:
        if low_current_status[phase]:
            value &= RELAY_PATTERNS[phase]
    bus.write_byte(SECONDARY_RELAY_ADDR, value)
    print(f"[INFO] Sent 0b{value:08b} to secondary relay at 0x{SECONDARY_RELAY_ADDR:02X}")

def get_target_mask(current_val):
    for (low, high, mask) in CURRENT_RANGES:
        if low <= current_val <= high:
            return mask
    return None

# =====================
# Core function: always executes
# =====================
def set_current(phase, current_value):
    in_low_range = (0.000 <= current_value <= 0.300)
    target_mask = get_target_mask(current_value)

    if target_mask is None:
        print(f"[WARNING] Current value {current_value} A is out of defined range!")
        return

    write_mcp23008(phase, target_mask)
    low_current_status[phase] = in_low_range

    print(f"[RANGE SET] Phase {phase.upper()} → bitmask {target_mask:#010b} for {current_value:.3f} A")
    update_secondary_relay()

# =====================
# Optional CLI loop for testing
# =====================
def command_input_loop():
    while True:
        try:
            phase = input("Enter phase (a/b/c/all) or 'q' to quit: ").strip().lower()
            if phase == 'q':
                break
            if phase not in ['a', 'b', 'c', 'all']:
                print("Invalid phase. Please enter a, b, c, or all.")
                continue

            current_str = input("Enter current value (e.g. 0.18): ").strip()
            current_val = float(current_str)

            if phase == "all":
                for ph in ['a', 'b', 'c']:
                    set_current(ph, current_val)
            else:
                set_current(phase, current_val)

        except ValueError:
            print("Invalid input. Please enter a valid number for current.")
        except KeyboardInterrupt:
            print("\n[INFO] Exiting loop.")
            break

def main():
    setup()
    print("[INFO] MCP23008 setup complete. Starting relay control...\n")
    command_input_loop()

if __name__ == "__main__":
    main()
