Public Class recode_Analisi_Tipologia

    Private _FROM_AnalisiTipologia_cod As Integer

    Private _TO_AnalisiTipologia_cod As Integer
    Public Property TO_AnalisiTipologia_cod() As Integer
        Get
            Return _TO_AnalisiTipologia_cod
        End Get
        Set(value As Integer)
            _TO_AnalisiTipologia_cod = Value
        End Set
    End Property
    Public Property FROM_AnalisiTipologia_cod() As Integer
        Get
            Return _FROM_AnalisiTipologia_cod
        End Get
        Set(value As Integer)
            _FROM_AnalisiTipologia_cod = Value
        End Set
    End Property


End Class
