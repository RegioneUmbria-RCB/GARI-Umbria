Imports System.Collections.Concurrent
Imports System.Data.SqlClient
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Provisioning
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class SincronizzatoreUtentiVisibilitaService

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _configurazioneServizio As Configurazione_Servizio
    Private ReadOnly _parametriExtra As SincroUtentiVisibilitaAppoggio_ParametriExtra
    Private ReadOnly _logger As New LogProvider
    Private _logFileName As String = "LogSincronizzazionePermessi.txt"
    Private objectLocker As New Object()
    Dim customLOGParams As CustomLOGParams

    Public Sub New(
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtente As AgronicaCoreParametri,
            ByVal objParametriSuperServer As AgronicaCoreParametri,
            ByVal configurazioneServizio As Configurazione_Servizio
        )

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _configurazioneServizio = configurazioneServizio

        If Not String.IsNullOrEmpty(_configurazioneServizio.Parametri_Extra) Then
            _parametriExtra = JsonConvert.DeserializeObject(Of SincroUtentiVisibilitaAppoggio_ParametriExtra)(_configurazioneServizio.Parametri_Extra)
        Else
            _parametriExtra = New SincroUtentiVisibilitaAppoggio_ParametriExtra With
            {
                .Fattore = 2,
                .Operazione = "*"
            }
        End If

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = _objParametriServer.LogDescrizioneUtente,
            .LogDirectory = _configurazioneServizio.DirectoryLOG,
            .LogFileName = _logFileName
        }


    End Sub

    Public Sub New(
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtente As AgronicaCoreParametri,
            ByVal objParametriSuperServer As AgronicaCoreParametri
        )
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer

        _parametriExtra = New SincroUtentiVisibilitaAppoggio_ParametriExtra With   {
            .Fattore = 2,
            .Operazione = "*"
        }
        _configurazioneServizio = New Configurazione_Servizio With {
            .DirectoryLOG = "C:\GiasLAN\LOG\SincronizzazioneUtentiVisibilita"
        }
    End Sub

    Public Function SincronizzaTuttiGliUtenti_Parallel() As Boolean
        Dim objProfilo_W As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
        Dim classJoin As New JoinFiltrone
        classJoin.bGerarchiaImprese = True
        classJoin.bCentriAziendali = True

        Dim debugMessage As String = String.Empty
        Dim strSql As String = String.Empty
        Dim PivaSuperUser = _objParametriServer.PivaSuperUser
        Dim retVal As Boolean = True
        Dim usaCapostipiti As Boolean = False
        Dim combineProcedures As Boolean = False
        Dim nomeRoutine As String = "AgronicaCoreUtentiBIZ.SincronizzatoreUtentiVisibilitaService.SincronizzaTuttiGliUtenti_Parallel"

        Try
            ReadConfiguration(usaCapostipiti, combineProcedures)
            If combineProcedures
                Return SincronizzaTuttiGliUtenti_Parallel_Std()
            End If

            Dim cnnServer As SqlConnection = DataProviderFactory.Instance.CreaNuovaConnessione(_objParametriServer.StringaConnessione)
            Dim cnnUtenti As SqlConnection = DataProviderFactory.Instance.CreaNuovaConnessione(_objParametriUtente.StringaConnessione)
            cnnServer.Open()
            cnnUtenti.Open()
            Dim chunksUtenti = LoadUserInChunks(nomeRoutine)
            cnnServer.Close()
            cnnUtenti.Close()

            Dim sw As New Stopwatch
            sw.Start()
            If chunksUtenti.Any Then
                Dim infoUtenti As New ConcurrentQueue(Of UtentiVisibilitaTempObject)
                Dim chunkCounuter As Integer = 0

                For Each chunck In chunksUtenti
                    Dim localCnn As SqlConnection = DataProviderFactory.Instance.CreaNuovaConnessione(_objParametriServer.StringaConnessione)
                    localCnn.Open()

                    Try
                        Dim swPerChunk As New Stopwatch
                        swPerChunk.Start()
                        chunkCounuter += 1

                        'Lavez - 20/03/2025 - Le elaborazioni concorrenti devono essere limitate altrimetni si corre il rischio di saturare il server
                        Dim options As New ParallelOptions With {
                            .MaxDegreeOfParallelism = If((Environment.ProcessorCount / 2) < 1, 1, Environment.ProcessorCount / 2)
                        }
                        Parallel.ForEach(chunck, options,
                          Sub(u As IDictionary(Of String, Object))
                              SyncLock (objectLocker)
                                  Dim sb As New StringBuilder
                                  Dim infoUtente As New UtentiVisibilitaTempObject(u)
                                  If Not infoUtente.visibilitaTotale Andalso infoUtente.ShouldRecalculateVisibility() Then
                                        infoUtente.daProcessare = True
                                        infoUtente.nomeTabellaTemp = Guid.NewGuid().ToString.Replace("-", "_")

                                        ' creazione tabella temporanea per utente
                                        Dim objUV_Creazione_Tmp_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                                        strSql = objUV_Creazione_Tmp_W.Ottieni_Sql_Creazione_Tabella_Temp_Utenti_Visibilita_Appoggio(infoUtente.nomeTabellaTemp)
                                        sb.Append(strSql & "; ")
                                        sb.Append(Environment.NewLine)

                                        Dim objPServer = _objParametriServer.CreateDeepCopy(_objParametriServer)
                                        Dim objPUtenti = _objParametriUtente.CreateDeepCopy(_objParametriUtente)

                                        If Not usaCapostipiti Then
                                            Dim FiltroneImprese As New AgronicaCoreUtility.Filtrone
                                            FiltroneImprese.MantieniParametri = True
                                            Dim sqlFiltroneImprese = FiltroneImprese.CreaStringaQueryPerDTFiltrone(objPServer, infoUtente.permessiSql, enum_TipoSelect_FiltroneSuperNova.Imprese_Visibilita_Appoggio, " ", classJoin, True, False).ToOrigin(objPServer)

                                            Dim objUT_Popola_Temp_Imprese_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                                            strSql = objUT_Popola_Temp_Imprese_R.Ottieni_Sql_Popola_Tabella_Temporanea_DT_FiltroneImprese(infoUtente.nomeTabellaTemp, PivaSuperUser, infoUtente.utente_UserName, enum_Entita_Analisi.Impresa, sqlFiltroneImprese)
                                            sb.Append(strSql & "; ")
                                            sb.Append(Environment.NewLine)

                                            Dim filtroneCentri As New AgronicaCoreUtility.Filtrone
                                            filtroneCentri.MantieniParametri = True
                                            Dim sqlFiltroneCentri = filtroneCentri.CreaStringaQueryPerDTFiltrone(objPServer, infoUtente.permessiSql, enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio, " ", classJoin, True, False)

                                            Dim objUT_Popola_Temp_Centri_R As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                                            strSql = objUT_Popola_Temp_Centri_R.Ottieni_Sql_Popola_Tabella_Temporanea_DT_FiltroneCentri(infoUtente.nomeTabellaTemp, PivaSuperUser, infoUtente.utente_UserName, enum_Entita_Analisi.Centro, sqlFiltroneCentri)
                                            sb.Append(strSql & "; ")
                                            sb.Append(Environment.NewLine)

                                            infoUtente.sql = sb.ToString

                                        Else
                                            Dim bizVisibilita As New AgronicaCoreUtentiBIZ.Utenti_Visibilita
                                            Dim pive = bizVisibilita.LeggiPiveCapostipiti(infoUtente.permessiXml)
                                            Dim strSqlPopolaConGerarchia As String = String.Empty
                                            objUV_Creazione_Tmp_W.PopolaConGerarchia(infoUtente.utente_UserName, pive, objPServer, strSqlPopolaConGerarchia, String.Format("#Utenti_Visibilita_Appoggio_{0}", infoUtente.nomeTabellaTemp))
                                            sb.Append(strSqlPopolaConGerarchia & "; ")
                                            sb.Append(Environment.NewLine)
                                            infoUtente.sql = sb.ToString
                                        End If
                                        objPServer = Nothing
                                        objPUtenti = Nothing
                                  End If
                                  infoUtenti.Enqueue(infoUtente)
                              End SyncLock
                          End Sub)

                        Dim numProfiliProcessati = infoUtenti.Where(Function(q) q.daProcessare).Count()
                        Dim numProfiliChunk = chunck.Count()

                        If infoUtenti.Count > 0 Then
                            Dim utentiDaProcessare As New List(Of UtentiVisibilitaTempObject)
                            Dim utentiConVisibilitaTotale As New List(Of UtentiVisibilitaTempObject)
                            DeqeueUsersOnVisibility(infoUtenti, utentiDaProcessare, utentiConVisibilitaTotale)

                            Dim sbCreazioneTemps As New StringBuilder
                            ' .1 per utenti senza visibilità totale sincronizza tabella Utenti_Visibilità_Appoggio
                            If utentiDaProcessare.Count > 0 Then
                                For Each u In utentiDaProcessare
                                    sbCreazioneTemps.Append(String.Format("-- Inizio Utente {0} ", u.utente_UserName))
                                    sbCreazioneTemps.Append(Environment.NewLine)
                                    sbCreazioneTemps.Append(u.sql)
                                    sbCreazioneTemps.Append(Environment.NewLine)
                                    sbCreazioneTemps.Append(String.Format(" -- Select * from #Utenti_Visibilita_Appoggio_{0} ;", u.nomeTabellaTemp))
                                    sbCreazioneTemps.Append(Environment.NewLine)
                                    sbCreazioneTemps.Append(String.Format("-- Fine Utente {0} ", u.utente_UserName))
                                    sbCreazioneTemps.Append(Environment.NewLine)
                                    _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, u.utente_UserName, CustomLOGParams:=customLOGParams)
                                Next

                                Dim cmd As SqlCommand = CreaCommand(localCnn, _objParametriServer.TimeoutQuery, sbCreazioneTemps.ToString)
                                Try
                                    cmd.ExecuteNonQuery()
                                    cmd.CommandText = String.Empty
                                Catch ex As Exception
                                    retVal = False
                                    _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, cmd.CommandText, CustomLOGParams:=customLOGParams)
                                End Try

                                For Each u In utentiDaProcessare
                                    Try
                                        Dim objUV_PopolaDaTemp_Tmp_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                                        strSql = objUV_PopolaDaTemp_Tmp_W.Ottieni_Sql_Popola_Utenti_Visibilita_Appoggio_Da_Tabella_Temp(
                                        u.nomeTabellaTemp, PivaSuperUser, u.utente_UserName)

                                        cmd.CommandText = strSql
                                        cmd.ExecuteNonQuery()
                                        cmd.CommandText = String.Empty

                                        Dim objUV_CancellaDaTemp_Tmp_W As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                                        strSql = objUV_CancellaDaTemp_Tmp_W.Ottieni_Sql_Cancella_Utenti_Visibilita_Appoggio_Da_Tabella_Temp(
                                        u.nomeTabellaTemp, PivaSuperUser, u.utente_UserName)
                                        cmd.CommandText = strSql
                                        cmd.ExecuteNonQuery()
                                        cmd.CommandText = String.Empty

                                        Dim sb As New StringBuilder
                                        sb.AppendLine("declare @tmpTableName nvarchar(100)                     ")
                                        sb.AppendLine("set     @tmpTableName =   @tmpTable                     ")
                                        sb.AppendLine("declare @sql nvarchar(100)                              ")
                                        sb.AppendLine("SET     @sql = 'DROP TABLE ' + QUOTENAME(@tmpTableName)	 ")
                                        sb.AppendLine("EXEC sp_executesql @sql	                                 ")


                                        Dim Param1 As String = "#Utenti_Visibilita_Appoggio_" & u.nomeTabellaTemp
                                        cmd.Parameters.Add(New SqlParameter("tmpTable", Param1))

                                        strSql = sb.ToString()

                                        cmd.CommandText = strSql
                                        cmd.ExecuteNonQuery()
                                        cmd.Parameters.Clear()
                                        cmd.CommandText = String.Empty

                                        'Se l'aggiornamento sulla utenti_profili (che è atomica) non va a buon fine devo invalidare anche la transazione sul db_server.
                                        Dim risAggProfili = objProfilo_W.ModificaDataUtentiVisibilitaAppoggio(u.utente_UserName, 5, DateTime.Now, _objParametriUtente)
                                        If risAggProfili = False Then
                                            Throw New Exception("Errore nell'aggiornamento data ultimo riporto utenti visibilita appoggio")
                                        End If
                                    Catch ex As Exception
                                        retVal = False
                                        _logger.Scrivi_LOG(
                                            _objParametriServer, nomeRoutine,
                                            String.Format("Errore nella sincronizzazione dell'utente {0}:{1}{2}", u.utente_UserName, Environment.NewLine, ex.StackTrace),
                                            CustomLOGParams:=customLOGParams)
                                    Finally
                                        If Not IsNothing(cmd) Then  cmd.Dispose()
                                    End Try
                                Next
                            End If

                            UpdateUsersFullVisibility(utentiConVisibilitaTotale, localCnn)
                        End If

                        swPerChunk.Stop()
                        _logger.Scrivi_LOG(
                            _objParametriServer, nomeRoutine,
                            String.Format("Tempo impiegato per sincronizzazione chunk {0}/{1} con {2} profili di cui {3} processati: {4} millisecondi.",
                                          chunkCounuter, chunksUtenti.Count, numProfiliChunk, numProfiliProcessati, swPerChunk.ElapsedMilliseconds.ToString),
                            CustomLOGParams:=customLOGParams)
                    Catch ex As Exception
                        retVal = False
                        debugMessage = String.Format("Errore durante sincronizzazione del chunk {0}/{1}: {3}{4}", chunkCounuter, chunksUtenti.Count, Environment.NewLine, ex.StackTrace)
                        _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, debugMessage, CustomLOGParams:=customLOGParams)
                    Finally
                        If Not IsNothing(localCnn) Then
                            If localCnn.State = ConnectionState.Open Then
                                localCnn.Close()
                            End If
                            localCnn.Dispose()
                        End If
                    End Try
                Next
            End If

            sw.Stop()
            debugMessage = String.Format("Tempo totale impiegato {0} secondi.", sw.Elapsed.TotalSeconds.ToString)
            _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, debugMessage, CustomLOGParams:=customLOGParams)

        Catch ex As Exception
            retVal = False
            debugMessage = ex.Message
            _logger.Scrivi_LOG(_objParametriServer,  nomeRoutine, debugMessage, CustomLOGParams:=customLOGParams)
        End Try
        Return retVal
    End Function

    ''' <summary>
    ''' Combined DS01-BL visibility synchronizer: calcola la visibilita' combinata
    ''' (gerarchia + pratiche AND/OR) per tutti gli utenti e popola
    ''' <c>Utenti_Visibilita_Appoggio</c> via <see cref="SqlBulkCopy"/>.
    ''' Attivato quando <c>Utenti_Visibilita_Calcolo_Combinato_Pratiche=1</c> in
    ''' <c>Configurazione_Siti</c>; altrimenti delega a <see cref="SincronizzaTuttiGliUtenti_Parallel"/>.
    ''' Scalato per 200.000+ utenti: calcolo parallelo senza SyncLock,
    ''' scrittura seriale per chunk via connessione dedicata.
    ''' </summary>
    Public Function SincronizzaTuttiGliUtenti_Parallel_Std() As Boolean
        Dim nomeRoutine As String = "AgronicaCoreUtentiBIZ.SincronizzatoreUtentiVisibilitaService.SincronizzaTuttiGliUtenti_Parallel_Std"
        Dim debugMessage As String = String.Empty
        Dim retVal As Boolean = True
        Try
            Dim confCalcoloCombinato As Boolean
            Dim daCapostipiti As Boolean 
            ReadConfiguration(daCapostipiti, confCalcoloCombinato)

            If Not confCalcoloCombinato Then
                Return SincronizzaTuttiGliUtenti_Parallel()
            End If

            Dim chunksUtenti = LoadUserInChunks(nomeRoutine)
            If Not chunksUtenti.Any Then
                Return retVal
            End If

            Dim sw As New Stopwatch
            sw.Start()
            CompteUserChunksVisibility(chunksUtenti, daCapostipiti, confCalcoloCombinato, retval, nomeRoutine)
            sw.Stop()
            debugMessage = String.Format("Sincronizzazione Std completata in {0} secondi.", sw.Elapsed.TotalSeconds.ToString)
            _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, debugMessage, CustomLOGParams:=customLOGParams)

        Catch ex As Exception
            retVal = False
            _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, ex.Message, CustomLOGParams:=customLOGParams)
        End Try
        Return retVal
    End Function

    Public Sub SincronizzaTuttiGliUtenti()
        Dim gestoreUtente As New AgronicaCoreUtentiBIZ.Utenti
        Dim utenti_R As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim dtUtenti As DataTable = Nothing
        Dim utenti As List(Of IDictionary(Of String, Object)) = Nothing
        Dim nomeRoutine As String = "AgronicaCoreUtentiBIZ.SincronizzatoreUtentiVisibilitaService.SincronizzaTuttiGliUtenti"
        Try
            dtUtenti = utenti_R.Leggi(AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", _objParametriUtente)
            If Not IsNothing(dtUtenti) Then
                utenti = dtUtenti.ToExpandoObject
            End If

            If Not IsNothing(utenti) AndAlso utenti.Any Then

                Dim sw As New Stopwatch
                sw.Start()

                For Each ut In utenti

                    Dim userNAme As String = If(ut("UserName") Is DBNull.Value, "", ut("UserName"))

                    If Not String.IsNullOrEmpty(userNAme) Then

                        Dim swPerUtente As New Stopwatch

                        Try
                            swPerUtente.Start()
                            gestoreUtente.InizializzaVisibilitaAppoggio(userNAme, _objParametriServer, _objParametriUtente)
                        Catch ex As Exception
                            Dim errorMessage As String = String.Format("Errore durante la sincronizzazione utente {0}:{1}{2}", userNAme, Environment.NewLine, ex.Message)
                            _logger.Scrivi_LOG(_objParametriServer,
                                               nomeRoutine,
                                               errorMessage,
                                               CustomLOGParams:=customLOGParams)
                        Finally
                            swPerUtente.Stop()
                            Dim debugMessage As String = String.Format("Tempo impiegato per sincronizzazione utente {0}: {1} millisecondi.", userNAme, swPerUtente.ElapsedMilliseconds.ToString)
                            _logger.Scrivi_LOG(_objParametriServer,
                                               nomeRoutine,
                                               debugMessage,
                                               CustomLOGParams:=customLOGParams)
                        End Try

                    End If

                Next

                sw.Stop()
                Dim messaggioFinale As String = String.Format("Tempo totale impiegato {0}", sw.ElapsedMilliseconds.ToString())
                _logger.Scrivi_LOG(_objParametriServer,
                                   nomeRoutine,
                                   messaggioFinale,
                                   CustomLOGParams:=customLOGParams)
            End If

        Catch ex As Exception
            ' TODO
        End Try
    End Sub

    Private Function ChunkBigSizeListBy(Of T)(ByVal source As List(Of T), ByVal chunkSize As Integer) As List(Of List(Of T))
        Dim result As New List(Of List(Of T))
        While source.Count > 0
            Dim newChunk As List(Of T) = source.Take(chunkSize).ToList()
            result.Add(newChunk)
            source.RemoveRange(0, newChunk.Count)
        End While
        Return result
    End Function

    ''' <summary>
    ''' Automatically adapts the chunk size based on the number of processors.
    ''' </summary>
    ''' <typeparam name="T">The type of the objects to be chunked.</typeparam>
    ''' <param name="source">The list of objects to be chunked.</param>
    ''' <param name="routineName">Optional name of the routine for logging purposes.</param>
    ''' <returns>A list of lists, where each inner list represents a chunk of the original list.</returns>
    Private Function ChunkByProcessors(Of T)(ByVal source As List(Of T), Optional routineName As String = "") As List(Of List(Of T))
        If Not String.IsNullOrEmpty(routineName) Then
            _logger.Scrivi_LOG(
                _objParametriServer, routineName,
                String.Format("Caricati {0} utenti. Calcolo dei chunks in corso...", source.Count),
                CustomLOGParams:=customLOGParams)
        End If

        Dim procCount As Integer = Environment.ProcessorCount
        Select Case _parametriExtra.Operazione
            Case "/"
                procCount = Environment.ProcessorCount / _parametriExtra.Fattore
            Case "*"
                procCount = Environment.ProcessorCount * _parametriExtra.Fattore
        End Select
        If _parametriExtra.DimensioneChunk.HasValue AndAlso _parametriExtra.DimensioneChunk.Value > 0 Then
            procCount = _parametriExtra.DimensioneChunk.Value
        End If
        Dim chunksUtenti = ChunkBigSizeListBy(source, procCount)

        If Not String.IsNullOrEmpty(routineName) Then
            _logger.Scrivi_LOG(
                _objParametriServer, routineName,
                String.Format("Creati {0} chunks da {1} utenti l'uno.", chunksUtenti.Count, procCount),
                CustomLOGParams:=customLOGParams)
        End If
        Return chunksUtenti
    End Function

    Private Sub ReadConfiguration(ByRef useAncestors As Boolean, ByRef combineProcedures As Boolean)
        Dim config As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim confCapostipiti = config.Leggi_Valore(
            Sito_Cod:=0, "Utenti_Visibilia_Appoggio_Da_Capostipiti",
            xFiltroAggiuntivo:="", xOrderBy:="", _objParametriServer
        ).Trim()
        useAncestors = String.IsNullOrWhiteSpace(confCapostipiti) OrElse confCapostipiti = "1"

        Dim confCombineProcedures = config.Leggi_Valore(
            Sito_Cod:=0, "Utenti_Visibilia_Appoggio_Combine_Procedures",
            xFiltroAggiuntivo:="", xOrderBy:="", _objParametriServer
        ).Trim()
        combineProcedures = String.IsNullOrWhiteSpace(confCombineProcedures) OrElse confCombineProcedures = "1"
    End Sub

    Private Function LoadUserInChunks(Optional nomeRoutine As String = "") As List(Of List(Of IDictionary(Of String, Object)))
        Dim sw As New Stopwatch
        sw.Start()

        Dim utenti_R As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim dtUtentiProfili = utenti_R.Leggi_X_Sincronizzazione_Uuenti_Visibilita_Appoggio(_objParametriUtente)
        Dim utentiProfili As List(Of IDictionary(Of String, Object)) = Nothing
        If Not IsNothing(dtUtentiProfili) Then
            utentiProfili = dtUtentiProfili.ToExpandoObject
        End If

        sw.Stop()
        If Not String.IsNullOrEmpty(nomeRoutine) Then
            _logger.Scrivi_LOG(
                _objParametriServer, nomeRoutine,
                String.Format("Caricamento info utenti e profili: {0} secondi.", sw.Elapsed.TotalSeconds.ToString),
                CustomLOGParams:=customLOGParams)
        End If

        If IsNothing(utentiProfili) OrElse Not utentiProfili.Any Then
            Return New List(Of List(Of IDictionary(Of String, Object)))
        End If

        Return ChunkByProcessors(Of IDictionary(Of String, Object))(utentiProfili, nomeRoutine)
    End Function

    #Region "Combined Visibility"
    ''' <summary>
    ''' Calculates the visibility of a chunk of users in parallel.
    ''' Each user's visibility is processed independently using parallel threads,
    ''' leveraging deep copies of server and user parameters to avoid synchronization issues.
    ''' Only users who do not already have full visibility and require recalculation are processed.
    ''' </summary>
    ''' <param name="chunk">A list of dictionaries representing users and their associated data.</param>
    ''' <param name="daCapostipiti">
    ''' Boolean flag indicating whether visibility should also consider root ancestors.
    ''' If True, the calculation includes visibility inherited from root-level users.
    ''' </param>
    ''' <param name="daCombinatoPratiche">
    ''' Boolean flag indicating whether to combine visibility based on practices.
    ''' If True, the calculation combines visibility based on both hierarchy and practices using AND/OR logic.
    ''' </param>
    ''' <param name="nomeRoutine">
    ''' Optional. Name of the routine for logging purposes.
    ''' If provided, any errors during the visibility calculation are logged with this routine name.
    ''' </param>
    ''' <returns>
    ''' A <see cref="ConcurrentQueue(Of UtentiVisibilitaTempObject)"/> containing visibility information
    ''' for each user in the chunk. Each object contains updated data tables for companies and centers,
    ''' and a flag indicating whether processing was performed.
    ''' </returns>
    ''' <remarks>
    ''' - The function uses a parallelized approach to improve performance on large user sets.
    ''' - Each thread operates on deep copies of server and user parameters, eliminating the need for locks.
    ''' - Exceptions during calculation are caught and logged without interrupting other threads.
    ''' - Users who already have total visibility or do not require recalculation are enqueued without modification.
    ''' </remarks>
    Private Function CalculateChunkVisbilityParallelStd(
        chunk As List(Of IDictionary(Of String, Object)),
        daCapostipiti As Boolean,
        daCombinatoPratiche As Boolean,
        Optional nomeRoutine As String = ""
    ) As ConcurrentQueue(Of UtentiVisibilitaTempObject)
        Dim gestoreUtente As New AgronicaCoreUtentiBIZ.Utenti
        Dim infoUtenti As New ConcurrentQueue(Of UtentiVisibilitaTempObject)
        Dim options As New ParallelOptions
        options.MaxDegreeOfParallelism = If(Environment.ProcessorCount / 2 < 1, 1, Environment.ProcessorCount / 2)

        Parallel.ForEach(chunk.ToList(), options,
                Sub(u As IDictionary(Of String, Object))
                    Dim infoUtente As New UtentiVisibilitaTempObject(u)

                    If Not infoUtente.visibilitaTotale AndAlso infoUtente.ShouldRecalculateVisibility() Then
                        Try
                            Dim objPServer = _objParametriServer.CreateDeepCopy(_objParametriServer)
                            Dim objPUtenti = _objParametriUtente.CreateDeepCopy(_objParametriUtente)
                            Dim risultato = gestoreUtente.CalcolaVisibilitaCombinata_Std(
                                infoUtente.utente_UserName, 5, daCapostipiti, daCombinatoPratiche, objPServer, objPUtenti
                            )
                            infoUtente.dtImprese = risultato.Item1
                            infoUtente.dtCentri = risultato.Item2
                            infoUtente.daProcessare = True
                        Catch ex As Exception
                            If Not String.IsNullOrEmpty(nomeRoutine) Then
                                _logger.Scrivi_LOG(
                                    _objParametriServer, nomeRoutine, 
                                    String.Format("Errore calcolo visibilita utente {0}: {1}", infoUtente.utente_UserName, ex.Message), 
                                    CustomLOGParams:=customLOGParams)
                            End If
                        End Try
                    End If

                    infoUtenti.Enqueue(infoUtente)
                End Sub)
        Return infoUtenti
    End Function
