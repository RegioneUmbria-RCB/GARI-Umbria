Imports System.Threading
Imports System.Timers
Imports System.Web.SessionState
Imports AgronicaControlli_2010
Imports AgronicaCoreGestioneRichieste

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Private _errore As String = ""
    Private _eventLog As EventLog = Nothing
    Private _logWarmUp As Boolean = False
    Private _objConnectionStringList As List(Of String)
    Private _contatore As Integer = 1
    Private _util As New UtilityWS
    Private _aTimer As Timers.Timer
    Private _firstTimeTimer As Boolean = True
    Private _watch As New Stopwatch
    Private secondsTimer As Long = 0


    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        '' Generato all'avvio dell'applicazione
        'System.Net.ServicePointManager.DefaultConnectionLimit = 2000
        Const nomeRoutine = "Global.asax->Application_Start"
        Dim messaggioErrore As String = ""

        'Scommentare la riga sotto per forzare l'interruzione per debug
        'Debugger.Launch()

        Debugger.Log(1, nomeRoutine, "Application_Start AVVIATA!")

        Try

            ' inizializza stringhe connessione in application
            GlobalAsax_Helper.Inizializza_Stringhe_Connessione()

            '22/02/2021: ora con EF6 il WarmUp EF è disattivato per tutti perché inutile e l'EF6 viene inizializzato in maniera diversa

            '_logWarmUp = If(ConfigurationManager.AppSettings("Log_Warm_Up_In_Eventi") = "true", True, False)

            'If ConfigurationManager.AppSettings("WarmUp_EF") = "true" Then

            '    _util.LogEvent(_eventLog, "[" & nomeRoutine & "] : Warm Up EF ATTIVATO ", EventLogEntryType.Warning, scriviLogEventi:=_logWarmUp)

            '    Dim res As Boolean = ThreadPool.QueueUserWorkItem(AddressOf DoMyBackgroundWorkOnNewThread)

            '    Debugger.Log(1, nomeRoutine, "res = " & res.ToString)
            'Else
            '    _util.LogEvent(_eventLog, "[" & nomeRoutine & "] : Warm Up EF DISATTIVATO ", EventLogEntryType.Warning, scriviLogEventi:=_logWarmUp)
            'End If


            'Dim trd As New Threading.Thread(AddressOf DoMyBackgroundWorkOnNewThread) With {
            '    .IsBackground = True
            '}
            'trd.Start()

        Catch ex As Exception
            messaggioErrore = ex.Message
            _errore &= "[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf
            _util.LogEvent(_eventLog, "[" & nomeRoutine & "] : " & messaggioErrore, EventLogEntryType.Error, scriviLogEventi:=_logWarmUp)
        End Try

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'avvio della sessione
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'inizio di ogni richiesta

        GlobalAsax_Helper.Manage_BeginRequest_Security(sender, e, Me)
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al tentativo di autenticare l'utilizzo
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando si verifica un errore
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine della sessione
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine dell'applicazione
    End Sub

    Private Sub Global_asax_PostMapRequestHandler(sender As Object, e As EventArgs) Handles Me.PostMapRequestHandler

        GlobalAsax_Helper.Manage_PostMapRequestHandler_Security(sender, e)

    End Sub

    Private Sub DoMyBackgroundWorkOnNewThread()

        Const nomeRoutine = "Global.asax->Application_Start"
        Dim messaggioErrore As String = ""
        Dim minutesTimer As Integer = 0

        Debugger.Log(1, "Global", "Thread AVVIATO!")

        Try

            minutesTimer = CInt(ConfigurationManager.AppSettings("Minutes_Timer"))

            _objConnectionStringList = _util.GeneraListaStringheConnessione(_errore, _eventLog, _logWarmUp)

            If _objConnectionStringList Is Nothing Then
                Throw New ConfigurationErrorsException("[" & nomeRoutine & "] : La stringa di connessione per il Warm Up dell'Entity Framework è vuota. " &
                                                           "Verificare il web.config del Core WS" & vbCrLf & _errore)
            End If

            If _objConnectionStringList.Count = 0 Then
                Throw New ConfigurationErrorsException("[" & nomeRoutine & "] : L'elenco delle stringhe di connessione per il Warm Up dell'Entity Framework è vuoto. " &
                                                           "Verificare il web.config del Core WS" & vbCrLf & _errore)
            End If

            'visto che con il timer non farebbe partire subito il warm up ma solo dopo che è passato il tempo impostato, la prima volta lo eseguo direttamente qui
            If _firstTimeTimer = True Then
                RunEvent()
                _firstTimeTimer = False
            End If

            If _firstTimeTimer = False AndAlso minutesTimer > 0 Then
                RunTimer(minutesTimer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            If Not IsNothing(ex.InnerException) Then
                messaggioErrore = messaggioErrore & " [" & ex.InnerException.Message & "]"
            End If
            _errore &= "[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf
            _util.LogEvent(_eventLog, "[" & nomeRoutine & "] : " & messaggioErrore, EventLogEntryType.Error, scriviLogEventi:=_logWarmUp)
        End Try

    End Sub


    Public Sub RunTimer(ByVal minutesTimer As Integer)
        Const nomeRoutine = "RunTimer"
        Dim messaggioErrore As String = ""

        Try
            _aTimer = New Timers.Timer()    'This will set the default interval
            _aTimer.Interval = minutesTimer * 60 * 1000

            ' Hook up the Elapsed event for the timer.  
            AddHandler _aTimer.Elapsed, AddressOf RunEvent

            ' Have the timer fire repeated events (true is the default)
            _aTimer.AutoReset = True

            ' Start the timer
            _aTimer.Start()

        Catch ex As Exception
            messaggioErrore = ex.Message
            If Not IsNothing(ex.InnerException) Then
                messaggioErrore = messaggioErrore & " [" & ex.InnerException.Message & "]"
            End If
            _errore &= "[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf
            _util.LogEvent(_eventLog, "[" & nomeRoutine & "] : " & messaggioErrore, EventLogEntryType.Error, scriviLogEventi:=_logWarmUp)
        End Try

    End Sub

    Public Sub RunEvent()
        Const nomeRoutine = "RunEvent"
        Dim messaggioErrore As String = ""
        Dim result As Boolean = False
        Dim nomeDB As String = ""
        Dim numDB As Integer = 0
        Dim globalWatch As New Stopwatch
        Dim timeEx As TimeSpan

        Try
            _util.LogEvent(_eventLog, "[" & nomeRoutine & "] :  START -    Esecuzione N. = " & _contatore.ToString, EventLogEntryType.Information, scriviLogEventi:=_logWarmUp)

            globalWatch.Restart()

            For Each strConnessione In _objConnectionStringList

                Dim valoreRitorno As String = ""
                nomeDB = Array.Find(strConnessione.Split(";"), Function(s) s.Contains("Initial Catalog")).Split("=")(1)
                numDB += 1

                _watch.Restart()

                result = _util.WarmUpEF(_errore, _eventLog, strConnessione, valoreRitorno, nomeDB, _logWarmUp)

                _watch.Stop()

                timeEx = _watch.Elapsed

                _util.LogEvent(_eventLog, "[" & nomeRoutine & "] :  " & vbCrLf &
                               "    FINISH -   Esecuzione N. = " & _contatore.ToString & vbCrLf & vbCrLf &
                               " - NUM DB = " & numDB & vbCrLf &
                               " - DB = " & nomeDB & vbCrLf & vbCrLf &
                               " - VALORE = " & valoreRitorno & vbCrLf &
                               " - RESULT = " & result & vbCrLf &
                               " - TIME = " & timeEx.Minutes & " Minuti e " & timeEx.Seconds & " Secondi e " & timeEx.Milliseconds & " Millisecondi",
                               EventLogEntryType.Information, 50, _logWarmUp)

            Next

            globalWatch.Stop()

            timeEx = globalWatch.Elapsed

            _util.LogEvent(_eventLog, "[" & nomeRoutine & "] :  FINISH GLOBAL -    Esecuzione N. = " & _contatore.ToString & vbCrLf &
                           " - NUM DB WARMED = " & numDB & vbCrLf &
                           " " & timeEx.Minutes & " Minuti e " & timeEx.Seconds & " Secondi e " & timeEx.Milliseconds & " Millisecondi",
                           EventLogEntryType.Information, 100, _logWarmUp)

            _contatore += 1

        Catch ex As Exception
            messaggioErrore = ex.Message
            If Not IsNothing(ex.InnerException) Then
                messaggioErrore = messaggioErrore & " [" & ex.InnerException.Message & "]"
            End If
            _errore &= "[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf
            _util.LogEvent(_eventLog, "[" & nomeRoutine & "] : " & messaggioErrore, EventLogEntryType.Error, scriviLogEventi:=_logWarmUp)
        End Try

    End Sub

End Class