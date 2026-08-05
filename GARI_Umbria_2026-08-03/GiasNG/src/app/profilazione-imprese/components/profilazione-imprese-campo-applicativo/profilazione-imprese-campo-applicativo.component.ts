import { Component, Input } from '@angular/core';
import { ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { Subject } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-campo-applicativo',
  templateUrl: './profilazione-imprese-campo-applicativo.component.html',
  styleUrls: ['./profilazione-imprese-campo-applicativo.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseCampoApplicativoComponent {
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  @Input() campoApplicativoSubject: Subject<number>;

  campoApplicativo$ = this.service.leggiNoteIntervento();

  constructor(private service: ProfilazioneImpreseService) { }
}
