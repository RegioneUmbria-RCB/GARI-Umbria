Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Fertilizzanti_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################
    Public Function Leggi(ByVal Fer_Cod As Int32,
                          ByVal Fer_Des As String,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                    StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                    StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")

                    StrSQL.Append(" FROM Fertilizzanti ")

                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Fertilizzanti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Fertilizzanti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fer_Des ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.Append("SELECT * FROM Fertilizzanti ")

                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Fertilizzanti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Fertilizzanti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fer_Des ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.Append(" select f.*, isnull(c.class_fer_Cod, 0) as class_fer_Cod, isnull( c.class_fer_des, 'Classificazione non disponibile') as  class_fer_des " & vbCrLf)
                    StrSQL.Append(" from fertilizzanti f " & vbCrLf)
                    StrSQL.Append("    left join FertilizzantixClassificazioni fxc " & vbCrLf)
                    StrSQL.Append("        on f.fer_cod = fxc.Fer_Cod " & vbCrLf)
                    StrSQL.Append("    left join ClassificazioniFertilizzanti c " & vbCrLf)
                    StrSQL.Append("        on c.Class_Fer_Cod = fxc.Class_Fer_Cod")

                    StrSQL.Append(" WHERE F.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   F.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND F.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND F.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   F.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   F.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY F.Fer_Des ASC")
                    End If

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

    '#############################################################################################
    Public Function Leggi_conDitte(ByVal DITTA_COD As Integer,
                                   ByVal FER_COD As Integer,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_conDitte()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  Fertilizzanti f")
                    StrSQL.AppendLine(" LEFT JOIN FertilizzantixDitte fxd")
                    StrSQL.AppendLine(" ON f.Fer_Cod = fxd.FER_COD")
                    StrSQL.AppendLine(" LEFT JOIN Ditte d")
                    StrSQL.AppendLine(" ON fxd.DITTA_COD = d.DITTA_COD")

                    StrSQL.AppendLine(" WHERE f.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.AppendLine(" AND   f.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                    StrSQL.AppendLine(" AND   (fxd.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " OR fxd.Validita_inizio IS NULL)")
                    StrSQL.AppendLine(" AND   (fxd.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " OR fxd.Validita_Fine IS NULL)")
                    StrSQL.AppendLine(" AND   (d.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " OR d.Validita_inizio IS NULL)")
                    StrSQL.AppendLine(" AND   (d.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " OR d.Validita_Fine IS NULL)")

                    If DITTA_COD <> 0 Then
                        StrSQL.AppendLine(" AND d.DITTA_COD =  " & Agro_SQL_SaveNum(DITTA_COD) & "  ")
                    End If

                    If FER_COD <> 0 Then
                        StrSQL.AppendLine(" AND f.FER_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   f.Inviato >=0 ")
                            StrSQL.AppendLine(" AND   (fxd.Inviato >=0 OR fxd.Inviato IS NULL)")
                            StrSQL.AppendLine(" AND   (d.Inviato >=0  OR d.Inviato IS NULL)")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   fxd.Inviato =-1 ")
                            StrSQL.AppendLine(" AND   f.Inviato =-1 ")
                            StrSQL.AppendLine(" AND   d.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY f.FER_DES, d.DITTA_DES")
                    End If

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

    '#############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' a differenza della precedente filtra la tipologia e carica anche i concimi aziendali (materie prime)
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Completa(ByVal Fer_Cod As Int32,
                                   ByVal Fer_Des As String,
                                   ByVal TipoRichiesto As Int32,
                                   ByVal IncludiAziendali As Boolean,
                                   ByVal Piva As String,
                                   ByVal Mat_Cod As Int32,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal Regolamento_Cod As Integer = 0
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Completa()"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA 2007
        ' 7  =  Ammendanti + Palabili + Liquami del PAN 2012
        ' 8  =  Ammendanti + Palabili + Liquami del PAN 2016

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            If IncludiAziendali = False Then

                StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO, Fertilizzanti.Cu ")

                ' casi regolamenti PUA
                If TipoRichiesto >= 6 Then
                    StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                End If

                StrSQL.Append(" FROM Fertilizzanti ")

                Select Case TipoRichiesto
                    Case 0
                    Case 1, 2, 3, 4, 5
                        StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                        StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")
                    Case Else
                        ' Casi regolamenti PUA
                        StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                        StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                End Select

                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                If Fer_Cod <> 0 Then
                    StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                End If

                If Fer_Des <> "" Then
                    StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                End If

                Select Case TipoRichiesto

                    Case 0  'Tutti i fertilizzanti

                    Case 1  'Trattamenti Antibutteratura

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                    Case 2  'Concimazione Fogliare

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")

                    Case 3  'Fertirrigazione

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")

                    Case 4  'Concimazione Organica

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")

                    Case 5  'Concimazione inpieno Campo

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")


                    Case 6  'Ammendanti + Palabili + Liquami del PUA 2007

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5))")
                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                    Case Else 'Ammendanti + Palabili + Liquami del PUA 2012

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                End Select


                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                '--------------------------------------------------------------------------

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            Else

                If Mat_Cod = 0 Then

                    StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                    StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                    StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO, Fertilizzanti.Cu ")

                    If TipoRichiesto >= 6 Then
                        StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                    Else
                        StrSQL.Append(" ,0 AS Mat_Cod ")
                    End If

                    StrSQL.Append(" FROM Fertilizzanti ")

                    Select Case TipoRichiesto
                        Case 0

                        Case 1, 2, 3, 4, 5
                            StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                            StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")
                        Case Else
                            StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                            StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                    End Select

                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If

                    Select Case TipoRichiesto

                        Case 0  'Tutti i fertilizzanti

                        Case 1  'Trattamenti Antibutteratura

                            StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                        Case 2  'Concimazione Fogliare

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                            StrSQL.Append("         )")

                        Case 3  'Fertirrigazione

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                            StrSQL.Append("         )")

                        Case 4  'Concimazione Organica

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                            StrSQL.Append("         )")

                        Case 5  'Concimazione inpieno Campo

                            StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")

                        Case 6  'Ammendanti + Palabili + Liquami del PUA

                            StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5)) ")
                            StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                        Case Else 'Ammendanti + Palabili + Liquami dei regolamenti PUA

                            StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------
                    If Fer_Cod = 0 Then
                        ' se non è un regolamento PUA
                        If TipoRichiesto < 6 Then
                            StrSQL.Append(" UNION ")
                        End If
                    End If

                    '--------------------------------------------------


                End If

                If Fer_Cod = 0 Then

                    ' se non è un regolamento PUA
                    If TipoRichiesto < 6 Then

                        StrSQL.Append(" SELECT  DISTINCT ")
                        StrSQL.Append("         0 AS FER_COD, ")
                        StrSQL.Append("         Materie_Prime.MAT_DES AS FER_DES, ")
                        StrSQL.Append("         '' AS Denominazione, ")
                        StrSQL.Append("         Materie_Prime.N,  Materie_Prime.P2O5 , Materie_Prime.K2O, Materie_Prime.MgO, ")
                        StrSQL.Append("         0 AS Cu, Materie_Prime.MAT_COD ")

                        'If TipoRichiesto = 6 Then
                        '    StrSQL.Append(" , '' AS descrizione, 0 AS id_tp_fer ")
                        'End If

                        StrSQL.Append(" FROM    ")
                        StrSQL.Append("         Materie_Prime ")
                        StrSQL.Append(" WHERE   (Materie_Prime.MAT_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%') ")
                        StrSQL.Append(" AND     (Materie_Prime.ELEM_COD = 3)")
                        StrSQL.Append(" AND     ((Materie_Prime.Sa_Cod = -1) OR (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'))")

                        If Mat_Cod <> 0 Then
                            StrSQL.Append(" AND   (Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & ") ")
                        End If

                        If xFiltroAggiuntivo <> "" Then
                            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If

                    End If

                End If

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            End If

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

    'legge sempre tutti i fertilizzanti assieme ed aggiunge la classificazione
    Public Function Leggi_Completa_Classificazione(ByVal Fer_Cod As Int32,
                                                   ByVal Fer_Des As String,
                                                   ByVal TipoRichiesto As Int32,
                                                   ByVal IncludiAziendali As Boolean,
                                                   ByVal Piva As String,
                                                   ByVal Mat_Cod As Int32,
                                                   ByVal Validita_Inizio As Date,
                                                   ByVal Validita_Fine As Date,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   Optional ByVal Regolamento_Cod As Integer = 0
                                                   ) As DataTable


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Completa_Classificazione()"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA 2007
        ' 7  =  Ammendanti + Palabili + Liquami del PAN 2012
        ' 8  =  Ammendanti + Palabili + Liquami del PAN 2016

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable


        Try

            StrSQL.Length = 0

            If IncludiAziendali = False Then

                StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO, ")
                StrSQL.Append(" Tipologie.TP_COD , Tipologie.TP_DES ")

                '' casi regolamenti PUA
                If TipoRichiesto >= 6 Then
                    StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                End If

                StrSQL.Append(" FROM Fertilizzanti  WITH(NOLOCK)")

                StrSQL.Append("  INNER JOIN FertilizzantiXTipologie WITH(NOLOCK) ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                StrSQL.Append("  INNER JOIN Tipologie WITH(NOLOCK) ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")

                If TipoRichiesto >= 6 Then
                    ' Casi regolamenti PUA
                    StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici WITH(NOLOCK) ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                    StrSQL.Append("  INNER JOIN TipoFertilizzante WITH(NOLOCK) ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")

                End If

                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                If Fer_Cod <> 0 Then
                    StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                End If

                If Fer_Des <> "" Then
                    StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                End If

                Select Case TipoRichiesto

                    Case 0  'Tutti i fertilizzanti

                    Case 1  'Trattamenti Antibutteratura

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                    Case 2  'Concimazione Fogliare

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")

                    Case 3  'Fertirrigazione

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")

                    Case 4  'Concimazione Organica

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")

                    Case 5  'Concimazione inpieno Campo

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")


                    Case 6  'Ammendanti + Palabili + Liquami del PUA 2007

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5))")
                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                    Case Else 'Ammendanti + Palabili + Liquami del PUA 2012

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                End Select


                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                '--------------------------------------------------------------------------

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            Else

                If Mat_Cod = 0 Then

                    StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                    StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                    StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO, ")
                    StrSQL.Append(" Tipologie.TP_COD , Tipologie.TP_DES ")

                    If TipoRichiesto >= 6 Then
                        StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                    Else
                        StrSQL.Append(" ,0 AS Mat_Cod ")
                    End If

                    StrSQL.Append(" FROM Fertilizzanti WITH(NOLOCK) ")

                    StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                    StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")

                    If TipoRichiesto >= 6 Then
                        StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                        StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                    End If

                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If

                    Select Case TipoRichiesto

                        Case 0  'Tutti i fertilizzanti

                        Case 1  'Trattamenti Antibutteratura

                            StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                        Case 2  'Concimazione Fogliare

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                            StrSQL.Append("         )")

                        Case 3  'Fertirrigazione

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                            StrSQL.Append("         )")

                        Case 4  'Concimazione Organica

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                            StrSQL.Append("         )")

                        Case 5  'Concimazione inpieno Campo

                            StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")

                        Case 6  'Ammendanti + Palabili + Liquami del PUA

                            StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5)) ")
                            StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                        Case Else 'Ammendanti + Palabili + Liquami dei regolamenti PUA

                            StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------
                    If Fer_Cod = 0 Then
                        ' se non è un regolamento PUA
                        If TipoRichiesto < 6 Then
                            StrSQL.Append(" UNION ")
                        End If
                    End If

                    '--------------------------------------------------


                End If

                If Fer_Cod = 0 Then

                    ' se non è un regolamento PUA
                    If TipoRichiesto < 6 Then

                        StrSQL.Append(" SELECT  DISTINCT ")
                        StrSQL.Append("         0 AS FER_COD, ")
                        StrSQL.Append("         Materie_Prime.MAT_DES AS FER_DES, ")
                        StrSQL.Append("         '' AS Denominazione, ")
                        StrSQL.Append("         Materie_Prime.N,  Materie_Prime.P2O5 , Materie_Prime.K2O, Materie_Prime.MgO, ")
                        StrSQL.Append("         0 as TP_COD , '' as TP_DES, ")
                        StrSQL.Append("         Materie_Prime.MAT_COD ")

                        StrSQL.Append(" FROM    ")
                        StrSQL.Append("         Materie_Prime WITH(NOLOCK) ")
                        StrSQL.Append(" WHERE   (Materie_Prime.MAT_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%') ")
                        StrSQL.Append(" AND     (Materie_Prime.ELEM_COD = 3)")
                        StrSQL.Append(" AND     ((Materie_Prime.Sa_Cod = -1) OR (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'))")

                        If Mat_Cod <> 0 Then
                            StrSQL.Append(" AND   (Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & ") ")
                        End If

                        If xFiltroAggiuntivo <> "" Then
                            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If

                    End If

                End If

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            End If

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

    'utilizzata nel web service
    Public Function Leggi_WS(ByVal Fer_Cod As Int32,
                             ByVal Fer_Des As String,
                             ByVal TipoRichiesto As Int32,
                             ByVal Regolamento_Cod As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByVal IncludiTipologia As Boolean,
                             ByVal Stato_Cod As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As DataTable


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_WS()"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA 2007
        ' 7  =  Ammendanti + Palabili + Liquami del PAN 2012
        ' 8  =  Ammendanti + Palabili + Liquami del PAN 2016

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
            StrSQL.Append(" Fertilizzanti.Denominazione , ISNULL(Fertilizzanti.N,0) AS N,  ")
            StrSQL.Append(" ISNULL(Fertilizzanti.P2O5,0) AS P2O5, ISNULL(Fertilizzanti.K2O,0) AS K2O, ISNULL(Fertilizzanti.MgO,0) AS MgO, ISNULL(Fertilizzanti.Cu,0) AS Cu ")

            '(09/10/2017 fede) aggiunta indicazione se prodotto Bio
            StrSQL.Append(" , ISNULL( (SELECT 1 as Bio FROM RegolamentixFertilizzanti rf WHERE rf.fer_cod = Fertilizzanti.fer_cod AND rf.REG_COD = 4), 0) AS Bio ")

            If IncludiTipologia Then
                StrSQL.Append(" ,Tipologie.TP_COD , Tipologie.TP_DES ")
            End If

            '' casi regolamenti PUA
            If TipoRichiesto >= 6 Then
                StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                StrSQL.Append(" ,isnull(Effluenti.udm_cod,0) as udm_cod ,isnull(Effluenti.eff_cod,0) as eff_cod ")
            End If

            StrSQL.Append(" FROM Fertilizzanti ")
            StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")

            If Stato_Cod <> "" Then
                StrSQL.Append("  INNER JOIN  FertilizzantixAmbitoEstero ON Fertilizzanti.Fer_Cod = FertilizzantixAmbitoEstero.Fer_Cod AND FertilizzantixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
            End If

            If IncludiTipologia Then
                StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")
            End If

            'bio
            If Regolamento_Cod = -2 Then
                StrSQL.Append("  INNER JOIN RegolamentixFertilizzanti ON Fertilizzanti.FER_COD = RegolamentixFertilizzanti.FER_COD ")
            End If

            If TipoRichiesto >= 6 Then
                ' Casi regolamenti PUA
                StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                StrSQL.Append("  LEFT JOIN EffluentixFertilizzanti ON Fertilizzanti.fer_cod = EffluentixFertilizzanti.fer_cod ")
                StrSQL.Append("            AND  EffluentixFertilizzanti.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
                StrSQL.Append("  LEFT JOIN Effluenti ON EffluentixFertilizzanti.Eff_Cod = Effluenti.Eff_Cod and EffluentixFertilizzanti.Regolamento_Cod = Effluenti.Regolamento_Cod ")
            End If

            StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Fer_Cod <> 0 Then
                StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
            End If

            If Fer_Des <> "" Then
                StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
            End If

            'bio
            If Regolamento_Cod = -2 Then
                StrSQL.Append("  AND RegolamentixFertilizzanti.REG_COD = 4 ")
            End If

            Select Case TipoRichiesto

                Case 0  'Tutti i fertilizzanti

                Case 1  'Trattamenti Antibutteratura
                    If IncludiTipologia Then
                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")
                    End If

                Case 2  'Concimazione Fogliare

                    If IncludiTipologia Then
                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")
                    End If

                Case 3  'Fertirrigazione
                    If IncludiTipologia Then
                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")
                    End If

                Case 4  'Concimazione Organica
                    If IncludiTipologia Then
                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")
                    End If

                Case 5  'Concimazione inpieno Campo
                    If IncludiTipologia Then
                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")
                    End If

                Case 6  'Ammendanti + Palabili + Liquami del PUA 2007

                    StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5))")
                    StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

                Case Else 'Ammendanti + Palabili + Liquami del PUA 2012

                    StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")

            End Select


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Fer_Des ASC")
            End If


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

    'a differenza della precedente carica concimi di una tipologia + eventualmente quelli di tutte le direttive
    Public Function Leggi_Completa_2(ByVal Fer_Cod As Int32,
                                     ByVal Fer_Des As String,
                                     ByVal TipoRichiesto As Int32,
                                     ByVal IncludiAziendali As Boolean,
                                     ByVal Piva As String,
                                     ByVal Mat_Cod As Int32,
                                     ByVal Validita_Inizio As Date,
                                     ByVal Validita_Fine As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal IncludiFertilizzantiDirettive As Boolean = False
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Completa_2"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA 2007
        ' 7  =  Ammendanti + Palabili + Liquami del PAN 2012
        ' 8  =  Ammendanti + Palabili + Liquami del PAN 2016

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            If IncludiAziendali = False Then

                StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")

                If IncludiFertilizzantiDirettive Then
                    StrSQL.Append(" ,'' as descrizione, 0 as id_tp_fer, 0 as regolamento_cod, '' as regolamento_des ")
                End If

                StrSQL.Append(" FROM Fertilizzanti ")

                Select Case TipoRichiesto
                    Case 0
                    Case 1, 2, 3, 4, 5
                        StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                        StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")

                End Select

                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                If Fer_Cod <> 0 Then
                    StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                End If

                If Fer_Des <> "" Then
                    StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                End If

                Select Case TipoRichiesto

                    Case 0  'Tutti i fertilizzanti

                    Case 1  'Trattamenti Antibutteratura

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                    Case 2  'Concimazione Fogliare

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")

                    Case 3  'Fertirrigazione

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")

                    Case 4  'Concimazione Organica

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")

                    Case 5  'Concimazione inpieno Campo

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")


                End Select


                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If


                If IncludiFertilizzantiDirettive Then

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" UNION ")
                    End If

                    StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                    StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                    StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")

                    StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer, FertilizzantixTipoOrganici.Regolamento_Cod, PUA_Regolamenti.regolamento_des ")

                    StrSQL.Append(" FROM Fertilizzanti ")

                    StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                    StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                    StrSQL.Append("  INNER JOIN PUA_Regolamenti ON FertilizzantixTipoOrganici.regolamento_cod = PUA_Regolamenti.regolamento_cod ")


                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If


                End If


                '--------------------------------------------------------------------------

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            Else

                If Mat_Cod = 0 Then

                    StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                    StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                    StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")
                    StrSQL.Append(" ,0 AS Mat_Cod ")

                    If IncludiFertilizzantiDirettive Then
                        StrSQL.Append(" ,'' as descrizione, 0 as id_tp_fer, 0 as regolamento_cod, '' as regolamento_des ")
                    End If

                    StrSQL.Append(" FROM Fertilizzanti ")

                    Select Case TipoRichiesto
                        Case 0
                        Case 1, 2, 3, 4, 5
                            StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                            StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")
                    End Select

                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If

                    Select Case TipoRichiesto

                        Case 0  'Tutti i fertilizzanti

                        Case 1  'Trattamenti Antibutteratura

                            StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                        Case 2  'Concimazione Fogliare

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                            StrSQL.Append("         )")

                        Case 3  'Fertirrigazione

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                            StrSQL.Append("         )")

                        Case 4  'Concimazione Organica

                            StrSQL.Append(" AND     (")
                            StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                            StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                            StrSQL.Append("         )")

                        Case 5  'Concimazione inpieno Campo

                            StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")

                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------
                    If Fer_Cod = 0 Then
                        StrSQL.Append(" UNION ")
                    End If


                End If

                If Fer_Cod = 0 Then

                    StrSQL.Append(" SELECT  DISTINCT ")
                    StrSQL.Append("         0 AS FER_COD, ")
                    StrSQL.Append("         Materie_Prime.MAT_DES AS FER_DES, ")
                    StrSQL.Append("         '' AS Denominazione, ")
                    StrSQL.Append("         Materie_Prime.N,  Materie_Prime.P2O5 , Materie_Prime.K2O, Materie_Prime.MgO, ")
                    StrSQL.Append("         Materie_Prime.MAT_COD ")

                    If IncludiFertilizzantiDirettive Then
                        StrSQL.Append(" ,'' as descrizione, 0 as id_tp_fer, 0 as regolamento_cod, '' as regolamento_des ")
                    End If

                    StrSQL.Append(" FROM    ")
                    StrSQL.Append("         Materie_Prime ")
                    StrSQL.Append(" WHERE   (Materie_Prime.MAT_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%') ")
                    StrSQL.Append(" AND     (Materie_Prime.ELEM_COD = 3)")
                    StrSQL.Append(" AND     ((Materie_Prime.Sa_Cod = -1) OR (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'))")

                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND   (Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                End If

                If IncludiFertilizzantiDirettive Then

                    'If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" UNION ")
                    ' End If

                    StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                    StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                    StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")
                    StrSQL.Append(" ,0 AS Mat_Cod ")

                    StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer, FertilizzantixTipoOrganici.Regolamento_Cod, PUA_Regolamenti.regolamento_des ")

                    StrSQL.Append(" FROM Fertilizzanti ")

                    StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                    StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                    StrSQL.Append("  INNER JOIN PUA_Regolamenti ON FertilizzantixTipoOrganici.regolamento_cod = PUA_Regolamenti.regolamento_cod ")


                    StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" AND Fertilizzanti.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If Fer_Des <> "" Then
                        StrSQL.Append(" AND Fertilizzanti.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                End If

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            End If

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

    Public Function Leggi_Completa_Magazzino(ByVal TipoRichiesto As Int32,
                                             ByVal Piva As String,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByVal xFiltroAggiuntivoFerCod As String,
                                             ByVal xFiltroAggiuntivoMatCod As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal Regolamento_Cod As Integer = 0
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Completa_Magazzino()"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            If xFiltroAggiuntivoFerCod <> "" Then

                StrSQL.Append(" SELECT distinct Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")

                If TipoRichiesto = 6 OrElse TipoRichiesto = 7 Then
                    StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                Else
                    StrSQL.Append(" ,0 AS Mat_Cod ")
                End If

                StrSQL.Append(" FROM Fertilizzanti ")

                Select Case TipoRichiesto
                    Case 0
                    Case 1, 2, 3, 4, 5
                        StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                        StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")
                    Case Else   ' casi regolamenti PUA
                        StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                        StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                End Select

                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                Select Case TipoRichiesto

                    Case 0  'Tutti i fertilizzanti

                    Case 1  'Trattamenti Antibutteratura

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                    Case 2  'Concimazione Fogliare

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")

                    Case 3  'Fertirrigazione

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")

                    Case 4  'Concimazione Organica

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")

                    Case 5  'Concimazione inpieno Campo

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")

                    Case 6  'Ammendanti + Palabili + Liquami del PUA

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5))")

                    Case Else 'Ammendanti + Palabili + Liquami dei regolamenti pua

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")
                End Select

                StrSQL.Append(" AND " & xFiltroAggiuntivoFerCod)


                '--------------------------------------------------
                If xFiltroAggiuntivoMatCod <> "" Then
                    If TipoRichiesto <> 6 AndAlso TipoRichiesto <> 7 Then
                        StrSQL.Append(" UNION ")
                    End If
                End If
                '--------------------------------------------------
            End If


            If xFiltroAggiuntivoMatCod <> "" Then

                If TipoRichiesto <> 6 AndAlso TipoRichiesto <> 7 Then

                    StrSQL.Append(" SELECT  DISTINCT ")
                    StrSQL.Append("         0 AS FER_COD, ")
                    StrSQL.Append("         Materie_Prime.MAT_DES AS FER_DES, ")
                    StrSQL.Append("         '' AS Denominazione, ")
                    StrSQL.Append("         Materie_Prime.N,  Materie_Prime.P2O5 , Materie_Prime.K2O, Materie_Prime.MgO, ")
                    StrSQL.Append("         Materie_Prime.MAT_COD ")
                    StrSQL.Append(" FROM    ")
                    StrSQL.Append("         Materie_Prime ")
                    StrSQL.Append(" WHERE   (Materie_Prime.ELEM_COD = 3)")
                    StrSQL.Append(" AND     ((Materie_Prime.Sa_Cod = -1) OR (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'))")

                    StrSQL.Append(" AND " & xFiltroAggiuntivoMatCod)

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fer_Des ASC")
                    End If

                End If

            End If


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

    'legge sempre tutti i fertilizzanti assieme ed aggiunge la classificazione
    Public Function Leggi_Completa_Magazzino_Classificazione(
                                   ByVal TipoRichiesto As Int32,
                                   ByVal Piva As String,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivoFerCod As String,
                                    ByVal xFiltroAggiuntivoMatCod As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Regolamento_Cod As Integer = 0
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Completa_Magazzino_Classificazione()"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            If xFiltroAggiuntivoFerCod <> "" Then

                StrSQL.Append(" SELECT distinct Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO, ")
                StrSQL.Append(" Tipologie.TP_COD , Tipologie.TP_DES ")

                If TipoRichiesto >= 6 Then
                    StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer ")
                Else
                    StrSQL.Append(" ,0 AS Mat_Cod ")
                End If

                StrSQL.Append(" FROM Fertilizzanti ")
                StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")

                If TipoRichiesto >= 6 Then
                    StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                    StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                End If

                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                Select Case TipoRichiesto

                    Case 0  'Tutti i fertilizzanti

                    Case 1  'Trattamenti Antibutteratura

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                    Case 2  'Concimazione Fogliare

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")

                    Case 3  'Fertirrigazione

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")

                    Case 4  'Concimazione Organica

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")

                    Case 5  'Concimazione inpieno Campo

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")

                    Case 6  'Ammendanti + Palabili + Liquami del PUA

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.id_tp_fer IN (2,3,4,5))")

                    Case Else 'Ammendanti + Palabili + Liquami dei regolamenti pua

                        StrSQL.Append(" AND     (FertilizzantixTipoOrganici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")
                End Select

                StrSQL.Append(" AND " & xFiltroAggiuntivoFerCod)


                '--------------------------------------------------
                If xFiltroAggiuntivoMatCod <> "" Then
                    If TipoRichiesto <> 6 AndAlso TipoRichiesto <> 7 Then
                        StrSQL.Append(" UNION ")
                    End If
                End If
                '--------------------------------------------------
            End If


            If xFiltroAggiuntivoMatCod <> "" Then

                If TipoRichiesto < 6 Then

                    StrSQL.Append(" SELECT  DISTINCT ")
                    StrSQL.Append("         0 AS FER_COD, ")
                    StrSQL.Append("         Materie_Prime.MAT_DES AS FER_DES, ")
                    StrSQL.Append("         '' AS Denominazione, ")
                    StrSQL.Append("         Materie_Prime.N,  Materie_Prime.P2O5 , Materie_Prime.K2O, Materie_Prime.MgO, ")
                    StrSQL.Append("         0 AS TP_COD , '' AS TP_DES ")
                    StrSQL.Append("         Materie_Prime.MAT_COD ")
                    StrSQL.Append(" FROM    ")
                    StrSQL.Append("         Materie_Prime ")
                    StrSQL.Append(" WHERE   (Materie_Prime.ELEM_COD = 3)")
                    StrSQL.Append(" AND     ((Materie_Prime.Sa_Cod = -1) OR (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'))")

                    StrSQL.Append(" AND " & xFiltroAggiuntivoMatCod)

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fer_Des ASC")
                    End If

                End If

            End If


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

    Public Function Leggi_Completa_Magazzino_2(
                                   ByVal TipoRichiesto As Int32,
                                   ByVal Piva As String,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivoFerCod As String,
                                    ByVal xFiltroAggiuntivoMatCod As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal IncludiFertilizzantiDirettive As Boolean = False) As DataTable


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Completa_Magazzino_2()"

        '------------------------------------------------------------------------------------
        'TipoRichiesto

        ' 0  =  Tutti i fertilizzanti
        ' 1  =  Trattamenti Antibutteratura
        ' 2  =  Concimazione Fogliare
        ' 3  =  Fertirrigazione
        ' 4  =  Concimazione Organica
        ' 5  =  Concimazione pieno Campo
        ' 6  =  Ammendanti + Palabili + Liquami del PUA


        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            If xFiltroAggiuntivoFerCod <> "" Then

                StrSQL.Append(" SELECT distinct Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")
                StrSQL.Append(" ,0 AS Mat_Cod ")

                If IncludiFertilizzantiDirettive Then
                    StrSQL.Append(" ,'' as descrizione, 0 as id_tp_fer, 0 as regolamento_cod, '' as regolamento_des ")
                End If

                StrSQL.Append(" FROM Fertilizzanti ")

                Select Case TipoRichiesto
                    Case 0
                    Case 1, 2, 3, 4, 5
                        StrSQL.Append("  INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD ")
                        StrSQL.Append("  INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD ")
                End Select

                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                Select Case TipoRichiesto

                    Case 0  'Tutti i fertilizzanti

                    Case 1  'Trattamenti Antibutteratura

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 1)")

                    Case 2  'Concimazione Fogliare

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 2) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 3)    ")
                        StrSQL.Append("         )")

                    Case 3  'Fertirrigazione

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 3) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 5)    ")
                        StrSQL.Append("         )")

                    Case 4  'Concimazione Organica

                        StrSQL.Append(" AND     (")
                        StrSQL.Append("         (Tipologie.TP_COD = 6) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 7) OR ")
                        StrSQL.Append("         (Tipologie.TP_COD = 8)    ")
                        StrSQL.Append("         )")

                    Case 5  'Concimazione inpieno Campo

                        StrSQL.Append(" AND     (Tipologie.TP_COD = 4)")

                End Select

                StrSQL.Append(" AND " & xFiltroAggiuntivoFerCod)

                '--------------------------------------------------
                If xFiltroAggiuntivoMatCod <> "" Then
                    StrSQL.Append(" UNION ")
                End If
                '--------------------------------------------------
            End If


            If xFiltroAggiuntivoMatCod <> "" Then

                StrSQL.Append(" SELECT  DISTINCT ")
                StrSQL.Append("         0 AS FER_COD, ")
                StrSQL.Append("         Materie_Prime.MAT_DES AS FER_DES, ")
                StrSQL.Append("         '' AS Denominazione, ")
                StrSQL.Append("         Materie_Prime.N,  Materie_Prime.P2O5 , Materie_Prime.K2O, Materie_Prime.MgO, ")
                StrSQL.Append("         Materie_Prime.MAT_COD ")

                If IncludiFertilizzantiDirettive Then
                    StrSQL.Append(" ,'' as descrizione, 0 as id_tp_fer, 0 as regolamento_cod, '' as regolamento_des ")
                End If

                StrSQL.Append(" FROM    ")
                StrSQL.Append("         Materie_Prime ")
                StrSQL.Append(" WHERE   (Materie_Prime.ELEM_COD = 3)")
                StrSQL.Append(" AND     ((Materie_Prime.Sa_Cod = -1) OR (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'))")

                StrSQL.Append(" AND " & xFiltroAggiuntivoMatCod)

                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.Append(" ORDER BY Fer_Des ASC")
                End If

            End If


            If IncludiFertilizzantiDirettive Then

                StrSQL.Append(" UNION ")

                StrSQL.Append(" SELECT Fertilizzanti.Fer_Cod , Fertilizzanti.Fer_Des,  ")
                StrSQL.Append(" Fertilizzanti.Denominazione , Fertilizzanti.N,  ")
                StrSQL.Append(" Fertilizzanti.P2O5 , Fertilizzanti.K2O, Fertilizzanti.MgO ")
                StrSQL.Append(" ,0 AS Mat_Cod ")

                StrSQL.Append(" ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer, FertilizzantixTipoOrganici.Regolamento_Cod, PUA_Regolamenti.regolamento_des ")

                StrSQL.Append(" FROM Fertilizzanti ")

                StrSQL.Append("  INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD ")
                StrSQL.Append("  INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer ")
                StrSQL.Append("  INNER JOIN PUA_Regolamenti ON FertilizzantixTipoOrganici.regolamento_cod = PUA_Regolamenti.regolamento_cod ")


                StrSQL.Append(" WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                StrSQL.Append(" AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                StrSQL.Append(" AND " & xFiltroAggiuntivoFerCod)

            End If

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



    Public Function DittaDes_from_FerCod(ByVal FerCod As Integer,
                                         ByRef OUTPUT_Ditta_Cod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim DtFer As DataTable
        Dim dt As DataTable

        Dim objFer As New AgronicaCoreMetaSchemaDAL.FertilizzantixDitte_R

        Dim objDitte As New AgronicaCoreMetaSchemaDAL.Ditte_R

        'Leggo le informazioni 		
        DtFer = objFer.Leggi(0, CInt(FerCod),
                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "", objParametri)

        objFer = Nothing

        If DtFer IsNot Nothing AndAlso DtFer.Rows.Count > 0 Then

            'Leggo le informazioni 		
            dt = objDitte.Leggi(CInt(DtFer.Rows(0).Item("Ditta_Cod")),
                                "", "", "", objParametri)

            OUTPUT_Ditta_Cod = CInt(DtFer.Rows(0).Item("Ditta_Cod"))
            objDitte = Nothing

            'Se il recordset non e' nullo
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Return CStr(dt.Rows(0).Item("Ditta_Des"))

            Else

                OUTPUT_Ditta_Cod = 0
                Return ""

            End If
        Else

            OUTPUT_Ditta_Cod = 0
            Return ""

        End If


        'Elimino i recordset (anche se non necessario in ASP.NET)
        dt = Nothing
        DtFer = Nothing

    End Function

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FerCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	27/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function FerDes_from_FerCod(ByVal FerCod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Dim dt As DataTable

        'Recupero le informazioni		
        dt = Leggi(CInt(FerCod), "", AGRODATAINIZIO, AGRODATAFINE,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            Return dt.Rows(0).Item("Fer_Des")

        End If

    End Function


    '#############################################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Fer_Cod"></param>
    ''' <param name="Denominazione"></param>
    ''' <param name="N"></param>
    ''' <param name="P2O5"></param>
    ''' <param name="K2O"></param>
    ''' <param name="MgO"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	27/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub Titoli_from_FerCod(ByVal Fer_Cod As Integer,
                                  ByRef Denominazione As String,
                                  ByRef N As String,
                                  ByRef P2O5 As String,
                                  ByRef K2O As String,
                                  ByRef MgO As String,
                                  ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable

        Denominazione = ""
        N = ""
        P2O5 = ""
        K2O = ""
        MgO = ""


        'Recupero le informazioni		
        dt = Leggi(CInt(Fer_Cod), "", AGRODATAINIZIO, AGRODATAFINE,
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            Denominazione = dt.Rows(0).Item("Denominazione")
            N = dt.Rows(0).Item("N")
            P2O5 = dt.Rows(0).Item("P2O5")
            K2O = dt.Rows(0).Item("K2O")
            MgO = dt.Rows(0).Item("MgO")

        End If

    End Sub


    '#############################################################################################
    Public Function Classificazioni_Fertilizzante_from_FerCod(
                            ByVal Fer_Cod As Int32,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As String


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT  ClassificazioniFertilizzanti.Class_Fer_Des ")
                    StrSQL.Append("FROM    FertilizzantixClassificazioni  INNER JOIN ")
                    StrSQL.Append(" ClassificazioniFertilizzanti ON FertilizzantixClassificazioni.Class_Fer_Cod = ClassificazioniFertilizzanti.Class_Fer_Cod ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" WHERE FertilizzantixClassificazioni.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   FertilizzantixClassificazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   FertilizzantixClassificazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Class_Fer_Des ASC")
                    End If

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
            Return ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        If dt.Rows.Count = 0 Then
            Return ""
        Else
            Dim i As Integer
            Dim strClassificazioni As String = ""
            For i = 0 To dt.Rows.Count - 1
                strClassificazioni = strClassificazioni & dt.Rows(i).Item("Class_Fer_Des") & ", "
            Next
            If strClassificazioni <> "" Then
                strClassificazioni = Left(strClassificazioni, strClassificazioni.Length - 2)
            End If

            Return strClassificazioni
        End If

    End Function

    '#############################################################################################
    Public Function FormulazioniFertilizzanti_FORM_FER_DES(
                            ByVal Fer_Cod As Int32,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As String

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT FormulazioniFertilizzanti.FORM_FER_DES AS Form_Fer_Des " &
                                    " FROM FertilizzantixFormulazioni INNER JOIN " &
                                    " FormulazioniFertilizzanti ON FertilizzantixFormulazioni.FORM_FER_COD = FormulazioniFertilizzanti.FORM_FER_COD ")

                    If Fer_Cod <> 0 Then
                        StrSQL.Append(" WHERE FertilizzantixFormulazioni.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   FertilizzantixFormulazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   FertilizzantixFormulazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FORM_FER_DES ASC")
                    End If

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
            Return ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count = 0 Then
            Return ""
        Else
            Dim i As Integer
            Dim strClassificazioni As String = ""

            For i = 0 To dt.Rows.Count - 1
                strClassificazioni = strClassificazioni & dt.Rows(i).Item("Form_Fer_Des") & vbCrLf
            Next

            Return strClassificazioni
        End If

    End Function


    Public Function FertilizzantixFormulazioni_Leggi(
                            ByVal Fer_Cod As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.FertilizzantixFormulazioni_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append(" Select FertilizzantixFormulazioni.FER_COD, ")
            StrSQL.Append(" FertilizzantixFormulazioni.FORM_FER_COD, FertilizzantixFormulazioni.UDM_Cod, ")
            StrSQL.Append(" FormulazioniFertilizzanti.FORM_FER_DES, UnitaMisura.UDM_SIM, UnitaMisura.UDM_des ")

            StrSQL.Append("   From FertilizzantixFormulazioni  ")
            StrSQL.Append("   inner Join FormulazioniFertilizzanti on FormulazioniFertilizzanti.FORM_FER_COD = FertilizzantixFormulazioni.FORM_FER_COD     ")
            StrSQL.Append("   inner Join UnitaMisura on UnitaMisura.UDM_COD=FertilizzantixFormulazioni.UDM_Cod ")

            If Fer_Cod <> "" Then
                StrSQL.Append(" WHERE FertilizzantixFormulazioni.FER_Cod IN (" & Agro_SQL_Save_Clausola_IN(Fer_Cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   FertilizzantixFormulazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   FertilizzantixFormulazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY FER_COD ASC")
            End If

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


    Public Function Leggi_conCosti(ByVal FER_COD As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_conCosti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            'Fertilizzanti SENZA COSTI

            StrSQL.Append(" (SELECT Fertilizzanti.*, 0 AS Prezzo_Unitario, " &
                          "  '01/01/1900' as Validita_Inizio_Prezzo, '31/12/2100' as Validita_Fine_Prezzo, " &
                          "  0 as Udm_Cod_Prezzo, '' as Udm_Sim_Prezzo " &
                          " FROM  Fertilizzanti " &
                          " WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                          " AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND NOT EXISTS ( ")
            StrSQL.Append("                 SELECT * ")
            StrSQL.Append("                 FROM Prodotti_Costi ")
            StrSQL.Append("                 WHERE Elem_Cod=3 ")
            StrSQL.Append("                 AND Fertilizzanti.Fer_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 )")

            If FER_COD <> 0 Then
                StrSQL.Append(" AND Fertilizzanti.FER_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Fertilizzanti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Fertilizzanti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(") UNION ALL (")


            'Formulati CON COSTI

            StrSQL.Append(" SELECT Fertilizzanti.*, Prodotti_Costi.Prezzo_Unitario, " &
                          "  Prodotti_Costi.validita_inizio as Validita_Inizio_Prezzo, Prodotti_Costi.validita_fine as Validita_Fine_Prezzo, " &
                          "  Prodotti_Costi.Udm_Cod as Udm_Cod_Prezzo, UnitaMisura.UDM_SIM as Udm_Sim_Prezzo " &
                          "  FROM Fertilizzanti INNER JOIN Prodotti_Costi ON Fertilizzanti.Fer_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 " &
                          "  INNER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.UDM_COD " &
                          "  WHERE Fertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                          "  AND   Fertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " &
                          "  AND   Elem_Cod=3 ")

            If FER_COD <> 0 Then
                StrSQL.Append(" AND Fertilizzanti.FeR_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Fertilizzanti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Fertilizzanti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ) ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Fertilizzanti.FeR_DES ASC ")
            End If

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



    Public Function Leggi_Fertilizzanti_X_Stato_Cod(ByVal Fer_Cod As Int32,
                                                    ByVal Fer_Des As String,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal Stato_Cod As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Fertilizzanti_R.Leggi_Fertilizzanti_X_Stato_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" select f.*, isnull(c.class_fer_Cod, 0) as class_fer_Cod, isnull( c.class_fer_des, 'Classificazione non disponibile') as  class_fer_des ")
            StrSQL.AppendLine(" from fertilizzanti f ")
            StrSQL.AppendLine("left join FertilizzantixClassificazioni fxc ")
            StrSQL.AppendLine("on f.fer_cod = fxc.Fer_Cod ")
            StrSQL.AppendLine("left join ClassificazioniFertilizzanti c ")
            StrSQL.AppendLine("on c.Class_Fer_Cod = fxc.Class_Fer_Cod")
            StrSQL.AppendLine("inner join  FertilizzantixAmbitoEstero ON f.Fer_Cod = FertilizzantixAmbitoEstero.Fer_Cod")

            StrSQL.AppendLine(" WHERE F.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   F.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Fer_Cod <> 0 Then
                StrSQL.AppendLine(" AND F.FER_Cod =" & Agro_SQL_SaveNum(Fer_Cod) & " ")
            End If

            If Fer_Des <> "" Then
                StrSQL.AppendLine(" AND F.FER_DES LIKE '%" & Agro_SQL_SaveText(Fer_Des) & "%' ")
            End If

            If Stato_Cod <> "" Then
                StrSQL.AppendLine(" AND FertilizzantixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   F.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   F.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY F.Fer_Des ASC")
            End If

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

End Class
