Public Class recode_particelle

    Private _From_PartCod As Integer

    Public Property From_PartCod() As Integer
        Get
            Return _From_PartCod
        End Get
        Set(value As Integer)
            _From_PartCod = Value
        End Set
    End Property


    Private _To_PartCod As Integer

    Public Property To_PartCod() As Integer
        Get
            Return _To_PartCod
        End Get
        Set(value As Integer)
            _To_PartCod = Value
        End Set
    End Property

End Class
