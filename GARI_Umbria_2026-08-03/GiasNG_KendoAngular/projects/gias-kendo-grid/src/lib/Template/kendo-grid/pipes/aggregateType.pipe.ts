import { Inject, Pipe, PipeTransform } from '@angular/core';
import { GRID_HTTP_TOKEN, KendoGridColumn, KendoServerResult } from '../models/grid.model';
import { AbstractGridConfigService } from '../services/grid-config.service';
import { GiasAggregateDescriptor } from "../models/configuration.model";

@Pipe({
    standalone: false,
    name: 'aggrType',
    pure: true,
})
export class AggregateTypePipe implements PipeTransform {
    constructor(@Inject(GRID_HTTP_TOKEN) public conf: AbstractGridConfigService<KendoServerResult>) {
    }

    transform(col: KendoGridColumn): { show: boolean, descriptor?: GiasAggregateDescriptor } {
        const descriptors = this.conf.aggregates.descriptors;
        const index = descriptors.findIndex(s => s.field === col.field);

        if (index >= 0) {
            return { show: true, descriptor: descriptors[index] };
        }

        return { show: false };
    }
}
