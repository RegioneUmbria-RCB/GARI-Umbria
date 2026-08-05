Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Public Class GIS_LayerElementiGraficiXGruppiUtente_R
    Inherits AgronicaCoreDataProvider.DataProvider2010
    Public Function Leggi(ByVal LayerCod As Integer,
                          ByVal GruppoUtenti As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal LeggiDescrizioneGruppo As Boolean = True
                          ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     [LayerElementiGrafici_Cod]")
            StrSQL.AppendLine("     , perm.[Gruppi_Utente_cod]")
            StrSQL.AppendLine("     , COALESCE([Flag_Inserimento], 0) AS Flag_Inserimento")
            StrSQL.AppendLine("     , COALESCE([Flag_Modifica], 0) AS Flag_Modifica")
            StrSQL.AppendLine("     , COALESCE([Flag_Cancellazione], 0) AS Flag_Cancellazione")
            StrSQL.AppendLine("     , COALESCE([Flag_Informazioni], 0) AS Flag_Informazioni")
            StrSQL.AppendLine("     , COALESCE([Flag_Amministrazione], 0) AS Flag_Amministrazione")
            StrSQL.AppendLine("     , COALESCE([Flag_Rimozione], 0) AS Flag_Rimozione")
            If LeggiDescrizioneGruppo Then
                StrSQL.AppendLine("     ,grp.Gruppi_Utente_Des ")
            End If
            StrSQL.AppendLine(" FROM  GIS_LayerElementiGraficiXGruppiUtente perm ")
            If LeggiDescrizioneGruppo Then
                StrSQL.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB() & ".dbo.Gruppi_Utente grp on")
                StrSQL.AppendLine(" ( perm.Gruppi_Utente_cod = grp.Gruppi_Utente_cod) ")
            End If
            StrSQL.AppendLine(" where 1=1 ")

            If LayerCod <> 0 Then
                StrSQL.AppendLine(" and LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(LayerCod) & " ")
            End If

            If GruppoUtenti <> 0 Then
                StrSQL.AppendLine(" and perm.Gruppi_Utente_cod=" & Agro_SQL_SaveNum(GruppoUtenti) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   perm.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   perm.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY LayerElementiGrafici_Cod, perm.Gruppi_Utente_cod ")
            End If

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

End Class

Public Class GIS_LayerElementiGraficiXGruppiUtente_W
    Inherits AgronicaCoreDataProvider.DataProvider2010

    Public Function Scrivi(ByVal LayerCod As Integer,
                           ByVal GruppoUtenti As Integer,
                           ByVal Flag_Inserimento As Integer,
                           ByVal Flag_Modifica As Integer,
                           ByVal Flag_Cancellazione As Integer,
                           ByVal Flag_Informazioni As Integer,
                           ByVal Flag_Amministrazione As Integer,
                           ByVal Flag_Rimozione As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_LayerElementiGraficiXGruppiUtente ")
            StrSQL.AppendLine("     ([LayerElementiGrafici_Cod]  ")
            StrSQL.AppendLine("     ,[Gruppi_Utente_cod] ")
            StrSQL.AppendLine("     ,[Flag_Inserimento] ")
            StrSQL.AppendLine("     ,[Flag_Modifica] ")
            StrSQL.AppendLine("     ,[Flag_Cancellazione] ")
            StrSQL.AppendLine("     ,[Flag_Informazioni] ")
            StrSQL.AppendLine("     ,[Flag_Amministrazione] ")
            StrSQL.AppendLine("     ,[Flag_Rimozione] ")
            StrSQL.AppendLine("     ,[inviato] ")
            StrSQL.AppendLine("     ,[datainvio] ")
            StrSQL.AppendLine("     ,[Data_Creazione] ")
            StrSQL.AppendLine("     ,[Data_Modifica] ")
            StrSQL.AppendLine("     ,[Username_Creazione] ")
            StrSQL.AppendLine("     ,[Username_Modifica] ")
            StrSQL.AppendLine("     ,[Validita_Inizio] ")
            StrSQL.AppendLine("     ,[Validita_Fine]) ")
            StrSQL.AppendLine(" VALUES (")

            StrSQL.AppendLine("         '" & Agro_SQL_SaveText(LayerCod) & "'  ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(GruppoUtenti) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Inserimento) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Modifica) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Cancellazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Informazioni) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Amministrazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Rimozione) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal LayerCod As Integer,
                             ByVal GruppoUtenti As Integer,
                             ByVal Flag_Inserimento As Integer,
                             ByVal Flag_Modifica As Integer,
                             ByVal Flag_Cancellazione As Integer,
                             ByVal Flag_Informazioni As Integer,
                             ByVal Flag_Amministrazione As Integer,
                             ByVal Flag_Rimozione As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If LayerCod = 0 And GruppoUtenti = 0 And xFiltroAggiuntivo = "" Then
                Throw New Exception("Modifica permessi utente su layer gis senza filtri (Codice Layer e/o GruppiUtente). Operazione non permessa.")
            End If


            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerElementiGraficiXGruppiUtente set")
            StrSQL.AppendLine($"     [Flag_Inserimento] = {Agro_SQL_SaveNum(Flag_Inserimento)}")
            StrSQL.AppendLine($"     ,[Flag_Modifica] = {Agro_SQL_SaveNum(Flag_Modifica)}")
            StrSQL.AppendLine($"     ,[Flag_Cancellazione] = {Agro_SQL_SaveNum(Flag_Cancellazione)}")
            StrSQL.AppendLine($"     ,[Flag_Informazioni] = {Agro_SQL_SaveNum(Flag_Informazioni)}")
            StrSQL.AppendLine($"     ,[Flag_Amministrazione] = {Agro_SQL_SaveNum(Flag_Amministrazione)}")
            StrSQL.AppendLine($"     ,[Flag_Rimozione] = {Agro_SQL_SaveNum(Flag_Rimozione)}")
            StrSQL.AppendLine("     ,[inviato] = 0")
            StrSQL.AppendLine("     ,[datainvio] = NULL")
            StrSQL.AppendLine($"     ,[Data_Modifica] {Agro_SQL_SaveDate(Date.Now)}")
            StrSQL.AppendLine($"     ,[Username_Modifica] = '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}'")
            StrSQL.AppendLine($"     ,[Validita_Inizio] =  {Agro_SQL_SaveDate(Validita_Inizio)}")
            StrSQL.AppendLine($"     ,[Validita_Fine]) =  {Agro_SQL_SaveDate(Validita_Fine)}")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If LayerCod <> 0 Then
                StrSQL.AppendLine(" and LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(LayerCod) & " ")
            End If

            If GruppoUtenti <> 0 Then
                StrSQL.AppendLine(" and Gruppi_Utente_cod=" & Agro_SQL_SaveNum(GruppoUtenti) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal LayerCod As Integer,
                             ByVal GruppoUtenti As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If LayerCod = 0 And GruppoUtenti = 0 And xFiltroAggiuntivo = "" Then
                Throw New Exception("cancellazione permessi utente su layer gis senza filtri (Codice Layer e/o GruppiUtente). Operazione non permessa.")
            End If
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE FROM GIS_LayerElementiGraficiXGruppiUtente ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If LayerCod <> 0 Then
                StrSQL.AppendLine(" and LayerElementiGrafici_Cod=" & Agro_SQL_SaveNum(LayerCod) & " ")
            End If

            If GruppoUtenti <> 0 Then
                StrSQL.AppendLine(" and Gruppi_Utente_cod=" & Agro_SQL_SaveNum(GruppoUtenti) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
