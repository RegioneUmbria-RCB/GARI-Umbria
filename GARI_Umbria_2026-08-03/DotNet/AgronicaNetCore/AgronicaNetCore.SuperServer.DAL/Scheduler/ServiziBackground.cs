using AgronicaCoreModelsSTD.attivita;
using AgronicaDataProvider6.Interfaces;
using AgronicaDataProvider6.Settings;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SuperServer.DAL.Resources;
using InData.Log.AgronicaLogInvio;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgronicaNetCore.SuperServer.DAL.Scheduler
{
    public class ServiziBackground : DAL_Base, IServiziBackground
    {
        public ServiziBackground(IServiceProvider provider, bool securityBypass = false) : base(provider, securityBypass)
        {
        }

        public async Task<DataTable> GetQuartzJobs(string schedulerName, AgronicaCoreParametri parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("@schedulerName", schedulerName);
            DataTable result;

            stbQuery.AppendLine("SELECT c.PivaSuperuser, j.Id, j.NomeIstanza,")
                .AppendLine("   j.NomeIstanza + '_' + c.PivaSuperuser AS JobKey,")
                .AppendLine("   c.TokenUtente, c.Mail_Tecnica, ")
                .AppendLine("   c.NumeroMassimoIstanze, c.ApiTimeout,")
                .AppendLine("   c.NumeroMassimoTentativi, c.Cron,")
                .AppendLine("   c.ConfigurazioneLog, c.IdDB_Server,")
                .AppendLine("   c.Parametri_Input, c.Parametri_Output")
                .AppendLine("FROM Jobs j")
                .AppendLine("JOIN Jobs_Configurazione c")
                .AppendLine("ON j.Id = c.JobId")
                .AppendLine("WHERE c.Attivo = 1")
                .AppendLine("AND (c.SchedulerName = '*' OR c.SchedulerName = @schedulerName)");

            try
            {
                result = await GetDataProvider(parametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer, ex);
                throw;
            }
            return result;
        }

        public async Task<DataTable> GetQuartzJobsRestart(string schedulerName, AgronicaCoreParametri parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("@schedulerName", schedulerName);
            DataTable result;

            stbQuery.AppendLine("SELECT DISTINCT jei.Instance, jc.*, jei.Esito")
                .AppendLine("FROM(")
                .AppendLine("    SELECT c.PivaSuperuser, j.Id, j.NomeIstanza,")
                .AppendLine("       j.NomeIstanza + '_' + c.PivaSuperuser AS JobKey,")
                .AppendLine("       c.Attivo, c.Ripristina, c.ApiTimeout, c.TokenUtente, ")
                .AppendLine("       c.Mail_Tecnica, c.NumeroMassimoIstanze,")
                .AppendLine("       c.NumeroMassimoTentativi, c.Cron,")
                .AppendLine("       c.ConfigurazioneLog, c.IdDB_Server,")
                .AppendLine("       c.Parametri_Input, c.Parametri_Output")
                .AppendLine("    FROM Jobs j")
                .AppendLine("    JOIN Jobs_Configurazione c")
                .AppendLine("    ON j.Id = c.JobId")
                .AppendLine("    AND c.SchedulerName = @schedulerName")
                .AppendLine("    AND c.Ripristina = 1")
                .AppendLine(") jc")
                .AppendLine("LEFT JOIN(")
                .AppendLine("    SELECT tot.PivaSuperuser, tot.JobId, tot.Esito, tot.DataElaborazione, tot.Instance")
                .AppendLine("    FROM Jobs_Esito_Istanze tot")
                .AppendLine("    JOIN(")
                .AppendLine("        SELECT PivaSuperuser, JobId, MAX(DataElaborazione) DataElaborazione")
                .AppendLine("        FROM Jobs_Esito_Istanze")
                .AppendLine("        GROUP BY PivaSuperuser, JobId")
                .AppendLine("    ) ultimo")
                .AppendLine("    ON tot.PivaSuperuser = ultimo.PivaSuperuser")
                .AppendLine("    AND tot.JobId = ultimo.JobId")
                .AppendLine("    AND tot.DataElaborazione = ultimo.DataElaborazione")
                .AppendLine(") jei")
                .AppendLine("ON jc.PivaSuperuser = jei.PivaSuperuser")
                .AppendLine("AND jc.Id = jei.JobId");

            try
            {
                result = await GetDataProvider(parametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer, ex);
                throw;
            }
            return result;
        }

        public async Task<string?> GetStatoJobDaSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri parametriInterscambio)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("@pivaSuperUser", pivaSuperUser!);
            parametriSql.TryAdd("@jobId", jobId!);

            string? result = null;

            stbQuery.AppendLine("SELECT PivaSuperuser, JobId, Status")
                .AppendLine("FROM Semaforo")
                .AppendLine("WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND JobId = @jobId");

            try
            {
                DataTable istanze = await GetDataProvider(parametriInterscambio).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
                if (istanze != null && istanze.AsEnumerable().Any())
                    result = istanze.Rows[0].Field<string>("Status");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriInterscambio);
            }
            return result;
        }

        public async Task<DataTable?> GetUltimoTentativo(string? pivaSuperUser, int? jobId, AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("@pivaSuperUser", pivaSuperUser!);
            parametriSql.TryAdd("@jobId", jobId!);

            DataTable? result = null;

            stbQuery.AppendLine("SELECT jes.PivaSuperuser, jes.JobId, ")
                .AppendLine("   CASE WHEN jes.Esito = 'OK' THEN 0")
                .AppendLine("        WHEN jes.Esito = 'RESET' THEN 0")
                .AppendLine("   ELSE ISNULL(jes.NumeroTentativo, 0)")
                .AppendLine("   END NumeroTentativo")
                .AppendLine("FROM Jobs_Esito_Istanze jes")
                .AppendLine("JOIN(")
                .AppendLine("   SELECT PivaSuperuser, JobId, Max(DataElaborazione) DataElaborazione")
                .AppendLine("   FROM Jobs_Esito_Istanze")
                .AppendLine("   GROUP BY PivaSuperuser, JobId")
                .AppendLine(") ultimo")
                .AppendLine("ON jes.PivaSuperuser = ultimo.PivaSuperuser")
                .AppendLine("AND jes.JobId = ultimo.JobId")
                .AppendLine("AND jes.DataElaborazione = ultimo.DataElaborazione")
                .AppendLine("WHERE jes.PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND jes.JobId = @jobId");

            try
            {
                result = await GetDataProvider(parametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
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

        public async Task<bool> ImpostaNuovoTentativo(string? pivaSuperUser, int? jobId, string? instanceId, string? pathLog, DateTime? avvio,
            int? numeroTentativo, AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperUser);
            expandoObj.TryAdd("@jobId", jobId);
            expandoObj.TryAdd("@instanceId", instanceId);
            expandoObj.TryAdd("@pathLog", pathLog);
            expandoObj.TryAdd("@avvio", avvio);
            expandoObj.TryAdd("@numeroTentativo", numeroTentativo);

            bool result = false;

            stbQuery.AppendLine("INSERT INTO Jobs_Esito_Istanze")
                .AppendLine("(PivaSuperuser, JobId, Instance, PathLog, DataElaborazione, Esito, NumeroTentativo)")
                .AppendLine("VALUES")
                .AppendLine("(@pivaSuperUser, @jobId, @instanceId, @pathLog, @avvio, NULL, @numeroTentativo)");

            try
            {
                result = await GetDataProvider(parametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
            }
            return result;
        }

        public async Task<bool> ImpostaEsitoTentativo(string? pivaSuperUser, int? jobId, string? instanceId, string esito, 
            AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperUser);
            expandoObj.TryAdd("@jobId", jobId);
            expandoObj.TryAdd("@instanceId", instanceId);
            expandoObj.TryAdd("@esito", esito);

            bool result = false;

            stbQuery.AppendLine("UPDATE Jobs_Esito_Istanze")
                .AppendLine("SET Esito = @esito")
                .AppendLine("WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND JobId = @jobId")
                .AppendLine("AND Instance = @instanceId");

            try
            {
                result = await GetDataProvider(parametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
            }
            return result;
        }

        public async Task<bool> DisattivaQuartzJobs(string? pivaSuperuser, int? jobId, AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperuser);
            expandoObj.TryAdd("@jobId", jobId);
            bool result = false;

            stbQuery.AppendLine("UPDATE Jobs_Configurazione")
                .AppendLine("SET Attivo = 0")
                .AppendLine("WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND JobId = @jobId");

            try
            {
                result = await GetDataProvider(parametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
            }
            return result;
        }

        public async Task<DataTable?> LeggiLog(string? instanceId, AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var parametriSql = new Dictionary<string, object>();
            parametriSql.TryAdd("@instanceId", instanceId!);
            DataTable? result = null;

            stbQuery.AppendLine("SELECT jsl.InstanceId,")
                .AppendLine("   jei.Instance UltimaIstanza,")
                .AppendLine("   CASE WHEN jsl.InstanceId = jei.Instance THEN 'Errore' ELSE 'Blocco processo' END Motivo,")
                .AppendLine("   jc.Attivo,")
                .AppendLine("   jei.Esito,")
                .AppendLine("   jsl.SchedulerName,")
                .AppendLine("   jsl.JobName,")
                .AppendLine("   CONVERT(VARCHAR(20), jsl.TimeStamp, 20) TimeStamp,")
                .AppendLine("   jsl.Level,")
                .AppendLine("   jsl.Message")
                .AppendLine("FROM Jobs_Scheduler_Logs jsl")
                .AppendLine("JOIN Jobs_Configurazione jc")
                .AppendLine("ON jsl.PivaSuperuser = jc.PivaSuperuser")
                .AppendLine("AND jsl.JobId = jc.JobId")
                .AppendLine("LEFT JOIN (")
                .AppendLine("   SELECT tot.PivaSuperuser, tot.JobId, tot.Esito, tot.DataElaborazione, tot.Instance")
                .AppendLine("   FROM Jobs_Esito_Istanze tot")
                .AppendLine("   JOIN (")
                .AppendLine("       SELECT PivaSuperuser, JobId, MAX(DataElaborazione) DataElaborazione")
                .AppendLine("       FROM Jobs_Esito_Istanze")
                .AppendLine("       GROUP BY PivaSuperuser, JobId")
                .AppendLine("   ) ultimo")
                .AppendLine("   ON tot.PivaSuperuser = ultimo.PivaSuperuser")
                .AppendLine("   AND tot.JobId = ultimo.JobId")
                .AppendLine("   AND tot.DataElaborazione = ultimo.DataElaborazione")
                .AppendLine(") jei")
                .AppendLine("ON jsl.PivaSuperuser = jei.PivaSuperuser")
                .AppendLine("AND jsl.JobId = jei.JobId")
                .AppendLine("WHERE jsl.InstanceId = @instanceId");

            try
            {
                result = await GetDataProvider(parametriSuperServer).ExecuteReadAsync(stbQuery.ToString(), parametriSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
            }
            return result;
        }

        public async Task ImpostaResetTentativi(string? pivaSuperuser, int? jobId, string oldInstance, AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperuser);
            expandoObj.TryAdd("@jobId", jobId);
            expandoObj.TryAdd("@oldInstance", oldInstance);

            stbQuery.AppendLine("UPDATE Jobs_Esito_Istanze")
                .AppendLine("SET Esito = 'RESET'")
                .AppendLine("WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND JobId = @jobId")
                .AppendLine("AND Instance = @oldInstance");

            try
            {
                await GetDataProvider(parametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
            }
        }

        public async Task ImpostaRiattivazioneJob(string? pivaSuperuser, int? jobId, AgronicaCoreParametriSuperServer parametriSuperServer)
        {
            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@pivaSuperUser", pivaSuperuser);
            expandoObj.TryAdd("@jobId", jobId);

            stbQuery.AppendLine("UPDATE Jobs_Configurazione")
                .AppendLine("SET Attivo = 1,")
                .AppendLine("   Ripristina = 0")
                .AppendLine("WHERE PivaSuperuser = @pivaSuperUser")
                .AppendLine("AND JobId = @jobId");

            try
            {
                await GetDataProvider(parametriSuperServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, parametriSuperServer);
            }
        }
    }
}
