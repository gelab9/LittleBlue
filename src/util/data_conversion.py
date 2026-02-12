"""Data conversion utilities for handling various byte formats.

This module provides utilities for converting between different data formats,
particularly for TI floating-point format used by Radian power analyzers.

Ported from original VB code: original/mDataManipulationRD.vb
Author: Frank Boudreau 2012 Landis+Gyr (original VB)
Python port: 2026
"""

from typing import Optional


def byte_array_to_ascii_string(data: bytes) -> str:
    """Convert a byte array to ASCII string.

    Args:
        data: Input byte array

    Returns:
        ASCII string representation of the bytes

    Example:
        >>> byte_array_to_ascii_string(b'\x30\x35\x39')
        '059'
    """
    try:
        return data.decode('ascii')
    except (UnicodeDecodeError, AttributeError):
        return ""


def ti_float_to_ieee_single(data: bytes) -> Optional[float]:
    """Convert TI floating-point format (4 bytes) to IEEE Single precision.

    TI floating-point format:
    - Byte 0: Exponent (signed, 2's complement)
    - Bytes 1-3: Mantissa (23 bits) with sign bit in bit 7 of byte 1

    This conversion is critical for Radian power analyzer data parsing.

    Args:
        data: 4-byte array in TI floating-point format

    Returns:
        IEEE Single precision float, or None if input is invalid

    Note:
        Based on Radian Research Application Note
        Ported from original VB function s_TI_to_VB_Single()
    """
    # Validate input length
    if len(data) < 4:
        return None

    # Boundary condition 1: Zero or too small for IEEE
    if data[0] in (0x80, 0x81):
        # 2's-complement maps to zero
        # 2's-complement too small for IEEE
        return 0.0

    # Boundary condition 2: Maximum value
    if (data[0] == 0x7F and data[1] == 0x7F and
        data[2] == 0xFF and data[3] == 0xFF):
        return 999999999.0

    # Extract mantissa (23 bits from bytes 1-3)
    # Byte 1: bits 6-0, Byte 2: bits 7-0, Byte 3: bits 7-0
    mantissa = ((data[1] & 0x7F) << 16) | (data[2] << 8) | data[3]

    # Extract sign bit (bit 7 of byte 1)
    is_negative = (data[1] & 0x80) != 0

    # Boundary condition 3: Minimum value
    if data[0] == 0x7F and is_negative and mantissa == 0:
        return -9999999999.0

    # Extract exponent (byte 0)
    exponent_byte = data[0]

    # Unwrap exponent (2's complement)
    if exponent_byte > 127:
        # Negative exponent
        exponent = -((256 - exponent_byte) & 0x7F)
    else:
        # Positive exponent
        exponent = exponent_byte

    # Process mantissa based on sign
    if not is_negative:
        # Positive mantissa
        mantissa = mantissa & 0x7FFFFFFF
    elif mantissa != 0:
        # Negative non-zero mantissa (2's complement)
        mantissa = (~mantissa + 1) & 0x7FFFFF
    else:
        # Mantissa is zero
        mantissa = 0

    # Calculate IEEE single precision value
    # Formula: (1 + mantissa/2^23) * 2^exponent
    value = (1.0 + float(mantissa) / (2 ** 23)) * (2 ** exponent)

    # Apply sign
    if is_negative:
        value *= -1.0

    return value


def hex_string_to_byte_array(hex_string: str) -> Optional[bytes]:
    """Convert hex string to byte array.

    Args:
        hex_string: Hex string (e.g., "A603FF")

    Returns:
        Byte array, or None if conversion fails

    Example:
        >>> hex_string_to_byte_array("A603FF")
        b'\\xa6\\x03\\xff'
    """
    try:
        # Remove any whitespace or separators
        hex_string = hex_string.replace(" ", "").replace("-", "").replace(":", "")

        # Convert pairs of hex characters to bytes
        byte_array = bytes.fromhex(hex_string)
        return byte_array
    except (ValueError, AttributeError):
        return None


def byte_array_to_readable_hex_string(data: bytes) -> str:
    """Convert bytes to readable hex string.

    Args:
        data: Byte array

    Returns:
        Hex string representation (e.g., "303539" for bytes 0x30, 0x35, 0x39)
        Empty string on error

    Example:
        >>> byte_array_to_readable_hex_string(b'\\x30\\x35\\x39')
        '303539'
    """
    try:
        # Convert each byte to 2-character hex string (with leading zero if needed)
        hex_string = ''.join(f'{byte:02X}' for byte in data)
        return hex_string
    except (TypeError, AttributeError):
        return ""


def find_min_max(data: list[float]) -> tuple[float, float]:
    """Find minimum and maximum values in a list.

    Args:
        data: List of float values

    Returns:
        Tuple of (min, max) values

    Raises:
        ValueError: If data list has fewer than 2 points

    Note:
        This is a Python-native replacement for the VB v_Bounds function.
        Python's built-in min() and max() are more efficient.
    """
    if len(data) < 2:
        raise ValueError("Data array must have at least two points")

    return min(data), max(data)
