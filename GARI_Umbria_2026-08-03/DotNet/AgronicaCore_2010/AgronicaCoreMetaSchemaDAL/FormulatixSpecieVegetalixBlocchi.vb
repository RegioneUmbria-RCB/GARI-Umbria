Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class FormulatixSpecieVegetalixBlocchi_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal FOR_VEG_COD As Int32,
                            ByVal FR_COD As Integer,
                            ByVal VEG_COD As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixSpecieVegetalixBlocchi.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT FormulatixSpecieVegetalixBlocchi.FOR_VEG_COD ")
            StrSQL.Append("       ,ISNULL(FormulatixSpecieVegetalixBlocchi.epoca_da,0) as epoca_da ")
            StrSQL.Append("       ,ISNULL(FormulatixSpecieVegetalixBlocchi.epoca_a,0) as epoca_a ")

            StrSQL.Append(" FROM FormulatixSpecieVegetalixBlocchi ")
            StrSQL.Append("     inner join FormulatixSpecieVegetali on FormulatixSpecieVegetalixBlocchi.FOR_VEG_COD = FormulatixSpecieVegetali.FOR_VEG_COD ")

            StrSQL.Append(" WHERE 1=1 ")

            If FOR_VEG_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetalixBlocchi.FOR_VEG_COD =  " & Agro_SQL_SaveNum(FOR_VEG_COD) & "  ")
            End If

            If FR_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND (FormulatixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  " &
                                      " OR FormulatixSpecieVegetali.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod " &
                                                                                  " FROM   GruppoColturaleXSpecieVegetali          " &
                                                                                  " WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(VEG_COD) & "))  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY FOR_VEG_COD ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

End Class
