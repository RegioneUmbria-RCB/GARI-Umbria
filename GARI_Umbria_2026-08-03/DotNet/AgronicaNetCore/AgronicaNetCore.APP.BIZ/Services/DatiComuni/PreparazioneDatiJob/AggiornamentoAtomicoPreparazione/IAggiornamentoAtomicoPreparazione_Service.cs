using AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggiornamentoAtomicoPreparazione;

/// <summary>
/// Persists the in-memory JSON collection atomically in <c>app_preparazione_daticomuni_web2app</c>
/// using a SERIALIZABLE SQL transaction with deadlock retry logic.
/// Ref: DS05-BL – Aggiornamento Atomico Tabella Preparazione Dati.
/// </summary>
public interface IAggiornamentoAtomicoPreparazioneService
{
    /// <summary>
    /// Executes a SERIALIZABLE transaction that UPDATEs one row per entry in
    /// <paramref name="jsonRaccolti"/>. Retries once on deadlock (5 s delay).
    /// Rolls back fully on any unrecoverable error.
    /// Ref: DS05-BL – Regole di Business.
    /// </summary>
    /// <param name="jsonRaccolti">Entries collected by DS04.</param>
    /// <param name="objParametriServer">Server connection parameters.</param>
    /// <exception cref="Exceptions.TransactionTimeoutException">Transaction exceeded the configured timeout.</exception>
    /// <exception cref="Exceptions.DeadlockException">Deadlock persisted after max retries.</exception>
    /// <exception cref="Exceptions.AtomicUpdateFailedException">UPDATE failed and rollback was performed.</exception>
    Task<AggiornamentoAtomicoResult> AggiornaAsync(
        IReadOnlyCollection<PreparazioneDatiComuniEntry> jsonRaccolti,
        AgronicaCoreParametriServer objParametriServer);
}
