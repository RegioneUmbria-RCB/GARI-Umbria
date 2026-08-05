using AgronicaDataProvider6.Interfaces;
using AgronicaDataProvider6.Settings;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SuperServer.DAL.Scheduler
{
    public interface IServiziBackground
    {
        Task<DataTable> GetQuartzJobs(string schedulerName, AgronicaCoreParametri ParametriSuperServer);
        Task<DataTable> GetQuartzJobsRestart(string schedulerName, AgronicaCoreParametri parametriSuperServer);
        Task<string?> GetStatoJobDaSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri parametriInterscambio);
        Task<DataTable?> GetUltimoTentativo(string? pivaSuperUser, int? jobId, AgronicaCoreParametriSuperServer parametriSuperServer);
        Task<bool> ImpostaBloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, DateTime? avvio, AgronicaCoreParametri objParametriInterscambio);
        Task<bool> ImpostaSbloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri objParametriInterscambio);
        Task<bool> ImpostaNuovoTentativo(string? pivaSuperUser, int? jobId, string? instanceId, string? pathLog, DateTime? avvio,
            int? numeroTentativo, AgronicaCoreParametriSuperServer parametriSuperServer);
        Task<bool> ImpostaEsitoTentativo(string? pivaSuperUser, int? jobId, string? instanceId, string esito, 
            AgronicaCoreParametriSuperServer parametriSuperServer);
        Task<bool> DisattivaQuartzJobs(string? pivaSuperuser, int? jobId, AgronicaCoreParametriSuperServer parametriSuperServer);
        Task<DataTable?> LeggiLog(string? instanceId, AgronicaCoreParametriSuperServer parametriSuperServer);
        Task ImpostaResetTentativi(string? pivaSuperuser, int? jobId, string oldInstance, AgronicaCoreParametriSuperServer parametriSuperServer);
        Task ImpostaRiattivazioneJob(string? pivaSuperuser, int? jobId, AgronicaCoreParametriSuperServer parametriSuperServer);
    }
}
