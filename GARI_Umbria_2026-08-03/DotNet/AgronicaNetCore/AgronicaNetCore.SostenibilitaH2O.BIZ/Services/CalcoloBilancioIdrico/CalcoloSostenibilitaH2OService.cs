using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Models;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Resources;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.AggregazionePayloadPerAzienda;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.LookupBenchmarkWaterFootprint;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.PersistenzaChiaviPayloadPerAzienda;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.PersistenzaRisultatiPerColture;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoConsumoIdricoEffettivo;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoPrecipitazioniM6;
using AgronicaNetCore.SostenibilitaH2O.BIZ.Services.RecuperoResaProduttiva;
using AgronicaNetCore.Utility.BIZ.Services.Firma;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Services.CalcoloBilancioIdrico
{
    /// <summary>
    /// <summary>
    /// Orchestratore del calcolo del bilancio idrico H2O.
    /// Coordina il pipeline completo DS03→DS10.
    /// Riferimento spec: DS-02.2-BL Chiamata WebApi Calcolo Bilancio Idrico.
    /// </summary>
    public class CalcoloSostenibilitaH2OService
        : BaseServiceSostenibilitaH2OBiz, ICalcoloSostenibilitaH2OService
    {
        private readonly IRecuperoConsumoIdricoEffettivoService _consumoService;
        private readonly IRecuperoResaProduttivaService _resaService;
        private readonly IRecuperoPrecipitazioniService _precipitazioniService;
        private readonly ILookupBenchmarkWaterFootprintService _benchmarkService;
        private readonly IPersistenzaRisultatiPerColtureService _coltureService;
        private readonly IAggregazionePayloadPerAziendaService _aggregazioneService;
        private readonly IPersistenzaChiaviPayloadPerAziendaService _persistenzaAziendaService;
        private readonly IFirmaDigitaleService _firmaDigitaleService;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        private const decimal _fattoreEfficienzaPioggia = 0.7m;
        private const decimal _mmToM3PerHa = 10m; // 1 mm su 1 ha = 10 m³

        public CalcoloSostenibilitaH2OService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            IRecuperoConsumoIdricoEffettivoService consumoService,
            IRecuperoResaProduttivaService resaService,
            IRecuperoPrecipitazioniService precipitazioniService,
            ILookupBenchmarkWaterFootprintService benchmarkService,
            IPersistenzaRisultatiPerColtureService coltureService,
            IAggregazionePayloadPerAziendaService aggregazioneService,
            IPersistenzaChiaviPayloadPerAziendaService persistenzaAziendaService)
            : base(provider, localizer)
        {
            _consumoService = consumoService;
            _resaService = resaService;
            _precipitazioniService = precipitazioniService;
            _benchmarkService = benchmarkService;
            _coltureService = coltureService;
            _aggregazioneService = aggregazioneService;
            _persistenzaAziendaService = persistenzaAziendaService;
            _firmaDigitaleService = provider.GetRequiredService<IFirmaDigitaleService>();
        }

        /// <inheritdoc/>
        public async Task<string> CalcoloSostenibilitaH2OAsync(
            CalcoloBilancioIdricoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Perimetro);
            ArgumentNullException.ThrowIfNull(objParametriServer);
            ArgumentNullException.ThrowIfNull(objParametriSuperServer);

            return await EseguiOrchestratoreAsync(
                request, objParametriServer, objParametriSuperServer);
        }

        // ── Full pipeline (DS03→DS10) ─────────────────────────────────────────────

        private async Task<string> EseguiOrchestratoreAsync(
            CalcoloBilancioIdricoRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var perimetro = request.Perimetro;
            var esercizi = request.Esercizi;
            var idInvocazione = Guid.NewGuid();
            var dataCalcolo = DateTime.Now;

            // DS03 + DS04 + DS05 in parallelo
            var consumoTask = _consumoService.GetVolumiAsync(esercizi, perimetro.Anno, objParametriServer);
            var resaTask    = _resaService.GetResaAsync(esercizi, perimetro.Anno, objParametriServer);
            var pioggeTask  = _precipitazioniService.GetPrecipitazioniAsync(
                                  esercizi, perimetro.Anno,
                                  objParametriServer, objParametriSuperServer);

            await Task.WhenAll(consumoTask, resaTask, pioggeTask);

            var volumiResult         = consumoTask.Result;
            var resaResult           = resaTask.Result;
            var precipitazioniResult = pioggeTask.Result;

            // Popola i campi anagrafici di EsercizioH2O dai dati ricavati da DS04 (LeggiImpiantiAsync raccolta)
            foreach (var e in esercizi)
            {
                if (resaResult.DatiImpiantoPerEsercizio.TryGetValue(e.IdEsercizio, out var dati))
                {
                    e.Azienda      = dati.Piva;
                    e.Appezzamento = dati.Appezza;
                    e.SuperficieHa = dati.SupApp;
                }
                if (resaResult.LottoRaccoltaPerEsercizio.TryGetValue(e.IdEsercizio, out var lotto))
                    e.LottoRaccolta = lotto;            }

            // DS06: lookup WF benchmark per ogni esercizio (parallelo)
            // Veg_Cod ricavato da DS04 (LeggiImpiantiAsync raccolta) — non fornito dal chiamante
            var wfTasks = esercizi.Select(e =>
            {
                resaResult.VegCodPerEsercizio.TryGetValue(e.IdEsercizio, out var vegCod);
                return _benchmarkService.GetWaterFootprintAsync(
                    vegCod,
                    e.Nazione,
                    string.IsNullOrWhiteSpace(e.IstatRegione) ? null : e.IstatRegione,
                    objParametriServer);
            }).ToList();

            await Task.WhenAll(wfTasks);

            // DS07: calcola indicatori
            var output = new BilancioIdricoIndicatoriOutput();
            var indicatoriDizionario = new Dictionary<string, IndicatoreBilancioIdrico>(esercizi.Count);

            for (int i = 0; i < esercizi.Count; i++)
            {
                var esercizio = esercizi[i];
                var idStr = esercizio.IdEsercizio.ToString();

                volumiResult.VolumiPerEsercizio.TryGetValue(esercizio.IdEsercizio, out var consumoM3);
                resaResult.ResaPerEsercizio.TryGetValue(esercizio.IdEsercizio, out var resaT);
                precipitazioniResult.PioggiaMmPerEsercizio.TryGetValue(esercizio.IdEsercizio, out var pioggiaMm);

                var wfM3PerT = wfTasks[i].Result;

                var bilancioInput = new BilancioIdricoInput
                {
                    IdEsercizio              = idStr,
                    ConsumoIdricoEffettivoM3 = consumoM3 == 0m ? null : consumoM3,
                    ResaTonnellate           = resaT == 0m ? null : resaT,
                    PrecipitazioniMm         = pioggiaMm,
                    SuperficieAppezzamentoHa = esercizio.SuperficieHa,
                    GreenBlueWfM3PerT        = wfM3PerT,
                    ModalitaCalcolo          = perimetro.Modalita
                };

                var indicatore = CalcolaIndicatore(bilancioInput);
                output.Indicatori.Add(indicatore);
                indicatoriDizionario[idStr] = indicatore;
            }

            LogInformation(
                "DS07-BL completato: {Count} indicatori calcolati. id_invocazione={IdInvocazione}",
                objParametriServer, null, output.Indicatori.Count, idInvocazione);

            // DS08 o DS09+DS10 in base alla modalità
            if (string.Equals(perimetro.Modalita, "Colture", StringComparison.OrdinalIgnoreCase))
            {
                await PersistiPerColtureAsync(
                    esercizi, indicatoriDizionario, perimetro,
                    idInvocazione, dataCalcolo, resaResult, objParametriServer, objParametriSuperServer);
            }
            else if (string.Equals(perimetro.Modalita, "Aziendale", StringComparison.OrdinalIgnoreCase))
            {
                await PersistiPerAziendaAsync(
                    esercizi, indicatoriDizionario, perimetro, dataCalcolo, resaResult, objParametriServer, objParametriSuperServer);
            }

            return JsonSerializer.Serialize(output, _jsonOptions);
        }

        // ── DS08 Persistenza Per Colture ─────────────────────────────────────────

        private async Task PersistiPerColtureAsync(
            List<EsercizioH2O> esercizi,
            Dictionary<string, IndicatoreBilancioIdrico> indicatoriDizionario,
            PerimetroCalcoloH2O perimetro,
            Guid idInvocazione,
            DateTime dataCalcolo,
            ResaProduttivaResult resaResult,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var risultati = new List<WriteLookupSostH2OLotto>(esercizi.Count);

            var imprese = _serviceProvider.GetRequiredService<IImpresa>();
            var cuaaFiliera = await imprese.CuaaFromPivaAsync(perimetro.Filiera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new DataNotFoundException($"CUAA non trovato per la filiera PIVA '{perimetro.Filiera}'.");

            foreach (var e in esercizi)
            {
                var idStr = e.IdEsercizio.ToString();
                if (!indicatoriDizionario.TryGetValue(idStr, out var indicatore))
                    continue;

                resaResult.ResaPerEsercizio.TryGetValue(e.IdEsercizio, out var resaT);
                var indicatoreArrotondato = new IndicatoreBilancioIdrico
                {
                    IdEsercizio       = indicatore.IdEsercizio,
                    FabbisognoM3PerHa = indicatore.FabbisognoM3PerHa.HasValue ? Math.Round(indicatore.FabbisognoM3PerHa.Value, 2) : null,
                    ConsumataM3PerHa  = indicatore.ConsumataM3PerHa.HasValue  ? Math.Round(indicatore.ConsumataM3PerHa.Value,  2) : null,
                    DaMeteoM3PerHa    = indicatore.DaMeteoM3PerHa.HasValue    ? Math.Round(indicatore.DaMeteoM3PerHa.Value,    2) : null,
                    DeltaM3PerHa      = indicatore.DeltaM3PerHa.HasValue      ? Math.Round(indicatore.DeltaM3PerHa.Value,      2) : null
                };
                var payloadJson = JsonSerializer.Serialize(indicatoreArrotondato, _jsonOptions);
                resaResult.CulCodPerEsercizio.TryGetValue(e.IdEsercizio, out var culCod);
                resaResult.VegCodPerEsercizio.TryGetValue(e.IdEsercizio, out var vegCod);

                byte[]? jsonFirmato;
                try
                {
                   jsonFirmato = await _firmaDigitaleService.SignAsync(payloadJson, false, objParametriServer, objParametriSuperServer);
                }
                catch (Exception ex)
                {
                    LogError($"Errore PersistiPerColtureAsync sulla firma del payload: {payloadJson}, dettagli eccezione: {ex.Message}");
                    jsonFirmato = Array.Empty<byte>();
                }

                var cuaaAzienda = await imprese.CuaaFromPivaAsync(e.Azienda, objParametriServer);
                if (string.IsNullOrWhiteSpace(cuaaAzienda))
                    throw new DataNotFoundException($"CUAA non trovato per l'azienda ' PIVA '{e.Azienda}'.");

                risultati.Add(new WriteLookupSostH2OLotto
                {
                    DataCalcolo           = dataCalcolo,
                    CuaaFiliera           = cuaaFiliera,
                    CuaaAzienda           = cuaaAzienda,
                    Anno                  = perimetro.Anno,
                    Appezzamento          = e.Appezzamento,
                    CulCod                = culCod,
                    VegCod                = vegCod == 0 ? null : vegCod,
                    Esercizio             = idStr,
                    LottoRaccolta         = e.LottoRaccolta,
                    Nazione               = e.Nazione,
                    Regione               = e.IstatRegione,
                    QuantitaRaccoltaKg    = resaT * 1000m,
                    SuperficieRiferimento = e.SuperficieHa,
                    PayloadJson           = payloadJson,
                    JsonFirmato           = jsonFirmato ?? Array.Empty<byte>(),
                    Username_Creazione    = objParametriServer.UtenteUsername,
                    Username_Modifica     = objParametriServer.UtenteUsername
                });
            }

            var coltureInput = new PersistenzaRisultatiPerColtureInput
            {
                IdInvocazione = idInvocazione,
                DataCalcolo   = dataCalcolo,
                Risultati     = risultati
            };

            var coltureOutput = await _coltureService.EseguiAsync(coltureInput, objParametriServer);

            LogInformation(
                "DS08-BL completato: {RowsInserted} righe inserite. id_invocazione={IdInvocazione}",
                objParametriServer, null, coltureOutput.RowsInserted, idInvocazione);
        }

        // ── DS09+DS10 Persistenza Per Azienda ────────────────────────────────────

        private async Task PersistiPerAziendaAsync(
            List<EsercizioH2O> esercizi,
            Dictionary<string, IndicatoreBilancioIdrico> indicatoriDizionario,
            PerimetroCalcoloH2O perimetro,
            DateTime dataCalcolo,
            ResaProduttivaResult resaResult,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var gruppiPerAzienda = esercizi.GroupBy(e => e.Azienda);

            var imprese = _serviceProvider.GetRequiredService<IImpresa>();
            var cuaaFiliera = await imprese.CuaaFromPivaAsync(perimetro.Filiera, objParametriServer);
            if (string.IsNullOrWhiteSpace(cuaaFiliera))
                throw new DataNotFoundException($"CUAA non trovato per la filiera PIVA '{perimetro.Filiera}'.");

            foreach (var gruppo in gruppiPerAzienda)
            {
                

                var azienda = gruppo.Key;
                var eserciziAzienda = gruppo.ToList();
                var idInvocazione = Guid.NewGuid();


                var cuaaAzienda = await imprese.CuaaFromPivaAsync(azienda, objParametriServer);
                if (string.IsNullOrWhiteSpace(cuaaAzienda))
                    throw new DataNotFoundException($"CUAA non trovato per l'azienda ' PIVA '{azienda}'.");

                // DS09 — Aggregazione payload
                var aggrInput = new AggregazionePayloadPerAziendaInput
                {
                    IdInvocazione      = idInvocazione,
                    DataCalcolo        = dataCalcolo,
                    Filiera            = cuaaFiliera,
                    Azienda            = cuaaAzienda,
                    Anno               = perimetro.Anno,
                    IndicatoriEsercizi = eserciziAzienda.Select(e =>
                    {
                        indicatoriDizionario.TryGetValue(e.IdEsercizio.ToString(), out var ind);
                        return new AggregazioneEsercizioInput
                        {
                            SuperficieHa      = e.SuperficieHa,
                            FabbisognoM3PerHa = ind?.FabbisognoM3PerHa,
                            ConsumataM3PerHa  = ind?.ConsumataM3PerHa,
                            DaMeteoM3PerHa    = ind?.DaMeteoM3PerHa,
                            DeltaM3PerHa      = ind?.DeltaM3PerHa
                        };
                    }).ToList()
                };

                var aggrOutput = _aggregazioneService.Aggrega(aggrInput);

                // DS10 — Persistenza chiavi+payload
                var firstEsercizio = eserciziAzienda[0];
                var quantitaTotaleKg = eserciziAzienda.Sum(e =>
                {
                    resaResult.ResaPerEsercizio.TryGetValue(e.IdEsercizio, out var resaT);
                    return resaT * 1000m;
                });
                var superficieTotaleHa = eserciziAzienda.Sum(e => e.SuperficieHa);

                byte[]? jsonFirmato;
                try
                {
                    jsonFirmato = await _firmaDigitaleService.SignAsync(aggrOutput, false, objParametriServer, objParametriSuperServer);
                } catch (Exception ex)
                {
                    LogError($"Errore PersistiPerAziendaAsync sulla firma del payload: {aggrOutput}, dettagli eccezione: {ex.Message}");
                    jsonFirmato = Array.Empty<byte>();
                }

                var persistInput = new PersistenzaChiaviPayloadPerAziendaInput
                {
                    IdInvocazione         = idInvocazione,
                    DataCalcolo           = dataCalcolo,
                    Filiera               = cuaaFiliera,
                    Azienda               = cuaaAzienda,
                    Anno                  = perimetro.Anno,
                    Nazione               = firstEsercizio.Nazione,
                    Regione               = firstEsercizio.IstatRegione,
                    Specie                = firstEsercizio.Specie,
                    Varieta               = firstEsercizio.Varieta,
                    QuantitaRaccoltaKg    = quantitaTotaleKg,
                    SuperficieColtivataHa = superficieTotaleHa,
                    PayloadJson           = aggrOutput,
                    JsonFirmato           = jsonFirmato ?? Array.Empty<byte>()
                };

                await _persistenzaAziendaService.EseguiAsync(persistInput, objParametriServer);

                LogInformation(
                    "DS10-BL completato per azienda={Azienda}. id_invocazione={IdInvocazione}",
                    objParametriServer, null, azienda, idInvocazione);
            }
        }

        // ── DS07-BL formula ──────────────────────────────────────────────────────

        private static IndicatoreBilancioIdrico CalcolaIndicatore(BilancioIdricoInput input)
        {
            if (input.ConsumoIdricoEffettivoM3 is null
                || input.ResaTonnellate is null
                || input.SuperficieAppezzamentoHa <= 0m
                || input.GreenBlueWfM3PerT <= 0m)
            {
                return new IndicatoreBilancioIdrico
                {
                    IdEsercizio             = input.IdEsercizio
                };
            }

            var consumoM3    = input.ConsumoIdricoEffettivoM3.Value;
            var resaT        = input.ResaTonnellate.Value;
            var superficieHa = input.SuperficieAppezzamentoHa;
            var wfM3PerT     = input.GreenBlueWfM3PerT;
            var pioggiaMm    = input.PrecipitazioniMm ?? 0m;

            // Tutti i calcoli intermedi e il valore di ritorno mantengono la precisione completa.
            // Non si arrotonda qui perché questi valori per-ha vengono:
            //   - moltiplicati per la superficie in AggregazionePayloadPerAziendaService (Aziendale)
            //   - ri-aggregati via media pesata in SostenibilitaH2OService (Colture)
            // In entrambi i casi sommare valori già arrotondati introduce un errore sistematico
            // proporzionale al numero di esercizi. L'arrotondamento a 2 dec avviene:
            //   - sui totali m³ finali in AggregazionePayloadPerAziendaService (Math.Round al JSON)
            //   - sul payload_json di Lookup_Sost_H2O_Lotto in PersistiPerColtureAsync (Math.Round)
            //   - sull'output API in SostenibilitaH2OService (Round2)
            var fabbisognoM3PerHa = (wfM3PerT * resaT) / superficieHa;
            var consumataM3PerHa  = consumoM3 / superficieHa;
            var daMeteoM3PerHa    = pioggiaMm * _mmToM3PerHa * _fattoreEfficienzaPioggia;
            var deltaM3PerHa      = consumataM3PerHa - fabbisognoM3PerHa;

            return new IndicatoreBilancioIdrico
            {
                IdEsercizio       = input.IdEsercizio,
                FabbisognoM3PerHa = fabbisognoM3PerHa,
                ConsumataM3PerHa  = consumataM3PerHa,
                DaMeteoM3PerHa    = daMeteoM3PerHa,
                DeltaM3PerHa      = deltaM3PerHa
            };
        }
    }
}
