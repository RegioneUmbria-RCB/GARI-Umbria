using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.CalcoloBilancioIdrico
{
    /// <summary>
    /// Contratto per il calcolo del bilancio idrico H2O.
    /// Implementato nel successivo DS: riceve il perimetro e gli esercizi,
    /// orchestra la logica di calcolo e restituisce il risultato serializzato.
    /// Riferimento spec: DS-02.2-BL Chiamata WebApi Calcolo Bilancio Idrico.
    /// </summary>
    public interface ICalcoloSostenibilitaH2OService
    {
        /// <summary>
        /// Esegue il calcolo del bilancio idrico per il perimetro e gli esercizi forniti.
        /// </summary>
        /// <param name="request">Perimetro di calcolo ed esercizi territoriali.</param>
        /// <param name="objParametriServer">Parametri di connessione al database GIAS.</param>
        /// <param name="objParametriSuperServer">Parametri super-server per la configurazione.</param>
        /// <returns>Risultato serializzato in JSON del calcolo del bilancio idrico.</returns>
        Task<string> CalcoloSostenibilitaH2OAsync(
            CalcoloBilancioIdricoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
