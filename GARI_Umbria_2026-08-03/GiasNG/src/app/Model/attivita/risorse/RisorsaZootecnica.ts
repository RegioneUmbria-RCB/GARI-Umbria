import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { Risorsa } from "./Risorsa";
import { Genere } from "app/Model/metaschema/utilizzi/Genere";
import { IndirizzoProduttivo } from "app/Model/metaschema/utilizzi/IndirizzoProduttivo";

export class RisorsaZootecnica extends Risorsa {

    genere: Genere;

    specie: Specie;

    indirizzoProd: IndirizzoProduttivo;

    descrizione: string;

    constructor() {
        super();
        this.classType = 'RisorsaZootecnica';
    }

}