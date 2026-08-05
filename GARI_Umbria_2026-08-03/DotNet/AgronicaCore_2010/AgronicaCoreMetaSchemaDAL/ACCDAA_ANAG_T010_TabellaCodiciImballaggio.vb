Imports AgronicaCoreDataProvider.UtilityProvider
Public Class ACCDAA_ANAG_T010_TabellaCodiciImballaggio_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Codice As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.ACCDAA_ANAG_T010_TabellaCodiciImballaggio_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  ACCDAA_ANAG_T010_TabellaCodiciImballaggio ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")


            If Trim(Codice) <> "" Then
                StrSQL.AppendLine(" AND Codice =  '" & Agro_SQL_SaveText(Codice) & "'  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" ORDER BY [Descrizione Italiana] ASC ")


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



