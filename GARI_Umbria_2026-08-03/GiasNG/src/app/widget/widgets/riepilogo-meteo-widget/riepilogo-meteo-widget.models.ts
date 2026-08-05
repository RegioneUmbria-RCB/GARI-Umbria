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
    color: string;
    axis: string;
}

export interface Title {
    text: string;
}

export interface Axis {
    title: Title;
    name: string | undefined;
}

export interface MeteoChart {
    horizAxis: HorizAxis;
    series: Series[];
    axis: Axis[];
}

export interface Meteo {
    Meteo_Charts: MeteoChart[];
    Meteo_RiepilogoPeriodo: string;
    Meteo_RiepilogoSensori: string;
    Meteo_Table: any;
}

export interface Stazioni {
    Descrizione: string;
    UltimoAggiornamento: Date;
    Meteo: Meteo;
}

export interface RiepilogoMeteoModel {
    Stazioni: Stazioni[];
}

