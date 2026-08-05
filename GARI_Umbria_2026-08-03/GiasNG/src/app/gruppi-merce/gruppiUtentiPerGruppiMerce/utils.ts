import { DropdownListItem, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, ModelEntry, } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';

/*
 * Imprese dtos.
 */

export class ImpreseDDLCtrlObj {
    imprese: DropdownListItem[];
    source: DropdownListItem[];
    defaultItem: DropdownListItem;
    selected: DropdownListItem;
    loading: boolean;
    modificaAbilitata: boolean;

    constructor(opts: Partial<ImpreseDDLCtrlObj>) {
        this.imprese = opts?.imprese ?? [];
        this.source = opts?.imprese ?? [];
        this.defaultItem = opts?.defaultItem ?? null;
        this.selected = opts?.selected ?? null;
        this.loading = opts?.loading ?? false;
    }

    voceSelezionataMostraTutto() {
        return this.selected.id === MOSTRA_TUTTO;
    }
}

export class GruppiPanelCtrl {

    disabled: boolean;
    expanded: boolean;
    shown: boolean;

    constructor(opts: Partial<GruppiPanelCtrl>) {
        this.disabled = opts?.disabled ?? true;
        this.expanded = opts?.expanded ?? false;
        this.shown = opts?.shown ?? false;
    }

    changeStateIfNeeded(mostraTutto: boolean) {
        if ((this.disabled && mostraTutto) || (!this.disabled && !mostraTutto))
            return;

        this.disabled = !this.disabled;
        this.expanded = !this.expanded;
        this.shown = !this.shown;
    }
}

export const MOSTRA_TUTTO: string = "";

export class CheckboxMostraTutteLeImpreseCtrlObj {

}

/** 
 * Gruppi utenti dtos.
 */

export class GruppiUtentiServerResult extends KendoServerResult {
    constructor(public model: GruppiUtentiGridModel,
        public columns: KendoGridColumn[],
        public rows: GruppiUtentiGridRow[]) {
        super(model, columns, rows);
    }
}
export class GruppiUtentiGridModel extends KendoGridModel {
    Gruppi_Utente_cod: ModelEntry;
    Gruppi_Utente_des: ModelEntry;
}

export class GruppiUtentiGridRow extends KendoGridRow {
    Gruppi_Utente_cod: number;
    Gruppi_Utente_des: string;
}

export const GruppiUtentiModel: GruppiUtentiGridModel = {
    Gruppi_Utente_cod: new ModelEntry(CELL_TYPES.NUMBER),
    Gruppi_Utente_des: new ModelEntry(CELL_TYPES.STRING)
}

export const GruppiUtentiRows: GruppiUtentiGridRow[] = [
    { Gruppi_Utente_cod: 1, Gruppi_Utente_des: 'Compilatori' },
    { Gruppi_Utente_cod: 2, Gruppi_Utente_des: 'Supervisori' }
];

/** 
 * Gruppi merce dtos.
 */

export class GruppiMerceServerResult extends KendoServerResult {
    constructor(public model: GruppiMerceGridModel,
        public columns: KendoGridColumn[],
        public rows: GruppiMerceGridRow[]) {
        super(model, columns, rows);
    }
}
export class GruppiMerceGridModel extends KendoGridModel {
    Codice: ModelEntry;
    Descrizione: ModelEntry;
    Id_Gruppo_Merce: ModelEntry;
    Piva: ModelEntry;
    Sa_cod: ModelEntry;
}

export class GruppiMerceGridRow extends KendoGridRow {
    Codice: number;
    Descrizione: string;
    Id_Gruppo_Merce: number;
    Piva: string;
    Sa_cod: number;
}

export const GruppiMerceModel: GruppiMerceGridModel = {
    Codice: new ModelEntry(CELL_TYPES.NUMBER),
    Descrizione: new ModelEntry(CELL_TYPES.STRING),
    Id_Gruppo_Merce: new ModelEntry(CELL_TYPES.NUMBER),
    Piva: new ModelEntry(CELL_TYPES.STRING),
    Sa_cod: new ModelEntry(CELL_TYPES.NUMBER)
}

export const GruppiMerceRows: GruppiMerceGridRow[] = [
    { Codice: 1, Descrizione: 'Merce1', Id_Gruppo_Merce: 1, Piva: 'p1', Sa_cod: 1 },
    { Codice: 2, Descrizione: 'Merce2', Id_Gruppo_Merce: 2, Piva: 'p2', Sa_cod: 2 }
];

/** 
 * Gruppi utentix gruppi merce dtos.
 */

export class GruppiUtentixGruppiMerceServerResult extends KendoServerResult {
    constructor(public model: GruppiUtentixGruppiMerceGridModel,
        public columns: KendoGridColumn[],
        public rows: GruppiUtentixGruppiMerceGridRow[]) {
        super(model, columns, rows);
    }
}
export class GruppiUtentixGruppiMerceGridModel extends KendoGridModel {
    DescrizioneGruppoMerce: ModelEntry;
    CodiceGruppoMerce: ModelEntry;
    Gruppi_Utente_des: ModelEntry;
    Gruppi_Utente_cod: ModelEntry;
    Id_Gruppo_Merce: ModelEntry;
    rag_soc: ModelEntry;
    Piva: ModelEntry;
}

