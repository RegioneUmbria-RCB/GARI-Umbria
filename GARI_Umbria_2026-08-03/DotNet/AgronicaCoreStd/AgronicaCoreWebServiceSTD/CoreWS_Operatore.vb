Public Class CoreWS_Operatore
    Inherits APICallsBasic

    Public usernameUtente As String
    Public escludiSuperUser As Boolean

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal usernameUtente As String, ByVal escludiSuperUser As Boolean)
        MyBase.New(objP_super_server, objP_server, objP_utenti)
        Me.usernameUtente = usernameUtente
        Me.escludiSuperUser = escludiSuperUser
    End Sub

End Class
