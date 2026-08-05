import { Attivita } from "../Attivita";
import { Risorsa } from "../risorse/Risorsa";

export class DettaglioVisita extends Risorsa {

    AttivitaCollegate: Attivita[];
    
    constructor() {
        super();
        this.AttivitaCollegate = [];
        this.classType = "DettaglioVisita";
    }
}