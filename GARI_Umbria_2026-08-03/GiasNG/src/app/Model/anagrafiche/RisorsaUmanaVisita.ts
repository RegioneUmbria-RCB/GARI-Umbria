import { Risorsa } from "../attivita/risorse/Risorsa";
import { RisorseUmane } from "./RisorseUmane";

export class RisorsaUmanaVisita extends Risorsa {

    risorsaUmana: RisorseUmane

    constructor() {
        super();
        this.risorsaUmana = new RisorseUmane();
        this.classType = "RisorsaAssegnatarioVisita";
    }
}
