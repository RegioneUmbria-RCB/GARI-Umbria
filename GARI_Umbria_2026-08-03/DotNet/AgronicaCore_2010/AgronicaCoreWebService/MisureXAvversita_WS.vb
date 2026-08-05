Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste
Public Class MisureXAvversita_WS
    Public Function MisureXAvversita(ByVal Input As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_input) As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/MisureXAvversita"
        End If

        Dim jss As New JavaScriptSerializer
        jss.MaxJsonLength = 2147483647
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output =
            jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output)(rval)

        Return Output

    End Function

End Class
