using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Base.DataLayer.Security
{
    public interface ISecurityLayerDAL
    {
        Task<DataTable> LeggiConnessioniAsync(AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<DataTable> LeggiConfigurazioneSitiAsync(string chiave, AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<DataTable> LeggiConfigurazioneSitiAsync(string chiave, AgronicaCoreParametriServer objParametriServer);
        Task<DataTable> LeggiConfigurazioneSitiAsync(List<string> chiavi, AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<DataTable> LeggiConfigurazioneSitiAsync(List<string> chiavi, AgronicaCoreParametriServer objParametriServer);
        Task<bool> AggiornaValoriAsync(List<string> chiavi, List<string> valori, AgronicaCoreParametriServer objParametriServer);
        Task<bool> AggiornaValoriAsync(List<string> chiavi, List<string> valori, AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<string> LeggiConfigurazioneSitiScalareAsync(string chiave, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<(string UrlEngine, string ApiKey)> RecuperaConfigurazioneEngineAsync(string ChiaveUrlEngine,string ChiaveApiKey,AgronicaCoreParametriServer objParametriServer,AgronicaCoreParametriSuperServer objParametriSuperServer);
        Task<(string UrlEngine, string ApiKey, string TenantName)> RecuperaConfigurazioneEngineAsync(string ChiaveUrlEngine, string ChiaveApiKey, string ChiaveNameTenant, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
