/** DS10: single item in the `catalogo` array returned by GET /v1/catalogo/pratiche */
export interface CatalogoPraticheApiItem {
  servizio_cod: number;
  servizio_descrizione: string;
  dataValiditaInizio: string | null;
  dataValiditaFine: string | null;
  isValida: boolean;
  isAttiva: boolean;
}

/** DS10: shape of `RispostaStringa` from GET /v1/catalogo/pratiche */
export interface CatalogoPraticheRisposta {
  success: boolean;
  catalogo: CatalogoPraticheApiItem[];
  total_count: number;
  page: number;
  page_size: number;
  total_pages: number;
}

export interface PraticaSelezionabile {
  Servizio_Cod: number;
  ServizioDescrizione: string;
  ConsideraValiditaTemporale: boolean;
  DataValiditaInizio: Date | null;
  DataValiditaFine: Date | null;
  IsValida: boolean;
  /** Frontend-only: whether the user has selected this practice. */
  Selected: boolean;
}

export interface ConfigurazionePraticheUtente {
  Username: string;
  OperatoreFiltri: 'AND' | 'OR';
  FiltroPraticheAttivo: boolean;
  Pratiche: PraticaSelezionabile[];
}


