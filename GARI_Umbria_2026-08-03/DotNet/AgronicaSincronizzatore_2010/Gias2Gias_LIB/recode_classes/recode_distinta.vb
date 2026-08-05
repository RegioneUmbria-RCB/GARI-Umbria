Public Class recode_distinta
    Private _From_Piva As String

    Public Property From_Piva() As String
        Get
            Return _From_Piva
        End Get
        Set(value As String)
            _From_Piva = Value
        End Set
    End Property


    Private _From_Progetto_cod As Integer
    Public Property From_Progetto_cod() As Integer
        Get
            Return _From_Progetto_cod
        End Get
        Set(value As Integer)
            _From_Progetto_cod = value
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

    Private _To_Progetto_cod As Integer
    Public Property To_Progetto_cod() As Integer
        Get
            Return _To_Progetto_cod
        End Get
        Set(value As Integer)
            _To_Progetto_cod = Value
        End Set
    End Property


End Class
