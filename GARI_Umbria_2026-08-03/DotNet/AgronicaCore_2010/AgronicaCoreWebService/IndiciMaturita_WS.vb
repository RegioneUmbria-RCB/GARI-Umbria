Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste
Public Class IndiciMaturita_WS
    Public Function IndiciMaturita(ByVal Input As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_input) As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/IndiciMaturita"
        End If

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output =
            jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output)(rval)

        Return Output

    End Function

End Class
