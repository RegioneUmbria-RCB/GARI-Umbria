Public Class ezCentri

    Private _NomeCentro As String

    Private _ListaImpianti As New List(Of ezImpianti)
    Public Property NomeCentro() As String
        Get
            Return _NomeCentro
        End Get
        Set(value As String)
            _NomeCentro = Value
        End Set
    End Property
    Public ReadOnly Property ListaImpianti() As List(Of ezImpianti)
        Get
            Return _ListaImpianti
        End Get
    End Property

    Public Sub AggiungiImpianto(ByVal i As ezImpianti)
        _ListaImpianti.Add(i)
    End Sub

End Class
