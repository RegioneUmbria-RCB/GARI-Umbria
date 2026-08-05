Public Class cls_Magazzino


    Dim _obj_Magazzino_Giacenze As cls_Magazzino_Giacenze
    Dim _obj_Magazzino_MovCarico As cls_Magazzino_MovCarico
    Dim _HT_Mapping_ChiaviMagazzino As Hashtable

End Class

'########################################################################################
'########################################################################################
'########################################################################################


Public Class cls_Magazzino_Giacenze

    Dim _Data_Lettura_Giacenze As Date
    Dim _Data_Scrittura_Giacenze As Date

    Public Property Data_Lettura_Giacenze() As Date
        Get
            Return _Data_Lettura_Giacenze
        End Get
        Set(ByVal value As Date)
            _Data_Lettura_Giacenze = value
        End Set
    End Property

    Public Property Data_Scrittura_Giacenze() As Date
        Get
            Return _Data_Scrittura_Giacenze
        End Get
        Set(ByVal value As Date)
            _Data_Scrittura_Giacenze = value
        End Set
    End Property


    Public Sub New(ByVal x_Data_Lettura_Giacenze As Date,
                    ByVal x_Data_Scrittura_Giacenze As Date)

        _Data_Lettura_Giacenze = x_Data_Lettura_Giacenze
        _Data_Scrittura_Giacenze = x_Data_Scrittura_Giacenze

    End Sub

End Class

'########################################################################################
'########################################################################################
'########################################################################################

Public Class cls_Magazzino_MovCarico

    Dim _DataInizio_MovCarico As Date
    Dim _DataFine_MovCarico As Date
    Dim _Flag_FattureRicevute As Boolean
    Dim _Flag_DDTRicevuti As Boolean
    Dim _Flag_Carichi As Boolean
    Dim _Flag_Trasferimenti As Boolean

    Public Property DataInizio_MovCarico() As Date
        Get
            Return _DataInizio_MovCarico
        End Get
        Set(ByVal value As Date)
            _DataInizio_MovCarico = value
        End Set
    End Property

    Public Property DataFine_MovCarico() As Date
        Get
            Return _DataFine_MovCarico
        End Get
        Set(ByVal value As Date)
            _DataFine_MovCarico = value
        End Set
    End Property

    Public Property Flag_FattureRicevute() As Boolean
        Get
            Return _Flag_FattureRicevute
        End Get
        Set(ByVal value As Boolean)
            _Flag_FattureRicevute = value
        End Set
    End Property

    Public Property Flag_DDTRicevuti() As Boolean
        Get
            Return _Flag_DDTRicevuti
        End Get
        Set(ByVal value As Boolean)
            _Flag_DDTRicevuti = value
        End Set
    End Property

    Public Property Flag_Carichi() As Boolean
        Get
            Return _Flag_Carichi
        End Get
        Set(ByVal value As Boolean)
            _Flag_Carichi = value
        End Set
    End Property

    Public Property Flag_Trasferimenti() As Boolean
        Get
            Return _Flag_Trasferimenti
        End Get
        Set(ByVal value As Boolean)
            _Flag_Trasferimenti = value
        End Set
    End Property

    '########################################################################################
    Public Sub New(ByVal x_DataInizio_MovCarico As Date,
                    ByVal x_DataFine_MovCarico As Date,
                    ByVal x_Flag_FattureRicevute As Boolean,
                    ByVal x_Flag_DDTRicevuti As Boolean,
                    ByVal x_Flag_Carichi As Boolean,
                    ByVal x_Flag_Trasferimenti As Boolean)

        _DataInizio_MovCarico = x_DataInizio_MovCarico
        _DataFine_MovCarico = x_DataFine_MovCarico
        _Flag_FattureRicevute = x_Flag_FattureRicevute
        _Flag_DDTRicevuti = x_Flag_DDTRicevuti
        _Flag_Carichi = x_Flag_Carichi
        _Flag_Trasferimenti = x_Flag_Trasferimenti

    End Sub


End Class

