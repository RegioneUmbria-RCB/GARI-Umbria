import {BaseCodeDescr as IBaseCodeDescr} from "app/Service/api.service";

export class BaseCodeDescr implements IBaseCodeDescr {
  constructor(
    public codice: number,
    public descrizione: string = ""
  ) {
  }
}

export class GenericCodeDescr<K> {
  constructor(
    public codice: K,
    public descrizione: string = ""
  ) {
  }
}
