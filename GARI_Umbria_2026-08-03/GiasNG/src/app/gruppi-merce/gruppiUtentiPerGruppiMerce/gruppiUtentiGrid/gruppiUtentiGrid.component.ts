import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GruppiUtentiGridService } from './gruppiUtentiGrid.config';


@Component({
    standalone: false,
    selector: 'gruppi-utenti-grid',
    templateUrl: './gruppiUtentiGrid.component.html',
    providers: [
        ...generateGridProviders(GruppiUtentiGridService, GruppiUtentiGridComponent)
    ]
})
export class GruppiUtentiGridComponent {

}
