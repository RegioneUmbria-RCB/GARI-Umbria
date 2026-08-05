import { Component } from '@angular/core'
import { generateGridProviders } from 'gias-kendo-grid';
import { GruppiUtentixGruppiMerceGridService } from './gruppiUtentixGruppiMerceGrid.config';


@Component({
    standalone: false,
    selector: 'gruppi-utentix-gruppi-merce-grid',
    templateUrl: './gruppiUtentixGruppiMerceGrid.component.html',
    providers: [
        ...generateGridProviders(GruppiUtentixGruppiMerceGridService, GruppiUtentixGruppiMerceGridComponent)
    ]
})
export class GruppiUtentixGruppiMerceGridComponent {

}
