

Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports AgronicaCoreWinsortDAL
Imports AgronicaCoreWinsortBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelloInSviluppo
Imports Agronica.Helpers.GiasBase
Public Class AlberoGerarchiaImprese2021
    Inherits System.Web.UI.Page



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetNodesAlberoGerachiaImprese(
        ByVal cfgSerialized As String,
        ByVal id As String, ByVal PathRoot As String,
        ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim cfg As AlberoGerarchiaImprese2021cfg = JsonConvert.DeserializeObject(Of AlberoGerarchiaImprese2021cfg)(cfgSerialized)

        Dim linkGiasBase As String = ""
        If PathRoot = "" Then
            GiasBaseHelper.Setta_Link_GiasBase(objParametri_Server, linkGiasBase)
            PathRoot = linkGiasBase + "agronica"
        End If

        Try

            Dim LetturaAlbero As New AgronicaControlli_2010.AlberoGerarchiaImprese2021

            objParametri_Server.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
            objParametri_Server.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()

            objParametri_Utenti.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
            objParametri_Utenti.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()

            r = LetturaAlbero.GetNodesAlberoAnagrafeGetJsonData(cfg, id, PathRoot, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

End Class