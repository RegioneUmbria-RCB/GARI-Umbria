Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class EnteTecnico_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Ente_COD As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EnteTecnico_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT * FROM  EnteTecnico " & _
                          " WHERE 1=1 ")

            If Ente_COD <> 0 Then
                StrSQL.Append(" AND Ente_COD =  " & Agro_SQL_SaveNum(Ente_COD) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ENTE_RAGSOC ASC ")
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

    Public Function EnteCod_from_ENTE_CODIFICA(ByVal Ente_CODifica As String, _
                        ByVal xFiltroAggiuntivo As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                  ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EnteTecnico_R.EnteCod_from_ENTE_CODIFICA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Ente_Cod As Integer = 0

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT ente_cod FROM  EnteTecnico " & _
                          " WHERE 1=1 ")

            If Ente_CODifica <> "" Then
                StrSQL.Append(" AND Ente_CODifica =  '" & Agro_SQL_SaveText(Ente_CODifica) & "'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Ente_Cod = DT.Rows(0).Item("Ente_COD")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Ente_Cod = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Ente_Cod


    End Function

End Class
