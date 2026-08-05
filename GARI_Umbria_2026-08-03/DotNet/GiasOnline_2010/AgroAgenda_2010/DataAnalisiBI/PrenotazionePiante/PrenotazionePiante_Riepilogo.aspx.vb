Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ

Public Class PrenotazionePiante_Riepilogo
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function PrenotazionePiante_Riepilogo(ByVal Data As Date?) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim objProgrammazione As New AgronicaCoreAnagrafeBIZ.Programmazione_R

            If Data Is Nothing Then
                Data = AGRODATAINIZIO
            End If

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(
                HttpContext.Current.Session("ASG_Utente_Username"), objParametri_Utenti)

            dt = objProgrammazione.Leggi_PrenotazionePiante_Riepilogo("", 0, Data, Filtro_Visibilita_Utente, False, objParametri_Server)

            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(dt)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function PrenotazionePiante_RiepilogoSintetico(ByVal Data As Date?) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable
            Dim objProgrammazione As New AgronicaCoreAnagrafeBIZ.Programmazione_R

            If Data Is Nothing Then
                Data = AGRODATAINIZIO
            End If

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(
                HttpContext.Current.Session("ASG_Utente_Username"), objParametri_Utenti)

            dt = objProgrammazione.Leggi_PrenotazionePiante_Riepilogo("", 0, Data, Filtro_Visibilita_Utente, True, objParametri_Server)

            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(dt)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Colori_Stati() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim dt As New DataTable

            Dim objStati As New AgronicaCoreMetaSchemaDAL.WAnagrafica_Stati_R

            dt = objStati.Leggi(0, 0, "", "", objParametri_Server)

            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(dt)
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function

End Class