import { FilterService } from "@progress/kendo-angular-grid";
import { IGridWorkerService } from "../../../../grid-worker.interface";
import { BehaviorSubject, Subject } from "rxjs";
import { GridDataWithFilter } from "../../services/grid-data-with-filter";

export interface IMultiCheckFilter {
    isPrimitive: boolean;
    currentFilter: any;
    textField;
    valueField;
    filterService: FilterService;
    field: string;
    multiIndex: number;
    showHTMLAsString: boolean;
    grid: BehaviorSubject<GridDataWithFilter>;
    gridWorker: IGridWorkerService
    shutdownPagination: Subject<void>;
    ordering: (rows: any) => any | null;
}