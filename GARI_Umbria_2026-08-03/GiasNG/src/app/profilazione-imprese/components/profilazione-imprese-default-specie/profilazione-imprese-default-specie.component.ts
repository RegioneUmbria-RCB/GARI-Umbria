import { Component, Input, Output } from '@angular/core';
import { generateGridProviders } from 'gias-kendo-grid';
import { ProfilazioneImpreseDefaultSpecieGridConfigService } from './profilazione-imprese-default-specie-gird-config.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-default-specie',
  templateUrl: './profilazione-imprese-default-specie.component.html',
  styleUrls: ['./profilazione-imprese-default-specie.component.scss', '../../profilazione-imprese.component.scss'],
  providers: [...generateGridProviders(ProfilazioneImpreseDefaultSpecieGridConfigService, ProfilazioneImpreseDefaultSpecieComponent)]
})
export class ProfilazioneImpreseDefaultSpecieComponent {
  @Input() specieSubject: BehaviorSubject<number>;
  @Input() defaultSpecie: number;
}
