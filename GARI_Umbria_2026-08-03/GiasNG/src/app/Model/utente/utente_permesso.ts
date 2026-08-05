import {enum_TipoPermesso} from "../../profilazione/models/profilazione.model";

export class Utente_Permesso{
  /**
   * @param Permesso_ID corrisponde al codice attività del permesso
   * @param Permesso_Tipo livello di permessi sull'attività
   */
  constructor(
        public Permesso_ID?: number,
        public Permesso_Tipo?: number | enum_TipoPermesso
    ) { }
}
