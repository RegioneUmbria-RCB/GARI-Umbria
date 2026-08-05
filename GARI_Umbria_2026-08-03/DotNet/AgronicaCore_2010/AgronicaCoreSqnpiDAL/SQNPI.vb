Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class SQNPI_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiDiario_Domande( _
        ByVal IdentificativoFlusso As String, _
        ByVal Stato_Richiesto As Integer, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" select  " & vbCrLf)
            Stb.Append("  distinct        " & vbCrLf)
            Stb.Append("       dd.*           " & vbCrLf)
            Stb.Append("     , '<div style=''white-space:nowrap''><span class=''stato'' style=''background-color: ' + wa.Colore  + '''></span><span style=''color: ' + wa.Colore  + '''>' + wa.WAnagraficaStati_Des + '</span></div>'as Stato_Descrizione  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  from dbo.ws_SQNPI_Domanda dd  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join wanagraficaStati wa  " & vbCrLf)
            Stb.Append("         on wa.WAnagraficaStati_Cod = dd.Stato_Gias  " & vbCrLf)


            If Stato_Richiesto <> 0 Then
                Stb.Append(" AND DD.Stato_Gias = '" & Agro_SQL_SaveText(Stato_Richiesto) & "'" & vbCrLf)
            End If

            If Not String.IsNullOrEmpty(IdentificativoFlusso) Then
                Stb.Append(" AND DD.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)
            End If




            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------

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


    '##############################################################################################
    Public Function LeggiDiario_DettagliSoci( _
        ByVal IdentificativoFlusso As String, _
        ByVal Stato_Richiesto As Integer, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0



            Stb.Append(" select  " & vbCrLf)
            Stb.Append(" distinct  " & vbCrLf)
            Stb.Append("       d.IdentificativoFlusso     " & vbCrLf)
            Stb.Append("     , t.Programmazione_Des  " & vbCrLf)
            Stb.Append("     , ii.rag_soc   " & vbCrLf)
            Stb.Append("     , '<div style=''white-space:nowrap''><span class=''stato'' style=''background-color: ' + wa.Colore  + '''></span><span style=''color: ' + wa.Colore  + '''>' + wa.WAnagraficaStati_Des + '</span></div>'as Stato_Descrizione  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  from dbo.ws_SQNPI_Domanda d " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join dbo.ws_SQNPI_Domanda_Soggetto sogg  " & vbCrLf)
            Stb.Append("         on sogg.IdentificativoFlusso = d.IdentificativoFlusso  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join ws_SQNPI_Domanda_Soggetto_Programmazione_Testata Plann  " & vbCrLf)
            Stb.Append("         on plann.IdentificativoFlusso = d.IdentificativoFlusso          " & vbCrLf)
            Stb.Append("         and Plann.SoggettoCUAA = sogg.SoggettoCUAA " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join Programmazione_Testata t  " & vbCrLf)
            Stb.Append("         on t.Programmazione_Cod = Plann.Programmazione_Cod   " & vbCrLf)
            Stb.Append("         and t.Piva_SuperUser = Plann.Piva_SuperUser   " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("   inner join Imprese ii  " & vbCrLf)
            Stb.Append("         on ii.PIVA = t.Piva      " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join wanagraficaStati wa  " & vbCrLf)
            Stb.Append("         on wa.WAnagraficaStati_Cod = t.Stato_SQNPI ")

            Stb.Append(" WHERE D.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)

            If Stato_Richiesto <> 0 Then
                Stb.Append(" AND t.Stato_SQNPI = '" & Agro_SQL_SaveText(Stato_Richiesto) & "'" & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------

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

    '##############################################################################################
    Public Function LeggiDiario_DettaglioInvioDatiWS( _
        ByVal IdentificativoFlusso As String, _
        ByVal Stato_Richiesto As Integer, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" select  " & vbCrLf)
            Stb.Append("  distinct  " & vbCrLf)
            Stb.Append("       i.ws_SQNPI_LogInvio_cod  " & vbCrLf)
            Stb.Append("     , ws_SQNPI_LogInvio_Des   " & vbCrLf)
            Stb.Append("     , i.DataInvio  " & vbCrLf)
            Stb.Append("     , i.DataElaborazione  " & vbCrLf)
            Stb.Append("     , '<div style=''white-space:nowrap''><span class=''stato'' style=''background-color: ' + wa.Colore  + '''></span><span style=''color: ' + wa.Colore  + '''>' + wa.WAnagraficaStati_Des + '</span></div>'as Stato_Descrizione  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  from ws_sqnpi_loginvio i  " & vbCrLf)
            Stb.Append("     inner join ws_SQNPI_LogInvio_Dettaglio d  " & vbCrLf)
            Stb.Append("         on i.ws_SQNPI_LogInvio_Cod = d.ws_SQNPI_LogInvio_Cod  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join dbo.ws_SQNPI_Domanda dd  " & vbCrLf)
            Stb.Append("         on dd.IdentificativoFlusso = d.IdentificativoFlusso  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("     inner join wanagraficaStati wa  " & vbCrLf)
            Stb.Append("         on wa.WAnagraficaStati_Cod = i.Stato_Gias  " & vbCrLf)
            Stb.Append("    where d.IdentificativoFlusso =  '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)

            If Stato_Richiesto <> 0 Then
                Stb.Append(" AND i.Stato_Gias = '" & Agro_SQL_SaveText(Stato_Richiesto) & "'" & vbCrLf)
            End If




            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------

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


    '##############################################################################################
    Public Function LeggiDiario_DettaglioInvioDatiWS_Errori( _
        ByVal ws_SQNPI_LogInvio_Cod As Integer, _
        ByVal Stato_Richiesto As Integer, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0



            Stb.Append("select * " & vbCrLf)
            Stb.Append(" from ws_SQNPI_LogInvio_Dettaglio_Errori " & vbCrLf)
            Stb.Append(" where ws_SQNPI_LogInvio_Cod = " & ws_SQNPI_LogInvio_Cod)




            If Stato_Richiesto <> 0 Then
                Stb.Append(" AND D.Stato_Gias = '" & Agro_SQL_SaveText(Stato_Richiesto) & "'" & vbCrLf)
            End If




            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------

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

    '##############################################################################################
    Public Function LeggiPerVerifica( _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" select distinct dd.ws_SQNPI_LogInvio_Cod, dd.IdentificativoFlusso, l.skXml " & vbCrLf)
            Stb.Append(" from dbo.ws_SQNPI_LogInvio l " & vbCrLf)

            Stb.Append("    inner join dbo.ws_SQNPI_LogInvio_Dettaglio dd " & vbCrLf)
            Stb.Append("        on l.ws_SQNPI_LogInvio_Cod = dd.ws_SQNPI_LogInvio_Cod " & vbCrLf)

            Stb.Append("    inner join dbo.ws_SQNPI_Domanda d " & vbCrLf)
            Stb.Append("        on d.IdentificativoFlusso = dd.IdentificativoFlusso")

            Stb.Append(" where d.Stato_Gias = " & enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_In_Fase_di_verifica_per_errori_formali & vbCrLf)
            Stb.Append(" and l.dataElaborazione is null " & vbCrLf)

            'Stb.Append(" and Not exists ( " & vbCrLf)
            'Stb.Append("    select 1  " & vbCrLf)
            'Stb.Append("    from ws_SQNPI_LogInvio_Dettaglio_Errori err " & vbCrLf)
            'Stb.Append("    where err.ws_SQNPI_LogInvio_Cod = dd.ws_SQNPI_LogInvio_Cod " & vbCrLf)
            'Stb.Append(" ) " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------

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


    '##############################################################################################
    Public Function LeggiPerInvio( _
        ByVal IdentificativoFlusso As String, _
        ByVal Stati_Richiesto As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" select  " & vbCrLf)
            Stb.Append("      D.* " & vbCrLf)
            Stb.Append("    , Sog.SoggettoCuaa as Cuaa_Richiedente " & vbCrLf)
            Stb.Append("    , Terre.PROV " & vbCrLf)
            Stb.Append("    , Terre.COM " & vbCrLf)
            Stb.Append("    , case when Terre.Sezione <> '0' then Terre.Sezione else '' end as  SEZIONE " & vbCrLf)
            Stb.Append("    , Terre.FOGLIO " & vbCrLf)
            Stb.Append("    , right('00000' + cast(Terre.NUMERO as varchar(5)), 5) AS Numero " & vbCrLf)
            Stb.Append("    , case when Terre.Subalterno <> '0' then Terre.Subalterno else '' end as  SUBALTERNO " & vbCrLf)
            Stb.Append("    , Terre.Prodotto_AGEA " & vbCrLf)
            Stb.Append("    , Terre.Varietà_AGEA " & vbCrLf)
            Stb.Append("    , Terre.IdentificativoFlusso " & vbCrLf)
            Stb.Append("    , Terre.SoggettoCUAA " & vbCrLf)
            Stb.Append("    , Terre.ResaMedia    " & vbCrLf)
            Stb.Append("    , Terre.MacroUso    " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" from dbo.ws_SQNPI_Domanda D " & vbCrLf)
            Stb.Append("    inner join dbo.ws_SQNPI_Domanda_Soggetto Sog " & vbCrLf)
            Stb.Append("        on D.IdentificativoFlusso = Sog.IdentificativoFlusso " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join dbo.ws_SQNPI_Domanda_Soggetto_Catasto_Terreno Terre " & vbCrLf)
            Stb.Append("        on Sog.IdentificativoFlusso = terre.IdentificativoFlusso         " & vbCrLf)
            Stb.Append("        and  Sog.SoggettoCUAA = terre.SoggettoCUAA " & vbCrLf)
            Stb.Append("  WHERE 1=1 " & vbCrLf)



            If Stati_Richiesto <> "" Then
                Stb.Append(" AND D.Stato_Gias in (" & Agro_SQL_SaveText(Stati_Richiesto) & ")" & vbCrLf)
            End If

            If Not String.IsNullOrEmpty(IdentificativoFlusso) Then
                Stb.Append(" AND D.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)
            End If




            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            ''--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            ''--------------------------------------------------------------------------

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

    '##############################################################################################
    Public Function LeggiPlanningNonMemorizzatiSuCache( _
        ByVal IdentificativoFlusso As String, _
        ByVal Stato_Richiesto As Integer, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" select T.Piva_SuperUser, T.programmazione_Cod, Year( T.Validita_Inizio) as anno " & vbCrLf)
            Stb.Append(" from Programmazione_Testata T " & vbCrLf)
            Stb.Append(" where t.Stato_SQNPI = " & Stato_Richiesto & vbCrLf)
            Stb.Append(" and Not exists ( " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    select 1 " & vbCrLf)
            Stb.Append("    from ws_SQNPI_Domanda_Soggetto_Programmazione_Testata DTes " & vbCrLf)
            Stb.Append("    where DTes.Programmazione_Cod = T.Programmazione_Cod  " & vbCrLf)
            Stb.Append("    and DTes.Piva_SuperUser = T.Piva_SuperUser " & vbCrLf)

            If Not String.IsNullOrEmpty(IdentificativoFlusso) Then
                Stb.Append("    and DTes.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)
            End If

            Stb.Append(" ) " & vbCrLf)




            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   T.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   T.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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


    '##############################################################################################
    Public Function LeggiPlanningMemorizzatiSuCache_CUAA_DaRimuovere( _
        ByVal IdentificativoFlusso As String, _
        ByVal Stato_Richiesto As Integer, _
        ByVal Stato_ListaEsclusi As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" " & vbCrLf)
            Stb.Append(" Select distinct DTes.SoggettoCUAA " & vbCrLf)
            Stb.Append(" from dbo.ws_SQNPI_Domanda_Soggetto_Programmazione_Testata DTes " & vbCrLf)
            Stb.Append("    inner join Programmazione_Testata Tes " & vbCrLf)
            Stb.Append("        on Tes.Programmazione_Cod = DTes.Programmazione_Cod " & vbCrLf)
            Stb.Append("        and Tes.Piva_SuperUser = DTes.Piva_SuperUser  " & vbCrLf)
            Stb.Append("    inner join ws_SQNPI_Domanda D " & vbCrLf)
            Stb.Append("        on D.IdentificativoFlusso = DTes.IdentificativoFlusso " & vbCrLf)
            Stb.Append(" where Tes.Stato_SQNPI = " & Stato_Richiesto & vbCrLf)
            Stb.Append(" and D.Stato_Gias not in (" & Agro_SQL_Save_Clausola_IN(Stato_ListaEsclusi) & ") " & vbCrLf)
            Stb.Append(" and D.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'")





            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   DTes.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   DTes.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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


    '##############################################################################################
    Public Function LeggiDomanda_PerAccodaDati( _
        ByVal AnnoRiferimento As String, _
        ByVal CuaaOP As String, _
        ByVal Stati_Esclusi As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "SQNPI_R.LeggiLeggiPerInvio()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" Select IdentificativoFlusso " & vbCrLf)
            Stb.Append(" from dbo.ws_SQNPI_Domanda " & vbCrLf)
            Stb.Append(" where IdentificativoFlusso like '" & AnnoRiferimento & "/" & CuaaOP & "/%' " & vbCrLf)
            Stb.Append(" and Stato_Gias not in (" & Agro_SQL_Save_Clausola_IN(Stati_Esclusi) & ") " & vbCrLf)




            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------



            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" order by IdentificativoFlusso desc ")
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


'#################################################################
'#################################################################
'#################################################################

Public Class SQNPI_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function AvanzamentoDiStato( _
                    ByVal IdentificativoFascicolo As String, _
                    ByVal tabella As String, _
                    ByVal Stato_Destianzione As Integer, _
                    ByVal ws_SQNPI_LogInvio_Cod As Integer, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" UPDATE DD SET " & vbCrLf)

            Stb.Append("                Stato_Gias =  " & Agro_SQL_SaveNum(Stato_Destianzione) & " " & vbCrLf)
            Stb.Append("              , Data_Modifica =  " & Agro_SQL_SaveDate(Data_modifica) & vbCrLf)
            Stb.Append("              , UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'" & vbCrLf)

            Stb.Append(" FROM " & tabella & " DD " & vbCrLf)


            Dim whereAnd As String = " WHERE "
            If IdentificativoFascicolo <> "" Then
                Stb.Append(" where IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFascicolo) & "'")
                whereAnd = " AND "
            End If


            If ws_SQNPI_LogInvio_Cod <> 0 Then
                Stb.Append(whereAnd & " ws_SQNPI_LogInvio_Cod = " & ws_SQNPI_LogInvio_Cod & vbCrLf)
            End If


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
    Public Function AvanzamentoDiStato_Planning_dato_IdentificativoFlusso( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal Stato_Destianzione As Integer, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" UPDATE TES SET " & vbCrLf)

            Stb.Append("                Stato_SQNPI =  " & Agro_SQL_SaveNum(Stato_Destianzione) & " " & vbCrLf)
            Stb.Append("              , Data_Modifica =  " & Agro_SQL_SaveDate(Data_modifica) & vbCrLf)
            Stb.Append("              , UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'" & vbCrLf)

            Stb.Append(" FROM ws_SQNPI_Domanda DD " & vbCrLf)
            Stb.Append(" INNER JOIN  ws_SQNPI_Domanda_Soggetto_Programmazione_Testata DDP " & vbCrLf)
            Stb.Append("    ON DD.IdentificativoFlusso = DDP.IdentificativoFlusso " & vbCrLf)

            Stb.Append(" INNER JOIN  Programmazione_Testata TES " & vbCrLf)
            Stb.Append("    ON TES.Programmazione_Cod = DDP.Programmazione_Cod " & vbCrLf)
            Stb.Append("    AND TES.Piva_SuperUser = DDP.Piva_SuperUser " & vbCrLf)

            Stb.Append(" where DD.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'")


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
    Public Function AvanzamentoDiStato_Planning( _
                    ByVal Programmazione_Cod As Integer, _
                    ByVal Stato_Destianzione As Integer, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" UPDATE TES SET " & vbCrLf)

            Stb.Append("                Stato_SQNPI =  " & Agro_SQL_SaveNum(Stato_Destianzione) & " " & vbCrLf)
            Stb.Append("              , Data_Modifica =  " & Agro_SQL_SaveDate(Data_modifica) & vbCrLf)
            Stb.Append("              , UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'" & vbCrLf)

            Stb.Append(" FROM Programmazione_Testata TES " & vbCrLf)

            Stb.Append(" where TES.Programmazione_Cod  = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")


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
    Public Function MemorizzaPlanning( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal Programmazione_Cod As Integer, _
                    ByVal Filtro_Veg_cod As String, _
                    ByVal Filtro_Macro_Uso As String, _
                    ByVal Filtro_id_cod As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" insert dbo.ws_SQNPI_Domanda_Soggetto_Catasto_Terreno " & vbCrLf)
            Stb.Append(" Select distinct " & vbCrLf)
            Stb.Append("      PP.Prov " & vbCrLf)
            Stb.Append("    , PP.Com " & vbCrLf)
            Stb.Append("    , PP.Sezione " & vbCrLf)
            Stb.Append("    , PP.Foglio " & vbCrLf)
            Stb.Append("    , PP.Numero " & vbCrLf)
            Stb.Append("    , PP.Subalterno  " & vbCrLf)
            Stb.Append("    , AG.Veg_Cod_Agea " & vbCrLf)
            Stb.Append("    , AG.Cul_Cod_Agea " & vbCrLf)
            Stb.Append("    , '" & Agro_SQL_SaveText(IdentificativoFlusso) & "' as IdentificativoFlusso " & vbCrLf)
            Stb.Append("    , ic.val_cod as soggetto_Cuaa " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    , max(E.Resa * 1000) as Resa " & vbCrLf)
            Stb.Append("    , sum(E.Superficie * 10000) as SUP_INVESTITA_CENTIARE " & vbCrLf)
            Stb.Append("    , mSqnpi.Macrouso_SQNPI as Macrouso_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append("    , 0  " + vbCrLf)
            Stb.Append("    , Null  " + vbCrLf)

            Stb.Append("	, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            Stb.Append(" from Programmazione_Testata T " & vbCrLf)
            Stb.Append("    inner join Programmazione_Entita E " & vbCrLf)
            Stb.Append("        on T.Piva_SuperUser = E.Piva_SuperUser " & vbCrLf)
            Stb.Append("        and T.Programmazione_Cod = E.Programmazione_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join Programmazione_Particelle P " & vbCrLf)
            Stb.Append("        on E.Piva_SuperUser = P.Piva_SuperUser  " & vbCrLf)
            Stb.Append("        and E.Programmazione_Entita_Cod = P.Programmazione_Entita_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("inner join ( " & vbCrLf)
            Stb.Append("        select   " & vbCrLf)
            Stb.Append("              M.prov " & vbCrLf)
            Stb.Append("            , M.com " & vbCrLf)
            Stb.Append("            , M.Sezione " & vbCrLf)
            Stb.Append("            , M.Foglio " & vbCrLf)
            Stb.Append("            , M.Numero " & vbCrLf)
            Stb.Append("            , M.Subalterno " & vbCrLf)
            Stb.Append("            , max(M.Macrouso_Cod) as Macrouso_Cod " & vbCrLf)
            Stb.Append("        from ParticelleCatastalixMacrousi M " & vbCrLf)
            Stb.Append("        where M.macrouso_Cod in ( " & vbCrLf)
            Stb.Append("         " & Filtro_Macro_Uso & vbCrLf)
            Stb.Append("        ) " & vbCrLf)
            Stb.Append("        group by  " & vbCrLf)
            Stb.Append("              M.prov " & vbCrLf)
            Stb.Append("            , M.com " & vbCrLf)
            Stb.Append("            , M.Sezione " & vbCrLf)
            Stb.Append("            , M.Foglio " & vbCrLf)
            Stb.Append("            , M.Numero " & vbCrLf)
            Stb.Append("            , M.Subalterno " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    ) PP " & vbCrLf)
            Stb.Append("        on PP.prov = P.Prov " & vbCrLf)
            Stb.Append("        and PP.COM = P.com " & vbCrLf)
            Stb.Append("        and PP.SEZIONE = P.Sezione " & vbCrLf)
            Stb.Append("        and PP.Foglio = P.Foglio " & vbCrLf)
            Stb.Append("        and PP.Numero = P.Numero " & vbCrLf)
            Stb.Append("        and PP.SUBALTERNO = P.Subalterno " & vbCrLf)
            Stb.Append(" ")

            Stb.Append("    inner join imprese_codici ic " & vbCrLf)
            Stb.Append("        on ic.piva = T.Piva  " & vbCrLf)
            Stb.Append("        and ic.id_cod = 1010  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  inner join MacroUsi_xMacroUsi_SQNPI mSqnpi " & vbCrLf)
            Stb.Append("    on mSqnpi.Macrouso_cod = PP.Macrouso_cod " & vbCrLf)

            
            Stb.Append(" inner join ( " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    select   " & vbCrLf)
            Stb.Append("          Veg_cod " & vbCrLf)
            Stb.Append("        , Cul_cod " & vbCrLf)
            Stb.Append("        , Grfi_cod " & vbCrLf)
            Stb.Append("        , Grva_cod " & vbCrLf)
            Stb.Append("        , Metodo_Produzione_cod " & vbCrLf)
            Stb.Append("        , Reg_cod " & vbCrLf)
            Stb.Append("        , Id_cod " & vbCrLf)
            Stb.Append("        , Max(Veg_Cod_Agea + '-' + Cul_Cod_Agea) as CodiceSelezionato " & vbCrLf)
            Stb.Append("    from Codifica_SpecieVegetali_Agea a  " & vbCrLf)
            Stb.Append("    group by  " & vbCrLf)
            Stb.Append("          Veg_cod " & vbCrLf)
            Stb.Append("        , Cul_cod " & vbCrLf)
            Stb.Append("        , Grfi_cod " & vbCrLf)
            Stb.Append("        , Grva_cod " & vbCrLf)
            Stb.Append("        , Metodo_Produzione_cod " & vbCrLf)
            Stb.Append("        , Reg_cod " & vbCrLf)
            Stb.Append("        , Id_cod " & vbCrLf)
            Stb.Append(" ) A11 " & vbCrLf)
            Stb.Append("        on A11.Veg_cod = E.Veg_Cod " & vbCrLf)
            Stb.Append("        and A11.Cul_cod = E.Cul_Cod  " & vbCrLf)
            Stb.Append("        and A11.Grfi_cod = E.Grfi_Cod " & vbCrLf)
            Stb.Append("        and A11.Grva_cod = E.Grva_Cod " & vbCrLf)
            Stb.Append("        and A11.Id_cod = E.id_cod " & vbCrLf)
            Stb.Append("        and A11.Metodo_Produzione_cod = E.MetodoProduzione_Cod " & vbCrLf)

            Stb.Append(" inner join Codifica_SpecieVegetali_Agea AG " & vbCrLf)
            Stb.Append("    on LEFT(A11.CodiceSelezionato, 3) = AG.Veg_Cod_Agea " & vbCrLf)
            Stb.Append("    and RIGHT(A11.CodiceSelezionato, 3) = AG.Cul_Cod_Agea " & vbCrLf)

            Stb.Append(" ")

            Stb.Append("  " & vbCrLf)
            Stb.Append(" where T.Programmazione_Cod =  " & Programmazione_Cod & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" and E.Resa > 0 " & vbCrLf)
            Stb.Append(" and (  " & vbCrLf)
            Stb.Append("        ( " & vbCrLf)
            Stb.Append("            E.Veg_Cod <> 0 and E.Grfi_Cod = 2 and E.Veg_Cod in ( " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & Filtro_Veg_cod & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("        ) " & vbCrLf)
            Stb.Append("    ) or (  " & vbCrLf)
            Stb.Append("            E.veg_Cod = 0 and E.Id_Cod in ( " & vbCrLf)
            Stb.Append("  " & Filtro_id_cod & vbCrLf)
            Stb.Append("        ) " & vbCrLf)
            Stb.Append("    ) " & vbCrLf)
            Stb.Append(" )")


            Stb.Append(" group by " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("      PP.Prov " & vbCrLf)
            Stb.Append("    , PP.Com " & vbCrLf)
            Stb.Append("    , PP.Sezione " & vbCrLf)
            Stb.Append("    , PP.Foglio " & vbCrLf)
            Stb.Append("    , PP.Numero " & vbCrLf)
            Stb.Append("    , PP.Subalterno  " & vbCrLf)
            Stb.Append("    , AG.Veg_Cod_Agea " & vbCrLf)
            Stb.Append("    , AG.Cul_Cod_Agea " & vbCrLf)
            Stb.Append("    , ic.val_cod " & vbCrLf)
            Stb.Append("    , mSqnpi.Macrouso_SQNPI")

            Stb.Append(" ")


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
    Public Function Memorizza_Domanda_Soggetto_Programmazione_Testata( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal Programmazione_Cod As Integer, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" insert dbo.ws_SQNPI_Domanda_Soggetto_Programmazione_Testata " & vbCrLf)
            Stb.Append(" Select  " & vbCrLf)

            Stb.Append("      T.Piva_SuperUser " & vbCrLf)
            Stb.Append("    , " & Programmazione_Cod & vbCrLf)
            Stb.Append("    , '" & Agro_SQL_SaveText(IdentificativoFlusso) & "' as IdentificativoFlusso " & vbCrLf)
            Stb.Append("    , ic.val_cod as soggetto_Cuaa " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    , 0  " + vbCrLf)
            Stb.Append("    , Null  " + vbCrLf)

            Stb.Append("	, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            Stb.Append(" from Programmazione_Testata T " & vbCrLf)

            Stb.Append("    inner join imprese_codici ic " & vbCrLf)
            Stb.Append("        on ic.piva = T.Piva  " & vbCrLf)
            Stb.Append("        and ic.id_cod = 1010  " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append(" where T.Programmazione_Cod =  " & Programmazione_Cod & vbCrLf)
            Stb.Append("  " & vbCrLf)


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
    Public Function MemorizzaSoggetto( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal Programmazione_Cod As Integer, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" insert dbo.ws_SQNPI_Domanda_Soggetto " & vbCrLf)
            Stb.Append(" Select top 1  " & vbCrLf)

            Stb.Append("      '" & Agro_SQL_SaveText(IdentificativoFlusso) & "' as IdentificativoFlusso " & vbCrLf)
            Stb.Append("    , ic.val_cod as soggetto_Cuaa " & vbCrLf)

            Stb.Append("    , 0  " + vbCrLf)
            Stb.Append("    , Null  " + vbCrLf)

            Stb.Append("	, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            Stb.Append(" from Programmazione_Testata T " & vbCrLf)

            Stb.Append("    inner join imprese_codici ic " & vbCrLf)
            Stb.Append("        on ic.piva = T.Piva  " & vbCrLf)
            Stb.Append("        and ic.id_cod = 1010  " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append(" where T.Programmazione_Cod =   " & Programmazione_Cod & vbCrLf)
            Stb.Append("  " & vbCrLf)


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
    Public Function RimuoviSoggetto( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal SoggettoCUAA As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" delete Sog from  dbo.ws_SQNPI_Domanda_Soggetto SOG " & vbCrLf)
            Stb.Append(" where Sog.SoggettoCUAA = '" & Agro_SQL_SaveText(SoggettoCUAA) & "'" & vbCrLf)
            Stb.Append(" And Sog.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)


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
    Public Function RimuoviSoggettoTerra( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal SoggettoCUAA As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" delete Sog from  dbo.ws_SQNPI_Domanda_Soggetto_Catasto_Terreno SOG " & vbCrLf)
            Stb.Append(" where Sog.SoggettoCUAA = '" & Agro_SQL_SaveText(SoggettoCUAA) & "'" & vbCrLf)
            Stb.Append(" And Sog.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)


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
    Public Function RimuoviSoggettoPlan( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal SoggettoCUAA As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" delete Sog from  dbo.ws_SQNPI_Domanda_Soggetto_Programmazione_Testata SOG " & vbCrLf)
            Stb.Append(" where Sog.SoggettoCUAA = '" & Agro_SQL_SaveText(SoggettoCUAA) & "'" & vbCrLf)
            Stb.Append(" And Sog.IdentificativoFlusso = '" & Agro_SQL_SaveText(IdentificativoFlusso) & "'" & vbCrLf)


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
    Public Function MemorizzaDomanda( _
                    ByVal IdentificativoFlusso As String, _
                    ByVal CuaaOP As String, _
                    ByVal skOdc As String, _
                    ByVal DataAdesione As DateTime, _
                    ByVal DestinatariResponso As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" insert dbo.ws_SQNPI_Domanda (IdentificativoFlusso, DataAdesione, skOdc, CuaaOP, skTipoDomanda, skTipoScopo, DestinatariResponso, Stato_Gias, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) " & vbCrLf)
            Stb.Append(" Select top 1  " & vbCrLf)

            Stb.Append("      '" & Agro_SQL_SaveText(IdentificativoFlusso) & "' as IdentificativoFlusso " & vbCrLf)

            Stb.Append("    ,  " & Agro_SQL_SaveDate(DataAdesione) & vbCrLf)
            Stb.Append("    ,  " & Agro_SQL_SaveNum(skOdc) & vbCrLf)
            Stb.Append("    ,  '" & Agro_SQL_SaveText(CuaaOP) & "'" & vbCrLf)
            Stb.Append("    ,  " & Agro_SQL_SaveNum(1) & vbCrLf)
            Stb.Append("    ,  " & Agro_SQL_SaveNum(1) & vbCrLf)
            Stb.Append("    ,  '" & Agro_SQL_SaveText(DestinatariResponso) & "'" & vbCrLf)


            Stb.Append("    ,  " & Agro_SQL_SaveNum(enum_WWorflow_WAnagraficaStati.Sistema_Qualità_Nazionale_Produzione_Integrata_Pratica_Validata) & vbCrLf)

            Stb.Append("    , 0  " + vbCrLf)
            Stb.Append("    , Null  " + vbCrLf)

            Stb.Append("	, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("	,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("	, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")



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




    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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




End Class


