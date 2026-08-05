Public Class WorkFlowEsito
    Private _WAnagraficaStati_Des As String
    Private _Servizio_Cod As Integer
    Private _WorkFlow_Cod As Integer
    Private _WorkFlow_des As String
    Private _conteggio As Integer
    Public Property Conteggio() As Integer
        Get
            Return _conteggio
        End Get
        Set(value As Integer)
            _conteggio = Value
        End Set
    End Property
    Public Property WorkFlow_des() As String
        Get
            Return _WorkFlow_des
        End Get
        Set(value As String)
            _WorkFlow_des = Value
        End Set
    End Property
    Public Property WorkFlow_Cod() As Integer
        Get
            Return _WorkFlow_Cod
        End Get
        Set(value As Integer)
            _WorkFlow_Cod = Value
        End Set
    End Property
    Public Property Servizio_Cod() As Integer
        Get
            Return _Servizio_Cod
        End Get
        Set(value As Integer)
            _Servizio_Cod = Value
        End Set
    End Property
    Public Property WAnagraficaStati_Des() As String
        Get
            Return _WAnagraficaStati_Des
        End Get
        Set(value As String)
            _WAnagraficaStati_Des = Value
        End Set
    End Property


End Class
