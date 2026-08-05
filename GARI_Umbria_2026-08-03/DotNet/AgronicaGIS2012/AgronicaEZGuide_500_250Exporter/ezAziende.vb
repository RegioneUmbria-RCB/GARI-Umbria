Public Class ezAziende

    Private _NomeAzienda As String

    Private _listaCentri As New List(Of ezCentri)
    Public Property NomeAzienda() As String
        Get
            Return _NomeAzienda
        End Get
        Set(value As String)
            _NomeAzienda = Value
        End Set
    End Property
    Public ReadOnly Property ListaCentri() As List(Of ezCentri)
        Get
            Return _listaCentri
        End Get

    End Property

    Public Sub AggiungiCentro(ByVal c As ezCentri)
        _listaCentri.Add(c)
    End Sub
End Class
