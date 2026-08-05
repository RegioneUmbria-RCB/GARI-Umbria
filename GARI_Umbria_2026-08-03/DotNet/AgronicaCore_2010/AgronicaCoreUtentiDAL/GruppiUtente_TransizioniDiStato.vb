Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class GruppiUtente_TransizioniDiStato_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         ByVal Piva_SuperUser As String,
                         ByVal Gruppo_Utente As Integer,
                         ByVal Stato_Origine_Cod As Integer,
                         ByVal Stato_Destinazione_Cod As Integer,
                         ByVal Servizio_Cod As Integer,
                         ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GruppiUtente_TransizioniDiStato_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    strSQL.AppendLine(" SELECT gt.* ")
                    strSQL.AppendLine(" , g.Gruppi_Utente_des ")
                    strSQL.AppendLine(" , s.Servizio_Des ")
                    strSQL.AppendLine(" , o.WAnagraficaStati_Des as Stato_Origine_Des ")
                    strSQL.AppendLine(" , d.WAnagraficaStati_Des as Stato_Destinazione_Des")
                    strSQL.AppendLine(" , wt.WTransizioniDiStatoConfigurazione_Des ")
                    strSQL.AppendLine(" FROM GruppiUtente_TransizioniDiStato gt ")
                    strSQL.AppendLine(" INNER JOIN Gruppi_Utente g ON gt.Gruppo_Utente = g.Gruppi_Utente_cod ")
                    strSQL.AppendLine(" INNER JOIN Servizi_Pratiche s ON gt.Servizio_Cod=s.Servizio_Cod ")
                    strSQL.AppendLine(" INNER JOIN WAnagraficaStati o ON gt.Stato_Origine_Cod=o.WAnagraficaStati_Cod ")
                    strSQL.AppendLine(" INNER JOIN WAnagraficaStati d ON gt.Stato_Destinazione_Cod=d.WAnagraficaStati_Cod ")
                    strSQL.AppendLine(" INNER JOIN WTransizioniDiStatoConfigurazione wt ON gt.Stato_Origine_Cod = wt.Stato_Origine_Cod AND gt.Stato_Destinazione_Cod = wt.Stato_Destinazione_Cod AND wt.Servizio_Cod = gt.Servizio_Cod ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSQL.AppendLine(" SELECT gt.* ")
                    strSQL.AppendLine(" FROM GruppiUtente_TransizioniDiStato gt ")

            End Select

            strSQL.AppendLine(" WHERE 1=1 ")

            If Stato_Origine_Cod <> 0 Then
                strSQL.AppendLine(" AND gt.Stato_Origine_Cod = " & Agro_SQL_SaveNum(Stato_Origine_Cod) & " ")
            End If

            If Stato_Destinazione_Cod <> 0 Then
                strSQL.AppendLine(" AND gt.Stato_Destinazione_Cod = " & Agro_SQL_SaveNum(Stato_Destinazione_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                strSQL.AppendLine(" AND gt.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Gruppo_Utente <> 0 Then
                strSQL.AppendLine(" AND gt.Gruppo_Utente = " & Agro_SQL_SaveNum(Gruppo_Utente) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND   gt.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND   gt.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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

Public Class GruppiUtente_TransizioniDiStato_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva_SuperUser As String,
                           ByVal Gruppo_Utente As Int32,
                           ByVal Stato_Origine_Cod As Int32,
                           ByVal Stato_Destinazione_Cod As Int32,
                           ByVal Servizio_Cod As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.GruppiUtente_TransizioniDiStato_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.AppendLine(" INSERT INTO GruppiUtente_TransizioniDiStato ")
            Stb.AppendLine("            ([Piva_SuperUser] ")
            Stb.AppendLine("            ,[Gruppo_Utente] ")
            Stb.AppendLine("            ,[Stato_Origine_Cod] ")
            Stb.AppendLine("            ,[Stato_Destinazione_Cod] ")
            Stb.AppendLine("            ,[Servizio_Cod] ")
            Stb.AppendLine("            ,[inviato] ")
            Stb.AppendLine("            ,[datainvio] ")
            Stb.AppendLine("            ,[Data_Creazione] ")
            Stb.AppendLine("            ,[Data_Modifica] ")
            Stb.AppendLine("            ,[Username_Creazione] ")
            Stb.AppendLine("            ,[Username_Modifica] ")
            Stb.AppendLine("            ,[Validita_Inizio] ")
            Stb.AppendLine("            ,[Validita_Fine]) ")
            Stb.AppendLine("      VALUES ")
            Stb.AppendLine("            (" & Agro_SQL_SaveText_NULL(Piva_SuperUser) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum(Gruppo_Utente) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum(Stato_Origine_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum(Stato_Destinazione_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            Stb.AppendLine("            ,0 ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Validita_Fine) & ")")

            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Cancella(ByVal Gruppo_Utente As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.GruppiUtente_TransizioniDiStato_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE GruppiUtente_TransizioniDiStato ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

                If Gruppo_Utente <> 0 Then
                    StrSQL.Append(" AND  Gruppo_Utente =  " & Agro_SQL_SaveNum(Gruppo_Utente) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     GruppiUtente_TransizioniDiStato ")
                StrSQL.Append(" WHERE    1=1 ")

                If Gruppo_Utente <> 0 Then
                    StrSQL.Append(" AND  Gruppo_Utente =  " & Agro_SQL_SaveNum(Gruppo_Utente) & " ")
                End If

            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return xRisp

    End Function


End Class
