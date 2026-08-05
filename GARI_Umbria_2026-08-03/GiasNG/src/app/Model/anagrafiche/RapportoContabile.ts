import { BaseCodeDescr } from '../baseClass/baseCodeDescr';

export class RapportoContabile extends BaseCodeDescr {
  cliente: boolean;
  fornitore: boolean;
  dipendente: boolean;
  terzista: boolean;
  legale: boolean;
  agente: boolean;
  consulente: boolean;
  flag_cancellazione: boolean;

  constructor(codice: number, description: string = '') {
    super(codice, description);
    this.cliente = false;
    this.fornitore = false;
    this.dipendente = false;
    this.terzista = false;
    this.legale = false;
    this.agente = false;
    this.consulente = false;
    this.flag_cancellazione = false;
  }
}
