Public Class SyncAcknowledge
    Public Property processed As Boolean
    Public Property errors As List(Of SyncErrors)
    Public Property entities As List(Of SyncEntity)

    Public Sub New()
        errors = New List(Of SyncErrors)
        entities = New List(Of SyncEntity)
    End Sub
End Class

Public Class SyncErrors
    Public Property code As String
    Public Property msg As String
    Public Property key As String
End Class

Public Class SyncAcknowledgeResponse
    Public Property ack As Boolean
End Class

Public Class SyncEntity
    Public Property entity As String
    Public Property ref_timestamp As DateTime
End Class

Public Class SyncAcknowledge_Agronica
    Inherits SyncAcknowledge

    Public Property app_count As Integer
End Class