Public Class ClientValidation_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal xAppName As String,
                                        ByVal xAppVersion As String,
                                        ByVal xPlatform As String,
                                        ByVal xEnvironment As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                      ) As DataTable


        Dim NomeRoutine As String = "ClientValidation_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0


            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  ClientValidation")
            StrSQL.Append(" WHERE  1 = 1")


            If xAppName <> "" Then
                StrSQL.Append(" AND  xAppName =  '" & Agro_SQL_SaveText(xAppName) & "' ")
            End If

            If xAppVersion <> "" Then
                StrSQL.Append(" AND  xAppVersion = '" & Agro_SQL_SaveText(xAppVersion) & "' ")
            End If

            If xAppName <> "" Then
                StrSQL.Append(" AND  xPlatform = '" & Agro_SQL_SaveText(xPlatform) & "'  ")
            End If

            If xAppName <> "" Then
                StrSQL.Append(" AND  xEnvironment =  '" & Agro_SQL_SaveText(xEnvironment) & "'  ")
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


    Public Function GetMessagioClientValidation(ByVal xAppName As String,
                                        ByVal xAppVersion As String,
                                        ByVal xPlatform As String,
                                        ByVal xEnvironment As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                      ) As DataTable


        Dim NomeRoutine As String = "ClientValidation_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0


            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  ClientValidation")
            StrSQL.Append(" WHERE  1 = 1")


            If xAppName <> "" Then
                StrSQL.Append(" AND  xAppName =  '" & Agro_SQL_SaveText(xAppName) & "' ")
            End If

            If xAppVersion <> "" Then
                StrSQL.Append(" AND  xAppVersion = '" & Agro_SQL_SaveText(xAppVersion) & "' ")
            End If

            If xAppName <> "" Then
                StrSQL.Append(" AND  xPlatform = '" & Agro_SQL_SaveText(xPlatform) & "'  ")
            End If

            If xAppName <> "" Then
                StrSQL.Append(" AND  xEnvironment =  '" & Agro_SQL_SaveText(xEnvironment) & "'  ")
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
