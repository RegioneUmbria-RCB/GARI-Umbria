Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Google.Apis.Auth.OAuth2
Imports Newtonsoft.Json.Linq

Public Class Caricamento_Piattaforma_GEE
    Inherits AlgoritmoBaseGEE
    Implements IAlgoritmoProiezione

    Public Sub New(ByVal Algoritmo_Cod As Int32, ByVal TipoAlgoritmo_Cod As Int32, cfg_siti As JObject)
        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod

        Me.cf_config = New CF_Config With {
            .jsonAuth = cfg_siti("jsonAuth").ToString,
            .endpoint = cfg_siti("endpoint").ToString
        }

    End Sub

    Public Overloads Function Esegui(ByVal LayerAnalysisConfig_Cod As Int32,
                                     ByVal Entita_cod_1 As Int32,
                                     ByVal Entita_cod_2 As Int32,
                                     ByVal Entita_cod_Risultato As Int32,
                                     ByVal Esecuzione_cod As Int32,
                                     ByVal Esecuzione_GUID As String,
                                     ByVal ParametriEsecuzione As String,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                     ByVal Optional override_transazione As Boolean = False) As Boolean Implements IAlgoritmoProiezione.Esegui

        Dim resp As Boolean = True

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri_Server)
            End If

            SetupDatiGEE(LayerAnalysisConfig_Cod, Entita_cod_1, Entita_cod_2, Entita_cod_Risultato, objParametri_Server)

            Dim daAccodare As New JObject

            Dim GUID_entita_1 = LeggiGUIDEntita(Entita_cod_1, objParametri_Server)

            Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

            Dim DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_cod_1, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception("Impossibile leggere i dati geografici del poligono.")
            End If

            Dim geoData = DT.Rows(0)("GeoData").ToString

            Dim input_factory = New ParametriAlgoritmo_Factory
            Dim generatore_input = input_factory.CreaParametri(Of Richiesta_Accodamento_GEE)(LayerAnalysisConfig_AlgorithmType_Cod)
            generatore_input.SetupParametriOpzionali(LayerAnalysisConfig_GUID, Layer_1, Layer_2, Layer_Risultato)

            Dim usaCollectionNames As Boolean = False
            Dim collectionNames As JArray
            Dim responseBody As String

            If LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE Then
                Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim DT_Conf = JObject.Parse(cfgRead.Leggi(0, CostantiPersonalizzate.Google_Earth_Engine_ConfKey, "", "", objParametri_Server).Rows(0)("Valore").ToString)

                collectionNames = CType(DT_Conf("CollectionNames"), JArray)

                For Each index In collectionNames
                    daAccodare = generatore_input.CostruisciRigaDaAccodare(Esecuzione_cod,
                                                                   geoData,
                                                                   GUID_entita_1, "", "",
                                                                   LayerAnalysisConfig_Algorithm_Cod,
                                                                   LayerAnalysisConfig_AlgorithmType_Cod,
                                                                   AttivoTuttoLayer,
                                                                   Esecuzione_GUID,
                                                                   objParametri_Server,
                                                                   objParametri_Utenti,
                                                                   objParametri_Super_Server,
                                                                   collectionName:=index.ToString)

                    Dim newEsecuzioneGUID = daAccodare.SelectToken("geoJsonPolygon.features[0].properties.GIS_LayerAnalysisConfig_Exec_Log_GUID")

                    If Not IsNothing(newEsecuzioneGUID) Then
                        Esecuzione_GUID = newEsecuzioneGUID.ToString
                    End If

                    responseBody = AccodaCF(daAccodare, objParametri_Server)
                Next
            Else
                daAccodare = generatore_input.CostruisciRigaDaAccodare(Esecuzione_cod,
                                                                       geoData,
                                                                       GUID_entita_1, "", "",
                                                                       LayerAnalysisConfig_Algorithm_Cod,
                                                                       LayerAnalysisConfig_AlgorithmType_Cod,
                                                                       AttivoTuttoLayer,
                                                                       Esecuzione_GUID,
                                                                       objParametri_Server,
                                                                       objParametri_Utenti,
                                                                       objParametri_Super_Server)

                responseBody = AccodaCF(daAccodare, objParametri_Server)
            End If

            If LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync Or
                LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly Then

                Dim responseObj = JObject.Parse(responseBody)

                EseguiOperazioniSincrone_Intersezione(responseObj,
                                                      Esecuzione_cod,
                                                      LayerAnalysisConfig_Cod,
                                                      Entita_cod_1,
                                                      Entita_cod_2,
                                                      GUID_entita_1,
                                                      objParametri_Server,
                                                      objParametri_Utenti)

                If Not ParametriEsecuzione.Equals("") Then

                    'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
                    Dim algExt As New Algorithm_Extension
                    algExt.AlgorithmRaster_Extension(LayerAnalysisConfig_Cod,
                                                     Esecuzione_cod,
                                                     Esecuzione_GUID,
                                                     ParametriEsecuzione,
                                                     responseObj,
                                                     objParametri_Server)

                End If

            End If

            resp = True

            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing And Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            If Not override_transazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            End If
        End Try

        Return resp
    End Function

    Private Sub EseguiOperazioniSincrone_Intersezione(ByVal responseObj As JObject,
                                                      ByVal esecuzione_cod As Integer,
                                                      ByVal layerAnalysisConfig_Cod As Integer,
                                                      ByVal entita_cod_1 As Integer,
                                                      ByVal entita_cod_2 As Integer,
                                                      ByVal gUID_entita_1 As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim resp As Boolean

        Dim messaggio As String
        Dim newIDEntita As Int32 = 0

        If responseObj("1.0") IsNot Nothing AndAlso CDbl(responseObj("1.0")) > 0 Then

            resp = xWrite.SalvaElementoGraficoIntersezioneDaRisultatoGEE(esecuzione_cod,
                                                                         layerAnalysisConfig_Cod,
                                                                         responseObj,
                                                                         newIDEntita,
                                                                         objParametri_Server,
                                                                         objParametri_Utenti)

            If Not resp Then
                Throw New Exception("Errore nella scrittura dell'elemento grafico per l'intersezione.")
            End If

            messaggio = String.Format("Il poligono interseca l'elemento per il {0}%.", responseObj("1.0").ToString)
        Else
            messaggio = "Il poligono non interseca l'elemento."
        End If

        Scrivi_LOG(objParametri_Server, "EseguiOperazioniSincrone_Intersezione", "Payload: " & responseObj.ToString(), False)

        If responseObj("RisultatoOperazione") IsNot Nothing Then

            Dim risultati = CType(responseObj("RisultatoOperazione"), JArray)
            Dim risultatoString As String = ""

            For Each annoObj As JObject In risultati
                Dim annoUltimeCifre = annoObj.Properties.First.Name.ToString

                Dim anno = ""
                If (annoUltimeCifre.Length = 4) Then
                    anno = annoUltimeCifre
                Else
                    anno = String.Format("{0}{1}", Date.Now.Year.ToString.Substring(0, 2), annoUltimeCifre)
                End If

                risultatoString = String.Format("{0}{1}: {2}, ", risultatoString, anno, annoObj(annoUltimeCifre).ToString)
            Next

            If Not risultatoString.Equals("") Then
                'Rimuovo ultima virgola
                messaggio = risultatoString.Substring(0, risultatoString.Length - 2)

                resp = xWrite.SalvaElementoGraficoIntersezioneDaRisultatoGEE(esecuzione_cod,
                                                                             layerAnalysisConfig_Cod,
                                                                             responseObj,
                                                                             newIDEntita,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti,
                                                                             messaggio)

                If Not resp Then
                    Scrivi_LOG(objParametri_Server, "EseguiOperazioniSincrone_Intersezione", "Errore nella scrittura dell'elemento grafico per il calcolo della deforestazione.", False)
                    Throw New Exception("Errore nella scrittura dell'elemento grafico per il calcolo della deforestazione.")
                End If
            End If
        End If

        resp = xWrite.InserisciLogEsecuzioneAlgoritmo(layerAnalysisConfig_Cod,
                                                      esecuzione_cod,
                                                      entita_cod_1,
                                                      entita_cod_2,
                                                      newIDEntita,
                                                      1,
                                                      messaggio,
                                                      objParametri_Server,
                                                      gUID_entita_1)

        If Not resp Then
            Throw New Exception("Errore nella scrittura del log dell'operazione.")
        End If

    End Sub

    Private Function AccodaCF(ByVal daAccodare As JObject, ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim resp As String = ""

        Dim jsonAuth = Me.cf_config.jsonAuth
        Dim credential As GoogleCredential = GoogleCredential.FromJson(System.Text.RegularExpressions.Regex.Unescape(jsonAuth))
        Dim audience = Me.cf_config.endpoint

        'DEBUG DEBUG DEBUG
        'Dim jsonAuth = System.IO.File.ReadAllText("C:\Files_Test\micheleFurnoKey.json")
        'Dim credential As GoogleCredential = GoogleCredential.FromJson(System.Text.RegularExpressions.Regex.Unescape(jsonAuth))
        'Dim audience = "https://europe-west1-ee-simoparmegtest.cloudfunctions.net/SubmitGISAlgorithm"
        'DEBUG DEBUG DEBUG

        Dim token = credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(audience), CancellationToken.None).Result
        Dim bearer As String = token.GetAccessTokenAsync(CancellationToken.None).Result

        Dim cf_client As New HttpClient()

        Dim payloadAsArray = New JArray
        payloadAsArray.Add(daAccodare)
        Dim content As New StringContent(payloadAsArray.ToString, Text.Encoding.UTF8, "application/json")

        cf_client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", bearer)

        Dim hr As HttpResponseMessage = cf_client.PostAsync(audience, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Dim geeError = hr.Content.ReadAsStringAsync().Result
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, IIf(Not geeError Is Nothing, geeError, hr.ReasonPhrase)))
        End If

        If LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync Or
            LayerAnalysisConfig_AlgorithmType_Cod = TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly Then
            resp = hr.Content.ReadAsStringAsync().Result
        End If

        cf_client.Dispose()

        Return resp

    End Function
End Class
