Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreInterscambioBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Demetra
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL

Friend NotInheritable Class FIELD_MAPPER

    Public Shared COD_CONTATTO As String = "cod_contatto"
    Public Shared PIVA As String = "piva"
    Public Shared SA_COD As String = "sa_cod"
    Public Shared ID_CF As String = "id_cf"
    Public Shared NOME As String = "nome"
    Public Shared COGNOME As String = "cognome"
    Public Shared RAG_SOC As String = "rag_soc"
    Public Shared CODICE_FISCALE As String = "codice_fiscale"
    Public Shared COD_RAPPORTO As String = "cod_rapporto"
    Public Shared VALIDITA_INIZIO As String = "validita_inizio"
    Public Shared VALIDITA_FINE As String = "validita_fine"
    Public Shared VAL_COD As String = "val_cod"
    Public Shared ID_COD As String = "id_cod"
    Public Shared DATA_CREAZIONE As String = "data_creazione"
    Public Shared DATA_MODIFICA As String = "data_modifica"
    Public Shared USERNAME_CREAZIONE As String = "username_creazione"
    Public Shared USERNAME_MODIFICA As String = "username_modifica"

    Public Shared FLAG_CANCELLAZIONE As String = "flag_cancellazione"
    Public Shared FLAG_CANCELLABILE As String = "flag_cancellabile"

    Public Shared COD_RISUM As String = "cod_risum"
    Public Shared RAPPORTO_DES As String = "rapporto_des"
    Public Shared TIPO_INDIRIZZO As String = "tipo_indirizzo"
    Public Shared COD_INDIRIZZO As String = "cod_indirizzo"
    Public Shared IND_DES As String = "ind_des"
    Public Shared FRZ_DES As String = "frz_des"
    Public Shared CAP As String = "cap"
    Public Shared COM_DES As String = "com_des"
    Public Shared PRO_COD As String = "pro_cod"
    Public Shared STATO As String = "stato"
    Public Shared PRO_COD_ISTAT As String = "pro_cod_istat"
    Public Shared COM_COD_ISTAT As String = "com_cod_istat"
    Public Shared NOTE As String = "note"
    Public Shared CODICE_LINGUA As String = "codice_lingua"
    Public Shared CODICE_ALTERNATIVO As String = "codice_alternativo"
    Public Shared USERNAME_VALIDAZIONE As String = "username_validazione"
    Public Shared DATA_VALIDAZIONE As String = "data_validazione"
    Public Shared VALIDAZIONE As String = "validazione"
    Public Shared CONVENEVOLI As String = "convenevoli"
    Public Shared TIPO_INDIRIZZO_DEFAULT As String = "tipo_indirizzo_default"
    Public Shared DATA_NASCITA As String = "data_nascita"
    Public Shared SESSO As String = "sesso"
    Public Shared COD_CONTATTO_REFERENTE As String = "cod_contatto_referente"
    Public Shared TIPO_SPEDITORE As String = "tipo_speditore"
    Public Shared TIPO_DESTINAZIONE As String = "tipo_destinazione"
    Public Shared AGENTE_COD As String = "agente_cod"
    Public Shared PROVVIGIONE As String = "provvigione"
    Public Shared ID_GESTIONE_NOTE As String = "id_gestione_note"
    Public Shared NOTE2 As String = "note2"
    Public Shared NOTE_OPERAZIONI As String = "note_operazioni"
    Public Shared NOTE2_OPERAZIONI As String = "note2_operazioni"
    Public Shared COD_RISUM_DESTINAZIONE_DIVERSA As String = "cod_risum_destinazione_diversa"
    Public Shared TIPO_INDIRIZZO_DEFAULT_DESTINAZIONE_DIVERSA As String = "tipo_indirizzo_default_destinazione_diversa"
    Public Shared FIDO As String = "fido"
    Public Shared LIMITE_POSIZIONI As String = "limite_posizioni"
    Public Shared LIMITE_GIORNI_EVASIONE As String = "limite_giorni_evasione"
    Public Shared ORARI_RITIRO As String = "orari_ritiro"
    Public Shared FILTRO_RIMBORSI As String = "filtro_rimborsi"
    Public Shared VETTORE_COD As String = "vettore_cod"
    Public Shared CAPOAREA_COD As String = "capoarea_cod"
    Public Shared PROVVIGIONE_CAPOAREA As String = "provvigione_capoarea"
    Public Shared MEMO As String = "memo"
    Public Shared SCONTO_CONTATTO As String = "sconto_contatto"
    Public Shared SCONTO_TESTO As String = "sconto_testo"
    Public Shared MODALITA_FATTURAZIONE As String = "modalita_fatturazione"
    Public Shared COD_IVA_CONTATTO As String = "cod_iva_contatto"
    Public Shared DOCUMENTO_FATTURAZIONE As String = "documento_fatturazione"
    Public Shared NRBADGE As String = "nrbadge"
    Public Shared CHKFITTIZIO As String = "chkfittizio"
    Public Shared NOME_BREVE As String = "nome_breve"
    Public Shared COD_CONTO_ECON As String = "cod_conto_econ"
    Public Shared COD_CONTO_PAT As String = "cod_conto_pat"
    Public Shared SETTORE_DES As String = "settore_des"
    Public Shared ATTIVITA_DES As String = "attivita_des"
    Public Shared ORE_SETTIMANALI As String = "ore_settimanali"
    Public Shared GIORNI_FERIE As String = "giorni_ferie"
    Public Shared CORRISPETTIVO_MENSILE As String = "corrispettivo_mensile"
    Public Shared CORRISPETTIVO_ORARIO As String = "corrispettivo_orario"
    Public Shared OCCASIONALE As String = "occasionale"
    Public Shared PATENTINO As String = "patentino"
    Public Shared FERIE_GODUTE As String = "ferie_godute"
    Public Shared GIORNI_MALATTIA As String = "giorni_malattia"
    Public Shared DATA_RILASCIO_PATENTINO As String = "data_rilascio_patentino"
    Public Shared DATA_SCADENZA_PATENTINO As String = "data_scadenza_patentino"
    Public Shared COD_RISUM_ORIGINE As String = "cod_risum_origine"
    Public Shared PIVA_SUPERUSER_ORIGINE As String = "piva_superuser_origine"
    Public Shared ENTE_DI_RILASCIO As String = "ente_di_rilascio"
    Public Shared SALDO_INIZIALE_CREDITI As String = "saldo_iniziale_crediti"
    Public Shared SALDO_INIZIALE_DEBITI As String = "saldo_iniziale_debiti"
    Public Shared CHKSPESOMETRO As String = "chkspesometro"
    Public Shared CHKBLOCCO As String = "chkblocco"
    Public Shared BLOCCO_DES As String = "blocco_des"
    Public Shared CLASSIFICAZIONE_COD As String = "classificazione_cod"
    Public Shared QUALIFICA_COD As String = "qualifica_cod"
    Public Shared MANSIONE_COD As String = "mansione_cod"
    Public Shared INFO_FAMIGLIA As String = "info_famiglia"
    Public Shared ID_ELENCO As String = "id_elenco"
    Public Shared ID_ALERT_ENTITA As String = "id_alert_entita"
    Public Shared ALLEGATI_DOCUMENTI_NUMERO As String = "allegati_documenti_numero"
    Public Shared ALLEGATI_DOCUMENTI_COD As String = "allegati_documenti_cod"
    Public Shared ALLEGATI_DOCUMENTI_NOMEFILE As String = "allegati_documenti_nomefile"
    Public Shared DESCRIZIONE_SCADENZA As String = "descrizione_scadenza"
    Public Shared VALIDAZIONE_DATA As String = "validazione_data"
    Public Shared VALIDAZIONE_FLAG As String = "validazione_flag"
    Public Shared DATA_SCADENZA As String = "data_scadenza"
    Public Shared ALLEGATI_DOCUMENTI_ENTE_DES As String = "allegati_documenti_ente_des"
    Public Shared ID_TIPOLOGIA As String = "id_tipologia"
    Public Shared CODICE As String = "codice"
    Public Shared ALLEGATI_DOCUMENTI_NOTE As String = "note"
    Public Shared DATA_UPLOAD As String = "data_upload"
    Public Shared PIVASUPERUSER As String = "pivasuperuser"
    Public Shared TIPO_OPERAZIONE_DB As String = "tipo_operazione_db"


End Class

