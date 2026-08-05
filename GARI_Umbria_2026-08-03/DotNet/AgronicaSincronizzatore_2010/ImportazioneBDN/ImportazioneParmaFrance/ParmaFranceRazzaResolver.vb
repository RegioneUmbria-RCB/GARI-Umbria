Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreUtility

' ─────────────────────────────────────────────────────────────────────────────────
'  ParmaFranceRazzaResolver — eccezioni specializzate
' ─────────────────────────────────────────────────────────────────────────────────

''' <summary>
''' Eccezione: il valore predefinito razza non è configurato in Lista_Razze_Animali.
''' Indica un errore di setup (tabella non popolata), non un errore di runtime.
''' </summary>
Public Class DefaultBreedNotConfiguredException
    Inherits Exception

    ''' <summary>
    ''' Inizializza l'eccezione con il messaggio descrittivo dell'errore di configurazione.
    ''' </summary>
    Public Sub New(message As String)
        MyBase.New(message)
    End Sub
End Class

''' <summary>
''' Eccezione: errore durante la query alla tabella Codifica_BDN_RazzeAnimali.
''' </summary>
Public Class BDNLookupException
    Inherits Exception

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub
End Class

''' <summary>
''' Eccezione: query BDN supera il timeout configurato.
''' Quando sollevata, il chiamante applica il fallback al default senza bloccare la riga.
''' </summary>
Public Class BDNTimeoutException
    Inherits BDNLookupException

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub
End Class

' ─────────────────────────────────────────────────────────────────────────────────
'  ParmaFranceRazzaResolver — modelli di output
' ─────────────────────────────────────────────────────────────────────────────────

''' <summary>
''' Stato del processo di lookup razza per una singola riga.
''' </summary>
Public Enum StatoLookupRazza
    ''' <summary>Valore in colonna E numerico — gestito dalla procedura legacy RicavaRazzaCapoAnimale dell'importer.</summary>
    NUMERIC_DIRECT = 0
    ''' <summary>Valore non-numerico trovato in Codifica_BDN_RazzeAnimali — RAZ_COD scritto in RazzaRisolta.</summary>
    BDN_FOUND = 1
    ''' <summary>Valore non-numerico non trovato in BDN o lookup fallito; applicato default da Lista_Razze_Animali.</summary>
    BDN_NOT_FOUND = 2
    ''' <summary>Campo E assente o vuoto; applicato default da Lista_Razze_Animali.</summary>
    DEFAULT_APPLIED = 3
End Enum

''' <summary>
''' Warning prodotto per un lookup razza non risolto via BDN o per campo razza assente.
''' Non è bloccante: la riga continua l'importazione con la razza default.
''' </summary>
Public Class WarningRazzaResolver
    ''' <summary>Numero di sequenza per tracciamento.</summary>
    Public Property NumeroSequenza As Integer
    ''' <summary>Matricola dell'animale (colonna A del file).</summary>
    Public Property Matricola As String
    ''' <summary>Valore grezzo della colonna E (razza) prima della risoluzione.</summary>
    Public Property RazzaOriginale As String
    ''' <summary>Codice razza applicato definitivamente (RAZ_COD Agronica).</summary>
    Public Property RazzaApplicata As Integer
    ''' <summary>Descrizione della razza applicata (per il report post-import).</summary>
    Public Property RazzaApplicataDescrizione As String
    ''' <summary>Stato del lookup che ha originato il warning.</summary>
    Public Property LookupStatus As StatoLookupRazza
    ''' <summary>
    ''' Motivo dell'applicazione del default.
    ''' Valori possibili: "BDN_NOT_FOUND" | "FIELD_ABSENT".
    ''' </summary>
    Public Property DefaultReason As String
    ''' <summary>Messaggio leggibile per il report post-import.</summary>
    Public Property Messaggio As String
End Class

''' <summary>
''' Risultato aggregato del resolver per l'intero DataTable.
''' Arricchisce ogni riga con le colonne RazzaRisolta, RazzaLookupStatus,
''' RazzaUsedDefault, RazzaDefaultReason per uso da parte di ParmaFranceImporter.
''' </summary>
Public Class RisoluzioneRazzaResult
    ''' <summary>
    ''' DataTable identico all'input con le seguenti colonne aggiuntive:
    ''' - RazzaRisolta (Object/Integer, DBNull per NUMERIC_DIRECT): codice RAZ_COD Agronica risolto.
    '''   Per NUMERIC_DIRECT è DBNull — la procedura legacy RicavaRazzaCapoAnimale dell'importer gestisce F5.
    ''' - RazzaLookupStatus (String): valore dell'enum StatoLookupRazza.
    ''' - RazzaUsedDefault (Boolean): True se è stato applicato il valore predefinito.
    ''' - RazzaDefaultReason (String): "BDN_NOT_FOUND" | "FIELD_ABSENT" | stringa vuota.
    ''' </summary>
    Public Property RigheArricchite As DataTable

    ''' <summary>
    ''' Warning prodotti per le righe dove è stato applicato il valore predefinito di razza.
    ''' </summary>
    Public Property Warning As New List(Of WarningRazzaResolver)
