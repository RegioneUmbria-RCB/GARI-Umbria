import { Specie } from "app/Model/metaschema/utilizzi/Specie";
import { Risorsa } from "../risorse/Risorsa";

export class DettaglioSpecie extends Risorsa {

    specie: Specie;
    
    constructor() {
        super();
    }
}