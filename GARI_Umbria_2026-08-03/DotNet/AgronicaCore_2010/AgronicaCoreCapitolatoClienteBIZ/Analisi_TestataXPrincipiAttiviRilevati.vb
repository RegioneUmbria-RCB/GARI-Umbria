Public Class Analisi_TestataXPrincipiAttiviRilevati
    Private _PA_Cod As Integer
    Public Property PA_Cod() As Integer
        Get

            Return _PA_Cod
        End Get
        Set(value As Integer)
            _PA_Cod = Value
        End Set
    End Property
    Private _qtaRilevata As Decimal
    Public Property QtaRilevata() As Decimal
        Get
            Return _qtaRilevata
        End Get
        Set(value As Decimal)
            _qtaRilevata = Value
        End Set
    End Property
    Private _qtaRilevataUdmGias As Integer
    Public Property QtaRilevataUdmGias() As Integer
        Get
            Return _qtaRilevataUdmGias
        End Get
        Set(value As Integer)
            _qtaRilevataUdmGias = Value
        End Set
    End Property

End Class
