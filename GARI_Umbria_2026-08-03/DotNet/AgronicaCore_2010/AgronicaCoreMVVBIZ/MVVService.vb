Imports AgronicaCoreDataProvider
Imports AgronicaCoreMVVBIZ.Persisters
Imports AgronicaCoreMVVCommon

Public Class MVVService

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _configMin As MVVServiceConfigMin
    Private ReadOnly _mvvController As MVVController

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtente As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
    End Sub

    Public Sub New(
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtente As AgronicaCoreParametri,
            ByVal objParametriSuperServer As AgronicaCoreParametri,
            ByVal configMin As MVVServiceConfigMin
        )

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _configMin = configMin

        Dim soapController As SoapController_MVV = Nothing

        If _configMin Is Nothing Then
            Throw New ArgumentNullException(NameOf(configMin), "Il parametro configMin non può essere nullo.")
        End If

        If Not String.IsNullOrEmpty(_configMin.UserName) AndAlso Not String.IsNullOrEmpty(_configMin.Password) AndAlso
           Not String.IsNullOrEmpty(_configMin.EnpointService) Then
            soapController = New SoapController_MVV(
                _configMin.UserName,
                _configMin.Password,
                _configMin.EnpointService,
                OttieniTimeoutServizi())
            soapController.Init(_configMin.CodOper, _configMin.PersonaFisica, _configMin.CodIcqrf)
        End If

        Dim fileManager As New FileManager(_configMin.DirectoryFileEsportazioni, _configMin.DirectoryLogv)
        fileManager.Initialize()

        Dim logger As New MVVLogger(_objParametriServer, New LogProvider(), fileManager)
        Dim fsPersister = New FileSystemPersister(fileManager, logger)
        Dim xmlValidator = New XMLValidator(fileManager)
        xmlValidator.Inizialize()

        If soapController IsNot Nothing Then
            _mvvController = New MVVController(
                objParametriServer,
                objParametriUtente,
                soapController,
                fsPersister,
                xmlValidator,
                configMin,
                logger)
        End If

    End Sub

    Public Function ControlliPreliminariMVV(ByVal piva As String, ByVal idAgenda As Integer) As List(Of String)

        Return _mvvController.ControlliPreliminariMVV(piva, idAgenda)

    End Function

    Public Function InviaMVV(ByVal piva As String, ByVal idAgenda As Integer) As List(Of String)

        Return _mvvController.InviaMVV(piva, idAgenda)

    End Function

    Public Function ConsultaMVV(ByRef listaMVV As String) As List(Of String)

        Dim errori As New List(Of String)
        listaMVV = _mvvController.ConsultaMVV(errori)

        Return errori

    End Function

    Public Function ScaricaMVV(ByVal piva As String, ByVal numMVV As String, ByRef fileMVV As Byte()) As String

        Return _mvvController.ScaricaMVV(piva, numMVV, fileMVV)

    End Function

    Public Function AnnullaMVV(ByVal piva As String, ByVal numMVV As String) As String

        Return _mvvController.AnnullaMVV(piva, numMVV)

    End Function

    Public Function PreparaDatiStampa(ByVal Piva As String, ByVal id_Agenda As Integer) As MVV

        Dim controller = New StampaController(_objParametriServer, _objParametriUtente)
        Return controller.PreparaDatiStampa(Piva, id_Agenda)

    End Function

    Private Function OttieniTimeoutServizi() As TimeSpan

        Dim timeout As Integer

        If String.IsNullOrEmpty(_configMin.ServiceTimeout) Then
            Return New TimeSpan(0, 2, 0)
        End If

        If Not Integer.TryParse(_configMin.ServiceTimeout, timeout) Then
            Return New TimeSpan(0, 2, 0)
        End If

        Return New TimeSpan(0, timeout, 0)

    End Function

End Class
