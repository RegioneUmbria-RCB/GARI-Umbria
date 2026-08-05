using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.Webhook.DAL.DataLayer.Models;
using Newtonsoft.Json;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models
{
    // ──────────────────────────────────────────────────────────────────────────────
    // BIZ input models
    // DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Input
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Input principale per la richiesta del consiglio nutrizionale di un appezzamento.
    /// Riferimento: DS04-BL — Input.
    /// </summary>
    public sealed class RichiestaConsiglioNutrizioneInput
    {
        public DateTime                                    DataConsiglio             { get; init; }
        public DateTime                                    DataSemina                { get; init; }
        public DatiImpiantoInput                           DatiImpianto              { get; init; } = null!;
        public DatiFaseFenologicaInput?                    DatiFaseFenologica        { get; init; }
        public DatiAnalisiTerrenoInput?                    DatiAnalisiTerreno        { get; init; }
        public IReadOnlyList<DatiFertilizzazionePrecedenteInput> DatiFertilizzazioniPrecedenti { get; init; }
            = Array.Empty<DatiFertilizzazionePrecedenteInput>();
        public string ModelCode { get; init; } = string.Empty;
    }

    /// <summary>
    /// Dati dell'impianto (coltura) necessari per la richiesta all'engine.
    /// Riferimento: DS04-BL — Input.dati_impianto.
    /// </summary>
    public sealed class DatiImpiantoInput
    {
        public string  Piva           { get; init; } = string.Empty;
        public int     SaCod          { get; init; }
        public int     Appezza        { get; init; }
        public int     IdReg          { get; init; }
        public int     VegCod         { get; init; }
        public int   CulCod         { get; init; }
        public int?    NumPiante      { get; init; }
        public string? PortinnestoCod { get; init; }
        public string? StatoImpianto  { get; init; }
        public string? GeometriaWkt   { get; init; }
        public string?    GeometriaSrid  { get; init; }
    }

    /// <summary>
    /// Dati della fase fenologica corrente (BBCH), inclusi gli identificativi QDCA.
    /// Riferimento: DS04-BL — Input.dati_fasefenologica.
    /// </summary>
    public sealed class DatiFaseFenologicaInput
    {
        public int    IdAgenda   { get; init; }
        public int    IdMov      { get; init; }
        public int    IdMovDet   { get; init; }
        public string CodiceBbch { get; init; } = string.Empty;
    }

    /// <summary>
    /// Dati dell'ultima analisi del terreno (granulometria e elementi rilevati).
    /// Riferimento: DS04-BL — Input.dati_analisiterreno.
    /// </summary>
    public sealed class DatiAnalisiTerrenoInput
    {
        public string  AnalisiSuperUser    { get; init; } = string.Empty;
        public int     AnalisiTestataCod   { get; init; }
        public int     AnalisiDettaglioCod { get; init; }
        public int     AnalisiParametroCod { get; init; }
        public int     Sabbia              { get; init; }
        public int     Limo               { get; init; }
        public int     Argilla            { get; init; }
        public DateTime Data { get; init; }
        public IReadOnlyList<ElementoRilevatoInput> ElencoElementiRilevati { get; init; }
            = Array.Empty<ElementoRilevatoInput>();
    }

    /// <summary>
    /// Singolo elemento rilevato nell'analisi del terreno.
    /// Riferimento: DS04-BL — Input.dati_analisiterreno.elencoElementiRilevati.
    /// </summary>
    public sealed class ElementoRilevatoInput
    {
        public string Elemento    { get; init; } = string.Empty;
        public double Quantitativo { get; init; }
    }

    /// <summary>
    /// Singola fertilizzazione precedente applicata all'appezzamento.
    /// Riferimento: DS04-BL — Input.dati_fertilizzazioni_precedenti.
    /// </summary>
    public sealed class DatiFertilizzazionePrecedenteInput
    {
        public int    IdAgenda          { get; init; }
        public int    IdMov             { get; init; }
        public int    IdMovDet          { get; init; }
        public double Quantitativo      { get; init; }
        public string FaseFenologicaBbch { get; init; } = string.Empty;
        public DateTime Data            { get; init; }
        public string Elemento          { get; init; } = string.Empty;
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // BIZ output models
    // DS04-BL Richiesta Consiglio Nutrizione Appezzamenti — Output
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Risultato della richiesta del consiglio nutrizionale per un appezzamento.
    /// Riferimento: DS04-BL — Output.
    /// </summary>
    public sealed class RichiestaConsiglioNutrizioneResult
    {
        public int?   ConsiglioId        { get; init; }
        public string Appezzamento       { get; init; } = string.Empty;
        public IReadOnlyList<ElementoConsigliatoDto> ElementiConsigliati { get; init; }
            = Array.Empty<ElementoConsigliatoDto>();
        /// <summary>SUCCESS o ERROR.</summary>
        public string  Status   { get; init; } = string.Empty;
        public string? Messaggi { get; init; }
    }

    /// <summary>
    /// Singolo elemento nutrizionale consigliato dall'engine.
    /// Riferimento: DS04-BL — Output.elementi_consigliati.
    /// </summary>
    public sealed class ElementoConsigliatoDto
    {
        public string Elemento                  { get; init; } = string.Empty;
        public double FabbisognoMinimo           { get; init; }
        public double FabbisognoMassimo          { get; init; }
        public double DoseConsigliataMiniima     { get; init; }
        public double DoseConsigliataMassima     { get; init; }
        public double QuantitativoPresente       { get; init; }
        public double QuantitativoMinimoResiduo  { get; init; }
        public double QuantitativoMassimoResiduo { get; init; }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // Engine API contract models (internal — not exposed outside the service)
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Payload inviato all'engine per la richiesta del consiglio nutrizionale.
    /// Riferimento: Esempio chiamata per ottenere il consiglio di Nutrizione.
    /// </summary>
    internal sealed class EngineConsiglioNutrizioneRequest
    {
        [JsonProperty("dataConsiglio")]
        public string DataConsiglio { get; init; } = string.Empty;

        [JsonProperty("dataSemina")]
        public string DataSemina { get; init; } = string.Empty;

        [JsonProperty("colturaInfo")]
        public EngineColturaNutrizioneInfo ColturaInfo { get; init; } = null!;

        [JsonProperty("faseFenologicaBBCH")]
        public string? FaseFenologicaBbch { get; init; }

        [JsonProperty("datiAnalisiTerreno")]
        public EngineAnalisiTerreno? DatiAnalisiTerreno { get; init; }

        [JsonProperty("elencoFertilizzazioniPrecedenti")]
        public IReadOnlyList<EngineFertilizzazionePrecedente> ElencoFertilizzazioniPrecedenti { get; init; } = Array.Empty<EngineFertilizzazionePrecedente>();
    }

    internal sealed class EngineColturaNutrizioneInfo
    {
        [JsonProperty("specieVegetale")]
        public string SpecieVegetale { get; init; } = string.Empty;

        [JsonProperty("varieta")]
        public string? Varieta { get; init; }

        [JsonProperty("tipoCodifica")]
        public string? TipoCodifica { get; init; }

        [JsonProperty("numeroPiante")]
        public int? NumeroPiante { get; init; }

        [JsonProperty("portinnesto")]
        public string? Portinnesto { get; init; }

        [JsonProperty("statoImpianto")]
        public string? StatoImpianto { get; init; }

        [JsonProperty("GeometriaImpiantoWKT")]
        public string? GeometriaImpiantoWkt { get; init; }

        [JsonProperty("GeometriaImpiantoSRID")]
        public string? GeometriaImpiantoSrid { get; init; }
    }

    internal sealed class EngineAnalisiTerreno
    {
        [JsonProperty("dataAnalisi")]
        public string Data { get; init; } = string.Empty;

        [JsonProperty("sabbia")]
        public int Sabbia { get; init; }

        [JsonProperty("limo")]
        public int Limo { get; init; }

        [JsonProperty("argilla")]
        public int Argilla { get; init; }

        [JsonProperty("elencoElementiRilevati")]
        public IReadOnlyList<EngineElementoRilevato>? ElencoElementiRilevati { get; init; }
    }

    internal sealed class EngineElementoRilevato
    {
        [JsonProperty("elemento")]
        public string Elemento { get; init; } = string.Empty;

        [JsonProperty("quantitativo")]
        public double Quantitativo { get; init; }
    }

    internal sealed class EngineFertilizzazionePrecedente
    {
        [JsonProperty("elemento")]
        public string Elemento { get; init; } = string.Empty;

        [JsonProperty("quantitativo")]
        public double Quantitativo { get; init; }

        [JsonProperty("faseFenologicaBBCH")]
        public string FaseFenologicaBbch { get; init; } = string.Empty;

        [JsonProperty("data")]
        public string Data { get; init; } = string.Empty;
    }

    /// <summary>
    /// Risposta iniziale dell'engine alla richiesta POST (presa in carico asincrona).
    /// Riferimento: Esempio di risposta dell'engine.
    /// </summary>
    internal sealed class EngineConsiglioNutrizioneInitialResponse
    {
        [JsonProperty("requestId")]
        public string RequestId { get; init; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; init; } = string.Empty;

        [JsonProperty("receivedAt")]
        public string ReceivedAt { get; init; } = string.Empty;
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // DS15-BL Polling models
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Input per il polling del WebHook_Testata.
    /// Riferimento: DS15-BL Polling WebHook Consiglio Nutrizione — Input.
    /// </summary>
    public sealed class PollingWebHookInput
    {
        public string RequestId           { get; init; } = string.Empty;
        public int    TestataId           { get; init; }
        public int    TimeoutSecondi      { get; init; } = 5;
        public int    IntervalloPollingMs  { get; init; } = 500;
    }

    /// <summary>
    /// Risultato del polling sul WebHook_Testata.
    /// Riferimento: DS15-BL Polling WebHook Consiglio Nutrizione — Output.
    /// </summary>
    public sealed class PollingWebHookResult
    {
        /// <summary>DONE | ERROR | TIMEOUT | NOT_FOUND</summary>
        public string  Status        { get; init; } = string.Empty;
        public string? ConsiglioId   { get; init; }
        public int     TestataId     { get; init; }
        public EsitoConsiglioNutrizione? Response { get; init; }
        public string  Esito         { get; init; } = string.Empty;
        public string? EsitoDettagli { get; init; }
        public string? Messaggi      { get; init; }
    }
}
