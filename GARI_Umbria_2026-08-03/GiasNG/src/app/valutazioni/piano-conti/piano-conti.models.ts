import { KendoGridColumn, KendoGridModel, KendoServerResult, ModelEntry } from 'gias-kendo-grid';
import { KendoCentroRow } from "app/anagrafica/centri/centri.models";

export class PianoContiModel extends KendoGridModel {
    Rag_Soc: ModelEntry;
    Piva: ModelEntry;
    Valutazione_Piano_Des : ModelEntry;
    Valutazione_Piano_Cod : ModelEntry;
    Num_Valutazioni_Associate : ModelEntry;

}

export class PianoContiWrapper {
    pianoConti: any[];

    constructor(opts: Required<PianoContiWrapper>) {
        this.pianoConti = opts.pianoConti;
    }
}

export class PianoContiKendoServerResult extends KendoServerResult {
    constructor(public model: PianoContiModel,
        public columns: KendoGridColumn[],
        public rows: KendoCentroRow[]) {
        super(model, columns, rows);
    }
}
