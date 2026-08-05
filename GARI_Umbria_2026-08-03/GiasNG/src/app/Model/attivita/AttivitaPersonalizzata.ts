import { BaseCodeDescr } from "../baseClass/baseCodeDescr";
import {Lavorazione} from "./Lavorazione";

//La classe AttivitaPersonalizzata si riferisce alla tabella Attivita sul db server
export class AttivitaPersonalizzata extends BaseCodeDescr{

    sigla: string;
    operazioni: Lavorazione[];
    constructor(codice: number) {
        super(codice);
    }
}
