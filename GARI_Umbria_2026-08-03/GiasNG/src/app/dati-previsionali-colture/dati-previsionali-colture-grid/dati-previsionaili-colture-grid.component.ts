import { Component } from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {DatiPrevisionaliColtureGridConfigurationService} from './dati-previsionali-colture-grid-configuration.service';

@Component({
  standalone: false,
  selector: 'app-dati-previsionaili-colture-grid',
  templateUrl: './dati-previsionaili-colture-grid.component.html',
  styleUrls: ['./dati-previsionaili-colture-grid.component.css'],
  providers: [
    ...generateGridProviders(DatiPrevisionaliColtureGridConfigurationService, DatiPrevisionailiColtureGridComponent)
  ]
})
export class DatiPrevisionailiColtureGridComponent {

}
