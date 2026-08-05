import { FilterService } from "@progress/kendo-angular-grid";
import { CompositeFilterDescriptor, FilterDescriptor } from "@progress/kendo-data-query";
import { Subject } from "rxjs";
import { DropdownListItem, KendoGridColumn } from "../../../models/grid.model";
import { MulticheckInput } from "../models";
import { DateFilters } from "../utils";
import { IMultiCheckFilter } from "../multicheck-filter";
import { isNullOrUndefined } from "../../../../../shared/utils";

export interface IManager {
	// ! Memorizza gli elementi e li mostra nella template.
	shownData: (string | DropdownListItem | number)[];
	store: { [key: string]: string[] };

	// ! Parsed elements of the rows from the table.
	allRows: (string | DropdownListItem | number)[];
	selectAllChecked: boolean;
	column: KendoGridColumn;

	// ! Initializzazione dei dati.
	init(inpt: Required<MulticheckInput>);
	initDataNoPagination();

	// ! Le computazioni più importanti/pesanti.
	onSelectionChange(item, service);
	filter(text: string | DateFilters | number);
	fetchNewPage();

	// ! When handling dropdown lists. For the worker.
	getColumn();

	// ! Methods called from the html template
	/**
	 * Verifica se la voce corrente è stata selezionata in precedenza.
	 * Le voci testate sono quelle prese dalla griglia come input
	 * (vedi currentFilter in multicheck-filter.component.ts).
	 * @param item la voce che (non) sarà selezionata
	 */
	isItemSelected(item): boolean;

	showAllDataIfBelowThreshold();
	distinctRowsLoaded();
	dataWasInitialized();
	onSelectAll(service: FilterService);
	textAccessor(dataItem: any);
	valueAccessor(dataItem: any);
}


export abstract class SharedManager implements IManager {
	SHOWN_THRESHOLD = 50;
	abstract shownData: (string | DropdownListItem | number)[];
	abstract distinctRows: (string | DropdownListItem | number)[];
	public allRows: (string | DropdownListItem | number)[];
	public store: { [key: string]: string[] };
	public column: KendoGridColumn;

	protected field: string;
	protected valueField: string;
	protected textField: string;
	protected values: string[];

	private curPageNum = 0;
	private shutdownPagination: Subject<void>;
	public selectAllChecked = false;

	// Per ora gestiamo solo il caso non primitive.
	protected isPrimitive = true;

	protected abstract setColumn(cols: KendoGridColumn[]);
	protected abstract mapFilters(descriptor: CompositeFilterDescriptor);
	protected abstract parseValue(item: any);
	public abstract onSelectAll(service: FilterService);

	constructor(protected comp: IMultiCheckFilter) {
		this.init(
			{
				textField: comp.textField,
				valueField: comp.valueField,
				currentFilter: comp.currentFilter,
				field: comp.field,
				allRows: comp.grid.value.data.rows,
				store: comp.gridWorker.resultsStore,
				shutdownPagination: comp.shutdownPagination,
                ordering: comp.ordering
			});
	}

	// ! Sincronizza i campi del manager con quelli della componente.
	public init(inpt: Required<MulticheckInput>) {

		this.field = inpt.field;
		this.setColumn(this.comp.grid.value.data.columns);
		this.allRows = this.map(inpt.allRows, inpt.ordering);
		this.store = inpt.store;
		this.shutdownPagination = inpt.shutdownPagination;
		this.values = this.mapFilters(inpt.currentFilter);
	}

	public getColumn(): KendoGridColumn {
		return this.column;
	}

	// ! Accessors.
	public abstract textAccessor(dataItem: any);
	public abstract valueAccessor(dataItem: any);

	// ! Mappa gli elementi della sorgente con quelli della dropdown.
	public abstract map(rows: string[], ordering: (rows: any) => any | null);

	public dataWasInitialized() {
		return this.shownData.length === 0 && this.allRows.length != 0;
	}

	public fetchNewPage() {
		const set = this.findNextDataPacket();

		if (this.dataWasInitialized()) {
			this.shownData = [...this.shownData, ...set];
		} else {
			this.shownData = [...set];
		}
		this.curPageNum++;
	}

