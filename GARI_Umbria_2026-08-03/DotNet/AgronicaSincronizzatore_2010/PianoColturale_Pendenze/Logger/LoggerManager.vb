Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Enum LogType
    Informazione = 0
    Warning = 1
    Errore = 2
    Critical = 3
End Enum

Public Enum LogLevel
    NONE = 0
    BASE = 1
    AGG_CATASTO = 2
    ERRORI = 3
    ALL = 4
End Enum

Public Enum LogDb
    NONE = 0
    ERRORI = 1
    ALL = 2
End Enum

Public Class LoggerManager
    Implements IDisposable

    Private disposedValue As Boolean
    Private _fileLogger As FileLogger = Nothing
    Private _logLevel As Integer = 0
    Private _dbLogger As DbLogger = Nothing
    Private _logDB As Integer = 0

    Private Const TIPO_LOG_PENDENZA_APPEZZAMENTO As String = "Pendenza_Appezzamento"

    Public Sub New(ByVal logPath As String,
                   ByVal logName As String,
                   ByVal TagName As String,
                   ByVal logLevel As Integer,
                   ByVal logDB As Integer,
                   Optional ByVal commitRow As Integer = 50)

        If logLevel > 0 Then
            _fileLogger = New FileLogger(logPath, logName, TagName, commitRow)
            _logLevel = logLevel
        End If

        If logDB > 0 Then
            _dbLogger = New DbLogger
            _logDB = logDB
        End If

    End Sub

    Public Sub AppendLog(ByVal Text As String,
                         Optional ByVal LogType As LogType = LogType.Informazione,
                         Optional ByVal ex As Exception = Nothing,
                         Optional ByVal bypassElastiSearch As Boolean = False)

        AppendLog(LogLevel.ALL, Text, LogType, ex)

    End Sub

    Public Sub AppendLog(ByVal Log_Level As LogLevel,
                         ByVal Text As String,
                         Optional ByVal LogType As LogType = LogType.Informazione,
                         Optional ByVal ex As Exception = Nothing,
                         Optional ByVal bypassElastiSearch As Boolean = False)

        If Not IsNothing(_fileLogger) AndAlso _logLevel >= Log_Level Then

            _fileLogger.AppendLog(Text, LogType, ex)

        End If

    End Sub

    'Public Sub AppendLog(ByVal cuaaObj As ObjNotifica(Of CUAAObj),
    '                     ByVal component As String,
    '                     ByVal Text As String,
    '                     Optional ByVal LogType As LogType = LogType.Informazione,
    '                     Optional ByVal ex As Exception = Nothing,
    '                     Optional ByVal bypassElastiSearch As Boolean = False)

    '    _fileLogger.AppendLog(Text, LogType, ex)

    'End Sub

    Public Sub LogImportazionePendenza(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Poligono As String,
                                       ByRef PendenzaDTO As AgronicaCoreDTOStd.InData.DataExchange.Pendenza, ByVal ApiStatusResp As Integer,
                                       ByVal StatoImportazione As Boolean, ByVal NoteImportazione As String,
                                       ByRef ObjParametri_Server As AgronicaCoreParametri)

        Dim strPendenzaDTO = If(IsNothing(PendenzaDTO), "", JsonConvert.SerializeObject(PendenzaDTO))

        LogImportazionePendenza(Piva, Sa_Cod, Appezza, Poligono, strPendenzaDTO, ApiStatusResp, StatoImportazione, NoteImportazione, ObjParametri_Server)

    End Sub

    Public Sub LogImportazionePendenza(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Poligono As String,
                                       ByRef StrPendenzaDTO As String, ByVal ApiStatusResp As Integer,
                                       ByVal StatoImportazione As Boolean, ByVal NoteImportazione As String,
                                       ByRef ObjParametri_Server As AgronicaCoreParametri)

        If Not IsNothing(_dbLogger) Then

            If Not StatoImportazione Or _logDB >= LogDb.ALL Then

                _dbLogger.ScriviLogInvioAnagrafe(TIPO_LOG_PENDENZA_APPEZZAMENTO, Piva, Sa_Cod, Appezza, If(StatoImportazione, 0, -1), NoteImportazione, Poligono, ApiStatusResp.ToString(), StrPendenzaDTO, ObjParametri_Server)

            Else

                _dbLogger.CancellaLogInvioAnagrafe(Piva, Sa_Cod, Appezza, ObjParametri_Server)

            End If

        End If

    End Sub

    Public Sub Flush()

        If Not IsNothing(_fileLogger) Then
            _fileLogger.Flush()
        End If

    End Sub

    Public Sub Flush(ByVal key As String)

        If Not IsNothing(_fileLogger) Then
            _fileLogger.Flush()
        End If

    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: eliminare lo stato gestito (oggetti gestiti)
                If Not IsNothing(_fileLogger) Then
                    _fileLogger.Dispose()
                End If
                If Not IsNothing(_dbLogger) Then
                    _dbLogger.Dispose()
                End If
            End If

            ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
            ' TODO: impostare campi di grandi dimensioni su Null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: eseguire l'override del finalizzatore solo se 'Dispose(disposing As Boolean)' contiene codice per liberare risorse non gestite
    ' Protected Overrides Sub Finalize()
    '     ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
