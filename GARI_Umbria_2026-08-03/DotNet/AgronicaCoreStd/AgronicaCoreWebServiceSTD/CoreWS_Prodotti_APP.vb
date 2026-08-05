Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class CoreWS_Prodotti_APP
    Inherits APICallsBasic

    Public piva As String
    Public Elem_Cod As Integer
    Public Data_Movimento_Str As String
    Public FiltroDescrizioneProdotto As String
    Public Veg_Cod As Integer
    Public Magazzino As Integer

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Data_Movimento_Str As String, ByVal Elem_Cod As Integer, ByVal FiltroDescrizioneProdotto As String, ByVal Veg_Cod As Integer, ByVal Magazzino As Integer)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva
        Me.Elem_Cod = Elem_Cod
        Me.Data_Movimento_Str = Data_Movimento_Str
        Me.FiltroDescrizioneProdotto = FiltroDescrizioneProdotto
        Me.Veg_Cod = Veg_Cod
        Me.Magazzino = Magazzino

    End Sub

End Class
