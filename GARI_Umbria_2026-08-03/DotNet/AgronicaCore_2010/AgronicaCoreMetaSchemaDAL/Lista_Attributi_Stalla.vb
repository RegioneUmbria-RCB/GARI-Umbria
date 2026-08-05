Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_Attributi_Stalla_R
	Inherits AgronicaCoreDataProvider.DataProvider

	''' <summary>
	''' 
	''' </summary>
	''' <param name="Cod_Fabb"></param>
	''' <param name="Att_Cod"></param>
	''' <param name="xSelezioneVariabile"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	Public Function Leggi(ByVal Cod_Fabb As String,
						  ByVal Att_Cod As Integer,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Attributi_Stalla_R.Leggi()"
		Dim msgErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Lista_attributi_stalla ").
						AppendLine("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Cod_Fabb <> "" Then strSQL.AppendLine("    AND COD_FABB = '" & Agro_SQL_SaveText(Cod_Fabb) & "'  ")

					If Att_Cod <> 0 Then strSQL.AppendLine("    AND ATT_COD = " & Agro_SQL_SaveNum(Att_Cod) & "  ")

					If xFiltroAggiuntivo <> "" Then strSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >=0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato =-1 ")
						Case enumVisibilita.Visibilita_Tutti

						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select

					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY ATT_des ASC ")
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

End Class