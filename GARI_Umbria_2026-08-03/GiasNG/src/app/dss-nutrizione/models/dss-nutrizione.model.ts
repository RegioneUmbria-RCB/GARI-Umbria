/**
 * Domain models for DSS Nutrizione feature.
 * Spec reference: UIDS001 FR002
 */

// ─────────────────────────────────────────────────────────────────────────────
// § REQUEST MODELS
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Request body sent to POST /DSSNutrizione/widget/dati
 * to retrieve the list of active appezzamenti for a company.
 * DS05-BL §Input – CaricamentoDatiWidgetNutrizione
 * FR002 §1 – Lista Appezzamenti Attivi per Azienda
 */
export interface AppezzamentiListaRequest {
  piva: string;
  /** If 0, all centres authorised for the user via Utenti_Visibilita_Appoggio are considered. */
  sa_cod: number;
  /** Defaults to current calendar year. */
  anno_solare: number;
}

// ─────────────────────────────────────────────────────────────────────────────
// § EMBEDDED DTO MODELS (shared between list response & aggregato request)
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Soil analysis data embedded in an appezzamento.
 * DS05-BL §Output – AnalisiTerrenoDto
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento
 */
export interface AnalisiTerreno {
  AnalisiSuperUser: string;
  AnalisiTestataCod: number;
  AnalisiDettaglioCod: number;
  AnalisiParametroCod: number;
  SabbiaPercentuale: number | null;
  LimoPercentuale: number | null;
  ArgillaPercentuale: number | null;
  NTotale: number | null;
}

/**
 * Current phenological phase of an appezzamento.
 * DS05-BL §Output – FaseFenologicaCorrenteDto
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento
 */
export interface FaseFenologicaCorrente {
  IdAgenda: number;
  IdMov: number;
  IdMovDet: number;
  BbchCod: string | null;
  BbchDescrizione: string | null;
  DataFase: string | null;
}

/**
 * A single nutrient element embedded in the last stored consiglio.
 * DS05-BL §Output – ElementoNutrizioneDto
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento
 */
export interface ElementoNutriente {
  Elemento: string;
  FabbisognoMinimo: number;
  FabbisognoMassimo: number;
  DoseConsigliataMinima: number;
  DoseConsigliataMassima: number;
  QuantitativoPresente: number;
  QuantitativoMinimoResiduo: number;
  QuantitativoMassimoResiduo: number;
}

/**
 * Last known nutrition advice embedded in the appezzamento data from the list endpoint.
 * DS05-BL §Output – ConsiglioNutrizioneUltimoDto
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento
 */
export interface ConsiglioNutrizioneUltimo {
  ConsiglioId: number | null;
  DataConsiglio: string | null;
  Elementi: ElementoNutriente[];
}

/**
 * Represents a single appezzamento as returned by POST /DSSNutrizione/widget/dati
 * and also used verbatim as the request body for POST /v1/nutrizione/appezzamenti/consigli/aggregato.
 * DS05-BL §Output – AppezzamentoNutrizioneDto
 * FR002 §1 – Lista Appezzamenti Attivi per Azienda
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento
 */
export interface AppezzamentoNutrizioneDto {
  /** Partita IVA dell'azienda */
  Piva: string;
  /** Codice Centro Aziendale */
  SaCod: number;
  /** Codice appezzamento */
  Appezza: number;
  IdReg: number;
  ProgettoCod: number;
  /** Descrizione Centro Aziendale */
  SaNome: string;
  /** Nome appezzamento */
  NomeAppezzamento: string;
  /** Descrizione specie vegetale */
  SpecieVegetale: string;
  /** Codice specie vegetale */
  SpecieCod: number;
  /** Varietà */
  Varieta: string;
  /** Codice varietà */
  VarietaCod: number;
  CoordinataLat: number | null;
  CoordinataLng: number | null;
  NumPiante: number;
  Portinnesto: string | null;
  StatoImpianto: string | null;
  /** Superficie in ettari */
  SuperficieHa: number;
  DataSeminaPrevista: string | null;
  AnalisiTerreno: AnalisiTerreno;
  FaseFenologicaCorrente: FaseFenologicaCorrente;
  ConsiglioNutrizioneUltimo: ConsiglioNutrizioneUltimo;
}

/**
 * Envelope returned by POST /DSSNutrizione/widget/dati.
 * DS05-BL §Output – DatiWidgetNutrizioneResult
 * FR002 §1 – Lista Appezzamenti Attivi per Azienda
 */
