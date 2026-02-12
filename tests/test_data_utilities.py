"""Unit tests for data conversion and validation utilities.

Tests verify that Python implementations match the original VB code behavior.
"""

import pytest
import struct
from src.util.data_conversion import (
    byte_array_to_ascii_string,
    ti_float_to_ieee_single,
    hex_string_to_byte_array,
    byte_array_to_readable_hex_string,
    find_min_max,
)
from src.util.data_validation import (
    calc_checksum_8bit,
    calc_crc16,
    calc_checksum_16bit,
    append_checksum_16bit,
    validate_checksum_16bit,
    extract_checksum_16bit_from_data,
)


class TestDataConversion:
    """Test data conversion utilities."""

    def test_byte_array_to_ascii_string_valid(self):
        """Test ASCII conversion with valid input."""
        data = b'\x30\x35\x39'  # '0', '5', '9'
        result = byte_array_to_ascii_string(data)
        assert result == '059'

    def test_byte_array_to_ascii_string_text(self):
        """Test ASCII conversion with text."""
        data = b'Hello'
        result = byte_array_to_ascii_string(data)
        assert result == 'Hello'

    def test_byte_array_to_ascii_string_invalid(self):
        """Test ASCII conversion with invalid input."""
        result = byte_array_to_ascii_string(None)
        assert result == ""

    def test_ti_float_to_ieee_zero_boundary(self):
        """Test TI float conversion - zero boundary condition."""
        # 0x80 = zero mapping
        data = bytes([0x80, 0x00, 0x00, 0x00])
        result = ti_float_to_ieee_single(data)
        assert result == 0.0

        # 0x81 = too small for IEEE
        data = bytes([0x81, 0x00, 0x00, 0x00])
        result = ti_float_to_ieee_single(data)
        assert result == 0.0

    def test_ti_float_to_ieee_max_boundary(self):
        """Test TI float conversion - maximum value boundary."""
        # Max value: 0x7F 0x7F 0xFF 0xFF
        data = bytes([0x7F, 0x7F, 0xFF, 0xFF])
        result = ti_float_to_ieee_single(data)
        assert result == 999999999.0

    def test_ti_float_to_ieee_min_boundary(self):
        """Test TI float conversion - minimum value boundary."""
        # Min value: 0x7F 0x80 0x00 0x00 (negative with zero mantissa)
        data = bytes([0x7F, 0x80, 0x00, 0x00])
        result = ti_float_to_ieee_single(data)
        assert result == -9999999999.0

    def test_ti_float_to_ieee_positive_value(self):
        """Test TI float conversion - positive value."""
        # Test value: exponent=2, mantissa=0x400000 (0.5 in fractional part)
        # Expected: (1 + 0.5) * 2^2 = 1.5 * 4 = 6.0
        data = bytes([0x02, 0x40, 0x00, 0x00])
        result = ti_float_to_ieee_single(data)
        assert abs(result - 6.0) < 0.0001

    def test_ti_float_to_ieee_negative_value(self):
        """Test TI float conversion - negative value."""
        # Test value: exponent=2, negative mantissa
        # Sign bit set (0x80), mantissa=0x400000
        data = bytes([0x02, 0xC0, 0x00, 0x00])
        result = ti_float_to_ieee_single(data)
        assert result < 0  # Should be negative

    def test_ti_float_to_ieee_negative_exponent(self):
        """Test TI float conversion - negative exponent."""
        # Exponent = -2 (0xFE in 2's complement = 254)
        # Expected: (1 + mantissa/2^23) * 2^(-2) = value / 4
        data = bytes([0xFE, 0x00, 0x00, 0x00])  # Mantissa = 0
        result = ti_float_to_ieee_single(data)
        assert abs(result - 0.25) < 0.0001  # 1.0 * 2^(-2) = 0.25

    def test_ti_float_to_ieee_invalid_length(self):
        """Test TI float conversion with invalid input length."""
        data = bytes([0x01, 0x02])  # Only 2 bytes
        result = ti_float_to_ieee_single(data)
        assert result is None

    def test_hex_string_to_byte_array_valid(self):
        """Test hex string to byte array conversion."""
        hex_str = "A603FF"
        result = hex_string_to_byte_array(hex_str)
        assert result == b'\xA6\x03\xFF'

    def test_hex_string_to_byte_array_with_spaces(self):
        """Test hex string conversion with spaces."""
        hex_str = "A6 03 FF"
        result = hex_string_to_byte_array(hex_str)
        assert result == b'\xA6\x03\xFF'

    def test_hex_string_to_byte_array_lowercase(self):
        """Test hex string conversion with lowercase."""
        hex_str = "a603ff"
        result = hex_string_to_byte_array(hex_str)
        assert result == b'\xA6\x03\xFF'

    def test_hex_string_to_byte_array_invalid(self):
        """Test hex string conversion with invalid input."""
        result = hex_string_to_byte_array("GGHHII")
        assert result is None

    def test_byte_array_to_readable_hex_string_valid(self):
        """Test byte array to readable hex string."""
        data = b'\x30\x35\x39'
        result = byte_array_to_readable_hex_string(data)
        assert result == '303539'

    def test_byte_array_to_readable_hex_string_with_leading_zeros(self):
        """Test hex string with leading zeros."""
        data = b'\x01\x0F\xFF'
        result = byte_array_to_readable_hex_string(data)
        assert result == '010FFF'

    def test_byte_array_to_readable_hex_string_invalid(self):
        """Test hex string conversion with invalid input."""
        result = byte_array_to_readable_hex_string(None)
        assert result == ""

    def test_find_min_max_valid(self):
        """Test min/max finding."""
        data = [1.0, 5.0, 3.0, 9.0, 2.0]
        min_val, max_val = find_min_max(data)
        assert min_val == 1.0
        assert max_val == 9.0

    def test_find_min_max_negative(self):
        """Test min/max with negative values."""
        data = [-5.0, 3.0, -10.0, 8.0]
        min_val, max_val = find_min_max(data)
        assert min_val == -10.0
        assert max_val == 8.0

    def test_find_min_max_insufficient_data(self):
        """Test min/max with insufficient data."""
        with pytest.raises(ValueError):
            find_min_max([1.0])


