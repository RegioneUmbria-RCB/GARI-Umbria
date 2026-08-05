Imports System.Net
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Agea
Imports AgronicaCoreDemetraBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class Utility

    Private _LinkAPIHubAgea As String = ""

    Private Function GetLinkAPIHubAgea(ByVal objParametri_Super_Server As AgronicaCoreParametri)

        Dim objConfig_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        If String.IsNullOrEmpty(_LinkAPIHubAgea) Then
            _LinkAPIHubAgea = objConfig_Siti.Leggi_Valore(0, "LinkAPIHubAgea", "", "", objParametri_Super_Server)
        End If

        Return _LinkAPIHubAgea
    End Function

    Private Function CallHubAgea(ByVal request As String, ByVal endpoint As String, ByVal method As String, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim risp As String = ""

        Dim objConfig_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim apiKeyBase64 As String = objConfig_Siti.Leggi_Valore(0, "APIKeyHubAgea", "", "", objParametri_Super_Server)

        Dim url As String = GetLinkAPIHubAgea(objParametri_Super_Server)

        If Not String.IsNullOrEmpty(apiKeyBase64) AndAlso Not String.IsNullOrEmpty(url) Then

            Dim apiKey As String = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, apiKeyBase64)

            Dim completeUrl As String = url & endpoint

            Dim http_Request = New AgronicaCoreUtility.Http
            Dim hdr = New WebHeaderCollection()

            hdr.Add("x-api-key", apiKey)

            risp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", method, "application/json", "", hdr)

        Else

            Throw New Exception("Chiavi APIKeyHubAgea e\o LinkAPIHubAgea non valorizzate")

        End If

        Return risp
    End Function

    Public Function CallHubAgeaPOST(ByVal request As String, ByVal endpoint As String, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String
        Return CallHubAgea(request, endpoint, "POST", objParametri_Super_Server)
    End Function

    Public Function CallHubAgeaGET(ByVal request As String, ByVal endpoint As String, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String
        Return CallHubAgea(request, endpoint, "GET", objParametri_Super_Server)
    End Function

    Public Sub WriteLogToElasticSearch(ByVal CUAA As String, ByVal bundle As Bundle, ByVal message As String, ByVal severity As String,
                                       ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Super_Server As AgronicaCoreParametri)

        Dim objConfig_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim environmentSTR As String = objConfig_Siti.Leggi_Valore(0, "ParametriElasticSearch", "", "", objParametri_Server)

        Dim url As String = objConfig_Siti.Leggi_Valore(0, "Coldiretti_ElasticSearchUrl", "", "", objParametri_Server)

        If Not String.IsNullOrEmpty(environmentSTR) AndAlso Not String.IsNullOrEmpty(url) Then

            Dim parametri = JsonConvert.DeserializeObject(Of ParametriElasticSearch)(environmentSTR)

            Dim objLogger As New AgronicaCoreDemetraBIZ.ElasticSearchLogger(parametri.Ambiente, url.Replace("generico", "g2a.creazione_fornitura"))

            Dim urlHubAgea As String = GetLinkAPIHubAgea(objParametri_Super_Server)

            If Not String.IsNullOrEmpty(urlHubAgea) AndAlso Not String.IsNullOrEmpty(message) Then
                message &= " - URL HubAgea: " & urlHubAgea
            End If

            Dim payloadToES_JSON As New payloadToES_JSON With {.CUAA = CUAA, .dati = bundle}

            objLogger.WriteLogWithoutTask(CUAA, "INS", "Gias2AgeaHub", severity, message, payloadToES_JSON, Now, Now)

        End If


    End Sub

    Public Function FindImpianto(ByVal impiantoKey As Impianto.PK, ByVal DT_Appezzamenti_Impianti As DataTable) As DataRow

        Dim Dr As DataRow = Nothing

        Dim Piva As String = impiantoKey.appezzamentoPK.centroAziendalePK.partitaIva

        Dim Sa_Cod As Integer = impiantoKey.appezzamentoPK.centroAziendalePK.codice

        Dim Appezza As Integer = impiantoKey.appezzamentoPK.codice

        Dim Id_Reg As Integer = impiantoKey.codice

        Dim Drs As DataRow() = DT_Appezzamenti_Impianti.Select("Piva = '" & Piva & "' And Sa_Cod = " & Sa_Cod & " And Appezza = " & Appezza & " And Id_Reg = " & Id_Reg)

        If Not IsNothing(Drs) AndAlso Drs.Count = 1 Then
            Dr = Drs(0)
        End If

        Return Dr

    End Function


End Class

Public Class InfoExceptionGias2AgeaHub : Inherits Exception

    Sub New(_excMsg As String)
        MyBase.New(_excMsg)
    End Sub
End Class


