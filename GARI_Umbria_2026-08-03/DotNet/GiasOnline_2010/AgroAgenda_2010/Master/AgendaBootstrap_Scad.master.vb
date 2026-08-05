Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Partial Class AgendaBootstrap_Scad
    Inherits System.Web.UI.MasterPage

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Public Property UtenteAbilitato_Scadenzario_Lettura As Boolean
    Public Property UtenteAbilitato_Scadenzario_Lista_Lettura As Boolean
    Public Property UtenteAbilitato_Scadenzario_FiltraEsporta_Lettura As Boolean
    Public Property UtenteAbilitato_Scadenzario_Impostazioni_Lettura As Boolean
    Public Property UtenteAbilitato_Scadenzario_IndiciRicerca_Lettura As Boolean

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
        _UtenteAbilitato_Scadenzario_Lettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Scadenziario_Menu,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now, "", objParametri_Utenti)

        If Not _UtenteAbilitato_Scadenzario_Lettura Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        'Verifico i permessi delle singole tab
        _UtenteAbilitato_Scadenzario_Lista_Lettura = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Scadenzario_Lista,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Lista_Lettura.Value = _UtenteAbilitato_Scadenzario_Lista_Lettura

        '_UtenteAbilitato_Scadenzario_FiltraEsporta_Lettura = objPermessi.Controlla_Permessi_Utente(
        '                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
        '                            enum_Security_Attivita.Scadenzario_FiltraEsporta,
        '                            enum_Security_Operazione.Lettura,
        '                            Date.Now, "", objParametri_Utenti)
        'hf_utenteAbilitato_FiltraEsporta_Lettura.Value = _UtenteAbilitato_Scadenzario_FiltraEsporta_Lettura
        hf_utenteAbilitato_FiltraEsporta_Lettura.Value = False

        _UtenteAbilitato_Scadenzario_Impostazioni_Lettura = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Scadenzario_Impostazioni,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Impostazioni_Lettura.Value = _UtenteAbilitato_Scadenzario_Impostazioni_Lettura

        _UtenteAbilitato_Scadenzario_IndiciRicerca_Lettura = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now, "", objParametri_Utenti)
        hf_utenteAbilitato_Anagrafiche_Lettura.Value = _UtenteAbilitato_Scadenzario_IndiciRicerca_Lettura

        If Not IsPostBack Then
            'imposto i dati nella barra superiore... -> Titolo
            Master.Lbl_Titolo.Text = "SCADENZARIO"
        End If

        'Imposto il flag nella pagina per attivare il pulsante "indietro"
        Master.flag_pag_Scadenzario = True

    End Sub

End Class