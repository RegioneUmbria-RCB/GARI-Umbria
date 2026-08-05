using AgronicaDataProvider6.Settings;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SuperServer.BIZ.Autenticazione;
using AgronicaNetCore.SuperServer.BIZ.Resources;
using AgronicaNetCore.SuperServer.DAL.Autenticazione;
using AgronicaNetCore.SuperServer.DAL.Scheduler;
using InData.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SuperServer.BIZ.Scheduler
{
    public class ServiziBackgroundService : BaseService, IServiziBackgroundService
    {
        private readonly IServiziBackground _serviziBackground;
        private string[] statiSemaforoAvvioJob = { "GREEN", "READING" };
        private string[] statiSemaforoStopJob = { "RED", "BLOCKED" };

        public ServiziBackgroundService(IServiceProvider provider) : base(provider)
        {
            _serviziBackground = _serviceProvider.GetRequiredService<IServiziBackground>();
            ParametriSuperServer = _securityService!.GetAgronicaCoreParametri();
        }

        public AgronicaCoreParametriSuperServer ParametriSuperServer { get; private set; }

        public async Task<DataTable> GetQuartzJobs(string schedulerName, bool attivaEsempi)
        {
            DataTable result = new DataTable();
            try
            {
                result = await _serviziBackground.GetQuartzJobs(schedulerName, ParametriSuperServer);
                if (attivaEsempi)
                    AggiungiEsempi(result);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<DataTable> GetQuartzJobsRestart(string schedulerName, bool attivaEsempi)
        {
            DataTable result = new DataTable();
            try
            {
                if (!attivaEsempi)
                    result = await _serviziBackground.GetQuartzJobsRestart(schedulerName, ParametriSuperServer);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<DataTable?> GetUltimoTentativo(string? pivaSuperUser, int? jobId)
        {
            DataTable? result = null;
            try
            {
                result = await _serviziBackground.GetUltimoTentativo(pivaSuperUser, jobId, ParametriSuperServer);
            }
            catch
            {
                throw;
            }
            return result;
        }

        public async Task<bool> ImpostaNuovoTentativo(string? pivaSuperuser, int? jobId, string? instanceId, string? pathLog, DateTime? avvio, int? numeroTentativo)
        {
            bool result = false;
            try
            {
                result = await _serviziBackground.ImpostaNuovoTentativo(pivaSuperuser, jobId, instanceId, pathLog, avvio, numeroTentativo, ParametriSuperServer);
            }
            catch
            {
                throw;
            }
            return result;
        }

        public async Task<bool> DisattivaQuartzJobs(string? pivaSuperuser, int? jobId)
        {
            bool result = false;
            try
            {
                result = await _serviziBackground.DisattivaQuartzJobs(pivaSuperuser, jobId, ParametriSuperServer);
            }
            catch
            {
                throw;
            }
            return result;
        }

        public async Task<bool> IsSemaforoVerde(string? pivaSuperuser, int? jobId, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            try
            {
                if (objParametriInterscambio != null)
                {
                    string? statoSemaforo = await _serviziBackground.GetStatoJobDaSemaforo(pivaSuperuser, jobId, objParametriInterscambio);
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

        public async Task<bool> ImpostaBloccoJobPerSemaforo(string? pivaSuperuser, int? jobId, DateTime? avvio, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            try
            {
                if (objParametriInterscambio != null)
                    result = await _serviziBackground.ImpostaBloccoJobPerSemaforo(pivaSuperuser, jobId, avvio, objParametriInterscambio);
                else
                    throw new Exception("La connessione verso 'Interscambio' non configurata, impossibile impostare lo stato del semaforo a 'Rosso'");
            }
            catch
            {
                throw;
            }
            return result;
        }

        public async Task<bool> ImpostaSbloccoPerTentativi(string? pivaSuperUser, int? jobId, string? instanceId, bool? esitoTentativo)
        {
            bool result = false;
            string? esito = "KO";
            try
            {
                if (esitoTentativo.HasValue && esitoTentativo.Value)
                    esito = "OK";
                result = await _serviziBackground.ImpostaEsitoTentativo(pivaSuperUser, jobId, instanceId, esito, ParametriSuperServer);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<bool> ImpostaSbloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri objParametriInterscambio)
        {
            bool result = false;
            try
            {
                if (objParametriInterscambio != null)
                    result = await _serviziBackground.ImpostaSbloccoJobPerSemaforo(pivaSuperUser, jobId, objParametriInterscambio);
                else
                    throw new Exception("La connessione verso 'Interscambio' non configurata, impossibile imposare lo stato del semaforo a 'Verde'");
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<DataTable?> LeggiLog(string? instanceId)
        {
            DataTable? result = null;
            try
            {
                Thread.Sleep(10000);
                result = await _serviziBackground.LeggiLog(instanceId, ParametriSuperServer);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task ImpostaResetTentativi(string? pivaSuperuser, int? jobId, string oldInstance)
        {
            try
            {
                await _serviziBackground.ImpostaResetTentativi(pivaSuperuser, jobId, oldInstance, ParametriSuperServer);
            }
            catch
            {
                throw;
            }
        }

        public async Task ImpostaRiattivazioneJob(string? pivaSuperuser, int? jobId)
        {
            try
            {
                await _serviziBackground.ImpostaRiattivazioneJob(pivaSuperuser, jobId, ParametriSuperServer);
            }
            catch
            {
                throw;
            }
        }

        private void AggiungiEsempi(DataTable result)
        {
            if (result != null)
            {
                result.Rows.Clear();

                string parametriInputBase = "{ \"Token\": null}";
                string logConfig = "{\"SaveOnDb\":false,\"PathLogFile\": \"Log\\\\AgronicaJobScheduler\\\\.txt\",\"RestrictedToMinimumLevel\":\"Debug\",\"RecordTemplate\":\"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}\",\"RollingInterval\":\"Day\",\"RollOnFileSizeLimit\":false,\"FileSizeLimitBytes\":null}";

                DataRow jobSenzaOutput = result.NewRow();
                jobSenzaOutput["PivaSuperuser"] = ParametriSuperServer.PivaSuperUser;
                jobSenzaOutput["Id"] = -1;
                jobSenzaOutput["NomeIstanza"] = "JobProcessoSenzaOutput";
                jobSenzaOutput["JobKey"] = $"JobProcessoSenzaOutput_{ParametriSuperServer.PivaSuperUser}";
                jobSenzaOutput["TokenUtente"] = string.Empty;
                jobSenzaOutput["NumeroMassimoIstanze"] = 1;
                jobSenzaOutput["NumeroMassimoTentativi"] = 1;
                jobSenzaOutput["Cron"] = "0 * * * * ?";
                jobSenzaOutput["Mail_Tecnica"] = null;
                jobSenzaOutput["ConfigurazioneLog"] = logConfig;
                jobSenzaOutput["IdDB_Server"] = 0;
                jobSenzaOutput["Parametri_Input"] = parametriInputBase;
                jobSenzaOutput["Parametri_Output"] = null;
                result.Rows.Add(jobSenzaOutput);

                DataRow jobConAllegato = result.NewRow();
                jobConAllegato["PivaSuperuser"] = ParametriSuperServer.PivaSuperUser;
                jobConAllegato["Id"] = -2;
                jobConAllegato["NomeIstanza"] = "JobProcessoConAllegato";
                jobConAllegato["JobKey"] = $"JobProcessoConAllegato_{ParametriSuperServer.PivaSuperUser}";
                jobConAllegato["TokenUtente"] = string.Empty;
                jobConAllegato["NumeroMassimoIstanze"] = 1;
                jobConAllegato["NumeroMassimoTentativi"] = 1;
                jobConAllegato["Cron"] = "0 * * * * ?";
                jobConAllegato["Mail_Tecnica"] = null;
                jobConAllegato["ConfigurazioneLog"] = logConfig;
                jobConAllegato["IdDB_Server"] = 0;
                jobConAllegato["Parametri_Input"] = parametriInputBase;
                jobConAllegato["Parametri_Output"] = "{ \"OutputFile\": null, \"Directory\": null, \"IsOk\": false }";
                result.Rows.Add(jobConAllegato);

                DataRow jobInvioMail = result.NewRow();
                jobInvioMail["PivaSuperuser"] = ParametriSuperServer.PivaSuperUser;
                jobInvioMail["Id"] = -3;
                jobInvioMail["NomeIstanza"] = "JobProcessoInvioMail";
                jobInvioMail["JobKey"] = $"JobProcessoInvioMail_{ParametriSuperServer.PivaSuperUser}";
                jobInvioMail["TokenUtente"] = string.Empty;
                jobInvioMail["NumeroMassimoIstanze"] = 1;
                jobInvioMail["NumeroMassimoTentativi"] = 1;
                jobInvioMail["Cron"] = "* * * * * ?";
                jobInvioMail["Mail_Tecnica"] = null;
                jobInvioMail["ConfigurazioneLog"] = logConfig;
                jobInvioMail["IdDB_Server"] = 0;
                jobInvioMail["Parametri_Input"] = parametriInputBase;
                jobInvioMail["Parametri_Output"] = "{ \"Email\": { \"From\": \"service@agronicagroup.it\", \"To\": [\"m.cecalupo@fastcode.it\"], \"ToConcatenated\": \"\", \"Cc\": [], \"CcConcatenated\": \"\", \"Ccn\": [], \"CcnConcatenated\": \"\", \"Subject\": \"Test invio mail\", \"Text\": \"Processo completato\", \"IsBodyHtml\": true, \"Attachments\": []} }";
                result.Rows.Add(jobInvioMail);

            }
        }
    }
}
