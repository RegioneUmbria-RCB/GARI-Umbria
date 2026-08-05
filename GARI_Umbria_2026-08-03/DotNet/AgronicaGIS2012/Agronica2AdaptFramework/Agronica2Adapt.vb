

Imports AgGateway.ADAPT.PluginManager
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Agronica2Adapt


    Private Shared _applicationId As Guid = Guid.Empty

    Public Function ScriviFileSetupSuPercorsoDaIDRicetta(PluginName As String, IdRicetta As Integer, ByVal CartellaEsportazione As String, setupDataCreator As SetupDataCreator) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim pluginLocation = AppDomain.CurrentDomain.BaseDirectory & "bin"
            Dim pluginManager = New PluginFactory(pluginLocation)

            For Each PluginName In pluginManager.AvailablePlugins
                Dim plugin = pluginManager.GetPlugin(PluginName)
                plugin.Initialize(_applicationId.ToString())
            Next

            CreateSetupFile(PluginName, CartellaEsportazione, pluginManager, setupDataCreator)

        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval

    End Function

    Private Sub CreateSetupFile(pluginName As String, CartellaEsportazione As String, pluginManager As PluginFactory, setupDataCreator As SetupDataCreator)
        Dim plugin = pluginManager.GetPlugin(pluginName)

        If plugin Is Nothing Then
        Else
            plugin.Export(setupDataCreator.GetDataModel(), CartellaEsportazione.ToString())
        End If
    End Sub
End Class