Public Class ImportContatti

    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Utenti As AgronicaCoreParametri = Nothing

    Private _dataLoader As D2G_Contatto_Loader = Nothing
    Private _mapper As D2G_Contatti_Import_Mapper = Nothing
    Private _contattoWriter As D2G_Contatto_Writer = Nothing
    Private _intercambio As D2G_Interscambio_Contatti = Nothing
    Private _validatore As D2G_ImportContatti_Validatore = Nothing
    Private _rapportiContabiliAmmessi As List(Of IDictionary(Of String, Object)) = Nothing

    Private _CUAA As String = String.Empty
    Private _paramAgg As ParametriInterscambioContatti = New ParametriInterscambioContatti()

    Public Sub New()

    End Sub

    Public Sub New(ByVal cuaa As String,
                   ByVal paramImport As ParametriInterscambioContatti,
                   ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)

        _CUAA = cuaa
        If paramImport IsNot Nothing Then
            _paramAgg = paramImport
        End If

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

        _dataLoader = New D2G_Contatto_Loader(objParametri_Server, objParametri_Utenti)
        _intercambio = New D2G_Interscambio_Contatti(objParametri_Server, objParametri_Utenti, _dataLoader)
        _validatore = New D2G_ImportContatti_Validatore(objParametri_Server, _dataLoader)
        _mapper = New D2G_Contatti_Import_Mapper(objParametri_Server, objParametri_Utenti, _intercambio, _dataLoader)
        _contattoWriter = New D2G_Contatto_Writer(_dataLoader, _intercambio, objParametri_Server, objParametri_Utenti)

        Inizializza()

    End Sub

    Private Sub Inizializza()

        If IsNothing(_rapportiContabiliAmmessi) OrElse _rapportiContabiliAmmessi.Count = 0 Then

            Dim xFiltroAggiuntivo = " terzista = 1 OR dipendente = 1 "
            If Not String.IsNullOrWhiteSpace(_paramAgg.FiltroAggRapportiContabili) Then
                xFiltroAggiuntivo = _paramAgg.FiltroAggRapportiContabili
            End If
            _rapportiContabiliAmmessi = _dataLoader.Leggi_Rapporti_Contabili_Ammessi(xFiltroAggiuntivo)

            If IsNothing(_rapportiContabiliAmmessi) OrElse _rapportiContabiliAmmessi.Count = 0 Then
                Throw New Exception("Non è stato possibile recuperare da Gias nessun rapporto contabile di tipo Terzista o Dipendente")
            End If

        End If

    End Sub

    Public Function Importa(ByVal cuaa As String, ByVal inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto)) As String

        'Dim flagTransazioneLocale As Boolean = False
        'Dim flagConnessioneLocale As Boolean = False
        Dim messaggio As String = String.Empty
        Dim contattoDaEliminare As Boolean = False
        Dim interscambioResult As D2G_Interscambio_Contatti_Result = Nothing

        Try
            'Lavez - 04/10/2024 - dato che apriamo connessione e transazione dentro la salva contatto ed in caso di errore dobbiamo cmq scrivere
            '                     dei log, la gestione di livello superiore non ha senso.
            'ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale,
            '                                            flagTransazioneLocale,
            '                                            _objParametri_Server)

            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim piva = xImpCodR.Piva_from_CUAA(cuaa, _objParametri_Server)

            If piva = "" Then
                messaggio = String.Format(My.Resources.AgronicaCoreDemetraBIZ.CuaaNonEsistente, cuaa)
                Return messaggio
            End If

            ' Normalizzazione oggetto del payload
            NormalizzaPayLoad(inData)

            _validatore.Controlli_Preliminari_Generali(inData)

            Dim contatto As IDictionary(Of String, Object) = Nothing
            interscambioResult = _intercambio.DammiTipoOperazione_Contatto(inData)

            ' SE IL RISULTATO DI INTERSCAMBIO MI DICE CHE SI TRATTA DI UN INSERIMENTO,
            ' PRIMA FACCIO ULTERIORE RICERCA IN TABELLA CONTATTI PER EVITARE INCROCI CON ALTRI FLUSSI CHE POTREBBERO AVERLO CREATO
            If interscambioResult.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                Controllo_Incrocio_Altri_Flussi(piva, inData, interscambioResult)
            End If

            ' Pre-validazione
            _validatore.Controlli_Preliminari_Rapporti_Contabili(inData.elemento_anagrafico, _rapportiContabiliAmmessi)
            _validatore.Controlli_Preliminari_Documenti(inData.elemento_anagrafico)

            Dim progressivoGias As Integer = 0
            Dim base, top As Integer
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            progressivoGias = objUtenti.ProgressivoGias_from_Superuser(_objParametri_Utenti)

            UtilityProvider.Calcola_BaseCode_TopCode(base, top, progressivoGias)

            _mapper.BaseCode = base
            _mapper.TopCode = top

            _contattoWriter.BaseCode = base
            _contattoWriter.TopCode = top

            Try
                Select Case interscambioResult.TipoOperazioneDB
                    Case enum_TipoOperazioneDB.Scrittura
                        contatto = _mapper.Mappa_Nuovo(inData, piva,
                                                        _objParametri_Server.PivaSuperUser,
                                                        _rapportiContabiliAmmessi,
                                                        _CUAA, interscambioResult)
                    Case enum_TipoOperazioneDB.Modifica
                        contatto = _mapper.Mappa_Esistente(inData, interscambioResult.Contatto,
                                                           piva, _objParametri_Server.PivaSuperUser,
                                                           _rapportiContabiliAmmessi,
                                                           _CUAA)
                End Select
            Catch mapEx As Exception
                messaggio = mapEx.Message
            End Try

            If IsNothing(contatto) OrElse Not String.IsNullOrEmpty(messaggio) Then
                Throw New Exception(messaggio)
            End If

            ' Prima di aprire la transazione, se ci sono rapporti contabili da eliminare verifico se possono essere eliminati
            Try
                contattoDaEliminare = Contatto_Da_Eliminare(inData, contatto)
            Catch checkEx As Exception
                messaggio = checkEx.Message
            End Try

            If Not String.IsNullOrEmpty(messaggio) Then
                Throw New Exception($"Si è verificato un errore durante i controlli preliminari di cancellazione dei ruoli. {vbCrLf} {messaggio}")
            End If

            If contattoDaEliminare Then
                Dim cod_contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)
                'prima verifico se possibile eliminare
                messaggio = Verifica_Se_Possibile_Eliminare_Contatto(piva, cod_contatto)
                If String.IsNullOrEmpty(messaggio) Then
                    messaggio = Elimina_Contatto(contatto)
                End If
            Else


                messaggio = Salva_Contatto(cuaa, piva, inData, contatto, interscambioResult)


            End If

            'Lavez - 04/10/2024 - dato che apriamo connessione e transazione dentro la salva contatto ed in caso di errore dobbiamo cmq scrivere
            '                     dei log, la gestione di livello superiore non ha senso.
            'ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, _objParametri_Server)

        Catch ex As Exception
            'Lavez - 04/10/2024 - dato che apriamo connessione e transazione dentro la salva contatto ed in caso di errore dobbiamo cmq scrivere
            '                     dei log, la gestione di livello superiore non ha senso.
            'If Not _objParametri_Server.objTransazione Is Nothing Then
            '    ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)
            'End If

            messaggio = ex.Message
        Finally
            'Lavez - 04/10/2024 - dato che apriamo connessione e transazione dentro la salva contatto ed in caso di errore dobbiamo cmq scrivere
            '                     dei log, la gestione di livello superiore non ha senso.
            'ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, _objParametri_Server)
        End Try

        Return messaggio

    End Function

    Private Sub NormalizzaPayLoad(ByVal inData As AnagraficaWrapper(Of Contatto))

        inData.elemento_anagrafico.Normalizza()
        For Each r As AgronicaCoreDTOStd.InData.Demetra.RapportoContabile In inData.elemento_anagrafico.ruolo
            If IsNothing(r.validita.inizio) OrElse r.validita.inizio = DateTime.MinValue Then
                r.validita.inizio = AGRODATAINIZIO
            End If
            If IsNothing(r.validita.fine) OrElse r.validita.fine = DateTime.MinValue Then
                r.validita.fine = AGRODATAFINE
            End If
        Next

    End Sub

    Private Sub Controllo_Incrocio_Altri_Flussi(ByVal Piva As String,
                     ByVal inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto),
                     ByRef interscambioResult As D2G_Interscambio_Contatti_Result)

        Dim contattoEsistente = Ricerca_Contatto(Piva, inData, interscambioResult)
        If Not IsNothing(contattoEsistente) Then
            interscambioResult.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
            interscambioResult.Contatto = contattoEsistente
            interscambioResult.ScriviInterscambio = True
        End If

    End Sub

    Private Function Ricerca_Contatto(
                                    ByVal Piva As String,
                                    ByVal inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto),
                                    ByVal interscambioResult As D2G_Interscambio_Contatti_Result) As IDictionary(Of String, Object)

        Dim contattoEsistente As IDictionary(Of String, Object) = Nothing
        Dim contattoDemetra = inData.elemento_anagrafico
        Dim rag_soc As String = String.Empty
        Dim nome As String = String.Empty
        Dim cognome As String = String.Empty
        Dim id_cf As Integer = CInt(contattoDemetra.Id_CF)                     ' persona fisica / giuridica / estero se 2 estero la persona è giuridica
        Dim isEstero As Boolean = (id_cf = CONTATTO_ESTERO)
        Dim xFiltroAggiuntivo As String = String.Empty
        Dim cod_contatto As String = String.Empty
        Dim codice_fiscale As String = String.Empty
        Dim partitaIva As String = String.Empty

        If isEstero Then
            id_cf = PERSONA_GIURIDICA
        End If

        Select Case id_cf
            Case PERSONA_FISICA
                codice_fiscale = contattoDemetra.Cod_Fisc.Trim()
                If String.IsNullOrEmpty(codice_fiscale) Then
                    cod_contatto = contattoDemetra.codice_contatto.Trim
                Else
                    cod_contatto = codice_fiscale
                End If

                nome = contattoDemetra.nome.Trim
                cognome = contattoDemetra.cognome.Trim
                xFiltroAggiuntivo = " (LTRIM(RTRIM(nome)) = '" & nome.Replace("'", "''") & "' and LTRIM(RTRIM(cognome)) = '" & cognome.Replace("'", "''") & "')"
            Case PERSONA_GIURIDICA
                partitaIva = contattoDemetra.partita_iva.Trim()
                If String.IsNullOrEmpty(partitaIva) Then
                    cod_contatto = contattoDemetra.codice_contatto.Trim
                Else
                    cod_contatto = partitaIva
                End If

                rag_soc = contattoDemetra.ragione_Sociale.Trim
                xFiltroAggiuntivo = " LTRIM(RTRIM(Rag_Soc)) = '" & rag_soc.Replace("'", "''") & "' "

        End Select

        If Not String.IsNullOrEmpty(interscambioResult.COD_CONTATTO) Then
            cod_contatto = interscambioResult.COD_CONTATTO
        End If

        ' tento la ricerca per Piva Cod_Contatto
        If Not String.IsNullOrEmpty(cod_contatto) Then
            contattoEsistente = _dataLoader.Carica_Contatto_Esistente(Piva, cod_contatto, True, True)
        End If

        If IsNothing(contattoEsistente) Then

            ' tento ricrca per nome / cognome oppure rag_soc
            Dim datiContattoBase = _dataLoader.Ricerca_Contatto(Piva, xFiltroAggiuntivo)
            If Not IsNothing(datiContattoBase) Then
                contattoEsistente = _dataLoader.Carica_Contatto_Esistente(Piva, datiContattoBase(FIELD_MAPPER.COD_CONTATTO), True, True)
            End If

        End If

        Return contattoEsistente

    End Function

    Private Function Verifica_Se_Possibile_Eliminare_Contatto(ByVal piva As String, ByVal cod_Contatto As String) As String

        'TODO check su PIVA cod_contatto ?????
        Dim messaggio As String = String.Empty
        'Dim isImpresaGias As Boolean = _dataLoader.VerificaEsistenza_PivaGIAS(piva)

        'If isImpresaGias Then
        '    messaggio = ""
        '    Return messaggio
        'End If

        Dim risorseUmane = _dataLoader.Leggi_Risorse_Umane_Contatto(piva, cod_Contatto)
        If Not IsNothing(risorseUmane) AndAlso risorseUmane.Count > 0 Then

            For Each rs In risorseUmane

                Dim Rag_Soc As String = rs(FIELD_MAPPER.RAG_SOC).ToString
                Dim Cod_RisUm As Integer = CInt(rs(FIELD_MAPPER.COD_RISUM))

                Dim dtMovNC As DataTable = _dataLoader.Leggi_Movimenti_Non_Contabili(Cod_RisUm)
                If Not IsNothing(dtMovNC) AndAlso dtMovNC.Rows.Count > 0 Then
                    messaggio = "Sono presenti movimenti non contabili agganciati al contatto."
                    Exit For
                End If

                Dim dtMovC As DataTable = _dataLoader.Leggi_Movimenti_Contabili(Cod_RisUm)
                If Not IsNothing(dtMovC) AndAlso dtMovC.Rows.Count > 0 Then
                    messaggio = "Sono presenti movimenti contabili agganciati al contatto."
                    Exit For
                End If

                Dim dtSquadreXAtt As DataTable = _dataLoader.Leggi_SquadreXAttivita(Cod_RisUm)
                If Not IsNothing(dtSquadreXAtt) AndAlso dtSquadreXAtt.Rows.Count > 0 Then
                    messaggio = "Contatto assegnato a squadre."
                    Exit For
                End If
            Next

        End If

        If Not String.IsNullOrEmpty(messaggio) Then
            Return messaggio
        End If

        Dim dtAltriRiferimenti = _dataLoader.Leggi_Contatti_Riferimenti(piva, cod_Contatto)
        If Not IsNothing(dtAltriRiferimenti) AndAlso dtAltriRiferimenti.Rows.Count > 0 Then
            messaggio = "Sono presenti registrazioni ancora agganciate al contatto."
        End If

        Return messaggio

    End Function

    Private Function Elimina_Contatto(contatto As IDictionary(Of String, Object))

        Return _contattoWriter.Elimina_Contatto(contatto)

    End Function

    Private Function Salva_Contatto(ByVal cuaa As String,
                                    ByVal piva As String,
                                    ByVal inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto),
                                    ByVal contatto As IDictionary(Of String, Object),
                                    ByVal interscambioResult As D2G_Interscambio_Contatti_Result) As String

        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False
        Dim messaggio As String = String.Empty
        Dim tipoOperazioneDB As Integer = interscambioResult.TipoOperazioneDB

        Try

            'Lavez - 04/10/2024 - dato che apriamo connessione e transazione a livello superiore non ha senso farlo qui
            'ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale,
            '                                            flagTransazioneLocale,
            '                                            _objParametri_Server)

            _contattoWriter.Salva(inData.elemento_anagrafico, contatto, tipoOperazioneDB)

            _validatore.Controllo_Periodi_Intersecati(contatto, _rapportiContabiliAmmessi)

            Chiama_Scrivi_Log_Invio_Contatti(cuaa, inData.elemento_anagrafico, piva, contatto(FIELD_MAPPER.COD_CONTATTO), inData.codice, tipoOperazioneDB, Util_Costanti.ESITO_OK, "", UtilizzaTransazione:=False)

            If tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse interscambioResult.ScriviInterscambio Then

                'PRIMA DI INSERIRE CONTROLLO ULTERIORMENTE DI NON AVERE IN INTERSCAMBIO UNA RECORD CON LA STESSA CHIAVE GIAS
                Dim recInterscambio = _intercambio.Contatti_Leggi_Tabella_Interscambio_Chiave_GIAS(piva, contatto(FIELD_MAPPER.COD_CONTATTO))
                If Not IsNothing(recInterscambio) Then
                    Dim codiceEsterno As String = recInterscambio("codice_esterno")
                    If (codiceEsterno <> inData.codice) Then
                        Dim chiaveGias = String.Format("{0}_{1}", piva, contatto(FIELD_MAPPER.COD_CONTATTO))
                        Throw New Exception(String.Format("Contatto duplicato per chiave GIAS {0}. codice_esterno ricevuto {1}, già presente con codice_esterno {2}", chiaveGias, inData.codice, codiceEsterno))
                    End If
                End If

                _intercambio.Scrivi_Tabella_Interscambio_Contatti(piva, contatto(FIELD_MAPPER.COD_CONTATTO), inData.codice)
            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, _objParametri_Server)

        Catch ex As Exception
            If _objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)
            End If
            messaggio = ex.Message

            Chiama_Scrivi_Log_Invio_Contatti(cuaa, inData.elemento_anagrafico, piva, contatto(FIELD_MAPPER.COD_CONTATTO), inData.codice, tipoOperazioneDB, Util_Costanti.ESITO_KO, ex.Message)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, _objParametri_Server)
        End Try

        Return messaggio

    End Function

    Private Sub Chiama_Scrivi_Log_Invio_Contatti(cuaa As String, contatto As AgronicaCoreDTOStd.InData.Demetra.Contatto, piva As String, cod_contatto As String, chiave_esterna As String, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Optional UtilizzaTransazione As Boolean = True)

        Dim objLogInvioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W

        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim pacchettoDaLoggare As New AgronicaCoreDTOStd.InData.importazioni.ImportDemetra With {
            .CUAA = cuaa,
            .dati = JsonConvert.SerializeObject(contatto, tzh)
        }

        Dim strPacchettoDaLoggare = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        objLogInvioChiamate.Scrivi_Log_Invio_Contatti(enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC, strPacchettoDaLoggare, piva, cod_contatto, chiave_esterna, TipoOperazione, Esito, Dati_Ricevuti, _objParametri_Server, UtilizzaTransazione)

    End Sub

    Private Function Contatto_Da_Eliminare(inData As AnagraficaWrapper(Of Contatto), contatto As IDictionary(Of String, Object)) As Boolean

        Dim daEliminare = False

        ' Se nel payload c'è un solo ruolo di tipo terzista marcato come eliminabile non procedo al controllo di cancellazione effettiva
        If inData.elemento_anagrafico.ruolo.Count = 1 AndAlso inData.elemento_anagrafico.ruolo.FirstOrDefault.cod_rapporto = enum_Rapporti_Contabili_Standard.Terzista Then
            Return daEliminare
        End If

        Dim piva As String = contatto(FIELD_MAPPER.PIVA)
        Dim sa_cod As Integer = CInt(contatto(FIELD_MAPPER.SA_COD))
        Dim cod_contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)
        Dim eliminabili As New List(Of Integer)

        Dim rapportiContabili As List(Of IDictionary(Of String, Object)) = contatto("rapporti_contabili")
        For Each rap In rapportiContabili

            Dim cod_Rapporto As Integer = CInt(rap(FIELD_MAPPER.COD_RAPPORTO))
            Dim cod_risum As Integer = CInt(rap(FIELD_MAPPER.COD_RISUM))
            Dim flagCancella As Boolean = CBool(rap(FIELD_MAPPER.FLAG_CANCELLAZIONE))
            Dim rapContGias As IDictionary(Of String, Object) = _rapportiContabiliAmmessi.FirstOrDefault(Function(r) CInt(r(FIELD_MAPPER.COD_RAPPORTO)).Equals(cod_Rapporto))
            Dim desRapporto As String = If(IsNothing(rapContGias), String.Empty, rapContGias(FIELD_MAPPER.RAPPORTO_DES))
            If flagCancella Then

                ' prima controllo se il cod_risum esiste ancora in Gias altrimenti errore
                If Not _dataLoader.Esiste_Risorsa_Umana(cod_risum) Then
                    Throw New Exception($"Si sta tentado di eliminare il ruolo {desRapporto} con codice {cod_Rapporto} che non esiste più in Gias")
                End If

                'se il cod_risum esiste verifico se posso eliminarlo
                Dim messaggioErrore = _dataLoader.Controlla_Per_Eliminazione_Rapporto_Contabile(piva, cod_contatto, cod_risum, 0, desRapporto)
                If Not String.IsNullOrEmpty(messaggioErrore) Then
                    Throw New Exception(messaggioErrore)
                End If

                rap(FIELD_MAPPER.FLAG_CANCELLABILE) = True
                eliminabili.Add(cod_risum)

            End If

        Next

        ' conto quelli da inserire
        Dim rapportiDaInserire = rapportiContabili.Where(Function(rc) CBool(rc(FIELD_MAPPER.FLAG_CANCELLAZIONE)) = False AndAlso CInt(rc(FIELD_MAPPER.SA_COD)) = 0).ToList

        If eliminabili.Count = rapportiContabili.Count AndAlso rapportiDaInserire.Count = 0 Then
            ' significa che posso eleiminare tutti i ruoli marcati come cancellabili
            ' verifico se rimangono altri codrisum agganciati
            Dim rapportiContabiliRimasti = _dataLoader.Leggi_Risorse_Umane_Contatto(piva, cod_contatto)
            If Not IsNothing(rapportiContabiliRimasti) Then
                For Each e In eliminabili
                    Dim risUmDaEliminare = rapportiContabiliRimasti.FirstOrDefault(Function(rc) CInt(rc(FIELD_MAPPER.COD_RISUM)).Equals(e))
                    If Not IsNothing(risUmDaEliminare) Then
                        rapportiContabiliRimasti.Remove(risUmDaEliminare)
                    End If
                Next

                daEliminare = rapportiContabiliRimasti.Count() = 0

            End If


        End If

        Return daEliminare

    End Function

End Class

Public Class D2G_Interscambio_Contatti_Result

    Public Contatto As IDictionary(Of String, Object) = Nothing
    Public TipoOperazioneDB As enum_TipoOperazioneDB
    Public ScriviInterscambio As Boolean = False
    Public COD_CONTATTO As String = String.Empty

End Class

Public Class D2G_Interscambio_Documento_Result

    Public Documento As IDictionary(Of String, Object) = Nothing
    Public TipoOperazioneDB As enum_TipoOperazioneDB
    Public Chiave As Tuple(Of String, String, Integer)

End Class

Public Class D2G_Interscambio_Ruolo_Result

    Public Ruolo As IDictionary(Of String, Object) = Nothing
    Public TipoOperazioneDB As enum_TipoOperazioneDB
    Public Chiave As Integer

End Class

