
import { Effluente } from 'app/Model/metaschema/Effluente';
import { TipoFertilizzante } from 'app/Model/metaschema/TipoFertilizzante';
import { TipologiaFertilizzante } from 'app/Model/metaschema/TipologiaFertilizzante';
import { RisorsaProdotto } from '../risorse/RisorsaProdotto';

export class DettaglioFertilizzazione extends RisorsaProdotto {
    N: number;
    P: number;
    K: number;
    Cu: number;
    Mg: number;
    efficienza: number;
    effluente: Effluente;
    tipoFertilizzante: TipoFertilizzante;
    tipologieFertilizzante: TipologiaFertilizzante[];
    N_Ponderato: boolean;
    P_Ponderato: boolean;
    K_Ponderato: boolean;
    Cu_Ponderato: boolean;

    constructor() {
        super();
        this.classType = 'DettaglioFertilizzazione';
    }
}
