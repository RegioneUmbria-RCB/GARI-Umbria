using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA
{
    /// <summary>
    /// Contratto BIZ per la verifica dell'unicità delle fasi fenologiche BBCH ricevute
    /// dall'engine rispetto ai dati storici registrati nel QDCA (Quaderno di Campagna).
    /// Restituisce la lista di oggetti <c>Attivita</c> per le sole fasi nuove da registrare.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS02-BL ConfrontiFasiFenologicheQDCA — Scopo e Descrizione.
    /// FR006 — Verifica e sincronizzazione delle fasi fenologiche BBCH.
    /// </remarks>
    public interface IConfrontiFasiFenologicheQDCAService
    {
        /// <summary>
        /// Legge le fasi fenologiche storiche nel QDCA per l'impianto indicato,
        /// confronta le fasi ricevute usando il codice BBCH come chiave univoca,
        /// e restituisce gli oggetti <c>Attivita</c> per le fasi non ancora presenti.
        /// </summary>
        /// <param name="input">Dati dell'impianto e lista delle fasi fenologiche da confrontare.</param>
        /// <param name="objParametriServer">Parametri server per l'accesso al database.</param>
        /// <param name="cancellationToken">Token di cancellazione.</param>
        /// <returns>
        /// Risultato con le fasi nuove, le fasi duplicate e gli oggetti Attivita da registrare.
        /// </returns>
        /// <remarks>
        /// Design Specification: DS02-BL — Regole di Business.
        /// </remarks>
        Task<ConfrontiFasiFenologicheQDCAResult> ConfrontaAsync(
            ConfrontiFasiFenologicheQDCAInput input,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default);
    }
}
