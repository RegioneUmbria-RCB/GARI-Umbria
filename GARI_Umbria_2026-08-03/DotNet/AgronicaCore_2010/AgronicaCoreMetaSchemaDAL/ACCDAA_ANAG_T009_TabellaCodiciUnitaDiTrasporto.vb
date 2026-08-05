Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ACCDAA_ANAG_T009_TabellaCodiciUnitaDiTrasporto_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal codice As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.ACCDAA_ANAG_T009_TabellaCodiciUnitaDiTrasporto_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  ACCDAA_ANAG_T009_TabellaCodiciUnitaDiTrasporto ")
            strSql.AppendLine(" WHERE 1 = 1 ")

            If codice <> -1 Then
                strSql.AppendLine(" AND Codice = " & Agro_SQL_SaveNum(codice) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Descrizione ASC ")
            End If

            '=====================================================================

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
