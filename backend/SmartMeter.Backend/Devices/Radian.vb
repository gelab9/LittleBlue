''' <summary>
''' Radian Power Analyzer Serial Communication Driver
''' Simplified version for REST API backend
''' Based on original mRadian.vb by Frank Boudreau 2012 Landis+Gyr
''' Python port/simplification: 2026
''' </summary>

Imports System.IO.Ports
Imports System.Text
Imports System.Threading

Namespace Devices
    Public Class Radian
        Private _port As SerialPort
        Private _receiveBuffer(4095) As Byte
        Private _isConnected As Boolean = False

        Public ReadOnly Property IsConnected As Boolean
            Get
                Return _isConnected AndAlso _port IsNot Nothing AndAlso _port.IsOpen
            End Get
        End Property

        ''' <summary>
        ''' Connect to Radian on specified COM port
        ''' </summary>
        Public Sub Connect(portName As String, baudRate As Integer, Optional parity As Parity = Parity.None,
                          Optional stopBits As StopBits = StopBits.One, Optional dataBits As Integer = 8)
            Try
                If _port IsNot Nothing AndAlso _port.IsOpen Then
                    _port.Close()
                End If

                _port = New SerialPort() With {
                    .PortName = portName,
                    .BaudRate = baudRate,
                    .Parity = parity,
                    .StopBits = stopBits,
                    .DataBits = dataBits,
                    .DtrEnable = False,
                    .Handshake = Handshake.None,
                    .ReadTimeout = 2000,
                    .WriteTimeout = 2000
                }

                _port.Open()

                ' Purge any junk from buffer
                Thread.Sleep(100)
                If _port.BytesToRead > 0 Then
                    _port.DiscardInBuffer()
                End If

                _isConnected = True

            Catch ex As Exception
                _isConnected = False
                Throw New Exception($"Failed to connect to Radian on {portName}: {ex.Message}")
            End Try
        End Sub

        ''' <summary>
        ''' Disconnect from Radian
        ''' </summary>
        Public Sub Disconnect()
            Try
                If _port IsNot Nothing AndAlso _port.IsOpen Then
                    _port.Close()
                End If
                _isConnected = False
            Catch ex As Exception
                Throw New Exception($"Failed to disconnect Radian: {ex.Message}")
            End Try
        End Sub

        ''' <summary>
        ''' Send hex command string to Radian (with checksum calculation)
        ''' </summary>
        ''' <param name="hexCommand">Hex string command without checksum (e.g., "A6020000")</param>
        Public Sub SendCommand(hexCommand As String)
            Try
                If Not IsConnected Then
                    Throw New Exception("Radian not connected")
                End If

                ' Convert hex string to bytes
                Dim bytes = HexStringToBytes(hexCommand)

                ' Calculate and append 16-bit checksum
                Dim checksum As UShort = CalculateChecksum16(bytes)
                ReDim Preserve bytes(bytes.Length + 1)
                bytes(bytes.Length - 2) = CByte((checksum >> 8) And &HFF)  ' MSB
                bytes(bytes.Length - 1) = CByte(checksum And &HFF)         ' LSB

                ' Clear buffers and send
                _port.DiscardInBuffer()
                _port.DiscardOutBuffer()
                _port.Write(bytes, 0, bytes.Length)
                
                ' Debug: log sent command
                Console.WriteLine($"[Radian] Sent {bytes.Length} bytes: {BitConverter.ToString(bytes).Replace("-", "")}")

            Catch ex As Exception
                Throw New Exception($"Failed to send Radian command: {ex.Message}")
            End Try
        End Sub

        ''' <summary>
        ''' Receive response from Radian with timeout
        ''' </summary>
        ''' <param name="timeoutMs">Timeout in milliseconds</param>
        ''' <returns>Received bytes</returns>
        Public Function ReceiveResponse(Optional timeoutMs As Integer = 1000) As Byte()
            Try
                If Not IsConnected Then
                    Throw New Exception("Radian not connected")
                End If

                Dim startTime = Environment.TickCount
                Dim offset As Integer = 0
                Dim expectedLength As Integer = 0
                Dim receivedData As New List(Of Byte)

                ' Wait for data with timeout
                While (Environment.TickCount - startTime) < timeoutMs
                    If _port.BytesToRead > 0 Then
                        Dim bytesToRead = _port.BytesToRead
                        Dim buffer(bytesToRead - 1) As Byte
                        Dim bytesRead = _port.Read(buffer, 0, bytesToRead)
                        receivedData.AddRange(buffer.Take(bytesRead))
                        
                        ' Debug: log received data
                        Console.WriteLine($"[Radian] Received {bytesRead} bytes: {BitConverter.ToString(buffer, 0, bytesRead).Replace("-", "")}")

                        ' Check if we have enough data to determine packet length
                        If receivedData.Count >= 4 AndAlso expectedLength = 0 Then
                            ' Length is at bytes 2-3 (big-endian)
                            expectedLength = (CInt(receivedData(2)) << 8) Or receivedData(3)
                            expectedLength += 6  ' Add header (4 bytes) + checksum (2 bytes)
                            Console.WriteLine($"[Radian] Detected packet length: {expectedLength}, current buffer: {receivedData.Count}")
                        End If

                        ' Check if we received complete packet
                        If expectedLength > 0 AndAlso receivedData.Count >= expectedLength Then
                            Return receivedData.ToArray()
                        End If
                    Else
                        Thread.Sleep(10)
                    End If
                End While

                ' Timeout - log what we got
                Console.WriteLine($"[Radian] Timeout after {timeoutMs}ms. Received {receivedData.Count} bytes.")
                
                ' Timeout - return what we have
                If receivedData.Count > 0 Then
                    Return receivedData.ToArray()
                End If

                Throw New TimeoutException("Radian receive timeout")

            Catch ex As Exception
                Throw New Exception($"Failed to receive Radian response: {ex.Message}")
            End Try
        End Function

        ''' <summary>
        ''' Send command and wait for response
        ''' </summary>
        Public Function QueryCommand(hexCommand As String, Optional timeoutMs As Integer = 1000) As Byte()
            SendCommand(hexCommand)
            Thread.Sleep(50)  ' Small delay for device processing
            Return ReceiveResponse(timeoutMs)
        End Function

        ''' <summary>
        ''' Calculate 16-bit checksum (circular addition, modulo 65536)
        ''' </summary>
        Private Function CalculateChecksum16(data As Byte()) As UShort
            Dim checksum As Integer = 0
            For Each b As Byte In data
                checksum += b
                If checksum > 65535 Then
                    checksum -= 65536
                End If
            Next
            Return CUShort(checksum And &HFFFF)
        End Function

        ''' <summary>
        ''' Convert hex string to byte array
        ''' </summary>
        Private Function HexStringToBytes(hexString As String) As Byte()
            If hexString.Length Mod 2 <> 0 Then
                Throw New ArgumentException("Hex string length must be even")
            End If

            Dim bytes(hexString.Length \ 2 - 1) As Byte
            For i As Integer = 0 To hexString.Length - 1 Step 2
                bytes(i \ 2) = Convert.ToByte(hexString.Substring(i, 2), 16)
            Next
            Return bytes
        End Function

        ''' <summary>
        ''' Convert byte array to hex string
        ''' </summary>
        Private Function BytesToHexString(bytes As Byte()) As String
            Dim sb As New StringBuilder()
            For Each b As Byte In bytes
                sb.Append(b.ToString("X2"))
            Next
            Return sb.ToString()
        End Function

        ' ===== RADIAN COMMAND CONSTANTS =====

        Public Const CMD_IDENTIFY As String = "A6020000"
        Public Const CMD_RESET_RD As String = "A6030000"
        Public Const CMD_STOP_ACCUM As String = "A6090000"
        Public Const CMD_RESET_METRICS_BASE As String = "A6070001"
        Public Const CMD_START_ACCUM_BASE As String = "A6080003"
        Public Const CMD_START_TIMED_ACCUM_BASE As String = "A60A0004"

        ''' <summary>
        ''' Build Reset Metrics command
        ''' </summary>
        Public Function BuildResetMetricsCommand(resetWaveform As Boolean, resetInstant As Boolean,
                                                 resetInstantMin As Boolean, resetInstantMax As Boolean,
                                                 resetAccum As Boolean) As String
            Dim resetCode As Byte = 0

            If resetWaveform Then resetCode = resetCode Or &H1
            If resetInstant Then resetCode = resetCode Or &H2
            If resetInstantMin Then resetCode = resetCode Or &H4
            If resetInstantMax Then resetCode = resetCode Or &H8
            If resetAccum Then resetCode = resetCode Or &H10

            Return CMD_RESET_METRICS_BASE & resetCode.ToString("X2")
        End Function

        ''' <summary>
        ''' Build Start Accumulation command
        ''' </summary>
        Public Function BuildStartAccumCommand(controlByte As Byte, pulseData As UShort) As String
            Dim pulseMSB As Byte = CByte((pulseData >> 8) And &HFF)
            Dim pulseLSB As Byte = CByte(pulseData And &HFF)

            Return CMD_START_ACCUM_BASE & controlByte.ToString("X2") &
                   pulseMSB.ToString("X2") & pulseLSB.ToString("X2")
        End Function

        ''' <summary>
        ''' Build Start Timed Accumulation Test command
        ''' </summary>
        Public Function BuildStartTimedAccumCommand(timeInSeconds As Single) As String
            ' Convert seconds to samples (20119.225 samples/sec for Rev 5+)
            Dim samplesPerSecond As Single = 20119.225
            Dim numberOfSamples As UInteger = CUInt(timeInSeconds * samplesPerSecond)

            Dim byte0 As Byte = CByte((numberOfSamples >> 24) And &HFF)
            Dim byte1 As Byte = CByte((numberOfSamples >> 16) And &HFF)
            Dim byte2 As Byte = CByte((numberOfSamples >> 8) And &HFF)
            Dim byte3 As Byte = CByte(numberOfSamples And &HFF)

            Return CMD_START_TIMED_ACCUM_BASE & byte0.ToString("X2") &
                   byte1.ToString("X2") & byte2.ToString("X2") & byte3.ToString("X2")
        End Function
    End Class
End Namespace
