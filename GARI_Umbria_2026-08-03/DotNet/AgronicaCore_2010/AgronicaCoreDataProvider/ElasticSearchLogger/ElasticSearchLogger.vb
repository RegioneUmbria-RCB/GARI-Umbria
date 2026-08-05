Imports Newtonsoft.Json

Public Class ElasticSearchLogger
    Private disposedValue As Boolean
    Private _environment As String
    Private _LogManager As Controller
    Public Sub New(ByVal environment As String,
                   ByVal baseUrlElasticSearch As String)
        _environment = environment
        _LogManager = New Controller(baseUrlElasticSearch, Nothing)
    End Sub
    Public Function WriteLog(ByVal cuaa As String,
                             ByVal componente As String,
                             ByVal message As String,
                             ByVal timestamp As DateTime,
                                Optional ByVal LogType As LogType = LogType.Errore
                             ) As Boolean
        Dim logEntry As New ElasticSearchLogEntry
        logEntry.ambiente = _environment
        logEntry.cuaa = cuaa
        logEntry.componente = componente
        logEntry.opType = "LOG-APPLICATIVO"
        logEntry.timestamp = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        logEntry.severity = LogType.ToString()

        Dim objMessage = New ElasticSearchLogEntryMessage With {.response = message, .payload = Nothing}
        logEntry.message = objMessage 'JsonConvert.SerializeObject(objMessage).Replace("\n", " ").Replace("\r", " ").Replace("\", String.Empty)

        logEntry.task = New ElasticSearchLogEntry_Task With {
            .action = "LOG-APPLICATIVO",
            .category = New List(Of String) From {componente},
            .cuaaEnd = cuaa,
            .cuaaInit = cuaa,
            .dateRef = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            .id = ""
        }


        ''Replace per avere un json valido all'interno di message
        'Dim strLog As String = JsonConvert.SerializeObject(logEntry).
        '    Replace("\", "").
        '    Replace("""{", "{").Replace("}""", "}").
        '    Replace("""[", "[").Replace("]""", "]")

        Return _LogManager.ChiamaWSLogger(JsonConvert.SerializeObject(logEntry))

    End Function
    Public Enum LogType
        Informazione = 0
        Warning = 1
        Errore = 2
        Critical = 3
    End Enum
End Class
Public Class ElasticSearchLogEntry
    Public Property ambiente As String
    Public Property cuaa As String
    Public Property componente As String
    Public Property opType As String
    Public Property timestamp As String
    Public Property severity As String
    Public Property message As ElasticSearchLogEntryMessage
    Public Property task As ElasticSearchLogEntry_Task
End Class

Public Class ElasticSearchLogEntryMessage
    Public Property response As String
    Public Property payload As Object
End Class

Public Class ElasticSearchLogEntry_Task
    Public Property id As String
    Public Property cuaaInit As String
    Public Property action As String
    Public Property cuaaEnd As String
    Public Property category As List(Of String)
    <JsonProperty("date")>
    Public Property dateRef As String
End Class
Public Class ParametriElasticSearch
    Public Property Ambiente As String
End Class