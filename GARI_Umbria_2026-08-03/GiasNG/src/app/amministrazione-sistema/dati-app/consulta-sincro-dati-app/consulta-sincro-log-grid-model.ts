import { KendoGridModel, ModelEntry, KendoServerResult } from 'gias-kendo-grid';

export class ConsultaSincroLogKendoServerResult extends KendoServerResult {
    constructor(public model, public columns, public rows) {
        super(model, columns, rows);
    }
}
