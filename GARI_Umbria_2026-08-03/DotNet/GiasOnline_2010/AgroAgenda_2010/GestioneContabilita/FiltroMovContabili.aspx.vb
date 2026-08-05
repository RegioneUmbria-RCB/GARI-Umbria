Imports System.Data.Common
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility

Public Class FiltroMovContabili
    Inherits System.Web.UI.Page
    Const COL_DATA As Integer = 1
    Const COL_DOC_NUMERO_SIN As Integer = 2
    Const COL_DOC_NUMERO As Integer = 3
    Const COL_DOC_NUMERO_DES As Integer = 4
    Const COL_DOC_NUMERO_COMPLETO As Integer = 5
    Const COL_LAV_COD As Integer = 6
    Const COL_CAUSALE As Integer = 7
    Const COL_DESCRIZIONE As Integer = 8
    Const COL_SCADENZA As Integer = 9
    Const COL_SALDATO As Integer = 10
    Const COL_COD_RISUM As Integer = 11
    Const COL_COD_CONTATTO As Integer = 12
    Const COL_CONTATTO As Integer = 13
    Const COL_ANNO As Integer = 14
    Const COL_RIC_COD As Integer = 15
    Const COL_COD_CONTO As Integer = 16
    Const COL_CONTO As Integer = 17
    Const COL_QTA As Integer = 18
    Const COL_PREZZO_UNITARIO As Integer = 19
    Const COL_PREZZO_UNITARIO_NETTO As Integer = 20
    Const COL_SCONTO As Integer = 21
    Const COL_IMPONIBILE As Integer = 22
    Const COL_COD_IVA As Integer = 23
    Const COL_ALIQUOTA As Integer = 24
    Const COL_IMPOSTA As Integer = 25
    Const COL_IMPORTO As Integer = 26
    Const COL_BLOCCO_FLAG As Integer = 27
    Const COL_BLOCCO_DATA As Integer = 28
    Const COL_BLOCCO_USERNAME As Integer = 29
    Const COL_ID_AGENDA As Integer = 30
    Const COL_XML_MOVDETTAGLI As Integer = 31
    Const COL_STATO_EXPORT As Integer = 32

    Const ID_TOOL_TROVA As Integer = 0
    Const ID_TOOL_AZZERA As Integer = 1
    '2 separatore
    Const ID_TOOL_SELEZIONA As Integer = 3
    Const ID_TOOL_DESELEZIONA As Integer = 4
    '5 separatore
    Const ID_TOOL_INFO As Integer = 6
    Const ID_TOOL_RIFERIMENTI As Integer = 7
    Const ID_TOOL_STAMPA As Integer = 8
    '9 separatore
    Const ID_TOOL_ALLEGA As Integer = 10
    '11 separatore
    Const ID_TOOL_BLOCCO As Integer = 12
    Const ID_TOOL_SBLOCCO As Integer = 13
    '14 separatore
    Const ID_TOOL_IMPRESA As Integer = 15
    '16 separatore

    Const NomeFileLog_AgronicaCore As String = "Log_AgronicaCore_BloccoSblocco.txt"


    '----- Gestione Querystring
    Dim Qs_Piva, Qs_Rag_Soc, Qs_Modalita As String
    Dim Qs_AnnoContabile, Qs_DataSelezionata As String
    Dim Qs_Cod_RisUm, Qs_Filtro, Qs_PagRitorno As String
    Dim Qs_TipoMov, Qs_LavCod, Qs_TipoSblocco As Integer

    Dim x_Validita_Inizio As String
    Dim x_Validita_Fine As String

    Dim x_Scadenza As String
    Dim x_DocNumero_Sin As String
    Dim x_DocNumero_Des As String
    Dim x_DocNumero As Integer

    Dim x_Lav_Cod As Integer
    Dim x_Tipo As Integer
    Dim x_Blocco_Flag As Integer

    Dim x_CodRapporto As Integer
    Dim x_CodRisUm As Integer

    Dim x_AnnoCont As Integer
    Dim x_Ric_Cod As Integer
    Dim x_Cod_Conto As Integer

    Dim Flag_VisualizzaDocTrasporto As Boolean = True

    'Dim x_Flag_Join_CodConto As Boolean

    Dim objParametri_Server As AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreParametri

    '##########################################################################################
    '
    'MODALITA' DI UTILIZZO DELLA PAGINA:
    '
    '1) FILTRO = enum_TipoModalita.ModRicerca
    '   A) filtro movimenti economici, pagamenti, bolle con varie funzionalità: info, modifica, stampa, info allegati, blocco, sblocco, ...
    '   b) Sblocco Fruttagel: sblocco solo di una bolla alla volta (filtro sul numero documento obbligatorio)
    '
    '2) ASSOCIAZIONE DI MOVIMENTI AD ALTRI = enum_TipoModalita.ModAssociazione
    '   A) aggancio di bolle a fatture -> gestito
    '   B) aggancio di bolle/fatture a semina/trapianto -> non gestito
    '
    '##########################################################################################



    '###############################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not IsNothing(Request.QueryString("dialog")) Then
            Dim strClose As String = "<script language='javascript'>$(document).ready(function() { parent.chiudidialog(); }); </script>"

            Me.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        Else

            'PremutoAnnulla = True

            'AAA_GestioneUscitaPagina()

        End If



    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()


        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto


    End Sub

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Public Master_Operazione As Agenda



    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
        'IMPOSTA IL NUMERO DI MINUTI DOPO I QUALI
        'LA PAGINA MEMORIZZATA NELLA CACHE SCADE

        Response.Expires = 0

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        Dim UtenteAbilitato As Boolean


        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            If Session("ASG_Utente_Username") = "" Then
                Response.Redirect("~/Custom500.aspx")
            End If

            'inizializzazione oggetti objParametri_Utenti e objParametri_Server
            '---
            objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

            '##############################################################
            '#####  Recupero le informazioni dal DB  ######################
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

            If Not IsNothing(Request.QueryString("rs")) Then

                Qs_Rag_Soc = Stringa_Decodifica(Request.QueryString("rs").ToString,
                               AgroKey_EncoderDecoder,
                               Server)


            Else
                Qs_Rag_Soc = RagSoc_from_Piva(objParametri_Server, Session, Page, Qs_Piva)
            End If

            'enum_TipoModalita
            Qs_Modalita = Stringa_Decodifica(Request.QueryString("mode").ToString,
                           AgroKey_EncoderDecoder,
                           Server)

            Qs_AnnoContabile = Stringa_Decodifica(Request.QueryString("a").ToString,
                                     AgroKey_EncoderDecoder,
                                     Server)

            Qs_DataSelezionata = Stringa_Decodifica(Request.QueryString("d").ToString,
                                AgroKey_EncoderDecoder,
                                Server)

            If Not IsNothing(Request.QueryString("cod_ru")) Then

                Qs_Cod_RisUm = Stringa_Decodifica(Request.QueryString("cod_ru").ToString,
                              AgroKey_EncoderDecoder,
                              Server)

            Else
                Qs_Cod_RisUm = "0"
            End If

            'boh
            If Not IsNothing(Request.QueryString("fil")) Then

                Qs_Filtro = Stringa_Decodifica(Request.QueryString("fil").ToString,
                              AgroKey_EncoderDecoder,
                              Server)

            Else
                Qs_Filtro = ""
            End If

            'pagina chiamante --> per gestire il tipo di uscita dalla pagina:
            'fare un redirect o chiuderla (perchè aperta in modal dialog)
            'valore= enum_PagineGiasOnline
            If Not IsNothing(Request.QueryString("orig")) Then

                Qs_PagRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString,
                              AgroKey_EncoderDecoder,
                              Server)

            Else
                Qs_PagRitorno = CStr(0)
            End If

            'tipo movimento: enum_TipoMovimentoContabile
            If Not IsNothing(Request.QueryString("tm")) Then

                Qs_TipoMov = Stringa_Decodifica(Request.QueryString("tm").ToString,
                              AgroKey_EncoderDecoder,
                              Server)

            Else
                Qs_TipoMov = 1
            End If

            'il lav_cod è il tipo causale!
            If Not IsNothing(Request.QueryString("lc")) Then

                Qs_LavCod = Stringa_Decodifica(Request.QueryString("lc").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)

            Else
                Qs_LavCod = 0
            End If

            'tipo di sblocco
            If Not IsNothing(Request.QueryString("tsb")) Then

                Qs_TipoSblocco = Stringa_Decodifica(Request.QueryString("tsb").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)

            Else
                Qs_TipoSblocco = 0
            End If


            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################
            Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            If Not Page.IsPostBack Then
                Session("Unid_Per_Agenda2010") = ""
                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

                '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

                UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                       Session("ASG_IdServizio"),
                                       enum_Security_Attivita.Gest_Contabilita,
                                       enum_Security_Operazione.Lettura,
                                       Now, "", objParametri_Utenti
                                       )


                '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
                If Not UtenteAbilitato Then
                    Dim Url As String
                    Url = PaginaAspx_from_TipoEnumPagina(Qs_PagRitorno, "../") & "?" & CStr(Request.QueryString.ToString)
                    Response.Redirect(Url)
                End If

            Else

                '==========================================
                '===== Pagina ricaricata in POSTBACK
                '==========================================

                Select Case Me.Txt_FiltroConti.Value
                    Case "", "undefined" 'NO
                        If Me.Txt_FiltroConti.Value.ToLower = "undefined" Then
                            Me.Txt_FiltroConti.Value = ""
                        End If
                        'nessun filtro
                        Me.Chk_Conti.Checked = False
                        Configura_Conti(True)
                    Case Else 'SI
                        'imposta filtro sui conti
                        Me.Chk_Conti.Checked = True
                        Configura_Conti(False)
                End Select

                Exit Sub

            End If


            '########################################################################


            'IMPOSTAZIONE PANNELLI

            ImpostaPannelli()

            '===========================================

            'ORDINAMENTO

            CaricaCombo_TipoOrdinamentoQueryContabilita(Server, Session, Page, Me.Cmb_Ordinamento)

            '===========================================

            'PERIODO DI COMPETENZA

            Me.Txt_DataInizio.Text = "01/" & Format(CDate(Qs_DataSelezionata), "MM") & "/" & CStr(CDate(Qs_DataSelezionata).Year)
            Me.Txt_DataFine.Text = CStr(CDate(Qs_DataSelezionata))

            '===========================================

            'MOVIMENTI e CAUSALI

            CaricaCombo_TipoMovimentoContabile(Me.Cmb_TipoMovimento, Flag_VisualizzaDocTrasporto)

            '===========================================

            'CONTATTI

            'rapporti contabili
            CaricaCombo_RapportiContabili_Manuale(Me.Cmb_RappContabili)

            Select Case CInt(Qs_Modalita)

                Case enum_TipoModalita.ModAssociazione

                    'carica solo il contatto passato dalla fattura
                    Dim FiltroAgg As String
                    FiltroAgg = " ( Risorse_Umane.Cod_Risum = " & Agro_SQL_SaveNum(Qs_Cod_RisUm) & " ) "
                    CaricaCombo_Contatti(objParametri_Server, Session, Page,
                                         Me.Cmb_Contatti, Qs_Piva,
                                         CStr(Session("ASG_SuperUser_CodFiscale")),
                                         0, 0, 0, 0, 0,
                                         FiltroAgg,
                                         , , ,
                                         False,
                                         0,
                                         1)

                Case Else

                    'carica TUTTI i contatti
                    'value = cod_risum
                    CaricaCombo_Contatti(objParametri_Server, Session, Page,
                                         Me.Cmb_Contatti, Qs_Piva,
                                         CStr(Session("ASG_SuperUser_CodFiscale")),
                                         0, 0, 0, 0, 0, , , , ,
                                         False,
                                         0,
                                         1)
            End Select



            '===========================================

            'PIANO DEI CONTI

            'Carica gli anni presenti nella tabella RicxConti per quella partita iva
            '  CaricaCombo_AnnoContabile2(Server, Session, Page, Me.Cmb_AnnoContabile, Qs_Piva, , , , True, , )

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile,
                                                                                       True, "", "",
                                                                                       Qs_Piva,
                                                                                       0,
                                                                                       0,
                                                                                       "", "",
                                                                                       objParametri_Server)

            '===========================================

            'MODALITA' PAGINA

            Configura_Modalita()

            '===========================================


        Catch exc As Exception

            Messaggi.AgroMsgBox("Problemi nel caricamento dei filtri disponibili: " & vbCrLf & exc.Message, Page)

        End Try


    End Sub


    '##########################################################################################
    'Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click

    '    Select Case Qs_PagRitorno

    '        Case enum_PagineGiasOnline.MenuContab
    '            Response.Redirect("MenuContabilita.aspx?p=" & Request.QueryString("p").ToString & "&d=" & Request.QueryString("d").ToString)

    '        Case enum_PagineGiasOnline.Fattura

    '            Dim strClose As String = "<script language='javascript'>window.close()</script>"
    '            Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    '        Case enum_PagineGiasOnline.MenuArchivi
    '            Response.Redirect(PaginaAspx_from_TipoEnumPagina(Qs_PagRitorno, "../"))

    '        Case enum_PagineAgenda_2010.Pagina_Semina_Trapianto

    '            Dim Unid_Per_Agenda2010 As String = ""
    '            If Not IsNothing(Session("Unid_Per_Agenda2010")) AndAlso Session("Unid_Per_Agenda2010") <> "" Then
    '                Unid_Per_Agenda2010 = Session("Unid_Per_Agenda2010")
    '            Else

    '            End If


    '            Dim strClose As String = "<script language='javascript'> " & _
    '                                        " window.returnValue = " & Unid_Per_Agenda2010 & "; " & _
    '                                        " window.close(); " & _
    '                                     " </script> "
    '            Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    '    End Select

    'End Sub

    '##########################################################################################
    Private Enum enum_Pannelli

        Pannello_Generale = 1
        Pannello_Filtri = 2
        Pannello_Risultati_MovEco = 3
        Pannello_Risultati_BolleDDT = 4
        Pannello_Movimenti = 5

    End Enum

    '##########################################################################################
    Private Sub ImpostaPannelli()

        'Dim Dimensione As Unit

        'With Me.Pannello_Generale
        '    .Height = Dimensione.Pixel(468)
        '    .Width = Dimensione.Pixel(954)
        '    '.Style.Item("Top") = Top
        '    '.Style.Item("Left") = Left()
        'End With

        'With Me.Pannello_Filtri
        '    .Height = Dimensione.Pixel(164)
        '    .Width = Dimensione.Pixel(930)
        '    '.Style.Item("Top") = Top
        '    '.Style.Item("Left") = Left()
        'End With

        'With Me.Pannello_Risultati_MovEco
        '    .Height = Dimensione.Pixel(35)
        '    .Width = Dimensione.Pixel(930)
        '    .Style.Item("Top") = 168
        '    .Style.Item("Left") = 8
        'End With

        'With Me.Pannello_Risultati_BolleDDT
        '    .Height = Dimensione.Pixel(35)
        '    .Width = Dimensione.Pixel(930)
        '    .Style.Item("Top") = 168
        '    .Style.Item("Left") = 8
        'End With

        'With Me.Pannello_Movimenti
        '    .Height = Dimensione.Pixel(248)
        '    .Width = Dimensione.Pixel(930)
        '    '.Style.Item("Top") = Top
        '    '.Style.Item("Left") = Left()
        'End With


    End Sub

    '##########################################################################################
    Private Sub Imposta_PannelloRiepilogo(ByVal Pannello As enum_Pannelli)

        Me.Pannello_Risultati_BolleDDT.Visible = False
        Me.Pannello_Risultati_MovEco.Visible = False

        If Pannello = enum_Pannelli.Pannello_Risultati_BolleDDT Then
            Me.Pannello_Risultati_BolleDDT.Visible = True
        Else
            Me.Pannello_Risultati_MovEco.Visible = True
        End If

    End Sub


    '##########################################################################################
    'viene chiamata al caricamento della pagina, e ne configura la modalità di utilizzo
    Private Sub Configura_Modalita()

        '=================================================================
        'Configurazione Controlli
        '-----------------------------------------------------------------

        Select Case CInt(Qs_Modalita)

            Case enum_TipoModalita.ModRicerca

                'Me.LblTitolo.Text = "Filtro Movimenti - " & Qs_Rag_Soc

                Me.Cmb_Contatti.Enabled = True
                Me.Cmb_RappContabili.Enabled = True

                Me.Cmb_TipoMovimento.Enabled = True
                Me.Cmb_TipoMovimento.SelectedValue = Qs_TipoMov
                Cmb_TipoMovimento_SelectedIndexChanged(Me, Nothing)

                If Qs_LavCod <> 0 Then
                    Me.Cmb_Causale.SelectedValue = Qs_LavCod
                End If



                Configura_GrigliaMovimenti(enum_TipoModalita.ModRicerca, Me.Cmb_TipoMovimento.SelectedValue)

                Configura_Toolbar(enum_TipoModalita.ModRicerca)

                Attiva_Conti()

                Select Case Qs_TipoMov

                    Case enum_TipoMovimentoContabile.BolleDDT
                        Imposta_PannelloRiepilogo(enum_Pannelli.Pannello_Risultati_BolleDDT)

                    Case enum_TipoMovimentoContabile.MovEconomici,
                        enum_TipoMovimentoContabile.Pagamenti,
                        enum_TipoMovimentoContabile.MovEconomiciPagamenti
                        Imposta_PannelloRiepilogo(enum_Pannelli.Pannello_Risultati_MovEco)

                End Select

                '=====================================================

            Case enum_TipoModalita.ModAssociazione

                'Me.LblTitolo.Text = "Allega Documenti - " & Qs_Rag_Soc

                'saranno bloccati: contatto, tipo movimento, scadenza, conti

                Me.Cmb_Contatti.Enabled = False
                If CInt(Qs_Cod_RisUm) <> 0 Then
                    Me.Cmb_Contatti.SelectedIndex = Me.Cmb_Contatti.Items.IndexOf(Me.Cmb_Contatti.Items.FindByValue(Qs_Cod_RisUm))
                End If
                Me.Cmb_RappContabili.Enabled = False

                Me.Cmb_TipoMovimento.Enabled = False

                'per la semina la sblocco
                If Qs_PagRitorno = enum_PagineAgenda_2010.Pagina_Semina_Trapianto Then
                    Me.Cmb_TipoMovimento.Enabled = True
                End If

                Me.Cmb_TipoMovimento.SelectedValue = 4
                Cmb_TipoMovimento_SelectedIndexChanged(Me, Nothing)

                Select Case Qs_LavCod
                    Case LAVCOD_FATTURA_EMESSA
                        Me.Cmb_Causale.SelectedIndex = Me.Cmb_Causale.Items.IndexOf(
                                                    Me.Cmb_Causale.Items.FindByValue(LAVCOD_BOLLA_EMESSA))
                    Case LAVCOD_FATTURA_RICEVUTA
                        Me.Cmb_Causale.SelectedIndex = Me.Cmb_Causale.Items.IndexOf(
                                                    Me.Cmb_Causale.Items.FindByValue(LAVCOD_BOLLA_RICEVUTA))
                End Select


                Configura_GrigliaMovimenti(enum_TipoModalita.ModAssociazione, Me.Cmb_TipoMovimento.SelectedValue)

                Configura_Toolbar(enum_TipoModalita.ModAssociazione)

                Disattiva_Conti()

                Imposta_PannelloRiepilogo(enum_Pannelli.Pannello_Risultati_BolleDDT)

                'saranno invece selezionabili: causale, intervallo temporale, num documento, causale trasporto.

        End Select


    End Sub


    '##############################################################
    Private Sub Configura_GrigliaMovimenti(ByVal Mode As enum_TipoModalita,
                                            ByVal TipoMovimento As Integer)

        Select Case Mode

            Case enum_TipoModalita.ModRicerca 'filtro

                Select Case TipoMovimento

                    'Case 0 'Mov Economici e Pagamenti
                    'Case 1 'Movimenti Economici
                    'Case 2 'Pagamenti
                    Case enum_TipoMovimentoContabile.MovEconomiciPagamenti,
                            enum_TipoMovimentoContabile.MovEconomici,
                                enum_TipoMovimentoContabile.Pagamenti

                        Me.DataGrid_Movimenti.Columns.Item(COL_CONTO).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_ALIQUOTA).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_IMPOSTA).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_IMPORTO).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_SCADENZA).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_SALDATO).Visible = True

                        Me.DataGrid_Movimenti.Columns.Item(COL_QTA).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_PREZZO_UNITARIO).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_PREZZO_UNITARIO_NETTO).Visible = False

                        Imposta_PannelloRiepilogo(enum_Pannelli.Pannello_Risultati_MovEco)

                    Case enum_TipoMovimentoContabile.MovFinanziari 'Movimenti Finanziari

                    Case enum_TipoMovimentoContabile.BolleDDT 'Bolle e DDT

                        Me.DataGrid_Movimenti.Columns.Item(COL_CONTO).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_ALIQUOTA).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_IMPOSTA).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_IMPORTO).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_SCADENZA).Visible = False
                        Me.DataGrid_Movimenti.Columns.Item(COL_SALDATO).Visible = False

                        Me.DataGrid_Movimenti.Columns.Item(COL_QTA).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_PREZZO_UNITARIO).Visible = True
                        Me.DataGrid_Movimenti.Columns.Item(COL_PREZZO_UNITARIO_NETTO).Visible = True

                        Imposta_PannelloRiepilogo(enum_Pannelli.Pannello_Risultati_BolleDDT)

                End Select

                If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
                    Me.DataGrid_Movimenti.Columns.Item(COL_STATO_EXPORT).Visible = True
                End If

                '---------------------------------

            Case enum_TipoModalita.ModAssociazione 'aggancia bolle a fatture

                Me.DataGrid_Movimenti.Columns.Item(COL_CONTO).Visible = False
                Me.DataGrid_Movimenti.Columns.Item(COL_ALIQUOTA).Visible = False
                Me.DataGrid_Movimenti.Columns.Item(COL_IMPOSTA).Visible = False
                Me.DataGrid_Movimenti.Columns.Item(COL_IMPORTO).Visible = False
                Me.DataGrid_Movimenti.Columns.Item(COL_SCADENZA).Visible = False
                Me.DataGrid_Movimenti.Columns.Item(COL_SALDATO).Visible = False

                Me.DataGrid_Movimenti.Columns.Item(COL_QTA).Visible = True
                Me.DataGrid_Movimenti.Columns.Item(COL_PREZZO_UNITARIO).Visible = True
                Me.DataGrid_Movimenti.Columns.Item(COL_PREZZO_UNITARIO_NETTO).Visible = True

                Imposta_PannelloRiepilogo(enum_Pannelli.Pannello_Risultati_BolleDDT)

        End Select


        Me.DataGrid_Movimenti.Columns.Item(COL_BLOCCO_FLAG).Visible = True
        Me.DataGrid_Movimenti.Columns.Item(COL_BLOCCO_DATA).Visible = True

        'Select Case Me.Rbl_BloccoSblocco.SelectedValue

        '    Case 0 'solo sbloccati
        '        Me.DataGrid_Movimenti.Columns.Item(COL_BLOCCO_FLAG).Visible = False
        '        Me.DataGrid_Movimenti.Columns.Item(COL_BLOCCO_DATA).Visible = False

        '    Case Else 'bloccati o tutti
        '        Me.DataGrid_Movimenti.Columns.Item(COL_BLOCCO_FLAG).Visible = True
        '        Me.DataGrid_Movimenti.Columns.Item(COL_BLOCCO_DATA).Visible = True

        'End Select



    End Sub


    '##############################################################
    Private Sub Configura_Toolbar(ByVal Mode As enum_TipoModalita)

        Select Case Mode

            Case enum_TipoModalita.ModRicerca 'filtro

                ID_Azzera.Enabled = True

                ID_Allega.Enabled = False

                ID_Blocca.Enabled = True
                ID_Sblocca.Enabled = True

                ID_Impresa.Enabled = True


            Case enum_TipoModalita.ModAssociazione 'aggancia bolle a fatture

                ID_Azzera.Enabled = False

                ID_Allega.Enabled = True

                ID_Blocca.Enabled = False
                ID_Sblocca.Enabled = False

                ID_Impresa.Enabled = False


        End Select

    End Sub


    '##########################################################################################
    Public Sub Combo_RapportiContabili_Manuale_Evento(ByRef objServer As System.Web.HttpServerUtility,
                                                        ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                        ByRef objPage As System.Web.UI.Page,
                                                        ByRef Cmb_contatti As System.Web.UI.WebControls.DropDownList,
                                                        ByVal Piva As String,
                                                        ByVal Cod_Rapporto As Integer)



        Select Case Cod_Rapporto

            Case 0 'tutti

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 0, , , , , False, , 1)


            Case COD_CLIENTE

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     1, 0, 0, 0, 0, , , , , False, , 1)

            Case COD_FORNITORE

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 1, 0, 0, 0, , , , , False, , 1)

            Case COD_CLIENTE_FORNITORE

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Fornitore = 1)", , , , False, , 1)

            Case COD_DIPENDENTE

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 1, 0, 0, , , , , False, , 1)

            Case COD_TERZISTA

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 1, 0, , , , , False, , 1)

            Case COD_DIPENDENTE_TERZISTA

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Dipendente = 1 OR Rapporti_Contabili.Terzista = 1)", , , , False, , 1)

            Case COD_TECNICO

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = -6)", , , , False, , 1)

            Case COD_CENTRO_MACCHINE

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = -7 )", , , , False, , 1)

            Case COD_LAB_ANALISI

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = -8 )", , , , False, , 1)

            Case COD_LEGALE

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                     Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                     0, 0, 0, 0, 1, , , , , False, , 1)

            Case Is > 0

                CaricaCombo_Contatti(objParametri_Server, objSession, objPage,
                                  Cmb_contatti, Piva, CStr(objSession("ASG_SuperUser_CodFiscale")),
                                  0, 0, 0, 0, 0, " AND (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto) & " )", , , , False, , 1)


        End Select



    End Sub


    '##########################################################################################
    Private Sub Cmb_TipoMovimento_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Cmb_TipoMovimento.SelectedIndexChanged

        CaricaCombo_TipoCausale(Me.Cmb_Causale, Me.Cmb_TipoMovimento.SelectedValue)


    End Sub


    '##########################################################################################
    Private Sub Toolbar_Azzera_Filtro()

        'toglie il check dagli eventuali movimenti selezionati
        Deseleziona_Tutto()

        x_Validita_Inizio = ""
        x_Validita_Fine = ""

        x_Scadenza = ""
        x_DocNumero = 0
        x_DocNumero_Sin = ""
        x_DocNumero_Des = ""

        x_Lav_Cod = 0
        x_Tipo = 0

        x_CodRapporto = 0
        x_CodRisUm = 0

        x_AnnoCont = 0
        x_Ric_Cod = 0
        x_Cod_Conto = 0

        Me.Txt_Imponibile.Text = ""
        Me.Txt_Importo.Text = ""
        Me.Txt_Imposta.Text = ""
        Me.Txt_NumMovimenti.Text = ""
        Me.Txt_Scadenza.Text = ""
        Me.Txt_DocNumero_Sin.Text = ""
        Me.Txt_DocNumero.Text = ""
        Me.Txt_DocNumero_Des.Text = ""

        '===========================================

        ImpostaPannelli()

        '===========================================

        'PERIODO DI COMPETENZA

        Me.Txt_DataInizio.Text = "01/" & Format(CDate(Qs_DataSelezionata), "MM") & "/" & CStr(CDate(Qs_DataSelezionata).Year)
        Me.Txt_DataFine.Text = CStr(CDate(Qs_DataSelezionata))

        '===========================================

        'MOVIMENTI E CAUSALI

        Me.Cmb_TipoMovimento.SelectedValue = Qs_TipoMov
        Cmb_TipoMovimento_SelectedIndexChanged(Me, Nothing)

        Me.Cmb_Causale.SelectedIndex = 0

        '===========================================

        'TIPO ORDINAMENTO

        Me.Cmb_Ordinamento.SelectedValue = 0

        '===========================================

        'MOVIMENTI BLOCCATI/SBLOCCATI

        Me.Rbl_BloccoSblocco.SelectedIndex = 0

        '===========================================

        'CONTATTI

        'rapporti contabili
        Me.Cmb_RappContabili.SelectedIndex = 0

        'carica TUTTI i contatti
        CaricaCombo_Contatti(objParametri_Server, Session, Page,
                             Me.Cmb_Contatti, Qs_Piva,
                             CStr(Session("ASG_SuperUser_CodFiscale")),
                             0, 0, 0, 0, 0, , , , ,
                             False,
                             0,
                             1)

        Me.Cmb_Contatti.SelectedIndex = 0

        '===========================================

        'PIANO DEI CONTI

        'anni contabili
        Me.Cmb_AnnoContabile.SelectedIndex = Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Qs_AnnoContabile))

        'riclassificazioni 
        Me.Cmb_Riclassificazione.SelectedIndex = Me.Cmb_Riclassificazione.Items.IndexOf(Me.Cmb_Riclassificazione.Items.FindByValue(BILANCIO_PERSONALIZZATO))

        'conti
        Me.Cmb_Conti.SelectedIndex = 0


        '===========================================

        Configura_Modalita()


    End Sub


    '##############################################################
    Private Sub Attiva_Conti()

        Me.Cmb_AnnoContabile.Enabled = True
        Me.Cmb_Riclassificazione.Enabled = True
        Me.Cmb_Conti.Enabled = True
        Me.ImgBtn_Conti.Enabled = True
        Me.Chk_Figli.Enabled = True
        Me.Chk_Fratelli.Enabled = True

    End Sub

    '##############################################################
    Private Sub Disattiva_Conti()

        Me.Cmb_AnnoContabile.Enabled = False
        Me.Cmb_Riclassificazione.Enabled = False
        Me.Cmb_Conti.Enabled = False
        Me.ImgBtn_Conti.Enabled = False
        Me.Chk_Figli.Enabled = False
        Me.Chk_Fratelli.Enabled = False

    End Sub


    '##############################################################
    Private Sub Configura_Conti(ByVal Flag_Attiva As Boolean)

        Select Case Flag_Attiva

            Case True

                If Qs_PagRitorno = enum_PagineGiasOnline.MenuContab Then
                    Me.Chk_Figli.Enabled = True
                    Me.Chk_Fratelli.Enabled = True
                    Me.Cmb_Conti.Enabled = True
                Else
                    Me.Chk_Figli.Enabled = False
                    Me.Chk_Fratelli.Enabled = False
                    Me.Cmb_Conti.Enabled = False
                End If

            Case False
                Me.Chk_Figli.Enabled = False
                Me.Chk_Figli.Checked = False
                Me.Chk_Fratelli.Enabled = False
                Me.Chk_Fratelli.Checked = False
                Me.Cmb_Conti.Enabled = False
                Me.Cmb_Conti.SelectedIndex = 0

        End Select

    End Sub


    '##########################################################################################
    Private Sub Cmb_AnnoContabile_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_AnnoContabile.SelectedIndexChanged

        If Me.Cmb_AnnoContabile.SelectedIndex > 0 Then

            'Nuovo Anno selezionato
            x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue

            ''Riempimento della riclassificazioni valide per l'anno selezionato
            'CaricaCombo_Riclassificazione2(Server, Session, Page, Me.Cmb_Riclassificazione, Qs_Piva, x_AnnoCont, 0, , )

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Riclassificazioni_2(
                                     Me.Cmb_Riclassificazione,
                                     True, "", "-1",
                                     Qs_Piva,
                                     x_AnnoCont,
                                     0,
                                     0, "",
                                     "", "",
                                     objParametri_Server)

            'seleziona il bilancio personalizzato
            Me.Cmb_Riclassificazione.SelectedIndex =
            Me.Cmb_Riclassificazione.Items.IndexOf(Me.Cmb_Riclassificazione.Items.FindByValue(BILANCIO_PERSONALIZZATO))

            Cmb_Riclassificazione_SelectedIndexChanged(Me, Nothing)

        Else
            Me.Cmb_Riclassificazione.Items.Clear()
            Me.Cmb_Conti.Items.Clear()
        End If

    End Sub


    '##########################################################################################
    Private Sub Cmb_Riclassificazione_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Riclassificazione.SelectedIndexChanged

        If Me.Cmb_Riclassificazione.SelectedIndex > 0 Then

            x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue

            x_Ric_Cod = Me.Cmb_Riclassificazione.SelectedValue

            ''carica i conti
            '_CaricaCombo_Conti2(Server, Session, Page, _
            '                    Me.Cmb_Conti, Qs_Piva, x_Ric_Cod, x_AnnoCont, 0, True, "", "", True)

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Conti(
                                    Me.Cmb_Conti,
                                    True, "", "-1",
                                    Qs_Piva,
                                    x_Ric_Cod,
                                    x_AnnoCont,
                                    0,
                                    0, 0,
                                    "",
                                    "",
                                    True,
                                    "", "",
                                    objParametri_Server)

        Else
            Me.Cmb_Conti.Items.Clear()
        End If


    End Sub



    '##########################################################################################
    Private Sub Cmb_RappContabili_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_RappContabili.SelectedIndexChanged

        x_CodRapporto = Me.Cmb_RappContabili.SelectedValue

        Combo_RapportiContabili_Manuale_Evento(Server, Session, Page,
                                                Me.Cmb_Contatti,
                                                Qs_Piva,
                                                x_CodRapporto)

    End Sub


    '####################################################################################
    Private Sub ImgBtn_Conti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Conti.Click


        If Me.Cmb_AnnoContabile.SelectedIndex > 0 Then

            'Nuovo Anno selezionato
            x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue

            Dim QueryString As String

            QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                            "&rs=" & Stringa_Codifica(Qs_Rag_Soc, AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(x_AnnoCont), AgroKey_EncoderDecoder, Server) &
                            "&rc=" & Stringa_Codifica(CStr(BILANCIO_PERSONALIZZATO), AgroKey_EncoderDecoder, Server) &
                            "&elenco=" & Stringa_Codifica(Me.Txt_FiltroConti.Value, AgroKey_EncoderDecoder, Server)

            Dim script As String = Page_ModalDialog_Script(
                          "../Popup_Informativi/Popup_PianoConti.aspx", QueryString, "Txt_FiltroConti",
                           PopupConti_HEIGHT, PopupConti_WIDTH, 0, 0,
                          , , , , , , "aspnetForm")

            If upContent Is Nothing Then
                Page.FindControl("aspnetForm").Controls.Add(New LiteralControl(script))
            Else
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(upContent, upContent.GetType(),
                                             String.Format("jQuery_{0}", "openmodal"), script, True)

            End If

        Else

            Messaggi.AgroMsgBox("Per impostare un filtro avanzato sui conti, occorre aver selezionato prima l'anno e la riclassificazione!", Page)

        End If



    End Sub


    '####################################################################################
    Private Sub Prepara_Filtro(ByRef Messaggio As String, ByRef Filtro As String, ByRef Ordinamento As String, ByRef Mat_Cod As Integer)

        Dim Filtro_RappCont As String
        Dim Id_Ricl As String
        Dim Dare_Avere As String
        Dim Filtro_Conti_1 As String = ""
        Dim Filtro_Conti_2 As String = ""
        Dim Filtro_Conti As String = ""
        'Dim Filtro_IdRicl As String = ""
        Dim Filtro_CodConto As String = ""
        'x_Flag_Join_CodConto = True

        x_Validita_Inizio = Me.Txt_DataInizio.Text
        If x_Validita_Inizio = "" Then
            x_Validita_Inizio = "01/01/1900"
        End If

        x_Validita_Fine = Me.Txt_DataFine.Text
        If x_Validita_Fine = "" Then
            x_Validita_Fine = "31/12/2100"
        End If

        x_Scadenza = Me.Txt_Scadenza.Text
        If x_Scadenza = "" Then
            x_Scadenza = "31/12/2100"
        End If

        If Me.Txt_DocNumero.Text <> "" Then
            If Not IsNumeric(Me.Txt_DocNumero.Text) Then
                Messaggio &= " E' necessario inserire un valore numerico non nullo nella parte numerica del numero del documento!" & vbCrLf
            Else
                x_DocNumero = CInt(Me.Txt_DocNumero.Text)
            End If
        Else
            If Qs_TipoSblocco = 0 Then
                x_DocNumero = 0
            Else
                'caso sblocco univoco: è necessario specificare il numero del documento da sbloccare!
                Messaggio &= " E' necessario indicare il numero del documento!" & vbCrLf
            End If
        End If

        If Me.Cmb_AnnoContabile.SelectedIndex <= 0 AndALso
             Me.Cmb_Riclassificazione.SelectedIndex <= 0 AndAlso
             Me.Cmb_Conti.SelectedIndex <= 0 Then
            x_AnnoCont = 0
            x_Ric_Cod = 0
            x_Cod_Conto = 0
        Else
            If Me.Cmb_AnnoContabile.SelectedIndex <= 0 Then
                Messaggio &= "Selezionare l'anno contabile!" & vbCrLf & vbCrLf
            Else
                x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue
            End If
            If Me.Cmb_Riclassificazione.SelectedIndex <= 0 Then
                Messaggio &= "Selezionare la riclassificazione!" & vbCrLf & vbCrLf
            Else
                x_Ric_Cod = Me.Cmb_Riclassificazione.SelectedValue
            End If
            If Not Me.Chk_Conti.Checked Then
                If Me.Cmb_Conti.SelectedIndex <= 0 Then
                    Messaggio &= "Selezionare il conto!" & vbCrLf & vbCrLf
                Else
                    x_Cod_Conto = Me.Cmb_Conti.SelectedValue.Split("|")(0)
                End If
            End If
        End If

        If Messaggio <> "" Then
            Exit Sub
        End If

        '================================

        If Not Me.Chk_Conti.Checked Then
            'filtro normale

            If Me.Chk_Fratelli.Checked Then

                Id_Ricl = Me.Cmb_Conti.SelectedItem.Text.Split(" ")(0)
                Dare_Avere = Me.Cmb_Conti.SelectedValue.Split("|")(1)

                Filtro_Conti_1 = " AND ( ( { fn LENGTH(RicXConti.Id_Riclassificazione) } = " & CStr(Id_Ricl.Length) & ") AND (RicXConti.Dare_Avere = '" & Dare_Avere & "') ) "

            End If

            If Me.Chk_Figli.Checked Then

                Id_Ricl = Me.Cmb_Conti.SelectedItem.Text.Split(" ")(0)
                Dare_Avere = Me.Cmb_Conti.SelectedValue.Split("|")(1)

                Filtro_Conti_2 = "  AND (  (RicXConti.Id_Riclassificazione LIKE '" & Id_Ricl & "%') AND (RicXConti.Dare_Avere = '" & Dare_Avere & "')  )"

            End If

            If Me.Chk_Fratelli.Checked OrElse Me.Chk_Figli.Checked Then

                If Filtro_Conti_1 <> "" AndALso Filtro_Conti_2 <> "" Then
                    Filtro_Conti = " AND ( " & Filtro_Conti_1 & " OR  " & Filtro_Conti_2 & " ) "
                Else
                    Filtro_Conti = " AND ( " & Filtro_Conti_1 & Filtro_Conti_2 & " ) "
                End If

            End If

        Else
            'filtro avanzato
            Filtro_Conti = Prepara_FiltroQuery_IdRicl(Me.Txt_FiltroConti.Value)
        End If

        If Me.Chk_Fratelli.Checked OrElse
                Me.Chk_Figli.Checked OrElse
                    Me.Chk_Conti.Checked Then

            'x_Flag_Join_CodConto = False
            x_Cod_Conto = 0

            Dim DT_Conti As DataTable
            Dim j As Integer

            'DT_Conti = _NewCom_PianoConti_Leggi(Server, Session, Page, _
            '                                          Qs_Piva, _
            '                                          x_AnnoCont, _
            '                                          "", _
            '                                          "", _
            '                                          0, _
            '                                          0, _
            '                                          0, _
            '                                          "", _
            '                                          0, _
            '                                          0, _
            '                                          "", _
            '                                          Filtro_Conti)

            Dim objContiEc As New AgronicaCoreContabDAL.PianoConti_Economici_R
            DT_Conti = objContiEc.PianoConti_ContoEconomico(Qs_Piva,
                                                            x_AnnoCont,
                                                            "", "", 0, 0, "", 0, 1, "",
                                                            Filtro_Conti,
                                                            "",
                                                            objParametri_Server)



            're-imposto IL FILTRO per la query generale

            'non va bene filtrare per Id_Riclassificazione perché la query non dovrebbe avere il join sul cod_conto e quindi si perderebbe l'aggancio al movimento
            'Filtro_Conti = "AND ( RicXConti.Id_Riclassificazione IN ( "

            'si fa allora il filtro sul cod_conto
            Filtro_CodConto = "AND ( RicXConti.Cod_Conto IN ( "

            For j = 0 To DT_Conti.Rows.Count - 1

                'Filtro_Conti &= "'" & SQL_SaveText(DT_Conti.Rows(j).Item("Id_Riclassificazione")) & "', "
                Filtro_CodConto &= "'" & Agro_SQL_SaveText(DT_Conti.Rows(j).Item("Cod_Conto")) & "', "

            Next

            Filtro_CodConto = Left(Filtro_CodConto, Filtro_CodConto.Length - 2)

            Filtro_CodConto &= " )   )"

        End If


        x_DocNumero_Sin = Me.Txt_DocNumero_Sin.Text
        If x_DocNumero_Sin = "" Then
            x_DocNumero_Sin = "XYZ"
        End If

        x_DocNumero_Des = Me.Txt_DocNumero_Des.Text
        If x_DocNumero_Des = "" Then
            x_DocNumero_Des = "XYZ"
        End If

        x_Tipo = Me.Cmb_TipoMovimento.SelectedValue
        x_Lav_Cod = Me.Cmb_Causale.SelectedValue


        x_Blocco_Flag = CInt(Me.Rbl_BloccoSblocco.SelectedValue)

        x_CodRapporto = Me.Cmb_RappContabili.SelectedValue

        Select Case x_CodRapporto

            Case COD_CLIENTE
                Filtro_RappCont = " AND (RisUm_Contab.Cod_Rapporto = " & CStr(COD_CLIENTE) & ")"

            Case COD_FORNITORE
                Filtro_RappCont = " AND (RisUm_Contab.Cod_Rapporto = " & CStr(COD_FORNITORE) & ")"

            Case -23

                Select Case CInt(Session("ASG_ProgressivoGIAS"))
                    Case enum_CodiceGIAS_Clienti.Fruttagel
                        Filtro_RappCont = " AND ( (RisUm_Contab.Cod_Rapporto = " & CStr(COD_CLIENTE) & ") OR  (RisUm_Contab.Cod_Rapporto = " & CStr(COD_FORNITORE) & ") OR  (RisUm_Contab.Cod_Rapporto = -11) ) "
                    Case Else
                        Filtro_RappCont = " AND ( (RisUm_Contab.Cod_Rapporto = " & CStr(COD_CLIENTE) & ") OR  (RisUm_Contab.Cod_Rapporto = " & CStr(COD_FORNITORE) & ") ) "
                End Select

            Case COD_DIPENDENTE
                Filtro_RappCont = " AND (RisUm_Contab.Cod_Rapporto = " & CStr(COD_DIPENDENTE) & ")"

            Case COD_TERZISTA
                Filtro_RappCont = " AND (RisUm_Contab.Cod_Rapporto = " & CStr(COD_TERZISTA) & ")"

            Case -45
                Filtro_RappCont = " AND ( (RisUm_Contab.Cod_Rapporto = " & CStr(COD_DIPENDENTE) & ") OR  (RisUm_Contab.Cod_Rapporto = " & CStr(COD_TERZISTA) & ") ) "

            Case Else
                Filtro_RappCont = " "

        End Select

        If Cmb_Contatti.SelectedIndex > 0 Then
            'x_CodRisUm = CInt(Split(Me.Cmb_Contatti.SelectedValue, "|")(0))
            x_CodRisUm = CInt(Me.Cmb_Contatti.SelectedValue)
        Else
            x_CodRisUm = 0
        End If

        'VEDI SOPRA
        'If Me.Cmb_Conti.SelectedIndex > 0 Then

        '    x_Cod_Conto = Me.Cmb_Conti.SelectedValue

        '    x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue

        '    If Me.Cmb_Riclassificazione.SelectedIndex > 0 Then
        '        x_Ric_Cod = Me.Cmb_Riclassificazione.SelectedValue
        '    Else
        '        x_Ric_Cod = 0
        '    End If

        'Else
        '    x_AnnoCont = 0
        '    x_Ric_Cod = 0
        '    x_Cod_Conto = 0
        'End If


        '====================================================


        Select Case Me.Cmb_TipoMovimento.SelectedValue

            'Case 0 'Mov Economici e Pagamenti

            '    'Filtro = "AND (Movimenti_Dettagli.Cod_Conto <> 0) "
            '    Filtro = "AND (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR " & _
            '                " Agenda.Lav_Cod = " & CStr(LAVCOD_ALTRI_COSTI) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_ALTRI_RICAVI) & " OR " & _
            '                " Agenda.Lav_Cod = " & CStr(LAVCOD_ACQUISTO) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_VENDITA) & " OR " & _
            '                " Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_CORRISPETTIVI) & " ) "


            '    '" Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_AL_CLIENTE) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_DAL_FORNITORE) & " ) "

            '--------------------------------

            Case enum_TipoMovimentoContabile.MovEconomiciPagamenti,
                            enum_TipoMovimentoContabile.MovEconomici,
                                enum_TipoMovimentoContabile.Pagamenti

                If Me.Cmb_Causale.SelectedValue <> "-1" Then

                    Filtro = "AND (Agenda.Lav_Cod = " & Me.Cmb_Causale.SelectedValue & " ) "

                Else

                    Filtro = "AND (Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_RICEVUTA) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_FATTURA_EMESSA) & " OR " &
                        " Agenda.Lav_Cod = " & CStr(LAVCOD_ALTRI_COSTI) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_ALTRI_RICAVI) & " OR " &
                        " Agenda.Lav_Cod = " & CStr(LAVCOD_ACQUISTO) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_VENDITA) & " OR " &
                        " Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_REG_COMPENSI) & " ) "

                    '" Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_AL_CLIENTE) & " OR Agenda.Lav_Cod = " & CStr(LAVCOD_NOTA_ACCREDITO_DAL_FORNITORE) & " ) "

                End If

                '--------------------------------

            Case enum_TipoMovimentoContabile.MovFinanziari 'finanziari

                '--------------------------------

            Case enum_TipoMovimentoContabile.BolleDDT 'doc trasporto

                If Me.Cmb_Causale.SelectedValue <> "-1" Then

                    Filtro = "AND (Agenda.Lav_Cod = " & Me.Cmb_Causale.SelectedValue & " ) "

                Else

                    Filtro = "AND (Agenda.Lav_Cod = " & CStr(LAVCOD_BOLLA_RICEVUTA) &
                            " OR Agenda.Lav_Cod = " & CStr(LAVCOD_BOLLA_EMESSA) &
                            " OR Agenda.Lav_Cod = " & CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) &
                            " OR Agenda.Lav_Cod = " & CStr(LAVCOD_ACCETTAZIONE) &
                            " OR Agenda.Lav_Cod = " & CStr(LAVCOD_ACCETTAZIONE_DIVERSI) &
                            " OR Agenda.Lav_Cod = " & CStr(LAVCOD_CONFERIMENTO) &
                            " OR Agenda.Lav_Cod = " & CStr(LAVCOD_CONFERIMENTO_DIVERSI) & " ) "

                End If

                '--------------------------------

        End Select


        Filtro &= Filtro_RappCont

        Filtro &= Filtro_Conti

        Select Case x_Blocco_Flag

            Case -999 'tutti

            Case Else
                '1=bloccati '0=sbloccati
                Filtro &= " AND ( Agenda.Blocco_Flag = " & CStr(x_Blocco_Flag) & ") "

        End Select


        Ordinamento = TipoOrdinamentoQueryContabilita_from_Cod(Me.Cmb_Ordinamento.SelectedValue)


    End Sub


    '####################################################################################
    Private Sub Toolbar_Carica_Dati()

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim Dt_Mov As DataTable
        ' Dim Dr_Mov As DataRow
        Dim Dt_Risc As DataTable = Nothing
        '   Dim Dr_Risc As DataRow

        Dim i As Integer
        Dim Num_Mov As Integer = 0

        Dim Qta As Decimal = 0
        Dim Imponibile As Decimal = 0
        Dim Tot_Imponibile As Decimal = 0
        Dim Importo As Decimal = 0
        Dim Tot_Importo As Decimal = 0
        Dim IVA As Decimal = 0
        Dim Tot_IVA As Decimal = 0
        Dim Tot_Qta As Decimal = 0

        'Dim Data_Inizio, Data_Fine As String
        Dim Filtro As String = ""
        Dim Ordinamento As String = ""
        Dim Log As String = ""
        Dim Tipo_Movimento As Integer

        'Dim Cod_RisUm As Integer = 0
        Dim Mat_Cod As Integer = 0
        'Dim Cod_Conto As Integer = 0

        Dim Dt As New DataTable
        Dim Dr As DataRow


        '------------------------------------

        Try


            Dt.Columns.Add(New DataColumn("Data", GetType(String)))
            Dt.Columns.Add(New DataColumn("DOC_NUMERO_SIN", GetType(String)))
            Dt.Columns.Add(New DataColumn("DOC_NUMERO", GetType(String)))
            Dt.Columns.Add(New DataColumn("DOC_NUMERO_DES", GetType(String)))
            Dt.Columns.Add(New DataColumn("DOC_NUMERO_COMPLETO", GetType(String)))
            Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("CAUSALE", GetType(String)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("SCADENZA", GetType(String)))
            Dt.Columns.Add(New DataColumn("SALDATO", GetType(String)))
            Dt.Columns.Add(New DataColumn("COD_RISUM", GetType(String)))
            Dt.Columns.Add(New DataColumn("COD_CONTATTO", GetType(String)))
            Dt.Columns.Add(New DataColumn("CONTATTO", GetType(String)))
            Dt.Columns.Add(New DataColumn("Anno", GetType(String)))
            Dt.Columns.Add(New DataColumn("Ric_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("COD_CONTO", GetType(String)))
            Dt.Columns.Add(New DataColumn("CONTO", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qta", GetType(String)))
            Dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
            Dt.Columns.Add(New DataColumn("Prezzo_Unitario_Netto", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sconto", GetType(String)))
            Dt.Columns.Add(New DataColumn("IMPONIBILE", GetType(String)))
            Dt.Columns.Add(New DataColumn("Cod_Iva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Aliquota", GetType(String)))
            Dt.Columns.Add(New DataColumn("Iva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Importo", GetType(String)))
            Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(String)))
            Dt.Columns.Add(New DataColumn("Blocco_Data", GetType(String)))
            Dt.Columns.Add(New DataColumn("Blocco_Username", GetType(String)))
            Dt.Columns.Add(New DataColumn("ID_AGENDA", GetType(String)))
            Dt.Columns.Add(New DataColumn("XML_MovDettagli", GetType(String)))
            Dt.Columns.Add(New DataColumn("Stato_Export", GetType(String)))

            '------------------------------------

            Configura_GrigliaMovimenti(Qs_Modalita, Me.Cmb_TipoMovimento.SelectedValue)

            '------------------------------------

            Dim Messaggio As String = ""

            Prepara_Filtro(Messaggio, Filtro, Ordinamento, Mat_Cod)

            If Messaggio <> "" Then
                Messaggi.AgroMsgBox(Messaggio, Page)
                Exit Sub
            End If

            'Ordinamento = "ORDER BY Movimenti.Data_Movimento ASC"

            '------------------------------------

            Select Case Me.Cmb_TipoMovimento.SelectedValue

                'Se è stato selezionato Tutti, economici o bolle
                Case enum_TipoMovimentoContabile.MovEconomiciPagamenti,
                        enum_TipoMovimentoContabile.MovEconomici,
                            enum_TipoMovimentoContabile.BolleDDT

                    Dim Flag_LeggiMagazzino As Boolean

                    If CInt(Qs_Modalita) = enum_TipoModalita.ModAssociazione Then
                        Flag_LeggiMagazzino = True
                    Else
                        Flag_LeggiMagazzino = False
                    End If



                    'TODO, verifica conto e numero , esiste core???

                    'x_Flag_Join_CodConto
                    Dim Contabilita_Movimenti_Dettagli_Leggi As New AgronicaCoreContabDAL.Contabilita_R
                    Dt_Mov = Contabilita_Movimenti_Dettagli_Leggi.Contabilita_Movimenti_Dettagli_Leggi(
                        Piva:=Qs_Piva,
                        Sa_Cod:=0,
                        Id_Agenda:=0,
                        Id_Mov:=0,
                        Id_Mov_Det:=0,
                        Elem_Cod:=0,
                        Pro_Cod:=0,
                        Mat_Cod:=Mat_Cod,
                        Cal_Cod:=0,
                        Cod_Progetto:=0,
                        Fase_Cod:=0,
                        Udm_Cod:=0,
                        Lotto:="",
                        Contabilizzato:=False,
                        Pendente:=0,
                        Cod_Conto:=x_Cod_Conto,
                        Ric_Cod:=x_Ric_Cod,
                        Anno:=x_AnnoCont,
                        Cau_Mov:="",
                        Cod_RisUm:=x_CodRisUm,
                        Cod_Contatto:="",
                        Piva_Contatto:="",
                        FinestraTemp_Inizio:=x_Validita_Inizio,
                        FinestraTemp_Fine:=x_Validita_Fine,
                        Flag_CostiAccessori_Corrispettivi:=False,
                        Flag_Contabilita:=True,
                        FiltroAggiuntivo:=Filtro,
                        Ordinamento:=Ordinamento,
                        Cod_RisUm_Origine:=0,
                        Piva_SuperUser_Origine:="",
                        Flag_AncheImportati:=True,
                        Doc_Numero_Sin:=x_DocNumero_Sin,
                        Doc_Numero:=x_DocNumero,
                        Doc_Numero_Des:=x_DocNumero_Des,
                        Scadenza:=x_Scadenza,
                        Flag_AncheMagazzino:=Flag_LeggiMagazzino,
                        objParametri:=objParametri_Server
                    )




                    'Dt_Mov = NewCom_Contabilita_Movimenti_Dettagli_Leggi(Server, Session, Page, _
                    '                                        Qs_Piva, 0, 0, 0, , _
                    '                                        0, 0, Mat_Cod, 0, 0, 0, _
                    '                                        0, "", 0, 0, _
                    '                                        x_Cod_Conto, x_Ric_Cod, x_AnnoCont, _
                    '                                        "", x_CodRisUm, "", "", _
                    '                                        x_Validita_Inizio, x_Validita_Fine, _
                    '                                        False, _
                    '                                        True, _
                    '                                        Filtro, _
                    '                                        Ordinamento, _
                    '                                        , , , _
                    '                                        x_DocNumero_Sin, x_DocNumero, x_DocNumero_Des, _
                    '                                        x_Scadenza, _
                    '                                        Flag_LeggiMagazzino)


                    'Riempio il datagrid
                    For i = 0 To Dt_Mov.Rows.Count - 1

                        'Progr += 1

                        Num_Mov = Dt_Mov.Rows.Count

                        Dr = Dt.NewRow

                        Dr.Item("Id_Agenda") = Dt_Mov.Rows(i).Item("Id_Agenda")
                        Dr.Item("Lav_Cod") = Dt_Mov.Rows(i).Item("Lav_Cod")

                        Dr.Item("Data") = Format(Dt_Mov.Rows(i).Item("Data_Movimento"), "dd/MM/yyyy")
                        Dr.Item("Doc_Numero_Sin") = Dt_Mov.Rows(i).Item("Doc_Numero_Sin")
                        Dr.Item("Doc_Numero") = Dt_Mov.Rows(i).Item("Doc_Numero")
                        Dr.Item("Doc_Numero_Des") = Dt_Mov.Rows(i).Item("Doc_Numero_Des")

                        Select Case CInt(Session("ASG_ProgressivoGIAS"))

                            Case enum_CodiceGIAS_Clienti.Fruttagel

                                Select Case Dr.Item("Lav_Cod")

                                    Case LAVCOD_ACCETTAZIONE_DIVERSI

                                        'imposto temporaneamente a 0 per non mandare in errore 
                                        'la funzione che calcola il numero completo
                                        'Ma è un caso che non si dovrebbe mai verificare
                                        If Dr.Item("Doc_Numero_Sin") = "" Then
                                            Dr.Item("Doc_Numero_Sin") = 0
                                        End If
                                        Dr.Item("Doc_Numero_Completo") = Formatta_NumeroBollaAccettazioneCompleto_Fruttagel(Dr.Item("Doc_Numero_Sin"), Dt_Mov.Rows(i).Item("Doc_Numero"))

                                    Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                                        Dr.Item("Doc_Numero_Completo") = Formatta_NumeroDDTCompleto_Fruttagel(Dr.Item("Doc_Numero_Sin"), Dt_Mov.Rows(i).Item("Doc_Numero"), Dr.Item("Doc_Numero_Des"))

                                    Case Else
                                        Dr.Item("Doc_Numero_Completo") = Dt_Mov.Rows(i).Item("Doc_Numero_Sin") & CStr(Dt_Mov.Rows(i).Item("Doc_Numero")) & Dt_Mov.Rows(i).Item("Doc_Numero_Des")

                                End Select


                            Case Else
                                Dr.Item("Doc_Numero_Completo") = Dt_Mov.Rows(i).Item("Doc_Numero_Sin") & CStr(Dt_Mov.Rows(i).Item("Doc_Numero")) & Dt_Mov.Rows(i).Item("Doc_Numero_Des")

                        End Select

                        If Me.Cmb_TipoMovimento.SelectedValue = enum_TipoMovimentoContabile.BolleDDT Then
                            Tipo_Movimento = enum_TipoMovimentoContabile.BolleDDT
                            Dr.Item("Descrizione") = Dt_Mov.Rows(i).Item("Mov_Det_Des")
                            If Dr.Item("Descrizione") = "" Then
                                Dim objPr As New AgronicaCoreContabDAL.Contabilita_R
                                Dr.Item("Descrizione") = objPr.LeggiProdotto(objParametri_Server, Nothing, "", Dt_Mov.Rows(i).Item("Elem_Cod"), Dt_Mov.Rows(i).Item("Pro_Cod"), Dt_Mov.Rows(i).Item("Mat_Cod"), , , , , , , , False)
                            End If
                        Else
                            Tipo_Movimento = enum_TipoMovimentoContabile.MovEconomici
                            Dr.Item("Descrizione") = Dt_Mov.Rows(i).Item("Extra_Str") & " - " & Dt_Mov.Rows(i).Item("Mov_Desc")  'Dt_Mov.Rows(i).Item("Des_Lib") 
                        End If

                        Dr.Item("Causale") = TipoCausale_from_LavCod_e_Tipo(Tipo_Movimento, Dt_Mov.Rows(i).Item("Lav_Cod"))

                        Dr.Item("Scadenza") = Scadenza_from_LavCod(Dt_Mov.Rows(i).Item("Lav_Cod"), Dt_Mov.Rows(i).Item("Scadenza"), Dt_Mov.Rows(i).Item("Scadenza_extra"))

                        Select Case CDbl(Dt_Mov.Rows(i).Item("Importo_Pagato"))
                            Case 0
                                'NON E' SALDATA
                                Dr.Item("Saldato") = "NO"

                            Case CDbl(Dt_Mov.Rows(i).Item("num_protocollo"))
                                'SALDATA SI'
                                Dr.Item("Saldato") = "SI"

                            Case Else
                                'SALDATA PARZIALMENTE
                                Dr.Item("Saldato") = "Parzialmente"
                        End Select

                        Dr.Item("Cod_Risum") = Dt_Mov.Rows(i).Item("Cod_Risum_contab")
                        Dr.Item("Cod_Contatto") = Dt_Mov.Rows(i).Item("Cod_Contatto_contab")
                        Dr.Item("Contatto") = Dt_Mov.Rows(i).Item("Rag_Soc_contab")
                        Dr.Item("Anno") = Dt_Mov.Rows(i).Item("Anno")
                        Dr.Item("Ric_cod") = Dt_Mov.Rows(i).Item("Ric_Cod")
                        Dr.Item("Cod_Conto") = Dt_Mov.Rows(i).Item("Cod_Conto")
                        Dr.Item("Conto") = Dt_Mov.Rows(i).Item("Id_Riclassificazione") & " - " & Dt_Mov.Rows(i).Item("Conto_Descr")

                        Qta = Dt_Mov.Rows(i).Item("Qta")
                        Dr.Item("Qta") = Format(Qta, "##,###,##0.00")
                        Tot_Qta += Qta

                        Dr.Item("Prezzo_Unitario") = Format(Dt_Mov.Rows(i).Item("Prezzo_Unitario"), "##,###,##0.00")
                        Dr.Item("Prezzo_Unitario_Netto") = Format(Dt_Mov.Rows(i).Item("Prezzo_Unitario_Netto"), "##,###,##0.00")
                        Dr.Item("Sconto") = Format(Dt_Mov.Rows(i).Item("Sconto"), "##,###,##0.00")

                        If CDbl(Dt_Mov.Rows(i).Item("Imponibile_Netto")) <> 0 Then
                            Imponibile = Math.Round(objContabHLP.Leggi_Imponibile_PositivoNegativo(Dt_Mov.Rows(i).Item("Lav_Cod"), CDbl(Dt_Mov.Rows(i).Item("Imponibile_Netto"))), 2)
                        Else
                            Imponibile = Math.Round(objContabHLP.Leggi_Imponibile_PositivoNegativo(Dt_Mov.Rows(i).Item("Lav_Cod"), CDbl(Dt_Mov.Rows(i).Item("Imponibile"))), 2)
                        End If
                        Dr.Item("Imponibile") = Format(Imponibile, "##,###,##0.00")
                        Tot_Imponibile += Imponibile

                        Dr.Item("Cod_Iva") = Dt_Mov.Rows(i).Item("Cod_Iva")

                        'Dr.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Dt_Mov.Rows(i).Item("Cod_Iva"))
                        Dr.Item("Aliquota") = Dt_Mov.Rows(i).Item("Sigla_IVA")

                        IVA = Math.Round(objContabHLP.Leggi_IVA_PositivaNegativa(Dt_Mov.Rows(i).Item("Lav_Cod"), Dt_Mov.Rows(i).Item("Iva")), 2)
                        Dr.Item("Iva") = Format(IVA, "##,###,##0.00")
                        Tot_IVA += IVA

                        Dr.Item("Importo") = Format(Imponibile + IVA, "##,###,##0.00")
                        Tot_Importo += Imponibile + IVA

                        Select Case CInt(Dt_Mov.Rows(i).Item("Blocco_Flag"))
                            Case 0
                                Dr.Item("Blocco_Flag") = "NO"
                            Case 1
                                Dr.Item("Blocco_Flag") = "SI"
                        End Select

                        If CDate(Dt_Mov.Rows(i).Item("Blocco_Data")) = AGRODATAINIZIO Then
                            Dr.Item("Blocco_Data") = ""
                        Else
                            Dr.Item("Blocco_Data") = CDate(Dt_Mov.Rows(i).Item("Blocco_Data")).ToShortDateString
                        End If
                        Dr.Item("Blocco_Username") = Dt_Mov.Rows(i).Item("Blocco_Username")
                        Dr.Item("Stato_Export") = Dt_Mov.Rows(i).Item("Stato_Export")

                        '##########################################################

                        'se sono in modalità associazione, preparo l'xml da mandare alla pagina chiamante
                        If CInt(Qs_Modalita) = enum_TipoModalita.ModAssociazione Then

                            '<Movimento_Dettaglio TipoOperazioneDB="1" piva="00730540382" sa_cod="131073" id_agenda="0" id_mov="0" id_mov_det="0" elem_cod="191" pro_cod="10118" mat_cod="0" udm_cod="2" mov_det_des="AMISTAR" cod_progetto="0" qta_extra="0" prezzo_effettivo="0" lav_cod="1025" cal_cod="0" fase_cod="0" lotto="" qta="2" cod_iva="4" chkiva_manuale="0" jolly_int="1" sconto="0" prezzo_unitario="1" prezzo_unitario_netto="1,00" cod_conto="1000002" contabilizzato="2" pendente="0" validita_inizio="31/ 03/2009" validita_fine="31/12/2100" basecode="131072" topcode="262143" ric_cod="2" anno="2009" imponibile="-2" imponibile_netto="-2" iva=",08" id_destinazione="131073" cau_mov="7300">
                            '<Movimento_Destinazione TipoOperazioneDB="1" piva="00730540382" sa_cod="131073" id_agenda="0" id_mov="0" id_mov_det="0" appezza="0" id_destinazione="131073" tipo_destinazione="20" qta="2" qta2="0" validita_inizio="31/03/2009" validita_fine="31/12/2100" basecode="0" topcode="2000000000" /> 
                            '<Movimento_Riferimento2 TipoOperazioneDB="1" piva="00730540382" sa_cod="0" lav_cod="1000" cau_mov="7300" id_agenda="0" id_mov="0" id_mov_det="0" piva_rif="00730540382" sa_cod_rif="0" lav_cod_rif="1025" cau_mov_rif="4000" id_agenda_rif="3474" id_mov_rif="13297" id_mov_det_rif="17981" qta="2" validita_inizio="31/03/2009" validita_fine="31/12/2100" /> 
                            '</Movimento_Dettaglio>

                            Dim XmlDoc As New XmlDocument
                            Dim Xml_Allegato As XmlElement
                            Dim Xml_MovDettagli As XmlElement
                            Dim Xml_MovDestinazione As XmlElement
                            Dim Xml_MovRiferimento2 As XmlElement

                            Dim QtaXRiferimento As Decimal

                            '/////////////////////////////////////////////////////////////
                            '           Movimento_Dettaglio
                            '/////////////////////////////////////////////////////////////

                            Xml_MovDettagli = XmlDoc.CreateElement("Movimento_Dettaglio")

                            Xml_MovDettagli.SetAttribute("piva", Dt_Mov.Rows(i).Item("piva"))
                            Xml_MovDettagli.SetAttribute("sa_cod", Dt_Mov.Rows(i).Item("Sa_Cod_Dett"))
                            Xml_MovDettagli.SetAttribute("id_agenda", Dt_Mov.Rows(i).Item("Id_Agenda"))
                            Xml_MovDettagli.SetAttribute("id_mov", Dt_Mov.Rows(i).Item("Id_Mov_Mag"))
                            Xml_MovDettagli.SetAttribute("id_mov_det", Dt_Mov.Rows(i).Item("Id_Mov_Det"))
                            Xml_MovDettagli.SetAttribute("mov_det_des", Dt_Mov.Rows(i).Item("Mov_Det_Des"))
                            Xml_MovDettagli.SetAttribute("elem_cod", Dt_Mov.Rows(i).Item("Elem_Cod"))
                            Xml_MovDettagli.SetAttribute("pro_cod", Dt_Mov.Rows(i).Item("Pro_Cod"))
                            Xml_MovDettagli.SetAttribute("mat_cod", Dt_Mov.Rows(i).Item("Mat_Cod"))
                            Xml_MovDettagli.SetAttribute("cod_progetto", Dt_Mov.Rows(i).Item("Cod_Progetto"))
                            Xml_MovDettagli.SetAttribute("fase_cod", Dt_Mov.Rows(i).Item("Fase_Cod"))
                            Xml_MovDettagli.SetAttribute("lotto", Dt_Mov.Rows(i).Item("Lotto"))
                            Xml_MovDettagli.SetAttribute("cal_cod", Dt_Mov.Rows(i).Item("Cal_Cod"))
                            Xml_MovDettagli.SetAttribute("udm_cod", Dt_Mov.Rows(i).Item("Udm_Cod"))
                            Xml_MovDettagli.SetAttribute("udm_cod_extra", Dt_Mov.Rows(i).Item("udm_cod_extra"))
                            Xml_MovDettagli.SetAttribute("qta", Dt_Mov.Rows(i).Item("qta"))
                            Xml_MovDettagli.SetAttribute("qta_extra", Dt_Mov.Rows(i).Item("qta_extra"))
                            Xml_MovDettagli.SetAttribute("prezzo_unitario", Dt_Mov.Rows(i).Item("Prezzo_Unitario"))
                            Xml_MovDettagli.SetAttribute("prezzo_unitario_netto", Dt_Mov.Rows(i).Item("Prezzo_Unitario_Netto"))
                            Xml_MovDettagli.SetAttribute("sconto", Dt_Mov.Rows(i).Item("Sconto"))
                            Xml_MovDettagli.SetAttribute("prezzo_effettivo", Dt_Mov.Rows(i).Item("prezzo_effettivo"))
                            Xml_MovDettagli.SetAttribute("anno", Dt_Mov.Rows(i).Item("Anno"))
                            Xml_MovDettagli.SetAttribute("ric_cod", Dt_Mov.Rows(i).Item("Ric_Cod"))
                            Xml_MovDettagli.SetAttribute("cod_conto", Dt_Mov.Rows(i).Item("Cod_Conto"))
                            'così nella fattura mi risparmio una query
                            Xml_MovDettagli.SetAttribute("conto", Dr.Item("Conto"))
                            Xml_MovDettagli.SetAttribute("pendente", Dt_Mov.Rows(i).Item("Pendente"))
                            Xml_MovDettagli.SetAttribute("contabilizzato", Dt_Mov.Rows(i).Item("Contabilizzato"))
                            Xml_MovDettagli.SetAttribute("imponibile", Dt_Mov.Rows(i).Item("Imponibile"))
                            Xml_MovDettagli.SetAttribute("imponibile_netto", Dt_Mov.Rows(i).Item("Imponibile_Netto"))
                            Xml_MovDettagli.SetAttribute("cod_iva", Dt_Mov.Rows(i).Item("Cod_Iva"))
                            Xml_MovDettagli.SetAttribute("aliquota", Dr.Item("Aliquota"))
                            Xml_MovDettagli.SetAttribute("iva", Dr.Item("Iva"))
                            Xml_MovDettagli.SetAttribute("importo", Dr.Item("Importo"))

                            'attenzione, il gestore delle giacenze controlla il cau_mov su movimento_dettaglio!!!
                            Xml_MovDettagli.SetAttribute("cau_mov", Dt_Mov.Rows(i).Item("Cau_Mov_Mag"))

                            'la fattura è allegata alla bolla e non bisogna movimentare di nuovo la giacenza!
                            Xml_MovDettagli.SetAttribute("jolly_int", MagazzinoNONMovimentato)

                            'fattura allegata a bolla
                            Xml_MovDettagli.SetAttribute("pendente", enum_Pendenza.DocBolla)

                            Xml_MovDettagli.SetAttribute("contabilizzato", Dt_Mov.Rows(i).Item("contabilizzato"))
                            Xml_MovDettagli.SetAttribute("extra_str", Dt_Mov.Rows(i).Item("extra_str"))
                            Xml_MovDettagli.SetAttribute("extra_int", Dt_Mov.Rows(i).Item("extra_int"))
                            Xml_MovDettagli.SetAttribute("extra_date", Dt_Mov.Rows(i).Item("extra_date"))
                            Xml_MovDettagli.SetAttribute("validita_inizio", Format(Dt_Mov.Rows(i).Item("Data_Movimento"), "dd/MM/yyyy"))
                            Xml_MovDettagli.SetAttribute("validita_fine", AGRODATAFINE)

                            XmlDoc.AppendChild(Xml_MovDettagli)

                            '/////////////////////////////////////////////////////////////
                            '           MovDestinazione
                            '/////////////////////////////////////////////////////////////

                            Xml_MovDestinazione = XML_2_Agenda_MovimentoDestinazione(enum_TipoOperazioneDB.Scrittura,
                                                                                     Dt_Mov.Rows(i).Item("piva"),
                                                                                     Dt_Mov.Rows(i).Item("Sa_Cod_Dett"),
                                                                                     Dt_Mov.Rows(i).Item("Id_Agenda"),
                                                                                     Dt_Mov.Rows(i).Item("Id_Mov"),
                                                                                     Dt_Mov.Rows(i).Item("Id_Mov_Det"),
                                                                                     0,
                                                                                     Dt_Mov.Rows(i).Item("Id_Destinazione"),
                                                                                     Dt_Mov.Rows(i).Item("Tipo_Destinazione"),
                                                                                     Dt_Mov.Rows(i).Item("Qta_Dest"),
                                                                                     Dt_Mov.Rows(i).Item("Qta2"),
                                                                                     Dt_Mov.Rows(i).Item("Tipo_Scorta"),
                                                                                     Dt_Mov.Rows(i).Item("Scorta_Min"),
                                                                                     , , , ,
                                                                                     XmlDoc)

                            'così nella fattura mi risparmio una query
                            Xml_MovDestinazione.SetAttribute("fabbricato_des", Dt_Mov.Rows(i).Item("Fabbricato_Des"))
                            Xml_MovDestinazione.SetAttribute("tipo_fabbricato_cod", Dt_Mov.Rows(i).Item("Tipo_Fabbricato_Cod"))
                            Xml_MovDestinazione.SetAttribute("tipo_fabbricato_des", Dt_Mov.Rows(i).Item("Tipo_Fabbricato_Des"))

                            Xml_MovDettagli.AppendChild(Xml_MovDestinazione)

                            '/////////////////////////////////////////////////////////////
                            '           MovRiferimento2
                            '/////////////////////////////////////////////////////////////

                            '<Movimento_Riferimento2 TipoOperazioneDB="1" piva="00730540382" sa_cod="0" lav_cod="1000" cau_mov="7300" id_agenda="0" id_mov="0" id_mov_det="0" 
                            'piva_rif="00730540382" sa_cod_rif="0" lav_cod_rif="1025" cau_mov_rif="4000" id_agenda_rif="3474" id_mov_rif="13297" id_mov_det_rif="17981" 
                            'qta="2" validita_inizio="31/03/2009" validita_fine="31/12/2100" /> 

                            'Nel Lan viene salvata QtaXRiferimento = 1
                            'Io salvo in QtaXRiferimento la Qta del ddettaglio

                            QtaXRiferimento = Dt_Mov.Rows(i).Item("qta")

                            Xml_MovRiferimento2 = XML_2_Agenda_Movimento_Riferimento2(enum_TipoOperazioneDB.Scrittura,
                                                                                    Dt_Mov.Rows(i).Item("piva"),
                                                                                    0, 0, 0, 0, 0, "",
                                                                                    Dt_Mov.Rows(i).Item("piva"),
                                                                                    0,
                                                                                    Dt_Mov.Rows(i).Item("id_agenda"),
                                                                                    Dt_Mov.Rows(i).Item("id_mov"),
                                                                                    Dt_Mov.Rows(i).Item("id_mov_det"),
                                                                                    Dt_Mov.Rows(i).Item("lav_cod"),
                                                                                    Dt_Mov.Rows(i).Item("cau_mov"),
                                                                                    QtaXRiferimento,
                                                                                    , ,
                                                                                    XmlDoc)

                            Xml_MovDettagli.AppendChild(Xml_MovRiferimento2)

                            '/////////////////////////////////////////////////////////////
                            '           Allegato
                            '/////////////////////////////////////////////////////////////

                            'aggiungo questo nodo, per mio comodo, per mandare le info sulla bolla
                            '(per risparmiarmi delle letture nell'altra pagina)

                            Xml_Allegato = XmlDoc.CreateElement("Allegato")

                            Xml_Allegato.SetAttribute("piva", Dt_Mov.Rows(i).Item("piva"))
                            Xml_Allegato.SetAttribute("sa_cod", 0)
                            Xml_Allegato.SetAttribute("id_agenda", Dt_Mov.Rows(i).Item("Id_Agenda"))
                            Xml_Allegato.SetAttribute("lav_cod", Dt_Mov.Rows(i).Item("lav_cod"))
                            Xml_Allegato.SetAttribute("cau_mov", Dt_Mov.Rows(i).Item("Cau_Mov_Mag"))
                            Xml_Allegato.SetAttribute("data", Dt_Mov.Rows(i).Item("Data_Movimento"))
                            Xml_Allegato.SetAttribute("doc_numero_sin", Dt_Mov.Rows(i).Item("Doc_Numero_Sin"))
                            Xml_Allegato.SetAttribute("doc_numero", Dt_Mov.Rows(i).Item("Doc_Numero"))
                            Xml_Allegato.SetAttribute("doc_numero_des", Dt_Mov.Rows(i).Item("Doc_Numero_Des"))
                            Xml_Allegato.SetAttribute("doc_numero_completo", Dr.Item("Doc_Numero_Completo"))
                            Xml_Allegato.SetAttribute("descrizione", Dr.Item("Descrizione"))
                            Xml_Allegato.SetAttribute("causale", Dr.Item("causale"))
                            Xml_Allegato.SetAttribute("cod_risum", Dt_Mov.Rows(i).Item("Cod_Risum_Contab"))
                            Xml_Allegato.SetAttribute("cod_contatto", Dt_Mov.Rows(i).Item("Cod_Contatto_Contab"))
                            Xml_Allegato.SetAttribute("contatto", Dt_Mov.Rows(i).Item("Rag_Soc_Contab"))

                            Xml_MovRiferimento2.AppendChild(Xml_Allegato)


                            Dr.Item("XML_MovDettagli") = XmlDoc.OuterXml

                        Else
                            Dr.Item("XML_MovDettagli") = "-999"
                        End If

                        '##########################################################

                        Dt.Rows.Add(Dr)

                    Next

            End Select




            '####################################################################################

            'LEGGI RISCOSSIONI

            Ordinamento = "ORDER BY Movimenti.Data_Movimento ASC"

            Select Case Me.Cmb_TipoMovimento.SelectedValue

                'Se è stato selezionato Pagamenti o Tutti
                Case enum_TipoMovimentoContabile.MovEconomiciPagamenti,
                        enum_TipoMovimentoContabile.Pagamenti


                    Dim pagamenti_leggi As New AgronicaCoreContabDAL.Pagamenti_R
                    pagamenti_leggi.Leggi(
                         Piva:=Qs_Piva,
                         Sa_Cod:=0,
                         Id_Agenda:=0,
                         Id_Mov:=0,
                         Cod_Pagamento:=0,
                         Cod_Liquidita:=0,
                         Cau_Pagamento:=0,
                         Flag_SoloScadute:=False,
                         Flag_Insolvenze:=False,
                         Flag_Riscossioni:=False,
                         Cau_Mov:=0,
                         Cod_RisUm:=x_CodRisUm,
                         Piva_Contatto:="",
                         Cod_Contatto:=0,
                         Cod_Conto:=x_Cod_Conto,
                         FinestraTemp_Inizio:=x_Validita_Inizio,
                         FinestraTemp_Fine:=x_Validita_Fine,
                         Cod_RisUm_Origine:=x_CodRisUm,
                         Piva_SuperUser_Origine:="",
                         Flag_AncheImportati:=True,
                         Flag_CostiAccessori_Corrispettivi:=True,
                         Flag_Contabilita:=True,
                         xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaCompleta,
                         xFiltroAggiuntivo:=Filtro,
                         xOrderBy:=Ordinamento,
                         objParametri:=objParametri_Server
                 )


                    'Dt_Risc = NewCom_Pagamenti_Leggi(Server, Session, Page, _
                    '                                    Qs_Piva, 0, 0, 0, _
                    '                                    0, 0, 0, _
                    '                                    False, _
                    '                                    False, _
                    '                                    True, _
                    '                                    "", _
                    '                                    x_CodRisUm, "", "", _
                    '                                    x_Cod_Conto, _
                    '                                    x_Validita_Inizio, x_Validita_Fine, _
                    '                                    Filtro, _
                    '                                    Ordinamento, _
                    '                                    , , , _
                    '                                    True, _
                    '                                    True)


                    'riempio il datagrid
                    For i = 0 To Dt_Risc.Rows.Count - 1

                        Num_Mov += Dt_Risc.Rows.Count

                        Select Case CDate(Dt_Risc.Rows(i).Item("Data_Pagamento"))

                            Case AGRODATAFINE

                                'Pagamento Non Ancora Eseguito

                            Case Else

                                'Pagamento Eseguito -> Storno Conto

                                Dr = Dt.NewRow

                                'Progr += 1

                                Dr.Item("Id_Agenda") = Dt_Risc.Rows(i).Item("Id_Agenda")
                                Dr.Item("Lav_Cod") = Dt_Risc.Rows(i).Item("Lav_Cod")

                                Dr.Item("Data") = Format(Dt_Risc.Rows(i).Item("Data_Movimento"), "dd/MM/yyyy")
                                Dr.Item("Doc_Numero_Sin") = Dt_Risc.Rows(i).Item("Doc_Numero_Sin")
                                Dr.Item("Doc_Numero") = Dt_Risc.Rows(i).Item("Doc_Numero")
                                Dr.Item("Doc_Numero_Des") = Dt_Risc.Rows(i).Item("Doc_Numero_Des")
                                Dr.Item("Doc_Numero_Completo") = Dt_Risc.Rows(i).Item("Doc_Numero_Sin") & CStr(Dt_Risc.Rows(i).Item("Doc_Numero")) & Dt_Risc.Rows(i).Item("Doc_Numero_Des")

                                Dr.Item("Causale") = TipoCausale_from_LavCod_e_Tipo(2, Dt_Risc.Rows(i).Item("Lav_Cod"))
                                Dr.Item("Descrizione") = Dt_Risc.Rows(i).Item("Mov_Desc")  'Dt_Risc.Rows(i).Item("Des_Lib") 

                                Dr.Item("Scadenza") = Scadenza_from_LavCod(Dt_Risc.Rows(i).Item("Lav_Cod"), Dt_Risc.Rows(i).Item("Scadenza"), Dt_Risc.Rows(i).Item("Scadenza_extra"))
                                Dr.Item("Saldato") = "---"
                                Dr.Item("Cod_Risum") = Dt_Risc.Rows(i).Item("Cod_Risum_Contab")
                                Dr.Item("Cod_Contatto") = Dt_Risc.Rows(i).Item("Cod_Contatto_Contab")
                                Dr.Item("Contatto") = Dt_Risc.Rows(i).Item("Rag_Soc_Contab")
                                Dr.Item("Anno") = Dt_Risc.Rows(i).Item("Anno")
                                Dr.Item("Ric_cod") = Dt_Risc.Rows(i).Item("Ric_Cod")
                                Dr.Item("Cod_Conto") = Dt_Risc.Rows(i).Item("Cod_Conto")
                                Dr.Item("Conto") = Dt_Risc.Rows(i).Item("Id_Riclassificazione") & " - " & Dt_Risc.Rows(i).Item("Conto_Descr")

                                Dr.Item("Qta") = "" ' Dt_Risc.Rows(i).Item("Qta")
                                Dr.Item("Prezzo_Unitario") = "" ' Dt_Risc.Rows(i).Item("Prezzo_Unitario")
                                Dr.Item("Prezzo_Unitario_Netto") = "" 'Dt_Risc.Rows(i).Item("Prezzo_Unitario_Netto")
                                Dr.Item("Sconto") = "" 'Dt_Risc.Rows(i).Item("Sconto")

                                Dr.Item("Imponibile") = "" 'Format(Imponibile, "##,###,##0.00")
                                Dr.Item("Cod_Iva") = "" '0
                                Dr.Item("Aliquota") = ""
                                Dr.Item("Iva") = "" 'Format(0, "##,###,##0.00")

                                Importo = Math.Round(objContabHLP.Leggi_Imponibile_PositivoNegativo(Dt_Risc.Rows(i).Item("Lav_Cod"), CDbl(Dt_Risc.Rows(i).Item("Importo"))), 2)
                                Dr.Item("Importo") = Format(Dt_Risc.Rows(i).Item("Importo"), "##,###,##0.00")

                                Dr.Item("XML_MovDettagli") = "-999"

                                Dr.Item("Blocco_Flag") = ""
                                Dr.Item("Blocco_Data") = ""
                                Dr.Item("Blocco_Username") = ""
                                Dr.Item("Stato_Export") = ""

                                Dt.Rows.Add(Dr)

                        End Select


                    Next


            End Select 'pagamenti

            ' ----------------------------------------------------------------------

            Me.DataGrid_Movimenti.DataSource = Dt
            Me.DataGrid_Movimenti.DataBind()

            ' ----------------------------------------------------------------------

            Me.Txt_Imponibile.Text = Format(Tot_Imponibile, "##,###,##0.00")
            Me.Txt_Imponibile_2.Text = Format(Tot_Imponibile, "##,###,##0.00")
            Me.Txt_Imposta.Text = Format(Tot_IVA, "##,###,##0.00")
            Me.Txt_Importo.Text = Format(Tot_Importo, "##,###,##0.00")
            Me.Txt_Quantita.Text = Format(Tot_Qta, "##,###,##0.00")

            Me.Txt_NumMovimenti.Text = Num_Mov
            Me.Txt_NumMovimenti2.Text = Num_Mov

            '---------------

            If Log <> "" Then
                Messaggi.AgroMsgBox(Log, Page)
            End If


            '################################################

        Catch ex As Exception

            Messaggi.AgroMsgBox("Problemi nel caricamento dei dati: " & vbCrLf & ex.Message, Page)

        End Try


    End Sub


    '####################################################################################
    Private Sub Toolbar_Info()

        Dim PaginaAspx As String
        Dim Id_Agenda As Integer = 0
        Dim Lav_Cod As Integer = 0
        Dim Operazioni_Selezionate As Integer = 0

        'selezionare l'operazione dal datagrid
        Recupera_ChiaveOperazione(Id_Agenda,
                                     Lav_Cod,
                                     Operazioni_Selezionate)

        Select Case Operazioni_Selezionate

            Case 0
                Messaggi.AgroMsgBox("Non è stato selezionato alcun movimento!", Page)
                Exit Sub

            Case 1
                'ok

                'passare anche i valori per creare la querystring
                PaginaAspx = PaginaContabile_from_LavCod(Server, "../", Qs_Piva, Lav_Cod, Id_Agenda, enum_PagineGiasOnline.FiltroMovContabili, Qs_Rag_Soc)


                Dim strOpen As String = "<script language='javascript'>" & vbNewLine &
                                        "window.open('" & PaginaAspx & "'," &
                                        "'Contabilita','height=800,width=1000,scrollbars=yes,top=0,left=0');" & vbNewLine &
                                        "</script>"
                'apro la finestra...
                Me.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))


            Case Is > 1
                Messaggi.AgroMsgBox("Sono stati selezionati più movimenti: " & vbCrLf &
                            "Selezionarne uno solo!", Page)
                Exit Sub

        End Select


    End Sub



    '####################################################################################
    Private Sub Toolbar_InfoRiferimenti()

        'Dim PaginaAspx As String
        Dim Id_Agenda As Integer = 0
        Dim Lav_Cod As Integer = 0
        Dim Operazioni_Selezionate As Integer = 0

        'selezionare l'operazione dal datagrid
        Recupera_ChiaveOperazione(Id_Agenda,
                                     Lav_Cod,
                                     Operazioni_Selezionate)

        Select Case Operazioni_Selezionate

            Case 0
                Messaggi.AgroMsgBox("Non è stato selezionato alcun movimento!", Page)
                Exit Sub

            Case 1
                'ok

                Dim QueryString As String

                QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                "&rs=" & Stringa_Codifica(QS_SaveText(Qs_Rag_Soc), AgroKey_EncoderDecoder, Server) &
                                "&i=" & Stringa_Codifica(CStr(Id_Agenda), AgroKey_EncoderDecoder, Server)

                Page_NewWindow(Page,
                                "GestioneAllegati.aspx",
                                QueryString,
                                "Allegati",
                                550,
                                1000,
                                0, 0,
                                , , , )

            Case Is > 1
                Messaggi.AgroMsgBox("Sono stati selezionati più movimenti: " & vbCrLf &
                            "Selezionarne uno solo!", Page)
                Exit Sub

        End Select

    End Sub



    '####################################################################################
    Private Sub Toolbar_Impresa()


        Dim Num_Imprese As Integer = 0
        Dim Piva As String = ""

        Num_Imprese = Numero_Imprese_from_SuperUser(objParametri_Server, Session, Page, Piva, Nothing)

        If Num_Imprese = 1 Then
            Exit Sub
        Else

            Dim TargetURL As String
            Dim Origine As String
            Dim Destinazione As String
            Dim Qs As String

            'Costruisco il link
            Origine = Stringa_Codifica(
                            "../GestioneContabilita/FiltroMovContabili.aspx",
                            AgroKey_EncoderDecoder, Server)

            Qs = "?a=" & Stringa_Codifica(Qs_AnnoContabile, AgroKey_EncoderDecoder, Server) &
                  "&d=" & Stringa_Codifica(Qs_DataSelezionata, AgroKey_EncoderDecoder, Server) &
                    "&mode=" & Stringa_Codifica(Qs_Modalita, AgroKey_EncoderDecoder, Server) &
                    "&cod_ru=" & Stringa_Codifica(Qs_Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                    "&fil=" & Stringa_Codifica(Qs_Filtro, AgroKey_EncoderDecoder, Server) &
                    "&orig=" & Stringa_Codifica(Qs_PagRitorno, AgroKey_EncoderDecoder, Server) &
                    "&tm=" & Stringa_Codifica(Qs_TipoMov, AgroKey_EncoderDecoder, Server)

            Destinazione = Stringa_Codifica(
                            "../GestioneContabilita/FiltroMovContabili.aspx" & Qs,
                            AgroKey_EncoderDecoder, Server)

            'TargetURL = "../Utility/SelezioneImpresa.aspx" & _
            '                "?o=" & Origine & _
            '                "&d=" & Destinazione

            TargetURL = "../Utility/Filtrino.aspx" &
                        "?o=" & Origine &
                        "&d=" & Destinazione

            'Vado alla pagina
            Response.Redirect(TargetURL)

        End If



    End Sub


    '####################################################################################
    Private Sub Toolbar_Stampa()

        Dim Id_Agenda As Integer = 0
        Dim Lav_Cod As Integer = 0
        Dim Operazioni_Selezionate As Integer = 0

        Recupera_ChiaveOperazione(Id_Agenda, Lav_Cod, Operazioni_Selezionate)

        Select Case Operazioni_Selezionate

            Case 0
                Messaggi.AgroMsgBox("Non è stato selezionato alcun movimento!", Page)
                Exit Sub

            Case 1

                Stampa_Documento(Server, objParametri_Server, Session, Page, Lav_Cod, Qs_Piva, Id_Agenda, "../")

            Case Is > 1
                Messaggi.AgroMsgBox("Sono stati selezionati più movimenti: " & vbCrLf &
                            "Selezionarne uno solo!", Page)
                Exit Sub

        End Select




    End Sub


    '####################################################################################
    Private Sub Toolbar_BloccaSblocca(ByVal BloccoSblocco As Enum_BloccoSblocco)

        'Select Case Me.Cmb_TipoMovimento.SelectedValue

        '    Case 1, 4

        '    Case Else

        '        messaggi.AgroMsgBox("Selezionare come Tipo Movimento 'Movimenti Economici' oppure 'Bolle e DDT'!", Page)
        '        Exit Sub

        'End Select

        '------------------

        Dim UtenteAbilitato As Boolean
        Dim Id_Attivita As Integer
        Dim Testo As String = ""
        Dim Testo2 As String = ""
        Dim Testo3 As String = ""

        Select Case BloccoSblocco

            Case Enum_BloccoSblocco.Blocco
                Id_Attivita = enum_Security_Attivita.Contabilita_Operazioni_Blocco
                Testo = "il Blocco"
                Testo2 = "bloccati"
                Testo3 = "bloccato"

            Case Enum_BloccoSblocco.Sblocco
                Id_Attivita = enum_Security_Attivita.Contabilita_Operazioni_Sblocco
                Testo = "lo Sblocco"
                Testo2 = "sbloccati"
                Testo3 = "sbloccato"

        End Select

        Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                       Session("ASG_IdServizio"),
                                       enum_Security_Attivita.Gest_Contabilita,
                                       enum_Security_Operazione.Modifica,
                                       Now, "",
                                       objParametri_Utenti
                                      )



        If UtenteAbilitato Then

            '==================================
            '========= PERMESSO OK ============
            '==================================

            Dim Id_Agenda As Integer
            'Dim Sa_Cod As Integer
            'Dim Lav_Cod As Integer = 0
            Dim Operazioni_Selezionate As Integer = 0
            Dim i As Integer
            'Dim j As Integer = -1
            Dim Hash_IdAgenda As New Hashtable
            'Dim Vet_IdAgenda() As Integer
            'Dim Vet_SaCod() As Integer
            Dim Num_Bloccati As Integer = 0

            'Recupera_ChiaveOperazione(Id_Agenda, Lav_Cod, Operazioni_Selezionate)

            For i = 0 To Me.DataGrid_Movimenti.Items.Count - 1

                If CType(Me.DataGrid_Movimenti.Items(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked Then

                    Id_Agenda = CStr(Me.DataGrid_Movimenti.Items(i).Cells(COL_ID_AGENDA).Text)
                    'LAV_Cod = CStr(Me.DataGrid_Movimenti.Items(i).Cells(COL_LAV_COD).Text)

                    If Not Hash_IdAgenda.ContainsKey(Id_Agenda) Then

                        Hash_IdAgenda.Add(Id_Agenda, Id_Agenda)

                        Operazioni_Selezionate += 1

                    End If

                    'j += 1
                    'ReDim Vet_IdAgenda(j)
                    'Vet_IdAgenda(j) = Id_Agenda

                End If

            Next


            Select Case Operazioni_Selezionate

                Case 0
                    Messaggi.AgroMsgBox("Non è stato selezionato alcun movimento da bloccare o sbloccare!", Page)
                    Exit Sub

                Case Else

                    Dim Connessione As DbConnection
                    Dim Transazione As DbTransaction
                    Dim ErrMSG As String = ""
                    Dim Flag_Insert As Boolean = False
                    Dim Log As String = ""
                    Dim MyKeys As ICollection
                    Dim Key As Object
                    Dim Des_Lib As String

                    Dim objCore_Agenda_W As AgronicaCoreContabDAL.Agenda_W
                    objCore_Agenda_W = New AgronicaCoreContabDAL.Agenda_W

                    'ciclo sul vettore
                    'For i = 0 To Vet_IdAgenda.Length - 1

                    If (Hash_IdAgenda.Count > 0) Then

                        MyKeys = Hash_IdAgenda.Keys()

                        For Each Key In MyKeys

                            Try

                                ' APRO LA CONNESSIONE AL DATABASE
                                Dim DataProvider As New DataProviderFactoryAspx
                                Connessione = DataProvider.Factory.Instance.Provider.ApriConnessione(objParametri_Server, ErrMSG)
                                Transazione = Connessione.BeginTransaction()

                                'Id_Agenda = Vet_IdAgenda(i)
                                Id_Agenda = Hash_IdAgenda(Key)

                                Select Case BloccoSblocco

                                    Case Enum_BloccoSblocco.Blocco

                                        Flag_Insert = objCore_Agenda_W.Agenda_Blocca(Qs_Piva,
                                                                                        0,
                                                                                        Id_Agenda,
                                                                                        CStr(Session("ASG_Utente_CodFiscale")),
                                                                                        Date.Now,
                                                                                         "",
                                                                                         objParametri_Server)

                                        '-------------------------------------------

                                    Case Enum_BloccoSblocco.Sblocco

                                        'Modifica dell' 08/03/2010
                                        'consultare il documento:
                                        '\\Diamante\bk_documentazione\GIAS --- Clienti --- Fruttagel\Esportazione Dati da Gias a One World (JDEdwards)/FRUTTAGEL --- incontro del 2010-02-05 --- richiesta modifiche.doc

                                        'If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then

                                        '    'FRUTTAGEL

                                        '    Flag_Insert = objCore_Agenda_W.Agenda_Sblocca_AzzeraStatoExport(Qs_Piva, _
                                        '                                                                    0, _
                                        '                                                                    Id_Agenda, _
                                        '                                                                    CStr(Session("ASG_Utente_CodFiscale")), _
                                        '                                                                    Date.Now, _
                                        '                                                                    CStr(Session("ASG_Utente_CodFiscale")), _
                                        '                                                                    AGRODATAINIZIO, _
                                        '                                                                    AGRODATAFINE, _
                                        '                                                                    Connessione, _
                                        '                                                                    Transazione, _
                                        '                                                                    Connessione.ConnectionString, _
                                        '                                                                    CStr(Session("ASG_AgronicaCore_DirectoryLOG")), _
                                        '                                                                    NomeFileLog_AgronicaCore, _
                                        '                                                                    CStr(Session("ASG_SuperUser_CodFiscale")))



                                        'Else

                                        '    'TUTTI GLI ALTRI CLIENTI

                                        '    Flag_Insert = objCore_Agenda_W.Agenda_Sblocca(Qs_Piva, _
                                        '                                                    0, _
                                        '                                                    Id_Agenda, _
                                        '                                                    CStr(Session("ASG_Utente_CodFiscale")), _
                                        '                                                    Date.Now, _
                                        '                                                    CStr(Session("ASG_Utente_CodFiscale")), _
                                        '                                                    AGRODATAINIZIO, _
                                        '                                                    AGRODATAFINE, _
                                        '                                                    Connessione, _
                                        '                                                    Transazione, _
                                        '                                                    Connessione.ConnectionString, _
                                        '                                                    CStr(Session("ASG_AgronicaCore_DirectoryLOG")), _
                                        '                                                    NomeFileLog_AgronicaCore, _
                                        '                                                    CStr(Session("ASG_SuperUser_CodFiscale")))


                                        'End If


                                        Flag_Insert = objCore_Agenda_W.Agenda_Sblocca(Qs_Piva,
                                                                                          0,
                                                                                          Id_Agenda,
                                                                                          CStr(Session("ASG_Utente_CodFiscale")),
                                                                                          Date.Now,
                                                                                          "",
                                                                                           objParametri_Server)


                                End Select


                                If Flag_Insert Then
                                    Num_Bloccati += 1
                                Else
                                    Des_Lib = DesLib_from_IdAgenda(objParametri_Server, Qs_Piva, 0, Id_Agenda)
                                    Log &= Testo & " del movimento " & Des_Lib & " non è andato a buon fine." & vbCrLf
                                End If

                                '=================================
                                '======== TRANSAZIONE OK =========
                                Transazione.Commit()
                                Connessione.Close()
                                '=================================

                            Catch ex As Exception
                                Des_Lib = DesLib_from_IdAgenda(objParametri_Server, Qs_Piva, 0, Id_Agenda)
                                Log &= "Errore durante " & Testo & " del movimento " & Des_Lib & ": " & ex.Message & vbCrLf
                            End Try

                        Next 'id_agenda

                        '============================
                        'ricarico i movimenti

                        Toolbar_Carica_Dati()

                        '============================

                    End If 'hashtable vuota

                    If Log <> "" Then
                        Messaggi.AgroMsgBox(Log, Page)
                    End If

                    Select Case Num_Bloccati
                        Case 0
                            Messaggi.AgroMsgBox("Non è stato " & Testo3 & " alcun movimento.", Page)
                        Case 1
                            Messaggi.AgroMsgBox("E' stato " & Testo3 & " un movimento.", Page)
                        Case Else
                            Messaggi.AgroMsgBox("Sono stati " & Testo2 & " " & CStr(Num_Bloccati) & " movimenti.", Page)
                    End Select


            End Select


        Else

            '==================================
            '====== PERMESSO NEGATO ===========
            '==================================

            Messaggi.AgroMsgBox("Non si dispone dei permessi per bloccare e sbloccare i movimenti!", Page)

        End If


    End Sub


    '####################################################################################
    Private Sub Toolbar_Allega()

        Dim i As Integer
        Dim Xml_MovAllegato As String

        Dim Unid As String
        Dim NumeroRecordInteressati As Integer
        Dim Id_Agenda As Integer
        Dim Flag_AlmenoUno As Boolean = False

        Unid = System.Guid.NewGuid.ToString
        Session("Unid_Per_Agenda2010") = Unid
        For i = 0 To Me.DataGrid_Movimenti.Items.Count - 1

            Flag_AlmenoUno = True

            If CType(Me.DataGrid_Movimenti.Items(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked Then

                Xml_MovAllegato = CStr(Me.DataGrid_Movimenti.Items(i).Cells(COL_XML_MOVDETTAGLI).Text)
                Id_Agenda = CInt(Me.DataGrid_Movimenti.Items(i).Cells(COL_ID_AGENDA).Text)

                If Xml_MovAllegato <> "-999" Then

                    Try

                        NumeroRecordInteressati = 0

                        NewCom_Web_ComunicazionePagine_Scrivi(objParametri_Server, Session, Page, NumeroRecordInteressati, Unid, i, enum_TipoOperazioneDB.Scrittura, 0, "", Id_Agenda, Xml_MovAllegato, "")

                    Catch ex As Exception

                        Messaggi.AgroMsgBox("Errore durante l'aggancio del movimento contabile: " & ex.Message, Page)
                        Exit Sub

                    End Try

                End If

            End If

        Next

        If Flag_AlmenoUno Then

            Me.Txt_DaInviare.Value = Unid

            'Dim strClose As String = "<SCRIPT language='javascript'> " & _
            '                        "window.returnValue = document.all('Txt_DaInviare').value; " & _
            '                        "window.close(); " & _
            '                        "</SCRIPT>"

            'Me.Controls.Add(New LiteralControl(strClose))


            Dim strClose As String = "parent.settavalore(' " & Unid & "', 'allegati');"

            'Me.Controls.Add(New LiteralControl(strClose))

            'update panel
            ScriptManager.RegisterClientScriptBlock(upContent, upContent.GetType(),
                                 String.Format("jQuery_{0}", "chiudimi"), strClose, True)



        Else
            Messaggi.AgroMsgBox("Non è stato selezionato alcun movimento!", Page)
            Exit Sub
        End If


    End Sub


    '####################################################################################
    Private Enum Enum_BloccoSblocco

        Blocco = 1
        Sblocco = 0

    End Enum




    '####################################################################################
    Private Sub GestioneOperazioniToolbar(ByVal sender As Object, ByVal e As System.EventArgs)


        Select Case sender.id.ToString

            Case ID_Trova.ID.ToString

                Toolbar_Carica_Dati()

                '--------------------------------------------------

            Case ID_Azzera.ID.ToString

                Toolbar_Azzera_Filtro()

                '--------------------------------------------------

            Case ID_Seleziona.ID.ToString

                Seleziona_Tutto()

                '--------------------------------------------------

            Case ID_Deseleziona.ID.ToString

                Deseleziona_Tutto()

                '--------------------------------------------------

            Case ID_Info.ID.ToString

                Toolbar_Info()

                '--------------------------------------------------

            Case ID_Riferimenti.ID.ToString

                Toolbar_InfoRiferimenti()

                '--------------------------------------------------

            Case ID_Stampa.ID.ToString

                Toolbar_Stampa()

                '--------------------------------------------------

            Case ID_Blocca.ID.ToString

                Toolbar_BloccaSblocca(Enum_BloccoSblocco.Blocco)

                '--------------------------------------------------

            Case ID_Sblocca.ID.ToString

                Toolbar_BloccaSblocca(Enum_BloccoSblocco.Sblocco)

                '--------------------------------------------------

            Case ID_Allega.ID.ToString

                Toolbar_Allega()

                '--------------------------------------------------

            Case ID_Impresa.ID.ToString

                Toolbar_Impresa()

                '  --------------------------------------------------


        End Select



    End Sub


    '####################################################################################
    Private Sub Recupera_ChiaveOperazione(ByRef Id_Agenda As Integer,
                                            ByRef Lav_Cod As Integer,
                                            ByRef Operazioni_Selezionate As Integer)

        Dim i As Integer

        For i = 0 To Me.DataGrid_Movimenti.Items.Count - 1

            If CType(Me.DataGrid_Movimenti.Items(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked Then

                Operazioni_Selezionate += 1

                Id_Agenda = CStr(Me.DataGrid_Movimenti.Items(i).Cells(COL_ID_AGENDA).Text)
                Lav_Cod = CStr(Me.DataGrid_Movimenti.Items(i).Cells(COL_LAV_COD).Text)

            End If

        Next

    End Sub


    '####################################################################################
    Private Sub Seleziona_Tutto()

        Dim i As Integer

        For i = 0 To Me.DataGrid_Movimenti.Items.Count - 1

            CType(Me.DataGrid_Movimenti.Items(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True

        Next


    End Sub


    '####################################################################################
    Private Sub Deseleziona_Tutto()

        Dim i As Integer

        For i = 0 To Me.DataGrid_Movimenti.Items.Count - 1

            CType(Me.DataGrid_Movimenti.Items(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = False

        Next


    End Sub


    '###########################################################################################
    Private Function Formatta_NumeroBollaAccettazioneCompleto_Fruttagel(ByVal Prefisso_Bolla As Integer,
                                                                        ByVal Numero_Bolla As Integer) As Integer

        Dim NumeroBollaCompleto As Integer

        NumeroBollaCompleto = CInt(CStr(Prefisso_Bolla) + Right("00000" & CStr(Numero_Bolla), 5))

        Return NumeroBollaCompleto


    End Function

    '###########################################################################################
    Private Function Formatta_NumeroDDTCompleto_Fruttagel(ByVal Prefisso_Bolla As String,
                                                            ByVal Numero_Bolla As Integer,
                                                            ByVal Suffisso_Bolla As String) As String

        Dim NumeroBollaCompleto As String

        NumeroBollaCompleto = Prefisso_Bolla + Right("00000" & CStr(Numero_Bolla), 5) + Suffisso_Bolla

        Return NumeroBollaCompleto


    End Function





    Protected Sub ID_Trova_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Trova.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Azzera_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Azzera.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Seleziona_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Seleziona.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Deseleziona_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Deseleziona.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Info_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Info.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Riferimenti_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Riferimenti.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Stampa.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Allega_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Allega.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Blocca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Blocca.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Sblocca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Sblocca.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub

    Protected Sub ID_Impresa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Impresa.Click
        GestioneOperazioniToolbar(sender, e)


    End Sub


    Private Sub InizializzaData()

        Dim stb As New StringBuilder
        stb.AppendLine("$(document).ready(function () {")

        stb.AppendLine("    $('.datepicker').datepicker({ ")
        stb.AppendLine("        dateFormat:  'dd/mm/yy',")
        stb.AppendLine("        disabled: false,")
        stb.AppendLine("        changeMonth: true,")
        stb.AppendLine("        changeYear: true")
        stb.AppendLine("    });")

        stb.AppendLine("});")
        ScriptManager.RegisterClientScriptBlock(upContent, upContent.GetType(),
                                         String.Format("jQuery_{0}", "datepicker"), stb.ToString, True)
    End Sub

End Class
