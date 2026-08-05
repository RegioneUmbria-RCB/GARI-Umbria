Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.provisioning
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class CaricamentoPiattaformaSat
    Inherits LogProvider
    Implements IAlgoritmoProiezione

    Private _layerAnalysisConfigAlgorithmTypeCod As Int32

    Private _layer1 As ProiezioneLayer
    Private _layer2 As ProiezioneLayer
    Private _layerRisultato As ProiezioneLayer
    Private _layerAnalysisConfigGuid As String

    Private Shared ReadOnly SatHttpClient As New HttpClient()

    Public Sub New(layerAnalysisConfigAlgorithmTypeCod As Int32)
        _layerAnalysisConfigAlgorithmTypeCod = layerAnalysisConfigAlgorithmTypeCod
    End Sub

    Public Overloads Function Esegui(layerAnalysisConfigCod As Int32,
                                     entitaCod1 As Int32,
                                     entitaCod2 As Int32,
                                     entitaCodRisultato As Int32,
                                     esecuzioneCod As Int32,
                                     esecuzioneGuid As String,
                                     parametriEsecuzione As String,
                                     ByRef objParametriServer As AgronicaCoreParametri,
                                     ByRef objParametriUtenti As AgronicaCoreParametri,
                                     ByRef objParametriSuperServer As AgronicaCoreParametri,
                                     Optional overrideTransazione As Boolean = False) As Boolean _
        Implements IAlgoritmoProiezione.Esegui

        Dim resp

        Dim flagTransazioneLocale = False
        Dim flagConnessioneLocale = False

        Try
            If Not overrideTransazione Then
                ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale,
                                                               flagTransazioneLocale,
                                                               objParametriServer)
            End If

            LeggiLayerDaConfigurazioneAlgoritmo(layerAnalysisConfigCod, objParametriServer)
            ValorizzaGuidParametri(layerAnalysisConfigCod, objParametriServer)

