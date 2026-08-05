using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.AggregazionePayloadPerAzienda
{
    /// <summary>
    /// Aggrega i risultati di sostenibilità idrica per tutti gli esercizi del perimetro aziendale-anno
    /// e genera il payload JSON UTF-8 strutturato conforme allo schema FS001 <c>/per_azienda_annuale</c>.
    /// <para>
    /// Fase 1 — Aggregazione: somma le superfici e calcola i totali m³ pesando i valori per-ha
    /// per la rispettiva superficie (<c>SUM(valore_m3_per_ha ?? 0 × superficie_ha)</c>).
    /// Gli indicatori null (INDETERMINATO da DS07-BL) sono trattati come zero.
    /// </para>
    /// <para>
    /// Fase 2 — Payload: costruisce la struttura JSON e la serializza in UTF-8.
    /// </para>
    /// Riferimento spec: DS09-BL AggregazionePayloadPerAzienda — Regole di Business, Serializzazione.
    /// </summary>
    public class AggregazionePayloadPerAziendaService : BaseServiceSostenibilitaH2OBiz, IAggregazionePayloadPerAziendaService
    {
        private readonly ILogger<AggregazionePayloadPerAziendaService> _logger;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        /// <inheritdoc cref="BaseServiceSostenibilitaH2OBiz"/>
        public AggregazionePayloadPerAziendaService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _logger = provider.GetRequiredService<ILogger<AggregazionePayloadPerAziendaService>>();
        }

        /// <inheritdoc/>
        public string Aggrega(AggregazionePayloadPerAziendaInput input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));
            if (string.IsNullOrWhiteSpace(input.Filiera))
                throw new ArgumentException("Il campo Filiera è obbligatorio.", nameof(input));
            if (string.IsNullOrWhiteSpace(input.Azienda))
                throw new ArgumentException("Il campo Azienda è obbligatorio.", nameof(input));
            if (input.IndicatoriEsercizi is null || input.IndicatoriEsercizi.Count == 0)
                throw new InvalidAggregationException(
                    $"La lista IndicatoriEsercizi non può essere vuota per azienda '{input.Azienda}' anno {input.Anno}.");

            // Fase 1: Aggregazione — superficie e componenti H2O ponderate per superficie
            var superficieTotaleHa = input.IndicatoriEsercizi.Sum(e => e.SuperficieHa);
            if (superficieTotaleHa <= 0m)
                throw new InvalidAggregationException(
                    $"La superficie coltivata totale deve essere > 0. " +
                    $"Valore calcolato: {superficieTotaleHa} ha per azienda '{input.Azienda}' anno {input.Anno}.");

            var fabbisognoM3 = input.IndicatoriEsercizi.Sum(e => (e.FabbisognoM3PerHa ?? 0m) * e.SuperficieHa);
            var consumataM3  = input.IndicatoriEsercizi.Sum(e => (e.ConsumataM3PerHa  ?? 0m) * e.SuperficieHa);
            var daMeteoM3    = input.IndicatoriEsercizi.Sum(e => (e.DaMeteoM3PerHa    ?? 0m) * e.SuperficieHa);
            var deltaM3      = input.IndicatoriEsercizi.Sum(e => (e.DeltaM3PerHa      ?? 0m) * e.SuperficieHa);

            _logger.LogInformation(
                "DS09-BL Aggregazione completata: azienda={Azienda} anno={Anno} " +
                "superficie={SuperficieHa} ha fabbisogno={FabbisognoM3} m³ consumata={ConsumataM3} m³ " +
                "da_meteo={DaMeteoM3} m³ delta={DeltaM3} m³",
                input.Azienda, input.Anno,
                Math.Round(superficieTotaleHa, 2),
                Math.Round(fabbisognoM3, 2),
                Math.Round(consumataM3, 2),
                Math.Round(daMeteoM3, 2),
                Math.Round(deltaM3, 2));
            // Fase 2: Costruzione payload JSON conforme a FS001 /per_azienda_annuale
            var payloadObj = new PayloadH2OAzienda
            {
                Aziende = new List<AziendaPayload>
                {
                    new AziendaPayload
                    {
                        IdAzienda = input.Azienda,
                        Anni = new List<AnnoPayload>
                        {
                            new AnnoPayload
                            {
                                Anno = input.Anno,
                                SuperficieColtivataHa = Math.Round(superficieTotaleHa, 2),
                                SostenibilitaH2O = new SostenibilitaH2OPayload
                                {
                                    FabbisognoM3 = Math.Round(fabbisognoM3, 2),
                                    ConsumataM3  = Math.Round(consumataM3, 2),
                                    DaMeteoM3    = Math.Round(daMeteoM3, 2),
                                    DeltaM3      = Math.Round(deltaM3, 2)
                                }
                            }
                        }
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string payloadJSON;

            try
            {
                payloadJSON = JsonSerializer.Serialize(payloadObj, options);
            }
            catch (Exception ex)
            {
                throw new PayloadGenerationException(
                    $"Errore durante la serializzazione del payload JSON per azienda '{input.Azienda}' anno {input.Anno}.",
                    ex);
            }

            return payloadJSON;
        }

        // ── Payload DTOs privati (struttura JSON FS001 /per_azienda_annuale) ──────────────────────

        private sealed class PayloadH2OAzienda
        {
            [JsonPropertyName("aziende")]
            public List<AziendaPayload> Aziende { get; set; } = new();
        }

        private sealed class AziendaPayload
        {
            [JsonPropertyName("id_azienda")]
            public string IdAzienda { get; set; } = string.Empty;

            [JsonPropertyName("anni")]
            public List<AnnoPayload> Anni { get; set; } = new();
        }

        private sealed class AnnoPayload
        {
            [JsonPropertyName("anno")]
            public int Anno { get; set; }

            [JsonPropertyName("superficie_coltivata_ha")]
            public decimal SuperficieColtivataHa { get; set; }

            [JsonPropertyName("sostenibilita_h2o")]
            public SostenibilitaH2OPayload SostenibilitaH2O { get; set; } = new();
        }

        private sealed class SostenibilitaH2OPayload
        {
            [JsonPropertyName("fabbisogno_m3")]
            public decimal FabbisognoM3 { get; set; }

            [JsonPropertyName("consumata_m3")]
            public decimal ConsumataM3 { get; set; }

            [JsonPropertyName("da_meteo_m3")]
            public decimal DaMeteoM3 { get; set; }

            [JsonPropertyName("delta_m3")]
            public decimal DeltaM3 { get; set; }
        }
    }
}
