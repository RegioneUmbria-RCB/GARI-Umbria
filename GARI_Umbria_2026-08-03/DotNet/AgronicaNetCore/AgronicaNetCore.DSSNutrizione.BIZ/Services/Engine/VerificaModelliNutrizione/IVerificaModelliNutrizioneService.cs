using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione
{
    /// <summary>
    /// Contratto del servizio BIZ per la verifica della disponibilità di modelli di calcolo
    /// nutrizionale per le coppie (specie, varietà) degli appezzamenti attivi dell'azienda.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Scopo.
    /// DS13-API POST /v1/dss/nutrizione/modelli/verifica.
    /// </summary>
    public interface IVerificaModelliNutrizioneService
    {
        /// <summary>
        /// Interroga l'engine di nutrizione in parallelo per ogni coppia (specie, varietà) univoca.
        /// Il fallimento su una singola coppia è isolato (status ERROR) e non interrompe le altre.
        /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Regole di Business.
        /// </summary>
        /// <param name="coppie">Coppie (specie, varietà) per cui verificare i modelli.</param>
        /// <param name="dataConsiglio">Data di riferimento del consiglio nutrizionale.</param>
        /// <param name="objParametriServer">Parametri server GIAS per accesso a DB e configurazione.</param>
        /// <param name="objParametriSuperServer">Parametri super-server GIAS per le chiavi engine.</param>
        /// <param name="cancellationToken">Token di cancellazione.</param>
        Task<VerificaModelliNutrizioneResult> VerificaAsync(
            IReadOnlyList<CoppiaSpecieVarieta> coppie,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
