Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Codifica_Macchine_Agea
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function leggi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef AGEA_Cod As String,
                          ByRef AGEA_Des As String,
                          ByRef Class_Cod As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea.leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.AppendLine("Select * ")
            Stb.AppendLine(" From [dbo].[Codifica_Macchine_Agea] ")
            Stb.AppendLine(" Where 1 = 1 ")

            If AGEA_Cod <> "" Then
                Stb.AppendLine(" AND AGEA_Cod = " + Agro_SQL_SaveText_NULL(AGEA_Cod) + " ")
            End If

            If AGEA_Des <> "" Then
                Stb.AppendLine(" AND AGEA_Des = " + Agro_SQL_SaveText_NULL(AGEA_Des) + " ")
            End If

            If Class_Cod <> "" Then
                Stb.AppendLine(" AND Class_Cod = " + Agro_SQL_SaveText_NULL(Class_Cod) + " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function Class_Cod_da_AGEA_Cod(
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef AGEA_Cod As String,
                                         ByRef AGEA_Des As String
                                         ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea.Class_Cod_da_AGEA_Cod()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Str_Res As String = ""
        AGEA_Des = ""

        Try

            Dim dt = leggi(objParametri_Server, AGEA_Cod, "", "")

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                AGEA_Des = dt.Rows(0)("AGEA_Des")
                Str_Res = dt.Rows(0)("Class_Cod")

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Str_Res

    End Function

End Class
