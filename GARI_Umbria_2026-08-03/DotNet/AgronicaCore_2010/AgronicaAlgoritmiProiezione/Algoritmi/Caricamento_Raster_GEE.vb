Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis
Imports Google.Apis.Auth.OAuth2
Imports Newtonsoft.Json.Linq

Public Class Caricamento_Raster_GEE
    Inherits AlgoritmoBaseGEE
    Implements IAlgoritmoProiezione

    Public Sub New(ByVal Algoritmo_Cod As Int32,
                   ByVal TipoAlgoritmo_Cod As Int32,
                   ByVal maxFileSize As Long,
                   cfg_siti As JObject)

        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod

        Me.cf_config = New CF_Config With {
            .maxFileSize = maxFileSize,
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
                                     Optional override_transazione As Boolean = False) As Boolean Implements IAlgoritmoProiezione.Esegui

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

            Dim GUID_entita_1 = LeggiGUIDEntita(Entita_cod_1, objParametri_Server)

            Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

            Dim DT = xRead.LeggiDescrizioneElementoDaCodiceEntita(Entita_cod_1, objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
                Throw New Exception("Impossibile leggere i dati geografici del poligono.")
            End If

            Dim geoData = DT.Rows(0)("GeoData").ToString

            Dim geoJson As New JObject

            Dim input_factory = New ParametriAlgoritmo_Factory
            Dim generatore_input = input_factory.CreaParametri(Of Richiesta_Accodamento_GEE)(LayerAnalysisConfig_AlgorithmType_Cod)
            generatore_input.SetupParametriOpzionali(LayerAnalysisConfig_GUID, Layer_1, Layer_2, Layer_Risultato)

            Dim viewNames As New List(Of String)
            viewNames.Add(CostantiPersonalizzate.Google_Cloud_Storage_Default_View)

            Dim paletteVisualizzazione = ""
            If Not DT.Rows(0)("PaletteVisualizzazione").ToString.Equals("") Then
                paletteVisualizzazione = DT.Rows(0)("PaletteVisualizzazione").ToString
            End If

            geoJson = generatore_input.CostruisciRigaDaAccodare(Esecuzione_cod,
                                                                geoData,
                                                                GUID_entita_1, "", "",
                                                                LayerAnalysisConfig_Algorithm_Cod,
                                                                LayerAnalysisConfig_AlgorithmType_Cod,
                                                                AttivoTuttoLayer,
                                                                Esecuzione_GUID,
                                                                objParametri_Server,
                                                                objParametri_Utenti,
                                                                objParametri_Super_Server,
                                                                Entita_cod_1,
                                                                viewNames:=viewNames,
                                                                paletteVisualizzazione:=paletteVisualizzazione)

            Dim leggiAllegato As New AgronicaCoreGisBIZ.GIS_Entita_R

            Dim fullPath = leggiAllegato.LeggiPathAllegatoDaEntita(Entita_cod_1, objParametri_Server)

            Dim fileInfo As New System.IO.FileInfo(fullPath)

            Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

            If fileInfo.Length > cf_config.maxFileSize Then

                resp = xWrite.InserisciLogEsecuzioneAlgoritmo(LayerAnalysisConfig_Cod,
                                                              Esecuzione_cod,
                                                              Entita_cod_1,
                                                              Entita_cod_2,
                                                              Entita_cod_Risultato,
                                                              0,
                                                              CostantiPersonalizzate.AlgoritmiProiezione_File_Raster_Max_Size,
                                                              objParametri_Server,
                                                              GUID_entita_1)

                If Not resp Then
                    Throw New Exception("Errore nell'inserimento del LOG di errore.")
                End If

                If Not override_transazione Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                End If

                If Not override_transazione Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
                End If

                Return False

            End If

            resp = CaricaRasterSuGEE(geoJson, fullPath)

            If Not resp Then
                Throw New Exception("Errore nel caricamento del file su Google Cloud.")
            End If

            Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim DT_Conf = cfgRead.Leggi(0, CostantiPersonalizzate.Google_Cloud_Conf_Key, "", "", objParametri_Server)

            If DT_Conf Is Nothing Then
                Throw New Exception("Chiave di configurazione per Google Cloud non trovata.")
            End If

            Dim confParams = JObject.Parse(DT_Conf.Rows(0)("Valore").ToString)

            Dim uri As String = String.Format("gs://{0}/{1}/{2}/{2}{3}", confParams("bucket").ToString,
                                                                         confParams("obj").ToString,
                                                                         GUID_entita_1,
                                                                         Path.GetExtension(fullPath))

            Dim params As New ParametriVisualizzazioneLayer With {
                .type = TypeVisualizzazioneEntita.CLOUD_STORAGE,
                .baseUrl = confParams("baseUrl").ToString,
                .bucket = confParams("bucket").ToString,
                .obj = confParams("obj").ToString,
                .views = New List(Of String),
                .gsUriFile = uri
            }

            params.views.Add(CostantiPersonalizzate.Google_Cloud_Storage_Default_View)

            resp = xWrite.ValorizzaParametriVisibilitaEntita(Entita_cod_1, params, objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nell'impostazione dei parametri di visualizzazione per l'entità.")
            End If


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

    Private Function CaricaRasterSuGEE(ByVal geoJson As JObject, ByVal fullPath As String) As Boolean

        Dim jsonAuth = Me.cf_config.jsonAuth
        Dim credential As GoogleCredential = GoogleCredential.FromJson(System.Text.RegularExpressions.Regex.Unescape(jsonAuth))
        Dim audience = Me.cf_config.endpoint

        'DEBUG DEBUG DEBUG
        'Dim jsonAuth = System.IO.File.ReadAllText("C:\Files_Test\micheleFurnoKey.json")
        'Dim credential As GoogleCredential = GoogleCredential.FromJson(System.Text.RegularExpressions.Regex.Unescape(jsonAuth))
        'Dim audience = "https://us-central1-ee-simoparmegtest.cloudfunctions.net/OverlayOnRaster-2gen"
        'DEBUG DEBUG DEBUG

        Dim token = credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(audience), CancellationToken.None).Result
        Dim bearer As String = token.GetAccessTokenAsync(CancellationToken.None).Result

        Dim cf_client As New HttpClient()

        Dim content As New MultipartFormDataContent()

        Dim fileContent As New ByteArrayContent(System.IO.File.ReadAllBytes(fullPath))

        Dim mediaType As String = ""

        Select Case System.IO.Path.GetExtension(fullPath).ToLower
            Case ".tif", ".tiff"
                mediaType = "image/tiff"
            Case ".ecw"
                mediaType = "application/octet-stream"
            Case ".jp2"
                mediaType = "image/x-jp2"
            Case Else
                Throw New Exception("Estensione file non accettata.")
        End Select

        fileContent.Headers.ContentType = New Headers.MediaTypeHeaderValue(mediaType)

        Dim jsonContent As New StringContent(geoJson.ToString, Text.Encoding.UTF8, "application/json")

        content.Add(fileContent, "file", System.IO.Path.GetFileName(fullPath))
        content.Add(jsonContent, "GeoData")

        cf_client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", bearer)

        Dim hr As HttpResponseMessage = cf_client.PostAsync(audience, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Caricamento Raster su piattaforma GEE fallito: Status {0}, Messaggio: {1}", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If

        Dim responseBody As String = hr.Content.ReadAsStringAsync().Result

        cf_client.Dispose()

        Return True

    End Function

End Class
