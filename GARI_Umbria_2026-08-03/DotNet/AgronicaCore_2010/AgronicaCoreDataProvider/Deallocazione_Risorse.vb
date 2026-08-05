Public Class Deallocazione_Risorse

    ''' <summary>
    ''' Dealloca datatable
    ''' </summary>
    ''' <param name="Dt"></param>
    ''' <remarks></remarks>
    Public Shared Sub Dealloca_DataTable(ByVal Dt As DataTable)

        If Not Dt Is Nothing Then
            Dt.Dispose()
            Dt = Nothing
        End If

    End Sub

End Class
