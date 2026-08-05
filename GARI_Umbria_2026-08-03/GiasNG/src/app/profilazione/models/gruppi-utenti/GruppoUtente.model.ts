import {BaseCodeDescr} from "../../../Model/baseClass/baseCodeDescr";
import {BaseCodeDescrStr} from "../../../Service/api.service";

export class GruppoUtente extends BaseCodeDescr {
  public transizioniUsate: BaseCodeDescrStr[] = [];

  /**
   * @param codice codice univoco del gruppo
   * @param descrizione nome del gruppo
   * @param identificativo Codice identificativo del gruppo utente, visibile all'utente. Può non essere valorizzato.
   */
  constructor(codice?: number, descrizione?: string, public identificativo?: string) {
    super(codice, descrizione);
  }
}
