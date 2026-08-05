import { FilterService } from "@progress/kendo-angular-grid";
import { CompositeFilterDescriptor, FilterDescriptor, filterBy, distinct } from "@progress/kendo-data-query";
import { SharedManager } from "./manager";
import { KendoGridColumn } from "../../../models/grid.model";


export class StringManager extends SharedManager {
  protected mapFilters(descriptor: CompositeFilterDescriptor) {
    return descriptor.filters.map(
      (f: FilterDescriptor) => f.value
    );
  }
  distinctRows: string[] = [];
  shownData: string[] = [];

  protected override setColumn(cols: KendoGridColumn[]) {
    this.column = null
  }

  public map(rows: any[], ordering: (rows: any) => any | null): string[] {
    return (ordering == null ? rows : ordering(rows)).map(d => d[this.field]);
  }

  public textAccessor(dataItem: any) {
    return this.isPrimitive ? dataItem : dataItem[this.textField];
  }
  public valueAccessor(dataItem: any) {
    return this.isPrimitive ? dataItem : dataItem[this.valueField];
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
    ) as string[];
  }

  protected parseValue(item: any) {
    return item;
  }

  // ! Called from the html template
  public isItemSelected(item: any): boolean {
    return this.values.some((x) => x === this.valueAccessor(item));
  }


  public onSelectAll(service: FilterService) {
    this.onSelectAllStringDropdown(service);
  }
}