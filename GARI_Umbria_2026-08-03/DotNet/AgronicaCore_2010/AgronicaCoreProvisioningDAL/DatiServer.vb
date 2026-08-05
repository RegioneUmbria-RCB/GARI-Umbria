Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DatiServer_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Server As String,
                          ByVal DB As String,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreProvisioningDAL.DatiServer_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT " & vbCrLf)
                    StrSQL.AppendLine(" ID_DB " & vbCrLf)
                    StrSQL.AppendLine(" , Descrizione " & vbCrLf)
                    StrSQL.AppendLine(" FROM connessioni " & vbCrLf)
                    StrSQL.AppendLine(" WHERE 1 = 1 " & vbCrLf)

                    If Not String.IsNullOrWhiteSpace(Server) Then
                        StrSQL.AppendLine(String.Format(" AND Server = '{0}' ", Agro_SQL_SaveText(Server)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(DB) Then
                        StrSQL.AppendLine(String.Format(" AND DB = '{0}' ", Agro_SQL_SaveText(DB)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                        StrSQL.AppendLine(String.Format(" AND '{0}' ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                        StrSQL.AppendLine(String.Format(" ORDER BY '{0}' ", Agro_SQL_Save_xOrderBy(xOrderBy)) & vbCrLf)
                    End If

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Super_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Super_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiConnessioneDB(ByVal ID_DB As Int32,
                                       ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreProvisioningDAL.DatiServer_R.LeggiConnessioneDB()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT " & vbCrLf)
                    StrSQL.AppendLine(" Provider " & vbCrLf)
                    StrSQL.AppendLine(" , Server " & vbCrLf)
                    StrSQL.AppendLine(" , DB " & vbCrLf)
                    StrSQL.AppendLine(" , UserId " & vbCrLf)
                    StrSQL.AppendLine(" , Password " & vbCrLf)
                    StrSQL.AppendLine(" FROM connessioni " & vbCrLf)
                    StrSQL.AppendLine(String.Format(" WHERE ID_DB = {0} ", Agro_SQL_SaveNum(ID_DB)) & vbCrLf)

                    If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                        StrSQL.AppendLine(String.Format(" AND '{0}' ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                        StrSQL.AppendLine(String.Format(" ORDER BY '{0}' ", Agro_SQL_Save_xOrderBy(xOrderBy)) & vbCrLf)
                    End If

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Super_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Super_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class
Public Class DatiServer_W

End Class
