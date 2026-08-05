Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp

Partial Class AgendaBootstrap_Visite
    Inherits System.Web.UI.MasterPage

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Public pivaAziendaSelezionataClientSide As String = ""

    Public Property UtenteAbilitato_Visite_Lettura As Boolean
    Public Property UtenteAbilitato_Visite_Lista_Lettura As Boolean
    Public Property UtenteAbilitato_Visite_Anagrafiche_Lettura As Boolean
    'Public pathAgronicaCoreWS As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Expires = 0

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = Utility.convertOBJparametritoString(objParametri_Utenti)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        _UtenteAbilitato_Visite_Lettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gest_CartellaAziendale_VisiteIspettive,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now, "", objParametri_Utenti)

        If Not _UtenteAbilitato_Visite_Lettura Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        'Verifico i permessi delle singole tab
        _UtenteAbilitato_Visite_Lista_Lettura = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Visite_Lista,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Lista_Lettura.Value = _UtenteAbilitato_Visite_Lista_Lettura

        _UtenteAbilitato_Visite_Anagrafiche_Lettura = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Visite_Anagrafiche,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Anagrafiche_Lettura.Value = _UtenteAbilitato_Visite_Anagrafiche_Lettura

        If Not IsPostBack Then
            'imposto i dati nella barra superiore... -> Titolo
            Master.Lbl_Titolo.Text = "VISITE"
        End If

        'Imposto il flag nella pagina per attivare il pulsante "indietro"
        Master.flag_pag_Visite = True

        Dim objParametriAgenda As New ParametriAgenda
        If objParametriAgenda.Piva <> "" Then
            pivaAziendaSelezionataClientSide = objParametriAgenda.Piva
        End If

    End Sub

End Class