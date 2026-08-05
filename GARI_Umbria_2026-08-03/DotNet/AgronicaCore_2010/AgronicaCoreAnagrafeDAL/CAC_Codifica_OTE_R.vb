
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class CAC_Codifica_OTE_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi( _
                            ByVal PivaSuperUser As String, _
                            ByVal Cod_Cliente As String, _
                            ByVal Ote_cod As String, _
                            ByVal Descrizione As String, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_OTE_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    CAC_Codifica_OTE ")
            StrSQL.Append(" WHERE   PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

            If Cod_Cliente <> "" Then
                StrSQL.Append(" AND Cod_Cliente = '" & Agro_SQL_SaveText(Cod_Cliente) & "'   ")
            End If

            If Ote_cod <> "" Then
                StrSQL.Append(" AND Ote_cod = " & Agro_SQL_SaveNum(Ote_cod) & "  ")
            End If

            If Descrizione <> "" Then
                StrSQL.Append(" AND Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Cod_Cliente ASC")
            End If



            '------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


End Class
