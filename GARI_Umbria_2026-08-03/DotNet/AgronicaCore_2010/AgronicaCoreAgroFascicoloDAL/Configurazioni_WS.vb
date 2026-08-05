Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Configurazioni_WS_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Configurazioni_WS(EnteValidatore_Cod As Integer,
                                            LinkWS As String,
                                            UsernameWS As String,
                                            PasswordWS As String,
                                            Extra As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Animali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("SELECT [EnteValidatore_Cod] ")
            Stb.AppendLine("       ,[LinkWS] ")
            Stb.AppendLine("       ,[UsernameWS] ")
            Stb.AppendLine("       ,[PasswordWS] ")
            Stb.AppendLine("       ,[PasswordWSCryptata] ")
            Stb.AppendLine("       ,[Extra] ")
            Stb.AppendLine("   FROM [dbo].[Configurazioni_WS]")

            Stb.AppendLine("   WHERE 1=1 ")

            If EnteValidatore_Cod <> 0 Then
                Stb.AppendLine(" AND EnteValidatore_Cod = " & Agro_SQL_SaveNum(EnteValidatore_Cod) & " ")
            End If

            If LinkWS <> "" Then
                Stb.AppendLine(" AND LinkWS = " & Agro_SQL_SaveText_NULL(LinkWS) & " ")
            End If

            If UsernameWS <> "" Then
                Stb.AppendLine(" AND UsernameWS = " & Agro_SQL_SaveText_NULL(UsernameWS) & " ")
            End If

            If PasswordWS <> "" Then
                Stb.AppendLine(" AND PasswordWS = " & Agro_SQL_SaveText_NULL(PasswordWS) & " ")
            End If

            If Extra <> "" Then
                Stb.AppendLine(" AND Extra = " & Agro_SQL_SaveText_NULL(Extra) & " ")
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

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class


Public Class Configurazioni_WS_W
    Inherits AgronicaCoreDataProvider.DataProvider



End Class