	transitionToDistinctRows = false;

	// ! Utilizzata per mappare gli elemnenti della paginazione.
	private findNextDataPacket(): Set<string | DropdownListItem | number> {
		let set: Set<string | DropdownListItem | number> = new Set<string | DropdownListItem>();

		if (!this.transitionToDistinctRows) {
			let rows = this.allRows;
			if (!rows) {
				rows = this.distinctRows;
			}

			let [init, final] = this.getIterationLowerUpperBounds();

			if (final > rows.length) {
				final = rows.length;
				this.shutdownPagination.next();

				if (init > rows.length) {
					return new Set();
				}
			}

			set = new Set(rows.slice(init, final));
		} else { // < used when making the transition between worker data and the data
			// < displayed initially.
			let [init, final] = this.getIterationLowerUpperBounds(1);

			const distinctRows = this.distinctRows;
			if (final > distinctRows.length) {
				final = distinctRows.length;
				this.shutdownPagination.next();

				if (init > distinctRows.length) {
					return new Set();
				}
			}

			const lastElem = this.shownData[this.shownData.length - 1];

			set = new Set(this.distinctRows.slice(init, final));
			set.delete(lastElem); // < remove item used for making the transition.
			this.transitionToDistinctRows = false;
		}

		return set;
	}

	protected getIterationLowerUpperBounds(offset: number = 0) {
		const init = this.curPageNum * this.SHOWN_THRESHOLD - offset;
		const final = this.curPageNum * this.SHOWN_THRESHOLD + this.SHOWN_THRESHOLD;
		return [init, final];
	}

	public initDataNoPagination() {
		this.distinctRows = this.store[this.field];
		this.shownData = this.store[this.field];


		if (this.values.length === this.shownData.length)
			this.selectAllChecked = true;
		else
			this.selectAllChecked = false;
	}
	public showAllDataIfBelowThreshold() {
		if (this.shownData.length < this.SHOWN_THRESHOLD) {
			this.shownData = this.distinctRows;
			this.shutdownPagination.next();
		}
	}

	public distinctRowsLoaded() {
		this.distinctRows = this.store[this.field];
		this.allRows = null;      // < no need to use allRows anymore.
		this.transitionToDistinctRows = true;
	}

	abstract filter(text: string);

	public abstract isItemSelected(item);



	public onSelectionChange(item: any, filterService: FilterService) {
		this.selectOrDeselectItem(item);

		let filters: (FilterDescriptor | CompositeFilterDescriptor)[] = [];
		filters = this.values.map((value) => {
			if (isNullOrUndefined(value)) {
				return {
					field: this.field,
					operator: 'isnull',
					value
				}
			} else if (value !== '' && this['type'] === 'multi') {
				return {
					field: this.field,
					operator: 'contains',
					value
				};
			} else if (value !== '') {
				return {
					field: this.field,
					operator: 'eq',
					value
				};
			} else {
				return {
					field: this.field,
					operator: 'isempty', // < display empty items.
					value
				};
			}
		});


		filterService.filter({
			filters: filters,
			logic: 'or',

		});
	}

	protected selectOrDeselectItem(item: any) {
		if (this.values.some((x) => x === this.parseValue(item))) {
			// item unselected.
			this.values = this.values.filter((x) => x !== item);
		} else {                                   // < item selected.
			this.values.push(this.parseValue(item));
		}
	}

	protected onSelectAllStringDropdown(service: FilterService) {
		this.updateSelectedValues();

		let filters: (FilterDescriptor | CompositeFilterDescriptor)[] = [];
		filters = this.values.map((value) => {
			if (value !== '') {
				return {
					field: this.field,
					operator: 'eq',
					value
				};
			} else {
				return {
					field: this.field,
					operator: 'isempty', // < display empty items.
					value
				};
			}
		});

		service.filter({
			filters: filters,
			logic: 'or',
		});
	}

	updateSelectedValues() {
		if (!this.selectAllChecked) {
			this.shownData.forEach(s => this.values.push(this.parseValue(s)));
			this.selectAllChecked = true;
		}
		else {
			this.values = [];
			this.selectAllChecked = false;
		}
	}

}