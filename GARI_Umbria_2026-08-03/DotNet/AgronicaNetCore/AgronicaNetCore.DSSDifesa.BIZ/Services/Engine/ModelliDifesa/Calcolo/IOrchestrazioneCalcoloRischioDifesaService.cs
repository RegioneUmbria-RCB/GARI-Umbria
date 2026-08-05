using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Calcolo
{
    /// <summary>
    /// Contratto per l'orchestrazione completa del calcolo del rischio fitosanitario (DS07-BL).
    /// </summary>
    public interface IOrchestrazioneCalcoloRischioDifesaService
    {
        /// <summary>
        /// Esegue il flusso completo: acquisizione meteo, recupero infestanti, invio modelli di difesa paralleli.
        /// </summary>        /// 
        /// <exception cref="Exceptions.CalcoloRischioDifesaRequestValidationException">Se la query SQL supera il <paramref name="timeoutMs"/>.</exception>
        /// <exception cref="Exceptions.CalcoloRischioDifesaDataReadFataleException">Se la query SQL supera il <paramref name="timeoutMs"/>.</exception>
        /// <exception cref="Exceptions.CalcoloRischioDifesaDataReadTimeoutException">Se la query SQL supera il <paramref name="timeoutMs"/>.</exception>
        Task<OrchestrazioneCalcoloRischioDifesaResponse> CalcolaRischioAvversitaCompletaAsync(
            OrchestrazioneCalcoloRischioDifesaRequest input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
