Option Explicit Off
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.ComponentModel
Imports System.IO
Imports System.IO.Ports.SerialPort
Imports System.Text.RegularExpressions
Imports System.Math

Public Module mDataManipulation


    ''' <summary>
    ''' This Function convets a Byte Array to a ASCII String IF the Bytes are 0x30h, 0x35, 0x39 then String is 059
    ''' </summary>
    ''' <param name="BaBytes">Input Byte Array</param>
    ''' <returns>The Byte array as a String on Success; VbNullString onEerror</returns>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Function str_Byte_Array_To_ASCII_String(ByRef BaBytes() As Byte) As String
        Dim i As Integer = 0
        str_Byte_Array_To_ASCII_String = ""
        Try
            str_Byte_Array_To_ASCII_String = System.Text.Encoding.ASCII.GetString(BaBytes)
        Catch ex As Exception
            str_Byte_Array_To_ASCII_String = vbNullString
        End Try

        i = 0
    End Function



    ''' <summary>
    ''' Converts TI Floating Point to VB Single
    ''' </summary>
    ''' <param name="BaInput">
    ''' BaInput    Byte Array; 
    ''' </param>
    ''' <returns>
    ''' s_TI_to_VB_Single Single
    ''' </returns>
    ''' <remarks>
    ''' TI Floating point number as Assy of 4 Bytes BaInput(0)...BaInput(3) MSB, LSB
    ''' Converted From Radian Research App Note 
    ''' Frank Boudreau 2012 Landis+Gyr
    ''' </remarks>
    Public Function s_TI_to_VB_Single(ByRef BaInput() As Byte) As Single

        'must be greater tha4 bytes
        If BaInput.Length < 4 Then

            s_TI_to_VB_Single = vbNull
            Exit Function
        End If

        ' Dim ulExponentIEEE_E1 As Long
        ' Dim ulExponentTI_E2 As Long
        ' Dim sBExponent As SByte
        ' Dim ulFractionalIEEE_F1 As Long
        ' Dim ulFractionalTI_F2 As Long
        ' Dim uiSignIEEE_S1 As UInteger
        ' Dim uiSignTI_S2 As UInteger
        ' Dim sIEEE_Float As Single = 0
        ' Dim sIEEE_Float As Single
        ' Dim lTI_SignBitmask As Long '= &H800000
        ' Dim lIEE_SignBitMask As Long ' = &H80000000
        ' Dim lFractionalMask As Long '= &H7FFFFF
        ' uiSignIEEE_S1 = 0
        ' Dim ulTI_Float As Long 'Unsugned long to hold TI FLoating Point.
        ' ulTI_Float = BaInput(0)
        ' ulTI_Float = ulTI_Float << 8
        ' ulTI_Float = ((((ulTI_Float Or BaInput(1)) << 8) Or BaInput(2)) << 8) Or BaInput(3)

        Dim ulMantissa As ULong
        Dim bNegMantissa As Boolean = False
        Dim ulExponent As ULong
        'Dim ulValue As ULong
        Dim sExponent As Single

        'First Byte is Exponent
        'Examine Boundry Conditions
        'From Radian App note
        If BaInput(0) = &H80 Or BaInput(0) = &H81 Then
            '2s-complment maps to zero
            '2s-compliment to small for IEEE
            s_TI_to_VB_Single = 0.0
            Exit Function
        End If

        'Continue to
        'Examine Boundry Conditions
        'From Radian App note
        If BaInput(0) = &H7F And BaInput(1) = &H7F And BaInput(2) = &HFF And BaInput(3) = &HFF Then
            'Max Value
            s_TI_to_VB_Single = 999999999
            Exit Function
        End If

        'Extract Mantissa
        ulMantissa = CULng(BaInput(1) And &H7F) << 16 Or CULng(BaInput(2) And &HFF) << 8 Or CULng(BaInput(3) And &HFF)

        'Extract Sign Bit
        If (BaInput(1) And &H80) <> 0 Then
            bNegMantissa = True
        End If
        'Last one
        'Examine Boundry Conditions
        'From Radian App note
        If BaInput(0) = &H7F And bNegMantissa And ulMantissa = 0 Then
            'Min Value
            s_TI_to_VB_Single = -9999999999
        End If

        'Get Exponent
        ulExponent = CULng(BaInput(0) And &HFF)

        'Unwrap Exponent (2's Compliment and get value and convert to Single)
        If ulExponent > 127 Then
            'Negative Exponent
            sExponent = -((CULng(256) - ulExponent) And CULng(&H7F))
        Else
            'Positive
            sExponent = ulExponent
        End If


        If bNegMantissa = False Then
            ' Positive
            ' ulValue = ulMantissa And CULng(&H7FFFFFFF)
            ulMantissa = ulMantissa And CULng(&H7FFFFFFF)
        ElseIf ulMantissa <> 0 Then
            'Negative non zero Mantissa
            'ulValue = CULng(Not ulMantissa) + CULng(&H1) And CULng(&H7FFFFF)
            ulMantissa = CULng(Not ulMantissa) + CULng(&H1) And CULng(&H7FFFFF)
        Else
            'Mantissa is zero
            'ulValue = 0
            ulMantissa = 0
        End If
        'BaInput(3) = CByte(ulValue And CULng(&HFF))
        ' Dim temp As ULong
        ' temp = ulValue >> 8

        's_TI_to_IEEE2 = CSng((1 + CSng(ulValue) / CSng(2 ^ 23)) * (2 ^ CSng(sExponent)))
        s_TI_to_VB_Single = CSng((1 + CSng(ulMantissa) / CSng(2 ^ 23)) * (2 ^ CSng(sExponent)))
        If bNegMantissa Then
            s_TI_to_VB_Single *= -1
        End If
        ' temp = 0
    End Function

    ''' <summary>
    ''' This Function takes a Hex String and converts it to a byte array. String A603FF... --> (0xA6h, 0x03h, 0xFFh,...) Not ASCII values!
    ''' </summary>
    ''' <param name="strHexString">Input string</param>
    ''' <param name="aBytes">Output Byte Array string</param>
    ''' <returns>TRUE if Success, False if Error</returns>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Public Function b_Hex_String_To_ByteArray(ByRef strHexString As String, ByRef aBytes() As Byte) As Boolean

        ReDim aBytes(CInt(strHexString.Length / 2) - 1)
        Dim i As Integer = 0
        b_Hex_String_To_ByteArray = True 'assume success

        For i = 0 To strHexString.Length - 2 Step 2
            Try
                'First Convert to Bytes from base 16
                aBytes(CInt(i / 2)) = Convert.ToByte(strHexString.Substring(i, 2), 16)
            Catch EX As Exception
                b_Hex_String_To_ByteArray = False
                Exit For
            End Try
        Next

    End Function

    ''' <summary>
    ''' Converts Bytes to String of Readable Byte values 0x30h, 0x35, 0x39 then String is 303539
    ''' </summary>
    ''' <param name="abytes">Byte array</param>
    ''' <returns>String Readable of Bytes on success; empty string on Fail</returns>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Public Function str_ByteArray_To_Readable_String(ByRef abytes() As Byte) As String
        'init
        str_ByteArray_To_Readable_String = ""
        'parse data from byte to string (Big endian) add dropped implied zeros to string!
        Try
            For i = 0 To abytes.Length - 1
                If abytes(i) > &HF Then
                    str_ByteArray_To_Readable_String += String.Format(abytes(i).ToString("X"))
                Else
                    str_ByteArray_To_Readable_String += "0" + String.Format(abytes(i).ToString("X"))
                End If
            Next
        Catch
            'return empty string on error
            str_ByteArray_To_Readable_String = ""

        End Try
    End Function

    ''' <summary>
    ''' "Function finds the MAx and Min of a Array of Singles 1 over loads
    ''' </summary>
    ''' <param name="Data">Array of type single</param>
    ''' <param name="Max">Largest Value</param>
    ''' <param name="Min">Smallest Value</param>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Public Sub v_Bounds(ByRef Data() As Single, ByRef Max As Single, ByRef Min As Single)
        Max = Data(0)
        Min = Data(0)
        Try
            If Data.Length < 2 Then
                Throw New Exception("Dat array must have at least teo points")
            End If
            For i = 1 To Data.Length - 1
                If Data(i) > Max Then
                    Max = Data(i)
                End If

                If Data(i) < Min Then
                    Min = Data(i)
                End If
            Next
        Catch ex As Exception
            Throw New Exception(ex.ToString)
        End Try

    End Sub
    ''' <summary>
    ''' "Function finds the MAx and Min of a Array of String. String Should be validated to only be numeric 1 overloads
    ''' </summary>
    ''' <param name="Data">Array of Numeric string data</param>
    ''' <param name="Max">Largest Value</param>
    ''' <param name="Min">Smallest Value</param>
    ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
    Public Sub v_Bounds(ByRef Data() As String, ByRef Max As Single, ByRef Min As Single)
        Max = Val(Data(0))
        Min = Val(Data(0))
        Try
            If Data.Length < 2 Then
                Throw New Exception("Dat array must have at least teo points")
            End If
            For i = 1 To Data.Length - 1
                If Val(Data(i)) > Max Then
                    Max = Val(Data(i))
                End If

                If Val(Data(i)) < Min Then
                    Min = Val(Data(i))
                End If
            Next
        Catch ex As Exception
            Throw New Exception(ex.ToString)
        End Try

    End Sub
End Module

