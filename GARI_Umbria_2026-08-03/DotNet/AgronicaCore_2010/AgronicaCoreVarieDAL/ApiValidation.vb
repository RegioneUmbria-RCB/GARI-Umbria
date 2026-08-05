Public Class ApiValidation_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ApivalidationForceDowngrade(ByVal xAppName As String,
                                        ByVal xAppVersion As String,
                                        ByVal xPlatform As String,
                                        ByVal xEnvironment As String,
                                        ByVal inputVersionAPI As String,
                                        ByVal isForcedDowngrade As Boolean,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "ApiValidation_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If (Not String.IsNullOrEmpty(inputVersionAPI)) Then
                StrSQL.Length = 0
                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" FROM  ApiValidation")
                StrSQL.Append(" where 1 = 1 ")

                If isForcedDowngrade = False Then
                    StrSQL.Append(" AND  xAppName = '" & Agro_SQL_SaveText(xAppName) & "' ")
                    StrSQL.Append(" AND  xPlatform = '" & Agro_SQL_SaveText(xPlatform) & "'  ")
                    StrSQL.Append(" AND  xEnvironment =  '" & Agro_SQL_SaveText(xEnvironment) & "'  ")
                    StrSQL.Append(" AND  xAppVersion = '" & Agro_SQL_SaveText(xAppVersion) & "' ")
                    StrSQL.Append(" AND  CAST(APIminVersion AS DECIMAL(10,2))  <= CAST('" & Agro_SQL_SaveText(inputVersionAPI) & "' AS DECIMAL(10,2))  ")
                    StrSQL.Append(" AND CAST(APImaxVersion AS DECIMAL(10,2))  >=  CAST('" & Agro_SQL_SaveText(inputVersionAPI) & "' AS DECIMAL(10,2))  ")
                ElseIf isForcedDowngrade = True Then
                    StrSQL.Append(" AND  xAppName = '" & Agro_SQL_SaveText(xAppName) & "' ")
                    StrSQL.Append(" AND  xPlatform = '" & Agro_SQL_SaveText(xPlatform) & "'  ")
                    StrSQL.Append(" AND  xEnvironment = '" & Agro_SQL_SaveText(xEnvironment) & "'  ")
                    StrSQL.Append(" AND  xAppVersion = '" & Agro_SQL_SaveText(xAppVersion) & "' ")
                    StrSQL.Append(" AND  CAST(APIminVersion AS DECIMAL(10,2)) > CAST('" & Agro_SQL_SaveText(inputVersionAPI) & "' AS DECIMAL(10,2))  ")
                End If

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
    Public Function VerificaVersioneAPP(ByVal xAppName As String,
                                        ByVal xAppVersion As String,
                                        ByVal xPlatform As String,
                                        ByVal xEnvironment As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "ApiValidation_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" FROM  ApiValidation")
            StrSQL.Append(" where 1 = 1 ")
            StrSQL.Append(" AND  xAppName = '" & Agro_SQL_SaveText(xAppName) & "' ")
            StrSQL.Append(" AND  xPlatform = '" & Agro_SQL_SaveText(xPlatform) & "'  ")
            StrSQL.Append(" AND  xEnvironment =  '" & Agro_SQL_SaveText(xEnvironment) & "'  ")
            StrSQL.Append(" AND  xAppVersion = '" & Agro_SQL_SaveText(xAppVersion) & "' ")

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