Public Class D2G_Interscambio_Contatti

    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Utenti As AgronicaCoreParametri = Nothing

    Private _objInterscambioContattiRead As Interscambio_Contatti_R = Nothing
    Private _objInterscambioContattiWrite As Interscambio_Contatti_W = Nothing

    Private _objInterscambioRisUmRead As Interscambio_Risorse_Umane_R = Nothing
    Private _objInterscambioRisUmWrite As Interscambio_Risorse_Umane_W = Nothing

    Private _objInterscambioDocsRead As Interscambio_Documenti_R = Nothing
    Private _objInterscambioDocsWrite As Interscambio_Documenti_W = Nothing

    Private _contattoLoader As D2G_Contatto_Loader = Nothing

    Public Sub New()

    End Sub

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                   ByVal contattoLoader As D2G_Contatto_Loader
                  )

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _objInterscambioContattiRead = New Interscambio_Contatti_R()
        _objInterscambioContattiWrite = New Interscambio_Contatti_W()
        _objInterscambioRisUmRead = New Interscambio_Risorse_Umane_R()
        _objInterscambioRisUmWrite = New Interscambio_Risorse_Umane_W()
        _objInterscambioDocsRead = New Interscambio_Documenti_R()
        _objInterscambioDocsWrite = New Interscambio_Documenti_W()
        _contattoLoader = contattoLoader

    End Sub

    Public Function DammiTipoOperazione_Ruolo(ByVal piva As String,
                                              ByVal cod_Contatto As String,
                                              ByVal ruoloDemetra As AgronicaCoreDTOStd.InData.Demetra.RapportoContabile) As D2G_Interscambio_Ruolo_Result

        Dim risultato As New D2G_Interscambio_Ruolo_Result

        Dim chiave As Integer = Nothing
        Dim ruoloEsistente As IDictionary(Of String, Object) = Nothing

        If Not String.IsNullOrEmpty(ruoloDemetra.codice_ruolo_esterno) AndAlso ruoloDemetra.codice_ruolo_esterno <> "0" Then

            ' OGGETTO CHE DOVREBBE GIA' ESSERE PRESENTE IN GIAS
            Dim arrChiavi = ruoloDemetra.codice_ruolo_esterno.Split("_")
            If arrChiavi.Count <> 1 Then
                Throw New Exception("Il valore del campo codice_ruolo_esterno non è una chiave Gias valida")
            End If
            chiave = CInt(arrChiavi(0))

            ruoloEsistente = _contattoLoader.Leggi_Risorsa_Umana(piva, cod_Contatto, chiave)
            If IsNothing(ruoloEsistente) Then

                ' Non trovato ne per chiave Gias ne per chiave Demetra
                Throw New Exception(String.Format("Ruolo non più esistente in gias. codice_ruolo_esterno {0}", ruoloDemetra.codice))

            Else

                ' trovato con chiave GIAS (Significa che l'id non è mai cambiato e vado in modifica)
                risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                risultato.Ruolo = ruoloEsistente

            End If

        Else

            If Not String.IsNullOrEmpty(ruoloDemetra.codice) Then

                Dim dtInterscambio = _objInterscambioRisUmRead.Leggi_Tabella_Interscambio_ChiaveEsterna(
                                                    enum_SistemiEsterni.demetra,
                                                    ruoloDemetra.codice,
                                                    _objParametri_Server)

                If IsNothing(dtInterscambio) OrElse dtInterscambio.Rows.Count = 0 Then

                    ' NON ESISTE IN INTERSCAMBIO -> CREAZIONE
                    risultato.Chiave = Nothing
                    risultato.Ruolo = Nothing
                    risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

                Else

                    If Not IsNothing(dtInterscambio) AndAlso dtInterscambio.Rows.Count = 1 Then

                        ' ESISTE IN INTERSCAMBIO --> VERIFICO CHE ESISTA ANCORA SU GIAS
                        chiave = CInt(dtInterscambio.Rows(0)(FIELD_MAPPER.COD_RISUM))
                        ruoloEsistente = _contattoLoader.Leggi_Risorsa_Umana(piva, cod_Contatto, chiave)
                        If IsNothing(ruoloEsistente) Then

                            ' ESISTE IN INTERSCAMBIO MA NON ESISTE PIù IN GIAS -> ERRORE
                            ' Throw New Exception("Documento non più esistente in Gias")
                            risultato.Chiave = Nothing
                            risultato.Ruolo = Nothing
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

                            Dim retVal As Boolean = _objInterscambioRisUmWrite.Cancella_Tabella_Interscambio(enum_SistemiEsterni.demetra, ruoloDemetra.codice, _objParametri_Server)
                            If Not retVal Then
                                Throw New Exception(String.Format("Errore durante la cancellazione del record dalla tabella Interscambio_Risorse_Umane per la chiave {0}", ruoloDemetra.codice))
                            End If

                        Else

                            risultato.Chiave = chiave
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                            risultato.Ruolo = ruoloEsistente

                        End If

                    Else

                        ' ESISTE DOPPIO IN INTERSCAMBIO - ERRORE
                        Throw New Exception("Ruolo duplicato per chiave Demetra")

                    End If

                End If

            End If

        End If

        Return risultato

    End Function

    Public Function DammiTipoOperazione_Documento(ByVal docDemetra As AgronicaCoreDTOStd.InData.Demetra.Documento) As D2G_Interscambio_Documento_Result

        Dim risultato As New D2G_Interscambio_Documento_Result

        Dim chiave As Tuple(Of String, String, Integer) = Nothing
        Dim documentoEsistente As IDictionary(Of String, Object) = Nothing

        If Not String.IsNullOrEmpty(docDemetra.codice_patentino_esterno) AndAlso docDemetra.codice_patentino_esterno <> "0" Then

            ' OGGETTO CHE DOVREBBE GIA' ESSERE PRESENTE IN GIAS
            Dim arrChiavi = docDemetra.codice_patentino_esterno.Split("_")
            If arrChiavi.Count <> 3 Then
                Throw New Exception("Il valore del campo Codice_Patentino_Esterno non è una chiave Gias valida")
            End If
            chiave = New Tuple(Of String, String, Integer)(arrChiavi(0), arrChiavi(1), CInt(arrChiavi(2)))

            documentoEsistente = _contattoLoader.Leggi_Documento(chiave.Item1, chiave.Item2, chiave.Item3)
            If IsNothing(documentoEsistente) Then

                ' Documento non più esistente in Gias (o perchè eliminato o perch sono cambiate le chiavi)
                ' Provo a leggere la interscambio_documenti per chiave Demetra per recuperare l'ID patentino aggiornato

                Dim dtInterscambio = _objInterscambioDocsRead.Leggi_Tabella_Interscambio_ChiaveEsterna(
                                                    enum_SistemiEsterni.demetra,
                                                    docDemetra.codice,
                                                    _objParametri_Server)

                If Not IsNothing(dtInterscambio) AndAlso dtInterscambio.Rows.Count > 1 Then
                    ' trovati due patentini con stessa chiave Demetra
                    Throw New Exception(String.Format("Trovati 2 patentini con stessa chiave Demetra. codice {0}", docDemetra.codice))
                End If

                If Not IsNothing(dtInterscambio) AndAlso dtInterscambio.Rows.Count = 1 Then

                    ' trovato con Chiave Demetra -> Ricavo la chiave Gias che è sempre aggiornata dopo ogni ins / mod
                    Dim codiceGias = dtInterscambio.Rows(0)("Codice_Gias").ToString
                    If String.IsNullOrEmpty(codiceGias) Then

                        ' Chiave Gias non valida
                        Throw New Exception("Il valore del campo codice_patentino_esterno non è una chiave Gias valida")

                    Else

                        arrChiavi = codiceGias.Split("_")
                        If arrChiavi.Count <> 3 Then
                            ' Chiave Gias non valida
                            Throw New Exception("Il valore del campo codice_patentino_esterno non è una chiave Gias valida")
                        End If

                        chiave = New Tuple(Of String, String, Integer)(arrChiavi(0), arrChiavi(1), CInt(arrChiavi(2)))
                        documentoEsistente = _contattoLoader.Leggi_Documento(chiave.Item1, chiave.Item2, chiave.Item3)
                        If IsNothing(documentoEsistente) Then

                            'Non trovato ne per chiave Gias più aggiornata (Significa che è stato eliminato
                            Throw New Exception("Patentino non più esistente in gias")

                        Else
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                            risultato.Documento = documentoEsistente
                        End If

                    End If
                Else

                    ' Non trovato ne per chiave Gias ne per chiave Demetra
                    Throw New Exception(String.Format("Patentino non più esistente in gias. codice_patentino_esterno {0}", docDemetra.codice))

                End If

            Else

                ' trovato con chiave GIAS (Significa che l'id non è mai cambiato e vado in modifica)

                risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                risultato.Documento = documentoEsistente

            End If

        Else

            If Not String.IsNullOrEmpty(docDemetra.codice) Then

                Dim dtInterscambio = _objInterscambioDocsRead.Leggi_Tabella_Interscambio_ChiaveEsterna(
                                                    enum_SistemiEsterni.demetra,
                                                    docDemetra.codice,
                                                    _objParametri_Server)

                If IsNothing(dtInterscambio) OrElse dtInterscambio.Rows.Count = 0 Then

                    ' NON ESISTE IN INTERSCAMBIO -> CREAZIONE
                    risultato.Chiave = Nothing
                    risultato.Documento = Nothing
                    risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

                Else

                    If Not IsNothing(dtInterscambio) AndAlso dtInterscambio.Rows.Count = 1 Then

                        ' ESISTE IN INTERSCAMBIO --> VERIFICO CHE ESISTA ANCORA SU GIAS
                        chiave = New Tuple(Of String, String, Integer)(
                                        dtInterscambio.Rows(0)(FIELD_MAPPER.PIVA).ToString,
                                        dtInterscambio.Rows(0)(FIELD_MAPPER.COD_CONTATTO).ToString(),
                                        CInt(dtInterscambio.Rows(0)(FIELD_MAPPER.ID_Elenco))
                            )

                        documentoEsistente = _contattoLoader.Leggi_Documento(chiave.Item1, chiave.Item2, chiave.Item3)
                        If IsNothing(documentoEsistente) Then

                            ' ESISTE IN INTERSCAMBIO MA NON ESISTE PIù IN GIAS -> ERRORE
                            ' Throw New Exception("Documento non più esistente in Gias")
                            risultato.Chiave = Nothing
                            risultato.Documento = Nothing
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

                            Dim retVal As Boolean = _objInterscambioDocsWrite.Elimina_Tabella_Interscambio(enum_SistemiEsterni.demetra, docDemetra.codice, _objParametri_Server)
                            If Not retVal Then
                                Throw New Exception(String.Format("Errore durante la cancellazione del record dalla tabella Interscambio_Documenti per la chiave {0}", docDemetra.codice))
                            End If

                        Else

                            risultato.Chiave = chiave
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                            risultato.Documento = documentoEsistente

                        End If

                    Else

                        ' ESISTE DOPPIO IN INTERSCAMBIO - ERRORE
                        Throw New Exception("Documento duplicato per chiave Demetra")

                    End If

                End If

            End If

        End If

        Return risultato

    End Function

    Public Function DammiTipoOperazione_Contatto(ByVal inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto)) As D2G_Interscambio_Contatti_Result

        Dim risultato As New D2G_Interscambio_Contatti_Result
        risultato.ScriviInterscambio = False

        Dim chiave As Tuple(Of String, String) = Nothing
        Dim contattoEsistente As IDictionary(Of String, Object) = Nothing

        If Not String.IsNullOrEmpty(inData.codice_esterno) AndAlso inData.codice_esterno <> "0" Then

            ' OGGETTO CHE DOVREBBE GIA' ESSERE PRESENTE IN GIAS

            Dim arrChiavi = inData.codice_esterno.Split("_")
            If arrChiavi.Count <> 2 Then
                Throw New Exception("Il valore del campo Codice_Esterno non è una chiave Gias valida")
            End If
            chiave = New Tuple(Of String, String)(arrChiavi(0), arrChiavi(1))

            contattoEsistente = _contattoLoader.Carica_Contatto_Esistente(chiave.Item1, chiave.Item2)

            If IsNothing(contattoEsistente) Then

                ' NON ESISTE PIù IN GIAS -> FORZO UN REINSERIMENTO
                ' INOLTRE RECICLO IL COD_CONTATTO CHE C'ERA PRECEDENTEMENTE SE PRESENTE IN INTERSCAMBIO
                risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                risultato.Contatto = Nothing
                risultato.ScriviInterscambio = True

                Dim dtInterscambio = _objInterscambioContattiRead.Leggi_Tabella_Interscambio_ChiaveGIAS(
                                                    enum_SistemiEsterni.demetra,
                                                    chiave.Item1, chiave.Item2, 0, _objParametri_Server)

                If Not IsNothing(dtInterscambio) AndAlso dtInterscambio.Rows.Count = 1 Then

                    Dim codContatto As String = dtInterscambio.Rows(0)(FIELD_MAPPER.COD_CONTATTO).ToString
                    Dim codiceEsterno As String = dtInterscambio.Rows(0)("Codice_Esterno").ToString
                    risultato.COD_CONTATTO = codContatto

                    ' Cancello il record esistente in interscambio 
                    Dim retVal As Boolean = _objInterscambioContattiWrite.Cancella_Tabella_Interscambio(enum_SistemiEsterni.demetra, codiceEsterno, _objParametri_Server)
                    If Not retVal Then
                        Throw New Exception(String.Format("Errore durante la cancellazione del record dalla tabella Interscambio_Contatti per la chiave {0}", inData.codice_esterno))
                    End If

                End If

            Else

                risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                risultato.Contatto = contattoEsistente

            End If

        Else

            If Not String.IsNullOrEmpty(inData.codice) Then

                Dim dtInterscambio = _objInterscambioContattiRead.Leggi_Tabella_Interscambio_ChiaveEsterna(
                                                    enum_SistemiEsterni.demetra,
                                                    inData.codice,
                                                    _objParametri_Server)
                If IsNothing(dtInterscambio) OrElse dtInterscambio.Rows.Count = 0 Then

                    ' NON ESISTE IN INTERSCAMBIO -> CREAZIONE
                    risultato.Contatto = Nothing
                    risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                    risultato.ScriviInterscambio = True

                Else

                    If Not IsNothing(dtInterscambio) AndAlso dtInterscambio.Rows.Count = 1 Then

                        ' ESISTE IN INTERSCAMBIO --> VERIFICO CHE ESISTA ANCORA SU GIAS
                        chiave = New Tuple(Of String, String)(
                        dtInterscambio.Rows(0)(FIELD_MAPPER.PIVA).ToString,
                        dtInterscambio.Rows(0)(FIELD_MAPPER.COD_CONTATTO).ToString())

                        contattoEsistente = _contattoLoader.Carica_Contatto_Esistente(chiave.Item1, chiave.Item2)
                        If IsNothing(contattoEsistente) Then

                            ' ESISTE IN INTERSCAMBIO MA NON ESISTE PIù IN GIAS -> FORZO UN REINSERIMENTO
                            ' INOLTRE RECICLO IL COD_CONTATTO CHE C'ERA PRECEDENTEMENTE IN INTERSCAMBIO
                            risultato.Contatto = Nothing
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                            risultato.ScriviInterscambio = True
                            risultato.COD_CONTATTO = chiave.Item2

                            ' Cancello il record esistente in interscambio attraverso la chiave Demetra con cui l'ho appena cercato (inData.codice)
                            Dim retVal As Boolean = _objInterscambioContattiWrite.Cancella_Tabella_Interscambio(enum_SistemiEsterni.demetra, inData.codice, _objParametri_Server)
                            If Not retVal Then
                                Throw New Exception(String.Format("Errore durante la cancellazione del record dalla tabella Interscambio_Contatti per la chiave {0}", inData.codice))
                            End If

                        Else
                            risultato.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                            risultato.Contatto = contattoEsistente
                        End If
                    Else

                        ' ESISTE DOPPIO IN INTERSCAMBIO - ERRORE
                        Throw New Exception("Contatto duplicato per chiave Demetra")
                    End If

                End If

            End If

        End If

        Return risultato

    End Function

    Public Function Contatti_Leggi_Tabella_Interscambio_Chiave_GIAS(ByVal piva As String, ByVal cod_contatto As String) As IDictionary(Of String, Object)

        Dim dt As DataTable = _objInterscambioContattiRead.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, piva, cod_contatto, 0, _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            If dt.Rows.Count > 1 Then
                Throw New Exception("Contatto duplicato per chiave Gias")
            Else
                Return dt.ToExpandoObjectCaseInsensitive().FirstOrDefault()
            End If
        Else
            Return Nothing
        End If

    End Function

    Public Function Documenti_Leggi_Tabella_Interscambio(ByVal piva As String, ByVal Cod_Contatto As String) As List(Of IDictionary(Of String, Object))

        Dim dt As DataTable = _objInterscambioDocsRead.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, piva, Cod_Contatto, 0, _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return dt.ToExpandoObjectCaseInsensitive()
        Else
            Return Nothing
        End If

    End Function

    Public Function Risorse_Umane_Leggi_Tabella_Interscambio_Chiave_GIAS(ByVal piva As String, ByVal cod_RisUm As Integer) As IDictionary(Of String, Object)

        Dim dt As DataTable = _objInterscambioRisUmRead.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, piva, cod_RisUm, _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            If dt.Rows.Count > 1 Then
                Throw New Exception("Ruolo duplicato per chiave Gias")
            Else
                Return dt.ToExpandoObjectCaseInsensitive().FirstOrDefault()
            End If
        Else
            Return Nothing
        End If

    End Function

    Public Function Documenti_Leggi_Tabella_Interscambio_Chiave_GIAS(ByVal piva As String, ByVal Cod_Contatto As String, ByVal Id_Elenco As Integer) As IDictionary(Of String, Object)

        Dim dt As DataTable = _objInterscambioDocsRead.Leggi_Tabella_Interscambio_ChiaveGIAS(enum_SistemiEsterni.demetra, piva, Cod_Contatto, Id_Elenco, _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            If dt.Rows.Count > 1 Then
                Throw New Exception("Ruolo duplicato per chiave Gias")
            Else
                Return dt.ToExpandoObjectCaseInsensitive().FirstOrDefault()
            End If
        Else
            Return Nothing
        End If

    End Function

    Public Sub Scrivi_Tabella_Interscambio_Contatti(ByVal piva As String, ByVal cod_contatto As String, codice_esterno As String)

        _objInterscambioContattiWrite.Scrivi_Tabella_Interscambio(enum_SistemiEsterni.demetra, piva, cod_contatto, codice_esterno, _objParametri_Server)

    End Sub

    Public Sub Scrivi_Tabella_Interscambio_Documenti(ByVal piva As String, ByVal cod_contatto As String, ByVal ID_Elenco As Integer, codice_esterno As String)

        _objInterscambioDocsWrite.Scrivi_Tabella_Interscambio(enum_SistemiEsterni.demetra, piva, cod_contatto, ID_Elenco, codice_esterno, _objParametri_Server)

    End Sub

    Public Sub Scrivi_Tabella_Interscambio_Risorse_Umane(ByVal piva As String, ByVal cod_RisUm As Integer, codice_esterno As String)

        _objInterscambioRisUmWrite.Scrivi_Tabella_Interscambio(enum_SistemiEsterni.demetra, piva, cod_RisUm, codice_esterno, _objParametri_Server)

    End Sub


    Public Sub Elimina_Da_Interscambio_Documenti(ByVal piva As String, ByVal cod_contatto As String, ByVal ID_Elenco As Integer, codice_esterno As String)

        _objInterscambioDocsWrite.Elimina_Tabella_Interscambio(enum_SistemiEsterni.demetra, piva, cod_contatto, ID_Elenco, codice_esterno, _objParametri_Server)

    End Sub

End Class


Public Class D2G_Contatto_Writer

    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Utenti As AgronicaCoreParametri = Nothing
    Private _objContattiWrite As AgronicaCoreAnagrafeDAL.Contatti_W = Nothing
    Private _objContattiXmlWrite As AgronicaCoreAnagrafeBIZ.Contatti_W = Nothing
    Private _objContatti_CodiciWrite As AgronicaCoreAnagrafeDAL.Contatti_Codici_W = Nothing
    Private _objContattixIndirizziWrite As AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_W = Nothing
    Private _objIndirizziWrite As AgronicaCoreAnagrafeDAL.Indirizzi_Write = Nothing
    Private _objRisorse_UmaneWrite As AgronicaCoreAnagrafeDAL.Risorse_Umane_W = Nothing
    Private _objAlertWrite As AgronicaCoreScadenziario_BIZ.Alert_W = Nothing
    Private _objAlertElencoWrite As AgronicaCoreScadenziario.Alert_Elenco_W
    Private _objLogContattiWrite As New AgronicaCoreAnagrafeDAL.Agronica_Log_Contatti_W
    Private _dataLoader As D2G_Contatto_Loader = Nothing
    Private _interscambio As D2G_Interscambio_Contatti = Nothing

    Public Property BaseCode As Integer
    Public Property TopCode As Integer

    Public Sub New()

    End Sub

    Public Sub New(
                  ByVal contattoLoader As D2G_Contatto_Loader,
                  ByVal interscambio As D2G_Interscambio_Contatti,
                  ByRef objParametri_Server As AgronicaCoreParametri,
                  ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _objContattiWrite = New AgronicaCoreAnagrafeDAL.Contatti_W()
        _objContattiXmlWrite = New AgronicaCoreAnagrafeBIZ.Contatti_W()
        _objContatti_CodiciWrite = New AgronicaCoreAnagrafeDAL.Contatti_Codici_W()
        _objContattixIndirizziWrite = New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_W()
        _objIndirizziWrite = New AgronicaCoreAnagrafeDAL.Indirizzi_Write()
        _objRisorse_UmaneWrite = New AgronicaCoreAnagrafeDAL.Risorse_Umane_W()
        _objAlertWrite = New AgronicaCoreScadenziario_BIZ.Alert_W()
        _objAlertElencoWrite = New AgronicaCoreScadenziario.Alert_Elenco_W()
        _objLogContattiWrite = New AgronicaCoreAnagrafeDAL.Agronica_Log_Contatti_W()
        _dataLoader = contattoLoader
        _interscambio = interscambio

    End Sub

    Public Sub Salva(inData As AgronicaCoreDTOStd.InData.Demetra.Contatto,
                     contatto As IDictionary(Of String, Object),
                     ByVal tipoOperazione As enum_TipoOperazioneDB)

        Dim piva As String = contatto(FIELD_MAPPER.PIVA)
        Dim cod_Contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)

        Salva_Contatto(inData, contatto, tipoOperazione)

        Salva_Rapporti_Contabili(contatto)

        If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

            Salva_Contatti_Codici(contatto, tipoOperazione)

            Salva_Indirizzi(contatto, tipoOperazione)

        End If

        Salva_Documenti(contatto)

    End Sub


    Public Function Elimina_Contatto(contatto As IDictionary(Of String, Object)) As String

        Dim messaggio As String = String.Empty

        Try

            Dim Piva As String = contatto(FIELD_MAPPER.PIVA)
            Dim Cod_Contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)

            'Leggo la stringa facendomi ritornare Tipooperazione = 3 (x la cancellazione)
            Dim StringaXML = _dataLoader.Leggi_Contatto_Completo_XML(Piva, Cod_Contatto)

            Dim strDummy = _objContattiXmlWrite.Contatto_Scrivi(
                                     CStr(StringaXML),
                                     Nothing,
                                     Nothing,
                                     _objParametri_Server,,, enum_SistemiEsterni.demetra)


            _objAlertWrite.Cancella_Allegati_Contatto(Piva, Cod_Contatto, _objParametri_Server)


        Catch ex As Exception
            messaggio = ex.Message
        End Try

        Return messaggio

    End Function
    Private Sub Salva_Contatto(inData As AgronicaCoreDTOStd.InData.Demetra.Contatto,
                               contatto As IDictionary(Of String, Object),
                               ByVal tipoOperazione As enum_TipoOperazioneDB)

        Try

            Select Case tipoOperazione
                Case enum_TipoOperazioneDB.Scrittura
                    _objContattiWrite.Scrivi(contatto(FIELD_MAPPER.PIVA),
                                       CInt(contatto(FIELD_MAPPER.SA_COD)),
                                       contatto(FIELD_MAPPER.COD_CONTATTO),
                                       CInt(contatto(FIELD_MAPPER.ID_CF)),
                                       contatto(FIELD_MAPPER.RAG_SOC),
                                       contatto(FIELD_MAPPER.CODICE_FISCALE),
                                       contatto(FIELD_MAPPER.CONVENEVOLI),
                                       CInt(contatto(FIELD_MAPPER.TIPO_INDIRIZZO_DEFAULT)),
                                       contatto(FIELD_MAPPER.NOME),
                                       contatto(FIELD_MAPPER.COGNOME),
                                       contatto(FIELD_MAPPER.DATA_NASCITA),
                                       contatto(FIELD_MAPPER.SESSO),
                                       contatto(FIELD_MAPPER.COD_CONTATTO_REFERENTE),
                                       CDate(contatto(FIELD_MAPPER.VALIDITA_INIZIO)),
                                       CDate(contatto(FIELD_MAPPER.VALIDITA_FINE)),
                                       _objParametri_Server,
                                       CDate(contatto(FIELD_MAPPER.DATA_CREAZIONE)),
                                       CDate(contatto(FIELD_MAPPER.DATA_MODIFICA)),
                                       contatto(FIELD_MAPPER.USERNAME_CREAZIONE),
                                       contatto(FIELD_MAPPER.USERNAME_MODIFICA),
                                       CInt(contatto(FIELD_MAPPER.TIPO_SPEDITORE)),
                                       CInt(contatto(FIELD_MAPPER.TIPO_DESTINAZIONE)),
                                       CInt(contatto(FIELD_MAPPER.AGENTE_COD)),
                                       CDec(contatto(FIELD_MAPPER.PROVVIGIONE)),
                                       contatto(FIELD_MAPPER.NOTE),
                                       CInt(contatto(FIELD_MAPPER.ID_GESTIONE_NOTE)),
                                       contatto(FIELD_MAPPER.NOTE2),
                                       contatto(FIELD_MAPPER.NOTE_OPERAZIONI),
                                       contatto(FIELD_MAPPER.NOTE2_OPERAZIONI),
                                       CInt(contatto(FIELD_MAPPER.COD_RISUM_DESTINAZIONE_DIVERSA)),
                                       CInt(contatto(FIELD_MAPPER.TIPO_INDIRIZZO_DEFAULT_DESTINAZIONE_DIVERSA)),
                                       CDec(contatto(FIELD_MAPPER.FIDO)),
                                       CInt(contatto(FIELD_MAPPER.LIMITE_POSIZIONI)),
                                       CDec(contatto(FIELD_MAPPER.LIMITE_GIORNI_EVASIONE)),
                                       contatto(FIELD_MAPPER.ORARI_RITIRO),
                                       contatto(FIELD_MAPPER.FILTRO_RIMBORSI),
                                       CInt(contatto(FIELD_MAPPER.VETTORE_COD)),
                                       CInt(contatto(FIELD_MAPPER.CAPOAREA_COD)),
                                       CDec(contatto(FIELD_MAPPER.PROVVIGIONE_CAPOAREA)),
                                       contatto(FIELD_MAPPER.MEMO),
                                       CDec(contatto(FIELD_MAPPER.SCONTO_CONTATTO)),
                                       contatto(FIELD_MAPPER.SCONTO_TESTO),
                                       CInt(contatto(FIELD_MAPPER.MODALITA_FATTURAZIONE)),
                                       CInt(contatto(FIELD_MAPPER.COD_IVA_CONTATTO)),
                                       CInt(contatto(FIELD_MAPPER.DOCUMENTO_FATTURAZIONE)),
                                       nrBadge:=contatto(FIELD_MAPPER.NRBADGE),
                                       ChkFittizio:=CInt(contatto(FIELD_MAPPER.CHKFITTIZIO)),
                                       Cod_Conto_Economico_Default:=CInt(contatto(FIELD_MAPPER.COD_CONTO_ECON)),
                                       Cod_Conto_Patrimoniale_Default:=CInt(contatto(FIELD_MAPPER.COD_CONTO_PAT)),
                                       Nome_Breve:=contatto(FIELD_MAPPER.NOME_BREVE))

                Case enum_TipoOperazioneDB.Modifica
                    _objContattiWrite.Modifica(
                                        contatto(FIELD_MAPPER.PIVA),
                                        CInt(contatto(FIELD_MAPPER.SA_COD)),
                                        contatto(FIELD_MAPPER.COD_CONTATTO),
                                        CInt(contatto(FIELD_MAPPER.ID_CF)),
                                        contatto(FIELD_MAPPER.RAG_SOC),
                                        contatto(FIELD_MAPPER.CODICE_FISCALE),
                                        contatto(FIELD_MAPPER.CONVENEVOLI),
                                        CInt(contatto(FIELD_MAPPER.TIPO_INDIRIZZO_DEFAULT)),
                                        contatto(FIELD_MAPPER.NOME),
                                        contatto(FIELD_MAPPER.COGNOME),
                                        contatto(FIELD_MAPPER.DATA_NASCITA),
                                        contatto(FIELD_MAPPER.SESSO),
                                        contatto(FIELD_MAPPER.COD_CONTATTO_REFERENTE),
                                        CDate(contatto(FIELD_MAPPER.VALIDITA_INIZIO)),
                                        CDate(contatto(FIELD_MAPPER.VALIDITA_FINE)),
                                        contatto(FIELD_MAPPER.NRBADGE),
                                        "",
                                        _objParametri_Server,
                                        Memo:=contatto(FIELD_MAPPER.MEMO),
                                        Tipo_Destinazione:=CInt(contatto(FIELD_MAPPER.TIPO_DESTINAZIONE)),
                                        Note:=contatto(FIELD_MAPPER.NOTE),
                                        Note2:=contatto(FIELD_MAPPER.NOTE2),
                                        Note_Operazioni:=contatto(FIELD_MAPPER.NOTE_OPERAZIONI),
                                        Note2_Operazioni:=contatto(FIELD_MAPPER.NOTE2_OPERAZIONI),
                                        Tipo_Speditore:=CInt(contatto(FIELD_MAPPER.TIPO_SPEDITORE)),
                                        Sconto_Testo:=contatto(FIELD_MAPPER.SCONTO_TESTO),
                                        Agente_Cod:=CInt(contatto(FIELD_MAPPER.AGENTE_COD)),
                                        CapoArea_Cod:=CInt(contatto(FIELD_MAPPER.CAPOAREA_COD)),
                                        Vettore_Cod:=CInt(contatto(FIELD_MAPPER.VETTORE_COD)),
                                        Modalita_Fatturazione:=CInt(contatto(FIELD_MAPPER.MODALITA_FATTURAZIONE)),
                                        Cod_Iva_Contatto:=CInt(contatto(FIELD_MAPPER.COD_IVA_CONTATTO)),
                                        Cod_Conto_Economico_Default:=CInt(contatto(FIELD_MAPPER.COD_CONTO_ECON)),
                                        Cod_Conto_Patrimoniale_Default:=CInt(contatto(FIELD_MAPPER.COD_CONTO_PAT)),
                                        Provvigione:=CDec(contatto(FIELD_MAPPER.PROVVIGIONE)),
                                        Provvigione_CapoArea:=CDec(contatto(FIELD_MAPPER.PROVVIGIONE_CAPOAREA)),
                                        Cod_Risum_Destinazione_Diversa:=CInt(contatto(FIELD_MAPPER.COD_RISUM_DESTINAZIONE_DIVERSA)),
                                        Tipo_Indirizzo_Default_Destinazione_Diversa:=CInt(contatto(FIELD_MAPPER.TIPO_INDIRIZZO_DEFAULT_DESTINAZIONE_DIVERSA)),
                                        ChkFittizio:=CInt(contatto(FIELD_MAPPER.CHKFITTIZIO)),
                                        Nome_Breve:=contatto(FIELD_MAPPER.NOME_BREVE)
                                    )
            End Select

            Log_Anagrafe_Contatto(inData, contatto, tipoOperazione)

        Catch ex As Exception
            Throw New Exception(String.Format("Errore nel salvataggio del Contatto:  {0}", ex.Message))
        End Try

    End Sub

    Private Sub Log_Anagrafe_Contatto(inData As AgronicaCoreDTOStd.InData.Demetra.Contatto,
                                      ByVal contatto As IDictionary(Of String, Object),
                                      tipoOperazione As enum_TipoOperazioneDB)

        Dim Piva As String = contatto(FIELD_MAPPER.PIVA)
        Dim Cod_Contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)
        Dim Id_CF As Integer = CInt(contatto(FIELD_MAPPER.ID_CF))
        Dim Note As String = NOTELOG_ANAGRAFE_CONTATTI_DEMETRA
        Dim Rag_Soc As String = contatto(FIELD_MAPPER.RAG_SOC)
        Dim Nome As String = contatto(FIELD_MAPPER.NOME)
        Dim Cognome As String = contatto(FIELD_MAPPER.COGNOME)
        Dim ContattoDes As String = If(String.IsNullOrEmpty(Rag_Soc), String.Format("{0} {1}", Cognome, Nome), Rag_Soc)

        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim datiContatto = JsonConvert.SerializeObject(inData, tzh)

        _objLogContattiWrite.Scrivi(tipoOperazione, Piva, Cod_Contatto, 0, Id_CF, ContattoDes, Note, enum_Id_Servizio.GiasOnline, _objParametri_Server, datiContatto, enum_SistemiEsterni.demetra)

    End Sub

    Private Sub Salva_Rapporti_Contabili(contatto As IDictionary(Of String, Object))

        Try

            Dim piva As String = contatto(FIELD_MAPPER.PIVA)
            Dim sa_cod As Integer = CInt(contatto(FIELD_MAPPER.SA_COD))
            Dim cod_contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)

            Dim rapportiContabili As List(Of IDictionary(Of String, Object)) = contatto("rapporti_contabili")
            'Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

            For Each rap In rapportiContabili

                Dim cod_risum As Integer = CInt(rap(FIELD_MAPPER.COD_RISUM))
                Dim flagCancella As Boolean = CBool(rap(FIELD_MAPPER.FLAG_CANCELLAZIONE))
                Dim codice As String = rap(FIELD_MAPPER.CODICE)
                Dim tipoOperazioneDB As enum_TipoOperazioneDB


                If flagCancella Then
                    tipoOperazioneDB = enum_TipoOperazioneDB.Cancellazione
                Else
                    tipoOperazioneDB = CInt(rap(FIELD_MAPPER.TIPO_OPERAZIONE_DB))
                End If


                'If cod_risum > 0 Then
                '    If flagCancella Then
                '        tipoOperazioneDB = enum_TipoOperazioneDB.Cancellazione
                '    Else
                '        tipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                '    End If
                'Else
                '    tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                'End If

                Select Case tipoOperazioneDB
                    Case enum_TipoOperazioneDB.Scrittura

                        'cod_risum = objSequenze.NuovoId_Tabella("Risorse_Umane", base, top, _objParametri_Server)
                        _objRisorse_UmaneWrite.Scrivi(
                                            piva,
                                            sa_cod,
                                            cod_risum,
                                            cod_contatto,
                                            CInt(rap(FIELD_MAPPER.COD_RAPPORTO)),
                                            rap(FIELD_MAPPER.SETTORE_DES),
                                            rap(FIELD_MAPPER.ATTIVITA_DES),
                                            CDbl(rap(FIELD_MAPPER.CORRISPETTIVO_MENSILE)),
                                            CDbl(rap(FIELD_MAPPER.CORRISPETTIVO_ORARIO)),
                                            CDbl(rap(FIELD_MAPPER.ORE_SETTIMANALI)),
                                            CInt(rap(FIELD_MAPPER.GIORNI_FERIE)),
                                            CInt(rap(FIELD_MAPPER.FERIE_GODUTE)),
                                            CInt(rap(FIELD_MAPPER.GIORNI_MALATTIA)),
                                            CInt(rap(FIELD_MAPPER.OCCASIONALE)),
                                            rap(FIELD_MAPPER.PATENTINO),
                                            CDate(rap(FIELD_MAPPER.DATA_RILASCIO_PATENTINO)),
                                            CDate(rap(FIELD_MAPPER.DATA_SCADENZA_PATENTINO)),
                                            rap(FIELD_MAPPER.ENTE_DI_RILASCIO),
                                            CInt(rap(FIELD_MAPPER.COD_RISUM_ORIGINE)),
                                            rap(FIELD_MAPPER.PIVA_SUPERUSER_ORIGINE),
                                            CDec(rap(FIELD_MAPPER.SALDO_INIZIALE_CREDITI)),
                                            CDec(rap(FIELD_MAPPER.SALDO_INIZIALE_DEBITI)),
                                            CInt(rap(FIELD_MAPPER.CHKSPESOMETRO)),
                                            CDate(rap(FIELD_MAPPER.VALIDITA_INIZIO)),
                                            CDate(rap(FIELD_MAPPER.VALIDITA_FINE)),
                                            _objParametri_Server,
                                            CDate(rap(FIELD_MAPPER.DATA_CREAZIONE)),
                                            CDate(rap(FIELD_MAPPER.DATA_MODIFICA)),
                                            rap(FIELD_MAPPER.USERNAME_CREAZIONE),
                                            rap(FIELD_MAPPER.USERNAME_MODIFICA),
                                            CInt(rap(FIELD_MAPPER.QUALIFICA_COD)),
                                            CInt(rap(FIELD_MAPPER.MANSIONE_COD)),
                                            CInt(rap(FIELD_MAPPER.CLASSIFICAZIONE_COD)),
                                            rap(FIELD_MAPPER.INFO_FAMIGLIA),
                                            Cod_Iva_Contatto:=CInt(rap(FIELD_MAPPER.COD_IVA_CONTATTO)),
                                            Cod_Conto_Econ:=CInt(rap(FIELD_MAPPER.COD_CONTO_ECON)),
                                            Cod_Conto_Pat:=CInt(rap(FIELD_MAPPER.COD_CONTO_PAT))
                                        )

                        ' Scrittura tabella interscambio
                        '_interscambio.Scrivi_Tabella_Interscambio_Risorse_Umane(piva, cod_risum, codice)

                    Case enum_TipoOperazioneDB.Modifica

                        Dim cod_Iva_contatto As Integer = If(IsDBNull(rap(FIELD_MAPPER.COD_IVA_CONTATTO)), -1, CInt(rap(FIELD_MAPPER.COD_IVA_CONTATTO)))
                        Dim cod_conto_econ As Integer = If(IsDBNull(rap(FIELD_MAPPER.COD_CONTO_ECON)), 0, CInt(rap(FIELD_MAPPER.COD_CONTO_ECON)))
                        Dim cod_conto_pat As Integer = If(IsDBNull(rap(FIELD_MAPPER.COD_CONTO_PAT)), 0, CInt(rap(FIELD_MAPPER.COD_CONTO_PAT)))

                        _objRisorse_UmaneWrite.Modifica(cod_risum,
                                                rap(FIELD_MAPPER.SETTORE_DES),
                                                rap(FIELD_MAPPER.ATTIVITA_DES),
                                                CDbl(rap(FIELD_MAPPER.ORE_SETTIMANALI)),
                                                CInt(rap(FIELD_MAPPER.GIORNI_FERIE)),
                                                CInt(rap(FIELD_MAPPER.FERIE_GODUTE)),
                                                CInt(rap(FIELD_MAPPER.GIORNI_MALATTIA)),
                                                CInt(rap(FIELD_MAPPER.OCCASIONALE)),
                                                rap(FIELD_MAPPER.PATENTINO),
                                                CDate(rap(FIELD_MAPPER.DATA_RILASCIO_PATENTINO)),
                                                CDate(rap(FIELD_MAPPER.DATA_SCADENZA_PATENTINO)),
                                                rap(FIELD_MAPPER.ENTE_DI_RILASCIO),
                                                CDec(rap(FIELD_MAPPER.SALDO_INIZIALE_CREDITI)),
                                                CDec(rap(FIELD_MAPPER.SALDO_INIZIALE_DEBITI)),
                                                CInt(rap(FIELD_MAPPER.CHKSPESOMETRO)),
                                                CDate(rap(FIELD_MAPPER.VALIDITA_INIZIO)),
                                                CDate(rap(FIELD_MAPPER.VALIDITA_FINE)), "",
                                                _objParametri_Server,
                                                CInt(rap(FIELD_MAPPER.QUALIFICA_COD)),
                                                CInt(rap(FIELD_MAPPER.MANSIONE_COD)),
                                                CInt(rap(FIELD_MAPPER.CLASSIFICAZIONE_COD)),
                                                rap(FIELD_MAPPER.INFO_FAMIGLIA),
                                                Cod_Rapporto:=CInt(rap(FIELD_MAPPER.COD_RAPPORTO)),
                                                Cod_Iva_Contatto:=cod_Iva_contatto,
                                                Cod_Conto_Econ:=cod_conto_econ,
                                                Cod_Conto_Pat:=cod_conto_pat,
                                                Sa_Cod:=0
                                                )


                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim cod_Rapporto As Integer = CInt(rap(FIELD_MAPPER.COD_RAPPORTO))

                        ' Procedo diretto perche tutti i controlli del caso sono stati fatti prima
                        _objRisorse_UmaneWrite.Cancella(
                                                        cod_risum,
                                                        cod_contatto,
                                                        cod_Rapporto,
                                                        "",
                                                        _objParametri_Server)


                End Select

            Next

        Catch ex As Exception
            Throw New Exception(String.Format("Errore nel salvataggio dei Ruoli: {0}", ex.Message))
        End Try

    End Sub

    Private Sub Salva_Contatti_Codici(contatto As IDictionary(Of String, Object), ByVal tipoOperazione As enum_TipoOperazioneDB)

        Try
            Dim piva As String = contatto(FIELD_MAPPER.PIVA)
            Dim sa_cod As Integer = CInt(contatto(FIELD_MAPPER.SA_COD))
            Dim cod_contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)

            Dim codici As List(Of IDictionary(Of String, Object)) = contatto("contatti_codici")

            For Each codice In codici
                Dim Dummy = _objContatti_CodiciWrite.Scrivi(
                                               piva,
                                               sa_cod,
                                               cod_contatto,
                                               CInt(codice(FIELD_MAPPER.ID_COD)),
                                               codice(FIELD_MAPPER.VAL_COD),
                                               CDate(codice(FIELD_MAPPER.VALIDITA_INIZIO)),
                                               CDate(codice(FIELD_MAPPER.VALIDITA_FINE)),
                                               _objParametri_Server,
                                               CDate(codice(FIELD_MAPPER.DATA_CREAZIONE)),
                                               CDate(codice(FIELD_MAPPER.DATA_MODIFICA)),
                                               codice(FIELD_MAPPER.USERNAME_CREAZIONE),
                                               codice(FIELD_MAPPER.USERNAME_MODIFICA)
                                       )

            Next
        Catch ex As Exception
            Throw New Exception(String.Format("Errore nel salvataggio dei Codici del contatto: {0}", ex.Message))
        End Try

    End Sub

    Private Sub Salva_Indirizzi(contatto As IDictionary(Of String, Object),
                                     ByVal tipoOperazione As enum_TipoOperazioneDB)

        Try
            Dim piva As String = contatto(FIELD_MAPPER.PIVA)
            Dim sa_cod As Integer = CInt(contatto(FIELD_MAPPER.SA_COD))
            Dim cod_contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)

            Dim indirizzi As List(Of IDictionary(Of String, Object)) = contatto("indirizzi")
            Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

            For Each indirizzo In indirizzi
                Dim cod_Indirizzo = objSequenze.NuovoId_Tabella("Indirizzi", BaseCode, TopCode, _objParametri_Server)

                ' Indirizzo
                Dim Dummy = _objIndirizziWrite.Scrivi(
                            CLng(cod_Indirizzo),
                            indirizzo(FIELD_MAPPER.IND_DES),
                            indirizzo(FIELD_MAPPER.FRZ_DES),
                            indirizzo(FIELD_MAPPER.CAP),
                            indirizzo(FIELD_MAPPER.COM_DES),
                            indirizzo(FIELD_MAPPER.PRO_COD),
                            indirizzo(FIELD_MAPPER.STATO),
                            indirizzo(FIELD_MAPPER.NOTE),
                            indirizzo(FIELD_MAPPER.PRO_COD_ISTAT),
                            indirizzo(FIELD_MAPPER.COM_COD_ISTAT),
                            CDate(indirizzo(FIELD_MAPPER.VALIDITA_INIZIO)),
                            CDate(indirizzo(FIELD_MAPPER.VALIDITA_FINE)),
                            _objParametri_Server,
                            CDate(indirizzo(FIELD_MAPPER.DATA_CREAZIONE)),
                            CDate(indirizzo(FIELD_MAPPER.DATA_MODIFICA)),
                            indirizzo(FIELD_MAPPER.USERNAME_CREAZIONE),
                            indirizzo(FIELD_MAPPER.USERNAME_MODIFICA),
                            Codice_Lingua:=indirizzo(FIELD_MAPPER.CODICE_LINGUA)
                )

                ' Contatto x Indirizzo
                Dummy = _objContattixIndirizziWrite.Scrivi(
                                                piva,
                                                sa_cod,
                                                cod_contatto,
                                                cod_Indirizzo,
                                                CInt(indirizzo(FIELD_MAPPER.TIPO_INDIRIZZO)),
                                                CDate(indirizzo(FIELD_MAPPER.VALIDITA_INIZIO)),
                                                CDate(indirizzo(FIELD_MAPPER.VALIDITA_FINE)),
                                                _objParametri_Server,
                                                CDate(indirizzo(FIELD_MAPPER.DATA_CREAZIONE)),
                                                CDate(indirizzo(FIELD_MAPPER.DATA_MODIFICA)),
                                                indirizzo(FIELD_MAPPER.USERNAME_CREAZIONE),
                                                indirizzo(FIELD_MAPPER.USERNAME_MODIFICA)
                                        )
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("Errore nel salvataggio degli Indirizzi: {0}", ex.Message))
        End Try

    End Sub

    Private Sub Salva_Documenti(contatto As IDictionary(Of String, Object))

        Dim idsDocGestiti As New List(Of Integer)

        Try

            Dim piva As String = contatto(FIELD_MAPPER.PIVA)
            Dim sa_cod As Integer = CInt(contatto(FIELD_MAPPER.SA_COD))
            Dim cod_contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)
            Dim tipoDocumento = enum_ID_Area_Tipologia.Patentino_trattamenti
            Dim documenti As List(Of IDictionary(Of String, Object)) = contatto("documenti")

            If Not IsNothing(documenti) AndAlso documenti.Count > 0 Then

                For Each doc In documenti

                    Dim newIdElenco As Integer = -1

                    Dim ID_Tipologia As Integer = enum_ID_Area_Tipologia.Patentino_trattamenti
                    Dim ID_Area As Integer = enum_ID_Area_Alert.Contatti

                    Dim ID_Alert_Entita As Integer = CInt(doc(FIELD_MAPPER.ID_ALERT_ENTITA))
                    Dim ID_Elenco As Integer = CInt(doc(FIELD_MAPPER.ID_ELENCO))
                    Dim Allegati_Documenti_Cod As Integer = CInt(doc(FIELD_MAPPER.ALLEGATI_DOCUMENTI_COD))

                    Dim codice As String = doc(FIELD_MAPPER.CODICE)
                    Dim descrizione As String = doc(FIELD_MAPPER.DESCRIZIONE_SCADENZA)
                    Dim numDocumento As String = doc(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NUMERO)
                    Dim dataRilascio As DateTime = CDate(doc(FIELD_MAPPER.VALIDAZIONE_DATA))
                    Dim dataScadenza As DateTime = CDate(doc(FIELD_MAPPER.DATA_SCADENZA))
                    Dim ente As String = doc(FIELD_MAPPER.ALLEGATI_DOCUMENTI_ENTE_DES)

                    Dim fileName As String = doc(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOMEFILE)                                      ' TODO arriverà nel payload (forse)
                    Dim fileAllegato As String = String.Empty                                                                   ' TODO arriverà nel payload (forse)

                    Dim retVal As String = String.Empty

                    retVal = _objAlertWrite.Salva_Patentino(piva,
                                                 cod_contatto,
                                                 ID_Tipologia,
                                                 numDocumento,
                                                 ente,
                                                 dataRilascio,
                                                 descrizione,
                                                 dataScadenza,
                                                 fileName,
                                                 fileAllegato,
                                                 ID_Elenco,
                                                 ID_Alert_Entita,
                                                 Allegati_Documenti_Cod,
                                                 _objParametri_Server,
                                                 _objParametri_Utenti,
                                                 New_ID_Elenco:=newIdElenco,
                                                 Note_Log:=NOTELOG_ANAGRAFE_CONTATTI_DEMETRA,
                                                 Origine:=enum_SistemiEsterni.demetra)


                    ' todo check retval se non vuoto throw new AnnaException

                    If ID_Elenco <= 0 Then

                        ' INSERIMENTO

                        _interscambio.Scrivi_Tabella_Interscambio_Documenti(piva, cod_contatto, newIdElenco, codice)
                        idsDocGestiti.Add(newIdElenco)

                    Else

                        ' MODIFICA
                        idsDocGestiti.Add(ID_Elenco)

                    End If

                    newIdElenco = -1

                Next

            End If

            ' CANCELLAZIONE PATENTINI RIMASTI IN GIAS ED ELIMINATI DA DEMETRA
            Dim documentiPresentiInGias = _dataLoader.Leggi_Documenti(piva, cod_contatto)
            If Not IsNothing(documentiPresentiInGias) AndAlso documentiPresentiInGias.Count > 0 Then

                Dim patentini = documentiPresentiInGias.Where(Function(d) CInt(d(FIELD_MAPPER.ID_TIPOLOGIA)) = enum_ID_Area_Tipologia.Patentino_trattamenti)

                If Not IsNothing(patentini) AndAlso patentini.Count > 0 Then

                    For Each p In patentini
                        Dim Id_elenco As Integer = CInt(p(FIELD_MAPPER.ID_Elenco))

                        If Not idsDocGestiti.Contains(Id_elenco) Then

                            If Not _objAlertWrite.Cancella_Allegato(Id_elenco, "", _objParametri_Server,,, NOTELOG_ANAGRAFE_CONTATTI_DEMETRA, enum_SistemiEsterni.demetra) Then
                                Throw New Exception("Si è verificato un errore durante la cancellazione dei patentini")
                            End If

                            Dim recInterscambioDoc = _interscambio.Documenti_Leggi_Tabella_Interscambio_Chiave_GIAS(piva, cod_contatto, Id_elenco)
                            If Not IsNothing(recInterscambioDoc) AndAlso recInterscambioDoc.Count > 0 Then
                                Dim codiceEsterno As String = recInterscambioDoc("codice_esterno").ToString
                                _interscambio.Elimina_Da_Interscambio_Documenti(piva, cod_contatto, Id_elenco, codiceEsterno)
                            End If

                        End If

                    Next

                End If

            End If

        Catch ex As Exception
            Throw New Exception(String.Format("Errore nel salvataggio dei documenti: {0}", ex.Message))
        End Try

    End Sub

    Private Sub CambiaDataScadenzaAllegato(ByVal Piva As String,
                                                       ByVal Cod_Contatto As String,
                                                       ByVal Data_Rilascio As Date,
                                                       ByVal Data_Scadenza As Date)

        Dim xFiltroAggiuntivo = "Alert_Elenco.Data_Scadenza > '" & Data_Rilascio & "'"

        'Leggo allegati con data scadenza successiva
        Dim dati As DataTable = _dataLoader.Leggi_Allegati_Entita(Piva, Cod_Contatto, xFiltroAggiuntivo)

        If dati.Rows.Count > 0 Then
            Dim ID_Elenco = dati(0).Item(FIELD_MAPPER.ID_Elenco)
            Dim Descrizione_Scadenza = dati(0).Item(FIELD_MAPPER.DESCRIZIONE_SCADENZA)
            Dim NewDataScadenza = Data_Rilascio.AddDays(-1)

            _objAlertElencoWrite.ModificaAutomatica(ID_Elenco, NewDataScadenza, Descrizione_Scadenza, _objParametri_Server)
        End If

    End Sub


