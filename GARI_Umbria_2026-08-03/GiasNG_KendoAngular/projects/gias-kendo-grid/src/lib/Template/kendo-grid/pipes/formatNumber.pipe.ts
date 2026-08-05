import { Inject, Pipe, PipeTransform } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { GRID_HTTP_TOKEN, KendoGridColumn, KendoServerResult } from '../models/grid.model';
import { AbstractGridConfigService } from '../services/grid-config.service';

@Pipe({
    standalone: false,
  name: 'formatNumber',
  pure: true
})
export class FormatNumberPipe implements PipeTransform {
  constructor(@Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<KendoServerResult>, public intl: IntlService) {
  }

  transform(val: any, col: KendoGridColumn, aggrFormat: string): string {
    if (val != undefined && val != null && val != "") {
      let val_str = val.toString();
      if (aggrFormat != undefined && aggrFormat != "") {
        val_str = this.intl.formatNumber(val, aggrFormat);
      } else if (col?.format != undefined && col?.format != "") {
        let format_str = this.pulisciFormato(col?.format);
        val_str = this.intl.formatNumber(val, format_str);
      } else if (col?.numeric?.format != undefined && col?.numeric?.format != "") {
        let format_str = this.pulisciFormato(col?.numeric?.format);
        val_str = this.intl.formatNumber(val, format_str);
      }
      return val_str;
    } else {
      return val;
    }
  }

  private pulisciFormato(format: string): string {
    let format_str = format.replace("{", "");
    format_str = format_str.replace("}", "");
    format_str = format_str.replace("0:", "");
    return format_str
  }

}
