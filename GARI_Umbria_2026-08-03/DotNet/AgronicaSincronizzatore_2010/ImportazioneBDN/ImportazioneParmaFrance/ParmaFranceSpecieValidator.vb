Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

''' <summary>
''' Specie animale identificata tramite il codice stalla (colonna S).
''' </summary>
Public Enum SpecieAnimale
	''' <summary>Specie suina: razza, certificato intra, stalla svezzamento sono OPZIONALI.</summary>
	Suino = 0
	''' <summary>Specie bovina: retrocompatibilità, tutti i campi rimangono OBBLIGATORI.</summary>
	Bovino = 1
	''' <summary>Altra specie non classificata come Suino o Bovino.</summary>
	Altra = 2
End Enum

''' <summary>
''' Regola di validazione per un singolo campo della riga Excel.
''' </summary>
Public Class RegolaCampo
	''' <summary>Nome della colonna nel DataTable (es. "F1", "F5", "Stalla_Svezzamento").</summary>
	Public Property NomeCampo As String
	''' <summary>True se il campo è obbligatorio per la specie corrente.</summary>
	Public Property Obbligatorio As Boolean
End Class

''' <summary>
''' Matrice di validazione adattiva per specie: specifica l'obbligatorietà di ogni campo.
''' </summary>
Public Class MatriceValidazione

	Private ReadOnly _regole As Dictionary(Of String, RegolaCampo)

	Public Sub New(regole As Dictionary(Of String, RegolaCampo))
		If regole Is Nothing Then Throw New ArgumentNullException(NameOf(regole))
		_regole = regole
	End Sub

	''' <summary>
	''' Restituisce True se il campo identificato da <paramref name="chiaveCampo"/> è obbligatorio.
	''' </summary>
	''' <param name="chiaveCampo">Chiave logica del campo (es. "razza_E", "matricola_A").</param>
	Public Function IsObbligatorio(chiaveCampo As String) As Boolean
		Dim regola As RegolaCampo = Nothing
		If _regole.TryGetValue(chiaveCampo, regola) Then Return regola.Obbligatorio
		Return False
	End Function

	''' <summary>Accesso in sola lettura all'insieme delle regole per campo.</summary>
	Public ReadOnly Property Regole As IReadOnlyDictionary(Of String, RegolaCampo)
		Get
			Return _regole
		End Get
	End Property

End Class

''' <summary>
''' Errore bloccante rilevato durante il riconoscimento della specie per una singola riga.
''' </summary>
Public Class RiconoscimentoSpecieErrore
	''' <summary>Numero di sequenza della riga Excel.</summary>
	Public Property NumeroSequenza As Integer
	''' <summary>Matricola dell'animale (colonna A del file).</summary>
	Public Property Matricola As String
	''' <summary>Valore letto dalla colonna S (F19).</summary>
	Public Property CodiceStalla As String
	Public Property Messaggio As String
	Public Property CodiceErrore As String
End Class

''' <summary>
''' Risultato del processo di riconoscimento specie: righe il cui codice stalla è stato riconosciuto,
''' con colonna "Specie" aggiunta, e lista delle righe rifiutate per codice stalla sconosciuto.
''' </summary>
Public Class ValidazioneSpecieResult
	''' <summary>DataTable delle righe la cui specie è stata riconosciuta. Include la colonna "Specie".</summary>
	Public Property RigheValide As DataTable
	''' <summary>Errori bloccanti per riga (codice stalla non trovato in mappatura).</summary>
	Public Property Errori As New List(Of RiconoscimentoSpecieErrore)
	''' <summary>Numero di righe valide dopo il riconoscimento specie.</summary>
	Public ReadOnly Property TotaleRigheValide As Integer
		Get
			Return If(RigheValide IsNot Nothing, RigheValide.Rows.Count, 0)
		End Get
	End Property
End Class

