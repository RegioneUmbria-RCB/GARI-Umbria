Imports AgronicaCoreDataProvider

Public Class MVVLogger

    Private ReadOnly _objLog As LogProvider
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _logFolder As String
    Private ReadOnly _logFileName As String
    Dim _objParametriServer As AgronicaCoreParametri
    Dim customLOGParams As CustomLOGParams

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
        ByVal logProvider As LogProvider, ByVal fileManager As IFIleManager)
        _objLog = logProvider
        _fileManager = fileManager
        _logFolder = _fileManager.OttieniPercorso(MVV_E_Path.Log)
        _logFileName = _fileManager.OttieniNomeFileLog()
        _objParametriServer = objParametriServer

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = _objParametriServer.LogDescrizioneUtente,
            .LogDirectory = _logFolder,
            .LogFileName = _logFileName
        }


    End Sub

    Public Sub Logga(ByVal nomeRoutine As String, ByVal messaggio As String)
        _objLog.Scrivi_LOG(_objParametriServer, nomeRoutine, messaggio, CustomLOGParams:=customLOGParams)
    End Sub

    Public Sub Logga(ByVal nomeRoutine As String, ByVal exception As Exception)
        _objLog.Scrivi_LOG(_objParametriServer, nomeRoutine, exception.Message, CustomLOGParams:=customLOGParams)
    End Sub

End Class
