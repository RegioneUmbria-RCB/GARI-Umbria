import { BaseCodeDescr } from "../baseClass/baseCodeDescr";
import { TipoEffluente } from "./TipoEffluente";
import { UnitaDiMisura } from "./UnitaDiMisura";

export class Effluente extends BaseCodeDescr {

    udm: UnitaDiMisura;
    tipoEffluente: TipoEffluente;
    carico: number;
    N: number;

    constructor(codice: number) {
        super(codice);
    }

}
