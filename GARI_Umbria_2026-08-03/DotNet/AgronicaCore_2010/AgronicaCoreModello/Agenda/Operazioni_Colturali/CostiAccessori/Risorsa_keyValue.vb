Public Class Risorsa_keyValue

    Private _Risorsa_Cod As String
    Private _Risorsa_Des As String
    Private _Costo_Unitario As String
    Public Property Udm_Cod As String
    Public Property Udm_Des As String
    Public Property Cod_Rapporto As String
    Public Property Risorsa_Cod As String
        Get
            Return _Risorsa_Cod
        End Get
        Set(value As String)
            _Risorsa_Cod = value
        End Set
    End Property

    Public Property Risorsa_Des As String
        Get
            Return _Risorsa_Des
        End Get
        Set(value As String)
            _Risorsa_Des = value
        End Set
    End Property

    Public Property Costo_Unitario As String
        Get
            Return _Costo_Unitario
        End Get
        Set(value As String)
            _Costo_Unitario = value
        End Set
    End Property


End Class
