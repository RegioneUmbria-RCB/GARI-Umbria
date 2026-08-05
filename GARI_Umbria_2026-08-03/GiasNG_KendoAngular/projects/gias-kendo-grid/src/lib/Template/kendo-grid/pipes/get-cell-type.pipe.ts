import { Pipe, PipeTransform } from '@angular/core';
import { KendoGridColumn } from '../models/grid.model';
import { KendoGridService } from '../services/kendo-grid.service';
import { CELL_TYPES } from 'gias-ui-kit';

@Pipe({
    standalone: false,
    name: 'cellType',
    pure: true
})
export class CellTypePipe implements PipeTransform {

    private service: KendoGridService;
    constructor(public services: KendoGridService) {
        if (Array.isArray(services)) {
            this.service = services[0];
        } else {
            this.service = services;
        }
    }


    transform(column: KendoGridColumn): any {
        const model = this.service.getModel();
        const exists = model[column.field] != null;

        if (!exists) {
            return;
        }

        switch (model[column.field].type as CELL_TYPES) {
            case CELL_TYPES.STRING:
            case CELL_TYPES.DATE:
            case CELL_TYPES.DATETIME:
            case CELL_TYPES.NUMBER:
            case CELL_TYPES.DROPDOWNLIST:
            case CELL_TYPES.MULTI_DROPDOWNLIST:
            case CELL_TYPES.CUSTOM:
            case CELL_TYPES.BOOLEAN:
                return model[column.field].type;
            default:
                return true;
        }
    }
}
