Imports ClosedXML.Excel
Imports System.IO

' ─────────────────────────────────────────────────────────────────────────────────
'  EstrazioneParsingExcelParmaFrance — modelli di output
' ─────────────────────────────────────────────────────────────────────────────────

''' <summary>
''' Stato complessivo del processo di estrazione dal file Excel.
''' </summary>
Public Enum StatoEstrazione
    ''' <summary>Tutte le righe estratte senza errori.</summary>
    Completata = 0
    ''' <summary>Alcune righe scartate; quelle valide sono disponibili.</summary>
    ParzialmenteCompletata = 1
    ''' <summary>Nessuna riga valida estratta (file vuoto o errore strutturale).</summary>
    Fallita = 2
End Enum

''' <summary>
''' Descrive un errore riscontrato durante l'estrazione di una singola riga Excel.
''' </summary>
Public Class LetturaExcelErrore
    Public Property NumeroRiga As Integer
    Public Property Messaggio As String
    Public Property CodiceErrore As String
End Class

''' <summary>
''' Risultato dell'estrazione e parsing del file Excel ParmaFrance.
''' </summary>
Public Class LetturaExcelResult
    ''' <summary>DataTable con le righe valide, include la colonna NumeroSequenza.</summary>
    Public Property Righe As DataTable
    ''' <summary>Errori per-riga (righe vuote, colonne critiche assenti).</summary>
    Public Property Errori As New List(Of LetturaExcelErrore)
    ''' <summary>Stato complessivo dell'estrazione.</summary>
    Public Property Stato As StatoEstrazione
    ''' <summary>Numero di righe estratte con successo.</summary>
    Public ReadOnly Property TotaleRigheEstratte As Integer
        Get
            Return If(Righe IsNot Nothing, Righe.Rows.Count, 0)
        End Get
    End Property
End Class

