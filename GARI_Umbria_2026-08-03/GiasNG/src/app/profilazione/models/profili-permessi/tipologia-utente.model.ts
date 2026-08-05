import { BaseCodeDescr } from "../../../Model/baseClass/baseCodeDescr";
import {Utente_Permesso} from '../../../Model/utente/utente_permesso';

/**
 * Classe che definisce una tipologia utente (aka. profilo utente).
 *
 * La tipologia utente definisce i permessi posseduti.
 */
export class TipologiaUtente extends BaseCodeDescr {
    Note = "";
    Permessi: Array<Utente_Permesso> = [];

    constructor(codice: number, descrizione?: string) {
        super(codice, descrizione);
    }
}
