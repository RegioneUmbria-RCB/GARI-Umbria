Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class Query_x_Report
    Inherits AgronicaCoreDataProvider.DataProvider

 
    Public Function Leggi_Dati_Analisi(ByVal ID_PDC_Testata As Integer, _
                          ByVal ID_PDC_Campione As Integer, _
                          ByVal Analisi_Testata_Cod As Integer, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
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
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT   distinct PDC_Testata.Da_Campagna, pdc_dettagli.data_fornitura, PDC_Campioni.Codice_Campione AS cod_rapporto_prova, PDC_Campioni.Data_Campionamento AS data_campione, ")
            StrSQL.AppendLine(" PDC_Analisi.Data_Richiesta_Analisi AS data_ricevimento, Analisi_Testata.Analisi_Testata_Data_Inizio AS data_inizio_analisi, ")
            StrSQL.AppendLine(" Analisi_Testata.Analisi_Testata_Data_Fine AS data_fine_analisi, Analisi_Tipologia.Analisi_Tipologia_Des, Analisi_Tipologia_Dettagli.LDM, ")
            StrSQL.AppendLine(" COALESCE(Analisi_Dettaglio_Valore_2,Analisi_Tipologia_Dettagli.UDM_Cod) as UDM_Cod, Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod, cast(Analisi_Dettagli.Analisi_Dettaglio_Valore_1 as varchar(2000)) as Analisi_Dettaglio_Valore_1, ")
            StrSQL.AppendLine(" Analisi_Dettagli.Analisi_Dettaglio_MargineErrore_1, lTRIM( ISNULL(cast( PrincipiAttivi.Pa_Des as varchar(2000))  , FamigliePrincipiAttivi.Descrizione) ) AS Principio_Famiglie, '' AS RMA,  ")
            StrSQL.AppendLine(" COALESCE(um2.UDM_SIM,UnitaMisura.UDM_SIM ) as UDM_SIM, PDC_Dettagli.Veg_Cod, Cultivar.Cul_Des, SpecieVegetali.Veg_Des, Analisi_testata.Analisi_testata_des , pdc_dettagli.Rag_soc as fornitore,  isnull(PDC_Campioni.Note_Campione,'') as Note_Campione , isnull(PDC_Dettagli.LottoFornitore,'') as LottoFornitore ")

            StrSQL.AppendLine(" FROM    PDC_Dettagli   ")
            StrSQL.AppendLine("			INNER JOIN pdc_testata on pdc_testata.Id_PDC_Testata = PDC_Dettagli.ID_PDC_Testata  ")
            StrSQL.AppendLine("			LEFT JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli   ")
            StrSQL.AppendLine("			LEFT JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione   ")
            StrSQL.AppendLine("			LEFT JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser   ")
            StrSQL.AppendLine("			LEFT JOIN Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Tipologia.PivaSuperUser   ")
            StrSQL.AppendLine("			LEFT JOIN Analisi_Tipologia_Dettagli ON Analisi_Tipologia.PivaSuperUser = Analisi_Tipologia_Dettagli.PivaSuperUser AND Analisi_Tipologia.Analisi_Tipologia_Cod = Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN UnitaMisura ON Analisi_Tipologia_Dettagli.UDM_Cod = UnitaMisura.UDM_COD   ")
            StrSQL.AppendLine("			LEFT JOIN PDC_Dettagli AS PDC_Dettagli_1 ON PDC_Campioni.PivaSuperUser = PDC_Dettagli_1.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli_1.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli_1.ID_PDC_Dettagli   ")
            StrSQL.AppendLine("			LEFT JOIN SpecieVegetali ON PDC_Dettagli_1.Veg_Cod = SpecieVegetali.Veg_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN Cultivar ON PDC_Dettagli_1.Cul_Cod = Cultivar.Cul_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN (SELECT Fam_Cod, Descrizione FROM FamigliePrincipiAttivi AS FamigliePrincipiAttivi_1 WHERE (isnumeric(Fam_Cod) <> 0)) AS FamigliePrincipiAttivi ON ABS(Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod) = FamigliePrincipiAttivi.Fam_Cod AND Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod < 0   ")
            StrSQL.AppendLine("			LEFT JOIN PrincipiAttivi ON Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = PrincipiAttivi.Pa_Cod AND Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod > 0   ")
            StrSQL.AppendLine("			LEFT JOIN Analisi_Dettagli ON Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod AND Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = Analisi_Dettagli.Analisi_Parametro_Cod  ")
            StrSQL.AppendLine("			LEFT JOIN UnitaMisura um2 ON Analisi_Dettagli.Analisi_Dettaglio_Valore_2 = um2.UDM_COD ")

            StrSQL.AppendLine(" WHERE 1=1 ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Testata  = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If

            StrSQL.AppendLine(" order by principio_famiglie ")

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


    Public Function Leggi_Dati_Analisi_Generico(ByVal ID_PDC_Testata As Integer, _
                          ByVal ID_PDC_Campione As Integer, _
                          ByVal Analisi_Testata_Cod As Integer, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
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
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT     PDC_Campioni.Codice_Campione AS cod_rapporto_prova, PDC_Campioni.Data_Campionamento AS data_campione, ")
            StrSQL.AppendLine(" PDC_Analisi.Data_Richiesta_Analisi AS data_ricevimento, Analisi_Testata.Analisi_Testata_Data_Inizio AS data_inizio_analisi, ")
            StrSQL.AppendLine(" Analisi_Testata.Analisi_Testata_Data_Fine AS data_fine_analisi, Analisi_Tipologia.Analisi_Tipologia_Des, Analisi_Tipologia_Dettagli.LDM, ")
            StrSQL.AppendLine(" COALESCE(Analisi_Dettaglio_Valore_2,Analisi_Tipologia_Dettagli.UDM_Cod) as UDM_Cod, Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod, cast(Analisi_Dettagli.Analisi_Dettaglio_Valore_1 as varchar(2000)) as Analisi_Dettaglio_Valore_1, ")
            StrSQL.AppendLine(" Analisi_Dettagli.Analisi_Dettaglio_MargineErrore_1, analisi_parametro_des as Principio_Famiglie, '' AS RMA,  ")
            StrSQL.AppendLine(" COALESCE(um2.UDM_SIM,UnitaMisura.UDM_SIM ) as UDM_SIM , PDC_Dettagli.Veg_Cod, Cultivar.Cul_Des, SpecieVegetali.Veg_Des, Analisi_testata.Analisi_testata_des , pdc_dettagli.Rag_soc as fornitore,  isnull(PDC_Campioni.Note_Campione,'') as Note_Campione , Analisi_Dettaglio_MargineErrore_2")
            StrSQL.AppendLine(" ,pdc_dettagli.LottoFornitore ")
            StrSQL.AppendLine(" , (SELECT        PDC_Dettagli.data_fornitura  FROM PDC_Dettagli  INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli  INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione  where PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ) as data_fornitura  ")
            StrSQL.AppendLine(" FROM     PDC_Dettagli   ")
            StrSQL.AppendLine("			INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli   ")
            StrSQL.AppendLine("			INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione   ")
            StrSQL.AppendLine("			INNER JOIN Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser   ")
            StrSQL.AppendLine("			INNER JOIN Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod AND PDC_Analisi.PivaSuperUser = Analisi_Tipologia.PivaSuperUser   ")
            StrSQL.AppendLine("			INNER JOIN Analisi_Tipologia_Dettagli ON Analisi_Tipologia.PivaSuperUser = Analisi_Tipologia_Dettagli.PivaSuperUser AND Analisi_Tipologia.Analisi_Tipologia_Cod = Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN UnitaMisura ON Analisi_Tipologia_Dettagli.UDM_Cod = UnitaMisura.UDM_COD   ")
            StrSQL.AppendLine("			LEFT JOIN PDC_Dettagli AS PDC_Dettagli_1 ON PDC_Campioni.PivaSuperUser = PDC_Dettagli_1.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli_1.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli_1.ID_PDC_Dettagli   ")
            StrSQL.AppendLine("			LEFT JOIN SpecieVegetali ON PDC_Dettagli_1.Veg_Cod = SpecieVegetali.Veg_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN Cultivar ON PDC_Dettagli_1.Cul_Cod = Cultivar.Cul_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN Analisi_Parametri on Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod   ")
            StrSQL.AppendLine("			LEFT JOIN Analisi_Dettagli ON Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod AND Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = Analisi_Dettagli.Analisi_Parametro_Cod  ")
            StrSQL.AppendLine("			LEFT JOIN UnitaMisura um2 ON Analisi_Dettagli.Analisi_Dettaglio_Valore_2 = um2.UDM_COD ")


            StrSQL.AppendLine(" WHERE 1=1 ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Testata  = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Analisi.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            If ID_PDC_Campione <> 0 Then
                StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Campione = " & Agro_SQL_SaveNum(ID_PDC_Campione))
            End If

            StrSQL.AppendLine(" order by analisi_parametro_des ")

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


    Public Function Leggi_Dati_Intestazione_Rapporto_Prova(ByVal ID_PDC_Testata As Integer, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
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
            '---------------------------------------------
            StrSQL.AppendLine("  SELECT     PivaSuperUser, Id_PDC_Testata, PDC_Testata_Des, PDC_Data_Istantanea, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, PDC_Stato, Da_Campagna ")
            StrSQL.AppendLine("  FROM         PDC_Testata   ")


            StrSQL.AppendLine(" WHERE 1=1 ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata.ID_PDC_Testata  = " & Agro_SQL_SaveNum(ID_PDC_Testata))
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

    Public Function Leggi_Dati_Report_Analisi(ByVal Capitolati As String, ByVal DataInizio As Date, ByVal DataFine As Date,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "Query_x_Report.Leggi_Dati_Report_Analisi()"

        '====================================================================================
        'Parametri :
        '   Capitolati 
        '   DataInizio 
        '   DataFine
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
            '---------------------------------------------
            StrSQL.AppendLine("  SELECT  ")
            StrSQL.AppendLine("  	pdc_testata.Id_PDC_Testata, pdc_testata.PDC_Testata_Des, pdc_testata.PDC_Data_Istantanea, pdc_testata.Da_Campagna, pdc_lfo.LFO_Des, ")
            StrSQL.AppendLine("  	pdc_dettagli.ID_PDC_Dettagli, pdc_dettagli.ID_LFO, pdc_capitolati.ID_CapitolatoPrivato, pdc_capitolati.Des_Capitolato_Privato, ")
            StrSQL.AppendLine("  	pdc_dettagli.CodiceFornitore, pdc_dettagli.Piva, pdc_dettagli.Rag_Soc, pdc_dettagli.Sa_Nome, pdc_dettagli.App_Nome, pdc_analisi.pdc_stato_analisi, ")
            StrSQL.AppendLine("  	pdc_dettagli.Veg_Cod, pdc_dettagli.Cul_Cod, SpecieVegetali.Veg_Des, cultivar.Cul_Des, a.* ")
            StrSQL.AppendLine("  FROM pdc_sblocca  ")
            StrSQL.AppendLine("  inner join pdc_testata on pdc_sblocca.PivaSuperUser=pdc_testata.PivaSuperUser and pdc_sblocca.ID_PDC_Testata=pdc_testata.Id_PDC_Testata ")
            StrSQL.AppendLine("  inner join pdc_dettagli on pdc_sblocca.PivaSuperUser=pdc_dettagli.PivaSuperUser  ")
            StrSQL.AppendLine("  	and pdc_sblocca.Id_PDC_Testata=pdc_dettagli.ID_PDC_Testata and pdc_sblocca.piva=pdc_dettagli.Piva ")
            StrSQL.AppendLine("  	and pdc_sblocca.sa_cod=pdc_dettagli.sa_cod and pdc_sblocca.appezza=pdc_dettagli.appezza and pdc_sblocca.id_reg=pdc_dettagli.id_reg ")
            StrSQL.AppendLine("  left join pdc_lfo on pdc_lfo.PivaSuperUser = pdc_dettagli.PivaSuperUser and pdc_lfo.Id_PDC_Testata = pdc_dettagli.Id_PDC_Testata and pdc_lfo.ID_LFO = pdc_dettagli.ID_LFO ")
            StrSQL.AppendLine("  left join cultivar on cultivar.cul_cod = pdc_dettagli.Cul_Cod and cultivar.Veg_Cod = pdc_dettagli.Veg_Cod left join SpecieVegetali on SpecieVegetali.veg_cod = pdc_dettagli.veg_cod ")
            StrSQL.AppendLine("  left join pdc_campioni on pdc_campioni.ID_PDC_Testata = pdc_dettagli.ID_PDC_Testata and pdc_campioni.ID_PDC_Dettagli = pdc_dettagli.ID_PDC_Dettagli ")
            StrSQL.AppendLine("  left join pdc_analisi on pdc_analisi.ID_PDC_Testata = pdc_campioni.ID_PDC_Testata and pdc_analisi.ID_PDC_Dettagli = pdc_campioni.ID_PDC_Dettagli and pdc_analisi.ID_PDC_Campione = pdc_campioni.ID_PDC_Campione ")
            StrSQL.AppendLine("  left join PDC_CapitolatiCliente_Attivi as pdc_capitolati on pdc_capitolati.PivaSuperUser=pdc_sblocca.PivaSuperUser and pdc_capitolati.ID_PDC_Testata=pdc_sblocca.ID_PDC_Testata and pdc_capitolati.ID_CapitolatoPrivato = pdc_sblocca.CapitolatoCliente_Cod ")
            StrSQL.AppendLine("  left join ( ")
            StrSQL.AppendLine("  	select pdc_dettagli.PivaSuperUser, pdc_dettagli.ID_PDC_Testata, pdc_dettagli.ID_PDC_Dettagli, pdc_dettagli.ID_LFO, pdc_analisi.Analisi_Testata_Cod, pdc_dettagli.veg_cod, pdc_dettagli.cul_cod, ")
            StrSQL.AppendLine("  	    pdc_campioni.Codice_Campione, analisi_testata.Analisi_Testata_Des, analisi_testata.Analisi_Testata_Data_Fine, analisi_tipologia.Analisi_Tipologia_Des, pdc_analisi.Altre_Molecole, ")
            StrSQL.AppendLine("  	    SpecieVegetali.veg_des as Specie_Vegetale, cultivar.cul_des as Varieta, lista_regioni.regione_des as Origine, contatti.rag_soc as Laboratorio, analisi_capitolato.Esito, analisi_capitolato.Descrizione_Esito ")
            StrSQL.AppendLine("  	from pdc_dettagli inner join pdc_campioni on pdc_campioni.ID_PDC_Testata = pdc_dettagli.ID_PDC_Testata and pdc_campioni.ID_PDC_Dettagli = pdc_dettagli.ID_PDC_Dettagli ")
            StrSQL.AppendLine("  	inner join pdc_analisi on pdc_analisi.ID_PDC_Testata = pdc_campioni.ID_PDC_Testata and pdc_analisi.ID_PDC_Dettagli = pdc_campioni.ID_PDC_Dettagli and pdc_analisi.ID_PDC_Campione = pdc_campioni.ID_PDC_Campione ")
            StrSQL.AppendLine("  	left join cultivar on cultivar.cul_cod = pdc_dettagli.Cul_Cod and cultivar.Veg_Cod = pdc_dettagli.Veg_Cod left join SpecieVegetali on SpecieVegetali.veg_cod = pdc_dettagli.veg_cod ")
            StrSQL.AppendLine("  	left join risorse_umane on risorse_umane.Cod_Risum = pdc_analisi.Cod_RisUm left join contatti on risorse_umane.cod_contatto = contatti.cod_contatto ")
            StrSQL.AppendLine("  	left join impresexindirizzi on impresexindirizzi.piva=pdc_dettagli.piva and impresexindirizzi.tipo_indirizzo=1 ")
            StrSQL.AppendLine("  	left join indirizzi on indirizzi.cod_indirizzo = impresexindirizzi.cod_indirizzo left join lista_province on lista_province.prov=indirizzi.pro_cod_istat left join lista_regioni on lista_regioni.reg=lista_province.reg ")
            StrSQL.AppendLine("  	left join analisi_tipologia on analisi_tipologia.Analisi_Tipologia_Cod = pdc_analisi.Analisi_Tipologia_Cod ")
            StrSQL.AppendLine("  	left join analisi_testata on analisi_testata.Analisi_Testata_Cod = pdc_analisi.Analisi_Testata_Cod ")
            StrSQL.AppendLine("  	left join analisi_conformita_capitolato_cliente as analisi_capitolato on analisi_capitolato.pivasuperuser = pdc_analisi.PivaSuperUser and analisi_capitolato.Analisi_Testata_Cod = pdc_analisi.Analisi_Testata_Cod and analisi_capitolato.CapitolatoCliente_Cod = 0 ")
            StrSQL.AppendLine("  	where pdc_analisi.Mostra_in_Stampe = -1 ")
            StrSQL.AppendLine("  ) a on a.PivaSuperUser = pdc_dettagli.PivaSuperUser and a.ID_PDC_Testata = pdc_dettagli.ID_PDC_Testata and a.ID_LFO = pdc_dettagli.ID_LFO ")

            ' lotti sbloccati e/o con analisi sbloccate
            StrSQL.AppendLine(" WHERE pdc_sblocca.esito=-1 and (pdc_analisi.Mostra_in_Stampe is null or pdc_analisi.Mostra_in_Stampe = -1) ")

            ' filtro su data validita piano campionamento
            StrSQL.AppendLine(" AND pdc_testata.PDC_Data_Istantanea <= " & Agro_SQL_SaveDate(DataFine))
            StrSQL.AppendLine(" AND pdc_testata.PDC_Data_Istantanea >= " & Agro_SQL_SaveDate(DataInizio))

            ' filtro su capitolati
            If Not String.IsNullOrEmpty(Capitolati) Then
                StrSQL.AppendLine(" AND pdc_sblocca.CapitolatoCliente_Cod IN (" & Agro_SQL_Save_Clausola_IN(Capitolati) & ") ")
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
  