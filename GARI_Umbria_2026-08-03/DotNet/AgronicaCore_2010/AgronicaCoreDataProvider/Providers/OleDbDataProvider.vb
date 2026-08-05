
Imports System.Data
'Imports System.data.SqlClient
Imports System.Data.OleDb
Imports System.Web
Imports System.Configuration
Imports System.IO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Common
Imports System.Text

Public Class OleDbDataProvider
    Inherits AgronicaCoreDataProvider.LogProvider
    Implements IDataProvider


    Private _parametri As Dictionary(Of Int32, AgroDBParametro) = Nothing

    Public Property ByPassaLog As Boolean Implements IDataProvider.ByPassaLog
        Get
            Return True
        End Get
        Set(value As Boolean)

        End Set
    End Property

    Public Property SqlDiretto As Boolean Implements IDataProvider.SqlDiretto
        Get
            Return True
        End Get
        Set(value As Boolean)

        End Set
    End Property

    Public Sub New()

    End Sub
    Public Sub New(ByVal parametri As Dictionary(Of Int32, AgroDBParametro))
        _parametri = parametri
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

        Dim nomeRoutineLocale = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objConnessione
            End If

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600
            xCommand.CommandText = StringaSQL

            If objTransazione IsNot Nothing Then
                xCommand.Transaction = objTransazione
            End If

            xDataAdapter = New OleDbDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = xConnessione.State

            If xConnessione.State = ConnectionState.Closed Then
                xConnessione.Open()
            End If

            xDataAdapter.Fill(dt)

        Catch ex As Exception

            messaggioErrore = ex.Message
            'Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xDataAdapter) Then
                xDataAdapter.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                Else
                    If xConnectionState = ConnectionState.Closed Then
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

        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Try


            'Se la connessione è chiusa la apro
            If objConnessione Is Nothing Then

                'Richiedo una connessione
                objConnessione = New OleDb.OleDbConnection

                objConnessione.ConnectionString = StringaConnessione
                objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            'If objTransazione Is Nothing Then

            '    'Inizializzo la transazione
            '    objTransazione = objConnessione.BeginTransaction

            '    FlagTransazioneLocale = True

            'End If


            xCommand = objConnessione.CreateCommand()
            xCommand.CommandTimeout = 600
            xCommand.CommandText = StringaSQL

            xDataAdapter = New OleDbDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objConnessione.State

            If objConnessione.State = ConnectionState.Closed Then
                objConnessione.Open()
            End If

            If Not IsNothing(objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objTransazione
            End If

            xDataAdapter.Fill(dt)

        Catch ex As Exception

            messaggioErrore = ex.Message
            'Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
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
            If Not IsNothing(objConnessione) Then
                If FlagConnessioneLocale Then
                    objConnessione.Close()
                    objConnessione.Dispose()
                    objConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
                        objConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura_XML(
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As String Implements IDataProvider.EseguiQuery_Lettura_XML

        Const nomeRoutineLocale = "EseguiQuery_Lettura_xml"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim strXMLOut As String = ""
        Dim conn As New SqlClient.SqlConnection
        conn.ConnectionString = getConnectionStringFromOleToSql(objParametri.StringaConnessione)

        If conn.State = ConnectionState.Closed Then
            conn.Open()
        End If

        Try

            Dim StringaSQL_ORIGINALE = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            Dim cmd As New SqlClient.SqlCommand(StringaSQL, conn)

            '22/03/2017: timeout configurabile su objparametri
            'xCommand.CommandTimeout = 1200
            cmd.CommandTimeout = objParametri.TimeoutQuery

            Dim rdr As Xml.XmlReader
            Dim xmlDoc As New Xml.XmlDocument()

            Try
                rdr = cmd.ExecuteXmlReader

            Catch e As Exception
                messaggioErrore = e.Message & " " & StringaSQL
                Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)

                'Torno a leggere con l'istruzione originale
                cmd.CommandText = StringaSQL_ORIGINALE
                rdr = cmd.ExecuteXmlReader
            End Try

            xmlDoc.Load(rdr)
            strXMLOut = xmlDoc.OuterXml

            conn.Close()

        Catch ex As Exception
            conn.Close()
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw New Exception(messaggioErrore)

        Finally


        End Try

        Return strXMLOut

    End Function


    Private Function getConnectionStringFromOleToSql(ByVal sConnessioneOle As String) As String

        Dim separator(1) As Char
        separator(0) = ";"c
        separator(1) = "="c

        Dim rVal As String = ""

        Dim daTenere As String = "server,data source,initial catalog,user id,password"

        Dim arrayFromString As String() = sConnessioneOle.Split(separator)
        For i As Integer = 0 To arrayFromString.Length - 2 Step 2
            If daTenere.IndexOf(arrayFromString(i).ToLower, 0) > -1 Then
                rVal &= arrayFromString(i) & "=" & arrayFromString(i + 1) & ";"
            End If
        Next

        Return rVal
    End Function


    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                            Optional chiamaScriviLog As Boolean = True
                                        ) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        Const nomeRoutineLocale = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        'objParametri.StringaConnessione = objParametri.StringaConnessione + ";Pooling=false;OLE DB Services=-4;"

        Dim dt As New DataTable

        Try


            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New OleDb.OleDbConnection

                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            'If objTransazione Is Nothing Then

            '    'Inizializzo la transazione
            '    objTransazione = objConnessione.BeginTransaction

            '    FlagTransazioneLocale = True

            'End If


            xCommand = objParametri.objConnessione.CreateCommand()

            '22/03/2017: timeout configurabile su objparametri
            'xCommand.CommandTimeout = 1200
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            Dim StringaSQL_ORIGINALE = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            xCommand.CommandText = StringaSQL

            xDataAdapter = New OleDbDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If

            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = e.Message & " " & StringaSQL
                Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)

                'Torno a leggere con l'istruzione originale
                xCommand.CommandText = StringaSQL_ORIGINALE

                xDataAdapter = New OleDbDataAdapter
                xDataAdapter.SelectCommand = xCommand

                xConnectionState = objParametri.objConnessione.State

                If objParametri.objConnessione.State = ConnectionState.Closed Then
                    objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                    objParametri.objConnessione.Open()
                End If

                If Not IsNothing(objParametri.objTransazione) Then
                    xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                End If

                xDataAdapter.Fill(dt)
            End Try

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
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
                    OleDbConnection.ReleaseObjectPool()
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

    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        StringaSQL As String,
                                        parametri As Dictionary(Of String, Object),
                                        NomeRoutine As String) As DataTable Implements IDataProvider.EseguiQuery_Lettura
        Throw New NotImplementedException()
    End Function

    '##############################################################################################
    Public Function EseguiQuery_Lettura(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String,
                                        ByRef DataSet2Fill As DataSet,
                                        ByVal strNomeDtNelDS As String
                                        ) As Boolean Implements IDataProvider.EseguiQuery_Lettura

        Const nomeRoutineLocale = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim risp As Boolean = False

        Try

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New OleDb.OleDbConnection

                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            'If objTransazione Is Nothing Then

            '    'Inizializzo la transazione
            '    objTransazione = objConnessione.BeginTransaction

            '    FlagTransazioneLocale = True

            'End If


            xCommand = objParametri.objConnessione.CreateCommand()

            '22/03/2017: timeout configurabile su objparametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            Dim StringaSQL_ORIGINALE = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            xCommand.CommandText = StringaSQL

            xDataAdapter = New OleDbDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If

            Try
                xDataAdapter.Fill(DataSet2Fill, strNomeDtNelDS)
            Catch e As Exception
                messaggioErrore = e.Message & " " & StringaSQL
                Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)

                'Torno a leggere con l'istruzione originale
                xCommand.CommandText = StringaSQL_ORIGINALE

                xDataAdapter = New OleDbDataAdapter
                xDataAdapter.SelectCommand = xCommand

                xConnectionState = objParametri.objConnessione.State

                If objParametri.objConnessione.State = ConnectionState.Closed Then
                    'objParametri.objConnessione = New OleDb.OleDbConnection
                    objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                    objParametri.objConnessione.Open()
                End If

                If Not IsNothing(objParametri.objTransazione) Then
                    xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                End If

                xDataAdapter.Fill(DataSet2Fill, strNomeDtNelDS)
            End Try

            risp = True


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
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
    Public Function EseguiQuery_Lettura(
                                        ByRef StringaConnessione As String,
                                        ByVal StringaSQL As String,
                                        ByVal NomeRoutine As String
                                        ) As DataTable Implements IDataProvider.EseguiQuery_Lettura

        Dim nomeRoutineLocale As String = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Try


            'Richiedo una connessione
            xConnessione = New OleDb.OleDbConnection

            xConnessione.ConnectionString = StringaConnessione
            xConnessione.Open()

            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600
            xCommand.CommandText = StringaSQL

            xDataAdapter = New OleDbDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xDataAdapter.Fill(dt)

        Catch ex As Exception

            messaggioErrore = ex.Message
            'Scrivi_LOG(objParametri.LogDirectory, objParametri.LogFileName, objParametri.LogDescrizioneUtente, NomeRoutine & "." & NomeRoutineLocale, MessaggioErrore)
            dt = Nothing
            'Throw New Exception(MessaggioErrore)

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


        Const nomeRoutineLocale = "EseguiQuery_Lettura"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Try


            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New OleDb.OleDbConnection

                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            xCommand = objParametri.objConnessione.CreateCommand()
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            Dim StringaSQL_ORIGINALE = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            If parametri IsNot Nothing AndAlso parametri.Any Then
                For Each p As DbParameter In parametri
                    xCommand.Parameters.Add(p)
                Next
            End If

            xCommand.CommandText = StringaSQL

            xDataAdapter = New OleDbDataAdapter
            xDataAdapter.SelectCommand = xCommand

            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
            End If

            Try
                xDataAdapter.Fill(dt)
            Catch e As Exception
                messaggioErrore = e.Message & " " & StringaSQL
                Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)

                'Torno a leggere con l'istruzione originale
                xCommand.CommandText = StringaSQL_ORIGINALE

                xDataAdapter = New OleDbDataAdapter
                xDataAdapter.SelectCommand = xCommand

                xConnectionState = objParametri.objConnessione.State

                If objParametri.objConnessione.State = ConnectionState.Closed Then
                    objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                    objParametri.objConnessione.Open()
                End If

                If Not IsNothing(objParametri.objTransazione) Then
                    xDataAdapter.SelectCommand.Transaction = objParametri.objTransazione
                End If

                xDataAdapter.Fill(dt)
            End Try

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
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
                    OleDbConnection.ReleaseObjectPool()
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

        Dim nomeRoutineLocale = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(StringaConnessione)

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
            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception

            'Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
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
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            ''Verifico se e' stata impostata una connessione
            'If IsNothing(objParametri.objConnessione) Then
            'Flag
            FlagConnessioneLocale = True
            'Creo la connessione localmente
            xConnessione = New OleDbConnection(stringaConnessione)

            'Apro la connessione
            xConnessione.Open()

            'Inizializzo a nothing la transazione
            xTransazione = Nothing

            'Else
            '    'Utilizzo quella passata come parametro
            '    xConnessione = objParametri.objConnessione

            '    xConnectionState = xConnessione.State

            '    If xConnessione.State = ConnectionState.Closed Then
            '        xConnessione.Open()
            '    End If

            '    xTransazione = objParametri.objTransazione

            'End If


            xCommand = xConnessione.CreateCommand()
            xCommand.CommandTimeout = 600
            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception

            'Scrivi_LOG(objParametri, NomeRoutine & "." & NomeRoutineLocale, MessaggioErrore)
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
                        xConnessione.Close()
                    End If
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

        Const nomeRoutineLocale = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing
        NumeroRecordInteressati = 0
        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(objParametri.StringaConnessione)

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

            '22/03/2017: timeout configurabile su objparametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            NumeroRecordInteressati = xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception

            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
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

        Const nomeRoutineLocale = "EseguiQuery_Scrittura"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(objParametri.StringaConnessione)

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

            '22/03/2017: timeout configurabile su objparametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception

            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
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

            Dim fs As New System.IO.StreamReader(strPathxFileIni)

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

    Public Function NomeDataBase_FromStringaConnessione(ByVal StringaConnessione As String
                                                        ) As String Implements IDataProvider.NomeDataBase_FromStringaConnessione



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
            Dim MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(exc, True, source:=True)


            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[FindConnessione_Su_Ini_O_Superserver()] : " & MessaggioErrore, exc)



        End Try

        Return strConString

    End Function

    'ricerca nel file ini la chiave x la connessione in base alla k passata
    Public Function FindIniConnessioni(ByVal strPath2Ini As String, ByVal strKey As String) As String Implements IDataProvider.FindIniConnessioni

        Dim fs As New System.IO.StreamReader(strPath2Ini)
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
                            'prendo la parte seguente il primo = 
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


            'uso questa funzione per ottenere il Messaggio:
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

        Dim Connessione As New OleDb.OleDbConnection

        Try
            'cerco la stringa di connessione dal file .ini
            'Dim strConnessione As String = FindConnessione_Su_Ini_O_Superserver()
            Dim strConnessione As String = objParametri.StringaConnessione

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

    Public Function EseguiQuery_Scrittura_ParamVarBinary(
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByVal StringaSQL As String,
                                    ByVal NomeRoutine As String,
                                    ByVal CmdParameters As Dictionary(Of String, Byte())
                                    ) As Boolean Implements IDataProvider.EseguiQuery_Scrittura_ParamVarBinary

        Const nomeRoutineLocale = "EseguiQuery_Scrittura_VarBinary"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(objParametri.StringaConnessione)

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

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception

            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function

    Public Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String, parametri As List(Of DbParameter)) As Boolean Implements IDataProvider.EseguiQuery_Scrittura_Param

        Const nomeRoutineLocale = "EseguiQuery_Scrittura_Param"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(objParametri.StringaConnessione)

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
                For Each param As OleDbParameter In parametri
                    xCommand.Parameters.Add(param)
                Next
            End If

            '22/03/2017: timeout configurabile su objParametri
            'xCommand.CommandTimeout = 600
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione


            'Eseguo la query
            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception

            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function

    Public Function EseguiQuery_Scrittura_Param(ByRef objParametri As AgronicaCoreParametri,
                                                ByRef objConnessione As DbConnection,
                                                ByRef objTransazione As DbTransaction,
                                                ByVal StringaConnessione As String,
                                                ByVal StringaSQL As String,
                                                ByVal NomeRoutine As String,
                                                ByVal parametri As List(Of DbParameter)) As Boolean Implements IDataProvider.EseguiQuery_Scrittura_Param

        Const nomeRoutineLocale = "EseguiQuery_Scrittura_Param"
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim messaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As OleDbConnection = Nothing
        Dim xCommand As OleDbCommand = Nothing
        Dim xDataAdapter As OleDbDataAdapter = Nothing

        Dim dt As New DataTable

        Dim xTransazione As OleDb.OleDbTransaction
        Dim FlagTransazioneLocale As Boolean = False

        Dim risp As Boolean = False

        Try

            'Verifico se e' stata impostata una connessione
            If IsNothing(objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = New OleDbConnection(StringaConnessione)

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

            If parametri IsNot Nothing AndAlso parametri.Count > 0 Then
                For Each param As OleDbParameter In parametri
                    xCommand.Parameters.Add(param)
                Next
            End If

            xCommand.CommandTimeout = 600
            xCommand.CommandText = StringaSQL
            xCommand.Transaction = xTransazione

            xCommand.ExecuteNonQuery()

            risp = True

        Catch ex As Exception
            If objParametri IsNot Nothing Then
                Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
            End If
            dt = Nothing
            Throw ex

        Finally

            If Not IsNothing(xCommand) Then
                xCommand.Dispose()
            End If

            If Not IsNothing(xConnessione) Then
                If FlagConnessioneLocale Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                    xConnessione = Nothing
                Else
                    If xConnectionState = ConnectionState.Closed Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return risp

    End Function

    Public Function EseguiQuery_Lettura_jSon(ByRef objParametri As AgronicaCoreParametri, StringaSQL As String, NomeRoutine As String) As String Implements IReadDataProvider.EseguiQuery_Lettura_jSon

        Const nomeRoutineLocale = "EseguiQuery_Lettura_jSon"

        Dim messaggioErrore As String = ""
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Dim xCommand As OleDbCommand = Nothing
        Dim reader As OleDbDataReader = Nothing

        'objParametri.StringaConnessione = objParametri.StringaConnessione + ";Pooling=false;OLE DB Services=-4;"

        Dim jSonResult As New StringBuilder

        Try


            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then

                'Richiedo una connessione
                objParametri.objConnessione = New OleDb.OleDbConnection

                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()

                FlagConnessioneLocale = True

            End If

            'If objTransazione Is Nothing Then

            '    'Inizializzo la transazione
            '    objTransazione = objConnessione.BeginTransaction

            '    FlagTransazioneLocale = True

            'End If


            xCommand = objParametri.objConnessione.CreateCommand()

            '22/03/2017: timeout configurabile su objparametri
            'xCommand.CommandTimeout = 1200
            xCommand.CommandTimeout = objParametri.TimeoutQuery

            Dim StringaSQL_ORIGINALE = StringaSQL
            Traduci(StringaSQL, objParametri.Lingua_Cod, "")

            xCommand.CommandText = StringaSQL


            xConnectionState = objParametri.objConnessione.State

            If objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                objParametri.objConnessione.Open()
            End If

            If Not IsNothing(objParametri.objTransazione) Then
                xCommand.Transaction = objParametri.objTransazione
            End If

            Try

                reader = xCommand.ExecuteReader()

                If Not reader.HasRows Then
                    jSonResult.Append("[]")
                Else

                    While reader.Read()
                        jSonResult.Append(reader.GetValue(0).ToString())
                    End While
                End If

                If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If

            Catch e As Exception
                messaggioErrore = e.Message & " " & StringaSQL
                Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)

                'Torno a leggere con l'istruzione originale
                xCommand.CommandText = StringaSQL_ORIGINALE


                xConnectionState = objParametri.objConnessione.State

                If objParametri.objConnessione.State = ConnectionState.Closed Then
                    objParametri.objConnessione.ConnectionString = objParametri.StringaConnessione
                    objParametri.objConnessione.Open()
                End If

                If Not IsNothing(objParametri.objTransazione) Then
                    xCommand.Transaction = objParametri.objTransazione
                End If

                reader = xCommand.ExecuteReader()
                jSonResult.Clear()

                If Not reader.HasRows Then
                    jSonResult.Append("[]")
                Else

                    While reader.Read()
                        jSonResult.Append(reader.GetValue(0).ToString())
                    End While
                End If

                If Not IsNothing(reader) AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If

            End Try

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine & "." & nomeRoutineLocale, messaggioErrore)
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
                    OleDbConnection.ReleaseObjectPool()
                    objParametri.objConnessione = Nothing
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    objParametri.objConnessione.Close()
                End If
            End If

        End Try

        Return jSonResult.ToString()

    End Function

    Public Function NomeIstanza_FromStringaConnessione(stringaConnessione As String) As String Implements IDataProvider.NomeIstanza_FromStringaConnessione

        Dim r As String = stringaConnessione.Split({";"c})(1)
        Dim s As String = r.Split({"="c})(1)
        Return s.Trim()
    End Function

    Public Function LivelloCompatibilita(objParametri As AgronicaCoreParametri) As Integer Implements IDataProvider.LivelloCompatibilita
        Dim rVal As Integer = -1
        Try
            Dim dbName = objParametri.Recupera_NomeDB()
            Dim dt As DataTable = EseguiQuery_Lettura(objParametri, "select compatibility_level from sys.databases where name = '" & dbName & "' ", "LivelloCompatibilita")
            If dt IsNot Nothing AndAlso dt.Rows.Count > 1 Then
                Return CInt(dt(0)(0))
            End If

        Catch ex As Exception
        End Try

        Return rVal
    End Function
End Class
