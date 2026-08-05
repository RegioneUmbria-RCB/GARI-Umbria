Imports AgronicaCoreVarieBIZ

Public Class JDeereFilesWrapper

    Private _A2Adapt As Agronica2AdaptFramework.Agronica2Adapt
    Public setupDataCreator As Agronica2AdaptFramework.SetupDataCreator

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CartellaEsportazione"></param>
    ''' <returns></returns>
    Public Function ScriviFileSetup(ByVal PluginName As String, ByVal idRicettaOperazione As Integer, ByVal CartellaEsportazione As String) As RispostaStandard
        _A2Adapt.ScriviFileSetupSuPercorsoDaIDRicetta(PluginName, idRicettaOperazione, CartellaEsportazione, setupDataCreator)

        Dim rval As New RispostaStandard
        Return rval

    End Function

    Public Sub New()
        _A2Adapt = New Agronica2AdaptFramework.Agronica2Adapt
        setupDataCreator = New Agronica2AdaptFramework.SetupDataCreator
    End Sub

End Class
