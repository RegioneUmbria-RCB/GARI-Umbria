import { Inject, Pipe, PipeTransform } from '@angular/core';
import { GRID_HTTP_TOKEN, KendoGridColumn, KendoServerResult } from '../models/grid.model';
import { AbstractGridConfigService } from '../services/grid-config.service';
import { ConversionService } from 'gias-ui-kit';

@Pipe({
  standalone: false,
  name: 'stringToDate',
  pure: true
})
export class StringToDatePipe implements PipeTransform {
  constructor(
    @Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<KendoServerResult>, private conversionService: ConversionService) {
  }

  transform(strVal: any, col: KendoGridColumn): Date {
    let retVal;
    if (strVal == null) {
      retVal = col?.date?.defaultValue;
    }
    retVal = strVal;
    if (typeof strVal == 'string') {
      retVal = this.conversionService.convertStringToDate(strVal);
    }
    return retVal
  }
}
