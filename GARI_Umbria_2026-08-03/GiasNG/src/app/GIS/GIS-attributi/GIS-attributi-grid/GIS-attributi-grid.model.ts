import { KendoServerResult, KendoGridModel, ModelEntry } from 'gias-kendo-grid';

export class GISAttributiKendoServerResult extends KendoServerResult {
    constructor(public model, public columns, public rows) {
        super(model, columns, rows);
    }
}

export class KendoGISAppuntiModel extends KendoGridModel {
    chiave: ModelEntry;
    Descrizione: ModelEntry;
}
