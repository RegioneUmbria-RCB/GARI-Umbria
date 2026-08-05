import { Ditta } from "app/Model/metaschema/Ditta";
import {BaseCodeDescr} from "../../baseClass/baseCodeDescr";


export class Trappola extends BaseCodeDescr {

    DurataFeromone: number;
    Ditta: Ditta;
    Scadenza: Date;

    constructor(codice: number) {
        super(codice);
    }
}
