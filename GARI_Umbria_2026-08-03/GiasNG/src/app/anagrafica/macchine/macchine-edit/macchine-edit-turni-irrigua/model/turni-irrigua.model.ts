import { KendoServerResult } from "gias-kendo-grid";
import { KendoGridColumn, ModelEntry, NumericSettings } from "gias-kendo-grid";

export class TurniIrriguaServerResult extends KendoServerResult {
    constructor(public model, public columns, public rows) {
        super(model, columns, rows);
    }
}