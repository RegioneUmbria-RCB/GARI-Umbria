Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PossibilitaDistribuzione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##########################################################################################################
    Public Function Leggi(ByVal Mese As Integer,
                          ByVal TipoZona As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Grfi_Cod As Integer,
                          ByVal Id_Gru As Integer,
                          ByVal Regolamento_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.PossibilitaDistribuzione_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT PossibilitaDistribuzione.*, veg_cod, grfi_cod ")
            strSql.AppendLine(" FROM   PossibilitaDistribuzione ")
            strSql.AppendLine(" inner join  GruppoFinalitaxSpeciexEpoca  ")
            strSql.AppendLine(" on GruppoFinalitaxSpeciexEpoca.Regolamento_Cod = PossibilitaDistribuzione.Regolamento_Cod ")
            strSql.AppendLine(" and GruppoFinalitaxSpeciexEpoca.Id_Gru = PossibilitaDistribuzione.Id_Gru ")

            strSql.AppendLine(" WHERE PossibilitaDistribuzione.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   PossibilitaDistribuzione.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Mese <> 0 Then
                strSql.AppendLine(" AND Mese =  " & Agro_SQL_SaveNum(Mese) & "  ")
            End If

            If TipoZona <> "" Then
                strSql.AppendLine(" AND TipoZona =  '" & Agro_SQL_SaveText(TipoZona) & "'  ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND veg_cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Grfi_Cod <> 0 Then
                strSql.AppendLine(" AND Grfi_Cod =  " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            End If

            If Id_Gru <> 0 Then
                strSql.AppendLine(" AND PossibilitaDistribuzione.Id_Gru =  " & Agro_SQL_SaveNum(Id_Gru) & "  ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND PossibilitaDistribuzione.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   PossibilitaDistribuzione.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   PossibilitaDistribuzione.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
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
