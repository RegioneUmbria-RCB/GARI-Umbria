using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using Microsoft.Extensions.DependencyInjection;
using OutData.Kendo;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;
using AgronicaNetCore.Statistiche.DAL.DataLayer.Statistiche;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiDettagli;
using System.Data;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Pratiche.DAL.DataLayer;
using AgronicaNetCore.Utility.BIZ.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using OutData.Varie;
using Newtonsoft.Json;
using AgronicaNetCore.Base.Services.Security;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaCoreDTOStd.Identity;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.GerarchiaImprese;

namespace AgronicaNetCore.Statistiche.BIZ.Services.Statistiche
{


    public class ReportStatisticheService : BaseService, IReportStatisticheService
    {
        private readonly ISecurityLayerDAL _securityLayer;
        private readonly IAgenda _agendaDAL;
        private readonly IGerarchiaImprese _gerarchieImpreseDAL;
        private readonly IUtentiDettagli _utentiDAL;
        private readonly IPratica _praticaDAL;
        private readonly IReportStatistiche _reportStatisticheDAL;
        private readonly IEsportaAllegatoService _esportaAllegato;
        private readonly int codiceServizio;

        public ReportStatisticheService(IServiceProvider provider) : base(provider)
        {
            _securityLayer = provider.GetRequiredService<ISecurityLayerDAL>();
            _agendaDAL = provider.GetRequiredService<IAgenda>();
            _gerarchieImpreseDAL = provider.GetRequiredService<IGerarchiaImprese>();
            _reportStatisticheDAL = provider.GetRequiredService<IReportStatistiche>();
            _praticaDAL = provider.GetRequiredService<IPratica>();
            _utentiDAL = provider.GetRequiredService<IUtentiDettagli>();
            _esportaAllegato = provider.GetRequiredService<IEsportaAllegatoService>();
            codiceServizio = (int)enum_Servizi.QDemetraQdCBluarancio;
        }

