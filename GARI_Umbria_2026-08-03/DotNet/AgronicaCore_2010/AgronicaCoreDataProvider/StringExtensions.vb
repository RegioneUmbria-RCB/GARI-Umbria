Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Public Module StringExtensions

    <Extension()>
    Public Function ReplaceCaseInsensitive(ByVal stringaSql As String, ByVal oldValue As String, ByVal newValue As String) As String

        Dim i = stringaSql.ToLower.IndexOf(oldValue.ToLower)
        While i <> -1
            stringaSql = stringaSql.Remove(i, oldValue.Length)
            stringaSql = stringaSql.Insert(i, newValue)
            i = stringaSql.ToLower.IndexOf(oldValue.ToLower)
        End While

        Return stringaSql
    End Function

End Module
