export interface PerimetroRischiMeteoItem {
  piva_azienda: string;
  nome_azienda: string;
  id_appezzamento: number;
  nome_appezzamento: string;
  id_esercizio: number;
  id_impianto: number;
  nazione: string;
  regione: string;
  istat_reg: string;
  superficie_ha: number;
  cod_specie: number;
  nome_specie: string;
  cod_varieta: number;
  nome_varieta: string;
  data_inizio_esercizio: string;
  data_fine_esercizio: string;
  nome_esercizio: string;
  sa_cod: number;
}

export interface RiepilogoRischiRequest {
  PivaFiliera: string;
}

export interface RiepilogoRischiResponse {
  StatoCaricamento: string;
  Perimetro: PerimetroRischiMeteoItem[];
}

export interface CalcoloRischiRequest {
  PivaFiliera: string;
  Perimetro: PerimetroRischiMeteoItem[];
}

export interface RiepilogoRischiRow extends PerimetroRischiMeteoItem {
  rowKey: string;
}

export interface EsercizioRischiKey {
  piva_azienda: string;
  sa_cod: number;
  id_appezzamento: number;
  id_esercizio: number;
  id_impianto: number;
}

export interface ErroreCostruzione {
  Esercizio: EsercizioRischiKey;
  TipoErrore: string;
  Messaggio: string;
}

export interface RisultatoCalcolo {
  Esercizio: EsercizioRischiKey;
  Risposta: unknown | null;
  Avvisi: unknown[];
  Successo: boolean;
  Errore: string | null;
}

export interface CalcoloRischiResponse {
  TotaleEsercizi: number;
  TotaleSuccessi: number;
  TotaleErrori: number;
  Risultati: RisultatoCalcolo[];
  ErroriCostruzione: ErroreCostruzione[];
}
