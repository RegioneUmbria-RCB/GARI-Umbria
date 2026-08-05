import { BaseCodeDescrStr } from '../baseClass/baseCodeDescrStr';
import {Disciplinare} from "./Disciplinari";
import {Regolamenti} from "./Regolamenti";

export class Vincolo extends BaseCodeDescrStr {
    disciplinare: Disciplinare;
    regolamento: Regolamenti;
    constructor(codice: string) {
        super(codice);
    }
}
