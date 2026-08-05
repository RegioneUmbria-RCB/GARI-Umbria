import { Component } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { ProfilazioneImpreseParametriGeneraliColturaGridConfigService } from './profilazione-imprese-parametri-generali-coltura-gird-config.service';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-parametri-generali-coltura',
  templateUrl: './profilazione-imprese-parametri-generali-coltura.component.html',
  styleUrls: ['./profilazione-imprese-parametri-generali-coltura.component.scss', '../../profilazione-imprese.component.scss'],
  providers: [...generateGridProviders(ProfilazioneImpreseParametriGeneraliColturaGridConfigService, ProfilazioneImpreseParametriGeneraliColturaComponent)]
})
export class ProfilazioneImpreseParametriGeneraliColturaComponent {

  constructor() { }

}
