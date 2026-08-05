Imports AgronicaCoreDataProvider

Public Class AcciseLogger

    Private ReadOnly _objLog As LogProvider
    Private ReadOnly _fileManager As IFileManager
    Private ReadOnly _logFolder As String
    Private ReadOnly _logFileName As String

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private _customLOGParams As CustomLOGParams

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
        ByVal logProvider As LogProvider, ByVal fileManager As IFileManager)
        _objLog = logProvider
        _fileManager = fileManager
        _logFolder = _fileManager.OttieniPercorso(AccisePath.Log)
        _logFileName = _fileManager.OttieniNomeFileLog()
        _objParametriServer = objParametriServer

        _customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = _objParametriServer.LogDescrizioneUtente,
            .LogDirectory = _logFolder,
            .LogFileName = _logFileName
        }

    End Sub
    Public Sub Logga(ByVal nomeRoutine As String, ByVal messaggio As String)
        _objLog.Scrivi_LOG(_objParametriServer, nomeRoutine, messaggio, CustomLOGParams:=_customLOGParams)
    End Sub
    Public Sub Logga(ByVal nomeRoutine As String, ByVal exception As Exception)
        _objLog.Scrivi_LOG(_objParametriServer, nomeRoutine, exception.Message, CustomLOGParams:=_customLOGParams)
    End Sub

End Class
Public Enum AccisePath

    Root = 0
    Log = 30
    Temp = 40
    Signed = 50

End Enum