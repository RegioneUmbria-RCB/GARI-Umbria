import {BaseCodeDescr} from '../baseClass/baseCodeDescr';
import {Specie} from "./utilizzi/Specie";

export class FaseFenologica extends BaseCodeDescr {
    stadioCrescitaBBCH: BaseCodeDescr;
    specieVegetale: Specie;
    stadio: string;
    fioritura: boolean;

    constructor(codice: number,descrizione: string) {
      super(codice,descrizione);
    }
}
