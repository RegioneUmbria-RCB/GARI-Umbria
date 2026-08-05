Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Integrazione2c.Attivo
Imports AgronicaCoreEFatturaBIZ.Persisters
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class FatturaAttivaController

    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtenti As AgronicaCoreParametri

    Private ReadOnly _reader As IDataReader
    Private ReadOnly _mapper As IFatturaMapper
    Private ReadOnly _fileSystemPersister As IPersister
    Private ReadOnly _logHelper As SDI_Log_Helper
    Private ReadOnly _memoryPersister As MemoryPersister
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _logger As EFatturaLogger

    Private ReadOnly _soapControllerCicloAttivo As ISOAPControllerCicloAttivo

    Public Sub New(ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri,
                   ByVal reader As IDataReader,
                   ByVal mapper As IFatturaMapper,
                   ByVal fileSystemPersister As IPersister,
                   ByVal logHelper As SDI_Log_Helper,
                   ByVal soapControllerCicloAttivo As ISOAPControllerCicloAttivo,
                   ByVal fileManager As IFIleManager,
                   ByVal logger As EFatturaLogger
        )

        _objParametriSuperServer = objParametriServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _reader = reader
        _mapper = mapper
        _fileSystemPersister = fileSystemPersister
        _logHelper = logHelper
        _soapControllerCicloAttivo = soapControllerCicloAttivo
        _fileManager = fileManager
        _logger = logger
        _memoryPersister = New MemoryPersister(_logger)

    End Sub

    Public Function CheckPreliminari(ByRef errore As String) As Boolean

        Dim nomeProcedura = "FatturaAttivaController.CheckPreliminari"

        If _soapControllerCicloAttivo Is Nothing Then
            errore = "Cliente non abilitato"
            _logger.Logga(nomeProcedura, "Cliente non abilitato")
            Return False
        End If

        'controlla se servizio 2c Running
        If Not _soapControllerCicloAttivo.ContattoHub() Then
            errore = "La chiamata ContattoHub è fallita o ha dato esito negativo"
            _logger.Logga(nomeProcedura, errore)
            Return False
        End If

        'controlla se cliente abilitato
        If Not _soapControllerCicloAttivo.CheckClienteFatturaPA(errore) Then
            _logger.Logga(nomeProcedura, errore)
            Return False
        End If

        Return True

    End Function
    Public Function LeggiEsitiFatture(ByVal filtro As AgendaFiltroLettura) As List(Of String)

        Dim nomeProcedura = "FatturaAttivaController.LeggiEsitiFatture"
        Dim result = New List(Of String)

        Dim filtroFatture = New SDI_Log_Filter With
        {
            .StatoGias = StatoFattura_Gias.InviatoSDI_OK,
            .StatoSDI = StatoFattura_SDI.EsitoNonAncoraDisponibile,
            .PIVA = filtro.PIVA
        }

        Try

            _logger.Logga(nomeProcedura, "Ricevuta richiesta di Lettura esiti fatture")

            Dim fatturaInAttesaEsito = _logHelper.Leggi(filtroFatture)

            If Not fatturaInAttesaEsito.Any() Then
                _logger.Logga(nomeProcedura, "Non ci sono fatture in attesa di esito")
            End If

            fatturaInAttesaEsito.ToList().ForEach(Sub(log)

                                                      Try

                                                          Dim daProcessare As Boolean = True

                                                          If Not filtro.Ids Is Nothing AndAlso filtro.Ids.Any Then
                                                              If Not filtro.Ids.Contains(log.Id_Agenda) Then daProcessare = False
                                                          End If
                                                          If daProcessare Then
                                                              Dim response As InvoiceOutcomeResponseWrapper = Nothing
                                                              If _soapControllerCicloAttivo.LeggiEsitiFattura(log.IDSDI, response) Then

                                                                  ' sblocca agenda
                                                                  If Not response Is Nothing AndAlso
                                                                                 response.StatoSDI <> StatoFattura_SDI.NonDefinito AndAlso
                                                                                 response.StatoSDI <> StatoFattura_SDI.EsitoNonAncoraDisponibile Then

                                                                      Dim dal = New Agenda_W(_objParametriServer)
                                                                      dal.BloccoAgenda(log.Id_Agenda, enum_Blocco_Flag.NonBloccato)

                                                                  End If

                                                                  AggiornaSDILogDopoLetturaEsito(log, response)
                                                              End If
                                                          End If

                                                      Catch ex As Exception
                                                          result.Add(ex.Message)
                                                          _logger.Logga(nomeProcedura, ex)
                                                      End Try

                                                  End Sub)

            _logger.Logga(nomeProcedura, "Richiesta di Lettura esiti fatture evasa")

        Catch ex As Exception
            _logger.Logga(nomeProcedura, ex)
            result.Add(ex.Message)
        End Try

        Return result

    End Function

    Public Function InviaFatture2C(ByVal pivaSuperUser As String,
                                   ByVal filtro As AgendaFiltroLettura,
                                   ByVal debug As Boolean
                                   ) As List(Of String)

        Const nomeProcedura = "FatturaAttivaController.InviaFatture2C"
        Dim result As New List(Of String)

        _logger.Logga(nomeProcedura, "Ricevuta richiesta di Invio Fatture al SDI")

        If debug = False Then
            Try
                ' rilancio la GenerazioneXML che
                ' 1) sistema eventuali buchi nelle fatture
                ' 2)eventualmente genera file fatture inserite ma non ancora generate

                Dim configMin As FatturaElettronicaServiceConfigMin = Nothing
                Dim codCliente As Integer = Int32.MinValue

                Dim confServizi = _reader.LeggiConfigurazioneServizi(pivaSuperUser, enum_Tipi_Servizi_Background.EFattura_Generazione_XML)
                If confServizi Is Nothing OrElse Not confServizi.Any() Then
                    Dim messaggio As String = "Configurazione Servizio  per Generazione XML fatture elettroniche non trovata"
                    _logger.Logga(nomeProcedura, messaggio)
                    result.Add(messaggio)
                    Return result
                End If

                For Each cs As Configurazione_Servizio In confServizi
                    configMin = JsonConvert.DeserializeObject(Of FatturaElettronicaServiceConfigMin)(cs.Parametri_Extra)
                    codCliente = cs.Id_Cod_Cliente
                    If configMin.PIVA.Equals(filtro.PIVA) Then Exit For
                Next

                If configMin Is Nothing Then
                    Dim messaggio As String = "Configurazione Servizio non trovata"
                    _logger.Logga(nomeProcedura, messaggio)
                    result.Add(messaggio)
                    Return result
                End If

                Me.GeneraFatture(filtro, codCliente, pivaSuperUser, configMin.PostValidazioneXML, configMin.EscludiFattureEstere, configMin.PeriodoControlloGG)

            Catch e As NumerazionFattureException
                result.Add(e.Message)
                result.Add("ATTENZIONE!!!! L'invio delle fatture non verrà effettuato a causa di problemi rilevati nella numerazione delle fatture.")
                Return result
            Catch e As Exception
                result.Add(e.Message)
                result.Add("ATTENZIONE!!!! L'invio delle fatture non verrà effettuato a causa di problemi rilevati nella numerazione delle fatture.")
                Return result
                _logger.Logga(nomeProcedura, e.Message)
            End Try
        End If

        Dim files = _fileManager.LeggiFilesDaSpedire()
        If files.Count = 0 Then
            _logger.Logga(nomeProcedura, "Non è stato trovato nessun file da inviare")
            Return result
        Else
            _logger.Logga(nomeProcedura, String.Format("Trovati {0} files da inviare", files.Count()))
        End If

        Dim contatoreSpediti As Integer = 0

        Dim agendeDaSpedire As New List(Of Object)

        Try

            For Each f As IO.FileInfo In files
                Dim nomeFile = f.Name
                Dim asl = _logHelper.LeggiConAgenda(New SDI_Log_Filter With
                                                                        {
                                                                        .NomeFileXML = nomeFile,
                                                                        .PIVA = filtro.PIVA
                                                                        }).FirstOrDefault()
                If Not asl Is Nothing Then
                    agendeDaSpedire.Add(asl)
                End If

            Next

        Catch ex As Exception
            result.Add(ex.Message)
            _logger.Logga(nomeProcedura, ex)
            Return result
        End Try

        'imposta codice di raggruppamento
        agendeDaSpedire.ForEach(Sub(a) a.Agenda.ImpostaCodiceRaggruppamento())

        'raggruppo le agende per sezionale 
        Dim agendeRaggruppate = (From a In agendeDaSpedire
                                 Order By a.Log.Doc_numero Ascending
                                 Group By CodRag = a.Agenda.CodiceRaggruppamento
                                  Into ag = Group, Count()
                                 Order By CodRag).ToList()

        For Each rag In agendeRaggruppate

            For Each lav As Object In rag.ag

                Dim sdiLog = DirectCast(lav.Log, SDI_Log)
                Dim agenda = DirectCast(lav.Agenda, AgendaXLavCod)

                Try
                    If Not sdiLog Is Nothing Then

                        Dim nomeFile = sdiLog.NomeFileXML
                        Dim daProcessare As Boolean = True

                        If Not filtro.Ids Is Nothing AndAlso filtro.Ids.Any Then
                            If Not filtro.Ids.Contains(sdiLog.Id_Agenda) Then daProcessare = False
                        End If

                        If daProcessare Then
                            'check di sicurezza
                            If sdiLog.Gias_Status = StatoFattura_Gias.InviatoSDI_OK AndAlso
                                          sdiLog.SDI_Status <> StatoFattura_SDI.RicevutaScato Then
                                ' significa che già stato inviato con esito positivo
                                daProcessare = False
                                _logger.Logga(nomeProcedura, String.Format("Tentativo di invio di un file già inviato allo SDI con esito positivo. Nome del File {0}", sdiLog.NomeFileXML))
                            End If
                        End If

                        If daProcessare Then

                            'controlla se agenda provvisoria non la spedisce e passa al sezionale successivo
                            ' alla prima in standby mi fermo
                            If agenda.Blocco_Flag = enum_Blocco_Flag.Standby Then
                                Dim messaggio = String.Format("L'agenda con numerazione {0} si trova in stato provvisorio e non può essere inviata così come le successive agende appartenenti allo stesso sezionale!", agenda.ToString())
                                _logger.Logga(nomeProcedura, String.Format(messaggio, agenda.ToString()))
                                result.Add(messaggio)
                                Exit For
                            End If

                            Dim response As SendInvoiceResponseWrapper = Nothing
                            If _soapControllerCicloAttivo.InviaFattura(nomeFile, response) Then

                                If Not response.SoapResponse Is Nothing AndAlso response.SoapResponse.ResultCode = ResultCode.Success Then

                                    'blocco agenda e sposto xml in spediti
                                    Dim dal = New Agenda_W(_objParametriServer)
                                    dal.BloccoAgenda(sdiLog.Id_Agenda, enum_Blocco_Flag.Bloccato)

                                End If
                                AggiornaSDILogDopoSpedizione(sdiLog, response)
                                contatoreSpediti += 1

                                If Not response.SoapResponse Is Nothing AndAlso response.SoapResponse.ResultCode = ResultCode.Success Then
                                    _fileManager.SpostaXMLInSpediti(nomeFile)
                                End If

                                'in ogni caso se la spedizione e avvenuta (con o senza errori) elimino lo zip perché al tentativo successivo
                                'verrà assegnato un nuovo zip file name da parte di 2c
                                '_fileManager.EliminaZip(response.NomeFileZip)

                            Else

                                If Not response Is Nothing AndAlso Not String.IsNullOrEmpty(response.Errore) Then
                                    Throw New Exception(response.Errore)
                                End If

                            End If

                        End If
                    End If

                Catch ex As Exception
                    Dim messaggio = String.Format("L'invio dell'agenda con numerazione {0} è andato in errore. Le successive agende appartenenti allo stesso sezionale non verranno inviate!", agenda.ToString())
                    result.Add(messaggio)
                    _logger.Logga(nomeProcedura, String.Format(messaggio, agenda.ToString()))
                    result.Add(ex.Message)
                    _logger.Logga(nomeProcedura, ex)
                    Exit For
                End Try

            Next

        Next

        _logger.Logga(nomeProcedura, String.Format("Richiesta di Invio Fatture evasa. Inviati {0} di {1} ", contatoreSpediti, files.Count()))

        Return result

    End Function

    Private Function ClienteSenzaInvioSDI(ByVal pivaSuperUser As String) As Boolean

        Const nomeProcedura = "FatturaAttivaController.ClienteSenzaInvioSDI"
        Dim result As Boolean = False

        Try

            Dim configMin As FatturaElettronicaServiceConfigMin = Nothing

            Dim confServizi = _reader.LeggiConfigurazioneServizi(pivaSuperUser, enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi)
            If confServizi Is Nothing OrElse Not confServizi.Any() Then
                ' non esiste la riga di configurazione servizi per l'invio
                result = True
            Else
                ' esiste la riga di configurazione servizi per l'invio ma non è abilitata
                Dim configurazioneInvio As Configurazione_Servizio = confServizi.FirstOrDefault
                If Not configurazioneInvio.Attivo Then
                    result = True
                End If

            End If

        Catch e As Exception
            _logger.Logga(nomeProcedura, e.Message)
        End Try

        Return result

    End Function

    Public Function GeneraFatture(ByVal filtro As AgendaFiltroLettura,
                                  ByVal id_cod_cliente As Integer,
                                  ByVal pivaSuperUser As String,
                                  Optional ByVal postValidazione As Boolean = False,
                                  Optional ByVal escludiFattureEstero As Boolean = False,
                                  Optional ByVal periodoControlloGG As Integer = 14
                                  ) As List(Of String)

        Const nomeProcedura = "FatturaAttivaController.GeneraFatture"
        Dim riepilogo = New GenerazioneXMLResult()
        Dim errori = New List(Of String)
        Dim xmlValidator As XMLValidator = Nothing

        Try

            _logger.Logga(nomeProcedura, "Ricevuta richiesta di Generazione Fatture")

            If postValidazione Then
                xmlValidator = New XMLValidator(_fileManager)
                xmlValidator.Inizialize()
            End If

            Dim dataAttivazione = _reader.LeggiDataAttivazione(filtro.PIVA)
            If dataAttivazione = DateTime.MinValue Then
                Dim messaggio = "Cliente non autorizzato alla fatturazione elettronica"
                _logger.Logga(nomeProcedura, messaggio)
                errori.Add(messaggio)
                Return errori
            End If

            ' Controllo se ci sono fatture in stato 400 (XmlGenerato) o in stato 403 (InviatoSDI_KO) più vecchie di 
            ' periodoControlloGG (default 14 gg)
            Dim agendeDaControllare = _reader.LeggiAgendeErrateDopoXGiorni(filtro.PIVA, periodoControlloGG)
            If agendeDaControllare.Generate.Count > 0 OrElse agendeDaControllare.InviatoSDI_KO.Count > 0 Then
                Dim messaggioWarning = ComponiMessaggioAgendeDaControllare(agendeDaControllare, periodoControlloGG)
                errori.Add(messaggioWarning)
            End If

            filtro.DataDal = dataAttivazione.Date

            ' elimino dal log di quelli già generati quelli in stato gias 402 (XMLDaRigenerare)
            Dim daRigenerare = _logHelper.Leggi(New SDI_Log_Filter With
                             {
                                .PIVA = filtro.PIVA,
                                .StatoGias = StatoFattura_Gias.XMLDaRigenerare
                             })
            If Not daRigenerare Is Nothing AndAlso daRigenerare.Any Then
                daRigenerare.ForEach(Sub(a) EliminaAgenda(a))
            End If

            'carico dati principali agende da processare
            Dim agende = _reader.LeggiAgendeXLavCod(filtro)
            riepilogo.TotaleDaProcessare = agende.DaGenerare.Count()

            '2 processa agende da generare (sono comprese anche quelle modificate da rigenerare)
            agende.DaGenerare.ForEach(Sub(a) a.ImpostaCodiceRaggruppamento())

            'raggruppo le agende 
            Dim agendeRaggruppate = (From a In agende.DaGenerare
                                     Order By a.DataMovimento Ascending, a.Doc_numero Ascending
                                     Group By CodRag = a.CodiceRaggruppamento
                                  Into ag = Group, Count()
                                     Order By CodRag).ToList()

            _mapper.Inizializza(filtro.PIVA, agende.DaGenerare)

            For Each rag In agendeRaggruppate

                For Each lav As AgendaXLavCod In rag.ag

                    _logger.Logga(nomeProcedura, String.Format("Generazione fattura n. {0}", lav.NumeroDocumento))

                    ' alla prima in standby mi fermo
                    If lav.Blocco_Flag = enum_Blocco_Flag.Standby Then
                        Dim messaggio = String.Format("L'agenda con numerazione {0} si trova in stato da verificare. Le successive agende appartenenti allo stesso sezionale non verranno processate!", lav.ToString())
                        _logger.Logga(nomeProcedura, String.Format(messaggio, lav.IdAgenda))
                        errori.Add(messaggio)
                        Exit For
                    End If

                    ' se non bloccata procedo a generare xml
                    If lav.Blocco_Flag = enum_Blocco_Flag.NonBloccato Then
                        Dim fg = _reader.LeggiAgenda(lav, _mapper.DecodificheMapper.TipoImpresaGerarchia)
                        Dim result = GeneraFattura(fg, lav, id_cod_cliente, riepilogo, xmlValidator, escludiFattureEstero)
                        If Not String.IsNullOrEmpty(result) Then
                            Dim messaggio = String.Format("La generazione dell'agenda con numerazione {0} è terminata con errori. Le successive agende appartenenti allo stesso sezionale per lo stesso anno {1} non verranno processate!", lav.ToString(), lav.Anno)
                            _logger.Logga(nomeProcedura, messaggio)
                            errori.Add(messaggio)
                            messaggio = String.Format("L'errore è il seguente: {0}", result)
                            errori.Add(messaggio)
                            _logger.Logga(nomeProcedura, messaggio)
                            Exit For
                        End If
                    Else
                        Dim messaggio = "Trovata agenda con id {0} in stato bloccato. Il file XML non verrà generato."
                        _logger.Logga(nomeProcedura, String.Format(messaggio, lav.IdAgenda))
                        riepilogo.TotaleBloccate += 1
                    End If

                Next

            Next

            If riepilogo.TotaleEstereNonGenerate > 0 Then
                errori.Add("ATTENZIONE. Sono state trovate fatture estere la cui generazione è stata saltata come da impostazione utente.")
            End If

            ' procedo alla cancellazione delle agende che sono state cancellate (sia file fisici che righe di log)
            ' procedo inoltre alla eliminazione di eventuali buchi
            '1 elimina record di log e file xml di agende eliminate

            ' NB: il controllo viene saltato se il cliente non fa invio allo SDI con Agronica
            Dim nonInviaAlloSDI As Boolean = ClienteSenzaInvioSDI(pivaSuperUser)
            If Not nonInviaAlloSDI Then
                RisolviBuchiNumerazione(agende.Eliminate, riepilogo)
            End If

            _logger.Logga(nomeProcedura, "Richiesta di Generazione Fatture evasa")
            _logger.Logga(nomeProcedura, riepilogo.ToString())

        Catch ex As NumerazionFattureException
            errori.Add(ex.Message)
            _logger.Logga(nomeProcedura, ex)
            Throw ex
        Catch ex As Exception
            errori.Add(ex.Message)
            _logger.Logga(nomeProcedura, ex)
        End Try

        Return errori

    End Function

    Private Sub RisolviBuchiNumerazione(ByVal daEliminare As List(Of AgendaXLavCod), ByVal riepilogo As GenerazioneXMLResult)

        Dim nomeProcedura = "FatturaAttivaController.RisolviBuchiNumerazione"

        Try
            Dim statoEliminazione = String.Empty
            If Not daEliminare Is Nothing AndAlso daEliminare.Any() Then

                _logger.Logga(nomeProcedura, "ATTENZIONE. Rilevate fatture eliminate. Avvio processo di controllo per buchi nella numerazione delle fatture")

                riepilogo.TotaleDaEliminare = daEliminare.Count()
                daEliminare.ForEach(Sub(a)
                                        Dim agendeStesseSezionale = _logHelper.LeggiPerSezionale(a.PIVA, a.Doc_NUmero_Sin, a.Doc_NUmero_Des, a.ID_LOg)
                                        If Not agendeStesseSezionale Is Nothing AndAlso agendeStesseSezionale.Any() Then

                                            _logger.Logga(nomeProcedura, ComponiMessaggioLogPerBuchiNumerazione(a, agendeStesseSezionale))

                                            agendeStesseSezionale.ForEach(Sub(ss)
                                                                              statoEliminazione = EliminaAgenda(ss)
                                                                              If String.IsNullOrEmpty(statoEliminazione) Then
                                                                                  riepilogo.TotaleEliminate += 1
                                                                              Else
                                                                                  Throw New Exception(statoEliminazione)
                                                                              End If
                                                                          End Sub)
                                        End If
                                        statoEliminazione = EliminaAgenda(a)
                                        If String.IsNullOrEmpty(statoEliminazione) Then
                                            riepilogo.TotaleEliminate += 1
                                        Else
                                            Throw New Exception(statoEliminazione)
                                        End If
                                    End Sub)
            End If

        Catch ex As Exception
            Throw New NumerazionFattureException(ex.Message)
        End Try

    End Sub

    Private Function ComponiMessaggioAgendeDaControllare(ByVal agendeDaControllare As ControlloPreliminare, ByVal periodoControlloGG As Integer) As String

        Dim sb As New StringBuilder()

        If agendeDaControllare.InviatoSDI_KO.Any Then
            sb.AppendFormat(String.Format("Rilevate fatture più vecchie di {0} gg con Invio allo SDI in stato Errato", periodoControlloGG))
            sb.AppendLine()
            For Each A As AgendaXLavCod In agendeDaControllare.InviatoSDI_KO
                sb.AppendLine(String.Format(" - Fattura {0}", A.ToString()))
            Next
        End If

        If agendeDaControllare.Generate.Any Then
            sb.AppendFormat(String.Format("Rilevate fatture più vecchie di {0} gg che sono in stato XmlGenerato ma non inviate allo SDI", periodoControlloGG))
            sb.AppendLine()
            For Each A As AgendaXLavCod In agendeDaControllare.Generate
                sb.AppendLine(String.Format(" - Fattura {0}", A.ToString()))
            Next
        End If

        sb.AppendLine(" CONTATTARE IL SUPPORTO TECNICO PER LE OPPORTUNE VERIFICHE !!!")
        Return sb.ToString

    End Function
    Private Function ComponiMessaggioLogPerBuchiNumerazione(ByVal daEliminare As AgendaXLavCod, ByVal collegate As List(Of SDI_Log)) As String

        Dim sb As New StringBuilder()

        sb.AppendFormat("Rilevata agenda eliminata -> {0}", daEliminare.ToString())
        sb.AppendLine()
        If collegate.Any() Then
            sb.AppendLine("Verranno eliminate le seguenti agende facenti parte dello stesso sezionale perchè successive:")
            collegate.ForEach(Sub(c) sb.AppendLine(_logHelper.ToString(c)))
        End If

        Return sb.ToString()

    End Function
    Private Function EliminaAgenda(ByVal log As SDI_Log) As String

        Dim momeProcedura As String = "FatturaAttivaController.EliminaAgenda"
        Dim returnValue = String.Empty

        Try

            If log.Gias_Status = StatoFattura_Gias.InLock Then
                Return String.Format("Non è stato possibile eliminare il file {0} relativo all'agenda con id {1} perchè era in lock da un altro processo", log.NomeFileXML, log.Id_Agenda)
            End If

            If log.Gias_Status = StatoFattura_Gias.InviatoSDI_OK OrElse log.Gias_Status = StatoFattura_Gias.InviatoSDI_KO OrElse log.Gias_Status = StatoFattura_Gias.XMLEsportato Then
                Return String.Format("Non è stato possibile eliminare il file {0} relativo all'agenda con id {1} perchè è già stato inviato allo SDI oppure Esportato", log.NomeFileXML, log.Id_Agenda)
            End If

            'elimino file xml (se non riesce a eliminare va in eccezione)
            If Not String.IsNullOrEmpty(log.NomeFileXML) Then
                _fileManager.EliminaXML(log.NomeFileXML)
            End If

            'elimino record da tabella log
            _logHelper.Elimina(log)

        Catch ex As Exception
            returnValue = ex.Message
            _logger.Logga(momeProcedura, returnValue)
        End Try

        Return returnValue

    End Function
    Private Function EliminaAgenda(ByVal lav As AgendaXLavCod) As String

        Dim momeProcedura As String = "FatturaAttivaController.EliminaAgenda"
        Dim returnValue = String.Empty

        Try

            Dim log As SDI_Log = _logHelper.Leggi(New SDI_Log_Filter With
                                {
                                    .PIVA = lav.PIVA,
                                    .Id_Agenda = lav.IdAgenda
                                }).FirstOrDefault()

            If log Is Nothing Then
                Return String.Format("Record SDI_LOG non trovato per agenda con id {0}", lav.IdAgenda)
            End If

            If log.Gias_Status = StatoFattura_Gias.InLock Then
                Return String.Format("Non è stato possibile eliminare il file {0} relativo all'agenda con id {1} perchè era in lock da un altro processo", log.NomeFileXML, log.Id_Agenda)
            End If

            If log.Gias_Status = StatoFattura_Gias.InviatoSDI_OK OrElse log.Gias_Status = StatoFattura_Gias.InviatoSDI_KO OrElse log.Gias_Status = StatoFattura_Gias.XMLEsportato Then
                Return String.Format("Non è stato possibile eliminare il file {0} relativo all'agenda con id {1} perchè è già stato inviato allo SDI oppure Esportato", log.NomeFileXML, log.Id_Agenda)
            End If

            'elimino file xml (se non riesce a eliminare va in eccezione)
            _fileManager.EliminaXML(log.NomeFileXML)

            'elimino record da tabella log
            _logHelper.Elimina(log)

        Catch ex As Exception
            returnValue = ex.Message
            _logger.Logga(momeProcedura, returnValue)
        End Try

        Return returnValue

    End Function

    Private Function GeneraFattura(ByVal f As FatturaGiasFromDB,
                              ByVal agenda As AgendaXLavCod,
                              ByVal id_cod_cliente As Integer,
                              ByVal riepilogo As GenerazioneXMLResult,
                              ByVal xmlValidaror As XMLValidator,
                              ByVal escludiFattureEstero As Boolean) As String

        Const nomeProcedura = "FatturaAttivaController.GeneraFattura"

        Dim numeroFattura As String = f.OttieniNumeroFattura()
        Dim tipoFattura As String = f.OttieniTipoDocumento(_mapper.DecodificheMapper).ToString()
        Dim docNumeroSin As String = f.DatiFattura.Testata.Doc_Numero_Sin
        Dim docNumero As Integer = f.DatiFattura.Testata.Doc_Numero
        Dim docNumeroDes As String = f.DatiFattura.Testata.Doc_Numero_Des

        Dim tipoTracciato = String.Empty
        Dim log As SDI_Log = Nothing
        Dim result As String = String.Empty

        Try

            Dim errori = New List(Of String)

            Dim statoPrecedente As StatoFattura_Gias = StatoFattura_Gias.NonDefinito
            Dim lockAcquisito = _logHelper.AcquisisciLock(New SDI_Log_Filter With
                                            {
                                                .PIVA = agenda.PIVA,
                                                .Id_Agenda = agenda.IdAgenda
                                            }, agenda, log, statoPrecedente)

            If lockAcquisito Then

                'riversa solo il contenuto delle query anonime su una entità tipizzata
                Dim fatturaGias = f.MappaFatturaGias(_mapper)

                'Se fattura appartiene a un sezionale escluso da fatturazione non la genero
                Dim sezionale = _mapper.DecodificheMapper.OttieniSezionale(fatturaGias.Fattura.Testata.Sezionale_Cod)
                If sezionale Is Nothing OrElse sezionale.Fatturazione <> 1 Then
                    _logHelper.Elimina(log)
                    Return String.Empty
                End If

                'Se fattura verso cliente estero controlla se deve generarla o meno
                If Not GeneraFatturaPerEstero(fatturaGias, escludiFattureEstero) Then
                    Dim messaggio = String.Format("Trovata fattura estera ({0}) che non è stata generata per impostazione utente!", agenda.ToString())
                    _logger.Logga(nomeProcedura, messaggio)
                    _logHelper.Elimina(log)
                    riepilogo.TotaleEstereNonGenerate += 1
                    Return String.Empty
                End If

                tipoTracciato = fatturaGias.OttieniFormatoTrasmissione().ToString()

                Dim fatturaValida = fatturaGias.IsValid(_mapper.DecodificheMapper, errori)
                If Not fatturaValida Then
                    Dim messaggio As String = String.Format("Check preliminari falliti per fattura con id {0} e numero documento {1}. Il file non è stato generato", agenda.IdAgenda, agenda.NumeroDocumento)
                    ' il file xml conterebbe errori quindi non lo salvo su disco ma loggo solamente
                    _logHelper.LoggaXmlNonGenerabile(tipoFattura, tipoTracciato, errori, log)
                    _logger.Logga(nomeProcedura, messaggio)
                    riepilogo.TotaleErroriPrecheck += 1
                    result = messaggio
                Else

                    ' Decodifico tutte le descrizioni dei prodotti di dettaglio 
                    fatturaGias.Fattura.Dettagli.ProdottiServizi.ForEach(
                                                         Sub(ps)

                                                             If ps.Movimento.Elem_Cod = RIGA_DESCRIZIONE_LIBERA Then
                                                                 ps.Descrizione = ps.Movimento.Mov_Det_Des.Replace("§", "")
                                                                 ps.Flag_Extra = False
                                                                 ps.DescrizioneLibera = True
                                                             Else

                                                                 Dim flag_extra As Boolean = False
                                                                 Dim descrizione = _reader.LeggiDescrizioneBeneServizio(ps.Movimento, flag_extra, _mapper.ModuloGenerazione)
                                                                 If Not String.IsNullOrEmpty(ps.Movimento.Extra_Str) Then
                                                                     descrizione = String.Format("{0} - {1}", descrizione, ps.Movimento.Extra_Str)
                                                                 End If
                                                                 ps.Descrizione = descrizione.Replace("§", "")
                                                                 ps.Flag_Extra = flag_extra
                                                                 ps.DescrizioneLibera = False
                                                             End If
                                                         End Sub
                                                    )

                    fatturaGias.ProgressivoFattura = OttieniProgressivoDocumento(log, fatturaGias.Fattura.PIVA)

                    Dim nomeFile = fatturaGias.OttieniNomeFileXML()

                    'mapping vero e proprio su fattura secondo specifiche AE
                    Dim fat = _mapper.MappaFatturaAttiva(fatturaGias, f.FatturaType, id_cod_cliente)

                    ' il post validatore potrebbero averlo solo i clienti che hanno abilitato solo la generazione dei file xml
                    Dim postValidazioneOk As Boolean = True
                    If Not xmlValidaror Is Nothing Then
                        Dim fatMemStream = _memoryPersister.ToMemoryStream(fat, f.FatturaType)
                        postValidazioneOk = xmlValidaror.Validate(fatMemStream, errori)
                    End If

                    If postValidazioneOk Then
                        _fileSystemPersister.Persist(fat, f.FatturaType, nomeFile)
                        'scrittura su tabella di log (se esisteva già ma non era ancora stato spedito non cambia il progressivo
                        ' e aggiorna data generazione xml
                        LoggaFileXmlGenerato(fatturaGias, nomeFile, log)
                        riepilogo.TotaleGenerati += 1
                    Else
                        ' il file xml conterebbe errori formali di validazione con xsd  quindi non lo salvo su disco ma loggo solamente
                        LoggaFileXmlNonGenerabile(docNumeroSin, docNumero, docNumeroDes, tipoFattura, tipoTracciato, errori, log)
                        _logger.Logga(nomeProcedura, String.Format("Check finali falliti per fattura con id {0} e numero documento {1}. Il file non è stato generato", agenda.IdAgenda, agenda.NumeroDocumento))
                        riepilogo.TotaleErroriPrecheck += 1

                        'compone messaggio errore per interfaccio
                        Dim sb = New StringBuilder()
                        errori.ForEach(Sub(e) sb.AppendLine(e))
                        result = sb.ToString()
                    End If

                End If

            Else
                _logger.Logga(nomeProcedura, String.Format("Lock non acquisito per fattura con id {0} e numero documento {1}", agenda.IdAgenda, agenda.NumeroDocumento))
            End If

        Catch ex As Exception When TypeOf ex Is DBToEntityMapException OrElse TypeOf ex Is GiasEntityToEFatturaMapException
            LoggaFileXmlNonGeneratoPerErrori(docNumeroSin, docNumero, docNumeroDes, tipoFattura, tipoTracciato, ex.Message, log)
            _logger.Logga(nomeProcedura, String.Format("Errore durante generazione del file xml per fattura con id {0} e numero documento {1}", agenda.IdAgenda, agenda.NumeroDocumento))
            _logger.Logga(nomeProcedura, ex)
            riepilogo.TotaleErroriEccezione += 1
            result = ex.Message
        Catch ex As Exception
            LoggaFileXmlNonGeneratoPerErrori(docNumeroSin, docNumero, docNumeroDes, tipoFattura, tipoTracciato, ex.Message, log)
            _logger.Logga(nomeProcedura, String.Format("Errore durante generazione del file xml per fattura con id {0} e numero documento {1}", agenda.IdAgenda, agenda.NumeroDocumento))
            _logger.Logga(nomeProcedura, ex)
            riepilogo.TotaleErroriEccezione += 1
            result = ex.Message
        End Try

        Return result

    End Function

    Private Function GeneraFatturaPerEstero(ByVal fattura As FatturaGias, ByVal escludiFattureEstero As Boolean) As Boolean

        'se cliente italiano italiano genero sempre xml
        If Not fattura.ClienteEstero() Then
            Return True
        Else
            'se cliente estero genero solo se parametro < generaFattureEstero > true oppure false ma con stabile organizzazione
            If Not escludiFattureEstero Then
                Return True
            Else
                If Not fattura.Cessionario.StabileOrganizzazione Is Nothing Then
                    Return True
                End If
            End If
        End If

        Return False

    End Function

    Private Sub LoggaFileXmlNonGenerabile(ByVal docNumeroSin As String,
                                         ByVal docNumero As Integer,
                                         ByVal docNumeroDes As String,
                                         ByVal tipoFattura As String,
                                         ByVal tipoTracciato As String,
                                         ByVal errori As List(Of String),
                                         ByVal log As SDI_Log)


        log.Doc_Numero_Sin = docNumeroSin
        log.Doc_Numero = docNumero
        log.Doc_Numero_Des = docNumeroDes
        _logHelper.LoggaXmlNonGenerabile(tipoFattura, tipoTracciato, errori, log)

    End Sub

    Private Sub LoggaFileXmlNonGeneratoPerErrori(ByVal docNumeroSin As String,
                                         ByVal docNumero As Integer,
                                         ByVal docNumeroDes As String,
                                         ByVal tipoFattura As String,
                                         ByVal tipoTracciato As String,
                                         ByVal errore As String,
                                         ByVal log As SDI_Log)


        log.Doc_Numero_Sin = docNumeroSin
        log.Doc_Numero = docNumero
        log.Doc_Numero_Des = docNumeroDes
        _logHelper.LoggaXmlNonGeneratoPerErrori(tipoFattura, tipoTracciato, errore, log)

    End Sub

    Private Sub LoggaFileXmlGenerato(ByVal fattura As FatturaGias, ByVal nomeFile As String, ByVal log As SDI_Log)

        log.NomeFileXML = nomeFile
        log.TipoTracciato = fattura.OttieniFormatoTrasmissione().ToString()
        log.TipoFattura = fattura.Fattura.TipoFattura
        log.Doc_Numero_Sin = fattura.Fattura.Testata.Doc_Numero_Sin
        log.Doc_Numero = fattura.Fattura.Testata.Doc_Numero
        log.Doc_Numero_Des = fattura.Fattura.Testata.Doc_Numero_Des
        log.Data_Gen_XML = DateTime.Now
        log.Gias_Status = StatoFattura_Gias.XMLGenerato
        log.SDI_Status = StatoFattura_SDI.NonDefinito
        log.Data_Modifica = DateTime.Now

        _logHelper.Aggiorna(log)

    End Sub

    Private Function OttieniProgressivoDocumento(ByVal log As SDI_Log, ByVal PIVA As String) As Integer

        If String.IsNullOrEmpty(log.NomeFileXML) Then
            'significa che non è mai stato generato l'xml quindi ottengo un nuovo progressivo
            Return OttieniProgressivoDocumento(PIVA)
        Else
            'significa che l'xml era già stato generato ma non acora spedito
            Return CInt(log.NomeFileXML.ToLower().Replace(".xml", "").Split("_")(1))
        End If

    End Function
    Private Sub AggiornaSDILogDopoSpedizione(ByVal sdiLog As SDI_Log, ByVal response As SendInvoiceResponseWrapper)

        Dim nomeProcedura As String = "FatturaAttivaController.AggiornaSDILogDopoSpedizione"

        sdiLog.NomeFileZIP = response.NomeFileZip

        If Not response.SoapResponse Is Nothing Then

            If response.SoapResponse.ResultCode = ResultCode.Success Then
                sdiLog.IDSDI = response.SoapResponse.IdSdi
                sdiLog.Data_SDI = response.SoapResponse.DateSdi
                sdiLog.Gias_Status = StatoFattura_Gias.InviatoSDI_OK
            Else

                Dim dettagli = response.SoapResponse.ResultMessage.ToList() _
                            .Select(Function(m) New SDI_Log_Dettaglio With
                                    {
                                        .Data_Creazione = DateTime.Now,
                                        .Id_Log = sdiLog.Id_Log,
                                        .Messaggio = m,
                                        .TipoErrore = TipoErroreEFattura.Validazione2C
                                    }).ToList()

                dettagli.ForEach(Sub(d) sdiLog.SDI_Log_Dettaglio.Add(d))

                sdiLog.Gias_Status = StatoFattura_Gias.InviatoSDI_KO

                _logger.Logga(nomeProcedura, String.Format("Il file {0} è stato inviato correttamente ma contiene errori di validazione da parte del SDI", sdiLog.NomeFileXML))

            End If
        End If

        _logHelper.Aggiorna(sdiLog)

    End Sub
    Private Sub AggiornaSDILogDopoLetturaEsito(ByVal sdiLog As SDI_Log, ByVal response As InvoiceOutcomeResponseWrapper)

        sdiLog.SDI_Status = response.StatoSDI
        sdiLog.Note = response.Descrizione
        If Not response.MessaggiErrore Is Nothing AndAlso response.MessaggiErrore.Any Then
            Dim dettagli = response.MessaggiErrore _
                            .Select(Function(m) New SDI_Log_Dettaglio With
                                    {
                                        .Data_Creazione = DateTime.Now,
                                        .Id_Log = sdiLog.Id_Log,
                                        .CodiceErrore = m.Item1,
                                        .Messaggio = m.Item2,
                                        .TipoErrore = TipoErroreEFattura.ValidazioneSDI
                                    }).ToList()
            dettagli.ForEach(Sub(d) sdiLog.SDI_Log_Dettaglio.Add(d))
        End If

        _logHelper.Aggiorna(sdiLog)

    End Sub
    Private Function OttieniProgressivoDocumento(ByVal pIva As String) As Integer

        Dim dal As New Sequenza_Progressivi_R
        Dim prg = dal.Nuovo_Progressivo_UpdateImmediato(
            pIva, 0, enum_SequenzaProgressiviTipi.FatturaElettronica, "", "", 0, _objParametriServer)

        Return prg

    End Function

End Class



