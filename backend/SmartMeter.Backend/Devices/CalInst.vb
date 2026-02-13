''' <summary>
''' California Instruments CA501TAC GPIB Communication Driver
''' Based on original GPIB_CA501TAC.vb
''' Uses NationalInstruments.NI4882 for GPIB bus communication.
''' </summary>

Imports NationalInstruments.NI4882

Namespace Devices
    Public Class CalInst
        Private _device As Device
        Private _isConnected As Boolean = False

        Public Property VoltageASetPoint As String = "0.0"
        Public Property VoltageCSetpoint As String = "0.0"

        Public ReadOnly Property IsConnected As Boolean
            Get
                Return _isConnected AndAlso _device IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Open GPIB connection to CA501TAC controller.
        ''' </summary>
        ''' <param name="boardId">GPIB board ID (typically 0)</param>
        ''' <param name="primaryAddress">Instrument primary address (0-30)</param>
        ''' <param name="secondaryAddress">Instrument secondary address (0 = none)</param>
        Public Sub Connect(boardId As Integer, primaryAddress As Integer, Optional secondaryAddress As Integer = 0)
            Try
                Disconnect()

                _device = New Device(boardId, CByte(primaryAddress), CByte(secondaryAddress))
                _isConnected = True

                ' Turn voltage off on connect (safety)
                VoltageOff()

            Catch ex As Exception
                _isConnected = False
                Throw New Exception($"Failed to connect to Cal Inst (Board={boardId}, Addr={primaryAddress}): {ex.Message}")
            End Try
        End Sub

        ''' <summary>
        ''' Close GPIB connection.
        ''' </summary>
        Public Sub Disconnect()
            Try
                If _device IsNot Nothing Then
                    _device.Dispose()
                End If
                _isConnected = False
            Catch
                ' swallow; disconnect should be safe
            Finally
                _device = Nothing
                _isConnected = False
            End Try
        End Sub

        ''' <summary>
        ''' Write a command to the GPIB device.
        ''' </summary>
        Public Sub Write(message As String)
            EnsureConnected()
            Try
                Dim cleaned = ReplaceCommonEscapeSequences(message.Trim())
                _device.Write(cleaned)
            Catch ex As Exception
                Throw New Exception($"GPIB Write Error: {ex.Message}")
            End Try
        End Sub

        ''' <summary>
        ''' Read a response from the GPIB device.
        ''' </summary>
        Public Function Read() As String
            EnsureConnected()
            Try
                Return ReplaceCommonEscapeSequences(_device.ReadString()).Trim()
            Catch ex As Exception
                Throw New Exception($"GPIB Read Error: {ex.Message}")
            End Try
        End Function

        ''' <summary>
        ''' Write a command and read the response.
        ''' </summary>
        Public Function Query(command As String) As String
            Write(command)
            Threading.Thread.Sleep(100)
            Return Read()
        End Function

        ''' <summary>
        ''' Set voltage on phase A using AMPA command.
        ''' </summary>
        Public Sub SetVoltage(voltage As Single)
            Write($"AMPA {voltage:F1}")
            Threading.Thread.Sleep(100)
            VoltageASetPoint = voltage.ToString("F1")
        End Sub

        ''' <summary>
        ''' Turn voltage off (set to 0.0V).
        ''' </summary>
        Public Sub VoltageOff()
            Write("AMPA 0.0")
            Threading.Thread.Sleep(100)
            VoltageASetPoint = "0.0"
        End Sub

        ''' <summary>
        ''' Get the GPIB timeout setting.
        ''' </summary>
        Public Function GetTimeout() As String
            EnsureConnected()
            Return _device.IOTimeout.ToString()
        End Function

        Private Function ReplaceCommonEscapeSequences(s As String) As String
            Return s.Replace("\n", ControlChars.Lf).Replace("\r", ControlChars.Cr)
        End Function

        Private Sub EnsureConnected()
            If Not IsConnected Then
                Throw New InvalidOperationException("Cal Inst not connected. Call /cal-inst/connect first.")
            End If
        End Sub
    End Class
End Namespace
