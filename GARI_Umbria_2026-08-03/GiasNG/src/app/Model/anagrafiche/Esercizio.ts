import { Regolamenti } from '../metaschema/Regolamenti';
import { Disciplinare } from '../metaschema/Disciplinari';
import { IntervalloTemporale } from './IntervalloTemporale';
import { ImpegniAggiuntiviFacoltativi} from '../metaschema/ImpegniAggiuntiviFacoltativi';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { PKImpianto } from './Impianto';
import { Fabbricato } from './Fabbricato';
import { ApportoMacroelementi} from '../metaschema/ApportoMacroelementi';
import { CatastoEsercizio } from './CastastoEsercizio';
import {BaseCodeDescr} from '../baseClass/baseCodeDescr';
import { BaseCodeDescrStr } from "../baseClass/baseCodeDescrStr";
import { Contatto } from './Contatto';
import { LicenzaColtivazione } from '../metaschema/LicenzaColtivazione';
import {Vincolo} from "../metaschema/Vincoli";
import {Prodotto} from "../attivita/risorse/Prodotto";
import {LinkedContribute} from '../metaschema/Contribute';
import {Pair} from '@progress/kendo-data-query/dist/npm/utils';
import {KeyValue} from '@angular/common';

export class Esercizio extends BaseCodeDescr {
  apportiMassimiMacroelementi: ApportoMacroelementi;

  capitolato_Privato: BaseCodeDescrStr;
  catastoEsercizio: CatastoEsercizio[];
  certificazioneAziendale: BaseCodeDescr[];
  certificazioneProdotto: BaseCodeDescrStr;
  codiceImpiantoRibaltato: string;
  codici: CodiciAnagrafeValori[];
  contributi: BaseCodeDescrStr[];

  data_Semina_Trapianto_Prevista: Date;
  data_Raccolta_Prevista: Date;
  data_Raccolta: Date;
  data_Fioritura_Prevista: Date;
  disciplinare: Disciplinare;

  esercizio_Chiuso: boolean;
  esercizioReplica: boolean

  flagSecondoRaccolto: boolean;
  flag_cancellazione: boolean;

  gruppoRaccolta: BaseCodeDescr;

  iaf: ImpegniAggiuntiviFacoltativi[];
  id_tr: number;
  impiantoPK: PKImpianto;

  lavorazione: BaseCodeDescrStr;
  licenza_Coltivazione: LicenzaColtivazione;
  lotto: string;

  magazzino_Conferimento: Fabbricato;

  organismo_Referente: Contatto;

  modalita_liquidazione: BaseCodeDescr;
  origine_prodotto: BaseCodeDescr;

  piano_Semina: BaseCodeDescrStr;
  piante_Ha: number;
  piante_Impianto: number;
  piante_Ha_Femmine: number;
  piante_Ha_Impianto_Femmine: number;
  piante_Ha_Maschi: number;
  piante_Ha_Impianto_Maschi: number;
  prodotto: Prodotto;

  replicaGias: string;
  resa_prevista: number;
  resa_effettiva: number;
  regolamento: Regolamenti;
  residuo: BaseCodeDescr;
  riferimento_Trasferimento_Dati: Contatto;

  specifica: BaseCodeDescrStr;

  tecnico: BaseCodeDescrStr[];
  validita: IntervalloTemporale;
  vincolo: Vincolo;

  acaContributes: LinkedContribute<KeyValue<number, string>>[];

  constructor(codice: number, descrizione?: string) {
    super(codice, descrizione);
    this.flag_cancellazione = false;
  }

}
