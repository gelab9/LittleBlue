
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.ComponentModel
Imports System.IO
Imports System.IO.Ports.SerialPort
Imports System.Text.RegularExpressions
Imports System.Math


Public Module mRadian
    Public Class Radian
        ''' <summary>
        ''' Transmit and recieve buffer to communicate with Radian
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landi+Gyr</remarks>
        Public aBytes(4095) As Byte
        'General all purpose buffer for passing data in out of Class
        Public strBuffer As String

        Public ControlByteStatus As Byte
        Public PulsesLeft As UShort
        Public NumberOfTestsToRun As Integer
        Public LoopTest As Boolean
        Public StartTime
        Public EndTIme
        Public Interval
        Public RelayBits As sChannelUShort
        Public TimeLeftForTest As Single
        Public WithEvents bgwRadianComm As BackgroundWorker
        Public IsConnected As Boolean = False
        Public Enum eMessageState
            IDLE
            WaitingForStart
            WaitingForLength
            WaitingForData
            WaitingForEnd
            ValidatingData
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
        Public Const RX_BUFFER_SIZE = 4096
        ''' <summary>
        ''' Serial port for communicating With Radian Standard
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public WithEvents SerialPort As New System.IO.Ports.SerialPort


        'create thread for communication
        Private Sub bgwContScanUSB_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwRadianComm.DoWork

        End Sub
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
        Public Sub v_InitSerialPort(ByVal iBaudrate As Integer, ByVal Parity As Ports.Parity, ByVal StopBits As Ports.StopBits, ByVal iDataBits As Integer, ByVal strComPort As String, ByVal bDTREnable As Boolean) ', ByVal HandShaking As IO.Ports.Handshake)
            Try
                'SerialPort.InitializeLifetimeService()
                SerialPort.BaudRate = iBaudrate
                SerialPort.Parity = Parity
                SerialPort.StopBits = StopBits
                SerialPort.PortName = strComPort
                SerialPort.DataBits = iDataBits
                SerialPort.DtrEnable = bDTREnable
                'SerialPort.Handshake = HandShaking

                'can we create a thread here


            Catch ex As Exception
                Throw New Exception("Unable to Configure Comport" + vbLf + ex.ToString)
            End Try
        End Sub
        ''' <summary>
        ''' Generic Transmit routine.
        ''' </summary>
        ''' <param name="aBytes">Byte array of Data to send</param>
        ''' <remarks></remarks>
        Public Sub v_Transmit(ByRef abytes() As Byte)

            Try

                SerialPort.DiscardInBuffer()
                SerialPort.DiscardOutBuffer()
                SerialPort.Write(abytes, 0, abytes.Length)
                ReDim abytes(RX_BUFFER_SIZE)
                ReceiveBufferControl.MessageState = eMessageState.WaitingForStart
                ReceiveBufferControl.Offset = 0
            Catch ex As Exception
                Throw New Exception(ex.ToString)
            End Try


        End Sub


        ''' <summary>
        ''' Close the comport to the Radian Standard
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
        ''' Open the comport to the Radian
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Sub v_Connect()

            Try
                If SerialPort.IsOpen Then
                    SerialPort.Close()
                End If
                SerialPort.DtrEnable = False
                SerialPort.Handshake = Ports.Handshake.None



                SerialPort.Open()

                ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                Dim iTimeout As Integer = ReceiveBufferControl.Ticks + 500 '200mS
                'purge buffer of junk
                While SerialPort.BytesToRead > 0 And ReceiveBufferControl.Ticks < iTimeout
                    SerialPort.DiscardInBuffer()
                    ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                End While

            Catch
                Throw New Exception("Access to Serial Port Denied")

            End Try

        End Sub

        Public Function b_ReceiveData() Handles SerialPort.DataReceived

            'Dim iBytesToRead As Integer = SerialPort.BytesToRead
            'Dim iOffset As Integer = 0


            Select Case ReceiveBufferControl.MessageState
                Case eMessageState.IDLE
                    'PC is the master if eMessage state is IDLE then no message is expected so flush
                    SerialPort.DiscardInBuffer()
                    ReceiveBufferControl.Offset = 0
                Case Else
                    If aBytes.Length < RX_BUFFER_SIZE Then
                        ReDim Preserve aBytes(RX_BUFFER_SIZE)
                    End If
                    ReceiveBufferControl.Timeout = ReceiveBufferControl.Timeout + 25 'Add 25 mS to time out everytime data received

                    ReceiveBufferControl.BytesToRead = SerialPort.BytesToRead
                    Try
                        SerialPort.Read(aBytes, ReceiveBufferControl.Offset, ReceiveBufferControl.BytesToRead)
                    Catch
                    End Try

                    ReceiveBufferControl.Offset = ReceiveBufferControl.Offset + ReceiveBufferControl.BytesToRead

                    If ReceiveBufferControl.Offset > 0 And (aBytes(0) And &HF) = CByte(&H6) Then ' It is a data packet
                        ReceiveBufferControl.MessageState = eMessageState.WaitingForLength
                    ElseIf ReceiveBufferControl.Offset > 0 And (aBytes(0) And &HF) = CByte(&H3) Then 'Ack with no Data
                        If ReceiveBufferControl.Offset >= 4 Then
                            ReceiveBufferControl.MessageState = eMessageState.IDLE
                            ReDim Preserve aBytes(ReceiveBufferControl.Offset - 1)
                            ReceiveBufferControl.Offset = 0
                        End If
                    ElseIf ReceiveBufferControl.Offset > 0 And (aBytes(0) And &HF) = CByte(&HC) Then 'Ack with Delay
                        ReceiveBufferControl.MessageState = eMessageState.WaitingForLength
                    ElseIf ReceiveBufferControl.Offset > 0 And (aBytes(0) And &HF) = CByte(&H9) Then 'Ack with Error
                        ReceiveBufferControl.MessageState = eMessageState.WaitingForLength
                    End If

                    If ReceiveBufferControl.MessageState = eMessageState.WaitingForLength Then
                        If ReceiveBufferControl.Offset >= 4 Then
                            ReceiveBufferControl.Length = CInt(aBytes(3) Or (CUInt(aBytes(2)) * 256)) + 6
                            ReceiveBufferControl.MessageState = eMessageState.WaitingForData
                        End If
                    End If

                    If ReceiveBufferControl.MessageState = eMessageState.WaitingForData Then
                        If ReceiveBufferControl.Length = ReceiveBufferControl.Offset Then
                            ReceiveBufferControl.MessageState = eMessageState.IDLE
                            ReDim Preserve aBytes(ReceiveBufferControl.Offset - 1)
                            ReceiveBufferControl.Offset = 0
                        End If
                    End If
            End Select

        End Function

        ''' <summary>
        ''' Reads available Buffer as String
        ''' This is timer driven could change to an event driven model
        ''' </summary>
        ''' <returns>Success = true; Fail = false</returns>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Function b_Recieve(ByRef LocalaBytes() As Byte) As Boolean
            'assume success
            b_Recieve = True
            Try

                'set time out
                'Dim iBytesToRead As Integer = SerialPort.BytesToRead
                'Dim iOffset As Integer = 0
                'Dim uiRecieveRetries As UInteger = 3
                Dim bPacketLengthError As Boolean = False
                Dim bReceiveTimeOutError As Boolean = False



                ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                ReceiveBufferControl.Timeout = ReceiveBufferControl.Ticks + 100 '100mS


                While ReceiveBufferControl.Ticks < ReceiveBufferControl.Timeout And ReceiveBufferControl.MessageState <> eMessageState.IDLE
                    Application.DoEvents()
                    ReceiveBufferControl.Ticks = My.Computer.Clock.TickCount
                End While

                If ReceiveBufferControl.Ticks > ReceiveBufferControl.Timeout Then
                    bReceiveTimeOutError = True
                End If

                If aBytes.Length < 4 Then   'minimum message length
                    Throw New Exception("Unable to recieve Message; check and make sure Device is connected to the comport")
                ElseIf bReceiveTimeOutError Then
                    Throw New Exception("Recieve Time Out")
                End If

            Catch ex As Exception
                Throw New Exception(ex.ToString)
            End Try

        End Function
        Class cPacket
            Public Start As Byte 'From RD
            Public PacketType As Byte 'FROM RD
            Public Length As UShort 'From RD
            Public Data() As Byte 'From RD
            Public Checksum As UShort 'From RD
            Public ErrorCode As UShort 'From RD
            Public Delay As UShort 'From RD
            ''' <summary>
            ''' This Value Must be set when read command is issued to the RD
            ''' The Value should be set from MESSAGECONSTANTS.eAccumMetricsIndex or MESSAGECONSTANTS.eInstantMetricsIndex
            ''' </summary>
            ''' <remarks></remarks>
            Public SubCommand As Integer 'Set by Calling routine
            Public ControlByte As sChannelBytes 'Some Radian Commands have a Control Byte Defined; Set by Calling Routine
        End Class

        ''' <summary>
        ''' Packet to Store Parse and store the Communiation packet fro the Radian standard
        ''' Once the mesaged fro mthe RD is parsed into this packet it is used to PARSE any data fro mthe RD.
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Packet As New cPacket

        Public Sub v_InitPacket()
            Packet.Start = 0
            Packet.PacketType = 0
            Packet.Length = 0
            ReDim Packet.Data(255)
            Packet.Checksum = 0
        End Sub

        ''' <summary>
        ''' Class to encapsulate Identifaction Data From RD, once parsed.
        ''' </summary>
        ''' <remarks></remarks>
        Class cRD_Identification
            Public ModelNumber As String
            Public SerialNumber As String
            Public Name As String
            Public VersionNumber As String
        End Class


        ''' <summary>
        ''' Structure Of Byte data to encapsulate three channels
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Structure sChannelBytes
            Public A As Byte
            Public B As Byte
            Public C As Byte
        End Structure
        ''' <summary>
        ''' Structure Of integer data to encapsulate three channels
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Structure sChannelInt
            Public A As Integer
            Public B As Integer
            Public C As Integer
        End Structure

        ''' <summary>
        ''' Structure Of integer data to encapsulate three channels
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Structure sChannelUShort
            Public A As UShort
            Public B As UShort
            Public C As UShort
        End Structure
        Public Identification As New cRD_Identification
        ''' <summary>
        ''' Class to encapsulate Instant and accumlated RD Metric data once parsed and converted from TI Floating Point to IEEE (VB single)
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Class cMetricData


            'Instaneous
            Public Volt As Single
            Public Amp As Single
            Public Watt As Single
            Public VA As Single
            Public VAR As Single
            Public Frequency As Single
            Public Phase As Single
            Public PowerFactor As Single
            Public AnalogSense As Single
            Public DeltaPhase As Single

            'accumulated
            Public WattHr As Single
            Public VARHr As Single
            Public QHr As Single
            Public VAHr As Single
            Public VoltHr As Single
            Public AmpHr As Single
            Public V2Hr As Single
            Public A2Hr As Single
            Public WattHrPlus As Single
            Public WattHrMinus As Single
            Public VarHrPlus As Single
            Public VarHrMinus As Single
        End Class

        ''' <summary>
        ''' Stores Instant and Metric Data as single data type
        ''' </summary>
        ''' <remarks></remarks>
        Public MetricData As New cMetricData


        ' Radian Messages
        ' Remember to remove checksum!
        Public Const NOP As String = "A6000000"
        Public Const IDENTIFY As String = "A6020000"
        Public Const RESET_RD As String = "A6030000"
        Public Const STOP_ACCUM_METRICS As String = "A6090000"
        Public Const TRIGGER_WAVEFORM As String = "A60C000202" '"A60C0001"
        Public Const AQUIRE_WAVEFORM As String = "A60E0006"
        Public Const WAVEFORM_MAX_SAMPLES As Integer = 1600
        Public Const WAVEFORM_MAX_VOLTAGE_SAMPLES As Integer = WAVEFORM_MAX_SAMPLES / 2
        Public Const WAVEFORM_MAX_CURRENT_SAMPLES As Integer = WAVEFORM_MAX_SAMPLES - WAVEFORM_MAX_VOLTAGE_SAMPLES
        Public Const WAVEFORM_CURRENT_OFFSET As Integer = WAVEFORM_MAX_VOLTAGE_SAMPLES
        Public Const WAVEFORM_VOLTAGE_OFFSET As Integer = 0
        Public Const WAVEFORM_BYTES_PER_SAMPLE As Integer = 4
        Public Const MAX_BUFFER_SIZE As Integer = 256
        Public Const SAMPLES_PER_SEC_REV_4_OR_EARLIER As Single = 23788.546
        Public Const SAMPLES_PER_SEC_REV_5_OR_Later As Single = 20119.225
        Public SamplesPerSecond As Single = SAMPLES_PER_SEC_REV_5_OR_Later 'default



        Public Const RESET_METRICS As String = "A6070001"
        Function str_BuildRDMetricResetMessage(ByVal bResetWaveFormBuffers As Boolean, ByVal bResetInstantData As Boolean, ByVal bResetInstantMinData As Boolean, _
                                               ByVal bResetInstantMaxdata As Boolean, ByVal bResetAccumData As Boolean) As String

            str_BuildRDMetricResetMessage = ""
            Dim BResetCode(0) As Byte

            If bResetWaveFormBuffers Then
                BResetCode(0) += CByte(&H1)
            End If

            If bResetInstantData Then
                BResetCode(0) += CByte(&H2)
            End If

            If bResetInstantMinData Then
                BResetCode(0) += CByte(&H4)
            End If

            If bResetInstantMaxdata Then
                BResetCode(0) += CByte(&H8)
            End If

            If bResetAccumData Then
                BResetCode(0) += CByte(&H10)
            End If

            str_BuildRDMetricResetMessage = RESET_METRICS + str_ByteArray_To_Readable_String(BResetCode)
            Dim i As Integer = 1

        End Function

        Public Const strStartAccumMetrics As String = "A6080003"

        Public Enum eStartAccumControlByte
            CONTROL_BYTE_STATUS_REQUEST = &H0
            NORMAL_GATE_START = &H1
            METER_GATE_PULSE_DATA = &H6
            METER_GATE_MANUAL_MODE = &H6
            METER_GATE_SENSOR_MODE = &HE
        End Enum

        ''' <summary>
        ''' This Function builds the Start Accumulating Metrics Message for an RD device
        ''' </summary>
        ''' <param name="Controlbyte">
        ''' Defines Method Of Gating
        ''' 0x00h = Control Byte Status Request
        ''' 0x01h = Normal Gate Start
        ''' 0x06h = Meter Gate: If Pulse Data = 0x00h inquiry to number of remaining pulses in current test.
        '''                     Sets Pulse mode to Manual mode. If Pulse Data > 0x00h then Resets and begins accumlating metrics on the next pulse 
        '''                     recieved and stops accum "PulseData" pulse later
        ''' 0x0Eh = Meter Gate Sensor Mode Same as 0x06h by gated by sensor instead of manual gating.
        ''' </param>
        ''' <param name="PulseData">Used to define the Number of pulses to use when gating</param>
        ''' <returns>String contaiing the RD device command</returns>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Function str_BuildStartAccumMessage(ByVal Controlbyte As Byte, ByVal PulseData As UShort) As String
            Dim Abyte(2) As Byte

            Abyte(0) = Controlbyte
            Abyte(1) = (PulseData >> 8)
            Abyte(2) = PulseData And &HFF
            str_BuildStartAccumMessage = strStartAccumMetrics + str_ByteArray_To_Readable_String(Abyte)

        End Function

        Public Const StartTimedAccumTest As String = "A60A0004"
        ''' <summary>
        ''' Function Calculates the number of samples from Seconds and returns the "Start A Timed Accumulation Test"
        ''' If Time = 0 then it is a Query and will reply with the # of samples remaining to complete the test
        ''' Else the RD Device will immediatly Start a timed test and return an ACK without data.
        ''' </summary>
        ''' <param name="sTimeInSeconds">Time in Seconds to Run the Test</param>
        ''' <returns></returns>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Function str_BuildStartTimedAccumTestMessage(ByVal sTimeInSeconds As Single) As String
            Dim uiNumberOfSamples As UInteger
            'convert seconds to samples
            uiNumberOfSamples = CUInt(sTimeInSeconds * SamplesPerSecond)
            'convert unsigned integer to byte array
            Dim Abyte(3) As Byte
            Abyte(0) = (uiNumberOfSamples >> 24) And &HFF
            Abyte(1) = (uiNumberOfSamples >> 16) And &HFF
            Abyte(2) = (uiNumberOfSamples >> 8) And &HFF
            Abyte(3) = uiNumberOfSamples And &HFF
            str_BuildStartTimedAccumTestMessage = StartTimedAccumTest + str_ByteArray_To_Readable_String(Abyte)

        End Function


        'Public Enum eInstantMetricsIndex3X
        '    ALL_RD3X_N 'All RD3X plus Neutral Current
        '    All_RD3x
        '    Neutral_Current
        '    Delta_Volts
        '    Delta_Watts
        '    Delta_VA
        'End Enum





        ''Instantaneous Metrics RD2X Format Compatable with RD3X and Master Slave RD2Xs (Like in WECO Racks)
        'Public Structure READ_INSTANT_METRICS_RD2X_FORMAT_RD3X
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3x_ALLN As String = "A60D0008004400000008FFFD" 'includes neutral current
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3x_ALL As String = "A60D0008004000000014FFFD"  'excludes Neutral Current
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_NEUTRAL_CURRENT_CALC As String = "A60D0008000400000008FFFD"
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_DELTA_VOLTS As String = "A60D000800040000003CFFFD"
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_DELTA_WATTS As String = "A60D0008000400000040FFFD"
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_DELTA_VA As String = "A60D0008000400000044FFFD"
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_DELTA_VAR As String = "A60D0008000400000048FFFD"
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_xdVAR As String = "A60D000800040000004CFFFD" 'Cross Connected Delta Var
        '    Public Const READ_INSTANTANEOUS_METRICS_RD3X_xyVAR As String = "A60D0008000400000050FFFD" 'Cross Connected Wye Var
        'End Structure




        'Public READ_INSTANT_METRICS_COMMANDS As READ_INSTANT_METRICS_RD2X_FORMAT_RD2X
        'Public READ_ACCUM_METRICS_COMMANDS As READ_ACCUM_METRICS_RD2X_FORMAT
        ''' <summary>
        ''' Radian Device Command Access (DCA) Structure. Start Byte for all messages to and From Radian Device
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Structure DCA
            Public Const MASTER_ACK_WITH_NO_DATA As String = "A3"
            Public Const MASTER_ACK_WITH_DATA As String = "A6"
            Public Const MASTER_ACK_WITH_DELAY As String = "AC"
            Public Const MASTER_ACK_WITH_ERROR As String = "A9"

            Public Const A_ACK_WITH_NO_DATA As String = "B3"
            Public Const A_ACK_WITH_DATA As String = "B6"
            Public Const A_ACK_WITH_DELAY As String = "BC"
            Public Const A_ACK_WITH_ERROR As String = "B9"

            Public Const B_ACK_WITH_NO_DATA As String = "C3"
            Public Const B_ACK_WITH_DATA As String = "C6"
            Public Const B_ACK_WITH_DELAY As String = "CC"
            Public Const B_ACK_WITH_ERROR As String = "C9"

            Public Const C_ACK_WITH_NO_DATA As String = "D3"
            Public Const C_ACK_WITH_DATA As String = "D6"
            Public Const C_ACK_WITH_DELAY As String = "DC"
            Public Const C_ACK_WITH_ERROR As String = "D9"
        End Structure



        ''' <summary>
        '''  Enumeration of the Instantaneous Metrics.  Changing the order here changes the order they are stores in "aInstantMetricsNamesList"
        ''' This serves as the index into the array list after the arraylist is initalized.
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Enum eInstantMetricsIndex2X
            ALL_RD2X
            Volts
            Amps
            Watts
            VA
            Frequency
            Degrees
            VAR
            PF
            Analog_Sense
            Delta_Phase
        End Enum

        ''' <summary>
        ''' Array List to store Accum Metrics Unit Labels.They are stored at the index pointed to by the enumeration  "eInstantMetricsIndex2X"  
        ''' These should not be moved arround Dynamicly once initialized
        ''' Not all table definations have been defined.  
        ''' See Radian Customer Access Commands Manual 944011.A 
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public aInstantMetricsUnitLabelList As New ArrayList

        ''' <summary>
        ''' This function must be called to store the UNIT labels for Accumulated metrics into "aAccumMetricsNamesList" array list. 
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Sub v_InitInstantMetricsArrayListNamesRD2X()

            For Each i As Integer In [Enum].GetValues(GetType(eInstantMetricsIndex2X))
                aInstantMetricsUnitLabelList.Add("")
            Next

            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.ALL_RD2X) = "All RD2X METRICS"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Volts) = "VOLTS"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Amps) = "AMPS"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Watts) = "WATTS"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.VA) = "VA"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Frequency) = "FREQUENCY"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Degrees) = "Degrees"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.VAR) = "VAR"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.PF) = "PF"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Analog_Sense) = "ANALOG SENSE"
            aInstantMetricsUnitLabelList(eInstantMetricsIndex2X.Delta_Phase) = "Delta Phase"
        End Sub

        ''' <summary>
        '''  'Instantaneous Metrics RD2X Format for single RD2X (All compatible with RD3X)
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Structure READ_INSTANT_METRICS_RD2X_FORMAT_RD2X
            'All RD Standards
            Public Const All As String = "A60D0008002400000014FFFD"  'This reads all RD2X metrics which is a subset of the RD3X.
            Public Const VOLTS As String = "A60D0008000400000014FFFD"
            Public Const AMPS As String = "A60D0008000400000018FFFD"
            Public Const WATT As String = "A60D000800040000001CFFFD"
            Public Const AMP As String = "A60D0008000400000020FFFD"
            Public Const VAR As String = "A60D0008000400000024FFFD"
            Public Const FREQ As String = "A60D0008000400000028FFFD"
            Public Const PHASE As String = "A60D000800040000002CFFFD"
            Public Const PF As String = "A60D0008000400000030FFFD"
            Public Const ANALOG_SENSE As String = "A60D0008000400000034FFFD"
            Public Const DELTA_PHASE As String = "A60D0008000400000038FFFD"
        End Structure
        ''' <summary>
        '''  Array list to store the Commands used to access Instantaneous Metrics when communicating with the Radian RD2X Standard.   
        '''  They are stored at the index pointed to by the enumeration  "eAccumMetricsIndex"
        '''  These should not be moved arround Dynamicly once initialized
        '''  Not all table definations have been defined.  
        '''  See Radian Customer Access Commands Manual 944011.A
        ''' </summary>
        ''' <remarks></remarks>
        Public aInstantMetricsReadCommandsListRD2X As New ArrayList
        ''' <summary>
        ''' This function must be called to store the UNIT labels for Accumulated metrics into "aInstantMetricsReadCommandsListRD2X" array list. 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub v_InitalizeInstantMetricsReadCommandsList()
            For Each i As Integer In [Enum].GetValues(GetType(eInstantMetricsIndex2X))
                aInstantMetricsReadCommandsListRD2X.Add("")
            Next
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Volts) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.VOLTS
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Amps) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.AMPS
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Watts) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.WATT
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.VA) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.AMP
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Frequency) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.FREQ
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Degrees) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.PHASE
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.VAR) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.VAR
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.PF) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.PF
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.ALL_RD2X) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.All
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Analog_Sense) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.ANALOG_SENSE
            aInstantMetricsReadCommandsListRD2X(eInstantMetricsIndex2X.Delta_Phase) = READ_INSTANT_METRICS_RD2X_FORMAT_RD2X.DELTA_PHASE
        End Sub


        ''' <summary>
        ''' Enumeration of the Accumulated Metrics.  Changing the order here changes the order they are stores in "aAccumMetricsNamesList"
        ''' This is the index into the array list after the array list is initalized.
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Enum eAccumMetricsIndex
            ALL_RD2X
            Wh
            VARh
            Qh
            VAh
            Vh
            Ah
            V2h
            A2h
            Wh_Plus
            Wh_Minus
            VARh_Plus
            VARh_Minus
        End Enum
        ''' <summary>
        ''' Array List to store Accum Metrics Unit Labels.They are stored at the index pointed to by the enumeration  "eAccumMetricsIndex"  
        ''' These should not be moved arround Dynamicly once initialized
        ''' Not all table definations have been defined.  
        ''' See Radian Customer Access Commands Manual 944011.A
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public aAccumMetricsUnitLabelList As New ArrayList
        ''' <summary>
        ''' This function must be called to store the UNIT labels for Accumulated metrics into "aAccumMetricsNamesList" array list. 
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Sub v_InitAccumMetricsArrayListNamesRD2X()
            For Each i As Integer In [Enum].GetValues(GetType(eAccumMetricsIndex))
                aAccumMetricsUnitLabelList.Add("")
            Next
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.Wh) = "WATT-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.VARh) = "VAR-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.Qh) = "Q-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.VAh) = "VOLT-AMPERE-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.Vh) = "VOLT-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.Ah) = "AMP-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.V2h) = "VOLT-SQUARED-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.A2h) = "AMP-SQUARED-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.A2h) = "AMP-SQUARED-HOURS"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.Wh_Plus) = "WATT-HOUR+"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.Wh_Minus) = "WATT-HOUR-"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.VARh_Plus) = "VAR-HOUR+"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.VARh_Minus) = "VAR-HOUR-"
            aAccumMetricsUnitLabelList(eAccumMetricsIndex.ALL_RD2X) = "All METRICS"

        End Sub

        ''' <summary>
        '''  'Read Commands for Accumulated Metrics RD 2X Format for Single RD2X (All compatible with RD3X)
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 </remarks>
        Public Structure READ_ACCUM_METRICS_RD2X_FORMAT
            Public Const All As String = "A6160006003000000004"  'This reads all RD2X metrics which is a subset of the RD3X.
            Public Const WH As String = "A6160006000400000004"   'WATT HOUR
            Public Const VARH As String = "A6160006000400000008"  'VAR HOUR
            Public Const QH As String = "A616000600040000000C"  'Q HOUR
            Public Const VAH As String = "A6160006000400000010" 'VA HOUR
            Public Const VH As String = "A6160006000400000014" 'VOLT HOUR
            Public Const AH As String = "A6160006000400000018" 'AMP HOUR
            Public Const V2H As String = "A616000600040000001C" 'VOLT SQUARED HOUR
            Public Const A2H As String = "A6160006000400000020" 'AMP SQUARED HOUR
            Public Const WH_PLUS As String = "A6160006000400000024" 'WATT HOUR+
            Public Const WH_MINUS As String = "A6160006000400000028" 'WATT HOUR-
            Public Const VARH_PLUS As String = "A616000600040000002C" 'VAR HOUR+
            Public Const VARH_MINUS As String = "A6160006000400000030" 'VAR HOUR-

        End Structure
        ''' <summary>
        '''  Array list to store the Commands used to access Accumulated Metrics when communicating with the Radian RD2X Standard.   
        '''  They are stored at the index pointed to by the enumeration  "eAccumMetricsIndex"
        '''  These should not be moved arround Dynamicly once initialized
        '''  Not all table definations have been defined.  
        '''  See Radian Customer Access Commands Manual 944011.A
        ''' </summary>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public aAccumMetricsReadCommandList As New ArrayList
        Public Sub v_InitAccumMetricsReadCommandListRD2X()
            For Each i As Integer In [Enum].GetValues(GetType(eAccumMetricsIndex))
                aAccumMetricsReadCommandList.Add("")
            Next
            aAccumMetricsReadCommandList(eAccumMetricsIndex.Wh) = READ_ACCUM_METRICS_RD2X_FORMAT.WH
            aAccumMetricsReadCommandList(eAccumMetricsIndex.VARh) = READ_ACCUM_METRICS_RD2X_FORMAT.VARH
            aAccumMetricsReadCommandList(eAccumMetricsIndex.Qh) = READ_ACCUM_METRICS_RD2X_FORMAT.QH
            aAccumMetricsReadCommandList(eAccumMetricsIndex.VAh) = READ_ACCUM_METRICS_RD2X_FORMAT.VAH
            aAccumMetricsReadCommandList(eAccumMetricsIndex.Vh) = READ_ACCUM_METRICS_RD2X_FORMAT.VH
            aAccumMetricsReadCommandList(eAccumMetricsIndex.Ah) = READ_ACCUM_METRICS_RD2X_FORMAT.AH
            aAccumMetricsReadCommandList(eAccumMetricsIndex.V2h) = READ_ACCUM_METRICS_RD2X_FORMAT.V2H
            aAccumMetricsReadCommandList(eAccumMetricsIndex.A2h) = READ_ACCUM_METRICS_RD2X_FORMAT.A2H
            aAccumMetricsReadCommandList(eAccumMetricsIndex.Wh_Plus) = READ_ACCUM_METRICS_RD2X_FORMAT.WH_PLUS
            aAccumMetricsReadCommandList(eAccumMetricsIndex.Wh_Minus) = READ_ACCUM_METRICS_RD2X_FORMAT.WH_MINUS
            aAccumMetricsReadCommandList(eAccumMetricsIndex.VARh_Plus) = READ_ACCUM_METRICS_RD2X_FORMAT.VARH_PLUS
            aAccumMetricsReadCommandList(eAccumMetricsIndex.VARh_Minus) = READ_ACCUM_METRICS_RD2X_FORMAT.VARH_MINUS
            aAccumMetricsReadCommandList(eAccumMetricsIndex.ALL_RD2X) = READ_ACCUM_METRICS_RD2X_FORMAT.All
        End Sub

        'Public Class ErrorCodes
        '    Public LINE_ERROR As String = "01"
        '    Public CHECKSUM_ERROR As String = "02"
        '    Public TIMEOUT_ERROR As String = "03"
        '    Public INVALID_START_BYTE_ERROR As String = "04"
        '    Public LENGTH_ERROR As String = "05"
        '    Public INVALID_PACKET_TYPE As String = "06"
        '    Public INVALID_START_ADDRESS As String = "07"
        '    Public INVALID_RANGE As String = "08"
        '    Public INVALID_DATA_LENGTH As String = "09"
        '    Public INVALID_BOUNDARY_BYTE_ERROR As String = "0A"
        '    Public INVALID_DEVICE_ERROR As String = "0B"
        '    Public DEVICE_ERROR As String = "0C"
        '    Public RECEIVE_REGISTER_ERROR As String = "0D"
        '    Public INVALID_START_DELIMITER As String = "0E"
        '    Public INVALID_DATA_ERROR As String = "0F"
        '    Public INTERNAL_COMM_ERROR As String = "10"
        'End Class
        Public Enum eACKCodes
            MASTER_ACK_WITH_NO_DATA = &HA3
            MASTER_ACK_WITH_DATA = &HA6
            MASTER_ACK_WITH_DELAY = &HAC
            MASTER_ACK_WITH_ERROR = &HA9

            A_ACK_WITH_NO_DATA = &HB3
            A_ACK_WITH_DATA = &HB6
            A_ACK_WITH_DELAY = &HBC
            A_ACK_WITH_ERROR = &HB9

            B_ACK_WITH_NO_DATA = &HC3
            B_ACK_WITH_DATA = &HC6
            B_ACK_WITH_DELAY = &HCC
            B_ACK_WITH_ERROR = &HC9

            C_ACK_WITH_NO_DATA = &HD3
            C_ACK_WITH_DATA = &HD6
            C_ACK_WITH_DELAY = &HDC
            C_ACK_WITH_ERROR = &HD9
        End Enum

        Public Enum eErrorCodes
            NO_ERROR = 0 'Not part of Radian; added to match size of array
            LINE_ERROR = &H1
            CHECKSUM_ERROR = &H2
            TIMEOUT_ERROR = &H3
            INVALID_START_BYTE_ERROR = &H4
            LENGTH_ERROR = &H5
            INVALID_PACKET_TYPE = &H6
            INVALID_START_ADDRESS = &H7
            INVALID_RANGE = &H8
            INVALID_DATA_LENGTH = &H9
            INVALID_BOUNDARY_BYTE_ERROR = &HA
            INVALID_DEVICE_ERROR = &HB
            DEVICE_ERROR = &HC
            RECEIVE_REGISTER_ERROR = &HD
            INVALID_START_DELIMITER = &HE
            INVALID_DATA_ERROR = &HF
            INTERNAL_COMM_ERROR = &H10
        End Enum
        ''' <summary>
        ''' List to hold Radian error codes in enumeration order.  Iniialized at start up
        ''' Read only.
        ''' </summary>
        ''' <remarks></remarks>
        Public aRadianErrors As New ArrayList
        Public Sub v_InitrRadianErrorCodes()
            For Each i As Integer In [Enum].GetValues(GetType(eErrorCodes))
                aRadianErrors.Add("")
            Next
            aRadianErrors(eErrorCodes.LINE_ERROR) = "LINE ERROR"
            aRadianErrors(eErrorCodes.CHECKSUM_ERROR) = "CHECKSUM ERROR"
            aRadianErrors(eErrorCodes.TIMEOUT_ERROR) = "TIMEOUT ERROR"
            aRadianErrors(eErrorCodes.INVALID_START_BYTE_ERROR) = "INVALID START BYTE ERROR"
            aRadianErrors(eErrorCodes.LENGTH_ERROR) = "LENGTH ERROR"
            aRadianErrors(eErrorCodes.INVALID_PACKET_TYPE) = "INVALID PACKET TYPE"
            aRadianErrors(eErrorCodes.INVALID_START_ADDRESS) = "INVALID START ADDRESS"
            aRadianErrors(eErrorCodes.INVALID_RANGE) = "INVALID RANGE"
            aRadianErrors(eErrorCodes.INVALID_DATA_LENGTH) = "INVALID DATA LENGTH"
            aRadianErrors(eErrorCodes.INVALID_BOUNDARY_BYTE_ERROR) = "INVALID BOUNDARY BYTE ERROR"
            aRadianErrors(eErrorCodes.INVALID_DEVICE_ERROR) = "INVALID DEVICE ERROR"
            aRadianErrors(eErrorCodes.DEVICE_ERROR) = "DEVICE ERROR"
            aRadianErrors(eErrorCodes.RECEIVE_REGISTER_ERROR) = "RECEIVE REGISTER  ERROR"
            aRadianErrors(eErrorCodes.INVALID_START_DELIMITER) = "INVALID START DELIMITER"
            aRadianErrors(eErrorCodes.INVALID_DATA_ERROR) = "INVALID DATA ERROR"
            aRadianErrors(eErrorCodes.INTERNAL_COMM_ERROR) = "INTERNAL COMM ERROR"
        End Sub

