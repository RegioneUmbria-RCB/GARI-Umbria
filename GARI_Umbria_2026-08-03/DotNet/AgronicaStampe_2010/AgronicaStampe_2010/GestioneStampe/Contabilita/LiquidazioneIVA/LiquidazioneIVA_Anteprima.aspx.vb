Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza

Public Class LiquidazioneIVA_Anteprima
    Inherits System.Web.UI.Page

#Region " Liquidazione IVA "



    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents LblTitolo As System.Web.UI.WebControls.Label
    Protected WithEvents ImgIcona As System.Web.UI.WebControls.Image
    Protected WithEvents ImageLogo As System.Web.UI.WebControls.Image
    Protected WithEvents Txt_RagioneSociale As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label7 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnAnnulla As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Credito As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Generale As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL4 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL3 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataInizio As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Anno_Date As System.Web.UI.WebControls.Panel
    Protected WithEvents Label1 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataFine As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Debito As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgBtnCaricaDati As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label2 As System.Web.UI.WebControls.Label
    Protected WithEvents Label5 As System.Web.UI.WebControls.Label
    Protected WithEvents Label6 As System.Web.UI.WebControls.Label
    Protected WithEvents Label8 As System.Web.UI.WebControls.Label
    Protected WithEvents Label9 As System.Web.UI.WebControls.Label
    Protected WithEvents Label10 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Credito As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Debito As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Differenza As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label11 As System.Web.UI.WebControls.Label
    Protected WithEvents Label12 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumFattureRicevute As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label13 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumAltriCosti As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label14 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumAcquisti As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label15 As System.Web.UI.WebControls.Label
    Protected WithEvents TEXTBOX1 As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumMovCredito As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label16 As System.Web.UI.WebControls.Label
    Protected WithEvents Label17 As System.Web.UI.WebControls.Label
    Protected WithEvents Label18 As System.Web.UI.WebControls.Label
    Protected WithEvents Label19 As System.Web.UI.WebControls.Label
    Protected WithEvents Label20 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumMovDebito As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumFattureEmesse As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumAltriRicavi As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumVendite As System.Web.UI.WebControls.TextBox
    Protected WithEvents ImgBtnStampa As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label21 As System.Web.UI.WebControls.Label
    Protected WithEvents Chk_IVA_Precedente As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Txt_IVA_Precedente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_TotCredito As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label22 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_TotDebito As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label23 As System.Web.UI.WebControls.Label
    Protected WithEvents Rbl_Temp As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents DataGrid_Debito As System.Web.UI.WebControls.DataGrid
    Protected WithEvents Panel1 As System.Web.UI.WebControls.Panel
    Protected WithEvents DataGrid_Credito As System.Web.UI.WebControls.DataGrid
    Protected WithEvents Pannello_DataGridCredito As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_NumResiVendite As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumResiAcquisti As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label26 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumRicevuteFiscaliEmesse As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_DataGridDebito As System.Web.UI.WebControls.Panel
    Protected WithEvents Label27 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Sezionali As System.Web.UI.WebControls.DropDownList
    Protected WithEvents LblRegimeIva As System.Web.UI.WebControls.Label
    Protected WithEvents LblLiquidazione As System.Web.UI.WebControls.Label
    Protected WithEvents Label28 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumDDTContabilizzati_Emessi As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label29 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumAutoconsumo As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label30 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumPaginaRegIVA As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label31 As System.Web.UI.WebControls.Label
    Protected WithEvents Label25 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Conti As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Cmb_Riclassificazione As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Label24 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_AnnoContabile As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_AnnoContabile As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_Indietro As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Conti As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgBtn_Avanti As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Txt_Acconto As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_InteresseDebito_Perc As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_InteresseDebito_Valore As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label32 As System.Web.UI.WebControls.Label
    Protected WithEvents Btn_CalcolaSaldo As System.Web.UI.WebControls.Button
    Protected WithEvents Txt_ImpostaDaVersare As System.Web.UI.WebControls.TextBox

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '----- Gestione Querystring
    Dim DataInizio As String
    Dim DataFine As String

    Dim Qs_Rag_Soc As String
    Dim Piva As String
    Dim Anno As String
    Dim x_AnnoCont As Integer
    Dim x_Ric_Cod As Integer
    Dim x_Cod_Conto As Integer

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            If Session("ASG_objParametri_Server") Is Nothing Then
                Response.Redirect("~/Custom500.aspx")
            End If


            '##############################################################
            '#####  Recupero le informazioni dal DB  ######################
            '##############################################################

            Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                            AgroKey_EncoderDecoder, _
                            Server)

            Anno = Stringa_Decodifica(Request.QueryString("a").ToString, _
                                     AgroKey_EncoderDecoder, _
                                     Server)

            DataInizio = Stringa_Decodifica(Request.QueryString("di").ToString, _
                                            AgroKey_EncoderDecoder, _
                                            Server)

            DataFine = Stringa_Decodifica(Request.QueryString("df").ToString, _
                                            AgroKey_EncoderDecoder, _
                                            Server)

            Qs_Rag_Soc = Stringa_Decodifica(Request.QueryString("rs").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################

            If Not Page.IsPostBack Then

                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

                ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

                'Dim strDummy As String      'controllo accesso negato.....
                'Dim UtenteAbilitato As Boolean


                'UtenteAbilitato = Controlla_Permessi_Utente_2( _
                '                            Server, Session, Page, _
                '                            Session("ASG_Utente_Username"), _
                '                            Session("ASG_IdServizio"), _
                '                            TipiEnumerativi.enum_Security_Attivita.Gest_Contabilita, _
                '                            TipiEnumerativi.enum_Security_Operazione.Lettura, _
                '                            strDummy)


                ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
                'If UtenteAbilitato = False Then
                '    'Response.Redirect("../MenuContabilita.aspx")
                '    Dim strClose As String = "<script language='javascript'>window.close()</script>"
                '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
                'End If

            Else

                '==========================================
                '===== Pagina ricaricata in POSTBACK
                '==========================================

                Exit Sub

            End If


            '########################################################################


            Me.Txt_RagioneSociale.Text = Qs_Rag_Soc 'RagSoc_from_Piva(Server, Session, Page, Piva)

            '===========================================

            'PERIODO DI COMPETENZA

            'Me.Txt_DataInizio.Text = "01/" + Format(Date.Today, "MM") + "/" + CStr(Date.Today.Year)
            'Me.Txt_DataFine.Text = CStr(Date.Today)

            Dim data_inizio As Date = AGRODATAINIZIO
            Dim data_fine As Date = AGRODATAFINE

            Dim objHLP As New AgronicaCoreContabHLP.Contabilita
            objHLP.DataInizioFineMese_from_Data(data_inizio, _
                                                 data_fine, _
                                                 Date.Today)


            Me.Txt_DataInizio.Text = data_inizio
            Me.Txt_DataFine.Text = data_fine

            '===========================================

            'PIANO DEI CONTI

            'Carica gli anni presenti nella tabella RicxConti per quella partita iva
            ' CaricaCombo_AnnoContabile2(Server, Session, Page, Me.Cmb_AnnoContabile, Piva, , , , True, , )
            AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile, _
                                                                                True, "", "", _
                                                                                Piva, _
                                                                                0, 0, _
                                                                                "", "", _
                                                                                objParametri_Server)


            '===========================================

            AgronicaCoreUtility.CaricaListControl.Imprese_Sezionali(Me.Cmb_Sezionali, _
                                                                    True, "Tutti i Sezionali", "-1|1|-1|-1", _
                                                                    5, _
                                                                    Piva, _
                                                                    0, _
                                                                    "", "", _
                                                                    objParametri_Server)

            If Me.Cmb_Sezionali.Items.Count > 1 Then
                Me.Cmb_Sezionali.SelectedIndex = 1
            End If

            Try

                Me.LblRegimeIva.Text = "Regime Iva: " & objHLP.RegimeIVA_Desc_from_Cod(Me.Cmb_Sezionali.SelectedValue.Split("|")(3))
                Me.LblLiquidazione.Text = "Liquidazione Iva: " & objHLP.LiquidazioneIVA_Desc_from_Cod(Me.Cmb_Sezionali.SelectedValue.Split("|")(2))

                Dim InteresseDebitoIva As Double = 0
                InteresseDebitoIva = Me.Cmb_Sezionali.SelectedValue.Split("|")(1)

                Me.Txt_InteresseDebito_Perc.Text = Format(InteresseDebitoIva, "#,###,##0.00") '& " %"
                Me.Txt_InteresseDebito_Valore.Text = "0.00"

            Catch ex As Exception
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Lettura Regime Iva  / Interesse Debito Iva: " + ex.Message, Page)
            End Try

            '==========================================
            ' CARICAMENTO DATI
            '==========================================

            CaricaDati()

            '===========================================

        Catch exc As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Errore: " + exc.Message, Page)
        End Try

    End Sub

    '##############################################################
    Private Sub ImgBtnAnnulla_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnulla.Click

        Dim strClose As String = "<script language='javascript'>window.close()</script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub


    '##############################################################
    Private Sub ImgBtn_Indietro_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Indietro.Click
        AgronicaCoreDataProvider.UtilityProvider.CalcolaDate_ScorriMese(1, _
                                                                        Me.Txt_DataInizio.Text, _
                                                                        Me.txt_DataFine.Text)
    End Sub

    '##############################################################
    Private Sub ImgBtn_Avanti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Avanti.Click
        AgronicaCoreDataProvider.UtilityProvider.CalcolaDate_ScorriMese(0, _
                                                                        Me.Txt_DataInizio.Text, _
                                                                        Me.txt_DataFine.Text)
    End Sub

    '##########################################################################################
    Private Sub Cmb_AnnoContabile_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If Me.Cmb_AnnoContabile.SelectedIndex > 0 Then

            'Nuovo Anno selezionato
            x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue

            ''Riempimento della riclassificazioni valide per l'anno selezionato
            'CaricaCombo_Riclassificazione2(Server, Session, Page, Me.Cmb_Riclassificazione, Piva, x_AnnoCont, 0, , )

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Riclassificazioni_2( _
                                                    Me.Cmb_Riclassificazione, _
                                                    True, "", "-1", _
                                                    Piva, _
                                                    x_AnnoCont, _
                                                    0, 0, "", _
                                                    "", "", _
                                                    objParametri_Server)

            'seleziona il bilancio personalizzato
            Me.Cmb_Riclassificazione.SelectedIndex = _
            Me.Cmb_Riclassificazione.Items.IndexOf(Me.Cmb_Riclassificazione.Items.FindByValue(BILANCIO_PERSONALIZZATO))

            Cmb_Riclassificazione_SelectedIndexChanged(Me, Nothing)

        Else
            Me.Cmb_Riclassificazione.Items.Clear()
            Me.Cmb_Conti.Items.Clear()
        End If

    End Sub


    '##########################################################################################
    Private Sub Cmb_Riclassificazione_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        If Me.Cmb_Riclassificazione.SelectedIndex > 0 Then

            x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue

            x_Ric_Cod = Me.Cmb_Riclassificazione.SelectedValue

            ''carica i conti
            '_CaricaCombo_Conti2(Server, Session, Page, _
            '                    Me.Cmb_Conti, Piva, x_Ric_Cod, x_AnnoCont, 0, True, "")

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Conti(Me.Cmb_Conti, _
                                True, "", "-1", _
                                Piva, _
                                x_Ric_Cod, _
                                x_AnnoCont, _
                                0, 0, 0, "", "", _
                                False, _
                                "", "", _
                                objParametri_Server)

        Else
            Me.Cmb_Conti.Items.Clear()
        End If


    End Sub

    '##############################################################
    Private Sub Rbl_Temp_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'Select Case Me.Rbl_Temp.SelectedValue

        '    Case 0

        '        Me.Cmb_AnnoContabile.Enabled = True
        '        Me.Cmb_AnnoContabile.SelectedIndex = _
        '              Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Anno))

        '        Me.BtnImpostaData1.Disabled = True
        '        Me.BtnImpostaData2.Disabled = True
        '        Me.Txt_DataInizio.Text = ""
        '        Me.Txt_DataFine.Text = ""


        '    Case 1

        '        Me.Cmb_AnnoContabile.Enabled = False
        '        Me.Cmb_AnnoContabile.SelectedIndex = 0

        '        Me.BtnImpostaData1.Disabled = False
        '        Me.BtnImpostaData2.Disabled = False

        'End Select


    End Sub


    '####################################################################################
    Private Sub Calcola_Date_InizioFine(ByRef Data_Inizio As String, ByRef Data_fine As String, _
                                        ByRef Log As String, Optional ByVal Flag_AnnoPrecedente As Boolean = False)

        If Flag_AnnoPrecedente = False Then

            'Select Case Me.Rbl_Temp.SelectedValue

            '    Case 0
            '        Data_Inizio = "01/01/" + Me.Cmb_AnnoContabile.SelectedValue
            '        Data_fine = "31/12/" + Me.Cmb_AnnoContabile.SelectedValue

            '    Case 1

            If Me.Txt_DataInizio.Text = "" Then
                Log += "Inserire la data inizio del periodo di competenza." & vbCrLf
            Else
                Data_Inizio = Me.Txt_DataInizio.Text
            End If

            If Me.Txt_DataFine.Text = "" Then
                Log += "Inserire la data fine del periodo di competenza." & vbCrLf
            Else
                Data_fine = Me.Txt_DataFine.Text
            End If

            'End Select

        Else

            'Select Case Me.Rbl_Temp.SelectedValue

            '    Case 0
            '        Data_Inizio = "01/01/" + CStr(CInt(Me.Cmb_AnnoContabile.SelectedValue) - 1)
            '        Data_fine = "31/12/" + CStr(CInt(Me.Cmb_AnnoContabile.SelectedValue) - 1)

            '   Case 1

            If Me.Txt_DataInizio.Text = "" Then
                Log += "Inserire la data inizio del periodo di competenza." & vbCrLf
            Else
                Data_Inizio = "01/01/" + CStr(CInt(CDate(Me.Txt_DataInizio.Text).Year) - 1)
                Data_fine = "31/12/" + CStr(CInt(CDate(Me.Txt_DataInizio.Text).Year) - 1)
            End If

            'End Select

        End If



    End Sub

    '####################################################################################
    Private Sub Cmb_Sezionali_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Sezionali.SelectedIndexChanged
        Cambia_Sezionale()
    End Sub

    '####################################################################################
    Private Sub Cambia_Sezionale()

        Svuota_dati()

        Dim objHLP As New AgronicaCoreContabHLP.Contabilita

        If Me.Cmb_Sezionali.SelectedValue.Split("|")(3) <> -1 Then
            Me.LblRegimeIva.Text = "Regime Iva: " & objHLP.RegimeIVA_Desc_from_Cod(Me.Cmb_Sezionali.SelectedValue.Split("|")(3))
        Else
            Me.LblRegimeIva.Text = ""
        End If

        If Me.Cmb_Sezionali.SelectedValue.Split("|")(2) <> -1 Then
            Me.LblLiquidazione.Text = "Liquidazione Iva: " & objHLP.LiquidazioneIVA_Desc_from_Cod(Me.Cmb_Sezionali.SelectedValue.Split("|")(2))
        Else
            Me.LblLiquidazione.Text = ""
        End If

    End Sub

    '####################################################################################
    Private Sub Svuota_dati()

        '########################################################################

        ' ----------------------- DATATABLE CREDITO -----------------------------

        Session("DT_IVA_Acquisti_Generale") = Nothing
        Me.DataGrid_Credito.DataSource = Nothing
        Me.DataGrid_Credito.DataBind()

        ' ----------------------------------------------------------------------

        '########################################################################

        ' ----------------------- DATATABLE DEBITO -----------------------------

        Session("DT_IVA_Vendite_Generale") = Nothing
        Me.DataGrid_Debito.DataSource = Nothing
        Me.DataGrid_Debito.DataBind()

        ' ----------------------------------------------------------------------

        Me.Txt_NumMovCredito.Text = 0
        Me.Txt_NumFattureRicevute.Text = 0
        Me.Txt_NumResiAcquisti.Text = 0 'resi su acquisti
        Me.Txt_NumAcquisti.Text = 0
        Me.Txt_NumAltriCosti.Text = 0

        Me.Txt_NumMovDebito.Text = 0
        Me.Txt_NumFattureEmesse.Text = 0
        Me.Txt_NumResiVendite.Text = 0 'resi su vendite
        Me.Txt_NumAutoconsumo.Text = 0
        Me.Txt_NumRicevuteFiscaliEmesse.Text = 0
        Me.Txt_NumVendite.Text = 0
        Me.Txt_NumDDTContabilizzati_Emessi.Text = 0
        Me.Txt_NumAltriRicavi.Text = 0

        Me.Txt_Credito.Text = Format(0, "##,###,##0.00")
        Me.Txt_TotCredito.Text = Format(0, "##,###,##0.00")

        Me.Txt_Debito.Text = Format(0, "##,###,##0.00")
        Me.Txt_TotDebito.Text = Format(0, "##,###,##0.00")

        Me.Txt_Acconto.Text = Format(0, "##,###,##0.00")
        Me.Txt_IVA_Precedente.Text = Format(0, "##,###,##0.00")

        Me.Chk_IVA_Precedente.Checked = False

        Me.Txt_Differenza.Text = Format(0, "##,###,##0.00")
        Me.Txt_ImpostaDaVersare.Text = Format(0, "##,###,##0.00")

        Dim InteresseDebitoIva As Double = 0
        InteresseDebitoIva = Me.Cmb_Sezionali.SelectedValue.Split("|")(1)

        Me.Txt_InteresseDebito_Perc.Text = Format(InteresseDebitoIva, "#,###,##0.00")
        Me.Txt_InteresseDebito_Valore.Text = Format(0, "#,###,##0.00")

    End Sub

    '####################################################################################
    Private Sub ImgBtnCaricaDati_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnCaricaDati.Click
        CaricaDati()
    End Sub


    '####################################################################################
    Private Sub CaricaDati()

        Dim objHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
        Dim objSottoIva As New cls_SottoReport_IVA

        Dim Dt_Query As DataTable
        Dim DT_Dettagli_Round As DataTable
        Dim DT_IVA_Round As DataTable
        Dim DT_IVA_Vendite_Generale As DataTable
        Dim DT_IVA_Acquisti_Generale As DataTable

        Dim i As Integer
        Dim Data_Inizio, Data_Fine As String
        '  Dim Filtro, Ordinamento As String
        Dim Log As String = ""

        'IVA A CREDITO
        Dim NumDettagli_Mov_Credito As Integer = 0
        Dim NumDettagli_Fatture_Ricevute As Integer = 0
        Dim NumDettagli_ResiAbbuoni_Acquisti As Integer = 0
        Dim NumDettagli_Corrispettivi_Acquisto As Integer = 0
        Dim NumDettagli_AltriCosti As Integer = 0

        Dim Num_Mov_Credito As Integer = 0
        Dim Num_Fatture_Ricevute As Integer = 0
        Dim Num_ResiAbbuoni_Acquisti As Integer = 0
        Dim Num_Corrispettivi_Acquisto As Integer = 0
        Dim Num_AltriCosti As Integer = 0

        'IVA A DEBITO
        Dim NumDettagli_Mov_Debito As Integer = 0
        Dim NumDettagli_Fatture_Emesse As Integer = 0
        Dim NumDettagli_ResiAbbuoni_Vendite As Integer = 0
        Dim NumDettagli_AutoConsumo As Integer = 0
        Dim NumDettagli_Corrispettivi_Vendita As Integer = 0
        Dim NumDettagli_RicevuteFiscali_Emesse As Integer = 0
        Dim NumDettagli_DDTContabilizzati_Emessi As Integer = 0
        Dim NumDettagli_AltriRicavi As Integer = 0

        Dim Num_Mov_Debito As Integer = 0
        Dim Num_Fatture_Emesse As Integer = 0
        Dim Num_ResiAbbuoni_Vendite As Integer = 0
        Dim Num_AutoConsumo As Integer = 0
        Dim Num_Corrispettivi_Vendita As Integer = 0
        Dim Num_RicevuteFiscali_Emesse As Integer = 0
        Dim Num_DDTContabilizzati_Emessi As Integer = 0
        Dim Num_AltriRicavi As Integer = 0

        Dim Tot_Saldo_Credito As Double = 0
        Dim Tot_Saldo_Debito As Double = 0

        Dim Sezionale_Cod As Integer
        Dim IVA As Double = 0
        Dim Imponibile_netto As Double = 0
        Dim Cod_Iva As Integer = 0
        Dim Aliquota As Double = 0
        Dim id_agenda_memo As Integer = 0
        Dim lav_cod_memo As Integer = 0
        Dim Id_Agenda As Integer
        Dim Lav_Cod As Integer
        Dim Sconto_Modalita As Integer
        Dim iva_indetraibile_perc As Double
        Dim iva_indetraibile As Double
        Dim Data_Movimento, des_Lib, Mov_Det_Des, sigla_iva As String
        Dim prezzo_unitario, qta As Double
        Dim Edit_Importo As enum_EditImporto
        Dim Edit_Importo_memo As enum_EditImporto

        Dim Flag_Vendite_memo As Boolean
        Dim Flag_Acquisti_memo As Boolean

        '   Dim Flag_Vendita As Boolean

        '=================================================================

        Try

            'Sezionale_Cod = Me.Cmb_Sezionali.SelectedValue
            Sezionale_Cod = Me.Cmb_Sezionali.SelectedValue.Split("|")(0)

            If Sezionale_Cod = -1 Then
                Dim objSez As New AgronicaCoreContabDAL.Imprese_Sezionali_R

                Dim flag_PiuRegimiIva As Boolean = False
                Dim RegimeIvaTuttiSezionali As Integer
                flag_PiuRegimiIva = objSez.EsistonoPiuRegimiIva(Piva, "", RegimeIvaTuttiSezionali, objParametri_Server)
                ViewState("RegimeIvaTuttiSezionali") = RegimeIvaTuttiSezionali
                If flag_PiuRegimiIva = True Then
                    Svuota_dati()
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Attenzione, per i sezionali gestiti, i regimi Iva sono diversi, non si può stampare un prospetto di liquidazione iva unico. Stamparne uno per ogni sezionale. ", Page)
                    Exit Sub
                Else
                    Me.LblRegimeIva.Text = "Regime Iva: " & objHLP.RegimeIVA_Desc_from_Cod(RegimeIvaTuttiSezionali)
                End If

                Dim flag_PiuLiquidazioniIva As Boolean = False
                Dim LiquidazioneIvaTuttiSezionali As Integer
                flag_PiuLiquidazioniIva = objSez.EsistonoPiuLiquidazioniIva(Piva, "", LiquidazioneIvaTuttiSezionali, objParametri_Server)
                ViewState("LiquidazioneIvaTuttiSezionali") = LiquidazioneIvaTuttiSezionali
                If flag_PiuLiquidazioniIva = True Then
                    Svuota_dati()
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Attenzione, per i sezionali gestiti, i periodi di liquidazione Iva sono diversi, non si può stampare un prospetto di liquidazione iva unico. Stamparne uno per ogni sezionale. ", Page)
                    Exit Sub
                Else
                    Me.LblLiquidazione.Text = "Liquidazione Iva: " & objHLP.LiquidazioneIVA_Desc_from_Cod(LiquidazioneIvaTuttiSezionali)
                End If

                Dim flag_PiuInteressiIva As Boolean = False
                Dim InteressiIvaTuttiSezionali As Integer
                flag_PiuInteressiIva = objSez.EsistonoPiuInteressiIva(Piva, "", InteressiIvaTuttiSezionali, objParametri_Server)
                ViewState("InteressiIvaTuttiSezionali") = InteressiIvaTuttiSezionali
                If flag_PiuInteressiIva = True Then
                    Svuota_dati()
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Attenzione, per i sezionali gestiti, gli interessi Iva sono diversi, non si può stampare un prospetto di liquidazione iva unico. Stamparne uno per ogni sezionale. ", Page)
                    Exit Sub
                Else
                    Dim InteresseDebitoIva As Double = InteressiIvaTuttiSezionali

                    Me.Txt_InteresseDebito_Perc.Text = Format(InteresseDebitoIva, "#,###,##0.00")

                End If

            End If

            If Me.Cmb_AnnoContabile.SelectedIndex <= 0 And _
            Me.Cmb_Riclassificazione.SelectedIndex <= 0 And _
            Me.Cmb_Conti.SelectedIndex <= 0 Then
                x_AnnoCont = 0
                x_Ric_Cod = 0
                x_Cod_Conto = 0
            Else
                If Me.Cmb_AnnoContabile.SelectedIndex <= 0 Then
                    Log += "Selezionare l'anno contabile!" & vbCrLf
                Else
                    x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue
                End If
                If Me.Cmb_Riclassificazione.SelectedIndex <= 0 Then
                    Log += "Selezionare la riclassificazione!" & vbCrLf
                Else
                    x_Ric_Cod = Me.Cmb_Riclassificazione.SelectedValue
                End If
                If Me.Cmb_Conti.SelectedIndex <= 0 Then
                    Log += "Selezionare il conto!" & vbCrLf & vbCrLf
                Else
                    x_Cod_Conto = Me.Cmb_Conti.SelectedValue
                End If
            End If

            Calcola_Date_InizioFine(Data_Inizio, Data_Fine, Log, False)

            If Log <> "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Log, Page)
                Exit Sub
            End If

            Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab

            Dt_Query = objStampe.LiquidazioneIVA_2(Piva, _
                                                    Sezionale_Cod, _
                                                    x_Cod_Conto, x_Ric_Cod, x_AnnoCont, _
                                                    Data_Inizio, Data_Fine, _
                                                    objParametri_Server)

            If Not IsNothing(Dt_Query) AndAlso Dt_Query.Rows.Count > 0 Then

                DT_IVA_Vendite_Generale = objSottoIva.CaricaGriglia_DtIvaGenerale()
                DT_IVA_Acquisti_Generale = objSottoIva.CaricaGriglia_DtIvaGenerale()

                Try
                    DT_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

                Catch ex As Exception
                    Throw New Exception("DT_Dettagli_Round, errore nella creazione: " + vbCrLf + ex.Message)
                End Try

                Try
                    DT_IVA_Round = objLanRound.CaricaGriglia_DtIva

                Catch ex As Exception
                    Throw New Exception("DT_IVA_Round, errore nella creazione: " + vbCrLf + ex.Message)
                End Try

                For i = 0 To Dt_Query.Rows.Count - 1

                    Lav_Cod = Dt_Query.Rows(i).Item("Lav_Cod")
                    Id_Agenda = Dt_Query.Rows(i).Item("Id_Agenda")

                    Select Case Lav_Cod
                        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                            'in questo caso devo sottrarre l'importo della nota di accredito
                            'moltiplico * -1  il num_protocollo
                            'se no non torna il totale
                            'Num_Protocollo = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(Num_Protocollo * -1)
                            'DT_Query.Rows(i).Item("Num_Protocollo") = Num_Protocollo
                            Dt_Query.Rows(i).Item("Prezzo_Unitario") = -1 * Dt_Query.Rows(i).Item("Prezzo_Unitario")
                            Dt_Query.Rows(i).Item("Prezzo_Unitario_Netto") = -1 * Dt_Query.Rows(i).Item("Prezzo_Unitario_Netto")
                    End Select

                    Sconto_Modalita = Dt_Query.Rows(i).Item("Sconto_Modalita")
                    iva_indetraibile_perc = Dt_Query.Rows(i).Item("iva_indetraibile_perc")
                    iva_indetraibile = Dt_Query.Rows(i).Item("iva_indetraibile")

                    Edit_Importo = Dt_Query.Rows(i).Item("Tipo_Sconto")

                    des_Lib = Dt_Query.Rows(i).Item("Des_Lib")
                    Mov_Det_Des = Dt_Query.Rows(i).Item("Mov_Det_Des")
                    Data_Movimento = Dt_Query.Rows(i).Item("Data_Movimento")
                    prezzo_unitario = Dt_Query.Rows(i).Item("Prezzo_Unitario")
                    qta = Dt_Query.Rows(i).Item("Qta")

                    Cod_Iva = CInt(Dt_Query.Rows(i).Item("Cod_Iva"))
                    Aliquota = Dt_Query.Rows(i).Item("Aliquota")
                    sigla_iva = Dt_Query.Rows(i).Item("Sigla_Iva")

                    ' IVA = Arrotonda_2Decimali(CDbl(Dt.Rows(i).Item("Iva")))
                    IVA = CDbl(Dt_Query.Rows(i).Item("Iva"))

                    '''DA NON FARE!!!! DOPO VIENE FATTO UN CONTROLLO SUL SEGNO DELL'IVA
                    '''IVA = Leggi_IVA_PositivaNegativa(Dt.Rows(i).Item("Lav_Cod"), IVA)

                    'If CDbl(Dt.Rows(i).Item("Imponibile_Netto")) <> 0 Then
                    '    Imponibile = Arrotonda_2Decimali(CDbl(Dt.Rows(i).Item("Imponibile_Netto")))
                    'Else
                    '    Imponibile = Arrotonda_2Decimali(CDbl(Dt.Rows(i).Item("Imponibile")))
                    'End If

                    Imponibile_netto = CDbl(Dt_Query.Rows(i).Item("Imponibile_Netto"))

                    ''DA NON FARE!!!! DOPO VIENE FATTO UN CONTROLLO SUL SEGNO DELL'IMPONIBILE
                    ''Imponibile = Leggi_Imponibile_PositivoNegativo(Dt.Rows(i).Item("Lav_Cod"), Imponibile)

                    If (IVA < 0 And Imponibile_netto < 0) Or (IVA > 0 And Imponibile_netto > 0) Then
                        'ERRORE
                        Log += "ERRORE. Operazione: " + CStr(des_Lib) + ", data " + CStr(Data_Movimento) + _
                        ", dettaglio: " + CStr(Mov_Det_Des) + ", quantità: " + CStr(qta) + _
                        ", prezzo unitario lordo: " + CStr(prezzo_unitario) + " euro, l'IVA e l'Imponibile hanno lo stesso segno, pertanto i dati sono incongruenti!" & vbCrLf + _
                        "---> Salvare nuovamente il documento e, se il problema persiste, contattare l'amministratore." & vbCrLf & vbCrLf

                    End If

                    'modifica del 08/10/2015: sostituito cod_iva con aliquota
                    'se codice iva valorizzato, ma iva 0 e non sconto merce/omaggio e non indetraibile al 100%, mando avviso
                    'If Cod_Iva < 22 And Cod_Iva > 1 And IVA = 0 And Sconto_Modalita = 0 And iva_indetraibile_perc <> 100 Then
                    If Aliquota <> 0 And IVA = 0 And Sconto_Modalita = 0 And iva_indetraibile_perc <> 100 Then
                        'ERRORE
                        Log += "ERRORE. Operazione: " + CStr(des_Lib) + ", data " + CStr(Data_Movimento) + _
                        ", dettaglio: " + CStr(Mov_Det_Des) + ", quantità: " + CStr(qta) + _
                        ", prezzo unitario lordo: " + CStr(prezzo_unitario) + " euro, ha l'aliquota IVA del " & CStr(sigla_iva) & " ma l'imposta è 0 " & _
                        vbCrLf & "---> Entrare in modifica del documento, correggere gli errori di inserimento e risalvare!" & vbCrLf & vbCrLf
                    End If

                    'IVA = Arrotonda_2Decimali(objContabHLP.Leggi_IVA_PositivaNegativa2(Lav_Cod, IVA))

                    'Imponibile = Arrotonda_2Decimali(objContabHLP.Leggi_Imponibile_PositivoNegativo2(Lav_Cod, Imponibile))

                    '-----------------------------------
                    Select Case Lav_Cod

                        Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI

                            'Tot_Saldo_Credito += IVA

                            NumDettagli_Mov_Credito += 1

                            NumDettagli_Fatture_Ricevute += 1

                        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA

                            ''modifica del 15/02/2012: le note sottraggono l'iva a quella delle fatture

                            ''Tot_Saldo_Debito += IVA
                            'Tot_Saldo_Credito += IVA

                            'NumDettagli_Mov_Debito += 1
                            NumDettagli_Mov_Credito += 1

                            NumDettagli_ResiAbbuoni_Acquisti += 1


                        Case LAVCOD_ACQUISTO

                            '    Tot_Saldo_Credito += IVA

                            NumDettagli_Mov_Credito += 1

                            NumDettagli_Corrispettivi_Acquisto += 1

                        Case LAVCOD_ALTRI_COSTI

                            '   Tot_Saldo_Credito += IVA

                            NumDettagli_Mov_Credito += 1

                            NumDettagli_AltriCosti += 1


                            '#########################


                        Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA

                            '  Tot_Saldo_Debito += IVA

                            NumDettagli_Mov_Debito += 1

                            NumDettagli_Fatture_Emesse += 1

                        Case LAVCOD_NOTA_ACCREDITO_EMESSA

                            ''modifica del 15/02/2012: le note sottraggono l'iva a quella delle fatture

                            ''Tot_Saldo_Credito += IVA
                            'Tot_Saldo_Debito += IVA

                            'NumDettagli_Mov_Credito += 1
                            NumDettagli_Mov_Debito += 1

                            NumDettagli_ResiAbbuoni_Vendite += 1

                        Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO

                            'inserita il 21/08/2012
                            NumDettagli_Mov_Debito += 1

                            NumDettagli_AutoConsumo += 1

                        Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO

                            ' Tot_Saldo_Debito += IVA

                            NumDettagli_Mov_Debito += 1

                            NumDettagli_Corrispettivi_Vendita += 1

                        Case LAVCOD_RICEVUTA_EMESSA

                            '  Tot_Saldo_Debito += IVA

                            NumDettagli_Mov_Debito += 1

                            NumDettagli_RicevuteFiscali_Emesse += 1

                        Case LAVCOD_DDT_CONTABILIZZATO_EMESSO

                            'inserita il 21/08/2012
                            NumDettagli_Mov_Debito += 1

                            NumDettagli_DDTContabilizzati_Emessi += 1

                        Case LAVCOD_ALTRI_RICAVI

                            ' Tot_Saldo_Debito += IVA

                            NumDettagli_Mov_Debito += 1

                            NumDettagli_AltriRicavi += 1

                        Case Else
                            Log += "Gestire causale: " + CStr(Lav_Cod)

                    End Select 'lav_cod

                    '===================================================


                    'dal secondo giro
                    If id_agenda_memo = Id_Agenda Then

                        'stessa operazione
                        DtDettRound_InserisciDettaglio(objContabHLP, _
                                                        objLanRound, _
                                                        Lav_Cod, _
                                                        DT_Dettagli_Round, _
                                                        Dt_Query.Rows(i))

                    Else

                        'nuova operazione

                        'se non sono al primo giro
                        'devo elaborare il gruppo di dettagli dell'operazione precedente
                        'e inserire le righe nel dataset
                        If id_agenda_memo <> 0 Then
                            ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
                                                            DT_Dettagli_Round, _
                                                            DT_IVA_Round, _
                                                            Flag_Vendite_memo, _
                                                            Edit_Importo_memo)

                            If Flag_Vendite_memo = True And Flag_Acquisti_memo = False Then
                                objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Vendite_Generale, Flag_Acquisti_memo)
                            End If
                            If Flag_Vendite_memo = False And Flag_Acquisti_memo = True Then
                                objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Acquisti_Generale, Flag_Acquisti_memo)
                            End If

                        End If

                        'ad ogni operazione creo un nuovo dt_dettagli
                        DT_Dettagli_Round.Clear()
                        'svuoto anche dt_iva che va popolato al termine
                        DT_IVA_Round.Clear()

                        If DT_Dettagli_Round.Rows.Count <> 0 Then
                            Throw New Exception("DT_Dettagli_Round.Rows.Count <> 0 --> non deve succedere, controllo ")
                        End If
                        If DT_IVA_Round.Rows.Count <> 0 Then
                            Throw New Exception("DT_IVA_Round.Rows.Count <> 0 --> non deve succedere, controllo ")
                        End If

                        'salvo l'id_agenda
                        id_agenda_memo = Id_Agenda
                        lav_cod_memo = Lav_Cod
                        Edit_Importo_memo = Edit_Importo

                        Select Case lav_cod_memo

                            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI, _
                                 LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ACQUISTO, LAVCOD_ALTRI_COSTI

                                Flag_Vendite_memo = False
                                Flag_Acquisti_memo = True
                                ' Flag_Vendita = False

                            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA, _
                                LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, _
                                LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO, LAVCOD_RICEVUTA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_ALTRI_RICAVI

                                Flag_Vendite_memo = True
                                Flag_Acquisti_memo = False
                                'Flag_Vendita = True

                            Case Else
                                Log += "lav_cod_memo non gestito."

                        End Select

                        DtDettRound_InserisciDettaglio(objContabHLP, _
                                                            objLanRound, _
                                                            Lav_Cod, _
                                                            DT_Dettagli_Round, _
                                                           Dt_Query.Rows(i))


                        'contatori
                        Select Case Lav_Cod

                            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI
                                Num_Mov_Credito += 1
                                Num_Fatture_Ricevute += 1

                            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                'Num_Mov_Debito += 1
                                Num_Mov_Credito += 1
                                Num_ResiAbbuoni_Acquisti += 1

                            Case LAVCOD_ACQUISTO
                                Num_Mov_Credito += 1
                                Num_Corrispettivi_Acquisto += 1

                            Case LAVCOD_ALTRI_COSTI
                                Num_Mov_Credito += 1
                                Num_AltriCosti += 1

                                '######################################

                            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
                                Num_Mov_Debito += 1
                                Num_Fatture_Emesse += 1

                            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                'Num_Mov_Credito += 1
                                Num_Mov_Debito += 1
                                Num_ResiAbbuoni_Vendite += 1

                            Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO
                                Num_Mov_Debito += 1
                                Num_AutoConsumo += 1

                            Case LAVCOD_RICEVUTA_EMESSA
                                Num_Mov_Debito += 1
                                Num_RicevuteFiscali_Emesse += 1

                            Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO
                                Num_Mov_Debito += 1
                                Num_Corrispettivi_Vendita += 1

                            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                                Num_Mov_Debito += 1
                                Num_DDTContabilizzati_Emessi += 1

                            Case LAVCOD_ALTRI_RICAVI
                                Num_Mov_Debito += 1
                                Num_AltriRicavi += 1

                            Case Else
                                Log += "Gestire causale: " + CStr(Lav_Cod)

                        End Select 'lav_cod

                    End If 'id_agenda_memo = Id_Agenda

                Next 'tutti i dettagli

                'per l'ultima operazione
                ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
                                                DT_Dettagli_Round, _
                                                DT_IVA_Round, _
                                                Flag_Vendite_memo, _
                                                edit_importo_memo)

                If Flag_Vendite_memo = True And Flag_Acquisti_memo = False Then
                    objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Vendite_Generale, Flag_Acquisti_memo)
                End If
                If Flag_Vendite_memo = False And Flag_Acquisti_memo = True Then
                    objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Acquisti_Generale, Flag_Acquisti_memo)
                End If


            End If

            '########################################################################

            ' ----------------------- DATATABLE CREDITO -----------------------------

            Session("DT_IVA_Acquisti_Generale") = DT_IVA_Acquisti_Generale
            Me.DataGrid_Credito.DataSource = DT_IVA_Acquisti_Generale
            Me.DataGrid_Credito.DataBind()

            ' ----------------------------------------------------------------------

            '########################################################################

            ' ----------------------- DATATABLE DEBITO -----------------------------

            Session("DT_IVA_Vendite_Generale") = DT_IVA_Vendite_Generale
            Me.DataGrid_Debito.DataSource = DT_IVA_Vendite_Generale
            Me.DataGrid_Debito.DataBind()


            ' ----------------------------------------------------------------------

            Me.Txt_NumMovCredito.Text = Num_Mov_Credito
            Me.Txt_NumFattureRicevute.Text = Num_Fatture_Ricevute
            Me.Txt_NumResiAcquisti.Text = Num_ResiAbbuoni_Acquisti 'resi su acquisti
            Me.Txt_NumAcquisti.Text = Num_Corrispettivi_Acquisto
            Me.Txt_NumAltriCosti.Text = Num_AltriCosti

            Me.Txt_NumMovDebito.Text = Num_Mov_Debito
            Me.Txt_NumFattureEmesse.Text = Num_Fatture_Emesse
            Me.Txt_NumResiVendite.Text = Num_ResiAbbuoni_Vendite 'resi su vendite
            Me.Txt_NumAutoconsumo.Text = Num_AutoConsumo
            Me.Txt_NumRicevuteFiscaliEmesse.Text = Num_RicevuteFiscali_Emesse
            Me.Txt_NumVendite.Text = Num_Corrispettivi_Vendita
            Me.Txt_NumDDTContabilizzati_Emessi.Text = Num_DDTContabilizzati_Emessi
            Me.Txt_NumAltriRicavi.Text = Num_AltriRicavi

            Tot_Saldo_Credito = Calcola_Totale_Iva(DT_IVA_Acquisti_Generale)
            Tot_Saldo_Debito = Calcola_Totale_Iva(DT_IVA_Vendite_Generale)

            '  Dim Saldo As Double = 0
            ' Dim Acconto As Double

            'If Me.Txt_Acconto.Text = "" Or Not IsNumeric(Me.Txt_Acconto.Text) Then
            '    Acconto = 0
            'Else
            '    Acconto = Me.Txt_Acconto.Text
            'End If

            Me.Txt_Credito.Text = Format(Tot_Saldo_Credito, "##,###,##0.00")
            Me.Txt_TotCredito.Text = Format(Tot_Saldo_Credito, "##,###,##0.00")

            Me.Txt_Debito.Text = Format(Tot_Saldo_Debito, "##,###,##0.00")
            Me.Txt_TotDebito.Text = Format(Tot_Saldo_Debito, "##,###,##0.00")

            'Saldo = Calcola_Saldo(Tot_Saldo_Credito, _
            '                        Tot_Saldo_Debito, _
            '                        0, _
            '                        Acconto)

            'va chiamata dopo aver valorizzato le txt credito e debito
            '  Saldo = Verifica_Calcola_Saldo()
            '   Me.Txt_Differenza.Text = Format(Saldo, "##,###,##0.00")
            Verifica_Calcola_Saldo(Log)

            If Me.Chk_IVA_Precedente.Checked = True Then
                Calcola_IVA_Precedente()
            End If

            If Log <> "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Log, Page)
            End If

        Catch exc As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Errore nel caricamento dei dati: " + exc.Message, Page)
        End Try


    End Sub

    Private Function Calcola_Totale_Iva(ByVal DT_IVA_Generale As DataTable) As Double

        Dim Totale As Double = 0
        Dim i As Integer
        Dim iva As Double

        If Not IsNothing(DT_IVA_Generale) Then

            For i = 0 To DT_IVA_Generale.Rows.Count - 1

                iva = DT_IVA_Generale.Rows(i).Item("iva")

                Totale = Totale + iva

            Next

        End If

        Return Totale

    End Function

    '####################################################################################
    Private Sub Btn_CalcolaSaldo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcolaSaldo.Click
        Dim str_errore As String = ""
        Verifica_Calcola_Saldo(str_errore)
    End Sub

    Private Sub Verifica_Calcola_Saldo(ByRef str_errore As String)

        Dim Saldo As Double = 0
        Dim SaldoFinale As Double = 0
        Dim Acconto, Credito, Debito, CreditoAnnoPrec, InteresseDebito_Perc, InteresseDebito_Valore As Double

        VerificaTxt()

        'If Me.Txt_Acconto.Text = "" Or Not IsNumeric(Me.Txt_Acconto.Text) Then
        '    Acconto = 0
        'Else
        Acconto = Me.Txt_Acconto.Text
        'End If

        'If Me.Txt_Credito.Text = "" Or Not IsNumeric(Me.Txt_Credito.Text) Then
        '    Credito = 0
        'Else
        Credito = Me.Txt_Credito.Text
        'End If

        'If Me.Txt_Debito.Text = "" Or Not IsNumeric(Me.Txt_Debito.Text) Then
        '    Debito = 0
        'Else
        Debito = Me.Txt_Debito.Text
        'End If

        'If Me.Txt_IVA_Precedente.Text = "" Or Not IsNumeric(Me.Txt_IVA_Precedente.Text) Then
        '    CreditoAnnoPrec = 0
        'Else
        CreditoAnnoPrec = Me.Txt_IVA_Precedente.Text
        'End If

        InteresseDebito_Perc = Me.Txt_InteresseDebito_Perc.Text

        If Acconto < 0 Then
            str_errore = "L'acconto deve essere un valore positivo."
            Acconto = 0
            Me.Txt_Acconto.Text = "0"
            Exit Sub
        End If

        Saldo = Calcola_Saldo(Credito, _
                               Debito, _
                              CreditoAnnoPrec, _
                                Acconto)

        Me.Txt_Differenza.Text = Format(Saldo, "##,###,##0.00")

        Calcola_SaldoFINALE(Saldo, _
                            InteresseDebito_Perc, _
                            InteresseDebito_Valore, _
                            SaldoFinale)

        Me.Txt_InteresseDebito_Valore.Text = Format(InteresseDebito_Valore, "##,###,##0.00")
        Me.Txt_ImpostaDaVersare.Text = Format(SaldoFinale, "##,###,##0.00")

        '   Return Saldo

    End Sub

    Private Sub VerificaTxt()

        'ByRef TxtCredito As System.Web.UI.WebControls.TextBox, _
        '                    ByRef TxtDebito As System.Web.UI.WebControls.TextBox, _
        '                    ByRef TxtAcconto As System.Web.UI.WebControls.TextBox, _
        '                    ByRef TxtSaldoPrecedente As System.Web.UI.WebControls.TextBox

        If Me.Txt_Acconto.Text = "" Or Not IsNumeric(Me.Txt_Acconto.Text) Then
            Me.Txt_Acconto.Text = "0"
        End If

        If Me.Txt_Credito.Text = "" Or Not IsNumeric(Me.Txt_Credito.Text) Then
            Me.Txt_Credito.Text = "0"
        End If

        If Me.Txt_Debito.Text = "" Or Not IsNumeric(Me.Txt_Debito.Text) Then
            Me.Txt_Debito.Text = "0"
        End If

        If Me.Txt_IVA_Precedente.Text = "" Or Not IsNumeric(Me.Txt_IVA_Precedente.Text) Then
            Me.Txt_IVA_Precedente.Text = "0"
        End If

        If Me.Txt_InteresseDebito_Perc.Text = "" Or Not IsNumeric(Me.Txt_InteresseDebito_Perc.Text) Then
            Me.Txt_InteresseDebito_Perc.Text = "0"
        End If


    End Sub

    Private Function Calcola_Saldo(ByVal Iva_Credito As Double, _
                                        ByVal Iva_Debito As Double, _
                                        ByVal Iva_Credito_AnnoPrecedente As Double, _
                                        ByVal Acconto_Versato As Double) As Double

        Dim Saldo As Double = 0

        Saldo = Iva_Credito + Iva_Credito_AnnoPrecedente + Acconto_Versato - Iva_Debito

        Return Saldo

    End Function

    Private Sub Calcola_SaldoFINALE(ByVal Saldo As Double, _
                                    ByVal InteresseDebito_Perc As Double, _
                                    ByRef InteresseDebito_Valore As Double, _
                                    ByRef SaldoFinale As Double)

        SaldoFinale = 0
        InteresseDebito_Valore = 0

        'se c'è iva a debito
        If Saldo < 0 Then
            ' Saldo = Saldo * -1
            InteresseDebito_Valore = Saldo * InteresseDebito_Perc / 100
            InteresseDebito_Valore = AgronicaCoreDataProvider.Agro_Math.RoundNumber_2Decimali(InteresseDebito_Valore)
            SaldoFinale = (Saldo + InteresseDebito_Valore) '* -1
        End If

    End Sub



    '########################################################################
    Private Sub DtDettRound_InserisciDettaglio(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                                    ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                    ByVal Lav_Cod As Integer, _
                                                    ByRef DT_Dettagli_Round As DataTable, _
                                                    ByRef DrQuery As DataRow)

        Dim modalita_doc As enum_ModalitaFattura

        With DrQuery

            If .Item("Modalita") = enum_ModalitaFattura.Fattura_AcquistiIntracom Then
                modalita_doc = enum_ModalitaFattura.Fattura_AcquistiIntracom
            Else
                'le altre modalità le devo far viaggiare insieme
                modalita_doc = enum_ModalitaFattura.NonDefinito
            End If

            objLanRound.InserisciRiga_DtDettagli(DT_Dettagli_Round, _
                                                .Item("Id_Mov_Det"), _
                                                .Item("ChkLayOut_Hide"), _
                                                 .Item("Sconto_Modalita"), _
                                                 .Item("Sconto"), _
                                                 .Item("Sconto_listino"), _
                                                .Item("qta"), _
                                                .Item("Prezzo_Unitario"), _
                                                .Item("Prezzo_Unitario_Netto"), _
                                                .Item("Imponibile"), _
                                                .Item("Imponibile_Netto"), _
                                                .Item("Cod_IVA"), _
                                                .Item("IVA"), _
                                                .Item("Aliquota"), _
                                                .Item("Sigla_IVA"), _
                                                0, _
                                                .Item("Iva_Indetraibile"), _
                                                .Item("Iva_Indetraibile_Perc"), _
                                                modalita_doc)

        End With

    End Sub

    '########################################################################
    Private Sub ElaboraDettagli_x_Riepilogo_IVA(ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                ByVal DT_Dettagli_Round As DataTable, _
                                                ByRef DT_IVA_Round As DataTable, _
                                                ByVal Flag_Vendita As Boolean, _
                                                ByVal Edit_Importo As enum_EditImporto)

        'questi servono per il riepilogo a fine fattura, non servono quindi in questo report
        Dim Riepilogo_ImponibileLordo As Double = 0
        Dim Riepilogo_Variazioni As Double = 0
        Dim Riepilogo_ImponibileNetto As Double = 0
        Dim Riepilogo_Imposta As Double = 0
        Dim Riepilogo_Importo As Double = 0

        DT_IVA_Round = objLanRound.FormAggiornaImporto(objParametri_Server, _
                                                        DT_Dettagli_Round, _
                                                        Riepilogo_ImponibileLordo, _
                                                        Riepilogo_Variazioni, _
                                                        Riepilogo_ImponibileNetto, _
                                                        Riepilogo_Imposta, _
                                                        Riepilogo_Importo, _
                                                        Edit_Importo, _
                                                        True, _
                                                        Flag_Vendita)
        'enum_TipoSconto.PrezzoUnitario

    End Sub

    '####################################################################################
    Private Sub CaricaDati_OLD()

        'Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita

        'Dim Sezionale_Cod As Integer

        'Dim Dt As DataTable
        'Dim Dt_Credito As New DataTable
        'Dim Dr_Credito As DataRow
        'Dim Dt_Debito As New DataTable
        'Dim Dr_Debito As DataRow

        'Dim i As Integer
        'Dim Data_Inizio, Data_Fine As String
        'Dim Filtro, Ordinamento As String
        'Dim Log As String = ""

        ''IVA A CREDITO
        'Dim NumDettagli_Mov_Credito As Integer = 0
        'Dim NumDettagli_Fatture_Ricevute As Integer = 0
        'Dim NumDettagli_ResiAbbuoni_Acquisti As Integer = 0
        'Dim NumDettagli_Corrispettivi_Acquisto As Integer = 0
        'Dim NumDettagli_AltriCosti As Integer = 0

        'Dim Num_Mov_Credito As Integer = 0
        'Dim Num_Fatture_Ricevute As Integer = 0
        'Dim Num_ResiAbbuoni_Acquisti As Integer = 0
        'Dim Num_Corrispettivi_Acquisto As Integer = 0
        'Dim Num_AltriCosti As Integer = 0

        ''IVA A DEBITO
        'Dim NumDettagli_Mov_Debito As Integer = 0
        'Dim NumDettagli_Fatture_Emesse As Integer = 0
        'Dim NumDettagli_ResiAbbuoni_Vendite As Integer = 0
        'Dim NumDettagli_AutoConsumo As Integer = 0
        'Dim NumDettagli_Corrispettivi_Vendita As Integer = 0
        'Dim NumDettagli_RicevuteFiscali_Emesse As Integer = 0
        'Dim NumDettagli_DDTContabilizzati_Emessi As Integer = 0
        'Dim NumDettagli_AltriRicavi As Integer = 0

        'Dim Num_Mov_Debito As Integer = 0
        'Dim Num_Fatture_Emesse As Integer = 0
        'Dim Num_ResiAbbuoni_Vendite As Integer = 0
        'Dim Num_AutoConsumo As Integer = 0
        'Dim Num_Corrispettivi_Vendita As Integer = 0
        'Dim Num_RicevuteFiscali_Emesse As Integer = 0
        'Dim Num_DDTContabilizzati_Emessi As Integer = 0
        'Dim Num_AltriRicavi As Integer = 0

        'Dim Tot_Saldo_Credito As Double = 0
        'Dim Tot_Saldo_Debito As Double = 0

        'Dim IVA As Double = 0
        'Dim Imponibile As Double = 0
        'Dim Cod_Iva As Integer = 0
        'Dim Id_Agenda_Temp As Integer = 0
        'Dim Id_Agenda As Integer
        'Dim Lav_Cod As Integer
        'Dim Sconto_Modalita As Integer
        'Dim Data_Movimento, des_Lib, Mov_Det_Des, sigla_iva As String
        'Dim prezzo_unitario, qta As Double

        'Dim Imponibile_Credito_4 As Double = 0
        'Dim Imponibile_Credito_10 As Double = 0
        'Dim Imponibile_Credito_12 As Double = 0
        'Dim Imponibile_Credito_20 As Double = 0
        'Dim Imponibile_Credito_21 As Double = 0
        'Dim Imponibile_Credito_NoIva As Double = 0

        'Dim Imponibile_Debito_4 As Double = 0
        'Dim Imponibile_Debito_10 As Double = 0
        'Dim Imponibile_Debito_12 As Double = 0
        'Dim Imponibile_Debito_20 As Double = 0
        'Dim Imponibile_Debito_21 As Double = 0
        'Dim Imponibile_Debito_NoIva As Double = 0

        'Dim Iva_Credito_4 As Double = 0
        'Dim Iva_Credito_10 As Double = 0
        'Dim Iva_Credito_12 As Double = 0
        'Dim Iva_Credito_20 As Double = 0
        'Dim Iva_Credito_21 As Double = 0
        'Dim Iva_Credito_NoIva As Double = 0

        'Dim Iva_Debito_4 As Double = 0
        'Dim Iva_Debito_10 As Double = 0
        'Dim Iva_Debito_12 As Double = 0
        'Dim Iva_Debito_20 As Double = 0
        'Dim Iva_Debito_21 As Double = 0
        'Dim Iva_Debito_NoIva As Double = 0

        ''Dim Imponibile_Credito_FCI As Double = 0
        ''Dim Imponibile_Credito_NO_IVABILE As Double = 0
        ''Dim Imponibile_Credito_61 As Double = 0
        ''Dim Imponibile_Credito_63 As Double = 0
        ''Dim Imponibile_Credito_64 As Double = 0
        ''Dim Imponibile_Credito_65 As Double = 0
        ''Dim Imponibile_Credito_66 As Double = 0
        ''Dim Imponibile_Credito_67 As Double = 0
        ''Dim Imponibile_Credito_68 As Double = 0
        ''Dim Imponibile_Credito_69 As Double = 0
        ''Dim Imponibile_Credito_70 As Double = 0
        ''Dim Imponibile_Credito_71 As Double = 0
        ''Dim Imponibile_Credito_72 As Double = 0
        ''Dim Imponibile_Credito_73 As Double = 0
        ''Dim Imponibile_Credito_74 As Double = 0
        ''Dim Imponibile_Credito_75 As Double = 0
        ''Dim Imponibile_Credito_76 As Double = 0
        ''Dim Imponibile_Credito_77 As Double = 0
        ''Dim Imponibile_Credito_78 As Double = 0

        ''Dim IVA_Credito_FCI As Double = 0
        ''Dim IVA_Credito_NO_IVABILE As Double = 0
        ''Dim IVA_Credito_61 As Double = 0
        ''Dim IVA_Credito_63 As Double = 0
        ''Dim IVA_Credito_64 As Double = 0
        ''Dim IVA_Credito_65 As Double = 0
        ''Dim IVA_Credito_66 As Double = 0
        ''Dim IVA_Credito_67 As Double = 0
        ''Dim IVA_Credito_68 As Double = 0
        ''Dim IVA_Credito_69 As Double = 0
        ''Dim IVA_Credito_70 As Double = 0
        ''Dim IVA_Credito_71 As Double = 0
        ''Dim IVA_Credito_72 As Double = 0
        ''Dim IVA_Credito_73 As Double = 0
        ''Dim IVA_Credito_74 As Double = 0
        ''Dim IVA_Credito_75 As Double = 0
        ''Dim IVA_Credito_76 As Double = 0
        ''Dim IVA_Credito_77 As Double = 0
        ''Dim IVA_Credito_78 As Double = 0

        ''Dim Imponibile_Debito_FCI As Double = 0
        ''Dim Imponibile_Debito_NO_IVABILE As Double = 0
        ''Dim Imponibile_Debito_61 As Double = 0
        ''Dim Imponibile_Debito_63 As Double = 0
        ''Dim Imponibile_Debito_64 As Double = 0
        ''Dim Imponibile_Debito_65 As Double = 0
        ''Dim Imponibile_Debito_66 As Double = 0
        ''Dim Imponibile_Debito_67 As Double = 0
        ''Dim Imponibile_Debito_68 As Double = 0
        ''Dim Imponibile_Debito_69 As Double = 0
        ''Dim Imponibile_Debito_70 As Double = 0
        ''Dim Imponibile_Debito_71 As Double = 0
        ''Dim Imponibile_Debito_72 As Double = 0
        ''Dim Imponibile_Debito_73 As Double = 0
        ''Dim Imponibile_Debito_74 As Double = 0
        ''Dim Imponibile_Debito_75 As Double = 0
        ''Dim Imponibile_Debito_76 As Double = 0
        ''Dim Imponibile_Debito_77 As Double = 0
        ''Dim Imponibile_Debito_78 As Double = 0

        ''Dim IVA_Debito_FCI As Double = 0
        ''Dim IVA_Debito_NO_IVABILE As Double = 0
        ''Dim IVA_Debito_61 As Double = 0
        ''Dim IVA_Debito_63 As Double = 0
        ''Dim IVA_Debito_64 As Double = 0
        ''Dim IVA_Debito_65 As Double = 0
        ''Dim IVA_Debito_66 As Double = 0
        ''Dim IVA_Debito_67 As Double = 0
        ''Dim IVA_Debito_68 As Double = 0
        ''Dim IVA_Debito_69 As Double = 0
        ''Dim IVA_Debito_70 As Double = 0
        ''Dim IVA_Debito_71 As Double = 0
        ''Dim IVA_Debito_72 As Double = 0
        ''Dim IVA_Debito_73 As Double = 0
        ''Dim IVA_Debito_74 As Double = 0
        ''Dim IVA_Debito_75 As Double = 0
        ''Dim IVA_Debito_76 As Double = 0
        ''Dim IVA_Debito_77 As Double = 0
        ''Dim IVA_Debito_78 As Double = 0

        ''=================================================================

        'Sezionale_Cod = Me.Cmb_Sezionali.SelectedValue

        'If Me.Cmb_AnnoContabile.SelectedIndex <= 0 And _
        'Me.Cmb_Riclassificazione.SelectedIndex <= 0 And _
        'Me.Cmb_Conti.SelectedIndex <= 0 Then
        '    x_AnnoCont = 0
        '    x_Ric_Cod = 0
        '    x_Cod_Conto = 0
        'Else
        '    If Me.Cmb_AnnoContabile.SelectedIndex <= 0 Then
        '        Log += "Selezionare l'anno contabile!" & vbCrLf
        '    Else
        '        x_AnnoCont = Me.Cmb_AnnoContabile.SelectedValue
        '    End If
        '    If Me.Cmb_Riclassificazione.SelectedIndex <= 0 Then
        '        Log += "Selezionare la riclassificazione!" & vbCrLf
        '    Else
        '        x_Ric_Cod = Me.Cmb_Riclassificazione.SelectedValue
        '    End If
        '    If Me.Cmb_Conti.SelectedIndex <= 0 Then
        '        Log += "Selezionare il conto!" & vbCrLf & vbCrLf
        '    Else
        '        x_Cod_Conto = Me.Cmb_Conti.SelectedValue
        '    End If
        'End If

        'Calcola_Date_InizioFine(Data_Inizio, Data_Fine, Log, False)

        'If Log <> "" Then
        '    AgroMsgBox(Log, Page)
        '    Exit Sub
        'End If

        'Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab

        'Dt = objStampe.LiquidazioneIVA(Piva, _
        '                                Sezionale_Cod, _
        '                                x_Cod_Conto, x_Ric_Cod, x_AnnoCont, _
        '                                Data_Inizio, Data_Fine, _
        '                                 objParametri_Server)

        'If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

        '    For i = 0 To Dt.Rows.Count - 1

        '        Lav_Cod = Dt.Rows(i).Item("Lav_Cod")
        '        Sconto_Modalita = Dt.Rows(i).Item("Sconto_Modalita")
        '        Cod_Iva = CInt(Dt.Rows(i).Item("Cod_Iva"))

        '        des_Lib = Dt.Rows(i).Item("Des_Lib")
        '        Mov_Det_Des = Dt.Rows(i).Item("Mov_Det_Des")
        '        Data_Movimento = Dt.Rows(i).Item("Data_Movimento")
        '        prezzo_unitario = Dt.Rows(i).Item("Prezzo_Unitario")
        '        qta = Dt.Rows(i).Item("Qta")
        '        sigla_iva = Dt.Rows(i).Item("Sigla_Iva")

        '        IVA = Arrotonda_2Decimali(CDbl(Dt.Rows(i).Item("Iva")))

        '        '''DA NON FARE!!!! DOPO VIENE FATTO UN CONTROLLO SUL SEGNO DELL'IVA
        '        '''IVA = Leggi_IVA_PositivaNegativa(Dt.Rows(i).Item("Lav_Cod"), IVA)

        '        If CDbl(Dt.Rows(i).Item("Imponibile_Netto")) <> 0 Then
        '            Imponibile = Arrotonda_2Decimali(CDbl(Dt.Rows(i).Item("Imponibile_Netto")))
        '        Else
        '            Imponibile = Arrotonda_2Decimali(CDbl(Dt.Rows(i).Item("Imponibile")))
        '        End If

        '        ''DA NON FARE!!!! DOPO VIENE FATTO UN CONTROLLO SUL SEGNO DELL'IMPONIBILE
        '        ''Imponibile = Leggi_Imponibile_PositivoNegativo(Dt.Rows(i).Item("Lav_Cod"), Imponibile)

        '        If (IVA < 0 And Imponibile < 0) Or (IVA > 0 And Imponibile > 0) Then
        '            'ERRORE
        '            Log += "ERRORE. Operazione: " + CStr(des_Lib) + ", data " + CStr(Data_Movimento) + _
        '            ", dettaglio: " + CStr(Mov_Det_Des) + ", quantità: " + CStr(qta) + _
        '            ", prezzo unitario lordo: " + CStr(prezzo_unitario) + " euro, l'IVA e l'Imponibile hanno lo stesso segno, pertanto i dati sono incongruenti!" & vbCrLf + _
        '            "---> Salvare nuovamente il documento e, se il problema persiste, contattare l'amministratore." & vbCrLf & vbCrLf

        '        End If

        '        'se codice iva valorizzato, ma iva 0 e non sconto merce, mando avviso
        '        If Cod_Iva < 22 And Cod_Iva > 1 And IVA = 0 And Sconto_Modalita = 0 Then
        '            'ERRORE
        '            Log += "ERRORE. Operazione: " + CStr(des_Lib) + ", data " + CStr(Data_Movimento) + _
        '            ", dettaglio: " + CStr(Mov_Det_Des) + ", quantità: " + CStr(qta) + _
        '            ", prezzo unitario lordo: " + CStr(prezzo_unitario) + " euro, ha l'aliquota IVA del " & CStr(sigla_iva) & " ma l'imposta è 0 " & _
        '            vbCrLf & "---> Entrare in modifica del documento, correggere gli errori di inserimento e risalvare!" & vbCrLf & vbCrLf
        '        End If

        '        IVA = Arrotonda_2Decimali(objContabHLP.Leggi_IVA_PositivaNegativa2(Lav_Cod, IVA))

        '        Imponibile = Arrotonda_2Decimali(objContabHLP.Leggi_Imponibile_PositivoNegativo2(Lav_Cod, Imponibile))

        '        Select Case Lav_Cod

        '            Case LAVCOD_FATTURA_EMESSA, _
        '             LAVCOD_NOTA_ACCREDITO_EMESSA, _
        '             LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA, _
        '             LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, _
        '                LAVCOD_RICEVUTA_EMESSA, _
        '                LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO, _
        '                LAVCOD_DDT_CONTABILIZZATO_EMESSO, _
        '                LAVCOD_ALTRI_RICAVI

        '                Select Case Cod_Iva

        '                    Case IVA_4
        '                        'la nota accredito emessa sottrae  iva A DEBITO 
        '                        Iva_Debito_4 = Arrotonda_2Decimali(Iva_Debito_4 + IVA)
        '                        Imponibile_Debito_4 = Arrotonda_2Decimali(Imponibile_Debito_4 + Imponibile)
        '                        Tot_Saldo_Debito = Arrotonda_2Decimali(Tot_Saldo_Debito + IVA)

        '                    Case IVA_10
        '                        'la nota accredito emessa sottrae  iva A DEBITO 
        '                        Iva_Debito_10 = Arrotonda_2Decimali(Iva_Debito_10 + IVA)
        '                        Imponibile_Debito_10 = Arrotonda_2Decimali(Imponibile_Debito_10 + Imponibile)
        '                        Tot_Saldo_Debito = Arrotonda_2Decimali(Tot_Saldo_Debito + IVA)

        '                    Case IVA_12
        '                        'la nota accredito emessa sottrae  iva A DEBITO 
        '                        Iva_Debito_12 = Arrotonda_2Decimali(Iva_Debito_12 + IVA)
        '                        Imponibile_Debito_12 = Arrotonda_2Decimali(Imponibile_Debito_12 + Imponibile)
        '                        Tot_Saldo_Debito = Arrotonda_2Decimali(Tot_Saldo_Debito + IVA)

        '                    Case IVA_20
        '                        'la nota accredito emessa sottrae  iva A DEBITO 
        '                        Iva_Debito_20 = Arrotonda_2Decimali(Iva_Debito_20 + IVA)
        '                        Imponibile_Debito_20 = Arrotonda_2Decimali(Imponibile_Debito_20 + Imponibile)
        '                        Tot_Saldo_Debito = Arrotonda_2Decimali(Tot_Saldo_Debito + IVA)

        '                    Case IVA_21
        '                        'la nota accredito emessa sottrae  iva A DEBITO 
        '                        Iva_Debito_21 = Arrotonda_2Decimali(Iva_Debito_21 + IVA)
        '                        Imponibile_Debito_21 = Arrotonda_2Decimali(Imponibile_Debito_21 + Imponibile)
        '                        Tot_Saldo_Debito = Arrotonda_2Decimali(Tot_Saldo_Debito + IVA)

        '                    Case Else 'NON_IVABILE, FCI, ecc
        '                        'la nota accredito emessa sottrae  iva A DEBITO 
        '                        Iva_Debito_NoIva = Arrotonda_2Decimali(Iva_Debito_NoIva + IVA)
        '                        Imponibile_Debito_NoIva = Arrotonda_2Decimali(Imponibile_Debito_NoIva + Imponibile)
        '                        Tot_Saldo_Debito = Arrotonda_2Decimali(Tot_Saldo_Debito + IVA)

        '                End Select

        '                '=======================================================

        '            Case LAVCOD_FATTURA_RICEVUTA, _
        '            LAVCOD_FATTURA_PROFESSIONISTI, _
        '                LAVCOD_NOTA_ACCREDITO_RICEVUTA, _
        '                LAVCOD_ACQUISTO, _
        '                LAVCOD_ALTRI_COSTI

        '                Select Case Cod_Iva

        '                    Case IVA_4
        '                        'la nota accredito ricevuta sottrae  iva A CREDITO 
        '                        Iva_Credito_4 = Arrotonda_2Decimali(Iva_Credito_4 + IVA)
        '                        Imponibile_Credito_4 = Arrotonda_2Decimali(Imponibile_Credito_4 + Imponibile)
        '                        Tot_Saldo_Credito = Arrotonda_2Decimali(Tot_Saldo_Credito + IVA)

        '                    Case IVA_10
        '                        'la nota accredito ricevuta sottrae  iva A CREDITO 
        '                        Iva_Credito_10 = Arrotonda_2Decimali(Iva_Credito_10 + IVA)
        '                        Imponibile_Credito_10 = Arrotonda_2Decimali(Imponibile_Credito_10 + Imponibile)
        '                        Tot_Saldo_Credito = Arrotonda_2Decimali(Tot_Saldo_Credito + IVA)

        '                    Case IVA_12
        '                        'la nota accredito ricevuta sottrae  iva A CREDITO 
        '                        Iva_Credito_12 = Arrotonda_2Decimali(Iva_Credito_12 + IVA)
        '                        Imponibile_Credito_12 = Arrotonda_2Decimali(Imponibile_Credito_12 + Imponibile)
        '                        Tot_Saldo_Credito = Arrotonda_2Decimali(Tot_Saldo_Credito + IVA)

        '                    Case IVA_20
        '                        'la nota accredito ricevuta sottrae  iva A CREDITO 
        '                        Iva_Credito_20 = Arrotonda_2Decimali(Iva_Credito_20 + IVA)
        '                        Imponibile_Credito_20 = Arrotonda_2Decimali(Imponibile_Credito_20 + Imponibile)
        '                        Tot_Saldo_Credito = Arrotonda_2Decimali(Tot_Saldo_Credito + IVA)

        '                    Case IVA_21
        '                        'la nota accredito ricevuta sottrae  iva A CREDITO 
        '                        Iva_Credito_21 = Arrotonda_2Decimali(Iva_Credito_21 + IVA)
        '                        Imponibile_Credito_21 = Arrotonda_2Decimali(Imponibile_Credito_21 + Imponibile)
        '                        Tot_Saldo_Credito = Arrotonda_2Decimali(Tot_Saldo_Credito + IVA)

        '                    Case Else 'NON_IVABILE, FCI, ecc
        '                        'la nota accredito ricevuta sottrae  iva A CREDITO 
        '                        Iva_Credito_NoIva = Arrotonda_2Decimali(Iva_Credito_NoIva + IVA)
        '                        Imponibile_Credito_NoIva = Arrotonda_2Decimali(Imponibile_Credito_NoIva + Imponibile)
        '                        Tot_Saldo_Credito = Arrotonda_2Decimali(Tot_Saldo_Credito + IVA)

        '                End Select

        '                '=====================================================

        '            Case Else
        '                AgroMsgBox("Gestire causale: " + CStr(Lav_Cod), Page)

        '                'tutte le altre causali


        '                '        Case Else 'NON_IVABILE, FCI, ecc

        '                '            If IVA <> 0 Then
        '                '                If Sconto_Modalita = 0 Then
        '                '                    'IVA NON VALORIZZATA
        '                '                    Log += "ERRORE. Operazione: " + CStr(des_lib) + ", data " + CStr(data_movimento) + _
        '                '                    ", dettaglio: " + CStr(Mov_Det_Des) + ", quantità: " + CStr(Qta) + _
        '                '                    ", prezzo lordo: " + CStr(prezzo_unitario) + " euro, è stato selezionato " + objContabHLP.Aliquota_from_CodIVA(Cod_Iva) + ", ma l'imposta è diversa da 0." & vbCrLf & vbCrLf
        '                '                End If

        '                '            Else
        '                '                ''IVA NON VALORIZZATA
        '                '                'Log += "AVVISO. Operazione: " + CStr(des_lib) + ", data " + CStr(data_movimento) + _
        '                '                '", dettaglio: " + CStr(Mov_Det_Des) + ", quantità: " + CStr(Qta) + _
        '                '                '", prezzo lordo: " + CStr(prezzo_unitario) + " euro, non ha valorizzata l'aliquota IVA." & vbCrLf & vbCrLf
        '                '            End If

        '                '            If Imponibile > 0 Then
        '                '                'RICAVO -> IVA A DEBITO
        '                '                Imponibile_Debito_NoIva += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '                Iva_Debito_NoIva += IVA
        '                '            Else
        '                '                'COSTO -> IVA A CREDITO
        '                '                Imponibile_Credito_NoIva += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '                Iva_Credito_NoIva += IVA
        '                '            End If


        '                '            'Select Case Cod_Iva

        '                '            '    Case FCI
        '                '            '        If Imponibile > 0 Then
        '                '            '            'RICAVO -> IVA A DEBITO
        '                '            '            Imponibile_Debito_FCI += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_FCI += IVA
        '                '            '        Else
        '                '            '            'COSTO -> IVA A CREDITO
        '                '            '            Imponibile_Credito_FCI += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_FCI += IVA
        '                '            '        End If

        '                '            '    Case NON_IVABILE
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_NO_IVABILE += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_NO_IVABILE += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_NO_IVABILE += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_NO_IVABILE += IVA
        '                '            '        End If

        '                '            '    Case EsclArt15
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_61 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_61 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_61 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_61 += IVA
        '                '            '        End If

        '                '            '    Case EsArt7
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_63 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_63 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_63 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_63 += IVA
        '                '            '        End If

        '                '            '    Case Art74LettC
        '                '            '        If Imponibile > 0 Then
        '                '            '            'ricavo -> imp debito
        '                '            '            Imponibile_Debito_64 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_64 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_64 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_64 += IVA
        '                '            '        End If

        '                '            '    Case Art74LettE
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_65 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_65 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_65 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_65 += IVA
        '                '            '        End If

        '                '            '    Case NonImpArt9
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_66 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_66 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_66 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_66 += IVA
        '                '            '        End If

        '                '            '    Case NonImpArt8
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_67 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_67 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_67 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_67 += IVA
        '                '            '        End If

        '                '            '    Case NonImpArt40
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_68 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_68 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_68 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_68 += IVA
        '                '            '        End If

        '                '            '    Case EsenteArt10
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_69 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_69 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_69 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_69 += IVA
        '                '            '        End If

        '                '            '    Case EsArt14L537
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_70 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_70 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_70 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_70 += IVA
        '                '            '        End If

        '                '            '    Case Art44comma
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_71 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_71 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_71 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_71 += IVA
        '                '            '        End If

        '                '            '    Case Art74Ter
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_72 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_72 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_72 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_72 += IVA
        '                '            '        End If

        '                '            '    Case NoNImpArt72
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_73 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_73 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_73 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_73 += IVA
        '                '            '        End If

        '                '            '    Case NonImpArt26
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_74 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_74 += Imponibile
        '                '            '        Else
        '                '            '            Imponibile_Credito_74 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_74 += IVA
        '                '            '        End If

        '                '            '    Case EsclusoArt5
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_75 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_75 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_75 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_75 += IVA
        '                '            '        End If

        '                '            '    Case Art3SubA
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_76 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_76 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_76 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_76 += IVA
        '                '            '        End If

        '                '            '    Case EsclArt134
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_77 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_77 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_77 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_77 += IVA
        '                '            '        End If

        '                '            '    Case Art4DL331
        '                '            '        If Imponibile > 0 Then
        '                '            '            Imponibile_Debito_78 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Debito_78 += IVA
        '                '            '        Else
        '                '            '            Imponibile_Credito_78 += objContabHLP.Leggi_Imponibile_PositivoNegativo(Lav_Cod, Imponibile)
        '                '            '            IVA_Credito_78 += IVA
        '                '            '        End If
        '                '            'End Select

        '                'End Select

        '        End Select 'movimenti

        '        '-----------------------------------

        '        ''ERRORE!!! non si fa il valore assoluto, ma si moltiplica x -1
        '        ''IVA = Math.Abs(CDbl(Dt.Rows(i).Item("Iva")))
        '        'IVA = Arrotonda_2Decimali(objContabHLP.Leggi_IVA_PositivaNegativa2(Lav_Cod, CDbl(Dt.Rows(i).Item("Iva"))))

        '        Select Case Lav_Cod

        '            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI

        '                'Tot_Saldo_Credito += IVA

        '                NumDettagli_Mov_Credito += 1

        '                NumDettagli_Fatture_Ricevute += 1

        '            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA

        '                ''modifica del 15/02/2012: le note sottraggono l'iva a quella delle fatture

        '                ''Tot_Saldo_Debito += IVA
        '                'Tot_Saldo_Credito += IVA

        '                'NumDettagli_Mov_Debito += 1
        '                NumDettagli_Mov_Credito += 1

        '                NumDettagli_ResiAbbuoni_Acquisti += 1


        '            Case LAVCOD_ACQUISTO

        '                '    Tot_Saldo_Credito += IVA

        '                NumDettagli_Mov_Credito += 1

        '                NumDettagli_Corrispettivi_Acquisto += 1

        '            Case LAVCOD_ALTRI_COSTI

        '                '   Tot_Saldo_Credito += IVA

        '                NumDettagli_Mov_Credito += 1

        '                NumDettagli_AltriCosti += 1


        '                '#########################


        '            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA

        '                '  Tot_Saldo_Debito += IVA

        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_Fatture_Emesse += 1

        '            Case LAVCOD_NOTA_ACCREDITO_EMESSA

        '                ''modifica del 15/02/2012: le note sottraggono l'iva a quella delle fatture

        '                ''Tot_Saldo_Credito += IVA
        '                'Tot_Saldo_Debito += IVA

        '                'NumDettagli_Mov_Credito += 1
        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_ResiAbbuoni_Vendite += 1

        '            Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO

        '                'inserita il 21/08/2012
        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_AutoConsumo += 1

        '            Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO

        '                ' Tot_Saldo_Debito += IVA

        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_Corrispettivi_Vendita += 1

        '            Case LAVCOD_RICEVUTA_EMESSA

        '                '  Tot_Saldo_Debito += IVA

        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_RicevuteFiscali_Emesse += 1

        '            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO

        '                'inserita il 21/08/2012
        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_DDTContabilizzati_Emessi += 1

        '            Case LAVCOD_ALTRI_RICAVI

        '                ' Tot_Saldo_Debito += IVA

        '                NumDettagli_Mov_Debito += 1

        '                NumDettagli_AltriRicavi += 1

        '            Case Else
        '                AgroMsgBox("Gestire causale: " + CStr(Lav_Cod), Page)

        '        End Select 'lav_cod

        '        '===================================================

        '        Id_Agenda = Dt.Rows(i).Item("Id_Agenda")

        '        'dal secondo giro
        '        If Id_Agenda_Temp = Id_Agenda Then
        '            'è la stessa operazione, non conteggio
        '        Else
        '            'imposto il nuovo temp
        '            Id_Agenda_Temp = Id_Agenda

        '            'sommo il num di doc
        '            Select Case Lav_Cod

        '                Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI
        '                    Num_Mov_Credito += 1
        '                    Num_Fatture_Ricevute += 1

        '                Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
        '                    'Num_Mov_Debito += 1
        '                    Num_Mov_Credito += 1
        '                    Num_ResiAbbuoni_Acquisti += 1

        '                Case LAVCOD_ACQUISTO
        '                    Num_Mov_Credito += 1
        '                    Num_Corrispettivi_Acquisto += 1

        '                Case LAVCOD_ALTRI_COSTI
        '                    Num_Mov_Credito += 1
        '                    Num_AltriCosti += 1

        '                    '######################################

        '                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
        '                    Num_Mov_Debito += 1
        '                    Num_Fatture_Emesse += 1

        '                Case LAVCOD_NOTA_ACCREDITO_EMESSA
        '                    'Num_Mov_Credito += 1
        '                    Num_Mov_Debito += 1
        '                    Num_ResiAbbuoni_Vendite += 1

        '                Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO
        '                    Num_Mov_Debito += 1
        '                    Num_AutoConsumo += 1

        '                Case LAVCOD_RICEVUTA_EMESSA
        '                    Num_Mov_Debito += 1
        '                    Num_RicevuteFiscali_Emesse += 1

        '                Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO
        '                    Num_Mov_Debito += 1
        '                    Num_Corrispettivi_Vendita += 1

        '                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
        '                    Num_Mov_Debito += 1
        '                    Num_DDTContabilizzati_Emessi += 1

        '                Case LAVCOD_ALTRI_RICAVI
        '                    Num_Mov_Debito += 1
        '                    Num_AltriRicavi += 1

        '                Case Else
        '                    AgroMsgBox("Gestire causale: " + CStr(Lav_Cod), Page)

        '            End Select 'lav_cod

        '        End If 'Id_Agenda_Temp = Id_Agenda

        '        'If Id_Agenda_Temp = 0 Then

        '        '    Id_Agenda_Temp = Id_Agenda

        '        '    'è il primo giro, sommo il primo documento
        '        '    'sommo il num di doc
        '        '    Select Case Lav_Cod

        '        '        'Case LAVCOD_ALTRI_COSTI
        '        '        '    Num_Mov_Credito += 1
        '        '        '    Num_AltriCosti += 1

        '        '        'Case LAVCOD_ALTRI_RICAVI
        '        '        '    Num_Mov_Debito += 1
        '        '        '    Num_AltriRicavi += 1

        '        '    Case LAVCOD_FATTURA_RICEVUTA
        '        '            Num_Mov_Credito += 1
        '        '            Num_Fatture_Ricevute += 1

        '        '        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
        '        '            'Num_Mov_Debito += 1
        '        '            Num_Mov_Credito += 1
        '        '            Num_ResiAbbuoni_Acquisti += 1

        '        '        Case LAVCOD_ACQUISTO
        '        '            Num_Mov_Credito += 1
        '        '            Num_Corrispettivi_Acquisto += 1

        '        '            '######################################

        '        '        Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
        '        '            Num_Mov_Debito += 1
        '        '            Num_Fatture_Emesse += 1

        '        '        Case LAVCOD_NOTA_ACCREDITO_EMESSA
        '        '            'Num_Mov_Credito += 1
        '        '            Num_Mov_Debito += 1
        '        '            Num_ResiAbbuoni_Vendite += 1

        '        '        Case LAVCOD_AUTOCONSUMO
        '        '            Num_Mov_Debito += 1
        '        '            Num_AutoConsumo += 1

        '        '        Case LAVCOD_RICEVUTA_EMESSA
        '        '            Num_Mov_Debito += 1
        '        '            Num_RicevuteFiscali_Emesse += 1

        '        '        Case LAVCOD_VENDITA
        '        '            Num_Mov_Debito += 1
        '        '            Num_Corrispettivi_Vendita += 1

        '        '        Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
        '        '            Num_Mov_Debito += 1
        '        '            Num_DDTContabilizzati_Emessi += 1

        '        '    End Select 'lav_cod

        '        'Else

        '        '    'dal secondo giro
        '        '    If Id_Agenda_Temp = Id_Agenda Then
        '        '        'è la stessa operazione, non conteggio
        '        '    Else
        '        '        'imposto il nuovo temp
        '        '        Id_Agenda_Temp = Id_Agenda

        '        '        'sommo il num di doc
        '        '        Select Case Lav_Cod

        '        '            'Case LAVCOD_ALTRI_COSTI
        '        '            '    Num_Mov_Credito += 1
        '        '            '    Num_AltriCosti += 1

        '        '            'Case LAVCOD_ALTRI_RICAVI
        '        '            '    Num_Mov_Debito += 1
        '        '            '    Num_AltriRicavi += 1

        '        '        Case LAVCOD_FATTURA_RICEVUTA
        '        '                Num_Mov_Credito += 1
        '        '                Num_Fatture_Ricevute += 1

        '        '            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
        '        '                'Num_Mov_Debito += 1
        '        '                Num_Mov_Credito += 1
        '        '                Num_ResiAbbuoni_Acquisti += 1

        '        '            Case LAVCOD_ACQUISTO
        '        '                Num_Mov_Credito += 1
        '        '                Num_Corrispettivi_Acquisto += 1

        '        '                '######################################

        '        '            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
        '        '                Num_Mov_Debito += 1
        '        '                Num_Fatture_Emesse += 1

        '        '            Case LAVCOD_NOTA_ACCREDITO_EMESSA
        '        '                'Num_Mov_Credito += 1
        '        '                Num_Mov_Debito += 1
        '        '                Num_ResiAbbuoni_Vendite += 1

        '        '            Case LAVCOD_AUTOCONSUMO
        '        '                Num_Mov_Debito += 1
        '        '                Num_AutoConsumo += 1

        '        '            Case LAVCOD_RICEVUTA_EMESSA
        '        '                Num_Mov_Debito += 1
        '        '                Num_RicevuteFiscali_Emesse += 1

        '        '            Case LAVCOD_VENDITA
        '        '                Num_Mov_Debito += 1
        '        '                Num_Corrispettivi_Vendita += 1

        '        '            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
        '        '                Num_Mov_Debito += 1
        '        '                Num_DDTContabilizzati_Emessi += 1

        '        '        End Select 'lav_cod

        '        '    End If

        '        'End If

        '    Next 'movimenti

        'End If

        ''########################################################################

        '' ----------------------- DATATABLE CREDITO -----------------------------

        'Dt_Credito.Columns.Add(New DataColumn("Imponibile", GetType(String)))
        'Dt_Credito.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
        'Dt_Credito.Columns.Add(New DataColumn("Aliquota", GetType(String)))
        'Dt_Credito.Columns.Add(New DataColumn("IVA", GetType(String)))

        'Dr_Credito = Dt_Credito.NewRow
        'Dr_Credito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Credito_4)
        'Dr_Credito.Item("Cod_Iva") = IVA_4
        'Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_4)
        'Dr_Credito.Item("IVA") = Arrotonda_2Decimali(Iva_Credito_4)
        'Dt_Credito.Rows.Add(Dr_Credito)
        'Dr_Credito = Dt_Credito.NewRow
        'Dr_Credito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Credito_10)
        'Dr_Credito.Item("Cod_Iva") = IVA_10
        'Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_10)
        'Dr_Credito.Item("IVA") = Arrotonda_2Decimali(Iva_Credito_10)
        'Dt_Credito.Rows.Add(Dr_Credito)
        'Dr_Credito = Dt_Credito.NewRow
        'Dr_Credito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Credito_12)
        'Dr_Credito.Item("Cod_Iva") = IVA_12
        'Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_12)
        'Dr_Credito.Item("IVA") = Arrotonda_2Decimali(Iva_Credito_12)
        'Dt_Credito.Rows.Add(Dr_Credito)
        'Dr_Credito = Dt_Credito.NewRow
        'Dr_Credito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Credito_20)
        'Dr_Credito.Item("Cod_Iva") = IVA_20
        'Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_20)
        'Dr_Credito.Item("IVA") = Arrotonda_2Decimali(Iva_Credito_20)
        'Dt_Credito.Rows.Add(Dr_Credito)
        'Dr_Credito = Dt_Credito.NewRow
        'Dr_Credito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Credito_21)
        'Dr_Credito.Item("Cod_Iva") = IVA_21
        'Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_21)
        'Dr_Credito.Item("IVA") = Arrotonda_2Decimali(Iva_Credito_21)
        'Dt_Credito.Rows.Add(Dr_Credito)
        'Dr_Credito = Dt_Credito.NewRow
        'Dr_Credito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Credito_NoIva)
        'Dr_Credito.Item("Cod_Iva") = 0
        'Dr_Credito.Item("Aliquota") = "FCI, Non Ivabile, Art. Escl.IVA"
        'Dr_Credito.Item("IVA") = Arrotonda_2Decimali(Iva_Credito_NoIva)
        'Dt_Credito.Rows.Add(Dr_Credito)
        ''------------------------------------

        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_FCI, 2)
        ''Dr_Credito.Item("Cod_Iva") = FCI
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(FCI)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_FCI, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_NO_IVABILE, 2)
        ''Dr_Credito.Item("Cod_Iva") = NON_IVABILE
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NON_IVABILE)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_NO_IVABILE, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)

        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_61, 2)
        ''Dr_Credito.Item("Cod_Iva") = EsclArt15
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsclArt15)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_61, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_63, 2)
        ''Dr_Credito.Item("Cod_Iva") = EsArt7
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsArt7)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_63, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_64, 2)
        ''Dr_Credito.Item("Cod_Iva") = Art74LettC
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art74LettC)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_64, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_65, 2)
        ''Dr_Credito.Item("Cod_Iva") = Art74LettE
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art74LettE)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_65, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_66, 2)
        ''Dr_Credito.Item("Cod_Iva") = NonImpArt9
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt9)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_66, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_67, 2)
        ''Dr_Credito.Item("Cod_Iva") = NonImpArt8
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt8)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_67, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_68, 2)
        ''Dr_Credito.Item("Cod_Iva") = NonImpArt40
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt40)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_68, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_69, 2)
        ''Dr_Credito.Item("Cod_Iva") = EsenteArt10
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsenteArt10)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_69, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_70, 2)
        ''Dr_Credito.Item("Cod_Iva") = EsArt14L537
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsArt14L537)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_70, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_71, 2)
        ''Dr_Credito.Item("Cod_Iva") = Art44comma
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art44comma)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_71, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_72, 2)
        ''Dr_Credito.Item("Cod_Iva") = Art74Ter
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art74Ter)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_72, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_73, 2)
        ''Dr_Credito.Item("Cod_Iva") = NoNImpArt72
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NoNImpArt72)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_73, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_74, 2)
        ''Dr_Credito.Item("Cod_Iva") = NonImpArt26
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt26)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_74, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_75, 2)
        ''Dr_Credito.Item("Cod_Iva") = EsclusoArt5
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsclusoArt5)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_75, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_76, 2)
        ''Dr_Credito.Item("Cod_Iva") = Art3SubA
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art3SubA)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_76, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_77, 2)
        ''Dr_Credito.Item("Cod_Iva") = EsclArt134
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsclArt134)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_77, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)
        ''Dr_Credito = Dt_Credito.NewRow
        ''Dr_Credito.Item("Imponibile") = Math.Round(Imponibile_Credito_78, 2)
        ''Dr_Credito.Item("Cod_Iva") = Art4DL331
        ''Dr_Credito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art4DL331)
        ''Dr_Credito.Item("IVA") = Math.Round(IVA_Credito_78, 2)
        ''Dt_Credito.Rows.Add(Dr_Credito)


        'Me.DataGrid_Credito.DataSource = Dt_Credito
        'Me.DataGrid_Credito.DataBind()

        '' ----------------------------------------------------------------------

        ''########################################################################

        '' ----------------------- DATATABLE DEBITO -----------------------------

        'Dt_Debito.Columns.Add(New DataColumn("Imponibile", GetType(String)))
        'Dt_Debito.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
        'Dt_Debito.Columns.Add(New DataColumn("Aliquota", GetType(String)))
        'Dt_Debito.Columns.Add(New DataColumn("IVA", GetType(String)))

        'Dr_Debito = Dt_Debito.NewRow
        'Dr_Debito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Debito_4)
        'Dr_Debito.Item("Cod_Iva") = IVA_4
        'Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_4)
        'Dr_Debito.Item("IVA") = Arrotonda_2Decimali(Iva_Debito_4)
        'Dt_Debito.Rows.Add(Dr_Debito)
        'Dr_Debito = Dt_Debito.NewRow
        'Dr_Debito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Debito_10)
        'Dr_Debito.Item("Cod_Iva") = IVA_10
        'Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_10)
        'Dr_Debito.Item("IVA") = Arrotonda_2Decimali(Iva_Debito_10)
        'Dt_Debito.Rows.Add(Dr_Debito)
        'Dr_Debito = Dt_Debito.NewRow
        'Dr_Debito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Debito_12)
        'Dr_Debito.Item("Cod_Iva") = IVA_12
        'Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_12)
        'Dr_Debito.Item("IVA") = Arrotonda_2Decimali(Iva_Debito_12)
        'Dt_Debito.Rows.Add(Dr_Debito)
        'Dr_Debito = Dt_Debito.NewRow
        'Dr_Debito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Debito_20)
        'Dr_Debito.Item("Cod_Iva") = IVA_20
        'Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_20)
        'Dr_Debito.Item("IVA") = Arrotonda_2Decimali(Iva_Debito_20)
        'Dt_Debito.Rows.Add(Dr_Debito)
        'Dr_Debito = Dt_Debito.NewRow
        'Dr_Debito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Debito_21)
        'Dr_Debito.Item("Cod_Iva") = IVA_21
        'Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(IVA_21)
        'Dr_Debito.Item("IVA") = Arrotonda_2Decimali(Iva_Debito_21)
        'Dt_Debito.Rows.Add(Dr_Debito)
        'Dr_Debito = Dt_Debito.NewRow
        'Dr_Debito.Item("Imponibile") = Arrotonda_2Decimali(Imponibile_Debito_NoIva)
        'Dr_Debito.Item("Cod_Iva") = 0
        'Dr_Debito.Item("Aliquota") = "FCI, Non Ivabile, Art. Escl.IVA"
        'Dr_Debito.Item("IVA") = Arrotonda_2Decimali(Iva_Debito_NoIva)
        'Dt_Debito.Rows.Add(Dr_Debito)
        ''------------------------------------

        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_FCI, 2)
        ''Dr_Debito.Item("Cod_Iva") = FCI
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(FCI)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_FCI, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_NO_IVABILE, 2)
        ''Dr_Debito.Item("Cod_Iva") = NON_IVABILE
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NON_IVABILE)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_NO_IVABILE, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)

        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_61, 2)
        ''Dr_Debito.Item("Cod_Iva") = EsclArt15
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsclArt15)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_61, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_63, 2)
        ''Dr_Debito.Item("Cod_Iva") = EsArt7
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsArt7)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_63, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_64, 2)
        ''Dr_Debito.Item("Cod_Iva") = Art74LettC
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art74LettC)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_64, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_65, 2)
        ''Dr_Debito.Item("Cod_Iva") = Art74LettE
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art74LettE)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_65, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_66, 2)
        ''Dr_Debito.Item("Cod_Iva") = NonImpArt9
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt9)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_66, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_67, 2)
        ''Dr_Debito.Item("Cod_Iva") = NonImpArt8
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt8)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_67, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_68, 2)
        ''Dr_Debito.Item("Cod_Iva") = NonImpArt40
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt40)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_68, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_69, 2)
        ''Dr_Debito.Item("Cod_Iva") = EsenteArt10
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsenteArt10)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_69, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_70, 2)
        ''Dr_Debito.Item("Cod_Iva") = EsArt14L537
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsArt14L537)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_70, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_71, 2)
        ''Dr_Debito.Item("Cod_Iva") = Art44comma
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art44comma)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_71, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_72, 2)
        ''Dr_Debito.Item("Cod_Iva") = Art74Ter
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art74Ter)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_72, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_73, 2)
        ''Dr_Debito.Item("Cod_Iva") = NoNImpArt72
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NoNImpArt72)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_73, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_74, 2)
        ''Dr_Debito.Item("Cod_Iva") = NonImpArt26
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(NonImpArt26)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_74, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_75, 2)
        ''Dr_Debito.Item("Cod_Iva") = EsclusoArt5
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsclusoArt5)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_75, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_76, 2)
        ''Dr_Debito.Item("Cod_Iva") = Art3SubA
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art3SubA)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_76, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_77, 2)
        ''Dr_Debito.Item("Cod_Iva") = EsclArt134
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(EsclArt134)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_77, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)
        ''Dr_Debito = Dt_Debito.NewRow
        ''Dr_Debito.Item("Imponibile") = Math.Round(Imponibile_Debito_78, 2)
        ''Dr_Debito.Item("Cod_Iva") = Art4DL331
        ''Dr_Debito.Item("Aliquota") = objContabHLP.Aliquota_from_CodIVA(Art4DL331)
        ''Dr_Debito.Item("IVA") = Math.Round(IVA_Debito_78, 2)
        ''Dt_Debito.Rows.Add(Dr_Debito)

        'Me.DataGrid_Debito.DataSource = Dt_Debito
        'Me.DataGrid_Debito.DataBind()


        '' ----------------------------------------------------------------------

        'Me.Txt_NumMovCredito.Text = Num_Mov_Credito
        'Me.Txt_NumFattureRicevute.Text = Num_Fatture_Ricevute
        'Me.Txt_NumResiAcquisti.Text = Num_ResiAbbuoni_Acquisti 'resi su acquisti
        'Me.Txt_NumAcquisti.Text = Num_Corrispettivi_Acquisto
        'Me.Txt_NumAltriCosti.Text = Num_AltriCosti

        'Me.Txt_NumMovDebito.Text = Num_Mov_Debito
        'Me.Txt_NumFattureEmesse.Text = Num_Fatture_Emesse
        'Me.Txt_NumResiVendite.Text = Num_ResiAbbuoni_Vendite 'resi su vendite
        'Me.Txt_NumAutoconsumo.Text = Num_AutoConsumo
        'Me.Txt_NumRicevuteFiscaliEmesse.Text = Num_RicevuteFiscali_Emesse
        'Me.Txt_NumVendite.Text = Num_Corrispettivi_Vendita
        'Me.Txt_NumDDTContabilizzati_Emessi.Text = Num_DDTContabilizzati_Emessi
        'Me.Txt_NumAltriRicavi.Text = Num_AltriRicavi

        'Me.Txt_Credito.Text = Arrotonda_2Decimali(Tot_Saldo_Credito)
        'Me.Txt_TotCredito.Text = Arrotonda_2Decimali(Tot_Saldo_Credito)

        'Me.Txt_Debito.Text = Arrotonda_2Decimali(Tot_Saldo_Debito)
        'Me.Txt_TotDebito.Text = Arrotonda_2Decimali(Tot_Saldo_Debito)

        'Me.Txt_Differenza.Text = Arrotonda_2Decimali(Tot_Saldo_Credito - Tot_Saldo_Debito)


        'If Me.Chk_IVA_Precedente.Checked = True Then
        '    Calcola_IVA_Precedente()
        'End If

        'If Log <> "" Then
        '    AgroMsgBox(Log, Page)
        'End If


    End Sub


    '####################################################################################
    Private Sub Chk_IVA_Precedente_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_IVA_Precedente.CheckedChanged
        Calcola_IVA_Precedente()
    End Sub


    '####################################################################################
    'da sistemare
    Private Sub Calcola_IVA_Precedente()

        'Dim Dt As DataTable
        'Dim i As Integer
        'Dim Data_Inizio, Data_Fine As String
        'Dim Filtro, Ordinamento As String
        'Dim IVA As Double = 0
        'Dim Log As String = ""
        'Dim Sezionale_Cod As Integer


        'If Me.Chk_IVA_Precedente.Checked = True Then

        '    Sezionale_Cod = Me.Cmb_Sezionali.SelectedValue

        '    Calcola_Date_InizioFine(Data_Inizio, Data_Fine, Log, True)

        '    If Log <> "" Then
        '        AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Log, Page)
        '        Exit Sub
        '    End If

        '    Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab

        '    Dt = objStampe.LiquidazioneIVA(Piva, _
        '                                    Sezionale_Cod, _
        '                                    0, 0, 0, _
        '                                    Data_Inizio, Data_Fine, _
        '                                     objParametri_Server)

        '    If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

        '        For i = 0 To Dt.Rows.Count - 1
        '            IVA = Arrotonda_2Decimali(IVA + CDbl(Dt.Rows(i).Item("Iva")))
        '        Next

        '        If IVA > 0 Then
        '            ' Me.Txt_IVA_Precedente.Text = Math.Round(IVA, 2)
        '            Me.Txt_IVA_Precedente.Text = Arrotonda_2Decimali(IVA)
        '        Else
        '            Me.Txt_IVA_Precedente.Text = "0"
        '            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non risulta IVA a credito dall'anno precedente.", Page)
        '        End If

        '        Me.Txt_Credito.Text = Arrotonda_2Decimali(CDbl(Me.Txt_Credito.Text) + CDbl(Me.Txt_IVA_Precedente.Text))

        '        Me.Txt_Differenza.Text = Arrotonda_2Decimali(CDbl(Me.Txt_Credito.Text) - CDbl(Me.Txt_Debito.Text))

        '    End If

        'Else

        '    Me.Txt_Credito.Text = Arrotonda_2Decimali(CDbl(Me.Txt_Credito.Text) - CDbl(Me.Txt_IVA_Precedente.Text))

        '    Me.Txt_Differenza.Text = Arrotonda_2Decimali(CDbl(Me.Txt_Credito.Text) - CDbl(Me.Txt_Debito.Text))

        '    Me.Txt_IVA_Precedente.Text = "0"

        'End If


    End Sub


    '####################################################################################
    Private Sub ImgBtnStampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnStampa.Click
        Stampa()
    End Sub


    '####################################################################################
    Private Sub Stampa()

        Dim Querystring As String
        '  Dim TargetURL, Anno, Iva_Prec, Acconto As String
        Dim Num_Pagina As Integer = 0
        Dim log As String = ""
        Dim Data_Inizio, Data_Fine As String
        Dim Sezionale_Des As String = ""
        Dim RegimeIva As enum_RegimeIva
        ' Dim Saldo As Double
        '  Dim SaldoFINALE As Double

        'If Me.Cmb_AnnoContabile.SelectedValue <> "" Then
        '    Anno = Me.Cmb_AnnoContabile.SelectedValue
        'Else
        '    Anno = 0
        'End If

        Calcola_Date_InizioFine(Data_Inizio, Data_Fine, log, False)

        If Me.Txt_NumPaginaRegIVA.Text = "" Or Not IsNumeric(Me.Txt_NumPaginaRegIVA.Text) Then
            log &= vbCrLf & "E' necessario specificare il numero di pagina dal quale iniziare la numerazione delle pagine del registro."
        Else
            Num_Pagina = CInt(Me.Txt_NumPaginaRegIVA.Text)
        End If


        If log <> "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(log, Page)
            Exit Sub
        End If

        'If Me.Txt_IVA_Precedente.Text <> "" Then
        '    Iva_Prec = Me.Txt_IVA_Precedente.Text
        'Else
        '    Iva_Prec = 0
        'End If

        'If Me.Txt_Acconto.Text <> "" And IsNumeric(Me.Txt_Acconto.Text) Then
        '    Acconto = Me.Txt_Acconto.Text
        'Else
        '    Acconto = 0
        'End If

        'Dim Conto As String

        'If Not IsNothing(Me.Cmb_Conti.SelectedItem) Then
        '    Conto = Me.Cmb_Conti.SelectedItem.Text
        'Else
        '    Conto = ""
        'End If

        ' Saldo = Verifica_Calcola_Saldo()

        Dim str_errore As String = ""
        Verifica_Calcola_Saldo(str_errore)
        If str_errore <> "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(str_errore, Page)
            Exit Sub
        End If

        'Me.Txt_Differenza.Text = Format(Saldo, "##,###,##0.00")
        'Me.Txt_ImpostaDaVersare.Text = Format(SaldoFINALE, "##,###,##0.00")

        '=======================================================================

        Const PaginaLinkLiquidazioneIVA = "LiquidazioneIVA_2.aspx"

        Sezionale_Des = Me.Cmb_Sezionali.SelectedItem.Text
        RegimeIva = Me.Cmb_Sezionali.SelectedValue.Split("|")(3)

        If RegimeIva = enum_RegimeIva.NonImpostato And Not IsNothing(ViewState("RegimeIvaTuttiSezionali")) Then
            RegimeIva = ViewState("RegimeIvaTuttiSezionali")
        End If

        Querystring = "?p=" & _
                    Stringa_Codifica(CStr(Piva), AgroKey_EncoderDecoder, Server) & _
                    "&di=" & _
                    Stringa_Codifica(CStr(Data_Inizio), AgroKey_EncoderDecoder, Server) & _
                    "&df=" & _
                    Stringa_Codifica(CStr(Data_Fine), AgroKey_EncoderDecoder, Server) & _
                     "&ivaprec=" & _
                    Stringa_Codifica(CStr(Me.Txt_IVA_Precedente.Text), AgroKey_EncoderDecoder, Server) & _
                     "&acc=" & _
                    Stringa_Codifica(CStr(Me.Txt_Acconto.Text), AgroKey_EncoderDecoder, Server) & _
                     "&cred=" & _
                    Stringa_Codifica(CStr(Me.Txt_Credito.Text), AgroKey_EncoderDecoder, Server) & _
                     "&deb=" & _
                    Stringa_Codifica(CStr(Me.Txt_Debito.Text), AgroKey_EncoderDecoder, Server) & _
                     "&saldo=" & _
                    Stringa_Codifica(CStr(Me.Txt_Differenza.Text), AgroKey_EncoderDecoder, Server) & _
                     "&np=" & _
                    Stringa_Codifica(CStr(Num_Pagina), AgroKey_EncoderDecoder, Server) & _
                    "&szd=" & _
                    Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) & _
                    "&regiv=" & _
                    Stringa_Codifica(RegimeIva, AgroKey_EncoderDecoder, Server) & _
                    "&idip=" & _
                    Stringa_Codifica(CStr(Me.Txt_InteresseDebito_Perc.Text), AgroKey_EncoderDecoder, Server) & _
                    "&idiv=" & _
                    Stringa_Codifica(CStr(Me.Txt_InteresseDebito_Valore.Text), AgroKey_EncoderDecoder, Server) & _
                    "&saldofin=" & _
                    Stringa_Codifica(CStr(Me.Txt_ImpostaDaVersare.Text), AgroKey_EncoderDecoder, Server)


        '   Const PaginaLinkLiquidazioneIVA = "LiquidazioneIVA.aspx"

        'Querystring = "?p=" & _
        '            Stringa_Codifica(CStr(Piva), AgroKey_EncoderDecoder, Server) & _
        '            "&conto=" & _
        '            Stringa_Codifica(Conto, AgroKey_EncoderDecoder, Server) & _
        '            "&di=" & _
        '            Stringa_Codifica(CStr(Data_Inizio), AgroKey_EncoderDecoder, Server) & _
        '            "&df=" & _
        '            Stringa_Codifica(CStr(Data_Fine), AgroKey_EncoderDecoder, Server) & _
        '             "&impc4=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(0).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivac4=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(0).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impc10=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(1).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivac10=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(1).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impc12=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(2).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivac12=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(2).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impc20=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(3).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivac20=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(3).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impc21=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(4).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivac21=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Credito.Items(4).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impd4=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(0).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivad4=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(0).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impd10=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(1).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivad10=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(1).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impd12=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(2).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivad12=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(2).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&impd20=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(3).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivad20=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(3).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '            "&impd21=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(4).Cells(0).Text), AgroKey_EncoderDecoder, Server) & _
        '             "&ivad21=" & _
        '            Stringa_Codifica(CStr(Me.DataGrid_Debito.Items(4).Cells(3).Text), AgroKey_EncoderDecoder, Server) & _
        '            "&ivaprec=" & _
        '            Stringa_Codifica(CStr(Iva_Prec), AgroKey_EncoderDecoder, Server)


        'apro la stampa in una nuova finestra
        AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow(Page, _
                                                                PaginaLinkLiquidazioneIVA, _
                                                                Querystring, _
                                                                "Liquidazione_IVA", _
                                                                , , _
                                                                , , , , , )


    End Sub



End Class
