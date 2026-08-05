Public Class AgronicaToken

    Private _Token As String
    Private _Errore As String


    Public Property Token
        Get
            Return _Token
        End Get
        Set(value)
            _Token = value
        End Set
    End Property


    Public Property Errore
        Get
            Return _Errore
        End Get
        Set(value)
            _Errore = value
        End Set
    End Property

End Class