using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.BilancioAzoto.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.BilancioAzoto;
using AgronicaNetCore.Operazione.DAL.DataLayer.Fertilizzazioni;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Globalization;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.BilancioAzoto
{
    /// <summary>
    /// Servizio BIZ per l'aggregazione degli eventi della timeline del bilancio azoto
    /// e l'invio delle richieste consigli nutrizionali aggregate all'engine.
    /// Flusso: caricamento eventi BBCH/Analisi dal DAL → caricamento fertilizzazioni →
    ///         costruzione timeline ordinata → aggregazione per data →
    ///         richiesta consiglio engine per ogni data con stato aggiornato.
    /// Riferimento: DS09-BL BilancioAzotoAggregazioneEventi.
    /// </summary>
    public sealed class BilancioAzotoService : BaseDSSNutrizioneBIZService, IBilancioAzotoService
    {
        // Tipi evento come costanti per evitare stringhe magic nel codice.
        private const string TipoEventoBbch        = "BBCH_CHANGE";
        private const string TipoEventoAnalisi      = "ANALISI_TERRENO";
        private const string TipoEventoFertilizz    = "FERTILIZZAZIONE";

        private readonly IBilancioAzotoDAL                      _bilancioAzotoDAL;
        private readonly IFertilizzazioniDAL                    _fertilizzazioniDAL;
        private readonly IRichiestaConsiglioNutrizioneService   _richiestaConsiglioService;
        private readonly IVerificaModelliNutrizioneService      _verificaModelliService;
        private readonly IAgro_Sequence _seq;

        public BilancioAzotoService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _bilancioAzotoDAL            = provider.GetRequiredService<IBilancioAzotoDAL>();
            _fertilizzazioniDAL          = provider.GetRequiredService<IFertilizzazioniDAL>();
            _richiestaConsiglioService   = provider.GetRequiredService<IRichiestaConsiglioNutrizioneService>();
            _verificaModelliService      = provider.GetRequiredService<IVerificaModelliNutrizioneService>();
            _seq = provider.GetRequiredService<IAgro_Sequence>();
        }

        /// <inheritdoc/>
        public async Task<BilancioAzotoResult> AggregazioneEventiEConsiglioAsync(
            AppezzamentoNutrizioneDto appezzamento,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            int idDbServer,
            CancellationToken cancellationToken = default)
        {
            if (appezzamento is null)           throw new ArgumentNullException(nameof(appezzamento));
            if (objParametriServer is null)     throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            // ── Step 1: caricamento eventi BBCH_CHANGE e ANALISI_TERRENO dal DAL ──────
            DataTable dtEventi = await _bilancioAzotoDAL.LeggiEventiTimelineAsync(
                appezzamento.Piva,
                appezzamento.SaCod,
                appezzamento.Appezza,
                appezzamento.IdReg,
                appezzamento.ProgettoCod,
                objParametriServer);

            if (dtEventi.Rows.Count == 0)
            {
                LogInformation(
                    $"Nessun evento nella timeline bilancio azoto. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}.",
                    objParametriServer);
                return new BilancioAzotoResult();
            }

            // La finestra di validità è presente in ogni riga; la estraggo dalla prima.
            DataRow primaRiga = dtEventi.Rows[0];
            var validitaInizio = primaRiga.Field<DateTime>("Validita_Inizio");
            var validitaFine   = primaRiga.Field<DateTime>("Validita_Fine");

            // ── Step 2: recupero ModelCode dall'engine modelli ───────────────────────
            // DS09-BL: le richieste all'engine richiedono un ModelCode valido per specie/varietà.
            string modelCode = string.Empty;
            try
            {
                var coppie = new List<CoppiaSpecieVarieta>
                {
                    new() { Veg_Cod = appezzamento.SpecieCod, Cul_Cod = appezzamento.VarietaCod }
                };
                var modelliResult = await _verificaModelliService.VerificaAsync(
                    coppie, objParametriServer, objParametriSuperServer, cancellationToken);

                modelCode = modelliResult.ModelliPerSpecieVarieta
                    .FirstOrDefault(m => m.Status == StatoModelloNutrizione.Available)
                    ?.Modelli.FirstOrDefault()?.Code ?? string.Empty;
            }
            catch (Exception ex)
            {
                LogWarning(
                    $"Errore nel recupero del ModelCode dall'engine modelli. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}. {ex.GetBaseException().Message}",
                    objParametriServer);
            }

            if (string.IsNullOrEmpty(modelCode))
            {
                LogWarning(
                    $"ModelCode non disponibile. Le richieste consiglio non verranno inviate all'engine. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}.",
                    objParametriServer);
            }

            // ── Step 3: caricamento TUTTE le fertilizzazioni nell'arco di validità ───
            // Pattern identico a WidgetNutrizioneService.AggregaConsiglioNutrizioneAsync.
            // I dati vengono caricati una sola volta e filtrati per data nel ciclo degli eventi.
            var tutteleFertilizzazioni = new List<DatiFertilizzazionePrecedenteInput>();
            try
            {
                DataTable dtFert = await _fertilizzazioniDAL.LeggiMacroelementiDistribuitiAsync(
                    new[]
                    {
                        LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME,
                        LAV_COD.LAVCOD_FERTIRRIGAZIONE,
                        LAV_COD.LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                        LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE,
                        LAV_COD.LAVCOD_SARCHIATURA_CONCIMAZIONE
                    },
                    appezzamento.Piva,
                    appezzamento.SaCod,
                    appezzamento.Appezza,
                    appezzamento.IdReg,
                    validitaInizio,
                    validitaFine,
                    objParametriServer);

                foreach (DataRow row in dtFert.Rows)
                {
                    tutteleFertilizzazioni.Add(new DatiFertilizzazionePrecedenteInput
                    {
                        IdAgenda          = row.Field<int>("Id_Agenda"),
                        IdMov             = row.Field<int>("Id_Mov"),
                        IdMovDet          = row.Field<int>("Id_Mov_Det"),
                        Elemento          = row.Field<string>("Elemento")!,
                        Quantitativo      = Convert.ToDouble(row["Quantitativo"]),
                        FaseFenologicaBbch = row.Field<string>("Fase_BBCH") ?? string.Empty,
                        Data              = row.Field<DateTime>("Data_Movimento")
                    });
                }
            }
            catch (Exception ex)
            {
                // Fallimento isolato: le fertilizzazioni non bloccano la costruzione della timeline.
                LogWarning(
                    $"Errore nel caricamento fertilizzazioni per bilancio azoto. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}. {ex.GetBaseException().Message}",
                    objParametriServer);
            }

            // ── Step 4: costruzione lista unificata di eventi in memoria ─────────────
            // Include BBCH_CHANGE, ANALISI_TERRENO (da DAL) e FERTILIZZAZIONE (da FertilizzazioniDAL).
            var eventiInMemoria = new List<EventoInternoTimeline>();

            // 4a: eventi da DAL (BBCH_CHANGE + ANALISI_TERRENO)
            foreach (DataRow row in dtEventi.Rows)
            {
                eventiInMemoria.Add(new EventoInternoTimeline(
                    DataEvento:   row.Field<DateTime>("Data_Evento"),
                    TipoEvento:   row.Field<string>("Tipo_Evento")!,
                    RigaDAL:      row));
            }

            // 4b: eventi FERTILIZZAZIONE — una voce per movimento unico (Id_Agenda/Id_Mov/Id_Mov_Det).
            // LeggiMacroelementiDistribuitiAsync restituisce una riga per elemento (N, P, K);
            // si deduplica per chiave movimento per avere un singolo evento timeline.
            var movimentiDistincti = tutteleFertilizzazioni
                .GroupBy(f => (f.IdAgenda, f.IdMov, f.IdMovDet))
                .Select(g => g.First());

            foreach (var fert in movimentiDistincti)
            {
                eventiInMemoria.Add(new EventoInternoTimeline(
                    DataEvento:   fert.Data.Date,
                    TipoEvento:   TipoEventoFertilizz,
                    RigaDAL:      null,
                    Fertilizzazione: fert));
            }

            // Ordine crescente per data (DS09-BL: "eventi ordinati cronologicamente per data crescente").
            eventiInMemoria.Sort((a, b) => a.DataEvento.CompareTo(b.DataEvento));

            // ── Step 5: raggruppamento per data e invio richieste all'engine ─────────
            // DS09-BL: "Aggregazione per data: se più eventi avvengono nella medesima data,
            // vengono aggregati in una singola richiesta consiglio".

            var timelineEventi      = new List<EventoTimelineDto>();
            var richiesteAggregate  = new List<RichiestaConsiglioAggregataDto>();

            // Stato mobile (rolling state): riflette i parametri correnti alla data di ogni gruppo.
            var currentBbch     = appezzamento.FaseFenologicaCorrente;
            var currentAnalisi  = appezzamento.AnalisiTerreno;

            // DataSemina: coerente con il pattern di AggregaConsiglioNutrizioneAsync.
            DateTime dataSemina = DateTime.TryParse(appezzamento.DataSeminaPrevista, out var ds)
                ? ds
                : CostantiPersonalizzate.AGRODATAINIZIO_DATE;

            var gruppiPerData = eventiInMemoria
                .GroupBy(e => e.DataEvento.Date)
                .OrderBy(g => g.Key);

            if(gruppiPerData is not null && gruppiPerData.Any())
            {
                int raccoglitoreCod = await _seq.NuovoId_TabellaAsync("Raccoglitore_WebHook_Testata", 0, 2_000_000_000, objParametriServer); ;

                foreach (var gruppo in gruppiPerData)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    DateTime dataGruppo = gruppo.Key;
                    var eventiDelGruppo = gruppo.ToList();

                    // 5a: aggiorna lo stato mobile con gli eventi di questa data.
                    foreach (var evento in eventiDelGruppo)
                    {
                        if (evento.TipoEvento == TipoEventoBbch && evento.RigaDAL is not null)
                        {
                            currentBbch = new FaseFenologicaCorrenteDto
                            {
                                IdAgenda = evento.RigaDAL.IsNull("Id_Agenda") ? 0 : evento.RigaDAL.Field<int>("Id_Agenda"),
                                IdMov = evento.RigaDAL.IsNull("Id_Mov") ? 0 : evento.RigaDAL.Field<int>("Id_Mov"),
                                IdMovDet = evento.RigaDAL.IsNull("Id_Mov_Det") ? 0 : evento.RigaDAL.Field<int>("Id_Mov_Det"),
                                BbchCod = evento.RigaDAL.Field<string?>("BBCH_Cod"),
                                BbchDescrizione = null,
                                DataFase = dataGruppo.ToString("yyyy-MM-ddTHH:mm:ssZ")
                            };
                        }
                        else if (evento.TipoEvento == TipoEventoAnalisi && evento.RigaDAL is not null)
                        {
                            currentAnalisi = new AnalisiTerrenoDto
                            {
                                AnalisiSuperUser = evento.RigaDAL.IsNull("Analisi_SuperUser") ? string.Empty : evento.RigaDAL.Field<string>("Analisi_SuperUser")!,
                                AnalisiTestataCod = evento.RigaDAL.IsNull("Analisi_Testata_Cod") ? 0 : evento.RigaDAL.Field<int>("Analisi_Testata_Cod"),
                                AnalisiDettaglioCod = 0,
                                AnalisiParametroCod = 0,
                                SabbiaPercentuale = evento.RigaDAL.IsNull("Sabbia_Percentuale") ? null : Convert.ToDouble(evento.RigaDAL["Sabbia_Percentuale"]),
                                LimoPercentuale = evento.RigaDAL.IsNull("Limo_Percentuale") ? null : Convert.ToDouble(evento.RigaDAL["Limo_Percentuale"]),
                                ArgillaPercentuale = evento.RigaDAL.IsNull("Argilla_Percentuale") ? null : Convert.ToDouble(evento.RigaDAL["Argilla_Percentuale"]),
                                NTotale = evento.RigaDAL.IsNull("N_Totale") ? null : Convert.ToDouble(evento.RigaDAL["N_Totale"]),
                                DataAnalisi = dataGruppo
                            };
                        }
                    }

                    // 5b: costruisce le voci della timeline per questa data.
                    foreach (var evento in eventiDelGruppo)
                    {
                        timelineEventi.Add(new EventoTimelineDto
                        {
                            DataEvento = dataGruppo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            EventoId = Guid.NewGuid().ToString(),
                            TipoEvento = evento.TipoEvento,
                            Descrizione = BuildDescrizioneEvento(evento, currentBbch)
                        });
                    }

                    // 5c: se il ModelCode non è disponibile, registra la richiesta senza consiglio.
                    if (string.IsNullOrEmpty(modelCode))
                    {
                        richiesteAggregate.Add(new RichiestaConsiglioAggregataDto
                        {
                            DataRichiesta = dataGruppo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            NumeroEventiAggregati = eventiDelGruppo.Count,
                            ConsiglioId = null
                        });
                        continue;
                    }

                    // 5d: filtra le fertilizzazioni cumulative fino alla data corrente.
                    // DS09-BL: la richiesta include TUTTI i parametri modificati fino alla data evento.
                    var fertilizzazioniCumulative = tutteleFertilizzazioni
                        .Where(f => f.Data.Date <= dataGruppo)
                        .ToList()
                        .AsReadOnly();

                    // 5e: costruisce il payload per la richiesta engine.
                    // Pattern identico a WidgetNutrizioneService.AggregaConsiglioNutrizioneAsync.
                    var richiestaInput = new RichiestaConsiglioNutrizioneInput
                    {
                        DataConsiglio = dataGruppo,
                        DataSemina = dataSemina,
                        ModelCode = modelCode,
                        DatiImpianto = new DatiImpiantoInput
                        {
                            Piva = appezzamento.Piva,
                            SaCod = appezzamento.SaCod,
                            Appezza = appezzamento.Appezza,
                            IdReg = appezzamento.IdReg,
                            VegCod = appezzamento.SpecieCod,
                            CulCod = appezzamento.VarietaCod,
                            NumPiante = (int?)appezzamento.NumPiante,
                            PortinnestoCod = appezzamento.Portinnesto,
                            StatoImpianto = appezzamento.StatoImpianto,
                            GeometriaWkt = (appezzamento.CoordinataLat.HasValue && appezzamento.CoordinataLng.HasValue)
                                ? $"POINT({appezzamento.CoordinataLng.Value.ToString(new CultureInfo("en-US"))} {appezzamento.CoordinataLat.Value.ToString(new CultureInfo("en-US"))})"
                                : null,
                            GeometriaSrid = (appezzamento.CoordinataLat.HasValue && appezzamento.CoordinataLng.HasValue) ? "EPSG:4326" : null
                        },
                        DatiFaseFenologica = currentBbch is null ? null : new DatiFaseFenologicaInput
                        {
                            IdAgenda = currentBbch.IdAgenda,
                            IdMov = currentBbch.IdMov,
                            IdMovDet = currentBbch.IdMovDet,
                            CodiceBbch = currentBbch.BbchCod ?? string.Empty
                        },
                        DatiAnalisiTerreno = currentAnalisi?.DataAnalisi is null ? null : new DatiAnalisiTerrenoInput
                        {
                            AnalisiSuperUser = currentAnalisi.AnalisiSuperUser ?? string.Empty,
                            AnalisiTestataCod = currentAnalisi.AnalisiTestataCod,
                            AnalisiDettaglioCod = currentAnalisi.AnalisiDettaglioCod,
                            AnalisiParametroCod = currentAnalisi.AnalisiParametroCod,
                            Data = currentAnalisi.DataAnalisi.Value,
                            Sabbia = (int)(currentAnalisi.SabbiaPercentuale ?? 0),
                            Limo = (int)(currentAnalisi.LimoPercentuale ?? 0),
                            Argilla = (int)(currentAnalisi.ArgillaPercentuale ?? 0),
                            ElencoElementiRilevati = new List<ElementoRilevatoInput>
                        {
                            new() { Elemento = "N", Quantitativo = currentAnalisi.NTotale ?? 0 }
                        }
                        },
                        DatiFertilizzazioniPrecedenti = fertilizzazioniCumulative
                    };

                    // 5f: invio richiesta all'engine; fallimento isolato non blocca le date successive.
                    // DS09-BL: "Raccoglitore_Cod già commentato nel codice" — raccoglitoreCod=0 per default.
                    int? consiglioId = null;
                    try
                    {
                        var risultato = await _richiestaConsiglioService.EseguiAsync(
                            richiestaInput,
                            objParametriServer.UtenteUsername,
                            objParametriServer,
                            objParametriSuperServer,
                            idDbServer,
                            salvaConsiglio: true,
                            raccoglitoreCod: raccoglitoreCod,
                            cancellationToken);

                        consiglioId = risultato.Status == "SUCCESS" ? risultato.ConsiglioId : null;
                    }
                    catch (Exception ex)
                    {
                        LogWarning(
                            $"Errore nella richiesta consiglio engine per data={dataGruppo:yyyy-MM-dd}. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}. {ex.GetBaseException().Message}",
                            objParametriServer);
                    }

                    richiesteAggregate.Add(new RichiestaConsiglioAggregataDto
                    {
                        DataRichiesta = dataGruppo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        NumeroEventiAggregati = eventiDelGruppo.Count,
                        ConsiglioId = consiglioId
                    });
                }
            }


            return new BilancioAzotoResult
            {
                TimelineEventi             = timelineEventi.AsReadOnly(),
                RichiesteConsiglioAggregate = richiesteAggregate.AsReadOnly()
            };
        }

        // ── Helper: descrizione leggibile dell'evento ───────────────────────────────

        private static string BuildDescrizioneEvento(
            EventoInternoTimeline evento,
            FaseFenologicaCorrenteDto? currentBbch)
        {
            return evento.TipoEvento switch
            {
                TipoEventoBbch     => $"Cambio fase BBCH: {currentBbch?.BbchCod ?? "N/D"}",
                TipoEventoAnalisi  => "Nuova analisi del terreno registrata",
                TipoEventoFertilizz => "Fertilizzazione applicata",
                _                  => evento.TipoEvento
            };
        }

        // ── Tipo record interno per la gestione della timeline in memoria ───────────

        private sealed record EventoInternoTimeline(
            DateTime DataEvento,
            string TipoEvento,
            DataRow? RigaDAL,
            DatiFertilizzazionePrecedenteInput? Fertilizzazione = null);
    }
}