End Class
Public Class D2G_Contatto_Loader

    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Utenti As AgronicaCoreParametri = Nothing

    Private _objContattiRead As AgronicaCoreAnagrafeDAL.Contatti_R = Nothing
    Private _objRisorse_UmaneRead As AgronicaCoreAnagrafeDAL.Risorse_Umane_R = Nothing
    Private _objDocumentiRead As AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R = Nothing
    Private _objContatto_MultiHost_R As AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R = Nothing
    Private _objImpreseRead As AgronicaCoreAnagrafeDAL.Imprese_Read = Nothing
    Private _objMovNCRead As AgronicaCoreContabDAL.Movimenti_Dettagli_R = Nothing
    Private _objMovCRead As AgronicaCoreContabDAL.Movimenti_R = Nothing
    Private _objSquadreRead As AgronicaCoreContabDAL.CDG_DAL_R = Nothing
    Private _objImpCodRead As New Imprese_Codici_Read
    Private _objLogContattiRead As New Agronica_Log_Contatti_R
    Private _objRapportiContabili As AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R = Nothing
    Private _objImpostazioniRead As AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read = Nothing
    Private _objConfSitiRead As AgronicaCoreVarieDAL.Configurazione_Siti_R = Nothing

    Public Sub New()

    End Sub


    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Server = objParametri_Server.CreateDeepCopy(objParametri_Server)
        _objParametri_Utenti = objParametri_Utenti
        _objContattiRead = New AgronicaCoreAnagrafeDAL.Contatti_R()
        _objRisorse_UmaneRead = New AgronicaCoreAnagrafeDAL.Risorse_Umane_R()
        _objDocumentiRead = New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R()
        _objContatto_MultiHost_R = New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R()
        _objMovNCRead = New AgronicaCoreContabDAL.Movimenti_Dettagli_R()
        _objMovCRead = New AgronicaCoreContabDAL.Movimenti_R()
        _objSquadreRead = New AgronicaCoreContabDAL.CDG_DAL_R()
        _objImpreseRead = New AgronicaCoreAnagrafeDAL.Imprese_Read()
        _objImpCodRead = New Imprese_Codici_Read()
        _objLogContattiRead = New Agronica_Log_Contatti_R
        _objRapportiContabili = New Rapporti_Contabili_R()
        _objImpostazioniRead = New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
        _objConfSitiRead = New AgronicaCoreVarieDAL.Configurazione_Siti_R()

    End Sub

    Public Function Esiste_Contatto(ByVal piva As String, ByVal cod_contatto As String) As Boolean

        Dim risultato As Boolean = False

        Dim dt As DataTable = _objContattiRead.LeggiContattoSpecifico(piva,
                                                                      cod_contatto,
                                                                      0,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                      "", "", _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return risultato = True
        End If

        Return risultato

    End Function

    Public Function Ricerca_Contatto(ByVal piva As String, ByVal xFitlroAggiuntivo As String) As IDictionary(Of String, Object)

        Dim dt As DataTable = _objContattiRead.LeggiContattoSpecifico(piva, "", 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFitlroAggiuntivo, "", _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return dt.ToExpandoObjectCaseInsensitive().FirstOrDefault
        End If

        Return Nothing

    End Function


    Public Function Leggi_CUAA(ByVal piva As String) As String

        Return _objImpCodRead.Leggi_CUAA(piva, _objParametri_Server)

    End Function

    Public Function Esiste_Risorsa_Umana(codRisUm As Integer) As Boolean

        Return _objRisorse_UmaneRead.Esiste_CodRisUm(codRisUm, _objParametri_Server)

    End Function

    Public Function Piva_From_CUAA(ByVal CUAA As String) As String

        Return _objImpCodRead.Piva_from_CUAA(CUAA, _objParametri_Server)

    End Function

    Public Function Leggi_Rapporti_Contabili_Ammessi(ByVal xFiltroAggiuntivo As String) As List(Of IDictionary(Of String, Object))

        Dim dtRapportiContabili As DataTable = _objRapportiContabili.RapportiContabili_Leggi(String.Empty, xFiltroAggiuntivo, "", _objParametri_Server)
        If Not IsNothing(dtRapportiContabili) AndAlso dtRapportiContabili.Rows.Count > 0 Then
            Return dtRapportiContabili.ToExpandoObjectCaseInsensitive()
        End If

        Return Nothing

    End Function

    Public Function Contatti_NonInviati(ByVal piveAmmesse As List(Of String)) As List(Of IDictionary(Of String, Object))

        Dim dt As DataTable = _objLogContattiRead.Contatti_NonInviati(enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC, piveAmmesse, "", "", _objParametri_Server, enum_SistemiEsterni.demetra)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return dt.ToExpandoObjectCaseInsensitive()
        End If

        Return Nothing

    End Function

    Public Function Carica_Contatto_Esistente(
                                             ByVal piva As String,
                                             ByVal cod_Contatto As String,
                                             Optional ByVal caricaDocumenti As Boolean = False,
                                             Optional ByVal caricaRisorseUmane As Boolean = False) As IDictionary(Of String, Object)

        Dim contattoEsistenta As IDictionary(Of String, Object) = Nothing

        ' 1) lettura contatto
        Dim dt As DataTable = _objContattiRead.LeggiContattoSpecifico(piva,
                                                                      cod_Contatto,
                                                                      0,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                      "", "", _objParametri_Server)
        If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
            Return Nothing
        End If
        contattoEsistenta = dt.ToExpandoObjectCaseInsensitive().FirstOrDefault()

        ' 2) Lettura rapporti_contabili (ruoli)
        If caricaRisorseUmane Then
            Dim rapportiContabili As New List(Of IDictionary(Of String, Object))
            'Dim xFiltro = " (Rapporti_Contabili.terzista = 1 Or Rapporti_Contabili.dipendente = 1) "
            Dim xFiltro = ""
            dt = _objRisorse_UmaneRead.LeggiSoloContatto(piva,
                                                         0,
                                                         cod_Contatto,
                                                         0,
                                                         0,
                                                         "",
                                                         True,
                                                         AGRODATAINIZIO,
                                                         AGRODATAFINE,
                                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                         xFiltro, "", _objParametri_Server)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                rapportiContabili = dt.ToExpandoObjectCaseInsensitive()
                For Each rp In rapportiContabili
                    If Not rp.ContainsKey(FIELD_MAPPER.FLAG_CANCELLAZIONE) Then
                        rp(FIELD_MAPPER.FLAG_CANCELLAZIONE) = False
                    End If
                    If Not rp.ContainsKey(FIELD_MAPPER.FLAG_CANCELLABILE) Then
                        rp(FIELD_MAPPER.FLAG_CANCELLABILE) = False
                    End If
                Next
            End If
            contattoEsistenta.Add("rapporti_contabili", rapportiContabili)
        End If

        If caricaDocumenti Then
            Dim allegati As New List(Of IDictionary(Of String, Object))
            Dim dtDocs As DataTable = Leggi_Allegati_Entita(piva, cod_Contatto, "")
            If Not IsNothing(dtDocs) AndAlso dtDocs.Rows.Count > 0 Then
                allegati = dtDocs.ToExpandoObjectCaseInsensitive()
            End If
            contattoEsistenta("documenti") = allegati
        End If

        Return contattoEsistenta

    End Function

    Public Function Leggi_Documenti(ByVal piva As String, ByVal cod_contatto As String) As List(Of IDictionary(Of String, Object))

        Dim dtDocs As DataTable = Leggi_Allegati_Entita(piva, cod_contatto, "")
        If Not IsNothing(dtDocs) AndAlso dtDocs.Rows.Count > 0 Then
            Return dtDocs.ToExpandoObjectCaseInsensitive()
        Else
            Return Nothing
        End If

    End Function

    Public Function Leggi_Documento(ByVal piva As String, ByVal cod_contatto As String, ByVal ID_Elenco As Integer) As IDictionary(Of String, Object)

        Dim dt As DataTable = _objDocumentiRead.Leggi_Allegati_Entita(piva, cod_contatto, 0, ID_Elenco, 0, 0, 0, 0, "", "Alert_Elenco.Data_Scadenza DESC", _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return dt.ToExpandoObjectCaseInsensitive().FirstOrDefault()
        End If

        Return Nothing

    End Function

    Public Function Leggi_Risorsa_Umana(ByVal piva As String, ByVal cod_Contatto As String, ByVal cod_RisUm As Integer) As IDictionary(Of String, Object)

        Dim risorsaUmana As IDictionary(Of String, Object) = Nothing
        Dim dt As DataTable = Nothing
        dt = _objRisorse_UmaneRead.LeggiSoloContatto(CStr(piva),
                                                        cod_RisUm,
                                                        cod_Contatto,
                                                        0, 0, "", True,
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            risorsaUmana = dt.ToExpandoObjectCaseInsensitive().FirstOrDefault()
        End If

        Return risorsaUmana

    End Function


    Public Function Leggi_Risorse_Umane_Contatto(ByVal piva As String, ByVal cod_Contatto As String) As List(Of IDictionary(Of String, Object))

        Dim rapportiContabili As New List(Of IDictionary(Of String, Object))
        Dim dt As DataTable = Nothing
        dt = _objRisorse_UmaneRead.LeggiSoloContatto(CStr(piva),
                                                        0,
                                                        cod_Contatto,
                                                        0, 0, "", True,
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", _objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            rapportiContabili = dt.ToExpandoObjectCaseInsensitive()
        End If

        Return rapportiContabili

    End Function

    Public Function Leggi_Movimenti_Non_Contabili(ByVal cod_RisUM As Integer) As DataTable

        Dim dt As DataTable = Nothing
        dt = _objMovNCRead.Leggi("", 0, 0, 0, 0, 0, 0, cod_RisUM, "", 0, 0, 0,
                                                   0, 0, 0,
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                   "Elem_Cod = 0", "", _objParametri_Server)

        Return dt

    End Function

    Public Function Leggi_Movimenti_Contabili(ByVal cod_RisUM As Integer) As DataTable

        Dim dt As DataTable = Nothing
        dt = _objMovCRead.Leggi("", 0, 0, 0,
                                                cod_RisUM,
                                                CAU_REGISTRAZIONI,
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "", _objParametri_Server)
        Return dt

    End Function

    Public Function Leggi_SquadreXAttivita(ByVal cod_RisUM As Integer) As DataTable

        Dim dt As DataTable = Nothing
        dt = _objSquadreRead.Leggi_SquadrexAttvita("", 0, 0, 0, 0,
                                                   " SquadrexAttivita.cod_risum_list LIKE '" & UtilityProvider.Agro_SQL_SaveText("%" & cod_RisUM & "%") & "'",
                                                   AGRODATAINIZIO, _objParametri_Server)

        Return dt

    End Function

    Public Function VerificaEsistenza_PivaGIAS(ByVal piva As String) As Boolean

        Dim dt As DataTable = Nothing
        dt = _objImpreseRead.Leggi(piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", _objParametri_Server)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Public Function Leggi_Allegati_Entita(piva As String, cod_contatto As String, xFiltroAggiuntivo As String) As DataTable

        Return _objDocumentiRead.Leggi_Allegati_Entita(piva, cod_contatto, 0, 0, 0, 0, 0, 0, xFiltroAggiuntivo, "Alert_Elenco.Data_Scadenza DESC", _objParametri_Server)

    End Function

    Public Function Controlla_Per_Eliminazione_Rapporto_Contabile(ByVal piva As String,
                            ByVal codContatto As String,
                            ByVal codRisUm As Integer,
                            ByVal sa_Cod As Integer,
                            ByVal desRapporto As String
                        ) As String

        Dim risultato As String = String.Empty
        If codRisUm = 0 Then
            Return risultato
        End If

        'Movimenti contabili
        Dim dtMov As DataTable = _objContatto_MultiHost_R.Contatto_ControllaMovimentiXRisorsaUmana(piva, codContatto, codRisUm, "", _objParametri_Server)
        If dtMov.Rows.Count <> 0 Then
            risultato = $"Esistono dei movimenti contabili associati il contatto corrente. Impossibile cancellare il rapporto contabile di tipo {desRapporto}"
            Return risultato
        End If

        Dim dtAltriRiferimenti As DataTable = _objContatto_MultiHost_R.ContattoRiferimenti(piva, codContatto, codRisUm, sa_Cod, "", _objParametri_Server)
        If dtAltriRiferimenti.Rows.Count > 0 Then
            risultato = $"Esistono dei riferimenti associati al contatto corrente. Impossibile cancellare il rapporto contabile di tipo {desRapporto}"
        End If

        Return risultato


    End Function


    Public Function Leggi_Contatti_Riferimenti(ByVal piva As String, ByVal cod_Contatto As String) As DataTable

        Dim dt As DataTable = _objContatto_MultiHost_R.ContattoRiferimenti(piva, cod_Contatto, 0, 0, "", _objParametri_Server)
        Return dt

    End Function

    Public Function Leggi_Contatto_Completo_XML(ByVal piva As String, ByVal cod_Contatto As String) As String

        'Leggo la stringa facendomi ritornare Tipooperazione = 3 (x la cancellazione)
        Return _objContatto_MultiHost_R.Contatto_Leggi(
                                                piva,
                                                cod_Contatto,
                                                "",
                                                True,
                                                _objParametri_Server)
    End Function

    Public Function Leggi_WorkFlow() As String

        Dim workFlow As String = String.Empty
        Dim MessaggioErrrore As String = String.Empty
        Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
        Dim dt As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, "", "", MessaggioErrrore, _objParametri_Server)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            workFlow = CStr(dt.Rows(0).Item("Valore1"))
        End If

        Return workFlow

    End Function

    Public Function Leggi_Tipo_Salva_Allegato() As Integer

        Dim DTSalvaAllegato As DataTable = _objImpostazioniRead.Leggi2(2, _objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", _objParametri_Utenti)
        If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
            Return CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
        End If

        Return 0

    End Function

    Public Function Leggi_Percorso_Allegati() As String

        Dim Percorso As String = String.Empty
        Dim dt_Conf As DataTable = _objConfSitiRead.Leggi(6, "GestioneAllegati_Repository", "", "", _objParametri_Server)
        If Not IsNothing(dt_Conf) AndAlso dt_Conf.Rows.Count > 0 Then
            Percorso = dt_Conf.Rows(0).Item("Valore")
        End If

        Return Percorso

    End Function

End Class

Public Class D2G_Contatti_Import_Mapper

    Private _objParametri_Utente As AgronicaCoreParametri = Nothing
    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _intercambio As D2G_Interscambio_Contatti = Nothing
    Private _loader As D2G_Contatto_Loader = Nothing

    Public Property BaseCode As Integer
    Public Property TopCode As Integer

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utente As AgronicaCoreParametri,
                   ByVal intercambio As D2G_Interscambio_Contatti,
                   ByVal loader As D2G_Contatto_Loader
                   )

        _objParametri_Server = objParametri_Server
        _objParametri_Utente = objParametri_Utente
        _intercambio = intercambio
        _loader = loader

    End Sub

    Public Function Crea_Mappa_Risorsa_Umana_Nuova(piva As String,
                                             sa_cod As Integer,
                                             cod_contatto As String,
                                             codice As String,
                                             cod_rapporto As Integer,
                                             cod_risum As Integer,
                                             utente_modifica As String,
                                             data_modifica As DateTime,
                                             validita_inizio As DateTime,
                                             validita_Fine As DateTime,
                                             flag_cancellazione As Boolean
                                             ) As IDictionary(Of String, Object)

        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando

        dictionary(FIELD_MAPPER.PIVA) = piva
        dictionary(FIELD_MAPPER.SA_COD) = sa_cod
        dictionary(FIELD_MAPPER.COD_RISUM) = cod_risum
        dictionary(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        dictionary(FIELD_MAPPER.CODICE) = codice
        dictionary(FIELD_MAPPER.COD_RAPPORTO) = cod_rapporto
        dictionary(FIELD_MAPPER.VALIDITA_INIZIO) = validita_inizio
        dictionary(FIELD_MAPPER.VALIDITA_FINE) = validita_Fine
        dictionary(FIELD_MAPPER.SETTORE_DES) = ""
        dictionary(FIELD_MAPPER.ATTIVITA_DES) = ""
        dictionary(FIELD_MAPPER.CORRISPETTIVO_MENSILE) = 0
        dictionary(FIELD_MAPPER.CORRISPETTIVO_ORARIO) = 0
        dictionary(FIELD_MAPPER.OCCASIONALE) = 0
        dictionary(FIELD_MAPPER.ORE_SETTIMANALI) = 0
        dictionary(FIELD_MAPPER.GIORNI_FERIE) = 0
        dictionary(FIELD_MAPPER.FERIE_GODUTE) = 0
        dictionary(FIELD_MAPPER.GIORNI_MALATTIA) = 0
        dictionary(FIELD_MAPPER.DATA_CREAZIONE) = #2/1/1900#
        dictionary(FIELD_MAPPER.DATA_MODIFICA) = data_modifica
        dictionary(FIELD_MAPPER.USERNAME_CREAZIONE) = utente_modifica
        dictionary(FIELD_MAPPER.USERNAME_MODIFICA) = utente_modifica
        dictionary(FIELD_MAPPER.PATENTINO) = String.Empty
        dictionary(FIELD_MAPPER.DATA_RILASCIO_PATENTINO) = AGRODATAINIZIO
        dictionary(FIELD_MAPPER.DATA_SCADENZA_PATENTINO) = AGRODATAFINE
        dictionary(FIELD_MAPPER.COD_RISUM_ORIGINE) = 0
        dictionary(FIELD_MAPPER.PIVA_SUPERUSER_ORIGINE) = String.Empty
        dictionary(FIELD_MAPPER.ENTE_DI_RILASCIO) = String.Empty
        dictionary(FIELD_MAPPER.SALDO_INIZIALE_CREDITI) = 0
        dictionary(FIELD_MAPPER.SALDO_INIZIALE_DEBITI) = 0
        dictionary(FIELD_MAPPER.CHKSPESOMETRO) = 0
        dictionary(FIELD_MAPPER.CHKBLOCCO) = 0
        dictionary(FIELD_MAPPER.BLOCCO_DES) = String.Empty
        dictionary(FIELD_MAPPER.CLASSIFICAZIONE_COD) = 0
        dictionary(FIELD_MAPPER.QUALIFICA_COD) = 0
        dictionary(FIELD_MAPPER.MANSIONE_COD) = 0
        dictionary(FIELD_MAPPER.INFO_FAMIGLIA) = String.Empty
        dictionary(FIELD_MAPPER.COD_IVA_CONTATTO) = -1
        dictionary(FIELD_MAPPER.COD_CONTO_ECON) = 0
        dictionary(FIELD_MAPPER.COD_CONTO_PAT) = 0
        dictionary(FIELD_MAPPER.FLAG_CANCELLAZIONE) = flag_cancellazione
        dictionary(FIELD_MAPPER.FLAG_CANCELLABILE) = False
        dictionary(FIELD_MAPPER.TIPO_OPERAZIONE_DB) = enum_TipoOperazioneDB.Scrittura

        Return dictionary

    End Function

    Public Function Crea_Mappa_Risorsa_UMana_Esistente(risorsaUmanaGias As IDictionary(Of String, Object),
                                                        piva As String,
                                                        sa_cod As Integer,
                                                        cod_contatto As String,
                                                        codice As String,
                                                        cod_rapporto As Integer,
                                                        utente_modifica As String,
                                                        validita_inizio As DateTime,
                                                        validita_Fine As DateTime,
                                                        flag_cancellazione As Boolean) As IDictionary(Of String, Object)

        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando

        dictionary(FIELD_MAPPER.PIVA) = piva
        dictionary(FIELD_MAPPER.SA_COD) = sa_cod
        dictionary(FIELD_MAPPER.COD_RISUM) = CInt(risorsaUmanaGias(FIELD_MAPPER.COD_RISUM))
        dictionary(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        dictionary(FIELD_MAPPER.CODICE) = codice
        dictionary(FIELD_MAPPER.COD_RAPPORTO) = cod_rapporto
        dictionary(FIELD_MAPPER.VALIDITA_INIZIO) = validita_inizio
        dictionary(FIELD_MAPPER.VALIDITA_FINE) = validita_Fine
        dictionary(FIELD_MAPPER.SETTORE_DES) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.SETTORE_DES)), "", risorsaUmanaGias(FIELD_MAPPER.SETTORE_DES))
        dictionary(FIELD_MAPPER.ATTIVITA_DES) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.ATTIVITA_DES)), "", risorsaUmanaGias(FIELD_MAPPER.ATTIVITA_DES))
        dictionary(FIELD_MAPPER.CORRISPETTIVO_MENSILE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.CORRISPETTIVO_MENSILE)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.CORRISPETTIVO_MENSILE)))
        dictionary(FIELD_MAPPER.CORRISPETTIVO_ORARIO) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.CORRISPETTIVO_ORARIO)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.CORRISPETTIVO_ORARIO)))
        dictionary(FIELD_MAPPER.OCCASIONALE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.OCCASIONALE)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.OCCASIONALE)))
        dictionary(FIELD_MAPPER.ORE_SETTIMANALI) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.ORE_SETTIMANALI)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.ORE_SETTIMANALI)))
        dictionary(FIELD_MAPPER.GIORNI_FERIE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.GIORNI_FERIE)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.GIORNI_FERIE)))
        dictionary(FIELD_MAPPER.FERIE_GODUTE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.FERIE_GODUTE)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.FERIE_GODUTE)))
        dictionary(FIELD_MAPPER.GIORNI_MALATTIA) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.GIORNI_MALATTIA)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.GIORNI_MALATTIA)))
        dictionary(FIELD_MAPPER.DATA_CREAZIONE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.DATA_CREAZIONE)), #2/1/1900#, CDate(risorsaUmanaGias(FIELD_MAPPER.DATA_CREAZIONE)))
        dictionary(FIELD_MAPPER.DATA_MODIFICA) = DateTime.Now
        dictionary(FIELD_MAPPER.USERNAME_CREAZIONE) = utente_modifica
        dictionary(FIELD_MAPPER.USERNAME_MODIFICA) = utente_modifica
        dictionary(FIELD_MAPPER.PATENTINO) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.PATENTINO)), "", risorsaUmanaGias(FIELD_MAPPER.PATENTINO))
        dictionary(FIELD_MAPPER.DATA_RILASCIO_PATENTINO) = AGRODATAINIZIO
        dictionary(FIELD_MAPPER.DATA_SCADENZA_PATENTINO) = AGRODATAFINE
        dictionary(FIELD_MAPPER.COD_RISUM_ORIGINE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.COD_RISUM_ORIGINE)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.COD_RISUM_ORIGINE)))
        dictionary(FIELD_MAPPER.PIVA_SUPERUSER_ORIGINE) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.PIVA_SUPERUSER_ORIGINE)), "", risorsaUmanaGias(FIELD_MAPPER.PIVA_SUPERUSER_ORIGINE))
        dictionary(FIELD_MAPPER.ENTE_DI_RILASCIO) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.ENTE_DI_RILASCIO)), "", risorsaUmanaGias(FIELD_MAPPER.ENTE_DI_RILASCIO))
        dictionary(FIELD_MAPPER.SALDO_INIZIALE_CREDITI) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.SALDO_INIZIALE_CREDITI)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.SALDO_INIZIALE_CREDITI)))
        dictionary(FIELD_MAPPER.SALDO_INIZIALE_DEBITI) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.SALDO_INIZIALE_DEBITI)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.SALDO_INIZIALE_DEBITI)))
        dictionary(FIELD_MAPPER.CHKSPESOMETRO) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.CHKSPESOMETRO)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.CHKSPESOMETRO)))
        dictionary(FIELD_MAPPER.CHKBLOCCO) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.CHKBLOCCO)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.CHKBLOCCO)))
        dictionary(FIELD_MAPPER.BLOCCO_DES) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.BLOCCO_DES)), "", risorsaUmanaGias(FIELD_MAPPER.BLOCCO_DES))
        dictionary(FIELD_MAPPER.CLASSIFICAZIONE_COD) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.CLASSIFICAZIONE_COD)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.CLASSIFICAZIONE_COD)))
        dictionary(FIELD_MAPPER.QUALIFICA_COD) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.QUALIFICA_COD)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.QUALIFICA_COD)))
        dictionary(FIELD_MAPPER.MANSIONE_COD) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.MANSIONE_COD)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.MANSIONE_COD)))
        dictionary(FIELD_MAPPER.INFO_FAMIGLIA) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.INFO_FAMIGLIA)), "", risorsaUmanaGias(FIELD_MAPPER.INFO_FAMIGLIA))
        dictionary(FIELD_MAPPER.COD_IVA_CONTATTO) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.COD_IVA_CONTATTO)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.COD_IVA_CONTATTO)))
        dictionary(FIELD_MAPPER.COD_CONTO_ECON) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.COD_CONTO_ECON)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.COD_CONTO_ECON)))
        dictionary(FIELD_MAPPER.COD_CONTO_PAT) = If(IsDBNull(risorsaUmanaGias(FIELD_MAPPER.COD_CONTO_PAT)), 0, CInt(risorsaUmanaGias(FIELD_MAPPER.COD_CONTO_PAT)))
        dictionary(FIELD_MAPPER.FLAG_CANCELLAZIONE) = flag_cancellazione
        dictionary(FIELD_MAPPER.FLAG_CANCELLABILE) = False
        dictionary(FIELD_MAPPER.TIPO_OPERAZIONE_DB) = enum_TipoOperazioneDB.Modifica

        Return dictionary

    End Function

    Public Function Crea_Mappa_Contatto_Codice(piva As String,
                                               sa_cod As Integer,
                                               cod_contatto As String,
                                               id_cod As Integer,
                                               val_cod As String,
                                               userName As String
                                                ) As IDictionary(Of String, Object)

        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando

        dictionary(FIELD_MAPPER.PIVA) = piva
        dictionary(FIELD_MAPPER.SA_COD) = sa_cod
        dictionary(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        dictionary(FIELD_MAPPER.ID_COD) = id_cod
        dictionary(FIELD_MAPPER.VAL_COD) = val_cod
        dictionary(FIELD_MAPPER.VALIDITA_INIZIO) = AGRODATAINIZIO
        dictionary(FIELD_MAPPER.VALIDITA_FINE) = AGRODATAFINE

        'optional
        dictionary(FIELD_MAPPER.DATA_CREAZIONE) = #2/1/1900#
        dictionary(FIELD_MAPPER.DATA_MODIFICA) = #2/1/1900#
        dictionary(FIELD_MAPPER.USERNAME_CREAZIONE) = userName
        dictionary(FIELD_MAPPER.USERNAME_MODIFICA) = userName

        Return dictionary

    End Function

    Public Function Crea_Mappa_Documento_Nuovo(piva As String,
                                         codice As String,
                                         cod_contatto As String,
                                         pivaSuperUSer As String,
                                         numeroDocumento As String,
                                         descrizione As String,
                                         dataRilascio As DateTime,
                                         dataScadenza As DateTime,
                                         ByVal tipoOperazioneDB As enum_TipoOperazioneDB
                                        ) As IDictionary(Of String, Object)

        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando

        dictionary(FIELD_MAPPER.PIVA) = piva
        dictionary(FIELD_MAPPER.CODICE) = codice
        dictionary(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        dictionary(FIELD_MAPPER.PIVASUPERUSER) = pivaSuperUSer
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NUMERO) = numeroDocumento
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_COD) = 0
        dictionary(FIELD_MAPPER.DESCRIZIONE_SCADENZA) = descrizione
        dictionary(FIELD_MAPPER.VALIDAZIONE_DATA) = dataRilascio
        dictionary(FIELD_MAPPER.DATA_SCADENZA) = dataScadenza
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_ENTE_DES) = String.Empty
        dictionary(FIELD_MAPPER.ID_TIPOLOGIA) = enum_ID_Area_Tipologia.Patentino_trattamenti
        dictionary(FIELD_MAPPER.ID_ELENCO) = 0
        dictionary(FIELD_MAPPER.ID_ALERT_ENTITA) = 0
        dictionary(FIELD_MAPPER.VALIDAZIONE_FLAG) = 0
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOMEFILE) = String.Empty
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOTE) = String.Empty
        dictionary(FIELD_MAPPER.TIPO_OPERAZIONE_DB) = tipoOperazioneDB

        Return dictionary

    End Function

    Public Function Crea_Mappa_Documento_Esistente(piva As String,
                                         codice As String,
                                         cod_contatto As String,
                                         pivaSuperUSer As String,
                                         numeroDocumento As String,
                                         descrizione As String,
                                         dataRilascio As DateTime,
                                         dataScadenza As DateTime,
                                         ID_Elenco As Integer,
                                         ID_Alert_Entita As Integer,
                                         FileName As String,
                                         allegati_Documenti_Cod As Integer,
                                         validazione_Flag As Integer,
                                         allegati_Documenti_Note As String,
                                         dataUpload As DateTime,
                                         allegati_Documenti_Ente_Des As String,
                                         ByVal tipoOperazioneDB As enum_TipoOperazioneDB
                                        ) As IDictionary(Of String, Object)

        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando

        dictionary(FIELD_MAPPER.PIVA) = piva
        dictionary(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        dictionary(FIELD_MAPPER.PIVASUPERUSER) = pivaSuperUSer
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NUMERO) = numeroDocumento
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_COD) = allegati_Documenti_Cod
        dictionary(FIELD_MAPPER.DESCRIZIONE_SCADENZA) = descrizione
        dictionary(FIELD_MAPPER.VALIDAZIONE_DATA) = dataRilascio
        dictionary(FIELD_MAPPER.DATA_SCADENZA) = dataScadenza
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_ENTE_DES) = allegati_Documenti_Ente_Des
        dictionary(FIELD_MAPPER.ID_TIPOLOGIA) = enum_ID_Area_Tipologia.Patentino_trattamenti
        dictionary(FIELD_MAPPER.ID_ELENCO) = ID_Elenco
        dictionary(FIELD_MAPPER.ID_ALERT_ENTITA) = ID_Alert_Entita
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOMEFILE) = FileName
        dictionary(FIELD_MAPPER.CODICE) = codice
        dictionary(FIELD_MAPPER.VALIDAZIONE_FLAG) = validazione_Flag
        dictionary(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOTE) = allegati_Documenti_Note
        dictionary(FIELD_MAPPER.DATA_UPLOAD) = dataUpload
        dictionary(FIELD_MAPPER.TIPO_OPERAZIONE_DB) = tipoOperazioneDB

        Return dictionary

    End Function

    Public Function Crea_Mappa_Indirizzo(piva As String,
                                               sa_cod As Integer,
                                               cod_contatto As String,
                                               tipo_indirizzo As Integer,
                                               isEstero As Boolean,
                                               userName As String) As IDictionary(Of String, Object)


        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando
        Dim stato As String = String.Empty

        dictionary(FIELD_MAPPER.PIVA) = piva
        dictionary(FIELD_MAPPER.SA_COD) = sa_cod
        dictionary(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        dictionary(FIELD_MAPPER.COD_INDIRIZZO) = 0
        dictionary(FIELD_MAPPER.TIPO_INDIRIZZO) = tipo_indirizzo
        dictionary(FIELD_MAPPER.IND_DES) = String.Empty
        dictionary(FIELD_MAPPER.FRZ_DES) = String.Empty
        dictionary(FIELD_MAPPER.CAP) = String.Empty
        dictionary(FIELD_MAPPER.COM_DES) = String.Empty
        dictionary(FIELD_MAPPER.PRO_COD) = "00"
        If Not isEstero Then
            stato = "IT"
        Else
            stato = If(tipo_indirizzo = 201, "IT", "")
        End If
        dictionary(FIELD_MAPPER.STATO) = stato
        dictionary(FIELD_MAPPER.PRO_COD_ISTAT) = "000"
        dictionary(FIELD_MAPPER.COM_COD_ISTAT) = "000"
        dictionary(FIELD_MAPPER.NOTE) = String.Empty
        dictionary(FIELD_MAPPER.VALIDITA_INIZIO) = AGRODATAINIZIO
        dictionary(FIELD_MAPPER.VALIDITA_FINE) = AGRODATAFINE

        'optional
        dictionary(FIELD_MAPPER.VALIDAZIONE) = 0
        dictionary(FIELD_MAPPER.DATA_VALIDAZIONE) = #2/1/1900#
        dictionary(FIELD_MAPPER.USERNAME_VALIDAZIONE) = String.Empty
        dictionary(FIELD_MAPPER.CODICE_LINGUA) = "000"
        dictionary(FIELD_MAPPER.CODICE_ALTERNATIVO) = String.Empty
        dictionary(FIELD_MAPPER.DATA_CREAZIONE) = #2/1/1900#
        dictionary(FIELD_MAPPER.DATA_MODIFICA) = #2/1/1900#
        dictionary(FIELD_MAPPER.USERNAME_CREAZIONE) = userName
        dictionary(FIELD_MAPPER.USERNAME_MODIFICA) = userName

        Return dictionary

    End Function


    Public Function Crea_Mappa_Contatto_Default() As IDictionary(Of String, Object)

        Dim objExpando As New System.Dynamic.ExpandoObject()
        Dim dictionary As IDictionary(Of String, Object) = objExpando

        dictionary(FIELD_MAPPER.PIVA) = String.Empty
        dictionary(FIELD_MAPPER.SA_COD) = 0
        dictionary(FIELD_MAPPER.COD_CONTATTO) = String.Empty
        dictionary(FIELD_MAPPER.ID_CF) = 0
        dictionary(FIELD_MAPPER.RAG_SOC) = String.Empty
        dictionary(FIELD_MAPPER.CONVENEVOLI) = String.Empty
        dictionary(FIELD_MAPPER.CODICE_FISCALE) = String.Empty
        dictionary(FIELD_MAPPER.TIPO_INDIRIZZO_DEFAULT) = 0
        dictionary(FIELD_MAPPER.NOME) = String.Empty
        dictionary(FIELD_MAPPER.COGNOME) = String.Empty
        dictionary(FIELD_MAPPER.DATA_NASCITA) = AGRODATAINIZIO
        dictionary(FIELD_MAPPER.SESSO) = String.Empty
        dictionary(FIELD_MAPPER.COD_CONTATTO_REFERENTE) = String.Empty
        dictionary(FIELD_MAPPER.VALIDITA_INIZIO) = AGRODATAINIZIO
        dictionary(FIELD_MAPPER.VALIDITA_FINE) = AGRODATAFINE

        'optional
        dictionary(FIELD_MAPPER.DATA_CREAZIONE) = #2/1/1900#
        dictionary(FIELD_MAPPER.DATA_MODIFICA) = #2/1/1900#
        dictionary(FIELD_MAPPER.USERNAME_CREAZIONE) = String.Empty
        dictionary(FIELD_MAPPER.USERNAME_MODIFICA) = String.Empty
        dictionary(FIELD_MAPPER.TIPO_SPEDITORE) = 0
        dictionary(FIELD_MAPPER.TIPO_DESTINAZIONE) = 0
        dictionary(FIELD_MAPPER.AGENTE_COD) = 0
        dictionary(FIELD_MAPPER.PROVVIGIONE) = 0
        dictionary(FIELD_MAPPER.NOTE) = String.Empty
        dictionary(FIELD_MAPPER.ID_GESTIONE_NOTE) = 0
        dictionary(FIELD_MAPPER.NOTE2) = String.Empty
        dictionary(FIELD_MAPPER.NOTE_OPERAZIONI) = String.Empty
        dictionary(FIELD_MAPPER.NOTE2_OPERAZIONI) = String.Empty
        dictionary(FIELD_MAPPER.COD_RISUM_DESTINAZIONE_DIVERSA) = 0
        dictionary(FIELD_MAPPER.TIPO_INDIRIZZO_DEFAULT_DESTINAZIONE_DIVERSA) = 0
        dictionary(FIELD_MAPPER.FIDO) = 0.0
        dictionary(FIELD_MAPPER.LIMITE_POSIZIONI) = 0
        dictionary(FIELD_MAPPER.LIMITE_GIORNI_EVASIONE) = 0.0
        dictionary(FIELD_MAPPER.ORARI_RITIRO) = String.Empty
        dictionary(FIELD_MAPPER.FILTRO_RIMBORSI) = String.Empty
        dictionary(FIELD_MAPPER.VETTORE_COD) = 0
        dictionary(FIELD_MAPPER.CAPOAREA_COD) = 0
        dictionary(FIELD_MAPPER.PROVVIGIONE_CAPOAREA) = 0.0
        dictionary(FIELD_MAPPER.MEMO) = String.Empty
        dictionary(FIELD_MAPPER.SCONTO_CONTATTO) = 0.0
        dictionary(FIELD_MAPPER.SCONTO_TESTO) = String.Empty
        dictionary(FIELD_MAPPER.MODALITA_FATTURAZIONE) = 0
        dictionary(FIELD_MAPPER.COD_IVA_CONTATTO) = -1
        dictionary(FIELD_MAPPER.DOCUMENTO_FATTURAZIONE) = 0
        dictionary(FIELD_MAPPER.NRBADGE) = ""
        dictionary(FIELD_MAPPER.CHKFITTIZIO) = 0
        dictionary(FIELD_MAPPER.COD_CONTO_ECON) = -1
        dictionary(FIELD_MAPPER.COD_CONTO_PAT) = -1
        dictionary(FIELD_MAPPER.NOME_BREVE) = String.Empty

        Return dictionary

    End Function

    Public Function Mappa_Esistente(inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto),
                                    ByVal contattoGias As IDictionary(Of String, Object),
                                    ByVal piva As String,
                                    ByVal pivaSuperUser As String,
                                    ByVal rapportiContabiliGias As List(Of IDictionary(Of String, Object)),
                                    ByVal CUAA As String) As IDictionary(Of String, Object)

        Dim contattoDemetra = inData.elemento_anagrafico
        Dim decoUtenti As New AgronicaCoreDemetraBIZ.decodificaUtenti()
        Dim userName As String = decoUtenti.decoficaUtenteGiasDaUtenteDemetra("", CUAA, _objParametri_Utente)

        Dim cod_contatto As String = String.Empty
        Dim rag_soc As String = String.Empty
        Dim nome As String = String.Empty
        Dim cognome As String = String.Empty
        Dim id_cf As Integer = CInt(contattoDemetra.Id_CF)              ' persona fisica / giuridica / estero se 2 estero la persona è giuridica
        Dim isEstero As Boolean = (id_cf = CONTATTO_ESTERO)

        If isEstero Then
            id_cf = PERSONA_GIURIDICA
        End If

        cod_contatto = contattoGias(FIELD_MAPPER.COD_CONTATTO)
        Select Case id_cf
            Case PERSONA_FISICA
                nome = contattoDemetra.nome
                cognome = contattoDemetra.cognome

                If String.IsNullOrEmpty(nome) Then
                    Throw New Exception("Il campo Nome non è stato valorizzato")
                End If

                If String.IsNullOrEmpty(nome) Then
                    Throw New Exception("Il campo Cognome non è stato valorizzato")
                End If

                contattoGias(FIELD_MAPPER.NOME) = nome
                contattoGias(FIELD_MAPPER.COGNOME) = cognome

            Case PERSONA_GIURIDICA

                rag_soc = contattoDemetra.ragione_Sociale
                If String.IsNullOrEmpty(rag_soc) Then
                    Throw New Exception("Il campo Ragione_Sociale non è stato valorizzato")
                End If

                contattoGias(FIELD_MAPPER.RAG_SOC) = rag_soc

        End Select
        contattoGias(FIELD_MAPPER.USERNAME_MODIFICA) = userName

