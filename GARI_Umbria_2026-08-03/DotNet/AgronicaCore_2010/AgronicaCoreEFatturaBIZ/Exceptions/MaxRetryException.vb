Public Class MaxRetryException : Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub
End Class
