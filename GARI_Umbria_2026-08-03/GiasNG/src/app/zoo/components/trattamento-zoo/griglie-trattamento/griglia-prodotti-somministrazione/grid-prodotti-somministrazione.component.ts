import { Component, Inject } from "@angular/core";
import { GridProdottiSomministrazioneService } from "./service/grid-prodotto-somministrazione.service";
import { GRID_HTTP_TOKEN, generateGridProviders } from "gias-kendo-grid";

@Component({
    selector: 'app-prodotti-somministrazione',
    templateUrl: './grid-prodotti-somministrazione.component.html',
    standalone: false,
    styleUrls: [],
    providers: [...generateGridProviders(GridProdottiSomministrazioneService,
                                        GridProdottiSomministrazioneComponent)]
})
export class GridProdottiSomministrazioneComponent {
    constructor(
        @Inject(GRID_HTTP_TOKEN) private gridService: GridProdottiSomministrazioneService
    ) {}

    public get gridSelectedRows(): Array<any> {
        return this.gridService.gridpublicService.getValue().data.rows.filter((row: any) => row.Selected);
    }

}