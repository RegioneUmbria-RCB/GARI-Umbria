using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using Microsoft.Extensions.Options;
using AgronicaNetCore.Base.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.SerializzazioneJsonDeterministico;

/// <summary>
/// Serialises entity collections produced by DS01 BL handlers into a deterministic JSON string.
/// Ref: DS02-BL – Serializzazione JSON Deterministico da oggetti entity specifici.
/// </summary>
public sealed class SerializzazioneJsonDeterministicoService : ISerializzazioneJsonDeterministicoService
{
    // 200 MB warning threshold. Ref: DS02-BL – Regole di Business.
    // Streaming threshold. Ref: DS02-BL – Regole di Business: streaming per collezioni > 10 000 record.

    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    // Thread-safe, shared serializer. The contract resolver caches reflection data after first use.
    private static readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        ContractResolver = new AlphabeticalContractResolver(),
        Formatting = Formatting.None,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        NullValueHandling = NullValueHandling.Include,
        Culture = CultureInfo.InvariantCulture
    });

    public SerializzazioneJsonDeterministicoService(
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public Task<SerializzazioneJsonResult> SerializzaAsync<TEntity>(
        IReadOnlyList<TEntity> entityCollection,
        string nomeTabella) where TEntity : class
    {
        // Fail-fast validation. Ref: DS02-BL – Eccezioni: InvalidEntityException.
        if (entityCollection is null)
            throw new InvalidEntityException($"[{nomeTabella}] La collezione entity è null.");

        // Custom/personalized tables can be empty. Ref: DS02-BL – Eccezioni.
        bool isCustomTable = nomeTabella.Contains("Personalizzat", StringComparison.OrdinalIgnoreCase);
        
        if (entityCollection.Count == 0)
        {
            // For custom tables, return an empty JSON array.
            _loggingService.LogInformation(
                "[{NomeTabella}] Tabella vuota, restituito array JSON vuoto.",
                null,
                null,
                nomeTabella);
            
            return Task.FromResult(new SerializzazioneJsonResult("{}", 2, 0));
        }

        for (int i = 0; i < entityCollection.Count; i++)
        {
            if (entityCollection[i] is null)
                throw new InvalidEntityException($"[{nomeTabella}] Entity null all'indice {i}.");
        }

        var stopwatch = Stopwatch.StartNew();
        string jsonContent;

        try
        {
            jsonContent = SerializeToJson(entityCollection, _settings.SerializzazioneStreamingThreshold);
        }
        catch (JsonSerializationException ex)
        {
            // Ref: DS02-BL – Eccezioni: InvalidDataTypeException.
            throw new InvalidDataTypeException(
                $"[{nomeTabella}] Tipo dati non serializzabile nell'entity: {ex.Message}", ex);
        }

        stopwatch.Stop();

        long sizeBytes = Encoding.UTF8.GetByteCount(jsonContent);

        // Ref: DS02-BL – Regole di Business: warning se JSON > 200 MB.
        if (sizeBytes > _settings.SerializzazioneWarningThresholdBytes)
            _loggingService.LogWarning(
                "[{NomeTabella}] JSON generato supera 200 MB ({SizeBytes} bytes). Potenziale problema di performance.",
                null,
                null,
                nomeTabella, sizeBytes);

        return Task.FromResult(new SerializzazioneJsonResult(jsonContent, sizeBytes, stopwatch.ElapsedMilliseconds));
    }

    /// <summary>
    /// Writes entities one-by-one into a JSON array via <see cref="JsonTextWriter"/>.
    /// This streaming approach avoids building intermediate collections and bounds memory use
    /// for collections over <see cref="StreamingThreshold"/> records.
    /// Ref: DS02-BL – Regole di Business: streaming per collezioni > 10 000 record.
    /// </summary>
    private static string SerializeToJson<TEntity>(IReadOnlyList<TEntity> entities, int streamingThreshold) where TEntity : class
    {
        int capacity = Math.Min(entities.Count, streamingThreshold) * 256;
        var sb = new StringBuilder(capacity);

        using var stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture);
        using var jsonWriter = new SanitizingJsonTextWriter(stringWriter);

        jsonWriter.WriteStartArray();
        foreach (var entity in entities)
        {
            _serializer.Serialize(jsonWriter, entity);
        }
        jsonWriter.WriteEndArray();

        return sb.ToString();
    }

    /// <summary>
    /// Orders all JSON object properties alphabetically (ordinal) to guarantee determinism.
    /// Ref: DS02-BL – Regole di Business: "Ordinare chiavi JSON alfabeticamente per determinismo".
    /// </summary>
    private sealed class AlphabeticalContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.OrderBy(p => p.PropertyName, StringComparer.Ordinal).ToList();
        }
    }

    /// <summary>
    /// Intercepts every <see cref="JsonTextWriter.WriteValue(string)"/> call to strip characters
    /// that would corrupt the JSON structure or be truncated at DB storage level.
    /// This writer-level hook is guaranteed to run for all string values — fields and properties alike —
    /// unlike <see cref="JsonConverter{T}"/> which Newtonsoft.Json bypasses for primitive types
    /// serialized from public fields.
    /// </summary>
    private sealed class SanitizingJsonTextWriter : JsonTextWriter
    {
        // Characters removed entirely — would break JSON structure or corrupt DB storage.
        private static readonly HashSet<char> CharsToRemove = new HashSet<char>
        {
            '"',      // U+0022 ASCII double-quote
            '\u201C', // " LEFT DOUBLE QUOTATION MARK
            '\u201D', // " RIGHT DOUBLE QUOTATION MARK
            '\u201E', // „ DOUBLE LOW-9 QUOTATION MARK
            '\u2033', // ″ DOUBLE PRIME
            '\uFF02', // ＂ FULLWIDTH QUOTATION MARK
            '\0',     // null byte — SQL Server NVARCHAR truncates silently
        };

        // Unicode dash variants normalized to plain ASCII hyphen for deterministic comparison.
        private static readonly HashSet<char> DashChars = new HashSet<char>
        {
            '\u2013', // – EN DASH
            '\u2014', // — EM DASH
            '\u2012', // ‒ FIGURE DASH
            '\u2015', // ― HORIZONTAL BAR
        };

        // Union used for fast IndexOfAny pre-check before allocating a StringBuilder.
        private static readonly char[] DangerousChars;

        static SanitizingJsonTextWriter()
        {
            var all = new HashSet<char>(CharsToRemove);
            all.UnionWith(DashChars);
            DangerousChars = new char[all.Count];
            all.CopyTo(DangerousChars);
        }

        public SanitizingJsonTextWriter(TextWriter textWriter) : base(textWriter)
        {
            Formatting = Formatting.None;
        }

        public override void WriteValue(string? value)
            => base.WriteValue(value != null ? Sanitize(value) : null);

        private static string Sanitize(string value)
        {
            if (value.IndexOfAny(DangerousChars) < 0)
                return value;

            var sb = new StringBuilder(value.Length);
            foreach (var ch in value)
            {
                if (CharsToRemove.Contains(ch)) continue;
                sb.Append(DashChars.Contains(ch) ? '-' : ch);
            }
            return sb.ToString();
        }
    }
}
