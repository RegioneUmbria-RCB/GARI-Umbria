Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreAuditBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Audit
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.Audit
Imports AgronicaCoreModelsSTD.Utility
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Audit
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRegolamenti(ByVal tipo As Integer, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditRegolamentiModel))
        Dim r As New rispostaStandard(Of List(Of AuditRegolamentiModel))
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim xLettura As New AuditAgronicaWS(objParametri_Server)
            Dim rval As List(Of AuditRegolamentiModel) = xLettura.LeggiRegolamenti(tipo)
            'Dim xLettura As New AuditRegolamenti_R
            'Dim rval As List(Of AuditRegolamentiModel) = xLettura.Leggi(tipo, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiStati(ByVal tipo As Integer, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditStatiModel))
        Dim r As New rispostaStandard(Of List(Of AuditStatiModel))
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim xLettura As New AuditAgronicaWS(objParametri_Server)
            Dim rval As List(Of AuditStatiModel) = xLettura.LeggiStati(tipo)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCampi(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditCampiModel))
        Dim r As New rispostaStandard(Of List(Of AuditCampiModel))
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim xLettura As New AuditAgronicaWS(objParametri_Server)
            Dim rval As List(Of AuditCampiModel) = xLettura.LeggiCampi(tipo, cod_reg)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDisposizioni(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal campo As Integer, ByVal cod_disp As Integer, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditDisposizioniModel))
        Dim r As New rispostaStandard(Of List(Of AuditDisposizioniModel))
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim xLettura As New AuditAgronicaWS(objParametri_Server)
            Dim rval As List(Of AuditDisposizioniModel) = xLettura.LeggiDisposizioni(tipo, cod_reg, campo, cod_disp)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiSezioni(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_dis As Integer, ByVal parte As Integer, ByVal data As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditSezioniModel))
        Dim r As New rispostaStandard(Of List(Of AuditSezioniModel))
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim xLettura As New AuditAgronicaWS(objParametri_Server)
            Dim rval As List(Of AuditSezioniModel) = xLettura.LeggiSezioni(tipo, cod_reg, cod_dis, parte, data)
            'Dim xLettura As New AuditSezioni_R
            'Dim rval As List(Of AuditSezioniModel) = xLettura.Leggi(tipo, cod_reg, cod_dis, parte, data_validita, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDisposizioniAttive(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal piva As String, ByVal data As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditDisposizioniModel))
        Dim r As New rispostaStandard(Of List(Of AuditDisposizioniModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim data_rif As Date = AGRODATAINIZIO
            If data <> "" Then
                data_rif = CDate(data)
            End If
            Dim rval As List(Of AuditDisposizioniModel) = xLettura.LeggiDisposizioniAttive(tipo, cod_reg, piva, data_rif, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDisposizioniAttiveTrasporti(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal disposizione_cod As Integer, ByVal piva As String, ByVal data As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditDisposizioniModel))
        Dim r As New rispostaStandard(Of List(Of AuditDisposizioniModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim data_rif As Date = AGRODATAINIZIO
            If data <> "" Then
                data_rif = CDate(data)
            End If
            Dim rval As List(Of AuditDisposizioniModel) = xLettura.LeggiDisposizioniAttive(tipo, cod_reg, piva, data_rif, objParametri_Server, disposizione_cod)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCodici(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_aud As Integer, ByVal cod_dis As Integer, ByVal cod_sez As Integer, ByVal data As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditRisposteModel))
        Dim r As New rispostaStandard(Of List(Of AuditRisposteModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim rval As List(Of AuditRisposteModel) = xLettura.LeggiCodici(tipo, cod_reg, cod_aud, cod_dis, cod_sez, data, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCodiciDefault(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_dis As Integer, ByVal cod_sez As Integer, ByVal data As String, ByVal risposte As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditFormModel))
        Dim r As New rispostaStandard(Of List(Of AuditFormModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim rval As List(Of AuditFormModel) = xLettura.LeggiCodiciDefault(tipo, cod_reg, cod_dis, cod_sez, data, risposte, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCodiciDeroga(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_dis As Integer, ByVal cod_sez As Integer, ByVal data As String, ByVal risposte As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditFormModel))
        Dim r As New rispostaStandard(Of List(Of AuditFormModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim rval As List(Of AuditFormModel) = xLettura.LeggiCodiciDeroga(tipo, cod_reg, cod_dis, cod_sez, data, risposte, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiInterviste(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_int As Integer, ByVal piva As String, ByVal inizio As String, ByVal fine As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditIntervisteModel))
        Dim r As New rispostaStandard(Of List(Of AuditIntervisteModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditProfilazione
            Dim data_inizio As Date = AGRODATAINIZIO
            Dim data_fine As Date = AGRODATAFINE
            If inizio IsNot Nothing Then
                data_inizio = CDate(inizio)
            End If
            If fine IsNot Nothing Then
                data_fine = CDate(fine)
            End If
            Dim rval As List(Of AuditIntervisteModel) = xLettura.LeggiInterviste(tipo, cod_reg, cod_int, piva, data_inizio, data_fine, 0, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiIntervisteKendo(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_int As Integer, ByVal piva As String, ByVal inizio As String, ByVal fine As String, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditProfilazione
            Dim data_inizio As Date = AGRODATAINIZIO
            Dim data_fine As Date = AGRODATAFINE
            If inizio IsNot Nothing Then
                data_inizio = CDate(inizio)
            End If
            If fine IsNot Nothing Then
                data_fine = CDate(fine)
            End If
            Dim rval As List(Of AuditIntervisteModel) = xLettura.LeggiInterviste(tipo, cod_reg, cod_int, piva, data_inizio, data_fine, 0, objParametri_Server)
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(rval.ToList(), Formatting.None, serializerSettings)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviInterviste(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_int As Integer, ByVal nome_int As String, ByVal piva As String, ByVal inizio As String, ByVal fine As String, ByVal risposte As String, ByVal objP_server As String) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xScrittura As New AuditProfilazione
            Dim rval As Boolean = False
            Dim data_inizio As Date = AGRODATAINIZIO
            Dim data_fine As Date = AGRODATAFINE
            If inizio <> "" Then
                data_inizio = CDate(inizio)
            End If
            If fine <> "" Then
                data_fine = CDate(fine)
            End If
            rval = xScrittura.ScriviInterviste(tipo, cod_reg, cod_int, nome_int, piva, data_inizio, data_fine, risposte, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaInterviste(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_int As Integer, ByVal piva As String, ByVal objP_server As String) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xScrittura As New AuditProfilazione
            Dim rval As Boolean = False
            If cod_int <> 0 Then
                rval = xScrittura.CancellaInterviste(tipo, cod_reg, cod_int, piva, objParametri_Server)
            End If
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRisposteInterviste(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_int As Integer, ByVal piva As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditRisposteIntervisteModel))
        Dim r As New rispostaStandard(Of List(Of AuditRisposteIntervisteModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            Dim xLettura As New AuditProfilazione
            Dim rval As List(Of AuditRisposteIntervisteModel) = xLettura.LeggiRisposteInterviste(tipo, cod_reg, cod_int, piva, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRisposteIntervisteKendo(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_int As Integer, ByVal piva As String, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            Dim xLettura As New AuditProfilazione
            Dim rval As List(Of AuditRisposteIntervisteModel) = xLettura.LeggiRisposteInterviste(tipo, cod_reg, cod_int, piva, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(rval.ToList(), Formatting.None, serializerSettings)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAudit(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_aud As Integer, ByVal piva As String, ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of AuditModel))
        Dim r As New rispostaStandard(Of List(Of AuditModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
        Try
            Dim xLettura As New AuditCheckList
            Dim rval As List(Of AuditModel) = xLettura.LeggiAudit(cod_aud, tipo, cod_reg, piva, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, objParametri_Utenti)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAuditKendo(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal piva As String, ByVal inizio As String, ByVal fine As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
        Try
            Dim xLettura As New AuditCheckList
            Dim data_inizio As Date = AGRODATAINIZIO
            Dim data_fine As Date = AGRODATAFINE
            If inizio IsNot Nothing Then
                data_inizio = CDate(inizio)
            End If
            If fine IsNot Nothing Then
                data_fine = CDate(fine)
            End If
            Dim rval As List(Of AuditModel) = xLettura.LeggiAudit(0, tipo, cod_reg, piva, data_inizio, data_fine, objParametri_Server, objParametri_Utenti)
            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(rval.ToList(), Formatting.None, serializerSettings)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviAudit(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_aud As Integer, ByVal piva As String, ByVal data As String, ByVal stato As String, ByVal note As String, ByVal Campionato As Integer, ByVal Rintracciabilita As Integer, ByVal risposte As String, ByVal calcola As String, ByVal objP_server As String) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xScrittura As New AuditCheckList
            Dim rval As Boolean = False
            If data <> "" Then
                rval = xScrittura.ScriviAudit(tipo, cod_reg, cod_aud, piva, CDate(data), stato, note, Campionato, risposte, calcola, objParametri_Server, Rintracciabilita)
            End If
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaAudit(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_aud As Integer, ByVal piva As String, ByVal objP_server As String) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xScrittura As New AuditCheckList
            Dim rval As Boolean = False
            If cod_aud <> 0 Then
                rval = xScrittura.CancellaAudit(tipo, cod_reg, cod_aud, piva, objParametri_Server)
            End If
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAuditRisposte(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_aud As Integer, ByVal cod_dis As Integer, ByVal parte As Integer, ByVal data As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditRisposteModel))
        Dim r As New rispostaStandard(Of List(Of AuditRisposteModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim rval As List(Of AuditRisposteModel) = xLettura.LeggiAuditRisposte(tipo, cod_reg, cod_aud, cod_dis, parte, data, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAuditPunteggi(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal cod_aud As Integer, ByVal piva As String, ByVal data As String, ByVal objP_server As String) As rispostaStandard(Of List(Of AuditPunteggiModel))
        Dim r As New rispostaStandard(Of List(Of AuditPunteggiModel))
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim data_val As Date = AGRODATAINIZIO
            If data <> "" Then
                data_val = CDate(data)
            End If
            Dim xLettura As New AuditCheckList
            Dim rval As List(Of AuditPunteggiModel) = xLettura.LeggiAuditPunteggi(tipo, cod_reg, cod_aud, piva, data_val, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CalcolaLivello(ByVal tipo As Integer, ByVal cod_reg As Integer, ByVal piva As String, ByVal cod_dis As Integer, ByVal cod_sez As Integer, ByVal livello As String, ByVal risposte As String, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim xLettura As New AuditCheckList
            Dim rval As String = xLettura.CalcolaLivello(tipo, cod_reg, piva, cod_dis, cod_sez, livello, risposte, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = rval
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetChecklistEUDR(InData As Object) As rispostaStandard(Of ChecklistEUDR)

        Dim r As New rispostaStandard(Of ChecklistEUDR)

        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiChecklistEUDR))(JsonConvert.SerializeObject(InData))
        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

        Try

            Dim data As Date = Date.Now.Date
            Dim tipo As Integer = enum_AuditPuaTipo.Audit_Fornitori_EUDR_INALCA
            Dim cod_reg As Integer = 1
            Dim cod_aud As Integer = 0
            Dim piva As String = objParametri_Server.PivaSuperUser
            Dim cod_fornitore As String = objRequest.InData.cod_fornitore

            Dim checklistEUDR As New ChecklistEUDR
            Dim objAuditCheckList As New AuditCheckList
            Dim objAudit As New AgronicaCoreAuditDAL.Audit_R
            Dim riferimento As String = LeggiRiferimentoEUDR(piva, cod_fornitore, objParametri_Server)
            Dim dtAudit = objAudit.Leggi(cod_aud, tipo, cod_reg, piva, AGRODATAINIZIO, AGRODATAFINE, "", "Validita_Inizio DESC", objParametri_Server, Riferimento_Cod:=riferimento)

            If dtAudit.Rows.Count > 0 Then

                cod_aud = dtAudit.Rows(0).Item("Audit_Cod")
                checklistEUDR = LeggiChecklistEUDR(tipo, cod_reg, cod_aud, piva, objParametri_Server, objParametri_Utenti)

                ' genera link per la checklist
                If checklistEUDR.pubblica = 1 Then
                    checklistEUDR.link = objAuditCheckList.GeneraLinkAudit(tipo, cod_reg, cod_aud, piva, objParametri_Server, objParametri_Super_Server)
                End If

            End If

            r.RispostaStringa = checklistEUDR
            r.RispostaOK = True

        Catch ex As Exception

            r.Errore = ex.Message
            r.RispostaOK = False

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PostChecklistEUDR(InData As Object) As rispostaStandard(Of ChecklistEUDR)

        Dim r As New rispostaStandard(Of ChecklistEUDR)

        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ScriviChecklistEUDR))(JsonConvert.SerializeObject(InData))
        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

        Try

            Dim tipo As Integer = enum_AuditPuaTipo.Audit_Fornitori_EUDR_INALCA
            Dim cod_reg As Integer = 1
            Dim cod_aud As Integer = 0
            Dim piva As String = objParametri_Server.PivaSuperUser
            Dim cod_fornitore As String = objRequest.InData.cod_fornitore
            Dim stato As Integer = 3 ' Non Compliant
            Dim data As Date = If(objRequest.InData.data <= AGRODATAINIZIO, Date.Now.Date, objRequest.InData.data.Date)
            Dim pubblica As Integer = objRequest.InData.pubblica
            Dim email As String = If(objRequest.InData.email, "")
            Dim note As String = If(objRequest.InData.note, "")
            Dim nuovo As Boolean = True
            Dim errore As String = ""

            Dim checklistEUDR As New ChecklistEUDR
            Dim objAuditCheckList As New AuditCheckList
            Dim objAudit As New AgronicaCoreAuditDAL.Audit_R
            Dim riferimento As String = LeggiRiferimentoEUDR(piva, cod_fornitore, objParametri_Server)
            Dim dtAudit = objAudit.Leggi(cod_aud, tipo, cod_reg, piva, AGRODATAINIZIO, AGRODATAFINE, "", "Validita_Inizio DESC", objParametri_Server, Riferimento_Cod:=riferimento)

            ' verifico se l'audit esiste già
            If dtAudit.Rows.Count > 0 Then
                cod_aud = dtAudit.Rows(0).Item("Audit_Cod")
                Dim data_fine As Date = dtAudit.Rows(0).Item("Validita_Fine")
                'creo la nuova solo è successiva alla scadenza di quella presente
                If data_fine > data Then
                    nuovo = False
                Else
                    data = data_fine.AddDays(1)
                End If
            End If

            ' forza la creazione se non esiste o se è nuovo
            If nuovo OrElse cod_aud = 0 Then
                pubblica = If(String.IsNullOrEmpty(email), pubblica, 1)
                cod_aud = objAuditCheckList.ScriviAuditRisposte(tipo, cod_reg, 0, piva, riferimento, data, stato, note, pubblica, "[]", errore, objParametri_Server)
            End If

            If cod_aud <> 0 Then

                checklistEUDR = LeggiChecklistEUDR(tipo, cod_reg, cod_aud, piva, objParametri_Server, objParametri_Utenti)

                ' genera link per la checklist
                If checklistEUDR.pubblica = 1 Then
                    checklistEUDR.link = objAuditCheckList.GeneraLinkAudit(tipo, cod_reg, cod_aud, piva, objParametri_Server, objParametri_Super_Server)
                End If

                ' invia mail se il destinatario è specificato e il link è presente
                If Not String.IsNullOrEmpty(email) AndAlso Not String.IsNullOrEmpty(checklistEUDR.link) Then
                    Dim oggetto As String = "Questionario EUDR - " & checklistEUDR.fornitore
                    Dim testo As String = "Gentilissimo,<br/><br/>" &
                    "La informiamo che è stato creato un nuovo questionario EUDR per il fornitore: <b>" & checklistEUDR.fornitore & "</b>.<br/><br/>" &
                    "Per visualizzare il questionario, clicca sul seguente link: <a href='" & checklistEUDR.link & "'>Questionario EUDR</a><br/><br/>" &
                    "Cordiali saluti,<br/>Il Team di INALCA"
                    errore = objAuditCheckList.InviaMailAudit(email, oggetto, testo, objParametri_Server)
                End If

            End If

            r.RispostaStringa = checklistEUDR
            r.RispostaOK = True
            r.Errore = errore

        Catch ex As Exception

            r.Errore = ex.Message
            r.RispostaOK = False

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetCompletamentoCheckList(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel))

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try
            Dim objAuditCheckList As New AuditCheckList
            Dim listCompletamentoCheckList As New List(Of AgronicaCoreModelsSTD.Audit.AuditCompletamentoModel)

            listCompletamentoCheckList = objAuditCheckList.CompletamentoChecklist(objParametri_Utenti, objParametri_Server)

            r.RispostaStringa = listCompletamentoCheckList
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try

        Return r

    End Function

    Private Function LeggiChecklistEUDR(ByVal tipo As Integer, ByVal cod_reg As Integer,
                                       ByVal cod_aud As Integer, ByVal piva As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri) As ChecklistEUDR

        Dim checklistEUDR As New ChecklistEUDR
        Dim objAuditCheckList As New AuditCheckList
        Dim json As String = objAuditCheckList.LeggiAuditJson(cod_aud, tipo, cod_reg, piva, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, objParametri_Utenti)
        Dim jsonArray As JArray = JArray.Parse(json)

        If jsonArray.Count > 0 Then
            Dim audit = jsonArray(0)
            Dim riferimento = audit("Riferimento_Cod").ToString().Split("_")
            Dim cod_fornitore As String = audit("CodFornitore")
            If String.IsNullOrEmpty(cod_fornitore) Then
                cod_fornitore = If(riferimento.Length > 2, riferimento(2), "")
            End If
            checklistEUDR.codice = audit("Audit_Cod")
            checklistEUDR.stato = audit("Audit_Stato")
            checklistEUDR.data = audit("Validita_Inizio")
            checklistEUDR.data_scadenza = audit("Validita_Fine")
            checklistEUDR.cod_fornitore = cod_fornitore
            checklistEUDR.fornitore = audit("Riferimento_Des")
            checklistEUDR.score = audit("Num_Registrazioni")
            checklistEUDR.pubblica = audit("Campionato")
            checklistEUDR.note = audit("Note")
        End If

        Return checklistEUDR

    End Function

    Private Function LeggiRiferimentoEUDR(ByVal piva As String, ByVal cod_fornitore As String, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dtContatti = objContatti.LeggiFornitoriEUDR(piva, "", objParametri_Server, Cod_Fornitore:=cod_fornitore)

        If dtContatti.Rows.Count > 0 Then
            cod_fornitore = dtContatti.Rows(0).Item("Cod_Contatto")
        End If

        Dim riferimento As String = "8_" & piva & "_" & cod_fornitore

        Return riferimento

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetCheckListManagementData(InData As CoreWS_Generic(Of Object))
        Dim r As New RispostaStandard
        Dim params As New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)            
        }
        Try
            Dim objAuditCheckList As New AuditCheckList
            Dim auditTypes = objAuditCheckList.LeggiTipiAudit(params)
            Dim services = objAuditCheckList.LeggiServiziChecklist(params)
            Dim settingsValue = objAuditCheckList.LeggiAuditImpostazioni(Enum_Audit_impostazione.Documentale_GestioneChecklist, params)
            Dim resultObject As New With {
                .auditTypes = auditTypes,
                .services = services,
                .settingValue = settingsValue
            }
            r.RispostaStringa = JsonConvert.SerializeObject(resultObject)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try
        Return r
    End Function

     <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SaveCheckListManagement(InData As CoreWS_Generic(Of string))
        Dim r As New RispostaStandard
        Dim params As New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)            
        }
        Try
            Dim objAuditCheckList As New AuditCheckList
            objAuditCheckList.ScriviAuditImpostazioni(Enum_Audit_impostazione.Documentale_GestioneChecklist, InData.InData, params)
            r.RispostaStringa = JsonConvert.SerializeObject("OK")
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetWorkflowManagementData(InData As CoreWS_Generic(Of Object))
        Dim r As New RispostaStandard
        Dim params As New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)            
        }
        Try
            Dim objAuditCheckList As New AuditCheckList
            Dim areas = objAuditCheckList.LeggiAreeDocumentale(params)
            Dim services = objAuditCheckList.LeggiServiziWorkflow(params)
            Dim settingsValue = objAuditCheckList.LeggiAuditImpostazioni(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, params)
            Dim resultObject As New With {
                .areas = areas,
                .services = services,
                .settingValue = settingsValue
            }
            r.RispostaStringa = JsonConvert.SerializeObject(resultObject)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SaveWorkflowManagement(InData As CoreWS_Generic(Of Object))
        Dim r As New RispostaStandard
        Dim params As New ObjParams With {
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)            
        }
        Try
            Dim objAuditCheckList As New AuditCheckList
            objAuditCheckList.ScriviAuditImpostazioni(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, InData.InData, params)
            r.RispostaStringa = JsonConvert.SerializeObject("OK")
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try

        Return r
    End Function
End Class