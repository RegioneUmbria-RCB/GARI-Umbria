import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { StatoPatrimonialeDettaglioHttpService } from './valutazioni-stato-patr-det-config.component';

@Component({
  standalone: false,
  selector: 'app-valutazioni-stato-patr-det',
  templateUrl: './valutazioni-stato-patr-det.component.html',
  styleUrls: ['./valutazioni-stato-patr-det.component.css'],
  providers: [
    ...generateGridProviders(StatoPatrimonialeDettaglioHttpService, ValutazioniStatoPatrimonialeDettaglioComponent)
  ]
})
export class ValutazioniStatoPatrimonialeDettaglioComponent {

}
