using AgronicaCoreDTOStd.InData.Gis;
using AgronicaDataProvider6.Settings;
using AgronicaNetCore.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SuperServer.BIZ.Scheduler
{
    public interface IServiziBackgroundService
    {
        AgronicaCoreParametriSuperServer ParametriSuperServer { get; }
        Task<DataTable> GetQuartzJobs(string schedulerName, bool attivaEsempi);
        Task<DataTable?> GetUltimoTentativo(string? pivaSuperUser, int? jobId);
        Task<bool> ImpostaNuovoTentativo(string? pivaSuperuser, int? jobId, string? instanceId, string? pathLog, DateTime? avvio, int? numeroTentativo);
        Task<bool> DisattivaQuartzJobs(string? pivaSuperuser, int? jobId);
        Task<bool> IsSemaforoVerde(string? pivaSuperuser, int? jobId, AgronicaCoreParametri objParametriInterscambio);
        Task<bool> ImpostaBloccoJobPerSemaforo(string? pivaSuperuser, int? jobId, DateTime? avvio, AgronicaCoreParametri objParametriInterscambio);
        Task<bool> ImpostaSbloccoPerTentativi(string? pivaSuperuser, int? jobId, string? instanceId, bool? esito);
        Task<bool> ImpostaSbloccoJobPerSemaforo(string? pivaSuperUser, int? jobId, AgronicaCoreParametri agronicaCoreParametri);
        Task<DataTable?> LeggiLog(string? instanceId);
        Task<DataTable> GetQuartzJobsRestart(string schedulerName, bool attivaEsempi);
        Task ImpostaResetTentativi(string? pivaSuperuser, int? jobId, string oldInstance);
        Task ImpostaRiattivazioneJob(string? pivaSuperuser, int? jobId);
    }
}
