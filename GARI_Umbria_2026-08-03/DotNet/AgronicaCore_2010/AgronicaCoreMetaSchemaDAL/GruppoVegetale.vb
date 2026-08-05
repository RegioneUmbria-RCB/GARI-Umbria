Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class GruppoVegetale_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' da passargli la connessione utente
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function GruppoVegetale_GestioneFiltroUtente_Leggi(ByVal Gru_Cod As Integer,
                                                              ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri_Utenti As AgronicaCoreParametri
                                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoVegetale_R.GruppoVegetale_GestioneFiltroUtente_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                    StrSQL.Length = 0
                    StrSQL.Append(" IF (  ")
                    StrSQL.Append(" SELECT COUNT(*)  ")
                    StrSQL.Append(" FROM          GruppoVegetale ")
                    StrSQL.Append(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append("               ON GruppoVegetale.Gru_Cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                    StrSQL.Append(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "'  ")
                    StrSQL.Append(" AND           Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_Utenti.UsernameOperazione) & "'  ")
                    StrSQL.Append(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI) & " ")
                    StrSQL.Append("   ) > 0 ")

                    StrSQL.Append("  ")
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM      GruppoVegetale ")
                    StrSQL.Append(" INNER JOIN Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append("           ON GruppoVegetale.Gru_Cod = Utenti_Impostazioni_FiltroMono.ID_0 ")

                    StrSQL.Append(" WHERE     GruppoVegetale.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
                    StrSQL.Append(" AND       GruppoVegetale.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
                    StrSQL.Append(" AND       Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "'  ")
                    StrSQL.Append(" AND       Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_Utenti.UsernameOperazione) & "'  ")
                    StrSQL.Append(" AND       Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI) & " ")
                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND   GruppoVegetale.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    Else
                        StrSQL.Append(" ORDER BY GruppoVegetale.Gru_Des ")
                    End If




                    StrSQL.Append("ELSE ")


                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    GruppoVegetale ")

                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
                    End If




                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    Else
                        StrSQL.Append(" ORDER BY Gru_Des ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    Public Function GruCod_from_VegCod(ByVal VegCod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Integer

        Dim dt As DataTable

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        'Recupero le informazioni
        dt = objCOM.Leggi(CInt(VegCod), CInt(0),
                          "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "", "", objParametri)

        'Elimino gli oggetti COM
        objCOM = Nothing

        'Se il recordset non è chiuso allora ...
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Gru_cod")
        Else
            Return "0"
        End If

    End Function


    '################################################################################
    Public Function GruCod_from_Cul_Cod(ByVal Cul_cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoVegetale_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            
            stb.Append(" Select veg.Gru_Cod " & vbCrLf)
            stb.Append(" from cultivar cc " & vbCrLf)
            stb.Append(" inner join specieVegetali veg " & vbCrLf)
            stb.Append(" on cc.Veg_Cod = veg.Veg_Cod " & vbCrLf)
            stb.Append(" where cc.Cul_Cod = " & Agro_SQL_SaveNum(Cul_cod))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Dim rVal As Integer = -1
        If dt.Rows.Count > 0 Then
            rVal = CInt(dt.Rows(0)(0))
        End If

        Return rVal

    End Function

    '##############################################################################################
    Public Function Leggi(ByVal Gru_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoVegetale_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    GruppoVegetale ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     GruppoVegetale.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     GruppoVegetale.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Gru_Des ")
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


    '################################################################################
    Public Function GruDes_from_GruCod(ByVal Gru_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoVegetale_R.GruDes_from_GruCod()"
        Dim dt As DataTable

        dt = Leggi(Gru_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Gru_des")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
