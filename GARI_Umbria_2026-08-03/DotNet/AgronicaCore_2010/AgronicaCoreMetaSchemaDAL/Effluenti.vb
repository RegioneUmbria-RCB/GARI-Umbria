Imports AgronicaCoreDataProvider.UtilityProvider
Public Class Effluenti
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal Eff_Cod As Int32,
                          ByVal Regolamento_Cod As Int32,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Effluenti.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT e.Regolamento_Cod, e.Eff_Cod,e.Eff_Des,   ")
            StrSQL.AppendLine(" te.Tipo_Eff_Cod, te.Tipo_Eff_Des, te.SpecieAllevamento, te.MatricePrevalente, ")
            StrSQL.AppendLine(" tf.id_tp_fer, tf.descrizione, f.Fer_des,f.Fer_Cod,f.n,f.P2O5,f.K2O,f.MgO,f.Cu,f.CuPeso, e.Udm_Cod,udm.UDM_SIM ")
            StrSQL.AppendLine(" FROM   Effluenti e ")
            StrSQL.AppendLine(" inner Join EffluentixFertilizzanti ef on ef.Eff_Cod=e.Eff_Cod And ef.Regolamento_Cod=e.Regolamento_Cod ")
            StrSQL.AppendLine(" inner Join FertilizzantixTipoOrganici fto on fto.FR_COD=ef.Fer_Cod And fto.Regolamento_Cod=ef.Regolamento_Cod ")
            StrSQL.AppendLine(" inner Join TipoFertilizzante tf on tf.id_tp_fer=fto.id_tp_fer ")
            StrSQL.AppendLine(" inner Join TipoEffluente te on te.Tipo_Eff_Cod=e.Tipo_Eff_Cod And te.Regolamento_Cod=e.Regolamento_Cod ")
            StrSQL.AppendLine(" inner Join Fertilizzanti f on f.Fer_Cod=ef.Fer_Cod ")
            StrSQL.AppendLine(" inner Join UnitaMisura udm on udm.UDM_COD=e.Udm_Cod ")

            StrSQL.AppendLine(" WHERE e.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   e.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Eff_Cod <> 0 Then
                StrSQL.AppendLine(" AND e.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If
            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND e.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function

    Public Function LeggiDivietoSpandimenti(ByVal Eff_Div_Cod As Int32,
                          ByVal Regolamento_Cod As Int32,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Effluenti.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine("  e.Regolamento_Cod, e.Eff_Div_Cod,e.Eff_Div_Des ")
            'StrSQL.AppendLine("  , te.Tipo_Eff_Cod, te.Tipo_Eff_Des, te.SpecieAllevamento, te.MatricePrevalente ")
            'StrSQL.AppendLine("  , tf.id_tp_fer, tf.descrizione, f.Fer_des,f.Fer_Cod,f.n,f.P2O5,f.K2O,f.MgO,f.Cu,f.CuPeso, e.Udm_Cod,udm.UDM_SIM ")
            StrSQL.AppendLine("FROM EffluentiDivieti e ")
            'StrSQL.AppendLine("  INNER JOIN EffluentixFertilizzanti ef ON ef.Eff_Cod = e.Eff_Div_Cod AND ef.Regolamento_Cod = e.Regolamento_Cod ")
            'StrSQL.AppendLine("  INNER JOIN FertilizzantixTipoOrganici fto ON fto.FR_COD = ef.Fer_Cod AND fto.Regolamento_Cod = ef.Regolamento_Cod ")
            'StrSQL.AppendLine("  INNER JOIN TipoFertilizzante tf ON tf.id_tp_fer = fto.id_tp_fer ")
            'StrSQL.AppendLine("  INNER JOIN TipoEffluente te ON te.Tipo_Eff_Cod = e.Tipo_Eff_Cod AND te.Regolamento_Cod = e.Regolamento_Cod ")
            'StrSQL.AppendLine("  INNER JOIN Fertilizzanti f ON f.Fer_Cod = ef.Fer_Cod ")
            'StrSQL.AppendLine("  INNER JOIN UnitaMisura udm ON udm.UDM_COD = e.Udm_Cod ")

            StrSQL.AppendLine("WHERE e.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("  AND   e.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Eff_Div_Cod <> 0 Then
                StrSQL.AppendLine("  AND e.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Div_Cod) & "  ")
            End If
            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine("  AND e.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function


End Class
