Imports System.Text
Imports AgronicaCoreDataProvider

Public Class Codifica_BDN_GeneriAnimali
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Read(ByRef objP_Server As AgronicaCoreParametri,
						 Optional ByVal GrSpe_Id As String = "",
						 Optional ByVal Gen_Cod As String = "",
						 Optional ByVal Sistema_Cod As String = "") As DataTable
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_BDN_GeneriAnimali.Read()"

		Dim dt As DataTable
		Dim stb As New StringBuilder

		Try
			'-----------------------------------------------------------------------------------------------------------------
			stb.AppendLine("SELECT * FROM Codifica_BDN_GeneriAnimali ").
				AppendLine("WHERE 1=1 ")

			If GrSpe_Id <> "" Then stb.AppendLine("    AND GRSPE_ID = " + Agro_SQL_SaveText_NULL(GrSpe_Id) + " ")

			If Gen_Cod <> "" Then stb.AppendLine("    AND GEN_COD = " + Agro_SQL_SaveText_NULL(Gen_Cod) + " ")

			If Sistema_Cod <> "" Then stb.AppendLine("    AND Sistema_Cod = " + Agro_SQL_SaveText_NULL(Sistema_Cod) + " ")

			'-----------------------------------------------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Server, stb.ToString, NomeRoutine)
			'-----------------------------------------------------------------------------------------------------------------

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return dt

	End Function

	Public Function Gen_Cod_da_GrSpe_Id(ByRef objP_Server As AgronicaCoreParametri,
										ByVal GrSpe_Id As String) As String
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea.Gen_Cod_da_GrSpe_Id()"

		Dim res As String = ""
		Dim stb As New StringBuilder

		Try
			Dim dt = Read(objP_Server, GrSpe_Id)
			If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then res = dt.Rows(0)("Gen_Cod")

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return res

	End Function

	Public Function GrSpe_Id_da_Gen_Cod(ByRef objP_Server As AgronicaCoreParametri,
										ByVal Gen_Cod As String) As String
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea.GrSpe_Id_da_Gen_Cod()"

		Dim res As String = ""
		Dim stb As New StringBuilder

		Try
			Dim dt = Read(objP_Server, Gen_Cod:=Gen_Cod)
			If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then res = dt.Rows(0)("GrSpe_Id")

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return res

	End Function

	Public Function SpeCodice_From_GrSpeId(ByVal GrSpe_Id As Integer,
										   ByRef objP_Server As AgronicaCoreParametri) As String
		Const NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea.SpeCodice_From_GrSpeId()"

		Dim res As String = ""
		Dim Stb As New StringBuilder

		Try
			Dim dt = Read(objP_Server, GrSpe_Id)
			If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then res = $"0{dt.Rows(0)("CODICE")}"

		Catch ex As Exception
			Scrivi_LOG(objP_Server, NomeRoutine, ex.Message)
			Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
		End Try

		Return res

	End Function

End Class