export class GruppiUtentixGruppiMerceGridRow extends KendoGridRow {
    DescrizioneGruppoMerce: string;
    CodiceGruppoMerce: string;
    Gruppi_Utente_des: string;
    Gruppi_Utente_cod: number;
    Id_Gruppo_Merce: number;
    rag_soc: string
    Piva: string;
}

export const GruppiUtentixGruppiMerceModel: GruppiUtentixGruppiMerceGridModel = {
    DescrizioneGruppoMerce: new ModelEntry(CELL_TYPES.STRING),
    Gruppi_Utente_des: new ModelEntry(CELL_TYPES.STRING),
    Gruppi_Utente_cod: new ModelEntry(CELL_TYPES.NUMBER),
    Id_Gruppo_Merce: new ModelEntry(CELL_TYPES.NUMBER),
    rag_soc: new ModelEntry(CELL_TYPES.STRING),
    Piva: new ModelEntry(CELL_TYPES.STRING),
    CodiceGruppoMerce: new ModelEntry(CELL_TYPES.STRING)
}

export const GruppiUtentixGruppiMerceRows: GruppiUtentixGruppiMerceGridRow[] = [
    { CodiceGruppoMerce: "Test1", DescrizioneGruppoMerce: "GruppoMerce1", Gruppi_Utente_des: "GruppoUtente1", Gruppi_Utente_cod: 1, Id_Gruppo_Merce: 1, Piva: "test", rag_soc: "r1" },
    { CodiceGruppoMerce: "Test2", DescrizioneGruppoMerce: "GruppoMerce2", Gruppi_Utente_des: "GruppoUtente2", Gruppi_Utente_cod: 2, Id_Gruppo_Merce: 2, Piva: "test", rag_soc: "r2" }
];

/* Classe che utilizzo per mostrare o meno i messaggi transloco */
export class TranslocoException {
    public dataAlreadyInitialized: boolean = false;
    public impresaSelezionataCambiata: boolean = false;
    public datiSonoStatiAppenaCancellati: boolean = false;
    public mostraTutteLeAziendeCallback: boolean = false;

    showTranslocoMessage() {
        return this.dataAlreadyInitialized && !this.impresaSelezionataCambiata && !this.datiSonoStatiAppenaCancellati && !this.mostraTutteLeAziendeCallback;
    }

    feedEmptyData() {
        return !this.dataAlreadyInitialized || this.impresaSelezionataCambiata || this.datiSonoStatiAppenaCancellati || this.mostraTutteLeAziendeCallback;
    }

    resetAllFlags() {
        this.datiSonoStatiAppenaCancellati = false;
        this.impresaSelezionataCambiata = false;
        this.dataAlreadyInitialized = true;
        this.mostraTutteLeAziendeCallback = false;
    }

}

/**
 * Other utilities
 */

export const USE_MOCK_DATA = false;

export const GrUtPerGrMerceLinks = {
    //GRUPPI_UTENTI_LINK_OLD: "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiGruppiUtenti",
    GRUPPI_UTENTI_LINK: "AgronicaCoreUtentiBIZ/LeggiGruppiUtenti",
    //GRUPPI_MERCE_LINK_OLD: "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiGruppiMerce",
    GRUPPI_MERCE_LINK: "AgronicaCoreUtentiBIZ/LeggiGruppiMerce",
    //GRUPPI_UTENTIMERCE_LINK_OLD:  "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/ScriviGruppiUtentiPerGruppiMerce",
    GRUPPI_UTENTIMERCE_LINK: "AgronicaCoreUtentiBIZ/ScriviGruppiUtentiPerGruppiMerce",
    CANCELLA_PERMESSO: "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/CancellaPermesso"
};

export enum PublicServices {
    GruppiUtenti,
    GruppiMerce,
    GruppiUtentiPerGruppiMerce,
}

export class RigheSelezionate {
    GruppiUtenti: GruppiUtentiGridRow[];
    GruppiMerce: GruppiMerceGridRow[];
    MustStillBeInitialized?: boolean;

    constructor(opts: Partial<RigheSelezionate>) {
        this.GruppiMerce = opts.GruppiMerce ?? [];
        this.GruppiUtenti = opts.GruppiUtenti ?? [];
        this.MustStillBeInitialized = opts.MustStillBeInitialized ?? false;
    }
}

export class ScriviPermessiResult {
    public DT: GruppiUtentixGruppiMerceGridRow[];
    public NumElementiInseriti: number;
}

export class ScrivPermessiHttpDto {
    Gruppi_Utente_codici: number[];
    Ids_Gruppo_Merce: number[];

    constructor() {
        this.Gruppi_Utente_codici = [];
        this.Ids_Gruppo_Merce = [];
    }
}

export class CancellaPermessiHttpDto {
    permessi: GruppiUtentixGruppiMerceGridRow[];
    objP_Server: string;
    objP_Utenti: string;
}
