Imports System.Net.Http
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class GestoreCache
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri

    Public objparametri_server_string, objparametri_utenti_string As String
    Public isSuperUser As Boolean = False

    Private Sub InizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Super_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        isSuperUser = (objParametri_Server.SuperUserUsername.ToLower() = objParametri_Server.UtenteUsername.ToLower())
    End Sub

    Private Sub InizializzoParametriPagina()

        Master.SetTitoloPaginaCustom("Gestione Cache (DataProvider)")

    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        InizializzoObjParametri()

        If Not IsPostBack Then
            InizializzoParametriPagina()
        End If

        'Controllo se l'utente ha i permessi per accedere
        'Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        'Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
        '                                    Session("ASG_Utente_Username"),
        '                                    Session("ASG_IdServizio"),
        '                                    enum_Security_Attivita.Contabilita_MVVElettronico,
        '                                    enum_Security_Operazione.Lettura,
        '                                    Date.Now,
        '                                    "",
        '                                    objParametri_Utenti)

        'Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
        '                                   Session("ASG_Utente_Username"),
        '                                   Session("ASG_IdServizio"),
        '                                   enum_Security_Attivita.Contabilita_MVVElettronico,
        '                                   enum_Security_Operazione.Modifica,
        '                                   Date.Now,
        '                                   "",
        '                                   objParametri_Utenti)

        If isSuperUser = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If Not Page.IsPostBack Then
            hf_cacheAbilitata.Value = MemoryCacheFactory.Instance.EnebaleCacheDataProvider
        End If

    End Sub

#Region "web services Cache"

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaElementiCache() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim oggetti = MemoryCacheFactory.Instance.Elenca_Tuttto()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(oggetti, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function DammiUrlPerScaching() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim urls = objConfSiti.getCaheServersUrls(objParametri_Server)
            r.RispostaStringa = JsonConvert.SerializeObject(urls.Where(Function(u) u <> String.Empty))
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function PulisciCache() As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = True
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim risultato = objCache.PulisciCache(Global_asax.HttpClient, objParametri_Server).ConfigureAwait(False).GetAwaiter.GetResult()
            r = risultato
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function PulisciElemento(ByVal chiave As String, ByVal tipoCache As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            MemoryCacheFactory.Instance.PulisciElemento(chiave, tipoCache)

            r.RispostaStringa = "Cache pulita correttamente"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AbilitaCache(ByVal abilita As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            MemoryCacheFactory.Instance.EnebaleCacheDataProvider = abilita

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function PulisciCachePermessi() As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = True
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim risultato = objCache.PulisciCachePermessi(Global_asax.HttpClient, objParametri_Server).ConfigureAwait(False).GetAwaiter.GetResult()

            r.Errore = risultato.Errore
            r.RispostaStringa = risultato.RispostaStringa
            r.RispostaStringaCustom = risultato.RispostaStringaCustom
            r.ErroriGias = risultato.ErroriGias

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function PulisciCacheImpostazioni() As RispostaStandard
        Dim r As New RispostaStandard
        r.RispostaOK = True
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim risultato = objCache.PulisciCacheImpostazioni(Global_asax.HttpClient, objParametri_Server).ConfigureAwait(False).GetAwaiter.GetResult()
            r = risultato
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function
#End Region


End Class