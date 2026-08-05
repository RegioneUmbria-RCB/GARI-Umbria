Imports System.Collections.Concurrent
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi
Public Class ElasticSearchLogger
    Implements IDisposable

    Private disposedValue As Boolean
    Private _cache As ConcurrentQueue(Of String)
    Private _tagname As String
    Private _onlyErrors As Boolean
    Private _environment As String

    Private _esColdirettiLogManager As ElasticSearchColdiretti

    Public Sub New(ByVal environment As String,
                   ByVal baseUrlElasticSearch As String,
                   ByVal TagName As String,
                   ByVal onlyErrors As Boolean)
        _cache = New ConcurrentQueue(Of String)
        _tagname = TagName
        _onlyErrors = onlyErrors
        _environment = environment
        _esColdirettiLogManager = New ElasticSearchColdiretti(baseUrlElasticSearch, Nothing)
    End Sub

    Public Sub New(ByVal environment As String,
                   ByVal baseUrlElasticSearch As String)
        _cache = New ConcurrentQueue(Of String)
        _environment = environment
        _esColdirettiLogManager = New ElasticSearchColdiretti(baseUrlElasticSearch, Nothing)
    End Sub


    Public Sub AppendLog(ByVal cuaaObj As ObjNotifica(Of CUAAObj),
                         ByVal component As String,
                         ByVal Text As String,
                         Optional ByVal LogType As LogType = LogType.Informazione,
                         Optional ByVal ex As Exception = Nothing)

        If (_onlyErrors AndAlso {LogType.Errore, LogType.Critical}.Contains(LogType)) OrElse Not _onlyErrors Then
            WriteLog(cuaaObj, component, Text, LogType, ex)
        End If

    End Sub

    Private Sub WriteLog(ByVal cuaaObj As ObjNotifica(Of CUAAObj),
                         ByVal component As String,
                         ByVal Text As String,
                         Optional ByVal LogType As LogType = LogType.Informazione,
                         Optional ByVal ex As Exception = Nothing)

        Dim logEntry As New ElasticSearchLogEntry

        logEntry.ambiente = _environment
        logEntry.cuaa = cuaaObj.payload.CUAA
        logEntry.componente = component
        logEntry.opType = cuaaObj.payload.Operazione
        logEntry.timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        logEntry.severity = LogType.ToString()

        'lavez - 10/04/2024 - fix uniform log model per elastic search
        Dim objMessage = New ElasticSearchLogEntryMessage With {.response = Text, .payload = Nothing}
        logEntry.message = objMessage 'JsonConvert.SerializeObject(objMessage).ToString().Replace("\", String.Empty)
        'lavez - 10/04/2024 - fix uniform log model per elastic search

        logEntry.task = getTaskDecode(Text)

        ''Replace per avere un json valido all'interno di message
        'Dim strLog As String = JsonConvert.SerializeObject(logEntry).
        '    Replace("\", "").
        '    Replace("""{", "{").Replace("}""", "}").
        '    Replace("""[", "[").Replace("]""", "]")

        _cache.Enqueue(JsonConvert.SerializeObject(logEntry))

    End Sub

    Public Function WriteLog(ByVal cuaa As String,
                             ByVal operazione As String,
                             ByVal componente As String,
                             ByVal tipo_esportazione As enum_Esportazioni_Sistema_Cod,
                             ByVal esito As String,
                             ByVal message As String,
                             ByVal payload As Object,
                             ByVal timestampLog As DateTime,
                             ByVal timestamp As DateTime
                             ) As Boolean

        Dim logEntry As New ElasticSearchLogEntry

        logEntry.ambiente = _environment
        logEntry.cuaa = cuaa
        logEntry.componente = componente
        logEntry.opType = operazione
        logEntry.timestamp = timestampLog.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

        Select Case esito
            Case Util_Costanti.ESITO_OK
                logEntry.severity = LogType.Informazione.ToString()
            Case Util_Costanti.ESITO_KO
                logEntry.severity = LogType.Errore.ToString()
            Case Util_Costanti.ESITO_BLK
                logEntry.severity = LogType.Warning.ToString()
        End Select

        Dim response As String = "Elaborazione " & esito & If(message <> "", ": ", "") & message
        Dim objMessage = New ElasticSearchLogEntryMessage With {.response = response, .payload = payload}
        logEntry.message = objMessage 'JsonConvert.SerializeObject(objMessage).ToString()

        'Select Case tipo_esportazione
        '    Case enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag, enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag,
        '         enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori, enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori
        '        'SONO XML
        '        'Rimuovo gli escape di new row e new line 
        '        'Sostituisco le virgolette con gli apici

        '        'logEntry.message.payload = logEntry.message.payload..ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\\\""", "'")
        '    Case Else
        '        'SONO OGGETTI
        '        'Rimuovo gli escape
        '        'logEntry.message = logEntry.message.Replace("\", "")
        'End Select

        logEntry.task = New ElasticSearchLogEntry_Task With {
            .action = operazione,
            .category = New List(Of String) From {componente},
            .cuaaEnd = cuaa,
            .cuaaInit = cuaa,
            .dateRef = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            .id = tipo_esportazione
        }

        ''Replace per avere un json valido all'interno di message
        'Dim strLog As String = JsonConvert.SerializeObject(logEntry).
        '    Replace("\", "").
        '    Replace("""{", "{").Replace("}""", "}").
        '    Replace("""[", "[").Replace("]""", "]")

        Return _esColdirettiLogManager.ChiamaWSLogger(JsonConvert.SerializeObject(logEntry), CInt(tipo_esportazione).ToString())

    End Function

    Public Function WriteLogWithoutTask(ByVal cuaa As String,
                                     ByVal operazione As String,
                                     ByVal componente As String,
                                     ByVal esito As String,
                                     ByVal message As String,
                                     ByVal payload As Object,
                                     ByVal timestampLog As DateTime,
                                     ByVal timestamp As DateTime) As Boolean

        Dim logEntry As New ElasticSearchLogEntry

        logEntry.ambiente = _environment
        logEntry.cuaa = cuaa
        logEntry.componente = componente
        logEntry.opType = operazione
        logEntry.timestamp = timestampLog.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

        Select Case esito
            Case Util_Costanti.ESITO_OK
                logEntry.severity = LogType.Informazione.ToString()
            Case Util_Costanti.ESITO_KO
                logEntry.severity = LogType.Errore.ToString()
            Case Util_Costanti.ESITO_BLK
                logEntry.severity = LogType.Warning.ToString()
        End Select

        Dim response As String = "Elaborazione " & esito & If(message <> "", ": ", "") & message
        Dim objMessage = New ElasticSearchLogEntryMessage With {.response = response, .payload = payload}
        logEntry.message = objMessage

        logEntry.task = Nothing

        Return _esColdirettiLogManager.ChiamaWSLogger(JsonConvert.SerializeObject(logEntry))

    End Function

    Public Sub Flush(ByVal key As String)
        If _cache.Count > 0 Then
            Dim s As String
            While _cache.TryDequeue(s)
                _esColdirettiLogManager.ChiamaWSLogger(s)
            End While
        End If
    End Sub

    Private Function getTaskDecode(ByVal message) As ElasticSearchLogEntry_Task
        Dim ret As New ElasticSearchLogEntry_Task
        ret.id = If(_tagname <> "", _tagname, "")

        If message.IndexOf("[") >= 0 AndAlso message.IndexOf("]") > 0 Then
            Dim logEntryPrefix = message.Substring(message.IndexOf("[") + 1, message.IndexOf("]") - 1 - message.IndexOf("["))
            Dim arStr As String() = logEntryPrefix.Split(" - ")
            ret.cuaaInit = arStr(0).Replace("[", "")
            ret.action = arStr(2)
            ret.category = New List(Of String)

            Dim subArStr = arStr(4).Replace("]", "").Substring(0, arStr(4).Length - 26).Split("-")
            ret.cuaaEnd = subArStr(0) + "-" + subArStr(1)

            If subArStr.Contains("anagrafica") Then
                ret.category.Add("anagrafica")
            End If
            If subArStr.Contains("terreni") Then
                ret.category.Add("terreni")
            End If
            If subArStr.Contains("pcg-terreni") Then
                ret.category.Add("pcg-terreni")
            End If
            If subArStr.Contains("pcg") Then
                ret.category.Add("pcg")
            End If
            If subArStr.Contains("equipaggiamenti") Then
                ret.category.Add("equipaggiamenti")
            End If
            If subArStr.Contains("lavoratori") Then
                ret.category.Add("lavoratori")
            End If

            Dim strPosDot = arStr(4).IndexOf(".", 1)
            Dim strLenght As Integer = -1
            If strPosDot >= 0 Then
                strLenght = arStr(4).Length - (strPosDot - 20) - 1   'dal punto dei decimali sul time stamp, la data-ora è lunga 20 chars "10T8."
            Else
                strLenght = arStr(4).Length - 20
            End If
            ret.dateRef = arStr(4).Substring(arStr(4).Length - strLenght, strLenght) '+ "Z"
            'Dim res = DateTime.ParseExact(ret.dateRef, "yyyy-MM-ddThh:mm:ssZ", System.Globalization.DateTimeFormatInfo.InvariantInfo)


        End If
        Return ret
    End Function

    Public Enum LogType
        Informazione = 0
        Warning = 1
        Errore = 2
        Critical = 3
    End Enum

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: eliminare lo stato gestito (oggetti gestiti)
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