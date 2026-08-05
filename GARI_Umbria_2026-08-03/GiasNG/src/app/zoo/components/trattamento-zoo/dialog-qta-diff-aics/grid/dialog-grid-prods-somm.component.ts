import { CommonModule } from "@angular/common";
import { Component, Inject } from "@angular/core";
import { TranslocoModule } from "@jsverse/transloco";
import { DialogGridProdsSommService, ProdottoDistribuito } from "./dialog-grid-prods-somm.service";
import { GRID_HTTP_TOKEN, generateGridProviders } from "gias-kendo-grid";

@Component({
    standalone: false,
    selector: 'app-dialog-grid-prods-somm',
    templateUrl: './dialog-grid-prods-somm.component.html',
    styleUrls: [],
    providers: []
})
export class DialogGridProdsSommComponent {
    constructor(@Inject(GRID_HTTP_TOKEN) public gridService: DialogGridProdsSommService) {}
}