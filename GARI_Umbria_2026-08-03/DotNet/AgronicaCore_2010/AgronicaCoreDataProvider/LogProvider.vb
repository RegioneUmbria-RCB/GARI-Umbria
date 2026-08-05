Imports System.Configuration
Imports System.IO
Imports System.Threading
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

<Serializable()>
Public Class DbLogClass
    Public Username As String
    Public Errore As String
    Public Data As DateTime
    Public db As String
    Public istanza As String
    Public routine As String
    Public occorrenze As Integer
    Public hash As String
End Class

Public Class LogProvider

    '##############################################################################################
    <Obsolete("1.Da utilizzare quella con objParametri", True)>
    Public Sub Scrivi_LOG(ByVal DirectoryLOG As String,
                          ByVal FileLOG As String,
                          ByVal IdentificatoreUtente As String,
                          ByVal NomeRoutine As String,
                          ByVal MessaggioErrore As String)

        'Dim DefaultDirectoryLOG As String = "C:\GIASLAN\Log"
        Dim DefaultDirectoryLOG As String = Path.GetTempPath()
        Dim DefaultFileLOG As String = "AgronicaCoreLOG.txt"

        Dim NomeCompletoFileLOG As String = ""
        Dim xStreamWriter As StreamWriter = Nothing
        Dim Testo As String = ""

        Try

            'Verifico se è stata indicata una directory di LOG
            If DirectoryLOG = "" Then
                DirectoryLOG = DefaultDirectoryLOG
            End If

            'Verifico se è stato indicato un file di LOG
            If FileLOG = "" Then
                FileLOG = DefaultFileLOG
            End If

            'Verifico se esiste la DIRECTORY di LOG indicata ... altrimenti la creo
            If Not Directory.Exists(DirectoryLOG) Then
                Directory.CreateDirectory(DirectoryLOG)
            End If

            If Not DirectoryLOG.EndsWith("\") Then
                DirectoryLOG &= "\"
            End If

            'Costruisco il nome completo del file di LOG
            ' NomeCompletoFileLOG = (DirectoryLOG & "\" & FileLOG).Replace("\\", "\")
            NomeCompletoFileLOG = (DirectoryLOG & FileLOG).Replace("\\", "\")

            ReplaceInvalidChars(FileLOG, NomeCompletoFileLOG)

            'Verifico se esiste il FILE di LOG indicato ... altrimenti lo creo
            If Not File.Exists(NomeCompletoFileLOG) Then

                'Creo il file di LOG nuovo
                xStreamWriter = File.CreateText(NomeCompletoFileLOG)
                xStreamWriter.WriteLine("Inizializzazione file di LOG ... " &
                                    Date.Now.ToShortDateString & " " &
                                    Date.Now.ToLongTimeString)
                xStreamWriter.Flush()
                xStreamWriter.Close()

            End If

            'Costruisco la stringa di testo da scrivere
            Testo = Date.Now.ToShortDateString &
                    " " &
                    Date.Now.ToLongTimeString &
                    " {" &
                    IdentificatoreUtente &
                    "} : [" &
                    NomeRoutine &
                    "] : " &
                    MessaggioErrore


            'Apro il file di LOG
            xStreamWriter = File.AppendText(NomeCompletoFileLOG)

            'Scrivo la stringa
            xStreamWriter.WriteLine(Testo)
            xStreamWriter.Flush()
            'xStreamWriter.Close()

        Catch ex As Exception
            ScriviLogEventViewer(ex, NomeRoutine, MessaggioErrore, Testo, "", NomeCompletoFileLOG)
        Finally

            If Not IsNothing(xStreamWriter) Then
                xStreamWriter.Close()
            End If

        End Try

    End Sub


    '##############################################################################################
    ''' <param name="objParametri"></param>
    ''' <param name="NomeRoutine"></param>
    ''' <param name="MessaggioErrore"></param>
    ''' <param name="verificaInviaElasticSearch">
    '''     Default a true, ma potrebbe essere chiamata dalle esegui_query, dove nei casi di errore del parametrizzatore non vogliamo mandare ad ES
    ''' </param>
    ''' <param name="CustomLOGParams">
    '''     Aggiunta per l'allineamento delle chiamate alla Scrivi_LOG, passando sempre per questa versione con l'objParametri.
    '''     Così siamo in grado di mantenere invariato il funzionamento ed accettare valori imposti dal chiamante come in precedenza, possibilmente diversi da quelli contenuti nella objParametri. 
    ''' </param> 
    Public Sub Scrivi_LOG(ByRef objParametri As AgronicaCoreParametri,
                          ByVal NomeRoutine As String,
                          ByVal MessaggioErrore As String,
                          Optional verificaInviaElasticSearch As Boolean = True,
                          Optional CustomLOGParams As CustomLOGParams = Nothing,
                          Optional ByVal cuaaLog As String = "")

        Dim DefaultDirectoryLOG As String = Path.GetTempPath()
        Dim DefaultFileLOG As String = "AgronicaCoreLOG.txt"

        Dim NomeCompletoFileLOG As String = ""
        Dim xStreamWriter As StreamWriter = Nothing
        Dim Testo As String = ""

        Dim CUAA As String = ""
        Dim timeStamp As Date = Date.Now()

        Dim LogDirectory As String = DefaultDirectoryLOG
        Dim LogFileName As String = DefaultFileLOG
        Dim LogDescrizioneUtente As String = ""

        Try
            'Vado a ricavare il CUAA dell'ultima azienda selezionata
            If objParametri IsNot Nothing AndAlso String.IsNullOrWhiteSpace(cuaaLog) Then
                CUAA = LeggiCUAA(objParametri)
            Else
                CUAA = cuaaLog
            End If

            If CustomLOGParams IsNot Nothing Then

                'Allineamento chiamate alla scrivi_log con objParametri
                'Per mantenere invariato il funzionamento aggiunto CustomLOGParams
                If Not String.IsNullOrEmpty(CustomLOGParams.LogDirectory) Then
                    LogDirectory = CustomLOGParams.LogDirectory
                End If

                If Not String.IsNullOrEmpty(CustomLOGParams.LogFileName) Then
                    LogFileName = CustomLOGParams.LogFileName
                End If

                If Not String.IsNullOrEmpty(CustomLOGParams.LogDescrizioneUtente) Then
                    LogDescrizioneUtente = CustomLOGParams.LogDescrizioneUtente
                End If

            ElseIf objParametri IsNot Nothing Then

                'Verifico se è stata indicata una directory di LOG
                If objParametri.LogDirectory = "" Then
                    objParametri.LogDirectory = DefaultDirectoryLOG
                End If
                LogDirectory = objParametri.LogDirectory

                'Verifico se è stato indicato un file di LOG
                If objParametri.LogFileName = "" Then
                    objParametri.LogFileName = DefaultFileLOG
                End If
                LogFileName = objParametri.LogFileName
                LogDescrizioneUtente = objParametri.LogDescrizioneUtente
            End If

            Dim LogxThread = False
            If Not IsNothing(ConfigurationManager.AppSettings("LogxThread")) AndAlso Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("LogxThread").ToString()) Then
                If ConfigurationManager.AppSettings("LogxThread").ToString = "True" Then
                    LogxThread = True
                End If
            End If

            If LogxThread Then
                Dim idThread = System.Threading.Thread.CurrentThread.ManagedThreadId
                LogFileName = idThread.ToString() & "_" & LogFileName
            End If

            'Verifico se esiste la DIRECTORY di LOG indicata ... altrimenti la creo
            If Not Directory.Exists(LogDirectory) Then
                Directory.CreateDirectory(LogDirectory)
            End If

            'Costruisco il nome completo del file di LOG
            NomeCompletoFileLOG = (LogDirectory & "\" & LogFileName).Replace("\\", "\")

            'Verifico se esiste il FILE di LOG indicato ... altrimenti lo creo
            'Monitor.Enter(timeStamp)
            If Not File.Exists(NomeCompletoFileLOG) Then

                'Creo il file di LOG nuovo
                xStreamWriter = File.CreateText(NomeCompletoFileLOG)
                xStreamWriter.WriteLine("Inizializzazione file di LOG ... " & Date.Now.ToShortDateString & " " & Date.Now.ToLongTimeString)
                xStreamWriter.Flush()
                xStreamWriter.Close()

            End If

            'Costruisco la stringa di testo da scrivere
            'Lavez 28/05/2025 -  ho bisogno di segnare anche i millisecondi.....
            'Testo = Date.Now.ToShortDateString & " " & Date.Now.ToLongTimeString &
            Testo = Date.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") &
                    " {" & LogDescrizioneUtente & "} " &
                    If(CUAA <> "", "CUAA " & CUAA, "") & " : " &
                    If(String.IsNullOrWhiteSpace(NomeRoutine), "", " [" & NomeRoutine & "] : ") &
                    MessaggioErrore

            'Apro il file di LOG
            xStreamWriter = File.AppendText(NomeCompletoFileLOG)

            'Scrivo la stringa
            xStreamWriter.WriteLine(Testo)
            xStreamWriter.WriteLine(vbCrLf & "=================================================================================================" & vbCrLf)
            xStreamWriter.Flush()
            'xStreamWriter.Close()

            If verificaInviaElasticSearch AndAlso
                objParametri IsNot Nothing Then
                VerificaInviaElasticSearchLogger(CUAA, NomeRoutine, Testo, timeStamp, objParametri)
            End If

        Catch ex As Exception
            ScriviLogEventViewer(ex, NomeRoutine, MessaggioErrore, Testo, CUAA, NomeCompletoFileLOG, LogDescrizioneUtente, objParametri)
        Finally

            If Not IsNothing(xStreamWriter) Then
                xStreamWriter.Close()
            End If
            'Monitor.Exit(timeStamp)
        End Try

    End Sub


    '################################################################################

    Public Sub Gestione_LogErrori(ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByVal Sottocartella As String,
                                  ByVal NomeFile_ConEstensione As String,
                                  ByVal IdentUtente As String,
                                  ByVal NomeRoutine As String,
                                  ByVal Log_Errori As String)

        Dim Path_Errore As String

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Sottocartella = Replace(Sottocartella, "\", "-")
        Sottocartella = Replace(Sottocartella, "/", "-")

        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, "\", "-")
        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, "/", "-")
        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, """", "")

        If objParametri_Server.LogDirectory <> "" Then
            If Not objParametri_Server.LogDirectory.EndsWith("\") Then
                objParametri_Server.LogDirectory &= "\"
            End If
            Path_Errore = objParametri_Server.LogDirectory & Sottocartella
        Else
            If Directory.Exists("C:\GIASLAN\Log") Then
                Path_Errore = "C:\GIASLAN\Log\" & Sottocartella
            ElseIf Directory.Exists("C:\Agronica_LOG") Then
                Path_Errore = "C:\Agronica_LOG\" & Sottocartella
            Else
                Path_Errore = Path.GetTempPath()
                If Not Path_Errore.EndsWith("\") Then
                    Path_Errore &= "\"
                End If
                Path_Errore &= Sottocartella
            End If
        End If

        If Not Path_Errore.EndsWith("\") Then
            Path_Errore &= "\"
        End If

        If Path_Errore.Length > 259 Then
            Throw New Exception("Superata la lunghezza tra path e nome del file: " & CStr(Path_Errore.Length) & "caratteri (max 259 caratteri).")
        End If

        Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = IdentUtente,
                .LogDirectory = Path_Errore,
                .LogFileName = NomeFile_ConEstensione
            }

        objLog.Scrivi_LOG(objParametri_Server, NomeRoutine, Log_Errori, CustomLOGParams:=customLOGParams)

    End Sub

    Protected Function PopolaDizionarioTabelleInLingua(ByVal CodiceISO As String) As Dictionary(Of String, String)
        return New Dictionary(Of String, String) From {
            {"Analisi_Tipi", "[Analisi_Tipi_XLingue_" & CodiceISO & "]"},
            {"Avversita", "[Avversita_XLingue_" & CodiceISO & "]"},
            {"CategorieMagazzino", "[CategorieMagazzino_XLingue_" & CodiceISO & "]"},
            {"ClassificazioniFormulati", "[ClassificazioniFormulati_XLingue_" & CodiceISO & "]"},
            {"Cultivar", "[Cultivar_XLingue_" & CodiceISO & "]"},
            {"Epoche", "[Epoche_XLingue_" & CodiceISO & "]"},
            {"Fabbricati_Tipi", "[Fabbricati_Tipi_XLingue_" & CodiceISO & "]"},
            {"FormeAllevamento", "[FormeAllevamento_XLingue_" & CodiceISO & "]"},
            {"GruppoAvversita", "[GruppoAvversita_XLingue_" & CodiceISO & "]"},
            {"GruppoFinalita", "[GruppoFinalita_XLingue_" & CodiceISO & "]"},
            {"GruppoOperazioni", "[GruppoOperazioni_XLingua_" & CodiceISO & "]"},
            {"GruppoVarietale", "[GruppoVarietale_XLingue_" & CodiceISO & "]"},
            {"GruppoVegetale", "[GruppoVegetale_XLingue_" & CodiceISO & "]"},
            {"ImpiantiIrrigazioni", "[ImpiantiIrrigazioni_XLingue_" & CodiceISO & "]"},
            {"Macchine", "[Macchine_XLingue_" & CodiceISO & "]"},
            {"Materie_Prime", "[Materie_Prime_XLingue_" & CodiceISO & "]"},
            {"MenuBS_2017_Sezioni", "[MenuBS_2017_Sezioni_XLingua_" & CodiceISO & "]"},
            {"Note_Intervento", "[Note_Intervento_XLingue_" & CodiceISO & "]"},
            {"Note_Intervento_Gruppi", "[Note_Intervento_Gruppi_XLingue_" & CodiceISO & "]"},
            {"Note_Intervento_Utilizzo", "[Note_Intervento_Utilizzo_XLingue_" & CodiceISO & "]"},
            {"Operazioni", "[Operazioni_XLingue_" & CodiceISO & "]"},
            {"Portinnesti", "[Portinnesti_XLingue_" & CodiceISO & "]"},
            {"Prenotazione_Piante_Categoria", "[Prenotazione_Piante_Categoria_XLingue_" & CodiceISO & "]"},
            {"Prenotazione_Piante_Certificazione", "[Prenotazione_Piante_Certificazione_XLingue_" & CodiceISO & "]"},
            {"Rapporti_Contabili", "[Rapporti_Contabili_XLingue_" & CodiceISO & "]"},
            {"SpecieVegetali", "[SpecieVegetali_XLingue_" & CodiceISO & "]"},
            {"SpecieVegetaliXStadiCrescita", "[StadiXSpecieVegetali_XLingua_" & CodiceISO & "]"},
            {"StampeReport", "[StampeReport_XLingue_" & CodiceISO & "]"},
            {"Tipologie", "[Tipologie_XLingue_" & CodiceISO & "]"},
            {"TipologieSementi", "[TipologieSementi_XLingue_" & CodiceISO & "]"},
            {"UnitaMisura", "[UnitaMisura_XLingue_" & CodiceISO & "]"},
            {"Modalita_Applicazione_Globali", "[Modalita_Applicazione_Globali_XLingue_" & CodiceISO & "]"},
            {"Modalita_Applicazione", "[Modalita_Applicazione_XLingue_" & CodiceISO & "]"},
            {"Lista_Categorie_Animali", "[Lista_Categorie_Animali_XLingue_" & CodiceISO & "]"},
            {"Lista_Causali_Morte", "[Lista_Causali_Morte_XLingue_" & CodiceISO & "]"},
            {"Lista_Generi_Animali", "[Lista_Generi_Animali_XLingue_" & CodiceISO & "]"},
            {"Lista_IndirizziProd_Animali", "[Lista_IndirizziProd_Animali_XLingue_" & CodiceISO & "]"},
            {"Lista_Patologie", "[Lista_Patologie_XLingue_" & CodiceISO & "]"},
            {"Lista_Razze_Animali", "[Lista_Razze_Animali_XLingue_" & CodiceISO & "]"},
            {"Lista_Specie_Animali", "[Lista_Specie_Animali_XLingue_" & CodiceISO & "]"},
            {"Lista_Tipi_Raggruppamento_Stalla", "[Lista_Tipi_Raggruppamento_Stalla_XLingue_" & CodiceISO & "]"},
            {"Guida_Impostazioni", "[Guida_Impostazioni_XLingue_" & CodiceISO & "]"},
            {"Guida_Impostazioni_Sezioni", "[Guida_Impostazioni_Sezioni_XLingue_" & CodiceISO & "]"},
            {"Guida_Impostazioni_Valori", "[Guida_Impostazioni_Valori_XLingue_" & CodiceISO & "]"}
        }
    End Function

    Private Shared Sub ReplaceInvalidChars(FileLOG As String, ByRef NomeCompletoFileLOG As String)
        '  Giulia, 04/11/2016 12.00.43: Migliore gestione errore di creazione file di log,
        '                   perché il nome file (spesso contenente la ragione sociale) contiene caratteri invalidi
        Dim InvalidName As Char() = Path.GetInvalidFileNameChars
        'Dim InvalidPath As Char() = Path.GetInvalidPathChars
        Dim InvalidCharFound As New Hashtable
        For Each CharInvalid In InvalidName
            If FileLOG.Contains(CharInvalid) Then
                InvalidCharFound.Add(FileLOG.IndexOf(CharInvalid), CharInvalid)
            End If
        Next
        If InvalidCharFound.Count <> 0 Then
            Dim StringInvaliChar As String = ""
            For Each pair In InvalidCharFound.Keys
                StringInvaliChar = StringInvaliChar & "Carattere: " & InvalidCharFound.Item(pair).ToString &
                                   " [ASCII code " & Asc(CChar(InvalidCharFound.Item(pair))) & "] - in posizione [" & CInt(pair) + 1 & "]" & vbCrLf
                'Tenta la sostituzione
                NomeCompletoFileLOG.Replace(CStr(InvalidCharFound.Item(pair)), "-")
            Next
        End If
    End Sub

    Private Function LeggiCUAA(ByRef objParametri As AgronicaCoreParametri) As String
        Const nomeRoutine = "LeggiCUAA"
        Dim CUAA As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim objProvider As New AgronicaCoreDataProvider.DataProvider
        Try

            strSql.Length = 0
            strSql.AppendLine(" IF EXISTS (SELECT * FROM sys.views where [name] = 'Last_Impresa_Selezionata_Dashboard' )  ")
            strSql.AppendLine(" BEGIN ")
            strSql.AppendLine(" SELECT TOP 1 ISNULL(i.val_cod, '') AS CUAA, l.Piva  ")
            strSql.AppendLine(" FROM Last_Impresa_Selezionata_Dashboard l WITH (NOLOCK)")
            strSql.AppendLine(" LEFT JOIN Imprese_Codici i WITH (NOLOCK) ON i.piva = l.piva AND id_cod = " & UtilityProvider.Agro_SQL_SaveNum(enum_CodiciAnagrafe.CodiceCUAA) & " ")
            strSql.AppendLine(" WHERE 1 = 1 ")
            strSql.AppendLine(" AND Username = '" & UtilityProvider.Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            strSql.AppendLine(" ORDER BY l.datainvio DESC ")
            strSql.AppendLine(" END ")

            '--------------------------------------------------------------------------
            dt = objProvider.EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine, chiamaScriviLog:=False)
            '--------------------------------------------------------------------------
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                CUAA = dt.Rows(0).Item("CUAA")
                'Se non riesco a ricavare il CUAA, prendo la PIVA
                If CUAA = "" Then
                    CUAA = dt.Rows(0).Item("Piva") & " [PIVA]"
                End If
            End If
        Catch ex As Exception
            CUAA = ""
        End Try
        Return CUAA
    End Function

#Region "ELASTIC SEARCH"
    Private Sub VerificaInviaElasticSearchLogger(ByVal CUAA As String, ByVal NomeRoutine As String, ByVal Testo As String, ByVal timeStamp As Date, ByVal objParametri As AgronicaCoreParametri)
        If objParametri.StringaConnessioneEncrypted Then
            'Necessario perché il thread non vede la stringa di connessione decriptata
            objParametri.StringaConnessione = objParametri.StringaConnessione
        End If

        'creo una copia del obj parametri e imposto connessione e transazione a Nothing, in modo tale da non avere problemi di concorrenza dovuti all'utilizzo della stessa istanza di objParametri
        Dim newObjParametri = objParametri.CreateDeepCopy(objParametri)
        newObjParametri.objConnessione = Nothing
        newObjParametri.objTransazione = Nothing

        ThreadPool.QueueUserWorkItem(Sub()
                                         Try
                                             ElasticSearchLogger(CUAA, NomeRoutine, Testo, timeStamp, newObjParametri)
                                         Catch ex As Exception
                                             ' Ultima difesa: se fallisce anche ElasticSearchLogger fuori dal Try interno
                                             Scrivi_LOG(newObjParametri,
                                                       NomeRoutine,
                                                       "Errore nel thread ElasticSearchLogger: " & ex.ToString(),
                                                       verificaInviaElasticSearch:=False)

                                         End Try
                                     End Sub)
    End Sub

    Private Sub ElasticSearchLogger(ByVal CUAA As String, ByVal NomeRoutine As String, ByVal Testo As String, ByVal timeStamp As Date, ByVal objParametri As AgronicaCoreParametri)

        Dim cfg As ConfigurazioneLogProviderEsteso = ConfigurazioneLogProviderFactory.Instance(objParametri)
        Dim urlColdirettiLogger As String = String.Empty
        Dim objParametriES As ParametriElasticSearch = Nothing

        If Not IsNothing(cfg) AndAlso Not IsNothing(cfg.ConfigurazioneElasticSearch) Then
            urlColdirettiLogger = cfg.ConfigurazioneElasticSearch.ElasticSearchUrl
            objParametriES = cfg.ConfigurazioneElasticSearch.Parametri
        End If

        If urlColdirettiLogger <> "" Then
            Try
                If objParametriES IsNot Nothing AndAlso objParametriES.Ambiente <> "" Then
                    Dim objLog As New ElasticSearchLogger(objParametriES.Ambiente, urlColdirettiLogger)
                    objLog.WriteLog(CUAA, NomeRoutine, Testo, timeStamp)
                Else
                    Throw New Exception("ParametriElasticSearch.Ambiente non configurato correttamente.")
                End If
            Catch ex As Exception
                Scrivi_LOG(objParametri,
                           NomeRoutine,
                           "Si è verificato un errore durante l'esportazione del log ad ElasticSearch: " & ex.ToString, verificaInviaElasticSearch:=False)
            End Try
        End If
    End Sub
#End Region

#Region "SCRITTURA LOG EVENTI"

    Public Sub ScriviLogEventViewer(ByRef ex As Exception,
                                    Optional NomeRoutine As String = "",
                                    Optional MessaggioErrore As String = "",
                                    Optional Testo As String = "",
                                    Optional CUAA As String = "",
                                    Optional NomeCompletoFileLOG As String = "",
                                    Optional LogDescrizioneUtente As String = "",
                                    Optional ByRef objParametri As AgronicaCoreParametri = Nothing)

        Dim dettagli As New Text.StringBuilder

        Dim nomeLog As String = "Gias"
        Dim sourceLog As String = ""

        Dim scriviLogEventViewer As Boolean = True

        Dim sourceExists As Boolean = False
        Dim canCreateEventSource As Boolean = True  'Assumo che si possa creare la event source

        Try

            If ConfigurationManager.AppSettings("ScriviLog_EventViewer") IsNot Nothing Then
                Boolean.TryParse(ConfigurationManager.AppSettings("ScriviLog_EventViewer"), scriviLogEventViewer)
            End If

            ' Se la configurazione non è presente o è impostata a false, esci senza fare nulla
            If Not scriviLogEventViewer Then
                Exit Sub
            End If

            RicavaCreaNomiLogEventi(nomeLog, sourceLog, sourceExists, canCreateEventSource)

            Using logAgroServizio As New EventLog(nomeLog)
                logAgroServizio.Source = sourceLog

                CostruisciDettagliLog(dettagli, ex, NomeRoutine, CUAA, MessaggioErrore, Testo, NomeCompletoFileLOG, LogDescrizioneUtente, objParametri)

                logAgroServizio.WriteEntry(dettagli.ToString(), EventLogEntryType.Error, 1000)
            End Using

        Catch exInternal As Exception

            Try
                'Se ha generato una qualche eccezione nella scritta del log eventi
                'cerco di scrivere un file di log in modo brutale
                'direttamente nella directory principale dell'applicazione

                Dim contenuto As String = $"{exInternal.Message} [{exInternal.InnerException?.Message}]"
                contenuto &= vbCrLf & "NOME LOG: " & nomeLog
                contenuto &= vbCrLf & "SOURCE LOG: " & sourceLog
                contenuto &= vbCrLf & "sourceExists: " & sourceExists.ToString()
                contenuto &= vbCrLf & "scriviLogEventViewer: " & scriviLogEventViewer.ToString()
                contenuto &= vbCrLf & "canCreateEventSource: " & canCreateEventSource.ToString()
                contenuto &= vbCrLf & "DETTAGLI: " & dettagli.ToString()
                contenuto &= vbCrLf & vbCrLf & "STACK: " & vbCrLf & exInternal.StackTrace

                ScriviLogBrutale("ScriviLog_EventViewer", contenuto)
            Catch
                ' Se anche qui fallisce, non fare nulla per evitare loop
            End Try
        End Try

    End Sub

    Private Sub RicavaCreaNomiLogEventi(ByRef nomeLog As String, ByRef sourceLog As String,
                                        ByRef sourceExists As Boolean, ByRef canCreateEventSource As Boolean)

        Const NOME_LOG_DEFAULT As String = "Application"
        Const SOURCE_LOG_DEFAULT As String = ".NET Runtime"

        sourceLog = GetSourceName() 'Questo può cambiare di volta in volta
        If ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax") IsNot Nothing AndAlso Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax")) Then
            nomeLog = ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax") & ".SLog"
            sourceLog = ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax")
        End If

        Try
            sourceExists = EventLog.SourceExists(sourceLog)
        Catch exPerm As Exception
            ' Non hai i permessi per accedere a tutti i log: ignora o logga l'errore
            sourceExists = False
        End Try

        If Not sourceExists Then
            Try
                ' Prova a richiedere i permessi di amministratore per la creazione della event source
                Dim perm As New EventLogPermission(EventLogPermissionAccess.Administer, ".")
                perm.Demand()
            Catch exPerm As Exception
                canCreateEventSource = False
            End Try

            If canCreateEventSource Then
                Try
                    EventLog.CreateEventSource(sourceLog, nomeLog)
                Catch exCreate As Exception
                    ' Se non puoi creare la source, usa una source di default già esistente
                    nomeLog = NOME_LOG_DEFAULT
                    sourceLog = SOURCE_LOG_DEFAULT
                End Try
            Else
                ' Se non hai i permessi, usa il log Application e come source una già esistente
                nomeLog = NOME_LOG_DEFAULT
                sourceLog = SOURCE_LOG_DEFAULT
            End If

        End If
    End Sub

    Private Sub CostruisciDettagliLog(ByRef dettagli As Text.StringBuilder,
                                      ByRef ex As Exception,
                                      ByVal NomeRoutine As String, CUAA As String, MessaggioErrore As String, Testo As String,
                                      ByVal NomeCompletoFileLOG As String, ByVal LogDescrizioneUtente As String,
                                      ByRef objParametri As AgronicaCoreParametri)

        dettagli.AppendLine($"APPLICAZIONE: {GetSourceName()} ")
        dettagli.AppendLine("")
        dettagli.AppendLine($" - Versione: {GetVersioneGiasTxt()} ")
        dettagli.AppendLine($" - Base Directory: {AppDomain.CurrentDomain.BaseDirectory} ")
        dettagli.AppendLine("")
        dettagli.AppendLine($" - ENTRY Assembly: {GetEntryAssemblyName()} ")
        dettagli.AppendLine($" - CALLING Assembly: {GetCallingAssemblyName()} ")

        If Web.HttpContext.Current IsNot Nothing Then
            ' Se siamo in ambiente web restituisci la virtual directory senza slash
            Dim virtualPath As String = Web.HttpRuntime.AppDomainAppVirtualPath.Trim("/"c)
            Dim physicalPath As String = Web.HttpRuntime.AppDomainAppPath
            dettagli.AppendLine("")
            dettagli.AppendLine($" - WEB - VirtualPath: {virtualPath} ")
            dettagli.AppendLine($" - WEB - PhysicalPath: {physicalPath} ")
        End If

        dettagli.AppendLine("")
        dettagli.AppendLine($"Routine: {NomeRoutine} ")
        dettagli.AppendLine("")
        dettagli.AppendLine($" - CUAA Scrivi_Log: {CUAA} ")
        dettagli.AppendLine($" - Messaggio Scrivi_Log: {MessaggioErrore} ")
        dettagli.AppendLine($" - Testo Scrivi_Log: {Testo} ")
        dettagli.AppendLine($" - Nome File Scrivi_Log: {NomeCompletoFileLOG} ")
        dettagli.AppendLine($" - Param LogDescrizioneUtente: {LogDescrizioneUtente} ")
        dettagli.AppendLine("")
        dettagli.AppendLine($" - Eccezione: {ex.Message} ")
        dettagli.AppendLine($" - Eccezione Interna: {ex.InnerException} ")
        dettagli.AppendLine("")
        'dettagli.AppendLine($"StackTrace: {ex.StackTrace.ToString()} ")
        'dettagli.AppendLine("")
        dettagli.AppendLine($"STACK COMPLETO: {vbCrLf}{DammiStackTrace()} ")
        If objParametri IsNot Nothing Then
            dettagli.AppendLine("")
            dettagli.AppendLine($" - Parametri: {JsonConvert.SerializeObject(objParametri)} ")
        End If
    End Sub

    Private Function DammiStackTrace() As String
        Dim metodoChiamante As String = String.Empty
        Try
            Dim st As New StackTrace()
            Dim sb As New Text.StringBuilder

            Dim frames As List(Of StackFrame) = st.GetFrames().Skip(1).Take(10).ToList()

            For Each f As StackFrame In frames
                Dim myMethod As Reflection.MethodBase = f.GetMethod()
                Dim myDeclaringType As Type = myMethod.DeclaringType
                If myDeclaringType IsNot Nothing Then
                    Dim paramList As String = String.Join(", ", myMethod.GetParameters().Select(Function(p) $"{p.ParameterType.Name} {p.Name}"))
                    sb.AppendLine($"   in {myDeclaringType.FullName}.{myMethod.Name}({paramList})")
                End If
            Next
            metodoChiamante = sb.ToString()
        Catch
            ' Se non riesci a ottenere lo stack, passa oltre
        End Try
        Return metodoChiamante
    End Function

    Private Sub ScriviLogBrutale(fileName As String, contenuto As String)
        Dim filePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fileName}.txt")

        Using fstr As IO.FileStream = IO.File.Open(filePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
            Dim sw As New IO.StreamWriter(fstr)

            sw.WriteLine(vbCrLf & "======================================================" & vbCrLf)
            sw.WriteLine(contenuto)
            sw.WriteLine(vbCrLf & "======================================================" & vbCrLf)

            sw.Flush()
            sw.Dispose()
        End Using
    End Sub

    Private Function GetSourceName() As STring
        Dim source As String = ""

        If Web.HttpContext.Current IsNot Nothing Then
            ' Se siamo in ambiente web restituisci la virtual directory senza slash
            source = Web.HttpRuntime.AppDomainAppVirtualPath.Trim("/"c)
        Else
            source = GetFriendlyAppName()
        End If

        Return source
    End Function

    Private Function GetFriendlyAppName() As String
        Try
            ' Se siamo in un'applicazione console o desktop, restituisci il nome dell'assembly principale

            Dim appName As String
            Dim appNameByAssemblyEntry As String = GetEntryAssemblyName()
            Dim appNameByAssemblyCall As String = GetCallingAssemblyName()

            If Not String.IsNullOrEmpty(appNameByAssemblyEntry) Then
                appName = appNameByAssemblyEntry
            ElseIf Not String.IsNullOrEmpty(appNameByAssemblyCall) Then
                appName = appNameByAssemblyCall
            Else
                appName = AppDomain.CurrentDomain.FriendlyName
            End If

            Return appName
        Catch
            Return AppDomain.CurrentDomain.FriendlyName
        End Try
    End Function

    Private Function GetEntryAssemblyName() As String
        Dim appName As String = ""
        Try
            Dim entryAssembly As Reflection.Assembly = Reflection.Assembly.GetEntryAssembly()

            If entryAssembly IsNot Nothing Then
                appName = entryAssembly.GetName().Name
            End If
        Catch
            'Se non riesci a ottenere il nome dell'assembly, passa oltre
        End Try
        Return appName
    End Function

    Private Function GetCallingAssemblyName() As String
        Dim appName As String = ""
        Try
            Dim callingAssembly As Reflection.Assembly = Reflection.Assembly.GetCallingAssembly()

            If callingAssembly IsNot Nothing Then
                appName = callingAssembly.GetName().Name
            End If
        Catch
            'Se non riesci a ottenere il nome dell'assembly, passa oltre
        End Try
        Return appName
    End Function

    Private Function GetVersioneGiasTxt() As String
        Dim versioneGias As String = ""
        Try
            Dim filePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GiasVersioneCorrente.txt")
            If File.Exists(filePath) Then
                versioneGias = File.ReadAllText(filePath).Trim()
            Else
                ' Se il file non esiste, ritorna un messaggio di errore
                versioneGias = "File GiasVersioneCorrente.txt non trovato."
            End If
        Catch ex As Exception
            ' Se non riesci a ottenere la versione, ritorna un messaggio di errore
            versioneGias = "Errore nel recupero della versione: " & ex.Message
        End Try
        Return versioneGias
    End Function

#End Region


End Class

Public Class CustomLOGParams
    Public LogDirectory As String = ""
    Public LogFileName As String = ""
    Public LogDescrizioneUtente As String = ""
End Class