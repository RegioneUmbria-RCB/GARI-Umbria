Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class CoreWS_ProdottiSenzaGiacenza

    Inherits APICallsBasic

    Public piva As String
    Public Elem_Cod As Integer
    Public FiltroDescrizioneProdotto As String
    Public Elenco_Specie As String
    Public Elenco_Codici As String
    Public Stato_Cod As String
    Public Lav_Cod As Integer

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Elem_Cod As Integer, ByVal FiltroDescrizioneProdotto As String, ByVal Elenco_Specie As String, ByVal Elenco_Codici As String, ByVal Stato_Cod As String, ByVal Lav_Cod As Integer)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Dim filtro As New JArray From {New JObject From {New JProperty("value", FiltroDescrizioneProdotto)}}

        Me.piva = piva
        Me.Elem_Cod = Elem_Cod
        Me.FiltroDescrizioneProdotto = JsonConvert.SerializeObject(filtro)
        Me.Elenco_Specie = Elenco_Specie
        Me.Elenco_Codici = Elenco_Codici
        Me.Stato_Cod = Stato_Cod
        Me.Lav_Cod = Lav_Cod

    End Sub

End Class
