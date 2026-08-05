import {
    ChangeDetectionStrategy,
    ChangeDetectorRef,
    Component,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output,
    ViewEncapsulation
} from '@angular/core';
import { FilterService } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { GridPublicService } from '../../services/grid-public.service';
import { GridWorkerService } from '../../../../grid-worker.service';
import { StringManager } from './managers/string-manager';
import { DropdownManager } from './managers/dropdown-manager';
import { MultiDropdownManager } from './managers/multi-dropdown-manager';
import { DateManager } from './managers/date-manager';
import { NumberManager } from './managers/number-manager';
import { IMultiCheckFilter } from './multicheck-filter';
import { IManager } from './managers/manager';
import { DateFilters } from './utils';
import { CELL_TYPES } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'gias-multicheck-filter',
    templateUrl: './multicheck-filter.component.html',
    styleUrls: ['./multicheck-filter.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    encapsulation: ViewEncapsulation.None,
})
export class MultiCheckFilterComponent implements IMultiCheckFilter, OnInit, OnDestroy {
    @Input() public type: CELL_TYPES;

    readonly STRING: string = CELL_TYPES.STRING;
    readonly DATE: string = CELL_TYPES.DATE;
    readonly DATETIME: string = CELL_TYPES.DATETIME;
    readonly DROPDOWNLIST: string = CELL_TYPES.DROPDOWNLIST;
    readonly MULTI_DROPDOWNLIST: string = CELL_TYPES.MULTI_DROPDOWNLIST;
    readonly NUMBER: string = CELL_TYPES.NUMBER;

    @Input() public isPrimitive: boolean;
    @Input() public currentFilter: any;
    @Input() public textField;
    @Input() public valueField;
    @Input() public filterService: FilterService;
    @Input() public field: string;
    @Input() public multiIndex: number;
    @Input() public showHTMLAsString: boolean;
    @Input() public ordering: (rows: any) => any | null = null;
    @Output() public valueChange = new EventEmitter<number[]>();

    shutdownPagination: Subject<void> = new Subject();

    public showFilter = true;
    public grid: GridPublicService;

    filterText = '';
    searchDisabled = true;
    signal = new Subject<void>();
    manager: IManager;

    constructor(
        private changeDetector: ChangeDetectorRef,
        public grids: GridPublicService,
        public gridWorker: GridWorkerService
    ) {
    }

    ngOnInit(): void {
        if (this.multiIndex != null && Array.isArray(this.grids)) {
            this.grid = this.grids.find(s => s.multiIndex === this.multiIndex);
        } else {
            this.grid = this.grids;
        }

        // Setting up the manager.
        if (this.type === CELL_TYPES.STRING) {
            this.manager = new StringManager(this);
        } else if (this.type === CELL_TYPES.DROPDOWNLIST) {
            this.manager = new DropdownManager(this);
        } else if (this.type === CELL_TYPES.MULTI_DROPDOWNLIST) {
            this.manager = new MultiDropdownManager(this);
        } else if (this.type === CELL_TYPES.DATE) {
            this.manager = new DateManager(this);
        } else if (this.type === CELL_TYPES.DATETIME) {
            this.manager = new DateManager(this);
        } else if (this.type === CELL_TYPES.NUMBER) {
            this.manager = new NumberManager(this);
        }

        if (!this.gridWorker.alreadySubed) {
            this.gridWorker.refreshWorker();
        }

        this.serveWorkerData();
    }

    paginationEnabled = false;

    serveWorkerData() {
        const store = this.manager.store;

        if (store[this.field] == null) {
            // Scatta l'evento quando il worker finisce lavoro.
            this.gridWorker.workLoad.pipe(takeUntil(this.signal))
                .subscribe(_ => {
                    this.searchDisabled = false;

                    this.manager.distinctRowsLoaded();

                    // Nel caso in cui ci sono pocchi risultati, possiamo farli comparire
                    // tutti appena sono pronti.
                    this.manager.showAllDataIfBelowThreshold();
                    this.changeDetector.detectChanges();
                });

            this.gridWorker.runTaskDistinct(
                this.manager.allRows,
                this.field,
                this.type,
                this.manager.getColumn(),
                this.ordering == null);

            this.paginationEnabled = true;
        } else {
            // Lavoro è stato già compiuto.
            this.searchDisabled = false;
            this.manager.initDataNoPagination();
            this.changeDetector.detectChanges();
        }
    }

    public onSelectAll() {
        this.manager.onSelectAll(this.filterService);
    }

    handler(ev) {
        // Smooth transition on loading new items.
    }

    public onSelectionChange(item, li) {
        this.manager.onSelectionChange(item, this.filterService);
    }

    public onInput(e: any) {
        // Manage filters.
        this.filterText = e.target.value;
        this.manager.filter(this.filterText);
    }

    public filterItems(filters: DateFilters) {
        this.manager.filter(filters)
    }

    ngOnDestroy() {
        this.signal.next();
        this.signal.complete();
    }

}
