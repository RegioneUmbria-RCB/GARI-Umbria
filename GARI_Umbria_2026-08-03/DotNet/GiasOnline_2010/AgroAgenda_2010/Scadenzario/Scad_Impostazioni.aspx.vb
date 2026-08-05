Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class Scad_Impostazioni
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    'Private Sub Scad_Anagrafiche_Init(sender As Object, e As EventArgs) Handles Me.Init
    '    AddHandler CType(Me.Master.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    'End Sub
    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Rapporti_Contabili(ByVal piva As String) As RispostaStandard
        Dim DT As New DataTable
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            'Leggo i RapCon validi per il documentale
            Dim objRapCon As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            DT = objRapCon.RapportiContabili_Leggi("", "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("GestioneParametriIndici"), String)

        Response.Expires = 0

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'NonConformita/Lettura controllato nella master

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Scadenzario_Impostazioni,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now, "", objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

    End Sub

    'Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

    '    Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda()

    '    Dim link As String = ""
    '    Try
    '        Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
    '        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

    '        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

    '            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
    '                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                                   enum_PagineGiasOnline_2010.Menu,
    '                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

    '        ElseIf sitoorigine = Enum_SiteRedirector.Sito_GiasOnline And paginaOnLineRitorno = enum_PagineGiasOnline.MenuCartellaAziendale Then

    '            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
    '            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Scadenzario_Lista
    '            link = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.MenuCartellaAziendale, objParametriAgenda)

    '        Else
    '            link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
    '        End If

    '    Catch ex As Exception
    '        link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
    '    End Try

    '    Response.Redirect(link)
    'End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class