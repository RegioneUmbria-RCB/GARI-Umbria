Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class DSSWidget
    Inherits System.Web.UI.Page

    Public cIdPiva As String = ""
    Public cModelloPrevisionaleAutorizzato As String = ""
    Public cEnableRedirectToDSSDifesa As String = ""
    Public MessaggioAggiuntivo As String = ""
    Private _linkGiasBase As String = ""

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return _linkGiasBase
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim objParametriAgenda As New ParametriAgenda

        cIdPiva = objParametriAgenda.Piva

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If objParametri_Utenti Is Nothing Then
            cModelloPrevisionaleAutorizzato = "false"
            'i18n Variabile utilizzata?
            MessaggioAggiuntivo = "Sessione Scaduta (" & HttpContext.Current.Session.IsNewSession & "). Si prega di eseguire nuovamente il login"
        Else
            cModelloPrevisionaleAutorizzato = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Analisi_Modelli_Previsionali,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti
                ).ToString().ToLower()

            cEnableRedirectToDSSDifesa = "false"
            If Not IsNothing(Request.QueryString("enablerdir")) Then
                If Request.QueryString("enablerdir").ToString().ToLower().Equals("1") Then
                    cEnableRedirectToDSSDifesa = "true"
                End If
            End If
        End If

        'GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Utenti", _linkGiasBase)

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaParametriAndGetDSSDifesaUrl(params As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objParametriAgenda = New ParametriAgenda

            Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
            objAgenda.Piva = objParametriAgenda.Piva
            objAgenda.Veg_Cod = 0

            Dim parametriEncoded = Stringa_Codifica(params, AgroKey_EncoderDecoder, objParametri_Server)
            objAgenda.QueryStringFiltrino = "?params=" & parametriEncoded & "&ExtRdir=" & Stringa_Codifica(0, AgroKey_EncoderDecoder)

            objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_DSS_Difesa
            objAgenda.PaginaProvenienza = enum_PagineAgenda_2010.DSSWidget_ConRdir

            Dim TargetURL As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                     Enum_SiteRedirector.Sito_GiasOnline_2010, objAgenda)

            r.RispostaOK = True
            r.RispostaStringa = TargetURL

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

End Class