Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class Analisi_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Leggi( _
                            ByVal Analisi_Testata_Cod As Integer, _
                            ByVal Analisi_Dettaglio_Cod As Integer, _
                            ByVal Analisi_Parametro_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_SuperUser,Analisi_Testata_Cod,Analisi_Dettaglio_Cod,Analisi_Parametro_Cod, ")
                    StrSQL.Append(" Analisi_Dettaglio_Valore_1,Analisi_Dettaglio_MargineErrore_1,Analisi_Dettaglio_Valore_2,Analisi_Dettaglio_MargineErrore_2 " & _
                                    " FROM   Analisi_Dettagli " & _
                                    " WHERE  Analisi_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                                    " AND    Analisi_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
                                    " AND    Analisi_Dettagli.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
                    End If

                    If Analisi_Parametro_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_Dettagli.* " & _
                    " FROM   Analisi_Dettagli " & _
                    " WHERE  Analisi_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                    " AND    Analisi_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
                    " AND    Analisi_Dettagli.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
                    End If

                    If Analisi_Parametro_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_Dettagli.*, Analisi_Testata_Data_Inizio, Analisi_Testata_Data_Fine, ")
                    StrSQL.Append(" Analisi_Parametro_Des,Analisi_Parametro_Simbolo,Analisi_Parametro_UdM,Analisi_Parametro_ValoreMin,Analisi_Parametro_ValoreMax,Analisi_Parametro_TipoAnalisi ")
                    StrSQL.Append(" FROM   Analisi_Dettagli ")
                    StrSQL.Append(" INNER JOIN   Analisi_Testata ON Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod AND Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser")
                    StrSQL.Append(" INNER JOIN   Analisi_Parametri ON Analisi_Parametri.Analisi_Parametro_Cod = Analisi_Dettagli.Analisi_Parametro_Cod ")
                    StrSQL.Append(" WHERE  Analisi_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_Dettagli.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
                    End If

                    If Analisi_Parametro_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_Dettagli.*, udm_des, udm_sim, ")
                    StrSQL.Append(" Analisi_Parametro_Des,Analisi_Parametro_Simbolo,Analisi_Parametro_UdM,Analisi_Parametro_ValoreMin,Analisi_Parametro_ValoreMax,Analisi_Parametro_TipoAnalisi ")
                    StrSQL.Append(" FROM   Analisi_Dettagli ")
                    StrSQL.Append(" INNER JOIN   Analisi_Parametri ON Analisi_Parametri.Analisi_Parametro_Cod = Analisi_Dettagli.Analisi_Parametro_Cod ")
                    StrSQL.Append(" INNER JOIN   UnitaMisura ON Analisi_Parametri.Analisi_Parametro_UdM = UnitaMisura.UDM_COD ")
                    StrSQL.Append(" WHERE  Analisi_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_Dettagli.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
                    End If

                    If Analisi_Parametro_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Dettagli.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If



            End Select
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


    ' data un'analisi e un parametro non valorizzato, seleziona il valore di quel parametro di un'analisi precedente
    '============================================================================
    Public Function Leggi_ValoreParametro_AnalisiPrecedente( _
                                            ByVal Analisi_Testata_Cod As Integer, _
                                            ByVal Analisi_Parametro_Cod As Integer, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As Decimal



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.Leggi_ValoreParametro_AnalisiPrecedente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dblRet As Decimal
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            'StrSQL.Append(" SELECT TOP 1 ANALISI_DETTAGLIO_VALORE_1 ")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM [Analisi_Dettagli] ")

            StrSQL.Append(" INNER JOIN Analisi_Testata ON [Analisi_Dettagli].Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
            StrSQL.Append(" INNER JOIN Analisi_EntitaxTestata ON Analisi_Testata.Analisi_Testata_Cod = Analisi_EntitaxTestata.Analisi_Testata_Cod ")
            StrSQL.Append(" WHERE Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(Analisi_Parametro_Cod) & " ")
            StrSQL.Append(" AND EXISTS (SELECT * FROM Analisi_EntitaxTestata AET2  ")
            StrSQL.Append(" WHERE AET2.Piva = Analisi_EntitaxTestata.Piva ")
            StrSQL.Append(" AND AET2.Sa_Cod = Analisi_EntitaxTestata.Sa_Cod ")
            StrSQL.Append(" AND AET2.Campo_Cod = Analisi_EntitaxTestata.Campo_Cod ")
            StrSQL.Append(" AND AET2.Appezza = Analisi_EntitaxTestata.Appezza ")
            StrSQL.Append(" AND AET2.Id_Imp = Analisi_EntitaxTestata.Id_Imp ")
            StrSQL.Append(" AND AET2.Analisi_testata_cod  = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " )")

            StrSQL.Append(" AND Analisi_Dettaglio_Valore_1<>0 AND Analisi_Dettaglio_Valore_1 IS NOT NULL ")

            ' analisi precedente dell'analisi data
            StrSQL.Append(" AND Analisi_Testata.Analisi_Testata_Data_Inizio<= (SELECT Analisi_Testata_Data_Inizio FROM ")
            StrSQL.Append(" Analisi_Testata WHERE Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " )")


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Analisi_Dettagli.Inviato >=0 ")
                    StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                    StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Analisi_Dettagli.Inviato =-1 ")
                    StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
                    StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Analisi_Testata.Analisi_Testata_Data_Inizio DESC ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 AndAlso Not IsDBNull(DT.Rows(0).Item("ANALISI_DETTAGLIO_VALORE_1")) Then
                dblRet = DT.Rows(0).Item("ANALISI_DETTAGLIO_VALORE_1")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dblRet = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dblRet

    End Function



    '============================================================================
    Public Function Leggi_x_Filtrone_Magico_PDC( _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Analisi_Dettagli.analisi_testata_cod,  Analisi_Dettagli.Analisi_Parametro_Cod, Analisi_Dettagli.Analisi_Dettaglio_Valore_1 ,isnull( isnull(pa_des,descrizione),analisi_parametro_des) as des ,udm_sim")

            StrSQL.AppendLine(" FROM PDC_Analisi ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Dettagli ON Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN PrincipiAttivi ON Analisi_Parametro_Cod > 0 AND Analisi_Parametro_Cod = pa_cod AND analisi_testata_tipo = 8 ")
            StrSQL.AppendLine(" LEFT JOIN (SELECT * FROM FamigliePrincipiAttivi WHERE fam_cod NOT LIKE '%clas%') a ON Analisi_Parametro_Cod < 0 AND abs(Analisi_Parametro_Cod) = a.fam_cod AND analisi_testata_tipo= 8 ")
            StrSQL.AppendLine(" LEFT JOIN UnitaMisura ON udm_cod = Analisi_Dettagli.Analisi_Dettaglio_Valore_2 ")
            StrSQL.AppendLine(" LEFT JOIN Analisi_Parametri ON Analisi_Parametri.Analisi_Parametro_Cod =Analisi_Dettagli.Analisi_Parametro_Cod AND analisi_testata_tipo<> 8 ")

            StrSQL.AppendLine(" WHERE 1=1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND Analisi_Dettagli.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND Analisi_Dettagli.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Analisi_Dettagli.analisi_testata_cod ")
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

    Public Sub ValorizzaDettagliAnalisiTerreno(ByVal Analisi_Testata_Cod As Integer,
                                               ByRef Sabbia As Decimal,
                                               ByRef Argilla As Decimal,
                                               ByRef SostanzaOrganica As Decimal,
                                               ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R.ValorizzaDettagliAnalisi()"
        Dim messaggioErrore As String = ""

        Try

            Dim dt As New DataTable
            dt = Leggi(Analisi_Testata_Cod,
                       0, 0,
                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                       "Analisi_Parametro_Cod IN (3,5,9)", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Dim Parametro_Cod As Integer
                Dim Dettaglio_Cod As Integer
                Dim valore As Decimal

                For Each row In dt.Rows

                    Parametro_Cod = row.Item("analisi_parametro_cod")
                    Dettaglio_Cod = row.Item("analisi_dettaglio_cod")
                    If Not IsDBNull(row.Item("analisi_dettaglio_valore_1")) Then
                        valore = row.Item("analisi_dettaglio_valore_1")
                    Else
                        valore = 0
                    End If

                    If valore = 0 Then
                        valore = Leggi_ValoreParametro_AnalisiPrecedente(Analisi_Testata_Cod, Parametro_Cod,
                                                                         "Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Terreno,
                                                                         "", objParametri)
                    End If

                    Select Case Parametro_Cod

                        Case enum_AnalisiParametri.AnalisiParametri_Sabbia
                            Sabbia = valore

                        Case enum_AnalisiParametri.AnalisiParametri_Argilla
                            Argilla = valore

                        Case enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica
                            SostanzaOrganica = valore

                    End Select
                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Carica_DatiAnalisi_xPianoConcimazione(ByVal Analisi_Testata_Cod As Integer, ByRef Sabbia As String, ByRef Limo As String, ByRef Argilla As String,
                               ByRef Ph As String, ByRef CalcTot As String, ByRef CalcAtt As String, ByRef SO As String,
                               ByRef N As String, ByRef P2O5 As String, ByRef K2O As String,
                               ByRef CN As String, ByRef Mg As String, ByRef CSC As String,
                               ByRef P As String, ByRef K As String, ByRef Data_Inizio As Date, ByRef Data_Fine As Date,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim Parametro_Cod As Integer
        Dim Dettaglio_Cod As Integer
        Dim Valore As Decimal

        '(27/04/2020) ripristinata verifica dei parametri dell'analisi precedente se non presenti nella selezionata
        Dim HashDettagliAnalisixPiano As New Hashtable
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Sabbia, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Limo, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Argilla, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_pH, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_CaCO3, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_CSC, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Ntot, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_P_assimilabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_K_scambiabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_RapportoCN, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_CarbonatiTotali, 0)


        Dim objAnalisiD As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
        Dim DtAnalisiD As New DataTable
        DtAnalisiD = objAnalisiD.Leggi(Analisi_Testata_Cod,
                                       0, 0,
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                       "Analisi_Dettagli.Analisi_Parametro_Cod IN (3,4,5,2,6,8,7,9,12,17,18,20,23,11,84,21,85)", "", objParametri)

        If Not IsNothing(DtAnalisiD) AndAlso DtAnalisiD.Rows.Count > 0 Then

            If Not IsDBNull(DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Inizio")) Then
                Data_Inizio = DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Inizio")
            End If
            If Not IsDBNull(DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Fine")) Then
                Data_Fine = DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Fine")
            End If

            Dim i As Integer
            For i = 0 To DtAnalisiD.Rows.Count - 1

                Parametro_Cod = DtAnalisiD.Rows(i).Item("analisi_parametro_cod")
                Dettaglio_Cod = DtAnalisiD.Rows(i).Item("analisi_dettaglio_cod")
                If Not IsDBNull(DtAnalisiD.Rows(i).Item("analisi_dettaglio_valore_1")) Then
                    Valore = DtAnalisiD.Rows(i).Item("analisi_dettaglio_valore_1")
                Else
                    Valore = 0
                End If

                If Valore = 0 Then
                    Valore = objAnalisiD.Leggi_ValoreParametro_AnalisiPrecedente(Analisi_Testata_Cod, Parametro_Cod, "Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Terreno, "", objParametri)
                End If

                Select Case Parametro_Cod

                    Case enum_AnalisiParametri.AnalisiParametri_Sabbia

                        Sabbia = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Sabbia) = Sabbia

                    Case enum_AnalisiParametri.AnalisiParametri_Limo

                        Limo = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Limo) = Limo

                    Case enum_AnalisiParametri.AnalisiParametri_Argilla

                        Argilla = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Argilla) = Argilla

                    Case enum_AnalisiParametri.AnalisiParametri_pH

                        Ph = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_pH) = Ph

                    Case enum_AnalisiParametri.AnalisiParametri_CaCO3

                        CalcTot = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_CaCO3) = CalcTot

                    Case enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo

                        CalcAtt = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo) = CalcAtt

                    Case enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica

                        SO = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica) = SO

                    Case enum_AnalisiParametri.AnalisiParametri_Ntot

                        N = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Ntot) = N

                    Case enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile

                        P2O5 = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile) = P2O5

                    Case enum_AnalisiParametri.AnalisiParametri_P_assimilabile

                        P = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_P_assimilabile) = P

                    Case enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile

                        If K2O = 0 Then
                            K2O = CStr(Valore)
                            HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile) = K2O
                        End If

                    Case enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile

                        If K2O = 0 Then
                            K2O = CStr(Valore)
                            HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile) = K2O
                        End If

                    Case enum_AnalisiParametri.AnalisiParametri_K_scambiabile

                        K = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_K_scambiabile) = K

                    Case enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile

                        Mg = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile) = Mg

                    Case enum_AnalisiParametri.AnalisiParametri_CSC

                        CSC = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_CSC) = CSC

                    Case enum_AnalisiParametri.AnalisiParametri_RapportoCN

                        CN = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_RapportoCN) = CN

                    Case enum_AnalisiParametri.AnalisiParametri_CarbonatiTotali

                        If CalcTot = 0 Then
                            CalcTot = CStr(Valore)
                            HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_CarbonatiTotali) = CalcTot
                        End If

                End Select


            Next


        End If

        For Each key In HashDettagliAnalisixPiano.Keys
            If HashDettagliAnalisixPiano(key) = 0 Then
                Valore = objAnalisiD.Leggi_ValoreParametro_AnalisiPrecedente(Analisi_Testata_Cod, key, "Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Terreno, "", objParametri)
                If Valore > 0 Then
                    Select Case key
                        Case enum_AnalisiParametri.AnalisiParametri_Sabbia
                            Sabbia = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Limo
                            Limo = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Argilla
                            Argilla = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_pH
                            Ph = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_CaCO3
                            CalcTot = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo
                            CalcAtt = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica
                            SO = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Ntot
                            N = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile
                            P2O5 = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_P_assimilabile
                            P = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile
                            If K2O = 0 Then
                                K2O = CStr(Valore)
                            End If
                        Case enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile
                            If K2O = 0 Then
                                K2O = CStr(Valore)
                            End If
                        Case enum_AnalisiParametri.AnalisiParametri_K_scambiabile
                            K = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile
                            Mg = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_CSC
                            CSC = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_RapportoCN
                            CN = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_CarbonatiTotali
                            If CalcTot = 0 Then
                                CalcTot = CStr(Valore)
                            End If
                    End Select
                End If
            End If
        Next




    End Sub

    '============================================================================
#Region "Piano Nutrizionale"
    Public Sub Carica_DatiAnalisi_xPianoNutrizionale(ByVal Analisi_Testata_Cod As Integer,
                                                     ByRef Sabbia As String, ByRef Limo As String, ByRef Argilla As String,
                                                     ByRef NTOT As String, ByRef NORG As String,
                                                     ByRef P2O5 As String, ByRef K2O As String,
                                                     ByRef P As String, ByRef K As String,
                                                     ByRef Data_Inizio As Date, ByRef Data_Fine As Date,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim Parametro_Cod As Integer
        Dim Dettaglio_Cod As Integer
        Dim Valore As Decimal

        '(27/04/2020) ripristinata verifica dei parametri dell'analisi precedente se non presenti nella selezionata
        Dim HashDettagliAnalisixPiano As New Hashtable
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Sabbia, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Limo, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Argilla, 0)

        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Ntot, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_Norg, 0)

        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_P_assimilabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile, 0)

        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile, 0)
        HashDettagliAnalisixPiano.Add(enum_AnalisiParametri.AnalisiParametri_K_scambiabile, 0)


        Dim objAnalisiD As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
        Dim DtAnalisiD As New DataTable
        DtAnalisiD = objAnalisiD.Leggi(Analisi_Testata_Cod,
                                       0, 0,
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                       "Analisi_Dettagli.Analisi_Parametro_Cod IN (3,4,5,12,17,18,20,21,85,98)", "", objParametri)

        If Not IsNothing(DtAnalisiD) AndAlso DtAnalisiD.Rows.Count > 0 Then

            If Not IsDBNull(DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Inizio")) Then
                Data_Inizio = DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Inizio")
            End If
            If Not IsDBNull(DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Fine")) Then
                Data_Fine = DtAnalisiD.Rows(0).Item("Analisi_Testata_Data_Fine")
            End If

            Dim i As Integer
            For i = 0 To DtAnalisiD.Rows.Count - 1

                Parametro_Cod = DtAnalisiD.Rows(i).Item("analisi_parametro_cod")
                Dettaglio_Cod = DtAnalisiD.Rows(i).Item("analisi_dettaglio_cod")
                If Not IsDBNull(DtAnalisiD.Rows(i).Item("analisi_dettaglio_valore_1")) Then
                    Valore = DtAnalisiD.Rows(i).Item("analisi_dettaglio_valore_1")
                Else
                    Valore = 0
                End If

                If Valore = 0 Then
                    Valore = objAnalisiD.Leggi_ValoreParametro_AnalisiPrecedente(Analisi_Testata_Cod, Parametro_Cod, "Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Terreno, "", objParametri)
                End If

                Select Case Parametro_Cod
                    Case enum_AnalisiParametri.AnalisiParametri_Sabbia
                        Sabbia = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Sabbia) = Sabbia
                    Case enum_AnalisiParametri.AnalisiParametri_Limo
                        Limo = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Limo) = Limo
                    Case enum_AnalisiParametri.AnalisiParametri_Argilla
                        Argilla = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Argilla) = Argilla

                    Case enum_AnalisiParametri.AnalisiParametri_Ntot
                        NTOT = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Ntot) = NTOT
                    Case enum_AnalisiParametri.AnalisiParametri_Norg
                        NORG = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_Norg) = NORG

                    Case enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile
                        P2O5 = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile) = P2O5
                    Case enum_AnalisiParametri.AnalisiParametri_P_assimilabile
                        P = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_P_assimilabile) = P

                    Case enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile
                        If K2O = 0 Then
                            K2O = CStr(Valore)
                            HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile) = K2O
                        End If
                    Case enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile
                        If K2O = 0 Then
                            K2O = CStr(Valore)
                            HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile) = K2O
                        End If
                    Case enum_AnalisiParametri.AnalisiParametri_K_scambiabile
                        K = CStr(Valore)
                        HashDettagliAnalisixPiano(enum_AnalisiParametri.AnalisiParametri_K_scambiabile) = K
                End Select
            Next
        End If

        For Each key In HashDettagliAnalisixPiano.Keys
            If HashDettagliAnalisixPiano(key) = 0 Then
                Valore = objAnalisiD.Leggi_ValoreParametro_AnalisiPrecedente(Analisi_Testata_Cod, key, "Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Terreno, "", objParametri)
                If Valore > 0 Then
                    Select Case key
                        Case enum_AnalisiParametri.AnalisiParametri_Sabbia
                            Sabbia = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Limo
                            Limo = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Argilla
                            Argilla = CStr(Valore)

                        Case enum_AnalisiParametri.AnalisiParametri_Ntot
                            NTOT = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_Norg
                            NORG = CStr(Valore)

                        Case enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile
                            P2O5 = CStr(Valore)
                        Case enum_AnalisiParametri.AnalisiParametri_P_assimilabile
                            P = CStr(Valore)

                        Case enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile
                            If K2O = 0 Then
                                K2O = CStr(Valore)
                            End If
                        Case enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile
                            If K2O = 0 Then
                                K2O = CStr(Valore)
                            End If
                        Case enum_AnalisiParametri.AnalisiParametri_K_scambiabile
                            K = CStr(Valore)
                    End Select
                End If
            End If
        Next
    End Sub
#End Region


End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Analisi_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(ByVal Analisi_Testata_Cod As Integer,
                           ByVal Analisi_Dettaglio_Cod As Integer,
                           ByVal Analisi_Parametro_Cod As Integer,
                           ByVal Analisi_Dettaglio_Valore_1 As Decimal,
                           ByVal Analisi_Dettaglio_MargineErrore_1 As Decimal,
                           ByVal Analisi_Dettaglio_Valore_2 As Decimal,
                           ByVal Analisi_Dettaglio_MargineErrore_2 As Decimal,
                           ByVal DataLock As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Analisi_Dettagli( ")
            StrSQL.AppendLine("            Analisi_SuperUser,        Analisi_Testata_Cod,             ")
            StrSQL.AppendLine("            Analisi_Dettaglio_Cod,            Analisi_Parametro_Cod,           ")
            StrSQL.AppendLine("            Analisi_Dettaglio_Valore_1,       Analisi_Dettaglio_MargineErrore_1,      ")
            StrSQL.AppendLine("            Analisi_Dettaglio_Valore_2,       Analisi_Dettaglio_MargineErrore_2,      ")
            StrSQL.AppendLine("            Data_Agg,                          DataLock,                        ")

            StrSQL.AppendLine("            Inviato, DataInvio, ")
            StrSQL.AppendLine("            Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("            Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("            ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Dettaglio_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Parametro_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Dettaglio_Valore_1) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Dettaglio_MargineErrore_1) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Dettaglio_Valore_2) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Dettaglio_MargineErrore_2) & "  ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , NULL  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function



    ''#########################################################################
    'Public Function Scrivi2( _
    '                            ByVal Analisi_SuperUser As String, _
    '                            ByVal Analisi_Testata_Cod As Long, _
    '                            ByVal Analisi_Entita_Cod As Long, _
    '                            ByVal Piva As String, ByVal Sa_Cod As Long, _
    '                            ByVal Campo_Cod As Long, ByVal Appezza As Long, _
    '                            ByVal Id_Imp As Long, ByVal Fabbricato_Cod As Long, _
    '                            ByVal PROV As String, ByVal COM As String, _
    '                            ByVal SEZIONE As String, ByVal FOGLIO As Long, _
    '                            ByVal NUMERO As Long, ByVal SUBALTERNO As String, _
    '                            ByVal Id_Oggetto_Grafico As String, _
    '                            ByVal Vas_Cod As Long, _
    '                            ByVal DataLock As Long, _
    '                                ByVal Validita_Inizio As Date, _
    '                                ByVal Validita_Fine As Date, _
    '                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                    ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_EntitaxTest_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0


    '        StrSQL.Append("INSERT INTO Analisi_EntitaxTestata( ")
    '        StrSQL.Append("            Analisi_SuperUser,        Analisi_Testata_Cod,   ")
    '        StrSQL.Append("            Analisi_Entita_Cod,    ")
    '        StrSQL.Append("            Piva,        Sa_Cod,         Campo_Cod,      Appezza,    ")
    '        StrSQL.Append("            Id_Imp,      Fabbricato_Cod, Prov,           Com,        ")
    '        StrSQL.Append("            Sezione,     Foglio,         Numero,         Subalterno, ")
    '        StrSQL.Append("            Id_Oggetto_Grafico,          Vas_Cod,        DataLock,       Data_Agg,    ")


    '        StrSQL.Append("            Inviato, DataInvio, ")
    '        StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("            ) ")

    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Analisi_SuperUser) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Entita_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Imp) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(PROV) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(COM) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(SEZIONE) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(FOGLIO) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(NUMERO) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(SUBALTERNO) & "' ")

    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Id_Oggetto_Grafico) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Vas_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
    '        StrSQL.Append(")")
    '        '---------------------------------------------

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function



    '#########################################################################
    Public Function Modifica(ByVal Analisi_Testata_Cod As Integer,
                             ByVal Analisi_Dettaglio_Cod As Integer,
                             ByVal Analisi_Parametro_Cod As Integer,
                             ByVal Analisi_Dettaglio_Valore_1 As Decimal,
                             ByVal Analisi_Dettaglio_MargineErrore_1 As Decimal,
                             ByVal Analisi_Dettaglio_Valore_2 As Decimal,
                             ByVal Analisi_Dettaglio_MargineErrore_2 As Decimal,
                             ByVal DataLock As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Analisi_Dettagli SET ")
            StrSQL.AppendLine("     Analisi_Dettaglio_Valore_1  =  " & Agro_SQL_SaveNum(Analisi_Dettaglio_Valore_1) & " ")
            StrSQL.AppendLine("   , Analisi_Dettaglio_MargineErrore_1  =  " & Agro_SQL_SaveNum(Analisi_Dettaglio_MargineErrore_1) & " ")
            StrSQL.AppendLine("   , Analisi_Dettaglio_Valore_2  =  " & Agro_SQL_SaveNum(Analisi_Dettaglio_Valore_2) & " ")
            StrSQL.AppendLine("   , Analisi_Dettaglio_MargineErrore_2  =  " & Agro_SQL_SaveNum(Analisi_Dettaglio_MargineErrore_2) & " ")

            StrSQL.AppendLine("   , DataLock                    =  " & Agro_SQL_SaveNum(DataLock) & " ")
            StrSQL.AppendLine("   , Data_Agg                    =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , Inviato           =  0 ")
            StrSQL.AppendLine("   , DataInvio         =  Null ")

            StrSQL.AppendLine("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            StrSQL.AppendLine(" AND   Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
            StrSQL.AppendLine(" AND   Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
            
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#########################################################################
    Public Function Modifica_Correzione(ByVal Analisi_Testata_Cod As Integer,
                                        ByVal Analisi_Dettaglio_Cod As Integer,
                                        ByVal Analisi_Parametro_Cod As Integer,
                                        ByVal Analisi_Dettaglio_Correzione_1 As Decimal,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W.Modifica_Correzione()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Analisi_Dettagli SET ")
            StrSQL.AppendLine("     Analisi_Dettaglio_Correzione_1  =  " & Agro_SQL_SaveNum(Analisi_Dettaglio_Correzione_1) & " ")
            StrSQL.AppendLine("   , Data_Agg                    =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , Inviato           =  0 ")
            StrSQL.AppendLine("   , DataInvio         =  Null ")

            StrSQL.AppendLine("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
            End If

            If Analisi_Dettaglio_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function



    '#########################################################################
    Public Function Cancella(ByVal Analisi_Testata_Cod As Integer,
                             ByVal Analisi_Dettaglio_Cod As Integer,
                             ByVal Analisi_Parametro_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.AppendLine(" UPDATE  Analisi_Dettagli ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.AppendLine("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.AppendLine("         ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine("   AND Inviato >= 0 ")
                
            Else

                StrSQL.Length = 0

                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Analisi_Dettagli ")
                StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine("   AND Inviato = 0 ")
                
            End If

            StrSQL.AppendLine(" AND Analisi_Dettagli.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")


            If Analisi_Dettaglio_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Dettagli.Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
            End If

            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Dettagli.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
