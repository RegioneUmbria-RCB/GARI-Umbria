import { Regolamenti } from "../metaschema/Regolamenti";
import { GruppoFinalita } from "../metaschema/utilizzi/GruppoFinalita";
import { Specie } from "../metaschema/utilizzi/Specie";

export class FiltroPC_Finalita_Rer{
    regolamento: Regolamenti;
    specie: Specie;
    finalita: GruppoFinalita;
}
