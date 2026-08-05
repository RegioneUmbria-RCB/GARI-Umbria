Public Class pathHelper
    Public Shared Function getFilepart(ByVal s As String, ByVal compareDir As String) As String
        Dim app As String() = s.Split("\")
        Dim rval As String = ""
        Dim f As Boolean = False
        For Each curValue In app
            If f Then
                rval &= curValue & "\"
            End If
            If curValue.ToLower = compareDir.ToLower Then
                f = True
            End If
        Next
        Return rval.TrimEnd("\")
    End Function


    Public Shared Function addslash(ByVal s As String) As String
        If Not s.EndsWith("\") Then
            Return s & "\"
        Else
            Return s
        End If
    End Function

End Class
