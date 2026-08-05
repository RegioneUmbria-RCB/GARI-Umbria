using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RecuperoPayloadM4Token
{
    /// <summary>
    /// Contratto per la business logic <c>RecuperoPayloadM4DaLookupToken</c> (DS09-BL).
    /// Espone due operazioni distinte: recupero degli anni disponibili per una filiera
    /// e recupero della tabella "Token Generabili" per filiera + anno.
    /// </summary>
    /// <remarks>
    /// Riferimento spec: DS09-BL RecuperoPayloadM4DaLookupToken.
    /// Il Cono di Visibilità Utente viene rivalutato ad ogni invocazione (no cache).
    /// </remarks>
    public interface IRecuperoPayloadM4TokenService
    {
        /// <summary>
        /// DS09-BL §Regola 1 — Restituisce gli anni distinti per cui esiste almeno
        /// un'invocazione M4 in modalità "Aziendale" visibile all'utente,
        /// ordinati per anno decrescente.
        /// </summary>
        /// <param name="filiera">P.IVA della filiera selezionata (obbligatorio).</param>
        /// <param name="objParametriServer">Parametri server (connessione DB, cultura, log).</param>
        /// <param name="objParametriUtenti">Parametri utente per il Cono di Visibilità.</param>
        /// <returns>Lista di anni disponibili, ordinata decrescente. Lista vuota se nessun dato.</returns>
        Task<List<int>> GetAnniDisponibiliAsync(
            string filiera,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// DS09-BL §Regola 2/3 — Restituisce la tabella "Token Generabili" per la filiera
        /// e l'anno indicati: una riga per azienda, corrispondente all'invocazione più recente.
        /// </summary>
        /// <param name="filiera">P.IVA della filiera selezionata (obbligatorio).</param>
        /// <param name="anno">Anno campagna (obbligatorio).</param>
        /// <param name="objParametriServer">Parametri server (connessione DB, cultura, log).</param>
        /// <param name="objParametriUtenti">Parametri utente per il Cono di Visibilità.</param>
        /// <returns>
        /// Lista di <see cref="TokenGenerabileItem"/>, ordinata per azienda ASC poi
        /// data invocazione DESC. Lista vuota se nessun dato disponibile per l'anno.
        /// </returns>
        Task<List<TokenGenerabileItem>> GetTokenGenerabiliAsync(
            string filiera,
            int anno,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
