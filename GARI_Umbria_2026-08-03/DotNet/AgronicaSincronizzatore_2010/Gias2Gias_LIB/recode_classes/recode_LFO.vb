Public Class recode_LFO
    Private _FROM_LFO As Integer
    Private _To_LFO As Integer
    Public Property To_LFO() As Integer
        Get
            Return _To_LFO
        End Get
        Set(value As Integer)
            _To_LFO = Value
        End Set
    End Property
    Public Property FROM_LFO() As Integer
        Get
            Return _FROM_LFO
        End Get
        Set(value As Integer)
            _FROM_LFO = Value
        End Set
    End Property

End Class
