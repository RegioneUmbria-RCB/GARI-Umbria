Public Class Route
    Public vertici As List(Of Vertice)
    Public LastVert As Vertice

    Public Function Add(ByVal newVertice As Vertice) As Boolean
        Try
            vertici.Add(newVertice)
            LastVert = newVertice
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function NextRoutes() As List(Of Route)

        Dim theNextRoutes As New List(Of Route)
        Dim newRoute As Route

        For Each adjacent As Vertice In LastVert.listaSuccessori
            newRoute = New Route(Me.vertici)
            If newRoute.Add(adjacent) = True Then
                theNextRoutes.Add(newRoute)
            End If
        Next
        Return theNextRoutes
    End Function

    Public Sub New(ByVal ParentVertici As List(Of Vertice))
        Me.vertici = New List(Of Vertice)
        For Each vt As Vertice In ParentVertici
            Me.Add(vt)
        Next
    End Sub

    Public Sub New(ByVal StartVertice As Vertice)
        vertici = New List(Of Vertice)
        Me.Add(StartVertice)
    End Sub
End Class
