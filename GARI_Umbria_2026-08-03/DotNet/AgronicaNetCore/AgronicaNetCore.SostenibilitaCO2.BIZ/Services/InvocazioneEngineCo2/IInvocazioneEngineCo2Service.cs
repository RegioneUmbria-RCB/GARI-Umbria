using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.InvocazioneEngineCo2
{
    /// <summary>
    /// Contratto per la business logic di invocazione del motore esterno M4
    /// (Engine Sostenibilità CO2).
    /// <para>
    /// Legge URL e token Bearer da <c>Configurazione_Siti</c>, invia il payload validato
    /// via HTTP POST con timeout 30s, gestisce le risposte di errore differenziate
    /// (400/401/403/429/5xx/timeout) e persiste richiesta e risposta nelle tabelle di lookup.
    /// </para>
    /// Riferimento spec: DS07-API InvocazioneEngineSOstenibilitàCO2.
    /// </summary>
    public interface IInvocazioneEngineCo2Service
    {
        /// <summary>
        /// Invoca il motore M4 con il payload validato e persiste il tentativo nelle
        /// tabelle di lookup.
        /// </summary>
        /// <param name="input">
        /// Payload M4 validato (DS06-BL) e contesto di invocazione (filiera, anno, modalità,
        /// username per audit).
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <returns>
        /// <see cref="RispostaEngineCo2"/> con gli indicatori CO2 restituiti dal motore.
        /// </returns>
        /// <exception cref="Exceptions.SostenibilitaCO2.EngineCo2Exception">
        /// Sollevata su risposta HTTP 400/401/403/429/5xx o timeout.
        /// Il campo <c>Codice</c> identifica la categoria di errore.
        /// </exception>
        Task<RispostaEngineCo2> InvokeAsync(InvocazioneEngineCo2Input input, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
