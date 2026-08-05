Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class EffluentixFertilizzanti
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Eff_Cod As Int32, _
                          ByVal Fer_Cod As Int32, _
                          ByVal Regolamento_Cod As Int32, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EffluentixFertilizzanti.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Efficienza As Decimal = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *  ")
            StrSQL.Append(" FROM   EffluentixFertilizzanti ")

            StrSQL.Append(" WHERE EffluentixFertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EffluentixFertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Eff_Cod <> 0 Then
                StrSQL.Append(" AND EffluentixFertilizzanti.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If

            If Fer_Cod <> 0 Then
                StrSQL.Append(" AND EffluentixFertilizzanti.Fer_Cod =  " & Agro_SQL_SaveNum(Fer_Cod) & "  ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND EffluentixFertilizzanti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
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
