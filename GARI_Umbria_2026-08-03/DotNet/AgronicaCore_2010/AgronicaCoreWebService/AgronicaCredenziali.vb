Public Class AgronicaCredenziali

    Private _Username As String
    Private _Password As String
    Private _UsernameSessione As String
    Private _UserAgent As String

    Public Property Username As String
        Get
            Return _Username
        End Get
        Set(value As String)
            _Username = value
        End Set
    End Property

    Public Property Password As String
        Get
            Return _Password
        End Get
        Set(value As String)
            _Password = value
        End Set
    End Property

    Public Property UsernameSessione As String
        Get
            Return _UsernameSessione
        End Get
        Set(value As String)
            _UsernameSessione = value
        End Set
    End Property

    Public Property UserAgent As String
        Get
            Return _UserAgent
        End Get
        Set(value As String)
            _UserAgent = value
        End Set
    End Property
End Class
