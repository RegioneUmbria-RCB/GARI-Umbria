Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreAnagrafeDAL.UMASetup_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json



Public Class UMASetup
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Setup_LeggiTabella(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMASetup_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMASetup.LeggiSetup()"
        Try
            Dim Setup As DataTable = AgronicaDAL.LeggiSetup(0, objParametri)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dt.Columns.Add(New DataColumn("Anno", GetType(Integer)))

            Dt.Columns.Add(New DataColumn("Per_riduzione", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Per_Mag_Terreno_B", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Per_Mag_Terreno_Medio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Per_Mag_Terreno_Tenace", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Altre_Cfg", GetType(String)))
            Dt.Columns.Add(New DataColumn("Nr_Litri_Maggiorazione", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Percentuale_Integrazione_Terzista", GetType(Double)))

            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))

            Dt.Columns.Add(New DataColumn("Inviato", GetType(Short)))
            Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Creazione", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Modifica", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))
            Dt.Columns.Add(New DataColumn("Modificabile", GetType(Boolean)))

            For Each elem As DataRow In Setup.Rows
                Dim d = Dt.NewRow

                d("Anno") = elem("Anno")

                d("Per_riduzione") = elem("Per_riduzione")
                d("Per_Mag_Terreno_B") = elem("Per_Mag_Terreno_B")
                d("Per_Mag_Terreno_Medio") = elem("Per_Mag_Terreno_Medio")
                d("Per_Mag_Terreno_Tenace") = elem("Per_Mag_Terreno_Tenace")
                d("Altre_Cfg") = elem("Altre_Cfg")
                d("Nr_Litri_Maggiorazione") = elem("Nr_Litri_Maggiorazione")
                d("Percentuale_Integrazione_Terzista") = elem("Percentuale_Integrazione_Terzista")

                d("Validita_Inizio") = elem("Validita_Inizio")
                d("Validita_Fine") = elem("Validita_Fine")

                d("Inviato") = elem("Inviato")
                d("DataInvio") = elem("DataInvio")
                d("Data_Creazione") = elem("Data_Creazione")
                d("Data_Modifica") = elem("Data_Modifica")
                d("Username_Creazione") = elem("Username_Creazione")
                d("Username_Modifica") = elem("Username_Modifica")
                d("Modificabile") = False

                Dt.Rows.Add(d)
            Next
            Return Dt
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Public Function Setup_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMASetup.Setup_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String
        Dim efConnString As String
        Dim righeCancellate As List(Of UMASetupDto)
        Dim righeModificate As List(Of UMASetupDto)
        Dim righeInserite As List(Of UMASetupDto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of UMASetupDto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of UMASetupDto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of UMASetupDto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            '------------------------------ Verifica validità righe input ------------------------------    
            Errori = String.Empty
            If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                ControlliValidita_UMASetup(righeInserite, righeModificate, objParametri, efConnString, Errori)
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
                Dim AgronicaDAL As New AgronicaCoreAnagrafeDAL.UMASetup_W

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

    Public Shared Sub ControlliValidita_UMASetup(ByVal righeInserite As List(Of UMASetupDto),
                                                      ByVal righeModificate As List(Of UMASetupDto),
                                                      ByVal objParametri As AgronicaCoreParametri,
                                                      ByVal efConnString As String,
                                                      ByRef Errori As String)

        Dim agronicaDAL As New UMASetup_R

        ' --------- Controlla se le righe inserite/modificate contengono degli errori --------------
        ' ------------------------------------------------------------------------------------------
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

        ' --------------------------- La chiave primaria non dovrebbe esistere già in db ---------------------------
        If String.IsNullOrEmpty(Errori) AndAlso Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
            For Each l In righeInserite
                Dim exists = agronicaDAL.VerificaElementoNonEsisteInDB(l, efConnString)

                If exists Then
                    Errori = "L'elemento existe già in database: Anno='" & l.Anno & "'"
                    Return
                End If
            Next
        End If
    End Sub
    Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As UMASetupDto)

        ' ******************************** Nuove righe ********************************
        ' Tutti i campi che compongono la chiave primaria sono obbligatori
        ' Tutti gli altri campi non devono contenere valori di null.
        Dim now As Date = Date.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")

        If isNew Then
            r.Data_Creazione = now
            r.Username_Creazione = usernameModifica
        End If

        r.Data_Modifica = now
        r.Username_Modifica = usernameModifica

        If r.DataInvio Is Nothing Then
            r.DataInvio = New Date(1900, 1, 1)
        End If

        If r.Validita_Inizio Is Nothing Then
            r.Validita_Inizio = New Date(1900, 1, 1)
        End If

        If r.Validita_Fine Is Nothing Then
            r.Validita_Fine = New Date(2100, 12, 31)
        End If

    End Sub
    Shared Sub VerificaValiditaRigaInserita(r As UMASetupDto,
                                    i As Integer,
                                    ByRef errori As String)
        Dim campiInvalidi = New List(Of String)


        If r.Anno = 0 Then
            campiInvalidi.Add("'Anno'")
        End If


        If campiInvalidi.Count = 1 Then
            errori += "L'elemento con indice " & i + 1 & ". Il campo " & campiInvalidi(0) & " è obbligatorio."
        ElseIf campiInvalidi.Count > 1 Then
            errori += "L'elemento con indice " & i + 1 & ". I campi: " & String.Join(", ", campiInvalidi) & " sono obbligatori."
        End If
    End Sub
End Class
