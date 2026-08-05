Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDemetraBIZ
Imports AgronicaCoreDTOStd.InData.Notifiche

Public Class LoggerManager
    Implements IDisposable

    Private _coldirettiLogger As ElasticSearchLogger = Nothing
    Private _fileLogger As FileLogger = Nothing
    Private disposedValue As Boolean

    Public Sub New(ByVal environment As String,
                   ByVal logPath As String,
                   ByVal logName As String,
                   ByVal TagName As String,
                   Optional ByVal commitRow As Integer = 50,
                   Optional ByVal urlColdirettiLogger As String = "",
                   Optional ByVal enableElasticSearchNotify As Boolean = False,
                   Optional ByVal inviaSoloErrori As Boolean = False)

        _fileLogger = New FileLogger(logPath, logName, TagName, commitRow)
        If urlColdirettiLogger <> "" AndAlso enableElasticSearchNotify Then
            '08/07/24 Puntiamo ad un nuovo endpoint gias.IdEsportazione. Su configurazione siti rimane quello originale gias.generico, sostituisco il generico con il nuovo ID
            urlColdirettiLogger = urlColdirettiLogger.Replace("generico", String.Empty) + CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish).ToString()
            _coldirettiLogger = New ElasticSearchLogger(environment, urlColdirettiLogger, TagName, inviaSoloErrori)
        End If

    End Sub

    Public Sub AppendLog(ByVal cuaaObj As ObjNotifica(Of CUAAObj),
                         ByVal component As String,
                         ByVal Text As String,
                         Optional ByVal LogType As LogType = LogType.Informazione,
                         Optional ByVal ex As Exception = Nothing,
                         Optional ByVal bypassElastiSearch As Boolean = False)

        _fileLogger.AppendLog(Text, LogType, ex)
        If _coldirettiLogger IsNot Nothing AndAlso bypassElastiSearch = False Then
            _coldirettiLogger.AppendLog(cuaaObj, component, Text, LogType, ex)
        End If

    End Sub

    Public Sub Flush(ByVal key As String)
        _fileLogger.Flush()

        If _coldirettiLogger IsNot Nothing Then
            _coldirettiLogger.Flush(key)
        End If
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: eliminare lo stato gestito (oggetti gestiti)
                _fileLogger.Dispose()
                If _coldirettiLogger IsNot Nothing Then
                    _coldirettiLogger.Dispose()
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
