Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web.Services
Imports System.Web.Script.Serialization

Public Class PannelloDiControllo_CreaModificaScadenza
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim log As New DataProvider()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Expires = 0

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

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
            'Master.Lbl_Titolo.Text = "Pannello di Controllo"

            'carico in session tutte le categorie
            CaricaCategorie()
        End If

        CaricaValori()

    End Sub

    Private Sub CaricaCategorie()
        'LEGGO LE AREE E LE TIPOLOGIE
        'lo faccio qui così lo faccio una volta sola sia per gli allegati che per le scadenze
        Dim CatR As New Categorie_R
        Dim tabCat As List(Of PnlCtrl_Categorie) = CatR.leggi_PnlCtrl_Categorie(objParametri_Server)
        Session("PnlCtrl_ListaCategorie") = tabCat
    End Sub

    Private Sub CaricaValori()

        'imposto l'ID dell'elemento - se vale -1 vuol dire che la scadenza è nuova
        If IsNothing(Session("PnlCtrl_ID_Elem")) Then
            'errore()
            'Throw New Exception("Non è stato passato in session nessun ID_Elem")
        Else
            hfID_Elem.Value = Session("PnlCtrl_ID_Elem")
            'Session.Remove("PnlCtrl_ID_Elem")

        End If

    End Sub


End Class