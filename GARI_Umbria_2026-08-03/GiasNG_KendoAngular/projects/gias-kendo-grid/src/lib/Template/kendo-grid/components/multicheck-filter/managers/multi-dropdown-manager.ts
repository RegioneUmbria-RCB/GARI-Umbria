import { FilterService } from "@progress/kendo-angular-grid";
import { CompositeFilterDescriptor, FilterDescriptor, filterBy, distinct } from "@progress/kendo-data-query";
import { DropdownListItem, KendoGridColumn } from "../../../models/grid.model";
import { MultiCheckFilterComponent } from "../multicheck-filter.component";
import { SharedManager } from "./manager";

export class MultiDropdownManager extends SharedManager {
  public shownData: DropdownListItem[] = [];
  public distinctRows: DropdownListItem[] = [];
  /** Definisce il tipo di Manager. Usato per determinare la modalità
   * di filtro usato nella funzione `onSelectionChange`. */
  public type: string = 'multi';

  constructor(that: MultiCheckFilterComponent) {
    super(that);
    this.setColumn(that.grid.value.data.columns);
  }

  protected mapFilters(descriptor: CompositeFilterDescriptor) {
    return descriptor.filters.map((f: FilterDescriptor) => f.value);
  }

  protected setColumn(cols: KendoGridColumn[]) {
    this.column = cols.find(c => c.field === this.field);
    this.textField = this.column.ddl.textField;
    this.valueField = this.column.ddl.valueField;
  }

  public map(rows: any[], ordering: (rows: any) => any | null) {
    const result = new Array<DropdownListItem>();
    const ddl = this.column.ddl;
    const empty = { id: '', name: '' };
    const nullItem = { id: null, name: null };
    (ordering == null ? rows : ordering(rows)).forEach(r => {
      const rowVal = r[this.field];
      if (rowVal == null) {
        result.push(nullItem);
        return;
      }
      // Filtro i DropdownListItems con il codice tra quelli scelti
      const col = ddl.data.filter(x => rowVal.includes(x.id));
      if (rowVal === '') {
        result.push(empty);
      } else if (col) {
        result.push(...col);
      } else {
        result.push(new DropdownListItem(r[ddl.formControlName], r[ddl.descriptionField]));
      }
    });
    return result;
  }

  public valueAccessor(value: DropdownListItem | string) {
    if (typeof (value) === 'object') {
      let val = value as DropdownListItem;
      return val.id;
    } else {
      return value;
    }
  }


  public textAccessor(dataItem: DropdownListItem) {
    return dataItem.name;
  }

  public override getColumn() {
    return this.column;
  }


  filter(text: string) {
    const fillterBy = filterBy(this.distinctRows, {
      operator: 'contains',
      field: this.textField,
      value: text,
    });

    const shownData = this.shownData;
    this.shownData = distinct(
      [
        ...shownData.filter((dataItem) =>
          this.values.some(
            (val) => val === this.valueAccessor(dataItem)
          )
        ),
        ...fillterBy,
      ],
      this.textField
    ) as DropdownListItem[];
  }

  protected parseValue(item: any) {
    if ((typeof item === 'string' || item instanceof String) ||
      (typeof item === 'number' || item instanceof Number))
      return item;
    else
      return item?.id;
  }


  // ! Called from the html template
  public isItemSelected(item: any): boolean {
    return this.values.some((x) => x === this.valueAccessor(item));
  }

  public onSelectAll(service: FilterService) {
    this.onSelectAllStringDropdown(service);
  }
}