Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.provisioning
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' Gestisce il flusso delle operazioni di fertilizzazione via Engine Mappe Prescrizione.
''' </summary>
Public Class MappePrescrizioneFertilizzazione
    ''' <summary>Shared client for Engine Mappe Prescrizione calls. Static to avoid socket exhaustion.</summary>
    Friend Shared ReadOnly EngineMappeHttpClient As New HttpClient()

    ''' <summary>
    ''' Entry point per il flusso di fertilizzazione via Engine Mappe Prescrizione.
    ''' </summary>
    Public Shared Function GestisciRichiestaMappePrescrizioneFertilizzazione(objPfRateoSrv As PfRateoSrv_In,
                                                                             codiceFiscaleTecnico As String,
                                                                             ByRef objParametriServer As _
                                                                                AgronicaCoreParametri,
                                                                             ByRef objParametriUtenti As _
                                                                                AgronicaCoreParametri,
                                                                             ByRef objParametriSuperServer As _
                                                                                AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim leggiEntita As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim flagTransazioneLocale = False
        Dim flagConnessioneLocale = False

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale, flagTransazioneLocale,
                                                           objParametriServer)

            Dim dtab = leggiEntita.leggiABDaDatiImpianto(objPfRateoSrv.ChiaveAlbero.Piva,
                                                         objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                                         objPfRateoSrv.ChiaveAlbero.Appezza,
                                                         objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                                         objParametriServer)

            If dtab Is Nothing OrElse dtab.Rows.Count = 0 Then
                Throw New Exception("Errore nel recupero delle informazioni del poligono selezionato.")
            End If

            Dim baseRow =
                    dtab.Select(String.Format("LayerElementiGrafici_Cod = {0}",
                                              CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI))).
                    FirstOrDefault

            If baseRow Is Nothing Then
                Throw New Exception("Nessun impianto trovato.")
            End If

            Dim dtEntita = leggiEntita.Leggi(objParametriServer.PivaSuperUser,
                                             0,
                                             0,
                                             objPfRateoSrv.ChiaveAlbero.Piva,
                                             objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                             objPfRateoSrv.ChiaveAlbero.Appezza,
                                             0,
                                             objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                             "",
                                             "",
                                             "-1",
                                             - 1,
                                             - 1,
                                             "-1",
                                             0,
                                             0,
                                             0,
                                             0,
                                             codiceFiscaleTecnico,
                                             "",
                                             Nothing,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             String.Format(
                                                 " Entita.TipoEntita_Cod in (select tipoEntita_cod from gis_tipoentita where layerElementiGrafici_cod = {0} )",
                                                 CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI).
                                                              ToString),
                                             "",
                                             objParametriServer,
                                             objParametriUtenti)

            If dtEntita Is Nothing OrElse dtEntita.Rows.Count = 0 Then
                Throw _
                    New Exception(
                        "IMPOSSBILE PROCEDERE: Non esiste la cartografia per l'impianto associato alla ricetta.")
            End If

            Dim qtaFertilizzante As Double = 120
            Dim ricettaDate = DateTime.UtcNow
            Dim leggiDettagli As New AgronicaCoreContabDAL.Ricette_Dettagli_R
            Dim dtleggiDettagli As DataTable =
                    leggiDettagli.Leggi(
                        0,
                        objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod,
                        0,
                        "",
                        0,
                        3,
                        0,
                        0,
                        CostantiPersonalizzate.AGRODATAINIZIO,
                        CostantiPersonalizzate.AGRODATAFINE,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametriServer,
                        joinRicette:=True
                        )

            If dtleggiDettagli.Rows.Count > 0 Then
                qtaFertilizzante = dtleggiDettagli.Rows(0)("qta")
                ricettaDate = CDate(dtleggiDettagli.Rows(0)("DataOperazione"))
            End If

            Dim fmisContext = CreaFmisContext(objParametriServer, objParametriUtenti, objParametriSuperServer)
            Dim engineInput = CreaInputPerEngineMappePrescrizione(baseRow, qtaFertilizzante, ricettaDate)
            Dim requestId = AccodaRichiestaEngineMappePrescrizione(engineInput, fmisContext, objParametriServer, objParametriSuperServer)

            Dim scriviRichiesta As New AgronicaCoreGisDAL.RichiesteEngine_Impianto_Ricetta_W
            scriviRichiesta.Scrivi(
                requestId,
                objPfRateoSrv.ChiaveAlbero.Piva,
                objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                objPfRateoSrv.ChiaveAlbero.Appezza,
                objPfRateoSrv.ChiaveAlbero.Id_Imp,
                objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod,
                objParametriServer)

            If objParametriServer.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametriServer)
            End If

            resp = True

        Catch ex As Exception
            If Not objParametriServer.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If

            Throw
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametriServer)
        End Try

        Return resp
    End Function

    ''' <summary>
    ''' Builds the fertilization execution request payload for Engine Mappe Prescrizione.
    ''' </summary>
    Private Shared Function CreaInputPerEngineMappePrescrizione(baseRow As DataRow,
                                                                qtaFertilizzante As Double,
                                                                dataRicetta As DateTime
                                                                ) As JObject
        Dim output As New JObject
        output.Add("geometry", baseRow("geoData").ToString())
        output.Add("base_fertilization_dosage", qtaFertilizzante)
        output.Add("srid", 4326)
        output.Add("evaluate_at", dataRicetta.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
        Return output
    End Function

    ''' <summary>
    ''' Builds the Base64-encoded fmis_context value used for webhook authentication.
    ''' </summary>
    Friend Shared Function CreaFmisContext(ByRef objParametriServer As AgronicaCoreParametri,
                                            ByRef objParametriUtenti As AgronicaCoreParametri,
                                            ByRef objParametriSuperServer As AgronicaCoreParametri) As String
        Try
            Dim confReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim linkCoreWs = confReader.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametriServer)

            Dim token = GetUtenteToken(objParametriServer, objParametriUtenti, objParametriSuperServer)
            Dim fmisContextData = New BackgroundAuthenticationModel With {
                    .access_token = token,
                    .corewsbaseurl = linkCoreWs,
                    .iddb = 0
                    }

            Dim fmisContextSerialized = JsonConvert.SerializeObject(fmisContextData)
            Dim fmisContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(fmisContextSerialized))
            Return fmisContext
        Catch
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' POSTs the fertilization execution request to Engine Mappe Prescrizione.
    ''' Returns the request_id from the engine response
    ''' Retries up to 3 times with exponential backoff (5 s, 10 s, 20 s) on transient failures.
    ''' Throws if URL, tenant, API key or model code are not configured.
    ''' </summary>
    Private Shared Function AccodaRichiestaEngineMappePrescrizione(engineInput As JObject,
                                                                   fmisContext As String,
                                                                   ByRef objParametriServer As AgronicaCoreParametri,
                                                                   ByRef objParametriSuperServer As AgronicaCoreParametri) _
        As String
        Dim confReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim urlEngine = confReader.Leggi_Valore_ServerESuperServer(0, CostantiPersonalizzate.MappePrescrizione_BaseUrl_ConfKey, "", "",
                                                objParametriServer, objParametriSuperServer)
        If String.IsNullOrEmpty(urlEngine) Then
            Throw New Exception("MissingConfigurationException: URL Engine Mappe Prescrizione non configurato.")
        End If

        Dim tenant = confReader.Leggi_Valore(0, CostantiPersonalizzate.MappePrescrizione_TenantName_ConfKey, "", "",
                                             objParametriServer)
        If String.IsNullOrEmpty(tenant) Then
            Throw New Exception("MissingConfigurationException: Tenant Engine Mappe Prescrizione non configurato.")
        End If

        Dim apiKey = confReader.Leggi_Valore(0, CostantiPersonalizzate.MappePrescrizione_ApiKey_ConfKey, "", "",
                                             objParametriServer)
        If String.IsNullOrEmpty(apiKey) Then
            Throw New Exception("MissingConfigurationException: API Key Engine Mappe Prescrizione non configurata.")
        End If

        Dim modelCode = confReader.Leggi_Valore(0, CostantiPersonalizzate.MappePrescrizione_ModelCode_ConfKey, "", "",
                                                objParametriServer)
        If String.IsNullOrEmpty(modelCode) Then
            Throw New Exception("MissingConfigurationException: Model code Engine Mappe Prescrizione non configurato.")
        End If

        Dim endpoint = String.Format("https://{0}/mappe-prescrizione/{1}/public/v1/models/{2}/executionrequest",
                                     urlEngine, tenant, modelCode)

        Return InviaRichiestaConRetry(endpoint, apiKey, fmisContext, engineInput)
    End Function

    ''' <summary>
    ''' Sends the HTTP POST to the Engine endpoint with exponential-backoff retry.
    ''' Only retries on transient failures (network exception or HTTP 5xx / 429);
    ''' non-retryable error codes (4xx except 429) throw immediately.
    ''' </summary>
    Friend Shared Function InviaRichiestaConRetry(endpoint As String,
                                                   apiKey As String,
                                                   fmisContext As String,
                                                   engineInput As JObject) As String
        Const maxTentativi = 3
        ' Backoff delays in seconds: attempt 1→5 s, attempt 2→10 s, attempt 3→20 s
        Dim backoffSeconds As Integer() = {5, 10, 20}

        Dim lastException As Exception = Nothing
        Dim lastStatusCode As String

        For tentativo = 0 To maxTentativi - 1
            Try
                ' HttpRequestMessage cannot be reused after send — rebuild each attempt
                Dim request As New HttpRequestMessage(HttpMethod.Post, endpoint)
                request.Headers.Add("Authorization", "APIKEY " & apiKey)
                request.Headers.Add("fmis-context", fmisContext)
                request.Content = New StringContent(engineInput.ToString(), Encoding.UTF8, "application/json")

                Dim response As HttpResponseMessage = EngineMappeHttpClient.SendAsync(request).Result

                ' Success
                If response.IsSuccessStatusCode Then
                    Dim responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                    Dim responseJson = JObject.Parse(responseBody)
                    Return responseJson.Value (Of String)("requestId")
                End If

                lastStatusCode = response.StatusCode.ToString()

                ' Non-retryable client error (4xx except 429 Too Many Requests)
                Dim statusCode = CInt(response.StatusCode)
                If statusCode >= 400 AndAlso statusCode < 500 AndAlso statusCode <> 429 Then
                    Throw _
                        New Exception(
                            String.Format(
                                "Accodamento esecuzione su Engine Mappe Prescrizione fallito (non-retryable): status {0}, Messaggio: {1}",
                                lastStatusCode, response.ReasonPhrase))
                End If

                ' Retryable: 5xx or 429 — fall through to backoff
                lastException = New Exception(String.Format("Engine Mappe Prescrizione ha risposto con status {0}",
                                                            lastStatusCode))

            Catch ex As Exception
                lastException = ex
            End Try

            ' Backoff before next attempt (skip after last attempt)
            If tentativo < maxTentativi - 1 Then
                Thread.Sleep(TimeSpan.FromSeconds(backoffSeconds(tentativo)))
            End If
        Next

        Throw _
            New Exception(
                String.Format(
                    "Accodamento esecuzione su Engine Mappe Prescrizione fallito dopo {0} tentativi. Ultimo errore: {1}",
                    maxTentativi, lastException?.Message), lastException)
    End Function

    Private Shared Function GetUtenteToken(
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

    Private Shared Function GetExistingToken(ByRef objParametri As AgronicaCoreParametri,
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

    Private Shared Function SalvaTokenImpostazioniUtenti(token As String,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         ByRef objParametriUtenti As AgronicaCoreParametri
                                                         )
        Dim dataTable As DataTable
        Dim ok As Boolean

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Try

            dataTable = xRead.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                                    2,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
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

    Private Shared Function SalvaNuovoToken(dBUtenti As Integer,
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

    Private Shared Function SalvaScadenzaToken(scadenza As Int32,
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
