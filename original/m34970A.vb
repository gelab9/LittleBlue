Option Strict Off
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.ComponentModel
Imports System.IO
Imports System.IO.Ports.SerialPort
Imports System.Text.RegularExpressions
Imports System.Math
Imports Microsoft.Office

Public Module m34970A
    Public Class Agilent34970A
        ''' <summary>
        ''' Transmit and recieve buffer to communicate with Radian
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landi+Gyr</remarks>
        Public aBytes(2047) As Byte
        'General all purpose buffer for passing data in out of Class
        Public strBuffer As String
        Public ControlByteStatus As Byte
        Public PulsesLeft As UShort
        Public NumberOfTestsToRun As Integer
        Public LoopTest As Boolean
        Public StartTime
        Public EndTIme
        Public Interval
        ' Public RelayBits As sChannelUShort
        Public TimeLeftForTest As Single
        'Public WithEvents bgw34970AComm As BackgroundWorker
        Public IsConnected As Boolean = False ' 
        Public numChannel As Integer = 60 ' Total number of channels (20 channels on each module and there are 3 modules)
        Public Enum eMessageState
            IDLE
            WaitingForData
            DataRecieved
        End Enum

        Public Structure sRecieveBufferCOntrol
            Public MessageState As Integer
            Public Offset As Integer
            Public Length As Integer
            Public BytesToRead As Integer
            Public Timeout As Integer
            Public Ticks As Integer
        End Structure

        Public ReceiveBufferControl As sRecieveBufferCOntrol

        Public Const RX_BUFFER_SIZE = 2048
        ''' <summary>
        ''' Serial port for communicating With test Equipment
        ''' </summary>
        ''' <remarks>Frank Boudreau 2014 Landis+Gyr</remarks>
        Public WithEvents SerialPort As New System.IO.Ports.SerialPort


        ' 34970A Messages
        'Public Const NOP As String = "A6000000"
        Structure sMESSAGE
            'IEEE-488 Common Commands
            Public Const QUERY_IDENTIFY As String = "*IDN?"
            Public Const CLEAR_EVENT_REGISTERS As String = "*CLS" 'CLear Status
            Public Const ENABLE_BIT_STANDARD_EVENT_REGISTER As String = "*ESE" '*ESE <Enable Value>
            Public Const QUERY_BIT_STANDARD_EVENT_REGISTER As String = "*ESE?"
            Public Const START_OPC_STATE_ENGINE As String = "*OPC"
            Public Const QUERY_OPC_STATE_ENGINE As String = "*OPC?"
            Public Const POWER_ON_STATUS_CLEAR_SETTING As String = "*PSC" '*PSC <state>
            Public Const QUERY_POWER_ON_STATUS_CLEAR_SETTING As String = "*PSC?"
            Public Const SAVE_INTSRTUMENT_STATE As String = "*SAV" ' *SAV {0|1|2|3|4|5}
            Public Const RECALL_INSTRUMENT_STATE As String = "*RCL" ' *RCL {0|1|2|3|4|5}
            Public Const RESET_TO_FACTORY_STATE As String = "*RST"
            Public Const ENABLE_BIT_STATUS_BYTE_REGISTER As String = "*SRE" ' *SRE <Enable Value>
            Public Const QUERY_BIT_STAUS_BYTE_REGISTER As String = "*STB?"
            Public Const TRIGGER_MEASUREMENT As String = "*TRG"
            Public Const SELF_TEST As String = "*TST?"
            Public Const WAIT As String = "*WAI"
            'INSTRUMENT Sub System
            Public Const DMM_ON As String = "INST:DMM 1"
            Public Const DMM_OFF As String = "INST:DMM 0"
            Public Const QUERY_DMM_STATE As String = "INST:DMM?"
            Public Const QUERY_DMM_INSTALLED As String = "INST:DMM:INST?"

            ' Added by Yang on 2/3/2016
            Public Const INITIATE As String = "INITiate"
            Public Const GET_DATE As String = "SYSTem:DATE?"
            Public Const GET_TIME As String = "SYSTem:TIME?"
            Public Const NUM_CHANNELS As String = "ROUTe:SCAN:SIZE?"
            Public Const NUM_READINGS As String = "TRIGger:COUNt?"
            Public Const TRIGGER_TIMER As String = "TRIGger:TIMer?"
            Public Const DATA_POINTS As String = "DATA:POINTS?" ' Determine the total number of readings stored in memory (all channels) from the most recent scan
            Public Const READING_VALUE1 As String = "DATA:REMove? 1"
            Public Const CHECK_MODULE_1 As String = "SYSTem:CTYPe? 100" ' Check if Module 1 installed
            Public Const CHECK_MODULE_2 As String = "SYSTem:CTYPe? 200" ' Check if Module 2 installed
            Public Const CHECK_MODULE_3 As String = "SYSTem:CTYPe? 300" ' Check if Module 3 installed
            Public Const READ_ERASE As String = "R?"
            Public Const CHECK_ERROR As String = "SYSTem:ERRor?"
        End Structure

        ''' <summary>
        ''' Declare the structure of Channel
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure Channel
            Public channelEnable As Boolean
            Public channelName As String
            Public channelNumber As String
            Public enableCheckBox As CheckBox
            Public nameComboBox As ComboBox
            Public offsetTextBox As TextBox
            Public gainTextBox As TextBox
        End Structure

        ''' <summary>
        ''' Declare the structure of Compare
        ''' </summary>
        ''' <remarks></remarks>
        Public Structure Compare
            Public compare1 As ComboBox
            Public compare2 As ComboBox
            Public compareValue1 As String
            Public compareValue2 As String
            Public label1 As Label
            Public label2 As Label
            Public warning As Boolean
            Public threshold As TextBox
            Public oldThreshold As Single
            Public difference As TextBox
            Public notification As CheckBox
            Public startTicks As Integer
            Public endTicks As Integer
        End Structure

        Public channelArray(numChannel - 1) As Channel ' Declare an array of structure
        Public MESSAGE As New sMESSAGE
        Public compareArray(4) As Compare
        ''' <summary>
        ''' Class to encapsulate Identifaction Data From the Device, once parsed.
        ''' </summary>
        ''' <remarks></remarks>
        Public Class cAgilent34970AParameters
            Public Manufacturer As String
            Public ModelNumber As String
            Public SerialNumber As String
            Public Firmware As String
            Public Sub init()
                Manufacturer = ""
                ModelNumber = ""
                SerialNumber = ""
                Firmware = ""
            End Sub
        End Class

        Public Class cAgilent34970AReadings
            Public numReadings As Long        ' Used for the number of readings taken
            Public numChannel As Integer         ' Used to determine the number of channels scanned 
            Public dataPoints As Double      ' The number of points in the memory of the instrument
            Public checkModule As String() = New String(2) {"", "", ""}   ' Check the module 34901A is correctly installed in the instrument
            Public totalReadings As Integer  ' Indicate the total number of readings that has been taken
            Public timerCountDown As Boolean = False ' The flag if the duration has already met the time specified by the user
            Public readyToStop As Boolean = False ' The flag that indicates the program is ready to stop
            Public allReadings As String    ' The string that is returned directly from the instrument
            Public readingInterval As String ' The amount of time between the scans of the channels for each sweep
            Public totalNumofReadings As Integer ' For Number of Reading mode, the total number of readings that needs to be taken
            Public errorMessage As String    ' The error message returned from the device
            Public TempCurrentReading As New List(Of String)
            Public TempMaxReading As New List(Of String) ' Array list containing the maximum of each channel
            Public TempMinReading As New List(Of String) ' Array list containing the minimum of each channel
            Public TempAveReading As New List(Of String) ' Array list containing the average of each channel
            Public TempSumReading As New List(Of String) ' Array list containing the Sum of each channel
            Public TempRiseReading As New List(Of String) ' Array list containing the temperature risen of each channel
            Public hour As Integer ' in duration mode, hours
            Public minute As Integer ' in duration mode, minutes
            Public second As Integer ' in duration mode, seconds
        End Class

        Public Class cAgilent34970AmyProgram
            Public DeviceRunning As Boolean 'True: Running, False: Not Running
            Public MeasureDevice As Integer '0: RTD(4W), 1:Thermocouple
            Public Mode As Integer '0: Free, 1: Duration, 2: Number of Readings
        End Class
        ''' <summary>
        ''' Class enclapulates parsing and control of the reply from the Standard Event Register Query on the 34907A
        ''' </summary>
        ''' <remarks></remarks>
        Public Class cStandardAgilentEvenRegister
            Public Value As UInteger
            Enum eEventRegisterMask
                OPERATION_COMPLETE = 1
                NOT_USED_BIT_1 = 2 'This Bit Position should always return 0 So if this mask to a TRUE then there is a Reply error
                QUERY_ERROR = 4
                DEVICE_ERROR = 8
                EXECUTION_ERROR = 16
                COMMAND_ERROR = 32
                NOT_USED_BIT_6 = 64 'This Bit Position should always return 0 So if this mask to a TRUE then there is reply error
                POWER_CYCLE_OCCURED = 128
            End Enum
            ''' <summary>
            ''' 'All commands prior to and including *OPC have been executed.
            ''' </summary>
            ''' <remarks></remarks>
            Public OperationComplete As Boolean
            ''' <summary>
            ''' The instrument tried to read the output buffer but it was empty. Or, a new command line was received before a previous query has been read. Or, both the input and output buffers are full.
            ''' </summary>
            ''' <remarks></remarks>
            Public QueryError As Boolean
            ''' <summary>
            ''' 'A device-specific error has been generated. For a complete listing of the error messages, see Error Messages.
            ''' </summary>
            ''' <remarks></remarks>
            Public DeviceError As Boolean
            ''' <summary>
            ''' 'An execution error occurred. These error codes are in the range -100 to -199.
            ''' </summary>
            ''' <remarks></remarks>
            Public ExecutionError As Boolean
            ''' <summary>
            ''' 'An command error occurred. These error codes are in the range -200 to -299.
            ''' </summary>
            ''' <remarks></remarks>
            Public CommandError As Boolean
            ''' <summary>
            ''' 'Power has been turned off and on since the last time the event register was read or cleared.
            ''' </summary>
            ''' <remarks></remarks>
            Public PowerCycleHasOccured As Boolean
            ''' <summary>
            ''' 'If either Bit 1 or Bit 6 Returns True this Flag is Set
            ''' </summary>
            ''' <remarks></remarks>
            Public FlagError As Boolean
            Public Sub GetFlags()
                ClearFlags()    'Clear Flags

                If Value And eEventRegisterMask.COMMAND_ERROR Then
                    CommandError = True
                End If

                If Value And eEventRegisterMask.DEVICE_ERROR Then
                    DeviceError = True
                End If

                If Value And eEventRegisterMask.EXECUTION_ERROR Then
                    ExecutionError = True
                End If

                If Value And eEventRegisterMask.OPERATION_COMPLETE Then
                    OperationComplete = True
                End If

                If Value And eEventRegisterMask.POWER_CYCLE_OCCURED Then
                    PowerCycleHasOccured = True
                End If

                If Value And eEventRegisterMask.QUERY_ERROR Then
                    QueryError = True
                End If

                If Value And (eEventRegisterMask.NOT_USED_BIT_6 Or eEventRegisterMask.NOT_USED_BIT_1) Then
                    FlagError = True
                End If
            End Sub
            Public Sub ClearFlags()
                OperationComplete = False
                QueryError = False
                DeviceError = False
                ExecutionError = False
                CommandError = False
                PowerCycleHasOccured = False
                FlagError = False
            End Sub
            Public Sub init()
                ClearFlags()
                Value = 0
            End Sub
        End Class

        ''' <summary>
        ''' Parses the reply to the Operation Complete Query form (*OPC?)
        ''' The command form (*OPC) Sets bit 0 of the Standad Event Register on completion instead
        ''' </summary>
        ''' <remarks> </remarks>
        Public Class cOPerationCompleteQuery
            Public Value As UInteger
            Public Const OPERATION_COMPLETE = 1
            Public OperationComplete As Boolean
            Public Sub GetFlag()
                ClearFlag()
                If Value And OPERATION_COMPLETE Then
                    OperationComplete = True
                End If
            End Sub
            Public Sub ClearFlag()
                OperationComplete = False
            End Sub
            Public Sub init()
                ClearFlag()
                Value = 0
            End Sub
        End Class


        Public Parameters As New cAgilent34970AParameters
        Public ReadingsVal As New cAgilent34970AReadings
        Public myProgram As New cAgilent34970AmyProgram

        ''' <summary>
        ''' This function Configures the Comport for communicaiton with the target device
        ''' </summary>
        ''' <param name="iBaudrate">Integer Baudrate of Serial Port</param>
        ''' <param name="Parity">Enumeration</param>
        ''' <param name="StopBits">Enumeration</param>
        ''' <param name="iDataBits">Integer 7, 8, or 9 (8 is typical)</param>
        ''' <param name="strComPort">String as returned by System.IO.Ports.SerialPort.getportsname() to set comport</param>
        ''' <param name="bDTREnable">Boolean; If True uses DTR during communication </param>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Sub v_InitSerialPort(iBaudrate As Integer, Parity As Ports.Parity, StopBits As Ports.StopBits, iDataBits As Integer, strComPort As String, Optional bDTREnable As Boolean = False)
            Try
                'SerialPort.InitializeLifetimeService()
                SerialPort.BaudRate = iBaudrate
                SerialPort.Parity = Parity
                SerialPort.StopBits = StopBits
                SerialPort.PortName = strComPort
                SerialPort.DataBits = iDataBits
                SerialPort.DtrEnable = bDTREnable
            Catch ex As Exception
                Throw New Exception("Unable to Configure Comport" + vbLf + ex.ToString)
            End Try
        End Sub
        ''' <summary>
        ''' Generic Transmit routine.
        ''' </summary>
        ''' <param name="aBytes">Byte array of Data to send</param>
        ''' <remarks></remarks>
        Public Sub v_Transmit(ByRef abytes() As Byte, ByVal expectReceive As Boolean)
            Try
                SerialPort.DiscardInBuffer()
                SerialPort.DiscardOutBuffer()
                SerialPort.Write(abytes, 0, abytes.Length)
                ReDim abytes(RX_BUFFER_SIZE - 1)
                ReceiveBufferControl.MessageState = eMessageState.WaitingForData
                ReceiveBufferControl.Offset = 0
            Catch ex As Exception
                Throw New Exception("In v_Transmit(): " + ex.ToString)
            End Try


        End Sub


        ''' <summary>
        ''' Close the comport to the Agilent 34970A
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Sub v_Disconnect()
            Try
                If SerialPort.IsOpen Then
                    SerialPort.Close()
                End If
                IsConnected = False
            Catch
                Throw New Exception("Access to Serial Port Denied")
            End Try

        End Sub
        ''' <summary>
        ''' Open the comport to the Agilent 34970A
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Sub v_Connect()
            Try
                ' Close the serial port if it is open
                If SerialPort.IsOpen Then
                    SerialPort.Close()
                End If
                SerialPort.DtrEnable = True
                SerialPort.Handshake = Ports.Handshake.RequestToSend
                SerialPort.Open()

                ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                Dim iTimeout As Integer = ReceiveBufferControl.Ticks + 2000 '200mS
                'purge buffer of junk
                While SerialPort.BytesToRead > 0 And ReceiveBufferControl.Ticks < iTimeout
                    SerialPort.DiscardInBuffer()
                    ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                End While

            Catch
                Throw New Exception("Access to Serial Port Denied")

            End Try

        End Sub

        Public Sub b_ReceiveData() Handles SerialPort.DataReceived
            'Dim iBytesToRead As Integer = SerialPort.BytesToRead
            'Dim iOffset As Integer = 0
            Select Case ReceiveBufferControl.MessageState
                Case eMessageState.IDLE
                    'PC is the master if eMessage state is IDLE then no message is expected so flush
                    SerialPort.DiscardInBuffer()
                    ReceiveBufferControl.Offset = 0
                Case Else
                    'PC is expecting data
                    'Move the data from the serial port to the message buffer aBytes
                    If aBytes.Length < RX_BUFFER_SIZE Then
                        ReDim Preserve aBytes(RX_BUFFER_SIZE - 1)
                    End If
                    ReceiveBufferControl.Timeout = ReceiveBufferControl.Timeout + 30 'Add 30 mS to time out everytime data received
                    ReceiveBufferControl.BytesToRead = SerialPort.BytesToRead 'get the number of bytes to read
                    Try

                   
                    SerialPort.Read(aBytes, ReceiveBufferControl.Offset, ReceiveBufferControl.BytesToRead) ' Reads a number of bytes from the SerialPort input buffer and writes those bytes into a byte array at the specified offset
                    'The 34970A terminate each message with Line feed
                    For i = ReceiveBufferControl.Offset To (aBytes.Length - 1)
                        If aBytes(i) = &HA Then
                            ReceiveBufferControl.MessageState = eMessageState.IDLE
                            Exit For
                        End If
                    Next

                    ReceiveBufferControl.Offset = ReceiveBufferControl.Offset + ReceiveBufferControl.BytesToRead
                    If ReceiveBufferControl.MessageState = eMessageState.IDLE Then
                        ReDim Preserve aBytes(ReceiveBufferControl.Offset - 1)
                    End If
                    Catch ex As Exception

                    End Try
            End Select
        End Sub

        ''' <summary>
        ''' Reads available Buffer as String
        ''' This is timer driven could change to an event driven model
        ''' </summary>
        ''' <returns>Success = true; Fail = false</returns>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Function b_Receive(ByRef LocalaBytes() As Byte) As Boolean
            'assume success
            b_Receive = True

            Try
                Dim bReceiveTimeOutError As Boolean = False
                ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                ReceiveBufferControl.Timeout = ReceiveBufferControl.Ticks + 200 '200mS = 0.2 seconds
                While ReceiveBufferControl.Ticks < ReceiveBufferControl.Timeout And ReceiveBufferControl.MessageState <> eMessageState.IDLE
                    Application.DoEvents()
                    ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                End While
                If ReceiveBufferControl.Ticks > ReceiveBufferControl.Timeout Then
                    bReceiveTimeOutError = True
                End If
                If aBytes.Length < 4 Then   'minimum message length is 4
                    Throw New Exception("Unable to recieve Message; check and make sure Device is connected to the comport")
                ElseIf bReceiveTimeOutError Then
                    Throw New Exception("Recieve Time Out")
                End If
            Catch ex As Exception
                frmCurrent_Temperature.v_UpdateError(ex.ToString)
                b_Receive = False
            End Try
            Return b_receive
        End Function


        ''' <summary>
        ''' Parse the message from the agilent, dependent on the command sent to the device at moment
        ''' </summary>
        ''' <param name="strMessage">strMessage: String to be parsed</param>
        ''' <param name="strCommand">strCommand: Command Sent to Device</param>
        ''' <remarks>Frank Boudreau 2014</remarks>
        Public Sub v_ParseAgilent34970Data(strMessage As String, Optional ByVal strCommand As String = m34970A.Agilent34970A.sMESSAGE.QUERY_IDENTIFY)
            Dim aString() As String  'String array
            aString = strMessage.Split(",")
            Try
                Select Case strCommand
                    Case m34970A.Agilent34970A.sMESSAGE.QUERY_IDENTIFY
                        If aString.Length = 4 Then
                            Parameters.Manufacturer = aString(0)
                            Parameters.ModelNumber = aString(1)
                            Parameters.SerialNumber = aString(2)
                            Parameters.Firmware = aString(3)
                        Else 'unexpected reply length
                            Throw New Exception("*IDN?: Unexpected Reply message length. Expected 4 Values . " + aString.Length + " Recieved.")
                        End If
                    Case m34970A.Agilent34970A.sMESSAGE.QUERY_BIT_STANDARD_EVENT_REGISTER
                    Case m34970A.Agilent34970A.sMESSAGE.QUERY_BIT_STAUS_BYTE_REGISTER
                    Case m34970A.Agilent34970A.sMESSAGE.QUERY_DMM_INSTALLED
                    Case m34970A.Agilent34970A.sMESSAGE.QUERY_DMM_STATE                  
                    Case m34970A.Agilent34970A.sMESSAGE.QUERY_POWER_ON_STATUS_CLEAR_SETTING                    
                    Case m34970A.Agilent34970A.sMESSAGE.NUM_CHANNELS
                        ReadingsVal.numChannel = Val(getData(strMessage))
                    Case m34970A.Agilent34970A.sMESSAGE.DATA_POINTS
                        ReadingsVal.dataPoints = Val(getData(strMessage))
                    Case m34970A.Agilent34970A.sMESSAGE.CHECK_MODULE_1
                        ReadingsVal.checkModule(0) = getData(strMessage)
                    Case m34970A.Agilent34970A.sMESSAGE.CHECK_MODULE_2
                        ReadingsVal.checkModule(1) = getData(strMessage)
                    Case m34970A.Agilent34970A.sMESSAGE.CHECK_MODULE_3
                        ReadingsVal.checkModule(2) = getData(strMessage)
                    Case m34970A.Agilent34970A.sMESSAGE.READ_ERASE
                        ReadingsVal.allReadings = getData(strMessage)
                    Case m34970A.Agilent34970A.sMESSAGE.CHECK_ERROR
                        ReadingsVal.errorMessage = getData(strMessage)
                End Select

            Catch ex As Exception
                Throw New Exception(ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Format the data received from the device
        ''' </summary>
        ''' <param name="strMessage">Message received from the device</param>
        ''' <returns>Formatted data</returns>
        ''' <remarks>Keysight Agilent</remarks>
        Public Function getData(ByVal strMessage As String) As String
            Try
                ' Strip out the carriage return, if any
                If InStr(strMessage, Chr(13)) Then
                    strMessage = strMessage.Remove((InStr(strMessage, Chr(13)) - 1), (Len(strMessage) - InStr(strMessage, Chr(13)) + 1))
                End If
                ' Strip out the line feed, if any
                If InStr(strMessage, Chr(10)) Then
                    strMessage = strMessage.Remove((InStr(strMessage, Chr(10)) - 1), (Len(strMessage) - InStr(strMessage, Chr(10)) + 1))
                End If
                getData = strMessage
            Catch ex As Exception
                MsgBox("Not able to get the data" + vbLf + vbLf + ex.ToString)
            End Try

        End Function

        ''' <summary>
        ''' Assign the CheckBox (to enable channel), ComboBox (to select channel name), TextBox (to set gain), TextBox (to set offset) to each channel
        ''' </summary>
        ''' <param name="index">Index of the channel, range from 0 to 59, since there are 60 channels available (20 for each module and totally 3 modules)</param>
        ''' <param name="enableCheckBox">Used to enable the channel</param>
        ''' <param name="comboBox">Used to select the name of the channel</param>
        ''' <param name="offset">Used to input the offset</param>
        ''' <param name="gain">Used to input the gain</param>
        ''' <remarks></remarks>
        Public Sub initializeChannel(ByVal index As Integer, ByRef enableCheckBox As CheckBox, ByVal comboBox As ComboBox, ByVal offset As TextBox, ByVal gain As TextBox)
            channelArray(index).enableCheckBox = enableCheckBox
            channelArray(index).nameComboBox = comboBox
            channelArray(index).offsetTextBox = offset
            channelArray(index).channelNumber = channelArray(index).enableCheckBox.Text
            channelArray(index).channelName = comboBox.Text
            channelArray(index).gainTextBox = gain
        End Sub

        ''' <summary>
        ''' Initialize Compare structure
        ''' </summary>
        ''' <param name="index">range from 0 - 4, since there are 5 pairs of compare</param>
        ''' <param name="compare1">The first combobox of compare</param>
        ''' <param name="compare2">The second combobox of compare</param>
        ''' <param name="label1">The display name of first channel</param>
        ''' <param name="label2">The display name of second channel</param>
        ''' <param name="difference">The textbox that is used to display the difference</param>
        ''' <param name="enableWarning">The checkbox that is used to indicate whether user wants to get warned if threshold is reached</param>
        ''' <param name="threshold">Contains the threshold that user inputs</param>
        ''' <remarks></remarks>
        Public Sub initializeCompare(ByVal index As Integer, ByRef compare1 As ComboBox, ByRef compare2 As ComboBox, ByRef label1 As Label, ByRef label2 As Label, ByRef difference As TextBox, ByVal enableWarning As CheckBox, ByRef threshold As TextBox)
            compareArray(index).compare1 = compare1
            compareArray(index).compare2 = compare2
            compareArray(index).label1 = label1
            compareArray(index).label2 = label2
            compareArray(index).difference = difference
            compareArray(index).notification = enableWarning
            compareArray(index).warning = True
            compareArray(index).threshold = threshold
        End Sub

        ''' <summary>
        ''' Set up the legend for the plot
        ''' </summary>
        ''' <returns>This returns a string in the format channelname@101,channelname@102,channelname@103...</returns>
        ''' <remarks></remarks>
        Public Function setupChannelPlotLegend()
            Dim channelNameNumberString As String = ""
            For i = 0 To numChannel - 1
                If channelArray(i).enableCheckBox.Checked And channelArray(i).enableCheckBox.Enabled Then
                    channelNameNumberString += (channelArray(i).channelName + "@" + channelArray(i).channelNumber + ",")
                End If
            Next
            channelNameNumberString = channelNameNumberString.TrimEnd(",")
            Return channelNameNumberString
        End Function

        ''' <summary>
        ''' Obtain the scan list in the format (@101,102,103,104,105); set up the row header in the datagridview
        ''' </summary>
        ''' <param name="dgvDAQData"></param>
        ''' <returns>This returns the scan list</returns>
        ''' <remarks></remarks>
        Public Function setUpScanListANDDataHeader(dgvDAQData As DataGridView)
            Dim channelCommandString As String = "(@"
            Dim index As Integer = 0
            For i = 0 To numChannel - 1
                If channelArray(i).enableCheckBox.Checked And channelArray(i).enableCheckBox.Enabled Then
                    channelCommandString += channelArray(i).channelNumber + ","
                    dgvDAQData.Rows.Add("", "", "", "")
                    dgvDAQData.Rows(index).HeaderCell.Value = channelArray(i).channelName + "@" + channelArray(i).channelNumber ' Set header text of format of <channel name>@<channel number>
                    ' Assign a color every other row
                    If index Mod 2 = 1 Then
                        dgvDAQData.Rows(index).DefaultCellStyle.BackColor = Color.LightBlue
                    End If
                    index += 1
                End If
            Next
            ' Get rid of the additional comma at the end
            If channelCommandString.EndsWith(",") Then
                channelCommandString = channelCommandString.TrimEnd(",")
            End If
            channelCommandString += ")"
            Return channelCommandString
        End Function

        ''' <summary>
        ''' Assign the channel name from the ComboBox to channelName; add channel names to the compare ComboBox
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub setUpChannel()
            For i = 0 To numChannel - 1
                ' Assign the text in textbox to the name of channel
                channelArray(i).channelName = channelArray(i).nameComboBox.Text
                ' Add item to compare list box so that user can select channels
                If channelArray(i).enableCheckBox.Checked And channelArray(i).enableCheckBox.Enabled Then
                    frmCurrent_Temperature.cbDAQCompare1.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare2.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare3.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare4.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare5.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare6.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare7.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare8.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare9.Items.Add(channelArray(i).channelNumber)
                    frmCurrent_Temperature.cbDAQCompare10.Items.Add(channelArray(i).channelNumber)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Check if the user input is valid. If not, correct it.
        ''' </summary>
        ''' <param name="textBox"></param>
        ''' <remarks></remarks>
        Public Sub examineHHMMSS(textBox As TextBox)
            Try
                If CInt(Val(textBox.Text)) < 0 Then
                    textBox.Text = "00"
                ElseIf CInt(Val(textBox.Text)) < 10 Then
                    Dim value = CInt(Val(textBox.Text))
                    textBox.Text = "0" + value.ToString
                Else
                    textBox.Text = CInt(Val(textBox.Text))
                End If
            Catch
                textBox.Text = "00"
            End Try
        End Sub

        ''' <summary>
        ''' A helper function that is used to get the index of a channel based on name of channel number
        ''' </summary>
        ''' <param name="name_number">0: find the index by channel name, 1: find the index by channel number</param>
        ''' <param name="id">channel name or channel number</param>
        ''' <param name="dgv">Datagridview object</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function returnRowIndex(ByVal name_number As Integer, ByVal id As String, dgv As DataGridView) As Integer
            ' The format of headercell is <channel name>@<channel number>
            For i = 0 To dgv.Rows.Count - 1
                If id = dgv.Rows(i).HeaderCell.Value.ToString.Split("@")(name_number) Then
                    Return i
                End If
            Next
            Return -1
        End Function

        ''' <summary>
        ''' return the index of Ambient channel in DataGridView
        ''' </summary>
        ''' <param name="dgv">datagridview object</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function returnAmbientIndex(dgv As DataGridView) As Integer
            For i = 0 To dgv.Rows.Count - 1
                If dgv.Rows(i).HeaderCell.Value.ToString.Split("@")(0) = "Ambient" Then
                    Return i
                End If
            Next
            Return -1
        End Function

        ''' <summary>
        ''' Calculate the temperature difference between two channels and change the color of the font based on which has higher temperature
        ''' </summary>
        ''' <param name="i">the index of the compare. There are maximum of 5 compares, so the index range is from 0 to 4</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function displayCompare(ByVal i As Integer)
            ' Based on the channel number, find the index of that channel in data table, then find the instantaneous temperature value, and store it
            compareArray(i).compareValue1 = frmCurrent_Temperature.dgvDAQData.Rows(returnRowIndex(1, compareArray(i).compare1.Text, frmCurrent_Temperature.dgvDAQData)).Cells(0).Value
            compareArray(i).compareValue2 = frmCurrent_Temperature.dgvDAQData.Rows(returnRowIndex(1, compareArray(i).compare2.Text, frmCurrent_Temperature.dgvDAQData)).Cells(0).Value
            Dim difference As Single = (Convert.ToDouble(compareArray(i).compareValue1) - Convert.ToDouble(compareArray(i).compareValue2))
            ' Red font indicates high temperature
            If difference > 0 Then
                compareArray(i).label1.ForeColor = Color.Red
                compareArray(i).label2.ForeColor = SystemColors.ControlText
            ElseIf difference < 0 Then
                compareArray(i).label1.ForeColor = SystemColors.ControlText
                compareArray(i).label2.ForeColor = Color.Red
            Else
                compareArray(i).label1.ForeColor = SystemColors.ControlText
                compareArray(i).label2.ForeColor = SystemColors.ControlText
            End If
            ' Get the absolute value of temperature difference and display it in the textbox
            difference = Abs(difference)
            compareArray(i).difference.Text = (Math.Round(difference, 3, MidpointRounding.AwayFromZero)).ToString
            ' If the user select email notification and the threshold is not blank (user entered some value)
            If compareArray(i).notification.Checked And compareArray(i).threshold.Text <> "" Then
                ' Check if the difference reaches the threshold
                If difference >= Val(compareArray(i).threshold.Text) Then
                    compareArray(i).warning = False ' set to false because this flag means the warning should be sent but has not been sent yet
                End If
            End If
            ' If the user select email notification and the difference reaches threshold and the warning email has not been sent yet
            If compareArray(i).notification.Checked And (Not compareArray(i).warning) Then
                ' the warning is now sent by returning true
                compareArray(i).warning = True
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Send email when test is completed or when threshold is reached
        ''' </summary>
        ''' <param name="testComplete">Is test complete?</param>
        ''' <param name="warningMessage">Is this a warning message?</param>
        ''' <remarks></remarks>
        Public Sub sendEmail(testComplete As Boolean, warningMessage As Boolean, Optional ByVal sendAgain As Boolean = False)
            Try
                Dim oApp As Interop.Outlook._Application = New Interop.Outlook.Application
                Dim oMsg As Interop.Outlook._MailItem = oApp.CreateItem(Interop.Outlook.OlItemType.olMailItem)
                Dim toEmailAddress As New ArrayList
                Dim ccEmailAddress As New ArrayList
                Dim stringToAddress As String = ""
                Dim stringCCAddress As String = ""
                Dim strS As String
                ' combine the to email address together, seperated by a delimiter ";"
                For i = 0 To frmEmailAddressList.lstEmailTo.Items.Count - 1
                    toEmailAddress.Add(frmEmailAddressList.lstEmailTo.Items(i) + ";")
                    stringToAddress = stringToAddress + toEmailAddress.Item(i) + ";"
                Next
                oMsg.To = stringToAddress
                oMsg.CC = ""
                ' combine the CC email address together, seperated by a delimiter ";"
                For j = 0 To frmEmailAddressList.lstEmailCC.Items.Count - 1
                    ccEmailAddress.Add(frmEmailAddressList.lstEmailCC.Items(j))
                    stringCCAddress = stringCCAddress + ccEmailAddress.Item(j) + ";"
                Next
                oMsg.CC = stringCCAddress
                ' edit email body message
                oMsg.Body = "Hello,"
                ' If test is complete, attach the log file
                If testComplete Then
                    oMsg.Subject = String.Concat("Temperature Rise Test Complete at ", DateTime.Now.ToString)
                    If frmCurrent_Temperature.gWriteInstantDataError Then
                        oMsg.Body = String.Concat(oMsg.Body, vbLf, "The Temperature Rise Test started at ", frmCurrent_Temperature.lbDAQInformationStartTime.Text.Substring(12), " is now complete at ", frmCurrent_Temperature.lbDAQInformationTerminateTime.Text.Substring(16), ".", vbLf, "However, an error occurred during the test, so the data has been saved to a template file in the path ""C:\Temp\Temperature Rise Test\temp.csv"".", vbLf, "This error could be caused by several reasons:", vbLf, vbTab, "1. The log file was opened when the program was writing data to it.", vbLf, vbTab, "2. The log file was deleted during the test.", vbLf, vbTab, "3. If the log file is on network drive, unstable internet connection could cause the program not able to locate the log file.", vbLf, vbLf, "The log file is attached to this e-mail.")
                        strS = "C:\Temp\Temperature Rise Test\temp.csv"
                    Else
                        oMsg.Body = String.Concat(oMsg.Body, vbLf, "The Temperature Rise Test started at ", frmCurrent_Temperature.lbDAQInformationStartTime.Text.Substring(12), " is now complete at ", frmCurrent_Temperature.lbDAQInformationTerminateTime.Text.Substring(16), ". The log file is attached to this e-mail.")
                        strS = frmCurrent_Temperature.gLogFileName
                    End If
                    If strS <> "" Then
                        Dim sBodyLen As Integer = Int(oMsg.Body.Length)
                        Dim oAttachs As Interop.Outlook.Attachments = oMsg.Attachments
                        Dim oAttach As Interop.Outlook.Attachment
                        oAttach = oAttachs.Add(strS, , sBodyLen, "Log File")
                    End If
                Else ' If test is not complete (threshold is reached)
                    If warningMessage Then
                        oMsg.Subject = "Temperature Rise Test Warning"
                        oMsg.Body = String.Concat(oMsg.Body, vbLf, "At ", Now.ToString, ", the threshold is reached, please see more details below.", vbLf, frmCurrent_Temperature.gWarningEmailBody, vbLf, "The program will check the temperature difference every 10 minutes thereafter until the temperature difference is less than the threshold or the test is complete.", vbLf, vbLf, "The program will check the temperature difference again at ", DateTime.Now.AddMinutes(10))
                    End If
                End If
                oMsg.Body = String.Concat(oMsg.Body, vbLf, "Operator: ", frmEmailAddressList.txtOperator.Text, vbLf)
                If frmEmailAddressList.chbSimulatedMeter.Checked Then
                    oMsg.Body = String.Concat(oMsg.Body, "Meter Serial Number: Simulated Meter", vbLf)
                Else
                    oMsg.Body = String.Concat(oMsg.Body, "Meter Serial Number: ", frmEmailAddressList.txtSerialNumber.Text, vbLf)
                End If
                If frmEmailAddressList.txtProjectNumber.Text = "" Then
                    oMsg.Body = String.Concat(oMsg.Body, "Project Number: N/A", vbLf)
                Else
                    oMsg.Body = String.Concat(oMsg.Body, "Project Number: ", frmEmailAddressList.txtProjectNumber.Text, vbLf)
                End If
                oMsg.Body = String.Concat(oMsg.Body, "Note: ", frmEmailAddressList.txtNote.Text, vbLf)
                oMsg.Body = String.Concat(oMsg.Body, "=-=-=-=-=-=-=-=-=-=-=", vbLf)
                ' If user choose to programly shut down the power source when the test is complete
                If testComplete And frmCurrent_Temperature.chbAutomaticShutDown.Checked Then
                    oMsg.Body = String.Concat(oMsg.Body, "The voltage and current both should be around 0V and 0A, since the power source is shut down", vbLf)
                End If
                oMsg.Body = String.Concat(oMsg.Body, "Voltage: ", frmCurrent_Temperature.MyRadian.MetricData.Volt.ToString, " V", vbLf)
                oMsg.Body = String.Concat(oMsg.Body, "Current: ", frmCurrent_Temperature.MyRadian.MetricData.Amp.ToString, " A", vbLf)
                oMsg.Body = String.Concat(oMsg.Body, "Frequency: ", frmCurrent_Temperature.MyRadian.MetricData.Frequency.ToString, " Hz", vbLf)
                oMsg.Body = String.Concat(oMsg.Body, "Phase: ", frmCurrent_Temperature.MyRadian.MetricData.Phase.ToString, "°", vbLf)
                oMsg.Body = String.Concat(oMsg.Body, vbLf, "Regards,", vbLf, "Current & Temperature Program")
                oMsg.Send()
            Catch ' Usually because the outlook app is not open
                If Not sendAgain Then
                    Dim process_outlook() As Process = Process.GetProcessesByName("Outlook.exe")
                    If process_outlook.Count = 0 Then ' If no process is running
                        Try
                            Dim processStartInfo As New System.Diagnostics.ProcessStartInfo("Outlook.exe")
                            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized
                            Dim myProcess As Process = System.Diagnostics.Process.Start(processStartInfo) ' start outlook
                        Catch
                            'MessageBox.Show("Outlook.exe is not installed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End Try
                        frmCurrent_Temperature.delay(5) ' wait 5 seconds for outlook to start
                        sendEmail(testComplete, warningMessage, True) ' try sending an email again
                    End If
                Else
                    Exit Sub
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Check if the reading interval is selected; check if the measuring device is selected; check if there is at least one channel selected
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Identified2Standby() As Boolean
            ' If user has entered the reading interval
            If frmCurrent_Temperature.cbDAQParametersReadingIntervals.Text <> "" Then
                ' If user has selected either FRTD or Thermocouple
                If (frmCurrent_Temperature.rbDAQParametersFRTD.Checked Or frmCurrent_Temperature.rbDAQParametersThermocouple.Checked) Then
                    ' If user has selected at least one channel from 101-110
                    If (frmCurrent_Temperature.cbDAQDataEnableChannel101.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel102.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel103.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel104.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel105.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel106.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel107.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel108.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel109.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel110.Checked) Then
                        Return True
                        ' If user has selected at least one channel from 111-120
                    ElseIf (frmCurrent_Temperature.cbDAQDataEnableChannel111.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel112.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel113.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel114.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel115.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel116.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel117.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel118.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel119.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel120.Checked) Then
                        Return True
                        ' If user has selected at least one channel from 201-210
                    ElseIf (frmCurrent_Temperature.cbDAQDataEnableChannel201.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel202.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel203.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel204.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel205.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel206.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel207.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel208.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel209.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel210.Checked) Then
                        Return True
                        ' If user has selected at least one channel from 211-220
                    ElseIf (frmCurrent_Temperature.cbDAQDataEnableChannel211.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel212.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel213.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel214.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel215.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel216.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel217.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel218.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel219.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel220.Checked) Then
                        Return True
                        ' If user has selected at least one channel from 301-310
                    ElseIf (frmCurrent_Temperature.cbDAQDataEnableChannel301.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel302.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel303.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel304.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel305.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel306.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel307.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel308.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel309.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel310.Checked) Then
                        Return True
                        ' If user has selected at least one channel from 311-320
                    ElseIf (frmCurrent_Temperature.cbDAQDataEnableChannel311.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel312.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel313.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel314.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel315.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel316.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel317.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel318.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel319.Checked Or frmCurrent_Temperature.cbDAQDataEnableChannel320.Checked) Then
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

    End Class
End Module
