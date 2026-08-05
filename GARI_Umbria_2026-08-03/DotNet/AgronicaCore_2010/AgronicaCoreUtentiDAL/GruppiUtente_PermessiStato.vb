Public Class GruppiUtente_PermessiStato_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Const IgnoraFiltroStatoCod As Integer = -1

    Public Function Leggi(ByVal Gruppo_Utente As Integer,
                          ByVal Servizio_Cod As Integer,
                          ByVal Stato_Cod As Integer,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GruppiUtente_PermessiStato_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    strSQL.AppendLine(" SELECT gruppo_permessi.* ")
                    strSQL.AppendLine(" , gruppo.Gruppi_Utente_des ")
                    strSQL.AppendLine(" , servizio.Servizio_Des ")
                    strSQL.AppendLine(" , stato.WAnagraficaStati_Des as Stato_Des ")
                    strSQL.AppendLine(" FROM GruppiUtente_PermessiStato gruppo_permessi ")
                    strSQL.AppendLine(" INNER JOIN Gruppi_Utente gruppo ON gruppo_permessi.Gruppo_Utente = gruppo_permessi.Gruppi_Utente_cod ")
                    strSQL.AppendLine(" INNER JOIN Servizi_Pratiche servizio ON gruppo_permessi.Servizio_Cod = servizio.Servizio_Cod ")
                    strSQL.AppendLine(" INNER JOIN WAnagraficaStati stato ON gruppo_permessi.Stato_Cod = stato.WAnagraficaStati_Cod ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    strSQL.AppendLine(" SELECT gruppo_permessi.* ")
                    strSQL.AppendLine(" FROM GruppiUtente_PermessiStato gruppo_permessi ")

            End Select

            strSQL.AppendLine(" WHERE 1=1 ")

            If Gruppo_Utente <> 0 Then
                strSQL.AppendLine(" AND gruppo_permessi.Gruppi_Utente_Cod = " & Agro_SQL_SaveNum(Gruppo_Utente) & " ")
            End If

            If Servizio_Cod <> 0 Then
                strSQL.AppendLine(" AND gruppo_permessi.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> IgnoraFiltroStatoCod Then
                strSQL.AppendLine(" AND gruppo_permessi.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND gruppo_permessi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND gruppo_permessi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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