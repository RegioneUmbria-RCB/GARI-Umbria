Public Class recode_campo
    Private _From_Piva As String

    Public Property From_Piva() As String
        Get
            Return _From_Piva
        End Get
        Set(value As String)
            _From_Piva = Value
        End Set
    End Property

    Private _From_sa_cod As Integer

    Public Property From_sa_cod() As Integer
        Get
            Return _From_sa_cod
        End Get
        Set(value As Integer)
            _From_sa_cod = Value
        End Set
    End Property

    Private _From_Campo_cod As Integer
    Public Property From_Campo_cod() As Integer
        Get
            Return _From_Campo_cod
        End Get
        Set(value As Integer)
            _From_Campo_cod = value
        End Set
    End Property


    Private _To_Piva As String

    Public Property To_Piva() As String
        Get
            Return _To_Piva
        End Get
        Set(value As String)
            _To_Piva = Value
        End Set
    End Property

    Private _To_sa_cod As Integer

    Public Property To_sa_cod() As Integer
        Get
            Return _To_sa_cod
        End Get
        Set(value As Integer)
            _To_sa_cod = Value
        End Set
    End Property


    Private _To_Campo_Cod As Integer
    Public Property To_Campo_Cod() As Integer
        Get
            Return _To_Campo_Cod
        End Get
        Set(value As Integer)
            _To_Campo_Cod = Value
        End Set
    End Property


End Class
