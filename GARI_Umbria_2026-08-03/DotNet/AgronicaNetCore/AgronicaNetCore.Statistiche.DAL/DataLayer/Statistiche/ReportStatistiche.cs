using AgronicaDataProvider6.Interfaces;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Statistiche.DAL.DataLayer.Statistiche
{
    public class ReportStatistiche : DAL_Base, IReportStatistiche
    {
        private string[] statiSemaforoStopJob = { "RED", "BLOCKED" };

        public ReportStatistiche(IServiceProvider provider, bool securityBypass = false) : base(provider, securityBypass)
        {
        }

        public async Task<bool> IsSemaforoVerde(string? pivaSuperuser, int? jobId, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            try
            {
                if (objParametriInterscambio != null)
                {
                    string? statoSemaforo = await GetStatoJobDaSemaforo(pivaSuperuser, jobId, objParametriInterscambio);
                    result = string.IsNullOrEmpty(statoSemaforo);
                    result = result || (!statiSemaforoStopJob.Contains(statoSemaforo));
                }
                else
                    throw new Exception("La connessione verso 'Interscambio' non configurata, impossibile verificare lo stato del semaforo");
            }
            catch
            {
                throw;
            }
            return result;
        }

        public async Task<bool> ImpostaBloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, DateTime? avvio, AgronicaCoreParametri objParametriInterscambio)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperUser);
            expandoObj.TryAdd("@jobId", jobId);
            expandoObj.TryAdd("@avvio", avvio);

            bool result = false;

            stbQuery.AppendLine("IF EXISTS(SELECT 1")
                .AppendLine("   FROM Semaforo")
                .AppendLine("   WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("   AND JobId = @jobId)")
                .AppendLine("BEGIN")
                .AppendLine("   UPDATE Semaforo")
                .AppendLine("   SET Status = 'RED',")
                .AppendLine("       DataAvvioElaborazione = @avvio")
                .AppendLine("   WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("   AND JobId = @jobId")
                .AppendLine("END")
                .AppendLine("ELSE")
                .AppendLine("BEGIN")
                .AppendLine("   INSERT INTO Semaforo")
                .AppendLine("   (PivaSuperuser, JobId, Status, DataAvvioElaborazione)")
                .AppendLine("   VALUES")
                .AppendLine("   (@pivaSuperUser, @jobId, 'RED', @avvio)")
                .AppendLine("END");

            try
            {
                result = await GetDataProvider(objParametriInterscambio).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriInterscambio);
            }
            return result;
        }

        public async Task<bool> ImpostaSbloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri objParametriInterscambio)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperUser);
            expandoObj.TryAdd("@jobId", jobId);

            bool result = false;

            stbQuery.AppendLine("UPDATE Semaforo")
                .AppendLine("   SET Status = 'GREEN',")
                .AppendLine("       DataUltimaElaborazione = DataAvvioElaborazione,")
                .AppendLine("       DataAvvioElaborazione = NULL")
                .AppendLine("   WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("   AND JobId = @jobId");

            try
            {
                result = await GetDataProvider(objParametriInterscambio).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriInterscambio);
            }
            return result;
        }

        public async Task<bool?> BulkInsertAsync(string tabella, DataTable? dettagliOperazioni, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            int batchSize = 5000;
            try
            {
                if (dettagliOperazioni != null && dettagliOperazioni.Rows.Count < batchSize)
                    batchSize = dettagliOperazioni.Rows.Count;

                if (batchSize > 0)
                {
                    result = await ClearSwapTableAsync(tabella, objParametriInterscambio);

                    if (result)
                    {
                        Dictionary<string, string> columnsMapping = new Dictionary<string, string>()
                        {
                            {"PIVA", $"PIVA"},
                            {"Cuaa", "Cuaa"},
                            {"RagioneSociale", "RagioneSociale"},
                            {"Referente", "Referente"},
                            {"Regione", "Regione"},
                            {"Provincia", "Provincia"},
                            {"Comune", "Comune"},
                            {"Num_Operazioni_Campagna", "Num_Operazioni_Campagna"},
                            {"Num_Operazioni_Magazzino", "Num_Operazioni_Magazzino"},
                            {"Dichiarazione_NonUtilizzo_Fertilizzanti", "Dichiarazione_NonUtilizzo_Fertilizzanti" },
                            {"Dichiarazione_NonUtilizzo_Trattamenti", "Dichiarazione_NonUtilizzo_Trattamenti" },
                            {"Num_PUA", "Num_PUA"},
                            {"Num_PianiConcimazione", "Num_PianiConcimazione"},
                            {"Data_sottoscrizione_QdC", "Data_sottoscrizione_QdC"},
                            {"Modalita_attivazione", "Modalita_attivazione"},
                            {"Data_primo_login", "Data_primo_login"},
                            {"Data_ultimo_login", "Data_ultimo_login"},
                            {"Utenti", "Utenti"},
                            {"N_Utenti", "N_Utenti"},
                            {"RowNumber", "RowNumber"},
                            {"Data_Estrazione", "Data_Estrazione" }
                        };

                        result = await GetDataProvider(objParametriInterscambio).ExecuteBulkInsertAsync(dettagliOperazioni!, tabella, columnsMapping, batchSize);
                    }
                }
                else
                {
                    throw new Exception("Non sono presenti dati da importare");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriInterscambio, ex);
                throw;
            }
        }

        public async Task<bool?> CopyFromSwapTableAsync(object swapTable, object targetTable, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            try
            {
                StringBuilder snapshot = new StringBuilder();
                snapshot.AppendLine()
                    .AppendLine($"INSERT INTO {targetTable}")
                    .AppendLine("SELECT *")
                    .AppendLine($"FROM {swapTable};");

                result = await GetDataProvider(objParametriInterscambio).Execute_WriteAsync($"TRUNCATE TABLE {targetTable};");
                result = result && await GetDataProvider(objParametriInterscambio).Execute_WriteAsync(snapshot.ToString());
                result = result && await GetDataProvider(objParametriInterscambio).Execute_WriteAsync($"TRUNCATE TABLE {swapTable};");

                if (!result)
                    throw new Exception("Errore durante la scrittura della tabella StatisticheUtilizzo");

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriInterscambio, ex);
                throw;
            }
        }

        public async Task<bool> ClearSwapTableAsync(object swapTable, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            try
            {
                result = await GetDataProvider(objParametriInterscambio).Execute_WriteAsync($"TRUNCATE TABLE {swapTable};");
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriInterscambio, ex);
                throw;
            }
        }

        public async Task<DataTable?> LeggiDettagliAsync(enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione,
            DateTime dataCompetenzaInizio, DateTime dataCompetenzaFine, string? filtroUsername, string? filtroPiva,
            AgronicaCoreParametriServer objParametriServer)
        {
            bool attivaFiltroPiva = !string.IsNullOrEmpty(filtroPiva);
            var sqlParams = new Dictionary<string, object>();
            sqlParams.TryAdd("utenteUsername", objParametriServer.UtenteUsername);
            sqlParams.TryAdd("dataValiditaInizio", dataCompetenzaInizio);
            sqlParams.TryAdd("dataValiditaFine", dataCompetenzaFine);
            if (attivaFiltroPiva)
                sqlParams.TryAdd("aziende", filtroPiva!);

            var stbQuery = new StringBuilder();

            stbQuery
                .AppendLine(FiltroImprese(attivaFiltroPiva))
                // PRATICHE
                .AppendLine(PraticheDettaglio(attivaFiltroPiva))
                // TOTALI AGGREGATI
                .AppendLine(TotaliAggregati(attivaFiltroPiva, filtroUsername, filtroPerDataCompetenzaOrDataRegistrazione))
                // ELENCO IMPRESE
                .AppendLine(ImpreseTotali(attivaFiltroPiva))
                // QUERY STATISTICHE
                .AppendLine(DettagliStatistiche())
                // RIMOZIONE TABELLE TEMPORANEE
                .AppendLine(RimozioneTabelleTemporanee());

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams); ;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private async Task<string?> GetStatoJobDaSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri parametriInterscambio)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("@pivaSuperUser", pivaSuperUser!);
            parametriSql.TryAdd("@jobId", jobId!);

            string? result = null;

            stbQuery.AppendLine("SELECT PivaSuperuser, JobId, Status, DataAvvioElaborazione")
                .AppendLine("FROM Semaforo")
                .AppendLine("WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND JobId = @jobId");

            try
            {
                DataTable istanze = await GetDataProvider(parametriInterscambio).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                if (istanze != null && istanze.AsEnumerable().Any())
                { //result = istanze.Rows[0].Field<string>("Status");
                    DateTime? dataAvvio = istanze.Rows[0].Field<DateTime?>("DataAvvioElaborazione");
                    if (dataAvvio.HasValue)
                        result = dataAvvio.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriInterscambio);
            }
            return result;
        }

        private string? FiltroImprese(bool attivaFiltroPiva)
        {
            var stbQuery = new StringBuilder();
            if (attivaFiltroPiva)
            {    
                stbQuery
                    .AppendLine(RimozioneTabellaTemporanea("#totalString"))
                    .AppendLine(string.Empty)
                    .AppendLine("CREATE TABLE #totalString (aziende VARCHAR(MAX))")
                    .AppendLine(string.Empty)
                    .AppendLine(RimozioneTabellaTemporanea("#aziende"))
                    .AppendLine(string.Empty)
                    .AppendLine("CREATE TABLE #aziende (azienda VARCHAR(MAX))")
                    .AppendLine(string.Empty)
                    .AppendLine("INSERT INTO #totalString VALUES (@aziende)")
                    .AppendLine(string.Empty)

                    .AppendLine("INSERT INTO #aziende")
                    .AppendLine("SELECT LTRIM(RTRIM(Split.string.value('.[1]', 'varchar(8000)'))) AS azienda")
                    .AppendLine("FROM (")
                    .AppendLine("   SELECT CAST('<XMLRoot><RowData>' + REPLACE(aziende, ',', '</RowData><RowData>') + '</RowData></XMLRoot>' AS XML) AS x")
                    .AppendLine("   FROM #totalString")
                    .AppendLine(") AS tot")
                    .AppendLine("CROSS APPLY x.nodes('/XMLRoot/RowData') Split(string)") ;
            }
            return stbQuery
                .AppendLine(string.Empty)
                .ToString();
        }

        private string PraticheDettaglio(bool attivaFiltroPiva)
        {
            var stbQuery = new StringBuilder();
            stbQuery
                .AppendLine(RimozioneTabellaTemporanea("#PraticheData"))
                .AppendLine("SELECT  pratiche.Piva, CONVERT(VARCHAR, pratiche_stati_attuali.validita_inizio, 3) as Data_Stampa_QDC")
                .AppendLine("INTO #PraticheData ")
                .AppendLine("FROM pratiche WITH(NOLOCK)")
                .AppendLine("INNER JOIN pratiche_stati_attuali  WITH(NOLOCK)")
                .AppendLine("ON pratiche.pratica_cod = pratiche_stati_attuali.pratica_cod");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON pratiche.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");
            
            stbQuery.AppendLine("WHERE pratiche.Servizio_Cod = 2004")
                .AppendLine("AND pratiche_stati_attuali.Stato_Cod In (2003, 2005)")
                .AppendLine("AND pratiche_stati_attuali.validita_inizio <= @dataValiditaFine")
                .AppendLine("AND pratiche_stati_attuali.validita_inizio >= @dataValiditaInizio;");

            return stbQuery
                .AppendLine(string.Empty)
                .ToString();
        }

        private string TotaliAggregati(bool attivaFiltroPiva, string? filtroUsername, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery
                .AppendLine(RimozioneTabellaTemporanea("#Agr"))
                .AppendLine("SELECT i.piva As Piva,")
                .AppendLine("    pra.Data_Stampa_QDC,")
                .AppendLine("    ISNULL(campagna.operazioni, 0) as Num_Operazioni_Campagna,")
                .AppendLine("    ISNULL(magazzino.operazioni, 0) as Num_Operazioni_Magazzino,")
                .AppendLine("    ISNULL(PUA.operazioni, 0) as Num_PUA,")
                .AppendLine("    ISNULL(trattamenti.operazioni, 0) As Num_Trattamenti,")
                .AppendLine("    ISNULL(concimazioni.operazioni, 0) As Num_Concimazioni,")
                .AppendLine("    ISNULL(altro.operazioni, 0) As Num_Operazioni_Campagna_Altre,")
                .AppendLine("    ISNULL(nonUtFert.operazioni, 0) as Dichiarazione_NonUtilizzo_Fertilizzanti,")
                .AppendLine("    ISNULL(nonUtTrat.operazioni, 0) as Dichiarazione_NonUtilizzo_Trattamenti,")
                .AppendLine("    ISNULL(PianiConcimazione.operazioni, 0) as Num_PianiConcimazione")
                .AppendLine("INTO #Agr")
                .AppendLine("FROM ( ")
                .AppendLine("   SELECT imprese.piva")
                .AppendLine("   FROM imprese WITH(NOLOCK)");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            stbQuery.AppendLine(") i  ")

                // OPERAZIONI MAGAZZINO
                .AppendLine("LEFT JOIN  ( ")
                .AppendLine(OperazioniMagazzino(attivaFiltroPiva ,filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") magazzino")
                .AppendLine("ON i.piva= magazzino.PIVA")

                // OPERAZIONI CAMPAGNA
                .AppendLine("LEFT JOIN ( ")
                .AppendLine(OpreazioniCampagna(attivaFiltroPiva, filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") campagna")
                .AppendLine("ON i.piva = campagna.piva")

                // PRATICHE
                .AppendLine("LEFT JOIN #PraticheData pra")
                .AppendLine("ON i.piva = pra.piva ")

                // OPERAZIONI CAMPAGNA - TRATTAMENTI
                .AppendLine("LEFT JOIN ( ")
                .AppendLine(OperazioniCampagnaTrattamenti(attivaFiltroPiva, filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") trattamenti  ")
                .AppendLine("ON i.piva = trattamenti.piva")

                // OPERAZIONI CAMPAGNA - CONCIMAZIONI
                .AppendLine("LEFT JOIN ( ")
                .AppendLine(OperazioniCampagnaConcimazioni(attivaFiltroPiva, filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") concimazioni ")
                .AppendLine("ON i.piva = concimazioni.piva")

                // OPERAZIONI CAMPAGNA - ALTRE
                .AppendLine("LEFT JOIN ( ")
                .AppendLine(OperazioniCampagnaAltro(attivaFiltroPiva, filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") altro  ")
                .AppendLine("ON i.piva = altro.piva")

                // OPERAZIONI PUA
                .AppendLine("LEFT JOIN ( ")
                .AppendLine(OperazioniPUA(attivaFiltroPiva, filtroUsername!))
                .AppendLine(") PUA")
                .AppendLine("ON i.piva = PUA.piva")

                // PIANI CONCIMAZIONE
                .AppendLine("LEFT JOIN ( ")
                .AppendLine(OperazioniPianiConcimazione(attivaFiltroPiva, filtroUsername!))
                .AppendLine(") PianiConcimazione       ")
                .AppendLine("ON i.piva = PianiConcimazione.piva")

                // SENZA UTILIZZO FERTILIZZANTI
                .AppendLine("LEFT JOIN  ( ")
                .AppendLine(SenzaUtilizzoFertilizzanti(attivaFiltroPiva, filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") nonUtFert ")
                .AppendLine("ON i.piva = nonUtFert.PIVA")

                // SENZA UTILIZZO TRATTAMENTI
                .AppendLine("LEFT JOIN  ( ")
                .AppendLine(SenzaUtilizzoTrattamenti(attivaFiltroPiva, filtroUsername!, filtroPerDataCompetenzaOrDataRegistrazione))
                .AppendLine(") nonUtTrat")
                .AppendLine("ON i.piva = nonUtTrat.PIVA");

            return stbQuery
                .AppendLine(string.Empty)
                .ToString();
        }
        
        private string ImpreseTotali(bool attivaFiltroPiva)
        {
            var stbQuery = new StringBuilder();
            stbQuery
                .AppendLine(RimozioneTabellaTemporanea("#impreses"))
                .AppendLine("SELECT i.Piva,")
                .AppendLine("    ic.val_cod as Cuaa,")
                .AppendLine("    i.rag_soc as RagioneSociale,")
                .AppendLine("    i1.rag_soc + ' - ' + i1.piva as Referente,")
                .AppendLine("    ISNULL(")
                .AppendLine("    ( ")
                .AppendLine("        SELECT TOP 1 Lista_Regioni.regione_des")
                .AppendLine("        FROM Lista_Regioni WITH(NOLOCK)")
                .AppendLine("        RIGHT JOIN Lista_Province WITH(NOLOCK)")
                .AppendLine("        ON Lista_Regioni.reg=Lista_Province.REG")
                .AppendLine("        RIGHT JOIN Indirizzi WITH(NOLOCK)")
                .AppendLine("        ON Indirizzi.pro_cod_istat = Lista_Province.PROV  ")
                .AppendLine("        RIGHT JOIN ImpresexIndirizzi WITH(NOLOCK)")
                .AppendLine("        ON Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo")
                .AppendLine("        WHERE i.PIVA = ImpresexIndirizzi.PIVA")
                .AppendLine("    ), ' ') as Regione,")
                .AppendLine("    istat.COMUNI_PROV as Provincia,")
                .AppendLine("    istat.LOCALITA as Comune")
                .AppendLine("INTO #impreses")
                .AppendLine("FROM imprese i WITH(NOLOCK)");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON i.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            stbQuery.AppendLine("JOIN ImpresexIndirizzi WITH(NOLOCK)")
                .AppendLine("ON i.PIVA = ImpresexIndirizzi.PIVA")
                .AppendLine("JOIN Indirizzi WITH(NOLOCK)")
                .AppendLine("ON Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo")
                .AppendLine("JOIN istat WITH(NOLOCK)")
                .AppendLine("ON istat.PROV = Indirizzi.pro_cod_istat")
                .AppendLine("AND istat.COM = Indirizzi.com_cod_istat")
                .AppendLine("JOIN Imprese_Codici ic")
                .AppendLine("ON i.PIVA = ic.PIVA")
                .AppendLine("AND id_cod = '1010'")
                .AppendLine("JOIN GerarchiaImprese WITH(NOLOCK)")
                .AppendLine("ON i.PIVA = GerarchiaImprese.figlio")
                .AppendLine("JOIN imprese i1")
                .AppendLine("ON GerarchiaImprese.padre = i1.piva");

            return stbQuery
                .AppendLine(string.Empty)
                .ToString();
        }

        private string? DettagliStatistiche()
        {
            var stbQuery = new StringBuilder();
            stbQuery
                .AppendLine("SELECT a.*,")
                .AppendLine("       CAST(NULL AS INT) as N_Utenti,")
                .AppendLine("       CAST(NULL AS VARCHAR) as Utenti,")
                .AppendLine("       CAST(NULL AS DATETIME) as Data_sottoscrizione_QdC,")
                .AppendLine("       CAST(NULL AS VARCHAR) as Modalita_attivazione,")
                .AppendLine("       CAST(NULL AS DATETIME) as Data_primo_login,")
                .AppendLine("       CAST(NULL AS DATETIME) as Data_ultimo_login,")
                .AppendLine("       GETDATE() as Data_Estrazione")
                .AppendLine("FROM ( ")
                .AppendLine("   SELECT imp.piva, imp.Num_Operazioni_Campagna, imp.Num_Operazioni_Magazzino, imp.Num_PUA, imp.Num_PianiConcimazione, ")
                .AppendLine("       imp.Dichiarazione_NonUtilizzo_Fertilizzanti, imp.Dichiarazione_NonUtilizzo_Trattamenti, ")
                .AppendLine("       impreses.Regione, impreses.Provincia, impreses.Comune, impreses.Referente, impreses.RagioneSociale, ")
                .AppendLine("       impreses.Cuaa, ROW_NUMBER() OVER (PARTITION BY imp.Piva order by imp.Piva) AS RowNumber")
                .AppendLine("   FROM #Agr imp")
                .AppendLine("   LEFT JOIN #impreses impreses")
                .AppendLine("   ON imp.piva=impreses.piva")
                .AppendLine(") as a ")
                .AppendLine("WHERE a.RowNumber = 1 ")
                .AppendLine("ORDER BY a.RagioneSociale");

            return stbQuery
                .AppendLine(string.Empty)
                .ToString();
        }

        private string? RimozioneTabelleTemporanee()
        {
            var stbQuery = new StringBuilder();
            stbQuery
                .AppendLine(RimozioneTabellaTemporanea("#PraticheData"))
                .AppendLine(RimozioneTabellaTemporanea("#Agr"))
                .AppendLine(RimozioneTabellaTemporanea("#impreses"))
                .AppendLine(RimozioneTabellaTemporanea("#aziende"))
                .AppendLine(RimozioneTabellaTemporanea("#totalString"));
            return stbQuery.ToString();
        }

        private string? RimozioneTabellaTemporanea(string tabella)
        {
            var stbQuery = new StringBuilder();
            stbQuery
                .AppendLine($"IF OBJECT_ID('tempdb..{tabella}') IS NOT NULL")
                .AppendLine($"   DROP TABLE {tabella};");
            return stbQuery.ToString();
        }

        private string OperazioniMagazzino(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA,")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND Movimenti.cau_mov In ('7300')")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       AND   Agenda.Inviato >=0")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc");
            return stbQuery.ToString();
        }

        private string OpreazioniCampagna(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA, ")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) As operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND Movimenti.cau_mov In ('2050','2100','2200','2300')")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       AND Agenda.Inviato >=0")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc");
            return stbQuery.ToString();
        }

        private string OperazioniCampagnaTrattamenti(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA, ")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva ");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND   Movimenti.cau_mov IN ('2050')")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc ");
            return stbQuery.ToString();
        }

        private string OperazioniCampagnaConcimazioni(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA, ")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva ");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND   Movimenti.cau_mov IN ('2300')")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc ");
            return stbQuery.ToString();
        }

        private string OperazioniCampagnaAltro(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA, ")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva ");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND   Movimenti.cau_mov IN ('2100','2200')")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc ");
            return stbQuery.ToString();
        }

        private string OperazioniPUA(bool attivaFiltroPiva, string filtroUtenti)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT PUA_Testata.PIVA,")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM PUA_Testata WITH(NOLOCK)")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON PUA_Testata.piva=imprese.piva");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            stbQuery.AppendLine("       WHERE PUA_Testata.Validita_Inizio <= @dataValiditaFine")
                .AppendLine("       AND PUA_Testata.Validita_Fine >= @dataValiditaInizio")
                .AppendLine($"       AND PUA_Testata.Username_Creazione {filtroUtenti}")
                .AppendLine("       AND PUA_Testata.Inviato >=0")
                .AppendLine("       GROUP BY PUA_Testata.piva, Imprese.rag_soc");
            return stbQuery.ToString();
        }

        private string OperazioniPianiConcimazione(bool attivaFiltroPiva, string filtroUtenti)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT PianoConcimazione_Dettagli.PC_Dettagli_PIVA as piva,")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM PianoConcimazione_Dettagli WITH(NOLOCK)")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON PianoConcimazione_Dettagli.PC_Dettagli_PIVA = imprese.piva");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            stbQuery
                .AppendLine("       WHERE PianoConcimazione_Dettagli.Validita_Inizio <= @dataValiditaFine")
                .AppendLine("       AND PianoConcimazione_Dettagli.Validita_Fine >= @dataValiditaInizio")
                .AppendLine($"       AND PianoConcimazione_Dettagli.Username_Creazione {filtroUtenti}")
                .AppendLine("       AND PianoConcimazione_Dettagli.Inviato >=0")
                .AppendLine("       GROUP BY PianoConcimazione_Dettagli.PC_Dettagli_PIVA, Imprese.rag_soc");
            return stbQuery.ToString();
        }

        private string SenzaUtilizzoFertilizzanti(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA,")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND Agenda.Lav_Cod = 165 ")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       AND Agenda.Inviato >=0")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc");
            return stbQuery.ToString();
        }

        private string SenzaUtilizzoTrattamenti(bool attivaFiltroPiva, string filtroUtenti, enum_FiltroDateStatisticheUtilizzo filtroPerDataCompetenzaOrDataRegistrazione)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("       SELECT DISTINCT Agenda.PIVA,")
                .AppendLine("           imprese.rag_soc,")
                .AppendLine("           COUNT(*) as operazioni")
                .AppendLine("       FROM agenda WITH(NOLOCK)")
                .AppendLine("       INNER JOIN Movimenti WITH(NOLOCK)")
                .AppendLine("       ON Agenda.PIVA = Movimenti.PIVA")
                .AppendLine("       AND Agenda.Id_Agenda = Movimenti.Id_Agenda")
                .AppendLine("       INNER JOIN imprese WITH(NOLOCK)")
                .AppendLine("       ON agenda.piva=imprese.piva");

            if (attivaFiltroPiva)
                stbQuery.AppendLine("JOIN #aziende")
                    .AppendLine("ON imprese.piva = #aziende.azienda COLLATE SQL_Latin1_General_CP850_CI_AS");

            switch (filtroPerDataCompetenzaOrDataRegistrazione)
            {
                case enum_FiltroDateStatisticheUtilizzo.DataCompetenza: // 1
                    stbQuery.AppendLine("       WHERE Movimenti.data_movimento <= @dataValiditaFine")
                        .AppendLine("       AND   Movimenti.data_movimento >= @dataValiditaInizio");
                    break;
                case enum_FiltroDateStatisticheUtilizzo.DataRegistrazione: // 2
                    stbQuery.AppendLine("       WHERE agenda.data_creazione <= @dataValiditaFine")
                        .AppendLine("       AND agenda.data_creazione >= @dataValiditaInizio");
                    break;
                default:
                    break;
            }
            stbQuery.AppendLine("       AND   Agenda.Lav_Cod = 166")
                .AppendLine($"       AND Agenda.Username_Creazione {filtroUtenti}")
                .AppendLine("       AND Agenda.Inviato >=0")
                .AppendLine("       GROUP BY agenda.piva, Imprese.rag_soc");
            return stbQuery.ToString();
        }

    }
}
