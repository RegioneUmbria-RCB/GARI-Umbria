Imports System.Net.Http

Public Class ElasticSearchColdiretti
    Inherits WebApiCaller
    Implements IDisposable

    Public Sub New(baseUrl As String, token As Tuple(Of String, String))
        MyBase.New(baseUrl, token, NameOf(ElasticSearchColdiretti), Nothing)
    End Sub

    Public Function ChiamaWSLogger(ByVal dataLog As String, Optional ByVal endPoint As String = "") As Boolean
        Return ChiamaWS(Of Boolean)(HttpMethod.Post, Nothing, Nothing, dataLog, "application/json", endPoint, True)
    End Function
End Class
