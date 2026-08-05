import {GruppoUtente} from "./GruppoUtente.model";
import {TransizioneDiStato} from "./TransizioneDiStato.model";
import {enum_TipoOperazioneDB} from "../../../Model/TipiEnumerativi";

export class ScriviGruppoUtentexTransizioniStato {
  constructor(
    public Gruppo: GruppoUtente,
    public Transizioni: TransizioneDiStato[],
    public Operazione: enum_TipoOperazioneDB
  ) {}
}

export class ScriviGruppoUtente {
  constructor(
    public Gruppo: GruppoUtente,
    public Operazione: enum_TipoOperazioneDB
  ) {}
}
