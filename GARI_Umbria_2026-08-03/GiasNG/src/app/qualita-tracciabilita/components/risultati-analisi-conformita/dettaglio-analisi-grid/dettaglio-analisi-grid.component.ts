import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { DettaglioAnalisiService } from './dettaglio-analisi.service';
import { SHARED_IMPORTS } from 'app/qualita-tracciabilita/qualita-tracciabilita.module';

@Component({
  selector: 'app-dettaglio-analisi-grid',
  imports: [SHARED_IMPORTS],
  templateUrl: './dettaglio-analisi-grid.component.html',
  styleUrl: './dettaglio-analisi-grid.component.scss',
  providers: [...generateGridProviders(DettaglioAnalisiService, DettaglioAnalisiGridComponent)]
})
export class DettaglioAnalisiGridComponent {

}
