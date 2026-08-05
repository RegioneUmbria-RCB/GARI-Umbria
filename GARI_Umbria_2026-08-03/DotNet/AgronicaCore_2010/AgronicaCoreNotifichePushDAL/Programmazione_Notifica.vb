Imports AgronicaCoreDataProvider

Public Class Programmazione_Notifica_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiDaInviare(ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.Programmazione_Notifica_R.LeggiDaInviare()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT " & vbCrLf)
            StrSQL.Append(" ID_Notifica " & vbCrLf)
            StrSQL.Append(" , TipoNotifica_ID " & vbCrLf)
            StrSQL.Append(" , SubscriberId " & vbCrLf)
            StrSQL.Append(" , DeepLink " & vbCrLf)
            StrSQL.Append(" , MediaURL " & vbCrLf)
            StrSQL.Append(" FROM NotifichePush_Programmazione " & vbCrLf)
            StrSQL.Append(" WHERE " & vbCrLf)
            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)) & vbCrLf)
            StrSQL.AppendLine(" And Spedita = 0 " & vbCrLf)
            StrSQL.AppendLine(" And AnnullatoInvio = 0 " & vbCrLf)
            StrSQL.AppendLine(" And DataOraDaCuiInviare <= GETDATE() " & vbCrLf)

            If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                StrSQL.Append(String.Format(" And {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)) & vbCrLf)
            End If

            If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                StrSQL.Append(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, objParametri)) & vbCrLf)
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
Public Class Programmazione_Notifica_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Accoda(ByVal mittente As String,
                           ByVal DataInvio As Date,
                           ByVal notifica As DataRow,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.Programmazione_Notifica_W.Accoda()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try

            Dim seq As New Agro_Sequenze()
            Dim nuovoId As Integer = seq.NuovoId_Tabella("Notifica_ID_Notifica", 0, 2000000000, objParametri)

            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO NotifichePush_Programmazione " & vbCrLf)
            StrSQL.Append(" ( " & vbCrLf)
            StrSQL.Append("     ID_Notifica, " & vbCrLf)
            StrSQL.Append(" 	PivaSuperUser, " & vbCrLf)
            StrSQL.Append(" 	TipoNotifica_ID, " & vbCrLf)
            StrSQL.Append(" 	TipoNotifica_Chiave, " & vbCrLf)
            StrSQL.Append(" 	Mittente, " & vbCrLf)
            StrSQL.Append(" 	Destinatario, " & vbCrLf)
            StrSQL.Append(" 	SubscriberId, " & vbCrLf)
            StrSQL.Append(" 	TestoNotifica, " & vbCrLf)
            StrSQL.Append(" 	DeepLink, " & vbCrLf)
            StrSQL.Append(" 	MediaURL, " & vbCrLf)
            StrSQL.Append(" 	Piattaforma, " & vbCrLf)
            StrSQL.Append(" 	DataOraDaCuiInviare, " & vbCrLf)
            StrSQL.Append(" 	Username_Creazione, " & vbCrLf)
            StrSQL.Append(" 	Username_Modifica " & vbCrLf)
            StrSQL.Append(" ) VALUES ( " & vbCrLf)
            StrSQL.Append(String.Format(" {0}, ", Agro_SQL_SaveNum(nuovoId)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(notifica("PivaSuperUser").ToString)) & vbCrLf)
            StrSQL.Append(String.Format(" {0}, ", Agro_SQL_SaveNum(notifica("IdServizio"))) & vbCrLf)
            StrSQL.Append(" '', " & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(mittente)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(notifica("Utente").ToString)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(notifica("SubscriberId").ToString)) & vbCrLf)
            StrSQL.Append(" '', " & vbCrLf)
            StrSQL.Append(" '', " & vbCrLf)
            StrSQL.Append(" '', " & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(notifica("Piattaforma").ToString)) & vbCrLf)
            StrSQL.Append(String.Format(" {0}, ", Agro_SQL_SaveDateTime(DataInvio)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSQL.Append(" ) " & vbCrLf)

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp
    End Function

    Public Function AggiornaInvioNotifica(ByVal idNotifica As Int32,
                                          ByVal InvioOK As Boolean,
                                          ByVal MessaggioNotifica As String,
                                          objParametri As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.Programmazione_Notifica_W.AggiornaInvioNotifica()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE NotifichePush_Programmazione SET " & vbCrLf)

            StrSQL.Append(String.Format(" Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername)) & vbCrLf)
            StrSQL.Append(" , Data_Modifica = GETDATE() " & vbCrLf)

            If InvioOK Then
                StrSQL.Append(" , Spedita = 1 " & vbCrLf)
                StrSQL.Append(" , Spedizione_DataOra = GETDATE() " & vbCrLf)
            End If

            If Not String.IsNullOrWhiteSpace(MessaggioNotifica) Then
                StrSQL.Append(String.Format(" , Spedizione_Risultato = '{0}' ", Agro_SQL_SaveText(MessaggioNotifica)) & vbCrLf)
            End If

            StrSQL.Append(" WHERE " & vbCrLf)
                StrSQL.Append(String.Format(" ID_Notifica = {0} ", Agro_SQL_SaveNum(idNotifica)) & vbCrLf)

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp

    End Function
End Class
