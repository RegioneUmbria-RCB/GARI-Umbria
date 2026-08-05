import { FaseCicloColturale } from "../metaschema/fase";
import { FinalitaPianoConcimazione } from "../metaschema/FinalitaPianoConcimazione";
import { RegolamentoConcimazione } from "../metaschema/RegolamentoConcimazione";
import { GruppoFinalita } from "../metaschema/utilizzi/GruppoFinalita";
import { Specie } from "../metaschema/utilizzi/Specie";

export class FiltroCalcoloNPK{
    regolamento: RegolamentoConcimazione;
    specie: Specie;
    finalita: FinalitaPianoConcimazione;
    stato: FaseCicloColturale;
}
