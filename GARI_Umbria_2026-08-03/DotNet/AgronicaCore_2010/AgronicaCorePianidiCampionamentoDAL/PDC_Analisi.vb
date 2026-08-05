Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProviders
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class PDC_Analisi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiDatiAnalisi_from_CodiceSocio_CodCACcultivar(ByVal Codice_Socio As String,
                                                                     ByVal Cultivar_Coltiva As String,
                                                                     ByVal Data As Date,
                                                                     ByVal Filtro_Capitolati As String,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByVal xOrderBy As String,
                                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                     ) As DataTable

        Const nomeRoutine As String = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiDatiAnalisi_from_CodiceSocio_CodCACcultivar()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0
            '---------------------------------------------
            stb.AppendLine(" SELECT PDC_Testata.Id_PDC_Testata,  PDC_Testata.PDC_Testata_Des, PDC_Testata.Validita_Fine AS PDC_DataFine, ")
            stb.AppendLine(" --Codice Fornitore salvato importato da dbwin così come già salvato sui PDC (lo prende da codice socio se non erro) ")
            stb.AppendLine(" PDC_Dettagli.ID_PDC_dettagli, PDC_Dettagli.codicefornitore AS codice_socio, PDC_Dettagli.piva, PDC_Dettagli.sa_cod,PDC_Dettagli.Appezza,PDC_Dettagli.id_reg, ")
            stb.AppendLine(" sv.veg_cod, veg_des, PDC_Dettagli.cul_cod, CUL_DES, ISNULL(CAC.Cultivar_Coltiva, 'NO MAPPING') AS Cultivar_Coltiva, ISNULL(CAC.Descrizione, 'NO MAPPING') AS Descrizione,")
            stb.AppendLine(" analisi_testata.Analisi_Testata_Cod, analisi_testata.analisi_testata_data_fine AS data_analisi, ")
            stb.AppendLine(" -- Numero/Codice dell'analisi così come denominata dal laboratorio ")
            stb.AppendLine(" analisi_testata.analisi_testata_des AS numero_analisi,  ")
            stb.AppendLine(" PDC_Analisi.cod_risum AS cod_risum_laboratorio, ISNULL(ru.settore_des,'') AS cod_laboratorio, ")
            stb.AppendLine(" pdc_sblocca.CapitolatoCliente_Cod, PDC_CapitolatiCliente_Attivi.des_capitolato_privato, CAP.Capitolato_Codice ")
            stb.AppendLine(" FROM analisi_testata ")
            stb.AppendLine(" INNER JOIN PDC_Analisi ON analisi_testata.Analisi_Testata_Cod  =PDC_Analisi.Analisi_Testata_Cod ")
            stb.AppendLine(" INNER JOIN PDC_Testata ON PDC_Analisi.PivaSuperUser = PDC_Testata.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata= PDC_Testata.Id_PDC_Testata ")
            stb.AppendLine(" INNER JOIN PDC_Dettagli ON PDC_Dettagli.ID_PDC_Testata=PDC_Analisi.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_dettagli=PDC_Analisi.ID_PDC_dettagli  ")
            stb.AppendLine(" INNER JOIN risorse_umane ru ON PDC_Analisi.cod_risum=ru.cod_risum AND ru.cod_rapporto=-8 ")
            stb.AppendLine(" INNER JOIN PDC_sblocca ON pdc_sblocca.PivaSuperUser  =PDC_Dettagli.PivaSuperUser AND pdc_sblocca.Id_PDC_Testata=PDC_Dettagli.ID_PDC_Testata AND pdc_sblocca.piva=PDC_Dettagli.Piva AND  pdc_sblocca.sa_cod= PDC_Dettagli.sa_cod AND pdc_sblocca.appezza=PDC_Dettagli.appezza AND pdc_sblocca.id_reg=PDC_Dettagli.id_reg  ")
            stb.AppendLine(" INNER JOIN PDC_CapitolatiCliente_Attivi ON PDC_CapitolatiCliente_Attivi.PivaSuperUser=pdc_sblocca.PivaSuperUser AND PDC_CapitolatiCliente_Attivi.ID_PDC_Testata=pdc_sblocca.ID_PDC_Testata AND PDC_CapitolatiCliente_Attivi.ID_CapitolatoPrivato = pdc_sblocca.CapitolatoCliente_Cod  ")
            stb.AppendLine(" INNER JOIN CapitolatoCliente  cap ON PDC_CapitolatiCliente_Attivi.ID_CapitolatoPrivato = cap.Capitolato_Cod  ")
            stb.AppendLine(" INNER JOIN cultivar cul ON cul.cul_cod = PDC_Dettagli.cul_cod ")
            stb.AppendLine(" INNER JOIN SpecieVegetali SV ON sv.veg_cod = CUL.Veg_Cod ")
            stb.AppendLine(" left outer JOIN cac_codifica_cultivar CAC ON PDC_Dettagli.cul_cod=CAC.cultivar_gias ")
            stb.AppendLine(" WHERE pdc_sblocca.esito=-1  ")
            '                                 Introdotto il cast per evitare che nella data venisse messo ora 12:00 AM
            stb.AppendLine(" AND PDC_Testata.Validita_Fine >= CAST( " & Agro_SQL_SaveDate(Data) & " AS Date ) ")
            stb.AppendLine(" AND (PDC_Analisi.Mostra_in_Stampe is null or PDC_Analisi.Mostra_in_Stampe = -1)  ")
            If Filtro_Capitolati <> "" Then
                stb.AppendLine(Filtro_Capitolati)
            End If
            If Codice_Socio <> "" Then
                '06/04/2022: dopo tel di Donati: sostituito = con LIKE perchè alcune aziende hanno più cod_forn separati da | nel codice socio di GIAS 
                '(perchè negli anni hanno cambiato codice socio)
                'stb.AppendLine(" AND PDC_Dettagli.codicefornitore= '" & Agro_SQL_SaveText(Codice_Socio) & "' ")
                stb.AppendLine(" AND PDC_Dettagli.codicefornitore LIKE '%" & Agro_SQL_SaveText(Codice_Socio) & "%' ")
            End If
            If Cultivar_Coltiva <> "" Then
                stb.AppendLine(" AND CAC.Cultivar_Coltiva ='" & Agro_SQL_SaveText(Cultivar_Coltiva) & "' ")
            End If
            stb.AppendLine(" ORDER BY analisi_testata.analisi_testata_data_fine desc ")

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

    Public Function LeggiAnalisixMail(ByVal ID_PDC_Testata As Integer,
                                      ByVal ID_PDC_Dettagli As Integer,
                                      ByVal ID_PDC_Campione As Integer,
                                      ByVal Analisi_Testata_Cod As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisixMail()"

        'vanni, 22/04/2013 corretto baco su tabella contatto in join piuttosto che impresa
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0
            '---------------------------------------------

            stb.AppendLine(" SELECT PDC_Dettagli.Piva, icGGN.val_cod AS GGN, ISNULL(PDC_Dettagli.CodiceFornitore,'') AS CodiceFornitore , ISNULL(PDC_Dettagli.Rag_Soc,'') as Rag_Soc, ISNULL(PDC_Dettagli.Sa_Nome,'') as Sa_Nome, isnull(PDC_Dettagli.App_Nome,'')as App_nome, ISNULL(PDC_Dettagli.Sup_Imp,'') as Sup_Imp,  ")
            stb.AppendLine(" ISNULL(PDC_Dettagli.CapitolatoPrivato,'')AS CapitolatoPrivato, isnull(PDC_Dettagli.Regolamento,'') AS Regolamento, ISNULL(PDC_Campioni.Data_Campionamento,'')as Data_Campionamento, ISNULL(PDC_Campioni.Codice_Campione,'')as Codice_Campione,  ")
            stb.AppendLine(" ISNULL(PDC_Dettagli.note_impianto, '') AS 'note_impianto',  " & vbCrLf)
            stb.AppendLine(" ISNULL(REPLACE ( PDC_Campioni.Tecnico_Campione , '&#39;' , ''''),'')as Tecnico_Campione, ISNULL(REPLACE ( PDC_Campioni.Note_Campione , '&#39;' , ''''),'')as Note_Campione, ISNULL(PDC_Campioni.Codice_Progressivo_Inizio_Anno,'')as Codice_Progressivo_Inizio_Anno, isnull(PDC_Campioni.Codice_Anno,'')as Codice_Anno, isnull(Descrizione_PuntoDiPrelievo,'') as PuntoPrelievo,  ")
            stb.AppendLine(" ISNULL(PDC_Analisi.Cod_Risum,'') as Cod_Risum, ISNULL(PDC_Analisi.Data_Richiesta_Analisi,'') as Data_Richiesta_Analisi, ISNULL(PDC_Analisi.Altre_Molecole,'') As Altre_Molecole, ISNULL(PDC_Analisi.Note_Richiesta_Analisi, '')  AS Note_Richiesta_Analisi,  ")
            stb.AppendLine(" ISNULL(PDC_FornitoreFatturazione.RagSoc, '') AS Fornitore_Di_Fatturazione, ISNULL(Contatti.Rag_Soc,  ")
            stb.AppendLine(" ISNULL(Contatti.Nome + ' ' + Contatti.Cognome, '')) AS Laboratorio, ISNULL(Analisi_Tipologia.Analisi_tipologia_des ,'' ) AS Analisi_Tipologia_Des, ISNULL(Analisi_Testata.Analisi_Testata_Note1,'')as Analisi_Testata_Note1 ")
            stb.AppendLine(" , ISNULL(SpecieVegetali.veg_des ,'')as Veg_Des , ISNULL(cultivar.cul_des ,'') as Cul_Des, ISNULL( (select grva_des from GruppoVarietale where GruppoVarietale.grva_cod = pdc_dettagli.grva_cod) , '') as tipologia_varietale ")
            stb.AppendLine(" , ISNULL(Imprese_Codici.val_cod ,'')as codice_2  ")
            stb.AppendLine(" , (select top 1 tipo_campione from PDC_Tipo_Campione_Apofruit where pdc_analisi.Tipo_Campione_Apofruit = pdc_tipo_campione_apofruit.codice  ) as Tipo_Campione_Apofruit ")
            stb.AppendLine(" , (select distinct descrizione_stabilimento from [PDC_Stabilimenti_Apofruit] where cod_stabilimento = PDC_testata.codicestabilimento) as Nome_Stabilimento ")
            stb.AppendLine(" , ISNULL( pdc_dettagli.veg_cod ,'') as Veg_Cod, isnull(pdc_dettagli.cul_cod,'') AS Cul_Cod, isnull(pdc_dettagli.grva_cod,'') as Grva_Cod ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Piva_FornitoreFatturazione,'') as Piva_FornitoreFatturazione ")
            stb.AppendLine(" , ISNULL(pdc_testata.CodiceStabilimento,'') as CodiceStabilimento, ISNULL(PDC_Campioni.Tipo_Campione,0) AS Tipo_Campione, PDC_Dettagli.id_lfo, ISNULL(i_padre.rag_soc,'') as rag_soc_padre ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Grower_Number,'') AS Grower_Number ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Block,'') AS Block ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Kpin,'') AS Kpin ")
            stb.AppendLine(" , ISNULL(Descrizione_MotivoCampionamento,'') AS MotivoCampione ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Analisi_Tipologia_Tipo,'') AS Analisi_Tipologia_Tipo ")
            stb.AppendLine(" , PDC_Dettagli.ind_des + ' ' + PDC_Dettagli.frz_des + ' - ' + PDC_Dettagli.Cap + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' ' + ISNULL(ISTAT.COMUNI_PROV, '') + ' ' + Stato AS Indirizzo ")
            stb.AppendLine(" , PDC_Dettagli.Tecnico_Campionamento, PDC_Dettagli.Tecnico_Campionamento_Tel ")

            stb.AppendLine(" FROM PDC_Dettagli ")
            stb.AppendLine(" INNER JOIN pdc_testata ON pdc_dettagli.id_pdc_testata = pdc_testata.id_pdc_testata ")
            stb.AppendLine(" INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND   PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli     ")
            stb.AppendLine(" INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND   PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione     ")
            stb.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser       ")
            stb.AppendLine(" LEFT JOIN PDC_FornitoreFatturazione ON PDC_Analisi.Piva_FornitoreFatturazione = PDC_FornitoreFatturazione.Piva ")
            stb.AppendLine(" LEFT JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = PDC_Analisi.Cod_Risum ")
            stb.AppendLine(" LEFT JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto And Contatti.Piva = Risorse_Umane.Piva ")
            stb.AppendLine(" LEFT JOIN Analisi_Tipologia ON Analisi_Tipologia.analisi_tipologia_cod = PDC_Analisi.analisi_tipologia_cod ")
            stb.AppendLine(" LEFT JOIN PDC_PuntoDiPrelievo ON PDC_PuntoDiPrelievo.id_puntodiprelievo = PDC_Campioni.PuntoPrelievo ")
            stb.AppendLine(" LEFT JOIN PDC_MotivoCampionamento ON PDC_MotivoCampionamento.ID_MotivoCampionamento = PDC_Campioni.Motivo_Campione ")
            stb.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.veg_cod = PDC_Dettagli.veg_cod ")
            stb.AppendLine(" LEFT JOIN cultivar ON cultivar.cul_cod = PDC_Dettagli.cul_cod ")
            stb.AppendLine(" LEFT JOIN Imprese_Codici ON Imprese_Codici.piva = PDC_Dettagli.piva and Imprese_Codici.id_cod = 1091 ")
            stb.AppendLine(" LEFT JOIN Imprese_Codici as icGGN ON icGGN.piva = PDC_Dettagli.Piva and icGGN.id_cod = 1261 ")
            stb.AppendLine(" LEFT JOIN GerarchiaImprese as ger ON ger.Figlio = PDC_Dettagli.Piva and ger.Padre <> '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" LEFT JOIN Imprese as i_padre ON i_padre.Piva = ger.Padre ")
            stb.AppendLine(" LEFT JOIN ISTAT ON PDC_Dettagli.pro_cod_istat = ISTAT.PROV AND PDC_Dettagli.com_cod_istat = ISTAT.COM ")

            stb.AppendLine("  " & vbCrLf)
            stb.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If ID_PDC_Dettagli <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If
            If ID_PDC_Campione <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If
            If Analisi_Testata_Cod <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function LeggiAnalisixInvioRest(ByVal ID_PDC_Testata As Integer,
                                           ByVal ID_PDC_Dettagli As Integer,
                                           ByVal ID_PDC_Campione As Integer,
                                           ByVal Analisi_Testata_Cod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisixInvioRest()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0
            '---------------------------------------------

            stb.AppendLine(" SELECT PDC_Analisi.PivaSuperUser ")
            stb.AppendLine(" , PDC_Analisi.ID_PDC_Testata, PDC_Analisi.ID_PDC_Dettagli ")
            stb.AppendLine(" , PDC_Analisi.ID_PDC_Campione, PDC_Analisi.Analisi_Testata_Cod ")
            stb.AppendLine(" , CAST(PDC_Analisi.ID_PDC_Testata AS VARCHAR) + '_' + CAST(PDC_Analisi.ID_PDC_Dettagli AS VARCHAR) + '_' + CAST(PDC_Analisi.ID_PDC_Campione AS VARCHAR) + '_' + CAST(PDC_Analisi.Analisi_Testata_Cod AS VARCHAR) AS Chiave ")
            stb.AppendLine(" , ISNULL(PDC_Testata.PivaOwner, PDC_Analisi.PivaSuperUser) AS PivaOwner ")
            stb.AppendLine(" , PDC_Dettagli.Piva ")

            stb.AppendLine(" , ISNULL(PDC_Dettagli.Piva_OP,'') AS Piva_OP ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Rag_Soc_OP,'') AS Rag_Soc_OP ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Kpin,'') AS Kpin ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.[Block],'') AS 'Block' ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Grower_Number,'') AS 'Grower_Number' ")
            stb.AppendLine(" , '' AS 'Area' ")    'TODO: valorizzare quando sarà usata anche l'area maturazione
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Ind_Des,'') AS Ind_Dettaglio_Ind_Des ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Frz_Des,'') AS Ind_Dettaglio_Frz_Des ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Com_Des,'') AS Ind_Dettaglio_Com_Des ")
            stb.AppendLine(" , LTRIM(RTRIM(ISNULL(PDC_Dettagli.Frz_Des,'') + ' ' + ISNULL(PDC_Dettagli.Com_Des,''))) AS Ind_Dettaglio_Comune_Completo ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.CAP,'') AS Ind_Dettaglio_CAP ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Pro_Cod,'') AS Ind_Dettaglio_Pro_Cod ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Stato,'IT') AS Ind_Dettaglio_Stato ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Reg,'000') AS Ind_Dettaglio_Regione_Cod ")
            stb.AppendLine(" , ISNULL(Lista_Regioni.Regione_Des,'') AS Ind_Dettaglio_Regione_Des ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Pro_Cod_Istat,'000') AS Ind_Dettaglio_Pro_Cod_Istat ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Com_Cod_Istat,'000') AS Ind_Dettaglio_Com_Cod_Istat ")
            stb.AppendLine(" , ISNULL(icGGN.val_cod,'') AS GGN ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.CodiceFornitore,'') AS CodiceFornitore ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Rag_Soc,'') AS Rag_Soc ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Sa_Nome,'') AS Sa_Nome ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.App_Nome,'') AS App_nome ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Sup_Imp,'') AS Sup_Imp ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.CapitolatoPrivato,'') AS CapitolatoPrivato ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Regolamento,'') AS Regolamento ")

            stb.AppendLine(" , ISNULL(PDC_Dettagli.Tecnico_Campionamento,'') AS Tecnico_Campionamento ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Tecnico_Campionamento_Tel,'') AS Tecnico_Campionamento_Tel ")

            stb.AppendLine(" , ISNULL(PDC_Campioni.Data_Campionamento,'') AS Data_Campionamento ")
            stb.AppendLine(" , ISNULL(PDC_Campioni.Codice_Campione,'') AS Codice_Campione ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.note_impianto, '') AS 'note_impianto' " & vbCrLf)

            stb.AppendLine(" , ISNULL(REPLACE(PDC_Campioni.Tecnico_Campione , '&#39;' , ''''),'') AS Tecnico_Campione ")
            stb.AppendLine(" , ISNULL(REPLACE(PDC_Campioni.Note_Campione , '&#39;' , ''''),'') AS Note_Campione ")
            stb.AppendLine(" , ISNULL(PDC_Campioni.Codice_Progressivo_Inizio_Anno,'') AS Codice_Progressivo_Inizio_Anno ")
            stb.AppendLine(" , ISNULL(PDC_Campioni.Codice_Anno,'') AS Codice_Anno ")
            stb.AppendLine(" , ISNULL(PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo,'') AS PuntoPrelievo ")

            stb.AppendLine(" , ISNULL(PDC_Campioni.Num_Prodotti,1) AS Num_Prodotti ")
            stb.AppendLine(" , ISNULL(PDC_Campioni.Motivo_Campione,0) AS Motivo_Campione_Cod ")
            stb.AppendLine(" , ISNULL(PDC_MotivoCampionamento.Descrizione_MotivoCampionamento,'') AS Motivo_Campione_Des ")

            stb.AppendLine(" , ISNULL(PDC_Analisi.Cod_Risum,'') AS Cod_Risum ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Data_Richiesta_Analisi,'') AS Data_Richiesta_Analisi ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Note_Richiesta_Analisi, '') AS Note_Richiesta_Analisi ")
            stb.AppendLine(" , ISNULL(PDC_FornitoreFatturazione.RagSoc, '') AS Fornitore_Di_Fatturazione ")
            stb.AppendLine(" , ISNULL(Contatti.Rag_Soc, ISNULL(Contatti.Nome + ' ' + Contatti.Cognome, '')) AS Laboratorio ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Analisi_Tipologia_Tipo,0) AS Analisi_Tipologia_Tipo ")
            stb.AppendLine(" , ISNULL(Analisi_Tipologia.Analisi_tipologia_des ,'') AS Analisi_Tipologia_Des ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Altre_Molecole,'') AS Altre_Molecole ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Altre_Molecole_Cod,'') AS Altre_Molecole_Cod ")
            stb.AppendLine(" , ISNULL(Analisi_Testata.Analisi_Testata_Note1,'') AS Analisi_Testata_Note1 ")
            stb.AppendLine(" , ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des ")
            stb.AppendLine(" , ISNULL(cultivar.cul_des ,'') AS Cul_Des ")
            stb.AppendLine(" , ISNULL( (SELECT Grva_Des FROM GruppoVarietale WHERE GruppoVarietale.Grva_Cod = PDC_Dettagli.Grva_Cod) , '') AS tipologia_varietale ")
            stb.AppendLine(" , ISNULL(Imprese_Codici.val_cod ,'') AS codice_2 ")
            stb.AppendLine(" , (SELECT TOP 1 tipo_campione from PDC_Tipo_Campione_Apofruit WHERE pdc_analisi.Tipo_Campione_Apofruit = pdc_tipo_campione_apofruit.codice ) AS Tipo_Campione_Apofruit ")
            stb.AppendLine(" , (SELECT DISTINCT descrizione_stabilimento FROM [PDC_Stabilimenti_Apofruit] WHERE cod_stabilimento = PDC_Testata.codicestabilimento) AS Nome_Stabilimento ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Veg_Cod,0) AS Veg_Cod ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Cul_Cod,0) AS Cul_Cod ")
            stb.AppendLine(" , ISNULL(PDC_Dettagli.Grva_Cod,0) AS Grva_Cod ")
            stb.AppendLine(" , ISNULL(PDC_Analisi.Piva_FornitoreFatturazione,'') AS Piva_FornitoreFatturazione ")
            stb.AppendLine(" , ISNULL(PDC_Testata.CodiceStabilimento,'') AS CodiceStabilimento ")
            stb.AppendLine(" , ISNULL(PDC_Campioni.Tipo_Campione,0) AS Tipo_Campione ")
            stb.AppendLine(" , PDC_Dettagli.ID_LFO ")
            stb.AppendLine(" , ISNULL(i_padre.rag_soc,'') AS rag_soc_padre ")

            stb.AppendLine(" FROM PDC_Dettagli ")
            stb.AppendLine(" INNER JOIN PDC_Testata ON PDC_Dettagli.id_pdc_testata = PDC_Testata.id_pdc_testata ")
            stb.AppendLine(" INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND   PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli     ")
            stb.AppendLine(" INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND   PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione     ")
            stb.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser ")
            stb.AppendLine(" LEFT JOIN PDC_FornitoreFatturazione ON PDC_Analisi.Piva_FornitoreFatturazione = PDC_FornitoreFatturazione.Piva ")
            stb.AppendLine(" LEFT JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = PDC_Analisi.Cod_Risum ")
            stb.AppendLine(" LEFT JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            stb.AppendLine(" LEFT JOIN Analisi_Tipologia ON Analisi_Tipologia.analisi_tipologia_cod = PDC_Analisi.analisi_tipologia_cod ")
            stb.AppendLine(" LEFT JOIN PDC_PuntoDiPrelievo ON PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo = PDC_Campioni.PuntoPrelievo ")
            stb.AppendLine(" LEFT JOIN PDC_MotivoCampionamento ON PDC_MotivoCampionamento.ID_MotivoCampionamento = PDC_Campioni.Motivo_Campione ")
            stb.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.veg_cod = PDC_Dettagli.veg_cod ")
            stb.AppendLine(" LEFT JOIN Cultivar ON Cultivar.cul_cod = PDC_Dettagli.cul_cod ")
            stb.AppendLine(" LEFT JOIN Imprese_Codici ON Imprese_Codici.piva = PDC_Dettagli.Piva AND Imprese_Codici.id_cod = 1091 ")
            stb.AppendLine(" LEFT JOIN Imprese_Codici AS icGGN ON icGGN.piva = PDC_Dettagli.Piva AND icGGN.id_cod = 1261 ")
            stb.AppendLine(" LEFT JOIN GerarchiaImprese AS ger ON ger.Figlio = PDC_Dettagli.Piva AND ger.Padre <> '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" LEFT JOIN Imprese AS i_padre ON i_padre.Piva = ger.Padre ")
            stb.AppendLine(" LEFT JOIN Lista_Regioni ON PDC_Dettagli.Reg = Lista_Regioni.Reg ")

            stb.AppendLine("  " & vbCrLf)
            stb.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If ID_PDC_Dettagli <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If
            If ID_PDC_Campione <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If
            If Analisi_Testata_Cod <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function LeggiAnalisixMailAllegati(ByVal Analisi_Testata_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisixMailAllegati()"

        'vanni, 22/04/2013 corretto baco su tabella contatto in join piuttosto che impresa
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0
            '---------------------------------------------

            stb.AppendLine("  SELECT      analisi_testata_des,  PDC_Dettagli.Piva, isnull(PDC_Dettagli.CodiceFornitore,'')as CodiceFornitore , isnull(PDC_Dettagli.Rag_Soc,'') as Rag_Soc, isnull(PDC_Dettagli.Sa_Nome,'') as Sa_Nome, isnull(PDC_Dettagli.App_Nome,'')as App_nome, isnull(PDC_Dettagli.Sup_Imp,'') as Sup_Imp,  ")
            stb.AppendLine("  isnull(PDC_Dettagli.CapitolatoPrivato,'')as CapitolatoPrivato, isnull(PDC_Dettagli.Regolamento,'')as Regolamento, isnull(PDC_Campioni.Data_Campionamento,'')as Data_Campionamento, isnull(PDC_Campioni.Codice_Campione,'')as Codice_Campione,  ")
            stb.AppendLine("  ISNULL(PDC_Dettagli.note_impianto, '') AS 'note_impianto',   ")
            stb.AppendLine("  isnull(REPLACE ( PDC_Campioni.Note_Campione , '&#39;' , ''''),'')as Note_Campione, isnull(PDC_Campioni.Codice_Progressivo_Inizio_Anno,'')as Codice_Progressivo_Inizio_Anno, isnull(PDC_Campioni.Codice_Anno,'')as Codice_Anno, isnull(Descrizione_PuntoDiPrelievo,'') as PuntoPrelievo,  ")
            stb.AppendLine("  isnull(PDC_Analisi.Cod_Risum,'') as Cod_Risum, isnull(PDC_Analisi.Data_Richiesta_Analisi,'') as Data_Richiesta_Analisi, isnull(PDC_Analisi.Altre_Molecole,'') As Altre_Molecole, ISNULL(PDC_Analisi.Note_Richiesta_Analisi, '')  AS Note_Richiesta_Analisi,  ")
            stb.AppendLine("  ISNULL(PDC_FornitoreFatturazione.RagSoc, '') AS Fornitore_Di_Fatturazione, ")
            stb.AppendLine("  ISNULL(Contatti.Rag_Soc, ISNULL(Contatti.Nome + ' ' + Contatti.Cognome, '')) AS Laboratorio, isnull(Analisi_Tipologia.Analisi_tipologia_des ,'' ) as Analisi_Tipologia_Des, isnull(Analisi_Testata.Analisi_Testata_Note1,'')as Analisi_Testata_Note1 ")
            stb.AppendLine("  , isnull(SpecieVegetali.veg_des ,'')as Veg_Des , isnull(cultivar.cul_des ,'') as Cul_Des, ISNULL( (select grva_des from GruppoVarietale where GruppoVarietale.grva_cod = pdc_dettagli.grva_cod) , '') as tipologia_varietale ")
            stb.AppendLine("  ,ISNULL(Imprese_Codici.val_cod ,'')as codice_2  ")
            stb.AppendLine("  , (select top 1 tipo_campione from PDC_Tipo_Campione_Apofruit where pdc_analisi.Tipo_Campione_Apofruit = pdc_tipo_campione_apofruit.codice  ) as Tipo_Campione_Apofruit ")
            stb.AppendLine("  , (select distinct descrizione_stabilimento from [PDC_Stabilimenti_Apofruit] where cod_stabilimento = PDC_testata.codicestabilimento) as Nome_Stabilimento ")
            stb.AppendLine("  , isnull( pdc_dettagli.veg_cod ,'') as Veg_Cod, isnull(pdc_dettagli.cul_cod,'')as Cul_Cod, isnull(pdc_dettagli.grva_cod,'') as Grva_Cod ")
            stb.AppendLine("  , isnull(PDC_Analisi.Piva_FornitoreFatturazione,'') as Piva_FornitoreFatturazione ")
            stb.AppendLine("  , isnull(pdc_testata.CodiceStabilimento,'') as CodiceStabilimento ")
            stb.AppendLine("  ,CASE 	WHEN Analisi_parametro_cod <0 THEN ( select Descrizione  from FamigliePrincipiAttivi where fam_cod = CAST(0 - Analisi_parametro_cod AS nvarchar(25)) ) ELSE  (select pa_des  from PrincipiAttivi  where Pa_Cod  =  Analisi_parametro_cod ) END AS nome ")
            stb.AppendLine("  , Analisi_Dettaglio_Valore_1  as Qta ")

            stb.AppendLine(" FROM            PDC_Dettagli ")
            stb.AppendLine(" inner join pdc_testata on pdc_dettagli.id_pdc_testata = pdc_testata.id_pdc_testata ")
            stb.AppendLine(" INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND   PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli     ")
            stb.AppendLine(" INNER JOIN  PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND   PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione     ")
            stb.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser       ")
            stb.AppendLine(" LEFT OUTER JOIN  PDC_FornitoreFatturazione ON PDC_Analisi.Piva_FornitoreFatturazione = PDC_FornitoreFatturazione.Piva     ")
            stb.AppendLine(" LEFT OUTER JOIN  Risorse_Umane ON Risorse_Umane.Cod_RisUm = PDC_Analisi.Cod_Risum     ")
            stb.AppendLine(" LEFT OUTER JOIN  Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto      ")
            stb.AppendLine(" left join Analisi_Tipologia on Analisi_Tipologia.analisi_tipologia_cod = PDC_Analisi.analisi_tipologia_cod      ")
            stb.AppendLine(" left join PDC_PuntoDiPrelievo on PDC_PuntoDiPrelievo.id_puntodiprelievo  = PDC_Campioni.PuntoPrelievo    ")
            stb.AppendLine("  left join SpecieVegetali on SpecieVegetali.veg_cod = PDC_Dettagli.veg_cod  ")
            stb.AppendLine("  left join cultivar on cultivar.cul_cod = PDC_Dettagli.cul_cod ")
            stb.AppendLine(" left join Imprese_Codici on Imprese_Codici.piva = PDC_Dettagli.piva and Imprese_Codici.id_cod = 1091 ")
            stb.AppendLine(" Left Join Analisi_Dettagli Ad on Ad.Analisi_Testata_Cod =  Analisi_Testata.Analisi_Testata_Cod and Ad.Analisi_Testata_Cod  =  Analisi_Testata.Analisi_Testata_Cod ")

            stb.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

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

    Public Function GetVeg_Cod(ByVal Analisi_Testata_Cod As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Integer

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.GetVeg_Cod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT     PDC_Dettagli.Veg_Cod ")
            StrSQL.AppendLine(" FROM         PDC_Analisi INNER JOIN ")
            StrSQL.AppendLine("       PDC_Dettagli ON PDC_Analisi.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Analisi.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli AND PDC_Analisi.PivaSuperUser = PDC_Dettagli.PivaSuperUser ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
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

        Return dt.Rows(0).Item("Veg_Cod")

    End Function

    Public Function GetAnnoCampione(ByVal Analisi_Testata_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Integer

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.GetAnnoCampione()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" Select Year(PDC_Campioni.Data_Campionamento) as anno ")
            StrSQL.AppendLine(" FROM         PDC_Analisi ")
            StrSQL.AppendLine(" INNER JOIN      PDC_Campioni  ")
            StrSQL.AppendLine("         ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND       PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione  ")
            StrSQL.AppendLine("     INNER JOIN      PDC_Dettagli ")
            StrSQL.AppendLine("         ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND      PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli  ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
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

        Return dt.Rows(0).Item("anno")

    End Function

    Public Function LeggixEsport_saragone(ByVal ID_PDC_Testata As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggixEsport_Saragone()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ad.Analisi_Testata_Cod ,   ad.Analisi_Dettaglio_Cod, ad.Analisi_Parametro_Cod, ")
            StrSQL.AppendLine(" ad.Analisi_Dettaglio_Valore_1, (select udm_sim from UnitaMisura where UDM_COD = ad.Analisi_Dettaglio_Valore_2) as udm , ")
            StrSQL.AppendLine(" pa.Pa_Des, fa.Descrizione ")
            StrSQL.AppendLine(" FROM            Analisi_Dettagli AS ad LEFT OUTER JOIN ")
            StrSQL.AppendLine(" PrincipiAttivi AS pa ON pa.Pa_Cod = ad.Analisi_Parametro_Cod LEFT OUTER JOIN")
            StrSQL.AppendLine(" FamigliePrincipiAttivi AS fa ON fa.Fam_Cod = CAST(0 - ad.Analisi_Parametro_Cod AS nvarchar(25))")
            StrSQL.AppendLine(" inner join PDC_Analisi pdc on pdc.Analisi_Testata_Cod = ad.Analisi_Testata_Cod  ")

            StrSQL.AppendLine(" WHERE pdc.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))

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

    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_PDC_Dettagli As Integer,
                          ByVal ID_PDC_Campione As Integer,
                          ByVal Analisi_Testata_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT        PDC_Analisi.ID_PDC_Testata, PDC_Analisi.ID_PDC_Dettagli, PDC_Analisi.ID_PDC_Campione, PDC_Analisi.Analisi_Testata_Cod, PDC_Analisi.PDC_Stato_Analisi, PDC_Analisi.PDC_Stato_Validazione, PDC_Analisi.PDC_Stato_Pubblicazione, PDC_Analisi.Cod_Risum, PDC_Analisi.Analisi_Tipologia_Cod, isnull( PDC_Analisi.Data_Richiesta_Analisi,'') as Data_Richiesta_Analisi ")
            StrSQL.AppendLine(" , isnull(PDC_Analisi.Altre_Molecole,'') as Altre_Molecole, isnull(PDC_Analisi.Altre_Molecole_Cod,'') as Altre_Molecole_Cod, isnull( PDC_Analisi.Mostra_in_Stampe ,'') as Mostra_In_Stampe,  isnull(PDC_Analisi.Note_Richiesta_Analisi,'') as Note_Richiesta_Analisi ")
            StrSQL.AppendLine(" , isnull( Analisi_Tipologia.Analisi_Tipologia_Tipo,'-1') Analisi_Tipologia_Tipo , PDC_Stato_Analisi.PDC_Stato_Analisi_Des , ISNULL(Analisi_Tipologia.Analisi_Tipologia_Des,'') Analisi_Tipologia_Des, Contatti.Rag_Soc as Cod_Risum_Des , Contatti.Cod_Contatto , Analisi_Testata.Analisi_Testata_Des ")
            StrSQL.AppendLine(" , isnull(PDC_FornitoreFatturazione.Piva,'') as Piva_FornitoreFatturazione  ,isnull(PDC_FornitoreFatturazione.ragSoc,'') as FornitoreFatturazione_Des     ")
            StrSQL.AppendLine(" , isnull( Tipo_Campione_Apofruit,'')as Tipo_Campione_cod , isnull( (select top 1 tipo_campione from PDC_Tipo_Campione_Apofruit where codice = Tipo_Campione_Apofruit),'') as Tipo_Campione_Des, PDC_Analisi.ID_NC ")

            StrSQL.AppendLine(" FROM         Contatti INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            StrSQL.AppendLine("   INNER JOIN PDC_Analisi ")
            StrSQL.AppendLine("   INNER JOIN PDC_Stato_Analisi ON PDC_Analisi.PDC_Stato_Analisi = PDC_Stato_Analisi.ID_PDC_Stato_Analisi ")
            StrSQL.AppendLine("   left JOIN Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Tipologia.PivaSuperUser ON Risorse_Umane.Cod_RisUm = PDC_Analisi.Cod_Risum ")
            StrSQL.AppendLine("   INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser ")
            StrSQL.AppendLine("   left JOIN PDC_FornitoreFatturazione ON PDC_Analisi.Piva_FornitoreFatturazione = PDC_FornitoreFatturazione.Piva  ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '10/05/2021 Giulia: Se l'utente ha finestra temporale, all'interno del modulo dei pdc le analisi si devono vedere a prescindere,
            '   quindi elimino i filtri sulle date di esecuzione delle analisi
            'StrSQL.AppendLine(" AND Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            'StrSQL.AppendLine(" AND   Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If
            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If
            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ''' <summary>
    ''' Anna 22/07/21: Aggiunta switch per mostrare dinamicamente Risultati Analisi
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Leggi_ParametriPossibili_Analisi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_ParametriPossibili_Analisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable


        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT Analisi_Testata_Tipo, Analisi_Dettagli.Analisi_Parametro_Cod")
            StrSQL.AppendLine(" , 'AnaParam_' + CAST(Analisi_Dettagli.Analisi_Parametro_Cod AS VARCHAR(10)) AS Analisi_Parametro_Sigla")
            StrSQL.AppendLine(" , Analisi_Parametri.Analisi_Parametro_Des")

            StrSQL.AppendLine(" FROM PDC_Analisi")

            StrSQL.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser")
            StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod")
            StrSQL.AppendLine(" INNER JOIN Analisi_Dettagli ON Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser")
            StrSQL.AppendLine(" AND Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod")
            StrSQL.AppendLine(" INNER JOIN Analisi_Parametri ON Analisi_Dettagli.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod")

            StrSQL.AppendLine(" WHERE Analisi_Testata.Analisi_Testata_Tipo NOT IN (8)")

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

    Public Function LeggiAnalisixUtente(ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal laboratori As String = Nothing,
                                        Optional ByVal leggiRisultati As Boolean = False,
                                        Optional ByVal Da_Zoo As Boolean = False,
                                        Optional ByVal IncludiCorrezioni As Boolean = False
                                        ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisixUtente()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dtPivot As DataTable
        Dim dtPivotCorrezioni As DataTable
        Dim stringColonnePivot As String = ""
        Dim stringColonnePivotCorrezioni As String = ""

        Try
            'Anna 22 / 7 / 21: Aggiunta Switch per mostrare dinamicamente Risultati Analisi
            If leggiRisultati Then
                If Da_Zoo Then
                    dtPivot = Leggi_ParametriPossibili_AnalisiZoo(0, "", objParametri)
                Else
                    dtPivot = Leggi_ParametriPossibili_Analisi(objParametri)
                End If

                If dtPivot IsNot Nothing AndAlso dtPivot.Rows.Count > 0 Then
                    For Each Parametro In dtPivot.Rows
                        stringColonnePivot &= "[" & Parametro.Item("Analisi_Parametro_Cod") & "],"
                    Next
                    If stringColonnePivot.EndsWith(",") Then
                        stringColonnePivot = stringColonnePivot.Substring(0, (stringColonnePivot.Length - 1))
                    End If
                End If

                If IncludiCorrezioni Then

                    If Da_Zoo Then
                        dtPivotCorrezioni = Leggi_ParametriCorrezioniValorizzate_AnalisiZoo("", objParametri)


                        If dtPivotCorrezioni IsNot Nothing AndAlso dtPivotCorrezioni.Rows.Count > 0 Then
                            For Each ParametroCorrezione In dtPivotCorrezioni.Rows
                                stringColonnePivotCorrezioni &= "[" & ParametroCorrezione.Item("Analisi_Parametro_Cod") & "],"
                            Next
                            If stringColonnePivotCorrezioni.EndsWith(",") Then
                                stringColonnePivotCorrezioni = stringColonnePivotCorrezioni.Substring(0, (stringColonnePivotCorrezioni.Length - 1))
                            End If
                        End If

                    End If

                End If

            End If

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT PDC_Analisi.ID_PDC_Dettagli, PDC_Analisi.ID_PDC_Campione, PDC_Analisi.Analisi_Testata_Cod, analisi_testata_des, analisi_tipologia_des, PDC_Analisi.PDC_Stato_Analisi, ")
            StrSQL.AppendLine("     PDC_Analisi.PDC_Stato_Validazione, PDC_Analisi.PDC_Stato_Pubblicazione, PDC_Analisi.Cod_Risum, PDC_Analisi.Analisi_Tipologia_Cod, REPLACE(PDC_Analisi.Altre_Molecole,'___',', ') AS Altre_Molecole, ")
            StrSQL.AppendLine("     PDC_Analisi.Data_Richiesta_Analisi, CONVERT (varchar(10) ,PDC_Analisi.Data_Richiesta_Analisi,103) as Data_Richiesta_Analisi_STR, ")
            StrSQL.AppendLine("     CASE CONVERT (varchar(255), PDC_Campioni.Codice_Campione) WHEN '' THEN CONVERT (varchar(255),PDC_Analisi.ID_PDC_Campione) ELSE isnull( PDC_Campioni.Codice_Campione,PDC_Analisi.ID_PDC_Campione) End as Codice_Campione, ")
            StrSQL.AppendLine("     Data_Campionamento, CONVERT (varchar(10) ,PDC_Campioni.Data_Campionamento,103 ) as Data_Campionamento_STR, ")
            StrSQL.AppendLine("     PDC_Analisi.ID_PDC_Testata, Contatti.Rag_Soc, PDC_Campioni.Note_Campione, PDC_Analisi.Analisi_Tipologia_Tipo, Analisi_Testata_Note1, ")
            StrSQL.AppendLine("     Analisi_Testata_Data_Inizio as Data_Inizio_Analisi, CONVERT (varchar(10), Analisi_Testata_Data_Inizio, 103) as Data_Inizio_Analisi_STR, ")
            StrSQL.AppendLine("     CASE WHEN Analisi_Testata_Data_Fine = CAST('2100-12-31' AS Date) THEN NULL ELSE Analisi_Testata_Data_Fine END AS Data_Fine_Analisi, CONVERT (varchar(10),  ")
            StrSQL.AppendLine("     Analisi_Testata_Data_Fine, 103) as Data_Fine_Analisi_STR ")
            StrSQL.AppendLine("     , PDC_Dettagli.Username_Creazione, Utenti.[USER] ")

            If Da_Zoo Then
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Cod_Animale, 0) AS Cod_Animale")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Cod_Progetto, 0) AS Cod_Progetto")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.GEN_COD, 0) as GEN_COD")
                StrSQL.AppendLine("     , ISNULL(Lista_Generi_Animali.GEN_DES, '') as Genere")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.SPE_COD, '') as SPE_COD")
                StrSQL.AppendLine("     , ISNULL(Lista_Specie_Animali.SPE_DES, '') as Specie")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.RAZ_COD, '') as RAZ_COD")
                StrSQL.AppendLine("     , ISNULL(Lista_Razze_Animali.RAZ_DES, '') as Razza")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Data_Nascita, '') as Data_Nascita")
                StrSQL.AppendLine("     , CASE WHEN ISNULL(PDC_Dettagli.Data_Nascita, '') <> '' THEN DATEDIFF(day, PDC_Dettagli.Data_Nascita, PDC_Testata.PDC_Data_Istantanea) ELSE 0 END AS Giorni_Vita")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Sesso, '') as Sesso")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Matricola, '') AS Matricola")
                StrSQL.AppendLine("     , SUBSTRING(ISNULL(PDC_Dettagli.Matricola, ''), LEN(TRIM(PDC_Dettagli.Matricola)) - 5, LEN(TRIM(PDC_Dettagli.Matricola))) AS MatricolaBreve")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Lotto, '') as Lotto")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Raggruppamento_Cod, 0) as Raggruppamento_Cod")
                StrSQL.AppendLine("     , ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento")
                StrSQL.AppendLine("     , sa.PDC_Stato_Analisi_Des")
                StrSQL.AppendLine("     , ISNULL(BDN_Codice_Azienda, '') as BDN_Codice_Azienda")
                StrSQL.AppendLine("     , STA_DES")
                StrSQL.AppendLine("     , Imprese.rag_soc as ImpresaRagSoc")
            Else
                StrSQL.AppendLine("     , ISNULL(PDC_Campioni.Tipo_Campione,0) AS Tipo_Campione, PDC_Dettagli.ID_LFO, PDC_Dettagli.Rag_Soc AS Rag_Soc_Azienda, PDC_Dettagli.Sup_Imp ")
                StrSQL.AppendLine("     , ISNULL(i_padre.rag_soc,'') AS Rag_Soc_Padre ")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Grower_Number,'') AS Grower_Number ")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Block,'') AS Block ")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Kpin,'') AS Kpin ")
                StrSQL.AppendLine("     , ISNULL(Descrizione_MotivoCampionamento,'') AS Motivo_Campione ")
                StrSQL.AppendLine("     , ISNULL(Descrizione_PuntoDiPrelievo,'') AS Punto_Prelievo ")
                StrSQL.AppendLine("     , PDC_Dettagli.Tecnico_Campionamento ")
                StrSQL.AppendLine("     , PDC_Dettagli.Tecnico_Campionamento_Tel ")

                'Colonna Progetto_Nome per Lotto Impianto
                StrSQL.AppendLine("     , PDC_Dettagli.Progetto_Nome ")

                'Colonne X e Y per Lat e Long Appezzamento
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.App_Lat, 0) AS App_Lat ")
                StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.App_Long, 0) AS App_Long ")

                'Colonne X e Y per Lat e Long Campione
                StrSQL.AppendLine("     , ISNULL(PDC_Campioni.X, 0) AS Lat_Campione ")
                StrSQL.AppendLine("     , ISNULL(PDC_Campioni.Y, 0) AS Long_Campione ")

                ' Anna 21/07/21 - Aggiunte info extra: Nazione, Regione, Città + Latest Sample
                StrSQL.AppendLine("     , UPPER(ISNULL(PDC_Dettagli.Stato, '')) AS Nazione, ISNULL(r.Regione_Des, '') AS Regione, ISNULL(PDC_Dettagli.Frz_Des, '') AS Citta ")
            End If

            StrSQL.AppendLine("     , ISNULL(PDC_Testata.PDC_Testata_Des,'') AS PDC_Testata_Des ")

            If Not Da_Zoo Then
                StrSQL.AppendLine("     , CASE (ROW_NUMBER() OVER(PARTITION BY PDC_Analisi.ID_PDC_Testata, PDC_Analisi.ID_PDC_Dettagli, PDC_Analisi.Analisi_Tipologia_Tipo ORDER BY PDC_Analisi.Data_Richiesta_Analisi DESC)) ")
                StrSQL.AppendLine("        WHEN 1 THEN  'YES' ELSE 'NO' END AS Latest_Sample  ")
            End If

            'Anna 22 / 7 / 21: Aggiunta Switch per mostrare dinamicamente Risultati Analisi
            If leggiRisultati Then
                If Not String.IsNullOrEmpty(stringColonnePivot) Then
                    For Each Parametro In dtPivot.Rows
                        StrSQL.AppendLine("     , piv.[" & Parametro.item("Analisi_Parametro_Cod") & "] AS " & Parametro.item("Analisi_Parametro_Sigla") & "  ")
                    Next

                    If IncludiCorrezioni Then
                        'Correzioni in Coda
                        If Not String.IsNullOrEmpty(stringColonnePivotCorrezioni) Then
                            For Each Parametro In dtPivotCorrezioni.Rows
                                StrSQL.AppendLine("     , pivCorr.[" & Parametro.item("Analisi_Parametro_Cod") & "] AS Corr" + Parametro.item("Analisi_Parametro_Sigla") & "  ")
                            Next
                        End If
                    End If
                End If

                If Not Da_Zoo Then
                    StrSQL.AppendLine("     , ISNULL(prA.MultiResidue, '') AS MultiResidue ")
                End If
            End If

            StrSQL.AppendLine(" FROM       PDC_Analisi ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Tipologia on Analisi_Tipologia.Analisi_Tipologia_Cod = PDC_Analisi.Analisi_Tipologia_Cod ")
            StrSQL.AppendLine(" INNER JOIN Analisi_testata on Analisi_testata.analisi_testata_cod = PDC_Analisi.analisi_testata_cod ")

            If laboratori Is Nothing Then
                StrSQL.AppendLine(" INNER JOIN Laboratori_Utenti ON PDC_Analisi.PivaSuperUser = Laboratori_Utenti.PivaSuperUser ")
                StrSQL.AppendLine("        AND PDC_Analisi.Cod_Risum = Laboratori_Utenti.Cod_Risum ")
            End If

            StrSQL.AppendLine(" INNER JOIN PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser ")
            StrSQL.AppendLine("        AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata And PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")
            StrSQL.AppendLine("        AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")

            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser ")
            StrSQL.AppendLine("        AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" INNER JOIN Utenti ON PDC_Dettagli.Username_Creazione = Utenti.CODICE_FISCALE ")
            StrSQL.AppendLine(" LEFT JOIN PDC_MotivoCampionamento ON PDC_MotivoCampionamento.ID_MotivoCampionamento = PDC_Campioni.Motivo_Campione ")
            StrSQL.AppendLine(" LEFT JOIN PDC_PuntoDiPrelievo ON PDC_PuntoDiPrelievo.id_puntodiprelievo = PDC_Campioni.PuntoPrelievo ")

            'Colonne Progetto_Cod e Progetto_Nome per Lotto Impianto
            'StrSQL.AppendLine(" LEFT JOIN Imprese_Progetti ON Imprese_Progetti.Piva = PDC_Dettagli.Piva ")
            'StrSQL.AppendLine("     AND Imprese_Progetti.Sa_Cod = PDC_Dettagli.Sa_Cod ")
            'StrSQL.AppendLine("     AND Imprese_Progetti.Appezza = PDC_Dettagli.Appezza ")
            'StrSQL.AppendLine("     AND Imprese_Progetti.Id_Reg = PDC_Dettagli.Id_Reg ")

            'JOIN Appezzamento per ricavare Lat (X) e Long (Y)
            StrSQL.AppendLine(" LEFT JOIN Appezzamento (NOLOCK) ON PDC_Dettagli.Piva = Appezzamento.Piva ")
            StrSQL.AppendLine("     AND PDC_Dettagli.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine("     AND PDC_Dettagli.Appezza = Appezzamento.Appezza ")

            If Da_Zoo Then
                StrSQL.AppendLine("  LEFT JOIN Lista_Generi_Animali ON	")
                StrSQL.AppendLine("     Lista_Generi_Animali.GEN_COD = PDC_Dettagli.GEN_COD 	")
                StrSQL.AppendLine(" LEFT JOIN Lista_Specie_Animali ON")
                StrSQL.AppendLine("	Lista_Specie_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
                StrSQL.AppendLine("	AND Lista_Specie_Animali.SPE_COD = PDC_Dettagli.SPE_COD")
                StrSQL.AppendLine(" LEFT JOIN Lista_Razze_Animali ON ")
                StrSQL.AppendLine("	Lista_Razze_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
                StrSQL.AppendLine("	AND Lista_Razze_Animali.SPE_COD = PDC_Dettagli.SPE_COD")
                StrSQL.AppendLine("	AND Lista_Razze_Animali.RAZ_COD = PDC_Dettagli.RAZ_COD")
                StrSQL.AppendLine(" LEFT JOIN Stalla_Raggruppamenti ON")
                StrSQL.AppendLine("	Stalla_Raggruppamenti.Raggruppamento_Cod = PDC_Dettagli.Raggruppamento_Cod")
                StrSQL.AppendLine(" LEFT JOIN PDC_Stato_Analisi sa on sa.ID_PDC_Stato_Analisi = PDC_Analisi.PDC_Stato_Analisi")
                StrSQL.AppendLine(" LEFT JOIN Stalla ON Stalla_Raggruppamenti.PIVA = Stalla.PIVA AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
                StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ON Stalla.PIVA = Centri_Aziendali.PIVA AND Stalla.sa_cod = Centri_Aziendali.sa_cod")
                StrSQL.AppendLine(" LEFT JOIN Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA")
            Else
                StrSQL.AppendLine(" LEFT JOIN GerarchiaImprese as ger ON ger.Figlio = PDC_Dettagli.Piva ")
                StrSQL.AppendLine(" LEFT JOIN Imprese as i_padre ON i_padre.Piva = ger.Padre ")
                ' Anna 21/07/21 - Aggiunte info extra: Nazione, Regione, Città e Piano Produttivo
                StrSQL.AppendLine(" LEFT JOIN Lista_Regioni r ON PDC_Dettagli.Reg = r.REG AND PDC_Dettagli.Reg <> '000'")
            End If

            StrSQL.AppendLine(" LEFT JOIN PDC_Testata ON PDC_Analisi.PivaSuperUser = PDC_Testata.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Testata.Id_PDC_Testata ")

            If leggiRisultati Then

                If Not String.IsNullOrEmpty(stringColonnePivot) Then
                    StrSQL.AppendLine(" LEFT JOIN (")
                    StrSQL.AppendLine("     SELECT * FROM (")
                    StrSQL.AppendLine("         SELECT Analisi_Dettagli.Analisi_SuperUser, Analisi_Dettagli.Analisi_Testata_Cod, Analisi_Testata_Tipo, Analisi_Parametro_Cod, Analisi_Dettaglio_Valore_1")
                    StrSQL.AppendLine("         FROM Analisi_Dettagli")
                    StrSQL.AppendLine("         INNER JOIN Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod")
                    StrSQL.AppendLine("         WHERE Analisi_Testata_Tipo NOT IN (" & enum_AnalisiTipo.Analisi_Fitofarmaci & ") AND Analisi_Testata_Tipo IN (" & If(Da_Zoo, enum_AnalisiTipo.Analisi_Del_Sangue, enum_AnalisiTipo.Analisi_Merceologiche) & ")")
                    StrSQL.AppendLine("     ) As SourceTable")

                    StrSQL.AppendLine("     PIVOT( MAX(Analisi_Dettaglio_Valore_1)")
                    StrSQL.AppendLine("     FOR Analisi_Parametro_Cod IN (" & stringColonnePivot & ")) AS PivotTable")
                    StrSQL.AppendLine(" ) piv ON Analisi_Testata.Analisi_SuperUser = piv.Analisi_SuperUser and Analisi_Testata.Analisi_Testata_Cod = piv.Analisi_Testata_Cod AND Analisi_Testata.Analisi_Testata_Tipo = piv.Analisi_Testata_Tipo ")

                    If IncludiCorrezioni And Not String.IsNullOrEmpty(stringColonnePivotCorrezioni) Then
                        'Inserimento Correzioni
                        StrSQL.AppendLine(" LEFT JOIN (")
                        StrSQL.AppendLine("     SELECT * FROM (")
                        StrSQL.AppendLine("         SELECT Analisi_Dettagli.Analisi_SuperUser, Analisi_Dettagli.Analisi_Testata_Cod, Analisi_Testata_Tipo, Analisi_Parametro_Cod, Analisi_Dettaglio_Correzione_1")
                        StrSQL.AppendLine("         FROM Analisi_Dettagli")
                        StrSQL.AppendLine("         INNER JOIN Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod")
                        StrSQL.AppendLine("         WHERE Analisi_Testata_Tipo NOT IN (" & enum_AnalisiTipo.Analisi_Fitofarmaci & ") AND Analisi_Testata_Tipo IN (" & If(Da_Zoo, enum_AnalisiTipo.Analisi_Del_Sangue, enum_AnalisiTipo.Analisi_Merceologiche) & ")")
                        StrSQL.AppendLine("     ) As SourceTable")

                        StrSQL.AppendLine("     PIVOT( MAX(Analisi_Dettaglio_Correzione_1)")
                        StrSQL.AppendLine("     FOR Analisi_Parametro_Cod IN (" & stringColonnePivotCorrezioni & ")) AS PivotTable")
                        StrSQL.AppendLine(" ) pivCorr ON Analisi_Testata.Analisi_SuperUser = pivCorr.Analisi_SuperUser and Analisi_Testata.Analisi_Testata_Cod = pivCorr.Analisi_Testata_Cod AND Analisi_Testata.Analisi_Testata_Tipo = pivCorr.Analisi_Testata_Tipo ")
                    End If

                End If

                If Not Da_Zoo Then
                    StrSQL.AppendLine(" LEFT JOIN (")
                    StrSQL.AppendLine("     SELECT Analisi_SuperUser, Analisi_Testata_Cod, Analisi_Testata_Tipo,")
                    StrSQL.AppendLine("     LEFT(r.ResourceName , LEN(r.ResourceName)-1) MultiResidue")
                    StrSQL.AppendLine("     FROM Analisi_Testata AS e")
                    StrSQL.AppendLine("     CROSS APPLY")
                    StrSQL.AppendLine("     (")
                    StrSQL.AppendLine("         SELECT (COALESCE(Pa_Des, FamigliePrincipiAttivi.Descrizione, '') + ' > ' + CAST(d.Analisi_Dettaglio_Valore_1 AS VARCHAR(10))) + ' ; '")
                    StrSQL.AppendLine("         FROM Analisi_Dettagli d")
                    StrSQL.AppendLine("         LEFT JOIN PrincipiAttivi ON d.Analisi_Parametro_Cod = PrincipiAttivi.Pa_Cod")
                    StrSQL.AppendLine("         LEFT JOIN FamigliePrincipiAttivi ON CAST(d.Analisi_Parametro_Cod AS VARCHAR(10)) = '-'+(FamigliePrincipiAttivi.Fam_Cod)")
                    StrSQL.AppendLine("         --INNER JOIN Analisi_Testata t ON d.Analisi_SuperUser = t.Analisi_SuperUser AND d.Analisi_Testata_Cod = t.Analisi_Testata_Cod")
                    StrSQL.AppendLine("         WHERE e.Analisi_SuperUser = d.Analisi_SuperUser AND e.Analisi_Testata_Cod = d.Analisi_Testata_Cod")
                    StrSQL.AppendLine("         --AND t.Analisi_Testata_Tipo IN (" & enum_AnalisiTipo.Analisi_Fitofarmaci & ")    ")
                    StrSQL.AppendLine("         FOR XML PATH('')")
                    StrSQL.AppendLine("     ) r (ResourceName)")
                    StrSQL.AppendLine("     WHERE Analisi_Testata_Tipo IN (" & enum_AnalisiTipo.Analisi_Fitofarmaci & ")")
                    StrSQL.AppendLine(" ) prA ON Analisi_Testata.Analisi_SuperUser = prA.Analisi_SuperUser and Analisi_Testata.Analisi_Testata_Cod = prA.Analisi_Testata_Cod AND Analisi_Testata.Analisi_Testata_Tipo = prA.Analisi_Testata_Tipo ")
                End If
            End If

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If laboratori Is Nothing Then
                StrSQL.AppendLine(" AND Laboratori_Utenti.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            ElseIf laboratori <> "" Then
                StrSQL.AppendLine(" AND PDC_Analisi.Cod_Risum IN (" & Agro_SQL_Save_Clausola_IN(laboratori) & ") ")
            End If

            StrSQL.AppendLine(" AND Risorse_Umane.Cod_Rapporto =  " & enum_Rapporti_Contabili_Standard.Laboratorio_Analisi & "  ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiSoloAbilitatexUtente(ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiSoloAbilitatexUtente()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT     PDC_Analisi.ID_PDC_Dettagli, PDC_Analisi.ID_PDC_Campione, PDC_Analisi.Analisi_Testata_Cod, analisi_testata_des, analisi_tipologia_des, ")
            StrSQL.AppendLine("     PDC_Analisi.PDC_Stato_Analisi, PDC_Analisi.Cod_Risum, PDC_Analisi.Analisi_Tipologia_Cod, ")
            StrSQL.AppendLine("     CONVERT (varchar(10) ,PDC_Analisi.Data_Richiesta_Analisi,103 ) as Data_Richiesta_Analisi, ")
            StrSQL.AppendLine("     PDC_Analisi.Altre_Molecole, ")
            StrSQL.AppendLine("     CASE CONVERT (varchar(255), PDC_Campioni.Codice_Campione) WHEN '' THEN CONVERT (varchar(255),PDC_Analisi.ID_PDC_Campione) ELSE isnull( PDC_Campioni.Codice_Campione,PDC_Analisi.ID_PDC_Campione) End as Codice_Campione, ")
            StrSQL.AppendLine("     CONVERT (varchar(10) ,PDC_Campioni.Data_Campionamento,103 ) as Data_Campionamento, ")
            StrSQL.AppendLine("     PDC_Analisi.ID_PDC_Testata, Contatti.Rag_Soc, PDC_Campioni.Note_Campione, PDC_Analisi.Analisi_Tipologia_Tipo, Analisi_Testata_Note1, ")
            StrSQL.AppendLine("     CONVERT (varchar(10) ,Analisi_Testata_Data_Inizio, 103) as Data_Inizio_Analisi, CONVERT (varchar(10) ,Analisi_Testata_Data_Fine, 103) as Data_Fine_Analisi,")

            'Grilli: 30/09/2019 Aggiunto per Jingold il codice degli Appezzamenti
            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine(" WHEN Tipo_Campione = 1 THEN ")
            StrSQL.AppendLine(" (Select SUBSTRING(  ")
            StrSQL.AppendLine(" 	(SELECT ', ' + ISNULL(val_cod,'') FROM PDC_Dettagli detImp ")
            StrSQL.AppendLine(" 	INNER JOIN PDC_Dettagli detLFO ON detImp.ID_LFO=detLFO.ID_LFO  ")
            StrSQL.AppendLine(" 	INNER JOIN Appezzamento_Codici ac ON ac.piva=detLFO.Piva AND ac.sa_cod=detLFO.Sa_Cod AND ac.appezza=detLFO.Appezza ")
            StrSQL.AppendLine(" 	WHERE ac.id_cod=1104 AND detImp.ID_PDC_Testata=PDC_Campioni.ID_PDC_Testata AND detImp.ID_PDC_Dettagli=PDC_Campioni.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" 	ORDER BY val_cod ")
            StrSQL.AppendLine(" 	FOR XML PATH('') ) ")
            StrSQL.AppendLine(" 	, 3 , 9999) ) ")
            StrSQL.AppendLine(" WHEN Tipo_Campione = 0 THEN ")
            StrSQL.AppendLine(" 	(SELECT TOP 1 ISNULL(val_cod,'') FROM Appezzamento_Codici ac ")
            StrSQL.AppendLine(" 	INNER JOIN PDC_Dettagli det ON ac.piva=det.Piva AND ac.sa_cod=det.Sa_Cod AND ac.appezza=det.Appezza  ")
            StrSQL.AppendLine(" 	WHERE ac.id_cod=1104 AND det.ID_PDC_Testata=PDC_Campioni.ID_PDC_Testata AND det.ID_PDC_Dettagli=PDC_Campioni.ID_PDC_Dettagli) ")
            StrSQL.AppendLine(" ELSE ")
            StrSQL.AppendLine(" 	'Errore: Tipo Campione non supportato' ")
            StrSQL.AppendLine(" END AS ListaCodiciApp ")

            StrSQL.AppendLine(" FROM         PDC_Analisi INNER JOIN ")
            StrSQL.AppendLine("     Analisi_Tipologia on Analisi_Tipologia.Analisi_Tipologia_Cod = PDC_Analisi.Analisi_Tipologia_Cod inner join ")
            StrSQL.AppendLine("     Analisi_testata on Analisi_testata.analisi_testata_cod =PDC_Analisi.analisi_testata_cod INNER JOIN ")
            StrSQL.AppendLine("     Laboratori_Utenti ON PDC_Analisi.PivaSuperUser = Laboratori_Utenti.PivaSuperUser AND ")
            StrSQL.AppendLine("     PDC_Analisi.Cod_Risum = Laboratori_Utenti.Cod_Risum INNER JOIN ")

            StrSQL.AppendLine("     PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND ")
            StrSQL.AppendLine("     PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata And PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli And ")
            StrSQL.AppendLine("     PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione INNER JOIN ")
            StrSQL.AppendLine("     Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm INNER JOIN ")
            StrSQL.AppendLine("     Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND Laboratori_Utenti.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.AppendLine(" AND Risorse_Umane.Cod_Rapporto = -8 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_Conformita_DP(ByVal ID_PDC_Testata As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_Conformita_DP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Analisi_Conformita_DP ")

            If ID_PDC_Testata <> "" Then
                StrSQL.AppendLine(" where Analisi_Testata_Cod in (" & Agro_SQL_Save_Clausola_IN(ID_PDC_Testata) & ") ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_Conformita_Capitolato_Cliente(ByVal ListaIn_ID_PDC_Testata As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_Conformita_Capitolato_Cliente()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Analisi_Conformita_Capitolato_Cliente ")

            If ListaIn_ID_PDC_Testata <> "" Then
                StrSQL.AppendLine(" where Analisi_Testata_Cod in (" & Agro_SQL_Save_Clausola_IN(ListaIn_ID_PDC_Testata) & ") ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_Conformita_Capitolato_Cliente_join_PDCAnalisi(ByVal ID_PDC_Testata As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                        ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_Conformita_Capitolato_Cliente_join_PDCAnalisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT     Analisi_Conformita_Capitolato_Cliente.PivaSuperUser, Analisi_Conformita_Capitolato_Cliente.Analisi_Testata_Cod, ")
            StrSQL.AppendLine(" Analisi_Conformita_Capitolato_Cliente.CapitolatoCliente_Cod, Analisi_Conformita_Capitolato_Cliente.Esito, ")
            StrSQL.AppendLine(" Analisi_Conformita_Capitolato_Cliente.Descrizione_Esito, Analisi_Conformita_Capitolato_Cliente.Capitolato_Des ")
            StrSQL.AppendLine(" FROM         Analisi_Conformita_Capitolato_Cliente INNER JOIN ")
            StrSQL.AppendLine(" PDC_Analisi ON Analisi_Conformita_Capitolato_Cliente.PivaSuperUser = PDC_Analisi.PivaSuperUser AND  ")
            StrSQL.AppendLine(" Analisi_Conformita_Capitolato_Cliente.Analisi_Testata_Cod = PDC_Analisi.Analisi_Testata_Cod ")

            If ID_PDC_Testata <> "" Then
                StrSQL.AppendLine(" where ID_PDC_Testata = " & ID_PDC_Testata & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiTutteAnalisi(ByVal Cod_Risum As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiTutteAnalisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT      PDC_Analisi.ID_PDC_Campione,PDC_Analisi.Analisi_Testata_Cod, PDC_Analisi.PDC_Stato_Analisi, PDC_Analisi.Cod_Risum,  ")
            StrSQL.AppendLine("             PDC_Analisi.Data_Richiesta_Analisi, Analisi_Testata.Analisi_Testata_Des,  ")
            StrSQL.AppendLine("             CASE convert(nvarchar(255),PDC_Campioni.Codice_Campione) WHEN '' THEN convert(nvarchar(255),PDC_Analisi.ID_PDC_Campione) ELSE isnull( PDC_Campioni.Codice_Campione,PDC_Analisi.ID_PDC_Campione) End as Codice_Campione      ")

            StrSQL.AppendLine("         FROM         PDC_Analisi INNER JOIN ")
            StrSQL.AppendLine("               Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND ")
            StrSQL.AppendLine("               PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser INNER JOIN ")
            StrSQL.AppendLine("               PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND ")
            StrSQL.AppendLine("               PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata And PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli And ")
            StrSQL.AppendLine("               PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione ")


            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.AppendLine(" AND PDC_Analisi.PDC_Stato_Analisi IN (-1," &
                              enum_PDC_Stato_Analisi.Analisi_In_Corso & "," &
                              enum_PDC_Stato_Analisi.Analizzata & "," &
                              enum_PDC_Stato_Analisi.Da_Inviare & ") ")

            If Cod_Risum > 0 Then
                StrSQL.AppendLine(" AND (PDC_Analisi.Cod_Risum = " & Agro_SQL_SaveNum(Cod_Risum) & ")")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiAnalisi_e_PDC_Campioni(ByVal Analisi_Testata_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine As String = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisi_e_PDC_Campioni()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT      PDC_Analisi.ID_PDC_Campione,  PDC_Analisi.Analisi_Testata_Cod, PDC_Analisi.PDC_Stato_Analisi, PDC_Analisi.Cod_Risum, PDC_Analisi.Data_Richiesta_Analisi, Analisi_Testata.Analisi_Testata_Des, ")
            StrSQL.AppendLine(" Analisi_Testata_Data_Inizio, Analisi_Testata_Data_Fine, Analisi_Tipologia_Cod, altre_Molecole ")

            StrSQL.AppendLine(" FROM         PDC_Analisi INNER JOIN ")
            StrSQL.AppendLine("         Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND   ")
            StrSQL.AppendLine("         PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser INNER JOIN  ")
            StrSQL.AppendLine("         PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND   ")
            StrSQL.AppendLine("         PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata And PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli And ")
            StrSQL.AppendLine("         PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione  ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Analisi_Testata_Cod > 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_x_controlloDP(ByVal ID_PDC_Testata As Integer,
                                        ByVal in_Analisi_Testata_Cod As String,
                                        ByVal data_x_Distinta_attiva As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal dettagliAggiunti As Boolean = False
                                        ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_x_controlloDP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine("SELECT PDC_Analisi.Analisi_Testata_Cod, Analisi_Testata.Analisi_Testata_Tipo, ")

            If dettagliAggiunti Then
                StrSQL.AppendLine("       ISNULL(PDC_Campioni.PuntoPrelievo, 0) AS PuntoPrelievo, ")
            End If

            StrSQL.AppendLine("       Cultivar.Veg_Cod, ")
            StrSQL.AppendLine("       Analisi_Dettagli.Analisi_Parametro_Cod, Analisi_Dettagli.Analisi_Dettaglio_Valore_1, ")
            StrSQL.AppendLine("       Analisi_Dettagli.Analisi_Dettaglio_Valore_2, Analisi_Dettagli.Analisi_Dettaglio_MargineErrore_1, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.Disciplinare_Cod,'0') AS Disciplinare_Cod, ")
            StrSQL.AppendLine("       Analisi_Testata.Analisi_Testata_Data_Inizio, ")


            'MULTIPLE CUL_COD
            StrSQL.AppendLine("  (  SELECT LEFT(Colonna_descrizioni, LEN(Colonna_Descrizioni)- 1 ) ")
            StrSQL.AppendLine("     FROM ( ")
            StrSQL.AppendLine("        SELECT ( ")
            StrSQL.AppendLine("           SELECT CONVERT (nvarchar(10), pd.Cul_Cod, 103) + ',' AS [text()] ")
            StrSQL.AppendLine("           FROM PDC_LFO p ")
            StrSQL.AppendLine("           INNER JOIN PDC_Dettagli pd ON p.PivaSuperUser = pd.PivaSuperUser AND p.ID_PDC_Testata = pd.ID_PDC_Testata AND p.ID_LFO = pd.ID_LFO ")
            StrSQL.AppendLine("           WHERE(PDC_Dettagli.ID_PDC_Testata = p.ID_PDC_Testata) AND PDC_Analisi.PivaSuperUser = p.PivaSuperUser AND PDC_Dettagli.id_lfo = p.ID_LFO ")
            StrSQL.AppendLine("           GROUP BY pd.Cul_Cod FOR XML PATH('') ")
            StrSQL.AppendLine("        ) Colonna_descrizioni ")
            StrSQL.AppendLine("     ) Tabella_descrizioni ")
            StrSQL.AppendLine("   ) AS Cul_Cod, ")

            StrSQL.AppendLine(" PDC_Dettagli.Piva as PivaImpianto ")

            StrSQL.AppendLine(" FROM PDC_Dettagli ")
            StrSQL.AppendLine(" INNER JOIN PDC_Analisi ON PDC_Dettagli.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND  PDC_Dettagli.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli  ")

            If dettagliAggiunti Then
                StrSQL.AppendLine(" LEFT JOIN PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND  PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione ")
            End If

            StrSQL.AppendLine(" INNER JOIN Cultivar ON PDC_Dettagli.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod  ")
            StrSQL.AppendLine(" LEFT JOIN Analisi_Dettagli ON Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod ")


            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            StrSQL.AppendLine(" AND PDC_Analisi.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod IN (" & Agro_SQL_Save_Clausola_IN(in_Analisi_Testata_Cod) & ") ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PDC_Analisi.Analisi_Testata_Cod ")
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

    Public Function Leggi_VarietaQuestaAnalisi(ByVal ID_PDC_Testata As Integer,
                                               ByVal ID_PDC_Dettagli As Integer,
                                               ByVal ID_PDC_Campione As Integer,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_VarietaQuestaAnalisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT     Cultivar.Cul_Des, SpecieVegetali.Veg_Des, PDC_Dettagli.ID_LFO, Cultivar.Cul_Cod, SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" FROM         PDC_Campioni INNER JOIN ")
            StrSQL.AppendLine("   PDC_Dettagli ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND  ")
            StrSQL.AppendLine("   PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli INNER JOIN ")
            StrSQL.AppendLine("   SpecieVegetali ON PDC_Dettagli.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.AppendLine("   Cultivar ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod AND PDC_Dettagli.Veg_Cod = Cultivar.Veg_Cod ")

            StrSQL.AppendLine(" WHERE PDC_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If


            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
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

    Public Function Leggi_AltreVarietaLotto(ByVal ID_PDC_Testata As Integer,
                                            ByVal ID_LFO As Integer,
                                            ByVal Cul_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_AltreVarietaLotto()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT DISTINCT Cultivar.Cul_Des  , Cultivar.Cul_Cod  ")
            StrSQL.AppendLine(" FROM         PDC_Dettagli INNER JOIN ")
            StrSQL.AppendLine("       Cultivar ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod AND PDC_Dettagli.Veg_Cod = Cultivar.Veg_Cod ")

            StrSQL.AppendLine(" WHERE PDC_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_LFO <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_LFO = " & Agro_SQL_SaveNum(ID_LFO))
            End If

            If Cul_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Cul_Cod <>" & Agro_SQL_SaveNum(Cul_Cod))
            End If

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Testata   = " & Agro_SQL_SaveNum(ID_PDC_Testata))
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

    Public Function Leggi_PADESoDESCRIZIONE_Valore(ByVal Analisi_Testata_Cod As Integer,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_PADESoDESCRIZIONE_Valore()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" ( ")

            StrSQL.AppendLine(" select (select pa_des from principiAttivi where analisi_parametro_cod = pa_cod) as nome , analisi_dettaglio_valore_1 ")
            StrSQL.AppendLine(" , (select udm_sim from UnitaMisura where udm_cod =  analisi_dettaglio_valore_2 ) as analisi_dettaglio_valore_2 ")
            StrSQL.AppendLine(" from Analisi_Dettagli ")
            StrSQL.AppendLine(" where analisi_parametro_cod > 0 ")
            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            StrSQL.AppendLine(" ) union ( ")

            'StrSQL.AppendLine(" select (select descrizione from (select * from famiglieprincipiattivi where ISNUMERIC(fam_cod) =1 ) f  ")
            'StrSQL.AppendLine(" , (select udm_sim from UnitaMisura where udm_cod =  analisi_dettaglio_valore_2 ) as analisi_dettaglio_valore_2 ")
            'StrSQL.AppendLine(" where fam_cod = (0 - fam_cod)) as nome , analisi_dettaglio_valore_1 ")
            'StrSQL.AppendLine(" from Analisi_Dettagli ")
            'StrSQL.AppendLine(" where analisi_parametro_cod < 0 ")

            StrSQL.Append("select ( " & vbCrLf)
            StrSQL.Append("                select descrizione  " & vbCrLf)
            StrSQL.Append("                from ( " & vbCrLf)
            StrSQL.Append("                    select *  " & vbCrLf)
            StrSQL.Append("                    from famiglieprincipiattivi  " & vbCrLf)
            StrSQL.Append("                    where isnumeric (fam_cod) =1 " & vbCrLf)
            StrSQL.Append("                ) f where analisi_parametro_cod = (0 - fam_cod) " & vbCrLf)
            StrSQL.Append("          ) as nome  " & vbCrLf)

            StrSQL.Append("         , analisi_dettaglio_valore_1  " & vbCrLf)

            StrSQL.Append("         , ( " & vbCrLf)
            StrSQL.Append("            select udm_sim  " & vbCrLf)
            StrSQL.Append("            from UnitaMisura  " & vbCrLf)
            StrSQL.Append("            where udm_cod =  " & vbCrLf)
            StrSQL.Append("            analisi_dettaglio_valore_2 " & vbCrLf)
            StrSQL.Append("        ) as analisi_dettaglio_valore_2  " & vbCrLf)

            StrSQL.Append("      from Analisi_Dettagli       " & vbCrLf)
            StrSQL.Append("     where analisi_parametro_cod < 0  " & vbCrLf)
            StrSQL.Append(" ")


            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            StrSQL.AppendLine(" )  ")


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

    Public Function LeggiAnalisixPivotZoo(piva As String, sa_cod As Integer, sta_num As Integer,
                                          data_da As Date, data_a As Date,
                                          xFiltroAggiuntivo As String, xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisixPivotZoo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dtPivot As DataTable
        Dim stringColonnePivot As String = ""

        Dim dtPivotCorrezioni As DataTable
        Dim stringColonnePivotCorrezioni As String = ""

        Try

            dtPivot = Leggi_ParametriPossibili_AnalisiZoo(0, "", objParametri)
            If dtPivot IsNot Nothing AndAlso dtPivot.Rows.Count > 0 Then
                For Each Parametro In dtPivot.Rows
                    stringColonnePivot &= "[" & Parametro.Item("Analisi_Parametro_Cod") & "],"
                Next
                If stringColonnePivot.EndsWith(",") Then
                    stringColonnePivot = stringColonnePivot.Substring(0, (stringColonnePivot.Length - 1))
                End If
            End If

            dtPivotCorrezioni = Leggi_ParametriCorrezioniValorizzate_AnalisiZoo("", objParametri)
            If dtPivotCorrezioni IsNot Nothing AndAlso dtPivotCorrezioni.Rows.Count > 0 Then
                For Each ParametroCorrezione In dtPivotCorrezioni.Rows
                    stringColonnePivotCorrezioni &= "[" & ParametroCorrezione.Item("Analisi_Parametro_Cod") & "],"
                Next
                If stringColonnePivotCorrezioni.EndsWith(",") Then
                    stringColonnePivotCorrezioni = stringColonnePivotCorrezioni.Substring(0, (stringColonnePivotCorrezioni.Length - 1))
                End If
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("    PDC_Testata.PivaOwner, PDC_Testata.Sa_CodOwner, PDC_Testata.Fabbricato_CodOwner,")
            StrSQL.AppendLine("	 Imprese.rag_soc AS Impresa, Centri_Aziendali.sa_nome AS Centro, Fabbricati.Fabbricato_Des AS Stalla")

            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Cod_Animale, 0) AS Cod_Animale")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Cod_Progetto, 0) AS Cod_Progetto")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.GEN_COD, 0) as GEN_COD")
            StrSQL.AppendLine("     , ISNULL(Lista_Generi_Animali.GEN_DES, '') as Genere")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.SPE_COD, '') as SPE_COD")
            StrSQL.AppendLine("     , ISNULL(Lista_Specie_Animali.SPE_DES, '') as Specie")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.RAZ_COD, '') as RAZ_COD")
            StrSQL.AppendLine("     , ISNULL(Lista_Razze_Animali.RAZ_DES, '') as Razza")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Data_Nascita, '') as Data_Nascita")
            StrSQL.AppendLine("     , CASE WHEN ISNULL(PDC_Dettagli.Data_Nascita, '') <> '' THEN DATEDIFF(day, PDC_Dettagli.Data_Nascita, PDC_Testata.PDC_Data_Istantanea) ELSE 0 END AS Giorni_Vita")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Sesso, '') as Sesso")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Matricola, '') AS Matricola")
            StrSQL.AppendLine("     , ISNULL(RIGHT(PDC_Dettagli.Matricola, 4), '') AS MatricolaBreve")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Lotto, '') as Lotto")
            StrSQL.AppendLine("     , ISNULL(PDC_Dettagli.Raggruppamento_Cod, 0) as Raggruppamento_Cod")
            StrSQL.AppendLine("     , ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento")

            StrSQL.AppendLine("     , ISNULL(PDC_Testata.PDC_Testata_Des, 'Piano Campionamento') + ' del '+ CONVERT(varchar,  FORMAT (PDC_Testata.PDC_Data_Istantanea, 'dd/MM/yyyy')) AS Piano_Campionamento ")
            StrSQL.AppendLine("     , PDC_Analisi.Analisi_Tipologia_Tipo")
            StrSQL.AppendLine("     , Analisi_Tipologia.Analisi_Tipologia_Des")
            StrSQL.AppendLine("     , PDC_Stato_Analisi.PDC_Stato_Analisi_Des")

            StrSQL.AppendLine("     , PDC_Campioni.ID_PDC_Campione")
            StrSQL.AppendLine("     , PDC_Campioni.Codice_Campione")

            If Not String.IsNullOrEmpty(stringColonnePivot) Then
                For Each Parametro In dtPivot.Rows
                    StrSQL.AppendLine("     , piv.[" & Parametro.item("Analisi_Parametro_Cod") & "] AS " & Parametro.item("Analisi_Parametro_Sigla") & "  ")
                Next
            End If

            If Not String.IsNullOrEmpty(stringColonnePivotCorrezioni) Then
                For Each Parametro In dtPivotCorrezioni.Rows
                    StrSQL.AppendLine("     , pivCorr.[" & Parametro.item("Analisi_Parametro_Cod") & "] AS Corr" + Parametro.item("Analisi_Parametro_Sigla") & "  ")
                Next
            End If

            StrSQL.AppendLine(" FROM PDC_Analisi ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Tipologia ON Analisi_Tipologia.Analisi_Tipologia_Cod = PDC_Analisi.Analisi_Tipologia_Cod ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Testata ON Analisi_Testata.analisi_testata_cod = PDC_Analisi.analisi_testata_cod ")

            StrSQL.AppendLine(" INNER JOIN PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser ")
            StrSQL.AppendLine("        AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata And PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")
            StrSQL.AppendLine("        AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione ")

            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser ")
            StrSQL.AppendLine("        AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")

            StrSQL.AppendLine("  LEFT JOIN Lista_Generi_Animali ON	")
            StrSQL.AppendLine("     Lista_Generi_Animali.GEN_COD = PDC_Dettagli.GEN_COD 	")
            StrSQL.AppendLine(" LEFT JOIN Lista_Specie_Animali ON")
            StrSQL.AppendLine("	Lista_Specie_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
            StrSQL.AppendLine("	AND Lista_Specie_Animali.SPE_COD = PDC_Dettagli.SPE_COD")
            StrSQL.AppendLine(" LEFT JOIN Lista_Razze_Animali ON ")
            StrSQL.AppendLine("	Lista_Razze_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
            StrSQL.AppendLine("	AND Lista_Razze_Animali.SPE_COD = PDC_Dettagli.SPE_COD")
            StrSQL.AppendLine("	AND Lista_Razze_Animali.RAZ_COD = PDC_Dettagli.RAZ_COD")
            StrSQL.AppendLine(" LEFT JOIN Stalla_Raggruppamenti ON")
            StrSQL.AppendLine("	Stalla_Raggruppamenti.Raggruppamento_Cod = PDC_Dettagli.Raggruppamento_Cod")

            StrSQL.AppendLine(" LEFT JOIN PDC_Stato_Analisi PDC_Stato_Analisi on PDC_Stato_Analisi.ID_PDC_Stato_Analisi = PDC_Analisi.PDC_Stato_Analisi")

            StrSQL.AppendLine(" LEFT JOIN PDC_Testata ON PDC_Analisi.PivaSuperUser = PDC_Testata.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Testata.Id_PDC_Testata ")

            StrSQL.AppendLine(" LEFT JOIN Imprese ON Imprese.PIVA = PDC_Testata.PivaOwner ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ON Centri_Aziendali.PIVA = PDC_Testata.PivaOwner AND Centri_Aziendali.sa_cod = PDC_Testata.Sa_CodOwner ")
            StrSQL.AppendLine(" LEFT JOIN Fabbricati ON Fabbricati.PIVA = PDC_Testata.PivaOwner AND Fabbricati.sa_cod = PDC_Testata.Sa_CodOwner AND Fabbricati.Fabbricato_Cod = PDC_Testata.Fabbricato_CodOwner")

            If Not String.IsNullOrEmpty(stringColonnePivot) Then
                StrSQL.AppendLine(" LEFT JOIN (")
                StrSQL.AppendLine("     SELECT * FROM (")
                StrSQL.AppendLine("         SELECT Analisi_Dettagli.Analisi_SuperUser, Analisi_Dettagli.Analisi_Testata_Cod, Analisi_Testata_Tipo, Analisi_Parametro_Cod, Analisi_Dettaglio_Valore_1")
                StrSQL.AppendLine("         FROM Analisi_Dettagli")
                StrSQL.AppendLine("         INNER JOIN Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod")
                StrSQL.AppendLine("         WHERE Analisi_Testata_Tipo IN (" & enum_AnalisiTipo.Analisi_Del_Sangue & ")")
                StrSQL.AppendLine("     ) As SourceTable")

                StrSQL.AppendLine("     PIVOT( MAX(Analisi_Dettaglio_Valore_1)")
                StrSQL.AppendLine("     FOR Analisi_Parametro_Cod IN (" & stringColonnePivot & ")) AS PivotTable")
                StrSQL.AppendLine(" ) piv ON Analisi_Testata.Analisi_SuperUser = piv.Analisi_SuperUser and Analisi_Testata.Analisi_Testata_Cod = piv.Analisi_Testata_Cod AND Analisi_Testata.Analisi_Testata_Tipo = piv.Analisi_Testata_Tipo ")
            End If

            If Not String.IsNullOrEmpty(stringColonnePivotCorrezioni) Then
                'Inserimento Correzioni
                StrSQL.AppendLine(" LEFT JOIN (")
                StrSQL.AppendLine("     SELECT * FROM (")
                StrSQL.AppendLine("         SELECT Analisi_Dettagli.Analisi_SuperUser, Analisi_Dettagli.Analisi_Testata_Cod, Analisi_Testata_Tipo, Analisi_Parametro_Cod, Analisi_Dettaglio_Correzione_1")
                StrSQL.AppendLine("         FROM Analisi_Dettagli")
                StrSQL.AppendLine("         INNER JOIN Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod")
                StrSQL.AppendLine("         WHERE Analisi_Testata_Tipo IN (" & enum_AnalisiTipo.Analisi_Del_Sangue & ")")
                StrSQL.AppendLine("     ) As SourceTable")

                StrSQL.AppendLine("     PIVOT( MAX(Analisi_Dettaglio_Correzione_1)")
                StrSQL.AppendLine("     FOR Analisi_Parametro_Cod IN (" & stringColonnePivotCorrezioni & ")) AS PivotTable")
                StrSQL.AppendLine(" ) pivCorr ON Analisi_Testata.Analisi_SuperUser = pivCorr.Analisi_SuperUser and Analisi_Testata.Analisi_Testata_Cod = pivCorr.Analisi_Testata_Cod AND Analisi_Testata.Analisi_Testata_Tipo = pivCorr.Analisi_Testata_Tipo ")
            End If

            StrSQL.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND  PDC_Analisi.PDC_Stato_Analisi <> " & enum_PDC_Stato_Analisi.Da_Inviare & " ")
            StrSQL.AppendLine(" AND  PDC_Testata.Da_Zoo = 1")

            StrSQL.AppendLine(" AND PDC_Testata.PivaOwner = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(" AND PDC_Testata.Sa_CodOwner = " & Agro_SQL_SaveNum(sa_cod) & "")
            If sta_num <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata.Fabbricato_CodOwner = " & Agro_SQL_SaveNum(sta_num) & "")
            End If

            StrSQL.AppendLine(" AND PDC_Testata.PDC_Data_Istantanea >= CAST(" & Agro_SQL_SaveDate(data_da) & " AS Date )")
            StrSQL.AppendLine(" AND PDC_Testata.PDC_Data_Istantanea <= CAST(" & Agro_SQL_SaveDate(data_a) & " AS Date )")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiAnalisixMailZoo(ID_PDC_Testata As Integer,
                                         xFiltroAggiuntivo As String,
                                         xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.LeggiAnalisixMail()"

        'vanni, 22/04/2013 corretto baco su tabella contatto in join piuttosto che impresa
        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            stb.Length = 0
            '---------------------------------------------

            stb.AppendLine(" SELECT ")
            stb.AppendLine("          ISNULL(PDC_Campioni.Codice_Campione, '') AS Codice_Campione  ")

            stb.AppendLine("        , ISNULL(Imprese.Rag_Soc, '') AS Rag_Soc  ")
            stb.AppendLine("        , ISNULL(Stalla.Sta_DES , '') AS Fabbricato_Des  ")
            stb.AppendLine("        , ISNULL(Stalla.BDN_Codice_Azienda , '') AS BDN_Codice_Azienda ")
            stb.AppendLine("        , ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des , '') AS Raggruppamento_Des  ")
            stb.AppendLine("        , ISNULL(PDC_Dettagli.Matricola, '') AS Matricola  ")
            stb.AppendLine("        , PDC_Analisi.Analisi_Testata_Cod  ")

            stb.AppendLine(" FROM PDC_Dettagli ")
            stb.AppendLine(" INNER JOIN PDC_Testata ON pdc_dettagli.id_pdc_testata = PDC_Testata.id_pdc_testata ")
            stb.AppendLine(" INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND   PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli     ")
            stb.AppendLine(" INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND   PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione     ")
            stb.AppendLine(" INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser       ")

            stb.AppendLine(" LEFT JOIN Imprese ON PDC_Testata.PivaOwner = Imprese.Piva")
            stb.AppendLine(" LEFT JOIN Stalla ON PDC_Testata.PivaOwner = Stalla.Piva AND PDC_Testata.Sa_CodOwner = Stalla.Sa_Cod AND PDC_Testata.Fabbricato_CodOwner = Stalla.STA_NUM	")
            stb.AppendLine(" LEFT JOIN Stalla_Raggruppamenti ON Stalla_Raggruppamenti.Raggruppamento_Cod = PDC_Dettagli.Raggruppamento_Cod ")

            stb.AppendLine("  " & vbCrLf)
            stb.AppendLine(" WHERE PDC_Analisi.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            stb.AppendLine(" AND PDC_Analisi.PDC_Stato_Analisi = " & Agro_SQL_SaveNum(enum_PDC_Stato_Analisi.Analisi_In_Corso))

            If ID_PDC_Testata <> 0 Then
                stb.AppendLine(" AND PDC_Analisi.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function Leggi_ParametriPossibili_AnalisiZoo(ID_PDC_Testata As Integer,
                                                        xFiltroAggiuntivo As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_ParametriPossibili_AnalisiZoo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable


        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" WITH ParametrixAnalisi AS ( ")
            StrSQL.AppendLine("     SELECT DISTINCT ")
            StrSQL.AppendLine("         Analisi_Testata.Analisi_Testata_Tipo,  Analisi_Dettagli.Analisi_Parametro_Cod as Analisi_Parametro_Cod ")
            StrSQL.AppendLine("     FROM PDC_Analisi (NOLOCK) ")
            StrSQL.AppendLine("     INNER JOIN Analisi_Testata (NOLOCK) ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser AND PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
            StrSQL.AppendLine("     INNER JOIN Analisi_Dettagli (NOLOCK) ON Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod ")
            StrSQL.AppendLine("     WHERE Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Del_Sangue & " ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine("     AND PDC_Analisi.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            End If
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine("      ParametrixAnalisi.Analisi_Testata_Tipo, ParametrixAnalisi.Analisi_Parametro_Cod")
            StrSQL.AppendLine("    , 'AnaParam_' + CAST(ParametrixAnalisi.Analisi_Parametro_Cod AS VARCHAR(10)) AS Analisi_Parametro_Sigla")
            StrSQL.AppendLine("    , Analisi_Parametri.Analisi_Parametro_Des")
            StrSQL.AppendLine("    , Analisi_Parametri.Analisi_Parametro_Simbolo COLLATE SQL_Latin1_General_CP1_CI_AS + COALESCE(' (' + um.udm_sim COLLATE SQL_Latin1_General_CP1_CI_AS + ')', '') as Descrizione")

            StrSQL.AppendLine(" FROM ParametrixAnalisi")
            StrSQL.AppendLine(" INNER JOIN Analisi_Parametri ON ParametrixAnalisi.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod")
            StrSQL.AppendLine(" LEFT JOIN UnitaMisura um ON um.UDM_COD = Analisi_Parametri.Analisi_Parametro_UdM ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    Public Function Leggi_ParametriCorrezioniValorizzate_AnalisiZoo(xFiltroAggiuntivo As String,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                    Optional fromReport As Boolean = False
                                                                    ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_R.Leggi_ParametriCorrezioniValorizzate_AnalisiZoo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable


        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" WITH ParametrixAnalisi AS ( ")
            StrSQL.AppendLine("     SELECT DISTINCT ")
            StrSQL.AppendLine("         Analisi_Testata.Analisi_Testata_Tipo, Analisi_Dettagli.Analisi_Parametro_Cod as Analisi_Parametro_Cod ")
            StrSQL.AppendLine("     FROM PDC_Analisi (NOLOCK) ")
            StrSQL.AppendLine("     INNER Join Analisi_Testata (NOLOCK) ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser And PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
            StrSQL.AppendLine("     INNER Join Analisi_Dettagli (NOLOCK) ON Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser And Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod And Analisi_Dettaglio_Correzione_1 <> 0  ")
            StrSQL.AppendLine("     WHERE 1 = 1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            StrSQL.AppendLine(" )")

            StrSQL.AppendLine(" SELECT  ")
            If fromReport Then
                StrSQL.AppendLine("      ParametrixAnalisi.Analisi_Parametro_Cod")
                StrSQL.AppendLine("    , 'CorrAnaParam_' + CAST(ParametrixAnalisi.Analisi_Parametro_Cod AS VARCHAR(10)) AS Analisi_Parametro_Sigla")
                StrSQL.AppendLine("    , Analisi_Parametri.Analisi_Parametro_Des")
                StrSQL.AppendLine("    , Analisi_Parametri.Analisi_Parametro_Simbolo COLLATE SQL_Latin1_General_CP1_CI_AS + COALESCE(' (' + um.udm_sim COLLATE SQL_Latin1_General_CP1_CI_AS + ')', '') AS Descrizione")
            Else
                StrSQL.AppendLine("     ParametrixAnalisi.Analisi_Parametro_Cod, ")
                StrSQL.AppendLine("     Analisi_Parametri.Analisi_Parametro_Des, Analisi_Parametri.Analisi_Parametro_Simbolo, ")
                StrSQL.AppendLine("     'AnaParam_' + CAST(Analisi_Parametri.Analisi_Parametro_Cod AS VARCHAR(10)) AS Analisi_Parametro_Sigla ")
            End If
            StrSQL.AppendLine(" FROM ParametrixAnalisi")
            StrSQL.AppendLine(" JOIN Analisi_Parametri On ParametrixAnalisi.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod ")

            If fromReport Then
                StrSQL.AppendLine(" LEFT JOIN UnitaMisura um ON um.UDM_COD = Analisi_Parametri.Analisi_Parametro_UdM ")
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


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_Analisi_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrivi"

    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal ID_PDC_Dettagli As Integer,
                           ByVal ID_PDC_Campione As Integer,
                           ByVal Analisi_Testata_Cod As Integer,
                           ByVal PDC_Stato_Analisi As Integer,
                           ByVal Cod_Risum As Integer,
                           ByVal Analisi_Tipologia_Cod As Integer,
                           ByVal Altre_Molecole As String,
                           ByVal Data_Richiesta_Analisi As Date,
                           ByVal Note_Richiesta_Analisi As String,
                           ByVal Piva_FornitoreFatturazione As String,
                           ByVal tipo_campione_apofruit As String,
                           ByVal mostra_in_stampe As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal Username_Creazione As String = "",
                           Optional ByVal Username_Modifica As String = "",
                           Optional ByVal Analisi_Tipologia_Tipo As Integer = Nothing,
                           Optional ByVal Altre_Molecole_Cod As String = "",
                           Optional ByVal PDC_Stato_Validazione As Integer = 0,
                           Optional ByVal PDC_Stato_Pubblicazione As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If Username_Creazione = "" Then
            Username_Creazione = objParametri.UsernameOperazione
        End If

        If Username_Modifica = "" Then
            Username_Modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Analisi(PivaSuperUser , ID_PDC_Testata, ID_PDC_Dettagli, ID_PDC_Campione, ")
            StrSQL.AppendLine("         Analisi_Testata_Cod, PDC_Stato_Analisi, Cod_Risum, Analisi_Tipologia_Cod, Altre_Molecole, ")
            StrSQL.AppendLine("         Data_Richiesta_Analisi, Note_Richiesta_Analisi, Piva_FornitoreFatturazione, ")
            StrSQL.AppendLine("         Tipo_Campione_Apofruit, Mostra_in_Stampe, ")
            StrSQL.AppendLine("         Analisi_Tipologia_Tipo, Altre_Molecole_Cod, PDC_Stato_Validazione, PDC_Stato_Pubblicazione ")
            StrSQL.AppendLine(" , Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica ")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Campione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod))

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(PDC_Stato_Analisi))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Risum))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Altre_Molecole) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Richiesta_Analisi) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Note_Richiesta_Analisi) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva_FornitoreFatturazione) & "' ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(tipo_campione_apofruit) & " ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(mostra_in_stampe))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Tipologia_Tipo))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Altre_Molecole_Cod) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(PDC_Stato_Validazione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(PDC_Stato_Pubblicazione))

            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(Username_Modifica) & "' ")

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

    Public Function Scrivi_Conformita_DP(ByVal ID_DP As Integer,
                                         ByVal Analisi_Testata_Cod As Integer,
                                         ByVal Privato_Pubblico As Integer,
                                         ByVal Esito As Integer,
                                         ByVal Descrizione_Esito As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Scrivi_Conformita_DP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Analisi_Conformita_DP(PivaSuperUser,Analisi_Testata_Cod, Id_DP, Privato_Pubblico, ")
            StrSQL.AppendLine("         Descrizione_Esito, Esito")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_DP))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Privato_Pubblico))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Descrizione_Esito) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Esito))
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

    '<Obsolete("ATTENZIONE: è duplicata rispetto a quella presente nel proprio file, all'interno di questo stesso Core --> TODO: Togliere da qui!!!")>
    'Public Function Scrivi_Conformita_Capitolato_Cliente(ByVal CapitolatoCliente_Cod As Integer,
    '                                                     ByVal Analisi_Testata_Cod As Integer,
    '                                                     ByVal Esito As Integer,
    '                                                     ByVal Descrizione_Esito As String,
    '                                                     ByVal Capitolato_Des As String,
    '                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                                     ) As Boolean

    '    Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Scrivi_Conformita_Capitolato_Cliente()"

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.AppendLine("INSERT INTO Analisi_Conformita_Capitolato_Cliente(PivaSuperUser,Analisi_Testata_Cod, CapitolatoCliente_Cod, Esito, ")
    '        StrSQL.AppendLine("         Descrizione_Esito, Capitolato_Des ")
    '        StrSQL.AppendLine("  ) ")

    '        StrSQL.AppendLine("  VALUES (")
    '        StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
    '        StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
    '        StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
    '        StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Esito))
    '        StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Descrizione_Esito) & "'")
    '        StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Capitolato_Des) & "'")
    '        StrSQL.AppendLine(")")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function

