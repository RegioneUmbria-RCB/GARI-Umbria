Public Class CoreWS_FasiFenologiche
    Inherits APICallsBasic

    Public Property vegCod As Integer
    Public Property fasiOld As Boolean
    Public Property personalizzate As Boolean

    Public Sub New(
        ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
        vegCod As Integer,
        fasiOld As Boolean,
        personalizzate As Boolean)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.vegCod = vegCod
        Me.fasiOld = fasiOld
        Me.personalizzate = personalizzate

    End Sub


End Class
