Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Partial Class AgendaBootstrap_CtrlPnl
    Inherits System.Web.UI.MasterPage

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Expires = 0

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente( _
                                            Session("ASG_Utente_Username"), _
                                            Session("ASG_IdServizio"), _
                                            enum_Security_Attivita.NonConformita, _
                                            enum_Security_Operazione.Lettura, _
                                            Date.Now, _
                                            "", _
                                            objParametri_Utenti)

        If Not UtenteAbilitato Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If Not IsPostBack Then
            'imposto i dati nella barra superiore... -> Titolo
            Master.Lbl_Titolo.Text = "Pannello di Controllo"
        End If
    End Sub


End Class