Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class Centri_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Centri_R.Leggi()"

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
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PIVA , Sa_Cod, Sa_Nome, Validita_Inizio , Validita_Fine, inviato")
                    StrSQL.Append(" FROM  Budget_Centri_Aziendali ")
                    StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
                                StrSQL.Append(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Sa_Nome ASC")
                    End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Budget_Centri_Aziendali.* ")
                    StrSQL.Append(" FROM  Budget_Centri_Aziendali ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then

                        StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
                                StrSQL.Append(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Sa_Nome ASC")
                    End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT (Budget_Centri_Aziendali.piva + '_' +  cast(Budget_Centri_Aziendali.sa_cod as nvarchar(10))) as chiave,    Budget_Centri_Aziendali.*, Imprese.Rag_Soc, Budget_CentrixIndirizzi.Tipo_Indirizzo, ")
                    StrSQL.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
                    StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  ")
                    StrSQL.Append("         , ISNULL  ((  SELECT    TOP 1 val_cod  ")
                    StrSQL.Append("                         FROM    Budget_Centri_Aziendali_Codici   ")
                    StrSQL.Append("                         WHERE   Budget_Centri_Aziendali_Codici.piva = Budget_Centri_Aziendali.piva AND Budget_Centri_Aziendali_Codici.SA_COD = Budget_Centri_Aziendali.sa_cod AND Budget_Centri_Aziendali_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.TitoloPossesso) + "), '1') AS Titolo_Possesso ")
                    StrSQL.Append("  ,(select [User] from utenti where CODICE_FISCALE = imprese.Username_Modifica ) as utente_modifica ")
                    StrSQL.Append(" FROM  Budget_Centri_Aziendali ")
                    StrSQL.Append(" INNER JOIN Imprese  ON Imprese.PIVA = Budget_Centri_Aziendali.PIVA ")
                    StrSQL.Append(" INNER JOIN Budget_CentrixIndirizzi  ON Budget_CentrixIndirizzi.PIVA = Budget_Centri_Aziendali.PIVA AND Budget_CentrixIndirizzi.sa_cod = Budget_Centri_Aziendali.sa_cod ")
                    StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Budget_CentrixIndirizzi.cod_indirizzo ")
                    StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

                    StrSQL.Append(" WHERE Budget_Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Budget_Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then

                        StrSQL.Append(" AND  Budget_Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Budget_Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
                                StrSQL.Append(" AND Budget_Centri_Aziendali.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Budget_Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Budget_Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc, Budget_Centri_Aziendali.Sa_Nome")
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
    Public Function Leggi_x_anagraficaNG(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Centri_R.Leggi()"

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

            StrSQL.AppendLine(" SELECT top 100 (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) as chiave, ca.sa_cod, ca.sa_nome, (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) AS CodiceGIAS, ")
            StrSQL.AppendLine(" i.Piva, i.Rag_Soc, ISNULL(ic.val_cod, ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ca.Validita_Inizio, ca.Validita_Fine, ")
            StrSQL.AppendLine(" ind.cod_indirizzo, ind.ind_des, ind.frz_des, ind.CAP, ind.stato, ind.note, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(Lista_Province.Provincia, '') AS pro_cod,  ")
            StrSQL.AppendLine(" ind.ind_des + ' ' + ind.frz_des + ' ' + ind.Cap + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' ' + ISNULL(ISTAT.COMUNI_PROV, '') + ' ' + Stato AS Indirizzo, ")
            StrSQL.AppendLine(" ind.pro_cod_istat, ind.com_cod_istat, ind.stato, ind.note, ")
            StrSQL.AppendLine(" CASE Stato WHEN '' THEN 'IT' WHEN 'ITALIA' THEN 'IT' ELSE stato END as Stato_Cod, ")
            StrSQL.AppendLine(" ISNULL(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, 'Italia') as Stato, ")
            StrSQL.AppendLine(" ISNULL(cacTipoCentro.val_cod, ' ') AS TipoCentro, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCentro_Attuale & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS CodiceOperatoreBio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TipoAttivita & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS tipoAttivitaCod, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Modifica), ' ') AS Utente_Modifica, ")

            StrSQL.AppendLine(" ca.Data_Creazione, ")
            StrSQL.AppendLine(" ca.Data_Modifica ")


            StrSQL.AppendLine(" FROM Centri_Aziendali ca ")
            StrSQL.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = ca.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali_Codici cacTipoCentro ON ca.PIVA = cacTipoCentro.PIVA AND ca.sa_cod = cacTipoCentro.sa_cod AND cacTipoCentro.ID_Cod IN (101, 102, 103) ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe caTipoCentro ON caTipoCentro.creatore='CSA' AND gruppo='TIPO_CA' AND caTipoCentro.codice=cacTipoCentro.id_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = ca.PIVA AND CentrixIndirizzi.sa_cod = ca.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Province ON ind.pro_cod_istat = Lista_Province.PROV ")
            StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ON ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice = ind.Stato ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            StrSQL.AppendLine(" AND   ca.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ca.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  ca.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND  ca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
                        StrSQL.AppendLine(" AND ca.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc, ca.Sa_Nome")
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

    Public Sub Recupera_Superfici_CentroAziendale(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByRef Sup_Totale As Decimal,
                                        ByRef Sup_Bosco As Decimal,
                                        ByRef Sup_Prati As Decimal,
                                        ByRef Sup_Tare As Decimal,
                                        ByRef SAU_Totale As Decimal,
                                        ByRef SAU_Biologico As Decimal,
                                        ByRef SAU_Conversione As Decimal,
                                        ByRef SAU_Convenzionale As Decimal,
                                        ByVal DataRecupero As Date,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Centri_R.Anagrafica_Centri_Leggi()"

        Dim data_inizio_appoggio As Date
        Dim data_fine_appoggio As Date

        data_inizio_appoggio = objParametri.FinestraTemporaleInizio
        data_fine_appoggio = objParametri.FinestraTemporaleFine
        objParametri.FinestraTemporaleInizio = DataRecupero
        objParametri.FinestraTemporaleFine = DataRecupero

        Try

            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable
            Dim TipoAgricoltura As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura

            '------------------------------
            '-----  Sup_Totale  -----------
            '------------------------------

            Dim SupHaCondotta As Decimal         'Superficie parziale in ettari
            Dim SupHaTot As Decimal      'Superficie totale in ettari    
            Dim SupHaCatasto As Decimal         'Superficie parziale in ettari


            'leggo le informazioni		
            DT = objParticelle.Leggi(0,
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        0,
                                        "",
                                        "",
                                        "",
                                        0,
                                        0,
                                        "",
                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "",
                                        "",
                                        objParametri)




            'Inizializzo
            SupHaCondotta = 0
            SupHaTot = 0
            Sup_Totale = 0

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                SupHaCondotta = CDbl(DT.Rows(i).Item("sup_condotta"))

                'Converto 
                SupHaCatasto = Ettari_from_EttariAreCentiare(
                                        CDbl(DT.Rows(i).Item("ettari")),
                                        CDbl(DT.Rows(i).Item("are")),
                                        CDbl(DT.Rows(i).Item("centiare")))

                If SupHaCondotta <> 0 Then
                    SupHaTot = SupHaTot + SupHaCondotta
                Else
                    'sup_condotta non valorizzata: prendo la sup. della particella
                    SupHaTot = SupHaTot + SupHaCatasto
                End If

            Next

            'Assegno il totale
            Sup_Totale = SupHaTot


            'resetto i valori di objParametri
            objParametri.FinestraTemporaleInizio = data_inizio_appoggio
            objParametri.FinestraTemporaleFine = data_fine_appoggio


            '------------------------------
            '-----  Sup_Bosco  ------------
            '-----  Sup_Prati  ------------
            '------------------------------
            Dim objCentri As New AgronicaCoreBudgetDAL.Centri_R

            DT = objCentri.Leggi(CStr(Piva),
                                    CInt(Sa_Cod),
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametri)

            Sup_Bosco = 0
            Sup_Prati = 0

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Sup_Bosco = CDbl(DT.Rows(0).Item("Sup_Bosco"))
                Sup_Prati = CDbl(DT.Rows(0).Item("Sup_Prati"))
            End If


            '------------------------------
            '-----  SAU_Totale  -----------
            '-----  SAU_Biologico  --------
            '-----  SAU_Conversione  ------
            '-----  SAU_Convenzionale  ----
            '------------------------------


            Try
                StrSQL.Length = 0
                StrSQL.Append(" SELECT  Appezzamento.PIVA, ")
                StrSQL.Append("         Appezzamento.SA_COD,  ")
                StrSQL.Append("         Appezzamento.Campo_Cod,  ")
                StrSQL.Append("         Appezzamento.APPEZZA,  ")
                StrSQL.Append("         Appezzamento.APP_NOME,  ")
                StrSQL.Append("         Appezzamento.SUP_APP,  ")
                StrSQL.Append("         Appezzamento_Codici.val_cod,  ")
                StrSQL.Append("         Appezzamento.Validita_Inizio,  ")
                StrSQL.Append("         Appezzamento.Validita_Fine, ")
                StrSQL.Append("         ISNULL(Appezzamento_Codici.id_cod, 1018) AS Id_Cod  ")
                StrSQL.Append(" FROM    Appezzamento LEFT OUTER JOIN ")
                StrSQL.Append("         Appezzamento_Codici ON Appezzamento.PIVA = Appezzamento_Codici.PIVA   ")
                StrSQL.Append("         AND Appezzamento.SA_COD = Appezzamento_Codici.sa_cod ")
                StrSQL.Append("         AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza ")
                StrSQL.Append(" WHERE   (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                StrSQL.Append(" AND     (Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                StrSQL.Append(" AND     (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & ") ")
                StrSQL.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & ")")
                StrSQL.Append(" AND id_cod = 1018")

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                DT = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try


            'Verifico la presenza di errori
            If IsNothing(MessaggioErrore) Then
                'ERRORE
                SAU_Totale = -999999
                SAU_Biologico = -999999
                SAU_Conversione = -999999
                SAU_Convenzionale = -999999
            Else
                'Inizializzo
                SAU_Totale = 0
                SAU_Biologico = 0
                SAU_Conversione = 0
                SAU_Convenzionale = 0

                If Not IsNothing(DT) Then
                    'Ciclo sugli elementi selezionati
                    For i = 0 To DT.Rows.Count - 1
                        SAU_Totale += CDbl(DT.Rows(i).Item("sup_app"))
                        'Verifico il tipo di agricoltura
                        If IsDBNull(DT.Rows(i).Item("val_cod")) Then
                            'NOTA
                            'Se non e' impostato il tipo di agricoltura,
                            'considero come default quella Convenzionale
                            TipoAgricoltura = AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.Convenzionale
                        Else
                            TipoAgricoltura = CInt(DT.Rows(i).Item("val_cod"))
                        End If

                        'Aggiorno il contatore giusto
                        Select Case TipoAgricoltura

                            Case AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.Convenzionale
                                SAU_Convenzionale += CDbl(DT.Rows(i).Item("sup_app"))

                            Case AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.InConversione
                                SAU_Conversione += CDbl(DT.Rows(i).Item("sup_app"))

                            Case AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.Biologica
                                SAU_Biologico += CDbl(DT.Rows(i).Item("sup_app"))
                        End Select
                    Next
                Else
                    SAU_Totale = 0
                    SAU_Biologico = 0
                    SAU_Conversione = 0
                    SAU_Convenzionale = 0
                End If

            End If


            '------------------------------
            '-----  Sup_Tare  -------------
            '------------------------------
            Sup_Tare = Sup_Totale - SAU_Totale



        Catch ex As Exception

        Finally
            'resetto i valori di objParametri
            objParametri.FinestraTemporaleInizio = data_inizio_appoggio
            objParametri.FinestraTemporaleFine = data_fine_appoggio
        End Try

    End Sub
End Class


Public Class Centri_W
    Inherits AgronicaCoreDataProvider.DataProvider


End Class
