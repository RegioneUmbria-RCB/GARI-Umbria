Imports AgronicaCoreDataProvider
Public Class Notifica_Utente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PivaSuperUser As String,
                          ByVal SubscriberId As String,
                          ByVal IdServizio As Int32,
                          ByVal Utente As String,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGiasAppDAL.Notifica_Utente_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM Notifiche_Utenti " & vbCrLf)
                    StrSQL.Append(" WHERE 1=1 " & vbCrLf)

                    If Not String.IsNullOrWhiteSpace(PivaSuperUser) Then
                        StrSQL.Append(String.Format(" AND PivaSuperUser = '{0}' ", Agro_SQL_SaveText(PivaSuperUser)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(SubscriberId) Then
                        StrSQL.Append(String.Format(" AND SubscriberId = '{0}' ", Agro_SQL_SaveText(SubscriberId)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(Utente) Then
                        StrSQL.Append(String.Format(" AND Utente = '{0}' ", Agro_SQL_SaveText(Utente)) & vbCrLf)
                    End If

                    If IdServizio > 0 Then
                        StrSQL.Append(String.Format(" AND IdServizio = {0} ", Agro_SQL_SaveNum(IdServizio)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                        StrSQL.Append(String.Format(" AND '{0}' ", Agro_SQL_SaveText(xFiltroAggiuntivo)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                        StrSQL.Append(String.Format(" ORDER BY '{0}' ", Agro_SQL_SaveText(xOrderBy)) & vbCrLf)
                    End If

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

    Public Function LeggiNotificheSottoscrivibili(pivaSuperUser As String,
                                                  utenteUsername As String,
                                                  objParametri As AgronicaCoreParametri,
                                                  objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGiasAppDAL.Notifica_Utente_R.LeggiNotificheSottoscrivibili()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim connBuilder As New Common.DbConnectionStringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT SSN.IDServizio, SSN.Descrizione, SSN.Obbligatorio, SSN.Sottoscrivi, SSN.DeepLink, SSN.UrlMedia " & vbCrLf)
            StrSQL.Append(" FROM Servizi_Sottoscrizione_Notifiche SSN " & vbCrLf)
            StrSQL.Append(" INNER JOIN ServiziNotificheXUtenti SNXU on SNXU.IdServizio = SSN.IdServizio " & vbCrLf)
            StrSQL.Append(" WHERE 1=1 " & vbCrLf)

            If Not String.IsNullOrWhiteSpace(pivaSuperUser) Then
                StrSQL.Append(String.Format(" AND SNXU.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(pivaSuperUser)) & vbCrLf)
            End If

            If Not String.IsNullOrWhiteSpace(utenteUsername) Then
                StrSQL.Append(String.Format(" AND SNXU.UserName = '{0}' ", Agro_SQL_SaveText(utenteUsername)) & vbCrLf)
            End If

            connBuilder.ConnectionString = objParametri_Utenti.StringaConnessione

            StrSQL.Append(" UNION " & vbCrLf)
            StrSQL.Append(" SELECT distinct SSNU.IDServizio, SSNU.Descrizione, SSNU.Obbligatorio, SSNU.Sottoscrivi, SSNU.DeepLink, SSNU.UrlMedia " & vbCrLf)
            StrSQL.Append(String.Format(" FROM {0}.dbo.Utenti_xGruppi_Utente GUU ", connBuilder("Initial Catalog")) & vbCrLf)
            StrSQL.Append(" INNER JOIN ServiziNotificheXGruppiUtente SNXGUU on SNXGUU.Gruppi_Utente_cod = GUU.Gruppi_Utente_cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Servizi_Sottoscrizione_Notifiche SSNU on SSNU.IdServizio = SNXGUU.IdServizio " & vbCrLf)
            StrSQL.Append(" WHERE 1=1 " & vbCrLf)

            If Not String.IsNullOrWhiteSpace(pivaSuperUser) Then
                StrSQL.Append(String.Format(" AND SNXGUU.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(pivaSuperUser)) & vbCrLf)
            End If

            If Not String.IsNullOrWhiteSpace(utenteUsername) Then
                StrSQL.Append(String.Format(" AND GUU.UserName = '{0}' ", Agro_SQL_SaveText(utenteUsername)) & vbCrLf)
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
End Class
Public Class Notifica_Utente_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function PushNotifica(ByVal SubscriberId As String,
                                 ByVal IdServizio As Int32,
                                 ByVal Piattaforma As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGiasAppDAL.Notifica_Utente_W.PushNotifica()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0

            stb.Append(" INSERT INTO Notifiche_Utenti ( " & vbCrLf)
            stb.Append(" PivaSuperUser, " & vbCrLf)
            stb.Append(" SubscriberId, " & vbCrLf)
            stb.Append(" Piattaforma, " & vbCrLf)
            stb.Append(" Utente, " & vbCrLf)
            stb.Append(" IdServizio, " & vbCrLf)
            stb.Append(" Data_Creazione, " & vbCrLf)
            stb.Append(" Data_Modifica, " & vbCrLf)
            stb.Append(" Username_Creazione, " & vbCrLf)
            stb.Append(" Username_Modifica " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" Values ( " & vbCrLf)
            stb.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser)) & vbCrLf)
            stb.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(SubscriberId)) & vbCrLf)
            stb.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(Piattaforma)) & vbCrLf)
            stb.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername)) & vbCrLf)
            stb.Append(String.Format(" {0}, ", Agro_SQL_SaveNum(IdServizio)) & vbCrLf)
            stb.Append(" GETDATE(), " & vbCrLf)
            stb.Append(" GETDATE(), " & vbCrLf)
            stb.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UsernameOperazione)) & vbCrLf)
            stb.Append(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)) & vbCrLf)

            stb.Append(" ) " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function PopNotifica(ByVal SubscriberId As String,
                                ByVal IdServizio As Int32,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGiasAppDAL.Notifica_Utente_W.PushNotifica()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0

            stb.Append(" DELETE FROM Notifiche_Utenti " & vbCrLf)
            stb.Append(" WHERE 1 = 1 " & vbCrLf)
            stb.Append(String.Format(" AND PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser)) & vbCrLf)
            stb.Append(String.Format(" AND SubscriberId = '{0}' ", Agro_SQL_SaveText(SubscriberId)) & vbCrLf)
            stb.Append(String.Format(" AND Utente = '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername)) & vbCrLf)
            stb.Append(String.Format(" AND IdServizio = {0} ", Agro_SQL_SaveNum(IdServizio)) & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
