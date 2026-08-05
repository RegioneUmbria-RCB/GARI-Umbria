import { KendoGridColumn, KendoGridModel, KendoServerResult, ModelEntry } from 'gias-kendo-grid';
import { KendoCentroRow } from "app/anagrafica/centri/centri.models";

export class ValutazioniModel extends KendoGridModel {
    Rag_Soc: ModelEntry;
    Piva: ModelEntry;
    Id_Testata: ModelEntry;
    Data_Redazione: ModelEntry;
    Valutazione_Piano_Des: ModelEntry;
    Anno1: ModelEntry;
    Anno1_Tipo_Des: ModelEntry;
    Anno2: ModelEntry;
    Anno2_Tipo_Des: ModelEntry;
    Anno3: ModelEntry;
    Anno3_Tipo_Des: ModelEntry;
    Note: ModelEntry;
}

export class ValutazioniiWrapper {
    valutazioni: any[];

    constructor(opts: Required<ValutazioniiWrapper>) {
        this.valutazioni = opts.valutazioni;
    }
}

export class ValutazioniKendoServerResult extends KendoServerResult {
    constructor(public model: ValutazioniModel,
        public columns: KendoGridColumn[],
        public rows: KendoCentroRow[]) {
        super(model, columns, rows);
    }
}

export class StatoPatrimonialeModel extends KendoGridModel {
    Valutazione_Conto_Des: ModelEntry;
    Dettaglio_Key: ModelEntry;
    Attivo_Passivo_Des: ModelEntry;
    Valutazione_Gruppo_Des: ModelEntry;
    Valutazione_Sezione_Des: ModelEntry;
}

export class StatoPatrimonialeWrapper {
    statoPatrimoniale: any[];

    constructor(opts: Required<StatoPatrimonialeWrapper>) {
        this.statoPatrimoniale = opts.statoPatrimoniale;
    }
}

export class StatoPatrimonialeKendoServerResult extends KendoServerResult {
    constructor(public model: StatoPatrimonialeModel,
        public columns: KendoGridColumn[],
        public rows: KendoCentroRow[]) {
        super(model, columns, rows);
    }
}

export class ContoEconomicoModel extends KendoGridModel {
    Valutazione_Conto_Des: ModelEntry;
    Dettaglio_Key: ModelEntry;
    Attivo_Passivo_Des: ModelEntry;
    Valutazione_Gruppo_Des: ModelEntry;
    Valutazione_Sezione_Des: ModelEntry;
    Dettaglio_Specifico : ModelEntry;
}

export class ContoEconomicoWrapper {
    contoEconomico: any[];

    constructor(opts: Required<ContoEconomicoWrapper>) {
        this.contoEconomico = opts.contoEconomico;
    }
}

export class ContoEconomicoKendoServerResult extends KendoServerResult {
    constructor(public model: ContoEconomicoModel,
        public columns: KendoGridColumn[],
        public rows: KendoCentroRow[]) {
        super(model, columns, rows);
    }
}

export class AnniModel extends KendoGridModel {
    id: ModelEntry;
    name: ModelEntry;
    id_: ModelEntry;
    tipo_: ModelEntry;
}

export class AnniKendoServerResult extends KendoServerResult {
    constructor(public model: AnniModel,
        public columns: KendoGridColumn[],
        public rows: KendoAnniRow[]) {
        super(model, columns, rows);
    }
}

export class KendoAnniRow {

    constructor(id: string, name: string, id_: string, tipo_: string) {
        this.id = id;
        this.name = name;
        this.id_ = id_;
        this.tipo_ = tipo_;
    }

    id: string;
    name: string;

    id_: string;
    tipo_: string;
}


/*Dettaglio specifico */
export class DettaglioSpecificoModel extends KendoGridModel {
    Dettaglio_Key: ModelEntry;
    Descrizione: ModelEntry;
}

export class DettaglioSpecificoWrapper {
    dettaglioSpecifico: any[];

    constructor(opts: Required<DettaglioSpecificoWrapper>) {
        this.dettaglioSpecifico = opts.dettaglioSpecifico;
    }
}

export class DettaglioSpecificoServerResult extends KendoServerResult {
    constructor(public model: DettaglioSpecificoModel,
        public columns: KendoGridColumn[],
        public rows: KendoAnniRow[]) {
        super(model, columns, rows);
    }
}
