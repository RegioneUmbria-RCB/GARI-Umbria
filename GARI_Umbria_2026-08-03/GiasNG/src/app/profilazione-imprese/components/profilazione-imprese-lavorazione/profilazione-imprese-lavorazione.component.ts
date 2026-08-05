import { Component, Input, OnInit } from '@angular/core';
import { Lavorazione, ProfilazioneImpreseService } from 'app/profilazione-imprese/services/profilazione-imprese.service';
import { DEFAULT_DROPDOWN_FILTER_SETTINGS } from 'app/Service/FunzioniComuni.service';
import { debounceTime, map, Observable, Subject, switchMap, tap } from 'rxjs';

@Component({
  standalone: false,
  selector: 'app-profilazione-imprese-lavorazione',
  templateUrl: './profilazione-imprese-lavorazione.component.html',
  styleUrls: ['./profilazione-imprese-lavorazione.component.scss', '../../profilazione-imprese.component.scss']
})
export class ProfilazioneImpreseLavorazioneComponent implements OnInit {
  dropdownSettings = DEFAULT_DROPDOWN_FILTER_SETTINGS;

  @Input() lavorazioneSubject: Subject<string>;
  @Input() gruppoOperazioneSubject: Subject<string>;
  @Input() addAll: boolean = false;

  lavorazioni$: Observable<Lavorazione[]> | null = null;

  constructor(private service: ProfilazioneImpreseService) { }

  ngOnInit(): void {
    this.lavorazioni$ = this.gruppoOperazioneSubject.pipe(
      debounceTime(100),
      switchMap(tipo => this.service.getLavorazioniPerTipo(tipo)),
      tap(x => x.forEach(y => y.LAV_DES = `${y.GRU_DES} - ${y.LAV_DES}`)),
      map(x => this.addAll ? [{ LAV_COD: 0, GRU_COD: 0, LAV_DES: "Tutte le lavorazioni" } as Lavorazione, ...x] : x)
    );
  }
}
