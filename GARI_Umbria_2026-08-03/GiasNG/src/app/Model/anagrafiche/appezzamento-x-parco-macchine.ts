import {AGRODATAFINE, AGRODATAINIZIO} from '../CostantiPersonalizzate';
import {IntervalloTemporale} from './IntervalloTemporale';

export class AppezzamentoXParcoMacchine {
  piva: string;
  saCod: number;
  appezza: number;
  macCod: number;
  classCode: string;
  validity: IntervalloTemporale;

  constructor(
    piva: string = '',
    saCod: number = 0,
    appezza: number = 0,
    macCod: number = 0,
    classCode: string = '',
    startValidity: Date = AGRODATAINIZIO,
    endValidity: Date = AGRODATAFINE
  ) {
    this.piva = piva;
    this.saCod = saCod;
    this.appezza = appezza;
    this.macCod = macCod;
    this.classCode = classCode;
    this.validity = new IntervalloTemporale(startValidity, endValidity);
  }
}
