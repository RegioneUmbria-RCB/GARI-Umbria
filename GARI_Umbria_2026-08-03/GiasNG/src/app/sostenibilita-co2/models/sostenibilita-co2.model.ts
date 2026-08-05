export type ModalitaCalcolo = 'Colture' | 'Aziendale';

export interface FiliereCO2Item {
  Piva: string;
  RagioneSociale: string;
  Padre?: string;
}

export interface FiliereCO2DisplayItem {
  Piva: string;
  RagioneSociale: string;
}

export interface FiliereCO2Response {
  Filiere: FiliereCO2Item[];
}

export interface ColturaCO2Item {
  VegCod: number;
  VegDes: string;
}

export interface ColtureCO2Response {
  Colture: ColturaCO2Item[];
}

export interface RiepilogoRaccoltiRequest {
  PivaFiliera: string;
  /** Sempre 0 — tutte le colture; il filtro coltura è client-side */
  VegCod: number;
  Anno: number;
}

export interface RigaRiepilogoRaccolti {
  Azienda: string;
  PartitaIva: string;
  Appezzamento: string;
  Esercizio: string;
  Nazione: string;
  Regione: string;
  ISTAT_reg: string;
  Provincia: string;
  Superficie: number;
  SpecieColturale: string;
  ProdottiRaccolti: string;
  CodiceLotti: string;
  DataUltimaRaccolta: string | null;
  TotaleRaccoltaKg: number;
}

export interface RiepilogoRaccoltiResponse {
  Righe: RigaRiepilogoRaccolti[];
}

export interface PerimetroFilters {
  filiera: string | null;
  modalita: ModalitaCalcolo;
  coltura: string[] | null;
  anno: number | null;
}

export interface RiepilogoPerimetroRow {
  rowKey: string;
  azienda: string;
  piva: string;
  appezzamento: string;
  esercizio: string;
  nazione: string;
  regione: string;
  istat_reg: string;
  provincia: string;
  superficie_ha: number;
  specie_colturale: string;
  prodotti_raccolti: string;
  codici_lotti: string;
  data_ultima_raccolta: string | null;
  totale_raccolto_kg: number;
  selezionabile: boolean;
}

// ── UIL002 — Immissione Consumi ──────────────────────────────────────────────

export interface AziendaPerimetro {
  azienda: string;
  piva: string;
}

export interface TipoCarburanteItem {
  Valore: number;
  Etichetta: string;
}

export interface CarburantiCO2Model {
  row_key: string;
  azienda: string;
  azienda_piva: string;
  tipo_carburante: string;
  tipo_carburante_des: string;
  quantita: number;
  unita_misura: string;
  unita_misura_des: string;
}

export interface EnergiaCO2Model {
  row_key: string;
  azienda: string;
  azienda_piva: string;
  data_inizio: Date | null;
  data_fine: Date | null;
  consumo_kwh: number;
  percentuale_rinnovabili: number;
}

export interface CarburanteM4 {
  Azienda: string;
  TipoCarburante: string;
  Quantita: number;
  UnitaMisura: string;
}

export interface EnergiaM4 {
  Azienda: string;
  DataInizio: string;
  DataFine: string;
  ConsumoKwh: number;
  PercentualeRinnovabili: number;
}

export interface PerimetroM4 {
  Aziende: string[];
  Filiera: string;
  Anno: number;
  Colture: string[];
  Modalita: ModalitaCalcolo;
}

export interface ValidazioneSostenibilitaRequest {
  PerimetroAziende: string[];
  Carburanti: CarburanteM4[];
  Energia: EnergiaM4[];
}

export interface ValidazioneSostenibilitaResult {
  ValidazioneEsito: boolean;
  Errori: string[];
  ConsumiValidati: ConsumiValidatiM4;
}

export interface ConsumiValidatiM4 {
  Carburanti: CarburanteM4[];
  Energia: EnergiaM4[];
}

export interface EsercizioM4 {
  id_esercizio: number;
  nazione: string;
  istat_reg: string;
}

export interface AssemblyPayloadM4Request {
  Perimetro: PerimetroM4;
  Carburanti: CarburanteM4[];
  Energia: EnergiaM4[];
  Esercizi: EsercizioM4[];
}

// ── FS2.07.1 — Creazione Token ───────────────────────────────────────────────

export interface TokenCreationFilters {
  filiera: string | null;
  filieraLabel: string | null;
  anno: number | null;
}

export interface TokenGenerabileApiRow {
  IdInvocazione: string;
  DataInvocazione: string | null;
  PivaAzienda: string;
  RagSocAzienda: string | null;
  VarSocSoilBiogenicCarbon: number | string;
  NumeroAppezzamenti: number | string;
  JsonRisposta: string | null;
}

export interface TokenGenerabiliResponse {
  Righe: TokenGenerabileApiRow[];
}

export interface AnniLookupResponse {
  Anni: number[];
}

export interface TokenGenerabileRow {
  rowKey: string;
  idInvocazione: string;
  dataInvocazione: string | null;
  azienda: string;
  aziendaPiva: string;
  aziendaLabel: string;
  jsonRisposta: string;
  variazSocBiogenico: number;
  nAppezzamenti: number;
  isMostRecent: boolean;
}

// ── FS2.07.2 — Predisposizione Token (Creazione Token Blockchain) ─────────────

export interface CreaTokenBlockchainRequest {
  Azienda: string;
  Filiera: string;
  IdInvocazione: string;
  PayloadLookupSostenibilitaCO2: string;
}
