Public Class ResultWrapper(Of T)
    Private _data As T
    Private _errors As New List(Of String)

    Public Sub New()
    End Sub

    Public Sub New(data As T)
        _data = data
    End Sub

    Public Property Data As T
        Get
            Return _data
        End Get
        Set(value As T)
            _data = value
        End Set
    End Property

    Public Property Errors As List(Of String)
        Get
            Return _errors
        End Get
        Set(value As List(Of String))
            _errors = value
        End Set
    End Property

    Public Sub AddError(errore As String)
        _errors.Add(errore)
    End Sub

    Public Function HasErrors() As Boolean
        Return _errors.Count > 0
    End Function

    Public Sub ClearErrors()
        _errors.Clear()
    End Sub
End Class