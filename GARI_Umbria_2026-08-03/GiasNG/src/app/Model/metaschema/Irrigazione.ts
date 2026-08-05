import {BaseCodeDescr} from '../baseClass/baseCodeDescr';

export class Irrigazione extends BaseCodeDescr {

    constructor(codice: number, descrizione: string = "") {
        super(codice, descrizione);
    }

}

/**
 * Classe creata per distinguere gli impianti di irrigazione dalle macchine in
 * fase di creazione e modifica di un impianto.
 */
export class MacchinaIrrigazione extends Irrigazione {

    constructor(codice: number, descrizione: string = "") {
        super(codice, descrizione);
    }
}