#Region "OLD MAPPING"
        ' Rapporti contabili
        'Dim rapportiContabiliEsistenti As List(Of IDictionary(Of String, Object)) = contattoGias("rapporti_contabili")

        'For Each ruolo In contattoDemetra.ruolo

        '    Dim codRapporto As Integer = ruolo.cod_rapporto
        '    Dim validitaInizio As DateTime = ruolo.validita.inizio

        '    Dim userNameRuolo = decoUtenti.decoficaUtenteGiasDaUtenteDemetra(ruolo.utente_ultima_modifica, CUAA, _objParametri_Utente)

        '    'cerco il rapporto contabile per cod rapporta e data inizio
        '    Dim rapportoEsistenti = rapportiContabiliEsistenti.Where(
        '            Function(r) CInt(r(FIELD_MAPPER.COD_RAPPORTO)) = CInt(codRapporto) AndAlso CDate(r(FIELD_MAPPER.VALIDITA_INIZIO)).Equals(validitaInizio)
        '        )

        '    If rapportoEsistenti.Count > 1 Then
        '        ' Trovati più rapporti contabili con stesso cod_rapporto e validita inizio
        '        Throw New Exception($"Rilevato lo stesso ruolo con intervalli di validità sovrapposti per il ruolo {ruolo.rapporto_des} ")
        '    End If

        '    Dim rapportoEsistente = rapportoEsistenti.FirstOrDefault()
        '    If IsNothing(rapportoEsistente) Then
        '        ' DA INSERIRE NUOVO
        '        rapportiContabiliEsistenti.Add(Crea_Mappa_Risorsa_Umana_Nuova(piva,
        '                                           0,
        '                                               cod_contatto,
        '                                               ruolo.cod_rapporto,
        '                                            0,
        '                                               userNameRuolo,
        '                                               ruolo.data_ultima_modifica,
        '                                               ruolo.validita.inizio,
        '                                               ruolo.validita.fine,
        '                                               ruolo.flag_cancellazione
        '                                            ))

        '    Else
        '        ' DA MODIFICARE
        '        rapportoEsistente(FIELD_MAPPER.FLAG_CANCELLAZIONE) = ruolo.flag_cancellazione
        '        rapportoEsistente(FIELD_MAPPER.VALIDITA_INIZIO) = ruolo.validita.inizio
        '        rapportoEsistente(FIELD_MAPPER.VALIDITA_FINE) = ruolo.validita.fine
        '        If ruolo.flag_cancellazione Then
        '            If ruolo.validita.fine > DateTime.Now Then
        '                ruolo.validita.fine = DateTime.Now
        '            End If
        '        End If

        '        rapportoEsistente(FIELD_MAPPER.USERNAME_MODIFICA) = userNameRuolo
        '        rapportoEsistente(FIELD_MAPPER.DATA_MODIFICA) = DateTime.Now

        '    End If

        'Next
