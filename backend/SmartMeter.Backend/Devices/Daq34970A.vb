Imports System.IO.Ports
Imports System.Text

Namespace Devices

    Public Class Daq34970A
        Private _port As SerialPort

        Public ReadOnly Property IsConnected As Boolean
            Get
                Return _port IsNot Nothing AndAlso _port.IsOpen
            End Get
        End Property

        Public Function GetSettings() As Object
            If _port Is Nothing Then
                Return New With {.connected = False}
            End If

                Return New With {
                    .connected = IsConnected,
                    .port = _port.PortName,
                    .baud = _port.BaudRate,
                    .parity = _port.Parity.ToString(),
                    .dataBits = _port.DataBits,
                    .stopBits = _port.StopBits.ToString(),
                    .handshake = _port.Handshake.ToString(),
                    .dtr = _port.DtrEnable,
                    .rts = _port.RtsEnable
            }
        End Function

        Public Sub Connect(portName As String,
                           Optional baud As Integer = 9600,
                           Optional readTimeoutMs As Integer = 2000,
                           Optional writeTimeoutMs As Integer = 2000)

            Disconnect()

            _port = New SerialPort(portName, baud, Parity.None, 8, StopBits.One) With {
                .Handshake = Handshake.None,
                .DtrEnable = True,
                .RtsEnable = False,
                .ReadTimeout = readTimeoutMs,
                .WriteTimeout = writeTimeoutMs
            }

            _port.Open()
            Threading.Thread.Sleep(200)
            _port.DiscardInBuffer()
            _port.DiscardOutBuffer()
        End Sub

        Public Sub Disconnect()
            Try
                If _port IsNot Nothing Then
                    If _port.IsOpen Then _port.Close()
                    _port.Dispose()
                End If
            Catch
                ' swallow; disconnect should be safe
            Finally
                _port = Nothing
            End Try
        End Sub

        Public Sub Send(command As String)
            EnsureConnected()
            _port.Write(command & vbCrLf)
            _port.BaseStream.Flush()
        End Sub


        Public Function ReadRaw(timeoutMs As Integer) As String
            EnsureConnected()
            Dim sb As New StringBuilder()
            Dim deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs)

            While DateTime.UtcNow < deadline
                Dim chunk = _port.ReadExisting()
                If Not String.IsNullOrEmpty(chunk) Then
                    sb.Append(chunk)
                Else
                    Threading.Thread.Sleep(20)
                End If
            End While

            Return sb.ToString()
        End Function

        Public Function Query(command As String) As String
            EnsureConnected()

            Send(command)
            Threading.Thread.Sleep(150) ' give it a moment to respond

            Return ReadRaw(_port.ReadTimeout).Trim()
        End Function


        Private Sub EnsureConnected()
            If Not IsConnected Then
                Throw New InvalidOperationException("DAQ not connected. Call /daq/connect first.")
            End If
        End Sub
    End Class

End Namespace
