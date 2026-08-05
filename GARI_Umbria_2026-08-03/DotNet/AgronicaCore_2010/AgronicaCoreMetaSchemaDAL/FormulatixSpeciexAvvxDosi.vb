Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class FormulatixSpeciexAvvxDosi_R


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

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexAvvxDosi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT *, FormulatixSpeciexAvversita.Verificato as VerAvv, FormulatixSpeciexAvversitaxDosi.Verificato as VerDosi " & _
                                    " FROM  FormulatixSpeciexAvversitaxDosi , FormulatixSpeciexAvversita , UnitaMisura " & _
                                    " WHERE FormulatixSpeciexAvversitaxDosi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexAvversitaxDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexAvversita.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexAvversita.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexAvversitaxDosi.FOR_VEG_AV_COD = FormulatixSpeciexAvversita.FOR_VEG_AV_COD " & _
                                    " AND   FormulatixSpeciexAvversitaxDosi.UDM_COD = UnitaMisura.UDM_COD ")


                    If FOR_VEG_AV_DOS_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversitaxDosi.FOR_VEG_AV_DOS_COD =  " & Agro_SQL_SaveNum(FOR_VEG_AV_DOS_COD) & "  ")
                    End If

                    If FOR_VEG_AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversitaxDosi.FOR_VEG_AV_COD =  " & Agro_SQL_SaveNum(FOR_VEG_AV_COD) & "  ")
                    End If

                    If UDM_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversitaxDosi.UDM_COD =  " & Agro_SQL_SaveNum(UDM_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexAvversitaxDosi.FOR_VEG_AV_COD ASC")
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
                              ByVal strAvversita As String, _
                              ByVal Validita_Inizio As Date, _
                              ByVal Validita_Fine As Date, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexAvvxDosi_R.Leggi2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT FormulatixSpeciexAvversitaxDosi.*  " & _
                                    " FROM  ( FormulatixSpecieVegetali Inner Join  FormulatixSpeciexAvversita ON ( FormulatixSpecieVegetali.For_Veg_Cod = FormulatixSpeciexAvversita.For_Veg_Cod ) " & _
                                    " Inner Join  Formulati ON ( Formulati.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod ) " & _
                                    " Inner Join  FormulatixSpeciexAvversitaxDosi ON ( FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod = FormulatixSpeciexAvversita.For_Veg_Av_Cod )) " & _
                                    " WHERE FormulatixSpeciexAvversitaxDosi.Validita_inizio < " & Validita_Fine & " " & _
                                    " AND   FormulatixSpeciexAvversitaxDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                    " AND   FormulatixSpeciexAvversita.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FormulatixSpeciexAvversita.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND Formulati.Fr_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If AV_GRU <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_Gru =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_Cod =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    If Trim(strAvversita) <> "" Then
                        StrSQL.Append(" AND " & strAvversita)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexAvversitaxDosi.FOR_VEG_AV_COD ASC")
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
                              ByVal strAvversita As String, _
                              ByVal Validita_Inizio As Date, _
                              ByVal Validita_Fine As Date, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpeciexAvvxDosi_R.Leggi_New()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  FormulatixSpeciexAvversitaxDosi.* " & _
                                 " FROM    FormulatixSpeciexAvversitaxDosi INNER JOIN " & _
                                 "         FormulatixSpeciexAvversita ON FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod = FormulatixSpeciexAvversita.For_Veg_Av_Cod " & _
                                 " WHERE   FormulatixSpeciexAvversitaxDosi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                 " AND     FormulatixSpeciexAvversitaxDosi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                 " AND     FormulatixSpeciexAvversita.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                 " AND     FormulatixSpeciexAvversita.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND ( FormulatixSpeciexAvversita.Veg_Cod = " & VEG_COD & "  " & _
                                      " OR  ( FormulatixSpeciexAvversita.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & _
                                       "      Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & VEG_COD & " ) ) ) ")
                    End If

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversita.Fr_Cod =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If AV_GRU <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_Gru =  " & Agro_SQL_SaveNum(AV_GRU) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixSpeciexAvversita.Av_Cod =  " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    If Trim(strAvversita) <> "" Then
                        StrSQL.Append(" AND " & strAvversita)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FormulatixSpeciexAvversitaxDosi.FOR_VEG_AV_COD ASC")
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
