export type ModalitaCalcolo = 'Colture' | 'Aziendale';

export interface FiliereH20Item {
  Piva: string;
  RagioneSociale: string;
}

export interface FiliereH20Response {
  Filiere: FiliereH20Item[];
}

export interface ColturaH20Item {
  VegCod: number;
  VegDes: string;
}

export interface ColtureH20Response {
  Colture: ColturaH20Item[];
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
  id_esercizio: number;
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

// ── Calcolo H2O ──────────────────────────────────────────────────────────────

export interface PerimetroH20Payload {
  Aziende: string[];
  Filiera: string;
  Anno: number;
  Colture: string[];
  Modalita: ModalitaCalcolo;
}

export interface EsercizioH20Payload {
  id_esercizio: number;
  nazione: string;
  istat_reg: string;
}

export interface CalcoloH20Request {
  Perimetro: PerimetroH20Payload;
  Esercizi: EsercizioH20Payload[];
}

export interface DettagliCalcoloH20 {
  benchmarkConsumoIdrico: number;
  fattoreEfficienza: number;
  formulaApplicata: string;
}

export interface IndicatoreH20 {
  idEsercizio: string;
  fabbisognoM3PerHa: number;
  consumataM3PerHa: number;
  daMeteoM3PerHa: number;
  deltaM3PerHa: number;
  sostenibilitaIndicatore: string;
  dettagliCalcolo: DettagliCalcoloH20;
}

export interface CalcoloH20Response {
  indicatori: IndicatoreH20[];
}
