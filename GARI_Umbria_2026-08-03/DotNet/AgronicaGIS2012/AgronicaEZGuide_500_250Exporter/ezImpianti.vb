Public Class ezImpianti


    Private _NomeImpianto As String


    Private _ListaOperazioni As New List(Of ezEventiOperazioni)
    Public Property NomeImpianto() As String
        Get
            Return _NomeImpianto
        End Get
        Set(value As String)
            _NomeImpianto = Value
        End Set
    End Property


    Public ReadOnly Property ListaOperazioni() As List(Of ezEventiOperazioni)
        Get
            Return _ListaOperazioni
        End Get
    End Property

    Public Sub AggiungiEventoOperazione(ByVal e As ezEventiOperazioni)
        _ListaOperazioni.Add(e)
    End Sub

End Class
