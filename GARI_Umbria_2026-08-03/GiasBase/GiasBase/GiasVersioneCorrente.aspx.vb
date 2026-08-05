Imports System.IO
Imports System.Web.Services

Public Class GiasVersioneCorrente
    Inherits System.Web.UI.Page


    <WebMethod()>
    <Script.Services.ScriptMethod(UseHttpGet:=True)>
    Public Shared Function Versioni() As String
        Dim r As String

        Try

            Dim versioneKendo As String = ""

            versioneKendo = "{ ""Nome"":""kendo"", ""Versione"": """ & UltimaVersione() & """ }"


            r = " { ""versioni"": [" & versioneKendo & "], ""errore"": """" }"

        Catch ex As Exception

            r = " { ""versioni"": [], ""errore"": " & ex.Message & """}"

        End Try

        Return r

    End Function

    Private Shared Function UltimaVersione() As String

        Dim fileContents As String
        fileContents = My.Computer.FileSystem.ReadAllText(HttpContext.Current.Server.MapPath(".\kendoui\") & "KendoVersioneCorrente.txt")

        Return fileContents
    End Function

    Private Shared Function UltimaVersioneDaFile() As String

        Dim cartelle As String = HttpContext.Current.Server.MapPath(".\kendoui\")

        Dim ff As New DirectoryInfo(cartelle)
        Dim l As New List(Of String)

        For Each cDir In ff.GetDirectories()
            If cDir.Name.Contains(".") Then
                l.Add(TrimZero(cDir.Name))
            End If
        Next

        Dim rVal As String = ""

        For Each curString In l
            If rVal < curString Then
                rVal = curString
            End If
        Next

        Dim xSplitRval As String() = rVal.Split({"."c})
        For i = 0 To xSplitRval.Length - 1
            xSplitRval(i) = xSplitRval(i).TrimStart(CChar("0"))
        Next
        Return String.Join(".", xSplitRval)

    End Function

    Private Shared Function TrimZero(ByVal s As String) As String

        Dim v As String() = s.Split({"."c})

        For i = 0 To v.Length - 1
            v(i) = v(i).PadLeft(5, CChar("0"))
        Next

        Return String.Join(".", v)

    End Function

End Class
