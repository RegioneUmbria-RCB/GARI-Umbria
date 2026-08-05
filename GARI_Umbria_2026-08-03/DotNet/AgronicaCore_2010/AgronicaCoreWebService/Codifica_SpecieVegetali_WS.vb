
Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste

Public Class Codifica_SpecieVegetali_WS

    Public Function SpecieVegetaliTranscod(ByVal input As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetali_Input) As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetali_Output

        Dim objAgroWebConfig As New AgroWebConfig
        Dim url As String = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/SpecieVegetaliTranscod"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(input)

        Dim hlpHttp As New Http
        Dim rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetali_Output = jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetali_Output)(rval)

        Return Output
    End Function

    Public Function SpecieVegetaliClienteTranscod(ByVal input As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetaliCliente_Input) As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetaliCliente_Output

        Dim objAgroWebConfig As New AgroWebConfig
        Dim url As String = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/SpecieVegetaliClienteTranscod"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(input)

        Dim hlpHttp As New Http
        Dim rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetaliCliente_Output = jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetaliCliente_Output)(rval)

        Return Output
    End Function

    Public Function ImpiantiIrriguiClienteTranscod(ByVal input As AgronicaCoreMetaSchemaBIZ.Codifica_ImpiantiIrrigui_Cliente_Input) As AgronicaCoreMetaSchemaBIZ.Codifica_ImpiantiIrrigui_Cliente_Output

        Dim objAgroWebConfig As New AgroWebConfig
        Dim url As String = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/ImpiantiIrriguiClienteTranscod"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(input)

        Dim hlpHttp As New Http
        Dim rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.Codifica_ImpiantiIrrigui_Cliente_Output = jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.Codifica_ImpiantiIrrigui_Cliente_Output)(rval)

        Return Output
    End Function

End Class
