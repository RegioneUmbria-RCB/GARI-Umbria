using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.CalcoloSostenibilitaCO2
{
    /// <summary>
    /// Contratto per l'orchestrazione del calcolo di sostenibilità CO2 (DS03→DS06→DS07).
    /// Esegue in sequenza: assembly del payload M4 (DS03-BL + DS04-BL), validazione finale
    /// (DS06-BL) e invocazione del motore esterno (DS07-API).
    /// </summary>
    public interface ICalcoloSostenibilitaCO2Service
    {
        /// <summary>
        /// Esegue il flusso completo di calcolo CO2: assembly → validazione → invocazione motore M4.
        /// </summary>
        /// <param name="input">Dati aggregati di input (perimetro, consumi, filiera, anno, modalità).</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <param name="username">Username dell'utente autenticato, usato per audit sul motore M4.</param>
        /// <returns><see cref="RispostaEngineCo2"/> con gli indicatori CO2 restituiti dal motore.</returns>
        /// <exception cref="Exceptions.SostenibilitaCO2.DataNotFoundException">
        /// Se un'azienda del perimetro non è trovata in tabella <c>Aziende</c>.
        /// </exception>
        /// <exception cref="Exceptions.SostenibilitaCO2.PayloadStructureException">
        /// Se il payload assemblato non è strutturalmente valido.
        /// </exception>
        /// <exception cref="Exceptions.SostenibilitaCO2.ValidazioneFinalePayloadCo2Exception">
        /// Se la validazione finale (DS06-BL) rileva errori bloccanti.
        /// Porta l'output completo con la lista degli errori per il mapping HTTP 422.
        /// </exception>
        /// <exception cref="Exceptions.SostenibilitaCO2.SerializationException">
        /// Se il payload non è serializzabile a JSON valido (errore tecnico di assembly).
        /// </exception>
        /// <exception cref="Exceptions.SostenibilitaCO2.EngineCo2Exception">
        /// Se il motore M4 restituisce HTTP 4xx/5xx o si verifica un timeout.
        /// </exception>
        Task<RispostaEngineCo2> CalcoloSostenibilitaCO2Async(
            AssemblyPayloadCo2Input input,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer
            );
    }
}
