import { Pipe, PipeTransform } from '@angular/core';
import { KendoGridService } from '../services/kendo-grid.service';

@Pipe({
    standalone: false,
    name: 'update-data-item'
})
export class SetDataItemPipe implements PipeTransform {

    constructor(private privateService: KendoGridService) { }

    transform(dataItem: any) {
        this.privateService.setDataItem(dataItem);
    }

}
