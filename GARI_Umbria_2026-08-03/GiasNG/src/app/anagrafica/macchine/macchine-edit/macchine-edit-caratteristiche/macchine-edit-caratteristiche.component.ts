import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { MacchinaEditCaratteristicheConfigService } from './macchine-edit-caratteristiche-grid.service';

@Component({
  standalone: false,
  selector: 'app-macchine-edit-caratteristiche',
  templateUrl: './macchine-edit-caratteristiche.component.html',
  styleUrls: ['./macchine-edit-caratteristiche.component.css'],
  providers:[
      ...generateGridProviders(MacchinaEditCaratteristicheConfigService, MacchinaEditCaratteristicheComponent)
  ]
})

export class MacchinaEditCaratteristicheComponent {

}
