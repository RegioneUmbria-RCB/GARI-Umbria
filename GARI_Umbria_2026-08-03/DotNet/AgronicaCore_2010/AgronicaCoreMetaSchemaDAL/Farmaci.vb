Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO

Public Class Farmaci
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function leggi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef Farm_Cod As Integer,
                          ByVal Aic() As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Farmaci.leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------------
            Stb.AppendLine("SELECT * ")
            Stb.AppendLine(" FROM [dbo].[Farmaci] ")
            Stb.AppendLine(" WHERE 1 = 1 ")

            If Farm_Cod <> 0 Then
                Stb.AppendLine(" AND Farm_Cod = " & Agro_SQL_SaveText_NULL(Farm_Cod) & " ")
            End If

            If Aic IsNot Nothing AndAlso Aic.Length > 0 Then
                Stb.AppendLine(" AND (")
                Stb.AppendLine(String.Join(" OR ", Aic.Select(Function(codice) "AIC LIKE " & Agro_SQL_SaveText_NULL(codice & "%") & " ")))
                Stb.AppendLine(") ")
                'Stb.AppendLine(" AND AIC LIKE " & Agro_SQL_SaveText_NULL(Aic & "%") & " ")
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

    Public Function FarmCod_Da_CodiceAIC(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef Aic As String) As Integer
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Farmaci.FarmCod_Da_CodiceAIC()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Res As Integer = 0

        Try

            Dim dt = leggi(objParametri_Server, 0, {Aic})

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Res = dt.Rows(0)("Farm_Cod")

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Res

    End Function

    Public Function CodiceAIC_Da_FarmCod(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef Farm_Cod As Integer) As String
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Farmaci.CodiceAIC_Da_FarmCod()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Res As String = ""

        Try

            Dim dt = leggi(objParametri_Server, Farm_Cod, {})

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Res = dt.Rows(0)("AIC")

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Res

    End Function

    Public Function leggi_Categorie(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef Id As Integer,
                          ByRef Categoria_Codice As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Farmaci.leggi_Categorie()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------------
            Stb.AppendLine("SELECT * ")
            Stb.AppendLine(" FROM [dbo].[Farmaci_Categorie] ")
            Stb.AppendLine(" WHERE 1 = 1 ")

            If Id <> 0 Then
                Stb.AppendLine(" AND Id = " + Agro_SQL_SaveNum(Id) + " ")
            End If

            If Categoria_Codice <> "" Then
                Stb.AppendLine(" AND Categoria_Codice = " + Agro_SQL_SaveText_NULL(Categoria_Codice) + " ")
            End If

            Stb.AppendLine(" ORDER BY Categoria_Codice ")

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

    Public Function leggi_Categorie_Semplificate(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef Id As Integer,
                          ByRef Categoria_Codice As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Farmaci.leggi_Categorie()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '-----------------------------------------------------------------------------------------------------------------
            Stb.AppendLine("SELECT * ")
            Stb.AppendLine(" FROM [dbo].[Farmaci_Categorie_Semplificate] ")
            Stb.AppendLine(" WHERE 1 = 1 ")

            If Id <> 0 Then
                Stb.AppendLine(" AND Id = " + Agro_SQL_SaveNum(Id) + " ")
            End If

            If Categoria_Codice <> "" Then
                Stb.AppendLine(" AND Descrizione = " + Agro_SQL_SaveText_NULL(Categoria_Codice) + " ")
            End If

            Stb.AppendLine(" ORDER BY Descrizione ")

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
