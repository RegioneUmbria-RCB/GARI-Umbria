

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json

Public Class RichiestaAssistenzaGias
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)
        '---
    End Sub

#Region "script services Carica Griglia Ricerca"


#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Master().Lbl_Titolo.Text = "Richiesta Assistenza e Banche Dati"

        inizializzoObjParametri()
        inizializzoParametriPagina()

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)



        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)




        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                          AgroKey_EncoderDecoder,
                                          Server)


        hdPiva_Codificata.Value = Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder, HttpContext.Current.Session)

        Dim PaginaRedirect As String = ""

        hdPaginaRedirect.Value = ""
        If Not Request.QueryString("origine") Is Nothing Then
            hdPaginaRedirect.Value = CStr(Request.QueryString("origine"))
        Else
            hdPaginaRedirect.Value = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        If Not Page.IsPostBack Then
            'caricaControlli()
        End If

        If Not IsNothing(Session("ParametriAgenda_2010")) Then

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.Leggi()

        End If



    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub

    Private Sub RichiestaAssistenzaGias_PreLoad(sender As Object, e As EventArgs) Handles Me.PreLoad

    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class