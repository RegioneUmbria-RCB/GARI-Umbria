using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione
{
    /// <summary>
    /// Contratto del servizio BIZ per la richiesta del consiglio nutrizionale per un appezzamento.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Scopo.
    /// </summary>
    public interface IRichiestaConsiglioNutrizioneService
    {
        /// <summary>
        /// Invia la richiesta all'engine di nutrizione, esegue il polling del risultato
        /// e persiste il consiglio nel database GIAS.
        /// Il fallimento per un singolo appezzamento non blocca gli altri (errore isolato).
        /// Riferimento: DS04-BL — Regole di Business.
        /// </summary>
        /// <param name="input">Dati aggregati dell'appezzamento (impianto, fenologia, terreno, fertilizzazioni).</param>
        /// <param name="usernameRichiedente">Username dell'utente che esegue la richiesta.</param>
        /// <param name="objParametriServer">Parametri server GIAS.</param>
        /// <param name="objParametriSuperServer">Parametri super-server GIAS per le chiavi engine.</param>
        /// <param name="cancellationToken">Token di cancellazione.</param>
        Task<RichiestaConsiglioNutrizioneResult> EseguiAsync(
            RichiestaConsiglioNutrizioneInput input,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            int IdDbServer,
            bool salvaConsiglio = true,
            int raccoglitoreCod = 0,
            CancellationToken cancellationToken = default);
    }
}
