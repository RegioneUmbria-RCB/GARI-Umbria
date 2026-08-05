Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.IO

Public Class Registro_RiBa
    Inherits System.Web.UI.Page

    Private Const _UpperBoundTabelle As Integer = 2000000000

    Private _rptRiba As New Rpt_RegistroRiBa

    Private _logErrori As String = ""

    Private _report As Integer
    Private _piva As String
    ' Private Data_Inizio As String
    ' Private Data_Fine As String
    ' Private Anno As Integer
    Private _codRapporto As Integer
    Private _codRisUm As Integer
    Private _tipoScadenza, _tipoNumeroDoc, _numeroDoc, _annoDoc As Integer
    Private _cbiCausale As Integer
    Private _scadenzaInizio, _scadenzaFine, _dataPagamento As Date
    Private _codIstitutoDare, _codIstitutoAvere, _codLiquiditaDare, _codLiquiditaAvere As Integer
    Private _flagPagamentoNonAvvenuto As Boolean
    Private _tipo As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region "Gestione evento uscita pagina master"
    'gestione evento uscita click pagina master necessaria la direttiva <%@ MasterType VirtualPath="~/Site.Master" %> nel file aspx.
    'la direttiva effettua un cast specifico dell'oggetto master alla pagina indicata nell'attributo VirtualPath
    Private Sub ProfilazioneHome_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.Annulla_Tutto

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Distinta richiesta anticipazioni: RI.BA."

    End Sub

    Private Sub Annulla_Tutto()

        Dim strJs1 As New StringBuilder
        strJs1.AppendLine("$(document).ready(function () { ")
        strJs1.AppendLine("      window.close() ")
        strJs1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJs1.ToString, True)


    End Sub

    'evento delegato btn uscita dalla pagina master, chiudo la form
    Protected Sub master_btnExit_click(ByVal sender As Object, ByVal e As EventArgs)
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "exit_k", "window.close();", True)
        'Response.Redirect("")
    End Sub

#End Region

    '#######################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
        Dim utenteAbilitato As Boolean
        '  Dim strDummy As String      'controllo accesso negato.....
        'utenteAbilitato = Controlla_Permessi_Utente_2(Server, Session, Page,
        '                            Session("ASG_Utente_Username"),
        '                            Session("ASG_IdServizio"),
        '                            enum_Security_Attivita.,
        '                            enum_Security_Operazione.Lettura,
        '                            strDummy)
        '----- !!!!!!!!!!! -------------
        'Attivazione forzata provvisoria
        utenteAbilitato = True
        '----- !!!!!!!!!!! -------------

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio ad AlberoImprese.
        If utenteAbilitato = False Then
            Me.FindControl("Form1").Controls.Add(New LiteralControl("<script language='javascript'> window.close() </script>"))
            Exit Sub
        End If

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        lblEsitoRiba.Text = ""
        ValorizzaDaQueryString()

        If Not Page.IsPostBack Then

            PopolaCBI_Causale()
            PopolaIstCredito()
            PopolaAnnoContabile()
            ValoriCombo()
            Gestione_Anteprima_Stampa(True, "", False)
        End If

    End Sub

    '#####################################################################################################
    Private Sub ValorizzaDaQueryString()


        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))

        'Anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        'Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        'Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _codRisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server)

        _codRapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")), AgroKey_EncoderDecoder, Server)

        _tipoScadenza = Stringa_Decodifica(CStr(Request.QueryString("tsc")), AgroKey_EncoderDecoder, Server)

        _scadenzaInizio = Stringa_Decodifica(CStr(Request.QueryString("sc")), AgroKey_EncoderDecoder, Server)

        _scadenzaFine = _scadenzaInizio

        If Not Page.IsPostBack Then
            Txt_ValiditaInizio.Text = meseCorrente_DataprimoGiorno(_scadenzaInizio.AddMonths(1))
            Txt_ValiditaFine.Text = meseCorrente_DataultimoGiorno(_scadenzaInizio.AddMonths(1))
        End If


        _scadenzaInizio = Txt_ValiditaInizio.Text
        _scadenzaFine = Txt_ValiditaFine.Text


        _tipoNumeroDoc = Stringa_Decodifica(CStr(Request.QueryString("tnd")), AgroKey_EncoderDecoder, Server)

        _numeroDoc = Stringa_Decodifica(CStr(Request.QueryString("nd")), AgroKey_EncoderDecoder, Server)

        _annoDoc = Stringa_Decodifica(CStr(Request.QueryString("ad")), AgroKey_EncoderDecoder, Server)

        'riba / anticipo fattura
        _tipo = Stringa_Decodifica(CStr(Request.QueryString("tipo")), AgroKey_EncoderDecoder, Server)

        '--------------------------------
        'IMPRESA
        _codIstitutoDare = Stringa_Decodifica(CStr(Request.QueryString("cid")), AgroKey_EncoderDecoder, Server)

        _codLiquiditaDare = Stringa_Decodifica(CStr(Request.QueryString("cld")), AgroKey_EncoderDecoder, Server)
        '--------------------------------


        '--------------------------------
        'CONTATTO
        _codIstitutoAvere = Stringa_Decodifica(CStr(Request.QueryString("cia")), AgroKey_EncoderDecoder, Server)

        _codLiquiditaAvere = Stringa_Decodifica(CStr(Request.QueryString("cla")), AgroKey_EncoderDecoder, Server)
        '--------------------------------

        _flagPagamentoNonAvvenuto = Stringa_Decodifica(CStr(Request.QueryString("flapag")), AgroKey_EncoderDecoder, Server)

        _dataPagamento = Stringa_Decodifica(CStr(Request.QueryString("dp")), AgroKey_EncoderDecoder, Server)

    End Sub

