Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Programmazione_Particelle_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    'Trova tutti le entita di un'impresa associate ad una data particella
    '============================================================================
    Public Function LeggiEntitaImpresa_Da_Particella(ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal PROV As String,
                                                          ByVal COM As String,
                                                          ByVal SEZIONE As String,
                                                          ByVal FOGLIO As Int32,
                                                          ByVal NUMERO As Int32,
                                                          ByVal SUBALTERNO As String,
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaImpresa_Da_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM Programmazione_Particelle PP ")
                    StrSQL.Append(" INNER JOIN Programmazione_entita PE ")
                    StrSQL.Append(" ON PP.programmazione_Entita_Cod=PE.Programmazione_Entita_Cod ")

                    StrSQL.Append(" WHERE PE.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   PE.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   PP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND   PP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND   PP.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    End If
                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND   PP.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    End If
                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND   PP.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    End If
                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND   PP.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND   PE.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   PE.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PP.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PP.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY PE.Validita_inizio ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM Programmazione_Particelle PP ")
                    StrSQL.Append(" INNER JOIN Programmazione_entita PE ")
                    StrSQL.Append(" ON PP.programmazione_Entita_Cod=PE.Programmazione_Entita_Cod ")

                    StrSQL.Append(" WHERE PE.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   PE.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   PP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.Append(" AND   PP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND   PP.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    End If
                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND   PP.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    End If
                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND   PP.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    End If
                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND   PP.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")
                    End If


                    If Piva <> "" Then
                        StrSQL.Append(" AND   PE.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   PE.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PP.Inviato >=0 ")
                            StrSQL.Append(" AND   PE.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PP.Inviato =-1 ")
                            StrSQL.Append(" AND   PE.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY PE.Validita_inizio ASC")
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


    Public Function LeggiPossessi_GerarchiaImpresa(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal id_cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByVal filtroUltima As Boolean,
                                                             ByVal filtroEnti As Boolean
                                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiPossessi_GerarchiaImpresa()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    Dim strGroupBy As String = "t.cuaa, t.rag_soc, t.Programmazione_Des_Long,t.PIVA,t.Com, t.LOCALITA, t.Prov, t.COMUNI_PROV, t.foglio, t.Numero, t.Sezione, t.Subalterno, t.Superficie_Catastale, t.Conduzione_Inizio, t.Conduzione_Fine, t.Sup_Condotta, t.Validazione_Data, t.Allegati_Documenti_Numero, t.TitoloPossesso, t.TitoloPossesso_Des  "
                    StrSQL.Length = 0
                    StrSQL.Length = 0
                    StrSQL.Append("SELECT * FROM " & vbCrLf)
                    StrSQL.Append("( ")
                    StrSQL.Append("SELECT  " & vbCrLf)
                    StrSQL.Append("      ic.val_cod as cuaa,    " & vbCrLf)
                    StrSQL.Append("      i.rag_soc,   " & vbCrLf)
                    StrSQL.Append("      pt.Programmazione_Des_Long,    " & vbCrLf)
                    StrSQL.Append("      i.Piva,    " & vbCrLf)
                    StrSQL.Append("      pp.Com,    " & vbCrLf)
                    StrSQL.Append("      comune.LOCALITA,    " & vbCrLf)
                    StrSQL.Append("      pp.Prov,    " & vbCrLf)
                    StrSQL.Append("      comune.COMUNI_PROV,    " & vbCrLf)
                    StrSQL.Append("      pp.Foglio,    " & vbCrLf)
                    StrSQL.Append("      pp.Numero,    " & vbCrLf)
                    StrSQL.Append("      pp.Sezione,    " & vbCrLf)
                    StrSQL.Append("      pp.Subalterno,    " & vbCrLf)
                    StrSQL.Append("      CAST(pc.Ettari + (cast(pc.Are as real)/100) + (cast(pc.centiare as real)/10000) as real) as Superficie_Catastale,    " & vbCrLf)
                    StrSQL.Append("      ip.Sup_Condotta as Sup_Condotta,   " & vbCrLf)
                    StrSQL.Append("     ip.Validita_Inizio as Conduzione_Inizio, " & vbCrLf)
                    StrSQL.Append("     ip.Validita_Fine as Conduzione_Fine, " & vbCrLf)
                    StrSQL.Append("     ad.Validazione_Data, " & vbCrLf)
                    StrSQL.Append("     ad.Allegati_Documenti_Numero, " & vbCrLf)
                    StrSQL.Append("     ip.TitoloPossesso, " & vbCrLf)
                    StrSQL.Append("     CASE ip.TitoloPossesso " & vbCrLf)
                    StrSQL.Append("     	WHEN 0 THEN 'Altro'" & vbCrLf)
                    StrSQL.Append("     	WHEN 1 THEN 'Proprietà'" & vbCrLf)
                    StrSQL.Append("     	WHEN 2 THEN 'Comodato d''uso'" & vbCrLf)
                    StrSQL.Append("     	WHEN 3 THEN 'Affitto con contratto'" & vbCrLf)
                    StrSQL.Append("     	WHEN 4 THEN 'Affitto senza contratto'" & vbCrLf)
                    StrSQL.Append("     	ELSE 'Altro' END as TitoloPossesso_Des" & vbCrLf)
                    StrSQL.Append("  FROM Imprese I   " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio   " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Imprese padre ON gi.padre = padre.piva " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva   " & vbCrLf)
                    If filtroUltima Then
                        StrSQL.Append(" AND Pt.programmazione_Cod IN ( " & vbCrLf)
                        StrSQL.Append("    SELECT Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
                        StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
                        StrSQL.Append("    AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
                        StrSQL.Append("    AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
                        If Not filtroEnti Then
                            StrSQL.Append("    AND fonte_cod=0 " & vbCrLf)
                        Else
                            StrSQL.Append("    AND fonte_cod <> 0 " & vbCrLf)
                        End If
                        StrSQL.Append(" ) " & vbCrLf)
                    End If
                    StrSQL.Append("  LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN GruppoVarietale grva ON pe.Grva_Cod = grva.Grva_Cod " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno    " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN ImpresexParticelle ip ON ip.ID = (   " & vbCrLf)
                    StrSQL.Append("     SELECT max(ID) FROM ImpreseXParticelle   " & vbCrLf)
                    StrSQL.Append("     WHERE ImpreseXParticelle.Piva = i.piva    " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.prov = pc.prov   " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.com = pc.com   " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.sezione = pc.sezione   " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.foglio = pc.foglio   " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.numero = pc.numero   " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.subalterno = pc.subalterno   " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine) & "       " & vbCrLf)
                    StrSQL.Append("     AND ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_Inizio) & "     " & vbCrLf)
                    StrSQL.Append("  )   " & vbCrLf)
                    StrSQL.Append(" LEFT JOIN ParticelleCatastalixMacrousi pcm ON pcm.ID = (SELECT MAX(ID)    " & vbCrLf)
                    StrSQL.Append("													      FROM ParticelleCatastalixMacrousi   " & vbCrLf)
                    StrSQL.Append("														  WHERE pp.prov = ParticelleCatastalixMacrousi.prov AND    " & vbCrLf)
                    StrSQL.Append("														  pp.com = ParticelleCatastalixMacrousi.com AND    " & vbCrLf)
                    StrSQL.Append("														  pp.foglio = ParticelleCatastalixMacrousi.foglio AND   " & vbCrLf)
                    StrSQL.Append("														  pp.sezione = ParticelleCatastalixMacrousi.sezione AND    " & vbCrLf)
                    StrSQL.Append("														  pp.numero = ParticelleCatastalixMacrousi.numero AND    " & vbCrLf)
                    StrSQL.Append("														  pp.subalterno = ParticelleCatastalixMacrousi.subalterno AND    " & vbCrLf)
                    StrSQL.Append("														  pe.Macrouso_Cod = ParticelleCatastalixMacrousi.macrouso_Cod     " & vbCrLf)
                    StrSQL.Append("	)   " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033   " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov  " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod  " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod  " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod  " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Allegati_EntitaxDocumenti aed ON aed.Programmazione_Cod=pt.Programmazione_Cod AND aed.Programmazione_Entita_Cod=0 " & vbCrLf)
                    StrSQL.Append("  LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod " & vbCrLf)
                    'StrSQL.Append(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva ON csva.id_cod = pe.id_cod  " & vbCrLf)
                    StrSQL.Append(" WHERE 1=1 ")
                    'StrSQL.Append(" --AND pp.Com is not null " & vbCrLf)
                    StrSQL.Append(" AND pt.programmazione_Cod is not null " & vbCrLf)
                    StrSQL.Append(" AND pt.Tipo_Pianificazione in (0,2) " & vbCrLf)
                    StrSQL.Append(" AND pe.Appezza <> 0 " & vbCrLf)
                    If programmazione_Cod <> 0 Then
                        StrSQL.Append(" AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " " & vbCrLf)
                    End If
                    If programmazione_entita_Cod <> 0 Then
                        StrSQL.Append(" AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " " & vbCrLf)
                    End If
                    If ImpresePadri.Count <> 0 Then
                        If ImpresePadri.Count = 1 Then
                            Dim impresa = ImpresePadri(0)
                            If impresa <> "-1" Then
                                StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                            Else
                                StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                            End If
                        Else
                            StrSQL.Append(" AND gi.Padre IN ( ")
                            Dim first = True
                            For Each pPadre In ImpresePadri
                                If pPadre <> "" Then
                                    If Not first Then
                                        StrSQL.Append(", ")
                                    Else
                                        first = False
                                    End If
                                    StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                                End If
                            Next
                            StrSQL.Append(" ) ")
                        End If
                    End If
                    'If livello <> 0 Then
                    '    StrSQL.Append(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " & vbCrLf)
                    'End If
                    If Piva <> "" Then
                        StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
                    End If
                    If validita_Inizio <> AGRODATAINIZIO Then
                        StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
                    End If
                    If validita_fine <> AGRODATAFINE Then
                        StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If
                    If Not filtroEnti Then
                        StrSQL.Append("    AND pt.fonte_cod=0 " & vbCrLf)
                    End If
                    StrSQL.Append(") t " & vbCrLf)
                    StrSQL.Append(" GROUP BY " & strGroupBy & " ")


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

    Public Function LeggiEntitaImpresa_Da_Particella_GerarchiaImpresaOld(ByVal ImpresePadri As List(Of String),
                                                                      ByVal Piva As String,
                                                                      ByVal Sa_Cod As Int32,
                                                                      ByVal validita_Inizio As DateTime,
                                                                      ByVal validita_fine As DateTime,
                                                                      ByVal veg_Cods As List(Of String),
                                                                      ByVal id_cods As List(Of String),
                                                                      ByVal programmazione_Cod As Integer,
                                                                      ByVal programmazione_entita_Cod As Integer,
                                                                      ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                                      ByVal xFiltroAggiuntivo As String,
                                                                      ByVal xOrderBy As String,
                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                      ByVal filtroUltima As Boolean,
                                                                      ByVal filtroEnti As Boolean,
                                                                      ByRef dtCuaa As DataTable
                                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaImpresa_Da_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    Dim strGroupBy As String = " t.Programmazione_entita_Cod, t.cuaa, t.rag_soc, t.PIVA_Padre, t.Rag_Soc_Padre, t.Programmazione_Des_Long,t.PIVA,t.Com, t.LOCALITA, t.Prov, t.COMUNI_PROV, t.foglio, t.Numero, t.Sezione, t.Subalterno, t.Superficie_Catastale, t.Sup_Condotta, t.SupUtilizzo, t.supUtilizzo1, t.supMacrouso, t.macrouso_cod, t.macrouso_des, t.veg_cod, t.utilizzo, t.Cul_Cod, t.Cul_Des, t.Veg_Cod_Cliente, t.Cul_Cod_Cliente, t.codiceSocio, t.Validita_Inizio, t.Validita_Fine, t.Num_Piante, t.Cop_Cod, t.Cop_Des, t.MetodoProduzione_cod, t.MetodoProduzione_Des, t.Campo_Cod, t.Grva_Cod, t.Grva_Des, t.Conduzione_Inizio, t.Conduzione_Fine, t.Sup_Condotta, t.Validazione_Data, t.Allegati_Documenti_Numero, t.TitoloPossesso, t.TitoloPossesso_Des  "
                    StrSQL.Length = 0
                    StrSQL.Length = 0
                    If veg_Cods.Count = 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT DISTINCT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryEntitaVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        'StrSQL.Append(" GROUP BY " & strGroupBy & " ")
                    ElseIf veg_Cods.Count = 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT DISTINCT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryEntitaId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        'StrSQL.Append(" GROUP BY " & strGroupBy & " ")
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT DISTINCT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryEntitaVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        'StrSQL.Append(" GROUP BY " & strGroupBy & " ")
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT DISTINCT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryEntitaVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append("UNION " & vbCrLf)
                        StrSQL.Append(creaQueryEntitaId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        'StrSQL.Append(" GROUP BY " & strGroupBy & " ")
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

    Public Function LeggiEntitaImpresa_Da_Particella_GerarchiaImpresa(ByVal ImpresePadri As List(Of String),
                                                                      ByVal Piva As String,
                                                                      ByVal Sa_Cod As Int32,
                                                                      ByVal validita_Inizio As DateTime,
                                                                      ByVal validita_fine As DateTime,
                                                                      ByVal veg_Cods As List(Of String),
                                                                      ByVal id_cods As List(Of String),
                                                                      ByVal programmazione_Cod As Integer,
                                                                      ByVal programmazione_entita_Cod As Integer,
                                                                      ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                                      ByVal xFiltroAggiuntivo As String,
                                                                      ByVal xOrderBy As String,
                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                      ByVal filtroUltima As Boolean,
                                                                      ByVal filtroEnti As Boolean,
                                                                      ByRef dtCuaa As DataTable
                                                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaImpresa_Da_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine("	WITH Programmazione_testata_cte as (  ")

                    StrSQL.AppendLine("		SELECT	Piva,  ")
                    If filtroUltima Then
                        StrSQL.AppendLine("				Max(programmazione_Cod) as Programmazione_Cod  ")
                    Else
                        StrSQL.AppendLine("				Programmazione_Cod  ")
                    End If

                    StrSQL.AppendLine("		FROM Programmazione_Testata  ")
                    StrSQL.AppendLine("		WHERE Validita_Inizio >=  " & Agro_SQL_SaveDate(validita_Inizio) & "     ")
                    StrSQL.AppendLine("			AND Validita_Fine <=  " & Agro_SQL_SaveDate(validita_fine) & "   ")
                    If filtroEnti Then
                        StrSQL.AppendLine("AND Programmazione_Testata.fonte_cod=0 ")
                    End If
                    If filtroUltima Then
                        StrSQL.AppendLine("		GROUP BY Piva ")
                    End If
                    StrSQL.AppendLine("	), ")
                    StrSQL.AppendLine("	gerarchiaImprese_ct as ( ")
                    StrSQL.AppendLine("		SELECT STRING_AGG(GerarchiaImprese.Padre, ',') as Piva_padre, STRING_AGG(Imprese.rag_soc, ',') as rag_soc_Padre, GerarchiaImprese.Figlio ")
                    StrSQL.AppendLine("		FROM GerarchiaImprese ")
                    StrSQL.AppendLine("		JOIN Imprese ON GerarchiaImprese.Padre = Imprese.PIVA ")
                    StrSQL.AppendLine("		GROUP BY GerarchiaImprese.Figlio ")
                    StrSQL.AppendLine("	), ")
                    StrSQL.AppendLine("	ixc_cte as ( ")
                    StrSQL.AppendLine("		SELECT Piva, prov, com, sezione, foglio, numero, subalterno, max(ID) as ID FROM ImpreseXParticelle    ")
                    StrSQL.AppendLine("		WHERE ImpreseXParticelle.Validita_Inizio <=  " & Agro_SQL_SaveDate(validita_Inizio) & "         ")
                    StrSQL.AppendLine("	    AND ImpreseXParticelle.Validita_Fine >=  " & Agro_SQL_SaveDate(validita_fine) & "        ")
                    StrSQL.AppendLine("		GROUP BY Piva, prov, com, sezione, foglio, numero, subalterno ")
                    StrSQL.AppendLine("	),  ")
                    StrSQL.AppendLine("	pcm_cte as ( ")
                    StrSQL.AppendLine("		SELECT prov, com, sezione, foglio, numero, subalterno, macrouso_cod, MAX(ID)  as ID  ")
                    StrSQL.AppendLine("		FROM ParticelleCatastalixMacrousi    ")
                    StrSQL.AppendLine("		GROUP BY prov, com, sezione, foglio, numero, subalterno, macrouso_cod ")
                    StrSQL.AppendLine("	), ")
                    StrSQL.AppendLine("	Allegati_EntitaxDocumenti_cte as ( ")
                    StrSQL.AppendLine("		SELECT max(Allegati_EntitaxDocumenti.Allegati_Documenti_Cod) as Allegati_Documenti_Cod, Allegati_EntitaxDocumenti.Programmazione_Cod  ")
                    StrSQL.AppendLine("		FROM Allegati_EntitaxDocumenti  ")
                    StrSQL.AppendLine("		JOIN Programmazione_testata_cte ON Allegati_EntitaxDocumenti.Programmazione_Cod = Programmazione_testata_cte.Programmazione_Cod ")
                    StrSQL.AppendLine("		WHERE Programmazione_Entita_Cod = 0 ")
                    StrSQL.AppendLine("		GROUP BY Allegati_EntitaxDocumenti.Programmazione_Cod ")
                    StrSQL.AppendLine("	), ")
                    StrSQL.AppendLine("   NumberedContacts AS (")
                    StrSQL.AppendLine("       SELECT Piva, Sa_Cod, Cod_Contatto, ISNULL(CONCAT(Rag_Soc, ' ', Nome, ' ', Cognome), '') AS Tecnico_Riferimento_Nome,")
                    StrSQL.AppendLine("       ROW_NUMBER() OVER (PARTITION BY Cod_Contatto ORDER BY Sa_Cod ASC) AS ContactNumber")
                    StrSQL.AppendLine("       From Contatti")
                    StrSQL.AppendLine("   ),")
                    StrSQL.AppendLine("   FirstEncounters AS (")
                    StrSQL.AppendLine("       Select Piva, Sa_Cod, Cod_Contatto, Tecnico_Riferimento_Nome")
                    StrSQL.AppendLine("       FROM NumberedContacts")
                    StrSQL.AppendLine("       WHERE ContactNumber = 1")
                    StrSQL.AppendLine("   )")
                    StrSQL.AppendLine("	SELECT  ")
                    StrSQL.AppendLine("		pe.Programmazione_Entita_Cod, ")
                    StrSQL.AppendLine("			ic.val_cod AS cuaa, ")
                    StrSQL.AppendLine("			i.rag_soc, ")
                    StrSQL.AppendLine("			pt.Programmazione_Des_Long, ")
                    StrSQL.AppendLine("			i.Piva, ")
                    StrSQL.AppendLine("         Case When ISNULL(i.partitaIvaReale, '') = '' THEN i.PIVA ELSE i.partitaIvaReale END AS partitaIvaReale,")
                    StrSQL.AppendLine("			COALESCE(Gruppi_Raccolta.GruppoRaccolta_Cod, 0) As GruppoRaccolta_Cod, ")
                    StrSQL.AppendLine("			COALESCE(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS GruppoRaccolta_Des, ")
                    StrSQL.AppendLine("			pp.Com AS Com, ")
                    StrSQL.AppendLine("			comune.LOCALITA AS LOCALITA, ")
                    StrSQL.AppendLine("			pp.Prov AS Prov, ")
                    StrSQL.AppendLine("			comune.COMUNI_PROV AS COMUNI_PROV, ")
                    StrSQL.AppendLine("			pp.Foglio AS Foglio, ")
                    StrSQL.AppendLine("			pp.Numero AS Numero, ")
                    StrSQL.AppendLine("			pp.Sezione AS Sezione, ")
                    StrSQL.AppendLine("			pp.Subalterno AS Subalterno, ")
                    StrSQL.AppendLine("			CAST(pc.Ettari + (cast(pc.Are AS real)/100) + (cast(pc.centiare AS real)/10000) AS real) AS Superficie_Catastale,         ")
                    StrSQL.AppendLine("			ip.Sup_Condotta AS Sup_Condotta, ")
                    StrSQL.AppendLine("			pe.Superficie AS SupUtilizzo, ")
                    StrSQL.AppendLine("			pp.Superficie AS SupUtilizzo1, ")
                    StrSQL.AppendLine("			Case WHEN pcm.Superficie IS NULL THEN 0 ELSE pcm.Superficie End AS SupMacrouso,   ")
                    StrSQL.AppendLine("			pe.Macrouso_Cod, ")
                    StrSQL.AppendLine("			m.Macrouso_Des,    ")
                    StrSQL.AppendLine("			pe.Veg_Cod, ")
                    StrSQL.AppendLine("			CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,          ")
                    StrSQL.AppendLine("			pe.Cul_Cod, ")
                    StrSQL.AppendLine("			c.Cul_Des, ")
                    StrSQL.AppendLine("			pe.veg_cod_Cliente, ")
                    StrSQL.AppendLine("			pe.cul_cod_Cliente, ")
                    StrSQL.AppendLine("			gerarchiaImprese_ct.Piva_padre AS Piva_Padre, ")
                    StrSQL.AppendLine("			gerarchiaImprese_ct.rag_soc_Padre AS rag_soc_Padre, ")
                    StrSQL.AppendLine("			icc.val_cod AS codiceSocio, ")
                    StrSQL.AppendLine("			pe.Validita_Inizio, ")
                    StrSQL.AppendLine("			pe.Validita_Fine, ")
                    StrSQL.AppendLine("			pe.num_Piante, ")
                    StrSQL.AppendLine("			cop.Cop_Cod, ")
                    StrSQL.AppendLine("			cop.Cop_des, ")
                    StrSQL.AppendLine("			pe.MetodoProduzione_Cod AS [MetodoProduzione_Cod],   ")
                    StrSQL.AppendLine("			CASE WHEN pe.MetodoProduzione_Cod = 1 THEN 'Integrato'   ")
                    StrSQL.AppendLine("	                    WHEN pe.MetodoProduzione_Cod = 2 THEN 'In Conversione'   ")
                    StrSQL.AppendLine("	                    WHEN pe.MetodoProduzione_Cod = 3 THEN 'Biologico'   ")
                    StrSQL.AppendLine("			END AS [MetodoProduzione_Des], ")
                    StrSQL.AppendLine("			pe.Campo_Cod, ")
                    StrSQL.AppendLine("			pe.Grva_Cod, ")
                    StrSQL.AppendLine("			grva.Grva_Des, ")
                    StrSQL.AppendLine("			ip.Validita_Inizio AS Conduzione_Inizio, ")
                    StrSQL.AppendLine("			ip.Validita_Fine AS Conduzione_Fine, ")
                    StrSQL.AppendLine("			ad.Validazione_Data, ")
                    StrSQL.AppendLine("			ad.Allegati_Documenti_Numero, ")
                    StrSQL.AppendLine("			ip.TitoloPossesso, ")
                    StrSQL.AppendLine("			pc.Proprietario, ")
                    StrSQL.AppendLine("			CASE ip.TitoloPossesso  ")
                    StrSQL.AppendLine("	     	    WHEN 0 THEN 'Altro' ")
                    StrSQL.AppendLine("	     	    WHEN 1 THEN 'Proprietà' ")
                    StrSQL.AppendLine("	     	    WHEN 2 THEN 'Comodato d''uso' ")
                    StrSQL.AppendLine("	     	    WHEN 3 THEN 'Affitto con contratto' ")
                    StrSQL.AppendLine("	     	    WHEN 4 THEN 'Affitto senza contratto' ")
                    StrSQL.AppendLine("	     	ELSE 'Altro' END AS TitoloPossesso_Des, ")
                    StrSQL.AppendLine("			pe.Data_Modifica, ")
                    StrSQL.AppendLine("			COALESCE(Utenti.[user], pe.Username_Modifica) as Username_Modifica, ")
                    StrSQL.AppendLine("			COALESCE(Materie_Prime.Mat_Cod, 0) as Mat_Cod, ")
                    StrSQL.AppendLine("			COALESCE(Materie_Prime.Cod_Articolo + ' - ' + Materie_Prime.Mat_Des, '') as Mat_Des, ")
                    StrSQL.AppendLine("			FE.Tecnico_Riferimento_Nome  ")
                    StrSQL.AppendLine("	FROM Programmazione_Entita pe ")
                    StrSQL.AppendLine("	JOIN Programmazione_testata_cte pt1 ON pe.Programmazione_Cod = pt1.Programmazione_Cod ")
                    StrSQL.AppendLine("	JOIN Programmazione_Testata pt ON pt1.Programmazione_Cod = pt.Programmazione_Cod ")
                    StrSQL.AppendLine("	JOIN gerarchiaImprese_ct ON pt.Piva = gerarchiaImprese_ct.Figlio ")
                    StrSQL.AppendLine("	JOIN Imprese i ON pt.Piva = i.PIVA ")
                    StrSQL.AppendLine("	LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod ")
                    StrSQL.AppendLine("	LEFT JOIN GruppoVarietale grva ON pe.Grva_Cod = grva.Grva_Cod  ")
                    StrSQL.AppendLine("	LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno     ")
                    StrSQL.AppendLine("	LEFT JOIN ixc_cte ON ixc_cte.Piva = pe.Piva AND pc.Prov = ixc_cte.PROV AND pc.Com = ixc_cte.Com AND pc.SEZIONE = ixc_cte.SEZIONE AND pc.FOGLIO = ixc_cte.FOGLIO AND pc.NUMERO = ixc_cte.NUMERO AND pc.SUBALTERNO = ixc_cte.SUBALTERNO ")
                    StrSQL.AppendLine("	LEFT JOIN ImpreseXParticelle ip ON ixc_cte.ID = ip.ID ")
                    StrSQL.AppendLine("	LEFT JOIN pcm_cte ON pp.prov = pcm_cte.prov  ")
                    StrSQL.AppendLine("						AND pp.com = pcm_cte.com  ")
                    StrSQL.AppendLine("						AND pp.foglio = pcm_cte.foglio  ")
                    StrSQL.AppendLine("						AND pp.sezione = pcm_cte.sezione  ")
                    StrSQL.AppendLine("						AND pp.numero = pcm_cte.numero  ")
                    StrSQL.AppendLine("						AND pp.subalterno = pcm_cte.subalterno  ")
                    StrSQL.AppendLine("						AND pe.Macrouso_Cod = pcm_cte.macrouso_Cod  ")
                    StrSQL.AppendLine("	LEFT JOIN ParticelleCatastalixMacrousi pcm ON  pcm_cte.ID = pcm.ID ")
                    StrSQL.AppendLine("	LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010    ")
                    StrSQL.AppendLine("	LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033    ")
                    StrSQL.AppendLine("	LEFT JOIN Imprese_Codici iccc ON i.piva = iccc.piva AND iccc.id_Cod = 1088    ")
                    StrSQL.AppendLine("	LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov   ")
                    StrSQL.AppendLine("	LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod   ")
                    StrSQL.AppendLine("	LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod   ")
                    StrSQL.AppendLine("	LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0    ")
                    StrSQL.AppendLine("	LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod  ")
                    StrSQL.AppendLine("	LEFT JOIN Allegati_EntitaxDocumenti_cte aed ON aed.Programmazione_Cod=pt.Programmazione_Cod ")
                    StrSQL.AppendLine("	LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod  ")
                    StrSQL.AppendLine("	LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0  ")
                    StrSQL.AppendLine("	LEFT JOIN Gruppi_Raccolta ON i.GruppoRaccolta_Cod = Gruppi_Raccolta.GruppoRaccolta_Cod  ")
                    StrSQL.AppendLine("	LEFT JOIN Materie_Prime ON pe.Mat_Cod = Materie_Prime.Mat_Cod  ")
                    StrSQL.AppendLine("	LEFT JOIN Utenti ON pe.Username_Modifica = Utenti.CODICE_FISCALE  ")
                    StrSQL.AppendLine("	LEFT JOIN FirstEncounters FE ON iccc.val_cod = FE.Cod_Contatto  ")
                    StrSQL.AppendLine("	WHERE 1 = 1  ")
                    StrSQL.AppendLine("	AND pt.Tipo_Pianificazione IN (0,2)  ")
                    If Piva <> "" Then
                        StrSQL.AppendLine("AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " ")
                    End If
                    If validita_Inizio <> AGRODATAINIZIO Then
                        StrSQL.AppendLine("AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " ")
                    End If
                    If validita_fine <> AGRODATAFINE Then
                        StrSQL.AppendLine("AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine("AND pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If ImpresePadri.Count <> 0 Then
                        If ImpresePadri.Count = 1 Then
                            Dim impresa = ImpresePadri(0)
                            If impresa <> "-1" Then
                                StrSQL.AppendLine("AND gerarchiaImprese_ct.Piva_Padre like '%" & Agro_SQL_SaveText(impresa) & "%' ")
                            Else
                                StrSQL.AppendLine("AND gerarchiaImprese_ct.Piva_Padre <> '' ")
                            End If
                        Else
                            StrSQL.AppendLine("AND gerarchiaImprese_ct.Piva_Padre IN ( ")
                            Dim first = True
                            For Each pPadre In ImpresePadri
                                If pPadre <> "" Then
                                    If Not first Then
                                        StrSQL.AppendLine(",")
                                    Else
                                        first = False
                                    End If
                                    StrSQL.AppendLine(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                                End If
                            Next
                            StrSQL.AppendLine(") ")
                        End If
                    End If
                    If veg_Cods.Count <> 0 Then
                        If veg_Cods.Count = 1 Then
                            Dim veg_cod = veg_Cods(0)
                            If veg_cod <> "0" AndAlso veg_cod <> "" AndAlso veg_cod <> " " Then
                                StrSQL.AppendLine("AND pe.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
                            End If
                        Else
                            StrSQL.AppendLine("AND pe.veg_cod IN ( ")
                            Dim first = True
                            For Each veg_cod In veg_Cods
                                If veg_cod <> "" Then
                                    If Not first Then
                                        StrSQL.AppendLine(",")
                                    Else
                                        first = False
                                    End If
                                    StrSQL.AppendLine(" " & Agro_SQL_SaveNum(veg_cod) & " ")
                                End If
                            Next
                            StrSQL.AppendLine(") ")
                        End If
                    End If
                    If id_cods.Count <> 0 Then
                        If id_cods.Count = 1 Then
                            Dim id_cod = id_cods(0)
                            If id_cod <> "0" AndAlso id_cod <> "" AndAlso id_cod <> " " Then
                                StrSQL.AppendLine(" AND pe.id_cod = " & Agro_SQL_SaveNum(id_cod) & " ")
                            End If
                        Else
                            StrSQL.AppendLine(" AND pe.id_cod IN ( ")
                            Dim first = True
                            For Each id_cod In id_cods
                                If id_cod <> "" Then
                                    If Not first Then
                                        StrSQL.AppendLine(",")
                                    Else
                                        first = False
                                    End If
                                    StrSQL.AppendLine(" " & Agro_SQL_SaveNum(id_cod) & " ")
                                End If
                            Next
                            StrSQL.AppendLine(" ) ")
                        End If
                    End If
                    If dtCuaa IsNot Nothing AndAlso dtCuaa.Rows.Count > 0 Then
                        StrSQL.AppendLine("AND ic.val_cod IN ( ")
                        For i = 0 To dtCuaa.Rows.Count - 1
                            Dim cuaa As String = dtCuaa.Rows(i)("CUAA")
                            cuaa = cuaa.Trim
                            cuaa = cuaa.Replace("'", "")
                            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(cuaa) & "'")
                            If i <> dtCuaa.Rows.Count - 1 Then
                                StrSQL.AppendLine(",")
                            End If
                        Next
                        StrSQL.AppendLine(" ) ")
                    End If

                    If programmazione_Cod <> 0 Then
                        StrSQL.AppendLine("AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " ")
                    End If
                    If programmazione_entita_Cod <> 0 Then
                        StrSQL.AppendLine("AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " ")
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

    Public Function LeggiEntitaEstrazioneParticellaCatasto(ByVal validita_Inizio As DateTime, ByVal validita_fine As DateTime, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaEstrazioneParticellaCatasto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        Dim DT As DataTable

        Try

            StrSQL.AppendLine("WITH #Programmazione_Testata as (")
            StrSQL.AppendLine("SELECT * FROM Programmazione_Testata WHERE PRogrammazione_Cod IN (")
            StrSQL.AppendLine("SELECT MAX(Programmazione_Cod)")
            StrSQL.AppendLine("FROM Programmazione_Testata")
            StrSQL.AppendLine("WHERE Validita_Inizio >= " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & " ")
            StrSQL.AppendLine("AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " ")
            StrSQL.AppendLine("GROUP BY Piva")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("), #ImpresexParticelle as (")
            StrSQL.AppendLine("SELECT * FROM ImpreseXParticelle WHERE ID IN (")
            StrSQL.AppendLine("SELECT MAX(id)")
            StrSQL.AppendLine("FROM ImpreseXParticelle")
            StrSQL.AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " ")
            StrSQL.AppendLine("AND Validita_Fine >= " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "  GROUP BY Piva, Sa_Cod, Prov, com, sezione, foglio, numero, SUBALTERNO")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("SELECT Imprese_Codici.val_cod As CUAA,")
            StrSQL.AppendLine("ISTAT.COMUNI_PROV as PROV,")
            StrSQL.AppendLine("ISTAT.LOCALITA as comune,")
            StrSQL.AppendLine("ISTAT.CAP,")
            StrSQL.AppendLine("#ImpresexParticelle.PROV as [istat_provincia],")
            StrSQL.AppendLine("#ImpresexParticelle.COM as [istat_comune],")
            StrSQL.AppendLine("#ImpresexParticelle.SEZIONE,")
            StrSQL.AppendLine("#ImpresexParticelle.FOGLIO,")
            StrSQL.AppendLine("#ImpresexParticelle.NUMERO as particella,")
            StrSQL.AppendLine("#ImpresexParticelle.SUBALTERNO,")
            StrSQL.AppendLine("ParticelleCatastali.ETTARI as [particella_ha],")
            StrSQL.AppendLine("case")
            StrSQL.AppendLine("WHEN ParticelleCatastali.ARE = 0 Then '00'")
            StrSQL.AppendLine("When ParticelleCatastali.ARE < 10 and ParticelleCatastali.ARE != 0 Then '0' + convert(varchar(1),ParticelleCatastali.ARE)")
            StrSQL.AppendLine("else convert(varchar(2),ParticelleCatastali.ARE) END as [particella_are],")
            StrSQL.AppendLine("case")
            StrSQL.AppendLine("WHEN ParticelleCatastali.CENTIARE = 0 Then '00'")
            StrSQL.AppendLine("When ParticelleCatastali.CENTIARE < 10 and ParticelleCatastali.CENTIARE != 0 Then '0' + convert(varchar(1),ParticelleCatastali.CENTIARE)")
            StrSQL.AppendLine("else convert(varchar(2),ParticelleCatastali.CENTIARE) END as [particella_centiare],")
            StrSQL.AppendLine("FLOOR(SUM(Programmazione_Entita.Superficie)) as [Semina_Trapianto_ha],")
            StrSQL.AppendLine("case")
            StrSQL.AppendLine("WHEN Len(substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 1, 2)) = 0")
            StrSQL.AppendLine("THEN '00'")
            StrSQL.AppendLine("WHEN Len(substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 1, 2)) = 1")
            StrSQL.AppendLine("THEN substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 1, 2) + '0'")
            StrSQL.AppendLine("ELSE substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 1, 2)")
            StrSQL.AppendLine("END")
            StrSQL.AppendLine("as [Semina_Trapianto_are],")
            StrSQL.AppendLine("case")
            StrSQL.AppendLine("WHEN Len(substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 3, 2)) = 0")
            StrSQL.AppendLine("THEN '00'")
            StrSQL.AppendLine("WHEN Len(substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 3, 2)) = 1")
            StrSQL.AppendLine("THEN substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 3, 2) + '0'")
            StrSQL.AppendLine("ELSE substring(convert(varchar(20), SUM(Programmazione_Entita.Superficie)), charindex('.', convert(varchar(20) , SUM(Programmazione_Entita.Superficie)))+ 3, 2)")
            StrSQL.AppendLine("END as [Semina_Trapianto_centiare]")
            StrSQL.AppendLine("FROM Imprese")
            StrSQL.AppendLine("INNER JOIN Imprese_Codici ON Imprese.Piva = Imprese_Codici.Piva AND Imprese_Codici.id_cod = 1010")
            StrSQL.AppendLine("INNER JOIN #ImpresexParticelle ON #ImpresexParticelle.Piva = Imprese.Piva")
            StrSQL.AppendLine("INNER JOIN ISTAT ON #ImpresexParticelle.PROV = ISTAT.PROV AND #ImpresexParticelle.COM = ISTAT.COM")
            StrSQL.AppendLine("INNER JOIN ParticelleCatastali ON #ImpresexParticelle.PROV = ParticelleCatastali.PROV")
            StrSQL.AppendLine("AND #ImpresexParticelle.COM = ParticelleCatastali.COM")
            StrSQL.AppendLine("AND #ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE")
            StrSQL.AppendLine("AND #ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO")
            StrSQL.AppendLine("AND #ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO")
            StrSQL.AppendLine("AND #ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO")
            StrSQL.AppendLine("INNER JOIN Programmazione_Particelle ON #ImpresexParticelle.PROV = Programmazione_Particelle.PROV")
            StrSQL.AppendLine("AND #ImpresexParticelle.COM = Programmazione_Particelle.COM")
            StrSQL.AppendLine("AND #ImpresexParticelle.SEZIONE = Programmazione_Particelle.Sezione")
            StrSQL.AppendLine("AND #ImpresexParticelle.FOGLIO = Programmazione_Particelle.Foglio")
            StrSQL.AppendLine("AND #ImpresexParticelle.NUMERO = Programmazione_Particelle.Numero")
            StrSQL.AppendLine("AND #ImpresexParticelle.SUBALTERNO = Programmazione_Particelle.Subalterno")
            StrSQL.AppendLine("INNER JOIN Programmazione_Entita ON Imprese.Piva = Programmazione_Entita.Piva AND Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod")
            StrSQL.AppendLine("INNER JOIN #Programmazione_Testata ON Imprese.Piva = #Programmazione_Testata.Piva AND Programmazione_Entita.Programmazione_Cod = #Programmazione_Testata.Programmazione_Cod")
            StrSQL.AppendLine("WHERE 1 = 1")
            StrSQL.AppendLine("AND Programmazione_Entita.Veg_Cod = 52")
            StrSQL.AppendLine("GROUP BY Imprese_Codici.val_cod ,")
            StrSQL.AppendLine("ISTAT.COMUNI_PROV ,")
            StrSQL.AppendLine("ISTAT.LOCALITA ,")
            StrSQL.AppendLine("ISTAT.CAP,")
            StrSQL.AppendLine("#ImpresexParticelle.PROV ,")
            StrSQL.AppendLine("#ImpresexParticelle.COM,")
            StrSQL.AppendLine("#ImpresexParticelle.SEZIONE,")
            StrSQL.AppendLine("#ImpresexParticelle.FOGLIO,")
            StrSQL.AppendLine("#ImpresexParticelle.NUMERO,")
            StrSQL.AppendLine("#ImpresexParticelle.SUBALTERNO,")
            StrSQL.AppendLine("ParticelleCatastali.ETTARI ,")
            StrSQL.AppendLine("ParticelleCatastali.ARE ,")
            StrSQL.AppendLine("ParticelleCatastali.CENTIARE")

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



    Private Function creaQueryEntitaVeg_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                          ByVal filtroUltima As Boolean,
                                                          ByVal filtroEnti As Boolean,
                                                          dtCuaa As DataTable) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.AppendLine("SELECT DISTINCT ")
        StrSQL.AppendLine("		pe.Programmazione_Entita_Cod,")
        StrSQL.AppendLine("		ic.val_cod AS cuaa,")
        StrSQL.AppendLine("		i.rag_soc,")
        StrSQL.AppendLine("		pt.Programmazione_Des_Long,")
        StrSQL.AppendLine("		i.Piva,")

        StrSQL.AppendLine("		ISNULL(pp.Com, ppq.com) AS Com,")
        StrSQL.AppendLine("		ISNULL(comune.LOCALITA, comune_ppq.LOCALITA) AS LOCALITA,")
        StrSQL.AppendLine("		ISNULL(pp.Prov, ppq.Prov) AS Prov,")
        StrSQL.AppendLine("		ISNULL(comune.COMUNI_PROV, comune_ppq.COMUNI_PROV) AS COMUNI_PROV,")
        StrSQL.AppendLine("		ISNULL(pp.Foglio, ppq.Foglio) AS Foglio,")
        StrSQL.AppendLine("		ISNULL(pp.Numero, ppq.Numero) AS Numero,")
        StrSQL.AppendLine("		ISNULL(pp.Sezione, ppq.Sezione) AS Sezione,")
        StrSQL.AppendLine("		ISNULL(pp.Subalterno, ppq.Subalterno) AS Subalterno,")
        StrSQL.AppendLine("		ISNULL(CAST(pc.Ettari + (cast(pc.Are AS real)/100) + (cast(pc.centiare AS real)/10000) AS real), CAST(pc_ppq.Ettari + (cast(pc_ppq.Are AS real)/100) + (cast(pc_ppq.centiare AS real)/10000) AS real)) AS Superficie_Catastale,        ")
        StrSQL.AppendLine("		ISNULL(ip.Sup_Condotta, ip_ppq.Sup_Condotta) AS Sup_Condotta,")
        StrSQL.AppendLine("		pe.Superficie AS SupUtilizzo,")
        StrSQL.AppendLine("		ISNULL(pp.Superficie, ppq.Superficie) AS SupUtilizzo1,")

        ' StrSQL.appendline("     CASE WHEN peq.Superficie IS NOT NULL THEN peq.Superficie WHEN pp.Superficie IS NOT NULL THEN pp.Superficie ELSE pe.superficie END AS SupUtilizzo,    "  )
        StrSQL.AppendLine("		Case WHEN pcm.Superficie IS NULL THEN 0 ELSE pcm.Superficie End AS SupMacrouso,  ")
        StrSQL.AppendLine("		pe.Macrouso_Cod,")
        StrSQL.AppendLine("		m.Macrouso_Des,   ")
        StrSQL.AppendLine("		pe.Veg_Cod,")
        StrSQL.AppendLine("		CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,         ")
        StrSQL.AppendLine("		pe.Cul_Cod,")
        StrSQL.AppendLine("		c.Cul_Des,")
        StrSQL.AppendLine("		pe.veg_cod_Cliente,")
        StrSQL.AppendLine("		pe.cul_cod_Cliente,")
        StrSQL.AppendLine("		padre.PIVA AS Piva_Padre,")
        StrSQL.AppendLine("		padre.rag_soc AS rag_soc_Padre,")
        StrSQL.AppendLine("		icc.val_cod AS codiceSocio,")
        StrSQL.AppendLine("		pe.Validita_Inizio,")
        StrSQL.AppendLine("		pe.Validita_Fine,")
        StrSQL.AppendLine("		pe.num_Piante,")
        StrSQL.AppendLine("		cop.Cop_Cod,")
        StrSQL.AppendLine("		cop.Cop_des,")
        StrSQL.AppendLine("		CASE WHEN peq.MetodoProduzione_Cod IS NOT NULL THEN peq.MetodoProduzione_Cod ELSE pe.MetodoProduzione_Cod END AS [MetodoProduzione_Cod],  ")
        StrSQL.AppendLine("		CASE WHEN peq.MetodoProduzione_Cod IS Not null  ")
        StrSQL.AppendLine("          THEN  ")
        StrSQL.AppendLine("              CASE WHEN peq.MetodoProduzione_Cod = 1 THEN 'Integrato'  ")
        StrSQL.AppendLine("                   WHEN peq.MetodoProduzione_Cod = 2 THEN 'In Conversione'  ")
        StrSQL.AppendLine("                   WHEN peq.MetodoProduzione_Cod = 3 THEN 'Biologico'  ")
        StrSQL.AppendLine("              END ")
        StrSQL.AppendLine("          ELSE CASE WHEN pe.MetodoProduzione_Cod = 1 THEN 'Integrato'  ")
        StrSQL.AppendLine("                    WHEN pe.MetodoProduzione_Cod = 2 THEN 'In Conversione'  ")
        StrSQL.AppendLine("                    WHEN pe.MetodoProduzione_Cod = 3 THEN 'Biologico'  ")
        StrSQL.AppendLine("		END END AS [MetodoProduzione_Des],")
        StrSQL.AppendLine("		pe.Campo_Cod,")
        StrSQL.AppendLine("		pe.Grva_Cod,")
        StrSQL.AppendLine("		grva.Grva_Des,")
        StrSQL.AppendLine("		ip.Validita_Inizio AS Conduzione_Inizio,")
        StrSQL.AppendLine("		ip.Validita_Fine AS Conduzione_Fine,")
        StrSQL.AppendLine("		ad.Validazione_Data,")
        StrSQL.AppendLine("		ad.Allegati_Documenti_Numero,")
        StrSQL.AppendLine("		ip.TitoloPossesso,")
        StrSQL.AppendLine("		CASE ip.TitoloPossesso ")
        StrSQL.AppendLine("     	    WHEN 0 THEN 'Altro'")
        StrSQL.AppendLine("     	    WHEN 1 THEN 'Proprietà'")
        StrSQL.AppendLine("     	    WHEN 2 THEN 'Comodato d''uso'")
        StrSQL.AppendLine("     	    WHEN 3 THEN 'Affitto con contratto'")
        StrSQL.AppendLine("     	    WHEN 4 THEN 'Affitto senza contratto'")
        StrSQL.AppendLine("     	ELSE 'Altro' END AS TitoloPossesso_Des")
        StrSQL.AppendLine("FROM Imprese I   ")
        StrSQL.AppendLine("LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio   ")
        StrSQL.AppendLine("LEFT JOIN Imprese padre ON gi.padre = padre.piva ")
        StrSQL.AppendLine("LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva   ")
        If filtroUltima Then
            StrSQL.AppendLine("AND Pt.programmazione_Cod IN ( ")
            StrSQL.AppendLine("                                 SELECT Max(programmazione_Cod) FROM Programmazione_Testata ")
            StrSQL.AppendLine("                                 WHERE Piva = pt.piva ")
            StrSQL.AppendLine("                                 AND Validita_Inizio >= " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   ")
            StrSQL.AppendLine("                                 AND Validita_Fine <= " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " ")
            If Not filtroEnti Then
                StrSQL.AppendLine("                                 AND fonte_cod=0 ")
            Else
                StrSQL.AppendLine("                                 AND fonte_cod <> 0 ")
            End If
            StrSQL.AppendLine("                                 ) ")
        End If
        StrSQL.AppendLine("LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   ")
        StrSQL.AppendLine("LEFT JOIN Programmazione_Entita peq ON pe.Programmazione_Cod = peq.Programmazione_Cod AND pe.Campo_Cod = peq.Campo_Cod AND peq.Appezza = 0   ")
        StrSQL.AppendLine("LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    ")
        StrSQL.AppendLine("LEFT JOIN GruppoVarietale grva ON pe.Grva_Cod = grva.Grva_Cod ")
        StrSQL.AppendLine("LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno    ")
        StrSQL.AppendLine("LEFT JOIN ImpresexParticelle ip ON ip.ID = (   ")
        StrSQL.AppendLine("                                             SELECT max(ID) FROM ImpreseXParticelle   ")
        StrSQL.AppendLine("                                             WHERE ImpreseXParticelle.Piva = i.piva    ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.prov = pc.prov   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.com = pc.com   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.sezione = pc.sezione   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.foglio = pc.foglio   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.numero = pc.numero   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.subalterno = pc.subalterno   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_Inizio) & "       ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_fine) & "     ")
        StrSQL.AppendLine("                                             )   ")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN ParticelleCatastalixMacrousi pcm ON pcm.ID = (SELECT MAX(ID)    ")
        StrSQL.AppendLine("                                                        FROM ParticelleCatastalixMacrousi   ")
        StrSQL.AppendLine("                                                        WHERE pp.prov = ParticelleCatastalixMacrousi.prov AND    ")
        StrSQL.AppendLine("                                                        pp.com = ParticelleCatastalixMacrousi.com AND    ")
        StrSQL.AppendLine("                                                        pp.foglio = ParticelleCatastalixMacrousi.foglio AND   ")
        StrSQL.AppendLine("                                                        pp.sezione = ParticelleCatastalixMacrousi.sezione AND    ")
        StrSQL.AppendLine("                                                        pp.numero = ParticelleCatastalixMacrousi.numero AND    ")
        StrSQL.AppendLine("                                                        pp.subalterno = ParticelleCatastalixMacrousi.subalterno AND    ")
        StrSQL.AppendLine("                                                        pe.Macrouso_Cod = ParticelleCatastalixMacrousi.macrouso_Cod     ")
        StrSQL.AppendLine("                                                        )")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   ")
        StrSQL.AppendLine("LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033   ")
        StrSQL.AppendLine("LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov  ")
        StrSQL.AppendLine("LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod  ")
        StrSQL.AppendLine("LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod  ")
        StrSQL.AppendLine("LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0   ")
        StrSQL.AppendLine("LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod ")
        StrSQL.AppendLine("LEFT JOIN Allegati_EntitaxDocumenti aed ON aed.Programmazione_Cod=pt.Programmazione_Cod AND aed.Programmazione_Entita_Cod=0 ")
        StrSQL.AppendLine("LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod ")
        StrSQL.AppendLine("LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0 ")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN Programmazione_Particelle ppq ON peq.Programmazione_Entita_Cod = ppq.Programmazione_Entita_Cod")
        StrSQL.AppendLine("LEFT JOIN ISTAT comune_ppq ON ppq.Com = comune_ppq.COM AND ppq.Prov = comune_ppq.prov")
        StrSQL.AppendLine("LEFT JOIN ParticelleCatastali pc_ppq ON pc_ppq.prov = ppq.prov ")
        StrSQL.AppendLine("     AND pc_ppq.com = ppq.com ")
        StrSQL.AppendLine("     AND pc_ppq.foglio = ppq.foglio")
        StrSQL.AppendLine("     AND pc_ppq.sezione = ppq.sezione")
        StrSQL.AppendLine("     AND pc_ppq.numero = ppq.numero ")
        StrSQL.AppendLine("     AND pc_ppq.subalterno = ppq.subalterno")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN ImpresexParticelle ip_ppq ON ip_ppq.ID = (")
        StrSQL.AppendLine("                                                     SELECT max(ID)")
        StrSQL.AppendLine("                                                     FROM ImpreseXParticelle ")
        StrSQL.AppendLine("                                                     WHERE ImpreseXParticelle.Piva = i.piva")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.prov = pc_ppq.prov")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.com = pc_ppq.com   ")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.sezione = pc_ppq.sezione")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.foglio = pc_ppq.foglio ")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.numero = pc_ppq.numero")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.subalterno = pc_ppq.subalterno")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.Validita_Inizio <=  CONVERT(DateTime,'2022/01/01',120)   ")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.Validita_Fine >=  CONVERT(DateTime,'2022/12/31',120) ")
        StrSQL.AppendLine("                                                     )")

        StrSQL.AppendLine("")

        'StrSQL.appendline(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva ON csva.id_cod = pe.id_cod  "  )
        StrSQL.AppendLine("WHERE 1=1 ")
        'StrSQL.appendline(" --AND pp.Com IS NOT NULL "  )
        StrSQL.AppendLine("AND pt.programmazione_Cod IS NOT NULL ")
        StrSQL.AppendLine("AND pt.Tipo_Pianificazione IN (0,2) ")
        StrSQL.AppendLine("AND pe.Appezza <> 0 ")
        If programmazione_Cod <> 0 Then
            StrSQL.AppendLine("AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " ")
        End If
        If programmazione_entita_Cod <> 0 Then
            StrSQL.AppendLine("AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " ")
        End If
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.AppendLine("AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " ")
                Else
                    StrSQL.AppendLine("AND gi.Padre <> '' ")
                End If
            Else
                StrSQL.AppendLine("AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.AppendLine(",")
                        Else
                            first = False
                        End If
                        StrSQL.AppendLine(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.AppendLine(") ")
            End If
        End If
        If veg_Cods.Count <> 0 Then
            If veg_Cods.Count = 1 Then
                Dim veg_cod = veg_Cods(0)
                If veg_cod <> "0" AndAlso veg_cod <> "" AndAlso veg_cod <> " " Then
                    StrSQL.AppendLine("AND pe.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
                End If
            Else
                StrSQL.AppendLine("AND pe.veg_cod IN ( ")
                Dim first = True
                For Each veg_cod In veg_Cods
                    If veg_cod <> "" Then
                        If Not first Then
                            StrSQL.AppendLine(",")
                        Else
                            first = False
                        End If
                        StrSQL.AppendLine(" " & Agro_SQL_SaveNum(veg_cod) & " ")
                    End If
                Next
                StrSQL.AppendLine(") ")
            End If
        End If
        'If livello <> 0 THEN
        '    StrSQL.appendline(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " "  )
        'End If
        If Piva <> "" Then
            StrSQL.AppendLine("AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " ")
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.AppendLine("AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " ")
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.AppendLine("AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.AppendLine("AND pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.AppendLine("AND pt.fonte_cod=0 ")
        End If
        If dtCuaa IsNot Nothing AndAlso dtCuaa.Rows.Count > 0 Then
            StrSQL.AppendLine("AND ic.val_cod IN ( ")
            For i = 0 To dtCuaa.Rows.Count - 1
                Dim cuaa As String = dtCuaa.Rows(i)("CUAA")
                cuaa = cuaa.Trim
                cuaa = cuaa.Replace("'", "")
                StrSQL.AppendLine(" '" & Agro_SQL_SaveText(cuaa) & "'")
                If i <> dtCuaa.Rows.Count - 1 Then
                    StrSQL.AppendLine(",")
                End If
            Next
            StrSQL.AppendLine(" ) ")
        End If

        Return StrSQL.ToString
    End Function

    Private Function creaQueryEntitaId_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal id_cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                          ByVal filtroUltima As Boolean,
                                                          ByVal filtroEnti As Boolean,
                                                          dtCuaa As DataTable) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.AppendLine("SELECT DISTINCT ")
        StrSQL.AppendLine("		pe.Programmazione_Entita_Cod,")
        StrSQL.AppendLine("		ic.val_cod AS cuaa,")
        StrSQL.AppendLine("		i.rag_soc,")
        StrSQL.AppendLine("		pt.Programmazione_Des_Long,")
        StrSQL.AppendLine("		i.Piva,")

        StrSQL.AppendLine("		ISNULL(pp.Com, ppq.com) AS Com,")
        StrSQL.AppendLine("		ISNULL(comune.LOCALITA, comune_ppq.LOCALITA) AS LOCALITA,")
        StrSQL.AppendLine("		ISNULL(pp.Prov, ppq.Prov) AS Prov,")
        StrSQL.AppendLine("		ISNULL(comune.COMUNI_PROV, comune_ppq.COMUNI_PROV) AS COMUNI_PROV,")
        StrSQL.AppendLine("		ISNULL(pp.Foglio, ppq.Foglio) AS Foglio,")
        StrSQL.AppendLine("		ISNULL(pp.Numero, ppq.Numero) AS Numero,")
        StrSQL.AppendLine("		ISNULL(pp.Sezione, ppq.Sezione) AS Sezione,")
        StrSQL.AppendLine("		ISNULL(pp.Subalterno, ppq.Subalterno) AS Subalterno,")
        StrSQL.AppendLine("		ISNULL(CAST(pc.Ettari + (CAST(pc.Are AS REAL)/100) + (CAST(pc.centiare AS REAL)/10000) AS REAL), CAST(pc_ppq.Ettari + (CAST(pc_ppq.Are AS REAL)/100) + (CAST(pc_ppq.centiare AS REAL)/10000) AS REAL)) AS Superficie_Catastale,        ")
        StrSQL.AppendLine("		ISNULL(ip.Sup_Condotta, ip_ppq.Sup_Condotta) AS Sup_Condotta,")
        StrSQL.AppendLine("		pe.Superficie AS SupUtilizzo,")
        StrSQL.AppendLine("		ISNULL(pp.Superficie, ppq.Superficie) AS SupUtilizzo1,")

        'StrSQL.appendline("     CASE WHEN peq.Superficie IS NOT NULL THEN peq.Superficie WHEN pp.Superficie IS NOT NULL THEN pp.Superficie ELSE pe.superficie END AS SupUtilizzo,    " )
        StrSQL.AppendLine("		Case WHEN pcm.Superficie is null Then 0 ELSE pcm.Superficie END AS SupMacrouso,")
        StrSQL.AppendLine("		pe.Macrouso_Cod,")
        StrSQL.AppendLine("		m.Macrouso_Des,")
        StrSQL.AppendLine("		pe.Veg_Cod,")
        StrSQL.AppendLine("		CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,")
        StrSQL.AppendLine("		pe.Cul_Cod,")
        StrSQL.AppendLine("		c.Cul_Des,")
        StrSQL.AppendLine("		pe.veg_cod_Cliente,")
        StrSQL.AppendLine("		pe.cul_cod_Cliente,")
        StrSQL.AppendLine("		padre.PIVA AS PIVA_Padre,")
        StrSQL.AppendLine("		padre.rag_soc AS Rag_soc_Padre,")
        StrSQL.AppendLine("		icc.val_cod AS codiceSocio,")
        StrSQL.AppendLine("		pe.Validita_Inizio,")
        StrSQL.AppendLine("		pe.Validita_Fine,")
        StrSQL.AppendLine("		pe.num_Piante,")
        StrSQL.AppendLine("		cop.Cop_Cod,")
        StrSQL.AppendLine("		cop.Cop_des,")
        StrSQL.AppendLine("		CASE WHEN peq.MetodoProduzione_Cod IS NOT NULL THEN peq.MetodoProduzione_Cod ELSE pe.MetodoProduzione_Cod END AS [MetodoProduzione_Cod],  ")
        StrSQL.AppendLine("		CASE WHEN peq.MetodoProduzione_Cod IS NOT NULL  ")
        StrSQL.AppendLine("          THEN  ")
        StrSQL.AppendLine("              CASE WHEN peq.MetodoProduzione_Cod = 1 THEN 'Integrato'  ")
        StrSQL.AppendLine("                   WHEN peq.MetodoProduzione_Cod = 2 THEN 'In Conversione'  ")
        StrSQL.AppendLine("                   WHEN peq.MetodoProduzione_Cod = 3 THEN 'Biologico'  ")
        StrSQL.AppendLine("              END ")
        StrSQL.AppendLine("          ELSE CASE WHEN pe.MetodoProduzione_Cod = 1 Then 'Integrato'  ")
        StrSQL.AppendLine("                    WHEN pe.MetodoProduzione_Cod = 2 THEN 'In Conversione'  ")
        StrSQL.AppendLine("                    WHEN pe.MetodoProduzione_Cod = 3 THEN 'Biologico'  ")
        StrSQL.AppendLine("		END END AS [MetodoProduzione_Des],")
        StrSQL.AppendLine("		pe.Campo_Cod,")
        StrSQL.AppendLine("		pe.Grva_Cod,")
        StrSQL.AppendLine("		grva.Grva_Des,")
        StrSQL.AppendLine("		ip.Validita_Inizio AS Conduzione_Inizio,")
        StrSQL.AppendLine("		ip.Validita_Fine AS Conduzione_Fine,")
        StrSQL.AppendLine("		ad.Validazione_Data,")
        StrSQL.AppendLine("		ad.Allegati_Documenti_Numero,")
        StrSQL.AppendLine("		ip.TitoloPossesso,")
        StrSQL.AppendLine("		CASE ip.TitoloPossesso ")
        StrSQL.AppendLine("     	    WHEN 0 THEN 'Altro'")
        StrSQL.AppendLine("     	    WHEN 1 THEN 'Proprietà'")
        StrSQL.AppendLine("     	    WHEN 2 THEN 'Comodato d''uso'")
        StrSQL.AppendLine("     	    WHEN 3 THEN 'Affitto con contratto'")
        StrSQL.AppendLine("     	    WHEN 4 THEN 'Affitto senza contratto'")
        StrSQL.AppendLine("     	ELSE 'Altro' END AS TitoloPossesso_Des")
        StrSQL.AppendLine("FROM Imprese I   ")
        StrSQL.AppendLine("LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio   ")
        StrSQL.AppendLine("LEFT JOIN Imprese padre ON gi.padre = padre.piva ")
        StrSQL.AppendLine("LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva   ")
        If filtroUltima Then
            StrSQL.AppendLine("AND Pt.programmazione_Cod IN ( ")
            StrSQL.AppendLine("                                 SELECT Max(programmazione_Cod) FROM Programmazione_Testata ")
            StrSQL.AppendLine("                                 WHERE Piva = pt.piva ")
            StrSQL.AppendLine("                                 AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   ")
            StrSQL.AppendLine("                                 AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " ")
            If Not filtroEnti Then
                StrSQL.AppendLine("                                 AND fonte_cod=0 ")
            End If
            StrSQL.AppendLine("                                 ) ")
        End If
        StrSQL.AppendLine("LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   ")
        StrSQL.AppendLine("LEFT JOIN Programmazione_Entita peq ON pe.Programmazione_Cod = peq.Programmazione_Cod AND pe.Campo_Cod = peq.Campo_Cod AND peq.Appezza = 0   ")
        StrSQL.AppendLine("LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    ")
        StrSQL.AppendLine("LEFT JOIN GruppoVarietale grva ON pe.Grva_Cod = grva.Grva_Cod ")
        StrSQL.AppendLine("LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno    ")
        StrSQL.AppendLine("LEFT JOIN ImpresexParticelle ip ON ip.ID = (   ")
        StrSQL.AppendLine("                                             SELECT MAX(ID) FROM ImpreseXParticelle   ")
        StrSQL.AppendLine("                                             WHERE ImpreseXParticelle.Piva = i.piva    ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.prov = pc.prov   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.com = pc.com   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.sezione = pc.sezione   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.foglio = pc.foglio   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.numero = pc.numero   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.subalterno = pc.subalterno   ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_Inizio) & "       ")
        StrSQL.AppendLine("                                             AND ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(validita_fine) & "     ")
        StrSQL.AppendLine("                                             )   ")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN ParticelleCatastalixMacrousi pcm ON pcm.ID = (SELECT MAX(ID)    ")
        StrSQL.AppendLine("													     FROM ParticelleCatastalixMacrousi   ")
        StrSQL.AppendLine("													     WHERE pp.prov = ParticelleCatastalixMacrousi.prov AND    ")
        StrSQL.AppendLine("													     pp.com = ParticelleCatastalixMacrousi.com AND    ")
        StrSQL.AppendLine("													     pp.foglio = ParticelleCatastalixMacrousi.foglio AND   ")
        StrSQL.AppendLine("													     pp.sezione = ParticelleCatastalixMacrousi.sezione AND    ")
        StrSQL.AppendLine("													     pp.numero = ParticelleCatastalixMacrousi.numero AND    ")
        StrSQL.AppendLine("													     pp.subalterno = ParticelleCatastalixMacrousi.subalterno AND    ")
        StrSQL.AppendLine("													     pe.Macrouso_Cod = ParticelleCatastalixMacrousi.macrouso_Cod     ")
        StrSQL.AppendLine("													     )   ")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   ")
        StrSQL.AppendLine("LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033   ")
        StrSQL.AppendLine("LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov  ")
        StrSQL.AppendLine("LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod  ")
        StrSQL.AppendLine("LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod  ")
        StrSQL.AppendLine("LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0   ")
        StrSQL.AppendLine("LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod ")
        StrSQL.AppendLine("LEFT JOIN Allegati_EntitaxDocumenti aed ON aed.Programmazione_Cod=pt.Programmazione_Cod AND aed.Programmazione_Entita_Cod=0 ")
        StrSQL.AppendLine("LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod ")
        StrSQL.AppendLine("LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0 ")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN Programmazione_Particelle ppq ON peq.Programmazione_Entita_Cod = ppq.Programmazione_Entita_Cod")
        StrSQL.AppendLine("LEFT JOIN ISTAT comune_ppq ON ppq.Com = comune_ppq.COM AND ppq.Prov = comune_ppq.prov")
        StrSQL.AppendLine("LEFT JOIN ParticelleCatastali pc_ppq ON pc_ppq.prov = ppq.prov ")
        StrSQL.AppendLine("     AND pc_ppq.com = ppq.com ")
        StrSQL.AppendLine("     AND pc_ppq.foglio = ppq.foglio")
        StrSQL.AppendLine("     AND pc_ppq.sezione = ppq.sezione")
        StrSQL.AppendLine("     AND pc_ppq.numero = ppq.numero ")
        StrSQL.AppendLine("     AND pc_ppq.subalterno = ppq.subalterno")

        StrSQL.AppendLine("")

        StrSQL.AppendLine("LEFT JOIN ImpresexParticelle ip_ppq ON ip_ppq.ID = (")
        StrSQL.AppendLine("                                                     SELECT max(ID)")
        StrSQL.AppendLine("                                                     FROM ImpreseXParticelle ")
        StrSQL.AppendLine("                                                     WHERE ImpreseXParticelle.Piva = i.piva")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.prov = pc_ppq.prov")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.com = pc_ppq.com   ")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.sezione = pc_ppq.sezione")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.foglio = pc_ppq.foglio ")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.numero = pc_ppq.numero")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.subalterno = pc_ppq.subalterno")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.Validita_Inizio <=  CONVERT(DateTime,'2022/01/01',120)   ")
        StrSQL.AppendLine("                                                     AND ImpreseXParticelle.Validita_Fine >=  CONVERT(DateTime,'2022/12/31',120) ")
        StrSQL.AppendLine("                                                     )")

        StrSQL.AppendLine("")

        'StrSQL.appendline(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva ON csva.id_cod = pe.id_cod  " )
        StrSQL.AppendLine("WHERE 1=1 ")
        StrSQL.AppendLine("--AND pp.Com IS NOT NULL ")
        StrSQL.AppendLine("AND pt.programmazione_Cod IS NOT NULL ")
        StrSQL.AppendLine("AND pt.Tipo_Pianificazione IN (0,2) ")
        StrSQL.AppendLine("AND pe.Appezza <> 0 ")
        If programmazione_Cod <> 0 Then
            StrSQL.AppendLine("AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " ")
        End If
        If programmazione_entita_Cod <> 0 Then
            StrSQL.AppendLine("AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " ")
        End If
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.AppendLine("AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " ")
                Else
                    StrSQL.AppendLine("AND gi.Padre <> '' ")
                End If
            Else
                StrSQL.AppendLine("AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.AppendLine(",")
                        Else
                            first = False
                        End If
                        StrSQL.AppendLine(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.AppendLine(" ) ")
            End If
        End If
        If id_cods.Count <> 0 Then
            If id_cods.Count = 1 Then
                Dim id_cod = id_cods(0)
                If id_cod <> "0" AndAlso id_cod <> "" AndAlso id_cod <> " " Then
                    StrSQL.AppendLine(" AND pe.id_cod = " & Agro_SQL_SaveNum(id_cod) & " ")
                End If
            Else
                StrSQL.AppendLine(" AND pe.id_cod IN ( ")
                Dim first = True
                For Each id_cod In id_cods
                    If id_cod <> "" Then
                        If Not first Then
                            StrSQL.AppendLine(",")
                        Else
                            first = False
                        End If
                        StrSQL.AppendLine(" " & Agro_SQL_SaveNum(id_cod) & " ")
                    End If
                Next
                StrSQL.AppendLine(" ) ")
            End If
        End If
        'If livello <> 0 Then
        '    StrSQL.appendline(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " )
        'END If
        If Piva <> "" Then
            StrSQL.AppendLine(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " ")
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.AppendLine(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " ")
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.AppendLine(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.AppendLine(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.AppendLine("    AND pt.fonte_cod=0 ")
        End If
        If dtCuaa IsNot Nothing AndAlso dtCuaa.Rows.Count > 0 Then
            StrSQL.AppendLine(" AND   ic.val_cod IN ( ")
            For i = 0 To dtCuaa.Rows.Count - 1
                Dim cuaa As String = dtCuaa.Rows(i)("CUAA")
                cuaa = cuaa.Trim
                cuaa = cuaa.Replace("'", "")
                StrSQL.AppendLine(" '" & Agro_SQL_SaveText(cuaa) & "'")
                If i <> dtCuaa.Rows.Count - 1 Then
                    StrSQL.AppendLine(",")
                End If
            Next
            StrSQL.AppendLine(" ) ")
        End If

        Return StrSQL.ToString
    End Function

    Public Function Leggi_ModAss(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal id_cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByVal filtroUltima As Boolean,
                                                             ByVal filtroEnti As Boolean
                                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Leggi_ModAss()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    If veg_Cods.Count = 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryModAssVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Programmazione_Cod, t.PIVA, t.veg_cod, t.utilizzo,t.SupUtilizzo, t.Veg_Cod_Cliente, t.Piva_Padre, t.rag_soc_Padre, t.codiceSocio, t.Fonte_cod ")
                    ElseIf veg_Cods.Count = 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryModAssId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Programmazione_Cod, t.PIVA, t.veg_cod, t.utilizzo,t.SupUtilizzo, t.Veg_Cod_Cliente, t.Piva_Padre, t.rag_soc_Padre, t.codiceSocio, t.Fonte_cod ")
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryModAssVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Programmazione_Cod, t.PIVA, t.veg_cod, t.utilizzo,t.SupUtilizzo, t.Veg_Cod_Cliente, t.Piva_Padre, t.rag_soc_Padre, t.codiceSocio, t.Fonte_cod ")
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryModAssVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append("UNION " & vbCrLf)
                        StrSQL.Append(creaQueryModAssId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Programmazione_Cod, t.PIVA, t.veg_cod, t.utilizzo,t.SupUtilizzo, t.Veg_Cod_Cliente, t.Piva_Padre, t.rag_soc_Padre, t.codiceSocio, t.Fonte_cod ")
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

    Public Function Leggi_CCDP(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal id_cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByVal filtroUltima As Boolean,
                                                             ByVal filtroEnti As Boolean
                                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaImpresa_Da_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    If veg_Cods.Count = 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryCCDPVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY  t.cuaa, t.rag_soc, t.PIVA,t.Com,  t.Prov,  t.foglio, t.Numero, t.Sezione, t.Subalterno,  t.SupUtilizzo, t.veg_cod, t.utilizzo, t.Validazione_Data, t.Fonte_Cod ")
                    ElseIf veg_Cods.Count = 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryCCDPId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY  t.cuaa, t.rag_soc, t.PIVA,t.Com,  t.Prov,  t.foglio, t.Numero, t.Sezione, t.Subalterno,  t.SupUtilizzo, t.veg_cod, t.utilizzo, t.Validazione_Data, t.Fonte_Cod ")
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryCCDPVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY  t.cuaa, t.rag_soc, t.PIVA,t.Com,  t.Prov,  t.foglio, t.Numero, t.Sezione, t.Subalterno,  t.SupUtilizzo, t.veg_cod, t.utilizzo, t.Validazione_Data, t.Fonte_Cod ")
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryCCDPVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append("UNION " & vbCrLf)
                        StrSQL.Append(creaQueryCCDPId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, programmazione_Cod, programmazione_entita_Cod, filtroUltima, filtroEnti))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" GROUP BY  t.cuaa, t.rag_soc, t.PIVA,t.Com,  t.Prov,  t.foglio, t.Numero, t.Sezione, t.Subalterno,  t.SupUtilizzo, t.veg_cod, t.utilizzo, t.Validazione_Data, t.Fonte_Cod ")
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

    Private Function creaQueryModAssVeg_Cod(ByVal ImpresePadri As List(Of String),
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Int32,
                                            ByVal validita_Inizio As DateTime,
                                            ByVal validita_fine As DateTime,
                                            ByVal veg_Cods As List(Of String),
                                            ByVal programmazione_Cod As Integer,
                                            ByVal programmazione_entita_Cod As Integer,
                                            ByVal filtroUltima As Boolean,
                                            ByVal filtroEnti As Boolean
                                            ) As String

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.Append("SELECT  " & vbCrLf)
        StrSQL.Append("      ic.val_cod as cuaa,    " & vbCrLf)
        StrSQL.Append("      i.rag_soc,   " & vbCrLf)
        StrSQL.Append("      pt.Programmazione_Des_Long,    " & vbCrLf)
        StrSQL.Append("      pt.programmazione_Cod,    " & vbCrLf)
        StrSQL.Append("      i.Piva,    " & vbCrLf)
        StrSQL.Append("      Case WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.PIVA ELSE I.partitaIvaReale END AS PivaReale, " & vbCrLf)
        StrSQL.Append("      SUM(pe.Superficie) as SupUtilizzo,    " & vbCrLf)
        StrSQL.Append("      pe.Veg_Cod,    " & vbCrLf)
        StrSQL.Append("      CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,     " & vbCrLf)
        StrSQL.Append("      pe.veg_cod_Cliente,    " & vbCrLf)
        StrSQL.Append("     padre.PIVA as Piva_Padre, " & vbCrLf)
        StrSQL.Append("     padre.rag_soc as rag_soc_Padre, " & vbCrLf)
        StrSQL.Append("     icc.val_cod as codiceSocio, " & vbCrLf)
        StrSQL.Append("     pt.Fonte_cod " & vbCrLf)
        StrSQL.Append("  FROM Imprese I   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese padre ON gi.padre = padre.piva " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva   " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" AND Pt.programmazione_Cod IN ( " & vbCrLf)
            StrSQL.Append("    SELECT Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
            StrSQL.Append("    AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
            If Not filtroEnti Then
                StrSQL.Append("    AND fonte_cod=0 " & vbCrLf)
            End If
            StrSQL.Append(" ) " & vbCrLf)
        End If
        StrSQL.Append("  LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ImpresexParticelle ip ON ip.prov = pp.prov AND ip.com = pp.com AND ip.foglio = pp.foglio AND ip.sezione = pp.sezione AND ip.numero = pp.numero AND ip.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastalixMacrousi pcm ON pp.prov = pcm.prov AND pp.com = pcm.com AND pp.foglio = pcm.foglio AND pp.sezione = pcm.sezione AND pp.numero = pcm.numero AND pp.subalterno = pcm.subalterno AND pe.Macrouso_Cod = pcm.macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0 ")
        StrSQL.Append("  LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod ")
        StrSQL.Append("  LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0 ")
        'StrSQL.Append(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva ON csva.id_cod = pe.id_cod  " & vbCrLf)
        StrSQL.Append(" WHERE 1=1 ")
        'StrSQL.Append(" --AND pp.Com is not null " & vbCrLf)
        StrSQL.Append(" AND pt.programmazione_Cod is not null " & vbCrLf)
        StrSQL.Append(" AND pt.Tipo_Pianificazione in (0,2) " & vbCrLf)
        If programmazione_Cod <> 0 Then
            StrSQL.Append(" AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " " & vbCrLf)
        End If
        If programmazione_entita_Cod <> 0 Then
            StrSQL.Append(" AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " " & vbCrLf)
        End If
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                Else
                    StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        If veg_Cods.Count <> 0 Then
            If veg_Cods.Count = 1 Then
                Dim veg_cod = veg_Cods(0)
                If veg_cod <> "0" AndAlso veg_cod <> "" AndAlso veg_cod <> " " Then
                    StrSQL.Append(" AND pe.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND pe.veg_cod IN ( ")
                Dim first = True
                For Each veg_cod In veg_Cods
                    If veg_cod <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveNum(veg_cod) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        'If livello <> 0 Then
        '    StrSQL.Append(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " & vbCrLf)
        'End If
        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.Append("    AND pt.fonte_cod=0 " & vbCrLf)
        End If
        StrSQL.Append(" GROUP BY ic.val_cod , i.rag_soc,pt.Programmazione_Des_Long, pt.programmazione_Cod,i.Piva,pe.Veg_Cod,sv.Veg_Des, ca.descrizione, pe.veg_cod_Cliente, padre.PIVA ,padre.rag_soc,icc.val_cod ,pe.Validita_Inizio,pe.Validita_Fine, pt.Fonte_cod " & vbCrLf)
        Return StrSQL.ToString
    End Function

    Private Function creaQueryModAssId_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal id_cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                          ByVal filtroUltima As Boolean,
                                                          ByVal filtroEnti As Boolean) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Append("SELECT  " & vbCrLf)
        StrSQL.Append("      ic.val_cod as cuaa,    " & vbCrLf)
        StrSQL.Append("      i.rag_soc,   " & vbCrLf)
        StrSQL.Append("      Case WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.PIVA ELSE I.partitaIvaReale END AS PivaReale, " & vbCrLf)
        StrSQL.Append("      pt.Programmazione_Des_Long,    " & vbCrLf)
        StrSQL.Append("      pt.programmazione_Cod,    " & vbCrLf)
        StrSQL.Append("      i.Piva,    " & vbCrLf)
        StrSQL.Append("      SUM(pe.Superficie) as SupUtilizzo,    " & vbCrLf)
        StrSQL.Append("      pe.Veg_Cod,    " & vbCrLf)
        StrSQL.Append("      CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,     " & vbCrLf)
        StrSQL.Append("      pe.veg_cod_Cliente,    " & vbCrLf)
        StrSQL.Append("     padre.PIVA as Piva_Padre, " & vbCrLf)
        StrSQL.Append("     padre.rag_soc as rag_soc_Padre, " & vbCrLf)
        StrSQL.Append("     icc.val_cod as codiceSocio, " & vbCrLf)
        StrSQL.Append("     pt.Fonte_cod " & vbCrLf)
        StrSQL.Append("  FROM Imprese I   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese padre ON gi.padre = padre.piva " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva   " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" AND Pt.programmazione_Cod IN ( " & vbCrLf)
            StrSQL.Append("    SELECT Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
            StrSQL.Append("    AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
            If Not filtroEnti Then
                StrSQL.Append("    AND fonte_cod=0 " & vbCrLf)
            End If
            StrSQL.Append(" ) " & vbCrLf)
        End If
        StrSQL.Append("  LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ImpresexParticelle ip ON ip.prov = pp.prov AND ip.com = pp.com AND ip.foglio = pp.foglio AND ip.sezione = pp.sezione AND ip.numero = pp.numero AND ip.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastalixMacrousi pcm ON pp.prov = pcm.prov AND pp.com = pcm.com AND pp.foglio = pcm.foglio AND pp.sezione = pcm.sezione AND pp.numero = pcm.numero AND pp.subalterno = pcm.subalterno AND pe.Macrouso_Cod = pcm.macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0  ")
        StrSQL.Append("  LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod ")
        StrSQL.Append("  LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0 ")
        'StrSQL.Append(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva ON csva.id_cod = pe.id_cod  " & vbCrLf)
        StrSQL.Append(" WHERE 1=1 ")
        StrSQL.Append(" --AND pp.Com is not null " & vbCrLf)
        StrSQL.Append(" AND pt.programmazione_Cod is not null " & vbCrLf)
        StrSQL.Append(" AND pt.Tipo_Pianificazione in (0,2) " & vbCrLf)
        If programmazione_Cod <> 0 Then
            StrSQL.Append(" AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " " & vbCrLf)
        End If
        If programmazione_entita_Cod <> 0 Then
            StrSQL.Append(" AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " " & vbCrLf)
        End If
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                Else
                    StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        If id_cods.Count <> 0 Then
            If id_cods.Count = 1 Then
                Dim id_cod = id_cods(0)
                If id_cod <> "0" AndAlso id_cod <> "" AndAlso id_cod <> " " Then
                    StrSQL.Append(" AND pe.id_cod = " & Agro_SQL_SaveNum(id_cod) & " " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND pe.id_cod IN ( ")
                Dim first = True
                For Each id_cod In id_cods
                    If id_cod <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveNum(id_cod) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        'If livello <> 0 Then
        '    StrSQL.Append(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " & vbCrLf)
        'End If
        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.Append("    AND pt.fonte_cod=0 " & vbCrLf)
        End If
        StrSQL.Append(" GROUP BY ic.val_cod , i.rag_soc,pt.Programmazione_Des_Long, pt.programmazione_Cod,i.Piva,pe.Veg_Cod,sv.Veg_Des, ca.descrizione, pe.veg_cod_Cliente, padre.PIVA ,padre.rag_soc,icc.val_cod ,pe.Validita_Inizio,pe.Validita_Fine, pt.Fonte_cod " & vbCrLf)
        Return StrSQL.ToString
    End Function

    Private Function creaQueryCCDPVeg_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                          ByVal filtroUltima As Boolean,
                                                          ByVal filtroEnti As Boolean) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.Append("SELECT  " & vbCrLf)
        StrSQL.Append("      ic.val_cod as cuaa,    " & vbCrLf)
        StrSQL.Append("      i.rag_soc,   " & vbCrLf)
        StrSQL.Append("      i.Piva,    " & vbCrLf)
        StrSQL.Append("      Case WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.PIVA ELSE I.partitaIvaReale END AS PivaReale, " & vbCrLf)
        StrSQL.Append("      pp.Com,    " & vbCrLf)
        StrSQL.Append("      pp.Prov,    " & vbCrLf)
        StrSQL.Append("      pp.Foglio,    " & vbCrLf)
        StrSQL.Append("      pp.Numero,    " & vbCrLf)
        StrSQL.Append("      pp.Sezione,    " & vbCrLf)
        StrSQL.Append("      pp.Subalterno,    " & vbCrLf)
        StrSQL.Append("      SUM(pe.Superficie) as SupUtilizzo,    " & vbCrLf)
        StrSQL.Append("      pe.Veg_Cod,    " & vbCrLf)
        StrSQL.Append("      CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,     " & vbCrLf)
        StrSQL.Append("      ad.Validazione_Data, " & vbCrLf)
        StrSQL.Append("      pt.Fonte_Cod " & vbCrLf)
        StrSQL.Append("  FROM Imprese I   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese padre ON gi.padre = padre.piva " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva   " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" AND Pt.programmazione_Cod IN ( " & vbCrLf)
            StrSQL.Append("    SELECT Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
            StrSQL.Append("    AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
            If Not filtroEnti Then
                StrSQL.Append("    AND fonte_cod=0 " & vbCrLf)
            End If
            StrSQL.Append(" ) " & vbCrLf)
        End If
        StrSQL.Append("  LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ImpresexParticelle ip ON ip.prov = pp.prov AND ip.com = pp.com AND ip.foglio = pp.foglio AND ip.sezione = pp.sezione AND ip.numero = pp.numero AND ip.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastali pc ON pc.prov = pp.prov AND pc.com = pp.com AND pc.foglio = pp.foglio AND pc.sezione = pp.sezione AND pc.numero = pp.numero AND pc.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastalixMacrousi pcm ON pp.prov = pcm.prov AND pp.com = pcm.com AND pp.foglio = pcm.foglio AND pp.sezione = pcm.sezione AND pp.numero = pcm.numero AND pp.subalterno = pcm.subalterno AND pe.Macrouso_Cod = pcm.macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1033   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Copertura cop ON pe.cop_Cod = cop.Cop_Cod " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0   " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_EntitaxDocumenti aed ON pt.Programmazione_Cod = aed.programmazione_Cod AND aed.Programmazione_Entita_Cod = 0 " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod ")
        'StrSQL.Append(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva ON csva.id_cod = pe.id_cod  " & vbCrLf)
        StrSQL.Append(" WHERE 1=1 ")
        'StrSQL.Append(" --AND pp.Com is not null " & vbCrLf)
        StrSQL.Append(" AND pt.programmazione_Cod is not null " & vbCrLf)
        StrSQL.Append(" AND pt.Tipo_Pianificazione in (0,2) " & vbCrLf)
        If programmazione_Cod <> 0 Then
            StrSQL.Append(" AND pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " " & vbCrLf)
        End If
        If programmazione_entita_Cod <> 0 Then
            StrSQL.Append(" AND pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " " & vbCrLf)
        End If
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                Else
                    StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        If veg_Cods.Count <> 0 Then
            If veg_Cods.Count = 1 Then
                Dim veg_cod = veg_Cods(0)
                If veg_cod <> "0" AndAlso veg_cod <> "" AndAlso veg_cod <> " " Then
                    StrSQL.Append(" AND pe.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND pe.veg_cod IN ( ")
                Dim first = True
                For Each veg_cod In veg_Cods
                    If veg_cod <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveNum(veg_cod) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        'If livello <> 0 Then
        '    StrSQL.Append(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " & vbCrLf)
        'End If
        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.Append("    AND pt.fonte_cod=0 " & vbCrLf)
        End If
        StrSQL.Append(" GROUP BY ic.val_cod, i.rag_soc, i.piva, pp.com, pp.prov, pp.foglio, pp.numero, pp.sezione, pp.subalterno, pe.veg_Cod, sv.veg_Des, ca.Descrizione, ad.validazione_data, pt.Fonte_cod " & vbCrLf)
        Return StrSQL.ToString
    End Function
    Private Function creaQueryCCDPId_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal id_cods As List(Of String),
                                                          ByVal programmazione_Cod As Integer,
                                                          ByVal programmazione_entita_Cod As Integer,
                                                          ByVal filtroUltima As Boolean,
                                                          ByVal filtroEnti As Boolean) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Append("SELECT  " & vbCrLf)
        StrSQL.Append("      ic.val_cod as cuaa,    " & vbCrLf)
        StrSQL.Append("      i.rag_soc,   " & vbCrLf)
        StrSQL.Append("      i.Piva,    " & vbCrLf)
        StrSQL.Append("      Case WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.PIVA ELSE I.partitaIvaReale END AS PivaReale, " & vbCrLf)
        StrSQL.Append("      pp.Com,    " & vbCrLf)
        StrSQL.Append("      pp.Prov,    " & vbCrLf)
        StrSQL.Append("      pp.Foglio,    " & vbCrLf)
        StrSQL.Append("      pp.Numero,    " & vbCrLf)
        StrSQL.Append("      pp.Sezione,    " & vbCrLf)
        StrSQL.Append("      pp.Subalterno,    " & vbCrLf)
        StrSQL.Append("      SUM(pe.Superficie) as SupUtilizzo,    " & vbCrLf)
        StrSQL.Append("      pe.Veg_Cod,    " & vbCrLf)
        StrSQL.Append("      CASE WHEN pe.Veg_Cod <> 0 THEN sv.Veg_Des ELSE ca.descrizione END AS Utilizzo,     " & vbCrLf)
        StrSQL.Append("      ad.Validazione_Data, " & vbCrLf)
        StrSQL.Append("      pt.Fonte_Cod " & vbCrLf)
        StrSQL.Append("  FROM Imprese I   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN GerarchiaImprese gi On I.PIVA = gi.Figlio   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese padre On gi.padre = padre.piva " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Testata pt On I.PIVA = pt.Piva   " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" And Pt.programmazione_Cod In ( " & vbCrLf)
            StrSQL.Append("    Select Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    And Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
            StrSQL.Append("    And Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
            If Not filtroEnti Then
                StrSQL.Append("    And fonte_cod=0 " & vbCrLf)
            End If
            StrSQL.Append(" ) " & vbCrLf)
        End If
        StrSQL.Append("  LEFT JOIN Programmazione_Entita pe On pe.Programmazione_Cod = pt.Programmazione_Cod   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Programmazione_Particelle pp On pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ImpresexParticelle ip On ip.prov = pp.prov And ip.com = pp.com And ip.foglio = pp.foglio And ip.sezione = pp.sezione And ip.numero = pp.numero And ip.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastali pc On pc.prov = pp.prov And pc.com = pp.com And pc.foglio = pp.foglio And pc.sezione = pp.sezione And pc.numero = pp.numero And pc.subalterno = pp.subalterno    " & vbCrLf)
        'StrSQL.Append("  LEFT JOIN ParticelleCatastalixMacrousi pcm On pp.prov = pcm.prov And pp.com = pcm.com And pp.foglio = pcm.foglio And pp.sezione = pcm.sezione And pp.numero = pcm.numero And pp.subalterno = pcm.subalterno And pe.Macrouso_Cod = pcm.macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici ic On i.piva = ic.piva And ic.id_Cod = 1010   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Imprese_Codici icc On i.piva = icc.piva And icc.id_Cod = 1033   " & vbCrLf)
        StrSQL.Append("  LEFT JOIN ISTAT comune On pp.Com = comune.COM And pp.Prov = comune.prov  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Macrousi m On pe.Macrouso_Cod = m.Macrouso_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Cultivar c On c.Cul_Cod = pe.Cul_Cod  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod AND pe.Veg_Cod <> 0  " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Copertura cop On pe.cop_Cod = cop.Cop_Cod " & vbCrLf)
        StrSQL.Append("  LEFT JOIN Codici_Anagrafe CA ON ca.codice = pe.Id_Cod AND pe.Id_Cod <> 0   " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_EntitaxDocumenti aed On pt.Programmazione_Cod = aed.programmazione_Cod And aed.Programmazione_Entita_Cod = 0 " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_Documenti ad On aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod " & vbCrLf)
        'StrSQL.Append(" LEFT JOIN Codifica_SpecieVegetali_Agrea csva On csva.id_cod = pe.id_cod  " & vbCrLf)
        StrSQL.Append(" WHERE 1=1 ")
        StrSQL.Append(" --And pp.Com Is Not null " & vbCrLf)
        StrSQL.Append(" And pt.programmazione_Cod Is Not null " & vbCrLf)
        StrSQL.Append(" And pt.Tipo_Pianificazione In (0,2) " & vbCrLf)
        If programmazione_Cod <> 0 Then
            StrSQL.Append(" And pt.Programmazione_Cod=" & Agro_SQL_SaveNum(programmazione_Cod) & " " & vbCrLf)
        End If
        If programmazione_entita_Cod <> 0 Then
            StrSQL.Append(" And pe.Programmazione_Entita_Cod=" & Agro_SQL_SaveNum(programmazione_entita_Cod) & " " & vbCrLf)
        End If
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" And gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                Else
                    StrSQL.Append(" And gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        If id_cods.Count <> 0 Then
            If id_cods.Count = 1 Then
                Dim id_cod = id_cods(0)
                If id_cod <> "0" AndAlso id_cod <> "" AndAlso id_cod <> " " Then
                    StrSQL.Append(" AND pe.id_cod = " & Agro_SQL_SaveNum(id_cod) & " " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND pe.id_cod IN ( ")
                Dim first = True
                For Each id_cod In id_cods
                    If id_cod <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveNum(id_cod) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        'If livello <> 0 Then
        '    StrSQL.Append(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " & vbCrLf)
        'End If
        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.Append("    AND pt.fonte_cod=0 " & vbCrLf)
        End If
        StrSQL.Append(" GROUP BY ic.val_cod, i.rag_soc, i.piva, pp.com, pp.prov, pp.foglio, pp.numero, pp.sezione, pp.subalterno, pe.veg_Cod, sv.veg_Des, ca.Descrizione, ad.validazione_data, pt.Fonte_cod " & vbCrLf)
        Return StrSQL.ToString
    End Function


    Public Function LeggiTestata_Da_GerarchiaImpresaOld(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal id_cods As List(Of String),
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef filtroUltima As Boolean,
                                                             ByRef filtroEnti As Boolean,
                                                     ByRef dtCuaa As DataTable) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaImpresa_Da_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    If veg_Cods.Count = 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryTestataVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" Group by t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Piva, t.Programmazione_Cod, t.stato, t.pro_cod, t.com_Des, t.ind_Des, t.Validazione_Data, t.Allegati_Documenti_Numero, t.Validita_inizio, t.Validita_Fine, t.Data_Creazione, t.Fonte_cod " & vbCrLf)
                    ElseIf veg_Cods.Count = 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryTestataId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" Group by t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Piva, t.Programmazione_Cod, t.stato, t.pro_cod, t.com_Des, t.ind_Des, t.Validazione_Data, t.Allegati_Documenti_Numero, t.Validita_inizio, t.Validita_Fine, t.Data_Creazione, t.Fonte_cod " & vbCrLf)
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count = 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryTestataVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" Group by t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Piva, t.Programmazione_Cod, t.stato, t.pro_cod, t.com_Des, t.ind_Des, t.Validazione_Data, t.Allegati_Documenti_Numero, t.Validita_inizio, t.Validita_Fine, t.Data_Creazione, t.Fonte_cod " & vbCrLf)
                    ElseIf veg_Cods.Count <> 0 AndAlso id_cods.Count <> 0 Then
                        StrSQL.Append("SELECT * FROM " & vbCrLf)
                        StrSQL.Append("( ")
                        StrSQL.Append(creaQueryTestataVeg_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, veg_Cods, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append("UNION " & vbCrLf)
                        StrSQL.Append(creaQueryTestataId_Cod(ImpresePadri, Piva, Sa_Cod, validita_Inizio, validita_fine, id_cods, filtroUltima, filtroEnti, dtCuaa))
                        StrSQL.Append(") t " & vbCrLf)
                        StrSQL.Append(" Group by t.cuaa, t.rag_soc, t.Programmazione_Des_Long, t.Piva, t.Programmazione_Cod, t.stato, t.pro_cod, t.com_Des, t.ind_Des, t.Validazione_Data, t.Allegati_Documenti_Numero, t.Validita_inizio, t.Validita_Fine, t.Data_Creazione, t.Fonte_cod " & vbCrLf)
                    End If


                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'StrSQL.Append(" ORDER BY PE.Validita_inizio ASC")
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

    Public Function LeggiTestata_Da_GerarchiaImpresa(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          ByVal id_cods As List(Of String),
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef filtroUltima As Boolean,
                                                             ByRef filtroEnti As Boolean,
                                                     ByRef dtCuaa As DataTable) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiEntitaImpresa_Da_Particella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine("	WITH Programmazione_testata_cte as (  ")

                    StrSQL.AppendLine("		SELECT	Piva,  ")
                    If filtroUltima Then
                        StrSQL.AppendLine("				Max(programmazione_Cod) as Programmazione_Cod  ")
                    Else
                        StrSQL.AppendLine("				Programmazione_Cod  ")
                    End If

                    StrSQL.AppendLine("		FROM Programmazione_Testata  ")
                    StrSQL.AppendLine("		WHERE Validita_Inizio >=  " & Agro_SQL_SaveDate(validita_Inizio) & "     ")
                    StrSQL.AppendLine("			AND Validita_Fine <=  " & Agro_SQL_SaveDate(validita_fine) & "   ")
                    If filtroEnti Then
                        StrSQL.AppendLine("AND Programmazione_Testata.fonte_cod=0 ")
                    End If
                    If filtroUltima Then
                        StrSQL.AppendLine("		GROUP BY Piva ")
                    End If
                    StrSQL.AppendLine("	), ")
                    If veg_Cods.Count > 0 OrElse id_cods.Count > 0 Then
                        StrSQL.AppendLine("		Programmazione_testata_filtro_cte as (   ")
                        StrSQL.AppendLine("			SELECT	DISTINCT Programmazione_testata_cte.Programmazione_Cod ")
                        StrSQL.AppendLine("			FROM Programmazione_testata_cte ")
                        StrSQL.AppendLine("			JOIN Programmazione_Entita pe ON Programmazione_testata_cte.Programmazione_Cod = pe.Programmazione_Cod ")
                        StrSQL.AppendLine("			WHERE 1 = 1 ")
                        If veg_Cods.Count <> 0 Then
                            If veg_Cods.Count = 1 Then
                                Dim veg_cod = veg_Cods(0)
                                If veg_cod <> "0" AndAlso veg_cod <> "" AndAlso veg_cod <> " " Then
                                    StrSQL.AppendLine("AND pe.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " ")
                                End If
                            Else
                                StrSQL.AppendLine("AND pe.veg_cod IN ( ")
                                Dim first = True
                                For Each veg_cod In veg_Cods
                                    If veg_cod <> "" Then
                                        If Not first Then
                                            StrSQL.AppendLine(",")
                                        Else
                                            first = False
                                        End If
                                        StrSQL.AppendLine(" " & Agro_SQL_SaveNum(veg_cod) & " ")
                                    End If
                                Next
                                StrSQL.AppendLine(") ")
                            End If
                        End If
                        If id_cods.Count <> 0 Then
                            If id_cods.Count = 1 Then
                                Dim id_cod = id_cods(0)
                                If id_cod <> "0" AndAlso id_cod <> "" AndAlso id_cod <> " " Then
                                    StrSQL.AppendLine(" AND pe.id_cod = " & Agro_SQL_SaveNum(id_cod) & " ")
                                End If
                            Else
                                StrSQL.AppendLine(" AND pe.id_cod IN ( ")
                                Dim first = True
                                For Each id_cod In id_cods
                                    If id_cod <> "" Then
                                        If Not first Then
                                            StrSQL.AppendLine(",")
                                        Else
                                            first = False
                                        End If
                                        StrSQL.AppendLine(" " & Agro_SQL_SaveNum(id_cod) & " ")
                                    End If
                                Next
                                StrSQL.AppendLine(" ) ")
                            End If
                        End If
                        StrSQL.AppendLine("		),  ")
                    End If
                    StrSQL.AppendLine("		gerarchiaImprese_ct as (  ")
                    StrSQL.AppendLine("			SELECT STRING_AGG(GerarchiaImprese.Padre, ',') as Piva_padre, STRING_AGG(Imprese.rag_soc, ',') as rag_soc_Padre, GerarchiaImprese.Figlio  ")
                    StrSQL.AppendLine("			FROM GerarchiaImprese  ")
                    StrSQL.AppendLine("			JOIN Imprese ON GerarchiaImprese.Padre = Imprese.PIVA  ")
                    StrSQL.AppendLine("			GROUP BY GerarchiaImprese.Figlio  ")
                    StrSQL.AppendLine("		), ")
                    StrSQL.AppendLine("		Allegati_EntitaxDocumenti_cte as (  ")
                    StrSQL.AppendLine("			SELECT max(Allegati_EntitaxDocumenti.Allegati_Documenti_Cod) as Allegati_Documenti_Cod, Allegati_EntitaxDocumenti.Programmazione_Cod   ")
                    StrSQL.AppendLine("			FROM Allegati_EntitaxDocumenti   ")
                    StrSQL.AppendLine("			JOIN Programmazione_testata_cte ON Allegati_EntitaxDocumenti.Programmazione_Cod = Programmazione_testata_cte.Programmazione_Cod  ")
                    StrSQL.AppendLine("			WHERE Programmazione_Entita_Cod = 0  ")
                    StrSQL.AppendLine("			GROUP BY Allegati_EntitaxDocumenti.Programmazione_Cod  ")
                    StrSQL.AppendLine("		),  ")
                    StrSQL.AppendLine("       NumberedContacts AS (")
                    StrSQL.AppendLine("           SELECT Piva, Sa_Cod, Cod_Contatto, ISNULL(CONCAT(Rag_Soc, ' ', Nome, ' ', Cognome), '') AS Tecnico_Riferimento_Nome,")
                    StrSQL.AppendLine("           ROW_NUMBER() OVER (PARTITION BY Cod_Contatto ORDER BY Sa_Cod ASC) AS ContactNumber")
                    StrSQL.AppendLine("           From Contatti")
                    StrSQL.AppendLine("       ),")
                    StrSQL.AppendLine("       FirstEncounters AS (")
                    StrSQL.AppendLine("           Select Piva, Sa_Cod, Cod_Contatto, Tecnico_Riferimento_Nome")
                    StrSQL.AppendLine("           FROM NumberedContacts")
                    StrSQL.AppendLine("           WHERE ContactNumber = 1")
                    StrSQL.AppendLine("       )")
                    StrSQL.AppendLine("		SELECT  ")
                    StrSQL.AppendLine("			ic.val_cod as Cuaa, ")
                    StrSQL.AppendLine("			i.rag_soc, ")
                    StrSQL.AppendLine("			pt.Programmazione_Des_Long, ")
                    StrSQL.AppendLine("			pt.Piva, ")
                    StrSQL.AppendLine("			COALESCE(Gruppi_Raccolta.GruppoRaccolta_Cod, 0) AS GruppoRaccolta_Cod, ")
                    StrSQL.AppendLine("			COALESCE(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS GruppoRaccolta_Des, ")
                    StrSQL.AppendLine("			pt.Programmazione_Cod, ")
                    StrSQL.AppendLine("			ind.stato, ")
                    StrSQL.AppendLine("			comune.PROV as pro_cod, ")
                    StrSQL.AppendLine("			comune.LOCALITA as com_Des, ")
                    StrSQL.AppendLine("			ind.ind_des, ")
                    StrSQL.AppendLine("			ad.Validazione_Data, ")
                    StrSQL.AppendLine("			ad.Allegati_Documenti_Numero, ")
                    StrSQL.AppendLine("			pt.Validita_Inizio, ")
                    StrSQL.AppendLine("			pt.Validita_Fine, ")
                    StrSQL.AppendLine("			pt.Data_Creazione, ")
                    StrSQL.AppendLine("			pt.Data_Modifica, ")
                    StrSQL.AppendLine("			COALESCE(Utenti_Creazione.[user], pt.Username_Modifica) as Username_Creazione, ")
                    StrSQL.AppendLine("			COALESCE(Utenti_Modifica.[user], pt.Username_Creazione) as Username_Modifica, ")
                    StrSQL.AppendLine("			pt.Fonte_Cod, ")
                    StrSQL.AppendLine("			FE.Tecnico_Riferimento_Nome ")
                    StrSQL.AppendLine("		FROM Programmazione_Testata pt ")
                    StrSQL.AppendLine("		JOIN Programmazione_testata_cte on pt.Programmazione_Cod = Programmazione_testata_cte.Programmazione_Cod ")
                    StrSQL.AppendLine("		JOIN gerarchiaImprese_ct ON pt.Piva = gerarchiaImprese_ct.Figlio  ")
                    StrSQL.AppendLine("		JOIN Imprese i ON pt.Piva = i.PIVA  ")
                    StrSQL.AppendLine("		LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010   ")
                    StrSQL.AppendLine("		LEFT JOIN Imprese_Codici icc ON i.piva = icc.piva AND icc.id_Cod = 1088   ")
                    StrSQL.AppendLine("		LEFT JOIN ImpreseXIndirizzi ii ON i.piva = ii.piva AND Tipo_Indirizzo = 1 ")
                    StrSQL.AppendLine("		LEFT JOIN Indirizzi ind ON ind.Cod_Indirizzo = ii.cod_indirizzo  ")
                    StrSQL.AppendLine("		LEFT JOIN ISTAT comune ON ind.com_cod_istat = comune.COM AND ind.pro_cod_istat = comune.prov  ")
                    StrSQL.AppendLine("		LEFT JOIN Allegati_EntitaxDocumenti_cte aed ON aed.Programmazione_Cod=pt.Programmazione_Cod  ")
                    StrSQL.AppendLine("		LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod   ")
                    StrSQL.AppendLine("		LEFT JOIN Gruppi_Raccolta ON i.GruppoRaccolta_Cod = Gruppi_Raccolta.GruppoRaccolta_Cod ")
                    StrSQL.AppendLine("		LEFT JOIN Utenti Utenti_Modifica ON pt.Username_Modifica = Utenti_Modifica.CODICE_FISCALE ")
                    StrSQL.AppendLine("		LEFT JOIN Utenti Utenti_Creazione ON pt.Username_Creazione = Utenti_Creazione.CODICE_FISCALE ")
                    StrSQL.AppendLine("	    LEFT JOIN FirstEncounters FE ON icc.val_cod = FE.Cod_Contatto  ")
                    If veg_Cods.Count > 0 OrElse id_cods.Count > 0 Then
                        StrSQL.AppendLine("		JOIN Programmazione_testata_filtro_cte ON pt.Programmazione_Cod = Programmazione_testata_filtro_cte.Programmazione_Cod ")
                    End If
                    StrSQL.AppendLine("		WHERE 1 = 1   ")
                    StrSQL.AppendLine("		AND pt.Tipo_Pianificazione IN (0,2)   ")
                    If Piva <> "" Then
                        StrSQL.AppendLine("AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " ")
                    End If
                    If validita_Inizio <> AGRODATAINIZIO Then
                        StrSQL.AppendLine("AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " ")
                    End If
                    If validita_fine <> AGRODATAFINE Then
                        StrSQL.AppendLine("AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine("AND pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If
                    If ImpresePadri.Count <> 0 Then
                        If ImpresePadri.Count = 1 Then
                            Dim impresa = ImpresePadri(0)
                            If impresa <> "-1" Then
                                StrSQL.AppendLine("AND gerarchiaImprese_ct.Piva_Padre like '%" & Agro_SQL_SaveText(impresa) & "%' ")
                            Else
                                StrSQL.AppendLine("AND gerarchiaImprese_ct.Piva_Padre <> '' ")
                            End If
                        Else
                            StrSQL.AppendLine("AND gerarchiaImprese_ct.Piva_Padre IN ( ")
                            Dim first = True
                            For Each pPadre In ImpresePadri
                                If pPadre <> "" Then
                                    If Not first Then
                                        StrSQL.AppendLine(",")
                                    Else
                                        first = False
                                    End If
                                    StrSQL.AppendLine(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                                End If
                            Next
                            StrSQL.AppendLine(") ")
                        End If
                    End If
                    If dtCuaa IsNot Nothing AndAlso dtCuaa.Rows.Count > 0 Then
                        StrSQL.AppendLine("AND ic.val_cod IN ( ")
                        For i = 0 To dtCuaa.Rows.Count - 1
                            Dim cuaa As String = dtCuaa.Rows(i)("CUAA")
                            cuaa = cuaa.Trim
                            cuaa = cuaa.Replace("'", "")
                            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(cuaa) & "'")
                            If i <> dtCuaa.Rows.Count - 1 Then
                                StrSQL.AppendLine(",")
                            End If
                        Next
                        StrSQL.AppendLine(" ) ")
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

    Private Function creaQueryTestataVeg_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal veg_Cods As List(Of String),
                                                          filtroUltima As Boolean,
                                                          filtroEnti As Boolean,
                                                          dtCuaa As DataTable) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.Append("SELECT val_cod as cuaa, rag_soc,pt.Programmazione_Des_Long, i.Piva, pt.Programmazione_Cod, ind.stato, ind.pro_cod, ind.com_Des, ind.ind_Des, ad.Validazione_Data, ad.Allegati_Documenti_Numero, pt.Validita_inizio, pt.Validita_Fine, pt.Data_Creazione, pt.Fonte_cod " & vbCrLf)
        StrSQL.Append(" FROM Imprese I  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva  " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" AND Pt.programmazione_Cod IN ( " & vbCrLf)
            StrSQL.Append("    SELECT Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
            StrSQL.Append("    AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
            If Not filtroEnti Then
                StrSQL.Append("    AND fonte_cod=0 " & vbCrLf)
            End If
            StrSQL.Append(" )")
        End If
        StrSQL.Append(" LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN ImpreseXIndirizzi ii ON i.piva = ii.piva AND Tipo_Indirizzo = 1" & vbCrLf)
        StrSQL.Append(" LEFT JOIN Indirizzi ind ON ind.Cod_Indirizzo = ii.cod_indirizzo " & vbCrLf)
        StrSQL.Append(" LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod " & vbCrLf)
        StrSQL.Append(" LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_EntitaxDocumenti aed ON pt.Programmazione_Cod = aed.programmazione_Cod AND aed.Programmazione_Entita_Cod = 0 " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod")
        StrSQL.Append(" WHERE 1=1 ")
        StrSQL.Append(" AND pt.programmazione_Cod is not null " & vbCrLf)
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                Else
                    StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        If veg_Cods.Count <> 0 Then
            If veg_Cods.Count = 1 Then
                Dim veg_cod = veg_Cods(0)
                If veg_cod <> "0" AndAlso veg_cod <> "" AndAlso veg_cod <> " " Then
                    StrSQL.Append(" AND pe.veg_cod = " & Agro_SQL_SaveNum(veg_cod) & " " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND pe.veg_cod IN ( ")
                Dim first = True
                For Each veg_cod In veg_Cods
                    If veg_cod <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveNum(veg_cod) & " ")
                    End If
                Next
                StrSQL.Append(" ) " & vbCrLf)
            End If
        End If
        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.Append(" AND pt.fonte_cod=0 " & vbCrLf)
        End If
        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If dtCuaa IsNot Nothing AndAlso dtCuaa.Rows.Count > 0 Then
            StrSQL.AppendLine(" AND   ic.val_cod in ( ")
            For i = 0 To dtCuaa.Rows.Count - 1
                Dim cuaa As String = dtCuaa.Rows(i)("CUAA")
                cuaa = cuaa.Trim
                cuaa = cuaa.Replace("'", "")
                StrSQL.AppendLine(" '" & Agro_SQL_SaveText(cuaa) & "'")
                If i <> dtCuaa.Rows.Count - 1 Then
                    StrSQL.Append(",")
                End If
            Next
            StrSQL.AppendLine(" ) ")
        End If

        Return StrSQL.ToString
    End Function

    Private Function creaQueryTestataId_Cod(ByVal ImpresePadri As List(Of String),
                                                                     ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal id_cods As List(Of String),
                                                          filtroUltima As Boolean,
                                                          filtroEnti As Boolean,
                                                          dtCuaa As DataTable) As String
        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.Append("SELECT val_cod as cuaa, rag_soc,pt.Programmazione_Des_Long, i.Piva, pt.Programmazione_Cod, ind.stato, ind.pro_cod, ind.com_Des, ind.ind_Des, ad.Validazione_Data, ad.Allegati_Documenti_Numero, pt.Validita_inizio, pt.Validita_Fine, pt.Data_Creazione, pt.Fonte_cod  " & vbCrLf)
        StrSQL.Append(" FROM Imprese I  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Programmazione_Testata pt ON I.PIVA = pt.Piva  " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" AND Pt.programmazione_Cod IN ( " & vbCrLf)
            StrSQL.Append("    SELECT Max(programmazione_Cod) FROM Programmazione_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    AND Validita_Inizio >=  " & Agro_SQL_SaveDateTime_NULL(validita_Inizio) & "   " & vbCrLf)
            StrSQL.Append("    AND Validita_Fine <=  " & Agro_SQL_SaveDateTime_NULL(validita_fine) & " " & vbCrLf)
            If Not filtroEnti Then
                StrSQL.Append("    AND fonte_cod=0 " & vbCrLf)
            End If
            StrSQL.Append(" )")
        End If
        StrSQL.Append(" LEFT JOIN Programmazione_Entita pe ON pe.Programmazione_Cod = pt.Programmazione_Cod  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN ImpreseXIndirizzi ii ON i.piva = ii.piva AND Tipo_Indirizzo = 1" & vbCrLf)
        StrSQL.Append(" LEFT JOIN Indirizzi ind ON ind.Cod_Indirizzo = ii.cod_indirizzo " & vbCrLf)
        StrSQL.Append(" LEFT JOIN ISTAT comune ON pp.Com = comune.COM AND pp.Prov = comune.prov " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Macrousi m ON pe.Macrouso_Cod = m.Macrouso_Cod " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Cultivar c ON c.Cul_Cod = pe.Cul_Cod " & vbCrLf)
        StrSQL.Append(" LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = pe.Veg_Cod  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_EntitaxDocumenti aed ON pt.Programmazione_Cod = aed.programmazione_Cod AND aed.Programmazione_Entita_Cod = 0 " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Allegati_Documenti ad ON aed.Allegati_Documenti_Cod=ad.Allegati_Documenti_Cod")
        StrSQL.Append(" WHERE 1=1 ")
        StrSQL.Append(" AND pt.programmazione_Cod is not null " & vbCrLf)
        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" AND gi.Padre = " & Agro_SQL_SaveText_NULL(impresa) & " " & vbCrLf)
                Else
                    StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveText_NULL(pPadre) & " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If
        If id_cods.Count <> 0 Then
            If id_cods.Count = 1 Then
                Dim id_cod = id_cods(0)
                If id_cod <> "0" AndAlso id_cod <> "" AndAlso id_cod <> " " Then
                    StrSQL.Append(" AND pe.id_cod = " & Agro_SQL_SaveNum(id_cod) & " " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND pe.id_cod IN ( ")
                Dim first = True
                For Each id_cod In id_cods
                    If id_cod <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " & Agro_SQL_SaveNum(id_cod) & " ")
                    End If
                Next
                StrSQL.Append(" ) " & vbCrLf)
            End If
        End If
        'If livello <> 0 Then
        '    StrSQL.Append(" AND i.TipoImpresaGerarchia = " & Agro_SQL_SaveNum(livello) & " " & vbCrLf)
        'End If
        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " & Agro_SQL_SaveDateTime(validita_Inizio) & " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " & Agro_SQL_SaveDateTime(validita_fine) & " ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND   pt.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If
        If Not filtroEnti Then
            StrSQL.Append(" AND pt.fonte_cod=0 " & vbCrLf)
        End If
        If dtCuaa IsNot Nothing AndAlso dtCuaa.Rows.Count > 0 Then
            StrSQL.AppendLine(" AND   ic.val_cod in ( ")
            For i = 0 To dtCuaa.Rows.Count - 1
                Dim cuaa As String = dtCuaa.Rows(i)("CUAA")
                cuaa = cuaa.Trim
                cuaa = cuaa.Replace("'", "")
                StrSQL.AppendLine(" '" & Agro_SQL_SaveText(cuaa) & "'")
                If i <> dtCuaa.Rows.Count - 1 Then
                    StrSQL.Append(",")
                End If
            Next
            StrSQL.AppendLine(" ) ")
        End If
        Return StrSQL.ToString
    End Function

    '################################################################################
    'Trova tutte le particelle di una programmazione o di una entita
    '============================================================================
    Public Function LeggiParticelle_Da_Programmazione(ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal Programmazione_Cod As Integer,
                                                          ByVal Programmazione_Entita_Cod As Integer,
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiParticelle_Da_Programmazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT *, CASE WHEN pe.veg_Cod = 0 THEN CASE WHEN ca.descrizione IS NULL THEN '' ELSE ca.descrizione END ELSE sp.veg_des END as Specie ")
                    StrSQL.AppendLine(" FROM Programmazione_Particelle PP ")
                    StrSQL.AppendLine(" LEFT JOIN Programmazione_Entita PE ")
                    StrSQL.AppendLine(" ON PP.programmazione_Entita_Cod=PE.Programmazione_Entita_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sp ON pe.veg_Cod =sp.Veg_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN Cultivar cu ON pe.veg_Cod =cu.Veg_Cod AND pe.Cul_Cod=cu.cul_cod ")
                    StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe ca ON ca.codice = pe.id_cod ")
                    StrSQL.AppendLine(" WHERE PE.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   PE.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND   PE.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   PE.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Programmazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   PE.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
                    End If

                    If Programmazione_Entita_Cod <> 0 Then
                        StrSQL.AppendLine(" AND   PE.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            'StrSQL.appendline(" AND   PP.Inviato >=0 ")
                            'StrSQL.appendline(" AND   PE.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            'StrSQL.appendline(" AND   PP.Inviato =-1 ")
                            'StrSQL.appendline(" AND   PE.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY PE.Validita_inizio ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


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

    Public Function LeggiParticelle_Da_Programmazione_Distinct(ByVal Piva As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal Programmazione_Cod As Integer,
                                                          ByVal Programmazione_Entita_Cod As Integer,
                                                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiParticelle_Da_Programmazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO ")
                    StrSQL.Append(" FROM Programmazione_Particelle PP ")
                    StrSQL.Append(" INNER JOIN Programmazione_Entita PE ")
                    StrSQL.Append(" ON PP.programmazione_Entita_Cod=PE.Programmazione_Entita_Cod ")
                    StrSQL.Append(" LEFT JOIN SpecieVegetali sp ON pe.veg_Cod =sp.Veg_Cod ")
                    StrSQL.Append(" LEFT JOIN Cultivar cu ON pe.veg_Cod =cu.Veg_Cod AND pe.Cul_Cod=cu.cul_cod ")
                    StrSQL.Append(" LEFT JOIN Codici_Anagrafe ca ON ca.codice = pe.id_cod ")
                    StrSQL.Append(" WHERE PE.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   PE.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   PE.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   PE.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Programmazione_Cod <> 0 Then
                        StrSQL.Append(" AND   PE.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
                    End If

                    If Programmazione_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND   PE.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   PP.Inviato >=0 ")
                            StrSQL.Append(" AND   PE.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   PP.Inviato =-1 ")
                            StrSQL.Append(" AND   PE.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    StrSQL.Append(" GROUP BY PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


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

    Public Function LeggiProvComParticelle_Da_Programmazione_Distinct(ByVal Programmazione_Cod As Integer,
                                                                        ByVal Programmazione_Entita_Cod As Integer,
                                                                         ByVal xFiltroAggiuntivo As String,
                                                                         ByVal xOrderBy As String,
                                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.LeggiParticelle_Da_Programmazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT distinct PP.Prov, PP.Com, i.LOCALITA, i.PROV,i.cap,i.COMUNI_PROV ")
            StrSQL.Append(" FROM Programmazione_Particelle PP ")
            StrSQL.Append(" INNER JOIN Programmazione_Entita PE ")
            StrSQL.Append(" ON PP.programmazione_Entita_Cod=PE.Programmazione_Entita_Cod ")
            StrSQL.Append(" AND PP.Piva_SuperUser=PE.Piva_SuperUser ")
            StrSQL.Append(" INNER JOIN Istat i on i.COM = PP.Com AND i.PROV = PP.PROV")
            StrSQL.Append(" WHERE PE.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND   PE.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND   PE.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PP.Inviato >=0 ")
                    StrSQL.Append(" AND   PE.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PP.Inviato =-1 ")
                    StrSQL.Append(" AND   PE.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '################################################################################
    Public Function Programmazione_Particelle_Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal Piva As String,
                                                    ByRef ErrMSG As String,
                                                    Optional ByVal Programmazione_Cod As Integer = 0,
                                                    Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                                                    Optional ByVal Prov As String = "",
                                                    Optional ByVal Com As String = "",
                                                    Optional ByVal Sezione As String = "0",
                                                    Optional ByVal Foglio As Integer = 0,
                                                    Optional ByVal Numero As Integer = 0,
                                                    Optional ByVal Subalterno As String = "0",
                                                    Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                    Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                    Optional ByVal Includi_Campo_Cod As Boolean = False,
                                                    Optional saCod As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Programmazione_Particelle_Leggi()"

        Dim DT As New DataTable
        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT DISTINCT")
            StrSQL.AppendLine("    Programmazione_Particelle.*")
            StrSQL.AppendLine("    , ISNULL(ISTAT.COMUNI_PROV,'') AS Prov_Des")
            StrSQL.AppendLine("    , ISNULL(ISTAT.LOCALITA,'') AS Com_Des")
            'StrSQL.Append("        (convert(varchar,ParticelleCatastali.ETTARI) + ',' + right('00' + convert(varchar(2),ParticelleCatastali.[ARE]), 2) + right('00' + convert(varchar(2),ParticelleCatastali.[CENTIARE]), 2)) AS Sup_Totale")
            StrSQL.AppendLine("    , ImpreseXParticelle.Sup_Condotta AS Sup_Totale")
            StrSQL.AppendLine("    , ImpreseXParticelle.TitoloPossesso AS TitoloPossesso")
            StrSQL.AppendLine("    , ImpreseXParticelle.Validita_Inizio AS Validita_Inizio")
            StrSQL.AppendLine("    , ImpreseXParticelle.Validita_Fine AS Validita_Fine")
            StrSQL.AppendLine("    , COALESCE(ParticelleCatastali.Proprietario, '') AS Proprietario")
            StrSQL.AppendLine("    , ParticelleCatastali.ETTARI AS ETTARI")
            StrSQL.AppendLine("    , ParticelleCatastali.ARE AS ARE")
            StrSQL.AppendLine("    , ParticelleCatastali.CENTIARE AS CENTIARE")

            If Includi_Campo_Cod Then
                StrSQL.AppendLine("    , Programmazione_Entita.Campo_Cod")
            End If

            StrSQL.AppendLine("FROM Programmazione_Particelle")
            StrSQL.AppendLine("    INNER JOIN ISTAT ON Programmazione_Particelle.Prov = ISTAT.PROV AND Programmazione_Particelle.Com = ISTAT.COM")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("    INNER JOIN ImpreseXParticelle ON Programmazione_Particelle.Prov = ImpreseXParticelle.PROV")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Com = ImpreseXParticelle.COM")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Sezione = ImpreseXParticelle.SEZIONE")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Foglio = ImpreseXParticelle.FOGLIO")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Numero = ImpreseXParticelle.NUMERO")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Subalterno = ImpreseXParticelle.SUBALTERNO")
            StrSQL.AppendLine("")

            StrSQL.AppendLine("    INNER JOIN ParticelleCatastali ON Programmazione_Particelle.Prov = ParticelleCatastali.PROV")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Com = ParticelleCatastali.COM")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Sezione = ParticelleCatastali.SEZIONE")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Foglio = ParticelleCatastali.FOGLIO")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Numero = ParticelleCatastali.NUMERO")
            StrSQL.AppendLine("        AND Programmazione_Particelle.Subalterno = ParticelleCatastali.SUBALTERNO")

            StrSQL.AppendLine("    INNER JOIN Programmazione_Entita ON Programmazione_Entita.Programmazione_Entita_Cod = Programmazione_Particelle.Programmazione_Entita_Cod")

            If Includi_Campo_Cod Then
                StrSQL.AppendLine("    INNER JOIN Programmazione_Entita ON Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod")
            End If

            StrSQL.AppendLine($"WHERE Programmazione_Particelle.Piva_SuperUser = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}'")
            StrSQL.AppendLine($"    AND Programmazione_Particelle.Validita_Inizio <= {Agro_SQL_SaveDate(Validita_Fine)}")
            StrSQL.AppendLine($"    AND Programmazione_Particelle.Validita_fine >= {Agro_SQL_SaveDate(Validita_Inizio)}")

            If Piva <> "" Then
                StrSQL.AppendLine("    AND     Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine("    AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Prov <> "" Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "0" Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Sezione = '" & Agro_SQL_SaveText(Sezione.ToString) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Foglio = " & Agro_SQL_SaveNum(Foglio.ToString))
            End If

            If Numero <> 0 Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Numero = " & Agro_SQL_SaveNum(Numero.ToString))
            End If

            If Subalterno <> "0" Then
                StrSQL.AppendLine("    AND Programmazione_Particelle.Subalterno = '" & Agro_SQL_SaveText(Subalterno.ToString) & "' ")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine("    AND Programmazione_Entita.Sa_Cod = " & Agro_SQL_SaveNum(saCod.ToString))
            End If

            StrSQL.AppendLine("ORDER BY Programmazione_Particelle.Programmazione_Entita_Cod, Programmazione_Particelle.Prov, Programmazione_Particelle.Com, Programmazione_Particelle.Sezione, Programmazione_Particelle.Foglio, Programmazione_Particelle.Numero, Programmazione_Particelle.Subalterno ASC ")


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

    '################################################################################
    Public Function Programmazione_Particelle_Leggi_2(
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal Programmazione_Cod As Integer,
                                                ByRef ErrMSG As String,
                                                Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                                                Optional ByVal Prov As String = "",
                                                Optional ByVal Com As String = "",
                                                Optional ByVal Sezione As String = "0",
                                                Optional ByVal Foglio As Integer = 0,
                                                Optional ByVal Numero As Integer = 0,
                                                Optional ByVal Subalterno As String = "0",
                                                Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                Optional ByVal Includi_Campo_Cod As Boolean = False) _
                                                As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Programmazione_Particelle_Leggi_2()"

        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder
        Dim MessaggioErrore As String

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT Programmazione_Particelle.*, ")
            StrSQL.Append("         ISNULL(ISTAT.COMUNI_PROV,'') AS Prov_Des, ISNULL(ISTAT.LOCALITA,'') AS Com_Des ")

            If Includi_Campo_Cod Then
                StrSQL.Append("    ,Programmazione_Entita.Campo_Cod ")
            End If

            StrSQL.Append(" FROM Programmazione_Particelle INNER JOIN")
            StrSQL.Append(" ISTAT ON Programmazione_Particelle.Prov = ISTAT.PROV AND Programmazione_Particelle.Com = ISTAT.COM INNER JOIN")
            StrSQL.Append(" Programmazione_Entita ON Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod AND  ")
            StrSQL.Append(" Programmazione_Particelle.Piva_SuperUser = Programmazione_Entita.Piva_SuperUser  ")

            StrSQL.Append(" WHERE   Programmazione_Particelle.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Programmazione_Particelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Particelle.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append(" AND     Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString) & " ")


            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Prov <> "" Then
                StrSQL.Append(" AND Programmazione_Particelle.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.Append(" AND Programmazione_Particelle.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "0" Then
                StrSQL.Append(" AND Programmazione_Particelle.Sezione = '" & Agro_SQL_SaveText(Sezione.ToString) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Foglio = " & Agro_SQL_SaveNum(Foglio.ToString))
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Numero = " & Agro_SQL_SaveNum(Numero.ToString))
            End If

            If Subalterno <> "0" Then
                StrSQL.Append(" AND Programmazione_Particelle.Subalterno = '" & Agro_SQL_SaveText(Subalterno.ToString) & "' ")
            End If


            StrSQL.Append(" ORDER BY Programmazione_Particelle.Programmazione_Entita_Cod, Programmazione_Particelle.Prov, Programmazione_Particelle.Com, Programmazione_Particelle.Sezione, Programmazione_Particelle.Foglio, Programmazione_Particelle.Numero, Programmazione_Particelle.Subalterno ASC ")


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

    ' se la particella ha più possessi prende il più recente (attenzione screma se ci sono piu particelle!!!!!!!! --> usare 4)
    '################################################################################
    Public Function Programmazione_Particelle_Leggi_3(
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal Piva As String,
                                                ByRef ErrMSG As String,
                                                Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                                                Optional ByVal Prov As String = "",
                                                Optional ByVal Com As String = "",
                                                Optional ByVal Sezione As String = "0",
                                                Optional ByVal Foglio As Integer = 0,
                                                Optional ByVal Numero As Integer = 0,
                                                Optional ByVal Subalterno As String = "0",
                                                Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                Optional ByVal Includi_Campo_Cod As Boolean = False) _
                                                As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Programmazione_Particelle_Leggi()"

        Dim DT As New DataTable
        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT TOP 1 Programmazione_Particelle.*, ")
            StrSQL.Append("         ISNULL(ISTAT.COMUNI_PROV,'') AS Prov_Des, ISNULL(ISTAT.LOCALITA,'') AS Com_Des, ")
            'StrSQL.Append("        (convert(varchar,ParticelleCatastali.ETTARI) + ',' + right('00' + convert(varchar(2),ParticelleCatastali.[ARE]), 2) + right('00' + convert(varchar(2),ParticelleCatastali.[CENTIARE]), 2)) AS Sup_Totale")
            StrSQL.Append("         ImpreseXParticelle.Sup_Condotta AS Sup_Totale ")

            If Includi_Campo_Cod Then
                StrSQL.Append("    ,Programmazione_Entita.Campo_Cod ")
            End If

            StrSQL.Append(" FROM Programmazione_Particelle INNER JOIN")
            StrSQL.Append(" ISTAT ON Programmazione_Particelle.Prov = ISTAT.PROV AND Programmazione_Particelle.Com = ISTAT.COM INNER JOIN")
            StrSQL.Append(" ImpreseXParticelle ON Programmazione_Particelle.Prov = ImpreseXParticelle.PROV AND ")
            StrSQL.Append(" Programmazione_Particelle.Com = ImpreseXParticelle.COM AND Programmazione_Particelle.Sezione = ImpreseXParticelle.SEZIONE AND ")
            StrSQL.Append(" Programmazione_Particelle.Foglio = ImpreseXParticelle.FOGLIO AND Programmazione_Particelle.Numero = ImpreseXParticelle.NUMERO AND ")
            StrSQL.Append(" Programmazione_Particelle.Subalterno = ImpreseXParticelle.SUBALTERNO ")

            If Includi_Campo_Cod Then
                StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ")
            End If

            StrSQL.Append(" WHERE   Programmazione_Particelle.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Programmazione_Particelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Particelle.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append(" AND     ImpreseXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            ' se la particella ha diversi possessi, prende il più recente nell'intervallo definito
            StrSQL.Append(" AND NOT EXISTS (SELECT * FROM ImpreseXParticelle IP2 ")
            StrSQL.Append(" WHERE IP2.PIVA = ImpreseXParticelle.PIVA And ")
            StrSQL.Append(" IP2.sa_cod = ImpreseXParticelle.sa_cod And ")
            StrSQL.Append(" IP2.Prov = ImpreseXParticelle.PROV And ")
            StrSQL.Append(" IP2.Com = ImpreseXParticelle.COM And IP2.Sezione = ImpreseXParticelle.SEZIONE And ")
            StrSQL.Append(" IP2.Foglio = ImpreseXParticelle.FOGLIO And IP2.Numero = ImpreseXParticelle.NUMERO And ")
            StrSQL.Append(" IP2.Subalterno = ImpreseXParticelle.SUBALTERNO ")
            StrSQL.Append(" AND IP2.Validita_Fine>ImpreseXParticelle.Validita_Fine ")
            StrSQL.Append(" AND IP2.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND IP2.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) ")


            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Prov <> "" Then
                StrSQL.Append(" AND Programmazione_Particelle.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.Append(" AND Programmazione_Particelle.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "0" Then
                StrSQL.Append(" AND Programmazione_Particelle.Sezione = '" & Agro_SQL_SaveText(Sezione.ToString) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Foglio = " & Agro_SQL_SaveNum(Foglio.ToString))
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Numero = " & Agro_SQL_SaveNum(Numero.ToString))
            End If

            If Subalterno <> "0" Then
                StrSQL.Append(" AND Programmazione_Particelle.Subalterno = '" & Agro_SQL_SaveText(Subalterno.ToString) & "' ")
            End If


            StrSQL.Append(" ORDER BY Programmazione_Particelle.Programmazione_Entita_Cod, Programmazione_Particelle.Prov, Programmazione_Particelle.Com, Programmazione_Particelle.Sezione, Programmazione_Particelle.Foglio, Programmazione_Particelle.Numero, Programmazione_Particelle.Subalterno ASC ")


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

    ' se la particella ha più possessi prende il più recente
    Public Function Programmazione_Particelle_Leggi_4(
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal Piva As String,
                                            ByRef ErrMSG As String,
                                            Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                                            Optional ByVal Prov As String = "",
                                            Optional ByVal Com As String = "",
                                            Optional ByVal Sezione As String = "0",
                                            Optional ByVal Foglio As Integer = 0,
                                            Optional ByVal Numero As Integer = 0,
                                            Optional ByVal Subalterno As String = "0",
                                            Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                            Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                            Optional ByVal Includi_Campo_Cod As Boolean = False) _
                                            As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Programmazione_Particelle_Leggi()"

        Dim DT As New DataTable
        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Programmazione_Particelle.*, ")
            StrSQL.Append("         ISNULL(ISTAT.COMUNI_PROV,'') AS Prov_Des, ISNULL(ISTAT.LOCALITA,'') AS Com_Des, ")
            'StrSQL.Append("        (convert(varchar,ParticelleCatastali.ETTARI) + ',' + right('00' + convert(varchar(2),ParticelleCatastali.[ARE]), 2) + right('00' + convert(varchar(2),ParticelleCatastali.[CENTIARE]), 2)) AS Sup_Totale")
            StrSQL.Append("         ImpreseXParticelle.Sup_Condotta AS Sup_Totale ")

            If Includi_Campo_Cod Then
                StrSQL.Append("    ,Programmazione_Entita.Campo_Cod ")
            End If

            StrSQL.Append(" FROM Programmazione_Particelle INNER JOIN")
            StrSQL.Append(" ISTAT ON Programmazione_Particelle.Prov = ISTAT.PROV AND Programmazione_Particelle.Com = ISTAT.COM INNER JOIN")
            StrSQL.Append(" ImpreseXParticelle ON Programmazione_Particelle.Prov = ImpreseXParticelle.PROV AND ")
            StrSQL.Append(" Programmazione_Particelle.Com = ImpreseXParticelle.COM AND Programmazione_Particelle.Sezione = ImpreseXParticelle.SEZIONE AND ")
            StrSQL.Append(" Programmazione_Particelle.Foglio = ImpreseXParticelle.FOGLIO AND Programmazione_Particelle.Numero = ImpreseXParticelle.NUMERO AND ")
            StrSQL.Append(" Programmazione_Particelle.Subalterno = ImpreseXParticelle.SUBALTERNO ")

            If Includi_Campo_Cod Then
                StrSQL.Append(" INNER JOIN Programmazione_Entita ON Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ")
            End If

            StrSQL.Append(" WHERE   Programmazione_Particelle.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Programmazione_Particelle.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Particelle.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append(" AND     ImpreseXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            ' se la particella ha diversi possessi, prende il più recente nell'intervallo definito
            StrSQL.Append(" AND NOT EXISTS (SELECT * FROM ImpreseXParticelle IP2 ")
            StrSQL.Append(" WHERE IP2.PIVA = ImpreseXParticelle.PIVA And ")
            StrSQL.Append(" IP2.sa_cod = ImpreseXParticelle.sa_cod And ")
            StrSQL.Append(" IP2.Prov = ImpreseXParticelle.PROV And ")
            StrSQL.Append(" IP2.Com = ImpreseXParticelle.COM And IP2.Sezione = ImpreseXParticelle.SEZIONE And ")
            StrSQL.Append(" IP2.Foglio = ImpreseXParticelle.FOGLIO And IP2.Numero = ImpreseXParticelle.NUMERO And ")
            StrSQL.Append(" IP2.Subalterno = ImpreseXParticelle.SUBALTERNO ")
            StrSQL.Append(" AND IP2.Validita_Fine>ImpreseXParticelle.Validita_Fine ")
            StrSQL.Append(" AND IP2.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND IP2.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) ")


            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString))
            End If

            If Prov <> "" Then
                StrSQL.Append(" AND Programmazione_Particelle.Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.Append(" AND Programmazione_Particelle.Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "0" Then
                StrSQL.Append(" AND Programmazione_Particelle.Sezione = '" & Agro_SQL_SaveText(Sezione.ToString) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Foglio = " & Agro_SQL_SaveNum(Foglio.ToString))
            End If

            If Numero <> 0 Then
                StrSQL.Append(" AND Programmazione_Particelle.Numero = " & Agro_SQL_SaveNum(Numero.ToString))
            End If

            If Subalterno <> "0" Then
                StrSQL.Append(" AND Programmazione_Particelle.Subalterno = '" & Agro_SQL_SaveNum(Subalterno.ToString) & "' ")
            End If


            StrSQL.Append(" ORDER BY Programmazione_Particelle.Programmazione_Entita_Cod, Programmazione_Particelle.Prov, Programmazione_Particelle.Com, Programmazione_Particelle.Sezione, Programmazione_Particelle.Foglio, Programmazione_Particelle.Numero, Programmazione_Particelle.Subalterno ASC ")


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



    '################################################################################
    Public Function Programmazione_Particelle_Leggi_in_x_prenotazione_piante(
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal Programmazione_Entita_Cod As String) _
                                                As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Programmazione_Particelle_Leggi_2()"

        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder
        Dim MessaggioErrore As String

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Programmazione_Particelle.* ")

            StrSQL.Append(" FROM Programmazione_Particelle ")

            StrSQL.Append(" WHERE   Programmazione_Entita_Cod in  (" & Agro_SQL_SaveText(Programmazione_Entita_Cod) & ") ")


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

    Public Function Leggi_Particelle_da_programmazione_Cod(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal Programmazione_Cod As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R.Programmazione_Particelle_Leggi_2()"

        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder
        Dim MessaggioErrore As String

        Try

            StrSQL.Length = 0

            StrSQL.Append("SELECT PROV, COM, Sezione, Foglio, Numero, Subalterno " & vbCrLf)
            StrSQL.Append("FROM  Programmazione_Entita pe " & vbCrLf)
            StrSQL.Append("LEFT JOIN Programmazione_Particelle pp ON pe.Programmazione_Entita_Cod=pp.Programmazione_Entita_Cod" & vbCrLf)
            StrSQL.Append("WHERE   pe.Programmazione_Cod=" & Agro_SQL_SaveNum(Programmazione_Cod) & " " & vbCrLf)
            StrSQL.Append("GROUP BY PROV, COM, Sezione, Foglio, Numero, Subalterno " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)
        End Try

        Return DT
    End Function

End Class





'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Programmazione_Particelle_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Function Scrivi(ByVal Programmazione_Entita_Cod As Integer,
                               ByVal Prov As String,
                               ByVal Com As String,
                               ByVal Sezione As String,
                               ByVal Foglio As Integer,
                               ByVal Numero As Integer,
                               ByVal Subalterno As String,
                               ByVal Superficie As Decimal,
                               ByVal Validita_Inizio As Date,
                               ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '----- Genero la query SQL 
            StrSQL.Append(" INSERT INTO Programmazione_Particelle ( ")
            StrSQL.Append("             Piva_SuperUser, Programmazione_Entita_Cod, ")
            StrSQL.Append("             Prov,           Com,        Sezione, ")
            StrSQL.Append("             Foglio,         Numero,     Subalterno,  ")
            StrSQL.Append("             Superficie, ")
            StrSQL.Append("             Inviato,                ")
            StrSQL.Append("             Data_Creazione,         Data_Modifica, ")
            StrSQL.Append("             UserName_Creazione,     UserName_Modifica, ")
            StrSQL.Append("             Validita_Inizio,        Validita_Fine ")
            StrSQL.Append(" ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Programmazione_Entita_Cod.ToString) & " ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Prov.ToString) & "' ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Com.ToString) & "' ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Sezione.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Foglio.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Numero.ToString) & " ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Subalterno.ToString) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Superficie.ToString) & " ")
            StrSQL.Append("         , 0  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(" ) ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & vbCrLf
            MessaggioErrore &= "Programmazione_Entita_Cod=" & CStr(Programmazione_Entita_Cod) &
                                "  PROV=" & CStr(Prov) &
                                "  COM=" & CStr(Com) &
                                "  Sezione=" & CStr(Sezione) &
                                "  Foglio=" & CStr(Foglio) &
                                "  Numero=" & CStr(Numero) &
                                "  Subalterno=" & CStr(Subalterno)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.Modifica
    ''' </summary>
    ''' <param name="Programmazione_Entita_Cod">OBBLIGATORIO</param>
    ''' <param name="Prov">OBBLIGATORIO</param>
    ''' <param name="Com">OBBLIGATORIO</param>
    ''' <param name="Sezione">OBBLIGATORIO</param>
    ''' <param name="Foglio">OBBLIGATORIO</param>
    ''' <param name="Numero">OBBLIGATORIO</param>
    ''' <param name="Subalterno">OBBLIGATORIO</param>
    ''' <param name="Superficie">StrDefault_per_MODIFICA</param>
    ''' <param name="Validita_Inizio">DataDefault_per_MODIFICA</param>
    ''' <param name="Validita_Fine">DataDefault_per_MODIFICA</param>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica(
                               ByVal Programmazione_Entita_Cod As Integer,
                               ByVal Prov As String,
                               ByVal Com As String,
                               ByVal Sezione As String,
                               ByVal Foglio As Integer,
                               ByVal Numero As Integer,
                               ByVal Subalterno As String,
                               ByVal Superficie As Decimal,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'controllo i campi obbligatori
            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Entita_Cod obbligatorio)")
            End If

            If Prov = "" Then
                Throw New Exception("Parametro non corretto nella query (Prov obbligatorio)")
            End If
            If Com = "" Then
                Throw New Exception("Parametro non corretto nella query (Com obbligatorio)")
            End If
            If Sezione = "" Then
                Throw New Exception("Parametro non corretto nella query (Sezione obbligatorio)")
            End If
            If Foglio = 0 Then
                Throw New Exception("Parametro non corretto nella query (Foglio obbligatorio)")
            End If
            If Numero = 0 Then
                Throw New Exception("Parametro non corretto nella query (Numero obbligatorio)")
            End If
            If Subalterno = "" Then
                Throw New Exception("Parametro non corretto nella query (Subalterno obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Particelle SET  ")
            If Superficie <> StrDefault_per_MODIFICA Then
                StrSQL.Append("    Superficie     = '" & Agro_SQL_SaveText(Superficie) & "'  ,")
            End If
            If Validita_Inizio <> DataDefault_per_MODIFICA Then
                StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveText(Validita_Inizio) & "  ,")
            End If
            If Validita_Fine <> DataDefault_per_MODIFICA Then
                StrSQL.Append("    Validita_Fine     = " & Agro_SQL_SaveText(Validita_Fine) & " ,")
            End If

            'rimuovo l ultima virgola
            StrSQL.Remove(StrSQL.Length - 1, 1)

            StrSQL.Append(" WHERE Programmazione_Entita_Cod     = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND   Prov =  '" & Agro_SQL_SaveText(Prov) & "'   ")
            StrSQL.Append(" AND   Com =  '" & Agro_SQL_SaveText(Com) & "'   ")
            StrSQL.Append(" AND   Sezione =  '" & Agro_SQL_SaveText(Sezione) & "'   ")
            StrSQL.Append(" AND   Foglio =  " & Agro_SQL_SaveNum(Foglio) & "   ")
            StrSQL.Append(" AND   Numero =  " & Agro_SQL_SaveNum(Numero) & "   ")
            StrSQL.Append(" AND   Subalterno =  '" & Agro_SQL_SaveText(Subalterno) & "'   ")
            '---------------------------------------------

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
    '################################################################################
    Public Function Cancella(
                               ByVal Piva As String,
                               ByVal Programmazione_Entita_Cod As Integer,
                               ByVal Prov As String,
                               ByVal Com As String,
                               ByVal Sezione As String,
                               ByVal Foglio As Integer,
                               ByVal Numero As Integer,
                               ByVal Subalterno As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser = ""         => Vengono cancellate tutte le entità del db
        '   Programmazione_Cod=0        => Vengono cancellate tutte le entità del Piva_SuperUser
        '   Programmazione_Entita_Cod=0 => Vengono cancellate tutte le entità di una programmazione
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Programmazione_Particelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      PP.Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("     ,PP.Inviato = -1 ")
                StrSQL.Append(" FROM Programmazione_Particelle PP ")
                StrSQL.Append(" INNER JOIN Programmazione_Entita PE ON PP.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ")
                StrSQL.Append(" WHERE  PP.Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE PP ")
                StrSQL.Append(" FROM  Programmazione_Particelle AS PP ")
                StrSQL.Append(" INNER JOIN Programmazione_Entita AS PE ON PP.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ")
                StrSQL.Append(" WHERE   1=1 ")


            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append("  AND   PP.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append("  AND   PE.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append("  AND   PP.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If Prov <> "" Then
                StrSQL.Append("  AND   Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.Append("  AND   Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "" AndAlso Sezione <> "0" Then
                StrSQL.Append("  AND   Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.Append("  AND   Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If

            If Numero <> 0 Then
                StrSQL.Append("  AND   Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If Subalterno <> "" AndAlso Subalterno <> "0" Then
                StrSQL.Append("  AND   Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica_Validita_Fine(
                            ByRef Programmazione_Entita_Cod As Integer,
                            ByVal Validita_Fine As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.Modifica_Validita_Fine()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Particelle SET  ")

            StrSQL.Append("    Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica_Validita(
                             ByRef Programmazione_Entita_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.Modifica_Validita()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Programmazione_Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Particelle SET  ")

            StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("    , Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Entita_Cod         = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaParticella(
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Programmazione_Entita_Cod As Integer,
                               ByVal Prov As String,
                               ByVal Com As String,
                               ByVal Sezione As String,
                               ByVal Foglio As Integer,
                               ByVal Numero As Integer,
                               ByVal Subalterno As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W.CancellaParticella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser = ""         => Vengono cancellate tutte le entità del db
        '   Programmazione_Cod=0        => Vengono cancellate tutte le entità del Piva_SuperUser
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.AppendLine(" UPDATE Programmazione_Particelle ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      PP.Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("     ,PP.Inviato = -1 ")
                StrSQL.AppendLine(" FROM Programmazione_Particelle PP ")
                StrSQL.AppendLine(" INNER JOIN Programmazione_Entita PE ON PP.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ")
                StrSQL.AppendLine(" WHERE  PP.Inviato >= 0")

            Else

                StrSQL.AppendLine(" DELETE PP ")
                StrSQL.AppendLine(" FROM  Programmazione_Particelle AS PP ")
                StrSQL.AppendLine(" INNER JOIN Programmazione_Entita AS PE ON PP.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ")
                StrSQL.AppendLine(" WHERE   1=1 ")


            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine("  AND   PP.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine("  AND   PE.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("  AND   PE.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.AppendLine("  AND   PP.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If Prov <> "" Then
                StrSQL.AppendLine("  AND   Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            End If

            If Com <> "" Then
                StrSQL.AppendLine("  AND   Com = '" & Agro_SQL_SaveText(Com) & "' ")
            End If

            If Sezione <> "" AndAlso Sezione <> "0" Then
                StrSQL.AppendLine("  AND   Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            End If

            If Foglio <> 0 Then
                StrSQL.AppendLine("  AND   Foglio = " & Agro_SQL_SaveNum(Foglio) & " ")
            End If

            If Numero <> 0 Then
                StrSQL.AppendLine("  AND   Numero = " & Agro_SQL_SaveNum(Numero) & " ")
            End If

            If Subalterno <> "" AndAlso Subalterno <> "0" Then
                StrSQL.AppendLine("  AND   Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class

