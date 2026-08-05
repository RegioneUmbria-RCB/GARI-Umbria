Public Class AgendaFiltroLettura

    Public PIVA As String
    Public Ids As List(Of Integer)
    Public DataDal As DateTime?
    Public DataAl As DateTime?
    Public Top As Integer

    Public Function IsEmpty() As Boolean

        If Not DataAl.HasValue AndAlso
             (Ids Is Nothing OrElse Not Ids.Any) Then
            Return True
        Else
            Return False
        End If

    End Function

End Class


