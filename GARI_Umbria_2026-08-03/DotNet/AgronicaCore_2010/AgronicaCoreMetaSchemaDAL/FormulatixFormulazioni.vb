Public Class FormulatixFormulazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################################
    'Restituisce un DataTable contenente Fr_Cod, Fr_Des e Polverulento (1=Polverulento,0=non Polverulento)
    Public Function Leggi_Polverulenti(ByVal FR_COD As String,
                                       ByVal Stato_Cod As String,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixFormulazioni_R.Leggi_Polverulenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Formulati.Fr_Cod, Formulati.Fr_Des,  ")
            StrSQL.Append("   Case When Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) In ('DP', 'DS') Then 1 Else 0 End AS Polverulento ")

            StrSQL.Append("  FROM  FormulatiXFormulazioni INNER JOIN ")
            StrSQL.Append("  Formulati ON FormulatiXFormulazioni.Fr_Cod = Formulati.Fr_Cod ")

            StrSQL.Append("  WHERE FormulatiXFormulazioni.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("  AND   FormulatiXFormulazioni.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("  AND   Formulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("  AND   Formulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

            If FR_COD <> "0" Then
                StrSQL.Append(" AND FormulatiXFormulazioni.FR_COD IN (" & Agro_SQL_Save_Clausola_IN(FR_COD) & ")  ")
            End If

            If Stato_Cod <> "" Then
                StrSQL.Append(" AND Formulati.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "'")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_COD ASC ")
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
