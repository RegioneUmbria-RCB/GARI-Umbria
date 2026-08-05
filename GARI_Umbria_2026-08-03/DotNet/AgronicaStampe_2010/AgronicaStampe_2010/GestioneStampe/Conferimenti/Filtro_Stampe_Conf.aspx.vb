Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports System.Management
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.Drawing
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class Filtro_Stampe_Conf
    Inherits System.Web.UI.Page

#Region " Filtro Stampe Conferimenti "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region



    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    Dim Qs_Mode As Integer
    Dim Qs_IdAgenda As Integer
    '\\zaffiro\HP LaserJet P2015 (Agronica)|\\zaffiro\HP LaserJet 2300L

    'Const INDICE_REG_CARICOSCARICO_POMO As Integer = 10

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione

    ''##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

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
            CType(MyBase.Master, StampeBootstrap).SetTitoloPagina(141)

            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

            'Valorizzo il campo input hidden con la partita iva, utilizzabile da js
            hdPiva.Value = Qs_Piva

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitatoExportConf As Boolean = objPermessi.Controlla_Permessi_Utente(
                                        HttpContext.Current.Session("ASG_Utente_Username"),
                                        HttpContext.Current.Session("ASG_IdServizio"),
                                        enum_Security_Attivita.ManutenzioneArchivi_EsportazioneConferimentiFF,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        HttpContext.Current.Session("ASG_objParametri_Utenti"))
            hdAbilitaExportConf.Value = IIf(UtenteAbilitatoExportConf, 1, 0)

            '**************************************************************************************************
            '*** IL FILTRO SI CONFIGURA IN BASE AI MODULI ATTIVI 
            '**************************************************************************************************
            'Configurazione per F&F : conferimento DA (es. LaBuonaRomagna)
            'Altri casi (al momento si escudono le cantine): conferimento A  (es. Agrisfera) 
            '**************************************************************************************************

            int_Configurazione_Moduli = ConfigurazioneModuli()
            hdIntConfigurazioneModuli.Value = int_Configurazione_Moduli

            'Leggo l'impostazione per la gerarchia attraverso obj parametri superuser e la classe DAL Utenti_Impostazioni_Read
            Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtSuperUser As DataTable = objImpR.Leggi(enum_Impostazioni_Utenti.SUPERUSER_ACCETTAZIONE_CON_GERARCHIA, 2,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", objParametri_Utenti)
            Dim superUserAccettazioneConGerarchia = 0
            if dtSuperUser.Rows.Count > 0
                superUserAccettazioneConGerarchia = dtSuperUser.Rows(0)("Impostazione_Valore_1")
            End If
            hdSuperUserAccGerarchia.Value = superUserAccettazioneConGerarchia


            '        '============================================================
            '        '       Sono in postback
            '        '============================================================
            '        If Me.IsPostBack Then

            If Not Page.IsPostBack Then
                Imposta_Default_Controlli(Me.Rbl_Report.SelectedValue)
                Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
                hf_filtroMateriePrimeConferimento.Value = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Conferimento, objParametri_Utenti)
            Else
                Exit Sub
            End If

            If UtenteAbilitatoExportConf = True Then
                Dim listitem As New System.Web.UI.WebControls.ListItem("Esportazione Conferimenti su file CSV", "666")
                Me.Rbl_Report.Items.Add(listitem)
            End If

        Catch ex As Exception
            '''Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
            '''Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)
            '''Me.Rbl_Certificati.SelectedValue = 1
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Problemi durante il caricamento della pagina: " + vbCrLf + ex.Message, Page, "MainContent", True)
        End Try



    End Sub


    ''##################################################################################
    'Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click

    '    Dim strClose As String = "<script language='javascript'>window.close()</script>"
    '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    'End Sub


    ''##################################################################################
    Private Sub Rbl_Report_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Report.SelectedIndexChanged

        Imposta_Filtri_Report(Me.Rbl_Report.SelectedValue)

    End Sub


    '##################################################################################
    Private Sub AbilitaDate(abilitato As Boolean)
        lbl_validita_inizio.Visible = abilitato
        lbl_validita_fine.Visible = abilitato
        Txt_ValiditaInizio.Visible = abilitato
        Txt_ValiditaFine.Visible = abilitato
    End Sub

    Private Sub AbilitaGiacenza(abilitato As Boolean)
        lbl_DataGiacenza.Visible = abilitato
        Txt_DataGiacenza.Visible = abilitato
    End Sub

    Private Sub AbilitaMagazzino(abilitato As Boolean)
        lbl_Magazzino.Visible = abilitato
        Cmb_Magazzino.Visible = abilitato
    End Sub

    Private Sub AbilitaRapportoContabile(abilitato As Boolean)
        Lbl_RapCon.Visible = abilitato
        Rbl_RapCon.Visible = abilitato
        If Not abilitato Then
            Rbl_RapCon.SelectedValue = "0"
        End If
    End Sub

    Private Sub AbilitaStabilimento(abilitato As Boolean)
        Lbl_DettaglioStabilimento.Visible = abilitato
        Chk_DettStabilimento.Visible = abilitato
    End Sub

    Private Sub AbilitaProdotto(abilitato As Boolean)
        lbl_prodotto.Visible = abilitato
        lbl_filtro_prodotti_cod.Visible = abilitato
        lbl_filtro_prodotti_des.Visible = abilitato
        Txt_Filtro_MatDes.Visible = abilitato
        Txt_Filtro_CodArticolo.Visible = abilitato
        Btn_Carica_Prodotti.Visible = abilitato
        Cmb_Prodotti.Visible = abilitato
    End Sub

    Private Sub AbilitaCedente(abilitato As Boolean)
        lbl_RagSoc_ConfCli.Visible = abilitato
        lbl_Piva_ConfCli.Visible = abilitato
        lbl_Soggetti.Visible = abilitato
        Txt_FiltroPiva_Conf.Visible = abilitato
        Txt_FiltroRagSoc_Conf.Visible = abilitato
        Btn_Carica_Conferente.Visible = abilitato
        cmb_conferente.Visible = abilitato
    End Sub

    Private Sub Imposta_Filtri_Report(ByVal Report As enum_CodificaStampe)

        AbilitaDate(False)
        AbilitaProdotto(False)
        AbilitaCedente(False)

        AbilitaGiacenza(False)
        AbilitaMagazzino(False)
        AbilitaRapportoContabile(False)
        AbilitaStabilimento(False)

        Select Case Report
            Case enum_CodificaStampe.Conf_EC_Imballi

                AbilitaDate(True)
                AbilitaProdotto(True)
                AbilitaCedente(True)
                AbilitaGiacenza(False)
                AbilitaMagazzino(True)
                AbilitaRapportoContabile(False)
                AbilitaStabilimento(True)

            Case enum_CodificaStampe.Conf_Saldo_Imballi

                AbilitaDate(True)
                AbilitaProdotto(True)
                AbilitaCedente(True)
                AbilitaGiacenza(True)
                AbilitaMagazzino(True)
                AbilitaRapportoContabile(False)
                AbilitaStabilimento(True)

            Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti

                AbilitaDate(True)
                AbilitaProdotto(True)
                AbilitaCedente(True)
                AbilitaGiacenza(False)
                AbilitaMagazzino(False)
                AbilitaRapportoContabile(True)
                AbilitaStabilimento(False)

                ' Azzero le scelte perchè potrei essere passato da report imballi dove vengono 
                ' considerati anche clienti e fornitori a report conferimenti dove vengono accettati
                ' solo fornitori ortofrutta e conferenti
                Txt_FiltroRagSoc_Conf.Text = ""
                Txt_FiltroPiva_Conf.Text = ""
                cmb_conferente.Items.Clear()

            Case enum_CodificaStampe.Conf_Tracciabilita

                AbilitaDate(True)
                AbilitaProdotto(True)
                AbilitaCedente(True)
                AbilitaGiacenza(False)
                AbilitaMagazzino(False)
                AbilitaRapportoContabile(True)
                AbilitaStabilimento(False)

                ' Azzero le scelte perchè potrei essere passato da report imballi dove vengono 
                ' considerati anche clienti e fornitori a report conferimenti dove vengono accettati
                ' solo fornitori ortofrutta e conferenti
                Txt_FiltroRagSoc_Conf.Text = ""
                Txt_FiltroPiva_Conf.Text = ""
                cmb_conferente.Items.Clear()

            Case 666 'EXPORT CONF CSV
                'non devo abilitare niente
                'avvio direttamente la stampa
                Stampa()
        End Select


        '    '-----------------------------------------------
        '    '-------------- Opzioni Stampa -----------------
        '    '-----------------------------------------------
        '    If Me.Pannello_OpzioniStampa.Visible = True Then

        '        'Dim ip As String = Request.UserHostAddress()

        '        Stampanti_Load()

        '    End If


    End Sub


    ''##################################################################################
    Private Sub Imposta_Default_Controlli(ByVal Report As enum_CodificaStampe)

        Dim objUtentiImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        Dim Validita_Inizio, Validita_Fine As String
        Dim Data_Giacenza As String


        Try

            '-----------------------------------------------
            '--- Configurazione Filtro in base ai moduli
            '-----------------------------------------------
            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                lbl_Soggetti.Text = "Cedente"
                'lbl_Soggetti.Font = New Font("myfontname", 10, FontStyle.Bold, GraphicsUnit.Point)
            Else
                lbl_Soggetti.Text = "Cliente"
                'lbl_Soggetti.Font.Bold = True
            End If
            '-----------------------------------------------



            '-----------------------------------------------
            '--------- Default Date  -----------------------
            '-----------------------------------------------
            If Me.Txt_ValiditaInizio.Text = "" Then
                Me.Txt_ValiditaInizio.Text = ("01/" & Right("00" & CStr(Month(Date.Today)), 2) & "/" & Year(Date.Today) & "")

            End If
            If Me.Txt_ValiditaFine.Text = "" Then
                Me.Txt_ValiditaFine.Text = Date.Today.ToShortDateString
            End If
            If Me.Txt_DataGiacenza.Text = "" Then
                Me.Txt_DataGiacenza.Text = Date.Today.ToShortDateString
            End If

            Imposta_Filtri_Report(Me.Rbl_Report.SelectedValue)

            Select Case Report

                Case enum_CodificaStampe.Conf_EC_Imballi
                    Validita_Inizio = Me.Txt_ValiditaInizio.Text
                    Validita_Fine = Me.Txt_ValiditaFine.Text
                    Data_Giacenza = Me.Txt_DataGiacenza.Text

                Case enum_CodificaStampe.Conf_Saldo_Imballi
                    Validita_Inizio = Me.Txt_ValiditaInizio.Text
                    Validita_Fine = Me.Txt_ValiditaFine.Text
                    Data_Giacenza = Me.Txt_DataGiacenza.Text

                Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti
                    Validita_Inizio = Me.Txt_ValiditaInizio.Text
                    Validita_Fine = Me.Txt_ValiditaFine.Text
                    Data_Giacenza = Me.Txt_DataGiacenza.Text
            End Select




            '-----------------------------------------------
            '---------------- Magazzini --------------------
            '-----------------------------------------------

            Dim Sa_Cod As Integer = 0
            Dim Fabbricato_Cod As Integer = 0

            'aggiunta riga vuota in data 14/06/17
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Fabbricati(Me.Cmb_Magazzino,
                                                            True, "Tutti i magazzini", "0|0",
                                                            Qs_Piva,
                                                            Sa_Cod,
                                                            Fabbricato_Cod,
                                                            MAGAZZINO,
                                                             True,
                                                             "",
                                                             " Fabbricati.Fabbricato_Des ",
                                                             AGRODATAFINE,
                                                             objParametri_Server)



        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Si è verificato il seguente errore: " + ex.Message, Page, "MainContent", True)
        End Try


    End Sub


    '''##################################################################################
    'Private Sub Cmb_Magazzino_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Magazzino.SelectedIndexChanged
    '    'Cambia_Selezione_Magazzino()
    'End Sub

    ''##################################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click
        Stampa()
    End Sub

    ''##################################################################################
    'VECCHIA FUNZIONE DI STAMPA, LA NUOVA E': EseguiEstrazione
    Private Sub Stampa()

        Try

            Dim QueryString As String

            Dim Validita_Inizio As String = AGRODATAINIZIO '"01/01/1900"
            Dim Validita_Fine As String = AGRODATAFINE '"31/12/2100"
            Dim Data_Giacenza As String
            Dim Mat_Cod As Integer = 0
            Dim Cod_Articolo As String = ""
            Dim Sa_Cod As Integer = 0
            Dim Fabbricato_Cod As Integer = 0
            Dim Codice_Specie As Integer = 0
            Dim DA_Codice_Specie As Integer = 0
            Dim A_Codice_Specie As Integer = 0
            Dim Str_Codici_Specie As String = ""
            Dim Codice_ConfCli As Integer = 0
            Dim Piva_ConfCli As String = ""
            Dim RagSoc_ConfCli As String = ""



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


            Dim Flag_SalvaPagRiga As Boolean


            Coltrolla_RecuperaFiltri(Me.Rbl_Report.SelectedValue,
                                    Log_Errori,
                                    Validita_Inizio,
                                    Validita_Fine,
                                    Codice_Specie,
                                    Str_Codici_Specie,
                                    Piva_ConfCli,
                                    RagSoc_ConfCli,
                                    Mat_Cod,
                                    Cod_Articolo,
                                    Sa_Cod,
                                    Fabbricato_Cod,
                                    Descr_Specie,
                                    Descr_Prodotto,
                                    Descr_Magazzino,
                                    Str_Id_Agenda_Bolle,
                                    Str_Flag_Fascicola,
                                    Numero_Copie,
                                    Flag_SalvaPagRiga,
                                    Data_Giacenza,
                                    Regolamento_cod)

            If Log_Errori <> "" Then
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(Log_Errori, Page, "MainContent", True)
                Exit Sub
            End If

            'Dim objOModuli As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            'Dim Flag01_GestioneCodEsterno As Integer = objOModuli.FF_GestioneCodiceEsterno_0No_1Si(Qs_Piva, objParametri_Server)


            Select Case Me.Rbl_Report.SelectedValue

                Case enum_CodificaStampe.Conf_RiepilogoxArticolo

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&sn=" + Stringa_Codifica(CStr(""), AgroKey_EncoderDecoder, Server) +
                                     "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                     "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&dpr=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page,
                                    "Riepilogo_Conf_Articolo/Riepilogo_Conf_Articolo.aspx",
                                    QueryString,
                                    "ConferimentiXArticolo",
                                    , , , , , , , "MainContent", True)


                Case enum_CodificaStampe.Conf_Esportazione_BolleFF_XLS

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&cc=" + Stringa_Codifica(CStr(Piva_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&rsc=" + Stringa_Codifica(CStr(RagSoc_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&dpr=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server) &
                                    "&ti=" + Stringa_Codifica(1, AgroKey_EncoderDecoder, Server) &
                                    "&ces=" + Stringa_Codifica("", AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page,
                                    "Bolle_Conf_XLS/Bolle_Conf_XLS.aspx",
                                    QueryString,
                                    "ConferimentiExcel",
                                    , , , , , , , "MainContent", True)

                Case enum_CodificaStampe.Conf_EsportazionexTrasportatori_XLS

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&cc=" + Stringa_Codifica(CStr(Piva_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&rsc=" + Stringa_Codifica(CStr(RagSoc_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&dpr=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server) &
                                    "&ti=" + Stringa_Codifica(1, AgroKey_EncoderDecoder, Server) &
                                    "&ces=" + Stringa_Codifica("", AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page,
                                    "Trasportatori_XLS/Trasportatori_Excel.aspx",
                                    QueryString,
                                    "TrasportatoriExcel",
                                    , , , , , , , "MainContent", True)

                Case enum_CodificaStampe.Conf_EC_Imballi

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&cc=" + Stringa_Codifica(CStr(Piva_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&rsc=" + Stringa_Codifica(CStr(RagSoc_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&imb=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server) +
                                    "&ds=" + Stringa_Codifica(Me.Chk_DettStabilimento.Checked, AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page,
                                    "EC_Imballi_Conf/EC_Imballi_Conf.aspx",
                                    QueryString,
                                    "EstrattoConto_Imballi",
                                     , , , , , , , "MainContent", True)

                    '------------------------------------
                Case enum_CodificaStampe.Conf_Saldo_Imballi

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&cc=" + Stringa_Codifica(CStr(Piva_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&rsc=" + Stringa_Codifica(CStr(RagSoc_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&imb=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&dg=" + Stringa_Codifica(Data_Giacenza, AgroKey_EncoderDecoder, Server) +
                                    "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server) +
                                    "&ds=" + Stringa_Codifica(Me.Chk_DettStabilimento.Checked, AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page,
                                    "SaldoImballi_Conf/SaldoImballi_Conf.aspx",
                                    QueryString,
                                    "SaldoImballi",
                                    , , , , , , , "MainContent", True)


                Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&cc=" + Stringa_Codifica(CStr(Piva_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&rsc=" + Stringa_Codifica(CStr(RagSoc_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&dpr=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&dg=" + Stringa_Codifica(Data_Giacenza, AgroKey_EncoderDecoder, Server) +
                                    "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server)


                    Page_NewWindow_2010(Page,
                                    "Riepilogo_Conf/Riepilogo_Conf.aspx",
                                    QueryString,
                                    "RiepilogoConferimenti",
                                    , , , , , , , "MainContent", True)


                Case enum_CodificaStampe.Conf_Tracciabilita

                    QueryString = "?dd=" + Stringa_Codifica(Validita_Inizio, AgroKey_EncoderDecoder, Server) +
                                    "&da=" + Stringa_Codifica(Validita_Fine, AgroKey_EncoderDecoder, Server) +
                                    "&cc=" + Stringa_Codifica(CStr(Piva_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&rsc=" + Stringa_Codifica(CStr(RagSoc_ConfCli), AgroKey_EncoderDecoder, Server) +
                                    "&p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&f=" + Stringa_Codifica(CStr(Fabbricato_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&m=" + Stringa_Codifica(CStr(Mat_Cod), AgroKey_EncoderDecoder, Server) +
                                    "&ca=" + Stringa_Codifica(CStr(Cod_Articolo), AgroKey_EncoderDecoder, Server) +
                                    "&dpr=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Prodotto)), AgroKey_EncoderDecoder, Server) +
                                    "&mag=" + Stringa_Codifica(QS_SaveText(CStr(Descr_Magazzino)), AgroKey_EncoderDecoder, Server) +
                                    "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) +
                                    "&dg=" + Stringa_Codifica(Data_Giacenza, AgroKey_EncoderDecoder, Server) +
                                    "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) +
                                    "&rc=" + Stringa_Codifica(Me.Rbl_RapCon.SelectedValue, AgroKey_EncoderDecoder, Server)

                    Page_NewWindow_2010(Page,
                                    "Tracciabilita_Conf/Tracciabilita_Conf_XLS.aspx",
                                    QueryString,
                                    "TracciabilitaConferimenti",
                                    , , , , , , , "MainContent", True)

                Case 666 ' link all'esportazione conferimenti CSV nel sito AgronicaSincronizzatore

                    'Dim objSincro As New AgronicaCoreGestioneRichieste.ParametriSincronizzatore_2010
                    'objSincro.Piva = Qs_Piva
                    'objSincro.Pagina_Richiesta =
                    'objSincro.Id_Cod_Cliente = 0

                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                                        Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                                        enum_PagineAgronicaSincro.EsportazioneConferimentiFF,
                                        0,
                                        Qs_Piva,
                                        0)

                    'IPOTESI1:
                    Dim urlRedirect As String
                    Dim xRedir As String() = strJS.Split("'")
                    urlRedirect = xRedir(3)
                    'Dim redirMe As String = "<script> window.location = '" & urlRedirect & "' </script>"
                    'ParametroDue = redirMe
                    'tipo_risposta = "1"
                    Response.Redirect(urlRedirect)

                    'IPOTESI2:
                    'ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel), _
                    '                       CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel).GetType(), _
                    '                       "jQuery_{0}", strOpen, False)

            End Select


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Si è verificato il seguente errore durante la chiamata alla stampa: " + ex.Message, Page, "MainContent", True)
        End Try



    End Sub


    ''##################################################################################
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


    ''##################################################################################
    Private Sub Coltrolla_RecuperaFiltri(ByVal Report As enum_CodificaStampe, _
                                          ByRef Log_Errori As String, _
                                            ByRef Validita_Inizio As String, _
                                            ByRef Validita_Fine As String, _
                                            ByRef Codice_Specie As Integer, _
                                            ByRef Str_Codici_Specie As String, _
                                            ByRef Piva_Conferente As String, _
                                            ByRef RagSoc_Conferente As String, _
                                            ByRef Mat_Cod As Integer, _
                                            ByRef Cod_Articolo As String, _
                                            ByRef Sa_Cod As Integer, _
                                            ByRef Fabbricato_Cod As Integer, _
                                            ByRef Descr_Specie As String, _
                                            ByRef Descr_Prodotto As String, _
                                            ByRef Descr_Magazzino As String, _
                                            ByRef Str_Id_Agenda_Bolle As String, _
                                            ByRef Str_Flag_Fascicola As String, _
                                            ByRef Numero_Copie As Integer, _
                                            ByRef Flag_SalvaPagRiga As Boolean, _
                                            ByRef Data_Giacenza As String, _
                                            ByRef Regolamento_Cod As Integer)



        Dim Messaggio_Conf As String = ""


        Dim Flag_ConfCli As Boolean = False


        Dim Flag_Prodotto As Boolean = False
        Dim Flag_Magazzino As Boolean = False

        Dim Flag_DataGiacenza As Boolean = False
        Dim Flag_Regolamento As Boolean = False

        'l'intervallo temporale è per tutti i report

        Controlla_ValiditaInizioFine(Log_Errori, _
                                    Validita_Inizio, _
                                    Validita_Fine)

        Select Case Report


            '------------------------------------
            Case enum_CodificaStampe.Conf_EC_Imballi

                Flag_Magazzino = True
                Flag_ConfCli = True
                Flag_Prodotto = True
                Flag_Prodotto = True

                '------------------------------------
            Case enum_CodificaStampe.Conf_Saldo_Imballi

                Flag_Magazzino = True
                Flag_ConfCli = True
                Flag_Prodotto = True
                Flag_DataGiacenza = True

                '------------------------------------
            Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti

                Flag_Magazzino = True
                Flag_ConfCli = True
                Flag_Prodotto = True
                Flag_DataGiacenza = False


            Case enum_CodificaStampe.Conf_Tracciabilita

                Flag_Magazzino = True
                Flag_ConfCli = True
                Flag_Prodotto = True
                Flag_DataGiacenza = False

        End Select


        '----------------------------
        '-------- PRODOTTO ----------
        '----------------------------
        If Flag_Prodotto = True Then

            If (Me.Txt_Filtro_MatDes.Text) <> "" Then
                Descr_Prodotto = Me.Txt_Filtro_MatDes.Text
            End If
            If Me.Txt_Filtro_CodArticolo.Text <> "" Then
                Cod_Articolo = Me.Txt_Filtro_CodArticolo.Text
            End If

            If Not IsNothing(Me.Cmb_Prodotti.SelectedItem) Then
                If Me.Cmb_Prodotti.SelectedValue <> "0" Then
                    Mat_Cod = Me.Cmb_Prodotti.SelectedValue
                End If

                'Log_Errori = "E' necessario selezionare il magazzino!" + vbCrLf
            End If

        End If


        '----------------------------
        '-------- CONFERENTE --------
        '----------------------------

        If Flag_ConfCli = True Then
            If Not IsNothing(Me.cmb_conferente.SelectedItem) Then
                If Me.cmb_conferente.SelectedValue <> "0" Then
                    Piva_Conferente = Me.cmb_conferente.SelectedValue
                    RagSoc_Conferente = Me.cmb_conferente.SelectedItem.Text
                Else
                    Piva_Conferente = Me.Txt_FiltroPiva_Conf.Text
                    RagSoc_Conferente = Me.Txt_FiltroRagSoc_Conf.Text
                End If
            Else
                Piva_Conferente = Me.Txt_FiltroPiva_Conf.Text
                RagSoc_Conferente = Me.Txt_FiltroRagSoc_Conf.Text

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

        If Messaggio_Conf <> "" Then
            Log_Errori += Messaggio_Conf + vbCrLf
        End If

    End Sub


    '##################################################################################

    Private Sub Btn_Carica_Conferente_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Conferente.Click

        Try

            Dim int_CodRapporti As Integer()
            If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                ' Per gli imballi considero ogni tipo di rapporto contabile
                If Rbl_Report.SelectedValue = enum_CodificaStampe.Conf_EC_Imballi Or
                   Rbl_Report.SelectedValue = enum_CodificaStampe.Conf_Saldo_Imballi Then
                    int_CodRapporti = New Integer() {COD_CLIENTE, COD_FORNITORE, COD_CONFERENTE, COD_FORNITORE_ORTOFRUTTA}
                Else
                    int_CodRapporti = New Integer() {COD_CONFERENTE, COD_FORNITORE_ORTOFRUTTA}
                End If

            Else
                int_CodRapporti = New Integer() {COD_CLIENTE}
            End If

            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.RisorseUmanePerConferimenti(Me.cmb_conferente,
                                                                True, "", "0",
                                                                Qs_Piva,
                                                                Me.Txt_FiltroPiva_Conf.Text,
                                                                0,
                                                                int_CodRapporti,
                                                                Me.Txt_FiltroRagSoc_Conf.Text,
                                                                "",
                                                                "",
                                                                objParametri_Server)


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Caricamento dei conferenti. Si è verificato il seguente errore: " + ex.Message, Page, "MainContent", True)
        End Try

    End Sub



    ''##################################################################################

    Private Sub Btn_Carica_Prodotti_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Prodotti.Click

        Carica_Prodotti()

    End Sub


    ''##################################################################################

    'L'elenco dei prodotti cambia a seconda del report selezionato:
    'Imballi o Trasformati vegetali/animali
    Private Sub Carica_Prodotti()

        Me.Cmb_Prodotti.Items.Clear()

        Select Case Me.Rbl_Report.SelectedValue

            Case enum_CodificaStampe.Conf_EC_Imballi, enum_CodificaStampe.Conf_Saldo_Imballi

                'AgronicaCoreUtility.CaricaListControl.Imballaggi(Me.Cmb_Prodotti, _
                '                                                  True, "", "0", _
                '                                                 Qs_Piva, _
                '                                                 Me.Txt_Filtro_MatDes.Text, _
                '                                                 Me.Txt_Filtro_CodArticolo.Text, _
                '                                                 "", _
                '                                                 0, _
                '                                                 "", "", _
                '                                                 objParametri_Server, objParametri_Utenti)

                AgronicaCoreUtility.CaricaListControl.BeniConfezionamentoVegetale(Me.Cmb_Prodotti, _
                                                  True, "", "0", _
                                                 Qs_Piva, _
                                                 Me.Txt_Filtro_MatDes.Text, _
                                                 Me.Txt_Filtro_CodArticolo.Text, _
                                                 "", _
                                                 0, _
                                                 "", "", _
                                                 objParametri_Server, objParametri_Utenti)



            Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti, enum_CodificaStampe.Conf_Tracciabilita

                AgronicaCoreUtility.CaricaListControl.ProdottiConferiti(Me.Cmb_Prodotti, _
                                                                  True, "", "0", _
                                                                 Qs_Piva, _
                                                                 Me.Txt_Filtro_MatDes.Text, _
                                                                 Me.Txt_Filtro_CodArticolo.Text, _
                                                                 "", _
                                                                 0, _
                                                                 " 1=1 ", "", _
                                                                 objParametri_Server, objParametri_Utenti)

        End Select
    End Sub




    Private Function ConfigurazioneModuli() As enum_Omni_Modulo_Generazione

        Dim DT_Moduli As DataTable
        Dim bln_ModuloFF As Boolean = False

        Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R

        DT_Moduli = objOmni.Leggi("", 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

        If DT_Moduli.Rows.Count > 0 Then
            For i = 0 To DT_Moduli.Rows.Count - 1
                If DT_Moduli.Rows(i).Item("Modulo_Generazione") = enum_Omni_Modulo_Generazione.FreshFood Then
                    bln_ModuloFF = True
                End If
            Next
        End If

        If bln_ModuloFF Then
            Return enum_Omni_Modulo_Generazione.FreshFood
        Else
            Return enum_Omni_Modulo_Generazione.Nessuno
        End If

    End Function
    
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiDocPrefissiSuffissi(ByVal piva As String, ByVal preSuf As Integer) As RispostaStandard
        Dim r As New RispostaStandard()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim handleMovimenti As New AgronicaCoreContabDAL.Movimenti_R()
        Try
            Dim dt = handleMovimenti.LeggiDistinctDocNumeroSinDes(
                piva, 
                preSuf, 
                New String() {CAU_REGISTRAZIONI}, 
                New Integer() {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}, 
                objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True
        
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiElencoStampanti(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim handleConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R()
        Try
            Dim strStampanti = handleConfigSiti.Leggi_Valore(0, "Lista_Stampanti", "", "", objParametri_Server)

            Dim arrStampanti = strStampanti.Split("|")

            'HP|Kyocera
            '[{Desc: "HP", Value: "HP"}, {Desc: "Kyocera", Value: "Kyocera"}]

            Dim jArrStampanti As New JArray()
            For Each stampante As String In arrStampanti
                jArrStampanti.Add(New JObject(New JProperty("Desc", stampante), New JProperty("Value", stampante)))
                'Dim jStampante As New JObject()
                'jStampante("Desc") = stampante
            Next

            For Each stampInstallata As String In Printing.PrinterSettings.InstalledPrinters
                jArrStampanti.Add(New JObject(New JProperty("Desc", stampInstallata), New JProperty("Value", stampInstallata)))
            Next

            'Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            'r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaStringa = jArrStampanti.ToString(Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function EseguiEstrazione(params As String) As RispostaStandard

        Dim r As New RispostaStandard()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        'Dim paramsJson = JObject.Parse(params)
        'Dim jconvert As New JsonConverter
        'Dim paramsObj = JsonConvert.DeserializeObject(params)
        Dim objFiltriEstrazioni As FiltriEstrazioni = JsonConvert.DeserializeObject(params, (New FiltriEstrazioni).GetType(), settingLoc)

        'Dim rispostaValFiltri = objFiltriEstrazioni.ControllaValiditaFiltri()
        Dim rispostaValFiltri = ValidaFiltri(objFiltriEstrazioni)
        If rispostaValFiltri.RispostaOK Then

            'Valorizzo filtri calcolati aggiuntivi
            Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objOModuli As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno = objOModuli.FF_GestioneCodiceEsterno_0No_1Si(objFiltriEstrazioni.hdPiva, objParametri_Server)

            Dim queryString As String
            Dim obj As New JObject

            'Se è stato selezionato un solo Prodotto passo il mat_cod in QueryString
            'altrimenti utilizzo la variabile di sessione Str_Codici_Prodotto con i mat_cod concatenati.
            Dim mat_cod_QS As Integer = 0
            Dim Codici_Prodotti As String = ""

            'Per i report "Estratto Conto Beni Confezionamento" (184) e
            '"Saldo Imballi" (185) nascondo la griglia dei Prodotti e mostro la Dropdown.
            'Per gli altri report invece lascio visibile la griglia e nascondo la DropDown.
            If objFiltriEstrazioni.ddlEstrazioni = enum_CodificaStampe.Conf_EC_Imballi OrElse
               objFiltriEstrazioni.ddlEstrazioni = enum_CodificaStampe.Conf_Saldo_Imballi Then

                mat_cod_QS = CInt(objFiltriEstrazioni.ddlProdotti)

            Else
                If Not objFiltriEstrazioni.ElencoProdotti Is Nothing AndAlso
                    Not String.IsNullOrEmpty(objFiltriEstrazioni.ElencoProdotti) Then

                    If objFiltriEstrazioni.ElencoProdotti.Split("|").Count = 1 Then
                        mat_cod_QS = CInt(objFiltriEstrazioni.ElencoProdotti.Split("|")(0))
                    Else
                        Codici_Prodotti = objFiltriEstrazioni.ElencoProdotti
                    End If

                End If

            End If


            HttpContext.Current.Session("Str_Codici_Prodotto") = Codici_Prodotti

            Select Case objFiltriEstrazioni.ddlEstrazioni

                '1) RIEPILOGO CONFERIMENTI X ARTICOLO
                Case enum_CodificaStampe.Conf_RiepilogoxArticolo

                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&rs=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlCentriAziendali), AgroKey_EncoderDecoder, Nothing) &
                                  "&sn=" & Stringa_Codifica(objFiltriEstrazioni.centroAziendaleDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&spe=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlSpecie), AgroKey_EncoderDecoder, Nothing) &
                                  "&var=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlVarieta), AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&fce=" & Stringa_Codifica(CStr(objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno), AgroKey_EncoderDecoder, Nothing) &
                                  "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "Riepilogo_Conf_Articolo/Riepilogo_Conf_Articolo.aspx"
                    obj("paginaTitolo") = "ConferimentiXArticolo"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                    '=========================================================================================

                    '2) ESTRATTO CONTO BOLLE CONFERIMENTO
                Case enum_CodificaStampe.Conf_EC_Bolle
                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&rs=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlCentriAziendali), AgroKey_EncoderDecoder, Nothing) &
                                  "&sn=" & Stringa_Codifica(objFiltriEstrazioni.centroAziendaleDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&spe=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlSpecie), AgroKey_EncoderDecoder, Nothing) &
                                  "&var=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlVarieta), AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&fce=" & Stringa_Codifica(CStr(objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno), AgroKey_EncoderDecoder, Nothing) &
                                  "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "EC_Bolle_Conf/EC_Bolle_Conf.aspx"
                    obj("paginaTitolo") = "EstrattoConto_Bolle"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True
                    'r.RispostaStringa = "Report in costruzione"

                    '=========================================================================================

                    '3) ESTRATTO CONTO IMBALLI
                Case enum_CodificaStampe.Conf_EC_Imballi
                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    'Dim saCod = objFiltriEstrazioni.ddlMagazzini.Split("_")(1)
                    'Dim fabbricatoCod = objFiltriEstrazioni.ddlMagazzini.Split("_")(2)
                    Dim saCod = CStr(objFiltriEstrazioni.ddlCentriAziendali)
                    Dim fabbricatoCod = 0

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(saCod, AgroKey_EncoderDecoder, Nothing) &
                                  "&f=" & Stringa_Codifica(fabbricatoCod, AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&imb=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&mag=" & Stringa_Codifica(objFiltriEstrazioni.magazzinoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&cm=" & Stringa_Codifica(CStr(objFiltriEstrazioni.hdIntConfigurazioneModuli), AgroKey_EncoderDecoder, Nothing) &
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing) &
                                  "&ds=" & Stringa_Codifica(objFiltriEstrazioni.kSwitchDettaglioStabilimento, AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "EC_Imballi_Conf/EC_Imballi_Conf.aspx"
                    obj("paginaTitolo") = "EstrattoConto_Imballi"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                      '=========================================================================================

                    '4) SALDO IMBALLI
                Case enum_CodificaStampe.Conf_Saldo_Imballi
                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    'Dim saCod = objFiltriEstrazioni.ddlMagazzini.Split("_")(1)
                    'Dim fabbricatoCod = objFiltriEstrazioni.ddlMagazzini.Split("_")(2)
                    Dim saCod = CStr(objFiltriEstrazioni.ddlCentriAziendali)
                    Dim fabbricatoCod = 0

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(saCod, AgroKey_EncoderDecoder, Nothing) &
                                  "&f=" & Stringa_Codifica(fabbricatoCod, AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&imb=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&mag=" & Stringa_Codifica(objFiltriEstrazioni.magazzinoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&dg=" & Stringa_Codifica(objFiltriEstrazioni.dpDataGiacenza.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&cm=" & Stringa_Codifica(CStr(objFiltriEstrazioni.hdIntConfigurazioneModuli), AgroKey_EncoderDecoder, Nothing) &
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing) &
                                  "&ds=" & Stringa_Codifica(objFiltriEstrazioni.kSwitchDettaglioStabilimento, AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "SaldoImballi_Conf/SaldoImballi_Conf.aspx"
                    obj("paginaTitolo") = "SaldoImballi"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                      '=========================================================================================

                    '5) ESPORTAZIONE BOLLE SU EXCEL
                Case enum_CodificaStampe.Conf_Esportazione_BolleFF_XLS

                    'Nuovo report, dati che verranno esportati in excel:
                    ' codice conferente (l'ho a disposizione in objFiltriEstrazioni.conferenteCodRisum imposto cod_RisUm come value oppure se prendo dal datasource)
                    ' ragione sociale conferente (come sopra ma con il campo Rag_Soc)
                    ' partita iva produttore (produttore si ha solo in caso "accettazione con gerarchia"; come per il conferente)
                    ' produttore (produttore si ha solo in caso "accettazione con gerarchia"; se si intende la ragione sociale come per il conferente)
                    ' codice specie (objFiltriEstrazioni.ddlSpecie)
                    ' descrizione prodotto (è presente nel data source come Prodotto_Des)
                    ' indice qualitativo (no)
                    ' netto a pagamento (no)
                    ' numero bolla (no)
                    ' data bolla (no)
                    ' classificazione qualitativa della specie (no)

                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&rs=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlCentriAziendali), AgroKey_EncoderDecoder, Nothing) &
                                  "&sn=" & Stringa_Codifica(objFiltriEstrazioni.centroAziendaleDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&spe=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlSpecie), AgroKey_EncoderDecoder, Nothing) &
                                  "&var=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlVarieta), AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&fce=" & Stringa_Codifica(CStr(objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno), AgroKey_EncoderDecoder, Nothing) &
                                  "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing) &
                                  "&ti=" & Stringa_Codifica(objFiltriEstrazioni.kSwitchTracciabilitaImpianti, AgroKey_EncoderDecoder, Nothing) &
                                  "&pp=" & Stringa_Codifica(objFiltriEstrazioni.produttorePiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&pc1=" & Stringa_Codifica(objFiltriEstrazioni.primoCessionarioPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&pc2=" & Stringa_Codifica(objFiltriEstrazioni.secondoCessionarioPiva, AgroKey_EncoderDecoder, Nothing)


                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "Bolle_Conf_XLS/Bolle_Conf_XLS.aspx"
                    obj("paginaTitolo") = "ConferimentiExcel"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True
                    'r.RispostaStringa = "Report temporaneamente in manutenzione"

                      '=========================================================================================

                    '6) ESPORTAZIONE TRASPORTATORI SU EXCEL
                Case enum_CodificaStampe.Conf_EsportazionexTrasportatori_XLS
                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&rs=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlCentriAziendali), AgroKey_EncoderDecoder, Nothing) &
                                  "&sn=" & Stringa_Codifica(objFiltriEstrazioni.centroAziendaleDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&spe=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlSpecie), AgroKey_EncoderDecoder, Nothing) &
                                  "&var=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlVarieta), AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&fce=" & Stringa_Codifica(CStr(objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno), AgroKey_EncoderDecoder, Nothing) &
                                  "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing)

                    '"&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing)
                    '"&ces=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Server)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "Trasportatori_XLS/Trasportatori_Excel.aspx"
                    obj("paginaTitolo") = "TrasportatoriExcel"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                                    '=========================================================================================

                    '7) STAMPA MASSIVA BOLLE CONFERIMENTO
                Case enum_CodificaStampe.FreshFood_BollaAccettazione

                    Dim elenIdAgenda As String = ""
                    Dim mexErrore = ElencoIdAgenda_StampaMassiva_BolleCertificati(objParametri_Server, objFiltriEstrazioni, elenIdAgenda)

                    If mexErrore = "" Then
                        HttpContext.Current.Session("Str_Id_Agenda_Bolle") = elenIdAgenda

                        queryString = "p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                      "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                                      "&l=" & Stringa_Codifica(LAVCOD_ACCETTAZIONE_DIVERSI, AgroKey_EncoderDecoder, Nothing) &
                                      "&ptp=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Nothing) &
                                      "&pn=" & Stringa_Codifica(objFiltriEstrazioni.Stampante, AgroKey_EncoderDecoder, Nothing) &
                                      "&nc=" & Stringa_Codifica(objFiltriEstrazioni.NumeroCopieStampe, AgroKey_EncoderDecoder, Nothing)

                        obj("queryString") = queryString
                        obj("paginaDaRichiamare") = VirtualPathUtility.ToAbsolute("~/GestioneStampe/Contabilita/Fattura/BollaAccettazione.aspx")
                        obj("paginaTitolo") = "BollaAccettazione"
                        r.RispostaStringa = obj.ToString()
                        r.RispostaOK = True
                    Else
                        r.Errore = New JArray(mexErrore).ToString
                    End If

                                '=========================================================================================

                    '8) STAMPA MASSIVA CERTIFICATI POMODORO
                Case enum_CodificaStampe.Conf_Certificato_Pomodoro

                    Dim elenIdAgenda As String = ""
                    Dim mexErrore = ElencoIdAgenda_StampaMassiva_BolleCertificati(objParametri_Server, objFiltriEstrazioni, elenIdAgenda)

                    If mexErrore = "" Then
                        HttpContext.Current.Session("Str_Id_Agenda_Certificati") = elenIdAgenda

                        queryString = "p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                      "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                                      "&l=" & Stringa_Codifica(LAVCOD_ACCETTAZIONE_DIVERSI, AgroKey_EncoderDecoder, Nothing) &
                                      "&ptp=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Nothing) &
                                      "&pn=" & Stringa_Codifica(objFiltriEstrazioni.Stampante, AgroKey_EncoderDecoder, Nothing) &
                                      "&nc=" & Stringa_Codifica(objFiltriEstrazioni.NumeroCopieStampe, AgroKey_EncoderDecoder, Nothing)

                        obj("queryString") = queryString
                        obj("paginaDaRichiamare") = VirtualPathUtility.ToAbsolute("~/GestioneStampe/Conferimenti/CertificatoPomodoro/CertificatoPomodoro.aspx")
                        obj("paginaTitolo") = "CertificatoPomodoro"
                        r.RispostaStringa = obj.ToString()
                        r.RispostaOK = True
                    Else
                        r.Errore = New JArray(mexErrore).ToString
                    End If
                                '=========================================================================================

                    '9) ESPORTAZIONE EXCEL CERTIFICATI POMODORO
                Case enum_CodificaStampe.Conf_Esportazione_CertificatiPomodoro_XLS

                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                "&rs=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Nothing) &
                                "&s=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlCentriAziendali), AgroKey_EncoderDecoder, Nothing) &
                                "&sn=" & Stringa_Codifica(objFiltriEstrazioni.centroAziendaleDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                "&spe=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlSpecie), AgroKey_EncoderDecoder, Nothing) &
                                "&var=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlVarieta), AgroKey_EncoderDecoder, Nothing) &
                                "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                "&fce=" & Stringa_Codifica(CStr(objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno), AgroKey_EncoderDecoder, Nothing) &
                                "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing) &
                                "&ti=" & Stringa_Codifica(objFiltriEstrazioni.kSwitchTracciabilitaImpianti, AgroKey_EncoderDecoder, Nothing) &
                                "&pp=" & Stringa_Codifica(objFiltriEstrazioni.produttorePiva, AgroKey_EncoderDecoder, Nothing) &
                                "&pc1=" & Stringa_Codifica(objFiltriEstrazioni.primoCessionarioPiva, AgroKey_EncoderDecoder, Nothing) &
                                "&pc2=" & Stringa_Codifica(objFiltriEstrazioni.secondoCessionarioPiva, AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = VirtualPathUtility.ToAbsolute("~/GestioneStampe/Conferimenti/CertificatiPomodoro_XLS/CertificatiPomodoro_XLS.aspx")
                    obj("paginaTitolo") = "CertificatiPomodoroExcel"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                                '=========================================================================================

                    'RIEPILOGO CONFERIMENTI (REPORT IBRIDO DI PIU' GESTIONI CONFERIMENTO - NON VERRA' USATO DA FRUTTAGEL)
                Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti
                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) +
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) +
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) +
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) +
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) +
                                  "&s=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Nothing) +
                                  "&f=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Nothing) +
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) +
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) +
                                  "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) +
                                  "&mag=" & Stringa_Codifica(objFiltriEstrazioni.magazzinoDescrizione, AgroKey_EncoderDecoder, Nothing) +
                                  "&cm=" & Stringa_Codifica(CStr(objFiltriEstrazioni.hdIntConfigurazioneModuli), AgroKey_EncoderDecoder, Nothing) +
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "Riepilogo_Conf/Riepilogo_Conf.aspx"
                    obj("paginaTitolo") = "RiepilogoConferimenti"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                      '=========================================================================================

                    'REPORT DISMESSO
                Case enum_CodificaStampe.Conf_Tracciabilita
                    'HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti
                    'queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                    '              "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                    '              "&cc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                    '              "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                    '              "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                    '              "&s=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Nothing) &
                    '              "&f=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Nothing) &
                    '              "&m=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlProdotti), AgroKey_EncoderDecoder, Nothing) &
                    '              "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                    '              "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                    '              "&cm=" & Stringa_Codifica(CStr(objFiltriEstrazioni.hdIntConfigurazioneModuli), AgroKey_EncoderDecoder, Nothing) &
                    '              "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing)

                    'obj("queryString") = queryString
                    'obj("paginaDaRichiamare") = "Tracciabilita_Conf/Tracciabilita_Conf_XLS.aspx"
                    'obj("paginaTitolo") = "TracciabilitaConferimenti"
                    'r.RispostaStringa = obj.ToString()

                    r.RispostaStringa = "Report dismesso-> usare export excel bolle + check tracciabilità impianti"
                    r.RispostaOK = True

                      '=========================================================================================

                    'ESPORTAZIONE CONFERIMENTI SU CSV (USATO DA MINI FRUTTA)
                Case 666
                    Dim strJs As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                        Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                        enum_PagineAgronicaSincro.EsportazioneConferimentiFF,
                        0,
                        objFiltriEstrazioni.hdPiva,
                        0)

                    Dim xRedir As String() = strJs.Split("'")
                    Dim urlRedirect As String() = xRedir(3).Split("?")

                    obj("queryString") = urlRedirect(1)
                    obj("paginaDaRichiamare") = urlRedirect(0)
                    obj("paginaTitolo") = "ExportCSV"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                    '=========================================================================================
                    
                'LETTERA COMUNICAZIONE AI CONFERENTI FRUTTAGEL
                Case enum_CodificaStampe.Conf_ComunicazioneCredito

                    HttpContext.Current.Session("Str_CodRisUm_Conferenti") = objFiltriEstrazioni.elencoConferenti

                    queryString = "dd=" & Stringa_Codifica(objFiltriEstrazioni.dpDataInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&da=" & Stringa_Codifica(objFiltriEstrazioni.dpDataFine.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing) &
                                  "&p=" & Stringa_Codifica(objFiltriEstrazioni.hdPiva, AgroKey_EncoderDecoder, Nothing) &
                                  "&rs=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Nothing) &
                                  "&s=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlCentriAziendali), AgroKey_EncoderDecoder, Nothing) &
                                  "&sn=" & Stringa_Codifica(objFiltriEstrazioni.centroAziendaleDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&spe=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlSpecie), AgroKey_EncoderDecoder, Nothing) &
                                  "&var=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlVarieta), AgroKey_EncoderDecoder, Nothing) &
                                  "&m=" & Stringa_Codifica(mat_cod_QS, AgroKey_EncoderDecoder, Nothing) &
                                  "&ca=" & Stringa_Codifica(objFiltriEstrazioni.prodottoCodArticolo, AgroKey_EncoderDecoder, Nothing) &
                                  "&fce=" & Stringa_Codifica(CStr(objFiltriEstrazioni.Flag_EsisteGestioneCodEsterno), AgroKey_EncoderDecoder, Nothing) &
                                  "&dpr=" & Stringa_Codifica(objFiltriEstrazioni.prodottoDescrizione, AgroKey_EncoderDecoder, Nothing) &
                                  "&ru=" & Stringa_Codifica(CStr(objFiltriEstrazioni.conferenteCodRisum), AgroKey_EncoderDecoder, Nothing) &
                                  "&cc=" & Stringa_Codifica(objFiltriEstrazioni.conferente_Progressivo_SettoreDes, AgroKey_EncoderDecoder, Nothing) &
                                  "&rsc=" & Stringa_Codifica(objFiltriEstrazioni.conferenteRagSoc, AgroKey_EncoderDecoder, Nothing) &
                                  "&rc=" & Stringa_Codifica(CStr(objFiltriEstrazioni.ddlRapportiContabili), AgroKey_EncoderDecoder, Nothing)

                    obj("queryString") = queryString
                    obj("paginaDaRichiamare") = "ComunicazioneCredito/ComunicazioneCredito.aspx"
                    obj("paginaTitolo") = "ComunicazioneCredito"
                    r.RispostaStringa = obj.ToString()
                    r.RispostaOK = True

                    '=========================================================================================

            End Select

        Else
            ' mando l'errore di filtro all'utente
            r = rispostaValFiltri
        End If

        Return r
    End Function

    ''' <summary>
    ''' La funzione si occupa di impostare i valori di default ai filtri non valorizzati e segnalare eventuali errori
    ''' </summary>
    ''' <param name="objFiltriEstrazioni">Oggetto con i filtri impostati dall'utente</param>
    ''' <returns>RispostaStandard; in caso di errori li restituisce come array json nella proprietà 'Errore'</returns>
    Private Shared Function ValidaFiltri(objFiltriEstrazioni As FiltriEstrazioni) As RispostaStandard
        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        r.RispostaOK = True
        'Dim listElencoErrori As New List(Of String)
        Dim jsonArrErrori As New JArray

        'date abilitate per tutti i report

        'Se una proprietà di tipologia Integer non è stata passata, automaticamente ottiene il valore di default 0
        'mentre una proprietà String ha di default Nothing, di conseguenza effettuo i controlli per impostarle a stringa vuota
        Dim Flag_Prodotto As Boolean = False
        Dim Flag_Magazzino As Boolean = False

        'Non occorre un controllo sul filtro "dettaglio stabilimento" perché essendo una kendoSwitch è al limite false
        'Dim Flag_DettaglioStabilimento As Boolean = False

        Dim Flag_DataGiacenza As Boolean = False

        'Filtro sulle tipologie dei rapporti contabili
        Dim Flag_RapportoContabile As Boolean = False

        'Non occorre un controllo sul filtro "tracciabilità impianti" perché essendo una kendoSwitch è al limite false
        'Dim Flag_TracciabilitaImpianti As Boolean = False

        Dim Flag_CentroAziendale As Boolean = False
        Dim Obbligatorio_CentroAziendale As Boolean = False

        'Filtro sui contatti "conferenti"(/"clienti")
        Dim Flag_ConfCli As Boolean = False

        Dim Flag_PrimoCess As Boolean = False
        Dim Flag_SecondoCess As Boolean = False

        'Filtro sui contatti "produttori"
        Dim Flag_Produttore As Boolean = False

        Dim Flag_Specie As Boolean = False
        Dim Flag_Varieta As Boolean = False

        Dim Flag_RangeBolle As Boolean = False
        
        Dim Flag_ParametriStampa As Boolean = False

        Select Case objFiltriEstrazioni.ddlEstrazioni
            Case enum_CodificaStampe.Conf_RiepilogoxArticolo
                Flag_CentroAziendale = True
                Obbligatorio_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_EC_Bolle
                Flag_CentroAziendale = True
                Obbligatorio_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_EC_Imballi
                Flag_Magazzino = True
                Flag_CentroAziendale = True
                Obbligatorio_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_Saldo_Imballi
                Flag_Magazzino = True
                Flag_CentroAziendale = True
                Obbligatorio_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Prodotto = True
                Flag_DataGiacenza = True
            'Report dismesso
            'Case enum_CodificaStampe.Conf_Tracciabilita
            '    Flag_ConfCli = True
            '    Flag_RapportoContabile = True
            '    Flag_Specie = True
            '    Flag_Varieta = True
            '    Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_Esportazione_BolleFF_XLS
                'Report nuovo
                Flag_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Produttore = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
                'Questi ultimi due se presente nel superuser impostazione di gerarchia
                Flag_PrimoCess = True
                Flag_SecondoCess = True
            Case enum_CodificaStampe.Conf_EsportazionexTrasportatori_XLS
                Flag_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.FreshFood_BollaAccettazione
                Flag_RangeBolle = True
                Flag_ParametriStampa = True
                Flag_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_Certificato_Pomodoro
                Flag_RangeBolle = True
                Flag_ParametriStampa = True
                Flag_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_Esportazione_CertificatiPomodoro_XLS
                Flag_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Produttore = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
                'Questi ultimi due se presente nel superuser impostazione di gerarchia
                Flag_PrimoCess = True
                Flag_SecondoCess = True
            Case enum_CodificaStampe.Conf_Riepilogo_Conferimenti
                Flag_ConfCli = True
                Flag_RapportoContabile = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
            Case enum_CodificaStampe.Conf_ComunicazioneCredito
                Flag_CentroAziendale = True
                Obbligatorio_CentroAziendale = True
                Flag_ConfCli = True
                Flag_Specie = True
                Flag_Varieta = True
                Flag_Prodotto = True
        End Select

        '----------------------------
        '-------- DATE --------------
        '----------------------------
        If IsNothing(objFiltriEstrazioni.dpDataInizio) Then
            objFiltriEstrazioni.dpDataInizio = AGRODATAINIZIO
        End If
        If IsNothing(objFiltriEstrazioni.dpDataFine) Then
            objFiltriEstrazioni.dpDataFine = AGRODATAFINE
        End If
        If objFiltriEstrazioni.dpDataInizio > objFiltriEstrazioni.dpDataFine Then
            jsonArrErrori.Add("La data di fine deve essere maggiore o uguale della data di inizio")
        End If

        '----------------------------
        '-------- CENTRO AZ ---------
        '----------------------------
        If Flag_CentroAziendale = True And objFiltriEstrazioni.ddlCentriAziendali = 0 Then
            If Obbligatorio_CentroAziendale = True Then
                jsonArrErrori.Add("Centro Aziendale obbligatorio per l'estrazione selezionata")
            End If
            objFiltriEstrazioni.centroAziendaleDescrizione = ""
        End If

        '----------------------------
        '-------- MAGAZZINO ---------
        '----------------------------
        If Flag_Magazzino = True And (IsNothing(objFiltriEstrazioni.ddlMagazzini) Or objFiltriEstrazioni.ddlMagazzini = "") Then
            'jsonArrErrori.Add("Magazzino obbligatorio per l'estrazione selezionata")
            'Viene passato key_Dest che è formato nel seguente modo:
            ' dr.Item("key_Dest") = drFab.Item("Tipo_Fabbricato_Cod").ToString & "_" & drFab.Item("Sa_Cod").ToString & "_" & drFab.Item("Fabbricato_Cod").ToString
            objFiltriEstrazioni.ddlMagazzini = "0_0_0"
        End If

        '----------------------------
        '-------- GIACENZA ----------
        '----------------------------
        If Flag_DataGiacenza = True And IsNothing(objFiltriEstrazioni.dpDataGiacenza) Then
            objFiltriEstrazioni.dpDataGiacenza = Date.Today
        End If

        '----------------------------
        '-------- CONFERENTE --------
        '----------------------------
        If Flag_ConfCli = True And objFiltriEstrazioni.conferenteCodRisum = 0 Then
            'jsonArrErrori.Add("Conferente obbligatorio per l'estrazione selezionata")
            objFiltriEstrazioni.conferentePiva = ""
            objFiltriEstrazioni.conferenteRagSoc = ""
            objFiltriEstrazioni.conferente_Progressivo_SettoreDes = ""
        End If

        '----------------------------
        '-------- PRODOTTO ----------
        '----------------------------
        If Flag_Prodotto = True And objFiltriEstrazioni.ddlProdotti = 0 Then
            'jsonArrErrori.Add("Prodotto obbligatorio per l'estrazione selezionata")
            objFiltriEstrazioni.prodottoCodArticolo = ""
            objFiltriEstrazioni.prodottoDescrizione = ""
        End If

        '----------------------------
        '-------- SPECIE ------------
        '----------------------------
        If Flag_Specie = True And objFiltriEstrazioni.ddlSpecie = -1 Then
            objFiltriEstrazioni.ddlSpecie = 0
        End If

        '----------------------------
        '------- RANGE BOLLE --------
        '----------------------------
        If Flag_RangeBolle = True Then
            If objFiltriEstrazioni.DocPrimoNumero > objFiltriEstrazioni.DocUltimoNumero Then
                jsonArrErrori.Add("Nell'intervallo dei documenti da stampare, 'A numero' deve essere maggiore o uguale di 'Da numero'")
            End If

            'Nel caso sia aggiunta l'opzione vuota ("-999") sulla pagina per sin/des verificare i numeri documento non siano valorizzati se scelta quell'opzione
        End If

        '----------------------------
        '--- PARAMETRI DI STAMPA ----
        '----------------------------
        If Flag_ParametriStampa = True Then
            If String.IsNullOrWhiteSpace(objFiltriEstrazioni.Stampante) = True Then
                jsonArrErrori.Add("Scegliere una stampante")
            End If

            If objFiltriEstrazioni.NumeroCopieStampe < 1 Then
                jsonArrErrori.Add("Inserire un numero di copie da stampare maggiore od uguale ad 1")
            End If
        End If


        If jsonArrErrori.Count > 0 Then
            r.RispostaOK = False
            r.Errore = jsonArrErrori.ToString()
        End If

        Return r
    End Function


    '##################################################################################
    Private Shared Function ElencoIdAgenda_StampaMassiva_BolleCertificati(ByRef objParametri As AgronicaCoreParametri, ByRef objFiltri As FiltriEstrazioni, ByRef out_elencoIdAgenda As String) As String

        Dim mexErrore As String = ""

        Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni

        'Indica se l'elenco restituito deve essere separato da pipe (impostato a false) oppure da virgole per poter essere usato in una clausola di IN
        Dim flagElencoIdAgendaSqlIn As Boolean = False

        'Indica se applicare la condizione di where per il range dei numeri documento. Utilizzo 0.1 come valore speciale 'vuoto'
        Dim flagFiltraNumBolla As Boolean = True
        If objFiltri.DocPrimoNumero = 0.1 And objFiltri.DocUltimoNumero = 0.1 Then
            flagFiltraNumBolla = False
        End If

        Try
            Select Case objFiltri.ddlEstrazioni

                Case enum_CodificaStampe.FreshFood_BollaAccettazione

                    Dim FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(objFiltri.ElencoProdotti)
                    Dim FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(objFiltri.elencoConferenti)

                    objConfFun.Prepara_StrFiltroBolle(
                        objParametri,
                        out_elencoIdAgenda,
                        mexErrore,
                        enum_CodificaStampe.FreshFood_BollaAccettazione,
                        flagElencoIdAgendaSqlIn,
                        objFiltri.hdPiva,
                        objFiltri.ddlCentriAziendali,
                        flagFiltraNumBolla,
                        objFiltri.DocPrefisso,
                        objFiltri.DocSuffisso,
                        objFiltri.DocPrimoNumero,
                        objFiltri.DocUltimoNumero,
                        objFiltri.dpDataInizio,
                        objFiltri.dpDataFine,
                        objFiltri.conferenteCodRisum,
                        objFiltri.ddlProdotti,
                        objFiltri.ddlSpecie,
                        objFiltri.ddlVarieta,
                        FiltroAggArticoli,
                        FiltroAggConferenti,
                        objFiltri.produttorePiva,
                        objFiltri.primoCessionarioPiva,
                        objFiltri.secondoCessionarioPiva
                    )

                    '=================================================================

                Case enum_CodificaStampe.Conf_Certificato_Pomodoro

                    'mexErrore = "Report in Costruzione"

                    Dim FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(objFiltri.ElencoProdotti)
                    Dim FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(objFiltri.elencoConferenti)

                    objConfFun.Prepara_StrFiltroBolle(
                        objParametri,
                        out_elencoIdAgenda,
                        mexErrore,
                        enum_CodificaStampe.Conf_Certificato_Pomodoro,
                        flagElencoIdAgendaSqlIn,
                        objFiltri.hdPiva,
                        objFiltri.ddlCentriAziendali,
                        flagFiltraNumBolla,
                        objFiltri.DocPrefisso,
                        objFiltri.DocSuffisso,
                        objFiltri.DocPrimoNumero,
                        objFiltri.DocUltimoNumero,
                        objFiltri.dpDataInizio,
                        objFiltri.dpDataFine,
                        objFiltri.conferenteCodRisum,
                        objFiltri.ddlProdotti,
                        objFiltri.ddlSpecie,
                        objFiltri.ddlVarieta,
                        FiltroAggArticoli,
                        FiltroAggConferenti,
                        objFiltri.produttorePiva,
                        objFiltri.primoCessionarioPiva,
                        objFiltri.secondoCessionarioPiva
                    )




                    'CODICE COPIATO DALLA VERSIONE PRECEDENTE DELLA PAGINA:

                    'Dim Str_Id_Agenda_Cert As String
                    'Dim Prefisso_DA_Bolla As String
                    'Dim Prefisso_A_Bolla As String

                    ''------------------------------------------------
                    ''------------- NUMERI CERTIFICATI ---------------
                    ''------------------------------------------------

                    ''Me.Txt_Prefisso_DaNumeroBolla.Text
                    'Prefisso_DA_Bolla = Me.Cmb_Prefisso_DaNumeroBolla.SelectedValue
                    ''Me.Txt_Prefisso_ANumeroBolla.Text
                    'Prefisso_A_Bolla = Me.Cmb_Prefisso_ANumeroBolla.SelectedValue

                    ''modifica in data 15/07/2010: leggo gli id_agenda dal numero bolla
                    ''non dal numero certificato, perchè non sono ancora valorizzati
                    'Str_Id_Agenda_Cert = objADDFun.Verifica_Filtro_Bolle(objParametri_Server,
                    '                                            Session("ReportSelezionato"),
                    '                                            Messaggio,
                    '                                            False,
                    '                                            Qs_Piva,
                    '                                            TipoReport,
                    '                                            Prefisso_DA_Bolla, Me.Txt_DaNumeroBolla.Text, Me.Txt_Suffisso_DaNumeroBolla.Text,
                    '                                            Prefisso_A_Bolla, Me.Txt_ANumeroBolla.Text, Me.Txt_Suffisso_ANumeroBolla.Text,
                    '                                            Validita_Inizio, Validita_Fine,
                    '                                            Codice_Specie,
                    '                                            Str_Codici_Specie,
                    '                                            Codice_Conferente,
                    '                                            Str_Codici_Conferenti,
                    '                                            Piva_Produttore,
                    '                                            Piva_Coop1,
                    '                                            Piva_Coop2)

                    'If Messaggio <> "" Then
                    '    AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Messaggio, Page)
                    '    Exit Sub
                    'End If

                    'Session("Str_Id_Agenda_Certificati") = Str_Id_Agenda_Cert


                    'Querystring = "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                    '            "&i=" + Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) +
                    '            "&l=" + Stringa_Codifica(CStr(LAVCOD_ACCETTAZIONE_DIVERSI), AgroKey_EncoderDecoder, Server) +
                    '            "&ptp=" + Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) +
                    '            "&pn=" + Stringa_Codifica(CStr(Nome_Stampante), AgroKey_EncoderDecoder, Server) +
                    '            "&ff=" + Stringa_Codifica(Str_Flag_Fascicola, AgroKey_EncoderDecoder, Server) +
                    '            "&nc=" + Stringa_Codifica(CStr(Numero_Copie), AgroKey_EncoderDecoder, Server)



                    'Dim Pagina_CertificatoPomodoro As String = objADDFun.PaginaCertificatoPomodoro_from_Str_Id_Agenda(objParametri_Server,
                    '                                                                                            Qs_Piva,
                    '                                                                                            Str_Id_Agenda_Cert)
                    '    Page_NewWindow_2010(Page,
                    '                    Pagina_CertificatoPomodoro,
                    '                    Querystring,
                    '                    "CertificatiPomodoro",
                    '                    , , , , , , , )



            End Select

        Catch ex As Exception
            mexErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return mexErrore

    End Function

End Class


Class FiltriEstrazioni
    Public Property hdPiva() As String
    Public Property hdIntConfigurazioneModuli() As Integer
    Public Property ddlEstrazioni() As enum_CodificaStampe
    Public Property dpDataInizio() As Date?
    Public Property dpDataFine() As Date?
    ''' <summary>
    ''' Mat_cod
    ''' </summary>
    ''' <returns></returns>
    Public Property ddlProdotti() As Integer
    Public Property ddlMagazzini() As String
    Public Property kSwitchDettaglioStabilimento() As Boolean
    Public Property dpDataGiacenza() As Date?
    Public Property ddlRapportiContabili() As Integer
    Public Property kSwitchTracciabilitaImpianti() As Boolean
    ''' <summary>
    ''' Sa_cod
    ''' </summary>
    ''' <returns></returns>
    Public Property ddlCentriAziendali() As Integer
    ''' <summary>
    ''' Cod_risum
    ''' </summary>
    ''' <returns></returns>
    Public Property conferenteCodRisum() As Integer
    Public Property elencoConferenti() As String
    Public Property ddlPrimiCessionari() As Integer
    Public Property ddlSecondiCessionari() As Integer
    Public Property ddlProduttori() As Integer
    ''' <summary>
    ''' Veg_cod
    ''' </summary>
    ''' <returns></returns>
    Public Property ddlSpecie() As Integer
    ''' <summary>
    ''' Cul_cod
    ''' </summary>
    ''' <returns></returns>
    Public Property ddlVarieta() As Integer
    Public Property conferentePiva() As String
    Public Property conferenteRagSoc() As String
    ''' <summary>
    ''' Codice conferente
    ''' </summary>
    ''' <returns></returns>
    Public Property conferente_Progressivo_SettoreDes() As String
    Public Property primoCessionarioPiva() As String
    Public Property secondoCessionarioPiva() As String
    Public Property produttorePiva() As String
    Public Property prodottoCodArticolo() As String
    Public Property prodottoDescrizione() As String
    Public Property magazzinoDescrizione() As String
    Public Property centroAziendaleDescrizione() As String
    Public Property ElencoProdotti() As String
    Public Property DocPrefisso() As String
    Public Property DocSuffisso() As String
    Public Property DocPrimoNumero() As Double
    Public Property DocUltimoNumero() As Double
    Public Property Stampante() As String
    Public Property NumeroCopieStampe() As Integer

    'Filtri calcolati non provenienti dalla pagina
    Public Property Flag_EsisteGestioneCodEsterno() As Integer

End Class