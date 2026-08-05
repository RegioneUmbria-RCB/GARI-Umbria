import { KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, ModelEntry } from 'gias-kendo-grid';
import {
    LAVCOD_ABBATTIMENTOIMPIANTI,
    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI,
    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI,
    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI,
    LAVCOD_ALTRE_OPERAZIONI,
    LAVCOD_ANDANAMENTO,
    LAVCOD_ARATURA,
    LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
    LAVCOD_ASSOLCATURA,
    LAVCOD_CARICO_MANUALE_FRUTTA,
    LAVCOD_CIMATURA,
    LAVCOD_CONCIA_SEME,
    LAVCOD_CONCIMAZIONE_FOGLIARE,
    LAVCOD_CONFUSIONE_SESSUALE,
    LAVCOD_DIRADAMENTO_MANUALE,
    LAVCOD_DISERBO,
    LAVCOD_DISORIENTAMENTO_SESSUALE,
    LAVCOD_DISSECCAMENTO,
    LAVCOD_DISSODAMENTO,
    LAVCOD_DISTRIBUZIONE_AMMENDANTI,
    LAVCOD_DISTRIBUZIONE_CONCIME,
    LAVCOD_DISTRIBUZIONE_INSETTI,
    LAVCOD_ERPICATURA,
    LAVCOD_ERPICATURA_ROTANTE,
    LAVCOD_ESPIANTO,
    LAVCOD_ESTIRPATURA,
    LAVCOD_FALCIACONDIZIONATURA,
    LAVCOD_FALCIATURA_ERBAI,
    LAVCOD_FERTIRRIGAZIONE,
    LAVCOD_FORMAZIONE_ARGINELLI,
    LAVCOD_FRANGIZOLLATURA,
    LAVCOD_FRESATURA,
    LAVCOD_GEBIATURA,
    LAVCOD_GEODISINFESTAZIONE,
    LAVCOD_IMBALLO_FIENO_ROTOLI,
    LAVCOD_INTERRAMENTO_PAGLIE,
    LAVCOD_INTERVENTO_ANTIBRINA,
    LAVCOD_LAVORAZIONE_CONBINATA,
    LAVCOD_LAVORAZIONE_SU_FILA,
    LAVCOD_LAVORAZIONE_TRA_FILA,
    LAVCOD_LEGATURA,
    LAVCOD_LIVELLAMENTO,
    LAVCOD_MANUTENZIONE_ARGINI,
    LAVCOD_MESSA_DIMORA_PIANTE,
    LAVCOD_MIETITREBBIATURA,
    LAVCOD_MINIMUM_TILLAGE,
    LAVCOD_PACCIAMATURA,
    LAVCOD_PIRODISERBO,
    LAVCOD_POTATURA_SECCA,
    LAVCOD_POTATURA_VERDE,
    LAVCOD_PRESSATURA,
    LAVCOD_RACCOLTA_MANUALE,
    LAVCOD_RACCOLTA_MECCANICA,
    LAVCOD_RANGHINATURA,
    LAVCOD_RINCALZATURA,
    LAVCOD_RIPPATURA,
    LAVCOD_RIPUNTATURA,
    LAVCOD_RIVOLTAMENTO_FORAGGIO,
    LAVCOD_ROMPICROSTA,
    LAVCOD_RULLATURA,
    LAVCOD_SARCHIATURA,
    LAVCOD_SARCHIATURA_CONCIMAZIONE,
    LAVCOD_SCARIFICATURA,
    LAVCOD_SCASSO,
    LAVCOD_STRIGLIATURA,
    LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
    LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
    LAVCOD_TRATTAMENTO_FITOREGOLATORE,
    LAVCOD_TRINCIATURA,
    LAVCOD_VANGATURA,
    LAVCOD_ZAPPATURA,
    LAVCOD__RACCOLTA_LEGNA_POTATURA,
    LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE
} from 'app/Model/CostantiPersonalizzate';
import { CommandsDropDownEvents } from 'gias-kendo-grid';
import {enum_LAVCOD, enum_TipoOperazioneDB} from 'app/Model/TipiEnumerativi';
import { APP_Ricetta_Operazione, Ricetta_Operazione } from 'app/Service/Agenda/Agenda.service';
import {FormControl, FormGroup} from '@angular/forms';
import {HashMap} from "@jsverse/transloco/lib/types";


