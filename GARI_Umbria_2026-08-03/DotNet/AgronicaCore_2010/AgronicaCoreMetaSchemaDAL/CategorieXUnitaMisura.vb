Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class CategorieXUnitaMisura_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Elem_Cod As Integer, _
                          ByVal Udm_Cod As Integer, _
                          ByVal Cau_Mov As String, _
                          ByVal Flag_Cantina As Boolean, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.CategorieXUnitaMisura_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Elem_Cod, Udm_Cod ")
                    StrSQL.Append(" FROM    CategorieXUnitaMisura ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    If Flag_Cantina = False Then
                        StrSQL.Append(" AND   Elem_Cod > 0  ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND   Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND   Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
                    End If

                    If Cau_Mov <> "" Then
                        If Cau_Mov <> AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_ANIMALE Then
                            StrSQL.Append(" AND   Elem_Cod <> " & Agro_SQL_SaveNum(AgronicaCoreDataProvider.CostantiPersonalizzate.ZOO_CONSISTENZA) & "   ")
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Elem_Cod, Udm_Cod ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                    'StrSQL = ""
                    'StrSQL += " SELECT *  "
                    'StrSQL += " FROM CategorieXUnitaMisura "
                    'StrSQL += " INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = CategorieXUnitaMisura.Udm_Cod  "
                    'StrSQL += " INNER JOIN CategorieMagazzino ON CategorieMagazzino.Elem_Cod = CategorieXUnitaMisura.Elem_Cod "
                    'StrSQL += " "
                    'StrSQL += "  "

                    'If Flag_Cantina = False Then
                    '    If i = 0 Then
                    '        StrSQL += " WHERE  ( CategorieXUnitaMisura.Elem_Cod > 0 ) "
                    '        i = i + 1
                    '    Else
                    '        StrSQL += " AND  ( CategorieXUnitaMisura.Elem_Cod > 0 ) "
                    '    End If
                    'End If

                    'If Elem_Cod <> 0 Then
                    '    If i = 0 Then
                    '        StrSQL += " WHERE   (CategorieXUnitaMisura.Elem_Cod = " & SQL_SaveNum(Elem_Cod) & ")   "
                    '        i = i + 1
                    '    Else
                    '        StrSQL += " AND   (CategorieXUnitaMisura.Elem_Cod = " & SQL_SaveNum(Elem_Cod) & ")   "
                    '    End If
                    'End If

                    'If Udm_Cod <> 0 Then
                    '    If i = 0 Then
                    '        StrSQL += " WHERE   (CategorieXUnitaMisura.Udm_Cod = " & SQL_SaveNum(Udm_Cod) & ")   "
                    '        i = i + 1
                    '    Else
                    '        StrSQL += " AND   (CategorieXUnitaMisura.Udm_Cod = " & SQL_SaveNum(Udm_Cod) & ")   "
                    '    End If
                    'End If

                    'If Cau_Mov <> "" Then
                    '    If Cau_Mov <> CAU_ANIMALE Then
                    '        If i = 0 Then
                    '            StrSQL += " WHERE   (CategorieXUnitaMisura.Elem_Cod <> " & SQL_SaveNum(ZOO_CONSISTENZA) & ")   "
                    '            i = i + 1
                    '        Else
                    '            StrSQL += " AND   (CategorieXUnitaMisura.Elem_Cod <> " & SQL_SaveNum(ZOO_CONSISTENZA) & ")   "
                    '        End If
                    '    End If
                    'End If

                    'If FiltroAggiuntivo_SenzaAND <> "" Then
                    '    If i = 0 Then
                    '        StrSQL += " WHERE   " + FiltroAggiuntivo_SenzaAND
                    '        i = i + 1
                    '    Else
                    '        StrSQL += " AND   " + FiltroAggiuntivo_SenzaAND
                    '    End If
                    'End If

                    'StrSQL += " ORDER BY Udm_des "

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM    CategorieXUnitaMisura ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    If Flag_Cantina = False Then
                        StrSQL.Append(" AND   Elem_Cod > 0  ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND   Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND   Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
                    End If

                    If Cau_Mov <> "" Then
                        If Cau_Mov <> AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_ANIMALE Then
                            StrSQL.Append(" AND   (Elem_Cod <> " & Agro_SQL_SaveNum(AgronicaCoreDataProvider.CostantiPersonalizzate.ZOO_CONSISTENZA) & ")   ")
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Elem_Cod, Udm_Cod ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT CategorieXUnitaMisura.Elem_Cod, CategorieXUnitaMisura.Udm_Cod, NomeComune, Udm_Des, Udm_Sim ")
                    StrSQL.Append(" FROM    CategorieXUnitaMisura ")
                    StrSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = CategorieXUnitaMisura.Udm_Cod  ")
                    StrSQL.Append(" INNER JOIN CategorieMagazzino ON CategorieMagazzino.Elem_Cod = CategorieXUnitaMisura.Elem_Cod ")
                    StrSQL.Append(" WHERE   CategorieXUnitaMisura.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     CategorieXUnitaMisura.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    If Flag_Cantina = False Then
                        StrSQL.Append(" AND   CategorieXUnitaMisura.Elem_Cod > 0  ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND   CategorieXUnitaMisura.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND   CategorieXUnitaMisura.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
                    End If

                    If Cau_Mov <> "" Then
                        If Cau_Mov <> AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_ANIMALE Then
                            StrSQL.Append(" AND   CategorieXUnitaMisura.Elem_Cod <> " & Agro_SQL_SaveNum(AgronicaCoreDataProvider.CostantiPersonalizzate.ZOO_CONSISTENZA) & "   ")
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     CategorieXUnitaMisura.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     CategorieXUnitaMisura.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY CategorieXUnitaMisura.Elem_Cod, CategorieXUnitaMisura.Udm_Cod ")
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

    Public Function Leggi_con_UnitaMisura(ByVal Elem_Cod As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.CategorieXUnitaMisura_R.Leggi_con_UnitaMisura()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  CategorieXUnitaMisura , UnitaMisura  ")
            StrSQL.AppendLine(" WHERE UnitaMisura.Validita_Inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND   CategorieXUnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   CategorieXUnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND   UnitaMisura.Udm_Cod = CategorieXUnitaMisura.Udm_Cod ")

            If Elem_Cod <> 0 Then
                StrSQL.AppendLine(" AND CategorieXUnitaMisura.Elem_Cod =  " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" ORDER BY CategorieXUnitaMisura.Udm_Cod ASC ")

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




End Class