''' <summary>
''' Identifica la specie animale dal codice stalla (F19)
''' tramite una mappatura configurata esternamente e costruisce la matrice di validazione
''' adattiva per la specie identificata.
'''
''' Nessuna persistenza: elaborazione interamente in memoria.
''' La mappatura codice stalla → specie è fornita tramite dependency injection al costruttore.
''' </summary>
Public Class ParmaFranceSpecieValidator

	' ── Codici errore ──────────────────────────────────────────────────────────
	Private Const CodiceStallaNonTrovata As String = "STALL_CODE_NOT_FOUND"

	Private ReadOnly _mappatura As IReadOnlyDictionary(Of String, SpecieAnimale)

	''' <summary>
	''' Costruisce il validatore con la mappatura codice stalla → specie fornita dall'esterno.
	''' </summary>
	''' <param name="mappaturaCodiceStalla">
	''' Dizionario (CodiceStalla → SpecieAnimale) caricato dalla fonte di configurazione.
	''' Deve contenere almeno un elemento.
	''' </param>
	''' <exception cref="ArgumentNullException">mappaturaCodiceStalla è Nothing.</exception>
	''' <exception cref="Exception">mappaturaCodiceStalla è vuota (InvalidMappingException).</exception>
	Public Sub New(mappaturaCodiceStalla As Dictionary(Of String, SpecieAnimale))
		If mappaturaCodiceStalla Is Nothing Then
			Throw New ArgumentNullException(NameOf(mappaturaCodiceStalla))
		End If
		If mappaturaCodiceStalla.Count = 0 Then
			Throw New Exception(
				"InvalidMappingException: la mappatura codice stalla → specie è vuota. " &
				"Verificare la tabella di configurazione.")
		End If
		_mappatura = mappaturaCodiceStalla
	End Sub

	''' <summary>
	''' Processa tutte le righe del DataTable in input.
	''' Per ogni riga:
	''' - Legge F19 (codice stalla) e cerca la specie nella mappatura.
	''' - Se non trovata: riga rifiutata con errore bloccante STALL_CODE_NOT_FOUND.
	''' - Se trovata: aggiunge colonna "Specie" e include la riga tra quelle valide.
	''' </summary>
	''' <param name="dtRighe">DataTable delle righe da validare (non può essere Nothing).</param>
	''' <returns>
	''' <see cref="ValidazioneSpecieResult"/> con le righe valide e gli errori bloccanti per riga.
	''' </returns>
	''' <exception cref="ArgumentNullException">dtRighe è Nothing.</exception>
	Public Function Valida(dtRighe As DataTable) As ValidazioneSpecieResult
		If dtRighe Is Nothing Then Throw New ArgumentNullException(NameOf(dtRighe))

		' Aggiunge la colonna "Specie" al DataTable se non presente.
		If Not dtRighe.Columns.Contains("Specie") Then
			dtRighe.Columns.Add(New DataColumn("Specie", GetType(String)))
		End If

		Dim risultato As New ValidazioneSpecieResult()
		Dim righeValide As New List(Of DataRow)

		For Each row As DataRow In dtRighe.Rows
			Dim codiceStalla As String = If(IsDBNull(row("F19")), "", CStr(row("F19")).Trim())
			Dim numSeq As Integer = If(IsDBNull(row("NumeroSequenza")), 0, CInt(row("NumeroSequenza")))

			Dim specie As SpecieAnimale
			If Not _mappatura.TryGetValue(codiceStalla, specie) Then
				' Codice stalla non trovato — riga rifiutata (STALL_CODE_NOT_FOUND).
				Dim mat As String = If(IsDBNull(row("F1")), "", CStr(row("F1")).Trim())
				risultato.Errori.Add(New RiconoscimentoSpecieErrore With {
					.NumeroSequenza = numSeq,
					.Matricola = mat,
					.CodiceStalla = codiceStalla,
					.Messaggio = "La stalla '" & codiceStalla & "' non è riconosciuta nel sistema: l'animale verrà scartato.",
					.CodiceErrore = CodiceStallaNonTrovata
				})
				Continue For
			End If

			row("Specie") = specie.ToString()
			righeValide.Add(row)
		Next

		risultato.RigheValide = If(righeValide.Count > 0, righeValide.CopyToDataTable(), dtRighe.Clone())
		Return risultato
	End Function

	''' <summary>
	''' Costruisce la matrice di validazione adattiva per la specie indicata.
	''' - Per Suini: razza (E, F, G), certificato intra (N), stalla svezzamento (U) sono OPZIONALI.
	''' - Per Bovini: tutti i campi rimangono OBBLIGATORI (retrocompatibilità).
	''' - Campi sempre OBBLIGATORI: matricola (A), azienda nascita (B), sesso (C), data nascita (D),
	'''   date movimento (I, J), stalla italiana (S).
	''' </summary>
	''' <param name="specie">Specie animale identificata per la riga corrente.</param>
	''' <returns>
	''' <see cref="MatriceValidazione"/> con obbligatorietà per campo.
	''' </returns>
	Public Shared Function CostruisciMatrice(specie As SpecieAnimale) As MatriceValidazione
		Dim eOpzionale As Boolean = (specie = SpecieAnimale.Suino)

		Dim regole As New Dictionary(Of String, RegolaCampo) From {
			{"matricola_A", New RegolaCampo With {.NomeCampo = "F1", .Obbligatorio = True}},
			{"azienda_nascita_B", New RegolaCampo With {.NomeCampo = "F2", .Obbligatorio = True}},
			{"sesso_C", New RegolaCampo With {.NomeCampo = "F3", .Obbligatorio = True}},
			{"data_nascita_D", New RegolaCampo With {.NomeCampo = "F4", .Obbligatorio = True}},
			{"razza_E", New RegolaCampo With {.NomeCampo = "F5", .Obbligatorio = Not eOpzionale}},
			{"razza_F", New RegolaCampo With {.NomeCampo = "F6", .Obbligatorio = Not eOpzionale}},
			{"razza_G", New RegolaCampo With {.NomeCampo = "F7", .Obbligatorio = Not eOpzionale}},
			{"data_partenza_I", New RegolaCampo With {.NomeCampo = "F9", .Obbligatorio = True}},
			{"data_arrivo_J", New RegolaCampo With {.NomeCampo = "F10", .Obbligatorio = True}},
			{"certificato_intra_N", New RegolaCampo With {.NomeCampo = "F14", .Obbligatorio = Not eOpzionale}},
			{"stalla_italiana_S", New RegolaCampo With {.NomeCampo = "F19", .Obbligatorio = True}},
			{"stalla_svezzamento_U", New RegolaCampo With {.NomeCampo = "Stalla_Svezzamento", .Obbligatorio = Not eOpzionale}}
		}

		Return New MatriceValidazione(regole)
	End Function

	''' <summary>
	''' Carica la mappatura codice stalla → specie dal database tramite EF e DAL già usati nel progetto.
	''' Risolve SPE_COD → SpecieAnimale leggendo SPE_DES da Lista_Specie_Animali,
	''' senza dipendere da valori interi hardcoded specifici dell'installazione.
	''' </summary>
	''' <param name="codiciStalla">
	''' Insieme dei codici stalla (colonna S / F19) distinti presenti nel file Excel corrente.
	''' Passare i soli codici rilevanti per evitare query inutili.
	''' </param>
	''' <param name="parametriServer">Parametri di connessione al DB Agronica.</param>
	''' <returns>Dizionario (BDN_Codice_Azienda → SpecieAnimale) pronto per il costruttore.</returns>
	''' <exception cref="ArgumentNullException">codiciStalla o parametriServer è Nothing.</exception>
	''' <exception cref="Exception">Errore di accesso al DB (MappingAccessException).</exception>
	Public Shared Function CaricaMappaturaDaDb(
			codiciStalla As IEnumerable(Of String),
			parametriServer As AgronicaCoreParametri) As Dictionary(Of String, SpecieAnimale)

		If codiciStalla Is Nothing Then Throw New ArgumentNullException(NameOf(codiciStalla))
		If parametriServer Is Nothing Then Throw New ArgumentNullException(NameOf(parametriServer))

		Dim mapping As New Dictionary(Of String, SpecieAnimale)(StringComparer.OrdinalIgnoreCase)
		Dim stallaR As New Stalla_R()

		Dim efConnString As String = New Gias_EF_Utility().GetEntityConnectionString(parametriServer.StringaConnessione)

		Try
			Using ctx As New Gias_DeveloperServer_Entities(efConnString)
				' Pre-carica l'intera Lista_Specie_Animali una sola volta.
				Dim tutteSpecie = ctx.Lista_Specie_Animali.ToList()

				For Each codiceStalla As String In codiciStalla.Distinct(StringComparer.OrdinalIgnoreCase)
					If String.IsNullOrWhiteSpace(codiceStalla) Then Continue For

					' Leggi la stalla tramite il DAL già usato dall'importer.
					Dim dtStalla As DataTable = stallaR.Leggi(
						"", 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
						"Stalla.BDN_Codice_Azienda = '" & codiceStalla & "'",
						"", parametriServer)

					If dtStalla Is Nothing OrElse dtStalla.Rows.Count = 0 Then
						' Stalla sconosciuta: sarà gestita da Valida() come STALL_CODE_NOT_FOUND.
						Continue For
					End If

					Dim speCod As Integer = CInt(dtStalla.Rows(0)("SPE_COD"))
					Dim genCod As Integer = CInt(dtStalla.Rows(0)("GEN_COD"))

					' Risolve il codice numerico in descrizione testuale per non dipendere
					' da valori interi specifici dell'installazione.
					' Si usa GEN_COD + SPE_COD perché SPE_COD da solo non è univoco in Lista_Specie_Animali
					' (es. GEN_COD=1/SPE_COD=1 = Bovino e GEN_COD=2/SPE_COD=1 = Suino coesistono).
					Dim speciePoco = tutteSpecie.FirstOrDefault(Function(s) s.GEN_COD = genCod AndAlso s.SPE_COD = speCod)
					If speciePoco Is Nothing Then Continue For

					mapping(codiceStalla) = MappaDescrizioneASpecie(speciePoco.SPE_DES)
				Next
			End Using
		Catch ex As Exception When Not TypeOf ex Is ArgumentNullException
			' Errore di accesso alla configurazione di mappatura stalla→specie.
			Throw New Exception(
				"Errore nel caricamento della mappatura stalle: " & ex.Message, ex)
		End Try

		Return mapping
	End Function

	''' <summary>
	''' Converte la descrizione testuale della specie (SPE_DES) nell'enum <see cref="SpecieAnimale"/>.
	''' Il confronto è case-insensitive e basato su sottostringa per tollerare varianti
	''' (es. "Bovini", "Bovino", "BOVINO").
	''' Specie supportate: Suino, Bovino, Altra.
	''' </summary>
	Private Shared Function MappaDescrizioneASpecie(speDes As String) As SpecieAnimale
		If String.IsNullOrWhiteSpace(speDes) Then Return SpecieAnimale.Altra
		Dim upper As String = speDes.Trim().ToUpperInvariant()
		If upper.Contains("SUINO") OrElse upper.Contains("SUINI") Then Return SpecieAnimale.Suino
		If upper.Contains("BOVINO") OrElse upper.Contains("BOVINI") Then Return SpecieAnimale.Bovino
		Return SpecieAnimale.Altra
	End Function

End Class
