Imports AgronicaCoreGestioneRichieste
Imports System.Web.Script.Serialization

Public Class Infestanti_WS
    Public Function ErbeInfestanti(ByVal Input As AgronicaCoreMetaSchemaBIZ.Infestanti_input) As AgronicaCoreMetaSchemaBIZ.Infestanti_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/ErbeInfestanti"
        End If

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.Infestanti_output =
        jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.Infestanti_output)(rval)

        Return Output

    End Function

End Class