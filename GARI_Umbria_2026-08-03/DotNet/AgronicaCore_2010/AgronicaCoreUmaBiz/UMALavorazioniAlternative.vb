Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R
Imports AgronicaCoreUmaDal.UMALavorazioniAlternative_W
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class UMALavorazioniAlternative
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LavorazionAlternative_LeggiTabella(ByRef objParametri As AgronicaCoreParametri, InizioValidita As String, FineValidita As String) As DataTable
        Dim AgronicaDAL As New UMALAvorazioniAlternative_R
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMALavorazioniAlternative.LeggiLavorazioniAlternative()"
        Try
            Dim lavAlternative As DataTable = AgronicaDAL.LeggiLavorazioniAlternative(objParametri, InizioValidita, FineValidita)

            Dim Dt As New DataTable
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dt.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("LavUMA_Lav_UMA_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("LavUMAAlt_Lav_UMA_Des", GetType(String)))

            Dt.Columns.Add(New DataColumn("Gruppo_Colturale_UMA", GetType(String)))
            Dt.Columns.Add(New DataColumn("Lavorazione_UMA", GetType(String)))
            Dt.Columns.Add(New DataColumn("Lavorazione_UMA_Alt", GetType(String)))

            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Inviato", GetType(Short)))
            Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Creazione", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Data_Modifica", GetType(DateTime)))
            Dt.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))
            Dt.Columns.Add(New DataColumn("Modificabile", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Regolamento_CodDes", GetType(String)))


            For Each elem As DataRow In lavAlternative.Rows
                Dim d = Dt.NewRow

                d("Macrouso_UMA_Des") = elem("Macrouso_UMA_Des")
                d("LavUMA_Lav_UMA_Des") = elem("LavUMA_Lav_UMA_Des")
                d("LavUMAAlt_Lav_UMA_Des") = elem("LavUMAAlt_Lav_UMA_Des")

                d("Gruppo_Colturale_UMA") = elem("Gruppo_Colturale_UMA")
                d("Lavorazione_UMA") = elem("Lavorazione_UMA")
                d("Lavorazione_UMA_Alt") = elem("Lavorazione_UMA_Alt")

                d("Validita_Inizio") = elem("Validita_Inizio")
                d("Validita_Fine") = elem("Validita_Fine")
                d("Inviato") = elem("Inviato")
                d("DataInvio") = elem("DataInvio")
                d("Data_Creazione") = elem("Data_Creazione")
                d("Data_Modifica") = elem("Data_Modifica")
                d("Username_Creazione") = elem("Username_Creazione")
                d("Username_Modifica") = elem("Username_Modifica")
                d("Regolamento_Cod") = elem("Regolamento_Cod")
                Dim tipoRegolamento_Cod As Integer = elem("Regolamento_Cod")
                Dim Regolamento_CodDes As String
                If tipoRegolamento_Cod = 1 Then
                    Regolamento_CodDes = "Convenzionale"
                ElseIf tipoRegolamento_Cod = 4 Then
                    Regolamento_CodDes = "Biologico"
                Else
                    Regolamento_CodDes = "Entrambi"
                End If
                d("Regolamento_CodDes") = Regolamento_CodDes

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

    Public Function LavorazioniAlternative_SalvaGriglia(ByRef objParametri As AgronicaCoreParametri,
                                                       righeInseriteJson As String,
                                                       righeModificateJson As String,
                                                       righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMALavorazioniAlternative.LavorazioniAlternative_SalvaGriglia()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String = ""
        Dim efConnString As String
        Dim righeCancellate As List(Of UMALavorazioniAlteranativeDto)
        Dim righeModificate As List(Of UMALavorazioniAlteranativeDto)
        Dim righeInserite As List(Of UMALavorazioniAlteranativeDto)
        Dim Errori As String = String.Empty


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard

        ' Due istruzioni try/catch per prenedre nella transazione soltanto le righe rilevanti
        Try
            '------------------------------ Deserializza le righe di input ------------------------------
            Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            righeCancellate = JsonConvert.DeserializeObject(Of List(Of UMALavorazioniAlteranativeDto))(righeCancellateJson, deserializerSettings)
            righeModificate = JsonConvert.DeserializeObject(Of List(Of UMALavorazioniAlteranativeDto))(righeModificateJson, deserializerSettings)
            righeInserite = JsonConvert.DeserializeObject(Of List(Of UMALavorazioniAlteranativeDto))(righeInseriteJson, deserializerSettings)

            '------------------------------- Carica Entity Framework ----------------------------------------
            Dim gefutils As New Gias_EF_Utility
            efConnString = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            '------------------------------ Verifica validità righe input ------------------------------    
            Errori = String.Empty
            If righeInserite.Count() > 0 OrElse righeModificate.Count() > 0 Then
                ControlliValidita_UMALavorazioniAlternative(righeInserite, righeModificate, objParametri, efConnString, Errori)
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
                Dim AgronicaDAL As New AgronicaCoreUmaDal.UMALavorazioniAlternative_W

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
    Public Function Dropdown_LavUMA_Lav_UMA_Des(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreAnagrafeBIZ.UMALavorazioniAlternative.Dropdown_LavUMA_Lav_UMA_Des()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("LavUMA_Lav_UMA_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("Lavorazione_UMA", GetType(String)))

        Dim biz As New AgronicaCoreUmaDal.UMALAvorazioniAlternative_R

        Dim elenco_macrouso = biz.Dropdown_LavUMA_Lav_UMA_Des(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("Lavorazione_UMA") = row.Code
            d("LavUMA_Lav_UMA_Des") = row.Descrizione
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
    Public Function Dropdown_LavUMAAlt_Lav_UMA_Des(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreAnagrafeBIZ.UMALavorazioniAlternative.Dropdown_LavUMAAlt_Lav_UMA_Des()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Lavorazione_UMA_Alt", GetType(String)))
        dt.Columns.Add(New DataColumn("LavUMAAlt_Lav_UMA_Des", GetType(String)))

        Dim biz As New AgronicaCoreUmaDal.UMALAvorazioniAlternative_R

        Dim elenco_macrouso = biz.Dropdown_LavUMA_Lav_UMA_Des(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("Lavorazione_UMA_Alt") = row.Code
            d("LavUMAAlt_Lav_UMA_Des") = row.Descrizione
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

    Public Function Dropdown_Gruppo_Colturale_UMA(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "AgronicaCoreAnagrafeBIZ.UMALavorazioniAlternative.Dropdown_Gruppo_Colturale_UMA()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Gruppo_Colturale_UMA", GetType(String)))
        dt.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))

        Dim biz As New AgronicaCoreUmaDal.UMALAvorazioniAlternative_R

        Dim elenco_macrouso = biz.Dropdown_Gruppo_Colturale_UMA(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("Gruppo_Colturale_UMA") = row.Code
            d("Macrouso_UMA_Des") = row.Descrizione
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

    Public Shared Sub ControlliValidita_UMALavorazioniAlternative(ByVal righeInserite As List(Of UMALavorazioniAlteranativeDto),
                                                       ByVal righeModificate As List(Of UMALavorazioniAlteranativeDto),
                                                       ByVal objParametri As AgronicaCoreParametri,
                                                       ByVal efConnString As String,
                                                       ByRef Errori As String)

        Dim agronicaDAL As New UMALAvorazioniAlternative_R

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
                Dim exists As Boolean = False

                If l.Regolamento_Cod = 0 Then
                    Dim Lavo = From lst In righeInserite
                               Where lst.Macrouso_UMA_Des = l.Macrouso_UMA_Des AndAlso lst.LavUMA_Lav_UMA_Des = l.LavUMA_Lav_UMA_Des AndAlso lst.LavUMAAlt_Lav_UMA_Des = l.LavUMAAlt_Lav_UMA_Des AndAlso (lst.Regolamento_Cod = 0 OrElse lst.Regolamento_Cod = 1 OrElse lst.Regolamento_Cod = 4)
                    If Lavo.Count > 1 Then
                        exists = True
                    End If
                Else
                    Dim Lavo = From lst In righeInserite
                               Where lst.Macrouso_UMA_Des = l.Macrouso_UMA_Des AndAlso lst.LavUMA_Lav_UMA_Des = l.LavUMA_Lav_UMA_Des AndAlso lst.LavUMAAlt_Lav_UMA_Des = l.LavUMAAlt_Lav_UMA_Des AndAlso (lst.Regolamento_Cod = 0 OrElse lst.Regolamento_Cod = l.Regolamento_Cod)
                    If Lavo.Count > 1 Then
                        exists = True
                    End If
                End If

                If exists = False Then
                    exists = agronicaDAL.VerificaElementoNonEsisteInDB(objParametri, l)
                End If

                If exists Then
                    Dim tipoRegolamento_Cod As Integer = l.Regolamento_Cod
                    Dim Regolamento_CodDes As String
                    If tipoRegolamento_Cod = 1 Then
                        Regolamento_CodDes = "CONVENZIONALE"
                    ElseIf tipoRegolamento_Cod = 4 Then
                        Regolamento_CodDes = "BIOLOGICO"
                    Else
                        Regolamento_CodDes = "ENTRAMBI"
                    End If
                    Errori = "Lavorazione alternativa già esistente o in sovrapposizione con il regolamento: Macrouso UMA Descrizione='" & l.Macrouso_UMA_Des & "', Regolamento='" & Regolamento_CodDes & "', Lavorazione UMA Descrizione='" & l.LavUMA_Lav_UMA_Des & "',  Lav. Alternativa UMA Descrizione='" & l.LavUMAAlt_Lav_UMA_Des & "'"
                    Return
                End If
            Next
        End If
    End Sub
    Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As UMALavorazioniAlteranativeDto)

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
    Shared Sub VerificaValiditaRigaInserita(r As UMALavorazioniAlteranativeDto,
                                    i As Integer,
                                    ByRef errori As String)
        Dim campiInvalidi = New List(Of String)


        If r.Gruppo_Colturale_UMA Is Nothing OrElse r.Gruppo_Colturale_UMA = "" Then
            campiInvalidi.Add("'Gruppo Colturale UMA'")
        End If

        If r.Lavorazione_UMA Is Nothing OrElse r.Lavorazione_UMA = "" Then
            campiInvalidi.Add("'Lavorazione UMA'")
        End If

        If r.Lavorazione_UMA_Alt Is Nothing OrElse r.Lavorazione_UMA_Alt = "" Then
            campiInvalidi.Add("'Lavorazione UMA Alt'")
        End If

        If campiInvalidi.Count = 1 Then
            errori += "L'elemento con indice " & i + 1 & ". Il campo " & campiInvalidi(0) & " è obbligatorio."
        ElseIf campiInvalidi.Count > 1 Then
            errori += "L'elemento con indice " & i + 1 & ". I campi: " & String.Join(", ", campiInvalidi) & " sono obbligatori."
        End If
    End Sub
End Class
