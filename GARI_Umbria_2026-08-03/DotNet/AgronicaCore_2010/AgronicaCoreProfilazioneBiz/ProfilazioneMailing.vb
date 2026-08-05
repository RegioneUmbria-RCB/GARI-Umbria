Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMailBIZ
Imports AgronicaCoreVarieBIZ

Public Class ProfilazioneMailing

    Public Function AccodaEmailNotifica(
        emailProfilazioneDestinatario As String,
        emailProfilazioneOggetto As String,
        emailProfilazioneTestoBody As String,
        TipoMail_Chiave As String,
        objParametri_Server As AgronicaCoreParametri
    ) As RispostaStandard

        Dim rval As New RispostaStandard


        Dim FlagConnessioneLocaleServer As Boolean = False
        Dim FlagTransazioneLocaleServer As Boolean = False

        'Apro la connessione, transazione al DB server
        ConnessioniTransazioni.ApriConnessioneXCoreBiz(
            FlagConnessioneLocaleServer,
            FlagTransazioneLocaleServer,
            objParametri_Server
        )

        Try



            Dim scriviAccodaMail As New Mail_Programmazione_W
            scriviAccodaMail.ScriviNuovaMail(
                objParametri_Server,
                enum_MailTipo.ProfilazioneUtenti,
                TipoMail_Chiave,
                "profilazioneUtenti@agronica.it",
                emailProfilazioneDestinatario,
                "",
                "",
                emailProfilazioneOggetto,
                emailProfilazioneTestoBody,
                True,
                "",
                Now()
            )


            'Chiudo la connessione al DB server
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocaleServer, objParametri_Server)

            rval.RispostaOK = True
            rval.RispostaStringa = "Messaggio accodato."

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            rval.RispostaOK = False
            rval.Errore = "Errore in accodamento email: " &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocaleServer, objParametri_Server)

        End Try

        Return rval

    End Function

End Class
