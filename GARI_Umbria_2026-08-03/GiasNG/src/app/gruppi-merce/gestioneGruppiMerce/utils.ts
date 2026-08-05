import { Validators } from '@angular/forms';
import {  KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';


export const USE_MOCK_DATA = false;

/**
 * Edit gruppi merce dtos.
 */

export class GruppiMerceServerResult extends KendoServerResult {
    constructor(public model: GruppiMerceGridModel,
        public columns: KendoGridColumn[],
        public rows: GruppiMerceGridRow[]) {
        super(model, columns, rows);
    }
}
export class GruppiMerceGridModel extends KendoGridModel {
    Piva: ModelEntry;
    RagioneSociale: ModelEntry;
    Sa_Cod: ModelEntry;
    Id_Gruppo_Merce: ModelEntry;
    Codice: ModelEntry;
    Descrizione: ModelEntry;
    Data_Creazione: ModelEntry;
    Data_Modifica: ModelEntry;
    Username_Creazione: ModelEntry;
    Username_Modifica: ModelEntry;
    Validita_Inzio: ModelEntry;
    Validita_Fine: ModelEntry;
}

export class GruppiMerceGridRow extends KendoGridRow {
    RagioneSociale: string;
    Sa_Cod_Desc: string;

    Piva_SuperUser: string;
    Piva: string;
    Sa_Cod: number;
    Id_Gruppo_Merce: number;
    Codice: string;
    Descrizione: string;
    inviato: number;
    datainvio: Date;
    Data_Creazione: Date;
    Data_Modifica: Date;
    Username_Creazione: string;
    Username_Modifica: string;
    Validita_Inzio: Date;
    Validita_Fine: Date;
}

export const GruppiMerceModel: GruppiMerceGridModel = {
    Piva: new ModelEntry(CELL_TYPES.STRING),
    RagioneSociale: new ModelEntry(CELL_TYPES.STRING),
    Sa_Cod: new ModelEntry(CELL_TYPES.DROPDOWNLIST),
    Sa_Cod_Desc: new ModelEntry(CELL_TYPES.STRING),
    Id_Gruppo_Merce: new ModelEntry(CELL_TYPES.NUMBER),
    Codice: new ModelEntry(CELL_TYPES.STRING),
    Descrizione: new ModelEntry(CELL_TYPES.STRING),
    Data_Creazione: new ModelEntry(CELL_TYPES.DATETIME),
    Data_Modifica: new ModelEntry(CELL_TYPES.DATETIME),
    Username_Creazione: new ModelEntry(CELL_TYPES.STRING),
    Username_Modifica: new ModelEntry(CELL_TYPES.STRING),
    Validita_Inzio: new ModelEntry(CELL_TYPES.DATE),
    Validita_Fine: new ModelEntry(CELL_TYPES.DATE),
}

export const GruppiMerceRows: GruppiMerceGridRow[] = [];

export enum PublicServices {
    EditGruppiMerce,
}


export const EditGrMerceLinks = {
    //CARICA_MERCE_LINK_OLD: "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiGruppiMerceAnagrafica",
    CARICA_MERCE_LINK: "AgronicaCoreUtentiBIZ/LeggiGruppiMerceAnagrafica",

    // Verifica se ci sono dei conflitti con la tabella dei permessi
    //TENTATIVO_CANCELLAZIONE_GRUPPO_MERCE_OLD: "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/TentativoCancellazioneGruppoMerce",
    TENTATIVO_CANCELLAZIONE_GRUPPO_MERCE: "AgronicaCoreUtentiBIZ/TentativoCancellazioneGruppoMerce",

    //AGGIUNGI_MODIFICA_GRUPPO_MERCE_OLD: "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/AggiungiOModificaGruppoMerce",
    AGGIUNGI_MODIFICA_GRUPPO_MERCE: "AgronicaCoreUtentiBIZ/AggiungiOModificaGruppoMerce"

};


/**
 * Tipi di risposte:
 *  - Tutte le verifiche passate, successo.
 *  - Ci sono conflitti con la tabella dei permessi, richiedere all'utente la conferma.
 *  - Ci sono conflitti con le tabelle Prodotti_Extra_Privata, Imprese_Impostazioni
 *    mostra messaggio impossibilità di cancellazione.
 * Verifiche delle tabelle: GruppiUtentePerGruppiMerce, Prodotti_Extra_Privata,
 *                          Imprese_Impostazioni.
 */
export class RispostaTentativoCancellazione
{
    Risultato: RisultatoCancellazione;
    NumberConflicts: number;
}

export enum RisultatoCancellazione
{
    SUCCESS = 0,
    RICHIEDE_CONFERMA_CANCELLAZIONE = 1,
    FAILURE = 2,
}

export const NESSUNA_IMPRESA_SELEZIONATA = "";


export enum InsertionErrors {
    CodiceDuplicato = "1",
    GruppoUtilizzatoDaUnProdottoExtra = "2",
    UtilizzatoDaUnAltraImpresaComeGruppoMerceDefault = "3"
}
