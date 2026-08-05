Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabHLP.Contabilita
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreGestioneRichieste
Imports AgroAgenda_2010.Resources

Public Class DocContabile
    Inherits System.Web.UI.Page

    Public Permessi As AgronicaCoreUtentiDAL.PermessiUtente

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    '----- Gestione Querystring
    Private _qsPiva As String
    Private _qsSaCod As Integer
    Private _qsDataSelezionata As String
    Private _qsIdAgenda As Integer
    Private _qsOperazione As Integer
    Private _qsPagRitorno As String
    Private _qsLavCod As Integer
    Private _qsRagSoc As String
    Private _qsCaricoScarico As String
    Private _qsModalitaDocContabile As Integer
    Private _qsPuaRegolamentoCod As Integer
    Private _qsModalitaProtettaDoc As Integer

    Private _qsRicercaType As String
    Private _qsRicercaDoc As String
    Private _qsServizioCod As Integer



    Private Sub InizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            '----- Verifico che l'utente sia autenticato

            If Session("ASG_Utente_Username") = "" Then
                GestistiSessioneScaduta()
            End If

            Permessi = New AgronicaCoreUtentiDAL.PermessiUtente()
            InizializzoObjParametri()


            'Master.Master_versione = "Agronica" 'Vecchio stile
            'Master.Master_versione = "2022" 'Nuovo stile Xonne

            Dim sUrl As String = Request.Url.AbsoluteUri

            '##############################################################
            '###################  QUERY STRING  ###########################
            '##############################################################
            If Not IsNothing(Request.QueryString("o")) Then
                _qsOperazione = CInt(Stringa_Decodifica(Request.QueryString("o").ToString, AgroKey_EncoderDecoder))

                'Mi imposto il link da usare post cancellazione del documento
                '(mantengo tutti i valori in query string arrivati, impostando tipo_op = creazione e id_agenda = 0)

                sUrl = sUrl.Replace("o=" & Request.QueryString("o").ToString, "o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder))

                'hf_UrlNuovoDocumento.Value = sUrl & "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) & "&nuovoDoc=1"

                If Not IsNothing(Request.QueryString("i")) Then
                    sUrl = sUrl.Replace("i=" & Request.QueryString("i").ToString, "i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder))
                Else
                    sUrl = sUrl & "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder)
                End If

                hf_UrlPostDelete.Value = sUrl
                hf_UrlNuovoDocumento.Value = sUrl & "&nuovoDoc=1"
            Else
                _qsOperazione = 0
                hf_UrlPostDelete.Value = ""
                hf_UrlNuovoDocumento.Value = ""
            End If


            Dim _aperturadaIFrame As Integer = 0
            If Not IsNothing(Request.QueryString("Ifr")) Then
                _aperturadaIFrame = CInt(Stringa_Decodifica(Request.QueryString("Ifr").ToString, AgroKey_EncoderDecoder))
            End If

            Select Case _aperturadaIFrame
                Case 1
                    'Se apro la pagina da un IFrame nascondo l'header e il footer della pagina
                    Master.flag_MostraHeader = False
                    Master.flag_MostraFooter = False

                    Dim _apertodaGiasNG As String = "false"
                    If Not IsNothing(Request.QueryString("apertodaGiasNG")) Then
                        _apertodaGiasNG = Stringa_Decodifica(Request.QueryString("apertodaGiasNG").ToString, AgroKey_EncoderDecoder)
                    End If

                    If _apertodaGiasNG = "true" Then
                        hf_Qs_PagRitorno.Value = "apertodaGiasNG"
                    Else
                        hf_Qs_PagRitorno.Value = "aperturadaIframe"
                    End If


                Case 2
                    Master.flag_MostraHeader = False
                    Master.flag_MostraFooter = False

                    hf_Qs_PagRitorno.Value = "aperturadaFinestra"

                Case Else
                    Master.flag_MostraHeader = True
                    Master.flag_MostraFooter = True

                    'pagina chiamante --> per gestire il tipo di uscita dalla pagina:
                    'fare un redirect o chiuderla (perché aperta in modal dialog)
                    'usa enum_PagineGiasOnline
                    If Not IsNothing(Request.QueryString("orig")) Then
                        _qsPagRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString, AgroKey_EncoderDecoder)
                    Else
                        _qsPagRitorno = CStr(0)
                    End If
                    hf_Qs_PagRitorno.Value = _qsPagRitorno

            End Select

            'Piva
            If Not IsNothing(Request.QueryString("p")) Then
                _qsPiva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)
            Else
                _qsPiva = CStr(0)
            End If

            'Sa_Cod
            If Not IsNothing(Request.QueryString("s")) <> 0 Then
                _qsSaCod = CInt(Stringa_Decodifica(Request.QueryString("s").ToString, AgroKey_EncoderDecoder))
            Else
                _qsSaCod = 0
            End If
            hf_Qs_SaCod.Value = _qsSaCod

            'Id_Agenda
            If Not IsNothing(Request.QueryString("i")) Then
                _qsIdAgenda = CInt(Stringa_Decodifica(Request.QueryString("i").ToString, AgroKey_EncoderDecoder))

                If Not IsNothing(Request.QueryString("nuovoDoc")) Then
                    If Request.QueryString("nuovoDoc") = 1 Then
                        _qsIdAgenda = 0
                        'HttpContext.Current.Session.Item("ASG_MenuBS_2017") = Nothing
                    End If
                End If
            Else
                _qsIdAgenda = 0
            End If

            'Lav_Cod
            If Not IsNothing(Request.QueryString("l")) Then
                _qsLavCod = CInt(Stringa_Decodifica(Request.QueryString("l").ToString, AgroKey_EncoderDecoder))
            Else
                _qsLavCod = 0
            End If

            'Data Selezionata
            If InStr(Request.QueryString.ToString, "&d=") <> 0 Then
                _qsDataSelezionata = Stringa_Decodifica(Request.QueryString("d").ToString, AgroKey_EncoderDecoder)
            Else
                _qsDataSelezionata = CStr(Date.Today)
            End If

            'Rag_Soc
            If Not IsNothing(Request.QueryString("rs")) Then
                _qsRagSoc = Stringa_Decodifica(Request.QueryString("rs").ToString, AgroKey_EncoderDecoder)
            Else
                _qsRagSoc = ""
            End If
            hf_Qs_RagSocContatto.Value = _qsRagSoc

            'PUA Regolamento
            If Not IsNothing(Request.QueryString("pr")) <> 0 Then
                _qsPuaRegolamentoCod = CInt(Stringa_Decodifica(Request.QueryString("pr").ToString, AgroKey_EncoderDecoder))
            Else
                _qsPuaRegolamentoCod = 0
            End If
            hf_qsPuaRegolamento.Value = _qsPuaRegolamentoCod

            'TODO DA GUARDARE SCATTO E GIULIA - NELLA VECCHIA C'ERANO
            'If InStr(Request.QueryString.ToString, "&codcont=") <> 0 Then
            '    Qs_CodContatto = Stringa_Decodifica(Request.QueryString("codcont").ToString, AgroKey_EncoderDecoder)
            'Else
            '    Qs_CodContatto = ""
            'End If
            'hf_Qs_CodContatto.Value = Qs_CodContatto

            'If VerificaEsistenza_PivaGIAS(objParametri_Server, Qs_CodContatto) Then
            '    hf_contattoAziendaGias.Value = 1
            'Else
            '    hf_contattoAziendaGias.Value = 0
            'End If

            'Modalità Doc Contabile
            If Not IsNothing(Request.QueryString("md")) Then
                _qsModalitaDocContabile = CInt(Stringa_Decodifica(Request.QueryString("md").ToString, AgroKey_EncoderDecoder))
            Else
                _qsModalitaDocContabile = enum_ModalitaDocContabile.Nessuno
            End If
            hf_Qs_ModalitaDoc.Value = _qsModalitaDocContabile

            'Modalità Protetta Documento
            If Not IsNothing(Request.QueryString("mp")) Then
                _qsModalitaProtettaDoc = CInt(Stringa_Decodifica(Request.QueryString("mp").ToString, AgroKey_EncoderDecoder))
            Else
                _qsModalitaProtettaDoc = enum_ModalitaProtetta.Nessuna
            End If
            hf_ModalitaProtettaDoc.Value = _qsModalitaProtettaDoc

            hf_ModalitaProtettaRiga.Value = enum_ModalitaProtetta.Nessuna

            hf_OperatoreCodFisc.Value = objParametri_Server.UtenteCodFiscale
            hf_OperatoreNominativo.Value = UtilityHelper.GetNomeUtenteFromCF(objParametri_Server.UtenteCodFiscale, objParametri_Utenti)

            'Servono a mantenere il corretto collegamento con la pagina di ricerca chiamante
            If Not IsNothing(Request.QueryString("ricercatype")) Then
                _qsRicercaType = Request.QueryString("ricercatype").ToString
            Else
                _qsRicercaType = ""
            End If
            hf_RicercaType.Value = _qsRicercaType

            If Not IsNothing(Request.QueryString("ricercadoc")) Then
                _qsRicercaDoc = Request.QueryString("ricercadoc").ToString
            Else
                _qsRicercaDoc = ""
            End If
            hf_RicercaDoc.Value = _qsRicercaDoc

            'Per pulsante Salva E nuovo Documento
            'Recupero dal configDocContabile il DocContabile_Nuovo corrispondente al tipo DocContabile recivuto in QueryString
            'per salvare la barra del titolo prima di fare il redirect
            hf_IdSessionNuovoDoc.Value = getIdSessioneNuovoDocContab(_qsRicercaType, _qsRicercaDoc, _qsLavCod)

            Dim impDict As New Dictionary(Of String, Object)
            hdOpzioniContab.Value = CaricaImpostazioniUtente(_qsPiva, impDict)

            'Dim dictImposImpresa As Dictionary(Of String, String) = CaricaImpostazioniImpresa(_qsPiva)
            'hdOpzioniContabImpresa.Value = JsonConvert.SerializeObject(dictImposImpresa, Formatting.None)

            'In questo caso è l'utente che tramite una impostazione stabilisce quale deve essere il lavcod da usare
            If _qsOperazione = enum_TipoOperazioneDB.Scrittura AndAlso _qsLavCod = 0 Then

                Select Case _qsModalitaDocContabile
                    Case enum_ModalitaDocContabile.Ricevimento_Prodotto
                        _qsLavCod = LAVCOD_BOLLA_RICEVUTA
                    Case enum_ModalitaDocContabile.Ricevimento_Ortofrutta
                        _qsLavCod = impDict(enum_Impostazioni_Utenti.SUPERUSER_COD_TIPO_DOC_ACCETTAZIONE_DEFAULT.ToString)
                    Case Else
                        Throw New Exception("Deve essere passato in query string il Lav_Cod oppure Modalita_Doc_Contabile")
                End Select

                hdLavCod.Value = _qsLavCod
            End If

            'Servizio_Cod
            If Not IsNothing(Request.QueryString("sc")) Then
                _qsServizioCod = CInt(Stringa_Decodifica(Request.QueryString("sc").ToString, AgroKey_EncoderDecoder))
            Else
                _qsServizioCod = enum_Servizi.Quaderno_Campagna_Caa
            End If
            hf_Qs_ServizioCod.Value = _qsServizioCod

            '##############################################################
            '#####################  PERMESSI  #############################
            '##############################################################

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            Dim utenteAbilitatoLettura As Boolean = False
            Dim utenteAbilitatoScrittura As Boolean = False
            Dim utenteAbilitatoGestionePrezziLettura As Boolean = False
            Dim utenteAbilitatoGestionePrezziScrittura As Boolean = False
            Dim utenteAbilitatoGestioneNuovoAllegato As Boolean = False
            Dim utenteAbilitatoGestioneVisualizaAllegato As Boolean = False
            Dim utenteAbilitatoGestioneGHG As Boolean = False
            Dim utenteAbilitatoISCCNonConforme As Boolean = False
            Dim utenteAbilitatoProdottiScrittura As Boolean = False
            Dim utenteAbilitatoContattiScrittura As Boolean = False

            ControllaPermessi(_qsPiva,
                              _qsLavCod,
                              _qsOperazione,
                              utenteAbilitatoLettura,
                              utenteAbilitatoScrittura,
                              utenteAbilitatoGestionePrezziLettura,
                              utenteAbilitatoGestionePrezziScrittura,
                              utenteAbilitatoGestioneNuovoAllegato,
                              utenteAbilitatoGestioneVisualizaAllegato,
                              utenteAbilitatoGestioneGHG,
                              utenteAbilitatoISCCNonConforme,
                              utenteAbilitatoProdottiScrittura,
                              utenteAbilitatoContattiScrittura)

            'Imposto le variabili di ponte con il client
            hf_UtenteAbilitatoLettura.Value = utenteAbilitatoLettura
            hf_UtenteAbilitatoScrittura.Value = utenteAbilitatoScrittura
            hf_UtenteAbilitatoGestionePrezziLettura.Value = utenteAbilitatoGestionePrezziLettura
            hf_UtenteAbilitatoGestionePrezziScrittura.Value = utenteAbilitatoGestionePrezziScrittura
            hf_UtenteAbilitatoGestioneNuovoAllegato.Value = utenteAbilitatoGestioneNuovoAllegato
            hf_UtenteAbilitatoGestioneVisualizaAllegato.Value = utenteAbilitatoGestioneVisualizaAllegato
            hf_UtenteAbilitatoGestioneGHG.Value = utenteAbilitatoGestioneGHG
            hf_utenteAbilitatoISCCNonConforme.Value = utenteAbilitatoISCCNonConforme
            hf_utenteAbilitatoProdottiScrittura.Value = utenteAbilitatoProdottiScrittura
            hf_utenteAbilitatoContattiScrittura.Value = utenteAbilitatoContattiScrittura

            'Applico controllo permessi
            Dim accessoNonConsentito As Boolean = False
            accessoNonConsentito = ApplicaControlloPermessi(utenteAbilitatoLettura, utenteAbilitatoScrittura)
            If accessoNonConsentito Then
                Exit Sub
            End If

            hdPiva.Value = _qsPiva
            hdLavCod.Value = _qsLavCod
            hdIdAgenda.Value = _qsIdAgenda
            hdOperazione.Value = _qsOperazione

            Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("DocumentoContabile"), String)

            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################

            If Not Page.IsPostBack Then

                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

                'Controllo se F&F / Cantine / Tabacco / Zoo
                hf_Modulo_Anagrafe_Log.Value = ""
                Dim elencoModuli = ""
                Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
                Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(hdPiva.Value, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                           "", "",
                                                           objParametri_Server)
                If dtAnagrafeLog IsNot Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
                    For Each rAnagrafeLog In dtAnagrafeLog.Rows
                        If elencoModuli <> "" Then
                            elencoModuli += "|"
                        End If
                        elencoModuli += CStr(rAnagrafeLog.Item("Modulo_Generazione"))
                    Next
                End If
                hf_Modulo_Anagrafe_Log.Value = elencoModuli
                dtAnagrafeLog = Nothing

                ' Lettura del documento sul quale si è entrato in modifica
                Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
                Dim statoOrdineDoc As New Contabilita_Evasione_Ordine

                If Not String.IsNullOrEmpty(hdIdAgenda.Value) AndAlso CInt(hdIdAgenda.Value) <> 0 Then
                    Dim msgError As String = ""
                    Dim serializerSettings As New JsonSerializerSettings With {
                        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        .DateTimeZoneHandling = DateTimeZoneHandling.Local
                    }
                    Dim objContabHelper As New ContabilitaHelper_Testata(objParametri_Server, objParametri_Utenti)

                    'Leggo la testata
                    Dim objTestata As Contabilita_Testata = objContabHelper.LeggiTestataDocumento(hdPiva.Value, 0, hdIdAgenda.Value, hdLavCod.Value, msgError)
                    hdKendo_TestataDoc.Value = JsonConvert.SerializeObject(objTestata, Formatting.None, serializerSettings)
                    If objTestata IsNot Nothing Then
                        hdDataDocumento.Value = JsonConvert.SerializeObject(objTestata.DataMovimento)
                    Else
                        hdDataDocumento.Value = ""
                    End If

                    hf_OperatoreModificaCodFisc.Value = objTestata.UsernameModifica
                    hf_OperatoreModificaNominativo.Value = UtilityHelper.GetNomeUtenteFromCF(objTestata.UsernameModifica, objParametri_Utenti)

                    hdTipoAccettazione.Value = objTestata.TipoAccettazione

                    'Controllo permessi in base allo stato documento
                    ControlloPermessiStatoWorkflow(objTestata,
                                                     utenteAbilitatoLettura,
                                                     utenteAbilitatoScrittura,
                                                     accessoNonConsentito)
                    If accessoNonConsentito Then
                        Exit Sub
                    End If

                    'Controllo agenda bloccata
                    If objTestata.BloccoFlag = 1 AndAlso _qsOperazione = enum_TipoOperazioneDB.Modifica Then
                        Dim bloccoData As Date = objTestata.BloccoData
                        Dim mesAgendaBloccata = String.Format(DirectCast(GetLocalResourceObject("DocBloccatoInData"), String), bloccoData.ToShortDateString())

                        Page.ClientScript.RegisterStartupScript(Me.GetType(),
                                                                "AgendaBloccata",
                                                                "MessaggioErrore_Bootstrap('" & mesAgendaBloccata & "', 'DIV_Messaggi');",
                                                                True)
                        _qsOperazione = enum_TipoOperazioneDB.Lettura
                    End If
                    Master.Lbl_Titolo.Text = If(_qsOperazione = enum_TipoOperazioneDB.Lettura, AgronicaAgenda_2010.Lettura, AgronicaAgenda_2010.Modifica) & " " & objTestata.DescrizioneAgenda

                    'Verifico se devo estrarre anche il gruppo merce dai dettagli
                    Dim GestioneGruppiMerce As Boolean = False
                    Dim GestioneGruppiUtenteMerce As Boolean = False

                    Dim handleGruppiUtentiMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametri_Server, objParametri_Utenti)
                    Dim dtGruppiUtenteMerce As New DataTable

                    If Not String.IsNullOrEmpty(
                        impDict([Enum].GetName(GetType(enum_Impostazioni_Utenti), enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto))
                            ) Then

                        GestioneGruppiMerce = True

                        'Per capire se è gestita la visibilità dei gruppi merce in base ai gruppi degli utenti, controllo che sia presente almeno una riga per l'impresa corrente
                        dtGruppiUtenteMerce = handleGruppiUtentiMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" & hdPiva.Value & "')", "")
                        GestioneGruppiUtenteMerce = dtGruppiUtenteMerce.Rows.Count > 0

                    End If

                    'Leggo le righe
                    Dim dtRighe As New DataTable
                    Dim SaCodDoc As Integer = 0
                    hdKendo_RigheDoc.Value = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(hdPiva.Value,
                                                                                            hdIdAgenda.Value, "", 0, hdLavCod.Value,
                                                                                            False, dtRighe, objParametri_Server, objParametri_Utenti,
                                                                                            "ordine_det <> 1000", "",
                                                                                            SaCodDoc, statoOrdineDoc:=statoOrdineDoc.Stato_Cod, gruppiMerce:=GestioneGruppiMerce, contestoDocContabile:=True)

                    hf_Qs_SaCod.Value = SaCodDoc

                    Dim mesAccessoNonConsentito As String = ""

                    If GestioneGruppiUtenteMerce AndAlso objParametri_Server.UtenteUsername <> objParametri_Server.SuperUserUsername Then

                        Dim handleGruppiUtente As New Utenti_xGruppi_Utente_R()

                        Dim gruppiUtente = handleGruppiUtente.LeggiJoinGruppi(objParametri_Utenti.UtenteUsername, 0, "", "", objParametri_Utenti)
                        Dim arrGruppiUtente = gruppiUtente.AsEnumerable().Select(Of Integer)(Function(gruppo) gruppo("Gruppi_Utente_Cod"))

                        'Leggo la Gruppi_UtenteXGruppi_Merce mettendo come clausola IN i gruppi utenti letti.
                        'Poi controllo che tutti i gruppi in dtRighe siano presenti nei gruppi merce trovati

                        'Filtro ulteriormente la tabella per i gruppi utenti a cui l'utente collegato appartiene
                        Dim drGruppiUtentiMerce = dtGruppiUtenteMerce.Select(String.Format("Gruppo_Utente IN({0})", String.Join(",", arrGruppiUtente)))

                        'Dim gruppiUtentiMerce = handleGruppiUtentiMerce.Leggi(String.Format("Gruppi_UtenteXGruppi_Merce.Piva = '{0}' AND Gruppi_UtenteXGruppi_Merce.Gruppo_Utente IN({1})",
                        '        hdPiva.Value, String.Join(",", arrGruppiUtente)), "")

                        Dim arrGruppiMerce = drGruppiUtentiMerce.AsEnumerable().Select(Of Integer)(Function(gruppo) gruppo("Id_Gruppo_Merce"))

                        For Each dettaglio As DataRow In dtRighe.Rows
                            If arrGruppiMerce.Contains(dettaglio("Id_Gruppo_Merce")) = False Then
                                accessoNonConsentito = True
                                mesAccessoNonConsentito = DirectCast(GetLocalResourceObject("DocNonVisualizzabileGruppiMerce"), String)
                                Exit For
                            End If
                        Next

                    End If

                    If {LAVCOD_BOLLA_EMESSA, LAVCOD_SCARICO}.Contains(hdLavCod.Value) Then

                        Dim dtRifDDTEmesso As DataTable = New Mov_Dettagli_Riferimenti_R().Recupera_DT_Rif_Unificato(hdPiva.Value, 0, hdIdAgenda.Value, 0, 0, 0, "", objParametri_Server)

                        If dtRifDDTEmesso.Rows.Count > 0 Then

                            Dim rifAccettazioneDaRaccolta = dtRifDDTEmesso.AsEnumerable().Where(
                                Function(detRif)
                                    Return UtilityHelper.IsAccettazione(detRif.Item("Lav_Cod_Risultato")) AndAlso detRif.Item("Tipo_Associazione") = 2
                                End Function)

                            If rifAccettazioneDaRaccolta.Count > 0 Then

                                If Regex.IsMatch(elencoModuli, "(^|\D)" & enum_Omni_Modulo_Generazione.FreshFood & "(\D|$)", RegexOptions.None, TimeSpan.FromSeconds(3)) Then
                                    'Nel caso in cui l'azienda corrente abbia il modulo F&F impedisco di visualizzare il documento in quanto
                                    'il prodotto viene scaricato dal magazzino nel quale era stato caricato dalla operazione di raccolta, ma questa pagina carica le celle,
                                    'non i magazzini, di conseguenza non trovando la destinazione viene mostrato errore.
                                    'Le operazioni di raccolta caricano per forza su magazzini.
                                    accessoNonConsentito = True
                                    mesAccessoNonConsentito = String.Format(
                                        DirectCast(GetLocalResourceObject("BollaEmessaDaAccettazioneNonVisualizzabile"), String),
                                        rifAccettazioneDaRaccolta.First().Item("Des_Lib_Risultato"),
                                        CDate(rifAccettazioneDaRaccolta.First().Item("Validita_Inizio_Risultato")).ToShortDateString)
                                Else
                                    Dim letturaDDTEmessoDaRaccolta As Boolean = True
                                    Dim modificaDDTEmessoDaRaccolta As Boolean = False

                                    hf_UtenteAbilitatoLettura.Value = letturaDDTEmessoDaRaccolta
                                    hf_UtenteAbilitatoScrittura.Value = modificaDDTEmessoDaRaccolta
                                    accessoNonConsentito = ApplicaControlloPermessi(letturaDDTEmessoDaRaccolta, modificaDDTEmessoDaRaccolta)
                                End If
                            End If

                        End If

                    End If

                    'ControlloPermessiGruppiMerce(dtRighe, accessoNonConsentito)
                    If accessoNonConsentito Then
                        'Passo al client un messaggio specifico da mostrare all'utente per bloccare il caricamento della pagina
                        hf_MesAccessoNonConsentito.Value = mesAccessoNonConsentito
                    End If

                Else

                    'Imposto il tipo accettazione per nuovo conferimento (pomodoro)
                    hdTipoAccettazione.Value = If(_qsRicercaType = DocContab_TipoRicerca_Conferimenti, If(_qsRicercaDoc = DocContab_TipoDoc_Pomodoro, "-1", "2"), "0")

                    Select Case _qsLavCod
                        Case LAVCOD_CARICO
                            Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("NuovoCaricoMagazzino"), String)
                        Case LAVCOD_SCARICO
                            Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("NuovoScaricoMagazzino"), String)
                        Case Else
                            Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("NuovoDocumentoContabile"), String)
                    End Select

                    'Leggo comunque le righe in modo da avere la griglia creata
                    hdKendo_RigheDoc.Value = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(hdPiva.Value,
                                                                                            -999999999, "", 0, hdLavCod.Value,
                                                                                            False, Nothing, objParametri_Server, objParametri_Utenti,
                                                                                            "ordine_det <> 1000", "", contestoDocContabile:=True)
                End If

                statoOrdineDoc.Stato_Des = GetStatoOrdine_Des(statoOrdineDoc.Stato_Cod)
                hdStatoOrdine.Value = JsonConvert.SerializeObject(statoOrdineDoc)

                'TODO Completare con quelli mancanti
                Select Case _qsLavCod
                    Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                        LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI,
                        LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_ORDINE_ACQUISTO,
                        LAVCOD_ACQUISTO, LAVCOD_CARICO, LAVCOD_CONTRATTO_AFFITTO
                        _qsCaricoScarico = "C"
                    Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ORDINE_VENDITA,
                        LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_VENDITA, LAVCOD_SCARICO
                        _qsCaricoScarico = "S"
                End Select

                'assegno il valore alla variabile xCaricoScarico di tipo integer
                hdCau_Mov_BC_Principale.Value = ""
                If _qsCaricoScarico = "C" Then
                    hf_Qs_CaricoScarico.Value = enum_Agenda_Causali.CARICO
                    hdCau_Mov_BC_Principale.Value = CAU_CARICO

                    'If Qs_Mode.ToLower = "magazzino" Then
                    '    Master().Lbl_Titolo.Text = "Carico di Magazzino"
                    'Else
                    '    If Qs_Mode.ToLower = "bolla" Then
                    '        Master().Lbl_Titolo.Text = "Inserimento dettaglio nel DDT Ricevuto"
                    '    Else
                    '        If Qs_Mode.ToLower = "fattura" Then
                    '            Select Case _qsLavCod
                    '                Case LAVCOD_FATTURA_RICEVUTA
                    '                    Master().Lbl_Titolo.Text = "Inserimento dettaglio nella Fattura Ricevuta"
                    '                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    '                    Master().Lbl_Titolo.Text = "Inserimento dettaglio nella Nota di Accredito Emessa"
                    '            End Select
                    '        Else
                    '            If Qs_Mode.ToLower = "compravendita" Then
                    '                Master().Lbl_Titolo.Text = "Acquisto beni"
                    '                'Else
                    '                '   If Qs_Mode.ToLower = "conferimento" Then
                    '                '       Master().Lbl_Titolo.Text = "Carico di Magazzino"
                    '                '   Else
                    '                '    If Qs_Mode.ToLower = "stalla" Then
                    '                '        Master().Lbl_Titolo.Text = "Carico di Magazzino"
                    '                '    End If
                    '                'End If
                    '            End If
                    '        End If
                    '    End If
                    'End If

                ElseIf _qsCaricoScarico = "S" Then
                    hf_Qs_CaricoScarico.Value = enum_Agenda_Causali.SCARICO
                    hdCau_Mov_BC_Principale.Value = CAU_SCARICO

                    'If Qs_Mode.ToLower = "magazzino" Then
                    '    Master().Lbl_Titolo.Text = "Scarico di Magazzino"
                    'Else
                    '    If Qs_Mode.ToLower = "bolla" Then
                    '        Master().Lbl_Titolo.Text = "Inserimento dettaglio nel DDT Emesso"
                    '    Else
                    '        If Qs_Mode.ToLower = "fattura" Then
                    '            Select Case _qsLavCod
                    '                Case LAVCOD_FATTURA_EMESSA
                    '                    Master().Lbl_Titolo.Text = "Inserimento dettaglio nella Fattura Emessa"
                    '                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                    '                    Master().Lbl_Titolo.Text = "Inserimento dettaglio nella Nota di Accredito Ricevuta"
                    '            End Select
                    '        Else
                    '            If Qs_Mode.ToLower = "compravendita" Then
                    '                Master().Lbl_Titolo.Text = "Vendita beni"
                    '                'Else
                    '                'If Qs_Mode.ToLower = "conferimento" Then
                    '                '   Master().Lbl_Titolo.Text = "Carico di Magazzino"
                    '                'Else
                    '                '    If Qs_Mode.ToLower = "stalla" Then
                    '                '        Master().Lbl_Titolo.Text = "Carico di Magazzino"
                    '                '    End If
                    '                'End If
                    '            End If
                    '        End If
                    '    End If
                    'End If

                ElseIf _qsCaricoScarico = "T" Then
                    hf_Qs_CaricoScarico.Value = enum_Agenda_Causali.TRASFERIMENTO
                    'Master().Lbl_Titolo.Text = "Trasferimento Merci"
                End If

                Dim objIndirizzo As New ImpresexIndirizzi_R
                hdStato_Cod.Value = objIndirizzo.Stato_From_Piva(hdPiva.Value, objParametri_Server)
            End If
            '-------------------------------------------------------------------------------------

            Page.Title = Master.Lbl_Titolo.Text

            Master.flag_MostraBtnEsci = True
            Master.flag_pag_DocContabile = True
            Master.flag_MostraBtnIndietro = True
            AddHandler Master.ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

            If _qsOperazione = enum_TipoOperazioneDB.Lettura Then
                hf_UtenteAbilitatoScrittura.Value = False
            End If

            hdTipoOp.Value = _qsOperazione

        Catch ex As Exception

            Dim msgError As String = ex.Message & " [" & If(ex.InnerException Is Nothing, "", ex.InnerException.Message) & "]"
            Page.ClientScript.RegisterStartupScript(Me.GetType(),
                                                    "Exception",
                                                    "MessaggioErrore_Bootstrap('" & HttpUtility.JavaScriptStringEncode(msgError) & "', 'DIV_Messaggi');",
                                                    True)
            Dim objLog As New LogProvider
            objLog.Scrivi_LOG(objParametri_Server, "DocContabile.aspx.vb.PageLoad()", msgError)

            'Disattiva_Pulsanti()

        End Try

    End Sub

    Private Function getIdSessioneNuovoDocContab(qsRicercaType As String, qsRicercaDoc As String, lavCod As Integer) As Integer

        Dim IdSessioneNuovoDoc As Integer = 0

        Dim configDocContabile As New Dictionary(Of Integer, DocContabile_Menu) From {
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Ordine_Acquisto, New DocContabile_Menu(LAVCOD_ORDINE_ACQUISTO, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Ordine)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_DDT_Ricevuto, New DocContabile_Menu(LAVCOD_BOLLA_RICEVUTA, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Consegna)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_Distinta_Carico, New DocContabile_Menu(LAVCOD_DISTINTA_CARICO, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Consegna)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_AutoDDT_Emesso, New DocContabile_Menu(LAVCOD_AUTO_DDT_EMESSO, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Consegna)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_fattura_Ricevuta, New DocContabile_Menu(LAVCOD_FATTURA_RICEVUTA, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Fattura)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_NotaCredito_Ricevuta, New DocContabile_Menu(LAVCOD_NOTA_ACCREDITO_RICEVUTA, DocContab_TipoRicerca_Acquisti, DocContab_TipoDoc_Fattura)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Ordine_Vendita, New DocContabile_Menu(LAVCOD_ORDINE_VENDITA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Ordine)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_DDT_Emesso, New DocContabile_Menu(LAVCOD_BOLLA_EMESSA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Consegna)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_Fattura_Emessa, New DocContabile_Menu(LAVCOD_FATTURA_EMESSA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Fattura)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuova_NotaCredito_Emessa, New DocContabile_Menu(LAVCOD_NOTA_ACCREDITO_EMESSA, DocContab_TipoRicerca_Vendite, DocContab_TipoDoc_Fattura)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento, New DocContabile_Menu(0, DocContab_TipoRicerca_Conferimenti, DocContab_TipoDoc_Consegna)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Conferimento_Pomodoro, New DocContabile_Menu(0, DocContab_TipoRicerca_Conferimenti, DocContab_TipoDoc_Pomodoro)},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Carico_Magazzino, New DocContabile_Menu(LAVCOD_CARICO, "", "")},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Scarico_Magazzino, New DocContabile_Menu(LAVCOD_SCARICO, "", "")},
                   {enum_Sezioni_MenuBS_2017.DocContabile_Nuovo_Contratto_Affitto, New DocContabile_Menu(LAVCOD_CONTRATTO_AFFITTO, DocContab_TipoRicerca_Contratti, DocContab_TipoDoc_ContrattoAffitto)}
               }

        Dim selectedItems As KeyValuePair(Of Integer, DocContabile_Menu)

        If Not String.IsNullOrWhiteSpace(qsRicercaType) AndAlso Not String.IsNullOrWhiteSpace(qsRicercaDoc) Then
            'Recupero dal configDocContabile il DocContabile_Nuovo corrispondente al tipo DocContabile recivuto in QueryString
            selectedItems = configDocContabile _
                .Where(Function(pair) pair.Value.RicercaType = qsRicercaType AndAlso pair.Value.RicercaDoc = qsRicercaDoc) _
                .Select(Function(pair) pair).FirstOrDefault()
        Else
            selectedItems = configDocContabile _
                .Where(Function(pair) pair.Value.LavCod = lavCod) _
                .Select(Function(pair) pair).FirstOrDefault()
        End If


        If Not String.IsNullOrEmpty(selectedItems.Key) AndAlso CInt(selectedItems.Key) <> 0 Then
            IdSessioneNuovoDoc = selectedItems.Key
        End If

        Return IdSessioneNuovoDoc

    End Function

    Private Class DocContabile_Menu
        Public Property LavCod As Integer
        Public Property RicercaType As String
        Public Property RicercaDoc As String

        Public Sub New(lavCod As Integer, ricercaType As String, ricercaDoc As String)
            Me.LavCod = lavCod
            Me.RicercaType = ricercaType
            Me.RicercaDoc = ricercaDoc
        End Sub
    End Class


    Private Sub ControllaPermessi(ByVal Piva As String,
                                  ByVal lavCod As Integer,
                                  ByVal tipoOperazione As enum_TipoOperazioneDB,
                                  ByRef utenteAbilitatoLettura As Boolean,
                                  ByRef utenteAbilitatoScrittura As Boolean,
                                  ByRef utenteAbilitatoGestionePrezziLettura As Boolean,
                                  ByRef utenteAbilitatoGestionePrezziScrittura As Boolean,
                                  ByRef utenteAbilitatoGestioneNuovoAllegato As Boolean,
                                  ByRef utenteAbilitatoGestioneVisualizaAllegato As Boolean,
                                  ByRef utenteAbilitatoGestioneGHG As Boolean,
                                  ByRef utenteAbilitatoISCCNonConforme As Boolean,
                                  ByRef utenteAbilitatoProdottiScrittura As Boolean,
                                  ByRef utenteAbilitatoContattiScrittura As Boolean)

        Dim attivita As enum_Security_Attivita = enum_Security_Attivita.Gest_Contabilita
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Select Case lavCod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                    attivita = enum_Security_Attivita.Nuovo_DDT_Vendita
                Else
                    attivita = enum_Security_Attivita.Consegne_Vendita
                End If

            Case LAVCOD_BOLLA_RICEVUTA
                attivita = enum_Security_Attivita.Consegne_Acquisto

            Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
                 LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_DISTINTA_CARICO

                If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                    attivita = enum_Security_Attivita.Nuovo_Conferimento
                Else
                    attivita = enum_Security_Attivita.Consegne_Conferimento
                End If

            Case LAVCOD_ORDINE_VENDITA
                attivita = enum_Security_Attivita.Ordini_Vendita

            Case LAVCOD_ORDINE_ACQUISTO
                attivita = enum_Security_Attivita.Ordini_Acquisto

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
                'TODO: vanno aggiunti altri lav_cod?!?
                attivita = enum_Security_Attivita.Fatture_Vendita

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                'TODO: vanno aggiunti altri lav_cod?!?
                attivita = enum_Security_Attivita.Fatture_Acquisto

            Case LAVCOD_CARICO, LAVCOD_SCARICO
                Dim objUtilOp As New Utility_Operazioni
                Dim setupDoc2021 = objUtilOp.SeSetupDoc2021(objParametri_Server)
                If setupDoc2021 Then
                    attivita = enum_Security_Attivita.Contabilita_CarichiScarichi_Magazzino
                Else
                    attivita = enum_Security_Attivita.Nessuna
                End If

            Case LAVCOD_CONTRATTO_AFFITTO
                attivita = enum_Security_Attivita.Contratti_Affitto

        End Select

        utenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                       Session("ASG_IdServizio"),
                                                                       attivita,
                                                                       enum_Security_Operazione.Lettura,
                                                                       Now, "", objParametri_Utenti)

        utenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                         Session("ASG_IdServizio"),
                                                                         attivita,
                                                                         enum_Security_Operazione.Modifica,
                                                                         Now, "",
                                                                         objParametri_Utenti)

        utenteAbilitatoGestionePrezziLettura = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                     Session("ASG_IdServizio"),
                                                                                     enum_Security_Attivita.Gestione_Prezzi,
                                                                                     enum_Security_Operazione.Lettura,
                                                                                     Now, "",
                                                                                     objParametri_Utenti)

        utenteAbilitatoGestionePrezziScrittura = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                       Session("ASG_IdServizio"),
                                                                                       enum_Security_Attivita.Gestione_Prezzi,
                                                                                       enum_Security_Operazione.Modifica,
                                                                                       Now, "",
                                                                                       objParametri_Utenti)

        utenteAbilitatoGestioneNuovoAllegato = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                   Session("ASG_IdServizio"),
                                                                                   enum_Security_Attivita.Documentale_Inser,
                                                                                   enum_Security_Operazione.Modifica,
                                                                                   Now, "",
                                                                                   objParametri_Utenti)

        utenteAbilitatoGestioneVisualizaAllegato = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                       Session("ASG_IdServizio"),
                                                                                       enum_Security_Attivita.Documentale_Lista,
                                                                                       enum_Security_Operazione.Lettura,
                                                                                       Now, "",
                                                                                       objParametri_Utenti)


        utenteAbilitatoISCCNonConforme = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                       Session("ASG_IdServizio"),
                                                                                       enum_Security_Attivita.InserisciDocNonConformeISCC,
                                                                                       enum_Security_Operazione.Modifica,
                                                                                       Now, "",
                                                                                       objParametri_Utenti)

        utenteAbilitatoProdottiScrittura = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                       Session("ASG_IdServizio"),
                                                                                       enum_Security_Attivita.Angrafica_Prodotti,
                                                                                       enum_Security_Operazione.Modifica,
                                                                                       Now, "",
                                                                                       objParametri_Utenti)

        utenteAbilitatoContattiScrittura = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                       Session("ASG_IdServizio"),
                                                                                       enum_Security_Attivita.Anagrafica_Contatto,
                                                                                       enum_Security_Operazione.Modifica,
                                                                                       Now, "",
                                                                                       objParametri_Utenti)



        Dim objLetturaImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        Dim listaCentriAziendali As New List(Of Integer)
        listaCentriAziendali.Add(0)

        'Controllo Abilitazione GHG
        utenteAbilitatoGestioneGHG =
            CBool(objLetturaImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(Piva,
                                                                                        listaCentriAziendali,
                                                                                        enum_Impostazioni_Utenti.SUPERUSER_Gestione_GHG,
                                                                                        0,
                                                                                        objParametri_Utenti,
                                                                                        objParametri_Server))

    End Sub

    Private Sub ControlloPermessiStatoWorkflow(ByVal objTestata As Contabilita_Testata,
                                                 ByVal utenteAbilitatoLettura As Boolean,
                                                 ByVal utenteAbilitatoScrittura As Boolean,
                                                 ByRef accessoNonConsentito As Boolean)

        Dim permessiModificati As Boolean = False

        Try

            Dim objUtilityHelper As New UtilityHelper

            permessiModificati = objUtilityHelper.SeControllaPermessiStatoWorkflowAperturaDocumento(_qsLavCod,
                                                                                                    objTestata,
                                                                                                    objParametri_Server,
                                                                                                    objParametri_Utenti,
                                                                                                    utenteAbilitatoLettura,
                                                                                                    utenteAbilitatoScrittura)

        Catch ex As Exception

            permessiModificati = False

            CambioPaginaSolaLettura(ex.Message)

        End Try

        If permessiModificati Then

            hf_UtenteAbilitatoLettura.Value = utenteAbilitatoLettura

            hf_UtenteAbilitatoScrittura.Value = utenteAbilitatoScrittura

            'NB: la funzione sottostante, in caso di utenteAbilitatoLettura=False genera una eccezione di tipo
            '    "Thread interrotto" : per questo motivo questo blocco è stato messo fuori dal Try/Catch.

            accessoNonConsentito = ApplicaControlloPermessi(utenteAbilitatoLettura, utenteAbilitatoScrittura)

        End If

    End Sub

    Private Function ApplicaControlloPermessi(ByVal utenteAbilitatoLettura As Boolean,
                                              ByVal utenteAbilitatoScrittura As Boolean) As Boolean

        Dim accessoNonConsentito As Boolean = False

        '--- Utente senza permesso di visualizzazione

        If utenteAbilitatoLettura = False Then

            accessoNonConsentito = PaginaAccessoNonConsentito()
            Return accessoNonConsentito

        End If

        '--- Utente senza permesso di modifica

        If utenteAbilitatoScrittura = False Then

            Select Case _qsOperazione

                Case enum_TipoOperazioneDB.Modifica

                    Dim messaggioErrore = DirectCast(GetLocalResourceObject("ModalitaSolaLettura"), String)

                    CambioPaginaSolaLettura(messaggioErrore)

                Case enum_TipoOperazioneDB.Scrittura

                    accessoNonConsentito = PaginaAccessoNonConsentito()
                    Return accessoNonConsentito

            End Select

        End If

        Return accessoNonConsentito

    End Function

    Private Function PaginaAccessoNonConsentito()

        Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")

        Return True

    End Function

    Private Function CambioPaginaSolaLettura(messaggioErrore As String)

        Dim modelloScript As String = "MessaggioErrore_Bootstrap('{0}', 'DIV_Messaggi');"

        Dim script = String.Format(modelloScript, messaggioErrore)


        Page.ClientScript.RegisterStartupScript(Me.GetType(),
                                        "ChangeInReadOnly",
                                        script,
                                        True)

        _qsOperazione = enum_TipoOperazioneDB.Lettura

    End Function




    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Response.Redirect(DocContabile_WS.Calcola_Url_Indietro_DocContabile(
            _qsPagRitorno, hdPiva.Value, _qsRicercaType, _qsRicercaDoc,
            Master.flag_MenuBS_2017, objParametri_Server, objParametriAgenda))

    End Sub
    
    Private Sub GestistiSessioneScaduta()
        Response.Redirect("~/Custom500.aspx")
    End Sub

    Private Function CaricaImpostazioniUtente(ByVal piva As String, ByRef impDict As Dictionary(Of String, Object)) As String

        'leggo e Jsonizzo le impostazioni
        Dim obj As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
        impDict = obj.LeggiOpzioni_DocContabili(piva, objParametri_Server, objParametri_Utenti)

        Return JsonConvert.SerializeObject(impDict, Formatting.None)

    End Function

    Public Shadows ReadOnly Property Master As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class