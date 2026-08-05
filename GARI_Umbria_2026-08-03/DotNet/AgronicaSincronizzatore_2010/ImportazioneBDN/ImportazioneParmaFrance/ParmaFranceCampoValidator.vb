' ─────────────────────────────────────────────────────────────────────────────────
'  ValidazioneDinamicaCampi — modelli di output
' ─────────────────────────────────────────────────────────────────────────────────

''' <summary>
''' Stato della validazione per una singola riga Excel.
''' </summary>
Public Enum StatoValidazioneCampi
    ''' <summary>Tutti i campi obbligatori sono presenti e validi.</summary>
    Superata = 0
    ''' <summary>Almeno un campo obbligatorio è assente o non valido.</summary>
    Fallita = 1
End Enum

''' <summary>
''' Errore rilevato su un singolo campo durante la validazione di una riga.
''' </summary>
Public Class ErroreCampoValidazione
    ''' <summary>Chiave logica del campo nella matrice di validazione (es. "matricola_A").</summary>
    Public Property NomeCampo As String
    ''' <summary>Nome della colonna nel DataTable (es. "F1", "F4", "Stalla_Svezzamento").</summary>
    Public Property ColonnaDataTable As String
    ''' <summary>
    ''' Codice errore standardizzato.
    ''' Valori: MANDATORY_FIELD_MISSING | INVALID_DATE_FORMAT | INVALID_FIELD_VALUE.
    ''' </summary>
    Public Property CodiceErrore As String
    Public Property MessaggioErrore As String
End Class

