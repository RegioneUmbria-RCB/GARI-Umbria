Imports System.Net.Http
Public Class Controller
    Inherits WebApiCaller
    Public Sub New(baseUrl As String, token As Tuple(Of String, String))
        MyBase.New(baseUrl, token)
    End Sub
    Public Function ChiamaWSLogger(ByVal dataLog As String) As Boolean
        Return ChiamaWS(Of Boolean)(HttpMethod.Post, Nothing, Nothing, dataLog, "application/json", "", True)
    End Function
End Class

