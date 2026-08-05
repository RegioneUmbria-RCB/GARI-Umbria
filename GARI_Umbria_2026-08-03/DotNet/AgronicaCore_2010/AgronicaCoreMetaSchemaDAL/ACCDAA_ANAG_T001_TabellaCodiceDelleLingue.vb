Imports AgronicaCoreDataProvider.UtilityProvider
Public Class ACCDAA_ANAG_T001_TabellaCodiceDelleLingue_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Codice As String,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.ACCDAA_ANAG_T001_TabellaCodiceDelleLingue_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  ACCDAA_ANAG_T001_TabellaCodiceDelleLingue " + vbCrLf)
            StrSQL.Append(" WHERE 1 = 1 " & vbCrLf)

            If Codice <> "" Then
                StrSQL.Append(" AND Codice = '" & Agro_SQL_SaveText(Codice) + "'" + vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & xFiltroAggiuntivo + vbCrLf)
            End If

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descrizione " & xOrderBy)
            End If


            '=====================================================================

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
