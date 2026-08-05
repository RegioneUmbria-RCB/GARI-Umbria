Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello.Nogmo

Public Class wsUpload
    Inherits wsNogmo


    Public Sub New(objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        MyBase.New(objParametriServer, objParametriUtenti)
    End Sub

    Public Function Add(dati As DatiNogmo) As RestSharp.IRestResponse
        Return request(dati, configurazioneNogmo.addEndPoint, RestSharp.Method.POST)
    End Function

    Public Function CheckBovine(dati As CheckBovine) As RestSharp.IRestResponse
        Return request(dati, configurazioneNogmo.checkEndPoint, RestSharp.Method.POST)
    End Function
End Class
