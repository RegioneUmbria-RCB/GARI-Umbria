import { ModelEntry} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {ZooActivityForRedirect} from "../services/zoo-redirector.service";
import {IChiaveCompositaPiva} from "../../menu-agenda/components/utils";

export class ZooOperationGridFlatItem implements ZooActivityForRedirect, IChiaveCompositaPiva {
  chiave_composita: string = "";
  AttivitaDesc: string;
  AttivitaSigla: string;
  /**
   * @UsageNotes
   * Used both to flag an activity as blocked and, to signal that it has already been synchronized to an animal
   * transport document (Modello 4). This choice was previously made because as soon as the animal transport document
   * is generated, the activity is flagged as blocked and cannot be edited further.
   *
   * An operation that is blocked can have a synched document but that's not always the case.
   */
  Blocco_Flag: number;
  Blocco_Flag1: number;
  Cod_Articolo: string;
  Costi_Macchine: string;
  Costi_Operatori: string;
  Creatore_Intervento: string;
  Data: Date;
  Data2: Date;
  Descrizione_Unica: string;
  Dettagli: string;
  Dettaglio_Tecnico: string;
  Elem_Cod: number;
  ID: number;
  ID_Mov_Det: number;
  Id_Agenda: number;
  Info: string;
  Lav_Cod: number;
  LottiImpianto: string;
  LottiProduzione: string;
  Mat_Cod: number;
  Mat_Des: string;
  Nota_Des: string;
  Note: string;
  Operazione_DES: string;
  Ora: Date;
  PermessoModifica: string;
  Piva: string;
  Pro_Cod: string;
  Prodotti: string;
  RifDdtFatture: string;
  Sa_Cod: number;
  Segnalazioni: string;
  Sup_Trattata: number;
  Tecnico: string;
  Tipo_Accettazione: number;
  Tipo_Destinazione: number;
  Username_Creazione: string;
  Veg_Cod: number;
  contabilizzato: number;
  gru_Des: string;
  lav_des: string;
  rag_soc: string;
  sa_nome: string;
  tipo: string;
  tipo_colore: string;
}

export const ZooOperationGridModel = {
  AttivitaDesc: new ModelEntry(CELL_TYPES.STRING),
  AttivitaSigla: new ModelEntry(CELL_TYPES.STRING),
  Blocco_Flag: new ModelEntry(CELL_TYPES.NUMBER),
  Blocco_Flag1: new ModelEntry(CELL_TYPES.NUMBER),
  Cod_Articolo: new ModelEntry(CELL_TYPES.STRING),
  Costi_Macchine: new ModelEntry(CELL_TYPES.STRING),
  Costi_Operatori: new ModelEntry(CELL_TYPES.STRING),
  Creatore_Intervento: new ModelEntry(CELL_TYPES.STRING),
  Data: new ModelEntry(CELL_TYPES.DATE),
  Data2: new ModelEntry(CELL_TYPES.DATE),
  Descrizione_Unica: new ModelEntry(CELL_TYPES.STRING),
  Dettagli: new ModelEntry(CELL_TYPES.STRING),
  Dettaglio_Tecnico: new ModelEntry(CELL_TYPES.STRING),
  Elem_Cod: new ModelEntry(CELL_TYPES.NUMBER),
  ID: new ModelEntry(CELL_TYPES.NUMBER),
  ID_Mov_Det: new ModelEntry(CELL_TYPES.NUMBER),
  Id_Agenda: new ModelEntry(CELL_TYPES.NUMBER),
  Info: new ModelEntry(CELL_TYPES.STRING),
  Lav_Cod: new ModelEntry(CELL_TYPES.NUMBER),
  LottiImpianto: new ModelEntry(CELL_TYPES.STRING),
  LottiProduzione: new ModelEntry(CELL_TYPES.STRING),
  Mat_Cod: new ModelEntry(CELL_TYPES.NUMBER),
  Mat_Des: new ModelEntry(CELL_TYPES.STRING),
  Nota_Des: new ModelEntry(CELL_TYPES.STRING),
  Note: new ModelEntry(CELL_TYPES.STRING),
  Operazione_DES: new ModelEntry(CELL_TYPES.STRING),
  Ora: new ModelEntry(CELL_TYPES.DATE),
  PermessoModifica: new ModelEntry(CELL_TYPES.STRING),
  Piva: new ModelEntry(CELL_TYPES.STRING),
  Pro_Cod: new ModelEntry(CELL_TYPES.STRING),
  Prodotti: new ModelEntry(CELL_TYPES.STRING),
  RifDdtFatture: new ModelEntry(CELL_TYPES.STRING),
  Sa_Cod: new ModelEntry(CELL_TYPES.NUMBER),
  Segnalazioni: new ModelEntry(CELL_TYPES.STRING),
  Sup_Trattata: new ModelEntry(CELL_TYPES.NUMBER),
  Tecnico: new ModelEntry(CELL_TYPES.STRING),
  Tipo_Accettazione: new ModelEntry(CELL_TYPES.NUMBER),
  Tipo_Destinazione: new ModelEntry(CELL_TYPES.NUMBER),
  Username_Creazione: new ModelEntry(CELL_TYPES.STRING),
  Veg_Cod: new ModelEntry(CELL_TYPES.NUMBER),
  contabilizzato: new ModelEntry(CELL_TYPES.NUMBER),
  gru_Des: new ModelEntry(CELL_TYPES.STRING),
  lav_des: new ModelEntry(CELL_TYPES.STRING),
  rag_soc: new ModelEntry(CELL_TYPES.STRING),
  sa_nome: new ModelEntry(CELL_TYPES.STRING),
  tipo: new ModelEntry(CELL_TYPES.STRING),
  tipo_colore: new ModelEntry(CELL_TYPES.STRING),
  STA_DES: new ModelEntry(CELL_TYPES.STRING),
  STA_NUM: new ModelEntry(CELL_TYPES.NUMBER)
};