//export const TableQdCLink_Old = '/Agenda/MenuBS_WS.asmx/CaricaOperazioni';
export const TableQdCLink = 'Agenda/CaricaOperazioni';

export class ReadDataFilters {
    filtro: string;
    piva: string;
    objP_server: string;
    objP_utenti: string;
}

export class Filters {
    TipoGriglia: string;
    xFiltroAggiuntivo_colturali: string;
    txt_Data1: string;
    txt_Data2: string;
    flag_TerrenoNudo: boolean;
    sa_cod: string;
    veg_cod: string;
    mode: string;
    ricetta_cod: string;
    tipoOperazione: number[];
    impianti: string[];
    flag_NessunaSpecieQdC: boolean;

    constructor(params: Required<Filters>) {
        this.TipoGriglia = params.TipoGriglia;
        this.xFiltroAggiuntivo_colturali = params.xFiltroAggiuntivo_colturali;
        this.txt_Data1 = params.txt_Data1;
        this.txt_Data2 = params.txt_Data2;
        this.flag_TerrenoNudo = params.flag_TerrenoNudo;
        this.sa_cod = params.sa_cod;
        this.veg_cod = params.veg_cod;
        this.mode = params.mode;
        this.ricetta_cod = params.ricetta_cod;
        this.tipoOperazione = params.tipoOperazione;
        this.impianti = params.impianti;
        this.flag_NessunaSpecieQdC = params.flag_NessunaSpecieQdC;
    }
}

export class RicetteServerResult extends KendoServerResult {
    constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: RicettaRow[]) {
        super(model, columns,rows);
    }
};

export class BrogliaccioServerResult extends KendoServerResult {
    constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: KendoGridRow[]) {
        super(model, columns, rows);
    }
};


export type DDLs = (
    CentroItem[] | SpecieItem[] | OperazioneItem[] | ImpiantoItem[] | TipoVisitaItem[]
)[];


export function ValidValuesFor(specie: SpecieItem, centro: CentroItem): boolean {
    const veg_cod = specie.veg_cod;
    const sa_cod = centro.sa_cod;
    const invalidValues = ['0', ''];
    return invalidValues.includes(veg_cod) && invalidValues.includes(sa_cod);
}

export function GetOperazioniParams(objServer: string): OperazioniParams {
    const parametri = {
        objP_server: objServer,
        Flag_OpColturali: true,
        Flag_OpZoo: false,
        Flag_OpMacchine: false,
        Flag_OpContabili: true
    };
    return parametri;
}

export function GetCentriParams(obj_server: string, piva: string): CentriParams {
    return {
        'objP_server': obj_server,
        'PrimaRiga_Flag': false,
        'PrimaRiga_Text': '',
        'PrimaRiga_Value': '',
        'Piva': piva,
        'Flag_SoloCentriAttivi': false,
        'Tipo_Value': 2
    };
}

export function GetCentriParams_NG(obj_server: string, piva: string): CentriParams_NG {
    return {
        'PrimaRiga_Flag': false,
        'PrimaRiga_Text': '',
        'PrimaRiga_Value': '',
        'Piva': piva,
        'Flag_SoloCentriAttivi': false,
        'Tipo_Value': 2
    };
}


export function GetSpeciParams(obj_server: string, piva: string): SpeciParams {
    const sa_cod = 0;
    const data_da = new Date('01/01/1900');
    const data_a = new Date('12/31/2100');

    return {
        'PrimaRiga_Flag': false,
        'PrimaRiga_Text': '',
        'PrimaRiga_Value': '',
        'Piva': piva,
        'Sa_Cod': sa_cod,
        'Data_Da': data_da,
        'Data_A': data_a,
        'ConsideraTerrenoNudo': true,
        'leggiAncheBloccati': true
    };
}

/*const links_Old = [
    '/Agenda/Impianti.asmx/LeggiImpianti',
    '/Metaschema/Operazioni.asmx/Leggi_GruppoOperazioni',
    '/AgronicaCoreUtility/CaricaListControl.asmx/LeggiSpecieColtivate',
    '/Anagrafica/CentroAziendale.asmx/LeggiCentriConFiltroUtente'
];*/

