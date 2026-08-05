Public Class jSon

    Public Shared Function Escape(ByVal s As String) As String

        ' VAnni: 2/10/2018: se s è nothing allora va in errore il replace
        If String.IsNullOrEmpty(s) Then
            Return s
        End If

        Return s.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")

    End Function

    Public Shared Function Escape2(ByVal s As String) As String
        Return s.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Replace("'", "\'")
    End Function

End Class
