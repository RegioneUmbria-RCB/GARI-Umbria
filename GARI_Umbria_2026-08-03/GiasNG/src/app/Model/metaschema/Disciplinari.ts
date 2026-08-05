import { BaseCodeDescrStr } from '../baseClass/baseCodeDescrStr';
import { RegolamentoConcimazione } from './RegolamentoConcimazione';
import { RaggruppamentiColturaliDPI } from './RaggruppamentiColturaliDPI';
import { GruppoFinalita } from './utilizzi/GruppoFinalita';
import {IntervalloTemporale} from "../anagrafiche/IntervalloTemporale";

export class Disciplinare extends BaseCodeDescrStr {

    /*0=non applicabile/nessun disciplinare/reg bio/nessun vincolo normativo
    //1=pubblico
    //2=privato*/
    disciplinarePubblicoPrivato: number;

    regolamentoConcimazione: RegolamentoConcimazione;

    raggruppamentiColturaliDPI: RaggruppamentiColturaliDPI;

    gruppoFinalita: GruppoFinalita;

    flagProtetto: number;

    idTr: number;

    validita: IntervalloTemporale;

    constructor(codice: string) {
        super(codice);
    }
}
