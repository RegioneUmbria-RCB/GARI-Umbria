
Imports System.Data.OleDb
Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Lista_Specie_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'##############################################################################################
	''' <summary>
	''' </summary>
	''' <param name="Gen_Cod"></param>
	''' <param name="Spe_Cod"></param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Specie_Animali_R.Leggi()")>
	Public Function Leggi(ByVal Gen_Cod As Long,
						  ByVal Spe_Cod As Long,
						  ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreZooDAL.Lista_Specie_Animali_R.Leggi()"

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New StringBuilder
		Dim DT As DataTable

		Try
			StrSQL.Length = 0

			Select Case xSelezioneVariabile

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					StrSQL.Append("SELECT SPE_COD, SPE_DES ")
					StrSQL.Append("FROM Lista_Specie_Animali ")
					StrSQL.Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
					StrSQL.Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> 0 Then
						StrSQL.Append("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
					End If

					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If
					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append("    AND Inviato >= 0 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append("    AND Inviato = -1 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append("ORDER BY SPE_DES ASC ")
					End If
					'------------------------------------------------------------------

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
					StrSQL.Length = 0
					StrSQL.Append(" SELECT * ")
					StrSQL.Append(" FROM  Lista_Specie_Animali ")
					StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


					If Gen_Cod <> 0 Then
						StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
					End If


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
						StrSQL.Append(" ORDER BY SPE_DES ASC ")
					End If
					'------------------------------------------------------------------

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
					'
					'
					'
					'


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

	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Specie_Animali_R.LeggiDaRegolamento()")>
	Public Function LeggiDaRegolamento(
		Regolamento_Cod As Integer,
		objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
		Optional xFiltroAggiuntivo As String = "",
		Optional xOrderBy As String = ""
	) As DataTable
		Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Specie_Animali_R.LeggiDaRegolamento"
		Dim StrSQL As New Text.StringBuilder With {.Length = 0}
		Try
			StrSQL.AppendLine(" SELECT DISTINCT Lista_Specie_Animali.Gen_Cod, Lista_Specie_Animali.Spe_Cod, Lista_Specie_Animali.Spe_Des ")
			StrSQL.AppendLine(" FROM  Lista_Specie_Animali ")
			StrSQL.AppendLine(" INNER JOIN Lista_Categorie_Animali_Attributi ")
			StrSQL.AppendLine("   ON Lista_Specie_Animali.Gen_Cod = Lista_Categorie_Animali_Attributi.Gen_Cod ")
			StrSQL.AppendLine("   AND Lista_Specie_Animali.Spe_Cod = Lista_Categorie_Animali_Attributi.Spe_Cod ")

			StrSQL.AppendLine(" WHERE Lista_Specie_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
			StrSQL.AppendLine(" AND   Lista_Specie_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
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
					StrSQL.AppendLine(" AND   Lista_Specie_Animali.Inviato >=0 ")
				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
					StrSQL.AppendLine(" AND   Lista_Specie_Animali.Inviato =-1 ")
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





'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################