        public async Task<string?> ImportReportElencoSinteticoAsync(int jobId, DateTime? avvio,
            int filtroPerDataCompetenzaOrDataRegistrazione, IntervalloTemporale intervalloOperazioniDiCampagna, 
            IntervalloTemporale intervalloPratiche, string? username, AgronicaCoreParametriUtenti objParametriUtenti, 
            AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            string? result = string.Empty;
            string swapTable = "dbo.StatisticheUtilizzoBackup";
            string targetTable = "dbo.StatisticheUtilizzo";
            try
            {
                bool isServizioBluArancio = true; // in questo modo si carica l'insieme massimo dei dati per le StatisticheUtilizzo                
                AgronicaCoreParametri? objParametriInterscambio = null;
                Exception? eccezione = null!;

                try
                {
                    string? interscambioConnectionString = await _securityLayer.LeggiConfigurazioneSitiScalareAsync("ConnessioneInterscambio",
                        objParametriServer!, objParametriSuperServer);
                    if (string.IsNullOrEmpty(interscambioConnectionString))
                        throw new Exception("Non è stata configurata la connessione verso il DB Interscambio");
                    else
                        objParametriInterscambio = new AgronicaCoreParametri(objParametriSuperServer.PivaSuperUser,
                            objParametriSuperServer.UsernameOperazione, objParametriSuperServer.UtenteUsername,
                            objParametriSuperServer.UtenteCodFiscale, objParametriSuperServer.SuperUserUsername,
                            objParametriSuperServer.FinestraTemporaleInizio, objParametriSuperServer.FinestraTemporaleFine,
                            objParametriSuperServer.FlagVisibilita, objParametriSuperServer.FlagCancellazioneLogica,
                            interscambioConnectionString, objParametriSuperServer.LogDirectory,
                            objParametriSuperServer.LogFileName, objParametriSuperServer.LogDescrizioneUtente);

                    bool isSemaforoVerde = await _reportStatisticheDAL.IsSemaforoVerde(objParametriServer.PivaSuperUser, jobId, objParametriInterscambio!);

                    if (!isSemaforoVerde)
                        throw new Exception("Il processo non può essere avviato, perchè si sta attendendo il termine di un processo precedente");
                    else
                    {
                        await _reportStatisticheDAL.ImpostaBloccoJobPerSemaforo(objParametriServer.PivaSuperUser, jobId, avvio, objParametriInterscambio);

                        Task task = Task.Run(async () =>
                        {
                            DataTable? data = await ExtractDataReportStatisticheUtilizzoAsync(filtroPerDataCompetenzaOrDataRegistrazione,
                                intervalloOperazioniDiCampagna, intervalloPratiche, isServizioBluArancio, username, null,
                                objParametriUtenti, objParametriServer);

                            if (data != null)
                            {
                                try
                                {
                                    eccezione = await EseguiBulkInsert(swapTable, data, objParametriInterscambio);
                                    if (eccezione == null)
                                    {
                                        eccezione = await EseguiCopyFromSwapTable(swapTable, targetTable, objParametriInterscambio);
                                        if (eccezione != null)
                                            eccezione = await EseguiClearSwapTableAsync(swapTable, eccezione, objParametriInterscambio);
                                    }
                                }
                                finally
                                {
                                    if (objParametriInterscambio != null)
                                        await _reportStatisticheDAL.ImpostaSbloccoJobPerSemaforo(
                                            objParametriServer.PivaSuperUser, jobId, objParametriInterscambio);
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    eccezione = ex;
                }
                finally
                {
                    if (eccezione != null)
                        result = eccezione.Message;
                }

            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        public async Task<objAllegato?> ExportReportElencoSinteticoAsync(
            int filtroPerDataCompetenzaOrDataRegistrazione, IntervalloTemporale intervalloOperazioniDiCampagna,
            IntervalloTemporale intervalloPratiche, bool dettagliQdC, string? username, string? piva,
            AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, 
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            objAllegato? result = null;
            try
            {
                bool isServizioBluArancio = await _praticaDAL.IsServizioBluArancioAsync(
                    codiceServizio,
                    intervalloOperazioniDiCampagna.inizio,
                    intervalloOperazioniDiCampagna.fine,
                    objParametriServer);

                DataTable? data = await ExtractDataReportStatisticheUtilizzoAsync(filtroPerDataCompetenzaOrDataRegistrazione,
                    intervalloOperazioniDiCampagna, intervalloPratiche, isServizioBluArancio, username, piva,
                    objParametriUtenti, objParametriServer);


                if (data != null)
                {
                    if (!isServizioBluArancio)
                    {
                        data.Columns.Remove("Data_sottoscrizione_QdC");
                        data.Columns.Remove("Modalita_attivazione");
                        data.Columns.Remove("Data_primo_login");
                        data.Columns.Remove("Data_ultimo_login");
                    }

                    if (!dettagliQdC)
                    {
                        data.Columns.Remove("Dichiarazione_NonUtilizzo_Fertilizzanti");
                        data.Columns.Remove("Dichiarazione_NonUtilizzo_Trattamenti");
                    }

                    string nomeFile = "ElencoSintetico_Movimenti_PerAzienda";
                    if (dettagliQdC)
                        nomeFile = "ElencoSintetico_Movimenti_PerAziendaDettagli";
                    var marckerFile = DateTime.Now.ToString("yyyyMMdd_HHmmssffff");
                    nomeFile = $"{nomeFile}__{marckerFile}";

                    string? pathFile = await _esportaAllegato.EsportaExcelPath(data, nomeFile, objParametriServer, objParametriSuperServer);
                    if (!string.IsNullOrEmpty(pathFile))
                        result  = await _esportaAllegato.ImpostaObjAllegato(pathFile, objParametriServer);
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        private async Task<DataTable?> ExtractDataReportStatisticheUtilizzoAsync(
            int filtroPerDataCompetenzaOrDataRegistrazione, IntervalloTemporale intervalloOperazioniDiCampagna,
            IntervalloTemporale intervalloPratiche, bool isServizioBluArancio, string? username, string? piva,
            AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer)
        {
            DataTable? result = null;
            try
            {
                int codiceStato = (int)enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Azienda_Attivata;
                string? stringaConnessioneUtenti = _securityService!.GetConnectionString(objParametriUtenti.StringaConnessione);
                string? nomeDbUtenti = stringaConnessioneUtenti.Split(";")[2].Split("=")[1];
                string superUser = objParametriUtenti.SuperUserUsername;
                string filtroUsername = $" <> '{objParametriServer.PivaSuperUser}'";
                if (!string.IsNullOrEmpty(username))
                    filtroUsername = $" = '{username}'";

                string? filtroPiva = null;
                if (!string.IsNullOrEmpty(piva) && piva != objParametriServer.PivaSuperUser)
                {
                    var impreseFiglie = await _gerarchieImpreseDAL.LeggiElencoGerarchiaImpreseFiglieAsync(piva!, objParametriServer);
                    if (impreseFiglie != null && impreseFiglie.Any())
                        filtroPiva = string.Join(",", impreseFiglie);
                }

                enum_FiltroDateStatisticheUtilizzo filtroDateStatisticheUtilizzo = enum_FiltroDateStatisticheUtilizzo.DataCompetenza;
                if (Enum.GetValues(typeof(enum_FiltroDateStatisticheUtilizzo)).Cast<int>().Any(v => v == filtroPerDataCompetenzaOrDataRegistrazione))
                    filtroDateStatisticheUtilizzo = (enum_FiltroDateStatisticheUtilizzo)filtroPerDataCompetenzaOrDataRegistrazione;

                // in questo caso sono i parametri dataInizioCampagna/dataFineCampagna
                result = await _reportStatisticheDAL.LeggiDettagliAsync(
                    filtroDateStatisticheUtilizzo,
                    intervalloOperazioniDiCampagna.inizio,
                    intervalloOperazioniDiCampagna.fine,
                    filtroUsername,
                    filtroPiva,
                    objParametriServer);

                DataTable? praticheStatistiche = null;
                DataTable? primoEdUltimoAccessoUtente = null;
                if (isServizioBluArancio)
                {
                    praticheStatistiche = await _praticaDAL.ReadPraticheStatisticheAsync(
                        codiceServizio,
                        codiceStato,
                        intervalloPratiche.inizio,
                        intervalloPratiche.fine,
                        objParametriServer);

                    primoEdUltimoAccessoUtente = await _utentiDAL.LeggiPrimoEdUltimoAccessoUtentiAsync(objParametriUtenti);
                }
                DataTable utenti = await _agendaDAL.ReadUtentiOperazioniAsync(
                    nomeDbUtenti, CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE,
                    objParametriServer);

                if (result != null)
                {
                    if (praticheStatistiche != null && praticheStatistiche.AsEnumerable().Any())
                    {
                        (
                            from dett in result.AsEnumerable()
                            join pratStat in praticheStatistiche.AsEnumerable()
                            on dett["piva"].ToString() equals pratStat["Piva"].ToString()
                            into joinDettagliPraticheStatistiche
                            from dettPratStat in joinDettagliPraticheStatistiche.DefaultIfEmpty()
                            select ImpostaCampiPratiche(dett, dettPratStat)
                        ).ToList()
                        .ForEach(row =>
                        {
                            if (utenti != null)
                            {
                                string? piva = row["piva"].ToString();
                                int numeroUtenti = 0;
                                string elencoUtenti = ElencoUtentiOperazioni(piva, superUser, utenti, out numeroUtenti);
                                row["N_Utenti"] = numeroUtenti;
                                row["Utenti"] = elencoUtenti;
                            }

                            if (primoEdUltimoAccessoUtente != null)
                            {
                                string? cuaa = row["cuaa"].ToString();
                                DateTime? primoLogin = null;
                                DateTime? ultimoLogin = null;
                                GetPrimoUltimoAccesso(cuaa, primoEdUltimoAccessoUtente, out primoLogin, out ultimoLogin);
                                if (primoLogin.HasValue)
                                    row["Data_primo_login"] = primoLogin.Value.Date;
                                if (ultimoLogin.HasValue)
                                    row["Data_ultimo_login"] = ultimoLogin.Value.Date;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return result;
        }

        private async Task<Exception> EseguiBulkInsert(string swapTable, DataTable data, AgronicaCoreParametri objParametriInterscambio)
        {
            Exception result = null!;
            try
            {
                await OpenConnectionAsync(objParametriInterscambio);
                await _reportStatisticheDAL.BulkInsertAsync(swapTable, data, objParametriInterscambio);
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriInterscambio, true);
                result = new Exception("Non è stato possibile eseguire l'aggiornamento delle statistiche sulla tabella di swap", ex);
            }
            finally
            {
                CloseConnection(objParametriInterscambio);
            }
            return result;
        }

        private async Task<Exception?> EseguiCopyFromSwapTable(string swapTable, string targetTable, AgronicaCoreParametri objParametriInterscambio)
        {
            Exception result = null!;
            try
            {
                await OpenConnectionAsync(objParametriInterscambio);
                await _reportStatisticheDAL.CopyFromSwapTableAsync(swapTable, targetTable, objParametriInterscambio);
            }
            catch (Exception ex)
            {
                CloseTransaction(objParametriInterscambio, true);
                result = new Exception("Non è stato possibile portare le statistiche aggiornate sulla tabella ufficiale", ex);
            }
            finally
            {
                CloseConnection(objParametriInterscambio);
            }
            return result;
        }

        private async Task<Exception> EseguiClearSwapTableAsync(string swapTable, Exception eccezione, AgronicaCoreParametri objParametriInterscambio)
        {
            try
            {
                await OpenConnectionAsync(objParametriInterscambio);
                await _reportStatisticheDAL.ClearSwapTableAsync(swapTable, objParametriInterscambio);
            }
            catch
            {
                CloseTransaction(objParametriInterscambio, true);
                eccezione = new Exception("Non è stato possibile ripulire la tabella di swap", eccezione);
            }
            finally
            {
                CloseConnection(objParametriInterscambio);
            }
            return eccezione;
        }
                
        private DataRow ImpostaCampiPratiche(DataRow dett, DataRow dettPratStat)
        {
            if (dettPratStat != null)
            {
                if ((dettPratStat["DataInizio"] != null && dettPratStat["DataInizio"] != DBNull.Value))
                    dett["Data_sottoscrizione_QdC"] =  ((DateTime)dettPratStat["DataInizio"]).Date;
                dett["Modalita_attivazione"] = dettPratStat["Stato_Des"].ToString();
            }
            return dett;
        }

        private void GetPrimoUltimoAccesso(string? cuaa, DataTable? primoEdUltimoAccessoUtente, out DateTime? primoLogin, out DateTime? ultimoLogin)
        {
            //DT: Sviluppo ad hoc Coldiretti: esiste un solo utente per ogni azienda agricola, ed ha il codice fiscale coincidente con il CUAA
            primoLogin = null;
            ultimoLogin = null;

            if (primoEdUltimoAccessoUtente != null && !string.IsNullOrEmpty(cuaa))
            {
                var row = primoEdUltimoAccessoUtente.AsEnumerable()
                    .Where(x => x["CodFisc"].ToString()!.Equals(cuaa, StringComparison.CurrentCultureIgnoreCase))
                    .Select(x => new
                    {
                        PrimoLogin = (DateTime)x["Data_PrimoAccesso"],
                        UltimoLogin = (DateTime)x["Data_UltimoAccesso"]
                    })
                    .FirstOrDefault();

                if (row != null)
                {
                    primoLogin = row.PrimoLogin;
                    ultimoLogin = row.UltimoLogin;
                }
            }
        }

        private string? GetDModalitaAttivazione(string? piva, DataTable? praticheStatoAttuale)
        {
            string? result = null;

            if (praticheStatoAttuale != null)
            {
                result = praticheStatoAttuale.AsEnumerable()
                    .Where(x => x["piva"].Equals(piva) &&
                        x["Stato_Des"] != null)
                    .OrderByDescending(x => (DateTime)x["Validita_Inizio_Stato"]) 
                    .Select(x => x["Stato_Des"].ToString())
                    .FirstOrDefault();
            }

            return result;

            
        }

        private DateTime? GetDataSottosrizioneQdC(string? piva, DataTable? praticheSottoscrizioneQdC)
        {
            DateTime? result = null;

            if (praticheSottoscrizioneQdC != null) {
                var dateSottosrizione = praticheSottoscrizioneQdC.AsEnumerable()
                    .Where(x => x["Piva"].Equals(piva) &&
                        x["Validita_Inizio_Stato"] != null)
                    .Select(x => (DateTime)x["Validita_Inizio_Stato"])
                    .ToList();

                if (dateSottosrizione != null && dateSottosrizione.Any())
                    result = dateSottosrizione.Max();
            }

            return result;            
        }

        private string ElencoUtentiOperazioni(string? piva, string superUser, DataTable utenti, out int numeroUtenti)
        {
            string result = string.Empty;
            numeroUtenti = 0;

            var elencoUtenti = utenti.AsEnumerable()
                .Where(x => x["piva"].Equals(piva))
                .Select(x => superUser.Equals(x["UserName"].ToString()!, StringComparison.CurrentCultureIgnoreCase) ? 
                    "": (string.IsNullOrEmpty(x["UserName"].ToString()!) ? "Utente Senza Nome" : x["Utente"].ToString()!))
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (elencoUtenti.Any())
            {
                numeroUtenti = elencoUtenti.Count();
                result = string.Join(",", elencoUtenti);
            }
            
            return result;
        }

    }
}
