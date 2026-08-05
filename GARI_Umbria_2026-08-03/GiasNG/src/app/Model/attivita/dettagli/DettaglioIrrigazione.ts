import { UnitaDiMisura } from "app/Model/metaschema/UnitaDiMisura";
import { Risorsa } from "../risorse/Risorsa";
import { EsercizioCDC } from "../centri_di_costo/EsercizioCDC";
import { BaseCodeDescr } from "gias-ui-kit";
import { ParcoMacchine } from "app/Model/anagrafiche/ParcoMacchine";

export interface DettaglioIrrigazione extends Risorsa {
    QtaRilevata: number;
    QtaTotale: number;
    unitaDiMisura: UnitaDiMisura;
    Ore: number;
    Portata: number;
    Efficienza: number;
    DataInizio: Date;
    DataFine: Date;
    Frequenza: number;
    tipoIrrigazione: TipoIrrigazione;
    esercizioCDC: EsercizioCDC;
    consiglioIrrigazione: ConsiglioIrrigazione;
    macchina: ParcoMacchine;
}

export class TipoIrrigazione extends BaseCodeDescr {

    constructor(codice?: number) {
    super(codice ?? 0, '');
  }
}

export enum ModelloDSSIrrigazione {
  Irriframe = 1
}

export enum ProviderDSSIrrigazione {
  GIAS_Irriframe = 1
}

export class ConsiglioIrrigazione extends BaseCodeDescr {
  dataEsecuzione?: Date;
  dataConsiglio?: Date;
  qtaAcqua?: number;
  unitaDiMisura?: UnitaDiMisura;
  providerConsiglio?: ProviderDSSIrrigazione;
  modelloConsiglio?: ModelloDSSIrrigazione;
  minDataTurno?: Date;
  maxDataTurno?: Date;
}

// Model for LeggiConsiglioIrrigazione
export interface LeggiConsiglioIrrigazione {
    data: Date | string;
    piva: string;
    sa_cod: number;
    appezza: number;
    id_reg: number;
    progetto_cod: number;
}
// codice: 0
// dataConsiglio: Mon Jan 01 1900 00:00:00 GMT+0100 (Ora standard dell’Europa centrale) {}
// dataEsecuzione: Mon Jan 01 0001 01:00:56 GMT+0049 (Ora standard dell’Europa centrale) {}
// descrizione: ""
// modelloConsiglio: 0
// providerConsiglio: 0
// qtaAcqua: 0
// unitaDiMisura: null