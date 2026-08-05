Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider

<CachedDataProviderAttribute("GruppoFinalita_R")>
Public Class GruppoFinalita_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi(ByVal GRFI_COD As Long,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoFinalita_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  GruppoFinalita ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If GRFI_COD <> 0 Then
                        StrSQL.Append(" AND GRFI_COD = " & Agro_SQL_SaveNum(GRFI_COD) & "  ")
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
                        StrSQL.Append(" ORDER BY GRFI_DES ASC")
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


    <Cacheable(True)>
    Public Function Leggi(ByVal GRFI_COD As Long,
                          ByVal Veg_Cod As Integer,
                          ByVal Cerca_GrfiDes As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoFinalita_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append(" SELECT  SpecieVegetali.Veg_des, GruppoFinalitaxSpecieVegetali.Veg_Cod,  GruppoFinalita.* ")
                    StrSQL.Append(" FROM        GruppoFinalita ")
                    StrSQL.Append(" INNER JOIN    GruppoFinalitaxSpecieVegetali ON GruppoFinalitaxSpecieVegetali.GRFI_COD = GruppoFinalita.GRFI_COD ")
                    StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = GruppoFinalitaxSpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   GruppoFinalita.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     GruppoFinalita.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If GRFI_COD <> 0 Then
                        StrSQL.Append(" AND GruppoFinalita.Grfi_Cod = " & Agro_SQL_SaveNum(GRFI_COD) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND GruppoFinalitaxSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If Cerca_GrfiDes <> "" Then
                        StrSQL.Append(" AND GruppoFinalita.Grfi_Des LIKE '%" & Agro_SQL_SaveText(Cerca_GrfiDes) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & " ")
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des, Grfi_Des ")
                    End If


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


    Public Function FasiCicloColturale_Leggi(ByVal Veg_Cod As Integer,
                                             ByVal Grfi_Cod As Integer,
                                             ByVal Regolamento_Cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoFinalita_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT GruppoFinalita.*, LimitiAzotoxSpecie.Veg_Cod, LimitiAzotoxSpecie.Grfi_Cod AS Finalita, LimitiAzotoxSpecie.Regolamento_Cod")
            StrSQL.AppendLine(" FROM GruppoFinalita INNER JOIN ")
            StrSQL.AppendLine(" FasiCicloColturalexGruppoFinalita ON GruppoFinalita.Grfi_Cod = FasiCicloColturalexGruppoFinalita.Grfi_Cod INNER JOIN ")
            StrSQL.AppendLine(" LimitiAzotoxSpecie ON FasiCicloColturalexGruppoFinalita.id_fase = LimitiAzotoxSpecie.id_fase AND FasiCicloColturalexGruppoFinalita.Regolamento_Cod = LimitiAzotoxSpecie.Regolamento_Cod ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            StrSQL.AppendLine(" AND   LimitiAzotoxSpecie.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod.ToString) & " ")
            StrSQL.AppendLine(" AND   LimitiAzotoxSpecie.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod.ToString) & " ")

            If Grfi_Cod <> 0 Then
                StrSQL.AppendLine(" AND   LimitiAzotoxSpecie.Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod.ToString) & " ")
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

    Public Function FasiCicloColturale_Anagrafiche(ByVal Veg_Cod As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.GruppoFinalita_R.FasiCicloColturale_Anagrafiche()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM FasiCicloColturale_Anagrafiche ")
            StrSQL.AppendLine(" JOIN FasiCicloColturalexSpecieVegetali ON FasiCicloColturale_Anagrafiche.Fase_Cod = FasiCicloColturalexSpecieVegetali.Fase_Cod ")
            StrSQL.AppendLine(" WHERE FasiCicloColturalexSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod.ToString) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then
                StrSQL.Length = 0

                StrSQL.AppendLine(" SELECT Gru_Cod FROM SpecieVegetali WHERE Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod.ToString) & " ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                Dim Gru_Cod = dt.Rows(0).Item("Gru_Cod")

                StrSQL.Length = 0

                StrSQL.AppendLine(" SELECT * ")
                StrSQL.AppendLine(" FROM FasiCicloColturale_Anagrafiche ")
                StrSQL.AppendLine(" JOIN FasiCicloColturalexSpecieVegetali ON FasiCicloColturale_Anagrafiche.Fase_Cod = FasiCicloColturalexSpecieVegetali.Fase_Cod ")
                StrSQL.AppendLine(" WHERE FasiCicloColturalexSpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod.ToString) & " AND FasiCicloColturalexSpecieVegetali.Veg_Cod = 0 ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function




    '##############################################################################################
    Public Function StatoImpianto_from_GrfiCod(ByVal GrfiCod As Long,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As String

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalita_R.StatoImpianto_from_GrfiCod()"

        Dim dt As DataTable

        'Recupero le informazioni
        dt = Leggi(CInt(GrfiCod), enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        'Se il recordset non è chiuso allora ...
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Grfi_des")
        End If

    End Function

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="GrfiCod"></param>
    ''' <param name="VegCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	26/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GrfiDes_from_GrfiCod(ByVal GrfiCod As Integer,
                                         ByVal VegCod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        'Recupero le informazioni		
        Dim dt As DataTable = Leggi(CInt(GrfiCod), CInt(VegCod), "",
                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                    "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Return dt.Rows(0).Item("Grfi_des")
        End If

    End Function

End Class
