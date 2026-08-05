import { RegolamentoConcimazione } from "../metaschema/RegolamentoConcimazione";
import { GruppoFinalita } from "../metaschema/utilizzi/GruppoFinalita";
import { Specie } from "../metaschema/utilizzi/Specie";

export class FiltroFinalita2{
    specie: Specie;
    finalita: GruppoFinalita;
    regolamentoConcimazione: RegolamentoConcimazione;
}