#Region "RD_Relay"

        Public Const LOCK_RELAY_RANGE As String = "A60B0003"

        Public Class RelayArrayGroup
            Public Rangename As New ArrayList
            Public Rangeaddress As New ArrayList
        End Class


        Public Enum eRelayControlCommand
            VOLTAGE_STATUS = &H0
            CURRENT_STATUS = &H0
            UNLOCK_VOLTAGE = &H4
            LOCK_VOLTAGE = &H6
            SET_LOCK_VOLTAGE = &H7
            UNLOCK_CURRENT = &H40
            LOCK_CURRENT = &H60
            SET_LOCK_CURRENT = &H70
            ACTIVATE_CLAMP_DISABLE_RELAY = &H8
            ACTIVATE_CLAMP_ENABLE_RELAY = &H80
            ACTIVATE_CLAMP_ENABLE_AND_DISABLE_RELAY = &H88
        End Enum
        Public Enum eRelayControlByteResponse
            UNLOCKED = &H0
            VOLTAGE_RELAY_LOCKED = &H2
            CURRENT_RELAY_LOCKED = &H20
            BOTH_LOCKED = &H22

        End Enum

        Public Class cRelayStatus
            Public VoltageRelayLocked As Boolean
            Public CurrentRelayLocked As Boolean
            Public VoltageRange As eVoltageTapsIndex
            Public CurrentRange As eCurrentTapsIndex
            Public VoltageRangeName As String
            Public CurrentRangeName As String
            Public VoltageRelayBits As eVoltageTapAddressesRD
            Public CurrentRelayBits As eCurrentTapsAddressRD
        End Class
        Public RelayStatus As New cRelayStatus
        Public Enum eCurrentTapsIndex
            ALL_OPEN  ' '120A, 200A'            '225A'
            RANGE_0   '   MIN TO 0.0328       Min TO 0.0349
            RANGE_1   '0.0320 TO 0.0656    0.0340 TO 0.0679 
            RANGE_2   '0.0640 TO 0.1312    0.0680 TO 0.1394
            RANGE_3   '0.1280 TO 0.2624    0.1360 TO 0.2788 
            RANGE_4   '0.2560 TO 0.5248    0.2720 TO 0.5576
            RANGE_5   '0.5120 TO 1.0496    0.5440 TO 1.1152
            RANGE_6   '1.0240 TO 2.0992    1.0880 TO 2.2304
            RANGE_7   '2.0480 TO 4.1984    2.1760 TO 4.4608
            RANGE_8   '4.0960 TO 8.3968    4.3520 TO 8.9216 
            RANGE_9   '8.1920 TO 16.7936   8.7040 TO 17.8432 
            RANGE_10  '16.384 TO 33.5872   17.408 TO 35.6864
            RANGE_11  '32.768 TO 67.1744   34.816 TO 71.3728
            RANGE_12  '65.536 TO 200.000   69.632 TO 225.00
        End Enum

        Public Enum eCurrentTapsAddressRD
            ALL_OPEN = &H0    ' '120A, 200A'            '225A'
            RANGE_0 = &H808   '   MIN TO 0.0328       Min TO 0.0349
            RANGE_1 = &H4008  '0.0320 TO 0.0656    0.0340 TO 0.0679 
            RANGE_2 = &H1008  '0.0640 TO 0.1312    0.0680 TO 0.1394
            RANGE_3 = &H8008  '0.1280 TO 0.2624    0.1360 TO 0.2788 
            RANGE_4 = &H2008  '0.2560 TO 0.5248    0.2720 TO 0.5576
            RANGE_5 = &H8010  '0.5120 TO 1.0496    0.5440 TO 1.1152
            RANGE_6 = &H2010  '1.0240 TO 2.0992    1.0880 TO 2.2304
            RANGE_7 = &H8020  '2.0480 TO 4.1984    2.1760 TO 4.4608
            RANGE_8 = &H2020  '4.0960 TO 8.3968    4.3520 TO 8.9216 
            RANGE_9 = &H8040  '8.1920 TO 16.7936   8.7040 TO 17.8432 
            RANGE_10 = &H2040 '16.384 TO 33.5872   17.408 TO 35.6864
            RANGE_11 = &H8080 '32.768 TO 67.1744   34.816 TO 71.3728
            RANGE_12 = &H2080 '65.536 TO 200.000   69.632 TO 225.00

        End Enum


        Public Structure sCurrentRangeName
            Public Const ALL_OPEN = "All Open"
            Public Const RANGE_0 = "MIN TO 0.0328"
            Public Const RANGE_1 = "0.0320 TO 0.0656"
            Public Const RANGE_2 = "0.0640 TO 0.1312"
            Public Const RANGE_3 = "0.1280 TO 0.2624"
            Public Const RANGE_4 = "0.2560 TO 0.5248"
            Public Const RANGE_5 = "0.5120 TO 1.0496"
            Public Const RANGE_6 = "1.0240 TO 2.0992"
            Public Const RANGE_7 = "2.0480 TO 4.1984"
            Public Const RANGE_8 = "4.0960 TO 8.3968"
            Public Const RANGE_9 = "8.1920 TO 16.7936"
            Public Const RANGE_10 = "16.384 TO 33.5872"
            Public Const RANGE_11 = "32.768 TO 67.1744"
            Public Const RANGE_12 = "65.536 TO 200.000"
        End Structure

        Public Structure sCurrentRangeName225
            Public Const ALL_OPEN = "All Open"
            Public Const RANGE_0 = "Min TO 0.0349"
            Public Const RANGE_1 = "0.0340 TO 0.0679 "
            Public Const RANGE_2 = "0.0680 TO 0.1394"
            Public Const RANGE_3 = "0.1360 TO 0.2788"
            Public Const RANGE_4 = "0.2720 TO 0.5576"
            Public Const RANGE_5 = "0.5440 TO 1.1152"
            Public Const RANGE_6 = "1.0880 TO 2.2304"
            Public Const RANGE_7 = "2.1760 TO 4.4608"
            Public Const RANGE_8 = "4.3520 TO 8.9216"
            Public Const RANGE_9 = "8.7040 TO 17.8432"
            Public Const RANGE_10 = "17.408 TO 35.6864"
            Public Const RANGE_11 = "334.816 TO 71.3728"
            Public Const RANGE_12 = "69.632 TO 225.00"
        End Structure

        Public aCurrentRelay As New RelayArrayGroup
        Public Sub v_initCurrentList(ByVal bIs225MaxAmp As Boolean)
            For Each i As Integer In [Enum].GetValues(GetType(eCurrentTapsAddressRD))
                aCurrentRelay.Rangename.Add("")
                aCurrentRelay.Rangeaddress.Add("")
            Next
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.ALL_OPEN) = eCurrentTapsAddressRD.ALL_OPEN
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_0) = eCurrentTapsAddressRD.RANGE_0
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_1) = eCurrentTapsAddressRD.RANGE_1
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_2) = eCurrentTapsAddressRD.RANGE_2
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_3) = eCurrentTapsAddressRD.RANGE_3
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_4) = eCurrentTapsAddressRD.RANGE_4
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_5) = eCurrentTapsAddressRD.RANGE_5
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_6) = eCurrentTapsAddressRD.RANGE_6
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_7) = eCurrentTapsAddressRD.RANGE_7
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_8) = eCurrentTapsAddressRD.RANGE_8
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_9) = eCurrentTapsAddressRD.RANGE_9
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_10) = eCurrentTapsAddressRD.RANGE_10
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_11) = eCurrentTapsAddressRD.RANGE_11
            aCurrentRelay.Rangeaddress(eCurrentTapsIndex.RANGE_12) = eCurrentTapsAddressRD.RANGE_12

            If bIs225MaxAmp Then 'Is it a 225 amp RD Standard?
                aCurrentRelay.Rangename(eCurrentTapsIndex.ALL_OPEN) = sCurrentRangeName225.ALL_OPEN
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_0) = sCurrentRangeName225.RANGE_0
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_1) = sCurrentRangeName225.RANGE_1
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_2) = sCurrentRangeName225.RANGE_2
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_3) = sCurrentRangeName225.RANGE_3
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_4) = sCurrentRangeName225.RANGE_4
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_5) = sCurrentRangeName225.RANGE_5
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_6) = sCurrentRangeName225.RANGE_6
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_7) = sCurrentRangeName225.RANGE_7
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_8) = sCurrentRangeName225.RANGE_8
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_9) = sCurrentRangeName225.RANGE_9
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_10) = sCurrentRangeName225.RANGE_10
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_11) = sCurrentRangeName225.RANGE_11
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_12) = sCurrentRangeName225.RANGE_12
            Else
                aCurrentRelay.Rangename(eCurrentTapsIndex.ALL_OPEN) = sCurrentRangeName.ALL_OPEN
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_0) = sCurrentRangeName.RANGE_0
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_1) = sCurrentRangeName.RANGE_1
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_2) = sCurrentRangeName.RANGE_2
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_3) = sCurrentRangeName.RANGE_3
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_4) = sCurrentRangeName.RANGE_4
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_5) = sCurrentRangeName.RANGE_5
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_6) = sCurrentRangeName.RANGE_6
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_7) = sCurrentRangeName.RANGE_7
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_8) = sCurrentRangeName.RANGE_8
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_9) = sCurrentRangeName.RANGE_9
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_10) = sCurrentRangeName.RANGE_10
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_11) = sCurrentRangeName.RANGE_11
                aCurrentRelay.Rangename(eCurrentTapsIndex.RANGE_12) = sCurrentRangeName.RANGE_12

            End If
        End Sub

        Public Enum eVoltageTapsIndex
            ALL_OPEN
            RANGE_120V
            RANGE_240V
            RANGE_480
        End Enum
        Public Enum eVoltageTapAddressesRD
            ALL_OPEN = &H0
            RANGE_120V = &H1
            RANGE_240V = &H2
            RANGE_480V = &H4
        End Enum

        Public Structure sVoltageRangeName
            Public Const ALL_OPEN = " All Open"
            Public Const RANGE_120V = "120V"
            Public Const RANGE_240V = "240V"
            Public Const RANGE_480V = "480V"
        End Structure
        Public aVoltageRelay As New RelayArrayGroup

        Public Sub v_initVoltageList()
            ' aVoltageRelay.Rangename = New ArrayList
            'aVoltageRelay.Rangeaddress = New ArrayList
            For Each i As Integer In [Enum].GetValues(GetType(eVoltageTapAddressesRD))
                aVoltageRelay.Rangename.Add("")
                aVoltageRelay.Rangeaddress.Add("")
            Next
            aVoltageRelay.Rangeaddress(eVoltageTapsIndex.ALL_OPEN) = eVoltageTapAddressesRD.ALL_OPEN
            aVoltageRelay.Rangeaddress(eVoltageTapsIndex.RANGE_120V) = eVoltageTapAddressesRD.RANGE_120V
            aVoltageRelay.Rangeaddress(eVoltageTapsIndex.RANGE_240V) = eVoltageTapAddressesRD.RANGE_240V
            aVoltageRelay.Rangeaddress(eVoltageTapsIndex.RANGE_480) = eVoltageTapAddressesRD.RANGE_480V
            aVoltageRelay.Rangename(eVoltageTapsIndex.ALL_OPEN) = sVoltageRangeName.ALL_OPEN

            aVoltageRelay.Rangename(eVoltageTapsIndex.RANGE_120V) = sVoltageRangeName.RANGE_120V
            aVoltageRelay.Rangename(eVoltageTapsIndex.RANGE_240V) = sVoltageRangeName.RANGE_240V
            aVoltageRelay.Rangename(eVoltageTapsIndex.RANGE_480) = sVoltageRangeName.RANGE_480V
        End Sub

        Public Function str_BuildLockRangeMessage(ByVal ControlByteVoltage As Byte, ByVal ControlByteCurrent As Byte, ByVal ControlByteClampOn As Byte, _
                                                  ByVal RelayBitsVoltage As UShort, ByVal RelaybitsCurrent As UShort) As String
            Dim abyte(2) As Byte
            Dim BControlByte As Byte
            Dim ui16RelayBits As UShort

            'init
            str_BuildLockRangeMessage = ""
            'or toget
            BControlByte = ControlByteVoltage Or ControlByteCurrent Or ControlByteClampOn
            ui16RelayBits = RelayBitsVoltage Or RelaybitsCurrent

            'Move to bytearray
            abyte(0) = BControlByte
            abyte(1) = CByte((ui16RelayBits >> 8) And &HFF)
            abyte(2) = CByte(ui16RelayBits And &HFF)
            str_BuildLockRangeMessage = LOCK_RELAY_RANGE + str_ByteArray_To_Readable_String(abyte)

        End Function


