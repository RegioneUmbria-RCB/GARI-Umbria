import { GridDataResult } from '@progress/kendo-angular-grid';
import { IScrollingMode } from '../interfaces/scrollingMode.interface';
import { KendoGridRow, KendoServerResult } from '../models/grid.model';
import { process, State } from '@progress/kendo-data-query';
import { PaginationSettings, ScrollingMode } from '../models/configuration.model';
import { Shared } from './shared.service';
import { VirtualScrollingService } from './virtualScrolling.service';


export class ScrollableScrollingOpts {
    pagination: PaginationSettings;
}

export class ScrollableScrollingService implements IScrollingMode {
    public rowHeight = 0;
    public mode: ScrollingMode = ScrollingMode.Scrollable;
    private pagination: PaginationSettings;
    private _totalCount: number = 0;

    constructor(opts: ScrollableScrollingOpts) {
        this.pagination = opts.pagination;
    }

    get pageSize(): number {
        return this.gridState.take;
    }

    get totalCount(): number {
        return this._totalCount;
    }

    private get gridState() {
        return this.pagination.gridState;
    }

    public getRowHeight(): number {
        return 0;
    }

    public isVirtual(): this is VirtualScrollingService {
        return false;
    }

    public loadData(data: KendoServerResult): GridDataResult {
        const result = process(data.rows ?? [], this.computeGridStateBasedOn(data.rows));
        this._totalCount = result.total;
        return result;
    }

    private computeGridStateBasedOn(rows: KendoGridRow[]): State {
        if (this.gridState.skip > rows.length)
            this.gridState.skip = Shared.changeGridPage(0, this.gridState.take);
        return this.gridState;
    }
}
