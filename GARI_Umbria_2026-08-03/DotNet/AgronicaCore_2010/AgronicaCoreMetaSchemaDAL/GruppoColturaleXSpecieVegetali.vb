Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider



Public Class GruppoColturaleXSpecieVegetali_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Grsp_Cod As integer, _
                          ByVal Veg_Cod As integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append("SELECT Veg_Cod, Grsp_Cod " & _
                                    "FROM GruppoColturaleXSpecieVegetali " & _
                                    " WHERE GruppoColturaleXSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                                    " AND   GruppoColturaleXSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Grsp_Cod <> 0 Then
                        StrSQL.Append(" AND GruppoColturaleXSpecieVegetali.Grsp_Cod = " & Agro_SQL_SaveNum(Grsp_Cod) & "  ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If


                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND  GruppoColturaleXSpecieVegetali.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND  GruppoColturaleXSpecieVegetali.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod, Grsp_Cod ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


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
    Public Function VegCod_from_GrspCod(ByVal Grsp_Cod As Integer, _
                                        ByRef Array_Veg_Cod() As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    )

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoColturaleXSpecieVegetali_R.VegCod_from_GrspCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable 'Recupero il recordset
        Dim objGruppoCol As New AgronicaCoreMetaSchemaDAL.GruppoColturaleXSpecieVegetali_R
        DT = objGruppoCol.Leggi(Grsp_Cod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                    "", "", objParametri)

        Dim i As Integer
        If DT.Rows.Count > 0 Then

            i = 0
            For i = 0 To DT.Rows.Count - 1
                ReDim Preserve Array_Veg_Cod(i)
                Array_Veg_Cod(i) = DT.Rows(i).Item("Veg_Cod")
            Next
        End If


    End Function


    '#############################################################################################################
    '#############################################################################################################


End Class
