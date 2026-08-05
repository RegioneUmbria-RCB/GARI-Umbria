Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Lista_Indprod_Animali_R
	Inherits AgronicaCoreDataProvider.DataProvider

	'################################################################################
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' Nuova versione senza COM+ e con objParametri
	''' </summary>
	''' <param name="GenCod"></param>
	''' <param name="SpeCod"></param>
	''' <param name="IProCod"></param>
	''' <param name="objParametri"></param>
	''' <returns></returns>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[pierantoni]	20/01/2011	Created
	''' </history>
	''' -----------------------------------------------------------------------------
	Public Shared Function StallaIProDes_from_StallaIProCod(ByVal GenCod As Integer,
															ByVal SpeCod As Integer,
															ByVal IProCod As Integer,
															ByRef objParametri As AgronicaCoreParametri
															) As String

		Dim objZoo As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
		Dim dt As DataTable

		dt = objZoo.Leggi(CInt(GenCod),
						  CInt(SpeCod),
						  CInt(IProCod),
						  enumSelezioneVariabile.Selezione_TabellaCompleta,
						  "", "", objParametri)

		'Elimino gli oggetti COM
		objZoo = Nothing

		If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
			Return dt.Rows(0).Item("IPro_Des")
		End If

		dt = Nothing

	End Function


	'##############################################################################################
	<Obsolete("DEPRECATA, usare AgronicaCoreMetaschemaDAL.Lista_IndProd_Animali_R.Leggi()")>
	Public Function Leggi(ByVal Gen_Cod As Long,
						  ByVal Spe_Cod As Long,
						  ByVal Ipro_Cod As Long,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine = "AgronicaCoreZooDAL.Lista_Specie_Animali_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim StrSQL As New StringBuilder
		Dim dt As DataTable

		Try
			StrSQL.Length = 0
			Select Case xSelezioneVariabile

				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					StrSQL.Append("SELECT IPRO_COD, IPRO_DES ")
					StrSQL.Append("FROM  Lista_IndirizziProd_Animali ")
					StrSQL.Append("WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
					StrSQL.Append("    AND Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

					If Gen_Cod <> 0 Then
						StrSQL.Append("    AND GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod))
					End If

					If Spe_Cod <> 0 Then
						StrSQL.Append("    AND SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod))
					End If

					If Ipro_Cod <> -1 Then
						StrSQL.Append("    AND IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod))
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
						StrSQL.Append("ORDER BY IPRO_DES ASC ")
					End If
					'------------------------------------------------------------------

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					StrSQL.Append(" SELECT * ")
					StrSQL.Append(" FROM  Lista_IndirizziProd_Animali ")
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


					If xFiltroAggiuntivo <> "" Then
						StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If
					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							StrSQL.Append(" AND   Lista_IndirizziProd_Animali.Inviato >=0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							StrSQL.Append(" AND   Lista_IndirizziProd_Animali.Inviato =-1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------
					If xOrderBy <> "" Then
						StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						StrSQL.Append(" ORDER BY IPRO_DES ASC ")
					End If
					'------------------------------------------------------------------

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni


				Case enumSelezioneVariabile.Selezione_JoinCompleta


			End Select

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return dt

	End Function

	''' <summary>
	''' DEPRECATA, utilizzare AgronicaCoreMetaschemaDAL.Lista_IndProd_Animali_R.LeggiRisorseZootecniche()	
	''' </summary>
	''' <param name="Gen_Cod"></param>
	''' <param name="Spe_Cod"></param>
	''' <param name="Ipro_Cod"></param>
	''' <param name="xFiltroAggiuntivo"></param>
	''' <param name="xOrderBy"></param>
	''' <param name="objParametri_Server"></param>
	''' <returns></returns>
	Public Function LeggiRisorseZootecniche(ByVal Gen_Cod As Long,
											ByVal Spe_Cod As Long,
											ByVal Ipro_Cod As Long,
											ByVal xFiltroAggiuntivo As String,
											ByVal xOrderBy As String,
											ByRef objParametri_Server As AgronicaCoreParametri
											) As DataTable

		Const nomeRoutine = "AgronicaCoreZooDAL.Lista_Specie_Animali_R.LeggiRisorseZootecniche()"

		Dim messaggioErrore As String = ""
		Dim StrSQL As New Text.StringBuilder
		Dim dt As DataTable

		Try

			StrSQL.Length = 0
			StrSQL.AppendLine("SELECT ")
			StrSQL.AppendLine(" s.GEN_COD, s.SPE_COD, i.IPRO_COD, CONCAT((g.GEN_DES), ' - ', CASE WHEN i.IPRO_COD > 0 THEN CONCAT(s.SPE_DES, ' - ', i.IPRO_DES) ELSE s.SPE_DES END) as ZOO_DES, ")
			StrSQL.AppendLine(" replace(CONCAT((g.GEN_DES), ' - ', CASE WHEN i.IPRO_COD > 0 THEN CONCAT(s.SPE_DES, ' - ', i.IPRO_DES) ELSE s.SPE_DES END), '(', '') as campo_ordinamento ")
			StrSQL.AppendLine(" FROM Lista_Specie_Animali s WITH(NOLOCK)")
			StrSQL.AppendLine(" INNER JOIN Lista_Generi_Animali g WITH(NOLOCK)")
			StrSQL.AppendLine(" ON s.GEN_COD = g.GEN_COD ")
			StrSQL.AppendLine(" LEFT JOIN Lista_IndirizziProd_Animali i WITH(NOLOCK)")
			StrSQL.AppendLine(" ON s.GEN_COD = i.GEN_COD and s.SPE_COD = i.SPE_COD ")
			StrSQL.AppendLine(" WHERE 1 = 1 ")
			StrSQL.AppendLine(" AND s.GEN_COD <> 0 ")

			If Gen_Cod <> 0 Then
				StrSQL.Append(" AND s.GEN_COD = " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
			End If

			If Spe_Cod <> 0 Then
				StrSQL.Append(" AND s.SPE_COD = " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
			End If

			If Ipro_Cod <> -1 Then
				StrSQL.Append(" AND i.IPRO_COD = " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
			End If

			If xFiltroAggiuntivo <> "" Then
				StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
			End If

			If xOrderBy <> "" Then
				StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
			Else
				StrSQL.AppendLine(" ORDER BY campo_ordinamento ")
			End If

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return dt

	End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
