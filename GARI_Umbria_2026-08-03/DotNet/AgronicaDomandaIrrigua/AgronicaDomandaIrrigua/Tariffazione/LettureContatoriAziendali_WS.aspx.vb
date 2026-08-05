Imports System.Globalization
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class LettureContatoriAziendali_WS
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiContatoriAziendali(ByVal piva As String, ByVal datarif As Date) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim lca_biz As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R
            Dim dt_cnt = lca_biz.LeggiContatoreAziendale(piva, datarif, datarif, objParametri_Server)
            If dt_cnt.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessun contatore valido nel periodo {0} - {1}. Impossibile proseguire", datarif, datarif))
            End If

            If dt_cnt.Rows.Count > 1 Then
                Throw New Exception(String.Format("Attenzione esistono più contatori validi alla data {0}. Impossibile recuperare il contatore a cui associare la lettura", datarif))
            End If

            r.RispostaStringa = dt_cnt.Rows(0)("Mac_Cod")

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiLettureContatore(ByVal piva As String, ByVal id_contatore As Integer, ByVal anno As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim lca_biz As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R

            Dim objRet = lca_biz.LeggiLettureContatore(piva, id_contatore, New Date(anno, 1, 1, 0, 0, 0), New Date(anno, 12, 31, 23, 59, 59), objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(objRet)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ScriviLettureContatori(ByVal data As String) As rispostaStandard(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale)
        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim dataObj = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale)(data, a)

            Dim lca_biz_W As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_W
            Dim lca_biz_R As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R

            Dim esito = lca_biz_W.ScriviLettureContatori(dataObj, objParametri_Server)
            If esito = False Then
                Throw New Exception(String.Format("Errore in inserimento\aggiornamento letture per contatore (piva: {0}", dataObj.piva))
            End If

            r.RispostaOK = True
            r.RispostaStringa = lca_biz_R.LeggiLettureContatore(dataObj.piva, 0, New Date(dataObj.anno, 1, 1), New Date(dataObj.anno, 12, 31), objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaLetturaContatore(ByVal data As String) As rispostaStandard(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale)
        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim dataObj = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.EliminaLettureContatori)(data, a)

            Dim lca_biz_W As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_W
            Dim lca_biz_R As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R
            Dim TempObj As AgronicaCoreDTOStd.InData.DomandaIrrigua.DatiContatoreAziendale

            If dataObj.id_lettura <> 0 Then
                TempObj = lca_biz_R.LeggiSingolaLetturaContatore(dataObj.id_lettura, "", 0, 0, objParametri_Server)
            End If

            Dim esito = lca_biz_W.EliminaLettureContatori(dataObj.id_lettura, dataObj.piva, dataObj.id_contatore, dataObj.startDate, dataObj.endDate, objParametri_Server)
            If esito = False Then
                Throw New Exception(String.Format("Errore in cancellazione letture contatore"))
            End If

            r.RispostaOK = True
            If dataObj.id_lettura <> 0 Then
                r.RispostaStringa = lca_biz_R.LeggiLettureContatore(dataObj.piva, 0, New Date(TempObj.anno, 1, 1), New Date(TempObj.anno, 12, 31), objParametri_Server)
            Else
                r.RispostaStringa = lca_biz_R.LeggiLettureContatore(dataObj.piva, dataObj.id_contatore, dataObj.startDate, dataObj.endDate, objParametri_Server)
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiSingolaLetturaContatore(ByVal piva As String, ByVal anno As Integer, ByVal tipolettura As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            If (piva = "" Or anno = 0 Or tipolettura = 0) Then
                Throw New Exception(String.Format("Parametri non corretti: piva {0} - anno {1} - tipolettura: {2}", piva, anno, tipolettura))
            End If

            Dim lca_biz As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R

            Dim objRet = lca_biz.LeggiSingolaLetturaContatore(0, piva, anno, tipolettura, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(objRet)

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