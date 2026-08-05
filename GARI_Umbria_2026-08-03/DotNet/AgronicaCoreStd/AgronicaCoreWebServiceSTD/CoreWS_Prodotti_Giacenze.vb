Public Class CoreWS_Prodotti_Giacenze
    Inherits APICallsBasic

    Public piva As String
    Public tipo_aggregazione As Integer
    Public soloCampiApp As Boolean
    Public sa_cod As Integer
    Public tipo_fabbricato_cod As Integer
    Public fabbricato_cod As Integer
    Public elem_cod As Integer
    Public pro_cod As Integer
    Public mat_cod As Integer
    Public lotto As String
    Public Data_Movimento_Str As String
    Public isFreshAndFood As Boolean
    Public flag_QtaNoZero As Boolean

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Data_Movimento_Str As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva
        Me.tipo_aggregazione = 0
        Me.soloCampiApp = True
        Me.Data_Movimento_Str = Data_Movimento_Str
        Me.isFreshAndFood = False
        Me.flag_QtaNoZero = False

    End Sub
End Class