const links = [
    'Agenda/LeggiImpianti',
    'MetaschemaNG/Leggi_GruppoOperazioni',
    'Anagrafica/LeggiSpecieVegetaliQdC',
    'AnagraficaNG/LeggiCentriConFiltroUtente'
];

export const IMPIANTI_LNK = links[0];
export const OPERAZIONI_LNK = links[1];
export const SPECI_LNK = links[2];
export const CENTRI_LNK = links[3];




export interface OperazioniParams {
    objP_server: string;
    Flag_OpColturali: boolean;
    Flag_OpZoo: boolean;
    Flag_OpMacchine: boolean;
    Flag_OpContabili: boolean;
}

export interface ImpiantiParams {
    piva: string;
    Sa_Cod: string;
    Veg_Cod: string;
    Data_Inizio: Date;
    Data_Fine: Date;
}

export interface SpeciParams {
    PrimaRiga_Flag: boolean;
    PrimaRiga_Text: string;
    PrimaRiga_Value: string;
    Piva: string;
    Sa_Cod: number;
    Data_Da: Date;
    Data_A: Date;
    ConsideraTerrenoNudo: boolean;
    leggiAncheBloccati: boolean;
}

export interface CentriParams {
    objP_server: string;
    PrimaRiga_Flag: boolean;
    PrimaRiga_Text: string;
    PrimaRiga_Value: string;
    Piva: string;
    Flag_SoloCentriAttivi: boolean;
    Tipo_Value: number;
}

export interface CentriParams_NG {
    PrimaRiga_Flag: boolean;
    PrimaRiga_Text: string;
    PrimaRiga_Value: string;
    Piva: string;
    Flag_SoloCentriAttivi: boolean;
    Tipo_Value: number;
}

export interface CentroItem { sa_nome: string; sa_cod: string }
export interface SpecieItem { veg_cod: string; veg_des: string }
export interface OperazioneItem { gru_cod: number; gru_des: string; tipo: string }
export interface ImpiantoItem { chiave: string; des: string }
export interface TipoVisitaItem {chiave: number; des: string}                       //aggiunta per le Visite


// export const lav_cod_non_editabili: number[] = [
//     LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
//     LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI];

// export const lav_cod_copiabili: number[] = [LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_DISTRIBUZIONE_INSETTI,
//     LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO,
//     LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
//     LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ARATURA,
//     LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE,
//     LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
//     LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE,
//     LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI,
//     LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE,
//     LAVCOD_PRESSATURA, LAVCOD__RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE, LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA,
//     LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO,
//     LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA,
//     LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_ALTRE_OPERAZIONI, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI];

export class RicettaForm {
    dataInizio: Date;
    dataFine: Date;
    numero: string;
    descrizione: string;
    nota: string;
}

export class CreateRecipeData {
    lblHtmlRicettaDaCreare: string;
    piva_ricetta: string;
    sa_cod: number;
    id_agenda: string;
    stesso_vag_cod: boolean;
    ErroreSingolaSpecie: string;
    data_operazione: string;

    ricettaForm: RicettaForm;

    prevent: boolean = false;

    constructor() {
        this.ricettaForm = new RicettaForm();
    }
}

export class GridMassiveEvent {
    items: KendoGridRow[];
    type: emun_MassiveEventType;
}

export enum emun_MassiveEventType {
    REMOVE,
    LOCK,
}

//export const RicetteNumLink_Old = '/Agenda/MenuBS_WS.asmx/ricetta_numero_default';
export const RicetteNumLink = 'Agenda/ricetta_numero_default';
//export const CreateRicettaLink_Old = '/Agenda/MenuBS_WS.asmx/crea_ricetta';
export const CreateRicettaLink = 'Agenda/crea_ricetta';
//export const CopiaOperazioneSingola_Old = '/Agenda/MenuBS_WS.asmx/CopiaOperazioneSingola';
export const CopiaOperazioneSingola = 'Agenda/CopiaOperazioneSingola'
//export const BloccaAttivitaAgendaLink_Old = '/Agenda/MenuBS_WS.asmx/BloccaAttivitaAgenda';
export const BloccaAttivitaAgendaLink = 'Agenda/BloccaAttivitaAgenda';
//export const SbloccaAttivitaAgendaLink_Old = '/Agenda/MenuBS_WS.asmx/SbloccaAttivitaAgenda';
export const SbloccaAttivitaAgendaLink = 'Agenda/SbloccaAttivitaAgenda';

