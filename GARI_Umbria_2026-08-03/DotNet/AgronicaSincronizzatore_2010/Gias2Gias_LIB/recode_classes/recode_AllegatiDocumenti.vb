Public Class recode_AllegatiDocumenti

    Private _FROM_AllegatiDocumentiCod As Integer
    Private _TO_AllegatiDocumentiCod As Integer
    Public Property TO_AllegatiDocumentiCod() As Integer
        Get
            Return _TO_AllegatiDocumentiCod
        End Get
        Set(value As Integer)
            _TO_AllegatiDocumentiCod = Value
        End Set
    End Property
    Public Property FROM_AllegatiDocumentiCod() As Integer
        Get
            Return _FROM_AllegatiDocumentiCod
        End Get
        Set(value As Integer)
            _FROM_AllegatiDocumentiCod = Value
        End Set
    End Property

End Class
