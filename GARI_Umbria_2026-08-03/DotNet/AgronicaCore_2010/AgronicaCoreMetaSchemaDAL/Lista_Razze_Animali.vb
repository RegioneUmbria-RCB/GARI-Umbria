Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports System.Text


Public Class Lista_Razze_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'##############################################################################################
	''' <summary>
	''' 
	''' </summary>
	''' <param name="Gen_Cod">Passare -1 per non filtrare</param>
	''' <param name="Spe_Cod">Passare -1 per non filtrare</param>
	''' <param name="Raz_Cod">Passare -1 per non filtrare</param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	Public Function Leggi(ByVal Gen_Cod As Integer,
						  ByVal Spe_Cod As Integer,
						  ByVal Raz_Cod As Integer,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Razze_Animali_R.Leggi()"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					strSQL.Append("SELECT RAZ_COD, RAZ_DES ").
						Append("FROM Lista_Razze_Animali ").
						Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)).
						Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))

					If Raz_Cod <> -1 Then strSQL.AppendLine("    AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod))

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'----------------------------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'----------------------------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY RAZ_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM  Lista_Razze_Animali ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Gen_Cod <> -1 Then strSQL.AppendLine("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))

					If Spe_Cod <> -1 Then strSQL.AppendLine("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))

					If Raz_Cod <> -1 Then strSQL.AppendLine("    AND RAZ_COD = " & Agro_SQL_SaveNum(Raz_Cod))

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'----------------------------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'----------------------------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY RAZ_DES ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni

				Case enumSelezioneVariabile.Selezione_JoinCompleta

			End Select

			'----------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
			'----------------------------------------------------------------------------------------------

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
	''' <param name="Piva"></param>
	''' <param name="Sa_Cod"></param>
	''' <param name="STA_NUM"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	Public Function Leggi_Da_Stalla(ByVal Piva As String,
									ByVal Sa_Cod As Integer,
									ByVal STA_NUM As Integer,
									ByVal xFiltroAggiuntivo As String,
									ByVal xOrderBy As String,
									ByRef objParametri As AgronicaCoreParametri
									) As DataTable

		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Razze_Animali_R.Leggi_Da_Stalla()"

		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			strSQL.AppendLine("SELECT Raz_Cod, Raz_Des ").
				AppendLine("FROM Lista_Razze_Animali ").
				AppendLine("INNER JOIN Stalla ON Stalla.Piva = '" & Agro_SQL_SaveText(Piva) & "' ").
				AppendLine("    AND Stalla.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ").
				AppendLine("    AND Stalla.STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & " ").
				AppendLine("    AND Lista_Razze_Animali.Gen_Cod = Stalla.Gen_Cod ").
				AppendLine("    AND Lista_Razze_Animali.Spe_Cod = Stalla.Spe_Cod ").
				AppendLine("WHERE Lista_Razze_Animali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
				AppendLine("    AND Lista_Razze_Animali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

			'--------------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

			'--------------------------------------------------------------------------
			Select Case objParametri.FlagVisibilita
				Case enumVisibilita.Visibilita_SoloNonCancellati
					strSQL.AppendLine("    AND Lista_Razze_Animali.Inviato >=0 ")
				Case enumVisibilita.Visibilita_SoloCancellati
					strSQL.AppendLine("    AND Lista_Razze_Animali.Inviato =-1 ")
				Case enumVisibilita.Visibilita_Tutti

				Case Else
					Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
			End Select

			'--------------------------------------------------------------------------
			If xOrderBy <> "" Then
				strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
			Else
				'StrSQL.AppendLine("ORDER BY Desc_CorpoEstraneo")
			End If

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


End Class
