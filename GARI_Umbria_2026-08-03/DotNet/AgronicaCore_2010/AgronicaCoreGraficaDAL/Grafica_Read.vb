Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Grafica_Read
    Inherits AgronicaCoreDataProvider.DataProvider


    Private Shared Function GetBasecod(ByVal Sa_cod As Integer) As Integer

        Return (Sa_cod \ (2 ^ 17)) * (2 ^ 17)

    End Function


    '#############################################################################################
    Public Function LeggiPerSincronizzazioneGIS2012( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Sezione As String, _
                            ByVal Id As String, _
                            ByVal Layers As String, _
                            ByVal tipoOperazioneDB As Integer, _
                                ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Const nomeRoutine = "AgronicaCoreGraficaDAL.Grafica_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    stb.Length = 0
                    stb.AppendLine(" SET NOCOUNT ON ")
                    stb.AppendLine("  ")
                    stb.AppendLine(" Declare @baseCod integer ")
                    stb.AppendLine(" Select top 1 @baseCod = cast(Sa_cod / (2 ^ 17) As int) * (2 ^ 17) ")
                    stb.AppendLine(" from Grafica  ")
                    stb.AppendLine(" where Sa_Cod Is Not null  ")
                    stb.AppendLine(" And Sa_Cod <> 0 ")
                    stb.AppendLine("  ")
                    stb.AppendLine(" Set  @baseCod  = ISNULL(@baseCod, 0)")

                    LeggiPerSincronizzazioneGIS2012_GetQuery(PIVA, Sa_Cod, Sezione, Id, Layers, tipoOperazioneDB, xFiltroAggiuntivo, xOrderBy, objParametri, stb, False)

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta




            End Select


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Shared Sub LeggiLayerImportGraficaXML_getQueryUtente2(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef stb As StringBuilder)
        If objParametri_server.UtenteCodFiscale <> objParametri_server.PivaSuperUser Then
            stb.AppendLine("  and ( Grafica.Username_Modifica in ( '" & objParametri_server.UtenteUsername & "', '" & objParametri_server.UtenteCodFiscale & "') ")
            stb.AppendLine(" or ( Grafica.Username_Modifica in ( '" & objParametri_server.SuperUserUsername & "', '" & objParametri_server.PivaSuperUser & "') AND imp1.Username_Modifica in ( '" & objParametri_server.UtenteUsername & "', '" & objParametri_server.UtenteCodFiscale & "') ) ")
            stb.AppendLine(" ) ")
        End If
    End Sub


    Public Sub LeggiPerSincronizzazioneGIS2012_GetQuery(ByVal PIVA As String, ByVal Sa_Cod As Integer, ByVal Sezione As String, ByVal Id As String, ByVal Layers As String, ByVal tipoOperazioneDB As Integer, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal Stb As StringBuilder, ByVal LeggiPerJoinSoloPivaSa_cod As Boolean)
        '---------------------------------------------


        If LeggiPerJoinSoloPivaSa_cod Then
            Stb.Append(" SELECT Distinct Grafica.Piva, Grafica.sa_cod")
        Else
            Stb.Append(" SELECT TipoImport.elementografico_cod, TipoImport.entita_cod, TipoImport.tipoOperazioneDB, Grafica.*, pp.*, coalesce(cam.Analisi_Campione_Cod, 0) as Analisi_Campione_Cod, coalesce(mov_dest.id_agenda, 0) as id_agenda, coalesce(ric_dest.ricetta_operazione_cod, 0) as ricetta_operazione_cod")
        End If


        Stb.Append(" FROM  Grafica " & vbCrLf)

        Stb.AppendLine("    Left Join Reg_Impianti imp1 ")
        Stb.AppendLine("        On imp1.PIVA = Grafica.Piva  ")
        Stb.AppendLine("        And imp1.SA_COD = Grafica.Sa_Cod ")
        Stb.AppendLine("        And substring(Grafica.Id, 1, 1) = 'I' ")
        Stb.AppendLine("        And imp1.APPEZZA = @baseCod -1 + CONVERT(BIGINT,CONVERT(varbinary(2), master.dbo.fn_cdc_hexstrtobin( substring(Grafica.id, 2,4)))) ")
        Stb.AppendLine("        And imp1.ID_REG = @baseCod -1 + CONVERT(BIGINT,CONVERT(varbinary(2), master.dbo.fn_cdc_hexstrtobin( substring(Grafica.id, 6,4))))")


        Stb.AppendLine("    Left Join( ")
        Stb.AppendLine("         select piva, Sa_Cod, Id_Agenda, MAX(mov_destinazioni_graphickey) as mov_destinazioni_graphickey  ")
        Stb.AppendLine("         From Mov_Destinazioni d  ")
        Stb.AppendLine("         Group By PIVA, Sa_Cod, Id_Agenda  ")
        Stb.AppendLine("     ) mov_dest  ")
        Stb.Append("    on mov_dest.Piva = Grafica.Piva  ")
        Stb.Append("    and mov_dest.sa_cod = grafica.sa_cod ")
        Stb.Append("    and mov_dest.mov_destinazioni_graphickey = Grafica.Id  ")

        Stb.Append("left join ( " & vbCrLf)

        Stb.Append(" select  piva, coalesce(sa_cod, 0 ) as sa_cod , op.ricetta_Operazione_cod, MAX(d.Ricette_Dettaglio_Tecnico_graphickey ) as Ricette_Dettaglio_Tecnico_graphickey  " & vbCrLf)
        Stb.Append(" from ricette r  " & vbCrLf)
        Stb.Append("    inner join Ricette_Operazioni op " & vbCrLf)
        Stb.Append("        on r.Ricetta_Cod = op.Ricetta_Cod " & vbCrLf)
        Stb.Append("    inner join Ricette_Dettaglio_Tecnico d  " & vbCrLf)
        Stb.Append("        on d.Ricetta_Cod = op.Ricetta_Cod " & vbCrLf)
        Stb.Append("        and d.Ricetta_Operazione_Cod = op.Ricetta_Operazione_Cod " & vbCrLf)
        Stb.Append(" group by piva, sa_cod, op.ricetta_Operazione_cod  " & vbCrLf)
        Stb.Append(" ")

        Stb.Append(") ric_dest  " & vbCrLf)

        Stb.Append("on ric_dest.Piva = Grafica.Piva  ")
        Stb.Append("and ric_dest.sa_cod = grafica.sa_cod ")
        Stb.Append("and ric_dest.Ricette_Dettaglio_Tecnico_graphickey = Grafica.Id  ")

        Stb.Append("left join analisi_campioni cam " & vbCrLf)
        Stb.Append(" on  cam.Analisi_Campione_Key_Piva = Grafica.Piva  " & vbCrLf)
        Stb.Append("   and Analisi_Campione_Key_saCOD = Grafica.Sa_Cod  " & vbCrLf)
        Stb.Append("   and Analisi_Campione_Key_IDGrafica = Grafica.Id  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" left join ParticelleCatastali pp on      " & vbCrLf)
        Stb.Append("    CONVERT(BIGINT,CONVERT(varbinary(4), master.dbo.fn_cdc_hexstrtobin( substring(Grafica.id, 2,8)))) = pp.Part_cod " & vbCrLf)

        Stb.Append("  inner join  ( " & vbCrLf)


        If tipoOperazioneDB = 1 Then
            Stb.Append("  select distinct  1 as tipoOperazioneDB, oldG.piva, oldG.sa_cod, oldG.ID, 0 as Entita_Cod, 0 as elementografico_cod" & vbCrLf)
            Stb.Append("  from Grafica oldG   " & vbCrLf)
            Stb.Append("     left join (  " & vbCrLf)
            Stb.Append("     select piva, sa_cod, Entita_Cod, OLDGrafica_ID  " & vbCrLf)
            Stb.Append("     from GIS_Entita  " & vbCrLf)
            Stb.Append("     where OLDGrafica_ID  is not null " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    union " & vbCrLf)
            Stb.Append("    select piva, sa_cod, -1 as Entita_Cod, OLDGrafica_ID " & vbCrLf)
            Stb.Append("    from GIS_ElementiGrafici_LogErrori err                   " & vbCrLf)
            Stb.Append("     ) ee    " & vbCrLf)
            Stb.Append("     on substring(oldG.Id,2,8) = substring(ee.OLDGrafica_ID  ,2,8)   " & vbCrLf)
            Stb.Append("     and oldG.piva = ee.piva " & vbCrLf)
            Stb.Append("     and oldG.sa_cod = ee.sa_cod   " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append("  where ee.Entita_Cod is null  " & vbCrLf)
            If Not LeggiPerJoinSoloPivaSa_cod Then
                Stb.Append("  and substring(oldG.id,1,1) in (" & Agro_SQL_Save_Clausola_IN(Layers, True) & ")  " & vbCrLf)
            End If


        Else
            Stb.Append(" select distinct 2 as tipoOperazioneDB, ee.piva, ee.sa_cod, oldG.ID, ee.Entita_Cod , ee.elementografico_cod" & vbCrLf)
            Stb.Append(" from Grafica oldG  " & vbCrLf)
            Stb.Append("    inner join ( " & vbCrLf)
            Stb.Append("    select e.piva, e.sa_cod, e.Entita_Cod, e.OLDGrafica_ID, e.data_modifica, g.elementografico_cod  " & vbCrLf)
            Stb.Append("    from GIS_Entita e inner join gis_elementiGrafici g on e.entita_cod = g.entita_cod " & vbCrLf)
            Stb.Append("    where OLDGrafica_ID  is not null) ee  " & vbCrLf)
            Stb.Append("    on substring(oldG.Id,2,8) = substring(ee.OLDGrafica_ID  ,2,8)  " & vbCrLf)
            Stb.Append("    and oldG.piva = ee.piva " & vbCrLf)
            Stb.Append("    and oldG.sa_cod = ee.sa_cod   " & vbCrLf)
            If Not LeggiPerJoinSoloPivaSa_cod Then
                Stb.Append("    and substring(oldG.id,1,1) in (" & Agro_SQL_Save_Clausola_IN(Layers, True) & ")  " & vbCrLf)
            End If

            Stb.Append(" where oldG.data_modifica>ee.data_modifica " & vbCrLf)
        End If

        Stb.Append(" ) TipoImport " & vbCrLf)
        Stb.Append("    on TipoImport.Id = Grafica.Id  " & vbCrLf)
        Stb.Append("    and TipoImport.piva = Grafica.Piva  " & vbCrLf)
        Stb.Append("    and TipoImport.sa_Cod = grafica.sa_cod  " & vbCrLf)



        Stb.Append("  " & vbCrLf)

        ' VAnni: 2/10/2018: a prescindere dalla data di validità la lettura degli elementi da importare devono coprire tutto, 
        'onde evitare di non vedere dati che sarebbero da importare ..

        Stb.Append(" WHERE 1=1 ")
        'Stb.Append(" WHERE Grafica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
        'Stb.Append(" AND   Grafica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

        LeggiLayerImportGraficaXML_getQueryUtente2(objParametri, Stb)


        Stb.Append(" AND   not (grafica.vx1 is null and grafica.vy1 is null and grafica.vx2 is null and grafica.vy2 is null) " & vbCrLf)

        If PIVA <> "" Then

            Stb.Append(" AND  Grafica.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")

        End If

        If Sa_Cod <> 0 Then

            Stb.Append(" AND  Grafica.Sa_Cod = " & Sa_Cod)

        End If

        If Sezione <> "" Then

            Stb.Append(" AND  Grafica.[Section] = '" & Agro_SQL_SaveText(Sezione) & "'")

        End If

        If Id <> "" Then

            Stb.Append(" AND  Grafica.Id = '" & Agro_SQL_SaveText(Id) & "'")

        End If

        If xFiltroAggiuntivo <> "" Then
            Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If
        '--------------------------------------------------------------------------
        Select Case objParametri.FlagVisibilita
            Case enumVisibilita.Visibilita_SoloNonCancellati
                Stb.Append(" AND   Grafica.Inviato >=0 ")
            Case enumVisibilita.Visibilita_SoloCancellati
                Stb.Append(" AND   Grafica.Inviato =-1 ")
            Case enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select
        '--------------------------------------------------------------------------
        If xOrderBy <> "" Then
            Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        End If
    End Sub
    '#############################################################################################
    Public Function LeggiConParticelle( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Sezione As String, _
                            ByVal Id As String, _
                                ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreGraficaDAL.Grafica_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Grafica.*, pp.* ")
                    StrSQL.Append(" FROM  Grafica ")
                    StrSQL.Append(" left join ParticelleCatastali pp on      " & vbCrLf)
                    StrSQL.Append("    CONVERT(BIGINT,CONVERT(varbinary(4), master.dbo.fn_cdc_hexstrtobin( substring(Grafica.id, 2,8)))) = pp.Part_cod " & vbCrLf)



                    StrSQL.Append(" WHERE Grafica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Grafica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND  Grafica.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Grafica.Sa_Cod = " & Sa_Cod)
                    End If

                    If Sezione <> "" Then
                        StrSQL.Append(" AND  Grafica.[Section] = '" & Agro_SQL_SaveText(Sezione) & "'")
                    End If

                    If Id <> "" Then
                        StrSQL.Append(" AND  Grafica.Id = '" & Agro_SQL_SaveText(Id) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Grafica.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Grafica.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni




                Case enumSelezioneVariabile.Selezione_JoinCompleta




            End Select


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


    '#############################################################################################
    Public Function Leggi(ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Sezione As String, _
                            ByVal Id As String, _
                                ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Const nomeRoutine = "AgronicaCoreGraficaDAL.Grafica_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Grafica.* ")
                    StrSQL.Append(" FROM  Grafica ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then

                        StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "'")

                    End If

                    If Sa_Cod <> 0 Then

                        StrSQL.Append(" AND  Sa_Cod = " & Sa_Cod)

                    End If

                    If Sezione <> "" Then

                        StrSQL.Append(" AND  [Section] = '" & Agro_SQL_SaveText(Sezione) & "'")

                    End If

                    If Id <> "" Then

                        StrSQL.Append(" AND  Id = '" & Agro_SQL_SaveText(Id) & "'")

                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Grafica.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Grafica.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni




                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select


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