#Region "Stampa"

    Protected Sub btnStampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnStampa.Click
        Gestione_Anteprima_Stampa(False, "", False)
    End Sub

    Protected Sub btnPdf_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles BtnPDF.Click
        Gestione_Anteprima_Stampa(False, "", True)
    End Sub

    Private Sub CaricaDT_Riba(ByVal xFiltroP As String)
        Try

            Dim objRIBA As New AgronicaCoreStampeDAL.RegistriContab
            Dim Lav_Cod As Integer = 0
            Dim lCBI_Causale As Integer = 0
            Dim DT_RIBA As DataTable

            lCBI_Causale = _cbiCausale

            DT_RIBA = objRIBA.RegistroRIBA(_piva,
                                           _codRisUm,
                                           _codRapporto,
                                           Lav_Cod,
                                           _codIstitutoDare,
                                           _codIstitutoAvere,
                                           _codLiquiditaDare,
                                           _codLiquiditaAvere,
                                           _dataPagamento,
                                           _tipoScadenza,
                                           _scadenzaInizio,
                                           _scadenzaFine,
                                           _tipoNumeroDoc,
                                           _numeroDoc,
                                           _annoDoc,
                                           _tipo,
                                           lCBI_Causale,
                                           xFiltroP, "",
                                           _objParametriServer)


            gwRiba.DataSource = DT_RIBA
            gwRiba.DataBind()

        Catch ex As Exception
            _logErrori &= "Lettura delle Ri.Ba: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '#######################################################################
    Private Sub Gestione_Anteprima_Stampa(ByVal isAnteprima As Boolean, ByVal xFiltroP As String, ByVal PDF As Boolean)

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        ValorizzaDaParametriRicerca()

        If xFiltroP <> "" Then
            ValorizzaDaParametriRicercaXPagamentiCodSpecificati()
        End If

        If isAnteprima Then

            CaricaDT_Riba(xFiltroP)

            Exit Sub
        Else
            Stampa_RegistroRiBa(xFiltroP, PDF)
        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_RegistroRiBa(ByVal xFiltroP As String, ByVal PDF As Boolean)

        Dim lavCod As Integer = 0
        Dim dt As DataTable
        Dim dsRiba As New DS_RegistroRiBa

        Try

            Dim objRiba As New AgronicaCoreStampeDAL.RegistriContab

            Dim cbiCausale As Integer = CBI_Riba_Causali.Nessuno

            If Not IsNothing(Me.Cmb_cbiCausale.SelectedValue) Then
                cbiCausale = Me.Cmb_cbiCausale.SelectedValue
            End If

            dt = objRiba.RegistroRIBA(_piva,
                                      _codRisUm,
                                      _codRapporto,
                                      lavCod,
                                      _codIstitutoDare,
                                      _codIstitutoAvere,
                                      _codLiquiditaDare,
                                      _codLiquiditaAvere,
                                      _dataPagamento,
                                      _tipoScadenza,
                                      _scadenzaInizio,
                                      _scadenzaFine,
                                      _tipoNumeroDoc,
                                      _numeroDoc,
                                      _annoDoc,
                                      _tipo,
                                      cbiCausale,
                                      xFiltroP, "",
                                      _objParametriServer)

            'If isAnteprima Then
            '    Exit Sub
            'End If

            ValorizzazioneRegistroRiba(_piva, dt, dsRiba, _logErrori)

        Catch ex As Exception
            _logErrori &= "Lettura delle Ri.Ba: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '########################################################################

        Select Case _tipo
            Case 0 'riba
                'il titolo è già a posto
            Case 1 'anticipo fatture
                CType(_rptRiba.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Distinta richiesta anticipazioni: Fatture"
        End Select


        Try

            '--------------------------------------------
            ' AGGANCIO DATI
            '--------------------------------------------
            _rptRiba.SetDataSource(dsRiba)

        Catch ex As Exception
            _logErrori &= "- Aggancio dataset al report: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim nomeDocumento As String = "RegistroRiBa"
        Dim identificazioneDocumento As String = ""

        Try

            Dim dataInizioAllegati As Date = _scadenzaInizio
            Dim dataFineAllegati As Date = _scadenzaFine
            identificazioneDocumento &= "_" & Format(_scadenzaInizio, "yyyy_MM_dd") & "_" & Format(_scadenzaFine, "yyyy_MM_dd")

            ' leggo la sottocartella da CategorieDocumenti
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Dim sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.PresentazioneRiba, "", "", _objParametriServer)

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(_rptRiba,
                                       enum_CategorieDocumenti.PresentazioneRiba,
                                       sottocartella,
                                       nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf",
                                       _objParametriServer,
                                       New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                            enum_CategorieDocumenti.LiquidazioneIVA,
                                                                            nomeDocumento,
                                                                            nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf",
                                                                            sottocartella,
                                                                            "", "", "", "",
                                                                            dataInizioAllegati,
                                                                            dataFineAllegati,
                                                                            _objParametriServer)



        Catch ex As Exception
            _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
        End Try

        'Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
        Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
        Try
            _rptRiba.SaveAs(reportTemporano, True)
        Catch ex As Exception
            _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
        End Try


        'Dispose dei dataset e del report per evitare problema deallocazione.
        dsRiba.Dispose()
        dsRiba = Nothing

        _rptRiba.Close()
        _rptRiba.Dispose()
        _rptRiba = Nothing

        GC.Collect()

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim infoInLog As String = "Registro_RiBa, Partita Iva = " & CStr(_piva) &
                                  ", Cod_Istituto_DARE = " & CStr(_codIstitutoDare) & ", Cod_Istituto_AVERE = " & CStr(_codIstitutoAvere) &
                                  ", Cod_Liquidita_DARE = " & CStr(_codLiquiditaDare) & ", Cod_Liquidita_AVERE = " & CStr(_codLiquiditaAvere) &
                                  ", Codice Risorsa Umana = " & CStr(_codRisUm) & ", Codice Rapporto Contabile = " & CStr(_codRapporto) &
                                  ", ScadenzaInizio = " & CStr(_scadenzaInizio) &
                                  ", ScadenzaFine = " & CStr(_scadenzaFine)
        SalvaLogErrori_Generico(_logErrori, nomeDocumento, identificazioneDocumento, "Registro_RiBa.aspx", "Stampe_Contabilita", infoInLog, _objParametriServer)
        '-----------------------------------------

        'Stefano - 27/1/2018 apertura diretta PDF
        If PDF Then
            Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
        Else
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                "&tmpReportPath=" & Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
        End If


        'Try
        '    Session("Report") = rptRIBA

        '    'Dim TargetURL As String = "..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server)

        '    Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        '    'Page_NewWindow(Page, TargetURL, "", , , , , , , , , , True)


        'Catch ex As Exception
        '    Log_Errori &= "- Export: " & vbCrLf & ex.Message & vbCrLf
        'End Try


        '########################################################################


    End Sub


