Imports AgronicaCoreDataProvider.UtilityProvider

Public Class EfficienzaxTipoFertilizzante_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Tp_Fer As Int32, _
                            ByVal Efficienza_Cod As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EfficienzaxTipoFertilizzante_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Id_Tp_Fer, Efficienza_Cod, Coeff_Efficienza  ")
            StrSQL.Append(" FROM         EfficienzaxTipoFertilizzante ")

            StrSQL.Append(" WHERE EfficienzaxTipoFertilizzante.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EfficienzaxTipoFertilizzante.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Tp_Fer <> 0 Then
                StrSQL.Append(" AND Id_Tp_Fer =  " & Agro_SQL_SaveNum(Id_Tp_Fer) & "  ")
            End If
            If Efficienza_Cod <> 0 Then
                StrSQL.Append(" AND Efficienza_Cod =  " & Agro_SQL_SaveNum(Efficienza_Cod) & "  ")
            End If
    
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Id_Tp_Fer, Efficienza_Cod ")
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

    'Utilizzata solo per il PUA 2007
    Public Function Efficienza_From_Id_Tp_Fer_Em_Cod(ByVal Id_Tp_Fer As Int32, _
                                                     ByVal Em_Cod As Int32, _
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                  ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EfficienzaxTipoFertilizzante_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Efficienza As Decimal = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Coeff_Efficienza  ")
            StrSQL.Append(" FROM         EfficienzaxTipoFertilizzante INNER JOIN ")
            StrSQL.Append(" EpocheModalita ON EfficienzaxTipoFertilizzante.Efficienza_Cod = EpocheModalita.Efficienza_Cod ")
            StrSQL.Append("  AND  EfficienzaxTipoFertilizzante.Regolamento_Cod = EpocheModalita.Regolamento_Cod ")

            StrSQL.Append(" WHERE EfficienzaxTipoFertilizzante.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EfficienzaxTipoFertilizzante.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND EfficienzaxTipoFertilizzante.Regolamento_Cod =  1 ")

            If Id_Tp_Fer <> 0 Then
                StrSQL.Append(" AND EfficienzaxTipoFertilizzante.Id_Tp_Fer =  " & Agro_SQL_SaveNum(Id_Tp_Fer) & "  ")
            End If
            If Em_Cod <> 0 Then
                StrSQL.Append(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(Em_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Efficienza = DT.Rows(0).Item("Coeff_Efficienza")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Efficienza


    End Function

End Class
