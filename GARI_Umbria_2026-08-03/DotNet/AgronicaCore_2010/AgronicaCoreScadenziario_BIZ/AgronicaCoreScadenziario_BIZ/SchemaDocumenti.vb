Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreScadenziario
Imports AgronicaCoreScadenziario.SchemaDocumenti_R
Imports System.Transactions
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreScadenziario_DAL
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreScadenziario.SchemaDocumenti_W

Public Class SchemaDocumenti
    Inherits AgronicaCoreDataProvider.DataProvider
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Public Function SchemaDocumenti_LeggiTabella(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New SchemaDocumenti_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.LeggiSchemaDocumenti"
        Try
            Dim schemaDocumneti As DataTable = AgronicaDAL.LeggiSchemaDocumenti(objParametri)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Return schemaDocumneti
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function


    Public Function SchemaDocumenti_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.SchemaDocumenti_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String = ""
        Dim efConnString As String
        Dim righeCancellate As List(Of SchemaDocumentiDto)
        Dim righeModificate As List(Of SchemaDocumentiDto)
        Dim righeInserite As List(Of SchemaDocumentiDto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of SchemaDocumentiDto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of SchemaDocumentiDto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of SchemaDocumentiDto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            '------------------------------ Verifica validità righe input ------------------------------    
            Errori = String.Empty
            If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                ControlliValidita_SchemaDocumenti(righeInserite, righeModificate, objParametri, efConnString, Errori)
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
                Dim AgronicaDAL As New SchemaDocumenti_W

                If String.IsNullOrEmpty(Errori) Then
                    '------------------------- Salva le nuove righe inserite -------------------------
                    If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
                        AgronicaDAL.AggiungiNouvi(righeInserite, context, objParametri)
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
        Dim AgronicaDAL As New SchemaDocumenti_R
        Dim MessaggioErrore As String
        Dim result As DataTable
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim NomeRoutine As String = "DpiBIZ.CentroAziendale_R.AgronicaCoreDataProvider_Leggi()"
        Try
            result = AgronicaDAL.Leggi("", 0, 0, 0, "", 0, New Date(1900, 1, 1), New Date(2100, 12, 31), enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, objParametri_Utenti)
        Catch ex As Exception
            result = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result

    End Function


    'da ripetere per ogni ddl
    Public Function Dropdown_Servizi(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Servizi()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Servizio_Cod", GetType(String)))
        dt.Columns.Add(New DataColumn("Servizio_Des", GetType(String)))

        Dim biz As New AgronicaCoreMetaSchemaDAL.Servizi_R

        Dim elenco_macrouso = biz.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
        For Each row In elenco_macrouso.Rows
            Dim d = dt.NewRow
            d("Servizio_Cod") = row("Servizio_Cod")
            d("Servizio_Des") = row("Servizio_Des")
            dt.Rows.Add(d)
        Next

        Try

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Stato_Da(Servizio_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Stato_Da()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Stato_Da", GetType(String)))
        dt.Columns.Add(New DataColumn("Stato_Da_Des", GetType(String)))

        Dim biz As New AgronicaCoreMetaSchemaDAL.WAnagrafica_Stati_R





        Dim elenco_macrouso = biz.StatiDaServizio(Servizio_Cod, "", "", objParametri_Server)
        For Each row In elenco_macrouso.Rows
            Dim d = dt.NewRow
            d("Stato_Da") = row("Stato_Origine_Cod")
            d("Stato_Da_Des") = row("WAnagraficaStati_Des")
            dt.Rows.Add(d)
        Next

        Try

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Stato_A(Servizio_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Stato_A()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Stato_A", GetType(String)))
        dt.Columns.Add(New DataColumn("Stato_A_Des", GetType(String)))

        Dim biz As New AgronicaCoreMetaSchemaDAL.WAnagrafica_Stati_R

        Dim elenco_macrouso = biz.StatiDaServizio(Servizio_Cod, "", "", objParametri_Server)
        For Each row In elenco_macrouso.Rows
            Dim d = dt.NewRow
            d("Stato_A") = row("Stato_Origine_Cod")
            d("Stato_A_Des") = row("WAnagraficaStati_Des")
            dt.Rows.Add(d)
        Next

        Try

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Tipologia(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Tipologia()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Tipologia", GetType(String)))
        dt.Columns.Add(New DataColumn("Tipologia_Des", GetType(String)))

        Dim biz As New Alert_Tipologia_R
        Dim elenco_macrouso = biz.Leggi(0, 0, "", False, objParametri_Server)
        For Each row In elenco_macrouso.Rows
            Dim d = dt.NewRow
            d("Tipologia") = row("ID_Tipologia")
            d("Tipologia_Des") = row("Nome")
            dt.Rows.Add(d)
        Next

        Try

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Ambito(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Ambito()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Ambito", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Ambito_Des", GetType(String)))

        Try

            Dim d = dt.NewRow
            d("Ambito") = -14
            d("Ambito_Des") = "UMA Carburanti"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Ambito") = -15
            d("Ambito_Des") = "Check List"
            dt.Rows.Add(d)

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Fase(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Fase()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Fase", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Fase_Des", GetType(String)))

        Try

            Dim d = dt.NewRow
            d("Fase") = -16
            d("Fase_Des") = "UMA Prima Richiesta"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Fase") = -17
            d("Fase_Des") = "UMA Approvazione Prima Richiesta"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Fase") = -18
            d("Fase_Des") = "UMA Richiesta Integrativa"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Fase") = -19
            d("Fase_Des") = "UMA Approvazione Richiesta Integrativa"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Fase") = -20
            d("Fase_Des") = "UMA Rendicontazione"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Fase") = -21
            d("Fase_Des") = "UMA Approvazione Rendicontazione"
            dt.Rows.Add(d)

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Firmato(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Firmato()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Flag_Firmato_Digit", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Firmato", GetType(String)))

        Try

            Dim d = dt.NewRow
            d("Flag_Firmato_Digit") = 0
            d("Firmato") = "No"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Flag_Firmato_Digit") = 1
            d("Firmato") = "Si"
            dt.Rows.Add(d)

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Dropdown_Obbligatorio(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreScadenziario_BIZ.SchemaDocumenti.Dropdown_Obbligatorio()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Flag_Obbligatorio", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Obbligatorio", GetType(String)))

        Try

            Dim d = dt.NewRow
            d("Flag_Obbligatorio") = 0
            d("Obbligatorio") = "No"
            dt.Rows.Add(d)

            d = dt.NewRow
            d("Flag_Obbligatorio") = 1
            d("Obbligatorio") = "Si"
            dt.Rows.Add(d)

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function

    Public Shared Sub ControlliValidita_SchemaDocumenti(ByVal righeInserite As List(Of SchemaDocumentiDto),
                                                       ByVal righeModificate As List(Of SchemaDocumentiDto),
                                                       ByVal objParametri As AgronicaCoreParametri,
                                                       ByVal efConnString As String,
                                                       ByRef Errori As String)

        Dim agronicaDAL As New SchemaDocumenti_R

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
                    Errori = "L'elemento existe già in database: Id Schema Documenti='" & l.Id_Schema_Template & "'"
                    Return
                End If
            Next
        End If
    End Sub

    Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As SchemaDocumentiDto)

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

        'If r.datainvio Is Nothing Then
        '    r.datainvio = New Date(1900, 1, 1)
        'End If

        If r.Validita_Inizio Is Nothing Then
            r.Validita_Inizio = New Date(1900, 1, 1)
        End If

        If r.Validita_Fine Is Nothing Then
            r.Validita_Fine = New Date(2100, 12, 31)
        End If

        If r.Flag_Firmato_Digit = Nothing Then
            r.Flag_Firmato_Digit = 0
        End If

        If r.Nr_Documenti = Nothing Then
            r.Nr_Documenti = 0
        End If

    End Sub
    Shared Sub VerificaValiditaRigaInserita(r As SchemaDocumentiDto,
                                    i As Integer,
                                    ByRef errori As String)
        Dim campiInvalidi = New List(Of String)


        If r.Id_Schema_Template < 0 Then
            campiInvalidi.Add("'Id Schema Documenti'")
        End If
        If r.Ambito < -15 Or r.Ambito > -14 Then
            campiInvalidi.Add("'Ambito'")
        End If
        If r.Fase < -21 Or r.Fase > -16 Then
            campiInvalidi.Add("'Fase'")
        End If
        'If r.Tipologia = Nothing Then
        '    campiInvalidi.Add("'Tipologia'")
        'End If
        'If r.Flag_Obbligatorio = Nothing Then
        '    campiInvalidi.Add("'Obbligatorio'")
        'End If

        If campiInvalidi.Count = 1 Then
            errori += "L'elemento con indice " & i + 1 & ". Il campo " & campiInvalidi(0) & " è obbligatorio."
        ElseIf campiInvalidi.Count > 1 Then
            errori += "L'elemento con indice " & i + 1 & ". I campi: " & String.Join(", ", campiInvalidi) & " sono obbligatori."
        End If
    End Sub

End Class

