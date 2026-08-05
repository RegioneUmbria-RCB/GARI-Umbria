Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

Public Class PannelloDiControllo_CreaModificaItem
    Inherits System.Web.UI.Page

    Const percorso_NC_OPTA_Dettaglio_ASCX As String = "~\PannelloDiControllo\NonConformita_OPTA_Dettaglio.ascx"

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
            Master.Master.Lbl_Titolo.Text = "Pannello di Controllo"

            'carico tutti i controlli della pagina
            PopolaCombo()
        End If

        'leggo i parametri passati in POST ed estraggo l'ID_NC
        Dim jsonParametri = Request.Form("parametri")
        Dim listaParametri = JsonConvert.DeserializeObject(jsonParametri)

        If IsNothing(listaParametri("PnlCtrl_ID_NC")) Then
            Throw New Exception("Non è stato passato in POST nessun ID_NC")
            Exit Sub
        End If

        Dim id_NC As Integer = CInt(listaParametri("PnlCtrl_ID_NC"))

        Dim lista As New List(Of PnlCtrl_NonConformita)
        Dim nonConformita As PnlCtrl_NonConformita = Nothing
        If Not IsNothing(listaParametri("NC")) Then
            nonConformita = New PnlCtrl_NonConformita(listaParametri("NC")(0))
            lista.Add(nonConformita)
        End If

        hfObjJsonNonConformita.Value = JsonConvert.SerializeObject(lista)

        'Salvo l'ID_NC nell'hiddenField
        hfID_NC.Value = id_NC

        CaricaValori(id_NC, nonConformita)

    End Sub

    Private Sub PopolaCombo()
        'LEGGO LE AREE E LE TIPOLOGIE
        Dim CatR As New Categorie_R
        Dim tabCat As List(Of PnlCtrl_Categorie) = CatR.leggi_PnlCtrl_Categorie(objParametri_Server)
        Session("PnlCtrl_ListaAree") = tabCat
    End Sub

    Private Sub CaricaValori(id_NC As Integer?, nonConformita As PnlCtrl_NonConformita)

        If id_NC <> -1 Then
            Dim elemR As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_R()
            Dim listaDetNC As List(Of PnlCtrl_NonConformita_Dettagli) = elemR.leggi_NonConformita_Dettagli( _
                objParametri_Server, hfID_NC.Value)

            For Each x As PnlCtrl_NonConformita_Dettagli In listaDetNC
                AggiungiDettaglioAlPannello(PnlDettagli, x.ID_Dettaglio) ' indico l'id del singolo dettaglio
            Next

            'popolo la non conformità con i dati che mi hanno passato via POST
            If Not IsNothing(nonConformita) Then

            End If
        End If

    End Sub

    Private Sub btnAggiungi_ServerClick(sender As Object, e As System.EventArgs) Handles btnAggiungi.ServerClick
        AggiungiDettaglioAlPannello(PnlDettagli, -1) ' indico che è un nuovo dettaglio
    End Sub

    Private Sub AggiungiDettaglioAlPannello(pnl As Panel, ID_Dettaglio As Integer)
        Dim det As NonConformita_OPTA_Dettaglio = CType(LoadControl(percorso_NC_OPTA_Dettaglio_ASCX), NonConformita_OPTA_Dettaglio)
        det.ID = "PnlDet" & (pnl.Controls.Count + 1)
        det.ID_Pannello = det.ID & "_"
        det.TitoloPannello = "Dettaglio " & (pnl.Controls.Count + 1)
        det.ID_Dettaglio = ID_Dettaglio
        pnl.Controls.Add(det)
    End Sub

End Class