#End Region

        ' RAPPORTI CONTABILI
        contattoGias("rapporti_contabili") = Mappa_Rapporti_Contabili(contattoDemetra.ruolo, CUAA, piva, cod_contatto)

        ' DOCUMENTI
        contattoGias("documenti") = Mappa_Documenti(contattoDemetra.documenti, piva, cod_contatto, pivaSuperUser)

        Return contattoGias


    End Function

    Private Function Mappa_Rapporti_Contabili(ruoliDemetra As List(Of RapportoContabile),
                                              ByVal CUAA As String,
                                              ByVal piva As String,
                                              ByVal cod_contatto As String) As List(Of IDictionary(Of String, Object))

        Dim decoUtenti As New AgronicaCoreDemetraBIZ.decodificaUtenti()
        Dim rapportiContabili As New List(Of IDictionary(Of String, Object))
        Dim objSequenze As New Agro_Sequenze

        For Each ruolo In ruoliDemetra

            Dim interscambioResult As D2G_Interscambio_Ruolo_Result = _intercambio.DammiTipoOperazione_Ruolo(piva, cod_contatto, ruolo)
            Dim userNameRuolo = decoUtenti.decoficaUtenteGiasDaUtenteDemetra(ruolo.utente_ultima_modifica, CUAA, _objParametri_Utente)

            Select Case interscambioResult.TipoOperazioneDB
                Case enum_TipoOperazioneDB.Scrittura

                    'Stacco qui subito il nuovo codRisum e scrivo in interscambio
                    Dim newCodRisum = objSequenze.NuovoId_Tabella("Risorse_Umane", BaseCode, TopCode, _objParametri_Server)
                    ' Scrittura tabella interscambio
                    _intercambio.Scrivi_Tabella_Interscambio_Risorse_Umane(piva, newCodRisum, ruolo.codice)

                    Dim newRisUm = Crea_Mappa_Risorsa_Umana_Nuova(piva,
                        0,
                        cod_contatto,
                        ruolo.codice,
                        ruolo.cod_rapporto,
                        newCodRisum,
                        userNameRuolo,
                        ruolo.data_ultima_modifica,
                        ruolo.validita.inizio,
                        ruolo.validita.fine,
                        ruolo.flag_cancellazione)

                    rapportiContabili.Add(newRisUm)

                Case enum_TipoOperazioneDB.Modifica

                    Dim risUm = Crea_Mappa_Risorsa_UMana_Esistente(interscambioResult.Ruolo,
                        piva,
                        0,
                        cod_contatto,
                        ruolo.codice,
                        ruolo.cod_rapporto,
                        userNameRuolo,
                        ruolo.validita.inizio,
                        ruolo.validita.fine,
                        ruolo.flag_cancellazione)

                    rapportiContabili.Add(risUm)

            End Select
        Next

        Return rapportiContabili

    End Function

    Private Function Mappa_Documenti(ByVal documentiDemetra As List(Of Documento),
                                     ByVal piva As String,
                                     ByVal cod_contatto As String,
                                     ByVal pivaSuperUSer As String
                                     ) As List(Of IDictionary(Of String, Object))

        Dim documenti As New List(Of IDictionary(Of String, Object))
        For Each doc In documentiDemetra

            Dim interscambioResult As D2G_Interscambio_Documento_Result = _intercambio.DammiTipoOperazione_Documento(doc)
            Select Case interscambioResult.TipoOperazioneDB
                Case enum_TipoOperazioneDB.Scrittura
                    documenti.Add(Crea_Mappa_Documento_Nuovo(piva,
                                               doc.Codice,
                                               cod_contatto,
                                               pivaSuperUSer,
                                               doc.Numero,
                                               doc.Descrizione,
                                               doc.Data_Rilascio,
                                               doc.Data_Scadenza,
                                               enum_TipoOperazioneDB.Scrittura
                                            ))
                Case enum_TipoOperazioneDB.Modifica

                    Dim nomeFile As String = If(IsDBNull(interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOMEFILE)), "", interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOMEFILE))
                    Dim note As String = If(IsDBNull(interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOTE)), "", interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_NOTE))
                    Dim ente As String = If(IsDBNull(interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_ENTE_DES)), "", interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_ENTE_DES))

                    documenti.Add(Crea_Mappa_Documento_Esistente(piva,
                                               doc.codice,
                                               cod_contatto,
                                               pivaSuperUSer,
                                               doc.Numero,
                                               doc.Descrizione,
                                               doc.Data_Rilascio,
                                               doc.Data_Scadenza,
                                               CInt(interscambioResult.Documento(FIELD_MAPPER.ID_ELENCO)),
                                               CInt(interscambioResult.Documento(FIELD_MAPPER.ID_ALERT_ENTITA)),
                                               nomeFile,
                                               CInt(interscambioResult.Documento(FIELD_MAPPER.ALLEGATI_DOCUMENTI_COD)),
                                               CInt(interscambioResult.Documento(FIELD_MAPPER.VALIDAZIONE_FLAG)),
                                               note,
                                               CDate(interscambioResult.Documento(FIELD_MAPPER.DATA_UPLOAD)),
                                               ente,
                                               enum_TipoOperazioneDB.Modifica
                                            ))
            End Select

        Next

        Return documenti

    End Function

    Public Function Mappa_Nuovo(inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto),
                          ByVal piva As String,
                          ByVal pivaSuperUSer As String,
                          ByVal rapportiContabiliGias As List(Of IDictionary(Of String, Object)),
                          ByVal CUAA As String,
                          ByVal interscambioResult As D2G_Interscambio_Contatti_Result) As IDictionary(Of String, Object)


        Dim contattoDemetra = inData.elemento_anagrafico
        Dim decoUtenti As New AgronicaCoreDemetraBIZ.decodificaUtenti()
        Dim userName As String = decoUtenti.decoficaUtenteGiasDaUtenteDemetra("", CUAA, _objParametri_Utente)

        ' -----------------------------------------------------------------------
        ' CONTATTO
        ' -----------------------------------------------------------------------
        Dim mappaDefault = Crea_Mappa_Contatto_Default()
        mappaDefault(FIELD_MAPPER.USERNAME_CREAZIONE) = userName
        mappaDefault(FIELD_MAPPER.USERNAME_MODIFICA) = userName

        Dim cod_contatto As String = String.Empty
        Dim rag_soc As String = String.Empty
        Dim nome As String = String.Empty
        Dim cognome As String = String.Empty
        Dim sa_cod As Integer = 0                                              ' privato
        Dim id_cf As Integer = CInt(contattoDemetra.Id_CF)                     ' persona fisica / giuridica / estero se 2 estero la persona è giuridica
        Dim isEstero As Boolean = (id_cf = CONTATTO_ESTERO)
        Dim codice_fiscale As String = String.Empty
        Dim partitaIva As String = String.Empty
        Dim fittizio As Boolean = False

        If isEstero Then
            id_cf = PERSONA_GIURIDICA
        End If

        Select Case id_cf

            Case PERSONA_FISICA
                codice_fiscale = contattoDemetra.Cod_Fisc.Trim()
                If Not String.IsNullOrEmpty(interscambioResult.COD_CONTATTO) Then
                    cod_contatto = interscambioResult.COD_CONTATTO
                Else
                    If String.IsNullOrEmpty(codice_fiscale) Then
                        cod_contatto = contattoDemetra.codice_contatto.Trim
                    Else
                        cod_contatto = codice_fiscale
                    End If
                End If

                If cod_contatto.Length <> 16 Then
                    ' demetra ha massato nel campo codice_contatto un dato che non è un CF 
                    ' MARCO COME FITTIZIO
                    fittizio = True
                End If

                nome = contattoDemetra.nome
                cognome = contattoDemetra.cognome

            Case PERSONA_GIURIDICA

                partitaIva = contattoDemetra.partita_iva.Trim()
                If Not String.IsNullOrEmpty(interscambioResult.COD_CONTATTO) Then
                    cod_contatto = interscambioResult.COD_CONTATTO
                Else
                    If String.IsNullOrEmpty(partitaIva) Then
                        cod_contatto = contattoDemetra.codice_contatto.Trim
                    Else
                        cod_contatto = partitaIva
                    End If
                End If

                If Not isEstero Then

                    ' persona giuridica non estera per cui demetra ha passato nel campo codice_contatto un dato che non è un PI
                    If cod_contatto.Length <> 11 OrElse Not IsNumeric(cod_contatto) Then
                        fittizio = True
                    End If

                Else

                    ' persona giuridica estera
                    If cod_contatto.Length > 25 Then
                        fittizio = True
                    End If

                End If

                rag_soc = contattoDemetra.ragione_Sociale
                codice_fiscale = partitaIva

        End Select

        If piva = cod_contatto Then
            Throw New Exception("La Piva non può essere la stessa dell'azienda che crea il contatto")
        End If

        If String.IsNullOrEmpty(cod_contatto) Then
            cod_contatto = Genera_Codice_Fittizio(piva)
            fittizio = True
        End If

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(cod_contatto) Then
            cod_contatto = Genera_Codice_Fittizio(piva)
            fittizio = True
        End If

        mappaDefault(FIELD_MAPPER.PIVA) = piva
        mappaDefault(FIELD_MAPPER.SA_COD) = sa_cod
        mappaDefault(FIELD_MAPPER.ID_CF) = id_cf
        mappaDefault(FIELD_MAPPER.NOME) = nome
        mappaDefault(FIELD_MAPPER.COGNOME) = cognome
        mappaDefault(FIELD_MAPPER.RAG_SOC) = rag_soc
        mappaDefault(FIELD_MAPPER.CODICE_FISCALE) = codice_fiscale
        mappaDefault(FIELD_MAPPER.COD_CONTATTO) = cod_contatto
        mappaDefault("chkfittizio") = If(fittizio, 1, 0)

        ' -----------------------------------------------------------------------
        ' CONTATTI CODICI
        ' -----------------------------------------------------------------------
        Dim contatti_Codici As New List(Of IDictionary(Of String, Object))
        Dim codici As New List(Of KeyValuePair(Of Integer, String)) From
            {
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.TipoContattoFattura, "-1"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.Gestione_Vettore_Default, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.ScontoContattoDefault, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.ListinoPrezziVenditaDefault, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.CodiceAccisa, ""),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.ModalitaPagamentoDefault, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.Codice_Ufficio_Doganale, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.IBANDefault, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.PEC, ""),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.SDI, ""),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.RappresentanteFiscale, "0"),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo, ""),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione, ""),
                New KeyValuePair(Of Integer, String)(enum_CodiciAnagrafe.ReferenteConferimento, "0")
            }

        For Each kvp In codici
            contatti_Codici.Add(Crea_Mappa_Contatto_Codice(piva, sa_cod, cod_contatto, kvp.Key, kvp.Value, userName))
        Next
        mappaDefault("contatti_codici") = contatti_Codici

        ' -----------------------------------------------------------------------
        ' RISORSE UMANE
        ' -----------------------------------------------------------------------

