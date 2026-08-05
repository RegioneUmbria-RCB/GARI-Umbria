import { Component, Inject } from '@angular/core';
import { ProfilazioneImpreseDefaultDistintaProduzioneGridConfigService } from './profilazione-imprese-default-distinta-produzione-gird-config.service';
import { generateGridProviders } from 'gias-kendo-grid';
import { ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { GRID_HTTP_TOKEN } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-default-distinta-produzione',
  templateUrl: './profilazione-imprese-default-distinta-produzione.component.html',
  styleUrls: ['./profilazione-imprese-default-distinta-produzione.component.scss', '../../profilazione-imprese.component.scss'],
  providers: [...generateGridProviders(ProfilazioneImpreseDefaultDistintaProduzioneGridConfigService, ProfilazioneImpreseDefaultDistintaProduzioneComponent)]
})
export class ProfilazioneImpreseDefaultDistintaProduzioneComponent {
  specieSubject = this.service.defaultPianiColturaliSpecieSubject;
  defaultSpecie = 0;

  constructor(
    private service: ProfilazioneImpreseService,
    @Inject(GRID_HTTP_TOKEN) private gridService: ProfilazioneImpreseDefaultDistintaProduzioneGridConfigService
  ) { }

  addCycle(): void {
    this.gridService.addCycle();
  }
}
