import { Component } from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {GridVisibilitaEditService} from "./grid-visibilita-edit.service";

@Component({
  standalone: false,
  selector: 'app-grid-visibilita-edit',
  templateUrl: './grid-visibilita-edit.component.html',
  styleUrls: ['./grid-visibilita-edit.component.css'],
  providers: [
    ...generateGridProviders(GridVisibilitaEditService, GridVisibilitaEditComponent),
  ]
})
export class GridVisibilitaEditComponent {
}
