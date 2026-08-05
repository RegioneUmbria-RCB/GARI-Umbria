Imports System.Text.RegularExpressions

'Imports System.Threading

Public Class Utilita_Compressione
    Private Shared ReadOnly RegexBetweenTags As New Regex(">(?! )\s+", RegexOptions.Compiled, TimeSpan.FromSeconds(3))
    Private Shared ReadOnly RegexLineBreaks As New Regex("([\n\s])+?(?<= {2,})<", RegexOptions.Compiled, TimeSpan.FromSeconds(3))


    Public Shared Function RemoveWhitespaceFromHtml(ByVal html As String) As String
        html = RegexBetweenTags.Replace(html, ">")
        html = RegexLineBreaks.Replace(html, "<")
        Return html.Trim()
    End Function
 
End Class
