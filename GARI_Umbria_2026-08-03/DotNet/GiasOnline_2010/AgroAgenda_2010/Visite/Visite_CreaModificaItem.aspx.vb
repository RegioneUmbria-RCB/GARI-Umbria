Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

Public Class Visite_CreaModificaItem
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
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Scadenzario_Lista,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now, "", objParametri_Utenti)

        If Not UtenteAbilitatoLettura Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        Master.Master.Lbl_Titolo.Text = "Visita"

        If Not IsPostBack Then
            'Dim jsonParametri

            ''Controllo se i parametri mi sono stati passati in querystring
            'If Not IsNothing(Request.QueryString.Item("visitastr")) Then
            '    jsonParametri = Request.QueryString.Item("visitastr")
            'Else
            '    Throw New Exception("Non è stato passato in QS nessun parametro")
            '    Exit Sub
            'End If

            ''Deserializzo il JSON che mi è stato passato
            'Dim listaParametri = JsonConvert.DeserializeObject(jsonParametri)

            ''Controllo che mi sia stato passato l'ID_Agenda
            'If Not IsNumeric(listaParametri("ID_Agenda")) Then
            '    Throw New Exception("Non è stato passato nessun ID_Agenda o questo non è un numero")
            '    Exit Sub
            'End If

            ''If (CInt(listaParametri("ID_Alert_Entita")) > 0) Then
            ''ho aperto una scadenza, imposto l'hidden con il json
            'hfId_Agenda.Value = listaParametri("ID_Agenda")
            hfId_Agenda.Value = -1

            ''Else
            ''    'è una scadenza nuova che si vuole creare
            ''    hfId_Alert_Entita.Value = listaParametri("ID_Alert_Entita")

            ''End If



        Else

            '    'Popolo pannelli in base alla NC(passando l'oggetto, disegna quello e non legge da DB)
            '    nc = CType(Session("NC_CreaModificaItem_ObjNc"), NC_Testata)
            '    disegnaSchemaNC(nc)

        End If

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Scadenzario_Lista,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now, "", objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura


        'If UtenteAbilitatoFaseCrea Then
        '    btnAggiungi.Visible = True

        '    'Da Sistemare... è oRendo...
        '    If objParametri_Server.PivaSuperUser = "01271980391" Then ' Fruttagel
        '        btnAggiungi.InnerHtml = "<span class='fa fa-plus-circle'></span> Aggiungi Trattamento"
        '    End If

        '    'Se c'è un dettaglio non ancora salvato (e quindi senza ID) allora nascondo il pulsante "aggiungi"
        '    If Not IsNothing(nc) AndAlso Not IsNothing(nc.Dettagli) AndAlso nc.Dettagli.Count > 0 AndAlso
        '        nc.Dettagli.Where(Function(x) IsNothing(x.ID_Dettaglio)).ToArray().Length > 0 Then
        '        btnAggiungi.Visible = False
        '    End If
        'End If

    End Sub

End Class