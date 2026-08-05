import {
    AfterViewInit, ChangeDetectorRef, Component, ElementRef, EventEmitter, Input,
    OnDestroy, Output, Renderer2, ViewEncapsulation
} from '@angular/core';
import { FilterService, PopupCloseEvent, SinglePopupService } from '@progress/kendo-angular-grid';
import { CompositeFilterDescriptor, FilterDescriptor } from '@progress/kendo-data-query';
import { Subject, takeUntil } from 'rxjs';
import { TranslocoService } from '@jsverse/transloco';
import { IManager } from '../managers/manager';
import { PopupSettingsManager, closest } from '../models';
import { filtersManager, FormValueManager, DateFilters, FILTER_MODES } from '../utils';


interface IDateRangeFilter {
    /**
     * Stores logic for handling multiple filter modes.
     * Basic filtering with data contained between two dates.
     * Extended filtering which supports all kendo operators.
     */
    filtersManager: filtersManager;

    /**
     * Used for setting the width of the popup, as well as preventing
     * the window from closing in certain situations.
     */
    popupManager: PopupSettingsManager;

    /**
     * Manages all the values within each form of the view.
     */
    formValueManager: FormValueManager;
}

@Component({
    standalone: false,
    selector: 'gias-date-range-filter-cell',
    styles: [
        `
            kendo-daterange > kendo-dateinput.range-filter {
                display: inline-block;
            }
            .k-button {
                margin-left: 5px;
            }
            .highlightOnHover:hover {
                color: #FFC000
            }
        `,
    ],
    templateUrl: './date-range-filter.component.html',
    encapsulation: ViewEncapsulation.Emulated
})
export class DateRangeFilterComponent implements IDateRangeFilter, AfterViewInit, OnDestroy {
    @Input() public filter: CompositeFilterDescriptor;
    @Input() public filterService: FilterService;
    @Input() public manager: IManager;
    @Input() public field: string;

    @Output() public filterChange: EventEmitter<DateFilters> = new EventEmitter<DateFilters>();

    // Manages the values of the view
    public formValueManager: FormValueManager;

    // Manages the two filter operating modes
    public filtersManager: filtersManager = new filtersManager();

    // Discards onClose events when the timePicker is selected
    public popupManager: PopupSettingsManager = new PopupSettingsManager();

    // Used for subscriptions
    private signal: Subject<void> = new Subject();


    constructor(
        public changeDetector: ChangeDetectorRef,
        private element: ElementRef,
        popupService: SinglePopupService,
        private transloco: TranslocoService,
        private renderer: Renderer2) {

        this.formValueManager = new FormValueManager(this.transloco);

        // Prevents the popup from closing when a datepicker date has been
        // clicked.
        popupService.onClose.pipe(takeUntil(this.signal)).subscribe((e: PopupCloseEvent) => {
            this.filtersManager.preventPopupCloseIfRequired(e);
            if (document.activeElement && closest(document.activeElement,
                node => node === this.element.nativeElement || (String(node.className).indexOf('timepicker-filter') >= 0))) {
                e.preventDefault();
            }
        });
    }


    get filters() {
        return this.filtersManager;
    }

    get values() {
        return this.formValueManager;
    }

    ngAfterViewInit(): void {
        this.popupManager.initializeFilterContainer(this.element);
        this.setFilterContainerWidth();
    }

    switchActiveMode() {
        this.filtersManager.switchActiveMode();
        this.values.resetOperators();
        // if(this.filtersManager.activeMode === FILTER_MODES.BASIC_FILTERS) {
        //     this.setFilterContainerWidth();
        // } else {
        //     this.setFilterContainerWidth();
        // }
        this.setFilterContainerWidth();
    }

    public clearFilter(): void {
        this.filterRange(null, null);
    }

    public filterRange(start: Date, end: Date): void {
        const filters: (FilterDescriptor | CompositeFilterDescriptor)[] = [];

        const startOp = this.transformOperator(this.values.firstOperator.id, start);
        if (startOp != null) {
            filters.push(startOp);
        }

        const endOp = this.transformOperator(this.values.secondOperator.id, end);
        if (endOp != null) {
            filters.push(endOp);
        }

        const root = this.filter || {
            logic: this.values.logicOperator.id,
            filters: [],
        };

        if (filters.length) {
            root.filters.push(...filters);
        }


        this.filterChange.emit({
            start: start,
            end: end,
            filterDescriptors: root
        });
    }

    transformOperator(operatorType: string, date: Date): CompositeFilterDescriptor | FilterDescriptor {

        if (operatorType === 'isnull') {
            return {
                operator: 'isnull'
            };
        }

        if (operatorType === 'isnotnull') {
            return {
                operator: 'isnotnull'
            };
        }

        if (date == null) {
            return null;
        }

        if (operatorType === 'eq') {
            return {
                logic: 'and',
                filters: [
                    { operator: 'gte', value: date.setHours(0, 0, 0, 0) },
                    { operator: 'lte', value: date.setHours(23, 59, 59, 0) }
                ]
            };
        }

        if (operatorType === 'neq') {
            return {
                logic: 'or',
                filters: [
                    { operator: 'lt', value: date.setHours(0, 0, 0, 0) },
                    { operator: 'gt', value: date.setHours(23, 59, 59, 0) }
                ]
            };
        }

        return {
            operator: operatorType,
            value: date,
        };
    }

    refreshFilters() {
        this.filterRange(this.values.start, this.values.end);
    }

    onSelectAll() {
        this.manager.onSelectAll(this.filterService);
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    private setFilterContainerWidth() {
        if (this.filtersManager.activeMode === FILTER_MODES.BASIC_FILTERS) {
            this.renderer.setStyle(this.popupManager.filterContainer, 'width', '350px');
        } else if (this.filtersManager.activeMode === FILTER_MODES.ADVANCED_FILTERS) {
            this.renderer.setStyle(this.popupManager.filterContainer, 'width', '450px');
        }
    }
}
