export interface Band {
  Value: number;
  Color: string;
  CurrentPerc: number;
}

export interface Parametri {
  Tipo_Sorgente: number;
  Stazione_Cod: number;
  Mod_Cod: number;
  Veg_Cod: number;
  Avv_Cod: number;
  Alg_Cod: number;
  ParametriElaborazione: string;
}

export interface Risultato {
  DataInizio: string;
  DataFine: string;
  Scale_Min: number;
  Scale_Max: number;
  Bands: Band[];
  Value: number;
  Status: string;
  StatusMsg: string;
  AuxMsg: string;
  MeteoStatus: string;
  MeteoMsg: string;
  OutputGridValues: any[];
}

export interface Indicatore {
  BackgroundColor: string;
  Parametri: Parametri;
  Risultato: Risultato;
  Stazione: string;
  Modello: string;
  Specie: string;
  Avversita: string;
  DescrParametri: string;
}

export interface IndicatoriData {
  Stato: number;
  Indicatori: Indicatore[];
}