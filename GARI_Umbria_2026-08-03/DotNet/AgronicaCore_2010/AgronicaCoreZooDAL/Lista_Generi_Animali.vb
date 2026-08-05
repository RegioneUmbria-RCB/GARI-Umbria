Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_Generi_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Generi_Animali_R.Leggi()")>
	Public Function Leggi(ByVal Gen_Cod As Integer,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreZooDAL.Lista_Generi_Animali_R.Leggi()"

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New StringBuilder
		Dim DT As DataTable

		Try
			StrSQL.Length = 0

			Select Case xSelezioneVariabile

				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					StrSQL.Append("SELECT GEN_COD, GEN_DES ")
					StrSQL.Append("FROM Lista_Generi_Animali ")
					StrSQL.Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
					StrSQL.Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> -1 Then
						StrSQL.Append("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))
					End If

					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append("ORDER BY GEN_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					StrSQL.Append("SELECT * ")
					StrSQL.Append("FROM Lista_Generi_Animali ")
					StrSQL.Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
					StrSQL.Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> 0 Then
						StrSQL.Append("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))
					End If

					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append("ORDER BY GEN_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni

				Case enumSelezioneVariabile.Selezione_JoinCompleta

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

End Class

