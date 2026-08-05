Public Class TokenHandler

    Property BDNtoken As String
    Property BDNRefreshToken As String

    Property VetInfotoken As String
    Property VetInfoRefreshToken As String

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property GetInstance As TokenHandler
        Get
            Static instance As TokenHandler = New TokenHandler
            Return instance

        End Get
    End Property

End Class
