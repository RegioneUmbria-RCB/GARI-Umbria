import {BaseCodeDescrStr as IBaseCodeDescrStr} from "../../Service/api.service";

export class BaseCodeDescrStr implements IBaseCodeDescrStr {
    codice: string;
    descrizione: string;

    constructor(codice: string, descrizione?: string) {
        this.codice = codice;
        this.descrizione = descrizione;
    }
}
