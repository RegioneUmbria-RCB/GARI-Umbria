
Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Sementieri_Sportello_LOG_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggiDaCache(ByVal DatiSerializzati As String) As DataRow

        Dim vDati As String() = DatiSerializzati.Split("§")
        Dim colonne() As String = vDati(0).Split("|")
        ' VAnni: 28/3/2018: Se due impianti sono in interferenza e viene quindi memorizzato il record in [Sementieri_Sportello_InterferenzePerConferma] ed entrambi vengono eliminati allora viene scritto un record sporco nella colonna Descrizione_casella_conflitto
        ' in particolare il record contiene due volte l'intestazione.. in prima battuta seleziono i dati in base al count dello split.
        Dim riga() As String = vDati(vDati.Count - 1).Split("|")

        Dim dt As New DataTable
        Dim dr As DataRow = dt.NewRow
        Dim col As Integer = 0
        While col < colonne.Length

            dt.Columns.Add(colonne(col))

            If col < riga.Length Then
                dr.Item(colonne(col)) = riga(col)
            Else
                dr.Item(colonne(col)) = ""
            End If

            col += 1
        End While

        'Oppure...

        'Dim dt As New DataTable
        'For Each cname In colonne
        '    dt.Columns.Add(cname)
        'Next
        'Dim dr As DataRow = dt.NewRow
        'dr.ItemArray = riga

        Return dr
    End Function


    Public Function Leggi(ByVal Sementieri_Sportello_Configurazione_cod As Integer, ByVal Entita_cod As Integer, ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "Sementieri_Sportello_LOG.Leggi()"
        Dim MessaggioErrore As String = ""

        Dim dt As DataTable
        Dim stb As New StringBuilder

        Try
            Dim nuovaquery As Boolean = True

            If nuovaquery Then

                Dim nomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

                stb.AppendLine("SELECT ")
                stb.AppendLine("    Descrizione_Casella = COALESCE(mylog.Descrizione_casella, '') ")
                stb.AppendLine("    , Veg_DES = COALESCE(veg.Veg_Des, '') ")
                stb.AppendLine("	, NomeScientifico = COALESCE(cla.Sementieri_ClassiDiSpecieVegetaliNomeScientifico_des, '') ")
                stb.AppendLine("    , Rag_SocSementiero = COALESCE(impr.rag_soc, '') ")
                stb.AppendLine("    , ind_des = COALESCE(indi.ind_des, '') ")
                stb.AppendLine("    , frz_des = COALESCE(indi.frz_des, '') ")
                stb.AppendLine("    , CAP = COALESCE(indi.CAP, '') ")
                stb.AppendLine("    , Com_DES = COALESCE(ISTAT.LOCALITA, '') ")
                stb.AppendLine("    , Pro_DES = COALESCE(ISTAT.Comuni_Prov, '') ")
                stb.AppendLine("	, Regione = COALESCE(regio.Regione_Des, '') ")
                stb.AppendLine("    , grva_des = COALESCE(grva.Grva_Des, '') + case when COALESCE(rimp.grva_cod_veg, 0) < 0 then ' (ibrido)' else '' end ")
                stb.AppendLine("    , myLog.TipoOperazione_DB ")
                stb.AppendLine("    , myLog.Flag_attivo ")
                stb.AppendLine("    , Data_log = myLog.Data_Creazione ")
                stb.AppendLine("    , lat = COALESCE(centri.lat, 0) ")
                stb.AppendLine("    , long = COALESCE(centri.long, 0) ")
                stb.AppendLine("    , OperazioneDaEvidenziare = COALESCE(( ")
                stb.AppendLine("                Select top 1 1 ")
                stb.AppendLine("                From Sementieri_Sportello_Passaggi pp ")
                stb.AppendLine("                Inner Join Sementieri_Sportello_ConfigurazioneXPassaggi cp ")
                stb.AppendLine("                    On cp.Sementieri_Sportello_Passaggi_cod = pp.Sementieri_Sportello_Passaggi_cod ")
                stb.AppendLine("                        And cp.Sementieri_Sportello_Configurazione_cod = mylog.Sementieri_Sportello_Configurazione_cod ")
                stb.AppendLine("                        And cp.Data_Inizio <= mylog.Data_Creazione ")
                stb.AppendLine("                        And cp.Data_Fine >= myLog.Data_Creazione ")
                stb.AppendLine("                        And pp.RichiediConfermaSuInterferenze = 1 ")
                stb.AppendLine("        ), 0) ")
                stb.AppendLine("    , myLog.Sementieri_Sportello_LogOperazioni_COD ")
                stb.AppendLine("    , vegImp_veg_cod = COALESCE(veg.veg_cod, -1) ")
                stb.AppendLine("    , vegPlan_veg_cod = -1 ")
                stb.AppendLine("    , imp_grva_veg_cod = COALESCE(rimp.grva_cod_veg, 0) ")
                stb.AppendLine("    , 0 as pl_grva_cod ")
                stb.AppendLine("    , gis.Entita_Cod ")
                stb.AppendLine("    , gis.piva ")
                stb.AppendLine("    , gis.sa_cod ")
                stb.AppendLine("    , PIVA_DittaSementieraReferente = COALESCE(rimp.Codice_Fiscale_Tecnico, '') ")
                stb.AppendLine("	, LayerDesc = COALESCE(layer.Gruppi_Utente_des, '') ")
                stb.AppendLine("	, Via_Stringa = COALESCE(app.Via_Stringa, '') ")
                stb.AppendLine("    , True_Lat = COALESCE(eg.Poligono_GeoEntity.EnvelopeCenter().Lat, 0) ")
                stb.AppendLine("    , True_Lng = COALESCE(eg.Poligono_GeoEntity.EnvelopeCenter().Long, 0) ")
                stb.AppendLine("    , Superficie = COALESCE(rimp.Sup_Imp, 0) ")
                stb.AppendLine()
                stb.AppendLine("FROM Sementieri_Sportello_LogOperazioni As mylog ")
                stb.AppendLine()
                stb.AppendLine("LEFT JOIN GIS_Entita gis ON gis.Entita_Cod = mylog.Entita_Cod_Propietario ")
                stb.AppendLine("LEFT JOIN GIS_ElementiGrafici eg ON eg.PivaSuperUser = gis.PivaSuperUser AND eg.Entita_Cod = gis.Entita_Cod ")
                stb.AppendLine()
                stb.AppendLine("LEFT JOIN Reg_Impianti rimp ON rimp.PIVA = gis.Piva AND rimp.sa_cod = gis.sa_cod AND rimp.appezza = gis.appezza AND rimp.id_reg = gis.Id_Imp ")
                stb.AppendLine("LEFT JOIN Appezzamento app ON app.PIVA = gis.Piva AND app.SA_COD = gis.Sa_Cod AND app.APPEZZA = gis.Appezza ")
                stb.AppendLine("LEFT JOIN Centri_Aziendali centri ON centri.PIVA = gis.Piva AND centri.sa_cod = gis.Sa_Cod ")
                stb.AppendLine("LEFT JOIN Imprese impr ON impr.PIVA = centri.PIVA ")
                stb.AppendLine("LEFT JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente layer ON rimp.Codice_Fiscale_Tecnico = layer.Gruppi_Utente_Identificativo ")
                stb.AppendLine()
                stb.AppendLine("LEFT JOIN CentrixIndirizzi cxi ON cxi.piva = centri.PIVA And cxi.sa_cod = centri.sa_cod ")
                stb.AppendLine("LEFT JOIN Indirizzi indi ON indi.cod_indirizzo = cxi.cod_indirizzo ")
                stb.AppendLine("LEFT JOIN ISTAT ON ISTAT.PROV = indi.pro_cod_istat AND ISTAT.COM = indi.com_cod_istat ")
                stb.AppendLine("LEFT JOIN Lista_Province prov ON prov.SIGLA = ISTAT.COMUNI_PROV ")
                stb.AppendLine("LEFT JOIN Lista_Regioni	regio ON regio.REG = prov.REG ")
                stb.AppendLine()
                stb.AppendLine("LEFT JOIN Cultivar cul ON cul.Cul_Cod = rimp.CUL_COD ")
                stb.AppendLine("LEFT JOIN SpecieVegetali veg ON veg.Veg_Cod = cul.veg_cod ")
                stb.AppendLine("LEFT JOIN GruppoVarietale grva ON grva.grva_cod = abs(rimp.GRVA_Cod_VEG) ")
                stb.AppendLine("LEFT JOIN Mappatura_Specie map ON map.Hybrid = (CASE WHEN veg.Veg_Cod in (6,5000021,70,69) THEN -1 ELSE (CASE WHEN rimp.GRVA_Cod_VEG < 0 THEN 1 ELSE 0 END) END) AND map.Veg_Cod = veg.Veg_Cod AND map.Grva_Cod = ABS(rimp.GRVA_Cod_VEG) ")
                stb.AppendLine("LEFT JOIN Sementieri_ClassiDiSpecieVegetali cla ON cla.Id_Specie = map.ID_Specie AND cla.ID_Sottospecie = map.ID_Sottospecie AND cla.ID_Gruppo = map.ID_Gruppo AND cla.ID_Genotipo = map.ID_Genotipo ")
                stb.AppendLine()
                stb.AppendLine("WHERE 1 = 1 ")

                If Entita_cod <> 0 Then
                    stb.AppendLine("AND mylog.Entita_cod_Propietario = " & Entita_cod)
                End If

                If Sementieri_Sportello_Configurazione_cod <> 0 Then
                    stb.AppendLine("AND mylog.Sementieri_Sportello_Configurazione_cod = " & Sementieri_Sportello_Configurazione_cod)
                End If

                stb.AppendLine("ORDER BY mylog.data_creazione desc ")

                'stb.AppendLine("SELECT ")
                'stb.AppendLine("    ISNULL(mylog.Descrizione_casella, '') AS Descrizione_Casella ")
                'stb.AppendLine("    , ISNULL(vegImp.Veg_Des, '') AS Veg_DES ")
                'stb.AppendLine("    , ISNULL(iImp.rag_soc, '') as Rag_SocSementiero ")
                'stb.AppendLine("    , ISNULL(ind_Imp.ind_des, '') as ind_des ")
                'stb.AppendLine("    , ISNULL(ind_Imp.frz_des, '') as frz_des ")
                'stb.AppendLine("    , ISNULL(ind_Imp.CAP, '') as CAP ")
                'stb.AppendLine("    , ISNULL(istatImp.LOCALITA, '') as Com_DES ")
                'stb.AppendLine("    , ISNULL(istatImp.Comuni_Prov, '') as Pro_DES ")
                'stb.AppendLine("    , ISNULL(grva_imp.Grva_Des, '') + case when ISNULL(imp.grva_cod_veg, 0) < 0 then ' (ibrido)' else '' end as grva_des ")
                'stb.AppendLine("    , myLog.TipoOperazione_DB ")
                'stb.AppendLine("    , myLog.Flag_attivo ")
                'stb.AppendLine("    , myLog.Data_Creazione as Data_log ")
                'stb.AppendLine("    , ISNULL(sa_Imp.lat, 0)  as lat ")
                'stb.AppendLine("    , ISNULL(sa_Imp.long, 0) as long ")
                'stb.AppendLine("    , ISNULL(( ")
                'stb.AppendLine("                Select top 1 1 ")
                'stb.AppendLine("                From Sementieri_Sportello_Passaggi pp ")
                'stb.AppendLine("                Inner Join Sementieri_Sportello_ConfigurazioneXPassaggi cp ")
                'stb.AppendLine("                    On cp.Sementieri_Sportello_Passaggi_cod = pp.Sementieri_Sportello_Passaggi_cod ")
                'stb.AppendLine("                        And cp.Sementieri_Sportello_Configurazione_cod = mylog.Sementieri_Sportello_Configurazione_cod ")
                'stb.AppendLine("                        And cp.Data_Inizio <= mylog.Data_Creazione ")
                'stb.AppendLine("                        And cp.Data_Fine >= myLog.Data_Creazione ")
                'stb.AppendLine("                        And pp.RichiediConfermaSuInterferenze = 1 ")
                'stb.AppendLine("        ), 0) as OperazioneDaEvidenziare ")
                'stb.AppendLine("    , myLog.Sementieri_Sportello_LogOperazioni_COD ")
                'stb.AppendLine("    , ISNULL(vegImp.veg_cod, -1) as vegImp_veg_cod ")
                'stb.AppendLine("    , -1 as vegPlan_veg_cod ")
                'stb.AppendLine("    , ISNULL(imp.grva_cod_veg, 0) as imp_grva_veg_cod ")
                'stb.AppendLine("    , 0 as pl_grva_cod ")
                'stb.AppendLine("    , e.Entita_Cod ")
                'stb.AppendLine("    , e.piva ")
                'stb.AppendLine("    , e.sa_cod ")
                'stb.AppendLine("    , ISNULL(imp.Codice_Fiscale_Tecnico, '') as PIVA_DittaSementieraReferente ")
                'stb.AppendLine("	, ISNULL(LConf.Gruppi_Utente_des, '') as LayerDesc ")
                'stb.AppendLine("	, ISNULL(app.Via_Stringa, '') as Via_Stringa ")
                'stb.AppendLine("    , ISNULL(geg.Poligono_GeoEntity.EnvelopeCenter().Lat, 0) AS True_Lat ")
                'stb.AppendLine("    , ISNULL(geg.Poligono_GeoEntity.EnvelopeCenter().Long, 0) AS True_Lng ")
                'stb.AppendLine("    , ISNULL(Imp.Sup_Imp, 0) AS Superficie ")
                'stb.AppendLine()
                'stb.AppendLine("FROM ")
                'stb.AppendLine("    Sementieri_Sportello_LogOperazioni As mylog ")
                'stb.AppendLine()
                'stb.AppendLine("LEFT JOIN GIS_Entita            e           ON e.Entita_Cod = mylog.Entita_Cod_Propietario ")
                'stb.AppendLine("LEFT JOIN Centri_Aziendali      sa_imp      ON e.Piva = sa_imp.PIVA And e.Sa_Cod = sa_imp.sa_cod ")
                'stb.AppendLine("LEFT JOIN Imprese               iImp        ON iImp.PIVA = sa_imp.PIVA ")
                'stb.AppendLine("LEFT JOIN Reg_Impianti          Imp         ON e.Piva = Imp.PIVA And e.sa_cod = imp.sa_cod And e.appezza = imp.appezza And e.Id_Imp  = imp.id_reg ")
                'stb.AppendLine("LEFT JOIN " & nomeDB_Utenti & ".dbo.Gruppi_Utente LConf ON Imp.Codice_Fiscale_Tecnico = LConf.Gruppi_Utente_Identificativo ")
                'stb.AppendLine("LEFT JOIN CentrixIndirizzi      CI_Imp      ON CI_Imp.piva = sa_Imp.PIVA And CI_Imp.sa_cod = sa_Imp.sa_cod ")
                'stb.AppendLine("LEFT JOIN Indirizzi             ind_Imp     ON Ind_Imp.cod_indirizzo = ci_Imp.cod_indirizzo ")
                'stb.AppendLine("LEFT JOIN ISTAT                 istatImp    ON istatImp.PROV = ind_Imp.pro_cod_istat And istatImp.COM = ind_Imp.com_cod_istat ")
                'stb.AppendLine("LEFT JOIN Cultivar              cImp        ON cImp.Cul_Cod = Imp.CUL_COD ")
                'stb.AppendLine("LEFT JOIN SpecieVegetali        vegImp      ON cImp.veg_cod = vegImp.Veg_Cod ")
                'stb.AppendLine("LEFT JOIN GruppoVarietale       grva_imp    ON grva_imp.grva_cod = abs(Imp.GRVA_Cod_VEG ) ")
                'stb.AppendLine("LEFT JOIN Appezzamento			app			ON e.Piva = app.PIVA AND e.Sa_Cod = app.SA_COD AND e.Appezza = app.APPEZZA ")
                'stb.AppendLine("LEFT JOIN GIS_ElementiGrafici   geg			ON geg.PivaSuperUser = e.PivaSuperUser ")
                'stb.AppendLine("                                            AND geg.Entita_Cod = e.Entita_Cod ")
                'stb.AppendLine()
                'stb.AppendLine("WHERE 1 = 1 ")

                'If Entita_cod <> 0 Then
                '    stb.AppendLine(" AND mylog.Entita_cod_Propietario = " & Entita_cod)
                'End If

                'If Sementieri_Sportello_Configurazione_cod <> 0 Then
                '    stb.AppendLine(" AND mylog.Sementieri_Sportello_Configurazione_cod = " & Sementieri_Sportello_Configurazione_cod)
                'End If

                'stb.AppendLine(" Order by mylog.data_creazione desc ")

            Else

                stb.Append(" SELECT      " & vbCrLf)
                stb.Append("      ISNULL(mylog.Descrizione_casella, '') AS Descrizione_Casella " & vbCrLf)
                stb.Append("    , ISNULL(vegImp.Veg_Des, vegPlan.Veg_Des) AS Veg_DES " & vbCrLf)
                stb.Append("    , ISNULL(iImpSem.rag_soc, iPlannSem.rag_soc) as Rag_SocSementiero " & vbCrLf)
                stb.Append("    , ISNULL(ind_Imp.ind_des, ind_Plann.ind_des ) as ind_des " & vbCrLf)
                stb.Append("    , ISNULL(ind_Imp.frz_des, ind_Plann.frz_des ) as frz_des  " & vbCrLf)
                stb.Append("    , ISNULL(ind_Imp.CAP, ind_Plann.CAP ) as CAP " & vbCrLf)
                stb.Append("    , ISNULL(istatImp.LOCALITA, istatImp.Localita) as Com_DES " & vbCrLf)
                stb.Append("    , ISNULL(istatImp.Comuni_Prov, istatImp.Comuni_Prov) as Pro_DES " & vbCrLf)
                stb.Append("    , ISNULL(grva_imp.Grva_Des, grva_plann.grva_des) + case when ISNULL(imp.grva_cod_veg, pl.Grva_Cod)<0 then ' (ibrido)' else '' end as grva_des " & vbCrLf)
                stb.Append("    , myLog.TipoOperazione_DB " & vbCrLf)
                stb.Append("    , myLog.Flag_attivo      " & vbCrLf)
                stb.Append("    , myLog.Data_Creazione as Data_log " & vbCrLf)
                stb.Append("    , isNull(sa_Imp.lat, sa_Plann.lat) as lat " & vbCrLf)
                stb.Append("    , isNull(sa_Imp.long, sa_Plann.long) as long " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    , isnull((  " & vbCrLf)
                stb.Append("        select top 1 1 " & vbCrLf)
                stb.Append("        from Sementieri_Sportello_Passaggi pp " & vbCrLf)
                stb.Append("            inner join Sementieri_Sportello_ConfigurazioneXPassaggi cp " & vbCrLf)
                stb.Append("                on cp.Sementieri_Sportello_Passaggi_cod = pp.Sementieri_Sportello_Passaggi_cod               " & vbCrLf)
                stb.Append("                and cp.Sementieri_Sportello_Configurazione_cod = mylog.Sementieri_Sportello_Configurazione_cod  " & vbCrLf)
                stb.Append("                and cp.Data_Inizio <= mylog.Data_Creazione  " & vbCrLf)
                stb.Append("                and cp.Data_Fine >= myLog.Data_Creazione " & vbCrLf)
                stb.Append("                and pp.RichiediConfermaSuInterferenze = 1 " & vbCrLf)
                stb.Append("        ),0) as OperazioneDaEvidenziare " & vbCrLf)

                stb.Append("    , myLog.Sementieri_Sportello_LogOperazioni_COD   " & vbCrLf)
                stb.Append("    , isnull(vegImp.veg_cod, -1) as vegImp_veg_cod  " & vbCrLf)
                stb.Append("    , isnull(vegPlan.veg_cod, -1) as vegPlan_veg_cod  " & vbCrLf)
                stb.Append("    , ISNULL(imp.grva_cod_veg, 0) as imp_grva_veg_cod " & vbCrLf)
                stb.Append("    , isnull(PL.grva_cod, 0) as pl_grva_cod " & vbCrLf)
                stb.Append("    , e.Entita_Cod  " & vbCrLf)
                stb.Append("    , e.piva " & vbCrLf)
                stb.Append("    , e.sa_cod " & vbCrLf)
                stb.Append("    , isNull(imp.Codice_Fiscale_Tecnico, PL.Codice_Fiscale_Tecnico ) as PIVA_DittaSementieraReferente " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append(" FROM " & vbCrLf)
                stb.Append("    Sementieri_Sportello_LogOperazioni AS mylog  " & vbCrLf)
                stb.Append("    LEFT JOIN GIS_Entita AS e  " & vbCrLf)
                stb.Append("        ON e.Entita_Cod = mylog.Entita_Cod_Propietario     " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Reg_Impianti AS Imp  " & vbCrLf)
                stb.Append("        ON e.Piva = Imp.PIVA " & vbCrLf)
                stb.Append("        and e.sa_cod = imp.sa_cod " & vbCrLf)
                stb.Append("        and e.appezza = imp.appezza " & vbCrLf)
                stb.Append("        and e.Id_Imp  = imp.id_reg  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Programmazione_Entita AS PL  " & vbCrLf)
                stb.Append("        ON e.Programmazione_Entita_Cod = PL.Programmazione_Entita_Cod " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Centri_Aziendali sa_Plann " & vbCrLf)
                stb.Append("        ON pl.Piva = sa_Plann.PIVA  " & vbCrLf)
                stb.Append("        and PL.Sa_Cod = sa_Plann.sa_cod  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN CentrixIndirizzi CI_Plann " & vbCrLf)
                stb.Append("        on CI_Plann.piva = sa_plann.PIVA  " & vbCrLf)
                stb.Append("        and CI_Plann.sa_cod = sa_Plann.sa_cod  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Indirizzi ind_Plann " & vbCrLf)
                stb.Append("        on Ind_Plann.cod_indirizzo = ci_plann.cod_indirizzo " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN ISTAT istatPlann " & vbCrLf)
                stb.Append("        on istatPlann.PROV = ind_Plann.pro_cod_istat  " & vbCrLf)
                stb.Append("        and istatPlann.COM = ind_Plann.com_cod_istat  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Centri_Aziendali sa_imp " & vbCrLf)
                stb.Append("        ON pl.Piva = sa_imp.PIVA  " & vbCrLf)
                stb.Append("        and PL.Sa_Cod = sa_imp.sa_cod  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN CentrixIndirizzi CI_Imp " & vbCrLf)
                stb.Append("        on CI_Imp.piva = sa_Imp.PIVA  " & vbCrLf)
                stb.Append("        and CI_Imp.sa_cod = sa_Imp.sa_cod  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Indirizzi ind_Imp " & vbCrLf)
                stb.Append("        on Ind_Imp.cod_indirizzo = ci_Imp.cod_indirizzo " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN ISTAT istatImp " & vbCrLf)
                stb.Append("        on istatImp.PROV = ind_Imp.pro_cod_istat  " & vbCrLf)
                stb.Append("        and istatImp.COM = ind_Imp.com_cod_istat  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Imprese iPlann " & vbCrLf)
                stb.Append("        ON iPlann.piva = sa_plann.PIVA  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Imprese iImp " & vbCrLf)
                stb.Append("        ON iImp.piva = sa_plann.PIVA  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Imprese iPlannSem " & vbCrLf)
                stb.Append("        ON iPlannSem.piva = PL.codice_fiscale_tecnico  " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Imprese iImpSem " & vbCrLf)
                stb.Append("        ON iImp.piva = imp.codice_fiscale_tecnico    " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN SpecieVegetali vegPlan " & vbCrLf)
                stb.Append("        on pl.veg_cod = vegPlan.Veg_Cod " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN Cultivar cImp " & vbCrLf)
                stb.Append("        on cImp.Cul_Cod = Imp.CUL_COD    " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN SpecieVegetali vegImp " & vbCrLf)
                stb.Append("        on cImp.veg_cod = vegImp.Veg_Cod " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN GruppoVarietale grva_imp " & vbCrLf)
                stb.Append("        on grva_imp.grva_cod = abs(Imp.GRVA_Cod_VEG ) " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append("    LEFT JOIN GruppoVarietale grva_Plann " & vbCrLf)
                stb.Append("        on grva_Plann.grva_cod = abs(Pl.GRVA_Cod ) " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append(" WHERE   1=1 " & vbCrLf)

                If Entita_cod <> 0 Then
                    stb.Append(" AND mylog.Entita_cod_Propietario = " & Entita_cod & " " & vbCrLf)
                End If

                If Sementieri_Sportello_Configurazione_cod <> 0 Then
                    stb.Append(" AND mylog.Sementieri_Sportello_Configurazione_cod = " & Sementieri_Sportello_Configurazione_cod & " " & vbCrLf)
                End If

                stb.Append(" Order by mylog.data_creazione desc " & vbCrLf)

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt
    End Function


End Class


Public Class Sementieri_Sportello_LOG_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi( _
                            ByVal Sementieri_Sportello_LogOperazioni_COD As Integer, _
                            ByVal Sementieri_Sportello_Configurazione_cod As Integer, _
                            ByVal TipoOperazioneDB As Integer, _
                            ByVal Entita_Cod_Propietario As Integer, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_LOG_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Sementieri_Sportello_LogOperazioni (Pivasuperuser, ")
            StrSQL.Append("                    Sementieri_Sportello_LogOperazioni_COD, ")
            StrSQL.Append("                    Sementieri_Sportello_Configurazione_cod, ")
            StrSQL.Append("                    TipoOperazione_DB, ")
            StrSQL.Append("                    Flag_Attivo, ")
            StrSQL.Append("                    Entita_Cod_Propietario ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Sementieri_Sportello_LogOperazioni_COD))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Sementieri_Sportello_Configurazione_cod))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(TipoOperazioneDB))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(-1))
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Entita_Cod_Propietario))
            StrSQL.Append(")")
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

    Public Function ScriviInCache(ByVal Sementieri_Sportello_LogOperazioni_COD As Integer,
                                  ByVal Flag_attivo As Integer,
                                  ByVal rigaXcache As DataRow,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Sementieri_Sportello_LOG_W.Modifica_Descrizione_Casella_Conflitto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Dim strCols As String = ""
        Dim strCells As String = ""
        Dim first As Boolean = True

        For Each col In rigaXcache.Table.Columns
            If Not first Then
                strCols &= "|"
                strCells &= "|"
            End If
            strCols &= col.ColumnName
            strCells &= rigaXcache.Item(col.ColumnName)
            first = False
        Next

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Sementieri_Sportello_LogOperazioni SET ")
            StrSQL.AppendLine("Descrizione_Casella = '" & Agro_SQL_SaveText(strCols & "§" & strCells) & "', ")
            StrSQL.AppendLine("Flag_Attivo = " & Agro_SQL_SaveNum(Flag_attivo))
            StrSQL.AppendLine("WHERE PivasuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("AND Sementieri_Sportello_LogOperazioni_COD = " & Agro_SQL_SaveNum(Sementieri_Sportello_LogOperazioni_COD) & " ")

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
