Public Class ScomponiResponse

    Private _Mostra As Boolean = False 
    Private _Ricarica As Boolean =False

    Public Property Mostra As Boolean
        Get
            Return _Mostra
        End Get
        Set(value As Boolean)
            _Mostra = value
        End Set
    End Property

    Public Property Ricarica As Boolean
        Get
            Return _Ricarica
        End Get
        Set(value As Boolean)
            _Ricarica = value
        End Set
    End Property
End Class
