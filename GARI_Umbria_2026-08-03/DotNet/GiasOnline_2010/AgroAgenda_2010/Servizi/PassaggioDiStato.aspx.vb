Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PassaggioDiStato
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri

    Public QS_Pratica_Cod As String = ""
    Public QS_PassaggioDiStato_Cod As Integer = 0
    Public HD_Username As String = ""

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Super_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False

        If Not IsNothing(Request.QueryString("pratica")) Then
            QS_Pratica_Cod = Request.QueryString("pratica")
        End If
        hdPraticaCod.Value = QS_Pratica_Cod

        QS_PassaggioDiStato_Cod = 0
        If Not IsNothing(Request.QueryString("passaggiodistato")) Then
            QS_PassaggioDiStato_Cod = CInt(Request.QueryString("passaggiodistato"))
        End If
        hdPassaggioDiStatoCod.Value = QS_PassaggioDiStato_Cod

        HD_Username = objParametri_Server.UtenteUsername
        hdUsername.Value = HD_Username

        If (Not IsPostBack) Then

            Dim UtenteAbilitato_R As Boolean
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_R = objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Gestione_Servizi,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti) OrElse objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Gestione_Servizi_NEW,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)


            If Not UtenteAbilitato_R Then
                Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
            End If

        Else
            Exit Sub
        End If

    End Sub

End Class