Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class HubIoT_RegoleXEntita_R
    Inherits DataProvider

    Public Function Leggi(ByVal Entita_Origine As Integer,
                          ByVal Categoria_Lavorazione As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_RegoleXEntita_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [HubIoT_RegoleXEntita] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If Entita_Origine <> 0 Then
                strSql.AppendLine(" And Entita_Origine = " & Agro_SQL_SaveNum(Entita_Origine) & " ")
            End If

            If Categoria_Lavorazione <> 0 Then
                strSql.AppendLine(" And Categoria_Lavorazione = " & Agro_SQL_SaveNum(Categoria_Lavorazione) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function
End Class
