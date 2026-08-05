Public Class Recode_Analisi
    Private _FromAnalisi_Testata_Cod As Integer
    Private _ToAnalisi_Testata_cod As Integer
    Public Property ToAnalisi_Testata_cod() As Integer
        Get
            Return _ToAnalisi_Testata_cod
        End Get
        Set(value As Integer)
            _ToAnalisi_Testata_cod = Value
        End Set
    End Property
    Public Property FromAnalisi_Testata_Cod() As Integer
        Get
            Return _FromAnalisi_Testata_Cod
        End Get
        Set(value As Integer)
            _FromAnalisi_Testata_Cod = Value
        End Set
    End Property
End Class
