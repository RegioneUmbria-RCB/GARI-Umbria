import {ModelEntry} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';

export class ZooPrescriptionGridFlatItem {
  AIC: string;
  Blocco_Data: Date | null;
  Blocco_Flag: number;
  Blocco_Username: string;
  DataEmissione: Date;
  DataInvio: Date | null;
  Data_Creazione: Date;
  Data_Modifica: Date;
  Denominazione: string;
  DetentoreIdFiscale: string;
  Quantitativo: number;
  Dose: number;
  IdRicetta: number;
  IdAgenda: number;
  IdDettaglio: number;
  IdMov: number;
  Intervallo_Somministrazioni: number;
  Inviato: number;
  Note: string;
  Numero: string;
  Numero_Somministrazioni: number;
  Pin: string;
  Piva: string;
  ProprietarioIdFiscale: string;
  ProtocolloCodice: string;
  RigaCardinalita: number;
  rag_soc: string;
  Sa_Cod: number;
  sa_nome: string;
  Sta_Num: number;
  STA_DES: string;
  StatoCodice: number;
  StrutturaCodice: string;
  StrutturaDenominazione: string;
  TipoCodice: number;
  TipoDes: string;
  Udm_Cod: number;
  Udm_Des: string;
  Username_Creazione: string;
  Username_Modifica: string;
  Validita_Fine: Date;
  Validita_Inizio: Date;
  VeterinarioIdFiscale: string;
}

export const ZooPrescriptionGridModel = {
  AIC: new ModelEntry(CELL_TYPES.STRING),
  Arrotondamento_Peso: new ModelEntry(CELL_TYPES.NUMBER),
  Blocco_Data: new ModelEntry(CELL_TYPES.DATE),
  Blocco_Flag: new ModelEntry(CELL_TYPES.NUMBER),
  Blocco_Username: new ModelEntry(CELL_TYPES.STRING),
  DataEmissione: new ModelEntry(CELL_TYPES.DATE),
  DataInvio: new ModelEntry(CELL_TYPES.DATE),
  Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME),
  Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME),
  Denominazione: new ModelEntry(CELL_TYPES.STRING),
  DetentoreIdFiscale: new ModelEntry(CELL_TYPES.STRING),
  IdRicetta: new ModelEntry(CELL_TYPES.NUMBER),
  IdAgenda: new ModelEntry(CELL_TYPES.NUMBER),
  IdDettaglio: new ModelEntry(CELL_TYPES.NUMBER),
  IdMov: new ModelEntry(CELL_TYPES.NUMBER),
  Inviato: new ModelEntry(CELL_TYPES.NUMBER),
  Note: new ModelEntry(CELL_TYPES.STRING),
  Numero: new ModelEntry(CELL_TYPES.STRING),
  Pin: new ModelEntry(CELL_TYPES.STRING),
  Piva: new ModelEntry(CELL_TYPES.STRING),
  rag_soc: new ModelEntry(CELL_TYPES.STRING),
  Posologia: new ModelEntry(CELL_TYPES.STRING),
  Intervallo_Somm: new ModelEntry(CELL_TYPES.NUMBER),
  Numero_Somm: new ModelEntry(CELL_TYPES.NUMBER),
  ProprietarioIdFiscale: new ModelEntry(CELL_TYPES.STRING),
  ProtocolloCodice: new ModelEntry(CELL_TYPES.STRING),
  RigaCardinalita: new ModelEntry(CELL_TYPES.NUMBER),
  Sa_Cod: new ModelEntry(CELL_TYPES.NUMBER),
  sa_nome: new ModelEntry(CELL_TYPES.STRING),
  Sta_Num: new ModelEntry(CELL_TYPES.NUMBER),
  STA_DES: new ModelEntry(CELL_TYPES.STRING),
  StatoCodice: new ModelEntry(CELL_TYPES.NUMBER),
  StrutturaCodice: new ModelEntry(CELL_TYPES.STRING),
  StrutturaDenominazione: new ModelEntry(CELL_TYPES.STRING),
  TipoCodice: new ModelEntry(CELL_TYPES.NUMBER),
  TipoDes: new ModelEntry(CELL_TYPES.STRING),
  Username_Creazione: new ModelEntry(CELL_TYPES.STRING),
  Username_Modifica: new ModelEntry(CELL_TYPES.STRING),
  DataInizioTrattamento: new ModelEntry(CELL_TYPES.DATE),
  Validita_Fine: new ModelEntry(CELL_TYPES.DATE),
  Validita_Inizio: new ModelEntry(CELL_TYPES.DATE),
  VeterinarioIdFiscale: new ModelEntry(CELL_TYPES.STRING),
  Quantitativo: new ModelEntry(CELL_TYPES.NUMBER),
  Qta_Dose: new ModelEntry(CELL_TYPES.NUMBER),
  Udm_Dose: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
  Udm_Sim: new ModelEntry(CELL_TYPES.STRING),
  Capi: new ModelEntry(CELL_TYPES.STRING),
  ribaltata: new ModelEntry(CELL_TYPES.NUMBER),
  Note_Agenda: new ModelEntry(CELL_TYPES.STRING),
  Massivo: new ModelEntry(CELL_TYPES.BOOLEAN)
};
