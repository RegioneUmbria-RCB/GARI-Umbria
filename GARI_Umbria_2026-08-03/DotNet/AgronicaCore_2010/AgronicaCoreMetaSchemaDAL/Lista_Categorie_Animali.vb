Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_Categorie_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'##############################################################################################
	Public Function Leggi(ByVal GEN_COD As Integer,
							ByVal SPE_COD As Integer,
							ByVal IPRO_COD As Integer,
							ByVal CAT_COD As Integer,
							ByVal xFiltroAggiuntivo As String,
							ByVal xOrderBy As String,
							ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
							) As DataTable

		Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Categorie_Animali_R.Leggi"

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try

			StrSQL.Length = 0
			StrSQL.Append(" SELECT * ")
			StrSQL.Append(" FROM  Lista_Categorie_Animali ")
			StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
			StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

			If GEN_COD <> 0 Then
				StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(GEN_COD) & " ")
			End If
			If SPE_COD <> 0 Then
				StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(SPE_COD) & " ")
			End If
			If IPRO_COD <> 0 Then
				StrSQL.Append(" AND IPRO_COD = " & Agro_SQL_SaveNum(IPRO_COD) & " ")
			End If
			If CAT_COD <> 0 Then
				StrSQL.Append(" AND CAT_COD = " & Agro_SQL_SaveNum(CAT_COD) & " ")
			End If
			'--------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then
				StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
			End If
			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
					StrSQL.Append(" AND   Inviato >=0 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
					StrSQL.Append(" AND   Inviato =-1 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
					'...................................
				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select
			'--------------------------------------------------------------------------
			If xOrderBy <> "" Then
				StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
			Else
				'StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
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

	''' <summary>
	''' 
	''' </summary>
	''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
	''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
	''' <param name="Ipro_Cod">Passare -1 per non filtrare</param>
	''' <param name="Cat_Cod">Passare -1 per non filtrare</param>
	''' <param name="Coeff_UBA"></param>
	''' <param name="Cat_Des"></param>
	''' <param name="Sesso"></param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	Public Function Leggi(ByVal Gen_Cod As Integer,
						  ByVal Spe_Cod As Integer,
						  ByVal Ipro_Cod As Integer,
						  ByVal Cat_Cod As Integer,
						  ByVal Coeff_UBA As Decimal,
						  ByVal Cat_Des As String,
						  ByVal Sesso As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Categorie_Animali_R.Leggi()"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					strSQL.AppendLine("SELECT CAT_COD, CAT_DES ").
						AppendLine("FROM Lista_Categorie_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))

					If Ipro_Cod <> -1 Then strSQL.AppendLine("    AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod))

					If Cat_Cod <> -1 Then strSQL.AppendLine("    AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod))

					If Coeff_UBA <> 0 Then strSQL.AppendLine("    AND COEFF_UBA = " & Agro_SQL_SaveNum(Coeff_UBA))

					If Cat_Des <> "" Then strSQL.AppendLine("    AND CAT_DES = '" & Agro_SQL_SaveText(Cat_Des) & "'")

					If Sesso <> "" Then strSQL.AppendLine("    AND SESSO = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'")

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Lista_Categorie_Animali.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Lista_Categorie_Animali.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY CAT_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM  Lista_Categorie_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

					If Ipro_Cod <> -1 Then strSQL.AppendLine("    AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & " ")

					If Cat_Cod <> -1 Then strSQL.AppendLine("    AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod) & " ")

					If Coeff_UBA <> 0 Then strSQL.AppendLine("    AND COEFF_UBA = " & Agro_SQL_SaveNum(Coeff_UBA) & " ")

					If Cat_Des <> "" Then strSQL.AppendLine("    AND CAT_DES = '" & Agro_SQL_SaveText(Cat_Des) & "' ")

					If Sesso <> "" Then strSQL.AppendLine("    AND SESSO = '" & Agro_SQL_SaveText(Trim(Sesso)) & "' ")

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.Append("    AND Lista_Categorie_Animali.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.Append("    AND Lista_Categorie_Animali.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY CAT_DES ASC ")
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
	''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
	''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
	''' <param name="objParametri"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <returns></returns>
	Public Function LeggiDaRegolamentoSpecie(ByVal Regolamento_Cod As Integer,
											 ByVal Spe_Cod As Integer,
											 ByVal Gen_Cod As Integer,
											 ByRef objParametri As AgronicaCoreParametri,
											 Optional ByVal xFiltroAggiuntivo As String = "",
											 Optional ByVal xOrderBy As String = "") As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Categorie_Animali_R.LeggiDaRegolamentoSpecie()"
		Dim strSQL As New StringBuilder
		Dim dt As New DataTable

		Try
			strSQL.Length = 0

			strSQL.AppendLine("SELECT DISTINCT ").
				AppendLine("    Lista_Categorie_Animali.Gen_Cod, Lista_Categorie_Animali.Spe_Cod, ").
				AppendLine("    Lista_Categorie_Animali.IPro_Cod, Lista_Categorie_Animali.Cat_Cod, ").
				AppendLine("    Lista_Categorie_Animali.Cat_Des, ").
				AppendLine("    Lista_Categorie_Animali_Attributi.Peso_Medio, ").
				AppendLine("    Lista_Categorie_Animali_Attributi.N_PV, ").
				AppendLine("    Lista_Categorie_Animali_Attributi.Acque_Residue ").
				AppendLine("FROM Lista_Categorie_Animali ").
				AppendLine("INNER JOIN Lista_Categorie_Animali_Attributi ").
				AppendLine("    ON Lista_Categorie_Animali.Gen_Cod = Lista_Categorie_Animali_Attributi.Gen_Cod ").
				AppendLine("    AND Lista_Categorie_Animali.Spe_Cod = Lista_Categorie_Animali_Attributi.Spe_Cod ").
				AppendLine("    AND Lista_Categorie_Animali.IPro_Cod = Lista_Categorie_Animali_Attributi.IPro_Cod ").
				AppendLine("    AND Lista_Categorie_Animali.Cat_Cod = Lista_Categorie_Animali_Attributi.Cat_Cod ").
				AppendLine("WHERE Lista_Categorie_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
				AppendLine("    AND Lista_Categorie_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

			If Gen_Cod <> -1 Then strSQL.AppendLine("    AND Lista_Categorie_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

			If Spe_Cod <> -1 Then strSQL.AppendLine("    AND Lista_Categorie_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

			If Regolamento_Cod <> 0 Then strSQL.AppendLine("    AND Lista_Categorie_Animali_Attributi.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

			'--------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case enumVisibilita.Visibilita_SoloNonCancellati
					strSQL.AppendLine("    AND Lista_Categorie_Animali.Inviato >=0 ")
				Case enumVisibilita.Visibilita_SoloCancellati
					strSQL.AppendLine("    AND Lista_Categorie_Animali.Inviato =-1 ")
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

    ''' <summary>
    ''' Stessa funzione di <see cref="LeggiDaRegolamentoSpecie"/> ma con un check per assicurare che la categoria 
	''' caricata abbia almeno una stabulazione associata.
    ''' </summary>
    ''' <param name="Regolamento_Cod"></param>
    ''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
    ''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
    ''' <param name="objParametri"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <returns></returns>
    Public Function LeggiDaRegolamentoSpecieConStabulazione(
		ByVal Regolamento_Cod As Integer,
		ByVal Spe_Cod As Integer,
		ByVal Gen_Cod As Integer,
		ByRef objParametri As AgronicaCoreParametri,
		Optional ByVal xFiltroAggiuntivo As String = "",
		Optional ByVal xOrderBy As String = ""
	) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Categorie_Animali_R.LeggiDaRegolamentoSpecie()"
		Dim strSQL As New StringBuilder With {.Length = 0}
		Dim dt As New DataTable

		Try
			strSQL.AppendLine("SELECT DISTINCT ").
				AppendLine("    Lista_Categorie_Animali.Gen_Cod, Lista_Categorie_Animali.Spe_Cod, ").
				AppendLine("    Lista_Categorie_Animali.IPro_Cod, Lista_Categorie_Animali.Cat_Cod, ").
				AppendLine("    Lista_Categorie_Animali.Cat_Des, ").
				AppendLine("    Lista_Categorie_Animali_Attributi.Peso_Medio, ").
				AppendLine("    Lista_Categorie_Animali_Attributi.N_PV, ").
				AppendLine("    Lista_Categorie_Animali_Attributi.Acque_Residue ").
				AppendLine("FROM Lista_Categorie_Animali ").
				AppendLine("INNER JOIN Lista_Categorie_Animali_Attributi ").
				AppendLine("    ON Lista_Categorie_Animali.Gen_Cod = Lista_Categorie_Animali_Attributi.Gen_Cod ").
				AppendLine("    AND Lista_Categorie_Animali.Spe_Cod = Lista_Categorie_Animali_Attributi.Spe_Cod ").
				AppendLine("    AND Lista_Categorie_Animali.IPro_Cod = Lista_Categorie_Animali_Attributi.IPro_Cod ").
				AppendLine("    AND Lista_Categorie_Animali.Cat_Cod = Lista_Categorie_Animali_Attributi.Cat_Cod ").
				AppendLine("WHERE").
				AppendLine("    EXISTS (").
				AppendLine("       SELECT TOP(1) 1 FROM Lista_TipiStallaxCategorie ").
				AppendLine("       WHERE Lista_Categorie_Animali.Gen_Cod = Lista_TipiStallaxCategorie.Gen_Cod ").
				AppendLine("       AND Lista_Categorie_Animali.Spe_Cod = Lista_TipiStallaxCategorie.Spe_Cod").
				AppendLine("       AND Lista_Categorie_Animali.IPro_Cod = Lista_TipiStallaxCategorie.IPro_Cod").
				AppendLine("       AND Lista_Categorie_Animali.Cat_Cod = Lista_TipiStallaxCategorie.Cat_Cod")
			
			If Regolamento_Cod <> 0 Then strSQL.AppendLine("       AND Lista_TipiStallaxCategorie.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

			strSQL.AppendLine("    )").
				AppendLine("    AND Lista_Categorie_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
				AppendLine("    AND Lista_Categorie_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

			If Gen_Cod <> -1 Then strSQL.AppendLine("    AND Lista_Categorie_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

			If Spe_Cod <> -1 Then strSQL.AppendLine("    AND Lista_Categorie_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

			If Regolamento_Cod <> 0 Then strSQL.AppendLine("    AND Lista_Categorie_Animali_Attributi.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

			'--------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case enumVisibilita.Visibilita_SoloNonCancellati
					strSQL.AppendLine("    AND Lista_Categorie_Animali.Inviato >=0 ")
				Case enumVisibilita.Visibilita_SoloCancellati
					strSQL.AppendLine("    AND Lista_Categorie_Animali.Inviato =-1 ")
				Case enumVisibilita.Visibilita_Tutti

				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select

			'--------------------------------------------------------------------------
			If xOrderBy <> "" Then strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
		Catch ex As Exception
			Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try
		Return dt
	End Function

End Class
