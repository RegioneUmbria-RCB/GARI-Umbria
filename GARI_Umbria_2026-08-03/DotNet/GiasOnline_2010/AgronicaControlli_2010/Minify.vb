Public Class Minify
    'Inherits System.Web.UI.WebControls.WebControl

    Public Shared Function LinkJsMin(ByVal percorsoFile As String)
        Dim minified As Boolean = False
        Dim giasVersione As String = ""
        Try
            minified = CBool(HttpContext.Current.Session("UsaFileMinify"))
            giasVersione = HttpContext.Current.Application("GiasVersioneCorrente")
        Catch ex As Exception
            giasVersione = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("")
        End Try

        Dim newFile As String = ""
        If minified = True Then
            If percorsoFile.EndsWith(".js") AndAlso Not percorsoFile.EndsWith(".min.js") Then
                newFile = percorsoFile.Remove(percorsoFile.Length - 3, 3) & ".build.min.js"
            Else
                'per evitare danni lo includo così com'è
                newFile = percorsoFile
            End If
        Else
            'è inutile che smanazzo, lascio tutto così com'è
            newFile = percorsoFile
        End If

        Dim linkFile As String = newFile & If(Not String.IsNullOrEmpty(giasVersione), "?" & giasVersione, "")
        Return linkFile
    End Function

End Class
