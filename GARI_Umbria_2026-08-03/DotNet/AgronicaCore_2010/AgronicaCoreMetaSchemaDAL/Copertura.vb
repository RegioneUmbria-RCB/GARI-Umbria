Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider


<CachedDataProviderAttribute("Copertura_R")>
Public Class Copertura_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Gru_Cod As Integer,
                          ByVal Cop_Cod As Integer,
                          ByVal Cerca_CopDes As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Copertura_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Cop_Cod, Cop_Des, Gru_cod")
                    StrSQL.Append(" FROM        Copertura ")
                    StrSQL.Append(" WHERE   Copertura.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Copertura.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    End If

                    If Cop_Cod <> 0 Then
                        StrSQL.Append(" AND Cop_Cod = " & Agro_SQL_SaveNum(Cop_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Copertura.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Copertura.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Gru_cod, Cop_Des ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  GruppoVegetale.*,  Copertura.* ")
                    StrSQL.Append(" FROM        Copertura ")
                    StrSQL.Append(" INNER JOIN    GruppoVegetale ON Copertura.Gru_Cod = GruppoVegetale.Gru_Cod ")

                    StrSQL.Append(" WHERE   Copertura.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Copertura.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Copertura.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    End If

                    If Cop_Cod <> 0 Then
                        StrSQL.Append(" AND Copertura.Cop_Cod = " & Agro_SQL_SaveNum(Cop_Cod) & " ")
                    End If

                    If Cerca_CopDes <> "" Then
                        StrSQL.Append(" AND Copertura.Cop_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CopDes) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Copertura.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Copertura.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Gru_Des, Cop_Des ")
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
    'mette in join anche le SpecieVegetali
    Public Function Leggi_2(ByVal Gru_Cod As Integer,
                            ByVal Veg_Cod As Integer,
                            ByVal Cop_Cod As Integer,
                            ByVal Cerca_CopDes As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Copertura_R.Leggi_2()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT      Copertura.*, GruppoVegetale.Gru_Des, SpecieVegetali.Veg_Des ")
            StrSQL.Append(" FROM        Copertura ")
            StrSQL.Append(" INNER JOIN  GruppoVegetale ON Copertura.Gru_Cod = GruppoVegetale.Gru_Cod ")
            StrSQL.Append(" INNER JOIN  SpecieVegetali ON GruppoVegetale.Gru_Cod = SpecieVegetali.Gru_Cod ")

            StrSQL.Append(" WHERE   Copertura.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
            StrSQL.Append(" AND     Copertura.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

            If Gru_Cod <> 0 Then
                StrSQL.Append(" AND Copertura.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
            End If

            If Cop_Cod <> 0 Then
                StrSQL.Append(" AND Copertura.Cop_Cod = " & Agro_SQL_SaveNum(Cop_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Cerca_CopDes <> "" Then
                StrSQL.Append(" AND Copertura.Cop_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CopDes) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Copertura.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Copertura.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Gru_Des, Veg_Des, Cop_Des ")
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
    Public Function CodCopNessuna_from_VegCod(ByVal Veg_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Copertura_R.CodCopNessuna_from_VegCod()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim copCod As Integer = 0

        Try

            dt = Leggi_2(0, Veg_Cod, 0, "nessun",
                         "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                copCod = dt.Rows(0).Item("Cop_Cod")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return copCod

    End Function

    '##############################################################################################
    Public Function CodCopSerra_from_VegCod(ByVal Veg_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Copertura_R.CodCopSerra_from_VegCod()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim copCod As Integer = 0

        Try

            dt = Leggi_2(0, Veg_Cod, 0, "serra",
                         "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                copCod = dt.Rows(0).Item("Cop_Cod")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return copCod

    End Function


    '##############################################################################################
    <Cacheable(True)>
    Public Function CopDes_from_CopCod(ByVal Cop_Cod As Integer,
                                       ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Copertura_R.CopDes_from_CopCod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim copDes As String = ""

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Cop_Des ")
                    StrSQL.Append(" FROM    Copertura ")
                    StrSQL.Append(" WHERE   Copertura.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Copertura.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")
                    StrSQL.Append(" AND     Copertura.Cop_Cod = " & Agro_SQL_SaveNum(Cop_Cod) & " ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Copertura.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Copertura.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Cop_Des ")
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                copDes = dt.Rows(0).Item("cop_des")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return copDes

    End Function

    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Cop_Cod"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	26/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    ''' 
    <Cacheable(True)>
    Public Function CopDes_from_CopCod_GruCod(ByVal Cop_Cod As Integer,
                                              ByVal Gru_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Copertura_R.CopDes_from_CopCod_GruCod()"

        Dim dt As DataTable
        Dim copDes As String = ""
        dt = Leggi(Gru_Cod, Cop_Cod, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            copDes = dt.Rows(0).Item("Cop_Des")
        End If

        Return copDes

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
