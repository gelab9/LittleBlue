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
Imports System.Net.Mail

Public Class frmCurrent_Temperature
#Region "Delegates"

    Private Delegate Sub setTextBoxInvoker(ByVal text As String, ByVal Txtbox As TextBox)
    Private Delegate Sub setButtonInvoker(ByVal Btn As Button, ByVal enable As Boolean)
    Private Delegate Sub setLabelInvoker(ByVal lbl As Label, ByVal text As String)
    Private Delegate Sub setTextBoxReadingInvoker(ByVal Txtbox As TextBox, ByVal reading As String)
    Private Delegate Sub setDataGridViewReadingInvoker(ByVal dgv As DataGridView, ByVal reading As String, ByVal readingIndex As Integer, ByVal indicator As Integer)
    Private Delegate Sub askLabelInvoker(ByVal lbl As Label, ByVal text As String)
    Private Delegate Sub setProgessBarInvoker(ByVal progress As Double, ByVal progressBar As ProgressBar)
    Private Delegate Sub addSeriesInvoker(ByVal i As Integer, ByVal channelName As String, ByVal chart As DataVisualization.Charting.Chart, ByVal xtitle As String, ByVal ytitle As String)
    Private Delegate Sub addChartPointsInvoker(ByVal index As Integer, ByVal data As Double, ByVal series As String, ByVal chart As DataVisualization.Charting.Chart)
    Private Delegate Sub setUpChartTitleInvoker(ByVal chart As DataVisualization.Charting.Chart, ByVal title As String)
    Private Delegate Sub setLineWidthInvoker(ByVal chart As DataVisualization.Charting.Chart, ByVal width As Integer)
    Private Delegate Sub addItemInvoker(ByVal item1 As String, ByVal list As ListBox)








    ''' <summary>
    ''' Set the "text" as String to "Txtbox" as TextBox
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="Txtbox"></param>
    ''' <remarks></remarks>
    Private Sub setTextBox(ByVal text As String, ByVal Txtbox As TextBox)
        If Txtbox.InvokeRequired Then
            Txtbox.Invoke(New setTextBoxInvoker(AddressOf setTextBox), text, Txtbox)
        Else
            Txtbox.Text = text
        End If
    End Sub
    ''' <summary>
    ''' Display the string in the specified Label
    ''' </summary>
    ''' <param name="lbl"></param>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Private Sub setLabel(ByVal lbl As Label, ByVal text As String)
        If lbl.InvokeRequired Then
            lbl.Invoke(New setLabelInvoker(AddressOf setLabel), lbl, text)
        Else
            lbl.Text = text
        End If
    End Sub

    ''' <summary>
    ''' Input the reading to the DataGridView
    ''' </summary>
    ''' <param name="dgv">DataGridView</param>
    ''' <param name="reading">Current Reading or Max Reading or Min Reading or Average Reading or Temperature Risen Reading</param>
    ''' <param name="readingIndex">Index of channel (only channels that are currently being used)</param>
    ''' <param name="indicator">0: Current Reading, 1: Max Reading, 2: Min Reading, 3: Average Reading, 4: Temperature Risen Reading</param>
    ''' <remarks></remarks>
    Private Sub setDataGridViewReading(ByVal dgv As DataGridView, ByVal reading As String, ByVal readingIndex As Integer, ByVal indicator As Integer)
        If dgv.InvokeRequired Then
            dgv.Invoke(New setDataGridViewReadingInvoker(AddressOf setDataGridViewReading), dgv, reading, readingIndex, indicator)
        Else
            dgv.Rows(readingIndex).Cells(indicator).Value = reading
        End If
    End Sub

    ''' <summary>
    ''' In Duration mode, if time matches the set time, set the flag readyToStop to true
    ''' </summary>
    ''' <param name="lbl"></param>
    ''' <param name="text"></param>
    ''' <remarks></remarks>
    Private Sub askLabel(ByVal lbl As Label, ByVal text As String)
        If lbl.InvokeRequired Then
            lbl.Invoke(New askLabelInvoker(AddressOf askLabel), lbl, text)
        Else
            If lbl.Text = text Then
                MyAgilent34970A.ReadingsVal.readyToStop = True
            End If
        End If
    End Sub
    ''' <summary>
    ''' Set the value of the progress bar
    ''' </summary>
    ''' <param name="progress">value of the progress</param>
    ''' <param name="progressBar">The object progess bar</param>
    ''' <remarks></remarks>
    Private Sub setProgressBar(ByVal progress As Double, ByVal progressBar As ProgressBar)
        If progressBar.InvokeRequired Then
            progressBar.Invoke(New setProgessBarInvoker(AddressOf setProgressBar), progress, progressBar)
        Else
            If progress > 100 Then
                progress = 100
            End If
            progressBar.Value = progress
        End If
    End Sub

    ''' <summary>
    ''' Add series and also set the parameters, like linewidth, markerstyle
    ''' </summary>
    ''' <param name="i">Clear all the series if this sub is called for the first time</param>
    ''' <param name="channelName">Series name</param>
    ''' <param name="chart">the Chart</param>
    ''' <remarks></remarks>
    Private Sub addSeries(ByVal i As Integer, ByVal channelName As String, ByVal chart As DataVisualization.Charting.Chart, ByVal xtitle As String, ByVal ytitle As String)
        If chart.InvokeRequired Then
            chart.Invoke(New addSeriesInvoker(AddressOf addSeries), i, channelName, chart, xtitle, ytitle)
        Else
            If i = 0 Then
                chart.Series.Clear()
            End If
            chart.Series.Add(channelName)
            chart.Series(channelName).ChartType = DataVisualization.Charting.SeriesChartType.FastLine
            chart.Series(channelName).XValueType = DataVisualization.Charting.ChartValueType.Time
            chart.Series(channelName).YValueType = DataVisualization.Charting.ChartValueType.Double
            chart.Series(channelName).BorderWidth = 2
            chart.Series(channelName).MarkerStyle = DataVisualization.Charting.MarkerStyle.Circle
            chart.ChartAreas(0).AxisX.Title = xtitle
            chart.ChartAreas(0).AxisX.TitleFont = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
            chart.ChartAreas(0).AxisY.Title = ytitle
            chart.ChartAreas(0).AxisY.TitleFont = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        End If
    End Sub

    ''' <summary>
    ''' Add data points
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="data"></param>
    ''' <param name="series"></param>
    ''' <param name="chart"></param>
    ''' <remarks></remarks>
    Private Sub addChartPoints(ByVal index As Integer, ByVal data As Double, ByVal series As String, ByVal chart As DataVisualization.Charting.Chart)
        If chart.InvokeRequired Then
            chart.Invoke(New addChartPointsInvoker(AddressOf addChartPoints), index, data, series, chart)
        Else
            chart.Series(series).Points.AddXY(lbDAQInformationDuration.Text.TrimStart("D", "u", "r", "a", "t", "i", "o", "n", ":", " "), data)

        End If
    End Sub

    Private Sub setUpChartTitle(ByVal chart As DataVisualization.Charting.Chart, ByVal title As String)
        If chart.InvokeRequired Then
            chart.Invoke(New setUpChartTitleInvoker(AddressOf setUpChartTitle), chart, title)
        Else
            chart.Titles.Add(title)
            chart.Titles(0).Font = New Font("Microsoft Sans Serif", 20, FontStyle.Bold)
        End If
    End Sub

    Private Sub setLineWidth(ByVal chart As DataVisualization.Charting.Chart, ByVal width As Integer)
        If chart.InvokeRequired Then
            chart.Invoke(New setLineWidthInvoker(AddressOf setLineWidth), chart, width)
        Else
            For cnt = 0 To chart.Series.Count - 1
                chart.Series(cnt).BorderWidth = width
            Next
        End If
    End Sub

    Private Sub addItem(ByVal item1 As String, ByVal list As ListBox)
        If list.InvokeRequired Then
            list.Invoke(New addItemInvoker(AddressOf addItem), item1, list)
        Else
            list.Items.Add(item1)
            list.TopIndex = list.Items.Count - 1
        End If
    End Sub
#End Region
    Public MyRadian As New Radian
    Public WithEvents MyPacPower As New cPacPowerUPC1
    Public StopWatch As New Diagnostics.Stopwatch
    Public MyAgilent34970A As New Agilent34970A
    Public MyEmail As New frmEmailAddressList
    Public GpibDevice As NationalInstruments.NI4882.Device
    Public MyCICA501TAC As New cCA501TAC
    Public gGPIBTextWidth As Integer
    Public WrapListBoxFonts As Font


    'Timer State Machine
    Public Enum eState
        Idle
        ReadingMetics
        ReadingWaveform
        RunningMeterTest
        RunningLoopTest
        RunningStandardTest
    End Enum
    Public Enum eDeviceState
        Initial                 ' Initial state
        Connected               ' When the com port is initialized
        Identified              ' When the computer is connected to the device successfully but parameters have not been set
        Standby                 ' When it's ready to start the test
        GettingReading          ' When the device is taking the readings
        StoppingReading         ' When the "Stio Reading" button is clicked but the device is not ready to stop
        ReadingComplete         ' When the reading progress is complete
        WaitingForSetup         ' The device is setting up
        WaitingForNextReading   ' When the device is waiting for the next reading, the program will not imediately if the "Stop reading" button is clicked 
    End Enum
    Public gStateMachineState As eState
    Public gLastState As eState
    Public gDAQStateMachine As eDeviceState
    'Global Flags
    Public gbWriteToLog As Boolean
    Public gbVerbose As Boolean
    Public gbCurrentDataSaved As Boolean = True
    Public gbToggleConnected As Boolean = False
    'Other globals '============================================= put prefix g

    Public gstrOutBuffer As String
    Public gVerboseLevel As Integer = 1
    Public guiSampleCount As UInteger
    Public gLogFileName As String ' String containing the path of the data file
    Public gLogFileNameTemp As String
    Public gImageFileName As String 'String containing the path of the image file
    Public gWriteInstantDataError As Boolean = False ' Check if the program still has the access to the path
    Public gWriteInstantTempDataError As Boolean = False
    Public gChannelLegendString As String()
    Public gChannelCommandString As String
    Public gReadingComplete As Boolean = False ' True if one iteration of reading is completed
    Public gWarningEmailBody As String = ""
    Public gWarningEndTicks As Integer

    ' Constant
    Private Const NUM_ALL_CHANNELS As Integer = 60
    Private Const NUM_ALL_COMPARES As Integer = 5

#Region "Functions Safely Available to all Threads"
    Public Delegate Sub DAQUpDateLogDelegate(ByVal message As String)
    Public Delegate Sub UpDateErrorDelegate(ByVal errorMessage As String)
    ''' <summary>
    '''  Thread safe UpdateLog sets Logdata's items.add property
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    ''' 
    Public Sub v_UpdateDAQLog(ByVal message As String, Optional VerboseLevel As Integer = 0)
        Dim FileNum As Integer = FreeFile()
        ' if modifying Output data is not thread safe
        If lstDAQLog.InvokeRequired Then
            ' use inherited method Invoke to execute DisplayMessage
            ' via a Delegate
            Try
                Invoke(New DAQUpDateLogDelegate(AddressOf v_UpdateDAQLog), New Object() {message})
            Catch ex As Exception
            End Try
        Else
            Try
                lstDAQLog.Items.Add("Time: " + DateTime.Now.ToString + vbTab + message) ' Add message
                lstDAQLog.TopIndex = lstDAQLog.Items.Count - 1 ' Focus on the newest added item
            Catch ex As Exception
                lstDAQLog.Items.Clear()
            End Try

        End If
    End Sub ' v_UpdateLog()

    ''' <summary>
    ''' Delay function
    ''' </summary>
    ''' <param name="dblSecs">how many seconds of delay</param>
    ''' <remarks></remarks>
    Public Sub delay(ByVal dblSecs As Single)
        Dim startTicks As Integer = My.Computer.Clock.TickCount
        Dim endTicks As Integer = startTicks + dblSecs * 1000
        Do Until My.Computer.Clock.TickCount > endTicks
            Application.DoEvents()
        Loop
    End Sub

    ''' <summary>
    ''' Update any error message, mainly used while testing
    ''' </summary>
    ''' <param name="errorMessage"></param>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Public Sub v_UpdateError(ByVal errorMessage As String)
        If lstDAQError.InvokeRequired Then
            Invoke(New UpDateErrorDelegate(AddressOf v_UpdateError), New Object() {errorMessage})
        Else
            lstDAQError.Items.Add("Time: " + Now.TimeOfDay.ToString + vbTab + errorMessage) ' add message
            lstDAQError.TopIndex = lstDAQError.Items.Count - 1 ' Always focus on the newest added message
        End If
    End Sub
#End Region '"Functions Safely Available to all Threads"

    ''' <summary>
    ''' Identify the device when Identify button clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub btnDAQIdentity_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDAQIdentificationIdentity.Click
        Dim moduleID As String() ' String array, which contains information returned from the device after sending SYSTem:CTYPe?
        ' Button available only when com port initialized and the instrument is ready to be identified
        If gDAQStateMachine <> eDeviceState.Connected Then
            Exit Sub
        End If
        Try
            Dim endTime = DateTime.Now.AddSeconds(2) ' Give the program 2 seconds to idendify the device
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            While Not SendCommand(Agilent34970A.sMESSAGE.QUERY_IDENTIFY, True)
                If DateTime.Now >= endTime Then
                    Throw New Exception("Timed out. Not able to identify the device.")
                End If
            End While
            lbDAQIdentification.Items.Clear()
            lbDAQIdentification.Items.Add("Manufacturer:" + vbTab + MyAgilent34970A.Parameters.Manufacturer)
            lbDAQIdentification.Items.Add("Model Number:" + vbTab + MyAgilent34970A.Parameters.ModelNumber)
            lbDAQIdentification.Items.Add("Serial Number:" + vbTab + MyAgilent34970A.Parameters.SerialNumber)
            lbDAQIdentification.Items.Add("FW Version:" + vbTab + MyAgilent34970A.Parameters.Firmware)
            delay(0.1)

            ' Check and make sure the 34901A Module is installed in slot 100, 200 or 300;
            For modulePrefix = 1 To 3
                SendCommand("SYSTem:CTYPe? " + modulePrefix.ToString + "00", True) ' If 34901A module is installed correctly, the instrument will return a string in the form HEWLETT-PACKARD,34901A,0,X.X
                moduleID = MyAgilent34970A.ReadingsVal.checkModule(modulePrefix - 1).Split(",")
                ' If moduleID indicates that 34901A is detected, enable the radio button
                If moduleID(1) = "34901A" Then
                    If modulePrefix = 1 Then
                        rbSlot100.Enabled = True
                    ElseIf modulePrefix = 2 Then
                        rbSlot200.Enabled = True
                    ElseIf modulePrefix = 3 Then
                        rbSlot300.Enabled = True
                    End If
                End If
            Next

            ' If 34901A is not installed in any slot, a message box will pop out and the state will go back to Connected
            If (Not rbSlot100.Enabled) And (Not rbSlot200.Enabled) And (Not rbSlot300.Enabled) Then
                MessageBox.Show("Module 34901A is not installed correctly!", "Module 34901A NOT Installed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            SendCommand(Agilent34970A.sMESSAGE.CLEAR_EVENT_REGISTERS, False)
            gDAQStateMachine = eDeviceState.Identified
            Windows.Forms.Cursor.Current = Cursors.Default
            v_UpdateDAQLog("Instrument identified")
            If rbSlot100.Enabled Then
                rbSlot100.Checked = True
            ElseIf rbSlot200.Enabled Then
                rbSlot200.Checked = True
            ElseIf rbSlot300.Enabled Then
                rbSlot300.Checked = True
            End If
            chbDAQCalibration.Checked = My.Settings.enableCalibration

            If Not (My.Settings.Channel.Count = 0 And My.Settings.checkStatus.Count = 0 And My.Settings.Gain.Count = 0 And My.Settings.Offset.Count = 0) Then
                For i = 0 To 59
                    If i < 20 Then
                        If rbSlot100.Enabled Then
                            'If My.Settings.enableChannel(i) = "True" Then
                            '    MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = True
                            'Else
                            '    MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = False
                            'End If
                            If My.Settings.checkStatus(i) = "True" Then
                                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = True
                            Else
                                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = False
                            End If
                            MyAgilent34970A.channelArray(i).nameComboBox.Text = My.Settings.Channel(i)
                            MyAgilent34970A.channelArray(i).gainTextBox.Text = My.Settings.Gain(i)
                            MyAgilent34970A.channelArray(i).offsetTextBox.Text = My.Settings.Offset(i)
                        Else
                            MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = False
                            MyAgilent34970A.channelArray(i).enableCheckBox.Checked = False
                            MyAgilent34970A.channelArray(i).nameComboBox.Text = ""
                            MyAgilent34970A.channelArray(i).gainTextBox.Text = ""
                            MyAgilent34970A.channelArray(i).offsetTextBox.Text = ""
                        End If
                    ElseIf i < 40 Then
                        If rbSlot200.Enabled Then
                            'If My.Settings.enableChannel(i) = "True" Then
                            '    MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = True
                            'Else
                            '    MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = False
                            'End If
                            If My.Settings.checkStatus(i) = "True" Then
                                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = True
                            Else
                                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = False
                            End If
                            MyAgilent34970A.channelArray(i).nameComboBox.Text = My.Settings.Channel(i)
                            MyAgilent34970A.channelArray(i).gainTextBox.Text = My.Settings.Gain(i)
                            MyAgilent34970A.channelArray(i).offsetTextBox.Text = My.Settings.Offset(i)
                        Else
                            MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = False
                            MyAgilent34970A.channelArray(i).enableCheckBox.Checked = False
                            MyAgilent34970A.channelArray(i).nameComboBox.Text = ""
                            MyAgilent34970A.channelArray(i).gainTextBox.Text = ""
                            MyAgilent34970A.channelArray(i).offsetTextBox.Text = ""
                        End If
                    ElseIf i < 60 Then
                        If rbSlot300.Enabled Then
                            'If My.Settings.enableChannel(i) = "True" Then
                            '    MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = True
                            'Else
                            '    MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = False
                            'End If
                            If My.Settings.checkStatus(i) = "True" Then
                                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = True
                            Else
                                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = False
                            End If
                            MyAgilent34970A.channelArray(i).nameComboBox.Text = My.Settings.Channel(i)
                            MyAgilent34970A.channelArray(i).gainTextBox.Text = My.Settings.Gain(i)
                            MyAgilent34970A.channelArray(i).offsetTextBox.Text = My.Settings.Offset(i)
                        Else
                            MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = False
                            MyAgilent34970A.channelArray(i).enableCheckBox.Checked = False
                            MyAgilent34970A.channelArray(i).nameComboBox.Text = ""
                            MyAgilent34970A.channelArray(i).gainTextBox.Text = ""
                            MyAgilent34970A.channelArray(i).offsetTextBox.Text = ""
                        End If
                    End If
                Next
            End If

        Catch ex As Exception
            MessageBox.Show("Not able to identify the instrument." + vbLf + ex.ToString, "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            MyAgilent34970A.v_Disconnect()
            lbDAQIdentification.Items.Clear()
            gDAQStateMachine = eDeviceState.Initial
            Windows.Forms.Cursor.Current = Cursors.Default
            v_UpdateDAQLog("Failed to identify the instrument")
        End Try
    End Sub

    ''' <summary>
    ''' Handles the connect button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDAQConnectionConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDAQConnectionConnect.Click
        Dim bSuccessInitialize As Boolean = True ' Suppose the port is initialized successfully
        ' Button will not respond if the device is taking reading
        If MyAgilent34970A.myProgram.DeviceRunning Then
            Exit Sub
        End If
        If gDAQStateMachine = eDeviceState.Initial Then
            Try
                MyAgilent34970A.v_Disconnect()
                MyAgilent34970A.v_InitSerialPort(CInt(txtDAQConnectionBaudrate.Text), IO.Ports.Parity.None, IO.Ports.StopBits.One, 8, "COM" + cbDAQConnectionComPort.Text, True)
                v_UpdateDAQLog("Initilization Of COM " + cbDAQConnectionComPort.Text.ToString + " complete. Baud Rate: " + txtDAQConnectionBaudrate.Text)
            Catch ex As Exception
                bSuccessInitialize = False ' Failed to initialize the com port
                v_UpdateDAQLog("Agilent 34970A: Error Initializing Com Port")
            End Try
            ' If initialize successfully
            If bSuccessInitialize Then
                Try
                    MyAgilent34970A.v_Connect()
                    btnDAQConnectionConnect.Text = "Disconnect"
                    gDAQStateMachine = eDeviceState.Connected
                    btnDAQIdentificationIdentity.PerformClick()

                Catch ex As Exception
                    v_UpdateDAQLog("Agilent 34970A: Error Connecting to Agilent 34907A")
                End Try
            End If
        Else ' Disconnect and let state go back to Initial
            Try
                ClearAll() ' Clear all the settings when user disconnect the device
                rbSlot100.Checked = False
                rbSlot200.Checked = False
                rbSlot300.Checked = False
                MyAgilent34970A.v_Disconnect()
                btnDAQConnectionConnect.Text = "Connect"
                lbDAQIdentification.Items.Clear()
                v_UpdateDAQLog("Disconnected From Device")
                gDAQStateMachine = eDeviceState.Initial
            Catch ex As Exception
                gbWriteToLog = True
                v_UpdateDAQLog(ex.ToString)
                gbWriteToLog = False
            End Try
        End If
    End Sub


    ''' <summary>
    ''' When "Get Readings" button is clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub GetReadings_Click(sender As System.Object, e As System.EventArgs) Handles btnDAQStartStopReading.Click
        Dim tempDirectoryString As String = "C:\Temp\Temperature Rise Test" ' The folder where the temporary file is 
        Dim tempFileString As String = tempDirectoryString + "\temp.csv" ' The path to the temporary file
        ' If the device is not running
        If Not MyAgilent34970A.myProgram.DeviceRunning Then
            ' If the program state is not at the Standby state, the button will not respond
            If gDAQStateMachine <> eDeviceState.Standby Then
                Exit Sub
            End If
            ' Check if the device is still connected; so send "*IDN?" to test
            Try
                If Not SendCommand("*IDN?", True) Then
                    Throw New Exception("")
                End If
            Catch
                MessageBox.Show("Failed to communicate with the instrument. Please recheck if the computer is connected to the instrument", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            End Try
            ' Clear the data
            ClearAll()
            ' Assign duration
            If txtDAQCountDownHH.Text = "" Then
                MyAgilent34970A.ReadingsVal.hour = 0
            Else
                MyAgilent34970A.ReadingsVal.hour = CInt(txtDAQCountDownHH.Text)
            End If
            If txtDAQCountDownMM.Text = "" Then
                MyAgilent34970A.ReadingsVal.minute = 0
            Else
                MyAgilent34970A.ReadingsVal.minute = CInt(txtDAQCountDownMM.Text)
            End If
            If txtDAQCountDownSS.Text = "" Then
                MyAgilent34970A.ReadingsVal.second = 0
            Else
                MyAgilent34970A.ReadingsVal.second = CInt(txtDAQCountDownSS.Text)
            End If


            ' Select the location to save the log file. If not saved successfully, exit the subroutine
            If Not saveLogFile() Then
                Exit Sub
            End If
            ' If there is no such directory for putting temporary file, program will create one
            If Not Directory.Exists(tempDirectoryString) Then
                Directory.CreateDirectory(tempDirectoryString)
            End If
            ' If there is already a temporary file existing, delete that file
            If File.Exists(tempFileString) Then
                File.Delete(tempFileString)
            End If

            saveLogFile(tempFileString) ' Save the temporary log file
            MyAgilent34970A.myProgram.DeviceRunning = True ' Set the flag DeviceRunning to true to indicate the device starts running
            MyAgilent34970A.ReadingsVal.readyToStop = False ' Make sure readyToStop flag is false before running test
            MyAgilent34970A.ReadingsVal.timerCountDown = False ' Make sure timerCountDown flag is false before running test
            TimerRunning.Start() ' Start the timer for getting readings
            MyAgilent34970A.setUpChannel() ' Assign the channel name from the ComboBox to channelName; add channel names to the compare ComboBox
            gChannelLegendString = MyAgilent34970A.setupChannelPlotLegend().Split(",") ' this string array contains channelname@channelnumber for each channel
            gChannelCommandString = MyAgilent34970A.setUpScanListANDDataHeader(dgvDAQData) ' Obtain the scan list in the format (@101,102,103,104,105); set up the row header in the datagridview
            gDAQStateMachine = eDeviceState.WaitingForSetup ' Set state to waiting for setup
            frmSettingUpDevice.Show() ' Pop up the waiting window
            Me.Hide() ' Hide the main form
            Setup() ' Set up the device before taking readings
            frmSettingUpDevice.Close() ' Close the waiting window
            Me.Show() ' Show the main form
            ' Initialize the every element in compare structure array; 5 pairs of compare, so index is from 0 - 4
            MyAgilent34970A.initializeCompare(0, cbDAQCompare1, cbDAQCompare2, lblDAQCompareCompare1, lblDAQCompareCompare2, txtDAQCompare1, cbDAQCompare1_Check, txtDAQCompareThreshold1)
            MyAgilent34970A.initializeCompare(1, cbDAQCompare3, cbDAQCompare4, lblDAQCompareCompare3, lblDAQCompareCompare4, txtDAQCompare2, cbDAQCompare2_Check, txtDAQCompareThreshold2)
            MyAgilent34970A.initializeCompare(2, cbDAQCompare5, cbDAQCompare6, lblDAQCompareCompare5, lblDAQCompareCompare6, txtDAQCompare3, cbDAQCompare3_Check, txtDAQCompareThreshold3)
            MyAgilent34970A.initializeCompare(3, cbDAQCompare7, cbDAQCompare8, lblDAQCompareCompare7, lblDAQCompareCompare8, txtDAQCompare4, cbDAQCompare4_Check, txtDAQCompareThreshold4)
            MyAgilent34970A.initializeCompare(4, cbDAQCompare9, cbDAQCompare10, lblDAQCompareCompare9, lblDAQCompareCompare10, txtDAQCompare5, cbDAQCompare5_Check, txtDAQCompareThreshold5)
            Me.StopWatch.Start() ' Start the stop watch
            gWarningEndTicks = My.Computer.Clock.TickCount ' Initialize warningEndTicks, then every after 10 minutes, the program will check the threshold if enabled
            '==============
            gStateMachineState = eState.ReadingMetics
            gLastState = eState.Idle
            gbCurrentDataSaved = False
            guiSampleCount = 0
            '==============
            BackgroundWorker1.RunWorkerAsync() ' Start the backgroundworker
        ElseIf MyAgilent34970A.myProgram.DeviceRunning Then
            MyAgilent34970A.ReadingsVal.readyToStop = True ' Set readyToStop to true to indicate the program is ready to stop
        End If
    End Sub

    ''' <summary>
    ''' This is the main function that is used to set up the device and retrieve the readings from the device. The logic flow chart can be found in the instruction
    ''' </summary>
    ''' <param name="worker"></param>
    ''' <param name="e"></param>
    ''' <returns></returns>
    ''' <remarks>Yang Cheng Landis+gyr 2016</remarks>
    Public Function readingProgress(ByVal worker As System.ComponentModel.BackgroundWorker, ByVal e As System.ComponentModel.DoWorkEventArgs) As Boolean
        ' If last state is waiting for setup
        If gDAQStateMachine = eDeviceState.WaitingForSetup Then
            Dim iteration As Integer = 0 ' Iteration of readings
            Try
                setLabel(lbDAQInformationStartTime, "Start Time: " + DateTime.Now.ToString) ' Set the start time
                setUpChartTitle(DataChart, "Temperature vs. Time") ' Set Data Chart title
                setUpChartTitle(CurrentChart, "Current vs. Time") ' Set Current(Amp) Chart title
                gDAQStateMachine = eDeviceState.WaitingForNextReading ' The program is ready to jump to waiting for next reading state
                ' Stay in the while loop until readyToStop is true
                While True
                    gDAQStateMachine = eDeviceState.GettingReading
                    Readings(iteration) ' Measure readings and put them into data table
                    gDAQStateMachine = eDeviceState.WaitingForNextReading
                    setLabel(lbDAQInformationNumofReadings, "Total Number of Readings: " + MyAgilent34970A.ReadingsVal.totalReadings.ToString)
                    'delay(1)
                    ' If the user wants to get a specific number of readings, the program exits the while loop when enough readings have been taken
                    If rbNumofReadings.Checked Then
                        If iteration = MyAgilent34970A.ReadingsVal.totalNumofReadings - 1 Then
                            MyAgilent34970A.ReadingsVal.readyToStop = True ' Set the flag to true if iteration has reached (the number of readings - 1), and the program will then stop
                        End If
                        setProgressBar((iteration + 1) / MyAgilent34970A.ReadingsVal.totalNumofReadings * 100, ProgressBar1) ' Update the progress bar
                    End If
                    ' If the program is ready to stop, program will exit the while loop when the state is ReadingComplete
                    If MyAgilent34970A.ReadingsVal.readyToStop Or MyAgilent34970A.ReadingsVal.timerCountDown Then ' If stopreadingbutton is clicked or time is up
                        Windows.Forms.Cursor.Current = Cursors.WaitCursor
                        While (gDAQStateMachine <> eDeviceState.ReadingComplete) And (gDAQStateMachine <> eDeviceState.Standby)
                        End While
                        Windows.Forms.Cursor.Current = Cursors.Default
                        Exit While
                    End If
                    iteration = iteration + 1
                End While
            Catch ex As Exception
                MsgBox("GetReadings_Click(): " + ex.ToString)
                Return False
            End Try
        End If
        Return True
    End Function

    ''' <summary>
    ''' This command sends a message to the Agilent 34970A
    ''' </summary>
    ''' <param name="command">SCPI Complianct Comamnd to instrument </param>
    ''' <param name="bRecieve">TRUE is data is expected to be returned, False otherwise</param>
    ''' <remarks>Yang Cheng, Landis Gyr 2016</remarks>
    Private Function SendCommand(ByVal command As String, Optional ByVal bRecieve As Boolean = False) As Boolean
        Dim success_receive As Boolean = False
        MyAgilent34970A.aBytes = System.Text.Encoding.ASCII.GetBytes(command + vbLf) 'convert string to byte array
        Dim strRXData As String = "" 'temp string to hold recieved data
        MyAgilent34970A.v_Transmit(MyAgilent34970A.aBytes, bRecieve) 'transmit message to Agilent
        'recieve data 
        If bRecieve = True Then
            Try
                success_receive = MyAgilent34970A.b_Receive(MyAgilent34970A.aBytes) ' If receive successfully
                strRXData = str_Byte_Array_To_ASCII_String(MyAgilent34970A.aBytes) 'Convert from byte array to ASCII string
                'Parse data from packet
                If success_receive Then
                    MyAgilent34970A.v_ParseAgilent34970Data(strRXData, command) ' Parse the data returned if receive successfully
                Else
                    Return False
                    Exit Function
                End If
            Catch ex As Exception
                MessageBox.Show("Something wrong while attempting to send command to the device" + vbLf + vbLf + ex.ToString, "Error in Receiving Data")
                Return False
            End Try
        End If
        Return True
    End Function

    ''' <summary>
    ''' Set up the device
    ''' </summary>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub Setup()
        frmSettingUpDevice.pbSettingUp.Value = 0
        SendCommand("ABORt")
        Try
            ' Reset instrument
            SendCommand("*RST", False)
            ' Set thermocouple or RTD depending on user's choice
            If MyAgilent34970A.myProgram.MeasureDevice = 1 Then
                SendCommand("CONFigure:TEMPerature TCouple, J, " + gChannelCommandString, False)
                frmSettingUpDevice.Label1.Text = "Measuring device set to Thermocouple."
            ElseIf MyAgilent34970A.myProgram.MeasureDevice = 0 Then
                SendCommand("CONFigure:TEMPerature FRTD, 85, " + gChannelCommandString, False) 'Changed from RTD (2-wire) to FRTD (4-wire) on 8/9/2016
                frmSettingUpDevice.Label1.Text = "Measuring device set to FRTD (4-wire RTD)."
            End If
            delay(0.5)
            frmSettingUpDevice.pbSettingUp.Value = 10
            ' Select the temperature unit (C = Celcius)
            SendCommand("UNIT:TEMPerature C, " + gChannelCommandString, False)
            frmSettingUpDevice.Label1.Text = "Temperaturer unit set to °C"
            delay(0.5)
            frmSettingUpDevice.pbSettingUp.Value = 20
            ' set thermocouple or RTD depending on user's choice
            If MyAgilent34970A.myProgram.MeasureDevice = 1 Then
                ' Set the reference temperature type (internal)
                SendCommand("SENSe:TEMPerature:TRANSducer:TCouple:RJUNction:TYPE INTernal, " + gChannelCommandString, False)
            ElseIf MyAgilent34970A.myProgram.MeasureDevice = 0 Then
                SendCommand("SENSe:TEMPerature:TRANSducer:FRTD:TYPE 85, " + gChannelCommandString, False) 'Changed from RTD (2-wire) to FRTD (4-wire) on 8/9/2016
            End If
            delay(0.5)
            frmSettingUpDevice.pbSettingUp.Value = 30
            ' If user wants to calibrate the RTDs (by scaling temperature mx+b). For more details, check page 192 of agilent 34970a manual
            If chbDAQCalibration.Checked Then
                frmSettingUpDevice.Label1.Text = "Setting gain and offset..."
                For i = 0 To MyAgilent34970A.numChannel - 1
                    If MyAgilent34970A.channelArray(i).enableCheckBox.Checked Then
                        ' Set the gain for each channel
                        If MyAgilent34970A.channelArray(i).gainTextBox.Text <> "" Then
                            SendCommand("CALCulate:SCALe:GAIN " + MyAgilent34970A.channelArray(i).gainTextBox.Text + ", (@" + MyAgilent34970A.channelArray(i).channelNumber + ")")
                        Else
                            SendCommand("CALCulate:SCALe:GAIN 1" + ", (@" + MyAgilent34970A.channelArray(i).channelNumber + ")")
                        End If
                        ' Set the offset for each channel
                        If MyAgilent34970A.channelArray(i).offsetTextBox.Text <> "" Then
                            SendCommand("CALCulate:SCALe:OFFSet " + MyAgilent34970A.channelArray(i).offsetTextBox.Text + ", (@" + MyAgilent34970A.channelArray(i).channelNumber + ")")
                        Else
                            SendCommand("CALCulate:SCALe:OFFSet 0" + ", (@" + MyAgilent34970A.channelArray(i).channelNumber + ")")
                        End If
                        ' Turn on the scaling mode
                        SendCommand("CALCulate:SCALe:STATe ON, (@" + MyAgilent34970A.channelArray(i).channelNumber + ")")
                        delay(0.1)
                    End If
                Next
                frmSettingUpDevice.Label1.Text = "Setting gain and offset complete."
            End If
            frmSettingUpDevice.pbSettingUp.Value = 70
            SendCommand("ROUTe:SCAN " + gChannelCommandString, False) ' Select the scan list for channels (all configured channels)
            delay(0.5)
            frmSettingUpDevice.Label1.Text = "Channel list set complete"
            SendCommand("ROUTe:CHANnel:DELay 0.02, " + gChannelCommandString, False) ' Set the time intervals between the scan of each channel per sweep
            delay(0.5)
            frmSettingUpDevice.Label1.Text = "Scanning time interval between each channel per sweep set to 0.02s"
            ' Determine the trigger count
            If MyAgilent34970A.myProgram.Mode = 2 Then
                SendCommand("TRIGger:COUNt " + txtDAQNumberofReadings.Text, False) ' Trigger count is specified
            ElseIf MyAgilent34970A.myProgram.Mode = 0 Or MyAgilent34970A.myProgram.Mode = 1 Then
                SendCommand("TRIGger:COUNt INFINITY", False) ' Otherwise set to infinity and stopped by user
            End If
            SendCommand("TRIGger:SOURce TIMer", False) ' Set the trigger mode to TIMER (timed trigger)
            frmSettingUpDevice.pbSettingUp.Value = 75
            SendCommand("TRIGger:TIMer " + MyAgilent34970A.ReadingsVal.readingInterval, False) ' Set the time interval between each sweep
            SendCommand("FORMat:READing:TIME:TYPE RELative", False) ' Format the reading time to show the time value from the start of the scan
            delay(1)
            SendCommand("*OPC?", True) ' Wait for instrument to setup
            delay(1)
            SendCommand("ROUTe:SCAN:SIZE?", True) ' Get the number of channels to be scanned
            delay(0.1)
            MyAgilent34970A.ReadingsVal.numReadings = MyAgilent34970A.ReadingsVal.numChannel ' Calculate total number of readings
            delay(0.2)
            frmSettingUpDevice.Label1.Text = "Setting up complete!"
            SendCommand("INITiate", False) ' Trigger instrument
            frmSettingUpDevice.pbSettingUp.Value = 100
            delay(0.2)
        Catch ex As Exception
            MsgBox("Instrument set up error: " + vbLf + vbLf + ex.ToString)
            End
        End Try
    End Sub

    ''' <summary>
    ''' Get the readings
    ''' </summary>
    ''' <param name="iteration">Indicate in which time the readings are being taken</param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Sub Readings(ByVal iteration As Integer)
        ' Definite Block Length Format: # <non-zero digit> <digits> <8-bit data bytes>
        Dim I As Long ' Used for a loop
        Dim tempReadings As String ' The data received from the instrument in Definite Block Length Format 
        Dim nonZeroDigit As Integer = 0 ' Represent <non-zero digit>
        Dim digits As Integer = 0 ' Represent <digits>
        Dim sum As Integer = 0 ' Represent the number of bytes of <8-bit data bytes>
        Dim actualSum As Integer = 0 ' The number of characters received
        Dim expectSum As Integer = 0 ' The number of characters expected
        Dim ambientIndex As Integer = MyAgilent34970A.returnAmbientIndex(dgvDAQData) ' Index of Ambient Channel in DataGridView
        Dim channelReading As String()
        Dim amp As String = ""
        Dim volt As String = ""
        Dim freq As String = ""
        Dim phase As String = ""
        Try
            'Wait until instrument is finished taken readings. The instrument is queried until all channels are measured.
            Do
                SendCommand("DATA:POINTS?", True) ' Ask for the number of readings in the non-volatile reading memory of the instrument
                I = MyAgilent34970A.ReadingsVal.dataPoints
                delay(0.2)
                If MyAgilent34970A.ReadingsVal.readyToStop Then
                    Exit Sub
                End If
            Loop Until I >= MyAgilent34970A.ReadingsVal.numReadings
            If Not SendCommand("R?", True) Then ' Read and erase all readings from reading memory
                Exit Sub
            End If
            ' Store the string returned directly from the device
            ' For example, the tempReadings would have a similar format like: #231+2.61400000E+01,+2.62400000E+01
            tempReadings = MyAgilent34970A.ReadingsVal.allReadings
            ' Calculate the length of the string returned. It's 36 in this example provided above
            actualSum = Val(tempReadings.Length)
            ' Calculate the length of the string expected. It's 36 in this example provided above
            expectSum = (MyAgilent34970A.ReadingsVal.numChannel * 16 - 1).ToString.Length + MyAgilent34970A.ReadingsVal.numChannel * 16 + 1 '- 1 + 2 = +1
            ' If not equal, it means that some data is missing
            If actualSum <> expectSum Then
                v_UpdateError("Actual Length <> Expected Length! Data Returned: " + tempReadings)
                Exit Sub
            End If
            ' Get the number of the digits of the value, which indicate the number of digits of all the readings for each scan. It's 2 in this example provided above
            nonZeroDigit = Convert.ToInt32(tempReadings(1)) - 48 ' character 2 to integer 2
            ' The number of digits of all the readings for each scan ' It's 31 in this example provided above
            digits = Val(tempReadings.Substring(2, nonZeroDigit + 1))
            ' Get all the readings for each scan. It's +2.61400000E+01,+2.62400000E+01 in this example provided above
            tempReadings = tempReadings.Substring(nonZeroDigit + 2)
            ' Calculate the actual sum. It's 31 in this example provided above
            sum = Val(tempReadings.Length)
            ' If digits and sum don't match each other, update the error log
            If digits <> sum Then
                v_UpdateError(sum.ToString + vbLf + tempReadings + vbLf + vbLf)
                Exit Sub
            End If
            channelReading = tempReadings.Split(New Char() {","c}) ' Split each reading
            ' Add chart points for temperature graph
            For m = 0 To channelReading.Count - 1
                channelReading(m) = sci2double(channelReading(m)).ToString ' Convert scientific notation to decimal format
                MyAgilent34970A.ReadingsVal.TempCurrentReading.Add(channelReading(m)) ' Update the array list
                ' Initialize the seiries in the data chart
                If iteration = 0 Then
                    addSeries(m, gChannelLegendString(m), DataChart, "Duration / HH:MM:SS", "Temperature / °C")
                End If
                If channelReading(m).Contains("9.9E+37") Then ' If the value is 9.9e+37, it means the connection to measuring device is lost. Change the value to 404, which is an impossible temperature
                    addChartPoints(iteration + 1, Double.Parse("404"), gChannelLegendString(m), DataChart)
                Else
                    addChartPoints(iteration + 1, Double.Parse(channelReading(m)), gChannelLegendString(m), DataChart)
                End If
            Next
            ' Add chart points for current (Amp) graph
            If iteration = 0 Then
                addSeries(0, "Current", CurrentChart, "Duration / HH:MM:SS", "Current / A")
            End If
            addChartPoints(iteration + 1, MyRadian.MetricData.Amp, "Current", CurrentChart)

            ' For the first time, just initialize all the array lists, then update each one seperately
            If iteration = 0 Then
                For calcI = 0 To MyAgilent34970A.ReadingsVal.numReadings - 1
                    MyAgilent34970A.ReadingsVal.TempMaxReading.Add(String.Copy(Math.Round(Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(calcI)), 3, MidpointRounding.AwayFromZero).ToString))
                    MyAgilent34970A.ReadingsVal.TempMinReading.Add(String.Copy(Math.Round(Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(calcI)), 3, MidpointRounding.AwayFromZero).ToString))
                    MyAgilent34970A.ReadingsVal.TempSumReading.Add(String.Copy(Math.Round(Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(calcI)), 3, MidpointRounding.AwayFromZero).ToString))
                    MyAgilent34970A.ReadingsVal.TempAveReading.Add(String.Copy(Math.Round(Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(calcI)), 3, MidpointRounding.AwayFromZero).ToString))
                    If Not ambientIndex = -1 Then ' Temperature risen is defined as the temperature subtracted by the room/ambient temperature
                        MyAgilent34970A.ReadingsVal.TempRiseReading.Add(String.Copy((Math.Round(Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(calcI)) - Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(ambientIndex)), 3, MidpointRounding.AwayFromZero)).ToString))
                    Else ' There is no channel named Ambient
                        MyAgilent34970A.ReadingsVal.TempRiseReading.Add("N/A")
                    End If
                Next
            Else
                For maxI As Integer = 0 To MyAgilent34970A.ReadingsVal.numReadings - 1
                    ' Update the maximum array list
                    If Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(maxI)) > Val(MyAgilent34970A.ReadingsVal.TempMaxReading(maxI)) Then
                        MyAgilent34970A.ReadingsVal.TempMaxReading(maxI) = String.Copy(MyAgilent34970A.ReadingsVal.TempCurrentReading(maxI))
                    End If
                    ' Update the minimum array list
                    If Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(maxI)) < Val(MyAgilent34970A.ReadingsVal.TempMinReading(maxI)) Then
                        MyAgilent34970A.ReadingsVal.TempMinReading(maxI) = String.Copy(MyAgilent34970A.ReadingsVal.TempCurrentReading(maxI))
                    End If
                    ' Update the average array list and the temp rise array list
                    MyAgilent34970A.ReadingsVal.TempSumReading(maxI) = String.Copy((Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(maxI)) + Val(MyAgilent34970A.ReadingsVal.TempSumReading(maxI))).ToString)
                    MyAgilent34970A.ReadingsVal.TempAveReading(maxI) = String.Copy(Math.Round(Val(MyAgilent34970A.ReadingsVal.TempSumReading(maxI) / (iteration + 1)), 3, MidpointRounding.AwayFromZero).ToString)
                    If Not ambientIndex = -1 Then
                        MyAgilent34970A.ReadingsVal.TempRiseReading(maxI) = String.Copy((Math.Round(Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(maxI)) - Val(MyAgilent34970A.ReadingsVal.TempCurrentReading(ambientIndex)), 3, MidpointRounding.AwayFromZero)).ToString)
                    Else
                        MyAgilent34970A.ReadingsVal.TempRiseReading(maxI) = "N/A"
                    End If
                Next
            End If
            inputing(iteration) ' Input them into DataGridView
            ' Set Current, Voltage, Phase and Frequency. Change the display format depending on the value. If the checkbox is not checked, just display N/A

            gReadingComplete = True ' Set to true because this iteration of reading is finished
            saveInstantData(MyAgilent34970A.ReadingsVal.numReadings, iteration, gLogFileName, gWriteInstantDataError) ' Immediately save the data
            saveInstantData(MyAgilent34970A.ReadingsVal.numReadings, iteration, gLogFileNameTemp, gWriteInstantTempDataError) ' Immediately save the temporary data
            MyAgilent34970A.ReadingsVal.TempCurrentReading.Clear() ' Clear the current reading as it is already saved into the data file
            MyAgilent34970A.ReadingsVal.totalReadings = iteration + 1 ' increment the number of readings taken by 1
        Catch ex As Exception
            MessageBox.Show("Error received in reading the data from the device" + vbLf + vbLf + ex.ToString, "Error In Getting Readings")
            End
        End Try
    End Sub

    ''' <summary>
    ''' Put the current readings in the DataGridView
    ''' </summary>
    ''' <param name="iteration">Indicate in which times the readings are being taken</param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Public Sub inputing(ByVal iteration As Integer)
        ' For each channel
        For index = 0 To MyAgilent34970A.ReadingsVal.TempCurrentReading.Count - 1
            setDataGridViewReading(dgvDAQData, MyAgilent34970A.ReadingsVal.TempCurrentReading(index), index, 0)
            setDataGridViewReading(dgvDAQData, MyAgilent34970A.ReadingsVal.TempMaxReading(index), index, 1)
            setDataGridViewReading(dgvDAQData, MyAgilent34970A.ReadingsVal.TempMinReading(index), index, 2)
            setDataGridViewReading(dgvDAQData, MyAgilent34970A.ReadingsVal.TempAveReading(index), index, 3)
            setDataGridViewReading(dgvDAQData, MyAgilent34970A.ReadingsVal.TempRiseReading(index), index, 4)
        Next
    End Sub

    ''' <summary>
    ''' Convert scientific notation to double, like +2.5230000E+01 -> 25.23
    ''' </summary>
    ''' <param name="original">Scientific notation</param>
    ''' <returns>Double notation</returns>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Function sci2double(original As String) As Double
        Dim original_splited() As String
        Dim value_original As Double
        original_splited = original.Split("E")
        value_original = Convert.ToDouble(original_splited(0)) * 10 ^ CInt(original_splited(1))
        Return value_original
    End Function

    ''' <summary>
    ''' Clear the parameters and the readings
    ''' </summary>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Public Sub ClearAll()
        lbDAQInformationStartTime.Text = "Start Time: " ' Reset start time label
        lbDAQInformationTerminateTime.Text = "Terminate Time: " ' Reset Terminate Time
        lbDAQInformationDuration.Text = "Duration: 00:00:00" ' Reset duration
        lbDAQInformationNumofReadings.Text = "Total Number of Readings: " ' Reset number of readings

        ' Enable buttons and textboxes
        btnDAQConnectionConnect.Enabled = True
        cbDAQParametersReadingIntervals.Enabled = True
        rbDAQParametersFRTD.Enabled = True
        rbDAQParametersThermocouple.Enabled = True
        btnDAQConnectionConnect.Enabled = True

        ' Clear those array lists. These are used to store values for each scan
        MyAgilent34970A.ReadingsVal.TempCurrentReading.Clear()
        MyAgilent34970A.ReadingsVal.TempMaxReading.Clear()
        MyAgilent34970A.ReadingsVal.TempMinReading.Clear()
        MyAgilent34970A.ReadingsVal.TempRiseReading.Clear()
        MyAgilent34970A.ReadingsVal.TempSumReading.Clear()

        Me.StopWatch.Reset() ' Reset the stopwatch

        ' Clear those two plots; series and titles
        DataChart.Series.Clear()
        DataChart.Titles.Clear()
        CurrentChart.Series.Clear()
        CurrentChart.Titles.Clear()

        ' Reset compare labels, items in combo boxes
        lblDAQCompareCompare1.Text = ""
        lblDAQCompareCompare2.Text = ""
        lblDAQCompareCompare3.Text = ""
        lblDAQCompareCompare4.Text = ""
        lblDAQCompareCompare5.Text = ""
        lblDAQCompareCompare6.Text = ""
        lblDAQCompareCompare7.Text = ""
        lblDAQCompareCompare8.Text = ""
        lblDAQCompareCompare9.Text = ""
        lblDAQCompareCompare10.Text = ""
        txtDAQCompare1.Text = ""
        txtDAQCompare2.Text = ""
        txtDAQCompare3.Text = ""
        txtDAQCompare4.Text = ""
        txtDAQCompare5.Text = ""
        cbDAQCompare1.Items.Clear()
        cbDAQCompare2.Items.Clear()
        cbDAQCompare3.Items.Clear()
        cbDAQCompare4.Items.Clear()
        cbDAQCompare5.Items.Clear()
        cbDAQCompare6.Items.Clear()
        cbDAQCompare7.Items.Clear()
        cbDAQCompare8.Items.Clear()
        cbDAQCompare9.Items.Clear()
        cbDAQCompare10.Items.Clear()

        gWriteInstantDataError = False ' Clear error flag that is used to indicate the error of writing data to the file
        dgvDAQData.Rows.Clear() ' Clear DataGridView
    End Sub

    ''' <summary>
    ''' if check status of FRTD changes
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbDAQParametersFRTD_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDAQParametersFRTD.CheckedChanged
        If rbDAQParametersFRTD.Checked Then
            rbDAQParametersThermocouple.Checked = False
            MyAgilent34970A.myProgram.MeasureDevice = 0 ' 0: FRTD, 1: Thermocouple
        End If
        ' Uncheck and disable the channels from 11 to 20 in each module, since user selects FRTD
        For i = 0 To MyAgilent34970A.numChannel - 1
            If (i >= 10 And i <= 19) Or (i >= 30 And i <= 39) Or (i >= 50 And i <= 59) Then
                MyAgilent34970A.channelArray(i).enableCheckBox.Checked = Not rbDAQParametersFRTD.Checked
                MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = Not rbDAQParametersFRTD.Checked
            End If
        Next
    End Sub

    ''' <summary>
    ''' if the check status of thermocouple changes
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbDAQParametersThermocouple_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rbDAQParametersThermocouple.CheckedChanged
        If rbDAQParametersThermocouple.Checked Then
            rbDAQParametersFRTD.Checked = False
            MyAgilent34970A.myProgram.MeasureDevice = 1 ' 0: FRTD, 1: Thermocouple
        End If
        ' Enable the channels from 11 to 20 in each module, since user selcts Thermocouple
        For i = 0 To MyAgilent34970A.numChannel - 1
            If (i >= 10 And i <= 19) Or (i >= 30 And i <= 39) Or (i >= 50 And i <= 59) Then
                MyAgilent34970A.channelArray(i).enableCheckBox.Enabled = rbDAQParametersThermocouple.Checked
            End If
        Next
    End Sub

    ''' <summary>
    ''' This timer will be used when the program is getting readings
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Public Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles TimerRunning.Tick
        Dim elapsed As TimeSpan = Me.StopWatch.Elapsed ' elapsed time of stopwatch (started when the program started taking readings)
        Dim sec As Integer ' elapsed seconds
        Dim min As Integer ' elapsed minutes
        Dim hour As Integer ' elapsed hours
        Dim totalSec As Integer ' user input seconds
        Dim totalMin As Integer ' user input minutes
        Dim totalHour As Integer ' user input hours
        Dim sendEmail As Boolean ' Whether it's time to send a warning message
        Dim exceedThreshold As Boolean = False ' Temperature exceeds threshold? Set to false for initialization
        setLabel(lbDAQInformationDuration, "Duration: " + String.Format("{0:00}:{1:00}:{2:00}", Math.Floor(elapsed.TotalHours), elapsed.Minutes, elapsed.Seconds))
        ' Update the progress bar depending on how much time has already passed
        If MyAgilent34970A.myProgram.Mode = 1 Then
            sec = Val(String.Format("{0:00}", elapsed.Seconds))
            min = Val(String.Format("{0:00}", elapsed.Minutes))
            hour = Val(String.Format("{0:00}", Math.Floor(elapsed.TotalHours)))
            totalSec = MyAgilent34970A.ReadingsVal.second
            totalMin = MyAgilent34970A.ReadingsVal.minute
            totalHour = MyAgilent34970A.ReadingsVal.hour
            setProgressBar((sec + 60 * min + 3600 * hour) / (totalSec + totalMin * 60 + totalHour * 3600) * 100, ProgressBar1) ' Convert to total number of seconds and calculate the how much has been done
        End If
        ' For Duration mode, if the time met 
        If MyAgilent34970A.myProgram.Mode = 1 Then
            If lbDAQInformationDuration.Text = ("Duration: " + MyAgilent34970A.ReadingsVal.hour.ToString("00") + ":" + MyAgilent34970A.ReadingsVal.minute.ToString("00") + ":" + MyAgilent34970A.ReadingsVal.second.ToString("00")) Then
                MyAgilent34970A.ReadingsVal.timerCountDown = True ' If the program has been running for an amount of time specified by the user, it is ready to be stopped
            End If
            askLabel(lbDAQInformationDuration, "Duration: " + MyAgilent34970A.ReadingsVal.hour.ToString("00") + ":" + MyAgilent34970A.ReadingsVal.minute.ToString("00") + ":" + MyAgilent34970A.ReadingsVal.second.ToString("00"))
        End If
        ' If ready to stop
        If MyAgilent34970A.ReadingsVal.timerCountDown Or MyAgilent34970A.ReadingsVal.readyToStop Then
            If gDAQStateMachine = eDeviceState.WaitingForNextReading Then
                
                gDAQStateMachine = eDeviceState.ReadingComplete
                SendCommand("ABORt") ' Abort the program
                MyAgilent34970A.myProgram.DeviceRunning = False
                setLabel(lbDAQInformationTerminateTime, "Terminate Time: " + DateTime.Now) ' Set terminate time
                setLabel(lbDAQInformationNumofReadings, "Total Number of Readings: " + MyAgilent34970A.ReadingsVal.totalReadings.ToString) ' Update the total number of readings
                TimerRunning.Stop() ' Stop the timer TimerRunning
                Me.StopWatch.Stop() ' Stop the stop watch
                ProgressBar1.Value = 100
                '========Automatic Shutdown===============
                If chbAutomaticShutDown.Checked Then
                    chkCloseLoop.Checked = False ' Uncheck closeCurrentLoop
                    delay(0.5)
                    btnVoltageOff_Click(sender, e) ' set voltage back to 0
                    delay(3)
                    closeButton_Click(sender, e)
                End If
                '=========================================
                '==============
                gStateMachineState = eState.Idle
                gLastState = eState.ReadingMetics
                '==============
                If cbDAQEmailNotification.Checked And cbDAQEmailTestDone.Checked Then ' send an email if user decides to send one when the test is finished
                    MyAgilent34970A.sendEmail(True, False) ' Test is complete and this is not a warning message
                End If
                ' Inform the user depending on if the program lost the access to the path or not during the test
                If Not gWriteInstantDataError Then ' If there no error occus when writing data to the file user selected before taking readings
                    File.Delete("C:\Temp\Temperature Rise Test\temp.csv")
                    MessageBox.Show("The test now is completed." + vbLf + "Totally " + MyAgilent34970A.ReadingsVal.totalReadings.ToString + " reading(s) are taken." + vbLf + "The data file has already been saved in the path """ + gLogFileName + """", "Test Completed", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("The test now is completed." + vbLf + "Totally " + MyAgilent34970A.ReadingsVal.totalReadings.ToString + " reading(s) are taken." + vbLf + "However, during the test, because the program could not find the data file, the data has been saved to a template file in the path ""C:\Temp\Temperature Rise Test\temp.csv""", "Test Completed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                ' The program needs to go back to Standby state
                gDAQStateMachine = eDeviceState.Standby
            Else
                ' Or the program needs to wait until the current readings are taken
                gDAQStateMachine = eDeviceState.StoppingReading
            End If
        End If
        ' If it's time to check if it's needed to send an warning email. Every other 10 min.
        If My.Computer.Clock.TickCount >= gWarningEndTicks Then
            sendEmail = True
            gWarningEndTicks += 600000 ' 600000ms = 600s = 10min, update the time to send warning
        Else
            sendEmail = False
        End If
        ' If one iteration of reading is complete
        If gReadingComplete Then
            For i = 0 To MyAgilent34970A.compareArray.Count - 1
                ' If user select two channels to compare
                If MyAgilent34970A.compareArray(i).compare1.Text <> "" And MyAgilent34970A.compareArray(i).compare2.Text <> "" Then
                    If MyAgilent34970A.displayCompare(i) Then ' Calculate the temperature difference and display the values
                        If sendEmail Then ' If it's time to send a warning message
                            If MyAgilent34970A.compareArray(i).warning Then ' If the temperature difference exceeds threshold for a channel
                                gWarningEmailBody += String.Concat(vbLf, MyAgilent34970A.compareArray(i).label1.Text, " ", "(", MyAgilent34970A.compareArray(i).compare1.Text, "): ", MyAgilent34970A.compareArray(i).compareValue1.ToString, "°C", vbLf, MyAgilent34970A.compareArray(i).label2.Text, " ", "(", MyAgilent34970A.compareArray(i).compare2.Text, "): ", MyAgilent34970A.compareArray(i).compareValue2.ToString, "°C", vbLf, "Temperature Difference: ", MyAgilent34970A.compareArray(i).difference.Text, "°C", vbLf, "Threshold: ", MyAgilent34970A.compareArray(i).threshold.Text, "°C", vbLf)
                                exceedThreshold = True
                            End If
                        End If
                    End If
                    ' display the channel name based on the user's selection of channel number
                    MyAgilent34970A.compareArray(i).label1.Text = dgvDAQData.Rows(MyAgilent34970A.returnRowIndex(1, MyAgilent34970A.compareArray(i).compare1.Text, dgvDAQData)).HeaderCell.Value.ToString.Split("@")(0)
                    MyAgilent34970A.compareArray(i).label2.Text = dgvDAQData.Rows(MyAgilent34970A.returnRowIndex(1, MyAgilent34970A.compareArray(i).compare2.Text, dgvDAQData)).HeaderCell.Value.ToString.Split("@")(0)
                Else
                    MyAgilent34970A.compareArray(i).difference.Text = ""
                End If
            Next
            If sendEmail And exceedThreshold Then
                MyAgilent34970A.sendEmail(False, True) ' Test is not complete and it is a warning message
                gWarningEmailBody = ""
            End If
        End If
    End Sub

    ''' <summary>
    ''' Let the user saved the data file, and initialize the title in the file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Public Function saveLogFile(Optional ByVal filepath As String = "") As Boolean
        Dim saveFileDialog1 As New SaveFileDialog
        Dim w_a_success As Boolean = False ' Check if the file is ready to be written
        Dim FileNum As Integer = FreeFile()
        Dim saveResult As DialogResult
        If filepath = "" Then
            saveFileDialog1.InitialDirectory = "\\am.bm.net\uslafdfs01\GE_Lab\"
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv"
            saveFileDialog1.FilterIndex = 1
            saveFileDialog1.RestoreDirectory = True
            If saveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                If filepath = "" Then
                    filepath = saveFileDialog1.FileName.ToString()
                End If
                gLogFileName = filepath
                While w_a_success = False ' If the file is not ready to be written
                    Try
                        FileOpen(FileNum, saveFileDialog1.FileName.ToString(), OpenMode.Output) 'Opens a file for input
                        w_a_success = True ' The file is ready to be written
                    Catch ex As Exception
                        saveResult = MessageBox.Show("File may be open. Check file status, close and try again.", "File Opened", MessageBoxButtons.RetryCancel)
                        If saveResult = DialogResult.Retry Then ' If the user presses "retry", the program will check again
                            Continue While
                        ElseIf saveResult = DialogResult.Cancel Then
                            Exit While
                        End If
                    End Try
                End While
                ' If the file is ready to be written
                If w_a_success = True Then
                    PrintLine(FileNum, "Title:,Temperature Rise Test Measurement")
                    PrintLine(FileNum, "Date:," + Now.Date)
                    Print(FileNum, "Instrument:," + MyAgilent34970A.Parameters.Manufacturer + "," + MyAgilent34970A.Parameters.ModelNumber + "," + MyAgilent34970A.Parameters.SerialNumber + "," + MyAgilent34970A.Parameters.Firmware)
                    If rbDAQParametersFRTD.Checked Then
                        PrintLine(FileNum, "Measuring Device:,4-Wire RTD")
                    ElseIf rbDAQParametersThermocouple.Checked Then
                        PrintLine(FileNum, "Measuring Device:,Thermocouple")
                    End If
                    PrintLine(FileNum, "COM Port:," + cbDAQConnectionComPort.Text)
                    Print(FileNum, "Slot 100: ,")
                    If MyAgilent34970A.ReadingsVal.checkModule(0).Split(",")(1) = "34901A" Then
                        PrintLine(FileNum, "34901A")
                    Else
                        PrintLine(FileNum, "None")
                    End If
                    Print(FileNum, "Slot 200:,")
                    If MyAgilent34970A.ReadingsVal.checkModule(1).Split(",")(1) = "34901A" Then
                        PrintLine(FileNum, "34901A")
                    Else
                        PrintLine(FileNum, "None")
                    End If
                    Print(FileNum, "Slot 300:,")
                    If MyAgilent34970A.ReadingsVal.checkModule(2).Split(",")(1) = "34901A" Then
                        PrintLine(FileNum, "34901A")
                    Else
                        PrintLine(FileNum, "None")
                    End If
                    FileClose(FileNum)
                    Return True
                Else
                    FileClose(FileNum)
                    Return False
                End If

            End If
        Else ' for writing data to temporary file, there's no need to pop up the dialog box to ask user where to save the log
            gLogFileNameTemp = filepath
            FileOpen(FileNum, gLogFileNameTemp, OpenMode.Output)
            PrintLine(FileNum, "Title:,Temperature Rise Test Measurement")
            PrintLine(FileNum, "Date:," + Now.Date)
            PrintLine(FileNum, "Instrument:," + MyAgilent34970A.Parameters.ModelNumber)
            PrintLine(FileNum, "COM Port:," + cbDAQConnectionComPort.Text)
            Print(FileNum, "Slot 100: ,")
            If MyAgilent34970A.ReadingsVal.checkModule(0).Split(",")(1) = "34901A" Then
                PrintLine(FileNum, "34901A")
            Else
                PrintLine(FileNum, "None")
            End If
            Print(FileNum, "Slot 200:,")
            If MyAgilent34970A.ReadingsVal.checkModule(1).Split(",")(1) = "34901A" Then
                PrintLine(FileNum, "34901A")
            Else
                PrintLine(FileNum, "None")
            End If
            Print(FileNum, "Slot 300:,")
            If MyAgilent34970A.ReadingsVal.checkModule(2).Split(",")(1) = "34901A" Then
                PrintLine(FileNum, "34901A")
            Else
                PrintLine(FileNum, "None")
            End If
            FileClose(FileNum)
        End If
        Return False
    End Function

    ''' <summary>
    ''' Save the data immediately
    ''' </summary>
    ''' <param name="numberOfReadings"></param>
    ''' <param name="index"></param>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Public Sub saveInstantData(ByVal numberOfReadings As Long, ByVal index As Integer, ByVal filepath As String, ByRef bError As Boolean)
        Dim FileNum As Integer = FreeFile()
        ' Always check if the program has the access to the path, but if not, the program will create a temp file in "C:\Temp_Temperature_Rise_Test"
        If Not bError Then
            Try
                FileOpen(FileNum, filepath, OpenMode.Append)
                If index = 0 Then
                    PrintLine(FileNum, "Total Channel Number:," + MyAgilent34970A.ReadingsVal.numChannel.ToString) ' Write total number of readings in the data file
                    Write(FileNum, "Time") ' Write time
                    Write(FileNum, "Current(Amps)") ' Write Current
                    For i = 0 To dgvDAQData.RowCount - 1
                        If Not i = dgvDAQData.RowCount - 1 Then
                            Write(FileNum, dgvDAQData.Rows(i).HeaderCell.Value.ToString)
                        Else
                            WriteLine(FileNum, dgvDAQData.Rows(i).HeaderCell.Value.ToString)
                        End If
                    Next
                End If
                ' write readings
                For j = 0 To numberOfReadings - 1
                    ' For each line, write time and current first
                    If j = 0 Then
                        Write(FileNum, Now.ToString)
                        If Math.Abs(MyRadian.MetricData.Amp) < 10 Then
                            Write(FileNum, MyRadian.MetricData.Amp.ToString("0.00000"))
                        ElseIf Math.Abs(MyRadian.MetricData.Amp) < 100 Then
                            Write(FileNum, MyRadian.MetricData.Amp.ToString("00.0000"))
                        Else
                            Write(FileNum, MyRadian.MetricData.Amp.ToString("000.000"))
                        End If
                    End If
                    ' Append data to the end of each line
                    If Not j = numberOfReadings - 1 Then
                        Write(FileNum, MyAgilent34970A.ReadingsVal.TempCurrentReading(j))
                    Else ' not only append data but also put a line feed
                        WriteLine(FileNum, MyAgilent34970A.ReadingsVal.TempCurrentReading(j))
                    End If
                Next
                FileClose(FileNum)
            Catch
                bError = True
                v_UpdateError("Could not find the data file, and the program has created a temp file")
            End Try
        Else
            Exit Sub
        End If


    End Sub

    Public Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim worker As System.ComponentModel.BackgroundWorker = CType(sender, System.ComponentModel.BackgroundWorker)
        e.Result = readingProgress(worker, e)
    End Sub

    ''' <summary>
    ''' The timer for the whole program. There are 9 states: Initial, Connected, Identified, Standby, Getting Reading, Stopping Reading, Waiting for Next Reading, Waiting for Set Up, Reading Complete
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Public Sub Timer_Program_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerProgram.Tick
        lblTime.Text = DateTime.Now.ToString
        If gDAQStateMachine = eDeviceState.Initial Then
            txtMachineState.Text = "Initial"
            btnDAQConnectionConnect.Text = "Connect"
        ElseIf gDAQStateMachine = eDeviceState.Connected Then
            txtMachineState.Text = "Connected"
        ElseIf gDAQStateMachine = eDeviceState.Identified Then
            txtMachineState.Text = "Identified"
            btnDAQStartStopReading.BackColor = Color.Transparent
            ' Check if the program is ready to jump to Stanby State
            If MyAgilent34970A.Identified2Standby Then ' Check if user selected reading interval, measuring device, enabled at least one channel
                If rbDuration.Checked Then ' In Duration mode, let user enter hours, minutes and seconds
                    If (txtDAQCountDownHH.Text.Replace(" ", "") <> "" Or txtDAQCountDownMM.Text.Replace(" ", "") <> "" Or txtDAQCountDownSS.Text.Replace(" ", "") <> "") And Not (Val(txtDAQCountDownHH.Text.Replace(" ", "")) = 0 And Val(txtDAQCountDownMM.Text.Replace(" ", "")) = 0 And Val(txtDAQCountDownSS.Text.Replace(" ", "")) = 0) Then
                        gDAQStateMachine = eDeviceState.Standby
                    End If
                ElseIf rbFree.Checked Then ' In free mode, just jump to standby state
                    gDAQStateMachine = eDeviceState.Standby
                ElseIf rbNumofReadings.Checked Then ' In Number of Reading mode, let user enter the number of readings
                    If txtDAQNumberofReadings.Text.Replace(" ", "") <> "" Then
                        gDAQStateMachine = eDeviceState.Standby
                    End If
                End If
            End If
        ElseIf gDAQStateMachine = eDeviceState.Standby Then
            txtMachineState.Text = "Standby"
            btnDAQStartStopReading.BackColor = Color.PaleGreen
            ' Check if the program needs to go back to Identified state
            If Not MyAgilent34970A.Identified2Standby Then
                gDAQStateMachine = eDeviceState.Identified
            End If
            If rbDuration.Checked Then
                If (txtDAQCountDownHH.Text.Replace(" ", "") = "" And txtDAQCountDownMM.Text.Replace(" ", "") = "" And txtDAQCountDownSS.Text.Replace(" ", "") = "") Or (Val(txtDAQCountDownHH.Text.Replace(" ", "")) = 0 And Val(txtDAQCountDownMM.Text.Replace(" ", "")) = 0 And Val(txtDAQCountDownSS.Text.Replace(" ", "")) = 0) Then
                    gDAQStateMachine = eDeviceState.Identified
                End If
            ElseIf rbNumofReadings.Checked Then
                If txtDAQNumberofReadings.Text = "" Or txtDAQNumberofReadings.Text = "0" Or txtDAQNumberofReadings.Text.StartsWith("-") Then
                    gDAQStateMachine = eDeviceState.Identified
                End If
            End If
            ProgressBar1.Value = 0 ' Make sure the progress bar value is 0 when not running

        ElseIf gDAQStateMachine = eDeviceState.GettingReading Then
            txtMachineState.Text = "Getting Reading"
        ElseIf gDAQStateMachine = eDeviceState.StoppingReading Then
            txtMachineState.Text = "Stopping Reading"
        ElseIf gDAQStateMachine = eDeviceState.WaitingForSetup Then
            txtMachineState.Text = "Waiting For Setup"
        ElseIf gDAQStateMachine = eDeviceState.WaitingForNextReading Then
            txtMachineState.Text = "Waiting For Next Reading"
        ElseIf gDAQStateMachine = eDeviceState.ReadingComplete Then
            txtMachineState.Text = "Reading Complete"
        End If

        ' Adjust the start/stop reading button text and color
        If MyAgilent34970A.myProgram.DeviceRunning Then
            btnDAQStartStopReading.Text = "Stop Reading"
            btnDAQStartStopReading.BackColor = Color.Red
        Else
            btnDAQStartStopReading.Text = "Start Reading"
            btnDAQStartStopReading.BackColor = Color.PaleGreen
        End If

        ' Users are only able to select baud rate and com port when the state machine is at initial state
        txtDAQConnectionBaudrate.Enabled = (gDAQStateMachine = eDeviceState.Initial)
        cbDAQConnectionComPort.Enabled = (gDAQStateMachine = eDeviceState.Initial)

        ' Enable/disable the radio buttons if program is not running/running
        cbDAQParametersReadingIntervals.Enabled = Not MyAgilent34970A.myProgram.DeviceRunning
        rbDAQParametersFRTD.Enabled = Not MyAgilent34970A.myProgram.DeviceRunning
        rbDAQParametersThermocouple.Enabled = Not MyAgilent34970A.myProgram.DeviceRunning
        rbFree.Enabled = Not MyAgilent34970A.myProgram.DeviceRunning
        rbDuration.Enabled = Not MyAgilent34970A.myProgram.DeviceRunning
        rbNumofReadings.Enabled = Not MyAgilent34970A.myProgram.DeviceRunning

        ' Enable/disable name textbox, offset textbox and gain textbox based on the status of calibration checkbox and channel enable check box
        For i = 0 To MyAgilent34970A.numChannel - 1
            MyAgilent34970A.channelArray(i).nameComboBox.Enabled = MyAgilent34970A.channelArray(i).enableCheckBox.Checked
            MyAgilent34970A.channelArray(i).offsetTextBox.Enabled = (chbDAQCalibration.Checked And MyAgilent34970A.channelArray(i).enableCheckBox.Checked)
            MyAgilent34970A.channelArray(i).gainTextBox.Enabled = (chbDAQCalibration.Checked And MyAgilent34970A.channelArray(i).enableCheckBox.Checked)
        Next
    End Sub

    ''' <summary>
    ''' Set the variable readingInterval to the value the user selects
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ReadingIntervals_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbDAQParametersReadingIntervals.LostFocus
        MyAgilent34970A.ReadingsVal.readingInterval = cbDAQParametersReadingIntervals.Text
    End Sub

    ''' <summary>
    ''' Check if the number of readings specified by the user is of integer type
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+gyr 2016</remarks>
    Private Sub tbDAQNumberofReadings_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDAQNumberofReadings.LostFocus
        MyAgilent34970A.ReadingsVal.totalNumofReadings = CInt(Val(txtDAQNumberofReadings.Text))
    End Sub

    ''' <summary>
    ''' Load Agilent 34970A base
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub frmCurrent_Temperature_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        MyAgilent34970A.Parameters.init() ' Initialize instrument identity to blank
        ' Get port names
        Dim portnames As String() = IO.Ports.SerialPort.GetPortNames
        For i = 0 To IO.Ports.SerialPort.GetPortNames.Count - 1
            portnames(i) = portnames(i).Replace("COM", "")
        Next
        cbPacPowerPorts.Items.Clear()
        ' Add port and baudrate
        cbDAQConnectionComPort.Items.AddRange(portnames)
        txtDAQConnectionBaudrate.SelectedItem = "57600" ' Set default baud rate to 57600
        ' add port names for the pacific power source...
        cbPacPowerPorts.Items.AddRange(portnames)

        ' Add email address
        chbDAQEmailAddress.Items.Add("")
        chbDAQEmailAddress.Items.Add("Yang.Cheng2")
        chbDAQEmailAddress.Items.Add("Alan.Espidio")
        chbDAQEmailAddress.Items.Add("Randy.Emery")
        chbDAQEmailAddress.Items.Add("Frank.Boudreau")
        'chbDAQEmailAddress.Items.Add("<New...>")

        ' Initialize the seetings for Channel, enableChannel, gain and offset
        ' Store the value of the channel name
        If My.Settings.Channel Is Nothing Then
            My.Settings.Channel = New System.Collections.Specialized.StringCollection
        End If
        ' Store the value of the channel enable checkbox
        'If My.Settings.enableChannel Is Nothing Then
        '    My.Settings.enableChannel = New System.Collections.Specialized.StringCollection
        'End If
        If My.Settings.checkStatus Is Nothing Then
            My.Settings.checkStatus = New System.Collections.Specialized.StringCollection
        End If
        ' Store the value of the gain
        If My.Settings.Gain Is Nothing Then
            My.Settings.Gain = New System.Collections.Specialized.StringCollection
        End If
        ' Store the value of the offset
        If My.Settings.Offset Is Nothing Then
            My.Settings.Offset = New System.Collections.Specialized.StringCollection
        End If

        ' Initialize channel
        MyAgilent34970A.initializeChannel(0, cbDAQDataEnableChannel101, cbDAQDataChannel101, txtDAQOffset101, txtDAQGain101)
        MyAgilent34970A.initializeChannel(1, cbDAQDataEnableChannel102, cbDAQDataChannel102, txtDAQOffset102, txtDAQGain102)
        MyAgilent34970A.initializeChannel(2, cbDAQDataEnableChannel103, cbDAQDataChannel103, txtDAQOffset103, txtDAQGain103)
        MyAgilent34970A.initializeChannel(3, cbDAQDataEnableChannel104, cbDAQDataChannel104, txtDAQOffset104, txtDAQGain104)
        MyAgilent34970A.initializeChannel(4, cbDAQDataEnableChannel105, cbDAQDataChannel105, txtDAQOffset105, txtDAQGain105)
        MyAgilent34970A.initializeChannel(5, cbDAQDataEnableChannel106, cbDAQDataChannel106, txtDAQOffset106, txtDAQGain106)
        MyAgilent34970A.initializeChannel(6, cbDAQDataEnableChannel107, cbDAQDataChannel107, txtDAQOffset107, txtDAQGain107)
        MyAgilent34970A.initializeChannel(7, cbDAQDataEnableChannel108, cbDAQDataChannel108, txtDAQOffset108, txtDAQGain108)
        MyAgilent34970A.initializeChannel(8, cbDAQDataEnableChannel109, cbDAQDataChannel109, txtDAQOffset109, txtDAQGain109)
        MyAgilent34970A.initializeChannel(9, cbDAQDataEnableChannel110, cbDAQDataChannel110, txtDAQOffset110, txtDAQGain110)
        MyAgilent34970A.initializeChannel(10, cbDAQDataEnableChannel111, cbDAQDataChannel111, txtDAQOffset111, txtDAQGain111)
        MyAgilent34970A.initializeChannel(11, cbDAQDataEnableChannel112, cbDAQDataChannel112, txtDAQOffset112, txtDAQGain112)
        MyAgilent34970A.initializeChannel(12, cbDAQDataEnableChannel113, cbDAQDataChannel113, txtDAQOffset113, txtDAQGain113)
        MyAgilent34970A.initializeChannel(13, cbDAQDataEnableChannel114, cbDAQDataChannel114, txtDAQOffset114, txtDAQGain114)
        MyAgilent34970A.initializeChannel(14, cbDAQDataEnableChannel115, cbDAQDataChannel115, txtDAQOffset115, txtDAQGain115)
        MyAgilent34970A.initializeChannel(15, cbDAQDataEnableChannel116, cbDAQDataChannel116, txtDAQOffset116, txtDAQGain116)
        MyAgilent34970A.initializeChannel(16, cbDAQDataEnableChannel117, cbDAQDataChannel117, txtDAQOffset117, txtDAQGain117)
        MyAgilent34970A.initializeChannel(17, cbDAQDataEnableChannel118, cbDAQDataChannel118, txtDAQOffset118, txtDAQGain118)
        MyAgilent34970A.initializeChannel(18, cbDAQDataEnableChannel119, cbDAQDataChannel119, txtDAQOffset119, txtDAQGain119)
        MyAgilent34970A.initializeChannel(19, cbDAQDataEnableChannel120, cbDAQDataChannel120, txtDAQOffset120, txtDAQGain120)
        MyAgilent34970A.initializeChannel(20, cbDAQDataEnableChannel201, cbDAQDataChannel201, txtDAQOffset201, txtDAQGain201)
        MyAgilent34970A.initializeChannel(21, cbDAQDataEnableChannel202, cbDAQDataChannel202, txtDAQOffset202, txtDAQGain202)
        MyAgilent34970A.initializeChannel(22, cbDAQDataEnableChannel203, cbDAQDataChannel203, txtDAQOffset203, txtDAQGain203)
        MyAgilent34970A.initializeChannel(23, cbDAQDataEnableChannel204, cbDAQDataChannel204, txtDAQOffset204, txtDAQGain204)
        MyAgilent34970A.initializeChannel(24, cbDAQDataEnableChannel205, cbDAQDataChannel205, txtDAQOffset205, txtDAQGain205)
        MyAgilent34970A.initializeChannel(25, cbDAQDataEnableChannel206, cbDAQDataChannel206, txtDAQOffset206, txtDAQGain206)
        MyAgilent34970A.initializeChannel(26, cbDAQDataEnableChannel207, cbDAQDataChannel207, txtDAQOffset207, txtDAQGain207)
        MyAgilent34970A.initializeChannel(27, cbDAQDataEnableChannel208, cbDAQDataChannel208, txtDAQOffset208, txtDAQGain208)
        MyAgilent34970A.initializeChannel(28, cbDAQDataEnableChannel209, cbDAQDataChannel209, txtDAQOffset209, txtDAQGain209)
        MyAgilent34970A.initializeChannel(29, cbDAQDataEnableChannel210, cbDAQDataChannel210, txtDAQOffset210, txtDAQGain210)
        MyAgilent34970A.initializeChannel(30, cbDAQDataEnableChannel211, cbDAQDataChannel211, txtDAQOffset211, txtDAQGain211)
        MyAgilent34970A.initializeChannel(31, cbDAQDataEnableChannel212, cbDAQDataChannel212, txtDAQOffset212, txtDAQGain212)
        MyAgilent34970A.initializeChannel(32, cbDAQDataEnableChannel213, cbDAQDataChannel213, txtDAQOffset213, txtDAQGain213)
        MyAgilent34970A.initializeChannel(33, cbDAQDataEnableChannel214, cbDAQDataChannel214, txtDAQOffset214, txtDAQGain214)
        MyAgilent34970A.initializeChannel(34, cbDAQDataEnableChannel215, cbDAQDataChannel215, txtDAQOffset215, txtDAQGain215)
        MyAgilent34970A.initializeChannel(35, cbDAQDataEnableChannel216, cbDAQDataChannel216, txtDAQOffset216, txtDAQGain216)
        MyAgilent34970A.initializeChannel(36, cbDAQDataEnableChannel217, cbDAQDataChannel217, txtDAQOffset217, txtDAQGain217)
        MyAgilent34970A.initializeChannel(37, cbDAQDataEnableChannel218, cbDAQDataChannel218, txtDAQOffset218, txtDAQGain218)
        MyAgilent34970A.initializeChannel(38, cbDAQDataEnableChannel219, cbDAQDataChannel219, txtDAQOffset219, txtDAQGain219)
        MyAgilent34970A.initializeChannel(39, cbDAQDataEnableChannel220, cbDAQDataChannel220, txtDAQOffset220, txtDAQGain220)
        MyAgilent34970A.initializeChannel(40, cbDAQDataEnableChannel301, cbDAQDataChannel301, txtDAQOffset301, txtDAQGain301)
        MyAgilent34970A.initializeChannel(41, cbDAQDataEnableChannel302, cbDAQDataChannel302, txtDAQOffset302, txtDAQGain302)
        MyAgilent34970A.initializeChannel(42, cbDAQDataEnableChannel303, cbDAQDataChannel303, txtDAQOffset303, txtDAQGain303)
        MyAgilent34970A.initializeChannel(43, cbDAQDataEnableChannel304, cbDAQDataChannel304, txtDAQOffset304, txtDAQGain304)
        MyAgilent34970A.initializeChannel(44, cbDAQDataEnableChannel305, cbDAQDataChannel305, txtDAQOffset305, txtDAQGain305)
        MyAgilent34970A.initializeChannel(45, cbDAQDataEnableChannel306, cbDAQDataChannel306, txtDAQOffset306, txtDAQGain306)
        MyAgilent34970A.initializeChannel(46, cbDAQDataEnableChannel307, cbDAQDataChannel307, txtDAQOffset307, txtDAQGain307)
        MyAgilent34970A.initializeChannel(47, cbDAQDataEnableChannel308, cbDAQDataChannel308, txtDAQOffset308, txtDAQGain308)
        MyAgilent34970A.initializeChannel(48, cbDAQDataEnableChannel309, cbDAQDataChannel309, txtDAQOffset309, txtDAQGain309)
        MyAgilent34970A.initializeChannel(49, cbDAQDataEnableChannel310, cbDAQDataChannel310, txtDAQOffset310, txtDAQGain310)
        MyAgilent34970A.initializeChannel(50, cbDAQDataEnableChannel311, cbDAQDataChannel311, txtDAQOffset311, txtDAQGain311)
        MyAgilent34970A.initializeChannel(51, cbDAQDataEnableChannel312, cbDAQDataChannel312, txtDAQOffset312, txtDAQGain312)
        MyAgilent34970A.initializeChannel(52, cbDAQDataEnableChannel313, cbDAQDataChannel313, txtDAQOffset313, txtDAQGain313)
        MyAgilent34970A.initializeChannel(53, cbDAQDataEnableChannel314, cbDAQDataChannel314, txtDAQOffset314, txtDAQGain314)
        MyAgilent34970A.initializeChannel(54, cbDAQDataEnableChannel315, cbDAQDataChannel315, txtDAQOffset315, txtDAQGain315)
        MyAgilent34970A.initializeChannel(55, cbDAQDataEnableChannel316, cbDAQDataChannel316, txtDAQOffset316, txtDAQGain316)
        MyAgilent34970A.initializeChannel(56, cbDAQDataEnableChannel317, cbDAQDataChannel317, txtDAQOffset317, txtDAQGain317)
        MyAgilent34970A.initializeChannel(57, cbDAQDataEnableChannel318, cbDAQDataChannel318, txtDAQOffset318, txtDAQGain318)
        MyAgilent34970A.initializeChannel(58, cbDAQDataEnableChannel319, cbDAQDataChannel319, txtDAQOffset319, txtDAQGain319)
        MyAgilent34970A.initializeChannel(59, cbDAQDataEnableChannel320, cbDAQDataChannel320, txtDAQOffset320, txtDAQGain320)

        ' Set the properties of both plot (Temperature vs. Time & Current vs. Time)
        DataChart.ChartAreas(0).CursorX.IsUserSelectionEnabled = True
        DataChart.ChartAreas(0).AxisX.ScaleView.Zoomable = True
        DataChart.ChartAreas(0).CursorX.AutoScroll = True
        DataChart.ChartAreas(0).CursorY.IsUserSelectionEnabled = True
        DataChart.ChartAreas(0).AxisY.ScaleView.Zoomable = True
        DataChart.ChartAreas(0).CursorY.AutoScroll = True
        CurrentChart.ChartAreas(0).CursorX.IsUserSelectionEnabled = True
        CurrentChart.ChartAreas(0).AxisX.ScaleView.Zoomable = True
        CurrentChart.ChartAreas(0).CursorX.AutoScroll = True
        CurrentChart.ChartAreas(0).CursorY.IsUserSelectionEnabled = True
        CurrentChart.ChartAreas(0).AxisY.ScaleView.Zoomable = True
        CurrentChart.ChartAreas(0).CursorY.AutoScroll = True

        ' Set the property of the row header
        dgvDAQData.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders ' Set the row header width size mode to auto size

        gDAQStateMachine = eDeviceState.Initial ' Initialize the state
        MyAgilent34970A.myProgram.Mode = 0 ' Set default mode to be Free mode. 0: Free, 1: Duration 2: Number of Readings
        MyAgilent34970A.myProgram.DeviceRunning = False ' Program is initially not running

        ' Add handler that handles the validating event of these textboxes
        AddHandler txtDAQCompareThreshold1.Validating, AddressOf txtDAQCompareThreshold_Validating
        AddHandler txtDAQCompareThreshold2.Validating, AddressOf txtDAQCompareThreshold_Validating
        AddHandler txtDAQCompareThreshold3.Validating, AddressOf txtDAQCompareThreshold_Validating
        AddHandler txtDAQCompareThreshold4.Validating, AddressOf txtDAQCompareThreshold_Validating
        AddHandler txtDAQCompareThreshold5.Validating, AddressOf txtDAQCompareThreshold_Validating

        ' Add handler that handles the validating event of gain and offset textboxes for each channel
        For i = 0 To NUM_ALL_CHANNELS - 1
            AddHandler MyAgilent34970A.channelArray(i).gainTextBox.Validating, AddressOf txtDAQOffsetGain_Validating
            AddHandler MyAgilent34970A.channelArray(i).offsetTextBox.Validating, AddressOf txtDAQOffsetGain_Validating
        Next

        TimerProgram.Start() ' Start the timer for the whole program
    End Sub

    ''' <summary>
    ''' Handles when Duration mode is selected/unselected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbDuration_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbDuration.CheckedChanged
        If rbDuration.Checked Then
            gbCountDown.Visible = True
            gbDAQNumberofReadings.Visible = False
            MyAgilent34970A.myProgram.Mode = 1 ' 0: Free, 1: Duration, 2: Number of Readings
        End If
    End Sub

    ''' <summary>
    ''' Handles when Free mode is selected/unselected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbFree_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbFree.CheckedChanged
        If rbFree.Checked Then
            MyAgilent34970A.myProgram.Mode = 0 ' 0: Free, 1: Duration, 2: Number of Readings
            gbDAQNumberofReadings.Visible = False
            gbCountDown.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Handles when Number of Reading mode is selected/unselected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbNumofReadings_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbNumofReadings.CheckedChanged
        If rbNumofReadings.Checked Then
            gbDAQNumberofReadings.Visible = True
            gbCountDown.Visible = False
            MyAgilent34970A.myProgram.Mode = 2 ' 0: Free, 1: Duration, 2: Number of Readings
        End If
    End Sub

    ''' <summary>
    ''' Reset X-axis of data plot
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPlotResetX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotResetX.Click
        If rbPlotTemperature.Checked Then
            DataChart.ChartAreas(0).AxisX.ScaleView.ZoomReset(0)
        ElseIf rbPlotCurrent.Checked Then
            CurrentChart.ChartAreas(0).AxisX.ScaleView.ZoomReset(0)
        End If
    End Sub

    ''' <summary>
    ''' Reset Y-axis of data plot
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPlotResetY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotResetY.Click
        If rbPlotTemperature.Checked Then
            DataChart.ChartAreas(0).AxisY.ScaleView.ZoomReset(0)
        ElseIf rbPlotCurrent.Checked Then
            CurrentChart.ChartAreas(0).AxisY.ScaleView.ZoomReset(0)
        End If
    End Sub

    ''' <summary>
    ''' Reset both X and Y-axis of data plot
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub btnPlotResetXY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotResetXY.Click
        If rbPlotTemperature.Checked Then
            DataChart.ChartAreas(0).AxisX.ScaleView.ZoomReset(0)
            DataChart.ChartAreas(0).AxisY.ScaleView.ZoomReset(0)
        ElseIf rbPlotCurrent.Checked Then
            CurrentChart.ChartAreas(0).AxisX.ScaleView.ZoomReset(0)
            CurrentChart.ChartAreas(0).AxisY.ScaleView.ZoomReset(0)
        End If
    End Sub

    ''' <summary>
    ''' Handles if the user wants to save the plot
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub btnPlotSavePlot_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotSavePlot.Click
        ' Don't save the plot if the program is still running
        If MyAgilent34970A.myProgram.DeviceRunning Then
            Exit Sub
        End If
        ' Save either plot, depending on user's input
        If rbPlotTemperature.Checked Then
            If Not saveImageFile(DataChart) Then
                MessageBox.Show("Not able to save the plot.", "Save Plot Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        ElseIf rbPlotCurrent.Checked Then
            If Not saveImageFile(CurrentChart) Then
                MessageBox.Show("Not able to save the plot.", "Save Plot Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Show the form that is used to display the email address(es)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDAQEmailView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDAQEmailView.Click
        frmEmailAddressList.Show()
    End Sub

    ''' <summary>
    ''' Add email address
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnDAQEmailAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDAQEmailAdd.Click
        If chbDAQEmailAddress.Text = "" Then
            Exit Sub
        End If
        For i = 0 To frmEmailAddressList.lstEmailTo.Items.Count - 1
            ' If the email address already exists, exit sub to cancel
            If frmEmailAddressList.lstEmailTo.Items(i) = String.Concat(chbDAQEmailAddress.Text, "@landisgyr.com") Then
                Exit Sub
            End If
        Next
        ' Add email address to the list
        frmEmailAddressList.lstEmailTo.Items.Add(String.Concat(chbDAQEmailAddress.Text, "@landisgyr.com"))
        ' Clear user input
        chbDAQEmailAddress.Text = ""
    End Sub

    ''' <summary>
    ''' Handles when main form is closing
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub frmCurrent_Temperature_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        ' Clear all the settings in order to add new
        My.Settings.Channel.Clear()
        My.Settings.Gain.Clear()
        My.Settings.Offset.Clear()
        'My.Settings.enableChannel.Clear()
        My.Settings.checkStatus.Clear()
        ' Save the status of Calibration checkbox
        My.Settings.enableCalibration = chbDAQCalibration.Checked
        For i = 0 To 59
            ' Save the channel name
            My.Settings.Channel.Add(MyAgilent34970A.channelArray(i).nameComboBox.Text)
            ' Save the check status for each channel
            If MyAgilent34970A.channelArray(i).enableCheckBox.Checked Then
                My.Settings.checkStatus.Add("True")
            Else
                My.Settings.checkStatus.Add("False")
            End If
            ' For each channel, save the gain value
            My.Settings.Gain.Add(MyAgilent34970A.channelArray(i).gainTextBox.Text)
            ' For each channel, save the offset value
            My.Settings.Offset.Add(MyAgilent34970A.channelArray(i).offsetTextBox.Text)
        Next
        ' Close the email address list form
        frmEmailAddressList.Close()
        txtDAQCountDownHH.CausesValidation = False
        txtDAQCountDownMM.CausesValidation = False
        txtDAQCountDownSS.CausesValidation = False
        txtDAQNumberofReadings.CausesValidation = False
        e.Cancel = False
    End Sub

    ''' <summary>
    ''' If email notification checkbox is checked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cbDAQEmailNotification_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbDAQEmailNotification.CheckedChanged
        If cbDAQEmailNotification.Checked Then
            ' If user hasn't added any email address, uncheck the checkbox
            If frmEmailAddressList.lstEmailTo.Items.Count = 0 Then
                cbDAQEmailNotification.Checked = False
            Else
                ' Display the options: 1. Notification when test is done. 2. Notifaction when threshold is reached. (Check notification when test is done by default)
                cbDAQEmailTestDone.Checked = cbDAQEmailNotification.Checked
                cbDAQEmailTestDone.Visible = cbDAQEmailNotification.Checked
                cbDAQEmailThreshold.Visible = cbDAQEmailNotification.Checked
            End If
        End If
    End Sub

    ''' <summary>
    ''' Display/hide the threshold panel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cbDAQEmailThreshold_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbDAQEmailThreshold.CheckedChanged
        pnThreshold.Visible = cbDAQEmailThreshold.Checked
    End Sub

    ''' <summary>
    ''' Module 1 Radio Button is checked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbModule1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbSlot100.CheckedChanged
        If rbSlot100.Checked Then
            rbSlot200.Checked = False
            rbSlot300.Checked = False
            panelModule1.Visible = True
            panelModule2.Visible = False
            panelModule3.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Module 2 Radio Button is checked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub rbModule2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbSlot200.CheckedChanged
        If rbSlot200.Checked Then
            rbSlot100.Checked = False
            rbSlot300.Checked = False
            panelModule1.Visible = False
            panelModule2.Visible = True
            panelModule3.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Module 3 Radio Button is checked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub rbModule3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbSlot300.CheckedChanged
        If rbSlot300.Checked Then
            rbSlot100.Checked = False
            rbSlot200.Checked = False
            panelModule1.Visible = False
            panelModule2.Visible = False
            panelModule3.Visible = True
        End If
    End Sub

    ''' <summary>
    ''' Make the chart visible/non visible
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub rbPlotCurrent_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbPlotCurrent.CheckedChanged
        CurrentChart.Visible = True
        DataChart.Visible = False
    End Sub

    ''' <summary>
    ''' Make the chart visible/non visible
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rbPlotTemperature_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbPlotTemperature.CheckedChanged
        CurrentChart.Visible = False
        DataChart.Visible = True
    End Sub

    ''' <summary>
    ''' Save the image file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Public Function saveImageFile(ByVal chart As DataVisualization.Charting.Chart)
        Dim saveFileDialog1 As New SaveFileDialog
        Dim w_a_success As Boolean = False ' Check if the file is ready to be written
        Dim saveResult As DialogResult
        saveFileDialog1.InitialDirectory = "\\am.bm.net\uslafdfs01\GE_Lab\" ' Set initial directory to GE_LAB network drive
        saveFileDialog1.Filter = "png files (*.png)|*.png|All files (*.*)|*.*"
        saveFileDialog1.FilterIndex = 1
        saveFileDialog1.RestoreDirectory = True
        Try
            If saveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                Dim filepath As String = saveFileDialog1.FileName.ToString()
                Dim directoryName As String = Path.GetDirectoryName(filepath)
                gImageFileName = saveFileDialog1.FileName.ToString()
                While w_a_success = False ' If the image is not saved
                    Try
                        chart.SaveImage(gImageFileName, System.Drawing.Imaging.ImageFormat.Png)
                        w_a_success = True ' Set the flag to true when the image is saved successfully
                    Catch ' Cannot overwrite a file that is already opened
                        saveResult = MessageBox.Show("File may be opened. Check file status, close and try again.", "File Opened", MessageBoxButtons.RetryCancel)
                        If saveResult = DialogResult.Retry Then ' If the user presses "retry", the program will check again
                            Continue While
                        ElseIf saveResult = DialogResult.Cancel Then
                            Exit While
                        End If
                    End Try
                End While
                Return w_a_success
            End If
        Catch
            MessageBox.Show("Not able to save the image file", "Failed to Save Plot", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
        Return False
    End Function

    ''' <summary>
    ''' Make a plot for the data loaded
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub btnPlotLoadPlot_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotLoadPlot.Click
        Try
            ' Load corresponding plot based on the user input
            If rbPlotTemperature.Checked Then
                If loadPlot(DataChart) Then
                    Throw New Exception ' If format error catched, pop up a warning message
                End If
            ElseIf rbPlotCurrent.Checked Then
                If loadPlot(CurrentChart) Then
                    Throw New Exception ' If format error catched, pop up a warning message
                End If
            End If
        Catch
            ' If the program catches any error, it means the format is not either old version or modified by someone
            MessageBox.Show("Not able to open the file. The file possibly does not have the same format as the one generated by the recently updated program.", "Open File Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            If rbPlotTemperature.Checked Then
                DataChart.Series.Clear()
            ElseIf rbPlotCurrent.Checked Then
                CurrentChart.Series.Clear()
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Set line width to 1, 2 or 3
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub btnPlotSetLineWidth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPlotSetLineWidth.Click
        setLineWidth(DataChart, CInt(cbPlotLineWidth.Text))
    End Sub

    ''' <summary>
    ''' This function is used to load the plot
    ''' </summary>
    ''' <param name="chart"></param>
    ''' <returns>True: The format of the csv file is wrong. False: The file is opened correctly or program chooses not to open the file</returns>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Function loadPlot(ByVal chart As DataVisualization.Charting.Chart) As Boolean
        ' Don't load the plot if the program is still running
        If MyAgilent34970A.myProgram.DeviceRunning Then
            Return False
        End If
        Dim comfirmResult As DialogResult
        If chart.Series.Count <> 0 Then ' If the plot is not empty, ask user if user wants to overwrite the current plot
            comfirmResult = MessageBox.Show("There is already a plot existed. Are you sure to load a new one to overwrite the current one?", "Plot Series Already Exists", MessageBoxButtons.YesNo)
            If comfirmResult <> Windows.Forms.DialogResult.No Then ' Clear the plot and load the plot if user chooses yes
                chart.Series.Clear()
            Else ' Or just exit the sub routine
                Return 0
            End If
        End If
        Dim openFileDialog As New OpenFileDialog
        Dim FileNum As Integer = FreeFile()
        Dim openResult As DialogResult
        Dim open_success As Boolean = False ' indicates if the file is loaded successfully
        Dim tempString As String()
        Dim value As Double ' temporarily store the reading value
        openFileDialog.InitialDirectory = "C:\"
        openFileDialog.Filter = "csv files (*.csv)|*.csv|All Files (*.*)|*.*"
        openFileDialog.FilterIndex = 1
        openFileDialog.RestoreDirectory = True
        If openFileDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Dim filepath As String = openFileDialog.FileName.ToString() ' store the file path
            Dim directoryName As String = Path.GetDirectoryName(filepath)
            While Not open_success ' If the file is not opened successfully
                Try
                    FileOpen(FileNum, filepath, OpenMode.Input)
                    open_success = True ' Set the flag to true if the file is opened successfully
                Catch ex As Exception ' Usually because the file is opened and cannot be read
                    openResult = MessageBox.Show("File may be opened. Check file status, close and try again.", "File Opened", MessageBoxButtons.RetryCancel)
                    If openResult = DialogResult.Retry Then
                        Continue While
                    ElseIf openResult = Windows.Forms.DialogResult.Cancel Then
                        Exit While
                    End If
                End Try
            End While
            ' Skip the first 9 lines, which just contain the test information
            For x = 0 To 8
                LineInput(FileNum)
            Next
            Try
                Dim tempTitle As String() = LineInput(FileNum).Split(",") ' This line contains the title: datetime, current(amps), channel101 reading...
                chart.ChartAreas(0).AxisX.Title = tempTitle(0).TrimEnd("""").TrimStart("""") ' Set X axis title, which is the date time
                chart.ChartAreas(0).AxisX.TitleFont = New Font("Microsoft Sans Serif", 12, FontStyle.Bold) ' Set font
                chart.ChartAreas(0).AxisY.TitleFont = New Font("Microsoft Sans Serif", 12, FontStyle.Bold) ' Set font
                If rbPlotTemperature.Checked Then
                    chart.ChartAreas(0).AxisY.Title = "Temperature / °C" ' Set y-axis
                    For cnt = 0 To tempTitle.Count - 3
                        chart.Series.Add(tempTitle(cnt + 2).TrimEnd("""").TrimStart(""""))
                        chart.Series(cnt).ChartType = DataVisualization.Charting.SeriesChartType.Spline
                    Next
                    Do Until EOF(FileNum)
                        tempString = LineInput(FileNum).Split(",")
                        For numSeries = 0 To tempString.Length - 3
                            value = CDbl(tempString(numSeries + 2).TrimEnd("""").TrimStart(""""))
                            chart.Series(numSeries).Points.AddXY(tempString(0).TrimEnd("""").TrimStart(""""), value)
                            chart.Series(numSeries).MarkerStyle = DataVisualization.Charting.MarkerStyle.Circle
                        Next
                    Loop
                ElseIf rbPlotCurrent.Checked Then
                    chart.ChartAreas(0).AxisY.Title = "Current / A"
                    chart.Series.Add(tempTitle(1).TrimEnd("""").TrimStart(""""))
                    chart.Series(0).ChartType = DataVisualization.Charting.SeriesChartType.Spline
                    chart.Series(0).MarkerStyle = DataVisualization.Charting.MarkerStyle.Circle
                    Do Until EOF(FileNum)
                        tempString = LineInput(FileNum).Split(",")
                        value = CDbl(tempString(1).TrimEnd("""").TrimStart(""""))
                        chart.Series(0).Points.AddXY(tempString(0).TrimEnd("""").TrimStart(""""), value)
                    Loop
                End If
                FileClose(FileNum)
                lblPlotReminder.Text = "File at """ + filepath + """ is loaded."
            Catch ' Format error catched
                Return True
            End Try
        Else ' User cancels openning file
            Return False
        End If
        Return False ' Do nothing
    End Function


    '####################################################################################################################################################################################################
#Region "GPIB CODE"
    Private Sub SetupControlState(ByVal isSessionOpen As Boolean)
        boardIdNumericUpDown.Enabled = Not isSessionOpen
        primaryAddressNumericUpDown.Enabled = Not isSessionOpen
        secondaryAddressComboBox.Enabled = Not isSessionOpen
        openButton.Enabled = Not isSessionOpen
        closeButton.Enabled = isSessionOpen
    End Sub 'SetupControlState

    Private Sub openButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles openButton.Click
        Try
            Windows.Forms.Cursor.Current = Cursors.WaitCursor
            Dim currentSecondaryAddress As Integer
            If secondaryAddressComboBox.SelectedIndex <> 0 Then
                currentSecondaryAddress = secondaryAddressComboBox.SelectedItem
            Else
                currentSecondaryAddress = 0
            End If

            GpibDevice = New Device(CInt(boardIdNumericUpDown.Value), CByte(primaryAddressNumericUpDown.Value), CByte(currentSecondaryAddress))

            v_UpdateGPIBLog("GPIB Time Out = " + GpibDevice.IOTimeout.ToString, gGPIBTextWidth)
            SetupControlState(True)
            CI501TCA_VoltageOff()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            'once connected disable switching between the voltage sources...
            rbUsePacificPower.Enabled = False
            rbUseCaliforniaInstruments.Enabled = False
            Windows.Forms.Cursor.Current = Cursors.Default
        End Try
    End Sub 'openButton_Click

    Private Sub closeButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles closeButton.Click
        Try
            GpibDevice.Dispose()
            SetupControlState(False)
            'once disconnected Enable switching between the voltage sources...
            rbUsePacificPower.Enabled = True
            rbUseCaliforniaInstruments.Enabled = True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub 'closeButton_Click

    Private Sub writeButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub 'writeButton_Click

    Private Sub readButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub 'readButton_Click

    Private Function ReplaceCommonEscapeSequences(ByVal s As String) As String
        Return s.Replace("\n", ControlChars.Lf).Replace("\r", ControlChars.Cr)
    End Function 'ReplaceCommonEscapeSequences

    Private Function InsertCommonEscapeSequences(ByVal s As String) As String
        Return s.Replace(ControlChars.Lf, "\n").Replace(ControlChars.Cr, "\r")
    End Function 'InsertCommonEscapeSequences

    Private Sub chkGPIBVerbose_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkGPIBVerbose.CheckedChanged
        Try
            If chkGPIBVerbose.Checked = True Then
                MyCICA501TAC._Verbose = True
            Else
                MyCICA501TAC._Verbose = False
            End If
        Catch ex As Exception
            v_UpdateGPIBLog("Verbose Check Changed Error: " + ex.Message, gGPIBTextWidth)
        End Try
    End Sub

    Private Sub btnGPIB_LogClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGPIB_LogClear.Click
        lbGPIB_Log.Items.Clear()
    End Sub


#End Region '"GPIB CODE"
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        'This is for testing only GUI states should be managed in state machine - FJB

        Dim bSuccess As Boolean = True
        Dim iConnectionRetries As Integer = 0
        Dim iMaxRetries As Integer = 3
        While iConnectionRetries < iMaxRetries
            If btnConnect.Text = "Connect" Then
                Try
                    MyRadian.v_InitSerialPort(CInt(txtMeterComBaudrate.Text), IO.Ports.Parity.None, IO.Ports.StopBits.One, 8, "COM" + cbMeter_COMPorts.Text, True)
                    v_UpdateLog("Initilization Of COM : " + cbMeter_COMPorts.Text.ToString + " complete.")
                Catch ex As Exception
                    bSuccess = False
                    gbWriteToLog = True
                    v_UpdateLog("Error Initializing Com Port" + ex.ToString)
                    gbWriteToLog = False
                    iConnectionRetries += 1
                End Try

                If bSuccess Then
                    Try
                        MyRadian.v_Connect()
                        btnConnect.Text = "Disconnect"

                    Catch ex As Exception
                        bSuccess = False
                        gbWriteToLog = True
                        v_UpdateLog("Error Connecting to Standard" + ex.ToString)
                        gbWriteToLog = False
                        iConnectionRetries += 1
                    End Try

                End If

                'Try sending NOP to Radian
                If bSuccess Then

                    'convert string to byte array
                    b_Hex_String_To_ByteArray(Radian.NOP, MyRadian.aBytes)
                    'Calculate 16 bit checksum and Append to array
                    b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
                    'temp string to hold recieved data
                    Dim strTemp2 As String = ""
                    'transmit message to radian
                    MyRadian.v_Transmit(MyRadian.aBytes)
                    Try
                        'recieve data 
                        MyRadian.b_Recieve(MyRadian.aBytes)
                        'Make Byte Readable as a string
                        strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
                        TextBox1.Text = strTemp2
                        'parse packet
                        MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
                        'validate  Checksum
                        If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                            v_UpdateLog("Checksum Does Not Match")
                            Throw New Exception("Checksum does not match!")
                        End If

                        If (MyRadian.Packet.Start And &HF) = CByte(&H3) Then 'Mask Off upper nibble
                            'success
                            v_UpdateLog("RD Standard Detected")
                            MyRadian.IsConnected = True
                            btnIdentify.PerformClick()
                            Exit While
                        ElseIf (MyRadian.Packet.Start And &HF) = CByte(&H9) Then
                            v_UpdateLog("RD Standard Detected, With Error")
                            iConnectionRetries = iMaxRetries
                        Else
                            v_UpdateLog("Unexpected reply; Disconnecting!")
                            Throw New Exception("Unexpected reply; Disconnecting!")
                        End If


                    Catch ex As Exception
                        MyRadian.IsConnected = False
                        gbWriteToLog = True
                        v_UpdateLog(ex.ToString)
                        gbWriteToLog = False
                        btnConnect.Text = "Connect"
                        MyRadian.v_Disconnect()
                        iConnectionRetries += 1
                    End Try
                Else

                    iConnectionRetries += 1

                End If

            Else
                Try
                    MyRadian.v_Disconnect()
                    btnConnect.Text = "Connect"
                    v_UpdateLog("Disconnected From Device")
                    iConnectionRetries = iMaxRetries
                Catch ex As Exception
                    bSuccess = False
                    gbWriteToLog = True
                    v_UpdateLog(ex.ToString)
                    gbWriteToLog = False
                End Try
            End If
        End While

        If iConnectionRetries >= iMaxRetries And Not bSuccess Then
            MsgBox("The maximum number (" + iMaxRetries.ToString + ") of connection attempts has been exceeded.  Verify your connection and comport and try again.")
        End If
    End Sub

#Region "Functions Safely Available to all Threads"
    Public Delegate Sub UpDateLogDelegate(ByVal message As String)
    ''' <summary>
    '''  Thread safe UpdateLog sets Logdata's items.add property
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    ''' 
    Public Sub v_UpdateLog(ByVal message As String, Optional ByVal VerboseLevel As Integer = 0)
        ' if modifying Output data is not thread safe
        If lblLogData.InvokeRequired Then
            ' use inherited method Invoke to execute DisplayMessage
            ' via a Delegate
            Try
                Invoke(New UpDateLogDelegate(AddressOf v_UpdateLog), New Object() {message})

            Catch ex As Exception

            End Try

            ' OK to modify output data in current thread
            ' write everything in verbose mode else only if flagged   
        ElseIf gbVerbose = True Or gbWriteToLog = True Or VerboseLevel >= gVerboseLevel Then

            Try
                lblLogData.Items.Add(message)
            Catch ex As Exception
                lblLogData.Items.Clear()
            End Try

        End If
    End Sub ' v_UpdateLog()

    Public Delegate Sub UpDateGPIBLogDelegate(ByVal message As String, ByVal MaxTextLength As Integer)
    ''' <summary>
    '''  Thread safe UpdateLog sets Logdata's items.add property
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    ''' 
    Public Sub v_UpdateGPIBLog(ByVal message As String, Optional ByVal MaxTextLength As Integer = 0)
        ' if modifying Output data is not thread safe
        If lbGPIB_Log.InvokeRequired Then
            ' use inherited method Invoke to execute DisplayMessage
            ' via a Delegate
            Try
                Invoke(New UpDateGPIBLogDelegate(AddressOf v_UpdateGPIBLog), New Object() {message}, New Object() {MaxTextLength})

            Catch ex As Exception

            End Try

            ' OK to modify output data in current thread

        Else

            Try
                'Dim i = MaxTextLength - 1

                ' Dim j As Integer = 0
                'If message.Length > MaxTextLength Then
                '    Do While j = 0
                '        If message.Substring(i, 1) = " " Then
                '            j = -1
                '        Else
                '            i = i - 1
                '            If i = -1 Then
                '                i = MaxTextLength - 1
                '                j = 0
                '            End If
                '        End If
                '    Loop
                '    message = message.Remove(i, 1).Insert(i, "@")
                '    i = i + MaxTextLength
                '    Do While i < message.Length - 1

                '        j = 0

                '        Do While j = 0
                '            If message.Substring(i, 1) = " " Then
                '                message = message.Remove(i, 1).Insert(i, "@")
                '                i = i + MaxTextLength
                '                j = -1
                '            ElseIf i > 0 Then
                '                i = i - 1
                '                If message.Substring(i, 1) = "@" Then 'wrap without space
                '                    i = i + MaxTextLength 'advance to original index
                '                    message = message.Insert(i, "@") 'insert parse character
                '                    i = i + MaxTextLength 'advance to next index
                '                    j = -1
                '                End If
                '            Else
                '                lbGPIB_Log.Items.Add("Update Log Parse Error")
                '                Exit Sub
                '            End If


                '        Loop
                '    Loop
                '    Dim TempString() As String = message.Split("@")
                '    For i = 0 To TempString.Count - 1
                '        lbGPIB_Log.Items.Add(TempString(i))
                '    Next
                'Else
                '    lbGPIB_Log.Items.Add(message)
                'End If

                lbGPIB_Log.Items.Add(message)
            Catch ex As Exception
                lbGPIB_Log.Items.Add("Error while updating Log: " + ex.ToString)
            End Try

        End If
    End Sub ' v_UpdateGPIBLog()



#End Region '"Functions Safely Available to all Threads"

    Private Sub btnIdentify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIdentify.Click
        Try
            ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4 = NO Data Returned (A3)
            Dim iParsePacketReturn As Integer
            'convert string to byte array
            b_Hex_String_To_ByteArray(Radian.IDENTIFY, MyRadian.aBytes)
            'Calculate 16 bit checksum and Append to array
            b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
            'Store Subcommand in Packet
            MyRadian.Packet.SubCommand = -1
            'temp string to hold recieved data
            Dim strTemp2 As String = ""
            'transmit message to radian
            MyRadian.v_Transmit(MyRadian.aBytes)
            'recieve data 
            MyRadian.b_Recieve(MyRadian.aBytes)
            'Make Byte Readable as a string
            strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
            TextBox1.Text = strTemp2
            'parse packet
            iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
            'validate  Checksum
            If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                Throw New Exception("Checksum does not match!")
            End If
            If iParsePacketReturn = 1 Then
                'Parse data from packet
                MyRadian.b_Parse_Radian_Data(MyRadian.Packet)
                lbIdentification.Items.Clear()
                lbIdentification.Items.Add("Model Number:    " + MyRadian.Identification.ModelNumber)
                lbIdentification.Items.Add("Serial Number:   " + MyRadian.Identification.SerialNumber)
                lbIdentification.Items.Add("NAME:            " + MyRadian.Identification.Name)
                lbIdentification.Items.Add("SW Version:      " + MyRadian.Identification.VersionNumber)

            ElseIf iParsePacketReturn = 2 Then
                'Radian Responded with a delay do not send new data unitl delay time expires
            ElseIf iParsePacketReturn = 3 Then
                'Radian Error
                Throw New Exception("Radian Error Message Returned:")

            ElseIf iParsePacketReturn = 4 Then
                Throw New Exception("Radian returned no Data")
            ElseIf iParsePacketReturn = 0 Then
                Throw New Exception("Unknown Error Returned from Parser")
            Else
                Throw New Exception("Unknown Response from Packet Parser!")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click
        Try
            Dim bResetWaveFormBuffers As Boolean
            Dim bResetInstantData As Boolean
            Dim bResetInstantMinData As Boolean
            Dim bResetInstantMaxdata As Boolean
            Dim bResetAccumData As Boolean
            Dim strRadianResetmessage As String

            'Set Flags based on selection of checkboxes
            bResetWaveFormBuffers = chkResetWaveformBufferToZero.Checked
            bResetInstantData = chkResetInstantaneousData.Checked
            bResetInstantMinData = chkResetMinData.Checked
            bResetInstantMaxdata = chkResetMaxData.Checked
            bResetAccumData = chkResetAccumData.Checked

            'Build reset message
            strRadianResetmessage = MyRadian.str_BuildRDMetricResetMessage(bResetWaveFormBuffers, bResetInstantData, bResetInstantMinData, bResetInstantMaxdata, bResetAccumData)

            ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4  = NO Data Returned (A3)
            Dim iParsePacketReturn As Integer

            'convert string to byte array
            b_Hex_String_To_ByteArray(strRadianResetmessage, MyRadian.aBytes)
            'Calculate 16 bit checksum and Append to array
            b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
            'transmit message to radian
            MyRadian.v_Transmit(MyRadian.aBytes)
            'Store Subcommand in Packet
            MyRadian.Packet.SubCommand = -1

            'temp string to hold recieved data
            Dim strTemp2 As String = ""


            'recieve data 
            MyRadian.b_Recieve(MyRadian.aBytes)
            'Make Byte Readable as a string
            strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
            TextBox1.Text = strTemp2
            'parse packet
            iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
            'validate  Checksum
            If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                Throw New Exception("Checksum does not match!")
            End If

            If iParsePacketReturn = 1 Then
                'Parse data from packet
                Throw New Exception("No Data expected on return!")
                MyRadian.b_Parse_Radian_Data(MyRadian.Packet)



            ElseIf iParsePacketReturn = 2 Then
                'Radian Responded with a delay do not send new data unitl delay time expires
            ElseIf iParsePacketReturn = 3 Then
                'Radian Error
                Throw New Exception("Radian Error Message Returned:")

            ElseIf iParsePacketReturn = 4 Then
                If MyRadian.Packet.PacketType = Radian.ePacketTypes.RESET_METRICS Then
                    'MsgBox("Reset Command accepted")
                    'Success!
                Else
                    Throw New Exception("Unexpected reply from Radian Device: " + MyRadian.Packet.PacketType.ToString)
                End If

            ElseIf iParsePacketReturn = 0 Then
                Throw New Exception("Unknown Error Returned from Parser")
            Else
                Throw New Exception("Unknown Response from Packet Parser!")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub btnRangingGetCurrentRange_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRangingGetCurrentRange.Click

        Try
            ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4  = No Data Returned (A3)
            Dim iParsePacketReturn As Integer
            'If Time = 0 then query
            Dim strTempString = MyRadian.str_BuildLockRangeMessage(CByte(Radian.eRelayControlCommand.VOLTAGE_STATUS), CByte(Radian.eRelayControlCommand.CURRENT_STATUS), CByte(Radian.eRelayControlCommand.CURRENT_STATUS), _
                                                                    CByte(Radian.eVoltageTapAddressesRD.ALL_OPEN), CByte(Radian.eCurrentTapsAddressRD.ALL_OPEN))
            'convert string to byte array
            b_Hex_String_To_ByteArray(strTempString, MyRadian.aBytes)
            'Calculate 16 bit checksum and Append to array
            b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
            'transmit message to radian
            MyRadian.v_Transmit(MyRadian.aBytes)
            'Store Subcommand in Packet
            MyRadian.Packet.SubCommand = -1 'There is no sub command
            MyRadian.Packet.ControlByte.A = 0 ' There is no control Byte default to 0
            'temp string to hold recieved data
            Dim strTemp2 As String = ""


            'recieve data 
            MyRadian.b_Recieve(MyRadian.aBytes)
            'Make Byte Readable as a string
            strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
            TextBox1.Text = strTemp2
            'parse packet
            iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
            'validate  Checksum
            If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                Throw New Exception("Checksum does not match!")
            End If

            If iParsePacketReturn = 1 Then
                'Parse data from packet
                MyRadian.b_Parse_Radian_Data(MyRadian.Packet)
                'lblRangingVoltageRangeSetPoint.Text = "Range = " + MyRadian.RelayStatus.VoltageRangeName + "; Locked = " + MyRadian.RelayStatus.VoltageRelayLocked.ToString
                lblRangingCurrentRangeSetPoint.Text = "Range = " + MyRadian.RelayStatus.CurrentRangeName + "; Locked = " + MyRadian.RelayStatus.CurrentRelayLocked.ToString
            ElseIf iParsePacketReturn = 2 Then
                'Radian Responded with a delay do not send new data unitl delay time expires
            ElseIf iParsePacketReturn = 3 Then
                'Radian Error
                Throw New Exception("Radian Error Message Returned: ")
            ElseIf iParsePacketReturn = 4 Then
                'Ack Recieved
                If gbVerbose Then
                    MsgBox("Ack Received")
                    v_UpdateLog("Get Range: Ack Recieved")
                End If

            ElseIf iParsePacketReturn = 0 Then
                Throw New Exception("Unknown Error Returned from Parser ")
            Else
                Throw New Exception("Unknown Response from Packet Parser! ")
            End If
        Catch ex As Exception
            If gbVerbose Then
                'MsgBox(ex.ToString)
                v_UpdateLog("Get Range Error: " + ex.ToString)
            End If

        End Try
    End Sub

    Private Sub btnRangingLockCurrentRangeToggle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRangingLockCurrentRangeToggle.Click
        Try
            ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4  = No Data Returned (A3)
            Dim iParsePacketReturn As Integer
            Dim strTempString As String = ""
            'If Time = 0 then query
            If chkRangingSetAndLockCurrent.Checked = False Then 'just lock range
                strTempString = MyRadian.str_BuildLockRangeMessage(Radian.eRelayControlCommand.VOLTAGE_STATUS, Radian.eRelayControlCommand.LOCK_CURRENT, Radian.eRelayControlCommand.CURRENT_STATUS, _
                                                                   Radian.eVoltageTapAddressesRD.ALL_OPEN, Radian.eCurrentTapsAddressRD.ALL_OPEN)
            ElseIf chkRangingSetAndLockCurrent.Checked = True Then
                Dim tempCurrentTapAddress As Radian.eVoltageTapAddressesRD
                tempCurrentTapAddress = MyRadian.aCurrentRelay.Rangeaddress(cbRangingCurrent.SelectedIndex)
                strTempString = MyRadian.str_BuildLockRangeMessage(Radian.eRelayControlCommand.VOLTAGE_STATUS, Radian.eRelayControlCommand.SET_LOCK_CURRENT, Radian.eRelayControlCommand.CURRENT_STATUS, _
                                                                  Radian.eVoltageTapAddressesRD.ALL_OPEN, tempCurrentTapAddress)
            End If

            'convert string to byte array

            b_Hex_String_To_ByteArray(strTempString, MyRadian.aBytes)
            'Calculate 16 bit checksum and Append to array
            b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
            'transmit message to radian
            MyRadian.v_Transmit(MyRadian.aBytes)
            'Store Subcommand in Packet
            MyRadian.Packet.SubCommand = -1 'There is no sub command
            MyRadian.Packet.ControlByte.A = 0 ' There is no control Byte default to 0
            'temp string to hold recieved data
            Dim strTemp2 As String = ""


            'recieve data 
            MyRadian.b_Recieve(MyRadian.aBytes)
            'Make Byte Readable as a string
            strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
            TextBox1.Text = strTemp2
            'parse packet
            iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
            'validate  Checksum
            If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                Throw New Exception("Checksum does not match!")
            End If

            If iParsePacketReturn = 1 Then
                'Parse data from packet
                MyRadian.b_Parse_Radian_Data(MyRadian.Packet)
                'lblRangingVoltageRangeSetPoint.Text = "Range = " + MyRadian.RelayStatus.VoltageRangeName + "; Locked = " + MyRadian.RelayStatus.VoltageRelayLocked.ToString
                lblRangingCurrentRangeSetPoint.Text = "Range = " + MyRadian.RelayStatus.CurrentRangeName + "; Locked = " + MyRadian.RelayStatus.CurrentRelayLocked.ToString
            ElseIf iParsePacketReturn = 2 Then
                'Radian Responded with a delay do not send new data unitl delay time expires
            ElseIf iParsePacketReturn = 3 Then
                'Radian Error
                Throw New Exception("Radian Error Message Returned:")
            ElseIf iParsePacketReturn = 4 Then
                'Ack Recieved
                MsgBox("Ack Received")
            ElseIf iParsePacketReturn = 0 Then
                Throw New Exception("Unknown Error Returned from Parser")
            Else
                Throw New Exception("Unknown Response from Packet Parser!")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub btnRangingUnlockCurrent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRangingUnlockCurrent.Click
        Try
            ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4  = No Data Returned (A3)
            Dim iParsePacketReturn As Integer
            Dim strTempString As String = ""


            strTempString = MyRadian.str_BuildLockRangeMessage(Radian.eRelayControlCommand.VOLTAGE_STATUS, Radian.eRelayControlCommand.UNLOCK_CURRENT, Radian.eRelayControlCommand.CURRENT_STATUS, _
                                                               Radian.eVoltageTapAddressesRD.ALL_OPEN, Radian.eCurrentTapsAddressRD.ALL_OPEN)


            'convert string to byte array

            b_Hex_String_To_ByteArray(strTempString, MyRadian.aBytes)
            'Calculate 16 bit checksum and Append to array
            b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
            'transmit message to radian
            MyRadian.v_Transmit(MyRadian.aBytes)
            'Store Subcommand in Packet
            MyRadian.Packet.SubCommand = -1 'There is no sub command
            MyRadian.Packet.ControlByte.A = 0 ' There is no control Byte default to 0
            'temp string to hold recieved data
            Dim strTemp2 As String = ""


            'recieve data 
            MyRadian.b_Recieve(MyRadian.aBytes)
            'Make Byte Readable as a string
            strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
            TextBox1.Text = strTemp2
            'parse packet
            iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
            'validate  Checksum
            If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                Throw New Exception("Checksum does not match!")
            End If

            If iParsePacketReturn = 1 Then
                'Parse data from packet
                MyRadian.b_Parse_Radian_Data(MyRadian.Packet)
                'lblRangingVoltageRangeSetPoint.Text = "Range = " + MyRadian.RelayStatus.VoltageRangeName + "; Locked = " + MyRadian.RelayStatus.VoltageRelayLocked.ToString
                lblRangingCurrentRangeSetPoint.Text = "Range = " + MyRadian.RelayStatus.CurrentRangeName + "; Locked = " + MyRadian.RelayStatus.CurrentRelayLocked.ToString
            ElseIf iParsePacketReturn = 2 Then
                'Radian Responded with a delay do not send new data unitl delay time expires
            ElseIf iParsePacketReturn = 3 Then
                'Radian Error
                Throw New Exception("Radian Error Message Returned:")
            ElseIf iParsePacketReturn = 4 Then
                'Ack Recieved
                MsgBox("Ack Received")
            ElseIf iParsePacketReturn = 0 Then
                Throw New Exception("Unknown Error Returned from Parser")
            Else
                Throw New Exception("Unknown Response from Packet Parser!")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub btnGetInstantMetrics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetInstantMetrics.Click
        v_UpdateInstantMetrics(True)
    End Sub

    Public Sub v_UpdateInstantMetrics(ByVal connected As Boolean)
        If connected Then
            Try
                ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4 =  = NO Data Returned (A3)
                Dim iParsePacketReturn As Integer

                'convert string to byte array
                b_Hex_String_To_ByteArray(MyRadian.aInstantMetricsReadCommandsListRD2X(cbSelectInstantMetrics.SelectedIndex), MyRadian.aBytes)
                'Calculate 16 bit checksum and Append to array
                b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
                'Store Subcommand in Packet
                MyRadian.Packet.SubCommand = cbSelectInstantMetrics.SelectedIndex
                'temp string to hold recieved data
                Dim strTemp2 As String = ""

                'transmit message to radian
                MyRadian.v_Transmit(MyRadian.aBytes)
                'recieve data 
                'ReDim MyRadian.aBytes(4095)
                MyRadian.b_Recieve(MyRadian.aBytes)
                'Make Byte Readable as a string
                strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
                TextBox1.Text = strTemp2
                'parse packet
                iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
                'validate  Checksum
                If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                    Throw New Exception("Checksum does not match!")
                End If
                If iParsePacketReturn = 1 Then
                    'Parse data from packet
                    MyRadian.b_Parse_Radian_Data(MyRadian.Packet)
                    lbDisplayInstantMetrics.Items.Clear()
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.Volt.ToString + " Volts")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.Amp.ToString + " Amps")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.Watt.ToString + " Watts")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.VA.ToString + " VA")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.VAR.ToString + " VARs")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.Frequency.ToString + " HZ")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.Phase.ToString + " Degrees")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.PowerFactor.ToString + " PF")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.AnalogSense.ToString + " Amps")
                    lbDisplayInstantMetrics.Items.Add(MyRadian.MetricData.DeltaPhase.ToString + " Degrees")

                ElseIf iParsePacketReturn = 2 Then
                    'Radian Responded with a delay do not send new data unitl delay time expires
                ElseIf iParsePacketReturn = 3 Then
                    'Radian Error
                    Throw New Exception("Radian Error Message Returned:")

                ElseIf iParsePacketReturn = 4 Then
                    'nop
                ElseIf iParsePacketReturn = 0 Then
                    Throw New Exception("Unknown Error Returned from Parser")
                Else
                    Throw New Exception("Unknown Response from Packet Parser!")
                End If
            Catch ex As Exception
                v_UpdateLog(ex.ToString)
            End Try
        Else
            lbDisplayInstantMetrics.Items.Clear()
        End If
    End Sub

    Private Sub btnGetAccumMetrics_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetAccumMetrics.Click
        v_UpdateAccumulatedMeterics()
    End Sub
    Public Sub v_UpdateAccumulatedMeterics()
        Try
            ' Store Return Status; VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4 =  = NO Data Returned (A3)
            Dim iParsePacketReturn As Integer

            'convert string to byte array
            b_Hex_String_To_ByteArray(MyRadian.aAccumMetricsReadCommandList(cbSelectAccumMetrics.SelectedIndex), MyRadian.aBytes)
            'Calculate 16 bit checksum and Append to array
            'This could be pre calcluated to speed this up
            b_Append_CheckSum16_to_ByteArray(MyRadian.aBytes)
            'Store Subcommand in Packet
            MyRadian.Packet.SubCommand = cbSelectAccumMetrics.SelectedIndex
            'temp string to hold recieved data
            Dim strTemp2 As String = ""

            'transmit message to radian
            MyRadian.v_Transmit(MyRadian.aBytes)
            'recieve data 
            MyRadian.b_Recieve(MyRadian.aBytes)
            'Make Byte Readable as a string
            strTemp2 = str_ByteArray_To_Readable_String(MyRadian.aBytes)
            TextBox1.Text = strTemp2
            'parse packet
            iParsePacketReturn = MyRadian.i_Parse_Radian_Packet(MyRadian.aBytes, MyRadian.Packet)
            'validate  Checksum
            If b_Validate_CheckSum16(MyRadian.aBytes, MyRadian.Packet.Checksum, True) = False Then
                Throw New Exception("Checksum does not match!")
            End If
            If iParsePacketReturn = 1 Then
                'Parse data from packet
                MyRadian.b_Parse_Radian_Data(MyRadian.Packet)
                lbDisplayAccumMetrics.Items.Clear()
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.WattHr.ToString + " Wh")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.VARHr.ToString + " VARh")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.QHr.ToString + " Qh")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.VAHr.ToString + " VAh")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.VoltHr.ToString + " Vh")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.AmpHr.ToString + " Ah")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.V2Hr.ToString + " V^2h")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.A2Hr.ToString + " A^2h")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.WattHrPlus.ToString + " Wh+")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.WattHrMinus.ToString + " Wh-")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.VarHrPlus.ToString + " VARh+")
                lbDisplayAccumMetrics.Items.Add(MyRadian.MetricData.VarHrMinus.ToString + " VARh-")

            ElseIf iParsePacketReturn = 2 Then
                'Radian Responded with a delay do not send new data unitl delay time expires
            ElseIf iParsePacketReturn = 3 Then
                'Radian Error
                Throw New Exception("Radian Error Message Returned:")

            ElseIf iParsePacketReturn = 4 Then
                'nop
            ElseIf iParsePacketReturn = 0 Then
                Throw New Exception("Unknown Error Returned from Parser")
            Else
                Throw New Exception("Unknown Response from Packet Parser!")
            End If
        Catch ex As Exception
            v_UpdateLog(ex.ToString)
        End Try
    End Sub

    Private Sub TimerRadian_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerRadian.Tick
        Dim strLog As String
        Select Case gStateMachineState
            Case eState.Idle
                TimerSample.Stop()
                If MyRadian.IsConnected Then
                    TimerRadian.Stop()
                    v_UpdateInstantMetrics(MyRadian.IsConnected)
                    TimerRadian.Start()
                Else
                    v_UpdateInstantMetrics(MyRadian.IsConnected)
                End If
                'Do last log update and enable controls
                If gLastState = eState.ReadingMetics Then
                    strLog = "Log Terminated at: " + vbTab + Now.ToString
                    lbDataLog.Items.Add(strLog)
                    'change control states
                    chkDataLoggerVolts.Enabled = True
                    chkDataLoggerCurrent.Enabled = True
                    chkDataLoggerFrequency.Enabled = True
                    chkDataLoggerPhase.Enabled = True
                    txtDataLoggerInterval.Enabled = True
                    btnDataLoggerToggleLogger.Text = "Start Logger"
                    'change last state
                    gLastState = eState.Idle
                End If
            Case eState.ReadingMetics
                'First time through clear previous results and add labels
                If gLastState = eState.Idle Then
                    lbDataLog.Items.Clear()
                    strLog = "" 'clear string
                    strLog = "Sample"
                    If chkDataLoggerVolts.Checked Then
                        strLog = strLog + vbTab + "Vrms"
                    End If
                    If chkDataLoggerCurrent.Checked Then
                        strLog = strLog + vbTab + vbTab + "Arms"
                    End If
                    If chkDataLoggerFrequency.Checked Then
                        strLog = strLog + vbTab + vbTab + "Hz"
                    End If
                    If chkDataLoggerPhase.Checked Then
                        strLog = strLog + vbTab + vbTab + "Deg"
                    End If
                    strLog = strLog + vbTab + vbTab + "Time Stamp"
                    lbDataLog.Items.Add(strLog)
                    txtDataLoggerHeader.Text = strLog
                    My.Settings.strDataLoggerHeader = strLog
                    'change control states
                    chkDataLoggerVolts.Enabled = False
                    chkDataLoggerCurrent.Enabled = False
                    chkDataLoggerFrequency.Enabled = False
                    chkDataLoggerPhase.Enabled = False
                    txtDataLoggerInterval.Enabled = False
                    btnDataLoggerToggleLogger.Text = "Stop Logger"
                    'change last state
                    gLastState = eState.ReadingMetics
                    TimerSample.Interval = 100 'Val(txtDataLoggerInterval.Text) * 1000
                    TimerSample.Start()
                End If
                If MyRadian.IsConnected Then
                    TimerRadian.Stop()
                    v_UpdateInstantMetrics(MyRadian.IsConnected)
                    TimerRadian.Start()
                End If
            Case eState.ReadingWaveform
            Case eState.RunningMeterTest
            Case eState.RunningLoopTest
            Case eState.RunningStandardTest
        End Select
    End Sub

#Region "FILE IO"

    Private Function b_CreateFile(ByRef strFileName As String, ByVal SaveDate As Date, ByVal strHeader As String, ByVal bOverWriteFile As Boolean, ByRef strSavePath As String) As Boolean
        'Dim SavePath As String = txtFolderSave.Text
        Dim result As New System.Text.StringBuilder
        b_CreateFile = True 'assume success
        'create streamwriter, False means overwrite if exists
        ' Dim outFile As IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(_strSavePath + "\" + strFileName + SaveDate.ToBinary.ToString + ".csv", False)
        If bOverWriteFile = False Then
            strFileName = strFileName.Substring(0, strFileName.Length - 4) + SaveDate.ToBinary.ToString + ".csv"
        Else
            strFileName = strFileName.Substring(0, strFileName.Length - 4) + ".csv"
        End If
        Dim outFile As IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(strSavePath + strFileName, False)
        Dim temppstring As String = "" ' "Closed Link"

        result.AppendLine("Radian Log File: " + temppstring)
        result.Append("Date:,").AppendLine(SaveDate.ToShortDateString)
        result.Append("Time:,").AppendLine(SaveDate.ToShortTimeString)
        result.AppendLine(strHeader)
        outFile.WriteLine(result)
        outFile.Close()
    End Function



    Private Function b_AppendFile(ByVal strFileName As String, ByVal SaveDate As Date, ByVal strData As String, ByRef strSavePath As String) As Boolean
        Dim SavePath As String = txtFolderSave.Text
        Dim result As New System.Text.StringBuilder
        b_AppendFile = True 'assume success
        Try
            'create streamwriter, False means overwrite if exists
            'Dim AppendStreamFile As New System.IO.StreamWriter(_strSavePath + "\" + strFileName + SaveDate.ToBinary.ToString + ".csv", True)
            Dim AppendStreamFile As New System.IO.StreamWriter(strSavePath + strFileName, True)
            AppendStreamFile.WriteLine(strData)
            AppendStreamFile.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Function

    Private Function b_EndFile(ByVal strFileName As String, ByRef strSavePath As String) As Boolean
        Dim SavePath As String = txtFolderSave.Text
        Dim result As New System.Text.StringBuilder
        b_EndFile = True 'assume success
        'create streamwriter, False means overwrite if exists
        'Dim AppendStreamFile As New System.IO.StreamWriter(_strSavePath + "\" + strFileName + SaveDate.ToBinary.ToString + ".csv", True)
        Dim AppendStreamFile As New System.IO.StreamWriter(strSavePath + strFileName, True)
        AppendStreamFile.WriteLine("")
        AppendStreamFile.Close()
    End Function
#End Region 'FILE IO

    Private Sub txtDataLoggerInterval_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtDataLoggerInterval.Validating
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)
        If IsNumeric((LocalTextBox.Text.Trim)) = False Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Non Numeric Values Not allowed." + vbCrLf + "Please enter a sample interval greater than or equal to 0.5 Seconds!"
            'MsgBox(errorMsg)
            ' Set the ErrorProvider error with the text to display. 
            ErrDataLogger.SetError(LocalTextBox, errorMsg)
        ElseIf Val(LocalTextBox.Text) < 0 Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Please enter a sample interval greater than or equal to 0.5 Seconds!"
            'MsgBox(errorMsg)
            ' Set the ErrorProvider error with the text to display. 
            ErrDataLogger.SetError(LocalTextBox, errorMsg)
            'ElseIf Val(LocalTextBox.Text) > 10000 Then
            '    e.Cancel = True
            '    LocalTextBox.Select(0, LocalTextBox.Text.Length)
            '    Dim errorMsg As String = "Value must be less than or equal to 10000)"
            '    'MsgBox(errorMsg)
            '    ' Set the ErrorProvider error with the text to display. 
            '    ErrLoopTest.SetError(LocalTextBox, errorMsg)
        Else
            My.Settings.strDataLogInterval = LocalTextBox.Text
            e.Cancel = False
            ErrDataLogger.SetError(LocalTextBox, "")

        End If
    End Sub




    Private Sub btnDataLoggerToggleLogger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDataLoggerToggleLogger.Click
        'prompt user to save data
        If gbCurrentDataSaved = False And gStateMachineState = eState.Idle Then
            'TimerRadian.Stop()
            Dim MsgBoxResult As MsgBoxResult = MessageBox.Show("Do you want to Save Current Data?", "Start Logger", MessageBoxButtons.YesNo)
            If MsgBoxResult = DialogResult.No Then
                'force data state to true if user doesn't want to save data
                gbCurrentDataSaved = True
            End If
            ' TimerRadian.Start()

        End If

        'Only enter logging state if data is saved


        If gStateMachineState = eState.Idle And gbCurrentDataSaved Then
            If chkDataLoggerVolts.Checked Or chkDataLoggerCurrent.Checked Or chkDataLoggerFrequency.Checked Or chkDataLoggerPhase.Checked Then
                'change state
                gStateMachineState = eState.ReadingMetics
                gLastState = eState.Idle
                gbCurrentDataSaved = False
                guiSampleCount = 0
                'TimerRadian.Stop()
                'TimerRadian.Interval = 1
                'TimerRadian.Start()
            Else
                MsgBox("Select at least one metric to log before starting data logger")
            End If

        ElseIf gStateMachineState = eState.ReadingMetics Then
            gStateMachineState = eState.Idle
            gLastState = eState.ReadingMetics
            'TimerRadian.Stop()
            'TimerRadian.Interval = 1
            'TimerRadian.Start()
        End If


    End Sub

    Private Sub btnDataSaveLocation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDataSaveLocation.Click
        ' Dim hey As New FolderBrowserDialog
        '   Dim hey2 As New OpenFileDialog
        Dim DataSaveFileDialog As New SaveFileDialog
        ' hey.ShowNewFolderButton = True


        DataSaveFileDialog.InitialDirectory = My.Settings.strDataSaveTestDataSavePath
        DataSaveFileDialog.DefaultExt = ".csv"
        DataSaveFileDialog.Filter = "csv files (*.csv)|*.csv"
        DataSaveFileDialog.FilterIndex = 0
        DataSaveFileDialog.AddExtension = True
        DataSaveFileDialog.CreatePrompt = True
        DataSaveFileDialog.OverwritePrompt = True
        DataSaveFileDialog.SupportMultiDottedExtensions = True
        DataSaveFileDialog.ShowDialog()

        If DataSaveFileDialog.FileName <> "" Then
            Dim tempstring() As String
            tempstring = DataSaveFileDialog.FileName.Split("\")
            txtDataFileName.Text = tempstring(tempstring.Length - 1)
            txtFolderSave.Text = DataSaveFileDialog.FileName.Substring(0, DataSaveFileDialog.FileName.Length - tempstring(tempstring.Length - 1).Length)
        End If


    End Sub



    Private Sub frmRadianLogger_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'radian Integration init
        MyRadian.v_InitPacket()
        MyRadian.v_InitalizeInstantMetricsReadCommandsList()
        MyRadian.v_InitInstantMetricsArrayListNamesRD2X()
        MyRadian.v_InitAccumMetricsArrayListNamesRD2X()
        MyRadian.v_InitAccumMetricsReadCommandListRD2X()
        MyRadian.v_InitrRadianErrorCodes()
        MyRadian.v_initVoltageList()
        MyRadian.v_initCurrentList(False) 'assume 120 or 200 amp standard! 
        MyRadian.v_initTriggerWaveFormStatusMessages()
        MyRadian.WaveForm.v_Init()

        '
        'Add Handlers so that Listboxes will wrap text
        SetListBoxToWrap(lbGPIB_Log)
        SetListBoxToWrap(lbIdentification)
        SetListBoxToWrap(lbDataLog)

        MyAgilent34970A.Parameters.init()

        'counting variable
        Dim i As Integer = 0

        'add each Instantaneous Metric Name to List Box
        For Each item In MyRadian.aInstantMetricsUnitLabelList
            cbSelectInstantMetrics.Items.Add(MyRadian.aInstantMetricsUnitLabelList.Item(i))
            i += 1
        Next

        'Select the lowest indexed item to populate list box.
        cbSelectInstantMetrics.SelectedItem = MyRadian.aInstantMetricsUnitLabelList.Item(0)
        i = 0
        'add each accumulated Metric Name to List Box
        For Each item In MyRadian.aAccumMetricsUnitLabelList
            cbSelectAccumMetrics.Items.Add(MyRadian.aAccumMetricsUnitLabelList.Item(i))
            i += 1
        Next
        'Select the lowest indexed item to populate list box.
        cbSelectAccumMetrics.SelectedItem = MyRadian.aAccumMetricsUnitLabelList.Item(0)

        'Auto fill the Comport selection combobox
        cbMeter_COMPorts.Items.Clear()

        For i = 0 To My.Computer.Ports.SerialPortNames.Count - 1
            cbMeter_COMPorts.Items.Add( _
               My.Computer.Ports.SerialPortNames(i).Replace("COM", ""))
        Next

        If cbMeter_COMPorts.Items.Count = 0 Then
            cbMeter_COMPorts.Items.Add("NONE!!")
        Else
            cbMeter_COMPorts.Sorted = True
        End If

        For i = 0 To My.Computer.Ports.SerialPortNames.Count - 1
            If cbMeter_COMPorts.Items(i) = My.Settings.strComPortValue Then
                cbMeter_COMPorts.SelectedIndex = i
            End If
        Next


        chkDataLoggerVolts.Checked = My.Settings.bchkDataLogVolt
        chkDataLoggerCurrent.Checked = My.Settings.bchkDataLogCurrent
        chkDataLoggerFrequency.Checked = My.Settings.bchkDataLogFreq
        chkDataLoggerPhase.Checked = My.Settings.bchkDataLogPhase
        txtDataLoggerInterval.Text = My.Settings.strDataLogInterval
        txtDataLoggerHeader.Text = My.Settings.strDataLoggerHeader
        rbDataSaveTab.Checked = My.Settings.bDataSaveDelimiterTab
        rbDataSaveComma.Checked = My.Settings.bDataSaveDelimiterComma
        rbDataSaveSemiColon.Checked = My.Settings.bDataSaveDelimiterSemiColon
        rbDataSaveSpace.Checked = My.Settings.bDataSaveDelimiterSpace

        cbVoltage_CurrentSetpoint.SelectedIndex = My.Settings.iCI501TAC_Default_Current
        cbVoltage_CurrentSetpoint.SelectedItem = My.Settings.iCI501TAC_Default_Current
        txtVoltage_ControlDeadBand.Text = My.Settings.strCI501TAC_CONTROLLER_DEADBAND
        txtVoltage_AccuracyDeadband.Text = My.Settings.strCI501TAC_ACCURACY_DEADBAND
        txtSetVoltage.Text = My.Settings.strCI501TAC_NOMINAL_VOLTAGE

        WrapListBoxFonts = lbGPIB_Log.Font 'Set to one listbox...all will be the same.
        TimerRadian.Interval = 250
        TimerRadian.Start()
    End Sub


    Private Sub TimerSample_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerSample.Tick
        Dim strLog As String

        strLog = "" 'clear string
        guiSampleCount += 1
        strLog = guiSampleCount.ToString
        If chkDataLoggerVolts.Checked Then
            If Math.Abs(MyRadian.MetricData.Volt) < 10 Then
                strLog = strLog + vbTab + MyRadian.MetricData.Volt.ToString("0.00000")
            ElseIf Math.Abs(MyRadian.MetricData.Volt) < 100 Then
                strLog = strLog + vbTab + MyRadian.MetricData.Volt.ToString("00.0000")
            Else
                strLog = strLog + vbTab + MyRadian.MetricData.Volt.ToString("000.000")
            End If
        End If

        If chkDataLoggerCurrent.Checked Then
            If Math.Abs(MyRadian.MetricData.Amp) < 10 Then
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Amp.ToString("0.00000")
            ElseIf Math.Abs(MyRadian.MetricData.Amp) < 100 Then
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Amp.ToString("00.0000")
            Else
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Amp.ToString("000.000")
            End If
        End If

        If chkDataLoggerFrequency.Checked Then
            If Math.Abs(MyRadian.MetricData.Frequency) < 10 Then
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Frequency.ToString("0.00000")
            ElseIf Math.Abs(MyRadian.MetricData.Frequency) < 100 Then
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Frequency.ToString("00.0000")
            Else
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Frequency.ToString("000.000")
            End If

        End If

        If chkDataLoggerPhase.Checked Then

            If Math.Abs(MyRadian.MetricData.Phase) < 10 Then
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Phase.ToString("0.00000")
            ElseIf Math.Abs(MyRadian.MetricData.Phase) < 100 Then
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Phase.ToString("00.0000")
            Else
                strLog = strLog + vbTab + vbTab + MyRadian.MetricData.Phase.ToString("000.000")
            End If

        End If

        strLog = strLog + vbTab + vbTab + Now.ToString
        lbDataLog.Items.Add(strLog)
        lbDataLog.TopIndex = lbDataLog.Items.Count - 1
        TimerSample.Interval = Val(cbDAQParametersReadingIntervals.Text) * 1000

        If chkCloseLoop.Checked = True Then
            TimerRadian.Stop()
            v_UpdateInstantMetrics(True)
            TimerRadian.Start()
            Dim Difference As Single
            Difference = MyRadian.MetricData.Amp - Val(cbVoltage_CurrentSetpoint.Text)

            Dim sControlDeadBand As Single = (Val(txtVoltage_ControlDeadBand.Text) / 100 * Val(cbVoltage_CurrentSetpoint.Text))

            If Math.Abs(Difference) < sControlDeadBand Then 'make sure within Control Limit

                Dim sAccuracyDeadBand As Single = Val(txtVoltage_AccuracyDeadband.Text) / 100 * Val(cbVoltage_CurrentSetpoint.Text)

                If Math.Abs(Difference) > sAccuracyDeadBand / 1.7 Then 'only change setpoint if greater than .5% difference
                    Dim LocalSetpoint As String = ""

                    Dim SourceSetPoint As String = ""
                    If rbUseCaliforniaInstruments.Checked Then
                        SourceSetPoint = MyCICA501TAC._VoltageASetPoint
                    Else
                        SourceSetPoint = MyPacPower._VoltageASetPoint
                    End If
                    LocalSetpoint = Math.Round((Val(SourceSetPoint) * Val(cbVoltage_CurrentSetpoint.Text) / MyRadian.MetricData.Amp), 1).ToString

                    v_UpdateGPIBLog("Time: " + Now.ToString, gGPIBTextWidth)
                    v_UpdateGPIBLog("Measured Current: " + MyRadian.MetricData.Amp.ToString + " Amps", gGPIBTextWidth)
                    v_UpdateGPIBLog("Old Voltage Set Point: " + SourceSetPoint + " Volts", gGPIBTextWidth)
                    v_UpdateGPIBLog("New Voltage Set Point: " + LocalSetpoint + " Volts", gGPIBTextWidth)

                    Dim Message As String = ""
                    Dim Success As Integer = 0 'assume success
                    If rbUseCaliforniaInstruments.Checked Then
                        Success = MyCICA501TAC.Write(GpibDevice, "AMPA " + LocalSetpoint)
                    Else
                        Dim Command As String = MyPacPower.SourceCommand.SetVoltage1(LocalSetpoint) 
                        Success = MyPacPower.i_Transmit(Command, Message)
                    End If


                    If Message <> "" Then
                        v_UpdateGPIBLog("Write Error: " + Message, gGPIBTextWidth)
                        Windows.Forms.Cursor.Current = Cursors.Default
                        Exit Sub
                    End If

                    Dim TickCount As Integer = My.Computer.Clock.TickCount
                    Dim TimeOut As Integer = TickCount + 100 '100mS


                    While TickCount < TimeOut
                        Application.DoEvents()
                        TickCount = My.Computer.Clock.TickCount
                    End While

                    btnSetVoltage.Enabled = True
                    Windows.Forms.Cursor.Current = Cursors.Default

                    If rbUseCaliforniaInstruments.Checked Then
                        MyCICA501TAC._VoltageASetPoint = LocalSetpoint.ToString
                    Else
                        MyPacPower._VoltageASetPoint = LocalSetpoint.ToString
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub chkDataLoggerVolts_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDataLoggerVolts.CheckedChanged
        My.Settings.bchkDataLogVolt = chkDataLoggerVolts.Checked

    End Sub

    Private Sub chkDataLoggerCurrent_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDataLoggerCurrent.CheckedChanged
        My.Settings.bchkDataLogCurrent = chkDataLoggerCurrent.Checked

    End Sub

    Private Sub chkDataLoggerFrequency_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDataLoggerFrequency.CheckedChanged
        My.Settings.bchkDataLogFreq = chkDataLoggerFrequency.Checked

    End Sub

    Private Sub chkDataLoggerPhase_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkDataLoggerPhase.CheckedChanged
        My.Settings.bchkDataLogPhase = chkDataLoggerPhase.Checked
    End Sub

    Private Sub rbDataSaveTab_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbDataSaveTab.CheckedChanged
        My.Settings.bDataSaveDelimiterTab = rbDataSaveTab.Checked
    End Sub

    Private Sub rbDataSaveSpace_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbDataSaveSpace.CheckedChanged
        My.Settings.bDataSaveDelimiterSpace = rbDataSaveSpace.Checked
    End Sub

    Private Sub rbDataSaveComma_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbDataSaveComma.CheckedChanged
        My.Settings.bDataSaveDelimiterComma = rbDataSaveComma.Checked
    End Sub

    Private Sub rbDataSaveColon_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbDataSaveSemiColon.CheckedChanged
        My.Settings.bDataSaveDelimiterSemiColon = rbDataSaveSemiColon.Checked
    End Sub

    Private Sub cbMeter_COMPorts_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbMeter_COMPorts.SelectedIndexChanged
        My.Settings.strComPortValue = cbMeter_COMPorts.Items(cbMeter_COMPorts.SelectedIndex)
        Dim i As Integer
        i = 4
    End Sub

    Private Sub btnClearLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearLog.Click
        lblLogData.Items.Clear()
    End Sub

    Private Sub chkVerbose_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkErrorLogVerbose.CheckedChanged
        gbVerbose = chkErrorLogVerbose.Checked
    End Sub

    Private Sub btnSaveLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveLog.Click
        Dim iIndex As Integer
        Dim SaveDate As Date
        Dim result As New System.Text.StringBuilder
        Dim SavePath As String = Nothing
        Dim SaveDialog As New SaveFileDialog

        SaveDialog.CheckPathExists = True
        If My.Settings.LogFileDirectory <> "" Then
            SaveDialog.InitialDirectory = My.Settings.LogFileDirectory 'My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\RadianLogger\"
        Else
            SaveDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End If


        'v_UpdateLog(My.Settings.LogFileDirectory)
        SaveDialog.ShowDialog()
        Try
            My.Settings.LogFileDirectory = System.IO.Path.GetDirectoryName(SaveDialog.FileName)
        Catch 'Catch error

        End Try
        v_UpdateLog(My.Settings.LogFileDirectory)



        SavePath = SaveDialog.FileName
        If SavePath = Nothing Then Exit Sub

        'put all or displayed data array in big string

        Dim outFile As IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(SavePath, False)
        SaveDate = Now
        result.Append("RadianLogger System Log File")
        result.Append("Date: ").AppendLine(SaveDate.ToShortDateString)
        result.Append("Time: ").AppendLine(SaveDate.ToShortTimeString)
        result.Append("------------------------------------------" + vbNewLine)
        For iIndex = 0 To lblLogData.Items.Count - 1
            result.Append(lblLogData.Items(iIndex).ToString + vbNewLine)
        Next
        outFile.WriteLine(result)
        outFile.Close()
    End Sub

    Private Sub btnRadianLoggerSaveData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRadianLoggerSaveData.Click
        Dim iIndex As Integer
        Dim SaveDate As Date
        Dim result As New System.Text.StringBuilder
        Dim SavePath As String = Nothing
        Dim SaveDialog As New SaveFileDialog

        SaveDialog.CheckPathExists = True
        If My.Settings.DataFileDirectory <> "" Then
            SaveDialog.InitialDirectory = My.Settings.DataFileDirectory 'My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\RadianLogger\"
        Else
            SaveDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End If

        'Make CSV the default format and automatically append to the file...
        SaveDialog.DefaultExt = ".csv"
        SaveDialog.Filter = "csv files (*.csv)|*.csv"
        SaveDialog.FilterIndex = 0
        SaveDialog.AddExtension = True
        SaveDialog.CreatePrompt = False
        SaveDialog.OverwritePrompt = True
        SaveDialog.SupportMultiDottedExtensions = True
        SaveDialog.ValidateNames = True

        'v_UpdateLog(My.Settings.DataFileDirectory)
        SaveDialog.ShowDialog()
        Try
            My.Settings.DataFileDirectory = System.IO.Path.GetDirectoryName(SaveDialog.FileName)
        Catch 'Catch error

        End Try
        v_UpdateLog(My.Settings.DataFileDirectory)



        SavePath = SaveDialog.FileName
        If SavePath = Nothing Then Exit Sub
        'SavePath = SavePath + ".csv"
        'put all or displayed data array in big string

        Dim outFile As IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(SavePath, False)
        SaveDate = Now
        result.Append("RadianLogger System Log File")
        result.Append("Date: ").AppendLine(SaveDate.ToShortDateString)
        result.Append("Time: ").AppendLine(SaveDate.ToShortTimeString)
        result.Append("Notes: ").AppendLine(txtLogDataNotes.Text)
        result.Append("------------------------------------------" + vbNewLine)
        For iIndex = 0 To lbDataLog.Items.Count - 2
            Dim strTempString As String = lbDataLog.Items(iIndex).ToString
            Dim strTempDelimiter As String = " "

            If rbDataSaveSemiColon.Checked Then
                strTempDelimiter = ";"
            ElseIf rbDataSaveComma.Checked Then
                strTempDelimiter = ","
            ElseIf rbDataSaveTab.Checked Then
                strTempDelimiter = vbTab
            ElseIf rbDataSaveSpace.Checked Then
                strTempDelimiter = " "
            End If

            ' Find and replace any occurences of multiple tabs
            Do While (InStr(strTempString, vbTab + vbTab))
                ' if true, the string still contains double tabs
                ' replace with single tab
                strTempString = Replace(strTempString, vbTab + vbTab, vbTab)
            Loop

            ' File is now tab delimited; change Delimiter only if needed 
            If strTempDelimiter <> vbTab Then
                Do While (InStr(strTempString, vbTab))
                    ' if true, the string still contains a space(s),
                    ' replace with new demitier...
                    strTempString = Replace(strTempString, vbTab, strTempDelimiter)
                Loop
            End If
            'now append to file...

            result.Append(strTempString + vbNewLine)
            'result.Append(lbDataLog.Items(iIndex).ToString + vbNewLine)
        Next
        If lbDataLog.Items.Count > 0 Then  'Make sure there are results
            result.Append(lbDataLog.Items(lbDataLog.Items.Count - 1).ToString)
        End If
        outFile.WriteLine(result)
        outFile.Close()
        gbCurrentDataSaved = True
    End Sub

    Private Sub btnSetVoltage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetVoltage.Click
        btnSetVoltage.Enabled = False
        Windows.Forms.Cursor.Current = Cursors.WaitCursor

        Dim Message As String = ""
        Dim status As Integer = 0

        If MyPacPower.SourceCommand Is Nothing Then
            MyPacPower.Init()
        End If

        If rbUseCaliforniaInstruments.Checked = True Then
            status = MyCICA501TAC.Write(GpibDevice, "AMPA " + txtSetVoltage.Text.Trim, Message)
        Else
            Dim Command As String = MyPacPower.SourceCommand.SetVoltage1(txtSetVoltage.Text.Trim) + ";" + MyPacPower.SourceCommand.SetOutputState( _
                                          Current_Temperature.mPacPowerUCP1.cPacPowerUPC1.cCommands.TOGGLE_STATE._ON)

            status = MyPacPower.i_Transmit(Command, Message)
         
        End If


        If Message <> "" Then
            v_UpdateGPIBLog("Write Error: " + Message, gGPIBTextWidth)
            btnSetVoltage.Enabled = True
            Windows.Forms.Cursor.Current = Cursors.Default
            Exit Sub

        End If

        Dim TickCount As Integer = My.Computer.Clock.TickCount
        Dim TimeOut As Integer = TickCount + 100 '100mS


        While TickCount < TimeOut
            Application.DoEvents()
            TickCount = My.Computer.Clock.TickCount
        End While

        btnSetVoltage.Enabled = True
        Windows.Forms.Cursor.Current = Cursors.Default
        If rbUseCaliforniaInstruments.Checked = True Then
            MyCICA501TAC._VoltageASetPoint = txtSetVoltage.Text.Trim
        Else
            MyPacPower._VoltageASetPoint = txtSetVoltage.Text.Trim
        End If

    End Sub

    Private Sub txtSetVoltage_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtSetVoltage.Validating
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)

        If (LocalTextBox.Text.Trim) = "" Then
            e.Cancel = False
            btnSetVoltage.Enabled = False
        ElseIf IsNumeric((LocalTextBox.Text.Trim)) = False Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Voltage must be a value in range of 0.0 to 150.0 Volts"
            ErrDataLogger.SetError(LocalTextBox, errorMsg)
            'btnCI501TCA_SetVoltage.Enabled = False
        ElseIf Val(LocalTextBox.Text.Trim) > 150.0 Or Val(LocalTextBox.Text.Trim) < 0.0 Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Voltage must be a value in range of 0.0 to 150.0 Volts"
            ErrDataLogger.SetError(LocalTextBox, errorMsg)
            'btnCI501TCA_SetVoltage.Enabled = False
        Else
            e.Cancel = False
            LocalTextBox.Text = Round(Val(LocalTextBox.Text), 1).ToString
            ErrDataLogger.SetError(LocalTextBox, "")
            If InStr(LocalTextBox.Text, ".") = 0 Then
                LocalTextBox.Text = LocalTextBox.Text + ".0"
            End If
            If InStr(LocalTextBox.Text, ".") = Trim(LocalTextBox.Text).Length Then
                LocalTextBox.Text = LocalTextBox.Text + "0"
            End If
            btnSetVoltage.Enabled = True
        End If

    End Sub

    Private Sub txtVoltage_SetVoltage_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSetVoltage.TextChanged
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)

        If (LocalTextBox.Text.Trim) = "" Then
            'e.Cancel = False
            btnSetVoltage.Enabled = False
        ElseIf IsNumeric((LocalTextBox.Text.Trim)) = False Then
            ' e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            'Dim errorMsg As String = "Voltage must be a value in range of 0.0 to 135.0 Volts"
            '  ErrDataLogger.SetError(LocalTextBox, errorMsg)
            btnSetVoltage.Enabled = False
        ElseIf Val(LocalTextBox.Text.Trim) > 150.0 Or Val(LocalTextBox.Text.Trim) < 0.0 Then
            'e.Cancel = True
            'LocalTextBox.Select(0, LocalTextBox.Text.Length)
            ' Dim errorMsg As String = "Voltage must be a value in range of 0.0 to 135.0 Volts"
            'ErrDataLogger.SetError(LocalTextBox, errorMsg)
            btnSetVoltage.Enabled = False
        Else
            'e.Cancel = False
            btnSetVoltage.Enabled = True
        End If
    End Sub

    Private Sub txtVoltage_ControlDeadBand_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtVoltage_ControlDeadBand.Validating
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)

        If (LocalTextBox.Text.Trim) = "" Then
            e.Cancel = False
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False
        ElseIf IsNumeric((LocalTextBox.Text.Trim)) = False Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Percentage must be a value in range of 10.0 to 1.0 Percent"
            ErrDataLogger.SetError(LocalTextBox, errorMsg)

        ElseIf Val(LocalTextBox.Text.Trim) > 10.0 Or Val(LocalTextBox.Text.Trim) < 1.0 Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Percentage must be a value in range of 10.0 to 1.0 Percent"
            ErrDataLogger.SetError(LocalTextBox, errorMsg)
            'btnCI501TCA_SetVoltage.Enabled = False
        Else
            e.Cancel = False
            LocalTextBox.Text = Round(Val(LocalTextBox.Text), 1).ToString
            ErrDataLogger.SetError(LocalTextBox, "")
            If InStr(LocalTextBox.Text, ".") = 0 Then
                LocalTextBox.Text = LocalTextBox.Text + ".0"
            End If
            If InStr(LocalTextBox.Text, ".") = Trim(LocalTextBox.Text).Length Then
                LocalTextBox.Text = LocalTextBox.Text + "0"
            End If
            chkCloseLoop.Enabled = True
            If Val(txtVoltage_AccuracyDeadband.Text) > Val(LocalTextBox.Text) Then
                txtVoltage_AccuracyDeadband.Text = Round(Val(LocalTextBox.Text) / 10, 2).ToString
            End If
        End If
    End Sub
    Private Sub txtVoltage_ControlDeadBand_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtVoltage_ControlDeadBand.TextChanged
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)

        If (LocalTextBox.Text.Trim) = "" Then

            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False
        ElseIf IsNumeric((LocalTextBox.Text.Trim)) = False Then
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False
        ElseIf Val(LocalTextBox.Text.Trim) > 10.0 Or Val(LocalTextBox.Text.Trim) < 1.0 Then
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False

        Else
            If Val(txtVoltage_AccuracyDeadband.Text) > Val(LocalTextBox.Text) Then
                txtVoltage_AccuracyDeadband.Text = Round(Val(LocalTextBox.Text) / 10, 2).ToString
            End If
            chkCloseLoop.Enabled = True
        End If
    End Sub

    Private Sub txtVoltage_AccuracyDeadband_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtVoltage_AccuracyDeadband.Validating
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)

        If (LocalTextBox.Text.Trim) = "" Then
            e.Cancel = False
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False
        ElseIf IsNumeric((LocalTextBox.Text.Trim)) = False Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Percentage must be a value in range of 1.0 to 0.1 Percent"
            ErrDataLogger.SetError(LocalTextBox, errorMsg)

        ElseIf Val(LocalTextBox.Text.Trim) > 1.0 Or Val(LocalTextBox.Text.Trim) < 0.1 Then
            e.Cancel = True
            LocalTextBox.Select(0, LocalTextBox.Text.Length)
            Dim errorMsg As String = "Percentage must be a value in range of 1.0 to 0.1 Percent"
            ErrDataLogger.SetError(LocalTextBox, errorMsg)
            'btnCI501TCA_SetVoltage.Enabled = False
        Else
            e.Cancel = False
            LocalTextBox.Text = Round(Val(LocalTextBox.Text), 1).ToString
            ErrDataLogger.SetError(LocalTextBox, "")
            If InStr(LocalTextBox.Text, ".") = 0 Then
                LocalTextBox.Text = LocalTextBox.Text + ".0"
            End If
            If InStr(LocalTextBox.Text, ".") = Trim(LocalTextBox.Text).Length Then
                LocalTextBox.Text = LocalTextBox.Text + "0"
            End If
            chkCloseLoop.Enabled = True
            If Val(txtVoltage_ControlDeadBand.Text) < Val(LocalTextBox.Text) Then
                txtVoltage_ControlDeadBand.Text = Round(Val(LocalTextBox.Text) * 10, 1).ToString
            End If
        End If
    End Sub


    Private Sub txtVoltage_AccuracyDeadband_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtVoltage_AccuracyDeadband.TextChanged
        Dim LocalTextBox As TextBox = DirectCast(sender, TextBox)

        If (LocalTextBox.Text.Trim) = "" Then
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False
        ElseIf IsNumeric((LocalTextBox.Text.Trim)) = False Then
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False

        ElseIf Val(LocalTextBox.Text.Trim) > 1.0 Or Val(LocalTextBox.Text.Trim) < 0.1 Then
            chkCloseLoop.Checked = False
            chkCloseLoop.Enabled = False
        Else

            chkCloseLoop.Enabled = True
            If Val(txtVoltage_ControlDeadBand.Text) < Val(LocalTextBox.Text) Then
                txtVoltage_ControlDeadBand.Text = Round(Val(LocalTextBox.Text) * 10, 1).ToString
            End If
        End If
    End Sub

    Private Sub chkCloseLoop_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkCloseLoop.CheckedChanged
        If chkCloseLoop.Checked = True Then
            txtVoltage_AccuracyDeadband.Enabled = False
            txtVoltage_ControlDeadBand.Enabled = False
            txtSetVoltage.Enabled = False
            cbVoltage_CurrentSetpoint.Enabled = False
            btnSetVoltage.Enabled = False
            btnVoltageOff.Enabled = False
        Else
            txtVoltage_AccuracyDeadband.Enabled = True
            txtVoltage_ControlDeadBand.Enabled = True
            txtSetVoltage.Enabled = True
            cbVoltage_CurrentSetpoint.Enabled = True
            btnSetVoltage.Enabled = True
            btnVoltageOff.Enabled = True
        End If
    End Sub

    Public Sub CI501TCA_VoltageOff()
        Windows.Forms.Cursor.Current = Cursors.WaitCursor
        'chkCloseLoop.Checked = False
        Dim strWriteError As String = MyCICA501TAC.Write(GpibDevice, "AMPA 0.0")

        If strWriteError <> "" Then
            v_UpdateGPIBLog("Write Error: " + strWriteError, gGPIBTextWidth)
            Windows.Forms.Cursor.Current = Cursors.Default
            Exit Sub

        End If

        Dim TickCount As Integer = My.Computer.Clock.TickCount
        Dim TimeOut As Integer = TickCount + 100 '100mS


        While TickCount < TimeOut
            Application.DoEvents()
            TickCount = My.Computer.Clock.TickCount
        End While

        MyCICA501TAC._VoltageASetPoint = "0.0"
        Windows.Forms.Cursor.Current = Cursors.Default
    End Sub

    Private Sub btnVoltageOff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVoltageOff.Click
        btnVoltageOff.Enabled = False
        Windows.Forms.Cursor.Current = Cursors.WaitCursor


        Dim Message As String = ""
        Dim Status As Integer = 0 'assume Success

        If rbUseCaliforniaInstruments.Checked Then
            MyCICA501TAC.Write(GpibDevice, "AMPA " + "0".Trim, Message)
        Else
            Dim Command As String = MyPacPower.SourceCommand.SetVoltage1("0") + ";" + MyPacPower.SourceCommand.SetOutputState( _
                                        Current_Temperature.mPacPowerUCP1.cPacPowerUPC1.cCommands.TOGGLE_STATE._OFF)

            Status = MyPacPower.i_Transmit(Command, Message)
        End If


        If Message <> "" Then
            v_UpdateGPIBLog("Write Error: " + Message, gGPIBTextWidth)
            btnVoltageOff.Enabled = True
            Windows.Forms.Cursor.Current = Cursors.Default
            Exit Sub

        End If

        Dim TickCount As Integer = My.Computer.Clock.TickCount
        Dim TimeOut As Integer = TickCount + 100 '100mS


        While TickCount < TimeOut
            Application.DoEvents()
            TickCount = My.Computer.Clock.TickCount
        End While

        btnVoltageOff.Enabled = True
        Windows.Forms.Cursor.Current = Cursors.Default
        If rbUseCaliforniaInstruments.Checked Then
            MyCICA501TAC._VoltageASetPoint = "0".Trim
        Else
            MyPacPower._VoltageASetPoint = "0".Trim
        End If

    End Sub


    Private Sub btnCI501TCA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCI501TCA.Click

        frmCI501TCA_Defaults.ShowDialog()

        If My.Settings.bCI501TAC_CopyValuesOnSave Then
            txtVoltage_ControlDeadBand.Text = My.Settings.strCI501TAC_CONTROLLER_DEADBAND
            txtVoltage_AccuracyDeadband.Text = My.Settings.strCI501TAC_ACCURACY_DEADBAND
            cbVoltage_CurrentSetpoint.SelectedIndex = My.Settings.iCI501TAC_Default_Current
            cbVoltage_CurrentSetpoint.SelectedItem = My.Settings.iCI501TAC_Default_Current
        End If

        If Val(txtSetVoltage.Text) > Val(My.Settings.strCI501TAC_MaxVoltage) Then
            txtSetVoltage.Text = My.Settings.strCI501TAC_MaxVoltage
        End If

    End Sub


    Private Sub btnSaveGPIB_LOG_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveGPIB_LOG.Click
        Dim iIndex As Integer
        Dim SaveDate As Date
        Dim result As New System.Text.StringBuilder
        Dim SavePath As String = Nothing
        Dim SaveDialog As New SaveFileDialog

        SaveDialog.CheckPathExists = True
        If My.Settings.LogFileDirectory <> "" Then
            SaveDialog.InitialDirectory = My.Settings.LogFileDirectory 'My.Computer.FileSystem.SpecialDirectories.MyDocuments + "\RadianLogger\"
        Else
            SaveDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End If

        'v_UpdateLog(My.Settings.LogFileDirectory)
        SaveDialog.DefaultExt = ".txt"
        SaveDialog.Filter = "txt files (*.txt)|*.txt"
        SaveDialog.FilterIndex = 0
        SaveDialog.AddExtension = True
        SaveDialog.CreatePrompt = False
        SaveDialog.OverwritePrompt = True
        SaveDialog.SupportMultiDottedExtensions = True
        SaveDialog.ValidateNames = True
        SaveDialog.ShowDialog()
        Try
            My.Settings.LogFileDirectory = System.IO.Path.GetDirectoryName(SaveDialog.FileName)
        Catch 'Catch error

        End Try


        SavePath = SaveDialog.FileName
        If SavePath = Nothing Then Exit Sub
        v_UpdateGPIBLog(My.Settings.LogFileDirectory)
        'put all or displayed data array in big string

        Dim outFile As IO.StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(SavePath, False)
        SaveDate = Now
        result.Append("California Instruments GPIB System Log File")
        result.Append("Date: ").AppendLine(SaveDate.ToShortDateString)
        result.Append("Time: ").AppendLine(SaveDate.ToShortTimeString)
        result.Append("------------------------------------------" + vbNewLine)
        For iIndex = 0 To lbGPIB_Log.Items.Count - 1
            result.Append(lbGPIB_Log.Items(iIndex).ToString + vbNewLine)
        Next
        outFile.WriteLine(result)
        outFile.Close()
    End Sub

    Private Sub frmCurrentController_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        gGPIBTextWidth = 50 'Trying to make sure everything fits...
        If MyPacPower.SourceCommand Is Nothing Then
            MyPacPower.Init()
        End If
    End Sub

    Private Sub btnTest_Click(sender As System.Object, e As System.EventArgs) Handles btnTest.Click
        v_UpdateGPIBLog(TextBox2.Text, gGPIBTextWidth)
    End Sub

#Region "Wrapped List Box Code"
    'http://vbcity.com/forums/t/130065.aspx
    'Author Ged Mead 
    'The following code is based on a sample from Evangelos Petroutsos's excellent "Mastering VB2005" book.
    'First set the listbox OwnerDraw property 
    'to OwnerDrawVariable via the Properties window. '
    'Then you can use the ListBox's built in events as shown below:
    'Add 
    '   Public FNT As Font
    '   Set FNT = lbGPIB_Log.Font in Form load event

    Private Sub SetListBoxToWrap(ByRef MyListBox As ListBox)
        MyListBox.DrawMode = DrawMode.OwnerDrawVariable
        AddHandler MyListBox.DrawItem, AddressOf WrapListBox_DrawItem
        AddHandler MyListBox.MeasureItem, AddressOf WrapListBox_MeasureItem

    End Sub

    Private Sub InializeWrapListBox()
        'Add Handlers so that Listboxes will wrap text
        SetListBoxToWrap(lbGPIB_Log)
        SetListBoxToWrap(lbIdentification)
        SetListBoxToWrap(lbDataLog)
    End Sub

    Private Sub WrapListBox_DrawItem(sender As System.Object, e As System.Windows.Forms.DrawItemEventArgs) 'Handles lbGPIB_Log.DrawItem
        Dim LocalListBox As ListBox = DirectCast(sender, ListBox)
        If e.Index = -1 Then Exit Sub
        e.DrawBackground()
        Dim txtBrush As SolidBrush = New SolidBrush(Color.Black) ' Text Color
        Dim bgBrush As SolidBrush = New SolidBrush(Color.White) 'Background Color
        Dim txtfnt As Font = WrapListBoxFonts 'Text Font

        e.Graphics.FillRectangle(bgBrush, e.Bounds) 'Fill the Rectangle with Background Color
        e.Graphics.DrawRectangle(Pens.LightGray, e.Bounds)
        Dim R As New RectangleF(e.Bounds.X, e.Bounds.Y, _
                                e.Bounds.Width, e.Bounds.Height)
        e.Graphics.DrawString(LocalListBox.Items(e.Index).ToString, txtfnt, txtBrush, R) 'This draws the text bounded by the rectangle
        e.DrawFocusRectangle()
    End Sub

    Private Sub WrapListBox_MeasureItem(sender As System.Object, e As System.Windows.Forms.MeasureItemEventArgs) 'Handles lbGPIB_Log.MeasureItem
        Dim LocalListBox As ListBox = DirectCast(sender, ListBox)
        If WrapListBoxFonts Is Nothing Then Exit Sub
        Dim itmSize As SizeF
        Dim S As New SizeF(LocalListBox.Width, 200)
        itmSize = e.Graphics.MeasureString(LocalListBox.Items(e.Index).ToString, WrapListBoxFonts, S)
        e.ItemHeight = Convert.ToInt32(itmSize.Height)
        e.ItemWidth = Convert.ToInt32(itmSize.Width)
    End Sub

#End Region '"Wrapped List Box Code"


    Private Sub btnSaveConfig_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveConfig.Click
        Dim filenum As Integer = FreeFile()
        FileClose()

    End Sub

    Private Sub btnLoadConfig_Click(sender As System.Object, e As System.EventArgs) Handles btnLoadConfig.Click
        Try
            Dim FileNum As Integer = FreeFile()
            FileOpen(FileNum, "C:\Users\chengya\Desktop\YC-Settings.txt", OpenMode.Input)
            Dim indexes() As String = LineInput(FileNum).Split(",")
            For i = 0 To indexes.Count - 1
                MyAgilent34970A.channelArray(indexes(i)).enableCheckBox.Enabled = True
                MyAgilent34970A.channelArray(indexes(i)).enableCheckBox.Checked = True
            Next
            FileClose()
        Catch
            MessageBox.Show("Error occurred!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub txtDAQCountDownHH_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles txtDAQCountDownHH.Validating
        txtDAQCountDownHH.Text = txtDAQCountDownHH.Text.Replace(" ", "")
        If System.Text.RegularExpressions.Regex.IsMatch(txtDAQCountDownHH.Text, "^[0-9]+$") Then
            If CInt(Val(txtDAQCountDownHH.Text)) < 0 Then
                e.Cancel = True
                txtDAQCountDownHH.Select(0, txtDAQCountDownHH.Text.Length)
                ErrorProvider.SetError(txtDAQCountDownHH, "The value should be greater than or equal to 0.")
            Else
                ErrorProvider.SetError(txtDAQCountDownHH, "")
            End If
        Else
            If txtDAQCountDownHH.Text = "" Then
                e.Cancel = False
                ErrorProvider.SetError(txtDAQCountDownHH, "")
            Else
                e.Cancel = True
                txtDAQCountDownHH.Select(0, txtDAQCountDownHH.Text.Length)
                ErrorProvider.SetError(txtDAQCountDownHH, "The value must be an integer.")
            End If

        End If
    End Sub

    Private Sub txtDAQCountDownMM_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles txtDAQCountDownMM.Validating
        Minutes_Seconds_Validating(txtDAQCountDownMM, sender, e)
    End Sub
    Private Sub txtDAQCountDownSS_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles txtDAQCountDownSS.Validating
        Minutes_Seconds_Validating(txtDAQCountDownSS, sender, e)
    End Sub
    Private Sub Minutes_Seconds_Validating(ByVal text As TextBox, sender As System.Object, e As System.ComponentModel.CancelEventArgs)
        text.Text = text.Text.Replace(" ", "")
        If System.Text.RegularExpressions.Regex.IsMatch(text.Text, "^[0-9]+$") Then
            If CInt(Val(text.Text)) < 0 Or CInt(Val(text.Text)) >= 60 Then
                e.Cancel = True
                text.Select(0, text.Text.Length)
                ErrorProvider.SetError(text, "The value should be in the range from 0 to 59.")
            Else
                ErrorProvider.SetError(text, "")
            End If
        Else
            If text.Text = "" Then
                e.Cancel = False
                ErrorProvider.SetError(text, "")
            Else
                e.Cancel = True
                ErrorProvider.SetError(text, "The value must be an integer.")
                text.Select(0, text.Text.Length)
            End If

        End If
    End Sub


    Private Sub tbDAQNumberofReadings_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles txtDAQNumberofReadings.Validating
        txtDAQNumberofReadings.Text = txtDAQNumberofReadings.Text.Replace(" ", "")
        If System.Text.RegularExpressions.Regex.IsMatch(txtDAQNumberofReadings.Text, "^[+-]?[0-9]+$") Then
            If CInt(Val(txtDAQNumberofReadings.Text)) <= 0 Then
                e.Cancel = True
                txtDAQNumberofReadings.Select(0, txtDAQNumberofReadings.Text.Length)
                ErrorProvider.SetError(txtDAQNumberofReadings, "The value should be greater than 0.")
            Else
                e.Cancel = False
                ErrorProvider.SetError(txtDAQNumberofReadings, "")
            End If
        Else
            If txtDAQNumberofReadings.Text = "" Then
                e.Cancel = False
                ErrorProvider.SetError(txtDAQNumberofReadings, "")
            Else
                e.Cancel = True
                txtDAQNumberofReadings.Select(0, txtDAQNumberofReadings.Text.Length)
                ErrorProvider.SetError(txtDAQNumberofReadings, "The value must be an integer.")
            End If
        End If
    End Sub

    Private Sub txtDAQCompareThreshold_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
        Dim text As TextBox = CType(sender, TextBox)
        text.Text = text.Text.Replace(" ", "")
        If IsNumeric(text.Text) Then
            If CInt(Val(text.Text)) <= 0 Then
                e.Cancel = True
                text.Select(0, text.Text.Length)
                ErrorProvider.SetError(text, "The value should be greater than 0.")
            Else
                ErrorProvider.SetError(text, "")
            End If
        Else
            If text.Text = "" Then
                e.Cancel = False
                ErrorProvider.SetError(text, "")
            Else
                e.Cancel = True
                text.Select(0, text.Text.Length)
                ErrorProvider.SetError(text, "The value is invalid.")
            End If

        End If
    End Sub

    ''' <summary>
    ''' Handle validating event of gain and offset textboxes
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Yang Cheng, Landis+Gyr 2016</remarks>
    Private Sub txtDAQOffsetGain_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
        Dim text As TextBox = CType(sender, TextBox)
        text.Text = text.Text.Replace(" ", "") ' Remove any blank space
        If IsNumeric(text.Text) Then
            ErrorProvider.SetError(text, "")
        Else
            If text.Text = "" Then
                e.Cancel = False
                ErrorProvider.SetError(text, "")
            Else
                e.Cancel = True
                text.Select(0, text.Text.Length)
                ErrorProvider.SetError(text, "The value is invalid.")
            End If
        End If
    End Sub



    Public Sub PacPowerReceiver(ByVal Message As String) Handles MyPacPower.BytesReceived
        If Message.IndexOf(vbCrLf) >= 0 Then
            ' addItem("CRLF @" + Message.IndexOf(vbCrLf).ToString, lbPacPowerLog)
            addItem(Message, lbPacPowerLog)
        End If

    End Sub

    Private Sub btnPacPowerConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPacPowerConnect.Click
        If MyPacPower.SourceCommand Is Nothing Then
            MyPacPower.Init()
        End If

        If btnPacPowerConnect.Text = "Connect" Then
            Try
                MyPacPower.i_InitSerialPort(CInt(Val(txtPacPowerBaudrate.Text.Trim)), IO.Ports.Parity.None, IO.Ports.StopBits.One, 8, "COM" + cbPacPowerPorts.Text.Trim, False, IO.Ports.Handshake.None)
            Catch ex As Exception
                MsgBox(ex.ToString)
                MyPacPower.IsConnected = False
                Exit Sub
            End Try

            Try
                MyPacPower.i_Connect()
                btnPacPowerConnect.Text = "Disconnect"
            Catch ex As Exception
                MsgBox(ex.ToString)
                MyPacPower.IsConnected = False
                Exit Sub
            End Try
            Dim Message As String = "" 'txtPacpowerTest.Text.Trim '+ vbCrLf  ' = "*IDN?" + vbCrLf

            MyPacPower.i_Transmit(txtPacpowerTest.Text.Trim, Message)
            MyPacPower.IsConnected = True

            Dim Status As Integer
            Dim Command As String = MyPacPower.SourceCommand.SetVoltage1("0") + ";" + MyPacPower.SourceCommand.SetOutputState( _
                                       Current_Temperature.mPacPowerUCP1.cPacPowerUPC1.cCommands.TOGGLE_STATE._OFF)
            MyPacPower._VoltageASetPoint = "0".Trim
            Status = MyPacPower.i_Transmit(Command, Message)
            'once connected disable switching between the voltage sources...
            rbUsePacificPower.Enabled = False
            rbUseCaliforniaInstruments.Enabled = False
        Else
            Try
                MyPacPower.i_Disconnect()
                btnPacPowerConnect.Text = "Connect"
                MyPacPower.IsConnected = False
                'once disconnected enable switching between the voltage sources...
                rbUsePacificPower.Enabled = True
                rbUseCaliforniaInstruments.Enabled = True
            Catch ex As Exception
                MsgBox(ex.ToString)
                Exit Sub

            End Try
        End If


    End Sub


    Private Sub btnPacPowerSetVoltage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPacPowerSetVoltageRMS.Click
        If MyPacPower.SourceCommand Is Nothing Then
            MyPacPower.Init()
        End If

        If MyPacPower.IsConnected Then

            Dim Message As String = ""
            Dim Status As Integer = 0
            If rbPacPowerSetVoltageRMSAll.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetVoltageAll(txtPacPowerSetVoltageRMS.Text.Trim), Message)
            ElseIf rbPacPowerSetVoltageRMSA.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetVoltage1(txtPacPowerSetVoltageRMS.Text.Trim), Message)
            ElseIf rbPacPowerSetVoltageRMSB.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetVoltage1(txtPacPowerSetVoltageRMS.Text.Trim), Message)
            ElseIf rbPacPowerSetVoltageRMSC.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetVoltage1(txtPacPowerSetVoltageRMS.Text.Trim), Message)
            End If

            If Status > 0 Then
                MsgBox(Message)
                'Else
                '    'Check if complete
                '    Status = MyPacPower.i_Transmit("*OPC?", Message)

                '    If Status > 0 Then
                '        MsgBox(Message)
                '    End If

            End If

        Else
            MsgBox("Power Source is not connected")
        End If
    End Sub


    Private Sub btnPacPowerGetVoltage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPacPowerGetVoltageRMS.Click
        If MyPacPower.MeasureCommands Is Nothing Then
            MyPacPower.Init()
        End If

        If MyPacPower.IsConnected Then

            Dim Message As String = ""
            Dim Status As Integer = 0
            If rbPacPowerSetVoltageRMSAll.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVoltageAll, Message)

            ElseIf rbPacPowerSetVoltageRMSA.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVoltageA, Message)
            ElseIf rbPacPowerSetVoltageRMSB.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVoltageB, Message)
            ElseIf rbPacPowerSetVoltageRMSC.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVoltageC, Message)
            End If

            If Status > 0 Then
                MsgBox(Message)
            End If
        Else
            MsgBox("Power Source is not connected")
        End If
    End Sub

    Private Sub btnPacPowerSetFrequency_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPacPowerSetFrequency.Click
        If MyPacPower.MeasureCommands Is Nothing Then
            MyPacPower.Init()
        End If

        Dim Message As String = ""
        Dim Status As Integer = 0
        If MyPacPower.IsConnected Then
            Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetFrequency(txtPacPowerSetFrequency.Text.Trim), Message)
        Else
            MsgBox("Power Source is not connected")
        End If

        If Status > 0 Then
            MsgBox(Message)
        End If
    End Sub

   

    Private Sub btnPacPowerGetVoltageLineToLine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPacPowerGetVoltageLineToLine.Click
        If MyPacPower.MeasureCommands Is Nothing Then
            MyPacPower.Init()
        End If

        If MyPacPower.IsConnected Then

            Dim Message As String = ""
            Dim Status As Integer = 0
            If rbPacPowerSetVoltageLineToLineAll.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVLL_ALL, Message)

            ElseIf rbPacPowerSetVoltageLineToLineA.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVLLA, Message)
            ElseIf rbPacPowerSetVoltageLineToLineB.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVLLB, Message)
            ElseIf rbPacPowerSetVoltageLineToLineC.Checked Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.GetVLLC, Message)
            End If

            If Status > 0 Then
                MsgBox(Message)
            End If
        Else
            MsgBox("Power Source is not connected")
        End If
    End Sub

    Private Sub btnPacPowerOutputEnable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPacPowerOutputEnable.Click
        If MyPacPower.MeasureCommands Is Nothing Then
            MyPacPower.Init()
        End If

        If MyPacPower.IsConnected Then

            Dim Message As String = ""
            Dim Status As Integer = 0

            If btnPacPowerOutputEnable.Text = "Enable" Then
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetOutputState(Current_Temperature.mPacPowerUCP1.cPacPowerUPC1.cCommands.TOGGLE_STATE._ON), Message)
                btnPacPowerOutputEnable.Text = "Disable"

            Else
                Status = MyPacPower.i_Transmit(MyPacPower.SourceCommand.SetOutputState(Current_Temperature.mPacPowerUCP1.cPacPowerUPC1.cCommands.TOGGLE_STATE._OFF), Message)
                btnPacPowerOutputEnable.Text = "Enable"
            End If
        Else
            MsgBox("Power Source is not connected")

        End If

    End Sub

    
    Private Sub rbUsePacificPower_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbUsePacificPower.CheckedChanged
        If Not gbPacPowerASX115 Is Nothing And Not GroupBox1 Is Nothing Then
            If rbUsePacificPower.Checked = True Then
                gbPacPowerASX115.Enabled = True
                GroupBox1.Enabled = False
            Else
                gbPacPowerASX115.Enabled = False
                GroupBox1.Enabled = True
            End If
        End If

        
    End Sub

End Class