#End Region


#Region "Carica Controlli"

    Private Sub ValoriCombo()
        Txt_ValiditaInizio.Text = _scadenzaInizio
        Txt_ValiditaFine.Text = _scadenzaFine
        txtNumeroDocumento.Text = _numeroDoc
    End Sub


    Private Sub PopolaIstCredito()

        Me.Cmb_IstitutoCredito.Items.Clear()

        AgronicaCoreUtility.CaricaListControl.Ist_Credito(Me.Cmb_IstitutoCredito,
                                                          True, "", "",
                                                          _piva,
                                                          _piva,
                                                          "", "",
                                                          _objParametriServer)
    End Sub

    Private Sub PopolaAnnoContabile()

        Me.Cmb_AnnoContabile.Items.Clear()

        'Carica gli anni presenti nella tabella RicxConti per quella partita iva
        AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile,
                                                                          False, "", "",
                                                                          _piva,
                                                                          0, 0,
                                                                          "", " anno desc",
                                                                          _objParametriServer)
    End Sub

    Private Sub PopolaCBI_Causale()

        Me.Cmb_cbiCausale.Items.Clear()

        'Carica gli anni presenti nella tabella RicxConti per quella partita iva
        AgronicaCoreUtility.CaricaListControl.CBI_Causale(True, "Nessun Filtro", CBI_Riba_Causali.Nessuno,
                                                          Me.Cmb_cbiCausale,
                                                          "", "",
                                                          _objParametriServer)

    End Sub

