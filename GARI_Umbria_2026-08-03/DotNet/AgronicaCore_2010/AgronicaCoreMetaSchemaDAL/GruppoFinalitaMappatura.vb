Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class GruppoFinalitaMappatura_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Veg_Cod As Integer,
                          ByVal Grfi_Cod_Rer As Long, ByVal Grfi_Cod_Gias As Long,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoFinalitaMappatura_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Append(" SELECT GruppoFinalitaMappatura.veg_cod, GruppoFinalitaMappatura.grfi_cod_gias, GruppoFinalitaMappatura.grfi_cod_pua, ")
            StrSQL.Append(" ISNULL(GruppoFinalita.grfi_des,'') as grfi_des_gias, ISNULL(GruppoFinalita_Rer.grfi_des,'') as grfi_des_pua ")
            StrSQL.Append(" FROM    GruppoFinalitaMappatura ")
            StrSQL.Append(" INNER JOIN   GruppoFinalita ON GruppoFinalitaMappatura.grfi_cod_gias = GruppoFinalita.grfi_cod ")
            StrSQL.Append(" INNER JOIN   GruppoFinalita_Rer ON GruppoFinalitaMappatura.grfi_cod_pua = GruppoFinalita_Rer.grfi_cod ")

            StrSQL.Append(" WHERE   1 = 1 ")

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Grfi_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Grfi_Cod_Gias = " & Agro_SQL_SaveNum(Grfi_Cod_Gias) & " ")
            End If

            If Grfi_Cod_Rer <> 0 Then
                StrSQL.Append(" AND Grfi_Cod_Rer = " & Agro_SQL_SaveNum(Grfi_Cod_Rer) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & " ")
            Else
                StrSQL.Append(" ORDER BY veg_cod, grfi_cod_pua, grfi_cod_gias ")
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
