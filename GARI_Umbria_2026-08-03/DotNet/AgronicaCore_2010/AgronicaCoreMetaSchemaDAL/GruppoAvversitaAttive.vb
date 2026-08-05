Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class GruppoAvversitaAttive_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################################
    Public Function Leggi(
        ByVal AV_GRU As Int32,
        ByVal Livello As Int32,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.AppendLine(" SELECT DISTINCT ")
                    StrSQL.AppendLine("     GruppoAvversitaAttive.Av_Gru, GruppoAvversita.Av_Gru_Des, GruppoAvversita.Av_Gru_Des_Lat, ISNULL(GruppoAvversita.Livello,0) AS Livello ")
                    StrSQL.AppendLine(" FROM GruppoAvversitaAttive ")
                    StrSQL.AppendLine(" INNER JOIN GruppoAvversita ON GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru ")
                    StrSQL.AppendLine(" WHERE GruppoAvversitaAttive.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("     AND   GruppoAvversitaAttive.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine("     AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' ")
                    StrSQL.AppendLine("     AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM GruppoAvversitaAttive ")
                    StrSQL.AppendLine(" INNER JOIN GruppoAvversita ON GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru ")
                    StrSQL.AppendLine(" WHERE GruppoAvversitaAttive.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("     AND   GruppoAvversitaAttive.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine("     AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' ")
                    StrSQL.AppendLine("     AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' ")

            End Select

            If AV_GRU <> 0 Then
                StrSQL.AppendLine(" AND GruppoAvversitaAttive.AV_GRU =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
            End If
            If Livello <> 0 Then
                StrSQL.AppendLine(" AND GruppoAvversita.Livello =  " & Agro_SQL_SaveNum(Livello) & "  ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY GruppoAvversita.AV_GRU_DES ASC ")
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function


    '################################################################################
    Public Function Numero_Gruppi_Infestanti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Num As Integer = 0
        Dim Dt As DataTable

        Dt = Leggi(0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
            Num = Dt.Rows.Count
        End If

        Return Num

    End Function



End Class
