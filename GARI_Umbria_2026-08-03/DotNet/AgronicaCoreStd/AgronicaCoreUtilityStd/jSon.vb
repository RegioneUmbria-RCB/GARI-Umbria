Public Class jSon

    Public Shared Function Escape(ByVal s As String) As String
        Return s.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
    End Function

    Public Shared Function Escape2(ByVal s As String) As String
        Return s.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Replace("'", "\'")
    End Function

End Class
