Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Budget_Campi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################
    Public Function Leggi(ByVal Id_Budget As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Campo_Cod As Int32,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Budget_Campi.* ")
                    StrSQL.Append(" FROM  Budget_Campi ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Campi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND    Budget_Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Budget_Campi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND Budget_Campi.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Budget_Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Campi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Campi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Campo_Des ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append(" SELECT  Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome, Budget_Campi.* ")
                    StrSQL.Append(" FROM  Imprese INNER JOIN ")
                    StrSQL.Append(" Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.Piva  ")
                    StrSQL.Append(" INNER JOIN Budget_Campi ON Budget_Campi.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.Append(" AND Budget_Campi.sa_cod = Centri_Aziendali.sa_cod ")

                    StrSQL.Append(" WHERE Budget_Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Id_Budget <> 0 Then
                        StrSQL.Append(" AND Budget_Campi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND    Budget_Campi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Budget_Campi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND Budget_Campi.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND     Budget_Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Campi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Campi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc, Sa_Nome, Campo_Des ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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

    '##############################################################
    Public Function Leggi_x_anagrafica(ByVal Id_Budget As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Long,
                                       ByVal Campo_Cod As Long,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional leggiDatiRibaltamento As Boolean = False
                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Campi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" WITH sup_catastali as ( ")
            StrSQL.AppendLine(" 	SELECT  ID_Budget, Piva, Sa_Cod, Campo_Cod, SUM(AREA) as Sup_Catastale ")
            StrSQL.AppendLine(" 	FROM    Budget_CampiXParticelle (NOLOCK) ")
            StrSQL.AppendLine("     WHERE Budget_CampiXParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("     AND   Budget_CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            If Piva <> "" Then
                StrSQL.AppendLine("     AND   Budget_CampiXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("     AND   Budget_CampiXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine("     AND   Budget_CampiXParticelle.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.AppendLine("     AND   Budget_CampiXParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
            End If

            StrSQL.AppendLine(" 	GROUP BY ID_Budget, Piva, Sa_Cod, CAMPO_COD ")
            StrSQL.AppendLine(" ), sup_appezzamenti as ( ")
            StrSQL.AppendLine(" 	SELECT  Budget_Appezzamento.Id_Budget, ")
            StrSQL.AppendLine(" 	        Budget_Appezzamento.PIVA, ")
            StrSQL.AppendLine(" 			Budget_Appezzamento.SA_COD,  ")
            StrSQL.AppendLine(" 			Budget_Appezzamento.Campo_Cod, ")
            StrSQL.AppendLine(" 			SUM (CASE WHEN Budget_Appezzamento_Codici.val_cod = '1' OR Budget_Appezzamento_Codici.val_cod IS NULL THEN Budget_Appezzamento.sup_app ELSE 0 END) as SAU_Convenzionale, ")
            StrSQL.AppendLine(" 			SUM (CASE WHEN Budget_Appezzamento_Codici.val_cod = '2' THEN Budget_Appezzamento.sup_app ELSE 0 END) as SAU_Conversione, ")
            StrSQL.AppendLine(" 			SUM (CASE WHEN Budget_Appezzamento_Codici.val_cod = '3' THEN Budget_Appezzamento.sup_app ELSE 0 END) as SAU_Biologico, ")
            StrSQL.AppendLine(" 			SUM (Budget_Appezzamento.sup_app) AS SAU_Totale ")
            StrSQL.AppendLine(" 	FROM    Budget_Appezzamento (NOLOCK) ")
            StrSQL.AppendLine(" 	LEFT OUTER JOIN Budget_Appezzamento_Codici (NOLOCK) ON Budget_Appezzamento.PIVA = Budget_Appezzamento_Codici.PIVA   ")
            StrSQL.AppendLine(" 										AND Budget_Appezzamento.SA_COD = Budget_Appezzamento_Codici.sa_cod ")
            StrSQL.AppendLine(" 										AND Budget_Appezzamento.APPEZZA = Budget_Appezzamento_Codici.appezza ")
            StrSQL.AppendLine(" 										AND Budget_Appezzamento_Codici.id_cod = 1018 ")
            StrSQL.AppendLine("     WHERE Budget_Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("     AND   Budget_Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            If Piva <> "" Then
                StrSQL.AppendLine("     AND   Budget_Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("     AND   Budget_Appezzamento.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine("     AND   Budget_Appezzamento.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            End If

            If Id_Budget <> 0 Then
                StrSQL.AppendLine("     AND   Budget_Appezzamento.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
            End If

            StrSQL.AppendLine(" 	GROUP BY Budget_Appezzamento.Id_Budget, Budget_Appezzamento.PIVA, Budget_Appezzamento.SA_COD, Budget_Appezzamento.Campo_Cod ")
            StrSQL.AppendLine(" )  ")

            StrSQL.AppendLine(" SELECT CAST(c.Id_Budget AS nvarchar(20)) + '_' + c.Piva + '_' + CAST(c.Sa_Cod AS nvarchar(20)) + '_' + CAST(c.Campo_Cod AS nvarchar(20)) AS 'chiave', ")
            StrSQL.AppendLine(" c.Id_Budget, ")
            StrSQL.AppendLine(" c.Campo_Des AS 'Campo', c.Campo_Des, c.Campo_Cod, c.Validita_Inizio, c.Validita_Fine, c.Gru_Cod, c.Veg_Cod, ")
            StrSQL.AppendLine(" (SELECT [User] FROM utenti where CODICE_FISCALE = c.Username_Modifica) AS utente_modifica, c.Data_Modifica, ")
            StrSQL.AppendLine(" (SELECT [User] FROM utenti where CODICE_FISCALE = c.Username_Creazione) AS utente_creazione, c.Data_Creazione, ")
            StrSQL.AppendLine(" gv.Gru_Des, s.Veg_Des, ")
            StrSQL.AppendLine(" ca.sa_cod, ca.sa_nome, ")
            StrSQL.AppendLine(" ca.PIVA, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_catastali.Sup_Catastale, 0) as Decimal(10,4)) AS Superficie_Catastale, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Convenzionale, 0) as Decimal(10,4)) AS Superficie_Convenzionale, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Conversione, 0) as Decimal(10,4)) AS Superficie_Conversione, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Biologico, 0) as Decimal(10,4)) AS Superficie_Biologico, ")
            StrSQL.AppendLine("    CAST(COALESCE(sup_appezzamenti.SAU_Totale, 0) as Decimal(10,4)) AS Superficie_Totale, ")
            StrSQL.AppendLine(" CASE WHEN c.Validita_Inizio < GETDATE() AND c.validita_fine > GETDATE() THEN 1 ELSE 0 END AS Attivo ")

            If leggiDatiRibaltamento Then
                StrSQL.AppendLine(" , CASE WHEN ISNULL(rib.ID, '-1') > 0")
                StrSQL.AppendLine("   THEN 'true'")
                StrSQL.AppendLine("   ELSE 'false' END AS Ribaltato")
                StrSQL.AppendLine(" , CASE WHEN ISNULL(rib.ID, '-1') > 0")
                StrSQL.AppendLine("   THEN rib.Data_Ribaltamento")
                StrSQL.AppendLine("   ELSE '' END AS Data_Ribaltamento")
            Else
                StrSQL.AppendLine("  , 'false' AS Ribaltato")
                StrSQL.AppendLine("  , '' AS Data_Ribaltamento")
            End If

            StrSQL.AppendLine(" FROM Budget_Campi AS c  ")
            StrSQL.AppendLine(" LEFT OUTER JOIN SpecieVegetali AS s ON c.Veg_Cod = s.Veg_Cod  ")
            StrSQL.AppendLine(" LEFT OUTER JOIN GruppoVegetale AS gv ON c.Gru_Cod = gv.Gru_Cod ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Centri_Aziendali AS ca ON c.sa_cod = ca.sa_Cod AND c.piva = ca.piva ")
            StrSQL.AppendLine("     LEFT JOIN Budget_Campi_Codici (NOLOCK) rif_alfanumerico ON c.ID_Budget = rif_alfanumerico.ID_Budget AND c.Piva = rif_alfanumerico.Piva AND c.Sa_Cod = rif_alfanumerico.Sa_Cod AND c.Campo_Cod = rif_alfanumerico.Campo_Cod AND rif_alfanumerico.id_cod = 1279 ")
            StrSQL.AppendLine("     LEFT JOIN Budget_Campi_Codici (NOLOCK) sup_contratto ON c.ID_Budget = sup_contratto.ID_Budget AND c.Piva = sup_contratto.Piva  AND c.Sa_Cod = sup_contratto.Sa_Cod AND c.Campo_Cod = sup_contratto.Campo_Cod AND sup_contratto.id_cod = 1325 ")
            StrSQL.AppendLine("     LEFT JOIN Budget_Campi_Codici (NOLOCK) filiera ON c.ID_Budget = filiera.ID_Budget AND c.Piva = filiera.Piva AND c.Sa_Cod = filiera.Sa_Cod AND c.Campo_Cod = filiera.Campo_Cod AND filiera.id_cod = 1326 ")
            StrSQL.AppendLine(" 	LEFT JOIN sup_catastali ON c.Piva = sup_catastali.Piva AND c.Sa_Cod = sup_catastali.Sa_Cod AND c.Campo_Cod = sup_catastali.Campo_Cod  ")
            StrSQL.AppendLine("     LEFT JOIN sup_appezzamenti ON c.Piva = sup_appezzamenti.PIVA AND c.Sa_Cod = sup_appezzamenti.SA_COD AND c.Campo_Cod = sup_appezzamenti.Campo_Cod ")
            If leggiDatiRibaltamento Then
                StrSQL.AppendLine(" LEFT JOIN Ribaltamento_Campi rib ON c.Id_Budget = rib.Budget_Id_Testata AND c.Piva = rib.Budget_Piva AND c.Sa_Cod = rib.Budget_Sa_Cod AND c.Campo_Cod = rib.Budget_Campo_Cod")
            End If

            StrSQL.AppendLine(" WHERE c.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND c.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND c.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND c.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND c.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND c.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND c.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   c.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   c.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Campo_Des ASC")
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

    '##############################################################

    Public Function LeggiCampixParticella(ByVal Id_Budget As Integer,
                                          ByVal Piva As String,
                                          ByVal Sa_Cod As Long,
                                          ByVal Campo_Cod As Long,
                                          ByVal PROV As String,
                                          ByVal COM As String,
                                          ByVal SEZIONE As String,
                                          ByVal FOGLIO As Int32,
                                          ByVal NUMERO As Int32,
                                          ByVal SUBALTERNO As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Campi_R.LeggiCampixParticella()"

        '====================================================================================
        'Parametri opzionali :    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine(" Budget_CampixParticelle.Id_Budget, ")
            StrSQL.AppendLine(" Imprese.rag_soc, ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_nome, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.Piva, ")
            StrSQL.AppendLine(" Centri_Aziendali.sa_cod, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.PROV, ")
            StrSQL.AppendLine(" ISTAT.COMUNI_PROV, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.COM, ")
            StrSQL.AppendLine(" ISTAT.LOCALITA, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.SEZIONE, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.FOGLIO, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.NUMERO, ")
            StrSQL.AppendLine(" Budget_CampixParticelle.SUBALTERNO, ")
            StrSQL.AppendLine(" Budget_Campi.Campo_Des, ")
            StrSQL.AppendLine(" Budget_Campi.Validita_Inizio, ")
            StrSQL.AppendLine(" Budget_Campi.Validita_Fine, ")
            StrSQL.AppendLine(" Budget_CampiXParticelle.AREA, ")
            StrSQL.AppendLine(" CASE WHEN Budget_Campi.Validita_Inizio < GETDATE() AND Budget_Campi.validita_fine > GETDATE() THEN 1 ELSE 0 END as Attivo ")
            StrSQL.AppendLine(" FROM Budget_CampixParticelle ")
            StrSQL.AppendLine(" JOIN Budget_Campi ON Budget_CampixParticelle.Id_Budget = Budget_Campi.Id_Budget AND Budget_CampixParticelle.Piva = Budget_Campi.Piva AND Budget_CampixParticelle.SA_COD = Budget_Campi.Sa_Cod AND Budget_CampixParticelle.CAMPO_COD = Budget_Campi.Campo_Cod ")
            StrSQL.AppendLine(" JOIN ISTAT ON Budget_CampixParticelle.PROV = ISTAT.PROV AND Budget_CampixParticelle.Com = ISTAT.Com ")
            StrSQL.AppendLine(" JOIN Imprese ON Budget_CampixParticelle.Piva = imprese.PIVA ")
            StrSQL.AppendLine(" JOIN Centri_Aziendali ON Budget_CampixParticelle.Piva = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" 						AND	Budget_CampixParticelle.sa_cod = Centri_Aziendali.sa_cod ")


            If Id_Budget <> 0 Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "  ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Budget_Campi.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Prov = '" & Agro_SQL_SaveText(PROV) & "'  ")
            End If

            If COM <> "" Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Com = '" & Agro_SQL_SaveText(COM) & "'  ")
            End If

            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Sezione = '" & Agro_SQL_SaveText(SEZIONE) & "'  ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Foglio = " & Agro_SQL_SaveNum(FOGLIO) & "  ")
            End If

            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Numero = " & Agro_SQL_SaveNum(NUMERO) & "  ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND Budget_CampixParticelle.Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "'  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Budget_CampixParticelle.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Budget_CampixParticelle.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Budget_CampixParticelle.Campo_Cod ASC")
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

End Class

Public Class Budget_Campi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal IdBudget As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Campo_Cod As Integer,
                                       ByVal Campo_Des As String,
                                       ByVal Gru_Cod As Integer,
                                       ByVal Veg_Cod As Integer,
                                       ByVal SAU_Totale As Decimal,
                                       ByVal SAU_Biologico As Decimal,
                                       ByVal SAU_Conversione As Decimal,
                                       ByVal SAU_Convenzionale As Decimal,
                                       ByVal Conversione_Inizio As Date,
                                       ByVal Conversione_Fine As Date,
                                       ByVal ConfiniRischio As String,
                                       ByVal Campo_Tipo As Integer,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                       Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                       Optional ByVal username_creazione As String = "",
                                       Optional ByVal username_modifica As String = ""
                                       ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Campi_W.Scrivi_Budget_Campi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
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
            StrSQL.Append("INSERT INTO Budget_Campi( Id_Budget, Piva, Sa_Cod, ")
            StrSQL.Append("                         Campo_Cod,   Campo_Des, ")
            StrSQL.Append("                         Gru_Cod,  Veg_Cod, ")
            StrSQL.Append("                         SAU_Totale,  SAU_Biologico, ")
            StrSQL.Append("                         SAU_Conversione,   SAU_Convenzionale, ")
            StrSQL.Append("                         Conversione_Inizio,   Conversione_Fine, ")
            StrSQL.Append("                         ConfiniRischio, Campo_Tipo, ")
            StrSQL.Append("                         Inviato, DataInvio, ")
            StrSQL.Append("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                         ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(IdBudget) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Campo_Des) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Totale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Biologico) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Conversione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SAU_Convenzionale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Fine) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(ConfiniRischio) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Campo_Tipo) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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

    Public Function Cancella(ByVal IdBudget As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Campi_W.EliminaDaBudget()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Campi ")
            StrSQL.Append(" WHERE    id_Budget= " & Agro_SQL_SaveNum(IdBudget) & " ")

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

    Public Function Disaggrega(ByVal Id_Budget As Integer,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Campo_Cod As Integer,
                               ByVal Appezza As Integer,
                               ByVal Validita_Inizio_Storico As String,
                               ByVal Validita_Fine_Storico As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudget_DAL.Campi_W.Disaggrega()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------------------------------------------------
            '----- 1.  Modifica del record nella tabella appezzamento (eliminazione campo_cod)
            '---------------------------------------------------------------------------------------
            StrSQL.Append(" UPDATE Budget_Appezzamento ")
            StrSQL.Append(" SET ")
            StrSQL.Append(" Campo_Cod = 0, ")
            StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(" WHERE Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            StrSQL.Append(" AND Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------------------------------------------------
            '----- 2.  Modifica del record in Campo_Storico (se non esiste lo creo...)
            '---------------------------------------------------------------------------------------

            StrSQL.Length = 0




            Dim objCampStor As New AgronicaCoreAnagrafeDAL.Campi_Storico_R
            If objCampStor.Esiste_Record_Campi_Storico(Piva, Sa_Cod, Campo_Cod, Appezza, objParametri) = True Then

                Validita_Inizio_Storico = CStr(DateAdd(DateInterval.Day, -1, CDate(Validita_Fine_Storico)))
                StrSQL.Append(" UPDATE Campi_Storico ")
                StrSQL.Append(" SET ")
                StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & ", ")
                StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
                StrSQL.Append(" Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine_Storico) & " ")

                StrSQL.Append(" WHERE Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            Else
                StrSQL.Append(" INSERT INTO Campi_Storico ")
                StrSQL.Append(" (Piva, Sa_Cod, Campo_Cod, Appezza, Data_Creazione, Data_Modifica,  ")
                StrSQL.Append(" Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
                StrSQL.Append(" VALUES ( ")
                StrSQL.Append("     '" & Agro_SQL_SaveText(Piva) & "', ")
                StrSQL.Append("     " & Agro_SQL_SaveNum(Sa_Cod) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveNum(Campo_Cod) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveNum(Appezza) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
                StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
                StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
                StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
                StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Inizio_Storico) & ",  ")
                StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Fine_Storico) & ") ")


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

    Public Function Aggrega(ByVal Id_Budget As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Validita_Inizio_Storico As String,
                            ByVal Validita_Fine_Storico As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Campi_W.Aggrega()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            '---------------------------------------------------------------------------------------
            '----- 1.  Modifica del record nella tabella appezzamento (inserimento campo_cod)
            '---------------------------------------------------------------------------------------


            'Genero la query SQL
            StrSQL.Append(" UPDATE Budget_Appezzamento ")
            StrSQL.Append(" SET ")
            StrSQL.Append(" Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & ", ")
            StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(" WHERE Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            StrSQL.Append(" AND Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------------------------------------------------
            '----- 2.  Inserimento di un record in Campo_Storico 
            '---------------------------------------------------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Campi_Storico ")
            StrSQL.Append(" (Piva, Sa_Cod, Campo_Cod, Appezza, Data_Creazione, Data_Modifica,  ")
            StrSQL.Append(" Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")
            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("     '" & Agro_SQL_SaveText(Piva) & "', ")
            StrSQL.Append("     " & Agro_SQL_SaveNum(Sa_Cod) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveNum(Campo_Cod) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveNum(Appezza) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append("     " & Agro_SQL_SaveDateTime(Now) & ", ")
            StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
            StrSQL.Append("     '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "',  ")
            StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Inizio_Storico) & ",  ")
            StrSQL.Append("     " & Agro_SQL_SaveDate(Validita_Fine_Storico) & ") ")

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
