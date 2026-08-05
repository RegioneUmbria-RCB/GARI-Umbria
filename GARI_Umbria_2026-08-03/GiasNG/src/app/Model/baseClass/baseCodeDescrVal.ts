import {BaseCodeDescr, GenericCodeDescr} from "./baseCodeDescr";

export class BaseCodeDescrVal extends BaseCodeDescr {
    public valore: string;

    constructor(codice?: number, descrizione?: string, valore?: string) {
      super(codice, descrizione);
        this.valore = valore;
    }
}

export class GenericCodeValue<K, V> extends GenericCodeDescr<K> {
  constructor(
    public codice: K,
    public descrizione: string = "",
    public valore?: V
  ) {
    super(codice, descrizione);
  }
}
