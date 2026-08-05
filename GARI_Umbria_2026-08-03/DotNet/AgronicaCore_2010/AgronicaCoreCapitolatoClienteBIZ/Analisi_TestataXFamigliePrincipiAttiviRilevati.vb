Public Class Analisi_TestataXFamigliePrincipiAttiviRilevati
    Private _Fam_Cod As String
    
    Public Property Fam_Cod() As String
        Get
            Return _Fam_Cod
        End Get
        Set(value As String)
            _Fam_Cod = Value
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
