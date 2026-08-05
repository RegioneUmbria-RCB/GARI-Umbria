using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.SerializzazioneJSONPreparazionePayloadRisposta;

/// <summary>
/// Represents the final DS05-BL output returned to the HTTP layer.
/// Ref: DS05-BL – Output.
/// </summary>
public sealed class SerializzazioneJSONPreparazionePayloadRispostaResult
{
    /// <summary>
    /// ISO 8601 server timestamp propagated from DS01-BL.
    /// Ref: DS05-BL – Output: timestamp_server.
    /// </summary>
    [JsonProperty("timestamp_server")]
    public string TimestampServer { get; }

    /// <summary>
    /// Serialized JSON payload containing the ordered Dati Azienda structure.
    /// Ref: DS05-BL – Output: data.
    /// </summary>
    [JsonProperty("data")]
    public string Data { get; }

    /// <summary>
    /// HTTP status code to return for the DS05-BL response.
    /// Ref: DS05-BL – Output: http_status_code.
    /// </summary>
    [JsonProperty("http_status_code")]
    public int HttpStatusCode { get; }

    /// <summary>
    /// HTTP headers prepared for the response. Includes the explicit JSON content type and,
    /// when generation succeeds, an ETag header.
    /// Ref: DS05-BL – Output: http_headers.
    /// </summary>
    [JsonProperty("http_headers")]
    public IReadOnlyDictionary<string, string> HttpHeaders { get; }

    /// <summary>
    /// Creates the DS05-BL result.
    /// Ref: DS05-BL – Output.
    /// </summary>
    public SerializzazioneJSONPreparazionePayloadRispostaResult(
        string timestampServer,
        string data,
        int httpStatusCode,
        IReadOnlyDictionary<string, string> httpHeaders)
    {
        TimestampServer = timestampServer;
        Data = data;
        HttpStatusCode = httpStatusCode;
        HttpHeaders = httpHeaders ?? throw new ArgumentNullException(nameof(httpHeaders));
    }
}