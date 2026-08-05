using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Apezzamenti
{
    public interface IAppezzamenti
    {
        Task<bool> BloccaSbloccaAsync(bool block, Appezzamento.PK appezzamento, AgronicaCoreParametriServer objParametriServer, DateTime? blockDate);
        Task<bool> IsBlockedAsync(Appezzamento.PK appezzamento, DateTime atDate, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> GetAppezzamentixParticelleAsync(bool filtraChiaviTmp, AgronicaCoreParametriServer objParametriServer, bool soloChiavi = false);
    }
}