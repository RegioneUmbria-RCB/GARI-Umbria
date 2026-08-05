Public Class ezEventiOperazioni

    Private _NomeOperazione As String
    Public Property NomeOperazione() As String
        Get
            Return _NomeOperazione
        End Get
        Set(value As String)
            _NomeOperazione = Value
        End Set
    End Property


End Class
