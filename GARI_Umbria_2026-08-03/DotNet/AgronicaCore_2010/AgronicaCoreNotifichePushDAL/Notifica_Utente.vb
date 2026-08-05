Public Class Notifica_Utente_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ServizioSottoscritto As Int32,
                          ByVal Piattaforma As String,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.Notifica_Utente_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM Notifiche_Utenti " & vbCrLf)
                    StrSQL.Append(" WHERE " & vbCrLf)
                    StrSQL.Append(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser)) & vbCrLf)

                    If ServizioSottoscritto > 0 Then
                        StrSQL.Append(String.Format(" AND IdServizio = {0} ", Agro_SQL_SaveNum(ServizioSottoscritto)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(Piattaforma) Then
                        StrSQL.Append(String.Format(" AND Piattaforma = '{0}' ", Agro_SQL_SaveText(Piattaforma)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                        StrSQL.Append(String.Format(" AND {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)) & vbCrLf)
                    End If

                    If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                        StrSQL.Append(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, objParametri)) & vbCrLf)
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

End Class
Public Class Notifica_Utente_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella(ByVal SubscriberId As String,
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
            'stb.Append(String.Format(" AND Utente = '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername)) & vbCrLf)
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
