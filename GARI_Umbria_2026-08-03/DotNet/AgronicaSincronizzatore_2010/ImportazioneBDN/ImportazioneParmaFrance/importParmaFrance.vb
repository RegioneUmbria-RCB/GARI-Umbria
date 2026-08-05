Imports System.IO
Imports System.Text
Imports System.Web.UI.WebControls
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

''' <summary>
''' Orchestratore thin dell'import ParmaFrance.
''' Responsabilità: setup log directory, loop sui file,
''' invocazione ParmaFranceExcelReader + ParmaFranceImporter, eliminazione file, output UI.
''' La firma pubblica è mantenuta identica all'originale per compatibilità con ImportazionePC.aspx.vb.
''' </summary>
Public Class importParmaFrance

	Private ReadOnly _parametriServer As AgronicaCoreParametri
	Private ReadOnly _parametriUtenti As AgronicaCoreParametri

	Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
		_parametriServer = objParametriServer
		_parametriUtenti = objParametriUtenti
	End Sub

	''' <summary>
	''' Avvia l'importazione dei file Excel ParmaFrance.
	''' I parametri anno, wkt_georiferimento_cod, ASG_Utente_Username, ASG_Utente_Password,
	''' ASG_ProgressivoGIAS, CodiceChiaveCliente, LinkWSImportaGIAS e flag_CentroUnico
	''' sono mantenuti per compatibilità con il chiamante ma non sono attualmente utilizzati
	''' dalla logica di import.
	''' </summary>
	Public Sub Avvia_Importazione_AnagrafeExcel(ByVal PivaSelezionata As String,
												ByVal listaFile As List(Of String),
												ByVal Directory_Log As String,
												ByVal anno As Integer,
												ByVal wkt_georiferimento_cod As String,
												ByRef lbl_conclusione As Label,
												ByVal objParametri_Server As AgronicaCoreParametri,
												ByVal objParametri_Utenti As AgronicaCoreParametri,
												ByVal ASG_Utente_Username As String,
												ByVal ASG_Utente_Password As String,
												ByVal ASG_ProgressivoGIAS As String,
												ByVal CodiceChiaveCliente As String,
												ByVal LinkWSImportaGIAS As String,
												ByVal flag_CentroUnico As Boolean)

		If String.IsNullOrEmpty(Directory_Log) Then
			Directory_Log = "C:\GIASLAN"
		End If

		Directory_Log = Path.Combine(Directory_Log, "Importazione_ParmaFrance_Log")
		If Not Directory.Exists(Directory_Log) Then
			Directory.CreateDirectory(Directory_Log)
		End If

		Dim logFileName As String = "Importazione_Anagrafiche_ParmaFrance__" & Now.ToString("yyyy-MM-dd__(hh.mm.ss)") & ".txt"
		File.CreateText(Path.Combine(Directory_Log, logFileName)).Close()

		lbl_conclusione.Visible = True

		Dim risultatoFinale As New StringBuilder()

		' Istanza unica del importer: i cache (razza, detentore, stati accrescimento)
		' vengono riusati su tutti i file della sessione corrente.
		Dim importer As New ParmaFranceImporter(_parametriServer, _parametriUtenti)
		' Pre-inizializza il filtrone subito, prima che qualsiasi DataReader venga aperto dall'EF context.
		importer.PreWarmFiltrone()

		' Istanza unica del razzaResolver: _bdnCache e _defaultRazzaCache
		' vengono riusati su tutti i file, evitando lookup BDN duplicati per le stesse razze.
		Dim razzaResolver As New ParmaFranceRazzaResolver(_parametriServer)

		For Each pathFile In listaFile
			Dim cuaa As String = Path.GetFileNameWithoutExtension(pathFile).Split("_")(2)
			Dim risultato As String = ""
			Try
				Dim resultLettura As LetturaExcelResult = ParmaFranceExcelReader.LeggiFile(pathFile)

				' Mostra avvisi per righe scartate durante la lettura (righe vuote o colonne critiche assenti).
				If resultLettura.Errori.Count > 0 Then
					risultato &= "<h5>File " & cuaa & " — avvisi lettura:</h5><ul>"
					For Each errore In resultLettura.Errori
						risultato &= "<li>Riga " & errore.NumeroRiga & ": " & errore.Messaggio & "</li>"
					Next
					risultato &= "</ul>"
				End If

				If resultLettura.TotaleRigheEstratte = 0 Then
					risultato &= "<h5 class=""red"">File " & cuaa & ": nessuna riga valida estratta.</h5>"
				Else
					' ── Riconoscimento Specie (sempre eseguito: popola "Specie" per il RazzaResolver) ──────────
					Dim dtCapi As DataTable = resultLettura.Righe
					Dim codiciStalla = resultLettura.Righe.Rows.Cast(Of DataRow)() _
								.Select(Function(r) If(IsDBNull(r("F19")), "", CStr(r("F19")).Trim())) _
								.Distinct(StringComparer.OrdinalIgnoreCase)
					Dim mappatura As Dictionary(Of String, SpecieAnimale) =
						ParmaFranceSpecieValidator.CaricaMappaturaDaDb(codiciStalla, _parametriServer)
					If mappatura.Count = 0 Then
						risultato &= "<h5 class=""red"">File " & cuaa & ": nessuna stalla riconosciuta nel DB. Verificare la configurazione delle stalle.</h5>"
						GoTo ContinuaFile
					Else
						Dim validatorSpecie As New ParmaFranceSpecieValidator(mappatura)
						Dim resultSpecie As ValidazioneSpecieResult = validatorSpecie.Valida(dtCapi)
						' Valida() aggiunge "Specie" in-place su dtCapi (tutte le righe); le righe
						' con stalla non riconosciuta hanno Specie=DBNull e vanno in resultSpecie.Errori.

						If resultSpecie.Errori.Count > 0 Then
							risultato &= "<h5>File " & cuaa & " — codici stalla non riconosciuti (" & resultSpecie.Errori.Count & " righe rifiutate):</h5><ul>"
							For Each errSpecie In resultSpecie.Errori
								risultato &= "<li>Capo " & errSpecie.Matricola & " (stalla " & errSpecie.CodiceStalla & "): " & errSpecie.Messaggio & "</li>"
							Next
							risultato &= "</ul>"
						End If
						If resultSpecie.TotaleRigheValide = 0 Then
							risultato &= "<h5 class=""red"">File " & cuaa & ": nessuna riga con specie riconosciuta.</h5>"
							GoTo ContinuaFile
						End If
						dtCapi = resultSpecie.RigheValide

						' ── Validazione Dinamica Campi Obbligatori/Opzionali ──────────────────────
						Dim resultValidazione As ValidazioneCampiResult = ParmaFranceCampoValidator.ValidaDataTable(dtCapi)
						If resultValidazione.RigheScartate.Count > 0 Then
							risultato &= "<h5>File " & cuaa & " — righe scartate per errori di validazione (" & resultValidazione.RigheScartate.Count & " righe):</h5><ul>"
							For Each rigaScartata In resultValidazione.RigheScartate
								risultato &= "<li>Capo " & rigaScartata.Matricola & ":<ul>"
								For Each errCampo In rigaScartata.CampiErrore
									risultato &= "<li>" & errCampo.MessaggioErrore & "</li>"
								Next
								risultato &= "</ul></li>"
							Next
							risultato &= "</ul>"
						End If
						If resultValidazione.TotaleRigheValide = 0 Then
							risultato &= "<h5 class=""red"">File " & cuaa & ": nessuna riga ha superato la validazione campi.</h5>"
							GoTo ContinuaFile
						End If
						dtCapi = resultValidazione.RigheValide
					End If

					' ── Determinazione Razza con Lookup BDN ─────────────────────────────────────────
					Dim resultRazza As RisoluzioneRazzaResult = razzaResolver.RisolviDataTable(dtCapi)
					If resultRazza.Warning.Count > 0 Then
						risultato &= "<h5>File " & cuaa & " — warning razza (" & resultRazza.Warning.Count & " righe con default applicato):</h5><ul>"
						For Each w In resultRazza.Warning
							risultato &= "<li>Capo " & w.Matricola & ": " & w.Messaggio & "</li>"
						Next
						risultato &= "</ul>"
					End If
					dtCapi = resultRazza.RigheArricchite

					Dim messaggio As String = ""
					importer.ImportaCapi(PivaSelezionata, dtCapi, Directory_Log, logFileName, messaggio)
					If Not String.IsNullOrEmpty(messaggio) Then
						risultato &= "<h5>File " & cuaa & "</h5>" & vbCrLf &
									 "Importa_Dati: " & messaggio & vbCrLf & "<br/><br/>"
					End If
				End If

ContinuaFile:
			Catch ex As Exception
				risultato = "<h5 class=""red"">File " & cuaa & "</h5>" & vbCrLf &
							 "<p class=""red"">Errore durante l'elaborazione del file: " & ex.Message & "</p><br/>"
			End Try

			risultatoFinale.Append(risultato)

			Try
				File.Delete(pathFile)
			Catch
				' L'eliminazione del file è non bloccante
			End Try
		Next

		lbl_conclusione.Visible = True
		lbl_conclusione.Text = "<h4>Import Capi UE: </h4>" & vbCrLf & risultatoFinale.ToString()
	End Sub

End Class
