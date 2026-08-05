
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports System.Text


Public Class Zoo_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'##############################################################################################
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' Da usare con Selezione_TabellaCompleta
	''' </summary>
	''' <param name="PIVA"></param>
	''' <param name="Sa_Cod"></param>
	''' <param name="Cod_Progetto"></param>
	''' <param name="Matricola"></param>
	''' <param name="Gen_Cod"></param>
	''' <param name="Spe_Cod"></param>
	''' <param name="Raz_Cod"></param>
	''' <param name="Ipro_Cod"></param>
	''' <param name="Nome"></param>
	''' <param name="Collare"></param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[pierantoni]	20/01/2011	Created
	''' </history>
	''' -----------------------------------------------------------------------------
	Public Function Leggi(
						ByVal PIVA As String,
						ByVal Sa_Cod As Integer,
						ByVal Cod_Progetto As Integer,
						ByVal Matricola As String,
						ByVal Gen_Cod As Integer,
						ByVal Spe_Cod As Integer,
						ByVal Raz_Cod As Integer,
						ByVal Ipro_Cod As Integer,
						ByVal Nome As String,
						ByVal Collare As String,
									ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
									ByVal xFiltroAggiuntivo As String,
									ByVal xOrderBy As String,
									ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
									) As DataTable


		Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_R.Leggi()"



		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try

			Select Case xSelezioneVariabile

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
					StrSQL.Length = 0

					StrSQL.Append(" SELECT * ")
					StrSQL.Append(" FROM  Zoo_Animali ")
					StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


					If PIVA <> "" Then
						StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")
					End If

					If Sa_Cod <> 0 Then
						StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
					End If

					If Cod_Progetto <> 0 Then
						StrSQL.Append(" AND Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "  ")
					End If

					If Matricola <> "" Then
						StrSQL.Append(" AND Matricola =  '" & Agro_SQL_SaveText(Trim(Matricola)) & "' ")
					End If

					If Gen_Cod <> 0 Then
						StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
					End If

					If Raz_Cod <> 0 Then
						StrSQL.Append(" AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
					End If

					If Ipro_Cod <> 0 Then
						StrSQL.Append(" AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
					End If

					If Nome <> "" Then
						StrSQL.Append(" AND NOME = '" & Agro_SQL_SaveText(Nome) & "'  ")
					End If

					If Collare <> "" Then
						StrSQL.Append(" AND COLLARE = '" & Agro_SQL_SaveText(Collare) & "'  ")
					End If


					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append(" AND     dbo.Zoo_Animali.Inviato >= 0 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append(" AND     dbo.Zoo_Animali.Inviato = -1 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append(" ORDER BY Spe_Cod, Gen_Cod, IPro_Cod, Raz_Cod, Matricola ASC ")
					End If

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
					'
					'
					'
					'


			End Select


			'--------------------------------------------------------------------------
			DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			DT = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return DT

	End Function


	Public Function LeggixAnagrafica(
					ByVal PIVA As String,
					ByVal Sa_Cod As Integer,
					ByVal Cod_Progetto As Integer,
					ByVal Matricola As String,
					ByVal Gen_Cod As Integer,
					ByVal Spe_Cod As Integer,
					ByVal Raz_Cod As Integer,
					ByVal Ipro_Cod As Integer,
					ByVal Nome As String,
					ByVal Collare As String,
								ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
								ByVal xFiltroAggiuntivo As String,
								ByVal xOrderBy As String,
								ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
								) As DataTable


		Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_R.Leggi()"



		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try

			Select Case xSelezioneVariabile

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
					StrSQL.Length = 0

					StrSQL.AppendLine("SELECT ")
					StrSQL.AppendLine("     CONCAT(ZOO_Animali.Piva, '_', ZOO_Animali.sa_cod, '_', ZOO_Animali.Cod_Progetto)  as chiave, ")
					StrSQL.AppendLine("     Zoo_Animali.*, ")
					StrSQL.AppendLine("     Lista_Specie_Animali.SPE_DES, ")
					StrSQL.AppendLine("     Lista_Razze_Animali.RAZ_DES, ")
					StrSQL.AppendLine("     ZOO_Animali.Validita_Inizio, ")
					StrSQL.AppendLine("     ZOO_Animali.Validita_Fine, ")
					StrSQL.AppendLine("     Zoo_Animali_Lista_Tipi.Tipo_Des, ")

					StrSQL.Append("      ZOO_Animali.Data_Creazione,   ")
					StrSQL.Append("      ZOO_Animali.Data_Modifica,   ")
					StrSQL.Append(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Creazione), ZOO_Animali.Username_Creazione) AS Utente_Creazione, ")
					StrSQL.Append(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ZOO_Animali.Username_Modifica), ZOO_Animali.Username_Modifica) AS Utente_Modifica ")

					StrSQL.AppendLine(" FROM ZOO_Animali ")
					StrSQL.AppendLine(" Left Join Lista_Specie_Animali ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD  ")
					StrSQL.AppendLine("                               AND Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD ")

					StrSQL.AppendLine(" Left Join Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD  ")
					StrSQL.AppendLine("                              AND Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD  ")
					StrSQL.AppendLine("                              AND Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD")

					StrSQL.AppendLine(" Left Join Zoo_Animali_Lista_Tipi ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD  ")
					StrSQL.AppendLine("                              AND Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD  ")
					StrSQL.AppendLine("                              AND Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD ")

					StrSQL.Append(" WHERE 1=1 ")

					If PIVA <> "" Then
						StrSQL.Append(" AND ZOO_Animali.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")
					End If

					If Sa_Cod <> 0 Then
						StrSQL.Append(" AND ZOO_Animali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
					End If

					If Cod_Progetto <> 0 Then
						StrSQL.Append(" AND ZOO_Animali.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "  ")
					End If

					If Matricola <> "" Then
						StrSQL.Append(" AND ZOO_Animali.Matricola =  '" & Agro_SQL_SaveText(Trim(Matricola)) & "' ")
					End If

					If Gen_Cod <> 0 Then
						StrSQL.Append(" AND ZOO_Animali.GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append(" AND ZOO_Animali.SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
					End If

					If Raz_Cod <> 0 Then
						StrSQL.Append(" AND ZOO_Animali.RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
					End If

					If Ipro_Cod <> 0 Then
						StrSQL.Append(" AND ZOO_Animali.IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
					End If

					If Nome <> "" Then
						StrSQL.Append(" AND ZOO_Animali.NOME = '" & Agro_SQL_SaveText(Nome) & "'  ")
					End If

					If Collare <> "" Then
						StrSQL.Append(" AND ZOO_Animali.COLLARE = '" & Agro_SQL_SaveText(Collare) & "'  ")
					End If


					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append(" AND     dbo.Zoo_Animali.Inviato >= 0 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append(" AND     dbo.Zoo_Animali.Inviato = -1 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append(" ORDER BY Zoo_Animali.Validita_Inizio,  Zoo_Animali.Matricola ASC ")
					End If

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
					'
					'
					'
					'


			End Select


			'--------------------------------------------------------------------------
			DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			DT = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return DT

	End Function

	''' <summary>
	''' Utilizzato in operazione carico - griglia capi 
	''' </summary>
	''' <param name="Piva"></param>
	''' <param name="Id_Agenda"></param>
	''' <param name="objP_Server"></param>
	''' <returns></returns>
	Public Function Leggi_Griglia(ByVal Piva As String,
								  ByVal Id_Agenda As Integer,
								  ByRef objP_Server As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_R.Leggi_Griglia()"

		Dim dt As DataTable
		Dim stb As New StringBuilder

		Try
			stb.Length = 0

			' TEMP VAR: Data Operazione
			stb.AppendLine("DECLARE @DataOperazione AS DATE").
				AppendLine("SET @DataOperazione = (SELECT TOP(1) Validita_Inizio ").
				AppendLine($"FROM Agenda WHERE Id_Agenda = {Agro_SQL_SaveNum(Id_Agenda)}); ")

			' CTE: Capi nell'operazione
			stb.AppendLine("	SELECT Movimenti_dettagli.Cod_Progetto ").
				AppendLine("	INTO #CapiOperazione_cte ").
				AppendLine("	FROM Agenda ").
				AppendLine("	INNER JOIN Movimenti_dettagli ON Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda").
				AppendLine($"	WHERE Agenda.Id_Agenda = {Agro_SQL_SaveNum(Id_Agenda)} ")

			' Capi Movimentati in data successiva al carico
			stb.AppendLine("    SELECT * ").
				AppendLine("    INTO #CapiMovimentati ").
				AppendLine("    FROM ( ").
				AppendLine("    SELECT DISTINCT Movimenti_dettagli.Cod_Progetto ").
				AppendLine("    FROM Movimenti_dettagli ").
				AppendLine("    INNER JOIN Movimenti ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda ").
				AppendLine("							AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ").
				AppendLine("    INNER JOIN Agenda ON Movimenti_dettagli.Id_Agenda = Agenda.Id_Agenda ").
				AppendLine("    WHERE 1=1 ").
				AppendLine("	      AND Agenda.Lav_Cod BETWEEN 3000 AND 3999 ").
				AppendLine($"	      AND Movimenti_dettagli.Id_Agenda <> {Agro_SQL_SaveNum(Id_Agenda)} ").
				AppendLine("	      AND Movimenti_dettagli.Cod_Progetto IN (SELECT Cod_Progetto FROM #CapiOperazione_cte) ").
				AppendLine("		  AND Movimenti.Data_Movimento >= @DataOperazione ").
				AppendLine("UNION ALL ").
				AppendLine("    SELECT DISTINCT Mov_Destinazioni.Id_Destinazione ").
				AppendLine("    FROM Mov_Destinazioni ").
				AppendLine("    INNER JOIN Movimenti ON Mov_Destinazioni.Id_Agenda = Movimenti.Id_Agenda ").
				AppendLine("							AND Mov_Destinazioni.Id_Mov = Movimenti.Id_Mov ").
				AppendLine("    INNER JOIN Agenda ON Mov_Destinazioni.Id_Agenda = Agenda.Id_Agenda ").
				AppendLine("    WHERE 1=1 ").
				AppendLine("        AND Agenda.Lav_Cod BETWEEN 3000 AND 3999 ").
				AppendLine($"        AND Mov_Destinazioni.Id_Agenda <> {Agro_SQL_SaveNum(Id_Agenda)} ").
				AppendLine("        AND Mov_Destinazioni.Id_Destinazione IN (SELECT Cod_Progetto FROM #CapiOperazione_cte) ").
				AppendLine("        AND Movimenti.Data_Movimento >= @DataOperazione ").
				AppendLine(" ) t")

			stb.AppendLine("    SELECT MovDettPeso.Cod_Progetto, MovDettPeso.Qta AS Kg_Pagati, MovDettPeso.Qta_Dettaglio1 AS Kg_Arrivo ").
				AppendLine("	  INTO #MovDettPeso ").
				AppendLine("	  FROM Movimenti AS MovPeso (NOLOCK) ").
				AppendLine("	  INNER JOIN Movimenti_dettagli AS MovDettPeso ON MovPeso.Piva = MovDettPeso.Piva ").
				AppendLine("	      AND MovPeso.Id_Agenda = MovDettPeso.Id_Agenda ").
				AppendLine("	      AND MovPeso.Id_Mov = MovDettPeso.Id_Mov ").
				AppendLine($"	  WHERE MovPeso.Cau_Mov = '{Agro_SQL_SaveText(CAU_PESATURA_ANIMALI)}' ").
				AppendLine($"        AND MovPeso.Piva = '{Agro_SQL_SaveText(Piva)}' ").
				AppendLine($"		   AND MovPeso.Id_Agenda = {Agro_SQL_SaveNum(Id_Agenda)} ")

			' SELECT
			stb.AppendLine("SELECT DISTINCT Movimenti_Dettagli.Id_Mov_Det, Movimenti_Dettagli.Id_Mov_Det AS Riga, Movimenti_Dettagli.Id_Mov_Esterno, ").
				AppendLine("    Zoo_Animali.Cod_Progetto, Zoo_Animali.Raz_Cod, ISNULL(Lista_Razze_Animali.RAZ_DES, '') AS Raz_Des, ").
				AppendLine("    Zoo_Animali.Matricola, Zoo_Animali.Tag, Zoo_Animali.MAT_MADRE AS Matricola_Madre, Zoo_Animali.DAT_NASCITA AS Data, Zoo_Animali.Sesso AS Sesso_Des, ").
				AppendLine("    Zoo_Animali.Certificato, Zoo_Animali.Data_Documento_Ingresso AS DataCertificato, ").
				AppendLine("    Zoo_Animali.Username_Creazione, Zoo_Animali.Data_Creazione, ").
				AppendLine("    COALESCE(Contatti_FornFatt.Cod_Contatto, '') AS CF_FornFatt, ").
				AppendLine("    COALESCE(Contatti_FornFatt.Rag_Soc + Contatti_FornFatt.Cognome + ' ' + Contatti_FornFatt.Nome, '') AS RagSoc_FornFatt, ").
				AppendLine("    COALESCE(Contatti_FornProv.Cod_Contatto, '') AS CF_FornProv, ").
				AppendLine("    COALESCE(Contatti_FornProv.Rag_Soc + Contatti_FornProv.Cognome + ' ' + Contatti_FornProv.Nome, '') AS RagSoc_FornProv, ").
				AppendLine("    COALESCE(Zoo_Animali.Lotto_Fornitore, '') AS Lotto_Fornitore, ").
				AppendLine("    COALESCE(Zoo_Animali.Modello4_Ingresso, '') AS Codice_Modello4_Ingresso, ").
				AppendLine("    COALESCE(Zoo_Animali.Modello4_Ingresso_Numero, '') AS N_Modello4_Ingresso, ").
				AppendLine("    COALESCE(Zoo_Animali.N_Bolla_Fornitore, '') AS N_DDT_Ingresso, ").
				AppendLine("    COALESCE(Zoo_Animali.N_Bolla_Uscita, '') AS N_DDT_Uscita, ").
				AppendLine($"    COALESCE(Zoo_Animali.Data_DDT_Ingresso, {Agro_SQL_SaveDate(AGRODATAINIZIO)}) AS Data_DDT_Ingresso, ").
				AppendLine($"    COALESCE(Zoo_Animali.Data_DDT_Uscita, {Agro_SQL_SaveDate(AGRODATAFINE)}) AS Data_DDT_Uscita, ").
				AppendLine("    COALESCE(Movimenti_Dettagli.Prezzo_Unitario, 0) AS Prezzo_Unitario, ").
				AppendLine("    COALESCE(MovDettPeso.Kg_Pagati, 0) AS Qta, ").
				AppendLine("    COALESCE(MovDettPeso.Kg_Arrivo, 0) AS Qta_Arrivo, ").
				AppendLine("    COALESCE(Zoo_Animali.Incremento_Teorico, 0) AS Incremento_Teorico, ")

			' Lotto
			stb.AppendLine("	  (SELECT TOP 1 Codice_Distinta FROM Zoo_Animali_Distinte ").
				AppendLine("	   WHERE Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale ").
				AppendLine("         AND Zoo_Animali.Piva = Movimenti_Dettagli.Piva ").
				AppendLine("         AND Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ").
				AppendLine("         AND CAST(Zoo_Animali_Distinte.Validita_inizio AS DATE) <= CAST(Movimenti.Data_Movimento AS DATE) ").
				AppendLine("         AND CAST(Zoo_Animali_Distinte.Validita_Fine AS DATE) >= CAST(Movimenti.Data_Movimento AS DATE) ").
				AppendLine("     ) AS Lotto, ")
			' Codice Distinta
			stb.AppendLine("	  (SELECT TOP 1 Cod_Progetto FROM Zoo_Animali_Distinte ").
				AppendLine("     WHERE Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale ").
				AppendLine("         AND Zoo_Animali.Piva = Movimenti_Dettagli.Piva ").
				AppendLine("         AND Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ").
				AppendLine("         AND CAST(Zoo_Animali_Distinte.Validita_inizio AS DATE) <= CAST(Movimenti.Data_Movimento AS DATE) ").
				AppendLine("         AND CAST(Zoo_Animali_Distinte.Validita_Fine AS DATE) >= CAST(Movimenti.Data_Movimento AS DATE) ").
				AppendLine("     ) AS Distinta_Cod, ")
			' Distinta_Chiusa
			'stb.AppendLine("	  (SELECT TOP 1 Distinta_Chiusa FROM Zoo_Animali_Distinte ").
			'	AppendLine("     WHERE Zoo_Animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale ").
			'	AppendLine("         AND Zoo_Animali.Piva = Movimenti_Dettagli.Piva ").
			'	AppendLine("         AND Movimenti_Dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ").
			'	AppendLine("         AND CAST(Zoo_Animali_Distinte.Validita_inizio AS DATE) <= CAST(Movimenti.Data_Movimento AS DATE) ").
			'	AppendLine("         AND CAST(Zoo_Animali_Distinte.Validita_Fine AS DATE) >= CAST(Movimenti.Data_Movimento AS DATE) ").
			'	AppendLine("     ) AS Distinta_Chiusa, ")
			' Capo Movimentato
			stb.AppendLine("    CASE WHEN CapiMovimentati.Cod_Progetto IS NOT NULL THEN 1 ELSE 0 ").
				AppendLine("    END AS Movimentato ")

			' FROM e JOIN
			stb.AppendLine("FROM Movimenti ").
				AppendLine("INNER JOIN Movimenti_dettagli ON Movimenti.Piva = Movimenti_dettagli.Piva ").
				AppendLine("    AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ").
				AppendLine("    AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ").
				AppendLine("INNER JOIN Zoo_Animali ON Movimenti_dettagli.Piva = Zoo_Animali.Piva ").
				AppendLine("    AND Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ").
				AppendLine("LEFT OUTER JOIN Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD ").
				AppendLine("     AND Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD ").
				AppendLine("     AND Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD ").
				AppendLine("LEFT JOIN #CapiMovimentati CapiMovimentati ON Movimenti_dettagli.Cod_Progetto = CapiMovimentati.Cod_Progetto ").
				AppendLine("LEFT JOIN #MovDettPeso MovDettPeso ON Movimenti_dettagli.Cod_Progetto = MovDettPeso.Cod_Progetto ")

			' Fornitore Fatturazione
			stb.AppendLine("OUTER APPLY ").
				AppendLine("    (SELECT TOP(1) Cod_Contatto, Rag_Soc, Cognome, Nome ").
				AppendLine("     FROM Contatti (NOLOCK) ").
				AppendLine("     WHERE Contatti.Cod_Contatto = Zoo_Animali.CF_Fornitore ").
				AppendLine("         AND (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1) ").
				AppendLine("    ) Contatti_FornFatt ")

			' Fornitore Provenienza
			stb.AppendLine("OUTER APPLY ").
				AppendLine("    (SELECT TOP(1) Cod_Contatto, Rag_Soc, Cognome, Nome ").
				AppendLine("     FROM Contatti (NOLOCK) ").
				AppendLine("     WHERE Contatti.Cod_Contatto = Zoo_Animali.Fornitore_Provenienza ").
				AppendLine("         AND (Contatti.Piva = Zoo_Animali.Piva OR Contatti.Sa_Cod = -1) ").
				AppendLine("    ) Contatti_FornProv ")

			' Peso Singolo Capo
			'stb.AppendLine("OUTER APPLY ").
			'	AppendLine("    (SELECT TOP 1 MovDettPeso.Qta AS Kg_Pagati, MovDettPeso.Qta_Dettaglio1 AS Kg_Arrivo  ").
			'	AppendLine("     FROM Movimenti AS MovPeso (NOLOCK) ").
			'	AppendLine("     INNER JOIN Movimenti_dettagli AS MovDettPeso ON MovPeso.Piva = MovDettPeso.Piva ").
			'	AppendLine("         AND MovPeso.Id_Agenda = MovDettPeso.Id_Agenda ").
			'	AppendLine("         AND MovPeso.Id_Mov = MovDettPeso.Id_Mov ").
			'	AppendLine($"     WHERE MovPeso.Cau_Mov = '{Agro_SQL_SaveText(CAU_PESATURA_ANIMALI)}' ").
			'	AppendLine($"         AND MovPeso.Piva = '{Agro_SQL_SaveText(Piva)}' ").
			'	AppendLine($"         AND MovPeso.Id_Agenda = {Agro_SQL_SaveNum(Id_Agenda)} ").
			'	AppendLine("    ) MovDettPeso ")

			' WHERE
			stb.AppendLine($"WHERE Movimenti.Cau_Mov = '{Agro_SQL_SaveText(CAU_CARICO_CONSISTENZE)}' ").
				AppendLine($"    AND Movimenti.Piva = '{Agro_SQL_SaveText(Piva)}' ").
				AppendLine($"    AND Movimenti.Id_Agenda = {Agro_SQL_SaveNum(Id_Agenda)} ")

			' ORDER BY
			stb.AppendLine("ORDER BY Id_Mov_Det ASC ")

			stb.AppendLine(" DROP TABLE #CapiMovimentati ")
			stb.AppendLine(" DROP TABLE #CapiOperazione_cte ")
			stb.AppendLine(" DROP TABLE #MovDettPeso ")

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Server, stb.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception($"[{NomeRoutine}] : {ex.Message}")
		End Try

		Return dt

	End Function


	Public Function Leggi_Barcode(ByVal Codice As String,
								  ByVal Tipo As Integer,
								  ByVal Bar_QR As Integer,
								  ByVal Codifica As Integer,
								  ByVal Data As DateTime,
								  ByVal xFiltroAggiuntivo As String,
								  ByVal xOrderBy As String,
								  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
								  ) As DataTable

		Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_R.Leggi_Barcode()"


		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try

			StrSQL.Length = 0

			StrSQL.Append(" SELECT * ")
			StrSQL.Append(" FROM  Zoo_Barcode ")
			StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
			StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")


			If Codice <> "" Then
				StrSQL.Append(" AND Codice = '" & Agro_SQL_SaveText(Trim(Codice)) & "'")
			End If

			If Tipo <> 0 Then
				StrSQL.Append(" AND Tipo = " & Agro_SQL_SaveNum(Tipo) & "  ")
			End If

			If Bar_QR <> 0 Then
				StrSQL.Append(" AND Bar_QR = " & Agro_SQL_SaveNum(Bar_QR) & "  ")
			End If

			If Codifica <> 0 Then
				StrSQL.Append(" AND Codifica = " & Agro_SQL_SaveNum(Codifica) & "  ")
			End If


			If xFiltroAggiuntivo <> "" Then
				StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
			End If

			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
					StrSQL.Append(" AND     dbo.Zoo_Barcode.Inviato >= 0 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
					StrSQL.Append(" AND     dbo.Zoo_Barcode.Inviato = -1 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
					'...................................
				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select
			'--------------------------------------------------------------------------

			If xOrderBy <> "" Then
				StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
			Else
				StrSQL.Append(" ORDER BY Codice, Tipo, Bar_QR, Inizio, Fine, Codifica ASC ")
			End If


			'--------------------------------------------------------------------------
			DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			DT = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return DT

	End Function



End Class
