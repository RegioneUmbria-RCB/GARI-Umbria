
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class GruppoAvversita_R


    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################################
    Public Function Leggi(ByVal AV_GRU As Int32,
                          ByVal Livello As Int32,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoAvversita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Av_Gru, Av_Gru_Des, Av_Gru_Des_Lat, Cod_Bayer, Livello   ")
                    StrSQL.AppendLine(" FROM  GruppoAvversita ")
                    StrSQL.AppendLine(" WHERE GruppoAvversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND GruppoAvversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND (Av_Gru_Des NOT LIKE '%non usare%') AND (Av_Gru_Des NOT LIKE '%(#)%') AND (Av_Gru_Des_Lat NOT LIKE '%non usare%') AND (Av_Gru_Des_Lat NOT LIKE '%(#)%') ")

                    If AV_GRU <> 0 Then
                        StrSQL.AppendLine(" AND GruppoAvversita.AV_GRU =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
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

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * FROM GruppoAvversita ")
                    StrSQL.AppendLine(" WHERE GruppoAvversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND GruppoAvversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If AV_GRU <> 0 Then
                        StrSQL.AppendLine(" AND GruppoAvversita.AV_GRU =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
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

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

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

    '#############################################################################################################
    Public Function AvGruDes_from_AvGruCod(ByVal Av_Gru As Int32,
                                        ByRef Av_Gru_Des_Lat As String,
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoAvversita_R.AvGruDes_from_AvGruCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Av_Gru_Des As String = ""

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Av_Gru_Des, Av_Gru_Des_Lat " &
                                  " FROM   GruppoAvversita " &
                                  " WHERE  GruppoAvversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                                  " AND    GruppoAvversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " &
                                    " AND (Av_Gru_Des NOT LIKE '%non usare%') AND (Av_Gru_Des NOT LIKE '%(#)%') AND (Av_Gru_Des_Lat NOT LIKE '%non usare%') AND (Av_Gru_Des_Lat NOT LIKE '%(#)%')")

                    If Av_Gru <> 0 Then
                        StrSQL.AppendLine(" AND GruppoAvversita.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND  Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND  Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine("  ORDER BY Av_Gru_Des")
                    End If

            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Av_Gru_Des = DT.Rows(0).Item("Av_Gru_Des")
                Av_Gru_Des_Lat = DT.Rows(0).Item("Av_Gru_Des_Lat")
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Av_Gru_Des


    End Function

    Public Function AvGruDes_Lat_from_AvGruCod(ByVal Av_Gru As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoAvversita_R.AvGruDes_Lat_from_AvGruCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Av_Gru_Des As String = ""

        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Av_Gru_Des, Av_Gru_Des_Lat " &
                            " FROM   GruppoAvversita " &
                            " WHERE  GruppoAvversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                            " AND    GruppoAvversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " &
                            " AND (Av_Gru_Des NOT LIKE '%non usare%') AND (Av_Gru_Des NOT LIKE '%(#)%') AND (Av_Gru_Des_Lat NOT LIKE '%non usare%') AND (Av_Gru_Des_Lat NOT LIKE '%(#)%')")

            If Av_Gru <> 0 Then
                StrSQL.AppendLine(" AND GruppoAvversita.Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY Av_Gru_Des")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count = 1 Then
                Av_Gru_Des = DT.Rows(0)("Av_Gru_Des") & " (" & DT.Rows(0)("Av_Gru_Des_Lat") & ")"
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Av_Gru_Des


    End Function

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################



End Class
