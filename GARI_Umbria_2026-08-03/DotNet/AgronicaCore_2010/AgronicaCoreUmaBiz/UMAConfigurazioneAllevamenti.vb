Imports System.Transactions
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaDal.UMAConfigurazioneAllevamenti_W
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class UMAConfigurazioneAllevamenti
    Inherits AgronicaCoreDataProvider.DataProvider
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Public Function UMAConfigurazioneAllevamenti_LeggiTabella(ByRef objParametri As AgronicaCoreParametri, Optional InizioValidita As String = "", Optional FineValidita As String = "") As DataTable
        Dim AgronicaDAL As New UMAConfigurazioneAllevamenti_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMAConfigurazioneAllevamenti.UMAConfigurazioneAllevamenti_LeggiTabella()"
        Try
            Dim configurazioneAllevamenti As DataTable = AgronicaDAL.LeggiUMAConfigurazioneAllevamenti(objParametri, InizioValidita, FineValidita)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Return configurazioneAllevamenti
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function


    Public Function UMAConfigurazioneAllevamenti_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMAConfigurazioneAllevamenti.UMAConfigurazioneAllevamenti_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String = ""
        Dim efConnString As String
        Dim righeCancellate As List(Of UMAConfigurazioneAllevamentiDto)
        Dim righeModificate As List(Of UMAConfigurazioneAllevamentiDto)
        Dim righeInserite As List(Of UMAConfigurazioneAllevamentiDto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of UMAConfigurazioneAllevamentiDto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of UMAConfigurazioneAllevamentiDto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of UMAConfigurazioneAllevamentiDto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            '------------------------------ Verifica validità righe input ------------------------------    
            Errori = String.Empty
            If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                ControlliValidita_UMAConfigurazioneAllevamenti(righeInserite, righeModificate, righeCancellate, objParametri, efConnString, Errori)
            End If

            If (Not String.IsNullOrEmpty(Errori)) Then
                risposta.RispostaOK = False
                risposta.Errore = Errori
                Return risposta
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Using scope As New TransactionScope()
            Try
                ' ------------------------------------- Salva i dati nel database -------------------------------------
                Dim context As New Gias_DeveloperServer_Entities(efConnString)
                Dim AgronicaDAL As New UMAConfigurazioneAllevamenti_W

                If String.IsNullOrEmpty(Errori) Then
                    '------------------------- Salva le nuove righe inserite -------------------------
                    If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
                        AgronicaDAL.AggiungiNuovi(righeInserite, context, objParametri)
                    End If

                    '------------------------- Inserici le righe modificate -------------------------
                    If Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
                        AgronicaDAL.Aggiorna(righeModificate, context, objParametri)
                    End If

                    '------------------------- Rimuovi le righe cancellate -------------------------
                    If Not righeCancellate Is Nothing AndAlso righeCancellate.Count > 0 Then
                        AgronicaDAL.Rimuovi(righeCancellate, context, objParametri)
                    End If
                End If
                scope.Complete()
                scope.Dispose()

                risposta.RispostaOK = True
                risposta.RispostaStringa = "L'operazione è stata completata con successo."

                Return risposta
            Catch ex As Exception
                scope.Dispose()

                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
                Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
            End Try
        End Using
    End Function

    Public Function AgronicaCoreDataProvider_Leggi(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMAConfigurazioneAllevamenti_R
        Dim MessaggioErrore As String
        Dim result As DataTable
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMAConfigurazioneAllevamenti.AgronicaCoreDataProvider_Leggi()"
        Try
            result = AgronicaDAL.Leggi(0, 0, 0, 0, 0, 0, 0, New Date(1900, 1, 1), New Date(2100, 12, 31), enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, objParametri_Utenti)
        Catch ex As Exception
            result = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result

    End Function

    Public Function Dropdown_Operazione(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreAnagrafeBIZ.UMAConfigurazioneAllevamenti.Dropdown_Operazione()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Tipo_Operazione", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Tipo_Operazione_Des", GetType(String)))

        Try

            Dim d = dt.NewRow
            d("Tipo_Operazione") = 1
            d("Tipo_Operazione_Des") = "Ordinaria"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Tipo_Operazione") = 2
            d("Tipo_Operazione_Des") = "Straordinaria"
            dt.Rows.Add(d)

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Shared Function ElencoAllevamenti() As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim handleTipiAllevamenti As New AgronicaCoreMetaSchemaDAL.UMA_Allevamenti_R()
            Dim dt = handleTipiAllevamenti.Leggi("010", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    Public Shared Sub ControlliValidita_UMAConfigurazioneAllevamenti(ByVal righeInserite As List(Of UMAConfigurazioneAllevamentiDto),
                                                                     ByVal righeModificate As List(Of UMAConfigurazioneAllevamentiDto),
                                                                     ByVal righeCancellate As List(Of UMAConfigurazioneAllevamentiDto),
                                                                     ByVal objParametri As AgronicaCoreParametri,
                                                                     ByVal efConnString As String,
                                                                     ByRef Errori As String)

        Dim agronicaDAL As New UMAConfigurazioneAllevamenti_R

        ' --------- Controlla se le righe inserite/modificate contengono degli errori --------------

        If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
            For i = 0 To righeInserite.Count - 1
                VerificaValiditaRigaInserita(righeInserite(i), i, Errori)
                If (String.IsNullOrEmpty(Errori)) Then
                    ImpostaValoriDiDefaultSeNecessario(True, objParametri.UtenteUsername, righeInserite(i))
                End If
            Next
        End If

        If String.IsNullOrEmpty(Errori) AndAlso Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
            For i = 0 To righeModificate.Count - 1
                VerificaValiditaRigaInserita(righeModificate(i), i, Errori)
                If (String.IsNullOrEmpty(Errori)) Then
                    ImpostaValoriDiDefaultSeNecessario(False, objParametri.UtenteUsername, righeModificate(i))
                Else
                    Exit For
                End If
            Next
        End If

        If String.IsNullOrEmpty(Errori) Then
            Dim ListaRighe As New List(Of UMAConfigurazioneAllevamentiDto)
            ListaRighe.AddRange(righeInserite)
            ListaRighe.AddRange(righeModificate)
            If ListaRighe.Count > 0 Then
                Errori = agronicaDAL.VerificaPeriodiSovrappostiDB(ListaRighe, righeCancellate, efConnString, objParametri)
            End If
        End If

    End Sub

    Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As UMAConfigurazioneAllevamentiDto)

        ' ******************************** Nuove righe ********************************
        ' Tutti i campi che compongono la chiave primaria sono obbligatori
        ' Tutti gli altri campi non devono contenere valori di null.
        Dim now As Date = Date.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")

        If isNew Then
            r.Data_Creazione = now
            r.Username_Creazione = usernameModifica
            r.Regione_Cod = "010"
            r.Tipo_Operazione = 1
            r.Gasolio_Lt = 0
            r.Benzina_Lt = 0
            r.N_Max_Allevamenti = 0
            r.Qta_Aggiuntiva_Carro = 0
        End If
        r.Data_Modifica = now
        r.Username_Modifica = usernameModifica

        'If r.datainvio Is Nothing Then
        '    r.datainvio = New Date(1900, 1, 1)
        'End If

        If r.Validita_Inizio Is Nothing Then
            r.Validita_Inizio = New Date(1900, 1, 1)
        End If

        If r.Validita_Fine Is Nothing Then
            r.Validita_Fine = New Date(2100, 12, 31)
        End If

    End Sub
    Shared Sub VerificaValiditaRigaInserita(r As UMAConfigurazioneAllevamentiDto,
                                    i As Integer,
                                    ByRef errori As String)
        Dim campiInvalidi = New List(Of String)


        If r.Regione_Cod = "" Then
            campiInvalidi.Add("'Regione_Cod'")
        End If
        If r.UMA_All_Cod = "" Then
            campiInvalidi.Add("'UMA_All_Cod'")
        End If
        If r.Tipo_Operazione < 1 Or r.Tipo_Operazione > 2 Then
            campiInvalidi.Add("'Tipo_Operazione'")
        End If

        If campiInvalidi.Count = 1 Then
            errori += "L'elemento con indice " & i + 1 & ". Il campo " & campiInvalidi(0) & " è obbligatorio."
        ElseIf campiInvalidi.Count > 1 Then
            errori += "L'elemento con indice " & i + 1 & ". I campi: " & String.Join(", ", campiInvalidi) & " sono obbligatori."
        End If
    End Sub




End Class
