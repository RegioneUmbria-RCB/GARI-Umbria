import { CentroAziendale, PKCentroAziendale } from './CentroAziendale';
import { Macchine } from '../metaschema/Macchine';
import { IntervalloTemporale } from './IntervalloTemporale';
import { UnitaDiMisura} from '../metaschema/UnitaDiMisura';
import { CostoUnitario } from './CostoUnitario';
import { DittaMacchina } from '../metaschema/DittaMacchina';
import { FinalitaMacchina } from '../metaschema/FinalitaMacchina';
import { MacchineDettaglio1 } from '../metaschema/MacchineDettaglio1';
import { MacchineDettaglio2 } from '../metaschema/MacchineDettaglio2';
import { TitoloDiPossesso } from '../metaschema/TitoloDiPossesso';
import { TipoTarga } from '../metaschema/TipoTarga';
import { Carburante } from '../metaschema/Carburante';
import { Immagine } from './Immagine';
import { List } from 'lodash';
import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { Tipo } from '../MetaschemaModel';
import { Contatto } from './Contatto';
import {AGRODATAFINE, AGRODATAINIZIO} from '../CostantiPersonalizzate';
import {MacchineCodificaAgea} from '../metaschema/CodificheAgea';
import { D } from '@angular/cdk/bidi-module.d-D-fEBKdS';

export class ParcoMacchine {
  partitaIva: string;
  codice: number;
  centroPK: PKCentroAziendale;

  contatto: Contatto;
  alimentazione: Carburante;
  codice_stringa: string;
  costi: CostoUnitario[];
  CUAA_Proprietario: string;

  data_Immatricolazione: Date;
  Data_Carico: Date;
  Data_Scarico: Date;
  data_Rilascio_Autorizzazione: Date;
  data_Ultima_Revisione: Date;
  data_Ultima_Manutenzione: Date;
  data_Ultima_Taratura: Date;
  descrizione: string;
  dettaglio_1: MacchineDettaglio1;
  dettaglio_2: MacchineDettaglio2;
  ageaCod: MacchineCodificaAgea;

  finalita: FinalitaMacchina;

  marca: DittaMacchina;
  modello: string;

  N_Autorizzazione_Trasporto: string;
  n_Immatricolazione: string;
  n_Immatricolazione_Rimorchio: string;
  note: string;

  potenza: string;
  proprietario: string;

  scadenza_Taratura: Date;
  stato_Utilizzo: string;

  immagineGrande: Immagine;
  immaginePiccola: Immagine;

  VIN: String;
  BTM_Serial: String;
  ExternalAPIKey: String;

  HubIoT_PlatformDestination: BaseCodeDescr;

  caratteristiche: Caratteristica[];

  gerarchiaPadre: MacchinaGerarchia;

  gerarchiaFigli: MacchinaGerarchia[];

  rateiTempo: RateoTempo[];

  taratura_Ugello: number;
  numero_certificato: string;
  targa: string;
  telaio: string;
  tipo: Macchine;
  tipo_Targa: TipoTarga;
  titolo_Possesso: TitoloDiPossesso;

  unita_Misura: UnitaDiMisura;

  validita: IntervalloTemporale;
  visibilitaPubblica: boolean|number;
  visibileControlloGestione: boolean;

  flag_cancellazione: boolean;

  efficienza: number = 0;
  portata: number = 0;
  codice_impianto: number = 0;

  //stazioni meteo
  Distinta_Installazione: string = "";
  Contratto_Installazione: string = "";
  Tipologia_Installazione: string = "";
  Data_Inizio_Installazione: Date;
  Data_Fine_Installazione: Date;
  Stato_Installazione: string = "";
  Provincia_Istat_Installazione: string = "";
  Comune_Istat_Installazione: string = "";
  Indirizzo_Installazione: string = "";
  Latitudine_Installazione: number;
  Longitudine_Installazione: number;


