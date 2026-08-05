Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class GestioneNumerazioneCertificati
    Inherits System.Web.UI.Page

#Region " NUMERAZIONE DEI CERTIFICATI "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents LblTitolo As System.Web.UI.WebControls.Label
    Protected WithEvents ImgIcona As System.Web.UI.WebControls.Image
    Protected WithEvents ImgBtnEsci As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Label1 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_DaInviare As System.Web.UI.WebControls.TextBox
    Protected WithEvents Label35 As System.Web.UI.WebControls.Label
    Protected WithEvents Label34 As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_NumerazioneCertificati As System.Web.UI.WebControls.Panel
    Protected WithEvents Btn_ImpostaNumerazione As System.Web.UI.WebControls.Button
    Protected WithEvents Label2 As System.Web.UI.WebControls.Label
    Protected WithEvents Label3 As System.Web.UI.WebControls.Label
    Protected WithEvents Txt_UltimoNumCertificatoValorizzato As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_UltimoNumBollaValorizzato As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_UltimoNumBollaDaValorizzare As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_UltimoNumBollaValorizzato_Suffisso As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_UltimoNumBollaDaValorizzare_Suffisso As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_UltimoNumBollaDaValorizzare_Prefisso As System.Web.UI.WebControls.TextBox
    Protected WithEvents Txt_UltimoNumBollaValorizzato_Prefisso As System.Web.UI.WebControls.TextBox
    Protected WithEvents Pannello_Magazzini As System.Web.UI.WebControls.Panel
    Protected WithEvents Txt_Messaggio As System.Web.UI.WebControls.TextBox

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Dim Qs_Piva As String
    Dim QS_Anno As Integer

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '##################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
        'IMPOSTA IL NUMERO DI MINUTI DOPO I QUALI
        'LA PAGINA MEMORIZZATA NELLA CACHE SCADE

        Response.Expires = 0

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


        Dim Str_Errore As String = ""


        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            '----- Verifico che l'utente sia autenticato

            If Session("ASG_objParametri_Server") Is Nothing Then
                Response.Redirect("~/Custom500.aspx")
            End If


            '##############################################################
            '#################  Recupero I DATI DA QUERY STRING  ##########
            '##############################################################

            'Dim Validita_Inizio As String
            'Dim Validita_Fine As String
            'Dim Num_Bolla_Da As String = ""
            Dim Prefisso_Num_Bolla_A As String = ""
            Dim Num_Bolla_A As Double = 0
            Dim Suffisso_Num_Bolla_A As String = ""
            Dim Id_Agenda As Integer

            Qs_Piva = Stringa_Decodifica( _
                                        Request.QueryString("p").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)

            'temporaneamente faccio così
            QS_Anno = Date.Today.Year

            'Validita_Inizio = Stringa_Decodifica( _
            '                  Request.QueryString("vi").ToString, _
            '                  AgroKey_EncoderDecoder, _
            '                  Server)

            'Validita_fine = Stringa_Decodifica( _
            '                  Request.QueryString("vf").ToString, _
            '                  AgroKey_EncoderDecoder, _
            '                  Server)

            Prefisso_Num_Bolla_A = Stringa_Decodifica( _
                     Request.QueryString("pnba").ToString, _
                     AgroKey_EncoderDecoder, _
                     Server)

            Num_Bolla_A = CDbl(Stringa_Decodifica( _
                        Request.QueryString("nba").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server))

            Suffisso_Num_Bolla_A = Stringa_Decodifica( _
                     Request.QueryString("snba").ToString, _
                     AgroKey_EncoderDecoder, _
                     Server)

            Id_Agenda = CInt(Stringa_Decodifica( _
                        Request.QueryString("i").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server))

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))



            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################


            If Not Page.IsPostBack Then

                '==================================
                '===== Pagina appena caricata =====
                '==================================

                Me.Txt_DaInviare.Text = "0"

            Else

                '==========================================
                '===== Pagina ricaricata in POSTBACK
                '==========================================

                Exit Sub


            End If

            '##############################################################
            '#####  Ripristina dati nei controlli  ########################
            '##############################################################

            Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
            Dim UltimoNumCertificatoValorizzato As Double
            Dim UltimoNumBollaValorizzatoPrefisso As String
            Dim UltimoNumBollaValorizzato As Double
            Dim UltimoNumBollaValorizzatoSuffisso As String

            'Recupero l'ultimo numero di certificato valorizzato
            'e il relativo numero di bolla
            Recupera_UltimoNumCertificatoEBolla(UltimoNumCertificatoValorizzato, _
                                                UltimoNumBollaValorizzatoPrefisso, _
                                                UltimoNumBollaValorizzato, _
                                                UltimoNumBollaValorizzatoSuffisso)

            Me.Txt_UltimoNumCertificatoValorizzato.Text = CStr(UltimoNumCertificatoValorizzato)

            Me.Txt_UltimoNumBollaValorizzato_Prefisso.Text = UltimoNumBollaValorizzatoPrefisso
            Me.Txt_UltimoNumBollaValorizzato.Text = CStr(UltimoNumBollaValorizzato)
            Me.Txt_UltimoNumBollaValorizzato_Suffisso.Text = UltimoNumBollaValorizzatoSuffisso


            If Num_Bolla_A = 0 Then

                If Id_Agenda <> 0 Then
                    'ricava numero bolla da id_agenda
                    'NumeroBolla_from_IdAgenda(Server, Session, Page, _
                    '                            Prefisso_Num_Bolla_A, _
                    '                            Num_Bolla_A, _
                    '                            Suffisso_Num_Bolla_A, _
                    '                            Qs_Piva, _
                    '                            Id_Agenda)

                    objADDFun.NumeroBolla_from_IdAgenda(objParametri_Server, _
                                                Prefisso_Num_Bolla_A, _
                                                Num_Bolla_A, _
                                                Suffisso_Num_Bolla_A, _
                                                Qs_Piva, _
                                                Id_Agenda)

                    If Num_Bolla_A = 0 Then
                        Str_Errore += "Non è stato identificato il numero bolla relativo all'ultimo certificato numerato." + vbCrLf
                    End If
                Else
                    Str_Errore += "Identificativo della bolla non inviato." + vbCrLf
                End If

            End If

            Me.Txt_UltimoNumBollaDaValorizzare_Prefisso.Text = Prefisso_Num_Bolla_A
            Me.Txt_UltimoNumBollaDaValorizzare.Text = CStr(Num_Bolla_A)
            Me.Txt_UltimoNumBollaDaValorizzare_Suffisso.Text = Suffisso_Num_Bolla_A



        Catch ex As Exception
            Str_Errore += ex.Message
        End Try


        If Str_Errore <> "" Then
            Me.Btn_ImpostaNumerazione.Enabled = False
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(Str_Errore + vbCrLf + "Non è possibile procedere con la valorizzazione dei numeri di certificato.", Page)
        End If


    End Sub


    '##############################################################
    Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click

        Dim strClose As String = "<SCRIPT language='javascript'> " + _
                                "window.returnValue = document.all('Txt_DaInviare').value; " + _
                                "window.close(); " + _
                                "</SCRIPT>"

        'Me.Txt_DaInviare.Text = ""


        Me.Controls.Add(New LiteralControl(strClose))


    End Sub



    '########################################################################################
    Private Sub Recupera_UltimoNumCertificatoEBolla(ByRef UltimoNumCertificatoValorizzato As Double, _
                                                    ByRef UltimoNumBollaValorizzatoPrefisso As String, _
                                                    ByRef UltimoNumBollaValorizzato As Double, _
                                                    ByRef UltimoNumBollaValorizzatoSuffisso As String)

        Dim DT_Cert As DataTable
        'Dim FiltroAggiuntivo As String
        Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

        DT_Cert = ADD.CertificatiPomodoro_MaxNumCertificato_Leggi(Qs_Piva, _
                                                                QS_Anno, _
                                                                AGRODATAINIZIO, _
                                                                AGRODATAFINE, _
                                                                "", "", _
                                                                objparametri_server)

        'DT_Cert = NewCom_ADD_CertificatiPomodoro_MaxNumCertificato_Leggi(Server, Session, Page, _
        '                                                                    Qs_Piva, _
        '                                                                    QS_Anno _
        '                                                                     , )

        If Not IsNothing(DT_Cert) AndAlso DT_Cert.Rows.Count > 0 Then

            'prendo il primo record hce è quello con num certificato massimo

            UltimoNumCertificatoValorizzato = DT_Cert.Rows(0).Item("Max_Doc_Numero_Cert")

            UltimoNumBollaValorizzatoPrefisso = DT_Cert.Rows(0).Item("Doc_Numero_Sin_Bolla")
            UltimoNumBollaValorizzatoSuffisso = DT_Cert.Rows(0).Item("Doc_Numero_Des_Bolla")

            If UltimoNumCertificatoValorizzato <> 0 Then
                UltimoNumBollaValorizzato = DT_Cert.Rows(0).Item("Doc_Numero_Bolla")
            End If


        End If



    End Sub


    '##############################################################
    Private Sub Btn_ImpostaNumerazione_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_ImpostaNumerazione.Click

        Imposta_NumerazioneCertificati()

    End Sub


    '##############################################################
    Private Sub Imposta_NumerazioneCertificati()

        Dim DT As DataTable
        Dim i As Integer
        Dim Log As String
        Dim Risp As Boolean = False
        Dim Flag_Errore As Boolean = False
        Dim N_Bolle_DaValorizzare As Integer
        Dim N_Bolle_Valorizzate As Integer = 0
        Dim IdAgenda_to_Update As Integer
        Dim NumeroBolla_to_Update As String
        Dim UltimoNumCertificato As Double
        Dim UltimoNumBollaValorizzatoPrefisso As String
        Dim UltimoNumBollaValorizzato As Double
        Dim UltimoNumBollaValorizzatoSuffisso As String
        Dim UltimoNumBollaDaValorizzare As Double

        Try

            UltimoNumCertificato = Me.Txt_UltimoNumCertificatoValorizzato.Text

            UltimoNumBollaValorizzatoPrefisso = Me.Txt_UltimoNumBollaValorizzato_Prefisso.Text
            UltimoNumBollaValorizzato = Me.Txt_UltimoNumBollaValorizzato.Text
            UltimoNumBollaValorizzatoSuffisso = Me.Txt_UltimoNumBollaValorizzato_Suffisso.Text

            UltimoNumBollaDaValorizzare = Me.Txt_UltimoNumBollaDaValorizzare.Text

            'leggi id_agenda accettazione pomodoro 
            'con materie prime contrattate (no surgelate)
            'con doc_numero certificato =0
            'con doc_numero bolla > ultimo doc_numero bolla valorizzato con certificato
            'con doc_numero bolla <= doc_numero bolla da valorizzare
            'con doc_numero_sin bolla = ultimo doc_numero_sin bolla valorizzato con certificato
            'con doc_numero_des bolla = ultimo doc_numero_des bolla valorizzato con certificato
            'ordinati per doc_numero_sin, doc_numero, doc_numero des (della bolla) ASC

            'DT = NewCom_ADD_CertificatiPomodoroContrattatoXNumerazione_Leggi(Server, Session, Page, _
            '                                                                Qs_Piva, _
            '                                                                QS_Anno, _
            '                                                                UltimoNumBollaValorizzatoPrefisso, _
            '                                                                UltimoNumBollaValorizzato, _
            '                                                                UltimoNumBollaValorizzatoSuffisso, _
            '                                                                UltimoNumBollaDaValorizzare, _
            '                                                                , )

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.CertificatiPomodoroContrattatoXNumerazione_Leggi(Qs_Piva, _
                                                                  QS_Anno, _
                                                                  UltimoNumBollaValorizzatoPrefisso, _
                                                                  UltimoNumBollaValorizzato, _
                                                                  UltimoNumBollaValorizzatoSuffisso, _
                                                                  UltimoNumBollaDaValorizzare, _
                                                                  AGRODATAINIZIO, _
                                                                  AGRODATAFINE, _
                                                                  "", _
                                                                  objParametri_Server)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                '------------------------------------------------
                '----- apro connessione e transazione
                '------------------------------------------------
                Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
                'objParametri_Server = Session("ASG_objParametri_Server")
                objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

                Try

                    Dim obj_ADD As New AgronicaCoreContabDAL.AccettazioneDaDiversi_W

                    N_Bolle_DaValorizzare = DT.Rows.Count

                    Log += "Sono state lette " + CStr(N_Bolle_DaValorizzare) + " bolle da valorizzare." + vbCrLf

                    For i = 0 To N_Bolle_DaValorizzare - 1

                        'incremento il numero certificato
                        UltimoNumCertificato += 1

                        IdAgenda_to_Update = DT.Rows(i).Item("Id_Agenda")

                        NumeroBolla_to_Update = Ricava_NumeroDocumento_Con_Sequenza( _
                                                        DT.Rows(i).Item("Doc_Numero_Sin_Bolla"), _
                                                        DT.Rows(i).Item("Doc_Numero_Bolla"), _
                                                        DT.Rows(i).Item("Doc_Numero_Des_Bolla"), _
                                                        3, _
                                                        5, _
                                                        0, _
                                                        "0")

                        'update

                        Risp = obj_ADD.ModificaNumCertificato_ByIdAgenda(Qs_Piva, _
                                                                    0, _
                                                                    IdAgenda_to_Update, _
                                                                    "", _
                                                                    UltimoNumCertificato, _
                                                                    "", _
                                                                    "", _
                                                                    objParametri_Server)

                        If Risp = True Then
                            Log += "OK! La Bolla " + CStr(NumeroBolla_to_Update) + " ha assunto il Numero di Certificato = " + CStr(UltimoNumCertificato) + vbCrLf
                            N_Bolle_Valorizzate += 1
                        Else
                            'Log += "Errore! La Bolla " + CStr(NumeroBolla_to_Update) + " NON ha assunto il Numero di Certificato = " + CStr(UltimoNumCertificato) + vbCrLf
                            Flag_Errore = True
                            'QUESTO CASO NON DEVE VERIFICARSI, GENERO UN'ECCEZIONE
                            Throw New Exception("Errore! La Bolla " + CStr(NumeroBolla_to_Update) + " NON ha assunto il Numero di Certificato = " + CStr(UltimoNumCertificato) + "!")
                        End If

                    Next

                    'chiudo la transazione
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                    'chiudo la connessione
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

                    Log += "Sono state valorizzate " + CStr(N_Bolle_Valorizzate) + " bolle su " + CStr(N_Bolle_DaValorizzare) + " bolle da valorizzare." + vbCrLf


                Catch ex As Exception
                    '------------------------------------------------
                    'Si e' verificata una eccezione !!!!!!
                    '------------------------------------------------
                    Flag_Errore = True
                    'chiudo la transazione con il rollback
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    'chiudo la connessione
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                    'Messaggio di errore
                    Log += ex.Message.ToString() + vbCrLf
                    '------------------------------------------------
                End Try

            Else
                Log += "Il range di bolle selezionato per la stampa ha già il numero di certificato valorizzato." + vbCrLf
            End If



        Catch ex As Exception
            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            Flag_Errore = True
            'Messaggio di errore
            Log += "Si è verificato un errore durante la lettura della numerazione: " & vbCrLf & ex.Message.ToString()
            '------------------------------------------------
        End Try

        If Flag_Errore = False Then
            'se ok
            Me.Txt_DaInviare.Text = "1"
        Else
            Log += "Si è verificato un errore perciò la valorizzazione dei numeri di certificato è stata annullata." + vbCrLf
        End If

        Me.Txt_Messaggio.Text = Log

    End Sub


End Class
