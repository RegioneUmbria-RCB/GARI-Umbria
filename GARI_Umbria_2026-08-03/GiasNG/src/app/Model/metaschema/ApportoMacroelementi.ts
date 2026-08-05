import { RegolamentoConcimazione } from './RegolamentoConcimazione';
import { FinalitaPianoConcimazione } from './FinalitaPianoConcimazione';
import { FaseCicloColturale } from "./fase";


export class ApportoMacroelementi {
    pianoConcimazione: RegolamentoConcimazione;
    tipologia: FinalitaPianoConcimazione;
    fase: FaseCicloColturale;
    n: number;
    p2o5: number;
    k2o: number;
    mgo: number;
}
