Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class TipologieSementi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal SEM_COD As Integer,
                          ByVal Cerca_SemDes As String,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.TipologieSementi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT    TipologieSementi.* ")
            StrSQL.Append(" FROM      TipologieSementi ")
            StrSQL.Append(" WHERE     1 = 1 ")

            If SEM_COD <> 0 Then
                StrSQL.Append(" AND TipologieSementi.Sem_Cod = " & Agro_SQL_SaveNum(SEM_COD) & " ")
            End If

            If Cerca_SemDes <> "" Then
                StrSQL.Append(" AND TipologieSementi.Sem_Des LIKE '%" & Agro_SQL_SaveText(Cerca_SemDes) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Sem_Des ")
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
