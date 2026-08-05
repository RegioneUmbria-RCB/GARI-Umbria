import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { DettaglioMagazzinoService } from './dettaglio-magazzino.service';
import { SHARED_IMPORTS } from 'app/qualita-tracciabilita/qualita-tracciabilita.module';

@Component({
  selector: 'app-dettaglio-giacenza-grid',
  imports: [SHARED_IMPORTS],
  templateUrl: './dettaglio-giacenza-grid.component.html',
  styleUrl: './dettaglio-giacenza-grid.component.css',
  providers: [...generateGridProviders(DettaglioMagazzinoService, DettaglioGiacenzaGridComponent)]
})
export class DettaglioGiacenzaGridComponent {

}
