Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelloInSviluppo
Imports AgronicaCoreVarieBIZ

Public Class AlberoGerarchiaImprese2021
    Public Function GetNodesAlberoAnagrafeGetJsonData(
            cfg As AlberoGerarchiaImprese2021cfg,
            id As String,
            pathRoot As String,
            objParametri_Server As AgronicaCoreParametri,
            objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard



        Dim r As New RispostaStandard
        r.RispostaOK = True

        Dim xLeggiVersioneSql As New DataProvider
        Dim major As Integer = xLeggiVersioneSql.VersioneSqlServer_Major(objParametri_Server)

        Dim results As List(Of AjaxTreeNode_FAST_JsonObject)
        If cfg.LetturaViaSQLJson And major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            Throw New NotImplementedException
        Else

            Dim AlberoGerarchiaLeggi As New Albero_Gerarchia_Imprese_FAST
            results = AlberoGerarchiaLeggi.GetNodesGearchiaObj(pathRoot, objParametri_Server.UtenteUsername, objParametri_Server, objParametri_Utenti)

        End If



        Dim kendoHierarch As New KendoHierarchicalDataSource
        AjaxTreeNodeJsonObjectConverter.AjaxTreeNodeJsonObject_KendoHierarchical(results, kendoHierarch)

        Dim ser As New JavaScriptSerializer()
        ser.MaxJsonLength = 50000000

        r.RispostaStringa = "[" & ser.Serialize(kendoHierarch) & "]"

        Return r
    End Function
End Class
