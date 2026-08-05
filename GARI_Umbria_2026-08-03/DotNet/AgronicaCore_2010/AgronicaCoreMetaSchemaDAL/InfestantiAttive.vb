
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class InfestantiAttive_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal AV_COD As Int32,
        ByVal AV_GRU As Int32,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InfestantiAttive_R.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM InfestantiAttive ")
                    StrSQL.AppendLine(" INNER JOIN Avversita ON InfestantiAttive.Av_Cod = Avversita.Av_Cod ")
                    StrSQL.AppendLine(" WHERE InfestantiAttive.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   InfestantiAttive.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.AppendLine(" SELECT ")
                    StrSQL.AppendLine("     InfestantiAttive.Av_Cod, InfestantiAttive.Av_Gru, Avversita.Av_Des_Vol, Avversita.Av_Des_Lat, Avversita.Abbreviazione  ")
                    StrSQL.AppendLine(" FROM InfestantiAttive ")
                    StrSQL.AppendLine(" INNER JOIN Avversita ON InfestantiAttive.AV_COD = Avversita.Av_Cod ")
                    StrSQL.AppendLine(" WHERE InfestantiAttive.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   InfestantiAttive.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            End Select

            If AV_COD <> 0 Then
                StrSQL.AppendLine(" AND InfestantiAttive.AV_COD =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
            End If
            If AV_GRU <> 0 Then
                StrSQL.AppendLine(" AND InfestantiAttive.AV_GRU =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Avversita.AV_DES_VOL ASC ")
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


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    'Public Function Leggi2(ByVal AV_COD As Int32, _
    '                          ByVal AV_GRU As Int32, _
    '                          ByVal Validita_Inizio As Date, _
    '                          ByVal Validita_Fine As Date, _
    '                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                          ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.InfestantiAttive_R.Leggi2()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        Select Case xSelezioneVariabile

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

    '                StrSQL.Length = 0

    '                StrSQL.Append(" SELECT * " & _
    '                                " FROM  InfestantiAttive, Avversita " & _
    '                                " WHERE InfestantiAttive.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
    '                                " AND   InfestantiAttive.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
    '                                " AND   InfestantiAttive.Av_Cod = Avversita.Av_Cod  " & "   ")


    '                If AV_COD <> 0 Then
    '                    StrSQL.Append(" AND InfestantiAttive.AV_COD =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
    '                End If

    '                If AV_GRU <> 0 Then
    '                    StrSQL.Append(" AND InfestantiAttive.AV_GRU =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
    '                End If


    '                '--------------------------------------------------------------------------
    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If

    '                '--------------------------------------------------------------------------

    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                Else
    '                    StrSQL.Append(" ORDER BY Avversita.AV_DES_VOL ASC ")
    '                End If


    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
    '                '
    '                '
    '                '
    '                '


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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################





End Class
