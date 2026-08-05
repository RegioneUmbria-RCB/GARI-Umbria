import {TipologiaUtente} from "../../../Service/api.service";
import {IUtentePassword} from "../IUtente.model";

export interface UtentePermessi extends IUtentePassword{
  ValiditaInizioPermessi: Date;
  ValiditaFinePermessi: Date;
  Tipologia: TipologiaUtente;
}
