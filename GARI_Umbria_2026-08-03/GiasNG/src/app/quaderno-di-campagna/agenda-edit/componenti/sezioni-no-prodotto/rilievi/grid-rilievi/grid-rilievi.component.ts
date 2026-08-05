import {Component } from '@angular/core';
import { GridRilieviConfigService } from './grid-rilievi.service';
import { generateGridProviders } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-grid-rilievi',
  templateUrl: './grid-rilievi.component.html',
  providers: [...generateGridProviders(GridRilieviConfigService, GridRilieviComponent)]
})
export class GridRilieviComponent {

}
