Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Modalita_Applicazione_Globali
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Codice As Integer,
                            ByVal AGEA_Cod As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Modalita_Applicazione_Globali.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Modalita_Applicazione_Globali WITH(NOLOCK)")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Codice <> 0 Then
                StrSQL.AppendLine(" AND Codice = " & Agro_SQL_SaveNum(Codice))
            End If

            If AGEA_Cod <> "" Then
                StrSQL.AppendLine(" AND AGEA_Cod = '" & Agro_SQL_SaveText(AGEA_Cod) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & xFiltroAggiuntivo)
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & xOrderBy)
            End If


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
