Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_Specie_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'##############################################################################################
	Public Function Leggi(ByVal GEN_COD As Integer,
						  ByVal SPE_COD As Integer,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri,
						  Optional joinDescrizioni As Boolean = False) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Specie_Animali_R.Leggi"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0
			strSQL.AppendLine("SELECT * ").
				AppendLine("FROM Lista_Specie_Animali ")

			If joinDescrizioni Then
				strSQL.AppendLine("JOIN Lista_Generi_Animali ").
					AppendLine("    ON Lista_Generi_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD ")
			End If

			strSQL.AppendLine("WHERE Lista_Specie_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
				AppendLine("    AND Lista_Specie_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

			If GEN_COD <> -1 Then strSQL.AppendLine("    AND Lista_Specie_Animali.GEN_COD = " & Agro_SQL_SaveNum(GEN_COD) & " ")

			If SPE_COD <> -1 Then strSQL.AppendLine("    AND Lista_Specie_Animali.SPE_COD = " & Agro_SQL_SaveNum(SPE_COD) & " ")

			'----------------------------------------------------------------------------------------------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

			'----------------------------------------------------------------------------------------------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case enumVisibilita.Visibilita_SoloNonCancellati
					strSQL.AppendLine("    AND Lista_Specie_Animali.Inviato >= 0 ")
				Case enumVisibilita.Visibilita_SoloCancellati
					strSQL.AppendLine("    AND Lista_Specie_Animali.Inviato = -1 ")
				Case enumVisibilita.Visibilita_Tutti

				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select

			'----------------------------------------------------------------------------------------------------------------------------------------------------------------
			If xOrderBy <> "" Then strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

			'----------------------------------------------------------------------------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
			'----------------------------------------------------------------------------------------------------------------------------------------------------------------

		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, msgErrore)
			dt = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return dt

	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
	''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	Public Function Leggi(ByVal Gen_Cod As Integer,
						  ByVal Spe_Cod As Integer,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Specie_Animali_R.Leggi()"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					strSQL.AppendLine("SELECT SPE_COD, SPE_DES ").
						AppendLine("FROM Lista_Specie_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)).
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY SPE_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Lista_Specie_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY SPE_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni

				Case enumSelezioneVariabile.Selezione_JoinCompleta

			End Select

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, msgErrore)
			dt = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return dt

	End Function

	''' <summary>
	''' 
	''' </summary>
	''' <param name="Regolamento_Cod"></param>
	''' <param name="objParametri"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <returns></returns>
	Public Function LeggiDaRegolamento(ByVal Regolamento_Cod As Integer,
									   ByRef objParametri As AgronicaCoreParametri,
									   Optional ByVal xFiltroAggiuntivo As String = "",
									   Optional ByVal xOrderBy As String = "") As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Specie_Animali_R.LeggiDaRegolamento()"
		Dim strSQL As New StringBuilder
		Dim dt As New DataTable

		Try
			strSQL.Length = 0

			strSQL.AppendLine("SELECT DISTINCT Lista_Specie_Animali.Gen_Cod, Lista_Specie_Animali.Spe_Cod, Lista_Specie_Animali.Spe_Des ").
				AppendLine("FROM Lista_Specie_Animali ").
				AppendLine("INNER JOIN Lista_Categorie_Animali_Attributi ").
				AppendLine("    ON Lista_Specie_Animali.Gen_Cod = Lista_Categorie_Animali_Attributi.Gen_Cod ").
				AppendLine("    AND Lista_Specie_Animali.Spe_Cod = Lista_Categorie_Animali_Attributi.Spe_Cod ").
				AppendLine("WHERE Lista_Specie_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
				AppendLine("    AND Lista_Specie_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

			If Regolamento_Cod <> 0 Then strSQL.AppendLine("    AND Lista_Categorie_Animali_Attributi.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

			'--------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case enumVisibilita.Visibilita_SoloNonCancellati
					strSQL.AppendLine("    AND Lista_Specie_Animali.Inviato >= 0 ")
				Case enumVisibilita.Visibilita_SoloCancellati
					strSQL.AppendLine("    AND Lista_Specie_Animali.Inviato = -1 ")
				Case enumVisibilita.Visibilita_Tutti

				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select

			'--------------------------------------------------------------------------
			If xOrderBy <> "" Then strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)

		Catch ex As Exception
			dt = Nothing
			Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return dt

	End Function

End Class
