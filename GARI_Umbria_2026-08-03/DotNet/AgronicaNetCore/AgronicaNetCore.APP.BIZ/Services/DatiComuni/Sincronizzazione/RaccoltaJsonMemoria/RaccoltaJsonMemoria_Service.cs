using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggiornamentoAtomicoPreparazione;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Options;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;

/// <summary>
/// In-memory buffer for JSON snapshots of all 26+6 common tables, scoped to a single job cycle.
/// Not thread-safe; designed for single-threaded sequential job execution.
/// Ref: DS04-BL – Raccolta JSON in Memoria per Aggiornamento Atomico.
/// </summary>
public sealed class RaccoltaJsonMemoriaService : IRaccoltaJsonMemoriaService
{
    private const double BytesPerMb = 1024.0 * 1024.0;

    private readonly ILoggingService _loggingService;
    private readonly IAggiornamentoAtomicoPreparazioneService _aggiornamentoAtomico;
    private readonly RaccoltaJsonMemoriaSettings _settings;

    // Key = nomeTabella (case-insensitive to prevent duplicates due to casing drift).
    private readonly Dictionary<string, RaccoltaEntry> _buffer =
        new(StringComparer.OrdinalIgnoreCase);

    private RaccoltaJsonStato _stato = RaccoltaJsonStato.InProgress;
    private long _memoriaAllocataBytes;

    public RaccoltaJsonMemoriaService(
        ILoggingService loggingService,
        IAggiornamentoAtomicoPreparazioneService aggiornamentoAtomico,
        IOptions<RaccoltaJsonMemoriaSettings> settings)
    {
        _loggingService = loggingService;
        _aggiornamentoAtomico = aggiornamentoAtomico;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public Task<RaccoltaJsonMemoriaResult> AggiungiAsync(
        string nomeTabella,
        string jsonNuovo,
        DateTime timestampGenerazione,
        bool exists)
    {
        // Ref: DS04-BL – Eccezioni: CollectionStateException.
        if (_stato != RaccoltaJsonStato.InProgress)
            throw new CollectionStateException(
                $"Impossibile aggiungere '{nomeTabella}': la collezione è in stato '{_stato}'. Ref: DS04-BL.");

        // Deduct previous allocation for the same table if overwriting.
        if (_buffer.TryGetValue(nomeTabella, out var existing))
            _memoriaAllocataBytes -= existing.StimaBytes;

        var entry = new RaccoltaEntry(nomeTabella, jsonNuovo, timestampGenerazione, exists);
        _buffer[nomeTabella] = entry;
        _memoriaAllocataBytes += entry.StimaBytes;

        // Ref: DS04-BL – Eccezioni: MemoryExceededException (soglia configurabile).
        if (_memoriaAllocataBytes > _settings.MaxThresholdBytes)
            throw new MemoryExceededException(
                $"Allocazione buffer JSON supera la soglia massima di {_settings.MaxThresholdBytes / BytesPerMb:F0} MB " +
                $"({_memoriaAllocataBytes / BytesPerMb:F1} MB in uso). Ref: DS04-BL.");

        // Ref: DS04-BL – Regole di Business: warning log se supera 1 GB.
        if (_memoriaAllocataBytes > _settings.WarnThresholdBytes)
            _loggingService.LogWarning(
                "Buffer JSON in-memory supera {WarnMb:F0} MB: {CurrentMb:F1} MB allocati per {Count} tabelle. Ref: DS04-BL.",
                null,
                null,
                _settings.WarnThresholdBytes / BytesPerMb,
                _memoriaAllocataBytes / BytesPerMb,
                _buffer.Count);

        return Task.FromResult(BuildResult());
    }

    /// <inheritdoc/>
    public async Task CommitAsync(AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS04-BL – Regole di Business: Commit() persiste in transazione atomica via DS05.
        var entries = _buffer.Values
            .Select(e => new PreparazioneDatiComuniEntry(e.NomeTabella, e.JsonNuovo, e.TimestampGenerazione, e.Exists))
            .ToList();

        _loggingService.LogInformation("Commit di {Count} tabelle via DS05. Ref: DS04-BL.", objParametriServer, null, entries.Count);

        await _aggiornamentoAtomico.AggiornaAsync(entries, objParametriServer);

        _buffer.Clear();
        _memoriaAllocataBytes = 0;
        _stato = RaccoltaJsonStato.ReadyForCommit;

        _loggingService.LogInformation("Commit completato con successo.", objParametriServer, null);
    }

    /// <inheritdoc/>
    public void Rollback()
    {
        // Ref: DS04-BL – Regole di Business: Rollback() elimina completamente la collezione in-memory.
        int count = _buffer.Count;
        _buffer.Clear();
        _memoriaAllocataBytes = 0;
        _stato = RaccoltaJsonStato.RolledBack;

        _loggingService.LogWarning("Rollback eseguito: {Count} tabelle scartate senza persistenza. Ref: DS04-BL.", null, null, count);
    }

    /// <inheritdoc/>
    public RaccoltaJsonMemoriaResult GetStato() => BuildResult();

    private RaccoltaJsonMemoriaResult BuildResult() =>
        new(_buffer.Count, _memoriaAllocataBytes / BytesPerMb, _stato);

    /// <summary>
    /// Internal buffer entry. Memory footprint is approximated as UTF-16 string size
    /// (2 bytes/char) plus a conservative object-header overhead of 40 bytes.
    /// </summary>
    private sealed class RaccoltaEntry
    {
        public string NomeTabella { get; }
        public string JsonNuovo { get; }
        public DateTime TimestampGenerazione { get; }
        public bool Exists { get; }
        public long StimaBytes { get; }

        public RaccoltaEntry(string nomeTabella, string jsonNuovo, DateTime timestampGenerazione, bool exists)
        {
            NomeTabella = nomeTabella;
            JsonNuovo = jsonNuovo;
            TimestampGenerazione = timestampGenerazione;
            Exists = exists;
            StimaBytes = (long)jsonNuovo.Length * 2 + 40;
        }
    }
}
