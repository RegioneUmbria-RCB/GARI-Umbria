Imports AgronicaCoreDataProvider.UtilityProvider

Public Class EpocheModalita_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggixSpecie(ByVal EM_COD As Int32, _
                                  ByVal Id_Gru As Int32, _
                                  ByVal Veg_Cod As Int32, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EpocheModalita_R.LeggixSpecie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT EpocheModalita.Id_Gru,  ")
            StrSQL.Append(" EpocheModalita.EM_Cod, EpocheModalita.EM_Des ")
            StrSQL.Append(" FROM         EpocheModalita  ")
            StrSQL.Append(" INNER JOIN GruppoFinalitaxSpeciexEpoca ON EpocheModalita.Id_Gru = GruppoFinalitaxSpeciexEpoca.Id_Gru ")
            StrSQL.Append(" AND  EpocheModalita.Regolamento_Cod = GruppoFinalitaxSpeciexEpoca.Regolamento_Cod ")

            StrSQL.Append(" WHERE EpocheModalita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EpocheModalita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If EM_COD <> 0 Then
                StrSQL.Append(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(EM_COD) & "  ")
            End If
            If Id_Gru <> 0 Then
                StrSQL.Append(" AND EpocheModalita.Id_Gru =  " & Agro_SQL_SaveNum(Id_Gru) & "  ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.veg_cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY EpocheModalita.EM_Cod ")
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

    Public Function EmDes_from_EmCod(ByVal EM_COD As Int32,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EpocheModalita_R.EmDes_from_EmCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Em_Des As String = ""

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT EM_Des ")
            StrSQL.Append(" FROM  EpocheModalita  ")
            StrSQL.Append(" WHERE EpocheModalita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EpocheModalita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If EM_COD <> 0 Then
                StrSQL.Append(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(EM_COD) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Em_Des = DT.Rows(0).Item("Em_Des")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Em_Des


    End Function

    ''' <summary>
    ''' a differenza della precedente può filtrare sul regolamento e restituisce anche il veg_cod
    ''' </summary>
    Public Function Leggi(ByVal EM_COD As Int32,
                                ByVal Id_Gru As Int32,
                                ByVal Veg_Cod As Int32,
                                ByVal Regolamento_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.EpocheModalita_R.LeggixSpecie()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT EpocheModalita.Id_Gru, GruppoFinalitaxSpeciexEpoca.veg_cod, ")
            StrSQL.Append(" EpocheModalita.EM_Cod, EpocheModalita.EM_Des  ")
            StrSQL.Append(" FROM         EpocheModalita INNER JOIN ")
            StrSQL.Append(" GruppoFinalitaxSpeciexEpoca ON EpocheModalita.Id_Gru = GruppoFinalitaxSpeciexEpoca.Id_Gru ")
            StrSQL.Append(" AND  EpocheModalita.Regolamento_Cod = GruppoFinalitaxSpeciexEpoca.Regolamento_Cod ")

            StrSQL.Append(" WHERE EpocheModalita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   EpocheModalita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If EM_COD <> 0 Then
                StrSQL.Append(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(EM_COD) & "  ")
            End If
            If Id_Gru <> 0 Then
                StrSQL.Append(" AND EpocheModalita.Id_Gru =  " & Agro_SQL_SaveNum(Id_Gru) & "  ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND GruppoFinalitaxSpeciexEpoca.veg_cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If
            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND EpocheModalita.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY EpocheModalita.EM_Cod ")
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

    ''' <summary>
    ''' può filtrare sul regolamento e restituisce anche l'efficienza
    ''' </summary>
    Public Function LeggiConEfficienza(ByVal EM_COD As Integer,
                                       ByVal Id_Gru As Integer,
                                       ByVal Veg_Cod As Integer,
                                       ByVal Regolamento_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.EpocheModalita_R.LeggiConEfficienza()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT EpocheModalita.Id_Gru, ")
            strSql.AppendLine("                 EpocheModalita.EM_Cod, EpocheModalita.EM_Des, ")
            strSql.AppendLine("                 EpocheModalita.Efficienza_Cod, Efficienza.Efficienza_Des ")
            strSql.AppendLine(" FROM EpocheModalita ")
            strSql.AppendLine(" INNER JOIN GruppoFinalitaxSpeciexEpoca ")
            strSql.AppendLine("            ON EpocheModalita.Id_Gru = GruppoFinalitaxSpeciexEpoca.Id_Gru ")
            strSql.AppendLine("            AND EpocheModalita.Regolamento_Cod = GruppoFinalitaxSpeciexEpoca.Regolamento_Cod ")
            strSql.AppendLine(" INNER JOIN Efficienza ")
            strSql.AppendLine("            ON EpocheModalita.Efficienza_Cod = Efficienza.Efficienza_Cod ")
            strSql.AppendLine("            AND EpocheModalita.Regolamento_Cod = Efficienza.Regolamento_Cod ")

            strSql.AppendLine(" WHERE EpocheModalita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   EpocheModalita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If EM_COD <> 0 Then
                strSql.AppendLine(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(EM_COD) & "  ")
            End If

            If Id_Gru <> 0 Then
                strSql.AppendLine(" AND EpocheModalita.Id_Gru =  " & Agro_SQL_SaveNum(Id_Gru) & "  ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND GruppoFinalitaxSpeciexEpoca.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND EpocheModalita.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY EpocheModalita.EM_Cod ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class
