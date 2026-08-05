Public Class UnitaMisura_Conversione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Udm_Cod_Da As Integer,
                          ByVal Udm_Cod_A As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As DateTime = Nothing,
                          Optional ByVal Validita_Fine As DateTime = Nothing) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.UnitaMisura_Conversione_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            If Udm_Cod_Da = 0 Then
                Throw New Exception("Parametro non corretto nella query (Udm_Cod_Da)")
            End If

            If Udm_Cod_A = 0 Then
                Throw New Exception("Parametro non corretto nella query (Udm_Cod_A)")
            End If

            StrSQL.Length = 0
            StrSQL.Append(" SELECT udm_cod_da, udm_cod_a, fattoreconversione ")
            StrSQL.Append(" FROM    UnitaMisura_Conversione ")
            StrSQL.Append(" WHERE  ")
            StrSQL.Append(" Udm_Cod_Da = " + Agro_SQL_SaveNum(Udm_Cod_Da))
            StrSQL.Append(" AND Udm_Cod_A =  " + Agro_SQL_SaveNum(Udm_Cod_A))

            If IsNothing(Validita_Inizio) Then
                StrSQL.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                'Else
                '    StrSQL.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            End If

            If IsNothing(Validita_Fine) Then
                StrSQL.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                'Else
                '    StrSQL.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Udm_Cod_Da ")
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

Public Class UnitaMisura_Conversione_W

End Class
