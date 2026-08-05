Imports System.IO
Imports System.Collections.ObjectModel
Imports System.Configuration
Imports System.Xml
Imports System.ServiceProcess
Imports System.Threading
Imports System.Diagnostics
Imports AgronicaGSB_Gestione_Servizi_in_Background.Utility
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste

Public Class ConfigurazioneServizioWindows
    Inherits System.ServiceProcess.ServiceBase



#Region " Codice generato da Progettazione componenti "

    'ATTENZIONE!!! MASSIMO 8 CARATTERI perché solo i primi 8 sono significativi per il file di log
    Private Nome_LogAgroServizio_8CaratteriMax_DEFAULT As String = "Agro-GSB"
    Property Nome_LogAgroServizio_8CaratteriMax As String = "Agro-GSB"

    Public Sub New()
        MyBase.New()

        ' Chiamata richiesta da Progettazione componenti.
        InitializeComponent()

        ' Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent()
        If Not EventLog.SourceExists(Nome_LogAgroServizio_8CaratteriMax) Then
            EventLog.CreateEventSource(Nome_LogAgroServizio_8CaratteriMax, Nome_LogAgroServizio_8CaratteriMax & ".SLog")
        End If
        LogAgroServizio.Source = Nome_LogAgroServizio_8CaratteriMax
        LogAgroServizio.Log = Nome_LogAgroServizio_8CaratteriMax & ".SLog"

    End Sub

    'UserService esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Punto di ingresso principale del processo
    <MTAThread()> _
    Shared Sub Main()
        Dim servicesToRun() As System.ServiceProcess.ServiceBase

        ' All'interno di uno stesso processo è possibile eseguire più servizi di Windows NT.
        ' Per aggiungere un servizio al processo, modificare la riga che segue in modo
        ' da creare un secondo oggetto servizio. Ad esempio,
        '
        '   servicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        servicesToRun = New System.ServiceProcess.ServiceBase() {New ConfigurazioneServizioWindows}

        System.ServiceProcess.ServiceBase.Run(servicesToRun)
    End Sub

    'Richiesto da Progettazione componenti
    Private components As System.ComponentModel.IContainer

    ' NOTA: la procedura che segue è richiesta da Progettazione componenti
    ' Può essere modificata in Progettazione componenti.  
    ' Non modificarla nell'editor del codice.
    Friend WithEvents LogAgroServizio As System.Diagnostics.EventLog

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        If Not IsNothing(ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax")) AndAlso
                ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax") <> "" Then

            Nome_LogAgroServizio_8CaratteriMax = ConfigurationManager.AppSettings("Nome_LogAgroServizio_8CaratteriMax")

        End If

        If Nome_LogAgroServizio_8CaratteriMax.Length > 8 Then
            Nome_LogAgroServizio_8CaratteriMax = Nome_LogAgroServizio_8CaratteriMax_DEFAULT
        End If

        Me.LogAgroServizio = New System.Diagnostics.EventLog
        CType(Me.LogAgroServizio, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'ConfigServizioSincronizzatore
        '
        Me.ServiceName = Nome_LogAgroServizio_8CaratteriMax
        CType(Me.LogAgroServizio, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

#End Region



#Region "Parametri "



    'Parametri app_config per filtro (non obbligatori, si lascia il default)
    Private Property Filtro_Tipo_Sincro As enum_Tipi_Servizi_Background = 0
    Private Property Filtro_Id_Riga As Integer = 0
    Private Property Filtro_PivaSuperuser As String = ""

    'Parametri per leggere da db (vengono passati all'AgroServizio)
    Private Property StringaConnessione_SuperServer As String
    'Private Property StringaConnessione_Server As String
    'Private Property StringaConnessione_Utenti As String

#End Region



#Region "Codice Personalizzato "

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Inserire qui il codice necessario per avviare il proprio servizio. Il metodo deve effettuare 
        ' le impostazioni necessarie per il funzionamento del servizio.

        'L'istruzione serve x il debug al momento dello Start del servizio, per avere il tempo di agganciare il processo al debugger
        Dim tout As Integer
        tout = 1000
        Try

            tout = tout * ConfigurationManager.AppSettings("pausaPerDebug")
        Catch ex As Exception

        End Try
        Thread.Sleep(tout)


        Try

            'Leggo i parametri dall'AppConfig
            '<!-- ##### cnONLINE_Super_Server ########
            '<add key="Super_Server" value="true"/>
            '<add key="Super_Server_Provider" value="SQLOLEDB"/>
            '<add key="Super_Server_Server" value="*****"/>
            '<add key="Super_Server_DB" value="GIAS_Super_Server"/>
            '<add key="Super_Server_UserId" value="*****"/>
            '<add key="Super_Server_Password" value="*****"/>
            '<add key="Super_Server_PivaSuperUser" value=""/>


       
            If Not IsNothing(ConfigurationManager.AppSettings("PivaSuperuser")) Then
                Filtro_PivaSuperuser = ConfigurationManager.AppSettings("PivaSuperuser")
            End If
            If Not IsNothing(ConfigurationManager.AppSettings("Tipo_Sincro")) Then
                Filtro_Tipo_Sincro = CInt(ConfigurationManager.AppSettings("Tipo_Sincro"))
            End If
            If Not IsNothing(ConfigurationManager.AppSettings("Id_Riga")) Then
                Filtro_Id_Riga = CInt(ConfigurationManager.AppSettings("Id_Riga"))
            End If

            Try

                'creo e avvio gli n thread in base alle n righe della tabella
                Crea_e_Avvia_Threads_Servizi()



            Catch ex As Exception
                ScriviLogServizio(LogAgroServizio, "Errore in fase di avvio del servizio " & Nome_LogAgroServizio_8CaratteriMax & " : " & ex.Message, EventLogEntryType.Error, id_Evento:=ID_eventi.ErroreInAvvioServizio)
            End Try

            ScriviLogServizio(LogAgroServizio, "Servizio " & Nome_LogAgroServizio_8CaratteriMax & " avviato.", id_Evento:=ID_eventi.ServizioAvviato)

        Catch ex As Exception
            ScriviLogServizio(LogAgroServizio, "Impossibile caricare il file di configurazione. " & ex.Message, EventLogEntryType.Error, id_Evento:=ID_eventi.ErroreInAvvioServizio)
        End Try

    End Sub

    Protected Overrides Sub OnStop()
        ' Inserire qui il codice delle procedure di chiusura necessarie per arrestare il proprio servizio.
        Dim threadServizio As Thread
        For Each threadServizio In ListaThread
            If threadServizio.IsAlive Then threadServizio.Abort()
        Next
        ScriviLogServizio(LogAgroServizio, "Servizio " & Nome_LogAgroServizio_8CaratteriMax & " arrestato.", id_Evento:=Utility.ID_eventi.servizioArrestato)

    End Sub

    Private Sub ImpTh__EventoErrore(ByVal sender As Object, ByVal e As EventoErroreServizio) ' Handles ImpTh._EventoErrore
        ScriviLogServizio(LogAgroServizio, e.Descrizione, EventLogEntryType.Error)
    End Sub

    Private Sub ImpTh__EventoImportazione(ByVal sender As Object, ByVal e As EventoServizio) 'Handles ImpTh._EventoImportazione
        ScriviLogServizio(LogAgroServizio, e.Descrizione)
    End Sub

#End Region



#Region "Gesione servizi threads"


    ''Filtri Opzionali
    'Property Filtro_Tipo_Sincro As TipiEnumerativi.enum_Tipi_Servizi_Background
    'Property Filtro_Id_Riga As Integer
    'Property Filtro_PivaSuperuser As String
    'Property Agro_Servizio_List As List(Of Agro_Servizio)
    Property ListaThread As New List(Of Thread)

    Private Sub Crea_e_Avvia_Threads_Servizi()

        Dim Super_Server_Provider As String = ConfigurationManager.AppSettings("Super_Server_Provider")
        Dim Super_Server_Server As String = ConfigurationManager.AppSettings("Super_Server_Server")
        Dim Super_Server_DB As String = ConfigurationManager.AppSettings("Super_Server_DB")
        Dim Super_Server_UserId As String = ConfigurationManager.AppSettings("Super_Server_UserId")
        Dim Super_Server_Password As String = ConfigurationManager.AppSettings("Super_Server_Password")
        Dim Super_Server_PivaSuperUser As String = ConfigurationManager.AppSettings("Super_Server_PivaSuperUser")
        Dim Import_Username As String = "Agro_GSB"
        If Not IsNothing(ConfigurationManager.AppSettings("Utente_Username")) Then
            Import_Username = ConfigurationManager.AppSettings("Utente_Username")
        End If
        StringaConnessione_SuperServer = "Provider=" & Super_Server_Provider & ";Server=" & Super_Server_Server & ";Initial Catalog=" & Super_Server_DB & ";User Id=" & Super_Server_UserId & ";Password=" & Super_Server_Password & ";"


        Dim dtServizi As DataTable = Leggi_Elenco_ServiziTask()

        If Not IsNothing(dtServizi) Then

            'creo la lista di servizi, uno per ciascuna riga
            'Agro_Servizio_List = New List(Of Agro_Servizio)
            ListaThread = New List(Of Thread)

            For i = 0 To dtServizi.Rows.Count - 1


                'dovrei fare n servizi, per ora 1

                'Prendo i 4 valori della chiave e il periodo di polling
                Dim PivaSuperuser As String = dtServizi.Rows(i).Item("PivaSuperuser") 'chiave
                Dim Id_Servizio As enum_Id_Servizio = CInt(dtServizi.Rows(i).Item("Id_Servizio")) ' chiave costante
                Dim Tipo_Sincro As enum_Tipi_Servizi_Background = CInt(dtServizi.Rows(i).Item("Tipo_Sincro")) 'chiave
                Dim Tipo_Sincro_Nome As String = Tipo_Sincro.ToString()
                Dim Id_Riga As Integer = CInt(dtServizi.Rows(i).Item("Id_Riga")) 'chiave
                Dim ID_DB As Integer = CInt(dtServizi.Rows(i).Item("ID_DB")) 'id_db SuperServer connessioni
                Dim DirectoryLOG As String = dtServizi.Rows(i).Item("DirectoryLOG")

                Dim ID As String = Tipo_Sincro_Nome & "(" & PivaSuperuser & "-" & Id_Servizio & "-" & Tipo_Sincro & "-" & Id_Riga & "-" & ID_DB & ")"



                '-------------------------------------------------------------
                Dim objParametriSuperServer As AgronicaCoreParametri
                Dim objParametriServer As AgronicaCoreParametri
                Dim objParametriUtenti As AgronicaCoreParametri
                Dim objConn As New AgronicaCoreGestioneRichieste.Inizializzatore
                objConn.AvviamentoConSuperServerFast(Super_Server_Provider,
                                                     Super_Server_Server,
                                                     Super_Server_DB,
                                                     Super_Server_UserId,
                                                     Super_Server_Password,
                                                     DirectoryLOG,
                                                     "AgroGSB_Log.txt",
                                                     PivaSuperuser,
                                                     Import_Username,
                                                     ID_DB,
                                                     objParametriSuperServer,
                                                     objParametriServer,
                                                     objParametriUtenti)



                '-------------------------------------------------------------


                'Verifico che si sia un solo superuser (per ora ne gestisco uno)
                Dim utenti = New AgronicaCoreUtentiDAL.Utenti_Read()
                Dim dtSuperUser = utenti.Leggi_SuperUser("", objParametriUtenti.StringaConnessione)

                '----------------------TEMP---------------------------------------
                'per ora gestisce un solo superuser
                If dtSuperUser.Rows.Count <> 1 Then
                    Throw New Exception("per ora gestisce un solo superuser")
                End If
                '----------------------TEMP---------------------------------------


                'creo il servizio, uno si riferisce ad una riga di Configurazione_Servizi
                'quindi gli passo la chiave della tabella Configurazione_Servizi 
                'e le connessioni per leggere nel db la riga e il resto che serve.
                'per ora gestisce una sola riga, poi potrà essere un unico servizio per più importazioni configurabile
                Dim Agro_Servizio As New Agro_Servizio(PivaSuperuser, Id_Servizio, Tipo_Sincro, Id_Riga, ID_DB, objParametriSuperServer, objParametriServer, objParametriUtenti, LogAgroServizio)
                'Agro_Servizio_List.Add(Agro_Servizio)

                Dim ThreadServizio As Thread

                ThreadServizio = New Thread(AddressOf Agro_Servizio.Task_Da_Eseguire)

                AddHandler Agro_Servizio._EventoErrore, New EventoErroreServizioHandler(AddressOf ImpTh__EventoErrore)
                AddHandler Agro_Servizio._EventoImportazione, New EventoServizioHandler(AddressOf ImpTh__EventoImportazione)

                ListaThread.Add(ThreadServizio)

            Next

        Else

            ScriviLogServizio(LogAgroServizio, "Nessun Servizio In Configurazione_Servizi con i parametri impostati " & Nome_LogAgroServizio_8CaratteriMax & " ", EventLogEntryType.Warning, id_Evento:=ID_eventi.ErroreInAvvioServizio)
            Exit Sub

        End If

        'se tutto va bene lancio i threads
        If Not IsNothing(ListaThread) AndAlso ListaThread.Count > 0 Then

            For Each threadServizio As Thread In ListaThread

                threadServizio.Start()

                'Dopo 2 sec, se il thread non ha terminato la procedura di inizializzazione il chiamante
                'prosegue comunque l'esecuzione del programma
                threadServizio.Join(2000)

            Next

        Else

            ScriviLogServizio(LogAgroServizio, "Nessun Servizio In ListaThread " & Nome_LogAgroServizio_8CaratteriMax & " ", EventLogEntryType.Warning, id_Evento:=ID_eventi.ErroreInAvvioServizio)
            Exit Sub

        End If



    End Sub




    Private Function Leggi_Elenco_ServiziTask() As DataTable

        Dim dt As DataTable

        Try

            'fisso
            Dim filtroIdServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline

            'codifica password non codificate
            Dim Configurazione_Servizi_W As New AgronicaCoreVarieDAL.Configurazione_Servizi_W()
            Configurazione_Servizi_W.CodificaPasswordSmtpNonCodificati(StringaConnessione_SuperServer)

            'Leggo le righe dei servizi da avviare (per ora uno)
            Dim Configurazione_Servizi_R As New AgronicaCoreVarieDAL.Configurazione_Servizi_R()
            'se ho impostato nell'AppConfig Tipo_Sincro e idRiga allora filtrerò la tabella
            dt = Configurazione_Servizi_R.Leggi(Filtro_PivaSuperuser,
                                                filtroIdServizio,
                                                Filtro_Tipo_Sincro,
                                                Filtro_Id_Riga,
                                                True,
                                                StringaConnessione_SuperServer)

            '' ''----------------------TEMP---------------------------------------
            '' ''per ora gestisce una sola riga, poi potrà essere un unico servizio per più importazioni configurabile
            ' ''If dt.Rows.Count <> 1 Then
            ' ''    Throw New Exception("per ora gestisce un solo sincro ")
            ' ''End If
            '' ''----------------------TEMP---------------------------------------

            Return dt

        Catch ex As Exception

            Utility.ScriviLogServizio(LogAgroServizio, "AgroServizio: " & vbCrLf & _
                    Utility.estraiMessaggiEccezioni(ex), EventLogEntryType.Error)

        End Try

        Return Nothing

    End Function

#End Region

End Class
