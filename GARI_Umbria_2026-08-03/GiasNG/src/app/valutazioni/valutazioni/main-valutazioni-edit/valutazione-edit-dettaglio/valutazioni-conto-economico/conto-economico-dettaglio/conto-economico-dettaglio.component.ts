import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { ContoEconomicoDettaglioConfigHttpService } from './conto-economico-dettaglio-config.service';

@Component({
  standalone: false,
  selector: 'app-conto-economico-dettaglio',
  templateUrl: './conto-economico-dettaglio.component.html',
  styleUrls: ['./conto-economico-dettaglio.component.css'],
  providers: [
    ...generateGridProviders(ContoEconomicoDettaglioConfigHttpService, ContoEconomicoDettaglioComponent)
  ]
})
export class ContoEconomicoDettaglioComponent {

}
