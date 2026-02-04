Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Hosting
Imports Microsoft.AspNetCore.Http

Imports SmartMeter.Backend.Devices
Imports System.IO.Ports



Module Program
    Private ReadOnly _daq As New Daq34970A()

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
                                    _daq.Send(req.cmd)
                                    Return Results.Ok(New With {.sent = req.cmd})
                                Catch ex As Exception
                                    Return Results.Problem(title:="DAQ write failed", detail:=ex.ToString())
                                End Try
                            End Function)

        app.MapGet("/daq/ports", Function()
                             Return SerialPort.GetPortNames()
                         End Function)

                                      ' Connect
        app.MapPost("/daq/connect", Function(req As ConnectRequest)
                                Try
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

        app.Run("http://127.0.0.1:5055")
    End Sub

    Public Class WriteRequest
                    Public Property cmd As String
                End Class
        Public Class ConnectRequest
        
        Public Property port As String
        Public Property baud As Integer = 9600
        Public Property readTimeoutMs As Integer = 2000
        Public Property writeTimeoutMs As Integer = 2000
    End Class
End Module