export interface DatiWidgetNutrizioneResult {
  TotaleAppezzamenti: number;
  PaginaAttuale: number;
  RecordPerPagina: number;
  Appezzamenti: AppezzamentoNutrizioneDto[];
}

/**
 * Request body for POST /DSSNutrizione/appezzamenti/consigli/aggregato.
 * Wraps the appezzamento DTO and carries the persistence flag.
 * DS05B-API §Formato Richiesta
 * FR004 – Recupero Dati Primari e Consiglio Nutrizionale del Singolo Widget
 */
export interface AggregaConsiglioNutrizioneRequest {
  /** Full appezzamento data from the list phase (DS05-BL §Output – AppezzamentoNutrizioneDto). */
  Appezzamento: AppezzamentoNutrizioneDto;
  /**
   * When false (default) the engine computes the consiglio without persisting it
   * in Consigli_Nutrizione_Engine. Set to true only during an explicit save (FR012).
   */
  SalvaConsiglioNutrizione: boolean;
}

// ─────────────────────────────────────────────────────────────────────────────
// § AGGREGATO RESPONSE MODELS (POST /v1/nutrizione/appezzamenti/consigli/aggregato)
// DS05B-API §Risposte – 200 Successo
// ─────────────────────────────────────────────────────────────────────────────

/**
 * A single nutrient element as computed and returned by the engine.
 * DS05B-API §Risposte 200 – elementi[]
 */
export interface ElementoNutrizioneResponse {
  Elemento: string;
  FabbisognoMinimo: number;
  FabbisognoMassimo: number;
  DoseConsigliataMinima: number;
  DoseConsigliataMassima: number;
  QuantitativoPresente: number;
  QuantitativoMinimoResiduo: number;
  QuantitativoMassimoResiduo: number;
  Status: 'success' | 'partial' | 'error';
  Message: string | null;
}

/**
 * The computed consiglio nutrizionale returned by the aggregato endpoint.
 * DS05B-API §Risposte 200 – consiglioNutrizione
 */
export interface ConsiglioNutrizioneResponse {
  ConsiglioId: number | null;
  DataConsiglio: string | null;
  Elementi: ElementoNutrizioneResponse[];
}

/**
 * Common engine response fields shared by all engine sub-responses.
 * DS05B-API §Risposte 200 – risposteEngine.*
 */
export interface EngineStatus {
  Status: 'success' | 'error' | 'timeout';
  ResponseCode: number;
  Message: string | null;
}

/**
 * Response from the Nutrizione engine.
 * DS05B-API §Risposte 200 – risposteEngine.engineNutrizione
 */
export interface EngineNutrizioneResponse extends EngineStatus {
  ConsiglioIdPersistito: number | null;
  TimestampEngine: string | null;
}

/**
 * Response from the Fenologia engine.
 * DS05B-API §Risposte 200 – risposteEngine.engineFenologia
 */
export interface EngineFenologiaResponse extends EngineStatus {
  FasiAcquisite: number;
  UltimaFaseSincronizzata: string | null;
  ListaAttivita: unknown;
  SalvataggioQDCACompletato: boolean;
}

/**
 * Response from the Modelli engine.
 * DS05B-API §Risposte 200 – risposteEngine.engineModelli
 */
export interface EngineModelliResponse extends EngineStatus {
  ModelliValidati: boolean;
  Dettagli: string | null;
}

/**
 * Aggregated engine responses returned by the aggregato endpoint.
 * DS05B-API §Risposte 200 – risposteEngine
 */
export interface RisposteEngine {
  EngineNutrizione: EngineNutrizioneResponse;
  EngineFenologia: EngineFenologiaResponse;
  EngineModelli: EngineModelliResponse;
}

/**
 * A partial-error entry describing a single failing engine.
 * DS05B-API §Risposte 200 – errori_parziali[]
 */
export interface ErroreParziale {
  Engine: string;
  CodiceErrore: string;
  Messaggio: string;
  Suggestion: string | null;
}

/**
 * Timing metrics for the aggregato call.
 * DS05B-API §Risposte 200 – metriche
 */
export interface MetricheAggregazione {
  TempoTotaleMs: number;
  TempoEngineNutrizioneMs: number;
  TempoEngineFenologiaMs: number;
  TempoEngineModelliMs: number;
}

/**
 * Full response from POST /v1/nutrizione/appezzamenti/consigli/aggregato.
 * DS05B-API §Risposte 200 – Successo
 * FR002 §3 – Consiglio Nutrizionale per Appezzamento
 */
