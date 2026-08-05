Imports AgronicaCoreDataProvider.UtilityProvider

Public Class EfficienzaRiferimentoxTipiAllevamentixEffluenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Efficienza_From_ALLCOD_EffCod(ByVal ALL_COD As Int32, _
                                                  ByVal Eff_Cod As Int32, _
                                                  ByVal Regolamento_Cod As Int32, _
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                  ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EfficienzaRiferimentoxTipiAllevamentixEffluenti_R.Efficienza_From_ALLCOD_EffCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Efficienza As Decimal = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Efficienza_Val ")
            StrSQL.Append(" FROM   EfficienzaRiferimentoxTipiAllevamentixEffluenti ")
            StrSQL.Append(" WHERE EfficienzaRiferimentoxTipiAllevamentixEffluenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EfficienzaRiferimentoxTipiAllevamentixEffluenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If ALL_COD <> -1 Then
                StrSQL.Append(" AND EfficienzaRiferimentoxTipiAllevamentixEffluenti.ALL_COD =  " & Agro_SQL_SaveNum(ALL_COD) & "  ")
            End If
            If Eff_Cod <> 0 Then
                StrSQL.Append(" AND EfficienzaRiferimentoxTipiAllevamentixEffluenti.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If
            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND EfficienzaRiferimentoxTipiAllevamentixEffluenti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Efficienza = DT.Rows(0).Item("Efficienza_Val")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Efficienza


    End Function

    Public Function Leggi(ByVal ALL_COD As Int32,
                        ByVal Eff_Cod As Int32,
                        ByVal Regolamento_Cod As Int32,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EfficienzaRiferimentoxTipiAllevamentixEffluenti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM  EfficienzaRiferimentoxTipiAllevamentixEffluenti ")
            StrSQL.Append(" WHERE EfficienzaRiferimentoxTipiAllevamentixEffluenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EfficienzaRiferimentoxTipiAllevamentixEffluenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If ALL_COD <> -1 Then
                StrSQL.Append(" AND EfficienzaRiferimentoxTipiAllevamentixEffluenti.ALL_COD =  " & Agro_SQL_SaveNum(ALL_COD) & "  ")
            End If
            If Eff_Cod <> 0 Then
                StrSQL.Append(" AND EfficienzaRiferimentoxTipiAllevamentixEffluenti.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If
            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND EfficienzaRiferimentoxTipiAllevamentixEffluenti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
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
