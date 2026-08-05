import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { MacchineEditGerarchiaGridService } from './gerarchia-grid.service';

@Component({
  standalone: false,
  selector: 'app-macchine-edit-gerarchia',
  templateUrl: './macchine-edit-gerarchia.component.html',
  styleUrls: ['./macchine-edit-gerarchia.component.css'],
  providers:[
      ...generateGridProviders(MacchineEditGerarchiaGridService, MacchineEditGerarchiaComponent)
  ]
})

export class MacchineEditGerarchiaComponent {

}
