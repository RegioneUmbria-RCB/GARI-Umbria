
Public Class Particella_Anagrafica

    Private _part_cod As Integer
    Private _prov As String
    Private _com As String
    Private _sezione As String
    Private _foglio As Integer
    Private _numero As Integer
    Private _subalterno As String
    Private _supimp As Decimal
    Private _istat_prov As String
    Private _istat_com As String


    Sub New()

        _part_cod = 0
        _prov = ""
        _com = ""
        _sezione = ""
        _foglio = 0
        _numero = 0
        _subalterno = ""
        _supimp = 0
        _istat_prov = "000"
        _istat_com = "000"
    End Sub

    Public Property Part_Cod() As Integer
        Get
            Return _part_cod
        End Get
        Set(ByVal value As Integer)
            _part_cod = value
        End Set
    End Property

    Public Property CodiceIstat_Provincia() As String
        Get
            Return _istat_prov
        End Get
        Set(ByVal value As String)
            _istat_prov = value
        End Set
    End Property

    Public Property CodiceIstat_Comune() As String
        Get
            Return _istat_com
        End Get
        Set(ByVal value As String)
            _istat_com = value
        End Set
    End Property

    Public Property Provincia() As String
        Get
            Return _prov
        End Get
        Set(ByVal value As String)
            _prov = value
        End Set
    End Property

    Public Property Comune() As String
        Get
            Return _com
        End Get
        Set(ByVal value As String)
            _com = value
        End Set
    End Property

    Public Property Sezione() As String
        Get
            Return _sezione
        End Get
        Set(ByVal value As String)
            _sezione = value
        End Set
    End Property

    Public Property Foglio() As Integer
        Get
            Return _foglio
        End Get
        Set(ByVal value As Integer)
            _foglio = value
        End Set
    End Property

    Public Property Numero() As Integer
        Get
            Return _numero
        End Get
        Set(ByVal value As Integer)
            _numero = value
        End Set
    End Property

    Public Property Subalterno() As String
        Get
            Return _subalterno
        End Get
        Set(ByVal value As String)
            _subalterno = value
        End Set
    End Property


    Public Property SuperficieImpiegata() As Decimal
        Get
            Return _supimp
        End Get
        Set(ByVal value As Decimal)
            _supimp = value
        End Set
    End Property

    Public Property Macrouso_Cod As Integer

    Public Property Macrouso_Des As String

    Public Property Sup_Macrouso As Decimal

    Public Property Veg_Cod_Agea As Integer

    Public Property Veg_Des_Agea As String

    Public Property Cul_Cod_Agea As Integer

    Public Property Cul_Des_Agea As String

    Public Property Sup_Utilizzo As Decimal


End Class
