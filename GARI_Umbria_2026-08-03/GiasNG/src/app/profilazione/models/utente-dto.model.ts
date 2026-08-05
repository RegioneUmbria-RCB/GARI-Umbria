import {TipologiaUtente} from "./profili-permessi/tipologia-utente.model";
import {IUtentePassword} from "./IUtente.model";

export interface IUtenteDTO {
  Piva_SuperUser?: string;
  Password?: string;
  UserName?: string;
  Nome?: string;
  Cognome?: string;
  Email?: string;
  Rag_Soc?: string;
  Tipologia?: TipologiaUtente;
}

export class UtenteDTO implements IUtenteDTO, IUtentePassword {
  Piva_SuperUser?: string;
  Nome?: string;
  Cognome?: string;
  Email?: string;
  Rag_Soc?: string;
  Tipologia?: TipologiaUtente;

  constructor(
    public UserName: string = "",
    public Password: string = ""
  ) {
  }
}
