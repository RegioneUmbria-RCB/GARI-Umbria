using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.SerializzazioneJSONPreparazionePayloadRisposta;

/// <summary>
/// Implements the final DS05-BL step that serializes the ordered Dati Azienda payload and
/// prepares the HTTP response metadata.
/// Ref: DS05-BL – Nome: SerializzazioneJSONPreparazionePayloadRisposta.
/// </summary>
public sealed class SerializzazioneJSONPreparazionePayloadRispostaService
    : ISerializzazioneJSONPreparazionePayloadRispostaService
{
    private const string ContentTypeHeaderName = "Content-Type";
    private const string ETagHeaderName = "ETag";
    private const string JsonContentType = "application/json; charset=utf-8";
    private const int SuccessHttpStatusCode = 200;

    private static readonly string[] FixedPayloadOrder =
    {
        "azienda",
        "centriAziendali",
        "pianoColturale",
        "campi",
        "magazzini",
        "contatti",
        "risorseUmane",
        "squadre",
        "fornitori",
        "risorseFornitori",
        "parcoMacchine",
        "prodotti",
        "progetti",
        "centriAziendaliAttivitaCDG",
        "impreseImpostazioni",
        "attivitaCDG",
        "lavorazioniAttivitaCDG",
        "areeTipologie",
        "tipologie",
        "codificaProdotti",
        "misureAvversitaAnagrafiche",
        "misureIndiciMaturitaAnagrafiche",
        "operazioneCausale"
    };

    private static readonly JsonSerializer Serializer = JsonSerializer.Create(
        new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
        });

    private readonly ILoggingService _loggingService;

    /// <summary>
    /// Creates the DS05-BL service instance.
    /// Ref: DS05-BL – Descrizione.
    /// </summary>
    public SerializzazioneJSONPreparazionePayloadRispostaService(
        ILoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    /// <inheritdoc/>
    public SerializzazioneJSONPreparazionePayloadRispostaResult Prepara(
        SerializzazioneJSONPreparazionePayloadRispostaInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        string payloadJson = BuildPayloadJson(input.DatiTabelle);

        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [ContentTypeHeaderName] = JsonContentType
        };

        return new SerializzazioneJSONPreparazionePayloadRispostaResult(
            input.TimestampSincronizzazione,
            payloadJson,
            SuccessHttpStatusCode,
            headers);
    }

    private static string NormalizeTimestamp(string timestampSincronizzazione, bool edgeCaseClockSkew)
    {
        if (string.IsNullOrWhiteSpace(timestampSincronizzazione))
        {
            throw new InvalidTimestampException(
                "DS05-BL: il timestamp_sincronizzazione è obbligatorio e deve essere in formato ISO 8601.");
        }

        if (!DateTimeOffset.TryParse(
                timestampSincronizzazione,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTimeOffset parsedTimestamp))
        {
            throw new InvalidTimestampException(
                $"DS05-BL: il timestamp_sincronizzazione '{timestampSincronizzazione}' non è un ISO 8601 valido.");
        }

        // In clock-skew scenarios DS01 already provides the authoritative server timestamp.
        // DS05 must propagate that server value without replacing it with a local clock reading.
        return edgeCaseClockSkew
            ? parsedTimestamp.ToString("O", CultureInfo.InvariantCulture)
            : parsedTimestamp.ToString("O", CultureInfo.InvariantCulture);
    }

    private static string BuildPayloadJson(IReadOnlyDictionary<string, object?> datiTabelle)
    {
        var payload = new JObject();
        var emittedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string nomeTabella in FixedPayloadOrder)
        {
            JToken token = datiTabelle.TryGetValue(nomeTabella, out object? value)
                ? ConvertToToken(nomeTabella, value)
                : new JObject();

            payload.Add(new JProperty(nomeTabella, token));
            emittedKeys.Add(nomeTabella);
        }

        foreach ((string nomeTabella, object? value) in datiTabelle
                     .Where(item => !emittedKeys.Contains(item.Key))
                     .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
        {
            payload.Add(new JProperty(nomeTabella, ConvertToToken(nomeTabella, value)));
        }

        try
        {
            return JsonConvert.SerializeObject(payload, Formatting.None);
        }
        catch (JsonSerializationException)
        {
            throw;
        }
        catch (JsonException ex)
        {
            throw new JsonSerializationException(
                "DS05-BL: errore durante la serializzazione del payload finale DatiAzienda.",
                ex);
        }
    }

    private static JToken ConvertToToken(string nomeTabella, object? value)
    {
        if (value is null)
        {
            return JValue.CreateNull();
        }

        if (value is JToken token)
        {
            return token.DeepClone();
        }

        try
        {
            return JToken.FromObject(value, Serializer);
        }
        catch (JsonSerializationException)
        {
            throw;
        }
        catch (JsonException ex)
        {
            throw new JsonSerializationException(
                $"DS05-BL: errore durante la serializzazione della tabella '{nomeTabella}'.",
                ex);
        }
    }

    private static string GenerateETag(string payloadJson)
    {
        try
        {
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
            byte[] hash = SHA256.HashData(payloadBytes);
            return $"\"{Convert.ToHexString(hash)}\"";
        }
        catch (Exception ex)
        {
            throw new ETagGenerationException(
                "DS05-BL: errore durante la generazione dell'ETag SHA256 del payload finale.",
                ex);
        }
    }
}
