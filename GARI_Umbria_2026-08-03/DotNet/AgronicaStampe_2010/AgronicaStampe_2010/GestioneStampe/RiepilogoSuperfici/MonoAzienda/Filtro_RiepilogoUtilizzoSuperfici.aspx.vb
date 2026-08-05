Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.My.Resources
Imports Newtonsoft.Json.Linq

Public Class Filtro_RiepilogoUtilizzoSuperfici
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Dim objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        ' Permesso di Lettura -> lo ignoro per chiamata dal GiasLAN
        'Dim UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
        '                                                               Session("ASG_IdServizio"),
        '                                                               enum_Security_Attivita.Gest_Stampe,
        '                                                               enum_Security_Operazione.Lettura,
        '                                                               Date.Now,
        '                                                               "",
        '                                                               objParametri_Utenti)

        'If Not UtenteAbilitatoLettura Then
        '    'TODO Verificare il redirect per le stampe
        '    Response.Redirect("~/Custom500.aspx")
        '    Exit Sub
        'End If


        hfPiva.Value = Stringa_Decodifica(Request.QueryString("p"), AgroKey_EncoderDecoder)


        'TODO Individuare l'id corretto
        CType(MyBase.Master, StampeBootstrap).SetTitoloPagina(42)


    End Sub


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function Stampa(ByVal piva As String, ByVal saCod As Integer, ByVal dataRif As Date) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Dim userName As String = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            'Dim prgGias As String = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            Dim queryString As String = "p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) &
                "&s_c=" & Stringa_Codifica(saCod, AgroKey_EncoderDecoder) &
                "&d_r=" & Stringa_Codifica(dataRif.ToString("u"), AgroKey_EncoderDecoder)
            Dim obj As New JObject

            obj("queryString") = queryString
            obj("paginaDaRichiamare") = "RiepilogoUtilizzoSuperfici.aspx"
            r.RispostaStringa = obj.ToString()
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)

        End Try

        Return r
    End Function

End Class