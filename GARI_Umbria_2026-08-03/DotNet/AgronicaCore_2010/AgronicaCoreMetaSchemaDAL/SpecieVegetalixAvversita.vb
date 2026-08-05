
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SpecieVegetalixAvversita_R


    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal COD As Int32,
                          ByVal VEG_COD As Int32,
                          ByVal AV_COD As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetalixAvversita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Veg_Cod, Av_Cod ")
                    StrSQL.AppendLine(" FROM SpecieVegetalixAvversita ")

                    If COD <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetalixAvversita.COD = " & Agro_SQL_SaveNum(COD) & "  ")
                    End If

                    If VEG_COD <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetalixAvversita.AV_COD = " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * FROM SpecieVegetalixAvversita, SpecieVegetali, Avversita ")
                    StrSQL.AppendLine(" WHERE SpecieVegetalixAvversita.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
                    StrSQL.AppendLine(" AND SpecieVegetalixAvversita.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))
                    StrSQL.AppendLine(" AND SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
                    StrSQL.AppendLine(" AND SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))
                    StrSQL.AppendLine(" AND Avversita.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
                    StrSQL.AppendLine(" AND Avversita.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))
                    StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = SpecieVegetali.VEG_COD ")
                    StrSQL.AppendLine(" AND SpecieVegetalixAvversita.AV_COD = Avversita.AV_COD ")


                    If COD <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetalixAvversita.COD = " & Agro_SQL_SaveNum(COD) & "  ")
                    End If

                    If VEG_COD <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetalixAvversita.VEG_COD = " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If AV_COD <> 0 Then
                        StrSQL.AppendLine(" AND SpecieVegetalixAvversita.AV_COD = " & Agro_SQL_SaveNum(AV_COD) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Avversita.AV_DES_VOL ASC , SpecieVegetali.VEG_DES ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

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




End Class
