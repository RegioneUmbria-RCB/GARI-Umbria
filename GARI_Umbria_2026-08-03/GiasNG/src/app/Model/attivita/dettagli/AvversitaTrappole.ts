import {BaseCodeDescr} from "../../baseClass/baseCodeDescr";
import {AvversitaGruppo} from "../../metaschema/avversita/AvversitaGruppo";
import {UnitaDiMisura} from "../../metaschema/UnitaDiMisura";
import {UtilizzoAvversitaTrappole} from "./UtilizzoAvversitaTrappole";

export class AvversitaTrappole extends BaseCodeDescr{
  public avversitaGruppo: AvversitaGruppo;
  public unitaDiMisura: UnitaDiMisura;
  public utilizzi: UtilizzoAvversitaTrappole[];

  constructor(codice: number, descrizione: string) {
    super(codice, descrizione);

  }
}
