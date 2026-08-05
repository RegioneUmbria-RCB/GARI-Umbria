using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.FasiFenologicheRegistrate
{
    /// <summary>
    /// Contratto BIZ per l'estrazione cronologica delle fasi fenologiche registrate
    /// su un appezzamento nell'anno corrente.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS20-BL GetPhenologicalPhasesChronological — Scopo.
    /// DS21-API POST /api/fasi-fenologiche-registrate — Dipendenze Business Logic.
    /// Supporta il bottone "Visualizza fasi fenologiche" del widget DSS Nutrizione (FR011).
    /// </remarks>
    public interface IGetPhenologicalPhasesChronologicalService
    {
        /// <summary>
        /// Estrae l'elenco completo delle fasi fenologiche registrate nell'intervallo indicato
        /// per l'appezzamento descritto da <paramref name="request"/>,
        /// ordinato cronologicamente in modo ascendente, con deduplica per (CodBbch, Data_Movimento).
        /// </summary>
        /// <param name="request">
        /// Richiesta contenente i dati dell'appezzamento (output di DS05-BL CaricamentoDatiWidgetNutrizione)
        /// e l'intervallo di validità (ValiditaInizio, ValiditaFine) per il filtro su Data_Movimento.
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al server GIAS.</param>
        /// <param name="cancellationToken">Token di cancellazione della richiesta HTTP.</param>
        /// <returns>
        /// Lista ordinata di <see cref="FaseFenologicaCorrenteDto"/>.
        /// Vuota se nessuna fase è registrata nell'intervallo (non è un errore).
        /// </returns>
        /// <remarks>
        /// Design Specification: DS20-BL — Dettaglio Procedurale; DS21-API — Processing Logic.
        /// </remarks>
        Task<IReadOnlyList<FaseFenologicaCorrenteDto>> EseguiAsync(
            FasiFenologicheRequest request,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default);
    }
}
