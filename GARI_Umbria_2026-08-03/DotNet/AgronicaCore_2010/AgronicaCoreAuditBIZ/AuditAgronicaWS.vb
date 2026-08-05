Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class AuditAgronicaWS

    Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim urlAuditWS As String = ""

    Public Sub New(ByVal _objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        objParametri = _objParametri
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "GiasOnline_WS_Audit_IAgroAPI_Audit", "", "", objParametri)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            urlAuditWS = DTConfigSiti.Rows(0).Item("Valore")
        Else
            urlAuditWS = "http://localhost:52558/agronicawebservice/AgroAPI_Audit.svc"
        End If
    End Sub

    Private Function ChiamaAgroAPI(ByVal api As String, ByVal request As String, ByVal auth As String) As String
        Dim url As String = urlAuditWS & "/" & api
        Dim hlpHttp As New AgronicaCoreUtility.Http
        Dim rval As String = ""
        Dim hdr As System.Net.WebHeaderCollection
        If auth <> "" Then
            hdr = New System.Net.WebHeaderCollection
            hdr.Add("Authorization", auth)
        End If
        rval = hlpHttp.chiamaWS(request, Nothing, url, "application/json", "POST", "application/json", "", hdr)
        Return rval
    End Function

    Public Function LeggiRegolamenti(ByVal Audit_Tipo As Integer) As List(Of AuditRegolamentiModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditRegolamentiModel))
            Dim rval As String = ChiamaAgroAPI("AuditRegolamenti", "{""tipo"":" & Audit_Tipo & "}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditRegolamentiModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiStati(ByVal Audit_Tipo As Integer) As List(Of AuditStatiModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditStatiModel))
            Dim rval As String = ChiamaAgroAPI("AuditStati", "{""tipo"":" & Audit_Tipo & "}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditStatiModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiDomandeInterviste(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer) As List(Of AuditDomandeIntervisteModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditDomandeIntervisteModel))
            Dim rval As String = ChiamaAgroAPI("AuditDomandeInterviste", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & "}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditDomandeIntervisteModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiDomandeDisposizioni(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disposizione_Cod As Integer) As List(Of AuditDomandeDisposizioniModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditDomandeDisposizioniModel))
            Dim rval As String = ChiamaAgroAPI("AuditDomandeDisposizioni", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""cod_dis"":" & Disposizione_Cod & "}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditDomandeDisposizioniModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiCampi(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer) As List(Of AuditCampiModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditCampiModel))
            Dim rval As String = ChiamaAgroAPI("AuditCampi", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & "}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditCampiModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiDisposizioni(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Campo As Integer, ByVal Disposizione_Cod As Integer) As List(Of AuditDisposizioniModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditDisposizioniModel))
            Dim rval As String = ChiamaAgroAPI("AuditDisposizioni", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""campo"":" & Campo & ", ""cod_disp"":" & Disposizione_Cod & "}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditDisposizioniModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiSezioni(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disposizione_Cod As Integer, ByVal Parte As Integer, ByVal Data As String) As List(Of AuditSezioniModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditSezioniModel))
            Dim rval As String = ChiamaAgroAPI("AuditSezioni", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""cod_dis"":" & Disposizione_Cod & ", ""parte"":" & Parte & ", ""data"":""" & Data & """}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditSezioniModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiCodici(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disposizione_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Data As String, Optional ByVal xFiltroAggiuntivo As String = "") As List(Of AuditCodiciModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditCodiciModel))
            Dim rval As String = ChiamaAgroAPI("AuditCodici", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""cod_dis"":" & Disposizione_Cod & ", ""cod_sez"":" & Sezione_Cod & ", ""cod_def"":0, ""data"":""" & Data & """, ""filtroaggiuntivo"":""" & xFiltroAggiuntivo & """}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditCodiciModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiCodiciDefault(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disposizione_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Data As String) As List(Of AuditCodiciModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditCodiciModel))
            Dim rval As String = ChiamaAgroAPI("AuditCodici", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""cod_dis"":" & Disposizione_Cod & ", ""cod_sez"":" & Sezione_Cod & ", ""cod_def"":1, ""data"":""" & Data & """}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditCodiciModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiCodiciDeroga(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disposizione_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Data As String) As List(Of AuditCodiciModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditCodiciModel))
            Dim rval As String = ChiamaAgroAPI("AuditCodici", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""cod_dis"":" & Disposizione_Cod & ", ""cod_sez"":" & Sezione_Cod & ", ""cod_def"":2, ""data"":""" & Data & """}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditCodiciModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiCodiciDisposizioni(ByVal Audit_Tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disposizione_Cod As Integer, ByVal Parte As Integer, ByVal Data As String) As List(Of AuditCodiciModel)
        Try
            Dim r As New rispostaStandard(Of List(Of AuditCodiciModel))
            Dim rval As String = ChiamaAgroAPI("AuditCodiciDisposizioni", "{""tipo"":" & Audit_Tipo & ", ""cod_reg"":" & Regolamento_Cod & ", ""cod_dis"":" & Disposizione_Cod & ", ""parte"":" & Parte & ", ""data"":""" & Data & """}", "")
            Dim obj As JObject = JsonConvert.DeserializeObject(rval)
            r = obj.ToObject(Of rispostaStandard(Of List(Of AuditCodiciModel)))()
            Return r.RispostaStringa
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class