#End Region

#Region "Export"

    '############################################
    Protected Sub btnEsportaCBI_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnEsportaCBI.Click

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
        Dim utenteAbilitato As Boolean
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        utenteAbilitato = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                Session("ASG_IdServizio"),
                                                                enum_Security_Attivita.Esportazione_RIBA_CBI,
                                                                enum_Security_Operazione.Lettura,
                                                                Date.Now, "",
                                                                _objParametriUtenti)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... 
        If utenteAbilitato = False Then
            'Messaggio = "Non e' consentito inserire un elemento gia' presente"
            AgronicaCoreUtility.Messaggi.AgroMsgBox("Il modulo di esportazione Riba CBI non è attivo. Contattatare il supporto commerciale di Agronica per maggiori dettagli o richiederne l'attivazione.", Page, , )
            'AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il modulo di esportazione Riba CBI non è attivo. Contattatare il supporto commerciale di Agronica per maggiori dettagli o richiederne l'attivazione.", Page)
            Exit Sub
        End If

        If Me.Cmb_IstitutoCredito.SelectedItem.Text = "" Then
            AgronicaCoreUtility.Messaggi.AgroMsgBox("E' necessario selezionare l'istituto di credito.", Page, , )
            Exit Sub
        End If

        'maga:
        'commento il controllo sullo stato delle riba
        'perchè nella cella 2 risulta stringa vuota in debug
        ' Dim AlmenoUnoNonCorretto As Boolean = False
        Dim AlmenoUnoSelezionato As Boolean = False
        ' Dim statoRiba As CBI_Riba_Causali

        For i = 0 To Me.gwRiba.Rows.Count - 1

            If CType(Me.gwRiba.Rows(i).FindControl("ckDettaglio"), CheckBox).Checked = True Then
                AlmenoUnoSelezionato = True

                'statoRiba = Me.gwRiba.Rows(i).Cells(2).Text

                'If statoRiba <> CBI_Riba_Causali.NonaAncoraEsportata Then
                '    AlmenoUnoNonCorretto = True
                '    Exit Sub
                'End If

            End If

        Next

        If AlmenoUnoSelezionato = False Then
            AgronicaCoreUtility.Messaggi.AgroMsgBox("E' necessario selezionare almeno un documento da esportare.", Page, , )
            Exit Sub
        End If

        'If AlmenoUnoNonCorretto = True Then
        '    AgronicaCoreUtility.Messaggi.AgroMsgBox("I documenti selezionati per l'esportazione CBI devono essere nello stato 'Non ancora esportata in CBI'.", Page, , )
        '    Exit Sub
        'End If

        CBI_Esporta()

    End Sub

    Private Sub CBI_Esporta()

        Dim FFIle As String
        Dim CBI_InOut As New CBI_CorporateBankingInterbancario.Esportazione_CBI_CorporateBankingInterbancario
        Dim xfiltroP As String = ""
        Dim xPaga As String = ""
        Dim listaEffetti As New List(Of CBI_CorporateBankingInterbancario.CBI_DisposizioneIncassoRiBa)

        Try

            If RibaUp.HasFile Then

                Dim tmpPath As String = ""
                Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                tmpPath = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(objWebConfig.GestioneImportazioni_Repository) & "ImportazioneCbi\"

                If Not My.Computer.FileSystem.DirectoryExists(tmpPath) Then
                    My.Computer.FileSystem.CreateDirectory(tmpPath)
                End If

                Dim nomefile_zip = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) 'nome temporaneo ed estensione univoco per un file

                Dim nomeFileZiptoPass = tmpPath & nomefile_zip & "." & RibaUp.PostedFile.FileName.Split(".")(1)
                RibaUp.PostedFile.SaveAs(nomeFileZiptoPass)


                listaEffetti = CBI_InOut.Importa(nomeFileZiptoPass, _objParametriServer)


                For Each lEF In listaEffetti
                    For Each lRiba In lEF.CBI_RiBa
                        xPaga &= lRiba.CBI_51.numero_ricevuta & ","
                    Next
                Next

                xPaga = xPaga.TrimEnd(",")

            Else
                xPaga = GetIdPagamentiSelezionati()
            End If


            If xPaga <> "" Then
                xfiltroP = " Pagamenti.COD_Pagamento in (" & xPaga & " )"
            Else
                lblEsitoRiba.Text = "Nessun risultato."
                Exit Sub
            End If


            If Not RibaUp.HasFile Then

                FFIle = CBI_InOut.Esporta(_piva,
                                          _codRisUm,
                                          _codRapporto,
                                          0,
                                          _codIstitutoDare,
                                          0,
                                          0,
                                          0,
                                          AGRODATAFINE,
                                          _tipoScadenza,
                                          _scadenzaInizio,
                                          _scadenzaFine,
                                          _tipoNumeroDoc,
                                          _numeroDoc,
                                          _annoDoc,
                                          0,
                                          xfiltroP,
                                          "",
                                          _objParametriServer,
                                          Nothing)

                Dim app As String() = FFIle.Replace(".zip", "").Split("|")
                Dim vAppNomeCartella As String() = app(0).Split("\")
                Dim appNomeCartella As String = vAppNomeCartella(vAppNomeCartella.Length - 1).Replace(".zip", "")

                zfileToSaveTo.Value = FFIle

                btnApriFile.Visible = True

                'lnkApriCartella.Text = "Click qui"
                'lnkApriCartella.Target = "__blank"
                'lnkApriCartella.NavigateUrl = "file:///" & app(0)


                lblEsitoRiba.Text = "Flussi memorizzati nella cartella: " & appNomeCartella

                Gestione_Anteprima_Stampa(True, "", False)

            Else

                Gestione_Anteprima_Stampa(True, xfiltroP, False)

                ImpostaGridViewDataListaEffetti(listaEffetti)
                btnConferma.Visible = True
                lblConferma.Visible = True

            End If

        Catch ex As Exception
            lblEsitoRiba.Text = "ERRORE " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try
    End Sub

    Public Sub ImpostaGridViewDataListaEffetti(ByVal listaEffetti As List(Of CBI_CorporateBankingInterbancario.CBI_DisposizioneIncassoRiBa))

        Dim lID_agenda As String = ""
        Dim rval As String = ""

        Dim descrizioneBreve As String
        Dim colore As String

        For Each ll In listaEffetti
            For Each lRIBA In ll.CBI_RiBa

                For Each dtDet As GridViewRow In gwRiba.Rows
                    descrizioneBreve = ""
                    colore = ""
                    DescrizioneColoreDaCodiceCausale(lRIBA.CBI_14.causale, descrizioneBreve, colore)
                    lID_agenda = dtDet.Cells(1).Text
                    If lID_agenda = lRIBA.CBI_51.numero_ricevuta Then
                        dtDet.Cells(3).Text = lRIBA.CBI_14.causale
                        dtDet.Cells(4).Text = descrizioneBreve
                        dtDet.Cells(5).Text = lRIBA.CBI_14.data_pagamento
                    End If

                Next
            Next
        Next

    End Sub

    Private Sub DescrizioneColoreDaCodiceCausale(ByVal codiceCausale As Integer, ByRef descrizioneBreve As String, ByRef colore As String)
        Dim leggi As New AgronicaCoreMetaSchemaDAL.CBI_Causali_R

        Dim dt As DataTable = leggi.Leggi(codiceCausale, "", "", _objParametriServer)

        If dt.Rows.Count > 0 Then
            descrizioneBreve = dt(0)("CBI_Causali_DesBreve")
            colore = dt(0)("Colore")
        End If

    End Sub


    Private Function GetIdPagamentiSelezionati() As String

        Dim lID_agenda As String = ""
        Dim rval As String = ""
        For Each dtDet As GridViewRow In gwRiba.Rows

            lID_agenda = dtDet.Cells(1).Text
            If CType(dtDet.FindControl("ckDettaglio"), CheckBox).Checked() = True AndAlso
               Not String.IsNullOrEmpty(lID_agenda) AndAlso
               lID_agenda <> "0" AndAlso
               Not lID_agenda.StartsWith("&") AndAlso
               Not lID_agenda.StartsWith(" ") Then

                rval &= lID_agenda & ","
            End If
        Next

        Return rval.TrimEnd(",")

    End Function


    Protected Sub btnLeggiCBI_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnLeggiCBI.Click
        CBI_Leggi()
    End Sub

    Private Sub CBI_Leggi()
        If RibaUp.HasFile Then
            CBI_Esporta()
            lblEsitoRiba.Text = "Il file contiene i seguenti dati:"
        Else
            lblEsitoRiba.Text = "Nessun file selezionato."
        End If

    End Sub

    '############################################
    Protected Sub btnAnnullaPresentazione_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnAnnullaPresentazione.Click
        CBI_Annulla_Presentazione()
    End Sub

    Private Sub CBI_Annulla_Presentazione()

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
        Dim utenteAbilitato As Boolean
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        utenteAbilitato = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                Session("ASG_IdServizio"),
                                                                enum_Security_Attivita.Esportazione_RIBA_CBI,
                                                                enum_Security_Operazione.Lettura,
                                                                Date.Now, "",
                                                                _objParametriUtenti)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio ad AlberoImprese.
        If utenteAbilitato = False Then
            ' AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il modulo di esportazione Riba CBI non è attivo. Contattatare il supporto commerciale di Agronica per maggiori dettagli o richiederne l'attivazione.", Page)
            AgronicaCoreUtility.Messaggi.AgroMsgBox("Il modulo di esportazione Riba CBI non è attivo. Contattatare il supporto commerciale di Agronica per maggiori dettagli o richiederne l'attivazione.", Page, , )
            Exit Sub
        End If

        Dim elementiSelezionati As String
        elementiSelezionati = GetIdPagamentiSelezionati()

        Dim sbloccatore As New AgronicaCoreContabDAL.Pagamenti_W

        If elementiSelezionati <> "" Then
            For Each elem In elementiSelezionati.Split(",")
                sbloccatore.CBI_RiBa_EliminaMarcaInFaseDiInvio(elem, _objParametriServer)
            Next
        End If

        lblEsitoRiba.Text = "Operazione eseguita."
        Gestione_Anteprima_Stampa(True, "", False)

    End Sub

    '####################################################
    Protected Sub btnRefreshFiltri_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnRefreshFiltri.Click
        RefreshFiltri()
    End Sub

    Private Sub RefreshFiltri()
        Gestione_Anteprima_Stampa(True, "", False)
        lblEsitoRiba.Text = "Risultato del filtro:"
    End Sub

    Protected Sub btnConferma_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnConferma.Click
        CBI_Conferma()
    End Sub

    Private Sub CBI_Conferma()

        Dim traccTmp As New CBI_CorporateBankingInterbancario.TracciatoDatiCBI_Riba
        Dim cc As New CBI_CorporateBankingInterbancario.CBI_DisposizioneIncassoRiBa

        Dim lID_agenda As String = ""

        For Each dtDet As GridViewRow In gwRiba.Rows

            lID_agenda = dtDet.Cells(1).Text
            If CType(dtDet.FindControl("ckDettaglio"), CheckBox).Checked() = True AndAlso
               Not String.IsNullOrEmpty(lID_agenda) AndAlso
               lID_agenda <> "0" AndAlso
               Not lID_agenda.StartsWith("&") AndAlso
               Not lID_agenda.StartsWith(" ") Then

                Dim tRiba As New CBI_CorporateBankingInterbancario.CBI_Riba
                tRiba.CBI_14.data_pagamento = Date.Parse(dtDet.Cells(5).Text)
                tRiba.CBI_14.causale = dtDet.Cells(3).Text
                tRiba.CBI_51.numero_ricevuta = dtDet.Cells(1).Text
                cc.CBI_RiBa.Add(tRiba)

            End If
        Next

        Dim rval As Boolean = traccTmp.CBI_DisposizioneIncassoRiBa_Salva(cc, _objParametriServer)

        If rval Then
            lblEsitoRiba.Text = "Operazione completata con successo."
        Else
            lblEsitoRiba.Text = "Si sono verificare degli errori durante la procedura. Alcuni elementi non sono stati processati correttamente."
        End If

    End Sub

