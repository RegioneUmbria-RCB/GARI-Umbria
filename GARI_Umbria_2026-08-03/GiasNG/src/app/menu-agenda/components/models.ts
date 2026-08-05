import { Fabbricato } from 'app/Model/anagrafiche/Fabbricato';
import { Attivita } from 'app/Model/attivita/Attivita';
import { Risorsa } from 'app/Model/attivita/risorse/Risorsa';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CentroItem, ImpiantoItem, OperazioneItem, SpecieItem } from './utils';
import {VariabiliInSessione_NG} from "../../Service/master.service";
import {Ricetta_Operazione} from "../../Service/Agenda/Agenda.service";

export class FiltersConfig {
    CentroAziendale: CentroItem;
    Specie: SpecieItem;
    TipoOperazione: OperazioneItem[];
    Impianti: ImpiantoItem[];
    Da: Date;
    A: Date;
    MostraDDT: boolean;
    TipoVisita: boolean;            //aggiunta per filtrare Visite per eseguite, non eseguite
}

export interface DropdownItem {descrizione: string; codice: number}

//export const GestioneMagazziniRedirectLink_Old = '/Agenda/MenuBS_WS.asmx/PaginaLinkGestioneMagazziniQueryStringDto';
export const GestioneMagazziniRedirectLink = 'Agenda/PaginaLinkGestioneMagazziniQueryStringDto'


export class GestioneMagazziniQS {
    public k: string;
    public c: string;
    public mode: string;
}

export enum enum_ModificaMultiplaOperazioni {
    AGGIUNGI_MACCHINE = -1,
    AGGIUNGI_CONTATTI = -2,
    ASSEGNA_MAGAZZINO = -3,
    MODIFICA_TIENI_QTA_TOT = -5,
    MODIFICA_TIENI_QTA_HA = -6,
    RIMUOVI_MACCHINE = -7,
    RIMUOVI_CONTATTI = -8
}

export class ModificaMultipla_Operazione {
    public Tipo_Modifica: enum_ModificaMultiplaOperazioni; //enum enum_ModificaMultiplaOperazioni
    public Attivita_list: AttivitaxModificaMutipla[];
    public Risorsa_list: Risorsa[];
    public Magazzino: Fabbricato;
    public Solo_Aziendali: boolean = false;
    public Elimina_Precedenti: boolean = false;
}

export class AttivitaxModificaMutipla {
    public ID_Agenda: number;
    public Raccoglitore_Cod: number;
    public Lav_Des: string;
    public Piva: string;
    public Sa_Cod: number;
    public Lav_Cod: number;
    public Data: Date;
}

export class InfomodificaOperazioneSingola {
    tipo: Enum_DBTypeOperation | number;
    dataOp: string;
    id_agenda: string;
    lav_cod: string;
    blocco_flag: string;
    veg_cod: string;
    piva: string;
    variabiliInSessione_NG: VariabiliInSessione_NG;
}

export class Elimina_Ricetta_Brogliaccio{
  ricette:Array<Ricetta_Operazione>;
  variabiliInSessione_NG: VariabiliInSessione_NG;
}

export class CaricaZoo{
  filtro: string;
  piva: string;
  variabiliInSessione_NG: VariabiliInSessione_NG;
}
