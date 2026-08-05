import { IntervalloTemporale } from './IntervalloTemporale';
import { FormeGiuridiche } from '../metaschema/FormeGiuridiche';
import { CentroAziendale } from './CentroAziendale';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { IndirizzoAssociato } from './addresses/IndirizzoAssociato';
import { Contatto } from './Contatto';
import { RisorseUmane } from './RisorseUmane';
import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { ImpresaPadre } from './ImpresaPadre';
import {BaseCodeDescrStr} from '../baseClass/baseCodeDescrStr';
import {ContattoAzienda} from './ContattoAzienda';

export class Impresa {
  partitaIva: string;
  partitaIvaReale?: string;
  CUAA: string;
  ragioneSociale: string;
  validita: IntervalloTemporale;
  forma_Giuridica: FormeGiuridiche;
  tipo_Impresa: number;
  centriAziendali: CentroAziendale[];
  codici: CodiciAnagrafeValori[];
  impresaPadre: ImpresaPadre[];
  certificazione: BaseCodeDescr[];
  indirizzi: IndirizzoAssociato[];
  tecnicoReferente: Contatto;
  organismo_di_Controllo: RisorseUmane;
  contatti?: Contatto[];
  //contatto_superuser: Contatto;
  flag_cancellazione: boolean;
  gruppoRaccolta: BaseCodeDescr;
  contattoAzienda?: Partial<ContattoAzienda>;
  guid: string;
  disciplinareAziendalePredefinito: BaseCodeDescrStr;

  constructor(partitaIva?: string) {
    this.flag_cancellazione = false;
    if (partitaIva !== undefined) this.partitaIva = partitaIva;
  }
}
