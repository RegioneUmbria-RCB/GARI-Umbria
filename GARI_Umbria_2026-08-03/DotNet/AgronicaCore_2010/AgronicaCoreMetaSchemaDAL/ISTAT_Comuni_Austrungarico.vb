Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ISTAT_Comuni_Austrungarico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Cod_Prov As String, _
                          ByVal Cod_CC As String, _
                          ByVal Pro_Cod_Istat As String, _
                          ByVal Com_Cod_Istat As String, _
                          ByVal Provincia_Sigla As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ISTAT_Comuni_Austrungarico_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  ISTAT_Comuni_Austrungarico ")

            If Cod_Prov <> "" Then
                StrSQL.Append(" AND Cod_Prov = '" & Agro_SQL_SaveText(Trim(Cod_Prov)) & "' ")
            End If

            If Cod_CC <> "" Then
                StrSQL.Append(" AND Cod_CC = '" & Agro_SQL_SaveText(Trim(Cod_CC)) & "' ")
            End If

            If Pro_Cod_Istat <> "" Then
                StrSQL.Append(" AND Prov = '" & Agro_SQL_SaveText(Trim(Pro_Cod_Istat)) & "' ")
            End If

            If Com_Cod_Istat <> "" Then
                StrSQL.Append(" AND Com = '" & Agro_SQL_SaveText(Trim(Com_Cod_Istat)) & "' ")
            End If

            If Provincia_Sigla <> "" Then
                StrSQL.Append(" AND sigla = '" & Agro_SQL_SaveText(Trim(Provincia_Sigla)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Den ")
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