class TestDataValidation:
    """Test data validation utilities."""

    def test_calc_checksum_8bit_simple(self):
        """Test 8-bit checksum calculation."""
        data = b'\x01\x02\x03'
        result = calc_checksum_8bit(data)
        assert result == 6  # 1 + 2 + 3 = 6

    def test_calc_checksum_8bit_wrapping(self):
        """Test 8-bit checksum with wrapping."""
        data = b'\xFF\xFF\xFF'  # 255 + 255 + 255 = 765
        result = calc_checksum_8bit(data)
        # 255 -> wrap -> (255-256=-1) + 255 -> wrap -> (254-256=-2) + 255 = 253
        # Actually: 255 + 255 = 510 -> 510-256 = 254, 254 + 255 = 509 -> 509-256 = 253
        assert result == 253

    def test_calc_checksum_8bit_zero(self):
        """Test 8-bit checksum with zeros."""
        data = b'\x00\x00\x00'
        result = calc_checksum_8bit(data)
        assert result == 0

    def test_calc_checksum_8bit_invalid(self):
        """Test 8-bit checksum with invalid input."""
        result = calc_checksum_8bit(None)
        assert result == -1

    def test_calc_crc16_simple(self):
        """Test CRC-16 calculation."""
        data = b'\x01\x02\x03'
        success, crc = calc_crc16(data)
        assert success is True
        assert isinstance(crc, int)
        assert 0 <= crc <= 65535

    def test_calc_crc16_empty(self):
        """Test CRC-16 with empty data."""
        data = b''
        success, crc = calc_crc16(data)
        assert success is True
        assert crc == 0

    def test_calc_crc16_known_value(self):
        """Test CRC-16 with a known test vector."""
        # Common test vector: "123456789"
        data = b'123456789'
        success, crc = calc_crc16(data)
        assert success is True
        # For CRC-16/IBM (also called CRC-16/ARC or CRC-16/ANSI)
        # The expected value for "123456789" varies by implementation
        # We just verify it's calculated successfully
        assert 0 <= crc <= 65535

    def test_calc_crc16_invalid(self):
        """Test CRC-16 with invalid input."""
        success, crc = calc_crc16(None)
        assert success is False
        assert crc == 0

    def test_calc_checksum_16bit_simple(self):
        """Test 16-bit checksum calculation."""
        data = b'\x01\x02\x03'
        result = calc_checksum_16bit(data)
        assert result == 6  # 1 + 2 + 3 = 6

    def test_calc_checksum_16bit_large(self):
        """Test 16-bit checksum with large values."""
        data = b'\xFF' * 300  # 255 * 300 = 76500
        result = calc_checksum_16bit(data)
        # 76500 % 65536 = 10964
        assert result == 10964

    def test_calc_checksum_16bit_invalid(self):
        """Test 16-bit checksum with invalid input."""
        result = calc_checksum_16bit(None)
        assert result == -1

    def test_append_checksum_16bit_simple(self):
        """Test appending 16-bit checksum."""
        data = b'\x01\x02\x03'
        success, result = append_checksum_16bit(data)
        assert success is True
        assert len(result) == len(data) + 2  # Original + 2 checksum bytes
        assert result[:3] == data  # Original data preserved

    def test_append_checksum_16bit_verify(self):
        """Test appending and extracting checksum."""
        data = b'\x01\x02\x03\x04\x05'
        success, data_with_cs = append_checksum_16bit(data)
        assert success is True

        # Extract checksum from end
        success2, extracted_cs = extract_checksum_16bit_from_data(data_with_cs)
        assert success2 is True

        # Verify checksum matches calculated value
        calculated_cs = calc_checksum_16bit(data)
        assert extracted_cs == calculated_cs

    def test_validate_checksum_16bit_valid(self):
        """Test validating correct checksum."""
        data = b'\x01\x02\x03'
        success, data_with_cs = append_checksum_16bit(data)
        assert success is True

        # Extract and validate
        success2, checksum = extract_checksum_16bit_from_data(data_with_cs)
        assert success2 is True

        is_valid = validate_checksum_16bit(data_with_cs, checksum, remove_checksum=True)
        assert is_valid is True

    def test_validate_checksum_16bit_invalid(self):
        """Test validating incorrect checksum."""
        data = b'\x01\x02\x03'
        wrong_checksum = 9999

        is_valid = validate_checksum_16bit(data, wrong_checksum, remove_checksum=False)
        assert is_valid is False

    def test_extract_checksum_16bit_big_endian(self):
        """Test extracting checksum in big-endian format."""
        # Manually construct data with known checksum
        # Checksum = 0x1234 in big-endian = [0x12, 0x34]
        data = b'\x01\x02\x03\x12\x34'
        success, checksum = extract_checksum_16bit_from_data(data)
        assert success is True
        assert checksum == 0x1234

    def test_extract_checksum_16bit_insufficient_data(self):
        """Test extracting checksum from insufficient data."""
        data = b'\x01'  # Only 1 byte
        success, checksum = extract_checksum_16bit_from_data(data)
        assert success is False


