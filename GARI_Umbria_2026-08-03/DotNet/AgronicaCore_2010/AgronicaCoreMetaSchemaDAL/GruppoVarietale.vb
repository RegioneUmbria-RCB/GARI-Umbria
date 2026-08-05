Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

<CachedDataProviderAttribute("GruppoVarietale_R")>
Public Class GruppoVarietale_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    '##############################################################################################
    <Cacheable(True)>
    Public Function LeggiTabella(ByVal Grva_Cod As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoVarietale_R.LeggiTabella()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    GruppoVarietale  ")
            StrSQL.Append(" WHERE   1 = 1 ")

            If Grva_Cod <> 0 Then
                StrSQL.Append(" AND GruppoVarietale.Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     GruppoVarietale.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     GruppoVarietale.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Grva_Des ")
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
    <Cacheable(True)>
    Public Function Leggi(ByVal Veg_Cod As Integer,
                          ByVal Grva_Cod As Integer,
                          ByVal Cerca_GrvaDes As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoVarietale_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  GruppoVarietale.*, SpecieVegetalixGruppoVarietale.*, SpecieVegetali.Veg_des ")
                    StrSQL.Append(" FROM        GruppoVarietale ")
                    StrSQL.Append(" INNER JOIN    SpecieVegetalixGruppoVarietale ON GruppoVarietale.Grva_Cod = SpecieVegetalixGruppoVarietale.Grva_Cod ")
                    StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = SpecieVegetalixGruppoVarietale.Veg_Cod ")

                    StrSQL.Append(" WHERE   GruppoVarietale.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     GruppoVarietale.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Grva_Cod <> 0 Then
                        StrSQL.Append(" AND GruppoVarietale.Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If Cerca_GrvaDes <> "" Then
                        StrSQL.Append(" AND GruppoVarietale.Grva_Des LIKE '%" & Agro_SQL_SaveText(Cerca_GrvaDes) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     GruppoVarietale.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     GruppoVarietale.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des, Grva_Des ")
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
    Public Function LeggixSementi(ByVal Veg_Cod As Integer,
                                  ByVal Grva_Cod As Integer,
                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoVarietale_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  GruppoVarietale.Grva_Cod, ")

                    StrSQL.Append("         GruppoVarietale.Grva_Des,  ")
                    StrSQL.Append("         SpecieVegetali.Veg_Cod,  ")
                    StrSQL.Append("         SpecieVegetali.Veg_Des ")
                    StrSQL.Append(" FROM    GruppoVarietale INNER JOIN ")
                    StrSQL.Append("         SpecieVegetalixGruppoVarietale ON GruppoVarietale.Grva_Cod = SpecieVegetalixGruppoVarietale.Grva_Cod INNER JOIN ")
                    StrSQL.Append("         SpecieVegetali ON SpecieVegetalixGruppoVarietale.Veg_Cod = SpecieVegetali.Veg_Cod ")
                    StrSQL.Append(" WHERE   (GruppoVarietale.Grva_Cod <> -999999)  ")


                    If Grva_Cod <> 0 Then
                        StrSQL.Append(" AND GruppoVarietale.Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append("  AND     (SpecieVegetali.Veg_Cod = " & Veg_Cod & ")    ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     GruppoVarietale.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     GruppoVarietale.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des, Grva_Des ")
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
    Public Function Grva_Des_From_Grva_Cod_xSementi(ByVal Veg_Cod As Integer,
                                                    ByVal Grva_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim dt As DataTable = Leggi(Veg_Cod, Math.Abs(Grva_Cod), "",
                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                    "", "", objParametri)
        Dim strR As String = ""
        Dim i As Integer
        If dt.Rows.Count > 0 Then
            If Grva_Cod > 0 Then
                strR = dt.Rows(i).Item("Grva_Des")
            Else
                'ibridi
                strR = dt.Rows(i).Item("Grva_Des").ToLower & " - ibrido</option>"
            End If
        End If

        Return strR

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
