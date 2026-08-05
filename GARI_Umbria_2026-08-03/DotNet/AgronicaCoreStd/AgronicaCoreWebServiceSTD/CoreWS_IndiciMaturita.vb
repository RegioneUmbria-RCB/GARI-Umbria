Public Class CoreWS_IndiciMaturita
    Inherits APICallsBasic

    Public Property vegCod As Integer
    Public Property personalizzate As Boolean
    Public Property tipoTestata As Integer

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
                   vegCod As Integer,
                   personalizzate As Boolean,
                   tipoTestata As Integer)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.vegCod = vegCod
        Me.personalizzate = personalizzate
        Me.tipoTestata = tipoTestata

    End Sub

End Class
