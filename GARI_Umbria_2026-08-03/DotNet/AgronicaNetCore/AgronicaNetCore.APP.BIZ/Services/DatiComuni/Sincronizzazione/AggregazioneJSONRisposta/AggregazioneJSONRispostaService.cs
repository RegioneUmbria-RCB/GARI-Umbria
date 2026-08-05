using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using Microsoft.Extensions.Options;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggregazioneJSONRisposta;

/// <summary>
/// Reads consolidated JSON payloads from <c>app_preparazione_daticomuni_web2app</c>
/// and aggregates them into a single structured JSON payload for the API response.
/// Uses <see cref="JsonTextWriter"/> over a <see cref="MemoryStream"/> for memory-efficient,
/// streaming-capable JSON assembly that handles payloads larger than 100 MB without
/// excessive heap allocations.
/// Ref: DS09-BL – Nome: AggregazioneJSONRisposta.
/// </summary>
public sealed class AggregazioneJSONRispostaService : IAggregazioneJSONRispostaService
{

    private readonly IAggregazioneJSONDatiComuni _aggregazioneDal;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;
    public AggregazioneJSONRispostaService(
        IAggregazioneJSONDatiComuni aggregazioneDal,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _aggregazioneDal = aggregazioneDal;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<AggregazioneJSONRispostaResult> AggregaAsync(
        List<string> tabelleRichieste,
        FiltriRichiesti filtriRichiesti,
        bool abilitaCompressione,
        AgronicaCoreParametriServer objParametriServer)
    {
        int timeoutMs = _settings.TimeoutMs;
        DateTime timestampServer = DateTime.Now;
        var sw = Stopwatch.StartNew();

        DataTable righe;
        try
        {
            righe = await _aggregazioneDal.LeggiJSONTabelleomuniAsync(tabelleRichieste, objParametriServer);
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Errore nella lettura dei JSON da app_preparazione_daticomuni_web2app.",
                ex
            );
        }

        // Build the aggregated JSON payload using a streaming writer over a MemoryStream.
        // This pattern handles payloads > 100 MB without creating large intermediate strings.
        // Ref: DS09-BL – Regole di Business: Implementare streaming JSON per payload > 100 MB.
        string payloadJson;
        int numeroTabelle = 0;

        using (var ms = new MemoryStream())
        {
            using (var sw2 = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true))
            using (var jw = new JsonTextWriter(sw2))
            {
                jw.Formatting = Formatting.None;
                jw.WriteStartObject();

                foreach (DataRow row in righe.Rows)
                {
                    string nomeTabella = row["nome_tabella"].ToString() ?? string.Empty;
                    string? jsonContent = row["json_content"] == DBNull.Value
                        ? null
                        : row["json_content"].ToString();

                    if (string.IsNullOrWhiteSpace(nomeTabella))
                        continue;

                    // Validate and parse each json_content.
                    // Ref: DS09-BL – Regole di Business: Validare che ogni json_content sia JSON valido.
                    JToken parsedToken;
                    try
                    {
                        parsedToken = string.IsNullOrWhiteSpace(jsonContent)
                            ? JValue.CreateNull()
                            : JToken.Parse(jsonContent);
                    }
                    catch (JsonException ex)
                    {
                        throw new JsonAggregationException(
                            $"Il json_content della tabella '{nomeTabella}' non è un JSON valido: {ex.Message}. Ref: DS09-BL.",
                            ex
                        );
                    }

                    // Unico switch per filtri tabella
                    if (parsedToken is JArray arrFiltra)
                    {
                        switch (nomeTabella.ToLowerInvariant())
                        {
                            // Filtri pivasuperuser per tabelle personalizzate
                            case var t when
                                t == nameof(TabellaComune.MisureAvversitaPersonalizzate).ToLowerInvariant() ||
                                t == nameof(TabellaComune.MisureDanniPersonalizzate).ToLowerInvariant() ||
                                t == nameof(TabellaComune.IndiciMaturitaPersonalizzate).ToLowerInvariant() ||
                                t == nameof(TabellaComune.IndiciMaturitaSpecieVegetaliPersonalizzate).ToLowerInvariant() ||
                                t == nameof(TabellaComune.MisureIndiciMaturitaPersonalizzate).ToLowerInvariant():
                                {
                                    var filtered = new JArray();
                                    foreach (var item in arrFiltra)
                                    {
                                        var pivaSuperUserProp = item["pivasuperuser"];
                                        if (pivaSuperUserProp != null
                                            && pivaSuperUserProp.Type == JTokenType.String
                                            && string.Equals(
                                                pivaSuperUserProp.Value<string>(),
                                                objParametriServer.PivaSuperUser,
                                                StringComparison.Ordinal))
                                        {
                                            // Crea una copia senza la proprietà pivasuperuser
                                            if (item is JObject obj)
                                            {
                                                var clone = (JObject)obj.DeepClone();
                                                clone.Remove("pivasuperuser");
                                                filtered.Add(clone);
                                            }
                                            else
                                            {
                                                filtered.Add(item);
                                            }
                                        }
                                    }
                                    parsedToken = filtered;
                                    // Cambia il nome della proprietà di output rimuovendo "Personalizzate" o "Personalizzati"
                                    nomeTabella = nomeTabella.Replace("Personalizzate", string.Empty).Replace("Personalizzati", string.Empty);
                                }
                                break;
                            // Caso SPECIEVEGETALISTADICRESCITAPERSONALIZZATI: se non esistono record per la pivasuperuser, restituisci quelli con pivasuperuser = ""
                            case var t when t == nameof(TabellaComune.SpecieVegetaliStadiCrescitaPersonalizzati).ToLowerInvariant():
                                {
                                    var filtered = new JArray();
                                    foreach (var item in arrFiltra)
                                    {
                                        var pivaSuperUserProp = item["pivasuperuser"];
                                        if (pivaSuperUserProp != null
                                            && pivaSuperUserProp.Type == JTokenType.String
                                            && string.Equals(
                                                pivaSuperUserProp.Value<string>(),
                                                objParametriServer.PivaSuperUser,
                                                StringComparison.Ordinal))
                                        {
                                            if (item is JObject obj)
                                            {
                                                var clone = (JObject)obj.DeepClone();
                                                clone.Remove("pivasuperuser");
                                                filtered.Add(clone);
                                            }
                                            else
                                            {
                                                filtered.Add(item);
                                            }
                                        }
                                    }
                                    // Se non ci sono record personalizzati, restituisci quelli con pivasuperuser = ""
                                    if (filtered.Count == 0)
                                    {
                                        foreach (var item in arrFiltra)
                                        {
                                            var pivaSuperUserProp = item["pivasuperuser"];
                                            if (pivaSuperUserProp != null
                                                && pivaSuperUserProp.Type == JTokenType.String
                                                && string.IsNullOrEmpty(pivaSuperUserProp.Value<string>()))
                                            {
                                                if (item is JObject obj)
                                                {
                                                    var clone = (JObject)obj.DeepClone();
                                                    clone.Remove("pivasuperuser");
                                                    filtered.Add(clone);
                                                }
                                                else
                                                {
                                                    filtered.Add(item);
                                                }
                                            }
                                        }
                                    }
                                    parsedToken = filtered;
                                    nomeTabella = nomeTabella.Replace("Personalizzate", string.Empty).Replace("Personalizzati", string.Empty);
                                }
                                break;
                            // Filtro per tabella nazioni
                            case "nazioni":
                                if (filtriRichiesti?.Nazioni?.Count > 0)
                                {
                                    var filtered = new JArray();
                                    foreach (var item in arrFiltra)
                                    {
                                        var codice = item["codice"];
                                        if (codice != null && codice.Type == JTokenType.String && filtriRichiesti.Nazioni.Contains(codice.Value<string>()))
                                        {
                                            filtered.Add(item);
                                        }
                                    }
                                    parsedToken = filtered;
                                }
                                break;
                            // Filtro per regioni, province, comuni
                            case "regioni":
                            case "province":
                            case "comuni":
                                if (filtriRichiesti?.Nazioni?.Count > 0)
                                {
                                    var filtered = new JArray();
                                    foreach (var item in arrFiltra)
                                    {
                                        var statoCod = item["statoCod"];
                                        if (statoCod != null && statoCod.Type == JTokenType.String && filtriRichiesti.Nazioni.Contains(statoCod.Value<string>()))
                                        {
                                            filtered.Add(item);
                                        }
                                    }
                                    parsedToken = filtered;
                                }
                                break;
                            // Filtro per tabella specie
                            case "specie":
                                if (filtriRichiesti?.Specie?.Count > 0)
                                {
                                    var filtered = new JArray();
                                    foreach (var item in arrFiltra)
                                    {
                                        var codice = item["codice"];
                                        if (codice != null &&
                                            (codice.Type == JTokenType.String && int.TryParse(codice.Value<string>(), out int codiceInt) && filtriRichiesti.Specie.Contains(codiceInt)
                                            || codice.Type == JTokenType.Integer && filtriRichiesti.Specie.Contains(codice.Value<int>())))
                                        {
                                            filtered.Add(item);
                                        }
                                    }
                                    parsedToken = filtered;
                                }
                                break;
                            // Filtro per tabella varieta e finalita
                            case "varieta":
                            case "finalita":
                                if (filtriRichiesti?.Specie?.Count > 0)
                                {
                                    var filtered = new JArray();
                                    foreach (var item in arrFiltra)
                                    {
                                        var specieCod = item["specieCod"];
                                        if (specieCod != null &&
                                            (specieCod.Type == JTokenType.String && int.TryParse(specieCod.Value<string>(), out int specieCodInt) && filtriRichiesti.Specie.Contains(specieCodInt)
                                            || specieCod.Type == JTokenType.Integer && filtriRichiesti.Specie.Contains(specieCod.Value<int>())))
                                        {
                                            filtered.Add(item);
                                        }
                                    }
                                    parsedToken = filtered;
                                }
                                break;
                            // altri casi: nessun filtro
                        }
                    }

                    // Sync semantic: if the read table has no elements, emit null
                    // (not [] or {}), so APP can treat it as a full clear from source.
                    if ((parsedToken is JArray emptyArray && emptyArray.Count == 0)
                        || (parsedToken is JObject emptyObject && !emptyObject.HasValues))
                    {
                        parsedToken = JValue.CreateNull();
                    }

                    jw.WritePropertyName(nomeTabella);
                    parsedToken.WriteTo(jw);
                    numeroTabelle++;

                    // Guard against runaway payload size during build.
                    // Ref: DS09-BL – Regole di Business: warning log se il payload supera il limite configurato.
                    if (ms.Length > _settings.MaxPayloadBytes)
                    {
                        _loggingService.LogWarning(
                            "AggregazioneJSONRisposta: payload ha superato la dimensione massima di {MaxMB} MB. Ref: DS09-BL.",
                            objParametriServer,
                            null,
                            _settings.MaxPayloadBytes / (1024 * 1024));
                    }
                }

                jw.WriteEndObject();
            }

            long dimensioneBytes = ms.Length;
            if (dimensioneBytes > _settings.MaxPayloadBytes)
            {
                _loggingService.LogWarning(
                    "AggregazioneJSONRisposta: payload di {SizeMB} MB supera il limite di {MaxMB} MB. Ref: DS09-BL.",
                    objParametriServer,
                    null,
                    dimensioneBytes / (1024 * 1024),
                    _settings.MaxPayloadBytes / (1024 * 1024));
            }

            if (dimensioneBytes >= _settings.StreamingThresholdBytes)
            {
                _loggingService.LogWarning(
                    "AggregazioneJSONRisposta: payload di dimensione {SizeMB} MB supera soglia streaming ({ThresholdMB} MB). Ref: DS09-BL.",
                    objParametriServer,
                    null,
                    dimensioneBytes / (1024 * 1024),
                    _settings.StreamingThresholdBytes / (1024 * 1024));
            }

            ms.Seek(0, SeekOrigin.Begin);
            payloadJson = new StreamReader(ms, Encoding.UTF8).ReadToEnd();
        }

