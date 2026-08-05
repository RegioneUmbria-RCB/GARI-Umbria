Public Class CoreWS_Codifica_Prodotti

    Inherits APICallsBasic

    Public Piva As String
    Public Tipo_Codifica As Integer
    Public Elem_Cod As Integer

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal Piva As String, ByVal Tipo_Codifica As Integer, ByVal Elem_Cod As Integer)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.Piva = Piva
        Me.Tipo_Codifica = Tipo_Codifica
        Me.Elem_Cod = Elem_Cod

    End Sub

End Class
