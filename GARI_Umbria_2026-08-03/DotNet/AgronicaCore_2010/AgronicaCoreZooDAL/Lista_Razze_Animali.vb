
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports System.Text


Public Class Lista_Razze_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider


	'##############################################################################################
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Razze_Animali_R.Leggi()
	''' Da usare con Selezione_TabellaCompleta
	''' </summary>
	''' <param name="Gen_Cod"></param>
	''' <param name="Spe_Cod"></param>
	''' <param name="Raz_Cod"></param>
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
	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_Razze_Animali_R.Leggi()")>
	Public Function Leggi(ByVal Gen_Cod As Integer,
						  ByVal Spe_Cod As Integer,
						  ByVal Raz_Cod As Integer,
						  ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreZooDAL.Lista_Razze_Animali_R.Leggi()"

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try
			StrSQL.Length = 0

			Select Case xSelezioneVariabile

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					StrSQL.Append("SELECT RAZ_COD, RAZ_DES ")
					StrSQL.Append("FROM Lista_Razze_Animali ")
					StrSQL.Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
					StrSQL.Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> 0 Then
						StrSQL.Append("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))
					End If

					If Raz_Cod <> -1 Then
						StrSQL.Append("    AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod))
					End If

					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append("    AND dbo.Lista_Razze_Animali.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append("    AND dbo.Lista_Razze_Animali.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append("ORDER BY RAZ_DES ASC ")
					End If

				Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
					StrSQL.Append(" SELECT * ")
					StrSQL.Append(" FROM  Lista_Razze_Animali ")
					StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


					If Gen_Cod <> 0 Then
						StrSQL.Append(" AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append(" AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
					End If

					If Raz_Cod <> -1 Then
						StrSQL.Append(" AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
					End If


					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append(" AND     dbo.Lista_Razze_Animali.Inviato >= 0 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append(" AND     dbo.Lista_Razze_Animali.Inviato = -1 ")
						Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append(" ORDER BY RAZ_DES ASC ")
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

	Public Function GetRazzaGiasFromFrancese(ByVal Razza_Codice As Integer,
											 ByRef objP_Server As AgronicaCoreParametri) As Integer
		Const NomeRoutine As String = "AgronicaCoreZooDAL.Lista_Razze_Animali_R.GetRazzaGiasFromFrancese()"

		Dim razCod As Integer = -1
		Dim strSql As New StringBuilder

		Try
			strSql.Length = 0

			strSql.AppendLine("SELECT listRzz.GEN_COD, listRzz.SPE_COD, listRzz.RAZ_COD, listRzz.RAZ_DES, ").
				AppendLine("    cacRzz.Argomento_Cod AS Cod_Razza_FR, cacRzz.TestoAux_1 AS Des_Razza_FR ").
				AppendLine("FROM Cac_Codifica_InfoAggiuntive cacRzz ").
				AppendLine("LEFT JOIN Codifica_BDN_RazzeAnimali confRzzBdn ON confRzzBdn.CODICE = cacRzz.TestoAux_1 ").
				AppendLine("LEFT JOIN Lista_Razze_Animali listRzz ON listRzz.GEN_COD = confRzzBdn.GEN_COD ").
				AppendLine("    AND listRzz.SPE_COD = confRzzBdn.SPE_COD ").
				AppendLine("    AND listRzz.RAZ_COD = confRzzBdn.RAZ_COD ").
				AppendLine("WHERE InfoAgg_Cod = " & Agro_SQL_SaveNum(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CodificaRazzeTRACESNT) & " ").
				AppendLine("    AND cacRzz.Argomento_Cod =  " & Agro_SQL_SaveNum(Razza_Codice) & " ")

			Dim dtRazzaFranceToGias As DataTable = EseguiQuery_Lettura(objP_Server, strSql.ToString, NomeRoutine)
			If Not IsNothing(dtRazzaFranceToGias) AndAlso dtRazzaFranceToGias.Rows.Count > 0 Then
				razCod = dtRazzaFranceToGias.Rows(0).Item("RAZ_COD")
			End If

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return razCod

	End Function


End Class
