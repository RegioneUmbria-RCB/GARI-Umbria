import { State } from '@progress/kendo-data-query';
import { DropdownList, DropdownListItem, KendoGridColumn } from '../../models/grid.model';


const throwError = (message) => { throw new Error(message) };

export class ViewBase {
    IdVista: string;
    GridId: string;
    NomeUtente: string;
    Predefinita: boolean;
    NomeVista: string;
    FiltroJSON: string | null;
    FlagPubblica: boolean;

    constructor(options?: Partial<ViewBase>) {
        this.IdVista = options?.IdVista;
        this.NomeUtente = options?.NomeUtente || 'null';
        this.Predefinita = options?.Predefinita ?? false;
        this.NomeVista = options?.NomeVista || '';
        this.GridId = options?.GridId ?? throwError('Grid Id è obbligatorio.');
        this.Predefinita = options?.Predefinita ?? false;
        this.FiltroJSON = options?.FiltroJSON || "";
        this.FlagPubblica = options?.FlagPubblica || false;
    }
}

export class DropdownChanged {
    constructor(
        public dropdown: DropdownList,
        public selected: DropdownListItem) { }
}

export class ServerView extends ViewBase {
    Colonne: string;
    Stato: string;

    constructor(options?: Partial<ServerView>) {
        super(options);
        this.Colonne = options?.Colonne || JSON.stringify({});
        this.Stato = options?.Stato || JSON.stringify({});
    }
}

export class SelectionChangeEvent {
    item: DropdownListItem;
    isNew: boolean;

    constructor(options: Required<SelectionChangeEvent>) {
        this.isNew = options?.isNew;
        this.item = options?.item;
    }
}

export class InMemoryView extends ViewBase {
    Colonne: KendoGridColumn[];
    Stato: State;
    IsModified: boolean;
    IsNew: boolean;

    constructor(options?: InMemoryView) {
        super(options);
        this.Colonne = options?.Colonne || null;
        this.Stato = options?.Stato || null;
        this.IsModified = options?.IsModified;
        this.IsNew = options?.IsNew;
    }
}

export class ChiaveVista {
    constructor(
        public GridId: string,
        public IdVista: string,
        public NomeUtente: string
    ) { }
}

export interface ColumnSettings {
    field: string;
    title?: string;
    filter?: 'text' | 'numeric' | 'date' | 'boolean' | 'datetime';
    format?: string;
    width?: number;
    filterable?: boolean;
    orderIndex?: number;
    hidden?: boolean;
}



export class SalvaVisteWrapper {
    VisteNuove: ServerView[];
    VisteModificate: ServerView[];

    constructor(options: Required<SalvaVisteWrapper>) {
        this.VisteNuove = options.VisteNuove;
        this.VisteModificate = options.VisteModificate;
    }
}

export class CustomizationRequest {
    dropdownItem: DropdownListItem;
    isNewView: boolean;
}
