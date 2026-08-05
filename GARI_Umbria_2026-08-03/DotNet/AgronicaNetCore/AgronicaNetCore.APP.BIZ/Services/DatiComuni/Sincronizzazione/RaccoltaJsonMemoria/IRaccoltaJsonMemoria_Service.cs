namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// Maintains an in-memory buffer of JSON snapshots for all 26+6 common tables during a single
/// job cycle, guaranteeing a fully atomic commit or complete rollback.
/// Ref: DS04-BL – Raccolta JSON in Memoria per Aggiornamento Atomico.
/// </summary>
public interface IRaccoltaJsonMemoriaService
{
    /// <summary>
    /// Adds or replaces the JSON snapshot for <paramref name="nomeTabella"/> in the buffer.
    /// Monitors memory allocation: warns above the configured threshold and throws if the
    /// hard limit is exceeded.
    /// Ref: DS04-BL – Regole di Business: aggiungere entry alla collezione.
    /// </summary>
    /// <exception cref="Exceptions.CollectionStateException">
    /// Buffer is not in <see cref="RaccoltaJsonStato.InProgress"/> state.
    /// </exception>
    /// <exception cref="Exceptions.MemoryExceededException">
    /// Buffer allocation exceeds the configured maximum threshold.
    /// </exception>
    Task<RaccoltaJsonMemoriaResult> AggiungiAsync(
        string nomeTabella,
        string jsonNuovo,
        DateTime timestampGenerazione,
        bool exists);

    /// <summary>
    /// Delegates all buffered entries to <see cref="PreparazioneDatiJob.AggiornamentoAtomicoPreparazione.IAggiornamentoAtomicoPreparazioneService"/>
    /// for atomic SERIALIZABLE persistence, then transitions the buffer to
    /// <see cref="RaccoltaJsonStato.ReadyForCommit"/>.
    /// Ref: DS04-BL – Regole di Business: metodo Commit(); DS05-BL – Aggiornamento Atomico.
    /// </summary>
    Task CommitAsync(Base.Models.AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Discards the entire in-memory buffer without any database write and transitions to
    /// <see cref="RaccoltaJsonStato.RolledBack"/>.
    /// Ref: DS04-BL – Regole di Business: metodo Rollback().
    /// </summary>
    void Rollback();

    /// <summary>
    /// Returns the current status snapshot of the buffer without modifying state.
    /// </summary>
    RaccoltaJsonMemoriaResult GetStato();
}
