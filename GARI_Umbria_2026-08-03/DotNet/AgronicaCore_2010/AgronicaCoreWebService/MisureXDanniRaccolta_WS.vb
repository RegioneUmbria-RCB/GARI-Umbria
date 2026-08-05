Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste
Public Class MisureXDanniRaccolta_WS

    Public Function MisureXDanniRaccolta(ByVal Input As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_input) As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/MisureXDanniRaccolta"
        End If

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output =
            jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output)(rval)

        Return Output

    End Function


End Class
