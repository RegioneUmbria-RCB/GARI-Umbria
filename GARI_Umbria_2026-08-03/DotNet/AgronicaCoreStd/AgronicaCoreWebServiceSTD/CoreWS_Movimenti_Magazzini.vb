Public Class CoreWS_Movimenti_Magazzini
    Inherits APICallsBasic

    Public piva As String
    Public sa_cod As Integer
    Public fabbricato_cod As Integer
    Public elem_cod As Integer
    Public cod_articolo As String
    Public pro_cod As Integer
    Public mat_cod As Integer
    Public lotto As String
    Public data_inizio As String
    Public data_fine As String
    Public flag_carichi As Boolean
    Public flag_scarichi As Boolean

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
                   ByVal piva As String, ByVal sacod As Integer, ByVal fabbricato As Integer,
                   ByVal categoria As Integer, ByVal prodotto As Integer,
                   ByVal data_inizio As String, ByVal data_fine As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva
        Me.sa_cod = sacod
        Me.fabbricato_cod = fabbricato
        Me.elem_cod = categoria
        Me.pro_cod = If(prodotto > 0, prodotto, 0)
        Me.mat_cod = If(prodotto < 0, -prodotto, 0)
        Me.data_inizio = data_inizio
        Me.data_fine = data_fine
        Me.flag_carichi = True
        Me.flag_scarichi = True

    End Sub

End Class
