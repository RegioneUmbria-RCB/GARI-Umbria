namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggregazioneJSONRisposta;

/// <summary>
/// Result returned by <see cref="IAggregazioneJSONRispostaService.AggregaAsync"/>.
/// Ref: DS09-BL – Output.
/// </summary>
public sealed class AggregazioneJSONRispostaResult
{
    /// <summary>
    /// The aggregated JSON string in the form <c>{ "tabella1": {...}, "tabella2": {...}, ... }</c>.
    /// Ref: DS09-BL – Output: payload_aggregato.
    /// </summary>
    public string PayloadAggregatoJson { get; }

    /// <summary>
    /// Optional gzip-compressed bytes of <see cref="PayloadAggregatoJson"/>.
    /// <c>null</c> when compression was not requested.
    /// Ref: DS09-BL – Regole di Business: Se abilita_compressione = true.
    /// </summary>
    public byte[]? PayloadCompresso { get; }

    /// <summary>
    /// UTC timestamp captured at the moment of aggregation.
    /// Ref: DS09-BL – Output: timestamp_server.
    /// </summary>
    public DateTime TimestampServer { get; }

    /// <summary>
    /// Number of tables included in <see cref="PayloadAggregatoJson"/>.
    /// Ref: DS09-BL – Output: numero_tabelle.
    /// </summary>
    public int NumeroTabelle { get; }

    /// <summary>
    /// Size in bytes of the uncompressed <see cref="PayloadAggregatoJson"/> (UTF-8).
    /// Ref: DS09-BL – Output: dimensione_bytes.
    /// </summary>
    public long DimensioneBytes { get; }

    /// <summary>
    /// Size in bytes of <see cref="PayloadCompresso"/>, or <c>0</c> when compression
    /// was not requested.
    /// Ref: DS09-BL – Output: dimensione_compressa_bytes.
    /// </summary>
    public long DimensioneCompressaBytes { get; }

    /// <summary>
    /// Total milliseconds elapsed during the aggregation (DB read + JSON build + optional gzip).
    /// Ref: DS09-BL – Output: aggregation_time_ms.
    /// </summary>
    public long AggregationTimeMs { get; }

    public AggregazioneJSONRispostaResult(
        string payloadAggregatoJson,
        byte[]? payloadCompresso,
        DateTime timestampServer,
        int numeroTabelle,
        long dimensioneBytes,
        long dimensioneCompressaBytes,
        long aggregationTimeMs)
    {
        PayloadAggregatoJson = payloadAggregatoJson;
        PayloadCompresso = payloadCompresso;
        TimestampServer = timestampServer;
        NumeroTabelle = numeroTabelle;
        DimensioneBytes = dimensioneBytes;
        DimensioneCompressaBytes = dimensioneCompressaBytes;
        AggregationTimeMs = aggregationTimeMs;
    }
}
