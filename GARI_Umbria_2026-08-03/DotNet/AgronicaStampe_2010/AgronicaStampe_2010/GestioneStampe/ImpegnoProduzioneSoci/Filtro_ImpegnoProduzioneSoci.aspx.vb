Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Filtro_ImpegnoProduzioneSoci
    Inherits System.Web.UI.Page

    Private Qs_Piva As String
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)


        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        hfPiva.Value = Qs_Piva

        CType(MyBase.Master, StampeBootstrap).SetTitoloPagina(42)

    End Sub

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function CostruisciLinkStampa(params As String) As RispostaStandard

        Dim r As New RispostaStandard()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objFiltriEstrazioni As FiltriStampa = JsonConvert.DeserializeObject(params, (New FiltriStampa).GetType(), settingLoc)

        Dim jsonArrErrori As New JArray

        If String.IsNullOrWhiteSpace(objFiltriEstrazioni.Piva) Then
            jsonArrErrori.Add("Non è stato selezionato un Socio")
        End If

        If Not objFiltriEstrazioni.ValiditaInizio.HasValue Then
            jsonArrErrori.Add("Non è stata indicata la data di inizio validità")
        End If

        If jsonArrErrori.Count = 0 Then

            Dim queryString As String = "p=" & Stringa_Codifica(objFiltriEstrazioni.Piva, AgroKey_EncoderDecoder, Nothing) &
                                  "&r=" & Stringa_Codifica(objFiltriEstrazioni.RagSocSocio, AgroKey_EncoderDecoder, Nothing) &
                                  "&ta=" & Stringa_Codifica(objFiltriEstrazioni.TipoArchivio, AgroKey_EncoderDecoder, Nothing) &
                                  "&tit=" & Stringa_Codifica(objFiltriEstrazioni.Impianti, AgroKey_EncoderDecoder, Nothing) & 'tipo intervallo temporale
                                  "&vi=" & Stringa_Codifica(objFiltriEstrazioni.ValiditaInizio.Value.Date.ToString(), AgroKey_EncoderDecoder, Nothing)

            Dim obj As New JObject
            obj("queryString") = queryString
            obj("paginaDaRichiamare") = "ImpegnoProduzioneSoci.aspx"
            obj("paginaTitolo") = "ImpegnoProduzioneSoci"
            r.RispostaStringa = obj.ToString()
            r.RispostaOK = True

        Else
            r.RispostaOK = False
            r.Errore = jsonArrErrori.ToString()
        End If

        Return r

    End Function

    Private Class FiltriStampa
        Public Property Piva As String
        Public Property RagSocSocio As String
        Public Property TipoArchivio As Integer
        Public Property Impianti As Integer
        Public Property ValiditaInizio As Date?
    End Class

End Class

