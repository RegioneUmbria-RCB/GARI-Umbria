import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { VisualizzaDettagliGridConfigurationService } from './visualizza-dettagli-grid-configuration.service';

@Component({
  standalone: false,
  selector: 'app-v-d-piano-colturale',
  templateUrl: './v-d-piano-colturale.component.html',
  styleUrls: [],
  providers: [...generateGridProviders(VisualizzaDettagliGridConfigurationService, VDPianoColturaleComponent)]
})
export class VDPianoColturaleComponent {

}