  constructor(macCod: number = 0) {
    this.flag_cancellazione = false;

    this.partitaIva = '';
    this.codice = macCod;
    this.centroPK = new CentroAziendale.PK(0, '');
    this.contatto = new Contatto();
    this.alimentazione = new Carburante();
    this.codice_stringa = '';
    this.costi = [];
    this.CUAA_Proprietario = '';
    this.data_Immatricolazione = AGRODATAINIZIO;
    this.data_Rilascio_Autorizzazione = new Date();
    this.data_Ultima_Revisione = new Date();
    this.data_Ultima_Manutenzione = new Date();
    this.Data_Carico = AGRODATAINIZIO;
    this.Data_Scarico = AGRODATAFINE;
    this.data_Ultima_Taratura = AGRODATAINIZIO;
    this.scadenza_Taratura = AGRODATAFINE;
    this.descrizione = '';
    this.dettaglio_1 = new MacchineDettaglio1();
    this.dettaglio_2 = new MacchineDettaglio2();
    this.ageaCod = new MacchineCodificaAgea();
    this.finalita = new FinalitaMacchina();
    this.marca = new DittaMacchina();
    this.modello = '';
    this.N_Autorizzazione_Trasporto = '';
    this.n_Immatricolazione = '';
    this.n_Immatricolazione_Rimorchio = '';
    this.note = '';
    this.potenza = '';
    this.proprietario = '';
    this.stato_Utilizzo = '';
    this.immagineGrande = undefined;
    this.immaginePiccola = undefined;
    this.VIN = '';
    this.BTM_Serial = '';
    this.ExternalAPIKey = '';
    this.HubIoT_PlatformDestination = new BaseCodeDescr(0);
    this.caratteristiche = [];
    this.gerarchiaPadre = undefined;
    this.gerarchiaFigli = [];
    this.taratura_Ugello = 0;
    this.numero_certificato = '';
    this.targa = '';
    this.telaio = '';
    this.tipo = new Macchine();
    this.tipo_Targa = new TipoTarga();
    this.titolo_Possesso = new TitoloDiPossesso(0);
    this.unita_Misura = new UnitaDiMisura(0, '');
    this.validita = new IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE);
    this.visibilitaPubblica = false;
    this.visibileControlloGestione = false;
  }

}

export class Caratteristica {
  codice: number;
  caratteristica: CaratteristicaDettagli;
  valore: string;
  validita_inizio: Date;
  validita_fine: Date;

  constructor(codice: number, Caratteristica_Cod: number, Caratteristica_Des: string, valore: string, validita: IntervalloTemporale) {
    this.codice = codice;
    this.caratteristica = new CaratteristicaDettagli(Caratteristica_Cod, Caratteristica_Des);
    this.valore = valore;
    this.validita_inizio = validita.inizio;
    this.validita_fine = validita.fine;
  }
}

export class CaratteristicaDettagli {
  codice: number;
  descrizione: string;

  constructor(codice: number, descrizione: string) {
    this.codice = codice;
    this.descrizione = descrizione;
  }
}

export class MacchinaGerarchia {
  ID: number;
  codice: number;
  macchina: ParcoMacchine;
  legame: BaseCodeDescr;
  desclegame: string;
  udm: UnitaDiMisura;
  qta: number;
  validita_inizio: Date;
  validita_fine: Date;

  constructor(ID: number, codice: number, Componente: ParcoMacchine, Legame_Cod: number, Legame_Des: string, DescrizioneLegame: string, UdM_Cod: number, UdM_Des: string, Qta: number, validita: IntervalloTemporale) {
    this.ID = ID;
    this.codice = codice;
    this.macchina = Componente;
    this.legame = new BaseCodeDescr(Legame_Cod, Legame_Des);
    this.desclegame = DescrizioneLegame;
    this.udm = new UnitaDiMisura(UdM_Cod, UdM_Des);
    this.qta = Qta;
    this.validita_inizio = validita.inizio;
    this.validita_fine = validita.fine;
  }
}

export class RateoTempo {
  
  Rateo_Cod: number;
  DataInizio: Date;
  DataFine: Date;
  OraInizio: Date;
  OraFine: Date;
  Rotazione: number
  ValiditaInizio: Date;
  ValiditaFine: Date;

  constructor(Rateo_Cod: number, DataInizio: Date, DataFine: Date, OraInizio: Date, OraFine: Date, Rotazione: number, ValiditaInizio: Date, ValiditaFine: Date) {
    this.Rateo_Cod = Rateo_Cod;
    this.DataInizio = DataInizio;
    this.DataFine = DataFine;
    this.OraInizio = OraInizio;
    this.OraFine = OraFine;
    this.Rotazione = Rotazione;
    this.ValiditaInizio = ValiditaInizio;
    this.ValiditaFine = ValiditaFine;
  }

}

export class LinkedMachine<T> extends ParcoMacchine {
  linkValidity: IntervalloTemporale;
  linkedItemPK: T;

  constructor(pk: T, macCod: number) {
    super(macCod);
    this.linkedItemPK = pk;
  }
}

export enum enum_TipoLegame {
  Gerarchia = 1
}
