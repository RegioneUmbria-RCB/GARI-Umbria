Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class Pratiche_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiXServiziStatoAttuale(
        ByVal WorkFlow_Cod As Integer,
        ByVal piva As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal Filtro_Workflow_Obbligatorio As Boolean
        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("  Select ")
            stb.AppendLine("    p.Pratica_Cod ")
            stb.AppendLine("  , p.Validita_Inizio ")
            stb.AppendLine("  , p.Validita_Fine ")
            stb.AppendLine("  , s.Servizio_Cod ")
            stb.AppendLine("  , s.Servizio_Des ")
            stb.AppendLine("  , p.piva  ")

            If piva <> "" Then
                stb.AppendLine("  , i.rag_soc ")
            Else
                stb.AppendLine("  , '' as rag_soc ")
            End If

            stb.AppendLine("  , w.WorkFlow_Cod ")
            stb.AppendLine("  , w.WorkFlow_Des ")
            stb.AppendLine("  , origine.WAnagraficaStati_Cod as WAnagraficaStati_Cod ")
            stb.AppendLine("  , origine.WAnagraficaStati_Des as WAnagraficaStati_Des ")
            stb.AppendLine("  , origine.Validita_Inizio as Data_Stato ")
            stb.AppendLine("  , cfg.Username_Creazione as utente ")
            stb.AppendLine(" From Pratiche p ")

            If piva <> "" Then
                stb.AppendLine("  inner Join Imprese i  ")
                stb.AppendLine("         On p.Piva = i.PIVA ")
            End If

            stb.AppendLine("      inner Join Pratiche_Stati_Attuali cfg ")
            stb.AppendLine("         On p.Pratica_Cod = cfg.Pratica_Cod ")
            stb.AppendLine("      inner Join WAnagraficaStati origine ")
            stb.AppendLine("         On origine.WAnagraficaStati_Cod = cfg.Stato_Cod ")
            stb.AppendLine("   inner Join Servizi s ")
            stb.AppendLine("         On s.Servizio_Cod = p.Servizio_Cod ")
            stb.AppendLine(" inner Join WWorkFlow w ")
            stb.AppendLine("         On w.WorkFlow_Cod = origine.WWorkFlow_cod ")
            stb.AppendLine("       ")

            Dim sWhere As String = " where "
            If Filtro_Workflow_Obbligatorio Then
                stb.AppendLine(sWhere & " W.WorkFlow_Cod = " & WorkFlow_Cod)
                sWhere = " AND "
            Else

                If WorkFlow_Cod > 0 Then
                    stb.AppendLine(sWhere & " W.WorkFlow_Cod = " & WorkFlow_Cod)
                    sWhere = " AND "
                End If

            End If


            If piva <> "" Then
                stb.AppendLine(sWhere & " p.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                sWhere = " AND "
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(sWhere & xFiltroAggiuntivo)
                sWhere = " AND "
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(sWhere & " p.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(sWhere & " p.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiXAccounting(
        ByVal piva As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("select  ")
            stb.AppendLine(" 	  at.Username_Creazione as username ")
            stb.AppendLine(" 	, p.Cuaa as impresa ")
            stb.AppendLine(" 	, p.Servizio_Cod as servizio_id ")
            stb.AppendLine(" 	, at.Stato_Cod as stato_id ")
            stb.AppendLine(" 	, at.Validita_Fine as dataScadenza ")
            stb.AppendLine(" from Pratiche p ")
            stb.AppendLine(" 	inner join Pratiche_Stati_Attuali at  ")
            stb.AppendLine(" 	on p.Pratica_Cod = at.Pratica_Cod")

            stb.AppendLine(" inner Join WAnagraficaStati s ")
            stb.AppendLine("     On s.WAnagraficaStati_Cod = at.Stato_Cod ")
            stb.AppendLine("     And s.WWorkFlow_cod = 1001 ")


            stb.AppendLine(" where p.piva = '" & Agro_SQL_SaveText(piva) & "' ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   p.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   p.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            stb.AppendLine(" order by at.username_creazione, p.cuaa, p.servizio_Cod ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_2(ByVal Pratica_Cod As Int32,
                          ByVal Piva As String,
                          ByVal Cuaa As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Veg_Cod As Int32,
                          ByVal Id_Cod As Int32,
                          ByVal Servizio_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByVal Pratica_New As Boolean
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Pratiche.* ")
            StrSQL.Append(" FROM  Pratiche ")

            StrSQL.Append(" WHERE Pratiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Pratiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Pratiche.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                StrSQL.Append(" AND Pratiche.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Pratica_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            Else
                If Pratica_New Then
                    StrSQL.Append(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Pratiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Pratiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi(ByVal Pratica_Cod As Int32,
                          ByVal Rag_Soc As String,
                          ByVal Piva As String,
                          ByVal Cuaa As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Veg_Cod As Int32,
                          ByVal Id_Cod As Int32,
                          ByVal Servizio_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          ByVal Programmazione_Cod As Integer,
                          ByVal Programmazione_Entita_Cod As Integer
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Pratiche.*, Servizi.Servizio_Des, Imprese.rag_soc ")
            StrSQL.Append(" FROM  Pratiche INNER JOIN ")
            StrSQL.Append(" Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod INNER JOIN ")
            StrSQL.Append(" Imprese ON Pratiche.Piva = Imprese.PIVA ")

            StrSQL.Append(" WHERE Pratiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Pratiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Pratica_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If

            If Rag_Soc <> "" Then
                StrSQL.Append(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                StrSQL.Append(" AND Pratiche.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                StrSQL.Append(" AND Pratiche.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Pratiche.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Pratiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Pratiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_Servizio_DaPratica(ByVal Pratica_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_Servizio_DaPratica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Servizio_Cod As Integer = 0

        Try

            Leggi_DT_Servizio_DaPratica_SQL(Pratica_Cod, xFiltroAggiuntivo, xOrderBy, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Servizio_Cod = DT.Rows(0).Item("Servizio_Cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Servizio_Cod

    End Function

    Public Function Leggi_DT_Servizio_DaPratica(ByVal Pratica_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_Servizio_DaPratica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Try

            Leggi_DT_Servizio_DaPratica_SQL(Pratica_Cod, xFiltroAggiuntivo, xOrderBy, objParametri, StrSQL)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub Leggi_DT_Servizio_DaPratica_SQL(Pratica_Cod As Integer,
                                                xFiltroAggiuntivo As String,
                                                xOrderBy As String,
                                                objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef StrSQL As Text.StringBuilder)
        StrSQL.Length = 0
        StrSQL.Append(" SELECT Pratiche.*, Servizi.Servizio_Des ")
        StrSQL.Append(" FROM  Pratiche INNER JOIN ")
        StrSQL.Append(" Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod ")
        StrSQL.Append(" WHERE  Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If
        '--------------------------------------------------------------------------
        Select Case objParametri.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                StrSQL.Append(" AND   Pratiche.Inviato >=0 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                StrSQL.Append(" AND   Pratiche.Inviato =-1 ")
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select
        '--------------------------------------------------------------------------
        If xOrderBy <> "" Then
            StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        End If

    End Sub

    Public Function Leggi_conStoricoTransizioniDiStato(ByVal Pratica_Cod As Int32,
                      ByVal Rag_Soc As String,
                      ByVal Piva As String,
                      ByVal Cuaa As String,
                      ByVal Sa_Cod As Int32,
                      ByVal Veg_Cod As Int32,
                      ByVal Id_Cod As Int32,
                      ByVal Servizio_Cod As Int32,
                      ByVal Stato_Cod As Int32,
                      ByVal Validita_Inizio As Date,
                      ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_conStoricoTransizioniDiStato()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.Append("SELECT      " & vbCrLf)
            stb.Append("    Imprese.rag_soc " & vbCrLf)
            stb.Append("    , Servizi.Servizio_Des " & vbCrLf)
            stb.Append("    , ISNULL(Pratiche_Stati.Stato_Cod, 0) AS Stato_Cod " & vbCrLf)
            stb.Append("    , ISNULL(st.WAnagraficaStati_Des, '') AS Stato_Des " & vbCrLf)
            stb.Append("    , st.Ordine " & vbCrLf)
            stb.Append("    , Pratiche_Stati.Validita_Inizio AS Validita_Inizio_Stato " & vbCrLf)
            stb.Append("    , Pratiche_Stati.Validita_Fine AS Validita_Fine_Stato " & vbCrLf)
            stb.Append("    , Pratiche_Stati.stato_Origine_cod  " & vbCrLf)
            stb.Append("    , Pratiche_Stati.PassaggioDiStato_cod  " & vbCrLf)
            stb.Append("    , Pratiche_Stati.note " & vbCrLf)

            stb.Append("    , st.colore " & vbCrLf)
            stb.Append("    , st.WWorkFlow_cod " & vbCrLf)
            stb.Append("    , Pratiche.* " & vbCrLf)

            stb.Append("    , replace( " & vbCrLf)
            stb.Append("        replace( " & vbCrLf)
            stb.Append("            statoAttuale.stati, '&gt;', '>') " & vbCrLf)
            stb.Append("        , '&lt;', '<')             " & vbCrLf)
            stb.Append("     as StatoAttuale_DES, " & vbCrLf)
            stb.Append("     statoAttuale.StatoFinale_Cod, " & vbCrLf)
            stb.Append("     statoAttuale.StatoFinale_Des " & vbCrLf)


            stb.Append(" " & vbCrLf)
            stb.Append(" FROM Pratiche " & vbCrLf)

            stb.Append(" inner join ( " & vbCrLf)
            stb.Append(" select p.pratica_cod, " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  (  " & vbCrLf)
            stb.Append("        select TOP 1 '<font color=""' + s.colore + '"">' + s.WAnagraficaStati_Des + '</font><br /> (' + replace(convert(varchar(100), at.Validita_Inizio, 105), '-', '/') + ') <br />' as [text()] " & vbCrLf)
            stb.Append("        from Pratiche_Stati_Attuali at " & vbCrLf)
            stb.Append("            inner join WAnagraficaStati s  " & vbCrLf)
            stb.Append("                on at.Stato_Cod  = s.WAnagraficaStati_Cod  " & vbCrLf)
            stb.Append("        where at.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
            stb.Append("        ORDER BY at.Validita_Inizio DESC  " & vbCrLf)
            stb.Append("        for xml path('') " & vbCrLf)
            stb.Append("        ) as stati, " & vbCrLf)
            stb.Append("  (  " & vbCrLf)
            stb.Append("        select TOP 1 s.WAnagraficaStati_Des " & vbCrLf)
            stb.Append("        from Pratiche_Stati_Attuali at " & vbCrLf)
            stb.Append("            inner join WAnagraficaStati s  " & vbCrLf)
            stb.Append("                on at.Stato_Cod  = s.WAnagraficaStati_Cod  " & vbCrLf)
            stb.Append("        where at.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
            stb.Append("        ORDER BY at.Validita_Inizio DESC  " & vbCrLf)
            stb.Append("        ) as StatoFinale_Des, " & vbCrLf)
            stb.Append("  (  " & vbCrLf)
            stb.Append("        select TOP 1 s.WAnagraficaStati_Cod " & vbCrLf)
            stb.Append("        from Pratiche_Stati_Attuali at " & vbCrLf)
            stb.Append("            inner join WAnagraficaStati s  " & vbCrLf)
            stb.Append("                on at.Stato_Cod  = s.WAnagraficaStati_Cod  " & vbCrLf)
            stb.Append("        where at.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
            stb.Append("        ORDER BY at.Validita_Inizio DESC  " & vbCrLf)
            stb.Append("        ) as StatoFinale_Cod " & vbCrLf)
            stb.Append(" from pratiche p  " & vbCrLf)
            stb.Append(" group by p.Pratica_Cod  " & vbCrLf)
            stb.Append(" ) statoAttuale on statoAttuale.Pratica_Cod = Pratiche.Pratica_Cod  " & vbCrLf)


            stb.Append("      INNER JOIN Pratiche_Stati   " & vbCrLf)
            stb.Append("   on Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser AND  Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod " & vbCrLf)
            stb.Append("   INNER JOIN Imprese ON Pratiche.Piva = Imprese.PIVA  " & vbCrLf)
            stb.Append("   INNER JOIN Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("   inner join WAnagraficaStati st on st.WAnagraficaStati_Cod = Pratiche_Stati.Stato_Cod  " & vbCrLf)

            stb.Append(" WHERE Pratiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.Append(" AND   Pratiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Pratica_Cod <> 0 Then
                stb.Append(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If
            If Rag_Soc <> "" Then
                stb.Append(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                stb.Append(" AND Pratiche.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                stb.Append(" AND Pratiche.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND Pratiche.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                stb.Append(" AND Pratiche.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                stb.Append(" AND Pratiche.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                stb.Append(" AND Pratiche.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                stb.Append(" AND Pratiche_Stati.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Pratiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Pratiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function Leggi_conStoricoTransizioniDiStato_FiltroUtente(ByVal Pratica_Cod As Int32,
                                                                    ByVal Rag_Soc As String,
                                                                    ByVal Piva As String,
                                                                    ByVal Cuaa As String,
                                                                    ByVal Sa_Cod As Int32,
                                                                    ByVal Veg_Cod As Int32,
                                                                    ByVal Id_Cod As Int32,
                                                                    ByVal Servizio_Cod As Int32,
                                                                    ByVal Stato_Cod As Int32,
                                                                    ByVal Validita_Inizio As Date,
                                                                    ByVal Validita_Fine As Date,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByVal xOrderBy As String,
                                                                    ByVal Filtro_Visibilita_Utente As Boolean,
                                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_conStoricoTransizioniDiStato()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.AppendLine("SELECT  DISTINCT    ")
            stb.AppendLine("    Imprese.rag_soc ")
            stb.AppendLine("    , Servizi.Servizio_Des ")
            stb.AppendLine("    , ISNULL(Pratiche_Stati.Stato_Cod, 0) AS Stato_Cod ")
            stb.AppendLine("    , ISNULL(st.WAnagraficaStati_Des, '') AS Stato_Des ")
            stb.AppendLine("    , st.Ordine ")
            stb.AppendLine("    , Pratiche_Stati.Validita_Inizio AS Validita_Inizio_Stato ")
            stb.AppendLine("    , Pratiche_Stati.Validita_Fine AS Validita_Fine_Stato ")
            stb.AppendLine("    , Pratiche_Stati.stato_Origine_cod  ")
            stb.AppendLine("    , Pratiche_Stati.PassaggioDiStato_cod  ")
            stb.AppendLine("    , Pratiche_Stati.note ")

            stb.AppendLine("    , st.colore ")
            stb.AppendLine("    , st.WWorkFlow_cod ")
            stb.AppendLine("    , Pratiche.* ")

            stb.AppendLine("    , replace( ")
            stb.AppendLine("        replace( ")
            stb.AppendLine("            statoAttuale.stati, '&gt;', '>') ")
            stb.AppendLine("        , '&lt;', '<')             ")
            stb.AppendLine("     as StatoAttuale_DES, ")
            stb.AppendLine("     statoAttuale.StatoFinale_Cod, ")
            stb.AppendLine("     statoAttuale.StatoFinale_Des, ")
            stb.AppendLine("     ud.Nome + ' ' + ud.Cognome + ' ' + ud.Rag_Soc as Utente ")


            stb.AppendLine(" ")
            stb.AppendLine(" FROM Pratiche ")

            stb.AppendLine(" inner join ( ")
            stb.AppendLine(" select p.pratica_cod, ")
            stb.AppendLine("  ")
            stb.AppendLine("  (  ")
            stb.AppendLine("        select top 1 '<font color=""' + s.colore + '"">' + s.WAnagraficaStati_Des + '</font><br /> (' + replace(convert(varchar(100), at.Validita_Inizio, 105), '-', '/') + ') <br />' as [text()] ")
            stb.AppendLine("        from Pratiche_Stati_Attuali at ")
            stb.AppendLine("            inner join WAnagraficaStati s  ")
            stb.AppendLine("                on at.Stato_Cod  = s.WAnagraficaStati_Cod  ")
            stb.AppendLine("        where at.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        for xml path('') ")
            stb.AppendLine("        ) as stati, ")
            stb.AppendLine("  (  ")
            stb.AppendLine("        select  top 1 s.WAnagraficaStati_Des ")
            stb.AppendLine("        from Pratiche_Stati_Attuali at ")
            stb.AppendLine("            inner join WAnagraficaStati s  ")
            stb.AppendLine("                on at.Stato_Cod  = s.WAnagraficaStati_Cod  ")
            stb.AppendLine("        where at.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        ) as StatoFinale_Des, ")
            stb.AppendLine("  (  ")
            stb.AppendLine("        select  top 1 s.WAnagraficaStati_Cod ")
            stb.AppendLine("        from Pratiche_Stati_Attuali at ")
            stb.AppendLine("            inner join WAnagraficaStati s  ")
            stb.AppendLine("                on at.Stato_Cod  = s.WAnagraficaStati_Cod  ")
            stb.AppendLine("        where at.Pratica_Cod = p.Pratica_Cod  ")
            stb.AppendLine("        ) as StatoFinale_Cod ")
            stb.AppendLine(" from pratiche p  ")
            stb.AppendLine(" group by p.Pratica_Cod  ")
            stb.AppendLine(" ) statoAttuale on statoAttuale.Pratica_Cod = Pratiche.Pratica_Cod  ")


            stb.AppendLine("      INNER JOIN Pratiche_Stati   ")
            stb.AppendLine("   on Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser AND  Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine("   INNER JOIN Imprese ON Pratiche.Piva = Imprese.PIVA  ")
            stb.AppendLine("   INNER JOIN Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod  ")
            stb.AppendLine("   INNER JOIN WAnagraficaStati st on st.WAnagraficaStati_Cod = Pratiche_Stati.Stato_Cod  ")
            stb.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            stb.AppendLine("   LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli ud On Pratiche_Stati.Username_Creazione = ud.CodFisc  ")

            stb.AppendLine(" WHERE 1=1  ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            stb.AppendLine(" AND   Pratiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND   Pratiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Pratica_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If
            If Rag_Soc <> "" Then
                stb.AppendLine(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                stb.AppendLine(" AND Pratiche.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                stb.AppendLine(" AND Pratiche.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche_Stati.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Pratiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Pratiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_FiltroUtente(ByVal Pratica_Cod As Int32,
                                                                    ByVal Rag_Soc As String,
                                                                    ByVal Piva As String,
                                                                    ByVal Cuaa As String,
                                                                    ByVal Sa_Cod As Int32,
                                                                    ByVal Veg_Cod As Int32,
                                                                    ByVal Id_Cod As Int32,
                                                                    ByVal Servizio_Cod As Int32,
                                                                    ByVal Stato_Cod As Int32,
                                                                    ByVal Validita_Inizio As Date,
                                                                    ByVal Validita_Fine As Date,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByVal xOrderBy As String,
                                                                    ByVal Filtro_Visibilita_Utente As Boolean,
                                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                    ByVal visualizza_Kpin_BlockName As Boolean
                                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_conStoricoTransizioniDiStato()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            stb.AppendLine("SELECT    ")
            stb.AppendLine("    Pratiche.Pratica_Cod,  ")
            stb.AppendLine("    Pratiche.Piva, ")
            stb.AppendLine("    CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Pratiche.Piva ELSE Imprese.partitaIvaReale END PivaReale, ")
            stb.AppendLine("    Pratiche.Cuaa, ")
            stb.AppendLine("    Pratiche.Data_Creazione, ")
            stb.AppendLine("    Pratiche.Data_Modifica, ")
            stb.AppendLine("    Pratiche.Username_Creazione, ")
            stb.AppendLine("    Pratiche.Username_Modifica, ")
            stb.AppendLine("    Imprese.rag_soc as Rag_Soc, ")
            stb.AppendLine("    Servizi.Servizio_Cod, ")
            stb.AppendLine("    Servizi.Servizio_Des, ")
            stb.AppendLine("    Pratiche.Validita_Inizio as Data_Inizio, ")
            stb.AppendLine("    Pratiche.Validita_Fine as Data_Fine, ")
            stb.AppendLine("    Pratiche_Stati_Attuali.Stato_Cod as StatoAttuale_Cod, ")
            stb.AppendLine("    '' as Colore, ")
            stb.AppendLine("    0 as PassaggioDiStato_cod, ")
            stb.AppendLine("    WAnagraficaStati.WAnagraficaStati_Des as Stato, ")
            stb.AppendLine("    Pratiche.Numero, ")
            stb.AppendLine("    WAnagraficaStati.WWorkFlow_cod as WWorkflow_Cod, ")

            If objParametri_Utenti IsNot Nothing Then
                stb.AppendLine("    ud.Nome + ' ' + ud.Cognome + ' ' + ud.Rag_Soc as Utente, ")
            Else
                stb.AppendLine("    '' as Utente, ")
            End If

            stb.AppendLine("    Pratiche.Blocco_Flag, ")
            stb.AppendLine("    Programmazione_Testata.Programmazione_Des_Long, Programmazione_Entita.Entita_Des, ")

            If visualizza_Kpin_BlockName Then

                stb.AppendLine("    KPIN.val_cod as KPIN, ")
                stb.AppendLine("    BlockName.val_Cod as Block_Name, ")
                stb.AppendLine("    CASE WHEN (Programmazione_Entita.Programmazione_Entita_Cod IS NULL) THEN '' ELSE ISNULL(Programmazione_Entita.Entita_Des, '') + ' - ' + + ISNULL(SpecieVegetali.Veg_Des, '') + ' ' + ISNULL(cultivar.cul_des, '') + ' ' + 'KPIN:' + ISNULL(KPIN.val_cod, '') + ' - '  + 'Block Name:' + ISNULL(BlockName.val_cod, '') END as Descrizione_Progetto, ")

            End If

            stb.AppendLine("    cultivar.cul_des, ")
            stb.AppendLine("    SpecieVegetali.Veg_Des, ")
            stb.AppendLine("    Programmazione_Entita.Validita_Inizio ")

            stb.AppendLine(" FROM Pratiche ")
            stb.AppendLine(" INNER JOIN Pratiche_Stati_Attuali ON Pratiche.Pratica_Cod = Pratiche_Stati_Attuali.Pratica_Cod ")
            stb.AppendLine(" INNER JOIN Imprese ON Pratiche.Piva = Imprese.PIVA ")
            stb.AppendLine(" INNER JOIN Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod ")
            stb.AppendLine(" INNER JOIN WAnagraficaStati ON Pratiche_Stati_Attuali.Stato_Cod = WAnagraficaStati.WAnagraficaStati_Cod ")

            stb.AppendLine(" LEFT JOIN Programmazione_Testata ON Pratiche.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            stb.AppendLine(" LEFT JOIN Programmazione_Entita ON Pratiche.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ")

            If visualizza_Kpin_BlockName Then

                stb.AppendLine(" LEFT JOIN Reg_Impianti_Programmazioni ON Programmazione_Entita.Programmazione_Entita_cod = Reg_Impianti_Programmazioni.Programmazione_Entita_Cod ")

                stb.AppendLine(" LEFT JOIN Reg_Impianti ON Reg_Impianti_Programmazioni.Piva = Reg_Impianti.Piva AND Reg_Impianti_Programmazioni.Sa_Cod = Reg_Impianti.Sa_Cod AND Reg_Impianti_Programmazioni.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Programmazioni.Id_Reg = Reg_Impianti.ID_REG  ")
                stb.AppendLine(" LEFT JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.ID_REG  AND Imprese_Progetti.Validita_inizio <=  Pratiche.Validita_inizio AND Imprese_Progetti.Validita_Fine >=  Pratiche.Validita_Fine ")

                stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici KPIN ON  Imprese_Progetti.Piva = KPIN.Piva AND Imprese_Progetti.Sa_Cod = KPIN.Sa_Cod AND Imprese_Progetti.Appezza = KPIN.Appezza AND Imprese_Progetti.Id_Reg = KPIN.ID_REG  AND Imprese_Progetti.Progetto_Cod = KPIN.Progetto_Cod AND KPIN.id_cod = 1287 ")
                stb.AppendLine(" LEFT JOIN Reg_Impianti_Codici BlockName ON  Imprese_Progetti.Piva = BlockName.Piva AND Imprese_Progetti.Sa_Cod = BlockName.Sa_Cod AND Imprese_Progetti.Appezza = BlockName.Appezza AND Imprese_Progetti.Id_Reg = BlockName.ID_REG  AND Imprese_Progetti.Progetto_Cod = BlockName.Progetto_Cod AND BlockName.id_cod = 1288 ")

            End If

            stb.AppendLine(" LEFT JOIN Cultivar ON Programmazione_Entita.Cul_Cod = Cultivar.Cul_Cod ")
            stb.AppendLine(" LEFT JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.veg_cod ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If
            If objParametri_Utenti IsNot Nothing Then
                stb.AppendLine("   LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli ud On Pratiche_Stati_Attuali.Username_Creazione = ud.CodFisc  ")
            End If

            stb.AppendLine(" WHERE 1=1  ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            stb.AppendLine(" AND   Pratiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND   Pratiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Pratica_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If
            If Rag_Soc <> "" Then
                stb.AppendLine(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                stb.AppendLine(" AND Pratiche.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                stb.AppendLine(" AND Pratiche.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                stb.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Pratiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Pratiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_conStatoAttuale(ByVal Pratica_Cod As Int32,
                      ByVal Rag_Soc As String,
                      ByVal Piva As String,
                      ByVal Cuaa As String,
                      ByVal Sa_Cod As Int32,
                      ByVal Veg_Cod As Int32,
                      ByVal Id_Cod As Int32,
                      ByVal Servizio_Cod As Int32,
                      ByVal Stato_Cod As Int32,
                      ByVal Validita_Inizio As Date,
                      ByVal Validita_Fine As Date,
                      ByVal xFiltroAggiuntivo As String,
                      ByVal xOrderBy As String,
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                      Programmazione_Cod As Integer,
                      Programmazione_Entita_Cod As Integer
                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_conStatoAttuale()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.Append("SELECT      " & vbCrLf)
            stb.Append("    Imprese.rag_soc " & vbCrLf)
            stb.Append("    , Servizi.Servizio_Des " & vbCrLf)
            stb.Append("    , ISNULL(Pratiche_Stati.Stato_Cod, 0) AS Stato_Cod " & vbCrLf)
            stb.Append("    , ISNULL(st.WAnagraficaStati_Des, '') AS Stato_Des " & vbCrLf)
            stb.Append("    , st.Ordine " & vbCrLf)
            stb.Append("    , Pratiche_Stati.Validita_Inizio AS Validita_Inizio_Stato " & vbCrLf)
            stb.Append("    , Pratiche_Stati.Validita_Fine AS Validita_Fine_Stato " & vbCrLf)
            stb.Append("    , st.colore " & vbCrLf)
            stb.Append("    , Pratiche.* " & vbCrLf)

            stb.Append(" " & vbCrLf)
            stb.Append(" FROM Pratiche " & vbCrLf)
            stb.Append("      INNER JOIN pratiche_stati_attuali Pratiche_Stati  " & vbCrLf)
            stb.Append("   on Pratiche_Stati.Piva_SuperUser = Pratiche.Piva_SuperUser AND  Pratiche_Stati.Pratica_Cod = Pratiche.Pratica_Cod " & vbCrLf)
            stb.Append("   INNER JOIN Imprese ON Pratiche.Piva = Imprese.PIVA  " & vbCrLf)
            stb.Append("   INNER JOIN Servizi ON Pratiche.Servizio_Cod = Servizi.Servizio_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("   inner join WAnagraficaStati st on st.WAnagraficaStati_Cod = Pratiche_Stati.Stato_Cod  " & vbCrLf)

            stb.Append(" WHERE Pratiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.Append(" AND   Pratiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Pratica_Cod <> 0 Then
                stb.Append(" AND Pratiche.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If
            If Rag_Soc <> "" Then
                stb.Append(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                stb.Append(" AND Pratiche.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                stb.Append(" AND Pratiche.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND Pratiche.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                stb.Append(" AND Pratiche.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                stb.Append(" AND Pratiche.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                stb.Append(" AND Pratiche.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                stb.Append(" AND Pratiche_Stati.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                stb.Append(" AND Pratiche.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                stb.Append(" AND Pratiche.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Pratiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Pratiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function Leggi_conDatiImpianti(ByVal Pratica_Cod As Int32,
                      ByVal Piva_Padre As String,
                      ByVal Rag_Soc As String,
                      ByVal Piva As String,
                      ByVal Cuaa As String,
                      ByVal Sa_Cod As Int32,
                      ByVal Veg_Cod As Int32,
                      ByVal Id_Cod As Int32,
                      ByVal Servizio_Cod As Int32,
                      ByVal Stato_Cod As Int32,
                      ByVal Validita_Inizio As Date,
                      ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_conStatoAttuale()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.AppendLine("Select ")
            stb.AppendLine(" p.Pratica_Cod, ")
            stb.AppendLine(" p.Pratica_Des, ")
            stb.AppendLine(" p.Anno, ")
            stb.AppendLine(" p.Numero, ")
            stb.AppendLine(" padre.piva + ' '+padre.rag_soc as [Impresa Padre], ")
            stb.AppendLine(" ut.Cognome + ' '+ut.Nome as Utente , ")
            stb.AppendLine(" Imprese.PIVA, ")
            stb.AppendLine(" ic.val_cod As cuaa, ")
            stb.AppendLine(" Imprese.rag_soc, ")
            stb.AppendLine(" Pratica_Cod.val_cod, ")
            stb.AppendLine(" sv.veg_cod, ")
            stb.AppendLine(" sv.veg_des, ")
            stb.AppendLine(" ri.CUL_COD, ")
            stb.AppendLine(" c.Cul_Des, ")
            stb.AppendLine(" psa.Stato_Cod, ")
            stb.AppendLine(" s.servizio_des + ': ' +  was.WAnagraficaStati_Des AS WAnagraficaStati_Des, ")
            stb.AppendLine(" psa.note, ")
            stb.AppendLine(" ric.val_cod as CodiciIAF ")
            stb.AppendLine(" From Pratiche p ")
            stb.AppendLine(" INNER Join Pratiche_Stati_Attuali psa ON p.Piva_SuperUser=psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" LEFT JOIN Servizi s ON p.servizio_cod =s.servizio_cod")
            stb.AppendLine(" Left Join Imprese ON p.Piva = Imprese.PIVA ")
            stb.AppendLine(" Left Join Imprese_Codici ic ON Imprese.PIVA = ic.PIVA And ic.id_cod = 1010 ")
            stb.AppendLine(" Left Join GerarchiaImprese gi ON gi.figlio = Imprese.piva ")
            stb.AppendLine(" Left Join Imprese padre ON gi.padre = padre.piva ")
            stb.AppendLine(" Left Join Reg_Impianti_Codici pratica_Cod ON p.Piva = pratica_Cod.piva And pratica_Cod.id_cod = 1298 And CAST(pratica_Cod.val_cod as int) = p.Pratica_Cod  ")
            stb.AppendLine(" Left Join Reg_Impianti ri ON pratica_Cod.piva = ri.PIVA And pratica_Cod.sa_cod = ri.SA_COD And pratica_Cod.appezza=ri.APPEZZA And pratica_Cod.Id_Reg=ri.ID_REG ")
            stb.AppendLine(" Left Join Reg_Impianti_Codici ric ON p.Piva = ric.piva And ric.sa_cod = ri.SA_COD And ric.appezza = ri.APPEZZA And ric.Id_Reg = ri.id_reg And ric.id_cod = 1296 ")
            stb.AppendLine(" Left Join Cultivar c ON ri.CUL_COD = c.Cul_Cod ")
            stb.AppendLine(" Left Join SpecieVegetali sv ON sv.Veg_Cod = c.Veg_Cod ")
            stb.AppendLine(" Left Join WAnagraficaStati was ON was.WAnagraficaStati_Cod  = psa.Stato_Cod ")
            Dim DB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.AppendLine(" Left Join " & DB_Utenti & ".dbo.Utenti_Dettagli ut ON ut.codFisc = psa.Username_Modifica")

            stb.AppendLine(" WHERE 1=1 ")

            If Piva_Padre <> "" Then
                stb.AppendLine(" AND padre.piva = '" & Agro_SQL_SaveText(Piva_Padre) & "' ")
            End If
            If Pratica_Cod <> 0 Then
                stb.Append(" AND p.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If
            If Rag_Soc <> "" Then
                stb.Append(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                stb.Append(" AND Imprese.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                stb.Append(" AND ic.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND p.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                stb.Append(" AND sv.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                stb.Append(" AND p.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                stb.Append(" AND p.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                stb.Append(" AND psa.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            stb.Append(" AND   p.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.Append(" AND   p.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            stb.Append(" AND s.servizio_Cod in (2001, 2002, 2003, 2004) ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   p.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   p.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_conDatiImpianti_FiltroUtente(ByVal Pratica_Cod As Int32,
                                                          ByVal Piva_Padre As String,
                                                          ByVal Rag_Soc As String,
                                                          ByVal Piva As String,
                                                          ByVal Cuaa As String,
                                                          ByVal Sa_Cod As Int32,
                                                          ByVal Veg_Cod As Int32,
                                                          ByVal Id_Cod As Int32,
                                                          ByVal Servizio_Cod As Int32,
                                                          ByVal Stato_Cod As Int32,
                                                          ByVal Validita_Inizio As Date,
                                                          ByVal Validita_Fine As Date,
                                                          ByVal Filtro_Visibilita_Utente As Boolean,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByVal xOrderBy As String,
                                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_conStatoAttuale()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.AppendLine("Select ")
            stb.AppendLine(" p.Pratica_Cod, ")
            stb.AppendLine(" p.Pratica_Des, ")
            stb.AppendLine(" p.Anno, ")
            stb.AppendLine(" p.Numero, ")
            stb.AppendLine(" padre.piva + ' '+padre.rag_soc as [Impresa Padre], ")
            stb.AppendLine(" ut.Cognome + ' '+ut.Nome as Utente , ")
            stb.AppendLine(" Imprese.PIVA, ")
            stb.AppendLine(" ic.val_cod As cuaa, ")
            stb.AppendLine(" Imprese.rag_soc, ")
            stb.AppendLine(" Pratica_Cod.val_cod, ")
            stb.AppendLine(" sv.veg_cod, ")
            stb.AppendLine(" sv.veg_des, ")
            stb.AppendLine(" ri.CUL_COD, ")
            stb.AppendLine(" c.Cul_Des, ")
            stb.AppendLine(" psa.Stato_Cod, ")
            stb.AppendLine(" s.servizio_des + ': ' +  was.WAnagraficaStati_Des AS WAnagraficaStati_Des, ")
            stb.AppendLine(" psa.note, ")
            stb.AppendLine(" ric.val_cod as CodiciIAF ")
            stb.AppendLine(" From Pratiche p ")
            stb.AppendLine(" INNER Join Pratiche_Stati_Attuali psa ON p.Piva_SuperUser=psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" LEFT JOIN Servizi s ON p.servizio_cod =s.servizio_cod")
            stb.AppendLine(" Left Join Imprese ON p.Piva = Imprese.PIVA ")
            stb.AppendLine(" Left Join Imprese_Codici ic ON Imprese.PIVA = ic.PIVA And ic.id_cod = 1010 ")
            stb.AppendLine(" Left Join GerarchiaImprese gi ON gi.figlio = Imprese.piva ")
            stb.AppendLine(" Left Join Imprese padre ON gi.padre = padre.piva ")
            stb.AppendLine(" Left Join Reg_Impianti_Codici pratica_Cod ON p.Piva = pratica_Cod.piva And pratica_Cod.id_cod = 1298 And CAST(pratica_Cod.val_cod as int) = p.Pratica_Cod  ")
            stb.AppendLine(" Left Join Reg_Impianti ri ON pratica_Cod.piva = ri.PIVA And pratica_Cod.sa_cod = ri.SA_COD And pratica_Cod.appezza=ri.APPEZZA And pratica_Cod.Id_Reg=ri.ID_REG ")
            stb.AppendLine(" Left Join Reg_Impianti_Codici ric ON p.Piva = ric.piva And ric.sa_cod = ri.SA_COD And ric.appezza = ri.APPEZZA And ric.Id_Reg = ri.id_reg And ric.id_cod = 1296 ")
            stb.AppendLine(" Left Join Cultivar c ON ri.CUL_COD = c.Cul_Cod ")
            stb.AppendLine(" Left Join SpecieVegetali sv ON sv.Veg_Cod = c.Veg_Cod ")
            stb.AppendLine(" Left Join WAnagraficaStati was ON was.WAnagraficaStati_Cod  = psa.Stato_Cod ")
            stb.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  " & vbCrLf)
            Dim DB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.AppendLine(" Left Join " & DB_Utenti & ".dbo.Utenti_Dettagli ut ON ut.codFisc = psa.Username_Modifica")

            stb.AppendLine(" WHERE 1=1 ")

            If Filtro_Visibilita_Utente Then
                stb.Append(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            If Piva_Padre <> "" Then
                stb.AppendLine(" AND padre.piva = '" & Agro_SQL_SaveText(Piva_Padre) & "' ")
            End If
            If Pratica_Cod <> 0 Then
                stb.Append(" AND p.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If
            If Rag_Soc <> "" Then
                stb.Append(" AND Imprese.rag_soc like '%" & Agro_SQL_SaveText(Rag_Soc) & "%' ")
            End If
            If Piva <> "" Then
                stb.Append(" AND Imprese.Piva like '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If
            If Cuaa <> "" Then
                stb.Append(" AND ic.Cuaa like '%" & Agro_SQL_SaveText(Cuaa) & "%' ")
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND p.sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                stb.Append(" AND sv.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Id_Cod <> 0 Then
                stb.Append(" AND p.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Servizio_Cod <> 0 Then
                stb.Append(" AND p.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                stb.Append(" AND psa.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            stb.Append(" AND   p.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.Append(" AND   p.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            stb.Append(" AND s.servizio_Cod in (2001, 2002, 2003, 2004) ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   p.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   p.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_TransizioniDisponibiliDatoStato_xWorkflow(
                ByVal StatoIniziale_Cod As Integer,
                ByVal WWorkFlow_COD As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_TransizioniDisponibiliDatoStato()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)
            stb.Append("             select a.WAnagraficaStati_Cod, a.WAnagraficaStati_Des,WTransizioniDiStatoConfigurazione_Des  " & vbCrLf)
            stb.Append(" from WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.Append("    inner join WAnagraficaStati a on c.Stato_Destinazione_cod = a.WAnagraficaStati_Cod " & vbCrLf)

            stb.Append(" where Stato_Origine_Cod =  " & StatoIniziale_Cod & vbCrLf)
            stb.Append(" and a.WWorkFlow_COD  =  " & WWorkFlow_COD & vbCrLf)

            stb.Append("  " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select




            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_TransizioniDisponibiliDatoStato(
            ByVal StatoIniziale_Cod As String,
            ByVal servizio_Cod As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
            Optional ByVal Validita_Fine As Date = AGRODATAFINE
        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_TransizioniDisponibiliDatoStato()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.AppendLine("             select a.WAnagraficaStati_Cod, a.WAnagraficaStati_Des,WTransizioniDiStatoConfigurazione_Des  " & vbCrLf)
            stb.AppendLine(" from WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.AppendLine("    inner join WAnagraficaStati a on c.Stato_Destinazione_cod = a.WAnagraficaStati_Cod " & vbCrLf)

            stb.AppendLine(" where Stato_Origine_Cod =  " & StatoIniziale_Cod & vbCrLf)
            stb.AppendLine(" and c.Servizio_cod  =  " & servizio_Cod & vbCrLf)

            If Validita_Inizio <> AGRODATAINIZIO Then
                stb.AppendLine(" AND a.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND a.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select




            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
            ByVal StatoIniziale_Cod As String,
            ByVal servizio_Cod As String,
            ByVal Gruppi_Utente_cod As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
            Optional ByVal Validita_Fine As Date = AGRODATAFINE
        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)
            stb.AppendLine(" SELECT a.WAnagraficaStati_Cod, a.WAnagraficaStati_Des,WTransizioniDiStatoConfigurazione_Des  " & vbCrLf)
            stb.AppendLine(" FROM WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.AppendLine(" INNER JOIN WAnagraficaStati a on c.Stato_Destinazione_cod = a.WAnagraficaStati_Cod " & vbCrLf)
            stb.AppendLine(" INNER JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.GruppiUtente_TransizioniDiStato g on c.Stato_Destinazione_cod = g.Stato_Destinazione_cod AND c.Stato_Origine_Cod = g.Stato_Origine_Cod AND c.Servizio_cod=g.servizio_Cod " & vbCrLf)

            stb.AppendLine(" where 1 = 1 ")

            If StatoIniziale_Cod <> 0 Then
                stb.AppendLine(" AND c.Stato_Origine_Cod =  " & StatoIniziale_Cod & vbCrLf)
            End If

            If servizio_Cod <> 0 Then
                stb.AppendLine(" AND c.Servizio_cod  =  " & servizio_Cod & vbCrLf)
            End If
            If Gruppi_Utente_cod <> 0 Then
                stb.AppendLine(" AND g.Gruppo_Utente = " & Gruppi_Utente_cod)
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                stb.AppendLine(" AND a.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND a.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            stb.AppendLine("  " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select




            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_StatiPerPassaggioDiStato(
            ByVal FiltroPratica_Cod As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_StatiPerPassaggioDiStato()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            If FiltroPratica_Cod = "" Then
                FiltroPratica_Cod = "-11"
            End If


            stb.Append(" " & vbCrLf)
            stb.Append(" select * " & vbCrLf)
            stb.Append(" from ( " & vbCrLf)
            stb.Append("    select wwf.Servizio_Cod,  wwf.WAnagraficaStati_Cod, wwf.WAnagraficaStati_Des, wwf.WorkFlow_Cod, wwf.WorkFlow_des, wwf.Colore " & vbCrLf)
            stb.Append("    from ( " & vbCrLf)
            stb.Append("        select ws.WAnagraficaStati_Cod, ws.WAnagraficaStati_Des, p.Servizio_Cod, w.WorkFlow_Cod, w.WorkFlow_des, ws.Colore " & vbCrLf)
            stb.Append("        from Pratiche p " & vbCrLf)
            stb.Append("        inner join Pratiche_Stati_Attuali att " & vbCrLf)
            stb.Append("            on att.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
            stb.Append("            and att.Piva_SuperUser = p.Piva_SuperUser   " & vbCrLf)
            stb.Append("        inner join WTransizioniDiStatoConfigurazione cc " & vbCrLf)
            stb.Append("            on cc.Stato_Origine_Cod = att.Stato_Cod   " & vbCrLf)
            stb.Append("        inner join WAnagraficaStati ws  " & vbCrLf)
            stb.Append("            on att.Stato_Cod = ws.WAnagraficaStati_Cod   " & vbCrLf)
            stb.Append("        inner join wworkflow w on  " & vbCrLf)
            stb.Append("            ws.WWorkFlow_cod = w.WorkFlow_cod  " & vbCrLf)
            stb.Append("        where p.Pratica_Cod in (" & Agro_SQL_Save_Clausola_IN(FiltroPratica_Cod) & ")  " & vbCrLf)
            stb.Append("        group by p.Servizio_Cod, ws.WAnagraficaStati_Cod, ws.WAnagraficaStati_Des, p.Servizio_Cod, w.WorkFlow_Cod, w.WorkFlow_des, ws.Colore " & vbCrLf)
            stb.Append("    ) wwf " & vbCrLf)
            stb.Append("    group by wwf.Servizio_Cod,  wwf.WAnagraficaStati_Cod, wwf.WAnagraficaStati_Des, wwf.WorkFlow_Cod, wwf.WorkFlow_des, wwf.Colore  " & vbCrLf)
            stb.Append(" ) xDescr " & vbCrLf)
            stb.Append(" inner join ( " & vbCrLf)
            stb.Append(" select wwf1.WorkFlow_Cod " & vbCrLf)
            stb.Append(" from ( " & vbCrLf)
            stb.Append("    select wwf.WAnagraficaStati_Cod, wwf.WAnagraficaStati_Des, wwf.WorkFlow_Cod, wwf.WorkFlow_des, wwf.Colore " & vbCrLf)
            stb.Append("    from ( " & vbCrLf)
            stb.Append("        select ws.WAnagraficaStati_Cod, ws.WAnagraficaStati_Des, p.Servizio_Cod, w.WorkFlow_Cod, w.WorkFlow_des, ws.Colore " & vbCrLf)
            stb.Append("        from Pratiche p " & vbCrLf)
            stb.Append("        inner join Pratiche_Stati_Attuali att " & vbCrLf)
            stb.Append("            on att.Pratica_Cod = p.Pratica_Cod  " & vbCrLf)
            stb.Append("            and att.Piva_SuperUser = p.Piva_SuperUser   " & vbCrLf)
            stb.Append("        inner join WTransizioniDiStatoConfigurazione cc " & vbCrLf)
            stb.Append("            on cc.Stato_Origine_Cod = att.Stato_Cod   " & vbCrLf)
            stb.Append("        inner join WAnagraficaStati ws  " & vbCrLf)
            stb.Append("            on att.Stato_Cod = ws.WAnagraficaStati_Cod   " & vbCrLf)
            stb.Append("        inner join wworkflow w on  " & vbCrLf)
            stb.Append("            ws.WWorkFlow_cod = w.WorkFlow_cod  " & vbCrLf)
            stb.Append("        where p.Pratica_Cod in (" & Agro_SQL_Save_Clausola_IN(FiltroPratica_Cod) & ")  " & vbCrLf)
            stb.Append("        group by p.Servizio_Cod, ws.WAnagraficaStati_Cod, ws.WAnagraficaStati_Des, p.Servizio_Cod, w.WorkFlow_Cod, w.WorkFlow_des, ws.Colore " & vbCrLf)
            stb.Append("    ) wwf " & vbCrLf)
            stb.Append("    group by wwf.WAnagraficaStati_Cod, wwf.WAnagraficaStati_Des, wwf.WorkFlow_Cod, wwf.WorkFlow_des, wwf.Colore  " & vbCrLf)
            stb.Append("    ) wwf1 " & vbCrLf)
            stb.Append(" group by wwf1.WorkFlow_Cod  " & vbCrLf)
            stb.Append(" having COUNT(*) = 1 " & vbCrLf)
            stb.Append(" ) xDistinct " & vbCrLf)
            stb.Append("    on xDistinct.WorkFlow_Cod = xDescr.WorkFlow_Cod  " & vbCrLf)


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_StatoInizialeServizioDato_WWorkFlow(
         ByVal WWorkflow_Cod As String,
         ByVal xFiltroAggiuntivo As String,
         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
     ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_StatoInizialeServizio()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StatoIniziale_Cod As Integer = 0

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)
            stb.Append(" select MIN(a.WAnagraficaStati_Cod) AS StatoIniziale_Cod " & vbCrLf)
            stb.Append(" from WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.Append("    inner join WAnagraficaStati a on c.Stato_Origine_Cod = a.WAnagraficaStati_Cod " & vbCrLf)


            stb.Append(" where a.wworkflow_cod  =  " & WWorkflow_Cod & vbCrLf)



            stb.Append("  " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                StatoIniziale_Cod = DT.Rows(0).Item("StatoIniziale_Cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return StatoIniziale_Cod

    End Function


    Public Function Leggi_StatoInizialeServizio(
          ByVal Servizio_Cod As String,
          ByVal xFiltroAggiuntivo As String,
          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
      ) As Integer


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_StatoInizialeServizio()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StatoIniziale_Cod As Integer = 0

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)
            stb.Append(" select MIN(a.WAnagraficaStati_Cod) AS StatoIniziale_Cod " & vbCrLf)
            stb.Append(" from WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.Append("    inner join WAnagraficaStati a on c.Stato_Origine_Cod = a.WAnagraficaStati_Cod " & vbCrLf)


            stb.Append(" where c.Servizio_cod  =  " & Servizio_Cod & vbCrLf)



            stb.Append("  " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                StatoIniziale_Cod = DT.Rows(0).Item("StatoIniziale_Cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return StatoIniziale_Cod

    End Function

    Public Function Leggi_StatiServizio(
       ByVal Servizio_Cod As String,
       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
   ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_StatiServizio()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)


            stb.Append(" (select a.WAnagraficaStati_Cod, a.WAnagraficaStati_Des " & vbCrLf)
            stb.Append(" from WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.Append("    inner join WAnagraficaStati a on c.Stato_destinazione_Cod = a.WAnagraficaStati_Cod " & vbCrLf)
            stb.Append(" where c.Servizio_cod  =  " & Servizio_Cod & vbCrLf)
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            stb.Append(" ) " & vbCrLf)

            stb.Append(" UNION " & vbCrLf)

            stb.Append(" (select TOP 1 a.WAnagraficaStati_Cod, a.WAnagraficaStati_Des " & vbCrLf)
            stb.Append(" from WTransizioniDiStatoConfigurazione c " & vbCrLf)
            stb.Append("    inner join WAnagraficaStati a on c.Stato_Origine_Cod = a.WAnagraficaStati_Cod " & vbCrLf)
            stb.Append(" where c.Servizio_cod  =  " & Servizio_Cod & vbCrLf)

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            stb.Append(" ) " & vbCrLf)

            stb.Append("  " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_WAnagraficaStati(ByVal WAnagraficaStati_Cod As Integer,
                                           WWorkflow_Cod As Integer,
                                           objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_StatiServizio()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)


            stb.Append(" SELECT * " & vbCrLf)
            stb.Append(" FROM WAnagraficaStati " & vbCrLf)
            stb.Append(" WHERE 1=1 " & vbCrLf)

            If WAnagraficaStati_Cod <> 0 Then
                stb.Append(" AND WAnagraficaStati_Cod = " & Agro_SQL_SaveNum(WAnagraficaStati_Cod) & " " & vbCrLf)
            End If

            If WWorkflow_Cod <> 0 Then
                stb.Append(" AND WWorkflow_Cod = " & Agro_SQL_SaveNum(WWorkflow_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function Leggi_WAnagraficaStati_Da_Servizio(ByVal WAnagraficaStati_Cod As Integer,
                                           Servizio_Cod As Integer,
                                           objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_R.Leggi_StatiServizio()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" " & vbCrLf)


            stb.Append(" SELECT Stato_Origine_Cod as Stato_Cod, WAnagraficaStati.WAnagraficaStati_Des " & vbCrLf)
            stb.Append(" FROM WTransizioniDiStatoConfigurazione " & vbCrLf)
            stb.Append(" JOIN WAnagraficaStati ON WTransizioniDiStatoConfigurazione.Stato_Origine_Cod = WAnagraficaStati.WAnagraficaStati_Cod " & vbCrLf)
            stb.Append(" WHERE 1=1 " & vbCrLf)

            If WAnagraficaStati_Cod <> 0 Then
                stb.Append(" AND WAnagraficaStati_Cod = " & Agro_SQL_SaveNum(WAnagraficaStati_Cod) & " " & vbCrLf)
            End If

            If Servizio_Cod <> 0 Then
                stb.Append(" AND Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " " & vbCrLf)
            End If

            stb.Append(" UNION " & vbCrLf)

            stb.Append(" SELECT Stato_Destinazione_cod as Stato, WAnagraficaStati.WAnagraficaStati_Des " & vbCrLf)
            stb.Append(" FROM WTransizioniDiStatoConfigurazione " & vbCrLf)
            stb.Append(" JOIN WAnagraficaStati ON WTransizioniDiStatoConfigurazione.Stato_Destinazione_cod = WAnagraficaStati.WAnagraficaStati_Cod " & vbCrLf)
            stb.Append(" WHERE 1=1 " & vbCrLf)

            If WAnagraficaStati_Cod <> 0 Then
                stb.Append(" AND WAnagraficaStati_Cod = " & Agro_SQL_SaveNum(WAnagraficaStati_Cod) & " " & vbCrLf)
            End If

            If Servizio_Cod <> 0 Then
                stb.Append(" AND Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiConfigurazioni(
                                ByVal G2GLocalConfigurazioni_COD As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim dt As DataTable
        Dim stb As New StringBuilder
        Dim NomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GLocal_R.LeggiConfigurazioni"
        Dim MessaggioErrore As String = ""
        Try

            stb.Append("        select * " & vbCrLf)
            stb.Append(" from G2GLocalConfigurazioni " & vbCrLf)

            If G2GLocalConfigurazioni_COD <> 0 Then
                stb.Append(" where G2GLocalConfigurazioni_COD =  " & G2GLocalConfigurazioni_COD & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt

    End Function

End Class

Public Class Pratiche_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Pratica_Cod As Int32,
                            ByVal Pratica_Des As String,
                            ByVal Piva As String,
                            ByVal Cuaa As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Veg_Cod As Int32,
                            ByVal Id_Cod As Int32,
                            ByVal Servizio_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Data_creazione As Date,
                            ByVal Data_modifica As Date,
                            ByVal username_creazione As String,
                            ByVal username_modifica As String,
                            ByVal Anno As Integer,
                            ByVal Numero_Pratica As String,
                            ByVal Blocco_Flag As String,
                            ByVal Blocco_Data As Date,
                            ByVal Blocco_Username As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Programmazione_Entita_Cod As Integer
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If


        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Pratiche ")
            StrSQL.Append("                   (Piva_SuperUser, Pratica_Cod, Pratica_Des, ")
            StrSQL.Append("                    Piva, Cuaa, Sa_Cod, Veg_Cod, Id_Cod,      ")
            StrSQL.Append("                    Servizio_Cod, Validita_Inizio, Validita_Fine, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Anno, Numero, ")
            StrSQL.Append("                    Blocco_Flag, Blocco_Data, Blocco_Username, ")
            StrSQL.Append("                    Programmazione_Cod, Programmazione_Entita_Cod ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pratica_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Pratica_Des) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cuaa) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Servizio_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveNum(Anno) & " ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(Numero_Pratica) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Blocco_Data) & " ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(Blocco_Username) & "' ")
            StrSQL.Append("		    , " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            StrSQL.Append("		    , " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            StrSQL.Append(" )")
            '---------------------------------------------


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


    Public Function AggiornaDate(ByVal Pratica_Cod As Int32,
                                 ByVal Validita_Inizio As Date,
                                 ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_W.AggiornaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Pratiche SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            If Validita_Inizio <> #2/1/1900# Then
                StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            End If

            StrSQL.Append("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    Public Function AggiornaNumero(ByVal Pratica_Cod As Int32,
                                 ByVal numero As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_W.AggiornaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Pratiche SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Numero   =  " & Agro_SQL_SaveText_NULL(numero))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    Public Function AggiornaServizio(ByVal Pratica_Cod As Int32,
                                 ByVal Servizio_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_W.AggiornaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Pratiche SET ")
            StrSQL.Append("              Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & "")
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function

    Public Function Cancella(
                            ByVal Pratica_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Pratiche ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
                StrSQL.Append(" AND Inviato >= 0")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Pratiche ")
                StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If

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


    '#################################################################
    Public Function ModificaG2G(
                            ByVal G2GLocalConfigurazioni_COD As Integer _
                          , ByVal G2GLocalConfigurazioni_DES As String _
                          , ByVal G2GLocalConfigurazioni_CFG As String _
                          , ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ModificaG2G()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            StrSQL.Append(" UPDATE G2GLocalConfigurazioni ")
            StrSQL.Append(" SET ")
            StrSQL.Append("         G2GLocalConfigurazioni_DES = '" & Agro_SQL_SaveText(G2GLocalConfigurazioni_DES) & "' ")
            StrSQL.Append("        , G2GLocalConfigurazioni_CFG = '" & Agro_SQL_SaveText(G2GLocalConfigurazioni_CFG) & "' ")
            StrSQL.Append(" WHERE   G2GLocalConfigurazioni_COD = " & Agro_SQL_SaveNum(G2GLocalConfigurazioni_COD) & " ")

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

    Public Function Blocca(ByVal Pratica_Cod As Int32,
                           ByVal Blocco_Flag As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_W.AggiornaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Pratiche SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Blocco_Flag = " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            StrSQL.Append("             ,Blocco_Data = " & Agro_SQL_SaveDateTime_NULL(DateTime.Now) & " ")
            StrSQL.Append("             ,Blocco_Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function



End Class
