import {Component} from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {PianoColturaleGridConfigService} from './piano-colturale-grid-config.service';

@Component({
  standalone: false,
  selector: 'app-piano-colturale-grid',
  templateUrl: './piano-colturale-grid.component.html',
  styleUrls: ['./piano-colturale-grid.component.css'],
  providers:[
    ...generateGridProviders(PianoColturaleGridConfigService, PianoColturaleGridComponent)
  ]
})
export class PianoColturaleGridComponent {
}
