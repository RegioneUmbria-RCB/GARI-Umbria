Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SpecieVegetalixGruppoVarietale_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################

    Public Function Leggi(ByVal VEG_COD As Integer, _
                        ByVal GRVA_COD As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.SpecieVegetalixGruppoVarietale_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append("  SELECT * " & _
                                    " FROM  SpecieVegetalixGruppoVarietale , SpecieVegetali , GruppoVarietale " & _
                                    " WHERE SpecieVegetalixGruppoVarietale.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                                    " AND   SpecieVegetalixGruppoVarietale.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
                                    " AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                                    " AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
                                    " AND   GruppoVarietale.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                                    " AND   GruppoVarietale.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
                                    " AND   SpecieVegetalixGruppoVarietale.VEG_COD = SpecieVegetali.VEG_COD " & _
                                    " AND   SpecieVegetalixGruppoVarietale.GRVA_COD = GruppoVarietale.GRVA_COD ")

                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND SpecieVegetalixGruppoVarietale.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If GRVA_COD <> 0 Then
                        StrSQL.Append(" AND SpecieVegetalixGruppoVarietale.GRVA_COD =  " & Agro_SQL_SaveNum(GRVA_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetalixGruppoVarietale.VEG_COD ASC, SpecieVegetalixGruppoVarietale.GRVA_COD ASC")
                    End If


            End Select

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



    'Public Function Leggi(ByVal Id_Servizio As Long, _
    '                        ByVal Utente As String, _
    '                        ByVal Utente_Profilo As String, _
    '                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.SpecieVegetalixGruppoVarietale_R.Leggi()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------
    '        Select Case xSelezioneVariabile

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
    '                StrSQL.Length = 0

    '                StrSQL.Append(" SELECT * ")
    '                StrSQL.Append(" FROM     Utenti_Profili " & _
    '                                " WHERE    Id_Servizio = " & Id_Servizio & " ")

    '                If Utente <> "" Then
    '                    StrSQL.Append(" AND Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
    '                End If

    '                If Utente_Profilo <> "" Then
    '                    StrSQL.Append(" AND Utente_Profilo = '" & Agro_SQL_SaveText(Utente_Profilo) & "' ")
    '                End If


    '                StrSQL.Append("  SELECT * " & _
    '                       " FROM  SpecieVegetalixGruppoVarietale , SpecieVegetali , GruppoVarietale " & _
    '                       " WHERE SpecieVegetalixGruppoVarietale.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
    '                       " AND   SpecieVegetalixGruppoVarietale.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
    '                       " AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
    '                       " AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
    '                       " AND   GruppoVarietale.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
    '                       " AND   GruppoVarietale.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
    '                       " AND   SpecieVegetalixGruppoVarietale.VEG_COD = SpecieVegetali.VEG_COD " & _
    '                       " AND   SpecieVegetalixGruppoVarietale.GRVA_COD = GruppoVarietale.GRVA_COD ")

    '                If VEG_COD <> 0 Then
    '                    sSql = sSql & " AND SpecieVegetalixGruppoVarietale.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  "
    '                End If

    '                If GRVA_COD <> 0 Then
    '                    sSql = sSql & " AND SpecieVegetalixGruppoVarietale.GRVA_COD =  " & Agro_SQL_SaveNum(GRVA_COD) & "  "
    '                End If

    '                sSql = sSql & " ORDER BY SpecieVegetalixGruppoVarietale.VEG_COD ASC, SpecieVegetalixGruppoVarietale.GRVA_COD ASC"


    '                'StrSQL.Append(" WHERE GruppoVegetale.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                'StrSQL.Append(" AND   GruppoVegetale.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                'If TECN_COD <> 0 Then
    '                '    StrSQL.Append(" AND ConduzioneTerrenoTraFila.TECN_COD =  " & Agro_SQL_SaveNum(TECN_COD) & "  ")
    '                'End If

    '                'If Gru_Cod <> 0 Then
    '                '    StrSQL.Append(" AND ConduzioneTerrenoTraFila.GRU_COD =  " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
    '                'End If

    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                End If


    '        End Select

    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT

    'End Function
End Class





