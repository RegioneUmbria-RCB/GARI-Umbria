Imports Newtonsoft.Json

Public Class ElastiSearchLogGiasGenerico
    Public Property key As String
    Public Property textLog As String
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
