Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports System.Management
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.Drawing
Imports AgronicaCoreUtility.CaricaListControl


Public Class Filtro_Stampe_Cespiti
    Inherits System.Web.UI.Page

#Region " Filtro Stampe Cespiti "

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
    Dim Sa_Cod As Long = 0
    Dim Qs_IdAgenda As Integer
    '\\zaffiro\HP LaserJet P2015 (Agronica)|\\zaffiro\HP LaserJet 2300L



    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione

    ''##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0
        Master.Lbl_Titolo.Text = "Filtro Registro Cespiti"

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Try


            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)



            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


            '**************************************************************************************************
            '*** IL FILTRO SI CONFIGURA IN BASE AI MODULI ATTIVI 
            '**************************************************************************************************
            'Configurazione per F&F : conferimento DA (es. LaBuonaRomagna)
            'Altri casi (al momento si escudono le cantine): conferimento A  (es. Agrisfera) 
            '**************************************************************************************************

            int_Configurazione_Moduli = ConfigurazioneModuli()


            '        '============================================================
            '        '       Sono in postback
            '        '============================================================
            '        If Me.IsPostBack Then



        Catch ex As Exception
            '''Me.Rbl_CertificatiStampaMassiva.SelectedValue = 1
            '''Me.Rbl_CertificatiStampaMassiva_SelectedIndexChanged(Me, Nothing)
            '''Me.Rbl_Certificati.SelectedValue = 1
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Problemi durante il caricamento della pagina: " + vbCrLf + ex.Message, Page, "MainContent", True)
        End Try


    End Sub

    ''##################################################################################
    Private Sub Rbl_TipoStampa_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_TipoStampa.SelectedIndexChanged

        'Imposta_Filtri_Report(Me.Rbl_TipoStampa.SelectedValue)

    End Sub


    '##################################################################################

    Private Sub Imposta_Filtri_Report(ByVal Report As enum_CodificaStampe)

        'AbilitaGiacenza(False)
        'AbilitaMagazzino(False)

    End Sub


    ''##################################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Stampa()

    End Sub


    ''##################################################################################
    Private Sub Stampa()


        Try

            Dim QueryString As String
            Dim Log_Errori As String = ""

            Dim Str_Flag_Fascicola As String
            Dim Numero_Copie As Integer

            Dim RagSoc_Impresa As String = ""
            Dim Flag_SalvaPagRiga As Boolean

            Dim TipoStampa As String = ""
            Dim Anno As Integer = 0

            Coltrolla_RecuperaFiltri(Log_Errori, TipoStampa, Anno)


            If Log_Errori <> "" Then
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(Log_Errori, Page, "MainContent", True)
                Exit Sub
            End If


            QueryString = _
                            "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                            "&s=" + Stringa_Codifica(CStr(Sa_Cod), AgroKey_EncoderDecoder, Server) + _
                            "&rs=" + Stringa_Codifica(QS_SaveText(CStr(Qs_RagSoc)), AgroKey_EncoderDecoder, Server) + _
                            "&cm=" + Stringa_Codifica(CStr(int_Configurazione_Moduli), AgroKey_EncoderDecoder, Server) + _
                            "&an=" + Stringa_Codifica(CStr(Anno), AgroKey_EncoderDecoder, Server) + _
                            "&ts=" + Stringa_Codifica(CStr(TipoStampa), AgroKey_EncoderDecoder, Server)


            Page_NewWindow_2010(Page, _
                            "RegistroCespiti/RegistroCespiti.aspx", _
                            QueryString, _
                            "RegistroCespiti", _
                            , , , , , , , "MainContent", True)



        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Si è verificato il seguente errore durante la chiamata alla stampa: " + ex.Message, Page, "MainContent", True)
        End Try



    End Sub


    ''##################################################################################
    Private Sub Controlla_ValiditaInizioFine(ByRef Log_Errori As String, _
                                                ByRef Validita_Inizio As String, _
                                                ByRef Validita_Fine As String)

        'If Me.Txt_ValiditaInizio.Text <> "" Then
        '    Validita_Inizio = Me.Txt_ValiditaInizio.Text
        'Else
        '    Validita_Inizio = AGRODATAINIZIO
        'End If

        'If Not IsDate(Validita_Inizio) Then
        '    If Validita_Inizio <> "" Then
        '        Log_Errori = "E' necessario specificare la Data DA nel formato data corretto!" + vbCrLf
        '    End If
        'End If

    End Sub


    ''##################################################################################
    Private Sub Coltrolla_RecuperaFiltri( _
                                         ByRef Log_Errori As String, _
                                         ByRef TipoStampa As String, _
                                         ByRef Anno As Integer)



        If (Not IsNumeric(Me.txt_anno.Text)) Or (Me.txt_anno.Text.Length <> 4) Then
            Log_Errori = "Indicare un anno valido!" + vbCrLf
        Else
            Anno = Me.txt_anno.Text
        End If

        TipoStampa = Me.Rbl_TipoStampa.SelectedValue



        '''Dim Messaggio_Conf As String = ""


        '''Dim Flag_ConfCli As Boolean = False


        '''Dim Flag_Prodotto As Boolean = False
        '''Dim Flag_Magazzino As Boolean = False

        '''Dim Flag_DataGiacenza As Boolean = False
        '''Dim Flag_Regolamento As Boolean = False

        ''''l'intervallo temporale è per tutti i report

        '''Controlla_ValiditaInizioFine(Log_Errori, _
        '''                            Validita_Inizio, _
        '''                            Validita_Fine)

        '''Select Case Report


        '''    '------------------------------------
        '''    Case enum_CodificaStampe.Conf_EC_Imballi

        '''        Flag_Magazzino = True
        '''        Flag_ConfCli = True
        '''        Flag_Prodotto = True
        '''        Flag_Prodotto = True



        '''End Select


        ''''----------------------------
        ''''-------- PRODOTTO ----------
        ''''----------------------------
        '''If Flag_Prodotto = True Then

        '''    If (Me.Txt_Filtro_MatDes.Text) <> "" Then
        '''        Descr_Prodotto = Me.Txt_Filtro_MatDes.Text
        '''    End If
        '''    If Me.Txt_Filtro_CodArticolo.Text <> "" Then
        '''        Cod_Articolo = Me.Txt_Filtro_CodArticolo.Text
        '''    End If

        '''    If Not IsNothing(Me.Cmb_Prodotti.SelectedItem) Then
        '''        If Me.Cmb_Prodotti.SelectedValue <> "0" Then
        '''            Mat_Cod = Me.Cmb_Prodotti.SelectedValue
        '''        End If

        '''        'Log_Errori = "E' necessario selezionare il magazzino!" + vbCrLf
        '''    End If

        '''End If




        '''''------------------------------------------------
        '''''----------- OPZIONI DI STAMPA ------------------
        '''''------------------------------------------------
        ''''If Flag_OpzioniStampa = True Then
        ''''    'viene fatto dopo

        ''''    Str_Flag_Fascicola = CStr(Me.Chk_Fascicola.Checked)

        ''''    If Me.Txt_NumeroCopie.Text <> "" Then
        ''''        Numero_Copie = CInt(Me.Txt_NumeroCopie.Text)
        ''''    Else
        ''''        Numero_Copie = 1
        ''''    End If
        ''''End If



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

End Class


