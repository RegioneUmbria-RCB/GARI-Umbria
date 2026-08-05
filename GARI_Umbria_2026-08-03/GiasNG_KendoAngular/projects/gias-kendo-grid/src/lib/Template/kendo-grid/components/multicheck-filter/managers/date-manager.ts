import { FilterService } from "@progress/kendo-angular-grid";
import { filterBy, CompositeFilterDescriptor, FilterDescriptor, distinct } from "@progress/kendo-data-query";
import { KendoGridColumn } from "../../../models/grid.model";
import { DateFilters } from "../utils";
import { SharedManager } from "./manager";


export class DateManager extends SharedManager {
  distinctRows: string[] = [];
  shownData: string[] = [];

	public map(rows: string[], ordering: (rows: any) => any | null): string[] {
    return (ordering == null ? rows : ordering(rows)).map(d => d[this.field]);
  }

  protected override setColumn(cols: KendoGridColumn[]) {
    this.column = null;
  }

  public textAccessor(dataItem: any) {
    return this.isPrimitive ?
      dateToStr(dataItem) : dateToStr(dataItem[this.textField]);
  }
  public valueAccessor(dataItem: any) {
    return this.isPrimitive ?
      dataItem : dataItem[this.valueField];
  }

  filter(date: DateFilters | string) {

    var _filterBy = filterBy(this.distinctRows,
      (date as DateFilters).filterDescriptors);

    const shownData = this.shownData;

    const checkedRows = shownData.filter((dataItem) =>
      this.values.some((val) => val === this.valueAccessor(dataItem))
    );
    const arr = [...checkedRows, ..._filterBy];

    this.shownData = distinct(
      arr,
      this.textField
    ) as string[];
  }

  protected parseValue(item: any) {
    return item;
  }

  protected mapFilters(descriptor: CompositeFilterDescriptor) {
    return descriptor.filters.map((filter: FilterDescriptor) => {
      return filter.value;
    })
  }

  // ! Called from the html template
  public isItemSelected(item: any): boolean {
    return this.values.some((x) =>
      (x as any as Date).getTime() === (this.valueAccessor(item) as Date).getTime()
    );
  }


  public onSelectAll(service: FilterService) {
    this.updateSelectedValues();

    const filters = this.values.map((value) => {
      if (value !== '') {
        return {
          field: this.field,
          operator: 'eq',
          value
        };
      }
      return null;
    });

    service.filter({
      filters: filters,
      logic: 'or',
    });
  }
}

function dateToStr(date: any) {
  if (date instanceof Date)
    return date.toLocaleDateString() + ' - ' + date.toLocaleTimeString();
  else
    return date;
}