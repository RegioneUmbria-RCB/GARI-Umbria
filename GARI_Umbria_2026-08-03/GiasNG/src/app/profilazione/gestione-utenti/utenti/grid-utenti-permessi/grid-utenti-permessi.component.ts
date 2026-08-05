import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { GridUtentiPermessiService } from './grid-utenti-permessi.service';

@Component({
  standalone: false,
  selector: 'app-grid-utenti-permessi',
  templateUrl: './grid-utenti-permessi.component.html',
  styleUrls: ['./grid-utenti-permessi.component.css'],
  providers: [
    ...generateGridProviders(GridUtentiPermessiService, GridUtentiPermessiComponent)
]
})
export class GridUtentiPermessiComponent {


}
