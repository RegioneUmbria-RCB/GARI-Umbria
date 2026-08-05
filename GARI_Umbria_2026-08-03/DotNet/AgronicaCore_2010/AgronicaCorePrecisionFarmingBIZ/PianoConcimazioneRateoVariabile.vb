Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreModelsSTD.provisioning
Imports AgronicaCoreVarieBIZ
Imports Google.Apis.Auth.OAuth2
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class PianoConcimazioneRateoVariabile_R

End Class
Public Class PianoConcimazioneRateoVariabile_W

    Public Function ElaboraRateoConWSMappe(ByVal objPfRateoSrv As PfRateoSrv_In,
                                       ByVal codice_Fiscale_Tecnico As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                       ByRef objParametri_Super_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim leggiEntita As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim DT As DataTable

            DT = leggiEntita.leggiABDaDatiImpianto(objPfRateoSrv.ChiaveAlbero.Piva,
                                                   objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                                   objPfRateoSrv.ChiaveAlbero.Appezza,
                                                   objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                                   objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Errore nel recupero delle informazioni del poligono selezionato")
            End If

            Dim baseRow = DT.Select(String.Format("LayerElementiGrafici_Cod = {0}", CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI))).FirstOrDefault
            Dim ARow = DT.Select("ElementoGrafico_Des = 'A'").FirstOrDefault
            Dim BRow = DT.Select("ElementoGrafico_Des = 'B'").FirstOrDefault

            If baseRow Is Nothing Then
                Throw New Exception("Nessun impianto trovato.")
            End If

            Dim DTEntita = leggiEntita.Leggi(objParametri_Server.PivaSuperUser,
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
                                             -1,
                                             -1,
                                             "-1",
                                             0,
                                             0,
                                             0,
                                             0,
                                             codice_Fiscale_Tecnico,
                                             "",
                                             Nothing,
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             String.Format(" Entita.TipoEntita_Cod in (select tipoEntita_cod from gis_tipoentita where layerElementiGrafici_cod = {0} )", baseRow("LayerElementiGrafici_Cod").ToString),
                                             "",
                                             objParametri_Server,
                                             objParametri_Utenti)

            If DTEntita Is Nothing OrElse DTEntita.Rows.Count = 0 Then
                Throw New Exception("IMPOSSBILE PROCEDERE: Non esiste la cartografia per l'impianto associato alla ricetta.")
            End If

            Dim qtaFertilizzante As Double = 120
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
                    objParametri_Server
                )

            If dtleggiDettagli.Rows.Count > 0 Then
                qtaFertilizzante = dtleggiDettagli.Rows(0)("qta")
            End If

            Dim chiaveAlbero = Converti_ChiaveAlbero_ToString(objPfRateoSrv.ChiaveAlbero)

            If Debugger.IsAttached Then
                ServicePointManager.ServerCertificateValidationCallback = Function(s, c, h, e) True
            End If

            'richiamo ws_mappe_2024
            '0- richiamo calendario alla data per recuperare id del geotiff da utilizzare per la mappa

            Dim req As New JObject

            req = createRequestGetImageTiffPerWSMappe2024(baseRow,
                                                          "NDVI",
                                                          "3",
                                                          objPfRateoSrv.DataRiferimento_LetturaDatiSentinel)

            Dim resImg = GetResultImageTiffPerWSMappe2024(req,
                                                          "/Maps/CustomMapOverlayBaseInizializzaCalendario",
                                                          objParametri_Server)

            '1- richiamo generazione griglia che ritorna il geojson da salvare negli allegati

            req = New JObject

            req = createRequestGetMapWSMappe2024(baseRow,
                                                 resImg.Last().Passaggi.Last().Url,
                                                 objPfRateoSrv.CellSize,
                                                 4326)

            getResultPrescriptionMap(req,
                                     "/GdalData/ProduceGrid",
                                     objPfRateoSrv,
                                     objParametri_Server)

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

            resp = True

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp
    End Function

    Private Function createRequestGetImageTiffPerWSMappe2024(ByVal baseRow As DataRow,
                                                             ByVal sensor As String,
                                                             ByVal source As String,
                                                             ByVal [date] As DateTime
                                                            ) As JObject
        Dim output As New JObject

        output.Add("wkt", baseRow("geoData").ToString)
        output.Add("sensor", sensor)
        output.Add("source", source)
        output.Add("startDate", ([date].AddMonths(-6)).ToString("yyyy-MM-ddTHH:mm:ssZ"))
        output.Add("endDate", [date].ToString("yyyy-MM-ddTHH:mm:ssZ"))

        Return output

    End Function

    Private Function GetResultImageTiffPerWSMappe2024(ByVal input As JObject,
                                                      ByVal endpoint As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri) As List(Of GisSatSentinelOverlayModel)

        Dim ret As List(Of GisSatSentinelOverlayModel) = Nothing

        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim confObj = JsonConvert.DeserializeObject(Of WS_Mappe_Config)(LeggiConfString.Leggi_Valore(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server))

        Dim cf_client As New HttpClient()
        cf_client.DefaultRequestHeaders.Add(confObj.auth_Key.type, confObj.auth_Key.value)

        Dim content As New StringContent(input.ToString, Text.Encoding.UTF8, "application/json")

        Dim hr As HttpResponseMessage = cf_client.PostAsync(confObj.baseUrl + endpoint, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If
        cf_client.Dispose()

        Return JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of GisSatSentinelOverlayModel)))(hr.Content.ReadAsStringAsync().GetAwaiter().GetResult()).RispostaStringa
    End Function

    Private Function createRequestGetMapWSMappe2024(ByVal baseRow As DataRow,
                                                             ByVal pathImg As String,
                                                             ByVal squareSide As Integer,
                                                             ByVal srid As Integer
                                                            ) As JObject
        Dim output As New JObject

        output.Add("wkt", baseRow("geoData").ToString)
        output.Add("path", pathImg)
        output.Add("squareSide", squareSide.ToString())
        output.Add("srid", srid)

        Return output

    End Function

    Private Function getResultPrescriptionMap(ByVal input As JObject,
                                              ByVal endpoint As String,
                                              ByVal objPfRateoSrv As PfRateoSrv_In,
                                              ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = True
        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim confObj = JsonConvert.DeserializeObject(Of WS_Mappe_Config)(LeggiConfString.Leggi_Valore(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server))

        Dim cf_client As New HttpClient()
        cf_client.DefaultRequestHeaders.Add(confObj.auth_Key.type, confObj.auth_Key.value)

        Dim content As New StringContent(input.ToString, Text.Encoding.UTF8, "application/json")

        Dim hr As HttpResponseMessage = cf_client.PostAsync(confObj.baseUrl + endpoint, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If
        Dim resp As New RispostaStandard
        Dim res = JsonConvert.DeserializeObject(Of GisDataReadRvalModel(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))(hr.Content.ReadAsStringAsync().GetAwaiter().GetResult())
        If res.Messages.Equals("") Then
            Dim OUTPUT_Allegati_Documenti_Cod As Int32

            Dim ScriviSuAllegati As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W

            resp = ScriviSuAllegati.PrecisionFarmingScriviSuAllegati(TipiEnumerativi.enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione,
                                                                     objPfRateoSrv.DescrizionePiano,
                                                                     "",
                                                                     objParametri_Server,
                                                                     objPfRateoSrv.ChiaveAlbero.Piva,
                                                                     objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                                                     objPfRateoSrv.ChiaveAlbero.Appezza,
                                                                     objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod,
                                                                     objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                                                     "",
                                                                     Nothing,
                                                                     System.Text.RegularExpressions.Regex.Unescape(JsonConvert.SerializeObject(res.MyGeoJson)).Replace("###", "§"),
                                                                     OUTPUT_Allegati_Documenti_Cod
                                                                    )

            If Not resp.RispostaOK Then
                Return False
            End If

        End If
        cf_client.Dispose()

        Return ret

    End Function

    Private Function creaInputPerWSMappe2024(ByVal ARow As DataRow,
                                             ByVal BRow As DataRow,
                                             ByVal baseRow As DataRow,
                                             ByVal chiaveAlbero As String,
                                             ByVal PfRateoSrv_In As PfRateoSrv_In,
                                             ByVal qtaFertilizzante As Double,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri,
                                             ByRef objParametri_Super_Server As AgronicaCoreParametri) As JObject
        Dim output As New JObject

        output.Add("geoJsonABLine", CreaSezioneAB(ARow, BRow, objParametri_Server))

        output.Add("geoJsonPolygon", CreaSezioneImpianto(baseRow, chiaveAlbero, PfRateoSrv_In, objParametri_Server, objParametri_Utenti, objParametri_Super_Server))

        output.Add("precisionFarmingData", CreaSezionePrecisionFarming(qtaFertilizzante, CInt(PfRateoSrv_In.CellSize)))

        Return output
    End Function

    Private Function RichiamaWSMappe2024(ByVal WsMappeInput As JObject, ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = True

        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim confObj = JsonConvert.DeserializeObject(Of WS_Mappe_Config)(LeggiConfString.Leggi_Valore(0, CostantiPersonalizzate.WS_Mappe_ConfKey, "", "", objParametri_Server))

        Dim cf_client As New HttpClient()

        Dim content As New StringContent(WsMappeInput.ToString, Text.Encoding.UTF8, "application/json")

        cf_client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue(confObj.auth_Key.type, confObj.auth_Key.value)

        Dim hr As HttpResponseMessage = cf_client.PostAsync(confObj.baseUrl + "\Maps\PfRateoSrv", content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If

        'gestire il ritorno
        Dim resp As New RispostaStandard
        Dim res = JsonConvert.DeserializeObject(Of PianoConcimazioneGEE)(hr.Content.ReadAsStringAsync().GetAwaiter().GetResult())

        If res.Messaggi.Equals("") Then
            Dim objPfRateoSrv = Newtonsoft.Json.JsonConvert.DeserializeObject(Of PfRateoSrv_In)(res.PfRateoSrv_In)

            Dim OUTPUT_Allegati_Documenti_Cod As Int32

            Dim ScriviSuAllegati As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W

            resp = ScriviSuAllegati.PrecisionFarmingScriviSuAllegati(TipiEnumerativi.enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione,
                                                                     objPfRateoSrv.DescrizionePiano,
                                                                     "",
                                                                     objParametri_Server,
                                                                     objPfRateoSrv.ChiaveAlbero.Piva,
                                                                     objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                                                     objPfRateoSrv.ChiaveAlbero.Appezza,
                                                                     objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod,
                                                                     objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                                                     "",
                                                                     Nothing,
                                                                     System.Text.RegularExpressions.Regex.Unescape(res.Risultato_Elaborazione).Replace("###", "§"),
                                                                     OUTPUT_Allegati_Documenti_Cod
                                                                    )

            If Not resp.RispostaOK Then
                Return ret = False
            End If

            'resp.RispostaOK = messaggistica.AccodaMessaggioEsecuzione(res.UtenteRichiedente, "Piano di concimazione calcolato con successo.", objParametri_Server)

            'If Not resp.RispostaOK Then
            '    resp.RispostaStringa = ""
            '    resp.Errore = "Errore nell'accodamento del messaggio"
            'End If
        Else
            'resp.RispostaOK = messaggistica.AccodaMessaggioEsecuzione(res.UtenteRichiedente, res.Messaggi, objParametri_Server)

            'If resp.RispostaOK Then
            '    resp.RispostaStringa = "Messaggio di errore accodato correttamente.."
            'Else
            '    resp.Errore = "Errore nell'accodamento del messaggio di errore"
            'End If
        End If

        cf_client.Dispose()

        Return ret

    End Function


    Public Function ElaboraRateoConGEE(ByVal objPfRateoSrv As PfRateoSrv_In,
                                       ByVal codice_Fiscale_Tecnico As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                       ByRef objParametri_Super_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim leggiEntita As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim DT As DataTable

            DT = leggiEntita.leggiABDaDatiImpianto(objPfRateoSrv.ChiaveAlbero.Piva,
                                                   objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                                   objPfRateoSrv.ChiaveAlbero.Appezza,
                                                   objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                                   objParametri_Server)

            If DT Is Nothing OrElse DT.Rows.Count = 0 Then
                Throw New Exception("Errore nel recupero delle informazioni del poligono selezionato")
            End If

            Dim baseRow = DT.Select(String.Format("LayerElementiGrafici_Cod = {0}", CInt(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI))).FirstOrDefault
            Dim ARow = DT.Select("ElementoGrafico_Des = 'A'").FirstOrDefault
            Dim BRow = DT.Select("ElementoGrafico_Des = 'B'").FirstOrDefault

            If baseRow Is Nothing Then
                Throw New Exception("Nessun impianto trovato.")
            End If

            Dim DTEntita = leggiEntita.Leggi(objParametri_Server.PivaSuperUser,
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
                                             -1,
                                             -1,
                                             "-1",
                                             0,
                                             0,
                                             0,
                                             0,
                                             codice_Fiscale_Tecnico,
                                             "",
                                             Nothing,
                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             String.Format(" Entita.TipoEntita_Cod in (select tipoEntita_cod from gis_tipoentita where layerElementiGrafici_cod = {0} )", baseRow("LayerElementiGrafici_Cod").ToString),
                                             "",
                                             objParametri_Server,
                                             objParametri_Utenti)

            If DTEntita Is Nothing OrElse DTEntita.Rows.Count = 0 Then
                Throw New Exception("IMPOSSBILE PROCEDERE: Non esiste la cartografia per l'impianto associato alla ricetta.")
            End If

            Dim qtaFertilizzante As Double = 120
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
                    objParametri_Server
                )

            If dtleggiDettagli.Rows.Count > 0 Then
                qtaFertilizzante = dtleggiDettagli.Rows(0)("qta")
            End If

            Dim chiaveAlbero = Converti_ChiaveAlbero_ToString(objPfRateoSrv.ChiaveAlbero)

            Dim GEEInput As New JObject

            GEEInput = creaInputPerGEE(ARow,
                                       BRow,
                                       baseRow,
                                       chiaveAlbero,
                                       objPfRateoSrv,
                                       qtaFertilizzante,
                                       objParametri_Server,
                                       objParametri_Utenti,
                                       objParametri_Super_Server)

            AccodaRichiesta(GEEInput, objParametri_Server)

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

            resp = True

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp
    End Function

    ''' <summary>
    ''' Determina il tipo di operazione collegata alla ricetta e delega al flusso corretto.
    ''' </summary>
    Public Function ElaboraRateoConEngineMappePrescrizione(objPfRateoSrv As PfRateoSrv_In,
                                       codiceFiscaleTecnico As String,
                                       ByRef objParametriServer As AgronicaCoreParametri,
                                       ByRef objParametriUtenti As AgronicaCoreParametri,
                                       ByRef objParametriSuperServer As AgronicaCoreParametri) As Boolean
        Dim leggiRicetta As New AgronicaCoreContabDAL.Ricette_Operazioni_R
        Dim lavCod = leggiRicetta.LeggiLavCodFromRicettaOperazioneCod(CInt(objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod), objParametriServer)

        If lavCod = CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_CONCIME Then
            Return MappePrescrizioneFertilizzazione.GestisciRichiestaMappePrescrizioneFertilizzazione(objPfRateoSrv, codiceFiscaleTecnico, objParametriServer, objParametriUtenti, objParametriSuperServer)
        ElseIf lavCod = CostantiPersonalizzate.LAVCOD_SEMINA Then
            Return MappePrescrizioneSemina.GestisciRichiestaMappePrescrizioneSemina(objPfRateoSrv, codiceFiscaleTecnico, objParametriServer, objParametriUtenti, objParametriSuperServer)
        Else
            Throw New Exception(String.Format("Tipo operazione non supportato per mappe di prescrizione: Lav_Cod '{0}'.", lavCod))
        End If

    End Function

    Private Function AccodaRichiesta(ByVal gEEInput As JObject, ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim confString As String =
            LeggiConfString.Leggi_Valore(0, "Configurazioni_GoogleEarthEngine", "", "", objParametri_Server)

        Dim confObj = JObject.Parse(confString)

        Dim audienceBaseUrl = confObj("BaseUrl_GEE").ToString

        Dim jsonAuth = confObj("JSONAuth").ToString
        Dim credential As GoogleCredential = GoogleCredential.FromJson(System.Text.RegularExpressions.Regex.Unescape(jsonAuth))
        Dim audience = String.Format("{0}/{1}", audienceBaseUrl, confObj("SubmitGISAlgorithm").ToString)

        'DEBUG DEBUG DEBUG
        'Dim jsonAuth = System.IO.File.ReadAllText("C:\Files_Test\micheleFurnoKey.json")
        'Dim credential As GoogleCredential = GoogleCredential.FromJson(System.Text.RegularExpressions.Regex.Unescape(jsonAuth))
        'Dim audience = "https://europe-west1-ee-simoparmegtest.cloudfunctions.net/SubmitGISAlgorithm"
        'DEBUG DEBUG DEBUG

        Dim token = credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(audience), CancellationToken.None).Result
        Dim bearer As String = token.GetAccessTokenAsync(CancellationToken.None).Result

        Dim cf_client As New HttpClient()

        Dim content As New StringContent(gEEInput.ToString, Text.Encoding.UTF8, "application/json")

        cf_client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", bearer)

        Dim hr As HttpResponseMessage = cf_client.PostAsync(audience, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception(String.Format("Accodamento esecuzione su piattaforma GEE fallito: status {0}, Messaggio: {1} ", hr.StatusCode.ToString, hr.ReasonPhrase))
        End If

        cf_client.Dispose()

        Return True

    End Function

    Private Function creaInputPerGEE(ByVal ARow As DataRow,
                                     ByVal BRow As DataRow,
                                     ByVal baseRow As DataRow,
                                     ByVal chiaveAlbero As String,
                                     ByVal PfRateoSrv_In As PfRateoSrv_In,
                                     ByVal qtaFertilizzante As Double,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     ByRef objParametri_Super_Server As AgronicaCoreParametri) As JObject
        Dim output As New JObject

        output.Add("geoJsonABLine", CreaSezioneAB(ARow, BRow, objParametri_Server))

        output.Add("geoJsonPolygon", CreaSezioneImpianto(baseRow, chiaveAlbero, PfRateoSrv_In, objParametri_Server, objParametri_Utenti, objParametri_Super_Server))

        output.Add("precisionFarmingData", CreaSezionePrecisionFarming(qtaFertilizzante, CInt(PfRateoSrv_In.CellSize)))

        Return output
    End Function

    Private Function CreaSezionePrecisionFarming(ByVal qtaFertilizzante As Double, ByVal scale As Int32) As JObject
        Dim precisionFarmingData As New JObject

        'mettere in costante
        precisionFarmingData.Add("attributeList", CostantiPersonalizzate.Precision_Farming_GEE_AttributeList)
        precisionFarmingData.Add("attributeValues", String.Format("{0},{0}", qtaFertilizzante.ToString.Replace(",", ".")))
        precisionFarmingData.Add("scale", scale)

        Return precisionFarmingData
    End Function

    Private Function CreaSezioneImpianto(ByVal baseRow As DataRow,
                                         ByVal chiaveAlbero As String,
                                         ByVal PfRateoSrv_In As PfRateoSrv_In,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                         ByRef objParametri_Super_Server As AgronicaCoreParametri) As JObject

        Dim geoJsonPolygon As New JObject

        geoJsonPolygon.Add("type", "FeatureCollection")

        Dim features As New JArray

        Dim feature As New JObject

        feature.Add("type", "Feature")

        Dim geometry As New JObject

        geometry.Add("type", "Polygon")

        Dim coordinates As New JArray

        geometry.Add("coordinates", JArray.FromObject(getStringGeometryGeoJson(baseRow("geoData").ToString).coordinates))

        feature.Add("geometry", geometry)

        Dim propertyBase As New JObject

        propertyBase.Add("Product", CostantiPersonalizzate.Precision_Farming_GEE_Product)
        propertyBase.Add("StartDate", AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(PfRateoSrv_In.DataRiferimento_LetturaDatiSentinel))
        propertyBase.Add("EndDate", AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(PfRateoSrv_In.DataRiferimento_LetturaDatiSentinel))
        propertyBase.Add("layer", Convert.ToString(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione))
        propertyBase.Add("StandardEntita_layerDiAppartenenza", Convert.ToString(TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Mappe_Prescrizione))
        propertyBase.Add("StandardEntita_layerDiAppartenenza_Des", CostantiPersonalizzate.Precision_Farming_GEE_Layer)
        propertyBase.Add("StandardEntita_layerDiAppartenenza_Icona32", CostantiPersonalizzate.Precision_Farming_GEE_Layer_Icon)
        propertyBase.Add("PivaSuperUser", objParametri_Server.PivaSuperUser)
        propertyBase.Add("chiavealbero", chiaveAlbero.Replace("§", "###"))
        propertyBase.Add("featureGUID", RicavaGUIDEntita(baseRow, objParametri_Server))
        propertyBase.Add("Index", CostantiPersonalizzate.Precision_Farming_GEE_Index)

        Dim LeggiCFGCallback As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim Installation As String =
            LeggiCFGCallback.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)

        If Installation.EndsWith("/") Then
            Installation = Installation.Substring(0, Installation.Length - 2)
        End If

        Dim CallBackBaseUrl As String =
            LeggiCFGCallback.Leggi_Valore(0, "GiasOnline_Core_API", "", "", objParametri_Server)

        If CallBackBaseUrl.StartsWith("/") Then
            CallBackBaseUrl = CallBackBaseUrl.Substring(1)
        End If

        propertyBase.Add("CallbackURL", String.Format("{0}/{1}/{2}", Installation, CallBackBaseUrl, CostantiPersonalizzate.CallBack_PianoConcimazione_GEE))

        propertyBase.Add("LayerAnalysisConfig_GUID", CInt(TipiEnumerativi.enum_AlgoritmoProiezione.CalcoloPianoConcimazione).ToString)
        propertyBase.Add("LayerAnalysisConfig_Algorithm_Cod", CInt(TipiEnumerativi.enum_AlgoritmoProiezione.CalcoloPianoConcimazione))
        propertyBase.Add("GIS_LayerAnalysisConfig_Exec_Log_GUID", Guid.NewGuid().ToString)

        propertyBase.Add("PfRateoSrv_In", Newtonsoft.Json.JsonConvert.SerializeObject(PfRateoSrv_In))
        propertyBase.Add("idDB", RecuperaIDDB(objParametri_Server, objParametri_Utenti, objParametri_Super_Server))
        propertyBase.Add("UtenteRichiedente", objParametri_Utenti.UtenteUsername)

        feature.Add("properties", propertyBase)

        features.Add(feature)

        geoJsonPolygon.Add("features", features)

        Return geoJsonPolygon
    End Function

    Private Function CreaSezioneAB(ByVal ARow As DataRow,
                                   ByVal BRow As DataRow,
                                   ByRef objParametri_Server As AgronicaCoreParametri) As JObject

        Dim ABSection As New JObject

        If ARow Is Nothing Or BRow Is Nothing Then
            ABSection = Nothing
        Else
            ABSection.Add("type", "FeatureCollection")

            Dim featuresAB As New JArray

            Dim featureAB As New JObject
            featureAB.Add("type", "Feature")

            Dim geometryAB As New JObject
            geometryAB.Add("type", "LineString")

            Dim coordArray As New JArray

            coordArray.Add(getStringGeometryGeoJson(ARow("geoData").ToString).coordinates)
            coordArray.Add(getStringGeometryGeoJson(BRow("geoData").ToString).coordinates)

            geometryAB.Add("coordinates", coordArray)

            featureAB.Add("geometry", geometryAB)

            Dim propertyAB As New JObject

            propertyAB.Add("APosition", "0")
            propertyAB.Add("featureGUID", RicavaGUIDEntita(ARow, objParametri_Server))

            featureAB.Add("properties", propertyAB)

            featuresAB.Add(featureAB)

            ABSection.Add("features", featuresAB)
        End If

        Return ABSection
    End Function

    Private Function RicavaGUIDEntita(ByVal entitaRow As DataRow,
                                      ByRef objParametri_Server As AgronicaCoreParametri) As String

        If Not entitaRow("Entita_GUID").ToString.Equals("") Then
            Return entitaRow("Entita_GUID").ToString
        End If

        Dim newGUIDEntita = Guid.NewGuid().ToString

        Dim xWrite As New AgronicaCoreGisDAL.GIS_Entita_W

        Dim resp = xWrite.SalvaGUIDEntita(CInt(entitaRow("Entita_Cod")), newGUIDEntita, objParametri_Server)

        If Not resp Then
            Throw New Exception("Impossibile salvare il GUID per l'entità.")
        End If

        Return newGUIDEntita

    End Function

    Private Function getStringGeometryGeoJson(ByVal elemento As String) As AgronicaCoreModelsSTD.Gis.GeoJson_Geometry_New

        Dim s As New SqlSpatialConverter(Of WktParser)(elemento)
        Return s.ToGeoJson

    End Function

    Private Function Converti_ChiaveAlbero_ToString(ByVal ChiaveAlbero As ChiaveAlbero) As String

        Dim ChiaveAlbero_ToString As String = ChiaveAlbero.TipoNodo & "§" &
                                              ChiaveAlbero.Piva & "§" &
                                              ChiaveAlbero.Sa_Cod & "§" &
                                              ChiaveAlbero.Campo_Cod & "§" &
                                              ChiaveAlbero.Appezza & "§" &
                                              ChiaveAlbero.Id_Imp & "§" &
                                              ChiaveAlbero.p_Part_Cod & "§" &
                                              ChiaveAlbero.p_Provincia_Cod & "§" &
                                              ChiaveAlbero.p_Comune_Cod & "§" &
                                              ChiaveAlbero.p_Sezione & "§" &
                                              ChiaveAlbero.p_Foglio & "§" &
                                              ChiaveAlbero.p_Numero & "§" &
                                              ChiaveAlbero.p_Subalterno & "§" &
                                              ChiaveAlbero.Cod_Fiscale & "§" &
                                              ChiaveAlbero.Fabbricato_Cod & "§" &
                                              ChiaveAlbero.Prodotto_Cod & "§" &
                                              ChiaveAlbero.Data_Lavorazione & "§" &
                                              ChiaveAlbero.Analisi_Certificato_Cod & "§" &
                                              ChiaveAlbero.Analisi_Testata_Cod & "§" &
                                              ChiaveAlbero.Analisi_Dettaglio_Cod & "§" &
                                              ChiaveAlbero.Analisi_Campione_Cod & "§" &
                                              ChiaveAlbero.PianoConcimazioneTestata_Cod & "§" &
                                              ChiaveAlbero.Progetto_Cod & "§" &
                                              ChiaveAlbero.Programmazione_Cod & "§" &
                                              ChiaveAlbero.Programmazione_Entita_Cod & "§" &
                                              ChiaveAlbero.Id_Agenda & "§" &
                                              ChiaveAlbero.PivaPadre & "§" &
                                              ChiaveAlbero.Ricetta_Cod & "§" &
                                              ChiaveAlbero.RicettaOperazione_Cod

        Return ChiaveAlbero_ToString

    End Function

    Private Function RecuperaIDDB(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim objBiz As New AgronicaCoreProvisioningBIZ.DatiServer_R

        Dim datiServerDT = objBiz.GetDatiServer(AgronicaCoreDTOStd.InData.Provisioning.DatiServerRequest.enum_Scelta_Server.Server,
                                                objParametri,
                                                objParametri_Utenti,
                                                objParametri_Super_Server)

        If Not IsDBNull(datiServerDT.Rows(0)("ID_DB")) Then
            Return datiServerDT.Rows(0)("ID_DB").ToString
        End If

        Throw New Exception("Impossibile recuperare l'ID del DB.")
    End Function
    
End Class
