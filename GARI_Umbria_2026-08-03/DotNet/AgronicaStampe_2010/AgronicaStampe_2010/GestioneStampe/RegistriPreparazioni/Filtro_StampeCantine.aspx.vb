

Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010

Public Class Filtro_StampeCantine
    Inherits System.Web.UI.Page


    Protected WithEvents LABEL24 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL20 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL21 As System.Web.UI.WebControls.Label
    Protected WithEvents Rbl_DAA_Esemplare As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents LABEL22 As System.Web.UI.WebControls.Label
    Protected WithEvents Rbl_DAA_Pagina As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents ImgBtn_StampaDAA As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label23 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_DAA As System.Web.UI.WebControls.Panel
    Protected WithEvents Chk_StampaRiporti As System.Web.UI.WebControls.CheckBox
    Protected WithEvents LABEL4 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL5 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DataInizio As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_DataFine As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Date As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Temporale As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL16 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_StampaRegistroVuoto As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LABEL14 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL17 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_StampaFrontespizio As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LABEL6 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL25 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_StampaDOCO As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label26 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_DOCO As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL9 As System.Web.UI.WebControls.Label
    Protected WithEvents Rbl_Layout As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents ImgBtn_StampaSceltaLayout As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Layout_DOCO As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL27 As System.Web.UI.WebControls.Label
    Protected WithEvents IMAGE2 As System.Web.UI.WebControls.Image
    Protected WithEvents LABEL3 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtnEsci As System.Web.UI.WebControls.ImageButton
    Protected WithEvents ImageLogo As System.Web.UI.WebControls.Image
    Protected WithEvents Chk_StampaIntestazione_DOCO_Layout As System.Web.UI.WebControls.CheckBox
    Protected WithEvents Chk_StampaIntestazione_DOCO_Vuoto As System.Web.UI.WebControls.CheckBox
    Protected WithEvents LABEL31 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_TelematizzazioneAccise As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Txt_RagSoc As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_RegCantina As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_StampaDOCO As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_TipoReport As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Report As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL32 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL34 As System.Web.UI.WebControls.Label
    Protected WithEvents btnSalvaCacheRegistro As System.Web.UI.WebControls.ImageButton
    Protected WithEvents btnApriCache As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LABEL43 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL42 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Partita As System.Web.UI.WebControls.Panel
    Protected WithEvents Cmb_TipiRegistro As System.Web.UI.WebControls.DropDownList
    Protected WithEvents LABEL33 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_LineaProduzione As System.Web.UI.WebControls.DropDownList
    Protected WithEvents Cmb_Partita As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ImgBtn_StampaConsistenze As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label8 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL1 As System.Web.UI.WebControls.Label
    Protected WithEvents Cmb_Piano As System.Web.UI.WebControls.DropDownList
    Protected WithEvents LABEL39 As System.Web.UI.WebControls.Label
    Protected WithEvents Label40 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_VerificaRegistri As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label44 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_StampaEtichetteVasche As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LABEL46 As System.Web.UI.WebControls.Label
    Protected WithEvents CBL_InfoEtichetteVasche As System.Web.UI.WebControls.CheckBoxList
    Protected WithEvents LABEL47 As System.Web.UI.WebControls.Label
    Protected WithEvents CBL_VascheElenco As System.Web.UI.WebControls.CheckBoxList
    Protected WithEvents LABEL48 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_EtichetteVasche As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_StampaEtichetteVasche As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL49 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_SelezionaTutti As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Lbl_SelezionaTutteSpecie As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_DeselezionaTutti As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Lbl_DeselezionaTutteSpecie As System.Web.UI.WebControls.Label
    Protected WithEvents Label50 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL51 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL52 As System.Web.UI.WebControls.Label
    Protected WithEvents CheckBox1 As System.Web.UI.WebControls.CheckBox

    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " FILTRO REGISTRI DI CANTINA "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Filtro_StampeCantine_AbortTransaction(sender As Object, e As System.EventArgs) Handles Me.AbortTransaction

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As Integer
    Dim Qs_Report As Integer
    Dim Qs_Id_Agenda As Integer
    Dim Qs_Lav_Cod As Integer
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Enum enum_TipoOperazione
        StampaRegistriCantina = 1
        StampaVerificaRegistriCantina = 2
        StampaCopertinaRegistro = 3
        StampaRegistroVuoto = 4
        StampaConsistenzeEnologiche = 5
        BrogliaccioMovimentiCompleto = 6
        BrogliaccioMovimentiPdfSemplificato = 7
        RiepilogoImbottigliamenti = 8
        RegistroCommercializzazioneOBSOLETO = -1
    End Enum

    '######################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            'Faccio scadere subito la pagina memorizzata nella cache
            Response.Expires = 0
            Me.Master.Lbl_Titolo.Text = "Stampe Cantine"

            '#################################################################################
            '#####  Verifico che l'utente sia autenticato
            '#################################################################################
            'If Session("ASG_Utente_Username") = "" Then
            '    Page.FindControl("Form1").Controls.Add( _
            '        New LiteralControl("<script language='javascript'>window.close();</script>"))
            'End If

            If Session("ASG_objParametri_Server") Is Nothing Then
                Response.Redirect("~/Custom500.aspx")
            End If

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


            '#################################################################################
            '#####  Recupero dati dalla QueryString 
            '#################################################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)

            If Not IsNothing(Request.QueryString("s")) Then
                Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                                               AgroKey_EncoderDecoder, _
                                               Server)
            Else
                Qs_Sa_Cod = 0
            End If

            If Not IsNothing(Request.QueryString("rp")) Then
                Qs_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
                                               AgroKey_EncoderDecoder, _
                                               Server)
            Else
                Qs_Report = -1
            End If

            Qs_Id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), _
                                AgroKey_EncoderDecoder, _
                                Server))

            Qs_Lav_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))

            'PrintToPrinter = Stringa_Decodifica(CStr(Request.QueryString("r")), _
            '                            AgroKey_EncoderDecoder, _
            '                            Server)

            'PrintName = Stringa_Decodifica(CStr(Request.QueryString("a")), _
            '                            AgroKey_EncoderDecoder, _
            '                            Server)


            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            ''----- Verifico se l'utente dispone dei permessi
            'Dim UtenteAbilitato As Boolean
            'Dim strDummy As String      'controllo accesso negato.....

            'UtenteAbilitato = Controlla_Permessi_Utente_2( _
            '                        Server, Session, Page, _
            '                        Session("ASG_Utente_Username"), _
            '                        Session("ASG_IdServizio"), _
            '                        TipiEnumerativi.enum_Security_Attivita.Registri_Cantina, _
            '                        TipiEnumerativi.enum_Security_Operazione.Lettura, _
            '                        strDummy)

            ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
            'If UtenteAbilitato = False Then
            '    Response.Redirect("../../Messaggi/AccessoNegato.htm")
            'End If


            '=====================================================
            If Me.IsPostBack Then
                Exit Sub
            End If


            '=====================================================
            '§§§§
            'togliere quando si attivano le stampe
            'Me.ImgBtn_Stampa.Enabled = False
            'Me.ImgBtn_StampaConsistenze.Enabled = False
            'Me.ImgBtn_StampaDAA.Enabled = False
            'Me.ImgBtn_StampaDOCO.Enabled = False
            'Me.ImgBtn_StampaEtichetteVasche.Enabled = False
            'Me.ImgBtn_StampaFrontespizio.Enabled = False
            'Me.ImgBtn_StampaRegistroVuoto.Enabled = False
            'Me.ImgBtn_StampaSceltaLayout.Enabled = False
            'Me.ImgBtn_VerificaRegistri.Enabled = False
            'Me.ImgBtn_TelematizzazioneAccise.Enabled = False

            'AgroMsgBox("Pagina in manutenzione, stampe disattivate. Utilizzare AgronicaStampe_2003 (eliminare da configurazione_siti codice 106).", Page)
            'Exit Sub

            '=====================================================

            '-------------------------------------------------
            'attivazione vecchi registri di cantina solo per chi ha il permesso
            '-------------------------------------------------
            '----- Verifico se l'utente dispone dei permessi
            Dim UtenteAbilitato_RegistriCantinaOLD As Boolean
            Dim strDummy As String
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            UtenteAbilitato_RegistriCantinaOLD = objUtenti.Controlla_Permessi_Utente( _
                                    Session("ASG_Utente_Username"), _
                                    Session("ASG_IdServizio"), _
                                    enum_Security_Attivita.Registri_Cantina, _
                                    enum_Security_Operazione.Lettura, _
                                    Date.Today, _
                                    "", _
                                    objParametri_Utenti)

            If UtenteAbilitato_RegistriCantinaOLD = True Then
                Me.ddl_TipoOperazione.Items.Add(New WebControls.ListItem("Stampa Registro di commercializzazione (OBSOLETO)", -1))
                Me.Rbl_Report.Items.Add(New WebControls.ListItem("Registro di commercializzazione (OBSOLETO)", enum_AgroReportistica.Commercializzazione))
            End If


            '-------------------------------------------------
            'vanni: abilito la cache registri di cantina
            '-------------------------------------------------
            Dim usaCacheRegistri As Boolean = False
            Try
                usaCacheRegistri = (ConfigurationSettings.AppSettings("usaCacheRegistri") = "true")
            Catch ex As Exception

            End Try

            btnSalvaCacheRegistro.Visible = usaCacheRegistri
            btnApriCache.Visible = usaCacheRegistri

            '-------------------------------------------------
            'Controlli registri di cantina
            '-------------------------------------------------
            Dim Log_Errori As String = ""

            Try

                Controlli_x_RegistriCantina(Log_Errori)

            Catch ex As Exception
                Log_Errori += "errore: " & ex.Message
            End Try

            If Log_Errori <> "" Then
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(Log_Errori, Page, "MainContent", True)
            End If


            '-------------------------------------------------
            'Configura_PaginaFiltro
            '-------------------------------------------------

            Try

                Configura_PaginaFiltro()

            Catch ex As Exception
                Log_Errori += "errore: " & ex.Message
            End Try

            If Log_Errori <> "" Then
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(Log_Errori, Page, "MainContent", True)
            End If

            '-------------------------------------------------
            'Impostazioni utente sull'accettazione da diversi
            '-------------------------------------------------
            'COMMENTATO IL 14/08/2017: smantellati vecchi registri di cantina
            ''se non sono nella stampa delle etichette
            'If Qs_Report <> enum_CodificaStampe.EtichetteVascheEnologiche Then

            '    Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            '    Dim Chk_AccettazioneDaDiversi As String

            '    Chk_AccettazioneDaDiversi = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.Utente_ChkAccettazioneDaDiversi, _
            '                                                         objParametri_Utenti, _
            '                                                          1)

            '    Me.Div_Categoria.Visible = False

            '    ViewState("Chk_AccettazioneDaDiversi") = Chk_AccettazioneDaDiversi

            '    If Chk_AccettazioneDaDiversi = "1" Then

            '        Me.Rbl_Report.SelectedIndex = _
            '                    Me.Rbl_Report.Items.IndexOf(Me.Rbl_Report.Items.FindByValue(enum_AgroReportistica.Vinificazione_DOC))

            '        Cambia_Report()

            '        'imposto di default registro unico ma separato x c/terzi
            '        Me.Rbl_ContoTerzi.SelectedIndex = 1

            '        Cambia_ContoTerzi()

            '    End If

            'End If


            '-------------------------------------------------
            '---------- Configura_Layout
            '-------------------------------------------------
            Configura_Layout()


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(ex.Message, Page, "MainContent", True)
        End Try


    End Sub


    '####################################################################################################################################
    Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click

        Session("strXmlVariabilistampe") = Nothing

        Dim strClose As String = "<script language='javascript'> window.close() </script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub

    Private Enum enum_Pannello
        Nessuno = 0
        Reg_Cantina = 1
        Stampa_DOCO = 2
        Reg_BIO = 3
        Stampa_EtichettaVasche = 4
    End Enum

    '##########################################################################################################################################
    Private Sub Controlli_x_RegistriCantina(ByRef Log_Errori As String)

        '-----------------------------------------------------------------------------
        '---- Query di verifica per Registri di Cantina -------------
        '------------------------------------------------------------------------------

        'se non sono nella stampa delle etichette, verifico i dati per i registri di cantina
        If Qs_Report <> enum_CodificaStampe.EtichetteVascheEnologiche Then

            Dim objCTerzi As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim Esiste_GestioneContoTerzi As Boolean

            Esiste_GestioneContoTerzi = objCTerzi.Esiste_GestioneContoTerzi_RegCantina( _
                                                    Qs_Piva, _
                                                    0, _
                                                    objParametri_Server)

            ViewState("GestioneContoTerzi") = Esiste_GestioneContoTerzi

            Dim objRegistri As New AgronicaCoreStampeDAL.RegistriCantina
            Dim LogXUtente As String = ""
            Dim LogXAdmin As String = ""

            Try

                'verifica se ci sono dei lotti duplicati
                objRegistri.Verifica_Doppioni_Lotto(Qs_Piva, _
                                                    LogXUtente, _
                                                    LogXAdmin, _
                                                    objParametri_Server)


            Catch ex As Exception
                Log_Errori += "verifica doppioni lotto, errore: " + vbCrLf + ex.Message + vbCrLf
                LogXAdmin += "verifica doppioni lotto, errore: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                'verifica se ci sono dei lav_cod non configurati in opxreport
                objRegistri.Verifica_LavCodAgenda_OperazionixReport(Qs_Piva, _
                                                                    LogXUtente, _
                                                                    LogXAdmin, _
                                                                    objParametri_Server)


            Catch ex As Exception
                Log_Errori += "verifica campo lav_cod, errore: " + vbCrLf + ex.Message + vbCrLf
                LogXAdmin += "verifica campo lav_cod, errore: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                'verifica se ci sono operazioni pianificate
                objRegistri.Verifica_Operazioni_Pianificate(Qs_Piva, _
                                                    LogXUtente, _
                                                    LogXAdmin, _
                                                    objParametri_Server)


            Catch ex As Exception
                Log_Errori += "verifica Verifica_Operazioni_Pianificate, errore: " + vbCrLf + ex.Message + vbCrLf
                LogXAdmin += "verifica Verifica_Operazioni_Pianificate, errore: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                'verifica il jolly_int (per la movimentazione del magazzino)
                objRegistri.Verifica_JollyInt(Qs_Piva, _
                                            LogXUtente, _
                                            LogXAdmin, _
                                            objParametri_Server)


            Catch ex As Exception
                Log_Errori += "verifica movimentazione di magazzino, errore: " + vbCrLf + ex.Message + vbCrLf
                LogXAdmin += "verifica movimentazione di magazzino, errore: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'nel caso del c/terzi
            If ViewState("GestioneContoTerzi") = True Then

                Try

                    'verifico che i lotti delle altre materie prime sia configurati in lotto_proprieta sul c/terzi
                    objRegistri.Verifica_AltreMateriePrime_ConfigProprieta(Qs_Piva, _
                                                                            LogXUtente, _
                                                                            LogXAdmin, _
                                                                            objParametri_Server)


                Catch ex As Exception
                    Log_Errori += "verifica movimentazione di magazzino, errore: " + vbCrLf + ex.Message + vbCrLf
                    LogXAdmin += "verifica movimentazione di magazzino, errore: " + vbCrLf + ex.Message + vbCrLf
                End Try

            End If

            'COMMENTATO IL 14/08/2017: smantellati vecchi registri di cantina
            'Try

            '    'verifica record di movimenti che hanno 31/12/2100 o 01/01/1900 nel campo ORA (quello considerato nei registri di cantina)              " & vbCrLf)
            '    objRegistri.Verifica_Ora_31_12_2100(Qs_Piva, _
            '                                        LogXUtente, _
            '                                        LogXAdmin, _
            '                                        objParametri_Server)


            'Catch ex As Exception
            '    Log_Errori += "verifica campo Ora, errore: " + vbCrLf + ex.Message + vbCrLf
            '    LogXAdmin += "verifica campo Ora, errore: " + vbCrLf + ex.Message + vbCrLf
            'End Try

            'COMMENTATO IL 14/08/2017: smantellati vecchi registri di cantina
            'Try

            '    'query di update della tabella Trasformazioni_Riferimenti prerequisito x i registri
            '    Dim objTrasfRif As New AgronicaCoreContabDAL.Trasformazioni_Riferimenti_W
            '    objTrasfRif.Update_ValiditaInizio_ConOra(objParametri_Server)

            'Catch ex As Exception
            '    LogXAdmin += "Update validita_inizio Trasformazioni_Riferimenti, errore: " & ex.Message
            '    Log_Errori += "Update validita_inizio Trasformazioni_Riferimenti, errore: " & ex.Message
            'End Try

            'COMMENTATO IL 14/08/2017: smantellati vecchi registri di cantina
            'Try

            '    'verifica passaggio_data dei lotti già a commercializzazione
            '    objRegistri.Verifica_RilevamentiVinoSfuso_GiaACommercializzazione(Qs_Piva, _
            '                                                                    LogXUtente, _
            '                                                                    LogXAdmin, _
            '                                                                    objParametri_Server)


            'Catch ex As Exception
            '    Log_Errori += "verifica rilevamenti, errore: " + vbCrLf + ex.Message + vbCrLf
            '    LogXAdmin += "verifica rilevamenti, errore: " + vbCrLf + ex.Message + vbCrLf
            'End Try

            Dim str_datetime As String = Format(Date.Now, "yyyy_MM_dd__hh_mm_ss")


            If LogXAdmin <> "" Then

                Try

                    AgronicaCoreDataProvider.GestioneFile.CancellaFiles(objParametri_Server.LogDirectory, _
                                                                          "*" & Session("ASG_Utente_Username") & "_VerificaRegistriCantina.txt", _
                                                                          Log_Errori)


                Catch ex As Exception
                    Log_Errori += "errore: " & ex.Message
                End Try


                Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                    .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                    .LogDirectory = objParametri_Server.LogDirectory,
                    .LogFileName = objParametri_Server.PivaSuperUser & "_" & str_datetime & "_" & Session("ASG_Utente_Username") & "_VerificaRegistriCantina.txt"
                }

                Dim objDP As New AgronicaCoreDataProvider.LogProvider
                objDP.Scrivi_LOG(objParametri_Server, "Verifica_x_RegistriCantina", LogXAdmin, CustomLOGParams:=CustomLOGParams)

            End If

            If LogXUtente <> "" Then

                Try
                    Dim objAgroWC As New AgronicaCoreGestioneRichieste.AgroWebConfig

                    Try

                        AgronicaCoreDataProvider.GestioneFile.CancellaFiles(objAgroWC.GestioneAllegati_Repository, _
                                                                            "*" & Session("ASG_Utente_Username") & "_VerificaRegistriCantina.txt", _
                                                                            Log_Errori)

                    Catch ex As Exception
                        Log_Errori += "errore: " & ex.Message
                    End Try

                    Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                        .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                        .LogDirectory = objAgroWC.GestioneAllegati_Repository,
                        .LogFileName = objParametri_Server.PivaSuperUser & "_" & str_datetime & "_" & Session("ASG_Utente_Username") & "_VerificaRegistriCantina.txt"
                    }

                    Dim objDP As New AgronicaCoreDataProvider.LogProvider
                    objDP.Scrivi_LOG(objParametri_Server, "Verifica_x_RegistriCantina", LogXUtente, CustomLOGParams:=CustomLOGParams)


                Catch ex As Exception
                    Log_Errori += "errore: " & ex.Message
                End Try

                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(LogXUtente, Page, "MainContent", True)
            End If

        End If 'controlli


    End Sub

    '##########################################################################################################################################
    Private Sub Configura_PaginaFiltro()


        '---- Impresa
        Dim a As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim RSc As String = a.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
        Me.Txt_RagSoc.Text = RSc



        Select Case Qs_Report

            Case -1

                Dim filtroAnno As String = " (YEAR(Validita_Inizio) <> " & CStr(CDate(AGRODATAINIZIO).Year) & " AND YEAR(Validita_Inizio) <> " & CStr(CDate(AGRODATAFINE).Year) & ") "
                AgronicaCoreUtility.CaricaListControl.Anno_Agenda(Me.Cmb_Anno, _
                                                                    False, "", "", _
                                                                    Qs_Piva, _
                                                                    0, _
                                                                    filtroAnno, "", _
                                                                    objParametri_Server)
                AgronicaCoreUtility.CaricaListControl.Mesi2(Me.Cmb_Mese, True, "Seleziona ...", "0")


                ''leggo i report attivi alla data odierna
                'objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Date.Today, Date.Today)

                'Dim Str_Opt_Gestione_RegistroVinificazione As String
                'Dim Opt_Gestione_RegistroVinificazione As Integer
                'Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                'Str_Opt_Gestione_RegistroVinificazione = objUtenti.ImpostazioneValore1_from_ImpostazioneCod( _
                '                                                enum_Impostazioni_Utenti.SUPERUSER_COD_GESTIONE_REG_VINIFICAZIONE, _
                '                                                 objParametri_Utenti, _
                '                                                  2)
                'If IsNumeric(Str_Opt_Gestione_RegistroVinificazione) Then
                '    Opt_Gestione_RegistroVinificazione = CInt(Str_Opt_Gestione_RegistroVinificazione)
                'Else
                '    Opt_Gestione_RegistroVinificazione = 0
                'End If

                'If Opt_Gestione_RegistroVinificazione = 1 Then
                '    'OMNI attivo: VINIFICAZIONE ATTIVA -> si può stampare il registro
                '    Str_Opt_Gestione_RegistroVinificazione = ""
                'Else
                '    'filtrare i reg di vinificazione
                '    'riciclo la variabile x fare il filtro
                '    Str_Opt_Gestione_RegistroVinificazione = " Id_Report NOT IN (1,2) "
                'End If

                'AgronicaCoreUtility.CaricaListControl.Agro_Reportistica(Me.Rbl_Report, _
                '                                                         True, "---", "0", _
                '                                                         0, _
                '                                                         enum_AgroReportisticaTipi.RegistriCantina, _
                '                                                         Str_Opt_Gestione_RegistroVinificazione, "", _
                '                                                         objParametri_Server)

                'objParametri_Server.ResettaFinestra()

                ''registro di commercializzazione
                'Me.Rbl_Report.SelectedIndex = _
                '   Me.Rbl_Report.Items.IndexOf(Me.Rbl_Report.Items.FindByValue(enum_AgroReportistica.Commercializzazione))


                If ViewState("GestioneContoTerzi") = True Then

                    'caricamento menù a tendina contatti terzi per registro vinificazione e commercializzazione
                    'il caricamento lo metto qui, così la query viene fatta solo una volta all'apertura della pagina
                    'e al cambio di selezione del report visualizzo o meno

                    'prima di caricare il menù a tendina + necessario un update della tabella linee_produzioni

                    Dim objLineeProd As New AgronicaCoreContabDAL.Linee_Produzioni_W
                    Dim risp As Boolean

                    risp = objLineeProd.AggiornaCodContattoTerzi_DefaultxRegistri( _
                                                                Qs_Piva, _
                                                                "", _
                                                                objParametri_Server)


                    AgronicaCoreUtility.CaricaListControl.LineeProduzioni_ContattiTerzi( _
                                                            Me.Cmb_ContattiContoTerzi, _
                                                            True, "Nessuna selezione", "", _
                                                            Qs_Piva, _
                                                            "", _
                                                            "", _
                                                            0, _
                                                            "", "", _
                                                            objParametri_Server)

                    Cambia_ContoTerzi()

                Else
                    ViewState("GestioneContoTerzi") = False
                End If

                'Cambia_Report()

                'Cambia_MeseIntervallo()

                'Cambia_1a_VoceRiepilogo()
                'Cambia_2_Linea()

                ''" com_cod_istat<>'000' and pro_cod_istat<>'000' "
                'AgronicaCoreUtility.CaricaListControl.Indirizzi_ImpresaCentro( _
                '                                            Me.Cmb_Indirizzo, _
                '                                            False, "", "", _
                '                                            Qs_Piva, _
                '                                            0, 1, 0, _
                '                                            "", _
                '                                            "", _
                '                                            "", _
                '                                            objParametri_Server)

                ' Cambia_FiltriVerifica()



                '-------------------------------------------
                '04/02/2016: DAA/DOCO doc obsoleti quindi non più gestiti
                'DAA
                'Me.Rbl_DAA_Esemplare.SelectedIndex = 0
                'Me.Rbl_DAA_Pagina.SelectedIndex = 0
                '-------------------------------------------


                'CONSISTENZE ENOLOGICHE
                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.Centri_Aziendali(Me.Cmb_CentroAziendale,
                                                                        True, "", "0",
                                                                        Qs_Piva,
                                                                        True,
                                                                        2,
                                                                        "", "",
                                                                        objParametri_Server)

                'linee
                AgronicaCoreUtility.CaricaListControl.Linee_Produzioni(Me.Cmb_LineeConsistenze, _
                                                                        True, "Nessun filtro", "", _
                                                                        Qs_Piva, _
                                                                        "", _
                                                                        0, _
                                                                        0, _
                                                                        3, _
                                                                        "", "", _
                                                                        objParametri_Server)

                Cambia_Centro_ConsEno()

                Me.Txt_DataConsistenze.Text = Date.Today.ToShortDateString

                '===================================================================

            Case enum_CodificaStampe.EtichetteVascheEnologiche

                Carica_ElencoVasche()

            Case Else


        End Select

    End Sub


    '##########################################################################################################################################
    Private Sub Imposta_Pannelli(ByVal panel As enum_Pannello)

        '----- Disattivo tutti i pannelli
        Me.Pannello_RegCantina.Visible = False
        Me.Pannello_StampaDOCO.Visible = False
        Me.Pannello_StampaEtichetteVasche.Visible = False
        Me.Pannello_TipoReport.Visible = False

        Select Case panel
            Case enum_Pannello.Reg_Cantina
                Me.Pannello_TipoReport.Visible = True
                Me.Pannello_RegCantina.Visible = True
            Case enum_Pannello.Stampa_DOCO
                Me.Pannello_StampaDOCO.Visible = True
            Case enum_Pannello.Stampa_EtichettaVasche
                Me.Pannello_StampaEtichetteVasche.Visible = True
            Case enum_Pannello.Reg_BIO
                Me.Pannello_TipoReport.Visible = True
        End Select

        Dim Dimensione As Unit

        '----- Imposto le dimensioni
        With Me.Pannello_StampaDOCO
            .Height = Dimensione.Pixel(350)
            .Width = Dimensione.Pixel(968)
            .Style.Item("Top") = 232
            .Style.Item("Left") = 8
        End With

        '----- Imposto le dimensioni
        With Me.Pannello_StampaEtichetteVasche
            .Height = Dimensione.Pixel(770)
            .Width = Dimensione.Pixel(970)
            .Style.Item("Top") = 96
            .Style.Item("Left") = 8
        End With


        '----- Imposto le dimensioni
        With Me.Pannello_RegCantina
            .Height = Dimensione.Pixel(650)
            .Width = Dimensione.Pixel(968)
            .Style.Item("Top") = 216
            .Style.Item("Left") = 8
        End With

        '----- Imposto le dimensioni
        With Me.Pannello_TipoReport
            .Height = Dimensione.Pixel(112)
            .Width = Dimensione.Pixel(968)
            '.Style.Item("Top") = 16
            '.Style.Item("Left") = 8
        End With
        With Me.Pannello_Report
            .Height = Dimensione.Pixel(96)
            .Width = Dimensione.Pixel(960)
            '.Style.Item("Top") = 16
            '.Style.Item("Left") = 8
        End With
        With Me.Pannello_ConsEnologiche
            .Height = Dimensione.Pixel(104)
            .Width = Dimensione.Pixel(960)
            '.Style.Item("Top") = 16
            '.Style.Item("Left") = 8
        End With

    End Sub

    '##########################################################################################################################################
    Private Sub Carica_ElencoVasche()

        Dim strXmlVariabilistampe As String
        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        If strXmlVariabilistampe = "" Then
            'AgroMsgBox("strXmlVariabilistampe vuota", Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("strXmlVariabilistampe vuota", Page, "MainContent", True)
            Exit Sub
        End If

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_Vasca As System.Xml.XmlNodeList
        Dim XML_Vasca As System.Xml.XmlElement
        Dim XMLs_Cantina As System.Xml.XmlNodeList
        Dim XML_Cantina As System.Xml.XmlElement

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument

        Try

            strXmlVariabilistampe = strXmlVariabilistampe.Replace("{", "<")
            strXmlVariabilistampe = strXmlVariabilistampe.Replace("}", ">")

            XmlDoc.LoadXml(strXmlVariabilistampe)

            Dim i, j As Integer
            Dim id As String
            Dim reparto As String
            Dim chk_vasca As System.Web.UI.WebControls.ListItem
            Dim HT_Vasche As New Hashtable

            If XmlDoc.HasChildNodes Then

                'modifica per passaggio da stampe 2003 a stampe 2010
                'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
                'XMLs_Cantina = XML_FiltroStampa.GetElementsByTagName("CANTINA")
                XMLs_Cantina = XmlDoc.SelectNodes("//CANTINA")

                For i = 0 To XMLs_Cantina.Count - 1

                    XML_Cantina = XMLs_Cantina(i)

                    reparto = XML_Cantina.GetAttribute("des")

                    XMLs_Vasca = XML_Cantina.GetElementsByTagName("VASCA")

                    For j = 0 To XMLs_Vasca.Count - 1

                        XML_Vasca = XMLs_Vasca(j)

                        id = XML_Vasca.GetAttribute("des1")

                        If Not HT_Vasche.ContainsKey(id) Then
                            HT_Vasche.Add(id, 0)
                            chk_vasca = New System.Web.UI.WebControls.ListItem(id & " (" & reparto & ")", id)
                            Me.CBL_VascheElenco.Items.Add(chk_vasca)
                            Me.CBL_VascheElenco.Items.FindByValue(id).Selected = XML_Vasca.GetAttribute("flag_checked")
                        End If
                    Next

                Next

            End If

        Catch ex As Exception
            'AgroMsgBox("errore caricamento elenco vasche: " & ex.Message, Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("errore caricamento elenco vasche: " & ex.Message, Page, "MainContent", True)
        End Try

    End Sub


    '###################################################################################
    Private Sub ImgBtn_SelezionaTutti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SelezionaTutti.Click

        Dim i As Integer

        For i = 0 To Me.CBL_VascheElenco.Items.Count - 1
            Me.CBL_VascheElenco.Items(i).Selected = True
        Next

    End Sub


    '####################################################################################
    Private Sub ImgBtn_DeselezionaTutti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DeselezionaTutti.Click

        Dim i As Integer

        For i = 0 To Me.CBL_VascheElenco.Items.Count - 1
            Me.CBL_VascheElenco.Items(i).Selected = False
        Next

    End Sub

    '#########################################################################################################################################################
    'Private Sub ImgBtn_StampaEtichetteVasche_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaEtichetteVasche.Click

    '    Stampa_EtichetteVasche()

    'End Sub

    '#########################################################################################################################################################
    Private Sub Stampa_EtichetteVasche()

        Try

            Dim Opt_Identificativo As Boolean = Me.CBL_InfoEtichetteVasche.Items(0).Selected
            Dim Opt_Capacita As Boolean = Me.CBL_InfoEtichetteVasche.Items(1).Selected
            'Dim Opt_Linea As Boolean = Me.CBL_InfoEtichetteVasche.Items(2).Selected
            Dim Opt_Lotto As Boolean = Me.CBL_InfoEtichetteVasche.Items(2).Selected
            Dim Opt_Colore As Boolean = Me.CBL_InfoEtichetteVasche.Items(3).Selected
            Dim Opt_AnnoProd As Boolean = Me.CBL_InfoEtichetteVasche.Items(4).Selected
            Dim Opt_GradoBabo As Boolean = Me.CBL_InfoEtichetteVasche.Items(5).Selected
            Dim Opt_Regolamento As Boolean = Me.CBL_InfoEtichetteVasche.Items(6).Selected
            Dim Opt_AttoApprovazione As Boolean = Me.CBL_InfoEtichetteVasche.Items(7).Selected
            Dim Opt_ContoLav As Boolean = Me.CBL_InfoEtichetteVasche.Items(8).Selected
            Dim Opt_Qta As Boolean = Me.CBL_InfoEtichetteVasche.Items(9).Selected


            Dim Opt_MatDes As Boolean = False
            Dim Opt_Linea As Boolean = False

            Select Case Me.Rbl_OptEtichetta.SelectedValue
                Case 0 'nessuno
                Case 1 'linea
                    Opt_Linea = True
                Case 2 'semilavorato
                    Opt_MatDes = True
            End Select

            Dim Reparto As String
            Dim Str_Identificativo As String
            Dim Str_Capacita As String = ""
            Dim Str_Linea As String = ""
            Dim Str_GradoBabo As String = ""
            Dim Str_ContoLav As String = ""
            Dim Str_Colore As String = ""
            Dim Str_AnnoProd As String = ""
            Dim Str_Regolamento As String = ""
            Dim Str_AttoApprovazione As String = ""
            Dim Str_Qta As String = ""
            Dim Str_Lotto As String = ""
            Dim Str_MatDes As String = ""

            Dim j, z As Integer
            Dim QueryString, Titolo, TargetURL As String


            Dim strXmlVariabilistampe As String
            strXmlVariabilistampe = Session("strXmlVariabilistampe")

            If strXmlVariabilistampe = "" Then
                'AgroMsgBox("strXmlVariabilistampe vuota", Page)
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("strXmlVariabilistampe vuota", Page, "MainContent", True)
                Exit Sub
            End If

            strXmlVariabilistampe = strXmlVariabilistampe.Replace("{", "<")
            strXmlVariabilistampe = strXmlVariabilistampe.Replace("}", ">")

            Dim XmlDoc As New System.Xml.XmlDocument
            Dim XMLs_Vasca As System.Xml.XmlNodeList
            Dim XML_Vasca As System.Xml.XmlElement

            If Not IsNothing(CBL_VascheElenco) AndAlso Me.CBL_VascheElenco.Items.Count > 0 Then

                XmlDoc = New System.Xml.XmlDocument

                XmlDoc.LoadXml(strXmlVariabilistampe)

                If Not IsNothing(XmlDoc) AndAlso XmlDoc.HasChildNodes Then

                    'Dim DSEtichette As New DS_EtichetteVasche.DT_EtichetteVascheDataTable
                    'Dim DSEticRow As DS_EtichetteVasche.DT_EtichetteVascheRow
                    Dim DT_Etichette As New DataTable
                    Dim Dr As DataRow

                    DT_Etichette.Columns.Add(New DataColumn("Reparto", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Identificativo", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Capacita", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Linea", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Grado_Babo", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("ContoLav", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Colore", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("AnnoProduzione", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Regolamento", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("AttoApprovazione", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Qta", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Lotto", GetType(String)))
                    DT_Etichette.Columns.Add(New DataColumn("Semilavorato", GetType(String)))

                    For z = 0 To Me.CBL_VascheElenco.Items.Count - 1

                        If Me.CBL_VascheElenco.Items(z).Selected = True Then

                            Str_Identificativo = Trim(CStr(Me.CBL_VascheElenco.Items(z).Text).Split("(")(0))
                            Reparto = Trim(CStr(Me.CBL_VascheElenco.Items(z).Text).Split("(")(1))
                            Reparto = Left(Reparto, Reparto.Length - 1)

                            If Str_Identificativo <> "" Then

                                'recupero l'elemento xml di quella vasca
                                XMLs_Vasca = XmlDoc.SelectNodes("//VASCA[@des1='" & Str_Identificativo & "']")

                                If Not IsNothing(XMLs_Vasca) AndAlso XMLs_Vasca.Count > 0 Then

                                    For j = 0 To XMLs_Vasca.Count - 1

                                        Dr = DT_Etichette.NewRow

                                        Dr.Item("Reparto") = Reparto
                                        Dr.Item("Identificativo") = Str_Identificativo

                                        XML_Vasca = XMLs_Vasca(j)

                                        If XML_Vasca.HasAttribute("capacita") Then
                                            Str_Capacita = XML_Vasca.GetAttribute("capacita")
                                        Else
                                            Str_Capacita = ""
                                        End If
                                        'DSEticRow.Capacita = Str_Capacita
                                        Dr.Item("Capacita") = Str_Capacita

                                        If XML_Vasca.HasAttribute("linea_des") Then
                                            Str_Linea = XML_Vasca.GetAttribute("linea_des")
                                        Else
                                            Str_Linea = ""
                                        End If
                                        'DSEticRow.Linea = Str_Linea
                                        Dr.Item("Linea") = Str_Linea

                                        If XML_Vasca.HasAttribute("analisi_gradobabo") Then
                                            Str_GradoBabo = XML_Vasca.GetAttribute("analisi_gradobabo")
                                        Else
                                            Str_GradoBabo = ""
                                        End If
                                        'DSEticRow.Grado_Babo = Str_GradoBabo
                                        Dr.Item("Grado_Babo") = Str_GradoBabo

                                        If XML_Vasca.HasAttribute("rag_soc_terzi") Then
                                            Str_ContoLav = XML_Vasca.GetAttribute("rag_soc_terzi")
                                        End If
                                        'DSEticRow.ContoLav = Str_ContoLav
                                        Dr.Item("ContoLav") = Str_ContoLav

                                        If XML_Vasca.HasAttribute("colore") Then
                                            Str_Colore = XML_Vasca.GetAttribute("colore")
                                        Else
                                            Str_Colore = ""
                                        End If
                                        ' DSEticRow.Colore = Str_Colore
                                        Dr.Item("Colore") = Str_Colore

                                        If XML_Vasca.HasAttribute("lotto") Then
                                            If IsNumeric(Left(XML_Vasca.GetAttribute("lotto"), 2)) Then
                                                Str_AnnoProd = Left(XML_Vasca.GetAttribute("lotto"), 2)
                                                Str_Lotto = XML_Vasca.GetAttribute("lotto")
                                            Else
                                                Str_AnnoProd = "ND"
                                            End If
                                        Else
                                            Str_AnnoProd = ""
                                        End If
                                        'DSEticRow.AnnoProduzione = Str_AnnoProd
                                        Dr.Item("AnnoProduzione") = Str_AnnoProd

                                        'If XML_Vasca.HasAttribute("des3") Then
                                        '    Str_Lotto = XML_Vasca.GetAttribute("des3")
                                        'Else
                                        '    Str_Lotto = ""
                                        'End If
                                        Dr.Item("Lotto") = Str_Lotto

                                        If XML_Vasca.HasAttribute("regolamento") Then
                                            Str_Regolamento = XML_Vasca.GetAttribute("regolamento")
                                        Else
                                            Str_Regolamento = ""
                                        End If
                                        ' DSEticRow.Regolamento = Str_Regolamento
                                        Dr.Item("Regolamento") = Str_Regolamento

                                        If XML_Vasca.HasAttribute("aaa") Then
                                            Str_AttoApprovazione = "" 'XML_Vasca.GetAttribute("")
                                        End If
                                        ' DSEticRow.AttoApprovazione = Str_AttoApprovazione
                                        Dr.Item("AttoApprovazione") = Str_AttoApprovazione

                                        If XML_Vasca.HasAttribute("qta") And XML_Vasca.HasAttribute("udm_des") Then
                                            Str_Qta = XML_Vasca.GetAttribute("qta") & " " & XML_Vasca.GetAttribute("udm_des")
                                        Else
                                            Str_Qta = ""
                                        End If
                                        ' DSEticRow.Qta = Str_Qta
                                        Dr.Item("Qta") = Str_Qta

                                        If XML_Vasca.HasAttribute("mat_des") Then
                                            Str_MatDes = XML_Vasca.GetAttribute("mat_des")
                                        Else
                                            Str_MatDes = ""
                                        End If
                                        Dr.Item("Semilavorato") = Str_MatDes

                                        'DSEtichette.AddDT_EtichetteVascheRow(DSEticRow)

                                        DT_Etichette.Rows.Add(Dr)

                                    Next

                                Else
                                    'AgroMsgBox("Identificativo non recuperato, serbatoio " & CStr(z + 1), Page)
                                    AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Identificativo non recuperato, serbatoio " & CStr(z + 1), Page, "MainContent", True)
                                End If

                            End If 'xpath

                        End If 'vasca selezionata


                    Next 'CBL_VascheElenco

                    'Session("DSEtichette") = DSEtichette
                    Session("DT_Etichette") = DT_Etichette

                    Titolo = "EtichettaSerbatoio" '& Str_Identificativo

                    TargetURL = "../Cantine/EtichetteVascheEnologiche/EtichetteVascheEnologiche.aspx"

                    QueryString = "?oid=" & Stringa_Codifica(Opt_Identificativo, AgroKey_EncoderDecoder, Server) & _
                                    "&ocap=" & Stringa_Codifica(Opt_Capacita, AgroKey_EncoderDecoder, Server) & _
                                    "&olin=" & Stringa_Codifica(Opt_Linea, AgroKey_EncoderDecoder, Server) & _
                                    "&obabo=" & Stringa_Codifica(Opt_GradoBabo, AgroKey_EncoderDecoder, Server) & _
                                    "&oclav=" & Stringa_Codifica(Opt_ContoLav, AgroKey_EncoderDecoder, Server) & _
                                    "&ocol=" & Stringa_Codifica(Opt_Colore, AgroKey_EncoderDecoder, Server) & _
                                    "&oap=" & Stringa_Codifica(Opt_AnnoProd, AgroKey_EncoderDecoder, Server) & _
                                    "&oreg=" & Stringa_Codifica(Opt_Regolamento, AgroKey_EncoderDecoder, Server) & _
                                    "&oaa=" & Stringa_Codifica(Opt_AttoApprovazione, AgroKey_EncoderDecoder, Server) & _
                                    "&oqta=" & Stringa_Codifica(Opt_Qta, AgroKey_EncoderDecoder, Server) & _
                                    "&p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                                    "&osem=" & Stringa_Codifica(Opt_MatDes, AgroKey_EncoderDecoder, Server) & _
                                    "&olot=" & Stringa_Codifica(Opt_Lotto, AgroKey_EncoderDecoder, Server)



                    Page_NewWindow_2010(Page, _
                        TargetURL, QueryString, Titolo, , , , , , , , "MainContent", True)


                End If ' XmlDoc
            Else
                'AgroMsgBox("Non sono presenti serbatoi enologici.", Page)
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Non sono presenti serbatoi enologici.", Page, "MainContent", True)
            End If 'CBL_VascheElenco

        Catch ex As Exception

        End Try


    End Sub


    '##########################################################################################################################################
    Private Sub Rbl_Report_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Report.SelectedIndexChanged
        Cambia_Report()
    End Sub


    '##########################################################################################################################################
    Private Sub Cambia_Report()

        Me.Pannello_Partita.Visible = False
        Me.Div_Periodo.Visible = False
        Me.Pannello_Date.Visible = False
        Me.Pannello_AnnoMese.Visible = False
        Me.Div_ContoTerzi.Visible = False
        Me.Div_Categoria.Visible = False

        'i riporti non sono più gestiti
        Me.Chk_StampaRiporti.Visible = False

        Dim chkregistri As Integer = 0
        Dim chkregistri_vinificazione As Integer = 0

        '-------------------------------------------
        '----------- REGISTRI CANTINA --------------
        '-------------------------------------------
        Select Case Me.Rbl_Report.SelectedValue

            Case enum_AgroReportistica.Imbottigliamento

                Me.Div_Periodo.Visible = True
                Cambia_MeseIntervallo()
                Me.Div_Categoria.Visible = True
                Me.Div_ContoTerzi.Visible = ViewState("GestioneContoTerzi")

                '----------------------------------------------------

            Case enum_AgroReportistica.Frizzanti

                'MODIFICA IN DATA 26/09/13: non si usa più tipo_default 
                '(è ora usato per il raggruppamento delle preparazioni, vedi menù a tendina caricato dal lotto)

                Dim filtro_friz As String = ""
                'filtro_friz = " Linee_Preparazioni.Tipo_Default IN (" & _
                '            CStr(enum_Omni_Codice_Generazione.Tipo10_Frizzantatura) & _
                '            "," & _
                '            CStr(enum_Omni_Codice_Generazione.Tipo10_Frizzantatura_Imbottigliamento) & _
                '            " ) "
                filtro_friz += " Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
                filtro_friz += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_VinoAttoA_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_VinoAttoA_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveMPF_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveMPFxVinoAttoADivenire_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveVNAF_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveVNAFxVinoAttoADivenire_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegCommercializzazione) & _
                            " ) "

                'AGGIORNARE ANCHE LA QUERY AgronicaCoreContabDAL.MovimentixReport_R.Leggi_Partite_LineeProduzioni()

                AgronicaCoreUtility.CaricaListControl.Linee_Produzioni_TipoDefault(Cmb_LineaProduzione, _
                                                                                    False, "", "", _
                                                                                    Qs_Piva, _
                                                                                    0, _
                                                                                    4, _
                                                                                    filtro_friz, "", _
                                                                                    objParametri_Server)

                If Cmb_LineaProduzione.Items.Count > 0 Then
                    Cmb_LineaProduzione.SelectedIndex = 0
                End If

                CaricaPartita()

                Me.Pannello_Partita.Visible = True
                Me.Div_Periodo.Visible = False

                '----------------------------------------------------

            Case enum_AgroReportistica.Spumanti

                'MODIFICA IN DATA 26/09/13: non si usa più tipo_default 
                '(è ora usato per il raggruppamento delle preparazioni, vedi menù a tendina caricato dal lotto)

                Dim filtro_spum As String = ""
                'filtro_spum = " Linee_Preparazioni.Tipo_Default IN (" & _
                '            CStr(enum_Omni_Codice_Generazione.Tipo10_Spumantizzazione) & _
                '            " ) "
                filtro_spum += " Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine)
                filtro_spum += " AND Linee_Preparazioni.Codice_Generazione IN (" & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_VinoAttoA_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_VinoAttoA_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveMPF_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveVNAF_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveMPF_XVinoAttoADivenire_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveVNAF_XVinoAttoADivenire_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegCommercializzazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegVinificazione) & "," & _
                            CStr(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegCommercializzazione) & _
                            " ) "

                AgronicaCoreUtility.CaricaListControl.Linee_Produzioni_TipoDefault(Cmb_LineaProduzione, _
                                                                    False, "", "", _
                                                                    Qs_Piva, _
                                                                    0, 4, _
                                                                    filtro_spum, "", _
                                                                    objParametri_Server)

                If Cmb_LineaProduzione.Items.Count > 0 Then
                    Cmb_LineaProduzione.SelectedIndex = 0
                End If

                CaricaPartita()

                Me.Pannello_Partita.Visible = True
                Me.Div_Periodo.Visible = False

                '----------------------------------------------------

            Case enum_AgroReportistica.Commercializzazione

                chkregistri = 1

                'data 17/09/2012: visualizzo il filtro del contatto per conto terzi
                Me.Div_ContoTerzi.Visible = ViewState("GestioneContoTerzi")

                Me.Div_Periodo.Visible = True
                Cambia_MeseIntervallo()

                '----------------------------------------------------

            Case enum_AgroReportistica.Vinificazione_DOC, _
                    enum_AgroReportistica.Vinificazione_ViniTavola

                chkregistri_vinificazione = 1

                'visualizzo il filtro del contatto per conto terzi
                Me.Div_ContoTerzi.Visible = ViewState("GestioneContoTerzi")

                Me.Div_Periodo.Visible = True
                Cambia_MeseIntervallo()

                If ViewState("Chk_AccettazioneDaDiversi") = "1" Then
                    Me.Div_Categoria.Visible = True
                End If
                '----------------------------------------------------

        End Select

        If chkregistri_vinificazione <> 0 Or chkregistri <> 0 Then

            'modifica del 17/06/2014: tolto il filtro piva (che veniva usato sulla tabella Materie_Prime_ParametriQualitativi)
            'perchè magari la materia prima è creata su altra piva
            AgronicaCoreUtility.CaricaListControl.Registri_VociDiRiepilogo(Me.Cmb_VociRiepilogo, _
                                                                     True, "Nessun filtro", "", _
                                                                     "", _
                                                                     0, 0, 0, _
                                                                     chkregistri, _
                                                                     chkregistri_vinificazione, _
                                                                     1, _
                                                                      "", "", _
                                                                     objParametri_Server)


            AgronicaCoreUtility.CaricaListControl.Linee_Produzioni(Me.Cmb_Linee, _
                                                                    True, "Nessun filtro", "", _
                                                                    Qs_Piva, _
                                                                    "", _
                                                                    Me.Rbl_Report.SelectedValue, _
                                                                    0, _
                                                                    3, _
                                                                    "", "", _
                                                                    objParametri_Server)


            Cambia_1a_VoceRiepilogo()
            Cambia_2_Linea()

        End If



    End Sub

    '##########################################################################################################################################
    Private Sub Rbl_ContoTerzi_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_ContoTerzi.SelectedIndexChanged
        Cambia_ContoTerzi()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_ContoTerzi()

        Me.Cmb_ContattiContoTerzi.SelectedIndex = 0
        Me.Cmb_ContattiContoTerzi.Enabled = False

        Select Case Me.Rbl_ContoTerzi.SelectedValue

            Case enum_RegistroContoTerzi.RegistroGlobale

            Case enum_RegistroContoTerzi.RegistroUnicoDiversificato

            Case enum_RegistroContoTerzi.RegistroSeparatoContoTerzi

                Me.Cmb_ContattiContoTerzi.Enabled = True
                'If Cmb_ContattiContoTerzi.Items.Count > 1 Then
                '    Me.Cmb_ContattiContoTerzi.SelectedIndex = 1
                'End If

                Me.Cmb_ContattiContoTerzi.SelectedIndex = _
                Me.Cmb_ContattiContoTerzi.Items.IndexOf(Me.Cmb_ContattiContoTerzi.Items.FindByValue(Qs_Piva))

        End Select

    End Sub


    '##########################################################################################################################################
    Private Sub Cmb_Linee_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Linee.SelectedIndexChanged
        Cambia_2_Linea()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_2_Linea()

        Me.Cmb_MateriePrimeByLinee.Items.Clear()

        If Not IsNothing(Me.Cmb_Linee.SelectedItem) AndAlso _
            Me.Cmb_Linee.SelectedValue <> "" AndAlso _
                Me.Cmb_Linee.SelectedValue <> "0" Then

            Dim chkregistri As Integer = 0
            Dim chkregistri_vinificazione As Integer = 0

            Select Case Me.Rbl_Report.SelectedValue
                Case enum_AgroReportistica.Commercializzazione
                    chkregistri = 1
                    '----------------------------------------------------
                Case enum_AgroReportistica.Vinificazione_DOC, _
                        enum_AgroReportistica.Vinificazione_ViniTavola
                    chkregistri_vinificazione = 1
                    '----------------------------------------------------
                Case Else
                    Me.Cmb_MateriePrimeByLinee.Items.Clear()
                    Exit Sub
            End Select

            AgronicaCoreUtility.CaricaListControl.MateriePrimeByLinee_LogOmni( _
                                                    Me.Cmb_MateriePrimeByLinee, _
                                                    True, _
                                                    "Nessun filtro", _
                                                     "", _
                                                     Qs_Piva, _
                                                     Me.Cmb_Linee.SelectedValue, _
                                                     0, _
                                                      Me.Rbl_Report.SelectedValue, _
                                                      4, _
                                                    "", _
                                                    "", _
                                                    objParametri_Server)

        End If

    End Sub

    '##########################################################################################################################################
    Private Sub Cmb_VociRiepilogo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_VociRiepilogo.SelectedIndexChanged
        Cambia_1a_VoceRiepilogo()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_1a_VoceRiepilogo()

        Me.Cmb_MateriePrime.Items.Clear()
        Me.cmb_LineeByMat_Cod.Items.Clear()

        If Not IsNothing(Me.Cmb_VociRiepilogo.SelectedItem) AndAlso _
            Me.Cmb_VociRiepilogo.SelectedValue <> "" AndAlso _
                Me.Cmb_VociRiepilogo.SelectedValue <> "0" Then

            Dim chkregistri As Integer = 0
            Dim chkregistri_vinificazione As Integer = 0

            Select Case Me.Rbl_Report.SelectedValue
                Case enum_AgroReportistica.Commercializzazione
                    chkregistri = 1
                    '----------------------------------------------------
                Case enum_AgroReportistica.Vinificazione_DOC, _
                        enum_AgroReportistica.Vinificazione_ViniTavola
                    chkregistri_vinificazione = 1
                    '----------------------------------------------------
                Case Else
                    Exit Sub
            End Select

            'modifica del 17/06/2014: tolto il filtro piva 
            'perchè magari la materia prima è creata su altra piva
            AgronicaCoreUtility.CaricaListControl.MateriePrimeByVoceDiRiepilogo( _
                                                    Me.Cmb_MateriePrime, _
                                                    True, _
                                                    "Nessun filtro", _
                                                    "0", _
                                                    Me.Cmb_VociRiepilogo.SelectedValue, _
                                                    Me.Rbl_Report.SelectedValue, _
                                                    chkregistri, _
                                                    chkregistri_vinificazione, _
                                                    Qs_Piva, _
                                                    0, _
                                                    0, _
                                                    True, _
                                                    2, _
                                                    "", _
                                                    "", _
                                                    objParametri_Server)

        End If

    End Sub

    '##########################################################################################################################################
    Private Sub Cmb_MateriePrime_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_MateriePrime.SelectedIndexChanged
        Cambia_1b_MatPrima_byVoce()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_1b_MatPrima_byVoce()

        Me.cmb_LineeByMat_Cod.Items.Clear()

        If Not IsNothing(Me.Cmb_MateriePrime.SelectedItem) AndAlso _
            Me.Cmb_MateriePrime.SelectedValue <> "" AndAlso _
                Me.Cmb_MateriePrime.SelectedValue <> "0" Then

            AgronicaCoreUtility.CaricaListControl.LineeByMateriePrime_LogOmni( _
                                                    Me.cmb_LineeByMat_Cod, _
                                                    True, _
                                                    "Nessun filtro", _
                                                    "0", _
                                                    Qs_Piva, _
                                                    0, _
                                                     Me.Cmb_MateriePrime.SelectedValue, _
                                                    Me.Rbl_Report.SelectedValue, _
                                                    3, _
                                                    "", _
                                                    "", _
                                                    objParametri_Server)

        End If

    End Sub

    '##########################################################################################################################################
    Private Sub Rbl_Verifica_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Verifica.SelectedIndexChanged
        Cambia_FiltriVerifica()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_FiltriVerifica()

        Me.Pannello_VoceRiepilogo.Visible = False
        Me.Pannello_Linee.Visible = False

        Select Case Me.Rbl_Verifica.SelectedValue

            Case 0 'linea
                Me.Pannello_Linee.Visible = True

            Case 1 'voce riepilogo
                Me.Pannello_VoceRiepilogo.Visible = True
        End Select

    End Sub

    '##########################################################################################################################################
    Private Sub Rbl_MeseIntervallo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_MeseIntervallo.SelectedIndexChanged
        Cambia_MeseIntervallo()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_MeseIntervallo()

        Me.Pannello_Date.Visible = False
        Me.Pannello_AnnoMese.Visible = False
        Me.Cmb_Anno.Enabled = False
        Me.Cmb_Mese.Enabled = False
        Me.Txt_DataInizio.Enabled = False
        Me.Txt_DataFine.Enabled = False

        Select Case Me.Rbl_MeseIntervallo.SelectedValue

            Case 0 'mese
                Me.Pannello_AnnoMese.Visible = True

                Me.Txt_DataFine.Text = ""
                Me.Txt_DataInizio.Text = ""

                Me.Cmb_Anno.SelectedIndex = Me.Cmb_Anno.Items.IndexOf(Me.Cmb_Anno.Items.FindByValue(Date.Today.Year))
                Me.Cmb_Mese.SelectedIndex = Me.Cmb_Mese.Items.IndexOf(Me.Cmb_Mese.Items.FindByValue(Right("00" + CStr(Date.Today.Month), 2)))

                Me.Cmb_Anno.Enabled = True
                Me.Cmb_Mese.Enabled = True

            Case 1 'intervallo
                Me.Pannello_Date.Visible = True

                Me.Cmb_Anno.SelectedIndex = 0
                Me.Cmb_Mese.SelectedIndex = 0

                '---- Intervallo Temporale
                Me.Txt_DataInizio.Text = "01/" & Right("0" + CStr(Date.Today.Month), 2) & "/" & CStr(Date.Today.Year)
                Me.Txt_DataFine.Text = Right("0" + CStr(Date.DaysInMonth(Date.Today.Year, Date.Today.Month)), 2) & "/" & Right("0" + CStr(Date.Today.Month), 2) & "/" & CStr(Date.Today.Year)

                Me.Txt_DataInizio.Enabled = True
                Me.Txt_DataFine.Enabled = True

        End Select

    End Sub

    ''##########################################################################################################################################
    'Private Sub Cmb_Centri_Registri_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Centri_Registri.SelectedIndexChanged
    '    Cambia_Centro_Registri()
    'End Sub

    ''##########################################################################################################################################
    'Private Sub Cambia_Centro_Registri()

    '    If Not IsNothing(Me.Cmb_Centri_Registri.SelectedItem) AndAlso _
    '        Me.Cmb_Centri_Registri.SelectedValue <> "" AndAlso _
    '            Me.Cmb_Centri_Registri.SelectedValue <> "0" Then

    '        AgronicaCoreUtility.CaricaListControl.Magazzini_e_Vasche( _
    '                                            Me.Cmb_MagazzinoVasca, _
    '                                            True, _
    '                                            "Nessun filtro", _
    '                                            "0", _
    '                                            Qs_Piva, _
    '                                                Me.Cmb_Centri_Registri.SelectedValue, _
    '                                                0, _
    '                                                MAGAZZINO, _
    '                                                  Qs_Piva, _
    '                                                Me.Cmb_Centri_Registri.SelectedValue, _
    '                                                0, _
    '                                                False, _
    '                                                2, _
    '                                                "", _
    '                                                "", _
    '                                                "", _
    '                                                objParametri_Server)

    '    End If

    'End Sub

    '##########################################################################################################################################
    Private Sub Cmb_CentroAziendale_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Cmb_CentroAziendale.SelectedIndexChanged
        Cambia_Centro_ConsEno()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_Centro_ConsEno()

        Me.Cmb_Piano.Items.Clear()
        Me.Cmb_Vasca.Items.Clear()

        If Not IsNothing(Me.Cmb_CentroAziendale.SelectedItem) AndAlso _
            Me.Cmb_CentroAziendale.SelectedValue <> "" AndAlso _
                Me.Cmb_CentroAziendale.SelectedValue <> "0" Then

            Select Case Me.ddl_TipoOperazione.SelectedValue

                Case enum_TipoOperazione.StampaConsistenzeEnologiche

                    AgronicaCoreUtility.CaricaListControl.Cantina_Caratteristiche( _
                                             Me.Cmb_Piano, _
                                             True, _
                                             "Tutti i piani", _
                                             "0", _
                                             Qs_Piva, _
                                                 Me.Cmb_CentroAziendale.SelectedValue, _
                                                 0, _
                                                 AGRODATAINIZIO, _
                                                 AGRODATAFINE, _
                                                 "", _
                                                 "", _
                                                 objParametri_Server)

                    CaricaCombo_OrdinamentoConsEnologiche(False, "", "")


                Case enum_TipoOperazione.StampaVerificaRegistriCantina

                    AgronicaCoreUtility.CaricaListControl.Magazzini_e_Vasche( _
                                           Me.Cmb_MagazzinoVasca, _
                                           True, _
                                           "Nessun filtro", _
                                           "0", _
                                           Qs_Piva, _
                                               Me.Cmb_CentroAziendale.SelectedValue, _
                                               0, _
                                               MAGAZZINO, _
                                                 Qs_Piva, _
                                               Me.Cmb_CentroAziendale.SelectedValue, _
                                               0, _
                                               False, _
                                               2, _
                                               "", _
                                               "", _
                                               "", _
                                               objParametri_Server)


            End Select

        End If

    End Sub

    '##########################################################################################################################################
    Private Sub Cmb_Piano_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Piano.SelectedIndexChanged
        Cambia_Piano()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_Piano()

        Me.Cmb_Vasca.Items.Clear()

        If Not IsNothing(Me.Cmb_Piano.SelectedItem) AndAlso _
            Me.Cmb_Piano.SelectedValue <> "" AndAlso _
                Me.Cmb_Piano.SelectedValue <> "0" Then

            AgronicaCoreUtility.CaricaListControl.Cantina_Vasche( _
                                                Me.Cmb_Vasca, _
                                                True, _
                                                "Tutti le vasche", _
                                                "0", _
                                                Qs_Piva, _
                                                    Me.Cmb_CentroAziendale.SelectedValue, _
                                                    Me.Cmb_Piano.SelectedValue, _
                                                    0, _
                                                    "", _
                                                    "", _
                                                    objParametri_Server)

            CaricaCombo_OrdinamentoConsEnologiche(False, "", "")

        End If

    End Sub

    '##########################################################################################################################################
    Private Sub CaricaCombo_OrdinamentoConsEnologiche(ByVal PrimaRiga_Flag As Boolean, _
                                                        ByVal PrimaRiga_Text As String, _
                                                        ByVal PrimaRiga_Value As String)

        'Pulisco il controllo
        Me.Cmb_Ordinamento.Items.Clear()

        If PrimaRiga_Flag = True Then
            Me.Cmb_Ordinamento.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
            'Cmb_Ordinamento.Items.Add(New ListItem("", enum_OrdinamentoStampaConsEnologiche.Nessuno))
        End If

        ' Cmb_Ordinamento.Items.Add(New ListItem("Piano", enum_OrdinamentoStampaConsEnologiche.Piano))
        Cmb_Ordinamento.Items.Add(New ListItem("Identificativo Vasca", enum_OrdinamentoStampaConsEnologiche.IdentificativoVasca))
        Cmb_Ordinamento.Items.Add(New ListItem("Numero Serie", enum_OrdinamentoStampaConsEnologiche.NumeroSerie))
        Cmb_Ordinamento.Items.Add(New ListItem("Materiale", enum_OrdinamentoStampaConsEnologiche.Materiale))
        Cmb_Ordinamento.Items.Add(New ListItem("Prodotto", enum_OrdinamentoStampaConsEnologiche.Prodotto))
        Cmb_Ordinamento.Items.Add(New ListItem("Lotto", enum_OrdinamentoStampaConsEnologiche.Lotto))
        Cmb_Ordinamento.Items.Add(New ListItem("Linea Produttiva", enum_OrdinamentoStampaConsEnologiche.LineaProduttiva))

    End Sub


    '####################################################################################################################################
    'Private Sub ImgBtn_VerificaRegistri_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_VerificaRegistri.Click
    '    Select Case Me.Rbl_Report.SelectedValue
    '        Case enum_AgroReportistica.Vinificazione_DOC, _
    '                enum_AgroReportistica.Vinificazione_ViniTavola, _
    '                    enum_AgroReportistica.Commercializzazione
    '            Stampa_RegCantina(False, True)
    '        Case Else
    '            AgroMsgBox("Verifica abilitata per Registri Vinificazione/Commercializzazione", Page)
    '    End Select
    'End Sub




    '####################################################################################################################################
    Private Sub Stampa_RegCantina(ByVal SalvaEStampa As Boolean, _
                                    ByVal Flag_VerificaRegistri As Boolean, _
                                    ByVal Flag_BrogliaccioMovimenti As Boolean, _
                                    ByVal Flag_RiepilogoImbottigliamenti As Boolean)

        Dim TargetURL As String
        Dim QueryString As String
        Dim Titolo, Tipo_Stampa As String
        Dim DataInizio As Date = #1/1/1900#
        Dim DataFine As Date = #12/31/2100#
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim Anno, Mese As String
        Dim Cod_Contatto_Terzi As String = ""
        Dim Ragsoc_CTerzi As String = ""
        Dim Filtro_Stampa As String = ""
        Dim id_agenda, id_mov As Integer
        Dim Cau_Mov As String = ""
        Dim Gestione_Conto_Terzi As enum_RegistroContoTerzi = enum_RegistroContoTerzi.Nessuno

        Dim Flag_StampaNumeroVasca As Boolean = False
        Dim Flag_StampaCapacitaVasca As Boolean = False
        Dim OptGestVisualNumVascaRegImbott As Integer = 0
        Dim Flag_GestioneRegistroVinificazione As Integer = 0
        Dim Flag_StampaLottoTrasformazione As Boolean = False

        Dim flag_docg, flag_dop, flag_igt, flag_tavola As Boolean

        Dim Id_Destinazione As Integer = 0
        Dim Cal_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Linea_Cod As Integer = 0
        Dim Sa_cod As Integer = 0



        If Me.Rbl_ContoTerzi.SelectedValue = enum_RegistroContoTerzi.RegistroUnicoDiversificato And Me.Rbl_Report.SelectedValue <> enum_AgroReportistica.Vinificazione_DOC Then
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Questo tipo di stampa è gestita solo per il Registro di Vinificazione", Page, "MainContent", True)
            Exit Sub
        End If

        If Me.Rbl_ContoTerzi.SelectedValue = enum_RegistroContoTerzi.RegistroSoloContoLavoro And Me.Rbl_Report.SelectedValue <> enum_AgroReportistica.Imbottigliamento Then
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Questo tipo di stampa è gestita solo per il Registro di Imbottigliamento", Page, "MainContent", True)
            Exit Sub
        End If


        Try

            '----- Verifico i dati

            Sa_cod = Cmb_CentroAziendale.SelectedValue
            '2016/06/13 : 
            'L 'id_destinazione non rappresenta il centro aziendale , ma rappresenta il magazzino/vasca
            'Id_Destinazione = Sa_cod
            Tipo_Stampa = Me.Rbl_MeseIntervallo.SelectedValue

            Select Case Me.Rbl_MeseIntervallo.SelectedValue

                Case 0 'mese
                    str_DataInizio = ""
                    str_DataFine = ""

                    If Me.Cmb_Anno.SelectedValue <> "0" Then
                        Anno = Me.Cmb_Anno.SelectedValue
                    Else
                        'AgroMsgBox("E' stata selezionata la stampa per mese: " + _
                        '           "selezionare l'anno e il mese desiderato!", Page)

                        AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("E' stata selezionata la stampa per mese: selezionare l'anno e il mese desiderato!", Page, "MainContent", True)

                        Exit Sub
                    End If
                    If Me.Cmb_Mese.SelectedValue <> "00" Then
                        Mese = Me.Cmb_Mese.SelectedValue
                    Else
                        'AgroMsgBox("E' stata selezionata la stampa per mese: " + _
                        '           "selezionare l'anno e il mese desiderato!", Page)

                        AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("E' stata selezionata la stampa per mese: selezionare l'anno e il mese desiderato!", Page, "MainContent", True)

                        Exit Sub
                    End If

                    Filtro_Stampa += "Anno " & CStr(Anno) & " Mese " & CStr(Me.Cmb_Mese.SelectedItem.Text) & " - "

                Case 1 'intervallo

                    Anno = ""
                    Mese = ""

                    '----- Date di Stampa
                    If Me.Txt_DataInizio.Text <> "" Then
                        DataInizio = CDate(Me.Txt_DataInizio.Text)
                    Else
                        'AgroMsgBox("E' stata selezionata la stampa per intervallo temporale: selezionare la data di inizio e la data di fine!", Page)
                        AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("E' stata selezionata la stampa per intervallo temporale: selezionare la data di inizio e la data di fine!", Page, "MainContent", True)
                        Exit Sub
                    End If

                    If Me.Txt_DataFine.Text <> "" Then
                        DataFine = CDate(Me.Txt_DataFine.Text)
                    Else
                        'AgroMsgBox("E' stata selezionata la stampa per intervallo temporale: selezionare la data di inizio e la data di fine!", Page)
                        AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("E' stata selezionata la stampa per intervallo temporale: selezionare la data di inizio e la data di fine!", Page, "MainContent", True)
                        Exit Sub
                    End If

                    str_DataInizio = Format(DataInizio, "dd/MM/yyyy")
                    str_DataFine = Format(DataFine, "dd/MM/yyyy")

                    Filtro_Stampa += "Dal " & CStr(str_DataInizio) & " Al " & CStr(str_DataFine) & " - "

            End Select

            If ViewState("GestioneContoTerzi") = True Then

                Gestione_Conto_Terzi = Me.Rbl_ContoTerzi.SelectedValue

                Select Case Me.Rbl_ContoTerzi.SelectedValue

                    Case enum_RegistroContoTerzi.RegistroGlobale

                    Case enum_RegistroContoTerzi.RegistroUnicoDiversificato

                    Case enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                        Cod_Contatto_Terzi = Me.Cmb_ContattiContoTerzi.SelectedValue
                        Ragsoc_CTerzi = Me.Cmb_ContattiContoTerzi.SelectedItem.Text

                        If Cod_Contatto_Terzi = "" Then
                            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Per stampare il registro separato per c/terzi, occorre selezionare il c/terzi nel menù a tendina.", Page, "MainContent", True)
                            Exit Sub
                        End If

                End Select

            End If

            If Flag_BrogliaccioMovimenti = True Then
                Me.Rbl_Report.SelectedValue = 0

                If ViewState("GestioneContoTerzi") = True Then
                    Gestione_Conto_Terzi = enum_RegistroContoTerzi.RegistroUnicoDiversificato
                Else
                    Gestione_Conto_Terzi = enum_RegistroContoTerzi.Nessuno
                End If

                TargetURL = "../Cantine/BrogliaccioMovimenti/BrogliaccioMovimenti.aspx"
                Titolo = "BrogliaccioMovimenti"

                'Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                'objUtenti.LeggiOpzioni_RegistriCantina(2, _
                '                                        objParametri_Utenti, _
                '                                        Flag_StampaNumeroVasca, _
                '                                        Flag_StampaCapacitaVasca, _
                '                                        OptGestVisualNumVascaRegImbott, _
                '                                        Flag_GestioneRegistroVinificazione, _
                '                                        Flag_StampaLottoTrasformazione)
            End If

            If Flag_RiepilogoImbottigliamenti = True Then
                Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Imbottigliamento
            End If

            'sono nel caso reg. vinificazione (accettazioen da diversi), o reg. imbottigliamento
            If Me.Div_Categoria.Visible = True Then

                flag_docg = Me.CBL_Categoria.Items.Item(0).Selected
                flag_dop = Me.CBL_Categoria.Items.Item(1).Selected
                flag_igt = Me.CBL_Categoria.Items.Item(2).Selected
                flag_tavola = Me.CBL_Categoria.Items.Item(3).Selected

                If flag_docg = False And flag_dop = False And flag_igt = False And flag_tavola = False Then
                    AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("E' necessario selezionare almeno una categoria di vino ", Page, "MainContent", True)
                    Exit Sub
                End If
            Else
                'no accettazione/conferimento
                flag_docg = True
                flag_dop = True
                flag_igt = True
                flag_tavola = True
            End If


            '----- Preparo il link
            Select Case Me.Rbl_Report.SelectedValue
                Case enum_AgroReportistica.Vinificazione_DOC, enum_AgroReportistica.Vinificazione_ViniTavola
                    TargetURL = "RegistroVinificazione/RegistroVinificazione.aspx"
                    Titolo = "RegistroVinificazione"
                Case enum_AgroReportistica.Imbottigliamento
                    TargetURL = "RegistroImbottigliamento/RegistroImbottigliamento.aspx"
                    Titolo = "RegistroImbottigliamento"
                Case enum_AgroReportistica.Commercializzazione
                    TargetURL = "RegistroCommercializzazioneVini/RegistroCommercializzazioneVini.aspx"
                    Titolo = "RegistroCommercializzazioneVini"
                Case enum_AgroReportistica.Frizzanti
                    TargetURL = "RegistroViniFrizzanti/RegistroViniFrizzanti.aspx"
                    Titolo = "RegistroViniFrizzanti"
                Case enum_AgroReportistica.Spumanti
                    TargetURL = "RegistroViniSpumanti/RegistroViniSpumanti.aspx"
                    Titolo = "RegistroViniSpumanti"
                    'Case Else
                    '    AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Registro in manutenzione.", Page, "MainContent", True)
                    '    Exit Sub
            End Select

            'filtro x deposito
            'disattivato in data 01/07/2015 -> è da gestire meglio
            'If Not IsNothing(Me.Cmb_Centri_Registri.SelectedItem) AndAlso _
            '           Me.Cmb_Centri_Registri.SelectedValue <> "" AndAlso _
            '               Me.Cmb_Centri_Registri.SelectedValue <> "0" Then
            '    Sa_cod = Me.Cmb_Centri_Registri.SelectedValue
            '    Filtro_Stampa += Me.Cmb_Centri_Registri.SelectedItem.Text & " - "
            'End If

            '----------
            'verifica opzione stampa numero e capacità vasca
            Select Case Me.Rbl_Report.SelectedValue

                Case enum_AgroReportistica.Vinificazione_DOC, _
                    enum_AgroReportistica.Vinificazione_ViniTavola, _
                        enum_AgroReportistica.Commercializzazione, _
                         enum_AgroReportistica.Imbottigliamento

                    Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                    objUtenti.LeggiOpzioni_RegistriCantina(2, _
                                                            objParametri_Utenti, _
                                                            Flag_StampaNumeroVasca, _
                                                            Flag_StampaCapacitaVasca, _
                                                            OptGestVisualNumVascaRegImbott, _
                                                            Flag_GestioneRegistroVinificazione, _
                                                            Flag_StampaLottoTrasformazione)



            End Select
            '----------

            'solo x verifica
            If Flag_VerificaRegistri = True Then

                If Me.Rbl_CauMov.SelectedValue <> "0" And Me.Rbl_CauMov.SelectedValue <> "" Then
                    Cau_Mov = Me.Rbl_CauMov.SelectedValue
                End If

                Select Case Me.Rbl_Verifica.SelectedIndex

                    Case 0 'LINEA

                        'linea
                        If Not IsNothing(Me.Cmb_Linee.SelectedItem) AndAlso _
                             Me.Cmb_Linee.SelectedValue <> "" AndAlso _
                                 Me.Cmb_Linee.SelectedValue <> "0" Then
                            Linea_Cod = Me.Cmb_Linee.SelectedValue
                            Filtro_Stampa += "Linea: " & Me.Cmb_Linee.SelectedItem.Text
                        End If

                        'prodotto
                        If Not IsNothing(Me.Cmb_MateriePrimeByLinee.SelectedItem) AndAlso _
                             Me.Cmb_MateriePrimeByLinee.SelectedValue <> "" AndAlso _
                                 Me.Cmb_MateriePrimeByLinee.SelectedValue <> "0" Then
                            Mat_Cod = Me.Cmb_MateriePrimeByLinee.SelectedValue
                            Filtro_Stampa += "Prodotto: " & Me.Cmb_MateriePrimeByLinee.SelectedItem.Text
                        End If

                        '====================================================

                    Case 1 'VOCE DI RIEPILOGO

                        'voce riepilogo
                        If Not IsNothing(Me.Cmb_VociRiepilogo.SelectedItem) AndAlso _
                             Me.Cmb_VociRiepilogo.SelectedValue <> "" AndAlso _
                                 Me.Cmb_VociRiepilogo.SelectedValue <> "0" Then
                            Cal_Cod = Me.Cmb_VociRiepilogo.SelectedValue
                            Filtro_Stampa += "Voce di riepilogo: " & Me.Cmb_VociRiepilogo.SelectedItem.Text & " - "
                        End If

                        'prodotto
                        If Not IsNothing(Me.Cmb_MateriePrime.SelectedItem) AndAlso _
                             Me.Cmb_MateriePrime.SelectedValue <> "" AndAlso _
                                 Me.Cmb_MateriePrime.SelectedValue <> "0" Then
                            Mat_Cod = Me.Cmb_MateriePrime.SelectedValue
                            Filtro_Stampa += "Prodotto: " & Me.Cmb_MateriePrime.SelectedItem.Text
                        End If

                        'linea
                        If Not IsNothing(Me.cmb_LineeByMat_Cod.SelectedItem) AndAlso _
                             Me.cmb_LineeByMat_Cod.SelectedValue <> "" AndAlso _
                                 Me.cmb_LineeByMat_Cod.SelectedValue <> "0" Then
                            Linea_Cod = Me.cmb_LineeByMat_Cod.SelectedValue
                            Filtro_Stampa += "Linea: " & Me.cmb_LineeByMat_Cod.SelectedItem.Text
                        End If

                End Select

                'magazzino/vasca
                If Not IsNothing(Me.Cmb_MagazzinoVasca.SelectedItem) AndAlso _
                     Me.Cmb_MagazzinoVasca.SelectedValue <> "" AndAlso _
                         Me.Cmb_MagazzinoVasca.SelectedValue <> "0" Then
                    Id_Destinazione = Me.Cmb_MagazzinoVasca.SelectedValue
                    Filtro_Stampa += "Mag/vasca: " & Me.Cmb_MagazzinoVasca.SelectedItem.Text & " - "
                End If

                Select Case Cau_Mov
                    Case CAU_CARICO
                        Filtro_Stampa += "solo movimenti di CARICO (tranne il saldo precedente)"
                    Case CAU_SCARICO
                        Filtro_Stampa += "solo movimenti di SCARICO (tranne il saldo precedente)"
                End Select

            End If

            'PARTITA
            id_agenda = 0
            id_mov = 0
            If Me.Pannello_Partita.Visible = True Then
                ' nei registri frizzanti/Spumanti le date sono inutili
                ' mi servono l'id_agenda e l'id_mov
                If Not IsNothing(Me.Cmb_Partita.SelectedItem) AndAlso _
                    Me.Cmb_Partita.SelectedValue <> "" AndAlso _
                        Me.Cmb_Partita.SelectedValue <> "0" Then

                    Dim vet() As String
                    vet = Me.Cmb_Partita.SelectedValue.Split("|")
                    id_agenda = vet(0)
                    id_mov = vet(1)
                Else
                    'AgroMsgBox("Per avviare la stampa è necessario selezionare la partita.", Page)
                    AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Per avviare la stampa è necessario selezionare la partita.", Page, "MainContent", True)
                    Exit Sub
                End If
            End If


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Stampa_RegCantina: " & ex.Message, Page, "MainContent", True)
        End Try


        If TargetURL <> String.Empty Then

            ' parametri comuni a tutti i registri
            'sostituito qs_sa_cod con sa_cod
            QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                          "&s=" & Stringa_Codifica(Sa_cod, AgroKey_EncoderDecoder, Server) & _
                          "&sn=" & Stringa_Codifica(Cmb_CentroAziendale.SelectedItem.Text, AgroKey_EncoderDecoder, Server) & _
                          "&rp=" & Stringa_Codifica(CStr(Rbl_Report.SelectedValue), AgroKey_EncoderDecoder, Server) & _
                          "&si=" & Stringa_Codifica((IIf(Me.Chk_StampaIntestazione.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
                          "&rs=" & Stringa_Codifica(QS_SaveText(CStr(Me.Txt_RagSoc.Text)), AgroKey_EncoderDecoder, Server) + _
                          "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server) & _
                          "&cache=" & CStr(SalvaEStampa)


            If Flag_BrogliaccioMovimenti = True Then
                QueryString &= "&di=" & Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) & _
                                  "&df=" & Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) & _
                                  "&t=" & Stringa_Codifica(Tipo_Stampa, AgroKey_EncoderDecoder, Server) & _
                                  "&a=" & Stringa_Codifica(Anno, AgroKey_EncoderDecoder, Server) & _
                                  "&m=" & Stringa_Codifica(Mese, AgroKey_EncoderDecoder, Server) & _
                                  "&sr=" & Stringa_Codifica((IIf(Me.Chk_StampaRiporti.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
                                  "&ct=" & Stringa_Codifica(Cod_Contatto_Terzi, AgroKey_EncoderDecoder, Server) & _
                                  "&ctd=" & Stringa_Codifica(Ragsoc_CTerzi, AgroKey_EncoderDecoder, Server) & _
                                  "&dest=" & Stringa_Codifica(Id_Destinazione, AgroKey_EncoderDecoder, Server) & _
                                  "&cc=" & Stringa_Codifica(Cal_Cod, AgroKey_EncoderDecoder, Server) & _
                                  "&mc=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) & _
                                  "&lc=" & Stringa_Codifica(Linea_Cod, AgroKey_EncoderDecoder, Server) & _
                                  "&fs=" & Stringa_Codifica(Filtro_Stampa, AgroKey_EncoderDecoder, Server) & _
                                  "&fvr=" & Stringa_Codifica(Flag_VerificaRegistri, AgroKey_EncoderDecoder, Server) & _
                                  "&fnv=" & Stringa_Codifica(Flag_StampaNumeroVasca, AgroKey_EncoderDecoder, Server) & _
                                  "&fcv=" & Stringa_Codifica(Flag_StampaCapacitaVasca, AgroKey_EncoderDecoder, Server) & _
                                  "&rv=" & Stringa_Codifica(Flag_GestioneRegistroVinificazione, AgroKey_EncoderDecoder, Server) & _
                                  "&cm=" & Stringa_Codifica(Cau_Mov, AgroKey_EncoderDecoder, Server) & _
                                  "&fct=" & Stringa_Codifica(Gestione_Conto_Terzi, AgroKey_EncoderDecoder, Server) & _
                                  "&flt=" & Stringa_Codifica(Flag_StampaLottoTrasformazione, AgroKey_EncoderDecoder, Server) & _
                                  "&fdocg=" & Stringa_Codifica(flag_docg, AgroKey_EncoderDecoder, Server) & _
                                  "&fdop=" & Stringa_Codifica(flag_dop, AgroKey_EncoderDecoder, Server) & _
                                  "&figt=" & Stringa_Codifica(flag_igt, AgroKey_EncoderDecoder, Server) & _
                                  "&ftav=" & Stringa_Codifica(flag_tavola, AgroKey_EncoderDecoder, Server)

            End If


            Select Case Me.Rbl_Report.SelectedValue

                Case enum_AgroReportistica.Frizzanti, _
                        enum_AgroReportistica.Spumanti

                    QueryString &= "&id_agenda=" & Stringa_Codifica(id_agenda, AgroKey_EncoderDecoder, Server) & _
                                   "&id_mov=" & Stringa_Codifica(id_mov, AgroKey_EncoderDecoder, Server) & _
                                   "&partita=" & Stringa_Codifica(Cmb_Partita.SelectedItem.Text, AgroKey_EncoderDecoder, Server)

                    '------------------------------------------------------------------
                Case enum_AgroReportistica.Vinificazione_DOC, _
                        enum_AgroReportistica.Vinificazione_ViniTavola, _
                            enum_AgroReportistica.Commercializzazione

                    'in data 17/09/2012 abilitato il filtro sul contatto x conto terzi anche per reg commercializzazione
                    'in data 08/04/2013 aggiunti filtri x verifica registri
                    'in data 03/10/13 aggiunto filtro cau_mov
                    QueryString &= "&di=" & Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) & _
                                    "&df=" & Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) & _
                                    "&t=" & Stringa_Codifica(Tipo_Stampa, AgroKey_EncoderDecoder, Server) & _
                                    "&a=" & Stringa_Codifica(Anno, AgroKey_EncoderDecoder, Server) & _
                                    "&m=" & Stringa_Codifica(Mese, AgroKey_EncoderDecoder, Server) & _
                                    "&sr=" & Stringa_Codifica((IIf(Me.Chk_StampaRiporti.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
                                    "&ct=" & Stringa_Codifica(Cod_Contatto_Terzi, AgroKey_EncoderDecoder, Server) & _
                                    "&ctd=" & Stringa_Codifica(Ragsoc_CTerzi, AgroKey_EncoderDecoder, Server) & _
                                    "&dest=" & Stringa_Codifica(Id_Destinazione, AgroKey_EncoderDecoder, Server) & _
                                    "&cc=" & Stringa_Codifica(Cal_Cod, AgroKey_EncoderDecoder, Server) & _
                                    "&mc=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) & _
                                    "&lc=" & Stringa_Codifica(Linea_Cod, AgroKey_EncoderDecoder, Server) & _
                                    "&fs=" & Stringa_Codifica(Filtro_Stampa, AgroKey_EncoderDecoder, Server) & _
                                    "&fvr=" & Stringa_Codifica(Flag_VerificaRegistri, AgroKey_EncoderDecoder, Server) & _
                                    "&fnv=" & Stringa_Codifica(Flag_StampaNumeroVasca, AgroKey_EncoderDecoder, Server) & _
                                    "&fcv=" & Stringa_Codifica(Flag_StampaCapacitaVasca, AgroKey_EncoderDecoder, Server) & _
                                    "&rv=" & Stringa_Codifica(Flag_GestioneRegistroVinificazione, AgroKey_EncoderDecoder, Server) & _
                                    "&cm=" & Stringa_Codifica(Cau_Mov, AgroKey_EncoderDecoder, Server) & _
                                    "&fct=" & Stringa_Codifica(Gestione_Conto_Terzi, AgroKey_EncoderDecoder, Server) & _
                                    "&flt=" & Stringa_Codifica(Flag_StampaLottoTrasformazione, AgroKey_EncoderDecoder, Server) & _
                                    "&fdocg=" & Stringa_Codifica(flag_docg, AgroKey_EncoderDecoder, Server) & _
                                    "&fdop=" & Stringa_Codifica(flag_dop, AgroKey_EncoderDecoder, Server) & _
                                    "&figt=" & Stringa_Codifica(flag_igt, AgroKey_EncoderDecoder, Server) & _
                                    "&ftav=" & Stringa_Codifica(flag_tavola, AgroKey_EncoderDecoder, Server)

                    '------------------------------------------------------------------

                Case enum_AgroReportistica.Imbottigliamento

                    QueryString &= "&di=" & Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) & _
                                   "&df=" & Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) & _
                                   "&t=" & Stringa_Codifica(Tipo_Stampa, AgroKey_EncoderDecoder, Server) & _
                                   "&a=" & Stringa_Codifica(Anno, AgroKey_EncoderDecoder, Server) & _
                                   "&m=" & Stringa_Codifica(Mese, AgroKey_EncoderDecoder, Server) & _
                                   "&sr=" & Stringa_Codifica((IIf(Me.Chk_StampaRiporti.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
                                   "&fnv=" & Stringa_Codifica(Flag_StampaNumeroVasca, AgroKey_EncoderDecoder, Server) & _
                                   "&fcv=" & Stringa_Codifica(Flag_StampaCapacitaVasca, AgroKey_EncoderDecoder, Server) & _
                                   "&onv=" & Stringa_Codifica(OptGestVisualNumVascaRegImbott, AgroKey_EncoderDecoder, Server) & _
                                 "&fdocg=" & Stringa_Codifica(flag_docg, AgroKey_EncoderDecoder, Server) & _
                                 "&fdop=" & Stringa_Codifica(flag_dop, AgroKey_EncoderDecoder, Server) & _
                                 "&figt=" & Stringa_Codifica(flag_igt, AgroKey_EncoderDecoder, Server) & _
                                 "&ftav=" & Stringa_Codifica(flag_tavola, AgroKey_EncoderDecoder, Server) & _
                                   "&fct=" & Stringa_Codifica(Gestione_Conto_Terzi, AgroKey_EncoderDecoder, Server) & _
                                   "&ct=" & Stringa_Codifica(Cod_Contatto_Terzi, AgroKey_EncoderDecoder, Server) & _
                                   "&ctd=" & Stringa_Codifica(Ragsoc_CTerzi, AgroKey_EncoderDecoder, Server)


                    '------------------------------------------------------------------


            End Select


            Page_NewWindow_2010(Page, _
                        TargetURL, QueryString, , , , , , , , , "MainContent", True)

        End If


    End Sub

    '######################################################################################################################################################################
    'Private Sub ImgBtn_StampaConsistenze_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaConsistenze.Click

    '    Stampa_ConsistenzeEnologiche()

    'End Sub

    '######################################################################################################################################################################
    Private Sub Stampa_ConsistenzeEnologiche()

        Dim str_Data As String
        Dim TargetURL As String
        Dim QueryString As String
        Dim Sa_Cod, Piano_Cod, Vas_Cod, Linea_Cod As Integer
        Dim Linea_Des As String = ""
        Dim Ordinamento As enum_OrdinamentoStampaConsEnologiche
        Dim Titolo As String = "ConsistenzeEnologiche"

        '----- Date di Stampa
        str_Data = Me.Txt_DataConsistenze.Text

        If Not IsNothing(Me.Cmb_CentroAziendale.SelectedItem) AndAlso _
            Me.Cmb_CentroAziendale.SelectedValue <> "" AndAlso _
                Me.Cmb_CentroAziendale.SelectedValue <> "0" Then

            Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue
        Else
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Selezionare un centro aziendale!", Page, "MainContent", True)
            Exit Sub
        End If

        If Not IsNothing(Me.Cmb_LineeConsistenze.SelectedItem) AndAlso _
            Me.Cmb_LineeConsistenze.SelectedValue <> "" AndAlso _
                 Me.Cmb_LineeConsistenze.SelectedValue <> "0" Then
            Linea_Cod = Me.Cmb_LineeConsistenze.SelectedValue
            Linea_Des = Me.Cmb_LineeConsistenze.SelectedItem.Text
        End If

        If Me.Cmb_Piano.SelectedValue <> "" Then
            Piano_Cod = Me.Cmb_Piano.SelectedItem.Value
        End If

        If Me.Cmb_Vasca.SelectedValue <> "" Then
            Vas_Cod = Me.Cmb_Vasca.SelectedItem.Value
        End If

        If Me.Cmb_Ordinamento.SelectedValue <> "" Then
            Ordinamento = Me.Cmb_Ordinamento.SelectedItem.Value
        End If

        Dim TipoLikeLotto As Integer = 0
        If Me.Txt_Lotto.Text <> "" Then
            TipoLikeLotto = Me.rbl_Lotto.SelectedValue
        End If


        '----- Preparo il link
        TargetURL = "../Cantine/ConsistenzeEnologiche/ConsistenzeEnologiche.aspx"

        If TargetURL <> String.Empty Then

            QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                            "&d=" & Stringa_Codifica(str_Data, AgroKey_EncoderDecoder, Server) & _
                            "&s=" & Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) + _
                            "&pc=" & Stringa_Codifica(Piano_Cod, AgroKey_EncoderDecoder, Server) + _
                            "&vc=" & Stringa_Codifica(Vas_Cod, AgroKey_EncoderDecoder, Server) + _
                            "&lc=" & Stringa_Codifica(Linea_Cod, AgroKey_EncoderDecoder, Server) + _
                            "&ld=" & Stringa_Codifica(Linea_Des, AgroKey_EncoderDecoder, Server) + _
                            "&cc=" & Stringa_Codifica(Me.Cmb_Categoria.SelectedValue, AgroKey_EncoderDecoder, Server) + _
                            "&cd=" & Stringa_Codifica(Me.Cmb_Categoria.SelectedItem.Text, AgroKey_EncoderDecoder, Server) + _
                            "&sc=" & Stringa_Codifica(Me.Cmb_Semilavorati.SelectedValue, AgroKey_EncoderDecoder, Server) + _
                            "&sd=" & Stringa_Codifica(Me.Cmb_Semilavorati.SelectedItem.Text, AgroKey_EncoderDecoder, Server) + _
                            "&ord=" & Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server) + _
                            "&fvm=" & Stringa_Codifica(Me.Chk_VascheNoMov.Checked, AgroKey_EncoderDecoder, Server) + _
                            "&fqz=" & Stringa_Codifica(Me.Chk_ConsEnoZero.Checked, AgroKey_EncoderDecoder, Server) + _
                            "&fsr=" & Stringa_Codifica(Me.Chk_StampaRiepilogo.Checked, AgroKey_EncoderDecoder, Server) + _
                            "&cnc=" & Stringa_Codifica(TipoLikeLotto, AgroKey_EncoderDecoder, Server) + _
                            "&lot=" & Stringa_Codifica(Me.Txt_Lotto.Text, AgroKey_EncoderDecoder, Server) + _
                            "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            'Response.Redirect(TargetURL)


            Page_NewWindow_2010(Page, _
                        TargetURL, QueryString, , , , , , , , , "MainContent", True)


        End If


    End Sub

    '########################################################################################################################################
    'Private Sub ImgBtn_StampaRegistroVuoto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaRegistroVuoto.Click

    '    StampaRegistroVuoto()

    'End Sub


    '########################################################################################################################################
    Private Sub Stampa_RegistroVuoto()

        Dim TargetURL As String
        Dim QueryString As String
        Dim Titolo As String = "RegistroVuoto"


        '----- Verifico i dati

        If Not IsNumeric(Me.Txt_NumPagineDa.Text) Then
            'AgroMsgBox("Specificare la prima pagina!", Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Specificare la prima pagina!", Page, "MainContent", True)
            Exit Sub
        End If

        If Not IsNumeric(Me.Txt_NumPagineA.Text) Then
            'AgroMsgBox("Specificare l'ultima pagina!", Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Specificare l'ultima pagina!", Page, "MainContent", True)
            Exit Sub
        End If

        If CInt(Me.Txt_NumPagineA.Text) < CInt(Me.Txt_NumPagineDa.Text) Then
            'AgroMsgBox("L'intervallo delle pagine non è corretto", Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("L'intervallo delle pagine non è corretto", Page, "MainContent", True)
            Exit Sub
        End If

        If CInt(Me.Txt_NumPagineA.Text) > 200 Then
            'AgroMsgBox("I registri di cantina digitali devono avere al massimo 200 pagine.", Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("I registri di cantina digitali devono avere al massimo 200 pagine.", Page, "MainContent", True)
            Exit Sub
        End If

        If IsNumeric(Me.Txt_NumPagine.Text) Then
            If CInt(Me.Txt_NumPagine.Text) > 200 Then
                'AgroMsgBox("I registri di cantina digitali devono avere al massimo 200 pagine.", Page)
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("I registri di cantina digitali devono avere al massimo 200 pagine.", Page, "MainContent", True)
                Exit Sub
            End If
        End If

        TargetURL = "RegistroVuoto/RegistroVuoto.aspx"

        If TargetURL <> String.Empty Then

            QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                            "&rp=" & Stringa_Codifica(CStr(Rbl_Report.SelectedValue), AgroKey_EncoderDecoder, Server) & _
                            "&np=" & Stringa_Codifica(CStr(Me.Txt_NumPagine.Text), AgroKey_EncoderDecoder, Server) & _
                            "&npda=" & Stringa_Codifica(CStr(CInt(Me.Txt_NumPagineDa.Text)), AgroKey_EncoderDecoder, Server) & _
                            "&npa=" & Stringa_Codifica(CStr(CInt(Me.Txt_NumPagineA.Text)), AgroKey_EncoderDecoder, Server) & _
                            "&si=" & Stringa_Codifica((IIf(Me.Chk_StampaIntestazione.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
                            "&pr=" & Stringa_Codifica(CStr(Me.Txt_ProgrSigla_Registro.Text), AgroKey_EncoderDecoder, Server) & _
                            "&rs=" & Stringa_Codifica(QS_SaveText(CStr(Me.Txt_RagSoc.Text)), AgroKey_EncoderDecoder, Server) + _
                         "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            'Response.Redirect(TargetURL)


            Page_NewWindow_2010(Page, _
                        TargetURL, QueryString, Titolo, , , , , , , , "MainContent", True)




        End If

    End Sub


    '##############################################################################################################################################################
    'Private Sub ImgBtn_StampaFrontespizio_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaFrontespizio.Click
    '    Stampa_CopertinaRegistri()
    'End Sub


    '##############################################################################################################################################################
    Private Sub Stampa_CopertinaRegistri()

        Dim Cod_Indirizzo As Integer = 0
        Dim Titolo As String = "Copertina"

        If Not IsNumeric(Me.Txt_Copertina_TotPagine.Text) Then
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Occorre specificare il numero totale di pagine del registro.", Page, "MainContent", True)
            Exit Sub
        Else
            If CInt(Me.Txt_Copertina_TotPagine.Text) > 200 Then
               AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("I registri di cantina digitali devono avere al massimo 200 pagine.", Page, "MainContent", True)
                Exit Sub
            End If
        End If

        If Not IsNothing(Me.Cmb_Indirizzo.SelectedItem) AndAlso _
             Me.Cmb_Indirizzo.SelectedValue <> "" AndAlso _
                 Me.Cmb_Indirizzo.SelectedValue <> "0" Then

            Cod_Indirizzo = Me.Cmb_Indirizzo.SelectedValue
        Else
            'AgroMsgBox("Occorre selezionare l'indirizzo della sede della cantina che si desidera stampare sulla copertina.", Page)
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Occorre selezionare l'indirizzo della sede della cantina che si desidera stampare sulla copertina.", Page, "MainContent", True)
            Exit Sub
        End If

        Dim TargetURL As String
        Dim QueryString As String

        '----- Verifico i dati

        TargetURL = "CopertinaRegistri/CopertinaRegistri.aspx"

        If TargetURL <> String.Empty Then

            QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                            "&rp=" & Stringa_Codifica(CStr(Rbl_Report.SelectedValue), AgroKey_EncoderDecoder, Server) & _
                            "&pr=" & Stringa_Codifica(CStr(Me.Txt_Frontespizio.Text), AgroKey_EncoderDecoder, Server) & _
                            "&np=" & Stringa_Codifica(CStr(Me.Txt_Copertina_TotPagine.Text), AgroKey_EncoderDecoder, Server) & _
                            "&pp=" & Stringa_Codifica((IIf(Me.Chk_Pagina1.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
                            "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server) & _
                            "&chkc=" & Stringa_Codifica(Me.Chk_SaNome.Checked, AgroKey_EncoderDecoder, Server) & _
                            "&ci=" & Stringa_Codifica(Cod_Indirizzo, AgroKey_EncoderDecoder, Server)

            'Response.Redirect(TargetURL)


            Page_NewWindow_2010(Page, _
                        TargetURL, QueryString, , , , , , , , , "MainContent", True)

        End If

    End Sub

    '04/02/2016: DAA/DOCO doc obsoleti quindi non più gestiti
    ''##############################################################################################################################################################
    'Private Sub ImgBtn_StampaDAA_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaDAA.Click

    '    StampaDAA()

    'End Sub

    ''##############################################################################################################################################################
    'Private Sub StampaDAA()

    '    Dim TargetURL As String
    '    Dim QueryString As String
    '    Dim Titolo As String = "DAA"

    '    '----- Verifico i dati

    '    If Me.Rbl_DAA_Pagina.SelectedValue = 1 Then
    '        TargetURL = "DAA/DAA_Avanti.aspx"
    '    Else
    '        TargetURL = "DAA/DAA_Retro.aspx"
    '    End If

    '    If TargetURL <> String.Empty Then

    '        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
    '                    "&i=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) & _
    '                    "&l=" & Stringa_Codifica(CStr(LAVCOD_DAA_EMESSO), AgroKey_EncoderDecoder, Server) & _
    '                    "&e=" & Stringa_Codifica(CStr(Rbl_DAA_Esemplare.SelectedValue), AgroKey_EncoderDecoder, Server) + _
    '                     "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)


    '        'Response.Redirect(TargetURL + QueryString)

    '        Page_NewWindow_2010(Page, _
    '                     TargetURL, QueryString, , , , , , , , , "MainContent", True)

    '    End If


    'End Sub


    '##############################################################################################################################################################
    'Private Sub ImgBtn_StampaDOCO_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaDOCO.Click

    '    StampaDOCO()

    'End Sub


    ''##############################################################################################################################################################
    'Private Sub StampaDOCO()

    '    Dim TargetURL As String
    '    Dim QueryString As String
    '    Dim Titolo As String = "DOCO"

    '    '----- Verifico i dati

    '    TargetURL = "DOCO/DOCO.aspx"

    '    If TargetURL <> String.Empty Then

    '        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
    '                     "&i=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) & _
    '                     "&l=" & Stringa_Codifica(CStr(LAVCOD_DOCO_EMESSO), AgroKey_EncoderDecoder, Server) & _
    '                    "&ol=" & Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) & _
    '                    "&si=" & Stringa_Codifica((IIf(Me.Chk_StampaIntestazione_DOCO_Vuoto.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) + _
    '                     "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

    '        'Response.Redirect(TargetURL+QueryString)

    '        Page_NewWindow_2010(Page, _
    '                    TargetURL, QueryString, , , , , , , , , "MainContent", True)


    '    End If

    'End Sub


    '##############################################################################################################################################################
    'Private Sub ImgBtn_StampaSceltaLayout_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaSceltaLayout.Click

    '    StampaDOCOSceltaLayout()

    'End Sub


    ''##############################################################################################################################################################
    'Private Sub StampaDOCOSceltaLayout()

    '    Dim TargetURL As String
    '    Dim QueryString As String
    '    Dim Titolo As String = "DOCO"

    '    TargetURL = "DOCO/DOCO.aspx"

    '    QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
    '                    "&i=" & Stringa_Codifica(Qs_Id_Agenda, AgroKey_EncoderDecoder, Server) & _
    '                    "&l=" & Stringa_Codifica(Qs_Lav_Cod, AgroKey_EncoderDecoder, Server) & _
    '                    "&ol=" & Stringa_Codifica(Me.Rbl_Layout.SelectedValue, AgroKey_EncoderDecoder, Server) & _
    '                    "&si=" & Stringa_Codifica((IIf(Me.Chk_StampaIntestazione_DOCO_Layout.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) + _
    '                     "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

    '    '"&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) & _
    '    '"&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server)

    '    Page_NewWindow_2010(Page, _
    '                    TargetURL, QueryString, , , , , , , , , "MainContent", True)


    'End Sub


    '###########################################################################################################################################################
    'Private Sub ImgBtn_TelematizzazioneAccise_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_TelematizzazioneAccise.Click

    '    Dim TargetURL As String
    '    Dim QueryString As String
    '    Dim Titolo As String = "Accise"

    '    TargetURL = "../../GestioneEsportazioni/Telematizzazione_Accise/Filtro_Accise.aspx"

    '    QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

    '    Page_NewWindow_2010(Page, _
    '                    TargetURL, QueryString, , , , , , , , , "MainContent", True)

    'End Sub


    '######################################################
    Private Sub btnSalvaCacheRegistro_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnSalvaCacheRegistro.Click
        Stampa_RegCantina(True, False, False, False)
    End Sub


    '######################################################
    Private Sub btnApriCache_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnApriCache.Click

        Dim TargetURL As String = ""
        Dim QueryString As String = ""
        Dim EncoderManager As String = ""
        Dim vManager As String()
        Dim Titolo As String = "Registri"

        Try
            TargetURL = ConfigurationSettings.AppSettings("percorsoManager")
            EncoderManager = ConfigurationSettings.AppSettings("EncoderManager")
            vManager = EncoderManager.Split("|")
            QueryString = "?u=" & Stringa_Codifica(vManager(1), vManager(0), Server) & _
            "&p=" & Stringa_Codifica(vManager(2), vManager(0), Server) & _
            "&cmd=goto%20P10000000\\P18000300\\M21000030&winApp=1"

            Page_NewWindow_2010(Page, _
                        TargetURL, QueryString, , , , , , , , , "MainContent", True)

        Catch ex As Exception

        End Try


    End Sub


    '#########################################################################################################################################################
    Private Sub Cmb_LineaProduzione_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Cmb_LineaProduzione.SelectedIndexChanged

        CaricaPartita()

    End Sub

    '#########################################################################################################################################################
    Private Sub CaricaPartita()

        If Me.Cmb_LineaProduzione.SelectedIndex <> -1 Then

            ' il value di Cmb_Partita è Id_Agenda|Id_Mov|Id_Report (che è l'operazioni di inizio frizzantatura/spumantizzazione)
            AgronicaCoreUtility.CaricaListControl.Partite_LineeProduzioni(Cmb_Partita, _
                                                                          False, "", "", _
                                                                          Qs_Piva, _
                                                                          Cmb_LineaProduzione.SelectedValue, _
                                                                          Rbl_Report.SelectedValue, _
                                                                          True, _
                                                                          "", "", _
                                                                          objParametri_Server)
        End If

    End Sub


    '####################################################################################################################################
    Private Sub ddl_TipoOperazione_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddl_TipoOperazione.SelectedIndexChanged
        Configura_Layout()
    End Sub

    Private Sub Configura_Layout()

        Me.Div_SelRegistri.Visible = False '0
        Me.Div_Periodo.Visible = False '1
        Me.Div_Categoria.Visible = False '2
        Me.Div_ContoTerzi.Visible = False '3
        Me.Div_Intestazione.Visible = False '4
        Me.Div_CentroAziendale.Visible = False '5
        Me.Div_VerificaRegistri.Visible = False '6
        Me.Div_CopertinaRegistro.Visible = False '7
        Me.Div_RegistroVuoto.Visible = False '8
        Me.Div_ConsistenzeEnologiche.Visible = False '9
        Me.Div_Etichette.Visible = False '100
        Me.Pannello_Date.Visible = False
        Me.Pannello_AnnoMese.Visible = False

        Me.Chk_StampaIntestazione.Checked = True

        If Qs_Report <> enum_CodificaStampe.EtichetteVascheEnologiche Then

            Select Case Me.ddl_TipoOperazione.SelectedValue

                Case enum_TipoOperazione.StampaConsistenzeEnologiche

                    Me.Div_SelRegistri.Visible = False '0
                    Me.Div_Periodo.Visible = False '1
                    Me.Div_Categoria.Visible = False '2
                    Me.Div_ContoTerzi.Visible = False '3
                    Me.Div_Intestazione.Visible = False '4
                    Me.Div_CentroAziendale.Visible = True '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = True '9

                    'selezionare il primo centro aziendale
                    Me.Cmb_CentroAziendale.SelectedIndex = 1
                    Cambia_Centro_ConsEno()


                Case enum_TipoOperazione.BrogliaccioMovimentiCompleto

                    Me.Div_SelRegistri.Visible = False '0
                    Me.Div_Periodo.Visible = False '1
                    Me.Div_Categoria.Visible = False '2
                    Me.Div_Intestazione.Visible = False '4
                    Me.Chk_StampaIntestazione.Checked = False 'default
                    Me.Div_CentroAziendale.Visible = False '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case enum_TipoOperazione.BrogliaccioMovimentiPdfSemplificato

                    Me.Div_SelRegistri.Visible = False '0
                    Me.Div_Periodo.Visible = True '1
                    Cambia_MeseIntervallo()
                    'If (ViewState("Chk_AccettazioneDaDiversi") = "1" And Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Vinificazione_DOC) Or _
                    '    Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Imbottigliamento Then
                    '    Me.Div_Categoria.Visible = True '2
                    'End If
                    Me.Div_Categoria.Visible = False '2

                    'Select Case Me.Rbl_Report.SelectedValue
                    '    Case enum_AgroReportistica.Commercializzazione, _
                    '        enum_AgroReportistica.Vinificazione_DOC, _
                    '        enum_AgroReportistica.Imbottigliamento
                    '        If ViewState("GestioneContoTerzi") = True Then
                    '            Me.Div_ContoTerzi.Visible = True '3
                    '        End If
                    '        Me.Div_CentroAziendale.Visible = True '5
                    '    Case Else
                    '        Me.Div_CentroAziendale.Visible = False '5
                    'End Select

                    Me.Div_Intestazione.Visible = False '4
                    Me.Chk_StampaIntestazione.Checked = False 'default
                    Me.Div_CentroAziendale.Visible = False '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case enum_TipoOperazione.RiepilogoImbottigliamenti

                    'non selezionare il centro aziendale
                    Me.Cmb_CentroAziendale.SelectedIndex = 0

                    Me.Div_Categoria.Visible = True
                    Me.Div_ContoTerzi.Visible = ViewState("GestioneContoTerzi")

                    Me.Div_SelRegistri.Visible = False '0

                    Me.Div_Periodo.Visible = True '1
                    Cambia_MeseIntervallo()

                    Me.Div_Intestazione.Visible = False '4
                    Me.Chk_StampaIntestazione.Checked = True 'default
                    Me.Div_CentroAziendale.Visible = True '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case enum_TipoOperazione.RegistroCommercializzazioneOBSOLETO

                    'non selezionare il centro aziendale
                    Me.Cmb_CentroAziendale.SelectedIndex = 0
                    Me.Div_CentroAziendale.Visible = True '5

                    Me.Div_Categoria.Visible = False
                    Me.Div_ContoTerzi.Visible = ViewState("GestioneContoTerzi")

                    Me.Div_SelRegistri.Visible = False '0

                    Me.Div_Periodo.Visible = True '1
                    Cambia_MeseIntervallo()

                    Me.Div_Intestazione.Visible = True '4
                    Me.Chk_StampaIntestazione.Checked = True 'default

                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9


                Case enum_TipoOperazione.StampaRegistriCantina 'Stampa Registri di Cantina

                    Me.Div_SelRegistri.Visible = True '0
                    Me.Div_Periodo.Visible = True '1

                    If (ViewState("Chk_AccettazioneDaDiversi") = "1" And Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Vinificazione_DOC) Or _
                        Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Imbottigliamento Then
                        Me.Div_Categoria.Visible = True '2
                    End If

                    Select Case Me.Rbl_Report.SelectedValue
                        Case enum_AgroReportistica.Commercializzazione, _
                            enum_AgroReportistica.Vinificazione_DOC, _
                            enum_AgroReportistica.Imbottigliamento
                            If ViewState("GestioneContoTerzi") = True Then
                                Me.Div_ContoTerzi.Visible = True '3
                            End If
                            Me.Div_CentroAziendale.Visible = True '5
                        Case Else
                            Me.Div_CentroAziendale.Visible = False '5
                    End Select

                    Me.Div_Intestazione.Visible = True '4
                    Me.Chk_StampaIntestazione.Checked = False 'default
                    'Me.Div_CentroAziendale.Visible = False '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case enum_TipoOperazione.StampaVerificaRegistriCantina 'Stampa Verifica Registri di Cantina

                    Me.Div_SelRegistri.Visible = True '0
                    Me.Div_Periodo.Visible = True '1

                    If (ViewState("Chk_AccettazioneDaDiversi") = "1" And Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Vinificazione_DOC) Or _
                      Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Imbottigliamento Then
                        Me.Div_Categoria.Visible = True '2
                    End If

                    Select Case Me.Rbl_Report.SelectedValue
                        Case enum_AgroReportistica.Commercializzazione, _
                            enum_AgroReportistica.Vinificazione_DOC, _
                            enum_AgroReportistica.Imbottigliamento
                            If ViewState("GestioneContoTerzi") = True Then
                                Me.Div_ContoTerzi.Visible = True '3
                            End If
                    End Select

                    Me.Div_Intestazione.Visible = True '4
                    Me.Div_CentroAziendale.Visible = True '5
                    Me.Div_VerificaRegistri.Visible = True '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case enum_TipoOperazione.StampaCopertinaRegistro 'Copertina registro

                    Me.Div_SelRegistri.Visible = True '0
                    Me.Div_Periodo.Visible = False '1
                    Me.Div_Categoria.Visible = False '2
                    Me.Div_ContoTerzi.Visible = False '3
                    Me.Div_Intestazione.Visible = False '4
                    Me.Div_CentroAziendale.Visible = False '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = True '7
                    Me.Div_RegistroVuoto.Visible = False '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case enum_TipoOperazione.StampaRegistroVuoto 'Stampa Registro Vuoto

                    Me.Div_SelRegistri.Visible = True '0
                    Me.Div_Periodo.Visible = False '1
                    Me.Div_Categoria.Visible = False '2
                    Me.Div_ContoTerzi.Visible = False '3
                    Me.Div_Intestazione.Visible = True '4
                    Me.Div_CentroAziendale.Visible = False '5
                    Me.Div_VerificaRegistri.Visible = False '6
                    Me.Div_CopertinaRegistro.Visible = False '7
                    Me.Div_RegistroVuoto.Visible = True '8
                    Me.Div_ConsistenzeEnologiche.Visible = False '9

                Case Else

            End Select

        Else
            Me.lbl_TipoOperazione.Visible = False
            Me.ddl_TipoOperazione.Visible = False
            Me.Div_Etichette.Visible = True

        End If

    End Sub

    '####################################################################################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click
        Stampa()
    End Sub

    Private Sub Stampa()

        If Qs_Report <> enum_CodificaStampe.EtichetteVascheEnologiche Then

            Select Case Me.ddl_TipoOperazione.SelectedValue

                Case enum_TipoOperazione.RiepilogoImbottigliamenti

                    Stampa_RegCantina(False, False, False, True)

                Case enum_TipoOperazione.BrogliaccioMovimentiCompleto

                    Dim TargetURL As String = "../Cantine/BrogliaccioMovimentiTabella/BrogliaccioMovimentiTabella.aspx"
                    Dim QueryString As String = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page, TargetURL, QueryString, , , , , , , , , "MainContent", True)


                Case enum_TipoOperazione.BrogliaccioMovimentiPdfSemplificato

                    Stampa_RegCantina(False, False, True, False)

                Case enum_TipoOperazione.StampaRegistriCantina 'Stampa Registri di Cantina

                    Stampa_RegCantina(False, False, False, False)

                Case enum_TipoOperazione.RegistroCommercializzazioneOBSOLETO

                    Me.Rbl_Report.SelectedValue = enum_AgroReportistica.Commercializzazione
                    Stampa_RegCantina(False, False, False, False)

                Case enum_TipoOperazione.StampaVerificaRegistriCantina 'Stampa Verifica Registri di Cantina

                    Select Case Me.Rbl_Report.SelectedValue
                        Case enum_AgroReportistica.Vinificazione_DOC, _
                                enum_AgroReportistica.Vinificazione_ViniTavola, _
                                    enum_AgroReportistica.Commercializzazione
                            Stampa_RegCantina(False, True, False, False)
                        Case Else
                            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Verifica abilitata per Registri Vinificazione/Commercializzazione", Page, "MainContent", True)
                    End Select

                Case enum_TipoOperazione.StampaCopertinaRegistro 'Copertina registro

                    Stampa_CopertinaRegistri()

                Case enum_TipoOperazione.StampaRegistroVuoto 'Stampa Registro Vuoto

                    Stampa_RegistroVuoto()

                Case enum_TipoOperazione.StampaConsistenzeEnologiche 'Stampa Consistenze Enologiche

                    Stampa_ConsistenzeEnologiche()

                Case Else
            End Select

        Else

            Stampa_EtichetteVasche()

        End If

    End Sub



End Class