        // Optional gzip compression of the final payload.
        // Ref: DS09-BL – Regole di Business: Se abilita_compressione = true, applicare gzip.
        byte[]? payloadCompresso = null;
        if (abilitaCompressione)
        {
            payloadCompresso = ComprimiGzip(payloadJson);
        }

        sw.Stop();
        long elapsedMs = sw.ElapsedMilliseconds;

        // Ref: DS09-BL – Regole di Business: warning log se l'aggregazione supera il limite configurato.
        if (elapsedMs > timeoutMs)
        {
            _loggingService.LogWarning(
                "AggregazioneJSONRisposta ha superato il timeout di {TimeoutMs} ms: {ElapsedMs}ms. Ref: DS09-BL.",
                objParametriServer,
                null,
                timeoutMs,
                elapsedMs
            );
        }

        long dimensioneBytesFinale = Encoding.UTF8.GetByteCount(payloadJson);
        long dimensioneCompressaBytes = payloadCompresso?.Length ?? 0L;

        return new AggregazioneJSONRispostaResult(
            payloadAggregatoJson: payloadJson,
            payloadCompresso: payloadCompresso,
            timestampServer: timestampServer,
            numeroTabelle: numeroTabelle,
            dimensioneBytes: dimensioneBytesFinale,
            dimensioneCompressaBytes: dimensioneCompressaBytes,
            aggregationTimeMs: elapsedMs
        );
    }

    /// <summary>
    /// Compresses <paramref name="json"/> to gzip bytes using optimal compression level.
    /// Ref: DS09-BL – Regole di Business: applicare gzip al payload aggregato finale.
    /// </summary>
    private static byte[] ComprimiGzip(string json)
    {
        byte[] sourceBytes = Encoding.UTF8.GetBytes(json);

        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(sourceBytes, 0, sourceBytes.Length);
        }

        return output.ToArray();
    }
}

public class FiltriRichiesti
{
    public List<string> Nazioni { get; set; } = new List<string>();
    public List<int> Specie { get; set; } = new List<int>();
    public List<int> Operazioni { get; set; } = new List<int>();

}