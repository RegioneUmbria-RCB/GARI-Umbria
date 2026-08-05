Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Security.Policy
Imports System.Net
Imports AgronicaCoreModelsSTD.exceptions

Public Class Caricamento_WS_Mappe
    Inherits AlgoritmoBaseWSMappe
    Implements IAlgoritmoProiezione

    Public Sub New(ByVal Algoritmo_Cod As Int32,
                   ByVal TipoAlgoritmo_Cod As Int32,
                   ByRef objParametri_Server As AgronicaCoreParametri)

        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod

        'recupero le configurazioni per richiamare le webapi di ws_mappe
        Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Me.ws_mappe_config = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.Gis.WS_Mappe_Config)(cfgRead.Leggi(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server).Rows(0)("Valore").ToString)

        If Me.ws_mappe_config.baseUrl = "" Then
            Throw New GiasException("BaseUrl WS_Mappe_2024 non trovato impossibile proseguire")
        End If

        Me._caller = New WebApiCaller(Me.ws_mappe_config.baseUrl, New Tuple(Of String, String)(Me.ws_mappe_config.auth_Key.type, Me.ws_mappe_config.auth_Key.value), "")

        If Debugger.IsAttached Then
            ServicePointManager.ServerCertificateValidationCallback = Function(s, c, h, e) True
        End If


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

        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W


        Dim sensorCode As String = "NDVI"                               'temporaneo in attesa di ingengerizzazione
        Dim sourceCode As String = "1"                                  'temporaneo in attesa di ingengerizzazione
        Dim startReq As Date = New Date(Date.Now.Year - 1, 1, 1)        'temporaneo in attesa di ingengerizzazione
        Dim endReq As Date = New Date(Date.Now.Year, 12, 31)            'temporaneo in attesa di ingengerizzazione

        Try
            'If Not override_transazione Then
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
            '                                                                            FlagTransazioneLocale,
            '                                                                            objParametri_Server)
            'End If

            SetupDati(LayerAnalysisConfig_Cod, Entita_cod_1, Entita_cod_2, Entita_cod_Risultato, objParametri_Server)

            Dim daAccodare As New JObject

            Dim GUID_entita_1 = LeggiGUIDEntita(Entita_cod_1, objParametri_Server)
            Dim GUID_entita_2 = If(Entita_cod_2 > 0, LeggiGUIDEntita(Entita_cod_2, objParametri_Server), "")

            If Esecuzione_GUID = "" Then
                Esecuzione_GUID = CreaNuovoGUIDEsecuzione(Esecuzione_cod, objParametri_Server)
            End If

            Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

            Dim DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_cod_1, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception("Impossibile leggere i dati geografici del poligono.")
            End If

            Dim geoData = DT.Rows(0)("GeoData").ToString

            Dim input_factory = New ParametriAlgoritmo_Factory
            Dim generatore_input = input_factory.CreaParametri(Of Richieste_WS_Mappe)(LayerAnalysisConfig_AlgorithmType_Cod)
            generatore_input.SetupParametriOpzionali(LayerAnalysisConfig_GUID, Layer_1, Layer_2, Layer_Risultato)

            '1- richiamo api per recupero elenco file da elaborare
            Dim responsebody As CustomMapOverlayBaseInizializzaCalendario_Out = GetElencoImmaginiDaElaborare(JsonConvert.SerializeObject(generatore_input.GetRequestInizializzaCalendario(geoData, sensorCode, sourceCode, startReq, endReq)))

            'recupero l'elenco degli url delle immagini da rasterizzare
            Dim urlList As New List(Of Tuple(Of String, Date))
            For Each item In responsebody.stringResponse
                For Each itm In item.passaggi.Select(Of Tuple(Of String, Date))(Function(x) New Tuple(Of String, Date)(x.url, item.dataRiferimento)).Distinct().ToList()
                    urlList.Add(itm)
                Next
            Next

            Dim savePath = GetPathToSave(objParametri_Server)

            For Each item In urlList
                'per ogni immagine genero il ritaglio e lo memorizzo in locale
                GenerateAndDownloadRasterCrop(Entita_cod_1,
                                              item.Item2,
                                              geoData,
                                              item.Item1,
                                              4326,
                                              savePath,
                                              generatore_input,
                                              objParametri_Server)
                Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri_Server.UtenteUsername,
                .LogDirectory = IO.Path.Combine(objParametri_Server.LogDirectory, "WS_Mappe_2024"),
                .LogFileName = Date.Now.ToString("yyyy-MM-dd") + ".log"
            }
                Scrivi_LOG(objParametri_Server,
                       "WS_Mappe_2024 - Rasterize Crop",
                       String.Format("Eseguito richiamo ritaglio ws_mappe_2024 per Long'url {0}", item.Item1),
                       CustomLOGParams:=customLOGParams)
            Next

            Dim messaggio As String = CostruisciPayloadRisultato(Esecuzione_cod,
                                                                 JsonConvert.SerializeObject(responsebody.stringResponse),
                                                                 GUID_entita_1,
                                                                 GUID_entita_2,
                                                                 "",
                                                                 LayerAnalysisConfig_Algorithm_Cod,
                                                                 LayerAnalysisConfig_AlgorithmType_Cod,
                                                                 False,
                                                                 Esecuzione_GUID,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti,
                                                                 objParametri_Super_Server).ToString()

            resp = xWrite.InserisciLogEsecuzioneAlgoritmo(LayerAnalysisConfig_Cod,
                                                          Esecuzione_cod,
                                                          Entita_cod_1,
                                                          Entita_cod_2,
                                                          0,
                                                          1,
                                                          "",
                                                          objParametri_Server,
                                                          GUID_entita_1,
                                                          Risultato_Json:=messaggio)


            'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
            Dim algExt As New Algorithm_Extension
            algExt.AlgorithmRaster_Extension(LayerAnalysisConfig_Cod,
                                                     Esecuzione_cod,
                                                     Esecuzione_GUID,
                                                     ParametriEsecuzione,
                                                     JObject.Parse(messaggio),
                                                     objParametri_Server)

            resp = True

            'If Not override_transazione Then
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            'End If

        Catch ex As Exception
            'If Not objParametri_Server.objTransazione Is Nothing And Not override_transazione Then
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'End If

            Throw ex
        Finally
            'If Not override_transazione Then
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
            'End If
        End Try

        Return resp
    End Function

    Private Function GetElencoImmaginiDaElaborare(ByVal request As String) As CustomMapOverlayBaseInizializzaCalendario_Out
        Dim res = Me._caller.ChiamaWS(Of CustomMapOverlayBaseInizializzaCalendario_Out)(HttpMethod.Post,
                                                                                Nothing,
                                                                                Nothing,
                                                                                request,
                                                                                "application/json",
                                                                                ENDPOINT_CUSTOMOVERLAYBASE_INIZIALIZZACALENDARIO)
        If res Is Nothing Then
            Throw New GiasException("no res 1")
        End If

        If res.stringResponse Is Nothing OrElse res.stringResponse.Count <= 0 Then
            Throw New GiasException("nessun dato satellitare recuperato")
        End If

        Return res
    End Function

    Private Sub GenerateAndDownloadRasterCrop(ByVal entita_cod As Integer,
                                              ByVal data_immagine As DateTime,
                                              ByVal wkt As String,
                                              ByVal url As String,
                                              ByVal srid As Integer,
                                              ByVal localPath As String,
                                              ByVal generatore_input As Richieste_WS_Mappe,
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              )

        Dim crop = GetRasterCrop(JsonConvert.SerializeObject(generatore_input.GetRequestRasterCrop(wkt, url, srid)))

        If crop IsNot Nothing Then
            DownloadTiffLocallyAndSaveChronoTable(generatore_input.GetRequestRasterized(crop.cropId, True),
                                                  entita_cod,
                                                  data_immagine,
                                                  localPath,
                                                  objParametri_Server)

        Else
            Throw New GiasException(String.Format("Nessuna immagine generata per l'url {0}", url))
        End If
    End Sub

    Private Function GetRasterCrop(ByVal request As String) As RasterCrop_Out
        Return Me._caller.ChiamaWS(Of RasterCrop_Out)(HttpMethod.Post,
                                                                  Nothing,
                                                                  Nothing,
                                                                  request,
                                                                  "application/json",
                                                                  ENDPOINT_RASTERCROP)
    End Function

    Private Sub DownloadTiffLocallyAndSaveChronoTable(ByVal request As Rasterized_In,
                                                      ByVal entita_cod As Integer,
                                                      ByVal data_immagine As DateTime,
                                                      ByVal localPath As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri)
        Dim data = Me._caller.ChiamaWS(Of Byte())(HttpMethod.Post,
                                                                  Nothing,
                                                                  Nothing,
                                                                  JsonConvert.SerializeObject(request),
                                                                  "application/json",
                                                                  ENDPOINT_RASTERIZED,
                                                                  SaveStreamToFile:=True,
                                                                  savePath:=localPath)
        If data IsNot Nothing Then
            Dim xProjW As New AgronicaCoreGisDAL.ProiezioniLayer_W
            xProjW.InserisciChronoImage(entita_cod, data_immagine, request.ImageId.ToString(), 0, objParametri_Server)
        End If
    End Sub

    Public Function CostruisciPayloadRisultato(ByVal Esecuzione_cod As Int32,
                                               ByVal payloadObj As String,
                                               ByVal GuidEntita1 As String,
                                               ByVal GuidEntita2 As String,
                                               ByVal GuidEntitaRisultato As String,
                                               ByVal LayerAnalysisConfig_Algorithm_Cod As Int32,
                                               ByVal LayerAnalysisConfig_AlgorithmType_Cod As Int32,
                                               ByVal AttivoTuttoLayer As Boolean,
                                               ByVal Esecuzione_GUID As String,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                               ByVal Optional elementoGraficoPerDescrizione As Int32 = 0,
                                               ByVal Optional collectionName As String = "",
                                               ByVal Optional viewNames As List(Of String) = Nothing,
                                               ByVal Optional paletteVisualizzazione As String = "") As JObject

        Dim output As New JObject

        Dim geoJsonPolygon As New JObject

        geoJsonPolygon.Add("type", "FeatureCollection")

        Dim features As New JArray

        Dim feature As New JObject

        feature.Add("type", "Feature")

        feature.Add("geometry", Nothing)

        Dim properties As New JObject

        Dim guidExec As String = Esecuzione_GUID

        'If guidExec.Equals("") Then
        '    guidExec = CreaNuovoGUIDEsecuzione(Esecuzione_cod, objParametri_Server)
        'End If

        properties.Add("PivaSuperUser", objParametri_Server.PivaSuperUser)
        properties.Add("idDB", Nothing)
        properties.Add("GIS_LayerAnalysisConfig_Exec_Log_GUID", guidExec)
        properties.Add("featureGUID", GuidEntita1)
        properties.Add("LayerAnalysisConfig_GUID", LayerAnalysisConfig_GUID)
        properties.Add("LayerAnalysisConfig_Algorithm_Cod", LayerAnalysisConfig_Algorithm_Cod)
        properties.Add("AttivaSuTuttiLayer", AttivoTuttoLayer)

        Dim response As New JArray
        Dim entity_response As New JObject

        Dim RisultatoElaborazione = JArray.Parse(payloadObj)

        entity_response.Add("Entita_GUID_1", GuidEntita1)
        entity_response.Add("Entita_GUID_2", GuidEntita2)
        entity_response.Add("Entita_GUID_Risultato", GuidEntitaRisultato)
        entity_response.Add("RisultatoElaborazione", RisultatoElaborazione)

        properties.Add("LayerAnalysisConfig_Response", entity_response)

        feature.Add("properties", properties)

        features.Add(feature)

        geoJsonPolygon.Add("features", features)
        output.Add("geoJsonPolygon", geoJsonPolygon)

        Return output

    End Function

End Class
