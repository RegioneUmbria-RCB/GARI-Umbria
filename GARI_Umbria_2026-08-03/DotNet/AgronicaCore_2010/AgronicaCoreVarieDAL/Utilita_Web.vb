
'Imports System.Threading

Public Class Utilita_Web

    'ricordare di inserire lo script che invoca la funzione HideWait() all'interno del tag Head
    '<script>HideWait();</script>
    Public Shared Function Wait() As String
        Dim Str As New System.Text.StringBuilder
        Str.Append("<div id='mydiv'>")
        Str.Append("<img src='../AB_Immagini/Varie/wait.gif'>")
        Str.Append("</div>")
        Str.Append("<script language=javascript>;")
        Str.Append("function ShowWait(){}")
        Str.Append("function StartShowWait(){mydiv.style.visibility = 'visible'; window.setInterval('ShowWait()',1000);}")
        Str.Append("function HideWait(){mydiv.style.visibility = 'hidden';window.clearInterval();}")
        Str.Append("StartShowWait();</script>")
        Return Str.ToString

    End Function

    Public Shared Function Wait(ByVal profondita As Int32) As String

        Dim Str As New System.Text.StringBuilder
        Dim barra As String
        Dim i As Integer
        For i = 0 To profondita - 1
            barra = barra + "../"
        Next

        Str.Append("<div id='mydiv'>")
        Str.Append("<img src='" + barra + "../AB_Immagini/Varie/wait.gif'>")
        Str.Append("</div>")
        Str.Append("<script language=javascript>;")
        Str.Append("function ShowWait(){}")
        Str.Append("function StartShowWait(){mydiv.style.visibility = 'visible'; window.setInterval('ShowWait()',1000);}")
        Str.Append("function HideWait(){mydiv.style.visibility = 'hidden';window.clearInterval();}")
        Str.Append("StartShowWait();</script>")
        Return Str.ToString
        'Thread.Sleep(10)

    End Function


End Class
