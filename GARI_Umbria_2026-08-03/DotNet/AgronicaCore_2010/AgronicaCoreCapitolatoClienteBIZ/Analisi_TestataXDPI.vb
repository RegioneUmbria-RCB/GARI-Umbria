Public Class Analisi_TestataXDPI

    Private _Flag_Privato_Pubblico As Integer
    Public Property Flag_Privato_Pubblico() As Integer
        Get
            Return _Flag_Privato_Pubblico
        End Get
        Set(value As Integer)
            _Flag_Privato_Pubblico = Value
        End Set
    End Property
    Private _Cod_Regolamento As Integer
    Public Property Cod_Regolamento() As Integer
        Get
            Return _Cod_Regolamento
        End Get
        Set(value As Integer)
            _Cod_Regolamento = Value
        End Set
    End Property

End Class
