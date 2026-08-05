Public Class TeleregistriExceptionFormattazioneDati
    Inherits Exception
    Public Sub New(manager As String, method As String)
        MyBase.New(manager + ": " + method)
    End Sub
End Class

Public Class TeleregistriExceptionResponseParsing
    Inherits Exception
    Public Sub New(manager As String, method As String)
        MyBase.New(manager + ": " + method)
    End Sub
End Class

Public Class TeleregistriExceptionIncoherentDBSincro
    Inherits Exception
    Public Sub New(manager As String, method As String)
        MyBase.New(manager + ": " + method)
    End Sub
End Class