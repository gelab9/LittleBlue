Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Hosting
Imports System.Text.Json
Imports Microsoft.AspNetCore.Http

Imports SmartMeter.Backend.Devices
Imports System.IO.Ports



Module Program
    Private ReadOnly _daq As New Daq34970A()
    Private ReadOnly _radian As New Radian()

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

        app.MapPost("/daq/write", Async Function(ctx As HttpContext)
                              Try
                                  Dim req = Await ctx.Request.ReadFromJsonAsync(Of WriteRequest)()

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


        app.MapPost("/daq/query", Async Function(ctx As HttpContext)
                             Try
                                 Dim req = Await ctx.Request.ReadFromJsonAsync(Of QueryRequest)()

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
        app.MapPost("/daq/connect", Async Function(ctx As HttpContext)
                                Try
                                    Dim req = Await ctx.Request.ReadFromJsonAsync(Of ConnectRequest)()

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

        app.MapPost("/radian/connect", Async Function(ctx As HttpContext)
                                           Try
                                               Dim req = Await ctx.Request.ReadFromJsonAsync(Of ConnectRequest)()
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

        app.MapPost("/radian/command", Async Function(ctx As HttpContext)
                                           Try
                                               Dim req = Await ctx.Request.ReadFromJsonAsync(Of RadianCommandRequest)()
                                               If req Is Nothing OrElse String.IsNullOrWhiteSpace(req.hexCommand) Then
                                                   Return Results.BadRequest(New With {.error = "Missing hexCommand"})
                                               End If

                                               Dim response = _radian.QueryCommand(req.hexCommand, If(req.timeoutMs, 1000))
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

        app.MapPost("/radian/reset_metrics", Async Function(ctx As HttpContext)
                                                 Try
                                                     Dim req = Await ctx.Request.ReadFromJsonAsync(Of ResetMetricsRequest)()
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

        app.MapPost("/radian/start_accum", Async Function(ctx As HttpContext)
                                               Try
                                                   Dim req = Await ctx.Request.ReadFromJsonAsync(Of StartAccumRequest)()
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

        app.MapPost("/radian/start_timed_accum", Async Function(ctx As HttpContext)
                                                     Try
                                                         Dim req = Await ctx.Request.ReadFromJsonAsync(Of StartTimedAccumRequest)()
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
End Module