#End Region

#Region "Modifica"

    <Obsolete("Usare PDC_Analisi_W.ModificaPuntuale")>
    Public Function Modifica(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByVal ID_PDC_Campione As Integer,
                             ByVal Analisi_Testata_Cod As Integer,
                             ByVal PDC_Stato_Analisi As Integer,
                             ByVal Cod_Risum As Integer,
                             ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal Altre_Molecole As String,
                             ByVal Data_Richiesta_Analisi As Date,
                             ByVal Note_Richiesta_Analisi As String,
                             ByVal Piva_FornitoreFatturazione As String,
                             ByVal Tipo_Campione_Apofruit As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Analisi SET ")

            StrSQL.AppendLine("   Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now) & "   ")
            StrSQL.AppendLine("   , Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")

            If PDC_Stato_Analisi > 0 Then
                StrSQL.AppendLine("   , PDC_Stato_Analisi   =  " & Agro_SQL_SaveNum(PDC_Stato_Analisi) & "   ")
            End If

            If Cod_Risum > 0 Then
                StrSQL.AppendLine("   , Cod_Risum   =  " & Agro_SQL_SaveNum(Cod_Risum) & "   ")
            End If

            'If Altre_Molecole <> "" Then
            StrSQL.AppendLine("   , Altre_Molecole   =  '" & Agro_SQL_SaveText(Altre_Molecole) & "'   ")
            'End If

            If Data_Richiesta_Analisi <> New Date Then
                StrSQL.AppendLine("   , Data_Richiesta_Analisi   =  " & Agro_SQL_SaveDate(Data_Richiesta_Analisi) & "   ")
            End If

            If Analisi_Tipologia_Cod > 0 Then
                StrSQL.AppendLine("   , Analisi_Tipologia_Cod   =  " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & "   ")
            End If

            StrSQL.AppendLine("   ,Note_Richiesta_Analisi   =  '" & Agro_SQL_SaveText(Note_Richiesta_Analisi) & "'   ")
            StrSQL.AppendLine("   ,Piva_FornitoreFatturazione   =  '" & Agro_SQL_SaveText(Piva_FornitoreFatturazione) & "'   ")

            If Tipo_Campione_Apofruit <> 0 Then
                StrSQL.AppendLine("   ,Tipo_Campione_Apofruit   =  " & Agro_SQL_SaveNum(Tipo_Campione_Apofruit) & "   ")
            End If

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If ID_PDC_Testata > 0 Then
                StrSQL.AppendLine("   and ID_PDC_Testata   =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            End If
            If ID_PDC_Dettagli > 0 Then
                StrSQL.AppendLine("   and ID_PDC_Dettagli   =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")
            End If
            If ID_PDC_Campione > 0 Then
                StrSQL.AppendLine("   and ID_PDC_Campione   =  " & Agro_SQL_SaveNum(ID_PDC_Campione) & "   ")
            End If
            If Analisi_Testata_Cod > 0 Then
                StrSQL.AppendLine("   and Analisi_Testata_Cod   =  " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "   ")
            End If
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    <Obsolete("Usare PDC_Analisi_W.ModificaPuntuale")>
    Public Function ModificaID_NC_PDCAnalisi(ByVal PivaSuperUser As String,
                                             ByVal ID_PDC_Testata As Integer,
                                             ByVal ID_PDC_Dettagli As Integer,
                                             ByVal ID_PDC_Campione As Integer,
                                             ByVal Analisi_Testata_Cod As Integer,
                                             ByVal ID_NC As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As Boolean


        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.ModificaID_NC_PDCAnalisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE PDC_Analisi ")
            StrSQL.AppendLine(" SET ID_NC = " & Agro_SQL_SaveNum(ID_NC) & " ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(" AND PivaSuperUser   =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND ID_PDC_Testata   =  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine(" AND ID_PDC_Dettagli   =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            StrSQL.AppendLine(" AND ID_PDC_Campione   =  " & Agro_SQL_SaveNum(ID_PDC_Campione))
            StrSQL.AppendLine(" AND Analisi_Testata_Cod   =  " & Agro_SQL_SaveNum(Analisi_Testata_Cod))

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

    <Obsolete("Usare PDC_Analisi_W.ModificaPuntuale")>
    Public Function Modifica_Mostra_in_Stampe(ByVal ID_PDC_Testata As Integer,
                                              ByVal ID_PDC_Dettagli As Integer,
                                              ByVal ID_PDC_Campione As Integer,
                                              ByVal Analisi_Testata_Cod As Integer,
                                              ByVal Mostra_in_Stampe As Integer,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Modifica_Mostra_in_Stampe()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Analisi SET ")

            StrSQL.AppendLine("   Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now) & "   ")
            StrSQL.AppendLine("   ,Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")


            StrSQL.AppendLine("   ,Mostra_in_Stampe   =  " & Agro_SQL_SaveNum(Mostra_in_Stampe) & "   ")


            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            If ID_PDC_Testata > 0 Then
                StrSQL.AppendLine("   and ID_PDC_Testata   =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            End If
            If ID_PDC_Dettagli > 0 Then
                StrSQL.AppendLine("   and ID_PDC_Dettagli   =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")
            End If
            If ID_PDC_Campione > 0 Then
                StrSQL.AppendLine("   and ID_PDC_Campione   =  " & Agro_SQL_SaveNum(ID_PDC_Campione) & "   ")
            End If
            If Analisi_Testata_Cod > 0 Then
                StrSQL.AppendLine("   and Analisi_Testata_Cod   =  " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "   ")
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

    Public Function ModificaPuntuale(ByVal ID_PDC_Testata As Integer,
                                     ByVal ID_PDC_Dettagli As Integer,
                                     ByVal ID_PDC_Campione As Integer,
                                     ByVal Analisi_Testata_Cod As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal PDC_Stato_Analisi As Integer? = Nothing,
                                     Optional ByVal Cod_Risum As Integer? = Nothing,
                                     Optional ByVal Analisi_Tipologia_Cod As Integer? = Nothing,
                                     Optional ByVal Data_Richiesta_Analisi As Date? = Nothing,
                                     Optional ByVal Altre_Molecole As String = Nothing,
                                     Optional ByVal Mostra_In_Stampe As Integer? = Nothing,
                                     Optional ByVal Piva_FornitoreFatturazione As String = Nothing,
                                     Optional ByVal Note_Richiesta_Analisi As String = Nothing,
                                     Optional ByVal Tipo_Campione_Apofruit As String = Nothing,
                                     Optional ByVal ID_NC As Integer? = Nothing,
                                     Optional ByVal Analisi_Tipologia_Tipo As Integer? = Nothing,
                                     Optional ByVal Altre_Molecole_Cod As String = Nothing,
                                     Optional ByVal PDC_Stato_Validazione As Integer? = Nothing,
                                     Optional ByVal PDC_Stato_Pubblicazione As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Analisi_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE PDC_Analisi ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(PDC_Stato_Analisi) Then
                strSql.AppendLine("   , PDC_Stato_Analisi = " & Agro_SQL_SaveNum(PDC_Stato_Analisi) & " ")
            End If

            If Not IsNothing(Cod_Risum) Then
                strSql.AppendLine("   , Cod_Risum = " & Agro_SQL_SaveNum(Cod_Risum) & " ")
            End If

            If Not IsNothing(Analisi_Tipologia_Cod) Then
                strSql.AppendLine("   , Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            End If

            If Not IsNothing(Data_Richiesta_Analisi) Then
                strSql.AppendLine("   , Data_Richiesta_Analisi = " & Agro_SQL_SaveDate(Data_Richiesta_Analisi) & " ")
            End If

            If Not IsNothing(Altre_Molecole) Then
                strSql.AppendLine("   , Altre_Molecole = '" & Agro_SQL_SaveText(Altre_Molecole) & "' ")
            End If

            If Not IsNothing(Mostra_In_Stampe) Then
                strSql.AppendLine("   , Mostra_In_Stampe = " & Agro_SQL_SaveNum(Mostra_In_Stampe) & " ")
            End If

            If Not IsNothing(Piva_FornitoreFatturazione) Then
                strSql.AppendLine("   , Piva_FornitoreFatturazione = '" & Agro_SQL_SaveText(Piva_FornitoreFatturazione) & "' ")
            End If

            If Not IsNothing(Note_Richiesta_Analisi) Then
                strSql.AppendLine("   , Note_Richiesta_Analisi = '" & Agro_SQL_SaveText(Note_Richiesta_Analisi) & "' ")
            End If

            If Not IsNothing(Tipo_Campione_Apofruit) Then
                strSql.AppendLine("   , Tipo_Campione_Apofruit = '" & Agro_SQL_SaveText(Tipo_Campione_Apofruit) & "' ")
            End If

            If Not IsNothing(ID_NC) Then
                strSql.AppendLine("   , ID_NC = " & Agro_SQL_SaveNum(ID_NC) & " ")
            End If

            If Not IsNothing(Analisi_Tipologia_Tipo) Then
                strSql.AppendLine("   , Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(Analisi_Tipologia_Tipo) & " ")
            End If

            If Not IsNothing(Altre_Molecole_Cod) Then
                strSql.AppendLine("   , Altre_Molecole_Cod = '" & Agro_SQL_SaveText(Altre_Molecole_Cod) & "' ")
            End If

            If Not IsNothing(PDC_Stato_Validazione) Then
                strSql.AppendLine("   , PDC_Stato_Validazione = " & Agro_SQL_SaveNum(PDC_Stato_Validazione) & " ")
            End If

            If Not IsNothing(PDC_Stato_Pubblicazione) Then
                strSql.AppendLine("   , PDC_Stato_Pubblicazione = " & Agro_SQL_SaveNum(PDC_Stato_Pubblicazione) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE PivaSuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                strSql.AppendLine(" AND   ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            End If

            If ID_PDC_Dettagli <> 0 Then
                strSql.AppendLine(" AND   ID_PDC_Dettagli =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & " ")
            End If

            If ID_PDC_Campione <> 0 Then
                strSql.AppendLine(" AND   ID_PDC_Campione =  " & Agro_SQL_SaveNum(ID_PDC_Campione) & " ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                strSql.AppendLine(" AND   Analisi_Testata_Cod =  " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            ' aggiorna stato pubblicazione solo per gli stati consentiti
            If Not IsNothing(PDC_Stato_Pubblicazione) Then
                Dim stato As Integer = -1
                If PDC_Stato_Pubblicazione = enum_PDC_Stato_Pubblicazione.Da_Pubblicare Then
                    stato = enum_PDC_Stato_Pubblicazione.Non_Pubblicata
                ElseIf PDC_Stato_Pubblicazione = enum_PDC_Stato_Pubblicazione.Non_Pubblicata Then
                    stato = enum_PDC_Stato_Pubblicazione.Da_Pubblicare
                ElseIf PDC_Stato_Pubblicazione = enum_PDC_Stato_Pubblicazione.Da_Rimuovere Then
                    stato = enum_PDC_Stato_Pubblicazione.Pubblicata
                ElseIf PDC_Stato_Pubblicazione = enum_PDC_Stato_Pubblicazione.Pubblicata Then
                    stato = enum_PDC_Stato_Pubblicazione.Da_Rimuovere
                Else
                    Return False
                End If
                strSql.AppendLine(" AND PDC_Stato_Pubblicazione = " & Agro_SQL_SaveNum(stato) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


#End Region

#Region "Cancellazione"

    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByVal ID_PDC_Campione As Integer,
                             ByVal Analisi_Testata_Cod As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Analisi ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine("     AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine("     AND ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine("     AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
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


    Public Function Cancella_Conformita(ByVal Analisi_Testata_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Analisi_W.Cancella_Conformita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM Analisi_Conformita_Capitolato_Cliente ")
            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine("     AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If


            StrSQL.AppendLine("DELETE FROM Analisi_Conformita_DP ")
            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine("     AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
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

#End Region

End Class
