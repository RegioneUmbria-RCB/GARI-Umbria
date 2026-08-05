Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class IndiciMaturitaxSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Usare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="IND_MAT_COD"></param>
    ''' <param name="VEG_COD"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	19/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal IND_MAT_COD As Integer,
                          ByVal VEG_COD As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.IndiciMaturitaxSpecie_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  IndiciMaturitaxSpecieVegetali , SpecieVegetali , IndiciMaturita ")
                    StrSQL.Append(" WHERE IndiciMaturitaxSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")

                    If IND_MAT_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.IND_MAT_COD =  " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
                    End If

                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato >= 0 ")
                            StrSQL.Append(" AND     SpecieVegetali.Inviato >= 0 ")
                            StrSQL.Append(" AND     IndiciMaturita.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato = -1 ")
                            StrSQL.Append(" AND     SpecieVegetali.Inviato = -1 ")
                            StrSQL.Append(" AND     IndiciMaturita.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES ASC, IndiciMaturita.IND_MAT_DES ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT IndiciMaturitaxSpecieVegetali.veg_cod, veg_des, IndiciMaturitaxSpecieVegetali.ind_mat_cod, ind_mat_des  ")
                    StrSQL.Append(" FROM  IndiciMaturitaxSpecieVegetali  ")
                    StrSQL.Append(" INNER JOIN SpecieVegetali ON IndiciMaturitaxSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD  ")
                    StrSQL.Append(" INNER JOIN IndiciMaturita ON IndiciMaturitaxSpecieVegetali.IND_MAT_COD = IndiciMaturita.IND_MAT_COD  ")
                    StrSQL.Append(" WHERE 1 = 1  ")
    
                    If IND_MAT_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.IND_MAT_COD =  " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
                    End If

                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato >= 0 ")
                            StrSQL.Append(" AND     SpecieVegetali.Inviato >= 0 ")
                            StrSQL.Append(" AND     IndiciMaturita.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato = -1 ")
                            StrSQL.Append(" AND     SpecieVegetali.Inviato = -1 ")
                            StrSQL.Append(" AND     IndiciMaturita.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES, IndiciMaturita.IND_MAT_DES ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta


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


    Public Function Leggi_X_UDM(ByVal IND_MAT_COD As Integer,
                                ByVal VEG_COD As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.IndiciMaturitaxSpecie_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  IndiciMaturitaxSpecieVegetali , SpecieVegetali , IndiciMaturita, MisuraxIndiciMaturita , UnitaMisura ")

            StrSQL.Append(" WHERE IndiciMaturitaxSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   IndiciMaturita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   IndiciMaturita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   MisuraxIndiciMaturita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   MisuraxIndiciMaturita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")
            StrSQL.Append(" AND   MisuraxIndiciMaturita.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")
            StrSQL.Append(" AND   MisuraxIndiciMaturita.UDM_COD = UnitaMisura.UDM_COD ")



            If IND_MAT_COD <> 0 Then
                StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.IND_MAT_COD =  " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato >= 0 ")
                    StrSQL.Append(" AND     SpecieVegetali.Inviato >= 0 ")
                    StrSQL.Append(" AND     IndiciMaturita.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato = -1 ")
                    StrSQL.Append(" AND     SpecieVegetali.Inviato = -1 ")
                    StrSQL.Append(" AND     IndiciMaturita.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES ASC, IndiciMaturita.IND_MAT_DES ASC ")
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


    '##############################################################################################
    Public Function Leggi_ParametroQualita(ByVal VEG_COD As Long,
                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.IndiciMaturitaxSpecie_r.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  IndiciMaturitaxSpecieVegetali , SpecieVegetali , IndiciMaturita ")
                    StrSQL.Append(" WHERE IndiciMaturitaxSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
                    StrSQL.Append(" AND   IndiciMaturitaxSpecieVegetali.Flag_Raccolta = 1" & "  ")


                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturitaxSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     IndiciMaturitaxSpecieVegetali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES ASC, IndiciMaturita.IND_MAT_DES ASC")
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

    '##############################################################################################
    Public Function ParametroRaccolta_from_VegCod(ByVal Veg_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Dim dt As DataTable

        'Recupero le informazioni
        dt = Leggi_ParametroQualita(CInt(Veg_Cod),
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "", objParametri)

        'Se il recordset non è chiuso allora ...
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Ind_Mat_Cod")
        End If

    End Function

End Class
