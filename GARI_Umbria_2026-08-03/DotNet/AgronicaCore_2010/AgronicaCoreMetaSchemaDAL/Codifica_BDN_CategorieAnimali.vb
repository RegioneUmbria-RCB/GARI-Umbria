Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Codifica_BDN_CategorieAnimali
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function leggi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef Tip_TipPr_Id As String,
                          ByRef Orient_Prod_Id As String,
                          ByRef Gen_Cod As String,
                          ByRef Spe_Cod As String,
                          ByRef IProd_Cod As String,
                          ByRef Cat_Cod As String,
                          ByRef Sistema_Cod As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_BDN_CategorieAnimali.leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------------
            Stb.AppendLine("SELECT * ")
            Stb.AppendLine(" FROM [dbo].[Codifica_BDN_CategorieAnimali] ")
            Stb.AppendLine(" WHERE 1 = 1 ")

            If Tip_TipPr_Id <> "" Then
                Stb.AppendLine(" AND TIP_TIPPR_ID = " + Agro_SQL_SaveText_NULL(Tip_TipPr_Id) + " ")
            End If

            If Orient_Prod_Id <> "" Then
                Stb.AppendLine(" AND ORIENT_PROD_ID = " + Agro_SQL_SaveText_NULL(Orient_Prod_Id) + " ")
            End If

            If Gen_Cod <> "" Then
                Stb.AppendLine(" AND GEN_COD = " + Agro_SQL_SaveText_NULL(Gen_Cod) + " ")
            End If

            If Spe_Cod <> "" Then
                Stb.AppendLine(" AND SPE_COD = " + Agro_SQL_SaveText_NULL(Spe_Cod) + " ")
            End If

            If IProd_Cod <> "" Then
                Stb.AppendLine(" AND IPROD_COD = " + Agro_SQL_SaveText_NULL(IProd_Cod) + " ")
            End If

            If Cat_Cod <> "" Then
                Stb.AppendLine(" AND CAT_COD = " + Agro_SQL_SaveText_NULL(Cat_Cod) + " ")
            End If

            If Sistema_Cod <> "" Then
                Stb.AppendLine(" AND Sistema_Cod = " + Agro_SQL_SaveText_NULL(Sistema_Cod) + " ")
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
