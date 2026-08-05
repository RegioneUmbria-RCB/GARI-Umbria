Imports System.Text
Imports System.IO
Imports System.Collections.Concurrent

Public Class FileLogger
    Implements IDisposable

    Private _cache As ConcurrentQueue(Of String)
    Private _commitRow As Integer
    Private _currentLogRow As Integer
    Private _logFullPath As String
    Private _bFlushing As Boolean
    Private _tagname As String

    Public Sub New(ByVal logPath As String,
                   ByVal logName As String,
                   ByVal TagName As String,
                   Optional ByVal commitRow As Integer = 50)
        _cache = New ConcurrentQueue(Of String)
        _commitRow = commitRow
        _currentLogRow = 0
        _bFlushing = False
        If Directory.Exists(logPath) = False Then
            Directory.CreateDirectory(logPath)
        End If
        _tagname = TagName
        _logFullPath = Path.Combine(logPath, IIf(_tagname <> "", "TASK" + _tagname, "") + "_" + logName)
        If File.Exists(_logFullPath) = False Then
            File.Create(_logFullPath)
        End If

    End Sub

    Public Sub AppendLog(ByVal Text As String,
                         Optional ByVal LogType As LogType = LogType.Informazione,
                         Optional ByVal ex As Exception = Nothing)
        If _currentLogRow >= _commitRow Then
            Flush()
        End If

        Dim s As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff")
        Select Case LogType
            Case LogType.Informazione
                s += " INFO "
            Case LogType.Warning
                s += " WARNING "
            Case LogType.Errore
                s += " ERROR "
            Case LogType.Critical
                s += " CRITICAL "
        End Select
        If _tagname <> "" Then
            s += "- Task " + _tagname
        End If
        s += "- " + Text
        _cache.Enqueue(s & vbCrLf)

        _currentLogRow += 1
        If ex IsNot Nothing Then
            _cache.Enqueue("Stack trace: " + ex.StackTrace.ToString() & vbCrLf)
            _currentLogRow += 1
        End If

        If _currentLogRow >= _commitRow Then
            Flush()
        End If


    End Sub

    Public Sub Flush()
        If _bFlushing Then
            Return
        Else
            _bFlushing = True
            Dim s As String
            While _cache.TryDequeue(s)
                File.AppendAllText(_logFullPath, s, Encoding.UTF8)
            End While
            _bFlushing = False
            _currentLogRow = 0
        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Flush()
        _cache = Nothing
    End Sub

End Class
