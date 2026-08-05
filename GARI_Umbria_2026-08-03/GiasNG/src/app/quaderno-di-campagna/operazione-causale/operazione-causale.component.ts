import { Component } from "@angular/core";
import { generateGridProviders } from 'gias-kendo-grid';
import { GridOperazioneCausaleHttpService } from "./service/grid-operazione-causale.service";

@Component({
    standalone: false,
    selector: 'app-operazione-causale',
    templateUrl: './operazione-causale.component.html',
    styleUrls: ['./operazione-causale.component.scss'],
    providers: [
        ...generateGridProviders(GridOperazioneCausaleHttpService,
                                OperazioneCausaleComponent)],
})
export class OperazioneCausaleComponent {

}