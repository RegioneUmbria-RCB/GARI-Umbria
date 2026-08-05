Imports System.Security.Policy
Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste
Public Class FasiFenologiche_WS
    Public Function FasiFenologiche(ByVal Input As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input, Optional ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByRef objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False) As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            If Not leggiUrlDaConfigurazioniSiti Then
                Dim objAgroWebConfig As New AgroWebConfig
                url = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali
            End If

            If leggiUrlDaConfigurazioniSiti OrElse String.IsNullOrEmpty(url) Then
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                url = objConfig.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Server)

                If url = "" AndAlso objParametri_SuperServer IsNot Nothing Then
                    url = objConfig.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_SuperServer)
                End If
            End If
            url &= "/FasiFenologiche"
        End If

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output =
            jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output)(rval)

        Return Output

    End Function

    Public Function FasiFenologiche_OLD(ByVal Input As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input) As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali & "/FasiFenologiche_OLD"
        End If

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output =
            jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output)(rval)

        Return Output

    End Function

End Class
