Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Public Class PannelloDiControllo_NC_OPTA
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

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

        CaricaDDL()
    End Sub

    Private Sub CaricaDDL()
        CaricaListControl.Imprese(ddlListaOP, True, "", "", "", " TipoImpresaGerarchia <> " & enum_TipoImpresaGerarchia.Impresa, "", objParametri_Server)
        CaricaListControl.ImpreseConFiltroUtente(ddlListaAziende, True, "", "", "", "", objParametri_Server, objParametri_Utenti)

        CaricaListControl.Utenti(ddlListaTec, True, "", "", "", "", objParametri_Utenti)
        CaricaCombo_AuditStati(ddlListaStato)

        ddlListaOggRil.Items.Clear()
        ddlListaOggRil.Items.Add(New ListItem("", "0"))
        ddlListaOggRil.Items.Add(New ListItem("Forni", "1"))
        ddlListaOggRil.Items.Add(New ListItem("Macchine", "2"))
        ddlListaOggRil.Items.Add(New ListItem("Zone", "3"))

    End Sub


    '##############################################################################################
    Public Sub CaricaCombo_AuditStati(ByRef Cmb As DropDownList)

        'Dim DLL_AD As New AccessoDB_Condizionalita.AccessoDati(objParametri_Server)

        'Dim strErr As String = String.Empty
        'Dim DT As DataTable = DLL_AD.AuditStati_Leggi(CInt(TipoAudit), 0, strErr)
        'Cmb.Items.Clear()
        'Cmb.Items.Add(New ListItem("", "-1"))
        'For Each dr As DataRow In DT.Rows
        '    Cmb.Items.Add(New ListItem(dr.Item("Stato_Des").ToString, dr.Item("Stato_Cod").ToString))
        'Next

    End Sub
End Class