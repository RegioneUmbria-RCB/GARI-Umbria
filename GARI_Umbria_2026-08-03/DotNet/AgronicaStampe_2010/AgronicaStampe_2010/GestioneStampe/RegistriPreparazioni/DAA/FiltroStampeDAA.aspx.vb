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
Imports AgronicaControlli_2010
Imports Newtonsoft.Json
Imports AgronicaCoreUtentiDAL
Imports CrystalDecisions.CrystalReports
Imports AgronicaCoreAcciseCommon
Imports AgronicaCoreAcciseDAL

Public Class FiltroStampeDAA
    Inherits System.Web.UI.Page


#Region "Proprietà"



#End Region

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Public permessi As PermessiUtente
    Public objparametri_server_string, objparametri_utenti_string As String


    Private _logErrori As String = ""
    Private _catCod As Integer
    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub inizializzoObjParametri()

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
    End Sub

#Region "script services Report DAA"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Report_DAA(ByVal piva As String,
                                                ByVal dataDal As String,
                                                ByVal dataAl As String,
                                                ByVal dataStampa As String,
                                                ByVal protocollo As String,
                                                ByVal ufficioDogane As String,
                                                ByVal tipoReport As String) As RispostaStandard

        Dim QueryString As String
        Dim redirect As String = ""

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Select Case tipoReport
            Case "0"
                redirect = "DAA_GaranzieCircolazione.aspx"
            Case "1"
                redirect = "DAA_PartiteSospensioneAccisa.aspx"
            Case Else
                r.Sessione = False
                Return r
        End Select

        QueryString = redirect + _
                          "?p=" + Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing) + _
                          "&di=" + Stringa_Codifica(dataDal, AgroKey_EncoderDecoder, Nothing) + _
                          "&df=" + Stringa_Codifica(dataAl, AgroKey_EncoderDecoder, Nothing) + _
                          "&ds=" + Stringa_Codifica(dataStampa, AgroKey_EncoderDecoder, Nothing) + _
                          "&np=" + Stringa_Codifica(protocollo, AgroKey_EncoderDecoder, Nothing) + _
                          "&ud=" + Stringa_Codifica(ufficioDogane, AgroKey_EncoderDecoder, Nothing)


        r.RispostaOK = True
        r.RispostaStringa = QueryString

        Return r

    End Function
#End Region

#Region "Script services carica griglia garanzie di circolazione"
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaLiquidazioneSocio(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim dataManager = New DataManager(objParametriServer, Nothing, Nothing, False)
            Dim dataDa As DateTime = If(String.IsNullOrEmpty(dataDal), AGRODATAINIZIO, Convert.ToDateTime(dataDal))
            Dim dataA As DateTime = If(String.IsNullOrEmpty(dataAl), AGRODATAFINE, Convert.ToDateTime(dataAl))

            Dim garanzie = dataManager.Stampe_LeggiGaranzieCircolazione(piva, dataDa, dataA)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            Dim jsonData = JsonConvert.SerializeObject(garanzie, Formatting.None, serializerSettings)


            r.RispostaStringa = jsonData
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Caricamento"

    Private Sub caricaControlli()
    End Sub

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()
        Dim data_Da As String
        Dim data_A As String
        Dim piva As String

        
        hdPaginaRedirect.Value = ""
        If Not Request.QueryString("origine") Is Nothing Then

            hdPaginaRedirect.Value = Stringa_Decodifica(CStr(Request.QueryString("origine")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If
        If Not Request.QueryString("di") Is Nothing Then
            data_Da = Stringa_Decodifica(CStr(Request.QueryString("di")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If
        If Not Request.QueryString("da") Is Nothing Then
            data_A = Stringa_Decodifica(CStr(Request.QueryString("da")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If
        If Not Request.QueryString("p") Is Nothing Then
            piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If

        hf_Data_Da.Value = If(String.IsNullOrEmpty(data_Da), JToken.Parse(JsonConvert.SerializeObject(AGRODATAINIZIO)), JToken.Parse(JsonConvert.SerializeObject(CDate(data_Da))))
        hf_Data_A.Value = If(String.IsNullOrEmpty(data_A), JToken.Parse(JsonConvert.SerializeObject(AGRODATAFINE)), JToken.Parse(JsonConvert.SerializeObject(CDate(data_A))))
        hf_Piva.Value = piva

        inizializzoObjParametri()

        If Not IsPostBack Then
            inizializzoParametriPagina()
        End If

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim UtenteAbilitatoLettura = True

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("../Messaggi/AccessoNegato.htm")
        End If

        If Not Page.IsPostBack Then
            caricaControlli()
        End If

    End Sub

    Private Sub inizializzoParametriPagina()

    End Sub

    Public Shadows ReadOnly Property Master() As AgronicaStampe_2010.StampeBootstrap
        Get
            Return CType(MyBase.Master, AgronicaStampe_2010.StampeBootstrap)
        End Get
    End Property
End Class