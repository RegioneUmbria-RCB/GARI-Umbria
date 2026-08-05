import { RisorsaZootecnica } from "./RisorsaZootecnica";

export class SpecieAnimale extends RisorsaZootecnica {

    codice_concatenato: string;

    constructor() {
        super();
    }

    setChiave() {
        this.codice_concatenato = this.genere.codice + "-" + this.specie.codice + "-" + this.indirizzoProd.codice;
    }

}