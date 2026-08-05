import { KendoServerResult } from "../models/grid.model";

export class GridDataWithFilter {
    stopPropagation?: boolean;
    data: KendoServerResult;
    forceRefresh: boolean = false;
    afterEdit: boolean = false;

    constructor(opts: Partial<GridDataWithFilter>) {
        this.stopPropagation = opts.stopPropagation ?? false;
        this.forceRefresh = opts.forceRefresh ?? false;
        this.afterEdit = opts.afterEdit ?? false;
        this.data = opts.data;
    }
}