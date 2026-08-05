Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Partial Class AgendaBootstrap_NC
    Inherits System.Web.UI.MasterPage

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Public Property UtenteAbilitato_NonConformita_Lettura As Boolean
    Public Property UtenteAbilitato_NonConformita_Lista_Lettura As Boolean
    Public Property UtenteAbilitato_NonConformita_FiltraEsporta_Lettura As Boolean
    Public Property UtenteAbilitato_NonConformita_Impostazioni_Lettura As Boolean
    Public Property UtenteAbilitato_NonConformita_Anagrafiche_Lettura As Boolean

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

        Master.bootstrapSelect_versione = "1.12.4"

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        _UtenteAbilitato_NonConformita_Lettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                            enum_Security_Attivita.NonConformita_Lista,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now, "", objParametri_Utenti)

        If _UtenteAbilitato_NonConformita_Lettura = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        'Verifico i permessi delle singole tab
        _UtenteAbilitato_NonConformita_Lista_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"), _
                                    enum_Security_Attivita.NonConformita_Lista, _
                                    enum_Security_Operazione.Lettura, _
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Lista_Lettura.Value = _UtenteAbilitato_NonConformita_Lista_Lettura

        _UtenteAbilitato_NonConformita_FiltraEsporta_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"), _
                                    enum_Security_Attivita.NonConformita_FiltraEsporta, _
                                    enum_Security_Operazione.Lettura, _
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_FiltraEsporta_Lettura.Value = _UtenteAbilitato_NonConformita_FiltraEsporta_Lettura

        _UtenteAbilitato_NonConformita_Impostazioni_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"), _
                                    enum_Security_Attivita.NonConformita_Impostazioni, _
                                    enum_Security_Operazione.Lettura, _
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Impostazioni_Lettura.Value = _UtenteAbilitato_NonConformita_Impostazioni_Lettura

        _UtenteAbilitato_NonConformita_Anagrafiche_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"), _
                                    enum_Security_Attivita.NonConformita_Anagrafiche, _
                                    enum_Security_Operazione.Lettura, _
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Anagrafiche_Lettura.Value = _UtenteAbilitato_NonConformita_Anagrafiche_Lettura

        If Not IsPostBack Then
            'imposto i dati nella barra superiore... -> Titolo
            Master.Lbl_Titolo.Text = "NON CONFORMITA'"
        End If
    End Sub

End Class