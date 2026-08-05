using AgronicaCoreModelsSTD.attivita;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models
{
    // ── Existing models (DS05-BL / DS11-API) ────────────────────────────────
    /// <summary>
    /// Dati dell'analisi del terreno per un appezzamento.
    /// Riferimento: DS05-BL - Output analisi_terreno; DS11-API risposta 200.
    /// </summary>
    public sealed class AnalisiTerrenoDto
    {
        public string AnalisiSuperUser { get; init; } = string.Empty;
        public int AnalisiTestataCod { get; init; }
        public int AnalisiDettaglioCod { get; init; }
        public int AnalisiParametroCod { get; init; }
        public double? SabbiaPercentuale { get; init; }
        public double? LimoPercentuale { get; init; }
        public double? ArgillaPercentuale { get; init; }
        public double? NTotale { get; init; }
        /// <summary>Data inizio analisi (Analisi_Testata_Data_Inizio). Usata come limite inferiore per le fertilizzazioni precedenti.</summary>
        public DateTime? DataAnalisi { get; init; }
    }

    /// <summary>
    /// Fase fenologica corrente rilevata per l'impianto.
    /// Riferimento: DS05-BL - Output fase_fenologica_corrente; DS11-API risposta 200.
    /// </summary>
    public sealed class FaseFenologicaCorrenteDto
    {
        public int IdAgenda { get; init; }
        public int IdMov { get; init; }
        public int IdMovDet { get; init; }
        public string? BbchCod { get; init; }
        public string? BbchDescrizione { get; init; }
        /// <summary>Data fase in formato ISO 8601, null se non disponibile.</summary>
        public string? DataFase { get; init; }
    }

    /// <summary>
    /// Singolo elemento nutrizionale incluso nel consiglio.
    /// Riferimento: DS05-BL - Output consiglio_nutrizione_ultimo.elementi; DS11-API risposta 200.
    /// </summary>
    public sealed class ElementoNutrizioneDto
    {
        public string Elemento { get; init; } = string.Empty;
        public double FabbisognoMinimo { get; init; }
        public double FabbisognoMassimo { get; init; }
        public double DoseConsigliataMinima { get; init; }
        public double DoseConsigliataMassima { get; init; }
        public double QuantitativoPresente { get; init; }
        public double QuantitativoMinimoResiduo { get; init; }
        public double QuantitativoMassimoResiduo { get; init; }
    }

    /// <summary>
    /// Ultimo consiglio nutrizionale disponibile per l'appezzamento.
    /// Riferimento: DS05-BL - Output consiglio_nutrizione_ultimo; DS11-API risposta 200.
    /// </summary>
    public sealed class ConsiglioNutrizioneUltimoDto
    {
        public int? ConsiglioId { get; init; }
        /// <summary>Data consiglio in formato ISO 8601, null se non disponibile.</summary>
        public string? DataConsiglio { get; init; }
        public IReadOnlyList<ElementoNutrizioneDto> Elementi { get; init; } = Array.Empty<ElementoNutrizioneDto>();
    }

    /// <summary>
    /// Dati completi per il widget di un singolo appezzamento nella pagina DSS Nutrizione.
    /// Riferimento: DS05-BL - Output appezzamenti; DS11-API risposta 200 (singolo item).
    /// </summary>
    public sealed class AppezzamentoNutrizioneDto
    {
        public string Piva { get; init; } = string.Empty;
        public int SaCod { get; init; }
        public int Appezza { get; init; }
        public int IdReg { get; init; }
        public int ProgettoCod { get; init; }
        public string SaNome { get; init; } = string.Empty;
        public string NomeAppezzamento { get; init; } = string.Empty;
        public string SpecieVegetale { get; init; } = string.Empty;
        public int SpecieCod { get; init; }
        public string Varieta { get; init; }
        public int VarietaCod { get; init; }
        public double? CoordinataLat { get; init; }
        public double? CoordinataLng { get; init; }
        public double NumPiante { get; init; }
        public string? Portinnesto { get; init; }
        public string? StatoImpianto { get; init; }
        public decimal SuperficieHa { get; init; }
        public string? DataSeminaPrevista { get; init; }
        public AnalisiTerrenoDto AnalisiTerreno { get; init; } = new();
        public FaseFenologicaCorrenteDto FaseFenologicaCorrente { get; init; } = new();
        public ConsiglioNutrizioneUltimoDto ConsiglioNutrizioneUltimo { get; init; } = new();
    }

    /// <summary>
    /// Richiesta per l'aggregazione del consiglio nutrizione.
    /// Riferimento: DS05B-API POST /v1/nutrizione/appezzamenti/consigli/aggregato — Request.
    /// </summary>
    public sealed class AggregaConsiglioNutrizioneRequest
    {
        public AppezzamentoNutrizioneDto Appezzamento { get; init; } = new();
        /// <summary>
        /// Se <c>true</c>, il risultato dell'engine Consiglio Nutrizione viene persistito
        /// nella tabella Consigli_Nutrizione_Engine. Se <c>false</c>, il consiglio viene
        /// calcolato ma non salvato.
        /// </summary>
        public bool SalvaConsiglioNutrizione { get; init; }
    }

    /// <summary>
    /// Richiesta per l'estrazione cronologica delle fasi fenologiche registrate su un appezzamento.
    /// Riferimento: DS20-BL GetPhenologicalPhasesChronological — Input.
    /// </summary>
    public sealed class FasiFenologicheRequest
    {
        public AppezzamentoNutrizioneDto Appezzamento { get; init; } = new();
        public DateTime ValiditaInizio { get; init; }
        public DateTime ValiditaFine { get; init; }
    }

    /// <summary>
    /// Risultato paginato del caricamento dati widget nutrizione.
    /// Riferimento: DS05-BL - Output; DS11-API risposta 200.
    /// </summary>
    public sealed class DatiWidgetNutrizioneResult
    {
        public int TotaleAppezzamenti { get; init; }
        public int PaginaAttuale { get; init; }
        public int RecordPerPagina { get; init; }
        public IReadOnlyList<AppezzamentoNutrizioneDto> Appezzamenti { get; init; } = Array.Empty<AppezzamentoNutrizioneDto>();
    }

    /// <summary>
    /// Dettaglio di una singola fase fenologica inclusa nella risposta di acquisizione.
    /// Riferimento: DS12-API — Response Schema dettagli.fasi.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS12-API POST /v1/dss/nutrizione/fasi-fenologiche/acquisisci — Response.
    /// </remarks>
    public sealed class FaseFenologicaAcquisitaDto
    {
        /// <summary>Codice fase BBCH (es. "65").</summary>
        public string BbchCod { get; init; } = string.Empty;
        /// <summary>Descrizione leggibile della fase (es. "Full flowering").</summary>
        public string BbchDescrizione { get; init; } = string.Empty;
        /// <summary>Data della fase in formato ISO-8601.</summary>
        public string DataFase { get; init; } = string.Empty;
        /// <summary>Stato sincronizzazione: SINCRONIZZATA | DUPLICATE | ERROR.</summary>
        public string StatoSincronizzazione { get; init; } = string.Empty;
    }

    /// <summary>
    /// Contenitore dei dettagli per fase incluso nella risposta di acquisizione.
    /// Riferimento: DS12-API — Response Schema dettagli.
    /// </summary>
    public sealed class DettagliFasiFenologicheDto
    {
        public IReadOnlyList<FaseFenologicaAcquisitaDto> Fasi { get; init; } = Array.Empty<FaseFenologicaAcquisitaDto>();
    }

    /// <summary>
    /// Risultato dell'acquisizione e sincronizzazione fasi fenologiche per un impianto.
    /// Mappa l'<c>OrchestrationRisposta</c> nel formato di risposta definito da DS12-API.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS12-API POST /v1/dss/nutrizione/fasi-fenologiche/acquisisci — Response Schema.
    /// DS01-BL Acquisizione Fasi Fenologiche da Engine Nutrizione — Output.
    /// DS02-BL Sincronizzazione Fasi Fenologiche QDCA — Output.
    /// </remarks>
    public sealed class AcquisizioneFasiFenologicheWidgetResult
    {
        /// <summary>Esito acquisizione dall'engine: SUCCESS | PARTIAL | ERROR.</summary>
        public string AcquisizioneEsito { get; init; } = string.Empty;
        /// <summary>Conteggio totale fasi fenologiche ricevute dall'engine.</summary>
        public int FasiAcquisite { get; init; }
        /// <summary>Conteggio fasi sincronizzate con successo nel QDCA.</summary>
        public int FasiSincronizzate { get; init; }
        /// <summary>Conteggio fasi duplicate rilevate e non inserite.</summary>
        public int FasiDuplicateEvitate { get; init; }
        /// <summary>Dettaglio per singola fase fenologica.</summary>
        public DettagliFasiFenologicheDto Dettagli { get; init; } = new();
        /// <summary>Timestamp di esecuzione dell'acquisizione in formato ISO-8601.</summary>
        public string TimestampAcquisizione { get; init; } = string.Empty;
        /// <summary>
        /// Oggetti Attivita pronti per essere persistiti nel QDCA, restituiti da
        /// <c>ConfrontiFasiFenologicheQDCAService.ConfrontaAsync</c>.
        /// Riferimento: DS02-BL — Output lista_attivita.
        /// </summary>
        public IReadOnlyList<Attivita> ListaAttivita { get; init; } = Array.Empty<Attivita>();
    }

    // ── DS05B-API: POST /v1/nutrizione/appezzamenti/consigli/aggregato ──────

    /// <summary>
    /// Singolo elemento nutrizionale con stato elaborazione nell'aggregazione.
    /// Riferimento: DS05B-API — Response 200.consiglio_nutrizione.elementi.
    /// </summary>
    public sealed class ElementoNutrizioneAggregatoDto
    {
        public string Elemento                  { get; init; } = string.Empty;
        public double FabbisognoMinimo           { get; init; }
        public double FabbisognoMassimo          { get; init; }
        public double DoseConsigliataMinima      { get; init; }
        public double DoseConsigliataMassima     { get; init; }
        public double QuantitativoPresente       { get; init; }
        public double QuantitativoMinimoResiduo  { get; init; }
        public double QuantitativoMassimoResiduo { get; init; }
        /// <summary>success | partial | error</summary>
        public string  Status  { get; init; } = string.Empty;
        public string? Message { get; init; }
    }

    /// <summary>
    /// Consiglio nutrizionale aggregato con elementi elaborati dagli engine.
    /// Riferimento: DS05B-API — Response 200.consiglio_nutrizione.
    /// </summary>
    public sealed class ConsiglioNutrizioneAggregatoDto
    {
        public int?    ConsiglioId   { get; init; }
        public string? DataConsiglio { get; init; }
        public IReadOnlyList<ElementoNutrizioneAggregatoDto> Elementi { get; init; }
            = Array.Empty<ElementoNutrizioneAggregatoDto>();
    }

    /// <summary>
    /// Risposta dell'engine nutrizione con stato e consiglio persistito.
    /// Riferimento: DS05B-API — Response 200.risposte_engine.engine_nutrizione.
    /// </summary>
    public sealed class EngineNutrizioneAggregatoDto
    {
        /// <summary>success | error | timeout</summary>
        public string  Status                { get; init; } = string.Empty;
        public int     ResponseCode          { get; init; }
        public string? Message               { get; init; }
        public int?    ConsiglioIdPersistito { get; init; }
        public string? TimestampEngine       { get; init; }
    }

    /// <summary>
    /// Risposta dell'engine fenologia con contatori di acquisizione.
    /// Riferimento: DS05B-API — Response 200.risposte_engine.engine_fenologia.
    /// </summary>
    public sealed class EngineFenologiaAggregatoDto
    {
        /// <summary>success | error | timeout</summary>
        public string  Status                   { get; init; } = string.Empty;
        public int     ResponseCode             { get; init; }
        public string? Message                  { get; init; }
        public int     FasiAcquisite            { get; init; }
        public string? UltimaFaseSincronizzata  { get; init; }
        /// <summary>
        /// Oggetti Attivita pronti per essere persistiti nel QDCA tramite il pipeline
        /// <c>MapAttivitaToAgenda</c> / <c>ScriviAttivitaAgendaAsync</c>.
        /// Riferimento: DS02-BL — Output lista_attivita; Framework.md — Procedura di Salvataggio.
        /// </summary>
        public IReadOnlyList<Attivita> ListaAttivita { get; init; } = Array.Empty<Attivita>();
        /// <summary>
        /// Indica se il salvataggio della lista attivita come operazioni QDCA
        /// è andato a buon fine (DS02B-BL SalvataggioFasiFenologicheQDCA).
        /// Rimane <c>false</c> se la lista è vuota o se si è verificato un errore.
        /// </summary>
        public bool SalvataggioQDCACompletato { get; set; }
    }

    /// <summary>
    /// Risposta dell'engine modelli con esito validazione.
    /// Riferimento: DS05B-API — Response 200.risposte_engine.engine_modelli.
    /// </summary>
    public sealed class EngineModelliAggregatoDto
    {
        /// <summary>success | error | timeout</summary>
        public string  Status          { get; init; } = string.Empty;
        public int     ResponseCode    { get; init; }
        public string? Message         { get; init; }
        public bool    ModelliValidati { get; init; }
        public string? Dettagli        { get; init; }
    }

    /// <summary>
    /// Contenitore delle risposte dei tre engine orchestrati.
    /// Riferimento: DS05B-API — Response 200.risposte_engine.
    /// </summary>
    public sealed class RisposteEngineAggregatoDto
    {
        public EngineNutrizioneAggregatoDto  EngineNutrizione { get; init; } = new();
        public EngineFenologiaAggregatoDto   EngineFenologia  { get; init; } = new();
        public EngineModelliAggregatoDto     EngineModelli    { get; init; } = new();
    }

    /// <summary>
    /// Errore parziale registrato durante l'aggregazione.
    /// Riferimento: DS05B-API — Response 200.errori_parziali.
    /// </summary>
    public sealed class ErroreParzialeBIZDto
    {
        public string  Engine       { get; init; } = string.Empty;
        public string  CodiceErrore { get; init; } = string.Empty;
        public string  Messaggio    { get; init; } = string.Empty;
        public string? Suggestion   { get; init; }
    }

    /// <summary>
    /// Metriche temporali di esecuzione dei singoli engine e totale.
    /// Riferimento: DS05B-API — Response 200.metriche.
    /// </summary>
    public sealed class MetricheAggregazioneDto
    {
        public long TempoTotaleMs           { get; init; }
        public long TempoEngineNutrizioneMs { get; init; }
        public long TempoEngineFenologiaMs  { get; init; }
        public long TempoEngineModelliMs    { get; init; }
    }

    /// <summary>
    /// Risultato aggregato dell'orchestrazione dei tre engine nutrizionali per un appezzamento.
    /// Contiene consiglio nutrizionale, risposte engine, stato aggregazione, errori parziali e metriche.
    /// Riferimento: DS05B-API POST /v1/nutrizione/appezzamenti/consigli/aggregato — Response 200.
    /// </summary>
    public sealed class AggregazioneConsiglioNutrizioneResult
    {
        public string                        RequestId          { get; init; } = string.Empty;
        public string                        Timestamp          { get; init; } = string.Empty;
        public AppezzamentoNutrizioneDto     Appezzamento       { get; init; } = new();
        public ConsiglioNutrizioneAggregatoDto ConsiglioNutrizione { get; init; } = new();
        public RisposteEngineAggregatoDto    RisposteEngine     { get; init; } = new();
        /// <summary>full_success | partial_success | failed</summary>
        public string                        AggregazioneStatus { get; init; } = string.Empty;
        public IReadOnlyList<ErroreParzialeBIZDto> ErroriParziali { get; init; }
            = Array.Empty<ErroreParzialeBIZDto>();
        public MetricheAggregazioneDto       Metriche           { get; init; } = new();
    }

    /// <summary>
    /// Request body per <c>POST /v1/dss/nutrizione/consigli/salva</c>.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS16-API POST /v1/dss/nutrizione/consigli/salva — Formato Richiesta.
    /// </remarks>
    public sealed class SalvataggioConsiglioNutrizioneRequest
    {
        public AggregazioneConsiglioNutrizioneResult AggregazioneConsiglio { get; init; } = new();

        /// <summary>
        /// Se <c>true</c>, verifica la presenza di duplicati prima del salvataggio.
        /// </summary>
        public bool ControllaDuplicati { get; init; }
    }
}

