import { Pipe, PipeTransform } from '@angular/core';
import { KendoGridColumn } from '../models/grid.model';

@Pipe({
    standalone: false,
  name: 'groupHeaderName',
  pure: true
})
export class GroupHeaderNamePipe implements PipeTransform {

  transform(group: any, column: KendoGridColumn, field: string): any {

    let elem = null;
    let nextGroup = group;
    do {
      nextGroup = nextGroup.items[0];

      if (!Array.isArray(nextGroup.items)) {
        elem = nextGroup;
        break;
      }
    } while (true);

    let result = elem[column.ddl.descriptionField];

    return result;
  }
}

