Public Class Journey
    Public Start As Vertice
    Public Finish As Vertice
    Public Routes As New List(Of Route)
    Public Complete As Boolean = False

    Public Sub Calculate()
        While Me.Complete = False
            Me.MoveNext()
        End While
    End Sub

    Private Sub MoveNext()
        Dim newRoutes As New List(Of Route)

        For Each oldRoute As Route In Routes
            For Each newRoute As Route In oldRoute.NextRoutes

                If newRoute.LastVert.Name <> Start.Name Then

                    newRoutes.Add(newRoute)

                    If newRoute.LastVert.Name = Finish.Name Then
                        Complete = True
                    End If

                End If
            Next
        Next

        If Complete = True Then
            Routes = New List(Of Route)

            For Each newRoute As Route In newRoutes

                If newRoute.LastVert.Name = Finish.Name Then

                    Routes.Add(newRoute)

                End If
            Next
        Else
            Routes = newRoutes
        End If
    End Sub

    Public Sub New(ByVal Start As Vertice, ByVal Finish As Vertice)
        Routes.Add(New Route(Start))
        Me.Start = Start
        Me.Finish = Finish

    End Sub
End Class
