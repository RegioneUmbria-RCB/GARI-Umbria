import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GruppiMerceGrid2Service } from './gruppiMerceGrid2.config';


@Component({
    standalone: false,
    selector: 'gruppi-merce-grid2',
    templateUrl: './gruppiMerceGrid2.component.html',
    providers: [
        ...generateGridProviders(GruppiMerceGrid2Service, GruppiMerceGrid2Component)
    ]
})
export class GruppiMerceGrid2Component {

}