''' <summary>
''' Risultato della validazione per una singola riga.
''' </summary>
Public Class RigaValidataResult
    ''' <summary>Numero di sequenza della riga Excel.</summary>
    Public Property NumeroSequenza As Integer
    ''' <summary>Matricola dell'animale (colonna A del file).</summary>
    Public Property Matricola As String
    ''' <summary>Stato complessivo della validazione.</summary>
    Public Property StatoValidazione As StatoValidazioneCampi
    ''' <summary>
    ''' Campi che hanno fallito la validazione.
    ''' </summary>
    Public Property CampiErrore As New List(Of ErroreCampoValidazione)
End Class

''' <summary>
''' Risultato aggregato della validazione sull'intero DataTable.
''' </summary>
Public Class ValidazioneCampiResult
    ''' <summary>DataTable con le sole righe che hanno superato la validazione.</summary>
    Public Property RigheValide As DataTable
    ''' <summary>Righe che hanno fallito la validazione, con dettaglio dei campi errati.</summary>
    Public Property RigheScartate As New List(Of RigaValidataResult)
    ''' <summary>Numero di righe valide.</summary>
    Public ReadOnly Property TotaleRigheValide As Integer
        Get
            Return If(RigheValide IsNot Nothing, RigheValide.Rows.Count, 0)
        End Get
    End Property
End Class

' ─────────────────────────────────────────────────────────────────────────────────
'  ValidazioneDinamicaCampi — implementazione
' ─────────────────────────────────────────────────────────────────────────────────

''' <summary>
''' Valida ogni riga rispetto alla matrice di validazione adattiva
''' costruita per la specie identificata.
'''
''' Per ogni campo OBBLIGATORIO: verifica che il valore non sia NULL/vuoto
''' e, se è un campo data, che sia una data valida.
''' Per ogni campo OPZIONALE: se compilato, applica validazione di formato minima.
''' Una riga con almeno un errore su campo obbligatorio è marcata come FALLITA.
'''
''' Nessuna persistenza: elaborazione interamente in memoria.
''' </summary>
Public Class ParmaFranceCampoValidator

    ' ── Codici errore standardizzati ────────────────────────────────────────────────
    Public Const MANDATORY_FIELD_MISSING As String = "MANDATORY_FIELD_MISSING"
    Public Const INVALID_DATE_FORMAT As String = "INVALID_DATE_FORMAT"
    Public Const INVALID_FIELD_VALUE As String = "INVALID_FIELD_VALUE"

    ' Chiavi logiche che identificano campi data nella matrice di validazione.
    Private Shared ReadOnly CampiData As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        "data_nascita_D", "data_partenza_I", "data_arrivo_J"
    }

    ''' <summary>
    ''' Processa l'intero DataTable, determinando per ogni riga
    ''' la specie dalla colonna "Specie" e costruendo la relativa
    ''' matrice di validazione tramite <see cref="ParmaFranceSpecieValidator.CostruisciMatrice"/>.
    '''
    ''' </summary>
    ''' <param name="dtRighe">
    ''' DataTable con colonna "Specie" (retrocompatibilità: se assente, specie trattata come Bovino).
    ''' </param>
    ''' <returns><see cref="ValidazioneCampiResult"/> con righe valide e righe scartate.</returns>
    ''' <exception cref="ArgumentNullException">dtRighe è Nothing.</exception>
    Public Shared Function ValidaDataTable(dtRighe As DataTable) As ValidazioneCampiResult
        If dtRighe Is Nothing Then Throw New ArgumentNullException(NameOf(dtRighe))

        Dim risultato As New ValidazioneCampiResult()
        Dim righeValide As New List(Of DataRow)

        For Each row As DataRow In dtRighe.Rows
            ' Ricava la specie dalla colonna "Specie" del DataTable.
            ' Default Bovino per retrocompatibilità se la colonna non è presente.
            Dim specie As SpecieAnimale = SpecieAnimale.Bovino
            If dtRighe.Columns.Contains("Specie") AndAlso Not IsDBNull(row("Specie")) Then
                [Enum].TryParse(Of SpecieAnimale)(CStr(row("Specie")), specie)
            End If

            ' Costruisce la matrice adattiva per la specie corrente.
            Dim matrice As MatriceValidazione = ParmaFranceSpecieValidator.CostruisciMatrice(specie)

            Dim rigaResult As RigaValidataResult = ValidaRiga(row, matrice)

            If rigaResult.StatoValidazione = StatoValidazioneCampi.Superata Then
                righeValide.Add(row)
            Else
                risultato.RigheScartate.Add(rigaResult)
            End If
        Next

        risultato.RigheValide = If(righeValide.Count > 0, righeValide.CopyToDataTable(), dtRighe.Clone())
        Return risultato
    End Function

    ''' <summary>
    ''' Valida una singola riga rispetto alla matrice di validazione.
    '''
    ''' Regole applicate per campo OBBLIGATORIO:
    ''' - Il valore non deve essere NULL né stringa vuota → MANDATORY_FIELD_MISSING.
    ''' - Se il campo è una data, deve essere una data valida → INVALID_DATE_FORMAT.
    '''
    ''' Regole applicate per campo OPZIONALE (solo se compilato):
    ''' - Se il campo è una data, deve essere una data valida → INVALID_DATE_FORMAT.
    ''' - Qualsiasi valore non-vuoto è accettato per campi non-data → nessun errore.
    '''
    ''' </summary>
    ''' <param name="row">Riga Excel da validare.</param>
    ''' <param name="matrice">Matrice di validazione per la specie corrente.</param>
    ''' <returns><see cref="RigaValidataResult"/> con stato e lista errori.</returns>
    Public Shared Function ValidaRiga(row As DataRow, matrice As MatriceValidazione) As RigaValidataResult
        If row Is Nothing Then Throw New ArgumentNullException(NameOf(row))
        If matrice Is Nothing Then Throw New ArgumentNullException(NameOf(matrice))

        Dim numSeq As Integer = If(IsDBNull(row("NumeroSequenza")), 0, CInt(row("NumeroSequenza")))
        Dim mat As String = If(IsDBNull(row("F1")), "", CStr(row("F1")).Trim())
        Dim risultato As New RigaValidataResult With {.NumeroSequenza = numSeq, .Matricola = mat}

        For Each kvp In matrice.Regole
            Dim chiave As String = kvp.Key
            Dim regola As RegolaCampo = kvp.Value
            Dim colonnaDataTable As String = regola.NomeCampo

            ' Colonna non presente nel DataTable: errore solo se il campo è obbligatorio.
            If Not row.Table.Columns.Contains(colonnaDataTable) Then
                If regola.Obbligatorio Then
                    risultato.CampiErrore.Add(New ErroreCampoValidazione With {
                        .NomeCampo = chiave,
                        .ColonnaDataTable = colonnaDataTable,
                        .CodiceErrore = MANDATORY_FIELD_MISSING,
                        .MessaggioErrore = "Il campo '" & chiave & "' è obbligatorio ma non è presente nel file."
                    })
                End If
                Continue For
            End If

            ' Leggi il valore grezzo dalla riga.
            Dim rawValue As Object = row(colonnaDataTable)
            Dim isVuoto As Boolean = IsDBNull(rawValue) OrElse String.IsNullOrWhiteSpace(CStr(rawValue))

            If regola.Obbligatorio Then
                ' ── Campo OBBLIGATORIO ──────────────────────────────────────────
                ' Il valore non deve essere NULL né stringa vuota.
                If isVuoto Then
                    risultato.CampiErrore.Add(New ErroreCampoValidazione With {
                        .NomeCampo = chiave,
                        .ColonnaDataTable = colonnaDataTable,
                        .CodiceErrore = MANDATORY_FIELD_MISSING,
                        .MessaggioErrore = "Il campo '" & chiave & "' è obbligatorio ma risulta vuoto."
                    })
                    Continue For
                End If

                ' Se campo data: verifica formato valido.
                If CampiData.Contains(chiave) AndAlso Not IsDataValida(rawValue) Then
                    risultato.CampiErrore.Add(New ErroreCampoValidazione With {
                        .NomeCampo = chiave,
                        .ColonnaDataTable = colonnaDataTable,
                        .CodiceErrore = INVALID_DATE_FORMAT,
                        .MessaggioErrore = "Il campo '" & chiave & "' contiene una data non valida: """ & CStr(rawValue).Trim() & """"
                    })
                End If
            Else
                ' ── Campo OPZIONALE: validazione minima solo se compilato ───────
                ' Se compilato e campo data: verifica formato.
                If Not isVuoto AndAlso CampiData.Contains(chiave) AndAlso Not IsDataValida(rawValue) Then
                    risultato.CampiErrore.Add(New ErroreCampoValidazione With {
                        .NomeCampo = chiave,
                        .ColonnaDataTable = colonnaDataTable,
                        .CodiceErrore = INVALID_DATE_FORMAT,
                        .MessaggioErrore = "Il campo '" & chiave & "' contiene una data non valida: """ & CStr(rawValue).Trim() & """"
                    })
                End If
            End If
        Next

        ' Un errore su un campo (obbligatorio o data opzionale) causa FALLITA.
        risultato.StatoValidazione = If(risultato.CampiErrore.Count > 0,
                                        StatoValidazioneCampi.Fallita,
                                        StatoValidazioneCampi.Superata)
        Return risultato
    End Function

    ''' <summary>
    ''' Verifica che un valore grezzo sia una data valida.
    ''' Accetta sia valori già tipizzati come <see cref="DateTime"/> (restituiti da OleDb)
    ''' sia stringhe in formato riconoscibile dalla funzione VB <c>IsDate</c>.
    ''' </summary>
    Private Shared Function IsDataValida(rawValue As Object) As Boolean
        If IsDBNull(rawValue) Then Return False
        If TypeOf rawValue Is DateTime Then Return True
        Return IsDate(CStr(rawValue).Trim())
    End Function

End Class
