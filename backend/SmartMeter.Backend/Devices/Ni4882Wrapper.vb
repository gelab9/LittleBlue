Imports System
Imports System.Runtime.InteropServices
Imports System.Text

' Define the NationalInstruments.NI4882 namespace at global scope
Namespace Global.NationalInstruments.NI4882

    ''' <summary>
    ''' Minimal managed wrapper around libni4882 for Linux/macOS compatibility.
    ''' Provides the basic Device interface needed by CalInst.vb
    ''' </summary>
    Public Class Device
        Implements IDisposable

        ' NI-488.2 library functions
        <DllImport("ni4882", EntryPoint:="ibdev")>
        Private Shared Function ibdev(board As Integer, pad As Byte, sad As Byte,
                                       timeout As Integer, sendEoi As Integer,
                                       eosMode As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibwrt")>
        Private Shared Function ibwrt(ud As Integer, buf As String, len As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibrd")>
        Private Shared Function ibrd(ud As Integer, buf As StringBuilder, len As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibonl")>
        Private Shared Function ibonl(ud As Integer, v As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibwrta")>
        Private Shared Function ibwrta(ud As Integer, buf As Byte(), len As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibrda")>
        Private Shared Function ibrda(ud As Integer, buf As Byte(), len As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibtmo")>
        Private Shared Function ibtmo(ud As Integer, timeout As Integer) As Integer
        End Function

        <DllImport("ni4882", EntryPoint:="ibsta")>
        Private Shared Function ibsta() As Integer
        End Function

        Private _ud As Integer
        Private _disposed As Boolean = False
        Private _timeout As Integer = 2000

        ''' <summary>
        ''' Create and open a GPIB device.
        ''' </summary>
        Public Sub New(board As Integer, pad As Byte, sad As Byte)
            _ud = ibdev(board, pad, sad, 2000, 0, 0)
            If _ud < 0 Then
                Throw New Exception($"GPIB device init failed: ibdev returned {_ud}")
            End If
        End Sub

        ''' <summary>
        ''' Write a string to the GPIB device.
        ''' </summary>
        Public Sub Write(message As String)
            ThrowIfDisposed()
            Dim bytes = Encoding.ASCII.GetBytes(message)
            Dim result = ibwrta(_ud, bytes, bytes.Length)
            If result < 0 Then
                Throw New Exception($"GPIB write failed: ibwrta returned {result}")
            End If
        End Sub

        ''' <summary>
        ''' Read a string from the GPIB device.
        ''' </summary>
        Public Function ReadString() As String
            ThrowIfDisposed()
            Dim buf = New Byte(255) {}
            Dim result = ibrda(_ud, buf, buf.Length)
            If result < 0 Then
                Throw New Exception($"GPIB read failed: ibrda returned {result}")
            End If
            ' Find the null terminator
            Dim len = Array.IndexOf(buf, CByte(0))
            If len < 0 Then len = buf.Length
            Return Encoding.ASCII.GetString(buf, 0, len)
        End Function

        ''' <summary>
        ''' Get or set the I/O timeout (in milliseconds).
        ''' </summary>
        Public Property IOTimeout As Integer
            Get
                Return _timeout
            End Get
            Set(value As Integer)
                _timeout = value
                ibtmo(_ud, value)
            End Set
        End Property

        ''' <summary>
        ''' Close the GPIB device.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                ibonl(_ud, 0)
                _disposed = True
            End If
        End Sub

        Private Sub ThrowIfDisposed()
            If _disposed Then
                Throw New ObjectDisposedException("Device")
            End If
        End Sub
    End Class

End Namespace