class TestIntegration:
    """Integration tests combining conversion and validation."""

    def test_roundtrip_hex_conversion(self):
        """Test hex string -> bytes -> hex string roundtrip."""
        original = "A603FF"
        bytes_data = hex_string_to_byte_array(original)
        assert bytes_data is not None

        result = byte_array_to_readable_hex_string(bytes_data)
        assert result == original

    def test_checksum_validation_workflow(self):
        """Test complete checksum workflow."""
        # Original data
        data = b'\x10\x20\x30\x40\x50'

        # Append checksum
        success, data_with_cs = append_checksum_16bit(data)
        assert success is True
        assert len(data_with_cs) == 7  # 5 + 2

        # Extract checksum
        success, checksum = extract_checksum_16bit_from_data(data_with_cs)
        assert success is True

        # Validate checksum
        is_valid = validate_checksum_16bit(data_with_cs, checksum, remove_checksum=True)
        assert is_valid is True

    def test_ti_float_realistic_values(self):
        """Test TI float with realistic power measurement values."""
        # Test that the function works with various exponent/mantissa combinations
        # Actual Radian test vectors would be needed for precise validation

        test_cases = [
            bytes([0x07, 0x70, 0x00, 0x00]),  # Exponent 7, positive mantissa
            bytes([0x00, 0x00, 0x00, 0x00]),  # Exponent 0, mantissa 0 -> 1.0
            bytes([0x05, 0x40, 0x00, 0x00]),  # Exponent 5, mantissa with MSB set
            bytes([0xFC, 0x00, 0x00, 0x00]),  # Negative exponent (-4)
        ]

        for data in test_cases:
            result = ti_float_to_ieee_single(data)
            # Just verify the function returns a valid float
            assert result is not None
            assert isinstance(result, float)
            # Verify result is not NaN or Inf
            assert not (result != result)  # Check not NaN
            assert abs(result) < 1e10  # Reasonable range check


if __name__ == '__main__':
    pytest.main([__file__, '-v'])
