import { KendoServerResult } from 'gias-kendo-grid';


export class GridServerResult extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}