export class RicettaModalParams {

}


export const lav_cod_non_editabili: number[] = [
    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
    LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI,
    enum_LAVCOD.CURA];

export const lav_cod_copiabili: number[] = [LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_DISTRIBUZIONE_INSETTI,
    LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO,
    LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
    LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ARATURA,
    LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE,
    LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
    LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE,
    LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI,
    LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE,
    LAVCOD_PRESSATURA, LAVCOD__RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE, LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA,
    LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO,
    LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA,
    LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_ALTRE_OPERAZIONI, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI,
    LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE, enum_LAVCOD.PASCOLAMENTO_PROPRIO, enum_LAVCOD.PASCOLAMENTO_TERZI
];


export enum TabTypes {
    QuadernoDiCampagna = 0,
    Ricette = 1,
    Brogliaccio = 2,
    GestioneTrappole = 3,
    Initializer = null
}

export enum RibaltamentoTypes {
    Nessuno = 0,
    Da_Brogliaccio_ad_Agenda = 1,
    Da_Ricetta_a_Brogliaccio = 2,
    Da_Ricetta_ad_Agenda = 3
}





export class RicettaRow extends KendoGridRow {
    App_Nome: string;
    APP_Ricetta_Operazione_ID: string;
    blocco_flag: string;
    Costi_Macchine: string;
    Costi_Operatori: string;
    Descrizione_Unica: string;
    Dettaglio_Tecnico: string;
    in_uso: string;
    Invia_App: string;
    lav_cod: number;
    lav_des: string;
    PermessoModifica: string;
    piva: string;
    Ricetta_Cod: number;
    Ricetta_Des: string;
    Ricetta_Numero: string;
    Ricetta_Operazione_Cod: number;
    Ricetta_Operazione_Data: Date;
    Sa_Cod: number;
    sa_nome: string;
    Sup_Trattata: number;
    Tipo_Ricetta_des: string;
    Tipo_Ricetta: number;
    Validita_Fine: Date;
    Validita_Inizio: Date;
    veg_cod_op: number;
    veg_cod_r: number;
    veg_des_unificato: string;
    WAnagraficaStati_Cod: number;
    WAnagraficaStati_Colore: string;
}

export class QdCRow extends KendoGridRow implements IChiaveCompositaPiva{
    Appezzamenti_Coinvolti: string;
    Avversita: string;
    blocco_flag: string;
    Centro_Aziendale: string;
    Centro_Campo: string;
    chiave_composita: string;
    contabilizzato: string;
    Costi_Macchine: string;
    Costi_Operatori: string;
    Creatore_Intervento: string;
    cul_Des: string;
    Data: Date;
    Data_Ultima_Modifica_Intervento: Date;
    Data2: Date;
    Descrizione_Unica: string;
    Dettaglio_Tecnico: string;
    gru_des: string;
    ID: string;
    id_agenda: string;
    id_mov_det: string;
    Lav_cod: string;
    Lav_Des: string;
    LottiImpianto: string;
    LottiProduzione: string;
    Note: string;
    Operazione_DES: string;
    PermessoModifica: boolean;
    Piva: string;
    Prodotti_Utilizzati: string;
    Rag_Soc: string;
    Ricetta_Cod: string;
    Ricetta_Des: string;
    sa_cod: string;
    Specie: string;
    Specie_Varieta: string;
    Sup_Trattata: number;
    tipo: string;
    Veg_cod: string;
}

// TODO(RV)
export interface BrogliaccioRow extends KendoGridRow
{
    App_Nome: string;
    APP_Ricetta_Operazione_ID: string;
    blocco_flag: string;
    Costi_Macchine: string;
    Costi_Operatori: string;
    Descrizione_Unica: string;
    Dettaglio_Tecnico: string;
    in_uso: string;
    lav_cod: number;
    lav_des: string;
    Origine: string;
    PermessoModifica: string;
    piva: string;
    Ricetta_Cod: number;
    Ricetta_Des: string;
    Ricetta_Numero: string;
    Ricetta_Operazione_Cod: number;
    Ricetta_Operazione_Data: string;
    Sa_Cod: number;
    sa_nome: string;
    Sup_Trattata: number;
    Tipo_Ricetta_des: string;
    Tipo_Ricetta: number;
    Validita_Fine: string;
    Validita_Inizio: string;
    veg_cod_op: number;
    veg_cod_r: number;
    veg_des_unificato: string;
    WAnagraficaStati_Cod: number;
    WAnagraficaStati_Colore: string;
}

