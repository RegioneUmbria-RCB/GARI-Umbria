Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class Filtro_SchedaTracciabilita
    Inherits System.Web.UI.Page

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents ImgBtnAnnulla As System.Web.UI.WebControls.ImageButton
    Protected WithEvents LABEL1 As System.Web.UI.WebControls.Label
    Protected WithEvents IMAGE1 As System.Web.UI.WebControls.Image
    Protected WithEvents ImageLogo As System.Web.UI.WebControls.Image
    Protected WithEvents LABEL2 As System.Web.UI.WebControls.Label
    Protected WithEvents TxtValiditaFine As System.Web.UI.WebControls.TextBox
    Protected WithEvents LblA As System.Web.UI.WebControls.Label
    Protected WithEvents TxtValiditaInizio As System.Web.UI.WebControls.TextBox
    Protected WithEvents LblDA As System.Web.UI.WebControls.Label
    Protected WithEvents Pannello_Intervallo As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Data As System.Web.UI.WebControls.Panel
    Protected WithEvents LABEL12 As System.Web.UI.WebControls.Label
    Protected WithEvents ImgBtn_Stampa As System.Web.UI.WebControls.ImageButton
    Protected WithEvents Pannello_Stampa As System.Web.UI.WebControls.Panel
    Protected WithEvents Pannello_Filtri As System.Web.UI.WebControls.Panel
    Protected WithEvents Chk_VisualizzaTipologieVarietali As System.Web.UI.WebControls.CheckBox
    Protected WithEvents LABEL3 As System.Web.UI.WebControls.Label
    Protected WithEvents LABEL16 As System.Web.UI.WebControls.Label
    Protected WithEvents Rbl_Arrotondamento As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents LABEL4 As System.Web.UI.WebControls.Label

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            TipiEnumerativi.enum_Security_Attivita.Gest_Stampe, _
        '                            TipiEnumerativi.enum_Security_Operazione.Lettura, _
        '                            strDummy)

        '----- !!!!!!!!!!! -------------

        'Attivazione forzata provvisoria

        UtenteAbilitato = True

        '----- !!!!!!!!!!! -------------

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If


        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then
            'output.Write("Page has just been loaded")

            'la prima volta che carico la pagina la metto in primo piano
            '(in caso contrario rimane in primo piano la pagina del GiasOnline)
            Dim strFocus As String = "<script language='javascript'> window.focus() </script>"
            Me.FindControl("Form1").Controls.Add(New LiteralControl(strFocus))

        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If

        '##############################################################
        '#####  CARICO I DATI  ########################################
        '##############################################################

        'di default imposto le date dell'intervallo coincidenti con l'annata agraria 
        'dal 1/11 al 31/10

        Dim Mese As Integer

        Mese = Now.Month

        Select Case Mese

            Case 11, 12
                'se sono nei mesi di novembre o dicembre..........
                'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
                Me.TxtValiditaInizio.Text = "01/11/" & Now.Year
                Me.TxtValiditaFine.Text = "31/10/" & Now.Year + 1

            Case Else
                'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
                Me.TxtValiditaInizio.Text = "01/11/" & Now.Year - 1
                Me.TxtValiditaFine.Text = "31/10/" & Now.Year

        End Select


    End Sub

    '###########################################################################
    Private Sub ImgBtnAnnulla_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnulla.Click

        Dim strClose As String = "<script language='javascript'> window.close() </script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub


    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Stampa()

    End Sub


    '###########################################################################
    Private Sub Stampa()

        Dim TargetURL As String
        Dim DataInizio As Date
        Dim DataFine As Date

        '----- Verifico i dati

        If Me.TxtValiditaInizio.Text = "" Then
            'AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Page)
            Exit Sub
        Else
            DataInizio = CDate(Me.TxtValiditaInizio.Text)
        End If

        If Me.TxtValiditaFine.Text = "" Then
            'AgroMsgBox("Impostare una data finale di riferimento per la stampa ...", Page)
            Exit Sub
        Else
            DataFine = CDate(Me.TxtValiditaFine.Text)
        End If

        If DataFine < DataInizio Then
            'AgroMsgBox("La data finale di riferimento per la stampa non può precedere quella di inizio...", Page)
            Exit Sub
        End If

        If Me.Chk_VisualizzaTipologieVarietali.Checked = True Then
            Session("VisualizzaTipologieVarietali") = True
        Else
            Session("VisualizzaTipologieVarietali") = False
        End If

        TargetURL = "SchedaTracciabilita.aspx" & _
                    "?dI=" & _
                    Stringa_Codifica(DataInizio.ToShortDateString, AgroKey_EncoderDecoder, Server) & _
                    "&dF=" & _
                    Stringa_Codifica(DataFine.ToShortDateString, AgroKey_EncoderDecoder, Server) & _
                    "&arr=" & _
                    Stringa_Codifica(Me.Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server)


        Response.Redirect(TargetURL)



    End Sub


End Class
