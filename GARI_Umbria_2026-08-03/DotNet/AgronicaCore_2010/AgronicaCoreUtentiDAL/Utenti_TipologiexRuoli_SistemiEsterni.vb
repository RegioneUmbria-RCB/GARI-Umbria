

Imports AgronicaCoreDTOStd.Identity

Public Class Utenti_TipologiexRuoli_SistemiEsterni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Sistema_Esterno As Integer,
                          ByVal Ruolo_SistemaEsterno As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexRuoli_SistemiEsterni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Utenti_TipologiexRuoli_SistemiEsterni ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Sistema_Esterno <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Esterno = " & Agro_SQL_SaveNum(Sistema_Esterno) & " ")
            End If

            If Ruolo_SistemaEsterno <> "" Then
                StrSQL.AppendLine(" AND Ruolo_SistemaEsterno = '" & Agro_SQL_SaveText(Ruolo_SistemaEsterno) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