#End Region

    ' Fase 2: drain della coda e scrittura seriale via localCnn.
    Private Sub DeqeueUsersOnVisibility(
        infoUtenti As ConcurrentQueue(Of UtentiVisibilitaTempObject),
        byref utentiDaProcessare As List(Of UtentiVisibilitaTempObject),
        byref utentiConVisibilitaTotale As List(Of UtentiVisibilitaTempObject)
    )
        Dim infoUtenteDq As UtentiVisibilitaTempObject = Nothing
        While infoUtenti.TryDequeue(infoUtenteDq)
            If Not IsNothing(infoUtenteDq) Then
                If infoUtenteDq.visibilitaTotale Then
                    utentiConVisibilitaTotale.Add(infoUtenteDq)
                ElseIf infoUtenteDq.daProcessare Then
                    utentiDaProcessare.Add(infoUtenteDq)
                End If
            End If
        End While
    End Sub

    ' .1 utenti con visibilita' limitata: cancella + BulkCopy + aggiorna timestamp batch
    Private Function UpdateUsersPartialVisibility(
        utentiDaProcessare As List(Of UtentiVisibilitaTempObject),
        ByRef localCnn As SqlConnection,
        Optional nomeRoutine As String = ""
    ) As Boolean
        If utentiDaProcessare.Count = 0 Then Return True

        If Not String.IsNullOrEmpty(nomeRoutine) Then
            For Each u In utentiDaProcessare
                _logger.Scrivi_LOG(_objParametriServer, nomeRoutine, u.utente_UserName, CustomLOGParams:=customLOGParams)
            Next
        End If

        Dim objUvaW As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
        Dim utentiScrittCorrettamente As New List(Of String)
        Dim hasError As Boolean = False

        For Each u In utentiDaProcessare
            Try
                Dim strSqlCancella = objUvaW.Ottieni_Sql_Cancella_Per_Piu_Utenti(0, "", u.utente_UserName, _objParametriServer)
                Dim cmdDel = CreaCommand(localCnn, _objParametriServer.TimeoutQuery, strSqlCancella)
                cmdDel.ExecuteNonQuery()
                cmdDel.Dispose()

                Using bulk As New SqlBulkCopy(localCnn)
                    bulk.DestinationTableName = "dbo.Utenti_Visibilita_Appoggio"
                    bulk.BulkCopyTimeout = _objParametriServer.TimeoutQuery
                    If u.dtImprese IsNot Nothing AndAlso u.dtImprese.Rows.Count > 0 Then
                        bulk.WriteToServer(u.dtImprese)
                    End If
                    If u.dtCentri IsNot Nothing AndAlso u.dtCentri.Rows.Count > 0 Then
                        bulk.WriteToServer(u.dtCentri)
                    End If
                End Using

                utentiScrittCorrettamente.Add(u.utente_UserName)
            Catch ex As Exception
                hasError = True
                If Not String.IsNullOrEmpty(nomeRoutine) Then
                    _logger.Scrivi_LOG(
                        _objParametriServer, nomeRoutine,
                        String.Format("Errore scrittura visibilita utente {0}:{1}{2}", u.utente_UserName, Environment.NewLine, ex.StackTrace),
                        CustomLOGParams:=customLOGParams)
                End If
            Finally
                If u.dtImprese IsNot Nothing Then
                    u.dtImprese.Dispose()
                    u.dtImprese = Nothing
                End If
                If u.dtCentri IsNot Nothing Then
                    u.dtCentri.Dispose()
                    u.dtCentri = Nothing
                End If
            End Try
        Next

        If utentiScrittCorrettamente.Count > 0 Then
            Dim objProfilo_W As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
            If Not objProfilo_W.ModificaDataUtentiVisibilitaAppoggio(utentiScrittCorrettamente, DateTime.Now, _objParametriUtente) Then
                hasError = True
                If Not String.IsNullOrEmpty(nomeRoutine) Then
                    _logger.Scrivi_LOG(
                        _objParametriServer, nomeRoutine,
                        "Errore nell'aggiornamento batch della data ultimo riporto utenti visibilita appoggio",
                        CustomLOGParams:=customLOGParams)
                End If
            End If
        End If

        Return Not hasError
    End Function

    ' .2 utenti con visibilita' totale: svuota Utenti_Visibilita_Appoggio
    Private Sub UpdateUsersFullVisibility(utentiConVisibilitaTotale As List(Of UtentiVisibilitaTempObject), ByRef localCnn As SqlConnection)
        If utentiConVisibilitaTotale.Count > 0 Then
            Dim objUcW As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
            Dim listaUtenti As String = String.Join(",", utentiConVisibilitaTotale.Select(Function(u) u.utente_UserName).ToList())
            Dim strSqlCancellaAll = objUcW.Ottieni_Sql_Cancella_Per_Piu_Utenti(0, "", listaUtenti, _objParametriServer)
            Dim cmdCancellaAll = CreaCommand(localCnn, _objParametriServer.TimeoutQuery, strSqlCancellaAll)
            cmdCancellaAll.ExecuteNonQuery()
            cmdCancellaAll.Dispose()
        End If
    End Sub

    Private sub CompteUserChunksVisibility(
        chunksUtenti As List(Of List(Of IDictionary(Of String, Object))),
        daCapostipiti as boolean,
        confCalcoloCombinato as boolean,
        byref retVal As Boolean,
        optional nomeRoutine As String = ""
    )
        Dim chunkCounter As Integer = 0
        For Each chunk In chunksUtenti
            Dim localCnn As SqlConnection = DataProviderFactory.Instance.CreaNuovaConnessione(_objParametriServer.StringaConnessione)
            localCnn.Open()
            Try
                Dim swPerChunk As New Stopwatch
                swPerChunk.Start()
                chunkCounter += 1

                Dim infoUtenti = CalculateChunkVisbilityParallelStd(chunk, daCapostipiti, confCalcoloCombinato, nomeRoutine)
                Dim numProfiliProcessati As Integer = infoUtenti.Where(Function(q) q.daProcessare).Count()
                Dim numProfiliChunk As Integer = chunk.Count()
                Dim utentiDaProcessare As New List(Of UtentiVisibilitaTempObject)
                Dim utentiConVisibilitaTotale As New List(Of UtentiVisibilitaTempObject)
                DeqeueUsersOnVisibility(infoUtenti, utentiDaProcessare, utentiConVisibilitaTotale)

                retVal = UpdateUsersPartialVisibility(utentiDaProcessare, localCnn, nomeRoutine) AndAlso retVal
                UpdateUsersFullVisibility(utentiConVisibilitaTotale, localCnn)

                swPerChunk.Stop()
                If Not String.IsNullOrEmpty(nomeRoutine) Then
                    _logger.Scrivi_LOG(
                        _objParametriServer, nomeRoutine,
                        String.Format("Tempo chunk {0}/{1} con {2} profili di cui {3} processati: {4} ms.", 
                                        chunkCounter, chunksUtenti.Count, numProfiliChunk, numProfiliProcessati,
                                        swPerChunk.ElapsedMilliseconds.ToString),
                        CustomLOGParams:=customLOGParams)
                End If
            Catch ex As Exception
                retVal = False
                If Not String.IsNullOrEmpty(nomeRoutine) Then
                    _logger.Scrivi_LOG(
                        _objParametriServer, nomeRoutine,
                        String.Format("Errore chunk {0}/{1}: {2}{3}", chunkCounter, chunksUtenti.Count, Environment.NewLine, ex.StackTrace),
                        CustomLOGParams:=customLOGParams)
                End If
            Finally
                If Not IsNothing(localCnn) Then
                    If localCnn.State = ConnectionState.Open Then localCnn.Close()
                    localCnn.Dispose()
                End If
            End Try
        Next
    End sub

    Private Function CreaCommand(ByVal connection As SqlConnection, ByVal timeout As Integer, ByVal strsql As String) As SqlCommand
        Dim cmd As SqlCommand = connection.CreateCommand()
        cmd.CommandType = CommandType.Text
        cmd.CommandTimeout = timeout
        cmd.CommandText = strsql
        Return cmd
    End Function

    Private Class UtentiVisibilitaTempObject
      Public objPServer As AgronicaCoreParametri = Nothing
      Public objPUtenti As AgronicaCoreParametri = Nothing
      Public cnnServer As SqlConnection = Nothing
      Public cnnUtenti As SqlConnection = Nothing
      Public utente_UserName As String
      Public nomeTabellaTemp As String = String.Empty
      Public sql As String
      Public permessiSql As String = String.Empty
      Public permessiXml As String = String.Empty
      Public visibilitaTotale As Boolean = False
      Public daProcessare As Boolean = False
      Public dtImprese As DataTable = Nothing
      Public dtCentri As DataTable = Nothing

      public filtroPraticheAttivo As Boolean = False
      public OperatoreFiltri As String = String.empty
      Private dataModificaUtentiProfili As DateTime
      Private dataUltimoAggiornamentoPermessi As DateTime?

      Public Sub New()

      End Sub

      Public Sub New(u As IDictionary(Of String, Object))
          utente_UserName = If(u("UserName") Is DBNull.Value, "", u("UserName"))
          permessiSql = u("Descrizione_2").ToString & ""
          permessiXml = u("Descrizione_1").ToString & ""
          dataModificaUtentiProfili = u("Data_Modifica")
          dataUltimoAggiornamentoPermessi = If(u("DataUltimoRiportoUtentiVisibilitaAppoggio") Is DBNull.Value, CType(Nothing, DateTime?), u("DataUltimoRiportoUtentiVisibilitaAppoggio"))
          filtroPraticheAttivo = u("Filtro_Pratiche_Attivo") IsNot DBNull.Value andalso u("Filtro_Pratiche_Attivo") = 1
          OperatoreFiltri = If(u("Operatore_Filtri") Is DBNull.Value, "OR", u("Operatore_Filtri").ToString())
          visibilitaTotale = HasFullVisibility()
      End Sub

      ''' <summary>
      ''' Checks, based on the flags in Utenti_Profili, whether the user has full visibility.
      ''' This function remains approximate, returning true only when full visibility is obvious.
      ''' If the visibility calculation must be performed to determine whether it is full or not,
      ''' this function returns false, even though the user might actually have full visibility.
      ''' This logic was chosen to avoid running complex queries every time this information is needed.
      ''' </summary>
      ''' <remarks>Even if visibility by root ancestors is enabled, if the user has full visibility
      ''' the SQL filter will always be empty.</remarks>
      Private Function HasFullVisibility() As Boolean
          Dim allBusiness = String.IsNullOrWhiteSpace(permessiSql)
          If Not filtroPraticheAttivo Then Return allBusiness
          ' When practice filter is active, full visibility requires all-business access AND AND-semantics.
          ' OR-semantics would still restrict visibility to practice-linked companies.
          Return allBusiness AndAlso OperatoreFiltri = "AND"
      End Function

      Public Function ShouldRecalculateVisibility() As Boolean
          'Return filtroPraticheAttivo OrElse
          return Not dataUltimoAggiornamentoPermessi.HasValue OrElse dataUltimoAggiornamentoPermessi.Value < dataModificaUtentiProfili
      End Function
  End Class

End Class

Public Class SincroUtentiVisibilitaAppoggio_ParametriExtra
    Public Operazione As String
    Public Fattore As Integer
    ''' <summary>
    ''' When set, directly controls the number of users per processing chunk,
    ''' overriding the processor-based formula. Recommended value for 200K+ users: 1000.
    ''' Nothing = use processor-based calculation (backward-compatible default).
    ''' </summary>
    Public DimensioneChunk As Integer?
End Class


