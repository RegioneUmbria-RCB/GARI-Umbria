Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Codifica_BDN_RazzeAnimali
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function leggi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef Spe_Id As String,
                          ByRef Razza_Id As String,
                          ByRef Gen_Cod As String,
                          ByRef Spe_Cod As String,
                          ByRef Raz_Cod As String,
                          ByRef Sistema_Cod As String,
                          Optional ByRef Codice As String = "") As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_BDN_RazzeAnimali.leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------------
            Stb.AppendLine("SELECT * ")
            Stb.AppendLine(" FROM [dbo].[Codifica_BDN_RazzeAnimali] ")
            Stb.AppendLine(" WHERE 1 = 1 ")

            If Spe_Id <> "" Then
                Stb.AppendLine(" AND SPE_ID = " + Agro_SQL_SaveText_NULL(Spe_Id) + " ")
            End If

            If Razza_Id <> "" Then
                Stb.AppendLine(" AND RAZZA_ID = " + Agro_SQL_SaveText_NULL(Razza_Id) + " ")
            End If

            If Gen_Cod <> "" Then
                Stb.AppendLine(" AND GEN_COD = " + Agro_SQL_SaveText_NULL(Gen_Cod) + " ")
            End If

            If Spe_Cod <> "" Then
                Stb.AppendLine(" AND SPE_COD = " + Agro_SQL_SaveText_NULL(Spe_Cod) + " ")
            End If

            If Raz_Cod <> "" Then
                Stb.AppendLine(" AND RAZ_COD = " + Agro_SQL_SaveText_NULL(Raz_Cod) + " ")
            End If

            If Sistema_Cod <> "" Then
                Stb.AppendLine(" AND Sistema_Cod = " + Agro_SQL_SaveText_NULL(Sistema_Cod) + " ")
            End If

            If Codice <> "" Then
                Stb.AppendLine(" AND Codice = " + Agro_SQL_SaveText_NULL(Codice) + " ")
            End If

            '-----------------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class
