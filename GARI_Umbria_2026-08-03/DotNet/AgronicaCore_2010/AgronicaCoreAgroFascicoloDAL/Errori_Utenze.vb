Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Errori_Utenze_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Errori_Utenze(EnteValidatore_Cod As Integer,
                                        UsernameWS As String,
                                        Data As Date,
                                            Parametri_Extra As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "Errori_Utenze_R.Leggi_Errori_Utenze()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("SELECT * ")
            Stb.AppendLine("   FROM [dbo].[Errori_Utenze]")

            Stb.AppendLine("   WHERE 1=1 ")

            If EnteValidatore_Cod <> 0 Then
                Stb.AppendLine(" AND EnteValidatore_Cod = " & Agro_SQL_SaveNum(EnteValidatore_Cod) & " ")
            End If

            If Data <> AGRODATAINIZIO Then
                Stb.AppendLine(" AND Data = " & Agro_SQL_SaveDate(Data) & " ")
            End If

            If UsernameWS <> "" Then
                Stb.AppendLine(" AND Utenza = " & Agro_SQL_SaveText_NULL(UsernameWS) & " ")
            End If

            If Parametri_Extra <> "" Then
                Stb.AppendLine(" AND Parametri_Extra = " & Agro_SQL_SaveText_NULL(Parametri_Extra) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Return CInt(DT.Rows(0)("NumeroErrori"))
            Else
                Return 0
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

    End Function

End Class

Public Class Errori_Utenze_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function incrementa_errori(EnteValidatore_Cod As Integer,
                                      UsernameWS As String,
                                      Data As Date,
                                      Parametri_Extra As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "Errori_Utenze_R.Leggi_Errori_Utenze()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim objLeggi As New Errori_Utenze_R

            Dim errori = objLeggi.Leggi_Errori_Utenze(EnteValidatore_Cod, UsernameWS, Data, Parametri_Extra, "", "", objParametri)

            If errori > 0 Then

                Stb.AppendLine("UPDATE [dbo].[Errori_Utenze] ")
                Stb.AppendLine("    SET [NumeroErrori] = " & Agro_SQL_SaveNum(errori + 1) & " ")
                Stb.AppendLine("   WHERE 1=1 ")

                If EnteValidatore_Cod <> 0 Then
                    Stb.AppendLine(" AND EnteValidatore_Cod = " & Agro_SQL_SaveNum(EnteValidatore_Cod) & " ")
                End If

                If Data <> AGRODATAINIZIO Then
                    Stb.AppendLine(" AND Data = " & Agro_SQL_SaveDate(Data) & " ")
                End If

                If UsernameWS <> "" Then
                    Stb.AppendLine(" AND Utenza = " & Agro_SQL_SaveText_NULL(UsernameWS) & " ")
                End If

                If Parametri_Extra <> "" Then
                    Stb.AppendLine(" AND Parametri_Extra = " & Agro_SQL_SaveText_NULL(Parametri_Extra) & " ")
                End If


            Else

                Stb.AppendLine("INSERT INTO [dbo].[Errori_Utenze] ")
                Stb.AppendLine("            ([Utenza] ")
                Stb.AppendLine("            ,[Data] ")
                Stb.AppendLine("            ,[NumeroErrori] ")
                Stb.AppendLine("            ,[EnteValidatore_Cod] ")
                Stb.AppendLine("            ,[Parametri_Extra]) ")
                Stb.AppendLine("      VALUES ")
                Stb.AppendLine("            (" & Agro_SQL_SaveText_NULL(UsernameWS) & " ")
                Stb.AppendLine("            ," & Agro_SQL_SaveDate(Data) & " ")
                Stb.AppendLine("            ,1 ")
                Stb.AppendLine("            ," & Agro_SQL_SaveNum(EnteValidatore_Cod) & " ")
                Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Parametri_Extra) & ")")

            End If


            Dim res = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

    End Function



End Class
