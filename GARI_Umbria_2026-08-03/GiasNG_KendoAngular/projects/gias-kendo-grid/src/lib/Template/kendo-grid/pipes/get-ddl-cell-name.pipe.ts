import { Pipe, PipeTransform } from '@angular/core';
import { GridErrorService } from '../services/grid-log.service';
import { DropdownListItem, KendoGridColumn, KendoGridRow } from '../models/grid.model';

@Pipe({
  standalone: false,
  name: 'cellDropdownName',
  pure: false
})
export class CellDropdownNamePipe implements PipeTransform {

  constructor(public logService: GridErrorService) { }

  transform(row: KendoGridRow, column: KendoGridColumn): any {
    let ddlItem = CellDropdownNamePipe.getDdlItem(column, row);
    if (ddlItem == null && column.ddl.loadOnEdit) {
      ddlItem = { name: row[column.ddl.descriptionField] };
    }

    return ddlItem?.name;
  }

  public static getDdlItem(column: KendoGridColumn, row: KendoGridRow) {
    let ddlItem = null;

    if (column.ddl?.valuePrimitive) {
      ddlItem = column.ddl.data.find(x => row[column.ddl.formControlName] == x['id'] && row[column.ddl.descriptionField] == x['name']);
    } else {
      let value;
      try {
        value = row[column.ddl.formControlName];
      } catch (e) {
        console.log(column.field);
      }
      value = row[column.ddl.formControlName];
      if (value?.id != null) {
        value = value.id;
      }
      ddlItem = column.ddl.data.find(x => value == x['id']);
    }

    return ddlItem;
  }
}

