using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.PerimetroRaccolti
{
    public interface IPerimetroRaccoltiRischiService
    {
        Task<PerimetroRaccoltiResult> GetPerimetroAsync(
            PerimetroRaccoltiRequest request,
            AgronicaCoreParametriServer objParametriServer);
    }
}
