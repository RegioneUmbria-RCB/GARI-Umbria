Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class CheckList_Extension_R
    Inherits DataProvider

    Public Function LeggiChecklistEstentionPerCodiceAlgoritmo(ByVal Algorimo_Cod As Integer,
                                                              ByVal data_validita As DateTime,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_Extension_R.LeggiChecklistEstentionPerCodiceAlgoritmo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	GIS_LayerAnalysisConfig_CheckList_Type_Cod ")
            StrSQL.AppendLine(" 	, a.GIS_LayerAnalysisConfig_CheckList_Cod ")
            StrSQL.AppendLine(" 	, Descrizione ")
            StrSQL.AppendLine(" 	, LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	, b.Applica_Risultato ")
            StrSQL.AppendLine(" FROM GIS_LayerAnalysisConfig_CheckList a ")
            StrSQL.AppendLine(" INNER Join GIS_LayerAnalysisConfigXCheckList b ")
            StrSQL.AppendLine("     ON a.GIS_LayerAnalysisConfig_CheckList_Cod=b.GIS_LayerAnalysisConfig_CheckList_Cod ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" a.Validita_Inizio<= {0} and a.Validita_Fine>= {1} ", Agro_SQL_SaveDateTime(data_validita), Agro_SQL_SaveDateTime(data_validita)))

            If Algorimo_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" and b.LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(Algorimo_Cod)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function
End Class
Public Class CheckList_Extension_W
    Inherits DataProvider

End Class
