import { UtilizzoTerreno } from '../metaschema/utilizzi/UtilizzoTerreno';
import { GruppoFinalita } from '../metaschema/utilizzi/GruppoFinalita';
import { FormaAllevamento } from '../metaschema/DensitaImpianto/FormaAllevamento';
import { Portinnesto } from '../metaschema/DensitaImpianto/Portinnesto';
import { TecnicaConduzioneSuFila } from '../metaschema/DensitaImpianto/TecnicaConduzioneSuFila';
import { TecnicaConduzioneTraFila } from '../metaschema/DensitaImpianto/TecnicaConduzioneTraFila';
import { Copertura } from '../metaschema/Copertura';
import { IntervalloTemporale } from './IntervalloTemporale';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { SeminaTrapianto } from '../metaschema/SeminaTrapianto';
import { ProvenienzaSeme } from '../metaschema/ProvenienzaSeme';
import { TagliatoIntero } from '../metaschema/TagliatoIntero';
import { SistemiRiferimentoCartografia } from '../metaschema/SistemiRiferimentoCartografia';
import { Esercizio } from './Esercizio';
import { Appezzamento, PKAppezzamento } from './Appezzamento';
import { GruppoVarietale } from '../metaschema/GruppoVarietale';
import { Irrigazione } from '../metaschema/Irrigazione';
import { GeoJSONAgroGisPropTreeNode } from '../GIS/GisDataReadRval_New';
import { GisDataReadRval_New_1OfGeoJSONAgroGisProp, UnitaDiMisura_Alternativa } from 'app/Service/api.service';
import {BaseCodeDescrStr} from "../baseClass/baseCodeDescrStr";
import {DettaglioVarietaPersonalizzato} from '../metaschema/DettaglioVarietaPersonalizzato';
import {ParcoMacchine} from './ParcoMacchine';

export type PKImpianto = typeof Impianto.PK.prototype;
export class Impianto {
  primaryKey: PKImpianto;
  descrizione: string;
  utilizzoTerreno: UtilizzoTerreno;
  superficie: number;
  superficieGis: number;
  gruppoFinalita: GruppoFinalita;
  gruppoVarietale: GruppoVarietale;
  data_Innesto_Varieta: Date;
  data_Inizio_Produzione: Date;
  data_Inizio_Portinnesto: Date;
  data_Inizio_Impianto: Date;
  codiceImpianto: string;
  algoritmoCodifica: string;
  cover_Crops: boolean;
  monitorato: boolean;
  irrigazione: Irrigazione;
  formaAllevamento: FormaAllevamento;
  portinnesto: Portinnesto;
  seminaTrapianto: SeminaTrapianto;
  provenienzaSeme: ProvenienzaSeme;
  tecnicaConduzioneTraFila: TecnicaConduzioneTraFila;
  tecnicaConduzioneSuFila: TecnicaConduzioneSuFila;
  infoAgg_Cod: number;
  consociazionePK: PKImpianto;
  impiantoConsociato: boolean;
  maschi_in_Sesto: boolean;
  cartografia: string;
  sistemaRiferimentoCartografia: SistemiRiferimentoCartografia;
  immagineBase64: string;
  tra_Fila_M: number;
  su_Fila_M: number;
  copertura: Copertura;
  cop_Data_Inizio: Date;
  cop_Data_Fine: Date;
  unita_Vitata: number;
  impianto_Ibrido: boolean;
  codBMBDBT_M: string;
  codBMBDBT_F: string;
  genetica_M: string;
  genetica_F: string;
  offType_M: string;
  offType_F: string;
  distanzaSuFila_F: number;
  distanzaTraFila_F: number;
  partiTuberi: number;
  tagliatoIntero: TagliatoIntero;
  interbina: number;
  germinabilita: number;
  dettaglio_varieta_personalizzato: DettaglioVarietaPersonalizzato;
  codiceZona: BaseCodeDescrStr;
  validita: IntervalloTemporale;
  esercizi: Esercizio[];
  codici: CodiciAnagrafeValori[];
  flag_cancellazione: boolean;
  obj_imp: GisDataReadRval_New_1OfGeoJSONAgroGisProp;
  nodeInfo_imp: GeoJSONAgroGisPropTreeNode;
  unitaMisuraAlternativa: UnitaDiMisura_Alternativa;
  superficieAlternativa: number;
  flagImpiantoIsMacchina: boolean = false;
  macchineIrrigazione: Array<ParcoMacchine> = [];

  Agea_idColt: string;

  constructor(primaryKey: PKImpianto) {
    this.primaryKey = primaryKey;
    this.flag_cancellazione = false;
  }

  static PK = class {
    codice: number;
    appezzamentoPK: PKAppezzamento;

    constructor(codice: number, appezzamentoPK: PKAppezzamento)  {
      this.codice = codice;
      this.appezzamentoPK = appezzamentoPK;
    }
  };

}
