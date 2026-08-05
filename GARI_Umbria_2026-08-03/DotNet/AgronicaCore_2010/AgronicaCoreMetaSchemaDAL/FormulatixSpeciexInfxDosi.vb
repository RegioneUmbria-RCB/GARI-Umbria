
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class FormulatixSpeciexInfxDosi_R


    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal FOR_VEG_AV_DOS_COD As Int32, _
                          ByVal FOR_VEG_AV_COD As Int32, _
                          ByVal UDM_COD As Int32, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexInfxDosi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT *, FormulatixSpeciexInfestanti.Verificato as VerAvv, FormulatixSpeciexInfestantixDosi.Verificato as VerDosi " & _
                                    " FROM  FormulatixSpeciexInfestantixDosi , FormulatixSpeciexInfestanti , UnitaMisura " & _
                                    " WHERE FormulatixSpeciexInfestantixDosi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexInfestantixDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexInfestanti.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexInfestanti.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexInfestantixDosi.FOR_VEG_AV_COD = FormulatixSpeciexInfestanti.FOR_VEG_AV_COD " & _
                                    " AND   FormulatixSpeciexInfestantixDosi.UDM_COD = UnitaMisura.UDM_COD ")


                    If FOR_VEG_AV_DOS_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestantixDosi.FOR_VEG_AV_DOS_COD =  " & Agro_SQL_SaveNum(FOR_VEG_AV_DOS_COD) & "  ")
                    End If

                    If FOR_VEG_AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestantixDosi.FOR_VEG_AV_COD =  " & Agro_SQL_SaveNum(FOR_VEG_AV_COD) & "  ")
                    End If

                    If UDM_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestantixDosi.UDM_COD =  " & Agro_SQL_SaveNum(UDM_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexInfestantixDosi.FOR_VEG_AV_COD ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_New(ByVal VEG_COD As Int32, _
                          ByVal FR_COD As Int32, _
                          ByVal AV_GRU As Int32, _
                          ByVal AV_COD As Int32, _
                          ByVal strInfestanti As String, _
                          ByVal Epoca_Cod As Int32, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexInfxDosi_R.Leggi_New()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct FormulatixSpeciexInfestantixDosi.Dose_Min, FormulatixSpeciexInfestantixDosi.Dose_Max, FormulatixSpeciexInfestantixDosi.Udm_Cod, FormulatixSpeciexInfestantixDosi.Note, " & _
                                    "         FormulatixSpeciexInfestantixDosi.Da_Epoca_1, FormulatixSpeciexInfestantixDosi.A_Epoca_1, FormulatixSpeciexInfestantixDosi.Da_Epoca_2, FormulatixSpeciexInfestantixDosi.A_Epoca_2 " & _
                                    " FROM    FormulatixSpeciexInfestantixDosi  " & _
                                    " INNER JOIN FormulatixSpeciexInfestanti ON FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod = FormulatixSpeciexInfestanti.For_Veg_Av_Cod " & _
                                    " Left Outer Join Epoche_Raggruppamenti ON (Epoche_Raggruppamenti.Ep_Cod_Formulati = FormulatixSpeciexInfestantixDosi.Da_Epoca_1 OR Epoche_Raggruppamenti.Ep_Cod_Formulati = FormulatixSpeciexInfestantixDosi.Da_Epoca_2) " & _
                                    " WHERE   FormulatixSpeciexInfestantixDosi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND     FormulatixSpeciexInfestantixDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND     FormulatixSpeciexInfestanti.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND     FormulatixSpeciexInfestanti.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND ( FormulatixSpeciexInfestanti.Veg_Cod = " & VEG_COD & "  " & _
                                      " OR  ( FormulatixSpeciexInfestanti.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & _
                                      " Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & VEG_COD & " ) ) ) ")
                    End If

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestanti.Fr_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If AV_GRU <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestanti.Av_Gru =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestanti.Av_Cod =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    If Trim(strInfestanti) <> "" Then
                        StrSQL.Append(" AND " & strInfestanti)
                    End If

                    If Epoca_Cod <> 0 Then
                        'Considero sia le epoche esatte che le non definite
                        StrSQL.Append(" AND ( Epoche_Raggruppamenti.Ep_Cod_DPI = " & Epoca_Cod & " OR ( FormulatixSpeciexInfestantixDosi.Da_Epoca_1 = 0 AND FormulatixSpeciexInfestantixDosi.Da_Epoca_2 = 0))  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexInfestantixDosi.Dose_Max Desc")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi2(ByVal VEG_COD As Int32, _
                          ByVal FR_COD As Int32, _
                          ByVal AV_GRU As Int32, _
                          ByVal AV_COD As Int32, _
                          ByVal strInfestanti As String, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexInfxDosi_R.Leggi2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct FormulatixSpeciexInfestantixDosi.*, FormulatixSpeciexInfestanti.For_Veg_Av_Cod, UnitaMisura.UDM_SIM, FormulatixSpecieVegetali.For_Veg_Cod, FormulatixSpecieVegetali.Veg_Cod, FormulatixSpecieVegetali.Fr_Cod " & _
                                    " FROM  ((((( FormulatixSpecieVegetali Inner Join  FormulatixSpeciexInfestanti ON ( FormulatixSpecieVegetali.For_Veg_Cod = FormulatixSpeciexInfestanti.For_Veg_Cod ) " & _
                                    " Inner Join  Formulati ON ( Formulati.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod )) " & _
                                    " Inner Join  FormulatixSpeciexInfestantixDosi ON ( FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod = FormulatixSpeciexInfestanti.For_Veg_Av_Cod )) " & " " & _
                                    " Inner Join GruppoAvversita ON ( FormulatixSpeciexInfestanti.Av_Gru = GruppoAvversita.Av_Gru )) " & _
                                    " Inner Join  AvversitaxGruppoAvversita ON  ( FormulatixSpeciexInfestanti.Av_Cod = AvversitaxGruppoAvversita.Av_Cod )) " & _
                                    " Inner Join  UnitaMisura ON  ( FormulatixSpeciexInfestantixDosi.UDM_COD = UnitaMisura.UDM_COD )) " & _
                                    " WHERE FormulatixSpeciexInfestantixDosi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexInfestantixDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexInfestanti.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexInfestanti.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND Formulati.Fr_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If AV_GRU <> 0 Then
                        StrSQL.Append(" AND AvversitaxGruppoAvversita.Av_Gru =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestanti.Av_Cod =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    If Trim(strInfestanti) <> "" Then
                        StrSQL.Append(" AND " & strInfestanti)
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexInfestantixDosi.Dose_Max ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    'Include anche le epoche
    Public Function Leggi3(ByVal VEG_COD As Int32, _
                          ByVal FR_COD As Int32, _
                          ByVal AV_GRU As Int32, _
                          ByVal AV_COD As Int32, _
                          ByVal strInfestanti As String, _
                          ByVal Epoca_Cod As Int32, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexInfxDosi_R.Leggi3()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct FormulatixSpeciexInfestantixDosi.Dose_Min, FormulatixSpeciexInfestantixDosi.Dose_Max, FormulatixSpeciexInfestantixDosi.Udm_Cod, FormulatixSpeciexInfestantixDosi.Note, " & _
                                    "               FormulatixSpeciexInfestantixDosi.Da_Epoca_1, FormulatixSpeciexInfestantixDosi.A_Epoca_1, FormulatixSpeciexInfestantixDosi.Da_Epoca_2, FormulatixSpeciexInfestantixDosi.A_Epoca_2 " & _
                                    " FROM  ((((( FormulatixSpecieVegetali Inner Join  FormulatixSpeciexInfestanti ON ( FormulatixSpecieVegetali.For_Veg_Cod = FormulatixSpeciexInfestanti.For_Veg_Cod ) " & _
                                    " Inner Join  Formulati ON ( Formulati.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod )) " & _
                                    " Inner Join  FormulatixSpeciexInfestantixDosi ON ( FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod = FormulatixSpeciexInfestanti.For_Veg_Av_Cod )) " & " " & _
                                    " Inner Join GruppoAvversita ON ( FormulatixSpeciexInfestanti.Av_Gru = GruppoAvversita.Av_Gru )) " & _
                                    " Inner Join  AvversitaxGruppoAvversita ON  ( FormulatixSpeciexInfestanti.Av_Cod = AvversitaxGruppoAvversita.Av_Cod )) " & _
                                    " Left outer join Epoche_Raggruppamenti ON (Epoche_Raggruppamenti.Ep_Cod_Formulati = FormulatixSpeciexInfestantixDosi.Da_Epoca_1 OR Epoche_Raggruppamenti.Ep_Cod_Formulati = FormulatixSpeciexInfestantixDosi.Da_Epoca_2)) " & _
                                    " WHERE FormulatixSpeciexInfestantixDosi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexInfestantixDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexInfestanti.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexInfestanti.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND Formulati.Fr_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If AV_GRU <> 0 Then
                        StrSQL.Append(" AND AvversitaxGruppoAvversita.Av_Gru =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexInfestanti.Av_Cod =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    If Trim(strInfestanti) <> "" Then
                        StrSQL.Append(" AND " & strInfestanti)
                    End If

                    If Epoca_Cod <> 0 Then
                        'Considero sia le epoche esatte che le non definite
                        StrSQL.Append(" AND ( Epoche_Raggruppamenti.Ep_Cod_DPI = " & Epoca_Cod & " OR ( FormulatixSpeciexInfestantixDosi.Da_Epoca_1 = 0 AND FormulatixSpeciexInfestantixDosi.Da_Epoca_2 = 0))  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexInfestantixDosi.Dose_Max Desc")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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



End Class
