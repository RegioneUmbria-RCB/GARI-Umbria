namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.SerializzazioneJsonDeterministico;

/// <summary>
/// Output of <see cref="ISerializzazioneJsonDeterministicoService.SerializzaAsync{TEntity}"/>.
/// Ref: DS02-BL – Output.
/// </summary>
public sealed class SerializzazioneJsonResult
{
    /// <summary>UTF-8 normalised deterministic JSON string. Ref: DS02-BL – Output: json_content.</summary>
    public string JsonContent { get; }

    /// <summary>Size in bytes of <see cref="JsonContent"/> encoded as UTF-8. Ref: DS02-BL – Output: json_size_bytes.</summary>
    public long JsonSizeBytes { get; }

    /// <summary>Milliseconds taken by the serialisation. Ref: DS02-BL – Output: serialization_time_ms.</summary>
    public long SerializationTimeMs { get; }

    public SerializzazioneJsonResult(string jsonContent, long jsonSizeBytes, long serializationTimeMs)
    {
        JsonContent = jsonContent;
        JsonSizeBytes = jsonSizeBytes;
        SerializationTimeMs = serializationTimeMs;
    }
}
