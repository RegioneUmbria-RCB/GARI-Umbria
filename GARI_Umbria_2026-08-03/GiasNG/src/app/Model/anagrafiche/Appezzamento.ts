import { Specie } from '../metaschema/utilizzi/Specie';
import { Campo, PKCampo } from './Campo';
import { CentroAziendale, PKCentroAziendale } from './CentroAziendale';
import { CodiciAnagrafeValori } from './CodiciAnagrafeValori';
import { Impianto } from './Impianto';
import { IndirizzoAssociato } from './addresses/IndirizzoAssociato';
import { MetodoProduzione } from '../metaschema/MetodoProduzione';
import { CatastoAppezzamento } from './CatastoAppezzamento';
import { IntervalloTemporale } from './IntervalloTemporale';
import { SistemiRiferimentoCartografia } from '../metaschema/SistemiRiferimentoCartografia';
import { BaseCodeDescrStr } from '../baseClass/baseCodeDescrStr';
import { GeoJSONAgroGisPropTreeNode } from '../GIS/GisDataReadRval_New';
import {LinkedMachine, ParcoMacchine} from './ParcoMacchine';
import { ClasseTessitura } from './ClasseTessitura';
import { Appezzamento_DatiSementieri, GisDataReadRval_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';

export type PKAppezzamento = typeof Appezzamento.PK.prototype;
export class Appezzamento {
  primaryKey: PKAppezzamento;
  descrizione: string;
  impianti: Impianto[];
  campoPK: PKCampo;
  catastoAppezzamento: CatastoAppezzamento[];
  codici: CodiciAnagrafeValori[];
  superficie: number;
  superficieGis: number;
  rif_Appezzamento: string;
  isola: string;
  terrenoInutilizzato: boolean;
  terrenoDegradato: boolean;
  lowILUC: boolean;
  sabbia: number;
  limo: number;
  argilla: number;
  classeTessitura: ClasseTessitura;
  codiceAppezzamento: CodiciAnagrafeValori;
  metodo_Produzione: MetodoProduzione;
  coltura_Precedente_1_Anno: Specie;
  coltura_Precedente_2_Anno: Specie;
  coltura_Precedente_3_Anno: Specie;
  coltura_Precedente_4_Anno: Specie;
  cartografia: string;
  sistemiRiferimentoCartografia: SistemiRiferimentoCartografia;
  immagineBase64: string;
  pendenza: number;
  esposizione: BaseCodeDescrStr;
  ubicazione: BaseCodeDescrStr;
  lat: number;
  lng: number;
  altitudine: number;
  distBZ_CorpiIdrici: number;
  distBZ_AreeResPub: number;
  distBZ_Allevamenti: number;
  distBZ_VegNatNonColt: number;
  supBZ_Riduzione: number;
  n_App_Bio: string;
  utilizzo_Terreno: BaseCodeDescrStr[];
  confini_A_Rischio: string;
  fine_Impiego_Prod_Non_Conformi: string;
  indirizzi: IndirizzoAssociato[];
  validita: IntervalloTemporale;
  flag_cancellazione: boolean;
  obj_app: GisDataReadRval_New_1OfGeoJSONAgroGisProp;
  nodeInfo_app: GeoJSONAgroGisPropTreeNode;
  linkedMachines: LinkedMachine<PKAppezzamento>[];
  dati_sementieri: Appezzamento_DatiSementieri;

  Agea_idSchedaValidazione: string;
  Agea_identificativoPianoColtivazione: string;
  Agea_codiBarrScheVali: string;
  Agea_identificativoIsola: string;
  Agea_identificativoAppezzamento: string;
  Agea_idAppezzamentoOrig: string;

  constructor(primaryKey: PKAppezzamento) {
    this.primaryKey = primaryKey;
    this.flag_cancellazione = false;
  }

  static PK = class {
    codice: number;
    centroAziendalePK: PKCentroAziendale;

    constructor(codice: number, centroAziendalePK: PKCentroAziendale) {
      this.codice = codice;
      this.centroAziendalePK = centroAziendalePK;
    }
  };
}

export class AppezzamentoJoinDescrizioni extends Appezzamento{
  saNome: string;
  ragSoc: string;

  constructor(primaryKey: PKAppezzamento) {
    super(primaryKey);
  }
}
