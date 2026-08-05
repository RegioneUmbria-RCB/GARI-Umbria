import { Observable } from "rxjs";
import { AGRODATAFINE, AGRODATAINIZIO, Enum_DBTypeOperation } from "./models";

export enum Tipo_Attivita {
    QuadernoDiCampagna = 1,
    Ricetta = 2
}

export enum enum_Tipo_Operazione_Agenda_Target {
    Reale = 1,
    Planning = 2
}

export enum Tipo_Ricetta {
    Standard = 0,
    Costi = 1,
    PUA = 2,
    Budget_Globale = 3,
    Budget_Utente = 4,
    Standard_Destinazioni = 5,
    PianoDistribuzioneConcimi = 6,
    ControlloDiGestione = 7,
    Standard_Destinazioni_Planning = 8,
    PianoDistribuzionePua = 9,
    RichiestaUMA = 10
}

export enum Stati {
    Da_Eseguire = 300,
    Eseguita = 301
}

export class ObjParametriAgenda {
    TipoOperazioneDB: Enum_DBTypeOperation;
    Piva: string;
    Sa_Cod?: number;
    Fabbricato?: number;
    Campo_Cod?: number;
    Appezza?: number;
    Id_Reg?: number;
    Progetto_Cod?: number;
    Validita_Inizio: Date;
    Validita_Fine: Date;
    Veg_Cod?: number;
    Veg_Des?: string;
    Id_Cod?: number;
    Id_Des?: string;
    Cau_Mov?: number;
    Mac_Cod?: number;
    Lav_Cod?: number;
    Lav_Des: string;
    SaNome: string;
    RagSoc: string;
    Pagina_Provenienza?: number;
    /*@description:
    * valorizzato se Sito_provenienza è diverso da Gias_NG per mantenere valorizzato e non modificare la Pagina di un altro sito
    * con cui si sta aprendo il Gias_NG
    * */
    Pagina_Provenienza_AltroSito?: number;
    Pagina_Richiesta?: number;
    Data: Date;
    Cod_Contatto: string;
    QueryStringFiltrino: string;
    Impianti: ImpiantiAgendaNG[];
    Id_Agenda: number;
    TipoOperazioneAgenda: Tipo_Attivita;
    TipoRicetta: Tipo_Ricetta;
    Stato: Stati | 0;
    Ricetta_Operazione_Cod: number;
    Ricetta_Cod: number;
    TargetOperazione: enum_Tipo_Operazione_Agenda_Target | 0;
    Programmazione_Cod: number;
    GenericObj_string: string;
    RedirectUrl: string;
    IdSezione: number;
    Chiave: string;
    Sito_Provenienza: number;
    Regolamento_Cod: number;
    Tipo_Regolamento: number;

    constructor() {
        this.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        this.Piva = '';
        this.Sa_Cod = 0;
        this.Fabbricato = 0;
        this.Campo_Cod = 0;
        this.Appezza = 0;
        this.Id_Reg = 0;
        this.Progetto_Cod = 0;
        this.Validita_Inizio = AGRODATAINIZIO;
        this.Validita_Fine = AGRODATAFINE;
        this.Veg_Cod = 0;
        this.Veg_Des = "";
        this.Id_Cod = 0;
        this.Id_Des = "";
        this.Cau_Mov = 0;
        this.Mac_Cod = 0;
        this.Lav_Cod = 0;
        this.Lav_Des = "";
        this.SaNome = "";
        this.RagSoc = "";
        this.Pagina_Provenienza = 0;
        this.Pagina_Provenienza_AltroSito = 0;
        this.Pagina_Richiesta = 0;
        this.Data = AGRODATAINIZIO;
        this.Cod_Contatto = "";
        this.QueryStringFiltrino = "";
        this.Impianti = [];
        this.Id_Agenda = 0;
        this.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
        this.TipoRicetta = 0;
        this.Stato = 0;
        this.Ricetta_Operazione_Cod = 0;
        this.Ricetta_Cod = 0;
        this.GenericObj_string = '';
        this.IdSezione = -1;
        this.Chiave = "";
        this.Sito_Provenienza = 0;
        this.Regolamento_Cod = 0;
        this.Tipo_Regolamento = 0;
    }

    isAgendaSelected() {
        return this.Piva != null && this.Piva !== '';
    }
    isCentroSelected() {
        return this.Sa_Cod != null && this.Sa_Cod !== 0;
    }
}

export class ImpiantiAgendaNG {
    Piva: string;
    Sa_Cod: number;
    Appezza: number;
    Id_Reg: number;
    Progetto_Cod: number;
    Veg_Cod: number;
    Id_Cod: number;
    Sup_Imp: number;
    Sup_Imp_help: number;
    Sup_Riduzione_BufferZone: number;
    Perc_Riduzione_Deriva: number;
}

export const GIAS_PARAMETRI_AGENDA_TOKEN = 'GIAS_PARAMETRI_AGENDA_TOKEN';

export interface IObjParametriAgendaService {
    getObjParamValue(): ObjParametriAgenda;

    currentObjParametriAgenda: Observable<ObjParametriAgenda>;
}
