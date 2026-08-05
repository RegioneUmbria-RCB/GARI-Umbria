Public Class Proiezioni_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiElencoAlgoritmi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Proiezioni_R.LeggiElencoAlgoritmi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ALG.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 	, ALG.LayerAnalysisConfig_Algorithm_Des ")
            StrSQL.AppendLine(" 	, ALGP.LayerAnalysisConfig_Algorithm_Param_Des ")
            StrSQL.AppendLine(" 	, ALGP.LayerAnalysisConfig_Algorithm_Param_Cod ")
            StrSQL.AppendLine(" 	, ALGP.LayerXConfig_Cod ")
            StrSQL.AppendLine(" FROM GIS_LayerAnalysisConfig_Algorithm ALG ")
            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig_Algorithm_Params ALGP ")
            StrSQL.AppendLine(" 	ON ALGP.LayerAnalysisConfig_Algorithm_Cod = ALG.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" ORDER BY ALG.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine("          , ALGP.LayerXConfig_Cod ")
            StrSQL.AppendLine("          , ALGP.LayerAnalysisConfig_Algorithm_Param_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function
End Class
Public Class Proiezioni_W

End Class
