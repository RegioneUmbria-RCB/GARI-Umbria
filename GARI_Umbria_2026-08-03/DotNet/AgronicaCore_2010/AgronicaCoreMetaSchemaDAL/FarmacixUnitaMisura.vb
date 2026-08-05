Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports System.Data.DataSetExtensions

Public Class FarmacixUnitaMisura
	Inherits AgronicaCoreDataProvider.DataProvider

	Private Const UDM_NON_DEFINITO As Integer = 0

	''' <summary>
	''' 
	''' </summary>
	''' <param name="Farm_Cod">per non filtrare = 0</param>
	''' <param name="Udm_Cod">per non filtrare = -1</param>
	''' <param name="objP_Server"></param>
	''' <param name="xSelezioneVariabile"></param>
	''' <returns></returns>
	Public Function Leggi(ByRef Farm_Cod As Integer,
						  ByRef Udm_Cod As Integer,
						  ByRef objP_Server As AgronicaCoreParametri,
						  Optional ByVal xSelezioneVariabile As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
						  Optional ByVal AIC As String = "") As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FarmacixUnitaMisura.Leggi()"

		Dim msgErrore As String = ""
		Dim stb As New StringBuilder
		Dim dt As DataTable

		If AIC <> "" Then
			xSelezioneVariabile = enumSelezioneVariabile.Selezione_JoinCompleta
		End If

		Try

			Select Case xSelezioneVariabile

				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					stb.AppendLine("SELECT Farm_Cod, Udm_Cod, [Default], QtaxUnita ").
					AppendLine("FROM [dbo].[FarmacixUnitaMisura] farmxUdm ")

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					stb.AppendLine("SELECT * ").
					AppendLine("FROM [dbo].[FarmacixUnitaMisura] farmxUdm ")

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni
					stb.AppendLine("SELECT farmxUdm.Farm_Cod, farmxUdm.Udm_Cod, farmxUdm.[Default], farmxUdm.QtaxUnita, ").
					AppendLine("    farm.Denominazione, farm.Confezione, ").
					AppendLine("    udm.UDM_SIM, udm.UDM_DES ").
					AppendLine("FROM [dbo].[FarmacixUnitaMisura] farmxUdm ").
					AppendLine("INNER JOIN [dbo].[Farmaci] farm ON farm.Farm_Cod = farmxUdm.Farm_Cod ").
					AppendLine("INNER JOIN [dbo].[UnitaMisura] udm ON udm.UDM_COD = farmxUdm.Udm_Cod ")


				Case enumSelezioneVariabile.Selezione_JoinCompleta
					stb.AppendLine("SELECT farmxUdm.Farm_Cod, farmxUdm.Udm_Cod, farmxUdm.[Default], farmxUdm.QtaxUnita, ").
					AppendLine("    farm.AIC, farm.Denominazione, farm.Confezione, farm.ModalitaPrescrizione, ").
					AppendLine("    udm.UDM_COD_AUX, udm.UDM_SIM, udm.UDM_DES, udm.TipoControllo_Cod ").
					AppendLine("FROM [dbo].[FarmacixUnitaMisura] farmxUdm ").
					AppendLine("INNER JOIN [dbo].[Farmaci] farm ON farm.Farm_Cod = farmxUdm.Farm_Cod ").
					AppendLine("INNER JOIN [dbo].[UnitaMisura] udm ON udm.UDM_COD = farmxUdm.Udm_Cod ")

			End Select

			stb.AppendLine("WHERE 1 = 1 ")

			If Farm_Cod <> 0 Then stb.AppendLine("    AND farmxUdm.Farm_Cod = " + Agro_SQL_SaveNum(Farm_Cod))

			If Udm_Cod <> -1 Then stb.AppendLine("    AND farmxUdm.Udm_Cod = " + Agro_SQL_SaveNum(Udm_Cod))

			If AIC <> "" Then stb.AppendLine("    AND farm.AIC = '" + Agro_SQL_SaveText(AIC) + "' ")

			'-----------------------------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Server, stb.ToString, NomeRoutine)
			'-----------------------------------------------------------------------------------------------------------------


		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objP_Server, NomeRoutine, msgErrore)
			dt = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return dt


	End Function


	Public Function DefaultUdmFromFarmaco(ByRef Farm_Cod As Integer,
										  ByRef objP_Server As AgronicaCoreParametri) As Integer
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FarmacixUnitaMisura.DefaultUdmFromFarmaco()"

		Dim msgErrore As String = ""
		Dim stb As New StringBuilder
		Dim defaultUdm As Integer = UDM_NON_DEFINITO

		Try
			Dim dt = Leggi(Farm_Cod, -1, objP_Server)

			If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then defaultUdm = dt.Select("Default = 1").FirstOrDefault()("Udm_Cod")

		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objP_Server, NomeRoutine, msgErrore)
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return defaultUdm

	End Function

	Public Function DefaultUdmFromFarmacoAic(ByRef AIC As String,
										  ByRef objP_Server As AgronicaCoreParametri) As Integer
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FarmacixUnitaMisura.DefaultUdmFromFarmaco()"

		Dim msgErrore As String = ""
		Dim stb As New StringBuilder
		Dim defaultUdm As Integer = UDM_NON_DEFINITO

		Try
			Dim dt = Leggi(0, -1, objP_Server, enumSelezioneVariabile.Selezione_JoinCompleta, AIC)

			If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then defaultUdm = dt.Select("Default = 1").FirstOrDefault()("Udm_Cod")

		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objP_Server, NomeRoutine, msgErrore)
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return defaultUdm

	End Function

	Public Function DefaultFromFarmacoAic(ByRef AIC As String,
										  ByRef objP_Server As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FarmacixUnitaMisura.DefaultUdmFromFarmaco()"

		Dim msgErrore As String = ""
		Dim stb As New StringBuilder
		Dim defaultDT As DataTable = Nothing

		Try
			Dim dt = Leggi(0, -1, objP_Server, enumSelezioneVariabile.Selezione_JoinCompleta, AIC)

			If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
				Dim filteredRows = dt.Select("Default = 1")
				If filteredRows.Length > 0 Then
					defaultDT = dt.Clone() ' Crea la struttura
					For Each row As DataRow In filteredRows
						defaultDT.ImportRow(row) ' Importa ogni riga
					Next
				End If
			End If

		Catch ex As Exception
			msgErrore = ex.Message
			Scrivi_LOG(objP_Server, NomeRoutine, msgErrore)
			Throw New Exception("[" & NomeRoutine & "] : " & msgErrore)
		End Try

		Return defaultDT

	End Function

End Class
