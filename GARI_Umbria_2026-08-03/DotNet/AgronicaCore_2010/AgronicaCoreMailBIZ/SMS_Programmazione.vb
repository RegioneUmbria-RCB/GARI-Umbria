


Imports System.Net
Imports AgronicaCoreDataProvider
Imports AgronicaCoreMailBIZ.smssender.gateway.sms
Imports AgronicaCoreVarieBIZ

Public Class SMS_Programmazione


#Region "Wrapper su servizio SMS"



    Public Function interrogaCredito(
       userid As String,
       password As String
    ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            'connessione SSL'
            ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf AcceptAllCertifications)


            r.RispostaOK = True

            'oggetto di risposta del ws per il credito residuo'
            Dim result As CreditoResiduoResult

            'wrapper per i Web Services di SMS Sender'
            Dim smsWrapper As New SMS

            'momentaneamente utilizzo un solo numero di telefono'

            'chiamata per l'invio'
            'user e password, mittente, lista di destinatari, testo del messaggio'
            result = smsWrapper.CreditoResiduo(userid, password)

            'esito dell'invio: visualizzo lo stato nell'apposito box nel form e nella messagebox'
            If result.Successo Then

                'in caso di successo'
                r.RispostaStringa = result.Credito

            Else

                'in caso di fallimento'
                r.RispostaOK = False
                r.Errore = "Error: " & result.CodiceErrore & ": " & result.MessaggioErrore
                'MsgBox("Error: " & result.CodiceErrore & ": " & result.MessaggioErrore, MsgBoxStyle.Critical)'

            End If

        Catch ex As Exception

            'gestione delle eccezioni'

            r.RispostaOK = False
            r.Errore = ("Exception: " & ex.Message)


        End Try

        Return r

    End Function


    Public Function inviaSMS(
       ByVal numeriCell() As String,
       userid As String,
       password As String,
       mittente As String,
       testo As String
    ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            'connessione SSL'
            ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf AcceptAllCertifications)

            r.RispostaOK = True

            Dim smsWrapper As New SMS
            Dim result As InvioSmsResult

            'momentaneamente utilizzo un solo numero di telefono'

            'chiamata per l'invio'
            'user e password, mittente, lista di destinatari, testo del messaggio'
            result = smsWrapper.InvioSms(userid, password, mittente, numeriCell, testo)

            'esito dell'invio: visualizzo lo stato nell'apposito box nel form e nella messagebox'
            If result.Successo Then

                'in caso di successo'
                r.RispostaOK = True
                r.RispostaStringa = result.IdUnivoco

            Else

                'in caso di fallimento'
                r.RispostaOK = False
                r.Errore = "Error: " & result.CodiceErrore & ": " & result.MessaggioErrore
                'MsgBox("Error: " & result.CodiceErrore & ": " & result.MessaggioErrore, MsgBoxStyle.Critical)'

            End If

        Catch ex As Exception

            'gestione delle eccezioni'

            r.RispostaOK = False
            r.Errore = ("Exception: " & ex.Message)

        End Try

        Return r

    End Function


    Public Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean

        Return True

    End Function

#End Region

End Class


Public Class SMS_Programmazione_R

    Public Function Lettura(ByVal ID_SMS As Integer?,
                            ByVal TipoSMS_ID As Integer?,
                            ByVal TipoSMS_Chiave As String,
                            ByVal DataOraDaCuiInviare_Inizio As DateTime?,
                            ByVal DataOraDaCuiInviare_Fine As DateTime?,
                            ByVal Spedito As Boolean?,
                            ByVal AnnullatoInvio As Boolean?,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As RispostaStandard

        Dim mp_R As New AgronicaCoreMailDAL.Mail_Programmazione_R()

        Dim rval As New RispostaStandard

        Try
            Dim esito As DataTable = mp_R.Leggi(ID_SMS,
                                              TipoSMS_ID,
                                              TipoSMS_Chiave,
                                              DataOraDaCuiInviare_Inizio,
                                              DataOraDaCuiInviare_Fine,
                                              Spedito,
                                              AnnullatoInvio,
                                              xFiltroAggiuntivo,
                                              xOrderBy,
                                              objParametri)

            rval.RispostaOK = esito.ToString
            rval.RispostaStringa = "Messaggio letto correttamente."

        Catch ex As Exception

            rval.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval

    End Function

End Class

Public Class SMS_Programmazione_W

    Public Function AccodaPerInvio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal TipoSMS_ID As Integer,
                            ByVal TipoSMS_Chiave As String,
                            ByVal Mittente As String,
                            ByVal Destinatari As String,
                            ByVal Testo As String,
                            ByVal DataOraDaCuiInviare As DateTime?,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As RispostaStandard

        '  Prendo l'indice dalle agrosequenze
        Dim seq As New Agro_Sequenze()
        Dim ID_SMS As Integer = seq.NuovoId_Tabella("SMS_ID_SMS", 0, 2000000000, objParametri)

        Dim mp_W As New AgronicaCoreMailDAL.SMS_Programmazione_W()

        Dim rval As New RispostaStandard

        Try


            Dim esito As Boolean = mp_W.Scrivi(
            objParametri,
            ID_SMS,
            TipoSMS_ID,
            TipoSMS_Chiave,
            Mittente,
            Destinatari,
            Testo,
            DataOraDaCuiInviare,
            False,
            Nothing,
            "",
            False,
            Nothing,
            ""
         )

            rval.RispostaOK = esito
            rval.RispostaStringa = "Messaggio accodato correttamente."

        Catch ex As Exception

            rval.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

End Class
