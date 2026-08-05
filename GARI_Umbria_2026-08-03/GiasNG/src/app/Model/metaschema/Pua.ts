import {BaseCodeDescr} from "../baseClass/baseCodeDescr";
import {Disciplinare} from "./Disciplinari";


export class Pua extends BaseCodeDescr{

    disciplinare: Disciplinare;

    constructor(codice:number, descrizione?: string) {
        super(codice,descrizione);
    }
}
