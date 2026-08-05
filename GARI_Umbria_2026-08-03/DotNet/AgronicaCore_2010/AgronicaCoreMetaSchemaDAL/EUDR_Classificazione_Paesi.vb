Public Class EUDR_Classificazione_Paesi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Codice As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.EUDR_Classificazione_Paesi_R.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Codice, Descrizione, Rischio, Punteggio, PaeseEU ")
            StrSQL.AppendLine(" FROM  EUDR_Classificazione_Paesi ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Codice <> "" Then
                StrSQL.AppendLine(" AND Codice IN (" & Agro_SQL_Save_Clausola_IN(Codice, True) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & xFiltroAggiuntivo)
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Descrizione")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class
