import { Component, Input } from '@angular/core';
import { Subject } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-global',
  templateUrl: './profilazione-imprese-global.component.html',
  styleUrls: ['./profilazione-imprese-global.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseGlobalComponent {
  @Input() globalSubject: Subject<boolean> | null = null;
}
