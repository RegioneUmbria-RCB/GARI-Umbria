namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.SerializzazioneJsonDeterministico;

/// <summary>
/// Transforms a collection of domain entities produced by DS01 BL handlers into a
/// deterministic, normalised JSON string.
/// Ref: DS02-BL – Serializzazione JSON Deterministico da oggetti entity specifici.
/// </summary>
public interface ISerializzazioneJsonDeterministicoService
{
    /// <summary>
    /// Serialises <paramref name="entityCollection"/> into a deterministic JSON string:
    /// alphabetically sorted keys, no extra whitespace, ISO 8601 UTC dates,
    /// invariant-culture decimals, JSON null for null values.
    /// Uses streaming writes for collections larger than 10 000 records.
    /// Ref: DS02-BL – Regole di Business.
    /// </summary>
    /// <typeparam name="TEntity">Entity type produced by a DS01 BL handler.</typeparam>
    /// <param name="entityCollection">Non-null, non-empty collection with no null entries.</param>
    /// <param name="nomeTabella">Table name used for logging / debug context.</param>
    /// <exception cref="Exceptions.InvalidEntityException">Collection is null, empty, or contains null entries.</exception>
    /// <exception cref="Exceptions.InvalidDataTypeException">An entity property type cannot be serialised to JSON.</exception>
    Task<SerializzazioneJsonResult> SerializzaAsync<TEntity>(
        IReadOnlyList<TEntity> entityCollection,
        string nomeTabella) where TEntity : class;
}
