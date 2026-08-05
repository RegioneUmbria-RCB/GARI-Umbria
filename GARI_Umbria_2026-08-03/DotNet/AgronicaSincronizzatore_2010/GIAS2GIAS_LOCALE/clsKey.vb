
'#################################################################
Public Class clsKeyImpresa

    Private _Piva As String

#Region "Costruttori"

    Public Sub New()
        _Piva = ""
    End Sub

    Public Sub New(ByVal Piva As String)
        _Piva = Piva
    End Sub

#End Region

#Region "Proprieta"



    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

#End Region

End Class

'#################################################################
Public Class clsKeyCentro

    Private _Piva As String
    Private _Sa_Cod As Integer

#Region "Costruttori"

    Public Sub New()
        _Piva = ""
        _Sa_Cod = "0"
    End Sub

    Public Sub New(ByVal Piva As String, ByVal sa_cod As Integer)
        _Piva = Piva
        _Sa_Cod = sa_cod
    End Sub

#End Region

#Region "Proprieta"

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property

#End Region

End Class

'#################################################################
Public Class clsKeyParticella

    Private _Prov As String
    Private _Com As String
    Private _Sezione As String
    Private _Foglio As Integer
    Private _Numero As Integer
    Private _Subalterno As String

#Region "Proprieta"

    Public Property Prov() As String
        Get
            Return _Prov
        End Get
        Set(ByVal value As String)
            _Prov = value
        End Set
    End Property

    Public Property Com() As String
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property

    Public Property Sezione() As String
        Get
            Return _Sezione
        End Get
        Set(ByVal value As String)
            _Sezione = value
        End Set
    End Property

    Public Property Foglio() As Integer
        Get
            Return _Foglio
        End Get
        Set(ByVal value As Integer)
            _Foglio = value
        End Set
    End Property

    Public Property Numero() As Integer
        Get
            Return _Numero
        End Get
        Set(ByVal value As Integer)
            _Numero = value
        End Set
    End Property

    Public Property Subalterno() As String
        Get
            Return _Subalterno
        End Get
        Set(ByVal value As String)
            _Subalterno = value
        End Set
    End Property

#End Region

End Class

'#################################################################
Public Class clsKeyIndirizzo

    Private _Cod_Indirizzo

#Region "Costruttori"

    Public Sub New()
        _Cod_Indirizzo = 0
    End Sub

#End Region

#Region "Proprieta"

    Public Property Cod_Indirizzo() As Integer
        Get
            Return _Cod_Indirizzo
        End Get
        Set(ByVal value As Integer)
            _Cod_Indirizzo = value
        End Set
    End Property

#End Region

End Class

'#################################################################
Public Class clsKeyRubrica

    Private _Cod_Rubrica

#Region "Costruttori"

    Public Sub New()
        _Cod_Rubrica = 0
    End Sub

#End Region

#Region "Proprieta"

    Public Property Cod_Rubrica() As Integer
        Get
            Return _Cod_Rubrica
        End Get
        Set(ByVal value As Integer)
            _Cod_Rubrica = value
        End Set
    End Property

#End Region

End Class

