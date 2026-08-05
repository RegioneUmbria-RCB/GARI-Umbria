
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services

Public Class DSSDifesa
    Inherits System.Web.UI.Page

    Private objParametriAgenda As ParametriAgenda
    Public DSSDifesa_Autorizzato As Boolean
    Public Autorizzato_Impostazione_Modelli As Boolean
    Public srv_gm As String
    Public vegCod As Int32
    Public NascondiFiltri As String = ""

    Private Sub DSSDifesa_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametriAgenda = New ParametriAgenda
        Session("objParametriAgenda") = objParametriAgenda

        DSSDifesa_Autorizzato = False
        Autorizzato_Impostazione_Modelli = False

        Master.SetTitoloPagina(43)

        'If Request.QueryString("gis") <> "" Then
        '    Me.Master.flag_MostraHeader = False
        '    Me.Master.flag_MostraFooter = False
        'Else
        '    Me.Master.flag_pag_Operazione = True
        'End If

        Master.flag_pag_Operazione = True

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim AperturaDaPopupOld As Boolean = False
        If Request.QueryString.Get("ViewModal") IsNot Nothing Then
            AperturaDaPopupOld = CType(Request.QueryString.Get("ViewModal"), Boolean)
        End If

        If AperturaDaPopupOld Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            Master.flag_MostraHeader = True
            Master.flag_MostraFooter = True
        End If

        If Not (HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server)) Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            DSSDifesa_Autorizzato = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Analisi_Modelli_Previsionali,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti)

            Autorizzato_Impostazione_Modelli = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Configurazione_Modelli_Previsionali,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti)

            'Recupero da webconfig la connessione alternativa da usare
            Dim AgroWebC As New AgronicaCoreGestioneRichieste.AgroWebConfig()

            srv_gm = "https://maps.googleapis.com/maps/api/js?key="

            If AgroWebC.GoogleMaps <> "" Then
                srv_gm = AgroWebC.GoogleMaps
            End If

            If Debugger.IsAttached Then
                srv_gm = "https://maps.googleapis.com/maps/api/js?v=3.exp&client=gme-addictive&sensor=false&libraries=drawing,geometry&callback=Function.prototype"
            End If

        End If

        vegCod = objParametriAgenda.Veg_Cod
        hdPiva.Value = objParametriAgenda.Piva
        hdRagSoc.Value = objParametriAgenda.RagSoc
        hdExternalLoad.Value = False.ToString
        hdParametri.Value = String.Empty
        If Not IsNothing(Request.QueryString("externalLoad")) Then
            hdExternalLoad.Value = Request.QueryString("externalLoad").ToString

            If Not IsNothing(Request.QueryString("parametri")) Then
                hdParametri.Value = Request.QueryString("parametri").ToString
            End If

        End If

        If Not IsNothing(Request.QueryString("params")) Then
            'Carico i dati, in modo che vada in sovraimpressione la vista dettagliata
            hdExternalLoad.Value = "true"
            hdParametri.Value = Stringa_Decodifica(Request.QueryString("params").ToString, AgroKey_EncoderDecoder, objParametri_Server)
        End If

        Dim AperturaDaPopup = Request.QueryString.AllKeys.Contains("Ifr")

        If AperturaDaPopup Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

        Dim AperturaDaRedirectEsterno = Request.QueryString.AllKeys.Contains("ExtRdir")

        If AperturaDaRedirectEsterno Then
            NascondiFiltri = "1"
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

        If Not DSSDifesa_Autorizzato Then

            AnnullaTutto()
        End If

    End Sub

    Private Sub AnnullaTutto()

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)


        Dim TargetUrl = "~/menu/menubs_2017.aspx"

        If DTConfigSiti Is Nothing OrElse DTConfigSiti.Rows.Count = 0 OrElse DTConfigSiti.Rows(0).Item("Valore").ToString <> "true" Then

            TargetUrl = CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda)
        End If

        Response.Redirect(TargetUrl)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDSSForecast(piva As String) As RispostaStandard
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
            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim ImpostazioneValore = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
                    piva, New List(Of Integer)({0}),
                    enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_FORECAST, "0",
                    objParametri_Utenti, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = ImpostazioneValore

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

End Class

