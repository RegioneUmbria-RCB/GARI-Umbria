
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ConfigurazioneMeteo
    Inherits System.Web.UI.Page

    Public srv_gm As String
    Private objParametriAgenda As ParametriAgenda
    Public Anagrafiche_Stazioni_Autorizzato As Boolean
    Public Alias_Pubbliche_Autorizzato As Boolean

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Master.flag_pag_Operazione = True

        Anagrafiche_Stazioni_Autorizzato = False
        Alias_Pubbliche_Autorizzato = False

        objParametriAgenda = New ParametriAgenda
        Session("objParametriAgenda") = objParametriAgenda

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If Not (HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server)) Then

            'Recupero da webconfig la connessione alternativa da usare
            Dim AgroWebC As New AgroWebConfig()

            srv_gm = "https://maps.googleapis.com/maps/api/js?key="

            If AgroWebC.GoogleMaps <> "" Then
                srv_gm = AgroWebC.GoogleMaps
            End If

            If Debugger.IsAttached Then
                srv_gm = "https://maps.googleapis.com/maps/api/js?v=3.exp&client=gme-addictive&sensor=false&libraries=drawing,geometry&callback=Function.prototype"
            End If


            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Anagrafiche_Stazioni_Autorizzato = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Meteo_Modifica_creazioneStazioniProprietaVirtuali,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti)

        End If


        '*** DEBUG ****************************************************
        Alias_Pubbliche_Autorizzato = Anagrafiche_Stazioni_Autorizzato
        'Anagrafiche_Stazioni_Autorizzato = False
        '***************************************************************

    End Sub

    Private Sub ConfigurazioneMeteo_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Private Sub AnnullaTutto()

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim configurazioneSitiLettura As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim flagMenu = configurazioneSitiLettura.Leggi_Valore(0, "MenuBS_2017", "", "", objParametri_Server)

        If flagMenu.ToLower = "true" Then

            Response.Redirect("../../../menu/menubs_2017.aspx")
        Else

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))
        End If
    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiCentri() As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim objParametriAgenda = New ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim centriR As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim XmlCentri = centriR.CentroAziendale_Leggi(piva, 0, False, False, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

            Dim xmldoc = XDocument.Parse(XmlCentri)

            Dim respArr As New JArray

            For Each c In xmldoc.<DatiCentriAziendali>.<CentroAziendale>

                respArr.Add(New JObject(
                            New JProperty("sa_cod", CInt(c.Attribute("sa_cod").Value)),
                            New JProperty("sa_nome", c.Attribute("sa_nome").Value.ToString)
                            ))
            Next

            risp.RispostaStringa = respArr.ToString
            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

End Class