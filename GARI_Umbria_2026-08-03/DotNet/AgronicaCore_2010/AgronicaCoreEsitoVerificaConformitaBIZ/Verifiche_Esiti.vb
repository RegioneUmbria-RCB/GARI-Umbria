Imports System.Threading
Imports AgronicaCoreDataProvider

Public Class VerificheEsitiReaderWithPolling
    Inherits AgronicaCoreDataProvider.DataProvider

    Private _objParametriServer As AgronicaCoreParametri
    Private _lockObject As New Object

    Public Sub New(ByRef objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function LeggiRisultatiVerificaConformita(ByVal idTestata As Integer,
                                                 Optional ByVal tempoInSecondiTraDueRichieste As Integer = 5,
                                                 Optional ByVal timeoutInSecondi As Integer = 100) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaBIZ.VerificheEsitiReaderWithPolling.LeggiRisultatiVerificaConformita()"
        Dim result As DataTable = Nothing
        Dim doneEvent As New ManualResetEvent(False)

        Dim pollTimer = New Timers.Timer(tempoInSecondiTraDueRichieste * 1000)

        'se la lettura produce dei risultati restituisco il risultato
        AddHandler pollTimer.Elapsed,
            Sub(sender, e)

                Dim lockTaken As Boolean = False

                Try

                    Monitor.TryEnter(_lockObject, 0, lockTaken)
                    If lockTaken Then
                        Dim verificheEsitiDal As New AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_R
                        Dim dt = verificheEsitiDal.LeggiRisultati(idTestata, _objParametriServer)

                        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso result Is Nothing Then
                            result = dt
                            doneEvent.Set()
                        End If

                    End If
                Catch ex As Exception
                    Scrivi_LOG(_objParametriServer, nomeRoutine, ex.Message)
                    Throw ex
                Finally
                    If lockTaken Then Monitor.Exit(_lockObject)
                End Try

            End Sub

        ' se finisce il tempo di timeout si fermano le lettura
        Dim timeoutTimer = New Timers.Timer(timeoutInSecondi * 1000)
        timeoutTimer.AutoReset = False

        AddHandler timeoutTimer.Elapsed,
            Sub(sender, e)
                doneEvent.Set()
            End Sub

        ' Start timers
        pollTimer.Start()
        timeoutTimer.Start()

        ' Wait until result found OR timeout
        doneEvent.WaitOne()

        ' Cleanup timers
        pollTimer.Stop()
        pollTimer.Dispose()

        timeoutTimer.Stop()
        timeoutTimer.Dispose()

        Return result

    End Function

    Public Function LeggiStatoRichiesta(
        idTestata As Integer,
        Optional tempoInSecondiTraDueRichieste As Integer = 5,
        Optional timeoutInSecondi As Integer = 100
    ) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaBIZ.VerificheEsitiReaderWithPolling.LeggiRisultatiVerificaConformita()"
        Dim result As DataTable = Nothing
        Dim doneEvent As New ManualResetEvent(False)

        Dim pollTimer = New Timers.Timer(tempoInSecondiTraDueRichieste * 1000)

        'se la lettura produce dei risultati restituisco il risultato
        AddHandler pollTimer.Elapsed,
            Sub(sender, e)
                Dim lockTaken As Boolean = False
                Try
                    Monitor.TryEnter(_lockObject, 0, lockTaken)
                    If lockTaken Then
                        Dim verificheEsitiDal As New AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_R
                        Dim dt = verificheEsitiDal.LeggiStatoRichiesta(idTestata, _objParametriServer)

                        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso result Is Nothing Then
                            result = dt
                            doneEvent.Set()
                        End If

                    End If
                Catch ex As Exception
                    Scrivi_LOG(_objParametriServer, nomeRoutine, ex.Message)
                    Throw ex
                Finally
                    If lockTaken Then Monitor.Exit(_lockObject)
                End Try
            End Sub

        ' se finisce il tempo di timeout si fermano le lettura
        Dim timeoutTimer = New Timers.Timer(timeoutInSecondi * 1000)
        timeoutTimer.AutoReset = False

        AddHandler timeoutTimer.Elapsed,
            Sub(sender, e)
                doneEvent.Set()
            End Sub

        ' Start timers
        pollTimer.Start()
        timeoutTimer.Start()

        ' Wait until result found OR timeout
        doneEvent.WaitOne()

        ' Cleanup timers
        pollTimer.Stop()
        pollTimer.Dispose()

        timeoutTimer.Stop()
        timeoutTimer.Dispose()

        Return result
    End Function

End Class

Public Class Verifiche_Esiti_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class
Public Class Verifiche_Esiti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi___InterventiEVerifiche(ByVal PIVA As String,
                                                    ByVal sa_Cod As String,
                                                    ByVal veg_Cod As Integer,
                                                    ByVal dpi As String,
                                                    ByVal data_Da As Date,
                                                    ByVal data_A As Date,
                                                    ByVal flagImpostazioni As String,
                                                    ByVal flagMagazzino As String,
                                                    ByVal flagIaf As String,
                                                    ByVal interventiVerificati As DataTable,
                                                    ByVal risultatiInterventi As DataTable,
                                                    ByVal interventiVerificatiMagazzino As DataTable,
                                                    ByVal risultatiInterventiMagazzino As DataTable,
                                                    ByVal dettagliGiacenzeMagazzinoInizio As DataTable,
                                                    ByVal dettagliGiacenzeMagazzinoFine As DataTable,
                                                    ByVal timeSpan As Double,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    Optional ByVal origine As Integer = 0,
                                                    Optional ByRef idTestata As Integer = 0
                                                    )
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaBIZ.Verifiche_Esiti_W.Scrivi___InterventiEVerifiche()"
        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server, System.Data.IsolationLevel.ReadUncommitted)

            'todo capire se va sotto il read o write (al momento è sotto write)
            Dim xWrite As New AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W
            Dim risultatoIdTestata As Integer = -1
            Dim DT As New DataTable
            Dim count As Integer = 0
            If (interventiVerificati IsNot Nothing) Then
                count = interventiVerificati.Rows.Count()
            End If

            risultatoIdTestata = xWrite.ControllaScriviTestata(PIVA,
                                                      sa_Cod,
                                                      veg_Cod,
                                                      dpi,
                                                      data_Da,
                                                      data_A,
                                                      count,
                                                      flagImpostazioni,
                                                      flagMagazzino,
                                                      flagIaf,
                                                      timeSpan,
                                                      objParametri_Server,
                                                      origine)
            Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            If risultatoIdTestata <> -1 Then
                'chiama la funzione per scrivere gli interventi verificati
                Dim listaVerifiche As New List(Of DataRow)

                idTestata = risultatoIdTestata

                Dim dtVerifica As DataTable = New DataTable("VerificaConformitaRisultati")

                dtVerifica.Columns.Add("Id_Risultato", GetType(Integer))
                dtVerifica.Columns.Add("Id_Testata", GetType(Integer))
                dtVerifica.Columns.Add("piva", GetType(String))
                dtVerifica.Columns.Add("sa_cod", GetType(Integer))
                dtVerifica.Columns.Add("id_agenda", GetType(Integer))
                dtVerifica.Columns.Add("Lav_Cod", GetType(Integer))
                dtVerifica.Columns.Add("data_operazione", GetType(Date))
                dtVerifica.Columns.Add("Dpi_Cod", GetType(String))
                dtVerifica.Columns.Add("sup_trattata", GetType(Double))
                dtVerifica.Columns.Add("conforme", GetType(Boolean))
                dtVerifica.Columns.Add("conforme_Magazzino", GetType(Boolean))

                Dim dictAgendaIdRisultatiId As New Dictionary(Of String, Integer)
                For Each verifica As DataRow In interventiVerificati.Rows
                    Dim idVerificaConformitaRisultati As Integer = -1
                    idVerificaConformitaRisultati = xAgrosequenze.NuovoId_Tabella("Verifica_Conformita_Risultati", 0, 20000000, objParametri_Server)

                    dictAgendaIdRisultatiId.Add(verifica("Id_Agenda").ToString(), idVerificaConformitaRisultati)

                    Dim rowEsito As DataRow = dtVerifica.NewRow()
                    rowEsito("Id_Risultato") = idVerificaConformitaRisultati
                    rowEsito("Id_Testata") = risultatoIdTestata
                    rowEsito("piva") = verifica("Piva")
                    rowEsito("sa_cod") = verifica("Sa_Cod")
                    rowEsito("id_agenda") = verifica("Id_Agenda")
                    rowEsito("Lav_Cod") = verifica("Lav_Cod")
                    rowEsito("data_operazione") = verifica("data_movimento")
                    rowEsito("Dpi_Cod") = verifica("Disciplinare_cod")

                    Dim sup As Double

                    If Double.TryParse(
                    verifica("Superficie").ToString().Replace(",", "."),
                    Globalization.NumberStyles.Any,
                    Globalization.CultureInfo.InvariantCulture,
                    sup) Then

                        rowEsito("sup_trattata") = sup
                    Else
                        rowEsito("sup_trattata") = 0.0
                    End If

                    rowEsito("conforme") = If(CBool(verifica("BooleanRisVer").ToString().ToLower()), 1, 0)
                    'messo a false se il filtro è attivo, poi verra calcolato il valore corretto dopo la fase di scrittura esiti magazzino
                    If (flagMagazzino) Then
                        rowEsito("conforme_Magazzino") = False
                    Else
                        rowEsito("conforme_Magazzino") = DBNull.Value
                    End If

                    listaVerifiche.Add(rowEsito)
                Next
                xWrite.ScriviRisultati(listaVerifiche, objParametri_Server)

                If dictAgendaIdRisultatiId IsNot Nothing Then
                    Dim listaEsitiVerifica As New List(Of DataRow)

                    Dim dtEsitiVerifica As DataTable = New DataTable("EsitiVerifica")
                    dtEsitiVerifica.Columns.Add("Id_Esito", GetType(Integer))
                    dtEsitiVerifica.Columns.Add("Id_Risultato", GetType(Integer))
                    dtEsitiVerifica.Columns.Add("Id_Testata", GetType(Integer))
                    dtEsitiVerifica.Columns.Add("Conforme", GetType(Boolean))
                    dtEsitiVerifica.Columns.Add("Err_Code", GetType(String))
                    dtEsitiVerifica.Columns.Add("Dettagli", GetType(String))

                    Dim listaEsitiVerificaMagazzino As New List(Of DataRow)
                    Dim dtEsitiVerificaMagazzino As DataTable = New DataTable("EsitiVerificaMagazzino")
                    dtEsitiVerificaMagazzino.Columns.Add("Id_Esito_Magazzino", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Id_Testata", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Id_Risultato", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Piva_Magazzino", GetType(String))
                    dtEsitiVerificaMagazzino.Columns.Add("Sa_Cod_Magazzino", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Fabbricato_Cod_Magazzino", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Lotto", GetType(String))
                    dtEsitiVerificaMagazzino.Columns.Add("Conforme", GetType(Boolean))
                    dtEsitiVerificaMagazzino.Columns.Add("Id_Prodotto", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Categoria_Prodotto", GetType(Integer))
                    dtEsitiVerificaMagazzino.Columns.Add("Qta_Scaricata", GetType(Double))
                    dtEsitiVerificaMagazzino.Columns.Add("Giacenza_Magazzino", GetType(Double))
                    dtEsitiVerificaMagazzino.Columns.Add("Unita_Misura", GetType(String))
                    dtEsitiVerificaMagazzino.Columns.Add("Dettagli", GetType(String))

                    Dim listaDettagliGiacenzeMagazzino As New List(Of DataRow)
                    Dim dtDettagliGiacenzaMagazzino As DataTable = New DataTable("DettagliGiacenzaMagazzino")
                    dtDettagliGiacenzaMagazzino.Columns.Add("Id_Esito_Giacenza", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Id_Testata", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Piva_Magazzino", GetType(String))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Sa_Cod_Magazzino", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Fabbricato_Cod_Magazzino", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Lotto", GetType(String))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Conforme", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Id_Prodotto", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Categoria_Prodotto", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Giacenza_Magazzino", GetType(Decimal))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Unita_Misura", GetType(Integer))
                    dtDettagliGiacenzaMagazzino.Columns.Add("Dettagli", GetType(String))
                    dtDettagliGiacenzaMagazzino.Columns.Add("DataGiacenza", GetType(DateTime))
                    dtDettagliGiacenzaMagazzino.Columns.Add("TipoGiacenza", GetType(Integer))

                    For Each map As KeyValuePair(Of String, Integer) In dictAgendaIdRisultatiId
                        Dim esitiVerifica As DataTable = risultatiInterventi.Select($"Id_Agenda = '{map.Key.ToString()}'").CopyToDataTable()

                        If esitiVerifica IsNot Nothing Then
                            For Each risultatoEsito As DataRow In esitiVerifica.Rows
                                Dim idVerificaConformitaEsiti As Integer = -1
                                idVerificaConformitaEsiti = xAgrosequenze.NuovoId_Tabella("Verifica_Conformita_Esiti", 0, 20000000, objParametri_Server)

                                Dim rowEsito As DataRow = dtEsitiVerifica.NewRow()
                                rowEsito("Id_Esito") = idVerificaConformitaEsiti
                                rowEsito("Id_Risultato") = map.Value
                                rowEsito("Id_Testata") = risultatoIdTestata
                                rowEsito("Conforme") = risultatoEsito("BooleanRisVer")
                                rowEsito("Err_Code") = risultatoEsito("Err_Code")
                                rowEsito("Dettagli") = risultatoEsito("Err_nota")

                                ' Aggiungi la riga alla lista
                                listaEsitiVerifica.Add(rowEsito)
                            Next
                        End If

                        'gestione dati magazzino
                        If flagMagazzino Then
                            ' Verifica se ci sono righe prima di chiamare CopyToDataTable
                            Dim filteredRows = risultatiInterventiMagazzino.Select($"Id_Agenda = '{map.Key.ToString()}'")

                            If filteredRows IsNot Nothing AndAlso filteredRows.Length > 0 Then
                                Dim esitiVerificaMagazzino As DataTable = filteredRows.CopyToDataTable()

                                For Each risultatoEsitoMagazzino As DataRow In esitiVerificaMagazzino.Rows
                                    Dim idVerificaConformitaEsitiMagazzino As Integer = -1
                                    idVerificaConformitaEsitiMagazzino = xAgrosequenze.NuovoId_Tabella("Verifica_Conformita_Esiti_Magazzino", 0, 20000000, objParametri_Server)

                                    Dim rowEsitoMagazzino As DataRow = dtEsitiVerificaMagazzino.NewRow()
                                    rowEsitoMagazzino("Id_Esito_Magazzino") = idVerificaConformitaEsitiMagazzino
                                    rowEsitoMagazzino("Id_Testata") = risultatoIdTestata
                                    rowEsitoMagazzino("Id_Risultato") = map.Value

                                    rowEsitoMagazzino("Piva_Magazzino") = risultatoEsitoMagazzino("piva")
                                    rowEsitoMagazzino("Sa_Cod_Magazzino") = risultatoEsitoMagazzino("Sa_Cod")
                                    rowEsitoMagazzino("Fabbricato_Cod_Magazzino") = risultatoEsitoMagazzino("Fabbricato_Cod")

                                    rowEsitoMagazzino("Lotto") = risultatoEsitoMagazzino("Lotto")

                                    rowEsitoMagazzino("Conforme") = risultatoEsitoMagazzino("BooleanRisVer")

                                    rowEsitoMagazzino("Id_Prodotto") = risultatoEsitoMagazzino("pro_cod")
                                    rowEsitoMagazzino("Categoria_Prodotto") = risultatoEsitoMagazzino("elem_cod")

                                    rowEsitoMagazzino("Qta_Scaricata") = risultatoEsitoMagazzino("qta")
                                    rowEsitoMagazzino("Giacenza_Magazzino") = risultatoEsitoMagazzino("qta_presente")

                                    rowEsitoMagazzino("Unita_Misura") = risultatoEsitoMagazzino("udm_cod")

                                    rowEsitoMagazzino("Dettagli") = risultatoEsitoMagazzino("RisultatoVerifica")

                                    ' Aggiungi la riga alla lista
                                    listaEsitiVerificaMagazzino.Add(rowEsitoMagazzino)
                                Next
                            End If
                        End If
                    Next
                    xWrite.ScriviEsiti(listaEsitiVerifica, objParametri_Server)
                    xWrite.ScriviEsitiMagazzino(listaEsitiVerificaMagazzino, objParametri_Server)

                    'INIZIO GIACENZA
                    If dettagliGiacenzeMagazzinoInizio IsNot Nothing AndAlso dettagliGiacenzeMagazzinoInizio.Rows.Count > 0 Then
                        For Each dettaglioGiacenzaInizio As DataRow In dettagliGiacenzeMagazzinoInizio.Rows
                            Dim idGiacenza As Integer = -1
                            idGiacenza = xAgrosequenze.NuovoId_Tabella("Verifica_Conformita_Esiti_Giacenze", 0, 20000000, objParametri_Server)

                            Dim rowEsitoMagazzino As DataRow = dtDettagliGiacenzaMagazzino.NewRow()
                            rowEsitoMagazzino("Id_Esito_Giacenza") = idGiacenza
                            rowEsitoMagazzino("Id_Testata") = risultatoIdTestata
                            rowEsitoMagazzino("Piva_Magazzino") = dettaglioGiacenzaInizio("Piva_Magazzino")
                            rowEsitoMagazzino("Sa_Cod_Magazzino") = dettaglioGiacenzaInizio("Sa_Cod_Magazzino")
                            rowEsitoMagazzino("Fabbricato_Cod_Magazzino") = dettaglioGiacenzaInizio("Fabbricato_Cod_Magazzino")
                            rowEsitoMagazzino("Lotto") = dettaglioGiacenzaInizio("lotto")
                            rowEsitoMagazzino("Conforme") = dettaglioGiacenzaInizio("Conforme")
                            rowEsitoMagazzino("Id_Prodotto") = dettaglioGiacenzaInizio("Id_Prodotto")
                            rowEsitoMagazzino("Categoria_Prodotto") = dettaglioGiacenzaInizio("Categoria_Prodotto")
                            rowEsitoMagazzino("Giacenza_Magazzino") = dettaglioGiacenzaInizio("Giacenza_Magazzino")
                            rowEsitoMagazzino("Unita_Misura") = dettaglioGiacenzaInizio("Unita_Misura")
                            rowEsitoMagazzino("Dettagli") = dettaglioGiacenzaInizio("Dettagli")
                            rowEsitoMagazzino("DataGiacenza") = dettaglioGiacenzaInizio("DataGiacenza")
                            rowEsitoMagazzino("TipoGiacenza") = dettaglioGiacenzaInizio("TipoGiacenza")

                            listaDettagliGiacenzeMagazzino.Add(rowEsitoMagazzino)
                        Next
                    End If

                    'FINE GIACENZA
                    If dettagliGiacenzeMagazzinoFine IsNot Nothing AndAlso dettagliGiacenzeMagazzinoFine.Rows.Count > 0 Then
                        For Each dettaglioGiacenzaFine As DataRow In dettagliGiacenzeMagazzinoFine.Rows
                            Dim idGiacenza As Integer = -1
                            idGiacenza = xAgrosequenze.NuovoId_Tabella("Verifica_Conformita_Esiti_Giacenze", 0, 20000000, objParametri_Server)

                            Dim rowEsitoMagazzino As DataRow = dtDettagliGiacenzaMagazzino.NewRow()
                            rowEsitoMagazzino("Id_Esito_Giacenza") = idGiacenza
                            rowEsitoMagazzino("Id_Testata") = risultatoIdTestata
                            rowEsitoMagazzino("Piva_Magazzino") = dettaglioGiacenzaFine("Piva_Magazzino")
                            rowEsitoMagazzino("Sa_Cod_Magazzino") = dettaglioGiacenzaFine("Sa_Cod_Magazzino")
                            rowEsitoMagazzino("Fabbricato_Cod_Magazzino") = dettaglioGiacenzaFine("Fabbricato_Cod_Magazzino")
                            rowEsitoMagazzino("Lotto") = dettaglioGiacenzaFine("lotto")
                            rowEsitoMagazzino("Conforme") = dettaglioGiacenzaFine("Conforme")
                            rowEsitoMagazzino("Id_Prodotto") = dettaglioGiacenzaFine("Id_Prodotto")
                            rowEsitoMagazzino("Categoria_Prodotto") = dettaglioGiacenzaFine("Categoria_Prodotto")
                            rowEsitoMagazzino("Giacenza_Magazzino") = dettaglioGiacenzaFine("Giacenza_Magazzino")
                            rowEsitoMagazzino("Unita_Misura") = dettaglioGiacenzaFine("Unita_Misura")
                            rowEsitoMagazzino("Dettagli") = dettaglioGiacenzaFine("Dettagli")
                            rowEsitoMagazzino("DataGiacenza") = dettaglioGiacenzaFine("DataGiacenza")
                            rowEsitoMagazzino("TipoGiacenza") = dettaglioGiacenzaFine("TipoGiacenza")

                            listaDettagliGiacenzeMagazzino.Add(rowEsitoMagazzino)
                        Next
                    End If

                    If listaDettagliGiacenzeMagazzino.Count > 0 Then
                        xWrite.ScriviDettagliGiacenzeMagazzino(listaDettagliGiacenzeMagazzino, objParametri_Server)
                    End If
                End If
            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            If objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'Non lanciamo eccenzione, la scrittura dei risultati non deve bloccare il processo di verifica conformità
            'Throw New Exception(ex)

        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try
    End Sub
End Class
