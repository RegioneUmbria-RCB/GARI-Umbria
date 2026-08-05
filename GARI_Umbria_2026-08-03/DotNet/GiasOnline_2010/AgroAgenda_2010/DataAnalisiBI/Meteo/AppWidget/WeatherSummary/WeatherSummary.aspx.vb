Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class WeatherSummary
    Inherits System.Web.UI.Page

    Public piva As String = ""
    Public weatherSummaryAllowed As String = ""
    Public MessaggioAggiuntivo As String = ""
    Private _linkGiasBase As String = ""

    Public Property Master_versione As String
        Get
            Return agroVersioneMaster.VersioneMaster
        End Get
        Set(value As String)
            agroVersioneMaster.VersioneMaster = value
        End Set
    End Property

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return _linkGiasBase
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim objParametriAgenda As New ParametriAgenda
        piva = objParametriAgenda.Piva

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If objParametri_Utenti Is Nothing Then
            weatherSummaryAllowed = "false"
            'i18n Variabile utilizzata?
            MessaggioAggiuntivo = "Sessione Scaduta (" & HttpContext.Current.Session.IsNewSession & "). Si prega di eseguire nuovamente il login"
        Else
            weatherSummaryAllowed = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.GiasAPP_Monitoraggio_Meteo,
                enum_Security_Operazione.Modifica,
                Date.Now,
                "",
                objParametri_Utenti
                ).ToString().ToLower()
        End If

        agroVersioneMaster.VersioneMasterPlaceHolder = versioneMasterPlaceHolder
        agroVersioneMaster.SitoOspite = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
    End Sub

End Class