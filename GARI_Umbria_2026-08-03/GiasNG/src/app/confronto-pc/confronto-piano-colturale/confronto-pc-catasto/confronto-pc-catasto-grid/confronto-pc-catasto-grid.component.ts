import { Component } from '@angular/core';
import {generateGridProviders} from 'gias-kendo-grid';
import {ConfrontoPcCatastoGridConfigService} from './confronto-pc-catasto-grid-config.service';

@Component({
  standalone: false,
  selector: 'app-confronto-pc-catasto-grid',
  templateUrl: './confronto-pc-catasto-grid.component.html',
  styleUrls: ['./confronto-pc-catasto-grid.component.scss'],
  providers:[
    ...generateGridProviders(ConfrontoPcCatastoGridConfigService, ConfrontoPcCatastoGridComponent)
  ]
})
export class ConfrontoPcCatastoGridComponent {

}
