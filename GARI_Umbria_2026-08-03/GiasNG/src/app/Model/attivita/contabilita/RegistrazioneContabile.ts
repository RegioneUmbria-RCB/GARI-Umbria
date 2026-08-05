import {BaseCodeDescr} from "../../baseClass/baseCodeDescr";
import {RisorseUmane} from "../../anagrafiche/RisorseUmane";

export class RegistrazioneContabile extends BaseCodeDescr {

    rifDocumento: string;

    anno: string;

    colli: number;

    contraente: RisorseUmane;

    constructor(codice: number, descrizione?: string) {
        super(codice,descrizione);
    }

}
