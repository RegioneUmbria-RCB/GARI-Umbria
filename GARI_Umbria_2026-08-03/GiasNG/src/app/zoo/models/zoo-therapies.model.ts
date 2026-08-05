import { CELL_TYPES } from "gias-ui-kit";
import { ModelEntry } from "gias-kendo-grid";

// Modello per la griglia delle terapie zootecniche
export class ZooTherapiesGridFlatItem {
    Id_Terapia: number;
    Terapia_Des: string;
    Data: Date;
    Piva: string;
    Impresa: string;
    Sa_Cod: number;
    Centro: string;
    Sta_Num: number;
    Stalla: string;
    Interventi: string;
}

export const ZooTherapiesGridModel = {
    Id_Terapia: new ModelEntry(CELL_TYPES.NUMBER),
    Terapia_Des: new ModelEntry(CELL_TYPES.STRING),
    Data: new ModelEntry(CELL_TYPES.DATE),
    Piva: new ModelEntry(CELL_TYPES.STRING),
    Impresa: new ModelEntry(CELL_TYPES.STRING),
    Sa_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Centro: new ModelEntry(CELL_TYPES.STRING),
    Sta_Num: new ModelEntry(CELL_TYPES.NUMBER),
    Stalla: new ModelEntry(CELL_TYPES.STRING),
    Interventi: new ModelEntry(CELL_TYPES.STRING)
};

export class ZooTherapiesFilters {
    constructor(
        public Piva: string,
        public Sa_Cod: number,
        public Sta_Num: number,
        public Data: Date
    ) {}
}

// Modello per il dettaglio della terapia zootecnica
export class ZooTherapy {
    Id_Terapia: number;
    Piva: string;
    Sa_Cod: number;
    Sta_Num: number;
    Descrizione: string;
    Interventi: ZooIntervention[];
}

export class ZooIntervention {
    Id_Intervento: number;
    Descrizione: string;
    Ordine: number;
    Protocolli: ZooProtocolxIntervento[];
}

export class ZooProtocolxIntervento {
    Id_Protocollo: number;
    Id_Protocollo_Alt: number;
}
