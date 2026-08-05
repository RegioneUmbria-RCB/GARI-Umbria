Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_IndProd_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'##############################################################################################
	''' <summary>
	''' 
	''' </summary>
	''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
	''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
	''' <param name="Ipro_Cod">Passare -1 per non filtrare</param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	Public Function Leggi(ByVal Gen_Cod As Long,
						  ByVal Spe_Cod As Long,
						  ByVal Ipro_Cod As Long,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine = "AgronicaCoreMetaSchemaDAL.Lista_Specie_Animali_R.Leggi()"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					strSQL.AppendLine("SELECT IPRO_COD, IPRO_DES ").
						AppendLine("FROM Lista_IndirizziProd_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)).
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))

					If Ipro_Cod <> -1 Then strSQL.AppendLine("    AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod))

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
						strSQL.AppendLine("ORDER BY IPRO_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Lista_IndirizziProd_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")

					If Ipro_Cod <> -1 Then strSQL.AppendLine("    AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")

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
						strSQL.AppendLine("ORDER BY IPRO_DES ASC ")
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
	''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
	''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
	''' <param name="Ipro_Cod">Passare -1 per non filtrare</param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri_Server"></param>
	''' <returns></returns>
	Public Function LeggiRisorseZootecniche(ByVal Gen_Cod As Integer,
											ByVal Spe_Cod As Integer,
											ByVal Ipro_Cod As Integer,
											ByVal xFiltroAggiuntivo As String,
											ByVal xOrderBy As String,
											ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

		Const NomeRoutine = "AgronicaCoreMetaSchemaDAL.Lista_Specie_Animali_R.LeggiRisorseZootecniche()"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			strSQL.AppendLine("SELECT ").
				AppendLine("     s.GEN_COD, s.SPE_COD, i.IPRO_COD, CONCAT((g.GEN_DES), ' - ', CASE WHEN i.IPRO_COD > 0 THEN CONCAT(s.SPE_DES, ' - ', i.IPRO_DES) ELSE s.SPE_DES END) as ZOO_DES, ").
				AppendLine(" REPLACE(CONCAT((g.GEN_DES), ' - ', CASE WHEN i.IPRO_COD > 0 THEN CONCAT(s.SPE_DES, ' - ', i.IPRO_DES) ELSE s.SPE_DES END), '(', '') AS campo_ordinamento ").
				AppendLine("FROM Lista_Specie_Animali s ").
				AppendLine("INNER JOIN Lista_Generi_Animali g ON s.GEN_COD = g.GEN_COD ").
				AppendLine("LEFT JOIN Lista_IndirizziProd_Animali i on s.GEN_COD = i.GEN_COD and s.SPE_COD = i.SPE_COD ").
				AppendLine("WHERE 1 = 1 ").
				AppendLine("    AND s.GEN_COD <> 0 ")

			If Gen_Cod <> -1 Then strSQL.AppendLine("    AND s.GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & " ")

			If Spe_Cod <> -1 Then strSQL.AppendLine("    AND s.SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & " ")

			If Ipro_Cod <> -1 Then strSQL.AppendLine("    AND i.IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & " ")

			If xFiltroAggiuntivo <> "" Then strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))

			If xOrderBy <> "" Then
				strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
			Else
				strSQL.AppendLine("ORDER BY campo_ordinamento ")
			End If

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri_Server, strSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objParametri_Server, NomeRoutine, msgErrore)
			dt = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return dt

	End Function

End Class
