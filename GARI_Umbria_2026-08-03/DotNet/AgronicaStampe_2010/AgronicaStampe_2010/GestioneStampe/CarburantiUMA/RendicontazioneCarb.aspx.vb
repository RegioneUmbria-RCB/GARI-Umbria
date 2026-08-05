Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreScadenziario
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal

Public Class RendicontazioneCarb
    Inherits System.Web.UI.Page

    Private rptRendicontazioneCarb As Rpt_RendicontazioneCarb
    Private rptGruppiColturali As Rpt_GruppiColturali
    Private rptLavorazioniProprio As Rpt_LavorazioniProprio
    Private rptLavorazioniTerzi As Rpt_LavorazioniTerzi
    Private rptRichiesteAllev As Rpt_RichiesteAllevamenti
    Private rptMacchineDenunciate As Rpt_MacchineDenunciate
    Private rptAllegatiRichiesta As Rpt_AllegatiRichiesta

    Private Log_Errori As String

    Private Qs_Piva As String
    Private Qs_CodRichiestaTestata As Integer

    Private Param_Dichiarazione As String = ""
    Private Param_Rag_Soc As String = ""


    Private objParametri_Server As New AgronicaCoreParametri
    Private objParametri_Utenti As New AgronicaCoreParametri

    Private nomeDocIdentifPratica As String = ""

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        rptRendicontazioneCarb = New Rpt_RendicontazioneCarb
        rptGruppiColturali = New Rpt_GruppiColturali
        rptLavorazioniProprio = New Rpt_LavorazioniProprio
        rptLavorazioniTerzi = New Rpt_LavorazioniTerzi
        rptRichiesteAllev = New Rpt_RichiesteAllevamenti
        rptMacchineDenunciate = New Rpt_MacchineDenunciate
        rptAllegatiRichiesta = New Rpt_AllegatiRichiesta

    End Sub

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_CodRichiestaTestata = Stringa_Decodifica(CStr(Request.QueryString("crt")),
                                   AgroKey_EncoderDecoder,
                                   Server)


        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "RendicontazioneCarbLavorazioni"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_RichiestaCarb()

            Catch ex As Exception
                Log_Errori &= "- Lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try


            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                'NOTA Come miglioramento, creare una enum_CategorieDocumenti specifica. Non è obbligatorio in quanto il file generato lo elimino
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

                Nome_Documento &= " " & nomeDocIdentifPratica
                Dim rnd As New Random
                Dim Nome_Documento_Estensione = Nome_Documento & "_" & rnd.Next() & ".pdf"

                'Salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptRendicontazioneCarb,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)

                rptRendicontazioneCarb.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " & CStr(Qs_Piva) & vbCrLf & vbCrLf & Log_Errori

                Nome_File_Log = "Log_Errori_" & Nome_Documento & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "CarburantiUMA",
                                                 Nome_File_Log,
                                                 Session("ASG_Utente_Username"),
                                                 Nome_Documento,
                                                 Log_Errori)

            End If


            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                            "?anteprima=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                            "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))



        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_RichiestaCarb()

        'DataSet Reports
        Dim dsRendicontazioneCarb As New DS_RendicontazioneCarb
        Dim dsGruppiColt As New DS_GruppiColturali
        Dim dsLavorazioniProprio As New DS_LavorazioniProprio
        Dim dsLavorazioniTerzi As New DS_LavorazioniTerzi
        Dim dsRichiesteAllev As New DS_RichiesteAllevamenti
        Dim dsAllegatiRichiesta As New DS_AllegatiRichiesta
        Dim dsMacchine As New DS_MacchineDenunciate

        'Oggetti BIZ e DAL
        Dim handleImprese As New Imprese_Read()
        Dim handleImpreseCodici As New Imprese_Codici_Read() 'Oppure aggiornare ed usare Imprese_Read/Leggi3
        Dim handleCompilatore As New Utenti_xGruppi_Utente_R()
        Dim handleRichiesteBiz As New AgronicaCoreUmaBiz.UMA_Richieste()
        Dim handleRichiesteTestate As New UMA_Richieste_Testata_R()
        Dim handleUmaSetup As New UMASetup_R()
        Dim handleRichieste As New UMA_Richieste_R()
        Dim handleRichiesteLavorazioni As New UMA_Richieste_Lavorazioni_R()
        Dim handleParcoMacchine As New Parco_Macchine_R()
        Dim handleAlertEntita As New Alert_Entita_R()
        Dim handleRichiesteAllevamenti As New UMA_Richieste_Allevamenti_R()
        Dim helperReport As New HelperStampeCarburantiUMA(objParametri_Server, objParametri_Utenti)

        'DataTable delle query
        Dim dtImpresa As New DataTable
        Dim dtCompilatore As New DataTable
        Dim dtUmaSetup As New DataTable
        Dim dtRichiesteGruppi As New DataTable
        Dim dtRichiesteLavorazioni As New DataTable
        Dim dtRichiesteAllevamenti As New DataTable
        Dim dtAllegatiRichiesta As New DataTable
        Dim arrDrMacchineRichiesta As DataRow() = New DataRow() {}
        Dim impresaUfficioRea As String = ""
        Dim impresaNumeroRea As String = ""

        'Dati richiesta corrente e riepiloghi anni precedenti
        Dim drRendicontazioneCorrente As DataRow = Nothing
        Dim annoPratica As Integer
        Dim numeroPratica As Integer
        Dim aziendaNuovaIscrizione As Boolean = True
        Dim flagRichiestaProprio As Boolean = False
        Dim flagRichiestaTerzi As Boolean = False
        Dim drRendicontazioneAnnoPrecProprio As DataRow = Nothing
        Dim drRendicontazioneAnnoPrecTerzi As DataRow = Nothing
        Dim carbPerTipoCorr As New List(Of CarburanteHelper)
        Dim carbPerTipoAnnoPrecProprio As New List(Of CarburanteHelper)
        Dim carbPerTipoAnnoPrecTerzi As New List(Of CarburanteHelper)

        'Parametri per i report
        Dim paramGruppiColtTitolo As String = ""
        Dim paramLavTitolo As String = ""

        '-----------------------------------------
        '---- Query di lettura  -------------
        '-----------------------------------------   

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessione(False, objParametri_Server)

            'Ottengo i dati dell'azienda
            dtImpresa = handleImprese.DatiIntestazioneImpresa(Qs_Piva, "", "", objParametri_Server)

            Dim filtroCodiceRea = String.Format("id_cod IN ({0}, {1})", CInt(enum_CodiciAnagrafe.UfficioRea), CInt(enum_CodiciAnagrafe.NumeroRea))
            Dim dtCodiceRea = handleImpreseCodici.Leggi(Qs_Piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtroCodiceRea, "", objParametri_Server)

            If dtCodiceRea.Rows.Count = 2 Then
                impresaUfficioRea = dtCodiceRea.Select("id_cod = " & enum_CodiciAnagrafe.UfficioRea).FirstOrDefault().Field(Of String)("val_cod")
                impresaNumeroRea = dtCodiceRea.Select("id_cod = " & enum_CodiciAnagrafe.NumeroRea).FirstOrDefault().Field(Of String)("val_cod")
            End If


            'Ottengo i dati del compilatore
            'dtCompilatore = handleCompilatore.LeggiUtenti(objParametri_Utenti.UtenteUsername, 0, "", "", objParametri_Utenti)

            'Ottengo i dati delle richieste_testata e delle pratiche dell'impresa
            Dim dtRichiestePratiche = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, False)
            Dim dtRichiestePraticheT = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, True)
            dtRichiestePratiche.Merge(dtRichiestePraticheT)

            'Ottengo i dati delle richieste
            dtRichiesteGruppi = handleRichieste.Leggi(
                Qs_Piva,
                Qs_CodRichiestaTestata,
                "",
                0,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                objParametri_Server)

            'Ottengo i dati delle lavorazioni
            'Non specifico la piva perché le lavorazioni conto terzi possono averne una differente
            dtRichiesteLavorazioni = handleRichiesteLavorazioni.Leggi(
                "",
                "",
                0,
                Qs_CodRichiestaTestata,
                0,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                objParametri_Server)

            'Elaboro i dati delle richieste/pratiche per ottenere la rendicontazione specifica
            drRendicontazioneCorrente = dtRichiestePratiche.Select(
                String.Format("Richiesta_Cod = {0}", Qs_CodRichiestaTestata)
            ).FirstOrDefault()
            annoPratica = drRendicontazioneCorrente.Item("Anno")
            numeroPratica = drRendicontazioneCorrente.Item("Numero")
            nomeDocIdentifPratica = annoPratica & "-" & numeroPratica

            'Ottengo i dati del compilatore: considero lo stesso come l'utente che ha creato la testata/pratica
            dtCompilatore = handleCompilatore.LeggiUtenti(drRendicontazioneCorrente.Field(Of String)("Username_Creazione"), 0, "", "", objParametri_Utenti)

            'Ottengo dati di configurazione come la percentuale di decurtamento da applicare alle quantità di carburante
            dtUmaSetup = handleUmaSetup.LeggiSetup(annoPratica, objParametri_Server)

            'carbPerTipoCorr = handleRichiesteBiz.LeggiTotaliCarburanteDaLavorazioni(Qs_CodRichiestaTestata, objParametri_Server)

            flagRichiestaProprio = drRendicontazioneCorrente.Item("Tipo_Richiesta") = 0
            flagRichiestaTerzi = drRendicontazioneCorrente.Item("Tipo_Richiesta") = -1

            'drRendicontazioneAnnoPrecProprio = dtRichiestePratiche.Select(
            '    "Avanzamento_Richiesta = 1 And Tipo_Richiesta = 0 And Anno = " & annoPratica - 1
            ').FirstOrDefault()
            'drRendicontazioneAnnoPrecTerzi = dtRichiestePratiche.Select(
            '    "Avanzamento_Richiesta = 1 And Tipo_Richiesta = -1 And Anno = " & annoPratica - 1
            ').FirstOrDefault()

            'If drRendicontazioneAnnoPrecProprio IsNot Nothing OrElse drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
            '    aziendaNuovaIscrizione = False
            'End If

            'Ottengo i totali del carburante inerenti alla rendicontazione in proprio anno precedente
            'If drRendicontazioneAnnoPrecProprio IsNot Nothing Then
            '    carbPerTipoAnnoPrecProprio = handleRichiesteBiz.LeggiTotaliCarburanteDaLavorazioni(
            '        drRendicontazioneAnnoPrecProprio("Richiesta_Cod"),
            '        objParametri_Server
            '    )
            'End If

            'Ottengo i totali del carburante inerenti alla rendicontazione per terzi anno precedente
            'If drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
            '    carbPerTipoAnnoPrecTerzi = handleRichiesteBiz.LeggiTotaliCarburanteDaLavorazioni(
            '        drRendicontazioneAnnoPrecTerzi("Richiesta_Cod"),
            '        objParametri_Server
            '    )
            'End If


            'Ottengo i dati degli allegati alla richiesta testata
            Dim filtroAggAlertEntita = "Alert_Entita.Richiesta_Cod = " & Qs_CodRichiestaTestata
            dtAllegatiRichiesta = handleAlertEntita.Leggi_con_documenti_2(0, filtroAggAlertEntita, "", objParametri_Server)


            'Ottengo i dati delle macchine
            Dim dtParcoMacchine = handleParcoMacchine.ParcoMacchine_Leggi(
                Qs_Piva, 0, True, "", "", "", "", "", 0, "", False, 0, "", False, AGRODATAINIZIO, AGRODATAFINE,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim macchineImpiegate As String = drRendicontazioneCorrente.Item("Macchine_Impiegate")
            Dim arrMacchineImpiegate() As String
            If macchineImpiegate.Length > 0 Then
                arrMacchineImpiegate = macchineImpiegate.Split("|")

                For i = 0 To arrMacchineImpiegate.Length - 1
                    'Ogni stringa è composta come segue, qua devo prelevare il mac_cod:
                    'piva_saCod_macCod
                    arrMacchineImpiegate(i) = arrMacchineImpiegate(i).Substring(
                        arrMacchineImpiegate(i).LastIndexOf("_") + 1
                    )
                Next

                arrDrMacchineRichiesta = dtParcoMacchine.Select("Mac_Cod IN (" & String.Join(", ", arrMacchineImpiegate) & ")")
            End If

            'Ottengo i dati delle Richieste degli Allevamenti
            dtRichiesteAllevamenti = handleRichiesteAllevamenti.Leggi(Qs_Piva, "", Qs_CodRichiestaTestata,
                    "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "query Stampa_RichiestaCarb: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        Finally
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End Try


        '------------------------------------------------------------
        '---- Impostazione dei dati trovati nei report  -------------
        '------------------------------------------------------------

        Try

            Dim drRichiestaCarb As DS_RendicontazioneCarb.DT_RendicontazioneCarbRow =
                    dsRendicontazioneCarb.DT_RendicontazioneCarb.NewRow

            drRichiestaCarb.DataConvalida = DateTime.Now
            drRichiestaCarb.Ente = "REGIONE UMBRIA"
            drRichiestaCarb.Normativa = "Ai sensi dell'Art.47 D.P.R. 28/12/2000 n.445 s.m.i."

            'drRichiestaCarb.DenComunitaMontana = "Afor - Ex C.M. Orvietano - Narnese - Amerino - Tuderte"

            'drRichiestaCarb.CompilatoreOrganiz = dtCompilatore.Rows(0)("Gruppi_Utente_des")
            'drRichiestaCarb.CompilatoreUfficio = "" 'TODO
            'drRichiestaCarb.CompilatoreDettagli = String.Format(
            '    "({0}) {1}", dtCompilatore.Rows(0)("UserName"), dtCompilatore.Rows(0)("Dettagli")
            ')

            'drRichiestaCarb.TipoRichiesta = "RICHIESTA ANNO IN CORSO [Assegnazione su base anno precedente]"

            drRichiestaCarb.AnnoRichiesta = annoPratica
            drRichiestaCarb.NumeroRichiesta = numeroPratica

            drRichiestaCarb.DichiarPiva = Qs_Piva
            drRichiestaCarb.DichiarCodFisc = Qs_Piva
            drRichiestaCarb.DichiarRagSocCogn = dtImpresa.Rows(0).Item("rag_soc")
            'drRichiestaCarb.DichiarComune = dtImpresa.Rows(0).Item("LOCALITA")
            'drRichiestaCarb.DichiarProv = dtImpresa.Rows(0).Item("COMUNI_PROV")
            'drRichiestaCarb.DichiarProvIstat = "" ' Identificare il tipo di codice, esempio Todi (PG) ha codice 054052
            'drRichiestaCarb.DichiarCap = dtImpresa.Rows(0).Item("CAP")
            'drRichiestaCarb.DichiarIndir = dtImpresa.Rows(0).Item("ind_impresa")
            drRichiestaCarb.Cuaa = dtImpresa.Rows(0).Item("codice_cuaa")

            'drRichiestaCarb.CodRea = impresaUfficioRea & impresaNumeroRea

            'If aziendaNuovaIscrizione = True Then
            '    drRichiestaCarb.DittaNuovaIscrizione = "SI"
            'Else
            '    drRichiestaCarb.DittaNuovaIscrizione = "NO"
            'End If

            'If flagRichiestaProprio = True Then
            '    drRichiestaCarb.ProprioTerzi = "PROPRIO"

            'ElseIf flagRichiestaTerzi = True Then
            '    drRichiestaCarb.ProprioTerzi = "TERZI"

            'End If

            'drRichiestaCarb.DataFineAnnoPrec = New Date(annoPratica, 12, 31)

            'Dim carbHelperDefault As New AgronicaCoreAnagrafeBIZ.CarburanteHelper() With {
            '        .Tipo = 0, .Calcolato = 0, .Richiesto = 0, .Assegnato = 0
            '}

            'If carbPerTipoCorr.Count > 0 Then
            '    Dim carbRichBenz = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Richiesto
            '    Dim carbRichGas = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Richiesto
            '    Dim carbRichSer = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Richiesto
            '    Dim carbRichTot = carbRichBenz + carbRichGas + carbRichSer

            '    If flagRichiestaProprio = True Then
            '        drRichiestaCarb.AnnoCorrRichiestoProprioBenzina = carbRichBenz
            '        drRichiestaCarb.AnnoCorrRichiestoProprioGasolio = carbRichGas
            '        drRichiestaCarb.AnnoCorrRichiestoProprioGasolioSerra = carbRichSer
            '        drRichiestaCarb.AnnoCorrRichiestoProprioTotale = carbRichTot

            '        drRichiestaCarb.AnnoCorrRichiestoTerziBenzina = 0
            '        drRichiestaCarb.AnnoCorrRichiestoTerziGasolio = 0
            '        drRichiestaCarb.AnnoCorrRichiestoTerziGasolioSerra = 0
            '        drRichiestaCarb.AnnoCorrRichiestoTerziTotale = 0

            '    ElseIf flagRichiestaTerzi = True Then
            '        drRichiestaCarb.AnnoCorrRichiestoTerziBenzina = carbRichBenz
            '        drRichiestaCarb.AnnoCorrRichiestoTerziGasolio = carbRichGas
            '        drRichiestaCarb.AnnoCorrRichiestoTerziGasolioSerra = carbRichSer
            '        drRichiestaCarb.AnnoCorrRichiestoTerziTotale = carbRichTot

            '        drRichiestaCarb.AnnoCorrRichiestoProprioBenzina = 0
            '        drRichiestaCarb.AnnoCorrRichiestoProprioGasolio = 0
            '        drRichiestaCarb.AnnoCorrRichiestoProprioGasolioSerra = 0
            '        drRichiestaCarb.AnnoCorrRichiestoProprioTotale = 0
            '    End If

            'End If

            ''TODO Il carburante Acquistato non lo abbiamo in queste tabelle, pertanto occorrerà ricavarlo in modo diverso

            'If drRendicontazioneAnnoPrecProprio IsNot Nothing Then
            '    drRichiestaCarb.AnnoPrecAssegnatoProprioBenzina = carbPerTipoAnnoPrecProprio.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
            '    drRichiestaCarb.AnnoPrecAssegnatoProprioGasolio = carbPerTipoAnnoPrecProprio.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
            '    drRichiestaCarb.AnnoPrecAssegnatoProprioGasolioSerra = carbPerTipoAnnoPrecProprio.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato

            '    drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina = drRendicontazioneAnnoPrecProprio.Item("Rimanenza_Benzina")
            '    drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio = drRendicontazioneAnnoPrecProprio.Item("Rimanenza_Gasolio")
            '    drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra = drRendicontazioneAnnoPrecProprio.Item("Rimanenza_Gasolio_Serra")
            'Else
            '    drRichiestaCarb.AnnoPrecAssegnatoProprioBenzina = 0
            '    drRichiestaCarb.AnnoPrecAssegnatoProprioGasolio = 0
            '    drRichiestaCarb.AnnoPrecAssegnatoProprioGasolioSerra = 0

            '    drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina = 0
            '    drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio = 0
            '    drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra = 0
            'End If

            'If drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
            '    drRichiestaCarb.AnnoPrecAssegnatoTerziBenzina = carbPerTipoAnnoPrecTerzi.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
            '    drRichiestaCarb.AnnoPrecAssegnatoTerziGasolio = carbPerTipoAnnoPrecTerzi.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
            '    drRichiestaCarb.AnnoPrecAssegnatoTerziGasolioSerra = carbPerTipoAnnoPrecTerzi.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato

            '    drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina = drRendicontazioneAnnoPrecTerzi.Item("Rimanenza_Benzina")
            '    drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio = drRendicontazioneAnnoPrecTerzi.Item("Rimanenza_Gasolio")
            '    drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra = drRendicontazioneAnnoPrecTerzi.Item("Rimanenza_Gasolio_Serra")
            'Else
            '    drRichiestaCarb.AnnoPrecAssegnatoTerziBenzina = 0
            '    drRichiestaCarb.AnnoPrecAssegnatoTerziGasolio = 0
            '    drRichiestaCarb.AnnoPrecAssegnatoTerziGasolioSerra = 0

            '    drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina = 0
            '    drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio = 0
            '    drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra = 0
            'End If

            '------------------Dati ultima pagina---------------------------------
            drRichiestaCarb.CompilatoreOrganiz = dtCompilatore.Rows(0)("Gruppi_Utente_des")
            drRichiestaCarb.CompilatoreUfficio = "" 'TODO
            drRichiestaCarb.CompilatoreDettagli = String.Format(
                "({0}) {1}", dtCompilatore.Rows(0)("UserName"), dtCompilatore.Rows(0)("Dettagli")
            )

            drRichiestaCarb.CompilatoreDichiarFascicolo = "X"
            drRichiestaCarb.CompilatoreDichiarDocum = "X"
            drRichiestaCarb.CompilatoreDichiarFirma = "X"
            drRichiestaCarb.CompilatoreDichiarLavorat = "X"



            '-----------------------------------------------------------------------------
            dsRendicontazioneCarb.DT_RendicontazioneCarb.Rows.Add(drRichiestaCarb)
            '-----------------------------------------------------------------------------

            'Primo SottoReport - Gruppi Colturali Conto Proprio
            rptRendicontazioneCarb.FooterSectionGruppiColt.SectionFormat.EnableSuppress = True
            'For i = 0 To dtRichiesteGruppi.Rows.Count - 1
            '    Dim drGruppiColt As DS_GruppiColturali.DT_GruppiColturaliRow = dsGruppiColt.DT_GruppiColturali.NewRow

            '    drGruppiColt.GruppoCod = CInt(dtRichiesteGruppi.Rows(i).Field(Of String)("Gruppo_Colturale_UMA"))
            '    drGruppiColt.GruppoDes = dtRichiesteGruppi.Rows(i).Field(Of String)("Macrouso_UMA_Des")
            '    drGruppiColt.SupTot = dtRichiesteGruppi.Rows(i).Field(Of Double)("Totale_Superficie_UMA_Edit")
            '    drGruppiColt.SupZonaA = dtRichiesteGruppi.Rows(i).Field(Of Double)("Zona_Pendenza_A_UMA_Edit")
            '    drGruppiColt.SupZonaB = dtRichiesteGruppi.Rows(i).Field(Of Double)("Zona_Pendenza_B_UMA_Edit")


            '    dsGruppiColt.DT_GruppiColturali.Rows.Add(drGruppiColt)
            'Next

            Dim percentualeDecurtamento = dtUmaSetup.Rows(0).Field(Of Double)("Per_Riduzione")
            Dim valMoltDecurt = (100 - percentualeDecurtamento) / 100

            If flagRichiestaProprio = True Then
                paramGruppiColtTitolo = "GRUPPI COLTURALI CONTO PROPRIO"

                'Nascondo il sottoreport delle lavorazioni per terzi
                rptRendicontazioneCarb.FooterSectionLavTerzi.SectionFormat.EnableSuppress = True

                'Secondo SottoReport - Lavorazioni Colturali Conto Proprio
                paramLavTitolo = "LAVORAZIONI D.M. 30/12/2015"
                For i = 0 To dtRichiesteLavorazioni.Rows.Count - 1
                    Dim drLavorazioniColt As DS_LavorazioniProprio.DT_LavorazioniProprioRow = dsLavorazioniProprio.DT_LavorazioniProprio.NewRow

                    Dim fascicolo =
                        (From drRichiesta In dtRichiesteGruppi.AsEnumerable()
                         Where drRichiesta.Field(Of String)("Gruppo_Colturale_UMA") =
                            dtRichiesteLavorazioni.Rows(i).Field(Of String)("Gruppo_Colturale_UMA")
                         Select New With {
                             .FascicoloCod = drRichiesta.Field(Of Integer)("Programmazione_Cod"),
                             .FascicoloDes = drRichiesta.Field(Of String)("Programmazione_Des")
                             }).First()

                    Dim lavDataInizio As Date = dtRichiesteLavorazioni.Rows(i).Field(Of DateTime)("Validita_Inizio")
                    Dim lavMesi As Integer = dtRichiesteLavorazioni.Rows(i).Item("Mesi")
                    Dim lavDataFine As Date = Nothing
                    If lavMesi > 0 Then
                        lavDataFine = lavDataInizio.AddMonths(dtRichiesteLavorazioni.Rows(i).Item("Mesi"))
                    End If


                    Dim dicituraAggiuntivaMacrouso As String = If(dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Regolamento_Cod") = 4, " (BIOLOGICO)", "")

                    drLavorazioniColt.GruppoCod = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Gruppo_Colturale_UMA")

                    drLavorazioniColt.FascicoloDes = helperReport.OttieniDescrizioneFascicolo_o_PCG(
                                                        fascicolo.FascicoloCod,
                                                        fascicolo.FascicoloDes,
                                                        dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Richiesta_Cod"),
                                                        drLavorazioniColt.GruppoCod)

                    drLavorazioniColt.GruppoDes = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Macrouso_UMA_Des") + dicituraAggiuntivaMacrouso
                    drLavorazioniColt.SupTot = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Totale_Superficie_UMA")
                    drLavorazioniColt.SupTotZonaA = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Pendenza_A_UMA")
                    drLavorazioniColt.SupTotZonaB = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Pendenza_B_UMA")
                    drLavorazioniColt.NumLavorazioniEff = dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Nr_Lavorazioni_Richieste")
                    drLavorazioniColt.LavTipoCod = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Lavorazione_UMA")
                    drLavorazioniColt.LavTipoDes = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Lav_UMA_Des")
                    drLavorazioniColt.LavDataInizio = lavDataInizio
                    drLavorazioniColt.LavDataFine = lavDataFine
                    drLavorazioniColt.LavCarburanteTipo = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Car_Des")
                    drLavorazioniColt.LavCarburanteQta = Math.Round(dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Fabbisogno_Richiesto") * valMoltDecurt, 0)
                    drLavorazioniColt.LavSupTotale = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Totale_Superficie_UMA")
                    drLavorazioniColt.LavSupZonaA = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Pendenza_A_UMA")
                    drLavorazioniColt.LavSupZonaB = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Pendenza_B_UMA")
                    drLavorazioniColt.LavSupTerrenoNormale = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Tessitura_Normale_UMA")
                    drLavorazioniColt.LavSupTerrenoMedioImpasto = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Tessitura_Media_UMA")
                    drLavorazioniColt.LavSupTerrenoTenace = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Tessitura_Tenace_UMA")
                    drLavorazioniColt.LavSupMaggiorazioneTrasferimenti = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Superficie_Maggiorazione_Trasferimenti")
                    drLavorazioniColt.LavNote = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Note_Compilatore")
                    Dim udmAlt = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Udm_Alternativa")
                    If udmAlt = "" Then
                        drLavorazioniColt.LavFlagParticolare = False
                    Else
                        drLavorazioniColt.LavFlagParticolare = True
                    End If
                    drLavorazioniColt.LavUdm = udmAlt
                    drLavorazioniColt.LavQta = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Qta_Manuale")
                    drLavorazioniColt.LavMesi = lavMesi
                    'drLavorazioniColt.LavOre = 0 'TODO

                    dsLavorazioniProprio.DT_LavorazioniProprio.Rows.Add(drLavorazioniColt)
                Next


                'Terzo Sottoreport - Allevamenti
                For i = 0 To dtRichiesteAllevamenti.Rows.Count - 1
                    Dim drRichAllev As DS_RichiesteAllevamenti.DT_RichiesteAllevamentiRow = dsRichiesteAllev.DT_RichiesteAllevamenti.NewRow

                    drRichAllev.GruppoCod = dtRichiesteAllevamenti.Rows(i).Item("UMA_AllGru_Cod")
                    drRichAllev.GruppoDes = dtRichiesteAllevamenti.Rows(i).Item("UMA_AllGru_Des")
                    drRichAllev.TipoCod = dtRichiesteAllevamenti.Rows(i).Item("UMA_All_Cod")
                    drRichAllev.TipoDes = dtRichiesteAllevamenti.Rows(i).Item("UMA_All_Des")
                    drRichAllev.TotaleCapi = dtRichiesteAllevamenti.Rows(i).Item("Totale_Capi")
                    drRichAllev.CarburanteTipo = dtRichiesteAllevamenti.Rows(i).Item("Car_Des")
                    drRichAllev.CarburanteQta = Math.Round(dtRichiesteAllevamenti.Rows(i).Item("Carburante_Richiesto") * valMoltDecurt, 0)
                    drRichAllev.Note = dtRichiesteAllevamenti.Rows(i).Field(Of String)("Note_Compilatore")

                    dsRichiesteAllev.DT_RichiesteAllevamenti.Rows.Add(drRichAllev)
                Next

            End If

            If flagRichiestaTerzi = True Then
                paramGruppiColtTitolo = "GRUPPI COLTURALI CONTO TERZI"

                'Nascondo il sottoreport delle lavorazioni in proprio
                rptRendicontazioneCarb.FooterSectionLavProprio.SectionFormat.EnableSuppress = True

                'Secondo SottoReport - Lavorazioni Colturali Conto Terzi
                paramLavTitolo = "LAVORAZIONI D.M. 30/12/2015"
                For i = 0 To dtRichiesteLavorazioni.Rows.Count - 1
                    Dim drLavorazioniColt As DS_LavorazioniTerzi.DT_LavorazioniTerziRow = dsLavorazioniTerzi.DT_LavorazioniTerzi.NewRow

                    Dim dtImpresaBeneficiaria = handleImprese.DatiIntestazioneImpresa(dtRichiesteLavorazioni.Rows(i).Item("Piva"), "", "", objParametri_Server)
                    drLavorazioniColt.BeneficiarioCuaa = dtImpresaBeneficiaria.Rows(0).Item("codice_cuaa")
                    drLavorazioniColt.BeneficiarioNominativo = dtImpresaBeneficiaria.Rows(0).Item("rag_soc")

                    Dim fascicolo =
                        (From drRichiesta In dtRichiesteGruppi.AsEnumerable()
                         Where drRichiesta.Field(Of String)("Gruppo_Colturale_UMA") = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Gruppo_Colturale_UMA") And
                             drRichiesta.Field(Of String)("Piva") = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Piva")
                         Select New With {
                             .FascicoloCod = drRichiesta.Field(Of Integer)("Programmazione_Cod"),
                             .FascicoloDes = drRichiesta.Field(Of String)("Programmazione_Des")
                             }).First()

                    Dim lavDataInizio As Date = dtRichiesteLavorazioni.Rows(i).Field(Of DateTime)("Validita_Inizio")
                    Dim lavMesi As Integer = dtRichiesteLavorazioni.Rows(i).Item("Mesi")
                    Dim lavDataFine As Date = Nothing
                    If lavMesi > 0 Then
                        lavDataFine = lavDataInizio.AddMonths(dtRichiesteLavorazioni.Rows(i).Item("Mesi"))
                    End If

                    Dim dicituraAggiuntivaMacrouso As String = If(dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Regolamento_Cod") = 4, " (BIOLOGICO)", "")

                    drLavorazioniColt.GruppoCod = dtRichiesteLavorazioni.Rows(i).Item("Gruppo_Colturale_UMA")

                    drLavorazioniColt.FascicoloDes = helperReport.OttieniDescrizioneFascicolo_o_PCG(
                                                        fascicolo.FascicoloCod,
                                                        fascicolo.FascicoloDes,
                                                        dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Richiesta_Cod"),
                                                        drLavorazioniColt.GruppoCod)

                    drLavorazioniColt.GruppoDes = dtRichiesteLavorazioni.Rows(i).Item("Macrouso_UMA_Des") + dicituraAggiuntivaMacrouso
                    'drLavorazioniColt.SupTot = dtRichiesteLavorazioni.Rows(i).Item("Totale_Superficie_UMA")
                    'drLavorazioniColt.SupTotZonaA = dtRichiesteLavorazioni.Rows(i).Item("Zona_Pendenza_A_UMA")
                    'drLavorazioniColt.SupTotZonaB = dtRichiesteLavorazioni.Rows(i).Item("Zona_Pendenza_B_UMA")
                    drLavorazioniColt.LavDataInizio = lavDataInizio
                    drLavorazioniColt.LavDataFine = lavDataFine
                    drLavorazioniColt.LavTipoCod = dtRichiesteLavorazioni.Rows(i).Item("Lavorazione_UMA")
                    drLavorazioniColt.LavTipoDes = dtRichiesteLavorazioni.Rows(i).Item("Lav_UMA_Des")
                    drLavorazioniColt.LavCarburanteTipo = dtRichiesteLavorazioni.Rows(i).Item("Car_Des")
                    drLavorazioniColt.LavCarburanteQta = Math.Round(dtRichiesteLavorazioni.Rows(i).Item("Fabbisogno_Richiesto") * valMoltDecurt, 0)
                    drLavorazioniColt.LavSupTotale = dtRichiesteLavorazioni.Rows(i).Item("Totale_Superficie_UMA")
                    drLavorazioniColt.LavSupZonaA = dtRichiesteLavorazioni.Rows(i).Item("Zona_Pendenza_A_UMA")
                    drLavorazioniColt.LavSupZonaB = dtRichiesteLavorazioni.Rows(i).Item("Zona_Pendenza_B_UMA")
                    drLavorazioniColt.LavSupTerrenoNormale = dtRichiesteLavorazioni.Rows(i).Item("Zona_Tessitura_Normale_UMA")
                    drLavorazioniColt.LavSupTerrenoMedioImpasto = dtRichiesteLavorazioni.Rows(i).Item("Zona_Tessitura_Media_UMA")
                    drLavorazioniColt.LavSupTerrenoTenace = dtRichiesteLavorazioni.Rows(i).Item("Zona_Tessitura_Tenace_UMA")
                    drLavorazioniColt.LavSupMaggiorazioneTrasferimenti = dtRichiesteLavorazioni.Rows(i).Item("Superficie_Maggiorazione_Trasferimenti")
                    drLavorazioniColt.LavNote = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Note_Compilatore")
                    Dim udmAlt = dtRichiesteLavorazioni.Rows(i).Item("Udm_Alternativa")
                    If udmAlt = "" Then
                        drLavorazioniColt.LavFlagParticolare = False
                    Else
                        drLavorazioniColt.LavFlagParticolare = True
                    End If
                    drLavorazioniColt.LavUdm = udmAlt
                    drLavorazioniColt.LavQta = dtRichiesteLavorazioni.Rows(i).Item("Qta_Manuale")
                    drLavorazioniColt.LavMesi = lavMesi
                    'drLavorazioniColt.LavOre = 0 'TODO

                    dsLavorazioniTerzi.DT_LavorazioniTerzi.Rows.Add(drLavorazioniColt)
                Next

                'Nascondo il sottoreport degli allevamenti
                rptRendicontazioneCarb.FooterSectionAllev.SectionFormat.EnableSuppress = True

            End If

            'Quarto Sottoreport - Allegati
            For i = 0 To dtAllegatiRichiesta.Rows.Count - 1
                Dim drAllegati As DS_AllegatiRichiesta.DT_AllegatiRichiestaRow = dsAllegatiRichiesta.DT_AllegatiRichiesta.NewRow

                drAllegati.AllegatoNome = String.Format("{0} ({1})",
                    dtAllegatiRichiesta(i).Item("Allegati_Documenti_NomeFile"),
                    dtAllegatiRichiesta(i).Item("nome_tipologia"))

                drAllegati.AllegatoObbligo = dtAllegatiRichiesta(i).Item("Obbligatorio_Des")
                drAllegati.AllegatoFase = dtAllegatiRichiesta(i).Item("Fase_Des")

                dsAllegatiRichiesta.DT_AllegatiRichiesta.Rows.Add(drAllegati)
            Next

            'Quinto Sottoreport - Macchine
            For i = 0 To arrDrMacchineRichiesta.Count - 1
                Dim drMacchineDenunciate As DS_MacchineDenunciate.DT_MacchineDenunciateRow = dsMacchine.DT_MacchineDenunciate.NewRow

                drMacchineDenunciate.MacCod = arrDrMacchineRichiesta(i).Item("Mac_Cod")
                drMacchineDenunciate.Targa = arrDrMacchineRichiesta(i).Item("Targa")
                drMacchineDenunciate.DataCarico = arrDrMacchineRichiesta(i).Item("Data_Carico")
                drMacchineDenunciate.MacchinaTipo = arrDrMacchineRichiesta(i).Item("Class_Desc")
                'Non ho valorizzato Caratteristiche
                drMacchineDenunciate.MacchinaMarca = arrDrMacchineRichiesta(i).Item("Ditta_Des")
                drMacchineDenunciate.MacchinaModello = arrDrMacchineRichiesta(i).Item("modello")
                drMacchineDenunciate.MacchinaMatricola = arrDrMacchineRichiesta(i).Item("n_immatricolazione")
                drMacchineDenunciate.MotoreMarca = arrDrMacchineRichiesta(i).Item("Motore_Ditta_Des")
                drMacchineDenunciate.MotoreModello = arrDrMacchineRichiesta(i).Item("Tipo_Motore")
                drMacchineDenunciate.MotoreMatricola = arrDrMacchineRichiesta(i).Item("Matricola_Motore")
                drMacchineDenunciate.TipoPossesso = arrDrMacchineRichiesta(i).Item("TitoloPossesso_Des")
                drMacchineDenunciate.CarburanteAlimentazione = arrDrMacchineRichiesta(i)("Car_Des").ToString()
                drMacchineDenunciate.Intestatario = arrDrMacchineRichiesta(i).Item("Denominazione_Proprietario")
                drMacchineDenunciate.Peso = arrDrMacchineRichiesta(i).Item("Peso")
                'Non ho valorizzato PotenzaCv
                'Non ho valorizzato PotenzaKw
                'Non ho valorizzato Calorie
                'Non ho valorizzato Consumo

                dsMacchine.DT_MacchineDenunciate.Rows.Add(drMacchineDenunciate)
            Next

        Catch ex As Exception
            Log_Errori &= "- caricamento dataset: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        Try

            '--------------------------------------------
            ' AGGANCIO DATASET AL REPORT
            '--------------------------------------------
            rptRendicontazioneCarb.SetDataSource(dsRendicontazioneCarb)

            rptGruppiColturali.SetDataSource(dsGruppiColt)
            rptLavorazioniProprio.SetDataSource(dsLavorazioniProprio)
            rptLavorazioniTerzi.SetDataSource(dsLavorazioniTerzi)
            rptRichiesteAllev.SetDataSource(dsRichiesteAllev)
            rptAllegatiRichiesta.SetDataSource(dsAllegatiRichiesta)
            rptMacchineDenunciate.SetDataSource(dsMacchine)

            rptRendicontazioneCarb.OpenSubreport("Rpt_GruppiColturali.rpt").SetDataSource(dsGruppiColt)
            rptRendicontazioneCarb.OpenSubreport("Rpt_LavorazioniProprio.rpt").SetDataSource(dsLavorazioniProprio)
            rptRendicontazioneCarb.OpenSubreport("Rpt_LavorazioniTerzi.rpt").SetDataSource(dsLavorazioniTerzi)
            rptRendicontazioneCarb.OpenSubreport("Rpt_RichiesteAllevamenti.rpt").SetDataSource(dsRichiesteAllev)
            rptRendicontazioneCarb.OpenSubreport("Rpt_AllegatiRichiesta.rpt").SetDataSource(dsAllegatiRichiesta)
            rptRendicontazioneCarb.OpenSubreport("Rpt_MacchineDenunciate.rpt").SetDataSource(dsMacchine)

        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        '#########################################################

        Try

            '--------------------------------------------
            ' IMPOSTAZIONE PARAMETRI
            '(va fatto dopo il SetDataSource altrimenti da errore! )
            '--------------------------------------------

            'rptRichiestaCarb.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            'rptRichiestaCarb.SetParameterValue("Dichiarazione", Param_Dichiarazione)

            rptRendicontazioneCarb.SetParameterValue("TitoloReport", paramGruppiColtTitolo, "Rpt_GruppiColturali.rpt")
            rptRendicontazioneCarb.SetParameterValue("TitoloReport", paramLavTitolo, "Rpt_LavorazioniProprio.rpt")
            rptRendicontazioneCarb.SetParameterValue("TitoloReport", paramLavTitolo, "Rpt_LavorazioniTerzi.rpt")

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

    End Sub
End Class