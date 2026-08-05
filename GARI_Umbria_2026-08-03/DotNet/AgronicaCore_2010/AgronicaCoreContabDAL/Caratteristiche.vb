Public Class Caratteristiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################
    Public Function Leggi(ByVal Mac_Cod As Int32,
                          ByVal username As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Caratteristiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM   Parco_MacchineXCaratteristiche ")

            StrSQL.Append(" WHERE  Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            StrSQL.Append(" AND Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")

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