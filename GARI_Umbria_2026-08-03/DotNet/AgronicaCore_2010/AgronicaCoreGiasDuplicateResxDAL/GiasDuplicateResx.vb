Public Class GiasDuplicateResx_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function InserisciRecordIntoResxDotNetData(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGiasDuplicateResxDAL.GiasDuplicateResx_W.InserisciRecordIntoResxDotNetData()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function
End Class
Public Class GiasDuplicateResx_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiChiaviDuplicati(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGiasDuplicateResxDAL.GiasDuplicateResx_R.LeggiChiaviDuplicati()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function
End Class
