Imports System.Text
Imports System.IO
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json.Converters
Imports System.Linq

Public Class EsolverOrdiniMV : Inherits LogProvider

    Private _objParametriSuperServer As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Private _objParametriInterscambio As AgronicaCoreParametri

    Private _objConfigServizio As Configurazione_Servizio
    Private _parametriExtra As String
    Private _objSettings As EsolverParametriOrdiniMV

    Private _handleRest As Http

    Private _token As String
    ''' <summary>
    ''' Il flag deve servire a ripetere una volta sola il tentativo di effettuare il ws nel caso sia dato errore di token scaduto,
    ''' e deve essere re-impostato alla prima chiamata effettuata con successo
    ''' </summary>
    Private _ritentaTokenNonValido As Boolean
    Private _tokenScaduto As Boolean

    Private _logFileName As String
    Private _logDirectory As String
    Private _logDescrizioneUtente As String

    Private customLOGParams As CustomLOGParams

    Public Sub New(ByVal configurazioneServizio As Configurazione_Servizio,
                   ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)

        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _objParametriInterscambio = New AgronicaCoreParametri(_objParametriServer)

        _parametriExtra = configurazioneServizio.Parametri_Extra
        _objSettings = JsonConvert.DeserializeObject(Of EsolverParametriOrdiniMV)(configurazioneServizio.Parametri_Extra)
        _objConfigServizio = configurazioneServizio

        _handleRest = New Http()
        _tokenScaduto = False
        _ritentaTokenNonValido = True

        _logFileName = configurazioneServizio.Tipo_Sincro.ToString & "_log.txt"
        _logDirectory = configurazioneServizio.DirectoryLOG
        _logDescrizioneUtente = configurazioneServizio.Tipo_Sincro.ToString

        customLOGParams = New CustomLOGParams With {
        .LogDescrizioneUtente = _logDescrizioneUtente,
        .LogDirectory = _logDirectory,
        .LogFileName = _logFileName
        }

    End Sub

    ''' <summary>
    ''' Invia su eSolver di ASIPO gli ordini che sono negli stati da_inviare, inviato e cancellato
    ''' </summary>
    ''' <param name="logSuFileSystem">se true, i log di errore, oltre ad essere restituiti vengono 
    ''' anche scritti su filesystem in base alla configurazione in _objParametriServer</param>
    ''' <param name="xFiltroAggiuntivo">filtro per la query</param>
    ''' <returns>Elenco dei messaggi di errore</returns>
    Public Function Esporta(Optional ByVal logSuFileSystem As Boolean = True, Optional ByVal xFiltroAggiuntivo As String = "") As List(Of String)

        Const nomeRoutine = "AnagrafeBIZ.EsolverOrdiniMV.Esporta()"

        Dim logErrori As New List(Of String)()
        Dim respEndpoint As Http.JsonRestResponse

        Dim logInvioChiamate As New Agronica_Log_Invio_Chiamate_W()

        Try
            Dim handleProgrammazioneTestata As New Programmazione_Testata_R()
            Dim dt = handleProgrammazioneTestata.LeggiEsportazioneMaterialeVivaisticoOrdini(xFiltroAggiuntivo, "", _objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            Dim jsonConverters As New IsoDateTimeConverter()
            jsonConverters.DateTimeFormat = "yyyy-MM-dd"

            Dim handleWriteProgrammazioneTestata As New Programmazione_Testata_W()

            Dim giasContext = Gias_EF_Utility.CreateGiasContextConnection(_objParametriServer.StringaConnessione)

            For Each dr As DataRow In dt.Rows

                Dim jobjectBody = CreaModelInvioOrdine(dr)

                Dim jsonFromDR = JsonConvert.SerializeObject(jobjectBody, jsonConverters)

                '-- CHIAMO WS -----------------------------------------------------------------------------------
                respEndpoint = ChiamaWS(jsonFromDR, _objSettings.ExportEndpoint)
                '------------------------------------------------------------------------------------------------

                Dim programmazioneCod = dr.Field(Of Integer)("Programmazione_Cod")
                Dim stato = dr.Field(Of enum_PrenotazionePiante_Stato)("Stato")

                Dim esito As Integer = 0
                Dim guidOperazione As String = ""

                Dim serializeContentForLog = JsonConvert.SerializeObject(respEndpoint.Content, jsonConverters)

                If respEndpoint.StatusCode = HttpStatusCode.OK Then

                    Dim rispExportOrdini = JsonConvert.DeserializeObject(Of EsolverRispostaExportOrdiniMV)(JsonConvert.SerializeObject(respEndpoint.Content.Item("Risposta")))

                    If rispExportOrdini.Esito = 1 Then
                        esito = rispExportOrdini.Esito
                        guidOperazione = rispExportOrdini.GuidOperazione

                        If stato <> enum_PrenotazionePiante_Stato.o_cancellato Then
                            'Segno l'ordine come correttamente inviato.
                            'Se lo stato di partenza è di cancellazione non devo aggiornarlo
                            handleWriteProgrammazioneTestata.Modifica_Stato(programmazioneCod, enum_PrenotazionePiante_Stato.o_inviato, "", _objParametriServer)
                        End If
                    End If

                    'Nei casi in cui lo StatusCode non sia 200 oppure Esito è 0,
                    'Scrivo il record di log dell'invio, non modificando lo stato,
                    'per poter riprovare l'invio alla prossima esecuzione
                Else
                    Dim dataOrdineGias = dr.Field(Of Date)("Data_Ordine_Gias")
                    Dim numeroOrdineGias = dr.Field(Of String)("Numero_Ordine_Gias") 'È scritto in questo formato: OYY-nnnnn ove 'O' sta per ordine. Devo inviare solo il numero

                    Dim dataOrdineGiasInvio = dataOrdineGias.ToString("dd-MM-yyyy")
                    Dim numeroOrdineGiasInvio = CInt(numeroOrdineGias.Split("-")(1))

                    Dim mesRisp = String.Format("Ordine Data-Num [{0}-{1}] ID [{2}] HttpStatusCode [{3}] Errore : {4}", 
                        dataOrdineGiasInvio, numeroOrdineGiasInvio, programmazioneCod, respEndpoint.StatusCode, serializeContentForLog)

                    logErrori.Add(mesRisp)

                    If logSuFileSystem Then
                        Scrivi_LOG(_objParametriServer, nomeRoutine, mesRisp, CustomLOGParams:=customLOGParams)
                    End If
                End If

                Dim operazione = enum_TipoOperazioneDB.Scrittura

                If stato = enum_PrenotazionePiante_Stato.o_cancellato Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf Not IsDBNull(dr("ID")) Then
                    operazione = enum_TipoOperazioneDB.Modifica
                End If

                logInvioChiamate.Create_Agronica_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.AsipoOrdiniMVExport,
                    jsonFromDR,
                    Date.Now(),
                    respEndpoint.StatusCode,
                    serializeContentForLog,
                    0, '?
                    operazione, 'In questo caso sarebbe da impostare sempre "POST", quindi invece la userei per dire creato (primo invio), modificato, cancellato
                    _objParametriServer,
                    giasContext, programmazioneCod, esito, guidOperazione)

            Next

        Catch ex As Exception

            Dim messaggioErrore As String

            if Not IsNothing(respEndpoint) Then
                messaggioErrore = String.Format("{0}{1}   Risposta Endpoint : {2}", 
                    ex.ToString(), vbCrLf, JsonConvert.SerializeObject(respEndpoint))
            Else
                messaggioErrore = ex.ToString()
            End If

            logErrori.Add(messaggioErrore)

            If logSuFileSystem Then
                Scrivi_LOG(_objParametriServer, nomeRoutine, messaggioErrore, CustomLOGParams:=customLOGParams)
            End If
        End Try

        Return logErrori

    End Function

    Private Function CreaModelInvioOrdine(ByRef dr As DataRow) As JObject

        Dim dataOrdineGias = dr.Field(Of Date)("Data_Ordine_Gias")
        Dim numeroOrdineGias = dr.Field(Of String)("Numero_Ordine_Gias") 'È scritto in questo formato: OYY-nnnnn ove 'O' sta per ordine. Devo inviare solo il numero

        Dim numeroOrdineGiasInvio = CInt(numeroOrdineGias.Split("-")(1))

        Dim stato = dr.Field(Of enum_PrenotazionePiante_Stato)("Stato")
        Dim qtaSemeEvasa = dr.Field(Of Integer)("Qta_Seme_Evaso")

        Dim cancellato = If(stato = enum_PrenotazionePiante_Stato.o_cancellato, 1, 0)

        Dim modelDataOrdineGias = New EsolverModelOrdiniMV_Data(EsolverTipiParametriOrdiniMV.DataOrdineGias, dataOrdineGias)
        Dim modelNumeroOrdineGias = New EsolverModelOrdiniMV_Numerico(EsolverTipiParametriOrdiniMV.NumeroOrdineGias, numeroOrdineGiasInvio)
        Dim modelCancellato = New EsolverModelOrdiniMV_Numerico(EsolverTipiParametriOrdiniMV.OrdineCancellato, cancellato)

        'Queste proprietà devono essere valorizzate ai loro default per l'array di testata, mentre devono contenere i dati per quello di riga
        Dim modelCodArticolo = New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.CodiceArticolo, "")
        Dim modelQtaSeme = New EsolverModelOrdiniMV_Numerico(EsolverTipiParametriOrdiniMV.QuantitaSemi, 0)
        Dim modelDataConsegna = New EsolverModelOrdiniMV_Data(EsolverTipiParametriOrdiniMV.DataConsegnaPrevista, Nothing)
        Dim modelQtaPiante = New EsolverModelOrdiniMV_Numerico(EsolverTipiParametriOrdiniMV.QuantitaPiante, 0)
        Dim modelDataTrapianto = New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.DataTrapiantoPrevista, "")
        Dim modelPivaAzAgricola = New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.PivaAziendaAgricola, "")
        Dim modelPivaVivaio = New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.PivaVivaio, "")
        Dim modelOmaggio = New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.Omaggio, "")

        Dim jarrayTestata As New JArray From {
            JToken.FromObject(New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.TipoRiga, "TES")),
            JToken.FromObject(modelDataOrdineGias),
            JToken.FromObject(modelNumeroOrdineGias),
            JToken.FromObject(modelCancellato),
            JToken.FromObject(modelCodArticolo),
            JToken.FromObject(modelQtaSeme),
            JToken.FromObject(modelDataConsegna),
            JToken.FromObject(modelQtaPiante),
            JToken.FromObject(modelDataTrapianto),
            JToken.FromObject(modelPivaAzAgricola),
            JToken.FromObject(modelPivaVivaio),
            JToken.FromObject(modelOmaggio)
        }

        Dim codArticolo = dr.Field(Of String)("Cod_Articolo")
        Dim qtaSeme = dr.Field(Of String)("Qta_Semente")
        Dim dataConsegna = dr.Field(Of Date)("Data_Consegna_Prevista")
        Dim qtaPiante = dr.Field(Of Integer)("Qta_Piante")
        Dim dataTrapianto = dr.Field(Of Date)("Data_Trapianto_Prevista")
        Dim pivaAzAgricola = dr.Field(Of String)("Piva_Azienda_Agricola")
        Dim pivaVivaio = dr.Field(Of String)("Piva_Vivaio")
        Dim valOmaggio = dr.Field(Of Integer)("Qta_Seme_Omaggio")

        modelCodArticolo.Valore = codArticolo
        modelQtaSeme.Valore = qtaSeme
        modelDataConsegna.Valore = dataConsegna
        modelQtaPiante.Valore = qtaPiante
        modelDataTrapianto.Valore = dataTrapianto.ToString("dd/MM/yyyy")
        modelPivaAzAgricola.Valore = pivaAzAgricola
        modelPivaVivaio.Valore = pivaVivaio
        modelOmaggio.Valore = If(valOmaggio > 0, "1", "0")

        Dim jarrayRiga As New JArray From {
            JToken.FromObject(New EsolverModelOrdiniMV_AlfaNumerico(EsolverTipiParametriOrdiniMV.TipoRiga, "RIG")),
            JToken.FromObject(modelDataOrdineGias),
            JToken.FromObject(modelNumeroOrdineGias),
            JToken.FromObject(modelCancellato),
            JToken.FromObject(modelCodArticolo),
            JToken.FromObject(modelQtaSeme),
            JToken.FromObject(modelDataConsegna),
            JToken.FromObject(modelQtaPiante),
            JToken.FromObject(modelDataTrapianto),
            JToken.FromObject(modelPivaAzAgricola),
            JToken.FromObject(modelPivaVivaio),
            JToken.FromObject(modelOmaggio)
        }

        Dim jobjectRigaTES As New JObject()
        jobjectRigaTES.Add("Riga", jarrayTestata)

        Dim jobjectRigaRIG As New JObject()
        jobjectRigaRIG.Add("Riga", jarrayRiga)

        'Ho un array con i due object di riga
        Dim jarrayFile As New JArray From {jobjectRigaTES, jobjectRigaRIG}

        Dim jobjectFile As New JObject()
        jobjectFile.Add("File", jarrayFile)

        Dim jobjectParametri As New JObject()
        jobjectParametri.Add("Tipologia", "RDA")
        jobjectParametri.Add("Modello", 1)
        jobjectParametri.Add("Dati", jobjectFile)

        Dim jobjectBody As New JObject()
        jobjectBody.Add("Parametri", jobjectParametri)

        Return jobjectBody

    End Function

    ''' <summary>
    ''' Interroga il ws di eSolver per ASIPO per ottenere le informazioni aggiornate di tutti gli ordini già inviati oppure uno specifico
    ''' </summary>
    ''' <param name="logSuFileSystem">se true, i log di errore, oltre ad essere restituiti vengono 
    ''' anche scritti su filesystem in base alla configurazione in _objParametriServer</param>
    ''' <param name="programmazioneCodOrdine"></param>
    ''' <param name="nuovoStatoOrdine">Parametro di ritorno, significativo se il parametro <paramref name="programmazioneCodOrdine"/> è valorizzato,
    ''' restituisce lo stato assegnato all'ordine. Nel caso non sia modificato, ha valore uguale a quello sul database, in caso di errore vale IntDefault_per_MODIFICA</param>
    ''' <returns>Elenco dei messaggi di errore</returns>
    Public Function AggiornaStato(Optional ByVal logSuFileSystem As Boolean = True, Optional ByVal programmazioneCodOrdine As Integer = 0, Optional ByRef nuovoStatoOrdine As enum_PrenotazionePiante_Stato = -999) As List(Of String)

        Const nomeRoutine = "AnagrafeBIZ.EsolverOrdiniMV.AggiornaStato()"

        Dim logErrori As New List(Of String)()
        Dim respEndpoint As New Http.JsonRestResponse

        Dim logInvioChiamate As New Agronica_Log_Invio_Chiamate_W()

        Try

            Dim handleProgrammazioneTestata As New Programmazione_Testata_R()

            Dim xFiltroAggiuntivo As String = ""

            If programmazioneCodOrdine <> 0 Then
                xFiltroAggiuntivo = "Programmazione_Testata.Programmazione_Cod = " & programmazioneCodOrdine
            End If

            Dim dt = handleProgrammazioneTestata.LeggiOrdiniMV_Inviati(xFiltroAggiuntivo, "", _objParametriServer)

            Dim jArrayOrdini As New JArray()
            Dim dictOrdini As New Dictionary(Of String, Integer)()

            For Each dr As DataRow In dt.Rows
                Dim dataOrdineGias = dr.Field(Of Date)("Data_Ordine_Gias")
                Dim numeroOrdineGias = dr.Field(Of String)("Numero_Ordine_Gias") 'È scritto in questo formato: OYY-nnnnn ove 'O' sta per ordine. Devo inviare solo il numero

                Dim dataOrdineGiasInvio = dataOrdineGias.ToString("dd-MM-yyyy")
                Dim numeroOrdineGiasInvio = CInt(numeroOrdineGias.Split("-")(1))

                jArrayOrdini.Add(dataOrdineGiasInvio & "-" & numeroOrdineGiasInvio)

                Dim idOrdine = dr.Field(Of Integer)("Programmazione_Cod")

                dictOrdini.Add(dataOrdineGiasInvio & "-" & numeroOrdineGiasInvio, idOrdine)
            Next

            Dim jsonFromDT As String = ""

            'Se non sono stati trovati ordini, non effettuo la chiamata e non restituisco errori
            If jArrayOrdini.Count > 0 Then
                Dim jobjectFiltro As New JObject()
                jobjectFiltro.Add("Name", "RDADataNum")
                jobjectFiltro.Add("Operators", 11)
                jobjectFiltro.Add("Values", jArrayOrdini)

                Dim jarrayFiltri As New JArray()
                jarrayFiltri.Add(jobjectFiltro)

                Dim jobjectBody As New JObject()
                jobjectBody.Add("Filters", jarrayFiltri)

                jsonFromDT = JsonConvert.SerializeObject(jobjectBody)

                '-- CHIAMO WS -----------------------------------------------------------------------------------
                respEndpoint = ChiamaWS(jsonFromDT, _objSettings.StatoEndpoint)
                '------------------------------------------------------------------------------------------------
            End If

            If respEndpoint.StatusCode = HttpStatusCode.OK Then

                Dim handleWriteProgrammazioneTestata As New Programmazione_Testata_W()
                Dim handleWriteProgrammazioneEntita As New Programmazione_Entita_W()
                Dim handleContatti As New Contatti_R

                Dim rispStatiOrdini = JsonConvert.DeserializeObject(Of EsolverRispostaGetStatoOrdiniMV)(JsonConvert.SerializeObject(respEndpoint.Content))

                If rispStatiOrdini.Result IsNot Nothing Then

                    For Each esolverStatoOrdini In rispStatiOrdini.Result

                        Dim programmazioneCod As Integer = 0

                        'Il WS restituisce nell'elenco solo i documenti che risultano essere stati processati su esolver,
                        'mentre in dictOrdini ho l'elenco che risulta in Gias;
                        'rimuovo dal dictionary gli ordini che corrispondono
                        'quindi alla fine di questo ciclo se risultano degli elementi nel dictionary li scrivo nel log di errore
                        If dictOrdini.ContainsKey(esolverStatoOrdini.RDADataNum) Then
                            programmazioneCod = dictOrdini(esolverStatoOrdini.RDADataNum)
                            dictOrdini.Remove(esolverStatoOrdini.RDADataNum)
                        Else
                            Exit For
                        End If

                        Dim drOrdine = dt.Select("Programmazione_Cod = " & programmazioneCod).FirstOrDefault()

                        Dim programmazioneEntitaCod = drOrdine.Field(Of Integer)("Programmazione_Entita_Cod")

                        Dim ordineStatoGias = drOrdine.Field(Of enum_PrenotazionePiante_Stato)("Stato")

                        nuovoStatoOrdine = ordineStatoGias

                        Dim pivaDittaSementiera As String = ""
                        Dim updQtaEvasa = IntDefault_per_MODIFICA

                        'Devo fare l'update di Programmazione_Testata per la colonna Stato
                        'Mentre di Programmazione_Entita per le colonne Codice_Fiscale_Tecnico e Qta_Seme_Evaso
                        'Stato deve diventare da inviato a generato su esolver se riga ordinata = 1 e evasa < totale
                        'Stato deve diventare da generato su esolver a inviato se riga ordinata = 0
                        'Stato deve diventare evaso se riga ordinata = 1 e evasa >= totale
                        'Stato deve diventare evaso se evaso forzatamente = 1
                        If esolverStatoOrdini.RigaOrdinata = 1 Then

                            Dim dbQtaRichiesta = If(IsNumeric(drOrdine("Qta_Semente")), CInt(drOrdine("Qta_Semente")), 0)

                            If esolverStatoOrdini.QtaConsegnata >= dbQtaRichiesta OrElse
                                esolverStatoOrdini.RigaSaldata = 1 Then

                                nuovoStatoOrdine = enum_PrenotazionePiante_Stato.o_evaso

                                handleWriteProgrammazioneTestata.Modifica_Stato(programmazioneCod, nuovoStatoOrdine, "", _objParametriServer)

                            ElseIf ordineStatoGias = enum_PrenotazionePiante_Stato.o_inviato Then

                                nuovoStatoOrdine = enum_PrenotazionePiante_Stato.o_ordine_creato_esolver

                                handleWriteProgrammazioneTestata.Modifica_Stato(programmazioneCod, nuovoStatoOrdine, "", _objParametriServer)

                            ElseIf ordineStatoGias = enum_PrenotazionePiante_Stato.o_evaso Then
                                'Nuova casistica: l'ordine può essere riportato indietro su esolver dallo stato di evasione, attraverso un reso
                                nuovoStatoOrdine = enum_PrenotazionePiante_Stato.o_ordine_creato_esolver

                                handleWriteProgrammazioneTestata.Modifica_Stato(programmazioneCod, nuovoStatoOrdine, "", _objParametriServer)

                            End If


                            If Not String.IsNullOrWhiteSpace(esolverStatoOrdini.PIVADittaSeme) Then

                                Dim dtContatti = handleContatti.Leggi_Contatti_ByCod_Rapporto(True, "", esolverStatoOrdini.PIVADittaSeme, COD_DITTA_SEMENTIERA, "", "", _objParametriServer)

                                If dtContatti.Rows.Count > 0 Then

                                    If drOrdine.Field(Of String)("Piva_Ditta_Sementiera") <> esolverStatoOrdini.PIVADittaSeme Then
                                        pivaDittaSementiera = esolverStatoOrdini.PIVADittaSeme
                                    End If

                                Else
                                    Dim mesDitta = String.Format("(Warning) Ordine Data-Num [{0}] ID [{1}] : Ditta sementiera {2} non presente come risorsa umana in GIAS",
                                        esolverStatoOrdini.RDADataNum, programmazioneCod, esolverStatoOrdini.PIVADittaSeme)

                                    logErrori.Add(mesDitta)

                                    If logSuFileSystem Then
                                        Scrivi_LOG(_objParametriServer, nomeRoutine, mesDitta, CustomLOGParams:=customLOGParams)
                                    End If

                                End If
                            End If

                            Dim dbQtaEvasa = drOrdine.Field(Of Integer)("Qta_Seme_Evaso")

                            If dbQtaEvasa <> esolverStatoOrdini.QtaConsegnata Then
                                updQtaEvasa = esolverStatoOrdini.QtaConsegnata
                            End If

                        ElseIf ordineStatoGias = enum_PrenotazionePiante_Stato.o_ordine_creato_esolver Then
                            'Vuol dire che il documento è tornato indietro di stato
                            nuovoStatoOrdine = enum_PrenotazionePiante_Stato.o_inviato

                            handleWriteProgrammazioneTestata.Modifica_Stato(programmazioneCod, nuovoStatoOrdine, "", _objParametriServer)
                        End If

                        If Not String.IsNullOrEmpty(pivaDittaSementiera) OrElse updQtaEvasa <> IntDefault_per_MODIFICA Then
                            handleWriteProgrammazioneEntita.Modifica(programmazioneCod, programmazioneEntitaCod, StrDefault_per_MODIFICA,
                                StrDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, StrDefault_per_MODIFICA, IntDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, DoubleDefault_per_MODIFICA, DoubleDefault_per_MODIFICA, StrDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, DoubleDefault_per_MODIFICA, DoubleDefault_per_MODIFICA, IntDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, IntDefault_per_MODIFICA,
                                IntDefault_per_MODIFICA, IntDefault_per_MODIFICA, DataDefault_per_MODIFICA, DataDefault_per_MODIFICA, StrDefault_per_MODIFICA,
                                StrDefault_per_MODIFICA, StrDefault_per_MODIFICA, StrDefault_per_MODIFICA, IntDefault_per_MODIFICA, DataDefault_per_MODIFICA,
                                DataDefault_per_MODIFICA, DataDefault_per_MODIFICA, "", _objParametriServer,
                                Codice_Fiscale_Tecnico:=pivaDittaSementiera, qta_seme_evaso:=updQtaEvasa)
                        End If

                    Next

                End If

                For Each ord In dictOrdini
                    Dim mesOrdNonTrovato = String.Format("Ordine Data-Num [{0}] ID [{1}] : Non è stato trovato in eSolver",
                                    ord.Key, ord.Value)

                    logErrori.Add(mesOrdNonTrovato)

                    If logSuFileSystem Then
                        Scrivi_LOG(_objParametriServer, nomeRoutine, mesOrdNonTrovato, CustomLOGParams:=customLOGParams)
                    End If
                Next

            ElseIf jArrayOrdini.Count > 0 Then 'Se zero vuol dire che non ci sono ordini da controllare quindi restituisco ok...
                Dim mesLogStatusCode = String.Format("Errore HttpStatusCode [{0}] Content : {1}",
                                    respEndpoint.StatusCode, JsonConvert.SerializeObject(respEndpoint.Content))

                logErrori.Add(mesLogStatusCode)

                If logSuFileSystem Then
                    Scrivi_LOG(_objParametriServer, nomeRoutine, mesLogStatusCode, CustomLOGParams:=customLOGParams)
                End If
            ElseIf programmazioneCodOrdine <> 0 Then '...eccetto che non sia stato trovato quello specifico richiesto
                Dim mesLogOrdineNonTrovato = String.Format("Ordine ID [{0}] non è stato trovato in Gias fra quelli di cui verificare lo stato", programmazioneCodOrdine)

                logErrori.Add(mesLogOrdineNonTrovato)

                If logSuFileSystem Then
                    Scrivi_LOG(_objParametriServer, nomeRoutine, mesLogOrdineNonTrovato, CustomLOGParams:=customLOGParams)
                End If
            End If

            Dim giasContext = Gias_EF_Utility.CreateGiasContextConnection(_objParametriServer.StringaConnessione)

            logInvioChiamate.Create_Agronica_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.AsipoOrdiniMVGetStato,
                            jsonFromDT,
                            Date.Now(),
                            respEndpoint.StatusCode,
                            JsonConvert.SerializeObject(respEndpoint.Content),
                            0, '?
                            "POST",
                            _objParametriServer,
                            giasContext, "", "", "")

        Catch ex As Exception
            nuovoStatoOrdine = IntDefault_per_MODIFICA 'Re-imposto il valore di default nel caso di errore

            Dim messaggioErrore As String

            if Not IsNothing(respEndpoint) Then
                messaggioErrore = String.Format("{0}{1}   Risposta Endpoint : {2}", 
                    ex.ToString(), vbCrLf, JsonConvert.SerializeObject(respEndpoint))
            Else
                messaggioErrore = ex.ToString()
            End If

            logErrori.Add(messaggioErrore)

            If logSuFileSystem Then
                Scrivi_LOG(_objParametriServer, nomeRoutine, messaggioErrore, CustomLOGParams:=customLOGParams)
            End If
        End Try

        Return logErrori

    End Function

    Private Function ChiamaWS(ByVal json As String, ByVal endpoint As String) As Http.JsonRestResponse

        Dim respEndpoint As New Http.JsonRestResponse()

        If String.IsNullOrEmpty(_token) OrElse _tokenScaduto Then
            If _tokenScaduto = True Then
                _ritentaTokenNonValido = False
            End If

            OttieniToken()
        End If

        Try

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("X-BC-Authorization", _token)

            'TODO Valutare se usare invece la funzione chiamaWS_RestShapr
            respEndpoint = _handleRest.CallWS_RestSharp_JSON(_objSettings.BaseUrl, endpoint, "POST", json, HeadCust, "application/json", "application/json")

            If (respEndpoint.StatusCode = HttpStatusCode.Unauthorized OrElse respEndpoint.StatusCode = HttpStatusCode.BadRequest) AndAlso _ritentaTokenNonValido = True Then
                _tokenScaduto = True
                respEndpoint = ChiamaWS(json, endpoint)
            Else
                _ritentaTokenNonValido = True
            End If

        Catch ex As Exception
            Throw New Exception("Errore WS: " & JsonConvert.SerializeObject(respEndpoint), ex)
        End Try

        Return respEndpoint

    End Function

    Private Sub OttieniToken()

        Dim jsonAuth As New JObject()
        jsonAuth.Add("User", _objConfigServizio.Username)
        jsonAuth.Add("Key", _objSettings.PasswordHash)

        Dim respToken As New Http.JsonRestResponse

        Try
            respToken = _handleRest.CallWS_RestSharp_JSON(_objSettings.BaseUrl, _objSettings.AuthEndpoint, "POST", jsonAuth.ToString(Formatting.None), Nothing, "application/json", "application/json")

            if respToken.Content Is Nothing Then
                Throw New Exception("Il WS ha restituito un token null")
            End If

            _token = CStr(respToken.Content)
            _tokenScaduto = False

            Scrivi_LOG(_objParametriServer, "OttieniToken", $"Token ottenuto: {_token}", CustomLOGParams:=customLOGParams)

        Catch ex As Exception
            Throw New Exception("Errore WS token: " & JsonConvert.SerializeObject(respToken), ex)
        End Try

    End Sub

End Class
