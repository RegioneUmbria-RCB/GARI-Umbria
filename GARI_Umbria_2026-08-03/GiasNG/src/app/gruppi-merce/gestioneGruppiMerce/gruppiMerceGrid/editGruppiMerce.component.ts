import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { EditGruppiMerceGridService } from './editGruppiMerceGrid.config';


@Component({
    standalone: false,
    selector: 'gruppi-merce-grid',
    templateUrl: './editGruppiMerce.component.html',
    providers: [
        ...generateGridProviders(EditGruppiMerceGridService, EditGruppiMerceGridComponent)
    ]
})
export class EditGruppiMerceGridComponent {

}