export interface AggregazioneConsiglioNutrizioneResult {
  RequestId: string;
  Timestamp: string;
  Appezzamento: AppezzamentoNutrizioneDto;
  ConsiglioNutrizione: ConsiglioNutrizioneResponse;
  RisposteEngine: RisposteEngine;
  AggregazioneStatus: 'full_success' | 'partial_success' | 'failed';
  ErroriParziali: ErroreParziale[];
  Metriche: MetricheAggregazione;
}

// ─────────────────────────────────────────────────────────────────────────────
// § SALVA CONSIGLIO MODELS (POST /dss/nutrizione/consigli/salva)
// DS16-API – FR012 Salvataggio Consiglio Nutrizionale nel Sistema GIAS
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Request body for POST /dss/nutrizione/consigli/salva.
 * DS16-API §Formato Richiesta
 * FR012 – Salvataggio Consiglio Nutrizionale nel Sistema GIAS
 */
export interface SalvaConsiglioNutrizioneRequest {
  /** Full aggregation result from the consiglio endpoint. */
  AggregazioneConsiglio: AggregazioneConsiglioNutrizioneResult;
  /**
   * When true the backend checks for an existing consiglio before persisting.
   * Set to false when the user has confirmed overwrite after a DUPLICATE response.
   */
  ControllaDuplicati: boolean;
}

/**
 * Response envelope from POST /dss/nutrizione/consigli/salva.
 * DS16-API §Formato Risposta
 * FR012 – Salvataggio Consiglio Nutrizionale nel Sistema GIAS
 */
export interface SalvataggioConsiglioNutrizioneResponse {
  SalvataggioId: number;
  /// Esito dell'operazione: SUCCESS | DUPLICATE | ERROR.
  Esito: string;
  Messaggio: string;
  DataSalvataggio: string;
}

// ─────────────────────────────────────────────────────────────────────────────
// § UI STATE MODELS
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Lifecycle state of a single widget card.
 * FR002 §3 – Card Footer stati
 */
export type WidgetCardStatus = 'loading' | 'hasData' | 'noData' | 'error';

/**
 * Specie vegetale item returned by GET /Metaschema/specie/aziendali.
 * FR002 §2 – Popolamento Filtro Specie Vegetale
 */
export interface SpecieVegetale {
  Veg_Cod: number;
  Veg_Des: string;
}

/**
 * An appezzamento enriched with its widget display state.
 * Used internally to drive the widget cards.
 */
export interface AppezzamentoWidget {
  appezzamento: AppezzamentoNutrizioneDto;
  consiglio: AggregazioneConsiglioNutrizioneResult | null;
  status: WidgetCardStatus;
}

/**
 * A group of appezzamenti sharing the same Centro Aziendale.
 * Used to build the gias-expansionpanel sections.
 * FR002 §1 – Panel Bar raggruppati per Centro Aziendale
 */
export interface CentroAziendaleGroup {
  saCod: number;
  saDes: string;
  appezzamenti: AppezzamentoWidget[];
}

// ─────────────────────────────────────────────────────────────────────────────
// § FASI FENOLOGICHE MODELS (POST /DSSNutrizione/fasi-fenologiche-registrate)
// DS21-API – FR011 Recupero Fasi Fenologiche dell'Impianto
// ─────────────────────────────────────────────────────────────────────────────

/**
 * Request body for POST /DSSNutrizione/fasi-fenologiche-registrate.
 * DS21-API §Input / DS20-BL §Input
 * FR011 – Recupero Fasi Fenologiche dell'Impianto
 */
export interface FasiFenologicheRequest {
  /** Full appezzamento data from the list phase (DS05-BL §Output). */
  Appezzamento: AppezzamentoNutrizioneDto;
  /** Start of the validity interval (ISO-8601 date string, e.g. "2026-01-01"). */
  ValiditaInizio: string;
  /** End of the validity interval (ISO-8601 date string, e.g. "2026-12-31"). */
  ValiditaFine: string;
}

/**
 * A single phenological phase as returned by POST /DSSNutrizione/fasi-fenologiche-registrate.
 * Results are ordered chronologically (ascending) per DS20-BL §Output.
 * DS21-API §Risposte / DS20-BL §Output
 * FR011 – Recupero Fasi Fenologiche dell'Impianto
 */
export interface FaseFenologicaCorrenteDto {
  /** Registration date of the phase (ISO-8601 string). */
  DataFase: string;
  /** BBCH code (e.g. "BBCH 65"). */
  BbchCod: string;
  /** Human-readable phase description (e.g. "Fioritura"). */
  BbchDescrizione: string;
}
