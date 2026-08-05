Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ImpresexParticelle2_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID As Integer,
                          ByVal PIVA As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Part_Cod As Integer,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Integer,
                          ByVal NUMERO As Integer,
                          ByVal SUBALTERNO As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0,
                          Optional ByVal includiZVN As Boolean = False
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ImpresexParticelle.* ")

                    If includiZVN Then
                        strSql.AppendLine(" , CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN ")
                    End If

                    strSql.AppendLine(" FROM  ImpresexParticelle  ")
                    If includiZVN Then

                        strSql.AppendLine(" LEFT JOIN ZonexParticelle ON ImpresexParticelle.PROV =  ZonexParticelle.PROV ")
                        strSql.AppendLine("                          AND ImpresexParticelle.COM =  ZonexParticelle.COM ")
                        strSql.AppendLine("                          AND ImpresexParticelle.SEZIONE =  ZonexParticelle.SEZIONE ")
                        strSql.AppendLine("                          AND ImpresexParticelle.FOGLIO =  ZonexParticelle.FOGLIO ")
                        strSql.AppendLine("                          AND ImpresexParticelle.NUMERO =  ZonexParticelle.NUMERO ")
                        strSql.AppendLine("                          AND ImpresexParticelle.SUBALTERNO =  ZonexParticelle.SUBALTERNO ")
                        strSql.AppendLine("                          AND ZonexParticelle.Zona_Cod = " & enum_Zone.ZVN & " ")

                    End If
                    strSql.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If ID <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Piva, Sa_Cod, PROV, COM, Sezione, Foglio, Numero, Subalterno ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, ")
                    strSql.AppendLine("       ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("       ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    strSql.AppendLine("       ImpresexParticelle.TitoloPossesso as xTitoloPossesso ")
                    strSql.AppendLine(" FROM  ImpresexParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If ID <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Part_Cod <> 0 Then
                        strSql.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY ImpresexParticelle.Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  ParticelleCatastali.*, ")
                    strSql.AppendLine(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    strSql.AppendLine(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso,")
                    strSql.AppendLine("       CASE ImpreseXParticelle.TitoloPossesso WHEN " & enum_TitoloPossesso.Proprieta & " THEN 'Proprietà' WHEN " & enum_TitoloPossesso.Comodato & "2 THEN 'Comodato d''uso' ")
                    strSql.AppendLine("                                              WHEN " & enum_TitoloPossesso.AffittoContratto & " THEN 'Affitto con contratto' WHEN " & enum_TitoloPossesso.AffittoSenzaContratto & " THEN 'Affitto senza contratto' ")
                    strSql.AppendLine("                                              WHEN " & enum_TitoloPossesso.InContoTerzi & " THEN 'In conto terzi' ELSE 'Altro' END AS xTitoloPossessoDes, ")

                    strSql.AppendLine(" ImpresexParticelle.Sup_Condotta, ImpresexParticelle.Sup_Spandibile, ImpresexParticelle.Sup_Divieto, ")
                    strSql.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
                    strSql.AppendLine(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome ")

                    If includiZVN Then
                        strSql.AppendLine(" , CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN ")
                    End If

                    strSql.AppendLine(" FROM  ImpreseXParticelle ")
                    strSql.AppendLine(" INNER JOIN ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine("             AND ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine("             AND ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    strSql.AppendLine(" INNER JOIN ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM ")
                    strSql.AppendLine(" INNER JOIN Lista_Province ON ISTAT.PROV = Lista_Province.PROV ")
                    strSql.AppendLine(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
                    strSql.AppendLine(" INNER JOIN Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA ")

                    If includiZVN Then

                        strSql.AppendLine(" LEFT JOIN ZonexParticelle ON ImpresexParticelle.PROV =  ZonexParticelle.PROV ")
                        strSql.AppendLine("                          AND ImpresexParticelle.COM =  ZonexParticelle.COM ")
                        strSql.AppendLine("                          AND ImpresexParticelle.SEZIONE =  ZonexParticelle.SEZIONE ")
                        strSql.AppendLine("                          AND ImpresexParticelle.FOGLIO =  ZonexParticelle.FOGLIO ")
                        strSql.AppendLine("                          AND ImpresexParticelle.NUMERO =  ZonexParticelle.NUMERO ")
                        strSql.AppendLine("                          AND ImpresexParticelle.SUBALTERNO =  ZonexParticelle.SUBALTERNO ")
                        strSql.AppendLine("                          AND ZonexParticelle.Zona_Cod = " & enum_Zone.ZVN & " ")

                    End If

                    strSql.AppendLine(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    strSql.AppendLine(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If ID <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Part_Cod <> 0 Then
                        strSql.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################

    Public Function LeggiPeriodiValidita(ByVal PIVA As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal PROV As String,
                                         ByVal COM As String,
                                         ByVal SEZIONE As String,
                                         ByVal FOGLIO As Integer,
                                         ByVal NUMERO As Integer,
                                         ByVal SUBALTERNO As String,
                                         ByVal ListaPeriodi As List(Of ImpresexParticelle2_Periodo),
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ImpresexParticelle.* ")
            strSql.AppendLine(" ,ISTAT.LOCALITA ,ISTAT.COMUNI_PROV ")
            strSql.AppendLine(" FROM  ImpresexParticelle  ")
            strSql.AppendLine(" LEFT JOIN ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM ")

            strSql.AppendLine(" WHERE ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If PROV <> "" Then
                strSql.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                strSql.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                strSql.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                strSql.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                strSql.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                strSql.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If ListaPeriodi.Count > 0 Then

                strSql.AppendLine(" AND ( ")

                Dim i As Integer = 0

                For Each periodo In ListaPeriodi

                    i += 1

                    If Not IsNothing(periodo.Validita_Inizio) AndAlso Not IsNothing(periodo.Validita_Fine) Then

                        If i > 1 Then
                            strSql.AppendLine(" OR ")
                        End If

                        strSql.AppendLine(" ( ImpresexParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(periodo.Validita_Fine) & " AND ")
                        strSql.AppendLine("   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(periodo.Validita_Inizio) & " ) ")

                    End If

                Next

                strSql.AppendLine(" ) ")

            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Piva, Sa_Cod, PROV, COM, Sezione, Foglio, Numero, Subalterno ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################
    Public Function Leggi2(ByVal ID As Integer, _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Part_Cod As Integer, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Integer, _
                            ByVal NUMERO As Integer, _
                            ByVal SUBALTERNO As String, _
                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT *, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    StrSQL.AppendLine("       ImpresexParticelle.TitoloPossesso as xTitoloPossesso ")
                    StrSQL.AppendLine(" FROM  ImpresexParticelle , ParticelleCatastali ")
                    StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If ID <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Part_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY ImpresexParticelle.Validita_inizio ASC")
                    End If
                    

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT *, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    StrSQL.AppendLine("       ImpresexParticelle.TitoloPossesso as xTitoloPossesso ")
                    StrSQL.AppendLine(" FROM  ImpresexParticelle , ParticelleCatastali ")
                    StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If ID <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Part_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY ImpresexParticelle.Validita_inizio ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

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

    '#####################################################################
    Public Function Leggi_Superfici(ByVal ID As Int32, _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Part_Cod As Int32, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Int32, _
                            ByVal NUMERO As Int32, _
                            ByVal SUBALTERNO As String, _
                                ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Leggi_Superfici()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT *, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    StrSQL.AppendLine("       ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ")

                    StrSQL.AppendLine("       CASE ImpreseXParticelle.TitoloPossesso WHEN " & enum_TitoloPossesso.Proprieta & " THEN 'Proprietà' WHEN " & enum_TitoloPossesso.Comodato & "2 THEN 'Comodato d''uso' ")
                    StrSQL.AppendLine("                                              WHEN " & enum_TitoloPossesso.AffittoContratto & " THEN 'Affitto con contratto' WHEN " & enum_TitoloPossesso.AffittoSenzaContratto & " THEN 'Affitto senza contratto' ")
                    StrSQL.AppendLine("                                              WHEN " & enum_TitoloPossesso.InContoTerzi & " THEN 'In conto terzi' ELSE 'Altro' END AS TitoloPossesso_Des, ")

                    StrSQL.AppendLine("ISNULL( (SELECT TOP 1  Superficie ")
                    StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
                    StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
                    StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 1) ")
                    StrSQL.AppendLine("    , 0) AS Sup_Seminabile,  ")

                    StrSQL.AppendLine("ISNULL( (SELECT TOP 1  Superficie ")
                    StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
                    StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
                    StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 3) ")
                    StrSQL.AppendLine("    , 0) AS Sup_Unar,  ")

                    StrSQL.AppendLine("ISNULL( (SELECT TOP 1  ImpresexParticelle_Codici.Val_Cod ")
                    StrSQL.AppendLine("		    FROM ImpresexParticelle_Codici ")
                    StrSQL.AppendLine("		    WHERE ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
                    StrSQL.AppendLine("	        AND ImpresexParticelle_Codici.Id_Cod = " & enum_DatiAnagrafici_CodiciAnagrafe.Catasto & " ), '0') AS Modifica ")

                    StrSQL.AppendLine(" FROM       ImpreseXParticelle  ")
                    StrSQL.AppendLine(" INNER JOIN ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
                    StrSQL.AppendLine("            ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.AppendLine("            ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.AppendLine(" INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM ")

                    StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If ID <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Part_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If




                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     (ParticelleCatastali.inviato >= 0)  ")
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     (ParticelleCatastali.inviato = -1)  ")
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY ImpresexParticelle.Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT *, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.AppendLine("       ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
                    StrSQL.AppendLine("       ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ")

                    StrSQL.AppendLine("       CASE ImpreseXParticelle.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato d''uso' ")
                    StrSQL.AppendLine("                                              WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' ")
                    StrSQL.AppendLine("                                              WHEN 5 THEN 'In conto terzi' ELSE 'Altro' END AS TitoloPossesso_Des, ")

                    StrSQL.AppendLine("ISNULL( (SELECT TOP 1  Superficie ")
                    StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
                    StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
                    StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 1) ")
                    StrSQL.AppendLine("    , 0) AS Sup_Seminabile,  ")

                    StrSQL.AppendLine("ISNULL( (SELECT TOP 1  Superficie ")
                    StrSQL.AppendLine("		    FROM ParticelleCatastalixEleggibilitaParticelle EP ")
                    StrSQL.AppendLine("		    WHERE EP.PROV = ParticelleCatastali.PROV AND EP.COM = ParticelleCatastali.COM AND ")
                    StrSQL.AppendLine("		    EP.SEZIONE = ParticelleCatastali.SEZIONE AND EP.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.AppendLine("         EP.NUMERO = ParticelleCatastali.NUMERO And EP.SUBALTERNO = ParticelleCatastali.SUBALTERNO And EP.Eleggibilita_Cod = 3) ")
                    StrSQL.AppendLine("    , 0) AS Sup_Unar,  ")

                    StrSQL.AppendLine("ISNULL( (SELECT TOP 1  ImpresexParticelle_Codici.Val_Cod ")
                    StrSQL.AppendLine("		    FROM ImpresexParticelle_Codici ")
                    StrSQL.AppendLine("		    WHERE ImpresexParticelle_Codici.ID = ImpresexParticelle.ID ")
                    StrSQL.AppendLine("	        AND ImpresexParticelle_Codici.Id_Cod = " & enum_DatiAnagrafici_CodiciAnagrafe.Catasto & " ), '0') AS Modifica ")

                    StrSQL.AppendLine(" FROM       ImpreseXParticelle  ")
                    StrSQL.AppendLine(" INNER JOIN ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
                    StrSQL.AppendLine("            ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
                    StrSQL.AppendLine("            ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    StrSQL.AppendLine(" INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM ")

                    StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If ID <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
                    End If

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Part_Cod <> 0 Then
                        StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     (ParticelleCatastali.inviato >= 0)  ")
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     (ParticelleCatastali.inviato = -1)  ")
                            StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY ImpresexParticelle.Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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

    '###################################################################################
    Public Function Esiste_CentroxParticella(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Prov As String,
                                             ByVal Com As String,
                                             ByVal Sezione As String,
                                             ByVal Foglio As Integer,
                                             ByVal Numero As Integer,
                                             ByVal Subalterno As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle.Esiste_CentroxParticella()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Flag_Esiste As Boolean = False

        If Sezione = "" Then
            Sezione = "0"
        End If
        If Subalterno = "" Then
            Subalterno = "0"
        End If

        Try

            dt = LeggixChiave(0,
                              Piva,
                              Sa_Cod,
                              Prov,
                              Com,
                              Sezione,
                              Foglio,
                              Numero,
                              Subalterno,
                              enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                              xFiltroAggiuntivo, xOrderBy,
                              objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Flag_Esiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Flag_Esiste

    End Function

    '#######################################################################
    Public Function LeggixChiave(ByVal ID As Integer,
                                 ByVal PIVA As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal PROV As String,
                                 ByVal COM As String,
                                 ByVal SEZIONE As String,
                                 ByVal FOGLIO As Integer,
                                 ByVal NUMERO As Integer,
                                 ByVal SUBALTERNO As String,
                                 ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.LeggixChiave()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  
        '   Sa_Cod = 0                  =>
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT  PIVA, sa_cod, TitoloPossesso, Sup_Condotta, Validita_Inizio, Validita_Fine ")
                    StrSQL.AppendLine(" FROM  ImpresexParticelle  ")
                    StrSQL.AppendLine(" WHERE   ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    StrSQL.AppendLine(" AND     ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    strSql.AppendLine(" AND     ImpresexParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    strSql.AppendLine(" AND     ImpresexParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    strSql.AppendLine(" AND     ImpresexParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    strSql.AppendLine(" AND     ImpresexParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM  ImpresexParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    strSql.AppendLine(" AND   ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    strSql.AppendLine(" AND   ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    strSql.AppendLine(" AND   ImpresexParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
                    strSql.AppendLine(" AND   ImpresexParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                    strSql.AppendLine(" AND   ImpresexParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                    strSql.AppendLine(" AND   ImpresexParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (ParticelleCatastali.inviato >= 0)  ")
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (ParticelleCatastali.inviato = -1)  ")
                            strSql.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '###########################################################################
    Public Function Esiste_Particella(ByVal Piva As String, _
                                      ByVal SaCod As String, _
                                      ByVal Prov As String, _
                                      ByVal Com As String, _
                                      ByVal Sezione As String, _
                                      ByVal Foglio As Integer, _
                                      ByVal Numero As Integer, _
                                      ByVal Subalterno As String, _
                                      ByRef Data_Modifica As Date, _
                                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Esiste_Particella()"

        '====================================================================================
        'Parametri opzionali :
        '   Nessuno
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False

        If Sezione = "" Then
            Sezione = "0"
        End If
        If Subalterno = "" Then
            Subalterno = "0"
        End If

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM    ImpreseXParticelle ")
                    StrSQL.AppendLine(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.AppendLine(" AND     sa_cod = " & Agro_SQL_SaveNum(SaCod) & " ")
                    StrSQL.AppendLine(" AND     Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
                    StrSQL.AppendLine(" AND     Com = '" & Agro_SQL_SaveText(Com) & "' ")
                    StrSQL.AppendLine(" AND     Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
                    StrSQL.AppendLine(" AND     Foglio = " & Foglio & " ")
                    StrSQL.AppendLine(" AND     Numero = " & Numero & " ")
                    StrSQL.AppendLine(" AND     Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM    ImpreseXParticelle ")
                    StrSQL.AppendLine(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.AppendLine(" AND     sa_cod = " & Agro_SQL_SaveNum(SaCod) & " ")
                    StrSQL.AppendLine(" AND     Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
                    StrSQL.AppendLine(" AND     Com = '" & Agro_SQL_SaveText(Com) & "' ")
                    StrSQL.AppendLine(" AND     Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
                    StrSQL.AppendLine(" AND     Foglio = " & Foglio & " ")
                    StrSQL.AppendLine(" AND     Numero = " & Numero & " ")
                    StrSQL.AppendLine(" AND     Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then

                    Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))
                    bRet = True

                Else

                    Data_Modifica = Nothing
                    bRet = False

                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return bRet

    End Function

    '################################################################################
    'verifica l'esistenza con la query DistinctCentri_Particella
    Public Function Esiste_Particella_2(ByVal Piva As String,
                                        ByVal Part_Cod As Integer,
                                        ByVal SaCod As String,
                                        ByVal Prov As String,
                                        ByVal Com As String,
                                        ByVal Sezione As String,
                                        ByVal Foglio As Integer,
                                        ByVal Numero As Integer,
                                        ByVal Subalterno As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim dt As DataTable
        Dim Esiste As Boolean = False

        dt = DistinctCentri_Particella(Piva,
                                       Part_Cod,
                                       Prov,
                                       Com,
                                       Sezione,
                                       Foglio,
                                       Numero,
                                       Subalterno,
                                       "", "",
                                       objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Esiste = True
        End If

        Return Esiste

    End Function

    '#######################################################################
    'recupera i centri nei quali si trova una particella
    '(indipendentemente dagli intervalli dei possessi)
    'e recupera la superficie totale della particella
    Public Function DistinctCentri_Particella(ByVal Piva As String,
                                              ByVal Part_Cod As Integer,
                                              ByVal PROV As String,
                                              ByVal COM As String,
                                              ByVal SEZIONE As String,
                                              ByVal FOGLIO As Integer,
                                              ByVal NUMERO As Integer,
                                              ByVal SUBALTERNO As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.DistinctCentri_Particella()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = 0 -> valorizzare allora la chiave della particella
        '   chiave particella -> valorizzare allora il part_cod
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT ImpresexParticelle.Piva, ImpresexParticelle.Sa_Cod, ")
            StrSQL.AppendLine("       ParticelleCatastali.Ettari, ParticelleCatastali.Are, ParticelleCatastali.Centiare ")
            StrSQL.AppendLine(" FROM  ImpresexParticelle , ParticelleCatastali ")
            StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Part_Cod <> 0 Then
                StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ImpresexParticelle.Piva, ImpresexParticelle.Sa_Cod ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = "Particella: " + PROV + " " + COM + " " + SEZIONE + " " + CStr(FOGLIO) + " " + CStr(NUMERO) + " " + SUBALTERNO + vbCrLf
            MessaggioErrore += "Piva: " + CStr(Piva) + vbCrLf
            MessaggioErrore += "Part_Cod: " + CStr(Part_Cod) + vbCrLf
            MessaggioErrore += "Errore: " + ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function


    '###########################################################################
    'non considera il sa_cod (x import casalasco)
    Public Function Esiste_Particella_3(ByVal Piva As String,
                                        ByVal Prov As String,
                                        ByVal Com As String,
                                        ByVal Sezione As String,
                                        ByVal Foglio As Integer,
                                        ByVal Numero As Integer,
                                        ByVal Subalterno As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Esiste_Particella_3()"

        '====================================================================================
        'Parametri opzionali :
        '   Nessuno
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False

        If Sezione = "" Then
            Sezione = "0"
        End If
        If Subalterno = "" Then
            Subalterno = "0"
        End If

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM    ImpreseXParticelle ")
            StrSQL.AppendLine(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND     Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
            StrSQL.AppendLine(" AND     Com = '" & Agro_SQL_SaveText(Com) & "' ")
            StrSQL.AppendLine(" AND     Sezione = '" & Agro_SQL_SaveText(Sezione) & "' ")
            StrSQL.AppendLine(" AND     Foglio = " & Foglio & " ")
            StrSQL.AppendLine(" AND     Numero = " & Numero & " ")
            StrSQL.AppendLine(" AND     Subalterno = '" & Agro_SQL_SaveText(Subalterno) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    bRet = True
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return bRet

    End Function


    '####################################################################
    'legge ImpresexParticelle, ParticelleCatastali, Centri_Aziendali, Imprese
    Public Function Leggi3(ByVal ID As Int32,
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Part_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal dataRiferimento As Date = AGRODATAINIZIO
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.Leggi3()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ImpresexParticelle.ID, Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome, ImpresexParticelle.PIVA, ImpresexParticelle.sa_cod, ")
            StrSQL.AppendLine("         ImpresexParticelle.PROV,ImpresexParticelle.COM, ImpresexParticelle.SEZIONE, ImpresexParticelle.FOGLIO, ImpresexParticelle.NUMERO,ImpresexParticelle.SUBALTERNO,")
            StrSQL.AppendLine("         ImpresexParticelle.TitoloPossesso, ImpresexParticelle.Validita_Inizio, ImpresexParticelle.Validita_Fine, ImpresexParticelle.Sup_Condotta,")
            StrSQL.AppendLine("         ParticelleCatastali.PART_COD, ParticelleCatastali.ETTARI, ParticelleCatastali.ARE, ParticelleCatastali.CENTIARE ")
            StrSQL.AppendLine("         , ParticelleCatastali.Proprietario ")

            StrSQL.AppendLine(" FROM  ImpresexParticelle  ")

            StrSQL.AppendLine(" INNER JOIN  ParticelleCatastali ")
            StrSQL.AppendLine(" ON   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            StrSQL.AppendLine(" INNER JOIN  Centri_Aziendali ")
            StrSQL.AppendLine(" ON   ImpresexParticelle.PIVA = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.sa_cod = Centri_Aziendali.sa_cod ")

            StrSQL.AppendLine(" INNER JOIN  Imprese ")
            StrSQL.AppendLine(" ON   Imprese.PIVA = Centri_Aziendali.PIVA ")

            StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
            End If

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Part_Cod <> 0 Then
                StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If dataRiferimento <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(dataRiferimento) & " AND ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(dataRiferimento) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ImpresexParticelle.Validita_inizio ASC")
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


    '####################################################################
    'legge ImpresexParticelle, ParticelleCatastali, Centri_Aziendali, Imprese, 
    Public Function LeggiJoinCentriIndirizzi(ByVal ID As Int32, _
                                            ByVal PIVA As String, _
                                            ByVal Sa_Cod As Int32, _
                                            ByVal Part_Cod As Int32, _
                                            ByVal PROV As String, _
                                            ByVal COM As String, _
                                            ByVal SEZIONE As String, _
                                            ByVal FOGLIO As Int32, _
                                            ByVal NUMERO As Int32, _
                                            ByVal SUBALTERNO As String, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.LeggiJoinCentriIndirizzi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Imprese.Rag_Soc, ImpresexParticelle.PIVA, ImpresexParticelle.sa_cod, ")
            StrSQL.AppendLine("         ImpresexParticelle.PROV,ImpresexParticelle.COM, ImpresexParticelle.SEZIONE, ImpresexParticelle.FOGLIO, ImpresexParticelle.NUMERO,ImpresexParticelle.SUBALTERNO,")
            StrSQL.AppendLine("         ImpresexParticelle.TitoloPossesso, ImpresexParticelle.Validita_Inizio, ImpresexParticelle.Validita_Fine, ImpresexParticelle.Sup_Condotta,")
            StrSQL.AppendLine("         ParticelleCatastali.PART_COD, ParticelleCatastali.ETTARI, ParticelleCatastali.ARE, ParticelleCatastali.CENTIARE, ")

            StrSQL.AppendLine(" Centri_Aziendali.Sa_Nome, Centri_Aziendali.sup_bosco, Centri_Aziendali.sup_prati,   ")
            StrSQL.AppendLine(" Centri_Aziendali.validita_inizio as CENTRO_validita_inizio, Centri_Aziendali.validita_fine as CENTRO_validita_fine, ")
            StrSQL.AppendLine("  CentrixIndirizzi.Tipo_Indirizzo, ")
            StrSQL.AppendLine(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
            StrSQL.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")

            StrSQL.AppendLine(" FROM  ImpresexParticelle  ")

            StrSQL.AppendLine(" INNER JOIN  ParticelleCatastali ")
            StrSQL.AppendLine(" ON   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            StrSQL.AppendLine(" INNER JOIN  Centri_Aziendali ")
            StrSQL.AppendLine(" ON   ImpresexParticelle.PIVA = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.sa_cod = Centri_Aziendali.sa_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            StrSQL.AppendLine(" INNER JOIN  Imprese ")
            StrSQL.AppendLine(" ON   Imprese.PIVA = Centri_Aziendali.PIVA ")

            StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
            End If

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Part_Cod <> 0 Then
                StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function LeggiJoinCentriIndirizzi2(ByVal ID As Integer,
                                            ByVal PIVA As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Part_Cod As Integer,
                                            ByVal PROV As String,
                                            ByVal COM As String,
                                            ByVal SEZIONE As String,
                                            ByVal FOGLIO As Integer,
                                            ByVal NUMERO As Integer,
                                            ByVal SUBALTERNO As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R.LeggiJoinCentriIndirizzi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Imprese.Rag_Soc, ImpresexParticelle.PIVA, ImpresexParticelle.sa_cod, ")
            StrSQL.AppendLine("         ImpresexParticelle.PROV,ImpresexParticelle.COM, ImpresexParticelle.SEZIONE, ImpresexParticelle.FOGLIO, ImpresexParticelle.NUMERO,ImpresexParticelle.SUBALTERNO,")
            StrSQL.AppendLine("         ImpresexParticelle.TitoloPossesso, ImpresexParticelle.Validita_Inizio, ImpresexParticelle.Validita_Fine, ImpresexParticelle.Sup_Condotta,")
            StrSQL.AppendLine("         ParticelleCatastali.PART_COD, ParticelleCatastali.ETTARI, ParticelleCatastali.ARE, ParticelleCatastali.CENTIARE, ")

            StrSQL.AppendLine(" Centri_Aziendali.Sa_Nome, Centri_Aziendali.sup_bosco, Centri_Aziendali.sup_prati,   ")
            StrSQL.AppendLine(" Centri_Aziendali.validita_inizio as CENTRO_validita_inizio, Centri_Aziendali.validita_fine as CENTRO_validita_fine, ")
            StrSQL.AppendLine("  CentrixIndirizzi.Tipo_Indirizzo, ")
            StrSQL.AppendLine(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
            StrSQL.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")

            StrSQL.AppendLine(" FROM  ImpresexParticelle  ")

            StrSQL.AppendLine(" INNER JOIN  ParticelleCatastali ")
            StrSQL.AppendLine(" ON   ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            StrSQL.AppendLine(" INNER JOIN  Centri_Aziendali ")
            StrSQL.AppendLine(" ON   ImpresexParticelle.PIVA = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.sa_cod = Centri_Aziendali.sa_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

            StrSQL.AppendLine(" INNER JOIN  Imprese ")
            StrSQL.AppendLine(" ON   Imprese.PIVA = Centri_Aziendali.PIVA ")

            StrSQL.AppendLine(" WHERE ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
            End If

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Part_Cod <> 0 Then
                StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function Leggi_conMacrousi(ByVal PIVA As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal PROV As String,
                                      ByVal COM As String,
                                      ByVal SEZIONE As String,
                                      ByVal FOGLIO As Integer,
                                      ByVal NUMERO As Integer,
                                      ByVal SUBALTERNO As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional ByVal includiZVN As Boolean = False
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Leggi_conMacrousi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  
        '   Sa_Cod = 0                  =>
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  ParticelleCatastali.*, ")
            StrSQL.AppendLine(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
            StrSQL.AppendLine(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
            StrSQL.AppendLine(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
            StrSQL.AppendLine(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome, ")

            StrSQL.AppendLine(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des ")

            If includiZVN Then
                StrSQL.AppendLine(" , CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN ")
            End If

            StrSQL.AppendLine(" FROM   Macrousi INNER JOIN ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod RIGHT OUTER JOIN ")
            StrSQL.AppendLine("        ImpreseXParticelle INNER JOIN ")
            StrSQL.AppendLine("        ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND  ")
            StrSQL.AppendLine("        ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.AppendLine("        ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
            StrSQL.AppendLine("        ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
            StrSQL.AppendLine("        Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
            StrSQL.AppendLine("        Imprese ON ImpreseXParticelle.PIVA = Imprese.PIVA INNER JOIN ")
            StrSQL.AppendLine("        Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ON  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM AND  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO AND  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            If includiZVN Then

                StrSQL.AppendLine(" LEFT JOIN ZonexParticelle ON ImpresexParticelle.PROV =  ZonexParticelle.PROV ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.COM =  ZonexParticelle.COM ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.SEZIONE =  ZonexParticelle.SEZIONE ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.FOGLIO =  ZonexParticelle.FOGLIO ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.NUMERO =  ZonexParticelle.NUMERO ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.SUBALTERNO =  ZonexParticelle.SUBALTERNO ")
                StrSQL.AppendLine("                          AND ZonexParticelle.Zona_Cod = " & enum_Zone.ZVN & " ")

            End If

            StrSQL.AppendLine(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
            StrSQL.AppendLine(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.AppendLine(" AND ParticelleCatastalixMacrousi.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            'If Part_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            'End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function Leggi_conMacrousiUtilizzi(ByVal PIVA As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal PROV As String,
                                              ByVal COM As String,
                                              ByVal SEZIONE As String,
                                              ByVal FOGLIO As Integer,
                                              ByVal NUMERO As Integer,
                                              ByVal SUBALTERNO As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal includiZVN As Boolean = False
                                              ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Leggi_conMacrousiUtilizzi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  
        '   Sa_Cod = 0                  =>
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  ParticelleCatastali.*, ")
            StrSQL.AppendLine(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
            StrSQL.AppendLine(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
            StrSQL.AppendLine(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
            StrSQL.AppendLine(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome, ")

            StrSQL.AppendLine(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des, ")

            StrSQL.AppendLine(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea,'') as Veg_Cod_Agea, ISNULL(Codifica_SpecieVegetali_AGEA.Veg_Des_Agea,'') as Veg_Des_Agea, ")
            StrSQL.AppendLine(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea,'') as Cul_Cod_Agea, ISNULL(Codifica_SpecieVegetali_AGEA.Cul_Des_Agea,'') as Cul_Des_Agea, ")
            StrSQL.AppendLine(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Superficie,0) AS Sup_Utilizzo ")

            'StrSQL.AppendLine(" FROM   Codici_Anagrafe RIGHT OUTER JOIN ")
            'StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo ON Codici_Anagrafe.codice = ParticelleCatastalixMacrousixUtilizzo.Id_Cod LEFT OUTER JOIN ")
            'StrSQL.AppendLine(" GruppoVarietale ON ParticelleCatastalixMacrousixUtilizzo.Grva_Cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
            'StrSQL.AppendLine(" GruppoFinalita ON ParticelleCatastalixMacrousixUtilizzo.Grfi_Cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
            'StrSQL.AppendLine(" Cultivar ON ParticelleCatastalixMacrousixUtilizzo.Cul_Cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
            'StrSQL.AppendLine(" SpecieVegetali ON ParticelleCatastalixMacrousixUtilizzo.Veg_Cod = SpecieVegetali.Veg_Cod RIGHT OUTER JOIN ")

            If includiZVN Then
                StrSQL.AppendLine(" , CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN 'NO' ELSE 'SI' END as ZVN ")
            End If

            StrSQL.AppendLine(" FROM   Codifica_SpecieVegetali_AGEA RIGHT OUTER JOIN ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo ON Codifica_SpecieVegetali_AGEA.Veg_Cod_Agea = ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea AND ")
            StrSQL.AppendLine(" Codifica_SpecieVegetali_AGEA.Cul_Cod_Agea = ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea RIGHT OUTER JOIN ")
            StrSQL.AppendLine(" Macrousi INNER JOIN ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod ON  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.PROV = ParticelleCatastalixMacrousi.PROV AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.COM = ParticelleCatastalixMacrousi.COM AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.SEZIONE = ParticelleCatastalixMacrousi.SEZIONE AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.FOGLIO = ParticelleCatastalixMacrousi.FOGLIO AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.NUMERO = ParticelleCatastalixMacrousi.NUMERO AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.SUBALTERNO = ParticelleCatastalixMacrousi.SUBALTERNO AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod RIGHT OUTER JOIN ")
            StrSQL.AppendLine(" ImpreseXParticelle INNER JOIN ")
            StrSQL.AppendLine(" ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND  ")
            StrSQL.AppendLine(" ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.AppendLine(" ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
            StrSQL.AppendLine(" ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
            StrSQL.AppendLine(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
            StrSQL.AppendLine(" Imprese ON ImpreseXParticelle.PIVA = Imprese.PIVA INNER JOIN ")
            StrSQL.AppendLine(" Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ON  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.AppendLine(" ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO And ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

            If includiZVN Then

                StrSQL.AppendLine(" LEFT JOIN ZonexParticelle ON ImpresexParticelle.PROV =  ZonexParticelle.PROV ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.COM =  ZonexParticelle.COM ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.SEZIONE =  ZonexParticelle.SEZIONE ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.FOGLIO =  ZonexParticelle.FOGLIO ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.NUMERO =  ZonexParticelle.NUMERO ")
                StrSQL.AppendLine("                          AND ImpresexParticelle.SUBALTERNO =  ZonexParticelle.SUBALTERNO ")
                StrSQL.AppendLine("                          AND ZonexParticelle.Zona_Cod = " & enum_Zone.ZVN & " ")

            End If


            StrSQL.AppendLine(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
            StrSQL.AppendLine(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            'If Part_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            'End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################

    Public Function Esistono_Macrousi(ByVal PIVA As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal PROV As String,
                                      ByVal COM As String,
                                      ByVal SEZIONE As String,
                                      ByVal FOGLIO As Integer,
                                      ByVal NUMERO As Integer,
                                      ByVal SUBALTERNO As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Esistono_Macrousi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  
        '   Sa_Cod = 0                  =>
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable
        Dim Esistono As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  ParticelleCatastali.*, ")
            StrSQL.AppendLine(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
            StrSQL.AppendLine(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
            StrSQL.AppendLine(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
            StrSQL.AppendLine(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome, ")

            StrSQL.AppendLine(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des ")

            StrSQL.AppendLine(" FROM   Macrousi INNER JOIN ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod INNER JOIN ")
            StrSQL.AppendLine("        ImpreseXParticelle INNER JOIN ")
            StrSQL.AppendLine("        ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND  ")
            StrSQL.AppendLine("        ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.AppendLine("        ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
            StrSQL.AppendLine("        ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
            StrSQL.AppendLine("        Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
            StrSQL.AppendLine("        Imprese ON ImpreseXParticelle.PIVA = Imprese.PIVA INNER JOIN ")
            StrSQL.AppendLine("        Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ON  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM AND  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO AND  ")
            StrSQL.AppendLine("        ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")


            StrSQL.AppendLine(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
            StrSQL.AppendLine(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.AppendLine(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If (PIVA <> "") Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.AppendLine(" AND ParticelleCatastalixMacrousi.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            'If Part_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            'End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Esistono = True
            Else
                Esistono = False
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Esistono = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Esistono

    End Function

    Public Function Leggi_Contatti(ByVal ID As Integer,
                                   ByVal PIVA As String,
                                   ByVal Sa_Cod As Int32,
                                   ByVal PROV As String,
                                   ByVal COM As String,
                                   ByVal SEZIONE As String,
                                   ByVal FOGLIO As Int32,
                                   ByVal NUMERO As Int32,
                                   ByVal SUBALTERNO As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Leggi_Contatti()"

        Dim MessaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" SELECT  IPC.* ")
            stb.Append(" FROM    ImpresexParticelle_Contatti IPC ")
            stb.Append(" INNER JOIN ImpreseXParticelle IP ON IP.ID = IPC.ID ")
            stb.Append(" WHERE   (IPC.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            stb.Append(" AND     (IPC.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If ID <> 0 Then
                stb.Append(" AND IPC.ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If PIVA <> "" Then
                stb.Append(" AND IP.Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND IP.Sa_Cod = " & Agro_SQL_SaveNum(Trim(Sa_Cod)))
            End If

            If PROV <> "" Then
                stb.Append(" AND IP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                stb.Append(" AND IP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                stb.Append(" AND IP.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                stb.Append(" AND IP.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                stb.Append(" AND IP.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                stb.Append(" AND IP.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   IP.Inviato >=0 ")
                    stb.Append(" AND   IPC.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   IP.Inviato =-1 ")
                    stb.Append(" AND   IPC.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY IPC.Validita_Fine ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class ImpresexParticelle2_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##########################################################
    Public Function Scrivi_2(ByVal PIVA As String,
                             ByVal Sa_Cod As Integer,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Integer,
                             ByVal NUMERO As Integer,
                             ByVal SUBALTERNO As String,
                             ByVal Partita_Catastale As String,
                             ByVal TitoloPossesso As Integer,
                             ByVal Sup_Condotta As Decimal,
                             ByVal FinestraTemp_Inizio As Date,
                             ByVal FinestraTemp_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_creazione As String = "",
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal Sup_Spandibile As Decimal = 0,
                             Optional ByVal Sup_Divieto As Decimal = 0
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Scrivi_2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO ImpreseXParticelle(       ")
            strSql.AppendLine("                    PIVA, Sa_Cod, PROV, COM, SEZIONE, ")
            strSql.AppendLine("                    FOGLIO, NUMERO, SUBALTERNO,  Partita_Catastale, TitoloPossesso,    ")
            strSql.AppendLine("                    Sup_Condotta,    Sup_Spandibile,    Sup_Divieto, ")
            strSql.AppendLine("                    Inviato,            DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            strSql.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.AppendLine("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.AppendLine("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Partita_Catastale) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TitoloPossesso) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Condotta) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Spandibile) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Divieto) & "  ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")
            strSql.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ModificaSupCondottaImpreseXParticelle_LeggendolaDaappezzamentiXparticelle(
                    ByVal PIVA As String,
                    ByVal Sa_Cod As Int32,
                    ByVal xFiltroAggiuntivo As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Modifica_2()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" ")
            stb.AppendLine(" update ip ")
            stb.AppendLine(" set Sup_Condotta = ap.area ")
            stb.AppendLine(" from ( ")
            stb.AppendLine(" select piva, SA_COD, PROV, COM, SEZIONE, FOGLIO, NUMERO , SUBALTERNO, sum(area) as area ")
            stb.AppendLine(" from AppezzamentiXParticelle    ")
            stb.AppendLine(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            stb.AppendLine(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            stb.AppendLine(" group by piva, SA_COD, PROV, COM, SEZIONE, FOGLIO, NUMERO , SUBALTERNO  ")
            stb.AppendLine(" ) ap ")
            stb.AppendLine(" inner join ")
            stb.AppendLine(" ImpreseXParticelle ip ")
            stb.AppendLine("    on ap.PIVA = ip.PIVA  ")
            stb.AppendLine("    and ap.SA_COD = ip.sa_cod  ")
            stb.AppendLine("    and ap.PROV = ip.PROV  ")
            stb.AppendLine("    and ap.com = ip.com  ")
            stb.AppendLine("    and ap.SEZIONE = ip.SEZIONE  ")
            stb.AppendLine("    and ap.FOGLIO = ip.FOGLIO  ")
            stb.AppendLine("    and ap.NUMERO = ip.NUMERO  ")
            stb.AppendLine("    and ap.SUBALTERNO = ip.SUBALTERNO  ")
            stb.AppendLine("  ")

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '############################################################  
    Public Function Modifica_2(ByVal ID As Integer,
                               ByVal PIVA As String,
                               ByVal Sa_Cod As Integer,
                               ByVal PROV As String,
                               ByVal COM As String,
                               ByVal SEZIONE As String,
                               ByVal FOGLIO As Integer,
                               ByVal NUMERO As Integer,
                               ByVal SUBALTERNO As String,
                               ByVal Partita_Catastale As String,
                               ByVal TitoloPossesso As Integer,
                               ByVal Sup_Condotta As Decimal,
                               ByVal Validita_Inizio As Date,
                               ByVal Validita_Fine As Date,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal Sup_Spandibile As Decimal? = Nothing,
                               Optional ByVal Sup_Divieto As Decimal? = Nothing
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Modifica_2()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE ImpresexParticelle SET ")
            StrSQL.AppendLine("        Partita_Catastale  = '" & Agro_SQL_SaveText(Partita_Catastale) & "'")
            StrSQL.AppendLine("       ,TitoloPossesso     =  " & Agro_SQL_SaveNum(TitoloPossesso) & " ")
            StrSQL.AppendLine("       ,Inviato            =  0 ")
            StrSQL.AppendLine("       ,DataInvio          =  Null ")
            StrSQL.AppendLine("       ,Data_Modifica      =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("       ,Validita_Inizio    =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("       ,Validita_Fine      =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("       ,Sup_Condotta       =  " & Agro_SQL_SaveNum(Sup_Condotta))

            If Not IsNothing(Sup_Spandibile) Then
                strSql.AppendLine("   ,Sup_Spandibile =  " & Agro_SQL_SaveNum(Sup_Spandibile) & " ")
            End If

            If Not IsNothing(Sup_Divieto) Then
                strSql.AppendLine("   ,Sup_Divieto =  " & Agro_SQL_SaveNum(Sup_Divieto) & " ")
            End If

            StrSQL.AppendLine(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.AppendLine(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.AppendLine(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.AppendLine(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.AppendLine(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.AppendLine(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.AppendLine(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
            End If
            '---------------------------------------------


            '----------------------------------------------------------------------
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

    '############################################################  
    Public Function Modifica_SupPUA(ByVal ID As Int32,
                                    ByVal PIVA As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Int32,
                                    ByVal NUMERO As Int32,
                                    ByVal SUBALTERNO As String,
                                    ByVal Sup_Divieto As Decimal,
                                    ByVal Sup_Spandibile As Decimal,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef dataRiferimento As Date
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Modifica_SupPUA()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE ImpresexParticelle SET ")
            StrSQL.AppendLine("        Sup_Divieto        =  " & Agro_SQL_SaveNum(Sup_Divieto))
            StrSQL.AppendLine("       ,Sup_Spandibile     =  " & Agro_SQL_SaveNum(Sup_Spandibile))
            StrSQL.AppendLine(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.AppendLine(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.AppendLine(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.AppendLine(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.AppendLine(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.AppendLine(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.AppendLine(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ImpresexParticelle.ID = " & Agro_SQL_SaveNum(ID) & "  ")
            End If
            '---------------------------------------------

            If dataRiferimento <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND ImpresexParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(dataRiferimento) & " AND ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(dataRiferimento) & " ")
            End If

            '----------------------------------------------------------------------
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

    '#################################################################
    Public Function Cancella(ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Cancella()"

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


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE ImpresexParticelle ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  PIVA <> '0' ")
                StrSQL.AppendLine(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     ImpresexParticelle ")
                StrSQL.AppendLine(" WHERE  PIVA <> '0' ")

            End If

            ' Il filtro è comune a entrambe le query, è inutile ripetere il codice!!!!!!!

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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


    '################################################################
    Public Function Cancella_Possesso(ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal TitoloPossesso As Int32,
                            ByVal Validita_Inizio As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Cancella_Possesso()"

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


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE ImpresexParticelle ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  PIVA <> '0' ")
                StrSQL.AppendLine(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     ImpresexParticelle ")
                StrSQL.AppendLine(" WHERE  PIVA <> '0' ")

            End If

            ' Il filtro è comune a entrambe le query, è inutile ripetere il codice!!!!!!!

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If

            StrSQL.AppendLine(" AND  TitoloPossesso = " & Agro_SQL_SaveNum(TitoloPossesso) & " ")

            StrSQL.AppendLine(" AND  Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

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


    '################################################################
    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                             ByVal Validita_Inizio As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexParticelle2_W.AggiornaValiditaInizio()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE ImpresexParticelle SET ")
            StrSQL.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine(" WHERE   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.AppendLine(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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


    '########################################################
    Public Function AggiornaValiditaFine(ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexParticelle2_W.AggiornaValiditaFine()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE ImpresexParticelle SET ")
            StrSQL.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.AppendLine(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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



    '##############################################################
    Public Function AggiornaValidita(ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                             ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ImpresexParticelle2_W.AggiornaValidita()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE ImpresexParticelle SET ")
            StrSQL.AppendLine("       UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("      ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

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

    Public Function Aggiungi_Aggiorna(ByRef id_part_ritorno As Integer, ByVal PIVA As String,
                                ByVal Sa_Cod As Int32,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Partita_Catastale As String,
                             ByVal TitoloPossesso As Int32,
                             ByVal Sup_Condotta As Decimal,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                  , Optional ByVal Data_creazione As Date = AGRODATAINIZIO _
                  , Optional ByVal Data_modifica As Date = AGRODATAINIZIO _
                  , Optional ByVal username_creazione As String = "" _
                  , Optional ByVal username_modifica As String = "" _
                     , Optional ByVal validita_inizio As Date = AGRODATAINIZIO _
                      , Optional ByVal validita_fine As Date = AGRODATAFINE
                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.impresexparticelle2_W.Aggiungi_Aggiorna()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Try
            id_part_ritorno = 0
            Dim ImpresexParticelle2_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim dtPC As DataTable = ImpresexParticelle2_R.Leggi(0, PIVA, Sa_Cod, 0, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)
            If dtPC.Rows.Count = 1 Then
                id_part_ritorno = dtPC.Rows(0).Item("id")


                If validita_fine = AGRODATAFINE Then
                    validita_fine = dtPC.Rows(0).Item("validita_fine")
                End If

                If validita_inizio = AGRODATAINIZIO Then
                    validita_inizio = dtPC.Rows(0).Item("validita_inizio")
                End If

                Modifica_2(0,
                            PIVA,
                            Sa_Cod,
                            PROV,
                            COM,
                            SEZIONE,
                            FOGLIO,
                            NUMERO,
                            SUBALTERNO,
                           Partita_Catastale,
                           TitoloPossesso,
                           Sup_Condotta,
                           validita_inizio,
                           validita_fine,
                           xFiltroAggiuntivo,
                           objParametri)

            ElseIf dtPC.Rows.Count = 0 Then
                'Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'id_part_ritorno = objSequenze.NuovoId_Tabella("ParticelleCAtastali", 0, 0, objParametri)
                Scrivi_2(PIVA,
                            Sa_Cod,
                            PROV,
                            COM,
                            SEZIONE,
                            FOGLIO,
                            NUMERO,
                            SUBALTERNO,
                           Partita_Catastale,
                           TitoloPossesso,
                           Sup_Condotta,
                           validita_inizio,
                           validita_fine,
                           objParametri)

                dtPC = ImpresexParticelle2_R.Leggi(0, PIVA, Sa_Cod, 0, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)
                id_part_ritorno = dtPC.Rows(0).Item("id")

            Else
                Throw New Exception("La query deve selezionare al massimo un solo record")
            End If

            xRisp = True
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    'Cancellazione di una determinata associazione.
    '(Gias Lan: utilizzata nel caso di eliminazione di UNA particella catastale)
    '##############################################################################################
    Public Function Cancella_Associazione(ByVal PIVA As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal PROV As String,
                                          ByVal COM As String,
                                          ByVal SEZIONE As String,
                                          ByVal FOGLIO As Integer,
                                          ByVal NUMERO As Integer,
                                          ByVal SUBALTERNO As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Cancella_Associazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE ImpresexParticelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Validita_Fine = " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

                StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
                StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ImpresexParticelle ")
                StrSQL.Append(" WHERE    PIVA <> '0' ")

                StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
                StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

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

    '##############################################################################################
    Public Function Cancella_Codici(ByVal Id_Cod As Integer, ByVal PIVA As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal PROV As String,
                                          ByVal COM As String,
                                          ByVal SEZIONE As String,
                                          ByVal FOGLIO As Integer,
                                          ByVal NUMERO As Integer,
                                          ByVal SUBALTERNO As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Cancella_Codici()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim ID As Integer = 0

        Dim ImpresexParticelle2_R As New ImpresexParticelle2_R
        Dim dtPC As DataTable = ImpresexParticelle2_R.Leggi(0, PIVA, Sa_Cod, 0, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)

        If dtPC.Rows.Count = 1 Then
            ID = CInt(dtPC.Rows(0).Item("id"))
        Else
            Return False
        End If

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.AppendLine(" UPDATE ImpresexParticelle_Codici ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE Inviato >= 0")
            Else
                StrSQL.AppendLine(" DELETE FROM ImpresexParticelle_Codici ")
                StrSQL.AppendLine(" WHERE 1=1 ")
            End If

            StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")

            If Id_Cod <> 0 Then
                StrSQL.AppendLine(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            End If

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

    '##############################################################################################
    Public Function Cancella_Contatti(ByVal Cod_RisUm As Integer,
                                        ByVal PIVA As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal PROV As String,
                                        ByVal COM As String,
                                        ByVal SEZIONE As String,
                                        ByVal FOGLIO As Integer,
                                        ByVal NUMERO As Integer,
                                        ByVal SUBALTERNO As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Cancella_Contatti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim ID As Integer = 0

        Dim ImpresexParticelle2_R As New ImpresexParticelle2_R
        Dim dtPC As DataTable = ImpresexParticelle2_R.Leggi(0, PIVA, Sa_Cod, 0, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)

        If dtPC.Rows.Count = 1 Then
            ID = CInt(dtPC.Rows(0).Item("id"))
        Else
            Return False
        End If

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.AppendLine(" UPDATE ImpresexParticelle_Contatti ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE Inviato >= 0")
            Else
                StrSQL.AppendLine(" DELETE FROM ImpresexParticelle_Contatti ")
                StrSQL.AppendLine(" WHERE 1=1 ")
            End If

            StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")

            If Cod_RisUm <> 0 Then
                StrSQL.AppendLine(" AND Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            End If

            'StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            'StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            'StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            'StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            'StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            'StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            'StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            'StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

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

    Public Function Modifica_Centro(ByVal Piva As String,
                                    ByVal Sa_Cod_OLD As Integer,
                                    ByVal Sa_Cod_NEW As Integer,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Integer,
                                    ByVal NUMERO As Integer,
                                    ByVal SUBALTERNO As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W.Modifica_Centro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE ImpresexParticelle SET " &
                        "        sa_cod  = " & Agro_SQL_SaveNum(Sa_Cod_NEW) &
                        "       ,Data_Modifica      =  " & Agro_SQL_SaveDateTime(Now) &
                        "       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'" &
                        " WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        " AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod_OLD))

            If PROV <> "" Then
                StrSQL.AppendLine(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.AppendLine(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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

Public Class ImpresexParticelle2_Periodo

    Public Validita_Inizio As Date? = Nothing
    Public Validita_Fine As Date? = Nothing

End Class
