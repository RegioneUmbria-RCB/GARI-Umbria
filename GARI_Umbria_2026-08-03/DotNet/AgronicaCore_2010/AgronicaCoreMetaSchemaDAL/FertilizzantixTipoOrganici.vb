Imports AgronicaCoreDataProvider.UtilityProvider

Public Class FertilizzantixTipoOrganici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Fr_Cod As Int32, _
                          ByVal Id_Tp_Fer As Int32, _
                                     ByVal xFiltroAggiuntivo As String, _
                             ByVal xOrderBy As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FertilizzantixTipoOrganici_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Fr_Cod, Id_Tp_Fer  ")
            StrSQL.Append(" FROM   FertilizzantixTipoOrganici ")

            StrSQL.Append(" WHERE FertilizzantixTipoOrganici.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   FertilizzantixTipoOrganici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Tp_Fer <> 0 Then
                StrSQL.Append(" AND Id_Tp_Fer =  " & Agro_SQL_SaveNum(Id_Tp_Fer) & "  ")
            End If
            If Fr_Cod <> 0 Then
                StrSQL.Append(" AND Fr_Cod =  " & Agro_SQL_SaveNum(Fr_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Id_Tp_Fer, Fr_Cod ")
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
