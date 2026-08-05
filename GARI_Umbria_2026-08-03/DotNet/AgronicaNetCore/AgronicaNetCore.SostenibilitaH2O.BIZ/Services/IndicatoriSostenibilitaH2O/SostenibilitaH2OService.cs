using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Resources;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Chiavi;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Aziendale_Payload;
using AgronicaNetCore.SostenibilitaH2O.DAL.DataLayer.Lookup_Sost_H2O_Lotto;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OutData.FoodMetaverse;
using System.Data;
using System.Globalization;
using System.Text;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.IndicatoriSostenibilitaH2O
{
    /// <summary>
    /// Implements GET annual H2O sustainability indicator retrieval by filiera/azienda.
    /// See DS09-API Endpoint GET /sostenibilita_h2o/per_azienda_annuale.
    /// </summary>
    public class SostenibilitaH2OService : BaseServiceSostenibilitaH2OBiz, ISostenibilitaH2OService
    {
        private readonly ILookup_Sost_H2O_Aziendale_Chiavi _chiaviDal;
        private readonly ILookup_Sost_H2O_Aziendale_Payload _payloadDal;
        private readonly ILookup_Sost_H2O_Lotto _lottoDal;

        /// <summary>
        /// Initializes a new instance and resolves DAL dependencies.
        /// See DS09-API sezione "Dipendenze Business Logic" (DS08-BL, DS07-BL).
        /// </summary>
        public SostenibilitaH2OService(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _chiaviDal = provider.GetRequiredService<ILookup_Sost_H2O_Aziendale_Chiavi>();
            _payloadDal = provider.GetRequiredService<ILookup_Sost_H2O_Aziendale_Payload>();
            _lottoDal = provider.GetRequiredService<ILookup_Sost_H2O_Lotto>();
        }

        /// <inheritdoc/>
        public async Task<SostenibilitaH2OAziendaAnnualeResponse?> GetPerAziendaAnnualeAsync(
            string idFiliera,
            string? idAzienda,
            AgronicaCoreParametriServer objP)
        {
            if (string.IsNullOrWhiteSpace(idFiliera))
                throw new ArgumentException("Parametro 'id_filiera' obbligatorio.", nameof(idFiliera));

            var filtro = new GetSostH2OAziendale
            {
                Filiera = idFiliera,
                Azienda = idAzienda ?? string.Empty
            };

            DataTable chiaviTable = await _chiaviDal.ReadAsync(filtro, objP);
            if (chiaviTable.Rows.Count == 0)
                return null;

            var latestByCompanyYear = chiaviTable.AsEnumerable()
                .Select(row => new
                {
                    IdInvocazione = ReadString(row, "id_invocazione"),
                    IdAzienda = ReadString(row, "cuaa_azienda"),
                    Anno = ReadInt(row, "anno"),
                    DataCalcolo = ReadDateTime(row, "data_calcolo")
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.IdInvocazione)
                         && !string.IsNullOrWhiteSpace(x.IdAzienda)
                         && x.Anno > 0)
                .GroupBy(x => new { x.IdAzienda, x.Anno })
                .Select(g => g.OrderByDescending(x => x.DataCalcolo).First())
                .OrderBy(x => x.IdAzienda)
                .ThenByDescending(x => x.Anno)
                .ToList();

            if (latestByCompanyYear.Count == 0)
                return null;

            var response = new SostenibilitaH2OAziendaAnnualeResponse();
            string? versioneAlgoritmo = null;
            string? fonteMeteo = null;
            string? fonteBenchmark = null;

            var aziendeMap = new Dictionary<string, SostenibilitaH2OAziendaDto>(StringComparer.OrdinalIgnoreCase);

            foreach (var keyRow in latestByCompanyYear)
            {
                try
                {
                    DataTable payloadTable = await _payloadDal.ReadAsync(keyRow.IdInvocazione, objP);
                    if (payloadTable.Rows.Count == 0)
                        continue;

                    var payloadRow = payloadTable.Rows[0];
                    string payloadJson = ReadPayloadJson(payloadRow);
                    if (string.IsNullOrWhiteSpace(payloadJson))
                        continue;

                    var root = JToken.Parse(payloadJson);
                    var metadata = root["metadata"];

                    versioneAlgoritmo ??= metadata?["versione_algoritmo"]?.ToString();
                    fonteMeteo ??= metadata?["fonte_meteo"]?.ToString();
                    fonteBenchmark ??= metadata?["fonte_benchmark"]?.ToString();

                    var aziendaToken = root["aziende"]?
                        .FirstOrDefault(a => string.Equals(
                            a?["id_azienda"]?.ToString(),
                            keyRow.IdAzienda,
                            StringComparison.OrdinalIgnoreCase));

                    if (aziendaToken == null)
                        continue;

                    if (!aziendeMap.TryGetValue(keyRow.IdAzienda, out var aziendaDto))
                    {
                        aziendaDto = new SostenibilitaH2OAziendaDto
                        {
                            IdAzienda = keyRow.IdAzienda,
                            DesAzienda = aziendaToken["des_azienda"]?.ToString() ?? keyRow.IdAzienda
                        };
                        aziendeMap.Add(keyRow.IdAzienda, aziendaDto);
                    }

                    var annoToken = aziendaToken["anni"]?
                        .FirstOrDefault(a => ReadNullableInt(a?["anno"]) == keyRow.Anno);

                    if (annoToken == null)
                        continue;

                    aziendaDto.Anni.Add(new SostenibilitaH2OAnnoDto
                    {
                        Anno = keyRow.Anno,
                        SuperficieColtivataHa = Round2(ReadNullableDecimal(annoToken["superficie_coltivata_ha"])),
                        SostenibilitaH2O = new SostenibilitaH2OValoriDto
                        {
                            FabbisognoM3 = Round2(ReadNullableDecimal(annoToken["sostenibilita_h2o"]?["fabbisogno_m3"])),
                            ConsumiM3 = Round2(ReadNullableDecimal(annoToken["sostenibilita_h2o"]?["consumata_m3"])),
                            DaMeteoM3 = Round2(ReadNullableDecimal(annoToken["sostenibilita_h2o"]?["da_meteo_m3"])),
                            DeltaM3 = Round2(ReadNullableDecimal(annoToken["sostenibilita_h2o"]?["delta_m3"]))
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogWarning($"Errore di parsing payload H2O per id_invocazione={keyRow.IdInvocazione}. Riga omessa.", objP, ex);
                }
            }

            response.Aziende = aziendeMap.Values
                .Select(a =>
                {
                    a.Anni = a.Anni.OrderByDescending(x => x.Anno).ToList();
                    return a;
                })
                .Where(a => a.Anni.Count > 0)
                .OrderBy(a => a.IdAzienda)
                .ToList();

            if (response.Aziende.Count == 0)
                return null;

            response.Metadata = new SostenibilitaH2OMetadataDto
            {
                TimestampRisposta = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                VersioneAlgoritmo = versioneAlgoritmo ?? string.Empty,
                FonteMeteo = fonteMeteo ?? string.Empty,
                FonteBenchmark = fonteBenchmark ?? string.Empty,
                NumeroAziendeRestituiti = response.Aziende.Count,
                NumeroAnniRestituiti = response.Aziende.Sum(x => x.Anni.Count)
            };

            return response;
        }

        /// <inheritdoc/>
        public async Task<SostenibilitaH2OProdottoFilieraResponse?> GetPerProdottoDiFilieraAsync(
            string idFiliera,
            string codProdottoFmp,
            string? idAzienda,
            AgronicaCoreParametriServer objP)
        {
            if (string.IsNullOrWhiteSpace(idFiliera))
                throw new ArgumentException("Parametro 'id_filiera' obbligatorio.", nameof(idFiliera));

            if (string.IsNullOrWhiteSpace(codProdottoFmp))
                throw new ArgumentException("Parametro 'cod_prodotto' obbligatorio.", nameof(codProdottoFmp));

            var filtro = new GetSostH2OLotto
            {
                Filiera = idFiliera,
                Azienda = idAzienda ?? string.Empty,
                CodProdottoFmp = codProdottoFmp
            };

            DataTable lottoTable = await _lottoDal.ReadAsync(filtro, objP);
            if (lottoTable.Rows.Count == 0)
                return null;

            var rows = lottoTable.AsEnumerable()
                .Select(row => new
                {
                    IdAzienda = ReadString(row, "cuaa_azienda"),
                    Anno = ReadInt(row, "anno"),
                    Appezzamento = ReadString(row, "appezzamento"),
                    CulCod = ReadInt(row, "cul_cod"),
                    Esercizio = ReadString(row, "esercizio"),
                    DataCalcolo = ReadDateTime(row, "data_calcolo"),
                    Superficie = ReadNullableDecimal(row, "superficie_riferimento") ?? 0m,
                    FabbisognoM3PerHa = ReadNullableDecimal(row, "fabbisogno_m3_per_ha") ?? 0m,
                    ConsumiM3PerHa = ReadNullableDecimal(row, "consumata_m3_per_ha") ?? 0m,
                    DaMeteoM3PerHa = ReadNullableDecimal(row, "da_meteo_m3_per_ha") ?? 0m,
                    DeltaM3PerHa = ReadNullableDecimal(row, "delta_m3_per_ha") ?? 0m
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.IdAzienda) && x.Anno > 0)
                .ToList();

            if (rows.Count == 0)
                return null;

            // Keep only latest version per candidate key, then aggregate by azienda/anno.
            var latestByCandidateKey = rows
                .GroupBy(x => new { x.IdAzienda, x.Anno, x.Appezzamento, x.CulCod, x.Esercizio })
                .Select(g => g.OrderByDescending(x => x.DataCalcolo).First())
                .ToList();

            var aziende = latestByCandidateKey
                .GroupBy(x => x.IdAzienda, StringComparer.OrdinalIgnoreCase)
                .Select(groupAzienda =>
                {
                    var aziendaDto = new SostenibilitaH2OProdottoAziendaDto
                    {
                        IdAzienda = groupAzienda.Key,
                        DesAzienda = groupAzienda.Key
                    };

                    aziendaDto.Anni = groupAzienda
                        .GroupBy(x => x.Anno)
                        .Select(groupAnno =>
                        {
                            decimal superficieTotale = groupAnno.Sum(x => x.Superficie);

                            decimal fabbisogno;
                            decimal consumi;
                            decimal daMeteo;
                            decimal delta;

                            if (superficieTotale > 0m)
                            {
                                fabbisogno = groupAnno.Sum(x => x.FabbisognoM3PerHa * x.Superficie) / superficieTotale;
                                consumi = groupAnno.Sum(x => x.ConsumiM3PerHa * x.Superficie) / superficieTotale;
                                daMeteo = groupAnno.Sum(x => x.DaMeteoM3PerHa * x.Superficie) / superficieTotale;
                                delta = groupAnno.Sum(x => x.DeltaM3PerHa * x.Superficie) / superficieTotale;
                            }
                            else
                            {
                                // Best-effort fallback when area is missing: arithmetic mean across valid rows.
                                int count = groupAnno.Count();
                                fabbisogno = count == 0 ? 0m : groupAnno.Sum(x => x.FabbisognoM3PerHa) / count;
                                consumi = count == 0 ? 0m : groupAnno.Sum(x => x.ConsumiM3PerHa) / count;
                                daMeteo = count == 0 ? 0m : groupAnno.Sum(x => x.DaMeteoM3PerHa) / count;
                                delta = count == 0 ? 0m : groupAnno.Sum(x => x.DeltaM3PerHa) / count;
                            }

                            return new SostenibilitaH2OProdottoAnnoDto
                            {
                                Anno = groupAnno.Key,
                                SuperficieColtivataHa = Round2(superficieTotale),
                                SostenibilitaH2O = new SostenibilitaH2OProdottoValoriDto
                                {
                                    FabbisognoM3PerHa = Round2(fabbisogno),
                                    ConsumiM3PerHa = Round2(consumi),
                                    DaMeteoM3PerHa = Round2(daMeteo),
                                    DeltaM3PerHa = Round2(delta)
                                }
                            };
                        })
                        .OrderByDescending(x => x.Anno)
                        .ToList();

                    return aziendaDto;
                })
                .Where(x => x.Anni.Count > 0)
                .OrderBy(x => x.IdAzienda)
                .ToList();

            if (aziende.Count == 0)
                return null;

            return new SostenibilitaH2OProdottoFilieraResponse
            {
                Aziende = aziende,
                Metadata = new SostenibilitaH2OProdottoMetadataDto
                {
                    TimestampRisposta = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                    CodProdottoFmp = codProdottoFmp,
                    NumeroAziendeProduttrici = aziende.Count,
                    NumeroAnniTotali = aziende.Sum(x => x.Anni.Count)
                }
            };
        }

        /// <inheritdoc/>
        public async Task<SostenibilitaH2OLottiRaccoltiResponse> GetSintesiPerLottiRaccoltiAsync(
            SostenibilitaH2OLottiRaccoltiRequest request,
            AgronicaCoreParametriServer objP)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.IdFiliera))
                throw new ArgumentException("Parametro 'id_filiera' obbligatorio.", nameof(request));

            if (string.IsNullOrWhiteSpace(request.CodProdottoFmp))
                throw new ArgumentException("Parametro 'cod_prodotto' obbligatorio.", nameof(request));

            if (request.Lotti == null || request.Lotti.Count == 0)
                throw new ArgumentException("Parametro 'lotti' obbligatorio.", nameof(request));

            var requestedLots = request.Lotti
                .Where(x => x != null
                         && !string.IsNullOrWhiteSpace(x.IdAzienda)
                         && !string.IsNullOrWhiteSpace(x.CodLottoFmp))
                .Select(x => new
                {
                    IdAzienda = x.IdAzienda.Trim(),
                    CodLottoFmp = x.CodLottoFmp.Trim(),
                    Key = $"{NormalizeKey(x.IdAzienda)}|{NormalizeKey(x.CodLottoFmp)}"
                })
                .DistinctBy(x => x.Key)
                .ToList();

            if (requestedLots.Count == 0)
                throw new ArgumentException("Nessun lotto valido presente nella richiesta.", nameof(request));

            var filtro = new GetSostH2OLottiBatch
            {
                Filiera = request.IdFiliera,
                CodProdottoFmp = request.CodProdottoFmp,
                Lotti = requestedLots
                    .Select(x => new GetSostH2OLottiBatchItem
                    {
                        Azienda = x.IdAzienda,
                        CodLottoFmp = x.CodLottoFmp
                    })
                    .ToList()
            };

            DataTable lottoTable = await _lottoDal.ReadPerLottiBatchAsync(filtro, objP);

            var rows = lottoTable.AsEnumerable()
                .Select(row => new
                {
                    IdAzienda = ReadString(row, "cuaa_azienda"),
                    CodLottoFmp = ReadString(row, "lotto_raccolta"),
                    Anno = ReadInt(row, "anno"),
                    Appezzamento = ReadString(row, "appezzamento"),
                    CulCod = ReadInt(row, "cul_cod"),
                    Esercizio = ReadString(row, "esercizio"),
                    DataCalcolo = ReadDateTime(row, "data_calcolo"),
                    Superficie = ReadNullableDecimal(row, "superficie_riferimento") ?? 0m,
                    FabbisognoM3PerHa = ReadNullableDecimal(row, "fabbisogno_m3_per_ha") ?? 0m,
                    ConsumiM3PerHa = ReadNullableDecimal(row, "consumata_m3_per_ha") ?? 0m,
                    DaMeteoM3PerHa = ReadNullableDecimal(row, "da_meteo_m3_per_ha") ?? 0m,
                    DeltaM3PerHa = ReadNullableDecimal(row, "delta_m3_per_ha") ?? 0m
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.IdAzienda)
                         && !string.IsNullOrWhiteSpace(x.CodLottoFmp)
                         && !string.IsNullOrWhiteSpace(x.Esercizio))
                .ToList();

            var latestByCandidateKey = rows
                .GroupBy(x => new
                {
                    IdAzienda = NormalizeKey(x.IdAzienda),
                    CodLottoFmp = NormalizeKey(x.CodLottoFmp),
                    x.Anno,
                    Appezzamento = NormalizeKey(x.Appezzamento),
                    CulCod = x.CulCod,
                    Esercizio = NormalizeKey(x.Esercizio)
                })
                .Select(g => g.OrderByDescending(x => x.DataCalcolo).First())
                .ToList();

            var aziendePresenti = latestByCandidateKey
                .Select(x => NormalizeKey(x.IdAzienda))
                .Distinct(StringComparer.Ordinal)
                .ToHashSet(StringComparer.Ordinal);

            var response = new SostenibilitaH2OLottiRaccoltiResponse();

            foreach (var requestedLot in requestedLots)
            {
                string aziendaKey = NormalizeKey(requestedLot.IdAzienda);
                string lottoKey = NormalizeKey(requestedLot.CodLottoFmp);

                var lotRows = latestByCandidateKey
                    .Where(x => string.Equals(NormalizeKey(x.IdAzienda), aziendaKey, StringComparison.Ordinal)
                             && string.Equals(NormalizeKey(x.CodLottoFmp), lottoKey, StringComparison.Ordinal))
                    .ToList();

                if (lotRows.Count == 0)
                {
                    response.LottiNonTrovati.Add(new SostenibilitaH2OLottoNonTrovatoDto
                    {
                        IdAzienda = requestedLot.IdAzienda,
                        CodLottoFmp = requestedLot.CodLottoFmp,
                        Motivo = aziendePresenti.Contains(aziendaKey) ? "LOTTO_NOT_FOUND" : "AZIENDA_NOT_FOUND"
                    });
                    continue;
                }

                decimal superficieTotale = lotRows.Sum(x => x.Superficie);

                decimal fabbisognoM3 = superficieTotale > 0m
                    ? lotRows.Sum(x => x.FabbisognoM3PerHa * x.Superficie)
                    : lotRows.Sum(x => x.FabbisognoM3PerHa);

                decimal consumiM3 = superficieTotale > 0m
                    ? lotRows.Sum(x => x.ConsumiM3PerHa * x.Superficie)
                    : lotRows.Sum(x => x.ConsumiM3PerHa);

                decimal daMeteoM3 = superficieTotale > 0m
                    ? lotRows.Sum(x => x.DaMeteoM3PerHa * x.Superficie)
                    : lotRows.Sum(x => x.DaMeteoM3PerHa);

                decimal deltaM3 = superficieTotale > 0m
                    ? lotRows.Sum(x => x.DeltaM3PerHa * x.Superficie)
                    : lotRows.Sum(x => x.DeltaM3PerHa);

                var dettagliEsercizi = lotRows
                    .GroupBy(x => new
                    {
                        Esercizio = NormalizeKey(x.Esercizio),
                        Azienda = NormalizeKey(x.IdAzienda)
                    })
                    .Select(groupEsercizio =>
                    {
                        decimal superficieEsercizio = groupEsercizio.Sum(x => x.Superficie);
                        bool hasSurface = superficieEsercizio > 0m;

                        decimal fabbisognoPerHa = hasSurface
                            ? groupEsercizio.Sum(x => x.FabbisognoM3PerHa * x.Superficie) / superficieEsercizio
                            : groupEsercizio.Average(x => x.FabbisognoM3PerHa);

                        decimal consumiPerHa = hasSurface
                            ? groupEsercizio.Sum(x => x.ConsumiM3PerHa * x.Superficie) / superficieEsercizio
                            : groupEsercizio.Average(x => x.ConsumiM3PerHa);

                        decimal daMeteoPerHa = hasSurface
                            ? groupEsercizio.Sum(x => x.DaMeteoM3PerHa * x.Superficie) / superficieEsercizio
                            : groupEsercizio.Average(x => x.DaMeteoM3PerHa);

                        decimal deltaPerHa = hasSurface
                            ? groupEsercizio.Sum(x => x.DeltaM3PerHa * x.Superficie) / superficieEsercizio
                            : groupEsercizio.Average(x => x.DeltaM3PerHa);

                        var first = groupEsercizio.First();
                        return new SostenibilitaH2ODettaglioEsercizioDto
                        {
                            EsercizioId = first.Esercizio,
                            Azienda = first.IdAzienda,
                            FabbisognoM3PerHa = Round2(fabbisognoPerHa),
                            ConsumiM3PerHa = Round2(consumiPerHa),
                            DaMeteoM3PerHa = Round2(daMeteoPerHa),
                            DeltaM3PerHa = Round2(deltaPerHa)
                        };
                    })
                    .OrderBy(x => x.EsercizioId)
                    .ToList();

                response.LottiElaborati.Add(new SostenibilitaH2OLottoElaboratoDto
                {
                    IdAzienda = requestedLot.IdAzienda,
                    CodLottoFmp = requestedLot.CodLottoFmp,
                    SostenibilitaH2O = new SostenibilitaH2OLottoValoriDto
                    {
                        FabbisognoM3 = Round2(fabbisognoM3),
                        ConsumiM3 = Round2(consumiM3),
                        DaMeteoM3 = Round2(daMeteoM3),
                        DeltaM3 = Round2(deltaM3)
                    },
                    SuperficieColtivataHa = Round2(superficieTotale),
                    NumeroEserciziContribuenti = dettagliEsercizi.Count,
                    DettaglioEsercizi = dettagliEsercizi
                });
            }

            response.Metadata = new SostenibilitaH2OLottiRaccoltiMetadataDto
            {
                TimestampRisposta = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                NumeroLottiRichiesti = requestedLots.Count,
                NumeroLottiTrovati = response.LottiElaborati.Count,
                NumeroLottiNonTrovati = response.LottiNonTrovati.Count
            };

            return response;
        }

        private static string ReadPayloadJson(DataRow row)
        {
            object? payloadObj = row.Table.Columns.Contains("payload_json") ? row["payload_json"] : null;
            if (payloadObj == null || payloadObj == DBNull.Value)
                return string.Empty;

            if (payloadObj is byte[] payloadBytes)
            {
                string charset = row.Table.Columns.Contains("charset")
                    ? row["charset"]?.ToString() ?? string.Empty
                    : string.Empty;

                Encoding encoding = ResolveEncoding(charset);
                return encoding.GetString(payloadBytes);
            }

            return payloadObj.ToString() ?? string.Empty;
        }

        private static Encoding ResolveEncoding(string charset)
        {
            if (string.IsNullOrWhiteSpace(charset))
                return Encoding.UTF8;

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

        private static string ReadString(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return string.Empty;

            object value = row[columnName];
            return value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
        }

        private static int ReadInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return 0;

            object value = row[columnName];
            if (value == DBNull.Value)
                return 0;

            if (value is int intValue)
                return intValue;

            return int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
                ? parsed
                : 0;
        }

        private static DateTime ReadDateTime(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return DateTime.MinValue;

            object value = row[columnName];
            if (value == DBNull.Value)
                return DateTime.MinValue;

            if (value is DateTime dateTimeValue)
                return dateTimeValue;

            return DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static decimal? ReadNullableDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            object value = row[columnName];
            if (value == DBNull.Value)
                return null;

            if (value is decimal decimalValue)
                return decimalValue;

            if (value is double doubleValue)
                return Convert.ToDecimal(doubleValue, CultureInfo.InvariantCulture);

            if (value is float floatValue)
                return Convert.ToDecimal(floatValue, CultureInfo.InvariantCulture);

            return decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsed)
                ? parsed
                : null;
        }

        private static int? ReadNullableInt(JToken? token)
        {
            if (token == null)
                return null;

            if (token.Type == JTokenType.Integer)
                return token.Value<int>();

            return int.TryParse(token.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : null;
        }

        private static decimal? ReadNullableDecimal(JToken? token)
        {
            if (token == null)
                return null;

            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
                return token.Value<decimal>();

            return decimal.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value)
                ? value
                : null;
        }

        private static decimal Round2(decimal? value)
        {
            return Math.Round(value ?? 0m, 2, MidpointRounding.AwayFromZero);
        }

        private static string NormalizeKey(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToUpperInvariant();
        }
    }
}
