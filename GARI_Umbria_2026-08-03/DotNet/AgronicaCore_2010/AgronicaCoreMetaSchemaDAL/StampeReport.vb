Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class StampeReport
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiConGruppi(ByVal Id_StampeReport As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe, _
                                   ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.StampeReport.LeggiConGruppi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT StampeReport.*,StampeReportGruppi.descrizione as GruppiDescrizione, StampeReportGruppi.PivaSuperuser_Permesso as GruppiPivaSuperuser_Permesso ")
            StrSQL.AppendLine(" FROM  StampeReport left join StampeReportGruppi on StampeReport.Id_StampeReportGruppi=StampeReportGruppi.Id_StampeReportGruppi    where 1=1  ")

            StrSQL.AppendLine(" AND    ( StampeReport.PivaSuperuser_Permesso = '' or StampeReport.PivaSuperuser_Permesso LIKE '%" & objParametri.PivaSuperUser & "%') ")
            StrSQL.AppendLine(" AND    ( StampeReportGruppi.PivaSuperuser_Permesso = '' or StampeReportGruppi.PivaSuperuser_Permesso LIKE '%" & objParametri.PivaSuperUser & "%') ")

            If Id_StampeReport <> 0 Then
                StrSQL.AppendLine(" AND Id_StampeReport = " & Id_StampeReport)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" AND StampeReport.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Now))
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY StampeReport.descrizione ")
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

    Public Function Leggi(ByVal Id_StampeReport As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe,
                          ByVal Id_StampeReportGruppi As Integer,
                          ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.StampeReport.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT StampeReport.* ")
            StrSQL.Append(" FROM  StampeReport    where 1=1 ")

            StrSQL.Append(" AND   ( PivaSuperuser_Permesso ='' or PivaSuperuser_Permesso LIKE '%" & objParametri.PivaSuperUser & "%') ")
            If Id_StampeReport <> 0 Then
                StrSQL.Append(" AND Id_StampeReport = " & Id_StampeReport)
            End If
            If Id_StampeReportGruppi <> 0 Then
                StrSQL.Append(" AND Id_StampeReportGruppi = " & Id_StampeReportGruppi)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY StampeReport.descrizione ")
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
    Public Function LeggiGruppi(ByVal Id_StampeReportGruppi As Integer,
                      ByVal xFiltroAggiuntivo As String,
                    ByVal xOrderBy As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.StampeReport.LeggiGruppi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT StampeReportGruppi.* ")
            StrSQL.Append(" FROM  StampeReportGruppi    where 1=1 ")

            StrSQL.Append(" AND   ( PivaSuperuser_Permesso ='' or PivaSuperuser_Permesso LIKE '%" & objParametri.PivaSuperUser & "%') ")
            If Id_StampeReportGruppi <> 0 Then
                StrSQL.Append(" AND Id_StampeReportGruppi = " & Id_StampeReportGruppi)
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY StampeReportGruppi.descrizione ")
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



    Public Function LeggiDescrizione( _
                                    ByVal Id_StampeReport As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe, _
                                    ByVal xFiltroAggiuntivo As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                      ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.StampeReport.LeggiDescrizione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim stampades As String = ""
        Try
            '---------------------------------------------


            '--------------------------------------------------------------------------
            DT = Leggi(Id_StampeReport, 0, "", "", objParametri)
            '--------------------------------------------------------------------------

            If DT.Rows.Count = 1 Then
                stampades = DT.Rows(0).Item("Descrizione")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return stampades

    End Function

    Public Function LeggiDescrizioneLunga( _
                                ByVal Id_StampeReport As AgronicaCoreDataProvider.TipiEnumerativi.enum_CodificaStampe, _
                                ByVal xFiltroAggiuntivo As String, _
                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                  ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.StampeReport.LeggiDescrizioneLunga()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim stampades As String = ""
        Try
            '---------------------------------------------


            '--------------------------------------------------------------------------
            DT = Leggi(Id_StampeReport, 0, "", "", objParametri)
            '--------------------------------------------------------------------------

            If DT.Rows.Count = 1 Then
                stampades = DT.Rows(0).Item("Descrizione_Lunga")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return stampades

    End Function

End Class




