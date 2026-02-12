"""Data validation utilities for checksums and CRC calculations.

This module provides utilities for calculating and validating checksums
and CRC values for packet-based communication protocols.

Ported from original VB code: original/mDataValidation.vb
Author: Frank Boudreau 2012 Landis+Gyr (original VB)
Python port: 2026
"""

import struct


def calc_checksum_8bit(data: bytes) -> int:
    """Calculate 8-bit circular checksum.

    Computes a simple additive checksum bounded by 0-255 using circular
    wrapping (modulo 256).

    Args:
        data: Byte array of data

    Returns:
        Checksum value (0-255), or -1 on error

    Example:
        >>> calc_checksum_8bit(b'\\x01\\x02\\x03')
        6
    """
    try:
        checksum = 0
        for byte in data:
            checksum += byte
            # Wrap if greater than 255 (circular checksum)
            if checksum > 255:
                checksum -= 256

        return checksum
    except (TypeError, AttributeError):
        return -1


def calc_crc16(data: bytes) -> tuple[bool, int]:
    """Calculate 16-bit CRC using IBM polynomial.

    This is the IBM version CRC-16, adopted by USB, ANSI, and other standards.
    Uses polynomial 0x8005 (x^16 + x^15 + x^2 + 1).

    Args:
        data: Input byte array

    Returns:
        Tuple of (success: bool, crc16_value: int)
        - success: True if calculation succeeded, False otherwise
        - crc16_value: The calculated CRC-16 value (0-65535)

    Example:
        >>> success, crc = calc_crc16(b'\\x01\\x02\\x03')
        >>> success
        True
    """
    try:
        # CRC-16 polynomial (IBM version)
        crc16_poly = 0x8005
        crc_remainder = 0x0
        crc16_final_xor = 0x0

        for byte in data:
            temp_int = byte * 256  # Shift byte to upper 8 bits

            for _ in range(8):
                # Check if MSB is 1 after XOR
                if ((temp_int ^ crc_remainder) >> 15) & 1:
                    crc_remainder = ((crc_remainder << 1) ^ crc16_poly) & 0xFFFF
                else:
                    crc_remainder = (crc_remainder << 1) & 0xFFFF

                temp_int = (temp_int << 1) & 0xFFFF

        # Final XOR
        crc16_value = (crc_remainder ^ crc16_final_xor) & 0xFFFF

        return True, crc16_value
    except (TypeError, AttributeError):
        return False, 0


def calc_checksum_16bit(data: bytes) -> int:
    """Calculate 16-bit circular checksum.

    Computes a simple additive checksum bounded by 0-65535 using circular
    wrapping (modulo 65536).

    Args:
        data: Byte array of data

    Returns:
        Checksum value (0-65535), or -1 on error

    Example:
        >>> calc_checksum_16bit(b'\\x01\\x02\\x03')
        6
    """
    try:
        checksum = 0
        for byte in data:
            checksum += byte
            # Wrap if greater than 65535 (circular checksum)
            if checksum > 65535:
                checksum -= 65536

        return checksum
    except (TypeError, AttributeError):
        return -1


def append_checksum_16bit(data: bytes) -> tuple[bool, bytes]:
    """Append 16-bit checksum to byte array in big-endian order.

    Calculates the 16-bit checksum and appends it to the data as 2 bytes
    in big-endian format (MSB first).

    Args:
        data: Array of data to calculate and append checksum

    Returns:
        Tuple of (success: bool, data_with_checksum: bytes)
        - success: True if operation succeeded, False otherwise
        - data_with_checksum: Original data with 2-byte checksum appended

    Example:
        >>> success, result = append_checksum_16bit(b'\\x01\\x02\\x03')
        >>> success
        True
        >>> len(result)
        5
    """
    try:
        # Calculate 16-bit checksum
        checksum = calc_checksum_16bit(data)
        if checksum == -1:
            return False, data

        # Convert to unsigned short (0-65535)
        checksum_u16 = checksum & 0xFFFF

        # Convert to big-endian bytes (MSB first)
        # struct.pack uses '>' for big-endian, 'H' for unsigned short
        checksum_bytes = struct.pack('>H', checksum_u16)

        # Append checksum to data
        data_with_checksum = data + checksum_bytes

        return True, data_with_checksum
    except (TypeError, struct.error):
        return False, data


def validate_checksum_16bit(data: bytes, expected_checksum: int,
                            remove_checksum: bool = False) -> bool:
    """Validate 16-bit checksum against expected value.

    Args:
        data: Data input (may or may not include checksum bytes)
        expected_checksum: Expected checksum value (0-65535)
        remove_checksum: If True, removes last 2 bytes before calculation
                        (assumes data includes checksum)

    Returns:
        True if checksum is valid, False otherwise

    Example:
        >>> # Data with checksum appended
        >>> success, data_with_cs = append_checksum_16bit(b'\\x01\\x02\\x03')
        >>> # Extract checksum from data
        >>> cs = struct.unpack('>H', data_with_cs[-2:])[0]
        >>> # Validate
        >>> validate_checksum_16bit(data_with_cs, cs, remove_checksum=True)
        True
    """
    try:
        # Make a copy of data
        local_data = bytearray(data)

        # Remove checksum bytes if needed (last 2 bytes)
        if remove_checksum and len(local_data) >= 2:
            local_data = local_data[:-2]

        # Calculate 16-bit checksum
        calculated_checksum = calc_checksum_16bit(bytes(local_data))

        # Compare
        return (calculated_checksum & 0xFFFF) == (expected_checksum & 0xFFFF)
    except (TypeError, AttributeError):
        return False


def extract_checksum_16bit_from_data(data: bytes) -> tuple[bool, int]:
    """Extract 16-bit checksum from end of data (last 2 bytes, big-endian).

    Args:
        data: Data with checksum appended (at least 2 bytes)

    Returns:
        Tuple of (success: bool, checksum: int)
        - success: True if extraction succeeded, False otherwise
        - checksum: The extracted checksum value

    Example:
        >>> success, data_with_cs = append_checksum_16bit(b'\\x01\\x02\\x03')
        >>> success2, cs = extract_checksum_16bit_from_data(data_with_cs)
        >>> success2
        True
    """
    try:
        if len(data) < 2:
            return False, 0

        # Extract last 2 bytes as big-endian unsigned short
        checksum = struct.unpack('>H', data[-2:])[0]
        return True, checksum
    except (struct.error, TypeError):
        return False, 0
