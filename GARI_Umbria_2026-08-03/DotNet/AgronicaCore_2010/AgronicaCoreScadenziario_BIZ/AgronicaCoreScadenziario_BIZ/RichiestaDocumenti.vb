Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreScadenziario
Imports System.Transactions
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreAnagrafeDAL.RichiestaDocumenti_W

Public Class RichiestaDocumenti
    Inherits AgronicaCoreDataProvider.DataProvider
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Public Function RichiestaDocumenti_LeggiTabella(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal Richiesta_Cod As Integer, ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New RichiestaDocumenti_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.SchemaDocumenti.RichiestaDocumenti_LeggiTabella()"
        Try
            Dim richiestaDocumenti As DataTable = AgronicaDAL.LeggiRichiestaDocumenti(objParametri, piva, Richiesta_Cod, objParametri_Utenti)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Return richiestaDocumenti
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Public Function RichiestaDocumenti_EstraiFase(ByRef objParametri As AgronicaCoreParametri, ByVal piva As String, ByVal Richiesta_Cod As Integer, ByRef objParametri_Utenti As AgronicaCoreParametri) As Integer
        Dim AgronicaDAL As New RichiestaDocumenti_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.SchemaDocumenti.RichiestaDocumenti_EstraiFase()"
        Try

            Dim DTEstrai As DataTable = AgronicaDAL.EstraiDati(objParametri, piva, Richiesta_Cod)

            Dim praticaCod As Integer = 0
            Dim avanzamento As Integer = 0
            Dim tipoRichiesta As Integer = 0
            Dim annoQuery As Integer = 0
            Dim servizioCod As Integer = 0
            Dim statoAttuale As Integer = 0
            Dim statoAttualeDes As String = ""

            If Not IsNothing(DTEstrai) AndAlso DTEstrai.Rows.Count > 0 Then

                praticaCod = If(Not IsDBNull(DTEstrai.Rows(0)("Pratica_Cod")), CInt(DTEstrai.Rows(0)("Pratica_Cod")), -1)
                avanzamento = If(Not IsDBNull(DTEstrai.Rows(0)("Avanzamento_Richiesta")), CInt(DTEstrai.Rows(0)("Avanzamento_Richiesta")), -1)
                tipoRichiesta = If(Not IsDBNull(DTEstrai.Rows(0)("Tipo_Richiesta")), CInt(DTEstrai.Rows(0)("Tipo_Richiesta")), -1)
                annoQuery = If(Not IsDBNull(DTEstrai.Rows(0)("anno")), CInt(DTEstrai.Rows(0)("anno")), 0)
                servizioCod = If(Not IsDBNull(DTEstrai.Rows(0)("Servizio_Cod")), CInt(DTEstrai.Rows(0)("Servizio_Cod")), 0)
                statoAttuale = If(Not IsDBNull(DTEstrai.Rows(0)("Stato_Cod")), CInt(DTEstrai.Rows(0)("Stato_Cod")), 0)
                statoAttualeDes = If(Not IsDBNull(DTEstrai.Rows(0)("WAnagraficaStati_Des")), DTEstrai.Rows(0)("WAnagraficaStati_Des"), "")

            End If

            Dim fase As Integer = AgronicaDAL.EstraiFase(objParametri, piva, avanzamento, tipoRichiesta, annoQuery, Richiesta_Cod, praticaCod, statoAttuale, objParametri_Utenti)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Return fase
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function


    'Public Function RichiestaDocumenti_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
    '                                                   righeInseriteJson As String,
    '                                                   righeModificateJson As String,
    '                                                   righeCancellateJson As String) As RispostaStandard

    '    Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.RichiestaDocumenti.RichiestaDocumenti_SalvaGriglia()"

    '    '-------------------- Dichiarazioni di variabili ----------------------
    '    Dim MessaggioErrore As String = ""
    '    Dim efConnString As String
    '    Dim righeCancellate As List(Of RichiestaDocumentiDto)
    '    Dim righeModificate As List(Of RichiestaDocumentiDto)
    '    Dim righeInserite As List(Of RichiestaDocumentiDto)
    '    Dim Errori As String = String.Empty


    '    Dim FlagTransazioneLocale As Boolean = False
    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim risposta As New RispostaStandard

    '    ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
    '    Try
    '        '------------------------------ Deserializza le righe di input ------------------------------
    '        Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
    '        righeCancellate = JsonConvert.DeserializeObject(Of List(Of RichiestaDocumentiDto))(righeCancellateJson, deserializerSettings)
    '        righeModificate = JsonConvert.DeserializeObject(Of List(Of RichiestaDocumentiDto))(righeModificateJson, deserializerSettings)
    '        righeInserite = JsonConvert.DeserializeObject(Of List(Of RichiestaDocumentiDto))(righeInseriteJson, deserializerSettings)

    '        '------------------------------- Carica Entity Framework ----------------------------------------
    '        Dim gefutils As New Gias_EF_Utility
    '        efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

    '        '------------------------------ Verifica validità righe input ------------------------------    
    '        Errori = String.Empty
    '        If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
    '            ControlliValidita_RichiestaDocumenti(righeInserite, righeModificate, objParametri, efConnString, Errori)
    '        End If

    '        If (Not String.IsNullOrEmpty(Errori)) Then
    '            risposta.RispostaOK = False
    '            risposta.Errore = Errori
    '            Return risposta
    '        End If
    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Using scope As New TransactionScope()
    '        Try
    '            ' ------------------------------------- Salva i dati nel database -------------------------------------
    '            Dim context As New Gias_DeveloperServer_Entities(efConnString)
    '            Dim AgronicaDAL As New RichiestaDocumenti_W

    '            If String.IsNullOrEmpty(Errori) Then
    '                '------------------------- Salva le nuove righe inserite -------------------------
    '                If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
    '                    AgronicaDAL.AggiungiNouvi(righeInserite, context, objParametri)
    '                End If

    '                '------------------------- Inserici le righe modificate -------------------------
    '                If Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
    '                    AgronicaDAL.Aggiorna(righeModificate, context, objParametri)
    '                End If

    '                '------------------------- Rimuovi le righe cancellate -------------------------
    '                If Not righeCancellate Is Nothing AndAlso righeCancellate.Count > 0 Then
    '                    AgronicaDAL.Rimuovi(righeCancellate, context, objParametri)
    '                End If
    '            End If
    '            scope.Complete()
    '            scope.Dispose()

    '            risposta.RispostaOK = True
    '            risposta.RispostaStringa = "L'operazione è stata completata con successo."

    '            Return risposta
    '        Catch ex As Exception
    '            scope.Dispose()

    '            MessaggioErrore = ex.Message
    '            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
    '            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
    '        End Try
    '    End Using
    'End Function

    Public Function AgronicaCoreDataProvider_Leggi(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New RichiestaDocumenti_R
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




    'Public Shared Sub ControlliValidita_RichiestaDocumenti(ByVal righeInserite As List(Of RichiestaDocumentiDto),
    '                                                   ByVal righeModificate As List(Of RichiestaDocumentiDto),
    '                                                   ByVal objParametri As AgronicaCoreParametri,
    '                                                   ByVal efConnString As String,
    '                                                   ByRef Errori As String)

    '    Dim agronicaDAL As New RichiestaDocumenti_R

    '    ' --------- Controlla se le righe inserite/modificate contengono degli errori --------------
    '    ' ------------------------------------------------------------------------------------------
    '    If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
    '        For i = 0 To righeInserite.Count - 1
    '            VerificaValiditaRigaInserita(righeInserite(i), i, Errori)
    '            If (String.IsNullOrEmpty(Errori)) Then
    '                ImpostaValoriDiDefaultSeNecessario(True, objParametri.UtenteUsername, righeInserite(i))
    '            End If
    '        Next
    '    End If

    '    If String.IsNullOrEmpty(Errori) AndAlso Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
    '        For i = 0 To righeModificate.Count - 1
    '            VerificaValiditaRigaInserita(righeModificate(i), i, Errori)
    '            If (String.IsNullOrEmpty(Errori)) Then
    '                ImpostaValoriDiDefaultSeNecessario(False, objParametri.UtenteUsername, righeModificate(i))
    '            Else
    '                Exit For
    '            End If
    '        Next
    '    End If

    '    ' --------------------------- La chiave primaria non dovrebbe esistere già in db ---------------------------
    '    If String.IsNullOrEmpty(Errori) AndAlso Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
    '        For Each l In righeInserite
    '            Dim exists = agronicaDAL.VerificaElementoNonEsisteInDB(l, efConnString)

    '            'If exists Then
    '            '    Errori = "L'elemento existe già in database: Id Schema Documenti='" & l.Id_Schema_Template & "'"
    '            '    Return
    '            'End If
    '        Next
    '    End If
    'End Sub

    'Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As RichiestaDocumentiDto)

    '    ' ******************************** Nuove righe ********************************
    '    ' Tutti i campi che compongono la chiave primaria sono obbligatori
    '    ' Tutti gli altri campi non devono contenere valori di null.
    '    Dim now As Date = Date.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")

    '    'If isNew Then
    '    '    r.Data_Creazione = now
    '    '    r.Username_Creazione = usernameModifica
    '    'End If
    '    'r.Data_Modifica = now
    '    'r.Username_Modifica = usernameModifica

    '    ''If r.datainvio Is Nothing Then
    '    ''    r.datainvio = New Date(1900, 1, 1)
    '    ''End If

    '    'If r.Validita_Inizio Is Nothing Then
    '    '    r.Validita_Inizio = New Date(1900, 1, 1)
    '    'End If

    '    'If r.Validita_Fine Is Nothing Then
    '    '    r.Validita_Fine = New Date(2100, 12, 31)
    '    'End If

    '    'If r.Flag_Firmato_Digit = Nothing Then
    '    '    r.Flag_Firmato_Digit = 0
    '    'End If

    '    'If r.Flag_Obbligatorio = Nothing Then
    '    '    r.Flag_Obbligatorio = 0
    '    'End If

    '    'If r.Nr_Documenti = Nothing Then
    '    '    r.Nr_Documenti = 0
    '    'End If

    'End Sub
    'Shared Sub VerificaValiditaRigaInserita(r As RichiestaDocumentiDto,
    '                                i As Integer,
    '                                ByRef errori As String)
    '    Dim campiInvalidi = New List(Of String)


    '    If r.Id_Schema_Template < 0 Then
    '        campiInvalidi.Add("'Id Schema Documenti'")
    '    End If
    '    If r.Richiesta_Cod < 0 Then
    '        campiInvalidi.Add("'Richiesta Cod'")
    '    End If
    '    If r.ID_Alert_Entita < 0 Then
    '        campiInvalidi.Add("'ID Alert Entita'")
    '    End If

    '    If campiInvalidi.Count = 1 Then
    '        errori += "L'elemento con indice " & i + 1 & ". Il campo " & campiInvalidi(0) & " è obbligatorio."
    '    ElseIf campiInvalidi.Count > 1 Then
    '        errori += "L'elemento con indice " & i + 1 & ". I campi: " & String.Join(", ", campiInvalidi) & " sono obbligatori."
    '    End If
    'End Sub

End Class
