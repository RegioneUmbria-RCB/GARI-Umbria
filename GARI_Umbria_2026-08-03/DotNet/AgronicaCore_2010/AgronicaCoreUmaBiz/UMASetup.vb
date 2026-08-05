Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreScadenziario
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUmaDal.UMASetup_W
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json



Public Class UMASetup
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Setup_LeggiTabella(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMASetup_R
        Dim alertTipologie As New Alert_Tipologia_R
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
            Dt.Columns.Add(New DataColumn("Percentuale_Richieste_Anticipo", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Gestione_Biologico", GetType(Short)))
            Dt.Columns.Add(New DataColumn("Gestione_Rimanenze", GetType(Short)))
            Dt.Columns.Add(New DataColumn("Gestione_Anticipazioni_Colturali", GetType(Short)))
            Dt.Columns.Add(New DataColumn("Vincola_Rendicontazione_e_Richiesta", GetType(String)))
            Dt.Columns.Add(New DataColumn("Gestione_BiologicoDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Gestione_RimanenzeDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Gestione_Anticipazioni_ColturaliDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macchine_Targa_ObbligatoriaString", GetType(String)))
            Dt.Columns.Add(New DataColumn("Tipologia_Report_Elas", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Tipologia_Report_ElasDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Tipologia_Elenco_Inadempienti", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Tipologia_Elenco_InadempientiDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Tipologia_Report_SegnalazioneAccise", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Tipologia_Report_SegnalazioneAcciseDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("Stati_Invio_MailString", GetType(String)))
            Dt.Columns.Add(New DataColumn("Gruppi_Utenti_Invio_MailString", GetType(String)))

            Dim dtTipologie = alertTipologie.Leggi(enum_ID_Area_Alert.UMA_Carburanti, 0, "", False, objParametri)

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
                d("Percentuale_Richieste_Anticipo") = elem("Percentuale_Richieste_Anticipo")
                d("Gestione_Biologico") = elem("Gestione_Biologico")
                d("Gestione_Rimanenze") = elem("Gestione_Rimanenze")
                d("Gestione_Anticipazioni_Colturali") = elem("Gestione_Anticipazioni_Colturali")
                d("Vincola_Rendicontazione_e_Richiesta") = elem("Vincola_Rendicontazione_e_Richiesta")
                d("Gestione_BiologicoDes") = elem("Gestione_BiologicoDes")
                d("Gestione_RimanenzeDes") = elem("Gestione_RimanenzeDes")
                Dim Gestione_Anticipazioni_Colturali As Integer = elem("Gestione_Anticipazioni_Colturali")
                Dim Gestione_Anticipazioni_ColturaliDes As String = ""
                Select Case Gestione_Anticipazioni_Colturali
                    Case 0
                        Gestione_Anticipazioni_ColturaliDes = "Tutte"
                    Case enum_TipoAzienda_UMA.Azienda_Agricola_Privata
                        Gestione_Anticipazioni_ColturaliDes = "Azienda Agricola Privata"
                    Case enum_TipoAzienda_UMA.Azienda_Terzista
                        Gestione_Anticipazioni_ColturaliDes = "Azienda Terzista"
                    Case enum_TipoAzienda_UMA.Cooperativa_Agricola
                        Gestione_Anticipazioni_ColturaliDes = "Cooperativa Agricola"
                    Case enum_TipoAzienda_UMA.Azienda_Agricola_Istituzioni_Pubbliche
                        Gestione_Anticipazioni_ColturaliDes = "Azienda Agricola Pubblica"
                    Case enum_TipoAzienda_UMA.Consorzio_Bonifica_Irrigazione
                        Gestione_Anticipazioni_ColturaliDes = "Consorzio di Bonifica e Irrigazione"
                    Case enum_TipoAzienda_UMA.Consorzio_Bonifica_Irrigazione
                        Gestione_Anticipazioni_ColturaliDes = "Consorzio di Bonifica e Irrigazione"
                End Select
                d("Gestione_Anticipazioni_ColturaliDes") = Gestione_Anticipazioni_ColturaliDes
                d("Macchine_Targa_ObbligatoriaString") = CStr(elem("Macchine_Targa_Obbligatoria"))
                'Tipologia Report ELAS
                Dim tipologiaReportElas = elem("Tipologia_Report_Elas")
                d("Tipologia_Report_Elas") = If(IsDBNull(tipologiaReportElas), 0, tipologiaReportElas)
                Dim tipologiaReportElasDes = "Nessuna"
                If d("Tipologia_Report_Elas") <> 0 Then
                    Dim dtTipologiaRiga = dtTipologie.Select("ID_Tipologia = " + tipologiaReportElas.ToString)
                    tipologiaReportElasDes = dtTipologiaRiga(0).Item("Nome")
                End If
                d("Tipologia_Report_ElasDes") = tipologiaReportElasDes
                'Tipologia elenco inadempienti
                Dim tipologiaElencoInadempienti = elem("Tipologia_Elenco_Inadempienti")
                d("Tipologia_Elenco_Inadempienti") = If(IsDBNull(tipologiaElencoInadempienti), 0, tipologiaElencoInadempienti)
                Dim tipologiaReportElencoInadempientiDes = "Nessuna"
                If d("Tipologia_Elenco_Inadempienti") <> 0 Then
                    Dim dtTipologiaRiga = dtTipologie.Select("ID_Tipologia = " + tipologiaElencoInadempienti.ToString)
                    tipologiaReportElencoInadempientiDes = dtTipologiaRiga(0).Item("Nome")
                End If
                d("Tipologia_Elenco_InadempientiDes") = tipologiaReportElencoInadempientiDes
                'Tipologia segnalazione accise
                Dim Tipologia_Report_SegnalazioneAccise = elem("Tipologia_Report_SegnalazioneAccise")
                d("Tipologia_Report_SegnalazioneAccise") = If(IsDBNull(Tipologia_Report_SegnalazioneAccise), 0, Tipologia_Report_SegnalazioneAccise)
                Dim Tipologia_Report_SegnalazioneAcciseDes = "Nessuna"
                If d("Tipologia_Report_SegnalazioneAccise") <> 0 Then
                    Dim dtTipologiaRiga = dtTipologie.Select("ID_Tipologia = " + Tipologia_Report_SegnalazioneAccise.ToString)
                    Tipologia_Report_SegnalazioneAcciseDes = dtTipologiaRiga(0).Item("Nome")
                End If
                d("Tipologia_Report_SegnalazioneAcciseDes") = Tipologia_Report_SegnalazioneAcciseDes
                d("Stati_Invio_MailString") = If(IsDBNull(elem("stati_invio_mail_avanz_pratica")), "", elem("stati_invio_mail_avanz_pratica"))
                d("Gruppi_Utenti_Invio_MailString") = If(IsDBNull(elem("gruppi_utenti_invio_mail_avanz_pratica")), "", elem("gruppi_utenti_invio_mail_avanz_pratica"))
                'Aggiunta riga
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
                Dim AgronicaDAL As New AgronicaCoreUmaDal.UMASetup_W

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
