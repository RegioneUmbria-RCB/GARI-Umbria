import { ValutazioneTestataxAnno } from './ValutazioneTestataxAnno';
import { BaseCodeDescrStr } from '../baseClass/baseCodeDescrStr';

export type PKValutazioneTestata = typeof ValutazioneTestata.PK.prototype;
export class ValutazioneTestata {

    static PK = class {
        Piva: string;
        Id_Testata: number;

        constructor(piva: string, codice: number) {
            this.Piva = piva;
            this.Id_Testata = codice;
        }
    };

    primaryKey: PKValutazioneTestata;
    Ragione_Sociale: string;
    Valutazione_Piano_Cod: number;
    Valutazione_Piano_Des: string;
    Data_Redazione: string;
    Note : string;

    Valutazione_TestataxAnno: ValutazioneTestataxAnno[];

    flag_cancellazione: boolean;

    constructor(primaryKey: PKValutazioneTestata) {
        this.primaryKey = primaryKey;
        this.flag_cancellazione = false;
    }

}
