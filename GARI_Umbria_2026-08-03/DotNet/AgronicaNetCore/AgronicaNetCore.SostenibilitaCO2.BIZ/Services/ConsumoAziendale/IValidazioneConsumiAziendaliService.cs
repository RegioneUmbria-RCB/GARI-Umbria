using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ConsumoAziendale
{
    /// <summary>
    /// Contratto per la business logic <c>ValidazioneDatiConsumoAziendale</c> (DS02-BL).
    /// Valida server-side i dati di consumo aziendale (carburanti ed energia) immessi dall'utente
    /// rispetto al perimetro di calcolo selezionato e alle regole di business definite.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale.
    /// </summary>
    public interface IValidazioneConsumiAziendaliService
    {
        /// <summary>
        /// Valida i dati di consumo carburante ed energia immessi dall'utente.
        /// </summary>
        /// <param name="request">
        /// Input contenente il perimetro aziende (da DS01-BL),
        /// le righe carburante e le righe energia da validare.
        /// </param>
        /// <param name="objParametriServer">
        /// Parametri server correnti (connessione DB, cultura, path log).
        /// Necessari per la query sui tipi carburante validi.
        /// </param>
        /// <param name="objParametriUtenti">Parametri utente correnti.</param>
        /// <returns>
        /// <see cref="ValidazioneConsumiAziendaliResult"/> con l'esito della validazione.
        /// Se <c>ValidazioneEsito = true</c>, <c>ConsumiValidati</c> è valorizzato
        /// e pronto come input per DS03-BL.
        /// </returns>
        Task<ValidazioneConsumiAziendaliResult> ValidaAsync(
            ValidazioneConsumiAziendaliRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
