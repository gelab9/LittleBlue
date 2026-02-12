''' <summary>
''' PAC Power UPC1/UPC3 Serial Communication Driver
''' ASCII SCPI-like protocol with OPC handshaking
''' Based on original PacPowerUCP1.vb by Frank Boudreau 2018 Landis+Gyr
''' </summary>

Imports System.IO.Ports
Imports System.Text
Imports System.Threading

Namespace Devices
    Public Class PacPower
        Private _port As SerialPort
        Private _isConnected As Boolean = False

        Public ReadOnly Property IsConnected As Boolean
            Get
                Return _isConnected AndAlso _port IsNot Nothing AndAlso _port.IsOpen
            End Get
        End Property

        ''' <summary>
        ''' Connect to PAC Power on specified COM port.
        ''' </summary>
        Public Sub Connect(portName As String,
                           Optional baudRate As Integer = 9600,
                           Optional parity As Parity = Parity.None,
                           Optional stopBits As StopBits = StopBits.One,
                           Optional dataBits As Integer = 8,
                           Optional dtrEnable As Boolean = True,
                           Optional handshake As Handshake = Handshake.None,
                           Optional readTimeoutMs As Integer = 3000,
                           Optional writeTimeoutMs As Integer = 2000)
            Try
                Disconnect()

                _port = New SerialPort() With {
                    .PortName = portName,
                    .BaudRate = baudRate,
                    .Parity = parity,
                    .StopBits = stopBits,
                    .DataBits = dataBits,
                    .DtrEnable = dtrEnable,
                    .Handshake = handshake,
                    .ReadTimeout = readTimeoutMs,
                    .WriteTimeout = writeTimeoutMs
                }

                _port.Open()

                ' Purge junk from buffer
                Thread.Sleep(200)
                If _port.BytesToRead > 0 Then
                    _port.DiscardInBuffer()
                End If
                _port.DiscardOutBuffer()

                _isConnected = True

            Catch ex As Exception
                _isConnected = False
                Throw New Exception($"Failed to connect to PAC Power on {portName}: {ex.Message}")
            End Try
        End Sub

        ''' <summary>
        ''' Disconnect from PAC Power.
        ''' </summary>
        Public Sub Disconnect()
            Try
                If _port IsNot Nothing Then
                    If _port.IsOpen Then _port.Close()
                    _port.Dispose()
                End If
                _isConnected = False
            Catch
                ' swallow; disconnect should be safe
            Finally
                _port = Nothing
                _isConnected = False
            End Try
        End Sub

        ''' <summary>
        ''' Send an ASCII SCPI command with ;*OPC? appended and CRLF termination.
        ''' Waits for "1" + CRLF response (operation complete acknowledgment).
        ''' </summary>
        ''' <param name="command">SCPI command string (e.g., ":VOLT1 120")</param>
        Public Sub Send(command As String)
            EnsureConnected()

            _port.DiscardInBuffer()
            _port.DiscardOutBuffer()

            ' Append OPC query for handshaking and CRLF termination
            Dim fullCommand = command.TrimEnd() & ";*OPC?" & vbCrLf
            Dim bytes = Encoding.ASCII.GetBytes(fullCommand)
            _port.Write(bytes, 0, bytes.Length)
            _port.BaseStream.Flush()

            ' Wait for "1\r\n" OPC acknowledgment
            WaitForOpc(3000)
        End Sub

        ''' <summary>
        ''' Send a query command and return the response string.
        ''' For measurement queries the response is the value followed by "1\r\n" from OPC.
        ''' </summary>
        Public Function Query(command As String) As String
            EnsureConnected()

            _port.DiscardInBuffer()
            _port.DiscardOutBuffer()

            ' Append OPC query and CRLF
            Dim fullCommand = command.TrimEnd() & ";*OPC?" & vbCrLf
            Dim bytes = Encoding.ASCII.GetBytes(fullCommand)
            _port.Write(bytes, 0, bytes.Length)
            _port.BaseStream.Flush()

            ' Read response
            Return ReadResponse(3000)
        End Function

        ''' <summary>
        ''' Send a raw command WITHOUT OPC handshaking (e.g., *IDN? which doesn't support OPC).
        ''' </summary>
        Public Function QueryRaw(command As String) As String
            EnsureConnected()

            _port.DiscardInBuffer()
            _port.DiscardOutBuffer()

            Dim fullCommand = command.TrimEnd() & vbCrLf
            _port.Write(fullCommand)
            _port.BaseStream.Flush()

            Thread.Sleep(200)
            Return ReadRaw(2000).Trim()
        End Function

        ' ===== Private helpers =====

        ''' <summary>
        ''' Wait for OPC acknowledgment ("1\r\n") within timeout.
        ''' </summary>
        Private Sub WaitForOpc(timeoutMs As Integer)
            Dim sb As New StringBuilder()
            Dim deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs)

            While DateTime.UtcNow < deadline
                If _port.BytesToRead > 0 Then
                    sb.Append(_port.ReadExisting())
                    If sb.ToString().Contains("1" & vbCrLf) Then
                        Return ' OPC received
                    End If
                Else
                    Thread.Sleep(10)
                End If
            End While
            ' Timeout - proceed anyway (some commands may not respond)
        End Sub

        ''' <summary>
        ''' Read response from device. Returns everything before the "1\r\n" OPC marker.
        ''' </summary>
        Private Function ReadResponse(timeoutMs As Integer) As String
            Dim sb As New StringBuilder()
            Dim deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs)

            While DateTime.UtcNow < deadline
                If _port.BytesToRead > 0 Then
                    sb.Append(_port.ReadExisting())
                    Dim full = sb.ToString()
                    ' Check for OPC terminator
                    Dim opcIdx = full.IndexOf("1" & vbCrLf)
                    If opcIdx >= 0 Then
                        ' Return everything before the OPC "1\r\n"
                        Return full.Substring(0, opcIdx).Trim()
                    End If
                Else
                    Thread.Sleep(10)
                End If
            End While

            Return sb.ToString().Trim()
        End Function

        ''' <summary>
        ''' Read raw data from serial port with timeout.
        ''' </summary>
        Private Function ReadRaw(timeoutMs As Integer) As String
            Dim sb As New StringBuilder()
            Dim deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs)

            While DateTime.UtcNow < deadline
                Dim chunk = _port.ReadExisting()
                If Not String.IsNullOrEmpty(chunk) Then
                    sb.Append(chunk)
                Else
                    Thread.Sleep(20)
                End If
            End While

            Return sb.ToString()
        End Function

        Private Sub EnsureConnected()
            If Not IsConnected Then
                Throw New InvalidOperationException("PAC Power not connected. Call /pac/connect first.")
            End If
        End Sub
    End Class
End Namespace
