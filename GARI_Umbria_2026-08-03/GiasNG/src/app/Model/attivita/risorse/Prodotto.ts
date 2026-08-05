import { UnitaDiMisura } from '../../metaschema/UnitaDiMisura';
import { BaseCodeDescr } from '../../baseClass/baseCodeDescr';
import { TipoRisorsa } from './TipoRisorsa';
import {Varieta} from "../../metaschema/utilizzi/Varieta";
import {Specie} from "../../metaschema/utilizzi/Specie";
import {GruppoFinalita} from "../../metaschema/utilizzi/GruppoFinalita";
import {Regolamenti} from "../../metaschema/Regolamenti";

export class Prodotto extends BaseCodeDescr {
  elemCod: number;
  unitaDiMisura: UnitaDiMisura;
  codice_alfanumerico: string;
  tipo: TipoRisorsa;

  specie: Specie;
  varieta: Varieta;
  finalita: GruppoFinalita;
  regolamento: Regolamenti;

  constructor(codice: number, descrizione?: string) {
    super(codice, descrizione);
  }
}
