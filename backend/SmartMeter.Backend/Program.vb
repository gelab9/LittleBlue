Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Hosting
Imports System.Text.Json
Imports Microsoft.AspNetCore.Http

Imports SmartMeter.Backend.Devices
Imports System.IO.Ports



Module Program
    Private ReadOnly _daq As New Daq34970A()
    Private ReadOnly _radian As New Radian()
    Private ReadOnly _pac As New PacPower()
    Private ReadOnly _calInst As New CalInst()

    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)
        Dim app = builder.Build()

        app.MapGet("/health", Function()
                                  Return Results.Ok(New With {
                                      .status = "ok",
                                      .service = "SmartMeter.Backend",
                                      .utc = DateTime.UtcNow.ToString("o")
                                  })
                              End Function)

        app.MapGet("/daq/status", Function()
                              Return Results.Ok(New With {.connected = _daq.IsConnected})
                          End Function)

        app.MapPost("/daq/write", Function(req As WriteRequest)
                              Try
                                  If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.cmd) Then
                                      Return Results.BadRequest(New With {.error = "Missing JSON body or cmd"})
                                  End If

                                  If Not _daq.IsConnected Then
                                      Return Results.BadRequest(New With {.error = "DAQ not connected"})
                                  End If

                                  _daq.Send(req.cmd)
                                  Return Results.Ok(New With {.sent = req.cmd})

                              Catch ex As Exception
                                  Return Results.Problem(title:="DAQ write failed", detail:=ex.ToString())
                              End Try
                          End Function)


        app.MapPost("/daq/query", Function(req As QueryRequest)
                             Try
                                 If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.cmd) Then
                                     Return Results.BadRequest(New With {.error = "Missing JSON body or cmd"})
                                 End If

                                 If Not _daq.IsConnected Then
                                     Return Results.BadRequest(New With {.error = "DAQ not connected"})
                                 End If

                                 Dim resp = _daq.Query(req.cmd)

                                 ' IMPORTANT: return "response" so the GUI can read it
                                 Return Results.Ok(New With {.cmd = req.cmd, .response = resp})

                             Catch ex As Exception
                                 Return Results.Problem(title:="DAQ query failed", detail:=ex.ToString())
                             End Try
                         End Function)

        app.MapGet("/daq/ports", Function()
                             Return SerialPort.GetPortNames()
                         End Function)

        ' Connect
        app.MapPost("/daq/connect", Function(req As ConnectRequest)
                                Try
                                    If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.port) Then
                                        Return Results.BadRequest(New With {.error = "Missing JSON body or port"})
                                    End If

                                    _daq.Connect(req.port, req.baud, req.readTimeoutMs, req.writeTimeoutMs)
                                    Return Results.Ok(New With {.connected = _daq.IsConnected, .port = req.port, .baud = req.baud})

                                Catch ex As Exception
                                    Return Results.Problem(title:="DAQ connect failed", detail:=ex.ToString())
                                End Try
                            End Function)


        ' Disconnect
        app.MapPost("/daq/disconnect", Function()
            _daq.Disconnect()
            Return Results.Ok(New With {.connected = _daq.IsConnected})
        End Function)

        ' IDN

        app.MapGet("/daq/rawidn", Function()
                              Try
                                  ' send without clearing, then read existing after a short wait
                                  _daq.Send("*IDN?")
                                  Threading.Thread.Sleep(200)
                                  Return Results.Ok(New With {.raw = _daq.ReadRaw(2000)})
                              Catch ex As Exception
                                  Return Results.Problem(title:="DAQ rawidn failed", detail:=ex.ToString())
                              End Try
                          End Function)

        app.MapGet("/daq/idn", Function()
                           Try
                               Dim idn = _daq.Query("*IDN?")
                               Return Results.Ok(New With {.idn = idn})
                           Catch ex As Exception
                               Return Results.Problem(title:="DAQ idn failed", detail:=ex.ToString())
                           End Try
                       End Function)

        app.MapGet("/daq/err", Function()
            Dim err = _daq.Query("SYST:ERR?")
            Return Results.Ok(New With {.error = err})
        End Function)

        ' ===== RADIAN ENDPOINTS =====

        app.MapGet("/radian/status", Function()
                                         Return Results.Ok(New With {.connected = _radian.IsConnected})
                                     End Function)

        app.MapPost("/radian/connect", Function(req As ConnectRequest)
                                           Try
                                               If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.port) Then
                                                   Return Results.BadRequest(New With {.error = "Missing port"})
                                               End If

                                               _radian.Connect(req.port, req.baud)
                                               Return Results.Ok(New With {.connected = _radian.IsConnected, .port = req.port, .baud = req.baud})
                                           Catch ex As Exception
                                               Return Results.Problem(title:="Radian connect failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapPost("/radian/disconnect", Function()
                                              Try
                                                  _radian.Disconnect()
                                                  Return Results.Ok(New With {.connected = _radian.IsConnected})
                                              Catch ex As Exception
                                                  Return Results.Problem(title:="Radian disconnect failed", detail:=ex.ToString())
                                              End Try
                                          End Function)

        app.MapPost("/radian/command", Function(req As RadianCommandRequest)
                                           Try
                                               If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.hexCommand) Then
                                                   Return Results.BadRequest(New With {.error = "Missing hexCommand"})
                                               End If

                                               Dim response = _radian.QueryCommand(req.hexCommand, req.timeoutMs)
                                               Dim hexResponse = BitConverter.ToString(response).Replace("-", "")

                                               Return Results.Ok(New With {
                                                   .command = req.hexCommand,
                                                   .response = hexResponse,
                                                   .bytes = response
                                               })
                                           Catch ex As Exception
                                               Return Results.Problem(title:="Radian command failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapGet("/radian/identify", Function()
                                           Try
                                               If Not _radian.IsConnected Then
                                                   Return Results.BadRequest(New With {.error = "Radian not connected"})
                                               End If

                                               Dim response = _radian.QueryCommand(Radian.CMD_IDENTIFY, 2000)
                                               Dim hexResponse = BitConverter.ToString(response).Replace("-", "")

                                               Return Results.Ok(New With {
                                                   .response = hexResponse,
                                                   .bytes = response
                                               })
                                           Catch ex As Exception
                                               Return Results.Problem(title:="Radian identify failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapPost("/radian/reset_metrics", Function(req As ResetMetricsRequest)
                                                 Try
                                                     If req Is Nothing Then
                                                         req = New ResetMetricsRequest()
                                                     End If

                                                     Dim cmd = _radian.BuildResetMetricsCommand(
                                                         req.resetWaveform, req.resetInstant,
                                                         req.resetInstantMin, req.resetInstantMax,
                                                         req.resetAccum
                                                     )

                                                     Dim response = _radian.QueryCommand(cmd, 1000)
                                                     Dim hexResponse = BitConverter.ToString(response).Replace("-", "")

                                                     Return Results.Ok(New With {
                                                         .command = cmd,
                                                         .response = hexResponse
                                                     })
                                                 Catch ex As Exception
                                                     Return Results.Problem(title:="Reset metrics failed", detail:=ex.ToString())
                                                 End Try
                                             End Function)

        app.MapPost("/radian/start_accum", Function(req As StartAccumRequest)
                                               Try
                                                   If req Is Nothing Then
                                                       Return Results.BadRequest(New With {.error = "Missing request body"})
                                                   End If

                                                   Dim cmd = _radian.BuildStartAccumCommand(req.controlByte, req.pulseData)
                                                   Dim response = _radian.QueryCommand(cmd, 1000)
                                                   Dim hexResponse = BitConverter.ToString(response).Replace("-", "")

                                                   Return Results.Ok(New With {
                                                       .command = cmd,
                                                       .response = hexResponse
                                                   })
                                               Catch ex As Exception
                                                   Return Results.Problem(title:="Start accum failed", detail:=ex.ToString())
                                               End Try
                                           End Function)

        app.MapPost("/radian/start_timed_accum", Function(req As StartTimedAccumRequest)
                                                     Try
                                                         If req Is Nothing Then
                                                             Return Results.BadRequest(New With {.error = "Missing timeInSeconds"})
                                                         End If

                                                         Dim cmd = _radian.BuildStartTimedAccumCommand(req.timeInSeconds)
                                                         Dim response = _radian.QueryCommand(cmd, 1000)
                                                         Dim hexResponse = BitConverter.ToString(response).Replace("-", "")

                                                         Return Results.Ok(New With {
                                                             .command = cmd,
                                                             .response = hexResponse
                                                         })
                                                     Catch ex As Exception
                                                         Return Results.Problem(title:="Start timed accum failed", detail:=ex.ToString())
                                                     End Try
                                                 End Function)

        app.MapPost("/radian/stop_accum", Function()
                                              Try
                                                  Dim response = _radian.QueryCommand(Radian.CMD_STOP_ACCUM, 1000)
                                                  Dim hexResponse = BitConverter.ToString(response).Replace("-", "")

                                                  Return Results.Ok(New With {
                                                      .command = Radian.CMD_STOP_ACCUM,
                                                      .response = hexResponse
                                                  })
                                              Catch ex As Exception
                                                  Return Results.Problem(title:="Stop accum failed", detail:=ex.ToString())
                                              End Try
                                          End Function)

        ' ===== PAC POWER ENDPOINTS =====

        app.MapGet("/pac/status", Function()
                                      Return Results.Ok(New With {.connected = _pac.IsConnected})
                                  End Function)

        app.MapPost("/pac/connect", Function(req As PacConnectRequest)
                                        Try
                                            If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.port) Then
                                                Return Results.BadRequest(New With {.error = "Missing port"})
                                            End If

                                            _pac.Connect(req.port, req.baud, req.parity, req.stopBits,
                                                         req.dataBits, req.dtrEnable, req.handshake)
                                            Return Results.Ok(New With {
                                                .connected = _pac.IsConnected,
                                                .port = req.port,
                                                .baud = req.baud
                                            })
                                        Catch ex As Exception
                                            Return Results.Problem(title:="PAC Power connect failed", detail:=ex.ToString())
                                        End Try
                                    End Function)

        app.MapPost("/pac/disconnect", Function()
                                           Try
                                               _pac.Disconnect()
                                               Return Results.Ok(New With {.connected = _pac.IsConnected})
                                           Catch ex As Exception
                                               Return Results.Problem(title:="PAC Power disconnect failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapGet("/pac/identify", Function()
                                        Try
                                            Dim idn = _pac.QueryRaw("*IDN?")
                                            Return Results.Ok(New With {.idn = idn})
                                        Catch ex As Exception
                                            Return Results.Problem(title:="PAC Power IDN failed", detail:=ex.ToString())
                                        End Try
                                    End Function)

        app.MapPost("/pac/send", Function(req As WriteRequest)
                                     Try
                                         If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.cmd) Then
                                             Return Results.BadRequest(New With {.error = "Missing cmd"})
                                         End If
                                         _pac.Send(req.cmd)
                                         Return Results.Ok(New With {.sent = req.cmd})
                                     Catch ex As Exception
                                         Return Results.Problem(title:="PAC Power send failed", detail:=ex.ToString())
                                     End Try
                                 End Function)

        app.MapPost("/pac/query", Function(req As QueryRequest)
                                      Try
                                          If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.cmd) Then
                                              Return Results.BadRequest(New With {.error = "Missing cmd"})
                                          End If
                                          Dim resp = _pac.Query(req.cmd)
                                          Return Results.Ok(New With {.cmd = req.cmd, .response = resp})
                                      Catch ex As Exception
                                          Return Results.Problem(title:="PAC Power query failed", detail:=ex.ToString())
                                      End Try
                                  End Function)

        app.MapPost("/pac/set-voltage", Function(req As PacVoltageRequest)
                                            Try
                                                If req Is Nothing Then
                                                    Return Results.BadRequest(New With {.error = "Missing request body"})
                                                End If

                                                Dim cmd As String
                                                Select Case req.phase
                                                    Case 1
                                                        cmd = $":VOLT1 {req.voltage}"
                                                    Case 2
                                                        cmd = $":VOLT2 {req.voltage}"
                                                    Case 3
                                                        cmd = $":VOLT3 {req.voltage}"
                                                    Case Else
                                                        cmd = $":VOLT {req.voltage}"
                                                End Select

                                                _pac.Send(cmd)
                                                Return Results.Ok(New With {.sent = cmd, .voltage = req.voltage, .phase = req.phase})
                                            Catch ex As Exception
                                                Return Results.Problem(title:="PAC Power set-voltage failed", detail:=ex.ToString())
                                            End Try
                                        End Function)

        app.MapPost("/pac/set-frequency", Function(req As PacFrequencyRequest)
                                              Try
                                                  If req Is Nothing Then
                                                      Return Results.BadRequest(New With {.error = "Missing request body"})
                                                  End If
                                                  Dim cmd = $":FREQ {req.frequency}"
                                                  _pac.Send(cmd)
                                                  Return Results.Ok(New With {.sent = cmd, .frequency = req.frequency})
                                              Catch ex As Exception
                                                  Return Results.Problem(title:="PAC Power set-frequency failed", detail:=ex.ToString())
                                              End Try
                                          End Function)

        app.MapPost("/pac/output-on", Function()
                                          Try
                                              _pac.Send(":OUTP ON")
                                              Return Results.Ok(New With {.output = "ON"})
                                          Catch ex As Exception
                                              Return Results.Problem(title:="PAC Power output-on failed", detail:=ex.ToString())
                                          End Try
                                      End Function)

        app.MapPost("/pac/output-off", Function()
                                           Try
                                               _pac.Send(":OUTP OFF")
                                               Return Results.Ok(New With {.output = "OFF"})
                                           Catch ex As Exception
                                               Return Results.Problem(title:="PAC Power output-off failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapPost("/pac/set-current-limit", Function(req As PacCurrentLimitRequest)
                                                  Try
                                                      If req Is Nothing Then
                                                          Return Results.BadRequest(New With {.error = "Missing request body"})
                                                      End If
                                                      Dim cmd = $":CURR:LIM {req.limit}"
                                                      _pac.Send(cmd)
                                                      Return Results.Ok(New With {.sent = cmd, .limit = req.limit})
                                                  Catch ex As Exception
                                                      Return Results.Problem(title:="PAC Power set-current-limit failed", detail:=ex.ToString())
                                                  End Try
                                              End Function)

        app.MapGet("/pac/measure/voltage", Function()
                                               Try
                                                   Dim v1 = _pac.Query(":MEAS:VOLT1?")
                                                   Dim v2 = _pac.Query(":MEAS:VOLT2?")
                                                   Dim v3 = _pac.Query(":MEAS:VOLT3?")
                                                   Return Results.Ok(New With {.volt1 = v1, .volt2 = v2, .volt3 = v3})
                                               Catch ex As Exception
                                                   Return Results.Problem(title:="PAC Power measure voltage failed", detail:=ex.ToString())
                                               End Try
                                           End Function)

        app.MapGet("/pac/measure/current", Function()
                                               Try
                                                   Dim i1 = _pac.Query("MEAS:CURR:RMS1?")
                                                   Dim i2 = _pac.Query("MEAS:CURR:RMS2?")
                                                   Dim i3 = _pac.Query("MEAS:CURR:RMS3?")
                                                   Return Results.Ok(New With {.curr1 = i1, .curr2 = i2, .curr3 = i3})
                                               Catch ex As Exception
                                                   Return Results.Problem(title:="PAC Power measure current failed", detail:=ex.ToString())
                                               End Try
                                           End Function)

        app.MapGet("/pac/measure/frequency", Function()
                                                 Try
                                                     Dim freq = _pac.Query("MEAS:FREQ?")
                                                     Return Results.Ok(New With {.frequency = freq})
                                                 Catch ex As Exception
                                                     Return Results.Problem(title:="PAC Power measure frequency failed", detail:=ex.ToString())
                                                 End Try
                                             End Function)

        app.MapGet("/pac/measure/power", Function()
                                             Try
                                                 Dim p1 = _pac.Query("MEAS:CURR:POW1?")
                                                 Dim p2 = _pac.Query("MEAS:CURR:POW2?")
                                                 Dim p3 = _pac.Query("MEAS:CURR:POW3?")
                                                 Dim kva1 = _pac.Query("MEAS:CURR:KVA1?")
                                                 Dim kva2 = _pac.Query("MEAS:CURR:KVA2?")
                                                 Dim kva3 = _pac.Query("MEAS:CURR:KVA3?")
                                                 Dim pf1 = _pac.Query("MEAS:CURR:PF1?")
                                                 Dim pf2 = _pac.Query("MEAS:CURR:PF2?")
                                                 Dim pf3 = _pac.Query("MEAS:CURR:PF3?")
                                                 Return Results.Ok(New With {
                                                     .power1 = p1, .power2 = p2, .power3 = p3,
                                                     .kva1 = kva1, .kva2 = kva2, .kva3 = kva3,
                                                     .pf1 = pf1, .pf2 = pf2, .pf3 = pf3
                                                 })
                                             Catch ex As Exception
                                                 Return Results.Problem(title:="PAC Power measure power failed", detail:=ex.ToString())
                                             End Try
                                         End Function)

        app.MapGet("/pac/measure/all", Function()
                                           Try
                                               Dim v1 = _pac.Query(":MEAS:VOLT1?")
                                               Dim freq = _pac.Query("MEAS:FREQ?")
                                               Dim i1 = _pac.Query("MEAS:CURR:RMS1?")
                                               Dim p1 = _pac.Query("MEAS:CURR:POW1?")
                                               Dim kva1 = _pac.Query("MEAS:CURR:KVA1?")
                                               Dim pf1 = _pac.Query("MEAS:CURR:PF1?")
                                               Return Results.Ok(New With {
                                                   .volt = v1,
                                                   .frequency = freq,
                                                   .current = i1,
                                                   .power = p1,
                                                   .kva = kva1,
                                                   .pf = pf1
                                               })
                                           Catch ex As Exception
                                               Return Results.Problem(title:="PAC Power measure all failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        ' ===== CAL INST (GPIB) ENDPOINTS =====

        app.MapGet("/cal-inst/status", Function()
                                           Return Results.Ok(New With {
                                               .connected = _calInst.IsConnected,
                                               .voltageSetPoint = _calInst.VoltageASetPoint
                                           })
                                       End Function)

        app.MapPost("/cal-inst/connect", Function(req As CalInstConnectRequest)
                                             Try
                                                 If req Is Nothing Then
                                                     Return Results.BadRequest(New With {.error = "Missing request body"})
                                                 End If

                                                 _calInst.Connect(req.boardId, req.primaryAddress, req.secondaryAddress)
                                                 Dim timeout = _calInst.GetTimeout()
                                                 Return Results.Ok(New With {
                                                     .connected = _calInst.IsConnected,
                                                     .boardId = req.boardId,
                                                     .primaryAddress = req.primaryAddress,
                                                     .secondaryAddress = req.secondaryAddress,
                                                     .timeout = timeout
                                                 })
                                             Catch ex As Exception
                                                 Return Results.Problem(title:="Cal Inst connect failed", detail:=ex.ToString())
                                             End Try
                                         End Function)

        app.MapPost("/cal-inst/disconnect", Function()
                                                Try
                                                    _calInst.Disconnect()
                                                    Return Results.Ok(New With {.connected = _calInst.IsConnected})
                                                Catch ex As Exception
                                                    Return Results.Problem(title:="Cal Inst disconnect failed", detail:=ex.ToString())
                                                End Try
                                            End Function)

        app.MapPost("/cal-inst/write", Function(req As WriteRequest)
                                           Try
                                               If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.cmd) Then
                                                   Return Results.BadRequest(New With {.error = "Missing cmd"})
                                               End If
                                               _calInst.Write(req.cmd)
                                               Return Results.Ok(New With {.sent = req.cmd})
                                           Catch ex As Exception
                                               Return Results.Problem(title:="Cal Inst write failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapPost("/cal-inst/query", Function(req As QueryRequest)
                                           Try
                                               If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.cmd) Then
                                                   Return Results.BadRequest(New With {.error = "Missing cmd"})
                                               End If
                                               Dim resp = _calInst.Query(req.cmd)
                                               Return Results.Ok(New With {.cmd = req.cmd, .response = resp})
                                           Catch ex As Exception
                                               Return Results.Problem(title:="Cal Inst query failed", detail:=ex.ToString())
                                           End Try
                                       End Function)

        app.MapPost("/cal-inst/set-voltage", Function(req As CalInstVoltageRequest)
                                                 Try
                                                     If req Is Nothing Then
                                                         Return Results.BadRequest(New With {.error = "Missing request body"})
                                                     End If
                                                     _calInst.SetVoltage(req.voltage)
                                                     Return Results.Ok(New With {
                                                         .voltage = req.voltage,
                                                         .setPoint = _calInst.VoltageASetPoint
                                                     })
                                                 Catch ex As Exception
                                                     Return Results.Problem(title:="Cal Inst set-voltage failed", detail:=ex.ToString())
                                                 End Try
                                             End Function)

        app.MapPost("/cal-inst/voltage-off", Function()
                                                 Try
                                                     _calInst.VoltageOff()
                                                     Return Results.Ok(New With {
                                                         .voltage = "0.0",
                                                         .setPoint = _calInst.VoltageASetPoint
                                                     })
                                                 Catch ex As Exception
                                                     Return Results.Problem(title:="Cal Inst voltage-off failed", detail:=ex.ToString())
                                                 End Try
                                             End Function)

        app.Run("http://127.0.0.1:5055")
    End Sub

    Public Class WriteRequest
        Public Property cmd As String
    End Class

    Public Class QueryRequest
        Public Property cmd As String
    End Class

    Public Class ConnectRequest
        Public Property port As String
        Public Property baud As Integer = 9600
        Public Property readTimeoutMs As Integer = 2000
        Public Property writeTimeoutMs As Integer = 2000
    End Class

    Public Class RadianCommandRequest
        Public Property hexCommand As String
        Public Property timeoutMs As Integer = 1000
    End Class

    Public Class ResetMetricsRequest
        Public Property resetWaveform As Boolean = False
        Public Property resetInstant As Boolean = False
        Public Property resetInstantMin As Boolean = False
        Public Property resetInstantMax As Boolean = False
        Public Property resetAccum As Boolean = True
    End Class

    Public Class StartAccumRequest
        Public Property controlByte As Byte = &H1
        Public Property pulseData As UShort = 0
    End Class

    Public Class StartTimedAccumRequest
        Public Property timeInSeconds As Single
    End Class

    ' ===== PAC Power Request Classes =====

    Public Class PacConnectRequest
        Public Property port As String
        Public Property baud As Integer = 9600
        Public Property parity As Parity = Parity.None
        Public Property stopBits As StopBits = StopBits.One
        Public Property dataBits As Integer = 8
        Public Property dtrEnable As Boolean = True
        Public Property handshake As Handshake = Handshake.None
    End Class

    Public Class PacVoltageRequest
        Public Property voltage As Single
        Public Property phase As Integer = 0  ' 0 = all, 1/2/3 = specific phase
    End Class

    Public Class PacFrequencyRequest
        Public Property frequency As Single
    End Class

    Public Class PacCurrentLimitRequest
        Public Property limit As Single
    End Class

    ' ===== Cal Inst Request Classes =====

    Public Class CalInstConnectRequest
        Public Property boardId As Integer = 0
        Public Property primaryAddress As Integer = 1
        Public Property secondaryAddress As Integer = 0
    End Class

    Public Class CalInstVoltageRequest
        Public Property voltage As Single
    End Class
End Module
