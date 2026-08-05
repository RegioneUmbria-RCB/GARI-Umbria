Imports System.Globalization
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class DomandaIrrigua_WS
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function ApriSchedaDomanda(ByVal id As Integer, ByVal dataOri As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim paginaLink As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            paginaLink = "../DomandeIrrigue/DomandaIrrigua.aspx"
            paginaLink &= "?i=" & Stringa_Codifica(id, AgroKey_EncoderDecoder)
            paginaLink &= "&d=" & Stringa_Codifica(dataOri, AgroKey_EncoderDecoder)

            r.RispostaOK = True
            r.RispostaStringa = paginaLink
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetUrlIndietro(ByVal codPagina As Integer, ByVal data As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim paginaLink As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Select Case codPagina
                Case TipiEnumerativi.enum_PagineAgronicaDomandaIrrigua.ElencoDomande
                    paginaLink = "../DomandeIrrigue/RicercaDomandeIrrigue.aspx"
                    paginaLink &= "?d=" & Stringa_Codifica(data, AgroKey_EncoderDecoder)
            End Select



            r.RispostaOK = True
            r.RispostaStringa = paginaLink
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDomanda(ByVal id As Integer, ByVal piva As String, ByVal anno As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim di_biz As New AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R
            Dim data As AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua
            If di_biz.CheckExistDomandaIrrigua(id, piva, anno, objParametri_Server) = True Then
                data = di_biz.LeggiDomanda(id, piva, anno, objParametri_Server)
            Else
                data = di_biz.LeggiPianoColturaleConParticellePerInizializzazioneDomandaIrrigua(piva, anno, objParametri_Server)
            End If


            r.RispostaStringa = JsonConvert.SerializeObject(data)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ScriviAggiornaDomanda(ByVal data As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim dataObj = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua)(data, a)

            Dim di_biz_W As New AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_W
            Dim di_biz_R As New AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R

            Dim esito = di_biz_W.ScriviAggiornaDomandaIrrigua(dataObj, objParametri_Server)
            If esito = False Then
                Throw New Exception("Errore in inserimento\aggiornamento domanda irrigua ")
            End If

            Dim dataRet = di_biz_R.LeggiDomanda(dataObj.id, "", 0, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dataRet)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiElencoDomandeIrrigue(ByVal data As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()
        Try
            Dim dataObj = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.LeggiElencoDomandeIrrigue)(data, a)

            Dim di_biz_R As New AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R

            Dim dataRet = di_biz_R.LeggiElencoDomande(dataObj, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(dataRet)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try
        Return r
    End Function



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class