Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_tipi_fabbricati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    <Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Tipi_Fabbricati_R.Leggi()")>
    Public Function Leggi(ByVal Cod_Fabb As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreZooDAL.Lista_tipi_fabbricati_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Lista_Tipi_Fabbricati ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cod_Fabb <> "" Then
                        StrSQL.Append(" AND COD_FABB = '" & Agro_SQL_SaveText(Cod_Fabb) & "'  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Cod_Fabb ASC ")
                    End If
                    '------------------------------------------------------------------

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    <Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Tipi_Fabbricati_R.Lista_Tipi_Fabbricati_Descr_from_COD_FABB()")>
    Public Function Lista_Tipi_Fabbricati_Descr_from_COD_FABB(ByVal COD_FABB As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As String

        Dim dt As New DataTable
        dt = Leggi(CStr(COD_FABB), enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("DESCR")
        End If

        Return ""

    End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
