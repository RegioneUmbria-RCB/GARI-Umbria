Imports AgronicaCoreContabDAL

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreEntityFramework_POCO

Imports AgronicaControlli_2010

Public Class Giacenze_Magazzino
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

#Region "script services Carica Griglia Movimenti di conferimento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaGiacenze(ByVal piva As String, ByVal _prodotto As Integer,
            ByVal _specie As Integer, ByVal _varieta As String, ByVal _sa_cod As Integer,
            ByVal _dataRif As String, ByVal _lotto As String,
            ByVal _calibro As Integer, ByVal _qualita As Integer, ByVal _certificazione As Integer, ByVal _rugginosita As Integer,
            ByVal _imballaggio As Integer, ByVal _contenitore As Integer, ByVal _confezione As Integer, ByVal _chkGiacenzePositive As Boolean, _mostraCampiInput As Boolean) As RispostaStandard

        Return Giacenze_MagazzinoUC.CaricaGrigliaGiacenze(piva, _prodotto,
             _specie, _varieta, _sa_cod, _dataRif, _lotto, _calibro, _qualita, _certificazione, _rugginosita,
               _imballaggio, _contenitore, _confezione, _chkGiacenzePositive, _mostraCampiInput)
    End Function
    Public Shared Function StringToInteger(st As String) As Integer
        Return CInt(st)
    End Function

#End Region

#Region "Caricamento"

    Private Sub caricaControlli()
    End Sub

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim r As New RispostaStandard

        Master().Lbl_Titolo.Text = "Giacenze magazzino"

        Master.flag_pag_LavorazioniFF = False

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        hdId_Agenda.Value = Request.QueryString("Id_Agenda")

        inizializzoObjParametri()

        If Not IsPostBack Then
            inizializzoParametriPagina()
        End If

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.FF_Magazzino,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        'Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
        '                                   Session("ASG_Utente_Username"),
        '                                   Session("ASG_IdServizio"),
        '                                   enum_Security_Attivita.FF_Magazzino,
        '                                   enum_Security_Operazione.Modifica,
        '                                   Date.Now,
        '                                   "",
        '                                   objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = False

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If UtenteAbilitatoScrittura = False Then

        End If

        If Not Page.IsPostBack Then
            caricaControlli()
        End If

    End Sub

    Private Sub inizializzoParametriPagina()
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property
End Class