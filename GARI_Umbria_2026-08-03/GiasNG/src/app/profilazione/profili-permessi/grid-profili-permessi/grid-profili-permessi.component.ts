import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridProfiliPermessiService } from './grid-profili-permessi.service';

@Component({
  standalone: false,
  selector: 'app-grid-profili-permessi',
  templateUrl: './grid-profili-permessi.component.html',
  styleUrls: ['./grid-profili-permessi.component.css'],
  providers: [
    ...generateGridProviders(GridProfiliPermessiService, GridProfiliPermessiComponent)
  ]
})
export class GridProfiliPermessiComponent {

}
