Imports System.Data.OleDb
Imports System.Web
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Common
Imports System.Data.SqlClient
Imports AgronicaCoreDataProvider
Imports System.Text
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports System.Configuration
Imports System.Runtime.Caching

Public Class SqlDataProvider
    Inherits AgronicaCoreDataProvider.LogProviderEsteso
    Implements IDataProvider

    Private _byPassaLog As Boolean = False
    Private _sqlDiretto As Boolean = False
    Private _parametri As Dictionary(Of Int32, AgroDBParametro) = Nothing
    Private _orderByEFiltroAggiuntivo As OrderByFiltroAggiuntivo = Nothing

    Public Property ByPassaLog As Boolean Implements IDataProvider.ByPassaLog
        Get
            Return _byPassaLog
        End Get
        Set(value As Boolean)
            _byPassaLog = value
        End Set
    End Property

    Public Property SqlDiretto As Boolean Implements IDataProvider.SqlDiretto
        Get
            Return _sqlDiretto
        End Get
        Set(value As Boolean)
            _sqlDiretto = value
        End Set
    End Property
    Public Sub New()
    End Sub
    Public Sub New(ByVal parametri As Dictionary(Of Int32, AgroDBParametro))
        Me.New()
        _parametri = parametri
    End Sub

    Public Sub SettaOrderBYeFiltroAggiuntivo(ByVal orderByeFiltro As OrderByFiltroAggiuntivo)
        _orderByEFiltroAggiuntivo = orderByeFiltro
    End Sub

    <Obsolete("Usare il metodo VersioneSqlServer_Major")>
    Public Function VersioneSqlServer_anno(objParametri As AgronicaCoreParametri) As Integer Implements IDataProvider.VersioneSqlServer_anno

        Dim rVal As Integer = -1
        Try

            Dim dt As DataTable = EseguiQuery_Lettura(objParametri, "select @@Version as versione", "")
            Dim s As String = CStr(dt(0)(0))

            Dim s1 As String() = s.Split({" "c})

            For i As Integer = 0 To s1.Length - 1

                If s1(i).Length = 4 AndAlso IsNumeric(s1(i)) Then
                    rVal = CInt(s1(i))
                    Exit For
                End If
            Next

        Catch ex As Exception
        End Try

        Return rVal

    End Function

    Public Function VersioneSqlServer_Major(objParametri As AgronicaCoreParametri) As Integer Implements IDataProvider.VersioneSqlServer_Major

        'sostituisce il metodo VersioneSqlServer_anno, che non funzionava bene in condizioni di server molto carico https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
        Dim major As Integer = -1
        Try
            Dim dt As DataTable = EseguiQuery_Lettura(objParametri, "SELECT SERVERPROPERTY('ProductVersion') as Version", "")
            Dim version As String = CStr(dt(0)(0))
            Dim versionNumbers = version.Split("."c)
            major = CInt(versionNumbers(0))

        Catch ex As Exception
            Scrivi_LOG(objParametri, "VersioneSqlServer_Major", "Errore durante la lettura della versione sql (lettura major)")
        End Try

        Return major
    End Function


    '=============================================================================
    ' NOTA : FlagVisibilita
    '
    ' Con questo flag discriminiamo i record che vogliamo visualizzare
    ' in funzione del loro stato di CANCELLAZIONE LOGICA
    '
    ' 1 = Voglio vedere solo i record NON CANCELLATI    = INVIATO >= 0
    ' 2 = Voglio vedere solo i record CANCELLATI        = INVIATO = -1
    ' 3 = Voglio vedere TUTTI i record                  = nessun controllo su INVIATO
    '
    '=============================================================================

    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objConnessione As DbConnection,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String,
                                        Optional ByVal objTransazione As DbTransaction = Nothing
                                        ) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim strOriginale_xLOG As String = ""
        Dim parametrizzata As Boolean
        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin
            strOriginale_xLOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL

            parametrizzata = Parametrizza(StringaSQL, parameters)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione))
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objConnessione
            End If

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If
            StringaSQL = gestioneTransactionLevel(StringaSQL, objTransazione)
            xCommand.CommandText = StringaSQL

            If objTransazione IsNot Nothing Then
                xCommand.Transaction = objTransazione
            End If

            xDataAdapter = New SqlDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = xConnessione.State

            If xConnessione.State = ConnectionState.Closed Then
                xConnessione.Open()
            End If

            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale(NomeRoutine, StringaSQL, StringaSQL_ORIGINALE, e.Message, DammiStackTrace())

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(New AgronicaCoreParametri With {.objConnessione = objConnessione})
                'TODO FIX
                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then

                    'Torno a leggere con l'istruzione originale
                    xCommand.Parameters.Clear()
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    If xConnessione.State = ConnectionState.Closed Then
                        xConnessione.Open()
                    End If

                    xDataAdapter.Fill(dt)

                Else

                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", e.Message)
                    End If

                    Throw New Exception(messaggioErrore)
                End If

            End Try
        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If ConfigurazioneEstesaSqlProviderFactory.Instance(New AgronicaCoreParametri With {.objConnessione = objConnessione}).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               strOriginale_xLOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, Nothing)
            End If

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)
            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objConnessione As DbConnection,
                                        ByRef objTransazione As DbTransaction,
                                        ByVal StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String,
                                        ByVal NomeRoutine As String
                                        ) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim strOriginale_xLOG As String = ""
        Dim parametrizzata As Boolean

        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin
            strOriginale_xLOG = StringaSQL_ORIGINALE
            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL

            parametrizzata = Parametrizza(StringaSQL, parameters)

            'Se la connessione è chiusa la apro
            If objConnessione Is Nothing Then

                'Richiedo una connessione
                objConnessione = New SqlConnection

                objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione)
                objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objConnessione.CreateCommand()
            xCommand.CommandTimeout = 600

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            StringaSQL = gestioneTransactionLevel(StringaSQL, objTransazione)

            xCommand.CommandText = StringaSQL

            xDataAdapter = New SqlDataAdapter
            xDataAdapter.SelectCommand = xCommand
            xConnectionState = objConnessione.State

            If objConnessione.State = ConnectionState.Closed Then
                objConnessione.Open()
            End If

            If Not IsNothing(objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objTransazione
            End If

            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale(NomeRoutine, StringaSQL, StringaSQL_ORIGINALE, e.Message, DammiStackTrace())

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(New AgronicaCoreParametri With {.objConnessione = objConnessione})

                'TODO FIX
                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then
                    'Torno a leggere con l'istruzione originale
                    xCommand.Parameters.Clear()
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    If objConnessione.State = ConnectionState.Closed Then
                        objConnessione.Open()
                    End If

                    If Not IsNothing(objTransazione) Then
                        xDataAdapter.SelectCommand.Transaction = objTransazione
                    End If

                    xDataAdapter.Fill(dt)
                Else

                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                           "", e.Message)
                    End If

                    Throw New Exception(messaggioErrore)
                End If
            End Try
        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If ConfigurazioneEstesaSqlProviderFactory.Instance(New AgronicaCoreParametri With {.objConnessione = objConnessione}).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               strOriginale_xLOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, Nothing)
            End If


            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)
            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            ' se ho creato la connessione locale, non ho creato nessuna transazione
            ' la transazione viene usata solo se esiste già
            If FlagConnessioneLocale Then
                If Not IsNothing(objConnessione) Then
                    objConnessione.Close()
                    objConnessione.Dispose()
                    objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(objConnessione) Then
                        objConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura_XML(ByRef objParametri As AgronicaCoreParametri,
                                            ByVal StringaSQL As String,
                                            ByVal NomeRoutine As String
                                            ) As String Implements IDataProvider.EseguiQuery_Lettura_XML

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura_xml"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim strXMLOut As String = ""
        Dim conn As New SqlConnection
        conn.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            Dim cmd As New SqlCommand()
            cmd.Connection = conn

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    cmd.Parameters.Add(p)
                Next
            End If

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 1200
            cmd.CommandText = StringaSQL
            cmd.CommandTimeout = objParametri.TimeoutQuery

            Dim rdr As Xml.XmlReader
            Dim xmlDoc As New Xml.XmlDocument()

            Try
                rdr = cmd.ExecuteXmlReader

                xmlDoc.Load(rdr)
                strXMLOut = xmlDoc.OuterXml

            Catch e As Exception

                'MessaggioErrore = e.Message & " " & StringaSQL
                'Scrivi_LOG(objParametri.LogDirectory, objParametri.LogFileName, objParametri.LogDescrizioneUtente, NomeRoutine & "." & NomeRoutineLocale, MessaggioErrore)

                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, e.Message, DammiStackTrace(), objParametri)

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)

                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then

                    'Torno a leggere con l'istruzione originale
                    cmd.Parameters.Clear()
                    rdr.Close()
                    cmd.CommandText = StringaSQL_ORIGINALE
                    rdr = cmd.ExecuteXmlReader

                    xmlDoc.Load(rdr)
                    strXMLOut = xmlDoc.OuterXml

                    conn.Close()
                Else
                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", e.Message)
                    End If

                    Throw New Exception(messaggioErrore)

                End If
            End Try

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            conn.Close()

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, objParametri)
            End If

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)
            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally


        End Try

        Return strXMLOut

    End Function



    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                            Optional chiamaScriviLog As Boolean = True
                                        ) As DataTable Implements IDataProvider.EseguiQuery_Lettura


        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        'objParametri.StringaConnessione = objParametri.StringaConnessione + ";Pooling=false;OLE DB Services=-4;"

        Dim dt As New DataTable

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New SqlConnection

                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objParametri.objConnessione.CreateCommand()
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                    If Debugger.IsAttached Then
                        Try
                            Debug.Print("DECLARE " & p.ParameterName & " as " & [Enum].GetName(GetType(System.Data.SqlDbType), p.SqlDbType) & "(" & p.Size.ToString & ")")
                            Debug.Print("SET " & p.ParameterName & "= '" & p.Value.ToString() & "'" & vbCrLf)
                        Catch ex As Exception
                        End Try
                    End If
                Next
            End If

            StringaSQL = gestioneTransactionLevel(StringaSQL, objParametri.objTransazione)

            xCommand.CommandText = StringaSQL

            xDataAdapter = New SqlDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If


            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                Dim stringaSqlConParametri = Componi_MessaggioErrore_Con_Parametri(StringaSQL, xCommand.Parameters)
                Dim stringaStackTrace = DammiStackTrace()

                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   stringaSqlConParametri, e.Message, stringaStackTrace)

                If chiamaScriviLog Then
                    LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, e.Message, stringaStackTrace, objParametri)
                End If

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)

                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then

                    'Torno a leggere con l'istruzione originale
                    xCommand.Parameters.Clear()
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    xConnectionState = objParametri.objConnessione.State

                    If objParametri.objConnessione.State = ConnectionState.Closed Then
                        objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                        objParametri.objConnessione.Open()
                    End If

                    If Not IsNothing(objParametri.objTransazione) Then
                        xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                    End If

                    xDataAdapter.Fill(dt)

                Else

                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", e.Message)
                    End If

                    dt = Nothing
                    Throw New Exception(messaggioErrore)

                End If

            End Try
        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If chiamaScriviLog AndAlso ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, objParametri)
            End If

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)

            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            ' se ho creato la connessione locale, non ho creato nessuna transazione
            ' la transazione viene usata solo se esiste già
            If FlagConnessioneLocale Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()

                    '' WARNING !!!!!
                    'OleDbConnection.ReleaseObjectPool()
                    '' WARNING !!!!!

                    objParametri.objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    objParametri.objConnessione.Close()
                End If
            End If

        End Try

        Return dt

    End Function


    Private Function gestioneTransactionLevel(ByVal query As String, objTransazione As DbTransaction) As String
        Dim gestioneReadUncommitted = True
        If Not IsNothing(ConfigurationManager.AppSettings("ReadUncommittedDefault")) AndAlso Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("ReadUncommittedDefault").ToString()) Then
            If ConfigurationManager.AppSettings("ReadUncommittedDefault").ToString = "False" Then
                gestioneReadUncommitted = False
            End If
        End If

        If objTransazione Is Nothing AndAlso Not query.ToUpper.Contains("SET TRANSACTION ISOLATION LEVEL") AndAlso gestioneReadUncommitted Then
            query = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" & vbCrLf & query
        End If
        Return query
    End Function

    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        StringaSQL As String,
                                        parametri As Dictionary(Of String, Object),
                                        NomeRoutine As String) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing
        Dim dt As New DataTable

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty

        Try


            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New SqlConnection

                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objParametri.objConnessione.CreateCommand()

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 1200
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            Dim StringaSQL_ORIGINALE = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            StringaSQL = gestioneTransactionLevel(StringaSQL, objParametri.objTransazione)

            xCommand.CommandText = StringaSQL

            ' Pulire sempre la collection dei command parameters
            xCommand.Parameters.Clear()
            For Each kvp As KeyValuePair(Of String, Object) In parametri
                xCommand.Parameters.AddWithValue(kvp.Key, kvp.Value)
            Next

            xDataAdapter = New SqlDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If

            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, e.Message, DammiStackTrace, objParametri)

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)

                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then

                    'Torno a leggere con l'istruzione originale
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    xConnectionState = objParametri.objConnessione.State

                    If objParametri.objConnessione.State = ConnectionState.Closed Then
                        objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                        objParametri.objConnessione.Open()
                    End If

                    If Not IsNothing(objParametri.objTransazione) Then
                        xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                    End If

                    xDataAdapter.Fill(dt)
                Else

                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", e.Message)
                    End If

                    dt = Nothing
                    Throw New Exception(messaggioErrore)
                End If
            End Try
        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, objParametri)
            End If

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)

            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            ' se ho creato la connessione locale, non ho creato nessuna transazione
            ' la transazione viene usata solo se esiste già
            If FlagConnessioneLocale Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()

                    '' WARNING !!!!!
                    'OleDbConnection.ReleaseObjectPool()
                    '' WARNING !!!!!

                    objParametri.objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    objParametri.objConnessione.Close()
                End If
            End If

        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                        ByRef DataSet2Fill As DataSet,
                                        ByVal strNomeDtNelDS As String
                                        ) As Boolean Implements IDataProvider.EseguiQuery_Lettura

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim risp As Boolean = False

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean
        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New SqlConnection

                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objParametri.objConnessione.CreateCommand()
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            StringaSQL = gestioneTransactionLevel(StringaSQL, objParametri.objTransazione)

            xCommand.CommandText = StringaSQL

            xDataAdapter = New SqlDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If

            Try
                xDataAdapter.Fill(DataSet2Fill, strNomeDtNelDS)
            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, e.Message, DammiStackTrace(), objParametri)

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)

                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then


                    'Torno a leggere con l'istruzione originale
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    xConnectionState = objParametri.objConnessione.State

                    If objParametri.objConnessione.State = ConnectionState.Closed Then
                        objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                        objParametri.objConnessione.Open()
                    End If

                    If Not IsNothing(objParametri.objTransazione) Then
                        xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                    End If

                    xDataAdapter.Fill(DataSet2Fill, strNomeDtNelDS)
                Else

                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", e.Message)
                    End If

                    risp = False
                    Throw New Exception(messaggioErrore)
                End If
            End Try

            risp = True

        Catch ex As ParametrizzatoreException
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, objParametri)
            End If
            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", ex.Message)
            End If

            risp = False
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            ' se ho creato la connessione locale, non ho creato nessuna transazione
            ' la transazione viene usata solo se esiste già
            If FlagConnessioneLocale Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()
                    objParametri.objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    objParametri.objConnessione.Close()
                End If
            End If

        End Try

        Return risp

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        'OK

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable
        Dim strOriginale_xLOG As String = ""
        Dim parametrizzata As Boolean

        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin
            strOriginale_xLOG = StringaSQL_ORIGINALE

            parametrizzata = Parametrizza(StringaSQL, parameters)

            'Richiedo una connessione
            xConnessione = New SqlConnection

            xConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione)
            xConnessione.Open()

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600

            xDataAdapter = New SqlDataAdapter()
            xDataAdapter.ReturnProviderSpecificTypes = False
            xDataAdapter.FillLoadOption = LoadOption.OverwriteChanges
            xDataAdapter.SelectCommand = xCommand

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            'StringaSQL = gestioneTransactionLevel(StringaSQL, objTransazione)

            xCommand.CommandText = StringaSQL

            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale(NomeRoutine, StringaSQL, StringaSQL_ORIGINALE, e.Message, DammiStackTrace())

                'TODO FIX
                If ConfigurazioneEstesaSqlProviderFactory.Instance(New AgronicaCoreParametri With {.StringaConnessione = StringaConnessione}).SQL_EseguiQueryOriginale Then

                    'Torno a leggere con l'istruzione originale
                    xCommand.Parameters.Clear()
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    xDataAdapter.Fill(dt)

                Else
                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                           "", e.Message)

                    End If

                    dt = Nothing
                End If

            End Try
        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())
            Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(New AgronicaCoreParametri With {.StringaConnessione = StringaConnessione})

            If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               strOriginale_xLOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, Nothing)
            End If

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)

            End If

            dt = Nothing

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                xConnessione.Close()
                xConnessione.Dispose()
                xConnessione = Nothing
            End If


        End Try

        Return dt

    End Function

    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, ByVal parametri As List(Of DbParameter), NomeRoutine As String) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New SqlConnection

                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objParametri.objConnessione.CreateCommand()

            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametri IsNot Nothing AndAlso parametri.Any Then
                For Each p As DbParameter In parametri
                    xCommand.Parameters.Add(p)
                Next
            End If

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            StringaSQL = gestioneTransactionLevel(StringaSQL, objParametri.objTransazione)

            xCommand.CommandText = StringaSQL

            xDataAdapter = New SqlDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If


            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, e.Message, DammiStackTrace(), objParametri)

                Dim cfg = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)

                If (If(cfg IsNot Nothing, cfg.SQL_EseguiQueryOriginale, CBool(ConfigurationManager.AppSettings("SQL_EseguiQueryOriginale")))) Then

                    'Torno a leggere con l'istruzione originale
                    xCommand.Parameters.Clear()
                    xCommand.CommandText = StringaSQL_ORIGINALE

                    xDataAdapter = New SqlDataAdapter
                    xDataAdapter.SelectCommand = xCommand

                    xConnectionState = objParametri.objConnessione.State

                    If objParametri.objConnessione.State = ConnectionState.Closed Then
                        objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                        objParametri.objConnessione.Open()
                    End If

                    If Not IsNothing(objParametri.objTransazione) Then
                        xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                    End If

                    xDataAdapter.Fill(dt)
                Else
                    If Not Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", e.Message)
                    End If

                    dt = Nothing
                    Throw New Exception(messaggioErrore)
                End If
            End Try
        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            If ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri).SQL_EseguiQueryOriginale Then
                'Scriviamo nel log anche la query senza parametri 
                Dim log_StrOriginale As String = String.Format("---> 3) ESEGUITA QUERY SENZA PARAMETRI (SQL_EseguiQueryOriginale = true) : {0}" & vbCrLf &
                                                               "ERRORE: {1}" & vbCrLf &
                                                               "METODO CHIAMANTE: {2}",
                                                               StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace())

                Logga(NomeRoutine, log_StrOriginale, objParametri)
            End If

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)

            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            ' se ho creato la connessione locale, non ho creato nessuna transazione
            ' la transazione viene usata solo se esiste già
            If FlagConnessioneLocale Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()
                    objParametri.objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    objParametri.objConnessione.Close()
                End If
            End If

        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Scrittura(ByRef objConnessione As DbConnection,
                                          ByRef objTransazione As DbTransaction,
                                          ByVal StringaConnessione As String,
                                          ByVal StringaSQL As String,
                                          ByVal DirectoryLOG As String,
                                          ByVal FileLOG As String,
                                          ByVal IdentificatoreUtente As String,
                                          ByVal NomeRoutine As String
                                          ) As Boolean Implements IDataProvider.EseguiQuery_Scrittura

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False
        Dim parametrizzata As Boolean

        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin
            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            parametrizzata = Parametrizza(StringaSQL, parameters)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione))

                'Apro la connessione
                xConnessione.Open()

                'Inizializzo a nothing la transazione
                xTransazione = Nothing

            Else
                'Utilizzo quella passata come parametro
                xConnessione = objConnessione

                xConnectionState = xConnessione.State

                If xConnessione.State = ConnectionState.Closed Then
                    xConnessione.Open()
                End If

                xTransazione = objTransazione

            End If

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione

            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale(NomeRoutine, StringaSQL, StringaSQL.ToOrigin, ex.Message, DammiStackTrace())

            dt = Nothing
            Throw ex
            'TODO Param Exception Check
        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function


    '##############################################################################################
    'fatto per Apofruit e scrivere su Siagr, non testato sul Gias o altro
    Public Function EseguiQuery_Scrittura(ByRef stringaConnessione As String,
                                          ByVal StringaSQL As String,
                                          ByVal NomeRoutine As String
                                          ) As Boolean Implements IDataProvider.EseguiQuery_Scrittura

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            ''Verifico se e' stata impostata una connessione
            'If IsNothing(objParametri.objConnessione) Then
            'Flag
            FlagConnessioneLocale = True
            xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(stringaConnessione))
            xConnessione.Open()
            xTransazione = Nothing

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600
            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione

            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale(NomeRoutine, StringaSQL, StringaSQL.ToOrigin, ex.Message, DammiStackTrace())

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)

            End If

            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    xConnessione.Close()
                End If
            End If

        End Try

        Return risp

    End Function



    '##############################################################################################
    Public Function EseguiQuery_ScritturaNum(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                        ByRef NumeroRecordInteressati As Integer) As Boolean Implements IDataProvider.EseguiQuery_ScritturaNum

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing
        NumeroRecordInteressati = 0
        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False
        Dim parametrizzata As Boolean

        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione))

                'Apro la connessione
                xConnessione.Open()

                'Inizializzo a nothing la transazione
                xTransazione = Nothing

            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione

                xConnectionState = xConnessione.State

                If xConnessione.State = ConnectionState.Closed Then
                    xConnessione.Open()
                End If

                xTransazione = objParametri.objTransazione

            End If


            xCommand = xConnessione.CreateCommand()

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione
            NumeroRecordInteressati = xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale(NomeRoutine, StringaSQL, StringaSQL.ToOrigin, ex.Message, DammiStackTrace())

            dt = Nothing
            Throw ex
            'TODO Param Exception Check
        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function


    Public Function EseguiQuery_Scrittura(ByRef objParametri As AgronicaCoreParametri,
                                          ByVal StringaSQL As String,
                                          ByVal NomeRoutine As String
                                          ) As Boolean Implements IDataProvider.EseguiQuery_Scrittura

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try

            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione))

                'Apro la connessione
                xConnessione.Open()

                'Inizializzo a nothing la transazione
                xTransazione = Nothing

            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione

                xConnectionState = xConnessione.State

                If xConnessione.State = ConnectionState.Closed Then
                    xConnessione.Open()
                End If

                xTransazione = objParametri.objTransazione

            End If


            xCommand = xConnessione.CreateCommand()

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex

        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace(), objParametri)

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}", "", ex.Message)
            End If

            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function


    Public Function NomeDataBase_FromConnessione_ATTENZIONE_NON_USARE_PER_COMPATIBILITA_SUPERSERVER(ByVal strPathxFileIni As String,
                                                       ByVal strKey As String,
                                                       Optional ByVal strNomeAttributoDB As String = "DbParam") _
                                                       As String Implements IDataProvider.NomeDataBase_FromConnessione_ATTENZIONE_NON_USARE_PER_COMPATIBILITA_SUPERSERVER

        Dim strNomeDB As String = ""

        'se strKey è numerico allora cerco sul superserver altrimenti su connessioni.ini
        If IsNumeric(strKey) Then
            Try


                Dim connessioni As New AgronicaCoreDataProvider.Connessioni
                If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                    'non dovrebbe mai essere nothing
                    'per il giasonline, se si usa la versione con superserver viene creato all'inizio nella pagina default
                    'nei passaggi tra i vari siti dovrebbe essere creato sempre all'ingresso
                    'se specificato che si usa la versione nuova
                    'Dim Inizializzazione As New Inizializzazione
                    'Inizializzazione.Crea_objParametri_Super_Server_Da_Config_GiasOnLine(HttpContext.Current.Session)
                    'se fallisce lo creo ????
                    ''If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                    ''    Inizializzazione.Crea_objParametri_Super_Server_Da_Config_GiasOnLine(HttpContext.Current.Session)
                    ''End If
                End If
                If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                    Throw New Exception("Non si è riuscito a creare objParametri_Super_Server e la stringa connessione non è pertanto presente")
                End If
                Dim dt As DataTable = connessioni.Leggi(CInt(strKey), TipiEnumerativi.enum_Tipo_DB.TUTTI, "", "", "", "", "", "", "", 0, "",
                AGRODATAINIZIO,
               AGRODATAFINE,
                "", "", CType(HttpContext.Current.Session("ASG_objParametri_Super_Server"), AgronicaCoreParametri))

                If dt.Rows.Count <> 1 Then
                    Throw New Exception("Doveva esserci almeno un record per il db")
                End If
                strNomeDB = CStr(dt.Rows(0).Item("DB"))
                Return strNomeDB


            Catch ex As Exception
                Throw New Exception(ex.Message.ToString) 'rilancio l'eccezione
            End Try

        Else


            'versione con file ini

            Dim fs As New IO.StreamReader(strPathxFileIni)

            Try

                Dim strLine As String = fs.ReadLine
                While Not IsNothing(strLine) 'cioè finché c'è qualcosa nel file cicla

                    If strLine = "[" & strKey.Trim & "]" Then 'cicla fino a quando non trova la chiave
                        'leggo la riga successiva..
                        strLine = fs.ReadLine 'leggo fino a quando non trovo il ; o la stringa vuota..
                        While Not strLine.StartsWith(";") OrElse strLine = ""

                            If strLine.StartsWith(strNomeAttributoDB) Then
                                'prendo la parte seguente il primo = ..
                                strNomeDB = strLine.Split({"="c})(2).Replace("""", "")
                                Exit While
                            End If

                            strLine = fs.ReadLine

                        End While

                        Exit While

                    End If

                    strLine = fs.ReadLine

                End While

                fs.Close() 'chiudo lo stream

                Return strNomeDB

            Catch exc As Exception
                fs.Close() 'chiudo lo stream
                Throw New Exception(exc.Message.ToString) 'rilancio l'eccezione
            End Try

        End If

    End Function

    Public Function NomeDataBase_FromStringaConnessione(ByVal StringaConnessione As String) _
                                                   As String Implements IDataProvider.NomeDataBase_FromStringaConnessione



        'esempio: "Provider=SQLOLEDB;Server=*****;Initial Catalog=*****_Utenti;User Id=*****;Password=*****;"
        Dim r As String = StringaConnessione.Split({";"c})(2)
        Dim s As String = r.Split({"="c})(1)
        Return s.Trim()

    End Function


    'ricerca la connessione nel file ini  o nel superserver  in base alla k passata (se intero allora su superserver)
    Public Function FindConnessione_Su_Ini_O_Superserver(
                ByVal strPath2Ini As String,
                ByVal strKey As String,
                Optional objParametriSuperServer As AgronicaCoreParametri = Nothing) As String Implements IDataProvider.FindConnessione_Su_Ini_O_Superserver

        Dim strConString As String = ""
        Dim connessioni As New AgronicaCoreDataProvider.Connessioni

        Try
            'se strKey è numerico allora cerco sul superserver altrimenti su connessioni.ini

            'funziona solo se si ha ASG_objParametri_Super_Server,
            'quindi non in ComunicazioneGiasOnLine_Satelliti che ancora non ha letto i parametri dall'xml di passaggio
            'quindi in quel caso è gestito passando nella querystring la stringa connessione superserver
            If IsNumeric(strKey) Then
                If objParametriSuperServer Is Nothing Then 'se non mi è stato passato il superserver...

                    If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                        'non dovrebbe mai essere nothing
                        'per il giasonline, se si usa la versione con superserver viene creato all'inizio nella pagina default
                        'nei passaggi tra i vari siti dovrebbe essere creato sempre all'ingresso
                        'se specificato che si usa la versione nuova
                        'Dim Inizializzazione As New Inizializzazione
                        'Inizializzazione.Crea_objParametri_Super_Server_Da_Config_GiasOnLine(HttpContext.Current.Session)
                        'se fallisce lo creo ????
                        ''If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                        ''    Inizializzazione.Crea_objParametri_Super_Server_Da_Config_GiasOnLine(HttpContext.Current.Session)
                        ''End If
                    End If

                    objParametriSuperServer = CType(HttpContext.Current.Session("ASG_objParametri_Super_Server"), AgronicaCoreParametri)

                    'NOTA!!!!
                    '02/12/2015
                    'il giaslan, per la connessione server passa strKey numerico ma Session("ASG_objParametri_Super_Server") è nothing,
                    'quindi tutte le volte spara eccezione che poi cattura subito dopo
                    'sostituisco con exit function
                    If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                        'Throw New Exception("Non si è riuscito a creare objParametri_Super_Server e la stringa connessione non è pertanto presente")
                        Return ""
                    End If
                End If
                'strConString = connessioni.Leggi_Stringa_Connessione(CInt(strKey), objParametriSuperServer)
                strConString = Sicurezza.Leggi_Stringa_Connessione(CInt(strKey), objParametriSuperServer)

            Else
                strConString = FindIniConnessioni(strPath2Ini, strKey)
            End If

        Catch exc As Exception
            strConString = ""
            'Throw New Exception(exc.Message) 'rilancio l'eccezione


            'uso questa funzione per ottenere il Messaggio..:
            Dim messaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(exc, True, source:=True)


            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[FindConnessione_Su_Ini_O_Superserver()] : " & messaggioErrore, exc)



        End Try

        Return strConString

    End Function

    'ricerca nel file ini la chiave x la connessione in base alla k passata
    Public Function FindIniConnessioni(ByVal strPath2Ini As String, ByVal strKey As String) As String Implements IDataProvider.FindIniConnessioni

        Dim fs As New IO.StreamReader(strPath2Ini)
        Dim strConString As String = ""
        Dim strParUsr As String = ""
        Dim strUsr As String = ""
        Dim strParPwd As String = ""
        Dim strPwd As String = ""

        Try

            Dim strLine As String = fs.ReadLine
            While Not IsNothing(strLine) 'cioè finché c'è qualcosa nel file cicla

                If strLine = "[" & strKey.Trim & "]" Then 'cicla fino a quando non trova la chiave
                    'leggo la riga successiva
                    strLine = fs.ReadLine 'leggo fino a quando non trovo il ; o la stringa vuota
                    While Not IsNothing(strLine) AndAlso (Not strLine.StartsWith(";") OrElse strLine = "")

                        If strLine.StartsWith("ProviderParam") OrElse strLine.StartsWith("ServerParam") OrElse
                            strLine.StartsWith("DbParam") Then
                            'prendo la parte seguente il primo = ..
                            strConString &= strLine.Substring(strLine.IndexOf("=") + 1).Replace("""", "") & ";"
                        ElseIf strLine.StartsWith("UserIdParam") Then
                            strParUsr = strLine.Substring(strLine.IndexOf("=") + 1) & "="
                        ElseIf strLine.StartsWith("PasswordParam") Then
                            strParPwd = strLine.Substring(strLine.IndexOf("=") + 1) & "="
                        ElseIf strLine.StartsWith("UserId") Then
                            strUsr = strLine.Substring(strLine.IndexOf("=") + 1)
                        ElseIf strLine.StartsWith("Password") Then
                            strPwd = strLine.Substring(strLine.IndexOf("=") + 1)

                        End If
0:
                        strLine = fs.ReadLine

                    End While

                    Exit While

                End If

                strLine = fs.ReadLine

            End While

            fs.Close() 'chiudo lo stream

            'assegno la stringa di connessione all'oggetto globale della classe 
            strConString = strConString & strParUsr & strUsr & ";" & strParPwd & strPwd & ";"

        Catch exc As Exception

            strConString = ""

            fs.Close() 'chiudo lo stream
            'Throw New Exception(exc.Message) 'rilancio l'eccezione


            'uso questa funzione per ottenere il Messaggio..:
            Dim messaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(exc, True, source:=True)

            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception(messaggioErrore, exc)



        End Try

        Return strConString

    End Function


    ' apre la connessione
    Public Function ApriConnessione(ByRef objParametri As AgronicaCoreParametri, ByRef strErr As String) _
                                    As DbConnection Implements IDataProvider.ApriConnessione

        Dim Connessione As New SqlConnection

        Try
            'cerco la stringa di connessione dal file .ini
            'Dim strConnessione As String = FindConnessione_Su_Ini_O_Superserver()
            Dim strConnessione As String = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)

            Connessione.ConnectionString = strConnessione

            'apro la connessione
            Connessione.Open()

            Return Connessione

        Catch exc As Exception 'ritorno l'eccezione...
            'se la connessione è aperta la chiudo..
            If Connessione.State = ConnectionState.Open Then
                Connessione.Close()
            End If
            strErr = exc.Message.ToString
            Return Nothing

        End Try

    End Function

    '###############################################################################
    Public Function EseguiQuery_InsertParametrizzata(ByRef objParametri As AgronicaCoreParametri,
                                               ByVal Nome_Tabella As String,
                                               ByVal strCampi As String,
                                               ByVal strValori As String) As Boolean Implements IDataProvider.EseguiQuery_InsertParametrizzata



        '----- Descrizione
        Dim DescrizioneFunzione As String = "AgronicaCoreDataProvider.DataProvider.EseguiQuery_InsertParametrizzata()"
        Dim StrSQL As String
        Dim res As Boolean

        StrSQL = String.Format(" INSERT INTO {0} ({1}) VALUES ({2} ) ", Nome_Tabella, strCampi, strValori)


        'Effettuo l'inserimento
        res = EseguiQuery_Scrittura(objParametri, StrSQL, DescrizioneFunzione)

        Return res

    End Function


    '###########################################################################################################
    Public Function ConnectToAccess(ByVal DataSource As String) As DbConnection Implements IDataProvider.ConnectToAccess

        Dim conn As New OleDb.OleDbConnection

        conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & DataSource & ";User Id=admin;Password=;"

        Try
            'Apre la connessione al DB
            conn.Open()

        Catch ex As Exception
            Console.Write("Failed to connect to data source: " & ex.Message)
        End Try

        Return conn

    End Function

    '#################################################################################################
    Public Function Scrivi_Dati_SuAccess(ByVal DataSource As String,
                                         ByVal StrSQL As String,
                                         ByRef strErr As String
                                         ) As Object Implements IDataProvider.Scrivi_Dati_SuAccess

        Dim sqlCommand As OleDb.OleDbCommand
        Dim IntDummy As Integer = 0

        Try

            Dim Connessione As New OleDb.OleDbConnection
            Connessione = ConnectToAccess(DataSource)

            sqlCommand = Connessione.CreateCommand()
            'imposto il timeout a 10 minuti
            sqlCommand.CommandTimeout = 600
            sqlCommand.CommandText = StrSQL

            IntDummy = sqlCommand.ExecuteNonQuery

            sqlCommand.Dispose()
            Connessione.Close()

            Return IntDummy

        Catch exc As Exception 'incaso di errore...
            strErr = exc.Message.ToString 'salvo il msg di errore nella stringa
            Return 0
        End Try


    End Function

    Protected Friend Function Parametrizza(ByRef StringaSql As String,
                                           ByRef parametriOutput As List(Of SqlParameter),
                                            Optional objParametri_Server As AgronicaCoreParametri = Nothing) As Boolean

        Dim retVal As Boolean = False
        Dim nomeRoutine As String = "AgronicaCoreDataProvider.SqlDataProvider.Parametrizza"

        Dim StringaSQL_ORIGINALE = StringaSql.ToOrigin(objParametri_Server)

        If SqlDiretto Then
            StringaSql = StringaSQL_ORIGINALE
            Return False
        End If

        'Dim p As New Parametrizzatore(DataProviderFactory.Instance.ConfigrurazioneLogProvider)
        Dim p As IParametrizzatore = DataProviderFactory.Instance.Parametrizzatore(DataProviderFactory.Instance.ConfigrurazioneLogProvider)
        p.ByPassaLog = False
        parametriOutput = New List(Of SqlParameter)

        Try
            retVal = p.Parametrizza(StringaSql, _parametri, _orderByEFiltroAggiuntivo, parametriOutput, objParametri_Server)
        Catch ex As ParametrizzatoreException
            Throw ex
        Catch ex As Exception
            Dim sb = New StringBuilder()
            sb.Append(String.Format("---> ECCEZIONE DURANTE LA PARAMETRIZZAZIONE DELLA QUERY : {0}", StringaSql))
            sb.AppendLine()
            sb.Append(String.Format("Dettagli: {0}", ex.Message))

            If Not Debugger.IsAttached Then
                p.Logga(nomeRoutine, sb.ToString, objParametri_Server)

                StringaSql = StringaSQL_ORIGINALE
            End If

        End Try

        Return retVal

    End Function

    '#################################################################################################
    Private Function Traduci(ByRef StringaSql As String, ByVal lingua As Integer, ByRef strErr As String) As Boolean

        If lingua > 1 Then

            Try

                Dim CodiceISO = "it"

                Select Case lingua
                    Case 2
                        CodiceISO = "en"
                    Case 3
                        CodiceISO = "fr"
                    Case 4
                        CodiceISO = "IT-ch"
                    Case 5
                        CodiceISO = "pt"

                End Select

                If CodiceISO <> "it" Then

                    '11/09/2024 Giulia: se ci sono delle query dove c'è NomeTabella e poi subito a capo questa non viene riconosciuta e sostituita
                    'Per evitare questo problema, se trovo un "a capo" ci aggiungo davanti uno spazio per sicurezza,
                    'in questo modo non devo toccare la parte sotto, visto che molte sostituzioni si basano sullo spazio finale
                    StringaSql = StringaSql.Replace(vbCrLf, " " & vbCrLf & " ")

                    '01/2025 Gianluca A: Sostituisco il tab (\t) con quattro spazi
                    StringaSql = StringaSql.Replace("	", "    ")

                    Dim tab_da_tradurre As Dictionary(Of String, String) = PopolaDizionarioTabelleInLingua(CodiceISO)

                    Dim w_key = ""
                    Dim w_value = ""
                    For Each item In tab_da_tradurre
                        w_key = item.Key
                        w_value = item.Value

                        '    " tabella " --> " tabella_XLingue_xx "
                        StringaSql = StringaSql.ReplaceCaseInsensitive(" " & w_key & " ", " " & w_value & " ")
                        '    " dbo.tabella " --> " tabella_XLingue_xx "
                        StringaSql = StringaSql.ReplaceCaseInsensitive(" dbo." & w_key & " ", " " & w_value & " ")
                        '    " tabella." --> " tabella_XLingue_xx."
                        StringaSql = StringaSql.ReplaceCaseInsensitive(" " & w_key & ".", " " & w_value & ".")
                        '    " dbo.tabella." --> " tabella_XLingue_xx."
                        StringaSql = StringaSql.ReplaceCaseInsensitive(" dbo." & w_key & ".", " " & w_value & ".")
                        '    "(tabella." --> "(tabella_XLingue_xx."
                        StringaSql = StringaSql.ReplaceCaseInsensitive("(" & w_key & ".", "(" & w_value & ".")
                        '    " tabella," --> " tabella_XLingue_xx,"
                        StringaSql = StringaSql.ReplaceCaseInsensitive(" " & w_key & ",", " " & w_value & ",")
                        '    "(tabella," --> "(tabella_XLingue_xx,"
                        StringaSql = StringaSql.ReplaceCaseInsensitive("(" & w_key & ",", "(" & w_value & ",")
                        '    ",tabella " --> ",tabella_XLingue_xx "
                        StringaSql = StringaSql.ReplaceCaseInsensitive("," & w_key & " ", "," & w_value & " ")
                        '    ",tabella," --> ",tabella_XLingue_xx,"
                        StringaSql = StringaSql.ReplaceCaseInsensitive("," & w_key & ",", "," & w_value & ",")
                        '    ",tabella." --> ",tabella_XLingue_xx."
                        StringaSql = StringaSql.ReplaceCaseInsensitive("," & w_key & ".", "," & w_value & ".")
                        '    "*tabella." --> "*tabella_XLingue_xx."
                        StringaSql = StringaSql.ReplaceCaseInsensitive("*" & w_key & ".", "*" & w_value & ".")
                        '    "=tabella." --> "=tabella_XLingue_xx."
                        StringaSql = StringaSql.ReplaceCaseInsensitive("=" & w_key & ".", "=" & w_value & ".")

                    Next

                    'Queste vengono fatte per risolvere il caso in cui ci fossero nomi di tabella scritti fra []
                    StringaSql = StringaSql.Replace("[[", "[")
                    StringaSql = StringaSql.Replace("]]", "]")

                    'FunzioniSqlCaseSentive(StringaSql)

                    Return True

                Else
                    Return False
                End If

            Catch ex As Exception
                strErr = ex.Message.ToString
                Return False
            End Try

        End If

        Return False

    End Function

    Private Sub FunzioniSqlCaseSentive(ByRef StringaSql As String)

        Dim listaFunzioni As New List(Of String)

        listaFunzioni.Add("STArea")
        listaFunzioni.Add("STAsBinary")
        listaFunzioni.Add("STAsText")
        listaFunzioni.Add("STBuffer")
        listaFunzioni.Add("STCurveN")
        listaFunzioni.Add("STCurveToLine")
        listaFunzioni.Add("STDifference")
        listaFunzioni.Add("STDimension")
        listaFunzioni.Add("STDisjoint")
        listaFunzioni.Add("STDistance")
        listaFunzioni.Add("STEndpoint")
        listaFunzioni.Add("STEquals")
        listaFunzioni.Add("STGeometryN")
        listaFunzioni.Add("STGeometryType")
        listaFunzioni.Add("STIntersection")
        listaFunzioni.Add("STIntersects")
        listaFunzioni.Add("STIsClosed")
        listaFunzioni.Add("STIsEmpty")
        listaFunzioni.Add("STIsValid")
        listaFunzioni.Add("STLength")
        listaFunzioni.Add("STNumCurves")
        listaFunzioni.Add("STNumGeometries")
        listaFunzioni.Add("STNumPoints")
        listaFunzioni.Add("STPointN")
        listaFunzioni.Add("STSrid")
        listaFunzioni.Add("STStartPoint")
        listaFunzioni.Add("STSymDifference")
        listaFunzioni.Add("STUnion")

        listaFunzioni.Add("EnvelopeCenter")


        For Each ff As String In listaFunzioni
            StringaSql = StringaSql.ReplaceCaseInsensitive(ff.ToLower, ff)
        Next



    End Sub

    Private Function Componi_MessaggioErrore_Con_Parametri(ByVal stringaSql As String, ByVal collectionParametri As SqlParameterCollection) As String

        Dim sbLog = New StringBuilder
        sbLog.AppendLine(stringaSql)
        sbLog.AppendLine("")

        Try
            If Not IsNothing(collectionParametri) AndAlso collectionParametri.Count > 0 Then
                Dim parametri(collectionParametri.Count - 1) As SqlParameter
                collectionParametri.CopyTo(parametri, 0)
                If Not IsNothing(parametri) AndAlso parametri.Any Then
                    Dim parametriOrdinati = parametri.OrderBy(Of String)(Function(p) p.ParameterName).ToList
                    sbLog.AppendLine("PARAMETRI: ")
                    For Each p As SqlParameter In parametriOrdinati
                        sbLog.AppendLine(String.Format("{0} -> {1}", p.ParameterName, p.Value.ToString))
                    Next
                End If
            End If
        Catch ex As Exception
        End Try
        Return sbLog.ToString()

    End Function
    Public Function EseguiQuery_Scrittura_ParamVarBinary(
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByVal StringaSQL As String,
                                    ByVal NomeRoutine As String,
                                    ByVal CmdParameters As Dictionary(Of String, Byte())
                                    ) As Boolean Implements IDataProvider.EseguiQuery_Scrittura_ParamVarBinary

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura_VarBinary"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione))

                'Apro la connessione
                xConnessione.Open()

                'Inizializzo a nothing la transazione
                xTransazione = Nothing

            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione

                xConnectionState = xConnessione.State

                If xConnessione.State = ConnectionState.Closed Then
                    xConnessione.Open()
                End If

                xTransazione = objParametri.objTransazione

            End If


            xCommand = xConnessione.CreateCommand()

            If CmdParameters IsNot Nothing AndAlso CmdParameters.Count > 0 Then
                For Each param In CmdParameters
                    xCommand.Parameters.AddWithValue(param.Key, OleDbType.VarBinary).Value = param.Value
                Next
            End If

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace(), objParametri)

            dt = Nothing
            Throw ex
            'TODO Param Exception Check
        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function

    Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, parametri As List(Of DbParameter)) As Boolean Implements IDataProvider.EseguiQuery_Scrittura_Param

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura_Param"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione))

                'Apro la connessione
                xConnessione.Open()

                'Inizializzo a nothing la transazione
                xTransazione = Nothing

            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione

                xConnectionState = xConnessione.State

                If xConnessione.State = ConnectionState.Closed Then
                    xConnessione.Open()
                End If

                xTransazione = objParametri.objTransazione

            End If


            xCommand = xConnessione.CreateCommand()

            If parametri IsNot Nothing AndAlso parametri.Count > 0 Then
                For Each param In parametri
                    xCommand.Parameters.Add(param)
                Next
            End If

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione

            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace(), objParametri)

            dt = Nothing
            Throw ex
            'TODO Param Exception Check
        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp


    End Function

    Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri,
                                         ByRef objConnessione As DbConnection,
                                         ByRef objTransazione As DbTransaction,
                                         ByVal StringaConnessione As String,
                                         ByVal StringaSQL As String,
                                         ByVal NomeRoutine As String,
                                         ByVal parametri As List(Of DbParameter)) As Boolean Implements IDataProvider.EseguiQuery_Scrittura_Param

        Dim nomeRoutineLocale As String = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As SqlConnection = Nothing
        Dim xCommand As SqlCommand = Nothing
        Dim xDataAdapter As SqlDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As SqlTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Verifico se e' stata impostata una connessione
            If IsNothing(objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New SqlConnection(DataProviderFactory.Instance.AggiustaStringaDiConnessione(StringaConnessione))

                'Apro la connessione
                xConnessione.Open()

                'Inizializzo a nothing la transazione
                xTransazione = Nothing

            Else
                'Utilizzo quella passata come parametro
                xConnessione = objConnessione

                xConnectionState = xConnessione.State

                If xConnessione.State = ConnectionState.Closed Then
                    xConnessione.Open()
                End If

                xTransazione = objTransazione

            End If

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600

            If parametri IsNot Nothing AndAlso parametri.Any Then
                For Each param As SqlParameter In parametri
                    xCommand.Parameters.Add(param)
                Next
            End If

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione

            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As ParametrizzatoreException
            dt = Nothing
            Throw ex
        Catch ex As Exception
            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace(), objParametri)

            dt = Nothing
            Throw ex
            'TODO Param Exception Check
        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function

    Private Function DammiStackTrace() As String

        Dim st = New StackTrace()
        Dim metodoChiamante As String = String.Empty
        Dim sb As New StringBuilder

        Dim frames As List(Of StackFrame) = st.GetFrames().Take(10).ToList()
        For Each f As StackFrame In frames
            Dim myDeclaringType As Type = f.GetMethod().DeclaringType
            If myDeclaringType IsNot Nothing Then
                sb.AppendLine(String.Format("{0} - {1}", myDeclaringType.FullName, f.GetMethod.Name))
            End If

        Next
        metodoChiamante = sb.ToString()
        Return metodoChiamante

    End Function

    Public Function EseguiQuery_Lettura_jSon(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String) As String Implements IReadDataProvider.EseguiQuery_Lettura_jSon

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura_jSon"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As SqlCommand = Nothing

        Dim reader As SqlDataReader = Nothing

        'objParametri.StringaConnessione = objParametri.StringaConnessione + ";Pooling=false;OLE DB Services=-4;"

        Dim jsonResult As New StringBuilder()

        Dim StrSQL_ORIGINALE_LOG As String = String.Empty
        Dim parametrizzata As Boolean

        Try
            Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)

            Dim StringaSQL_ORIGINALE = StringaSQL.ToOrigin(objParametri)
            StrSQL_ORIGINALE_LOG = StringaSQL_ORIGINALE

            Dim StringaSql_PARAMETRIZZATA As String = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            parametrizzata = Parametrizza(StringaSQL, parameters, objParametri)

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New SqlConnection

                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objParametri.objConnessione.CreateCommand()
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            If parametrizzata Then
                For Each p As SqlParameter In parameters
                    xCommand.Parameters.Add(p)
                    If Debugger.IsAttached Then
                        Try
                            Debug.Print("DECLARE " & p.ParameterName & " as " & [Enum].GetName(GetType(System.Data.SqlDbType), p.SqlDbType) & "(" & p.Size.ToString & ")")
                            Debug.Print("SET " & p.ParameterName & "= '" & p.Value.ToString() & "'" & vbCrLf)
                        Catch ex As Exception
                        End Try
                    End If
                Next
            End If

            xCommand.CommandText = StringaSQL


            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xCommand.Transaction = objParametri.objTransazione
            End If


            Try

                reader = xCommand.ExecuteReader()

                If Not reader.HasRows Then
                    jsonResult.Append("[]")
                Else

                    While reader.Read()
                        jsonResult.Append(reader.GetValue(0).ToString())
                    End While
                End If

                If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If

            Catch e As Exception
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, e.Message, DammiStackTrace())

                Logga(NomeRoutine, messaggioErrore, objParametri)

                'Torno a leggere con l'istruzione originale
                xCommand.Parameters.Clear()
                xCommand.CommandText = StringaSQL_ORIGINALE


                xConnectionState = objParametri.objConnessione.State

                If objParametri.objConnessione.State = ConnectionState.Closed Then
                    objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                    objParametri.objConnessione.Open()
                End If

                If Not IsNothing(objParametri.objTransazione) Then
                    xCommand.Transaction = objParametri.objTransazione
                End If

                jsonResult.Clear()
                reader = xCommand.ExecuteReader()

                If Not reader.HasRows Then
                    jsonResult.Append("[]")
                Else

                    While reader.Read()
                        jsonResult.Append(reader.GetValue(0).ToString())
                    End While
                End If

                If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
            End Try
        Catch ex As ParametrizzatoreException

            jsonResult = Nothing
            Throw ex
        Catch ex As Exception

            messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, ex.Message, DammiStackTrace())

            LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine, StringaSQL, StrSQL_ORIGINALE_LOG, ex.Message, DammiStackTrace(), objParametri)

            If Not Debugger.IsAttached Then
                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}",
                                                   "", ex.Message)

            End If

            jsonResult = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
                reader.Close()
            End If

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If


            ' se ho creato la connessione locale, non ho creato nessuna transazione
            ' la transazione viene usata solo se esiste già
            If FlagConnessioneLocale Then
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()

                    '' WARNING !!!!!
                    'OleDbConnection.ReleaseObjectPool()
                    '' WARNING !!!!!

                    objParametri.objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    objParametri.objConnessione.Close()
                End If
            End If

        End Try


        Return jsonResult.ToString()


    End Function

    Public Function NomeIstanza_FromStringaConnessione(stringaConnessione As String) As String Implements IDataProvider.NomeIstanza_FromStringaConnessione

        Dim r As String = stringaConnessione.Split({";"c})(1)
        Dim s As String = r.Split({"="c})(1)
        Return s.Trim()

    End Function

    Private Sub LoggaConParametrieOriginale_InviaElasticSearch(NomeRoutine As String, StringaSQL As String, StrSQL_ORIGINALE_LOG As String, errore As String, stackTrace As String, objParametri As AgronicaCoreParametri)
        Dim messaggioErrore = String.Format("---> 1) MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, errore, stackTrace)

        Logga(NomeRoutine, messaggioErrore, objParametri, verificaInviaElasticSearch:=True)

        'Scriviamo nel log anche la query senza parametri 
        Dim log_StrOriginale As String = String.Format("---> 2) QUERY ORIGINALE SENZA PARAMETRI : {0}" & vbCrLf, StrSQL_ORIGINALE_LOG)
        Logga(NomeRoutine, log_StrOriginale, objParametri)
    End Sub
    Private Sub LoggaConParametrieOriginale(NomeRoutine As String, StringaSQL As String, StrSQL_ORIGINALE_LOG As String, errore As String, stackTrace As String)
        Dim messaggioErrore = String.Format("---> 1) MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "QUERY PARAMETRIZZATA MA IN ERRORE" & vbCrLf & "ERRORE: {1}" & vbCrLf & "METODO CHIAMANTE: {2}",
                                                   StringaSQL, errore, stackTrace)

        Logga(NomeRoutine, messaggioErrore, Nothing, verificaInviaElasticSearch:=True)

        'Scriviamo nel log anche la query senza parametri 
        Dim log_StrOriginale As String = String.Format("---> 2) QUERY ORIGINALE SENZA PARAMETRI : {0}" & vbCrLf, StrSQL_ORIGINALE_LOG)
        Logga(NomeRoutine, log_StrOriginale, Nothing)
    End Sub

    Public Function LivelloCompatibilita(objParametri As AgronicaCoreParametri) As Integer Implements IDataProvider.LivelloCompatibilita
        Dim rVal As Integer = -1
        Try
            Dim dbName = objParametri.Recupera_NomeDB()
            Dim dt As DataTable = EseguiQuery_Lettura(objParametri, "select compatibility_level from sys.databases where name = '" & dbName & "' ", "LivelloCompatibilita")
            If dt IsNot Nothing AndAlso dt.Rows.Count >= 1 Then
                Dim val = CInt(dt(0)(0))
                Return val
            End If

        Catch ex As Exception
        End Try

        Return rVal
    End Function
End Class