#End Region '"RD_Relay"


#Region "Waveform"
        Public Enum eWaveformControlWord
            STATUS_REQUEST = 0
            STOP__CAPTURE = 1
            START_CAPTURE = 2
            START_SIMULATION_MODE = 3
            EXIT_SIMULATION_MODE = 4
        End Enum

        Public Enum eWaveFormStatusReplies
            BUFFER_NOT_ACTIVE = 0
            BUFFER_STOPPED_DATA_AVAILABLE = 1
            BUFFER_RUNNING_DATA_ACCUMULATING = 2
            IN_SUMULATION_MODE
            SIM_MODE_BUFFER_STOPPED
            SIM_MODE_BUFFER_RUNNING
        End Enum

        Public Structure sStatusReplies
            Public Const BUFFER_NOT_ACTIVE As String = "Waveform buffer not active."
            Public Const BUFFER_STOPPED_DATA_AVAILABLE As String = "Waveform buffer stopped; data available."
            Public Const BUFFER_RUNNING_DATA_ACCUMULATING As String = "Waveform buffer running; accumulating data."
            Public Const IN_SIME_MODE As String = "In Simulation Mode"
            Public Const SIM_MODE_BUFFER_STOPPED = "In Simulation Mode Buffer Stopped"
            Public Const SIM_MODE_BUFFER_RUNNING = "In Simulation Mode Buffer Running"
        End Structure

        Public aTriggerWaveFormStatusMessages As New ArrayList
        Public Sub v_initTriggerWaveFormStatusMessages()
            For Each i As Integer In [Enum].GetValues(GetType(eWaveFormStatusReplies))
                aTriggerWaveFormStatusMessages.Add("")

            Next
            aTriggerWaveFormStatusMessages(eWaveFormStatusReplies.BUFFER_NOT_ACTIVE) = sStatusReplies.BUFFER_NOT_ACTIVE
            aTriggerWaveFormStatusMessages(eWaveFormStatusReplies.BUFFER_RUNNING_DATA_ACCUMULATING) = sStatusReplies.BUFFER_RUNNING_DATA_ACCUMULATING
            aTriggerWaveFormStatusMessages(eWaveFormStatusReplies.BUFFER_STOPPED_DATA_AVAILABLE) = sStatusReplies.BUFFER_STOPPED_DATA_AVAILABLE
            aTriggerWaveFormStatusMessages(eWaveFormStatusReplies.SIM_MODE_BUFFER_STOPPED) = sStatusReplies.SIM_MODE_BUFFER_STOPPED
            aTriggerWaveFormStatusMessages(eWaveFormStatusReplies.SIM_MODE_BUFFER_RUNNING) = sStatusReplies.SIM_MODE_BUFFER_RUNNING
        End Sub

        Public Class cWaveform
            Public Status As New eWaveFormStatusReplies
            Public Command As New eWaveformControlWord
            Public InSimMode As Boolean 'True if waveform Capture in Simulation Mode
            Public CurrentData() As Single
            Public VoltageData() As Single
            Public VoltageAndCurrentData() As Single
            Public Pointer As UInteger 'Pointer into VoltageAndCurrentData array
            Public EndOfData As UShort    'Pointer at last DataPoint
            Public StartOfData As UInteger 'Pointer at first datapoint
            Public Sub v_Init()
                ReDim CurrentData(WAVEFORM_MAX_CURRENT_SAMPLES - 1)
                ReDim VoltageData(WAVEFORM_MAX_VOLTAGE_SAMPLES - 1)
                ReDim VoltageAndCurrentData(WAVEFORM_MAX_SAMPLES - 1)
                Pointer = 0
                InSimMode = False
            End Sub
        End Class

        Public WaveForm As New cWaveform

        Public Function str_BuildTriggerWaveformMessage(ByVal ControlByte As Integer) As String
            str_BuildTriggerWaveformMessage = "" 'init
            Dim abyte(0) As Byte
            abyte(0) = CByte(ControlByte)
            str_BuildTriggerWaveformMessage = TRIGGER_WAVEFORM + str_ByteArray_To_Readable_String(abyte)
        End Function

        Public Function str_BuildAquireWaveformMessage(ByRef NumSamples As UShort, ByRef Offset As UInteger)

            'call this funciton until the Number of Samples (NumSamples)  = zero
            'Byte array to hold buffer size and byte offset into DSP memory
            Dim abyte(5) As Byte
            'Number of Bytes to Aquire = samples * 4
            Dim ui16BytesToAquire As UShort
            'init Wave from message
            str_BuildAquireWaveformMessage = ""

            'Determine number of bytes to aquire
            ui16BytesToAquire = NumSamples * CUShort(WAVEFORM_BYTES_PER_SAMPLE)

            'Maximum size is 256 bytes
            If ui16BytesToAquire > CUShort(MAX_BUFFER_SIZE) Then  'aquire max buffer size
                abyte(0) = CByte((MAX_BUFFER_SIZE >> 8) And &HFF)
                abyte(1) = CByte(MAX_BUFFER_SIZE And &HFF)
                abyte(2) = CByte((Offset >> 24) And &HFF)
                abyte(3) = CByte((Offset >> 16) And &HFF)
                abyte(4) = CByte((Offset >> 8) And &HFF)
                abyte(5) = CByte(Offset And &HFF)
                NumSamples = NumSamples - CUShort(MAX_BUFFER_SIZE / WAVEFORM_BYTES_PER_SAMPLE) 'update number of smaples to aquire
                Offset = Offset + MAX_BUFFER_SIZE '/ WAVEFORM_BYTES_PER_SAMPLE 'update offset location in Waveform buffer
            Else 'bytes to aquire is less than or equal to Maximum buffer size (256)
                abyte(0) = CByte((ui16BytesToAquire >> 8) And &HFF)
                abyte(1) = CByte(ui16BytesToAquire And &HFF)
                abyte(2) = CByte((Offset >> 24) And &HFF)
                abyte(3) = CByte((Offset >> 16) And &HFF)
                abyte(4) = CByte((Offset >> 8) And &HFF)
                abyte(5) = CByte(Offset And &HFF)
                NumSamples = NumSamples - CUShort(ui16BytesToAquire / WAVEFORM_BYTES_PER_SAMPLE) 'update number of smaples to aquire should be zero
                Offset = Offset + ui16BytesToAquire 'update offset location in Waveform buffer
            End If
            'return the aquire message as a string
            str_BuildAquireWaveformMessage = AQUIRE_WAVEFORM + str_ByteArray_To_Readable_String(abyte)

        End Function

