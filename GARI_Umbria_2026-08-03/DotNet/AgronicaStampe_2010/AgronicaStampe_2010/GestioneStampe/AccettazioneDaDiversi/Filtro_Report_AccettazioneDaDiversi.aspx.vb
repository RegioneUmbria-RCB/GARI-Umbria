Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports System.Management
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.Drawing





Public Class Filtro_Report_AccettazioneDaDiversi
    Inherits System.Web.UI.Page

#Region " Filtro Report Accettazione da diversi "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents LblTitolo As System.Web.UI.WebControls.Label
    Protected WithEvents ImgIcona As System.Web.UI.WebControls.Image
    Protected WithEvents ImgBtnEsci As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Txt_ValiditaFine As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_ValiditaInizio As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label72 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL3 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Date As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL1 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Generale As System.Web.UI.WebControls.Panel
    Protected WithEvents Rbl_Report As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Lbl_Stampa As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_Stampa As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Stampa As System.Web.UI.WebControls.Panel
    Protected WithEvents Label6 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Conferente As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL4 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL7 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL8 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Magazzini As System.Web.UI.WebControls.Panel
    Protected WithEvents lbl_AnnoContabile As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Da_CodiceConferente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_A_CodiceConferente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Btn_Da_Conferente As System.Web.UI.WebControls.Button
    Protected WithEvents Btn_A_Conferente As System.Web.UI.WebControls.Button
    Protected WithEvents Txt_Da_Conferente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_A_Conferente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Specie As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL12 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_Da_CodiceSpecie As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL11 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_A_CodiceSpecie As System.Web.UI.WebControls.TextBox
    Protected WithEvents Btn_Da_Specie As System.Web.UI.WebControls.Button
    Protected WithEvents Btn_A_Specie As System.Web.UI.WebControls.Button
    Protected WithEvents Txt_Da_Specie As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_A_Specie As System.Web.UI.WebControls.TextBox
    Protected WithEvents Cmb_Magazzino As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lbl_A_Specie As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_SringaFiltroConferenti As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_SringaFiltroSpecie As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL9 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL10 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Produttore As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_FiltroPiva_Produttore As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL2 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_FiltroRagSoc_Produttore As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label13 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Prodotti As System.Web.UI.WebControls.Panel
    Protected WithEvents Cmb_Prodotti As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Btn_Carica_Produttore As System.Web.UI.WebControls.Button
    Protected WithEvents Txt_Filtro_MatDes As System.Web.UI.WebControls.TextBox
    Protected WithEvents Btn_Carica_Prodotti As System.Web.UI.WebControls.Button
    Protected WithEvents Txt_CodContatto_DA_Conferente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_CodContatto_A_Conferente As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_RagSoc_Produttore As System.Web.UI.WebControls.TextBox
    Protected WithEvents lbl_filtro_prodotti As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL14 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL15 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL16 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_NumeriBolla As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_DaNumeroBolla As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_ANumeroBolla As System.Web.UI.WebControls.TextBox
    Protected WithEvents Chk_TracciaImpianti As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Txt_SringaFiltroBolle As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Suffisso_DaNumeroBolla As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Suffisso_ANumeroBolla As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_OpzioniStampa As System.Web.UI.WebControls.Panel
    Protected WithEvents Chk_Fascicola As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Label17 As System.Web.UI.WebControls.Label
    Protected WithEvents Label18 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NumeroCopie As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label19 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Stampante As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ImgBtn_StampaMassivaBolle As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_ConfermaStampaMassiva As System.Web.UI.WebControls.Panel
    Protected WithEvents ImgBtn_AnnullaStampaMassiva As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label22 As System.Web.UI.WebControls.Label
    Protected WithEvents Label23 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_NomeStampante As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label25 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_RiepilogoBolle As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_NumeroCopie_Bis As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label30 As System.Web.UI.WebControls.Label
    Protected WithEvents Label31 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_StampaCertificatoPomodoro As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_CertificatiPomodoro As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_CaricoScaricoPomodoro As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL26 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataRegPom As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label27 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_PagRegPom As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label28 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_RigaRegPom As System.Web.UI.WebControls.TextBox
    Protected WithEvents Chk_SalvaNumPagRigaPom As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Rbl_PomodoroContrattato As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Btn_CalcolaProgressivi As System.Web.UI.WebControls.Button
    Protected WithEvents Rbl_Certificati As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Rbl_StampaPomodoro As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Txt_Suffisso_ANumeroCert As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Suffisso_DaNumeroCert As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Prefisso_ANumeroCert As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Prefisso_DaNumeroCert As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL29 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_ANumeroCert As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_DaNumeroCert As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL32 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL33 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_NumeriCertificato As System.Web.UI.WebControls.Panel
    Protected WithEvents Rbl_CertificatiStampaMassiva As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents Txt_TipoStampa As System.Web.UI.WebControls.TextBox
    Protected WithEvents Lbl_ConfermaStampaMassiva As System.Web.UI.WebControls.Label
    Protected WithEvents Lbl_SelezioneStampaMassiva As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL37 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_FiltroConti As System.Web.UI.HtmlControls.HtmlInputText
    Protected WithEvents Txt_FlagInsertNumCert As System.Web.UI.HtmlControls.HtmlInputText
    Protected WithEvents LABEL5 As System.Web.UI.WebControls.Label
    Protected WithEvents lbl_stampamassiva As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL20 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataGiacenza As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Giacenza As System.Web.UI.WebControls.Panel
    Protected WithEvents Panel1 As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_Prefisso_DaNumeroBolla_D As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_Prefisso_ANumeroBolla_D As System.Web.UI.WebControls.TextBox
    Protected WithEvents Cmb_Prefisso_DaNumeroBolla As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Cmb_Prefisso_ANumeroBolla As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Txt_FiltroRagSoc_Coop1 As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL21 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_RagSoc_Coop1 As System.Web.UI.WebControls.TextBox
    Protected WithEvents Btn_Carica_Coop1 As System.Web.UI.WebControls.Button
    Protected WithEvents LABEL24 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_FiltroPiva_Coop1 As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL34 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Coop1 As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_FiltroRagSoc_Coop2 As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL35 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_RagSoc_Coop2 As System.Web.UI.WebControls.TextBox
    Protected WithEvents Btn_Carica_Coop2 As System.Web.UI.WebControls.Button
    Protected WithEvents LABEL36 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_FiltroPiva_Coop2 As System.Web.UI.WebControls.TextBox
    Protected WithEvents LABEL38 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Coop2 As System.Web.UI.WebControls.Panel
    Protected WithEvents Cmb_Regolamento As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Label39 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Regolamento As System.Web.UI.WebControls.Panel

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Dim Qs_PrintName As String
    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    Dim Qs_Mode As Integer
    Dim Qs_IdAgenda As Integer
    '\\zaffiro\HP LaserJet P2015 (Agronica)|\\zaffiro\HP LaserJet 2300L

    Const INDICE_REG_CARICOSCARICO_POMO As Integer = 10

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Try

            ''Imposto il default sul bottone, in base alla casella di testo selezionata
            'Me.Txt_Da_CodiceConferente.Attributes.Add("onkeydown", "SetFocus('Btn_Da_Conferente');")
            'Me.Txt_A_CodiceConferente.Attributes.Add("onkeydown", "SetFocus('Btn_A_Conferente');")
            'Me.Txt_Da_CodiceSpecie.Attributes.Add("onkeydown", "SetFocus('Btn_Da_Specie');")
            'Me.Txt_A_CodiceSpecie.Attributes.Add("onkeydown", "SetFocus('Btn_A_Specie');")


            'COMMENTO PER IL GIASLAN

            ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            'Dim strDummy As String      'controllo accesso negato.....
            'Dim UtenteAbilitato As Boolean

            'UtenteAbilitato = Controlla_Permessi_Utente_2( _
            '                            Server, Session, Page, _
            '                            Session("ASG_Utente_Username"), _
            '                            Session("ASG_IdServizio"), _
            '                            TipiEnumerativi.enum_Security_Attivita.Report_Accettazione_DaDiversi, _
            '                            TipiEnumerativi.enum_Security_Operazione.Lettura, _
            '                            strDummy)

            ''----- Se l'utente non ha il permesso per visualizzare la pagina ... 
            'If UtenteAbilitato = False Then

            '    Dim strClose As String = "<script language='javascript'> window.close() </script>"
            '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

            '    Exit Sub

            'End If



            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################

            'Report = Session("ReportSelezionato")

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

            Qs_RagSoc = Stringa_Decodifica(Request.QueryString("rs").ToString, _
                                  AgroKey_EncoderDecoder, _
                                  Server)

            Qs_PrintName = Stringa_Decodifica(Request.QueryString("pn").ToString, _
                                  AgroKey_EncoderDecoder, _
                                  Server)

            Qs_Mode = CInt(Stringa_Decodifica(Request.QueryString("m").ToString, _
                               AgroKey_EncoderDecoder, _
                               Server))

            If Not IsNothing(Request.QueryString("i")) Then
                Qs_IdAgenda = CInt(Stringa_Decodifica(Request.QueryString("i").ToString, _
                              AgroKey_EncoderDecoder, _
                              Server))
            Else
                Qs_IdAgenda = 0
            End If

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

            '============================================================
            '       Sono in postback
            '============================================================
            If Me.IsPostBack Then


                Select Case Me.Txt_FlagInsertNumCert.Value

                    Case "0"
                        Me.Txt_FlagInsertNumCert.Value = ""
                        Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
                        Me.Rbl_Certificati.SelectedValue = 1
                        Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)

                    Case "1"
                        Me.Txt_FlagInsertNumCert.Value = ""

                        If Qs_IdAgenda <> 0 Then
                            'modalità stampa singola certificato
                            StampaCertificatoPomodoro()
                        Else
                            'modalità stampa massiva
                            'chiama la stampa del certificato esterno
                            'che a sua volta, dopo il recupero controlli, 
                            'chiamerà StampaMassivaBolle
                            Stampa()
                        End If

                        Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
                        Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)
                        Me.Rbl_Certificati.SelectedValue = 1

                End Select

                Exit Sub

            End If

            '============================================================
            '----- Verifico se l'utente dispone dei permessi 
            'della stampa del registro di carico e scarico
            'e del certificato esterno del pomodoro
            'da fruttagel: solo claudio pisi
            Dim UtenteAbilitato_RegCS_CertEsterno As Boolean
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            UtenteAbilitato_RegCS_CertEsterno = objUtenti.Controlla_Permessi_Utente( _
                Session("ASG_Utente_Username"), _
                Session("ASG_IdServizio"), _
                enum_Security_Attivita.Stampe_RegistroCaricoScarico_Pomodoro, _
                enum_Security_Operazione.Lettura, _
                 Now(), _
                 "", _
                 objParametri_Utenti _
            )


            '----- Se l'utente non ha il permesso 
            If UtenteAbilitato_RegCS_CertEsterno = False Then
                'rimuovo l'opzione di stampa del reg di carico e scarico
                Me.Rbl_Report.Items.RemoveAt(INDICE_REG_CARICOSCARICO_POMO)
                'disabilito la scelta del tipo di certificato
                Me.Rbl_Certificati.Enabled = False
                Me.Rbl_CertificatiStampaMassiva.Enabled = False
            Else
                'attivo la scelta del tipo di certificato
                Me.Rbl_Certificati.Enabled = True
                'attualmente disattivo la stampa massiva
                Me.Rbl_CertificatiStampaMassiva.Enabled = True
            End If
            '============================================================


            '-------------------------------------------
            '----------- imposta modalità --------------
            '-------------------------------------------
            Select Case Qs_Mode

                Case 0 'FILTRO REPORT

                    'configura il pannello principale
                    Imposta_Pannelli(enum_Pannelli.Pannello_Generale)

                    Me.LblTitolo.Text = "Filtro Report Accettazione da Diversi"

                    'configurazione in base al report scelto

                    Imposta_Filtri_Report(Me.Rbl_Report.SelectedValue)

                    Imposta_Default_Controlli(Me.Rbl_Report.SelectedValue)

                    '-----------------------------------------------------------------------------

                Case 1 'CERTIFICATO POMODORO

                    Disattiva_Pannelli()

                    'configura il pannello di stampa bolla o certificato
                    Imposta_Pannelli(enum_Pannelli.Pannello_Pomodoro)

                    Me.LblTitolo.Text = "Stampa Certificato del Pomodoro"

                    'redirect subito al certificato interno
                    StampaCertificatoPomodoro()

            End Select


        Catch ex As Exception
            Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
            Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)
            Me.Rbl_Certificati.SelectedValue = 1
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Problemi durante il caricamento della pagina: " + vbCrLf + ex.Message, Page)
        End Try



    End Sub


    '##################################################################################
    Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click

        Dim strClose As String = "<script language='javascript'>window.close()</script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub


    '##################################################################################
    Private Sub Rbl_Report_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Report.SelectedIndexChanged

        Imposta_Filtri_Report(Me.Rbl_Report.SelectedValue)

    End Sub

    '##################################################################################
    Private Sub Disattiva_Pannelli()

        Me.Pannello_Date.Visible = False
        Me.Pannello_Giacenza.Visible = False
        Me.Pannello_NumeriBolla.Visible = False
        Me.Pannello_Magazzini.Visible = False

        Me.Pannello_Conferente.Visible = False
        Me.Pannello_Produttore.Visible = False
        Me.Pannello_Coop1.Visible = False
        Me.Pannello_Coop2.Visible = False

        Me.Pannello_Specie.Visible = False
        Me.Pannello_Prodotti.Visible = False

        Me.Pannello_Stampa.Visible = False
        Me.Pannello_OpzioniStampa.Visible = False
        Me.Pannello_ConfermaStampaMassiva.Visible = False

        Me.Pannello_CertificatiPomodoro.Visible = False
        Me.Pannello_CaricoScaricoPomodoro.Visible = False
        Me.Pannello_NumeriCertificato.Visible = False

        Me.Pannello_Regolamento.Visible = False

    End Sub


    '##################################################################################
    Private Sub Imposta_Filtri_Report(ByVal Report As enum_CodificaStampe)

        Disattiva_Pannelli()

        Me.lbl_A_Specie.Visible = True
        Me.Txt_A_CodiceSpecie.Visible = True
        Me.Txt_A_Specie.Visible = True
        Me.Btn_A_Specie.Visible = True

        Me.lbl_filtro_prodotti.Text = "Descrizione:"
        Me.Cmb_Prodotti.Items.Clear()
        Me.Txt_Filtro_MatDes.Text = ""

        Me.Chk_TracciaImpianti.Visible = False

        Select Case Report

            Case enum_CodificaStampe.ADD_Riepilogo_Conf_XSpecie

                Me.Pannello_Conferente.Visible = True
                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Specie.Visible = True
                Me.Pannello_Stampa.Visible = True

                'Me.lbl_A_Specie.Visible = False
                'Me.Txt_A_CodiceSpecie.Visible = False
                'Me.Txt_A_Specie.Visible = False
                'Me.Btn_A_Specie.Visible = False

                '------------------------------------
            Case enum_CodificaStampe.ADD_EC_Bolle_Accettazione_DaDiversi

                'Me.Pannello_Stampa.Visible = False

                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Conferente.Visible = True
                Me.Pannello_Specie.Visible = True
                Me.Pannello_Prodotti.Visible = True
                Me.Pannello_Stampa.Visible = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_EC_Imballi

                'Me.Pannello_Stampa.Visible = False

                Me.lbl_filtro_prodotti.Text = "Cod/Descr:"

                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Conferente.Visible = True

                Me.Pannello_Prodotti.Visible = True
                Me.Pannello_Stampa.Visible = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_Saldo_Imballi

                'Me.Pannello_Stampa.Visible = False
                Me.Pannello_Giacenza.Visible = True
                Me.Txt_DataGiacenza.Text = Date.Today.ToShortDateString

                Me.lbl_filtro_prodotti.Text = "Cod/Descr:"

                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Conferente.Visible = True

                Me.Pannello_Prodotti.Visible = True
                Me.Pannello_Stampa.Visible = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_Export_Bolle_Accettazione_DaDiversi

                'Me.Pannello_Stampa.Visible = False

                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Conferente.Visible = True
                Me.Pannello_Produttore.Visible = True
                Me.Pannello_Coop1.Visible = True
                Me.Pannello_Coop2.Visible = True
                Me.Pannello_Specie.Visible = True
                Me.Pannello_Prodotti.Visible = True
                Me.Pannello_Stampa.Visible = True

                Me.Chk_TracciaImpianti.Visible = True

                'Me.lbl_A_Specie.Visible = False
                'Me.Txt_A_CodiceSpecie.Visible = False
                'Me.Txt_A_Specie.Visible = False
                'Me.Btn_A_Specie.Visible = False

                '------------------------------------
            Case enum_CodificaStampe.ADD_Export_Traportatori

                Me.Pannello_Conferente.Visible = False
                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Specie.Visible = True
                Me.Pannello_Stampa.Visible = True

                '------------------------------------
            Case enum_CodificaStampe.Buono_Accettazione_Diversi 'stampa massiva

                Me.Pannello_Date.Visible = True
                Me.Pannello_NumeriBolla.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Stampa.Visible = True
                Me.Pannello_OpzioniStampa.Visible = True

                Me.Pannello_Conferente.Visible = True
                Me.Pannello_Produttore.Visible = True
                Me.Pannello_Coop1.Visible = True
                Me.Pannello_Coop2.Visible = True
                Me.Pannello_Specie.Visible = True

                'Me.Txt_ValiditaInizio.Text = ""
                'Me.Txt_ValiditaFine.Text = ""

                '------------------------------------

            Case enum_CodificaStampe.Certificato_Pomodoro 'stampa massiva certificato

                Me.Pannello_Date.Visible = True
                Me.Pannello_NumeriBolla.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_NumeriCertificato.Visible = True
                Me.Pannello_Stampa.Visible = True
                Me.Pannello_OpzioniStampa.Visible = True

                Me.Pannello_Conferente.Visible = True
                Me.Pannello_Produttore.Visible = True
                Me.Pannello_Coop1.Visible = True
                Me.Pannello_Coop2.Visible = True
                Me.Pannello_Specie.Visible = True

                'Me.Txt_ValiditaInizio.Text = ""
                'Me.Txt_ValiditaFine.Text = ""

                '-----------------------------------

            Case enum_CodificaStampe.Registro_CaricoScarico_Pomodoro

                'Me.Pannello_Prodotti.Visible = True
                Me.Pannello_CaricoScaricoPomodoro.Visible = True
                Me.Pannello_Stampa.Visible = True

                Me.Txt_DataRegPom.Text = CStr(Date.Today)

                Calcola_Progressivi(Year(Date.Today))

                '------------------------------------

            Case enum_CodificaStampe.ADD_ExcelTracciabilitaConferimenti

                Me.Pannello_Date.Visible = True
                Me.Pannello_Regolamento.Visible = True
                Me.Pannello_Stampa.Visible = True

                '------------------------------------


            Case enum_CodificaStampe.ADD_ExportExcel_CertificatiPomodoro

                'Me.Pannello_Stampa.Visible = False

                Me.Pannello_Date.Visible = True
                Me.Pannello_Magazzini.Visible = True
                Me.Pannello_Conferente.Visible = True
                Me.Pannello_Coop1.Visible = True
                Me.Pannello_Produttore.Visible = True
                Me.Pannello_Specie.Visible = True
                'Me.Pannello_Prodotti.Visible = True
                Me.Pannello_Stampa.Visible = True

                '------------------------------------

        End Select


        '-----------------------------------------------
        '-------------- Opzioni Stampa -----------------
        '-----------------------------------------------
        If Me.Pannello_OpzioniStampa.Visible = True Then

            'Dim ip As String = Request.UserHostAddress()

            Stampanti_Load()

        End If


    End Sub


    '##################################################################################
    Private Sub Imposta_Default_Controlli(ByVal Report As enum_CodificaStampe)

        Dim objUtentiImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Try

            Dim Validita_Inizio, Validita_Fine As String

            Me.Cmb_Prodotti.Items.Clear()
            Me.Txt_Filtro_MatDes.Text = ""


            Select Case Report

                Case enum_CodificaStampe.ADD_Riepilogo_Conf_XSpecie

                    Validita_Inizio = Date.Today.ToShortDateString
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------
                Case enum_CodificaStampe.ADD_EC_Bolle_Accettazione_DaDiversi

                    Validita_Inizio = Date.Today.ToShortDateString
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------
                Case enum_CodificaStampe.ADD_EC_Imballi

                    Validita_Inizio = Date.Today.ToShortDateString
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------
                Case enum_CodificaStampe.ADD_Saldo_Imballi

                    Validita_Inizio = Date.Today.ToShortDateString
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------
                Case enum_CodificaStampe.ADD_Export_Bolle_Accettazione_DaDiversi

                    Validita_Inizio = "01/" + Right("00" + CStr(Date.Today.Month), 2) + "/" + CStr(Date.Today.Year)
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------
                Case enum_CodificaStampe.ADD_Export_Traportatori

                    Validita_Inizio = "01/" + Right("00" + CStr(Date.Today.Month), 2) + "/" + CStr(Date.Today.Year)
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------
                Case enum_CodificaStampe.Buono_Accettazione_Diversi

                    'Validita_Inizio = "01/" + Right("00" + CStr(Date.Today.Month), 2) + "/" + CStr(Date.Today.Year)
                    'Validita_Fine = Date.Today.ToShortDateString
                    Validita_Inizio = ""
                    Validita_Fine = ""

                    '------------------------------------
                Case enum_CodificaStampe.Certificato_Pomodoro 'stampa massiva

                    'Validita_Inizio = "01/" + Right("00" + CStr(Date.Today.Month), 2) + "/" + CStr(Date.Today.Year)
                    'Validita_Fine = Date.Today.ToShortDateString
                    Validita_Inizio = ""
                    Validita_Fine = ""

                    '------------------------------------
                Case enum_CodificaStampe.Registro_CaricoScarico_Pomodoro

                    Validita_Inizio = ""
                    Validita_Fine = ""

                    '------------------------------------
                Case enum_CodificaStampe.ADD_ExportExcel_CertificatiPomodoro

                    Validita_Inizio = Date.Today.ToShortDateString
                    Validita_Fine = Date.Today.ToShortDateString

                    '------------------------------------


            End Select



            '-----------------------------------------------
            '--------- Default Intervallo Temporale --------
            '-----------------------------------------------

            Me.Txt_ValiditaInizio.Text = Validita_Inizio
            Me.Txt_ValiditaFine.Text = Validita_Fine


            '-----------------------------------------------
            '---------------- Magazzini --------------------
            '-----------------------------------------------

            If Me.Pannello_Magazzini.Visible = True Then

                'se l'utente ha il magazzino impostato nel filtro di visibilità
                '-> far vedere solo quello
                'altrimenti caricarli tutti
                Dim Impostazione_Valore_1 As String
                Dim Piva As String = ""
                Dim Sa_Cod As Integer = 0
                Dim Fabbricato_Cod As Integer = 0

                Impostazione_Valore_1 = objUtentiImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_MAGAZZINO_RIFERIMENTO, _
                                                                                               objParametri_Utenti _
                                                                                                )


                Dim objADD As New AgronicaCoreContabHLP.AccettazioneDaDiversi
                objADD.Leggi_ChiaveMagazzino_Default(Piva, Sa_Cod, Fabbricato_Cod, Impostazione_Valore_1)

                'CaricaCombo_Fabbricati(Server, Session, Page, _
                '                        Me.Cmb_Magazzino, _
                '                        Nothing, _
                '                        Qs_Piva, _
                '                        Sa_Cod, _
                '                        MAGAZZINO, _
                '                        False, , , _
                '                        , , _
                '                        , _
                '                        " ORDER BY Fabbricati.Fabbricato_Des ", True, _
                '                        Fabbricato_Cod)

                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.Fabbricati(Me.Cmb_Magazzino,
                                                                False, "", "",
                                                                Qs_Piva,
                                                                Sa_Cod,
                                                                Fabbricato_Cod,
                                                                MAGAZZINO,
                                                                 True,
                                                                 "",
                                                                 " Fabbricati.Fabbricato_Des ",
                                                                 AGRODATAFINE,
                                                                 objParametri_Server)

                Cambia_Selezione_Magazzino()

            End If


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato il seguente errore: " + ex.Message, Page)
        End Try


    End Sub

    '##################################################################################
    Private Sub Btn_CalcolaProgressivi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcolaProgressivi.Click

        If Me.Txt_DataRegPom.Text <> "" Then
            Calcola_Progressivi(Year(Me.Txt_DataRegPom.Text))
        Else
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("E' necessario specificare la data di stampa del Registro di Carico e Scarico del Pomodoro!", Page)
        End If

    End Sub

    '##################################################################################
    Private Sub Rbl_PomodoroContrattato_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Rbl_PomodoroContrattato.SelectedIndexChanged

        Calcola_Progressivi(Year(Me.Txt_DataRegPom.Text))

    End Sub

    '##################################################################################
    Private Sub Calcola_Progressivi(ByRef Anno As Integer)

        Dim Codice_Riga As Integer
        Dim Codice_Pagina As Integer
        Dim Nuovo_Valore_Pag As Integer = 0
        Dim Nuovo_Valore_Riga As Integer = 0

        Select Case Me.Rbl_PomodoroContrattato.SelectedValue
            Case 0
                Codice_Pagina = SEQ_PROG_NUMPAG_RegCS_PomoContrattato
                Codice_Riga = SEQ_PROG_NUMRIGA_RegCS_PomoContrattato
            Case 1
                Codice_Pagina = SEQ_PROG_NUMPAG_RegCS_PomoNoContrattato
                Codice_Riga = SEQ_PROG_NUMRIGA_RegCS_PomoNoContrattato
        End Select


        Dim objSeqProg_R As New AgronicaCoreDataProvider.Sequenza_Progressivi_R

        Try

            'nuovo progressivo di pagina
            Nuovo_Valore_Pag = objSeqProg_R.Nuovo_Progressivo(Qs_Piva, _
                                                            Anno, _
                                                            Codice_Pagina, _
                                                            "", _
                                                            "", _
                                                            0, _
                                                             Session("ASG_objParametri_Server"))

            'nuovo progressivo di riga
            Nuovo_Valore_Riga = objSeqProg_R.Nuovo_Progressivo(Qs_Piva, _
                                                                Anno, _
                                                                Codice_Riga, _
                                                                "", _
                                                                "", _
                                                                0, _
                                                                Session("ASG_objParametri_Server"))


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(ex.Message, Page)
        End Try


        Me.Txt_PagRegPom.Text = CStr(Nuovo_Valore_Pag)
        Me.Txt_RigaRegPom.Text = CStr(Nuovo_Valore_Riga)


    End Sub


    '##################################################################################
    Private Sub Stampanti_Load()

        Dim i As Integer
        Dim NomeStampante As String

        Me.Cmb_Stampante.Items.Clear()

        '==============================================
        '--- Stampanti installate nel pc
        Dim PP As New Printing.PageSettings
        Dim Lista_Stampanti As System.Drawing.Printing.PrinterSettings.StringCollection = Printing.PrinterSettings.InstalledPrinters
        For i = 0 To Lista_Stampanti.Count - 1

            NomeStampante = Lista_Stampanti.Item(i)

            Me.Cmb_Stampante.Items.Add(New ListItem(NomeStampante, i))

            ''evidenzia predefinita
            'If NomeStampante = a.PrinterSettings.PrinterName() Then
            'End If

        Next
        '==============================================


        '==============================================
        '--- Stampanti di rete
        ' Use the ObjectQuery to get the list of configured printers
        'Dim oquery As System.Management.ObjectQuery = New System.Management.ObjectQuery("SELECT * FROM Win32_Printer")

        'Dim mosearcher As System.Management.ManagementObjectSearcher = New System.Management.ManagementObjectSearcher(oquery)

        'Dim moc As System.Management.ManagementObjectCollection = mosearcher.Get()

        'For Each mo As ManagementObject In moc
        '    Dim pdc As System.Management.PropertyDataCollection = mo.Properties
        '    For Each pd As System.Management.PropertyData In pdc
        '        If CBool(mo("Network")) Then

        '            'cmbPrinters.Items.Add(mo(pd.Name))
        '            Me.Cmb_Stampante.Items.Add(New ListItem(mo(pd.Name), mo(pd.Name)))

        '        End If
        '    Next pd
        'Next mo


        'Dim searcher As New ManagementObjectSearcher("Select * From Win32_Printer Where Network = True")
        'For Each queryObj As ManagementObject In searcher.Get()
        '    Me.Cmb_Stampante.Items.Add(queryObj("DeviceID").ToString())
        'Next

        'On Error Resume Next
        'Dim strComputer As String = "."
        'Dim objWMIService As Object
        'objWMIService = GetObject("winmgmts:\\" & strComputer & "\root\cimv2")
        'Dim colItems As Object
        'colItems = objWMIService.ExecQuery("Select * from Win32_Printer", , 48)
        'Dim objItem As Object
        'For Each objItem In colItems
        '    Debug.WriteLine("Attributes: " & objItem.Attributes)
        '    Debug.WriteLine("Availability: " & objItem.Availability)
        '    Debug.WriteLine("AveragePagesPerMinute: " & objItem.AveragePagesPerMinute)
        '    Debug.WriteLine("Capabilities: " & objItem.Capabilities)
        '    Debug.WriteLine("CapabilityDescriptions: " & objItem.CapabilityDescriptions)
        '    Debug.WriteLine("Caption: " & objItem.Caption)
        '    Debug.WriteLine("ConfigManagerErrorCode: " & objItem.ConfigManagerErrorCode)
        '    Debug.WriteLine("ConfigManagerUserConfig: " & objItem.ConfigManagerUserConfig)
        '    Debug.WriteLine("CreationClassName: " & objItem.CreationClassName)
        '    Debug.WriteLine("DefaultPriority: " & objItem.DefaultPriority)
        '    Debug.WriteLine("Description: " & objItem.Description)
        '    Debug.WriteLine("DetectedErrorState: " & objItem.DetectedErrorState)
        '    Debug.WriteLine("DeviceID: " & objItem.DeviceID)
        '    Debug.WriteLine("DriverName: " & objItem.DriverName)
        '    Debug.WriteLine("ErrorCleared: " & objItem.ErrorCleared)
        '    Debug.WriteLine("ErrorDescription: " & objItem.ErrorDescription)
        '    Debug.WriteLine("HorizontalResolution: " & objItem.HorizontalResolution)
        '    Debug.WriteLine("InstallDate: " & objItem.InstallDate)
        '    Debug.WriteLine("JobCountSinceLastReset: " & objItem.JobCountSinceLastReset)
        '    Debug.WriteLine("LanguagesSupported: " & objItem.LanguagesSupported)
        '    Debug.WriteLine("LastErrorCode: " & objItem.LastErrorCode)
        '    Debug.WriteLine("Location: " & objItem.Location)
        '    Debug.WriteLine("Name: " & objItem.Name)
        '    Debug.WriteLine("PaperSizesSupported: " & objItem.PaperSizesSupported)
        '    Debug.WriteLine("PNPDeviceID: " & objItem.PNPDeviceID)
        '    Debug.WriteLine("PortName: " & objItem.PortName)
        '    Debug.WriteLine("PowerManagementCapabilities: " & objItem.PowerManagementCapabilities)
        '    Debug.WriteLine("PowerManagementSupported: " & objItem.PowerManagementSupported)
        '    Debug.WriteLine("PrinterPaperNames: " & objItem.PrinterPaperNames)
        '    Debug.WriteLine("PrinterState: " & objItem.PrinterState)
        '    Debug.WriteLine("PrinterStatus: " & objItem.PrinterStatus)
        '    Debug.WriteLine("PrintJobDataType: " & objItem.PrintJobDataType)
        '    Debug.WriteLine("PrintProcessor: " & objItem.PrintProcessor)
        '    Debug.WriteLine("SeparatorFile: " & objItem.SeparatorFile)
        '    Debug.WriteLine("ServerName: " & objItem.ServerName)
        '    Debug.WriteLine("ShareName: " & objItem.ShareName)
        '    Debug.WriteLine("SpoolEnabled: " & objItem.SpoolEnabled)
        '    Debug.WriteLine("StartTime: " & objItem.StartTime)
        '    Debug.WriteLine("Status: " & objItem.Status)
        '    Debug.WriteLine("StatusInfo: " & objItem.StatusInfo)
        '    Debug.WriteLine("SystemCreationClassName: " & objItem.SystemCreationClassName)
        '    Debug.WriteLine("SystemName: " & objItem.SystemName)
        '    Debug.WriteLine("TimeOfLastReset: " & objItem.TimeOfLastReset)
        '    Debug.WriteLine("UntilTime: " & objItem.UntilTime)
        '    Debug.WriteLine("VerticalResolution: " & objItem.VerticalResolution)
        'Next
        '==============================================


        'Dim printDocument1 As New PrintDocument
        'Dim nomestamp As String
        'nomestamp = printDocument1.PrinterSettings.PrinterName

        '==============================================


        '==============================================
        '--- Aggiungi stampanti a mano dal web.config
        Dim Vet_Stampanti As String()
        Vet_Stampanti = Recupera_Lista_Stampanti_ByWebConfig()

        If Not IsNothing(Vet_Stampanti) Then

            For i = 0 To Vet_Stampanti.Length - 1

                NomeStampante = Vet_Stampanti(i)

                If NomeStampante <> "" Then

                    Me.Cmb_Stampante.Items.Add(New ListItem(NomeStampante, CStr(i + 50)))

                End If

            Next

        End If
        '==============================================


        '==============================================
        '-- Stampante predefinita letta dal Giaslan
        Me.Cmb_Stampante.Items.Add(New ListItem("Stampante predefinita - by GiasLan", -1))


        '==============================================
        '--- Seleziona stampante predefinita
        'Me.Cmb_Stampante.SelectedIndex = _
        ' Me.Cmb_Stampante.Items.IndexOf(Me.Cmb_Stampante.Items.FindByValue( _
        '     PP.PrinterSettings.PrinterName()))

        Me.Cmb_Stampante.SelectedIndex = _
            Me.Cmb_Stampante.Items.IndexOf(Me.Cmb_Stampante.Items.FindByText( _
                PP.PrinterSettings.PrinterName()))

        'If Me.Cmb_Stampante.Items.Count = 0 Then
        '    Me.Cmb_Stampante.Items.Add(New ListItem("<Nessuna stampante installata>", -1))
        'End If

        '==============================================

        'Nel caso di fruttagel imposto di default la stampante della pesa
        Select Case CInt(Session("ASG_ProgressivoGIAS"))
            Case enum_CodiceGIAS_Clienti.Fruttagel
                Me.Cmb_Stampante.SelectedIndex = _
                    Me.Cmb_Stampante.Items.IndexOf(Me.Cmb_Stampante.Items.FindByText( _
                        "\\FRGPRINT\HPLPESA2"))

        End Select




    End Sub



    '##################################################################################
    Private Sub Cmb_Magazzino_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Magazzino.SelectedIndexChanged

        Cambia_Selezione_Magazzino()

    End Sub


    '##################################################################################
    Private Sub Cambia_Selezione_Magazzino()

        Select Case CInt(Session("ASG_ProgressivoGIAS"))

            Case enum_CodiceGIAS_Clienti.Fruttagel

                Dim objADD As New AgronicaCoreContabHLP.AccettazioneDaDiversi

                AgronicaCoreUtility.CaricaListControl.Prefisso_BolleAccettazione_FRG(Me.Cmb_Prefisso_DaNumeroBolla, _
                                                                                        False, "", "", _
                                                                                        Me.Cmb_Magazzino.SelectedItem.Text.ToLower)

                AgronicaCoreUtility.CaricaListControl.Prefisso_BolleAccettazione_FRG(Me.Cmb_Prefisso_ANumeroBolla, _
                                                                        False, "", "", _
                                                                        Me.Cmb_Magazzino.SelectedItem.Text.ToLower)

                Me.Txt_Prefisso_DaNumeroBolla_D.Text = objADD.Ricava_PrefissoBolleAccettazione_FRG(Me.Cmb_Magazzino.SelectedItem.Text)
                Me.Txt_Prefisso_ANumeroBolla_D.Text = objADD.Ricava_PrefissoBolleAccettazione_FRG(Me.Cmb_Magazzino.SelectedItem.Text)

                Me.Cmb_Prefisso_DaNumeroBolla.SelectedIndex = Me.Cmb_Prefisso_DaNumeroBolla.Items.IndexOf(Me.Cmb_Prefisso_DaNumeroBolla.Items.FindByText(Me.Txt_Prefisso_DaNumeroBolla_D.Text))
                Me.Cmb_Prefisso_ANumeroBolla.SelectedIndex = Me.Cmb_Prefisso_ANumeroBolla.Items.IndexOf(Me.Cmb_Prefisso_ANumeroBolla.Items.FindByText(Me.Txt_Prefisso_ANumeroBolla_D.Text))

                'Me.Txt_Prefisso_DaNumeroBolla.Text = objADD.Ricava_PrefissoBolleAccettazione_FRG(Me.Cmb_Magazzino.SelectedItem.Text.ToLower)
                'Me.Txt_Prefisso_ANumeroBolla.Text = objADD.Ricava_PrefissoBolleAccettazione_FRG(Me.Cmb_Magazzino.SelectedItem.Text.ToLower)

                ''If InStr(Me.Cmb_Magazzino.SelectedItem.Text.ToLower, "larino", ) > 0 Then
                ''    Me.Txt_Prefisso_DaNumeroBolla.Text = objADD.Ricava_Prefisso_Fruttagel(7)
                ''    Me.Txt_Prefisso_ANumeroBolla.Text = objADD.Ricava_Prefisso_Fruttagel(7)
                ''Else
                ''    Me.Txt_Prefisso_DaNumeroBolla.Text = objADD.Ricava_Prefisso_Fruttagel(1)
                ''    Me.Txt_Prefisso_ANumeroBolla.Text = objADD.Ricava_Prefisso_Fruttagel(1)
                ''End If

        End Select


    End Sub


    '##################################################################################
    Private Sub Btn_Da_Conferente_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Da_Conferente.Click

        Try

            Me.Txt_Da_Conferente.Text = ""

            If Not IsNumeric(Me.Txt_Da_CodiceConferente.Text) Or _
             InStr(Me.Txt_Da_CodiceConferente.Text, ".") > 0 Or _
                 InStr(Me.Txt_Da_CodiceConferente.Text, ",") > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il codice conferente DA deve essere un numero!", Page)
                Exit Sub

            End If

            Dim Cod_Contatto As String

            Me.Txt_Da_Conferente.Text = Carica_Conferente(Me.Txt_Da_CodiceConferente.Text, Cod_Contatto)
            Me.Txt_CodContatto_DA_Conferente.Text = Cod_Contatto

            If Me.Txt_Da_Conferente.Text = "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non esiste un conferente con codice " + Me.Txt_Da_CodiceConferente.Text + "!", Page)
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato il seguente errore: " + ex.Message, Page)
        End Try


    End Sub

    '##################################################################################
    Private Sub Btn_A_Conferente_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_A_Conferente.Click

        Try

            Me.Txt_A_Conferente.Text = ""

            If Not IsNumeric(Me.Txt_A_CodiceConferente.Text) Or _
                InStr(Me.Txt_A_CodiceConferente.Text, ".") > 0 Or _
                    InStr(Me.Txt_A_CodiceConferente.Text, ",") > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il codice conferente A deve essere un numero!", Page)
                Exit Sub

            End If

            Dim Cod_Contatto As String

            Me.Txt_A_Conferente.Text = Carica_Conferente(Me.Txt_A_CodiceConferente.Text, Cod_Contatto)
            Me.Txt_CodContatto_A_Conferente.Text = Cod_Contatto

            If Me.Txt_A_Conferente.Text = "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non esiste un conferente con codice " + Me.Txt_A_CodiceConferente.Text + "!", Page)
            End If

            If Not IsNumeric(Me.Txt_Da_CodiceConferente.Text) Or _
                InStr(Me.Txt_Da_CodiceConferente.Text, ".") > 0 Or _
                    InStr(Me.Txt_Da_CodiceConferente.Text, ",") > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il codice conferente DA deve essere un numero!", Page)
                Exit Sub

            End If

            If (Me.Txt_Da_CodiceConferente.Text <> "" And _
                Me.Txt_A_CodiceConferente.Text = "") Or _
                    (Me.Txt_Da_CodiceConferente.Text = "" And _
                        Me.Txt_A_CodiceConferente.Text <> "") Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Per filtrare un range di codici, è necessario specificare sia il codice DA che il codice A!", Page)
                Exit Sub

            End If

            Dim Messaggio As String = ""

            Verifica_DA_A_Conferenti(Messaggio)

            If Messaggio <> "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
            End If


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato il seguente errore: " + ex.Message, Page)
        End Try


    End Sub



    '##################################################################################
    Private Sub Verifica_DA_A_Conferenti(ByRef Messaggio As String)

        Me.Txt_SringaFiltroConferenti.Text = ""

        Try

            'se sono vuoti, non filtra nulla
            'altrimenti viene preparato il filtro per codice conf o intervallo conferenti
            If Me.Txt_Da_CodiceConferente.Text <> "" And Me.Txt_A_CodiceConferente.Text <> "" Then

                If Not IsNumeric(Me.Txt_Da_CodiceConferente.Text) Or Not IsNumeric(Me.Txt_A_CodiceConferente.Text) Then
                    Messaggio = "I codici conferente devono essere numerici."
                    Exit Sub
                End If

                'se sono uguali, si filtra solo un conf
                If Me.Txt_Da_CodiceConferente.Text <> Me.Txt_A_CodiceConferente.Text Then

                    If CInt(Me.Txt_A_CodiceConferente.Text) < CInt(Me.Txt_Da_CodiceConferente.Text) Then
                        Messaggio = "Il codice A conferente deve essere maggiore rispetto al codice DA conferente."
                        Exit Sub
                    End If

                    'compongo la stringa dei progressivi da filtrare
                    Dim DT_Conf As DataTable
                    Dim i As Integer
                    Dim Str_Progressivi As String = ""

                    DT_Conf = Leggi_Range_Conferenti(CInt(Me.Txt_Da_CodiceConferente.Text), CInt(Me.Txt_A_CodiceConferente.Text))

                    If Not IsNothing(DT_Conf) Then

                        For i = 0 To DT_Conf.Rows.Count - 1
                            Str_Progressivi += "'" + CStr(DT_Conf.Rows(i).Item("Settore_Des")) + "'" + ", "
                        Next
                        If Str_Progressivi <> "" Then
                            Str_Progressivi = "( " + Left(Str_Progressivi, Str_Progressivi.Length - 2) + ")"
                        End If

                        Me.Txt_SringaFiltroConferenti.Text = Str_Progressivi

                    End If

                End If

            End If

        Catch ex As Exception
            Messaggio = "Si è verificato il seguente errore: " + ex.Message
        End Try


    End Sub

    '##################################################################################
    Private Function Leggi_Range_Conferenti(ByVal Progressivo_Inizio As Integer, _
                                            ByVal Progressivo_Fine As Integer) As DataTable


        'prima di fare la query di lettura dei progressivi,
        'occorre fare una query di update per impostare a 0 i NULL e i blank


        Dim Risposta As Boolean
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objAgronicaCoreUtility As New AgronicaCoreUtility.QueryParametrizzata_W

        Try

            Dim Connessione As OleDb.OleDbConnection
            Dim Transazione As OleDb.OleDbTransaction

            Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider


            Dim ErrMSG As String

            ' APRO LA CONNESSIONE AL DATABASE
            Connessione = objAgronicaCore.ApriConnessione(objParametri_Server, ErrMSG)
            Transazione = Connessione.BeginTransaction()

            Risposta = objAgronicaCoreUtility.Modifica_Parametrizzata(
                                                        "Risorse_Umane", _
                                                        "Settore_Des", _
                                                        "0", _
                                                        "WHERE Settore_Des IS NULL OR Settore_Des = '' ", _
                                                        objParametri_Server)

  

            '=================================
            '======== TRANSAZIONE OK =========
            Transazione.Commit()
            Connessione.Close()
            '=================================

        Catch ex As Exception

        End Try


        Dim DT_Conf As DataTable
        Dim FiltroAggiuntivo, Ordinamento As String

        FiltroAggiuntivo += " 1=1 "
        FiltroAggiuntivo += " AND ( CONVERT(int, Risorse_Umane.Settore_Des) >= " + Agro_SQL_SaveText(CStr(Progressivo_Inizio)) + " "
        FiltroAggiuntivo += " AND CONVERT(int, Risorse_Umane.Settore_Des) <= " + Agro_SQL_SaveText(CStr(Progressivo_Fine)) + " ) "

        Ordinamento = " Risorse_Umane.Settore_Des "

        DT_Conf = objContatti.Contatti_Contatto_Leggi("", "", _
                                                 0, 0, False, False, 0, 0, 0, 0, _
                                                FRUTTAGEL_COD_CLIENTE_FORNITORE, _
                                                0, "", False, 0, 0, 0, 0, 0, _
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                FiltroAggiuntivo, _
                                                Ordinamento, _
                                                objParametri_Server)


        Return DT_Conf


    End Function



    '##################################################################################
    Private Function Carica_Conferente(ByVal Progressivo As Integer, _
                                        ByRef Cod_Contatto As String) As String

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        Try

            Dim DT_Conf As DataTable

            DT_Conf = objContatti.Contatti_Contatto_Leggi("", "", _
                                                 0, 0, False, False, 0, 0, 0, 0, _
                                                 ID_CF_NOFILTRO, _
                                                 0, "", False, 0, 0, 0, 0, 0, _
                                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                 "", _
                                                 "", _
                                                 objParametri_Server, _
                                                     CStr(Progressivo))

            If Not IsNothing(DT_Conf) AndAlso DT_Conf.Rows.Count <> 0 Then
                Cod_Contatto = DT_Conf.Rows(0).Item("Cod_Contatto")
                Return DT_Conf.Rows(0).Item("Rag_Soc")
            Else
                Return ""
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento del conferente. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try

    End Function


    '##################################################################################
    Private Sub Btn_Da_Specie_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Da_Specie.Click

        Try
            Me.Txt_Da_Specie.Text = ""

            If Not IsNumeric(Me.Txt_Da_CodiceSpecie.Text) Or _
                InStr(Me.Txt_Da_CodiceSpecie.Text, ".") > 0 Or _
                    InStr(Me.Txt_Da_CodiceSpecie.Text, ",") > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il codice specie deve essere un numero!", Page)
                Exit Sub

            End If

            Me.Txt_Da_Specie.Text = Carica_Specie(Me.Txt_Da_CodiceSpecie.Text)

            If Me.Txt_Da_Specie.Text = "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non esiste una specie con codice " + Me.Txt_Da_CodiceSpecie.Text + "!", Page)
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato il seguente errore: " + ex.Message, Page)
        End Try

    End Sub


    '##################################################################################
    Private Sub Btn_A_Specie_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_A_Specie.Click

        Try

            Me.Txt_A_Specie.Text = ""

            If Not IsNumeric(Me.Txt_A_CodiceSpecie.Text) Or _
                InStr(Me.Txt_A_CodiceSpecie.Text, ".") > 0 Or _
                    InStr(Me.Txt_A_CodiceSpecie.Text, ",") > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il codice specie A deve essere un numero!", Page)
                Exit Sub

            End If


            Me.Txt_A_Specie.Text = Carica_Specie(Me.Txt_A_CodiceSpecie.Text)

            If Me.Txt_A_Specie.Text = "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non esiste una specie con codice " + Me.Txt_A_CodiceSpecie.Text + "!", Page)
            End If


            If Not IsNumeric(Me.Txt_Da_CodiceSpecie.Text) Or _
                    InStr(Me.Txt_Da_CodiceSpecie.Text, ".") > 0 Or _
                        InStr(Me.Txt_Da_CodiceSpecie.Text, ",") > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Il codice specie DA deve essere un numero!", Page)
                Exit Sub

            End If

            If (Me.Txt_Da_CodiceSpecie.Text <> "" And _
                Me.Txt_A_CodiceSpecie.Text = "") Or _
                    (Me.Txt_Da_CodiceSpecie.Text = "" And _
                        Me.Txt_A_CodiceSpecie.Text <> "") Then

                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Per filtrare un range di codici, è necessario specificare sia il codice DA che il codice A!", Page)
                Exit Sub

            End If

            Dim Messaggio As String = ""

            Verifica_DA_A_Specie(Messaggio)

            If Messaggio <> "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
            End If


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato il seguente errore: " + ex.Message, Page)
        End Try


    End Sub


    '##################################################################################
    Private Sub Verifica_DA_A_Specie(ByRef Messaggio As String)

        Me.Txt_SringaFiltroSpecie.Text = ""

        Try

            'se sono vuoti, non filtra nulla
            'altrimenti viene preparato il filtro per codice specie o intervallo specie
            If Me.Txt_Da_CodiceSpecie.Text <> "" And Me.Txt_A_CodiceSpecie.Text <> "" Then

                If Not IsNumeric(Me.Txt_Da_CodiceSpecie.Text) Or Not IsNumeric(Me.Txt_A_CodiceSpecie.Text) Then
                    Messaggio = "I codici specie devono essere numerici."
                    Exit Sub
                End If

                'se sono uguali, si filtra solo una specie
                If Me.Txt_Da_CodiceSpecie.Text <> Me.Txt_A_CodiceSpecie.Text Then

                    If CInt(Me.Txt_A_CodiceSpecie.Text) < CInt(Me.Txt_Da_CodiceSpecie.Text) Then
                        Messaggio = "Il codice A specie deve essere maggiore rispetto al codice DA specie"
                        Exit Sub
                    End If

                    'compongo la stringa dei progressivi da filtrare
                    Dim DT_Specie As DataTable
                    Dim i As Integer
                    Dim Str_CodArticolo As String = ""

                    DT_Specie = Leggi_Range_Specie(CInt(Me.Txt_Da_CodiceSpecie.Text), CInt(Me.Txt_A_CodiceSpecie.Text))

                    If Not IsNothing(DT_Specie) Then

                        For i = 0 To DT_Specie.Rows.Count - 1
                            Str_CodArticolo += "'" + CStr(DT_Specie.Rows(i).Item("Cod_Articolo")) + "'" + ", "
                        Next
                        If Str_CodArticolo <> "" Then
                            Str_CodArticolo = "( " + Left(Str_CodArticolo, Str_CodArticolo.Length - 2) + ")"
                        End If

                        Me.Txt_SringaFiltroSpecie.Text = Str_CodArticolo

                    End If

                End If

            End If

        Catch ex As Exception
            Messaggio += "Si è verificato il seguente errore: " + ex.Message
        End Try


    End Sub

    '##################################################################################
    Private Function Leggi_Range_Specie(ByVal CodArticolo_Inizio As Integer, _
                                        ByVal CodArticolo_Fine As Integer) As DataTable





        Dim DT_Specie As DataTable
        Dim FiltroAggiuntivo, Ordinamento As String
        Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        FiltroAggiuntivo += " 1=1 "
        FiltroAggiuntivo += " AND ( CONVERT(int, Materie_Prime.Cod_Articolo) >= " + Agro_SQL_SaveText(CStr(CodArticolo_Inizio)) + " "
        FiltroAggiuntivo += " AND CONVERT(int, Materie_Prime.Cod_Articolo) <= " + Agro_SQL_SaveText(CStr(CodArticolo_Fine)) + " ) "

        Ordinamento = " Materie_Prime.Cod_Articolo "

        DT_Specie = objMateriePrime.Cod_Articolo_Leggi("", 0, _
                                                       TRASFORMATI_VEGETALI, _
                                                       0, 0, 0, 0, 0, 0, 0, 0, 0, _
                                                       CDate("01/01/1900"), CDate("31/12/2100"), _
                                                       "", 0, "", False, _
                                                       FiltroAggiuntivo, _
                                                       Ordinamento, _
                                                       objParametri_Server, _
                                                       objParametri_Utenti)

        'Cod_Articolo_Leggi(Server, Session, Page, _
        '                                      , , _
        '                                      TRASFORMATI_VEGETALI, _
        '                                      , , , , , , , , , , , , , , , _
        '                                      FiltroAggiuntivo, _
        '                                      Ordinamento)
        Return DT_Specie


    End Function



    '##################################################################################
    Private Function Carica_Specie(ByVal Codice_Articolo As Integer) As String



        Try

            Dim DT_Specie As DataTable
            Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R


            'DT_Specie = NewCom_MateriePrime_Leggi(Server, Session, Page, _
            '                                      , , _
            '                                      TRASFORMATI_VEGETALI, _
            '                                      , _
            '                                      CStr(Codice_Articolo), _
            '                                      , , , , , , , , , , , , )


            DT_Specie = objMateriePrime.Leggi("", 0, _
                                               TRASFORMATI_VEGETALI, _
                                               0, _
                                               CStr(Codice_Articolo), _
                                               0, 0, 0, 0, 0, 0, 0, "", 0, "", False, False, "", _
                                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                               "", "", objParametri_Server)


            If Not IsNothing(DT_Specie) AndAlso DT_Specie.Rows.Count <> 0 Then
                'Fruttagel 
                'il mat_des contiene "-" : su altri archivi questa istruzione va in errore 
                Return CStr(DT_Specie.Rows(0).Item("Mat_des")).Split("-")(0)
            Else
                Return ""
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento della specie. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try


    End Function


    '##################################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Stampa()

    End Sub


    '##################################################################################
    Private Sub Stampa()

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

        Try

            Dim QueryString As String

            Dim Validita_Inizio As String = "01/01/1900"
            Dim Validita_Fine As String = "31/12/2100"
            Dim Data_Giacenza As String
            Dim Mat_Cod As Integer = 0
            Dim Sa_Cod As Integer = 0
            Dim Fabbricato_Cod As Integer = 0
            Dim Codice_Specie As Integer = 0
            Dim DA_Codice_Specie As Integer = 0
            Dim A_Codice_Specie As Integer = 0
            Dim Str_Codici_Specie As String = ""
            Dim Codice_Conferente As Integer = 0
            Dim DA_Codice_Conferente As Integer = 0
            Dim A_Codice_Conferente As Integer = 0
            Dim Str_Codici_Conferenti As String = ""
            Dim Piva_Produttore As String = ""
            Dim Piva_Coop1 As String = ""
            Dim Piva_Coop2 As String = ""
            Dim Log_Errori As String = ""
            'Dim DA_NumeroBolla As String = ""
            'Dim A_NumeroBolla As String = ""
            Dim Str_Id_Agenda_Bolle As String = ""
            Dim Str_Flag_Fascicola As String
            Dim Numero_Copie As Integer
            Dim Regolamento_cod As Integer = 0

            Dim RagSoc_Impresa As String = ""
            Dim Descr_Specie As String = ""
            Dim Descr_Prodotto As String = ""
            Dim Descr_Magazzino As String = ""
            Dim Tracciabilita_Impianti As Integer = 0
            Dim Data_CS_Pom As String
            Dim Pag_CS_Pom As Integer
            Dim Riga_CS_Pom As Integer
            Dim Tipo_Pom As Integer
            Dim Flag_SalvaPagRiga As Boolean


            Coltrolla_RecuperaFiltri(Me.Rbl_Report.SelectedValue, _
                                    Log_Errori, _
                                    Validita_Inizio, _
                                    Validita_Fine, _
                                    Codice_Specie, _
                                    Str_Codici_Specie, _
                                    Codice_Conferente, _
                                    Str_Codici_Conferenti, _
                                    Piva_Produttore, _
                                    Piva_Coop1, _
                                    Piva_Coop2, _
                                    Mat_Cod, _
                                    Sa_Cod, _
                                    Fabbricato_Cod, _
                                    Descr_Specie, _
                                    Descr_Prodotto, _
                                    Descr_Magazzino, _
                                    Tracciabilita_Impianti, _
                                    Str_Id_Agenda_Bolle, _
                                    Str_Flag_Fascicola, _
                                    Numero_Copie, _
                                    Data_CS_Pom, _
                                    Pag_CS_Pom, _
                                    Riga_CS_Pom, _
                                    Tipo_Pom, _
                                    Flag_SalvaPagRiga, _
                                    Data_Giacenza, _
                                    Regolamento_cod)

            If Log_Errori <> "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Log_Errori, Page)
                Exit Sub
            End If

            Session("Str_Codici_Conferenti") = Str_Codici_Conferenti
            Session("Str_Codici_Specie") = Str_Codici_Specie
            Session("Str_Id_Agenda_Bolle") = Str_Id_Agenda_Bolle

            Select Case Me.Rbl_Report.SelectedValue

                Case enum_CodificaStampe.ADD_Riepilogo_Conf_XSpecie

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                    "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                    "&cs=" + Stringa_Codifica(CStr(Codice_Specie), AgroKey_EncoderDecoder, Server) + _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&spe=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Specie)), AgroKey_EncoderDecoder, Server) + _
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server)

                    'li passo con la session, altrimenti la querystring diventa troppo lunga e da errore
                    '"&sfc=" + Stringa_Codifica(Str_Codici_Conferenti, AgroKey_EncoderDecoder, Server) + _


                    'Response.Redirect("RiepilogoConferimentoXSpecie/RiepilogoConferimentoXSpecie.aspx" + QueryString)


                    Page_NewWindow_2010(Page, _
                                    "RiepilogoConferimentoXSpecie/RiepilogoConferimentoXSpecie.aspx", _
                                    QueryString, _
                                    "RiepilogoConferimentoXSpecie", _
                                    , , , , , , , )


                    '------------------------------------
                Case enum_CodificaStampe.ADD_EC_Bolle_Accettazione_DaDiversi

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                    "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                    "&cs=" + Stringa_Codifica(CStr(Codice_Specie), AgroKey_EncoderDecoder, Server) + _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&pp=" + Stringa_Codifica(Piva_Produttore, AgroKey_EncoderDecoder, Server) + _
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&spe=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Specie)), AgroKey_EncoderDecoder, Server) + _
                                    "&pro=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) + _
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page, _
                                    "EstrattoConto_BolleAccettazione/EC_BolleAccettazione.aspx", _
                                    QueryString, _
                                    "EstrattoConto_BolleAccettazione", _
                                    , , , , , , , )


                    '------------------------------------
                Case enum_CodificaStampe.ADD_EC_Imballi

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                    "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&imb=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) + _
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page, _
                                    "EstrattoConto_Imballi/EC_Imballi.aspx", _
                                    QueryString, _
                                    "EstrattoConto_Imballi", _
                                    , , , , , , , )

                    '------------------------------------
                Case enum_CodificaStampe.ADD_Saldo_Imballi

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                    "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&imb=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) + _
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) + _
                                    "&dg=" + Stringa_Codifica(Data_Giacenza, AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page, _
                                    "SaldoImballi/SaldoImballi.aspx", _
                                    QueryString, _
                                    "SaldoImballi", _
                                    , , , , , , , )

                    '------------------------------------
                Case enum_CodificaStampe.ADD_Export_Bolle_Accettazione_DaDiversi

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                    "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                    "&cs=" + Stringa_Codifica(CStr(Codice_Specie), AgroKey_EncoderDecoder, Server) + _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&pp=" + Stringa_Codifica(Piva_Produttore, AgroKey_EncoderDecoder, Server) + _
                                    "&pc1=" + Stringa_Codifica(Piva_Coop1, AgroKey_EncoderDecoder, Server) + _
                                    "&pc2=" + Stringa_Codifica(Piva_Coop2, AgroKey_EncoderDecoder, Server) + _
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&spe=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Specie)), AgroKey_EncoderDecoder, Server) + _
                                    "&pro=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) + _
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) + _
                                    "&ti=" + Stringa_Codifica(CStr(Tracciabilita_Impianti), AgroKey_EncoderDecoder, Server)


                    Page_NewWindow_2010(Page, _
                                    "ExportBolleAccettazione/BolleAccettazione_XLS.aspx", _
                                    QueryString, _
                                    "RiepilogoBolleAccettazione", _
                                    , , , , , , , )

                    '-----------------------------------------

                Case enum_CodificaStampe.ADD_Export_Traportatori

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                   "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                   "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                   "&cs=" + Stringa_Codifica(CStr(Codice_Specie), AgroKey_EncoderDecoder, Server) + _
                                   "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                   "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                   "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                   "&spe=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Specie)), AgroKey_EncoderDecoder, Server) + _
                                   "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                                   "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page, _
                                    "ExportTrasportatori/Trasportatori_XLS.aspx", _
                                    QueryString, _
                                    "RiepilogoTrasportatori", _
                                    , , , , , , , )

                    '------------------------------------

                Case enum_CodificaStampe.Registro_CaricoScarico_Pomodoro

                    QueryString = "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&d=" + Stringa_Codifica(Data_CS_Pom, AgroKey_EncoderDecoder, Server) + _
                                    "&pg=" + Stringa_Codifica(CStr(Pag_CS_Pom), AgroKey_EncoderDecoder, Server) + _
                                    "&r=" + Stringa_Codifica(CStr(Riga_CS_Pom), AgroKey_EncoderDecoder, Server) + _
                                    "&t=" + Stringa_Codifica(CStr(Tipo_Pom), AgroKey_EncoderDecoder, Server) + _
                                    "&fsp=" + Stringa_Codifica(CStr(Flag_SalvaPagRiga), AgroKey_EncoderDecoder, Server) + _
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) + _
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page, _
                                    "RegistroCaricoScaricoPomodoro/RegistroCaricoScaricoPomodoro.aspx", _
                                    QueryString, _
                                    "RegistroCaricoScaricoPomodoro", _
                                    , , , , , , , )

                    '------------------------------------


                Case enum_CodificaStampe.Buono_Accettazione_Diversi 'stampa massiva

                    Me.Txt_TipoStampa.Text = CStr(enum_CodificaStampe.Buono_Accettazione_Diversi)

                    Me.Lbl_ConfermaStampaMassiva.Text = "Conferma Stampa Massiva Bolle"
                    'Me.Lbl_SelezioneStampaMassiva.Text = "Bolle selezionate per la stampa:"


                    'chiamata da stampa massiva bolle e certificati
                    StampaMassiva_ImpostaConferma(Codice_Specie, _
                                                    Str_Codici_Specie, _
                                                    Codice_Conferente, _
                                                    Str_Codici_Conferenti, _
                                                    Piva_Produttore, _
                                                    Piva_Coop1, _
                                                    Piva_Coop2)

                    'il codice seguente è spostato nella funzione chiamata sopra

                    'Me.Pannello_Generale.Visible = False
                    'Me.Pannello_ConfermaStampaMassiva.Visible = True

                    ''----- Imposto le dimensioni
                    'With Me.Pannello_ConfermaStampaMassiva
                    '    '.Height = Dimensione.Pixel(490)
                    '    '.Width = Dimensione.Pixel(480)
                    '    .Style.Item("Top") = "56px"
                    '    .Style.Item("Left") = "8px"
                    'End With

                    'Me.Txt_NomeStampante.Text = Me.Cmb_Stampante.SelectedItem.Text
                    'Me.Txt_NumeroCopie_Bis.Text = Me.Txt_NumeroCopie.Text

                    'Dim Num_Bolla_Da, Num_Bolla_A As String
                    'Dim Tot_Num_Bolle As Integer

                    'Num_Bolla_Da = Ricava_NumeroDocumento_Con_Sequenza(Me.Txt_Prefisso_DaNumeroBolla.Text, _
                    '                                                    Me.Txt_DaNumeroBolla.Text, _
                    '                                                    Me.Txt_Suffisso_DaNumeroBolla.Text, _
                    '                                                    3, _
                    '                                                    5, _
                    '                                                    0, _
                    '                                                    "0")

                    'Num_Bolla_A = Ricava_NumeroDocumento_Con_Sequenza(Me.Txt_Prefisso_ANumeroBolla.Text, _
                    '                                                   Me.Txt_ANumeroBolla.Text, _
                    '                                                   Me.Txt_Suffisso_ANumeroBolla.Text, _
                    '                                                   3, _
                    '                                                   5, _
                    '                                                   0, _
                    '                                                   "0")

                    'If IsNumeric(Num_Bolla_Da) And IsNumeric(Num_Bolla_A) Then
                    '    Tot_Num_Bolle = CInt(Num_Bolla_A) - CInt(Num_Bolla_Da) + 1
                    'End If

                    'Me.Txt_RiepilogoBolle.Text = "Dalla n." + Num_Bolla_Da + " alla n. " + Num_Bolla_A + " (in totale " + CStr(Tot_Num_Bolle) + " bolle)."

                    '------------------------------------

                Case enum_CodificaStampe.Certificato_Pomodoro 'stampa massiva

                    Select Case Me.Rbl_CertificatiStampaMassiva.SelectedValue

                        Case 1 'INTERNO
                            Session("ReportSelezionato") = enum_CodificaStampe.Certificato_Pomodoro_Interno

                        Case 2 'ESTERNO
                            Session("ReportSelezionato") = enum_CodificaStampe.Certificato_Pomodoro_Esterno
                    End Select

                    Me.Txt_TipoStampa.Text = CStr(enum_CodificaStampe.Certificato_Pomodoro)

                    Me.Lbl_ConfermaStampaMassiva.Text = "Conferma Stampa Massiva Certificati"
                    ''Me.Lbl_SelezioneStampaMassiva.Text = "Certificati selezionati per la stampa:"
                    'Me.Lbl_SelezioneStampaMassiva.Text = "Bolle selezionate per la stampa:"

                    'chiamata da stampa massiva bolle e certificati
                    StampaMassiva_ImpostaConferma(Codice_Specie, _
                                                    Str_Codici_Specie, _
                                                    Codice_Conferente, _
                                                    Str_Codici_Conferenti, _
                                                    Piva_Produttore, _
                                                    Piva_Coop1, _
                                                    Piva_Coop2)

                    'il codice seguente è sostituito con quello della stampa massiva bolle

                    'Me.Pannello_Generale.Visible = False
                    'Me.Pannello_ConfermaStampaMassiva.Visible = True

                    ''----- Imposto le dimensioni
                    'With Me.Pannello_ConfermaStampaMassiva
                    '    '.Height = Dimensione.Pixel(490)
                    '    '.Width = Dimensione.Pixel(480)
                    '    .Style.Item("Top") = "56px"
                    '    .Style.Item("Left") = "8px"
                    'End With

                    'Me.Txt_NomeStampante.Text = Me.Cmb_Stampante.SelectedItem.Text
                    'Me.Txt_NumeroCopie_Bis.Text = Me.Txt_NumeroCopie.Text

                    'Dim Num_Certificato_Da, Num_Certificato_A As String
                    'Dim Tot_Num_Certificati As Integer

                    'Num_Certificato_Da = Me.Txt_DaNumeroCert.Text
                    'Num_Certificato_A = Me.Txt_ANumeroCert.Text

                    'If IsNumeric(Num_Certificato_Da) And IsNumeric(Num_Certificato_A) Then
                    '    Tot_Num_Certificati = CInt(Num_Certificato_A) - CInt(Num_Certificato_Da) + 1
                    'End If

                    'Me.Txt_RiepilogoBolle.Text = "Dal n." + Num_Certificato_Da + " al n. " + Num_Certificato_A + " (in totale " + CStr(Tot_Num_Certificati) + " Certificati)."

                    '------------------------------------
                Case enum_CodificaStampe.ADD_ExcelTracciabilitaConferimenti

                    'QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) & _
                    '                "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) & _
                    '                 "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                    '                "&reg=" + Stringa_Codifica(, AgroKey_EncoderDecoder, Server)

                    QueryString = "?reg=" & Stringa_Codifica(Regolamento_cod, AgroKey_EncoderDecoder, Server) & _
                                    "&dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) & _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) & _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

                    '"&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                    '"&cs=" + Stringa_Codifica(CStr(Codice_Specie), AgroKey_EncoderDecoder, Server) + _
                    '"&pp=" + Stringa_Codifica(Piva_Produttore, AgroKey_EncoderDecoder, Server) + _
                    '"&pc1=" + Stringa_Codifica(Piva_Coop1, AgroKey_EncoderDecoder, Server) + _
                    '"&pc2=" + Stringa_Codifica(Piva_Coop2, AgroKey_EncoderDecoder, Server) + _
                    '"&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                    '"&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                    '"&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) + _
                    '"&spe=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Specie)), AgroKey_EncoderDecoder, Server) + _
                    '"&pro=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) + _
                    '"&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                    '"&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) + _
                    '"&ti=" + Stringa_Codifica(CStr(Tracciabilita_Impianti), AgroKey_EncoderDecoder, Server)


                    Page_NewWindow_2010(Page, _
                                    "ExportTracciabilitaConferimenti/TracciabilitaConferimenti_XLS.aspx", _
                                    QueryString, _
                                    "TracciabilitaConferimenti", _
                                     , , , , , , , )

                    '-----------------------------------------
                Case enum_CodificaStampe.ADD_ExportExcel_CertificatiPomodoro

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _
                                    "&cc=" + Stringa_Codifica(CStr(Codice_Conferente), AgroKey_EncoderDecoder, Server) + _
                                    "&cs=" + Stringa_Codifica(CStr(Codice_Specie), AgroKey_EncoderDecoder, Server) + _
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) + _
                                       "&pp=" + Stringa_Codifica(Piva_Produttore, AgroKey_EncoderDecoder, Server) + _
                                    "&pc1=" + Stringa_Codifica(Piva_Coop1, AgroKey_EncoderDecoder, Server)
                    '"&spe=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Specie)), AgroKey_EncoderDecoder, Server) + _
                    '"&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) + _
                    '"&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server)

                    Dim Pagina_ExportCertificatiPomodoro As String = objADDFun.PaginaExportCertificatiPomodoro_from_AnnoDataSelezionata(Me.Txt_ValiditaInizio.Text)

                    Page_NewWindow_2010(Page, _
                                    Pagina_ExportCertificatiPomodoro, _
                                    QueryString, _
                                    "CertificatiPomodoro_XLS", _
                                    , , , , , , , )

            End Select


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato il seguente errore durante la chiamata alla stampa: " + ex.Message, Page)
        End Try



    End Sub


    '##################################################################################
    Private Sub StampaMassiva_ImpostaConferma(ByRef Codice_Specie As Integer, _
                                            ByRef Str_Codici_Specie As String, _
                                            ByRef Codice_Conferente As Integer, _
                                            ByRef Str_Codici_Conferenti As String, _
                                            ByRef Piva_Produttore As String, _
                                            ByRef Piva_Coop1 As String, _
                                            ByRef Piva_Coop2 As String)

        Me.Pannello_Generale.Visible = False
        Me.Pannello_ConfermaStampaMassiva.Visible = True

        '----- Imposto le dimensioni
        With Me.Pannello_ConfermaStampaMassiva
            '.Height = Dimensione.Pixel(490)
            '.Width = Dimensione.Pixel(480)
            .Style.Item("Top") = "56px"
            .Style.Item("Left") = "8px"
        End With

        Me.Txt_NomeStampante.Text = Me.Cmb_Stampante.SelectedItem.Text
        Me.Txt_NumeroCopie_Bis.Text = Me.Txt_NumeroCopie.Text

        'Riepiloga il filtro impostato
        Dim Str_RiepilogoFiltro As String = ""

        If Me.Txt_DaNumeroBolla.Text <> "" And Me.Txt_ANumeroBolla.Text <> "" Then

            'Dim Tot_Num_Bolle As Integer
            Dim Num_Bolla_Da As String = ""
            Dim Num_Bolla_A As String = ""

            Ricava_NumBollaDa_NumBollaA(Num_Bolla_Da, Num_Bolla_A)

            'If IsNumeric(Num_Bolla_Da) And IsNumeric(Num_Bolla_A) Then
            '    Tot_Num_Bolle = CInt(Num_Bolla_A) - CInt(Num_Bolla_Da) + 1
            'End If

            Str_RiepilogoFiltro += "Dalla n." + Num_Bolla_Da + " alla n. " + Num_Bolla_A & vbCrLf '+ " (in totale " + CStr(Tot_Num_Bolle) + " bolle)." & vbCrLf

        Else
            If Me.Txt_DaNumeroBolla.Text <> "" Or Me.Txt_ANumeroBolla.Text <> "" Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Se si sceglie di filtrare per numeri bolla, occorre specificarli entrambi.", Page)
                Me.ImgBtn_StampaMassivaBolle.Visible = False
                Me.lbl_stampamassiva.Visible = False
            End If
        End If

        If Me.Txt_ValiditaInizio.Text <> "" Or Me.Txt_ValiditaFine.Text <> "" Then
            Str_RiepilogoFiltro += "Dal: " + Me.Txt_ValiditaInizio.Text + " al: " + Me.Txt_ValiditaFine.Text & vbCrLf
        End If

        If Codice_Specie <> 0 Then
            Str_RiepilogoFiltro += "Codice Specie: " + CStr(Codice_Specie) & vbCrLf
        End If

        If Str_Codici_Specie <> "" Then
            Str_RiepilogoFiltro += "Elenco Codici Specie: " + CStr(Str_Codici_Specie) & vbCrLf
        End If

        If Codice_Conferente <> 0 Then
            Str_RiepilogoFiltro += "Codice Conferente: " + CStr(Codice_Conferente) & vbCrLf
        End If

        If Str_Codici_Conferenti <> "" Then
            Str_RiepilogoFiltro += "Elenco Codici Conferenti: " + CStr(Str_Codici_Conferenti) & vbCrLf
        End If

        If Piva_Produttore <> "" Then
            Str_RiepilogoFiltro += "Piva Produttore: " + CStr(Piva_Produttore) & vbCrLf
        End If

        If Piva_Coop1 <> "" Then
            Str_RiepilogoFiltro += "Piva Cooperativa: " + CStr(Piva_Coop1) & vbCrLf
        End If
        If Piva_Coop2 <> "" Then
            Str_RiepilogoFiltro += "Piva seconda Cooperativa: " + CStr(Piva_Coop2) & vbCrLf
        End If

        Me.Txt_RiepilogoBolle.Text = Str_RiepilogoFiltro

        If Str_RiepilogoFiltro = "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stato impostato alcun criterio di filtro, non è possibile procedere con la stampa massiva.", Page)
            Me.ImgBtn_StampaMassivaBolle.Visible = False
            Me.lbl_stampamassiva.Visible = False
        Else
            Me.ImgBtn_StampaMassivaBolle.Visible = True
            Me.lbl_stampamassiva.Visible = True
        End If


    End Sub


    '##################################################################################
    Private Sub Ricava_NumBollaDa_NumBollaA(ByRef Num_Bolla_Da As String, _
                                                ByRef Num_Bolla_A As String)

        If Me.Txt_DaNumeroBolla.Text = "" Then
            Me.Txt_DaNumeroBolla.Text = "0"
        End If

        Dim Prefisso_Num_Bolla_DA, Prefisso_Num_Bolla_A As String

        Prefisso_Num_Bolla_DA = Me.Cmb_Prefisso_DaNumeroBolla.SelectedValue
        'Prefisso_Num_Bolla_dA = Me.Txt_Prefisso_dANumeroBolla.Text

        Num_Bolla_Da = Ricava_NumeroDocumento_Con_Sequenza(Prefisso_Num_Bolla_DA, _
                                                            Me.Txt_DaNumeroBolla.Text, _
                                                            Me.Txt_Suffisso_DaNumeroBolla.Text, _
                                                            3, _
                                                            5, _
                                                            0, _
                                                            "0")

        If Me.Txt_ANumeroBolla.Text = "" Then
            Me.Txt_ANumeroBolla.Text = "0"
        End If

        Prefisso_Num_Bolla_A = Me.Cmb_Prefisso_ANumeroBolla.SelectedValue
        'Prefisso_Num_Bolla_A = Me.Txt_Prefisso_ANumeroBolla.Text

        Num_Bolla_A = Ricava_NumeroDocumento_Con_Sequenza(Prefisso_Num_Bolla_A, _
                                                           Me.Txt_ANumeroBolla.Text, _
                                                           Me.Txt_Suffisso_ANumeroBolla.Text, _
                                                           3, _
                                                           5, _
                                                           0, _
                                                           "0")


    End Sub


    '##################################################################################
    Private Sub Controlla_ValiditaInizioFine(ByRef Log_Errori As String, _
                                                ByRef Validita_Inizio As String, _
                                                ByRef Validita_Fine As String)

        If Me.Txt_ValiditaInizio.Text <> "" Then
            Validita_Inizio = Me.Txt_ValiditaInizio.Text
        Else
            Validita_Inizio = AGRODATAINIZIO
        End If

        If Me.Txt_ValiditaFine.Text <> "" Then
            Validita_Fine = Me.Txt_ValiditaFine.Text
        Else
            Validita_Fine = AGRODATAFINE
        End If

        If Not IsDate(Validita_Inizio) Then
            If Validita_Inizio <> "" Then
                Log_Errori = "E' necessario specificare la Data DA nel formato data corretto!" + vbCrLf
            End If
        End If

        If Not IsDate(Validita_Fine) Then
            If Validita_Fine <> "" Then
                Log_Errori = "E' necessario specificare la Data A nel formato data corretto!" + vbCrLf
            End If
        End If

        If Log_Errori = "" Then
            If CDate(Validita_Fine) < CDate(Validita_Inizio) Then
                Log_Errori = "La Data A deve essere maggiore rispetto alla Data DA!" + vbCrLf
            End If
        End If


    End Sub


    '##################################################################################
    Private Sub Coltrolla_RecuperaFiltri(ByVal Report As enum_CodificaStampe, _
                                            ByRef Log_Errori As String, _
                                            ByRef Validita_Inizio As String, _
                                            ByRef Validita_Fine As String, _
                                            ByRef Codice_Specie As Integer, _
                                            ByRef Str_Codici_Specie As String, _
                                            ByRef Codice_Conferente As Integer, _
                                            ByRef Str_Codici_Conferenti As String, _
                                            ByRef Piva_Produttore As String, _
                                            ByRef Piva_Coop1 As String, _
                                            ByRef Piva_Coop2 As String, _
                                            ByRef Mat_Cod As Integer, _
                                            ByRef Sa_Cod As Integer, _
                                            ByRef Fabbricato_Cod As Integer, _
                                            ByRef Descr_Specie As String, _
                                            ByRef Descr_Prodotto As String, _
                                            ByRef Descr_Magazzino As String, _
                                            ByRef Tracciabilita_Impianti As Integer, _
                                            ByRef Str_Id_Agenda_Bolle As String, _
                                            ByRef Str_Flag_Fascicola As String, _
                                            ByRef Numero_Copie As Integer, _
                                            ByRef Data_CS_Pom As String, _
                                            ByRef Pag_CS_Pom As Integer, _
                                            ByRef Riga_CS_Pom As Integer, _
                                            ByRef Tipo_Pom As Integer, _
                                            ByRef Flag_SalvaPagRiga As Boolean, _
                                            ByRef Data_Giacenza As String, _
                                            ByRef Regolamento_Cod As Integer)


        Dim Messaggio_Specie As String = ""
        Dim Messaggio_Conf As String = ""
        Dim Messaggio_Bolle As String = ""
        Dim Messaggio_Pomodoro As String = ""

        Dim Flag_RangeConferenti As Boolean = False
        Dim Flag_Conferente As Boolean = False
        'il filtro sulle coop va insieme al flag produttore
        Dim Flag_Produttore As Boolean = False
        Dim Flag_RangeSpecie As Boolean = False
        Dim Flag_Specie As Boolean = False
        Dim Flag_Prodotto As Boolean = False
        Dim Flag_Magazzino As Boolean = False
        Dim Flag_TracciaImpianti As Boolean = False
        'Dim Flag_NumeriBolla As Boolean = False
        'Dim Flag_OpzioniStampa As Boolean = False
        Dim Flag_RegCSPom As Boolean = False
        Dim Flag_DataGiacenza As Boolean = False
        Dim Flag_Regolamento As Boolean = False

        'l'intervallo temporale è per tutti i report

        Controlla_ValiditaInizioFine(Log_Errori, _
                                    Validita_Inizio, _
                                    Validita_Fine)

        Select Case Report

            Case enum_CodificaStampe.ADD_Riepilogo_Conf_XSpecie

                Flag_Magazzino = True
                Flag_RangeConferenti = True
                'Flag_Specie = True
                Flag_RangeSpecie = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_EC_Bolle_Accettazione_DaDiversi

                Flag_Magazzino = True
                Flag_RangeConferenti = True
                'Flag_Produttore = True
                Flag_RangeSpecie = True
                Flag_Prodotto = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_EC_Imballi

                Flag_Magazzino = True
                Flag_RangeConferenti = True
                Flag_Prodotto = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_Saldo_Imballi

                Flag_Magazzino = True
                Flag_RangeConferenti = True
                Flag_Prodotto = True
                Flag_DataGiacenza = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_Export_Bolle_Accettazione_DaDiversi

                Flag_Magazzino = True
                Flag_RangeConferenti = True
                Flag_Produttore = True
                Flag_RangeSpecie = True
                Flag_Prodotto = True
                Flag_TracciaImpianti = True

                '------------------------------------------

            Case enum_CodificaStampe.ADD_Export_Traportatori

                Flag_Magazzino = True
                Flag_RangeSpecie = True

                '-----------------------------------

            Case enum_CodificaStampe.Buono_Accettazione_Diversi 'stampa massiva bolle

                'Flag_NumeriBolla = True
                'Flag_OpzioniStampa = True
                Flag_RangeConferenti = True
                Flag_RangeSpecie = True
                Flag_Produttore = True

                '-----------------------------------

            Case enum_CodificaStampe.Certificato_Pomodoro 'stampa massiva certificati

                'Flag_NumeriBolla = True
                'Flag_OpzioniStampa = True
                Flag_RangeConferenti = True
                Flag_RangeSpecie = True
                Flag_Produttore = True

                '-----------------------------------

            Case enum_CodificaStampe.Registro_CaricoScarico_Pomodoro

                Flag_RegCSPom = True
                'Flag_Prodotto = True

                '------------------------------------
            Case enum_CodificaStampe.ADD_ExcelTracciabilitaConferimenti

                Flag_Regolamento = True

                '-----------------------------------

            Case enum_CodificaStampe.ADD_ExportExcel_CertificatiPomodoro

                Flag_Magazzino = True
                Flag_RangeConferenti = True
                Flag_RangeSpecie = True
                Flag_Produttore = True

                '-----------------------------------

        End Select

        '----------------------------
        '------ RANGE CONFERENTI ----
        '----------------------------
        If Flag_RangeConferenti = True Then

            'rifaccio il controllo sui codici,
            'per l'eventuale composizione della stringa di + codici
            '(nel caso l'utente non abbia premuto su carica)
            Verifica_DA_A_Conferenti(Messaggio_Conf)

            If Messaggio_Conf = "" Then

                If Me.Txt_SringaFiltroConferenti.Text = "" Then
                    'ho selezionato un solo codice o li voglio tutti
                    If Me.Txt_Da_CodiceConferente.Text = "" And Me.Txt_A_CodiceConferente.Text = "" Then
                        'voglio tutti i conferenti
                    Else
                        'ho selezionato un solo codice
                        If Me.Txt_Da_CodiceConferente.Text = "" Or Me.Txt_A_CodiceConferente.Text = "" Then
                            Log_Errori = "E' necessario specificare il codice del conferente!" + vbCrLf
                        Else
                            'Ricalcolo in ogni caso la ragione sociale
                            '(potrebbe essere cambiato il codice, non caricata la ragione sociale, e rimasta quella di prima)
                            ''If Me.Txt_Da_Conferente.Text = "" Then
                            ''non è stata valorizzata la ragione sociale:
                            ''- l'utente non ha premuto su carica
                            ''- oppure non esiste un conferente con quel codice
                            Me.Txt_Da_Conferente.Text = ""
                            Dim Cod_Contatto As String
                            Me.Txt_Da_Conferente.Text = Carica_Conferente(Me.Txt_Da_CodiceConferente.Text, Cod_Contatto)
                            Me.Txt_CodContatto_DA_Conferente.Text = Cod_Contatto

                            If Me.Txt_Da_Conferente.Text = "" Then
                                Log_Errori = "Non esiste un conferente con codice " + Me.Txt_Da_CodiceConferente.Text + "!" + vbCrLf
                            Else
                                Codice_Conferente = CInt(Me.Txt_Da_CodiceConferente.Text)
                            End If
                            ''Else
                            ''    Codice_Conferente = CInt(Me.Txt_Da_CodiceConferente.Text)
                            ''End If
                        End If
                    End If
                Else
                    'ho selezionato un range di conferenti
                    Str_Codici_Conferenti = Me.Txt_SringaFiltroConferenti.Text
                End If

            End If


        End If


        '----------------------------
        '------ RANGE SPECIE --------
        '----------------------------
        If Flag_RangeSpecie = True Then

            'rifaccio il controllo sui codici,
            'per l'eventuale composizione della stringa di + codici
            '(nel caso l'utente non abbia premuto su carica)
            Verifica_DA_A_Specie(Messaggio_Specie)

            If Messaggio_Specie = "" Then

                If Me.Txt_SringaFiltroSpecie.Text = "" Then
                    'ho selezionato un solo codice o li voglio tutti
                    If Me.Txt_Da_CodiceSpecie.Text = "" And Me.Txt_A_CodiceSpecie.Text = "" Then
                        'voglio tutte le specie
                    Else
                        'ho selezionato un solo codice
                        If Me.Txt_Da_CodiceSpecie.Text = "" Or Me.Txt_A_CodiceSpecie.Text = "" Then
                            Log_Errori = "E' necessario specificare il codice della specie!" + vbCrLf
                        Else
                            'Ricalcolo in ogni caso la descrizione del prodotto
                            '(potrebbe essere cambiato il codice, non caricata la desc, e rimasta quella di prima)
                            'If Me.Txt_Da_Specie.Text = "" Then
                            'non è stata valorizzata la ragione sociale:
                            ''- l'utente non ha premuto su carica
                            ''- oppure non esiste un conferente con quel codice
                            Me.Txt_Da_Specie.Text = Carica_Specie(Me.Txt_Da_CodiceSpecie.Text)
                            If Me.Txt_Da_Specie.Text = "" Then
                                Log_Errori = "Non esiste una specie con codice " + Me.Txt_Da_CodiceSpecie.Text + "!" + vbCrLf
                            Else
                                Codice_Specie = CInt(Me.Txt_Da_CodiceSpecie.Text)
                            End If
                        End If
                    End If
                Else
                    'ho selezionato un range di conferenti
                    Str_Codici_Specie = Me.Txt_SringaFiltroSpecie.Text
                End If

            End If

        End If



        '----------------------------
        '------- CODICE SPECIE ------
        '----------------------------
        If Flag_Specie = True Then

            If Me.Txt_Da_CodiceSpecie.Text = "" Then
                Log_Errori = "E' necessario specificare il codice della specie!" + vbCrLf
            Else
                'Ricalcolo in ogni caso la descrizione
                '(potrebbe essere cambiato il codice, non caricata la descrizione e rimasta quella di prima)
                ''If Me.Txt_Da_Specie.Text = "" Then
                ''non è stata valorizzata la descrizione del prodotto:
                ''- l'utente non ha premuto su carica
                ''- oppure non esiste un prodotto con quel codice
                Me.Txt_Da_Specie.Text = Carica_Specie(Me.Txt_Da_CodiceSpecie.Text)
                If Me.Txt_Da_Specie.Text = "" Then
                    Log_Errori = "Non esiste una specie con codice " + Me.Txt_Da_CodiceSpecie.Text + "!" + vbCrLf
                Else
                    Codice_Specie = CInt(Me.Txt_Da_CodiceSpecie.Text)
                    Descr_Specie = CStr(Me.Txt_Da_Specie.Text)
                End If
                'Else
                '    Codice_Specie = CInt(Me.Txt_Da_CodiceSpecie.Text)
                '    Descr_Specie = CStr(Me.Txt_Da_Specie.Text)
                'End If
            End If

        End If

        '----------------------------
        '-------- MAGAZZINO ---------
        '----------------------------
        If Flag_Magazzino = True Then

            If Not IsNothing(Me.Cmb_Magazzino.SelectedItem) Then
                Sa_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(1)
                Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(0)
                Descr_Magazzino = CStr(Me.Cmb_Magazzino.SelectedItem.Text.Split("(")(0))
            Else
                Log_Errori = "E' necessario selezionare il magazzino!" + vbCrLf
            End If

        End If

        '----------------------------
        '-------- REGOLAMENTO ---------
        '----------------------------
        If Flag_Regolamento = True Then

            If Not IsNothing(Me.Cmb_Regolamento.SelectedItem) Then
                Regolamento_Cod = CInt(Me.Cmb_Regolamento.SelectedValue)
            Else
                Log_Errori = "E' necessario selezionare il regolamento!" + vbCrLf
            End If

        End If


        '----------------------------
        '-------- PRODUTTORE --------
        '----------------------------
        If Flag_Produttore = True Then

            If Me.Txt_RagSoc_Produttore.Text <> "" Then
                Piva_Produttore = Me.Txt_FiltroPiva_Produttore.Text
            End If

            If Me.Txt_RagSoc_Coop1.Text <> "" Then
                Piva_Coop1 = Me.Txt_FiltroPiva_Coop1.Text
            End If

            If Me.Txt_RagSoc_Coop2.Text <> "" Then
                Piva_Coop2 = Me.Txt_FiltroPiva_Coop2.Text
            End If

        End If


        '----------------------------
        '-------- PRODOTTO ----------
        '----------------------------
        If Flag_Prodotto = True Then

            If Not IsNothing(Me.Cmb_Prodotti.SelectedItem) Then
                If Me.Cmb_Prodotti.SelectedValue <> "" Then
                    Mat_Cod = Me.Cmb_Prodotti.SelectedValue
                    Descr_Prodotto = Me.Cmb_Prodotti.SelectedItem.Text
                End If
            Else
                'Log_Errori = "E' necessario selezionare il magazzino!" + vbCrLf
            End If

        End If

        '------------------------------------------------
        '-------- CHECK TRACCIABILITA IMPIANTI ----------
        '------------------------------------------------
        If Flag_TracciaImpianti = True Then
            If Me.Chk_TracciaImpianti.Checked = True Then
                Tracciabilita_Impianti = 1
            End If
        End If

        ''------------------------------------------------
        ''------------- NUMERI BOLLA ---------------------
        ''------------------------------------------------
        'If Flag_NumeriBolla = True Then
        '    'viene fatto dopo
        '    Verifica_Filtro_Bolle(Server, Session, Page, _
        '                            Messaggio_Bolle, _
        '                            False, _
        '                            Qs_Piva, _
        '                            enum_CodificaStampe.Buono_Accettazione_Diversi, _
        '                            Me.Txt_Prefisso_DaNumeroBolla.Text, Me.Txt_DaNumeroBolla.Text, Me.Txt_Suffisso_DaNumeroBolla.Text, _
        '                            Me.Txt_Prefisso_ANumeroBolla.Text, Me.Txt_ANumeroBolla.Text, Me.Txt_Suffisso_ANumeroBolla.Text, _
        '                            Validita_Inizio, Validita_Fine)

        '    Str_Id_Agenda_Bolle = Me.Txt_SringaFiltroBolle.Text
        'End If

        ''------------------------------------------------
        ''----------- OPZIONI DI STAMPA ------------------
        ''------------------------------------------------
        'If Flag_OpzioniStampa = True Then
        '    'viene fatto dopo

        '    Str_Flag_Fascicola = CStr(Me.Chk_Fascicola.Checked)

        '    If Me.Txt_NumeroCopie.Text <> "" Then
        '        Numero_Copie = CInt(Me.Txt_NumeroCopie.Text)
        '    Else
        '        Numero_Copie = 1
        '    End If
        'End If


        '------------------------------------------------
        '------ OPZIONI REGISTRO CARICO SCARICO POM -----
        '------------------------------------------------
        If Flag_RegCSPom = True Then

            'Non va bene! Se l'utente ha specificato a mano i numeri, vengono sovrascritti con quelli salvati
            'Calcola_Progressivi(Year(Me.Txt_DataRegPom.Text))

            If Not IsDate(Me.Txt_DataRegPom.Text) Then
                Messaggio_Pomodoro += "E' necessario impostare la data di stampa del Registro di Carico e Scarico del Pomodoro !" + vbCrLf
            Else
                Data_CS_Pom = Me.Txt_DataRegPom.Text
            End If

            If Not IsNumeric(Me.Txt_PagRegPom.Text) Then
                Messaggio_Pomodoro += "E' necessario impostare il numero di pagina del Registro di Carico e Scarico del Pomodoro !" + vbCrLf
            Else
                Pag_CS_Pom = Me.Txt_PagRegPom.Text
            End If

            If Not IsNumeric(Me.Txt_RigaRegPom.Text) Then
                Messaggio_Pomodoro += "E' necessario impostare il numero di riga del Registro di Carico e Scarico del Pomodoro !" + vbCrLf
            Else
                Riga_CS_Pom = Me.Txt_RigaRegPom.Text
            End If

            Tipo_Pom = Me.Rbl_PomodoroContrattato.SelectedValue

            Flag_SalvaPagRiga = Me.Chk_SalvaNumPagRigaPom.Checked

            'If Mat_Cod = 0 Then
            '    Messaggio_Pomodoro += "E' necessario selezionare la tipologia di Pomodoro!" + vbCrLf
            'End If

        End If

        If Flag_DataGiacenza = True Then
            If Me.Txt_DataGiacenza.Text = "" Then
                Data_Giacenza = Date.Today.ToShortDateString
            Else
                If IsDate(Me.Txt_DataGiacenza.Text) Then
                    Data_Giacenza = Me.Txt_DataGiacenza.Text
                Else
                    Log_Errori += "La data alla quale stampare la giacenza non è valida!" + vbCrLf
                End If
            End If
        End If

        If Messaggio_Specie <> "" Or Messaggio_Conf <> "" Or Messaggio_Bolle <> "" Or Messaggio_Pomodoro <> "" Then
            Log_Errori += Messaggio_Specie + vbCrLf
            Log_Errori += Messaggio_Conf + vbCrLf
            Log_Errori += Messaggio_Bolle + vbCrLf
            Log_Errori += Messaggio_Pomodoro + vbCrLf
        End If



    End Sub



    '##################################################################################
    'Private Sub Verifica_Filtro_Bolle(ByRef Messaggio As String, _
    '                                    ByVal Validita_Inizio As String, _
    '                                    ByVal Validita_Fine As String)


    '    Me.Txt_SringaFiltroBolle.Text = ""

    '    Try

    '        If Me.Txt_DaNumeroBolla.Text <> "" And Me.Txt_ANumeroBolla.Text <> "" Then

    '            If Not IsNumeric(Me.Txt_DaNumeroBolla.Text) Or Not IsNumeric(Me.Txt_ANumeroBolla.Text) Then
    '                Messaggio = "I numeri bolla devono essere numerici."
    '                Exit Sub
    '            End If

    '            ''se sono uguali, si filtra solo una specie
    '            'If Me.Txt_DaNumeroBolla.Text <> Me.Txt_ANumeroBolla.Text Then

    '            If CInt(Me.Txt_ANumeroBolla.Text) < CInt(Me.Txt_DaNumeroBolla.Text) Then
    '                Messaggio = "Il Numero Bolla A deve essere maggiore rispetto al Numero Bolla DA"
    '                Exit Sub
    '            End If

    '            Prepara_StrFiltroBolle(True, Messaggio, Validita_Inizio, Validita_Fine)


    '            'Else
    '            '    'se sono uguali, si filtra solo una specie
    '            '    Me.Txt_SringaFiltroBolle.Text = Str_NumBolla
    '            'End If

    '        Else
    '            If Me.Txt_ValiditaInizio.Text <> "" And Me.Txt_ValiditaFine.Text <> "" Then
    '                'ok, non si filtra per numero, ma solo per date
    '                Prepara_StrFiltroBolle(False, Messaggio, Validita_Inizio, Validita_Fine)
    '            Else
    '                'è necessario impostare almeno un tipo di filtro
    '                Messaggio = "E' necessario impostare almeno un criterio di filtro di stampa delle bolle (per intervallo temporale o per numero bolla)."
    '            End If
    '        End If

    '    Catch ex As Exception
    '        Messaggio += "Si è verificato il seguente errore: " + ex.Message
    '    End Try


    'End Sub

    ''##################################################################################
    'Private Sub Prepara_StrFiltroBolle(ByVal Flag_FiltraNumBolla As Boolean, _
    '                                    ByRef Messaggio As String, _
    '                                    ByVal Validita_Inizio As String, _
    '                                    ByVal Validita_Fine As String)


    '    'compongo la stringa dei progressivi da filtrare
    '    Dim DT_NumBolla As DataTable
    '    Dim i As Integer
    '    Dim Str_NumBolla As String = ""

    '    DT_NumBolla = Leggi_Range_IdAgenda_Bolle(Flag_FiltraNumBolla, _
    '                                                Me.Txt_Prefisso_DaNumeroBolla.Text, _
    '                                                Me.Txt_DaNumeroBolla.Text, _
    '                                                Me.Txt_Suffisso_DaNumeroBolla.Text, _
    '                                                Me.Txt_Prefisso_ANumeroBolla.Text, _
    '                                                Me.Txt_ANumeroBolla.Text, _
    '                                                Me.Txt_Suffisso_ANumeroBolla.Text, _
    '                                                Validita_Inizio, _
    '                                                Validita_Fine)


    '    If Not IsNothing(DT_NumBolla) Then

    '        If DT_NumBolla.Rows.Count <> 0 Then

    '            For i = 0 To DT_NumBolla.Rows.Count - 1

    '                If i <> DT_NumBolla.Rows.Count - 1 Then
    '                    Str_NumBolla += CStr(DT_NumBolla.Rows(i).Item("Id_Agenda")) + "|"
    '                Else
    '                    Str_NumBolla += CStr(DT_NumBolla.Rows(i).Item("Id_Agenda"))
    '                End If

    '            Next

    '            Me.Txt_SringaFiltroBolle.Text = Str_NumBolla

    '        Else
    '            Messaggio = "Non è stata trovata alcuna bolla che soddisfi il criterio di filtro impostato."
    '        End If

    '    End If

    'End Sub


    ''##################################################################################
    'Private Function Leggi_Range_IdAgenda_Bolle(ByVal Flag_FiltraNumBolla As Boolean, _
    '                                            ByVal Prefisso_Num_Bolla_DA As String, _
    '                                            ByVal Num_Bolla_DA As String, _
    '                                            ByVal Suffisso_Num_Bolla_DA As String, _
    '                                            ByVal Prefisso_Num_Bolla_A As String, _
    '                                            ByVal Num_Bolla_A As String, _
    '                                            ByVal Suffisso_Num_Bolla_A As String, _
    '                                            ByVal Validita_Inizio As String, _
    '                                            ByVal Validita_Fine As String) As DataTable


    '    Dim DT_NumBolla As DataTable
    '    Dim FiltroAggiuntivo As String = ""
    '    Dim Ordinamento As String


    '    If Flag_FiltraNumBolla = True Then

    '        Dim j As Integer
    '        Dim ElencoNumeriBolla As String = ""
    '        Dim ChiaveBolla As String = ""

    '        For j = CInt(Num_Bolla_DA) To CInt(Num_Bolla_A)

    '            ChiaveBolla = Prefisso_Num_Bolla_DA & "_" & CStr(j) & "_" & Suffisso_Num_Bolla_DA

    '            ElencoNumeriBolla += ",'" & ChiaveBolla & "'"

    '        Next

    '        'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
    '        ElencoNumeriBolla = Mid(ElencoNumeriBolla, 2)

    '        FiltroAggiuntivo += " AND               ( ( Mov_Accett.Doc_Numero_Sin  "
    '        FiltroAggiuntivo += "                     + '_' + CONVERT(varchar(10), Mov_Accett.Doc_Numero)  "
    '        FiltroAggiuntivo += "                     + '_' + Mov_Accett.Doc_Numero_Des )  "
    '        FiltroAggiuntivo += "                     IN (" & ElencoNumeriBolla & ")    )  "

    '    End If

    '    Ordinamento = " ORDER BY Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des "


    '    DT_NumBolla = NewCom_Accettazione_DaDiversi_IdAgenda_Leggi(Server, Session, Page, _
    '                                                                Qs_Piva, _
    '                                                                Prefisso_Num_Bolla_DA, _
    '                                                                FiltroAggiuntivo, _
    '                                                                Ordinamento, _
    '                                                                Validita_Inizio, _
    '                                                                Validita_Fine)


    '    Return DT_NumBolla


    'End Function


    '##################################################################################
    Private Sub Btn_Carica_Produttore_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Produttore.Click

        Try

            'If Me.Txt_Da_CodiceConferente.Text = Me.Txt_A_CodiceConferente.Text Then

            'ho selezionato un solo conferente

            'Me.Txt_RagSoc_Produttore.Text = Carica_Produttore(Me.Txt_FiltroPiva_Produttore.Text, Me.Txt_FiltroRagSoc_Produttore.Text)

            If Me.Txt_FiltroPiva_Produttore.Text <> "" Or Me.Txt_FiltroRagSoc_Produttore.Text <> "" Then

                'carico tutto, non solo le foglie
                'perchè propar è produttore ma non foglia
                Me.Txt_RagSoc_Produttore.Text = Carica_ImpresaInGerarchia(Me.Txt_FiltroPiva_Produttore.Text, _
                                                                            Me.Txt_FiltroRagSoc_Produttore.Text, _
                                                                            0, _
                                                                             " 1=1 ")

                If Me.Txt_RagSoc_Produttore.Text = "" Then
                    Me.Txt_FiltroPiva_Produttore.Text = ""
                    Me.Txt_FiltroRagSoc_Produttore.Text = ""
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stato trovato alcun produttore con il criterio di filtro impostato.", Page)
                End If
            Else
                Me.Txt_RagSoc_Produttore.Text = ""
            End If

            'Else
            '    AgroMsgBox("Per caricare il produttore è necessario selezionare un solo conferente.", Page)
            'End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento dei produttori. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try

    End Sub

    '##################################################################################
    Private Sub Btn_Carica_Coop1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Coop1.Click

        Try

            If Me.Txt_FiltroPiva_Coop1.Text <> "" Or Me.Txt_FiltroRagSoc_Coop1.Text <> "" Then

                Dim Filtro As String = " AND GerarchiaImprese.Livello = 3 "

                Me.Txt_RagSoc_Coop1.Text = Carica_ImpresaInGerarchia(Me.Txt_FiltroPiva_Coop1.Text, _
                                                                    Me.Txt_FiltroRagSoc_Coop1.Text, _
                                                                    0, _
                                                                    Filtro)

                If Me.Txt_RagSoc_Coop1.Text = "" Then
                    Me.Txt_FiltroPiva_Coop1.Text = ""
                    Me.Txt_FiltroRagSoc_Coop1.Text = ""
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stata trovata alcuna Cooperativa con il criterio di filtro impostato.", Page)
                End If
            Else
                Me.Txt_RagSoc_Coop1.Text = ""
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento delle cooperative. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try

    End Sub

    '##################################################################################
    Private Sub Btn_Carica_Coop2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Coop2.Click

        Try

            If Me.Txt_FiltroPiva_Coop2.Text <> "" Or Me.Txt_FiltroRagSoc_Coop2.Text <> "" Then

                Dim Filtro As String = " AND GerarchiaImprese.Livello = 4 "

                Me.Txt_RagSoc_Coop2.Text = Carica_ImpresaInGerarchia(Me.Txt_FiltroPiva_Coop2.Text, _
                                                                    Me.Txt_FiltroRagSoc_Coop2.Text, _
                                                                    0, _
                                                                    Filtro)

                If Me.Txt_RagSoc_Coop2.Text = "" Then
                    Me.Txt_FiltroPiva_Coop2.Text = ""
                    Me.Txt_FiltroRagSoc_Coop2.Text = ""
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stata trovata alcuna seconda Cooperativa con il criterio di filtro impostato.", Page)
                End If
            Else
                Me.Txt_RagSoc_Coop2.Text = ""
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento delle cooperative. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try

    End Sub


    ''##################################################################################
    'Private Function Carica_Produttore(ByRef Piva As String, _
    '                                    ByVal Rag_Soc As String) As String

    '    Try

    '        Dim DT_Prod As DataTable
    '        Dim FiltroAggiuntivo As String = ""
    '        'Dim Padre As String

    '        'Padre = Me.Txt_CodContatto_DA_Conferente.Text

    '        'FiltroAggiuntivo = " AND     (Imprese.TipoImpresaGerarchia = 1 ) "

    '        If Rag_Soc <> "" Then
    '            FiltroAggiuntivo += " AND     (Imprese.Rag_Soc LIKE '%" & SQL_SaveText(Rag_Soc) & "%')   "
    '        End If

    '        DT_Prod = NewCom_GerarchiaImprese_Leggi(Server, Session, Page, _
    '                                                , _
    '                                                Piva, _
    '                                                1, _
    '                                                , _
    '                                                , , _
    '                                                FiltroAggiuntivo, _
    '                                                )

    '        If Not IsNothing(DT_Prod) AndAlso DT_Prod.Rows.Count <> 0 Then
    '            Piva = DT_Prod.Rows(0).Item("Figlio")
    '            Return DT_Prod.Rows(0).Item("RagSoc_Figlio")
    '        Else
    '            Return ""
    '        End If

    '    Catch ex As Exception
    '        AgroMsgBox("Caricamento dei produttori. Si è verificato il seguente errore: " + ex.Message, Page)
    '    End Try


    'End Function

    '##################################################################################
    Private Function Carica_ImpresaInGerarchia(ByRef Piva As String, _
                                                ByVal Rag_Soc As String, _
                                                ByVal Foglia As Integer, _
                                                ByVal FiltroAggiuntivo As String) As String


        Dim DT As DataTable
        Dim objGerarchiaImprese As AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        '" 1=1 " passato nella chiamata come parametro
        If Rag_Soc <> "" Then
            FiltroAggiuntivo += " AND     (Imprese.Rag_Soc LIKE '%" & Agro_SQL_SaveText(Rag_Soc) & "%')   "
        End If



        DT = objGerarchiaImprese.GerarchiaImprese_Leggi("",
                                                        Piva, _
                                                        Foglia, _
                                                        0, _
                                                        CDate("01/01/1900"), CDate("31/12/2100"), _
                                                        FiltroAggiuntivo, _
                                                        "", _
                                                        objParametri_Server)

        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then
            Piva = DT.Rows(0).Item("Figlio")
            Return DT.Rows(0).Item("RagSoc_Figlio")
        Else
            Return ""
        End If

    End Function



    '##################################################################################
    Private Sub Btn_Carica_Prodotti_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Prodotti.Click

        Me.Cmb_Prodotti.Items.Clear()

        Dim Elem_Cod As Integer = 0


        Select Case Me.Rbl_Report.SelectedValue

            Case enum_CodificaStampe.ADD_Riepilogo_Conf_XSpecie


                '------------------------------------
            Case enum_CodificaStampe.ADD_EC_Bolle_Accettazione_DaDiversi

                Elem_Cod = TRASFORMATI_VEGETALI

                '------------------------------------
            Case enum_CodificaStampe.ADD_EC_Imballi

                Elem_Cod = BENI_CONFEZ_VEGETALE

                '------------------------------------
            Case enum_CodificaStampe.ADD_Saldo_Imballi

                Elem_Cod = BENI_CONFEZ_VEGETALE

                '------------------------------------
            Case enum_CodificaStampe.ADD_Export_Bolle_Accettazione_DaDiversi

                Elem_Cod = TRASFORMATI_VEGETALI

                '------------------------------------
            Case enum_CodificaStampe.ADD_Export_Traportatori


                '------------------------------------
            Case enum_CodificaStampe.Registro_CaricoScarico_Pomodoro

                Elem_Cod = TRASFORMATI_VEGETALI


                '------------------------------------


        End Select

        Carica_Prodotti(Elem_Cod)


    End Sub


    '##################################################################################
    Private Sub Carica_Prodotti(ByVal Elem_Cod As Integer)


        Select Case Elem_Cod

            Case TRASFORMATI_VEGETALI

                Select Case Me.Rbl_Report.SelectedValue

                    Case enum_CodificaStampe.Registro_CaricoScarico_Pomodoro

                        Carica_SpecieVarieta("", "", 52)

                        'Select Case CInt(Session("ASG_ProgressivoGIAS"))
                        '    Case enum_CodiceGIAS_Clienti.Fruttagel
                        '    Case Else
                        'End Select


                        '-----------------------------------------
                    Case Else

                        If (Me.Txt_Da_CodiceSpecie.Text = Me.Txt_A_CodiceSpecie.Text) And _
                              Me.Txt_Da_CodiceSpecie.Text <> "" And Me.Txt_A_CodiceSpecie.Text <> "" Then

                            'è selezionata una sola specie
                            Carica_SpecieVarieta(Me.Txt_Da_CodiceSpecie.Text, Me.Txt_Filtro_MatDes.Text)

                        Else

                            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Per caricare i prodotti è necessario selezionare una sola specie.", Page)

                        End If

                End Select

                '==============================================

            Case BENI_CONFEZ_VEGETALE

                Carica_Imballi(Me.Cmb_Magazzino.SelectedValue, Me.Cmb_Magazzino.SelectedItem.Text, Me.Txt_Filtro_MatDes.Text)

        End Select


    End Sub


    '##################################################################################
    Private Sub Carica_Imballi(ByVal Chiave_Magazzino As String, _
                                ByVal Descr_Magazzino As String, _
                                ByVal Filtro_Cod_Descr_Imballo As String)

        Try

            Dim Sa_Cod As Integer
            Dim Fabbricato_Cod As Integer
            Dim DT_Imballi As DataTable
            Dim i As Integer
            Dim x_Cod, x_Des As String
            Dim FiltroAggiuntivo As String = ""
            Dim obj_MateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R


            Sa_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(1)
            Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(0)

            FiltroAggiuntivo = " 1=1 "
            If InStr(Descr_Magazzino.ToLower, "larino") Then
                FiltroAggiuntivo += " AND     (Materie_Prime.Cod_Articolo LIKE '" & Agro_SQL_SaveText("7") & "%')   "
            Else
                FiltroAggiuntivo += " AND     (Materie_Prime.Cod_Articolo LIKE '" & Agro_SQL_SaveText("9") & "%')   "
            End If

            If Filtro_Cod_Descr_Imballo <> "" Then
                FiltroAggiuntivo += " AND (    (Materie_Prime.Mat_Des LIKE '%" & Agro_SQL_SaveText(Filtro_Cod_Descr_Imballo) & "%')   "
                FiltroAggiuntivo += "       OR     (Materie_Prime.Cod_Articolo LIKE '%" & Agro_SQL_SaveText(Filtro_Cod_Descr_Imballo) & "%')  ) "
            End If



            'DT_Imballi = NewCom_MateriePrime_Leggi(Server, Session, Page, _
            '                                        , , _
            '                                        BENI_CONFEZ_VEGETALE, _
            '                                        , _
            '                                        , _
            '                                        , , , , , , , , , , , , , _
            '                                        FiltroAggiuntivo, )

            DT_Imballi = obj_MateriePrime.Leggi("", 0, _
                                              BENI_CONFEZ_VEGETALE, _
                                              0, _
                                              "", _
                                              0, 0, 0, 0, 0, 0, 0, "", 0, "", False, False, "", _
                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                              FiltroAggiuntivo, "", objParametri_Server)


            If Not IsNothing(DT_Imballi) AndAlso DT_Imballi.Rows.Count <> 0 Then

                Me.Cmb_Prodotti.Items.Add(New ListItem("", ""))

                For i = 0 To DT_Imballi.Rows.Count - 1

                    x_Cod = DT_Imballi.Rows(i).Item("Mat_Cod")

                    x_Des = CStr(DT_Imballi.Rows(i).Item("Mat_Des")) + " (Cod. " + CStr(DT_Imballi.Rows(i).Item("Cod_Articolo")) + ")"

                    Me.Cmb_Prodotti.Items.Add(New ListItem(x_Des, x_Cod))

                Next

                Me.Cmb_Prodotti.SelectedIndex = 1 'Me.Cmb_Prodotti.Items.IndexOf(Me.Cmb_Prodotti.Items.FindByValue(0))

            Else
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stato trovato alcun imballo.", Page)
            End If


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento degli imballi. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try




    End Sub


    '##################################################################################
    Private Sub Carica_SpecieVarieta(ByVal Cod_Articolo As String, _
                                        ByVal Mat_Des As String, _
                                        Optional ByVal Veg_Cod As Integer = 0)

        Try

            Dim DT_Prodotti As DataTable
            Dim i As Integer
            Dim x_Cod, x_Des As String
            Dim FiltroAggiuntivo As String = ""
            Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

            FiltroAggiuntivo = " 1=1 "
            If Mat_Des <> "" Then
                FiltroAggiuntivo += " AND     (Materie_Prime.Mat_Des LIKE '%" & Agro_SQL_SaveText(Mat_Des) & "%')   "
            End If


            'DT_Prodotti = NewCom_MateriePrime_Leggi(Server, Session, Page, _
            '                                        , , _
            '                                        TRASFORMATI_VEGETALI, _
            '                                        , _
            '                                        CStr(Cod_Articolo), _
            '                                         Veg_Cod, , , , , , , , , , , , , _
            '                                        FiltroAggiuntivo, )

            DT_Prodotti = objMateriePrime.Leggi("", 0, _
                                             TRASFORMATI_VEGETALI, _
                                             0, _
                                             CStr(Cod_Articolo), _
                                             Veg_Cod, _
                                             0, 0, 0, 0, 0, 0, "", 0, "", False, False, "", _
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                             FiltroAggiuntivo, "", objParametri_Server)


            If Not IsNothing(DT_Prodotti) AndAlso DT_Prodotti.Rows.Count <> 0 Then

                Me.Cmb_Prodotti.Items.Add(New ListItem("", ""))

                For i = 0 To DT_Prodotti.Rows.Count - 1

                    x_Cod = DT_Prodotti.Rows(i).Item("Mat_Cod")

                    x_Des = CStr(DT_Prodotti.Rows(i).Item("Mat_Des"))

                    Me.Cmb_Prodotti.Items.Add(New ListItem(x_Des, x_Cod))

                Next

                Me.Cmb_Prodotti.SelectedIndex = 1 'Me.Cmb_Prodotti.Items.IndexOf(Me.Cmb_Prodotti.Items.FindByValue(0))


            Else
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stato trovato alcun prodotto per la specie selezionata e che soddisfi il criterio impostato.", Page)
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Caricamento dei prodotti. Si è verificato il seguente errore: " + ex.Message, Page)
        End Try


    End Sub


    '##################################################################################
    Private Sub ImgBtn_StampaMassivaBolle_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaMassivaBolle.Click

        StampaMassiva_BolleCertificati()

    End Sub


    '##################################################################################
    Private Sub StampaMassiva_BolleCertificati()

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim Str_Flag_Fascicola As String
        Dim Numero_Copie As Integer
        Dim Nome_Stampante, Querystring As String
        Dim Validita_Inizio As String
        Dim Validita_Fine As String
        Dim Messaggio As String = ""

        Controlla_ValiditaInizioFine(Messaggio, Validita_Inizio, Validita_Fine)

        If Messaggio <> "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
            Exit Sub
        End If

        '------------------------------------------------
        '----------- OPZIONI DI STAMPA ------------------
        '------------------------------------------------
        Str_Flag_Fascicola = CStr(Me.Chk_Fascicola.Checked)

        If Me.Txt_NumeroCopie.Text <> "" Then
            Numero_Copie = CInt(Me.Txt_NumeroCopie.Text)
        Else
            Numero_Copie = 1
        End If

        Select Case Me.Cmb_Stampante.SelectedValue
            Case "-1"
                Nome_Stampante = Qs_PrintName
            Case Else
                Nome_Stampante = Me.Cmb_Stampante.SelectedItem.Text
        End Select

        If Nome_Stampante = "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è possibile procedere con la stampa massiva delle bolle poichè il nome della stampante non è pervenuto!", Page)
            Exit Sub
        End If

        Dim Codice_Specie As Integer = 0
        Dim Str_Codici_Specie As String = ""
        Dim Codice_Conferente As Integer = 0
        Dim Str_Codici_Conferenti As String = ""
        Dim Piva_Produttore As String = ""
        Dim Piva_Coop1 As String = ""
        Dim Piva_Coop2 As String = ""

        Coltrolla_RecuperaFiltri(Me.Rbl_Report.SelectedValue, _
                                   Messaggio, _
                                   Nothing, _
                                   Nothing, _
                                   Codice_Specie, _
                                   Str_Codici_Specie, _
                                   Codice_Conferente, _
                                   Str_Codici_Conferenti, _
                                   Piva_Produttore, _
                                   Piva_Coop1, _
                                   Piva_Coop2, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing, _
                                   Nothing)

        If Messaggio <> "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
            Exit Sub
        End If


        Select Case CInt(Me.Txt_TipoStampa.Text)

            Case enum_CodificaStampe.Buono_Accettazione_Diversi

                Dim Str_Id_Agenda_Bolle As String
                Dim Prefisso_Num_Bolla_DA, Prefisso_Num_Bolla_A As String
                Prefisso_Num_Bolla_DA = Me.Cmb_Prefisso_DaNumeroBolla.SelectedValue
                'Prefisso_Num_Bolla_dA = Me.Txt_Prefisso_dANumeroBolla.Text
                Prefisso_Num_Bolla_A = Me.Cmb_Prefisso_ANumeroBolla.SelectedValue
                'Prefisso_Num_Bolla_A = Me.Txt_Prefisso_ANumeroBolla.Text

                '------------------------------------------------
                '------------- NUMERI BOLLA ---------------------
                '------------------------------------------------
                Str_Id_Agenda_Bolle = objADDFun.Verifica_Filtro_Bolle(objParametri_Server, _
                                                            Session("ReportSelezionato"), _
                                                            Messaggio, _
                                                            False, _
                                                            Qs_Piva, _
                                                            enum_CodificaStampe.Buono_Accettazione_Diversi, _
                                                            Prefisso_Num_Bolla_DA, Me.Txt_DaNumeroBolla.Text, Me.Txt_Suffisso_DaNumeroBolla.Text, _
                                                            Prefisso_Num_Bolla_A, Me.Txt_ANumeroBolla.Text, Me.Txt_Suffisso_ANumeroBolla.Text, _
                                                            Validita_Inizio, Validita_Fine, _
                                                            Codice_Specie, _
                                                            Str_Codici_Specie, _
                                                            Codice_Conferente, _
                                                            Str_Codici_Conferenti, _
                                                            Piva_Produttore, _
                                                            Piva_Coop1, _
                                                            Piva_Coop2)

                If Messaggio <> "" Then
                    AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If

                Session("Str_Id_Agenda_Bolle") = Str_Id_Agenda_Bolle


                Querystring = "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                            "&i=" + Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) + _
                            "&l=" + Stringa_Codifica(CStr(LAVCOD_ACCETTAZIONE_DIVERSI), AgroKey_EncoderDecoder, Server) + _
                            "&ptp=" + Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) + _
                            "&pn=" + Stringa_Codifica(CStr(Nome_Stampante), AgroKey_EncoderDecoder, Server) + _
                            "&ff=" + Stringa_Codifica(Str_Flag_Fascicola, AgroKey_EncoderDecoder, Server) + _
                            "&nc=" + Stringa_Codifica(CStr(Numero_Copie), AgroKey_EncoderDecoder, Server)

                Page_NewWindow_2010(Page, _
                                "../Contabilita/Fattura/BollaAccettazione.aspx", _
                                Querystring, _
                                "BollaAccettazione", _
                                , , , , , , , )

                '=================================================================

            Case enum_CodificaStampe.Certificato_Pomodoro

                Dim Str_Id_Agenda_Cert As String
                Dim Prefisso_DA_Bolla As String
                Dim Prefisso_A_Bolla As String

                '------------------------------------------------
                '------------- NUMERI CERTIFICATI ---------------
                '------------------------------------------------

                'Str_Id_Agenda_Cert = Verifica_Filtro_Bolle( _
                '                        Server, Session, Page, _
                '                        Messaggio, _
                '                        False, _
                '                        Qs_Piva, _
                '                        enum_CodificaStampe.Certificato_Pomodoro, _
                '                        Me.Txt_Prefisso_DaNumeroCert.Text, Me.Txt_DaNumeroCert.Text, Me.Txt_Suffisso_DaNumeroCert.Text, _
                '                        Me.Txt_Prefisso_ANumeroCert.Text, Me.Txt_ANumeroCert.Text, Me.Txt_Suffisso_ANumeroCert.Text, _
                '                        Validita_Inizio, Validita_Fine)

                'Me.Txt_Prefisso_DaNumeroBolla.Text
                Prefisso_DA_Bolla = Me.Cmb_Prefisso_DaNumeroBolla.SelectedValue
                'Me.Txt_Prefisso_ANumeroBolla.Text
                Prefisso_A_Bolla = Me.Cmb_Prefisso_ANumeroBolla.SelectedValue

                'modifica in data 15/07/2010: leggo gli id_agenda dal numero bolla
                'non dal numero certificato, perchè non sono ancora valorizzati
                Str_Id_Agenda_Cert = objADDFun.Verifica_Filtro_Bolle(objParametri_Server, _
                                                            Session("ReportSelezionato"), _
                                                            Messaggio, _
                                                            False, _
                                                            Qs_Piva, _
                                                            enum_CodificaStampe.Certificato_Pomodoro, _
                                                            Prefisso_DA_Bolla, Me.Txt_DaNumeroBolla.Text, Me.Txt_Suffisso_DaNumeroBolla.Text, _
                                                            Prefisso_A_Bolla, Me.Txt_ANumeroBolla.Text, Me.Txt_Suffisso_ANumeroBolla.Text, _
                                                            Validita_Inizio, Validita_Fine, _
                                                            Codice_Specie, _
                                                            Str_Codici_Specie, _
                                                            Codice_Conferente, _
                                                            Str_Codici_Conferenti, _
                                                            Piva_Produttore, _
                                                            Piva_Coop1, _
                                                            Piva_Coop2)

                If Messaggio <> "" Then
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If

                Session("Str_Id_Agenda_Certificati") = Str_Id_Agenda_Cert


                Querystring = "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                            "&i=" + Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) + _
                            "&l=" + Stringa_Codifica(CStr(LAVCOD_ACCETTAZIONE_DIVERSI), AgroKey_EncoderDecoder, Server) + _
                            "&ptp=" + Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) + _
                            "&pn=" + Stringa_Codifica(CStr(Nome_Stampante), AgroKey_EncoderDecoder, Server) + _
                            "&ff=" + Stringa_Codifica(Str_Flag_Fascicola, AgroKey_EncoderDecoder, Server) + _
                            "&nc=" + Stringa_Codifica(CStr(Numero_Copie), AgroKey_EncoderDecoder, Server)

                Try

                    Dim Pagina_CertificatoPomodoro As String = objADDFun.PaginaCertificatoPomodoro_from_Str_Id_Agenda(objParametri_Server, _
                                                                                                            Qs_Piva, _
                                                                                                            Str_Id_Agenda_Cert)
                    Page_NewWindow_2010(Page, _
                                    Pagina_CertificatoPomodoro, _
                                    Querystring, _
                                    "CertificatiPomodoro", _
                                    , , , , , , , )

                Catch ex As Exception
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(ex.Message, Page)
                End Try


        End Select

    End Sub


    '##################################################################################
    Private Sub ImgBtn_AnnullaStampaMassiva_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_AnnullaStampaMassiva.Click

        StampaMassiva_Annulla()

    End Sub

    '##################################################################################
    Private Sub StampaMassiva_Annulla()

        Me.Pannello_Generale.Visible = True
        Me.Pannello_ConfermaStampaMassiva.Visible = False

    End Sub

    Private Enum enum_Pannelli

        Pannello_Generale = 0
        Pannello_Pomodoro = 1

    End Enum

    '##################################################################################
    Private Sub Imposta_Pannelli(ByVal Pannello As enum_Pannelli)

        Me.Pannello_Generale.Visible = False
        Me.Pannello_CertificatiPomodoro.Visible = False

        Select Case Pannello

            Case enum_Pannelli.Pannello_Generale
                Me.Pannello_Generale.Visible = True

                '----- Imposto le dimensioni
                With Me.Pannello_Regolamento
                    .Style.Item("Top") = "352px"
                    .Style.Item("Left") = "8px"
                End With


            Case enum_Pannelli.Pannello_Pomodoro
                Me.Pannello_CertificatiPomodoro.Visible = True
                Me.Rbl_Certificati.Visible = True

                '----- Imposto le dimensioni
                With Me.Pannello_CertificatiPomodoro
                    '.Height = Dimensione.Pixel(490)
                    '.Width = Dimensione.Pixel(480)
                    .Style.Item("Top") = "56px"
                    .Style.Item("Left") = "8px"
                End With

        End Select

    End Sub


    '##################################################################################
    Private Sub Rbl_StampaPomodoro_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_StampaPomodoro.SelectedIndexChanged

        Me.Rbl_Certificati.Visible = False

        Select Case Me.Rbl_StampaPomodoro.SelectedValue

            Case 1 'BOLLA
                Me.Rbl_Certificati.SelectedValue = 1

            Case 2 'CERTIFICATO
                Me.Rbl_Certificati.Visible = True

        End Select

    End Sub


    '##################################################################################
    Private Sub Rbl_Certificati_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Certificati.SelectedIndexChanged

        Select Case Me.Rbl_Certificati.SelectedValue

            Case 1 'interno

            Case 2 'esterno

                Chiama_GestioneNumerazioneCertificati(False)

        End Select

    End Sub

    '##################################################################################
    Private Sub Rbl_CertificatiStampaMassiva_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Rbl_CertificatiStampaMassiva.SelectedIndexChanged

        Select Case Me.Rbl_CertificatiStampaMassiva.SelectedValue

            Case 1 'interno

            Case 2 'esterno
                Chiama_GestioneNumerazioneCertificati(True)

        End Select

    End Sub

    '##################################################################################
    Private Sub Chiama_GestioneNumerazioneCertificati(ByVal Flag_StampaMassiva As Boolean)

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim Prefisso_Num_Bolla_Da As String = ""
        Dim Num_Bolla_Da As String = ""
        Dim Suffisso_Num_Bolla_Da As String = ""
        Dim Prefisso_Num_Bolla_A As String = ""
        Dim Num_Bolla_A As String = ""
        Dim Suffisso_Num_Bolla_A As String = ""
        Dim Messaggio As String = ""
        Dim Id_Agenda As Integer


        If Flag_StampaMassiva = True Then
            'stampa massiva

            Prefisso_Num_Bolla_A = Me.Cmb_Prefisso_ANumeroBolla.SelectedValue
            'Prefisso_Num_Bolla_A = Me.Txt_Prefisso_ANumeroBolla.Text
            Num_Bolla_A = Me.Txt_ANumeroBolla.Text
            Suffisso_Num_Bolla_A = Me.Txt_Suffisso_ANumeroBolla.Text
            Id_Agenda = 0

            If Num_Bolla_A = "" Or Num_Bolla_A = "0" Then

                Dim Validita_Inizio As String
                Dim Validita_Fine As String
                Dim Codice_Specie As Integer = 0
                Dim Str_Codici_Specie As String = ""
                Dim Codice_Conferente As Integer = 0
                Dim Str_Codici_Conferenti As String = ""
                Dim Piva_Produttore As String = ""
                Dim Piva_Coop1 As String = ""
                Dim Piva_Coop2 As String = ""

                Coltrolla_RecuperaFiltri(Me.Rbl_Report.SelectedValue, _
                                           Messaggio, _
                                           Validita_Inizio, _
                                           Validita_Fine, _
                                           Codice_Specie, _
                                           Str_Codici_Specie, _
                                           Codice_Conferente, _
                                           Str_Codici_Conferenti, _
                                           Piva_Produttore, _
                                           Piva_Coop1, _
                                            Piva_Coop2, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing, _
                                           Nothing)

                If Messaggio <> "" Then
                    Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
                    Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If

                Dim Doc_Numero_Max As Double
                Dim Doc_Numero_Min As Double

                'Leggi_MinMaxNumeroBolla_byFiltro(Server, Session, Page, _
                '                                Doc_Numero_Max, _
                '                                Doc_Numero_Min, _
                '                                Qs_Piva, _
                '                                Prefisso_Num_Bolla_A, _
                '                                Validita_Inizio, _
                '                                Validita_Fine, _
                '                                Codice_Specie, _
                '                                Str_Codici_Specie, _
                '                                Codice_Conferente, _
                '                                Str_Codici_Conferenti, _
                '                                Piva_Produttore)

                objADDFun.Leggi_MinMaxNumeroBolla_byFiltro(objParametri_Server, _
                                                    Doc_Numero_Max, _
                                                    Doc_Numero_Min, _
                                                    Qs_Piva, _
                                                    Prefisso_Num_Bolla_A, _
                                                    Validita_Inizio, _
                                                    Validita_Fine, _
                                                    Codice_Specie, _
                                                    Str_Codici_Specie, _
                                                    Codice_Conferente, _
                                                    Str_Codici_Conferenti, _
                                                    Piva_Produttore)

                Num_Bolla_A = CStr(Doc_Numero_Max)

                Me.Txt_ANumeroBolla.Text = Num_Bolla_A
                Me.Txt_DaNumeroBolla.Text = CStr(Doc_Numero_Min)

                If Num_Bolla_A = "" Or Num_Bolla_A = "0" Then
                    Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
                    Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)
                    'AgroMsgBox("E' necessario valorizzare il numero bolla A, per determinare l'ultimo numero bolla per cui verrà valorizzato il numero di certificato.", Page)
                    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Non è stato possibile ricavare il numero bolla A, necessario per determinare l'ultima bolla da valorizzare con il numero di certificato." + vbCrLf + "Specificare manualmente il Numero Bolla A:", Page)
                    Exit Sub
                End If

            End If

        Else
            'stampa singola
            Id_Agenda = Qs_IdAgenda
            Num_Bolla_A = "0"
            'Validita_Inizio = STR_DATAINIZIO
            'Validita_Fine = STR_DATAFINE
        End If

        Dim QueryString As String
        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                        "&pnba=" & Stringa_Codifica(Prefisso_Num_Bolla_A, AgroKey_EncoderDecoder, Server) + _
                        "&nba=" & Stringa_Codifica(Num_Bolla_A, AgroKey_EncoderDecoder, Server) + _
                        "&snba=" & Stringa_Codifica(Suffisso_Num_Bolla_A, AgroKey_EncoderDecoder, Server) + _
                        "&i=" & Stringa_Codifica(CStr(Id_Agenda), AgroKey_EncoderDecoder, Server)

        '"&vi=" & Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) + _
        '                "&vf=" & Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) + _

        Page_ModalDialog(Page, _
                        "GestioneNumerazioneCertificati.aspx", QueryString, "Txt_FlagInsertNumCert", _
                        PopupConti_HEIGHT, PopupConti_WIDTH, 0, 0, _
                        , , , , , , )

    End Sub

    '##################################################################################
    Private Sub ImgBtn_StampaCertificatoPomodoro_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaCertificatoPomodoro.Click

        StampaCertificatoPomodoro()

    End Sub

    '##################################################################################
    Private Sub StampaCertificatoPomodoro()

        Dim TargetURL As String
        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

        Select Case Me.Rbl_StampaPomodoro.SelectedValue

            Case 1 'BOLLA

                Dim Querystring As String

                Querystring = "?p=" & _
                                Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                                "&i=" & _
                                Stringa_Codifica(Qs_IdAgenda, AgroKey_EncoderDecoder, Server) & _
                                "&l=" & _
                                Stringa_Codifica(CStr(LAVCOD_ACCETTAZIONE_DIVERSI), AgroKey_EncoderDecoder, Server) & _
                                "&r=" & _
                                Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) & _
                                "&a=" & _
                                Stringa_Codifica(CStr(""), AgroKey_EncoderDecoder, Server)

                'Page_NewWindow(Server, Session, Page, _
                '                "../Contabilita/Fattura/BollaAccettazione.aspx", _
                '                Querystring, _
                '                "BollaAccettazione", _
                '                , , , , , , , )

                TargetURL = "../Contabilita/Fattura/BollaAccettazione.aspx" + Querystring

                '----------------------------------------------------------------------

            Case 2 'CERTIFICATO

                Select Case Me.Rbl_Certificati.SelectedValue

                    Case 1 'INTERNO
                        Session("ReportSelezionato") = enum_CodificaStampe.Certificato_Pomodoro_Interno

                    Case 2 'ESTERNO
                        'qui non dovrebbe mai entrare, perchè il default è sul cert interno
                        'e selezionando cert esterno, scatta l'evento di gestione della numerazione
                        Session("ReportSelezionato") = enum_CodificaStampe.Certificato_Pomodoro_Esterno
                End Select


                Dim Querystring As String

                Querystring = "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                            "&i=" + Stringa_Codifica(CStr(Qs_IdAgenda), AgroKey_EncoderDecoder, Server) + _
                            "&l=" + Stringa_Codifica(CStr(LAVCOD_ACCETTAZIONE_DIVERSI), AgroKey_EncoderDecoder, Server) + _
                            "&ptp=" + Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) + _
                            "&pn=" + Stringa_Codifica(CStr("true"), AgroKey_EncoderDecoder, Server) + _
                            "&ff=" + Stringa_Codifica("", AgroKey_EncoderDecoder, Server) + _
                            "&nc=" + Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server)

                Dim Pagina_CertificatoPomodoro As String = objADDFun.PaginaCertificatoPomodoro_from_IdAgenda(objParametri_Server, Qs_Piva, Qs_IdAgenda)


                'Page_NewWindow(Server, Session, Page, _
                '                Pagina_CertificatoPomodoro, _
                '                Querystring, _
                '                "CertificatoPomodoro", _
                '                , , , , , , , )

                TargetURL = Pagina_CertificatoPomodoro + Querystring


        End Select

        Response.Redirect(TargetURL)


    End Sub










End Class