'            Dim guidEntita1 = LeggiGuidEntita(entitaCod1, objParametriServer)

            Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

            Dim dataTable = xRead.LeggiDescrizioneElementoDaCodiceEntita(entitaCod1, objParametriServer)

            If dataTable Is Nothing OrElse dataTable.Rows.Count <> 1 Then
                Throw New Exception("Impossibile leggere i dati geografici del poligono.")
            End If

            Dim geoData = dataTable.Rows(0)("GeoData").ToString

            ' load SAT configurations.
            Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim satEndpoint As String = cfgRead.Leggi_Valore_ServerESuperServer(0, CostantiPersonalizzate.SAT_BaseUrl_ConfKey, "" , "",
                                                                      objParametriServer, objParametriSuperServer)
            Dim tenantId = cfgRead.Leggi_Valore(0, CostantiPersonalizzate.SAT_TenantName_ConfKey, "" , "",
                                                         objParametriServer)
            Dim apiKey = cfgRead.Leggi_Valore(0, CostantiPersonalizzate.SAT_ApiKey_ConfKey, "" , "", objParametriServer)
            Dim indexes = cfgRead.Leggi_Valore(0, CostantiPersonalizzate.SAT_Available_Indexes_ConfKey, "" , "",
                                                        objParametriServer)

            Dim maxObservationYears = CInt(cfgRead.Leggi_Valore(0, CostantiPersonalizzate.SAT_MaxObservationWindowYears_ConfKey, "" , "", objParametriServer))

            Dim inputFactory = New ParametriAlgoritmo_Factory
            Dim generatoreInput = inputFactory.
                    CreaParametri(Of Richiesta_Accodamento_GEE)(_layerAnalysisConfigAlgorithmTypeCod)
            generatoreInput.SetupParametriOpzionali(_layerAnalysisConfigGuid, _layer1, _layer2, _layerRisultato)

            Dim collectionNames As JArray

            If _
                _layerAnalysisConfigAlgorithmTypeCod =
                TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE Then
                ' collectionNames must be sourced from the configured "indexes" value (availableIndexesEngine_SAT).
                collectionNames = New JArray
                If Not String.IsNullOrWhiteSpace(indexes) Then
                    For Each collName As String In _
                        indexes.Split(New Char() {","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)
                        Dim trimmed = collName.Trim()
                        If trimmed <> String.Empty Then
                            collectionNames.Add(trimmed)
                        End If
                    Next
                End If

                ' Prepare SAT payload parameters
                Dim geometryWkt As String = geoData
                Dim timeRangeStart As DateTime
                Dim timeRangeEnd As DateTime

                ' Compute time ranges from entity validity
                Dim dtValidita As DataTable
                Dim xReadEntita As New AgronicaCoreGisBIZ.GIS_Entita_R

                If _layer1.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI Then
                    dtValidita = xReadEntita.LeggiValiditaImpiantoDaEntitaCod(entitaCod1, objParametriServer)

                    If dtValidita Is Nothing OrElse dtValidita.Rows.Count <> 1 Then
                        Throw New Exception("Errore nella lettura della validità dell'impianto.")
                    End If

                Else
                    dtValidita = xReadEntita.LeggiValiditaEntitaDaEntitaCod(entitaCod1, objParametriServer)

                    If dtValidita Is Nothing OrElse dtValidita.Rows.Count <> 1 Then
                        Throw New Exception("Errore nella lettura della validità dell'impianto.")
                    End If

                End If

                timeRangeStart = CDate(dtValidita.Rows(0)("Validita_Inizio")).ToUniversalTime()
                timeRangeEnd = CDate(dtValidita.Rows(0)("Validita_Fine")).ToUniversalTime()

                ' Cap timeRangeEnd to maxYears after timeRangeStart
                If timeRangeEnd > timeRangeStart.AddYears(maxObservationYears) Then
                    timeRangeEnd = timeRangeStart.AddYears(maxObservationYears)
                End If

                Dim token = GetUtenteToken(objParametriServer, objParametriUtenti, objParametriSuperServer)
                Dim linkCoreWs = cfgRead.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametriServer)
                Dim httpContextData = New BackgroundAuthenticationModel With {
                        .access_token = token,
                        .corewsbaseurl = linkCoreWs,
                        .iddb = 0
                        }

                Dim httpContextSerialized = JsonConvert.SerializeObject(httpContextData)
                Dim httpContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(httpContextSerialized))

                ' Prepare SAT payload
                Dim satPayload As New JObject From {
                        {"geometry", New JObject From {
                        {"wkt", geometryWkt},
                        {"srid", 4326}
                        }},
                        {"day_from", timeRangeStart.ToString("yyyy-MM-ddTHH:mm:ssZ")},
                        {"day_to", timeRangeEnd.ToString("yyyy-MM-ddTHH:mm:ssZ")},
                        {"indexes", collectionNames},
                        {"http_context", httpContext}
                        }

                ' Send SAT request
                Dim satUrl As String = "https://" & satEndpoint & "/indexes/" & tenantId & "/public/v1/polygons"
                Dim request As New HttpRequestMessage(HttpMethod.Post, satUrl)
                request.Headers.Add("Authorization", "APIKEY " & apiKey)
                Dim content As New StringContent(satPayload.ToString(), Encoding.UTF8, "application/json")
                request.Content = content
                Dim response As HttpResponseMessage = SatHttpClient.SendAsync(request).Result
                If response.IsSuccessStatusCode Then
                    Dim responseJson As JObject = JObject.Parse(response.Content.ReadAsStringAsync().Result)
                    Dim polygonIdSat As String = responseJson("polygon_id").ToString()
                    ' Update GIS_LayerAnalysisConfig_Exec_Log with success
                    Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W
                    xWrite.SalvaGUIDEsecuzione(esecuzioneCod, polygonIdSat, objParametriServer)
                    
                    SalvaGuidEntita(entitaCod1, polygonIdSat, objParametriServer)
                Else
                    Dim satError = response.Content.ReadAsStringAsync().Result
                    Throw _
                        New Exception(
                            String.Format(
                                "Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ",
                                response.StatusCode.ToString,
                                IIf(Not satError Is Nothing, satError, response.ReasonPhrase)))
                End If
            End If

            resp = True

            If Not overrideTransazione Then
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale,
                                                                 objParametriServer)
            End If

        Catch ex As Exception
            If Not objParametriServer.objTransazione Is Nothing And Not overrideTransazione Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If

            Throw
        Finally
            If Not overrideTransazione Then
                ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale,
                                                                 objParametriServer)
            End If
        End Try

        Return resp
    End Function

    Private Function SalvaGuidEntita(entitaCod As Int32,
                                     guidEntita As String,
                                     ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W
        Dim resp = xWrite.SalvaGUIDEntita(entitaCod, guidEntita, objParametriServer)

        If Not resp Then
            Throw New Exception("Impossibile salvare il GUID per l'entità.")
        End If

        Return True
    End Function


    Private Sub ValorizzaGuidParametri(layerAnalysisConfigCod As Int32,
                                       ByRef objParametriServer As AgronicaCoreParametri)
        Dim resp As Boolean

        If _layerAnalysisConfigGuid Is Nothing OrElse _layerAnalysisConfigGuid.Equals("") Then
            _layerAnalysisConfigGuid = Guid.NewGuid().ToString

            Dim xWriteProiezioni As New AgronicaCoreGisBIZ.ProiezioniLayer_W
            resp = xWriteProiezioni.SalvaGUIDConfigurazione(layerAnalysisConfigCod, _layerAnalysisConfigGuid,
                                                            objParametriServer)

            If Not resp Then
                Throw New Exception("Impossibile salvare il GUID per la configurazione.")
            End If
        End If

        VerificaGuidLayer(_layer1, objParametriServer)
        VerificaGuidLayer(_layer2, objParametriServer)
        VerificaGuidLayer(_layerRisultato, objParametriServer)
    End Sub

    Private Sub VerificaGuidLayer(ByRef layer As ProiezioneLayer,
                                  ByRef objParametriServer As AgronicaCoreParametri)

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        If layer.LayerElementiGrafici_GUID Is Nothing OrElse layer.LayerElementiGrafici_GUID.Equals("") Then
            Dim newGuidLayer = Guid.NewGuid.ToString

            Dim resp = xWrite.SalvaGUIDLayer(layer.LayerElementiGrafici_Cod,
                                             layer.TipologiaLayer_cod,
                                             newGuidLayer,
                                             objParametriServer)

            If Not resp Then
                Throw New Exception("Errore nell'aggiornamento del GUID del layer.")
            End If

            layer.LayerElementiGrafici_GUID = newGuidLayer
        End If

        For Each param In layer.Params
            If param.TipologiaLayer_struct_GUID Is Nothing OrElse param.TipologiaLayer_struct_GUID.Equals("") Then
                Dim newGuidParam = Guid.NewGuid.ToString

                Dim resp = xWrite.SalvaGUIDStruct(param.TipologiaLayer_struct_cod, newGuidParam, objParametriServer)

                If Not resp Then
                    Throw New Exception("Errore nell'aggiornamento del GUID del parametro.")
                End If

                param.TipologiaLayer_struct_GUID = newGuidParam
            End If
        Next
    End Sub

    Public Sub LeggiLayerDaConfigurazioneAlgoritmo(layerAnalysisConfigCod As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri) _
        Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo
        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Dim configurazione = xRead.LeggiLayerDaConfigurazione(layerAnalysisConfigCod, objParametri)

        _layer1 = configurazione.Layer1
        _layer2 = configurazione.Layer2
        _layerRisultato = configurazione.LayerRisultato
        _layerAnalysisConfigGuid = configurazione.ConfigurazioneProiezione_GUID
    End Sub

    Public Sub LeggiLayerDaParametriEsecuzione(parametriEsecuzione As String) _
        Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione
        Dim parametriObj As JObject = JObject.Parse(parametriEsecuzione)
        _layer1 = New ProiezioneLayer With {
            .LayerElementiGrafici_Cod = CInt(parametriObj("layerElementiGrafici_Cod"))
            }
    End Sub

    Private Function GetUtenteToken(
                                    ByRef objParametriUtente As AgronicaCoreParametri,
                                    ByRef objParametriServer As AgronicaCoreParametri,
                                    ByRef objParametriSuperServer As AgronicaCoreParametri
                                    ) As String
        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(True,
                                                           True,
                                                           objParametriSuperServer,
                                                           IsolationLevel.ReadUncommitted
                                                           )

            Dim verificaToken = GetExistingToken(objParametriServer, objParametriSuperServer)

            If Not verificaToken Is Nothing And Not verificaToken.Equals("") Then
                Return verificaToken
            End If

            Dim newToken = SalvaNuovoToken(0,
                                           0,
                                           objParametriServer,
                                           objParametriUtente,
                                           objParametriSuperServer)

            SalvaTokenImpostazioniUtenti(newToken,
                                         objParametriServer,
                                         objParametriUtente)

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(True, objParametriSuperServer)
            Return newToken
        Catch ex As Exception
            If Not objParametriSuperServer.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametriSuperServer)
            End If
            Return Nothing
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(True, objParametriSuperServer)
        End Try
    End Function

    Private Function GetExistingToken(ByRef objParametri As AgronicaCoreParametri,
                                      ByRef objParametriSuperServer As AgronicaCoreParametri
                                      ) As String

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Try

            objParametriSuperServer.SuperUserUsername = objParametri.SuperUserUsername

            Dim dt = xRead.LeggiSuperUser("", "", objParametriSuperServer)
            If dt Is Nothing OrElse dt.Rows.Count > 1 Then
                Return Nothing
            ElseIf dt.Rows.Count > 0 Then
                Return dt.Rows(0)("Token_ID").ToString
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function SalvaTokenImpostazioniUtenti(token As String,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  ByRef objParametriUtenti As AgronicaCoreParametri
                                                  )
        Dim dataTable As DataTable
        Dim ok As Boolean

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Try

            dataTable = xRead.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                                    2,
                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametriUtenti)

            If dataTable Is Nothing OrElse dataTable.Rows.Count > 1 Then
                Throw New Exception("Si è verificato un errore nel recupero delle impostazioni utente.")
            End If

            Dim impostazioni As New ImpostazioniAPP

            Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            If dataTable IsNot Nothing AndAlso dataTable.Rows.Count = 1 Then

                impostazioni =
                    JsonConvert.DeserializeObject (Of ImpostazioniAPP)(
                        dataTable.Rows(0)("Impostazione_Valore_1"))

                impostazioni.Token = token

                ok = xWrite.Modifica2(objParametri.SuperUserUsername,
                                      TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                                      JsonConvert.SerializeObject(impostazioni),
                                      "",
                                      "",
                                      "",
                                      CostantiPersonalizzate.AGRODATAINIZIO,
                                      CostantiPersonalizzate.AGRODATAFINE,
                                      objParametriUtenti)
            Else
                impostazioni.Token = token

                ok = xWrite.Scrivi2(objParametri.SuperUserUsername,
                                    TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                                    JsonConvert.SerializeObject(impostazioni),
                                    "",
                                    "",
                                    "",
                                    CostantiPersonalizzate.AGRODATAINIZIO,
                                    CostantiPersonalizzate.AGRODATAFINE,
                                    objParametriUtenti)

            End If
            Return ok

        Catch ex As Exception
            Return ok
        End Try
    End Function

    Private Function SalvaNuovoToken(dBUtenti As Integer,
                                     dBServer As Integer,
                                     objParametri As AgronicaCoreParametri,
                                     objParametriUtenti As AgronicaCoreParametri,
                                     objParametriSuperServer As AgronicaCoreParametri
                                     ) As String

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_Token_W


        Dim token As String

        Dim nomeProvider As String = objParametriSuperServer.StringaConnessione.Split(";")(0).Split("=")(1)
        Dim nomeServer As String = objParametriSuperServer.StringaConnessione.Split(";")(1).Split("=")(1)
        Dim userId As String = objParametriSuperServer.StringaConnessione.Split(";")(3).Split("=")(1)
        Dim pivaSuperUser As String = objParametri.PivaSuperUser

        'recupera ID_DB: Server
        If dBServer = 0 Then
            Dim tipoDbServer As Integer = 1
            Dim nomeDbServer As String = objParametri.StringaConnessione.Split(";")(2).Split("=")(1)

            dBServer = xRead.Leggi_idDB(tipoDbServer, nomeProvider, nomeServer, nomeDbServer,
                                        userId, pivaSuperUser, objParametriSuperServer)
            If dBServer = 0 Then
                Throw New Exception("Errore nel salvataggio del token: ID DB Server mancante")
            End If
        End If

        'recupera ID_DB: Utenti
        If dBUtenti = 0 Then
            Dim tipoDbUtenti = 2
            Dim nomeDbUtenti As String = objParametriUtenti.StringaConnessione.Split(";")(2).Split("=")(1)

            dBUtenti = xRead.Leggi_idDB(tipoDbUtenti, nomeProvider, nomeServer, nomeDbUtenti,
                                        userId, pivaSuperUser, objParametriSuperServer)
            If dBUtenti = 0 Then
                Throw New Exception("Errore nel salvataggio del token: ID DB Utenti mancante")
            End If
        End If

        Dim parametri As New JObject
        parametri.Add("idDB_Server", dBServer.ToString)
        parametri.Add("idDB_Utenti", dBUtenti.ToString)

        token = GenerateRandomToken()

        Dim ok = xWrite.Scrivi(token,
                               objParametri.PivaSuperUser,
                               objParametri.SuperUserUsername,
                               "",
                               objParametri.SuperUserUsername,
                               "",
                               TipiEnumerativi.enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_G2G_API,
                               "",
                               0,
                               parametri.ToString,
                               objParametriSuperServer
                               )

        If Not ok Then
            Throw New Exception("Errore nel salvataggio del token")
        End If

        ok = SalvaScadenzaToken(- 1, objParametri, objParametriSuperServer)

        If Not ok Then
            Throw New Exception("Errore nel salvataggio della validità del token ")
        End If

        Return token
    End Function

    Private Function SalvaScadenzaToken(scadenza As Int32,
                                        objParametri As AgronicaCoreParametri,
                                        objParametriSuperServer As AgronicaCoreParametri
                                        ) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_W

        resp = xWrite.Scrivi(objParametri.SuperUserUsername,
                             "",
                             scadenza,
                             objParametriSuperServer)

        Return resp
    End Function

    Private Shared Function GenerateRandomToken() As String

        Dim g1 = New Byte(25) {}
        Dim gen As RandomNumberGenerator = RandomNumberGenerator.Create()
        gen.GetBytes(g1)

        Dim s As String = Convert.ToBase64String(g1).Replace("+", "A").Replace("/", "B").Replace("=", "C")
        Return s
    End Function
End Class
