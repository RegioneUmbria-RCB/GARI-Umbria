Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGiasDuplicateResxDAL
Imports GiasDuplicateResx

Public Class ResxDatabaseManager
    Implements IResxDatabaseManager
    Private ReadOnly _path As String
    Private ReadOnly _logService As ILogService
    Private blockSize As Integer = 1000
    Private objParametriDBAppoggio As AgronicaCoreParametri

    Public Sub New(ByVal path As String, logService As ILogService, objParametriDBAppoggio As AgronicaCoreParametri)
        Me._path = path
        Me._logService = logService
        Me.objParametriDBAppoggio = objParametriDBAppoggio
    End Sub

    Public Function InsertToDB(resxDataList As List(Of ResxData)) As Boolean Implements IResxDatabaseManager.InsertToDB

        'Dim sqlBatch As String = GenerateSqlBatch(resxDataList)

        'Return sqlBatch

        Dim xGiasDuplicateResx_W As New GiasDuplicateResx_W
        Try
            ' Utilizza la funzione SuddividiInBlocchi per ottenere i blocchi di dati
            Dim dataBlocks = GiasDuplicateResx.Utility.SuddividiInBlocchi(resxDataList, blockSize)
            Dim counter = 0
            Dim stopwatch As New Stopwatch()
            Dim query As String = ""
            Dim logResult As DataTable

            For Each dataBlock In dataBlocks
                Try
                    counter += 1

                    ' Chiamata alla funzione GeneraInsertQuery per ottenere la stringa di query
                    query = GenerateSqlBatch(dataBlock, counter)

                    stopwatch.Start()
                    ' Esegui la query nel database
                    logResult = xGiasDuplicateResx_W.InserisciRecordIntoResxDotNetData(objParametriDBAppoggio, query)
                    'Esegui l'operazione di inserimento nel tuo database qui

                    stopwatch.Stop()

                    ' Log del tempo di esecuzione
                    _logService.Scrivi_LOG("FileResxRepository.InsertToDB", $"Tempo di esecuzione: {stopwatch.ElapsedMilliseconds} ms", TipoMessaggio.Informazione)

                    ' Scrive nei log gli errori nella tabella logResult con ErrorNumber maggiore di 0
                    For Each row As DataRow In logResult?.Rows.Cast(Of DataRow)().Where(Function(r) Convert.ToInt32(r("ErrorNumber")) > 0)
                        _logService.Scrivi_LOG("FileResxRepository.InsertToDB", $"Errore durante l'inserimento (Error Number: {Convert.ToInt32(row("ErrorNumber"))}): {row("ErrorMessage")} - Query: {row("Query")}", TipoMessaggio.Errore)
                    Next

                    'If logResult.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(logResult.Rows(0)("ErrorMessage").ToString()) Then
                    '    ' Se c'è un messaggio di errore, interrompi l'operazione e ritorna falso
                    '    Return False
                    'End If
                Catch ex As Exception
                    _logService.Scrivi_LOG("FileResxRepository.InsertToDB", $"Errore - {ex.Message}", TipoMessaggio.Errore)
                    Throw
                End Try
            Next

            Return True
        Catch ex As Exception
            _logService.Scrivi_LOG("FileResxRepository.InsertToDB", $"Errore - {ex.Message}", TipoMessaggio.Errore)
            Throw
        End Try

    End Function

    Public Function GenerateSqlBatch(resxDataList As List(Of ResxData), giro As Integer) As String
        Dim sqlBatch As New StringBuilder()
        Dim tempErrorLogTableName As String = "#ErrorLogTable"

        ' Aggiunta dell'intestazione del batch SQL e verifica ed eventuale eliminazione della tabella temporanea
        sqlBatch.AppendLine("BEGIN TRANSACTION;")
        sqlBatch.AppendLine("")

        ' Se il giro è 1, aggiungi la query per eliminare i dati dalla tabella principale
        If giro = 1 Then
            sqlBatch.AppendLine("DELETE FROM ResxDotNetData;")
            sqlBatch.AppendLine("")
        End If

        sqlBatch.AppendLine($"IF OBJECT_ID('tempdb..{tempErrorLogTableName}') IS NOT NULL")
        sqlBatch.AppendLine($"    DROP TABLE {tempErrorLogTableName};")
        sqlBatch.AppendLine("")
        sqlBatch.AppendLine($"CREATE TABLE {tempErrorLogTableName} (error_message NVARCHAR(MAX), error_number INT, query_failed NVARCHAR(MAX));")

        ' Iterazione attraverso la lista di oggetti ResxData e costruzione delle query di inserimento
        For Each resxData As ResxData In resxDataList
            Dim insertQuery As String = $"INSERT INTO ResxDotNetData (data_ora_inizio_scansione, percorso_file_scansito, chiave, valore) VALUES ('{resxData.ScanDateTime}','{resxData.FilePath.Replace("'", "''")}','{resxData.Key.Replace("'", "''")}','{resxData.Value.Replace("'", "''")}');"
            sqlBatch.AppendLine("BEGIN TRY")
            sqlBatch.AppendLine(insertQuery)
            sqlBatch.AppendLine("END TRY")
            sqlBatch.AppendLine("BEGIN CATCH")
            sqlBatch.AppendLine("IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;")
            sqlBatch.AppendLine($"INSERT INTO {tempErrorLogTableName} (error_message, error_number, query_failed) VALUES (ERROR_MESSAGE(), ERROR_NUMBER(), '{insertQuery.Replace("'", "''")}');")
            sqlBatch.AppendLine("END CATCH")
        Next

        ' Aggiunta del commit finale del batch SQL e pulizia
        sqlBatch.AppendLine("IF @@TRANCOUNT > 0 COMMIT TRANSACTION;")
        sqlBatch.AppendLine($"SELECT * FROM {tempErrorLogTableName};")
        sqlBatch.AppendLine($"DROP TABLE {tempErrorLogTableName};")

        ' Restituzione della stringa SQL batch
        Return sqlBatch.ToString()
    End Function




    Public Function VerificaDuplicati() As String Implements IResxDatabaseManager.VerificaDuplicati
        Dim xGiasDuplicateResx_R As New GiasDuplicateResx_R
        Try
            Dim logResult As DataTable
            Dim DT As DataTable
            Dim query As String = ""
            ' Chiamata alla funzione GetDuplicateKeysQuery per ottenere la stringa di query
            query = GetDuplicateKeysQuery()
            DT = xGiasDuplicateResx_R.LeggiChiaviDuplicati(objParametriDBAppoggio, query)
            Return ElencoDuplicati(DT)
        Catch ex As Exception
            Return ""
        End Try
        Return ""
    End Function



    Public Function GetDuplicateKeysQuery() As String
        Dim sqlBatch As New StringBuilder()

        sqlBatch.AppendLine("SELECT percorso_file_scansito, chiave, RipetizioniTotali, valore")
        sqlBatch.AppendLine("FROM (")
        sqlBatch.AppendLine("    SELECT percorso_file_scansito, chiave, valore,")
        sqlBatch.AppendLine("    (SELECT COUNT(*) FROM ResxDotNetData AS R WHERE R.chiave = RD.chiave) AS RipetizioniTotali")
        sqlBatch.AppendLine("    FROM ResxDotNetData RD")
        sqlBatch.AppendLine("    GROUP BY percorso_file_scansito, chiave, valore")
        sqlBatch.AppendLine(") AS T")
        sqlBatch.AppendLine("WHERE RipetizioniTotali > 1")
        sqlBatch.AppendLine("ORDER BY RipetizioniTotali DESC;")

        Return sqlBatch.ToString()
    End Function

    Public Function ElencoDuplicati(dt As DataTable) As String
        Dim Messaggio_di_Ritorno_Opzionale As New StringBuilder()

        Try
            ' Aggiungi data e ora del report all'inizio
            Dim dataOraReport As String = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            Messaggio_di_Ritorno_Opzionale.AppendLine($"📅 Data e Ora del Report: {dataOraReport}")
            Messaggio_di_Ritorno_Opzionale.AppendLine()

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Intestazione del report
                Messaggio_di_Ritorno_Opzionale.AppendLine("🔍 Report Duplicati 🔍")
                Messaggio_di_Ritorno_Opzionale.AppendLine(New String("=", 100))
                Messaggio_di_Ritorno_Opzionale.AppendLine()

                ' Raggruppa i risultati per chiave
                Dim query = From row In dt.AsEnumerable()
                            Group row By Chiave = row.Field(Of String)("chiave") Into Group
                            Select Chiave, Dettagli = Group

                For Each gruppo In query
                    ' Mostra la chiave e il conteggio dei file unici per quella chiave
                    Dim conteggioPercorsi As Integer = gruppo.Dettagli.Count()
                    Dim headerLine As String = $"🔑 : '{gruppo.Chiave}' - 🔄 Totali File: {conteggioPercorsi}"

                    Messaggio_di_Ritorno_Opzionale.AppendLine(headerLine)
                    Messaggio_di_Ritorno_Opzionale.AppendLine(New String("-", 10))

                    ' Calcolo della lunghezza massima dei percorsi per l'allineamento
                    Dim maxPercorsoLength = gruppo.Dettagli.Max(Function(d) d.Field(Of String)("percorso_file_scansito").Length)

                    ' Elenco dei percorsi unici dei file e il valore in cui la chiave è presente
                    For Each dettaglio In gruppo.Dettagli
                        Dim percorsoCompleto As String = dettaglio("percorso_file_scansito").ToString()
                        Dim percorso As String = RemoveCommonPath(percorsoCompleto)
                        Dim valore As String = dettaglio("valore").ToString()
                        Dim lineToAdd As String = $"📁 File: {percorso.PadRight(maxPercorsoLength + 5)}| Valore: {valore}"
                        Messaggio_di_Ritorno_Opzionale.AppendLine(lineToAdd)
                    Next
                    Messaggio_di_Ritorno_Opzionale.AppendLine(New String("-", 10))
                    Messaggio_di_Ritorno_Opzionale.AppendLine()
                Next

                ' Chiusura del report
                Messaggio_di_Ritorno_Opzionale.AppendLine(New String("=", 100))
                Messaggio_di_Ritorno_Opzionale.AppendLine("Fine Report Duplicati")
            Else
                ' Nessun duplicato trovato
                Messaggio_di_Ritorno_Opzionale.AppendLine("✅ Nessun duplicato trovato. Tutto in ordine!")
            End If

            Return Messaggio_di_Ritorno_Opzionale.ToString()
        Catch ex As Exception
            ' Log dell'errore
            _logService.Scrivi_LOG("ResxDatabaseManager.ElencoDuplicati", $"Errore durante la generazione dell'elenco dei duplicati: {ex.Message}", TipoMessaggio.Errore)
            Return ""
        End Try
    End Function


    Private Function RemoveCommonPath(fullPath As String) As String
        ' Determinare il percorso comune (ad esempio, directory radice) e rimuoverlo
        Dim commonPath As String = "percorso_comune_da_rimuovere" ' Modifica questo con il percorso comune appropriato
        Return fullPath.Replace(commonPath, "").TrimStart(New Char() {"\"c, "/"c})
    End Function


    'Public Function ElencoDuplicati(dt As DataTable) As String

    '    Dim Messaggio_di_Ritorno_Opzionale As New StringBuilder()

    '    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
    '        ' Intestazione
    '        Messaggio_di_Ritorno_Opzionale.AppendLine("🔍 Report Duplicati 🔍")
    '        Messaggio_di_Ritorno_Opzionale.AppendLine(New String("=", 100))
    '        Messaggio_di_Ritorno_Opzionale.AppendLine()

    '        ' Raggruppa i risultati per chiave
    '        Dim query = From row In dt.AsEnumerable()
    '                    Group row By Chiave = row.Field(Of String)("chiave") Into Group
    '                    Select Chiave, Dettagli = Group

    '        For Each gruppo In query
    '            ' Mostra la chiave e il conteggio dei file unici per quella chiave
    '            Dim conteggioPercorsi As Integer = gruppo.Dettagli.Count()
    '            Messaggio_di_Ritorno_Opzionale.AppendLine($"🔑 Chiave: '{gruppo.Chiave}' - 🔄 Ripetizioni Totali File: {conteggioPercorsi}")
    '            Messaggio_di_Ritorno_Opzionale.AppendLine(New String("-", 100))

    '            ' Calcolo della lunghezza massima dei percorsi per l'allineamento
    '            Dim maxPercorsoLength = gruppo.Dettagli.Max(Function(d) d.Field(Of String)("percorso_file_scansito").Length)

    '            ' Elenco dei percorsi unici dei file e il valore in cui la chiave è presente
    '            For Each dettaglio In gruppo.Dettagli
    '                Dim percorso As String = dettaglio("percorso_file_scansito").ToString()
    '                Dim valore As String = dettaglio("valore").ToString()
    '                Messaggio_di_Ritorno_Opzionale.AppendLine($"📁 Percorso File: {percorso.PadRight(maxPercorsoLength + 5)}| Valore: {valore}")
    '            Next
    '            Messaggio_di_Ritorno_Opzionale.AppendLine(New String("-", 100))
    '            Messaggio_di_Ritorno_Opzionale.AppendLine()
    '        Next

    '        ' Chiusura
    '        Messaggio_di_Ritorno_Opzionale.AppendLine(New String("=", 100))
    '        Messaggio_di_Ritorno_Opzionale.AppendLine("Fine Report Duplicati")
    '    Else
    '        ' Nessun duplicato trovato
    '        Messaggio_di_Ritorno_Opzionale.AppendLine("✅ Nessun duplicato trovato. Tutto in ordine!")
    '    End If

    '    Return Messaggio_di_Ritorno_Opzionale.ToString()
    'End Function
End Class
