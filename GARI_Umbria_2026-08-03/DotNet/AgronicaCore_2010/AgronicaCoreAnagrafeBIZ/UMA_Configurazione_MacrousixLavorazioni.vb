Imports System.Runtime.CompilerServices
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni
Imports AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_R
Imports AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_W
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class UMA_Configurazione_MacrousixLavorazioni_BIZ
    Inherits AgronicaCoreDataProvider.LogProvider


    Public Function AgronicaCoreDataProvider_Leggi(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim AgronicaDAL As New UMA_Configurazione_MacrousixLavorazioni_R
        Dim MessaggioErrore As String
        Dim result As DataTable
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim NomeRoutine As String = "DpiBIZ.CentroAziendale_R.AgronicaCoreDataProvider_Leggi()"
        Try
            result = AgronicaDAL.Leggi(objParametri)
        Catch ex As Exception
            result = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result

    End Function

    Public Function Salva_GrigliaLavorazioniUMA(ByRef objParametri As AgronicaCoreParametri,
                                                righeinseriteJson As String,
                                                righeModificateJson As String,
                                                righeCancellateJson As String) As RispostaStandard

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.UMA_Configurazione_MacrousixLavorazioni_BIZ.Salva_GrigliaLavorazioniUMA()"

        '-------------------- Dichiarazioni di variabili ----------------------
        Dim MessaggioErrore As String = ""

        Dim AgronicaDAL As New UMA_Configurazione_MacrousixLavorazioni_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim risposta As New RispostaStandard
        Using scope As New TransactionScope()

            Try
                '------------------------------- Carica Entity Framework ----------------------------------------
                Dim gefutils As New Gias_EF_Utility
                Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

                '------------------------------ Deserializza le righe di input ------------------------------
                Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim righeInseriteArr As List(Of LavorazioneUMA) = JsonConvert.DeserializeObject(Of List(Of LavorazioneUMA))(righeinseriteJson, deserializerSettings)
                Dim righeModificateArr As List(Of LavorazioneUMA) = JsonConvert.DeserializeObject(Of List(Of LavorazioneUMA))(righeModificateJson, deserializerSettings)
                Dim righeCancellateArr As List(Of LavorazioneUMA) = JsonConvert.DeserializeObject(Of List(Of LavorazioneUMA))(righeCancellateJson, deserializerSettings)

                '------------------------------ Verifica validità righe input ------------------------------    
                Dim Errori = String.Empty
                ControlliValidita_LavorazioniUMA(righeInseriteArr, righeModificateArr, righeCancellateArr, objParametri, efConnString, Errori)

                If (Not String.IsNullOrEmpty(Errori)) Then
                    risposta.RispostaOK = False
                    risposta.Errore = Errori
                    Return risposta
                End If

                ' ------------------------------------- Salva i dati nel database -------------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                Dim context As New Gias_DeveloperServer_Entities(efConnString)

                If String.IsNullOrEmpty(Errori) Then
                    '------------------------- Salva le nuove righe inserite -------------------------
                    If Not righeInseriteArr Is Nothing AndAlso righeInseriteArr.Count > 0 Then
                        AgronicaDAL.AggiungiNouvi(righeInseriteArr, context, objParametri)
                    End If

                    '------------------------- Inserici le righe modificate -------------------------
                    If Not righeModificateArr Is Nothing AndAlso righeModificateArr.Count > 0 Then
                        AgronicaDAL.Aggiorna(righeModificateArr, context, objParametri)
                    End If

                    '------------------------- Rimuovi le righe cancellate -------------------------
                    If Not righeCancellateArr Is Nothing AndAlso righeCancellateArr.Count > 0 Then
                        For Each riga As LavorazioneUMA In righeCancellateArr
                            AgronicaDAL.Rimuovi(riga, context, objParametri)
                        Next
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
            Finally
                scope.Dispose()
            End Try
        End Using
    End Function

    Public Shared Sub ControlliValidita_LavorazioniUMA(ByVal righeInserite As List(Of LavorazioneUMA),
                                                       ByVal righeModificate As List(Of LavorazioneUMA),
                                                       ByVal righeCancellate As List(Of LavorazioneUMA),
                                                       ByVal objParametri As AgronicaCoreParametri,
                                                       ByVal efConnString As String,
                                                       ByRef Errori As String)

        Dim agronicaDAL As New UMA_Configurazione_MacrousixLavorazioni_R

        ' ------------------------------------------------------------------------------------------
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
                    Errori = "L'elemento existe già in database: Regione_Cod=" & l.RegioneCod & ", Macrousi UMA Descrizione=" & l.UMAMacrousi_MacrousoUMADes & ",  Lavorazioni UMA Descrizione=" & l.UMALavorazioni_LavUmaDes & ", Operazioni Lav. Descrizione=" & l.Operazioni_LavDeS & ", Attivita Descrizione=" & l.Attivita_Desc
                    Return
                End If
            Next
        End If
    End Sub

    Shared Sub ImpostaValoriDiDefaultSeNecessario(isNew As Boolean, usernameModifica As String, ByRef r As LavorazioneUMA)

        ' ******************************** Nuove righe ********************************
        ' Tutti i campi che compongono la chiave primaria sono obbligatori
        ' Tutti gli altri campi non devono contenere valori di null.
        Dim now As Date = Date.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")

        If isNew Then
            r.RegioneCod = "010"
            r.DataCreazione = now
            r.UsernameCreazione = usernameModifica
        End If
        r.DataModifica = now
        r.UsernameModifica = usernameModifica

        If r.Default Is Nothing Then
            r.Default = 0
        End If

        If r.DataInvio Is Nothing Then
            r.DataInvio = New Date(1900, 1, 1)
        End If

        If r.ValiditaInizio Is Nothing Then
            r.ValiditaInizio = New Date(1900, 1, 1)
        End If

        If r.ValiditaFine Is Nothing Then
            r.ValiditaFine = New Date(2100, 12, 31)
        End If

        If r.UDMAlternativa Is Nothing Then
            r.UDMAlternativa = String.Empty
        End If
    End Sub

    ''' <summary>
    '''  
    ''' Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica non sono modificabili
    ''' </summary>
    Shared Sub VerificaValiditaRigaInserita(r As LavorazioneUMA,
                                    i As Integer,
                                    ByRef errori As String)
        Dim campiInvalidi = New List(Of String)


        If r.MacrousoUMACod Is Nothing OrElse r.MacrousoUMACod = "" Then
            campiInvalidi.Add("Macrouso UMA Des.")
        End If

        If r.LavUMACod Is Nothing OrElse r.LavUMACod = "" Then
            campiInvalidi.Add("Lav. UMA Des.")
        End If

        If r.LavCod = 0 Then
            campiInvalidi.Add("Operazioni Lav. Cod.")
        End If

        If campiInvalidi.Count > 0 Then
            errori += "L'elemento con indice " & i + 1 & ". I campi: " + String.Join(", ", campiInvalidi) & " sono obbligatori."
        End If
    End Sub

    Private Class Lavorazione_Model
        Public Operazioni_LavDeS As String
        Public UMAMacrousi_MacrousoUMADes As String
        Public UMALavorazioni_LavUmaDes As String
        Public Attivita_Desc As String

        Public RegioneCod As String
        Public MacrousoUMACod As String
        Public LavUMACod As String
        Public LavCod As Integer
        Public IdAttivita As Integer
        Public TipoOperazione As Tipo_Operazione
        Public GasolioLt As Double
        Public BenzinaLt As Double
        Public Ordinamento As Integer
        Public NMaxOperazioni As Integer
        Public [Default] As Integer
        Public ValiditaInizio As DateTime
        Public ValiditaFine As DateTime
        Public UDMAlternativa As String
        Public GasolinoLTxBiologico As Double
        Public BensinaLTxBiologico As Double
    End Class

    ''' Logica per mostrare l'elenco Macrousi Uma.
    ''' </summary>
    ''' <param name="objParametri_Server">Database object</param>
    ''' <returns>Dt_Schede</returns>
    Public Function LeggiElenco_UMA_Macrousi(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "UMA_Configurazione_MacrousixLavorazioni_BIZ.LeggiElenco_UMA_Macrousi()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))

        Dim biz As New AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_R

        Dim elenco_macrouso = biz.LeggiDropdown_UMA_Macrousi(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("Macrouso_UMA_Cod") = row.Code
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

    ''' <summary>
    ''' Logica per mostrare l'elenco UMA Lavorazioni.
    ''' </summary>
    ''' <param name="objParametri_Server">Database object</param>
    ''' <returns>Dt_Schede</returns>
    Public Function LeggiElenco_UMA_Lavorazioni(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "UMA_Configurazione_MacrousixLavorazioni_BIZ.LeggiElenco_UMA_Lavorazioni()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("UMA_Lavorazioni_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("UMA_Lavorazioni_Des", GetType(String)))

        Dim biz As New AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_R

        Dim elenco_macrouso = biz.LeggiDropdown_UMALavorazioni(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("UMA_Lavorazioni_Cod") = row.Code
            d("UMA_Lavorazioni_Des") = row.Descrizione
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
    ''' <summary>
    ''' Logica per mostrare l'elenco dalla tabella Operazioni.
    ''' </summary>
    ''' <param name="objParametri_Server">Database object</param>
    ''' <returns>Dt_Schede</returns>
    Public Function LeggiElenco_Operazioni(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "UMA_Configurazione_MacrousixLavorazioni_BIZ.LeggiElenco_Operazioni()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Operazioni_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Operazioni_Des", GetType(String)))

        Dim biz As New AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_R

        Dim elenco_macrouso = biz.LeggiDropdown_Operazioni(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("Operazioni_Cod") = row.Code
            d("Operazioni_Des") = row.Descrizione
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

    ''' <summary>
    ''' Logica per mostrare l'elenco della tabella Attività.
    ''' </summary>
    ''' <param name="objParametri_Server">Database object</param>
    ''' <returns>Dt_Schede</returns>
    Public Function LeggiElenco_Attivita(objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine = "UMA_Configurazione_MacrousixLavorazioni_BIZ.LeggiElenco_Attivita()"

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Attivita_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))

        Dim biz As New AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_R

        Dim elenco_macrouso = biz.LeggiDropdown_Attivita(objParametri_Server)
        For Each row As VoceElencoDiDropdown In elenco_macrouso
            Dim d = dt.NewRow
            d("Attivita_Cod") = row.Code
            d("Attivita_Des") = row.Descrizione
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
    Enum Tipo_Operazione
        Ordinaria = 1
        Straordinaria = 2
    End Enum

End Class
Module Extensions
    '<Extension()>
    'Function ToLavorazioniAlternative(ByVal dt As DataTable) As List(Of UMA_Configurazione_MacrousixLavorazioni_W.UMALavorazioniAlterantive)
    '    Dim result As New List(Of UMA_Configurazione_MacrousixLavorazioni_W.UMALavorazioniAlterantive)
    '    For Each row As DataRow In dt.Rows
    '        Dim elem As UMA_Configurazione_MacrousixLavorazioni_W.UMALavorazioniAlterantive = New UMA_Configurazione_MacrousixLavorazioni_W.UMALavorazioniAlterantive With {
    '            .DataInvio = row.Item("DataInvio"),
    '            .Data_Creazione = row.Item("Data_Creazione"),
    '            .Data_Modifica = row.Item("Data_Modifica"),
    '            .Gruppo_Colturale_UMA = row.Item("Gruppo_Colturale_UMA"),
    '            .Inviato = row.Item("Inviato"),
    '            .Lavorazione_UMA = row.Item("Lavorazione_UMA"),
    '            .Lavorazione_UMA_Alt = row.Item("Lavorazione_UMA_Alt"),
    '            .Username_Creazione = row.Item("Username_Creazione"),
    '            .Username_Modifica = row.Item("Username_Modifica"),
    '            .Validita_Fine = row.Item("Validita_Fine"),
    '            .Validita_Inizio = row.Item("Validita_Inizio")
    '        }
    '        result.Add(elem)
    '    Next
    '    Return result
    'End Function
End Module
