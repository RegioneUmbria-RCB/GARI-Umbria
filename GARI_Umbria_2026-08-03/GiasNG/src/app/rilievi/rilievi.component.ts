import { Component, Inject } from "@angular/core";
import { GridRilieviHttpService } from "./service/grid-rilievi-http.service";
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridComponent, PageChangeEvent } from "@progress/kendo-angular-grid";

@Component({
  standalone: false,
  selector: 'app-menu-rilievi',
  templateUrl: './rilievi.component.html',
  styleUrls: ['./rilievi.component.scss'],
  providers: [...generateGridProviders(GridRilieviHttpService,
    MenuRilieviComponent)]
})
export class MenuRilieviComponent {

  constructor(@Inject(GRID_HTTP_TOKEN) private gridRilieviHttpService: GridRilieviHttpService) { }

  // se si vorrà in futuro crare un nuovo rilievo, si dovrà implementare il metodo onNew() e il servizio GridRilieviHttpService
  // onNew() {
  //   this.gridRilieviHttpService.goToEditPage();
  // }

}