#End Region

    Private Sub ValorizzaDaParametriRicercaXPagamentiCodSpecificati()
        _tipoScadenza = 0
        _tipoNumeroDoc = 0
        _codIstitutoDare = 0
        _codIstitutoAvere = 0
    End Sub


    Private Sub ValorizzaDaParametriRicerca()
        ValorizzaDaQueryString()

        _scadenzaInizio = Txt_ValiditaInizio.Text
        If Txt_ValiditaFine.Text = "" Then
            _scadenzaFine = AGRODATAFINE
        Else
            _scadenzaFine = Txt_ValiditaFine.Text
        End If


        _numeroDoc = txtNumeroDocumento.Text
        _tipoNumeroDoc = cmbNDoc.SelectedValue
        _tipoScadenza = 6 'cmbScadenza.SelectedValue
        If _numeroDoc <> 0 Then
            _annoDoc = Cmb_AnnoContabile.SelectedValue
        End If


        If Me.Cmb_IstitutoCredito.SelectedIndex > 0 Then
            _codIstitutoDare = Me.Cmb_IstitutoCredito.SelectedValue
        Else
            _codIstitutoDare = 0
        End If

        _cbiCausale = Me.Cmb_cbiCausale.SelectedValue

    End Sub


    Protected Sub btnApriFile_Click(sender As Object, e As EventArgs) Handles btnApriFile.Click

        Dim name As String = ""

        Dim zapp As String() = zfileToSaveTo.Value.Split("|")
        Dim zapp1 As String() = zapp(0).Split("\")

        Dim file As FileInfo = New FileInfo(zapp(0))

        Response.Clear()
        Response.ClearContent()
        Response.ClearHeaders()
        Response.Buffer = True
        Response.AppendHeader("content-disposition", "attachment; filename=" & name)
        Response.AppendHeader("content-length", file.Length.ToString())
        Response.ContentType = "application/zip"
        Response.TransmitFile(zapp(0))
        Response.Flush()
        Response.End()

    End Sub

End Class