Public Class recode_SpecieVegetaliDefault

    Private _From_PivaSuperUser As String
    Public Property From_PivaSuperUser() As String
        Get
            Return _From_PivaSuperUser
        End Get
        Set(value As String)
            _From_PivaSuperUser = Value
        End Set
    End Property

    Private _To_PivaSuperUSer As String
    Public Property To_PivaSuperUSer() As String
        Get
            Return _To_PivaSuperUSer
        End Get
        Set(value As String)
            _To_PivaSuperUSer = value
        End Set
    End Property

    Private _From_Id As Integer
    Public Property From_Id() As Integer
        Get
            Return _From_Id
        End Get
        Set(value As Integer)
            _From_Id = value
        End Set
    End Property
    Private _To_Id As Integer
    Public Property To_Id() As Integer
        Get
            Return _To_Id
        End Get
        Set(value As Integer)
            _To_Id = value
        End Set
    End Property


End Class