''' <summary>
''' Legge un file Excel ParmaFrance dal percorso fisico, esegue il parsing delle righe
''' e restituisce un <see cref="LetturaExcelResult"/> con righe estratte, errori per-riga
''' e stato complessivo. Nessuna persistenza; elaborazione interamente in memoria.
'''
''' Mapping colonne Excel → DataTable:
'''   A=F1 (matricola, CRITICA), B=F2, C=F3 (sesso), D=F4 (data nascita),
'''   E=F5 (razza, raw — non validata qui; delegata a ParmaFranceRazzaResolver),
'''   F=F6, G=F7, I=F9, J=F10, N=F14,
'''   S=F19 (codice stalla, CRITICA), T=F20 (matricola madre, obbligatoria strutturale),
'''   U=Stalla_Svezzamento (opzionale, aggiunta come colonna vuota se assente).
'''
''' Colonne extra aggiunte ma non popolate in questa fase:
'''   Matricola (String), Stalla_Nascita (String) — placeholder per ParmaFranceImporter.
'''
''' </summary>
Public Class ParmaFranceExcelReader

    ' Struttura minima effettiva: S=colonna 19 (obbligatoria per specie) + T=colonna 20
    ' (matricola madre, obbligatoria strutturale). Stalla_Svezzamento (col 21) è opzionale.
    ' Colonne critiche per-riga (blocco riga): solo A (matricola) e S (codice stalla).
    ' E (razza) è raw e opzionale a questo livello — gestita da ParmaFranceRazzaResolver.
    Private Const NumColonneMinime As Integer = 20

    Private Const CodiceErroreRigaVuota As String = "EMPTY_ROW_DISCARDED"
    Private Const CodiceErroreParsingRiga As String = "ROW_PARSING_ERROR"

    ''' <summary>
    ''' Legge e parsa il file Excel dal percorso fisico indicato.
    ''' </summary>
    ''' <param name="filePath">Percorso fisico del file Excel (.xls / .xlsx).</param>
    ''' <returns>
    ''' <see cref="LetturaExcelResult"/> con DataTable delle righe valide,
    ''' lista errori per-riga e stato complessivo.
    ''' </returns>
    ''' <exception cref="FileNotFoundException">Il file non esiste al percorso indicato.</exception>
    ''' <exception cref="IOException">Accesso al file negato o errore di I/O.</exception>
    ''' <exception cref="Exception">
    ''' Il file non è un Excel valido oppure mancano le colonne critiche strutturali.
    ''' </exception>
    Public Shared Function LeggiFile(filePath As String) As LetturaExcelResult
        ' ── 1. Esistenza file ────────────────────────────────────────────────
        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException("File Excel non trovato: " & filePath, filePath)
        End If

        ' ── 2. Lettura tramite ClosedXML (non richiede Microsoft.ACE.OLEDB) ────────────────
        Dim dtRaw As New DataTable("fileXls")
        Try
            Using wb As New XLWorkbook(filePath)
                Dim ws As IXLWorksheet = wb.Worksheets.First()
                Dim lastRow As Integer = If(ws.LastRowUsed() IsNot Nothing, ws.LastRowUsed().RowNumber(), 0)
                Dim lastCol As Integer = If(ws.LastColumnUsed() IsNot Nothing, ws.LastColumnUsed().ColumnNumber(), 0)

                For col As Integer = 1 To lastCol
                    dtRaw.Columns.Add(New DataColumn())
                Next

                ' Salta la riga 1 (intestazione), equivalente a HDR=YES in OleDb
                For rowNum As Integer = 2 To lastRow
                    Dim dr As DataRow = dtRaw.NewRow()
                    For col As Integer = 1 To lastCol
                        Dim cell As IXLCell = ws.Cell(rowNum, col)
                        dr(col - 1) = If(cell.IsEmpty(), DBNull.Value, cell.Value)
                    Next
                    dtRaw.Rows.Add(dr)
                Next
            End Using
        Catch ex As UnauthorizedAccessException
            Throw New IOException("Accesso negato al file Excel: " & filePath, ex)
        End Try

        ' ── 3. Verifica struttura minima ───────────────────────────────────────────────────

        If dtRaw.Columns.Count < NumColonneMinime Then
            Throw New Exception(
                "Il file Excel non ha la struttura attesa: contiene " & dtRaw.Columns.Count & " colonne, ma ne sono richieste almeno " & NumColonneMinime & ".")
        End If

        ' ── 4. Rinomina colonne posizionali ────────────────────────────────────────────────
        Dim nomiBase() As String = {"F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9",
                                    "F10", "F11", "F12", "F13", "F14", "F15", "F16", "F17", "F18", "F19"}
        For i As Integer = 0 To nomiBase.Length - 1
            dtRaw.Columns(i).ColumnName = nomiBase(i)
        Next

        If dtRaw.Columns.Count > 19 Then
            dtRaw.Columns(19).ColumnName = "F20"
        Else
            ' Questo branch non dovrebbe mai essere raggiunto grazie al check NumColonneMinime=20.
            Throw New Exception("Struttura del file non valida: colonne insufficienti.")
        End If

        If dtRaw.Columns.Count > 20 Then
            dtRaw.Columns(20).ColumnName = "Stalla_Svezzamento"
        Else
            dtRaw.Columns.Add(New DataColumn("Stalla_Svezzamento", GetType(String)))
        End If

        dtRaw.Columns.Add(New DataColumn("Matricola", GetType(String)))
        dtRaw.Columns.Add(New DataColumn("Stalla_Nascita", GetType(String)))
        ' Placeholder popolati da ParmaFranceImporter durante la fase di persistenza.
        ' NumeroSequenza: identificatore univoco per riga nella sessione di import.
        dtRaw.Columns.Add(New DataColumn("NumeroSequenza", GetType(Integer)))

        ' ── 5. Iterazione righe: validazione per-riga e assegnazione sequenza ────────────────
        Dim risultato As New LetturaExcelResult()
        Dim righeValide As New List(Of DataRow)
        Dim numeroSequenza As Integer = 0
        ' HDR=YES: la riga 1 dell'Excel è il nome colonna (skipata da OleDb).
        ' Il primo DataRow corrisponde alla riga 2 dell'Excel.
        Dim excelRowNum As Integer = 1

        For Each row As DataRow In dtRaw.Rows
            excelRowNum += 1

            ' ── Riga vuota: scartata con warning ──────────────────────────────────────────
            If row.ItemArray.All(Function(f) IsDBNull(f) OrElse String.IsNullOrWhiteSpace(f.ToString())) Then
                risultato.Errori.Add(New LetturaExcelErrore With {
                    .NumeroRiga = excelRowNum,
                    .Messaggio = "Riga vuota scartata.",
                    .CodiceErrore = CodiceErroreRigaVuota
                })
                Continue For
            End If

            ' ── Colonne critiche: A (matricola) e S (codice stalla) ────────────────────────

            Dim valA As String = If(IsDBNull(row("F1")), "", CStr(row("F1")).Trim())
            Dim valS As String = If(IsDBNull(row("F19")), "", CStr(row("F19")).Trim())

            If String.IsNullOrWhiteSpace(valA) Then
                risultato.Errori.Add(New LetturaExcelErrore With {
                    .NumeroRiga = excelRowNum,
                    .Messaggio = "La matricola dell'animale (colonna A) è obbligatoria ma non è valorizzata.",
                    .CodiceErrore = CodiceErroreParsingRiga
                })
                Continue For
            End If

            If String.IsNullOrWhiteSpace(valS) Then
                risultato.Errori.Add(New LetturaExcelErrore With {
                    .NumeroRiga = excelRowNum,
                    .Messaggio = "Il codice stalla (colonna S) è obbligatorio ma non è valorizzato.",
                    .CodiceErrore = CodiceErroreParsingRiga
                })
                Continue For
            End If

            ' ── Riga valida: assegna numero di sequenza ────────────────────────────────────
            numeroSequenza += 1
            row("NumeroSequenza") = numeroSequenza
            righeValide.Add(row)
        Next

        ' ── 6. Costruzione risultato ───────────────────────────────────────────────────────
        If righeValide.Count = 0 Then
            risultato.Righe = dtRaw.Clone()
            risultato.Stato = StatoEstrazione.Fallita
            Return risultato
        End If

        risultato.Righe = righeValide.CopyToDataTable()
        risultato.Stato = If(risultato.Errori.Count = 0,
                             StatoEstrazione.Completata,
                             StatoEstrazione.ParzialmenteCompletata)
        Return risultato
    End Function

End Class
