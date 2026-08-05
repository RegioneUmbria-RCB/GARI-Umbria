Imports System.Text
Imports AgronicaCoreDataProvider

Public Class BDN_TipologieAllevamento
	Inherits DataProvider

	Public Function Read(ByVal Codice As String,
						 ByRef objP_Server As AgronicaCoreParametri,
						 Optional ByVal Tipo_Variazione As String = "",
						 Optional ByVal GrSpe_ID As Integer = 0) As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.BDN_TipologieAllevamento.Leggi()"

		Dim dt As DataTable
		Dim stb As New StringBuilder

		Try
			stb.Length = 0

			stb.AppendLine("SELECT * ").
				AppendLine("FROM BDN_TipologieAllevamento ").
				AppendLine("WHERE 1 = 1 ")

			If Codice <> "" Then stb.AppendLine($"    AND Codice = {Agro_SQL_SaveText_NULL(Codice)} ")

			If Tipo_Variazione <> "" Then stb.AppendLine($"    AND Tipo_Variazione = {Agro_SQL_SaveText_NULL(Tipo_Variazione)} ")

			If GrSpe_ID <> 0 Then stb.AppendLine($"    AND GrSpe_ID = {Agro_SQL_SaveNum(GrSpe_ID)} ")

			'-----------------------------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Server, stb.ToString, NomeRoutine)
			'-----------------------------------------------------------------------------------------------------------------

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return dt

	End Function

	Public Function ReadAll(ByRef objP_Server As AgronicaCoreParametri,
							Optional ByVal Tipo_Variazione As String = "",
							Optional ByVal GrSpe_ID As Integer = 0) As DataTable
		Return Read("", objP_Server, Tipo_Variazione, GrSpe_ID)
	End Function

End Class
