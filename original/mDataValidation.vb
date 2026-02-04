Public Module mDataValidation

    ''' <summary>
    ''' This Function computes the checksum of an array of bytes using a circular checksum byte(0) + byte(1) + byte(3) ... + byte(n-1)  
    ''' </summary>
    ''' <param name="aByteData">Byte Array of Data</param>
    ''' <returns> Success Return an integer bound by 0 - 255 Decimal; Fail Returns -1 </returns>
    ''' <remarks>Frank Boudreau 2012 LAndis+Gyr</remarks>
    Public Function i_CalcChecksum(ByRef aByteData As Byte()) As Integer
        Dim iArraySize As Integer
        Dim iChecksum As Integer
        Dim x As UInteger 'indexing variable
        Try
            'compute index length for  for do while loop
            iArraySize = aByteData.Length 'Add 1 here because I don't want to use less than or equal in my loop

            'initialze index start location because STX is not part of checksum perhaps this
            'should be conditional instead.
            x = 0

            'initialze check sum value
            iChecksum = 0

            'calculate 8 byte checksum
            Do While x < iArraySize
                iChecksum = iChecksum + aByteData(x)
                'wrap if greater than 255 there are otherways to do this in .net !!! -FJB 
                If iChecksum > 255 Then
                    iChecksum = iChecksum - 256
                End If
                x = x + 1
            Loop
            'return checksum to calling procedure
            i_CalcChecksum = iChecksum
        Catch ex As Exception
            i_CalcChecksum = -1
        End Try



    End Function 'iCalcCheckSum

    ''' <summary>
    ''' This Function computes the 16 bit CRC of the buffer of byte data. I believe this is the IBM Version, adpted by USB, ANSI ect... -FJB 
    ''' </summary>
    ''' <param name="BaData">Input Byte array of Data</param>
    ''' <returns>Returns True if Success; False if Failed</returns>
    ''' <remarks>Frank Boudreau Landis+Gyr</remarks>
    Public Function b_CalcCRC16(ByRef BaData() As Byte, ByRef ui16_CalcCRC16 As UInt16) As Boolean

        Dim ui16CRC16Poly As UInt16 = &H8005
        Dim ui16CRCRemainder As UInt16 = &H0
        Dim ui16CRC16Final_XOR As UInt16 = &H0
        Dim uiTempInt As UInt16
        'counting variables
        Dim i As UInteger
        Dim j As UInteger
        Try
            For i = 0 To BaData.Length - 1
                uiTempInt = BaData(i) * 256
                For j = 0 To 7
                    If ((uiTempInt Xor ui16CRCRemainder) >> 15) Then 'after exclusive or if most significant bit is 1
                        ui16CRCRemainder = (ui16CRCRemainder << 1) Xor ui16CRC16Poly
                    Else
                        ui16CRCRemainder = ui16CRCRemainder << 1
                    End If
                    uiTempInt = uiTempInt << 1
                Next
            Next
            ui16_CalcCRC16 = ui16CRCRemainder Xor ui16CRC16Final_XOR
            b_CalcCRC16 = True
        Catch
            b_CalcCRC16 = False
        End Try


    End Function


    ''' <summary>
    ''' This Function computes the checksum of an array of bytes using a circular checksum byte(0) + byte(1) + byte(3) ... + byte(n-1)  
    ''' </summary>
    ''' <param name="aByteData">Byte Array of Data</param>
    ''' <returns> Success Return an integer bound by 0 - 255 Decimal; Fail Returns -1 </returns>
    ''' <remarks>Frank Boudreau 2012 LAndis+Gyr</remarks>
    Public Function i_CalcChecksum16(ByRef aByteData As Byte()) As Integer
        Dim iArraySize As Integer
        Dim iChecksum As Integer
        Dim x As UInteger 'indexing variable
        Try
            'compute index length for  for do while loop
            iArraySize = aByteData.Length 'Add 1 here because I don't want to use less than or equal in my loop

            'initialze index start location because STX is not part of checksum perhaps this
            'should be conditional instead.
            x = 0

            'initialze check sum value
            iChecksum = 0

            'calculate 8 byte checksum
            Do While x < iArraySize
                iChecksum = iChecksum + aByteData(x)
                'wrap if greater than 65535 there are otherways to do this in .net !!! -FJB 
                If iChecksum > 65535 Then
                    iChecksum = iChecksum - 65536
                End If
                x = x + 1
            Loop
            'return checksum to calling procedure
            i_CalcChecksum16 = iChecksum
        Catch ex As Exception
            i_CalcChecksum16 = -1
        End Try



    End Function 'iCalcCheckSum

    ''' <summary>
    ''' Function Appends 16 Checksum to byte array in Bigendian order
    ''' </summary>
    ''' <returns>Success = True; Fail = false</returns>
    ''' <param name="aBytes">Array of data to calculate and append 16 bit checksum</param>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Public Function b_Append_CheckSum16_to_ByteArray(ByRef aBytes() As Byte) As Boolean
        Dim ichecksum16 As Integer
        Dim u16LocalChecksum As UShort
        'assume success
        b_Append_CheckSum16_to_ByteArray = True
        Try
            'calcualte 16 bit check sum
            ichecksum16 = i_CalcChecksum16(aBytes)
            'resize the byte array to append checksum without wrtting over data
            ReDim Preserve aBytes(aBytes.Length + 1)
            'convert check sum to short
            u16LocalChecksum = ichecksum16 And &HFFFF
            'convert check sum to byte array
            Dim bytes() As Byte = BitConverter.GetBytes(u16LocalChecksum)
            'Radian is big endian so append with correct endianness
            If BitConverter.IsLittleEndian Then
                aBytes(aBytes.Length - 2) = bytes(1)
                aBytes(aBytes.Length - 1) = bytes(0)
            Else
                aBytes(aBytes.Length - 2) = bytes(0)
                aBytes(aBytes.Length - 1) = bytes(1)
            End If
        Catch
            b_Append_CheckSum16_to_ByteArray = False
        End Try
    End Function


    ''' <summary>
    ''' This Function calcualtes the 16 bit checksum and compares to provided checksum
    ''' </summary>
    ''' <param name="aBytes">Data Input</param>
    ''' <param name="CheckSum">Checksum</param>
    ''' <param name="bRemoveChecksum">If Data has Checksum set to true to remove it assumes 2 Bytes</param>
    ''' <returns>True if Checksum is good; False if Checksum is bad </returns>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Public Function b_Validate_CheckSum16(ByRef aBytes() As Byte, ByVal CheckSum As UShort, ByVal bRemoveChecksum As Boolean) As Boolean
        Dim ichecksum16 As Integer
        Dim BaLocalData(aBytes.Length - 1) As Byte

        'assume success
        b_Validate_CheckSum16 = True
        Try
            'make new copy of data
            Array.Copy(aBytes, BaLocalData, aBytes.Length)
            'remove checksum if needed
            If bRemoveChecksum Then
                ReDim Preserve BaLocalData(BaLocalData.Length - 3)
            End If
            'calculate 16 bit check sum
            ichecksum16 = i_CalcChecksum16(BaLocalData)
            If CUShort(ichecksum16) <> CheckSum Then
                b_Validate_CheckSum16 = False
            End If
        Catch
            b_Validate_CheckSum16 = False
            Throw New Exception("Unknown error while validating Checksum")
        End Try
    End Function

End Module 'mDataValidation
