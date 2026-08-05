Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class HubIoT_UnitaMisuraConversione_R
    Inherits DataProvider

    Public Function Leggi(ByVal ID As Integer,
                          ByVal entita_origine As Integer,
                          ByVal udm_cod As Integer,
                          ByVal modalita_distribuzione As Integer,
                          ByVal mezzo As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_UnitaMisuraConversione_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [HubIoT_UnitaMisuraConversione] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If ID <> 0 Then
                strSql.AppendLine(" And ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If entita_origine <> 0 Then
                strSql.AppendLine(" And EntitaOrigine = " & Agro_SQL_SaveNum(entita_origine) & " ")
            End If

            If udm_cod <> 0 Then
                strSql.AppendLine(" And uom_cod = " & Agro_SQL_SaveNum(udm_cod) & " ")
            End If

            If modalita_distribuzione <> 0 Then
                strSql.AppendLine(" And TipoModalitaDistribuzione = " & Agro_SQL_SaveNum(modalita_distribuzione) & " ")
            End If

            If mezzo <> 0 Then
                strSql.AppendLine(" And TipoMezzo = " & Agro_SQL_SaveNum(mezzo) & " ")
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
