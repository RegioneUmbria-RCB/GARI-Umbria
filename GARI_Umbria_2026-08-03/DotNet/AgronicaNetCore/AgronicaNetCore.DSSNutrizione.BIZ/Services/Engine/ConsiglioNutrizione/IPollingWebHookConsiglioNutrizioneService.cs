using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione
{
    /// <summary>
    /// Contratto del servizio di polling sul WebHook_Testata per il consiglio nutrizionale.
    /// Riferimento: DS15-BL Polling WebHook Consiglio Nutrizione — Scopo.
    /// </summary>
    public interface IPollingWebHookConsiglioNutrizioneService
    {
        /// <summary>
        /// Esegue interrogazioni periodiche sul WebHook_Testata per il requestId dato,
        /// attendendo che lo Status raggiunga uno stato terminale (DONE o ERROR) o il timeout.
        /// Riferimento: DS15-BL — Regole di Business.
        /// </summary>
        /// <param name="input">Parametri di polling (requestId, timeout, intervallo).</param>
        /// <param name="objParametriServer">Parametri server GIAS.</param>
        /// <param name="cancellationToken">Token di cancellazione.</param>
        Task<PollingWebHookResult> EseguiPollingAsync(
            PollingWebHookInput input,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default);
    }
}
