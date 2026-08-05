import {BaseCodeDescrStr} from "../../../Model/baseClass/baseCodeDescrStr";
import {BaseCodeDescr as BaseCodeDescrInterface} from "../../../Service/api.service";
import {BaseCodeDescr} from "../../../Model/baseClass/baseCodeDescr";

export class TransizioneDiStato extends BaseCodeDescrStr {
  // public StatoOrigine: BaseCodeDescrInterface;
  // public StatoDestinazione: BaseCodeDescrInterface;
  // public Servizio: BaseCodeDescrInterface;

  constructor(codice: string, descrizione?: string) {
    super(codice, descrizione);
  }

  // public static fromBaseCodeDescrStr(base: BaseCodeDescrStr) {
  //   let t = new TransizioneDiStato(base.codice, base.descrizione);
  //   TransizioneDiStato.autofillFromCodice(t);
  //   return t;
  // }
  //
  // public static autofillFromCodice(t: TransizioneDiStato) {
  //   if (t.codice.split("_").length === 3) {
  //     let codici = t.codice.split("_");
  //     t.StatoOrigine = new BaseCodeDescr(+codici[0]);
  //     t.StatoDestinazione = new BaseCodeDescr(+codici[1]);
  //     t.Servizio = new BaseCodeDescr(+codici[2]);
  //   }
  // }
}
