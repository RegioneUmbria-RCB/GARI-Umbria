Imports System.Globalization
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class CalcolaTariffazione_WS
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function CalcolaTariffazione(ByVal data As String) As RispostaStandard
        Dim r As New RispostaStandard(False)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim dataObj = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.CalcoloTariffazioneParametri)(data, a)

            Dim calc_biz As New AgronicaCoreDomandaIrriguaBIZ.CalcoloTariffazione

            Dim ret = calc_biz.EseguiCalcolo(dataObj, objParametri_Server)

            Dim serializedResp As String = JsonConvert.SerializeObject(ret)

            r.RispostaStringa = serializedResp

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class