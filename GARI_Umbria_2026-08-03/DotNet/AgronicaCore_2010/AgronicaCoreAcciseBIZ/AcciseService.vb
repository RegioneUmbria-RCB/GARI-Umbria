Imports AgronicaCoreAcciseDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class AcciseService

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _configurazioneServizio As Configurazione_Servizio
    Private ReadOnly _configMin As AcciseServiceConfigMin
    Private ReadOnly _uploadController As UploadController
    Private ReadOnly _notificheController As NotificheController
    Private ReadOnly _soapFactory As SoapControllerFactory
    Private ReadOnly _safetyController As SafetyController

    Public Sub New(
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtente As AgronicaCoreParametri,
            ByVal objParametriSuperServer As AgronicaCoreParametri,
            ByVal configurazioneServizio As Configurazione_Servizio
        )

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _configurazioneServizio = configurazioneServizio

        _configMin = JsonConvert.DeserializeObject(Of AcciseServiceConfigMin)(_configurazioneServizio.Parametri_Extra)
        Dim dataManager = New DataManager(_objParametriServer, objParametriUtente, _objParametriSuperServer, _configMin.Debug)

        Dim username As String = dataManager.Leggi_Chiave_ACCDAA_ACC_Configurazione("UserName")
        Dim password As String = dataManager.Leggi_Chiave_ACCDAA_ACC_Configurazione("Password")

        ' Override username e password della configurazioneServizi partendo dalla tabella ACCDAA_ACC_Configurazione
        If Not String.IsNullOrEmpty(username) Then
            _configurazioneServizio.Username = username
        End If

        If Not String.IsNullOrEmpty(password) Then
            _configurazioneServizio.Password = password
        End If

        Dim fileManager = New FileManager(_configurazioneServizio.DirectoryFileEsportazioni, _configurazioneServizio.DirectoryLOG)
        fileManager.Initialize()

        Dim logger As New AcciseLogger(_objParametriServer, New LogProvider(), fileManager)
        Dim filePersister As New FileSystemPersister(logger, fileManager)
        Dim signHelper = New SignHelper(logger, _configMin, filePersister)

        _soapFactory = New SoapControllerFactory(_configMin, _configurazioneServizio, logger, signHelper)

        _uploadController = New UploadController(signHelper, logger, _soapFactory, dataManager, filePersister, fileManager)
        _notificheController = New NotificheController(signHelper, logger, _soapFactory, dataManager, filePersister, fileManager)
        _safetyController = New SafetyController(logger, _soapFactory, dataManager)

    End Sub
    Public Function LeggiMessaggiRisposte() As List(Of String)

        If Not ControllaServizioAttivo() Then
            Return New List(Of String)
        End If

        If VerificaAutorizzazioniDogane() Then
            Return _notificheController.LeggiMessaggiRisposte()
        Else
            Return New List(Of String)
        End If

    End Function
    Public Function InviaMessaggiCreati() As List(Of String)

        If Not ControllaServizioAttivo() Then
            Return New List(Of String)
        End If


        If VerificaAutorizzazioniDogane() Then
            Return _uploadController.InviaMessaggiCreati()
        Else
            Return New List(Of String)
        End If

    End Function

    Public Function LeggiEsiti() As List(Of String)

        If Not ControllaServizioAttivo() Then
            Return New List(Of String)
        End If


        If VerificaAutorizzazioniDogane() Then
            Return _notificheController.LeggiEsiti()
        Else
            Return New List(Of String)
        End If

    End Function

    Public Function ControllaServizioAttivo() As Boolean
        Return _safetyController.ServizioAttivo()
    End Function

    Public Function VerificaAutorizzazioniDogane(Optional ByVal disabilita As Boolean = True) As Boolean
        Return _safetyController.Verifica(disabilita)
    End Function

    Public Function VerificaAutorizzazioniDogane(ByVal userName As String, ByVal password As String) As Boolean
        Return _safetyController.Verifica(userName, password)
    End Function

End Class
