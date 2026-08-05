import {BaseCodeDescr} from "../../../Model/baseClass/baseCodeDescr";
import {enum_TipoPermesso} from "../profilazione.model";

export interface UtentePermessoGerarchia {
  MenuPrimoLivello: BaseCodeDescr;
  MenuSecondoLivello: BaseCodeDescr;
  Attivita_Des: string;
  Attivita_Cod: number;
  /** 0 = Lettura, 2 = Scrittura */
  Id_Operazione: enum_TipoPermesso;
  Ordinamento: string;
}
