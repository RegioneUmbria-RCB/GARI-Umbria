Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ

Imports System.Text.RegularExpressions

Public Class SmsSender
    Inherits System.Web.UI.Page


    Private objParametriAgenda As ParametriAgenda
    Private Master_Operazione As AgendaBootstrap

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Not Page.IsPostBack Then

            Dim permessi As AgronicaCoreUtentiDAL.TipoPermesso = verificaPermessiGiasSMS()

            If permessi.Scrittura Then
                panSMS.Visible = True
                panSMScredito.Visible = True
                panCredenziali.Visible = True
                panRisultato.Visible = True
            Else
                If permessi.Lettura Then
                    panCredenziali.Visible = True
                    panSMScredito.Visible = True
                    panRisultato.Visible = True
                Else
                    panRisultato.Visible = True
                    resultBox.Text = "Utente non autorizzato all'invio di sms o alla consultazione del credito."
                End If

            End If

        End If
    End Sub

    Private Function verificaPermessiGiasSMS() As AgronicaCoreUtentiDAL.TipoPermesso

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.PermessiUtente

        Dim permessi As AgronicaCoreUtentiDAL.TipoPermesso =
            ObjUtenti.getPermesso(enum_Security_Attivita.invioSMS)

        Return permessi
    End Function

    Protected Sub btnInviaSMS_Click(sender As Object, e As EventArgs) Handles btnInviaSMS.Click

        If verificaPermessiGiasSMS().Scrittura Then

            Dim smsSender As New AgronicaCoreMailBIZ.SMS_Programmazione_W

            'elimino gli eventuali spazi nei numeri di telefono, sia in lista che singoli'
            Dim numeriSenzaEventualiSpazi As String
            'elimino spazi prima e dopo la lista di numeri o del numero di telefono singolo'
            numeriSenzaEventualiSpazi = textPhoneNumber.Text.Trim
            'elimino gli spazi interni'
            numeriSenzaEventualiSpazi.Replace(" ", "")

            'pulisco l'area di testo degli SMS dai caratteri non consentiti'
            Dim areaDiTesto As String
            areaDiTesto = Regex.Replace(textSMS.Text, "[^\w\.@-_èéàìù§°çò]", "", RegexOptions.None, TimeSpan.FromSeconds(3))

            'Dim rval As RispostaStandard =
            '    smsSender.inviaSMS(numeriCell, textUserID.Text.Trim, textUserPassword.Text.Trim, textSenderNum.Text, textSMS.Text)

            Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

            Dim rval As RispostaStandard =
                smsSender.AccodaPerInvio(objParametri_Server, 0, "", textSenderNum.Text.Trim, numeriSenzaEventualiSpazi, areaDiTesto, Nothing)


            If rval.RispostaOK Then
                resultBox.Text = rval.RispostaStringa
            Else
                resultBox.Text = rval.Errore
            End If


        End If

    End Sub

    Protected Sub btnVerificaCreditoResiduo_Click(sender As Object, e As EventArgs) Handles btnVerificaCreditoResiduo.Click
        If verificaPermessiGiasSMS().Scrittura Then

            Dim smsSender As New AgronicaCoreMailBIZ.SMS_Programmazione

            Dim rval As RispostaStandard =
               smsSender.interrogaCredito(textUserID.Text.Trim, textUserPassword.Text)

            If rval.RispostaOK Then
                textRemCredit.Text = rval.RispostaStringa
            Else
                resultBox.Text = rval.Errore
            End If


        End If

    End Sub

    Private Sub SmsSender_Init(sender As Object, e As EventArgs) Handles Me.Init


        Me.Master.flag_pag_Operazione = True

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

    End Sub

    Private PaginaRitorno As String = "../Menu/Menu.aspx"

    Private Sub AnnullaTutto()

        Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)


        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            PaginaRitorno = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        Response.Redirect(PaginaRitorno)

    End Sub
End Class