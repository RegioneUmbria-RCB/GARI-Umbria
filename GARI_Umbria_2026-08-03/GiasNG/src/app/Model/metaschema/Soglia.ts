import { Lavorazione } from "../attivita/Lavorazione";
import { BaseCodeDescr } from "../baseClass/baseCodeDescr";
import { Avversita } from "./avversita/Avversita";
import { UnitaDiMisura } from "./UnitaDiMisura";

export class Soglia extends BaseCodeDescr {

    quantita: number;
    avversita: Avversita;
    udm: UnitaDiMisura;
    lavorazione: Lavorazione;

    constructor(codice: number) {
        super(codice);
    }

}
