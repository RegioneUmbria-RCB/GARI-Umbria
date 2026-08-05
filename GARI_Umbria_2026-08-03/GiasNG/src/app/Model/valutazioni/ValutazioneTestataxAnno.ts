import { PKValutazioneTestata } from './ValutazioneTestata';
import { BaseCodeDescrStr } from '../baseClass/baseCodeDescrStr';

export type PKValutazioneTestataxAnno = typeof ValutazioneTestataxAnno.PK.prototype;
export class ValutazioneTestataxAnno {

    static PK = class {
        Anno: number;
        valutazioneTestataPK: PKValutazioneTestata;

        constructor(anno: number, valutazioneTestataPK: PKValutazioneTestata) {
            this.Anno = anno;
            this.valutazioneTestataPK = valutazioneTestataPK;
        }
    };

    primaryKey: PKValutazioneTestataxAnno;

    Anno_Tipo: number;
    Anno_Tipo_Des: string;

    flag_cancellazione: boolean;

    constructor(primaryKey: PKValutazioneTestataxAnno) {
        this.primaryKey = primaryKey;
        this.flag_cancellazione = false;
    }

}
