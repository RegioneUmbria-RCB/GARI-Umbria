Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider



Public Class Avversita_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################################
    Public Function Leggi_Con_Misura(ByVal Veg_Cod As Int32,
                                     ByVal Av_Cod As Int32,
                                     ByVal Abbreviazione As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Avversita_R.Leggi_Con_Misura()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT  ")
            StrSQL.Append("     MisuraxAvversita.VEG_COD,  ")
            StrSQL.Append("     Avversita.Av_Cod, ")
            StrSQL.Append("     Avversita.Av_Des_Vol, ")
            StrSQL.Append("     UnitaMisura.UDM_COD, ")
            StrSQL.Append("     UnitaMisura.UDM_SIM, ")
            StrSQL.Append("     UnitaMisura.UDM_DES ")
            StrSQL.Append(" FROM    MisuraxAvversita INNER JOIN      ")
            StrSQL.Append("     Avversita ON MisuraxAvversita.AV_COD = Avversita.Av_Cod INNER JOIN  ")
            StrSQL.Append("     UnitaMisura ON MisuraxAvversita.UDM_COD = UnitaMisura.UDM_COD  ")
            StrSQL.Append(" WHERE    (MisuraxAvversita.Fondamentale <> 0)    ")
            StrSQL.Append("     AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' ")
            StrSQL.Append("     AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")

            If Veg_Cod <> 0 Then
                StrSQL.Append("     AND  (MisuraxAvversita.VEG_COD = " & Agro_SQL_SaveNum(Veg_Cod) & ")  ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.Append(" AND Avversita.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If Abbreviazione <> "" Then
                StrSQL.Append(" AND Avversita.Abbreviazione LIKE '%" & Agro_SQL_SaveNum(Abbreviazione) & "%'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Avversita.Av_Des_Vol, UnitaMisura.UDM_DES      ")

            End If


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
    Public Function Leggi(ByVal Av_Cod As Int32,
                          ByVal Abbreviazione As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Avversita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append("SELECT Av_Cod, Av_Des_Vol, Av_Des_Lat, Abbreviazione " &
                                  "FROM Avversita " &
                                  " WHERE Avversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                                  " AND   Avversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    StrSQL.Append("     AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' ")
                    StrSQL.Append("     AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")

                    If Av_Cod <> 0 Then
                        StrSQL.Append(" AND Avversita.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                    End If

                    If Abbreviazione <> "" Then
                        StrSQL.Append(" AND Avversita.Abbreviazione LIKE '%" & Agro_SQL_SaveNum(Abbreviazione) & "%'  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If


                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND  Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND  Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("  ORDER BY Av_Des_Vol")
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
    Public Function AvDes_from_AvCod(ByVal Av_Cod As Int32,
                                        ByRef Av_Des_Lat As String,
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Avversita_R.AvDes_from_AvCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Av_Des As String = ""

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Av_Des_Vol, Av_Des_Lat " &
                                  " FROM   Avversita " &
                                  " WHERE  Avversita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                                  " AND    Avversita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Av_Cod <> 0 Then
                        StrSQL.Append(" AND Avversita.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND  Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND  Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("  ORDER BY Av_Des_Vol")
                    End If

            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Av_Des = DT.Rows(0).Item("Av_Des_Vol")
                Av_Des_Lat = DT.Rows(0).Item("Av_Des_Lat")
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Av_Des


    End Function

    '#############################################################################################################
    Public Function LeggiConSpecieVegetali(ByVal Veg_Cod As Int32,
                                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Avversita_R.LeggiConSpecieVegetali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Avversita.Av_Des_Vol, Avversita.Av_Cod ")
                    StrSQL.Append(" FROM Avversita INNER JOIN SpecieVegetalixAvversita ON Avversita.Av_Cod = SpecieVegetalixAvversita.Av_Cod ")
                    StrSQL.Append(" WHERE Avversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                                  " AND   Avversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    StrSQL.Append("     AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' ")
                    StrSQL.Append("     AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' ")

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetalixAvversita.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND  Avversita.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND  Avversita.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Av_Des_Vol ")
                    End If

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


    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="AvCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Abbreviazione_from_AvCod(ByVal AvCod As Integer,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As String

        Dim Dt As DataTable

        Dt = Leggi(CInt(AvCod), "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Abbreviazione")

        End If

    End Function

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Numero_Avversita(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Num_Avversita As Integer = 0
        Dim Dt As DataTable

        Dt = Leggi(0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
            Num_Avversita = Dt.Rows.Count
        End If

        Return Num_Avversita

    End Function

    '#############################################################################################################
    Public Function AvDes_Vol_Lat_from_AvCod(ByVal Av_Cod As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Avversita_R.AvDes_Vol_Lat_from_AvCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Av_Des As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Av_Des_Vol, Av_Des_Lat " &
                                  " FROM   Avversita " &
                                  " WHERE  Avversita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                                  " AND    Avversita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Av_Cod <> 0 Then
                StrSQL.AppendLine(" AND Avversita.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append("  ORDER BY Av_Des_Vol")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count = 1 Then
                Av_Des = DT.Rows(0)("Av_Des_Vol") & " (" & DT.Rows(0)("Av_Des_Lat") & ")"
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Av_Des


    End Function

    '#############################################################################################################
    '#############################################################################################################


End Class
