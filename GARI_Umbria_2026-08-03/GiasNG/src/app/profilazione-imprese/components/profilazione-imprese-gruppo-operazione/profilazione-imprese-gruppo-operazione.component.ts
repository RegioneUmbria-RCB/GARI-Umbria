import { Component, Input, OnInit } from '@angular/core';
import { GruppoOperazione, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { map, Observable, Subject, tap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-gruppo-operazione',
  templateUrl: './profilazione-imprese-gruppo-operazione.component.html',
  styleUrls: ['./profilazione-imprese-gruppo-operazione.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseGruppoOperazioneComponent implements OnInit {
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  @Input() gruppoOperazioneSubject: Subject<string>;
  @Input() addAll: boolean = false;

  gruppoOperazioni$: Observable<GruppoOperazione[]> | null = null;

  constructor(private service: ProfilazioneImpreseService) { }

  ngOnInit(): void {
    this.gruppoOperazioni$ = this.service
      .getGruppiOperazione()
      .pipe(
        map(x => this.addAll ? [{ value: "0", text: "Tutte le operazioni" } as GruppoOperazione, ...x] : x),
        tap(types => this.gruppoOperazioneSubject.next(types[0]?.value))
      );
  }
}
