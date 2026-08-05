Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_TipoAcqua_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    '######################################################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Integer,
                          ByVal TipoZona As String,
                          ByVal TipoAcqua_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_TipoAcqua_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM PC_TipoAcqua ")
            StrSQL.AppendLine(" WHERE   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.AppendLine(" AND   Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If TipoZona <> "" Then
                StrSQL.AppendLine(" AND TipoZona = '" & Agro_SQL_SaveText(TipoZona) & "'  ")
            End If

            If TipoAcqua_Cod <> -1 Then
                StrSQL.AppendLine(" AND TipoAcqua_Cod =  " & Agro_SQL_SaveNum(TipoAcqua_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   PC_TipoAcqua.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   PC_TipoAcqua.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY TipoAcqua_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt


    End Function

End Class
