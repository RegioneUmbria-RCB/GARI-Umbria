Public Class DebugHelper

    Private _watch As Object

    Public Sub DebugWriteLine(ByVal message As String)
        If Debugger.IsAttached Then
            Debug.WriteLine(message)
        End If
    End Sub

    Public Sub WatchStartNew()
        If Debugger.IsAttached Then
            _watch = Stopwatch.StartNew()
        End If
    End Sub

    Public Sub DebugWriteLineElapsedMs(ByVal message As String)
        If Debugger.IsAttached Then
            Debug.WriteLine(message + ": " + _watch.ElapsedMilliseconds.ToString())
            _watch.Restart()
        End If
    End Sub

End Class
