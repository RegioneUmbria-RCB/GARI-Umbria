import { KendoGridColumn, KendoGridModel, KendoServerResult } from 'gias-kendo-grid';

export class ContattiEditGridModel extends KendoGridModel {
    // TODO: Aggiungere le proprietà
}

export class ContattiEditGridRow {

}

export class ContattiEditServerResult extends KendoServerResult {
    constructor(public model: ContattiEditGridModel,
        public columns: KendoGridColumn[],
        public rows: ContattiEditGridRow[]) {
        super(model, columns, rows);
    }
}
