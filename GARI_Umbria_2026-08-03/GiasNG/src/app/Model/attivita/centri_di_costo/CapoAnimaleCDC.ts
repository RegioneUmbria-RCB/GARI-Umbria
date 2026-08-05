import { BaseCodeDescr, CapoAnimale, SottogruppoStalla } from "app/Service/api.service";
import { CentroDiCosto, Tipo } from "./CentroDiCosto";

export class CapoAnimaleCDC extends CentroDiCosto {

    sottogruppoStalla_ingresso: SottogruppoStalla;

    sottogruppoStalla_uscita: SottogruppoStalla;

    capoAnimale: CapoAnimale;

    capoAnimaleNonPresente: BaseCodeDescr;

    id_movimentazione_BDN: number;

    qtaSomministrata: number;

    constructor() {
        super();
        this.classType = "CapoAnimaleCDC";
        this.tipo = Tipo.CapoAnimale;
    }

}