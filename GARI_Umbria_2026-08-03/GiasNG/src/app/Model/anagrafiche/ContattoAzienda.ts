import {RisorseUmane} from './RisorseUmane';

export class ContattoAzienda {
  risorseUmane: RisorseUmane[];
  contattoPubblico: boolean;
  proprietarioContattoAzienda: number;
  aziendaCorrentePIVA: string;

  constructor() {
    this.risorseUmane = [];
    this.contattoPubblico = false;
    this.proprietarioContattoAzienda = 0;
    this.aziendaCorrentePIVA = '';
  }
}
