Option Strict Off
Imports System.IO
Imports System.Threading
Imports System.ComponentModel
Imports System.Collections
Imports System.Math
Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Data
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Windows.Forms
Imports NationalInstruments.NI4882

Public Module GPIB_CA501TAC

    Public Class cCA501TAC
        Public GpibDevice As Device
        Public _Verbose As Boolean = False
        Public _VoltageASetPoint As String
        Public _VoltageCSetpoint As String
        Public _DeadBandSetPoint As Single 'If Current is within Dead band No Control Action is taken %Fullscale
        Public _ControlBand As Single 'Current must be within Control band (%Fullscale)
        ''' <summary>
        ''' This functions writes data to the Gpib port of the California instruments CA501TAC controller
        ''' </summary>
        ''' <param name="GpibDevice">National Instruments GPIB device controller driver</param>
        ''' <param name="StrMessage">Message to write to Instrument</param>
        ''' <param name="ErrorMessage">Error message if any</param>
        ''' <returns>0 = Success, 1 = failure</returns>
        ''' <remarks>Yang Cheng/Frank Boudreau Updated 2018</remarks>
        Public Function Write(ByVal GpibDevice As Device, ByVal StrMessage As String, Optional ByRef ErrorMessage As String = "") As Integer
            Dim success As Integer = 0 'assume success

            Try
                StrMessage = ReplaceCommonEscapeSequences(StrMessage.Trim)
                GpibDevice.Write(StrMessage)
            Catch ex As Exception
                ErrorMessage = "Write Error: " + ex.Message
                success = 1
            End Try
            Return success
        End Function

        ''' <summary>
        ''' This functions Reads datafrom the Gpib port of the California instruments CA501TAC controller
        ''' </summary>
        ''' <param name="GpibDevice">National Instruments GPIB device controller driver</param>
        ''' <param name="strMessage">Message read from Instrument</param>
        ''' <param name="ErrorMessage">Error message if any</param>
        ''' <returns>0 = Success, 1 = Failure</returns>
        ''' <remarks>Yang Cheng/Frank Boudreau Updated 2018</remarks>
        Public Overloads Function Read(ByVal GpibDevice As Device, ByRef strMessage As String, Optional ByRef ErrorMessage As String = "") As Integer

            Dim success As Integer = 0 'assume success
            strMessage = "" 'Clearbuffer
            Try
                Windows.Forms.Cursor.Current = Cursors.WaitCursor
                strMessage = ReplaceCommonEscapeSequences(GpibDevice.ReadString())
            Catch ex As Exception

                ErrorMessage = "Read Error: " + ex.Message
                success = 1
            Finally
                Windows.Forms.Cursor.Current = Cursors.Default
            End Try
            Return success
        End Function



        Private Function ReplaceCommonEscapeSequences(ByVal s As String) As String
            Return s.Replace("\n", ControlChars.Lf).Replace("\r", ControlChars.Cr)
        End Function 'ReplaceCommonEscapeSequences

        Private Function InsertCommonEscapeSequences(ByVal s As String) As String
            Return s.Replace(ControlChars.Lf, "\n").Replace(ControlChars.Cr, "\r")
        End Function 'InsertCommonEscapeSequences
    End Class
End Module
