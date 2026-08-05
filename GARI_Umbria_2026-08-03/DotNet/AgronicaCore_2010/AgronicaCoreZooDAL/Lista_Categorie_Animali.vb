
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider


Public Class Lista_Categorie_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider


	'##############################################################################################
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' DEPRECATA, utilizzare AgronicaCoreMetaschemaDAL.Lista_Categorie_Animali_R.Leggi()
	''' </summary>
	''' <param name="Gen_Cod"></param>
	''' <param name="Spe_Cod"></param>
	''' <param name="Ipro_Cod"></param>
	''' <param name="Cat_Cod"></param>
	''' <param name="Coeff_UBA"></param>
	''' <param name="Cat_Des"></param>
	''' <param name="Sesso"></param>
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
	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Categorie_Animali_R.Leggi()")>
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
		Const NomeRoutine As String = "AgronicaCoreZooDAL.Lista_Categorie_Animali_R.Leggi()"

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try
			StrSQL.Length = 0

			Select Case xSelezioneVariabile

				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					StrSQL.Append("SELECT CAT_COD, CAT_DES ")
					StrSQL.Append("FROM Lista_Categorie_Animali ")
					StrSQL.Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					StrSQL.Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> 0 Then
						StrSQL.Append("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))
					End If

					If Ipro_Cod <> -1 Then
						StrSQL.Append("    AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod))
					End If

					If Cat_Cod <> -1 Then
						StrSQL.Append("    AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod))
					End If

					If Coeff_UBA <> 0 Then
						StrSQL.Append("    AND COEFF_UBA = " & Agro_SQL_SaveNum(Coeff_UBA))
					End If

					If Cat_Des <> "" Then
						StrSQL.Append("    AND CAT_DES = '" & Agro_SQL_SaveText(Cat_Des) & "'")
					End If

					If Sesso <> "" Then
						StrSQL.Append("    AND SESSO = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'")
					End If

					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append("    AND dbo.Lista_Categorie_Animali.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append("    AND dbo.Lista_Categorie_Animali.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append("ORDER BY CAT_DES ASC ")
					End If

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
					StrSQL.Append(" SELECT * ")
					StrSQL.Append(" FROM  Lista_Categorie_Animali ")
					StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> 0 Then
						StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
					End If

					If Ipro_Cod <> -1 Then
						StrSQL.Append(" AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
					End If

					If Cat_Cod <> -1 Then
						StrSQL.Append(" AND CAT_COD = " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
					End If

					If Coeff_UBA <> 0 Then
						StrSQL.Append(" AND COEFF_UBA = " & Agro_SQL_SaveNum(Coeff_UBA) & "  ")
					End If

					If Cat_Des <> "" Then
						StrSQL.Append(" AND CAT_DES = '" & Agro_SQL_SaveText(Cat_Des) & "'")
					End If

					If Sesso <> "" Then
						StrSQL.Append(" AND SESSO = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'")
					End If


					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append(" AND     dbo.Lista_Categorie_Animali.Inviato >= 0 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append(" AND     dbo.Lista_Categorie_Animali.Inviato = -1 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append(" ORDER BY CAT_DES ASC ")
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

	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Categorie_Animali_R.LeggiDaRegolamentoSpecie()")>
	Public Function LeggiDaRegolamentoSpecie(
		Regolamento_Cod As Integer,
		Spe_Cod As Integer,
		Gen_Cod As Integer,
		objParametri As AgronicaCoreParametri,
		Optional xFiltroAggiuntivo As String = "",
		Optional xOrderBy As String = ""
	) As DataTable
		Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Categorie_Animali_R.LeggiDaRegolamentoSpecie"
		Dim StrSQL As New Text.StringBuilder With {.Length = 0}
		Try
			StrSQL.AppendLine(" SELECT DISTINCT ")
			StrSQL.AppendLine(" Lista_Categorie_Animali.Gen_Cod, Lista_Categorie_Animali.Spe_Cod, ")
			StrSQL.AppendLine(" Lista_Categorie_Animali.IPro_Cod, Lista_Categorie_Animali.Cat_Cod, ")
			StrSQL.AppendLine(" Lista_Categorie_Animali.Cat_Des, ")
			StrSQL.AppendLine(" Lista_Categorie_Animali_Attributi.Peso_Medio, ")
			StrSQL.AppendLine(" Lista_Categorie_Animali_Attributi.N_PV, ")
			StrSQL.AppendLine(" Lista_Categorie_Animali_Attributi.Acque_Residue ")
			StrSQL.AppendLine(" FROM  Lista_Categorie_Animali ")
			StrSQL.AppendLine(" INNER JOIN Lista_Categorie_Animali_Attributi ")
			StrSQL.AppendLine("   ON Lista_Categorie_Animali.Gen_Cod = Lista_Categorie_Animali_Attributi.Gen_Cod ")
			StrSQL.AppendLine("   AND Lista_Categorie_Animali.Spe_Cod = Lista_Categorie_Animali_Attributi.Spe_Cod ")
			StrSQL.AppendLine("   AND Lista_Categorie_Animali.IPro_Cod = Lista_Categorie_Animali_Attributi.IPro_Cod ")
			StrSQL.AppendLine("   AND Lista_Categorie_Animali.Cat_Cod = Lista_Categorie_Animali_Attributi.Cat_Cod ")

			StrSQL.AppendLine(" WHERE Lista_Categorie_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
			StrSQL.AppendLine(" AND   Lista_Categorie_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
			If Spe_Cod <> 0 Then
				StrSQL.AppendLine(" AND Lista_Categorie_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & " ")
			End If
			If Gen_Cod <> 0 Then
				StrSQL.AppendLine(" AND Lista_Categorie_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & " ")
			End If
			If Regolamento_Cod <> 0 Then
				StrSQL.AppendLine(" AND Lista_Categorie_Animali_Attributi.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
			End If
			'--------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then
				StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
			End If
			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
					StrSQL.AppendLine(" AND   Lista_Categorie_Animali.Inviato >=0 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
					StrSQL.AppendLine(" AND   Lista_Categorie_Animali.Inviato =-1 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
					'...................................
				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select
			'--------------------------------------------------------------------------
			If xOrderBy <> "" Then
				StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
			End If
			'--------------------------------------------------------------------------
			Return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
		Catch ex As Exception
			Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try
	End Function

End Class
