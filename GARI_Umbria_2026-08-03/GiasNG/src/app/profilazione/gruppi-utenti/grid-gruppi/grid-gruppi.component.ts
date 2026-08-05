import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridGruppiService } from './grid-gruppi.service';

@Component({
  standalone: false,
  selector: 'app-grid-gruppi',
  templateUrl: './grid-gruppi.component.html',
  styleUrls: ['./grid-gruppi.component.css'],
  providers: [
    ...generateGridProviders(GridGruppiService, GridGruppiComponent)
  ]
})
export class GridGruppiComponent {

}
