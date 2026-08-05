Imports System.Text
Imports AgronicaCoreDataProvider

Public Class BDN_MotiviIngressoUscita
	Inherits DataProvider

	Public Function Read_MotiviIngresso(ByVal Codice As String,
										ByRef objP_Server As AgronicaCoreParametri,
										Optional ByVal Tipo As String = "") As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.BDN_MotiviIngressoUscita.Read_MotiviIngresso()"

		Dim dt As DataTable
		Dim stb As New StringBuilder

		Try
			stb.Length = 0

			stb.AppendLine("SELECT * ").
				AppendLine("FROM BDN_MotiviIngresso ").
				AppendLine("WHERE 1 = 1 ")

			If Codice <> "" Then stb.AppendLine($"    AND Codice = {Agro_SQL_SaveText_NULL(Codice)} ")

			If Tipo <> "" Then stb.AppendLine($"    AND Tipo = {Agro_SQL_SaveText_NULL(Tipo)} ")

			'-----------------------------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Server, stb.ToString, NomeRoutine)
			'-----------------------------------------------------------------------------------------------------------------

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return dt

	End Function

	Public Function Read_MotiviUscita(ByVal Codice As String,
									  ByRef objP_Server As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.BDN_MotiviIngressoUscita.Read_MotiviUscita()"

		Dim dt As DataTable
		Dim stb As New StringBuilder

		Try
			stb.Length = 0

			stb.AppendLine("SELECT * ").
				AppendLine("FROM BDN_MotiviUscita ").
				AppendLine("WHERE 1 = 1 ")

			If Codice <> "" Then stb.AppendLine($"    AND Codice = {Agro_SQL_SaveText_NULL(Codice)} ")

			'-----------------------------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Server, stb.ToString, NomeRoutine)
			'-----------------------------------------------------------------------------------------------------------------

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return dt

	End Function

	Public Function ReadAll_MotiviIngresso(ByRef objP_Server As AgronicaCoreParametri,
										   Optional ByVal Tipo As String = "") As DataTable
		Return Read_MotiviIngresso("", objP_Server, Tipo)
	End Function

	Public Function ReadAll_MotiviUscita(ByRef objP_Server As AgronicaCoreParametri) As DataTable
		Return Read_MotiviUscita("", objP_Server)
	End Function

End Class
