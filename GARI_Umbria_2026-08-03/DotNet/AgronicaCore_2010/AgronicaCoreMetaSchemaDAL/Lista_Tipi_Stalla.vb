Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_Tipi_Stalla_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
    ''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
    ''' <param name="Ipro_Cod">Passare -1 per non filtrare</param>
    ''' <param name="Cod_Fabb"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Leggi(ByVal Gen_Cod As Integer,
                          ByVal Spe_Cod As Integer,
                          ByVal Ipro_Cod As Integer,
                          ByVal Cod_Fabb As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri) As DataTable
        Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Tipi_Stalla_R.Leggi()"

        Dim msgErrore As String = ""
        Dim strSQL As New StringBuilder
        Dim dt As DataTable

        Try
            strSQL.Length = 0

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSQL.AppendLine("SELECT * ").
                        AppendLine("FROM Lista_Tipi_Stalla ").
                        AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
                        AppendLine("     AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Gen_Cod <> 0 Then strSQL.AppendLine("     AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

                    If Spe_Cod <> 0 Then strSQL.AppendLine("     AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

                    If Ipro_Cod <> 0 Then strSQL.AppendLine("     AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & " ")

                    If Cod_Fabb <> "" Then strSQL.AppendLine("     AND COD_FABB = '" & Agro_SQL_SaveText(Cod_Fabb) & "' ")

                    If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSQL.AppendLine("    AND Lista_Tipi_Stalla.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSQL.AppendLine("    AND Lista_Tipi_Stalla.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti

                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSQL.AppendLine("ORDER BY COD_FABB ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            msgErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, msgErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Regolamento_Cod"></param>
    ''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
    ''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
    ''' <param name="Cat_Cod">Passare -1 per non filtrare</param>
    ''' <param name="IPro_Cod">Passare -1 per non filtrare</param>
    ''' <param name="objParametri"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <returns></returns>
    Public Function LeggiDaRegolamentoCategoria(ByVal Regolamento_Cod As Integer,
                                                ByVal Spe_Cod As Integer,
                                                ByVal Gen_Cod As Integer,
                                                ByVal Cat_Cod As Integer,
                                                ByVal IPro_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                Optional ByVal xFiltroAggiuntivo As String = "",
                                                Optional ByVal xOrderBy As String = "") As DataTable
        Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Tipi_Stalla_R.LeggiDaRegolamentoCategoria()"
        Dim strSQL As New StringBuilder
        Dim dt As New DataTable

        Try
            strSQL.Length = 0

            strSQL.AppendLine("SELECT Lista_Tipi_Fabbricati.DESCR as Descr_Fabb, Lista_TipiStallaxCategorie.* ").
                AppendLine("FROM Lista_TipiStallaxCategorie ").
                AppendLine("INNER JOIN Lista_Tipi_Fabbricati ").
                AppendLine("    ON Lista_TipiStallaxCategorie.COD_FABB COLLATE Latin1_General_CI_AS ").
                Append(" = Lista_Tipi_Fabbricati.COD_FABB COLLATE Latin1_General_CI_AS ").
                AppendLine("WHERE Lista_TipiStallaxCategorie.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
                AppendLine("    AND Lista_TipiStallaxCategorie.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Spe_Cod <> -1 Then strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

            If Gen_Cod <> -1 Then strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

            If Cat_Cod > -1 Then strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & " ")

            If IPro_Cod > -1 Then strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod) & " ")

            If Regolamento_Cod <> 0 Then strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine("    AND Lista_TipiStallaxCategorie.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti

                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)

        Catch ex As Exception
            dt = Nothing
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

End Class
