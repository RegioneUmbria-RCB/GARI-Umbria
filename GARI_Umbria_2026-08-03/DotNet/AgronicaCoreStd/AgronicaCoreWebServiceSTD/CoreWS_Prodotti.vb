Public Class CoreWS_Prodotti
    Inherits APICallsBasic

    Public piva As String
    Public Elem_Cod As Integer
    Public Data_Movimento_Str As String
    Public metaschema As String
    Public soloInGiacenza As String
    Public Flag_QtaNoZero As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Data_Movimento_Str As String, ByVal Elem_Cod As Integer, ByVal metaschema As String, ByVal soloInGiacenza As Boolean, ByVal Flag_QtaNoZero As Boolean)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva
        Me.Data_Movimento_Str = Data_Movimento_Str
        Me.Elem_Cod = Elem_Cod
        Me.metaschema = metaschema
        Me.soloInGiacenza = CStr(soloInGiacenza)
        Me.Flag_QtaNoZero = CStr(Flag_QtaNoZero)

    End Sub

End Class