#End Region '"Waveform"



        Public Enum ePacketTypes
            NOP = &H0
            IDENT = &H2
            RESET_RD_XX = &H3
            RESET_METRICS = &H7
            START_ACCUM_METRICS = &H8
            STOP_ACCUM_METRICS = &H9
            START_TIMED_ACCUM_TEST = &HA
            TOGGLE_LOCK_RELAY_RANGE = &HB
            TRIGGER_WAVEFORM = &HB
            READ_INSTANT_METRICS_RD2X = &HD
            READ_ACCUM_WAVEFORM_DATA = &HE
            READ_HARMONIC_DATA = &HF
            READ_ACCUM_METRICS_RD2X = &H16
            AUTO_CAL = &H1B
            BNC_CONTROl = &H1D
            SYSTEM_STATUS = &H20
            READ_MINIMUM_METRICS_RD2X = &H21
            READ_MAXIMUM_METRICS_RD2X = &H23
            TRIGGER_HARMONIC_ANALYSIS = &H28
            TOGGLE_MODE_RMS_AVG = &H2C
            READ_INSTANT_METRICS_RD3X = &H2E
            READ_ACCUM_METRICS_RD3X = &H2F
            READ_MINIMUM_METRICS_RD3X = &H30
            READ_MAXIMUM_METRICS_RD3X = &H31
            CHANGE_OUTPUT_CONSTANT = &H32
            RUN_STANDARD_TEST = &H34
            RUN_METER_TEST = &H39
        End Enum

        Public PacketTypes As ePacketTypes
        ''' <summary>
        ''' This Function Parses out the Start, PacketType, Length, Data, and Checksum into a Data Structure of the Packet Class
        ''' </summary>
        ''' <param name="aBytes">Byte array Containing Radian Message</param>
        ''' <param name="Packet">Encapsulates the Radian Message</param>
        ''' <returns>  VB Error = 0, 1 = Data, 2 = Delay, 3 = Radian Error, 4 = NOP Reply  </returns>
        ''' <remarks>Frank Boudreau 2012 Landis+Gyr</remarks>
        Public Function i_Parse_Radian_Packet(ByRef aBytes() As Byte, ByRef Packet As cPacket) As Integer
            i_Parse_Radian_Packet = 0
            Try
                'All messages have Start Byte, Packet Type and Length
                Packet.Start = aBytes(0)
                Packet.PacketType = aBytes(1)
                Packet.Checksum = aBytes(aBytes.Length - 1) Or (CUInt(aBytes(aBytes.Length - 2)) * 256)
                'All others Have a length and some sort of data...
                If (Packet.Start And &HF) = CByte(&H6) Then 'Data
                    Packet.Length = aBytes(3) Or (CUInt(aBytes(2)) * 256)
                    ReDim Packet.Data(Packet.Length - 1)
                    For i As Integer = 0 To Packet.Length - 1
                        Packet.Data(i) = aBytes(i + 4)
                    Next
                    i_Parse_Radian_Packet = 1
                ElseIf (Packet.Start And &HF) = CByte(&HC) Then 'Delay  
                    Packet.Length = 2
                    Packet.Delay = aBytes(6) Or (CUInt(aBytes(5)) & 256)
                    i_Parse_Radian_Packet = 2
                ElseIf (Packet.Start And &HF) = CByte(&H9) Then 'Error
                    Packet.Length = 2
                    Packet.ErrorCode = aBytes(6) Or (CUInt(aBytes(5)) * 256)
                    i_Parse_Radian_Packet = 3
                ElseIf (Packet.Start And &HF) = CByte(&H3) Then 'No Data
                    i_Parse_Radian_Packet = 4
                Else
                    Throw New Exception("Unknown Start Packet = 0")
                End If
            Catch
                Throw New Exception("Unknown Start Packet = 0")
            End Try

        End Function

        ''' <summary>
        ''' Parses the data portion fo the packet returned by the RD standard. Assumes the Comm Packet has been validated.  
        ''' Since some Commands have returns with and without data handle both cases.  
        ''' </summary>
        ''' <param name="Packet">Parsed Validated Comm Packet</param>
        ''' <returns>Success = true</returns>
        ''' <remarks></remarks>
        Public Function b_Parse_Radian_Data(Packet As cPacket) As Boolean
            b_Parse_Radian_Data = True
            Select Case Packet.PacketType
                Case ePacketTypes.IDENT
                    Dim Tempstring As String = str_Byte_Array_To_ASCII_String(Packet.Data)
                    Dim strTempStringArray() As String
                    strTempStringArray = Tempstring.Split(","c)
                    Identification.ModelNumber = strTempStringArray(0)
                    Identification.SerialNumber = strTempStringArray(1)
                    Identification.Name = strTempStringArray(2)
                    Identification.VersionNumber = strTempStringArray(3)
                    If Val(Identification.VersionNumber) < 5 Then
                        SamplesPerSecond = SAMPLES_PER_SEC_REV_4_OR_EARLIER
                    Else
                        SamplesPerSecond = SAMPLES_PER_SEC_REV_5_OR_Later
                    End If
                Case ePacketTypes.RESET_METRICS
                    'No Data

                Case ePacketTypes.START_ACCUM_METRICS
                    'Check for Data
                    If Packet.Length > 0 Then
                        If Packet.ControlByte.A = &H0 Then 'Status Request 
                            ControlByteStatus = Packet.Data(0)
                        Else 'Process as if Pulse Remaining Pulse CountThen
                            PulsesLeft = Packet.Data(0) * 256 Or Packet.Data(1)
                        End If
                    End If
                Case ePacketTypes.STOP_ACCUM_METRICS
                    'nodata
                Case ePacketTypes.START_TIMED_ACCUM_TEST
                    Dim uiSamplesLeft As UInteger
                    If Packet.Length = 4 Then
                        uiSamplesLeft = (CUInt(Packet.Data(0)) << 24) Or (CUInt(Packet.Data(1)) << 16) Or (CUInt(Packet.Data(2)) << 8) Or CUInt(Packet.Data(3))
                        TimeLeftForTest = CSng(uiSamplesLeft) / SamplesPerSecond
                    End If
                Case ePacketTypes.TOGGLE_LOCK_RELAY_RANGE
                    If Packet.Length = 3 Then
                        'parse 2X data
                        Packet.ControlByte.A = Packet.Data(0)
                        If Packet.ControlByte.A = eRelayControlByteResponse.UNLOCKED Then
                            RelayStatus.VoltageRelayLocked = False
                            RelayStatus.CurrentRelayLocked = False
                        ElseIf Packet.ControlByte.A = eRelayControlByteResponse.BOTH_LOCKED Then
                            RelayStatus.VoltageRelayLocked = True
                            RelayStatus.CurrentRelayLocked = True
                        Else 'else mask them

                            If (Packet.ControlByte.A And &HF) = eRelayControlByteResponse.UNLOCKED Then
                                RelayStatus.VoltageRelayLocked = False
                                'RelayStatus.CurrentRelayLocked = False
                            End If

                            If (Packet.ControlByte.A And &HF0) = eRelayControlByteResponse.UNLOCKED Then
                                'RelayStatus.VoltageRelayLocked = True
                                RelayStatus.CurrentRelayLocked = False
                            End If

                            If (Packet.ControlByte.A And &HF) = eRelayControlByteResponse.VOLTAGE_RELAY_LOCKED Then
                                RelayStatus.VoltageRelayLocked = True
                                'RelayStatus.CurrentRelayLocked = False
                            End If
                            If (Packet.ControlByte.A And &HF0) = eRelayControlByteResponse.CURRENT_RELAY_LOCKED Then
                                'RelayStatus.VoltageRelayLocked = False
                                RelayStatus.CurrentRelayLocked = True
                            End If
                        End If
                        RelayStatus.VoltageRelayBits = CUInt((CUInt(Packet.Data(1)) << 8) Or CUInt(Packet.Data(2))) And &H7

                        RelayStatus.CurrentRelayBits = (CUInt(Packet.Data(1)) << 8) Or CUInt(Packet.Data(2))
                        RelayStatus.CurrentRelayBits = RelayStatus.CurrentRelayBits And &HF8F8
                        RelayStatus.VoltageRange = -1 'assume fail
                        For i As Integer = 0 To aVoltageRelay.Rangeaddress.Count - 1

                            If aVoltageRelay.Rangeaddress(i) = RelayStatus.VoltageRelayBits Then
                                RelayStatus.VoltageRange = i
                                RelayStatus.VoltageRangeName = aVoltageRelay.Rangename(i)
                                Exit For

                            End If
                        Next

                        RelayStatus.CurrentRange = -1 'assume fail
                        For i As Integer = 0 To aCurrentRelay.Rangeaddress.Count - 1

                            If aCurrentRelay.Rangeaddress(i) = RelayStatus.CurrentRelayBits Then
                                ' RelayStatus.CurrentRange = i
                                RelayStatus.CurrentRangeName = aCurrentRelay.Rangename(i)
                                Exit For
                            End If
                        Next


                    ElseIf Packet.Length = 9 Then
                        'parse 3x data

                    End If
                Case ePacketTypes.TRIGGER_WAVEFORM
                    WaveForm.Status = CInt(Packet.Data(0))
                    If WaveForm.Status = 4 Or WaveForm.Status = 5 Or WaveForm.Status = 6 Then
                        WaveForm.InSimMode = True
                    Else
                        WaveForm.InSimMode = False
                    End If
                Case ePacketTypes.READ_ACCUM_WAVEFORM_DATA
                    'local byte array to hold 32 bt float
                    Dim LocalaBytes(3) As Byte
                    'The wave form sample pointer should not be pointed past the end of the data 
                    If WaveForm.Pointer < WaveForm.EndOfData Then
                        'For each sample in the Packet parse and convert to VB single
                        For i = 0 To Packet.Length - 1 Step 4
                            LocalaBytes(0) = Packet.Data(i)
                            LocalaBytes(1) = Packet.Data(i + 1)
                            LocalaBytes(2) = Packet.Data(i + 2)
                            LocalaBytes(3) = Packet.Data(i + 3)
                            'convert to single and put in waveform buffer
                            WaveForm.VoltageAndCurrentData(i / WAVEFORM_BYTES_PER_SAMPLE + WaveForm.Pointer) = s_TI_to_VB_Single(LocalaBytes)
                        Next
                        'adavance the waveform pointer the number of samples collected (Packet length / 4)
                        WaveForm.Pointer += Packet.Length / WAVEFORM_BYTES_PER_SAMPLE
                        'Once the waveform pointer exceeds the End of the data requested process the data
                        If WaveForm.Pointer >= WaveForm.EndOfData Then
                            'all Samples collected sort into buffers
                            Try
                                If WaveForm.StartOfData < WAVEFORM_CURRENT_OFFSET Then
                                    'Then there has to be voltage data
                                    If WaveForm.EndOfData < WAVEFORM_CURRENT_OFFSET Then
                                        'If the EndOfData is less than Current offset then not all voltage points are collected
                                        're-demension the Data array for the amount of data
                                        ReDim WaveForm.VoltageData(WaveForm.EndOfData - WaveForm.StartOfData - 1)
                                        For i As UShort = WaveForm.StartOfData To WaveForm.EndOfData - 1
                                            WaveForm.VoltageData(i - WaveForm.StartOfData) = WaveForm.VoltageAndCurrentData(i)
                                        Next

                                    Else
                                        'else collect up to the current data boundry
                                        ReDim WaveForm.VoltageData(WAVEFORM_CURRENT_OFFSET - WaveForm.StartOfData - 1)
                                        For i As UShort = WaveForm.StartOfData To WAVEFORM_CURRENT_OFFSET - 1
                                            WaveForm.VoltageData(i - WaveForm.StartOfData) = WaveForm.VoltageAndCurrentData(i)
                                        Next
                                    End If

                                End If
                                'if the endofdata exceeds the Current OFfset then current data needs to be collected
                                If WaveForm.EndOfData > WAVEFORM_CURRENT_OFFSET Then
                                    'if for some reason the data is does not start at the boundry
                                    If WaveForm.StartOfData > WAVEFORM_CURRENT_OFFSET Then
                                        ReDim WaveForm.CurrentData(WaveForm.EndOfData - WaveForm.StartOfData - 1)
                                        For i As UShort = WaveForm.StartOfData To WaveForm.EndOfData - 1
                                            WaveForm.CurrentData(i - WaveForm.StartOfData) = WaveForm.VoltageAndCurrentData(i)
                                        Next
                                    Else
                                        'Collect from Voltage/Current boundry
                                        ReDim WaveForm.CurrentData(WaveForm.EndOfData - WAVEFORM_CURRENT_OFFSET - 1)
                                        For i = WAVEFORM_CURRENT_OFFSET To WaveForm.EndOfData - 1
                                            WaveForm.CurrentData(i - WAVEFORM_CURRENT_OFFSET) = WaveForm.VoltageAndCurrentData(i)
                                        Next
                                    End If

                                End If

                            Catch ex As Exception
                                Dim i As Integer = 1
                            End Try

                        End If
                    Else
                        'Error
                    End If

                    Dim j As Integer = 0
                Case ePacketTypes.READ_INSTANT_METRICS_RD2X
                    Select Case Packet.SubCommand 'This value must be set when a read command is issued to the RD
                        'Only ED 2X sngle phase fpr now
                        Case eInstantMetricsIndex2X.Volts
                            MetricData.Volt = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.Amps
                            MetricData.Amp = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.ALL_RD2X
                            'need to parse all data here
                            Dim i As Integer
                            Dim j As Integer
                            Dim abyteLocal(3) As Byte
                            For i = 0 To Packet.Length - 1
                                abyteLocal(j) = Packet.Data(i)
                                j += 1
                                If i = 3 Then
                                    MetricData.Volt = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 7 Then
                                    MetricData.Amp = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 11 Then
                                    MetricData.Watt = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 15 Then
                                    MetricData.VA = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 19 Then
                                    MetricData.VAR = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 23 Then
                                    MetricData.Frequency = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 27 Then
                                    MetricData.Phase = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 31 Then
                                    MetricData.PowerFactor = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 35 Then
                                    MetricData.AnalogSense = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 39 Then
                                    MetricData.DeltaPhase = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                End If
                            Next
                        Case eInstantMetricsIndex2X.Watts
                            MetricData.Watt = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.Frequency
                            MetricData.Frequency = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.PF
                            MetricData.PowerFactor = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.VA
                            MetricData.VA = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.VAR
                            MetricData.VAR = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.Degrees
                            MetricData.Phase = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.Analog_Sense
                            MetricData.AnalogSense = s_TI_to_VB_Single(Packet.Data)
                        Case eInstantMetricsIndex2X.Delta_Phase
                            MetricData.DeltaPhase = s_TI_to_VB_Single(Packet.Data)
                    End Select
                Case ePacketTypes.READ_ACCUM_METRICS_RD2X
                    'MsgBox("Accumulated Metric message returned")
                    Select Case Packet.SubCommand 'This value must be set when a read command is issued to the RD
                        'Only DO 2X sngle phase for now
                        Case eAccumMetricsIndex.Wh
                            MetricData.WattHr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.VARh
                            MetricData.VARHr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.ALL_RD2X
                            'need to parse all data here
                            Dim i As Integer
                            Dim j As Integer
                            Dim abyteLocal(3) As Byte
                            For i = 0 To Packet.Length - 1
                                abyteLocal(j) = Packet.Data(i)
                                j += 1
                                If i = 3 Then
                                    MetricData.WattHr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 7 Then
                                    MetricData.VARHr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 11 Then
                                    MetricData.QHr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 15 Then
                                    MetricData.VAHr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 19 Then
                                    MetricData.VoltHr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 23 Then
                                    MetricData.AmpHr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 27 Then
                                    MetricData.V2Hr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 31 Then
                                    MetricData.A2Hr = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 35 Then
                                    MetricData.WattHrPlus = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 39 Then
                                    MetricData.WattHrMinus = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 43 Then
                                    MetricData.VarHrPlus = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                ElseIf i = 47 Then
                                    MetricData.VarHrMinus = s_TI_to_VB_Single(abyteLocal)
                                    j = 0
                                End If
                            Next
                        Case eAccumMetricsIndex.Qh
                            MetricData.QHr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.VAh
                            MetricData.VoltHr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.Vh
                            MetricData.VoltHr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.Ah
                            MetricData.AmpHr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.V2h
                            MetricData.V2Hr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.A2h
                            MetricData.A2Hr = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.Wh_Plus
                            MetricData.WattHrPlus = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.Wh_Minus
                            MetricData.WattHrMinus = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.VARh_Plus
                            MetricData.VarHrPlus = s_TI_to_VB_Single(Packet.Data)
                        Case eAccumMetricsIndex.VARh_Minus
                            MetricData.VarHrMinus = s_TI_to_VB_Single(Packet.Data)
                    End Select
                Case ePacketTypes.READ_HARMONIC_DATA
                    MsgBox("Harmonic Metric message returned")

            End Select

        End Function


    End Class
End Module