// Errors
export const SLIGHTLY_DIFFERENT_CODE_FROM_GIAS2010 =
    "Manca la gestione di .btnCreaOp. Sembra di essere commentato e non utilizzato in GIAS2010.";
export const DA_GESTIRE_PUA = "PUA non gestito";
export const DA_GESTIRE_PIANO_DISTRIBUZIONE_CONCIMI = "non gestito";

export class ImpostazioniApp {
    ImportaSoloAziendaSelezionata: boolean;
    SincroDatiApp: boolean;
}


export function isQdCRow (x: any): x is QdCRow {
    return !!x.Centro_Aziendale && typeof x.Centro_Aziendale === 'string'
}

export function isBrogliaccioRow (x: any): x is BrogliaccioRow {
    return !!x.Origine && typeof x.Origine === 'string'
}

export class GridZooResult extends KendoServerResult {
    constructor(
        model: ZooModel,
        columns: KendoGridColumn[],
        rows: KendoGridRow[]) {
        super(model, columns, rows);
    }
};

export class ZooModel extends KendoGridModel {
    [value: string]: ModelEntry;
};

export class ZooRow extends KendoGridRow implements IChiaveCompositaPiva{
    id_mov_det: 147915;
    id_agenda: string;
    Data2: Date;
    Lav_Des: string;
    Prodotti_Utilizzati: string;
    Dettaglio_Tecnico: string;
    Centro_Aziendale: string;
    chiave_composita: string;
    tipo: string;
    Data: Date;
    contabilizzato: string;
    LottiProduzione: string;
    Note: string;
    Costi_Operatori: string;
    Costi_Macchine: string;
    Creatore_Intervento: string;
    ID: number;
    Lav_cod: string;
    blocco_flag: string;
    Piva: string;
    sa_cod: string;
    Operazione_DES: string;
    Prodotti: string;
    Rag_Soc: string;
    PermessoModifica: string;
    Descrizione_Unica: string;
}


//export const Link_ElimOpMultipla_Old = '/Agenda/MenuBS_WS.asmx/elimina_operazione_multipla';

export const Link_ElimOpMultipla = 'Agenda/EliminaOperazioneMultipla';

export interface IChiaveCompositaPiva {
  chiave_composita: string;
  Piva: string;
}

export function construisciChiaviComposite (rows: IChiaveCompositaPiva[])
{
    if(rows.length == 1) {
        let chiave = rows[0].chiave_composita + "_" + rows[0].Piva;
        return chiave;
    }
    else {
        let chiavi = "";
        for (var row of rows) {
            chiavi += row.chiave_composita + "_" + row.Piva + ',';
        }

        if (chiavi !== "") {
            //elimino l'ultima virgola
            chiavi = chiavi.slice(0, -1);
        }
        return chiavi;
    }
}

// grid commands
export enum enum_menuAgendaGridCommands {
    RICERCA_DOCUMENTI,
    NUOVO_ALLEGATO,
    VAI_AI_COSTI,
    COPIA,
    AGGIUNGI_RICETTA,
    STAMPA_RICETTA,
    STAMPA_ORDINE_LAVORO,
    BROGLIACCIO,
    QUADERNO_DI_CAMPAGNA,
    INFO,
    MODIFICA,
    CANCELLA,
    VAI_AL_PIANO_CONCIMAZIONE,
    IMPORTA_NEL_PUA,
    VAI_ALLA_VISITA,
    REINNESCO_TRAPPOLE
}

export interface IMenuAgendaGridCommand {
    action: enum_menuAgendaGridCommands;
    event: any;
    dataItem: any;
}

export class GridCommandItem {
    constructor(
        public actionName: string,
        public action: CommandsDropDownEvents | number,
        public iconClass?: string,
        public fontawesomeIcon?: any,
        public isStatic: boolean = true,
        public paramsforTrasloco: HashMap = null,
    ) {}
}

export class ModificaListaRicetteQdC {
    ListaRicette: Array<APP_Ricetta_Operazione>;
}
