Public Class AgroGISHelper




    Public Shared Function GetValueFromGeoDes(chiave As String, elem As String) As String

        Dim v1 As String() = elem.Split("|")

        For Each s As String In v1
            If s.ToLower.Contains(chiave.ToLower) Then
                Return s.Split("§")(1).Trim
            End If
        Next

        Return ""

    End Function

End Class
