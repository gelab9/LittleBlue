<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCurrent_Temperature
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend2 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCurrent_Temperature))
        Me.tcCurrent_Temperature = New System.Windows.Forms.TabControl()
        Me.DAQ = New System.Windows.Forms.TabPage()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.rbUseCaliforniaInstruments = New System.Windows.Forms.RadioButton()
        Me.rbUsePacificPower = New System.Windows.Forms.RadioButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.boardIdNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.secondaryAddressComboBox = New System.Windows.Forms.ComboBox()
        Me.secondaryAddressLabel = New System.Windows.Forms.Label()
        Me.primaryAddressLabel = New System.Windows.Forms.Label()
        Me.primaryAddressNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.closeButton = New System.Windows.Forms.Button()
        Me.boardIdLabel = New System.Windows.Forms.Label()
        Me.openButton = New System.Windows.Forms.Button()
        Me.gbPacPowerASX115 = New System.Windows.Forms.GroupBox()
        Me.cbPacPowerPorts = New System.Windows.Forms.ComboBox()
        Me.lblPacPowerComPort = New System.Windows.Forms.Label()
        Me.lblPacPowerBaudRate = New System.Windows.Forms.Label()
        Me.txtPacPowerBaudrate = New System.Windows.Forms.TextBox()
        Me.btnPacPowerConnect = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.gbDAQConnection = New System.Windows.Forms.GroupBox()
        Me.txtDAQConnectionBaudrate = New System.Windows.Forms.ComboBox()
        Me.cbDAQConnectionComPort = New System.Windows.Forms.ComboBox()
        Me.lblDAQConnectionComPort = New System.Windows.Forms.Label()
        Me.lblDAQConnectionBaudRate = New System.Windows.Forms.Label()
        Me.btnDAQConnectionConnect = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.chbDAQCalibration = New System.Windows.Forms.CheckBox()
        Me.rbSlot300 = New System.Windows.Forms.RadioButton()
        Me.rbSlot200 = New System.Windows.Forms.RadioButton()
        Me.rbSlot100 = New System.Windows.Forms.RadioButton()
        Me.panelModule1 = New System.Windows.Forms.Panel()
        Me.txtDAQGain118 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain119 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain120 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain117 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain116 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain115 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain114 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain113 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain112 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain111 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain109 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain110 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain108 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain107 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain106 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain105 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain104 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain103 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain102 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain101 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset118 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset119 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset120 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset117 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset116 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset115 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset114 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset113 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset112 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset111 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset109 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset110 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset108 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset107 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset106 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset105 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset104 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset103 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset102 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset101 = New System.Windows.Forms.TextBox()
        Me.cbDAQDataChannel112 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel110 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel120 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel109 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel111 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel108 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel120 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel107 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel111 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel106 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel119 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel105 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel112 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel104 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel119 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel103 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel113 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel102 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel101 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel118 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel113 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel118 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel114 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel117 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel114 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel117 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel115 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel116 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel115 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel116 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel101 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel102 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel103 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel104 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel105 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel106 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel107 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel108 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel109 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel110 = New System.Windows.Forms.CheckBox()
        Me.panelModule2 = New System.Windows.Forms.Panel()
        Me.txtDAQGain218 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain219 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain220 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain217 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain216 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain215 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain214 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain213 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain212 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain211 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain209 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain210 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain208 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain207 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain206 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain205 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain204 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain203 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain202 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain201 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset218 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset219 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset220 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset217 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset216 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset215 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset214 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset213 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset212 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset211 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset209 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset210 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset208 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset207 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset206 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset205 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset204 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset203 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset202 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset201 = New System.Windows.Forms.TextBox()
        Me.cbDAQDataChannel212 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel210 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel220 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel209 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel211 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel208 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel220 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel207 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel211 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel206 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel219 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel205 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel212 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel204 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel219 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel203 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel213 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel202 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel201 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel218 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel213 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel218 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel214 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel217 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel214 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel217 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel215 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel216 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel215 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel216 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel201 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel202 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel203 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel204 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel205 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel206 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel207 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel208 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel209 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel210 = New System.Windows.Forms.CheckBox()
        Me.panelModule3 = New System.Windows.Forms.Panel()
        Me.txtDAQGain318 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain319 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain320 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain317 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain316 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain315 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain314 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain313 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain312 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain311 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain309 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain310 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain308 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain307 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain306 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain305 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain304 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain303 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain302 = New System.Windows.Forms.TextBox()
        Me.txtDAQGain301 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset318 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset319 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset320 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset317 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset316 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset315 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset314 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset313 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset312 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset311 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset309 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset310 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset308 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset307 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset306 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset305 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset304 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset303 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset302 = New System.Windows.Forms.TextBox()
        Me.txtDAQOffset301 = New System.Windows.Forms.TextBox()
        Me.cbDAQDataChannel312 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel310 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel320 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel309 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel311 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel308 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel320 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel307 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel311 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel306 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel319 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel305 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel312 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel304 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel319 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel303 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel313 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel302 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel301 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel318 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel313 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel318 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel314 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel317 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel314 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel317 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel315 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataChannel316 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataChannel315 = New System.Windows.Forms.ComboBox()
        Me.cbDAQDataEnableChannel316 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel301 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel302 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel303 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel304 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel305 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel306 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel307 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel308 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel309 = New System.Windows.Forms.CheckBox()
        Me.cbDAQDataEnableChannel310 = New System.Windows.Forms.CheckBox()
        Me.lblTime = New System.Windows.Forms.Label()
        Me.chbAutomaticShutDown = New System.Windows.Forms.CheckBox()
        Me.gbControlSettings = New System.Windows.Forms.GroupBox()
        Me.lblAccuracyDeadBand = New System.Windows.Forms.Label()
        Me.txtVoltage_ControlDeadBand = New System.Windows.Forms.TextBox()
        Me.lblControlDeadBand = New System.Windows.Forms.Label()
        Me.cbVoltage_CurrentSetpoint = New System.Windows.Forms.ComboBox()
        Me.chkCloseLoop = New System.Windows.Forms.CheckBox()
        Me.txtVoltage_AccuracyDeadband = New System.Windows.Forms.TextBox()
        Me.gbDAQVoltage = New System.Windows.Forms.GroupBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnVoltageOff = New System.Windows.Forms.Button()
        Me.btnSetVoltage = New System.Windows.Forms.Button()
        Me.txtSetVoltage = New System.Windows.Forms.TextBox()
        Me.btnSaveConfig = New System.Windows.Forms.Button()
        Me.btnLoadConfig = New System.Windows.Forms.Button()
        Me.gbRadianRS232 = New System.Windows.Forms.GroupBox()
        Me.cbMeter_COMPorts = New System.Windows.Forms.ComboBox()
        Me.lblMeterComPort = New System.Windows.Forms.Label()
        Me.lblMeterComBaudRate = New System.Windows.Forms.Label()
        Me.txtMeterComBaudrate = New System.Windows.Forms.TextBox()
        Me.btnConnect = New System.Windows.Forms.Button()
        Me.gbDAQNumberofReadings = New System.Windows.Forms.GroupBox()
        Me.lblDAQTotally = New System.Windows.Forms.Label()
        Me.lblDAQReadings = New System.Windows.Forms.Label()
        Me.txtDAQNumberofReadings = New System.Windows.Forms.TextBox()
        Me.txtMachineState = New System.Windows.Forms.Label()
        Me.gbCountDown = New System.Windows.Forms.GroupBox()
        Me.txtDAQCountDownHH = New System.Windows.Forms.TextBox()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.txtDAQCountDownMM = New System.Windows.Forms.TextBox()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.txtDAQCountDownSS = New System.Windows.Forms.TextBox()
        Me.Label74 = New System.Windows.Forms.Label()
        Me.gbDAQDataGrid = New System.Windows.Forms.GroupBox()
        Me.txtDataLoggerHeader = New System.Windows.Forms.TextBox()
        Me.chkDataLoggerPhase = New System.Windows.Forms.CheckBox()
        Me.chkDataLoggerFrequency = New System.Windows.Forms.CheckBox()
        Me.chkDataLoggerCurrent = New System.Windows.Forms.CheckBox()
        Me.chkDataLoggerVolts = New System.Windows.Forms.CheckBox()
        Me.lbDataLog = New System.Windows.Forms.ListBox()
        Me.dgvDAQData = New System.Windows.Forms.DataGridView()
        Me.currentReading = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.maxReading = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.minReading = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.averageReading = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.tempRisen = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnThreshold = New System.Windows.Forms.Panel()
        Me.txtDAQCompareThreshold5 = New System.Windows.Forms.TextBox()
        Me.txtDAQCompareThreshold4 = New System.Windows.Forms.TextBox()
        Me.cbDAQCompare1_Check = New System.Windows.Forms.CheckBox()
        Me.txtDAQCompareThreshold3 = New System.Windows.Forms.TextBox()
        Me.cbDAQCompare2_Check = New System.Windows.Forms.CheckBox()
        Me.txtDAQCompareThreshold2 = New System.Windows.Forms.TextBox()
        Me.cbDAQCompare3_Check = New System.Windows.Forms.CheckBox()
        Me.txtDAQCompareThreshold1 = New System.Windows.Forms.TextBox()
        Me.cbDAQCompare4_Check = New System.Windows.Forms.CheckBox()
        Me.cbDAQCompare5_Check = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cbDAQEmailThreshold = New System.Windows.Forms.CheckBox()
        Me.cbDAQEmailTestDone = New System.Windows.Forms.CheckBox()
        Me.chbDAQEmailAddress = New System.Windows.Forms.ComboBox()
        Me.cbDAQEmailNotification = New System.Windows.Forms.CheckBox()
        Me.btnDAQEmailView = New System.Windows.Forms.Button()
        Me.btnDAQEmailAdd = New System.Windows.Forms.Button()
        Me.lblDAQEmailExt = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare10 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare8 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare6 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare4 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare9 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare7 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare5 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare3 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare2 = New System.Windows.Forms.Label()
        Me.lblDAQCompareCompare1 = New System.Windows.Forms.Label()
        Me.txtDAQCompare5 = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtDAQCompare4 = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtDAQCompare3 = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtDAQCompare2 = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.cbDAQCompare10 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare9 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare8 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare6 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare7 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare4 = New System.Windows.Forms.ComboBox()
        Me.txtDAQCompare1 = New System.Windows.Forms.TextBox()
        Me.cbDAQCompare2 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare5 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare1 = New System.Windows.Forms.ComboBox()
        Me.cbDAQCompare3 = New System.Windows.Forms.ComboBox()
        Me.gbMode = New System.Windows.Forms.GroupBox()
        Me.rbNumofReadings = New System.Windows.Forms.RadioButton()
        Me.rbDuration = New System.Windows.Forms.RadioButton()
        Me.rbFree = New System.Windows.Forms.RadioButton()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.btnDAQStartStopReading = New System.Windows.Forms.Button()
        Me.gbInformation = New System.Windows.Forms.GroupBox()
        Me.lbDAQInformationNumofReadings = New System.Windows.Forms.Label()
        Me.lbDAQInformationDuration = New System.Windows.Forms.Label()
        Me.lbDAQInformationStartTime = New System.Windows.Forms.Label()
        Me.lbDAQInformationTerminateTime = New System.Windows.Forms.Label()
        Me.gbParameters = New System.Windows.Forms.GroupBox()
        Me.rbDAQParametersThermocouple = New System.Windows.Forms.RadioButton()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.rbDAQParametersFRTD = New System.Windows.Forms.RadioButton()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbDAQParametersReadingIntervals = New System.Windows.Forms.ComboBox()
        Me.Plot = New System.Windows.Forms.TabPage()
        Me.CurrentChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.rbPlotCurrent = New System.Windows.Forms.RadioButton()
        Me.rbPlotTemperature = New System.Windows.Forms.RadioButton()
        Me.DataChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.gbPlotLineWidth = New System.Windows.Forms.GroupBox()
        Me.btnPlotSetLineWidth = New System.Windows.Forms.Button()
        Me.cbPlotLineWidth = New System.Windows.Forms.ComboBox()
        Me.lblPlotReminder = New System.Windows.Forms.Label()
        Me.btnPlotLoadPlot = New System.Windows.Forms.Button()
        Me.btnPlotSavePlot = New System.Windows.Forms.Button()
        Me.btnPlotResetXY = New System.Windows.Forms.Button()
        Me.btnPlotResetY = New System.Windows.Forms.Button()
        Me.btnPlotResetX = New System.Windows.Forms.Button()
        Me.Log = New System.Windows.Forms.TabPage()
        Me.gbDAQIdentification = New System.Windows.Forms.GroupBox()
        Me.lbDAQIdentification = New System.Windows.Forms.ListBox()
        Me.btnDAQIdentificationIdentity = New System.Windows.Forms.Button()
        Me.lbDisplayInstantMetrics = New System.Windows.Forms.ListBox()
        Me.chkErrorLogVerbose = New System.Windows.Forms.CheckBox()
        Me.lstDAQLog = New System.Windows.Forms.ListBox()
        Me.lstDAQError = New System.Windows.Forms.ListBox()
        Me.CurrentController = New System.Windows.Forms.TabPage()
        Me.gbDataLogger = New System.Windows.Forms.GroupBox()
        Me.lblDataLoggerSampleInterval = New System.Windows.Forms.Label()
        Me.lblDataLoggerSeconds = New System.Windows.Forms.Label()
        Me.txtDataLoggerInterval = New System.Windows.Forms.TextBox()
        Me.btnDataLoggerToggleLogger = New System.Windows.Forms.Button()
        Me.btnTest = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.btnSaveGPIB_LOG = New System.Windows.Forms.Button()
        Me.btnCI501TCA = New System.Windows.Forms.Button()
        Me.gbSaveData = New System.Windows.Forms.GroupBox()
        Me.btnRadianLoggerSaveData = New System.Windows.Forms.Button()
        Me.lblDataSaveDelimiter = New System.Windows.Forms.Label()
        Me.rbDataSaveComma = New System.Windows.Forms.RadioButton()
        Me.rbDataSaveSpace = New System.Windows.Forms.RadioButton()
        Me.rbDataSaveTab = New System.Windows.Forms.RadioButton()
        Me.rbDataSaveSemiColon = New System.Windows.Forms.RadioButton()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtLogDataNotes = New System.Windows.Forms.TextBox()
        Me.chkCheckToSaveData = New System.Windows.Forms.CheckBox()
        Me.chkMeterTestSaveDataOverWrite = New System.Windows.Forms.CheckBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.btnDataSaveLocation = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtFolderSave = New System.Windows.Forms.TextBox()
        Me.txtDataFileName = New System.Windows.Forms.TextBox()
        Me.lbGPIB_Log = New System.Windows.Forms.ListBox()
        Me.gbMetrics = New System.Windows.Forms.GroupBox()
        Me.cbSelectInstantMetrics = New System.Windows.Forms.ComboBox()
        Me.btnGetInstantMetrics = New System.Windows.Forms.Button()
        Me.chkGPIBVerbose = New System.Windows.Forms.CheckBox()
        Me.btnGPIB_LogClear = New System.Windows.Forms.Button()
        Me.ErrorLog = New System.Windows.Forms.TabPage()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.chkVerbose = New System.Windows.Forms.CheckBox()
        Me.btnClearLog = New System.Windows.Forms.Button()
        Me.btnSaveLog = New System.Windows.Forms.Button()
        Me.lblLogData = New System.Windows.Forms.ListBox()
        Me.tbRadian = New System.Windows.Forms.TabPage()
        Me.gbIdentification = New System.Windows.Forms.GroupBox()
        Me.lbIdentification = New System.Windows.Forms.ListBox()
        Me.btnIdentify = New System.Windows.Forms.Button()
        Me.lbDisplayAccumMetrics = New System.Windows.Forms.ListBox()
        Me.btnGetAccumMetrics = New System.Windows.Forms.Button()
        Me.cbSelectAccumMetrics = New System.Windows.Forms.ComboBox()
        Me.gbCurrentRange = New System.Windows.Forms.GroupBox()
        Me.btnRangingUnlockCurrent = New System.Windows.Forms.Button()
        Me.chkRangingSetAndLockCurrent = New System.Windows.Forms.CheckBox()
        Me.btnRangingLockCurrentRangeToggle = New System.Windows.Forms.Button()
        Me.lblRangingCurrentRangeSetPoint = New System.Windows.Forms.Label()
        Me.btnRangingGetCurrentRange = New System.Windows.Forms.Button()
        Me.cbRangingCurrent = New System.Windows.Forms.ComboBox()
        Me.gbReset = New System.Windows.Forms.GroupBox()
        Me.chkResetAccumData = New System.Windows.Forms.CheckBox()
        Me.chkResetMaxData = New System.Windows.Forms.CheckBox()
        Me.chkResetMinData = New System.Windows.Forms.CheckBox()
        Me.chkResetInstantaneousData = New System.Windows.Forms.CheckBox()
        Me.chkResetWaveformBufferToZero = New System.Windows.Forms.CheckBox()
        Me.btnReset = New System.Windows.Forms.Button()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.txtPacpowerTest = New System.Windows.Forms.TextBox()
        Me.gbPacPowerOutput = New System.Windows.Forms.GroupBox()
        Me.btnPacPowerOutputEnable = New System.Windows.Forms.Button()
        Me.gbPacPowerVoltageLineToLine = New System.Windows.Forms.GroupBox()
        Me.btnPacPowerGetVoltageLineToLine = New System.Windows.Forms.Button()
        Me.rbPacPowerSetVoltageLineToLineB = New System.Windows.Forms.RadioButton()
        Me.rbPacPowerSetVoltageLineToLineC = New System.Windows.Forms.RadioButton()
        Me.rbPacPowerSetVoltageLineToLineAll = New System.Windows.Forms.RadioButton()
        Me.rbPacPowerSetVoltageLineToLineA = New System.Windows.Forms.RadioButton()
        Me.lbPacPowerLog = New System.Windows.Forms.ListBox()
        Me.gbPacPowerFrequency = New System.Windows.Forms.GroupBox()
        Me.txtPacPowerGetFrequency = New System.Windows.Forms.TextBox()
        Me.btnPacPowerGetFrequency = New System.Windows.Forms.Button()
        Me.txtPacPowerSetFrequency = New System.Windows.Forms.TextBox()
        Me.btnPacPowerSetFrequency = New System.Windows.Forms.Button()
        Me.gbPacPowerVoltageRMS = New System.Windows.Forms.GroupBox()
        Me.btnPacPowerGetVoltageRMS = New System.Windows.Forms.Button()
        Me.txtPacPowerSetVoltageRMS = New System.Windows.Forms.TextBox()
        Me.rbPacPowerSetVoltageRMSB = New System.Windows.Forms.RadioButton()
        Me.rbPacPowerSetVoltageRMSC = New System.Windows.Forms.RadioButton()
        Me.rbPacPowerSetVoltageRMSAll = New System.Windows.Forms.RadioButton()
        Me.rbPacPowerSetVoltageRMSA = New System.Windows.Forms.RadioButton()
        Me.btnPacPowerSetVoltageRMS = New System.Windows.Forms.Button()
        Me.TimerRunning = New System.Windows.Forms.Timer(Me.components)
        Me.TimerProgram = New System.Windows.Forms.Timer(Me.components)
        Me.ErrDataLogger = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.TimerSample = New System.Windows.Forms.Timer(Me.components)
        Me.TimerRadian = New System.Windows.Forms.Timer(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ErrorProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.tcCurrent_Temperature.SuspendLayout()
        Me.DAQ.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.boardIdNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.primaryAddressNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbPacPowerASX115.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.gbDAQConnection.SuspendLayout()
        Me.panelModule1.SuspendLayout()
        Me.panelModule2.SuspendLayout()
        Me.panelModule3.SuspendLayout()
        Me.gbControlSettings.SuspendLayout()
        Me.gbDAQVoltage.SuspendLayout()
        Me.gbRadianRS232.SuspendLayout()
        Me.gbDAQNumberofReadings.SuspendLayout()
        Me.gbCountDown.SuspendLayout()
        Me.gbDAQDataGrid.SuspendLayout()
        CType(Me.dgvDAQData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnThreshold.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.gbMode.SuspendLayout()
        Me.gbInformation.SuspendLayout()
        Me.gbParameters.SuspendLayout()
        Me.Plot.SuspendLayout()
        CType(Me.CurrentChart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataChart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbPlotLineWidth.SuspendLayout()
        Me.Log.SuspendLayout()
        Me.gbDAQIdentification.SuspendLayout()
        Me.CurrentController.SuspendLayout()
        Me.gbDataLogger.SuspendLayout()
        Me.gbSaveData.SuspendLayout()
        Me.gbMetrics.SuspendLayout()
        Me.ErrorLog.SuspendLayout()
        Me.tbRadian.SuspendLayout()
        Me.gbIdentification.SuspendLayout()
        Me.gbCurrentRange.SuspendLayout()
        Me.gbReset.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.gbPacPowerOutput.SuspendLayout()
        Me.gbPacPowerVoltageLineToLine.SuspendLayout()
        Me.gbPacPowerFrequency.SuspendLayout()
        Me.gbPacPowerVoltageRMS.SuspendLayout()
        CType(Me.ErrDataLogger, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tcCurrent_Temperature
        '
        Me.tcCurrent_Temperature.Controls.Add(Me.DAQ)
        Me.tcCurrent_Temperature.Controls.Add(Me.Plot)
        Me.tcCurrent_Temperature.Controls.Add(Me.Log)
        Me.tcCurrent_Temperature.Controls.Add(Me.CurrentController)
        Me.tcCurrent_Temperature.Controls.Add(Me.ErrorLog)
        Me.tcCurrent_Temperature.Controls.Add(Me.tbRadian)
        Me.tcCurrent_Temperature.Controls.Add(Me.TabPage1)
        Me.tcCurrent_Temperature.Location = New System.Drawing.Point(0, 0)
        Me.tcCurrent_Temperature.Multiline = True
        Me.tcCurrent_Temperature.Name = "tcCurrent_Temperature"
        Me.tcCurrent_Temperature.SelectedIndex = 0
        Me.tcCurrent_Temperature.Size = New System.Drawing.Size(1282, 719)
        Me.tcCurrent_Temperature.TabIndex = 1
        '
        'DAQ
        '
        Me.DAQ.Controls.Add(Me.Panel2)
        Me.DAQ.Controls.Add(Me.Panel1)
        Me.DAQ.Controls.Add(Me.lblTime)
        Me.DAQ.Controls.Add(Me.chbAutomaticShutDown)
        Me.DAQ.Controls.Add(Me.gbControlSettings)
        Me.DAQ.Controls.Add(Me.gbDAQVoltage)
        Me.DAQ.Controls.Add(Me.btnSaveConfig)
        Me.DAQ.Controls.Add(Me.btnLoadConfig)
        Me.DAQ.Controls.Add(Me.gbRadianRS232)
        Me.DAQ.Controls.Add(Me.gbDAQNumberofReadings)
        Me.DAQ.Controls.Add(Me.txtMachineState)
        Me.DAQ.Controls.Add(Me.gbCountDown)
        Me.DAQ.Controls.Add(Me.gbDAQDataGrid)
        Me.DAQ.Controls.Add(Me.pnThreshold)
        Me.DAQ.Controls.Add(Me.GroupBox3)
        Me.DAQ.Controls.Add(Me.GroupBox2)
        Me.DAQ.Controls.Add(Me.gbMode)
        Me.DAQ.Controls.Add(Me.ProgressBar1)
        Me.DAQ.Controls.Add(Me.btnDAQStartStopReading)
        Me.DAQ.Controls.Add(Me.gbInformation)
        Me.DAQ.Controls.Add(Me.gbParameters)
        Me.DAQ.Location = New System.Drawing.Point(4, 22)
        Me.DAQ.Name = "DAQ"
        Me.DAQ.Padding = New System.Windows.Forms.Padding(3)
        Me.DAQ.Size = New System.Drawing.Size(1274, 693)
        Me.DAQ.TabIndex = 0
        Me.DAQ.Text = "DAQ"
        Me.DAQ.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.rbUseCaliforniaInstruments)
        Me.Panel2.Controls.Add(Me.rbUsePacificPower)
        Me.Panel2.Controls.Add(Me.GroupBox1)
        Me.Panel2.Controls.Add(Me.gbPacPowerASX115)
        Me.Panel2.Location = New System.Drawing.Point(3, 486)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(284, 201)
        Me.Panel2.TabIndex = 345
        '
        'rbUseCaliforniaInstruments
        '
        Me.rbUseCaliforniaInstruments.AutoSize = True
        Me.rbUseCaliforniaInstruments.Location = New System.Drawing.Point(104, 11)
        Me.rbUseCaliforniaInstruments.Name = "rbUseCaliforniaInstruments"
        Me.rbUseCaliforniaInstruments.Size = New System.Drawing.Size(125, 17)
        Me.rbUseCaliforniaInstruments.TabIndex = 328
        Me.rbUseCaliforniaInstruments.Text = "California Instruments"
        Me.rbUseCaliforniaInstruments.UseVisualStyleBackColor = True
        '
        'rbUsePacificPower
        '
        Me.rbUsePacificPower.AutoSize = True
        Me.rbUsePacificPower.Checked = True
        Me.rbUsePacificPower.Location = New System.Drawing.Point(8, 11)
        Me.rbUsePacificPower.Name = "rbUsePacificPower"
        Me.rbUsePacificPower.Size = New System.Drawing.Size(90, 17)
        Me.rbUsePacificPower.TabIndex = 327
        Me.rbUsePacificPower.TabStop = True
        Me.rbUsePacificPower.Text = "Pacific Power"
        Me.rbUsePacificPower.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.boardIdNumericUpDown)
        Me.GroupBox1.Controls.Add(Me.secondaryAddressComboBox)
        Me.GroupBox1.Controls.Add(Me.secondaryAddressLabel)
        Me.GroupBox1.Controls.Add(Me.primaryAddressLabel)
        Me.GroupBox1.Controls.Add(Me.primaryAddressNumericUpDown)
        Me.GroupBox1.Controls.Add(Me.closeButton)
        Me.GroupBox1.Controls.Add(Me.boardIdLabel)
        Me.GroupBox1.Controls.Add(Me.openButton)
        Me.GroupBox1.Location = New System.Drawing.Point(5, 102)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(218, 96)
        Me.GroupBox1.TabIndex = 323
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Cal Inst Connection"
        '
        'boardIdNumericUpDown
        '
        Me.boardIdNumericUpDown.Location = New System.Drawing.Point(101, 19)
        Me.boardIdNumericUpDown.Name = "boardIdNumericUpDown"
        Me.boardIdNumericUpDown.Size = New System.Drawing.Size(60, 20)
        Me.boardIdNumericUpDown.TabIndex = 26
        '
        'secondaryAddressComboBox
        '
        Me.secondaryAddressComboBox.Location = New System.Drawing.Point(101, 67)
        Me.secondaryAddressComboBox.Name = "secondaryAddressComboBox"
        Me.secondaryAddressComboBox.Size = New System.Drawing.Size(60, 21)
        Me.secondaryAddressComboBox.TabIndex = 31
        '
        'secondaryAddressLabel
        '
        Me.secondaryAddressLabel.AutoSize = True
        Me.secondaryAddressLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.secondaryAddressLabel.Location = New System.Drawing.Point(2, 70)
        Me.secondaryAddressLabel.Name = "secondaryAddressLabel"
        Me.secondaryAddressLabel.Size = New System.Drawing.Size(102, 13)
        Me.secondaryAddressLabel.TabIndex = 30
        Me.secondaryAddressLabel.Text = "Secondary Address:"
        '
        'primaryAddressLabel
        '
        Me.primaryAddressLabel.AutoSize = True
        Me.primaryAddressLabel.Location = New System.Drawing.Point(14, 46)
        Me.primaryAddressLabel.Name = "primaryAddressLabel"
        Me.primaryAddressLabel.Size = New System.Drawing.Size(85, 13)
        Me.primaryAddressLabel.TabIndex = 29
        Me.primaryAddressLabel.Text = "Primary Address:"
        '
        'primaryAddressNumericUpDown
        '
        Me.primaryAddressNumericUpDown.Location = New System.Drawing.Point(101, 43)
        Me.primaryAddressNumericUpDown.Name = "primaryAddressNumericUpDown"
        Me.primaryAddressNumericUpDown.Size = New System.Drawing.Size(60, 20)
        Me.primaryAddressNumericUpDown.TabIndex = 27
        Me.primaryAddressNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'closeButton
        '
        Me.closeButton.Enabled = False
        Me.closeButton.Location = New System.Drawing.Point(167, 44)
        Me.closeButton.Name = "closeButton"
        Me.closeButton.Size = New System.Drawing.Size(46, 23)
        Me.closeButton.TabIndex = 19
        Me.closeButton.Text = "&Close"
        '
        'boardIdLabel
        '
        Me.boardIdLabel.AutoSize = True
        Me.boardIdLabel.Location = New System.Drawing.Point(50, 22)
        Me.boardIdLabel.Name = "boardIdLabel"
        Me.boardIdLabel.Size = New System.Drawing.Size(52, 13)
        Me.boardIdLabel.TabIndex = 28
        Me.boardIdLabel.Text = "Board ID:"
        '
        'openButton
        '
        Me.openButton.Location = New System.Drawing.Point(167, 18)
        Me.openButton.Name = "openButton"
        Me.openButton.Size = New System.Drawing.Size(46, 23)
        Me.openButton.TabIndex = 18
        Me.openButton.Text = "&Open"
        '
        'gbPacPowerASX115
        '
        Me.gbPacPowerASX115.Controls.Add(Me.cbPacPowerPorts)
        Me.gbPacPowerASX115.Controls.Add(Me.lblPacPowerComPort)
        Me.gbPacPowerASX115.Controls.Add(Me.lblPacPowerBaudRate)
        Me.gbPacPowerASX115.Controls.Add(Me.txtPacPowerBaudrate)
        Me.gbPacPowerASX115.Controls.Add(Me.btnPacPowerConnect)
        Me.gbPacPowerASX115.Location = New System.Drawing.Point(5, 34)
        Me.gbPacPowerASX115.Name = "gbPacPowerASX115"
        Me.gbPacPowerASX115.Size = New System.Drawing.Size(211, 65)
        Me.gbPacPowerASX115.TabIndex = 326
        Me.gbPacPowerASX115.TabStop = False
        Me.gbPacPowerASX115.Text = "PAC Power ASX115"
        '
        'cbPacPowerPorts
        '
        Me.cbPacPowerPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPacPowerPorts.FormattingEnabled = True
        Me.cbPacPowerPorts.Items.AddRange(New Object() {"1"})
        Me.cbPacPowerPorts.Location = New System.Drawing.Point(65, 34)
        Me.cbPacPowerPorts.Name = "cbPacPowerPorts"
        Me.cbPacPowerPorts.Size = New System.Drawing.Size(45, 21)
        Me.cbPacPowerPorts.TabIndex = 63
        '
        'lblPacPowerComPort
        '
        Me.lblPacPowerComPort.Location = New System.Drawing.Point(62, 16)
        Me.lblPacPowerComPort.Name = "lblPacPowerComPort"
        Me.lblPacPowerComPort.Size = New System.Drawing.Size(48, 15)
        Me.lblPacPowerComPort.TabIndex = 62
        Me.lblPacPowerComPort.Text = "Port check"
        '
        'lblPacPowerBaudRate
        '
        Me.lblPacPowerBaudRate.Location = New System.Drawing.Point(6, 16)
        Me.lblPacPowerBaudRate.Name = "lblPacPowerBaudRate"
        Me.lblPacPowerBaudRate.Size = New System.Drawing.Size(65, 14)
        Me.lblPacPowerBaudRate.TabIndex = 60
        Me.lblPacPowerBaudRate.Text = "BaudRate"
        '
        'txtPacPowerBaudrate
        '
        Me.txtPacPowerBaudrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtPacPowerBaudrate.Location = New System.Drawing.Point(9, 34)
        Me.txtPacPowerBaudrate.Name = "txtPacPowerBaudrate"
        Me.txtPacPowerBaudrate.Size = New System.Drawing.Size(49, 20)
        Me.txtPacPowerBaudrate.TabIndex = 61
        Me.txtPacPowerBaudrate.Text = "38400"
        '
        'btnPacPowerConnect
        '
        Me.btnPacPowerConnect.Location = New System.Drawing.Point(116, 33)
        Me.btnPacPowerConnect.Name = "btnPacPowerConnect"
        Me.btnPacPowerConnect.Size = New System.Drawing.Size(91, 24)
        Me.btnPacPowerConnect.TabIndex = 49
        Me.btnPacPowerConnect.Text = "Connect"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.gbDAQConnection)
        Me.Panel1.Controls.Add(Me.Label7)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.chbDAQCalibration)
        Me.Panel1.Controls.Add(Me.rbSlot300)
        Me.Panel1.Controls.Add(Me.rbSlot200)
        Me.Panel1.Controls.Add(Me.rbSlot100)
        Me.Panel1.Controls.Add(Me.panelModule1)
        Me.Panel1.Controls.Add(Me.panelModule2)
        Me.Panel1.Controls.Add(Me.panelModule3)
        Me.Panel1.Location = New System.Drawing.Point(3, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(284, 408)
        Me.Panel1.TabIndex = 343
        '
        'gbDAQConnection
        '
        Me.gbDAQConnection.Controls.Add(Me.txtDAQConnectionBaudrate)
        Me.gbDAQConnection.Controls.Add(Me.cbDAQConnectionComPort)
        Me.gbDAQConnection.Controls.Add(Me.lblDAQConnectionComPort)
        Me.gbDAQConnection.Controls.Add(Me.lblDAQConnectionBaudRate)
        Me.gbDAQConnection.Controls.Add(Me.btnDAQConnectionConnect)
        Me.gbDAQConnection.Location = New System.Drawing.Point(8, 7)
        Me.gbDAQConnection.Name = "gbDAQConnection"
        Me.gbDAQConnection.Size = New System.Drawing.Size(216, 62)
        Me.gbDAQConnection.TabIndex = 325
        Me.gbDAQConnection.TabStop = False
        Me.gbDAQConnection.Text = "34970A Connection"
        '
        'txtDAQConnectionBaudrate
        '
        Me.txtDAQConnectionBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.txtDAQConnectionBaudrate.FormattingEnabled = True
        Me.txtDAQConnectionBaudrate.Items.AddRange(New Object() {"9600", "19200", "38400", "57600", "115200"})
        Me.txtDAQConnectionBaudrate.Location = New System.Drawing.Point(6, 35)
        Me.txtDAQConnectionBaudrate.Name = "txtDAQConnectionBaudrate"
        Me.txtDAQConnectionBaudrate.Size = New System.Drawing.Size(65, 21)
        Me.txtDAQConnectionBaudrate.TabIndex = 293
        '
        'cbDAQConnectionComPort
        '
        Me.cbDAQConnectionComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQConnectionComPort.FormattingEnabled = True
        Me.cbDAQConnectionComPort.Location = New System.Drawing.Point(77, 35)
        Me.cbDAQConnectionComPort.Name = "cbDAQConnectionComPort"
        Me.cbDAQConnectionComPort.Size = New System.Drawing.Size(41, 21)
        Me.cbDAQConnectionComPort.TabIndex = 63
        '
        'lblDAQConnectionComPort
        '
        Me.lblDAQConnectionComPort.Location = New System.Drawing.Point(74, 18)
        Me.lblDAQConnectionComPort.Name = "lblDAQConnectionComPort"
        Me.lblDAQConnectionComPort.Size = New System.Drawing.Size(27, 15)
        Me.lblDAQConnectionComPort.TabIndex = 62
        Me.lblDAQConnectionComPort.Text = "Port check"
        '
        'lblDAQConnectionBaudRate
        '
        Me.lblDAQConnectionBaudRate.Location = New System.Drawing.Point(5, 18)
        Me.lblDAQConnectionBaudRate.Name = "lblDAQConnectionBaudRate"
        Me.lblDAQConnectionBaudRate.Size = New System.Drawing.Size(66, 14)
        Me.lblDAQConnectionBaudRate.TabIndex = 60
        Me.lblDAQConnectionBaudRate.Text = "BaudRate"
        '
        'btnDAQConnectionConnect
        '
        Me.btnDAQConnectionConnect.Location = New System.Drawing.Point(122, 34)
        Me.btnDAQConnectionConnect.Name = "btnDAQConnectionConnect"
        Me.btnDAQConnectionConnect.Size = New System.Drawing.Size(91, 23)
        Me.btnDAQConnectionConnect.TabIndex = 49
        Me.btnDAQConnectionConnect.Text = "Connect"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(210, 120)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(35, 13)
        Me.Label7.TabIndex = 324
        Me.Label7.Text = "Offset"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(153, 120)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(29, 13)
        Me.Label6.TabIndex = 323
        Me.Label6.Text = "Gain"
        '
        'chbDAQCalibration
        '
        Me.chbDAQCalibration.AutoSize = True
        Me.chbDAQCalibration.Location = New System.Drawing.Point(169, 97)
        Me.chbDAQCalibration.Name = "chbDAQCalibration"
        Me.chbDAQCalibration.Size = New System.Drawing.Size(75, 17)
        Me.chbDAQCalibration.TabIndex = 322
        Me.chbDAQCalibration.Text = "Calibration"
        Me.chbDAQCalibration.UseVisualStyleBackColor = True
        '
        'rbSlot300
        '
        Me.rbSlot300.AutoSize = True
        Me.rbSlot300.Enabled = False
        Me.rbSlot300.Location = New System.Drawing.Point(10, 112)
        Me.rbSlot300.Name = "rbSlot300"
        Me.rbSlot300.Size = New System.Drawing.Size(64, 17)
        Me.rbSlot300.TabIndex = 321
        Me.rbSlot300.Text = "Slot 300"
        Me.rbSlot300.UseVisualStyleBackColor = True
        '
        'rbSlot200
        '
        Me.rbSlot200.AutoSize = True
        Me.rbSlot200.Enabled = False
        Me.rbSlot200.Location = New System.Drawing.Point(10, 93)
        Me.rbSlot200.Name = "rbSlot200"
        Me.rbSlot200.Size = New System.Drawing.Size(64, 17)
        Me.rbSlot200.TabIndex = 320
        Me.rbSlot200.Text = "Slot 200"
        Me.rbSlot200.UseVisualStyleBackColor = True
        '
        'rbSlot100
        '
        Me.rbSlot100.AutoSize = True
        Me.rbSlot100.Enabled = False
        Me.rbSlot100.Location = New System.Drawing.Point(10, 75)
        Me.rbSlot100.Name = "rbSlot100"
        Me.rbSlot100.Size = New System.Drawing.Size(64, 17)
        Me.rbSlot100.TabIndex = 319
        Me.rbSlot100.Text = "Slot 100"
        Me.rbSlot100.UseVisualStyleBackColor = True
        '
        'panelModule1
        '
        Me.panelModule1.AutoScroll = True
        Me.panelModule1.BackColor = System.Drawing.Color.Lime
        Me.panelModule1.Controls.Add(Me.txtDAQGain118)
        Me.panelModule1.Controls.Add(Me.txtDAQGain119)
        Me.panelModule1.Controls.Add(Me.txtDAQGain120)
        Me.panelModule1.Controls.Add(Me.txtDAQGain117)
        Me.panelModule1.Controls.Add(Me.txtDAQGain116)
        Me.panelModule1.Controls.Add(Me.txtDAQGain115)
        Me.panelModule1.Controls.Add(Me.txtDAQGain114)
        Me.panelModule1.Controls.Add(Me.txtDAQGain113)
        Me.panelModule1.Controls.Add(Me.txtDAQGain112)
        Me.panelModule1.Controls.Add(Me.txtDAQGain111)
        Me.panelModule1.Controls.Add(Me.txtDAQGain109)
        Me.panelModule1.Controls.Add(Me.txtDAQGain110)
        Me.panelModule1.Controls.Add(Me.txtDAQGain108)
        Me.panelModule1.Controls.Add(Me.txtDAQGain107)
        Me.panelModule1.Controls.Add(Me.txtDAQGain106)
        Me.panelModule1.Controls.Add(Me.txtDAQGain105)
        Me.panelModule1.Controls.Add(Me.txtDAQGain104)
        Me.panelModule1.Controls.Add(Me.txtDAQGain103)
        Me.panelModule1.Controls.Add(Me.txtDAQGain102)
        Me.panelModule1.Controls.Add(Me.txtDAQGain101)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset118)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset119)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset120)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset117)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset116)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset115)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset114)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset113)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset112)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset111)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset109)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset110)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset108)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset107)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset106)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset105)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset104)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset103)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset102)
        Me.panelModule1.Controls.Add(Me.txtDAQOffset101)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel112)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel110)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel120)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel109)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel111)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel108)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel120)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel107)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel111)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel106)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel119)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel105)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel112)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel104)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel119)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel103)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel113)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel102)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel101)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel118)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel113)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel118)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel114)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel117)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel114)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel117)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel115)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel116)
        Me.panelModule1.Controls.Add(Me.cbDAQDataChannel115)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel116)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel101)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel102)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel103)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel104)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel105)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel106)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel107)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel108)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel109)
        Me.panelModule1.Controls.Add(Me.cbDAQDataEnableChannel110)
        Me.panelModule1.Location = New System.Drawing.Point(5, 138)
        Me.panelModule1.Name = "panelModule1"
        Me.panelModule1.Size = New System.Drawing.Size(275, 265)
        Me.panelModule1.TabIndex = 316
        Me.panelModule1.Visible = False
        '
        'txtDAQGain118
        '
        Me.txtDAQGain118.Enabled = False
        Me.txtDAQGain118.Location = New System.Drawing.Point(143, 449)
        Me.txtDAQGain118.Name = "txtDAQGain118"
        Me.txtDAQGain118.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain118.TabIndex = 34
        '
        'txtDAQGain119
        '
        Me.txtDAQGain119.Enabled = False
        Me.txtDAQGain119.Location = New System.Drawing.Point(143, 476)
        Me.txtDAQGain119.Name = "txtDAQGain119"
        Me.txtDAQGain119.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain119.TabIndex = 36
        '
        'txtDAQGain120
        '
        Me.txtDAQGain120.Enabled = False
        Me.txtDAQGain120.Location = New System.Drawing.Point(143, 501)
        Me.txtDAQGain120.Name = "txtDAQGain120"
        Me.txtDAQGain120.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain120.TabIndex = 38
        '
        'txtDAQGain117
        '
        Me.txtDAQGain117.Enabled = False
        Me.txtDAQGain117.Location = New System.Drawing.Point(143, 424)
        Me.txtDAQGain117.Name = "txtDAQGain117"
        Me.txtDAQGain117.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain117.TabIndex = 32
        '
        'txtDAQGain116
        '
        Me.txtDAQGain116.Enabled = False
        Me.txtDAQGain116.Location = New System.Drawing.Point(143, 398)
        Me.txtDAQGain116.Name = "txtDAQGain116"
        Me.txtDAQGain116.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain116.TabIndex = 30
        '
        'txtDAQGain115
        '
        Me.txtDAQGain115.Enabled = False
        Me.txtDAQGain115.Location = New System.Drawing.Point(143, 372)
        Me.txtDAQGain115.Name = "txtDAQGain115"
        Me.txtDAQGain115.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain115.TabIndex = 28
        '
        'txtDAQGain114
        '
        Me.txtDAQGain114.Enabled = False
        Me.txtDAQGain114.Location = New System.Drawing.Point(143, 346)
        Me.txtDAQGain114.Name = "txtDAQGain114"
        Me.txtDAQGain114.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain114.TabIndex = 26
        '
        'txtDAQGain113
        '
        Me.txtDAQGain113.Enabled = False
        Me.txtDAQGain113.Location = New System.Drawing.Point(143, 320)
        Me.txtDAQGain113.Name = "txtDAQGain113"
        Me.txtDAQGain113.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain113.TabIndex = 24
        '
        'txtDAQGain112
        '
        Me.txtDAQGain112.Enabled = False
        Me.txtDAQGain112.Location = New System.Drawing.Point(143, 294)
        Me.txtDAQGain112.Name = "txtDAQGain112"
        Me.txtDAQGain112.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain112.TabIndex = 22
        '
        'txtDAQGain111
        '
        Me.txtDAQGain111.Enabled = False
        Me.txtDAQGain111.Location = New System.Drawing.Point(143, 266)
        Me.txtDAQGain111.Name = "txtDAQGain111"
        Me.txtDAQGain111.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain111.TabIndex = 20
        '
        'txtDAQGain109
        '
        Me.txtDAQGain109.Enabled = False
        Me.txtDAQGain109.Location = New System.Drawing.Point(143, 214)
        Me.txtDAQGain109.Name = "txtDAQGain109"
        Me.txtDAQGain109.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain109.TabIndex = 16
        '
        'txtDAQGain110
        '
        Me.txtDAQGain110.Enabled = False
        Me.txtDAQGain110.Location = New System.Drawing.Point(143, 241)
        Me.txtDAQGain110.Name = "txtDAQGain110"
        Me.txtDAQGain110.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain110.TabIndex = 18
        '
        'txtDAQGain108
        '
        Me.txtDAQGain108.Enabled = False
        Me.txtDAQGain108.Location = New System.Drawing.Point(143, 189)
        Me.txtDAQGain108.Name = "txtDAQGain108"
        Me.txtDAQGain108.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain108.TabIndex = 14
        '
        'txtDAQGain107
        '
        Me.txtDAQGain107.Enabled = False
        Me.txtDAQGain107.Location = New System.Drawing.Point(143, 163)
        Me.txtDAQGain107.Name = "txtDAQGain107"
        Me.txtDAQGain107.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain107.TabIndex = 12
        '
        'txtDAQGain106
        '
        Me.txtDAQGain106.Enabled = False
        Me.txtDAQGain106.Location = New System.Drawing.Point(143, 137)
        Me.txtDAQGain106.Name = "txtDAQGain106"
        Me.txtDAQGain106.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain106.TabIndex = 10
        '
        'txtDAQGain105
        '
        Me.txtDAQGain105.Enabled = False
        Me.txtDAQGain105.Location = New System.Drawing.Point(143, 111)
        Me.txtDAQGain105.Name = "txtDAQGain105"
        Me.txtDAQGain105.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain105.TabIndex = 8
        '
        'txtDAQGain104
        '
        Me.txtDAQGain104.Enabled = False
        Me.txtDAQGain104.Location = New System.Drawing.Point(143, 85)
        Me.txtDAQGain104.Name = "txtDAQGain104"
        Me.txtDAQGain104.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain104.TabIndex = 6
        '
        'txtDAQGain103
        '
        Me.txtDAQGain103.Enabled = False
        Me.txtDAQGain103.Location = New System.Drawing.Point(143, 59)
        Me.txtDAQGain103.Name = "txtDAQGain103"
        Me.txtDAQGain103.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain103.TabIndex = 4
        '
        'txtDAQGain102
        '
        Me.txtDAQGain102.Enabled = False
        Me.txtDAQGain102.Location = New System.Drawing.Point(143, 33)
        Me.txtDAQGain102.Name = "txtDAQGain102"
        Me.txtDAQGain102.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain102.TabIndex = 2
        '
        'txtDAQGain101
        '
        Me.txtDAQGain101.Enabled = False
        Me.txtDAQGain101.Location = New System.Drawing.Point(143, 5)
        Me.txtDAQGain101.Name = "txtDAQGain101"
        Me.txtDAQGain101.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain101.TabIndex = 0
        '
        'txtDAQOffset118
        '
        Me.txtDAQOffset118.Enabled = False
        Me.txtDAQOffset118.Location = New System.Drawing.Point(198, 449)
        Me.txtDAQOffset118.Name = "txtDAQOffset118"
        Me.txtDAQOffset118.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset118.TabIndex = 35
        '
        'txtDAQOffset119
        '
        Me.txtDAQOffset119.Enabled = False
        Me.txtDAQOffset119.Location = New System.Drawing.Point(198, 476)
        Me.txtDAQOffset119.Name = "txtDAQOffset119"
        Me.txtDAQOffset119.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset119.TabIndex = 37
        '
        'txtDAQOffset120
        '
        Me.txtDAQOffset120.Enabled = False
        Me.txtDAQOffset120.Location = New System.Drawing.Point(198, 501)
        Me.txtDAQOffset120.Name = "txtDAQOffset120"
        Me.txtDAQOffset120.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset120.TabIndex = 39
        '
        'txtDAQOffset117
        '
        Me.txtDAQOffset117.Enabled = False
        Me.txtDAQOffset117.Location = New System.Drawing.Point(198, 424)
        Me.txtDAQOffset117.Name = "txtDAQOffset117"
        Me.txtDAQOffset117.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset117.TabIndex = 33
        '
        'txtDAQOffset116
        '
        Me.txtDAQOffset116.Enabled = False
        Me.txtDAQOffset116.Location = New System.Drawing.Point(198, 398)
        Me.txtDAQOffset116.Name = "txtDAQOffset116"
        Me.txtDAQOffset116.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset116.TabIndex = 31
        '
        'txtDAQOffset115
        '
        Me.txtDAQOffset115.Enabled = False
        Me.txtDAQOffset115.Location = New System.Drawing.Point(198, 372)
        Me.txtDAQOffset115.Name = "txtDAQOffset115"
        Me.txtDAQOffset115.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset115.TabIndex = 29
        '
        'txtDAQOffset114
        '
        Me.txtDAQOffset114.Enabled = False
        Me.txtDAQOffset114.Location = New System.Drawing.Point(198, 346)
        Me.txtDAQOffset114.Name = "txtDAQOffset114"
        Me.txtDAQOffset114.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset114.TabIndex = 27
        '
        'txtDAQOffset113
        '
        Me.txtDAQOffset113.Enabled = False
        Me.txtDAQOffset113.Location = New System.Drawing.Point(198, 320)
        Me.txtDAQOffset113.Name = "txtDAQOffset113"
        Me.txtDAQOffset113.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset113.TabIndex = 25
        '
        'txtDAQOffset112
        '
        Me.txtDAQOffset112.Enabled = False
        Me.txtDAQOffset112.Location = New System.Drawing.Point(198, 294)
        Me.txtDAQOffset112.Name = "txtDAQOffset112"
        Me.txtDAQOffset112.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset112.TabIndex = 23
        '
        'txtDAQOffset111
        '
        Me.txtDAQOffset111.Enabled = False
        Me.txtDAQOffset111.Location = New System.Drawing.Point(198, 266)
        Me.txtDAQOffset111.Name = "txtDAQOffset111"
        Me.txtDAQOffset111.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset111.TabIndex = 21
        '
        'txtDAQOffset109
        '
        Me.txtDAQOffset109.Enabled = False
        Me.txtDAQOffset109.Location = New System.Drawing.Point(198, 214)
        Me.txtDAQOffset109.Name = "txtDAQOffset109"
        Me.txtDAQOffset109.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset109.TabIndex = 17
        '
        'txtDAQOffset110
        '
        Me.txtDAQOffset110.Enabled = False
        Me.txtDAQOffset110.Location = New System.Drawing.Point(198, 241)
        Me.txtDAQOffset110.Name = "txtDAQOffset110"
        Me.txtDAQOffset110.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset110.TabIndex = 19
        '
        'txtDAQOffset108
        '
        Me.txtDAQOffset108.Enabled = False
        Me.txtDAQOffset108.Location = New System.Drawing.Point(198, 189)
        Me.txtDAQOffset108.Name = "txtDAQOffset108"
        Me.txtDAQOffset108.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset108.TabIndex = 15
        '
        'txtDAQOffset107
        '
        Me.txtDAQOffset107.Enabled = False
        Me.txtDAQOffset107.Location = New System.Drawing.Point(198, 163)
        Me.txtDAQOffset107.Name = "txtDAQOffset107"
        Me.txtDAQOffset107.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset107.TabIndex = 13
        '
        'txtDAQOffset106
        '
        Me.txtDAQOffset106.Enabled = False
        Me.txtDAQOffset106.Location = New System.Drawing.Point(198, 137)
        Me.txtDAQOffset106.Name = "txtDAQOffset106"
        Me.txtDAQOffset106.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset106.TabIndex = 11
        '
        'txtDAQOffset105
        '
        Me.txtDAQOffset105.Enabled = False
        Me.txtDAQOffset105.Location = New System.Drawing.Point(198, 111)
        Me.txtDAQOffset105.Name = "txtDAQOffset105"
        Me.txtDAQOffset105.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset105.TabIndex = 9
        '
        'txtDAQOffset104
        '
        Me.txtDAQOffset104.Enabled = False
        Me.txtDAQOffset104.Location = New System.Drawing.Point(198, 85)
        Me.txtDAQOffset104.Name = "txtDAQOffset104"
        Me.txtDAQOffset104.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset104.TabIndex = 7
        '
        'txtDAQOffset103
        '
        Me.txtDAQOffset103.Enabled = False
        Me.txtDAQOffset103.Location = New System.Drawing.Point(198, 59)
        Me.txtDAQOffset103.Name = "txtDAQOffset103"
        Me.txtDAQOffset103.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset103.TabIndex = 5
        '
        'txtDAQOffset102
        '
        Me.txtDAQOffset102.Enabled = False
        Me.txtDAQOffset102.Location = New System.Drawing.Point(198, 33)
        Me.txtDAQOffset102.Name = "txtDAQOffset102"
        Me.txtDAQOffset102.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset102.TabIndex = 3
        '
        'txtDAQOffset101
        '
        Me.txtDAQOffset101.Enabled = False
        Me.txtDAQOffset101.Location = New System.Drawing.Point(198, 5)
        Me.txtDAQOffset101.Name = "txtDAQOffset101"
        Me.txtDAQOffset101.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset101.TabIndex = 1
        '
        'cbDAQDataChannel112
        '
        Me.cbDAQDataChannel112.Enabled = False
        Me.cbDAQDataChannel112.FormattingEnabled = True
        Me.cbDAQDataChannel112.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel112.Location = New System.Drawing.Point(52, 293)
        Me.cbDAQDataChannel112.Name = "cbDAQDataChannel112"
        Me.cbDAQDataChannel112.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel112.TabIndex = 230
        '
        'cbDAQDataChannel110
        '
        Me.cbDAQDataChannel110.Enabled = False
        Me.cbDAQDataChannel110.FormattingEnabled = True
        Me.cbDAQDataChannel110.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel110.Location = New System.Drawing.Point(52, 240)
        Me.cbDAQDataChannel110.Name = "cbDAQDataChannel110"
        Me.cbDAQDataChannel110.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel110.TabIndex = 227
        '
        'cbDAQDataChannel120
        '
        Me.cbDAQDataChannel120.Enabled = False
        Me.cbDAQDataChannel120.FormattingEnabled = True
        Me.cbDAQDataChannel120.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel120.Location = New System.Drawing.Point(52, 501)
        Me.cbDAQDataChannel120.Name = "cbDAQDataChannel120"
        Me.cbDAQDataChannel120.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel120.TabIndex = 238
        '
        'cbDAQDataChannel109
        '
        Me.cbDAQDataChannel109.Enabled = False
        Me.cbDAQDataChannel109.FormattingEnabled = True
        Me.cbDAQDataChannel109.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel109.Location = New System.Drawing.Point(52, 214)
        Me.cbDAQDataChannel109.Name = "cbDAQDataChannel109"
        Me.cbDAQDataChannel109.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel109.TabIndex = 226
        '
        'cbDAQDataEnableChannel111
        '
        Me.cbDAQDataEnableChannel111.AutoSize = True
        Me.cbDAQDataEnableChannel111.Location = New System.Drawing.Point(5, 268)
        Me.cbDAQDataEnableChannel111.Name = "cbDAQDataEnableChannel111"
        Me.cbDAQDataEnableChannel111.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel111.TabIndex = 299
        Me.cbDAQDataEnableChannel111.Text = "111"
        Me.cbDAQDataEnableChannel111.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel108
        '
        Me.cbDAQDataChannel108.Enabled = False
        Me.cbDAQDataChannel108.FormattingEnabled = True
        Me.cbDAQDataChannel108.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel108.Location = New System.Drawing.Point(52, 188)
        Me.cbDAQDataChannel108.Name = "cbDAQDataChannel108"
        Me.cbDAQDataChannel108.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel108.TabIndex = 225
        '
        'cbDAQDataEnableChannel120
        '
        Me.cbDAQDataEnableChannel120.AutoSize = True
        Me.cbDAQDataEnableChannel120.Location = New System.Drawing.Point(5, 502)
        Me.cbDAQDataEnableChannel120.Name = "cbDAQDataEnableChannel120"
        Me.cbDAQDataEnableChannel120.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel120.TabIndex = 308
        Me.cbDAQDataEnableChannel120.Text = "120"
        Me.cbDAQDataEnableChannel120.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel107
        '
        Me.cbDAQDataChannel107.Enabled = False
        Me.cbDAQDataChannel107.FormattingEnabled = True
        Me.cbDAQDataChannel107.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel107.Location = New System.Drawing.Point(52, 162)
        Me.cbDAQDataChannel107.Name = "cbDAQDataChannel107"
        Me.cbDAQDataChannel107.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel107.TabIndex = 224
        '
        'cbDAQDataChannel111
        '
        Me.cbDAQDataChannel111.Enabled = False
        Me.cbDAQDataChannel111.FormattingEnabled = True
        Me.cbDAQDataChannel111.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel111.Location = New System.Drawing.Point(52, 266)
        Me.cbDAQDataChannel111.Name = "cbDAQDataChannel111"
        Me.cbDAQDataChannel111.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel111.TabIndex = 229
        '
        'cbDAQDataChannel106
        '
        Me.cbDAQDataChannel106.Enabled = False
        Me.cbDAQDataChannel106.FormattingEnabled = True
        Me.cbDAQDataChannel106.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel106.Location = New System.Drawing.Point(52, 136)
        Me.cbDAQDataChannel106.Name = "cbDAQDataChannel106"
        Me.cbDAQDataChannel106.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel106.TabIndex = 223
        '
        'cbDAQDataChannel119
        '
        Me.cbDAQDataChannel119.Enabled = False
        Me.cbDAQDataChannel119.FormattingEnabled = True
        Me.cbDAQDataChannel119.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel119.Location = New System.Drawing.Point(52, 475)
        Me.cbDAQDataChannel119.Name = "cbDAQDataChannel119"
        Me.cbDAQDataChannel119.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel119.TabIndex = 237
        '
        'cbDAQDataChannel105
        '
        Me.cbDAQDataChannel105.Enabled = False
        Me.cbDAQDataChannel105.FormattingEnabled = True
        Me.cbDAQDataChannel105.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel105.Location = New System.Drawing.Point(52, 110)
        Me.cbDAQDataChannel105.Name = "cbDAQDataChannel105"
        Me.cbDAQDataChannel105.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel105.TabIndex = 222
        '
        'cbDAQDataEnableChannel112
        '
        Me.cbDAQDataEnableChannel112.AutoSize = True
        Me.cbDAQDataEnableChannel112.Location = New System.Drawing.Point(5, 294)
        Me.cbDAQDataEnableChannel112.Name = "cbDAQDataEnableChannel112"
        Me.cbDAQDataEnableChannel112.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel112.TabIndex = 300
        Me.cbDAQDataEnableChannel112.Text = "112"
        Me.cbDAQDataEnableChannel112.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel104
        '
        Me.cbDAQDataChannel104.Enabled = False
        Me.cbDAQDataChannel104.FormattingEnabled = True
        Me.cbDAQDataChannel104.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel104.Location = New System.Drawing.Point(52, 84)
        Me.cbDAQDataChannel104.Name = "cbDAQDataChannel104"
        Me.cbDAQDataChannel104.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel104.TabIndex = 221
        '
        'cbDAQDataEnableChannel119
        '
        Me.cbDAQDataEnableChannel119.AutoSize = True
        Me.cbDAQDataEnableChannel119.Location = New System.Drawing.Point(5, 476)
        Me.cbDAQDataEnableChannel119.Name = "cbDAQDataEnableChannel119"
        Me.cbDAQDataEnableChannel119.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel119.TabIndex = 307
        Me.cbDAQDataEnableChannel119.Text = "119"
        Me.cbDAQDataEnableChannel119.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel103
        '
        Me.cbDAQDataChannel103.Enabled = False
        Me.cbDAQDataChannel103.FormattingEnabled = True
        Me.cbDAQDataChannel103.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel103.Location = New System.Drawing.Point(52, 58)
        Me.cbDAQDataChannel103.Name = "cbDAQDataChannel103"
        Me.cbDAQDataChannel103.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel103.TabIndex = 220
        '
        'cbDAQDataEnableChannel113
        '
        Me.cbDAQDataEnableChannel113.AutoSize = True
        Me.cbDAQDataEnableChannel113.Location = New System.Drawing.Point(5, 320)
        Me.cbDAQDataEnableChannel113.Name = "cbDAQDataEnableChannel113"
        Me.cbDAQDataEnableChannel113.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel113.TabIndex = 301
        Me.cbDAQDataEnableChannel113.Text = "113"
        Me.cbDAQDataEnableChannel113.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel102
        '
        Me.cbDAQDataChannel102.Enabled = False
        Me.cbDAQDataChannel102.FormattingEnabled = True
        Me.cbDAQDataChannel102.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel102.Location = New System.Drawing.Point(52, 32)
        Me.cbDAQDataChannel102.Name = "cbDAQDataChannel102"
        Me.cbDAQDataChannel102.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel102.TabIndex = 219
        '
        'cbDAQDataChannel101
        '
        Me.cbDAQDataChannel101.Enabled = False
        Me.cbDAQDataChannel101.FormattingEnabled = True
        Me.cbDAQDataChannel101.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel101.Location = New System.Drawing.Point(52, 5)
        Me.cbDAQDataChannel101.Name = "cbDAQDataChannel101"
        Me.cbDAQDataChannel101.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel101.TabIndex = 59
        '
        'cbDAQDataChannel118
        '
        Me.cbDAQDataChannel118.Enabled = False
        Me.cbDAQDataChannel118.FormattingEnabled = True
        Me.cbDAQDataChannel118.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel118.Location = New System.Drawing.Point(52, 449)
        Me.cbDAQDataChannel118.Name = "cbDAQDataChannel118"
        Me.cbDAQDataChannel118.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel118.TabIndex = 236
        '
        'cbDAQDataChannel113
        '
        Me.cbDAQDataChannel113.Enabled = False
        Me.cbDAQDataChannel113.FormattingEnabled = True
        Me.cbDAQDataChannel113.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel113.Location = New System.Drawing.Point(52, 319)
        Me.cbDAQDataChannel113.Name = "cbDAQDataChannel113"
        Me.cbDAQDataChannel113.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel113.TabIndex = 231
        '
        'cbDAQDataEnableChannel118
        '
        Me.cbDAQDataEnableChannel118.AutoSize = True
        Me.cbDAQDataEnableChannel118.Location = New System.Drawing.Point(5, 450)
        Me.cbDAQDataEnableChannel118.Name = "cbDAQDataEnableChannel118"
        Me.cbDAQDataEnableChannel118.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel118.TabIndex = 306
        Me.cbDAQDataEnableChannel118.Text = "118"
        Me.cbDAQDataEnableChannel118.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel114
        '
        Me.cbDAQDataEnableChannel114.AutoSize = True
        Me.cbDAQDataEnableChannel114.Location = New System.Drawing.Point(5, 346)
        Me.cbDAQDataEnableChannel114.Name = "cbDAQDataEnableChannel114"
        Me.cbDAQDataEnableChannel114.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel114.TabIndex = 302
        Me.cbDAQDataEnableChannel114.Text = "114"
        Me.cbDAQDataEnableChannel114.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel117
        '
        Me.cbDAQDataChannel117.Enabled = False
        Me.cbDAQDataChannel117.FormattingEnabled = True
        Me.cbDAQDataChannel117.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel117.Location = New System.Drawing.Point(52, 423)
        Me.cbDAQDataChannel117.Name = "cbDAQDataChannel117"
        Me.cbDAQDataChannel117.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel117.TabIndex = 235
        '
        'cbDAQDataChannel114
        '
        Me.cbDAQDataChannel114.Enabled = False
        Me.cbDAQDataChannel114.FormattingEnabled = True
        Me.cbDAQDataChannel114.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel114.Location = New System.Drawing.Point(52, 345)
        Me.cbDAQDataChannel114.Name = "cbDAQDataChannel114"
        Me.cbDAQDataChannel114.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel114.TabIndex = 232
        '
        'cbDAQDataEnableChannel117
        '
        Me.cbDAQDataEnableChannel117.AutoSize = True
        Me.cbDAQDataEnableChannel117.Location = New System.Drawing.Point(5, 424)
        Me.cbDAQDataEnableChannel117.Name = "cbDAQDataEnableChannel117"
        Me.cbDAQDataEnableChannel117.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel117.TabIndex = 305
        Me.cbDAQDataEnableChannel117.Text = "117"
        Me.cbDAQDataEnableChannel117.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel115
        '
        Me.cbDAQDataEnableChannel115.AutoSize = True
        Me.cbDAQDataEnableChannel115.Location = New System.Drawing.Point(5, 372)
        Me.cbDAQDataEnableChannel115.Name = "cbDAQDataEnableChannel115"
        Me.cbDAQDataEnableChannel115.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel115.TabIndex = 303
        Me.cbDAQDataEnableChannel115.Text = "115"
        Me.cbDAQDataEnableChannel115.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel116
        '
        Me.cbDAQDataChannel116.Enabled = False
        Me.cbDAQDataChannel116.FormattingEnabled = True
        Me.cbDAQDataChannel116.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel116.Location = New System.Drawing.Point(52, 397)
        Me.cbDAQDataChannel116.Name = "cbDAQDataChannel116"
        Me.cbDAQDataChannel116.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel116.TabIndex = 234
        '
        'cbDAQDataChannel115
        '
        Me.cbDAQDataChannel115.Enabled = False
        Me.cbDAQDataChannel115.FormattingEnabled = True
        Me.cbDAQDataChannel115.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel115.Location = New System.Drawing.Point(52, 371)
        Me.cbDAQDataChannel115.Name = "cbDAQDataChannel115"
        Me.cbDAQDataChannel115.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel115.TabIndex = 233
        '
        'cbDAQDataEnableChannel116
        '
        Me.cbDAQDataEnableChannel116.AutoSize = True
        Me.cbDAQDataEnableChannel116.Location = New System.Drawing.Point(5, 398)
        Me.cbDAQDataEnableChannel116.Name = "cbDAQDataEnableChannel116"
        Me.cbDAQDataEnableChannel116.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel116.TabIndex = 304
        Me.cbDAQDataEnableChannel116.Text = "116"
        Me.cbDAQDataEnableChannel116.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel101
        '
        Me.cbDAQDataEnableChannel101.AutoSize = True
        Me.cbDAQDataEnableChannel101.Location = New System.Drawing.Point(5, 7)
        Me.cbDAQDataEnableChannel101.Name = "cbDAQDataEnableChannel101"
        Me.cbDAQDataEnableChannel101.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel101.TabIndex = 111
        Me.cbDAQDataEnableChannel101.Text = "101"
        Me.cbDAQDataEnableChannel101.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel102
        '
        Me.cbDAQDataEnableChannel102.AutoSize = True
        Me.cbDAQDataEnableChannel102.Location = New System.Drawing.Point(5, 33)
        Me.cbDAQDataEnableChannel102.Name = "cbDAQDataEnableChannel102"
        Me.cbDAQDataEnableChannel102.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel102.TabIndex = 112
        Me.cbDAQDataEnableChannel102.Text = "102"
        Me.cbDAQDataEnableChannel102.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel103
        '
        Me.cbDAQDataEnableChannel103.AutoSize = True
        Me.cbDAQDataEnableChannel103.Location = New System.Drawing.Point(5, 59)
        Me.cbDAQDataEnableChannel103.Name = "cbDAQDataEnableChannel103"
        Me.cbDAQDataEnableChannel103.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel103.TabIndex = 113
        Me.cbDAQDataEnableChannel103.Text = "103"
        Me.cbDAQDataEnableChannel103.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel104
        '
        Me.cbDAQDataEnableChannel104.AutoSize = True
        Me.cbDAQDataEnableChannel104.Location = New System.Drawing.Point(5, 85)
        Me.cbDAQDataEnableChannel104.Name = "cbDAQDataEnableChannel104"
        Me.cbDAQDataEnableChannel104.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel104.TabIndex = 114
        Me.cbDAQDataEnableChannel104.Text = "104"
        Me.cbDAQDataEnableChannel104.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel105
        '
        Me.cbDAQDataEnableChannel105.AutoSize = True
        Me.cbDAQDataEnableChannel105.Location = New System.Drawing.Point(5, 111)
        Me.cbDAQDataEnableChannel105.Name = "cbDAQDataEnableChannel105"
        Me.cbDAQDataEnableChannel105.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel105.TabIndex = 115
        Me.cbDAQDataEnableChannel105.Text = "105"
        Me.cbDAQDataEnableChannel105.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel106
        '
        Me.cbDAQDataEnableChannel106.AutoSize = True
        Me.cbDAQDataEnableChannel106.Location = New System.Drawing.Point(5, 137)
        Me.cbDAQDataEnableChannel106.Name = "cbDAQDataEnableChannel106"
        Me.cbDAQDataEnableChannel106.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel106.TabIndex = 116
        Me.cbDAQDataEnableChannel106.Text = "106"
        Me.cbDAQDataEnableChannel106.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel107
        '
        Me.cbDAQDataEnableChannel107.AutoSize = True
        Me.cbDAQDataEnableChannel107.Location = New System.Drawing.Point(5, 163)
        Me.cbDAQDataEnableChannel107.Name = "cbDAQDataEnableChannel107"
        Me.cbDAQDataEnableChannel107.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel107.TabIndex = 117
        Me.cbDAQDataEnableChannel107.Text = "107"
        Me.cbDAQDataEnableChannel107.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel108
        '
        Me.cbDAQDataEnableChannel108.AutoSize = True
        Me.cbDAQDataEnableChannel108.Location = New System.Drawing.Point(5, 189)
        Me.cbDAQDataEnableChannel108.Name = "cbDAQDataEnableChannel108"
        Me.cbDAQDataEnableChannel108.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel108.TabIndex = 118
        Me.cbDAQDataEnableChannel108.Text = "108"
        Me.cbDAQDataEnableChannel108.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel109
        '
        Me.cbDAQDataEnableChannel109.AutoSize = True
        Me.cbDAQDataEnableChannel109.Location = New System.Drawing.Point(5, 215)
        Me.cbDAQDataEnableChannel109.Name = "cbDAQDataEnableChannel109"
        Me.cbDAQDataEnableChannel109.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel109.TabIndex = 119
        Me.cbDAQDataEnableChannel109.Text = "109"
        Me.cbDAQDataEnableChannel109.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel110
        '
        Me.cbDAQDataEnableChannel110.AutoSize = True
        Me.cbDAQDataEnableChannel110.Location = New System.Drawing.Point(5, 241)
        Me.cbDAQDataEnableChannel110.Name = "cbDAQDataEnableChannel110"
        Me.cbDAQDataEnableChannel110.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel110.TabIndex = 120
        Me.cbDAQDataEnableChannel110.Text = "110"
        Me.cbDAQDataEnableChannel110.UseVisualStyleBackColor = True
        '
        'panelModule2
        '
        Me.panelModule2.AutoScroll = True
        Me.panelModule2.BackColor = System.Drawing.Color.Yellow
        Me.panelModule2.Controls.Add(Me.txtDAQGain218)
        Me.panelModule2.Controls.Add(Me.txtDAQGain219)
        Me.panelModule2.Controls.Add(Me.txtDAQGain220)
        Me.panelModule2.Controls.Add(Me.txtDAQGain217)
        Me.panelModule2.Controls.Add(Me.txtDAQGain216)
        Me.panelModule2.Controls.Add(Me.txtDAQGain215)
        Me.panelModule2.Controls.Add(Me.txtDAQGain214)
        Me.panelModule2.Controls.Add(Me.txtDAQGain213)
        Me.panelModule2.Controls.Add(Me.txtDAQGain212)
        Me.panelModule2.Controls.Add(Me.txtDAQGain211)
        Me.panelModule2.Controls.Add(Me.txtDAQGain209)
        Me.panelModule2.Controls.Add(Me.txtDAQGain210)
        Me.panelModule2.Controls.Add(Me.txtDAQGain208)
        Me.panelModule2.Controls.Add(Me.txtDAQGain207)
        Me.panelModule2.Controls.Add(Me.txtDAQGain206)
        Me.panelModule2.Controls.Add(Me.txtDAQGain205)
        Me.panelModule2.Controls.Add(Me.txtDAQGain204)
        Me.panelModule2.Controls.Add(Me.txtDAQGain203)
        Me.panelModule2.Controls.Add(Me.txtDAQGain202)
        Me.panelModule2.Controls.Add(Me.txtDAQGain201)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset218)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset219)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset220)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset217)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset216)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset215)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset214)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset213)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset212)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset211)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset209)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset210)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset208)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset207)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset206)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset205)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset204)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset203)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset202)
        Me.panelModule2.Controls.Add(Me.txtDAQOffset201)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel212)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel210)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel220)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel209)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel211)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel208)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel220)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel207)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel211)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel206)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel219)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel205)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel212)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel204)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel219)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel203)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel213)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel202)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel201)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel218)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel213)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel218)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel214)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel217)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel214)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel217)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel215)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel216)
        Me.panelModule2.Controls.Add(Me.cbDAQDataChannel215)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel216)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel201)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel202)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel203)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel204)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel205)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel206)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel207)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel208)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel209)
        Me.panelModule2.Controls.Add(Me.cbDAQDataEnableChannel210)
        Me.panelModule2.Location = New System.Drawing.Point(5, 138)
        Me.panelModule2.Name = "panelModule2"
        Me.panelModule2.Size = New System.Drawing.Size(275, 265)
        Me.panelModule2.TabIndex = 312
        Me.panelModule2.Visible = False
        '
        'txtDAQGain218
        '
        Me.txtDAQGain218.Enabled = False
        Me.txtDAQGain218.Location = New System.Drawing.Point(143, 450)
        Me.txtDAQGain218.Name = "txtDAQGain218"
        Me.txtDAQGain218.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain218.TabIndex = 34
        '
        'txtDAQGain219
        '
        Me.txtDAQGain219.Enabled = False
        Me.txtDAQGain219.Location = New System.Drawing.Point(143, 477)
        Me.txtDAQGain219.Name = "txtDAQGain219"
        Me.txtDAQGain219.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain219.TabIndex = 36
        '
        'txtDAQGain220
        '
        Me.txtDAQGain220.Enabled = False
        Me.txtDAQGain220.Location = New System.Drawing.Point(143, 501)
        Me.txtDAQGain220.Name = "txtDAQGain220"
        Me.txtDAQGain220.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain220.TabIndex = 38
        '
        'txtDAQGain217
        '
        Me.txtDAQGain217.Enabled = False
        Me.txtDAQGain217.Location = New System.Drawing.Point(143, 425)
        Me.txtDAQGain217.Name = "txtDAQGain217"
        Me.txtDAQGain217.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain217.TabIndex = 32
        '
        'txtDAQGain216
        '
        Me.txtDAQGain216.Enabled = False
        Me.txtDAQGain216.Location = New System.Drawing.Point(143, 399)
        Me.txtDAQGain216.Name = "txtDAQGain216"
        Me.txtDAQGain216.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain216.TabIndex = 30
        '
        'txtDAQGain215
        '
        Me.txtDAQGain215.Enabled = False
        Me.txtDAQGain215.Location = New System.Drawing.Point(143, 373)
        Me.txtDAQGain215.Name = "txtDAQGain215"
        Me.txtDAQGain215.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain215.TabIndex = 28
        '
        'txtDAQGain214
        '
        Me.txtDAQGain214.Enabled = False
        Me.txtDAQGain214.Location = New System.Drawing.Point(143, 347)
        Me.txtDAQGain214.Name = "txtDAQGain214"
        Me.txtDAQGain214.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain214.TabIndex = 26
        '
        'txtDAQGain213
        '
        Me.txtDAQGain213.Enabled = False
        Me.txtDAQGain213.Location = New System.Drawing.Point(143, 321)
        Me.txtDAQGain213.Name = "txtDAQGain213"
        Me.txtDAQGain213.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain213.TabIndex = 24
        '
        'txtDAQGain212
        '
        Me.txtDAQGain212.Enabled = False
        Me.txtDAQGain212.Location = New System.Drawing.Point(143, 295)
        Me.txtDAQGain212.Name = "txtDAQGain212"
        Me.txtDAQGain212.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain212.TabIndex = 22
        '
        'txtDAQGain211
        '
        Me.txtDAQGain211.Enabled = False
        Me.txtDAQGain211.Location = New System.Drawing.Point(143, 267)
        Me.txtDAQGain211.Name = "txtDAQGain211"
        Me.txtDAQGain211.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain211.TabIndex = 20
        '
        'txtDAQGain209
        '
        Me.txtDAQGain209.Enabled = False
        Me.txtDAQGain209.Location = New System.Drawing.Point(143, 215)
        Me.txtDAQGain209.Name = "txtDAQGain209"
        Me.txtDAQGain209.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain209.TabIndex = 16
        '
        'txtDAQGain210
        '
        Me.txtDAQGain210.Enabled = False
        Me.txtDAQGain210.Location = New System.Drawing.Point(143, 242)
        Me.txtDAQGain210.Name = "txtDAQGain210"
        Me.txtDAQGain210.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain210.TabIndex = 18
        '
        'txtDAQGain208
        '
        Me.txtDAQGain208.Enabled = False
        Me.txtDAQGain208.Location = New System.Drawing.Point(143, 190)
        Me.txtDAQGain208.Name = "txtDAQGain208"
        Me.txtDAQGain208.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain208.TabIndex = 14
        '
        'txtDAQGain207
        '
        Me.txtDAQGain207.Enabled = False
        Me.txtDAQGain207.Location = New System.Drawing.Point(143, 164)
        Me.txtDAQGain207.Name = "txtDAQGain207"
        Me.txtDAQGain207.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain207.TabIndex = 12
        '
        'txtDAQGain206
        '
        Me.txtDAQGain206.Enabled = False
        Me.txtDAQGain206.Location = New System.Drawing.Point(143, 138)
        Me.txtDAQGain206.Name = "txtDAQGain206"
        Me.txtDAQGain206.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain206.TabIndex = 10
        '
        'txtDAQGain205
        '
        Me.txtDAQGain205.Enabled = False
        Me.txtDAQGain205.Location = New System.Drawing.Point(143, 112)
        Me.txtDAQGain205.Name = "txtDAQGain205"
        Me.txtDAQGain205.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain205.TabIndex = 8
        '
        'txtDAQGain204
        '
        Me.txtDAQGain204.Enabled = False
        Me.txtDAQGain204.Location = New System.Drawing.Point(143, 86)
        Me.txtDAQGain204.Name = "txtDAQGain204"
        Me.txtDAQGain204.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain204.TabIndex = 6
        '
        'txtDAQGain203
        '
        Me.txtDAQGain203.Enabled = False
        Me.txtDAQGain203.Location = New System.Drawing.Point(143, 60)
        Me.txtDAQGain203.Name = "txtDAQGain203"
        Me.txtDAQGain203.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain203.TabIndex = 4
        '
        'txtDAQGain202
        '
        Me.txtDAQGain202.Enabled = False
        Me.txtDAQGain202.Location = New System.Drawing.Point(143, 34)
        Me.txtDAQGain202.Name = "txtDAQGain202"
        Me.txtDAQGain202.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain202.TabIndex = 2
        '
        'txtDAQGain201
        '
        Me.txtDAQGain201.Location = New System.Drawing.Point(143, 6)
        Me.txtDAQGain201.Name = "txtDAQGain201"
        Me.txtDAQGain201.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain201.TabIndex = 0
        '
        'txtDAQOffset218
        '
        Me.txtDAQOffset218.Enabled = False
        Me.txtDAQOffset218.Location = New System.Drawing.Point(198, 449)
        Me.txtDAQOffset218.Name = "txtDAQOffset218"
        Me.txtDAQOffset218.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset218.TabIndex = 35
        '
        'txtDAQOffset219
        '
        Me.txtDAQOffset219.Enabled = False
        Me.txtDAQOffset219.Location = New System.Drawing.Point(198, 476)
        Me.txtDAQOffset219.Name = "txtDAQOffset219"
        Me.txtDAQOffset219.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset219.TabIndex = 37
        '
        'txtDAQOffset220
        '
        Me.txtDAQOffset220.Enabled = False
        Me.txtDAQOffset220.Location = New System.Drawing.Point(198, 501)
        Me.txtDAQOffset220.Name = "txtDAQOffset220"
        Me.txtDAQOffset220.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset220.TabIndex = 39
        '
        'txtDAQOffset217
        '
        Me.txtDAQOffset217.Enabled = False
        Me.txtDAQOffset217.Location = New System.Drawing.Point(198, 424)
        Me.txtDAQOffset217.Name = "txtDAQOffset217"
        Me.txtDAQOffset217.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset217.TabIndex = 33
        '
        'txtDAQOffset216
        '
        Me.txtDAQOffset216.Enabled = False
        Me.txtDAQOffset216.Location = New System.Drawing.Point(198, 398)
        Me.txtDAQOffset216.Name = "txtDAQOffset216"
        Me.txtDAQOffset216.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset216.TabIndex = 31
        '
        'txtDAQOffset215
        '
        Me.txtDAQOffset215.Enabled = False
        Me.txtDAQOffset215.Location = New System.Drawing.Point(198, 372)
        Me.txtDAQOffset215.Name = "txtDAQOffset215"
        Me.txtDAQOffset215.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset215.TabIndex = 29
        '
        'txtDAQOffset214
        '
        Me.txtDAQOffset214.Enabled = False
        Me.txtDAQOffset214.Location = New System.Drawing.Point(198, 346)
        Me.txtDAQOffset214.Name = "txtDAQOffset214"
        Me.txtDAQOffset214.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset214.TabIndex = 27
        '
        'txtDAQOffset213
        '
        Me.txtDAQOffset213.Enabled = False
        Me.txtDAQOffset213.Location = New System.Drawing.Point(198, 320)
        Me.txtDAQOffset213.Name = "txtDAQOffset213"
        Me.txtDAQOffset213.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset213.TabIndex = 25
        '
        'txtDAQOffset212
        '
        Me.txtDAQOffset212.Enabled = False
        Me.txtDAQOffset212.Location = New System.Drawing.Point(198, 294)
        Me.txtDAQOffset212.Name = "txtDAQOffset212"
        Me.txtDAQOffset212.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset212.TabIndex = 23
        '
        'txtDAQOffset211
        '
        Me.txtDAQOffset211.Enabled = False
        Me.txtDAQOffset211.Location = New System.Drawing.Point(198, 266)
        Me.txtDAQOffset211.Name = "txtDAQOffset211"
        Me.txtDAQOffset211.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset211.TabIndex = 21
        '
        'txtDAQOffset209
        '
        Me.txtDAQOffset209.Enabled = False
        Me.txtDAQOffset209.Location = New System.Drawing.Point(198, 214)
        Me.txtDAQOffset209.Name = "txtDAQOffset209"
        Me.txtDAQOffset209.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset209.TabIndex = 17
        '
        'txtDAQOffset210
        '
        Me.txtDAQOffset210.Enabled = False
        Me.txtDAQOffset210.Location = New System.Drawing.Point(198, 241)
        Me.txtDAQOffset210.Name = "txtDAQOffset210"
        Me.txtDAQOffset210.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset210.TabIndex = 19
        '
        'txtDAQOffset208
        '
        Me.txtDAQOffset208.Enabled = False
        Me.txtDAQOffset208.Location = New System.Drawing.Point(198, 189)
        Me.txtDAQOffset208.Name = "txtDAQOffset208"
        Me.txtDAQOffset208.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset208.TabIndex = 15
        '
        'txtDAQOffset207
        '
        Me.txtDAQOffset207.Enabled = False
        Me.txtDAQOffset207.Location = New System.Drawing.Point(198, 163)
        Me.txtDAQOffset207.Name = "txtDAQOffset207"
        Me.txtDAQOffset207.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset207.TabIndex = 13
        '
        'txtDAQOffset206
        '
        Me.txtDAQOffset206.Enabled = False
        Me.txtDAQOffset206.Location = New System.Drawing.Point(198, 137)
        Me.txtDAQOffset206.Name = "txtDAQOffset206"
        Me.txtDAQOffset206.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset206.TabIndex = 11
        '
        'txtDAQOffset205
        '
        Me.txtDAQOffset205.Enabled = False
        Me.txtDAQOffset205.Location = New System.Drawing.Point(198, 111)
        Me.txtDAQOffset205.Name = "txtDAQOffset205"
        Me.txtDAQOffset205.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset205.TabIndex = 9
        '
        'txtDAQOffset204
        '
        Me.txtDAQOffset204.Enabled = False
        Me.txtDAQOffset204.Location = New System.Drawing.Point(198, 85)
        Me.txtDAQOffset204.Name = "txtDAQOffset204"
        Me.txtDAQOffset204.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset204.TabIndex = 7
        '
        'txtDAQOffset203
        '
        Me.txtDAQOffset203.Enabled = False
        Me.txtDAQOffset203.Location = New System.Drawing.Point(198, 59)
        Me.txtDAQOffset203.Name = "txtDAQOffset203"
        Me.txtDAQOffset203.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset203.TabIndex = 5
        '
        'txtDAQOffset202
        '
        Me.txtDAQOffset202.Enabled = False
        Me.txtDAQOffset202.Location = New System.Drawing.Point(198, 33)
        Me.txtDAQOffset202.Name = "txtDAQOffset202"
        Me.txtDAQOffset202.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset202.TabIndex = 3
        '
        'txtDAQOffset201
        '
        Me.txtDAQOffset201.Enabled = False
        Me.txtDAQOffset201.Location = New System.Drawing.Point(198, 5)
        Me.txtDAQOffset201.Name = "txtDAQOffset201"
        Me.txtDAQOffset201.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset201.TabIndex = 1
        '
        'cbDAQDataChannel212
        '
        Me.cbDAQDataChannel212.Enabled = False
        Me.cbDAQDataChannel212.FormattingEnabled = True
        Me.cbDAQDataChannel212.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel212.Location = New System.Drawing.Point(52, 293)
        Me.cbDAQDataChannel212.Name = "cbDAQDataChannel212"
        Me.cbDAQDataChannel212.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel212.TabIndex = 230
        '
        'cbDAQDataChannel210
        '
        Me.cbDAQDataChannel210.Enabled = False
        Me.cbDAQDataChannel210.FormattingEnabled = True
        Me.cbDAQDataChannel210.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel210.Location = New System.Drawing.Point(52, 240)
        Me.cbDAQDataChannel210.Name = "cbDAQDataChannel210"
        Me.cbDAQDataChannel210.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel210.TabIndex = 227
        '
        'cbDAQDataChannel220
        '
        Me.cbDAQDataChannel220.Enabled = False
        Me.cbDAQDataChannel220.FormattingEnabled = True
        Me.cbDAQDataChannel220.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel220.Location = New System.Drawing.Point(52, 501)
        Me.cbDAQDataChannel220.Name = "cbDAQDataChannel220"
        Me.cbDAQDataChannel220.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel220.TabIndex = 238
        '
        'cbDAQDataChannel209
        '
        Me.cbDAQDataChannel209.Enabled = False
        Me.cbDAQDataChannel209.FormattingEnabled = True
        Me.cbDAQDataChannel209.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel209.Location = New System.Drawing.Point(52, 214)
        Me.cbDAQDataChannel209.Name = "cbDAQDataChannel209"
        Me.cbDAQDataChannel209.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel209.TabIndex = 226
        '
        'cbDAQDataEnableChannel211
        '
        Me.cbDAQDataEnableChannel211.AutoSize = True
        Me.cbDAQDataEnableChannel211.Location = New System.Drawing.Point(5, 268)
        Me.cbDAQDataEnableChannel211.Name = "cbDAQDataEnableChannel211"
        Me.cbDAQDataEnableChannel211.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel211.TabIndex = 299
        Me.cbDAQDataEnableChannel211.Text = "211"
        Me.cbDAQDataEnableChannel211.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel208
        '
        Me.cbDAQDataChannel208.Enabled = False
        Me.cbDAQDataChannel208.FormattingEnabled = True
        Me.cbDAQDataChannel208.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel208.Location = New System.Drawing.Point(52, 188)
        Me.cbDAQDataChannel208.Name = "cbDAQDataChannel208"
        Me.cbDAQDataChannel208.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel208.TabIndex = 225
        '
        'cbDAQDataEnableChannel220
        '
        Me.cbDAQDataEnableChannel220.AutoSize = True
        Me.cbDAQDataEnableChannel220.Location = New System.Drawing.Point(5, 502)
        Me.cbDAQDataEnableChannel220.Name = "cbDAQDataEnableChannel220"
        Me.cbDAQDataEnableChannel220.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel220.TabIndex = 308
        Me.cbDAQDataEnableChannel220.Text = "220"
        Me.cbDAQDataEnableChannel220.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel207
        '
        Me.cbDAQDataChannel207.Enabled = False
        Me.cbDAQDataChannel207.FormattingEnabled = True
        Me.cbDAQDataChannel207.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel207.Location = New System.Drawing.Point(52, 162)
        Me.cbDAQDataChannel207.Name = "cbDAQDataChannel207"
        Me.cbDAQDataChannel207.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel207.TabIndex = 224
        '
        'cbDAQDataChannel211
        '
        Me.cbDAQDataChannel211.Enabled = False
        Me.cbDAQDataChannel211.FormattingEnabled = True
        Me.cbDAQDataChannel211.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel211.Location = New System.Drawing.Point(52, 266)
        Me.cbDAQDataChannel211.Name = "cbDAQDataChannel211"
        Me.cbDAQDataChannel211.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel211.TabIndex = 229
        '
        'cbDAQDataChannel206
        '
        Me.cbDAQDataChannel206.Enabled = False
        Me.cbDAQDataChannel206.FormattingEnabled = True
        Me.cbDAQDataChannel206.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel206.Location = New System.Drawing.Point(52, 136)
        Me.cbDAQDataChannel206.Name = "cbDAQDataChannel206"
        Me.cbDAQDataChannel206.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel206.TabIndex = 223
        '
        'cbDAQDataChannel219
        '
        Me.cbDAQDataChannel219.Enabled = False
        Me.cbDAQDataChannel219.FormattingEnabled = True
        Me.cbDAQDataChannel219.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel219.Location = New System.Drawing.Point(52, 475)
        Me.cbDAQDataChannel219.Name = "cbDAQDataChannel219"
        Me.cbDAQDataChannel219.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel219.TabIndex = 237
        '
        'cbDAQDataChannel205
        '
        Me.cbDAQDataChannel205.Enabled = False
        Me.cbDAQDataChannel205.FormattingEnabled = True
        Me.cbDAQDataChannel205.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel205.Location = New System.Drawing.Point(52, 110)
        Me.cbDAQDataChannel205.Name = "cbDAQDataChannel205"
        Me.cbDAQDataChannel205.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel205.TabIndex = 222
        '
        'cbDAQDataEnableChannel212
        '
        Me.cbDAQDataEnableChannel212.AutoSize = True
        Me.cbDAQDataEnableChannel212.Location = New System.Drawing.Point(5, 294)
        Me.cbDAQDataEnableChannel212.Name = "cbDAQDataEnableChannel212"
        Me.cbDAQDataEnableChannel212.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel212.TabIndex = 300
        Me.cbDAQDataEnableChannel212.Text = "212"
        Me.cbDAQDataEnableChannel212.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel204
        '
        Me.cbDAQDataChannel204.Enabled = False
        Me.cbDAQDataChannel204.FormattingEnabled = True
        Me.cbDAQDataChannel204.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel204.Location = New System.Drawing.Point(52, 84)
        Me.cbDAQDataChannel204.Name = "cbDAQDataChannel204"
        Me.cbDAQDataChannel204.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel204.TabIndex = 221
        '
        'cbDAQDataEnableChannel219
        '
        Me.cbDAQDataEnableChannel219.AutoSize = True
        Me.cbDAQDataEnableChannel219.Location = New System.Drawing.Point(5, 476)
        Me.cbDAQDataEnableChannel219.Name = "cbDAQDataEnableChannel219"
        Me.cbDAQDataEnableChannel219.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel219.TabIndex = 307
        Me.cbDAQDataEnableChannel219.Text = "219"
        Me.cbDAQDataEnableChannel219.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel203
        '
        Me.cbDAQDataChannel203.Enabled = False
        Me.cbDAQDataChannel203.FormattingEnabled = True
        Me.cbDAQDataChannel203.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel203.Location = New System.Drawing.Point(52, 58)
        Me.cbDAQDataChannel203.Name = "cbDAQDataChannel203"
        Me.cbDAQDataChannel203.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel203.TabIndex = 220
        '
        'cbDAQDataEnableChannel213
        '
        Me.cbDAQDataEnableChannel213.AutoSize = True
        Me.cbDAQDataEnableChannel213.Location = New System.Drawing.Point(5, 320)
        Me.cbDAQDataEnableChannel213.Name = "cbDAQDataEnableChannel213"
        Me.cbDAQDataEnableChannel213.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel213.TabIndex = 301
        Me.cbDAQDataEnableChannel213.Text = "213"
        Me.cbDAQDataEnableChannel213.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel202
        '
        Me.cbDAQDataChannel202.Enabled = False
        Me.cbDAQDataChannel202.FormattingEnabled = True
        Me.cbDAQDataChannel202.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel202.Location = New System.Drawing.Point(52, 32)
        Me.cbDAQDataChannel202.Name = "cbDAQDataChannel202"
        Me.cbDAQDataChannel202.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel202.TabIndex = 219
        '
        'cbDAQDataChannel201
        '
        Me.cbDAQDataChannel201.Enabled = False
        Me.cbDAQDataChannel201.FormattingEnabled = True
        Me.cbDAQDataChannel201.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel201.Location = New System.Drawing.Point(52, 5)
        Me.cbDAQDataChannel201.Name = "cbDAQDataChannel201"
        Me.cbDAQDataChannel201.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel201.TabIndex = 59
        '
        'cbDAQDataChannel218
        '
        Me.cbDAQDataChannel218.Enabled = False
        Me.cbDAQDataChannel218.FormattingEnabled = True
        Me.cbDAQDataChannel218.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel218.Location = New System.Drawing.Point(52, 449)
        Me.cbDAQDataChannel218.Name = "cbDAQDataChannel218"
        Me.cbDAQDataChannel218.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel218.TabIndex = 236
        '
        'cbDAQDataChannel213
        '
        Me.cbDAQDataChannel213.Enabled = False
        Me.cbDAQDataChannel213.FormattingEnabled = True
        Me.cbDAQDataChannel213.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel213.Location = New System.Drawing.Point(52, 319)
        Me.cbDAQDataChannel213.Name = "cbDAQDataChannel213"
        Me.cbDAQDataChannel213.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel213.TabIndex = 231
        '
        'cbDAQDataEnableChannel218
        '
        Me.cbDAQDataEnableChannel218.AutoSize = True
        Me.cbDAQDataEnableChannel218.Location = New System.Drawing.Point(5, 450)
        Me.cbDAQDataEnableChannel218.Name = "cbDAQDataEnableChannel218"
        Me.cbDAQDataEnableChannel218.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel218.TabIndex = 306
        Me.cbDAQDataEnableChannel218.Text = "218"
        Me.cbDAQDataEnableChannel218.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel214
        '
        Me.cbDAQDataEnableChannel214.AutoSize = True
        Me.cbDAQDataEnableChannel214.Location = New System.Drawing.Point(5, 346)
        Me.cbDAQDataEnableChannel214.Name = "cbDAQDataEnableChannel214"
        Me.cbDAQDataEnableChannel214.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel214.TabIndex = 302
        Me.cbDAQDataEnableChannel214.Text = "214"
        Me.cbDAQDataEnableChannel214.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel217
        '
        Me.cbDAQDataChannel217.Enabled = False
        Me.cbDAQDataChannel217.FormattingEnabled = True
        Me.cbDAQDataChannel217.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel217.Location = New System.Drawing.Point(52, 423)
        Me.cbDAQDataChannel217.Name = "cbDAQDataChannel217"
        Me.cbDAQDataChannel217.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel217.TabIndex = 235
        '
        'cbDAQDataChannel214
        '
        Me.cbDAQDataChannel214.Enabled = False
        Me.cbDAQDataChannel214.FormattingEnabled = True
        Me.cbDAQDataChannel214.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel214.Location = New System.Drawing.Point(52, 345)
        Me.cbDAQDataChannel214.Name = "cbDAQDataChannel214"
        Me.cbDAQDataChannel214.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel214.TabIndex = 232
        '
        'cbDAQDataEnableChannel217
        '
        Me.cbDAQDataEnableChannel217.AutoSize = True
        Me.cbDAQDataEnableChannel217.Location = New System.Drawing.Point(5, 424)
        Me.cbDAQDataEnableChannel217.Name = "cbDAQDataEnableChannel217"
        Me.cbDAQDataEnableChannel217.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel217.TabIndex = 305
        Me.cbDAQDataEnableChannel217.Text = "217"
        Me.cbDAQDataEnableChannel217.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel215
        '
        Me.cbDAQDataEnableChannel215.AutoSize = True
        Me.cbDAQDataEnableChannel215.Location = New System.Drawing.Point(5, 372)
        Me.cbDAQDataEnableChannel215.Name = "cbDAQDataEnableChannel215"
        Me.cbDAQDataEnableChannel215.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel215.TabIndex = 303
        Me.cbDAQDataEnableChannel215.Text = "215"
        Me.cbDAQDataEnableChannel215.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel216
        '
        Me.cbDAQDataChannel216.Enabled = False
        Me.cbDAQDataChannel216.FormattingEnabled = True
        Me.cbDAQDataChannel216.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel216.Location = New System.Drawing.Point(52, 397)
        Me.cbDAQDataChannel216.Name = "cbDAQDataChannel216"
        Me.cbDAQDataChannel216.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel216.TabIndex = 234
        '
        'cbDAQDataChannel215
        '
        Me.cbDAQDataChannel215.Enabled = False
        Me.cbDAQDataChannel215.FormattingEnabled = True
        Me.cbDAQDataChannel215.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel215.Location = New System.Drawing.Point(52, 371)
        Me.cbDAQDataChannel215.Name = "cbDAQDataChannel215"
        Me.cbDAQDataChannel215.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel215.TabIndex = 233
        '
        'cbDAQDataEnableChannel216
        '
        Me.cbDAQDataEnableChannel216.AutoSize = True
        Me.cbDAQDataEnableChannel216.Location = New System.Drawing.Point(5, 398)
        Me.cbDAQDataEnableChannel216.Name = "cbDAQDataEnableChannel216"
        Me.cbDAQDataEnableChannel216.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel216.TabIndex = 304
        Me.cbDAQDataEnableChannel216.Text = "216"
        Me.cbDAQDataEnableChannel216.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel201
        '
        Me.cbDAQDataEnableChannel201.AutoSize = True
        Me.cbDAQDataEnableChannel201.Location = New System.Drawing.Point(5, 7)
        Me.cbDAQDataEnableChannel201.Name = "cbDAQDataEnableChannel201"
        Me.cbDAQDataEnableChannel201.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel201.TabIndex = 111
        Me.cbDAQDataEnableChannel201.Text = "201"
        Me.cbDAQDataEnableChannel201.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel202
        '
        Me.cbDAQDataEnableChannel202.AutoSize = True
        Me.cbDAQDataEnableChannel202.Location = New System.Drawing.Point(5, 33)
        Me.cbDAQDataEnableChannel202.Name = "cbDAQDataEnableChannel202"
        Me.cbDAQDataEnableChannel202.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel202.TabIndex = 112
        Me.cbDAQDataEnableChannel202.Text = "202"
        Me.cbDAQDataEnableChannel202.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel203
        '
        Me.cbDAQDataEnableChannel203.AutoSize = True
        Me.cbDAQDataEnableChannel203.Location = New System.Drawing.Point(5, 59)
        Me.cbDAQDataEnableChannel203.Name = "cbDAQDataEnableChannel203"
        Me.cbDAQDataEnableChannel203.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel203.TabIndex = 113
        Me.cbDAQDataEnableChannel203.Text = "203"
        Me.cbDAQDataEnableChannel203.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel204
        '
        Me.cbDAQDataEnableChannel204.AutoSize = True
        Me.cbDAQDataEnableChannel204.Location = New System.Drawing.Point(5, 85)
        Me.cbDAQDataEnableChannel204.Name = "cbDAQDataEnableChannel204"
        Me.cbDAQDataEnableChannel204.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel204.TabIndex = 114
        Me.cbDAQDataEnableChannel204.Text = "204"
        Me.cbDAQDataEnableChannel204.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel205
        '
        Me.cbDAQDataEnableChannel205.AutoSize = True
        Me.cbDAQDataEnableChannel205.Location = New System.Drawing.Point(5, 111)
        Me.cbDAQDataEnableChannel205.Name = "cbDAQDataEnableChannel205"
        Me.cbDAQDataEnableChannel205.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel205.TabIndex = 115
        Me.cbDAQDataEnableChannel205.Text = "205"
        Me.cbDAQDataEnableChannel205.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel206
        '
        Me.cbDAQDataEnableChannel206.AutoSize = True
        Me.cbDAQDataEnableChannel206.Location = New System.Drawing.Point(5, 137)
        Me.cbDAQDataEnableChannel206.Name = "cbDAQDataEnableChannel206"
        Me.cbDAQDataEnableChannel206.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel206.TabIndex = 116
        Me.cbDAQDataEnableChannel206.Text = "206"
        Me.cbDAQDataEnableChannel206.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel207
        '
        Me.cbDAQDataEnableChannel207.AutoSize = True
        Me.cbDAQDataEnableChannel207.Location = New System.Drawing.Point(5, 163)
        Me.cbDAQDataEnableChannel207.Name = "cbDAQDataEnableChannel207"
        Me.cbDAQDataEnableChannel207.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel207.TabIndex = 117
        Me.cbDAQDataEnableChannel207.Text = "207"
        Me.cbDAQDataEnableChannel207.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel208
        '
        Me.cbDAQDataEnableChannel208.AutoSize = True
        Me.cbDAQDataEnableChannel208.Location = New System.Drawing.Point(5, 189)
        Me.cbDAQDataEnableChannel208.Name = "cbDAQDataEnableChannel208"
        Me.cbDAQDataEnableChannel208.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel208.TabIndex = 118
        Me.cbDAQDataEnableChannel208.Text = "208"
        Me.cbDAQDataEnableChannel208.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel209
        '
        Me.cbDAQDataEnableChannel209.AutoSize = True
        Me.cbDAQDataEnableChannel209.Location = New System.Drawing.Point(5, 215)
        Me.cbDAQDataEnableChannel209.Name = "cbDAQDataEnableChannel209"
        Me.cbDAQDataEnableChannel209.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel209.TabIndex = 119
        Me.cbDAQDataEnableChannel209.Text = "209"
        Me.cbDAQDataEnableChannel209.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel210
        '
        Me.cbDAQDataEnableChannel210.AutoSize = True
        Me.cbDAQDataEnableChannel210.Location = New System.Drawing.Point(5, 241)
        Me.cbDAQDataEnableChannel210.Name = "cbDAQDataEnableChannel210"
        Me.cbDAQDataEnableChannel210.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel210.TabIndex = 120
        Me.cbDAQDataEnableChannel210.Text = "210"
        Me.cbDAQDataEnableChannel210.UseVisualStyleBackColor = True
        '
        'panelModule3
        '
        Me.panelModule3.AutoScroll = True
        Me.panelModule3.BackColor = System.Drawing.Color.Red
        Me.panelModule3.Controls.Add(Me.txtDAQGain318)
        Me.panelModule3.Controls.Add(Me.txtDAQGain319)
        Me.panelModule3.Controls.Add(Me.txtDAQGain320)
        Me.panelModule3.Controls.Add(Me.txtDAQGain317)
        Me.panelModule3.Controls.Add(Me.txtDAQGain316)
        Me.panelModule3.Controls.Add(Me.txtDAQGain315)
        Me.panelModule3.Controls.Add(Me.txtDAQGain314)
        Me.panelModule3.Controls.Add(Me.txtDAQGain313)
        Me.panelModule3.Controls.Add(Me.txtDAQGain312)
        Me.panelModule3.Controls.Add(Me.txtDAQGain311)
        Me.panelModule3.Controls.Add(Me.txtDAQGain309)
        Me.panelModule3.Controls.Add(Me.txtDAQGain310)
        Me.panelModule3.Controls.Add(Me.txtDAQGain308)
        Me.panelModule3.Controls.Add(Me.txtDAQGain307)
        Me.panelModule3.Controls.Add(Me.txtDAQGain306)
        Me.panelModule3.Controls.Add(Me.txtDAQGain305)
        Me.panelModule3.Controls.Add(Me.txtDAQGain304)
        Me.panelModule3.Controls.Add(Me.txtDAQGain303)
        Me.panelModule3.Controls.Add(Me.txtDAQGain302)
        Me.panelModule3.Controls.Add(Me.txtDAQGain301)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset318)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset319)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset320)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset317)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset316)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset315)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset314)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset313)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset312)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset311)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset309)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset310)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset308)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset307)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset306)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset305)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset304)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset303)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset302)
        Me.panelModule3.Controls.Add(Me.txtDAQOffset301)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel312)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel310)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel320)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel309)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel311)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel308)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel320)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel307)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel311)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel306)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel319)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel305)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel312)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel304)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel319)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel303)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel313)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel302)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel301)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel318)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel313)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel318)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel314)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel317)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel314)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel317)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel315)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel316)
        Me.panelModule3.Controls.Add(Me.cbDAQDataChannel315)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel316)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel301)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel302)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel303)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel304)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel305)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel306)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel307)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel308)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel309)
        Me.panelModule3.Controls.Add(Me.cbDAQDataEnableChannel310)
        Me.panelModule3.Location = New System.Drawing.Point(5, 138)
        Me.panelModule3.Name = "panelModule3"
        Me.panelModule3.Size = New System.Drawing.Size(275, 265)
        Me.panelModule3.TabIndex = 317
        Me.panelModule3.Visible = False
        '
        'txtDAQGain318
        '
        Me.txtDAQGain318.Enabled = False
        Me.txtDAQGain318.Location = New System.Drawing.Point(143, 449)
        Me.txtDAQGain318.Name = "txtDAQGain318"
        Me.txtDAQGain318.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain318.TabIndex = 34
        '
        'txtDAQGain319
        '
        Me.txtDAQGain319.Enabled = False
        Me.txtDAQGain319.Location = New System.Drawing.Point(143, 476)
        Me.txtDAQGain319.Name = "txtDAQGain319"
        Me.txtDAQGain319.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain319.TabIndex = 36
        '
        'txtDAQGain320
        '
        Me.txtDAQGain320.Enabled = False
        Me.txtDAQGain320.Location = New System.Drawing.Point(143, 501)
        Me.txtDAQGain320.Name = "txtDAQGain320"
        Me.txtDAQGain320.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain320.TabIndex = 38
        '
        'txtDAQGain317
        '
        Me.txtDAQGain317.Enabled = False
        Me.txtDAQGain317.Location = New System.Drawing.Point(143, 424)
        Me.txtDAQGain317.Name = "txtDAQGain317"
        Me.txtDAQGain317.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain317.TabIndex = 32
        '
        'txtDAQGain316
        '
        Me.txtDAQGain316.Enabled = False
        Me.txtDAQGain316.Location = New System.Drawing.Point(143, 398)
        Me.txtDAQGain316.Name = "txtDAQGain316"
        Me.txtDAQGain316.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain316.TabIndex = 30
        '
        'txtDAQGain315
        '
        Me.txtDAQGain315.Enabled = False
        Me.txtDAQGain315.Location = New System.Drawing.Point(143, 372)
        Me.txtDAQGain315.Name = "txtDAQGain315"
        Me.txtDAQGain315.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain315.TabIndex = 28
        '
        'txtDAQGain314
        '
        Me.txtDAQGain314.Enabled = False
        Me.txtDAQGain314.Location = New System.Drawing.Point(143, 346)
        Me.txtDAQGain314.Name = "txtDAQGain314"
        Me.txtDAQGain314.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain314.TabIndex = 26
        '
        'txtDAQGain313
        '
        Me.txtDAQGain313.Enabled = False
        Me.txtDAQGain313.Location = New System.Drawing.Point(143, 320)
        Me.txtDAQGain313.Name = "txtDAQGain313"
        Me.txtDAQGain313.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain313.TabIndex = 24
        '
        'txtDAQGain312
        '
        Me.txtDAQGain312.Enabled = False
        Me.txtDAQGain312.Location = New System.Drawing.Point(143, 294)
        Me.txtDAQGain312.Name = "txtDAQGain312"
        Me.txtDAQGain312.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain312.TabIndex = 22
        '
        'txtDAQGain311
        '
        Me.txtDAQGain311.Enabled = False
        Me.txtDAQGain311.Location = New System.Drawing.Point(143, 266)
        Me.txtDAQGain311.Name = "txtDAQGain311"
        Me.txtDAQGain311.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain311.TabIndex = 20
        '
        'txtDAQGain309
        '
        Me.txtDAQGain309.Enabled = False
        Me.txtDAQGain309.Location = New System.Drawing.Point(143, 214)
        Me.txtDAQGain309.Name = "txtDAQGain309"
        Me.txtDAQGain309.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain309.TabIndex = 16
        '
        'txtDAQGain310
        '
        Me.txtDAQGain310.Enabled = False
        Me.txtDAQGain310.Location = New System.Drawing.Point(143, 241)
        Me.txtDAQGain310.Name = "txtDAQGain310"
        Me.txtDAQGain310.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain310.TabIndex = 18
        '
        'txtDAQGain308
        '
        Me.txtDAQGain308.Enabled = False
        Me.txtDAQGain308.Location = New System.Drawing.Point(143, 189)
        Me.txtDAQGain308.Name = "txtDAQGain308"
        Me.txtDAQGain308.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain308.TabIndex = 14
        '
        'txtDAQGain307
        '
        Me.txtDAQGain307.Enabled = False
        Me.txtDAQGain307.Location = New System.Drawing.Point(143, 163)
        Me.txtDAQGain307.Name = "txtDAQGain307"
        Me.txtDAQGain307.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain307.TabIndex = 12
        '
        'txtDAQGain306
        '
        Me.txtDAQGain306.Enabled = False
        Me.txtDAQGain306.Location = New System.Drawing.Point(143, 137)
        Me.txtDAQGain306.Name = "txtDAQGain306"
        Me.txtDAQGain306.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain306.TabIndex = 10
        '
        'txtDAQGain305
        '
        Me.txtDAQGain305.Enabled = False
        Me.txtDAQGain305.Location = New System.Drawing.Point(143, 111)
        Me.txtDAQGain305.Name = "txtDAQGain305"
        Me.txtDAQGain305.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain305.TabIndex = 8
        '
        'txtDAQGain304
        '
        Me.txtDAQGain304.Enabled = False
        Me.txtDAQGain304.Location = New System.Drawing.Point(143, 85)
        Me.txtDAQGain304.Name = "txtDAQGain304"
        Me.txtDAQGain304.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain304.TabIndex = 6
        '
        'txtDAQGain303
        '
        Me.txtDAQGain303.Enabled = False
        Me.txtDAQGain303.Location = New System.Drawing.Point(143, 59)
        Me.txtDAQGain303.Name = "txtDAQGain303"
        Me.txtDAQGain303.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain303.TabIndex = 4
        '
        'txtDAQGain302
        '
        Me.txtDAQGain302.Enabled = False
        Me.txtDAQGain302.Location = New System.Drawing.Point(143, 33)
        Me.txtDAQGain302.Name = "txtDAQGain302"
        Me.txtDAQGain302.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain302.TabIndex = 2
        '
        'txtDAQGain301
        '
        Me.txtDAQGain301.Enabled = False
        Me.txtDAQGain301.Location = New System.Drawing.Point(143, 5)
        Me.txtDAQGain301.Name = "txtDAQGain301"
        Me.txtDAQGain301.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQGain301.TabIndex = 0
        '
        'txtDAQOffset318
        '
        Me.txtDAQOffset318.Enabled = False
        Me.txtDAQOffset318.Location = New System.Drawing.Point(198, 449)
        Me.txtDAQOffset318.Name = "txtDAQOffset318"
        Me.txtDAQOffset318.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset318.TabIndex = 35
        '
        'txtDAQOffset319
        '
        Me.txtDAQOffset319.Enabled = False
        Me.txtDAQOffset319.Location = New System.Drawing.Point(198, 476)
        Me.txtDAQOffset319.Name = "txtDAQOffset319"
        Me.txtDAQOffset319.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset319.TabIndex = 37
        '
        'txtDAQOffset320
        '
        Me.txtDAQOffset320.Enabled = False
        Me.txtDAQOffset320.Location = New System.Drawing.Point(198, 501)
        Me.txtDAQOffset320.Name = "txtDAQOffset320"
        Me.txtDAQOffset320.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset320.TabIndex = 39
        '
        'txtDAQOffset317
        '
        Me.txtDAQOffset317.Enabled = False
        Me.txtDAQOffset317.Location = New System.Drawing.Point(198, 424)
        Me.txtDAQOffset317.Name = "txtDAQOffset317"
        Me.txtDAQOffset317.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset317.TabIndex = 33
        '
        'txtDAQOffset316
        '
        Me.txtDAQOffset316.Enabled = False
        Me.txtDAQOffset316.Location = New System.Drawing.Point(198, 398)
        Me.txtDAQOffset316.Name = "txtDAQOffset316"
        Me.txtDAQOffset316.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset316.TabIndex = 31
        '
        'txtDAQOffset315
        '
        Me.txtDAQOffset315.Enabled = False
        Me.txtDAQOffset315.Location = New System.Drawing.Point(198, 372)
        Me.txtDAQOffset315.Name = "txtDAQOffset315"
        Me.txtDAQOffset315.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset315.TabIndex = 29
        '
        'txtDAQOffset314
        '
        Me.txtDAQOffset314.Enabled = False
        Me.txtDAQOffset314.Location = New System.Drawing.Point(198, 346)
        Me.txtDAQOffset314.Name = "txtDAQOffset314"
        Me.txtDAQOffset314.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset314.TabIndex = 27
        '
        'txtDAQOffset313
        '
        Me.txtDAQOffset313.Enabled = False
        Me.txtDAQOffset313.Location = New System.Drawing.Point(198, 320)
        Me.txtDAQOffset313.Name = "txtDAQOffset313"
        Me.txtDAQOffset313.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset313.TabIndex = 25
        '
        'txtDAQOffset312
        '
        Me.txtDAQOffset312.Enabled = False
        Me.txtDAQOffset312.Location = New System.Drawing.Point(198, 294)
        Me.txtDAQOffset312.Name = "txtDAQOffset312"
        Me.txtDAQOffset312.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset312.TabIndex = 23
        '
        'txtDAQOffset311
        '
        Me.txtDAQOffset311.Enabled = False
        Me.txtDAQOffset311.Location = New System.Drawing.Point(198, 266)
        Me.txtDAQOffset311.Name = "txtDAQOffset311"
        Me.txtDAQOffset311.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset311.TabIndex = 21
        '
        'txtDAQOffset309
        '
        Me.txtDAQOffset309.Enabled = False
        Me.txtDAQOffset309.Location = New System.Drawing.Point(198, 214)
        Me.txtDAQOffset309.Name = "txtDAQOffset309"
        Me.txtDAQOffset309.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset309.TabIndex = 17
        '
        'txtDAQOffset310
        '
        Me.txtDAQOffset310.Enabled = False
        Me.txtDAQOffset310.Location = New System.Drawing.Point(198, 241)
        Me.txtDAQOffset310.Name = "txtDAQOffset310"
        Me.txtDAQOffset310.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset310.TabIndex = 19
        '
        'txtDAQOffset308
        '
        Me.txtDAQOffset308.Enabled = False
        Me.txtDAQOffset308.Location = New System.Drawing.Point(198, 189)
        Me.txtDAQOffset308.Name = "txtDAQOffset308"
        Me.txtDAQOffset308.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset308.TabIndex = 15
        '
        'txtDAQOffset307
        '
        Me.txtDAQOffset307.Enabled = False
        Me.txtDAQOffset307.Location = New System.Drawing.Point(198, 163)
        Me.txtDAQOffset307.Name = "txtDAQOffset307"
        Me.txtDAQOffset307.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset307.TabIndex = 13
        '
        'txtDAQOffset306
        '
        Me.txtDAQOffset306.Enabled = False
        Me.txtDAQOffset306.Location = New System.Drawing.Point(198, 137)
        Me.txtDAQOffset306.Name = "txtDAQOffset306"
        Me.txtDAQOffset306.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset306.TabIndex = 11
        '
        'txtDAQOffset305
        '
        Me.txtDAQOffset305.Enabled = False
        Me.txtDAQOffset305.Location = New System.Drawing.Point(198, 111)
        Me.txtDAQOffset305.Name = "txtDAQOffset305"
        Me.txtDAQOffset305.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset305.TabIndex = 9
        '
        'txtDAQOffset304
        '
        Me.txtDAQOffset304.Enabled = False
        Me.txtDAQOffset304.Location = New System.Drawing.Point(198, 85)
        Me.txtDAQOffset304.Name = "txtDAQOffset304"
        Me.txtDAQOffset304.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset304.TabIndex = 7
        '
        'txtDAQOffset303
        '
        Me.txtDAQOffset303.Enabled = False
        Me.txtDAQOffset303.Location = New System.Drawing.Point(198, 59)
        Me.txtDAQOffset303.Name = "txtDAQOffset303"
        Me.txtDAQOffset303.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset303.TabIndex = 5
        '
        'txtDAQOffset302
        '
        Me.txtDAQOffset302.Enabled = False
        Me.txtDAQOffset302.Location = New System.Drawing.Point(198, 33)
        Me.txtDAQOffset302.Name = "txtDAQOffset302"
        Me.txtDAQOffset302.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset302.TabIndex = 3
        '
        'txtDAQOffset301
        '
        Me.txtDAQOffset301.Enabled = False
        Me.txtDAQOffset301.Location = New System.Drawing.Point(198, 5)
        Me.txtDAQOffset301.Name = "txtDAQOffset301"
        Me.txtDAQOffset301.Size = New System.Drawing.Size(51, 20)
        Me.txtDAQOffset301.TabIndex = 1
        '
        'cbDAQDataChannel312
        '
        Me.cbDAQDataChannel312.Enabled = False
        Me.cbDAQDataChannel312.FormattingEnabled = True
        Me.cbDAQDataChannel312.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel312.Location = New System.Drawing.Point(50, 293)
        Me.cbDAQDataChannel312.Name = "cbDAQDataChannel312"
        Me.cbDAQDataChannel312.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel312.TabIndex = 230
        '
        'cbDAQDataChannel310
        '
        Me.cbDAQDataChannel310.Enabled = False
        Me.cbDAQDataChannel310.FormattingEnabled = True
        Me.cbDAQDataChannel310.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel310.Location = New System.Drawing.Point(50, 240)
        Me.cbDAQDataChannel310.Name = "cbDAQDataChannel310"
        Me.cbDAQDataChannel310.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel310.TabIndex = 227
        '
        'cbDAQDataChannel320
        '
        Me.cbDAQDataChannel320.Enabled = False
        Me.cbDAQDataChannel320.FormattingEnabled = True
        Me.cbDAQDataChannel320.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel320.Location = New System.Drawing.Point(50, 501)
        Me.cbDAQDataChannel320.Name = "cbDAQDataChannel320"
        Me.cbDAQDataChannel320.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel320.TabIndex = 238
        '
        'cbDAQDataChannel309
        '
        Me.cbDAQDataChannel309.Enabled = False
        Me.cbDAQDataChannel309.FormattingEnabled = True
        Me.cbDAQDataChannel309.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel309.Location = New System.Drawing.Point(50, 214)
        Me.cbDAQDataChannel309.Name = "cbDAQDataChannel309"
        Me.cbDAQDataChannel309.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel309.TabIndex = 226
        '
        'cbDAQDataEnableChannel311
        '
        Me.cbDAQDataEnableChannel311.AutoSize = True
        Me.cbDAQDataEnableChannel311.Location = New System.Drawing.Point(5, 268)
        Me.cbDAQDataEnableChannel311.Name = "cbDAQDataEnableChannel311"
        Me.cbDAQDataEnableChannel311.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel311.TabIndex = 299
        Me.cbDAQDataEnableChannel311.Text = "311"
        Me.cbDAQDataEnableChannel311.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel308
        '
        Me.cbDAQDataChannel308.Enabled = False
        Me.cbDAQDataChannel308.FormattingEnabled = True
        Me.cbDAQDataChannel308.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel308.Location = New System.Drawing.Point(50, 188)
        Me.cbDAQDataChannel308.Name = "cbDAQDataChannel308"
        Me.cbDAQDataChannel308.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel308.TabIndex = 225
        '
        'cbDAQDataEnableChannel320
        '
        Me.cbDAQDataEnableChannel320.AutoSize = True
        Me.cbDAQDataEnableChannel320.Location = New System.Drawing.Point(5, 502)
        Me.cbDAQDataEnableChannel320.Name = "cbDAQDataEnableChannel320"
        Me.cbDAQDataEnableChannel320.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel320.TabIndex = 308
        Me.cbDAQDataEnableChannel320.Text = "320"
        Me.cbDAQDataEnableChannel320.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel307
        '
        Me.cbDAQDataChannel307.Enabled = False
        Me.cbDAQDataChannel307.FormattingEnabled = True
        Me.cbDAQDataChannel307.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel307.Location = New System.Drawing.Point(50, 162)
        Me.cbDAQDataChannel307.Name = "cbDAQDataChannel307"
        Me.cbDAQDataChannel307.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel307.TabIndex = 224
        '
        'cbDAQDataChannel311
        '
        Me.cbDAQDataChannel311.Enabled = False
        Me.cbDAQDataChannel311.FormattingEnabled = True
        Me.cbDAQDataChannel311.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel311.Location = New System.Drawing.Point(50, 266)
        Me.cbDAQDataChannel311.Name = "cbDAQDataChannel311"
        Me.cbDAQDataChannel311.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel311.TabIndex = 229
        '
        'cbDAQDataChannel306
        '
        Me.cbDAQDataChannel306.Enabled = False
        Me.cbDAQDataChannel306.FormattingEnabled = True
        Me.cbDAQDataChannel306.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel306.Location = New System.Drawing.Point(50, 136)
        Me.cbDAQDataChannel306.Name = "cbDAQDataChannel306"
        Me.cbDAQDataChannel306.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel306.TabIndex = 223
        '
        'cbDAQDataChannel319
        '
        Me.cbDAQDataChannel319.Enabled = False
        Me.cbDAQDataChannel319.FormattingEnabled = True
        Me.cbDAQDataChannel319.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel319.Location = New System.Drawing.Point(50, 475)
        Me.cbDAQDataChannel319.Name = "cbDAQDataChannel319"
        Me.cbDAQDataChannel319.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel319.TabIndex = 237
        '
        'cbDAQDataChannel305
        '
        Me.cbDAQDataChannel305.Enabled = False
        Me.cbDAQDataChannel305.FormattingEnabled = True
        Me.cbDAQDataChannel305.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel305.Location = New System.Drawing.Point(50, 110)
        Me.cbDAQDataChannel305.Name = "cbDAQDataChannel305"
        Me.cbDAQDataChannel305.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel305.TabIndex = 222
        '
        'cbDAQDataEnableChannel312
        '
        Me.cbDAQDataEnableChannel312.AutoSize = True
        Me.cbDAQDataEnableChannel312.Location = New System.Drawing.Point(5, 294)
        Me.cbDAQDataEnableChannel312.Name = "cbDAQDataEnableChannel312"
        Me.cbDAQDataEnableChannel312.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel312.TabIndex = 300
        Me.cbDAQDataEnableChannel312.Text = "312"
        Me.cbDAQDataEnableChannel312.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel304
        '
        Me.cbDAQDataChannel304.Enabled = False
        Me.cbDAQDataChannel304.FormattingEnabled = True
        Me.cbDAQDataChannel304.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel304.Location = New System.Drawing.Point(50, 84)
        Me.cbDAQDataChannel304.Name = "cbDAQDataChannel304"
        Me.cbDAQDataChannel304.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel304.TabIndex = 221
        '
        'cbDAQDataEnableChannel319
        '
        Me.cbDAQDataEnableChannel319.AutoSize = True
        Me.cbDAQDataEnableChannel319.Location = New System.Drawing.Point(5, 476)
        Me.cbDAQDataEnableChannel319.Name = "cbDAQDataEnableChannel319"
        Me.cbDAQDataEnableChannel319.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel319.TabIndex = 307
        Me.cbDAQDataEnableChannel319.Text = "319"
        Me.cbDAQDataEnableChannel319.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel303
        '
        Me.cbDAQDataChannel303.Enabled = False
        Me.cbDAQDataChannel303.FormattingEnabled = True
        Me.cbDAQDataChannel303.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel303.Location = New System.Drawing.Point(50, 58)
        Me.cbDAQDataChannel303.Name = "cbDAQDataChannel303"
        Me.cbDAQDataChannel303.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel303.TabIndex = 220
        '
        'cbDAQDataEnableChannel313
        '
        Me.cbDAQDataEnableChannel313.AutoSize = True
        Me.cbDAQDataEnableChannel313.Location = New System.Drawing.Point(5, 320)
        Me.cbDAQDataEnableChannel313.Name = "cbDAQDataEnableChannel313"
        Me.cbDAQDataEnableChannel313.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel313.TabIndex = 301
        Me.cbDAQDataEnableChannel313.Text = "313"
        Me.cbDAQDataEnableChannel313.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel302
        '
        Me.cbDAQDataChannel302.Enabled = False
        Me.cbDAQDataChannel302.FormattingEnabled = True
        Me.cbDAQDataChannel302.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel302.Location = New System.Drawing.Point(50, 32)
        Me.cbDAQDataChannel302.Name = "cbDAQDataChannel302"
        Me.cbDAQDataChannel302.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel302.TabIndex = 219
        '
        'cbDAQDataChannel301
        '
        Me.cbDAQDataChannel301.Enabled = False
        Me.cbDAQDataChannel301.FormattingEnabled = True
        Me.cbDAQDataChannel301.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel301.Location = New System.Drawing.Point(50, 5)
        Me.cbDAQDataChannel301.Name = "cbDAQDataChannel301"
        Me.cbDAQDataChannel301.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel301.TabIndex = 59
        '
        'cbDAQDataChannel318
        '
        Me.cbDAQDataChannel318.Enabled = False
        Me.cbDAQDataChannel318.FormattingEnabled = True
        Me.cbDAQDataChannel318.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel318.Location = New System.Drawing.Point(50, 449)
        Me.cbDAQDataChannel318.Name = "cbDAQDataChannel318"
        Me.cbDAQDataChannel318.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel318.TabIndex = 236
        '
        'cbDAQDataChannel313
        '
        Me.cbDAQDataChannel313.Enabled = False
        Me.cbDAQDataChannel313.FormattingEnabled = True
        Me.cbDAQDataChannel313.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel313.Location = New System.Drawing.Point(50, 319)
        Me.cbDAQDataChannel313.Name = "cbDAQDataChannel313"
        Me.cbDAQDataChannel313.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel313.TabIndex = 231
        '
        'cbDAQDataEnableChannel318
        '
        Me.cbDAQDataEnableChannel318.AutoSize = True
        Me.cbDAQDataEnableChannel318.Location = New System.Drawing.Point(5, 450)
        Me.cbDAQDataEnableChannel318.Name = "cbDAQDataEnableChannel318"
        Me.cbDAQDataEnableChannel318.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel318.TabIndex = 306
        Me.cbDAQDataEnableChannel318.Text = "318"
        Me.cbDAQDataEnableChannel318.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel314
        '
        Me.cbDAQDataEnableChannel314.AutoSize = True
        Me.cbDAQDataEnableChannel314.Location = New System.Drawing.Point(5, 346)
        Me.cbDAQDataEnableChannel314.Name = "cbDAQDataEnableChannel314"
        Me.cbDAQDataEnableChannel314.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel314.TabIndex = 302
        Me.cbDAQDataEnableChannel314.Text = "314"
        Me.cbDAQDataEnableChannel314.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel317
        '
        Me.cbDAQDataChannel317.Enabled = False
        Me.cbDAQDataChannel317.FormattingEnabled = True
        Me.cbDAQDataChannel317.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel317.Location = New System.Drawing.Point(50, 423)
        Me.cbDAQDataChannel317.Name = "cbDAQDataChannel317"
        Me.cbDAQDataChannel317.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel317.TabIndex = 235
        '
        'cbDAQDataChannel314
        '
        Me.cbDAQDataChannel314.Enabled = False
        Me.cbDAQDataChannel314.FormattingEnabled = True
        Me.cbDAQDataChannel314.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel314.Location = New System.Drawing.Point(50, 345)
        Me.cbDAQDataChannel314.Name = "cbDAQDataChannel314"
        Me.cbDAQDataChannel314.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel314.TabIndex = 232
        '
        'cbDAQDataEnableChannel317
        '
        Me.cbDAQDataEnableChannel317.AutoSize = True
        Me.cbDAQDataEnableChannel317.Location = New System.Drawing.Point(5, 424)
        Me.cbDAQDataEnableChannel317.Name = "cbDAQDataEnableChannel317"
        Me.cbDAQDataEnableChannel317.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel317.TabIndex = 305
        Me.cbDAQDataEnableChannel317.Text = "317"
        Me.cbDAQDataEnableChannel317.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel315
        '
        Me.cbDAQDataEnableChannel315.AutoSize = True
        Me.cbDAQDataEnableChannel315.Location = New System.Drawing.Point(5, 372)
        Me.cbDAQDataEnableChannel315.Name = "cbDAQDataEnableChannel315"
        Me.cbDAQDataEnableChannel315.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel315.TabIndex = 303
        Me.cbDAQDataEnableChannel315.Text = "315"
        Me.cbDAQDataEnableChannel315.UseVisualStyleBackColor = True
        '
        'cbDAQDataChannel316
        '
        Me.cbDAQDataChannel316.Enabled = False
        Me.cbDAQDataChannel316.FormattingEnabled = True
        Me.cbDAQDataChannel316.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel316.Location = New System.Drawing.Point(50, 397)
        Me.cbDAQDataChannel316.Name = "cbDAQDataChannel316"
        Me.cbDAQDataChannel316.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel316.TabIndex = 234
        '
        'cbDAQDataChannel315
        '
        Me.cbDAQDataChannel315.Enabled = False
        Me.cbDAQDataChannel315.FormattingEnabled = True
        Me.cbDAQDataChannel315.Items.AddRange(New Object() {"", "A Jaw Line", "A Jaw Load", "B Jaw Line", "B Jaw Load", "C Jaw Line", "C Jaw Load", "A Coil", "B Coil", "C Coil", "A1", "B1", "C1", "A2", "B2", "C2", "A3", "B3", "C3", "Ambient"})
        Me.cbDAQDataChannel315.Location = New System.Drawing.Point(50, 371)
        Me.cbDAQDataChannel315.Name = "cbDAQDataChannel315"
        Me.cbDAQDataChannel315.Size = New System.Drawing.Size(85, 21)
        Me.cbDAQDataChannel315.TabIndex = 233
        '
        'cbDAQDataEnableChannel316
        '
        Me.cbDAQDataEnableChannel316.AutoSize = True
        Me.cbDAQDataEnableChannel316.Location = New System.Drawing.Point(5, 398)
        Me.cbDAQDataEnableChannel316.Name = "cbDAQDataEnableChannel316"
        Me.cbDAQDataEnableChannel316.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel316.TabIndex = 304
        Me.cbDAQDataEnableChannel316.Text = "316"
        Me.cbDAQDataEnableChannel316.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel301
        '
        Me.cbDAQDataEnableChannel301.AutoSize = True
        Me.cbDAQDataEnableChannel301.Location = New System.Drawing.Point(5, 7)
        Me.cbDAQDataEnableChannel301.Name = "cbDAQDataEnableChannel301"
        Me.cbDAQDataEnableChannel301.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel301.TabIndex = 111
        Me.cbDAQDataEnableChannel301.Text = "301"
        Me.cbDAQDataEnableChannel301.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel302
        '
        Me.cbDAQDataEnableChannel302.AutoSize = True
        Me.cbDAQDataEnableChannel302.Location = New System.Drawing.Point(5, 33)
        Me.cbDAQDataEnableChannel302.Name = "cbDAQDataEnableChannel302"
        Me.cbDAQDataEnableChannel302.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel302.TabIndex = 112
        Me.cbDAQDataEnableChannel302.Text = "302"
        Me.cbDAQDataEnableChannel302.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel303
        '
        Me.cbDAQDataEnableChannel303.AutoSize = True
        Me.cbDAQDataEnableChannel303.Location = New System.Drawing.Point(5, 59)
        Me.cbDAQDataEnableChannel303.Name = "cbDAQDataEnableChannel303"
        Me.cbDAQDataEnableChannel303.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel303.TabIndex = 113
        Me.cbDAQDataEnableChannel303.Text = "303"
        Me.cbDAQDataEnableChannel303.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel304
        '
        Me.cbDAQDataEnableChannel304.AutoSize = True
        Me.cbDAQDataEnableChannel304.Location = New System.Drawing.Point(5, 85)
        Me.cbDAQDataEnableChannel304.Name = "cbDAQDataEnableChannel304"
        Me.cbDAQDataEnableChannel304.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel304.TabIndex = 114
        Me.cbDAQDataEnableChannel304.Text = "304"
        Me.cbDAQDataEnableChannel304.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel305
        '
        Me.cbDAQDataEnableChannel305.AutoSize = True
        Me.cbDAQDataEnableChannel305.Location = New System.Drawing.Point(5, 111)
        Me.cbDAQDataEnableChannel305.Name = "cbDAQDataEnableChannel305"
        Me.cbDAQDataEnableChannel305.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel305.TabIndex = 115
        Me.cbDAQDataEnableChannel305.Text = "305"
        Me.cbDAQDataEnableChannel305.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel306
        '
        Me.cbDAQDataEnableChannel306.AutoSize = True
        Me.cbDAQDataEnableChannel306.Location = New System.Drawing.Point(5, 137)
        Me.cbDAQDataEnableChannel306.Name = "cbDAQDataEnableChannel306"
        Me.cbDAQDataEnableChannel306.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel306.TabIndex = 116
        Me.cbDAQDataEnableChannel306.Text = "306"
        Me.cbDAQDataEnableChannel306.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel307
        '
        Me.cbDAQDataEnableChannel307.AutoSize = True
        Me.cbDAQDataEnableChannel307.Location = New System.Drawing.Point(5, 163)
        Me.cbDAQDataEnableChannel307.Name = "cbDAQDataEnableChannel307"
        Me.cbDAQDataEnableChannel307.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel307.TabIndex = 117
        Me.cbDAQDataEnableChannel307.Text = "307"
        Me.cbDAQDataEnableChannel307.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel308
        '
        Me.cbDAQDataEnableChannel308.AutoSize = True
        Me.cbDAQDataEnableChannel308.Location = New System.Drawing.Point(5, 189)
        Me.cbDAQDataEnableChannel308.Name = "cbDAQDataEnableChannel308"
        Me.cbDAQDataEnableChannel308.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel308.TabIndex = 118
        Me.cbDAQDataEnableChannel308.Text = "308"
        Me.cbDAQDataEnableChannel308.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel309
        '
        Me.cbDAQDataEnableChannel309.AutoSize = True
        Me.cbDAQDataEnableChannel309.Location = New System.Drawing.Point(5, 215)
        Me.cbDAQDataEnableChannel309.Name = "cbDAQDataEnableChannel309"
        Me.cbDAQDataEnableChannel309.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel309.TabIndex = 119
        Me.cbDAQDataEnableChannel309.Text = "309"
        Me.cbDAQDataEnableChannel309.UseVisualStyleBackColor = True
        '
        'cbDAQDataEnableChannel310
        '
        Me.cbDAQDataEnableChannel310.AutoSize = True
        Me.cbDAQDataEnableChannel310.Location = New System.Drawing.Point(5, 241)
        Me.cbDAQDataEnableChannel310.Name = "cbDAQDataEnableChannel310"
        Me.cbDAQDataEnableChannel310.Size = New System.Drawing.Size(44, 17)
        Me.cbDAQDataEnableChannel310.TabIndex = 120
        Me.cbDAQDataEnableChannel310.Text = "310"
        Me.cbDAQDataEnableChannel310.UseVisualStyleBackColor = True
        '
        'lblTime
        '
        Me.lblTime.AutoSize = True
        Me.lblTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTime.Location = New System.Drawing.Point(1018, 16)
        Me.lblTime.Name = "lblTime"
        Me.lblTime.Size = New System.Drawing.Size(43, 20)
        Me.lblTime.TabIndex = 336
        Me.lblTime.Text = "Time"
        '
        'chbAutomaticShutDown
        '
        Me.chbAutomaticShutDown.Location = New System.Drawing.Point(307, 98)
        Me.chbAutomaticShutDown.Name = "chbAutomaticShutDown"
        Me.chbAutomaticShutDown.Size = New System.Drawing.Size(420, 21)
        Me.chbAutomaticShutDown.TabIndex = 335
        Me.chbAutomaticShutDown.Text = "Turn Off Power Source When Test Is Done"
        Me.chbAutomaticShutDown.UseVisualStyleBackColor = True
        '
        'gbControlSettings
        '
        Me.gbControlSettings.Controls.Add(Me.lblAccuracyDeadBand)
        Me.gbControlSettings.Controls.Add(Me.txtVoltage_ControlDeadBand)
        Me.gbControlSettings.Controls.Add(Me.lblControlDeadBand)
        Me.gbControlSettings.Controls.Add(Me.cbVoltage_CurrentSetpoint)
        Me.gbControlSettings.Controls.Add(Me.chkCloseLoop)
        Me.gbControlSettings.Controls.Add(Me.txtVoltage_AccuracyDeadband)
        Me.gbControlSettings.Location = New System.Drawing.Point(401, 4)
        Me.gbControlSettings.Name = "gbControlSettings"
        Me.gbControlSettings.Size = New System.Drawing.Size(255, 92)
        Me.gbControlSettings.TabIndex = 334
        Me.gbControlSettings.TabStop = False
        Me.gbControlSettings.Text = "Control Settings"
        '
        'lblAccuracyDeadBand
        '
        Me.lblAccuracyDeadBand.AutoSize = True
        Me.lblAccuracyDeadBand.Location = New System.Drawing.Point(83, 69)
        Me.lblAccuracyDeadBand.Name = "lblAccuracyDeadBand"
        Me.lblAccuracyDeadBand.Size = New System.Drawing.Size(161, 13)
        Me.lblAccuracyDeadBand.TabIndex = 332
        Me.lblAccuracyDeadBand.Text = "Accuracy Dead Band %Fullscale"
        '
        'txtVoltage_ControlDeadBand
        '
        Me.txtVoltage_ControlDeadBand.Location = New System.Drawing.Point(9, 42)
        Me.txtVoltage_ControlDeadBand.Name = "txtVoltage_ControlDeadBand"
        Me.txtVoltage_ControlDeadBand.Size = New System.Drawing.Size(62, 20)
        Me.txtVoltage_ControlDeadBand.TabIndex = 329
        Me.txtVoltage_ControlDeadBand.Text = "5.0"
        Me.txtVoltage_ControlDeadBand.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblControlDeadBand
        '
        Me.lblControlDeadBand.AutoSize = True
        Me.lblControlDeadBand.Location = New System.Drawing.Point(83, 46)
        Me.lblControlDeadBand.Name = "lblControlDeadBand"
        Me.lblControlDeadBand.Size = New System.Drawing.Size(159, 13)
        Me.lblControlDeadBand.TabIndex = 330
        Me.lblControlDeadBand.Text = "Controller Deadband % Fullscale"
        '
        'cbVoltage_CurrentSetpoint
        '
        Me.cbVoltage_CurrentSetpoint.FormattingEnabled = True
        Me.cbVoltage_CurrentSetpoint.Items.AddRange(New Object() {"100", "200", "320"})
        Me.cbVoltage_CurrentSetpoint.Location = New System.Drawing.Point(9, 18)
        Me.cbVoltage_CurrentSetpoint.Name = "cbVoltage_CurrentSetpoint"
        Me.cbVoltage_CurrentSetpoint.Size = New System.Drawing.Size(62, 21)
        Me.cbVoltage_CurrentSetpoint.Sorted = True
        Me.cbVoltage_CurrentSetpoint.TabIndex = 327
        '
        'chkCloseLoop
        '
        Me.chkCloseLoop.AutoSize = True
        Me.chkCloseLoop.Location = New System.Drawing.Point(86, 22)
        Me.chkCloseLoop.Name = "chkCloseLoop"
        Me.chkCloseLoop.Size = New System.Drawing.Size(116, 17)
        Me.chkCloseLoop.TabIndex = 328
        Me.chkCloseLoop.Text = "Close Current Loop"
        Me.chkCloseLoop.UseVisualStyleBackColor = True
        '
        'txtVoltage_AccuracyDeadband
        '
        Me.txtVoltage_AccuracyDeadband.Location = New System.Drawing.Point(9, 66)
        Me.txtVoltage_AccuracyDeadband.Name = "txtVoltage_AccuracyDeadband"
        Me.txtVoltage_AccuracyDeadband.Size = New System.Drawing.Size(62, 20)
        Me.txtVoltage_AccuracyDeadband.TabIndex = 331
        Me.txtVoltage_AccuracyDeadband.Text = "0.1"
        Me.txtVoltage_AccuracyDeadband.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'gbDAQVoltage
        '
        Me.gbDAQVoltage.Controls.Add(Me.Label8)
        Me.gbDAQVoltage.Controls.Add(Me.btnVoltageOff)
        Me.gbDAQVoltage.Controls.Add(Me.btnSetVoltage)
        Me.gbDAQVoltage.Controls.Add(Me.txtSetVoltage)
        Me.gbDAQVoltage.Location = New System.Drawing.Point(301, 5)
        Me.gbDAQVoltage.Name = "gbDAQVoltage"
        Me.gbDAQVoltage.Size = New System.Drawing.Size(94, 92)
        Me.gbDAQVoltage.TabIndex = 333
        Me.gbDAQVoltage.TabStop = False
        Me.gbDAQVoltage.Text = "Voltage"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(62, 19)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(14, 13)
        Me.Label8.TabIndex = 327
        Me.Label8.Text = "V"
        '
        'btnVoltageOff
        '
        Me.btnVoltageOff.Location = New System.Drawing.Point(13, 65)
        Me.btnVoltageOff.Name = "btnVoltageOff"
        Me.btnVoltageOff.Size = New System.Drawing.Size(71, 22)
        Me.btnVoltageOff.TabIndex = 326
        Me.btnVoltageOff.Text = "Voltage Off"
        '
        'btnSetVoltage
        '
        Me.btnSetVoltage.Location = New System.Drawing.Point(12, 39)
        Me.btnSetVoltage.Name = "btnSetVoltage"
        Me.btnSetVoltage.Size = New System.Drawing.Size(72, 24)
        Me.btnSetVoltage.TabIndex = 325
        Me.btnSetVoltage.Text = "Set &Voltage"
        '
        'txtSetVoltage
        '
        Me.txtSetVoltage.Location = New System.Drawing.Point(13, 16)
        Me.txtSetVoltage.Name = "txtSetVoltage"
        Me.txtSetVoltage.Size = New System.Drawing.Size(45, 20)
        Me.txtSetVoltage.TabIndex = 324
        Me.txtSetVoltage.Text = "110"
        Me.txtSetVoltage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'btnSaveConfig
        '
        Me.btnSaveConfig.Location = New System.Drawing.Point(1102, 211)
        Me.btnSaveConfig.Name = "btnSaveConfig"
        Me.btnSaveConfig.Size = New System.Drawing.Size(80, 24)
        Me.btnSaveConfig.TabIndex = 341
        Me.btnSaveConfig.Text = "Save"
        Me.btnSaveConfig.UseVisualStyleBackColor = True
        Me.btnSaveConfig.Visible = False
        '
        'btnLoadConfig
        '
        Me.btnLoadConfig.Location = New System.Drawing.Point(1016, 211)
        Me.btnLoadConfig.Name = "btnLoadConfig"
        Me.btnLoadConfig.Size = New System.Drawing.Size(80, 24)
        Me.btnLoadConfig.TabIndex = 342
        Me.btnLoadConfig.Text = "Load"
        Me.btnLoadConfig.UseVisualStyleBackColor = True
        Me.btnLoadConfig.Visible = False
        '
        'gbRadianRS232
        '
        Me.gbRadianRS232.Controls.Add(Me.cbMeter_COMPorts)
        Me.gbRadianRS232.Controls.Add(Me.lblMeterComPort)
        Me.gbRadianRS232.Controls.Add(Me.lblMeterComBaudRate)
        Me.gbRadianRS232.Controls.Add(Me.txtMeterComBaudrate)
        Me.gbRadianRS232.Controls.Add(Me.btnConnect)
        Me.gbRadianRS232.Location = New System.Drawing.Point(8, 419)
        Me.gbRadianRS232.Name = "gbRadianRS232"
        Me.gbRadianRS232.Size = New System.Drawing.Size(239, 65)
        Me.gbRadianRS232.TabIndex = 321
        Me.gbRadianRS232.TabStop = False
        Me.gbRadianRS232.Text = "Radian Standard"
        '
        'cbMeter_COMPorts
        '
        Me.cbMeter_COMPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMeter_COMPorts.FormattingEnabled = True
        Me.cbMeter_COMPorts.Items.AddRange(New Object() {"1"})
        Me.cbMeter_COMPorts.Location = New System.Drawing.Point(65, 34)
        Me.cbMeter_COMPorts.Name = "cbMeter_COMPorts"
        Me.cbMeter_COMPorts.Size = New System.Drawing.Size(45, 21)
        Me.cbMeter_COMPorts.TabIndex = 63
        '
        'lblMeterComPort
        '
        Me.lblMeterComPort.Location = New System.Drawing.Point(62, 16)
        Me.lblMeterComPort.Name = "lblMeterComPort"
        Me.lblMeterComPort.Size = New System.Drawing.Size(48, 15)
        Me.lblMeterComPort.TabIndex = 62
        Me.lblMeterComPort.Text = "Port check"
        '
        'lblMeterComBaudRate
        '
        Me.lblMeterComBaudRate.Location = New System.Drawing.Point(6, 16)
        Me.lblMeterComBaudRate.Name = "lblMeterComBaudRate"
        Me.lblMeterComBaudRate.Size = New System.Drawing.Size(65, 14)
        Me.lblMeterComBaudRate.TabIndex = 60
        Me.lblMeterComBaudRate.Text = "BaudRate"
        '
        'txtMeterComBaudrate
        '
        Me.txtMeterComBaudrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtMeterComBaudrate.Location = New System.Drawing.Point(9, 34)
        Me.txtMeterComBaudrate.Name = "txtMeterComBaudrate"
        Me.txtMeterComBaudrate.Size = New System.Drawing.Size(49, 20)
        Me.txtMeterComBaudrate.TabIndex = 61
        Me.txtMeterComBaudrate.Text = "57600"
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(116, 33)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(91, 24)
        Me.btnConnect.TabIndex = 49
        Me.btnConnect.Text = "Connect"
        '
        'gbDAQNumberofReadings
        '
        Me.gbDAQNumberofReadings.Controls.Add(Me.lblDAQTotally)
        Me.gbDAQNumberofReadings.Controls.Add(Me.lblDAQReadings)
        Me.gbDAQNumberofReadings.Controls.Add(Me.txtDAQNumberofReadings)
        Me.gbDAQNumberofReadings.Location = New System.Drawing.Point(1013, 82)
        Me.gbDAQNumberofReadings.Name = "gbDAQNumberofReadings"
        Me.gbDAQNumberofReadings.Size = New System.Drawing.Size(253, 56)
        Me.gbDAQNumberofReadings.TabIndex = 271
        Me.gbDAQNumberofReadings.TabStop = False
        Me.gbDAQNumberofReadings.Text = "# of Readings"
        Me.gbDAQNumberofReadings.Visible = False
        '
        'lblDAQTotally
        '
        Me.lblDAQTotally.AutoSize = True
        Me.lblDAQTotally.Location = New System.Drawing.Point(9, 28)
        Me.lblDAQTotally.Name = "lblDAQTotally"
        Me.lblDAQTotally.Size = New System.Drawing.Size(31, 13)
        Me.lblDAQTotally.TabIndex = 2
        Me.lblDAQTotally.Text = "Total"
        '
        'lblDAQReadings
        '
        Me.lblDAQReadings.AutoSize = True
        Me.lblDAQReadings.Location = New System.Drawing.Point(107, 28)
        Me.lblDAQReadings.Name = "lblDAQReadings"
        Me.lblDAQReadings.Size = New System.Drawing.Size(52, 13)
        Me.lblDAQReadings.TabIndex = 1
        Me.lblDAQReadings.Text = "Readings"
        '
        'txtDAQNumberofReadings
        '
        Me.txtDAQNumberofReadings.Location = New System.Drawing.Point(53, 25)
        Me.txtDAQNumberofReadings.Name = "txtDAQNumberofReadings"
        Me.txtDAQNumberofReadings.Size = New System.Drawing.Size(48, 20)
        Me.txtDAQNumberofReadings.TabIndex = 0
        '
        'txtMachineState
        '
        Me.txtMachineState.BackColor = System.Drawing.Color.Bisque
        Me.txtMachineState.Font = New System.Drawing.Font("Elephant", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMachineState.ForeColor = System.Drawing.Color.Coral
        Me.txtMachineState.Location = New System.Drawing.Point(788, 69)
        Me.txtMachineState.Name = "txtMachineState"
        Me.txtMachineState.Size = New System.Drawing.Size(222, 22)
        Me.txtMachineState.TabIndex = 286
        Me.txtMachineState.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'gbCountDown
        '
        Me.gbCountDown.Controls.Add(Me.txtDAQCountDownHH)
        Me.gbCountDown.Controls.Add(Me.Label76)
        Me.gbCountDown.Controls.Add(Me.txtDAQCountDownMM)
        Me.gbCountDown.Controls.Add(Me.Label75)
        Me.gbCountDown.Controls.Add(Me.txtDAQCountDownSS)
        Me.gbCountDown.Controls.Add(Me.Label74)
        Me.gbCountDown.Location = New System.Drawing.Point(1013, 81)
        Me.gbCountDown.Name = "gbCountDown"
        Me.gbCountDown.Size = New System.Drawing.Size(253, 56)
        Me.gbCountDown.TabIndex = 265
        Me.gbCountDown.TabStop = False
        Me.gbCountDown.Text = "Duration"
        Me.gbCountDown.Visible = False
        '
        'txtDAQCountDownHH
        '
        Me.txtDAQCountDownHH.Location = New System.Drawing.Point(12, 28)
        Me.txtDAQCountDownHH.Name = "txtDAQCountDownHH"
        Me.txtDAQCountDownHH.Size = New System.Drawing.Size(24, 20)
        Me.txtDAQCountDownHH.TabIndex = 123
        '
        'Label76
        '
        Me.Label76.AutoSize = True
        Me.Label76.Location = New System.Drawing.Point(185, 31)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(49, 13)
        Me.Label76.TabIndex = 128
        Me.Label76.Text = "Seconds"
        '
        'txtDAQCountDownMM
        '
        Me.txtDAQCountDownMM.Location = New System.Drawing.Point(78, 28)
        Me.txtDAQCountDownMM.Name = "txtDAQCountDownMM"
        Me.txtDAQCountDownMM.Size = New System.Drawing.Size(24, 20)
        Me.txtDAQCountDownMM.TabIndex = 124
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.Location = New System.Drawing.Point(105, 31)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(44, 13)
        Me.Label75.TabIndex = 127
        Me.Label75.Text = "Minutes"
        '
        'txtDAQCountDownSS
        '
        Me.txtDAQCountDownSS.Location = New System.Drawing.Point(157, 28)
        Me.txtDAQCountDownSS.Name = "txtDAQCountDownSS"
        Me.txtDAQCountDownSS.Size = New System.Drawing.Size(24, 20)
        Me.txtDAQCountDownSS.TabIndex = 125
        '
        'Label74
        '
        Me.Label74.AutoSize = True
        Me.Label74.Location = New System.Drawing.Point(39, 31)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(35, 13)
        Me.Label74.TabIndex = 126
        Me.Label74.Text = "Hours"
        '
        'gbDAQDataGrid
        '
        Me.gbDAQDataGrid.Controls.Add(Me.txtDataLoggerHeader)
        Me.gbDAQDataGrid.Controls.Add(Me.chkDataLoggerPhase)
        Me.gbDAQDataGrid.Controls.Add(Me.chkDataLoggerFrequency)
        Me.gbDAQDataGrid.Controls.Add(Me.chkDataLoggerCurrent)
        Me.gbDAQDataGrid.Controls.Add(Me.chkDataLoggerVolts)
        Me.gbDAQDataGrid.Controls.Add(Me.lbDataLog)
        Me.gbDAQDataGrid.Controls.Add(Me.dgvDAQData)
        Me.gbDAQDataGrid.Location = New System.Drawing.Point(301, 219)
        Me.gbDAQDataGrid.Name = "gbDAQDataGrid"
        Me.gbDAQDataGrid.Size = New System.Drawing.Size(673, 444)
        Me.gbDAQDataGrid.TabIndex = 313
        Me.gbDAQDataGrid.TabStop = False
        Me.gbDAQDataGrid.Text = "Data"
        '
        'txtDataLoggerHeader
        '
        Me.txtDataLoggerHeader.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtDataLoggerHeader.Location = New System.Drawing.Point(6, 315)
        Me.txtDataLoggerHeader.Name = "txtDataLoggerHeader"
        Me.txtDataLoggerHeader.Size = New System.Drawing.Size(511, 13)
        Me.txtDataLoggerHeader.TabIndex = 348
        Me.txtDataLoggerHeader.Text = "Sample"
        '
        'chkDataLoggerPhase
        '
        Me.chkDataLoggerPhase.AutoSize = True
        Me.chkDataLoggerPhase.Location = New System.Drawing.Point(556, 398)
        Me.chkDataLoggerPhase.Name = "chkDataLoggerPhase"
        Me.chkDataLoggerPhase.Size = New System.Drawing.Size(56, 17)
        Me.chkDataLoggerPhase.TabIndex = 347
        Me.chkDataLoggerPhase.Text = "Phase"
        Me.chkDataLoggerPhase.UseVisualStyleBackColor = True
        '
        'chkDataLoggerFrequency
        '
        Me.chkDataLoggerFrequency.AutoSize = True
        Me.chkDataLoggerFrequency.Location = New System.Drawing.Point(556, 381)
        Me.chkDataLoggerFrequency.Name = "chkDataLoggerFrequency"
        Me.chkDataLoggerFrequency.Size = New System.Drawing.Size(76, 17)
        Me.chkDataLoggerFrequency.TabIndex = 346
        Me.chkDataLoggerFrequency.Text = "Frequency"
        Me.chkDataLoggerFrequency.UseVisualStyleBackColor = True
        '
        'chkDataLoggerCurrent
        '
        Me.chkDataLoggerCurrent.AutoSize = True
        Me.chkDataLoggerCurrent.Location = New System.Drawing.Point(556, 365)
        Me.chkDataLoggerCurrent.Name = "chkDataLoggerCurrent"
        Me.chkDataLoggerCurrent.Size = New System.Drawing.Size(60, 17)
        Me.chkDataLoggerCurrent.TabIndex = 345
        Me.chkDataLoggerCurrent.Text = "Current"
        Me.chkDataLoggerCurrent.UseVisualStyleBackColor = True
        '
        'chkDataLoggerVolts
        '
        Me.chkDataLoggerVolts.AutoSize = True
        Me.chkDataLoggerVolts.Location = New System.Drawing.Point(556, 347)
        Me.chkDataLoggerVolts.Name = "chkDataLoggerVolts"
        Me.chkDataLoggerVolts.Size = New System.Drawing.Size(49, 17)
        Me.chkDataLoggerVolts.TabIndex = 344
        Me.chkDataLoggerVolts.Text = "Volts"
        Me.chkDataLoggerVolts.UseVisualStyleBackColor = True
        '
        'lbDataLog
        '
        Me.lbDataLog.FormattingEnabled = True
        Me.lbDataLog.Location = New System.Drawing.Point(6, 330)
        Me.lbDataLog.Name = "lbDataLog"
        Me.lbDataLog.ScrollAlwaysVisible = True
        Me.lbDataLog.Size = New System.Drawing.Size(538, 108)
        Me.lbDataLog.TabIndex = 343
        '
        'dgvDAQData
        '
        Me.dgvDAQData.AllowUserToAddRows = False
        Me.dgvDAQData.AllowUserToDeleteRows = False
        Me.dgvDAQData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvDAQData.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvDAQData.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvDAQData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvDAQData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.currentReading, Me.maxReading, Me.minReading, Me.averageReading, Me.tempRisen})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvDAQData.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgvDAQData.Location = New System.Drawing.Point(6, 18)
        Me.dgvDAQData.Name = "dgvDAQData"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvDAQData.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.dgvDAQData.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.dgvDAQData.RowsDefaultCellStyle = DataGridViewCellStyle4
        Me.dgvDAQData.Size = New System.Drawing.Size(657, 295)
        Me.dgvDAQData.TabIndex = 0
        '
        'currentReading
        '
        Me.currentReading.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.currentReading.HeaderText = "Instantaneous Reading"
        Me.currentReading.Name = "currentReading"
        Me.currentReading.ReadOnly = True
        '
        'maxReading
        '
        Me.maxReading.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.maxReading.HeaderText = "Maximum Reading"
        Me.maxReading.Name = "maxReading"
        Me.maxReading.ReadOnly = True
        '
        'minReading
        '
        Me.minReading.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.minReading.HeaderText = "Minimum Reading"
        Me.minReading.Name = "minReading"
        Me.minReading.ReadOnly = True
        '
        'averageReading
        '
        Me.averageReading.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.averageReading.HeaderText = "Average Reading"
        Me.averageReading.Name = "averageReading"
        Me.averageReading.ReadOnly = True
        '
        'tempRisen
        '
        Me.tempRisen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.tempRisen.HeaderText = "Temperature Risen"
        Me.tempRisen.Name = "tempRisen"
        Me.tempRisen.ReadOnly = True
        '
        'pnThreshold
        '
        Me.pnThreshold.Controls.Add(Me.txtDAQCompareThreshold5)
        Me.pnThreshold.Controls.Add(Me.txtDAQCompareThreshold4)
        Me.pnThreshold.Controls.Add(Me.cbDAQCompare1_Check)
        Me.pnThreshold.Controls.Add(Me.txtDAQCompareThreshold3)
        Me.pnThreshold.Controls.Add(Me.cbDAQCompare2_Check)
        Me.pnThreshold.Controls.Add(Me.txtDAQCompareThreshold2)
        Me.pnThreshold.Controls.Add(Me.cbDAQCompare3_Check)
        Me.pnThreshold.Controls.Add(Me.txtDAQCompareThreshold1)
        Me.pnThreshold.Controls.Add(Me.cbDAQCompare4_Check)
        Me.pnThreshold.Controls.Add(Me.cbDAQCompare5_Check)
        Me.pnThreshold.Location = New System.Drawing.Point(987, 367)
        Me.pnThreshold.Name = "pnThreshold"
        Me.pnThreshold.Size = New System.Drawing.Size(74, 256)
        Me.pnThreshold.TabIndex = 297
        Me.pnThreshold.Visible = False
        '
        'txtDAQCompareThreshold5
        '
        Me.txtDAQCompareThreshold5.Location = New System.Drawing.Point(24, 224)
        Me.txtDAQCompareThreshold5.Name = "txtDAQCompareThreshold5"
        Me.txtDAQCompareThreshold5.Size = New System.Drawing.Size(39, 20)
        Me.txtDAQCompareThreshold5.TabIndex = 253
        '
        'txtDAQCompareThreshold4
        '
        Me.txtDAQCompareThreshold4.Location = New System.Drawing.Point(24, 171)
        Me.txtDAQCompareThreshold4.Name = "txtDAQCompareThreshold4"
        Me.txtDAQCompareThreshold4.Size = New System.Drawing.Size(39, 20)
        Me.txtDAQCompareThreshold4.TabIndex = 252
        '
        'cbDAQCompare1_Check
        '
        Me.cbDAQCompare1_Check.AutoSize = True
        Me.cbDAQCompare1_Check.Location = New System.Drawing.Point(8, 13)
        Me.cbDAQCompare1_Check.Name = "cbDAQCompare1_Check"
        Me.cbDAQCompare1_Check.Size = New System.Drawing.Size(15, 14)
        Me.cbDAQCompare1_Check.TabIndex = 244
        Me.cbDAQCompare1_Check.UseVisualStyleBackColor = True
        '
        'txtDAQCompareThreshold3
        '
        Me.txtDAQCompareThreshold3.Location = New System.Drawing.Point(24, 118)
        Me.txtDAQCompareThreshold3.Name = "txtDAQCompareThreshold3"
        Me.txtDAQCompareThreshold3.Size = New System.Drawing.Size(39, 20)
        Me.txtDAQCompareThreshold3.TabIndex = 251
        '
        'cbDAQCompare2_Check
        '
        Me.cbDAQCompare2_Check.AutoSize = True
        Me.cbDAQCompare2_Check.Location = New System.Drawing.Point(8, 69)
        Me.cbDAQCompare2_Check.Name = "cbDAQCompare2_Check"
        Me.cbDAQCompare2_Check.Size = New System.Drawing.Size(15, 14)
        Me.cbDAQCompare2_Check.TabIndex = 245
        Me.cbDAQCompare2_Check.UseVisualStyleBackColor = True
        '
        'txtDAQCompareThreshold2
        '
        Me.txtDAQCompareThreshold2.Location = New System.Drawing.Point(24, 65)
        Me.txtDAQCompareThreshold2.Name = "txtDAQCompareThreshold2"
        Me.txtDAQCompareThreshold2.Size = New System.Drawing.Size(39, 20)
        Me.txtDAQCompareThreshold2.TabIndex = 250
        '
        'cbDAQCompare3_Check
        '
        Me.cbDAQCompare3_Check.AutoSize = True
        Me.cbDAQCompare3_Check.Location = New System.Drawing.Point(8, 122)
        Me.cbDAQCompare3_Check.Name = "cbDAQCompare3_Check"
        Me.cbDAQCompare3_Check.Size = New System.Drawing.Size(15, 14)
        Me.cbDAQCompare3_Check.TabIndex = 246
        Me.cbDAQCompare3_Check.UseVisualStyleBackColor = True
        '
        'txtDAQCompareThreshold1
        '
        Me.txtDAQCompareThreshold1.Location = New System.Drawing.Point(24, 9)
        Me.txtDAQCompareThreshold1.Name = "txtDAQCompareThreshold1"
        Me.txtDAQCompareThreshold1.Size = New System.Drawing.Size(39, 20)
        Me.txtDAQCompareThreshold1.TabIndex = 249
        '
        'cbDAQCompare4_Check
        '
        Me.cbDAQCompare4_Check.AutoSize = True
        Me.cbDAQCompare4_Check.Location = New System.Drawing.Point(8, 175)
        Me.cbDAQCompare4_Check.Name = "cbDAQCompare4_Check"
        Me.cbDAQCompare4_Check.Size = New System.Drawing.Size(15, 14)
        Me.cbDAQCompare4_Check.TabIndex = 247
        Me.cbDAQCompare4_Check.UseVisualStyleBackColor = True
        '
        'cbDAQCompare5_Check
        '
        Me.cbDAQCompare5_Check.AutoSize = True
        Me.cbDAQCompare5_Check.Location = New System.Drawing.Point(8, 228)
        Me.cbDAQCompare5_Check.Name = "cbDAQCompare5_Check"
        Me.cbDAQCompare5_Check.Size = New System.Drawing.Size(15, 14)
        Me.cbDAQCompare5_Check.TabIndex = 248
        Me.cbDAQCompare5_Check.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cbDAQEmailThreshold)
        Me.GroupBox3.Controls.Add(Me.cbDAQEmailTestDone)
        Me.GroupBox3.Controls.Add(Me.chbDAQEmailAddress)
        Me.GroupBox3.Controls.Add(Me.cbDAQEmailNotification)
        Me.GroupBox3.Controls.Add(Me.btnDAQEmailView)
        Me.GroupBox3.Controls.Add(Me.btnDAQEmailAdd)
        Me.GroupBox3.Controls.Add(Me.lblDAQEmailExt)
        Me.GroupBox3.Location = New System.Drawing.Point(1013, 236)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(254, 101)
        Me.GroupBox3.TabIndex = 296
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "E-mail"
        '
        'cbDAQEmailThreshold
        '
        Me.cbDAQEmailThreshold.AutoSize = True
        Me.cbDAQEmailThreshold.Location = New System.Drawing.Point(34, 80)
        Me.cbDAQEmailThreshold.Name = "cbDAQEmailThreshold"
        Me.cbDAQEmailThreshold.Size = New System.Drawing.Size(171, 17)
        Me.cbDAQEmailThreshold.TabIndex = 9
        Me.cbDAQEmailThreshold.Text = "When the threshold is reached"
        Me.cbDAQEmailThreshold.UseVisualStyleBackColor = True
        Me.cbDAQEmailThreshold.Visible = False
        '
        'cbDAQEmailTestDone
        '
        Me.cbDAQEmailTestDone.AutoSize = True
        Me.cbDAQEmailTestDone.Location = New System.Drawing.Point(34, 64)
        Me.cbDAQEmailTestDone.Name = "cbDAQEmailTestDone"
        Me.cbDAQEmailTestDone.Size = New System.Drawing.Size(112, 17)
        Me.cbDAQEmailTestDone.TabIndex = 8
        Me.cbDAQEmailTestDone.Text = "When test is done"
        Me.cbDAQEmailTestDone.UseVisualStyleBackColor = True
        Me.cbDAQEmailTestDone.Visible = False
        '
        'chbDAQEmailAddress
        '
        Me.chbDAQEmailAddress.FormattingEnabled = True
        Me.chbDAQEmailAddress.Location = New System.Drawing.Point(12, 19)
        Me.chbDAQEmailAddress.Name = "chbDAQEmailAddress"
        Me.chbDAQEmailAddress.Size = New System.Drawing.Size(118, 21)
        Me.chbDAQEmailAddress.TabIndex = 6
        '
        'cbDAQEmailNotification
        '
        Me.cbDAQEmailNotification.AutoSize = True
        Me.cbDAQEmailNotification.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbDAQEmailNotification.Location = New System.Drawing.Point(12, 47)
        Me.cbDAQEmailNotification.Name = "cbDAQEmailNotification"
        Me.cbDAQEmailNotification.Size = New System.Drawing.Size(170, 17)
        Me.cbDAQEmailNotification.TabIndex = 5
        Me.cbDAQEmailNotification.Text = "Send Email Notification..."
        Me.cbDAQEmailNotification.UseVisualStyleBackColor = True
        '
        'btnDAQEmailView
        '
        Me.btnDAQEmailView.Location = New System.Drawing.Point(193, 60)
        Me.btnDAQEmailView.Name = "btnDAQEmailView"
        Me.btnDAQEmailView.Size = New System.Drawing.Size(49, 21)
        Me.btnDAQEmailView.TabIndex = 3
        Me.btnDAQEmailView.Text = "View"
        Me.btnDAQEmailView.UseVisualStyleBackColor = True
        '
        'btnDAQEmailAdd
        '
        Me.btnDAQEmailAdd.Location = New System.Drawing.Point(193, 37)
        Me.btnDAQEmailAdd.Name = "btnDAQEmailAdd"
        Me.btnDAQEmailAdd.Size = New System.Drawing.Size(49, 21)
        Me.btnDAQEmailAdd.TabIndex = 2
        Me.btnDAQEmailAdd.Text = "Add"
        Me.btnDAQEmailAdd.UseVisualStyleBackColor = True
        '
        'lblDAQEmailExt
        '
        Me.lblDAQEmailExt.AutoSize = True
        Me.lblDAQEmailExt.Location = New System.Drawing.Point(128, 22)
        Me.lblDAQEmailExt.Name = "lblDAQEmailExt"
        Me.lblDAQEmailExt.Size = New System.Drawing.Size(82, 13)
        Me.lblDAQEmailExt.TabIndex = 1
        Me.lblDAQEmailExt.Text = "@landisgyr.com"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare10)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare8)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare6)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare4)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare9)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare7)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare5)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare3)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare2)
        Me.GroupBox2.Controls.Add(Me.lblDAQCompareCompare1)
        Me.GroupBox2.Controls.Add(Me.txtDAQCompare5)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.txtDAQCompare4)
        Me.GroupBox2.Controls.Add(Me.Label14)
        Me.GroupBox2.Controls.Add(Me.txtDAQCompare3)
        Me.GroupBox2.Controls.Add(Me.Label15)
        Me.GroupBox2.Controls.Add(Me.txtDAQCompare2)
        Me.GroupBox2.Controls.Add(Me.Label16)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare10)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare9)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare8)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare6)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare7)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare4)
        Me.GroupBox2.Controls.Add(Me.txtDAQCompare1)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare2)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare5)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare1)
        Me.GroupBox2.Controls.Add(Me.cbDAQCompare3)
        Me.GroupBox2.Location = New System.Drawing.Point(980, 340)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(286, 318)
        Me.GroupBox2.TabIndex = 295
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Compare"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(264, 96)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(18, 13)
        Me.Label2.TabIndex = 246
        Me.Label2.Text = "°C"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(264, 255)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(18, 13)
        Me.Label1.TabIndex = 245
        Me.Label1.Text = "°C"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.Red
        Me.Label5.Location = New System.Drawing.Point(91, 16)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(184, 13)
        Me.Label5.TabIndex = 244
        Me.Label5.Text = "Red font indicates higher temperature"
        '
        'lblDAQCompareCompare10
        '
        Me.lblDAQCompareCompare10.Location = New System.Drawing.Point(166, 275)
        Me.lblDAQCompareCompare10.Name = "lblDAQCompareCompare10"
        Me.lblDAQCompareCompare10.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare10.TabIndex = 243
        '
        'lblDAQCompareCompare8
        '
        Me.lblDAQCompareCompare8.Location = New System.Drawing.Point(166, 222)
        Me.lblDAQCompareCompare8.Name = "lblDAQCompareCompare8"
        Me.lblDAQCompareCompare8.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare8.TabIndex = 242
        '
        'lblDAQCompareCompare6
        '
        Me.lblDAQCompareCompare6.Location = New System.Drawing.Point(166, 169)
        Me.lblDAQCompareCompare6.Name = "lblDAQCompareCompare6"
        Me.lblDAQCompareCompare6.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare6.TabIndex = 241
        '
        'lblDAQCompareCompare4
        '
        Me.lblDAQCompareCompare4.Location = New System.Drawing.Point(166, 116)
        Me.lblDAQCompareCompare4.Name = "lblDAQCompareCompare4"
        Me.lblDAQCompareCompare4.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare4.TabIndex = 240
        '
        'lblDAQCompareCompare9
        '
        Me.lblDAQCompareCompare9.Location = New System.Drawing.Point(84, 275)
        Me.lblDAQCompareCompare9.Name = "lblDAQCompareCompare9"
        Me.lblDAQCompareCompare9.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare9.TabIndex = 239
        '
        'lblDAQCompareCompare7
        '
        Me.lblDAQCompareCompare7.Location = New System.Drawing.Point(84, 222)
        Me.lblDAQCompareCompare7.Name = "lblDAQCompareCompare7"
        Me.lblDAQCompareCompare7.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare7.TabIndex = 238
        '
        'lblDAQCompareCompare5
        '
        Me.lblDAQCompareCompare5.Location = New System.Drawing.Point(84, 169)
        Me.lblDAQCompareCompare5.Name = "lblDAQCompareCompare5"
        Me.lblDAQCompareCompare5.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare5.TabIndex = 237
        '
        'lblDAQCompareCompare3
        '
        Me.lblDAQCompareCompare3.Location = New System.Drawing.Point(84, 116)
        Me.lblDAQCompareCompare3.Name = "lblDAQCompareCompare3"
        Me.lblDAQCompareCompare3.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare3.TabIndex = 236
        '
        'lblDAQCompareCompare2
        '
        Me.lblDAQCompareCompare2.Location = New System.Drawing.Point(166, 61)
        Me.lblDAQCompareCompare2.Name = "lblDAQCompareCompare2"
        Me.lblDAQCompareCompare2.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare2.TabIndex = 235
        '
        'lblDAQCompareCompare1
        '
        Me.lblDAQCompareCompare1.Location = New System.Drawing.Point(84, 62)
        Me.lblDAQCompareCompare1.Name = "lblDAQCompareCompare1"
        Me.lblDAQCompareCompare1.Size = New System.Drawing.Size(76, 19)
        Me.lblDAQCompareCompare1.TabIndex = 234
        '
        'txtDAQCompare5
        '
        Me.txtDAQCompare5.Location = New System.Drawing.Point(221, 251)
        Me.txtDAQCompare5.Name = "txtDAQCompare5"
        Me.txtDAQCompare5.Size = New System.Drawing.Size(40, 20)
        Me.txtDAQCompare5.TabIndex = 14
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(264, 201)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(18, 13)
        Me.Label12.TabIndex = 232
        Me.Label12.Text = "°C"
        '
        'txtDAQCompare4
        '
        Me.txtDAQCompare4.Location = New System.Drawing.Point(221, 198)
        Me.txtDAQCompare4.Name = "txtDAQCompare4"
        Me.txtDAQCompare4.Size = New System.Drawing.Size(40, 20)
        Me.txtDAQCompare4.TabIndex = 13
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(264, 148)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(18, 13)
        Me.Label14.TabIndex = 231
        Me.Label14.Text = "°C"
        '
        'txtDAQCompare3
        '
        Me.txtDAQCompare3.Location = New System.Drawing.Point(221, 145)
        Me.txtDAQCompare3.Name = "txtDAQCompare3"
        Me.txtDAQCompare3.Size = New System.Drawing.Size(40, 20)
        Me.txtDAQCompare3.TabIndex = 12
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(848, 65)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(18, 13)
        Me.Label15.TabIndex = 230
        Me.Label15.Text = "°C"
        '
        'txtDAQCompare2
        '
        Me.txtDAQCompare2.Location = New System.Drawing.Point(221, 92)
        Me.txtDAQCompare2.Name = "txtDAQCompare2"
        Me.txtDAQCompare2.Size = New System.Drawing.Size(40, 20)
        Me.txtDAQCompare2.TabIndex = 11
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(264, 39)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(18, 13)
        Me.Label16.TabIndex = 229
        Me.Label16.Text = "°C"
        '
        'cbDAQCompare10
        '
        Me.cbDAQCompare10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare10.FormattingEnabled = True
        Me.cbDAQCompare10.Location = New System.Drawing.Point(169, 251)
        Me.cbDAQCompare10.Name = "cbDAQCompare10"
        Me.cbDAQCompare10.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare10.TabIndex = 10
        '
        'cbDAQCompare9
        '
        Me.cbDAQCompare9.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare9.FormattingEnabled = True
        Me.cbDAQCompare9.Location = New System.Drawing.Point(92, 251)
        Me.cbDAQCompare9.Name = "cbDAQCompare9"
        Me.cbDAQCompare9.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare9.TabIndex = 9
        '
        'cbDAQCompare8
        '
        Me.cbDAQCompare8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare8.FormattingEnabled = True
        Me.cbDAQCompare8.Location = New System.Drawing.Point(169, 198)
        Me.cbDAQCompare8.Name = "cbDAQCompare8"
        Me.cbDAQCompare8.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare8.TabIndex = 8
        '
        'cbDAQCompare6
        '
        Me.cbDAQCompare6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare6.FormattingEnabled = True
        Me.cbDAQCompare6.Location = New System.Drawing.Point(169, 145)
        Me.cbDAQCompare6.Name = "cbDAQCompare6"
        Me.cbDAQCompare6.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare6.TabIndex = 6
        '
        'cbDAQCompare7
        '
        Me.cbDAQCompare7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare7.FormattingEnabled = True
        Me.cbDAQCompare7.Location = New System.Drawing.Point(92, 198)
        Me.cbDAQCompare7.Name = "cbDAQCompare7"
        Me.cbDAQCompare7.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare7.TabIndex = 7
        '
        'cbDAQCompare4
        '
        Me.cbDAQCompare4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare4.FormattingEnabled = True
        Me.cbDAQCompare4.Location = New System.Drawing.Point(169, 92)
        Me.cbDAQCompare4.Name = "cbDAQCompare4"
        Me.cbDAQCompare4.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare4.TabIndex = 4
        '
        'txtDAQCompare1
        '
        Me.txtDAQCompare1.Location = New System.Drawing.Point(221, 36)
        Me.txtDAQCompare1.Name = "txtDAQCompare1"
        Me.txtDAQCompare1.Size = New System.Drawing.Size(40, 20)
        Me.txtDAQCompare1.TabIndex = 2
        '
        'cbDAQCompare2
        '
        Me.cbDAQCompare2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare2.FormattingEnabled = True
        Me.cbDAQCompare2.Location = New System.Drawing.Point(169, 36)
        Me.cbDAQCompare2.Name = "cbDAQCompare2"
        Me.cbDAQCompare2.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare2.TabIndex = 1
        '
        'cbDAQCompare5
        '
        Me.cbDAQCompare5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare5.FormattingEnabled = True
        Me.cbDAQCompare5.Location = New System.Drawing.Point(92, 145)
        Me.cbDAQCompare5.Name = "cbDAQCompare5"
        Me.cbDAQCompare5.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare5.TabIndex = 5
        '
        'cbDAQCompare1
        '
        Me.cbDAQCompare1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare1.FormattingEnabled = True
        Me.cbDAQCompare1.Location = New System.Drawing.Point(92, 36)
        Me.cbDAQCompare1.Name = "cbDAQCompare1"
        Me.cbDAQCompare1.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare1.TabIndex = 0
        '
        'cbDAQCompare3
        '
        Me.cbDAQCompare3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQCompare3.FormattingEnabled = True
        Me.cbDAQCompare3.Location = New System.Drawing.Point(92, 92)
        Me.cbDAQCompare3.Name = "cbDAQCompare3"
        Me.cbDAQCompare3.Size = New System.Drawing.Size(43, 21)
        Me.cbDAQCompare3.TabIndex = 3
        '
        'gbMode
        '
        Me.gbMode.Controls.Add(Me.rbNumofReadings)
        Me.gbMode.Controls.Add(Me.rbDuration)
        Me.gbMode.Controls.Add(Me.rbFree)
        Me.gbMode.Location = New System.Drawing.Point(1014, 43)
        Me.gbMode.Name = "gbMode"
        Me.gbMode.Size = New System.Drawing.Size(253, 36)
        Me.gbMode.TabIndex = 292
        Me.gbMode.TabStop = False
        Me.gbMode.Text = "Mode"
        '
        'rbNumofReadings
        '
        Me.rbNumofReadings.AutoSize = True
        Me.rbNumofReadings.Location = New System.Drawing.Point(78, 12)
        Me.rbNumofReadings.Name = "rbNumofReadings"
        Me.rbNumofReadings.Size = New System.Drawing.Size(92, 17)
        Me.rbNumofReadings.TabIndex = 2
        Me.rbNumofReadings.Text = "# of Readings"
        Me.rbNumofReadings.UseVisualStyleBackColor = True
        '
        'rbDuration
        '
        Me.rbDuration.AutoSize = True
        Me.rbDuration.Location = New System.Drawing.Point(182, 12)
        Me.rbDuration.Name = "rbDuration"
        Me.rbDuration.Size = New System.Drawing.Size(65, 17)
        Me.rbDuration.TabIndex = 1
        Me.rbDuration.Text = "Duration"
        Me.rbDuration.UseVisualStyleBackColor = True
        '
        'rbFree
        '
        Me.rbFree.AutoSize = True
        Me.rbFree.Checked = True
        Me.rbFree.Location = New System.Drawing.Point(12, 12)
        Me.rbFree.Name = "rbFree"
        Me.rbFree.Size = New System.Drawing.Size(46, 17)
        Me.rbFree.TabIndex = 0
        Me.rbFree.TabStop = True
        Me.rbFree.Text = "Free"
        Me.rbFree.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(301, 664)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(965, 23)
        Me.ProgressBar1.TabIndex = 290
        '
        'btnDAQStartStopReading
        '
        Me.btnDAQStartStopReading.BackColor = System.Drawing.Color.PaleGreen
        Me.btnDAQStartStopReading.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDAQStartStopReading.Font = New System.Drawing.Font("Californian FB", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDAQStartStopReading.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnDAQStartStopReading.Location = New System.Drawing.Point(662, 10)
        Me.btnDAQStartStopReading.Name = "btnDAQStartStopReading"
        Me.btnDAQStartStopReading.Size = New System.Drawing.Size(93, 73)
        Me.btnDAQStartStopReading.TabIndex = 285
        Me.btnDAQStartStopReading.Text = "Start Reading"
        Me.btnDAQStartStopReading.UseVisualStyleBackColor = False
        '
        'gbInformation
        '
        Me.gbInformation.Controls.Add(Me.lbDAQInformationNumofReadings)
        Me.gbInformation.Controls.Add(Me.lbDAQInformationDuration)
        Me.gbInformation.Controls.Add(Me.lbDAQInformationStartTime)
        Me.gbInformation.Controls.Add(Me.lbDAQInformationTerminateTime)
        Me.gbInformation.Location = New System.Drawing.Point(1013, 138)
        Me.gbInformation.Name = "gbInformation"
        Me.gbInformation.Size = New System.Drawing.Size(254, 73)
        Me.gbInformation.TabIndex = 264
        Me.gbInformation.TabStop = False
        Me.gbInformation.Text = "Information"
        '
        'lbDAQInformationNumofReadings
        '
        Me.lbDAQInformationNumofReadings.AutoSize = True
        Me.lbDAQInformationNumofReadings.Location = New System.Drawing.Point(6, 57)
        Me.lbDAQInformationNumofReadings.Name = "lbDAQInformationNumofReadings"
        Me.lbDAQInformationNumofReadings.Size = New System.Drawing.Size(137, 13)
        Me.lbDAQInformationNumofReadings.TabIndex = 107
        Me.lbDAQInformationNumofReadings.Text = "Total Number of Readings: "
        '
        'lbDAQInformationDuration
        '
        Me.lbDAQInformationDuration.AutoSize = True
        Me.lbDAQInformationDuration.ForeColor = System.Drawing.Color.Red
        Me.lbDAQInformationDuration.Location = New System.Drawing.Point(6, 42)
        Me.lbDAQInformationDuration.Name = "lbDAQInformationDuration"
        Me.lbDAQInformationDuration.Size = New System.Drawing.Size(53, 13)
        Me.lbDAQInformationDuration.TabIndex = 106
        Me.lbDAQInformationDuration.Text = "Duration: "
        '
        'lbDAQInformationStartTime
        '
        Me.lbDAQInformationStartTime.AutoSize = True
        Me.lbDAQInformationStartTime.Location = New System.Drawing.Point(6, 16)
        Me.lbDAQInformationStartTime.Name = "lbDAQInformationStartTime"
        Me.lbDAQInformationStartTime.Size = New System.Drawing.Size(61, 13)
        Me.lbDAQInformationStartTime.TabIndex = 104
        Me.lbDAQInformationStartTime.Text = "Start Time: "
        '
        'lbDAQInformationTerminateTime
        '
        Me.lbDAQInformationTerminateTime.AutoSize = True
        Me.lbDAQInformationTerminateTime.Location = New System.Drawing.Point(6, 29)
        Me.lbDAQInformationTerminateTime.Name = "lbDAQInformationTerminateTime"
        Me.lbDAQInformationTerminateTime.Size = New System.Drawing.Size(86, 13)
        Me.lbDAQInformationTerminateTime.TabIndex = 105
        Me.lbDAQInformationTerminateTime.Text = "Terminate Time: "
        '
        'gbParameters
        '
        Me.gbParameters.Controls.Add(Me.rbDAQParametersThermocouple)
        Me.gbParameters.Controls.Add(Me.Label4)
        Me.gbParameters.Controls.Add(Me.rbDAQParametersFRTD)
        Me.gbParameters.Controls.Add(Me.Label3)
        Me.gbParameters.Controls.Add(Me.cbDAQParametersReadingIntervals)
        Me.gbParameters.Location = New System.Drawing.Point(788, 6)
        Me.gbParameters.Name = "gbParameters"
        Me.gbParameters.Size = New System.Drawing.Size(222, 58)
        Me.gbParameters.TabIndex = 263
        Me.gbParameters.TabStop = False
        Me.gbParameters.Text = "Parameters"
        '
        'rbDAQParametersThermocouple
        '
        Me.rbDAQParametersThermocouple.AutoSize = True
        Me.rbDAQParametersThermocouple.Location = New System.Drawing.Point(107, 37)
        Me.rbDAQParametersThermocouple.Name = "rbDAQParametersThermocouple"
        Me.rbDAQParametersThermocouple.Size = New System.Drawing.Size(93, 17)
        Me.rbDAQParametersThermocouple.TabIndex = 315
        Me.rbDAQParametersThermocouple.Text = "Thermocouple"
        Me.rbDAQParametersThermocouple.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(164, 18)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(47, 13)
        Me.Label4.TabIndex = 66
        Me.Label4.Text = "seconds"
        '
        'rbDAQParametersFRTD
        '
        Me.rbDAQParametersFRTD.AutoSize = True
        Me.rbDAQParametersFRTD.Location = New System.Drawing.Point(13, 37)
        Me.rbDAQParametersFRTD.Name = "rbDAQParametersFRTD"
        Me.rbDAQParametersFRTD.Size = New System.Drawing.Size(88, 17)
        Me.rbDAQParametersFRTD.TabIndex = 314
        Me.rbDAQParametersFRTD.Text = "RTD (4-Wire)"
        Me.rbDAQParametersFRTD.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 18)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 13)
        Me.Label3.TabIndex = 65
        Me.Label3.Text = "Reading Intervals"
        '
        'cbDAQParametersReadingIntervals
        '
        Me.cbDAQParametersReadingIntervals.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbDAQParametersReadingIntervals.FormattingEnabled = True
        Me.cbDAQParametersReadingIntervals.Items.AddRange(New Object() {"5", "10", "15", "20", "30", "60", "120", "180", "300", "600", "900", "1200", "1800", "3600", "7200"})
        Me.cbDAQParametersReadingIntervals.Location = New System.Drawing.Point(107, 15)
        Me.cbDAQParametersReadingIntervals.Name = "cbDAQParametersReadingIntervals"
        Me.cbDAQParametersReadingIntervals.Size = New System.Drawing.Size(50, 21)
        Me.cbDAQParametersReadingIntervals.TabIndex = 64
        '
        'Plot
        '
        Me.Plot.Controls.Add(Me.CurrentChart)
        Me.Plot.Controls.Add(Me.rbPlotCurrent)
        Me.Plot.Controls.Add(Me.rbPlotTemperature)
        Me.Plot.Controls.Add(Me.DataChart)
        Me.Plot.Controls.Add(Me.gbPlotLineWidth)
        Me.Plot.Controls.Add(Me.lblPlotReminder)
        Me.Plot.Controls.Add(Me.btnPlotLoadPlot)
        Me.Plot.Controls.Add(Me.btnPlotSavePlot)
        Me.Plot.Controls.Add(Me.btnPlotResetXY)
        Me.Plot.Controls.Add(Me.btnPlotResetY)
        Me.Plot.Controls.Add(Me.btnPlotResetX)
        Me.Plot.Location = New System.Drawing.Point(4, 22)
        Me.Plot.Name = "Plot"
        Me.Plot.Size = New System.Drawing.Size(1274, 693)
        Me.Plot.TabIndex = 2
        Me.Plot.Text = "DAQ Plot"
        Me.Plot.UseVisualStyleBackColor = True
        '
        'CurrentChart
        '
        ChartArea1.Name = "ChartArea1"
        Me.CurrentChart.ChartAreas.Add(ChartArea1)
        Legend1.Name = "Legend1"
        Me.CurrentChart.Legends.Add(Legend1)
        Me.CurrentChart.Location = New System.Drawing.Point(3, 3)
        Me.CurrentChart.Name = "CurrentChart"
        Me.CurrentChart.Size = New System.Drawing.Size(1268, 602)
        Me.CurrentChart.TabIndex = 24
        Me.CurrentChart.Text = "Chart1"
        Me.CurrentChart.Visible = False
        '
        'rbPlotCurrent
        '
        Me.rbPlotCurrent.AutoSize = True
        Me.rbPlotCurrent.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbPlotCurrent.Location = New System.Drawing.Point(804, 655)
        Me.rbPlotCurrent.Name = "rbPlotCurrent"
        Me.rbPlotCurrent.Size = New System.Drawing.Size(122, 20)
        Me.rbPlotCurrent.TabIndex = 23
        Me.rbPlotCurrent.Text = "Current vs. Time"
        Me.rbPlotCurrent.UseVisualStyleBackColor = True
        '
        'rbPlotTemperature
        '
        Me.rbPlotTemperature.AutoSize = True
        Me.rbPlotTemperature.Checked = True
        Me.rbPlotTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbPlotTemperature.Location = New System.Drawing.Point(623, 655)
        Me.rbPlotTemperature.Name = "rbPlotTemperature"
        Me.rbPlotTemperature.Size = New System.Drawing.Size(158, 20)
        Me.rbPlotTemperature.TabIndex = 22
        Me.rbPlotTemperature.TabStop = True
        Me.rbPlotTemperature.Text = "Temperature vs. Time"
        Me.rbPlotTemperature.UseVisualStyleBackColor = True
        '
        'DataChart
        '
        ChartArea2.Name = "ChartArea1"
        Me.DataChart.ChartAreas.Add(ChartArea2)
        Legend2.Name = "Legend1"
        Me.DataChart.Legends.Add(Legend2)
        Me.DataChart.Location = New System.Drawing.Point(3, 3)
        Me.DataChart.Name = "DataChart"
        Me.DataChart.Size = New System.Drawing.Size(1268, 602)
        Me.DataChart.TabIndex = 21
        Me.DataChart.Text = "Chart1"
        '
        'gbPlotLineWidth
        '
        Me.gbPlotLineWidth.Controls.Add(Me.btnPlotSetLineWidth)
        Me.gbPlotLineWidth.Controls.Add(Me.cbPlotLineWidth)
        Me.gbPlotLineWidth.Location = New System.Drawing.Point(1006, 611)
        Me.gbPlotLineWidth.Name = "gbPlotLineWidth"
        Me.gbPlotLineWidth.Size = New System.Drawing.Size(81, 79)
        Me.gbPlotLineWidth.TabIndex = 19
        Me.gbPlotLineWidth.TabStop = False
        Me.gbPlotLineWidth.Text = "Line Width"
        '
        'btnPlotSetLineWidth
        '
        Me.btnPlotSetLineWidth.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlotSetLineWidth.Location = New System.Drawing.Point(21, 46)
        Me.btnPlotSetLineWidth.Name = "btnPlotSetLineWidth"
        Me.btnPlotSetLineWidth.Size = New System.Drawing.Size(42, 29)
        Me.btnPlotSetLineWidth.TabIndex = 14
        Me.btnPlotSetLineWidth.Text = "Set"
        Me.btnPlotSetLineWidth.UseVisualStyleBackColor = True
        '
        'cbPlotLineWidth
        '
        Me.cbPlotLineWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPlotLineWidth.FormattingEnabled = True
        Me.cbPlotLineWidth.Items.AddRange(New Object() {"1", "2", "3"})
        Me.cbPlotLineWidth.Location = New System.Drawing.Point(21, 19)
        Me.cbPlotLineWidth.Name = "cbPlotLineWidth"
        Me.cbPlotLineWidth.Size = New System.Drawing.Size(42, 21)
        Me.cbPlotLineWidth.TabIndex = 0
        '
        'lblPlotReminder
        '
        Me.lblPlotReminder.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.lblPlotReminder.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPlotReminder.Location = New System.Drawing.Point(3, 611)
        Me.lblPlotReminder.Name = "lblPlotReminder"
        Me.lblPlotReminder.Size = New System.Drawing.Size(983, 30)
        Me.lblPlotReminder.TabIndex = 17
        '
        'btnPlotLoadPlot
        '
        Me.btnPlotLoadPlot.Font = New System.Drawing.Font("Californian FB", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlotLoadPlot.Location = New System.Drawing.Point(3, 644)
        Me.btnPlotLoadPlot.Name = "btnPlotLoadPlot"
        Me.btnPlotLoadPlot.Size = New System.Drawing.Size(111, 40)
        Me.btnPlotLoadPlot.TabIndex = 16
        Me.btnPlotLoadPlot.Text = "Load Plot"
        Me.btnPlotLoadPlot.UseVisualStyleBackColor = True
        '
        'btnPlotSavePlot
        '
        Me.btnPlotSavePlot.Font = New System.Drawing.Font("Californian FB", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlotSavePlot.Location = New System.Drawing.Point(472, 644)
        Me.btnPlotSavePlot.Name = "btnPlotSavePlot"
        Me.btnPlotSavePlot.Size = New System.Drawing.Size(111, 40)
        Me.btnPlotSavePlot.TabIndex = 4
        Me.btnPlotSavePlot.Text = "Save Plot As Image"
        Me.btnPlotSavePlot.UseVisualStyleBackColor = True
        '
        'btnPlotResetXY
        '
        Me.btnPlotResetXY.Font = New System.Drawing.Font("Californian FB", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlotResetXY.Location = New System.Drawing.Point(355, 644)
        Me.btnPlotResetXY.Name = "btnPlotResetXY"
        Me.btnPlotResetXY.Size = New System.Drawing.Size(111, 40)
        Me.btnPlotResetXY.TabIndex = 3
        Me.btnPlotResetXY.Text = "Reset XY-Axis"
        Me.btnPlotResetXY.UseVisualStyleBackColor = True
        '
        'btnPlotResetY
        '
        Me.btnPlotResetY.Font = New System.Drawing.Font("Californian FB", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlotResetY.Location = New System.Drawing.Point(237, 644)
        Me.btnPlotResetY.Name = "btnPlotResetY"
        Me.btnPlotResetY.Size = New System.Drawing.Size(111, 40)
        Me.btnPlotResetY.TabIndex = 2
        Me.btnPlotResetY.Text = "Reset Y-Axis"
        Me.btnPlotResetY.UseVisualStyleBackColor = True
        '
        'btnPlotResetX
        '
        Me.btnPlotResetX.Font = New System.Drawing.Font("Californian FB", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPlotResetX.Location = New System.Drawing.Point(120, 644)
        Me.btnPlotResetX.Name = "btnPlotResetX"
        Me.btnPlotResetX.Size = New System.Drawing.Size(111, 40)
        Me.btnPlotResetX.TabIndex = 1
        Me.btnPlotResetX.Text = "Reset X-Axis"
        Me.btnPlotResetX.UseVisualStyleBackColor = True
        '
        'Log
        '
        Me.Log.Controls.Add(Me.gbDAQIdentification)
        Me.Log.Controls.Add(Me.lbDisplayInstantMetrics)
        Me.Log.Controls.Add(Me.chkErrorLogVerbose)
        Me.Log.Controls.Add(Me.lstDAQLog)
        Me.Log.Controls.Add(Me.lstDAQError)
        Me.Log.Location = New System.Drawing.Point(4, 22)
        Me.Log.Name = "Log"
        Me.Log.Padding = New System.Windows.Forms.Padding(3)
        Me.Log.Size = New System.Drawing.Size(1274, 693)
        Me.Log.TabIndex = 1
        Me.Log.Text = "DAQ Log"
        Me.Log.UseVisualStyleBackColor = True
        '
        'gbDAQIdentification
        '
        Me.gbDAQIdentification.Controls.Add(Me.lbDAQIdentification)
        Me.gbDAQIdentification.Controls.Add(Me.btnDAQIdentificationIdentity)
        Me.gbDAQIdentification.Location = New System.Drawing.Point(437, 216)
        Me.gbDAQIdentification.Name = "gbDAQIdentification"
        Me.gbDAQIdentification.Size = New System.Drawing.Size(172, 114)
        Me.gbDAQIdentification.TabIndex = 300
        Me.gbDAQIdentification.TabStop = False
        Me.gbDAQIdentification.Text = "34970A Identification"
        '
        'lbDAQIdentification
        '
        Me.lbDAQIdentification.FormattingEnabled = True
        Me.lbDAQIdentification.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lbDAQIdentification.Location = New System.Drawing.Point(6, 17)
        Me.lbDAQIdentification.Name = "lbDAQIdentification"
        Me.lbDAQIdentification.Size = New System.Drawing.Size(157, 56)
        Me.lbDAQIdentification.TabIndex = 58
        '
        'btnDAQIdentificationIdentity
        '
        Me.btnDAQIdentificationIdentity.Location = New System.Drawing.Point(6, 79)
        Me.btnDAQIdentificationIdentity.Name = "btnDAQIdentificationIdentity"
        Me.btnDAQIdentificationIdentity.Size = New System.Drawing.Size(94, 23)
        Me.btnDAQIdentificationIdentity.TabIndex = 57
        Me.btnDAQIdentificationIdentity.Text = "Identify"
        '
        'lbDisplayInstantMetrics
        '
        Me.lbDisplayInstantMetrics.FormattingEnabled = True
        Me.lbDisplayInstantMetrics.Location = New System.Drawing.Point(437, 37)
        Me.lbDisplayInstantMetrics.Name = "lbDisplayInstantMetrics"
        Me.lbDisplayInstantMetrics.Size = New System.Drawing.Size(229, 173)
        Me.lbDisplayInstantMetrics.TabIndex = 299
        '
        'chkErrorLogVerbose
        '
        Me.chkErrorLogVerbose.AutoSize = True
        Me.chkErrorLogVerbose.Location = New System.Drawing.Point(24, 15)
        Me.chkErrorLogVerbose.Name = "chkErrorLogVerbose"
        Me.chkErrorLogVerbose.Size = New System.Drawing.Size(95, 17)
        Me.chkErrorLogVerbose.TabIndex = 298
        Me.chkErrorLogVerbose.Text = "Verbose Mode"
        Me.chkErrorLogVerbose.UseVisualStyleBackColor = True
        '
        'lstDAQLog
        '
        Me.lstDAQLog.FormattingEnabled = True
        Me.lstDAQLog.HorizontalScrollbar = True
        Me.lstDAQLog.Location = New System.Drawing.Point(24, 37)
        Me.lstDAQLog.Name = "lstDAQLog"
        Me.lstDAQLog.Size = New System.Drawing.Size(407, 277)
        Me.lstDAQLog.TabIndex = 297
        '
        'lstDAQError
        '
        Me.lstDAQError.FormattingEnabled = True
        Me.lstDAQError.HorizontalScrollbar = True
        Me.lstDAQError.Location = New System.Drawing.Point(24, 323)
        Me.lstDAQError.Name = "lstDAQError"
        Me.lstDAQError.Size = New System.Drawing.Size(407, 277)
        Me.lstDAQError.TabIndex = 296
        '
        'CurrentController
        '
        Me.CurrentController.Controls.Add(Me.gbDataLogger)
        Me.CurrentController.Controls.Add(Me.btnTest)
        Me.CurrentController.Controls.Add(Me.TextBox2)
        Me.CurrentController.Controls.Add(Me.btnSaveGPIB_LOG)
        Me.CurrentController.Controls.Add(Me.btnCI501TCA)
        Me.CurrentController.Controls.Add(Me.gbSaveData)
        Me.CurrentController.Controls.Add(Me.lbGPIB_Log)
        Me.CurrentController.Controls.Add(Me.gbMetrics)
        Me.CurrentController.Controls.Add(Me.chkGPIBVerbose)
        Me.CurrentController.Controls.Add(Me.btnGPIB_LogClear)
        Me.CurrentController.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CurrentController.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CurrentController.Location = New System.Drawing.Point(4, 22)
        Me.CurrentController.Name = "CurrentController"
        Me.CurrentController.Size = New System.Drawing.Size(1274, 693)
        Me.CurrentController.TabIndex = 3
        Me.CurrentController.Text = "Current Controller"
        Me.CurrentController.UseVisualStyleBackColor = True
        '
        'gbDataLogger
        '
        Me.gbDataLogger.Controls.Add(Me.lblDataLoggerSampleInterval)
        Me.gbDataLogger.Controls.Add(Me.lblDataLoggerSeconds)
        Me.gbDataLogger.Controls.Add(Me.txtDataLoggerInterval)
        Me.gbDataLogger.Controls.Add(Me.btnDataLoggerToggleLogger)
        Me.gbDataLogger.Location = New System.Drawing.Point(19, 6)
        Me.gbDataLogger.Name = "gbDataLogger"
        Me.gbDataLogger.Size = New System.Drawing.Size(163, 121)
        Me.gbDataLogger.TabIndex = 324
        Me.gbDataLogger.TabStop = False
        Me.gbDataLogger.Text = "Data Logger"
        '
        'lblDataLoggerSampleInterval
        '
        Me.lblDataLoggerSampleInterval.AutoSize = True
        Me.lblDataLoggerSampleInterval.Location = New System.Drawing.Point(1, 69)
        Me.lblDataLoggerSampleInterval.Name = "lblDataLoggerSampleInterval"
        Me.lblDataLoggerSampleInterval.Size = New System.Drawing.Size(80, 13)
        Me.lblDataLoggerSampleInterval.TabIndex = 7
        Me.lblDataLoggerSampleInterval.Text = "Sample Interval"
        '
        'lblDataLoggerSeconds
        '
        Me.lblDataLoggerSeconds.AutoSize = True
        Me.lblDataLoggerSeconds.Location = New System.Drawing.Point(67, 88)
        Me.lblDataLoggerSeconds.Name = "lblDataLoggerSeconds"
        Me.lblDataLoggerSeconds.Size = New System.Drawing.Size(49, 13)
        Me.lblDataLoggerSeconds.TabIndex = 6
        Me.lblDataLoggerSeconds.Text = "Seconds"
        '
        'txtDataLoggerInterval
        '
        Me.txtDataLoggerInterval.Location = New System.Drawing.Point(6, 86)
        Me.txtDataLoggerInterval.Name = "txtDataLoggerInterval"
        Me.txtDataLoggerInterval.Size = New System.Drawing.Size(55, 20)
        Me.txtDataLoggerInterval.TabIndex = 5
        Me.txtDataLoggerInterval.Text = "5"
        '
        'btnDataLoggerToggleLogger
        '
        Me.btnDataLoggerToggleLogger.Location = New System.Drawing.Point(6, 21)
        Me.btnDataLoggerToggleLogger.Name = "btnDataLoggerToggleLogger"
        Me.btnDataLoggerToggleLogger.Size = New System.Drawing.Size(72, 38)
        Me.btnDataLoggerToggleLogger.TabIndex = 0
        Me.btnDataLoggerToggleLogger.Text = "Start Logger"
        Me.btnDataLoggerToggleLogger.UseVisualStyleBackColor = True
        '
        'btnTest
        '
        Me.btnTest.Location = New System.Drawing.Point(746, 202)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(63, 21)
        Me.btnTest.TabIndex = 130
        Me.btnTest.Text = "Button1"
        Me.btnTest.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(697, 240)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(155, 112)
        Me.TextBox2.TabIndex = 129
        '
        'btnSaveGPIB_LOG
        '
        Me.btnSaveGPIB_LOG.Location = New System.Drawing.Point(1002, 20)
        Me.btnSaveGPIB_LOG.Name = "btnSaveGPIB_LOG"
        Me.btnSaveGPIB_LOG.Size = New System.Drawing.Size(63, 21)
        Me.btnSaveGPIB_LOG.TabIndex = 128
        Me.btnSaveGPIB_LOG.Text = "Save Log"
        Me.btnSaveGPIB_LOG.UseVisualStyleBackColor = True
        '
        'btnCI501TCA
        '
        Me.btnCI501TCA.Location = New System.Drawing.Point(746, 18)
        Me.btnCI501TCA.Name = "btnCI501TCA"
        Me.btnCI501TCA.Size = New System.Drawing.Size(80, 23)
        Me.btnCI501TCA.TabIndex = 120
        Me.btnCI501TCA.Text = "Set Defaults"
        '
        'gbSaveData
        '
        Me.gbSaveData.Controls.Add(Me.btnRadianLoggerSaveData)
        Me.gbSaveData.Controls.Add(Me.lblDataSaveDelimiter)
        Me.gbSaveData.Controls.Add(Me.rbDataSaveComma)
        Me.gbSaveData.Controls.Add(Me.rbDataSaveSpace)
        Me.gbSaveData.Controls.Add(Me.rbDataSaveTab)
        Me.gbSaveData.Controls.Add(Me.rbDataSaveSemiColon)
        Me.gbSaveData.Controls.Add(Me.Label10)
        Me.gbSaveData.Controls.Add(Me.txtLogDataNotes)
        Me.gbSaveData.Controls.Add(Me.chkCheckToSaveData)
        Me.gbSaveData.Controls.Add(Me.chkMeterTestSaveDataOverWrite)
        Me.gbSaveData.Controls.Add(Me.Label27)
        Me.gbSaveData.Controls.Add(Me.btnDataSaveLocation)
        Me.gbSaveData.Controls.Add(Me.Label13)
        Me.gbSaveData.Controls.Add(Me.txtFolderSave)
        Me.gbSaveData.Controls.Add(Me.txtDataFileName)
        Me.gbSaveData.Location = New System.Drawing.Point(188, 11)
        Me.gbSaveData.Name = "gbSaveData"
        Me.gbSaveData.Size = New System.Drawing.Size(351, 261)
        Me.gbSaveData.TabIndex = 124
        Me.gbSaveData.TabStop = False
        Me.gbSaveData.Text = "Save Data"
        '
        'btnRadianLoggerSaveData
        '
        Me.btnRadianLoggerSaveData.Location = New System.Drawing.Point(6, 57)
        Me.btnRadianLoggerSaveData.Name = "btnRadianLoggerSaveData"
        Me.btnRadianLoggerSaveData.Size = New System.Drawing.Size(82, 45)
        Me.btnRadianLoggerSaveData.TabIndex = 134
        Me.btnRadianLoggerSaveData.Text = "Save Results"
        Me.btnRadianLoggerSaveData.UseVisualStyleBackColor = True
        '
        'lblDataSaveDelimiter
        '
        Me.lblDataSaveDelimiter.AutoSize = True
        Me.lblDataSaveDelimiter.Location = New System.Drawing.Point(6, 107)
        Me.lblDataSaveDelimiter.Name = "lblDataSaveDelimiter"
        Me.lblDataSaveDelimiter.Size = New System.Drawing.Size(50, 13)
        Me.lblDataSaveDelimiter.TabIndex = 133
        Me.lblDataSaveDelimiter.Text = "Delimiter:"
        '
        'rbDataSaveComma
        '
        Me.rbDataSaveComma.AutoSize = True
        Me.rbDataSaveComma.Checked = True
        Me.rbDataSaveComma.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbDataSaveComma.Location = New System.Drawing.Point(112, 141)
        Me.rbDataSaveComma.Name = "rbDataSaveComma"
        Me.rbDataSaveComma.Size = New System.Drawing.Size(60, 17)
        Me.rbDataSaveComma.TabIndex = 132
        Me.rbDataSaveComma.TabStop = True
        Me.rbDataSaveComma.Text = "Comma"
        Me.rbDataSaveComma.UseVisualStyleBackColor = True
        '
        'rbDataSaveSpace
        '
        Me.rbDataSaveSpace.AutoSize = True
        Me.rbDataSaveSpace.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbDataSaveSpace.Location = New System.Drawing.Point(178, 141)
        Me.rbDataSaveSpace.Name = "rbDataSaveSpace"
        Me.rbDataSaveSpace.Size = New System.Drawing.Size(56, 17)
        Me.rbDataSaveSpace.TabIndex = 131
        Me.rbDataSaveSpace.Text = "Space"
        Me.rbDataSaveSpace.UseVisualStyleBackColor = True
        '
        'rbDataSaveTab
        '
        Me.rbDataSaveTab.AutoSize = True
        Me.rbDataSaveTab.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbDataSaveTab.Location = New System.Drawing.Point(62, 141)
        Me.rbDataSaveTab.Name = "rbDataSaveTab"
        Me.rbDataSaveTab.Size = New System.Drawing.Size(44, 17)
        Me.rbDataSaveTab.TabIndex = 130
        Me.rbDataSaveTab.Text = "Tab"
        Me.rbDataSaveTab.UseVisualStyleBackColor = True
        '
        'rbDataSaveSemiColon
        '
        Me.rbDataSaveSemiColon.AutoSize = True
        Me.rbDataSaveSemiColon.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbDataSaveSemiColon.Location = New System.Drawing.Point(240, 141)
        Me.rbDataSaveSemiColon.Name = "rbDataSaveSemiColon"
        Me.rbDataSaveSemiColon.Size = New System.Drawing.Size(78, 17)
        Me.rbDataSaveSemiColon.TabIndex = 129
        Me.rbDataSaveSemiColon.Text = "Semi-Colon"
        Me.rbDataSaveSemiColon.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(6, 172)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(95, 13)
        Me.Label10.TabIndex = 101
        Me.Label10.Text = "File Header Notes:"
        '
        'txtLogDataNotes
        '
        Me.txtLogDataNotes.Location = New System.Drawing.Point(9, 191)
        Me.txtLogDataNotes.Multiline = True
        Me.txtLogDataNotes.Name = "txtLogDataNotes"
        Me.txtLogDataNotes.Size = New System.Drawing.Size(333, 55)
        Me.txtLogDataNotes.TabIndex = 128
        Me.txtLogDataNotes.Text = "1"
        '
        'chkCheckToSaveData
        '
        Me.chkCheckToSaveData.AutoSize = True
        Me.chkCheckToSaveData.Location = New System.Drawing.Point(214, 92)
        Me.chkCheckToSaveData.Name = "chkCheckToSaveData"
        Me.chkCheckToSaveData.Size = New System.Drawing.Size(114, 17)
        Me.chkCheckToSaveData.TabIndex = 25
        Me.chkCheckToSaveData.Text = "Ask To Save Data"
        Me.chkCheckToSaveData.UseVisualStyleBackColor = True
        '
        'chkMeterTestSaveDataOverWrite
        '
        Me.chkMeterTestSaveDataOverWrite.AutoSize = True
        Me.chkMeterTestSaveDataOverWrite.Location = New System.Drawing.Point(112, 92)
        Me.chkMeterTestSaveDataOverWrite.Name = "chkMeterTestSaveDataOverWrite"
        Me.chkMeterTestSaveDataOverWrite.Size = New System.Drawing.Size(96, 17)
        Me.chkMeterTestSaveDataOverWrite.TabIndex = 24
        Me.chkMeterTestSaveDataOverWrite.Text = "Over Write File"
        Me.chkMeterTestSaveDataOverWrite.UseVisualStyleBackColor = True
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Enabled = False
        Me.Label27.Location = New System.Drawing.Point(6, 17)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(91, 13)
        Me.Label27.TabIndex = 23
        Me.Label27.Text = "Start of File Name"
        Me.Label27.Visible = False
        '
        'btnDataSaveLocation
        '
        Me.btnDataSaveLocation.Enabled = False
        Me.btnDataSaveLocation.Font = New System.Drawing.Font("Arial Black", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDataSaveLocation.Location = New System.Drawing.Point(314, 115)
        Me.btnDataSaveLocation.Name = "btnDataSaveLocation"
        Me.btnDataSaveLocation.Size = New System.Drawing.Size(30, 21)
        Me.btnDataSaveLocation.TabIndex = 18
        Me.btnDataSaveLocation.Text = "..."
        Me.btnDataSaveLocation.UseVisualStyleBackColor = True
        Me.btnDataSaveLocation.Visible = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(4, 41)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(108, 13)
        Me.Label13.TabIndex = 20
        Me.Label13.Text = "Folder Save Location"
        Me.Label13.Visible = False
        '
        'txtFolderSave
        '
        Me.txtFolderSave.Enabled = False
        Me.txtFolderSave.Location = New System.Drawing.Point(6, 115)
        Me.txtFolderSave.Name = "txtFolderSave"
        Me.txtFolderSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.txtFolderSave.Size = New System.Drawing.Size(302, 20)
        Me.txtFolderSave.TabIndex = 21
        Me.txtFolderSave.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.txtFolderSave.Visible = False
        '
        'txtDataFileName
        '
        Me.txtDataFileName.Enabled = False
        Me.txtDataFileName.Location = New System.Drawing.Point(100, 14)
        Me.txtDataFileName.Name = "txtDataFileName"
        Me.txtDataFileName.Size = New System.Drawing.Size(242, 20)
        Me.txtDataFileName.TabIndex = 22
        Me.txtDataFileName.Text = "File"
        Me.txtDataFileName.Visible = False
        '
        'lbGPIB_Log
        '
        Me.lbGPIB_Log.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.lbGPIB_Log.FormattingEnabled = True
        Me.lbGPIB_Log.Location = New System.Drawing.Point(881, 52)
        Me.lbGPIB_Log.Name = "lbGPIB_Log"
        Me.lbGPIB_Log.Size = New System.Drawing.Size(263, 329)
        Me.lbGPIB_Log.TabIndex = 108
        '
        'gbMetrics
        '
        Me.gbMetrics.Controls.Add(Me.cbSelectInstantMetrics)
        Me.gbMetrics.Controls.Add(Me.btnGetInstantMetrics)
        Me.gbMetrics.Location = New System.Drawing.Point(545, 11)
        Me.gbMetrics.Name = "gbMetrics"
        Me.gbMetrics.Size = New System.Drawing.Size(177, 199)
        Me.gbMetrics.TabIndex = 123
        Me.gbMetrics.TabStop = False
        Me.gbMetrics.Text = "Metrics"
        '
        'cbSelectInstantMetrics
        '
        Me.cbSelectInstantMetrics.FormattingEnabled = True
        Me.cbSelectInstantMetrics.Location = New System.Drawing.Point(17, 199)
        Me.cbSelectInstantMetrics.Name = "cbSelectInstantMetrics"
        Me.cbSelectInstantMetrics.Size = New System.Drawing.Size(139, 21)
        Me.cbSelectInstantMetrics.TabIndex = 51
        '
        'btnGetInstantMetrics
        '
        Me.btnGetInstantMetrics.Location = New System.Drawing.Point(50, 226)
        Me.btnGetInstantMetrics.Name = "btnGetInstantMetrics"
        Me.btnGetInstantMetrics.Size = New System.Drawing.Size(72, 48)
        Me.btnGetInstantMetrics.TabIndex = 52
        Me.btnGetInstantMetrics.Text = "Update" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Instant" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Metrics"
        '
        'chkGPIBVerbose
        '
        Me.chkGPIBVerbose.AutoSize = True
        Me.chkGPIBVerbose.Checked = True
        Me.chkGPIBVerbose.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkGPIBVerbose.Location = New System.Drawing.Point(881, 29)
        Me.chkGPIBVerbose.Name = "chkGPIBVerbose"
        Me.chkGPIBVerbose.Size = New System.Drawing.Size(65, 17)
        Me.chkGPIBVerbose.TabIndex = 109
        Me.chkGPIBVerbose.Text = "Verbose"
        Me.chkGPIBVerbose.UseVisualStyleBackColor = True
        '
        'btnGPIB_LogClear
        '
        Me.btnGPIB_LogClear.Location = New System.Drawing.Point(1071, 18)
        Me.btnGPIB_LogClear.Name = "btnGPIB_LogClear"
        Me.btnGPIB_LogClear.Size = New System.Drawing.Size(61, 23)
        Me.btnGPIB_LogClear.TabIndex = 110
        Me.btnGPIB_LogClear.Text = "Clear Log"
        Me.btnGPIB_LogClear.UseVisualStyleBackColor = True
        '
        'ErrorLog
        '
        Me.ErrorLog.Controls.Add(Me.TextBox1)
        Me.ErrorLog.Controls.Add(Me.TextBox4)
        Me.ErrorLog.Controls.Add(Me.chkVerbose)
        Me.ErrorLog.Controls.Add(Me.btnClearLog)
        Me.ErrorLog.Controls.Add(Me.btnSaveLog)
        Me.ErrorLog.Controls.Add(Me.lblLogData)
        Me.ErrorLog.Location = New System.Drawing.Point(4, 22)
        Me.ErrorLog.Name = "ErrorLog"
        Me.ErrorLog.Size = New System.Drawing.Size(1274, 693)
        Me.ErrorLog.TabIndex = 4
        Me.ErrorLog.Text = "ErrorLog"
        Me.ErrorLog.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(18, 294)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(322, 20)
        Me.TextBox1.TabIndex = 113
        '
        'TextBox4
        '
        Me.TextBox4.Location = New System.Drawing.Point(437, 56)
        Me.TextBox4.Multiline = True
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(326, 201)
        Me.TextBox4.TabIndex = 112
        '
        'chkVerbose
        '
        Me.chkVerbose.AutoSize = True
        Me.chkVerbose.Location = New System.Drawing.Point(142, 19)
        Me.chkVerbose.Name = "chkVerbose"
        Me.chkVerbose.Size = New System.Drawing.Size(95, 17)
        Me.chkVerbose.TabIndex = 111
        Me.chkVerbose.Text = "Verbose Mode"
        Me.chkVerbose.UseVisualStyleBackColor = True
        '
        'btnClearLog
        '
        Me.btnClearLog.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClearLog.Location = New System.Drawing.Point(82, 15)
        Me.btnClearLog.Name = "btnClearLog"
        Me.btnClearLog.Size = New System.Drawing.Size(54, 25)
        Me.btnClearLog.TabIndex = 110
        Me.btnClearLog.Text = "Clear Log"
        Me.btnClearLog.UseVisualStyleBackColor = True
        '
        'btnSaveLog
        '
        Me.btnSaveLog.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSaveLog.Location = New System.Drawing.Point(19, 15)
        Me.btnSaveLog.Name = "btnSaveLog"
        Me.btnSaveLog.Size = New System.Drawing.Size(57, 25)
        Me.btnSaveLog.TabIndex = 109
        Me.btnSaveLog.Text = "Save Log"
        Me.btnSaveLog.UseVisualStyleBackColor = True
        '
        'lblLogData
        '
        Me.lblLogData.FormattingEnabled = True
        Me.lblLogData.HorizontalScrollbar = True
        Me.lblLogData.Location = New System.Drawing.Point(19, 58)
        Me.lblLogData.Name = "lblLogData"
        Me.lblLogData.Size = New System.Drawing.Size(321, 199)
        Me.lblLogData.TabIndex = 108
        '
        'tbRadian
        '
        Me.tbRadian.Controls.Add(Me.gbIdentification)
        Me.tbRadian.Controls.Add(Me.lbDisplayAccumMetrics)
        Me.tbRadian.Controls.Add(Me.btnGetAccumMetrics)
        Me.tbRadian.Controls.Add(Me.cbSelectAccumMetrics)
        Me.tbRadian.Controls.Add(Me.gbCurrentRange)
        Me.tbRadian.Controls.Add(Me.gbReset)
        Me.tbRadian.Location = New System.Drawing.Point(4, 22)
        Me.tbRadian.Name = "tbRadian"
        Me.tbRadian.Size = New System.Drawing.Size(1274, 693)
        Me.tbRadian.TabIndex = 6
        Me.tbRadian.Text = "Radian"
        Me.tbRadian.UseVisualStyleBackColor = True
        '
        'gbIdentification
        '
        Me.gbIdentification.Controls.Add(Me.lbIdentification)
        Me.gbIdentification.Controls.Add(Me.btnIdentify)
        Me.gbIdentification.Location = New System.Drawing.Point(39, 255)
        Me.gbIdentification.Name = "gbIdentification"
        Me.gbIdentification.Size = New System.Drawing.Size(238, 113)
        Me.gbIdentification.TabIndex = 323
        Me.gbIdentification.TabStop = False
        Me.gbIdentification.Text = "Radian Identification"
        '
        'lbIdentification
        '
        Me.lbIdentification.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
        Me.lbIdentification.FormattingEnabled = True
        Me.lbIdentification.Location = New System.Drawing.Point(6, 16)
        Me.lbIdentification.Name = "lbIdentification"
        Me.lbIdentification.Size = New System.Drawing.Size(226, 65)
        Me.lbIdentification.TabIndex = 58
        '
        'btnIdentify
        '
        Me.btnIdentify.Location = New System.Drawing.Point(6, 87)
        Me.btnIdentify.Name = "btnIdentify"
        Me.btnIdentify.Size = New System.Drawing.Size(91, 22)
        Me.btnIdentify.TabIndex = 57
        Me.btnIdentify.Text = "Identify"
        '
        'lbDisplayAccumMetrics
        '
        Me.lbDisplayAccumMetrics.FormattingEnabled = True
        Me.lbDisplayAccumMetrics.Location = New System.Drawing.Point(396, 26)
        Me.lbDisplayAccumMetrics.Name = "lbDisplayAccumMetrics"
        Me.lbDisplayAccumMetrics.Size = New System.Drawing.Size(139, 173)
        Me.lbDisplayAccumMetrics.TabIndex = 77
        '
        'btnGetAccumMetrics
        '
        Me.btnGetAccumMetrics.Location = New System.Drawing.Point(425, 239)
        Me.btnGetAccumMetrics.Name = "btnGetAccumMetrics"
        Me.btnGetAccumMetrics.Size = New System.Drawing.Size(70, 48)
        Me.btnGetAccumMetrics.TabIndex = 79
        Me.btnGetAccumMetrics.Text = "Update" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Accum" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Metrics"
        '
        'cbSelectAccumMetrics
        '
        Me.cbSelectAccumMetrics.FormattingEnabled = True
        Me.cbSelectAccumMetrics.Location = New System.Drawing.Point(396, 212)
        Me.cbSelectAccumMetrics.Name = "cbSelectAccumMetrics"
        Me.cbSelectAccumMetrics.Size = New System.Drawing.Size(139, 21)
        Me.cbSelectAccumMetrics.TabIndex = 78
        '
        'gbCurrentRange
        '
        Me.gbCurrentRange.Controls.Add(Me.btnRangingUnlockCurrent)
        Me.gbCurrentRange.Controls.Add(Me.chkRangingSetAndLockCurrent)
        Me.gbCurrentRange.Controls.Add(Me.btnRangingLockCurrentRangeToggle)
        Me.gbCurrentRange.Controls.Add(Me.lblRangingCurrentRangeSetPoint)
        Me.gbCurrentRange.Controls.Add(Me.btnRangingGetCurrentRange)
        Me.gbCurrentRange.Controls.Add(Me.cbRangingCurrent)
        Me.gbCurrentRange.Location = New System.Drawing.Point(47, 126)
        Me.gbCurrentRange.Name = "gbCurrentRange"
        Me.gbCurrentRange.Size = New System.Drawing.Size(335, 107)
        Me.gbCurrentRange.TabIndex = 81
        Me.gbCurrentRange.TabStop = False
        Me.gbCurrentRange.Text = "Current Ranging"
        '
        'btnRangingUnlockCurrent
        '
        Me.btnRangingUnlockCurrent.Location = New System.Drawing.Point(236, 62)
        Me.btnRangingUnlockCurrent.Name = "btnRangingUnlockCurrent"
        Me.btnRangingUnlockCurrent.Size = New System.Drawing.Size(81, 39)
        Me.btnRangingUnlockCurrent.TabIndex = 7
        Me.btnRangingUnlockCurrent.Text = "Unlock Range"
        Me.btnRangingUnlockCurrent.UseVisualStyleBackColor = True
        '
        'chkRangingSetAndLockCurrent
        '
        Me.chkRangingSetAndLockCurrent.AutoSize = True
        Me.chkRangingSetAndLockCurrent.Location = New System.Drawing.Point(105, 48)
        Me.chkRangingSetAndLockCurrent.Name = "chkRangingSetAndLockCurrent"
        Me.chkRangingSetAndLockCurrent.Size = New System.Drawing.Size(125, 17)
        Me.chkRangingSetAndLockCurrent.TabIndex = 5
        Me.chkRangingSetAndLockCurrent.Text = "Set and Lock Range"
        Me.chkRangingSetAndLockCurrent.UseVisualStyleBackColor = True
        '
        'btnRangingLockCurrentRangeToggle
        '
        Me.btnRangingLockCurrentRangeToggle.Location = New System.Drawing.Point(15, 42)
        Me.btnRangingLockCurrentRangeToggle.Name = "btnRangingLockCurrentRangeToggle"
        Me.btnRangingLockCurrentRangeToggle.Size = New System.Drawing.Size(81, 26)
        Me.btnRangingLockCurrentRangeToggle.TabIndex = 4
        Me.btnRangingLockCurrentRangeToggle.Text = "Lock Range"
        Me.btnRangingLockCurrentRangeToggle.UseVisualStyleBackColor = True
        '
        'lblRangingCurrentRangeSetPoint
        '
        Me.lblRangingCurrentRangeSetPoint.AutoSize = True
        Me.lblRangingCurrentRangeSetPoint.Location = New System.Drawing.Point(102, 26)
        Me.lblRangingCurrentRangeSetPoint.Name = "lblRangingCurrentRangeSetPoint"
        Me.lblRangingCurrentRangeSetPoint.Size = New System.Drawing.Size(85, 13)
        Me.lblRangingCurrentRangeSetPoint.TabIndex = 3
        Me.lblRangingCurrentRangeSetPoint.Text = "Current Range ="
        '
        'btnRangingGetCurrentRange
        '
        Me.btnRangingGetCurrentRange.Location = New System.Drawing.Point(15, 19)
        Me.btnRangingGetCurrentRange.Name = "btnRangingGetCurrentRange"
        Me.btnRangingGetCurrentRange.Size = New System.Drawing.Size(81, 26)
        Me.btnRangingGetCurrentRange.TabIndex = 2
        Me.btnRangingGetCurrentRange.Text = "Query Range"
        Me.btnRangingGetCurrentRange.UseVisualStyleBackColor = True
        '
        'cbRangingCurrent
        '
        Me.cbRangingCurrent.FormattingEnabled = True
        Me.cbRangingCurrent.Location = New System.Drawing.Point(15, 74)
        Me.cbRangingCurrent.Name = "cbRangingCurrent"
        Me.cbRangingCurrent.Size = New System.Drawing.Size(158, 21)
        Me.cbRangingCurrent.TabIndex = 1
        '
        'gbReset
        '
        Me.gbReset.Controls.Add(Me.chkResetAccumData)
        Me.gbReset.Controls.Add(Me.chkResetMaxData)
        Me.gbReset.Controls.Add(Me.chkResetMinData)
        Me.gbReset.Controls.Add(Me.chkResetInstantaneousData)
        Me.gbReset.Controls.Add(Me.chkResetWaveformBufferToZero)
        Me.gbReset.Controls.Add(Me.btnReset)
        Me.gbReset.Location = New System.Drawing.Point(47, 20)
        Me.gbReset.Name = "gbReset"
        Me.gbReset.Size = New System.Drawing.Size(270, 91)
        Me.gbReset.TabIndex = 80
        Me.gbReset.TabStop = False
        Me.gbReset.Text = "Reset"
        '
        'chkResetAccumData
        '
        Me.chkResetAccumData.AutoSize = True
        Me.chkResetAccumData.Location = New System.Drawing.Point(150, 30)
        Me.chkResetAccumData.Name = "chkResetAccumData"
        Me.chkResetAccumData.Size = New System.Drawing.Size(114, 17)
        Me.chkResetAccumData.TabIndex = 65
        Me.chkResetAccumData.Text = "Accumulated Data"
        Me.chkResetAccumData.UseVisualStyleBackColor = True
        '
        'chkResetMaxData
        '
        Me.chkResetMaxData.AutoSize = True
        Me.chkResetMaxData.Location = New System.Drawing.Point(150, 63)
        Me.chkResetMaxData.Name = "chkResetMaxData"
        Me.chkResetMaxData.Size = New System.Drawing.Size(96, 17)
        Me.chkResetMaxData.TabIndex = 64
        Me.chkResetMaxData.Text = "Maximum Data"
        Me.chkResetMaxData.UseVisualStyleBackColor = True
        '
        'chkResetMinData
        '
        Me.chkResetMinData.AutoSize = True
        Me.chkResetMinData.Location = New System.Drawing.Point(150, 46)
        Me.chkResetMinData.Name = "chkResetMinData"
        Me.chkResetMinData.Size = New System.Drawing.Size(93, 17)
        Me.chkResetMinData.TabIndex = 63
        Me.chkResetMinData.Text = "Minimum Data"
        Me.chkResetMinData.UseVisualStyleBackColor = True
        '
        'chkResetInstantaneousData
        '
        Me.chkResetInstantaneousData.AutoSize = True
        Me.chkResetInstantaneousData.Location = New System.Drawing.Point(16, 64)
        Me.chkResetInstantaneousData.Name = "chkResetInstantaneousData"
        Me.chkResetInstantaneousData.Size = New System.Drawing.Size(119, 17)
        Me.chkResetInstantaneousData.TabIndex = 62
        Me.chkResetInstantaneousData.Text = "Instantaneous Data"
        Me.chkResetInstantaneousData.UseVisualStyleBackColor = True
        '
        'chkResetWaveformBufferToZero
        '
        Me.chkResetWaveformBufferToZero.AutoSize = True
        Me.chkResetWaveformBufferToZero.Location = New System.Drawing.Point(16, 47)
        Me.chkResetWaveformBufferToZero.Name = "chkResetWaveformBufferToZero"
        Me.chkResetWaveformBufferToZero.Size = New System.Drawing.Size(131, 17)
        Me.chkResetWaveformBufferToZero.TabIndex = 61
        Me.chkResetWaveformBufferToZero.Text = "Zero Waveform Buffer"
        Me.chkResetWaveformBufferToZero.UseVisualStyleBackColor = True
        '
        'btnReset
        '
        Me.btnReset.Location = New System.Drawing.Point(16, 19)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(72, 22)
        Me.btnReset.TabIndex = 60
        Me.btnReset.Text = "Reset"
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.txtPacpowerTest)
        Me.TabPage1.Controls.Add(Me.gbPacPowerOutput)
        Me.TabPage1.Controls.Add(Me.gbPacPowerVoltageLineToLine)
        Me.TabPage1.Controls.Add(Me.lbPacPowerLog)
        Me.TabPage1.Controls.Add(Me.gbPacPowerFrequency)
        Me.TabPage1.Controls.Add(Me.gbPacPowerVoltageRMS)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1274, 693)
        Me.TabPage1.TabIndex = 7
        Me.TabPage1.Text = "PAC Power"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'txtPacpowerTest
        '
        Me.txtPacpowerTest.Location = New System.Drawing.Point(7, 268)
        Me.txtPacpowerTest.Name = "txtPacpowerTest"
        Me.txtPacpowerTest.Size = New System.Drawing.Size(153, 20)
        Me.txtPacpowerTest.TabIndex = 345
        Me.txtPacpowerTest.Text = ":SYST:BEEP"
        '
        'gbPacPowerOutput
        '
        Me.gbPacPowerOutput.Controls.Add(Me.btnPacPowerOutputEnable)
        Me.gbPacPowerOutput.Location = New System.Drawing.Point(468, 25)
        Me.gbPacPowerOutput.Name = "gbPacPowerOutput"
        Me.gbPacPowerOutput.Size = New System.Drawing.Size(87, 67)
        Me.gbPacPowerOutput.TabIndex = 7
        Me.gbPacPowerOutput.TabStop = False
        Me.gbPacPowerOutput.Text = "Output"
        '
        'btnPacPowerOutputEnable
        '
        Me.btnPacPowerOutputEnable.Location = New System.Drawing.Point(11, 22)
        Me.btnPacPowerOutputEnable.Name = "btnPacPowerOutputEnable"
        Me.btnPacPowerOutputEnable.Size = New System.Drawing.Size(67, 26)
        Me.btnPacPowerOutputEnable.TabIndex = 6
        Me.btnPacPowerOutputEnable.Text = "Enable"
        Me.btnPacPowerOutputEnable.UseVisualStyleBackColor = True
        '
        'gbPacPowerVoltageLineToLine
        '
        Me.gbPacPowerVoltageLineToLine.Controls.Add(Me.btnPacPowerGetVoltageLineToLine)
        Me.gbPacPowerVoltageLineToLine.Controls.Add(Me.rbPacPowerSetVoltageLineToLineB)
        Me.gbPacPowerVoltageLineToLine.Controls.Add(Me.rbPacPowerSetVoltageLineToLineC)
        Me.gbPacPowerVoltageLineToLine.Controls.Add(Me.rbPacPowerSetVoltageLineToLineAll)
        Me.gbPacPowerVoltageLineToLine.Controls.Add(Me.rbPacPowerSetVoltageLineToLineA)
        Me.gbPacPowerVoltageLineToLine.Location = New System.Drawing.Point(173, 16)
        Me.gbPacPowerVoltageLineToLine.Name = "gbPacPowerVoltageLineToLine"
        Me.gbPacPowerVoltageLineToLine.Size = New System.Drawing.Size(87, 156)
        Me.gbPacPowerVoltageLineToLine.TabIndex = 5
        Me.gbPacPowerVoltageLineToLine.TabStop = False
        Me.gbPacPowerVoltageLineToLine.Text = "Voltage (L-L)"
        '
        'btnPacPowerGetVoltageLineToLine
        '
        Me.btnPacPowerGetVoltageLineToLine.Location = New System.Drawing.Point(11, 22)
        Me.btnPacPowerGetVoltageLineToLine.Name = "btnPacPowerGetVoltageLineToLine"
        Me.btnPacPowerGetVoltageLineToLine.Size = New System.Drawing.Size(67, 26)
        Me.btnPacPowerGetVoltageLineToLine.TabIndex = 6
        Me.btnPacPowerGetVoltageLineToLine.Text = "Get"
        Me.btnPacPowerGetVoltageLineToLine.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageLineToLineB
        '
        Me.rbPacPowerSetVoltageLineToLineB.AutoSize = True
        Me.rbPacPowerSetVoltageLineToLineB.Location = New System.Drawing.Point(12, 93)
        Me.rbPacPowerSetVoltageLineToLineB.Name = "rbPacPowerSetVoltageLineToLineB"
        Me.rbPacPowerSetVoltageLineToLineB.Size = New System.Drawing.Size(56, 17)
        Me.rbPacPowerSetVoltageLineToLineB.TabIndex = 4
        Me.rbPacPowerSetVoltageLineToLineB.Text = "B Only"
        Me.rbPacPowerSetVoltageLineToLineB.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageLineToLineC
        '
        Me.rbPacPowerSetVoltageLineToLineC.AutoSize = True
        Me.rbPacPowerSetVoltageLineToLineC.Location = New System.Drawing.Point(12, 110)
        Me.rbPacPowerSetVoltageLineToLineC.Name = "rbPacPowerSetVoltageLineToLineC"
        Me.rbPacPowerSetVoltageLineToLineC.Size = New System.Drawing.Size(56, 17)
        Me.rbPacPowerSetVoltageLineToLineC.TabIndex = 3
        Me.rbPacPowerSetVoltageLineToLineC.Text = "C Only"
        Me.rbPacPowerSetVoltageLineToLineC.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageLineToLineAll
        '
        Me.rbPacPowerSetVoltageLineToLineAll.AutoSize = True
        Me.rbPacPowerSetVoltageLineToLineAll.Checked = True
        Me.rbPacPowerSetVoltageLineToLineAll.Location = New System.Drawing.Point(12, 59)
        Me.rbPacPowerSetVoltageLineToLineAll.Name = "rbPacPowerSetVoltageLineToLineAll"
        Me.rbPacPowerSetVoltageLineToLineAll.Size = New System.Drawing.Size(44, 17)
        Me.rbPacPowerSetVoltageLineToLineAll.TabIndex = 2
        Me.rbPacPowerSetVoltageLineToLineAll.TabStop = True
        Me.rbPacPowerSetVoltageLineToLineAll.Text = "ALL"
        Me.rbPacPowerSetVoltageLineToLineAll.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageLineToLineA
        '
        Me.rbPacPowerSetVoltageLineToLineA.AutoSize = True
        Me.rbPacPowerSetVoltageLineToLineA.Location = New System.Drawing.Point(12, 76)
        Me.rbPacPowerSetVoltageLineToLineA.Name = "rbPacPowerSetVoltageLineToLineA"
        Me.rbPacPowerSetVoltageLineToLineA.Size = New System.Drawing.Size(56, 17)
        Me.rbPacPowerSetVoltageLineToLineA.TabIndex = 1
        Me.rbPacPowerSetVoltageLineToLineA.Text = "A Only"
        Me.rbPacPowerSetVoltageLineToLineA.UseVisualStyleBackColor = True
        '
        'lbPacPowerLog
        '
        Me.lbPacPowerLog.FormattingEnabled = True
        Me.lbPacPowerLog.Location = New System.Drawing.Point(911, 16)
        Me.lbPacPowerLog.Name = "lbPacPowerLog"
        Me.lbPacPowerLog.Size = New System.Drawing.Size(353, 667)
        Me.lbPacPowerLog.TabIndex = 4
        '
        'gbPacPowerFrequency
        '
        Me.gbPacPowerFrequency.Controls.Add(Me.txtPacPowerGetFrequency)
        Me.gbPacPowerFrequency.Controls.Add(Me.btnPacPowerGetFrequency)
        Me.gbPacPowerFrequency.Controls.Add(Me.txtPacPowerSetFrequency)
        Me.gbPacPowerFrequency.Controls.Add(Me.btnPacPowerSetFrequency)
        Me.gbPacPowerFrequency.Location = New System.Drawing.Point(8, 178)
        Me.gbPacPowerFrequency.Name = "gbPacPowerFrequency"
        Me.gbPacPowerFrequency.Size = New System.Drawing.Size(159, 84)
        Me.gbPacPowerFrequency.TabIndex = 3
        Me.gbPacPowerFrequency.TabStop = False
        Me.gbPacPowerFrequency.Text = "Frequency"
        '
        'txtPacPowerGetFrequency
        '
        Me.txtPacPowerGetFrequency.Location = New System.Drawing.Point(85, 54)
        Me.txtPacPowerGetFrequency.Name = "txtPacPowerGetFrequency"
        Me.txtPacPowerGetFrequency.ReadOnly = True
        Me.txtPacPowerGetFrequency.Size = New System.Drawing.Size(67, 20)
        Me.txtPacPowerGetFrequency.TabIndex = 7
        '
        'btnPacPowerGetFrequency
        '
        Me.btnPacPowerGetFrequency.Location = New System.Drawing.Point(85, 22)
        Me.btnPacPowerGetFrequency.Name = "btnPacPowerGetFrequency"
        Me.btnPacPowerGetFrequency.Size = New System.Drawing.Size(67, 26)
        Me.btnPacPowerGetFrequency.TabIndex = 6
        Me.btnPacPowerGetFrequency.Text = "Get"
        Me.btnPacPowerGetFrequency.UseVisualStyleBackColor = True
        '
        'txtPacPowerSetFrequency
        '
        Me.txtPacPowerSetFrequency.Location = New System.Drawing.Point(12, 54)
        Me.txtPacPowerSetFrequency.Name = "txtPacPowerSetFrequency"
        Me.txtPacPowerSetFrequency.Size = New System.Drawing.Size(67, 20)
        Me.txtPacPowerSetFrequency.TabIndex = 5
        Me.txtPacPowerSetFrequency.Text = "120.0"
        '
        'btnPacPowerSetFrequency
        '
        Me.btnPacPowerSetFrequency.Location = New System.Drawing.Point(12, 22)
        Me.btnPacPowerSetFrequency.Name = "btnPacPowerSetFrequency"
        Me.btnPacPowerSetFrequency.Size = New System.Drawing.Size(67, 26)
        Me.btnPacPowerSetFrequency.TabIndex = 0
        Me.btnPacPowerSetFrequency.Text = "Set"
        Me.btnPacPowerSetFrequency.UseVisualStyleBackColor = True
        '
        'gbPacPowerVoltageRMS
        '
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.btnPacPowerGetVoltageRMS)
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.txtPacPowerSetVoltageRMS)
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.rbPacPowerSetVoltageRMSB)
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.rbPacPowerSetVoltageRMSC)
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.rbPacPowerSetVoltageRMSAll)
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.rbPacPowerSetVoltageRMSA)
        Me.gbPacPowerVoltageRMS.Controls.Add(Me.btnPacPowerSetVoltageRMS)
        Me.gbPacPowerVoltageRMS.Location = New System.Drawing.Point(8, 16)
        Me.gbPacPowerVoltageRMS.Name = "gbPacPowerVoltageRMS"
        Me.gbPacPowerVoltageRMS.Size = New System.Drawing.Size(159, 156)
        Me.gbPacPowerVoltageRMS.TabIndex = 1
        Me.gbPacPowerVoltageRMS.TabStop = False
        Me.gbPacPowerVoltageRMS.Text = "Voltage (RMS)"
        '
        'btnPacPowerGetVoltageRMS
        '
        Me.btnPacPowerGetVoltageRMS.Location = New System.Drawing.Point(85, 22)
        Me.btnPacPowerGetVoltageRMS.Name = "btnPacPowerGetVoltageRMS"
        Me.btnPacPowerGetVoltageRMS.Size = New System.Drawing.Size(67, 26)
        Me.btnPacPowerGetVoltageRMS.TabIndex = 6
        Me.btnPacPowerGetVoltageRMS.Text = "Get"
        Me.btnPacPowerGetVoltageRMS.UseVisualStyleBackColor = True
        '
        'txtPacPowerSetVoltageRMS
        '
        Me.txtPacPowerSetVoltageRMS.Location = New System.Drawing.Point(12, 54)
        Me.txtPacPowerSetVoltageRMS.Name = "txtPacPowerSetVoltageRMS"
        Me.txtPacPowerSetVoltageRMS.Size = New System.Drawing.Size(67, 20)
        Me.txtPacPowerSetVoltageRMS.TabIndex = 5
        Me.txtPacPowerSetVoltageRMS.Text = "120.0"
        '
        'rbPacPowerSetVoltageRMSB
        '
        Me.rbPacPowerSetVoltageRMSB.AutoSize = True
        Me.rbPacPowerSetVoltageRMSB.Location = New System.Drawing.Point(12, 114)
        Me.rbPacPowerSetVoltageRMSB.Name = "rbPacPowerSetVoltageRMSB"
        Me.rbPacPowerSetVoltageRMSB.Size = New System.Drawing.Size(56, 17)
        Me.rbPacPowerSetVoltageRMSB.TabIndex = 4
        Me.rbPacPowerSetVoltageRMSB.Text = "B Only"
        Me.rbPacPowerSetVoltageRMSB.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageRMSC
        '
        Me.rbPacPowerSetVoltageRMSC.AutoSize = True
        Me.rbPacPowerSetVoltageRMSC.Location = New System.Drawing.Point(12, 131)
        Me.rbPacPowerSetVoltageRMSC.Name = "rbPacPowerSetVoltageRMSC"
        Me.rbPacPowerSetVoltageRMSC.Size = New System.Drawing.Size(56, 17)
        Me.rbPacPowerSetVoltageRMSC.TabIndex = 3
        Me.rbPacPowerSetVoltageRMSC.Text = "C Only"
        Me.rbPacPowerSetVoltageRMSC.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageRMSAll
        '
        Me.rbPacPowerSetVoltageRMSAll.AutoSize = True
        Me.rbPacPowerSetVoltageRMSAll.Checked = True
        Me.rbPacPowerSetVoltageRMSAll.Location = New System.Drawing.Point(12, 80)
        Me.rbPacPowerSetVoltageRMSAll.Name = "rbPacPowerSetVoltageRMSAll"
        Me.rbPacPowerSetVoltageRMSAll.Size = New System.Drawing.Size(44, 17)
        Me.rbPacPowerSetVoltageRMSAll.TabIndex = 2
        Me.rbPacPowerSetVoltageRMSAll.TabStop = True
        Me.rbPacPowerSetVoltageRMSAll.Text = "ALL"
        Me.rbPacPowerSetVoltageRMSAll.UseVisualStyleBackColor = True
        '
        'rbPacPowerSetVoltageRMSA
        '
        Me.rbPacPowerSetVoltageRMSA.AutoSize = True
        Me.rbPacPowerSetVoltageRMSA.Location = New System.Drawing.Point(12, 97)
        Me.rbPacPowerSetVoltageRMSA.Name = "rbPacPowerSetVoltageRMSA"
        Me.rbPacPowerSetVoltageRMSA.Size = New System.Drawing.Size(56, 17)
        Me.rbPacPowerSetVoltageRMSA.TabIndex = 1
        Me.rbPacPowerSetVoltageRMSA.Text = "A Only"
        Me.rbPacPowerSetVoltageRMSA.UseVisualStyleBackColor = True
        '
        'btnPacPowerSetVoltageRMS
        '
        Me.btnPacPowerSetVoltageRMS.Location = New System.Drawing.Point(12, 22)
        Me.btnPacPowerSetVoltageRMS.Name = "btnPacPowerSetVoltageRMS"
        Me.btnPacPowerSetVoltageRMS.Size = New System.Drawing.Size(67, 26)
        Me.btnPacPowerSetVoltageRMS.TabIndex = 0
        Me.btnPacPowerSetVoltageRMS.Text = "Set"
        Me.btnPacPowerSetVoltageRMS.UseVisualStyleBackColor = True
        '
        'TimerRunning
        '
        '
        'TimerProgram
        '
        '
        'ErrDataLogger
        '
        Me.ErrDataLogger.ContainerControl = Me
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        '
        'TimerSample
        '
        '
        'TimerRadian
        '
        '
        'ErrorProvider
        '
        Me.ErrorProvider.ContainerControl = Me
        '
        'frmCurrent_Temperature
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1280, 717)
        Me.Controls.Add(Me.tcCurrent_Temperature)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmCurrent_Temperature"
        Me.Text = "Current & Temperature"
        Me.tcCurrent_Temperature.ResumeLayout(False)
        Me.DAQ.ResumeLayout(False)
        Me.DAQ.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.boardIdNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.primaryAddressNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbPacPowerASX115.ResumeLayout(False)
        Me.gbPacPowerASX115.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.gbDAQConnection.ResumeLayout(False)
        Me.panelModule1.ResumeLayout(False)
        Me.panelModule1.PerformLayout()
        Me.panelModule2.ResumeLayout(False)
        Me.panelModule2.PerformLayout()
        Me.panelModule3.ResumeLayout(False)
        Me.panelModule3.PerformLayout()
        Me.gbControlSettings.ResumeLayout(False)
        Me.gbControlSettings.PerformLayout()
        Me.gbDAQVoltage.ResumeLayout(False)
        Me.gbDAQVoltage.PerformLayout()
        Me.gbRadianRS232.ResumeLayout(False)
        Me.gbRadianRS232.PerformLayout()
        Me.gbDAQNumberofReadings.ResumeLayout(False)
        Me.gbDAQNumberofReadings.PerformLayout()
        Me.gbCountDown.ResumeLayout(False)
        Me.gbCountDown.PerformLayout()
        Me.gbDAQDataGrid.ResumeLayout(False)
        Me.gbDAQDataGrid.PerformLayout()
        CType(Me.dgvDAQData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnThreshold.ResumeLayout(False)
        Me.pnThreshold.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.gbMode.ResumeLayout(False)
        Me.gbMode.PerformLayout()
        Me.gbInformation.ResumeLayout(False)
        Me.gbInformation.PerformLayout()
        Me.gbParameters.ResumeLayout(False)
        Me.gbParameters.PerformLayout()
        Me.Plot.ResumeLayout(False)
        Me.Plot.PerformLayout()
        CType(Me.CurrentChart, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbPlotLineWidth.ResumeLayout(False)
        Me.Log.ResumeLayout(False)
        Me.Log.PerformLayout()
        Me.gbDAQIdentification.ResumeLayout(False)
        Me.CurrentController.ResumeLayout(False)
        Me.CurrentController.PerformLayout()
        Me.gbDataLogger.ResumeLayout(False)
        Me.gbDataLogger.PerformLayout()
        Me.gbSaveData.ResumeLayout(False)
        Me.gbSaveData.PerformLayout()
        Me.gbMetrics.ResumeLayout(False)
        Me.ErrorLog.ResumeLayout(False)
        Me.ErrorLog.PerformLayout()
        Me.tbRadian.ResumeLayout(False)
        Me.gbIdentification.ResumeLayout(False)
        Me.gbCurrentRange.ResumeLayout(False)
        Me.gbCurrentRange.PerformLayout()
        Me.gbReset.ResumeLayout(False)
        Me.gbReset.PerformLayout()
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.gbPacPowerOutput.ResumeLayout(False)
        Me.gbPacPowerVoltageLineToLine.ResumeLayout(False)
        Me.gbPacPowerVoltageLineToLine.PerformLayout()
        Me.gbPacPowerFrequency.ResumeLayout(False)
        Me.gbPacPowerFrequency.PerformLayout()
        Me.gbPacPowerVoltageRMS.ResumeLayout(False)
        Me.gbPacPowerVoltageRMS.PerformLayout()
        CType(Me.ErrDataLogger, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tcCurrent_Temperature As System.Windows.Forms.TabControl
    Friend WithEvents CurrentController As System.Windows.Forms.TabPage
    Friend WithEvents btnTest As System.Windows.Forms.Button
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents btnSaveGPIB_LOG As System.Windows.Forms.Button
    Private WithEvents btnCI501TCA As System.Windows.Forms.Button
    Friend WithEvents gbSaveData As System.Windows.Forms.GroupBox
    Friend WithEvents btnRadianLoggerSaveData As System.Windows.Forms.Button
    Friend WithEvents lblDataSaveDelimiter As System.Windows.Forms.Label
    Friend WithEvents rbDataSaveComma As System.Windows.Forms.RadioButton
    Friend WithEvents rbDataSaveSpace As System.Windows.Forms.RadioButton
    Friend WithEvents rbDataSaveTab As System.Windows.Forms.RadioButton
    Friend WithEvents rbDataSaveSemiColon As System.Windows.Forms.RadioButton
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtLogDataNotes As System.Windows.Forms.TextBox
    Friend WithEvents chkCheckToSaveData As System.Windows.Forms.CheckBox
    Friend WithEvents chkMeterTestSaveDataOverWrite As System.Windows.Forms.CheckBox
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents btnDataSaveLocation As System.Windows.Forms.Button
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtFolderSave As System.Windows.Forms.TextBox
    Friend WithEvents txtDataFileName As System.Windows.Forms.TextBox
    Friend WithEvents lbGPIB_Log As System.Windows.Forms.ListBox
    Friend WithEvents gbMetrics As System.Windows.Forms.GroupBox
    Friend WithEvents cbSelectInstantMetrics As System.Windows.Forms.ComboBox
    Friend WithEvents btnGetInstantMetrics As System.Windows.Forms.Button
    Friend WithEvents chkGPIBVerbose As System.Windows.Forms.CheckBox
    Friend WithEvents btnGPIB_LogClear As System.Windows.Forms.Button
    Friend WithEvents Plot As System.Windows.Forms.TabPage
    Friend WithEvents gbPlotLineWidth As System.Windows.Forms.GroupBox
    Friend WithEvents btnPlotSetLineWidth As System.Windows.Forms.Button
    Friend WithEvents cbPlotLineWidth As System.Windows.Forms.ComboBox
    Friend WithEvents lblPlotReminder As System.Windows.Forms.Label
    Friend WithEvents btnPlotLoadPlot As System.Windows.Forms.Button
    Friend WithEvents btnPlotSavePlot As System.Windows.Forms.Button
    Friend WithEvents btnPlotResetXY As System.Windows.Forms.Button
    Friend WithEvents btnPlotResetY As System.Windows.Forms.Button
    Friend WithEvents btnPlotResetX As System.Windows.Forms.Button
    Friend WithEvents Log As System.Windows.Forms.TabPage
    Friend WithEvents chkErrorLogVerbose As System.Windows.Forms.CheckBox
    Friend WithEvents lstDAQLog As System.Windows.Forms.ListBox
    Friend WithEvents lstDAQError As System.Windows.Forms.ListBox
    Friend WithEvents DataChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents TimerRunning As System.Windows.Forms.Timer
    Friend WithEvents TimerProgram As System.Windows.Forms.Timer
    Friend WithEvents ErrDataLogger As System.Windows.Forms.ErrorProvider
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents ErrorLog As System.Windows.Forms.TabPage
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents chkVerbose As System.Windows.Forms.CheckBox
    Friend WithEvents btnClearLog As System.Windows.Forms.Button
    Friend WithEvents btnSaveLog As System.Windows.Forms.Button
    Friend WithEvents lblLogData As System.Windows.Forms.ListBox
    Friend WithEvents tbRadian As System.Windows.Forms.TabPage
    Friend WithEvents lbDisplayAccumMetrics As System.Windows.Forms.ListBox
    Friend WithEvents btnGetAccumMetrics As System.Windows.Forms.Button
    Friend WithEvents cbSelectAccumMetrics As System.Windows.Forms.ComboBox
    Friend WithEvents gbCurrentRange As System.Windows.Forms.GroupBox
    Friend WithEvents btnRangingUnlockCurrent As System.Windows.Forms.Button
    Friend WithEvents chkRangingSetAndLockCurrent As System.Windows.Forms.CheckBox
    Friend WithEvents btnRangingLockCurrentRangeToggle As System.Windows.Forms.Button
    Friend WithEvents lblRangingCurrentRangeSetPoint As System.Windows.Forms.Label
    Friend WithEvents btnRangingGetCurrentRange As System.Windows.Forms.Button
    Friend WithEvents cbRangingCurrent As System.Windows.Forms.ComboBox
    Friend WithEvents gbReset As System.Windows.Forms.GroupBox
    Friend WithEvents chkResetAccumData As System.Windows.Forms.CheckBox
    Friend WithEvents chkResetMaxData As System.Windows.Forms.CheckBox
    Friend WithEvents chkResetMinData As System.Windows.Forms.CheckBox
    Friend WithEvents chkResetInstantaneousData As System.Windows.Forms.CheckBox
    Friend WithEvents chkResetWaveformBufferToZero As System.Windows.Forms.CheckBox
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents TimerSample As System.Windows.Forms.Timer
    Friend WithEvents TimerRadian As System.Windows.Forms.Timer
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents rbPlotCurrent As System.Windows.Forms.RadioButton
    Friend WithEvents rbPlotTemperature As System.Windows.Forms.RadioButton
    Friend WithEvents CurrentChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents gbDataLogger As System.Windows.Forms.GroupBox
    Friend WithEvents lblDataLoggerSampleInterval As System.Windows.Forms.Label
    Friend WithEvents lblDataLoggerSeconds As System.Windows.Forms.Label
    Friend WithEvents txtDataLoggerInterval As System.Windows.Forms.TextBox
    Friend WithEvents btnDataLoggerToggleLogger As System.Windows.Forms.Button
    Friend WithEvents lbDisplayInstantMetrics As System.Windows.Forms.ListBox
    Friend WithEvents ErrorProvider As System.Windows.Forms.ErrorProvider
    Friend WithEvents DAQ As System.Windows.Forms.TabPage
    Friend WithEvents lblTime As System.Windows.Forms.Label
    Friend WithEvents chbAutomaticShutDown As System.Windows.Forms.CheckBox
    Friend WithEvents gbControlSettings As System.Windows.Forms.GroupBox
    Friend WithEvents lblAccuracyDeadBand As System.Windows.Forms.Label
    Friend WithEvents txtVoltage_ControlDeadBand As System.Windows.Forms.TextBox
    Friend WithEvents lblControlDeadBand As System.Windows.Forms.Label
    Friend WithEvents cbVoltage_CurrentSetpoint As System.Windows.Forms.ComboBox
    Friend WithEvents chkCloseLoop As System.Windows.Forms.CheckBox
    Friend WithEvents txtVoltage_AccuracyDeadband As System.Windows.Forms.TextBox
    Friend WithEvents gbDAQVoltage As System.Windows.Forms.GroupBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents btnVoltageOff As System.Windows.Forms.Button
    Private WithEvents btnSetVoltage As System.Windows.Forms.Button
    Friend WithEvents txtSetVoltage As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents boardIdNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents secondaryAddressComboBox As System.Windows.Forms.ComboBox
    Private WithEvents secondaryAddressLabel As System.Windows.Forms.Label
    Private WithEvents primaryAddressLabel As System.Windows.Forms.Label
    Private WithEvents primaryAddressNumericUpDown As System.Windows.Forms.NumericUpDown
    Private WithEvents closeButton As System.Windows.Forms.Button
    Private WithEvents boardIdLabel As System.Windows.Forms.Label
    Private WithEvents openButton As System.Windows.Forms.Button
    Friend WithEvents btnSaveConfig As System.Windows.Forms.Button
    Friend WithEvents btnLoadConfig As System.Windows.Forms.Button
    Friend WithEvents gbRadianRS232 As System.Windows.Forms.GroupBox
    Friend WithEvents cbMeter_COMPorts As System.Windows.Forms.ComboBox
    Friend WithEvents lblMeterComPort As System.Windows.Forms.Label
    Friend WithEvents lblMeterComBaudRate As System.Windows.Forms.Label
    Friend WithEvents txtMeterComBaudrate As System.Windows.Forms.TextBox
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents gbDAQNumberofReadings As System.Windows.Forms.GroupBox
    Friend WithEvents lblDAQTotally As System.Windows.Forms.Label
    Friend WithEvents lblDAQReadings As System.Windows.Forms.Label
    Friend WithEvents txtDAQNumberofReadings As System.Windows.Forms.TextBox
    Friend WithEvents txtMachineState As System.Windows.Forms.Label
    Friend WithEvents gbCountDown As System.Windows.Forms.GroupBox
    Friend WithEvents txtDAQCountDownHH As System.Windows.Forms.TextBox
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents txtDAQCountDownMM As System.Windows.Forms.TextBox
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents txtDAQCountDownSS As System.Windows.Forms.TextBox
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents gbDAQDataGrid As System.Windows.Forms.GroupBox
    Friend WithEvents txtDataLoggerHeader As System.Windows.Forms.TextBox
    Friend WithEvents chkDataLoggerPhase As System.Windows.Forms.CheckBox
    Friend WithEvents chkDataLoggerFrequency As System.Windows.Forms.CheckBox
    Friend WithEvents chkDataLoggerCurrent As System.Windows.Forms.CheckBox
    Friend WithEvents chkDataLoggerVolts As System.Windows.Forms.CheckBox
    Friend WithEvents lbDataLog As System.Windows.Forms.ListBox
    Friend WithEvents dgvDAQData As System.Windows.Forms.DataGridView
    Friend WithEvents currentReading As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents maxReading As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents minReading As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents averageReading As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tempRisen As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents pnThreshold As System.Windows.Forms.Panel
    Friend WithEvents txtDAQCompareThreshold5 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQCompareThreshold4 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQCompare1_Check As System.Windows.Forms.CheckBox
    Friend WithEvents txtDAQCompareThreshold3 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQCompare2_Check As System.Windows.Forms.CheckBox
    Friend WithEvents txtDAQCompareThreshold2 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQCompare3_Check As System.Windows.Forms.CheckBox
    Friend WithEvents txtDAQCompareThreshold1 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQCompare4_Check As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQCompare5_Check As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents cbDAQEmailThreshold As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQEmailTestDone As System.Windows.Forms.CheckBox
    Friend WithEvents chbDAQEmailAddress As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQEmailNotification As System.Windows.Forms.CheckBox
    Friend WithEvents btnDAQEmailView As System.Windows.Forms.Button
    Friend WithEvents btnDAQEmailAdd As System.Windows.Forms.Button
    Friend WithEvents lblDAQEmailExt As System.Windows.Forms.Label
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare10 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare8 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare6 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare4 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare9 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare7 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare5 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare3 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare2 As System.Windows.Forms.Label
    Friend WithEvents lblDAQCompareCompare1 As System.Windows.Forms.Label
    Friend WithEvents txtDAQCompare5 As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtDAQCompare4 As System.Windows.Forms.TextBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtDAQCompare3 As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtDAQCompare2 As System.Windows.Forms.TextBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents cbDAQCompare10 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare9 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare8 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare6 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare7 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare4 As System.Windows.Forms.ComboBox
    Friend WithEvents txtDAQCompare1 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQCompare2 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare5 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare1 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQCompare3 As System.Windows.Forms.ComboBox
    Friend WithEvents gbMode As System.Windows.Forms.GroupBox
    Friend WithEvents rbNumofReadings As System.Windows.Forms.RadioButton
    Friend WithEvents rbDuration As System.Windows.Forms.RadioButton
    Friend WithEvents rbFree As System.Windows.Forms.RadioButton
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents btnDAQStartStopReading As System.Windows.Forms.Button
    Friend WithEvents gbInformation As System.Windows.Forms.GroupBox
    Friend WithEvents lbDAQInformationNumofReadings As System.Windows.Forms.Label
    Friend WithEvents lbDAQInformationDuration As System.Windows.Forms.Label
    Friend WithEvents lbDAQInformationStartTime As System.Windows.Forms.Label
    Friend WithEvents lbDAQInformationTerminateTime As System.Windows.Forms.Label
    Friend WithEvents gbParameters As System.Windows.Forms.GroupBox
    Friend WithEvents rbDAQParametersThermocouple As System.Windows.Forms.RadioButton
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents rbDAQParametersFRTD As System.Windows.Forms.RadioButton
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cbDAQParametersReadingIntervals As System.Windows.Forms.ComboBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents gbDAQConnection As System.Windows.Forms.GroupBox
    Friend WithEvents txtDAQConnectionBaudrate As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQConnectionComPort As System.Windows.Forms.ComboBox
    Friend WithEvents lblDAQConnectionComPort As System.Windows.Forms.Label
    Friend WithEvents lblDAQConnectionBaudRate As System.Windows.Forms.Label
    Friend WithEvents btnDAQConnectionConnect As System.Windows.Forms.Button
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents chbDAQCalibration As System.Windows.Forms.CheckBox
    Friend WithEvents rbSlot300 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSlot200 As System.Windows.Forms.RadioButton
    Friend WithEvents rbSlot100 As System.Windows.Forms.RadioButton
    Friend WithEvents panelModule1 As System.Windows.Forms.Panel
    Friend WithEvents txtDAQGain118 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain119 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain120 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain117 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain116 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain115 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain114 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain113 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain112 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain111 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain109 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain110 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain108 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain107 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain106 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain105 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain104 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain103 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain102 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain101 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset118 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset119 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset120 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset117 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset116 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset115 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset114 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset113 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset112 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset111 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset109 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset110 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset108 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset107 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset106 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset105 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset104 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset103 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset102 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset101 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQDataChannel112 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel110 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel120 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel109 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel111 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel108 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel120 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel107 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel111 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel106 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel119 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel105 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel112 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel104 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel119 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel103 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel113 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel102 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel101 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel118 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel113 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel118 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel114 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel117 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel114 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel117 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel115 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel116 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel115 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel116 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel101 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel102 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel103 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel104 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel105 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel106 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel107 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel108 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel109 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel110 As System.Windows.Forms.CheckBox
    Friend WithEvents panelModule2 As System.Windows.Forms.Panel
    Friend WithEvents txtDAQGain218 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain219 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain220 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain217 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain216 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain215 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain214 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain213 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain212 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain211 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain209 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain210 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain208 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain207 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain206 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain205 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain204 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain203 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain202 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain201 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset218 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset219 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset220 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset217 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset216 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset215 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset214 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset213 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset212 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset211 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset209 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset210 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset208 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset207 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset206 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset205 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset204 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset203 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset202 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset201 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQDataChannel212 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel210 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel220 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel209 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel211 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel208 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel220 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel207 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel211 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel206 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel219 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel205 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel212 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel204 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel219 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel203 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel213 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel202 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel201 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel218 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel213 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel218 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel214 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel217 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel214 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel217 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel215 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel216 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel215 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel216 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel201 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel202 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel203 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel204 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel205 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel206 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel207 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel208 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel209 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel210 As System.Windows.Forms.CheckBox
    Friend WithEvents panelModule3 As System.Windows.Forms.Panel
    Friend WithEvents txtDAQGain318 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain319 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain320 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain317 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain316 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain315 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain314 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain313 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain312 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain311 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain309 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain310 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain308 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain307 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain306 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain305 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain304 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain303 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain302 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQGain301 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset318 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset319 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset320 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset317 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset316 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset315 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset314 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset313 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset312 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset311 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset309 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset310 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset308 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset307 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset306 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset305 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset304 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset303 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset302 As System.Windows.Forms.TextBox
    Friend WithEvents txtDAQOffset301 As System.Windows.Forms.TextBox
    Friend WithEvents cbDAQDataChannel312 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel310 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel320 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel309 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel311 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel308 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel320 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel307 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel311 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel306 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel319 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel305 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel312 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel304 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel319 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel303 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel313 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel302 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel301 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel318 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel313 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel318 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel314 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel317 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel314 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel317 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel315 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataChannel316 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataChannel315 As System.Windows.Forms.ComboBox
    Friend WithEvents cbDAQDataEnableChannel316 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel301 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel302 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel303 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel304 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel305 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel306 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel307 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel308 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel309 As System.Windows.Forms.CheckBox
    Friend WithEvents cbDAQDataEnableChannel310 As System.Windows.Forms.CheckBox
    Friend WithEvents gbDAQIdentification As System.Windows.Forms.GroupBox
    Friend WithEvents lbDAQIdentification As System.Windows.Forms.ListBox
    Friend WithEvents btnDAQIdentificationIdentity As System.Windows.Forms.Button
    Friend WithEvents gbIdentification As System.Windows.Forms.GroupBox
    Friend WithEvents lbIdentification As System.Windows.Forms.ListBox
    Friend WithEvents btnIdentify As System.Windows.Forms.Button
    Friend WithEvents gbPacPowerASX115 As System.Windows.Forms.GroupBox
    Friend WithEvents cbPacPowerPorts As System.Windows.Forms.ComboBox
    Friend WithEvents lblPacPowerComPort As System.Windows.Forms.Label
    Friend WithEvents lblPacPowerBaudRate As System.Windows.Forms.Label
    Friend WithEvents txtPacPowerBaudrate As System.Windows.Forms.TextBox
    Friend WithEvents btnPacPowerConnect As System.Windows.Forms.Button
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents gbPacPowerVoltageRMS As System.Windows.Forms.GroupBox
    Friend WithEvents btnPacPowerSetVoltageRMS As System.Windows.Forms.Button
    Friend WithEvents txtPacPowerSetVoltageRMS As System.Windows.Forms.TextBox
    Friend WithEvents rbPacPowerSetVoltageRMSB As System.Windows.Forms.RadioButton
    Friend WithEvents rbPacPowerSetVoltageRMSC As System.Windows.Forms.RadioButton
    Friend WithEvents rbPacPowerSetVoltageRMSAll As System.Windows.Forms.RadioButton
    Friend WithEvents rbPacPowerSetVoltageRMSA As System.Windows.Forms.RadioButton
    Friend WithEvents gbPacPowerFrequency As System.Windows.Forms.GroupBox
    Friend WithEvents txtPacPowerGetFrequency As System.Windows.Forms.TextBox
    Friend WithEvents btnPacPowerGetFrequency As System.Windows.Forms.Button
    Friend WithEvents txtPacPowerSetFrequency As System.Windows.Forms.TextBox
    Friend WithEvents btnPacPowerSetFrequency As System.Windows.Forms.Button
    Friend WithEvents btnPacPowerGetVoltageRMS As System.Windows.Forms.Button
    Friend WithEvents lbPacPowerLog As System.Windows.Forms.ListBox
    Friend WithEvents gbPacPowerVoltageLineToLine As System.Windows.Forms.GroupBox
    Friend WithEvents btnPacPowerGetVoltageLineToLine As System.Windows.Forms.Button
    Friend WithEvents rbPacPowerSetVoltageLineToLineB As System.Windows.Forms.RadioButton
    Friend WithEvents rbPacPowerSetVoltageLineToLineC As System.Windows.Forms.RadioButton
    Friend WithEvents rbPacPowerSetVoltageLineToLineAll As System.Windows.Forms.RadioButton
    Friend WithEvents rbPacPowerSetVoltageLineToLineA As System.Windows.Forms.RadioButton
    Friend WithEvents gbPacPowerOutput As System.Windows.Forms.GroupBox
    Friend WithEvents btnPacPowerOutputEnable As System.Windows.Forms.Button
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents rbUseCaliforniaInstruments As System.Windows.Forms.RadioButton
    Friend WithEvents rbUsePacificPower As System.Windows.Forms.RadioButton
    Friend WithEvents txtPacpowerTest As System.Windows.Forms.TextBox

End Class
