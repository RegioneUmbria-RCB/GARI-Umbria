
export interface SimpleBand {
  StopValue: number;
  Color: string;
  Opacity: number;
}

export interface ComplexBand {
  Bands: SimpleBand[];
}

export interface ComplexAxis {
  axes: SimpleAxis[];
  bands: ComplexBand;
}

export interface AxisTitle {
  text: string;
}

export interface SimpleAxis {
  name: string | undefined;
  title: AxisTitle;
}

export interface HorizAxis {
  field: string;
  baseUnit: string;
}

export interface Series {
  name: string;
  type: string;
  tipo_sensore: number;
  FunAggreg: string;
  field: string;
  color: string | undefined;
  axis: string | undefined;
}

export interface MeteoChart {
  horizAxis: HorizAxis;
  series: Series[];
  axis: SimpleAxis[] | ComplexAxis;
}

export interface AlertSerie {
  data: Date;
  value: number;
  color: string;
}

export interface MeteoData {
  Meteo_Charts: MeteoChart[];
  Meteo_RiepilogoPeriodo: string;
  Meteo_RiepilogoSensori: string;
  Meteo_Table: string;
}

export interface MonitoraggioStation {
  AlertSerie: AlertSerie[];
  Descrizione: string;
  Meteo: MeteoData;
  SogliaInf: number;
  SogliaSup: number;
}