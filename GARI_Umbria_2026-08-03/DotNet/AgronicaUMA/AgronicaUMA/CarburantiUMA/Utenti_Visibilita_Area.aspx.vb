Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json
Imports System.Web
Imports System.Web.UI.WebControls

Public Class Utenti_Visibilita_Area
    Inherits System.Web.UI.Page

    Public QS_Area As Integer = -1

    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametriProfilazione As AgronicaCoreGestioneRichieste.ParametriProfilazione_2010

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        objParametriProfilazione = New AgronicaCoreGestioneRichieste.ParametriProfilazione_2010
        objParametriProfilazione.Leggi()

        'CType(Page.Master.FindControl("LblTitolo"), Label).Text = "Gestione Visibilita per Area"
        'CType(Page.Master.FindControl("Lbl_Rag_Soc"), Label).Text = ""

        If Not IsNothing(Request.QueryString("area")) Then
            QS_Area = Request.QueryString("area")
        End If

        'If objParametriProfilazione.Piva <> "" Then
        '    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        '    Dim DTImpresa = objImprese.Leggi(objParametriProfilazione.Piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        '    If DTImpresa IsNot Nothing AndAlso DTImpresa.Rows.Count > 0 Then
        '        CType(Page.Master.FindControl("Lbl_Rag_Soc"), Label).Text = CStr(DTImpresa.Rows(0)("Rag_Soc"))
        '    End If
        'End If

        HD_Username.Value = objParametri_Server.UtenteUsername

        If (Not IsPostBack) Then

            Dim UtenteAbilitato_R As Boolean
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_R = objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Visibilita_Aziende_UMA,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)


            If UtenteAbilitato_R = False Then
                Response.Redirect("../Messaggi/AccessoNegato.htm")
            End If

        Else
            Exit Sub
        End If
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Visibilita_Area(ByVal area As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim areaCod As Integer

        Select Case area.ToLower
            Case "uma"
                areaCod = TipiEnumerativi.enum_Area_Visibilita.UMA
        End Select

        Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim userName = objParametri_Server.UtenteUsername

        Dim gruppo = 0

        Dim visibilitaCompleta = 0

        Dim checkDt As DataTable = objVisibilita.CheckVisibilitaUtenteGruppoArea(userName, 0, area, "", "", objParametri_Utenti)

        If checkDt.Rows.Count <= 0 Then

            Dim dtGruppi = objGruppi_Utente.Leggi_IdentificativoGruppoUtenti(userName, objParametri_Utenti)

            If dtGruppi.Rows.Count > 0 Then

                gruppo = dtGruppi.Rows.Item(0).Item("Gruppi_Utente_cod")
                userName = String.Empty

                checkDt = objVisibilita.CheckVisibilitaUtenteGruppoArea("", gruppo, area, "", "", objParametri_Utenti)

            End If

        End If

        If checkDt.Rows.Count > 0 Then

            If (checkDt.Rows.Item(0).Item("Visibilita_Completa")) Then
                visibilitaCompleta = 1
            End If

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato_VisibilitaCompleta = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                           enum_Id_Servizio.GiasOnline,
                                                                                           enum_Security_Attivita.Visibilita_Aziende_UMA_GestioneVisibilitaCompleta,
                                                                                           enum_Security_Operazione.Modifica,
                                                                                           Date.Now,
                                                                                           "",
                                                                                           objParametri_Utenti)

            Dt = objVisibilita.LeggiVisibilitaArea(userName,
                                                   gruppo,
                                                   areaCod,
                                                   visibilitaCompleta,
                                                   "",
                                                   "",
                                                   objParametri_Utenti,
                                                   objParametri_Server,
                                                   UtenteAbilitato_VisibilitaCompleta)

            'For Each row In Dt.Rows
            '    row.Item("CUAA") = leggiCUAA.Leggi_CUAA(row.Item("Piva_Azienda"), objParametri_Server)
            'Next

        End If

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiAree() As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

        Dt = objVisibilita.LeggiAree("", "", objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaVisibilita(ByVal righeInserite As String,
                                              ByVal righeModificate As String,
                                              ByVal righeCancellate As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim scrivi As New AgronicaCoreUtentiBIZ.Utenti
            r.RispostaStringa = scrivi.Aggiorna_Visibilita_Area(righeInserite, righeModificate, righeCancellate, objParametri_Utenti)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiGruppi() As RispostaStandard

        Dim r As New RispostaStandard
        Dim utentiGrp As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim dtGrp As DataTable

            dtGrp = utentiGrp.Leggi(0, "", "", objParametri_Utenti)

            r.RispostaStringa = JsonConvert.SerializeObject(dtGrp, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Azienda_Da_CUAA(ByVal CUAA As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim rag_soc As String = ""
        Dim piva As String = ""
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim leggiRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Try

            piva = leggiCUAA.Piva_from_CUAA(CUAA, objParametri_Server)

            If piva = "" Then
                r.RispostaOK = False
                r.Errore = "CUAA non trovato"
            Else
                rag_soc = leggiRagSoc.RagSoc_from_Piva(piva, objParametri_Server)
            End If

            r.RispostaStringa = JsonConvert.SerializeObject((rag_soc, piva), Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckVisibilita(ByVal Gruppo As Integer,
                                           ByVal Username As String,
                                           ByVal Area As Integer,
                                           ByVal Piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim check As Boolean = True

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

        Try

            'Dim dtPive = leggiVisibilita.LeggiVisibilitaArea(Username, Gruppo, Area, 0, "", "", objParametri_Utenti, objParametri_Server)
            Dim dtPive = leggiVisibilita.CheckPiveMultiple(Piva, 0, "", "", objParametri_Utenti, objParametri_Server)

            If Gruppo = 0 Then
                check = dtPive.Select("Gruppo_Cod > 0").Length > 0 'dtPive.Select("Piva_Azienda = '" + Piva + "' ").Length > 0
            Else
                check = dtPive.Rows.Count > 0
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(check, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckUtente(ByVal Username As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim check As Boolean = True

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

        Try

            Dim dtPive = leggiUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, "UserName like '" + Username + "' ", "", objParametri_Utenti)

            check = dtPive.Rows.Count > 0

            r.RispostaStringa = JsonConvert.SerializeObject(check, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

End Class