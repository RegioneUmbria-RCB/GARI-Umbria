import {BaseCodeDescr} from "../../../Service/api.service";

export class MisuraAvversita {
    CodiceAnagrafica: number = 0;
    CodiceMisura: number = 0;
    DPI_COD: number = 0;
    DPI_FlagPrivatoPubblico: number = 0;
    Descrizione: string = "";
    cancellabile: boolean;
    modificabile: boolean;
    valoreAnagrafica: number;

    public static fromBaseCodDescr(base: BaseCodeDescr) {
      const m = new MisuraAvversita();
      m.CodiceAnagrafica = base.codice;
      m.Descrizione = base.descrizione;
      m.valoreAnagrafica = base.codice;
      return m;
    }
}

