
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


Public Class Filtro_Fattura
    Inherits System.Web.UI.Page

#Region " Filtro Stampe Fatture "

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
    Dim Qs_Lav_Cod As Integer
    Dim Qs_Cod_Report As Integer
    '\\zaffiro\HP LaserJet P2015 (Agronica)|\\zaffiro\HP LaserJet 2300L



    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione

    ''##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0
        Master.Lbl_Titolo.Text = "Filtro Stampa Documenti"


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        'If Session("ASG_Utente_Username") = "" Then
        '    Dim strClose As String = "<script language='javascript'>window.close()</script>"
        '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        'End If

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
            
            Qs_IdAgenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))

            Qs_Lav_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))

            Qs_Cod_Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), _
                                     AgroKey_EncoderDecoder, _
                                     Server))


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

            '20160616
            'Questo andrà in Tabella personalizzazioni 
            Select Case Qs_Cod_Report
                Case enum_CodificaStampe.DDT_Contabilizzato_Emesso,
                   enum_CodificaStampe.Bolle
                    Riga_TipoView.Visible = False

            End Select


        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Problemi durante il caricamento della pagina: " + vbCrLf + ex.Message, Page, "MainContent", True)
        End Try


    End Sub

    ''##################################################################################
    Private Sub Rbl_TipoView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_TipoView.SelectedIndexChanged


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

            Dim TipoView As String = ""
            Dim TipoOuput As String = ""


            Coltrolla_RecuperaFiltri(Log_Errori, TipoView, TipoOuput)


            If Log_Errori <> "" Then
                AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010(Log_Errori, Page, "MainContent", True)
                Exit Sub
            End If


            QueryString = _
                            "?p=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) + _
                            "&i=" + Stringa_Codifica(CStr(Qs_IdAgenda), AgroKey_EncoderDecoder, Server) + _
                            "&l=" + Stringa_Codifica(CStr(Qs_Lav_Cod), AgroKey_EncoderDecoder, Server) + _
                            "&rep=" + Stringa_Codifica(Qs_Cod_Report, AgroKey_EncoderDecoder, Server) + _
                            "&ts=" + Stringa_Codifica(CStr(TipoView), AgroKey_EncoderDecoder, Server) + _
                            "&to=" + Stringa_Codifica(CStr(TipoOuput), AgroKey_EncoderDecoder, Server)


            Page_NewWindow_2010(Page, _
                            "Fattura/Fattura_NotaAccredito.aspx", _
                            QueryString, _
                            "Documento", _
                            , , , , , , , "MainContent", True)



        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider_2010.AgroMsgBox_2010("Si è verificato il seguente errore durante la chiamata alla stampa: " + ex.Message, Page, "MainContent", True)
        End Try



    End Sub



    ''##################################################################################
    Private Sub Coltrolla_RecuperaFiltri( _
                                         ByRef Log_Errori As String, _
                                         ByRef TipoView As String, _
                                         ByRef TipoOuput As String)


        TipoView = Me.Rbl_TipoView.SelectedValue
        TipoOuput = Me.Rbl_TipoOutput.SelectedValue



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


