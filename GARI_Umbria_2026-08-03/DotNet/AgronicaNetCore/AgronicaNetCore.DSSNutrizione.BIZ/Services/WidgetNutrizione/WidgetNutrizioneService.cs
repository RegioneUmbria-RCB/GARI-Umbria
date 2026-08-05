using AgronicaNetCore.Anagrafe.DAL.DataLayer.Esercizi;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.FasiFenologiche.Orchestration;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer.Fertilizzazioni;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili;
using InData.Engine.FasiFenologiche;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione
{
    /// <summary>
    /// Servizio BIZ per il caricamento e l'aggregazione dei dati widget DSS Nutrizione.
    /// Aggrega dati colturali, analisi terreno, fasi fenologiche e consigli nutrizionali
    /// per ogni appezzamento attivo dell'azienda selezionata.
    /// Riferimento: DS05-BL Caricamento Dati Widget Nutrizione; DS11-API GET /v1/dss/nutrizione/appezzamenti.
    /// </summary>
    public sealed class WidgetNutrizioneService : BaseDSSNutrizioneBIZService, IWidgetNutrizioneService
    {
        private readonly IRichiestaConsiglioNutrizioneService _richiestaConsiglioService;
        private readonly IVerificaModelliNutrizioneService _verificaModelliService;
        private readonly IUtentiProfili _utentiProfili;
        private readonly IOrchestrationAcquisizioneFasiFenologicheService _orchestrationFasiFenologiche;
        private readonly IConfrontiFasiFenologicheQDCAService _confrontiFasiFenologiche;
        private readonly IEsercizi _eserciziDAL;
        private readonly IFertilizzazioniDAL _fertilizzazioniNutrizioneDAL;

        public WidgetNutrizioneService(IServiceProvider provider,IStringLocalizer<Messages> localizer): base(provider, localizer)
        {
            _utentiProfili = _serviceProvider.GetRequiredService<IUtentiProfili>();
            _orchestrationFasiFenologiche = _serviceProvider.GetRequiredService<IOrchestrationAcquisizioneFasiFenologicheService>();
            _confrontiFasiFenologiche = _serviceProvider.GetRequiredService<IConfrontiFasiFenologicheQDCAService>();
            _eserciziDAL = _serviceProvider.GetRequiredService<IEsercizi>();
            _richiestaConsiglioService = _serviceProvider.GetRequiredService<IRichiestaConsiglioNutrizioneService>();
            _verificaModelliService = _serviceProvider.GetRequiredService<IVerificaModelliNutrizioneService>();
            _fertilizzazioniNutrizioneDAL = _serviceProvider.GetRequiredService<IFertilizzazioniDAL>();
        }

        /// <inheritdoc/>
        public async Task<DatiWidgetNutrizioneResult> CaricaDatiWidgetNutrizioneAsync(
            string piva,
            int saCod,
            int annoSolare,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti)
        {
            // Fail fast: PIVA è obbligatoria per filtrare la query a livello DB.
            if (string.IsNullOrWhiteSpace(piva))
                throw new ArgumentException("La PIVA è obbligatoria per il caricamento dei dati widget nutrizione.", nameof(piva));

            // Determina se applicare il filtro di visibilità per i centri autorizzati.
                // VisibilitaTotaleGiasOnline restituisce true quando l'utente è un utente
                // GIAS Online con visibilità limitata ai centri in Utenti_Visibilita_Appoggio.
                bool filtroVisibilita = await _utentiProfili.VisibilitaTotaleGiasOnline(
                    objParametriUtenti, objParametriServer);

                DataTable dt = await _eserciziDAL.LeggiEserciziDSSNutrizioneAsync(
                    piva,
                    saCod,
                    annoSolare,
                    objParametriServer,
                    filtroVisibilitaUtente: false);



                if (dt == null || dt.Rows.Count == 0)
                {
                    return new DatiWidgetNutrizioneResult
                    {
                        TotaleAppezzamenti = 0,
                        PaginaAttuale = 0,
                        RecordPerPagina = 0,
                        Appezzamenti = Array.Empty<AppezzamentoNutrizioneDto>()
                    };
                }

                int totale = Convert.ToInt32(dt.Rows[0]["Totale_Appezzamenti"]);
                var appezzamenti = MapDataTableToAppezzamentiDto(dt);

                return new DatiWidgetNutrizioneResult
                {
                    TotaleAppezzamenti = totale,
                    PaginaAttuale = 0,
                    RecordPerPagina = 0,
                    Appezzamenti = appezzamenti
                };
        }

        /// <summary>
        /// Mappa il DataTable piatto (una riga per elemento del consiglio) nella struttura
        /// DTO gerarchica: ogni AppezzamentoNutrizioneDto contiene il suo ConsiglioNutrizioneUltimoDto
        /// con la lista degli ElementoNutrizioneDto.
        /// Riferimento: DS05-BL - Output appezzamenti (struttura annidata).
        /// </summary>
        private static IReadOnlyList<AppezzamentoNutrizioneDto> MapDataTableToAppezzamentiDto(DataTable dt)
        {
            // Chiave composita di identità dell'impianto: (PIVA, SA_COD, APPEZZA, ID_REG).
            // Le righe sono ordinate per Nome_Appezzamento; ogni appezzamento può avere
            // più righe se ha più elementi nel consiglio nutrizionale.
            var result = new List<AppezzamentoNutrizioneDto>();
            string? currentKey = null;
            var currentElementi = new List<ElementoNutrizioneDto>();
            DataRow? firstRowForAppezza = null;

            foreach (DataRow row in dt.Rows)
            {
                string rowPiva = row.Field<string>("PIVA")!;
                int rowSaCod = row.Field<int>("SA_COD");
                int rowAppezza = row.Field<int>("APPEZZA");
                int rowIdReg = row.Field<int>("ID_REG");
                int rowProgettoCod = row.Field<int>("PROGETTO_COD");

                string rowKey = $"{rowPiva}|{rowSaCod}|{rowAppezza}|{rowIdReg}|{rowProgettoCod}";

                // Quando cambia l'appezzamento, finalizza il precedente e inizia il nuovo.
                if (rowKey != currentKey)
                {
                    if (firstRowForAppezza != null)
                        result.Add(BuildAppezzamentoDto(firstRowForAppezza, currentElementi));

                    currentKey = rowKey;
                    firstRowForAppezza = row;
                    currentElementi = new List<ElementoNutrizioneDto>();
                }

                // Aggiunge l'elemento nutrizionale se presente (campo Elemento non NULL).
                if (!row.IsNull("Elemento"))
                    currentElementi.Add(BuildElementoDto(row));
            }

            // Finalizza l'ultimo appezzamento.
            if (firstRowForAppezza != null)
                result.Add(BuildAppezzamentoDto(firstRowForAppezza, currentElementi));

            return result.AsReadOnly();
        }

        private static AppezzamentoNutrizioneDto BuildAppezzamentoDto(
            DataRow row,
            IReadOnlyList<ElementoNutrizioneDto> elementi)
        {
            return new AppezzamentoNutrizioneDto
            {
                Piva = row.Field<string>("PIVA")!,
                SaCod = row.Field<int>("SA_COD"),
                Appezza = row.Field<int>("APPEZZA"),
                IdReg = row.Field<int>("ID_REG"),
                ProgettoCod = row.Field<int>("PROGETTO_COD"),
                SaNome = row.Field<string>("Sa_Nome") ?? string.Empty,
                NomeAppezzamento = row.Field<string>("Nome_Appezzamento") ?? string.Empty,
                SpecieVegetale = row.Field<string>("Specie_Vegetale") ?? string.Empty,
                SpecieCod = row.IsNull("Specie_Cod") ? 0 : row.Field<int>("Specie_Cod"),
                Varieta = row.Field<string>("Varieta") ?? string.Empty,
                VarietaCod = row.IsNull("Varieta_Cod") ? 0 : row.Field<int>("Varieta_Cod"),
                CoordinataLat = row.IsNull("Lat") ? null : Convert.ToDouble(row["Lat"]),
                CoordinataLng = row.IsNull("Lng") ? null : Convert.ToDouble(row["Lng"]),
                NumPiante = row.IsNull("Num_Piante") ? 0 : Convert.ToDouble(row["Num_Piante"]),
                Portinnesto = row.Field<string?>("Portinnesto"),
                StatoImpianto = row.Field<string?>("Stato_Impianto"),
                SuperficieHa = row.IsNull("Superficie_Ha") ? 0 : Convert.ToDecimal(row["Superficie_Ha"]),
                DataSeminaPrevista = row.IsNull("Data_Semina_Prevista") ? null: row.Field<DateTime?>("Data_Semina_Prevista")?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                AnalisiTerreno = BuildAnalisiTerrenoDto(row),
                FaseFenologicaCorrente = BuildFaseFenologicaDto(row),
                ConsiglioNutrizioneUltimo = BuildConsiglioDto(row, elementi)
            };
        }

        private static AnalisiTerrenoDto BuildAnalisiTerrenoDto(DataRow row)
        {
            return new AnalisiTerrenoDto
            {
                AnalisiSuperUser = row.IsNull("Analisi_SuperUser") ? string.Empty : row.Field<string>("Analisi_SuperUser")!,
                AnalisiTestataCod = row.IsNull("Analisi_Testata_Cod") ? 0 : row.Field<int>("Analisi_Testata_Cod"),
                AnalisiDettaglioCod = 0,
                AnalisiParametroCod = 0,
                SabbiaPercentuale = row.IsNull("Sabbia_Percentuale") ? null : Convert.ToDouble(row["Sabbia_Percentuale"]),
                LimoPercentuale = row.IsNull("Limo_Percentuale") ? null : Convert.ToDouble(row["Limo_Percentuale"]),
                ArgillaPercentuale = row.IsNull("Argilla_Percentuale") ? null : Convert.ToDouble(row["Argilla_Percentuale"]),
                NTotale = row.IsNull("N_Totale") ? null : Convert.ToDouble(row["N_Totale"]),
                DataAnalisi = row.IsNull("Data_Analisi") ? null : row.Field<DateTime?>("Data_Analisi")
            };
        }

        private static FaseFenologicaCorrenteDto BuildFaseFenologicaDto(DataRow row)
        {
            return new FaseFenologicaCorrenteDto
            {
                IdAgenda = row.IsNull("Id_Agenda") ? 0 : row.Field<int>("Id_Agenda"),
                IdMov = row.IsNull("Id_Mov") ? 0 : row.Field<int>("Id_Mov"),
                IdMovDet = row.IsNull("Id_Mov_Det") ? 0 : row.Field<int>("Id_Mov_Det"),
                BbchCod = row.Field<string?>("BBCH_Cod"),
                BbchDescrizione = row.Field<string?>("BBCH_Descrizione"),
                DataFase = row.IsNull("Data_Fase")
                    ? null
                    : row.Field<DateTime?>("Data_Fase")?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };
        }

        private static ConsiglioNutrizioneUltimoDto BuildConsiglioDto(
            DataRow row,
            IReadOnlyList<ElementoNutrizioneDto> elementi)
        {
            return new ConsiglioNutrizioneUltimoDto
            {
                ConsiglioId = row.IsNull("Consiglio_ID") ? null : row.Field<int?>("Consiglio_ID"),
                DataConsiglio = row.IsNull("Data_Consiglio")
                    ? null
                    : row.Field<DateTime?>("Data_Consiglio")?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Elementi = elementi
            };
        }

        private static ElementoNutrizioneDto BuildElementoDto(DataRow row)
        {
            return new ElementoNutrizioneDto
            {
                Elemento = row.Field<string>("Elemento")!,
                FabbisognoMinimo = row.IsNull("Fabbisogno_Minimo") ? 0d : Convert.ToDouble(row["Fabbisogno_Minimo"]),
                FabbisognoMassimo = row.IsNull("Fabbisogno_Massimo") ? 0d : Convert.ToDouble(row["Fabbisogno_Massimo"]),
                DoseConsigliataMinima = row.IsNull("Dose_Consigliata_Minima") ? 0d : Convert.ToDouble(row["Dose_Consigliata_Minima"]),
                DoseConsigliataMassima = row.IsNull("Dose_Consigliata_Massima") ? 0d : Convert.ToDouble(row["Dose_Consigliata_Massima"]),
                QuantitativoPresente = row.IsNull("Quantitativo_Presente") ? 0d : Convert.ToDouble(row["Quantitativo_Presente"]),
                QuantitativoMinimoResiduo = row.IsNull("Quantitativo_Minimo_Residuo") ? 0d : Convert.ToDouble(row["Quantitativo_Minimo_Residuo"]),
                QuantitativoMassimoResiduo = row.IsNull("Quantitativo_Massimo_Residuo") ? 0d : Convert.ToDouble(row["Quantitativo_Massimo_Residuo"])
            };
        }

        // ── DS12-API: POST /v1/dss/nutrizione/fasi-fenologiche/acquisisci ────

        /// <summary>
        /// Acquisisce le fasi fenologiche per un singolo impianto dall'engine esterno e le sincronizza
        /// nel QDCA (Quaderno di Campagna), evitando duplicati.
        /// Invoca <c>OrchestrationAcquisizioneFasiFenologicheService.EseguiAsync</c> che coordina
        /// la chiamata all'engine e la persistenza.
        /// </summary>
        /// <param name="request">Dati identificativi dell'impianto e parametri geografici/colturali.</param>
        /// <param name="objParametriServer">Parametri server GIAS per accesso a DB e configurazione.</param>
        /// <param name="objParametriSuperServer">Parametri super-server GIAS per accesso alle chiavi engine.</param>
        /// <param name="cancellationToken">Token di cancellazione.</param>
        /// <returns>Risultato con esito, contatori e dettaglio per fase.</returns>
        /// <remarks>
        /// Design Specification: DS12-API POST /v1/dss/nutrizione/fasi-fenologiche/acquisisci.
        /// DS01-BL Acquisizione Fasi Fenologiche da Engine Nutrizione.
        /// DS02-BL Sincronizzazione Fasi Fenologiche QDCA.
        /// </remarks>

        private async Task<AcquisizioneFasiFenologicheWidgetResult> AcquisisciFasiFenologicheAsync(
            AcquisizioneFasiFenologicheRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            // Fail fast: parametri identificativi dell'impianto obbligatori.
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            try
            {
                // Phase 1.2: Invocazione del servizio di orchestrazione.
                // Riferimento: DS12-API — Phase 1 step 2.
                var orchestrationResult = await _orchestrationFasiFenologiche.EseguiAsync(
                    request,
                    objParametriServer.UtenteUsername,
                    objParametriServer,
                    objParametriSuperServer,
                    cancellationToken);

                // Phase 1.3: Valutazione esito orchestrazione.
                // Riferimento: DS12-API — Phase 1 step 3.
                string acquisizioneEsito;
                if (orchestrationResult.Esito)
                {
                    acquisizioneEsito = "SUCCESS";
                    LogInformation(
                        $"Fasi fenologiche acquisite con successo. Impianto={request.Impianto.Piva}/{request.Impianto.SaCod}/{request.Impianto.Appezza}/{request.Impianto.IdReg} FasiAcquisite={orchestrationResult.FasiFenologiche.Count}.",
                        objParametriServer);
                }
                else
                {
                    // La persistenza parziale (PARTIAL) non è distinguibile a questo livello;
                    // errori di configurazione/engine vengono trattati come ERROR.
                    acquisizioneEsito = "ERROR";
                    LogWarning(
                        $"Acquisizione fasi fenologiche completata con errori. Impianto={request.Impianto.Piva}/{request.Impianto.SaCod}/{request.Impianto.Appezza}/{request.Impianto.IdReg} Codice={orchestrationResult.Errore?.Codice} Messaggio={orchestrationResult.Errore?.Messaggio}.",
                        objParametriServer);
                }

                // Phase 2: Confronto QDCA per evitare fasi duplicate (DS02-BL).
                // Riferimento: DS12-API — Phase 2; DS02-BL ConfrontiFasiFenologicheQDCA.
                int fasiAcquisite = orchestrationResult.FasiFenologiche.Count;
                int fasiSincronizzate = orchestrationResult.Esito ? fasiAcquisite : 0;
                int fasiDuplicateEvitate = 0;
                IReadOnlyList<AgronicaCoreModelsSTD.attivita.Attivita> listaAttivita = Array.Empty<AgronicaCoreModelsSTD.attivita.Attivita>();

                // Dizionario indicizzato per BBCH per impostare StatoSincronizzazione per fase.
                // Default: tutte SINCRONIZZATE o ERROR in base all'esito orchestrazione.
                var statoPerfase = orchestrationResult.FasiFenologiche
                    .ToDictionary(
                        f => f.CodiceBbch,
                        _ => orchestrationResult.Esito ? "SINCRONIZZATA" : "ERROR",
                        StringComparer.OrdinalIgnoreCase);

                // DS02-BL: confronto QDCA attivo solo se progettoCod è valorizzato e
                // l'orchestrazione è andata a buon fine.
                if (orchestrationResult.Esito
                    && request.Impianto.ProgettoCod > 0)
                {
                    var confrontoInput = new ConfrontiFasiFenologicheQDCAInput
                    {
                        Piva = request.Impianto.Piva,
                        SaCod = request.Impianto.SaCod,
                        Appezza = request.Impianto.Appezza,
                        IdReg = request.Impianto.IdReg,
                        ProgettoCod = request.Impianto.ProgettoCod,
                        VegCod = int.TryParse(request.ColturaId, out int value) ? value : 0,
                        SuperficieImpianto = request.Impianto.SuperficieImpianto,
                        ElencoFasi = orchestrationResult.FasiFenologiche
                            .Select(f => new FaseFenologicaQDCAInput
                            {
                                BbchCod = f.CodiceBbch,
                                BbchDescrizione = f.DescrizioneFase,
                                DataFase = f.DataStimataRaggiungimento
                            })
                            .ToList()
                            .AsReadOnly()
                    };

                    var confrontoResult = await _confrontiFasiFenologiche.ConfrontaAsync(
                        confrontoInput, objParametriServer, cancellationToken);

                    fasiSincronizzate = confrontoResult.FasiNuove.Count;
                    fasiDuplicateEvitate = confrontoResult.FasiDuplicate.Count;
                    listaAttivita = confrontoResult.ListaAttivita;

                    // Aggiorna lo stato per fase in base al risultato del confronto.
                    foreach (var faseDuplicata in confrontoResult.FasiDuplicate)
                        statoPerfase[faseDuplicata.BbchCod] = "DUPLICATE";
                }

                // Phase 3: Costruzione della risposta API.
                // Riferimento: DS12-API — Phase 2.1 Transform Orchestration Result.
                var fasiDettaglio = orchestrationResult.FasiFenologiche
                    .Select(f => new FaseFenologicaAcquisitaDto
                    {
                        BbchCod = f.CodiceBbch,
                        BbchDescrizione = f.DescrizioneFase,
                        DataFase = f.DataStimataRaggiungimento.ToString("O"),
                        StatoSincronizzazione = statoPerfase.TryGetValue(f.CodiceBbch, out var stato) ? stato : "ERROR"
                    })
                    .ToList();

                return new AcquisizioneFasiFenologicheWidgetResult
                {
                    AcquisizioneEsito = acquisizioneEsito,
                    FasiAcquisite = fasiAcquisite,
                    FasiSincronizzate = fasiSincronizzate,
                    FasiDuplicateEvitate = fasiDuplicateEvitate,
                    Dettagli = new DettagliFasiFenologicheDto { Fasi = fasiDettaglio.AsReadOnly() },
                    TimestampAcquisizione = orchestrationResult.TimestampAcquisizione.ToString("O"),
                    ListaAttivita = listaAttivita
                };
            }
            catch (Exception ex)
            {
                LogError(
                    $"Errore durante l'acquisizione fasi fenologiche. Impianto={request.Impianto.Piva}/{request.Impianto.SaCod}/{request.Impianto.Appezza}/{request.Impianto.IdReg}.",
                    objParametriServer,
                    ex);
                throw;
            }
        }

        // ── DS05B-API: POST /v1/nutrizione/appezzamenti/consigli/aggregato ─────────
        public async Task<AggregazioneConsiglioNutrizioneResult> AggregaConsiglioNutrizioneAsync(
            AppezzamentoNutrizioneDto appezzamento,
            bool salvaConsiglioNutrizione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            int idDbServer,
            CancellationToken cancellationToken = default)
        {
            if (appezzamento is null)   throw new ArgumentNullException(nameof(appezzamento));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            var requestId = Guid.NewGuid().ToString();
            var timestamp = DateTime.UtcNow.ToString("O");

            // Engine timeout: 10 s per engine come da DS05B spec.
            using var engineTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            engineTimeoutCts.CancelAfter(TimeSpan.FromSeconds(10));
            var engineToken = engineTimeoutCts.Token;

            var erroriParziali = new List<ErroreParzialeBIZDto>();
            var swTotal = Stopwatch.StartNew();

            // ── Engine Fenologia ──────────────────────────────────────────────────
            var swFenologia = Stopwatch.StartNew();
            var fenologiaRequest = new AcquisizioneFasiFenologicheRequest
            {
                Impianto = new ImpiantoInput
                {
                    Piva        = appezzamento.Piva,
                    SaCod       = appezzamento.SaCod,
                    Appezza     = appezzamento.Appezza,
                    IdReg       = appezzamento.IdReg,
                    ProgettoCod = appezzamento.ProgettoCod,
                    SuperficieImpianto = appezzamento.SuperficieHa
                },
                TipoRichiesta     = "lungo_termine",
                TipoCodiceColtura = 1,
                ColturaId         = appezzamento.SpecieCod.ToString(),
                VarietaId         = appezzamento.VarietaCod.ToString(),
                Latitudine        = (decimal)(appezzamento.CoordinataLat ?? 0),
                Longitudine       = (decimal)(appezzamento.CoordinataLng ?? 0),
                DataRichiesta     = appezzamento.FaseFenologicaCorrente?.DataFase ?? DateTime.UtcNow.ToString("O"),
                DataSemina        = appezzamento.DataSeminaPrevista ?? DateTime.UtcNow.ToString("O"),
                CodiceLingua      = "it"
            };
            AcquisizioneFasiFenologicheWidgetResult? fenologiaResult = null;
            Exception? fenologiaException = null;
            try
            {
                fenologiaResult = await AcquisisciFasiFenologicheAsync(fenologiaRequest, objParametriServer, objParametriSuperServer, engineToken);
            }
            catch (Exception ex)
            {
                fenologiaException = ex;
            }
            swFenologia.Stop();

            // ── Engine Modelli ────────────────────────────────────────────────────
            // Modelli deve completarsi prima di Nutrizione perché il ModelCode è input obbligatorio.
            var swModelli = Stopwatch.StartNew();
            var coppie = new List<CoppiaSpecieVarieta>
            {
                new() { Veg_Cod = appezzamento.SpecieCod, Cul_Cod = appezzamento.VarietaCod }
            };
            VerificaModelliNutrizioneResult? modelliResult = null;
            Exception? modelliException = null;
            try
            {
                modelliResult = await _verificaModelliService.VerificaAsync(
                    coppie, objParametriServer, objParametriSuperServer, engineToken);
            }
            catch (Exception ex)
            {
                modelliException = ex;
            }
            swModelli.Stop();

            // ── Engine Nutrizione ─────────────────────────────────────────────────
            // Il ModelCode è estratto dal risultato dei modelli prima di invocare l'engine nutrizione.
            var swNutrizione = Stopwatch.StartNew();
            string modelCode = modelliResult?.ModelliPerSpecieVarieta
                .FirstOrDefault(m => m.Status == StatoModelloNutrizione.Available)
                ?.Modelli.FirstOrDefault()?.Code ?? string.Empty;

            var dataConsiglio = DateTime.UtcNow;

            RichiestaConsiglioNutrizioneResult? nutrizioneResult = null;
            Exception? nutrizioneException = null;
            if (string.IsNullOrEmpty(modelCode))
            {
                LogWarning(
                    $"ModelCode non disponibile dall'Engine Modelli. Engine Nutrizione saltato. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}.",
                    objParametriServer);
                nutrizioneException = new InvalidOperationException("Nessun modello nutrizionale disponibile per la specie/varietà selezionata. Engine Nutrizione non invocato.");
            }
            else
            {
                // ── Fertilizzazioni precedenti ────────────────────────────────────────
                // DS04-BL: quelle applicate DOPO la data dell'ultima analisi terreno
                // E PRIMA della data_consiglio. Richiede DataAnalisi valorizzata.
                IReadOnlyList<DatiFertilizzazionePrecedenteInput> fertilizzazioniPrecedenti =
                    Array.Empty<DatiFertilizzazionePrecedenteInput>();

                if (appezzamento.AnalisiTerreno?.DataAnalisi is not null)
                {
                    try
                    {
                        var dt = await _fertilizzazioniNutrizioneDAL
                            .LeggiMacroelementiDistribuitiAsync(
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
                                appezzamento.AnalisiTerreno.DataAnalisi.Value,
                                dataConsiglio,
                                objParametriServer);

                        var _rows = new List<DatiFertilizzazionePrecedenteInput>(dt.Rows.Count);
                        foreach (DataRow row in dt.Rows)
                        {
                            _rows.Add(new DatiFertilizzazionePrecedenteInput
                            {
                                IdAgenda = row.Field<int>("Id_Agenda"),
                                IdMov = row.Field<int>("Id_Mov"),
                                IdMovDet = row.Field<int>("Id_Mov_Det"),
                                Elemento = row.Field<string>("Elemento")!,
                                Quantitativo = Convert.ToDouble(row["Quantitativo"]),
                                FaseFenologicaBbch = row.Field<string>("Fase_BBCH") ?? string.Empty,
                                Data = row.Field<DateTime>("Data_Movimento")
                            });
                        }
                        fertilizzazioniPrecedenti = _rows.AsReadOnly();
                    }
                    catch (Exception ex)
                    {
                        // Fallimento isolato: le fertilizzazioni non bloccano il flusso principale.
                        LogWarning(
                            $"Errore durante il caricamento fertilizzazioni precedenti. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}. {ex.GetBaseException().Message}",
                            objParametriServer);
                    }
                }

                var richiestaConsiglioInput = new RichiestaConsiglioNutrizioneInput
                {
                    DataConsiglio = dataConsiglio,
                    DataSemina = DateTime.TryParse(appezzamento.DataSeminaPrevista, out var dataSemina) ? dataSemina : CostantiPersonalizzate.AGRODATAINIZIO_DATE,
                    DatiImpianto  = new DatiImpiantoInput
                    {
                        Piva           = appezzamento.Piva,
                        SaCod          = appezzamento.SaCod,
                        Appezza        = appezzamento.Appezza,
                        IdReg          = appezzamento.IdReg,
                        VegCod         = appezzamento.SpecieCod,
                        CulCod         = appezzamento.VarietaCod,
                        NumPiante      = (int?)appezzamento.NumPiante,
                        PortinnestoCod = appezzamento.Portinnesto,
                        StatoImpianto  = appezzamento.StatoImpianto,
                        GeometriaWkt   = (appezzamento.CoordinataLat.HasValue && appezzamento.CoordinataLng.HasValue)
                            ? $"POINT({appezzamento.CoordinataLng.Value.ToString(new CultureInfo("en-US"))} {appezzamento.CoordinataLat.Value.ToString(new CultureInfo("en-US"))})"
                            : null,
                        GeometriaSrid  = (appezzamento.CoordinataLat.HasValue && appezzamento.CoordinataLng.HasValue) ? "EPSG:4326" : null
                    },
                    DatiFaseFenologica = appezzamento.FaseFenologicaCorrente is null ? null : new DatiFaseFenologicaInput
                    {
                        IdAgenda       = appezzamento.FaseFenologicaCorrente.IdAgenda,
                        IdMov          = appezzamento.FaseFenologicaCorrente.IdMov,
                        IdMovDet       = appezzamento.FaseFenologicaCorrente.IdMovDet,
                        CodiceBbch        = appezzamento.FaseFenologicaCorrente.BbchCod is null ? "": appezzamento.FaseFenologicaCorrente.BbchCod
                    },
                    DatiAnalisiTerreno = appezzamento.AnalisiTerreno is null || appezzamento.AnalisiTerreno.DataAnalisi is null ? null : new DatiAnalisiTerrenoInput
                    {
                        AnalisiSuperUser = appezzamento.AnalisiTerreno.AnalisiSuperUser ?? string.Empty,
                        AnalisiTestataCod = appezzamento.AnalisiTerreno.AnalisiTestataCod,
                        AnalisiDettaglioCod = appezzamento.AnalisiTerreno.AnalisiDettaglioCod,
                        AnalisiParametroCod = appezzamento.AnalisiTerreno.AnalisiParametroCod,
                        Data = (DateTime) appezzamento.AnalisiTerreno.DataAnalisi,
                        Sabbia  = (int)(appezzamento.AnalisiTerreno.SabbiaPercentuale  ?? 0),
                        Limo    = (int)(appezzamento.AnalisiTerreno.LimoPercentuale    ?? 0),
                        Argilla = (int)(appezzamento.AnalisiTerreno.ArgillaPercentuale ?? 0),
                        ElencoElementiRilevati = new List<Engine.ConsiglioNutrizione.Models.ElementoRilevatoInput>{ new Engine.ConsiglioNutrizione.Models.ElementoRilevatoInput{
                            Elemento = "N",
                            Quantitativo = appezzamento.AnalisiTerreno.NTotale ?? 0
                        }}
                    },
                    DatiFertilizzazioniPrecedenti = fertilizzazioniPrecedenti,
                    ModelCode = modelCode
                };
                try
                {
                    nutrizioneResult = await _richiestaConsiglioService.EseguiAsync(
                        richiestaConsiglioInput,
                        objParametriServer.UtenteUsername,
                        objParametriServer,
                        objParametriSuperServer,
                        idDbServer,
                        salvaConsiglioNutrizione,
                        raccoglitoreCod: 0,
                        engineToken);
                }
                catch (Exception ex)
                {
                    nutrizioneException = ex;
                }
            }
            swNutrizione.Stop();

            // ── Risultato Fenologia ───────────────────────────────────────────────
            EngineFenologiaAggregatoDto engineFenologia;
            if (fenologiaResult != null)
            {
                var isOk = string.Equals(fenologiaResult.AcquisizioneEsito, "SUCCESS", StringComparison.OrdinalIgnoreCase);
                engineFenologia = new EngineFenologiaAggregatoDto
                {
                    Status = isOk ? "success" : "error",
                    ResponseCode = isOk ? 200 : 500,
                    FasiAcquisite = fenologiaResult.FasiAcquisite,
                    UltimaFaseSincronizzata = fenologiaResult.Dettagli.Fasi.Count > 0
                        ? fenologiaResult.Dettagli.Fasi[fenologiaResult.Dettagli.Fasi.Count - 1].DataFase : null,
                    ListaAttivita = fenologiaResult.ListaAttivita,
                    SalvataggioQDCACompletato = false
                };
                if (!isOk)
                    erroriParziali.Add(new ErroreParzialeBIZDto
                    {
                        Engine = "engineFenologia", CodiceErrore = "FENOLOGIA_ERROR",
                        Messaggio = fenologiaResult.AcquisizioneEsito
                    });
            }
            else
            {
                var isTimeout = fenologiaException is OperationCanceledException;
                engineFenologia = new EngineFenologiaAggregatoDto
                {
                    Status       = isTimeout ? "timeout" : "error",
                    ResponseCode = isTimeout ? 504 : 500,
                    Message      = fenologiaException?.GetBaseException().Message
                };
                erroriParziali.Add(new ErroreParzialeBIZDto
                {
                    Engine       = "engineFenologia",
                    CodiceErrore = isTimeout ? "ENGINE_TIMEOUT" : "ENGINE_ERROR",
                    Messaggio    = engineFenologia.Message ?? string.Empty,
                    Suggestion   = isTimeout ? "Riprovare dopo alcuni secondi." : null
                });
                LogWarning(
                    $"Engine fenologia {(isTimeout ? "timeout" : "error")}. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}.",
                    objParametriServer);
            }

            // ── Risultato Modelli ─────────────────────────────────────────────────
            EngineModelliAggregatoDto engineModelli;
            if (modelliResult != null)
            {
                engineModelli = new EngineModelliAggregatoDto
                {
                    Status          = "success",
                    ResponseCode    = 200,
                    ModelliValidati = modelliResult.ModelliPerSpecieVarieta.Any(m => m.Status == StatoModelloNutrizione.Available),
                    Dettagli        = modelliResult.TimestampVerifica
                };
            }
            else
            {
                var isTimeout = modelliException is OperationCanceledException;
                engineModelli = new EngineModelliAggregatoDto
                {
                    Status       = isTimeout ? "timeout" : "error",
                    ResponseCode = isTimeout ? 504 : 500,
                    Message      = modelliException?.Message
                };
                erroriParziali.Add(new ErroreParzialeBIZDto
                {
                    Engine       = "engineModelli",
                    CodiceErrore = isTimeout ? "ENGINE_TIMEOUT" : "ENGINE_ERROR",
                    Messaggio    = engineModelli.Message ?? string.Empty,
                    Suggestion   = isTimeout ? "Riprovare dopo alcuni secondi." : null
                });
                LogWarning(
                    $"Engine modelli {(isTimeout ? "timeout" : "error")}. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}.",
                    objParametriServer);
            }

            // ── Risultato Nutrizione ──────────────────────────────────────────────
            EngineNutrizioneAggregatoDto engineNutrizione;
            ConsiglioNutrizioneAggregatoDto consiglioNutrizione;
            if (nutrizioneResult != null)
            {
                var isOk = string.Equals(nutrizioneResult.Status, "DONE", StringComparison.OrdinalIgnoreCase);
                engineNutrizione = new EngineNutrizioneAggregatoDto
                {
                    Status                = isOk ? "success" : "error",
                    ResponseCode          = isOk ? 200 : 500,
                    Message               = nutrizioneResult.Messaggi,
                    ConsiglioIdPersistito = nutrizioneResult.ConsiglioId,
                    TimestampEngine       = DateTime.UtcNow.ToString("O")
                };
                if (!isOk)
                    erroriParziali.Add(new ErroreParzialeBIZDto
                    {
                        Engine = "engineNutrizione", CodiceErrore = "NUTRIZIONE_ERROR",
                        Messaggio = nutrizioneResult.Messaggi ?? string.Empty
                    });

                consiglioNutrizione = new ConsiglioNutrizioneAggregatoDto
                {
                    ConsiglioId   = nutrizioneResult.ConsiglioId,
                    DataConsiglio = dataConsiglio.ToString("O"),
                    Elementi      = (nutrizioneResult.ElementiConsigliati ?? Array.Empty<ElementoConsigliatoDto>())
                        .Select(e => new ElementoNutrizioneAggregatoDto
                        {
                            Elemento               = e.Elemento,
                            FabbisognoMinimo       = e.FabbisognoMinimo,
                            FabbisognoMassimo      = e.FabbisognoMassimo,
                            DoseConsigliataMinima  = e.DoseConsigliataMiniima,
                            DoseConsigliataMassima = e.DoseConsigliataMassima,
                            QuantitativoPresente       = e.QuantitativoPresente,
                            QuantitativoMinimoResiduo  = e.QuantitativoMinimoResiduo,
                            QuantitativoMassimoResiduo = e.QuantitativoMassimoResiduo,
                            Status  = isOk ? "success" : "error",
                            Message = nutrizioneResult.Messaggi
                        })
                        .ToList()
                        .AsReadOnly()
                };
            }
            else
            {
                var isTimeout = nutrizioneException is OperationCanceledException;
                engineNutrizione = new EngineNutrizioneAggregatoDto
                {
                    Status       = isTimeout ? "timeout" : "error",
                    ResponseCode = isTimeout ? 504 : 500,
                    Message      = nutrizioneException?.GetBaseException().Message
                };
                erroriParziali.Add(new ErroreParzialeBIZDto
                {
                    Engine       = "engineNutrizione",
                    CodiceErrore = isTimeout ? "ENGINE_TIMEOUT" : "ENGINE_ERROR",
                    Messaggio    = engineNutrizione.Message ?? string.Empty,
                    Suggestion   = isTimeout ? "Riprovare dopo alcuni secondi." : null
                });
                LogWarning(
                    $"Engine nutrizione {(isTimeout ? "timeout" : "error")}. Impianto={appezzamento.Piva}/{appezzamento.SaCod}/{appezzamento.Appezza}/{appezzamento.IdReg}.",
                    objParametriServer);
                // Fallback: restituisce l'ultimo consiglio noto (già persistito in DB).
                consiglioNutrizione = new ConsiglioNutrizioneAggregatoDto
                {
                    ConsiglioId   = appezzamento.ConsiglioNutrizioneUltimo?.ConsiglioId,
                    DataConsiglio = appezzamento.ConsiglioNutrizioneUltimo?.DataConsiglio,
                    Elementi      = Array.Empty<ElementoNutrizioneAggregatoDto>()
                };
            }

            swTotal.Stop();

            var aggregazioneStatus = erroriParziali.Count == 0
                ? "full_success"
                : nutrizioneResult != null
                    ? "partial_success"
                    : "failed";

            return new AggregazioneConsiglioNutrizioneResult
            {
                RequestId  = requestId,
                Timestamp  = timestamp,
                Appezzamento = appezzamento,
                ConsiglioNutrizione = consiglioNutrizione,
                RisposteEngine = new RisposteEngineAggregatoDto
                {
                    EngineNutrizione = engineNutrizione,
                    EngineFenologia  = engineFenologia,
                    EngineModelli    = engineModelli
                },
                AggregazioneStatus = aggregazioneStatus,
                ErroriParziali     = erroriParziali.AsReadOnly(),
                Metriche = new MetricheAggregazioneDto
                {
                    TempoTotaleMs           = swTotal.ElapsedMilliseconds,
                    TempoEngineNutrizioneMs = swNutrizione.ElapsedMilliseconds,
                    TempoEngineFenologiaMs  = swFenologia.ElapsedMilliseconds,
                    TempoEngineModelliMs    = swModelli.ElapsedMilliseconds
                }
            };
        }

    }
}