End Class

' ─────────────────────────────────────────────────────────────────────────────────
'  ParmaFranceRazzaResolver — logica di risoluzione
' ─────────────────────────────────────────────────────────────────────────────────

''' <summary>
''' Risolve il codice razza definitivo per ogni riga del DataTable validato,
''' gestendo tre scenari distinti per il valore della colonna E (razza):
'''   1. NUMERIC_DIRECT: valore già numerico, delega alla procedura legacy dell'importer.
'''   2. BDN_FOUND / BDN_NOT_FOUND: valore non-numerico, lookup in Codifica_BDN_RazzeAnimali.
'''   3. DEFAULT_APPLIED: campo assente/vuoto, valore predefinito da Lista_Razze_Animali.
'''
''' Richiede che la colonna "Specie" (SpecieAnimale enum) sia stata aggiunta al DataTable.
''' </summary>
Public Class ParmaFranceRazzaResolver

    Private ReadOnly _parametriServer As AgronicaCoreParametri

    ' Cache lookup BDN: CODICE stringa → RAZ_COD (Nothing = non trovato in BDN)
    Private ReadOnly _bdnCache As New Dictionary(Of String, Nullable(Of Integer))(StringComparer.OrdinalIgnoreCase)

    ' Cache lookup per descrizione RAZ_DES (case-insensitive): stringa → RAZ_COD (Nothing = non trovato)
    Private ReadOnly _desCache As New Dictionary(Of String, Nullable(Of Integer))(StringComparer.OrdinalIgnoreCase)

    ' Cache default razza per specie: SpecieAnimale → (RAZ_COD, RAZ_DES)
    Private ReadOnly _defaultRazzaCache As New Dictionary(Of SpecieAnimale, Tuple(Of Integer, String))

    ''' <summary>
    ''' Inizializza il resolver con i parametri di connessione al DB Agronica.
    ''' </summary>
    ''' <param name="parametriServer">Parametri di connessione; non può essere Nothing.</param>
    ''' <exception cref="ArgumentNullException">parametriServer è Nothing.</exception>
    Public Sub New(parametriServer As AgronicaCoreParametri)
        If parametriServer Is Nothing Then Throw New ArgumentNullException(NameOf(parametriServer))
        _parametriServer = parametriServer
    End Sub

    ''' <summary>
    ''' Elabora l'intero DataTable aggiungendo le colonne di risoluzione razza a ogni riga.
    ''' </summary>
    ''' <param name="dtRighe">
    ''' DataTable validato; deve contenere le colonne "F5", "NumeroSequenza"
    ''' e preferibilmente "Specie" (SpecieAnimale enum).
    ''' </param>
    ''' <returns>
    ''' RisoluzioneRazzaResult con il DataTable arricchito e i warning per le righe
    ''' in cui è stato applicato il valore predefinito di razza.
    ''' </returns>
    ''' <exception cref="ArgumentNullException">dtRighe è Nothing.</exception>
    ''' <exception cref="DefaultBreedNotConfiguredException">
    ''' Nessuna razza default configurata in Lista_Razze_Animali
    ''' per la specie richiesta. Questo è un errore di setup, non di runtime.
    ''' </exception>
    Public Function RisolviDataTable(dtRighe As DataTable) As RisoluzioneRazzaResult
        If dtRighe Is Nothing Then Throw New ArgumentNullException(NameOf(dtRighe))

        AggiungiColonneRisoluzioneRazza(dtRighe)

        Dim result As New RisoluzioneRazzaResult With {.RigheArricchite = dtRighe}

        For Each row As DataRow In dtRighe.Rows
            Dim seqNum As Integer = If(dtRighe.Columns.Contains("NumeroSequenza") AndAlso
                                       Not IsDBNull(row("NumeroSequenza")),
                                       CInt(row("NumeroSequenza")), 0)
            Dim matricola As String = If(IsDBNull(row("F1")), "", CStr(row("F1")).Trim())

            Dim specie As SpecieAnimale = SpecieAnimale.Suino
            If dtRighe.Columns.Contains("Specie") AndAlso Not IsDBNull(row("Specie")) Then
                [Enum].TryParse(Of SpecieAnimale)(CStr(row("Specie")), specie)
            End If

            Dim rawF5 As String = If(IsDBNull(row("F5")), "", CStr(row("F5")).Trim())

            ' ── Scenario 3: Campo E assente o vuoto — applica il valore predefinito ─────────
            If String.IsNullOrEmpty(rawF5) Then
                Dim def As Tuple(Of Integer, String) = GetDefaultRazza(specie)
                row("RazzaRisolta") = def.Item1
                row("RazzaLookupStatus") = StatoLookupRazza.DEFAULT_APPLIED.ToString()
                row("RazzaUsedDefault") = True
                row("RazzaDefaultReason") = "FIELD_ABSENT"
                result.Warning.Add(New WarningRazzaResolver With {
                    .NumeroSequenza = seqNum,
                    .Matricola = matricola,
                    .RazzaOriginale = rawF5,
                    .RazzaApplicata = def.Item1,
                    .RazzaApplicataDescrizione = def.Item2,
                    .LookupStatus = StatoLookupRazza.DEFAULT_APPLIED,
                    .DefaultReason = "FIELD_ABSENT",
                    .Messaggio = "Codice razza assente nel file: verrà utilizzata la razza predefinita (" & def.Item2 & ")"
                })
                Continue For
            End If

            ' ── Scenario 1: Valore numerico — delegato a RicavaRazzaCapoAnimale dell'importer ──
            If IsNumeric(rawF5) Then
                row("RazzaRisolta") = DBNull.Value   ' RicavaRazzaCapoAnimale dell'importer usa F5 direttamente
                row("RazzaLookupStatus") = StatoLookupRazza.NUMERIC_DIRECT.ToString()
                row("RazzaUsedDefault") = False
                row("RazzaDefaultReason") = ""
                Continue For
            End If

            ' ── Scenario 2: Valore non-numerico — lookup sincrono su Codifica_BDN_RazzeAnimali ──
            Dim lookupResult As Nullable(Of Integer) = Nothing
            Dim lookupOk As Boolean = True
            Try
                lookupResult = RisolviRazzaBDN(rawF5)
            Catch ex As BDNLookupException
                ' Lookup BDN fallito — applica fallback al default senza bloccare la riga.
                lookupOk = False
                result.Warning.Add(New WarningRazzaResolver With {
                    .NumeroSequenza = seqNum,
                    .Matricola = matricola,
                    .RazzaOriginale = rawF5,
                    .LookupStatus = StatoLookupRazza.BDN_NOT_FOUND,
                    .DefaultReason = "BDN_NOT_FOUND",
                    .Messaggio = "Impossibile riconoscere il codice razza '" & rawF5 & "': verrà utilizzata la razza predefinita."
                })
            End Try

            If lookupOk AndAlso lookupResult.HasValue Then
                ' BDN_FOUND
                row("RazzaRisolta") = lookupResult.Value
                row("RazzaLookupStatus") = StatoLookupRazza.BDN_FOUND.ToString()
                row("RazzaUsedDefault") = False
                row("RazzaDefaultReason") = ""
            Else
                ' BDN_NOT_FOUND o lookup fallito → prova match per RAZ_DES (case-insensitive)
                Dim desByDes As Nullable(Of Integer) = RisolviRazzaPerDescrizione(rawF5, specie)
                If desByDes.HasValue Then
                    row("RazzaRisolta") = desByDes.Value
                    row("RazzaLookupStatus") = StatoLookupRazza.BDN_FOUND.ToString()
                    row("RazzaUsedDefault") = False
                    row("RazzaDefaultReason") = ""
                Else
                    ' Nessun match nemmeno per descrizione → applica default
                    Dim def As Tuple(Of Integer, String) = GetDefaultRazza(specie)
                    row("RazzaRisolta") = def.Item1
                    row("RazzaLookupStatus") = StatoLookupRazza.BDN_NOT_FOUND.ToString()
                    row("RazzaUsedDefault") = True
                    row("RazzaDefaultReason") = "BDN_NOT_FOUND"

                    ' Aggiungi warning solo se lookup_ok (cioè non già aggiunto per errore di accesso)
                    If lookupOk Then
                        result.Warning.Add(New WarningRazzaResolver With {
                            .NumeroSequenza = seqNum,
                            .Matricola = matricola,
                            .RazzaOriginale = rawF5,
                            .RazzaApplicata = def.Item1,
                            .RazzaApplicataDescrizione = def.Item2,
                            .LookupStatus = StatoLookupRazza.BDN_NOT_FOUND,
                            .DefaultReason = "BDN_NOT_FOUND",
                            .Messaggio = "Codice razza '" & rawF5 & "' non trovato: verrà utilizzata la razza predefinita (" & def.Item2 & ")"
                        })
                    Else
                        ' Aggiorna il warning già aggiunto con i dati del default applicato
                        Dim lastW = result.Warning.LastOrDefault()
                        If lastW IsNot Nothing Then
                            lastW.RazzaApplicata = def.Item1
                            lastW.RazzaApplicataDescrizione = def.Item2
                            lastW.Messaggio &= " Razza predefinita applicata: " & def.Item2 & "."
                        End If
                    End If
                End If
            End If
        Next

        Return result
    End Function

    ' ─── Helpers privati ────────────────────────────────────────────────────────

    ''' <summary>
    ''' Aggiunge le colonne di risoluzione razza al DataTable se non già presenti (operazione idempotente).
    ''' </summary>
    Private Shared Sub AggiungiColonneRisoluzioneRazza(dt As DataTable)
        If Not dt.Columns.Contains("RazzaRisolta") Then dt.Columns.Add("RazzaRisolta", GetType(Object))
        If Not dt.Columns.Contains("RazzaLookupStatus") Then dt.Columns.Add("RazzaLookupStatus", GetType(String))
        If Not dt.Columns.Contains("RazzaUsedDefault") Then dt.Columns.Add("RazzaUsedDefault", GetType(Boolean))
        If Not dt.Columns.Contains("RazzaDefaultReason") Then dt.Columns.Add("RazzaDefaultReason", GetType(String))
    End Sub

    ''' <summary>
    ''' Esegue il lookup di un codice razza non-numerico nella tabella Codifica_BDN_RazzeAnimali
    ''' filtrando per Sistema_Cod = BDN e Codice = codiceStringa.
    ''' Il risultato è cachato per sessione per evitare query ripetute sullo stesso codice.
    ''' </summary>
    ''' <param name="codiceStringa">Valore grezzo di colonna E (es. "Landrace").</param>
    ''' <returns>RAZ_COD Agronica se trovato; Nothing se nessun record corrisponde.</returns>
    ''' <exception cref="BDNLookupException">Errore di accesso al DB durante la query.</exception>
    ''' <exception cref="BDNTimeoutException">Timeout durante la query BDN.</exception>
    Private Function RisolviRazzaBDN(codiceStringa As String) As Nullable(Of Integer)
        If _bdnCache.ContainsKey(codiceStringa) Then Return _bdnCache(codiceStringa)

        Try
            Dim razzeR As New Codifica_BDN_RazzeAnimali()
            ' .ToString() restituisce il NOME dell'enum ("BDN"), ma il DB memorizza il valore numerico ("3").
            Dim sistCod As String = CInt(enum_Esportazioni_Sistema_Cod.BDN).ToString()
            Dim dtBDN As DataTable = razzeR.leggi(_parametriServer,
                                                   Spe_Id:="", Razza_Id:="",
                                                   Gen_Cod:="", Spe_Cod:="", Raz_Cod:="",
                                                   Sistema_Cod:=sistCod,
                                                   Codice:=codiceStringa)

            Dim risultato As Nullable(Of Integer) = Nothing
            If dtBDN IsNot Nothing AndAlso dtBDN.Rows.Count > 0 Then
                risultato = CInt(dtBDN.Rows(0)("RAZ_COD"))
            End If

            _bdnCache(codiceStringa) = risultato
            Return risultato

        Catch ex As TimeoutException
            ' BDNTimeoutException — il chiamante applica il fallback al default
            _bdnCache(codiceStringa) = Nothing
            Throw New BDNTimeoutException("Timeout lookup BDN per codice '" & codiceStringa & "': " & ex.Message)
        Catch ex As Exception
            ' BDNLookupException
            _bdnCache(codiceStringa) = Nothing
            Throw New BDNLookupException("Errore lookup BDN per codice '" & codiceStringa & "': " & ex.Message, ex)
        End Try
    End Function

    ''' <summary>
    ''' Cerca una razza in Lista_Razze_Animali confrontando RAZ_DES in modo case-insensitive
    ''' con il valore grezzo del file. Restituisce RAZ_COD se trovato, Nothing altrimenti.
    ''' Il risultato è cachato per sessione.
    ''' </summary>
    Private Function RisolviRazzaPerDescrizione(descrizione As String, specie As SpecieAnimale) As Nullable(Of Integer)
        If _desCache.ContainsKey(descrizione) Then Return _desCache(descrizione)

        Dim efConnString As String = New Gias_EF_Utility().GetEntityConnectionString(_parametriServer.StringaConnessione)
        Using ctx As New Gias_DeveloperServer_Entities(efConnString)
            Dim keyword As String = If(specie = SpecieAnimale.Suino, "SUINO", "BOVINO")
            Dim specieRec = ctx.Lista_Specie_Animali.
                FirstOrDefault(Function(s) s.SPE_DES.ToUpper() = keyword)
            If specieRec Is Nothing Then
                specieRec = ctx.Lista_Specie_Animali.
                    OrderBy(Function(s) s.SPE_COD).
                    FirstOrDefault(Function(s) s.SPE_DES.ToUpper().Contains(keyword))
            End If

            Dim risultato As Nullable(Of Integer) = Nothing
            If specieRec IsNot Nothing Then
                Dim descUpper As String = descrizione.ToUpper()
                Dim razzaRec = ctx.Lista_Razze_Animali.
                    FirstOrDefault(Function(r) r.GEN_COD = specieRec.GEN_COD AndAlso
                                               r.SPE_COD = specieRec.SPE_COD AndAlso
                                               r.RAZ_DES.ToUpper() = descUpper)
                If razzaRec IsNot Nothing Then risultato = razzaRec.RAZ_COD
            End If

            _desCache(descrizione) = risultato
            Return risultato
        End Using
    End Function

    ''' <summary>
    ''' Recupera il codice e la descrizione della razza predefinita per la specie indicata
    ''' tramite Lista_Razze_Animali (prima razza per RAZ_COD, GEN_COD e SPE_COD della specie).
    ''' Il risultato è cachato per sessione.
    ''' </summary>
    ''' <returns>Tuple(Of Integer, String) = (RAZ_COD, RAZ_DES).</returns>
    ''' <exception cref="DefaultBreedNotConfiguredException">
    ''' Specie non trovata in Lista_Specie_Animali o nessuna razza configurata in Lista_Razze_Animali.
    ''' Indica un errore di setup che blocca l'intera elaborazione.
    ''' </exception>
    Private Function GetDefaultRazza(specie As SpecieAnimale) As Tuple(Of Integer, String)
        If _defaultRazzaCache.ContainsKey(specie) Then Return _defaultRazzaCache(specie)

        Dim efConnString As String = New Gias_EF_Utility().GetEntityConnectionString(_parametriServer.StringaConnessione)
        Using ctx As New Gias_DeveloperServer_Entities(efConnString)
            ' Risolve SPE_COD e GEN_COD della specie tramite Lista_Specie_Animali,
            ' senza dipendere da valori numerici specifici dell'installazione.
            Dim keyword As String = If(specie = SpecieAnimale.Suino, "SUINO", "BOVINO")
            ' Exact match first; fallback to Contains if not found (data quality guard).
            Dim specieRec = ctx.Lista_Specie_Animali.
                FirstOrDefault(Function(s) s.SPE_DES.ToUpper() = keyword)
            If specieRec Is Nothing Then
                specieRec = ctx.Lista_Specie_Animali.
                    OrderBy(Function(s) s.SPE_COD).
                    FirstOrDefault(Function(s) s.SPE_DES.ToUpper().Contains(keyword))
            End If

            If specieRec Is Nothing Then
                Throw New DefaultBreedNotConfiguredException(
                    "DefaultBreedNotConfiguredException: specie '" & specie.ToString() &
                    "' (keyword: '" & keyword & "') non trovata in Lista_Specie_Animali. " &
                    "Verificare che la tabella contenga il record corretto.")
            End If

            ' Prima razza per RAZ_COD crescente = default più generico della specie
            Dim razzaRec = ctx.Lista_Razze_Animali.
                Where(Function(r) r.GEN_COD = specieRec.GEN_COD AndAlso r.SPE_COD = specieRec.SPE_COD).
                OrderBy(Function(r) r.RAZ_COD).
                FirstOrDefault()

            If razzaRec Is Nothing Then
                Throw New DefaultBreedNotConfiguredException(
                    "DefaultBreedNotConfiguredException: nessuna razza configurata per la specie '" &
                    specieRec.SPE_DES & "' in Lista_Razze_Animali " &
                    "(GEN_COD=" & specieRec.GEN_COD & ", SPE_COD=" & specieRec.SPE_COD & "). " &
                    "Configurare almeno una razza predefinita.")
            End If

            Dim def As New Tuple(Of Integer, String)(razzaRec.RAZ_COD, razzaRec.RAZ_DES)
            _defaultRazzaCache(specie) = def
            Return def
        End Using
    End Function

End Class
