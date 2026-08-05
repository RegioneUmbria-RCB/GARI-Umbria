import { Razza, TipoGruppo_Zoo, TipologiaCapoAnimale } from "app/Service/api.service";
import { SottogruppoStallaLight } from "./SottogruppoStallaLight";
import { IntervalloTemporale } from "./IntervalloTemporale";
import { Specie } from "../metaschema/utilizzi/Specie";

export class SottogruppoStalla extends SottogruppoStallaLight {

    tipo: TipoGruppo_Zoo;

    validita: IntervalloTemporale;

    area: number;

    specie: Specie;

    razza: Razza;

    stato: TipologiaCapoAnimale;

}