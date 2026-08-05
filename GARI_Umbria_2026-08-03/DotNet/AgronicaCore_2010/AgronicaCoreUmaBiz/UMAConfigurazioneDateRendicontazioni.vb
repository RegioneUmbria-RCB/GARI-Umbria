Imports System.Transactions
'Imports AgronicaCoreAnagrafeDAL
'Imports AgronicaCoreAnagrafeDAL.UMASetup_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaDal.UMAConfigurazioneDateRendicontazioni_W
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class UMAConfigurazioneDateRendicontazioni

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function DateRendicontazioni_LeggiTabella(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMAConfigurazioneDateRendicontazioni_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMASetup.LeggiSetup()"
        Try
            Dim DateRendicont As DataTable = AgronicaDAL.LeggiDateRendicontazioni(objParametri)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt.Columns.Add(New DataColumn("Tipo_Azienda", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Tipo_AziendaDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Anno_Richiesta", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Data_Inizio_Rendicontazione", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Data_Limite_INS_Azienda_Terzista", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Data_Fine_Rendicontazione", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Termine_Ultimo_Rendicontazione", GetType(Date)))
            'Dt.Columns.Add(New DataColumn("Inviato", GetType(Short)))
            'Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
            'Dt.Columns.Add(New DataColumn("Data_Creazione", GetType(DateTime)))
            'Dt.Columns.Add(New DataColumn("Data_Modifica", GetType(DateTime)))
            'Dt.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))

            'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Data_Limite_INS_Richiesta_Anticipo", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Data_Fine_Blocco_Rendic_Conto_Proprio", GetType(Date)))


            For Each elem As DataRow In DateRendicont.Rows
                Dim d = Dt.NewRow
                Dim tipoAzienda As Integer = elem("Tipo_Azienda")
                Dim tipoAziendaDes As String = ""
                Select Case tipoAzienda
                    Case enum_TipoAzienda_UMA.Azienda_Agricola_Privata
                        tipoAziendaDes = Azienda_Agricola_Privata
                    Case enum_TipoAzienda_UMA.Azienda_Terzista
                        tipoAziendaDes = Azienda_Terzista
                    Case enum_TipoAzienda_UMA.Cooperativa_Agricola
                        tipoAziendaDes = Cooperativa_Agricola
                    Case enum_TipoAzienda_UMA.Azienda_Agricola_Istituzioni_Pubbliche
                        tipoAziendaDes = Azienda_Agricola_Istituzioni_Pubbliche
                    Case enum_TipoAzienda_UMA.Consorzio_Bonifica_Irrigazione
                        tipoAziendaDes = Consorzio_Bonifica_Irrigazione
                End Select

                d("chiave") = tipoAzienda.ToString() + "_" + elem("Anno_Richiesta").ToString()
                d("Tipo_Azienda") = tipoAzienda
                d("Tipo_AziendaDes") = tipoAziendaDes
                d("Anno_Richiesta") = elem("Anno_Richiesta")
                d("Data_Inizio_Rendicontazione") = elem("Data_Inizio_Rendicontazione")
                d("Data_Limite_INS_Azienda_Terzista") = elem("Data_Limite_INS_Azienda_Terzista")
                d("Data_Fine_Rendicontazione") = elem("Data_Fine_Rendicontazione")
                d("Termine_Ultimo_Rendicontazione") = elem("Termine_Ultimo_Rendicontazione")
                'd("Inviato") = elem("Inviato")
                'd("DataInvio") = elem("DataInvio")

                'd("Data_Creazione") = elem("Data_Creazione")
                'd("Data_Modifica") = elem("Data_Modifica")

                'd("Username_Creazione") = elem("Username_Creazione")
                'd("Username_Modifica") = elem("Username_Modifica")
                'd("Validita_Inizio") = elem("Validita_Inizio")
                'd("Validita_Fine") = elem("Validita_Fine")
                d("Data_Limite_INS_Richiesta_Anticipo") = elem("Data_Limite_INS_Richiesta_Anticipo")
                d("Data_Fine_Blocco_Rendic_Conto_Proprio") = elem("Data_Fine_Blocco_Rendic_Conto_Proprio")

                Dt.Rows.Add(d)
            Next
            Return Dt
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Public Function SetupDateRendicontazione_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMASetup.Setup_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String
        Dim efConnString As String
        Dim righeCancellate As List(Of UMASetupDateRendicontazioneDto)
        Dim righeModificate As List(Of UMASetupDateRendicontazioneDto)
        Dim righeInserite As List(Of UMASetupDateRendicontazioneDto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of UMASetupDateRendicontazioneDto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of UMASetupDateRendicontazioneDto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of UMASetupDateRendicontazioneDto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            '------------------------------ Verifica validità righe input ------------------------------    
            Errori = String.Empty
            If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                ControlliValidita_UMASetupDateRendicontazioni(righeInserite, righeModificate, objParametri, efConnString, Errori)
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
                Dim AgronicaDAL As New AgronicaCoreUmaDal.UMAConfigurazioneDateRendicontazioni_W

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

    Public Shared Sub ControlliValidita_UMASetupDateRendicontazioni(ByVal righeInserite As List(Of UMASetupDateRendicontazioneDto),
                                                      ByVal righeModificate As List(Of UMASetupDateRendicontazioneDto),
                                                      ByVal objParametri As AgronicaCoreParametri,
                                                      ByVal efConnString As String,
                                                      ByRef Errori As String)

        Dim agronicaDAL As New UMAConfigurazioneDateRendicontazioni_R

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
                    Errori = "L'elemento existe già in database: Anno Richiesta='" & l.Anno_Richiesta & "' Tipo Azienda='" & l.Tipo_Azienda & "'"
                    Return
                End If
            Next
        End If
    End Sub

    Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As UMASetupDateRendicontazioneDto)

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

        If r.Data_Limite_INS_Azienda_Terzista Is Nothing Then
            r.Data_Limite_INS_Azienda_Terzista = New Date(1900, 1, 1)
        End If

        If r.Data_Limite_INS_Richiesta_Anticipo Is Nothing Then
            r.Data_Limite_INS_Richiesta_Anticipo = New Date(1900, 1, 1)
        End If

        If r.Data_Fine_Blocco_Rendic_Conto_Proprio Is Nothing Then
            r.Data_Fine_Blocco_Rendic_Conto_Proprio = New Date(2100, 12, 31)
        End If

    End Sub

    Shared Sub VerificaValiditaRigaInserita(r As UMASetupDateRendicontazioneDto,
                                    i As Integer,
                                    ByRef errori As String)
        Dim campiInvalidi = New List(Of String)


        If r.Anno_Richiesta = 0 Then
            campiInvalidi.Add("'Anno Richiesta'")
        End If

        If r.Tipo_Azienda = 0 Then
            campiInvalidi.Add("'Tipo Azienda'")
        End If


        If campiInvalidi.Count = 1 Then
            errori += "L'elemento con indice " & i + 1 & ". Il campo " & campiInvalidi(0) & " è obbligatorio."
        ElseIf campiInvalidi.Count > 1 Then
            errori += "L'elemento con indice " & i + 1 & ". I campi: " & String.Join(", ", campiInvalidi) & " sono obbligatori."
        End If
    End Sub

End Class