#Region "OLD MAPPING"

        'Dim risorse_umane As New List(Of IDictionary(Of String, Object))
        'For Each ruolo In contattoDemetra.ruolo

        '    Dim userNameRuolo = decoUtenti.decoficaUtenteGiasDaUtenteDemetra(ruolo.utente_ultima_modifica, CUAA, _objParametri_Utente)

        '    risorse_umane.Add(Crea_Mappa_Risorsa_Umana(piva,
        '                                               sa_cod,
        '                                               cod_contatto,
        '                                               ruolo.cod_rapporto,
        '                                              0,
        '                                               userNameRuolo,
        '                                               ruolo.data_ultima_modifica,
        '                                               ruolo.validita.inizio,
        '                                               ruolo.validita.fine,
        '                                               ruolo.flag_cancellazione
        '                                            ))
        'Next

#End Region

        mappaDefault("rapporti_contabili") = Mappa_Rapporti_Contabili(contattoDemetra.ruolo, CUAA, piva, cod_contatto)

        ' -----------------------------------------------------------------------
        ' INDIRIZZI
        ' -----------------------------------------------------------------------
        ' todo persona F/G ita / estero

        Dim indirizzi As New List(Of IDictionary(Of String, Object))
        Dim codIndirizziDefault As List(Of Integer) = Nothing

        Select Case id_cf
            Case PERSONA_FISICA
                'Residenza, LuogoDiNascita, Domicilio, ResidenzaEstiva
                codIndirizziDefault = New List(Of Integer) From
                {
                    3, 5, 2, 4
                }

            Case PERSONA_GIURIDICA
                'SedeOperativa, SedeLegale, SedeAziendale, Stabilimento
                codIndirizziDefault = New List(Of Integer) From
                {
                    1, 101, 102, 103
                }
                If isEstero Then
                    'StabileOrganizzazione
                    codIndirizziDefault.Add(201)
                End If
        End Select

        For Each ind In codIndirizziDefault
            indirizzi.Add(Crea_Mappa_Indirizzo(piva, sa_cod, cod_contatto, ind, isEstero, userName))
        Next
        mappaDefault("indirizzi") = indirizzi

        mappaDefault("documenti") = Mappa_Documenti(contattoDemetra.documenti, piva, cod_contatto, pivaSuperUSer)

        Return mappaDefault

    End Function

    Private Sub Controlli_Preliminari_Rapporti_Contabili(ByVal contattoDemetra As AgronicaCoreDTOStd.InData.Demetra.Contatto,
                                                         ByVal rapportiContabiliGias As List(Of IDictionary(Of String, Object)))

        ' Controlli preventivi sui rapporti contabili
        If IsNothing(contattoDemetra.ruolo) OrElse contattoDemetra.ruolo.Count = 0 Then
            Throw New Exception("Non è stato fornito nessun ruolo per il contatto")
        End If

        Dim codRapportoTester As Integer = 0
        For Each r In contattoDemetra.ruolo
            If String.IsNullOrEmpty(r.cod_rapporto) Then
                Throw New Exception("Il campo Cod_Rapporto non è stato valorizzato per 1 o più ruoli")
            Else
                If Not Int32.TryParse(r.cod_rapporto, codRapportoTester) Then
                    Throw New Exception("Il campo Cod_Rapporto è stato fornito con valori non numerici per 1 o più ruoli")
                End If
            End If

            Dim rappGias = rapportiContabiliGias.FirstOrDefault(Function(rg) rg(FIELD_MAPPER.COD_RAPPORTO) = codRapportoTester)
            If IsNothing(rappGias) Then
                Throw New Exception(String.Format("Il Cod_Rapporto con valore {0} per il ruolo {1} non esite in Gias", r.cod_rapporto, r.rapporto_des))
            End If
        Next

    End Sub

    Private Function Genera_Codice_Fittizio(ByVal piva As String) As String

        _objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim codiceFittizio As String = String.Empty
        Dim ok = False
        Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze

        While ok = False
            codiceFittizio = objAgroSe.NuovoId_Tabella("impresa", 0, 0, _objParametri_Server).ToString.Replace("-", "F")
            'controllo se è già usato 
            ok = Not _loader.Esiste_Contatto(piva, codiceFittizio)
        End While
        _objParametri_Server.ResettaFinestra()

        Return codiceFittizio

    End Function

