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
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaBiz

Public Class RichiestaCarbPrevisioneLav
    Inherits System.Web.UI.Page

    Private rptRichiestaCarb As Rpt_RichiestaCarbPrevisioneLav
    Private rptGruppiColturali As Rpt_GruppiColturali
    Private rptLavorazioniProprio As Rpt_LavorazioniProprio
    Private rptLavorazioniTerzi As Rpt_LavorazioniTerzi
    Private rptRichiesteAllev As Rpt_RichiesteAllevamenti
    Private rptMacchineDenunciate As Rpt_MacchineDenunciate
    Private rptAllegatiRichiesta As Rpt_AllegatiRichiesta
    Private rptRimanenzeInutilizzo As Rpt_GestioneRimanenzeInutilizzo

    Private Log_Errori As String

    Private Qs_Piva As String
    Private Qs_CodRichiestaTestata As Integer

    Private Param_Dichiarazione As String = ""
    Private Param_Rag_Soc As String = ""


    Private objParametri_Server As New AgronicaCoreParametri
    Private objParametri_Utenti As New AgronicaCoreParametri

    Private nomeDocIdentifPratica As String = ""
    Private Tipo_Azienda As Integer
    Private isLavorazioneParzialeRichiesta As Boolean = False

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        rptRichiestaCarb = New Rpt_RichiestaCarbPrevisioneLav
        rptGruppiColturali = New Rpt_GruppiColturali
        rptLavorazioniProprio = New Rpt_LavorazioniProprio
        rptLavorazioniTerzi = New Rpt_LavorazioniTerzi
        rptRichiesteAllev = New Rpt_RichiesteAllevamenti
        rptMacchineDenunciate = New Rpt_MacchineDenunciate
        rptAllegatiRichiesta = New Rpt_AllegatiRichiesta
        rptRimanenzeInutilizzo = New Rpt_GestioneRimanenzeInutilizzo

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

        Dim Nome_Documento As String = "RichiestaCarbPrevisioneLav"
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
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptRichiestaCarb,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)
                'Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
                'Dim dummy = rptRichiestaCarb.FormatEngine.GetLastPageNumber(prc)
                rptRichiestaCarb.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", Partita Iva = " & CStr(Qs_Piva) & vbCrLf & vbCrLf & Log_Errori

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
                            "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))



        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_RichiestaCarb()

        'DataSet Reports
        Dim dsRichiestaCarb As New DS_RichiestaCarbPrevisioneLav
        Dim dsGruppiColt As New DS_GruppiColturali
        Dim dsLavorazioniProprio As New DS_LavorazioniProprio
        Dim dsLavorazioniTerzi As New DS_LavorazioniTerzi
        Dim dsRichiesteAllev As New DS_RichiesteAllevamenti
        Dim dsAllegatiRichiesta As New DS_AllegatiRichiesta
        Dim dsMacchine As New DS_MacchineDenunciate
        Dim dsRimanenzeInutilizzo As New DS_GestioneRimanenzeInutilizzo

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
        Dim handleSchemaDocTemplate As New RichiestaDocumenti_R()
        Dim leggiPivaReale As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim helperReport As New HelperStampeCarburantiUMA(objParametri_Server, objParametri_Utenti)

        'DataTable delle query
        Dim dtImpresa As New DataTable
        Dim dtCompilatore As New DataTable
        Dim dtUmaSetup As New DataTable
        '''0 = non abilitata, 1 = abilitata, 2 = abilitata solo rendicontazioni
        Dim setupGestioneRimanenze As Integer
        Dim dtRichiesteGruppi As New DataTable
        Dim dtRichiesteLavorazioni As New DataTable
        Dim dtRichiesteAllevamenti As New DataTable
        Dim dtAllegatiRichiesta As New DataTable
        Dim dtAllegatiInformazioni As New DataTable
        Dim arrDrAllegatiInformazioni As DataRow() = New DataRow() {}
        Dim arrDrMacchineRichiesta As DataRow() = New DataRow() {}
        Dim impresaUfficioRea As String = ""
        Dim impresaNumeroRea As String = ""
        Dim pivaReale As String = ""

        'Dati richiesta corrente e riepiloghi anni precedenti
        Dim drRichiestaCorrente As DataRow = Nothing
        Dim annoPratica As Integer
        Dim numeroPratica As Integer
        Dim flagRichiestaIntegrazione As Boolean
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

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessione(False, objParametri_Server)

            'Ottengo i dati dell'azienda
            dtImpresa = handleImprese.DatiIntestazioneImpresa(Qs_Piva, "", "", objParametri_Server)

            Dim filtroCodiceRea = String.Format("id_cod IN ({0}, {1})", CInt(enum_CodiciAnagrafe.UfficioRea), CInt(enum_CodiciAnagrafe.NumeroRea))
            Dim dtCodiceRea = handleImpreseCodici.Leggi(Qs_Piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtroCodiceRea, "", objParametri_Server)

            If dtCodiceRea.Rows.Count = 2 Then
                If dtCodiceRea.Select("id_cod = " & enum_CodiciAnagrafe.UfficioRea).FirstOrDefault() IsNot Nothing AndAlso
                        dtCodiceRea.Select("id_cod = " & enum_CodiciAnagrafe.NumeroRea).FirstOrDefault() IsNot Nothing Then

                    impresaUfficioRea = dtCodiceRea.Select("id_cod = " & enum_CodiciAnagrafe.UfficioRea).First().Field(Of String)("val_cod")
                    impresaNumeroRea = dtCodiceRea.Select("id_cod = " & enum_CodiciAnagrafe.NumeroRea).First().Field(Of String)("val_cod")
                End If
            End If

            'Ottengo i dati delle richieste_testata e delle pratiche dell'impresa
            Dim dtRichiestePratiche = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, False)
            Dim dtRichiestePraticheT = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, True)
            dtRichiestePratiche.Merge(dtRichiestePraticheT)

            'Elaboro i dati delle richieste/pratiche per ottenere la richiesta_testata specifica ed eventuali richieste di riepilogo precedenti
            drRichiestaCorrente = dtRichiestePratiche.Select(
                String.Format("Richiesta_Cod = {0}", Qs_CodRichiestaTestata)
                ).FirstOrDefault()
            annoPratica = drRichiestaCorrente.Field(Of Integer)("Anno")
            numeroPratica = drRichiestaCorrente.Field(Of String)("Numero")
            nomeDocIdentifPratica = annoPratica & "-" & numeroPratica
            Tipo_Azienda = drRichiestaCorrente.Field(Of Integer)("Tipo_Azienda")

            'Ottengo i dati delle richieste
            dtRichiesteGruppi = handleRichieste.Leggi(
                Qs_Piva,
                Qs_CodRichiestaTestata,
                "",
                0,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                objParametri_Server)

            If Tipo_Azienda = enum_TipoAzienda_UMA.Azienda_Terzista AndAlso drRichiestaCorrente.Field(Of Integer)("Avanzamento_Richiesta") = enum_UMA_Avanzamento.Richiesta Then
                isLavorazioneParzialeRichiesta = True
                'Ottengo i dati delle lavorazioni
                'Non specifico la piva perché le lavorazioni conto terzi possono averne una differente
                dtRichiesteLavorazioni = handleRichiesteLavorazioni.Leggi_Lavorazioni_Parziali(
                    Qs_Piva,
                    Qs_CodRichiestaTestata,
                    0,
                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                    objParametri_Server)

                'Aggiungo colonne mancant per non far schiantare il pregresso
                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Gruppo_Colturale_UMA", GetType(String)))

                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))

                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))

                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Note_Compilatore", GetType(String)))

                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Qta_Manuale", GetType(Double)))
                dtRichiesteLavorazioni.Columns.Add(New DataColumn("Mesi", GetType(Integer)))

                For Each row In dtRichiesteLavorazioni.Rows
                    row.item("Gruppo_Colturale_UMA") = ""
                    row.item("Programmazione_Cod") = 0
                    row.item("Programmazione_Des") = ""
                    row.item("Macrouso_UMA_Des") = ""
                    row.item("Note_Compilatore") = ""
                    row.item("Qta_Manuale") = 0.0
                    row.item("Mesi") = 0
                Next
            Else
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
            End If


            If drRichiestaCorrente.Field(Of Integer)("Richiesta_Integrativa") = 0 OrElse drRichiestaCorrente.Field(Of Integer)("Richiesta_Integrativa").Equals(DBNull.Value) Then
                flagRichiestaIntegrazione = False
            Else
                flagRichiestaIntegrazione = True
            End If

            'Ottengo i dati del compilatore: considero lo stesso come l'utente che ha creato la testata/pratica
            dtCompilatore = handleCompilatore.LeggiUtenti(drRichiestaCorrente.Field(Of String)("Username_Creazione"), 0, "", "", objParametri_Utenti)

            'Ottengo dati di configurazione come la percentuale di decurtamento da applicare alle quantità di carburante
            dtUmaSetup = handleUmaSetup.LeggiSetup(annoPratica, objParametri_Server)

            'Ottengo i totali di carburante dalle lavorazioni della richiesta
            carbPerTipoCorr = handleRichiesteBiz.LeggiTotaliCarburantePerTipo(Qs_CodRichiestaTestata, objParametri_Server, isLavorazioneParzialeRichiesta)

            'Individuo il tipo della richiesta
            flagRichiestaProprio = drRichiestaCorrente.Field(Of Integer)("Tipo_Richiesta") = 0
            flagRichiestaTerzi = drRichiestaCorrente.Field(Of Integer)("Tipo_Richiesta") = -1

            'Individuo eventuali rendicontazioni dell'anno precedente
            drRendicontazioneAnnoPrecProprio = dtRichiestePratiche.Select(
                "Avanzamento_Richiesta = 1 And Tipo_Richiesta = 0 And Anno = " & annoPratica - 1
                ).FirstOrDefault()
            drRendicontazioneAnnoPrecTerzi = dtRichiestePratiche.Select(
                "Avanzamento_Richiesta = 1 And Tipo_Richiesta = -1 And Anno = " & annoPratica - 1
                ).FirstOrDefault()

            If drRendicontazioneAnnoPrecProprio IsNot Nothing OrElse drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
                aziendaNuovaIscrizione = False
            End If

            'Ottengo i totali del carburante inerenti alla rendicontazione in proprio anno precedente
            If drRendicontazioneAnnoPrecProprio IsNot Nothing Then
                carbPerTipoAnnoPrecProprio = handleRichiesteBiz.LeggiTotaliCarburantePerTipo(
                    drRendicontazioneAnnoPrecProprio("Richiesta_Cod"), objParametri_Server, isLavorazioneParzialeRichiesta)
            End If

            'Ottengo i totali del carburante inerenti alla rendicontazione per terzi anno precedente
            If drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
                carbPerTipoAnnoPrecTerzi = handleRichiesteBiz.LeggiTotaliCarburantePerTipo(
                    drRendicontazioneAnnoPrecTerzi("Richiesta_Cod"), objParametri_Server, isLavorazioneParzialeRichiesta)
            End If


            'Ottengo i dati degli allegati alla richiesta testata
            Dim filtroAggAlertEntita = "Alert_Entita.Richiesta_Cod = " & Qs_CodRichiestaTestata
            dtAllegatiRichiesta = handleAlertEntita.Leggi_con_documenti_2(0, filtroAggAlertEntita, "", objParametri_Server)
            'Non utilizzo questa funzione, perché restituisce solo le tipologie di documenti inserite e in alcune fasi specifiche
            'dtAllegatiInformazioni = handleSchemaDocTemplate.LeggiRichiestaDocumenti(objParametri_Server, Qs_Piva, Qs_CodRichiestaTestata)
            'arrDrAllegatiInformazioni = dtAllegatiInformazioni.Select("Nr_Documenti_Presenti = 1")

            'Ottengo i dati delle macchine
            Dim dtParcoMacchine = handleParcoMacchine.ParcoMacchine_Leggi(
                Qs_Piva, 0, True, "", "", "", "", "", 0, "", False, 0, "", False, AGRODATAINIZIO, AGRODATAFINE,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim macchineImpiegate As String = drRichiestaCorrente.Field(Of String)("Macchine_Impiegate")
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

            setupGestioneRimanenze = dtUmaSetup.Rows(0)("Gestione_Rimanenze")

            If setupGestioneRimanenze = 1 Then
                'dtTrasferimenti = handleTrasferimenti.Leggi(Qs_Piva, Qs_CodRichiestaTestata, objParametri_Server)
                'dtRestituzioni = handleRestituzioni.Leggi(Qs_Piva, Qs_CodRichiestaTestata, objParametri_Server)
            End If

            pivaReale = leggiPivaReale.Leggi_PivaReale(Qs_Piva, objParametri_Server)

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

            Dim drRichiestaCarb As DS_RichiestaCarbPrevisioneLav.DT_RichiestaCarbPrevisioneLavRow =
                    dsRichiestaCarb.DT_RichiestaCarbPrevisioneLav.NewRow

            drRichiestaCarb.DataConvalida = DateTime.Now
            drRichiestaCarb.Ente = "REGIONE UMBRIA"
            drRichiestaCarb.Normativa = "Ai sensi dell'Art.47 D.P.R. 28/12/2000 n.445 s.m.i."

            drRichiestaCarb.DenComunitaMontana = "AFOR"

            '--------------- Dati utente utilizzati prima ed ultima pagina-----------------------
            drRichiestaCarb.CompilatoreOrganiz = dtCompilatore.Rows(0)("Gruppi_Utente_des")
            drRichiestaCarb.CompilatoreUfficio = "" 'TODO
            drRichiestaCarb.CompilatoreDettagli = String.Format(
                "({0}) {1}", dtCompilatore.Rows(0)("UserName"), dtCompilatore.Rows(0)("Dettagli")
            )
            '-------------------------------------------------------------------------------------

            If aziendaNuovaIscrizione = True Then
                drRichiestaCarb.DittaNuovaIscrizione = "SI"
                drRichiestaCarb.TipoRichiesta = If(flagRichiestaIntegrazione, "RICHIESTA INTEGRAZIONE ANNO IN CORSO", "RICHIESTA ANNO IN CORSO [Assegnazione su previsione lavorazioni]")
            Else
                drRichiestaCarb.DittaNuovaIscrizione = "NO"
                drRichiestaCarb.TipoRichiesta = If(flagRichiestaIntegrazione, "RICHIESTA INTEGRAZIONE ANNO IN CORSO", "RICHIESTA ANNO IN CORSO [Assegnazione su base anno precedente]")
            End If


            drRichiestaCarb.AnnoRichiesta = annoPratica
            drRichiestaCarb.NumeroRichiesta = numeroPratica

            drRichiestaCarb.DichiarPiva = pivaReale
            drRichiestaCarb.DichiarCodFisc = Qs_Piva
            drRichiestaCarb.DichiarTipoAz = dtImpresa.Rows(0).Field(Of String)("Forma_Giuridica_Des")
            If dtImpresa.Rows(0).Field(Of String)("Forma_Giuridica_Cod") = "18" OrElse dtImpresa.Rows(0).Field(Of String)("Forma_Giuridica_Cod") = "19" Then
                drRichiestaCarb.DichiarBonificaUniversita = "X"
            End If
            drRichiestaCarb.DichiarRagSocCogn = dtImpresa.Rows(0).Field(Of String)("rag_soc")
            drRichiestaCarb.DichiarComune = dtImpresa.Rows(0).Field(Of String)("LOCALITA")
            drRichiestaCarb.DichiarProv = dtImpresa.Rows(0).Field(Of String)("COMUNI_PROV")
            drRichiestaCarb.DichiarProvIstat = "" ' Identificare il tipo di codice, esempio Todi (PG) ha codice 054052
            drRichiestaCarb.DichiarCap = dtImpresa.Rows(0).Field(Of String)("CAP")
            drRichiestaCarb.DichiarIndir = dtImpresa.Rows(0).Field(Of String)("ind_impresa")
            drRichiestaCarb.Cuaa = dtImpresa.Rows(0).Field(Of String)("codice_cuaa")

            drRichiestaCarb.CodRea = impresaUfficioRea & impresaNumeroRea

            '------------------Dati ultima pagina---------------------------------
            drRichiestaCarb.CompilatoreDichiarImprIdentif = "X"
            drRichiestaCarb.CompilatoreDichiarRichCompleta = "X"
            drRichiestaCarb.CompilatoreDichiarImprFirma = "X"
            drRichiestaCarb.CompilatoreDichiarRichArchiviata = "X"
            '-----------------------------------------------------------------------


            '-----------Inizio dati seconda pagina------------------------------------------------------------------------------------------------------
            If flagRichiestaProprio = True Then
                drRichiestaCarb.ProprioTerzi = "PROPRIO"

            ElseIf flagRichiestaTerzi = True Then
                drRichiestaCarb.ProprioTerzi = "TERZI"

            End If

            drRichiestaCarb.DataFineAnnoPrec = New Date(annoPratica - 1, 12, 31)

            Dim percentualeDecurtamento = dtUmaSetup.Rows(0).Field(Of Double)("Per_Riduzione")
            Dim valMoltDecurt = (100 - percentualeDecurtamento) / 100

            If flagRichiestaIntegrazione = False Then
                'drRichiestaCarb.RiepilogoAssegnazioneCarb = String.Format(
                '    "Che durante l'anno prevede di effettuare lavorazioni per le quali chiede l'autorizzazione " &
                '    "ad ottenere i seguenti quantitativi di carburante agricolo, al netto di eventuale quantitativo residuo " &
                '    "e detrazione per l'annualità {0}",
                '    IIf(percentualeDecurtamento = 0, "a norma di legge", "del " + percentualeDecurtamento.ToString("F2") + "%"))
            Else
                rptRichiestaCarb.HeaderSectionCarbRimanenze.SectionFormat.EnableSuppress = True
            End If

            drRichiestaCarb.RiepilogoAssegnazioneCarb = String.Format(
                    "Che durante l'anno prevede di effettuare lavorazioni per le quali chiede l'autorizzazione " &
                    "ad ottenere i seguenti quantitativi di carburante agricolo, " &
                    "al netto della detrazione per l'annualità {0}",
                    IIf(percentualeDecurtamento = 0, "a norma di legge", "del " + percentualeDecurtamento.ToString("F2") + "%"))

            Dim carbHelperDefault As New CarburanteHelper() With {
                .Tipo = 0, .Calcolato = 0, .Richiesto = 0, .Assegnato = 0
            }

            'TODO Il carburante Acquistato non lo abbiamo in queste tabelle, pertanto occorrerà ricavarlo in modo diverso. EDIT: Nascosta al 11/06 la sezione di riepilogo corrispondente

            If drRendicontazioneAnnoPrecProprio IsNot Nothing Then
                drRichiestaCarb.AnnoPrecAssegnatoProprioBenzina = carbPerTipoAnnoPrecProprio.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                drRichiestaCarb.AnnoPrecAssegnatoProprioGasolio = carbPerTipoAnnoPrecProprio.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                drRichiestaCarb.AnnoPrecAssegnatoProprioGasolioSerra = carbPerTipoAnnoPrecProprio.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato

                'drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina = drRendicontazioneAnnoPrecProprio.Field(Of Double)("Rimanenza_Benzina")
                'drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio = drRendicontazioneAnnoPrecProprio.Field(Of Double)("Rimanenza_Gasolio")
                'drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra = drRendicontazioneAnnoPrecProprio.Field(Of Double)("Rimanenza_Gasolio_Serra")
            Else
                drRichiestaCarb.AnnoPrecAssegnatoProprioBenzina = 0
                drRichiestaCarb.AnnoPrecAssegnatoProprioGasolio = 0
                drRichiestaCarb.AnnoPrecAssegnatoProprioGasolioSerra = 0

                'drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina = 0
                'drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio = 0
                'drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra = 0
            End If

            If drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
                drRichiestaCarb.AnnoPrecAssegnatoTerziBenzina = carbPerTipoAnnoPrecTerzi.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                drRichiestaCarb.AnnoPrecAssegnatoTerziGasolio = carbPerTipoAnnoPrecTerzi.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                drRichiestaCarb.AnnoPrecAssegnatoTerziGasolioSerra = carbPerTipoAnnoPrecTerzi.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato

                'drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina = drRendicontazioneAnnoPrecTerzi.Field(Of Double)("Rimanenza_Benzina")
                'drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio = drRendicontazioneAnnoPrecTerzi.Field(Of Double)("Rimanenza_Gasolio")
                'drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra = drRendicontazioneAnnoPrecTerzi.Field(Of Double)("Rimanenza_Gasolio_Serra")
            Else
                drRichiestaCarb.AnnoPrecAssegnatoTerziBenzina = 0
                drRichiestaCarb.AnnoPrecAssegnatoTerziGasolio = 0
                drRichiestaCarb.AnnoPrecAssegnatoTerziGasolioSerra = 0

                'drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina = 0
                'drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio = 0
                'drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra = 0
            End If

            'If carbPerTipoCorr.Count > 0 Then
            'Per quanto riguarda il carburante richiesto, posso prendere una scorciatoia in quanto questo è riportato nella tabella delle testate attraverso colonne apposite

            'Questo modo di calcolare il carburante prende anche in considerazione eventuali altre tipologie di carburanti, ma attualmente queste sono state bloccate in inserimento
            'Dim carbRichTot = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto") * valMoltDecurt, 4)

            'Dim statoCod = drRichiestaCorrente.Field(Of Integer)("Stato_Cod")
            'statoCod <> enum_WAnagraficaStati.In_Compilazione AndAlso

            If flagRichiestaProprio = True Then
                Dim carbAssegnabileBenz As Double
                Dim carbAssegnabileGas As Double
                Dim carbAssegnabileSer As Double

                'Per le richieste conto proprio ho queste priorità:
                '1) Approvazione da calcolo lavorazioni
                '2) Richiesta da calcolo lavorazioni

                carbAssegnabileBenz = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Benzina") * valMoltDecurt, 0)
                carbAssegnabileGas = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio") * valMoltDecurt, 0)
                carbAssegnabileSer = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio_Serra") * valMoltDecurt, 0)

                'Dim totaliBenzAssegn = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                'If totaliBenzAssegn = 0 Then
                '    carbAssegnabileBenz = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Benzina") * valMoltDecurt, 0)
                'Else
                '    carbAssegnabileBenz = totaliBenzAssegn
                'End If

                'Dim totaliGasAssegn = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                'If totaliGasAssegn = 0 Then
                '    carbAssegnabileGas = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio") * valMoltDecurt, 0)
                'Else
                '    carbAssegnabileGas = totaliGasAssegn
                'End If

                'Dim totaliSerAssegn = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                'If totaliSerAssegn = 0 Then
                '    carbAssegnabileSer = Math.Round(drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio_Serra") * valMoltDecurt, 0)
                'Else
                '    carbAssegnabileSer = totaliSerAssegn
                'End If

                drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina = If(drRichiestaCorrente("Rimanenza_Benzina").Equals(DBNull.Value), 0, drRichiestaCorrente.Field(Of Double)("Rimanenza_Benzina"))
                drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio = If(drRichiestaCorrente("Rimanenza_Gasolio").Equals(DBNull.Value), 0, drRichiestaCorrente.Field(Of Double)("Rimanenza_Gasolio"))
                drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra = If(drRichiestaCorrente("Rimanenza_Gasolio_Serra").Equals(DBNull.Value), 0, drRichiestaCorrente.Field(Of Double)("Rimanenza_Gasolio_Serra"))

                'Dim carbRichBenzNetto = carbAssegnabileBenz - drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina
                'Dim carbRichGasNetto = carbAssegnabileGas - drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio
                'Dim carbRichSerNetto = carbAssegnabileSer - drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra

                'drRichiestaCarb.AnnoCorrRichiestoProprioBenzina = If(carbRichBenzNetto > 0, carbRichBenzNetto, 0)
                'drRichiestaCarb.AnnoCorrRichiestoProprioGasolio = If(carbRichGasNetto > 0, carbRichGasNetto, 0)
                'drRichiestaCarb.AnnoCorrRichiestoProprioGasolioSerra = If(carbRichSerNetto > 0, carbRichSerNetto, 0)

                drRichiestaCarb.AnnoCorrRichiestoProprioBenzina = carbAssegnabileBenz
                drRichiestaCarb.AnnoCorrRichiestoProprioGasolio = carbAssegnabileGas
                drRichiestaCarb.AnnoCorrRichiestoProprioGasolioSerra = carbAssegnabileSer
                drRichiestaCarb.AnnoCorrRichiestoProprioTotale = drRichiestaCarb.AnnoCorrRichiestoProprioBenzina +
                    drRichiestaCarb.AnnoCorrRichiestoProprioGasolio + drRichiestaCarb.AnnoCorrRichiestoProprioGasolioSerra

                drRichiestaCarb.AnnoCorrRichiestoTerziBenzina = 0
                drRichiestaCarb.AnnoCorrRichiestoTerziGasolio = 0
                drRichiestaCarb.AnnoCorrRichiestoTerziGasolioSerra = 0
                drRichiestaCarb.AnnoCorrRichiestoTerziTotale = 0

                drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina = 0
                drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio = 0
                drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra = 0

            ElseIf flagRichiestaTerzi = True Then
                Dim carbAssegnabileBenz As Double
                Dim carbAssegnabileGas As Double
                Dim carbAssegnabileSer As Double

                'Per le richieste conto terzi ho queste priorità:
                '1) Approvazione Iniziale
                '2) Approvazione da calcolo lavorazioni
                '3) Richiesta Iniziale
                '4) Richiesta da calcolo lavorazioni

                Dim richInizialeBenz = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Benzina")
                If Not richInizialeBenz.Equals(DBNull.Value) AndAlso richInizialeBenz <> 0 Then
                    '3) Richiesta Iniziale
                    carbAssegnabileBenz = richInizialeBenz
                Else
                    '4) Richiesta da calcolo lavorazioni
                    carbAssegnabileBenz = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Benzina")
                End If
                'Per le richieste, decurto la quantità
                carbAssegnabileBenz = Math.Round(carbAssegnabileBenz * valMoltDecurt, 0)


                'Dim apprInizialeBenz = drRichiestaCorrente.Field(Of Double)("Approvazione_Iniziale_Benzina")
                'If Not apprInizialeBenz.Equals(DBNull.Value) AndAlso apprInizialeBenz <> 0 Then
                '    '1) Approvazione Iniziale
                '    carbAssegnabileBenz = apprInizialeBenz
                'Else
                '    Dim totaliBenzAssegn = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                '    If statoCod <> enum_WAnagraficaStati.In_Compilazione AndAlso totaliBenzAssegn <> 0 Then
                '        '2) Approvazione da calcolo lavorazioni
                '        carbAssegnabileBenz = totaliBenzAssegn
                '    Else
                '        Dim richInizialeBenz = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Benzina")
                '        If Not richInizialeBenz.Equals(DBNull.Value) AndAlso richInizialeBenz <> 0 Then
                '            '3) Richiesta Iniziale
                '            carbAssegnabileBenz = richInizialeBenz
                '        Else
                '            '4) Richiesta da calcolo lavorazioni
                '            carbAssegnabileBenz = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Benzina")
                '        End If
                '        'Per le richieste, decurto la quantità
                '        carbAssegnabileBenz = Math.Round(carbAssegnabileBenz * valMoltDecurt, 0)
                '    End If
                'End If

                Dim richInizialeGas = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio")
                If Not richInizialeGas.Equals(DBNull.Value) AndAlso richInizialeGas <> 0 Then
                    '3) Richiesta Iniziale
                    carbAssegnabileGas = richInizialeGas
                Else
                    '4) Richiesta da calcolo lavorazioni
                    carbAssegnabileGas = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio")
                End If
                'Per le richieste, decurto la quantità
                carbAssegnabileGas = Math.Round(carbAssegnabileGas * valMoltDecurt, 0)

                'Dim apprInizialeGas = drRichiestaCorrente.Field(Of Double)("Approvazione_Iniziale_Gasolio")
                'If Not apprInizialeGas.Equals(DBNull.Value) AndAlso apprInizialeGas <> 0 Then
                '    '1) Approvazione Iniziale
                '    carbAssegnabileGas = apprInizialeGas
                'Else
                '    Dim totaliGasAssegn = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                '    If statoCod <> enum_WAnagraficaStati.In_Compilazione AndAlso totaliGasAssegn <> 0 Then
                '        '2) Approvazione da calcolo lavorazioni
                '        carbAssegnabileGas = totaliGasAssegn
                '    Else
                '        Dim richInizialeGas = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio")
                '        If Not richInizialeGas.Equals(DBNull.Value) AndAlso richInizialeGas <> 0 Then
                '            '3) Richiesta Iniziale
                '            carbAssegnabileGas = richInizialeGas
                '        Else
                '            '4) Richiesta da calcolo lavorazioni
                '            carbAssegnabileGas = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio")
                '        End If
                '        'Per le richieste, decurto la quantità
                '        carbAssegnabileGas = Math.Round(carbAssegnabileGas * valMoltDecurt, 0)
                '    End If
                'End If

                Dim richInizialeSer = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio_Serra")
                If Not richInizialeSer.Equals(DBNull.Value) AndAlso richInizialeSer <> 0 Then
                    '3) Richiesta Iniziale
                    carbAssegnabileSer = richInizialeSer
                Else
                    '4) Richiesta da calcolo lavorazioni
                    carbAssegnabileSer = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio_Serra")
                End If
                'Per le richieste, decurto la quantità
                carbAssegnabileSer = Math.Round(carbAssegnabileSer * valMoltDecurt, 0)

                'Dim apprInizialeSer = drRichiestaCorrente.Field(Of Double)("Approvazione_Iniziale_Gasolio_Serra")
                'If Not apprInizialeSer.Equals(DBNull.Value) AndAlso apprInizialeSer <> 0 Then
                '    '1) Approvazione Iniziale
                '    carbAssegnabileSer = apprInizialeSer
                'Else
                '    Dim totaliSerAssegn = carbPerTipoCorr.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                '    If statoCod <> enum_WAnagraficaStati.In_Compilazione AndAlso totaliSerAssegn <> 0 Then
                '        '2) Approvazione da calcolo lavorazioni
                '        carbAssegnabileSer = totaliSerAssegn
                '    Else
                '        Dim richInizialeSer = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio_Serra")
                '        If Not richInizialeSer.Equals(DBNull.Value) AndAlso richInizialeSer <> 0 Then
                '            '3) Richiesta Iniziale
                '            carbAssegnabileSer = richInizialeSer
                '        Else
                '            '4) Richiesta da calcolo lavorazioni
                '            carbAssegnabileSer = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio_Serra")
                '        End If
                '        'Per le richieste, decurto la quantità
                '        carbAssegnabileSer = Math.Round(carbAssegnabileSer * valMoltDecurt, 0)
                '    End If
                'End If

                'Dim apprInizialeSer = drRichiestaCorrente.Field(Of Double)("Approvazione_Iniziale_Gasolio_Serra")
                'If Not apprInizialeSer.Equals(DBNull.Value) AndAlso apprInizialeSer <> 0 Then
                '    carbAssegnabileSer = apprInizialeSer
                'Else
                '    Dim richInizialeSer = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio_Serra")

                '    carbAssegnabileSer = Math.Round(If(richInizialeSer.Equals(DBNull.Value), 0, richInizialeSer) * valMoltDecurt, 0)
                'End If

                'carbAssegnabileBenz = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Benzina")
                'carbAssegnabileGas = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio")
                'carbAssegnabileSer = drRichiestaCorrente.Field(Of Double)("Carburante_Richiesto_Gasolio_Serra")

                'If Not drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Benzina").Equals(DBNull.Value) Then
                '    carbAssegnabileBenz = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Benzina")
                'End If

                'If Not drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio").Equals(DBNull.Value) Then
                '    carbAssegnabileGas = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio")
                'End If

                'If Not drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio_Serra").Equals(DBNull.Value) Then
                '    carbAssegnabileSer = drRichiestaCorrente.Field(Of Double)("Richiesta_Iniziale_Gasolio_Serra")
                'End If

                'carbAssegnabileBenz = Math.Round(carbAssegnabileBenz * valMoltDecurt, 0)
                'carbAssegnabileGas = Math.Round(carbAssegnabileGas * valMoltDecurt, 0)
                'carbAssegnabileSer = Math.Round(carbAssegnabileSer * valMoltDecurt, 0)


                drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina = If(drRichiestaCorrente("Rimanenza_Benzina").Equals(DBNull.Value), 0, drRichiestaCorrente.Field(Of Double)("Rimanenza_Benzina"))
                drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio = If(drRichiestaCorrente("Rimanenza_Gasolio").Equals(DBNull.Value), 0, drRichiestaCorrente.Field(Of Double)("Rimanenza_Gasolio"))
                drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra = If(drRichiestaCorrente("Rimanenza_Gasolio_Serra").Equals(DBNull.Value), 0, drRichiestaCorrente.Field(Of Double)("Rimanenza_Gasolio_Serra"))

                'Dim carbRichBenzNetto = carbAssegnabileBenz - drRichiestaCarb.AnnoPrecRimanenzeTerziBenzina
                'Dim carbRichGasNetto = carbAssegnabileGas - drRichiestaCarb.AnnoPrecRimanenzeTerziGasolio
                'Dim carbRichSerNetto = carbAssegnabileSer - drRichiestaCarb.AnnoPrecRimanenzeTerziGasolioSerra

                'Modifica a gennaio 2024: è stato deciso di mostrare il carburante richiesto (al netto della decurtazione)
                'ma non al netto delle rimanenze, nella griglia del report il totale è ora chiamato "richiesto" e non "assegnabile"
                'drRichiestaCarb.AnnoCorrRichiestoTerziBenzina = If(carbRichBenzNetto > 0, carbRichBenzNetto, 0)
                'drRichiestaCarb.AnnoCorrRichiestoTerziGasolio = If(carbRichGasNetto > 0, carbRichGasNetto, 0)
                'drRichiestaCarb.AnnoCorrRichiestoTerziGasolioSerra = If(carbRichSerNetto > 0, carbRichSerNetto, 0)

                drRichiestaCarb.AnnoCorrRichiestoTerziBenzina = carbAssegnabileBenz
                drRichiestaCarb.AnnoCorrRichiestoTerziGasolio = carbAssegnabileGas
                drRichiestaCarb.AnnoCorrRichiestoTerziGasolioSerra = carbAssegnabileSer
                drRichiestaCarb.AnnoCorrRichiestoTerziTotale = drRichiestaCarb.AnnoCorrRichiestoTerziBenzina +
                    drRichiestaCarb.AnnoCorrRichiestoTerziGasolio + drRichiestaCarb.AnnoCorrRichiestoTerziGasolioSerra

                drRichiestaCarb.AnnoCorrRichiestoProprioBenzina = 0
                drRichiestaCarb.AnnoCorrRichiestoProprioGasolio = 0
                drRichiestaCarb.AnnoCorrRichiestoProprioGasolioSerra = 0
                drRichiestaCarb.AnnoCorrRichiestoProprioTotale = 0

                drRichiestaCarb.AnnoPrecRimanenzeProprioBenzina = 0
                drRichiestaCarb.AnnoPrecRimanenzeProprioGasolio = 0
                drRichiestaCarb.AnnoPrecRimanenzeProprioGasolioSerra = 0
            End If
            'End If

            If setupGestioneRimanenze = 1 Then
                helperReport.ValorizzaSR_Inutilizzati_Richiesta(rptRichiestaCarb, dsRimanenzeInutilizzo, drRichiestaCorrente, Qs_Piva, Qs_CodRichiestaTestata, flagRichiestaProprio, drRichiestaCarb.DataConvalida)
            Else
                rptRichiestaCarb.HeaderSectionGestRimInu.SectionFormat.EnableSuppress = True
            End If


            '------------Fine dati seconda pagina-------------------------------------------------------------------------------

            '-----------------------------------------------------------------------------
            dsRichiestaCarb.DT_RichiestaCarbPrevisioneLav.Rows.Add(drRichiestaCarb)
            '-----------------------------------------------------------------------------

            'Primo SottoReport - Gruppi Colturali
            For i = 0 To dtRichiesteGruppi.Rows.Count - 1
                Dim drGruppiColt As DS_GruppiColturali.DT_GruppiColturaliRow = dsGruppiColt.DT_GruppiColturali.NewRow
                Dim dicituraAggiuntivaMacrouso As String = If(dtRichiesteGruppi.Rows(i).Field(Of Integer)("Regolamento_Cod") = 4, " (BIOLOGICO)", "")
                drGruppiColt.FascicoloCod = dtRichiesteGruppi.Rows(i).Field(Of Integer)("Programmazione_Cod")
                drGruppiColt.GruppoCod = dtRichiesteGruppi.Rows(i).Field(Of String)("Gruppo_Colturale_UMA")
                drGruppiColt.FascicoloDes = helperReport.OttieniDescrizioneFascicolo_o_PCG(
                                                drGruppiColt.FascicoloCod,
                                                dtRichiesteGruppi.Rows(i).Field(Of String)("Programmazione_Des"),
                                                dtRichiesteGruppi.Rows(i).Field(Of Integer)("Richiesta_Cod"),
                                                drGruppiColt.GruppoCod)
                drGruppiColt.GruppoDes = dtRichiesteGruppi.Rows(i).Field(Of String)("Macrouso_UMA_Des") + dicituraAggiuntivaMacrouso
                drGruppiColt.SupTot = dtRichiesteGruppi.Rows(i).Field(Of Double)("Totale_Superficie_UMA_Edit")
                drGruppiColt.SupZonaA = dtRichiesteGruppi.Rows(i).Field(Of Double)("Zona_Pendenza_A_UMA_Edit")
                drGruppiColt.SupZonaB = dtRichiesteGruppi.Rows(i).Field(Of Double)("Zona_Pendenza_B_UMA_Edit")

                dsGruppiColt.DT_GruppiColturali.Rows.Add(drGruppiColt)
            Next

            If flagRichiestaProprio = True Then
                paramGruppiColtTitolo = "GRUPPI COLTURALI CONTO PROPRIO"

                'Nascondo il sottoreport delle lavorazioni per terzi
                rptRichiestaCarb.FooterSectionLavTerzi.SectionFormat.EnableSuppress = True

                'Secondo SottoReport - Lavorazioni Colturali Conto Proprio
                paramLavTitolo = "LAVORAZIONI CONTO PROPRIO"
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

                    Dim dicituraAggiuntivaMacrouso As String = If(dtRichiesteGruppi.Select("Gruppo_Colturale_UMA = " & dtRichiesteLavorazioni.Rows(i).Field(Of String)("Gruppo_Colturale_UMA")).First.Field(Of Integer)("Regolamento_Cod") = 4, " (BIOLOGICO)", "")

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
                    drLavorazioniColt.LavDataInizio = Nothing
                    drLavorazioniColt.LavDataFine = Nothing
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
                    drLavorazioniColt.LavMesi = dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Mesi")
                    'drLavorazioniColt.LavOre = 0 'TODO

                    dsLavorazioniProprio.DT_LavorazioniProprio.Rows.Add(drLavorazioniColt)
                Next

                'Terzo Sottoreport - Allevamenti
                For i = 0 To dtRichiesteAllevamenti.Rows.Count - 1
                    Dim drRichAllev As DS_RichiesteAllevamenti.DT_RichiesteAllevamentiRow = dsRichiesteAllev.DT_RichiesteAllevamenti.NewRow

                    drRichAllev.GruppoCod = dtRichiesteAllevamenti.Rows(i).Field(Of String)("UMA_AllGru_Cod")
                    drRichAllev.GruppoDes = dtRichiesteAllevamenti.Rows(i).Field(Of String)("UMA_AllGru_Des")
                    drRichAllev.TipoCod = dtRichiesteAllevamenti.Rows(i).Field(Of String)("UMA_All_Cod")
                    drRichAllev.TipoDes = dtRichiesteAllevamenti.Rows(i).Field(Of String)("UMA_All_Des")
                    drRichAllev.TotaleCapi = dtRichiesteAllevamenti.Rows(i).Field(Of Integer)("Totale_Capi")
                    drRichAllev.CarburanteTipo = dtRichiesteAllevamenti.Rows(i).Field(Of String)("Car_Des")
                    drRichAllev.CarburanteQta = Math.Round(dtRichiesteAllevamenti.Rows(i).Field(Of Double)("Carburante_Richiesto") * valMoltDecurt, 0)
                    drRichAllev.Note = dtRichiesteAllevamenti.Rows(i).Field(Of String)("Note_Compilatore")

                    dsRichiesteAllev.DT_RichiesteAllevamenti.Rows.Add(drRichAllev)
                Next

            End If

            If flagRichiestaTerzi = True Then
                paramGruppiColtTitolo = "GRUPPI COLTURALI CONTO TERZI"

                'Nascondo il sottoreport delle lavorazioni in proprio
                rptRichiestaCarb.FooterSectionLavProprio.SectionFormat.EnableSuppress = True

                'Secondo SottoReport - Lavorazioni Colturali Conto Terzi
                paramLavTitolo = "LAVORAZIONI CONTO TERZI"
                For i = 0 To dtRichiesteLavorazioni.Rows.Count - 1
                    Dim drLavorazioniColt As DS_LavorazioniTerzi.DT_LavorazioniTerziRow = dsLavorazioniTerzi.DT_LavorazioniTerzi.NewRow

                    Dim dtImpresaBeneficiaria = handleImprese.DatiIntestazioneImpresa(dtRichiesteLavorazioni.Rows(i).Field(Of String)("Piva"), "", "", objParametri_Server)
                    drLavorazioniColt.BeneficiarioCuaa = dtImpresaBeneficiaria.Rows(0).Field(Of String)("codice_cuaa")
                    drLavorazioniColt.BeneficiarioNominativo = dtImpresaBeneficiaria.Rows(0).Field(Of String)("rag_soc")

                    Dim fascicoloDes As String = ""

                    drLavorazioniColt.GruppoCod = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Gruppo_Colturale_UMA")

                    If dtRichiesteGruppi.Rows.Count > 0 Then
                        Dim fascicolo = (From drRichiesta In dtRichiesteGruppi.AsEnumerable()
                                         Where drRichiesta.Field(Of String)("Gruppo_Colturale_UMA") =
                                            dtRichiesteLavorazioni.Rows(i).Field(Of String)("Gruppo_Colturale_UMA")
                                         Select New With {
                             .FascicoloCod = drRichiesta.Field(Of Integer)("Programmazione_Cod"),
                             .FascicoloDes = drRichiesta.Field(Of String)("Programmazione_Des")
                             }).First()

                        fascicoloDes = helperReport.OttieniDescrizioneFascicolo_o_PCG(
                                        fascicolo.FascicoloCod,
                                        fascicolo.FascicoloDes,
                                        dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Richiesta_Cod"),
                                        drLavorazioniColt.GruppoCod)
                    End If

                    Dim dicituraAggiuntivaMacrouso As String = If(dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Regolamento_Cod") = 4, " (BIOLOGICO)", "")
                    drLavorazioniColt.FascicoloDes = fascicoloDes
                    drLavorazioniColt.GruppoDes = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Macrouso_UMA_Des")
                    drLavorazioniColt.SupTot = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Totale_Superficie_UMA")
                    drLavorazioniColt.SupTotZonaA = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Pendenza_A_UMA")
                    drLavorazioniColt.SupTotZonaB = dtRichiesteLavorazioni.Rows(i).Field(Of Double)("Zona_Pendenza_B_UMA")
                    drLavorazioniColt.NumLavorazioniEff = dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Nr_Lavorazioni_Richieste")
                    drLavorazioniColt.LavTipoCod = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Lavorazione_UMA")
                    drLavorazioniColt.LavTipoDes = dtRichiesteLavorazioni.Rows(i).Field(Of String)("Lav_UMA_Des") + dicituraAggiuntivaMacrouso
                    drLavorazioniColt.LavDataInizio = Nothing
                    drLavorazioniColt.LavDataFine = Nothing
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
                    drLavorazioniColt.LavMesi = dtRichiesteLavorazioni.Rows(i).Field(Of Integer)("Mesi")
                    'drLavorazioniColt.LavOre = 0 'TODO

                    dsLavorazioniTerzi.DT_LavorazioniTerzi.Rows.Add(drLavorazioniColt)
                Next

                'Nascondo il sottoreport degli allevamenti
                rptRichiestaCarb.FooterSectionAllev.SectionFormat.EnableSuppress = True

            End If

            'Quarto Sottoreport - Allegati
            For i = 0 To dtAllegatiRichiesta.Rows.Count - 1
                Dim drAllegati As DS_AllegatiRichiesta.DT_AllegatiRichiestaRow = dsAllegatiRichiesta.DT_AllegatiRichiesta.NewRow

                drAllegati.AllegatoNome = String.Format("{0} ({1})",
                    dtAllegatiRichiesta(i).Field(Of String)("Allegati_Documenti_NomeFile"),
                    dtAllegatiRichiesta(i).Field(Of String)("nome_tipologia"))

                drAllegati.AllegatoObbligo = dtAllegatiRichiesta(i).Field(Of String)("Obbligatorio_Des")
                drAllegati.AllegatoFase = dtAllegatiRichiesta(i).Field(Of String)("Fase_Des")

                dsAllegatiRichiesta.DT_AllegatiRichiesta.Rows.Add(drAllegati)
            Next
            'For i = 0 To arrDrAllegatiInformazioni.Count - 1
            '    Dim drAllegati As DS_AllegatiRichiesta.DT_AllegatiRichiestaRow = dsAllegatiRichiesta.DT_AllegatiRichiesta.NewRow

            '    drAllegati.AllegatoNome = arrDrAllegatiInformazioni(i).Field(Of String)("Descrizione")
            '    drAllegati.AllegatoObbligo = arrDrAllegatiInformazioni(i).Field(Of String)("Obbligatorio")
            '    drAllegati.AllegatoFase = arrDrAllegatiInformazioni(i).Field(Of String)("Fase_Des")

            '    dsAllegatiRichiesta.DT_AllegatiRichiesta.Rows.Add(drAllegati)
            'Next

            'Quinto Sottoreport - Macchine
            For i = 0 To arrDrMacchineRichiesta.Count - 1
                Dim drMacchineDenunciate As DS_MacchineDenunciate.DT_MacchineDenunciateRow = dsMacchine.DT_MacchineDenunciate.NewRow

                drMacchineDenunciate.MacCod = arrDrMacchineRichiesta(i).Field(Of Integer)("Mac_Cod")
                drMacchineDenunciate.Targa = arrDrMacchineRichiesta(i).Field(Of String)("Targa")
                drMacchineDenunciate.DataCarico = arrDrMacchineRichiesta(i).Field(Of DateTime)("Data_Carico")
                drMacchineDenunciate.MacchinaTipo = arrDrMacchineRichiesta(i).Field(Of String)("Class_Desc")
                'Non ho valorizzato Caratteristiche
                drMacchineDenunciate.MacchinaMarca = arrDrMacchineRichiesta(i).Field(Of String)("Ditta_Des")
                drMacchineDenunciate.MacchinaModello = arrDrMacchineRichiesta(i).Field(Of String)("modello")
                drMacchineDenunciate.MacchinaMatricola = arrDrMacchineRichiesta(i).Field(Of String)("n_immatricolazione")
                drMacchineDenunciate.MotoreMarca = arrDrMacchineRichiesta(i).Field(Of String)("Motore_Ditta_Des")
                drMacchineDenunciate.MotoreModello = arrDrMacchineRichiesta(i).Field(Of String)("Tipo_Motore")
                drMacchineDenunciate.MotoreMatricola = arrDrMacchineRichiesta(i).Field(Of String)("Matricola_Motore")
                drMacchineDenunciate.TipoPossesso = arrDrMacchineRichiesta(i).Field(Of String)("TitoloPossesso_Des")
                drMacchineDenunciate.CarburanteAlimentazione = arrDrMacchineRichiesta(i).Field(Of String)("Car_Des")
                drMacchineDenunciate.Intestatario = arrDrMacchineRichiesta(i).Field(Of String)("Denominazione_Proprietario")
                drMacchineDenunciate.Peso = arrDrMacchineRichiesta(i).Field(Of Double)("Peso")
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
            rptRichiestaCarb.SetDataSource(dsRichiestaCarb)

            'rptGruppiColturali.SetDataSource(dsGruppiColt)
            rptLavorazioniProprio.SetDataSource(dsLavorazioniProprio)
            rptLavorazioniTerzi.SetDataSource(dsLavorazioniTerzi)
            rptRichiesteAllev.SetDataSource(dsRichiesteAllev)
            rptAllegatiRichiesta.SetDataSource(dsAllegatiRichiesta)
            rptMacchineDenunciate.SetDataSource(dsMacchine)
            rptRimanenzeInutilizzo.SetDataSource(dsRimanenzeInutilizzo)

            rptRichiestaCarb.OpenSubreport("Rpt_GruppiColturali.rpt").SetDataSource(dsGruppiColt)
            rptRichiestaCarb.OpenSubreport("Rpt_LavorazioniProprio.rpt").SetDataSource(dsLavorazioniProprio)
            rptRichiestaCarb.OpenSubreport("Rpt_LavorazioniTerzi.rpt").SetDataSource(dsLavorazioniTerzi)
            rptRichiestaCarb.OpenSubreport("Rpt_RichiesteAllevamenti.rpt").SetDataSource(dsRichiesteAllev)
            rptRichiestaCarb.OpenSubreport("Rpt_AllegatiRichiesta.rpt").SetDataSource(dsAllegatiRichiesta)
            rptRichiestaCarb.OpenSubreport("Rpt_MacchineDenunciate.rpt").SetDataSource(dsMacchine)
            rptRichiestaCarb.OpenSubreport("Rpt_GestioneRimanenzeInutilizzo.rpt").SetDataSource(dsRimanenzeInutilizzo)

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

            rptRichiestaCarb.SetParameterValue("TitoloReport", paramGruppiColtTitolo, "Rpt_GruppiColturali.rpt")
            rptRichiestaCarb.SetParameterValue("TitoloReport", paramLavTitolo, "Rpt_LavorazioniProprio.rpt")
            rptRichiestaCarb.SetParameterValue("TitoloReport", paramLavTitolo, "Rpt_LavorazioniTerzi.rpt")

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

    End Sub
End Class