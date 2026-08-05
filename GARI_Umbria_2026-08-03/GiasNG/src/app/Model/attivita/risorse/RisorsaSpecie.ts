import { Risorsa } from "./Risorsa";
import { Specie } from "app/Model/metaschema/utilizzi/Specie";

export class RisorsaSpecie extends Risorsa {

    specie: Specie;

    constructor() {
        super();
        this.classType = 'RisorsaSpecie';
    }

}