End Class


Public Class D2G_ImportContatti_Validatore

    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _loader As D2G_Contatto_Loader = Nothing

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByVal loader As D2G_Contatto_Loader)

        _objParametri_Server = objParametri_Server
        _loader = loader

    End Sub

    Public Sub Controlli_Preliminari_Generali(inData As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto))

        If IsNothing(inData.elemento_anagrafico) Then
            Throw New Exception("L'elemento Anagrafico 'Contatto' non è valorizzato")
        End If

        If String.IsNullOrEmpty(inData.codice_esterno) AndAlso String.IsNullOrEmpty(inData.codice) Then
            Throw New Exception("Entrambi i campi Codice e Codice_Esterno non sono stati valorizzato")
        End If

        If String.IsNullOrEmpty(inData.elemento_anagrafico.Id_CF) Then
            Throw New Exception("Il campo Id_Cf non è stato valorizzato")
        End If

        If inData.elemento_anagrafico.Id_CF <> "0" AndAlso inData.elemento_anagrafico.Id_CF <> "1" AndAlso inData.elemento_anagrafico.Id_CF <> "2" Then
            Throw New Exception("Il campo Id_Cf non è stato valorizzato correttamente: " & inData.elemento_anagrafico.Id_CF)
        End If

        Dim id_cf As Integer = CInt(inData.elemento_anagrafico.Id_CF)          ' persona fisica / giuridica / estero se 2 estero la persona è giuridica
        Dim isEstero As Boolean = (id_cf = CONTATTO_ESTERO)

        If isEstero Then
            id_cf = PERSONA_GIURIDICA
        End If

        Select Case id_cf
            Case PERSONA_FISICA
                If String.IsNullOrEmpty(inData.elemento_anagrafico.nome) Then
                    Throw New Exception("Il campo Nome non è stato valorizzato")
                End If

                If String.IsNullOrEmpty(inData.elemento_anagrafico.cognome) Then
                    Throw New Exception("Il campo Cognome non è stato valorizzato")
                End If

            Case PERSONA_GIURIDICA
                If String.IsNullOrEmpty(inData.elemento_anagrafico.ragione_Sociale) Then
                    Throw New Exception("Il campo Ragione_Sociale non è stato valorizzato")
                End If

        End Select

    End Sub

    Public Sub Controlli_Preliminari_Rapporti_Contabili(ByVal contattoDemetra As AgronicaCoreDTOStd.InData.Demetra.Contatto,
                                                        ByVal rapportiContabiliGias As List(Of IDictionary(Of String, Object)))

        ' Controlli preventivi sui rapporti contabili
        If IsNothing(contattoDemetra.ruolo) OrElse contattoDemetra.ruolo.Count = 0 Then
            Throw New Exception("Non è stato fornito nessun ruolo per il contatto")
        End If

        Dim codRapportoTester As Integer = 0
        For Each r In contattoDemetra.ruolo
            If String.IsNullOrEmpty(r.cod_rapporto) Then
                Throw New Exception("Il campo Cod_Rapporto non è stato valorizzato per 1 o più ruoli")
            Else
                If Not Int32.TryParse(r.cod_rapporto, codRapportoTester) Then
                    Throw New Exception("Il campo Cod_Rapporto è stato fornito con valori non numerici per 1 o più ruoli")
                End If
            End If

            Dim rappGias = rapportiContabiliGias.FirstOrDefault(Function(rg) rg(FIELD_MAPPER.COD_RAPPORTO) = codRapportoTester)
            If IsNothing(rappGias) Then
                Throw New Exception(String.Format("Il Cod_Rapporto con valore {0} per il ruolo {1} non esite in Gias", r.cod_rapporto, r.rapporto_des))
            End If
        Next

    End Sub

    Public Sub Controlli_Preliminari_Documenti(ByVal contattoDemetra As AgronicaCoreDTOStd.InData.Demetra.Contatto)

        If IsNothing(contattoDemetra.documenti) OrElse contattoDemetra.documenti.Count = 0 Then
            Return
        End If

        For Each doc In contattoDemetra.documenti

            If String.IsNullOrEmpty(doc.Numero) OrElse String.IsNullOrWhiteSpace(doc.Numero) Then
                Throw New Exception("Il campo Numero non è stato valorizzato per uno o più documenti")
            End If

            If IsNothing(doc.Data_Rilascio) OrElse doc.Data_Rilascio.Equals(DateTime.MinValue) Then
                Throw New Exception("Il campo Data_Rilascio non è stato valorizzato per uno o più documenti")
            End If

            If IsNothing(doc.Data_Scadenza) OrElse doc.Data_Scadenza.Equals(DateTime.MinValue) Then
                Throw New Exception("Il campo Data_Scadenza non è stato valorizzato per uno o più documenti")
            End If

            If doc.Data_Scadenza < doc.Data_Rilascio Then
                Throw New Exception("La Data_Scadenza deve essere successiva alla Data_Rilascio")
            End If
        Next

    End Sub

    Public Sub Controllo_Periodi_Intersecati(ByVal contatto As IDictionary(Of String, Object), ByVal rapportiContabiliAmmessi As List(Of IDictionary(Of String, Object)))


        Dim piva As String = contatto(FIELD_MAPPER.PIVA)
        Dim cod_Contatto As String = contatto(FIELD_MAPPER.COD_CONTATTO)

        Dim rapportiGias As List(Of IDictionary(Of String, Object)) = _loader.Leggi_Risorse_Umane_Contatto(piva, cod_Contatto)
        If Not IsNothing(rapportiGias) Then

            If rapportiGias.Count = 0 Then
                Return
            End If

            Dim tipiRapporto As List(Of Integer) = rapportiGias.Select(Function(rc) CInt(rc(FIELD_MAPPER.COD_RAPPORTO))).Distinct().ToList()
            For Each tr In tipiRapporto

                Dim rappAmmesso = rapportiContabiliAmmessi.FirstOrDefault(Function(rg) rg(FIELD_MAPPER.COD_RAPPORTO) = tr)
                If Not IsNothing(rappAmmesso) Then

                    Dim tuttiIRapporti As List(Of IDictionary(Of String, Object)) = rapportiGias.Where(Function(r) CInt(r(FIELD_MAPPER.COD_RAPPORTO)).Equals(tr)).ToList
                    If Not IsNothing(tuttiIRapporti) AndAlso tuttiIRapporti.Count > 0 Then

                        For Each rap In tuttiIRapporti

                            Dim peridodoRiferiemnto = New Validita() With
                                                                       {
                                                                           .inizio = CDate(rap(FIELD_MAPPER.VALIDITA_INIZIO)),
                                                                           .fine = CDate(rap(FIELD_MAPPER.VALIDITA_FINE))
                                                                       }

                            Dim rapportiDaConfrontare As List(Of IDictionary(Of String, Object)) = tuttiIRapporti.Except(
                                    New List(Of IDictionary(Of String, Object)) From {rap}).ToList()

                            For Each rapConf In rapportiDaConfrontare

                                Dim peridodoConfronto = New Validita() With
                                {
                                    .inizio = CDate(rapConf(FIELD_MAPPER.VALIDITA_INIZIO)),
                                    .fine = CDate(rapConf(FIELD_MAPPER.VALIDITA_FINE))
                                }

                                If peridodoRiferiemnto.IntersectsWith(peridodoConfronto) Then
                                    Throw New Exception($"Contatto {contatto(FIELD_MAPPER.COD_CONTATTO)} con Ruoli differenti aventi validità sovrapposta ( {peridodoRiferiemnto.ToString} , {peridodoConfronto.ToString} )")
                                End If

                            Next

                        Next

                    End If

                End If

            Next

        End If

    End Sub

#Region "OLD CODE INCROCI"
    'Public Sub Controllo_Periodi_Intersecati(ByVal contatto As IDictionary(Of String, Object))

    '    Dim rapportiContabili As List(Of IDictionary(Of String, Object)) = contatto("rapporti_contabili")

    '    If IsNothing(rapportiContabili) OrElse rapportiContabili.Count = 0 Then
    '        Return
    '    End If

    '    Dim tipiRapporto As List(Of Integer) = rapportiContabili.Select(Function(rc) CInt(rc(FIELD_MAPPER.COD_RAPPORTO))).Distinct().ToList()

    '    For Each tr In tipiRapporto

    '        Dim rapportiDaNonCancellare As List(Of IDictionary(Of String, Object)) =
    '            rapportiContabili.Where(Function(r) CInt(r(FIELD_MAPPER.COD_RAPPORTO)).Equals(tr) AndAlso CBool(r(FIELD_MAPPER.FLAG_CANCELLAZIONE)) = False).ToList
    '        Dim rapportiDaCancellareMaNonCancellabili As List(Of IDictionary(Of String, Object)) =
    '            rapportiContabili.Where(Function(r) CInt(r(FIELD_MAPPER.COD_RAPPORTO)).Equals(tr) AndAlso CBool(r(FIELD_MAPPER.FLAG_CANCELLAZIONE)) = True AndAlso CBool(r(FIELD_MAPPER.FLAG_CANCELLABILE)) = False).ToList
    '        Dim tuttiIRapporti As New List(Of IDictionary(Of String, Object))
    '        tuttiIRapporti.AddRange(rapportiDaNonCancellare)
    '        tuttiIRapporti.AddRange(rapportiDaCancellareMaNonCancellabili)

    '        If Not IsNothing(tuttiIRapporti) AndAlso tuttiIRapporti.Count > 0 Then

    '            For Each rap In tuttiIRapporti

    '                Dim peridodoRiferiemnto = New Validita() With
    '                {
    '                    .inizio = CDate(rap(FIELD_MAPPER.VALIDITA_INIZIO)),
    '                    .fine = CDate(rap(FIELD_MAPPER.VALIDITA_FINE))
    '                }

    '                Dim rapportiDaConfrontare As List(Of IDictionary(Of String, Object)) = tuttiIRapporti.Except(
    '                    New List(Of IDictionary(Of String, Object)) From {rap}).ToList()

    '                For Each rapConf In rapportiDaConfrontare

    '                    Dim peridodoConfronto = New Validita() With
    '                    {
    '                        .inizio = CDate(rapConf(FIELD_MAPPER.VALIDITA_INIZIO)),
    '                        .fine = CDate(rapConf(FIELD_MAPPER.VALIDITA_FINE))
    '                    }

    '                    If peridodoRiferiemnto.IntersectsWith(peridodoConfronto) Then
    '                        Throw New Exception($"Rilevato lo stesso ruolo con intervalli di validità sovrapposti ( {peridodoRiferiemnto.ToString} , {peridodoConfronto.ToString} )")
    '                    End If

    '                Next

    '            Next

    '        End If


    '    Next
    'End Sub

#End Region

End Class

