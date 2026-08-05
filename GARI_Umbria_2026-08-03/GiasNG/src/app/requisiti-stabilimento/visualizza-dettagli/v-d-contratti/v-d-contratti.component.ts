import { Component } from '@angular/core';
import {VDContrattiGridService} from "./v-d-contratti-grid.service";
import {VDPianoColturaleComponent} from "../v-d-piano-colturale/v-d-piano-colturale.component";
import { generateGridProviders } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-v-d-contratti',
  templateUrl: './v-d-contratti.component.html',
  styleUrls: [],
  providers: [...generateGridProviders(VDContrattiGridService, VDPianoColturaleComponent)]
})
export class VDContrattiComponent {
}
