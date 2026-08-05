Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Public Class Programmazione_Notifica_R

End Class

Public Class Programmazione_Notifica_W

    Public Function Accoda(ByVal mittente As String,
                           ByVal ServizioSottoscritto As Int32,
                           ByVal Piattaforma As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As RispostaStandard

        Dim messaggioErrore As String
        Dim xRead As New AgronicaCoreNotifichePushDAL.Notifica_Utente_R
        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        Try

            DT = xRead.Leggi(ServizioSottoscritto,
                             Piattaforma,
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri,
                             objParametri_Utenti)

            If DT IsNot Nothing Then
                Dim xWrite As New AgronicaCoreNotifichePushDAL.Programmazione_Notifica_W

                For Each row In DT.Rows
                    xResp.RispostaOK = xWrite.Accoda(mittente, Date.Now, row, objParametri, objParametri_Utenti)
                Next

            End If

            xResp.RispostaStringa = "Operazione Terminata con successo"

        Catch ex As Exception

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            xResp.RispostaOK = False
            xResp.Errore = messaggioErrore

        End Try

        Return xResp

    End Function

    Public Function InviaNotifiche(ByVal APIURL As String,
                                   ByVal APIToken As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As RispostaStandard

        Dim messaggioErrore As String
        Dim xRead As New AgronicaCoreNotifichePushDAL.Programmazione_Notifica_R
        Dim xWrite As New AgronicaCoreNotifichePushDAL.Programmazione_Notifica_W
        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Try

            DT = xRead.LeggiDaInviare("",
                                      "",
                                      objParametri,
                                      objParametri_Utenti)

            Dim sender = New AgronicaCoreUtility.RestSharpHelper(APIURL)
            sender.AddHeader("Authorization", String.Format("key={0}", APIToken))

            For Each row In DT.Rows
                Dim ErroreNotifica As String = ""

                Dim body As New BodyNotifica
                body.SubscriberIDtoReplace = row("SubscriberId").ToString
                body.data = New BodyNotifica.DataNotifica
                body.data.type = row("TipoNotifica_ID").ToString
                body.data.url = row("MediaUrl").ToString
                body.data.dl = row("DeepLink").ToString

                'Necessario fare replace della proprietà SubscriberIDtoReplace perchè non è possibile
                'usare "to" come nome di una proprietà in quanto parola chiave del linguaggio

                sender.AddOrReplaceBody(Newtonsoft.Json.JsonConvert.SerializeObject(body).Replace("SubscriberIDtoReplace", "to"))

                Dim APIResp = sender.Execute()

                Dim esitoInvio = APIResp.StatusCode.Equals(Net.HttpStatusCode.OK)

                Dim messaggioNotifica = APIResp.Content.ToString

                xWrite.AggiornaInvioNotifica(Convert.ToInt32(row("ID_Notifica").ToString),
                                             esitoInvio,
                                             messaggioNotifica,
                                             objParametri,
                                             objParametri_Utenti)

                ' rimuovo sottoscrizione se l'app risulta disinstallata
                If messaggioNotifica.Contains("error") AndAlso messaggioNotifica.Contains("NotRegistered") Then
                    Dim xWriteNotifica As New AgronicaCoreNotifichePushDAL.Notifica_Utente_W
                    Dim result = xWriteNotifica.Cancella(row("SubscriberId").ToString, CInt(row("TipoNotifica_ID")), objParametri, objParametri_Utenti)
                End If

                xResp.RispostaOK = esitoInvio

                If Not esitoInvio Then
                    xResp.Errore = messaggioNotifica
                End If
            Next

            If String.IsNullOrWhiteSpace(xResp.Errore) Then
                xResp.RispostaStringa = "Operazione Terminata con successo"
            End If

        Catch ex As Exception

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            xResp.RispostaOK = False
            xResp.Errore = messaggioErrore

        End Try

        Return xResp

    End Function

    Private Class BodyNotifica
        Public SubscriberIDtoReplace As String
        Public data As DataNotifica

        Public Class DataNotifica
            Public type As String
            Public url As String
            Public dl As String
        End Class
    End Class

End Class
