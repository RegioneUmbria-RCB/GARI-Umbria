import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { isNullOrUndefined } from '../../../shared/utils';
import { map, Observable } from 'rxjs';
import { IScrollingMode } from '../interfaces/scrollingMode.interface';
import { GeneralSettings, PaginationSettings, ScrollingMode } from '../models/configuration.model';
import { KendoGridRow, KendoServerResult } from '../models/grid.model';
import { KendoGridService } from '../services/kendo-grid.service';
import {
    process, State
} from '@progress/kendo-data-query';
import { Shared } from './shared.service';

export interface VirtualScrollingParams {
    generalSettings: GeneralSettings;
    pagination: PaginationSettings;
    gridPrivate: KendoGridService;
}
export class VirtualScrollingService implements IScrollingMode {

    public mode: ScrollingMode = ScrollingMode.Virtual;
    public rowHeight: number; // will be computed

    private _pageSize; // will be computed
    private _totalCount = 0; // will be computed

    private pagination: PaginationSettings;
    private gridPrivate: KendoGridService;


    constructor(opts: VirtualScrollingParams) {
        this.pagination = opts.pagination;
        this.gridPrivate = opts.gridPrivate;

        this.calculateRequiredFields();
    }

    public get pageSize(): number {
        return this._pageSize;
    }

    public get totalCount() {
        return this._totalCount;
    }

    private get gridState() {
        return this.pagination.gridState;
    }

    private get settings() {
        return this.pagination.virtualScrolling;
    }

    private get skip() {
        return this.pagination.gridState.skip;
    }

    private set skip(value: number) {
        // this.skip = value;
        this.pagination.gridState.skip = value;
    }

    public isVirtual(): this is VirtualScrollingService {
        return true;
    }

    public calculateRequiredFields() {
        if (isNullOrUndefined(this.settings?.viewportHeight) ||
            isNullOrUndefined(this.settings?.rowHeight))
            throw Error('Required settings used to perform calculations were not provided.');

        const gridViewport = Math.ceil(this.settings.viewportHeight);
        this.rowHeight = Math.ceil(this.settings.rowHeight);
        this._pageSize = Math.ceil(3 * (gridViewport / this.rowHeight));
    }

    public pageChange(event: PageChangeEvent): Observable<GridDataResult> {
        this.pagination.gridState.skip = event.skip;
        return this.gridPrivate.pipe(map((kdata) => this.loadData(kdata)));
    }

    public loadData(data: KendoServerResult): GridDataResult {
        const state = { ...this.computeGridStateBasedOn(data.rows) };
        state.take = this._pageSize;

        const result = process(data.rows, state)
        this._totalCount = result.total;
        return result;
    }

    private computeGridStateBasedOn(rows: KendoGridRow[]): State {
        if (this.gridState.skip > rows.length)
            this.gridState.skip = Shared.changeGridPage(0, this.gridState.take);
        return this.gridState;
    }

}
