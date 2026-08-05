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
'Imports AgronicaCoreEntityFramework_POCO

Imports AgronicaControlli_2010
Imports Newtonsoft.Json
Imports AgronicaCoreUtentiDAL

Public Class Editor
    Inherits System.Web.UI.Page


#Region "Proprietà"



#End Region

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Public permessi As PermessiUtente
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub InizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

#Region "script services Carica Griglia"

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaTestoEditor(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Cod_RisUm As Integer, ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggi As New AgronicaCoreVarieDAL.Agro_Comunicazioni_R
            r.RispostaStringa =
                leggi.Leggi(Piva, Sa_Cod, Cod_RisUm, Elem_Cod, Pro_Cod, Mat_Cod, 0, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaTestoEditor(ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Modulo As Integer,
                               ByVal Testo As String,
                               ByVal Soluzione As String,
                                ByVal Stato As Integer,
                               ByVal Testo_Parametri As String,
                               ByVal Soluzione_Parametri As String,
                               ByVal Cod_RisUm As Integer,
                                ByVal Colore As Integer,
                               ByVal Modalita As Integer,
                               ByVal Elem_Cod As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Mat_Cod As Integer) As RispostaStandard


        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim scrivi As New AgronicaCoreVarieDAL.Agro_Comunicazioni_W

            r.RispostaOK = scrivi.Aggiorna(Piva, Sa_Cod, Modulo, Testo, Soluzione, Stato, Testo_Parametri, Soluzione_Parametri, Cod_RisUm, Colore,
                                         objParametri_Server.UtenteUsername, Modalita, Elem_Cod, Pro_Cod, Mat_Cod, objParametri_Server)


            If Not r.RispostaOK Then

                r.Errore = "Errore durante la fase di aggiornamento dati"

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function

#End Region




#Region "Caricamento"

    Private Sub CaricaControlli()
    End Sub

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False

        If Request.QueryString("p") IsNot Nothing Then
            hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            If Request.QueryString("p_nc") IsNot Nothing Then
                hdPiva.Value = CStr(Request.QueryString("p_nc"))
            End If
        End If

        If Request.QueryString("Sa_Cod") IsNot Nothing Then
            hdSa_Cod.Value = CInt(Stringa_Decodifica(CStr(Request.QueryString("Sa_Cod")),
                                            AgroKey_EncoderDecoder,
                                            Server))
        Else
            If Request.QueryString("Sa_Cod_nc") IsNot Nothing Then
                hdSa_Cod.Value = CInt(Request.QueryString("Sa_Cod_nc"))
            Else
                hdSa_Cod.Value = 0
            End If
        End If

        If Request.QueryString("Cod_RisUm") IsNot Nothing Then
            hdCod_RisUm.Value = CInt(Stringa_Decodifica(CStr(Request.QueryString("Cod_RisUm")),
                                            AgroKey_EncoderDecoder,
                                            Server))
        Else
            If Request.QueryString("Cod_RisUm_nc") IsNot Nothing Then
                hdCod_RisUm.Value = CInt(Request.QueryString("Cod_RisUm_nc"))
            Else
                hdCod_RisUm.Value = 0
            End If
        End If

        If Request.QueryString("Elem_Cod") IsNot Nothing Then
            hdElem_Cod.Value = CInt(Stringa_Decodifica(CStr(Request.QueryString("Elem_Cod")),
                                            AgroKey_EncoderDecoder,
                                            Server))
        Else
            If Request.QueryString("Elem_Cod_nc") IsNot Nothing Then
                hdElem_Cod.Value = CInt(Request.QueryString("Elem_Cod_nc"))
            Else
                hdElem_Cod.Value = 0
            End If
        End If

        If Request.QueryString("Pro_Cod") IsNot Nothing Then
            hdPro_Cod.Value = CInt(Stringa_Decodifica(CStr(Request.QueryString("Pro_Cod")),
                                            AgroKey_EncoderDecoder,
                                            Server))
        Else
            If Request.QueryString("Pro_Cod_nc") IsNot Nothing Then
                hdPro_Cod.Value = CInt(Request.QueryString("Pro_Cod_nc"))
            Else
                hdPro_Cod.Value = 0
            End If
        End If

        If Request.QueryString("Mat_Cod") IsNot Nothing Then
            hdMat_Cod.Value = CInt(Stringa_Decodifica(CStr(Request.QueryString("Mat_Cod")),
                                            AgroKey_EncoderDecoder,
                                            Server))
        Else
            If Request.QueryString("Mat_Cod_nc") IsNot Nothing Then
                hdMat_Cod.Value = CInt(Request.QueryString("Mat_Cod_nc"))
            Else
                hdMat_Cod.Value = 0
            End If
        End If

        InizializzoObjParametri()

        If Not IsPostBack Then
            InizializzoParametriPagina()
        End If

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        ' TODO
        'Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
        '                                    Session("ASG_Utente_Username"),
        '                                    Session("ASG_IdServizio"),
        '                                    enum_Security_Attivita.,
        '                                    enum_Security_Operazione.Lettura,
        '                                    Date.Now,
        '                                    "",
        '                                    objParametri_Utenti)
        Dim UtenteAbilitatoLettura As Boolean = True

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = True
        hf_UtenteAbilitatoScrittura.Value = True

        If Not UtenteAbilitatoLettura Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If Not Page.IsPostBack Then
            CaricaControlli()
        End If

        If Not String.IsNullOrEmpty(hdCod_RisUm.Value) Then
            Dim leggiRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim dtCodRisum = leggiRisUm.Leggi3(CStr(hdPiva.Value), "", CInt(hdCod_RisUm.Value), 0, "", "", objParametri_Server)
            If dtCodRisum.Rows.Count > 0 Then
                Master().Lbl_Titolo.Text = dtCodRisum.Rows(0).Item("Rag_Soc") & " " & dtCodRisum.Rows(0).Item("Cognome") & " " & dtCodRisum.Rows(0).Item("Nome")
            End If
        End If
    End Sub

    Private Sub InizializzoParametriPagina()

    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property
End Class