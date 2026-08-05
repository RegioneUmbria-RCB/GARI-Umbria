Imports System.Net.Http
Imports System.Text
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.OutData.Gis.MUZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.analisi
Imports AgronicaCoreModelsSTD.metaschema
Imports Newtonsoft.Json.Linq

''' <summary>
''' Gestisce il caso in cui le MUZ non siano
''' disponibili nel database locale GIAS. Invoca l'engine "SAT e Grandi layer" in
''' modalità sincrona, e mappa il GeoJSON ricevuto su
''' </summary>
Public Class GenerazioneMuzDaEngineSat

    ''' <summary>
    ''' Shared HttpClient con timeout esplicito di 30 secondi per le chiamate
    ''' all'endpoint MUZ dell'engine SAT. Statico per evitare socket exhaustion.
    ''' </summary>
    Friend Shared ReadOnly SatMuzHttpClient As New HttpClient() With {
        .Timeout = TimeSpan.FromSeconds(60)
    }

    ''' <summary>
    ''' Genera le MUZ per la geometria
    ''' fornita invocando l'endpoint <c>/muz/api/v1/evaluate</c> dell'engine SAT
    ''' in modalità sincrona.
    ''' </summary>
    ''' <param name="wktImpianto">WKT del poligono dell'appezzamento/impianto (SRID 4326).</param>
    ''' <param name="entitaCod">Entita_Cod GIAS del poligono (campo <c>geometry.id</c>).</param>
    ''' <param name="dataOperazione">Data dell'operazione di semina (campo <c>evaluation_options.at</c>).</param>
    ''' <param name="objParametri">Parametri di connessione al database.</param>
    ''' <returns>Lista di <see cref="SatMuzFeature_Out"/> estratte dal GeoJSON di risposta.</returns>
    ''' <exception cref="Exception">
    '''   SatEngineNotFoundException  — endpoint non configurato o non raggiungibile dopo 3 retry.<br/>
    '''   SatEngineTimeoutException   — risposta non ricevuta entro 60 secondi.<br/>
    '''   SatEngineAuthenticationException — credenziali API invalide (401/403).<br/>
    '''   SatEngineClientException    — geometria malformata o parametri invalidi (400).<br/>
    '''   SatEngineInternalException  — errore lato engine (500/503) non recuperabile.<br/>
    '''   InvalidGeometryException    — risposta GeoJSON non parsabile.
    ''' </exception>
    Public Shared Function GeneraMuz(wktImpianto As String,
                                     entitaCod As Integer,
                                     dataOperazione As Date,
                                     ByRef objParametri As AgronicaCoreParametri) As List(Of SatMuzFeature_Out)

        Dim confReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim satBaseUrl = confReader.Leggi_Valore(0, CostantiPersonalizzate.SAT_BaseUrl_ConfKey,
                                                 "", "", objParametri)
        If String.IsNullOrEmpty(satBaseUrl) Then
            Throw New Exception(
                "SatEngineNotFoundException: URL Engine SAT (urlEngine_SAT) non configurato in Configurazione_Siti.")
        End If

        Dim satApiKey = confReader.Leggi_Valore(0, CostantiPersonalizzate.SAT_ApiKey_ConfKey,
                                                "", "", objParametri)
        If String.IsNullOrEmpty(satApiKey) Then
            Throw New Exception(
                "SatEngineNotFoundException: API Key Engine SAT (apiKeyEngine_SAT) non configurata in Configurazione_Siti.")
        End If

        Dim endpoint = String.Format("https://{0}/muz/api/v1/evaluate", satBaseUrl)
        Dim requestBody = CostruisciRequestBody(wktImpianto, entitaCod, dataOperazione)

        Return InviaSatMuzConRetry(endpoint, satApiKey, requestBody)
    End Function

    ''' <summary>
    ''' Costruisce il body JSON per la richiesta all'endpoint /muz/api/v1/evaluate.
    ''' </summary>
    Private Shared Function CostruisciRequestBody(wktImpianto As String,
                                                  entitaCod As Integer,
                                                  dataOperazione As Date) As JObject

        Dim geometry As New JObject()
        geometry("srid") = 4326
        geometry("wkt") = wktImpianto
        geometry("id") = entitaCod

        Dim evaluationOptions As New JObject()
        evaluationOptions("at") = dataOperazione.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

        Dim body As New JObject()
        body("geometry") = geometry
        body("evaluation_options") = evaluationOptions

        Return body
    End Function

    ''' <summary>
    ''' Invia la richiesta POST all'engine SAT con backoff esponenziale (1° immediato; 2° dopo 5 s; 3° dopo 15 s).
    ''' I codici 500 e 503 attivano il retry; i codici 4xx generano un'eccezione immediata.
    ''' </summary>
    Private Shared Function InviaSatMuzConRetry(endpoint As String,
                                                apiKey As String,
                                                requestBody As JObject) As List(Of SatMuzFeature_Out)

        ' 1° tentativo immediato, 2° dopo 5 s, 3° dopo 15 s
        Dim backoffSeconds As Integer() = {0, 5, 15}
        Dim lastException As Exception = Nothing

        For tentativo = 0 To 2
            If backoffSeconds(tentativo) > 0 Then
                Thread.Sleep(TimeSpan.FromSeconds(backoffSeconds(tentativo)))
            End If

            Try
                ' HttpRequestMessage non è riutilizzabile dopo l'invio: viene ricreato ad ogni tentativo
                Dim request As New HttpRequestMessage(HttpMethod.Post, endpoint)
                request.Headers.TryAddWithoutValidation("Authorization", "APIKEY " & apiKey)
                request.Content = New StringContent(requestBody.ToString(), Encoding.UTF8, "application/json")

                Dim response As HttpResponseMessage =
                    SatMuzHttpClient.SendAsync(request).GetAwaiter().GetResult()
                Dim responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                Dim statusCode = CInt(response.StatusCode)

                Select Case statusCode
                    Case 200
                        Return ParseGeoJsonResponse(responseBody)
                    Case 202
                        lastException = New Exception(
                            "SatEngineInternalException: engine SAT ha risposto 202 Accepted (elaborazione in corso).")
                    Case 400
                        Throw New Exception(
                            "SatEngineClientException: Geometria malformata o parametri invalidi. Dettagli: " &
                            responseBody)
                    Case 401, 403
                        Throw New Exception(
                            "SatEngineAuthenticationException: Credenziali API Engine SAT invalide (HTTP " &
                            statusCode & ").")
                    Case 404
                        Throw New Exception(
                            "SatEngineNotFoundException: Endpoint Engine SAT non trovato: " & endpoint)
                    Case 500, 503
                        lastException = New Exception(
                            String.Format(
                                "SatEngineInternalException: errore lato engine SAT (HTTP {0}). Dettagli: {1}",
                                statusCode, responseBody))
                    Case Else
                        lastException = New Exception(
                            String.Format(
                                "SatEngineInternalException: risposta inattesa dall'engine SAT (HTTP {0}).",
                                statusCode))
                End Select

            Catch ex As TaskCanceledException
                lastException = New Exception(
                    "SatEngineTimeoutException: Risposta non ricevuta entro 30 secondi.", ex)
            Catch ex As HttpRequestException
                lastException = New Exception(
                    "SatEngineNotFoundException: Endpoint engine SAT non raggiungibile. " & ex.Message, ex)
            End Try
        Next

        If lastException IsNot Nothing Then
            Throw lastException
        End If
        Throw New Exception(
            "SatEngineNotFoundException: Engine SAT non raggiungibile dopo 3 tentativi.")
    End Function

    ''' <summary>
    ''' Parsing GeoJSON: Mappa le feature del GeoJSON di risposta su
    ''' Lancia <c>SatEngineClientException</c> se features è assente o vuoto.
    ''' </summary>
    Private Shared Function ParseGeoJsonResponse(responseBody As String) As List(Of SatMuzFeature_Out)

        Dim jObj As JObject
        Try
            jObj = JObject.Parse(responseBody)
        Catch ex As Exception
            Throw New Exception(
                "InvalidGeometryException: Risposta GeoJSON non parsabile. Dettagli: " & ex.Message, ex)
        End Try

        Dim featuresToken = jObj.Item("features")
        If featuresToken Is Nothing OrElse featuresToken.Type = JTokenType.Null Then
            Throw New Exception("SatEngineClientException: Risposta GeoJSON non contiene il campo 'features'.")
        End If

        Dim result As New List(Of SatMuzFeature_Out)

        For Each featureToken As JToken In featuresToken
            Dim feature = DirectCast(featureToken, JObject)
            Dim geometry = TryCast(feature.Item("geometry"), JObject)
            Dim properties = TryCast(feature.Item("properties"), JObject)

            Dim geo As New SatMuzGeometry_Out With {
                .Type = geometry?.Item("type")?.ToString(),
                .Coordinates = geometry?.Item("coordinates")?.ToObject(Of Double()()())()
            }

            Dim props As New SatMuzProperties_Out With {
                .IdMuz = properties?.Item("id_MUZ")?.ToString(),
                .IdClasse = properties?.Item("id_classe")?.ToString(),
                .IdGeom = properties?.Item("id_geom")?.Value(Of Integer?)(),
                .IdSoil = properties?.Item("id_soil")?.ToString(),
                .Area = properties?.Item("area")?.Value(Of Double?)(),
                .Clay = properties?.Item("clay")?.Value(Of Double?)(),
                .Silt = properties?.Item("silt")?.Value(Of Double?)(),
                .Sand = properties?.Item("sand")?.Value(Of Double?)(),
                .SoilTexture = properties?.Item("soil_texture")?.ToString(),
                .Ce = properties?.Item("CE")?.Value(Of Double?)(),
                .K = properties?.Item("K")?.Value(Of Double?)(),
                .NTot = properties?.Item("N_tot")?.Value(Of Double?)(),
                .Oc = properties?.Item("OC")?.Value(Of Double?)(),
                .Soc = properties?.Item("SOC")?.Value(Of Double?)(),
                .Ph = properties?.Item("pH")?.Value(Of Double?)(),
                .P = properties?.Item("P")?.Value(Of Double?)(),
                .Bdod = properties?.Item("BDOD")?.Value(Of Double?)(),
                .CscMeq = properties?.Item("CSC_meq")?.Value(Of Double?)(),
                .Cod = properties?.Item("COD")?.Value(Of Double?)(),
                .So = properties?.Item("SO")?.ToString(),
                .Ca = properties?.Item("Ca")?.ToString(),
                .Mg = properties?.Item("Mg")?.ToString(),
                .Na = properties?.Item("Na")?.ToString()
            }

            result.Add(New SatMuzFeature_Out With {
                .Type = feature.Item("type")?.ToString(),
                .Geometry = geo,
                .Properties = props
            })
        Next

        Return result
    End Function

    ''' <summary>
    ''' Persiste le MUZ generate dall'engine SAT
    ''' nel database locale creando, per ogni feature ricevuta:
    ''' un'analisi virtuale (Analisi_Terreno), un record in Area_Omogenea_MUZ,
    ''' l'entità GIS e le associazioni con l'appezzamento e l'analisi.
    ''' Al termine rilegge le MUZ appena salvate tramite LeggiMUZDaAppezzamento
    ''' e restituisce il DataTable per continuare il flusso di semina.
    ''' </summary>
    ''' <param name="muzGenerate">Feature GeoJSON restituite dall'engine SAT.</param>
    ''' <param name="piva">Partita IVA dell'azienda.</param>
    ''' <param name="saCod">Codice SA.</param>
    ''' <param name="appezza">Codice appezzamento.</param>
    ''' <param name="dataRichiesta">Data dell'operazione di semina (validità analisi e MUZ).</param>
    ''' <param name="objParametriServer">Parametri di connessione server.</param>
    ''' <param name="objParametriUtenti">Parametri utente.</param>
    ''' <param name="objParametriSuperServer">Parametri super-server.</param>
    ''' <returns>DataTable delle MUZ appena persistite (stessa struttura di LeggiMUZDaAppezzamento).</returns>
    ''' <exception cref="Exception">MuzPersistenceException se la persistenza fallisce; AnalysisGenerationException se la creazione dell'analisi virtuale fallisce.</exception>
    Public Shared Function PersistiMuzDaEngineSat(muzGenerate As List(Of SatMuzFeature_Out),
                                                   piva As String,
                                                   saCod As Integer,
                                                   appezza As Integer,
                                                   validitaInizio As Date,
                                                   validitaFine As Date,
                                                   ByRef objParametriServer As AgronicaCoreParametri,
                                                   ByRef objParametriUtenti As AgronicaCoreParametri,
                                                   ByRef objParametriSuperServer As AgronicaCoreParametri) As DataTable

        Const routine = "AgronicaCorePrecisionFarmingBIZ.GenerazioneMuzDaEngineSat.PersistiMuzDaEngineSat()"

        If muzGenerate Is Nothing OrElse muzGenerate.Count = 0 Then
            Throw New Exception(
                String.Format("[{0}] MuzPersistenceException: Nessuna MUZ da persistere ricevuta dall'engine SAT.", routine))
        End If

        Dim objMuzBiz As New AgronicaCoreGisBIZ.GIS_MUZ_W()

        For Each feature In muzGenerate
            If feature.Properties Is Nothing OrElse feature.Geometry Is Nothing Then 
                Throw New Exception(
                    String.Format("[{0}] MuzPersistenceException: Una Muz ritornata non ha una geometria associata.", routine))
            End If

            Dim props = feature.Properties

            ' --- Step 1 (DS32-BL §1): Crea analisi virtuale con i dati di suolo ---
            Dim analisiTestataCod = 0
            Try
                Dim analisiTerreno As New AnalisiTerreno With {
                    .AnalisiTipo = New AnalisiTipo(CInt(TipiEnumerativi.enum_AnalisiTipo.Analisi_Terreno)),
                    .analisiTipologia = New AnalisiTipologia(CInt(TipiEnumerativi.enum_AnalisiTipologia_Schema.Area_Omogenea)),
                    .validita = New IntervalloTemporale(validitaInizio, validitaFine),
                    .certificatoAnalisi = Nothing,
                    .campioni = Nothing,
                    .dettagli = CreaDettagliAnalisiMuz(props)
                }

                Dim objAnalisi As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W()
                objAnalisi.Scrivi_AnalisiTerreno_Modello(
                    piva,
                    analisiTestataCod,
                    analisiTerreno,
                    objParametriSuperServer,
                    objParametriServer,
                    objParametriUtenti)

            Catch ex As Exception
                Throw New Exception(
                    String.Format("[{0}] AnalysisGenerationException: {1}", routine, ex.Message), ex)
            End Try

            ' --- Step 2 (DS32-BL §2): Crea MUZ in Area_Omogenea e associa all'appezzamento e all'analisi ---
            Dim soValue As Double = 0.0
            If props.So IsNot Nothing Then
                Double.TryParse(props.So,
                                Globalization.NumberStyles.Any,
                                Globalization.CultureInfo.InvariantCulture,
                                soValue)
            End If

            Try
                Dim wktMuz = ConvertCoordinatesToWkt(feature.Geometry)

                ' Analisi_Testate_Aggiungi: il BIZ ScriviMUZ chiama AssociaMUZAdAnalisi per ogni voce
                Dim analisiDaAssociare As New List(Of Integer)
                If analisiTestataCod > 0 Then analisiDaAssociare.Add(analisiTestataCod)

                Dim muzModel As New MUZ With {
                    .Piva = piva,
                    .Area_Cod = 0,
                    .Area_Des = String.Format("MUZ {0} - {1} - {2}", piva, saCod, appezza),
                    .Tessitura_cod = 0,
                    .Altimetria = String.Empty,
                    .SO = soValue,
                    .TipoZona = String.Empty,
                    .Geometry = wktMuz,
                    .Validita_Inizio = validitaInizio,
                    .Validita_Fine = validitaFine,
                    .Operazione_Cod = enum_Operazioni_MUZ.INSERT,
                    .Gruppo_Area_Cod = 0,
                    .Gruppo_Area_Des = String.Empty,
                    .Analisi_Testate_Aggiungi = analisiDaAssociare,
                    .Analisi_Testate_Elimina = New List(Of Integer),
                    .Particelle_Catastali_Aggiungi = New List(Of ParticelleCatastali_MUZ),
                    .Particelle_Catastali_Elimina = New List(Of ParticelleCatastali_MUZ),
                    .appezzamento = New List(Of Appezzamento_MUZ) From {
                        New Appezzamento_MUZ With {
                            .Piva = piva,
                            .Sa_Cod = saCod,
                            .Appezza = appezza
                        }
                    }
                }

                If Not objMuzBiz.EseguiOperazioneMUZ(muzModel, objParametriServer, True) Then
                    Throw New Exception(
                        String.Format("Errore nel salvataggio della MUZ per appezzamento {0}/{1}/{2}.", piva, saCod, appezza))
                End If

            Catch ex As Exception
                Throw New Exception(
                    String.Format("[{0}] MuzPersistenceException: {1}", routine, ex.Message), ex)
            End Try
        Next

        ' --- Step 3: Rilegge le MUZ appena persistite per continuare il flusso ---
        Dim muzReader As New AgronicaCoreGisDAL.GIS_MUZ_R()
        Dim dtMuzPersistite = muzReader.LeggiMUZDaAppezzamento(piva, saCod, appezza, objParametriServer)

        If dtMuzPersistite Is Nothing OrElse dtMuzPersistite.Rows.Count = 0 Then
            Throw New Exception(
                String.Format("[{0}] MuzPersistenceException: MUZ persistite ma non rilette dall'appezzamento {1}/{2}/{3}.",
                              routine, piva, saCod, appezza))
        End If

        Return dtMuzPersistite
    End Function

    ''' <summary>
    ''' Costruisce la lista dei dettagli analitici da associare all'analisi virtuale
    ''' usando i valori pedologici restituiti dall'engine SAT.
    ''' I parametri con valore Nothing vengono omessi.
    ''' </summary>
    Private Shared Function CreaDettagliAnalisiMuz(props As SatMuzProperties_Out) As List(Of AnalisiDettaglio)
        Dim dettagli As New List(Of AnalisiDettaglio)

        Dim aggiungi = Sub(paramCod As TipiEnumerativi.enum_AnalisiParametri, valore As Double?)
            If valore.HasValue Then
                dettagli.Add(New AnalisiDettaglio With {
                    .parametro = New AnalisiParametro(CInt(paramCod)),
                    .valore1 = valore
                })
            End If
        End Sub

        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Argilla, props.Clay)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Limo, props.Silt)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Sabbia, props.Sand)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_pH, props.Ph)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_K_scambiabile, props.K)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Ntot, props.NTot)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_P_assimilabile, props.P)
        aggiungi(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_CSC, props.CscMeq)

        ' Mg è restituito come stringa dall'engine SAT: si converte con fallback silente
        Dim mgValue = 0.0
        If props.Mg IsNot Nothing AndAlso
           Double.TryParse(props.Mg, Globalization.NumberStyles.Any,
                           Globalization.CultureInfo.InvariantCulture, mgValue) Then
            dettagli.Add(New AnalisiDettaglio With {
                .parametro = New AnalisiParametro(
                    CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile)),
                .valore1 = mgValue
            })
        End If

        Return dettagli
    End Function

    ''' <summary>
    ''' Converte le coordinate GeoJSON di un poligono (double[][][]) in stringa WKT
    ''' considerando solo l'anello esteriore (prima sequenza di coordinate).
    ''' </summary>
    Private Shared Function ConvertCoordinatesToWkt(geometry As SatMuzGeometry_Out) As String
        If geometry Is Nothing OrElse
           geometry.Coordinates Is Nothing OrElse
           geometry.Coordinates.Length = 0 OrElse
           geometry.Coordinates(0) Is Nothing Then
            Throw New Exception("InvalidGeometryException: Geometria MUZ mancante o non valida.")
        End If

        ' GeoJSON Polygon: coordinates[ring][point] dove ogni point è [lon, lat]
        Dim sb As New Text.StringBuilder("POLYGON ((")
        Dim ring = geometry.Coordinates(0)
        For i = 0 To ring.Length - 1
            If i > 0 Then sb.Append(", ")
            sb.AppendFormat(Globalization.CultureInfo.InvariantCulture, "{0} {1}", ring(i)(0), ring(i)(1))
        Next
        sb.Append("))")
        Return sb.ToString()
    End Function

End Class
