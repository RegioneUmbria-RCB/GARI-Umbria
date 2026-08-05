Public Class ParticelleCatastali_Acclivita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Long,
                              ByVal NUMERO As Long,
                              ByVal SUBALTERNO As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Acclivita_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM ParticelleCatastali_Acclivita ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PROV <> "" Then
                StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PROV,COM,SEZIONE,FOGLIO,NUMERO,SUBALTERNO ")
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


    '##############################################################################################
    Public Function Pendenza_A(ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Long,
                           ByVal NUMERO As Long,
                           ByVal SUBALTERNO As String,
                           ByRef BSL As Integer,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Acclivita_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM ParticelleCatastali_Acclivita ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            ' If PROV <> "" Then
            StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            ' End If

            ' If COM <> "" Then
            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            ' End If

            StrSQL.Append(" AND FG_Zona_Acclivita = 'A' ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PROV,COM,SEZIONE,FOGLIO,NUMERO,SUBALTERNO ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            Dim DrPV() As DataRow
            Dim CheckFinito As Boolean = False


            If DT.Rows.Count > 0 Then

                ' controllo se tutto il comune è vulnerabile
                DrPV = DT.Select("SEZIONE='0' AND FOGLIO=0 AND NUMERO=0 AND SUBALTERNO='0'")

                ' tutto il comune è vulnerabile, ho finito, ritorno true ed esco
                If DrPV.Length > 0 Then
                    bRet = True
                    CheckFinito = True
                    BSL = DrPV(0).Item("BSL")
                Else
                    If SEZIONE <> "0" Then
                        ' controllo se tutta la sezione è vulnerabile
                        DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=0 AND NUMERO=0 AND SUBALTERNO='0'")

                        If DrPV.Length > 0 Then
                            bRet = True
                            CheckFinito = True
                            BSL = DrPV(0).Item("BSL")
                        End If
                    End If

                    If Not CheckFinito Then

                        ' controllo se il foglio è vulnerabile
                        DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=" & FOGLIO & " AND NUMERO=0 AND SUBALTERNO='0'")

                        If DrPV.Length > 0 Then
                            bRet = True
                            CheckFinito = True
                            BSL = DrPV(0).Item("BSL")
                        Else

                            ' controllo se il la particella è vulnerabile
                            DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=" & FOGLIO & " AND NUMERO= " & NUMERO & " AND SUBALTERNO='0'")

                            If DrPV.Length > 0 Then
                                bRet = True
                                CheckFinito = True
                                BSL = DrPV(0).Item("BSL")
                            Else

                                If SUBALTERNO <> "0" Then
                                    ' controllo se il subalterno è vulnerabile
                                    DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=" & FOGLIO & " AND NUMERO= " & NUMERO & " AND SUBALTERNO='" & SUBALTERNO & "'")

                                    If DrPV.Length > 0 Then
                                        bRet = True
                                        BSL = DrPV(0).Item("BSL")
                                    End If
                                End If

                            End If

                        End If

                    End If

                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return bRet

    End Function


    Public Function Pendenza_B(ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Long,
                           ByVal NUMERO As Long,
                           ByVal SUBALTERNO As String,
                           ByRef BSL As Integer,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Acclivita_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM ParticelleCatastali_Acclivita ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            ' If PROV <> "" Then
            StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            ' End If

            ' If COM <> "" Then
            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            ' End If

            StrSQL.Append(" AND FG_Zona_Acclivita = 'B' ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PROV,COM,SEZIONE,FOGLIO,NUMERO,SUBALTERNO ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            Dim DrPV() As DataRow
            Dim CheckFinito As Boolean = False


            If DT.Rows.Count > 0 Then

                ' controllo se tutto il comune è vulnerabile
                DrPV = DT.Select("SEZIONE='0' AND FOGLIO=0 AND NUMERO=0 AND SUBALTERNO='0'")

                ' tutto il comune è vulnerabile, ho finito, ritorno true ed esco
                If DrPV.Length > 0 Then
                    bRet = True
                    CheckFinito = True
                    BSL = DrPV(0).Item("BSL")
                Else
                    If SEZIONE <> "0" Then
                        ' controllo se tutta la sezione è vulnerabile
                        DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=0 AND NUMERO=0 AND SUBALTERNO='0'")

                        If DrPV.Length > 0 Then
                            bRet = True
                            CheckFinito = True
                            BSL = DrPV(0).Item("BSL")
                        End If
                    End If

                    If Not CheckFinito Then

                        ' controllo se il foglio è vulnerabile
                        DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=" & FOGLIO & " AND NUMERO=0 AND SUBALTERNO='0'")

                        If DrPV.Length > 0 Then
                            bRet = True
                            CheckFinito = True
                            BSL = DrPV(0).Item("BSL")
                        Else

                            ' controllo se il la particella è vulnerabile
                            DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=" & FOGLIO & " AND NUMERO= " & NUMERO & " AND SUBALTERNO='0'")

                            If DrPV.Length > 0 Then
                                bRet = True
                                CheckFinito = True
                                BSL = DrPV(0).Item("BSL")
                            Else

                                If SUBALTERNO <> "0" Then
                                    ' controllo se il subalterno è vulnerabile
                                    DrPV = DT.Select("SEZIONE='" & SEZIONE & "' AND FOGLIO=" & FOGLIO & " AND NUMERO= " & NUMERO & " AND SUBALTERNO='" & SUBALTERNO & "'")

                                    If DrPV.Length > 0 Then
                                        bRet = True
                                        BSL = DrPV(0).Item("BSL")
                                    End If
                                End If

                            End If

                        End If

                    End If

                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return bRet

    End Function

End Class
