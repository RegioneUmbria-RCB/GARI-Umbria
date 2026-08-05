Imports System.Text
Imports AgronicaCoreVarieDAL
Imports log4net

Public Class Logger
    Private LogErrori As StringBuilder
    Private _mailLogger As MailLogger = Nothing
    Private _log As ILog = Nothing
    Private _dbLogger As DataBaseLogger = Nothing

    Public ReadOnly Property Logger() As ILog
        Get
            Return _log
        End Get
    End Property


    Public Sub New(configurazione As Configurazione_Servizio, configurazioneImportatore As ConfigurazioneImportatore, objParametri As ObjParametri)
        LogErrori = New StringBuilder()
        _log = LogManager.GetLogger(configurazioneImportatore.IdServizio)
        _mailLogger = New MailLogger(Dammi_Configurazione_Mail_Logger(configurazione), objParametri.Server)
        _dbLogger = New DataBaseLogger()
    End Sub

    Public Sub New(objParametri As ObjParametri, logName As String)
        LogErrori = New StringBuilder()
        _log = LogManager.GetLogger(logName)
        _mailLogger = New MailLogger(Dammi_Configurazione_Mail_Logger(New Configurazione_Servizio), objParametri.Server)
        _dbLogger = New DataBaseLogger()
    End Sub

    Public Sub Info(ByVal msg As String)
        Try
            _log.Info(msg)
        Catch ex As Exception
        End Try

    End Sub

    Public Sub InfoFormat(format As String, ParamArray args() As Object)
        Try
            _log.InfoFormat(format, args)
        Catch ex As Exception
        End Try

    End Sub

    Public Sub Debug(ByVal msg As String)
        Try
            _log.Debug(msg)
        Catch ex As Exception
        End Try

    End Sub
    Public Sub Warn(ByVal msg As String)
        Try
            _log.Warn(msg)
        Catch ex As Exception
        End Try

    End Sub

    Public Sub WarnFormat(format As String, ParamArray args() As Object)
        Try
            _log.WarnFormat(format, args)
        Catch ex As Exception
        End Try

    End Sub

    Public Sub Fatal(ByVal msg As String)
        Try
            _log.Fatal(msg)
        Catch ex As Exception
        End Try

    End Sub
    Public Sub [Error](ByVal msg As String)
        Try
            LogErrori.AppendLine(msg)
            _log.Error(msg)
            _mailLogger.Error("Errore durante l'esecuzione di un Importatore / Esportatore", msg)
        Catch ex As Exception
        End Try

    End Sub

    Public Sub ErrorFormat(format As String, ParamArray args() As Object)
        Try
            _log.ErrorFormat(format, args)
            _mailLogger.Error("Errore", String.Format(format, args))
        Catch ex As Exception
        End Try

    End Sub
    Friend Function GetLogErrori() As String
        Return LogErrori.ToString()
    End Function
    Private Function Dammi_Configurazione_Mail_Logger(configurazione As Configurazione_Servizio) As ConfigurazioneSmtpClient

        Return New ConfigurazioneSmtpClient With
            {
                .Destinatari = configurazione.Destinatari,
                .Host_Smtp = "",
                .Host_Smtp_Porta = "",
                .Mittente = configurazione.Mittente,
                .Smtp_Enablessl = "",
                .Smtp_Password = "",
                .Smtp_Utente = ""
            }

    End Function


End Class